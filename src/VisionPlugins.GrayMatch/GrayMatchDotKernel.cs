namespace VisionPlugins.GrayMatch;

public enum GrayMatchDotBackend { ScalarUnsignedTotal, SseUnsignedTotal, AvxDoubleRows }
public readonly record struct GrayMatchDotRow(int SourceOffset, int TemplateOffset, int Length);

// Source core85d17e... c96130 dispatch and ca0310/c9fa70/c9ff70 arithmetic.
// This raw dot leaf does not implement normalization, candidate extraction, or matching.
public static class GrayMatchDotKernel
{
    public static GrayMatchDotBackend Select(int sourceCpuClass, int rowMetric, int rowStride)
    {
        if (rowStride <= 0) throw new ArgumentOutOfRangeException(nameof(rowStride));
        if (sourceCpuClass == 11 && rowMetric / rowStride > 32)
            return GrayMatchDotBackend.AvxDoubleRows;
        return sourceCpuClass is 7 or 11 ? GrayMatchDotBackend.SseUnsignedTotal : GrayMatchDotBackend.ScalarUnsignedTotal;
    }

    public static double Compute(ReadOnlySpan<byte> source, ReadOnlySpan<byte> template,
        ReadOnlySpan<GrayMatchDotRow> rows, GrayMatchDotBackend backend)
    {
        if (!Enum.IsDefined(backend)) throw new ArgumentOutOfRangeException(nameof(backend));
        ulong unsignedTotal = 0;
        double doubleTotal = 0;
        foreach (var row in rows)
        {
            // Independent managed boundary for malformed native pointers/ranges.
            if (row.Length < 0 || row.SourceOffset < 0 || row.TemplateOffset < 0 ||
                row.SourceOffset > source.Length - row.Length || row.TemplateOffset > template.Length - row.Length)
                throw new ArgumentOutOfRangeException(nameof(rows));
            int sum = 0;
            for (int i = 0; i < row.Length; i++)
                sum = unchecked(sum + source[row.SourceOffset + i] * template[row.TemplateOffset + i]);
            // Source SIMD lane regrouping is associative modulo2^32; row reduction
            // then sign-extension/conversion is preserved, including overflow.
            if (backend == GrayMatchDotBackend.AvxDoubleRows)
                doubleTotal += (double)sum;
            else
                unsignedTotal = unchecked(unsignedTotal + unchecked((ulong)(long)sum));
        }
        return backend == GrayMatchDotBackend.AvxDoubleRows ? doubleTotal : (double)unsignedTotal;
    }
}
