using System.IO;
using System.Text.Json;
using VisionCore;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.CalibrationBoard;

internal static class FailureOutputChecks
{
    public static async Task Run(Action<bool, string> check)
    {
        var plugin = new CalibrationBoardPlugin();
        var parameters = plugin.Descriptor.Parameters.ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone());
        var blank = new GrayImage(480, 400, Enumerable.Repeat((byte)255, 480 * 400).ToArray());
        var input = new Dictionary<string, PortValue> { ["InputImage"] = PortValue.Image(blank) };
        var emptyRoi = await plugin.CreateRuntime().ExecuteAsync(new("fail-empty-roi", Environment.CurrentDirectory, input, parameters), default);
        check(emptyRoi.Status == ModuleStatus.NoMatch, "blank image with empty ROI remains no-match");
        check(emptyRoi.Outputs["ModuStatus"].As<long>() == 0, "failed native result status is zero");
        check(!new[] { "DetectCenterX", "DetectCenterY", "DetectWidth", "DetectHeight", "DetectAngle" }.Any(emptyRoi.Outputs.ContainsKey), "no ROI input leaves native zero-count Detect outputs absent");
        check(!new[] { "CalibPointNum", "CalibPointX", "CalibPointY", "CalibData", "EstimationError", "Scale", "TranslateX", "TranslateY", "Rotate", "Skew", "Aspect", "PixelPrecision" }.Any(emptyRoi.Outputs.ContainsKey), "failed result does not leak calibration outputs");

        var roi = new RoiDocument("failed-box", RoiKind.Rectangle, 201.25, 190.5, 125.25, 88.5, AngleDegrees: 17.25);
        var withRoi = await plugin.CreateRuntime().ExecuteAsync(new("fail-with-roi", Environment.CurrentDirectory, input, parameters, [roi]), default);
        check(withRoi.Status == ModuleStatus.NoMatch, "blank image with configured ROI remains no-match");
        var expected = new Dictionary<string, float>
        {
            ["DetectCenterX"] = (float)roi.CenterX,
            ["DetectCenterY"] = (float)roi.CenterY,
            ["DetectWidth"] = (float)roi.Width,
            ["DetectHeight"] = (float)roi.Height,
            ["DetectAngle"] = (float)roi.AngleDegrees
        };
        foreach (var pair in expected)
        {
            check(withRoi.Outputs.TryGetValue(pair.Key, out var actual) && (float)actual.As<double>() == pair.Value, $"failed result emits original float ROI output {pair.Key}");
        }
        check(!new[] { "CalibPointNum", "CalibPointX", "CalibPointY", "CalibData", "EstimationError", "Scale", "TranslateX", "TranslateY", "Rotate", "Skew", "Aspect", "PixelPrecision" }.Any(withRoi.Outputs.ContainsKey), "ROI diagnostics do not bypass successful-result output gate");
        var failedDisplay = plugin.LastDisplay(new("fail-with-roi", plugin.Manifest.Id, "1.0.0", "failed board", parameters), Environment.CurrentDirectory, null);
        check(failedDisplay is not null, "native failure result is retained for the result display lifecycle");
        var roiGraphic = failedDisplay!.Graphics.SingleOrDefault(graphic => graphic.Name == "ROI");
        check(roiGraphic is not null, "source ROI display object consumes failed Detect outputs");
        check(roiGraphic!.Color.Equals("#ff0000", StringComparison.OrdinalIgnoreCase), "source ROI display uses NG color for failed ModuStatus");
        check((float)roiGraphic.Points.Single().X == (float)roi.CenterX && (float)roiGraphic.Width == (float)roi.Width && (float)roiGraphic.Angle == (float)roi.AngleDegrees, "failed ROI display geometry uses ProcessOutput detect values");
        check(!failedDisplay.Graphics.Any(graphic => graphic.Name is "Display Calibration Point" or "Display Original Calibration Point" or "Coordinate System"), "failed result does not retain success-only calibration graphics");

        var checkerPixels = Enumerable.Repeat((byte)255, 480 * 400).ToArray();
        for (var y = 0; y < 400; y++)
        for (var x = 0; x < 480; x++)
            if (x >= 60 && x < 420 && y >= 50 && y < 350)
                checkerPixels[y * 480 + x] = (byte)((((x - 60) / 30 + (y - 50) / 30) & 1) == 0 ? 0 : 255);
        var success = await plugin.CreateRuntime().ExecuteAsync(
            new("fail-with-roi", Environment.CurrentDirectory,
                new Dictionary<string, PortValue> { ["InputImage"] = PortValue.Image(new GrayImage(480, 400, checkerPixels)) },
                parameters), default);
        check(success.Status == ModuleStatus.Success && success.Outputs["ModuStatus"].As<long>() == 1, "success after failed display restores native success status");
        var successDisplay = plugin.LastDisplay(new("fail-with-roi", plugin.Manifest.Id, "1.0.0", "failed board", parameters), Environment.CurrentDirectory, null);
        check(successDisplay is not null && successDisplay.Graphics.Any(graphic => graphic.Name == "Display Calibration Point"), "success after failed result restores calibration graphics");
        check(successDisplay!.Graphics.Single(graphic => graphic.Name == "ROI").Color.Equals("#0000ff", StringComparison.OrdinalIgnoreCase), "source ROI display returns to OK color after successful execution");

        var directory = Path.GetFullPath(Path.Combine("analysis/calibration-board", "retained-export-" + Guid.NewGuid().ToString("N")));
        Directory.CreateDirectory(directory);
        var node = new NodeDocument("fail-with-roi", plugin.Manifest.Id, "1.0.0", "board", parameters);
        var editor = new ToolEditorContext(node, Environment.CurrentDirectory, _ => { }, _ => throw new NotSupportedException());
        var beforePath = Path.Combine(directory, "before.iwcal");
        plugin.Export(editor, beforePath);
        var expectedFile = File.ReadAllBytes(beforePath);
        var nextFailure = await plugin.CreateRuntime().ExecuteAsync(new("fail-with-roi", Environment.CurrentDirectory, input, parameters, [roi]), default);
        check(nextFailure.Status == ModuleStatus.NoMatch && nextFailure.Outputs["ModuStatus"].As<long>() == 0, "success then failed detection closes current result gate");
        check(!nextFailure.Outputs.ContainsKey("CalibData") && !nextFailure.Outputs.ContainsKey("CalibPointNum"), "retained export matrix is not published as a successful current result");
        var afterPath = Path.Combine(directory, "after.iwcal");
        plugin.Export(editor, afterPath);
        check(File.ReadAllBytes(afterPath).SequenceEqual(expectedFile), "explicit export after detection failure retains the successful matrix bytes");
        var missingDirectory = Path.Combine(directory, "not-prepared");
        bool failedOpen = false;
        try { plugin.Export(editor, Path.Combine(missingDirectory, "result.iwcal")); }
        catch (CalibrationBoardFileFailure error) { failedOpen = error.NativeErrorCode == 0x80100010; }
        check(failedOpen && !Directory.Exists(missingDirectory), "explicit Save does not run CalibPathName directory preparation");
        bool invalidSuffix = false;
        try { plugin.Export(editor, Path.Combine(directory, "upper.IWCAL")); }
        catch (CalibrationBoardFileFailure error) { invalidSuffix = error.NativeErrorCode == 0xe0000204; }
        check(invalidSuffix, "explicit Save uses the original case-sensitive iwcal suffix gate");
        plugin.Reset(node.Id);
        bool released = false;
        try { plugin.Export(editor, Path.Combine(directory, "removed.iwcal")); }
        catch (CalibrationBoardFileFailure error) { released = error.NativeErrorCode == 0x80100003; }
        check(released, "module removal/reset releases the retained export artifact");
    }
}
