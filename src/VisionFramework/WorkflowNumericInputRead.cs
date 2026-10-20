namespace VisionFramework;
public readonly record struct ModuleNumericInputReadResult(uint ErrorCode,int Count,double Value);
public readonly record struct ModuleIntegerInputReadResult(uint ErrorCode,int Count,int Value);
public readonly record struct ModuleTextInputReadResult(uint ErrorCode,int Count,string Value,int RequiredBytes);
// Per-invocation raw input snapshot. The two methods model separate getter phases;
// an integer container is never silently converted into a successful float read.
public sealed class WorkflowNumericInputRead(IReadOnlyDictionary<string,PortValue> inputs,IReadOnlyDictionary<string,WorkflowArrayIndex?> indices,Func<WorkflowValueReference,PortValue> resolve){
 int Index(WorkflowArrayIndex? selector,int requestedIndex){
  if(selector?.LiteralText is {} literal)return WorkflowArrayIndex.ParseLiteral(literal);
  if(selector?.IndexSource is {} source){
   try{var value=resolve(source).Elements().FirstOrDefault();return value is not null&&value.Kind==DataKind.Integer?checked((int)value.As<long>()):0;}
   catch(Exception e)when(e is InvalidDataException or InvalidOperationException or OverflowException){return 0;}
  }
  return requestedIndex;
 }
 public ModuleNumericInputReadResult? Read(string name,int requestedIndex){
  if(!indices.TryGetValue(name,out var selector))return null;
  int index=Index(selector,requestedIndex);
  if(index<0)return new(0xe0000014,0,0);
  if(!inputs.TryGetValue(name,out var input))return new(0xe0000314,0,0);
  IReadOnlyList<double> values=input.Kind==DataKind.NumberArray?input.As<IReadOnlyList<double>>():input.Kind==DataKind.Number?input.Elements().Select(v=>v.As<double>()).ToArray():Array.Empty<double>();
  if(values.Count==0)return new(0xe0000314,0,0);
  if(index>=values.Count)return new(0xe0000014,0,0);
  return new(0,selector is null?values.Count:1,values[index]);
 }
 public ModuleIntegerInputReadResult? ReadInteger(string name,int requestedIndex){
  if(!indices.TryGetValue(name,out var selector))return null;
  int index=Index(selector,requestedIndex);
  if(index<0)return new(0xe0000014,0,0);
  if(!inputs.TryGetValue(name,out var input)||input.Kind!=DataKind.Integer)return new(0xe0000314,0,0);
  var values=input.Elements().ToArray();
  if(values.Length==0)return new(0xe0000314,0,0);
  if(index>=values.Length)return new(0xe0000014,0,0);
  return new(0,selector is null?values.Length:1,unchecked((int)values[index].As<long>()));
 }
 public ModuleTextInputReadResult? ReadText(string name,int requestedIndex,int capacity){
  if(!indices.TryGetValue(name,out var selector))return null;
  int index=Index(selector,requestedIndex);
  if(index<0)return new(0xe0000014,0,"",0);
  if(!inputs.TryGetValue(name,out var input)||input.Kind!=DataKind.Text)return new(0xe0000314,0,"",0);
  var values=input.Elements().ToArray();
  if(values.Length==0)return new(0xe0000314,0,"",0);
  if(index>=values.Length)return new(0xe0000014,0,"",0);
  string text=values[index].As<string>().Split('\0')[0];
  int required=checked(System.Text.Encoding.UTF8.GetByteCount(text)+1);
  if(capacity<required)return new(0xe0000012,0,"",required);
  return new(0,selector is null?values.Length:1,text,required);
 }

}
