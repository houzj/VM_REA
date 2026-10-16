namespace VisionCore;

public enum EdgePolarity { DarkToLight, LightToDark, Both }
public enum CircleDetectionStatus { Success, InsufficientEdges, DegenerateEdges }
public sealed record RadialCircleOptions(double CenterX, double CenterY, double InitialRadius,
    double SearchHalfWidth = 5, int SampleCount = 64, double GradientThreshold = 15,
    EdgePolarity Polarity = EdgePolarity.Both, double RadialStep = .5, double GradientHalfSpan = 1,
    int MinimumEdges = 6);
public sealed record CircleDetectionResult(CircleDetectionStatus Status, CircleFit? Circle,
    IReadOnlyList<Point2> EdgePoints, int OutOfBoundsSamples);

// Independent radial strongest-edge detector. No vendor-equivalence claim.
public static class RadialCircleDetector
{
    public static CircleDetectionResult Detect(GrayImage image, RadialCircleOptions options)
    {
        ArgumentNullException.ThrowIfNull(image);
        ArgumentNullException.ThrowIfNull(options);
        ValidateOptions(options);
        var points = new List<Point2>();
        int outside = 0;
        int steps = (int)Math.Ceiling(2 * options.SearchHalfWidth / options.RadialStep);
        for (int i = 0; i < options.SampleCount; i++)
        {
            double angle = i * (2 * Math.PI / options.SampleCount), c = Math.Cos(angle), s = Math.Sin(angle);
            double best = 0, bestRadius = 0;
            bool hadValidSample = false;
            for (int j = 0; j <= steps; j++)
            {
                double r = options.InitialRadius - options.SearchHalfWidth + Math.Min(j * options.RadialStep, 2 * options.SearchHalfWidth);
                if (!TrySample(image, options.CenterX + (r - options.GradientHalfSpan) * c,
                    options.CenterY + (r - options.GradientHalfSpan) * s, out double inner) ||
                    !TrySample(image, options.CenterX + (r + options.GradientHalfSpan) * c,
                    options.CenterY + (r + options.GradientHalfSpan) * s, out double outer)) continue;
                hadValidSample = true;
                double gradient = outer - inner;
                double strength = options.Polarity switch {
                    EdgePolarity.DarkToLight => gradient,
                    EdgePolarity.LightToDark => -gradient,
                    _ => Math.Abs(gradient)
                };
                // Positive threshold excludes flat profiles; ties choose the inner edge.
                if (strength >= options.GradientThreshold && strength > best) { best = strength; bestRadius = r; }
            }
            if (!hadValidSample) outside++;
            if (best > 0) points.Add(new Point2(options.CenterX + bestRadius * c, options.CenterY + bestRadius * s));
        }
        var edges = points.AsReadOnly();
        if (points.Count < options.MinimumEdges) return new(CircleDetectionStatus.InsufficientEdges, null, edges, outside);
        try { return new(CircleDetectionStatus.Success, Algorithms.FitCircle(edges), edges, outside); }
        catch (ArgumentException) { return new(CircleDetectionStatus.DegenerateEdges, null, edges, outside); }
    }

    public static void ValidateOptions(RadialCircleOptions o)
    {
        ArgumentNullException.ThrowIfNull(o);
        if (!double.IsFinite(o.CenterX) || !double.IsFinite(o.CenterY) ||
            !double.IsFinite(o.InitialRadius) || !double.IsFinite(o.SearchHalfWidth) ||
            !double.IsFinite(o.RadialStep) || !double.IsFinite(o.GradientHalfSpan) ||
            !double.IsFinite(o.GradientThreshold) || o.SearchHalfWidth <= 0 || o.RadialStep <= 0 ||
            o.GradientHalfSpan <= 0 || o.InitialRadius <= o.SearchHalfWidth + o.GradientHalfSpan ||
            o.GradientThreshold <= 0 || o.GradientThreshold > 255 ||
            o.SampleCount < 3 || o.SampleCount > 100000 || o.MinimumEdges < 3 || o.MinimumEdges > o.SampleCount ||
            !Enum.IsDefined(o.Polarity) || 2 * o.SearchHalfWidth / o.RadialStep > 100000 ||
            (Math.Ceiling(2 * o.SearchHalfWidth / o.RadialStep) + 1) * o.SampleCount > 5000000)
            throw new ArgumentOutOfRangeException(nameof(o), "Invalid radial circle parameters or excessive sampling budget.");
    }

    // Coordinates outside the image are unavailable; never clamped or padded.
    private static bool TrySample(GrayImage image, double x, double y, out double value)
    {
        value = 0;
        if (!double.IsFinite(x) || !double.IsFinite(y) || x < 0 || y < 0 || x > image.Width - 1 || y > image.Height - 1) return false;
        int x0 = (int)Math.Floor(x), y0 = (int)Math.Floor(y);
        int x1 = Math.Min(x0 + 1, image.Width - 1), y1 = Math.Min(y0 + 1, image.Height - 1);
        double tx = x - x0, ty = y - y0;
        value = (image[x0,y0] * (1-tx) + image[x1,y0] * tx) * (1-ty)
            + (image[x0,y1] * (1-tx) + image[x1,y1] * tx) * ty;
        return true;
    }
}


