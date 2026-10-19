using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;
using System.Globalization;

static class UiParameters
{
    public static object Inspect(ZipArchiveEntry entry)
    {
        if (entry.Length > 1048576) return new { path = entry.FullName, status = "unsupported-size", records = Array.Empty<object>() };
        using var stream = entry.Open();
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        var bytes = memory.ToArray();
        int offset = 0;
        int ReadInt()
        {
            if (offset > bytes.Length - 4) throw new InvalidDataException("Truncated integer.");
            int value = BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(offset, 4)); offset += 4; return value;
        }
        byte[] ReadPayload(int length)
        {
            if (length < 0 || length > bytes.Length - offset) throw new InvalidDataException("Invalid record length.");
            var value = bytes.AsSpan(offset, length).ToArray(); offset += length; return value;
        }
        try
        {
            if (ReadInt() != 0x66553322) throw new InvalidDataException("Unknown magic.");
            int version = ReadInt();
            if (version != 1) throw new InvalidDataException("Unknown version.");
            int count = ReadInt();
            if (count < 0 || count > 10000) throw new InvalidDataException("Invalid record count.");
            var records = new List<object>();
            for (int index = 0; index < count; index++)
            {
                string key = new UTF8Encoding(false, true).GetString(ReadPayload(ReadInt()));
                byte[] value = ReadPayload(ReadInt());
                object? decoded = key == "Position" && value.Length == 16
                    ? new { x = BitConverter.Int64BitsToDouble(BinaryPrimitives.ReadInt64LittleEndian(value.AsSpan(0, 8))), y = BitConverter.Int64BitsToDouble(BinaryPrimitives.ReadInt64LittleEndian(value.AsSpan(8, 8))) }
                    : key == "IsReady" && value.Length == 1 ? value[0] != 0
                    : key == "ProceZoomIndex" ? DecodeZoom(value) : null;
                records.Add(new { key, bytes = value.Length, rawBase64 = Convert.ToBase64String(value), decoded });
            }
            return new { path = entry.FullName, status = offset == bytes.Length ? "parsed" : "partial-trailing-bytes", version, records, trailingBytes = bytes.Length - offset };
        }
        catch (Exception ex) when (ex is InvalidDataException or DecoderFallbackException)
        {
            return new { path = entry.FullName, status = "unsupported", reason = ex.Message, rawBase64 = Convert.ToBase64String(bytes) };
        }
    }

    static object DecodeZoom(byte[] value)
    {
        // VmModuleSetDecorator.ProceZoomIndex -> StringUtil.UTF8GetString:
        // trim terminal NULs, then parse an integer. Never clamp to UI ticks or
        // replace malformed persisted data with the decorator's missing-key default.
        try
        {
            string text = new UTF8Encoding(false, true).GetString(value).TrimEnd('\0');
            return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int index)
                ? new { status = "decoded-integer", text, index = (int?)index,
                    interpretation = "Original property integer; 0..19 UI tick validity is separate." }
                : new { status = "invalid-integer", text, index = (int?)null,
                    interpretation = "Original getter parses rather than defaulting malformed bytes." };
        }
        catch (DecoderFallbackException)
        {
            return new { status = "invalid-utf8", interpretation = "Strict inspector decoding; original Encoding.UTF8 uses replacement fallback. Raw bytes preserved." };
        }
    }
}
