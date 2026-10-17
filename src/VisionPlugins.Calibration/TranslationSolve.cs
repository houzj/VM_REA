namespace VisionPlugins.Calibration;

/// <summary>Translation module's source-confirmed optional world-translation compensation.</summary>
public static class TranslationSolve
{
    public static int RightCoordinate(IReadOnlyList<double> matrix) =>
        (float)((double)(float)matrix[0] * (float)matrix[4] -
            (double)(float)matrix[1] * (float)matrix[3]) > 0 ? 1 : -1;

    public static NPointSolveResult Solve(IReadOnlyList<NPointPair> offsets,
        IReadOnlyList<NPointRotation> rotations, int calibrationType, bool union, NPointSolveOptions options)
    {
        if (!union || calibrationType == 0 || rotations.Count == 0)
            return NPointSolver.Solve(offsets, rotations, options);
        var translation = NPointSolver.Solve(offsets, [], options);
        if (translation.ErrorCode != 0) return translation;
        var inverse = Inverse(translation.Matrix);
        if (inverse is null) return new() { Status = 1, ErrorCode = 0xe0000002 };
        var anchor = rotations[(rotations.Count - 1) / 2];
        var compensated = rotations.Select(p => p with
        {
            ImageX = (float)((float)p.ImageX + (float)((float)(anchor.WorldX - p.WorldX) * (float)inverse[0]) +
                (float)((float)(anchor.WorldY - p.WorldY) * (float)inverse[1])),
            ImageY = (float)((float)p.ImageY + (float)((float)(anchor.WorldX - p.WorldX) * (float)inverse[3]) +
                (float)((float)(anchor.WorldY - p.WorldY) * (float)inverse[4])),
            WorldX = anchor.WorldX, WorldY = anchor.WorldY
        }).ToArray();
        return NPointSolver.Solve(offsets, compensated, options);
    }

    static double[]? Inverse(IReadOnlyList<double> h)
    {
        var p = h.Select(v => (float)v).ToArray();
        float determinant = (((p[0]*p[4]*p[8] + p[1]*p[5]*p[6] + p[2]*p[3]*p[7]) -
            p[0]*p[5]*p[7]) - p[1]*p[3]*p[8]) - p[2]*p[4]*p[6];
        // Original MatrixInv returns success and leaves the zero buffer below its 1e-7 threshold.
        if (MathF.Abs(determinant) <= 1e-7f) return new double[9];
        if (!float.IsFinite(determinant)) return null;
        float reciprocal = 1f / determinant;
        return new[] { (p[4]*p[8]-p[7]*p[5])*reciprocal, (p[2]*p[7]-p[1]*p[8])*reciprocal,
            (p[1]*p[5]-p[2]*p[4])*reciprocal, (p[5]*p[6]-p[8]*p[3])*reciprocal,
            (p[8]*p[0]-p[2]*p[6])*reciprocal, (p[2]*p[3]-p[0]*p[5])*reciprocal,
            (p[3]*p[7]-p[6]*p[4])*reciprocal, (p[6]*p[1]-p[0]*p[7])*reciprocal,
            (p[0]*p[4]-p[1]*p[3])*reciprocal }.Select(v => (double)v).ToArray();
    }
}
