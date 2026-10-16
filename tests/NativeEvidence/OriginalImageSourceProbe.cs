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
    static string root;
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
    static IEnumerable<DependencyObject> Descendants(DependencyObject parent) {
        for(int i=0;i<VisualTreeHelper.GetChildrenCount(parent);i++) {
            DependencyObject child=VisualTreeHelper.GetChild(parent,i);yield return child;
            foreach(DependencyObject inner in Descendants(child))yield return inner;
        }
    }
    static void CaptureModes(object module,object parameters,string output) {
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
        FrameworkElement control=(FrameworkElement)Activator.CreateInstance(type);
        type.GetProperty("ParamsConfig").SetValue(control,moduleParameters,null);
        Window window=new Window{Width=420,Height=700,Left=-30000,Top=-30000,ShowActivated=false,Content=control};
        window.Show();
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
                RenderTargetBitmap bitmap=new RenderTargetBitmap(420,700,96,96,PixelFormats.Pbgra32);bitmap.Render(window);
                PngBitmapEncoder encoder=new PngBitmapEncoder();encoder.Frames.Add(BitmapFrame.Create(bitmap));
                string imagePath=Path.Combine(Path.GetDirectoryName(output),"original-sdk-ui-"+variant+".png");
                using(FileStream file=File.Create(imagePath))encoder.Save(file);
                records.Add(new {phase="sdk-hosted-ui",mode=mode,variant=variant,nodes=nodes,accepted=nodes.Length>0&&items.Length>0,imageSha256=BitConverter.ToString(SHA256.Create().ComputeHash(File.ReadAllBytes(imagePath))).Replace("-","").ToLowerInvariant(),limitation="Original SDK embedded control with byte-identical writable XML copy; not the desktop debug window. Offscreen WPF rendering; full expanded content can exceed viewport."});
                if(capture>=4) {
                    window.Height=1600;window.UpdateLayout();
                    RenderTargetBitmap full=new RenderTargetBitmap(420,1600,96,96,PixelFormats.Pbgra32);full.Render(window);
                    PngBitmapEncoder fullEncoder=new PngBitmapEncoder();fullEncoder.Frames.Add(BitmapFrame.Create(full));
                    using(FileStream file=File.Create(Path.Combine(Path.GetDirectoryName(output),"original-sdk-ui-"+variant+"-tall.png")))fullEncoder.Save(file);
                    window.Height=700;window.UpdateLayout();
                }
                File.WriteAllText(output,new JavaScriptSerializer{MaxJsonLength=8388608}.Serialize(records));
            }
        }finally{AppDomain.CurrentDomain.FirstChanceException -= ObserveUiException;window.Close();if(control is IDisposable)((IDisposable)control).Dispose();}
    }
    static void ObserveUiException(object sender,System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs args) {
        if(uiErrors.Count<100)uiErrors.Add(args.Exception.ToString());
    }
    [STAThread] public static int Main(string[] args)
    {
        root = args[0]; string output = args[3]; string phase = "resolve";
        if(args.Length>4&&args[4]=="ui"&&Application.Current==null)new Application { ShutdownMode=ShutdownMode.OnExplicitShutdown };
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
                if(args.Length>4&&args[4]=="ui") { phase="capture-sdk-ui"; CaptureModes(module,parameters,output); }
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
