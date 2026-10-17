using System.Text.Json;
using VisionEditor.Core;
using VisionFramework;
using VisionCore;
static class CompoundLeafRuntimeChecks
{
 public static async Task Run(ModuleRegistry registry,Action<bool,string,object?> check){
  JsonElement J<T>(T value)=>JsonSerializer.SerializeToElement(value);
  var doc=new GraphEditorDocument(registry);var root=doc.CurrentProcedureId;var group=doc.AddOwnedGroup(100,100);doc.SetParameter(group,"EnableLoop",J(true));doc.SetParameter(group,"EndIndex",J(2));var body=doc.EnterOwnedGroup(group);
  var image=doc.AddNode("vision.image-file","1.0.0",100,100,new(){["Path"]=J(Path.GetFullPath("analysis/images/circle-find-sample.bmp"))});var threshold=doc.AddNode("vision.threshold","1.0.0",100,200,new(){["Threshold"]=J(128)});var blobs=doc.AddNode("vision.components","1.0.0",100,300);doc.Connect(image,"Image",threshold,"Image");doc.Connect(threshold,"Image",blobs,"Image");
  doc.SetWorkflowConfiguration(body,new(){Outputs=[new("areas","areas",DataKind.Number,blobs,"Components.Area",Accumulate:true){Cardinality=ValueCardinality.Array},new("count","count",DataKind.Integer,blobs,"Components.BlobNum")]});doc.LeaveGroup();
  var executor=new SolutionExecutor(registry);var run=await executor.RunAsync(new(doc.Document,doc.BaseDirectory),root);var child=run.Calls[group];var raw=child.Nodes.Single(n=>n.NodeId==blobs).Outputs["Components"].As<IReadOnlyList<Component>>();var areas=child.WorkflowOutputs["areas"].As<IReadOnlyList<double>>();
  check(run.Status==SessionStatus.Success&&raw.Count>0&&areas.Count==raw.Count*2&&areas.Sum()==raw.Sum(v=>(double)v.Area)*2,"Actual BMP threshold components leaf Area accumulates across two Group rounds",new{components=raw.Count,leafCount=areas.Count,total=areas.Sum()});
  check(child.WorkflowOutputs["count"].As<long>()==raw.Count,"Native-named BlobNum leaf exports actual algorithm component count",null);
  var bodyConfig=doc.Document.Procedures.Single(p=>p.Id==body).Configuration;
  doc.SetWorkflowConfiguration(body,bodyConfig with{Outputs=bodyConfig.Outputs.Append(new("root","blobs",DataKind.Components,blobs,"Components")).ToArray()});
  doc.SetWorkflowConfiguration(root,new(){Outputs=[new("total","total",DataKind.Number,group,"blobs.TotalArea")]});
  doc.SetWorkflowConfiguration(body,doc.Document.Procedures.Single(p=>p.Id==body).Configuration with{Outputs=doc.Document.Procedures.Single(p=>p.Id==body).Configuration.Outputs.Select(o=>o.Id=="root"?o with{Name="renamed"}:o).ToArray()});
  var renamed=await executor.RunAsync(new(doc.Document,doc.BaseDirectory),root);
  check(doc.CurrentProcedure.Configuration.Outputs.Single().Port=="renamed.TotalArea"&&renamed.WorkflowOutputs["total"].As<double>()==raw.Sum(v=>(double)v.Area),"Renaming compound Group root propagates derived leaf references and actual parent output",null);
  var withRoot=doc.Document.Procedures.Single(p=>p.Id==body).Configuration;doc.SetWorkflowConfiguration(body,withRoot with{Outputs=withRoot.Outputs.Where(o=>o.Id!="root").ToArray()});
  check(doc.CurrentProcedure.Configuration.Outputs.Single().Port is null,"Deleting compound Group export clears derived leaf binding rather than leaving stale path",null);doc.Undo();
  var path=Path.GetFullPath("analysis/full-audit/compound-leaf-roundtrip.vmx.json");doc.Save(path);var reopened=new GraphEditorDocument(registry);reopened.Load(path);var again=await executor.RunAsync(new(reopened.Document,reopened.BaseDirectory),root);check(again.Calls[group].WorkflowOutputs["areas"].As<IReadOnlyList<double>>().SequenceEqual(areas),"Compound leaf full path and accumulated values survive save reopen actual rerun",null);
 }
}
