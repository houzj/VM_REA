using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using VisionEditor;
using VisionFramework;

internal static class Program
{
internal const string LongPort = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMN";
[STAThread]
private static void Main()
{
int checks = 0;
void Check(bool condition, string label) { if (!condition) throw new Exception(label); checks++; Console.WriteLine("PASS " + label); }

var registry = new ModuleRegistry();
WorkflowCalls.Register(registry);
var producer = new ModuleDescriptor("test.group.source", "1", "源", [],
    [new("scalar", DataKind.Integer), new("array", DataKind.Integer) { Cardinality = ValueCardinality.Array },
     new("out", DataKind.Integer), new(LongPort, DataKind.Integer)],
    [new("gain", ParameterKind.Integer, true, JsonSerializer.SerializeToElement(11), 0, 100),
     new("other", ParameterKind.Integer, true, JsonSerializer.SerializeToElement(2), 0, 100)]);
var consumer = new ModuleDescriptor("test.group.consumer", "1", "消费", [new("value", DataKind.Integer)], [new("value", DataKind.Integer)],
    [new("gain", ParameterKind.Integer, true, JsonSerializer.SerializeToElement(99), 0, 100)]);
registry.Register(producer, () => new SourceModule(producer));
registry.Register(consumer, () => new ConsumerModule(consumer));
var body = new ProcedureDocument("body", "Group", [new("source", producer.Id, "1", "source", new()),
    new("source2", producer.Id, "1", "source2", new() { ["gain"] = JsonSerializer.SerializeToElement(3) }),
    new("consumer", consumer.Id, "1", "consumer", new())], [], [new("source", "consumer")])
{
    GroupOwner = new("parent", "group"),
    Configuration = new()
    {
        Inputs = [new("input", "in0", DataKind.Integer)],
        Outputs = [new("output", "out", DataKind.Integer, "consumer", "value")],
        Subscriptions = [new("consumer", "value", false, new(WorkflowSourceKind.WorkflowInput, "input"))],
        RunParameters = [new("proxy", "consumer", "value", "输入值", true), new("gain-proxy", "source", "gain", "增益")]
    }
};
var group = new NodeDocument("group", WorkflowCalls.ModuleId, WorkflowCalls.Version, "Group", new()) { Call = WorkflowCalls.From(body) };
var parent = new ProcedureDocument("parent", "流程", [new("parent-source", producer.Id, "1", "parent-source", new()), group], [], [new("parent-source", "group")])
{
    Configuration = new()
    {
        Groups = [new("variables", "局部变量")],
        Variables = [new("parent-value", "variables", "parentValue", "int", "7")],
        Subscriptions = [new("group", "in0", false, new(WorkflowSourceKind.LocalVariable, "parent-value"))]
    }
};
var solution = new SolutionDocument(SolutionStore.CurrentVersion, "group-source", "Group来源", [parent, body], []);
void SaveGroupInputs(IReadOnlyList<WorkflowInterfaceItem> items, IReadOnlyDictionary<string, WorkflowValueReference?> bindings)
{
    var editPath = Path.GetFullPath("artifacts/group-input-edit.vmx.json"); Directory.CreateDirectory(Path.GetDirectoryName(editPath)!);
    SolutionStore.Save(editPath, solution);
    var edit = new VisionEditor.Core.GraphEditorDocument(registry); edit.Load(editPath);
    edit.ConfigureOwnedGroupInputs("group", items, bindings);
    solution = edit.Document;
}
ProcedureDocument Parent() => solution.Procedures.Single(p => p.Id == "parent");
ProcedureDocument Body() => solution.Procedures.Single(p => p.Id == "body");
NodeDocument Group() => Parent().Nodes.Single(n => n.Id == "group");
void ReplaceBody(ProcedureDocument updated) => solution = solution with { Procedures = solution.Procedures.Select(p => p.Id == "body" ? updated : p).ToArray() };
IEnumerable<WorkflowSourceOption> SourceOptions(DataKind? kind) => Body().Nodes.SelectMany(n => registry.GetDescriptor(n).Outputs
    .Where(p => !kind.HasValue || DataKinds.CanFlow(p.Kind, kind.Value))
    .Select(p => new WorkflowSourceOption(n.Id, p.Name, p.Kind, n.Name + "." + p.Name) { Cardinality = p.Cardinality }));
IEnumerable<WorkflowSourceOption> ParentSourceOptions(DataKind kind) => Parent().Nodes.Where(n => n.Id == "parent-source")
    .SelectMany(n => registry.GetDescriptor(n).Outputs.Where(p => DataKinds.CanFlow(p.Kind, kind))
        .Select(p => new WorkflowSourceOption(n.Id, p.Name, p.Kind, n.Name + "." + p.Name) { Cardinality = p.Cardinality,
            Reference = new(WorkflowSourceKind.NodeOutput, n.Id, p.Name) }));
ExecutionSessionResult? last = null;
var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
var window = new SubprogramCallWindow(Parent, Group, () => solution.Procedures, _ => { }, (_, _) => { }, ParentSourceOptions,
    async () => last = await new GroupDebugExecutor(registry).RunAsync(new(solution, Environment.CurrentDirectory), "parent", "group"),
    () => { }, () => last, saveGroupConfiguration: c => ReplaceBody(Body() with { Configuration = c }),
    groupSources: SourceOptions, groupRegistry: registry,
    saveGroupInputs: SaveGroupInputs, setGroupProxySource: (id, source) =>
    {
        var current = Body();
        var proxy = current.Configuration.RunParameters.Single(p => p.Id == id);
        ReplaceBody(current with { Configuration = current.Configuration with
        {
            Subscriptions = current.Configuration.Subscriptions.Where(s => !(s.NodeId == proxy.NodeId && s.Target == proxy.Parameter && !s.IsParameter))
                .Concat(source is null ? [] : [new WorkflowSubscription(proxy.NodeId, proxy.Parameter, false, source)]).ToArray()
        } });
    });
window.ShowInTaskbar = false; window.WindowStartupLocation = WindowStartupLocation.Manual; window.Left = window.Top = -20000;
window.Show(); Pump();
var tabs = (TabControl)window.FindName("ConfigurationTabs"); tabs.SelectedIndex = 2; Pump();
tabs.SelectedIndex = 0; Pump();
Exception? inputPickerError = null;
_ = window.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() =>
{
    WorkflowSourcePicker? picker = null;
    try
    {
        picker = Application.Current.Windows.OfType<WorkflowSourcePicker>().Single(); picker.UpdateLayout();
        var labels = Children(picker).OfType<CheckBox>().Select(c => c.Content?.ToString()).ToArray();
        Check(labels.Contains("scalar"), "Group input picker shows compatible parent scalar output");
        Check(!labels.Contains("array"), "Group input picker excludes parent array output from scalar signature");
        picker.Close();
    }
    catch (Exception ex) { inputPickerError = ex; picker?.Close(); }
}));
Children(window).OfType<Button>().Single(b => Equals(b.Tag, "GroupInputSource:input"))
    .RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent)); Pump();
if (inputPickerError is not null) throw inputPickerError;
tabs.SelectedIndex = 2; Pump();
var button = Children(window).OfType<Button>().Single(b => Equals(b.Tag, "GroupRunSource:proxy"));
Exception? pickerError = null;
_ = window.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() =>
{
    WorkflowSourcePicker? picker = null;
    try
    {
        picker = Application.Current.Windows.OfType<WorkflowSourcePicker>().Single(); picker.UpdateLayout();
        var labels = Children(picker).OfType<CheckBox>().Select(c => c.Content?.ToString()).ToArray(); Console.WriteLine("Picker labels: " + string.Join(" | ", labels));
        Check(labels.Contains("scalar"), "Group scalar picker shows compatible scalar output");
        Check(labels.Contains("in0"), "Group scalar picker shows compatible Group input");
        Check(!labels.Contains("array"), "Group scalar picker excludes integer array output");
        Children(picker).OfType<CheckBox>().First(c => Equals(c.Content, "scalar")).IsChecked = true;
        Children(picker).OfType<Button>().Single(b => Equals(b.Content, "确定")).RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
    }
    catch (Exception ex) { pickerError = ex; picker?.Close(); }
}));
button.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent)); Pump();
if (pickerError is not null) throw pickerError;
Check(Body().Configuration.Subscriptions.Single(s => s.NodeId == "consumer").Source == new WorkflowValueReference(WorkflowSourceKind.NodeOutput, "source", "scalar"), "Picker commits selected source into Group execution data");
last = new GroupDebugExecutor(registry).RunAsync(new(solution, Environment.CurrentDirectory), "parent", "group").GetAwaiter().GetResult();
Console.WriteLine($"Run: {last.Status}, outputErrors={string.Join(';',last.OutputErrors)}, nodes={string.Join(';',last.Nodes.Select(n=>n.NodeId+':'+n.Status+':'+n.ErrorCode+':'+n.Message))}, call={string.Join(';',last.Calls.Select(c=>c.Key+':'+c.Value.Status+':'+string.Join(',',c.Value.OutputErrors)))}");
Check(last.Status == SessionStatus.Success && last.Calls["group"].WorkflowOutputs["out"].As<long>() == 11, "Real Group run consumes selected scalar source");
var path = Path.GetFullPath("artifacts/group-source-boundary.vmx.json"); Directory.CreateDirectory(Path.GetDirectoryName(path)!);
SolutionStore.Save(path, solution); solution = SolutionStore.Load(path);
last = new GroupDebugExecutor(registry).RunAsync(new(solution, Environment.CurrentDirectory), "parent", "group").GetAwaiter().GetResult();
Check(last.Status == SessionStatus.Success && last.Calls["group"].WorkflowOutputs["out"].As<long>() == 11, "Saved and reopened Group keeps source and executable result");
var rejected = false;
try
{
    var invalidBody = Body() with { Configuration = Body().Configuration with
    {
        Subscriptions = Body().Configuration.Subscriptions.Select(s => s.NodeId == "consumer"
            ? s with { Source = new(WorkflowSourceKind.NodeOutput, "source", "array") } : s).ToArray()
    } };
    WorkflowSubscriptions.Validate(invalidBody, registry);
}
catch (InvalidDataException) { rejected = true; }
Check(rejected, "Runtime validator still rejects manually inserted array-to-scalar binding");
var configurationWindow = window.CreateRunConfigurationWindow(); configurationWindow.Show(); Pump();
Exception? relationPickerError = null;
_ = window.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() =>
{
    WorkflowSourcePicker? picker = null;
    try
    {
        picker = Application.Current.Windows.OfType<WorkflowSourcePicker>().Single(); picker.UpdateLayout();
        var labels = Children(picker).OfType<CheckBox>().Select(c => c.Content?.ToString()).ToArray();
        Check(labels.Count(l => l == "gain") == 1, "Relation picker includes matching tool type and parameter exactly once");
        Check(!labels.Contains("other"), "Relation picker excludes different parameter of same integer kind");
        Children(picker).OfType<CheckBox>().Single(c => Equals(c.Content, "gain")).IsChecked = true;
        Children(picker).OfType<Button>().Single(b => Equals(b.Content, "确定"))
            .RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
    }
    catch (Exception ex) { relationPickerError = ex; picker?.Close(); }
}));
Children(configurationWindow).OfType<Button>().Single(b => Equals(b.Tag, "GroupProxyRelations:gain-proxy"))
    .RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent)); Pump();
if (relationPickerError is not null) throw relationPickerError;
configurationWindow.Close(); Pump();
Check(Body().Configuration.RunParameters.Single(p => p.Id == "gain-proxy").Relations?.Single() == new WorkflowRunTarget("source2", "gain"),
    "Relation picker commits matching secondary tool parameter");
SolutionStore.Save(path, solution);
var editor = new VisionEditor.Core.GraphEditorDocument(registry); editor.Load(path);
editor.SetGroupRunParameterValue("body", "gain-proxy", JsonSerializer.SerializeToElement(5));
Check(editor.Document.Procedures.Single(p => p.Id == "body").Nodes.Where(n => n.ModuleId == producer.Id)
    .All(n => n.Parameters["gain"].GetInt32() == 5), "Group parameter edit synchronizes both related real nodes");
var synchronized = new GroupDebugExecutor(registry).RunAsync(new(editor.Document, editor.BaseDirectory), "parent", "group").GetAwaiter().GetResult();
Check(synchronized.Status == SessionStatus.Success && synchronized.Calls["group"].WorkflowOutputs["out"].As<long>() == 5,
    "Related parameter value participates in actual Group node execution");
editor.Save(path); editor.Load(path);
Check(editor.Document.Procedures.Single(p => p.Id == "body").Configuration.RunParameters.Single(p => p.Id == "gain-proxy")
    .Relations?.Single() == new WorkflowRunTarget("source2", "gain"), "Parameter relation survives save and reopen");
solution = editor.Document; tabs.SelectedIndex = 0; Pump();
Exception? multiInputError = null;
_ = window.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() =>
{
    WorkflowSourcePicker? picker = null;
    try
    {
        picker = Application.Current.Windows.OfType<WorkflowSourcePicker>().Single(); picker.UpdateLayout();
        Children(picker).OfType<CheckBox>().Single(c => Equals(c.Content, "包含模块名")).IsChecked = false;
        Children(picker).OfType<CheckBox>().Single(c => Equals(c.Content, "out")).IsChecked = true;
        Children(picker).OfType<CheckBox>().Single(c => Equals(c.Content, LongPort)).IsChecked = true;
        Children(picker).OfType<Button>().Single(b => Equals(b.Content, "确定"))
            .RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
    }
    catch (Exception ex) { multiInputError = ex; picker?.Close(); }
}));
Children(window).OfType<Button>().Single(b => Equals(b.Tag, "GroupInputAdd"))
    .RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent)); Pump();
if (multiInputError is not null) throw multiInputError;
Check(Body().Configuration.Inputs.Any(i => i.Name == "out0"), "Native multi-select name avoids existing Group output using first numeric suffix");
Check(Body().Configuration.Inputs.Any(i => i.Name == LongPort[..32]), "Native multi-select name truncates source to 32 characters");
Check(Parent().Configuration.Subscriptions.Count(s => s.NodeId == "group" && s.Source.Kind == WorkflowSourceKind.NodeOutput) == 2,
    "Multi-select commits two parent-to-Group input bindings with stable identities");
var parentRun = new SolutionExecutor(registry).RunAsync(new(solution, Environment.CurrentDirectory), "parent").GetAwaiter().GetResult();
Check(parentRun.Status == SessionStatus.Success && parentRun.Calls["group"].WorkflowOutputs["out"].As<long>() == 5,
    "Full parent workflow runs selected source inputs and related Group parameters");
SolutionStore.Save(path, solution); solution = SolutionStore.Load(path);
parentRun = new SolutionExecutor(registry).RunAsync(new(solution, Environment.CurrentDirectory), "parent").GetAwaiter().GetResult();
Check(parentRun.Status == SessionStatus.Success && parentRun.Calls["group"].WorkflowOutputs["out"].As<long>() == 5,
    "Multi-selected Group input bindings survive save and reopen with identical runtime result");
var categories = Enumerable.Range(0, 32).Select(i => new WorkflowRunCategory("category-" + i, "分组" + i)).ToArray();
ReplaceBody(Body() with { Configuration = Body().Configuration with { RunCategories = categories } });
var limitConfiguration = window.CreateRunConfigurationWindow(); limitConfiguration.Show(); Pump();
Children(limitConfiguration).OfType<Button>().Single(b => Equals(b.Tag, "GroupCategoryAdd"))
    .RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent)); Pump();
Check(Body().Configuration.RunCategories!.Count == 32 && ((TextBlock)window.FindName("Error")).Text.Contains("32"),
    "Native default Group run-category limit reports error without adding a 33rd category");
limitConfiguration.Close(); Pump();
parentRun = new SolutionExecutor(registry).RunAsync(new(solution, Environment.CurrentDirectory), "parent").GetAwaiter().GetResult();
Check(parentRun.Status == SessionStatus.Success && parentRun.Calls["group"].WorkflowOutputs["out"].As<long>() == 5,
    "Rejected category addition leaves real multi-target Group execution unchanged");
SolutionStore.Save(path, solution); solution = SolutionStore.Load(path);
Check(Body().Configuration.RunCategories!.Count == 32 && Body().Configuration.RunCategories![0].Id == categories[0].Id,
    "Rejected category addition preserves category identities after save and reopen");
var originalInputs = Body().Configuration.Inputs;
var limitInputs = originalInputs.Concat(Enumerable.Range(originalInputs.Count, 1024 - originalInputs.Count)
    .Select(i => new WorkflowInterfaceItem("limit-" + i, "limit" + i, DataKind.Integer))).ToArray();
SaveGroupInputs(limitInputs, new Dictionary<string, WorkflowValueReference?>());
tabs.SelectedIndex = 1; Pump(); tabs.SelectedIndex = 0; Pump();
Children(window).OfType<ComboBox>().Single(c => Equals(c.Tag, "GroupInputAddType")).SelectedIndex = 1;
Children(window).OfType<Button>().Single(b => Equals(b.Tag, "GroupInputAdd"))
    .RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent)); Pump();
Check(Body().Configuration.Inputs.Count == 1024 && ((TextBlock)window.FindName("Error")).Text.Contains("1024"),
    "Native single-type Group input limit reports error without mutating 1024 existing inputs");
SolutionStore.Save(path, solution); solution = SolutionStore.Load(path);
Check(Body().Configuration.Inputs.Count == 1024 && Body().Configuration.Inputs.Take(originalInputs.Count)
    .Select(i => i.Id).SequenceEqual(originalInputs.Select(i => i.Id)),
    "Rejected input addition preserves existing identities after save and reopen");
window.Close(); app.Shutdown(); Console.WriteLine($"PASS {checks} Group source WPF and runtime checks");
}

static void Pump() { var frame = new DispatcherFrame(); Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => frame.Continue = false)); Dispatcher.PushFrame(frame); }
static IEnumerable<FrameworkElement> Children(DependencyObject root)
{
    if (root is FrameworkElement element) yield return element;
    for (var i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(root); i++)
        foreach (var child in Children(System.Windows.Media.VisualTreeHelper.GetChild(root, i))) yield return child;
}
}

sealed class SourceModule(ModuleDescriptor descriptor) : IModule
{
    public ModuleDescriptor Descriptor => descriptor;
    public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context, CancellationToken token) => ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,
        new Dictionary<string, PortValue> { ["scalar"] = PortValue.Integer(context.Integer("gain")),
            ["array"] = PortValue.IntegerArray([context.Integer("gain"), 22]),
            ["out"] = PortValue.Integer(context.Integer("gain")),
            [Program.LongPort] = PortValue.Integer(context.Integer("gain")) }));
}
sealed class ConsumerModule(ModuleDescriptor descriptor) : IModule
{
    public ModuleDescriptor Descriptor => descriptor;
    public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context, CancellationToken token) => ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,
        new Dictionary<string, PortValue> { ["value"] = PortValue.Integer(context.Input<long>("value")) }));
}
