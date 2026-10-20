using System.Diagnostics;
using System.IO;
using System.Text.Json;
using VisionCore;
using VisionFramework;
using VisionPluginContracts;

namespace VisionPlugins.DistortionCalibration;

/// <summary>Production node adapter. Native component lifecycle evidence is recorded separately
/// from the independently reconstructed detector, solver, evaluation and image remap.</summary>
public sealed class DistortionCalibrationPlugin : IVisionToolPlugin, IWpfToolEditorFactory,
    IToolConfigurationProvider, IToolDisplayProvider, IToolPointInputAliasProvider
{
    public const string Id = DistortionCalibrationMetadata.ModuleId;
    public ToolManifest Manifest { get; } = new(Id, "1.0.0", "畸变标定", "calibration", "catalog/IMVSImageCalibModuImageEnable.png");
    public ModuleDescriptor Descriptor => DistortionCalibrationMetadata.Descriptor;
    public IReadOnlyList<ToolPointInputAlias> PointInputAliases { get; } = [new("CorrectCenterPoint", "CorrectCenterPointX", "CorrectCenterPointY")];
    public ToolDisplayDefinition DisplayDefinition => DistortionCalibrationMetadata.DisplayDefinition;

    sealed class State : IDisposable
    {
        internal readonly object Gate = new();
        internal readonly DistortionCalibrationArtifactSession Artifacts = new();
        internal readonly DistortionCalibrationCenterState Center = new();
        internal string? AppliedPath;
        internal IReadOnlyDictionary<string, PortValue>? Inputs, Outputs;
        internal ToolDisplayDiagnostic? Diagnostic;
        public void Dispose()
        {
            lock (Gate)
            {
                Artifacts.Dispose();
                Inputs = Outputs = null;
                Diagnostic = null;
            }
        }
    }

    readonly Dictionary<string, State> fallback = new(StringComparer.Ordinal);
    readonly object fallbackGate = new();
    State GetState(string node, string directory, ModuleExecutionScope? scope)
    {
        if (scope is not null) return scope.State(Id, node, () => new State());
        string key = Path.GetFullPath(directory) + "|" + node;
        lock (fallbackGate)
        {
            if (!fallback.TryGetValue(key, out var state)) fallback[key] = state = new State();
            return state;
        }
    }

    public void Reset(string nodeId)
    {
        lock (fallbackGate)
            foreach (string key in fallback.Keys.Where(k => k.EndsWith("|" + nodeId, StringComparison.Ordinal)).ToArray())
            {
                fallback[key].Dispose();
                fallback.Remove(key);
            }
    }

    public IModule CreateRuntime() => new Runtime(this);
    public System.Windows.Forms.Form CreateEditor(ToolEditorContext context) => throw new InvalidOperationException("使用公共 WPF 配置宿主。");
    public System.Windows.Window CreateWpfEditor(ToolEditorContext context) => new ToolConfigurationModelWindow(context, CreateConfiguration(context));
    public ToolConfigurationSession CreateConfiguration(ToolEditorContext context) =>
        new DistortionCalibrationConfigurationProvider(Export).CreateConfiguration(context);

    void Export(ToolEditorContext context, string path)
    {
        var state = GetState(context.Node.Id, context.BaseDirectory, context.Scope);
        lock (state.Gate) state.Artifacts.Export(Path.GetFullPath(path, context.BaseDirectory));
    }

    public ToolDisplayFrame? LastDisplay(NodeDocument node, string baseDirectory, ModuleExecutionScope? scope)
    {
        var state = GetState(node.Id, baseDirectory, scope);
        lock (state.Gate)
        {
            if (state.Inputs is null || state.Outputs is null) return null;
            // The shipped Display.xml bridge names this object PostCorrectionImageData;
            // the module output contract publishes the same image as PostCorrectionImage.
            var displayOutputs = state.Outputs.ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal);
            if (displayOutputs.TryGetValue("PostCorrectionImage", out var corrected))
                displayOutputs["PostCorrectionImageData"] = corrected;
            var frame = ToolDisplayService.Build(DisplayDefinition, ToolDisplayService.Read(node.Parameters, DisplayDefinition),
                state.Inputs, displayOutputs, diagnostic: state.Diagnostic);
            return frame with
            {
                // The source Display.xml objects, including their own visibility settings,
                // now own this tool's image surface instead of the legacy single-image flag.
                ImageVisible = false,
                ImageLayers = ToolDisplayImageLayers.Build(DistortionCalibrationMetadata.DisplayObjects, node.Parameters, state.Inputs, displayOutputs),
                Graphics = DistortionCalibrationDisplay.BuildGraphics(node.Parameters, state.Inputs, displayOutputs)
            };
        }
    }

    static GrayImage OwnedMono(IImageData source)
    {
        if (source.Format != ImagePixelFormat.Mono8)
            throw new DistortionCalibrationRuntimeException(0xe0000503, "畸变标定只接受 Mono8 图像（0xe0000503）。");
        if (source is GrayImage gray) return gray;
        int stride = source.Stride == 0 ? source.Width : source.Stride;
        if (stride < source.Width || source.Pixels.Length < checked(stride * source.Height))
            throw new DistortionCalibrationRuntimeException(0xe0000503, "Mono8 图像步长或缓冲区无效（0xe0000503）。");
        var data = new byte[checked(source.Width * source.Height)];
        for (int y = 0; y < source.Height; y++) source.Pixels.Slice(y * stride, source.Width).CopyTo(data.AsSpan(y * source.Width, source.Width));
        return new(source.Width, source.Height, data);
    }

    static (GrayImage Image, int X, int Y, RoiDocument Output, RectangleRoiMask? Mask) Crop(GrayImage image, IReadOnlyList<RoiDocument> rois)
    {
        var full = new RoiDocument("distortion-roi", RoiKind.Rectangle, image.Width / 2d, image.Height / 2d, image.Width, image.Height);
        if (rois.Count == 0 || rois[0].Kind == RoiKind.FullImage) return (image, 0, 0, full, null);
        if (rois.Count != 1 || rois[0].Kind != RoiKind.Rectangle || rois[0].Exclusion || rois[0].CoordinateSpace != RoiCoordinateSpace.ImagePixels)
            throw new DistortionCalibrationRuntimeException(0x1000002d, "畸变标定仅确认单个图像坐标矩形 ROI。 ");
        var roi = rois[0];
        if (Math.Abs((double)(float)roi.AngleDegrees) > .0001d)
        {
            // SDK Run routes rotated rectangles through PreproMask. Recognition
            // keeps original bbox pixels and applies the mask before tag decoding.
            var mask = RectangleRoiMaskRasterizer.Create(image.Width, image.Height,
                (float)roi.CenterX, (float)roi.CenterY, (float)roi.Width, (float)roi.Height, (float)roi.AngleDegrees);
            if (mask.Width <= 0 || mask.Height <= 0)
                throw new DistortionCalibrationRuntimeException(0x80100025, "ROI 位于图像之外（0x80100025）。");
            return (image, mask.Left, mask.Top, roi, mask);
        }
        float width = (float)roi.Width, height = (float)roi.Height;
        float left = (float)((float)roi.CenterX - width * .5f), top = (float)((float)roi.CenterY - height * .5f);
        float right = (float)(left + width), bottom = (float)(top + height);
        if (left >= image.Width || top >= image.Height || right < 0 || bottom < 0)
            throw new DistortionCalibrationRuntimeException(0x80100025, "ROI 位于图像之外（0x80100025）。");
        int x = Math.Max(0, (int)((double)left + .5)), y = Math.Max(0, (int)((double)top + .5));
        int r = (int)Math.Clamp((long)((double)right + .5), 0, image.Width), b = (int)Math.Clamp((long)((double)bottom + .5), 0, image.Height);
        int cropWidth = r - x, cropHeight = b - y;
        if (cropWidth <= 0 || cropHeight <= 0) throw new DistortionCalibrationRuntimeException(0x1000002d, "ROI 无有效像素（0x1000002d）。");
        byte[] pixels = new byte[checked(cropWidth * cropHeight)];
        for (int row = 0; row < cropHeight; row++) image.Pixels.Slice((y + row) * image.Width + x, cropWidth).CopyTo(pixels.AsSpan(row * cropWidth, cropWidth));
        return (new(cropWidth, cropHeight, pixels), x, y, roi, null);
    }

    static DistortionCalibrationRecognitionResult Recognize(GrayImage source,
        (GrayImage Image, int X, int Y, RoiDocument Output, RectangleRoiMask? Mask) region,
        ModuleExecutionContext c, CancellationToken token) => region.Mask is { } mask
        ? DistortionCalibrationRecognition.RecognizeMasked(source, mask, c.Integer("CalibBoardType"), c.Integer("FilterStatus"),
            c.Integer("Circularity"), c.Integer("CircleBoardCircleMode"), c.Integer("EdgeThreshLow"), c.Integer("EdgeThreshHigh"), token)
        : DistortionCalibrationRecognition.RecognizeCropped(source, region.Image, region.X, region.Y, c.Integer("CalibBoardType"), c.Integer("FilterStatus"),
            c.Integer("Circularity"), c.Integer("CircleBoardCircleMode"), c.Integer("EdgeThreshLow"), c.Integer("EdgeThreshHigh"), token);

    static (double X, double Y) Center(ModuleExecutionContext c, State state)
    {
        // Process20550 reads X, then Y. Only e000031b is missing; all other
        // read failures stop the round with e0000314, without updating SDK center.
        const uint missing = 0xe000031b;
        (uint Error, double Value) Axis(string name, bool horizontal)
        {
            if (c.Scope?.Service<IToolNumericInputReader>() is { } provider)
            {
                var supplied = provider.Read(c, name, 0);
                return (supplied.ErrorCode, supplied.Value);
            }
            if (c.ReadNumericInput?.Invoke(name, 0) is { } deferred)
                return (deferred.ErrorCode, deferred.Value);
            if (c.Inputs.TryGetValue(name, out var leaf))
                return leaf.Kind switch
                {
                    DataKind.Number => (0, leaf.As<double>()),
                    DataKind.Integer => (0, leaf.As<long>()),
                    DataKind.NumberArray => leaf.As<IReadOnlyList<double>>() is { Count: > 0 } values ? (0, values[0]) : (0xe0000314, 0),
                    _ => (0xe0000314, 0)
                };
            // Independent direct-context compatibility only. Actual alias-backed
            // documents store the same two leaves, never a second stale POINT.
            // An explicit empty/error leaf remains authoritative over this fallback.
            if (c.Inputs.TryGetValue("CorrectCenterPoint", out var combined))
            {
                Point2? point = null;
                if (combined.Kind == DataKind.Point)
                {
                    if (combined.Cardinality == ValueCardinality.Array)
                    {
                        var points = combined.As<IReadOnlyList<Point2>>();
                        if (points.Count > 0) point = points[0];
                    }
                    else point = combined.As<Point2>();
                }
                return point is { } value ? (0, horizontal ? value.X : value.Y) : (0xe0000314, 0);
            }
            return (missing, 0);
        }
        var x = Axis("CorrectCenterPointX", true);
        if (x.Error != 0 && x.Error != missing)
            throw new DistortionCalibrationRuntimeException(0xe0000314, "CorrectCenterPointX 读取失败（0xe0000314）。");
        var y = Axis("CorrectCenterPointY", false);
        if (y.Error != 0 && y.Error != missing)
            throw new DistortionCalibrationRuntimeException(0xe0000314, "CorrectCenterPointY 读取失败（0xe0000314）。");
        var center = state.Center.Update(x.Error == 0 ? x.Value : null, y.Error == 0 ? y.Value : null);
        return (center.X, center.Y);
    }
    static PortValue Numbers(IEnumerable<float> values) => PortValue.Numbers(values.Select(v => (double)v));
    static Dictionary<string, PortValue> PointOutputs(IReadOnlyList<DistortionCalibrationPoint> points, IReadOnlyList<float> statuses)
    {
        var output = new Dictionary<string, PortValue>
        {
            ["CalibPointNum"] = PortValue.Integer(points.Count),
            ["Calibration Point"] = PortValue.PointArray(points.Select(p => new Point2(p.X, p.Y))),
            ["CalibPointX"] = Numbers(points.Select(p => p.X)),
            ["CalibPointY"] = Numbers(points.Select(p => p.Y)),
            ["CalibPointStatus"] = Numbers(statuses)
        };
        return output;
    }

    sealed class Runtime(DistortionCalibrationPlugin owner) : IModule
    {
        public ModuleDescriptor Descriptor => owner.Descriptor;

        public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext c, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var clock = Stopwatch.StartNew();
            var state = owner.GetState(c.NodeId, c.BaseDirectory, c.Scope);
            lock (state.Gate)
            {
                try
                {
                    foreach (var spec in Descriptor.Parameters)
                        if (!c.Parameters.TryGetValue(spec.Name, out var value) || !GraphValidator.ValidParameter(spec, value))
                            throw new DistortionCalibrationRuntimeException(0xe0000314, "标定参数无效：" + spec.Name);
                    if (!c.Inputs.TryGetValue("InputImage", out var input) || input.Value is not IImageData imageData)
                        throw new DistortionCalibrationRuntimeException(0xe000031c, "未绑定畸变标定输入图像（0xe000031c）。");
                    var source = OwnedMono(imageData);
                    var capturedInputs = c.Inputs.ToDictionary(p => p.Key, p => p.Value);
                    capturedInputs["InImage"] = PortValue.Image(source);
                    state.Inputs = capturedInputs;
                    // Process20550 applies center before ProcessAlg/SDK recognition.
                    var center = Center(c, state);
                    if (source.Width < 64 || source.Height < 64)
                        throw new DistortionCalibrationRuntimeException(0x10103054, "输入图像的宽和高均须至少64像素。");

                    string path = c.Text("CalibPathName");
                    if (path.Length > 0 && !string.Equals(path, state.AppliedPath, StringComparison.Ordinal))
                    {
                        DistortionCalibrationPath.Prepare(path);
                        state.AppliedPath = path;
                    }
                    var cropped = Crop(source, c.Rois);
                    var detected = Recognize(source, cropped, c, token);
                    if (detected.ErrorCode != 0)
                        return Fail(state, detected.ErrorCode, detected.Diagnostic ?? "未识别到有效的畸变标定板。", ModuleStatus.NoMatch, clock);

                    var solved = DistortionCalibrationSolver.SolveFromPointsNativeDetailed(detected.Points,
                        source.Width, source.Height, c.Integer("CorrectModel"), c.Integer("HomoFreedom"),
                        c.Integer("CalibBoardWeightFun"), c.Number("WeightFactor"), new(center.X, center.Y), token);
                    if (solved.ErrorCode != 0 || solved.File is null)
                        return Fail(state, solved.ErrorCode == 0 ? 0x10103002u : solved.ErrorCode,
                            "畸变标定拟合失败。", ModuleStatus.Failed, clock);

                    bool evaluate = c.Boolean("DistortionEvaluationEnable");
                    bool correctedPoints = evaluate && c.Boolean("OutputPostCorrectionEnable");
                    GrayImage? correctedImage = null;
                    IReadOnlyList<DistortionCalibrationPoint> publishedPoints = detected.Points;
                    float evalError = float.NaN;
                    float[]? pointErrors = null;
                    if (evaluate)
                    {
                        correctedImage = (GrayImage)new DistortionCalibrationWarp(solved.File).Correct(source, token);
                        if (correctedPoints)
                        {
                            var correctedDetection = Recognize(correctedImage, Crop(correctedImage, c.Rois), c, token);
                            if (correctedDetection.ErrorCode != 0)
                                return Fail(state, correctedDetection.ErrorCode, correctedDetection.Diagnostic ?? "矫正图像重检测失败。", ModuleStatus.NoMatch, clock);
                            publishedPoints = correctedDetection.Points;
                            var evaluation = DistortionCalibrationPointErrorEstimator.Estimate(publishedPoints,
                                c.Integer("HomoFreedom"), c.Integer("CalibBoardWeightFun"), c.Integer("WeightFactor"));
                            if (evaluation.ErrorCode != 0)
                                return Fail(state, evaluation.ErrorCode, "矫正点误差评估失败。", ModuleStatus.Failed, clock);
                            evalError = evaluation.EvaluationError;
                            pointErrors = evaluation.PointErrors;
                        }
                        else
                        {
                            var evaluation = DistortionCalibrationPointErrorEstimator.Estimate(detected.Points,
                                c.Integer("HomoFreedom"), c.Integer("CalibBoardWeightFun"), c.Integer("WeightFactor"));
                            if (evaluation.ErrorCode != 0 || evaluation.PointErrors.Length != detected.Points.Count)
                                return Fail(state, 0x10103002, "原标定拟合未返回对应的点误差。", ModuleStatus.Failed, clock);
                            evalError = evaluation.EvaluationError;
                            pointErrors = evaluation.PointErrors;
                        }
                    }

                    var output = PointOutputs(publishedPoints,
                        pointErrors is null ? Enumerable.Repeat(1f, publishedPoints.Count).ToArray() : DistortionCalibrationPointEvaluation.Project(pointErrors, (float)c.Number("PointErrorThreshold")).Statuses);
                    output["ModuStatus"] = PortValue.Integer(1);
                    output["EstimationError"] = PortValue.Number(solved.EstimationError);
                    output["ModuRunTime"] = PortValue.Number(clock.Elapsed.TotalMilliseconds);
                    output["ROI"] = PortValue.Region([cropped.Output]);
                    output["DetectCenterX"] = PortValue.Number(cropped.Output.CenterX);
                    output["DetectCenterY"] = PortValue.Number(cropped.Output.CenterY);
                    output["DetectWidth"] = PortValue.Number(cropped.Output.Width);
                    output["DetectHeight"] = PortValue.Number(cropped.Output.Height);
                    output["DetectAngle"] = PortValue.Number(cropped.Output.AngleDegrees);
                    if (evaluate)
                    {
                        var projected = DistortionCalibrationPointEvaluation.Project(pointErrors!, (float)c.Number("PointErrorThreshold"));
                        output["EvaluationError"] = PortValue.Number(evalError);
                        output["CalibPointError"] = Numbers(projected.Errors);
                        output["CalibPointStatus"] = Numbers(projected.Statuses);
                        output["PostCorrectionImage"] = PortValue.Image(correctedImage!);
                    }

                    if (path.Length > 0)
                    {
                        var target = c.ResolvePath(path);
                        try { DistortionCalibrationFileLifecycle.WriteAutomatic(solved.File, target, c.Boolean("RefreshFileEnable")); }
                        catch (DistortionCalibrationFileFailure error) { return Fail(state, error.NativeErrorCode, error.Message, ModuleStatus.Failed, clock); }
                        catch (Exception error) when (error is IOException or UnauthorizedAccessException or ArgumentException)
                        { return Fail(state, 0x8010000e, error.Message, ModuleStatus.Failed, clock); }
                    }

                    state.Artifacts.AcceptFit(solved.File);
                    state.Diagnostic = null;
                    state.Outputs = output.ToDictionary(p => p.Key, p => p.Value);
                    return ValueTask.FromResult(new ModuleResult(ModuleStatus.Success, output) { NativeErrorCode = 0 });
                }
                catch (OperationCanceledException) { throw; }
                catch (DistortionCalibrationRuntimeException error)
                {
                    return Fail(state, error.NativeErrorCode, error.Message, ModuleStatus.Failed, clock);
                }
                catch (Exception error) when (error is ArgumentException or InvalidOperationException or InvalidDataException or IOException or OverflowException or NotSupportedException)
                {
                    return Fail(state, 0x10000019, error.Message, ModuleStatus.Failed, clock);
                }
            }
        }

        static ValueTask<ModuleResult> Fail(State state, uint code, string message, ModuleStatus status, Stopwatch clock)
        {
            state.Artifacts.Fail(code == 0 ? 0x10000019 : code);
            state.Diagnostic = new(code, message);
            var output = new Dictionary<string, PortValue>
            {
                ["ModuStatus"] = PortValue.Integer(0),
                ["ModuRunTime"] = PortValue.Number(clock.Elapsed.TotalMilliseconds)
            };
            // ProcessOutput publishes this round's status/runtime before its success gate.
            // Retain fields not published by the failed round; do not invent native rollback.
            var snapshot = state.Outputs?.ToDictionary(p => p.Key, p => p.Value) ?? new Dictionary<string, PortValue>();
            foreach (var pair in output) snapshot[pair.Key] = pair.Value;
            state.Outputs = snapshot;
            return ValueTask.FromResult(new ModuleResult(status, output,
                "DistortionCalibration", message) { NativeErrorCode = code });
        }
    }
}

sealed class DistortionCalibrationRuntimeException(uint nativeErrorCode, string message) : Exception(message)
{
    public uint NativeErrorCode { get; } = nativeErrorCode;
}




