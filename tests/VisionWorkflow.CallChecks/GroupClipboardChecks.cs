using System.Text.Json;
using VisionEditor.Core;
using VisionFramework;
using VisionCore;

static class GroupClipboardChecks
{
 public static async Task Run(ModuleRegistry registry,Action<bool,string,object?> check)
 {
  var doc=new GraphEditorDocument(registry);var root=doc.CurrentProcedureId;
  var group=doc.AddNode(WorkflowCalls.ModuleId,WorkflowCalls.Version,100,100);var body=doc.EnterOwnedGroup(group);
  var nested=doc.AddNode(WorkflowCalls.ModuleId,WorkflowCalls.Version,100,100);var inner=doc.EnterOwnedGroup(nested);
  var source=doc.AddNode("vision.image-file","1.0.0",100,100,new(){["Path"]=JsonSerializer.SerializeToElement(Path.GetFullPath("analysis/images/circle-find-sample.bmp"))});
  var threshold=doc.AddNode("vision.threshold","1.0.0",100,200,new(){["Threshold"]=JsonSerializer.SerializeToElement(50)});
  var components=doc.AddNode("vision.components","1.0.0",100,300);
  doc.Connect(source,"Image",threshold,"Image");doc.Connect(threshold,"Image",components,"Image");
  doc.SetWorkflowConfiguration(inner,new(){Outputs=[new("count","count",DataKind.Integer,components,"Count"),new("image","image",DataKind.GrayImage,threshold,"Image")],RunParameters=[new("threshold",threshold,"Threshold","阈值")]});
  doc.LeaveGroup();doc.SetWorkflowConfiguration(body,new(){Outputs=[new("count","count",DataKind.Integer,nested,"count"),new("image","image",DataKind.GrayImage,nested,"image")]});doc.LeaveGroup();
  var executor=new SolutionExecutor(registry);var original=await executor.RunAsync(new(doc.Document,doc.BaseDirectory),root);
  var expected=original.Calls[group].WorkflowOutputs["count"].As<long>();
  var fragment=doc.CopyNodes([group]);check(fragment.OwnedProcedures.Count==2,"Group clipboard captures both owned internal graphs",null);
  doc.SetProcedureNodeParameters(inner,threshold,new(){["Threshold"]=JsonSerializer.SerializeToElement(90)});
  var updated=await executor.RunAsync(new(doc.Document,doc.BaseDirectory),root);expected=updated.Calls[group].WorkflowOutputs["count"].As<long>();
  var copied=doc.PasteNodes(fragment,300,200).Single();
  var result=await executor.RunAsync(new(doc.Document,doc.BaseDirectory),root);
  check(result.Status==SessionStatus.Success&&result.Calls[copied].WorkflowOutputs["count"].As<long>()==expected,"Group paste exports current live owned graph and executes real BMP pipeline",expected);
  var copiedBody=doc.Document.Procedures.Single(p=>p.GroupOwner==new GroupOwnership(root,copied));
  var copiedNested=copiedBody.Nodes.Single();var copiedInner=doc.Document.Procedures.Single(p=>p.Id==copiedNested.Call!.ProcedureId);
  check(copiedBody.Name==doc.CurrentProcedure.Nodes.Single(n=>n.Id==copied).Name,"Copied Group node and internal Tab share the same module-set name",null);
  check(copiedBody.Id!=body&&copiedInner.Id!=inner&&copiedInner.Configuration.RunParameters.Single().NodeId!=threshold,"Group clipboard remaps ownership and runtime proxy targets",null);
  check(copiedInner.Nodes.Single(n=>n.ModuleId=="vision.threshold").Parameters["Threshold"].GetInt32()==90,"Paste copies current source parameters rather than stale copy-time snapshot",null);
  doc.Undo();check(doc.Document.Procedures.Count==3&&doc.CurrentProcedure.Nodes.Count==1,"One undo removes whole pasted Group closure",null);
  doc.Redo();check(doc.Document.Procedures.Count==5&&doc.CurrentProcedure.Nodes.Any(n=>n.Id==copied),"One redo restores Group closure and identities",null);
  doc.EditProcedure(copiedBody.Id,d=>d.RenameProcedure(copiedBody.Id,"重命名Group"));
  check(doc.CurrentProcedure.Nodes.Single(n=>n.Id==copied).Name=="重命名Group"&&doc.Document.Procedures.Single(p=>p.Id==copiedBody.Id).Name=="重命名Group","Group module-set rename updates node and internal canvas atomically",null);
  doc.Undo();
  var second=doc.PasteNodes(fragment,500,200).Single();
  check(doc.Document.Procedures.Count==7&&doc.CurrentProcedure.Nodes.Single(n=>n.Id==second).Call!.ProcedureId!=copiedBody.Id,"Repeated Group paste owns distinct internal graphs",null);
  doc.EnterOwnedGroup(copied);doc.EnterOwnedGroup(copiedNested.Id);var current=doc.CurrentProcedureId;var revision=doc.Revision;
  bool rejected=false;try{doc.PasteNodes(fragment,500,500);}catch(InvalidDataException){rejected=true;}
  check(rejected&&doc.Revision==revision&&doc.CurrentProcedureId==current,"Over-depth nested Group paste is rejected before mutation",null);
  bool rollback=false;try{doc.EditProcedure(root,d=>{d.RenameProcedure(root,"should rollback");throw new InvalidOperationException();});}catch(InvalidOperationException){rollback=true;}
  check(rollback&&doc.Document.Procedures.Single(p=>p.Id==root).Name!="should rollback"&&doc.Revision==revision,"Bound procedure edit rolls back complete failed callback without changing canvas",null);
  doc.EditProcedure(root,d=>d.SetParameter(copied,"LoopTimeGap",JsonSerializer.SerializeToElement(3)));
  check(doc.CurrentProcedureId==current&&doc.Document.Procedures.Single(p=>p.Id==root).Nodes.Single(n=>n.Id==copied).Parameters["LoopTimeGap"].GetInt32()==3,"Bound owner parameter edit keeps nested canvas selected",null);
  doc.Undo();check(doc.CurrentProcedureId==current,"Undo bound owner edit preserves visible nested canvas",null);
  doc.SelectProcedure(root);doc.DeleteNode(group);revision=doc.Revision;
  check(doc.PasteNodes(fragment,100,100).Count==0&&doc.Revision==revision,"Paste skips deleted source Group as native ProcedurePaste does",null);
  var path=Path.GetFullPath("analysis/full-audit/group-clipboard-closure.vmx.json");doc.Save(path);var loaded=new GraphEditorDocument(registry);loaded.Load(path);
  var reopened=await executor.RunAsync(new(loaded.Document,loaded.BaseDirectory),root);
  check(reopened.Status==SessionStatus.Success&&reopened.Calls[copied].WorkflowOutputs["count"].As<long>()==expected&&reopened.Calls[second].WorkflowOutputs["count"].As<long>()==expected,"Both pasted Group closures save reopen and execute real image pipeline",expected);
  var edges=new GraphEditorDocument(registry);var edgeRoot=edges.CurrentProcedureId;
  var upstream=edges.AddNode("vision.image-file","1.0.0",100,100,new(){["Path"]=JsonSerializer.SerializeToElement(Path.GetFullPath("analysis/images/circle-find-sample.bmp"))});
  var consumer=edges.AddNode("vision.threshold","1.0.0",100,200,new(){["Threshold"]=JsonSerializer.SerializeToElement(70)});edges.Connect(upstream,"Image",consumer,"Image");
  var consumerCopy=edges.CopyNodes([consumer]);var newConsumer=edges.PasteNodes(consumerCopy,300,300).Single();
  check(edges.CurrentProcedure.Connections.Any(c=>c.FromNode==upstream&&c.ToNode==newConsumer)&&ProcedureGraph.ExecutionEdges(edges.CurrentProcedure).Any(e=>e.FromNode==upstream&&e.ToNode==newConsumer),"Same-parent paste preserves live external upstream data and execution connections",null);
  var edgeRun=await executor.RunAsync(new(edges.Document,edges.BaseDirectory),edgeRoot);
  check(edgeRun.Status==SessionStatus.Success&&edgeRun.Nodes.Single(n=>n.NodeId==consumer).Outputs["Image"].As<GrayImage>().Pixels.SequenceEqual(edgeRun.Nodes.Single(n=>n.NodeId==newConsumer).Outputs["Image"].As<GrayImage>().Pixels),"Copied consumer actually processes original upstream image with identical pixels",null);
  edges.Undo();edges.DeleteNode(consumer);revision=edges.Revision;
  check(edges.PasteNodes(consumerCopy,300,300).Count==0&&edges.Revision==revision,"Deleted ordinary clipboard source is skipped without adding history",null);
 }
}
