using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Text;
using VisionCore;
using VisionFramework;

namespace VisionPlugins.ImageSource;

/// <summary>Optional adapter to an explicitly selected installed MVS camera SDK.</summary>
public sealed class MvsCameraAcquisition:IImageAcquisitionService,IDisposable {
 // The vendor runtime owns process-wide transport threads. Keep its library
 // loaded across solution reloads; device handles remain session-owned.
 static readonly System.Collections.Concurrent.ConcurrentDictionary<string,Lazy<IntPtr>> libraries=new(StringComparer.OrdinalIgnoreCase);
 public static MvsCameraAcquisition? TryCreateInstalled(){
  string native=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86),"MVS","Runtime","Win64_x64","MvCameraControl.dll");if(!File.Exists(native))return null;
  var candidates=new List<string?>{Environment.GetEnvironmentVariable("VISION_MVS_MANAGED_SDK"),Path.Combine(AppContext.BaseDirectory,"providers","MVS","MvCameraControl.Net.dll"),Path.Combine(AppContext.BaseDirectory,"MvCameraControl.Net.dll")};
  foreach(var view in new[]{Microsoft.Win32.RegistryView.Registry64,Microsoft.Win32.RegistryView.Registry32}){
   using var root=Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine,view);using var uninstall=root.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");if(uninstall is null)continue;
   foreach(string name in uninstall.GetSubKeyNames()){using var key=uninstall.OpenSubKey(name);if(key?.GetValue("DisplayName") is not string display||!display.StartsWith("VisionMaster",StringComparison.OrdinalIgnoreCase))continue;string? icon=key.GetValue("DisplayIcon") as string;if(icon is not null){icon=icon.Trim('"');int suffix=icon.LastIndexOf(",",StringComparison.Ordinal);if(suffix>0)icon=icon[..suffix].Trim('"');candidates.Add(Path.Combine(Path.GetDirectoryName(icon)!,"MvCameraControl.Net.dll"));}}
  }
  string? managed=candidates.FirstOrDefault(path=>path is not null&&File.Exists(path));return managed is null?null:new(managed,native);
 }
 readonly object gate=new();
 readonly AssemblyLoadContext loadContext;
 readonly Type api;
 readonly IntPtr library;
 readonly Dictionary<int,object> devices=new();
 readonly Dictionary<int,object> handles=new();
 readonly Dictionary<int,uint> previousFrames=new();
 readonly Dictionary<int,long> lostFrames=new(),lostPackets=new();
 bool disposed;
 public IReadOnlyList<CameraInfo> Cameras {get;}
 public uint SdkVersion {get;}
 public MvsCameraAcquisition(string managedAssembly,string nativeLibrary){
  if(!OperatingSystem.IsWindows()||RuntimeInformation.ProcessArchitecture!=Architecture.X64)throw new PlatformNotSupportedException("MVS camera adapter requires Windows x64.");
  loadContext=new("MVS-"+Guid.NewGuid().ToString("N"),isCollectible:true);
  try {
   var assembly=loadContext.LoadFromAssemblyPath(Path.GetFullPath(managedAssembly));api=assembly.GetType("MvCamCtrl.NET.MyCamera",throwOnError:true)!;
   library=libraries.GetOrAdd(Path.GetFullPath(nativeLibrary),path=>new Lazy<IntPtr>(()=>NativeLibrary.Load(path,assembly,DllImportSearchPath.UseDllDirectoryForDependencies|DllImportSearchPath.System32))).Value;
   NativeLibrary.SetDllImportResolver(assembly,(name,_,_)=>name.Equals("MvCameraControl.dll",StringComparison.OrdinalIgnoreCase)?library:IntPtr.Zero);
   SdkVersion=Convert.ToUInt32(api.GetMethod("MV_CC_GetSDKVersion_NET")!.Invoke(null,null));
   object list=Create("MV_CC_DEVICE_INFO_LIST");Set(list,"pDeviceInfo",new IntPtr[256]);object?[] arguments=[5u,list];Check(Invoke(null,"MV_CC_EnumDevices_NET",arguments),"Enumerate devices");list=arguments[1]!;
   uint count=Convert.ToUInt32(Get(list,"nDeviceNum"));if(count>256)throw new InvalidDataException("Invalid camera enumeration count.");var pointers=(IntPtr[])Get(list,"pDeviceInfo");var cameras=new List<CameraInfo>();
   for(int i=0;i<count;i++){object info=Marshal.PtrToStructure(pointers[i],Nested("MV_CC_DEVICE_INFO"))!;int id=i+1;devices.Add(id,info);var label=Describe(info,id);cameras.Add(new(id,label.Name){PersistentId=label.Serial});}
   Cameras=cameras.AsReadOnly();
  } catch {loadContext.Unload();throw;}
 }
 Type Nested(string name)=>api.GetNestedType(name,BindingFlags.Public)??throw new MissingMemberException(name);
 object Create(string name)=>Activator.CreateInstance(Nested(name))!;
 static object Get(object value,string name)=>value.GetType().GetField(name)!.GetValue(value)!;
 static void Set(object value,string name,object item)=>value.GetType().GetField(name)!.SetValue(value,item);
 int Invoke(object? handle,string name,params object?[] arguments){var method=api.GetMethods().Single(m=>m.Name==name&&m.GetParameters().Length==arguments.Length);try{return Convert.ToInt32(method.Invoke(handle,arguments));}catch(TargetInvocationException ex)when(ex.InnerException is not null){System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex.InnerException).Throw();throw;}}
 static void Check(int result,string operation){if(result!=0)throw new CameraAcquisitionException(unchecked((uint)result),operation+" failed: 0x"+unchecked((uint)result).ToString("x8"));}
 (string Name,string Serial) Describe(object info,int id){
  uint layer=Convert.ToUInt32(Get(info,"nTLayerType"));object special=Get(info,"SpecialInfo");string member=layer==1?"stGigEInfo":"stUsb3VInfo",type=layer==1?"MV_GIGE_DEVICE_INFO":"MV_USB3_DEVICE_INFO";var raw=(byte[])Get(special,member);IntPtr pointer=Marshal.AllocHGlobal(raw.Length);
  try{Marshal.Copy(raw,0,pointer,raw.Length);object detail=Marshal.PtrToStructure(pointer,Nested(type))!;string Text(string name){object value=Get(detail,name);if(value is string text)return text.TrimEnd('\0');byte[] bytes=(byte[])value;int end=Array.IndexOf(bytes,(byte)0);return Encoding.UTF8.GetString(bytes,0,end<0?bytes.Length:end);}string serial=Text("chSerialNumber");return ($"{id}: {Text("chModelName")} {serial}".Trim(),serial);}finally{Marshal.FreeHGlobal(pointer);}
 }
 object Open(int id){
  if(handles.TryGetValue(id,out var handle))return handle;if(!devices.TryGetValue(id,out var device))throw new CameraAcquisitionException(0xe0000309,"Camera not bound.");
  handle=Activator.CreateInstance(api)!;object?[] arguments=[device];Check(Invoke(handle,"MV_CC_CreateDevice_NET",arguments),"Create camera");
  try{Check(Invoke(handle,"MV_CC_OpenDevice_NET"),"Open camera");Check(Invoke(handle,"MV_CC_StartGrabbing_NET"),"Start acquisition");handles.Add(id,handle);return handle;}catch{Invoke(handle,"MV_CC_CloseDevice_NET");Invoke(handle,"MV_CC_DestroyDevice_NET");throw;}
 }
 public ValueTask<CameraFrame> AcquireAsync(CameraRequest request,CancellationToken cancellationToken)=>new(Task.Run(()=>{
  lock(gate){ObjectDisposedException.ThrowIf(disposed,this);cancellationToken.ThrowIfCancellationRequested();var handle=Open(request.CameraId);
   // Original ImageSource tries the fallback name but does not fail acquisition
   // when both parameter writes are rejected (confirmed with negative inputs).
   void Write(double? value,string key,string fallback){if(value is null)return;float number=float.Parse(((float)value.Value).ToString("F3",CultureInfo.InvariantCulture),CultureInfo.InvariantCulture);int result=Invoke(handle,"MV_CC_SetFloatValue_NET",key,number);if(result!=0)_=Invoke(handle,"MV_CC_SetFloatValue_NET",fallback,number);}
   void Wait(){if(request.SetParameterWaitTimeMs>0&&cancellationToken.WaitHandle.WaitOne(request.SetParameterWaitTimeMs))cancellationToken.ThrowIfCancellationRequested();}
   Write(request.ExposureTime,"ExposureTime","ExposureTimeAbs");Wait();Write(request.Gain,"Gain","GainRaw");Wait();
   uint ReadEnum(string key){object?[] query=[key,Create("MVCC_ENUMVALUE")];Check(Invoke(handle,"MV_CC_GetEnumValue_NET",query),key);return Convert.ToUInt32(Get(query[1]!,"nCurValue"));}
   // Do not overwrite the device's trigger configuration. A software-triggered
   // camera requires one command per requested image; external triggers wait.
   if(ReadEnum("TriggerMode")!=0&&ReadEnum("TriggerSource")==7)Check(Invoke(handle,"MV_CC_SetCommandValue_NET","TriggerSoftware"),"Software trigger");
   object payload=Create("MVCC_INTVALUE_EX");object?[] query=["PayloadSize",payload];Check(Invoke(handle,"MV_CC_GetIntValueEx_NET",query),"Read payload size");long size=Convert.ToInt64(Get(query[1]!,"nCurValue"));if(size<=0||size>256L*1024*1024)throw new InvalidDataException("Camera payload exceeds resource limit.");IntPtr buffer=Marshal.AllocHGlobal(checked((int)size));
   try{
    object?[] frame=[buffer,(uint)size,Create("MV_FRAME_OUT_INFO_EX"),100];var clock=Stopwatch.StartNew();int result;
    do{cancellationToken.ThrowIfCancellationRequested();result=Invoke(handle,"MV_CC_GetOneFrameTimeout_NET",frame);if(result==0)break;if(unchecked((uint)result)!=0x80000007&&unchecked((uint)result)!=0x80000107)Check(result,"Read frame");}while(clock.ElapsedMilliseconds<3000);Check(result,"Read frame timeout");
    object info=frame[2]!;uint frameLength=Convert.ToUInt32(Get(info,"nFrameLen"));if(frameLength==0||frameLength>size)throw new InvalidDataException("Invalid camera payload length.");int width=Convert.ToInt32(Get(info,"nWidth")),height=Convert.ToInt32(Get(info,"nHeight"));if(width<=0||height<=0||(long)width*height>VisionIO.BmpReader.MaxPixels)throw new InvalidDataException("Camera dimensions exceed resource limit.");object pixelType=Get(info,"enPixelType");uint type=Convert.ToUInt32(pixelType);bool mono=pixelType.ToString()!.Contains("Mono",StringComparison.Ordinal);int bytes=checked(width*height*(mono?1:3));byte[] pixels=new byte[bytes];uint target=mono?0x01080001u:0x02180014u;
    if(type==target){if(Convert.ToUInt32(Get(info,"nFrameLen"))<bytes)throw new InvalidDataException("Truncated camera frame.");Marshal.Copy(buffer,pixels,0,bytes);}
    else {IntPtr converted=Marshal.AllocHGlobal(bytes);try{object conversion=Create("MV_PIXEL_CONVERT_PARAM");Set(conversion,"nWidth",(ushort)width);Set(conversion,"nHeight",(ushort)height);Set(conversion,"enSrcPixelType",pixelType);Set(conversion,"pSrcData",buffer);Set(conversion,"nSrcDataLen",Convert.ToUInt32(Get(info,"nFrameLen")));Set(conversion,"enDstPixelType",Enum.ToObject(pixelType.GetType(),target));Set(conversion,"pDstBuffer",converted);Set(conversion,"nDstBufferSize",(uint)bytes);Set(conversion,"nRes",new uint[4]);object?[] convert=[conversion];Check(Invoke(handle,"MV_CC_ConvertPixelType_NET",convert),"Convert camera pixels");if(Convert.ToUInt32(Get(convert[0]!,"nDstLen"))!=bytes)throw new InvalidDataException("Invalid converted frame size.");Marshal.Copy(converted,pixels,0,bytes);}finally{Marshal.FreeHGlobal(converted);}}
    uint number=Convert.ToUInt32(Get(info,"nFrameNum"));long lost=lostFrames.GetValueOrDefault(request.CameraId);if(previousFrames.TryGetValue(request.CameraId,out uint previous)&&number>previous+1)lost+=number-previous-1;previousFrames[request.CameraId]=number;lostFrames[request.CameraId]=lost;long packets=lostPackets.GetValueOrDefault(request.CameraId)+Convert.ToUInt32(Get(info,"nLostPacket"));lostPackets[request.CameraId]=packets;cancellationToken.ThrowIfCancellationRequested();return new CameraFrame(mono?new GrayImage(width,height,pixels):new RgbImage(width,height,pixels),number,lost,packets);
   } finally{Marshal.FreeHGlobal(buffer);}
  }
 },cancellationToken));
 public void Dispose(){lock(gate){if(disposed)return;disposed=true;foreach(var handle in handles.Values){Invoke(handle,"MV_CC_StopGrabbing_NET");Invoke(handle,"MV_CC_CloseDevice_NET");Invoke(handle,"MV_CC_DestroyDevice_NET");}handles.Clear();devices.Clear();loadContext.Unload();}}
}
