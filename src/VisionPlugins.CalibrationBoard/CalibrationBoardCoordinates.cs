using VisionPlugins.Calibration;

namespace VisionPlugins.CalibrationBoard;

/// <summary>Source-derived board coordinate construction, distinct from the shared fitting algorithm.</summary>
public static class CalibrationBoardCoordinates
{
    public static IReadOnlyList<NPointPair> Build(IReadOnlyList<NImageBoardPoint> points, BoardCalibrationOptions options, double physicalSize)
    {
        // Core 0x1807f7050, 0x1807ed0c0:792 and 0x180853380.
        float angle = (float)options.RotationAngle * 0.01745329238474369f;
        float cosine = MathF.Cos(angle), sine = MathF.Sin(angle), sign = options.CoordinateMode == 1 ? 1 : -1;
        float[,] transform = {{cosine, -sign * sine, (float)options.OriginX},
            {sine, sign * cosine, (float)options.OriginY}, {0, 0, 1}};
        Invert(transform);
        return points.Select(point =>
        {
            float x = point.IndexX * (float)physicalSize, y = point.IndexY * (float)physicalSize;
            return new NPointPair(point.X, point.Y,
                transform[0, 0] * x + transform[0, 1] * y + transform[0, 2],
                transform[1, 0] * x + transform[1, 1] * y + transform[1, 2]);
        }).ToArray();
    }

    internal static void Invert(float[,] matrix)
    {
        // Original 0x180730e90: full pivoting, in-place float inversion and reversed swaps.
        int[] rows = new int[3], columns = new int[3];
        for (int k = 0; k < 3; k++)
        {
            float maximum = 0; int row = k, column = k;
            for (int i = k; i < 3; i++) for (int j = k; j < 3; j++)
                if (MathF.Abs(matrix[i, j]) > maximum) { maximum = MathF.Abs(matrix[i, j]); row = i; column = j; }
            if (maximum == 0) throw new InvalidOperationException("标定板坐标变换不可逆。");
            rows[k] = row; columns[k] = column;
            if (row != k) for (int j = 0; j < 3; j++) (matrix[k, j], matrix[row, j]) = (matrix[row, j], matrix[k, j]);
            if (column != k) for (int i = 0; i < 3; i++) (matrix[i, k], matrix[i, column]) = (matrix[i, column], matrix[i, k]);
            float inverse = 1 / matrix[k, k];
            for (int j = 0; j < 3; j++) if (j != k) matrix[k, j] *= inverse;
            for (int i = 0; i < 3; i++) if (i != k) for (int j = 0; j < 3; j++) if (j != k) matrix[i, j] -= matrix[i, k] * matrix[k, j];
            for (int i = 0; i < 3; i++) if (i != k) matrix[i, k] *= -inverse;
            matrix[k, k] = inverse;
        }
        for (int k = 2; k >= 0; k--)
        {
            if (columns[k] != k) for (int j = 0; j < 3; j++) (matrix[k, j], matrix[columns[k], j]) = (matrix[columns[k], j], matrix[k, j]);
            if (rows[k] != k) for (int i = 0; i < 3; i++) (matrix[i, k], matrix[i, rows[k]]) = (matrix[i, rows[k]], matrix[i, k]);
        }
    }
}
