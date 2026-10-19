using System.Buffers.Binary;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

static class ModuleFrame
{
    public static object Inspect(ZipArchiveEntry entry)
    {
        if (entry.Length > 128 * 1024 * 1024) return new { status = "unsupported-size" };
        using var input = entry.Open();
        using var memory = new MemoryStream(); input.CopyTo(memory);
        byte[] bytes = memory.ToArray(); int offset = 0;
        int ReadInt() { if (offset > bytes.Length - 4) throw new InvalidDataException("Truncated integer."); int v = BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(offset,4)); offset += 4; return v; }
        byte[] Read(int size) { if (size < 0 || size > bytes.Length - offset) throw new InvalidDataException("Invalid record length."); var v = bytes.AsSpan(offset,size).ToArray(); offset += size; return v; }
        try
        {
            int version = ReadInt(), count = ReadInt();
            if (version != 5 || count < 0 || count > 10000) throw new InvalidDataException("Unrecognized frame header.");
            var records = new List<object>();
            for (int i=0; i<count; i++)
            {
                int start = offset;
                byte[] nameSlot = Read(256);
                string name = new UnicodeEncoding(false,false,true).GetString(nameSlot).TrimEnd('\0');
                var payload = Read(ReadInt());
                object? parameters = null;
                if (name.EndsWith(".mdata",StringComparison.Ordinal)) parameters = InspectParameters(payload);
                records.Add(new { name, nameSlotBase64 = Convert.ToBase64String(nameSlot), offset = start, bytes = payload.Length, sha256 = Convert.ToHexString(SHA256.HashData(payload)).ToLowerInvariant(), parameters });
            }
            return new { status = offset == bytes.Length ? "parsed-container" : "partial-container", version, count, records, trailingBytes = bytes.Length-offset, limitations = new[] { "Observed version 5 container layout; sample-bound inference, not an official format specification.", "Parameter labels and zero-tag text values are observations. Tag semantics and trailing parameter payloads remain unknown.", "Bindings, runtime policies, ROI and embedded resources are not executed or reconstructed." } };
        }
        catch (Exception ex) when (ex is InvalidDataException or DecoderFallbackException) { return new { status = "unsupported", reason = ex.Message }; }
    }
    private static object InspectParameters(byte[] payload)
    {
        if (payload.Length < 4) return new { status = "unsupported" };
        int count = BinaryPrimitives.ReadInt32BigEndian(payload);
        if (count < 0 || count > (payload.Length-4)/1284) return new { status = "unsupported", reason = "Fixed parameter table does not fit." };
        var items = new List<object>();
        try
        {
            for(int i=0; i<count; i++)
            {
                int at=4+i*1284;
                string key = new UTF8Encoding(false,true).GetString(payload,at,256).TrimEnd('\0');
                int tag = BinaryPrimitives.ReadInt32BigEndian(payload.AsSpan(at+256,4));
                var raw = payload.AsSpan(at+260,1024);
                string? text = tag == 0 ? new UTF8Encoding(false,true).GetString(raw).TrimEnd('\0') : null;
                items.Add(new { key, keySlotBase64 = Convert.ToBase64String(payload,at,256), tag, text, rawBase64 = Convert.ToBase64String(raw) });
            }
            int tail=4+count*1284;
            return new { status = "parsed-fixed-table", count, items, unknownTailBytes = payload.Length-tail, unknownTailBase64 = Convert.ToBase64String(payload.AsSpan(tail)) };
        }
        catch(DecoderFallbackException ex) { return new { status = "unsupported", reason = ex.Message }; }
    }
}
