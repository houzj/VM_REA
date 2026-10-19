"""Read-only exact PE exception bounds and MSVC instruction export, no target run."""
from pathlib import Path
import hashlib
import json
import re
import struct
import subprocess
import uuid

ROOT = Path(__file__).resolve().parents[3]
TOOL = Path("C:/Program Files/Microsoft Visual Studio/18/Community/VC/Tools/MSVC/14.51.36231/bin/Hostx64/x64/dumpbin.exe")


def sha(data):
    return hashlib.sha256(data).hexdigest()


def main():
    out = ROOT / "analysis/full-audit/batch8-20261019/B05" / ("instructions-" + uuid.uuid4().hex)
    out.mkdir(parents=True)
    previous = json.loads((ROOT / "analysis/full-audit/batch1-20261019/D01/native-coordinates.json").read_text(encoding="utf-8-sig"))
    rows = []
    for artifact, name in [(previous[0], "IMVS_SetBinaryData"), (previous[0], "InternalSetBinaryData_66f090")]:
        path = Path(artifact["path"])
        data = path.read_bytes()
        assert sha(data) == artifact["sha256"].lower(), "Original identity changed"
        pe = struct.unpack_from("<I", data, 0x3c)[0]
        assert data[pe:pe + 4] == b"PE\0\0"
        sections_count = struct.unpack_from("<H", data, pe + 6)[0]
        optional_size = struct.unpack_from("<H", data, pe + 20)[0]
        opt = pe + 24
        assert struct.unpack_from("<H", data, opt)[0] == 0x20b
        base = struct.unpack_from("<Q", data, opt + 24)[0]
        sections = []
        for i in range(sections_count):
            section = opt + optional_size + 40 * i
            size, va, raw_size, raw = struct.unpack_from("<IIII", data, section + 8)
            sections.append((va, size, raw, raw_size))

        def offset(rva):
            for va, size, raw, raw_size in sections:
                if va <= rva < va + max(size, raw_size):
                    assert rva - va < raw_size, "RVA is not file backed"
                    return raw + rva - va
            raise ValueError("Unmapped RVA")

        export_rva = struct.unpack_from("<I", data, opt + 112)[0]
        fields = struct.unpack_from("<IIHHIIIIIII", data, offset(export_rva))
        count, functions_rva, names_rva, ordinals_rva = fields[7], fields[8], fields[9], fields[10]
        found = []
        for ni in range(count):
            nr = struct.unpack_from("<I", data, offset(names_rva) + 4 * ni)[0]
            no = offset(nr)
            ename = data[no:data.index(b"\0", no)].decode("ascii")
            if ename == name:
                ordinal = struct.unpack_from("<H", data, offset(ordinals_rva) + 2 * ni)[0]
                found.append(struct.unpack_from("<I", data, offset(functions_rva) + 4 * ordinal)[0])
        if name == "InternalSetBinaryData_66f090":
            found = [0x66f090]  # Exact direct call at IMVS_SetBinaryData VA 1806472e3.
        assert len(found) == 1
        entry = found[0]
        literals = []
        for ap in previous:
            bd = Path(ap["path"]).read_bytes()
            assert sha(bd) == ap["sha256"].lower()
            for key in ("GlobalValueImport", "GlobalValueExport"):
                for encoding in ("ascii", "utf-16-le"):
                    pattern = key.encode(encoding)
                    pos = 0
                    while True:
                        pos = bd.find(pattern, pos)
                        if pos < 0: break
                        literals.append({"path": ap["path"], "sha256": sha(bd), "key": key, "encoding": encoding, "fileOffset": hex(pos), "note": "literal presence only, not a consumer/xref"})
                        pos += len(pattern)
        (out / "key-literals.json").write_text(json.dumps(literals, ensure_ascii=False, indent=2), encoding="utf-8")
        if name == "InternalSetBinaryData_66f090":
            constants = []
            for rv in (0xac7c14, 0xac7c20, 0xac7cb0, 0xac7c70):
                at = offset(rv)
                raw = data[at:data.index(b"\0", at)]
                constants.append({"rva": hex(rv), "fileOffset": hex(at), "bytesHex": raw.hex(), "ascii": raw.decode("ascii", errors="replace")})
            (out / "direct-constants.json").write_text(json.dumps(constants, indent=2), encoding="utf-8")
        exception_rva, exception_size = struct.unpack_from("<II", data, opt + 112 + 3 * 8)
        assert exception_size % 12 == 0
        table = offset(exception_rva)
        bounds = [struct.unpack_from("<III", data, table + i) for i in range(0, exception_size, 12)]
        matches = [(begin, end, unwind) for begin, end, unwind in bounds if begin <= entry < end]
        assert len(matches) == 1
        def unwind_info(record):
            u = offset(record[2])
            version_flags, _, codes, _ = struct.unpack_from("<BBBB", data, u)
            assert version_flags & 7 == 1, "Unsupported unwind version"
            flags = version_flags >> 3
            tail = u + ((4 + 2 * codes + 3) & ~3)
            parent = struct.unpack_from("<III", data, tail) if flags & 4 else None
            assert not (flags & 4 and flags & 3), "Invalid chained/handler flags"
            return flags, parent

        def primary(record):
            seen = set()
            while True:
                assert record not in seen, "Unwind cycle"
                seen.add(record)
                _, parent = unwind_info(record)
                if parent is None:
                    return record
                record = parent

        owner = primary(matches[0])
        # Compare the entire parent RUNTIME_FUNCTION tuple, never just UnwindInfo RVA.
        regions = sorted(record for record in bounds if primary(record) == owner)
        branches, region_rows, combined = [], [], []
        for i, (begin, end, unwind) in enumerate(regions):
            assert 0 < end - begin < 0x10000
            command = [str(TOOL), "/NOLOGO", "/DISASM:BYTES", f"/RANGE:0x{base + begin:x},0x{base + end - 1:x}", str(path)]
            result = subprocess.run(command, stdout=subprocess.PIPE, stderr=subprocess.PIPE, timeout=60)
            stem = name + ".region" + str(i)
            (out / (stem + ".assembly.txt")).write_bytes(result.stdout)
            (out / (stem + ".stderr.txt")).write_bytes(result.stderr)
            assert result.returncode == 0, "MSVC export failed; output retained"
            text = result.stdout.decode("utf-8", errors="replace")
            instructions = [(int(m.group(1), 16), m.group(2)) for m in re.finditer(r"^\s*([0-9A-Fa-f]{16}):\s*(.*)$", text, re.M)]
            assert instructions and all(base + begin <= address < base + end for address, _ in instructions)
            branches.extend({"va": hex(address), "instruction": ins} for address, ins in instructions if re.search(r"\b(call|jmp)\b", ins))
            raw_bytes = data[offset(begin):offset(begin) + end - begin]
            (out / (stem + ".bin")).write_bytes(raw_bytes)
            flags, parent = unwind_info((begin, end, unwind))
            region_rows.append({"beginRva": hex(begin), "endRvaExclusive": hex(end), "unwindRva": hex(unwind),
                                "flags": flags, "parent": [hex(x) for x in parent] if parent else None,
                                "instructionCount": len(instructions), "bytesSha256": sha(raw_bytes), "command": command})
            combined.append(text)
        (out / (name + ".assembly.txt")).write_text("\n".join(combined), encoding="utf-8")
        rows.append({"path": str(path), "sha256Before": sha(data), "sha256After": sha(path.read_bytes()),
                     "export": name, "entryRva": hex(entry), "imageBase": hex(base),
                     "primaryRuntimeFunction": [hex(x) for x in owner], "regions": region_rows,
                     "instructionCount": sum(x["instructionCount"] for x in region_rows), "branches": branches,
                     "limitations": "Primary/chained runtime regions only; language-specific funclets, callees, native execution and writer-format semantics remain unknown"})
        assert rows[-1]["sha256Before"] == rows[-1]["sha256After"]
    report = {"authority": "original PE bytes + MSVC dumpbin instruction output", "tool": str(TOOL),
              "toolSha256": sha(TOOL.read_bytes()), "functions": rows}
    (out / "functions.json").write_text(json.dumps(report, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    print(str(out))
    for row in rows:
        print(row["export"], "regions", len(row["regions"]), "instructions", row["instructionCount"])


if __name__ == "__main__":
    main()
