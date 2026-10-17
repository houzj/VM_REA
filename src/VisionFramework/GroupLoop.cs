using System.Text.Json;
namespace VisionFramework;

// Source: IMVSGroupAlgorithm.xml and ModuleFrame CGroupNode::DoWork.
// Break conditions and accumulated reporting are deliberately separate from this range runner.
public sealed record GroupLoop(bool Enabled,int Start,int End,int IntervalMs)
{
 // The shipped engine exposes LoopStep; the shipped configuration XML has no step control.
 public int Step {get;init;}=1;
 public long Increment=>Start<End?Step:-(long)Step;
 public static IReadOnlyList<ParameterDescriptor> Parameters {get;} = [
  new("EnableBreak",ParameterKind.Boolean,false,JsonSerializer.SerializeToElement(false)),
  new("DataType",ParameterKind.Integer,false,JsonSerializer.SerializeToElement(1),1,3),
  new("CmpType",ParameterKind.Integer,false,JsonSerializer.SerializeToElement(1),1,6),
  new("LoopStep",ParameterKind.Integer,false,JsonSerializer.SerializeToElement(1),int.MinValue,int.MaxValue),
  new("EnableLoop",ParameterKind.Boolean,false,JsonSerializer.SerializeToElement(false)),
  new("StartIndex",ParameterKind.Integer,false,JsonSerializer.SerializeToElement(0),int.MinValue,int.MaxValue),
  new("EndIndex",ParameterKind.Integer,false,JsonSerializer.SerializeToElement(1),int.MinValue,int.MaxValue),
  new("LoopTimeGap",ParameterKind.Integer,false,JsonSerializer.SerializeToElement(0),0,10000)];
 public static GroupLoop Read(NodeDocument node)
 {
  int Integer(string name,int fallback)=>node.Parameters.TryGetValue(name,out var v)?v.GetInt32():fallback;
  var enabled=node.Parameters.TryGetValue("EnableLoop",out var value)&&value.GetBoolean();
  var interval=Integer("LoopTimeGap",0);
  if(interval is <0 or >10000)throw new InvalidDataException("Group loop interval must be 0..10000 ms.");
  var step=Integer("LoopStep",1);
  if(step==0)throw new InvalidDataException("Group LoopStep must be nonzero.");
  return new(enabled,enabled?Integer("StartIndex",0):0,enabled?Integer("EndIndex",1):1,interval){Step=step};
 }
}
