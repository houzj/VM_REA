// Dedicated original SDK host. Loads the sample read-only and never saves it.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
public static class OriginalNPointCalibrationProbe {
 [DllImport("kernel32.dll",CharSet=CharSet.Unicode)] static extern bool SetDllDirectory(string path);
 [DllImport("kernel32.dll",CharSet=CharSet.Unicode)] static extern IntPtr LoadLibrary(string path);
 [UnmanagedFunctionPointer(CallingConvention.StdCall)] delegate int CreateTool(out IntPtr tool);
 [UnmanagedFunctionPointer(CallingConvention.StdCall)] delegate int DestroyTool(IntPtr tool);
 [DllImport("kernel32.dll",CharSet=CharSet.Ansi)] static extern IntPtr GetProcAddress(IntPtr module,string name);
 static string root; static List<object> rows=new List<object>();
 static object Get(object o,string n){return o.GetType().GetProperty(n).GetValue(o,null);}
 static object Call(object o,string n,params object[] a){return o.GetType().GetMethods().Where(m=>m.Name==n&&m.GetParameters().Length==a.Length).OrderBy(m=>m.DeclaringType==o.GetType()?0:1).First().Invoke(o,a);}
 static void Set(object o,string n,object v){var p=o.GetType().GetProperty(n);p.SetValue(o,p.PropertyType.IsEnum?Enum.ToObject(p.PropertyType,v):Convert.ChangeType(v,p.PropertyType),null);}
 static void Text(object module,string key,string value){var p=Get(module,"Params");p.GetType().GetProperty("Item").SetValue(p,value,new object[]{key});}
 static void Attempt(string phase,Action run){try{run();}catch(Exception e){while(e.InnerException!=null)e=e.InnerException;var p=e.GetType().GetProperty("errorCode");rows.Add(new{phase,error=e.GetType().FullName,message=e.Message,code=p==null?null:Convert.ToString(p.GetValue(e,null))});}}
 static string output;
 static Dictionary<string,object> Snapshot(object o){var d=new Dictionary<string,object>();foreach(var p in o.GetType().GetProperties().Where(x=>x.GetIndexParameters().Length==0)){try{var v=p.GetValue(o,null);if(v==null||v is string||v.GetType().IsPrimitive||v.GetType().IsEnum)d[p.Name]=v;else if(v.GetType().Name=="PointF")d[p.Name]=new{X=Get(v,"X"),Y=Get(v,"Y")};}catch{}}return d;}
 static void Run(object m,string phase){Attempt(phase,delegate{Call(m,"Run");rows.Add(new{phase,parameters=Snapshot(Get(m,"ModuParams")),result=Snapshot(Get(m,"ModuResult"))});});}
 static void Ui(object module,object parameters,string output){
  if(Application.Current==null)new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};

  var parser=Assembly.LoadFrom(Path.Combine(root,"myLibs/Apps.XmlParser.dll"));var groups=parser.GetType("Apps.XmlParser.ParamTab.GroupsRoot",true);var groupRoot=groups.GetMethod("GetInstance").Invoke(null,null);var xml=new System.Xml.XmlDocument();xml.Load(Path.Combine(root,"Module(sp)/Group.xml"));Call(groupRoot,"Init",xml.DocumentElement);
  var model=Get(module,"Params");string source=Path.Combine(root,"Module(sp)/x64/Calibration/IMVSNPointCalibModu/IMVSNPointCalibModuAlgorithmTab.xml"),copy=Path.Combine(output,Path.GetFileName(source));File.Copy(source,copy,true);File.SetAttributes(copy,FileAttributes.Normal);model.GetType().GetProperty("XmlPath").SetValue(model,copy,null);model.GetType().GetProperty("Data").SetValue(model,null,null);
  var t=Assembly.LoadFrom(Path.Combine(root,"myLibs/VMControls.WPF.dll")).GetType("VMControls.WPF.ParamsConfigControlBase",true);var control=(FrameworkElement)Activator.CreateInstance(t);var window=new Window{Width=420,Height=1500,Left=-30000,Top=-30000,ShowActivated=false,ShowInTaskbar=false,Content=control};window.Show();t.GetProperty("ParamsConfig").SetValue(control,model,null);
  try{Call(control,"UpdateAllItem");window.Dispatcher.Invoke(delegate{},System.Windows.Threading.DispatcherPriority.ApplicationIdle);window.UpdateLayout();var bitmap=new RenderTargetBitmap(420,1500,96,96,PixelFormats.Pbgra32);bitmap.Render(window);var encoder=new PngBitmapEncoder();encoder.Frames.Add(BitmapFrame.Create(bitmap));using(var file=File.Create(Path.Combine(output,"original-sdk-ui.png")))encoder.Save(file);var items=((System.Collections.IEnumerable)Call(control.DataContext,"GetAllItems")).Cast<object>().Select(item=>new{type=item.GetType().FullName,properties=item.GetType().GetProperties().Where(p=>p.GetIndexParameters().Length==0&&new[]{"Name","CustomVisible","IsVisible","Value","DisplayName","CurValue","Description"}.Contains(p.Name)).ToDictionary(p=>p.Name,p=>Convert.ToString(p.GetValue(item,null)))}).ToArray();rows.Add(new{phase="sdk-ui",items,limitation="Original embedded control in owned SDK host; not installed desktop window"});}  finally{window.Close();if(control is IDisposable)((IDisposable)control).Dispose();}
 }
 [STAThread] public static int Main(string[] args){root=args[0];output=args[2];object solution=null;
 AppDomain.CurrentDomain.AssemblyResolve+=(s,a)=>{string file=new AssemblyName(a.Name).Name+".dll";foreach(string dir in new[]{"myLibs","PublicFile/x64","Module(sp)/x64/Calibration/IMVSNPointCalibModu",".CommonControl"}){string p=Path.Combine(root,dir,file);if(File.Exists(p))return Assembly.LoadFrom(p);}return null;};
 try{SetDllDirectory(Path.Combine(root,"PublicFile/x64"));string algo=Path.GetFullPath(Path.Combine(root,"../MVDAlgorithmSDK/Runtime/x64/MVDNPointCalibCpp.dll"));IntPtr lib=LoadLibrary(algo),preloaded=IntPtr.Zero;int precode=((CreateTool)Marshal.GetDelegateForFunctionPointer(GetProcAddress(lib,"CreateNPointCalibToolInstance"),typeof(CreateTool)))(out preloaded);rows.Add(new{phase="preload-algorithm",code=precode,created=preloaded!=IntPtr.Zero});var core=Assembly.LoadFrom(Path.Combine(root,"myLibs/VM.Core.dll"));var st=core.GetType("VM.Core.VmSolution",true);Attempt("load",()=>st.GetMethod("Load",new[]{typeof(string),typeof(string),typeof(bool)}).Invoke(null,new object[]{args[1],"",false}));solution=st.GetProperty("Instance").GetValue(null,null);var proc=Activator.CreateInstance(core.GetType("VM.Core.VmProcedure",true),new[]{solution});var mt=Assembly.LoadFrom(Path.Combine(root,"Module(sp)/x64/Calibration/IMVSNPointCalibModu/IMVSNPointCalibModuCs.dll")).GetType("IMVSNPointCalibModuCs.IMVSNPointCalibModuTool",true);var m=Activator.CreateInstance(mt,new[]{proc});Run(m,"default");Attempt("sdk-ui",()=>Ui(m,Get(m,"ModuParams"),output));
 foreach(int freedom in new[]{1,2,3})foreach(int camera in new[]{1,2,3}){var n=Activator.CreateInstance(mt,new[]{proc});Attempt("manual-set",delegate{Text(n,"CalibPointGet","1");Text(n,"CalibPointTotalNum","9");Text(n,"HomoFreedom",freedom.ToString());Text(n,"CameraMode",camera.ToString());Text(n,"WeightFun","1");for(int i=0;i<9;i++){double x=(i%3)*10,y=(i/3)*10;Text(n,"ImagePointX"+i,x.ToString());Text(n,"ImagePointY"+i,y.ToString());Text(n,"WorldPointX"+i,(2*x+10).ToString());Text(n,"WorldPointY"+i,(2*y-5).ToString());}Run(n,"manual-"+freedom+"-"+camera);Text(n,"SaveCalibPath",Path.Combine(output,"manual-"+freedom+"-"+camera+".xml"));});Call(n,"Dispose");}
 Call(m,"Dispose");if(preloaded!=IntPtr.Zero)((DestroyTool)Marshal.GetDelegateForFunctionPointer(GetProcAddress(lib,"DestroyNPointCalibToolInstance"),typeof(DestroyTool)))(preloaded);rows.Add(new{phase="finished",devicesTested=false,parityEstablished=false});
 }catch(Exception e){while(e.InnerException!=null)e=e.InnerException;rows.Add(new{phase="fatal",message=e.Message,stack=e.StackTrace});}finally{rows.Add(new{phase="loaded-native",modules=System.Diagnostics.Process.GetCurrentProcess().Modules.Cast<System.Diagnostics.ProcessModule>().Where(x=>x.ModuleName.StartsWith("MVD")).Select(x=>new{path=x.FileName}).ToArray()});if(solution!=null)Attempt("dispose",()=>Call(solution,"Dispose"));File.WriteAllText(Path.Combine(output,"probe.json"),new JavaScriptSerializer{MaxJsonLength=20000000}.Serialize(rows));}Environment.Exit(0);return 0;}
}
