using System.Text.Json;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using VisionEditor;
using VisionFramework;

internal static class Program
{
[STAThread]
private static void Main()
{
var checks = 0;
void Check(bool result, string message) { if (!result) throw new Exception(message); checks++; Console.WriteLine("PASS " + message); }
var registry = new ModuleRegistry();
WorkflowCalls.Register(registry);
var descriptor = new ModuleDescriptor("test.group.variable", "1", "可选输入", [new("value", DataKind.Integer)], [new("out", DataKind.Integer)], []);
registry.Register(descriptor, () => new OptionalInput(descriptor));
var body = new ProcedureDocument("body", "Group", [new("consumer", descriptor.Id, "1", "consumer", new())], [], [])
{
    GroupOwner = new("parent", "group"),
    Configuration = new()
    {
        Groups = [new("g", "分组0")], Variables = [new("v", "g", "%same%", "int", "7")],
        Subscriptions = [new("consumer", "value", false, new(WorkflowSourceKind.LocalVariable, "v"))],
        Outputs = [new("o", "result", DataKind.Integer, "consumer", "out")]
    }
};
var group = new NodeDocument("group", WorkflowCalls.ModuleId, WorkflowCalls.Version, "Group", new()) { Call = WorkflowCalls.From(body) };
var parent = new ProcedureDocument("parent", "流程", [group], [], [])
{
    Configuration = new() { Groups = [new("pg", "分组0")], Variables = [new("pv", "pg", "%same%", "int", "99")] }
};
var solution = new SolutionDocument(SolutionStore.CurrentVersion, "group-variable-type", "局部变量类型", [parent, body], []);
ProcedureDocument Body() => solution.Procedures.Single(p => p.Id == "body");
void Save(WorkflowConfiguration value) => solution = solution with { Procedures = solution.Procedures.Select(p => p.Id == "body" ? p with { Configuration = value } : p).ToArray() };
IEnumerable<WorkflowSourceOption> Sources(DataKind? kind) => [];
ExecutionSessionResult Run()
{
    var result = new SolutionExecutor(registry).RunAsync(new(solution, Environment.CurrentDirectory), "parent").GetAwaiter().GetResult();
    Console.WriteLine($"Run {result.Status}: {string.Join(';', result.Nodes.Select(n => n.NodeId + ':' + n.Status + ':' + n.Message))}; calls={string.Join(';', result.Calls.Select(c => c.Key + ':' + c.Value.Status + ':' + string.Join(',', c.Value.OutputErrors)))}");
    return result;
}
var initial = Run();
Check(initial.Status == SessionStatus.Success && initial.Calls["group"].WorkflowOutputs["result"].As<long>() == 7,
    "Group consumer receives original local variable value");
var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
var window = new WorkflowLocalVariablesWindow(Body, Save, Sources, (_, _) => null, registry)
{
    ShowInTaskbar = false, WindowStartupLocation = WindowStartupLocation.Manual, Left = -20000, Top = -20000
};
window.Show(); Pump();
var type = Children(window).OfType<ComboBox>().Single(c => c.ItemsSource is string[] values && values.Contains("float"));
type.SelectedItem = "float"; Pump(); Pump();
Check(Body().Configuration.Variables.Single().Type == "float", "Actual WPF type editor commits new variable type");
Check(Body().Configuration.Variables.Single().Value == "7", "Type change preserves convertible initial value");
Check(Body().Configuration.Subscriptions.Count == 0, "Type change removes old Group local variable target subscription");
Check(solution.Procedures.Single(p => p.Id == "parent").Configuration.Variables.Single().Value == "99", "Group edit does not change parent local variable scope");
var unbound = Run();
Check(unbound.Status == SessionStatus.Failed && (unbound.Nodes.Single().Message?.Contains("MissingInput", StringComparison.Ordinal) ?? false),
    "Runtime rejects now-unbound required Group input instead of consuming stale integer subscription");
Save(Body().Configuration with { Subscriptions = [new("consumer", "value", false, new(WorkflowSourceKind.IntegerConstant, "0"))] });
var rebound = Run();
Check(rebound.Status == SessionStatus.Success && rebound.Calls["group"].WorkflowOutputs["result"].As<long>() == 0,
    "Rebinding compatible integer constant restores real Group execution");
var path = Path.GetFullPath("artifacts/group-variable-type.vmx.json"); Directory.CreateDirectory(Path.GetDirectoryName(path)!);
SolutionStore.Save(path, solution); solution = SolutionStore.Load(path);
Check(Body().Configuration.Variables.Single().Type == "float" && Body().Configuration.Subscriptions.Single().Source.Kind == WorkflowSourceKind.IntegerConstant,
    "Type and replacement binding survive save and reopen");
var reopened = Run();
Check(reopened.Status == SessionStatus.Success && reopened.Calls["group"].WorkflowOutputs["result"].As<long>() == 0,
    "Reopened solution still executes without stale local variable binding");
window.Close(); app.Shutdown(); Console.WriteLine($"PASS {checks} Group variable type checks");

static void Pump() { var frame = new DispatcherFrame(); Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => frame.Continue = false)); Dispatcher.PushFrame(frame); }
static IEnumerable<FrameworkElement> Children(DependencyObject root)
{
    if (root is FrameworkElement element) yield return element;
    for (var i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(root); i++)
        foreach (var child in Children(System.Windows.Media.VisualTreeHelper.GetChild(root, i))) yield return child;
}
}
}
sealed class OptionalInput(ModuleDescriptor descriptor) : IModule
{
    public ModuleDescriptor Descriptor => descriptor;
    public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context, CancellationToken token) => ValueTask.FromResult(
        new ModuleResult(ModuleStatus.Success, new Dictionary<string, PortValue> { ["out"] = PortValue.Integer(context.Inputs.TryGetValue("value", out var value) ? value.As<long>() : 0) }));
}
