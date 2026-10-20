using VisionPlugins.Calibration;

namespace VisionPlugins.DistortionCalibration;

/// <summary>Computes the same inverse-projection residual direction used by the calibration
/// solver, for the point set returned by the SDK's corrected-location branch.</summary>
public static class DistortionCalibrationPointErrorEstimator
{
    public static (uint ErrorCode, float EvaluationError, float[] PointErrors) Estimate(
        IReadOnlyList<DistortionCalibrationPoint> points, int freedom, int weight, float weightFactor)
    {
        if (points.Count < 4 || freedom is < 1 or > 3 || weight is < 1 or > 3 || !float.IsFinite(weightFactor))
            return (0x10103002, float.NaN, []);

        var pairs = points.Select(p => new NPointPair(p.X, p.Y, p.GridX, p.GridY)).ToArray();
        var fit = weight == 1
            ? NPointSolver.FitNativeLeastSquaresPoints(pairs.Select(Pair).ToArray(), freedom)
            : NPointSolver.FitNativePoints(pairs.Select(Pair).ToArray(), freedom, weight, weightFactor * .1f, 20, 1e-4f);
        if (fit.ErrorCode != 0 || fit.Matrix is not { Length: 9 } matrix)
            return (fit.ErrorCode == 0 ? 0x10103002u : fit.ErrorCode, float.NaN, []);
        var nativePairs = pairs.Select(p => ((float)p.ImageX, (float)p.ImageY, (float)p.WorldX, (float)p.WorldY)).ToArray();
        var analysis = weight == 1
            ? NPointSolver.AnalyzeNativeLeastSquaresMatrix(nativePairs, matrix)
            : NPointSolver.AnalyzeNativeIrlsMatrix(nativePairs, matrix);
        return (analysis.ErrorCode, analysis.MeanInverseError, analysis.PointErrors ?? []);

        static (float ImageX, float ImageY, float WorldX, float WorldY) Pair(NPointPair p) =>
            ((float)p.ImageX, (float)p.ImageY, (float)p.WorldX, (float)p.WorldY);
    }
}
