using System.Buffers.Binary;
using VisionCore;
namespace VisionIO;

/// <summary>Independent BI_RGB BMP reader. RGB luminance uses rounded (299R+587G+114B)/1000; alpha is ignored.</summary>
public static class BmpReader
{
    public const int MaxPixels = 64 * 1024 * 1024;
    private const int MaxHeaderBytes = 1024 * 1024;
    public static GrayImage Read(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if (!stream.CanRead) throw new ArgumentException("Stream must be readable.", nameof(stream));
        byte[] header = new byte[54];
        Exact(stream, header);
        if (header[0] != 'B' || header[1] != 'M') throw Bad("Not a BMP file.");
        uint fileSize = U32(header, 2), offset = U32(header, 10), dib = U32(header, 14);
        if (dib < 40 || dib > MaxHeaderBytes || offset < 14L + dib || offset > MaxHeaderBytes) throw Bad("Unsupported or invalid BMP header.");
        int width = I32(header, 18), signedHeight = I32(header, 22);
        ushort bits = U16(header, 28);
        if (width <= 0 || signedHeight == 0 || signedHeight == int.MinValue || U16(header, 26) != 1) throw Bad("Invalid dimensions or planes.");
        int height = Math.Abs(signedHeight);
        long pixels = (long)width * height;
        if (pixels > MaxPixels) throw Bad("BMP exceeds pixel resource limit.");
        if (bits != 8 && bits != 24 && bits != 32) throw Bad("Only 8, 24 and 32 bit BMP supported.");
        if (U32(header, 30) != 0) throw Bad("Compressed and bitfield BMP unsupported.");
        long strideLong = (((long)width * bits + 31) / 32) * 4;
        long requiredSize = offset + strideLong * height;
        if (strideLong > MaxPixels * 4L || (fileSize != 0 && fileSize < requiredSize)) throw Bad("Invalid BMP file size.");
        uint declaredImageSize = U32(header, 34);
        if (declaredImageSize != 0 && declaredImageSize != strideLong * height) throw Bad("Invalid BMP image size.");
        byte[] metadata = new byte[(int)offset - 54];
        Exact(stream, metadata);
        byte[]? palette = null;
        if (bits == 8)
        {
            uint colors = U32(header, 46);
            if (colors == 0) colors = 256;
            if (colors > 256) throw Bad("Invalid palette size.");
            int paletteStart = checked((int)dib - 40);
            if (paletteStart + colors * 4L > metadata.Length) throw Bad("Missing or truncated palette.");
            palette = new byte[(int)colors];
            for (int i = 0; i < palette.Length; i++)
            {
                int at = paletteStart + i * 4;
                palette[i] = Gray(metadata[at + 2], metadata[at + 1], metadata[at]);
            }
        }
        byte[] data = new byte[(int)pixels], row = new byte[(int)strideLong];
        for (int r = 0; r < height; r++)
        {
            Exact(stream, row);
            int y = signedHeight < 0 ? r : height - 1 - r;
            for (int x = 0; x < width; x++)
            {
                byte value;
                if (palette is not null)
                {
                    if (row[x] >= palette.Length) throw Bad("Pixel references missing palette entry.");
                    value = palette[row[x]];
                }
                else
                {
                    int at = x * (bits / 8);
                    value = Gray(row[at + 2], row[at + 1], row[at]);
                }
                data[y * width + x] = value;
            }
        }
        return new GrayImage(width, height, data);
    }
    private static byte Gray(byte r, byte g, byte b) => (byte)((299 * r + 587 * g + 114 * b + 500) / 1000);
    private static void Exact(Stream stream, byte[] data)
    {
        try { stream.ReadExactly(data); }
        catch (EndOfStreamException e) { throw new InvalidDataException("Truncated BMP.", e); }
    }
    private static uint U32(byte[] b, int p) => BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(p));
    private static int I32(byte[] b, int p) => BinaryPrimitives.ReadInt32LittleEndian(b.AsSpan(p));
    private static ushort U16(byte[] b, int p) => BinaryPrimitives.ReadUInt16LittleEndian(b.AsSpan(p));
    private static InvalidDataException Bad(string message) => new(message);
}
