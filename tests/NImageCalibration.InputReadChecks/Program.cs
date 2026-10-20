using System.Text.Json;
using VisionCore;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;
static class Program {
 static int checks;static void Check(bool condition,string message){if(!condition)throw new Exception(message);checks++;}
 static Dictionary<string,JsonElement> Parameters(){var p=NImageParameters.Defaults();p["CalibPointTotalNum"]=JsonSerializer.SerializeToElement(4);p["RotPointTotalNum"]=JsonSerializer.SerializeToElement(0);return p;}
 static ToolEditorContext Editor(NImagePlugin plugin,Dictionary<string,JsonElement> p,ModuleExecutionScope scope)=>new(new("image",NImagePlugin.Id,"1.0.0","image",p),Environment.CurrentDirectory,_=>{},_=>throw new Exception()){Scope=scope};
 static Dictionary<string,PortValue> Inputs()=>new(){["InputImage"]=PortValue.Image(new GrayImage(64,64,new byte[4096])),["MovePhysicalPointX"]=PortValue.Numbers([1.25,91]),["MovePhysicalPointY"]=PortValue.Numbers([2.5,92]),["RotateAngle"]=PortValue.Numbers([3.75,93])};
 static int Main(){try{Materialized();foreach(string failed in new[]{"PicPointX","PicPointY","PhysicalPointX","PhysicalPointY"})Phase(failed);Console.WriteLine($"PASS {checks} NImage source input phase checks");return 0;}catch(Exception error){Console.Error.WriteLine(error);return 1;}}
 static void Materialized(){var plugin=new NImagePlugin();var p=Parameters();using var scope=new ModuleExecutionScope();var result=plugin.CreateRuntime().ExecuteAsync(new("image",Environment.CurrentDirectory,Inputs(),p){Scope=scope},default).Result;Check(result.Status==ModuleStatus.Success,"array motion appends pending frame");var f=plugin.Inspect(Editor(plugin,p,scope)).Frames[0];Check(f.PhysicalX==1.25&&f.PhysicalY==2.5&&f.Angle==3.75,"all three arrays use first element");var input=Inputs();input["RotateAngle"]=PortValue.Numbers([]);plugin.Clear(Editor(plugin,p,scope));result=plugin.CreateRuntime().ExecuteAsync(new("image",Environment.CurrentDirectory,input,p){Scope=scope},default).Result;f=plugin.Inspect(Editor(plugin,p,scope)).Frames[0];var auto=NImageSequence.Motion(0,NImageParameters.Sequence(p));Check(f.PhysicalX==auto.X&&f.PhysicalY==auto.Y&&f.Angle==auto.Angle,"one zero Count replaces all three motion fields");Check(plugin.Descriptor.Inputs.Where(d=>new[]{"MovePhysicalPointX","MovePhysicalPointY","RotateAngle","PicPointX","PicPointY","PhysicalPointX","PhysicalPointY"}.Contains(d.Name)).All(d=>d.SupportsDeferredNumericRead),"all seven source fields opt in to raw reads");}
 static void Phase(string failed){var plugin=new NImagePlugin();var p=Parameters();p["HomoFixEnable"]=JsonSerializer.SerializeToElement(true);using var scope=new ModuleExecutionScope();var reader=new Reader(failed);scope.RegisterService<IToolNumericInputReader>(reader);var runtime=plugin.CreateRuntime();for(int frame=0;frame<4;frame++){
  reader.Calls.Clear();var result=runtime.ExecuteAsync(new("image",Environment.CurrentDirectory,Inputs(),p){Scope=scope,ReadNumericInput=(_,_)=>throw new Exception("scope must take priority")},default).Result;
  var expected=new List<string>{"MovePhysicalPointX","MovePhysicalPointY","RotateAngle"};if(frame==3){foreach(var name in new[]{"PicPointX","PicPointY","PhysicalPointX","PhysicalPointY"}){expected.Add(name);if(name==failed)break;}}
  Check(reader.Calls.SequenceEqual(expected),"motion always three reads; correction only full batch and short circuits "+failed+"/"+frame);
  Check(plugin.Inspect(Editor(plugin,p,scope)).Current==frame+1,"append precedes correction error");
  Check(result.Status==(frame==3?ModuleStatus.Failed:ModuleStatus.Success),"missing correction cannot fail pending frames");
  if(frame==3){Check(result.NativeErrorCode==0xe0000014,"raw correction read error is propagated");Check(result.Outputs["CurNum"].As<long>()==4&&result.Outputs["ModuStatus"].As<long>()==1,"failed correction still publishes native pending/count frame");}
 }}
 sealed class Reader(string failed):IToolNumericInputReader{public List<string> Calls=[];public ToolNumericInputReadResult Read(ModuleExecutionContext context,string name,int index){Check(index==0,"original input read index0");Calls.Add(name);return name==failed?new(0xe0000014,0,0):new(0,2,name=="RotateAngle"?0:1);}}
}
