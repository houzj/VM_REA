using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;

static class Program
{
    const BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
    [STAThread] static int Main(string[] args)
    {
        try
        {
            if(args.Length!=1)throw new ArgumentException("Supply an owned audit directory.");
            string output=Path.Combine(Path.GetFullPath(args[0]),"history-host-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(output);
            var checks=new List<string>();void Check(bool condition,string label){if(!condition)throw new Exception(label);checks.Add(label);Console.WriteLine("PASS "+label);}
            var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};var window=new MainWindow();
            object Field(string name)=>typeof(MainWindow).GetField(name,Private)!.GetValue(window)!;
            object? Call(string name,params object[] values)=>typeof(MainWindow).GetMethod(name,Private)!.Invoke(window,values);
            string Text(string name)=>((TextBlock)window.FindName(name)).Text;
            var doc=(GraphEditorDocument)Field("doc");var history=(FrameworkRunHistory)Field("frameworkRunHistory");
            var grid=(DataGrid)window.FindName("HistoryGrid");
            Check(ReferenceEquals(grid.ItemsSource,history.Entries),"Actual HistoryGrid binds bounded host collection");
            doc.NewSolution();string owner=doc.CurrentProcedureId;
            var session=new ExecutionSessionResult(SessionStatus.Success,[]);
            for(int i=0;i<750;i++)Call("RecordFrameworkRunHistory",owner,session);
            Check(grid.Items.Count==500&&history.Entries.Count==500,"Real grid consumes only last 500 records");
            Check(((FrameworkRunHistoryEntry)grid.Items[0]).Seq==750&&((FrameworkRunHistoryEntry)grid.Items[499]).Seq==251,"Real grid newest first and stable sequence after eviction");
            string first=doc.AddOwnedGroup(100,100);string second=doc.AddOwnedGroup(300,100);
            var a=new NodeRunRecord(first,NodeRunStatus.Success,new Dictionary<string,PortValue>(),12.25);
            var b=new NodeRunRecord(second,NodeRunStatus.Success,new Dictionary<string,PortValue>(),3.5);
            var records=(Dictionary<string,NodeRunRecord>)Field("results");var sessions=(Dictionary<string,ExecutionSessionResult>)Field("workflowSessions");
            records[first]=a;records[second]=b;sessions[owner]=new(SessionStatus.Success,[a,b]);
            typeof(MainWindow).GetField("selected",Private)!.SetValue(window,second);Call("RefreshRight");
            Check(Text("ModuleTimeText")=="3.50","Actual right refresh consumes selected NodeRunRecord elapsed");
            Check(Text("ProcessTimeText")=="15.75","Actual status bar reads current session timing");
            Check(Text("AlgorithmTimeText")==FrameworkResultTiming.Unavailable,"Algorithm-only timing remains explicitly unavailable");
            typeof(MainWindow).GetField("selected",Private)!.SetValue(window,first);Call("RefreshRight");
            Check(Text("ModuleTimeText")=="12.25","Changing real host selection switches status timing source");
            records.Clear();Call("RefreshRight");
            Check(Text("ModuleTimeText")==FrameworkResultTiming.Unavailable,"Clearing visible result source removes stale module number");
            doc.NewSolution();
            Check(history.Entries.Count==0&&grid.Items.Count==0,"Actual Doc.NewSolution event clears independent grid history");
            Call("RecordFrameworkRunHistory",doc.CurrentProcedureId,session);
            Check(history.Entries.Single().Seq==1,"Actual replacement event resets independent summary sequence");
            Check(Text("ModuleTimeText")!="12.25"&&Text("ProcessTimeText")!="15.75","Actual replacement removes prior solution timing source");
            string path=Path.Combine(output,"replacement.vmx.json");doc.Save(path);doc.Load(path);
            Check(history.Entries.Count==0,"Actual successful Load also clears independent summary");
            window.Close();app.Shutdown();
            File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(new{passed=checks.Count,checks,scope="actual WPF host with controlled managed result fixtures; not original SDK execution or interactive desktop acceptance"},new JsonSerializerOptions{WriteIndented=true}));
            Console.WriteLine("PASS "+checks.Count+" host checks; audit="+output);return 0;
        }
        catch(Exception error){Console.Error.WriteLine(error);return 1;}
    }
}
