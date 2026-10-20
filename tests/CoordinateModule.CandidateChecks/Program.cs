using System.Text.Json;
using VisionCore;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;
static class Program{
 static int checks;static void Check(bool value,string message){if(!value)throw new Exception(message);Console.WriteLine($"PASS {++checks}: {message}");}
 static Dictionary<string,JsonElement> Values(CoordinateModulePlugin p,int mode=0,int way=0){var v=p.Descriptor.Parameters.ToDictionary(x=>x.Name,x=>x.DefaultValue!.Value.Clone());v["CordinateCreate"]=JsonSerializer.SerializeToElement(mode);v["InputWay"]=JsonSerializer.SerializeToElement(way);return v;}
 static Dictionary<string,PortValue> Inputs()=>new(){["InputImage"]=PortValue.Image(new GrayImage(64,48,new byte[64*48]))};
 static ModuleResult Run(CoordinateModulePlugin p,ModuleExecutionScope scope,Dictionary<string,PortValue> inputs,int mode=0,int way=0,IReadOnlyList<RoiDocument>? rois=null,Func<string,int,ModuleNumericInputReadResult?>? read=null)=>p.CreateRuntime().ExecuteAsync(new("coordinate",Environment.CurrentDirectory,inputs,Values(p,mode,way),rois){Scope=scope,ReadNumericInput=read},default).GetAwaiter().GetResult();
 [STAThread]static int Main(){try{
 var p=new CoordinateModulePlugin();using var scope=new ModuleExecutionScope();
 Check(p.PointInputAliases.Single()==new ToolPointInputAlias("Src Input Point","SrcPointX","SrcPointY"),"source POINT maps actual same X/Y leaves");
 Check(p.Descriptor.Inputs.Where(x=>x.Name is "SrcPointX" or "SrcPointY" or "SrcAngle" or "CalibMatrix").All(x=>x.Kind==DataKind.NumberArray&&x.SupportsDeferredNumericRead),"four source float selectors permit arrays with deferred reads");
 var result=Run(p,scope,Inputs());Check(result.Status==ModuleStatus.Failed&&result.ErrorCode=="NativeUninitializedMatrix"&&!result.Outputs.ContainsKey("CorCalibMatrix"),"first missing ROI does not publish invented zero matrix");
 result=Run(p,scope,Inputs(),rois:[new("roi",RoiKind.Coordinate,2,3)]);Check(result.Status==ModuleStatus.Success&&result.Outputs["ModuStatus"].As<long>()==1,"draw valid ROI succeeds");
 Check(result.Outputs["CorCalibMatrix"].As<IReadOnlyList<double>>().SequenceEqual(new double[]{1,0,-2,0,1,-3,0,0,1}),"literal original draw transform direction");
 Check(result.Outputs["CoordinateAxisYY"].As<double>()==50,"source YY=m00 output");
 result=Run(p,scope,Inputs(),rois:[new("quarter-turn",RoiKind.Coordinate,2,3,AngleDegrees:90)]);var quarter=result.Outputs["CorCalibMatrix"].As<IReadOnlyList<double>>();Check(Math.Abs(quarter[0])<1e-6&&quarter[1]==1&&quarter[2]==-3&&quarter[3]==-1&&quarter[5]==2,"ROI angle is degrees and source clockwise matrix order is preserved");
 result=Run(p,scope,Inputs());Check(result.Status==ModuleStatus.Success&&result.Outputs["CoordinateOriPointX"].As<double>()==2,"missing ROI later follows timing-success and retains pose");
 var calls=new List<string>();result=Run(p,scope,Inputs(),1,1,read:(n,k)=>{calls.Add(n);return n=="SrcPointY"?new(0xe0000314,0,999):new(0,1,8);});
 Check(result.Status==ModuleStatus.Success&&result.Outputs["ModuStatus"].As<long>()==0&&calls.SequenceEqual(new[]{"SrcPointX","SrcPointY"}),"outer ignores inner error while source float read stops at Y");
 Check(result.Outputs["CoordinateOriPointX"].As<double>()==2,"float failure does not partially replace pose");
 result=Run(p,scope,Inputs(),1,0,read:(_,_)=>new(0,0,999));Check(result.Outputs["CoordinateOriPointX"].As<double>()==0,"Count0 successful no-write leaves initial local zero");
 var matrix=Inputs();matrix["CalibMatrix"]=PortValue.Numbers([1,0,-8,0,1,-9,0,0,1]);calls.Clear();
 result=Run(p,scope,matrix,1,2,read:(n,k)=>{calls.Add(n+":"+k);return null;});
 Check(calls.SequenceEqual(new[]{"CalibMatrix:0","CalibMatrix:0"}),"source repeats GetFloat0 then immutable batch adapter reads fixed frame");
 Check(result.Outputs["CoordinateOriPointX"].As<double>()==-8&&result.Outputs["CoordinateOriPointY"].As<double>()==-9,"matrix SDK inverse decomposition then negate");
 matrix["CalibMatrix"]=PortValue.Numbers([2]);result=Run(p,scope,matrix,1,2);
 Check(result.Outputs["CorCalibMatrix"].As<IReadOnlyList<double>>()[2]==-8,"known prefix leaves tail unchanged");
 matrix["CalibMatrix"]=PortValue.Numbers([]);result=Run(p,scope,matrix,1,2);Check(result.Outputs["ModuStatus"].As<long>()==0&&result.Outputs["CorCalibMatrix"].As<IReadOnlyList<double>>()[0]==2,"empty matrix Count rejects with retained outputs");
 var missing=Run(p,scope,new());Check(missing.Status==ModuleStatus.Failed&&missing.NativeErrorCode==0xe0000510,"image prerequisite is not bypassed by subscribe mode");
 var frame=p.LastDisplay(new("coordinate",CoordinateModulePlugin.Id,"1.0.0","Coordinate",Values(p)),Environment.CurrentDirectory,scope);Check(frame is not null,"display snapshot exists after execution");
 Check(CoordinateModulePlugin.DisplayObjects.Single(x=>x.Name=="Coordinate System").Type=="coordinate","exact original display resource loaded");
 result=Run(p,scope,Inputs(),1,1,read:(n,k)=>new(0,1,n=="SrcPointX"?double.MaxValue:0));Check(result.Status==ModuleStatus.Failed&&result.ErrorCode=="NativeNonFiniteResult"&&!result.Outputs.ContainsKey("CoordinateOriPointX")&&!result.Outputs.ContainsKey("CorCalibMatrix"),"double-to-float overflow is explicit safety failure without fake zero result");
 result=Run(p,scope,Inputs(),1,1,read:(n,k)=>new(0,1,n=="SrcAngle"?double.NaN:1));Check(result.Status==ModuleStatus.Failed&&result.ErrorCode=="NativeNonFiniteResult"&&!result.Outputs.ContainsKey("CoordinateAxisXX"),"scope NaN remains stored but cannot publish invalid finite-contract axis");
 result=Run(p,scope,Inputs(),rois:[new("recovery",RoiKind.Coordinate,2,3)]);Check(result.Status==ModuleStatus.Success&&result.Outputs.ContainsKey("CorCalibMatrix"),"valid draw fully recovers after non-finite state");
 matrix["CalibMatrix"]=PortValue.Numbers(new double[9]);result=Run(p,scope,matrix,1,2);Check(result.Status==ModuleStatus.Success&&result.Outputs["ModuStatus"].As<long>()==1&&result.Outputs["CoordinateOriPointX"].As<double>()==2&&result.Outputs["CorCalibMatrix"].As<IReadOnlyList<double>>().All(x=>x==0),"SDK all-zero exception timing tail sets status1 and retains old pose with newly copied raw matrix");
 matrix["CalibMatrix"]=PortValue.Numbers(Enumerable.Repeat(1d,10));result=Run(p,scope,matrix,1,2);Check(result.Status==ModuleStatus.Success&&result.Outputs["ModuStatus"].As<long>()==0&&result.Outputs["CorCalibMatrix"].As<IReadOnlyList<double>>().Count==9,"Count10 rejects inner read and outer retains previous nine values");
 Check(p.DisplayDefinition.ResultColumns.SequenceEqual(new[]{"ModuRunTime","ModuStatus","CorCalibMatrix"}),"source-visible three results do not expose hidden six display leaves as list columns");
 using(var injected=new ModuleExecutionScope()){var reader=new BatchReader();injected.RegisterService<IToolNumericInputReader>(reader);matrix["CalibMatrix"]=PortValue.Numbers([1,0,-8,0,1,-9,0,0,1]);result=Run(p,injected,matrix,1,2);Check(result.Status==ModuleStatus.Failed&&result.ErrorCode=="UnsupportedDynamicMatrixBatch"&&reader.Calls==2&&!result.Outputs.ContainsKey("CorCalibMatrix"),"scope source cannot mix a different materialized payload into unrepresented native batch");}
 foreach(int tag in new[]{0x01000000,0x01080001,0x02180014,0x02180021}){
  int channels=tag is 0x02180014 or 0x02180021?3:1;byte[] bytes=Enumerable.Range(0,4*channels).Select(i=>(byte)(i+1)).ToArray();var raw=new RawImage(2,2,(ImagePixelFormat)tag,bytes);var rawInput=new Dictionary<string,PortValue>{["InputImage"]=PortValue.Image(raw)};
  result=Run(p,scope,rawInput,rois:[new("raw",RoiKind.Coordinate,1,1)]);Check(result.Status==ModuleStatus.Success,"source image format accepted "+tag.ToString("x"));
  var preview=ImagePreview.Packed(raw);Check(preview is not null&&preview.Width==2&&preview.Height==2,"source raw format preview supported "+tag.ToString("x"));
  if(channels==1)Check(preview!.Pixels.SequenceEqual(bytes),"mono preview preserves exact bytes "+tag.ToString("x"));
  if(tag==0x02180014)Check(preview!.Pixels.SequenceEqual(bytes),"packed RGB preview preserves exact source order");
  if(tag==0x02180021)Check(preview!.Pixels.SequenceEqual(new byte[]{1,5,9,2,6,10,3,7,11,4,8,12}),"planar RGB preview uses exact three-plane order");
  Check(raw.Pixels.SequenceEqual(bytes),"preview does not mutate raw cached payload "+tag.ToString("x"));
  rawInput["InputImage"]=PortValue.Image(new RawImage(2,2,(ImagePixelFormat)tag,bytes[..^1]));result=Run(p,scope,rawInput,rois:[new("raw",RoiKind.Coordinate,1,1)]);Check(result.NativeErrorCode==0xe0000314,"known format exact payload length gate "+tag.ToString("x"));
 }
 result=Run(p,scope,new(){["InputImage"]=PortValue.Image(new RawImage(2,2,(ImagePixelFormat)123,new byte[4]))},rois:[new("raw",RoiKind.Coordinate,1,1)]);Check(result.NativeErrorCode==0xe0000510,"unknown format leaves native data pointer empty and outer rejects image");
 var pointInput=Inputs();pointInput["Src Input Point"]=PortValue.Point(new Point2(9,11));pointInput["SrcAngle"]=PortValue.Numbers([0]);result=Run(p,scope,pointInput,1,0);Check(result.Outputs["CoordinateOriPointX"].As<double>()==9&&result.Outputs["CoordinateOriPointY"].As<double>()==11,"independent direct scalar POINT context is projected only when leaf sources are absent");
 var callsBefore=new List<string>();result=Run(p,scope,pointInput,1,0,read:(n,k)=>{callsBefore.Add(n);return new(0xe000031b,0,999);});Check(result.Outputs["ModuStatus"].As<long>()==0&&result.Outputs["CoordinateOriPointX"].As<double>()==9&&callsBefore.SequenceEqual(new[]{"SrcPointX"}),"explicit deferred failure cannot be replaced by materialized parent POINT");
 using(var priority=new ModuleExecutionScope()){priority.RegisterService<IToolNumericInputReader>(new PointReader());result=Run(p,priority,pointInput,1,0,read:(_,_)=>throw new Exception("Scope must win"));Check(result.Outputs["CoordinateOriPointX"].As<double>()==42&&result.Outputs["CoordinateOriPointY"].As<double>()==43,"scope primitive source has priority over both callback and parent POINT");}
 matrix["CalibMatrix"]=PortValue.Numbers([1,double.MaxValue,0,0,1,0,0,0,1]);result=Run(p,scope,matrix,1,2);Check(result.ErrorCode=="NativeNonFiniteResult"&&!result.Outputs.ContainsKey("CoordinateAxisXY")&&result.Outputs.ContainsKey("CoordinateAxisXX"),"single invalid axis component is omitted without replacing the other valid components");
 frame=p.LastDisplay(new("coordinate",CoordinateModulePlugin.Id,"1.0.0","Coordinate",Values(p)),Environment.CurrentDirectory,scope);Check(frame is not null&&frame.Graphics.Count==0,"missing coordinate component cannot be converted into a false zero-vector graphic");
 GraphChecks.Run(Check);
 Console.WriteLine($"PASS {checks} Coordinate candidate checks; not a production-host or native-parity claim");return 0;
 }catch(Exception e){Console.Error.WriteLine(e);return 1;}}
 sealed class BatchReader:IToolNumericInputReader{public int Calls;public ToolNumericInputReadResult Read(ModuleExecutionContext context,string name,int index){Calls++;return new(0,9,99);}}
 sealed class PointReader:IToolNumericInputReader{public ToolNumericInputReadResult Read(ModuleExecutionContext context,string name,int index)=>new(0,1,name=="SrcPointX"?42:name=="SrcPointY"?43:0);}
}
