using System.IO;
using System.Collections;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using Application=System.Windows.Application;

static class Program
{
 const BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
 [STAThread]static int Main(string[] args)
 {
  try{
   if(args.Length is <1 or >2)throw new ArgumentException("Supply owned output parent [observe-unfixed].");
   bool observe=args.Length==2&&args[1]=="observe-unfixed";
   string output=Path.Combine(Path.GetFullPath(args[0]),"result-view-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(output);
   AppContext.SetSwitch("VisionEditor.DisableAutomaticCameraDiscovery",true);
   var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};
   SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
   var window=new MainWindow();
   object Field(string name)=>typeof(MainWindow).GetField(name,Private)!.GetValue(window)!;
   void Set(string name,object? value)=>typeof(MainWindow).GetField(name,Private)!.SetValue(window,value);
   object? Call(string name,params object?[] values)=>typeof(MainWindow).GetMethod(name,Private)!.Invoke(window,values);
   var checks=new List<string>();void Check(bool ok,string label){if(!ok)throw new Exception(label);checks.Add(label);Console.WriteLine("PASS "+label);}
   var doc=(GraphEditorDocument)Field("doc");var modules=(ModuleRegistry)Field("modules");var plugins=(ToolPluginRegistry)Field("plugins");
   var provider=new DisplayPlugin();plugins.Register(provider,modules);
   var plain=new ModuleDescriptor("test.display-plain","1","plain",[],[],[]);modules.Register(plain,()=>new NoRunModule(plain));
   doc.NewSolution();string a=doc.AddNode(provider.Descriptor.Id,"1",100,100),b=doc.AddNode(provider.Descriptor.Id,"1",250,100),c=doc.AddNode(plain.Id,"1",400,100);
   provider.Text[a]="source A";provider.Text[b]="source B error";
   var results=(Dictionary<string,NodeRunRecord>)Field("results");
   NodeRunRecord Record(string id,double time)=>new(id,NodeRunStatus.Success,new Dictionary<string,PortValue>{{"Value",PortValue.Text(id)}},time);
   void Populate(){results[a]=Record(a,11);results[b]=Record(b,22);}
   var image=(ToolDisplayImagePresenter)Field("toolDisplayImage");var record=(ToolDisplayRecordPresenter)Field("toolDisplayRecord");var list=(ToolDisplayResultListPresenter)Field("toolDisplayList");
   void Select(string? id){Set("selected",id);Call("RefreshRight");}
   Populate();Select(a);Check(image.Frame?.Text=="source A"&&record.Text=="source A","Actual host selects A frame and record");
   Select(b);Check(image.Frame?.Text=="source B error"&&record.Text=="source B error","Selecting B replaces A text and record");
   Check(list.Columns.Count==1&&list.Columns[0].Header?.ToString()=="B","B replaces result column declaration");
   Check(((System.Windows.Controls.TextBlock)window.FindName("ModuleTimeText")).Text=="22.00","Selected module timing belongs to B");
   Select(c);Check(image.Frame is null&&record.Text==""&&list.Columns.Count==0&&list.ItemsSource is null,"No-provider selection clears every plugin presenter");
   Select(null);Check(image.Frame is null&&((IEnumerable)((System.Windows.Controls.DataGrid)window.FindName("ModuleResultsGrid")).ItemsSource).Cast<object>().Count()==0,"No selection clears output grid and presenter");
   Select(a);provider.ThrowNode=b;bool providerFault=false;
   try{Select(b);}catch(TargetInvocationException e)when(e.InnerException is InvalidOperationException){providerFault=true;}
   if(observe)Check(providerFault&&image.Frame?.Text=="source A"&&record.Text=="source A","Unfixed provider failure demonstrably leaves prior A presentation visible");
   else Check(image.Frame is null&&record.Text==""&&list.Columns.Count==0,"Provider failure never leaves prior A presentation visible");provider.ThrowNode=null;
   Populate();Select(a);Check(image.Frame?.Text=="source A","New valid selection recovers after provider failure");
   string saved=Path.Combine(output,"same-id.vmx.json");doc.Save(saved);string solution=doc.SolutionId,procedure=doc.CurrentProcedureId;
   Call("RecordFrameworkRunHistory",procedure,new ExecutionSessionResult(SessionStatus.Success,new[]{Record(a,11),Record(b,22)}));
   Check(((FrameworkRunHistory)Field("frameworkRunHistory")).Entries.Count==1,"Fixture establishes nonempty execution record count before replacement");
   // Queue old-scope refresh, then replace before pumping Dispatcher.
   var oldScope=(ModuleExecutionScope)Field("executionScope");oldScope.ForProcedure(procedure).RequestRefresh(a);
   doc.Load(saved);int beforePump=provider.Calls;Pump();
   Check(doc.SolutionId==solution&&doc.CurrentProcedureId==procedure&&doc.CurrentProcedure.Nodes.Any(n=>n.Id==a),"Reload fixture reuses source document identities");
   Check(provider.Calls==beforePump,"Queued old-scope refresh does not ask current display provider");
   bool HasHandlers(ModuleExecutionScope scope,string name)=>typeof(ModuleExecutionScope).GetField(name,Private)!.GetValue(scope) is Delegate;
   bool oldHandlers=HasHandlers(oldScope,"ProcedureRefreshRequested")||HasHandlers(oldScope,"ProcedureRunRequested");
   Check(observe?oldHandlers:!oldHandlers,observe?"Unfixed replaced scope retains physical main event subscriptions":"Replacement physically removes both old main event subscriptions");
   Check(results.Count==0&&image.Frame is null&&record.Text==""&&list.Columns.Count==0,"Same-ID reload clears result cache and all displayed frames");
   Check(((FrameworkRunHistory)Field("frameworkRunHistory")).Entries.Count==0,"Replacement leaves no old execution record count");
   Check(((System.Windows.Controls.TextBlock)window.FindName("ModuleTimeText")).Text is "—" or "0.00","Replacement clears selected module elapsed cache");
   if(!observe){
    // LastDisplay is synchronous external code and may pump UI callbacks. The
    // fixture captures its old value before invoking the controlled reentry.
    Populate();provider.Text[a]="before reentrant Load";
    provider.BeforeReturn=()=>{provider.BeforeReturn=null;doc.Load(saved);};
    Select(a);
    Check(results.Count==0&&image.Frame is null&&record.Text==""&&list.Columns.Count==0,"Synchronous provider Load cannot publish its old return into replacement view");
    Populate();provider.Text[a]="old same source";
    provider.BeforeReturn=()=>{provider.BeforeReturn=null;provider.Text[a]="new same source";Select(a);};
    Select(a);
    Check(image.Frame?.Text=="new same source"&&record.Text=="new same source","Nested same-owner same-node refresh wins over older provider return");
    provider.Text[a]="source A";
   }
   int disposed=0;var currentScope=(ModuleExecutionScope)Field("executionScope");currentScope.ForProcedure(procedure).State("test.display-owner",a,()=>new OwnedState(()=>disposed++));
   doc.DeleteNode(a);Check(disposed==1,"Actual document node removal disposes old source state once");
   currentScope.ForProcedure(procedure).RequestRefresh(a);Pump();Check(disposed==1,"Post-removal refresh cannot resurrect old owned source state");
   window.Close();
   bool closeHandlers=HasHandlers(currentScope,"ProcedureRefreshRequested")||HasHandlers(currentScope,"ProcedureRunRequested");
   Check(observe?closeHandlers:!closeHandlers,observe?"Unfixed closed scope retains physical main event subscriptions":"Actual host close physically removes current main event subscriptions");
   app.Shutdown();
   File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(new{observeUnfixed=observe,passed=checks.Count,checks,providerFaultPropagated=providerFault,scope="actual main pull-display consumer and owned scope/Dispatcher; controlled synthetic display provider, no SDK/device/vendor UI or interactive desktop parity"},new JsonSerializerOptions{WriteIndented=true}));
   Console.WriteLine($"PASS {checks.Count} result lifecycle checks; audit={output}");return 0;
  }catch(Exception error){Console.Error.WriteLine(error);return 1;}
 }
 static void Pump(){for(int i=0;i<5;i++){var frame=new DispatcherFrame();Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,new Action(()=>frame.Continue=false));Dispatcher.PushFrame(frame);}}
}
sealed class DisplayPlugin:IVisionToolPlugin,IToolDisplayProvider
{
 public Dictionary<string,string> Text{get;}=new();public string? ThrowNode;public int Calls;public Action? BeforeReturn;
 public ToolManifest Manifest{get;}=new("test.display-lifecycle","1","display","test","");
 public ModuleDescriptor Descriptor{get;}=new("test.display-lifecycle","1","display",[],[new("Value",DataKind.Text)],[]);
 public ToolDisplayDefinition DisplayDefinition{get;}=new("",[],[]);
 public IModule CreateRuntime()=>new NoRunModule(Descriptor);
 public System.Windows.Forms.Form CreateEditor(ToolEditorContext context)=>throw new NotSupportedException("Fixture has no configuration UI.");
 public void Reset(string nodeId){}
 public ToolDisplayFrame? LastDisplay(NodeDocument node,string baseDirectory,ModuleExecutionScope? scope){Calls++;if(node.Id==ThrowNode)throw new InvalidOperationException("owned-display-failure");string value=Text[node.Id];BeforeReturn?.Invoke();return new(null,value,"#00ff00",12,1,0,0,value,[new(value=="source A"?"A":"B",value)]){TextVisible=true,RecordVisible=true,ResultListVisible=true};}
}
sealed class NoRunModule(ModuleDescriptor descriptor):IModule
{
 public ModuleDescriptor Descriptor=>descriptor;
 public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token)=>throw new InvalidOperationException("Display fixture never executes modules.");
}
sealed class OwnedState(Action release):IDisposable{public void Dispose()=>release();}
