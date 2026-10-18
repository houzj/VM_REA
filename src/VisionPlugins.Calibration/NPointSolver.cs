namespace VisionPlugins.Calibration;

public readonly record struct NPointPair(double ImageX, double ImageY, double WorldX, double WorldY);
public readonly record struct NPointRotation(double ImageX, double ImageY, double WorldX, double WorldY, double Angle);
public sealed record NPointSolveOptions(int Freedom = 2, int Weight = 3, int CameraMode = 1,
    double OriginX = 0, double OriginY = 0, double WeightFactor = 20, double DistThreshold = 30, double SampleRatio = 60);
public sealed record NPointSolveResult
{
    public int Status { get; init; }
    public uint ErrorCode { get; init; }
    public double[] Matrix { get; init; } = [1, 0, 0, 0, 1, 0, 0, 0, 1];
    public double OffsetError { get; init; }
    public double OffsetMaxError { get; init; }
    public int OffsetMaxErrorPointNum { get; init; }
    public double OffsetWorldError { get; init; }
    public double OffsetWorldMaxError { get; init; }
    public double RotateError { get; init; }
    public double RotateMaxError { get; init; }
    public int RotateMaxErrorPointNum { get; init; }
    public double RotateWorldError { get; init; }
    public double RotateWorldMaxError { get; init; }
    public int Direction { get; init; }
    public double CenterImageX { get; init; }
    public double CenterImageY { get; init; }
    public double CenterWorldX { get; init; }
    public double CenterWorldY { get; init; }
    public double Scale { get; init; }
    public double DeltaX { get; init; }
    public double DeltaY { get; init; }
    public double Theta { get; init; }
    public double Aspect { get; init; }
    public double Skew { get; init; }
    public double PixelPrecision { get; init; }
}

/// <summary>Independent numerical calibration. Original SDK evidence and limits are recorded with its tests.</summary>
public static class NPointSolver
{
    public static NPointSolveResult Solve(IReadOnlyList<NPointPair> offsets,
        IReadOnlyList<NPointRotation> rotations, NPointSolveOptions options)
    {
        ArgumentNullException.ThrowIfNull(offsets);
        ArgumentNullException.ThrowIfNull(rotations);
        ArgumentNullException.ThrowIfNull(options);
        if (!double.IsFinite(options.WeightFactor) || options.WeightFactor < 1)
            return new() { Status = 1, ErrorCode = 0x10103028 };
        if (!double.IsFinite(options.DistThreshold) || options.DistThreshold is < 1 or > 100)
            return new() { Status = 1, ErrorCode = 0x10103029 };
        if (!double.IsFinite(options.SampleRatio) || options.SampleRatio is < 1 or > 100)
            return new() { Status = 1, ErrorCode = 0x1010302a };
        if (offsets.Count < 4 || offsets.Count > 1024 || rotations.Count > 32)
            return new() { Status = 1, ErrorCode = 0x80100005 };
        if (options.Freedom is < 1 or > 3 || options.Weight is < 1 or > 3 || options.CameraMode is < 1 or > 3 ||
            offsets.Any(p => !double.IsFinite(p.ImageX + p.ImageY + p.WorldX + p.WorldY)))
            return new() { Status = 1, ErrorCode = 0x80100006 };
        offsets = offsets.Select(p => p with { ImageX = p.ImageX - options.OriginX, ImageY = p.ImageY - options.OriginY }).ToArray();
        var matrix = SelectFourPointFit(offsets, options);
        if (matrix is null) return new() { Status = 1, ErrorCode = 0x10103007 };
        var inv = FloatInverse(matrix);
        if (inv is null) return new() { Status = 1, ErrorCode = 0x10103007 };
        // 0x7ff3c0 computes inverse reprojection and sqrt in float; the running
        // sum is left-associated (sum + e0 + e1), not a double LINQ average.
        var imageErrors = offsets.Select(p => {
            float x=(float)p.WorldX,y=(float)p.WorldY;
            float z=(float)inv[6]*x+(float)inv[7]*y+(float)inv[8];
            float dx=((float)inv[0]*x+(float)inv[1]*y+(float)inv[2])/z-(float)p.ImageX;
            float dy=((float)inv[3]*x+(float)inv[4]*y+(float)inv[5])/z-(float)p.ImageY;
            return (double)MathF.Sqrt(dx*dx+dy*dy);
        }).ToArray();
        float errorSum=0;foreach(var error in imageErrors)errorSum+=(float)error;
        double meanError=errorSum/offsets.Count;
        const double epsilon = 1.1920928955078125e-7;
        double precision = (Math.Sqrt(matrix[0] * matrix[0] + matrix[1] * matrix[1]) +
                            Math.Sqrt(matrix[3] * matrix[3] + matrix[4] * matrix[4])) / (2 * (matrix[8] + epsilon));
        var result = new NPointSolveResult
        {
            Status = meanError > 1.5 ? 0x1010304b : imageErrors.Max() > 5 ? 0x1010304a : 0,
            Matrix = matrix, OffsetError = meanError, OffsetMaxError = imageErrors.Max(),
            OffsetMaxErrorPointNum = Array.IndexOf(imageErrors, imageErrors.Max()),
            OffsetWorldError = meanError * precision, OffsetWorldMaxError = imageErrors.Max() * precision,
            PixelPrecision = precision
        };
        if (rotations.Count != 0) result = SolveRotation(result, rotations, options);
        if (options.CameraMode == 3)
        {
            matrix = (double[])result.Matrix.Clone();
            for (int i = 0; i < 6; i++) matrix[i] = -matrix[i];
            result = result with { Matrix = matrix,
                CenterWorldX = -result.CenterWorldX, CenterWorldY = -result.CenterWorldY, Direction = -result.Direction };
        }
        return Decompose(result);
    }

    static NPointSolveResult Decompose(NPointSolveResult result)
    {
        var inverse = FloatInverse(result.Matrix); if (inverse is null || inverse[8] == 0) return result;
        var h = inverse.Select(x => (float)x / (float)inverse[8]).ToArray();
        const float eps = 1.1920928955078125e-7f;
        float theta = MathF.Atan2(h[3], h[0]);
        float scale = h[0] / (MathF.Cos(theta) + eps);
        float skew = MathF.Atan2(-h[1], h[4]) - theta;
        if (skew < -MathF.PI) skew += MathF.PI * 2;
        if (skew > MathF.PI) skew -= MathF.PI * 2;
        float aspect = -h[1] / (MathF.Sin(theta + skew) * scale + eps);
        return result with { Scale = scale, DeltaX = h[2], DeltaY = h[5], Theta = theta, Skew = skew, Aspect = aspect };
    }

    static NPointSolveResult SolveRotation(NPointSolveResult result, IReadOnlyList<NPointRotation> points, NPointSolveOptions o)
    {
        if (points.Count < 2) return result with { Status = 1, ErrorCode = 0x10103002 };
        var h = (double[])result.Matrix.Clone(); h[2] = h[5] = 0;
        var mapped = points.Select(p => {
            float x = (float)p.ImageX, y = (float)p.ImageY;
            float z = (float)h[6] * x + (float)h[7] * y + (float)h[8];
            return (X: (double)(((float)h[0] * x + (float)h[1] * y) / z), Y: (double)(((float)h[3] * x + (float)h[4] * y) / z));
        }).ToArray();
        // Exact coincident float samples with distinct angles locate the pivot itself.
        // This algebraic solution preserves the original SDK's exact zero residual/index 0
        // without introducing an arbitrary near-zero threshold (three camera-mode replays).
        if (points.All(p => (float)p.ImageX == (float)points[0].ImageX && (float)p.ImageY == (float)points[0].ImageY) &&
            points.Any(p => (float)(p.Angle - points[0].Angle) % 360f != 0))
        {
            h[2] = -mapped[0].X; h[5] = -mapped[0].Y;
            return result with { Matrix = h, CenterImageX = (float)points[0].ImageX, CenterImageY = (float)points[0].ImageY,
                CenterWorldX = mapped[0].X, CenterWorldY = mapped[0].Y, Direction = o.CameraMode == 3 ? -1 : 1 };
        }
        double bestError = double.PositiveInfinity; double[]? best = null; int direction = 0;
        int preferredDirection = o.CameraMode == 3 ? -1 : 1;
        var imageInverse = Inverse(h)!;
        foreach (int sign in points.Count == 2 ? new[] { preferredDirection } : new[] { preferredDirection, -preferredDirection })
            {
                var rows = new List<double[]>(); var values = new List<double>();
                for (int i = 0; i < points.Count; i++)
                {
                    float a = (float)((float)(points[i].Angle - points[0].Angle) * sign * 0.01745329238474369f);
                    float c = MathF.Cos(a), s = MathF.Sin(a);
                    rows.Add([1f - c, s]); rows.Add([-s, 1f - c]);
                    values.Add((float)((float)mapped[i].X - c * (float)mapped[0].X + s * (float)mapped[0].Y));
                    values.Add((float)((float)mapped[i].Y - s * (float)mapped[0].X - c * (float)mapped[0].Y));
                }
                var v = FloatNestedNormal(rows, values, null, false); if (v is null) continue;
                double error = 0;
                for (int i = 0; i < points.Count; i++)
                {
                    double ex = rows[i * 2].Zip(v, (a, b) => a * b).Sum() - values[i * 2];
                    double ey = rows[i * 2 + 1].Zip(v, (a, b) => a * b).Sum() - values[i * 2 + 1];
                    error += Distance(Transform(imageInverse, mapped[i].X + ex, mapped[i].Y + ey), (points[i].ImageX, points[i].ImageY));
                }
                error /= points.Count;
                if (best is null || ((bestError >= .1 || error >= .1) && bestError > error * 1.5))
                    { bestError = error; best = v; direction = sign; }
            }
        if (best is null) return result with { Status = 1, ErrorCode = 0x10103002 };
        double cx = best[0], cy = best[1];
        var originalInverse = Inverse(h)!; var centerImage = Transform(originalInverse, cx, cy);
        h[2] = -cx; h[5] = -cy;
        var inv = Inverse(h)!;
        var errors = points.Select((p, i) => {
            float a = (float)(p.Angle - points[0].Angle) * direction * 0.01745329238474369f, c = MathF.Cos(a), s = MathF.Sin(a);
            float x = (float)cx + c * ((float)mapped[0].X - (float)cx) - s * ((float)mapped[0].Y - (float)cy);
            float y = (float)cy + s * ((float)mapped[0].X - (float)cx) + c * ((float)mapped[0].Y - (float)cy);
            float dx = x - (float)mapped[i].X, dy = y - (float)mapped[i].Y;
            return MathF.Sqrt(dx * dx + dy * dy) / result.PixelPrecision;
        }).ToArray();
        return result with { Status = result.Status != 0 ? result.Status : errors.Average() > 1.5 ? 0x1010304d : errors.Max() > 1.5 ? 0x1010304c : 0,
            Matrix = h, CenterImageX = centerImage.X, CenterImageY = centerImage.Y,
            CenterWorldX = cx, CenterWorldY = cy, DeltaX = inv[2], DeltaY = inv[5],
            Direction = direction, RotateError = errors.Average(), RotateMaxError = errors.Max(),
            RotateMaxErrorPointNum = Array.IndexOf(errors, errors.Max()), RotateWorldError = errors.Average() * result.PixelPrecision,
            RotateWorldMaxError = errors.Max() * result.PixelPrecision };
    }

    static double[]? Fit(IReadOnlyList<NPointPair> points, double[] weights, int freedom, bool nativeNormal = false, bool nested = true)
    {
        var rows = new List<double[]>(); var values = new List<double>(); var w = new List<double>();
        foreach (var (p, i) in points.Select((p, i) => (p, i)))
        {
            double x = p.ImageX, y = p.ImageY, u = p.WorldX, v = p.WorldY;
            if (freedom == 1) { rows.Add([x, y, 1, 0, 0, 0, -u * x, -u * y]); rows.Add([0, 0, 0, x, y, 1, -v * x, -v * y]); }
            else if (freedom == 2) { rows.Add([x, y, 1, 0, 0, 0]); rows.Add([0, 0, 0, x, y, 1]); }
            else { rows.Add([x, -y, 1, 0]); rows.Add([y, x, 0, 1]); }
            values.Add(u); values.Add(v); w.Add(weights[i]); w.Add(weights[i]);
        }
        var a = nativeNormal ? FloatNestedNormal(rows, values, w, nested) : LeastSquares(rows, values, w);
        return a is null ? null : freedom == 1 ? [.. a, 1] : freedom == 2 ? [.. a, 0, 0, 1] : [a[0], -a[1], a[2], a[1], a[0], a[3], 0, 0, 1];
    }

    static double[]? SelectFourPointFit(IReadOnlyList<NPointPair> p, NPointSolveOptions o)
    {
        // MVD_Algorithm 0x7f2560 enumerates four-point combinations through 40 points;
        // larger sets use one normalized fit. The module UI limits translation points to 16.
        if (p.Count > 40) return RefineNormalized(p.ToArray(), o);
        const int minimal = 4;
        double[]? best = null; double bestError = double.PositiveInfinity;
        var indices = new int[minimal];
        void Visit(int depth, int start)
        {
            if (depth < minimal) { for (int i = start; i <= p.Count - (minimal - depth); i++) { indices[depth] = i; Visit(depth + 1, i + 1); } return; }
            var sample = indices.Select(i => p[i]).ToArray();
            var h = RefineNormalized(sample, o);
            var inv = h is null ? null : FloatInverse(h);
            if (inv is null) return;
            float error = 0;
            for (int i = 0; i < p.Count; i++)
            {
                float x = (float)p[i].WorldX, y = (float)p[i].WorldY;
                float z = (float)inv[6] * x + (float)inv[7] * y + (float)inv[8];
                float dx = ((float)inv[0] * x + (float)inv[1] * y + (float)inv[2]) / z - (float)p[i].ImageX;
                float dy = ((float)inv[3] * x + (float)inv[4] * y + (float)inv[5]) / z - (float)p[i].ImageY;
                error += MathF.Sqrt(dx * dx + dy * dy);
            }
            if (error < bestError) { bestError = error; best = h; }
        }
        Visit(0, 0);
        return best;
    }

    static double[]? RefineNormalized(NPointPair[] points, NPointSolveOptions o)
    {
        // 0x7ff3c0 centers/scales both point sets; 0x7fd4e0 runs at most 50 IRLS
        // iterations with upper-median residual * 1.482579708 * WeightFactor / 10.
        static (float X, float Y, float Scale) Normalize((double X, double Y)[] p)
        {
            float x = 0, y = 0; foreach (var q in p) { x += (float)q.X; y += (float)q.Y; } x *= 1f / p.Length; y *= 1f / p.Length;
            float sum = 0; foreach (var q in p) { float dx = (float)q.X - x, dy = (float)q.Y - y; sum += MathF.Sqrt(dx * dx + dy * dy); }
            // Native 0x1807ff3c0: float at 0x18162b69c is 1.0 (00 00 80 3f).
            // Preserve sum * inverseCount rounding; this is not sqrt(2) normalization.
            return (x, y, 1f / (sum * (1f / p.Length)));
        }
        var ni = Normalize(points.Select(p => (p.ImageX, p.ImageY)).ToArray()); var nw = Normalize(points.Select(p => (p.WorldX, p.WorldY)).ToArray());
        double ix = ni.X, iy = ni.Y, si = ni.Scale, wx = nw.X, wy = nw.Y, sw = nw.Scale;
        var normalized = points.Select(p => new NPointPair((float)((float)((float)p.ImageX - ni.X) * ni.Scale), (float)((float)((float)p.ImageY - ni.Y) * ni.Scale),
            (float)((float)((float)p.WorldX - nw.X) * nw.Scale), (float)((float)((float)p.WorldY - nw.Y) * nw.Scale))).ToArray();
        // 0x7fcf60 feeds the original design matrix to 0x8580e0/0x7075e0;
        // refinement feeds its already formed normal matrix to that same solver.
        var weights = Enumerable.Repeat(1d, points.Length).ToArray(); var h = Fit(normalized, weights, o.Freedom, true, false);
        bool converged = false; const double eps = 1.1920928955078125e-7;
        for (int iteration = 0; h is not null && iteration < 50; iteration++)
        {
            var inverse = FloatInverse(h); if (inverse is null) return null;
            var radii = normalized.Select(p => {
                float x = (float)p.WorldX, y = (float)p.WorldY;
                float z = (float)inverse[6] * x + (float)inverse[7] * y + (float)inverse[8];
                float dx = ((float)inverse[0] * x + (float)inverse[1] * y + (float)inverse[2]) / z - (float)p.ImageX;
                float dy = ((float)inverse[3] * x + (float)inverse[4] * y + (float)inverse[5]) / z - (float)p.ImageY;
                return (double)MathF.Sqrt(dx * dx + dy * dy);
            }).ToArray();
            if (radii.Max() < 1e-10) { converged = true; break; }
            double threshold = (float)((float)((float)radii.Order().ElementAt(points.Length / 2) * 1.4825797080993652f) * (float)((float)o.WeightFactor * .1f));
            for (int i = 0; i < points.Length; i++)
            {
                float radius = (float)radii[i], limit = (float)threshold, e = (float)eps;
                float weight = 1f - radius * radius / (limit * limit + e);
                weights[i] = o.Weight == 1 ? 1 : o.Weight == 2 ? (radius + e > limit ? limit / (radius + e) : 1) : radius > limit ? e : weight * weight;
            }
            var next = Fit(normalized, weights, o.Freedom, true); if (next is null) return null;
            var dimensions = o.Freedom == 3 ? new[] { 0, 1, 2, 5 } : Enumerable.Range(0, o.Freedom == 1 ? 8 : 6).ToArray();
            double step = Math.Sqrt(dimensions.Sum(i => Math.Pow(h[i] - next[i], 2))); h = next;
            if (step < 1e-4) { converged = true; break; }
        }
        if (!converged || h is null) return null;
        double[] ti = [si, 0, (float)(-(float)si * (float)ix), 0, si, (float)(-(float)si * (float)iy), 0, 0, 1];
        // 0x7fcf60/0x7fd4e0 use the full float determinant/adjugate inverse,
        // not the algebraically equivalent 1/scale and original centroid shortcut.
        // Its rounding is observable in the original exported calibration matrix.
        var twInverse = FloatInverse([(float)sw, 0, (float)(-(float)sw * (float)wx),
            0, (float)sw, (float)(-(float)sw * (float)wy), 0, 0, 1]);
        if (twInverse is null) return null;
        static double[] Multiply(double[] a, double[] b) => Enumerable.Range(0, 9).Select(i => {
            float sum = 0; for (int k = 0; k < 3; k++) sum += (float)a[i / 3 * 3 + k] * (float)b[k * 3 + i % 3]; return (double)sum;
        }).ToArray();
        var raw = Multiply(Multiply(twInverse, h), ti);
        return raw.Select(x => (double)((float)x / (float)raw[8])).ToArray();
    }

    static double[]? LeastSquares(IReadOnlyList<double[]> rows, IReadOnlyList<double> values, IReadOnlyList<double>? weights, bool nativeNormal = false)
    {
        int n = rows[0].Length; var a = new double[n, n + 1];
        for (int r = 0; r < rows.Count; r++)
            for (int i = 0; i < n; i++)
            {
                double v = rows[r][i] * (weights?[r] ?? 1);
                for (int j = 0; j < n; j++) a[i, j] += v * rows[r][j];
                a[i, n] += v * values[r];
            }
        if (nativeNormal)
        {
            return FloatNestedNormal(rows, values, weights);
        }
        for (int i = 0; i < n; i++)
        {
            int pivot = i; for (int j = i + 1; j < n; j++) if (Math.Abs(a[j, i]) > Math.Abs(a[pivot, i])) pivot = j;
            if (Math.Abs(a[pivot, i]) < 1e-12) return null;
            if (pivot != i) for (int j = i; j <= n; j++) (a[i, j], a[pivot, j]) = (a[pivot, j], a[i, j]);
            double d = a[i, i]; for (int j = i; j <= n; j++) a[i, j] /= d;
            for (int k = 0; k < n; k++) if (k != i) { double factor = a[k, i]; for (int j = i; j <= n; j++) a[k, j] -= factor * a[i, j]; }
        }
        return Enumerable.Range(0, n).Select(i => a[i, n]).ToArray();
    }

    static double[]? FloatNestedNormal(IReadOnlyList<double[]> rows, IReadOnlyList<double> values, IReadOnlyList<double>? weights, bool nested = true)
    {
        // 0x7fd4e0 constructs A'WA and calls 0x8580e0 -> 0x7075e0, which forms
        // normal equations again. 0x730e90 inverts in float with full pivoting.
        int n = rows[0].Length; var normal = new float[n, n]; var rhs = new float[n];
        for (int i = 0; i < n; i++)
        {
            for (int r = 0; r < rows.Count; r++) rhs[i] += (float)((float)rows[r][i] * (float)(weights?[r] ?? 1)) * (float)values[r];
            for (int j = 0; j < n; j++) for (int r = 0; r < rows.Count; r++) normal[i, j] += (float)((float)rows[r][i] * (float)(weights?[r] ?? 1)) * (float)rows[r][j];
        }
        var a = new float[n, n]; var b = new float[n];
        if (!nested) { a = normal; b = rhs; }
        else
        for (int i = 0; i < n; i++)
        { for (int r = 0; r < n; r++) b[i] += normal[r, i] * rhs[r]; for (int j = 0; j < n; j++) for (int r = 0; r < n; r++) a[i, j] += normal[r, i] * normal[r, j]; }
        var row = new int[n]; var col = new int[n];
        for (int k = 0; k < n; k++)
        {
            float maximum = 0; int p = k, q = k;
            for (int i = k; i < n; i++) for (int j = k; j < n; j++) if (Math.Abs(a[i, j]) > maximum) { maximum = Math.Abs(a[i, j]); p = i; q = j; }
            if (maximum == 0) return null;
            row[k] = p; col[k] = q;
            if (p != k) for (int j = 0; j < n; j++) (a[k, j], a[p, j]) = (a[p, j], a[k, j]);
            if (q != k) for (int i = 0; i < n; i++) (a[i, k], a[i, q]) = (a[i, q], a[i, k]);
            float inv = 1 / a[k, k];
            for (int j = 0; j < n; j++) if (j != k) a[k, j] *= inv;
            for (int i = 0; i < n; i++) if (i != k) for (int j = 0; j < n; j++) if (j != k) a[i, j] -= a[i, k] * a[k, j];
            for (int i = 0; i < n; i++) if (i != k) a[i, k] *= -inv;
            a[k, k] = inv;
        }
        for (int k = n - 1; k >= 0; k--)
        {
            if (col[k] != k) for (int j = 0; j < n; j++) (a[k, j], a[col[k], j]) = (a[col[k], j], a[k, j]);
            if (row[k] != k) for (int i = 0; i < n; i++) (a[i, k], a[i, row[k]]) = (a[i, row[k]], a[i, k]);
        }
        var result = new double[n];
        for (int i = 0; i < n; i++) { float sum = 0; for (int j = 0; j < n; j++) sum += a[i, j] * b[j]; result[i] = sum; }
        return result;
    }

    public static (double X, double Y) Transform(IReadOnlyList<double> h, double x, double y)
    { double z = h[6] * x + h[7] * y + h[8]; return ((h[0] * x + h[1] * y + h[2]) / z, (h[3] * x + h[4] * y + h[5]) / z); }
    static double Distance((double X, double Y) a, (double X, double Y) b) => Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
    static double[]? Inverse(double[] m)
    {
        double[] a = [m[4] * m[8] - m[5] * m[7], m[2] * m[7] - m[1] * m[8], m[1] * m[5] - m[2] * m[4],
            m[5] * m[6] - m[3] * m[8], m[0] * m[8] - m[2] * m[6], m[2] * m[3] - m[0] * m[5],
            m[3] * m[7] - m[4] * m[6], m[1] * m[6] - m[0] * m[7], m[0] * m[4] - m[1] * m[3]];
        double det = m[0] * a[0] + m[1] * a[3] + m[2] * a[6];
        return Math.Abs(det) < 1e-15 ? null : a.Select(v => v / det).ToArray();
    }

    static double[]? FloatInverse(double[] matrix)
    {
        var m = matrix.Select(x => (float)x).ToArray();
        float det = (((m[8] * m[0] * m[4] + m[6] * m[1] * m[5] + m[7] * m[2] * m[3]) - m[7] * m[0] * m[5]) - m[8] * m[1] * m[3]) - m[6] * m[4] * m[2];
        if (det == 0) return null;
        float inv = 1f / det;
        return [(m[4] * m[8] - m[5] * m[7]) * inv, (m[2] * m[7] - m[1] * m[8]) * inv, (m[1] * m[5] - m[2] * m[4]) * inv,
            (m[5] * m[6] - m[3] * m[8]) * inv, (m[0] * m[8] - m[2] * m[6]) * inv, (m[2] * m[3] - m[0] * m[5]) * inv,
            (m[3] * m[7] - m[4] * m[6]) * inv, (m[1] * m[6] - m[0] * m[7]) * inv, (m[0] * m[4] - m[1] * m[3]) * inv];
    }
}
