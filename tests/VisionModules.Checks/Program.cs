using System.Text.Json;
using VisionCore;
using VisionFramework;
using VisionModules;
var registry=new ModuleRegistry(); BuiltinModules.RegisterAll(registry);
int count=0;
void Check(bool condition,string name){if(!condition)throw new Exception(name);count++;}
Dictionary<string,JsonElement> Params(params (string,object)[] args)=>args.ToDictionary(x=>x.Item1,x=>JsonSerializer.SerializeToElement(x.Item2));
async Task<ModuleResult> Run(string id,Dictionary<string,PortValue>? inputs=null,Dictionary<string,JsonElement>? parameters=null,CancellationToken token=default)=>
    await registry.Create(id,"1.0.0").ExecuteAsync(new("test",AppContext.BaseDirectory,inputs??[],parameters??[]),token);
Check(registry.Descriptors.Count==6&&registry.Descriptors.Any(d=>d.Id==WorkflowCalls.ModuleId),"five algorithms plus subprogram control module");
Check(!ReferenceEquals(registry.Create("vision.circle","1.0.0"),registry.Create("vision.circle","1.0.0")),"fresh factory");
var image=new GrayImage(3,2,new byte[]{0,127,128,255,0,255});
var threshold=await Run("vision.threshold",new(){{"Image",PortValue.Image(image)}},Params(("Threshold",128)));
Check(threshold.Status==ModuleStatus.Success&&threshold.Outputs["Image"].As<GrayImage>().Pixels.SequenceEqual(new byte[]{0,0,255,255,0,255}),"inclusive threshold");
var components=await Run("vision.components",new(){{"Image",threshold.Outputs["Image"]}});
Check(components.Outputs["Count"].Kind==DataKind.Integer&&components.Outputs["Count"].As<long>()==2,"components count type and connectivity");
Check((await Run("vision.threshold",new(){{"Image",PortValue.Image(image)}},Params(("Threshold",256)))).Status==ModuleStatus.Failed,"threshold range");
Check((await Run("vision.threshold",new(){{"Image",PortValue.Image(image)}})).Status==ModuleStatus.Failed,"missing required");
Check((await Run("vision.components",new(){{"Image",PortValue.Number(2)}})).Status==ModuleStatus.Failed,"strict port type");
var cp=Params(("CenterX",32d),("CenterY",32d),("InitialRadius",16d),("SearchHalfWidth",4d),("SampleCount",64),("GradientThreshold",15d),("Polarity","LightToDark"),("RadialStep",.5d),("GradientHalfSpan",1d),("MinimumEdges",6));
byte[] disk=new byte[65*65];
for(int y=0;y<65;y++)for(int x=0;x<65;x++)disk[y*65+x]=(x-32)*(x-32)+(y-32)*(y-32)<=256?(byte)255:(byte)0;
var found=await Run("vision.circle",new(){{"Image",PortValue.Image(new(65,65,disk))}},cp);
Check(found.Status==ModuleStatus.Success&&found.Outputs["Found"].As<bool>(),"circle found");
Check(found.Outputs["Circle"].Kind==DataKind.Circle&&found.Outputs["Radius"].Kind==DataKind.Number&&Math.Abs(found.Outputs["Radius"].As<double>()-16)<1,"circle typed geometry");
var noMatch=await Run("vision.circle",new(){{"Image",PortValue.Image(new(65,65,new byte[65*65]))}},cp);
Check(noMatch.Status==ModuleStatus.NoMatch&&!noMatch.Outputs["Found"].As<bool>()&&!noMatch.Outputs.ContainsKey("Circle")&&!noMatch.Outputs.ContainsKey("Radius"),"no fabricated circle");
cp["RadialStep"]=JsonSerializer.SerializeToElement(0);
Check((await Run("vision.circle",new(){{"Image",PortValue.Image(image)}},cp)).Status==ModuleStatus.Failed,"invalid circle fails not nomatch");
foreach(var op in new[]{"==","!=",">",">=","<","<="}) {
 var result=await Run("logic.number-compare",new(){{"Number",PortValue.Number(3)}},Params(("Operator",op),("Reference",3d)));
 Check(result.Outputs["Passed"].As<bool>()==(op is "==" or ">=" or "<="),"comparison "+op);
}
Check((await Run("logic.number-compare",new(){{"Number",PortValue.Number(3)}},Params(("Operator","oops"),("Reference",3d)))).Status==ModuleStatus.Failed,"invalid comparison");
using(var cancellation=new CancellationTokenSource()){
 cancellation.Cancel();
 try {await Run("vision.components",new(){{"Image",PortValue.Image(image)}},token:cancellation.Token);throw new Exception("not cancelled");}
 catch(OperationCanceledException){count++;}
}
string filename="module-check-"+Guid.NewGuid().ToString("N")+".bmp";
string path=Path.Combine(AppContext.BaseDirectory,filename);
try {
 using(var writer=new BinaryWriter(File.Create(path))){
  writer.Write((ushort)0x4d42);writer.Write(62);writer.Write(0);writer.Write(54);
  writer.Write(40);writer.Write(2);writer.Write(1);writer.Write((ushort)1);writer.Write((ushort)24);
  writer.Write(0);writer.Write(8);writer.Write(0);writer.Write(0);writer.Write(0);writer.Write(0);
  writer.Write(new byte[]{0,0,0,255,255,255,0,0});
 }
 var file=await Run("vision.image-file",parameters:Params(("Path",filename)));
 Check(file.Status==ModuleStatus.Success&&file.Outputs["Image"].As<GrayImage>().Pixels.SequenceEqual(new byte[]{0,255}),"real BMP relative path");
 var t=await Run("vision.threshold",new(){{"Image",file.Outputs["Image"]}},Params(("Threshold",255)));
 var c=await Run("vision.components",new(){{"Image",t.Outputs["Image"]}});
 Check(c.Outputs["Count"].As<long>()==1,"file threshold components pipeline");
 File.WriteAllText(path,"broken bmp");
 Check((await Run("vision.image-file",parameters:Params(("Path",filename)))).Status==ModuleStatus.Failed,"bad BMP fails");
} finally {File.Delete(path);}
Console.WriteLine($"PASS {count} module semantic checks");
