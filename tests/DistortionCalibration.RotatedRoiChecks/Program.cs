using System.Buffers.Binary;
using System.Text.Json;
using VisionCore;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.DistortionCalibration;

try
{
    const string path = "analysis/distortion-calibration/trace-coded-boards/fixture.bmp";
    var bytes = File.ReadAllBytes(path);
    if (bytes[0] != 'B' || bytes[1] != 'M' || BinaryPrimitives.ReadUInt16LittleEndian(bytes.AsSpan(28)) != 8)
        throw new InvalidDataException("Expected captured 8-bit SDK fixture.");
    int offset = BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(10));
    int width = BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(18));
    int signedHeight = BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(22));
    int height = Math.Abs(signedHeight), stride = (width + 3) & ~3;
    var pixels = new byte[checked(width * height)];
    for (int y = 0; y < height; y++)
        bytes.AsSpan(offset + (signedHeight < 0 ? y : height - 1 - y) * stride, width).CopyTo(pixels.AsSpan(y * width, width));
    var image = new GrayImage(width, height, pixels);
    var plugin = new DistortionCalibrationPlugin();
    var parameters = DistortionCalibrationMetadata.Defaults();
    parameters["CalibBoardType"] = JsonSerializer.SerializeToElement(7);
    parameters["CalibPathName"] = JsonSerializer.SerializeToElement("");
    parameters["CalibBoardWeightFun"] = JsonSerializer.SerializeToElement(1);
    parameters["DistortionEvaluationEnable"] = JsonSerializer.SerializeToElement(true);
    parameters["OutputPostCorrectionEnable"] = JsonSerializer.SerializeToElement(true);
    var inputs = new Dictionary<string, PortValue>
    {
        ["InputImage"] = PortValue.Image(image),
        ["CorrectCenterPoint"] = PortValue.Point(new Point2(width * .5, height * .5))
    };
    int checks = 0;
    void Check(bool condition, string message) { if (!condition) throw new Exception(message); checks++; }
    // An enclosing rotated rectangle has a full-image bbox. It exercises the SDK
    // mask path without introducing a second image rotation/resampling operation.
    var enclosing = new RoiDocument("enclosing", RoiKind.Rectangle, width * .5, height * .5, width * 2, height * 2, AngleDegrees: 15);
    using var scope = new ModuleExecutionScope();
    var first = await plugin.CreateRuntime().ExecuteAsync(new("rotated", Environment.CurrentDirectory, inputs, parameters, [enclosing]) { Scope = scope }, CancellationToken.None);
    Check(first.Status == ModuleStatus.Success, $"Enclosing rotated ROI failed: {first.NativeErrorCode:X8} {first.Message}");
    Check(first.Outputs["CalibPointNum"].As<long>() == 168, "Enclosing rotated mask changed captured board point count.");
    Check(first.Outputs["DetectAngle"].As<double>() == 15 && first.Outputs["DetectWidth"].As<double>() == (float)(width * 2), "ROI display geometry lost the requested angle/extent.");
    Check(first.Outputs.ContainsKey("CalibPointError") && first.Outputs.ContainsKey("PostCorrectionImage"), "Corrected-image masked recognition did not publish evaluated results.");

    var subset = new RoiDocument("subset", RoiKind.Rectangle, width * .5, height * .5, width * .8, height * .8, AngleDegrees: 15);
    var mask = RectangleRoiMaskRasterizer.Create(width, height, (float)subset.CenterX, (float)subset.CenterY, (float)subset.Width, (float)subset.Height, (float)subset.AngleDegrees);
    parameters["DistortionEvaluationEnable"] = JsonSerializer.SerializeToElement(false);
    parameters["OutputPostCorrectionEnable"] = JsonSerializer.SerializeToElement(false);
    var second = await plugin.CreateRuntime().ExecuteAsync(new("rotated", Environment.CurrentDirectory, inputs, parameters, [subset]) { Scope = scope }, CancellationToken.None);
    Check(second.Status == ModuleStatus.Success, $"Subset rotated ROI failed: {second.NativeErrorCode:X8} {second.Message}");
    var points = second.Outputs["Calibration Point"].As<IReadOnlyList<Point2>>();
    Check(points.Count >= 4 && points.Count < 168, "Rotated subset did not remove out-of-region calibration points.");
    foreach (var point in points)
    {
        int x = (int)((float)point.X + .5f), y = (int)((float)point.Y + .5f);
        Check(x >= 0 && y >= 0 && x < width && y < height && mask.Pixels[y * width + x] != 0, "Published point was not projected into source-image masked coordinates.");
    }
    Check(second.Outputs["DetectAngle"].As<double>() == 15 && second.Outputs["ModuStatus"].As<long>() == 1, "Subset ROI geometry/status differs from the current run.");
    Console.WriteLine($"Rotated ROI checks passed: {checks}; enclosing/evaluated points=168; subset points={points.Count}; original image pixels preserved and mask applied before encoded identity decoding.");
    Console.WriteLine("Offline product regression using an existing SDK image fixture; no new native run or claim of complete rotated-ROI runtime parity.");
    return 0;
}
catch (Exception error)
{
    Console.Error.WriteLine(error);
    return 1;
}
