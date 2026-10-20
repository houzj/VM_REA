namespace VisionPlugins.GrayMatch;

// Exact scalar arithmetic leaves core85d17e... c9ace0/c9ad00.
// Does not implement map assembly, search, model generation, or score publication.
public static class GrayMatchScoreArithmetic
{
    // Original .rdata 1816d0200: 2d431cebe2361a3f. This is 0.0001,
    // not a typical machine epsilon or a guessed zero-denominator clamp.
    public static readonly double SourceFloor = BitConverter.Int64BitsToDouble(0x3f1a36e2eb1c432d);

    public static double Variance(double sum, double sumSquares, int count)
    {
        double product = sum * sum;
        double meanTerm = product / (double)count;
        return sumSquares - meanTerm;
    }

    public static double PositiveSquaredScore(double productSum, double correctionFactor,
        double denominatorFactor, double sum, double variance)
    {
        double correction = correctionFactor * sum;
        double numerator = productSum - correction;
        // Native COMISD zero,numerator + JNC returns floor for ordered <=0;
        // unordered takes arithmetic branch. Decompiled `0<n` loses that case.
        if (numerator <= 0.0) return SourceFloor;
        double denominatorProduct = denominatorFactor * variance;
        double squared = numerator * numerator;
        double denominator = denominatorProduct + SourceFloor;
        return squared / denominator;
    }
}
