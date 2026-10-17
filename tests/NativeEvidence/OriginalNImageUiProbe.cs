// Original controls/XML with an explicitly synthetic parameter model. No native module is created.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using VMControls.BaseInterface;

public class SyntheticNImageParams : IParamsConfig {
 public Dictionary<string,string> Values=new Dictionary<string,string>();
 public string XmlPath {get;set;} public uint TimeOut {get;set;}
 string IUserStringData.this[string key] {get {string value;return Values.TryGetValue(key,out value)?value:"";} set{Values[key]=value;}}
 byte[] IUserBytesData.this[string key] {get{return new byte[0];} set{}}
}
public class SyntheticNImageOwner : RealProxy {
 readonly SyntheticNImageParams parameters;
 public SyntheticNImageOwner(Type type,SyntheticNImageParams model):base(type){parameters=model;}
 public override IMessage Invoke(IMessage message){var call=(IMethodCallMessage)message;object value=null;var method=(MethodInfo)call.MethodBase;
  if(call.MethodName=="get_Params")value=parameters;
  else if(call.MethodName=="get_ModuleTypeName")value="IMVSNImageCalibModu";
  else if(method.ReturnType==typeof(string))value="";
  else if(method.ReturnType.IsValueType)value=Activator.CreateInstance(method.ReturnType);
  return new ReturnMessage(value,null,0,call.LogicalCallContext,call);
 }
}
public static class OriginalNImageUiProbe {
 static string root,output;static List<object> rows=new List<object>();
 static object Get(object o,string name){return o.GetType().GetProperty(name).GetValue(o,null);}
 static object Call(object o,string name,params object[] args){return o.GetType().GetMethods().First(m=>m.Name==name&&m.GetParameters().Length==args.Length).Invoke(o,args);}
 static void Attempt(string phase,Action action){try{action();}catch(Exception e){File.AppendAllText(Path.Combine(output,"exceptions.log"),e.ToString()+Environment.NewLine);while(e.InnerException!=null)e=e.InnerException;rows.Add(new{phase,error=e.GetType().FullName,message=e.Message,stack=e.StackTrace});}}
 static object[] Items(object model){return ((System.Collections.IEnumerable)Call(model,"GetAllItems")).Cast<object>().ToArray();}
 static object Find(object model,string name){return Call(model,"FindBaseNodeByName",name);}
 static void SetCommand(object model,string name,object value){var item=Find(model,name);string command="ValueChangedCommand";if(item.GetType().Name=="Enumeration"||item.GetType().Name=="Boolean"||item.GetType().Name=="Integer"||item.GetType().Name=="Float"){item.GetType().GetProperty("CurValueA").SetValue(item,value,null);return;}((System.Windows.Input.ICommand)Get(item,command)).Execute(value);}
 static void Capture(Window window,object model,string phase){if(window!=null){window.Dispatcher.Invoke(delegate{},System.Windows.Threading.DispatcherPriority.ApplicationIdle);window.UpdateLayout();}var items=Items(model).Select(item=>{var values=new Dictionary<string,string>();foreach(string key in new[]{"NameA","DisplayNameA","CurValueA","CustomVisibleA","SubscribeVisibleA","MinValueA","MaxValueA","IsVisible"}){var p=item.GetType().GetProperty(key);if(p!=null)try{values[key]=Convert.ToString(p.GetValue(item,null));}catch{}}
  var entries=new List<object>();var en=item.GetType().GetProperty("EnumEntrysA");if(en!=null)try{foreach(object entry in (System.Collections.IEnumerable)en.GetValue(item,null)){var props=new Dictionary<string,string>();foreach(string key in new[]{"NameA","DisplayNameA","ValueA"}){var p=entry.GetType().GetProperty(key);if(p!=null)props[key]=Convert.ToString(p.GetValue(entry,null));}entries.Add(props);}}catch{}return new{type=item.GetType().FullName,values,entries};}).ToArray();
  string name=null;if(window!=null){var bmp=new RenderTargetBitmap((int)window.ActualWidth,(int)window.ActualHeight,96,96,PixelFormats.Pbgra32);bmp.Render(window);var encoder=new PngBitmapEncoder();encoder.Frames.Add(BitmapFrame.Create(bmp));name=phase+".png";using(var file=File.Create(Path.Combine(output,name)))encoder.Save(file);}var categories=new List<object>();foreach(object tab in (System.Collections.IEnumerable)model.GetType().GetField("Tabs").GetValue(model)){foreach(object category in (System.Collections.IEnumerable)Get(tab,"CategorysA")){categories.Add(new{name=Convert.ToString(category.GetType().GetField("Name").GetValue(category)),visible=Get(category,"CustomVisibleA")});}}rows.Add(new{phase,image=name,items,categories,authority="original-parser/synthetic-model",nativeParameters=false,desktopOperation=false});File.WriteAllText(Path.Combine(output,"probe.json"),new JavaScriptSerializer{MaxJsonLength=int.MaxValue}.Serialize(rows));
 }
 [STAThread] public static int Main(string[] args){root=args[0];output=args[1];Directory.CreateDirectory(output);AppDomain.CurrentDomain.FirstChanceException+=(s,e)=>{if(e.Exception is FileNotFoundException || e.Exception is TypeLoadException || e.Exception is NotImplementedException)File.AppendAllText(Path.Combine(output,"first-chance.log"),e.Exception.ToString()+Environment.NewLine);};AppDomain.CurrentDomain.AssemblyResolve+=(s,a)=>{string file=new AssemblyName(a.Name).Name+".dll";foreach(string dir in new[]{"myLibs","PublicFile/x64","Module(sp)/x64/Calibration/IMVSNImageCalibModu",".CommonControl","3rdLib/MVVM"}){string p=Path.Combine(root,dir,file);if(File.Exists(p))return Assembly.LoadFrom(p);}return null;};
 try{new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};var parser=Assembly.LoadFrom(Path.Combine(root,"myLibs/Apps.XmlParser.dll"));var original=Assembly.LoadFrom(Path.Combine(root,"Module(sp)/x64/Calibration/IMVSNImageCalibModu/IMVSNImageCalibModuControl.dll"));var activator=Assembly.LoadFrom(Path.Combine(root,"myLibs/VM.Util.dll")).GetType("Hik.Util.ActivatorWrap",true);foreach(string library in new[]{parser.Location,original.Location})activator.GetMethod("LoadAssembly",new[]{typeof(string),typeof(bool)}).Invoke(null,new object[]{library,true});var groups=parser.GetType("Apps.XmlParser.ParamTab.GroupsRoot",true);var groupXml=new XmlDocument();groupXml.Load(Path.Combine(root,"Module(sp)/Group.xml"));if(groups.GetField("instance").GetValue(null)==null)Call(groups.GetMethod("GetInstance").Invoke(null,null),"Init",groupXml.DocumentElement);
  var parameters=new SyntheticNImageParams{XmlPath=Path.Combine(root,"Module(sp)/x64/Calibration/IMVSNImageCalibModu/IMVSNImageCalibModuAlgorithmTab.xml")};var source=new XmlDocument();source.Load(parameters.XmlPath);string localXml=Path.Combine(output,"IMVSNImageCalibModuAlgorithmTab.xml");File.Copy(parameters.XmlPath,localXml,true);parameters.XmlPath=localXml;foreach(XmlElement node in source.SelectNodes("//*[@Name]")){var cur=node.SelectSingleNode("CurValue");if(cur!=null)parameters.Values[node.GetAttribute("Name")]=cur.InnerText;}
  var iface=Assembly.LoadFrom(Path.Combine(root,"myLibs/VMControls.Interface.dll")).GetTypes().First(t=>t.Name=="IVmModule");var owner=new SyntheticNImageOwner(iface,parameters).GetTransparentProxy();var model=Activator.CreateInstance(parser.GetType("Apps.XmlParser.ParamTab.AlgorithmTabRoot",true),new object[]{parameters.XmlPath,owner,parameters});rows.Add(new{phase="model-created",itemCount=Items(model).Length,defaults=new Dictionary<string,string>(parameters.Values),authority="synthetic-inputs"});Attempt("initialize",()=>Call(model,"DoIniter"));Attempt("read-values",()=>Call(model,"GetItemsValueUnit"));
  Window window=null;
  Attempt("default",()=>Capture(window,model,"default"));
  foreach(int mode in new[]{1,0})Attempt("motion-input-"+mode,()=>{SetCommand(model,"CalibPointInput",mode);Capture(null,model,"motion-input-"+mode);});
  foreach(bool fix in new[]{true,false})Attempt("fixture-"+fix,()=>{SetCommand(model,"HomoFixEnable",fix);Capture(null,model,"fixture-"+fix);});
  SetCommand(model,"HomoFixEnable",true);foreach(int mode in new[]{1,0})Attempt("fixture-input-"+mode,()=>{SetCommand(model,"InputMode",mode);Capture(null,model,"fixture-input-"+mode);});
  foreach(int count in new[]{0,3})Attempt("rotation-count-"+count,()=>{SetCommand(model,"RotPointTotalNum",count);Capture(null,model,"rotation-count-"+count);});
  foreach(int board in new[]{6,7,8,9})Attempt("board-"+board,()=>{SetCommand(model,"CalibBoardType",board);Capture(null,model,"board-"+board);});

 }catch(Exception e){File.AppendAllText(Path.Combine(output,"exceptions.log"),e.ToString()+Environment.NewLine);while(e.InnerException!=null)e=e.InnerException;rows.Add(new{phase="fatal",error=e.GetType().FullName,message=e.Message,stack=e.StackTrace});}
 finally{File.WriteAllText(Path.Combine(output,"probe.json"),new JavaScriptSerializer{MaxJsonLength=int.MaxValue}.Serialize(rows));}return 0;
 }
}
