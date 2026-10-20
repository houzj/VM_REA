using System.Text.Json;
using VisionPlugins.DistortionCalibration;
try{
int checks=0;void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
using var evidence=JsonDocument.Parse(File.ReadAllText("docs/plugins/distortion-calibration-sdk-evidence.json"));var cases=evidence.RootElement.GetProperty("cases").EnumerateArray().Where(e=>e.TryGetProperty("board",out _)).ToArray();
foreach(var row in evidence.RootElement.GetProperty("exports").EnumerateArray()){
 var bytes=Convert.FromBase64String(row.GetProperty("base64").GetString()!);var data=DistortionCalibrationFile.Read(bytes);Check(data.Bytes().SequenceEqual(bytes),"Original SDK byte-for-byte .iccal roundtrip");
 var native=cases.Single(e=>$"board-{e.GetProperty("board").GetInt32()}-model-{e.GetProperty("model").GetInt32()}-warp-{e.GetProperty("warp").GetInt32()}-eval-{e.GetProperty("evaluate").GetInt32()}.iccal"==row.GetProperty("name").GetString());var matrix=native.GetProperty("matrix").EnumerateArray().Select(e=>(float)e.GetDouble()).ToArray();Check(data.Values.Select((v,i)=>BitConverter.SingleToInt32Bits(v)==BitConverter.SingleToInt32Bits(matrix[i])).All(v=>v),"Original file values match real SDK nineteen output floats");Check(data.Width==480&&data.Height==400&&data.Model==native.GetProperty("model").GetInt32(),"Observed dimensions/model positions");
}
Console.WriteLine($"Distortion original SDK binary file checks passed: {checks}");
var exportRow=evidence.RootElement.GetProperty("exports")[0];var original=DistortionCalibrationFile.Read(Convert.FromBase64String(exportRow.GetProperty("base64").GetString()!));
string directory=Path.GetFullPath("analysis/distortion-calibration/file-policy-checks/"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(directory);string path=Path.Combine(directory,"automatic.iccal");
Check(DistortionCalibrationFileLifecycle.WriteAutomatic(original,path,false)&&File.ReadAllBytes(path).SequenceEqual(original.Bytes()),"Missing file is exported even when refresh is false.");File.WriteAllBytes(path,[107,101,101,112]);
Check(!DistortionCalibrationFileLifecycle.WriteAutomatic(original,path,false)&&File.ReadAllBytes(path).SequenceEqual(new byte[]{107,101,101,112}),"Existing marker file survives refresh false.");
Check(DistortionCalibrationFileLifecycle.WriteAutomatic(original,path,true)&&File.ReadAllBytes(path).SequenceEqual(original.Bytes()),"Refresh true overwrites the marker with the real payload.");
foreach(string suffix in new[]{".txt",".ICCAL",""}){bool rejected=false;try{DistortionCalibrationFileLifecycle.WriteExplicit(original,Path.Combine(directory,"explicit"+suffix));}catch(DistortionCalibrationFileFailure failure){rejected=failure.NativeErrorCode==0xe0000204;}Check(rejected,"Explicit source extension rejection: "+suffix);}
string wrong=Path.Combine(directory,"automatic.txt");bool automaticRejected=false;try{DistortionCalibrationFileLifecycle.WriteAutomatic(original,wrong,true);}catch(DistortionCalibrationFileFailure failure){automaticRejected=failure.NativeErrorCode==0x8010000e;}Check(automaticRejected&&!File.Exists(wrong),"Automatic invalid extension has the distinct ProcessAlg error mapping.");
File.WriteAllBytes(wrong,[1,2,3]);Check(!DistortionCalibrationFileLifecycle.WriteAutomatic(original,wrong,false)&&File.ReadAllBytes(wrong).Length==3,"Refresh false bypasses export validation for an existing file.");
Check(!DistortionCalibrationFileLifecycle.WriteAutomatic(original,"",true),"Empty automatic path has no export side effect.");Console.WriteLine($"File codec and source lifecycle checks: {checks}.");
string existingDirectory=Path.Combine(directory,"directory.iccal");Directory.CreateDirectory(existingDirectory);Check(!DistortionCalibrationFileLifecycle.WriteAutomatic(original,existingDirectory,false),"Source _waccess(path,0) also recognizes an existing directory and bypasses refresh-false export.");string explicitPath=Path.Combine(directory,"explicit.iccal");DistortionCalibrationFileLifecycle.WriteExplicit(original,explicitPath);Check(File.ReadAllBytes(explicitPath).SequenceEqual(original.Bytes()),"Explicit valid save writes the real payload.");Console.WriteLine($"File lifecycle final checks: {checks}.");using var lifecycle=JsonDocument.Parse(File.ReadAllText("docs/plugins/distortion-calibration-lifecycle-evidence.json"));var lifecycleFile=lifecycle.RootElement.GetProperty("files")[0];var successful=DistortionCalibrationFile.Read(Convert.FromBase64String(lifecycleFile.GetProperty("bytes").GetString()!));using var session=new DistortionCalibrationArtifactSession();bool unavailable=false;try{session.Export(Path.Combine(directory,"before-fit.iccal"));}catch(DistortionCalibrationFileFailure failure){unavailable=failure.NativeErrorCode==0x80100003;}Check(unavailable,"Original before-run save error 80100003.");session.AcceptFit(successful);
foreach(var row in lifecycle.RootElement.GetProperty("files").EnumerateArray().Skip(1)){uint error=row.GetProperty("name").GetString()!.Contains("export")?0x8010000eu:0x10103000u;session.Fail(error);string retained=Path.Combine(directory,row.GetProperty("name").GetString()!);session.Export(retained);Check(File.ReadAllBytes(retained).SequenceEqual(Convert.FromBase64String(row.GetProperty("bytes").GetString()!))&&session.LastRunErrorCode==error,"Original failed run retains exact last successful export payload: "+row.GetProperty("name").GetString());}
foreach(bool automatic in new[]{false,true}){
 bool openFailed=false;
 try{if(automatic)DistortionCalibrationFileLifecycle.WriteAutomatic(original,existingDirectory,true);else session.Export(existingDirectory);}
 catch(DistortionCalibrationFileFailure failure){openFailed=failure.NativeErrorCode==0x80100010;}
 Check(openFailed,"SDK open-target failure code for "+(automatic?"automatic":"explicit")+" valid-suffix directory target.");
}
session.Dispose();Check(session.LastSuccessfulArtifact is null,"Owned scope disposal releases the result.");Console.WriteLine($"File and lifecycle total checks: {checks}.");return 0;
}catch(Exception error){Console.Error.WriteLine(error);return 1;}


