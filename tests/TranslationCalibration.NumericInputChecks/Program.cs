using System.Text.Json;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;
static class Program{
 static int checks;static void Check(bool c,string text){if(!c)throw new Exception(text);checks++;}
 static Dictionary<string,JsonElement> Values(TranslationPlugin p){var v=p.Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());v["RotPointTotalNum"]=JsonSerializer.SerializeToElement(0);return v;}
 static Dictionary<string,PortValue> Inputs()=>new(){["ImagePointX"]=PortValue.Numbers([1,2]),["ImagePointY"]=PortValue.Numbers([3,4])};
 static ModuleResult Run(TranslationPlugin p,Dictionary<string,PortValue> i,Dictionary<string,JsonElement>? v=null,Func<string,int,ModuleNumericInputReadResult?>? read=null,ModuleExecutionScope? scope=null)=>p.CreateRuntime().ExecuteAsync(new("numeric",Environment.CurrentDirectory,i,v??Values(p)){ReadNumericInput=read,Scope=scope},default).GetAwaiter().GetResult();
 [STAThread]static int Main(){try{
 var empty=Inputs();empty["ImagePointX"]=empty["ImagePointY"]=PortValue.Numbers([]);var result=Run(new(),empty);Check(result.NativeErrorCode==0xe0000314&&result.Outputs.Count==0,"materialized empty float payload follows complete Frame E314");
 var wrong=Inputs();wrong["ImagePointX"]=wrong["ImagePointY"]=PortValue.Integer(1);Check(Run(new(),wrong).NativeErrorCode==0xe0000314,"direct non-float kind cannot impersonate successful empty float payload");
 result=Run(new(),new(),read:(n,k)=>n is "ImagePointX" or "ImagePointY"?new(0,0,0):new(0xe000031c,0,0));Check(result.Status==ModuleStatus.Success&&result.Outputs["CalibCurIndex"].As<long>()==0,"explicit synthetic success Count0 remains success");
 foreach(uint code in new uint[]{0xe000031b,0xe000031c,0xe0000314,0xe0000012}){result=Run(new(),new(),read:(n,k)=>new(code,9,17));Check(result.NativeErrorCode==code,"required read source error propagates with normalized Count0 "+code.ToString("x"));}
 result=Run(new(),new(),read:(n,k)=>n=="ImagePointY"?new(0,1,8):new(0xe0000314,0,0));Check(result.NativeErrorCode==0xe0000001,"raw count mismatch takes precedence before read error");
 var calls=new List<string>();result=Run(new(),Inputs(),read:(n,k)=>{calls.Add(n);return null;});Check(result.Status==ModuleStatus.Success&&calls.Take(4).SequenceEqual(new[]{"ImagePointX","ImagePointY","PhysicalPointX","PhysicalPointY"}),"native Count discovery order and rotation0 skips angle Count phase");
 var large=Inputs();large["ImagePointX"]=PortValue.Numbers(Enumerable.Range(0,33).Select(i=>(double)i));large["ImagePointY"]=PortValue.Numbers(Enumerable.Range(0,32).Select(i=>(double)i));result=Run(new(),large);Check(result.NativeErrorCode==0xe0000001,"compare original counts before32 cap");
 var p=new TranslationPlugin();var manual=Values(p);manual["CalibPointGet"]=JsonSerializer.SerializeToElement(1);manual["CalibPointTotalNum"]=JsonSerializer.SerializeToElement(4);for(int i=0;i<4;i++){manual["ImagePointX"+i]=JsonSerializer.SerializeToElement((double)(i%2));manual["ImagePointY"+i]=JsonSerializer.SerializeToElement((double)(i/2));manual["WorldPointX"+i]=JsonSerializer.SerializeToElement((double)(i%2+1));manual["WorldPointY"+i]=JsonSerializer.SerializeToElement((double)(i/2+2));}
 using(var scope=new ModuleExecutionScope()){result=Run(p,new(),manual,(_,_)=>throw new Exception("manual must not read sampling getters"),scope);Check(result.Status==ModuleStatus.Success,"manual solves without any numeric input read");}
 var teacher=new TranslationPlugin();var teach=Values(teacher);teach["TeachEnable"]=JsonSerializer.SerializeToElement(true);var ti=Inputs();ti["Trigger"]=PortValue.Text("go");ti["TeachFlag"]=PortValue.Text("go");calls.Clear();result=Run(teacher,ti,teach,(n,k)=>{calls.Add(n);return new(0xe000031b,3,7);});Check(result.NativeErrorCode==0xe000031b&&calls.SequenceEqual(new[]{"PhysicalPointX"}),"teaching first physical error stops later reads and propagates");
 using(var scope=new ModuleExecutionScope()){var reader=new Reader();scope.RegisterService<IToolNumericInputReader>(reader);result=Run(new(),new(),read:(_,_)=>throw new Exception("scope wins"),scope:scope);Check(result.Status==ModuleStatus.Success&&reader.Calls>0,"scope numeric source priority over callback");}
 Graph();Console.WriteLine($"PASS {checks} Translation native Count/error/manual/scope input checks");return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
 static void Graph(){
  var plugin=new TranslationPlugin();var registry=new ModuleRegistry();registry.Register(plugin.Descriptor,plugin.CreateRuntime);var producer=new Source();registry.Register(producer.Descriptor,()=>producer);
  foreach(string? index in new string?[]{null,"1","9","-1"}){
   var source=new WorkflowValueReference(WorkflowSourceKind.NodeOutput,"source","Value"){ArrayIndex=index is null?null:new(index)};
   var flow=new ProcedureDocument("translation","Translation",[new("source",producer.Descriptor.Id,"1","source",[]),new("target",TranslationPlugin.Id,"1.0.0","Translation",Values(plugin))],[]){Configuration=new(){Subscriptions=[new("target","ImagePointX",false,source),new("target","ImagePointY",false,source)]}};
   using var scope=new ModuleExecutionScope();var run=new GraphExecutor(registry).RunAsync(flow,Environment.CurrentDirectory,scope:scope).Result.Nodes.Single(n=>n.NodeId=="target");
   Check(index is null?run.Status==NodeRunStatus.Success&&run.Outputs["CalibCurIndex"].As<long>()==2:index=="1"?run.Status==NodeRunStatus.Success&&run.Outputs["CalibCurIndex"].As<long>()==1:run.NativeErrorCode==0xe0000014,"real graph All/selector native count/error "+index);
  }
  Source.Empty=true;var emptyFlow=new ProcedureDocument("empty","empty",[new("source",producer.Descriptor.Id,"1","source",[]),new("target",TranslationPlugin.Id,"1.0.0","Translation",Values(plugin))],[new("source","Value","target","ImagePointX"),new("source","Value","target","ImagePointY")]);
  using(var scope=new ModuleExecutionScope()){var run=new GraphExecutor(registry).RunAsync(emptyFlow,Environment.CurrentDirectory,scope:scope).Result.Nodes.Single(n=>n.NodeId=="target");Check(run.NativeErrorCode==0xe0000314,"real graph empty bound array differs from legacy direct synthetic success fixture");}Source.Empty=false;
 }
 sealed class Source:IModule{
  public static bool Empty;public ModuleDescriptor Descriptor{get;}=new("translation-numeric-source","1","source",[],[new("Value",DataKind.NumberArray,false)],[]);
  public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext c,CancellationToken t)=>ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"Value",PortValue.Numbers(Empty?[]:[10,20])}}));
 }
 sealed class Reader:IToolNumericInputReader{public int Calls;public ToolNumericInputReadResult Read(ModuleExecutionContext c,string name,int index){Calls++;return name is "ImagePointX" or "ImagePointY"?new(0,0,0):new(0xe000031c,0,0);}}
}
