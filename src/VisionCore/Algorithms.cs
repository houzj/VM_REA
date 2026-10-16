namespace VisionCore;

public static class Algorithms
{
    // Inclusive foreground rule: pixel >= threshold. Output is 0 or 255.
    public static GrayImage Threshold(GrayImage image, byte threshold)
    {
        ArgumentNullException.ThrowIfNull(image);
        var output = new byte[image.Pixels.Length];
        for (int i = 0; i < output.Length; i++) output[i] = image.Pixels[i] >= threshold ? (byte)255 : (byte)0;
        return new GrayImage(image.Width, image.Height, output);
    }

    // Nonzero pixels are foreground, with diagonal adjacency included.
    public static IReadOnlyList<Component> ConnectedComponents8(GrayImage image)
    {
        ArgumentNullException.ThrowIfNull(image);
        int w = image.Width, h = image.Height;
        var seen = new bool[image.Pixels.Length];
        var result = new List<Component>();
        var queue = new Queue<int>();
        for (int seed = 0; seed < seen.Length; seed++)
        {
            if (seen[seed] || image.Pixels[seed] == 0) continue;
            seen[seed] = true; queue.Enqueue(seed);
            int area = 0, minX = w, minY = h, maxX = 0, maxY = 0;
            long sumX = 0, sumY = 0;
            while (queue.TryDequeue(out int index))
            {
                int x = index % w, y = index / w;
                area++; sumX += x; sumY += y;
                minX = Math.Min(minX,x); minY = Math.Min(minY,y); maxX = Math.Max(maxX,x); maxY = Math.Max(maxY,y);
                for (int dy = -1; dy <= 1; dy++) for (int dx = -1; dx <= 1; dx++)
                {
                    int nx = x + dx, ny = y + dy;
                    if ((uint)nx >= (uint)w || (uint)ny >= (uint)h) continue;
                    int neighbor = ny * w + nx;
                    if (seen[neighbor] || image.Pixels[neighbor] == 0) continue;
                    seen[neighbor] = true; queue.Enqueue(neighbor);
                }
            }
            result.Add(new Component(area,minX,minY,maxX,maxY,(double)sumX/area,(double)sumY/area));
        }
        return result.AsReadOnly();
    }

    // Algebraic least squares: 2*a*x + 2*b*y + c = x*x + y*y,
    // after mean centering and uniform scaling; not a robust geometric fit.
    public static CircleFit FitCircle(IReadOnlyList<Point2> points)
    {
        ArgumentNullException.ThrowIfNull(points);
        if (points.Count < 3) throw new ArgumentException("At least three points are required.", nameof(points));
        double mx = 0, my = 0;
        foreach (var p in points)
        {
            if (!double.IsFinite(p.X) || !double.IsFinite(p.Y)) throw new ArgumentException("Coordinates must be finite.", nameof(points));
            mx += p.X / points.Count; my += p.Y / points.Count;
        }
        double scale = 0;
        foreach (var p in points) scale = Math.Max(scale,Math.Max(Math.Abs(p.X-mx),Math.Abs(p.Y-my)));
        if (!double.IsFinite(scale) || scale == 0) throw new ArgumentException("Degenerate point set.", nameof(points));
        var m = new double[3,4];
        foreach (var p in points)
        {
            double x = (p.X-mx)/scale, y = (p.Y-my)/scale, z = x*x+y*y;
            double[] row = [2*x,2*y,1];
            for (int i=0;i<3;i++) { for (int j=0;j<3;j++) m[i,j] += row[i]*row[j]; m[i,3] += row[i]*z; }
        }
        double magnitude = 0;
        for (int i=0;i<3;i++) for (int j=0;j<3;j++) magnitude = Math.Max(magnitude, Math.Abs(m[i,j]));
        for (int col=0;col<3;col++)
        {
            int pivot=col;
            for (int row=col+1;row<3;row++) if (Math.Abs(m[row,col]) > Math.Abs(m[pivot,col])) pivot=row;
            if (Math.Abs(m[pivot,col]) <= magnitude*1e-12) throw new ArgumentException("Collinear or ill-conditioned point set.", nameof(points));
            for (int j=col;j<4;j++) (m[col,j],m[pivot,j]) = (m[pivot,j],m[col,j]);
            double divisor=m[col,col];
            for (int j=col;j<4;j++) m[col,j]/=divisor;
            for (int row=0;row<3;row++) if (row!=col) { double factor=m[row,col]; for (int j=col;j<4;j++) m[row,j]-=factor*m[col,j]; }
        }
        double a=m[0,3], b=m[1,3], r2=a*a+b*b+m[2,3];
        if (!(r2>0) || !double.IsFinite(r2)) throw new ArgumentException("Invalid fitted radius.",nameof(points));
        double cx=mx+a*scale, cy=my+b*scale, radius=Math.Sqrt(r2)*scale, error=0;
        foreach (var p in points) { double dx=(p.X-mx)/scale-a, dy=(p.Y-my)/scale-b; double residual=Math.Sqrt(dx*dx+dy*dy)-Math.Sqrt(r2); error+=residual*residual/points.Count; }
        if (!double.IsFinite(cx) || !double.IsFinite(cy) || !double.IsFinite(radius)) throw new ArgumentException("Fit exceeds numeric range.",nameof(points));
        return new CircleFit(cx,cy,radius,Math.Sqrt(error)*scale);
    }
}

