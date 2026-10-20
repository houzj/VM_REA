using System.Text;
using VisionFramework;
using VisionPluginContracts;
namespace VisionPlugins.Calibration;
public sealed partial class NPointPlugin{
 static ToolTextInputReadResult ReadTeachText(ModuleExecutionContext c,string name){
  const int capacity=0x200;
  ToolTextInputReadResult result;
  if(c.Scope?.Service<IToolTextInputReader>() is {} reader)result=reader.Read(c,name,0,capacity);
  else if(c.ReadTextInput?.Invoke(name,0,capacity) is {} read)result=new(read.ErrorCode,read.Count,read.Value,read.RequiredBytes);
  else if(!c.Inputs.TryGetValue(name,out var input)||input.Kind!=DataKind.Text)result=new(0xe0000314,0,"",0);
  else{
   var values=input.Elements().ToArray();
   if(values.Length==0)result=new(0xe0000314,0,"",0);
   else{string text=values[0].As<string>().Split('\0')[0];int bytes=checked(Encoding.UTF8.GetByteCount(text)+1);result=bytes>capacity?new(0xe0000012,0,"",bytes):new(0,values.Length,text,bytes);}
  }
  // VM_M failure zeros Count: first status is overwritten by second caller read.
  return result.ErrorCode==0?result:new(result.ErrorCode,0,"",result.RequiredBytes);
 }
}
