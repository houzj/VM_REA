using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text.Json;
using VisionCore;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.DistortionCalibration;

static float MaxDelta(IReadOnlyList<Point2> actual, IReadOnlyList<Point2> expected) => actual.Zip(expected)
    .Max(pair => MathF.Sqrt(MathF.Pow((float)pair.First.X - (float)pair.Second.X, 2) + MathF.Pow((float)pair.First.Y - (float)pair.Second.Y, 2)));

static Point2[] ReadPoints(JsonElement row, string key) => row.GetProperty(key).EnumerateArray()
    .Select(point => point.EnumerateArray().Take(2).Select(value => value.GetSingle()).ToArray())
    .Select(point => new Point2(point[0], point[1])).ToArray();

try
{
    const string fixture = "analysis/distortion-calibration/trace-coded-boards/fixture.bmp";
    if (!File.Exists(fixture)) throw new FileNotFoundException("Run the owned no-device SDK fixture capture first.", fixture);
    var bytes = File.ReadAllBytes(fixture);
    if (bytes[0] != (byte)'B' || bytes[1] != (byte)'M' || BinaryPrimitives.ReadUInt16LittleEndian(bytes.AsSpan(28)) != 8)
        throw new InvalidDataException("Expected the captured 8-bit BMP fixture.");
    int pixelOffset = BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(10));
    int width = BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(18));
    int height = Math.Abs(BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(22)));
    int stride = (width + 3) & ~3;
    var pixels = new byte[checked(width * height)];
    bool topDown = BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(22)) < 0;
    for (int y = 0; y < height; y++)
        bytes.AsSpan(pixelOffset + (topDown ? y : height - 1 - y) * stride, width).CopyTo(pixels.AsSpan(y * width, width));
    var image = new GrayImage(width, height, pixels);
    var plugin = new DistortionCalibrationPlugin();
    if (!plugin.Descriptor.Outputs.Single(port => port.Name == "ModuRunTime").Required)
        throw new Exception("Original module contract requires ModuRunTime.");
    var parameters = DistortionCalibrationMetadata.Defaults();
    parameters["CalibBoardType"] = JsonSerializer.SerializeToElement(7);
    string exportPath = Path.Combine(Path.GetTempPath(), "distortion-plugin-" + Guid.NewGuid().ToString("N") + ".iccal");
    parameters["CalibPathName"] = JsonSerializer.SerializeToElement(exportPath);
    using var scope = new ModuleExecutionScope();
    var context = new ModuleExecutionContext("distortion", Environment.CurrentDirectory,
        new Dictionary<string, PortValue>
        {
            ["InputImage"] = PortValue.Image(image),
            ["CorrectCenterPoint"] = PortValue.Point(new Point2(width * .5, height * .5))
        }, parameters)
        { Scope = scope };
    using (var failedScope = new ModuleExecutionScope())
    {
        var missingInput = new ModuleExecutionContext("distortion-missing-input", Environment.CurrentDirectory,
            new Dictionary<string, PortValue>(), DistortionCalibrationMetadata.Defaults()) { Scope = failedScope };
        var failed = await plugin.CreateRuntime().ExecuteAsync(missingInput, CancellationToken.None);
        if (failed.Status != ModuleStatus.Failed || !failed.Outputs.TryGetValue("ModuRunTime", out var elapsed) ||
            elapsed.Kind != DataKind.Number || !double.IsFinite(elapsed.As<double>()) || elapsed.As<double>() < 0)
            throw new Exception("Required ModuRunTime output is missing or invalid on a handled failure.");
    }
    var run = await plugin.CreateRuntime().ExecuteAsync(context, CancellationToken.None);
    if (run.Status != ModuleStatus.Success) throw new Exception($"Captured fixture execution failed: 0x{run.NativeErrorCode:x8} {run.Message}");
    if (run.Outputs["CalibPointNum"].As<long>() != 168) throw new Exception("Captured Board7 point count differs from original SDK trace.");
    var outputPoints = run.Outputs["Calibration Point"].As<IReadOnlyList<Point2>>();
    if (outputPoints.Count != 168) throw new Exception("Output point-array cardinality mismatch.");
    var outFile = plugin.Descriptor.Outputs.Single(p => p.Name == "PostCorrectionImage");
    if (outFile.Kind != DataKind.Image) throw new Exception("Post-correction image descriptor missing.");
    var display = plugin.LastDisplay(new NodeDocument("distortion", plugin.Manifest.Id, "1.0.0", "畸变标定", parameters), Environment.CurrentDirectory, scope)
        ?? throw new Exception("Successful runtime did not expose a display frame.");
    if (display.Image is not GrayImage displayedInput || !displayedInput.Pixels.SequenceEqual(image.Pixels))
        throw new Exception("Source Display.xml InImage object did not resolve to the captured input image.");
    var initialImageLayers = display.ImageLayers;
    if (initialImageLayers.Select(layer => layer.Name).SequenceEqual(["InImage", "Display Post-Correction Image"]) is false ||
        !initialImageLayers[0].Visible || initialImageLayers[1].Visible || initialImageLayers[0].Image is not GrayImage inputLayer || !inputLayer.Pixels.SequenceEqual(image.Pixels))
        throw new Exception("Display.xml image objects, declared order, input mapping, or default visibility were not projected from source metadata.");
    int pointLayerCount = display.Graphics.Where(graphic => graphic.Name == "Display Calibration Point").Sum(graphic => graphic.Points.Count);
    int errorTextLayerCount = display.Graphics.Where(graphic => graphic.Name == "Show Evaluation Error Text").Sum(graphic => graphic.Points.Count);
    int roiLayerCount = display.Graphics.Where(graphic => graphic.Name == "ROI").Sum(graphic => graphic.Points.Count);
    if (pointLayerCount != 168 || errorTextLayerCount != 168 || roiLayerCount != 1)
        throw new Exception("Visible source point/text/ROI display objects were not independently projected from the captured outputs.");
    if (!display.TextVisible || !display.RecordVisible || !display.ResultListVisible || display.ResultList.Count != 1 || !display.RecordText.Contains("EstimationError", StringComparison.Ordinal))
        throw new Exception("Source Show Text/Data Record/Result List display surfaces did not receive the captured execution result.");

    using var nativeEvidence = JsonDocument.Parse(File.ReadAllText("docs/plugins/distortion-calibration-post-correction-evidence.json"));
    var native = nativeEvidence.RootElement.GetProperty("cases").EnumerateArray()
        .Single(row => row.GetProperty("board").GetInt32() == 7 && row.GetProperty("model").GetInt32() == 1);
    var nativePoints = native.GetProperty("locatePoints").EnumerateArray()
        .Select(row => row.EnumerateArray().Take(2).Select(value => value.GetSingle()).ToArray()).ToArray();
    float maxPointDelta = outputPoints.Zip(nativePoints).Max(pair => MathF.Sqrt(
        MathF.Pow((float)pair.First.X - pair.Second[0], 2) + MathF.Pow((float)pair.First.Y - pair.Second[1], 2)));
    float nativeError = native.GetProperty("estimationError").GetSingle();
    float productError = (float)run.Outputs["EstimationError"].As<double>();

    parameters["DistortionEvaluationEnable"] = JsonSerializer.SerializeToElement(true);
    parameters["OutputPostCorrectionEnable"] = JsonSerializer.SerializeToElement(false);
    var rawEvaluationRun = await plugin.CreateRuntime().ExecuteAsync(context, CancellationToken.None);
    if (rawEvaluationRun.Status != ModuleStatus.Success) throw new Exception($"Raw evaluation runtime failed: 0x{rawEvaluationRun.NativeErrorCode:x8} {rawEvaluationRun.Message}");
    var nativeRawErrors = native.GetProperty("rawPointErrors").EnumerateArray().Select(value => value.GetSingle()).ToArray();
    var productRawErrors = rawEvaluationRun.Outputs["CalibPointError"].As<IReadOnlyList<double>>();
    float maxRawErrorDelta = nativeRawErrors.Zip(productRawErrors).Max(pair => MathF.Abs(
        MathF.Round(pair.First * 100f, MidpointRounding.AwayFromZero) / 100f - (float)pair.Second));
    float rawEvaluationDelta = (float)rawEvaluationRun.Outputs["EvaluationError"].As<double>() - native.GetProperty("rawEvaluationError").GetSingle();

    parameters["OutputPostCorrectionEnable"] = JsonSerializer.SerializeToElement(true);
    var evaluationRun = await plugin.CreateRuntime().ExecuteAsync(context, CancellationToken.None);
    if (evaluationRun.Status != ModuleStatus.Success) throw new Exception($"Post-correction runtime failed: 0x{evaluationRun.NativeErrorCode:x8} {evaluationRun.Message}");
    var objectStore = new ToolDisplayObjectStore(DistortionCalibrationMetadata.DisplayObjects, () => parameters,
        next => parameters = next);
    if (!objectStore.Write("InImage", objectStore.Read("InImage")!.Style with { Visible = false }).Accepted ||
        !objectStore.Write("Display Post-Correction Image", objectStore.Read("Display Post-Correction Image")!.Style with { Visible = true }).Accepted)
        throw new Exception("Source display-object visibility settings could not be persisted.");
    var switchedDisplay = plugin.LastDisplay(new NodeDocument("distortion", plugin.Manifest.Id, "1.0.0", "畸变标定", parameters), Environment.CurrentDirectory, scope)
        ?? throw new Exception("Post-correction execution did not expose a display frame.");
    var correctedLayer = switchedDisplay.ImageLayers.Single(layer => layer.Name == "Display Post-Correction Image");
    if (switchedDisplay.ImageLayers.Single(layer => layer.Name == "InImage").Visible || !correctedLayer.Visible ||
        correctedLayer.Image is not GrayImage correctedLayerImage || !correctedLayerImage.Pixels.SequenceEqual(evaluationRun.Outputs["PostCorrectionImage"].As<IImageData>().Pixels))
        throw new Exception("Display.xml post-correction image mapping or per-object visibility differs from the captured runtime output.");
    var correctedPoints = evaluationRun.Outputs["Calibration Point"].As<IReadOnlyList<Point2>>();
    var nativeCorrected = native.GetProperty("correctLocatePoints").EnumerateArray()
        .Select(row => row.EnumerateArray().Take(2).Select(value => value.GetSingle()).ToArray()).ToArray();
    float maxCorrectedDelta = correctedPoints.Zip(nativeCorrected).Max(pair => MathF.Sqrt(
        MathF.Pow((float)pair.First.X - pair.Second[0], 2) + MathF.Pow((float)pair.First.Y - pair.Second[1], 2)));
    float correctedEvaluationDelta = (float)evaluationRun.Outputs["EvaluationError"].As<double>() - native.GetProperty("correctEvaluationError").GetSingle();
    var nativeCorrectErrors = native.GetProperty("correctPointErrors").EnumerateArray().Select(value => value.GetSingle()).ToArray();
    var productCorrectErrors = evaluationRun.Outputs["CalibPointError"].As<IReadOnlyList<double>>();
    float maxCorrectErrorDelta = nativeCorrectErrors.Zip(productCorrectErrors)
        .Max(pair => MathF.Abs(MathF.Round(pair.First * 100f, MidpointRounding.AwayFromZero) / 100f - (float)pair.Second));
    var productFile = DistortionCalibrationFile.Read(File.ReadAllBytes(exportPath));
    float matrixDelta = productFile.Values.Zip(native.GetProperty("matrix").EnumerateArray().Select(v => v.GetSingle()))
        .Max(pair => MathF.Abs(pair.First - pair.Second));
    var correctedSdkPoints = nativeCorrected.Select(xy => new Point2(xy[0], xy[1])).ToArray();
    var sdkFile = new DistortionCalibrationFile(native.GetProperty("matrix").EnumerateArray().Select(value => value.GetSingle()));
    var sdkCorrectedImage = new DistortionCalibrationWarp(sdkFile).Correct(image);
    var sdkCorrectedDetection = DistortionCalibrationRecognition.RecognizeCropped((GrayImage)sdkCorrectedImage, 0, 0,
        7, 1, 50, 1, 20, 40, CancellationToken.None);
    var sdkMatrixCorrectedPoints = sdkCorrectedDetection.Points.Select(point => new Point2(point.X, point.Y)).ToArray();
    float sdkMatrixRecognitionDelta = MaxDelta(sdkMatrixCorrectedPoints, correctedSdkPoints);

    var digest = Convert.ToHexString(SHA256.HashData(image.Pixels)).ToLowerInvariant();
    if (matrixDelta != 0 || maxPointDelta != 0 || productError != nativeError || maxRawErrorDelta != 0 || rawEvaluationDelta != 0 ||
        maxCorrectedDelta != 0 || correctedEvaluationDelta != 0 || maxCorrectErrorDelta != 0 || sdkCorrectedDetection.ErrorCode != 0 || sdkMatrixRecognitionDelta != 0)
        throw new Exception("Original Board7 SDK runtime differential is not bit-exact after native output rounding.");
    int exactCases = 1;
    foreach (var nativeCase in nativeEvidence.RootElement.GetProperty("cases").EnumerateArray()
        .Where(row => row.GetProperty("board").GetInt32() != 7 || row.GetProperty("model").GetInt32() != 1))
    {
        int board = nativeCase.GetProperty("board").GetInt32(), model = nativeCase.GetProperty("model").GetInt32();
        var caseParameters = DistortionCalibrationMetadata.Defaults();
        caseParameters["CalibBoardType"] = JsonSerializer.SerializeToElement(board);
        caseParameters["CorrectModel"] = JsonSerializer.SerializeToElement(model);
        caseParameters["DistortionEvaluationEnable"] = JsonSerializer.SerializeToElement(true);
        caseParameters["OutputPostCorrectionEnable"] = JsonSerializer.SerializeToElement(true);
        string casePath = Path.Combine(Path.GetTempPath(), "distortion-plugin-" + Guid.NewGuid().ToString("N") + ".iccal");
        caseParameters["CalibPathName"] = JsonSerializer.SerializeToElement(casePath);
        using var caseScope = new ModuleExecutionScope();
        var caseContext = new ModuleExecutionContext($"distortion-{board}-{model}", Environment.CurrentDirectory,
            new Dictionary<string, PortValue>
            {
                ["InputImage"] = PortValue.Image(image),
                ["CorrectCenterPoint"] = PortValue.Point(new Point2(width * .5, height * .5))
            }, caseParameters) { Scope = caseScope };
        var caseRun = await plugin.CreateRuntime().ExecuteAsync(caseContext, CancellationToken.None);
        if (caseRun.Status != ModuleStatus.Success) throw new Exception($"Board {board}/model {model} failed: 0x{caseRun.NativeErrorCode:x8} {caseRun.Message}");
        var caseFile = DistortionCalibrationFile.Read(File.ReadAllBytes(casePath));
        float fileDelta = caseFile.Values.Zip(nativeCase.GetProperty("matrix").EnumerateArray().Select(value => value.GetSingle()))
            .Max(pair => MathF.Abs(pair.First - pair.Second));
        float locateDelta = MaxDelta(caseRun.Outputs["Calibration Point"].As<IReadOnlyList<Point2>>(), ReadPoints(nativeCase, "correctLocatePoints"));
        float errorDelta = (float)caseRun.Outputs["EvaluationError"].As<double>() - nativeCase.GetProperty("correctEvaluationError").GetSingle();
        var nativeRoundedErrors = nativeCase.GetProperty("correctPointErrors").EnumerateArray().Select(value => value.GetSingle())
            .Select(value => MathF.Round(value * 100f, MidpointRounding.AwayFromZero) / 100f).ToArray();
        var productErrors = caseRun.Outputs["CalibPointError"].As<IReadOnlyList<double>>().Select(value => (float)value).ToArray();
        float errorArrayDelta = nativeRoundedErrors.Zip(productErrors).Max(pair => MathF.Abs(pair.First - pair.Second));
        caseParameters["OutputPostCorrectionEnable"] = JsonSerializer.SerializeToElement(false);
        var rawCaseRun = await plugin.CreateRuntime().ExecuteAsync(caseContext, CancellationToken.None);
        if (rawCaseRun.Status != ModuleStatus.Success) throw new Exception($"Board {board}/model {model} raw evaluation failed: 0x{rawCaseRun.NativeErrorCode:x8} {rawCaseRun.Message}");
        float rawLocateDelta = MaxDelta(rawCaseRun.Outputs["Calibration Point"].As<IReadOnlyList<Point2>>(), ReadPoints(nativeCase, "locatePoints"));
        float estimationDelta = (float)rawCaseRun.Outputs["EstimationError"].As<double>() - nativeCase.GetProperty("estimationError").GetSingle();
        float rawMeanDelta = (float)rawCaseRun.Outputs["EvaluationError"].As<double>() - nativeCase.GetProperty("rawEvaluationError").GetSingle();
        var nativeRawRounded = nativeCase.GetProperty("rawPointErrors").EnumerateArray().Select(value => value.GetSingle())
            .Select(value => MathF.Round(value * 100f, MidpointRounding.AwayFromZero) / 100f).ToArray();
        var productRaw = rawCaseRun.Outputs["CalibPointError"].As<IReadOnlyList<double>>().Select(value => (float)value).ToArray();
        float rawErrorArrayDelta = nativeRawRounded.Zip(productRaw).Max(pair => MathF.Abs(pair.First - pair.Second));
        var correctedStatuses = caseRun.Outputs["CalibPointStatus"].As<IReadOnlyList<double>>();
        var expectedCorrectedStatuses = nativeRoundedErrors.Select(value => value > 1f ? 0d : 1d).ToArray();
        var rawStatuses = rawCaseRun.Outputs["CalibPointStatus"].As<IReadOnlyList<double>>();
        var expectedRawStatuses = nativeRawRounded.Select(value => value > 1f ? 0d : 1d).ToArray();
        float statusDelta = MathF.Max(correctedStatuses.Zip(expectedCorrectedStatuses).Max(pair => (float)Math.Abs(pair.First - pair.Second)),
            rawStatuses.Zip(expectedRawStatuses).Max(pair => (float)Math.Abs(pair.First - pair.Second)));
        if (fileDelta != 0 || locateDelta != 0 || rawLocateDelta != 0 || estimationDelta != 0 || errorDelta != 0 || errorArrayDelta != 0 || rawMeanDelta != 0 || rawErrorArrayDelta != 0 || statusDelta != 0)
            throw new Exception($"Board {board}/model {model} SDK output delta: file={fileDelta:R}, raw/corrected locate={rawLocateDelta:R}/{locateDelta:R}px, estimation={estimationDelta:R}, corrected evaluation={errorDelta:R}/{errorArrayDelta:R}, raw evaluation={rawMeanDelta:R}/{rawErrorArrayDelta:R}, status={statusDelta:R}.");
        exactCases++;
    }
    var blank = new GrayImage(64, 64, new byte[64 * 64]);
    var failParameters = DistortionCalibrationMetadata.Defaults();
    failParameters["CalibBoardType"] = JsonSerializer.SerializeToElement(7);
    var failure = await plugin.CreateRuntime().ExecuteAsync(new ModuleExecutionContext("distortion", Environment.CurrentDirectory,
        new Dictionary<string, PortValue> { ["InputImage"] = PortValue.Image(blank) }, failParameters) { Scope = scope }, CancellationToken.None);
    if (failure.Status == ModuleStatus.Success || failure.Outputs["ModuStatus"].As<long>() != 0)
        throw new Exception("Failed round did not publish current ModuStatus=0.");
    var failedDisplay = plugin.LastDisplay(new NodeDocument("distortion", plugin.Manifest.Id, "1.0.0", "畸变标定", failParameters), Environment.CurrentDirectory, scope)
        ?? throw new Exception("Success-to-failure display snapshot disappeared.");
    if (!failedDisplay.Text.Contains("ModuleStatus:0", StringComparison.Ordinal) || failedDisplay.Color != "#ff0000" ||
        !ReferenceEquals(failedDisplay.ImageLayers.Single(layer => layer.Name == "InImage").Image, blank) ||
        !failedDisplay.Graphics.Any(graphic => graphic.Name == "Display Calibration Point"))
        throw new Exception("Failed display retained a successful status/image or discarded unpublished prior fields.");
    using (var firstFailureScope = new ModuleExecutionScope())
    {
        var firstFailure = await plugin.CreateRuntime().ExecuteAsync(new ModuleExecutionContext("first-failure", Environment.CurrentDirectory,
            new Dictionary<string, PortValue> { ["InputImage"] = PortValue.Image(blank) }, failParameters) { Scope = firstFailureScope }, CancellationToken.None);
        var firstDisplay = plugin.LastDisplay(new NodeDocument("first-failure", plugin.Manifest.Id, "1.0.0", "畸变标定", failParameters), Environment.CurrentDirectory, firstFailureScope);
        if (firstFailure.Status == ModuleStatus.Success || firstDisplay is null || !firstDisplay.Text.Contains("ModuleStatus:0", StringComparison.Ordinal) ||
            firstDisplay.Graphics.Any(graphic => graphic.Name == "Display Calibration Point"))
            throw new Exception("First valid-image failure did not display current status without fabricated points.");
    }
    // Existing directory with a valid suffix passes path preparation, reaches
    // automatic export, and deterministically exercises its I/O failure mapping.
    string blockedExport = Path.Combine(Path.GetTempPath(), "distortion-blocked-" + Guid.NewGuid().ToString("N") + ".iccal");
    Directory.CreateDirectory(blockedExport);
    parameters["CalibPathName"] = JsonSerializer.SerializeToElement(blockedExport);
    parameters["RefreshFileEnable"] = JsonSerializer.SerializeToElement(true);
    parameters["DistortionEvaluationEnable"] = JsonSerializer.SerializeToElement(false);
    var saveFailure = await plugin.CreateRuntime().ExecuteAsync(new ModuleExecutionContext(context.NodeId, context.BaseDirectory, context.Inputs, parameters) { Scope = scope }, CancellationToken.None);
    if (saveFailure.Status != ModuleStatus.Failed || saveFailure.NativeErrorCode != 0x80100010 ||
        saveFailure.Outputs["ModuStatus"].As<long>() != 0 || !Directory.Exists(blockedExport))
        throw new Exception($"Automatic export I/O failure did not publish mapped failure status: {saveFailure.Status} {saveFailure.NativeErrorCode:X8} {saveFailure.Message}");
    Directory.Delete(blockedExport);
    parameters["CalibPathName"] = JsonSerializer.SerializeToElement(exportPath);
    parameters["DistortionEvaluationEnable"] = JsonSerializer.SerializeToElement(true);
    // Reuse the original node/scope after failure: a fresh fit must replace status,
    // image and published values rather than leaving the failure snapshot active.
    var recovered = await plugin.CreateRuntime().ExecuteAsync(context, CancellationToken.None);
    var recoveredDisplay = plugin.LastDisplay(new NodeDocument("distortion", plugin.Manifest.Id, "1.0.0", "畸变标定", parameters), Environment.CurrentDirectory, scope);
    if (recovered.Status != ModuleStatus.Success || recovered.Outputs["ModuStatus"].As<long>() != 1 ||
        recoveredDisplay is null || !recoveredDisplay.Text.Contains("ModuleStatus:1", StringComparison.Ordinal) ||
        !ReferenceEquals(recoveredDisplay.ImageLayers.Single(layer => layer.Name == "InImage").Image, image) ||
        MaxDelta(recovered.Outputs["Calibration Point"].As<IReadOnlyList<Point2>>(), correctedPoints) != 0)
        throw new Exception("Failure-to-success recovery did not replace the same-node display/result snapshot.");
    // Independent direct-context compatibility: unlike the original alias-backed
    // document, callers can pass both an old POINT and a newer scalar leaf.
    var mixedParameters = DistortionCalibrationMetadata.Defaults();
    mixedParameters["CalibBoardType"] = JsonSerializer.SerializeToElement(7);
    mixedParameters["CalibBoardWeightFun"] = JsonSerializer.SerializeToElement(1);
    mixedParameters["CalibPathName"] = JsonSerializer.SerializeToElement(exportPath);
    mixedParameters["RefreshFileEnable"] = JsonSerializer.SerializeToElement(true);
    var mixedInputs = new Dictionary<string, PortValue>
    {
        ["InputImage"] = PortValue.Image(image),
        ["CorrectCenterPoint"] = PortValue.Point(new Point2(width - 30, height * .5)),
        ["CorrectCenterPointX"] = PortValue.Number(30)
    };
    using (var mixedScope = new ModuleExecutionScope())
    {
        var mixed = await plugin.CreateRuntime().ExecuteAsync(new("mixed-center", Environment.CurrentDirectory, mixedInputs, mixedParameters) { Scope = mixedScope }, CancellationToken.None);
        var recognized = DistortionCalibrationRecognition.Recognize(image, 7);
        var expected = DistortionCalibrationSolver.SolveFromPointsNativeDetailed(recognized.Points, width, height, 1, 1, 1, 20, new Point2(30, height * .5));
        if (mixed.Status != ModuleStatus.Success || expected.File is null ||
            !File.ReadAllBytes(exportPath).SequenceEqual(expected.File.Bytes()))
            throw new Exception("Direct-context explicit X did not override stale combined X while retaining combined Y.");
    }
    Console.WriteLine("Center compatibility check: current explicit X overrides stale POINT X; missing Y retains POINT Y in actual runtime file.");
    Console.WriteLine("Failure display checks: success-to-failure-to-success, retained unpublished prior fields, and first valid-image failure passed.");
    Console.WriteLine($"Original SDK corrected-result replay is exact for {exactCases}/6 board-model cases: 19-float file, 168 raw/corrected points, both EvaluationError paths, rounded point-error/status arrays; inputSha256={digest}.");
    Console.WriteLine("GetCorrectLocateInfo is reconstructed as detection over the corrected image; CorrectErrorEvaluationInfo uses native inverse-projection point residuals before 0.01 output rounding.");
    Console.WriteLine("No device was used. Input fixture is generated by the original SDK trace probe and remains in local analysis evidence.");
    return 0;
}
catch (Exception error)
{
    Console.Error.WriteLine(error);
    return 1;
}



