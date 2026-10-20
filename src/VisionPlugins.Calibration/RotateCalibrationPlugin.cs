using System.Diagnostics;
using System.IO;
using System.Text.Json;
using VisionFramework;
using VisionPluginContracts;
namespace VisionPlugins.Calibration;

public sealed class RotateCalibrationPlugin : IVisionToolPlugin, IWpfToolEditorFactory, IToolAssetPersistence, IToolDisplayProvider, IToolPointInputAliasProvider, IToolConfigurationProvider
{
    public const string Id = "vision.rotate-calibration";
    public IReadOnlyList<ToolPointInputAlias> PointInputAliases { get; } = [new("Image Point", "ImagePointX", "ImagePointY")];
    public ToolManifest Manifest { get; } = new(Id, "1.0.0", "旋转标定", "calibration", "catalog/RotateCalibModuImageEnable.png");
    public ToolDisplayDefinition DisplayDefinition { get; } = new(
        "模块状态:{0}, 旋转轴图像坐标X:{1}, 旋转轴图像坐标Y:{2}, 旋转中心物理坐标X:{3}, 旋转中心物理坐标Y:{4}, 旋转像素平均误差:{5}, 旋转真实平均误差:{6},剩余标定次数:{7}",
        ["ModuStatus", "RotCenterImageX", "RotCenterImageY", "RotCenterWorldX", "RotCenterWorldY", "RotError", "RotWorldError", "remainCount"],
        ["ModuStatus", "RotCenterImageX", "RotCenterImageY", "RotCenterWorldX", "RotCenterWorldY", "RotError", "RotWorldError"]);
    static ParameterDescriptor P<T>(string name, ParameterKind kind, T value, double? min = null, double? max = null) => new(name, kind, true, JsonSerializer.SerializeToElement(value), min, max);
    public ModuleDescriptor Descriptor { get; } = new(Id, "1.0.0", "旋转标定",
        new[] { "ImagePointX", "ImagePointY", "WorldRotateAngle" }.Select(n => new PortDescriptor(n, DataKind.NumberArray, false){SupportsDeferredNumericRead=true}).Concat(new[] { "Image Point" }.Select(n => new PortDescriptor(n, DataKind.Point, false) { Cardinality = ValueCardinality.Array })).Concat(new[] { "RotNum", "RefreshSignal" }.Select(n => new PortDescriptor(n, DataKind.Integer, false){SupportsDeferredIntegerRead=true})).Concat(new[] { ToolDisplayService.PositionXInput, ToolDisplayService.PositionYInput }.Select(n => new PortDescriptor(n, DataKind.Number, false, AcceptInteger: true))).Append(new("InputImage", DataKind.Image, false)).ToArray(),
        new[] { "ModuStatus", "remainCount" }.Select(n => new PortDescriptor(n, DataKind.Integer, false)).Concat(new[] { "ModuRunTime", "RotCenterImageX", "RotCenterImageY", "RotCenterWorldX", "RotCenterWorldY", "RotError", "RotWorldError" }.Select(n => new PortDescriptor(n, DataKind.Number, false))).ToArray(),
        [P("InputWay", ParameterKind.Integer, 0, 0, 1), P("RevOutputFlag", ParameterKind.Boolean, false), P("FilePath", ParameterKind.Text, ""), P("CachedMatrix", ParameterKind.Text, ""), P("CachedMatrixLoaded", ParameterKind.Boolean, false), P(ToolDisplayService.SettingsParameter, ParameterKind.Text, "")]);
    sealed class State { public readonly object Gate = new(); public readonly RotateCalibrationSession Session = new(); public string? AppliedPath; public bool Initialized; public Dictionary<string, PortValue>? DisplayInputs, DisplayOutputs; public ToolDisplayDiagnostic? DisplayDiagnostic; }
    readonly Dictionary<string, State> fallback = new(); readonly object gate = new();
    public void Reset(string nodeId) { lock (gate) foreach (var key in fallback.Keys.Where(k => k.EndsWith("|" + nodeId, StringComparison.Ordinal)).ToArray()) fallback.Remove(key); }
    State Get(string node, string directory, ModuleExecutionScope? scope)
    {
        if (scope is not null) return scope.State(Id, node, () => new State());
        lock (gate) { string key = Path.GetFullPath(directory) + "|" + node; if (!fallback.TryGetValue(key, out var state)) fallback[key] = state = new(); return state; }
    }
    static void Synchronize(State state, IReadOnlyDictionary<string, JsonElement> p, string directory)
    {
        string path = p["FilePath"].GetString() ?? "";
        if (!state.Initialized)
        {
            state.Initialized = true;
            string cache = p.TryGetValue("CachedMatrix", out var json) ? json.GetString() ?? "" : "";
            if (cache.Length > 0)
            {
                try { var bytes = Convert.FromBase64String(cache); if (bytes.Length == 36) { state.Session.Restore(Enumerable.Range(0, 9).Select(i => BitConverter.ToSingle(bytes, 4 * i)).ToArray(), p["CachedMatrixLoaded"].GetBoolean(), path.Length == 0 ? "" : Path.GetFullPath(path, directory)); state.AppliedPath = path; } }
                catch (FormatException) { /* Invalid persisted cache falls back to normal file loading. */ }
            }
        }
        if (state.AppliedPath != path) { state.Session.Load(path.Length == 0 ? "" : Path.GetFullPath(path, directory)); state.AppliedPath = path; }
    }
    public void UpdateConfiguration(ToolEditorContext context, IReadOnlyDictionary<string, JsonElement> values)
    { var state = Get(context.Node.Id, context.BaseDirectory, context.Scope); lock (state.Gate) Synchronize(state, values, context.BaseDirectory); }
    public ToolDisplayFrame? LastDisplay(NodeDocument node, string baseDirectory, ModuleExecutionScope? scope)
    { var state = Get(node.Id, baseDirectory, scope); lock (state.Gate) return state.DisplayInputs is null || state.DisplayOutputs is null ? null : ToolDisplayService.Build(DisplayDefinition, ToolDisplayService.Read(node.Parameters, DisplayDefinition), state.DisplayInputs, state.DisplayOutputs, diagnostic: state.DisplayDiagnostic); }
    static string NativeErrorInfo(uint code) => code switch { 0xe0000001 => "参数错误", 0xe0000008 => "资源未初始化或未准备好", 0xe0000201 => "文件无法打开", 0xe000031c => "订阅参数有误", _ => "" };
    public IModule CreateRuntime() => new Runtime(this);
    public Form CreateEditor(ToolEditorContext context) => throw new InvalidOperationException("Use WPF editor factory.");
    public System.Windows.Window CreateWpfEditor(ToolEditorContext context) => new ToolConfigurationModelWindow(context, CreateConfiguration(context));
    public ToolConfigurationSession CreateConfiguration(ToolEditorContext context) => new CalibrationToolConfigurationProvider(Id, "旋转标定", Descriptor, DisplayDefinition, "RotateCalibModuAlgorithmTab.xml", UpdateConfiguration, algorithmResetResource:"RotateCalibModuAlgorithm.xml", resetParameterWriter:CheckResetParameter, parameterWriter:WriteFilePathParameter, calibrationPathError:message=>ShowPathError(context,message)).CreateConfiguration(context);
    // Frame 42760 passes strlen(value); the sole Rotate default is empty FilePath.
    // Original SetParam 38980 rejects length zero before touching matrix/path/sample state.
    internal static ConfigurationOperationResult? CheckResetParameter(ConfigurationOperationRequest operation) =>
        operation.Kind==ConfigurationOperationKind.WriteParameter && operation.Source.Name=="FilePath" && string.IsNullOrEmpty(operation.Value)
            ? new(true,Error:"0xe0000001") : null;
    void ShowPathError(ToolEditorContext context,string message)
    {
        if(context.ConfigurationNotifications is null||context.ConfigurationNotifications.GetType()==typeof(WpfConfigurationNotificationService))
            RotateCalibrationFileErrorDialog.Show(message,ConfigurationLocalization.ForLanguage("Chinese").Get("Confirm"),context.Mode);
        else context.ConfigurationNotifications.Notify(null,new(ConfigurationNotificationKind.Error,message,Manifest.Name,context.Mode));
    }
    // Original 38980: after its suffix branch, Load* return is reported but FilePath
    // is committed even on failure. Explicit writes reload even the same path.
    ConfigurationOperationResult? WriteFilePathParameter(ToolEditorContext context,IToolConfigurationDispatcher adapter,ConfigurationOperationRequest operation)
    {
        if(operation.Kind!=ConfigurationOperationKind.WriteParameter||operation.Source.Name!="FilePath")return null;
        var state=Get(context.Node.Id,context.BaseDirectory,context.Scope);
        lock(state.Gate){
            string path=operation.Value??"";
            uint code=state.Session.Load(path.Length==0?"":Path.GetFullPath(path,context.BaseDirectory));
            state.Initialized=true;state.AppliedPath=path;
            // Native cache changes above are source behavior; host document rejection
            // below is an independent transport failure, not a native rollback promise.
            var committed=adapter.Dispatch(operation);
            if(!committed.Handled||committed.Error is not null)return committed;
            return code==0?committed:new(true,Error:"0x"+code.ToString("x8")){KeepAttemptedValueOnError=true};
        }
    }
    public ToolAssetSnapshot PrepareSave(NodeDocument node, string directory, ResourceCatalog resources) => PrepareSave(node, directory, resources, null);
    public ToolAssetSnapshot PrepareSave(NodeDocument node, string directory, ResourceCatalog resources, ModuleExecutionScope? scope)
    {
        var state = Get(node.Id, directory, scope); lock (state.Gate)
        {
            Synchronize(state, node.Parameters, directory);
            var parameters = node.Parameters.ToDictionary(p => p.Key, p => p.Value.Clone());
            parameters["CachedMatrix"] = JsonSerializer.SerializeToElement(Convert.ToBase64String(state.Session.Matrix.SelectMany(BitConverter.GetBytes).ToArray()));
            parameters["CachedMatrixLoaded"] = JsonSerializer.SerializeToElement(state.Session.Loaded);
            return new(node with { Parameters = parameters }, []);
        }
    }
    sealed class Runtime(RotateCalibrationPlugin owner) : IModule
    {
        public ModuleDescriptor Descriptor => owner.Descriptor;
        public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context, CancellationToken token)
        {
            token.ThrowIfCancellationRequested(); var watch = Stopwatch.StartNew(); var state = owner.Get(context.NodeId, context.BaseDirectory, context.Scope); RotateCalibrationStep step;
            lock (state.Gate)
            {
                Synchronize(state, context.Parameters, context.BaseDirectory);
                step = state.Session.Run(name=>RotateCalibrationInputReader.Float(context,name),name=>RotateCalibrationInputReader.Integer(context,name),context.Boolean("RevOutputFlag"));
            }
            var nonFinite = step.Outputs.Where(p => !double.IsFinite(p.Value)).ToArray();
            var outputs = step.Outputs.Where(p => double.IsFinite(p.Value)).ToDictionary(p => p.Key, p => p.Key is "ModuStatus" or "remainCount" ? PortValue.Integer((int)p.Value) : PortValue.Number(p.Value));
            outputs["ModuRunTime"] = PortValue.Number(watch.Elapsed.TotalMilliseconds);
            lock (state.Gate) { state.DisplayInputs = context.Inputs.ToDictionary(p => p.Key, p => p.Value); state.DisplayOutputs = outputs; state.DisplayDiagnostic = nonFinite.Length > 0 ? new(step.ErrorCode, "有限数值接口无法表示原厂结果：" + string.Join(", ", nonFinite.Select(p => p.Key + "=" + p.Value))) : step.ErrorCode != 0 ? new(step.ErrorCode, NativeErrorInfo(step.ErrorCode)) : null; }
            // Preserve native code 0 while reporting the explicit host representation boundary.
            // Do not substitute zero for the original NaN/Infinity diagnostic fields.
            if (nonFinite.Length > 0) return ValueTask.FromResult(new ModuleResult(ModuleStatus.Failed, outputs, "NativeNonFiniteResult", "原厂结果超出有限数值接口：" + string.Join(", ", nonFinite.Select(p => p.Key + "=" + p.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)))) { NativeErrorCode = step.ErrorCode });
            return ValueTask.FromResult(new ModuleResult(step.ErrorCode == 0 ? ModuleStatus.Success : ModuleStatus.Failed, outputs, step.ErrorCode == 0 ? null : "RotateCalibration", null) { NativeErrorCode = step.ErrorCode });
        }
    }
}



