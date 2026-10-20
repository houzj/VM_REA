using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
using VisionPlugins.MapCalibration;
try {
 int checks=0;void Check(bool value,string message){if(!value)throw new Exception(message);checks++;}
 string directory=Path.GetFullPath("analysis/map-calibration/image-file");using var sdk=JsonDocument.Parse(File.ReadLines("analysis/map-calibration/image-fit-weight1-trace/image-results.jsonl").First());var row=sdk.RootElement;
 float[] Values(string name)=>row.GetProperty(name).EnumerateArray().Select(v=>v.GetSingle()).ToArray();var scale=Values("scale");var ratio=Values("ratio");var result=new MapCalibrationImageResult(0,Values("matrix"),ratio[0],ratio[1],scale[0],scale[1],row.GetProperty("error").GetSingle(),[],[],[]);
 foreach(int mode in new[]{0,2,6}){string xml=File.ReadAllText(mode==6?Path.GetFullPath("analysis/map-calibration/image-runtime/teach-process.xml"):Path.Combine(directory,"image-writer-"+mode+".xml"));var x=XDocument.Parse(xml);string time=x.Descendants("CalibParam").Single(p=>p.Attribute("ParamName")?.Value=="CreateCalibTime").Element("ParamValue")!.Value;string actual=MapCalibrationImageFiles.Xml(result,DateTime.ParseExact(time,"yyyy-MM-dd HH:mm:ss",CultureInfo.InvariantCulture),current:mode==6?new MapCalibrationPose(-999999,-999,0):default,target:mode==6?new MapCalibrationPose(-999999,-999,0):default,teach:mode is 2 or 6?new MapCalibrationPose(10,20,30):null);Check(actual==xml,"Original image writer complete bytes mode "+mode);}
 foreach(string line in File.ReadLines(Path.Combine(directory,"image-results.jsonl"))){using var native=JsonDocument.Parse(line);int mode=native.RootElement.GetProperty("mode").GetInt32();string path=Path.Combine(directory,"managed-writer-"+mode+".xml");var saved=MapCalibrationImageFiles.Save(path,result,mode>0,mode>=2,mode==3,mode==4,teach:new(10,20,30));Check(saved.ErrorCode==native.RootElement.GetProperty("code").GetUInt32(),"Original five-state writer code "+mode);Check(saved.Written==native.RootElement.GetProperty("fileExists").GetBoolean(),"Original five-state file creation "+mode);}
 Check(MapCalibrationFiles.NativeFloat(1e-12f)=="1e-012","Original three-digit exponent");Console.WriteLine($"PASS {checks}: original image XML bytes and five writer states");File.WriteAllText("analysis/map-calibration/image-file-checks.json",JsonSerializer.Serialize(new{checks,passed=true,completeImageXmlBytes=true,authority="original writer unit and official SDK result; not full Process"}));return 0;
}catch(Exception error){Console.Error.WriteLine(error);return 1;}

