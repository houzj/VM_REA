using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;

static class DynamicOutputPresentationChecks
{
    static IEnumerable<DependencyObject> Children(DependencyObject root)
    {
        for(int i=0;i<VisualTreeHelper.GetChildrenCount(root);i++)
        {
            var child=VisualTreeHelper.GetChild(root,i);yield return child;
            foreach(var nested in Children(child))yield return nested;
        }
    }

    public static void Run(MainWindow host,Action<bool,string> check,Action pump,Action<Task> wait)
    {
        object? Invoke(string name,params object[] args)=>typeof(MainWindow).GetMethod(name,BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(host,args);
        T Field<T>(string name)=>(T)typeof(MainWindow).GetField(name,BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(host)!;
        var document=Field<GraphEditorDocument>("doc");var registry=Field<ModuleRegistry>("modules");
        document.NewSolution();var procedure=document.CurrentProcedureId;
        var initial=new ModuleDescriptor("test.dynamic-presentation","1","动态输出检查",[],[new("OldPort",DataKind.Text)],[]){SupportsDynamicOutputs=true};
        var updated=initial with{Outputs=[new PortDescriptor("DecodedPath",DataKind.Number,false){Cardinality=ValueCardinality.Array}]};
        registry.Register(initial,()=>new DynamicOutputPresentationModule(initial,updated));
        var nodeId=document.AddNode(initial.Id,initial.Version,160,100);
        document.AddNode("vision.image-source","1.0.0",400,100);
        var persistedBefore=JsonSerializer.Serialize(document.Document);
        pump();
        check((double)Invoke("NodeVisualHeight",document.CurrentProcedure.Nodes.Single(n=>n.ModuleId=="vision.image-source"))! == 30,"Static tool canvas geometry retains its original height");
        var task=(Task)Invoke("Run")!;wait(task);pump();
        var sessions=Field<Dictionary<string,ExecutionSessionResult>>("workflowSessions");var session=sessions[procedure];
        check(session.NodeDescriptors[nodeId].Outputs.Single().Name=="DecodedPath","WPF run consumes the final dynamic descriptor from ExecutionSessionResult");
        var card=(Canvas)Field<Dictionary<string,Canvas>>("nodeCards")[nodeId];
        var portLabel=Children(card).OfType<TextBlock>().SingleOrDefault(t=>t.Text.Contains("DecodedPath"));
        check(card.Height==64&&portLabel is not null&&portLabel.ToolTip?.ToString().Contains("类型：Number · 基数：数组 · 可选")==true,"WPF canvas refresh renders dynamic port name, type, cardinality and optionality while expanding only that card");
        check(JsonSerializer.Serialize(document.Document)==persistedBefore,"Runtime descriptor presentation leaves the saved workflow document unchanged");
        Field<Dictionary<string,NodeRunRecord>>("results").TryGetValue(nodeId,out _);
        typeof(MainWindow).GetField("selected",BindingFlags.Instance|BindingFlags.NonPublic)!.SetValue(host,nodeId);
        var tabs=(TabControl)host.FindName("DisplayTabs");tabs.SelectedIndex=1;Invoke("RefreshRight");pump();
        var rows=((DataGrid)host.FindName("ModuleResultsGrid")).Items.Cast<object>().ToArray();
        check(rows.Any(row=>(string?)row.GetType().GetProperty("Port")?.GetValue(row)=="运行端口 · DecodedPath")&&rows.Any(row=>(string?)row.GetType().GetProperty("Port")?.GetValue(row)=="DecodedPath"),"Module result view distinguishes refreshed schema ports from values emitted by the run");
        var schemaRow=rows.Single(row=>(string?)row.GetType().GetProperty("Port")?.GetValue(row)=="运行端口 · DecodedPath");
        check((string?)schemaRow.GetType().GetProperty("Kind")?.GetValue(schemaRow)=="Number[] · 数组 · 可选"&&(string?)schemaRow.GetType().GetProperty("Value")?.GetValue(schemaRow)=="[2]","Result view reports dynamic type/cardinality/requiredness separately from the emitted value");

        var sinkDescriptor=new ModuleDescriptor("test.dynamic-presentation-sink","1","旧连线消费者",[new("Input",DataKind.Text)],[],[]);var sinkCalls=0;
        registry.Register(sinkDescriptor,()=>new DynamicOutputPresentationSink(sinkDescriptor,()=>sinkCalls++));var sinkId=document.AddNode(sinkDescriptor.Id,sinkDescriptor.Version,460,100);document.Connect(nodeId,"OldPort",sinkId,"Input");
        var withSavedLink=JsonSerializer.Serialize(document.Document);var second=(Task)Invoke("Run")!;wait(second);pump();session=sessions[procedure];
        check(session.Status==SessionStatus.Failed&&session.Nodes.Single(n=>n.NodeId==sinkId).ErrorCode=="DynamicConnectionInvalid"&&sinkCalls==0,"UI-driven execution blocks the old saved link before running its downstream consumer");
        check(JsonSerializer.Serialize(document.Document)==withSavedLink&&document.CurrentProcedure.Connections.Any(c=>c.FromNode==nodeId&&c.FromPort=="OldPort"&&c.ToNode==sinkId&&c.ToPort=="Input"),"Dynamic refresh leaves the saved stale link byte-for-byte unchanged and does not reconnect it");
    }
}

sealed class DynamicOutputPresentationModule(ModuleDescriptor descriptor,ModuleDescriptor updated):IModule
{
    public ModuleDescriptor Descriptor=>descriptor;
    public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken cancellationToken)=>ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"DecodedPath",PortValue.NumberArray([2])}}){UpdatedDescriptor=updated});
}

sealed class DynamicOutputPresentationSink(ModuleDescriptor descriptor,Action executed):IModule
{
    public ModuleDescriptor Descriptor=>descriptor;
    public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken cancellationToken){executed();return ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>()));}
}
