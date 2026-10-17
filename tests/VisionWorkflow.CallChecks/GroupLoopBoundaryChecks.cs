using System.Text.Json;
using VisionEditor.Core;
using VisionFramework;
using VisionCore;

static class GroupLoopBoundaryChecks
{
 public static async Task Run(ModuleRegistry registry,Action<bool,string,object?> check)
 {
  var doc=new GraphEditorDocument(registry);var root=doc.CurrentProcedureId;
  var group=doc.AddOwnedGroup(100,100);var body=doc.EnterOwnedGroup(group);
  var source=doc.AddNode("vision.image-file","1.0.0",100,100,new(){["Path"]=JsonSerializer.SerializeToElement(Path.GetFullPath("analysis/images/circle-find-sample.bmp"))});
  var threshold=doc.AddNode("vision.threshold","1.0.0",100,200,new(){["Threshold"]=JsonSerializer.SerializeToElement(0)});
  var count=doc.AddNode("vision.components","1.0.0",100,300);doc.Connect(source,"Image",threshold,"Image");doc.Connect(threshold,"Image",count,"Image");
  doc.SetWorkflowConfiguration(body,new(){Subscriptions=[new(threshold,"Threshold",true,new(WorkflowSourceKind.GroupLoopIndex,"LoopIndex"))],Outputs=[new("count","count",DataKind.Integer,count,"Count",Accumulate:true),new("image","image",DataKind.GrayImage,threshold,"Image")]});doc.LeaveGroup();
  void Set(string key,object value)=>doc.SetParameter(group,key,JsonSerializer.SerializeToElement(value));
  Set("EnableLoop",true);Set("StartIndex",20);Set("EndIndex",200);Set("LoopStep",50);
  var executor=new SolutionExecutor(registry);
  async Task<ExecutionSessionResult> Run()=>await executor.RunAsync(new(doc.Document,doc.BaseDirectory),root);
  var result=await Run();var session=result.Calls[group];
  check(result.Status==SessionStatus.Success&&session.CompletedGroupIterations==4&&session.GroupLoopIndex==170,"Stepped Group executes four real image rounds",session.CompletedGroupIterations);
  var original=session.Nodes.Single(n=>n.NodeId==source).Outputs["Image"].As<GrayImage>();
  check(session.Nodes.Single(n=>n.NodeId==threshold).Outputs["Image"].As<GrayImage>().Pixels.SequenceEqual(Algorithms.Threshold(original,170).Pixels),"Last image is actually thresholded with loop index170 rather than index23",null);
  var expected=new List<long>();
  foreach(var index in new[]{20,70,120,170}){Set("StartIndex",index);Set("EndIndex",index+1);var one=await Run();expected.Add(one.Calls[group].WorkflowOutputs["count"].As<IReadOnlyList<long>>().Single());}
  check(session.WorkflowOutputs["count"].As<IReadOnlyList<long>>().SequenceEqual(expected),"Stepped Group accumulation equals four independently executed real thresholds",expected);
  Set("StartIndex",20);Set("EndIndex",200);
  doc.SetGroupBreak(group,true,1,5,new(new(true,new(WorkflowSourceKind.GroupLoopIndex,"LoopIndex")),new(Constant:JsonSerializer.SerializeToElement(120))));
  result=await Run();session=result.Calls[group];
  check(result.Status==SessionStatus.Success&&session.GroupBroke&&session.GroupBreakIndex==120&&session.CompletedGroupIterations==2,"Stepped Group checks break before index120 and executes only20/70",session.GroupBreakIndex);
  check(session.WorkflowOutputs["count"].As<IReadOnlyList<long>>().SequenceEqual(expected.Take(2))&&session.WorkflowOutputs["image"].As<GrayImage>().Pixels.SequenceEqual(Algorithms.Threshold(original,70).Pixels),"Break preserves only completed stepped outputs and last real image",null);
  var path=Path.GetFullPath("analysis/full-audit/group-loop-boundary.vmx.json");doc.Save(path);var loaded=new GraphEditorDocument(registry);loaded.Load(path);
  var reopened=await executor.RunAsync(new(loaded.Document,loaded.BaseDirectory),root);
  check(reopened.Calls[group].GroupBreakIndex==120&&reopened.Calls[group].WorkflowOutputs["count"].As<IReadOnlyList<long>>().SequenceEqual(expected.Take(2)),"Save reopen retains LoopStep, loop-index subscription, break and accumulation",null);
  var copy=doc.PasteNodes(doc.CopyNodes([group]),300,100).Single();var copied=await Run();check(copied.Calls[copy].CompletedGroupIterations==2&&copied.Calls[copy].GroupBreakIndex==120,"Pasted Group retains stepped break execution with independent internal identities",null);doc.DeleteNode(copy);
  doc.SetGroupBreak(group,false,1,5,null);Set("StartIndex",200);Set("EndIndex",20);result=await Run();
  check(result.Status==SessionStatus.Failed&&result.Calls[group].CompletedGroupIterations==0&&result.Calls[group].WorkflowOutputs["count"].As<IReadOnlyList<long>>().Count==0&&!result.Calls[group].WorkflowOutputs.ContainsKey("image"),"Descending range executes no body, clears old image and reports missing required image rather than stale success",null);
  Set("EndIndex",200);result=await Run();check(result.Calls[group].CompletedGroupIterations==0,"Equal range executes zero iterations",null);
  Set("EnableLoop",false);result=await Run();check(result.Status==SessionStatus.Success&&result.Calls[group].CompletedGroupIterations==1&&result.Calls[group].WorkflowOutputs["image"].As<GrayImage>().Pixels.SequenceEqual(Algorithms.Threshold(original,0).Pixels),"Disabled loop uses native default0..1 range despite saved start/end/step",null);
  var revision=doc.Revision;var rejected=false;try{doc.SetParameters(group,doc.CurrentProcedure.Nodes.Single(n=>n.Id==group).Parameters.ToDictionary(k=>k.Key,k=>k.Key=="LoopStep"?JsonSerializer.SerializeToElement(0):k.Value));}catch(ArgumentException){rejected=true;}
  check(rejected&&doc.Revision==revision,"Zero LoopStep configuration rejects atomically before execution",null);
  rejected=false;try{Set("LoopStep",0);}catch(ArgumentException){rejected=true;}
  check(rejected&&doc.Revision==revision,"Native-style single LoopStep setter rejects zero without mutating scheme",null);
  var empty=new GraphEditorDocument(registry);var emptyRoot=empty.CurrentProcedureId;var emptyGroup=empty.AddOwnedGroup(100,100);empty.SetParameter(emptyGroup,"EnableLoop",JsonSerializer.SerializeToElement(true));empty.SetParameter(emptyGroup,"EndIndex",JsonSerializer.SerializeToElement(3));empty.SetParameter(emptyGroup,"LoopStep",JsonSerializer.SerializeToElement(-1));empty.SetGroupBreak(emptyGroup,true,1,6,new(new(true,new(WorkflowSourceKind.GroupLoopIndex,"LoopIndex")),new(Constant:JsonSerializer.SerializeToElement(-2))));
  var negative=await executor.RunAsync(new(empty.Document,empty.BaseDirectory),emptyRoot);check(negative.Calls[emptyGroup].CompletedGroupIterations==2&&negative.Calls[emptyGroup].GroupBreakIndex==-2,"Signed negative native step follows less-than guard and can be terminated by configured break",null);
  empty.SetGroupBreak(emptyGroup,false,1,6,null);empty.SetParameter(emptyGroup,"LoopStep",JsonSerializer.SerializeToElement(int.MinValue));
  var overflow=await executor.RunAsync(new(empty.Document,empty.BaseDirectory),emptyRoot);
  check(overflow.Status==SessionStatus.Failed&&overflow.Nodes.Single(n=>n.NodeId==emptyGroup).Message!.Contains("overflow guard"),"Independent int32 index guard fails safely instead of wrapping negative step forever",null);
  empty.SetParameter(emptyGroup,"StartIndex",JsonSerializer.SerializeToElement(int.MaxValue-1));empty.SetParameter(emptyGroup,"EndIndex",JsonSerializer.SerializeToElement(int.MaxValue));empty.SetParameter(emptyGroup,"LoopStep",JsonSerializer.SerializeToElement(int.MaxValue));
  var large=await executor.RunAsync(new(empty.Document,empty.BaseDirectory),emptyRoot);check(large.Status==SessionStatus.Success&&large.Calls[emptyGroup].CompletedGroupIterations==1,"Wide next-index arithmetic terminates oversized positive step without int32 wraparound",null);
 }
}
