using System.Text.Json;
using VisionEditor.Core;

if(args.Length!=1)throw new ArgumentException("Supply an owned audit directory.");
string output=Path.Combine(Path.GetFullPath(args[0]),"epoch-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(output);
var checks=new List<string>();void Check(bool value,string name){if(!value)throw new Exception(name);checks.Add(name);}
var epoch=new ExecutionOwnerEpoch();var owner=new object();var ticket=epoch.Capture("same-id",owner);
Check(epoch.IsCurrent(ticket,"same-id",owner),"Current owner accepted");
Check(!epoch.IsCurrent(default,"same-id",owner),"Default ticket rejected");
Check(!epoch.IsCurrent(ticket,"different",owner),"Different solution rejected");
Check(!epoch.IsCurrent(ticket,"same-id",new object()),"Different scope rejected even with same solution id");
Check(!new ExecutionOwnerEpoch().IsCurrent(ticket,"same-id",owner),"Different host rejects foreign ticket");
var entered=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);var release=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);int writes=0;
async Task Pending(){entered.SetResult();await release.Task;if(epoch.IsCurrent(ticket,"same-id",owner))writes++;}
Task old=Pending();await entered.Task;epoch.Invalidate();release.SetResult();await old;
Check(writes==0,"Real async late completion cannot commit after same-id invalidation");
Check(!epoch.IsCurrent(ticket,"same-id",owner),"Invalidation rejects old generation even with same object scope");
var current=epoch.Capture("same-id",owner);Check(epoch.IsCurrent(current,"same-id",owner),"New generation accepts new work");
epoch.Close();Check(!epoch.IsCurrent(current,"same-id",owner),"Close rejects pending work");
Check(!epoch.IsCurrent(epoch.Capture("same-id",owner),"same-id",owner),"Closed gate cannot be reopened by capture");
File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(new{passed=checks.Count,checks,scope="independent managed owner guard; not vendor SDK generation semantics"},new JsonSerializerOptions{WriteIndented=true}));
Console.WriteLine("PASS "+checks.Count+" checks; audit="+output);
