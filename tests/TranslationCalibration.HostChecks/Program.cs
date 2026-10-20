using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;
using W=System.Windows.Controls;
using VisionFramework;
using VisionEditor;
using VisionEditor.Core;
using VisionPluginContracts;
using VisionPlugins.Calibration;
static class Program{
 const BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;static int checks;
 static T Field<T>(object x,string n)=>(T)x.GetType().GetField(n,Private)!.GetValue(x)!;
 static object? Call(object x,string n,params object[] args)=>x.GetType().GetMethod(n,Private)!.Invoke(x,args);
 static void Check(bool b,string m){if(!b)throw new Exception(m);Console.WriteLine($"PASS {++checks}: {m}");}
 static void Pump()=>Application.Current.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);
 static T Wait<T>(Task<T> task){var frame=new DispatcherFrame();_=task.ContinueWith(_=>Application.Current.Dispatcher.BeginInvoke(()=>frame.Continue=false));if(!task.IsCompleted)Dispatcher.PushFrame(frame);return task.GetAwaiter().GetResult();}
 static void Wait(Task task){var frame=new DispatcherFrame();_=task.ContinueWith(_=>Application.Current.Dispatcher.BeginInvoke(()=>frame.Continue=false));if(!task.IsCompleted)Dispatcher.PushFrame(frame);task.GetAwaiter().GetResult();}
 static T? Find<T>(DependencyObject x,Func<T,bool> test)where T:DependencyObject{if(x is T item&&test(item))return item;for(int i=0;i<VisualTreeHelper.GetChildrenCount(x);i++)if(Find(VisualTreeHelper.GetChild(x,i),test)is{}found)return found;return null;}
 [STAThread]static int Main(string[] args){try{
 string output=Path.GetFullPath(args.Length==1?args[0]:"analysis/translation/host");Directory.CreateDirectory(output);var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(app.Dispatcher));
 var shell=new MainWindow{Left=-20000,Top=-20000,ShowInTaskbar=false,ShowActivated=false};shell.Show();try{
 var doc=Field<GraphEditorDocument>(shell,"doc");var plugin=(TranslationPlugin)Field<ToolPluginRegistry>(shell,"plugins").Find(TranslationPlugin.Id)!;Check(plugin is not null,"real MainWindow Translation registration");doc.NewSolution();string id=doc.AddNode(TranslationPlugin.Id,"1.0.0",100,0);NodeDocument Node()=>doc.CurrentProcedure.Nodes.Single(n=>n.Id==id);
 using var fixture=JsonDocument.Parse(File.ReadAllText("docs/plugins/n-point-solver-evidence.json"));var f=fixture.RootElement.GetProperty("cases").EnumerateArray().Single(x=>x.GetProperty("name").GetString()=="state-rotation-three");var points=f.GetProperty("points").EnumerateArray().ToArray();var rotations=f.GetProperty("rotation").EnumerateArray().ToArray();var values=Node().Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());values["CalibPointGet"]=JsonSerializer.SerializeToElement(1);values["CalibPointTotalNum"]=JsonSerializer.SerializeToElement(points.Length);values["RotPointTotalNum"]=JsonSerializer.SerializeToElement(rotations.Length);values["WeightFun"]=JsonSerializer.SerializeToElement(f.GetProperty("weight").GetInt32());values["HomoFreedom"]=JsonSerializer.SerializeToElement(f.GetProperty("freedom").GetInt32());
 string[] keys=["ImagePointX","ImagePointY","WorldPointX","WorldPointY","DeltaAngle"];for(int i=0;i<points.Length+rotations.Length;i++){var p=i<points.Length?points[i]:rotations[i-points.Length];for(int a=0;a<5;a++)values[keys[a]+i]=JsonSerializer.SerializeToElement(a<p.GetArrayLength()?p[a].GetDouble():0);}
 doc.SetParameters(id,values);shell.GetType().GetField("selected",Private)!.SetValue(shell,id);void Run(){Wait((Task)Call(shell,"RunProcedure",doc.CurrentProcedureId)!);Call(shell,"RefreshAll");Pump();}NodeRunRecord Result()=>Field<Dictionary<string,NodeRunRecord>>(shell,"results")[id];Run();Check(Result().Status==NodeRunStatus.Success&&Result().Outputs.ContainsKey("CalibData"),"real manual table invokes Translation solve");
 var presenter=Field<ToolDisplayImagePresenter>(shell,"toolDisplayImage");Check(presenter.Frame!.Graphics.Single(x=>x.Name=="Image Point").Points.Count==points.Length+rotations.Length,"original path shows acquired translation and rotation prefix");var axis=presenter.Frame.Graphics.Single(x=>x.Name=="RotAxisPoint").Points.Single();Check(axis.X==Result().Outputs["RotCenterImageX"].As<double>()&&axis.Y==Result().Outputs["RotCenterImageY"].As<double>(),"original axis maps actual solve center");Check(Find<System.Windows.Shapes.Path>(presenter,x=>Equals(x.Tag,"Image Point"))is not null,"actual presenter draws source path");
 Call(shell,"OpenToolConfiguration",id);var host=Field<ToolConfigurationWindowHost>(shell,"toolConfigurationWindows");var window=(ToolConfigurationModelWindow)host.Current!;Check(window.ConfigurationView.Tabs.Items.Count==2,"source basic/results two tabs");foreach(var tab in window.ConfigurationView.Tabs.Items){window.ConfigurationView.Tabs.SelectedItem=tab;Pump();Check(window.ConfigurationView.Diagnostics.Count==0,"all source tab templates resolve");}window.ConfigurationView.Tabs.SelectedIndex=0;Pump();Check(Find<W.Button>(window,x=>x.Name=="CalibrationEditPoints")is not null,"real common provider exposes Translation point table");
 var context=(ToolEditorContext)Call(shell,"ConnectToolEditorServices",new ToolEditorContext(Node(),doc.BaseDirectory,p=>doc.SetParameters(id,p),_=>throw new NotSupportedException()),id)!;var self=Wait(context.Run(values));Check(self.Status==NodeRunStatus.Success&&self.Outputs.ContainsKey("CalibData"),"actual configuration self-run reuses manual source table");Check(plugin!.LastDisplay(Node(),doc.BaseDirectory,context.Scope)!.Graphics.Count==2,"self-run retains source display objects in scope");
 var table=new TranslationTableWindow(window,values.ToDictionary(p=>p.Key,p=>p.Value.Clone()),()=>{},()=>{});Check(table.Width==520&&table.Height==399,"source table factory size520x399");table.Close();
 var session=Field<ToolConfigurationSession>(window,"session");Call(shell,"CloseToolConfiguration");Check(Field<bool>(session,"disposed"),"configuration close disposes session");string path=Path.Combine(output,"translation.vmx.json");doc.Save(path);doc.Load(path);Run();Check(Result().Status==NodeRunStatus.Success&&presenter.Frame!.Graphics.Count==2,"whole document reload restores table and display");
 var scope=Field<ModuleExecutionScope>(shell,"executionScope").ForProcedure(doc.CurrentProcedureId);scope.RemoveNode(TranslationPlugin.Id,id);Check(plugin.LastDisplay(Node(),doc.BaseDirectory,scope)is null,"RemoveNode drops display/cache");Run();Check(Result().Status==NodeRunStatus.Success,"removed state reconstructs from saved table");
 var registry=Field<ModuleRegistry>(shell,"modules");var sourceModule=new Source();registry.Register(sourceModule.Descriptor,()=>sourceModule);string sourceId=doc.AddNode(sourceModule.Descriptor.Id,"1",0,0);doc.ConnectExecution(sourceId,id);
 var capture=Node().Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());capture["CalibPointGet"]=JsonSerializer.SerializeToElement(0);capture["RotPointTotalNum"]=JsonSerializer.SerializeToElement(0);capture["ClearRevision"]=JsonSerializer.SerializeToElement(capture["ClearRevision"].GetInt32()+1);doc.SetParameters(id,capture);
 var actual=(ToolEditorContext)Call(shell,"ConnectToolEditorServices",new ToolEditorContext(Node(),doc.BaseDirectory,p=>doc.SetParameters(id,p),_=>throw new NotSupportedException()),id)!;
 var binding=new ConfigurationSubscriptionBinding(new ConfigurationOutputReference(doc.CurrentProcedureId,sourceId,"Points","POINT"),ConfigurationSubscriptionIndex.All);
 var bound=actual.ConfigurationSubscriptions!.Bind(new(ConfigurationSubscriptionDomain.Combination,"Image Point","POINT"),binding);Check(bound.Status==ConfigurationSubscriptionStatus.Available,"real MainWindow POINT array binds Translation child coordinates: "+bound.Status+" "+bound.Diagnostic);
 var cache=Field<Dictionary<string,Dictionary<string,NodeRunRecord>>>(shell,"configurationResults");cache[doc.CurrentProcedureId]=new(){{sourceId,new(sourceId,NodeRunStatus.Success,new Dictionary<string,PortValue>{{"Points",PortValue.PointArray([new VisionCore.Point2(10,11),new(20,21)])}},0)}};
 var acquired=Wait(actual.Run(capture));Check(acquired.Status==NodeRunStatus.Success&&acquired.Outputs["OutImagePointX"].As<IReadOnlyList<double>>().SequenceEqual(new[]{10d,20}),"actual cached self-run consumes POINT array without executing producer");
 var debug=Wait(new NodeDebugExecutor(registry).RunAsync(doc.CurrentProcedure,id,doc.BaseDirectory,capture,cache[doc.CurrentProcedureId].Values.ToArray()));Check(debug.Status==NodeRunStatus.Success,"actual debug consumes POINT array cache");
 File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(new{passed=true,checks,nativeExecution=false}));return 0;
 }finally{Call(shell,"CloseToolConfiguration");shell.Close();app.Shutdown();}
 }catch(Exception e){Console.Error.WriteLine(e);return 1;}}
 sealed class Source:IModule{
  public ModuleDescriptor Descriptor{get;}=new("translation-host-points","1","Points",[],[new("Points",DataKind.Point,false){Cardinality=ValueCardinality.Array}],[]);
  public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext c,CancellationToken t)=>throw new Exception("cached self-run must not execute producer");
 }
}
