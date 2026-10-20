using System.Text.Json;
using VisionPlugins.MapCalibration;
int checks=0;void Check(bool condition,string name){if(!condition)throw new Exception(name);checks++;}
string directory=Path.GetFullPath("analysis/map-calibration/native");var s=new MapCalibrationFileSession(directory);
Check(s.Save("before-first.xml").ErrorCode==0x80100203,"No original result cannot export");
Check(s.SetInputPath(0,"result-2.xml")==0,"First matrix read");Check(s.SetInputPath(1,"result-3.xml")==0,"Second matrix read");
var run=s.Run();Check(run.ErrorCode==0&&s.CalibrationValid,"Original controlled file-mode run");
using var original=JsonDocument.Parse(File.ReadLines(Path.Combine(directory,"results.jsonl")).Single(line=>JsonDocument.Parse(line).RootElement.GetProperty("case").GetInt32()==4));var expected=original.RootElement.GetProperty("matrix").EnumerateArray().Select(x=>x.GetSingle()).ToArray();for(int i=0;i<9;i++)Check(run.Matrix[i]==expected[i],"Official matrix "+i);
var io=s.Save("module-absent-parent/output.xml");Check(io.ErrorCode==0&&!io.Written&&s.OutputPath=="module-absent-parent/output.xml","Original ignored IO failure still commits path");
Check(s.SetInputPath(1,"reader-5.xml")==0xe0000018,"Original short XML error");Check(s.InputPath(1)=="result-3.xml"&&s.CachedMatrix(1)!.All(v=>v==0),"Original failed setter retains path but changes matrix cache");
var failed=s.Run();Check(failed.ErrorCode==0x10103002&&!s.CalibrationValid&&s.LastResult is null,"Original target-zero failure clears result availability");Check(s.Save("after-failed-run.xml").ErrorCode==0x80100203,"Original save after failed run rejected");
Check(s.SetInputPath(1,"result-3.xml")==0&&s.Run().ErrorCode==0,"Restore valid target");Check(s.SetInputPath(1,"not-existing.xml")==0xe0000201&&s.InputPath(1)=="result-3.xml","Missing file preserves path");Check(s.LastResult is not null,"Read failure before Run does not clear SDK result");
// Replay the normal original module's eight Process calls in an isolated local directory.
string replay=Path.GetFullPath("analysis/map-calibration/session-replay");Directory.CreateDirectory(replay);
void WriteMatrix(string name,int count=9,bool zero=false)=>File.WriteAllText(Path.Combine(replay,name),"<CalibInfo><CalibOutputParam><CalibFloatListParam>"+string.Concat(Enumerable.Range(0,count).Select(i=>"<ParamValue>"+(!zero&&i is 0 or 4 or 8?"1":"0")+"</ParamValue>"))+"</CalibFloatListParam></CalibOutputParam></CalibInfo>");
WriteMatrix("a.xml");WriteMatrix("b.xml");var replaySession=new MapCalibrationFileSession(replay);replaySession.SetInputPath(0,"a.xml");replaySession.SetInputPath(1,"b.xml");
var rows=File.ReadLines(Path.Combine(directory,"module-runtime-results.jsonl")).Select(l=>JsonDocument.Parse(l).RootElement.Clone()).Where(x=>x.TryGetProperty("step",out _)).ToArray();
foreach(var row in rows){int step=row.GetProperty("step").GetInt32();if(step==1)WriteMatrix("a.xml",2);if(step==3)WriteMatrix("a.xml");if(step==4)WriteMatrix("a.xml",9,true);if(step==6)WriteMatrix("b.xml",9,true);if(step==7){WriteMatrix("a.xml");WriteMatrix("b.xml");replaySession.SetOutputPath("absent/out.xml");replaySession.RefreshOutput=true;}
 var actual=replaySession.Run(row.GetProperty("refresh").GetInt32()!=0);Check(actual.ErrorCode==row.GetProperty("code").GetUInt32(),"Original Process code step "+step);Check((replaySession.LastResult is not null)==row.GetProperty("sdkResult").GetBoolean(),"Original SDK result availability step "+step);if(replaySession.LastResult is {} cached){int index=0;foreach(var coefficient in row.GetProperty("matrix").EnumerateArray())Check(cached.Matrix[index++]==coefficient.GetSingle(),"Original cached matrix step "+step);}}
Console.WriteLine($"PASS {checks}: per-module file path/cache/run/save lifecycle; source and controlled-module evidence");File.WriteAllText("analysis/map-calibration/session-checks.json",JsonSerializer.Serialize(new{checks,passed=true,controlledOriginalProcessSteps=rows.Length,generatedUtc=DateTime.UtcNow}));
