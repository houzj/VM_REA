using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

internal static class CompareModes
{
    private static byte[,] Read(string path, out int width, out int height)
    {
        using (var bitmap = new Bitmap(path))
        {
            width = bitmap.Width; height = bitmap.Height;
            var result = new byte[height, width];
            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++) result[y, x] = bitmap.GetPixel(x, y).R;
            return result;
        }
    }

    private static int Sample(byte[,] image, int width, int height, int x, int y, string border, out bool included)
    {
        if (border == "clamp") { x = Math.Max(0, Math.Min(width - 1, x)); y = Math.Max(0, Math.Min(height - 1, y)); }
        else if (border == "reflect")
        {
            if (x < 0) x = -x - 1; if (x >= width) x = 2 * width - x - 1;
            if (y < 0) y = -y - 1; if (y >= height) y = 2 * height - y - 1;
        }
        else if (x < 0 || x >= width || y < 0 || y >= height) { included = false; return 0; }
        included = true; return image[y, x];
    }

    private static int Differences(byte[,] source, byte[,] actual, int width, int height, Func<int, int, int> threshold, Func<int, int, int, bool> white)
    {
        int differences = 0;
        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            int src = source[y, x]; int value = white(src, x, y) ? 255 : 0;
            if (value != actual[y, x]) differences++;
        }
        return differences;
    }

    private static void Emit(string mode, byte[,] source, byte[,] actual, int width, int height, Func<int, int, int> threshold, string label)
    {
        foreach (string polarity in new[] { "ge", "gt", "le", "lt" })
        {
            Func<int, int, int, bool> white = (v, x, y) => polarity == "ge" ? v >= threshold(x, y) : polarity == "gt" ? v > threshold(x, y) : polarity == "le" ? v <= threshold(x, y) : v < threshold(x, y);
            Console.WriteLine(mode + " " + label + " " + polarity + " diff=" + Differences(source, actual, width, height, threshold, white));
        }
    }

    private static Func<int, int, int> Local(byte[,] image, int width, int height, int rx, int ry, string border, string rounding, double sigma, bool sauvola, double k = .2, double range = 128)
    {
        double[] gx = null, gy = null;
        if (sigma > 0)
        {
            gx = new double[rx * 2 + 1]; gy = new double[ry * 2 + 1]; double sx = 0, sy = 0;
            for (int i = -rx; i <= rx; i++) { gx[i + rx] = Math.Exp(-i * i / (2 * sigma * sigma)); sx += gx[i + rx]; }
            for (int i = -ry; i <= ry; i++) { gy[i + ry] = Math.Exp(-i * i / (2 * sigma * sigma)); sy += gy[i + ry]; }
            for (int i = 0; i < gx.Length; i++) gx[i] /= sx;
            for (int i = 0; i < gy.Length; i++) gy[i] /= sy;
        }
        return (x, y) =>
        {
            double sum = 0, sumSq = 0, weights = 0; int count = 0;
            for (int dy = -ry; dy <= ry; dy++)
            for (int dx = -rx; dx <= rx; dx++)
            {
                bool included; int value = Sample(image, width, height, x + dx, y + dy, border, out included);
                if (!included) continue;
                double weight = sigma > 0 ? gx[dx + rx] * gy[dy + ry] : 1;
                sum += value * weight; sumSq += value * value * weight; weights += weight; count++;
            }
            double mean = sigma > 0 ? sum / weights : sum / count;
            double result = mean;
            if (sauvola)
            {
                double deviation = Math.Sqrt(Math.Max(0, (sigma > 0 ? sumSq / weights : sumSq / count) - mean * mean));
                result = mean * (1 + k * (deviation / range - 1));
            }
            if (rounding == "floor") result = Math.Floor(result);
            else if (rounding == "round") result = Math.Round(result);
            else if (rounding == "ceil") result = Math.Ceiling(result);
            return (int)Math.Floor(result);
        };
    }

    private static void SearchSauvola(byte[,] source, byte[,] actual, int width, int height)
    {
        var best = new List<KeyValuePair<string, int>>();
        foreach (int window in new[] { 3, 7, 15 })
        foreach (double k in new[] { .1, .2, .5 })
        foreach (double range in new[] { 64.0, 128.0, 255.0 })
        foreach (string border in new[] { "clamp", "crop" })
        foreach (string polarity in new[] { "ge", "gt", "le", "lt" })
        {
            var threshold = Local(source, width, height, window / 2, window / 2, border, "none", 0, true, k, range);
            Func<int, int, int, bool> white = (v, x, y) => polarity == "ge" ? v >= threshold(x, y) : polarity == "gt" ? v > threshold(x, y) : polarity == "le" ? v <= threshold(x, y) : v < threshold(x, y);
            int diff = Differences(source, actual, width, height, threshold, white);
            if (best.Count == 20 && diff >= best[19].Value) continue;
            best.Add(new KeyValuePair<string, int>("window=" + window + " k=" + k + " R=" + range + " border=" + border + " cmp=" + polarity, diff));
            best.Sort((a, b) => a.Value.CompareTo(b.Value));
            if (best.Count > 20) best.RemoveAt(20);
        }
        foreach (var item in best) Console.WriteLine("sauvola best diff=" + item.Value + " " + item.Key);
    }

    private static int Otsu(byte[,] image, int width, int height)
    {
        long[] histogram = new long[256]; double totalSum = 0; long total = (long)width * height;
        for (int y = 0; y < height; y++) for (int x = 0; x < width; x++) { int v = image[y, x]; histogram[v]++; totalSum += v; }
        long backgroundWeight = 0; double backgroundSum = 0, best = -1; int threshold = 0;
        for (int t = 0; t < 256; t++)
        {
            backgroundWeight += histogram[t]; if (backgroundWeight == 0) continue;
            long foregroundWeight = total - backgroundWeight; if (foregroundWeight == 0) break;
            backgroundSum += t * histogram[t]; double mb = backgroundSum / backgroundWeight; double mf = (totalSum - backgroundSum) / foregroundWeight;
            double between = backgroundWeight * foregroundWeight * (mb - mf) * (mb - mf);
            if (between > best) { best = between; threshold = t; }
        }
        return threshold;
    }

    private static void Main(string[] args)
    {
        int width, height; byte[,] source = Read(Path.Combine(args[0], "input.bmp"), out width, out height);
        string[] modes = { "mean", "gaussian", "sauvola" };
        foreach (string mode in modes)
        foreach (string border in new[] { "clamp", "reflect", "crop" })
        foreach (string rounding in new[] { "none", "floor", "round", "ceil" })
        {
            int rx = mode == "mean" ? 1 : mode == "gaussian" ? 2 : 7;
            int ry = mode == "mean" ? 2 : mode == "gaussian" ? 2 : 7;
            double sigma = mode == "gaussian" ? 1.2 : 0;
            var threshold = Local(source, width, height, rx, ry, border, rounding, sigma, mode == "sauvola");
            int actualWidth, actualHeight;
            byte[,] actual = Read(Path.Combine(args[0], mode + ".bmp"), out actualWidth, out actualHeight);
            if (actualWidth != width || actualHeight != height) throw new InvalidDataException(mode + " output size differs from input");
            if (mode == "mean" || mode == "gaussian")
            {
                Func<int, int, int> withOffset = (x, y) => threshold(x, y) + 7;
                Emit(mode, source, actual, width, height, withOffset, border + " " + rounding + " +7");
            }
            else Emit(mode, source, actual, width, height, threshold, border + " " + rounding);
        }
        int otsuWidth, otsuHeight;
        byte[,] otsu = Read(Path.Combine(args[0], "otsu.bmp"), out otsuWidth, out otsuHeight);
        if (otsuWidth != width || otsuHeight != height) throw new InvalidDataException("otsu output size differs from input");
        int tOtsu = Otsu(source, width, height);
        Console.WriteLine("Otsu threshold=" + tOtsu);
        Emit("otsu", source, otsu, width, height, (x, y) => tOtsu, "global");
        SearchSauvola(source, Read(Path.Combine(args[0], "sauvola.bmp"), out otsuWidth, out otsuHeight), width, height);
    }
}
