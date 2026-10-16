namespace VisionFramework;
// VmIODecorate.IsDisplayForImage creates/removes a linked DynamicObject;
// deleting the IO removes that display object too. Independent stable identities replace native object pointers.
public static class GroupDisplayLinks {
 public static WorkflowConfiguration Synchronize(WorkflowConfiguration before,WorkflowConfiguration next){
  if(next.Outputs.Any(o=>o.Display&&o.Kind!=DataKind.GrayImage))throw new InvalidDataException("Automatic compound output display is not implemented.");
  var outputs=next.Outputs.Select(o=>o.Display&&before.Outputs.Any(old=>old.Id==o.Id&&old.Display)&&before.Displays.Any(d=>d.LinkedOutputId==o.Id)&&!next.Displays.Any(d=>d.LinkedOutputId==o.Id)?o with{Display=false}:o).ToArray();
  var displays=next.Displays.Where(d=>d.LinkedOutputId is null).ToList();
  foreach(var output in outputs.Where(o=>o.Display&&o.Kind==DataKind.GrayImage)){
   var current=next.Displays.FirstOrDefault(d=>d.LinkedOutputId==output.Id);if(current is null){int n=0;while(displays.Any(d=>d.Name=="display"+n))n++;current=new(Guid.NewGuid().ToString("N"),"display"+n,output.Kind){LinkedOutputId=output.Id};}
   displays.Add(current with{NodeId=output.NodeId,Port=output.Port,Kind=output.Kind,Cardinality=output.Cardinality});
  }
  return next with{Outputs=outputs,Displays=displays.ToArray()};
 }
}
