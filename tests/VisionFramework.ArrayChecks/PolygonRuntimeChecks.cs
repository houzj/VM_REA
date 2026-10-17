using VisionCore;
using VisionFramework;

namespace VisionFrameworkArrayChecks;

// Call Run() from the main array-check entry point after framework integration.
public static class PolygonRuntimeChecks
{
    public static int Run()
    {
        int count = 0;
        void Check(bool condition, string message)
        {
            if (!condition) throw new Exception(message);
            count++;
        }
        void Reject(Action action, string message)
        {
            try { action(); }
            catch (Exception e) when (e is ArgumentException or InvalidDataException or System.Text.Json.JsonException)
            {
                count++;
                return;
            }
            throw new Exception(message);
        }

        var mutable = new[] { new Point2(0, 0), new Point2(4, 0), new Point2(0, 3) };
        var p0 = new PolygonResult(mutable, PolygonCoordinateSpace.ImagePixels);
        mutable[0] = new Point2(999, 999);
        Check(p0.Num == 3 && p0.Points[0] == new Point2(0, 0), "polygon copies input points");
        Check(((ICollection<Point2>)p0.Points).IsReadOnly, "polygon point view cannot be modified");

        var p1 = new PolygonResult([new(10, 10), new(12, 10), new(12, 13), new(10, 13)],
            PolygonCoordinateSpace.ImagePixels);
        var batch = PolygonBatch.ForRound([p0, p1], PolygonCoordinateSpace.ImagePixels);
        Check(batch.Counts.SequenceEqual([3, 4]) && batch.RoundCounts.SequenceEqual([2]),
            "first round keeps polygon object boundaries");
        Check(batch.PointCount == 7 && batch.X.SequenceEqual([0d, 4, 0, 10, 12, 12, 10])
            && batch.Y.SequenceEqual([0d, 0, 3, 10, 10, 13, 13]),
            "leaf sequences preserve point order without adding a closing point");
        var p2 = new PolygonResult([new(-1, -2), new(1, -2), new(0, 1)],
            PolygonCoordinateSpace.ImagePixels);
        var second = batch.AppendRound([p2]);
        Check(second.Counts.SequenceEqual([3, 4, 3]) && second.RoundCounts.SequenceEqual([2, 1])
            && second.PointCount == 10, "two rounds retain object and frame segmentation");
        Check(batch.Counts.SequenceEqual([3, 4]) && batch.PointCount == 7,
            "append publishes a new batch and leaves old batch unchanged");

        var reopened = PolygonBatch.FromJson(second.ToJson());
        Check(reopened.CoordinateSpace == PolygonCoordinateSpace.ImagePixels
            && reopened.Counts.SequenceEqual(second.Counts)
            && reopened.RoundCounts.SequenceEqual(second.RoundCounts)
            && reopened.X.SequenceEqual(second.X) && reopened.Y.SequenceEqual(second.Y),
            "JSON roundtrip preserves leaves, coordinate space and round boundaries");
        var rebuilt = PolygonBatch.FromLeaves([3, 4, 3], second.X, second.Y, [2, 1],
            PolygonCoordinateSpace.ImagePixels);
        Check(rebuilt.Polygons[2].Points.SequenceEqual(p2.Points),
            "Num, X and Y leaf tree rebuilds original object sequence");

        var empty = PolygonBatch.ForRound([
            new PolygonResult([], PolygonCoordinateSpace.Unspecified),
            new PolygonResult([new(1, 2)], PolygonCoordinateSpace.Unspecified),
            new PolygonResult([new(3, 4), new(5, 6)], PolygonCoordinateSpace.Unspecified)],
            PolygonCoordinateSpace.Unspecified);
        Check(empty.Counts.SequenceEqual([0, 1, 2]) && empty.PointCount == 3,
            "zero-, one- and two-point objects remain distinct results");
        Check(empty.AppendRound([]).RoundCounts.SequenceEqual([3, 0]),
            "empty successful round has an explicit boundary");

        Reject(() => batch.AppendRound([new PolygonResult([new(0.1, 0.2)],
            PolygonCoordinateSpace.Normalized)]), "mixed coordinate spaces must fail atomically");
        Check(batch.RoundCounts.Count == 1 && batch.Polygons.Count == 2,
            "failed append does not commit a partial round");
        Reject(() => new PolygonResult([new(double.NaN, 0)], PolygonCoordinateSpace.ImagePixels),
            "NaN point rejected");
        Reject(() => new PolygonResult([new(0, double.PositiveInfinity)], PolygonCoordinateSpace.ImagePixels),
            "infinite point rejected");
        Reject(() => new PolygonResult([new(0, 0)], (PolygonCoordinateSpace)99),
            "invalid coordinate space rejected");
        Reject(() => PolygonBatch.FromLeaves([3, 4], new double[6], new double[7], [2],
            PolygonCoordinateSpace.ImagePixels), "missing X leaf rejected");
        Reject(() => PolygonBatch.FromLeaves([3, 4], new double[7], new double[6], [2],
            PolygonCoordinateSpace.ImagePixels), "missing Y leaf rejected");
        Reject(() => PolygonBatch.FromLeaves([3, -1], new double[2], new double[2], [2],
            PolygonCoordinateSpace.ImagePixels), "negative object point count rejected");
        Reject(() => PolygonBatch.FromLeaves([3], new double[3], new double[3], [2],
            PolygonCoordinateSpace.ImagePixels), "round counts inconsistent with object count rejected");
        Reject(() => PolygonBatch.FromLeaves([PolygonBatch.MaximumPoints + 1], [], [], [1],
            PolygonCoordinateSpace.ImagePixels), "declared point limit rejected before allocation");
        Reject(() => PolygonBatch.FromJson("{\"Space\":1,\"RoundCounts\":[1],\"Counts\":[2],\"X\":[1,2],\"Y\":[1]}"),
            "corrupted serialized leaves rejected");
        return count;
    }
}
