using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;

static class Program
{
    const BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
    [STAThread] static int Main(string[] args)
    {
        try
        {
            if(args.Length is <1 or >2)throw new ArgumentException("Usage: output-parent [observe-unfixed]");
            bool observe=args.Length==2&&args[1]=="observe-unfixed";
            string output=Path.Combine(Path.GetFullPath(args[0]),"owner-host-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(output);
            AppContext.SetSwitch("VisionEditor.DisableAutomaticCameraDiscovery",true);
            var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
            var window=new MainWindow();object Field(string name)=>typeof(MainWindow).GetField(name,Private)!.GetValue(window)!;
            object? Call(string name,params object[] values)=>typeof(MainWindow).GetMethod(name,Private)!.Invoke(window,values);
            var doc=(GraphEditorDocument)Field("doc");var modules=(ModuleRegistry)Field("modules");
            var sessions=(Dictionary<string,ExecutionSessionResult>)Field("workflowSessions");var history=(FrameworkRunHistory)Field("frameworkRunHistory");
            var entered=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);var release=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var descriptor=new ModuleDescriptor("test.epoch-blocked","1","epoch-blocked",[],[],[]);modules.Register(descriptor,()=>new PendingModule(descriptor,entered,release));
            doc.NewSolution();string procedure=doc.CurrentProcedureId;string node=doc.AddNode(descriptor.Id,"1",100,100);
            string path=Path.Combine(output,"same-id.vmx.json");doc.Save(path);string solution=doc.SolutionId;
            var pending=(Task<bool>)Call("RunCore",procedure,CancellationToken.None)!;PumpUntil(entered.Task);
            doc.Load(path);
            bool identitiesReused=doc.SolutionId==solution&&doc.CurrentProcedureId==procedure&&doc.CurrentProcedure.Nodes.Single().Id==node;
            bool cleanBefore=sessions.Count==0&&history.Entries.Count==0;
            release.SetResult();PumpUntil(pending);
            bool staleSession=sessions.ContainsKey(procedure);int staleHistory=history.Entries.Count;
            bool isolated=!staleSession&&staleHistory==0;
            doc.NewSolution();
            var configEntered=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);var configRelease=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var configDescriptor=new ModuleDescriptor("test.config-owner-blocked","1","config-owner-blocked",[],[],[]);
            modules.Register(configDescriptor,()=>new PendingModule(configDescriptor,configEntered,configRelease));
            string configNode=doc.AddNode(configDescriptor.Id,"1",100,100);var configDocumentNode=doc.CurrentProcedure.Nodes.Single();bool configurationAlive=true;
            var basic=new ToolEditorContext(configDocumentNode,doc.BaseDirectory,_=>{},_=>Task.FromResult(new NodeRunRecord(configNode,NodeRunStatus.Success,new Dictionary<string,PortValue>(),0))){ConfigurationActive=()=>configurationAlive};
            var connected=(ToolEditorContext)Call("ConnectToolEditorServices",basic,configNode)!;
            Task<NodeRunRecord> oldConfiguration=connected.Run(new());PumpUntil(configEntered.Task);
            configurationAlive=false;configRelease.SetResult();PumpUntil(oldConfiguration);
            var configurationResults=(Dictionary<string,Dictionary<string,NodeRunRecord>>)Field("configurationResults");
            bool staleConfiguration=configurationResults.GetValueOrDefault(doc.CurrentProcedureId)?.ContainsKey(configNode)==true;
            configurationResults.Clear();
            var fresh=(ToolEditorContext)Call("ConnectToolEditorServices",basic with{ConfigurationActive=()=>true},configNode)!;
            var currentConfiguration=fresh.Run(new());PumpUntil(currentConfiguration);
            bool currentConfigurationAccepted=configurationResults.GetValueOrDefault(doc.CurrentProcedureId)?.ContainsKey(configNode)==true;
            var report=new{observeUnfixed=observe,identitiesReused,cleanBefore,oldRunReturn=pending.Result,isolated,staleSession,staleHistory,staleConfiguration,currentConfigurationAccepted,
                scope="Actual WPF RunCore awaits a controlled managed module then same-file Load reuses all IDs. No original SDK, devices, other processes or desktop acceptance."};
            File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(report,new JsonSerializerOptions{WriteIndented=true}));
            window.Close();app.Shutdown();
            if(!identitiesReused||!cleanBefore)throw new Exception("Fixture did not establish reused identity and clean replacement.");
            if(observe?isolated:!isolated)throw new Exception(observe?"Expected original unguarded stale commit was not observed.":"Old run polluted replacement solution.");
            if(observe?!staleConfiguration:staleConfiguration)throw new Exception(observe?"Expected closed configuration stale result was not observed.":"Closed configuration committed a late result.");
            if(!currentConfigurationAccepted)throw new Exception("Current configuration completion was incorrectly rejected.");
            Console.WriteLine((observe?"OBSERVED UNFIXED RACE":"PASS owner isolation")+"; audit="+output);return 0;
        }
        catch(Exception error){Console.Error.WriteLine(error);return 1;}
    }
    static void PumpUntil(Task task)
    {
        var elapsed=Stopwatch.StartNew();
        while(!task.IsCompleted&&elapsed.Elapsed<TimeSpan.FromSeconds(20))
        {var frame=new DispatcherFrame();Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,new Action(()=>frame.Continue=false));Dispatcher.PushFrame(frame);Thread.Sleep(1);}
        if(!task.IsCompleted)throw new TimeoutException("Controlled fixture did not settle within 20 seconds.");task.GetAwaiter().GetResult();
    }
}

sealed class PendingModule(ModuleDescriptor descriptor,TaskCompletionSource entered,TaskCompletionSource release):IModule
{
    public ModuleDescriptor Descriptor=>descriptor;
    public async ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token)
    {entered.TrySetResult();await release.Task;return new(ModuleStatus.Success,new Dictionary<string,PortValue>());}
}
