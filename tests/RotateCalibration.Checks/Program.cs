using System.Text.Json;
using System.IO;
using VisionPlugins.Calibration;
TestProcessPolicy.Configure();
try {
if(args.Contains("--verify-failure-exit"))throw new InvalidOperationException("CONTROLLED_ROTATE_CHECK_FAILURE: verify captured main failure exit; this check must return 1.");
var root=Directory.GetCurrentDirectory();
using var evidenceDocument=JsonDocument.Parse(File.ReadAllText(Path.Combine(root,"docs/plugins/rotate-calibration-evidence.json")));var evidence=evidenceDocument.RootElement;
var source="docs/plugins/rotate-calibration-evidence.json";
float[][] matrices=[[1,0,0,0,1,0,0,0,1],[2,.3f,10,.1f,3,-5,0,0,1],[-2,0,0,0,2,0,0,0,1],[0,0,0,0,0,0,0,0,1]];
int checks=0,failed=0;
foreach(var row in evidence.GetProperty("numericalRun").GetProperty("cases").EnumerateArray()){string line=row.GetRawText();
 var samples=row.GetProperty("points").EnumerateArray().Select(p=>new RotateCalibrationSample(p[0].GetSingle(),p[1].GetSingle(),p[2].GetSingle())).ToArray();
 var r=RotateCalibrationSolver.Solve(samples,matrices[row.GetProperty("matrix").GetInt32()],row.GetProperty("reverse").GetBoolean());
 if(r.ErrorCode!=row.GetProperty("code").GetUInt32()){failed++;Console.WriteLine("status mismatch "+line);}
 double[] actual=[r.ImageX,r.ImageY,r.WorldX,r.WorldY,r.WorldError,r.PixelError];
 for(int i=0;i<6;i++){checks++;var n=row.GetProperty("values")[i];double expected=n.ValueKind==JsonValueKind.String?double.NaN:n.GetDouble();if(double.IsNaN(expected)&&double.IsNaN(actual[i]))continue;if(Math.Abs(expected-actual[i])<=1e-9*Math.Max(1,Math.Abs(expected)))continue;failed++;Console.WriteLine($"matrix={row.GetProperty("matrix")} rev={row.GetProperty("reverse")} count={samples.Length} field={i} native={expected:R} ours={actual[i]:R}");}
}
Console.WriteLine($"Rotate calibration: {checks} numerical checks, {failed} failures; original synthetic-host export evidence {source}");
var stateSource=Path.Combine(root,"analysis/full-audit/rotate-calibration/checks/native-fixtures");Directory.CreateDirectory(stateSource);foreach(var file in evidence.GetProperty("moduleRun").GetProperty("files").EnumerateObject())File.WriteAllBytes(Path.Combine(stateSource,file.Name),Convert.FromBase64String(file.Value.GetString()!));
RotateCalibrationSession? session=null;bool reverse=false;int instance=0;
foreach(var row in evidence.GetProperty("moduleRun").GetProperty("events").EnumerateArray()){string line=row.GetRawText();string phase=row.GetProperty("phase").GetString()!;
 if(phase=="instance-start"){session=new();reverse=instance++!=0;continue;}if(session is null)continue;
 if(phase=="file-case"){uint code=session.Load(Path.Combine(stateSource,row.GetProperty("name").GetString()!));checks+=11;if(code!=row.GetProperty("code").GetUInt32()||session.Loaded!=row.GetProperty("loaded").GetBoolean()){failed++;Console.WriteLine("file status "+line);}for(int i=0;i<9;i++)if(session.Matrix[i]!=row.GetProperty("matrix")[i].GetSingle()){failed++;Console.WriteLine("file matrix "+line);}continue;}
 if(phase=="set"){string key=row.GetProperty("key").GetString()!;if(key=="FilePath"&&row.GetProperty("lengthDelta").GetInt32()==0){if(!session.Loaded)session.Load(Path.Combine(stateSource,"matrix.iwcal"));else session.Load(Path.Combine(stateSource,"not-found.iwcal"));}continue;}
 if(!row.TryGetProperty("inputs",out var ins))continue;
 var inputs=ins.EnumerateObject().ToDictionary(p=>p.Name,p=>(IReadOnlyList<double>)p.Value.EnumerateArray().Select(x=>x.GetDouble()).ToArray());var step=session.Run(inputs,reverse);
 void Check(bool ok,string name){checks++;if(!ok){failed++;Console.WriteLine($"state {instance}/{phase}: {name}");}}
 Check(step.ErrorCode==row.GetProperty("code").GetUInt32(),"code");Check(session.Current==row.GetProperty("current").GetInt32(),"current");Check(session.Total==row.GetProperty("total").GetInt32(),"total");Check(session.Remaining==row.GetProperty("remain").GetInt32(),"remaining");Check(session.Loaded==row.GetProperty("loaded").GetBoolean(),"loaded");
 foreach(var write in row.GetProperty("writes").EnumerateArray()){string name=write.GetProperty("name").GetString()!;double expected=write.GetProperty("values")[0].GetDouble();Check(step.Outputs.TryGetValue(name,out var value)&&Math.Abs(value-expected)<1e-6*Math.Max(1,Math.Abs(expected)),name);}
 Check(step.Outputs.Count==row.GetProperty("writes").GetArrayLength(),"output presence");
}
Console.WriteLine($"Rotate calibration total: {checks} checks, {failed} failures");
checks+=UiChecks.Run(root);
checks+=await RuntimeChecks.Run(root);
checks+=DisplayChecks.Run(root);
Console.WriteLine($"With independent WPF behavior: {checks} checks, {failed} failures");
return failed==0?0:1;
} catch(Exception exception) {Console.Error.WriteLine(exception);return 1;}
