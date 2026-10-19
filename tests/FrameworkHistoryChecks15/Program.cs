using System.Globalization;
using System.Text.Json;
using VisionEditor.Core;
using VisionFramework;

if(args.Length!=1)throw new ArgumentException("Supply an owned audit directory.");
string output=Path.Combine(Path.GetFullPath(args[0]),"framework-history-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(output);
var checks=new List<string>();
void Check(bool condition,string name){if(!condition)throw new Exception(name);checks.Add(name);}
void Reject<T>(Action action,string name)where T:Exception{try{action();}catch(T){Check(true,name);return;}throw new Exception("Missing rejection: "+name);}
var history=new FrameworkRunHistory();var time=new DateTime(2026,10,19,12,34,56,789);
Check(history.Entries.Count==0,"History begins empty");
for(int i=1;i<=750;i++)history.Append("p","flow",SessionStatus.Success,i,time);
Check(history.Entries.Count==500,"History is bounded at original display cap 500");
Check(history.Entries[0].Seq==750&&history.Entries[^1].Seq==251,"Newest first with oldest excess removed");
Check(history.Entries.Select(x=>x.Seq).SequenceEqual(Enumerable.Range(251,500).Reverse().Select(i=>(long)i)),"Sequence stays stable after eviction");
Check(history.Entries[0].Time=="2026-10-19 12:34:56:789","Full completion timestamp includes milliseconds");
Check(history.Entries[0].ProcedureId=="p"&&history.Entries[0].ElapsedMilliseconds==750,"Summary preserves procedure and actual elapsed");
history.Append("q","other",SessionStatus.Failed,3,time);
Check(history.Entries[0].Seq==751&&history.Entries[0].Data=="other · Failed"&&history.Entries.Count==500,"Mixed procedures and failures share bounded summary");
Reject<ArgumentOutOfRangeException>(()=>history.Append("q","other",SessionStatus.Success,double.NaN,time),"NaN elapsed rejected");
Reject<ArgumentOutOfRangeException>(()=>history.Append("q","other",SessionStatus.Success,-1,time),"Negative elapsed rejected");
Reject<ArgumentException>(()=>history.Append("","other",SessionStatus.Success,1,time),"Missing owner rejected");
Check(history.Entries[0].Seq==751&&history.Entries.Count==500,"Invalid append does not consume sequence or evict");
history.Clear();Check(history.Entries.Count==0,"Independent replacement clear empties own summary");
history.Append("new","new",SessionStatus.Cancelled,0,time);Check(history.Entries.Single().Seq==1,"Replacement resets independent summary counter");
NodeRunRecord Node(string id,double elapsed)=>new(id,NodeRunStatus.Success,new Dictionary<string,PortValue>(),elapsed);
var a=Node("a",12.25);var b=Node("b",3.5);var session=new ExecutionSessionResult(SessionStatus.Success,[a,b]);
var timing=FrameworkResultTiming.From(session,[a,b],"b");
Check(timing.ProcessText=="15.75","Process uses actual current managed session elapsed sum");
Check(timing.ModuleText=="3.50","Module uses selected node actual elapsed");
Check(timing.AlgorithmText==FrameworkResultTiming.Unavailable,"Algorithm time is unavailable rather than copied module time");
Check(FrameworkResultTiming.From(session,[a,b],"a").ModuleText=="12.25","Changing selection changes module time");
Check(FrameworkResultTiming.From(null,[],null).ProcessText==FrameworkResultTiming.Unavailable,"Missing current session has no stale process time");
Check(FrameworkResultTiming.From(session,[],"a").ModuleText==FrameworkResultTiming.Unavailable,"Cleared visible results have no stale module time");
Check(FrameworkResultTiming.From(session,[a,b],"missing").ModuleText==FrameworkResultTiming.Unavailable,"Unknown selected node has unavailable time");
Check(FrameworkResultTiming.From(new(SessionStatus.Failed,[Node("bad",double.PositiveInfinity)]),[Node("bad",-1)],"bad").ModuleText==FrameworkResultTiming.Unavailable,"Invalid node elapsed never displayed");
var originalCulture=CultureInfo.CurrentCulture;try{CultureInfo.CurrentCulture=CultureInfo.GetCultureInfo("fr-FR");Check(timing.ModuleText=="3.50","Time formatting remains stable across decimal cultures");}finally{CultureInfo.CurrentCulture=originalCulture;}
File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(new{passed=checks.Count,checks,scope="bounded independent main summaries and actual managed timing; not native module log/global SDK cache"},new JsonSerializerOptions{WriteIndented=true}));
Console.WriteLine("PASS "+checks.Count+" checks; audit="+output);
