namespace VisionPlugins.Calibration;

public readonly record struct RotateCalibrationSample(float ImageX, float ImageY, float WorldAngle);
public sealed record RotateCalibrationResult(uint ErrorCode, double ImageX, double ImageY,
    double WorldX, double WorldY, double WorldError, double PixelError);

/// <summary>Independent reconstruction of RotateCalibModu CalibRot (RVA 392e0).
/// Original uses only the first three samples to fit, then all samples for its mean error.
/// This deliberately differs from the N-point solver and from a general circle fit.</summary>
public static class RotateCalibrationSolver
{
    public static RotateCalibrationResult Solve(IReadOnlyList<RotateCalibrationSample> samples,
        IReadOnlyList<float> imageToWorld, bool reverseOutput)
    {
        if (samples.Count < 3 || imageToWorld.Count < 9)
            return new(0xe0000001, 0, 0, 0, 0, 0, 0);
        double[] h = [imageToWorld[0], imageToWorld[1], imageToWorld[3], imageToWorld[4]];
        var first = Project(samples[0], h);
        double[] a = new double[8], b = new double[4];
        for (int i = 0; i < 2; i++)
        {
            double angle = Angle(samples[i + 1], samples[0], reverseOutput);
            double c = Math.Cos(angle), s = Math.Sin(angle);
            a[4 * i] = 1 - c; a[4 * i + 1] = s;
            a[4 * i + 2] = -s; a[4 * i + 3] = 1 - c;
            var point = Project(samples[i + 1], h);
            b[2 * i] = point.X - first.X; b[2 * i + 1] = point.Y - first.Y;
        }
        double[] at = new double[8];
        for (int i = 0; i < 2; i++) for (int j = 0; j < 4; j++) at[i * 4 + j] = a[j * 2 + i];
        var normal = Multiply(at, a, 2, 4, 2);
        var pseudo = Multiply(Inverse(normal), at, 2, 2, 4);
        var offset = Multiply(pseudo, b, 2, 4, 1);
        double wx = first.X + offset[0], wy = first.Y + offset[1];
        var inverse = Inverse(h);
        double ix = inverse[0] * wx + inverse[1] * wy;
        double iy = inverse[2] * wx + inverse[3] * wy;
        double error = 0;
        for (int i = 1; i < samples.Count; i++)
        {
            double angle = Angle(samples[i], samples[0], reverseOutput);
            double c = Math.Cos(angle), s = Math.Sin(angle);
            double predictedX = c * (first.X - wx) - s * (first.Y - wy) + wx;
            double predictedY = s * (first.X - wx) + c * (first.Y - wy) + wy;
            var point = Project(samples[i], h);
            double dx = point.X - predictedX, dy = point.Y - predictedY;
            error += Math.Sqrt(dx * dx + dy * dy);
        }
        error /= samples.Count;
        double pixelFactor = 2 / (Math.Sqrt(h[0] * h[0] + h[1] * h[1]) + Math.Sqrt(h[2] * h[2] + h[3] * h[3]));
        return new(0, ix, iy, wx, wy, error, error * pixelFactor);
    }
    private static double Angle(RotateCalibrationSample p, RotateCalibrationSample first, bool reverse) =>
        (double)((reverse ? 1f : -1f) * (p.WorldAngle - first.WorldAngle)) / 180 * 3.1415926;
    private static (double X, double Y) Project(RotateCalibrationSample p, double[] h) =>
        (h[0] * p.ImageX + h[1] * p.ImageY, h[2] * p.ImageX + h[3] * p.ImageY);
    private static double[] Inverse(double[] h)
    {
        double determinant = h[0] * h[3] - h[1] * h[2];
        if (Math.Abs(determinant) <= (double)1e-7f) return new double[4];
        double reciprocal = 1 / determinant;
        return [h[3] * reciprocal, -1 * h[1] * reciprocal, -1 * h[2] * reciprocal, h[0] * reciprocal];
    }
    private static double[] Multiply(double[] a, double[] b, int rows, int inner, int cols)
    {
        var result = new double[rows * cols];
        for (int i = 0; i < rows; i++) for (int j = 0; j < cols; j++) for (int k = 0; k < inner; k++)
            result[i * cols + j] += a[i * inner + k] * b[k * cols + j];
        return result;
    }
}
