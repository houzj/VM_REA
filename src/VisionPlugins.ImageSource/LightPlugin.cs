using System.Buffers.Binary;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using VisionFramework;
using VisionPluginContracts;
namespace VisionPlugins.ImageSource;

/// <summary>Independent reconstruction of LightModule, including the native 748-byte command boundary.</summary>
public sealed class LightPlugin:IVisionToolPlugin,IWpfToolEditorFactory {
 public const string Id="vision.light";
 public ToolManifest Manifest{get;}=new(Id,"1.0.0","光源","collection","catalog/LightModuleImageEnable.png");
 public static int NormalizeDeviceType(int type)=>type switch{1 or 0x1004=>101,2 or 0x440101=>103,3 or 0x880204=>110,4 or 0x204=>107,5 or 0x880000=>106,6 or 0x380000=>108,7 or 0x881304=>201,0x1006=>102,0xff0204=>109,0x2cf0208=>114,_=>type};
 public static int BrightnessMaximum(int type)=>NormalizeDeviceType(type) is 103 or 104 or 105 or 107 or 109 or 110 or 114?100:255;
 // EnumerationLight applies LightSourceHelper's old-global-resource mapping before passing the selection to the native module.
 public static string DeviceSelection(AcquisitionLightDevice device){int type=device.DeviceType;if(device.ModuleId==14000)type=type switch{4100=>101,4456705=>103,4456706=>104,8913412=>110,516=>107,8912896=>106,3670016=>108,25690628=>105,47120904=>114,_=>type};return $"{device.DeviceIndex}#{device.ModuleId}#{NormalizeDeviceType(type)}#{device.DeviceGuid}";}
 public sealed record Device(int Index,int ModuleId,int Type,string Guid){public static Device Parse(string text){var parts=text.Split('#',4);return parts.Length>=4&&int.TryParse(parts[0],out int index)&&int.TryParse(parts[1],out int module)&&int.TryParse(parts[2],out int type)?new(index,module,NormalizeDeviceType(type),parts[3]):new(-1,-1,0,"");}}
 static ParameterDescriptor P<T>(string name,ParameterKind kind,T value,double? min=null,double? max=null)=>new(name,kind,true,JsonSerializer.SerializeToElement(value),min,max);
 static ParameterDescriptor[] Parameters(){var result=new List<ParameterDescriptor>{P("LightDevice",ParameterKind.Text,""),P("TriggerTime",ParameterKind.Integer,500,-1,65535),P("OutputType",ParameterKind.Integer,1),P("InputType",ParameterKind.Integer,1,1,2)};for(int i=1;i<=8;i++){result.Add(P($"Channel{i}Enable",ParameterKind.Boolean,false));result.Add(P($"Channel{i}Brightness",ParameterKind.Integer,0,0,255));result.Add(P($"Channel{i}LightState",ParameterKind.Integer,1));result.Add(P($"Channel{i}EdgeType",ParameterKind.Integer,1));result.Add(P($"Channel{i}DurationTime",ParameterKind.Integer,0,0,30000));}result.Add(P("Channel1LightStateVB",ParameterKind.Integer,1));result.Add(P("Channel2LightStateVB",ParameterKind.Integer,1));return result.ToArray();}
 public ModuleDescriptor Descriptor{get;}=new(Id,"1.0.0","光源",Enumerable.Range(1,8).Select(i=>new PortDescriptor($"InputChannel{i}Brightness",DataKind.Integer,false)).Append(new("InputString",DataKind.Text,false)).ToArray(),Enumerable.Range(1,8).SelectMany(i=>new[]{"Enable","Brightness","LightState","EdgeType","DurationTime"}.Select(s=>new PortDescriptor($"Channel{i}{s}",DataKind.Integer,false))).Concat(new[]{new PortDescriptor("ModuStatus",DataKind.Integer),new("ModuRunTime",DataKind.Number)}).ToArray(),Parameters());
 public IModule CreateRuntime()=>new Runtime(this);
 public Form CreateEditor(ToolEditorContext context)=>throw new InvalidOperationException("Use WPF editor factory.");
 public System.Windows.Window CreateWpfEditor(ToolEditorContext context)=>new LightWindow(context);
 sealed class State {public readonly SemaphoreSlim Gate=new(1,1);public string Device="";public readonly int[] Brightness=new int[8];}
 readonly Dictionary<string,State> fallback=new();readonly object gate=new();
 State GetState(ModuleExecutionContext c){if(c.Scope is {} scope)return scope.State(Id,c.NodeId,()=>new State());lock(gate){string key=Path.GetFullPath(c.BaseDirectory)+"|"+c.NodeId;if(!fallback.TryGetValue(key,out var state))fallback[key]=state=new();return state;}}
 public void Reset(string nodeId){lock(gate)foreach(string key in fallback.Keys.Where(k=>k.EndsWith("|"+nodeId,StringComparison.Ordinal)).ToArray())fallback.Remove(key);}
 public static byte[] Encode(Device device,int triggerTime,IReadOnlyList<bool> enabled,IReadOnlyList<int> brightness,IReadOnlyList<int> lightStates,IReadOnlyList<int> edges,IReadOnlyList<int> durations){
  if(new[]{enabled.Count,brightness.Count,lightStates.Count,edges.Count,durations.Count}.Any(n=>n!=8))throw new ArgumentException("Eight channel values required.");
  // Native copies a fixed 16-byte UTF-8 identifier. Zero short-field padding instead of copying uninitialized storage.
  byte[] data=new byte[748];void I(int offset,int value)=>BinaryPrimitives.WriteInt32LittleEndian(data.AsSpan(offset,4),value);I(0,device.Index);I(4,device.Type);var identifier=Encoding.UTF8.GetBytes(device.Guid);identifier.AsSpan(0,Math.Min(16,identifier.Length)).CopyTo(data.AsSpan(8,16));I(24,triggerTime);
  for(int i=0;i<8;i++){int offset=28+i*44;I(offset,i+1);data[offset+4]=enabled[i]?(byte)1:(byte)0;I(offset+8,brightness[i]);I(offset+12,lightStates[i]);I(offset+16,edges[i]);I(offset+20,durations[i]);}return data;
 }
 sealed class Runtime(LightPlugin owner):IModule {
  public ModuleDescriptor Descriptor=>owner.Descriptor;
  public async ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext c,CancellationToken token){token.ThrowIfCancellationRequested();var timer=Stopwatch.StartNew();var state=owner.GetState(c);await state.Gate.WaitAsync(token);var output=new Dictionary<string,PortValue>();uint code=0;int status=0;string? message=null;
   try{string text=c.Text("LightDevice");var device=Device.Parse(text);if(state.Device!=text){state.Device=text;if(device.Index>=0)Array.Clear(state.Brightness);}int max=BrightnessMaximum(device.Type);var enabled=Enumerable.Range(1,8).Select(i=>c.Boolean($"Channel{i}Enable")).ToArray();var states=Enumerable.Range(1,8).Select(i=>c.Integer($"Channel{i}LightState")).ToArray();if(device.Type is 103 or 104){states[0]=c.Integer("Channel1LightStateVB");if(device.Type==104)states[1]=c.Integer("Channel2LightStateVB");}var edges=Enumerable.Range(1,8).Select(i=>c.Integer($"Channel{i}EdgeType")).ToArray();var durations=Enumerable.Range(1,8).Select(i=>c.Integer($"Channel{i}DurationTime")).ToArray();
    if(Enumerable.Range(1,8).Any(i=>c.Integer($"Channel{i}Brightness") is <0||c.Integer($"Channel{i}Brightness")>255)||durations.Any(n=>n is <0 or >30000)||c.Integer("TriggerTime") is <-1 or >65535){code=0xe000030e;message="光源参数超出原厂范围。";output["ModuStatus"]=PortValue.Integer(0);}
    else{
     string trigger=c.Inputs.TryGetValue("InputString",out var input)?input.As<string>():"";int nul=trigger.IndexOf('\0');if(nul>=0)trigger=trigger[..nul];bool match=c.Integer("OutputType")==1&&trigger=="OK"||c.Integer("OutputType")==2&&trigger=="NG";
     if(device.Index>=0&&match){for(int i=0;i<8;i++)state.Brightness[i]=c.Inputs.TryGetValue($"InputChannel{i+1}Brightness",out var value)?checked((int)value.As<long>()):c.Integer($"Channel{i+1}Brightness");
      // Original sends the staged bytes before validating subscribed brightness for outputs.
      var transport=c.Scope?.Service<IAcquisitionLightModuleTransport>();try{if(transport is not null){await transport.SetBinaryDataAsync(device.ModuleId,"LightDeviceParamSetTrigger",Encode(device,c.Integer("TriggerTime"),enabled,state.Brightness,states,edges,durations),token);status=1;}else message="光源传输服务不可用。";}catch(OperationCanceledException){throw;}catch(IOException ex){message=ex.Message;}
     }
     output["ModuStatus"]=PortValue.Integer(status);for(int i=0;i<8;i++)output[$"Channel{i+1}Enable"]=PortValue.Integer(enabled[i]?1:0);
     for(int i=0;i<8;i++){if(state.Brightness[i]<0||state.Brightness[i]>max){code=0xe000030e;message="订阅亮度超出原厂范围。";break;}output[$"Channel{i+1}Brightness"]=PortValue.Integer(state.Brightness[i]);}
     if(code==0&&device.Type is >=2 and <=6)for(int i=0;i<8;i++){output[$"Channel{i+1}LightState"]=PortValue.Integer(states[i]);output[$"Channel{i+1}EdgeType"]=PortValue.Integer(edges[i]);output[$"Channel{i+1}DurationTime"]=PortValue.Integer(durations[i]);}
    }
   }finally{state.Gate.Release();}
   if(code!=0)output["ModuStatus"]=PortValue.Integer(0);output["ModuRunTime"]=PortValue.Number(timer.Elapsed.TotalMilliseconds);return new(code!=0?ModuleStatus.Failed:status==1?ModuleStatus.Success:ModuleStatus.NoMatch,output,code!=0?"LightParameterRange":null,message){NativeErrorCode=code};
  }
 }
}
