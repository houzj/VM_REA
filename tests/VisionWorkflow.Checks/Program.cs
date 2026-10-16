using VisionCore;
using VisionWorkflow;
using System.Text.Json.Nodes;
using System.Security.Cryptography;
int checks=0;
void Check(bool condition,string name) { if(!condition) throw new Exception(name); checks++; }
void Reject(Action action,string name) { try { action(); } catch(Exception e) when(e is InvalidDataException or ArgumentException or System.Text.Json.JsonException) { checks++; return; } throw new Exception(name); }
string dir=Path.Combine(Path.GetTempPath(),"vision-plan-"+Guid.NewGuid());
Directory.CreateDirectory(dir);
try {
 var options=new RadialCircleOptions(40,40,20,4,96,20,EdgePolarity.LightToDark,.5,1,20);
 var plan=new OfflineCirclePlan(OfflinePlanStore.Schema,"circle.bmp",options);
 string path=Path.Combine(dir,"test.vplan.json");
 OfflinePlanStore.Save(path,plan);
 var loaded=OfflinePlanStore.Load(path);
 Check(loaded.Options==options,"options roundtrip");
 Check(loaded.ImagePath==Path.Combine(dir,"circle.bmp"),"relative path anchored to plan directory");
 Reject(()=>OfflinePlanStore.Save(path,plan with {Schema="unknown"}),"schema");
 Reject(()=>OfflinePlanStore.Save(path,plan with {ImagePath=" "}),"empty path");
 Reject(()=>OfflinePlanStore.Save(path,plan with {Options=options with {InitialRadius=0}}),"invalid radius");
 var json=JsonNode.Parse(File.ReadAllText(path))!;
 json["Options"]!.AsObject().Remove("CenterX");
 string bad=Path.Combine(dir,"bad.vplan.json"); File.WriteAllText(bad,json.ToJsonString());
 Reject(()=>OfflinePlanStore.Load(bad),"missing ROI");
 json=JsonNode.Parse(File.ReadAllText(path))!; json["Schema"]="bad"; File.WriteAllText(bad,json.ToJsonString());
 Reject(()=>OfflinePlanStore.Load(bad),"load unknown schema");
 using(var stream=File.Create(loaded.ImagePath)) using(var w=new BinaryWriter(stream)) {
  int size=54+80*80*3;
  w.Write((byte)'B');w.Write((byte)'M');w.Write(size);w.Write(0);w.Write(54);w.Write(40);w.Write(80);w.Write(80);w.Write((ushort)1);w.Write((ushort)24);w.Write(0);w.Write(80*80*3);w.Write(0);w.Write(0);w.Write(0);w.Write(0);
  for(int y=79;y>=0;y--) for(int x=0;x<80;x++) {byte v=(x-40)*(x-40)+(y-40)*(y-40)<=400?(byte)240:(byte)10;w.Write(v);w.Write(v);w.Write(v);}
 }
 var result=OfflineCircleExecutor.Run(loaded);
 Check(result.Image.Width==80 && result.Image.Height==80,"BMP dimensions");
 Check(result.Detection.Status==CircleDetectionStatus.Success,"real BMP detection");
 Check(Math.Abs(result.Detection.Circle!.Value.Radius-20)<1,"radius");
 Check(Math.Abs(result.Detection.Circle.Value.CenterX-40)<.5 && Math.Abs(result.Detection.Circle.Value.CenterY-40)<.5,"center");
 Check(result.ImageSha256==Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(loaded.ImagePath))).ToLowerInvariant(),"input hash");
 Check(result.ElapsedMilliseconds>=0,"elapsed");
 Console.WriteLine($"PASS {checks} workflow checks");
} finally {Directory.Delete(dir,true);}
