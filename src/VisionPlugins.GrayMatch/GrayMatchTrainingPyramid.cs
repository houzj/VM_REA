namespace VisionPlugins.GrayMatch;

// 1390 c97090/c96f20/d46a10/d472b0. Owned representation, not serialized vendor model.
public static class GrayMatchTrainingPyramid
{
    public sealed record Layer(int Width, int Height, int SourceRelativeOffset, byte[] Pixels);
    public static IReadOnlyList<Layer> Build(ReadOnlySpan<byte> input, int width, int height,
        int stride, int normalizedRequestedLevels, bool mask)
    {
        // Explicit bounded managed domain, not native bad-pointer/overflow parity.
        // Caller already normalized training raw0->8 (befdc0); search supplies its own count.
        if (width <= 0 || height <= 0 || stride < width || normalizedRequestedLevels is < 1 or > 8 ||
            (long)(height - 1) * stride + width > input.Length)
            throw new ArgumentException("Outside supported training pyramid input domain.");
        int maxLevels = 1, nextW = (width + 1) >> 1, nextH = (height + 1) >> 1;
        while ((long)nextW * nextH >= 32 && nextW > 1 && nextH > 1)
        {
            maxLevels++; nextW = (nextW + 1) >> 1; nextH = (nextH + 1) >> 1;
        }
        int count = Math.Min(normalizedRequestedLevels, maxLevels);
        var levels = new List<Layer>(count);
        byte[] current = new byte[checked(width * height)];
        // c97090 first calls6e7ca0: downstream stride is tight current width.
        for (int y = 0; y < height; y++) input.Slice(y * stride, width).CopyTo(current.AsSpan(y * width));
        int offset = 0x80;
        for (int level = 0; level < count; level++)
        {
            levels.Add(new Layer(width, height, offset, current));
            offset = checked(offset + ((current.Length + 127) & ~127));
            if (level + 1 == count) break;
            int ow = (width + 1) >> 1, oh = (height + 1) >> 1;
            byte[] result = new byte[checked(ow * oh)];
            for (int y = 0; y < oh; y++) for (int x = 0; x < ow; x++)
            {
                int yy = y * 2, xx = x * 2, p = yy * width + xx;
                int value;
                if (mask) value = current[p];
                else if (xx + 1 < width && yy + 1 < height)
                    value = (current[p] + current[p + 1] + current[p + width] + current[p + width + 1]) >> 2;
                else if (xx + 1 < width) value = (current[p] + current[p + 1]) >> 1;
                else if (yy + 1 < height) value = (current[p] + current[p + width]) >> 1;
                else value = current[p];
                result[y * ow + x] = (byte)value;
            }
            current = result; width = ow; height = oh;
        }
        return levels.AsReadOnly();
    }
}
