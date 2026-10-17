using System.Collections.ObjectModel;
namespace VisionFramework;
// ProcessGroupOutputV44 clears mode1 primitives before append; mode0 retains them.
// Images are replaced every round. Managed lists are independent of the native ABI.
internal sealed class GroupOutputAccumulator(ProcedureDocument body){
 readonly Dictionary<string,List<PortValue>> displays=new(StringComparer.Ordinal);int displayCount;
 readonly Dictionary<string,int> elementCounts=new(StringComparer.Ordinal);
 readonly Dictionary<string,List<PortValue>> accumulated=new(StringComparer.Ordinal);int count;
 public void Add(ExecutionSessionResult iteration){foreach(var spec in body.Configuration.Displays){if(!iteration.WorkflowDisplays.TryGetValue(spec.Name,out var values))continue;if(!displays.TryGetValue(spec.Name,out var frames))displays[spec.Name]=frames=new();if(!spec.Accumulate||spec.Kind==DataKind.GrayImage){displayCount-=frames.Sum(v=>v.ElementCount);frames.Clear();}if(values.Sum(v=>(long)v.ElementCount)>1000000-displayCount)throw new InvalidOperationException("Independent display frame limit is 1000000.");displayCount+=values.Sum(v=>v.ElementCount);frames.AddRange(values);}foreach(var spec in body.Configuration.Outputs.Where(o=>o.Accumulate&&GroupBreak.Accumulatable(o.Kind))){if(!iteration.WorkflowOutputs.TryGetValue(spec.Name,out var value))continue;int length=value.ElementCount;if(length>1000000-count)throw new InvalidOperationException("Independent accumulated output element limit is 1000000.");count+=length;elementCounts[spec.Name]=elementCounts.GetValueOrDefault(spec.Name)+length;if(!accumulated.TryGetValue(spec.Name,out var list))accumulated[spec.Name]=list=new();list.Add(value);}}
 public PortValue ReadSelfElement(string name,int index,ExecutionSessionResult? last){
  var spec=body.Configuration.Outputs.SingleOrDefault(o=>o.Name==name)??throw new InvalidDataException("Group self output no longer exists.");
  if(!spec.Accumulate||!GroupBreak.Accumulatable(spec.Kind))return (last?.WorkflowOutputs.GetValueOrDefault(name)??throw new InvalidDataException("Group self output unavailable.")).Element(index);
  if(index<0||index>=elementCounts.GetValueOrDefault(name))throw new InvalidDataException("Group self output element is unavailable.");
  foreach(var value in accumulated[name]){int length=value.ElementCount;if(index<length)return value.Element(index);index-=length;}throw new InvalidDataException("Group self output unavailable.");
 }
 public IReadOnlyDictionary<string,IReadOnlyList<PortValue>> FinishDisplays()=>new ReadOnlyDictionary<string,IReadOnlyList<PortValue>>(displays.ToDictionary(p=>p.Key,p=>(IReadOnlyList<PortValue>)Array.AsReadOnly(p.Value.ToArray()),StringComparer.Ordinal));
 public IReadOnlyDictionary<string,PortValue> Finish(ExecutionSessionResult? last){var outputs=last?.WorkflowOutputs.ToDictionary(p=>p.Key,p=>p.Value,StringComparer.Ordinal)??new(StringComparer.Ordinal);foreach(var spec in body.Configuration.Outputs.Where(o=>o.Accumulate&&GroupBreak.Accumulatable(o.Kind))){var values=accumulated.GetValueOrDefault(spec.Name)??[];outputs[spec.Name]=PortValue.JoinArray(spec.Kind,values);}return new ReadOnlyDictionary<string,PortValue>(outputs);}
}
