"""Bounded source inventory of framework WPF input registrations.

Reads decompiled text only. Hits are candidates, not proof of runtime binding.
"""
import argparse
import hashlib
import json
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
TREES = (
    "VisionMaster.Exe.Audit",
    "VisionMasterMain",
    "VM.Controls",
    "VMControls.WPF",
    "VMControls.WPF.Release",
)
PATTERNS = {
    "binding": re.compile(r"\b(?:KeyBinding|MouseBinding|KeyGesture|InputBindings|RegisterClassInputBinding|RegisterClassCommandBinding)\b"),
    "keyboard_hook": re.compile(r"\b(?:PreviewKeyDown|PreviewKeyUp|KeyDown|KeyUp|PreviewTextInput)\b|\bInputManager\.Current\b"),
    "key_test": re.compile(r"\b(?:Key\.[A-Z][A-Za-z0-9_]*|Keyboard\.(?:Modifiers|IsKeyDown)|ModifierKeys\.)"),
}

def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()

def main():
    p = argparse.ArgumentParser()
    p.add_argument("--output", required=True)
    args = p.parse_args()
    out = Path(args.output).resolve()
    out.parent.mkdir(parents=True, exist_ok=True)
    files = []
    hits = []
    for tree in TREES:
        base = ROOT / "analysis" / "decompiled" / tree
        for path in sorted(base.rglob("*")):
            if path.suffix.lower() not in (".cs", ".xaml"):
                continue
            if "obj" in path.relative_to(base).parts:
                continue
            relative = path.relative_to(ROOT).as_posix()
            files.append({"path": relative, "sha256": sha(path)})
            for number, line in enumerate(path.read_text(encoding="utf-8-sig", errors="replace").splitlines(), 1):
                kinds = [name for name, pattern in PATTERNS.items() if pattern.search(line)]
                if kinds:
                    hits.append({"path": relative, "line": number, "kinds": kinds, "source": line.strip()})
    result = {
        "scope": "Five named decompiled framework trees, .cs/.xaml only; candidate index, no runtime claim",
        "trees": TREES,
        "fileCount": len(files),
        "hitCount": len(hits),
        "hitKinds": {kind: sum(kind in hit["kinds"] for hit in hits) for kind in PATTERNS},
        "files": files,
        "hits": hits,
        "limitations": ["BAML not parsed by this probe", "Other vendor assemblies and native hooks not included", "Lexical hit is not a verified routed event or shortcut"],
    }
    out.write_text(json.dumps(result, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    print(json.dumps({key: result[key] for key in ("fileCount", "hitCount", "hitKinds")}, ensure_ascii=False))

if __name__ == "__main__":
    main()
