namespace VisionPlugins.CameraMapping;
// Existing independently reconstructed float kernel, reused from NPointSolver; original addresses retained below.
internal static class CameraMappingLinearAlgebra {
    public static double[]? Solve(IReadOnlyList<double[]> rows, IReadOnlyList<double> values, IReadOnlyList<double>? weights, bool nested = true)
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

    public static double[]? Inverse(double[] matrix)
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
