// Runs vendor SDK code in this probe process. Never saves the loaded solution.
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Web.Script.Serialization;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

public static class OriginalImageSourceProbe
{
    [DllImport("kernel32.dll", CharSet=CharSet.Unicode)] static extern bool SetDllDirectory(string path);
    [DllImport("kernel32.dll", CharSet=CharSet.Unicode)] static extern IntPtr GetModuleHandle(string name);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate byte NativeLess(IntPtr left,IntPtr right);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate IntPtr NativeFiles(IntPtr owner,IntPtr result,IntPtr directory);
    static IntPtr SmallString(string text) {
        byte[] bytes=System.Text.Encoding.Default.GetBytes(text);if(bytes.Length>15)throw new ArgumentException("Probe SSO input too long");
        IntPtr result=Marshal.AllocHGlobal(32);Marshal.Copy(new byte[32],0,result,32);Marshal.Copy(bytes,0,result,bytes.Length);Marshal.WriteInt64(result,16,bytes.Length);Marshal.WriteInt64(result,24,15);return result;
    }
    static void FolderCatalogScenarios(string output) {
        // Bound to the already loaded, hash-checked original. No patching or injected hooks.
        IntPtr binary=GetModuleHandle("ImageSourceModule.dll");if(binary==IntPtr.Zero)throw new Exception("Original native module missing");
        NativeLess less=(NativeLess)Marshal.GetDelegateForFunctionPointer(IntPtr.Add(binary,0x23550),typeof(NativeLess));
        string[] names={"img1.bmp","img2.bmp","img10.bmp","Img2.bmp","img02.bmp","a.bmp","B.bmp","a-2.png","a_2.png","图2.bmp","图10.bmp"};
        foreach(string left in names)foreach(string right in names){IntPtr a=SmallString(left),b=SmallString(right);try{records.Add(new{phase="folder-less",left,right,less=less(a,b)!=0});}finally{Marshal.FreeHGlobal(a);Marshal.FreeHGlobal(b);}}
        string directory=Path.Combine(Path.GetDirectoryName(output),"catalog");Directory.CreateDirectory(directory);
        foreach(string name in new[]{"img10.bmp","img2.bmp","img1.jpg","upper.JPG","color.png","multi.part.tif","long.jpeg","long.tiff","raw.BMP","bad.txt","dotless","noext.","almost.bm","图3.png"})File.WriteAllText(Path.Combine(directory,name),"fixture");
        Directory.CreateDirectory(Path.Combine(directory,"subdir.bmp"));File.WriteAllText(Path.Combine(directory,"subdir.bmp","nested.jpg"),"fixture");
        NativeFiles enumerate=(NativeFiles)Marshal.GetDelegateForFunctionPointer(IntPtr.Add(binary,0x22750),typeof(NativeFiles));
        IntPtr owner=Marshal.AllocHGlobal(0x26000),vector=Marshal.AllocHGlobal(24),input=SmallString("catalog");Marshal.Copy(new byte[0x26000],0,owner,0x26000);Marshal.Copy(new byte[24],0,vector,24);
        try {enumerate(owner,vector,input);long begin=Marshal.ReadInt64(vector),end=Marshal.ReadInt64(vector,8);long count=(end-begin)/32;if(count<0||count>100)throw new Exception("Unexpected original vector bounds");var paths=new List<string>();for(int i=0;i<count;i++){IntPtr item=new IntPtr(begin+i*32);int length=checked((int)Marshal.ReadInt64(item,16));IntPtr data=Marshal.ReadInt64(item,24)>15?Marshal.ReadIntPtr(item):item;paths.Add(Marshal.PtrToStringAnsi(data,length));}records.Add(new{phase="folder-catalog",paths=paths});}
        finally {Marshal.FreeHGlobal(owner);Marshal.FreeHGlobal(vector);Marshal.FreeHGlobal(input);}
        // Returned vendor-owned vector allocations intentionally die with this isolated probe.
    }
    static string root;
    static object Fields(object value){return value.GetType().GetFields().ToDictionary(f=>f.Name,f=>f.GetValue(value));}
    static void CameraScenarios(object solution,object module,object parameters,string output){
        Type cameraType=Assembly.LoadFrom(Path.Combine(root,"Module(sp)/x64/Global/GlobalCameraModule/GlobalCameraModuleCs.dll")).GetType("GlobalCameraModuleCs.GlobalCameraModuleTool",true);
        object camera=Activator.CreateInstance(cameraType,new object[]{solution});object cp=Get(camera,"ModuParams");
        try{
            object list=Call(cp,"GetCameraInfoList");records.Add(new{phase="camera-catalog",data=Fields(list)});
            int count=Convert.ToInt32(list.GetType().GetField("nNum").GetValue(list));if(count!=1)throw new Exception("Exactly one original camera required");object info=((Array)list.GetType().GetField("astCameraInfo").GetValue(list)).GetValue(0);string serial=Convert.ToString(info.GetType().GetField("strCameraSN").GetValue(info));
            Call(cp,"SetChosenCameraSN",serial);records.Add(new{phase="camera-bind",serial,chosen=Call(cp,"GetChosenCameraSN"),connected=Call(camera,"bIsCameraConnect"),id=Get(camera,"ID")});
            Parameter(parameters,"ImageSourceType",2);StringParameter(module,"CameraID",Convert.ToString(Get(camera,"ID")));Parameter(parameters,"OutMono8",true);
            double exposure=Convert.ToDouble(Get(cp,"ExposureTime")),gain=Convert.ToDouble(Get(cp,"Gain"));records.Add(new{phase="camera-settings",exposure,gain,triggerSource=Get(cp,"TriggerSource"),width=Get(cp,"Width"),height=Get(cp,"Height")});
            for(int i=0;i<3;i++){Scenario(module,"camera-frame-"+i,output);object result=Get(module,"ModuResult");records.Add(new{phase="camera-counters-"+i,frameNum=Get(result,"FrameNum"),lostFrames=Get(result,"LostFrameCount"),lostPackets=Get(result,"LostPacketCount"),failures=Get(result,"GetCameraFailCount")});}
            try{int index=0;foreach(float[] values in new[]{new[]{1000.1234f,1.2345f},new[]{5000.5678f,3.4567f},new[]{-1f,-1f}}){Call(parameters,"SetInputFloat","ExposureTimeInput",new float[]{values[0]});Call(parameters,"SetInputFloat","GainInput",new float[]{values[1]});StringParameter(module,"SetCameraParamWaitTime","10");Scenario(module,"camera-parameters-"+index,output);records.Add(new{phase="camera-readback-"+index++,requestedExposure=values[0],requestedGain=values[1],exposure=Get(cp,"ExposureTime"),gain=Get(cp,"Gain")});}}finally{Parameter(cp,"ExposureTime",exposure);Parameter(cp,"Gain",gain);records.Add(new{phase="camera-settings-restored",exposure=Get(cp,"ExposureTime"),gain=Get(cp,"Gain")});}
            Call(parameters,"SetInputFloat","ExposureTimeInput",new float[]{(float)exposure});Call(parameters,"SetInputFloat","GainInput",new float[]{(float)gain});
            int source=Convert.ToInt32(Get(cp,"TriggerSource"));try{Parameter(cp,"TriggerSource",0);Scenario(module,"camera-no-trigger",output);object failed=Get(module,"ModuResult");records.Add(new{phase="camera-failure-counters",failures=Get(failed,"GetCameraFailCount"),frame=Get(failed,"FrameNum")});}finally{Parameter(cp,"TriggerSource",source);}
            Scenario(module,"camera-recovered",output);object recovered=Get(module,"ModuResult");records.Add(new{phase="camera-recovered-counters",failures=Get(recovered,"GetCameraFailCount"),frame=Get(recovered,"FrameNum")});
            Parameter(parameters,"StitchEnable",true);Parameter(parameters,"StitchHeight",1);Parameter(parameters,"StitchStartHeight",2048);Scenario(module,"camera-invalid-stitch",output);object invalid=Get(module,"ModuResult");records.Add(new{phase="camera-invalid-stitch-counters",frame=Get(invalid,"FrameNum"),failures=Get(invalid,"GetCameraFailCount")});Parameter(parameters,"StitchEnable",false);Scenario(module,"camera-after-invalid-stitch",output);
        }finally{Call(camera,"Dispose");}
    }
    static readonly List<object> records = new List<object>();
    static readonly List<string> uiErrors = new List<string>();
    static object Get(object instance, string name) { return instance.GetType().GetProperty(name).GetValue(instance, null); }
    static object Call(object instance, string name, params object[] args) {
        return instance.GetType().GetMethods().Where(m => m.Name == name && m.GetParameters().Length == args.Length).OrderBy(m=>m.DeclaringType==instance.GetType()?0:1).First().Invoke(instance, args);
    }
    static void Parameter(object parameters,string key,object value) {
        PropertyInfo property=parameters.GetType().GetProperty(key);
        if(property.PropertyType.IsEnum)value=Enum.ToObject(property.PropertyType,Convert.ToInt32(value));
        else value=Convert.ChangeType(value,property.PropertyType);
        property.SetValue(parameters,value,null);
    }
    static void Snapshot(object module,string phase,string output) {
        Call(module,"Run"); object result=Get(module,"ModuResult"); object image=Get(result,"ImageData");
        int length=Convert.ToInt32(Get(image,"DataLen")); byte[] pixels=new byte[length];
        if(length>0)Marshal.Copy((IntPtr)Get(image,"ImageDataPtr"),pixels,0,length);
        object mono=Get(result,"ImageDataMono8");int monoLength=Convert.ToInt32(Get(mono,"DataLen"));byte[] monoPixels=new byte[monoLength];if(monoLength>0)Marshal.Copy((IntPtr)Get(mono,"ImageDataPtr"),monoPixels,0,monoLength);File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(output),phase+".mono.pixels"),monoPixels);
        records.Add(new{phase=phase+"-extra-mono",width=Get(mono,"Width"),height=Get(mono,"Height"),format=Get(mono,"Pixelformat"),length=monoLength,pixelSha256=BitConverter.ToString(SHA256.Create().ComputeHash(monoPixels)).Replace("-","").ToLowerInvariant()});
        File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(output),phase+".pixels"),pixels);
        records.Add(new { phase=phase,nativeErrorCode=Get(result,"ErrorCode"),sn=Get(result,"SNCode"),path=Get(result,"CurImagePath"),name=Get(result,"CurImageName"),width=Get(image,"Width"),height=Get(image,"Height"),format=Get(image,"Pixelformat"),length=length,extraMonoLength=Get(Get(result,"ImageDataMono8"),"DataLen"),pixelSha256=BitConverter.ToString(SHA256.Create().ComputeHash(pixels)).Replace("-","").ToLowerInvariant() });
    }
    static void StringParameter(object module,string key,string value) {
        object parameters=Get(module,"Params");parameters.GetType().GetProperty("Item").SetValue(parameters,value,new object[]{key});
    }
    static void Scenario(object module,string name,string output) {
        try { Snapshot(module,name,output); } catch(Exception ex) {
            while(ex.InnerException!=null)ex=ex.InnerException;
            PropertyInfo code=ex.GetType().GetProperty("errorCode");
            records.Add(new {phase=name,scenarioError=ex.GetType().FullName,code=code==null?null:Convert.ToString(code.GetValue(ex,null))});
        }
    }
    static void Scenarios(object module,object parameters,string sample,string output) {
        string directory=Path.Combine(Path.GetDirectoryName(output),"native-fixtures");Directory.CreateDirectory(directory);
        string a=Path.Combine(directory,"a.bmp"),b=Path.Combine(directory,"b.bmp"),c=Path.Combine(directory,"c.bmp");
        foreach(string path in new[]{a,b,c})File.Copy(sample,path,true);
        Call(module,"ClearAllInputImage");foreach(string path in new[]{a,b,c})Call(module,"AddInputImageByPath",path);
        Parameter(parameters,"AutoPlay",true);StringParameter(module,"CurrentImagePath",a);Scenario(module,"current-then-auto",output);
        Parameter(parameters,"AutoPlay",false);StringParameter(module,"CurrentImagePath",a);Scenario(module,"manual-current",output);
        StringParameter(module,"ExecuteType","2");StringParameter(module,"SelectExecutePic",c);StringParameter(module,"SelectExecutePic",a);Parameter(parameters,"AutoPlay",true);
        Scenario(module,"selected-first",output);Scenario(module,"selected-second",output);
        StringParameter(module,"DelAllExecutePic","");Scenario(module,"empty-selection",output);
        StringParameter(module,"ExecuteType","1");Parameter(parameters,"AutoPlay",false);StringParameter(module,"CurrentImagePath",a);Parameter(parameters,"PathCache",1);
        Scenario(module,"cache-admit",output);File.Delete(a);
        try{Scenario(module,"cache-source-removed",output);}finally{File.Copy(sample,a,true);}
        Parameter(parameters,"ImageSourceType",2);StringParameter(module,"CameraID","-1");Scenario(module,"camera-unbound",output);
        Parameter(parameters,"ImageSourceType",3);Scenario(module,"sdk-unsubmitted",output);
        Call(module,"SetImagePath",sample);Scenario(module,"sdk-submitted-path",output);
        Parameter(parameters,"ImageSourceType",1);
    }
    static void StitchScenarios(object module,object parameters,string output) {
        string directory=Path.Combine(Path.GetDirectoryName(output),"stitch-fixtures");Directory.CreateDirectory(directory);
        string a=Path.Combine(directory,"rows-a.bmp"),b=Path.Combine(directory,"rows-b.bmp");
        WriteRows(a,new byte[]{10,20,30,40});WriteRows(b,new byte[]{50,60,70,80});
        Call(module,"ClearAllInputImage");Call(module,"AddInputImageByPath",a);Call(module,"AddInputImageByPath",b);
        foreach(var setting in new[]{new[]{"ImageSourceType","1"},new[]{"PixelFormat","1"},new[]{"AutoPlay","True"},new[]{"StitchEnable","True"},new[]{"StitchHeight","2"},new[]{"StitchStartHeight","1"},new[]{"ClearTrigger","False"},new[]{"PathCache","0"}})Parameter(parameters,setting[0],setting[1]);
        Scenario(module,"stitch-first",output);Scenario(module,"stitch-second",output);
        Parameter(parameters,"StitchStartHeight",0);Scenario(module,"stitch-start-change",output);
        Parameter(parameters,"StitchHeight",1);Scenario(module,"stitch-height-change",output);
        Parameter(parameters,"StitchStartHeight",4);Scenario(module,"stitch-invalid-crop",output);
        Parameter(parameters,"StitchStartHeight",0);Scenario(module,"stitch-after-error",output);
        Parameter(parameters,"StitchEnable",false);Scenario(module,"stitch-disabled",output);
        Parameter(parameters,"StitchEnable",true);Scenario(module,"stitch-reenabled",output);
        Parameter(parameters,"StitchHeight",0);Scenario(module,"stitch-zero-height",output);
        Parameter(parameters,"StitchHeight",1);Parameter(parameters,"AutoPlay",false);StringParameter(module,"CurrentImagePath",a);
        WriteRows(a,new byte[]{10,20,30,40},5);Scenario(module,"stitch-width-change",output);
        WriteRows(a,new byte[]{10,20,30,40});Scenario(module,"stitch-width-restored",output);
        Parameter(parameters,"StitchEnable",false);
    }
    static void CodecScenarios(object module,object parameters,string output) {
        string directory=Path.Combine(Path.GetDirectoryName(output),"codec-fixtures");
        Parameter(parameters,"ImageSourceType",1);Parameter(parameters,"PixelFormat",1);Parameter(parameters,"StitchEnable",false);Parameter(parameters,"PathCache",0);Parameter(parameters,"AutoPlay",false);
        foreach(string path in ImageSourceCodecFixtures.Create(directory)) {
            Call(module,"ClearAllInputImage");Call(module,"AddInputImageByPath",path);
            string name="codec-"+Path.GetFileName(path);
            records.Add(new {phase="codec-fixture",name=name,fileSha256=BitConverter.ToString(SHA256.Create().ComputeHash(File.ReadAllBytes(path))).Replace("-","").ToLowerInvariant()});
            Scenario(module,name,output);
            if(Path.GetExtension(path)==".jpg") {Parameter(parameters,"PixelFormat",2);Scenario(module,"codec-rgb-color.jpg",output);Parameter(parameters,"PixelFormat",1);}
        }
    }
    static void FolderScenarios(string output) {
        Type type=Assembly.LoadFrom(Path.Combine(root,"myLibs/Apps.XmlParser.dll")).GetType("Apps.XmlParser.ParamTab.OpenFolderDialog",true);
        object dialog=Activator.CreateInstance(type);MethodInfo syntax=type.GetMethod("IsCorrectDir",BindingFlags.Instance|BindingFlags.NonPublic);
        string directory=Path.Combine(Path.GetDirectoryName(output),"folder-fixture");Directory.CreateDirectory(directory);
        foreach(var input in new[]{new[]{"local",directory},new[]{"relative","images"},new[]{"unc",@"\\server\images"},new[]{"forward-slash","D:/images"},new[]{"control","D:\\im\tages"},new[]{"unicode","D:\\图像"}})
            records.Add(new {phase="folder-syntax",name=input[0],accepted=syntax.Invoke(dialog,new object[]{input[1]})});
        records.Add(new {phase="folder-write-access",accepted=type.GetMethod("IsHaveRWAccess",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(dialog,new object[]{directory,false}),probeFileRemains=File.Exists(Path.Combine(directory,"test.txt"))});
    }
    static void JpegScenarios(object module,object parameters,string directory,string output) {
        Parameter(parameters,"ImageSourceType",1);Parameter(parameters,"StitchEnable",false);Parameter(parameters,"PathCache",0);Parameter(parameters,"AutoPlay",false);
        foreach(string path in Directory.GetFiles(directory,"*.jpg").OrderBy(p=>p,StringComparer.Ordinal)) {
            Call(module,"ClearAllInputImage");Call(module,"AddInputImageByPath",path);
            string name=Path.GetFileName(path);records.Add(new {phase="jpeg-fixture",name=name,fileSha256=BitConverter.ToString(SHA256.Create().ComputeHash(File.ReadAllBytes(path))).Replace("-","").ToLowerInvariant()});
            Parameter(parameters,"PixelFormat",1);Scenario(module,"jpeg-mono-"+name,output);
            Parameter(parameters,"PixelFormat",2);Scenario(module,"jpeg-rgb-"+name,output);
        }
    }
    static void FormatScenarios(object module,object parameters,string output) {
        Parameter(parameters,"ImageSourceType",1);Parameter(parameters,"StitchEnable",false);Parameter(parameters,"PathCache",0);Parameter(parameters,"AutoPlay",false);
        foreach(string path in ImageSourceCodecFixtures.CreateExtended(Path.Combine(Path.GetDirectoryName(output),"format-fixtures"))) {
            Call(module,"ClearAllInputImage");Call(module,"AddInputImageByPath",path);string name=Path.GetFileName(path);records.Add(new{phase="format-fixture",name=name,fileSha256=BitConverter.ToString(SHA256.Create().ComputeHash(File.ReadAllBytes(path))).Replace("-","").ToLowerInvariant()});
            Parameter(parameters,"PixelFormat",1);Scenario(module,"format-mono-"+name,output);Parameter(parameters,"PixelFormat",2);Scenario(module,"format-rgb-"+name,output);
        }
    }
    static void ClearScenarios(object module,object parameters,string output) {
        string directory=Path.Combine(Path.GetDirectoryName(output),"clear-fixtures");Directory.CreateDirectory(directory);string a=Path.Combine(directory,"a.bmp"),b=Path.Combine(directory,"b.bmp");WriteRows(a,new byte[]{10,20,30,40});WriteRows(b,new byte[]{50,60,70,80});
        Call(module,"ClearAllInputImage");Call(module,"AddInputImageByPath",a);Call(module,"AddInputImageByPath",b);
        foreach(var setting in new[]{new[]{"ImageSourceType","1"},new[]{"PixelFormat","1"},new[]{"AutoPlay","True"},new[]{"StitchEnable","True"},new[]{"StitchHeight","1"},new[]{"StitchStartHeight","0"},new[]{"ClearTrigger","True"},new[]{"PathCache","0"}})Parameter(parameters,setting[0],setting[1]);
        Scenario(module,"clear-missing-input",output);
        foreach(var value in new[]{0,1,1,0,-1}){Call(parameters,"SetInputInt","ClearTriggerValue",new int[]{value});Scenario(module,"clear-value-"+records.Count+"-"+value,output);}
        WriteRows(a,new byte[]{10,20,30,40},5);Parameter(parameters,"AutoPlay",false);StringParameter(module,"CurrentImagePath",a);Call(parameters,"SetInputInt","ClearTriggerValue",new int[]{0});Scenario(module,"clear-width-rejected",output);Call(parameters,"SetInputInt","ClearTriggerValue",new int[]{1});Scenario(module,"clear-width-accepted",output);
    }
    static void RgbScenarios(object module,object parameters,string output) {
        string directory=Path.Combine(Path.GetDirectoryName(output),"rgb-fixtures");string path=ImageSourceCodecFixtures.Create(directory).First(p=>Path.GetFileName(p)=="color.png");
        Call(module,"ClearAllInputImage");Call(module,"AddInputImageByPath",path);foreach(var setting in new[]{new[]{"ImageSourceType","1"},new[]{"PixelFormat","1"},new[]{"AutoPlay","False"},new[]{"StitchEnable","False"},new[]{"PathCache","1"},new[]{"OutMono8","False"}})Parameter(parameters,setting[0],setting[1]);
        Scenario(module,"rgb-cache-admit-mono",output);byte[] encoded=File.ReadAllBytes(path);File.Delete(path);try{Parameter(parameters,"PixelFormat",2);Scenario(module,"rgb-cache-format-change",output);}finally{File.WriteAllBytes(path,encoded);}
        Parameter(parameters,"PathCache",0);Call(module,"ClearAllInputImage");Call(module,"AddInputImageByPath",path);Parameter(parameters,"OutMono8",true);Scenario(module,"rgb-extra-mono",output);
        Parameter(parameters,"StitchEnable",true);Parameter(parameters,"ClearTrigger",false);Parameter(parameters,"StitchHeight",1);Parameter(parameters,"StitchStartHeight",0);Scenario(module,"rgb-stitch-first",output);Scenario(module,"rgb-stitch-second",output);
        Parameter(parameters,"StitchEnable",false);string tiny=Path.Combine(directory,"tiny.bmp");WriteRows(tiny,new byte[]{10,20,30,40},2);Call(module,"ClearAllInputImage");Call(module,"AddInputImageByPath",tiny);Scenario(module,"rgb-small-extra-mono",output);
        string tinyColor=ImageSourceCodecFixtures.CreateExtended(Path.Combine(directory,"tiny-formats")).First(p=>Path.GetFileName(p)=="tiny-color.bmp");Call(module,"ClearAllInputImage");Call(module,"AddInputImageByPath",tinyColor);Scenario(module,"rgb-small-color-extra-mono",output);
    }
    static void InputString(object parameters,string key,string value){
        MethodInfo method=parameters.GetType().GetMethod("SetInputString");Type element=method.GetParameters()[1].ParameterType.GetElementType();object item=Activator.CreateInstance(element);element.GetField("strValue").SetValue(item,value);element.GetField("strReserved").SetValue(item,new byte[32]);Array array=Array.CreateInstance(element,1);array.SetValue(item,0);method.Invoke(parameters,new object[]{key,array});
    }
    static void ControlScenarios(object module,object parameters,string sample,string output){
        Call(module,"ClearAllInputImage");Call(module,"AddInputImageByPath",sample);foreach(var setting in new[]{new[]{"ImageSourceType","1"},new[]{"PixelFormat","1"},new[]{"AutoPlay","False"},new[]{"StitchEnable","False"},new[]{"PathCache","0"},new[]{"TriggerStringEnable","True"},new[]{"AutoStop","True"}})Parameter(parameters,setting[0],setting[1]);
        int index=0;foreach(var pair in new[]{new[]{"go","go"},new[]{"go","stop"},new[]{"","stop"},new[]{"go",""},new[]{"##RunProcedureSilently##","stop"},new[]{"go","##RunProcedureSilently##"},new[]{"GO","go"}}){string phase="filter-"+index++;InputString(parameters,"TriggerString",pair[0]);InputString(parameters,"TriggerFilter",pair[1]);records.Add(new{phase="filter-input",name=phase,trigger=pair[0],filter=pair[1]});Scenario(module,phase,output);}
        Parameter(parameters,"TriggerStringEnable",false);Parameter(parameters,"ImageSourceType",3);Parameter(parameters,"OutMono8",true);Call(module,"SetImagePath",sample);Scenario(module,"sdk-fresh-submission",output);Scenario(module,"sdk-submission-consumed",output);Call(module,"SetImagePath",sample);Scenario(module,"sdk-resubmitted",output);
    }
    static void WatchScenarios(object module,object parameters,string output){
        string directory=Path.Combine(Path.GetDirectoryName(output),"watch-fixtures");Directory.CreateDirectory(directory);string a=Path.Combine(directory,"a2.bmp"),b=Path.Combine(directory,"a10.bmp"),added=Path.Combine(directory,"a1.bmp");WriteRows(a,new byte[]{10,20,30,40});WriteRows(b,new byte[]{50,60,70,80});
        Call(module,"ClearAllInputImage");foreach(var setting in new[]{new[]{"ImageSourceType","1"},new[]{"PixelFormat","1"},new[]{"AutoPlay","False"},new[]{"StitchEnable","False"},new[]{"PathCache","0"},new[]{"IsSubscribeFolderMode","True"},new[]{"RefreshImage","True"},new[]{"TriggerProcessRun","False"}})Parameter(parameters,setting[0],setting[1]);StringParameter(module,"SubscribeFolderPath",directory);System.Threading.Thread.Sleep(200);Scenario(module,"watch-initial",output);
        WriteRows(added,new byte[]{90,100,110,120});System.Threading.Thread.Sleep(200);Scenario(module,"watch-create-preserve-index",output);File.Delete(added);System.Threading.Thread.Sleep(200);Scenario(module,"watch-delete",output);WriteRows(a,new byte[]{77,78,79,80});System.Threading.Thread.Sleep(200);Scenario(module,"watch-modify",output);File.Move(a,Path.Combine(directory,"renamed.bmp"));System.Threading.Thread.Sleep(200);Scenario(module,"watch-rename",output);Parameter(parameters,"IsSubscribeFolderMode",false);
    }
    static void EmbedScenarios(object module,object parameters,string output,bool folder=false){
        string directory=Path.Combine(Path.GetDirectoryName(output),"embed-fixtures");Directory.CreateDirectory(directory);string path=Path.Combine(directory,"owned.bmp");WriteRows(path,new byte[]{10,20,30,40});Call(module,"ClearAllInputImage");Call(module,"AddInputImageByPath",path);
        foreach(var setting in new[]{new[]{"ImageSourceType","1"},new[]{"PixelFormat","1"},new[]{"AutoPlay","False"},new[]{"StitchEnable","False"},new[]{"PathCache","0"},new[]{"SolSaveImageData","True"}})Parameter(parameters,setting[0],setting[1]);if(folder){Parameter(parameters,"IsSubscribeFolderMode",true);StringParameter(module,"SubscribeFolderPath",directory);Call(module,"ClearAllInputImage");}Scenario(module,"embed-before-save",output);
        Type type=Assembly.LoadFrom(Path.Combine(root,"myLibs/VM.Core.dll")).GetType("VM.Core.VmSolution",true);string saved=Path.Combine(Path.GetDirectoryName(output),"owned-embedded.sol");type.GetMethod("SaveAs",new[]{typeof(string),typeof(string)}).Invoke(null,new object[]{saved,null});records.Add(new{phase="embed-save-as",exists=File.Exists(saved),length=new FileInfo(saved).Length,sha256=BitConverter.ToString(SHA256.Create().ComputeHash(File.ReadAllBytes(saved))).Replace("-","").ToLowerInvariant()});File.Delete(path);
        try{type.GetMethod("Load",new[]{typeof(string),typeof(string),typeof(bool)}).Invoke(null,new object[]{saved,"",false});}catch(TargetInvocationException ex){PropertyInfo code=ex.InnerException.GetType().GetProperty("errorCode");if(code==null||Convert.ToInt32(code.GetValue(ex.InnerException,null))!=-536870093)throw;records.Add(new{phase="embed-partial-reload",code=-536870093});}
        object solution=type.GetProperty("Instance").GetValue(null,null);foreach(object info in (IEnumerable)Call(solution,"GetFullModuleList",new object[]{null})){Type infoType=info.GetType();string full=(string)infoType.GetField("FullName").GetValue(info)+"."+(string)infoType.GetField("Name").GetValue(info);object reloaded=type.GetProperty("Item").GetValue(solution,new object[]{full});if(reloaded!=null&&reloaded.GetType().FullName=="ImageSourceModuleCs.ImageSourceModuleTool"){Scenario(reloaded,"embed-after-source-removed",output);break;}}
    }
    static void WriteRows(string path,byte[] rows,int width=4) {
        using(BinaryWriter writer=new BinaryWriter(File.Create(path))) {
            int stride=(width+3)&~3,length=rows.Length*stride;writer.Write((ushort)0x4d42);writer.Write(1078+length);writer.Write(0);writer.Write(1078);
            writer.Write(40);writer.Write(width);writer.Write(rows.Length);writer.Write((ushort)1);writer.Write((ushort)8);writer.Write(0);writer.Write(length);writer.Write(0);writer.Write(0);writer.Write(256);writer.Write(0);
            for(int i=0;i<256;i++){writer.Write((byte)i);writer.Write((byte)i);writer.Write((byte)i);writer.Write((byte)0);}
            for(int y=rows.Length-1;y>=0;y--)for(int x=0;x<stride;x++)writer.Write(x<width?rows[y]:(byte)0);
        }
    }
    static IEnumerable<DependencyObject> Descendants(DependencyObject parent) {
        for(int i=0;i<VisualTreeHelper.GetChildrenCount(parent);i++) {
            DependencyObject child=VisualTreeHelper.GetChild(parent,i);yield return child;
            foreach(DependencyObject inner in Descendants(child))yield return inner;
        }
    }
    static void CaptureModes(object module,object parameters,string output,bool desktop=false) {
        object moduleParameters=Get(module,"Params");
        string installedXml=Path.Combine(root,"Module(sp)/x64/Collection/ImageSourceModule/ImageSourceModuleAlgorithmTab.xml");
        // Vendor loadXml changes file attributes before reading. Use an exact
        // byte copy in this probe's writable output, never mutate installed evidence.
        string localXml=Path.Combine(Path.GetDirectoryName(output),Path.GetFileName(installedXml));
        File.Copy(installedXml,localXml,true);File.SetAttributes(localXml,FileAttributes.Normal);
        Assembly.LoadFrom(Path.Combine(root,"Module(sp)/x64/Collection/ImageSourceModule/ImageSourceModuleControl.dll"));
        records.Add(new {phase="sdk-ui-path",original=Get(moduleParameters,"XmlPath"),supplied=localXml,installed=installedXml,xmlSha256=BitConverter.ToString(SHA256.Create().ComputeHash(File.ReadAllBytes(localXml))).Replace("-","").ToLowerInvariant()});
        moduleParameters.GetType().GetProperty("XmlPath").SetValue(moduleParameters,localXml,null);
        // Rebuild the cached model so diagnostics describe this exact XML.
        moduleParameters.GetType().GetProperty("Data").SetValue(moduleParameters,null,null);
        AppDomain.CurrentDomain.FirstChanceException += ObserveUiException;
        Type type=Assembly.LoadFrom(Path.Combine(root,"myLibs/VMControls.WPF.dll")).GetType("VMControls.WPF.ParamsConfigControlBase",true);
        Window ownedHost=null;if(desktop){ownedHost=new Window{Width=800,Height=600,Left=-30000,Top=-30000,ShowActivated=false,ShowInTaskbar=false};Application.Current.MainWindow=ownedHost;ownedHost.Show();}
        FrameworkElement control;Window window;
        if(desktop){window=(Window)Activator.CreateInstance(type.Assembly.GetType("VMControls.WPF.ParamsTabControlBase",true));window.Owner=ownedHost;window.Left=-30000;window.Top=-30000;window.ShowActivated=false;window.Show();control=(FrameworkElement)window.FindName("paramstab");}
        else{control=(FrameworkElement)Activator.CreateInstance(type);window=new Window{Width=420,Height=700,Left=-30000,Top=-30000,ShowActivated=false,Content=control};window.Show();}
        type.GetProperty("ParamsConfig").SetValue(control,moduleParameters,null);
        try {
            int capture=0;
            foreach(int mode in new[]{1,2,3,1,1}) {
                capture++;
                string variant=capture<=3?"mode-"+mode:capture==4?"local-expanded":"local-rgb24";
                if(capture==4)foreach(string key in new[]{"AutoPlay","StitchEnable","ClearTrigger","TriggerStringEnable","IsSubscribeFolderMode"})Parameter(parameters,key,true);
                if(capture==5)Parameter(parameters,"PixelFormat",2);
                Parameter(parameters,"ImageSourceType",mode);Call(control,"UpdateAllItem");
                window.Dispatcher.Invoke(delegate{},System.Windows.Threading.DispatcherPriority.ApplicationIdle);window.UpdateLayout();
                var items=((IEnumerable)Call(control.DataContext,"GetAllItems")).Cast<object>().Select(item=>new {type=item.GetType().FullName,fields=item.GetType().GetFields().Where(f=>new[]{"Name","CustomVisible","IsVisible","CurValue","DisplayName"}.Contains(f.Name)).ToDictionary(f=>f.Name,f=>Convert.ToString(f.GetValue(item))),properties=item.GetType().GetProperties().Where(p=>p.GetIndexParameters().Length==0&&new[]{"Name","CustomVisible","IsVisible","CurValue","Value","DisplayName"}.Contains(p.Name)).ToDictionary(p=>p.Name,p=>Convert.ToString(p.GetValue(item,null)))}).ToArray();
                records.Add(new {phase="sdk-ui-model",mode=mode,variant=variant,items=items});
                records.Add(new {phase="sdk-ui-diagnostic",mode=mode,xmlExists=File.Exists(installedXml),context=control.DataContext==null?null:control.DataContext.GetType().FullName,width=control.ActualWidth,height=control.ActualHeight,visuals=Descendants(control).OfType<FrameworkElement>().Select(e=>new {type=e.GetType().FullName,width=e.ActualWidth,height=e.ActualHeight,context=e.DataContext==null?null:e.DataContext.GetType().FullName}).Take(35).ToArray(),exceptions=uiErrors.Distinct().Take(30).ToArray()});
                var nodes=Descendants(control).OfType<FrameworkElement>().Where(e=>e.DataContext!=null&&e.DataContext.GetType().GetProperty("Name")!=null).Select(e=>new { type=e.GetType().FullName,name=Convert.ToString(e.DataContext.GetType().GetProperty("Name").GetValue(e.DataContext,null)),visible=e.IsVisible,visibility=e.Visibility.ToString() }).Distinct().ToArray();
                RenderTargetBitmap bitmap=new RenderTargetBitmap((int)Math.Ceiling(window.ActualWidth),(int)Math.Ceiling(window.ActualHeight),96,96,PixelFormats.Pbgra32);bitmap.Render(window);
                PngBitmapEncoder encoder=new PngBitmapEncoder();encoder.Frames.Add(BitmapFrame.Create(bitmap));
                string imagePath=Path.Combine(Path.GetDirectoryName(output),"original-sdk-ui-"+variant+".png");
                using(FileStream file=File.Create(imagePath))encoder.Save(file);
                if(desktop&&mode==2){
                    object model=control.DataContext;model.GetType().GetProperty("IsSubscribeInWindow").SetValue(model,true,null);object item=Call(model,"FindBaseNodeByName","ExposureTimeInput");var command=(System.Windows.Input.ICommand)Get(model,"ChangeSelectedSubParamCommand");command.Execute(item);window.Dispatcher.Invoke(delegate{},System.Windows.Threading.DispatcherPriority.ApplicationIdle);
                    foreach(Window popup in Application.Current.Windows.Cast<Window>().Where(w=>w!=window&&w.GetType().Name=="ParamSubscribeWindow").ToArray()){popup.Left=-30000;popup.Top=-30000;popup.UpdateLayout();var popupBitmap=new RenderTargetBitmap((int)Math.Ceiling(popup.ActualWidth),(int)Math.Ceiling(popup.ActualHeight),96,96,PixelFormats.Pbgra32);popupBitmap.Render(popup);var popupEncoder=new PngBitmapEncoder();popupEncoder.Frames.Add(BitmapFrame.Create(popupBitmap));string popupPath=Path.Combine(Path.GetDirectoryName(output),"original-subscription-exposure.png");using(var file=File.Create(popupPath))popupEncoder.Save(file);records.Add(new{phase="original-subscription-window",type=popup.GetType().FullName,width=popup.ActualWidth,height=popup.ActualHeight,imageSha256=BitConverter.ToString(SHA256.Create().ComputeHash(File.ReadAllBytes(popupPath))).Replace("-","").ToLowerInvariant(),texts=Descendants(popup).OfType<System.Windows.Controls.TextBlock>().Select(t=>t.Text).Where(t=>!String.IsNullOrEmpty(t)).Distinct().ToArray()});popup.Close();}
                }
                records.Add(new {phase="sdk-hosted-ui",mode=mode,variant=variant,nodes=nodes,accepted=nodes.Length>0&&items.Length>0,imageSha256=BitConverter.ToString(SHA256.Create().ComputeHash(File.ReadAllBytes(imagePath))).Replace("-","").ToLowerInvariant(),windowType=window.GetType().FullName,windowWidth=window.ActualWidth,windowHeight=window.ActualHeight,limitation=desktop?"Original ParamsTabControlBase window hosted by SDK probe with byte-identical XML. SDK backend retained; not the installed desktop application process.":"Original SDK embedded control with byte-identical writable XML copy; not the desktop debug window. Offscreen WPF rendering; full expanded content can exceed viewport."});
                if(capture>=4) {
                    window.Height=1600;window.UpdateLayout();
                    RenderTargetBitmap full=new RenderTargetBitmap(420,1600,96,96,PixelFormats.Pbgra32);full.Render(window);
                    PngBitmapEncoder fullEncoder=new PngBitmapEncoder();fullEncoder.Frames.Add(BitmapFrame.Create(full));
                    using(FileStream file=File.Create(Path.Combine(Path.GetDirectoryName(output),"original-sdk-ui-"+variant+"-tall.png")))fullEncoder.Save(file);
                    window.Height=700;window.UpdateLayout();
                }
                File.WriteAllText(output,new JavaScriptSerializer{MaxJsonLength=8388608}.Serialize(records));
            }
        }finally{AppDomain.CurrentDomain.FirstChanceException -= ObserveUiException;window.Close();if(control is IDisposable)((IDisposable)control).Dispose();if(ownedHost!=null)ownedHost.Close();}
    }
    static void ObserveUiException(object sender,System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs args) {
        if(uiErrors.Count<100)uiErrors.Add(args.Exception.ToString());
    }
    [STAThread] public static int Main(string[] args)
    {
        root = args[0]; string output = args[3]; string phase = "resolve";
        if(args.Length>4&&(args[4]=="ui"||args[4]=="desktop-ui")&&Application.Current==null)new Application { ShutdownMode=ShutdownMode.OnExplicitShutdown };
        AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs request) {
            string file = new AssemblyName(request.Name).Name + ".dll";
            foreach (string dir in new[] { "myLibs", "PublicFile/x64", "Module(sp)/x64/Collection/ImageSourceModule", ".CommonControl" }) {
                string path = Path.Combine(root, dir, file); if (File.Exists(path)) return Assembly.LoadFrom(path);
            }
            return null;
        };
        object solution = null;
        try {
            SetDllDirectory(Path.Combine(root, "PublicFile/x64"));
            Type type = Assembly.LoadFrom(Path.Combine(root, "myLibs/VM.Core.dll")).GetType("VM.Core.VmSolution", true);
            phase = "load-read-only-sample";
            try {
                type.GetMethod("Load", new[] { typeof(string), typeof(string), typeof(bool) }).Invoke(null, new object[] { args[1], "", false });
            } catch(TargetInvocationException loadError) {
                Exception cause=loadError.InnerException;
                PropertyInfo errorCode=cause.GetType().GetProperty("errorCode");
                if(errorCode==null||Convert.ToInt32(errorCode.GetValue(cause,null))!=-536870093)throw;
                records.Add(new { phase="partial-solution-load", code=-536870093, limitation="some sample modules failed to load; only a present ImageSource module will be probed" });
            }
            solution = type.GetProperty("Instance").GetValue(null, null);
            phase = "find-image-source";
            foreach (object info in (IEnumerable)Call(solution, "GetFullModuleList", new object[] { null })) {
                Type infoType = info.GetType();
                string full = (string)infoType.GetField("FullName").GetValue(info) + "." + (string)infoType.GetField("Name").GetValue(info);
                object module = type.GetProperty("Item").GetValue(solution, new object[] { full });
                if (module == null || module.GetType().FullName != "ImageSourceModuleCs.ImageSourceModuleTool") continue;
                records.Add(new { phase="module", name=full });
                phase = "configure";
                Call(module, "ClearAllInputImage");
                Call(module, "AddInputImageByPath", args[2]);
                object parameters = Get(module,"ModuParams");
                foreach (var setting in new[] { new[]{"ImageSourceType","1"},new[]{"InitialSN","7"},new[]{"AutoPlay","True"},new[]{"AutoStop","False"},new[]{"StitchEnable","False"},new[]{"PixelFormat","1"} }) {
                    Parameter(parameters,setting[0],setting[1]);
                }
                for (int i=0;i<3;i++) {
                    phase="run-"+i;Snapshot(module,phase,output);
                }
                Parameter(parameters,"ShowImageName",false);Snapshot(module,"display-change",output);
                Scenarios(module,parameters,args[2],output);
                if(args.Length>4&&args[4]=="stitch")StitchScenarios(module,parameters,output);
                if(args.Length>4&&args[4]=="codecs")CodecScenarios(module,parameters,output);
                if(args.Length>4&&args[4]=="folder")FolderScenarios(output);
                if(args.Length>4&&args[4]=="catalog")FolderCatalogScenarios(output);
                if(args.Length>4&&args[4]=="formats")FormatScenarios(module,parameters,output);
                if(args.Length>4&&args[4]=="clear")ClearScenarios(module,parameters,output);
                if(args.Length>4&&args[4]=="rgb")RgbScenarios(module,parameters,output);
                if(args.Length>4&&args[4]=="control")ControlScenarios(module,parameters,args[2],output);
                if(args.Length>4&&args[4]=="watch")WatchScenarios(module,parameters,output);
                if(args.Length>4&&args[4]=="embed")EmbedScenarios(module,parameters,output);
                if(args.Length>4&&args[4]=="embed-folder")EmbedScenarios(module,parameters,output,true);
                if(args.Length>4&&args[4]=="camera")CameraScenarios(solution,module,parameters,output);
                if(args.Length>5&&args[4]=="jpeg")JpegScenarios(module,parameters,args[5],output);
                if(args.Length>4&&args[4]=="ui") { phase="capture-sdk-ui"; CaptureModes(module,parameters,output); }
                if(args.Length>4&&args[4]=="desktop-ui") { phase="capture-desktop-window"; CaptureModes(module,parameters,output,true); }
                break;
            }
            records.Add(new { phase="finished", originalRuntimeEquivalence=false, ranImageSource=records.Any(r=>r.GetType().GetProperty("sn")!=null) });
        } catch(Exception ex) {
            while(ex.InnerException!=null)ex=ex.InnerException;
            PropertyInfo code=ex.GetType().GetProperty("errorCode");
            PropertyInfo detail=ex.GetType().GetProperty("errorMessage");
            records.Add(new { phase=phase, error=ex.GetType().FullName, message=ex.Message, code=code==null?null:Convert.ToString(code.GetValue(ex,null)), detail=detail==null?null:Convert.ToString(detail.GetValue(ex,null)), originalRuntimeEquivalence=false });
        } finally {
            if(solution!=null) { try { Call(solution,"Dispose"); } catch(Exception ex) { records.Add(new { phase="dispose",error=ex.GetType().FullName }); } }
            File.WriteAllText(output,new JavaScriptSerializer().Serialize(records));
        }
        int exitCode=records.Any(r=>r.GetType().GetProperty("error")!=null)?1:0;
        // Failed native SDK startup may leave foreground callback threads alive.
        // This terminates only this dedicated probe process, never a VM desktop/server.
        Environment.Exit(exitCode);
        return exitCode;
    }
}
