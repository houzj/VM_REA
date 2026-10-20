using System.IO;
using System.Text.Json;
using VisionPlugins.Calibration;
using VisionPlugins.CalibrationBoard;

internal static class Circle80FitDiagnostic
{
    public static void Run()
    {
        var row = File.ReadLines("docs/plugins/calibration-board-native-fixtures.jsonl")
            .Select(line => JsonDocument.Parse(line).RootElement.Clone())
            .Single(e => e.TryGetProperty("board", out var board) && board.GetInt32() == 2 &&
                         e.TryGetProperty("mode", out var mode) && mode.GetInt32() == 1 && e.TryGetProperty("code", out _));
        var metric = File.ReadLines("docs/plugins/calibration-board-native-fixtures.jsonl")
            .Select(line => JsonDocument.Parse(line).RootElement.Clone())
            .Single(e => e.TryGetProperty("phase", out var phase) && phase.GetString() == "metrics" &&
                         e.GetProperty("board").GetInt32() == 2 && e.GetProperty("mode").GetInt32() == 1);
        var points = row.GetProperty("points").EnumerateArray()
            .Select(p => new NImageBoardPoint(p[2].GetInt32(), p[3].GetInt32(), p[0].GetSingle(), p[1].GetSingle())).ToArray();
        var options = new BoardCalibrationOptions(BoardType: 2, CoordinateMode: 1, Freedom: 1, Weight: 2, WeightFactor: 20, DistThreshold: 30, SampleRatio: 60);
        var stages = CalibrationBoardFitting.Fit(points, options);
        var boardResult = CalibrationBoardFitting.Analyze(points, options, stages);
        var replay = CalibrationBoardCoordinates.Build(points, options, options.PhysicalSize)
            .Select(p => new NPointPair(p.ImageX, p.ImageY, p.WorldX, p.WorldY)).ToArray();
        var generic = NPointSolver.Solve(replay, [], new(Freedom: 1, Weight: 2));
        var boardAnalyze = NPointSolver.AnalyzeNativeIrlsMatrix(
            replay.Select(p => ((float)p.ImageX, (float)p.ImageY, (float)p.WorldX, (float)p.WorldY)).ToArray(),
            stages.ImageToWorld ?? []);
        var expected = row.GetProperty("matrix").EnumerateArray().Select(e => e.GetSingle()).ToArray();
        var first = stages.NormalizedGridToImage ?? [];
        var final = stages.ImageToWorld ?? [];
        Console.WriteLine(JsonSerializer.Serialize(new
        {
            phase = "circle80-two-stage-fit-diagnostic",
            fixture = new { nativeError = metric.GetProperty("error").GetDouble(), nativeMatrix = expected },
            points = points.Length,
            stageError = stages.Error,
            firstStageMatrix = first,
            finalStageMatrix = final,
            firstStageFinalVsNativeMaxFloatDifference = final.Length == 9 ? final.Zip(expected).Max(p => Math.Abs((float)p.First - p.Second)) : (float?)null,
            boardAnalyzeError = boardResult.OffsetError,
            boardAnalyzeMaxError = boardResult.OffsetMaxError,
            rawFinalMatrixAnalyzeError = boardAnalyze.MeanInverseError,
            genericNPointSolverError = generic.OffsetError,
            genericMatrix = generic.Matrix,
            genericMatrixMaxFloatDifference = generic.Matrix.Length == 9 ? generic.Matrix.Zip(expected).Max(p => Math.Abs((float)p.First - p.Second)) : (float?)null
        }));
    }
}
