using System.Text.Json;
using VisionCore;

namespace VisionFramework;

// Independent managed result transport. IVmROIPolygon only establishes Num and ordered Points;
// space and round boundaries are explicit managed metadata, not claims about the native ABI.
public enum PolygonCoordinateSpace { Unspecified, ImagePixels, Normalized, Fixture }

public sealed class PolygonResult
{
    public const int MaximumPoints = 1_000_000;
    private readonly IReadOnlyList<Point2> points;

    public PolygonResult(IEnumerable<Point2> points, PolygonCoordinateSpace coordinateSpace)
    {
        ArgumentNullException.ThrowIfNull(points);
        if (!Enum.IsDefined(coordinateSpace)) throw new InvalidDataException("Invalid polygon coordinate space.");
        var copy = points.Take(MaximumPoints + 1).ToArray();
        if (copy.Length > MaximumPoints) throw new InvalidDataException("Polygon point limit exceeded.");
        if (copy.Any(p => !double.IsFinite(p.X) || !double.IsFinite(p.Y)))
            throw new InvalidDataException("Polygon contains a non-finite point.");
        this.points = Array.AsReadOnly(copy);
        CoordinateSpace = coordinateSpace;
    }

    public PolygonCoordinateSpace CoordinateSpace { get; }
    public IReadOnlyList<Point2> Points => points;
    public int Num => points.Count;
}

public sealed class PolygonBatch
{
    public const int MaximumPoints = 1_000_000;
    public const int MaximumPolygons = 1_000_000;
    private readonly IReadOnlyList<PolygonResult> polygons;
    private readonly IReadOnlyList<int> roundCounts;

    private PolygonBatch(IEnumerable<PolygonResult> polygons, IEnumerable<int> roundCounts,
        PolygonCoordinateSpace coordinateSpace)
    {
        ArgumentNullException.ThrowIfNull(polygons);
        ArgumentNullException.ThrowIfNull(roundCounts);
        if (!Enum.IsDefined(coordinateSpace)) throw new InvalidDataException("Invalid polygon coordinate space.");
        var copy = polygons.Take(MaximumPolygons + 1).ToArray();
        if (copy.Length > MaximumPolygons) throw new InvalidDataException("Polygon count limit exceeded.");
        if (copy.Any(p => p is null || p.CoordinateSpace != coordinateSpace))
            throw new InvalidDataException("Mixed polygon coordinate spaces are not allowed.");
        var frames = roundCounts.Take(MaximumPolygons + 1).ToArray();
        if (frames.Length == 0 || frames.Length > MaximumPolygons || frames.Any(n => n < 0)
            || frames.Sum(n => (long)n) != copy.Length)
            throw new InvalidDataException("Polygon round counts do not match the object count.");
        var total = copy.Sum(p => (long)p.Num);
        if (total > MaximumPoints) throw new InvalidDataException("Polygon batch point limit exceeded.");
        this.polygons = Array.AsReadOnly(copy);
        this.roundCounts = Array.AsReadOnly(frames);
        CoordinateSpace = coordinateSpace;
        PointCount = (int)total;
    }

    public PolygonCoordinateSpace CoordinateSpace { get; }
    public IReadOnlyList<PolygonResult> Polygons => polygons;
    public IReadOnlyList<int> RoundCounts => roundCounts;
    public int PointCount { get; }
    public IReadOnlyList<int> Counts => Array.AsReadOnly(polygons.Select(p => p.Num).ToArray());
    public IReadOnlyList<double> X => Array.AsReadOnly(polygons.SelectMany(p => p.Points).Select(p => p.X).ToArray());
    public IReadOnlyList<double> Y => Array.AsReadOnly(polygons.SelectMany(p => p.Points).Select(p => p.Y).ToArray());

    public static PolygonBatch ForRound(IEnumerable<PolygonResult> polygons, PolygonCoordinateSpace space)
    {
        ArgumentNullException.ThrowIfNull(polygons);
        var copy = polygons.Take(MaximumPolygons + 1).ToArray();
        return new PolygonBatch(copy, [copy.Length], space);
    }

    public PolygonBatch AppendRound(IEnumerable<PolygonResult> next)
    {
        ArgumentNullException.ThrowIfNull(next);
        var frame = next.Take(MaximumPolygons + 1).ToArray();
        // Build and validate the complete replacement before publishing any change.
        return new PolygonBatch(polygons.Concat(frame), roundCounts.Append(frame.Length), CoordinateSpace);
    }

    public static PolygonBatch FromLeaves(IEnumerable<int> counts, IEnumerable<double> x,
        IEnumerable<double> y, IEnumerable<int> roundCounts, PolygonCoordinateSpace space)
    {
        ArgumentNullException.ThrowIfNull(counts);
        ArgumentNullException.ThrowIfNull(x);
        ArgumentNullException.ThrowIfNull(y);
        ArgumentNullException.ThrowIfNull(roundCounts);
        var lengths = counts.Take(MaximumPolygons + 1).ToArray();
        if (lengths.Length > MaximumPolygons || lengths.Any(n => n < 0) || lengths.Sum(n => (long)n) > MaximumPoints)
            throw new InvalidDataException("Invalid polygon point counts.");
        var xs = x.Take(MaximumPoints + 1).ToArray();
        var ys = y.Take(MaximumPoints + 1).ToArray();
        if (xs.Length != lengths.Sum(n => (long)n) || ys.Length != xs.Length)
            throw new InvalidDataException("Polygon Num, X and Y leaf lengths disagree.");
        var items = new PolygonResult[lengths.Length];
        int offset = 0;
        for (int i = 0; i < lengths.Length; i++)
        {
            items[i] = new PolygonResult(Enumerable.Range(offset, lengths[i])
                .Select(j => new Point2(xs[j], ys[j])), space);
            offset += lengths[i];
        }
        return new PolygonBatch(items, roundCounts, space);
    }

    public string ToJson() => JsonSerializer.Serialize(new WireValue(CoordinateSpace,
        roundCounts.ToArray(), Counts.ToArray(), X.ToArray(), Y.ToArray()));

    public static PolygonBatch FromJson(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);
        var value = JsonSerializer.Deserialize<WireValue>(json)
            ?? throw new InvalidDataException("Missing polygon batch data.");
        if (value.RoundCounts is null || value.Counts is null || value.X is null || value.Y is null)
            throw new InvalidDataException("Incomplete polygon batch data.");
        return FromLeaves(value.Counts, value.X, value.Y, value.RoundCounts, value.Space);
    }

    private sealed record WireValue(PolygonCoordinateSpace Space, int[] RoundCounts,
        int[] Counts, double[] X, double[] Y);
}
