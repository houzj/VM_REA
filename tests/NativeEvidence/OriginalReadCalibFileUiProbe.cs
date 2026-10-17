// Executes original managed parser/control against explicit synthetic text fixtures.
// Does not create a native module or claim original platform/desktop operation.
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

public static class OriginalReadCalibFileUiProbe {
 static string applications, output;
 static List<object> rows = new List<object>();
 static object Describe(object value) {
  var properties = new Dictionary<string,object>();
  foreach(var p in value.GetType().GetProperties()) {
   if(p.GetIndexParameters().Length != 0)continue;
   object field=p.GetValue(value,null);
   if(field==null || field is string || field.GetType().IsPrimitive)properties[p.Name]=field;
   else if(field is IEnumerable)properties[p.Name]=((IEnumerable)field).Cast<object>().Select(Describe).ToArray();
   else if(p.Name=="Point")properties[p.Name]=Describe(field);
  }
  return new {type=value.GetType().FullName,properties};
 }
 [STAThread] public static int Main(string[] args) {
  applications=args[0];output=args[1];Directory.CreateDirectory(output);
  AppDomain.CurrentDomain.AssemblyResolve+=(s,a)=>{string file=new AssemblyName(a.Name).Name+".dll";foreach(string dir in new[]{"myLibs","PublicFile/x64","Module(sp)/x64/Calibration/ReadCalibFileModu",".CommonControl","3rdLib/MVVM"}){string path=Path.Combine(applications,dir,file);if(File.Exists(path))return Assembly.LoadFrom(path);}return null;};
  new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};
  try {
   var original=Assembly.LoadFrom(Path.Combine(applications,"Module(sp)/x64/Calibration/ReadCalibFileModu/ReadCalibFileModuControl.dll"));
   var originalTemplates=(ResourceDictionary)Application.LoadComponent(new Uri("/ReadCalibFileModuControl;component/datatemplate/paramusercontroldatatemplate.xaml",UriKind.Relative));
   Assembly.LoadFrom(Path.Combine(applications,"myLibs/Apps.XmlUI.dll")).GetType("Apps.XmlUI.UserTempConverter",true).GetField("_ResourceDictionary").SetValue(null,originalTemplates);
   var type=original.GetType("Apps.XmlParser.ParamTab.ReadFileConfigData",true);
   var parse=type.GetMethod("AnalyzeString",BindingFlags.Instance|BindingFlags.NonPublic);
   var fixtures=new Dictionary<string,string>{
    {"all-types","int#TransNum,9$RotNum,3;float#PixelPrecision,0.25;string#CalibType,NPointCalib;CalibPointList#ImagePointLst,1~2~3,4~5~6;CalibPointF#SnapImage,7~8~9;FloatList#CalibMatrix,1,0,10,0,1,20,0,0,1"},
    {"invalid-and-unknown","int#Good,1$Bad,nope;float#Bad,nope$Good,2.5;string#Bad,a,b$Good,ok;CalibPointList#Points,1~2,3~4~5;CalibPointF#Bad,1~2$Good,6~7~8;FloatList#Values,1,invalid,3;unknown#Hidden,1"},
    {"empty",""}
   };
   string nativeDescription=Path.Combine(output,"native-description.txt");if(File.Exists(nativeDescription))fixtures["original-module-description"]=File.ReadAllText(nativeDescription);
   foreach(var fixture in fixtures) {
    var model=Activator.CreateInstance(type);
    var parsed=(IList)parse.Invoke(model,new object[]{fixture.Value});
    type.GetProperty("CalibParam").SetValue(model,Activator.CreateInstance(type.GetProperty("CalibParam").PropertyType,new object[]{parsed}),null);
    string screenshot=null,error=null;
    try {
     var control=(FrameworkElement)Activator.CreateInstance(original.GetType("ReadFileModuleControl.ReadFilePanel",true));control.DataContext=model;
     int height=fixture.Key=="original-module-description"?1600:850;var window=new Window{Width=580,Height=height,Left=-30000,Top=-30000,ShowActivated=false,ShowInTaskbar=false,Content=control};
     try {window.Show();window.Dispatcher.Invoke(delegate{},System.Windows.Threading.DispatcherPriority.ApplicationIdle);window.UpdateLayout();var bmp=new RenderTargetBitmap(580,height,96,96,PixelFormats.Pbgra32);bmp.Render(window);var encoder=new PngBitmapEncoder();encoder.Frames.Add(BitmapFrame.Create(bmp));screenshot=fixture.Key+".png";using(var file=File.Create(Path.Combine(output,screenshot)))encoder.Save(file);}finally{window.Close();}
    } catch(Exception e){File.AppendAllText(Path.Combine(output,"exceptions.log"),e.ToString()+Environment.NewLine);while(e.InnerException!=null)e=e.InnerException;error=e.GetType().FullName+": "+e.Message;}
    rows.Add(new {fixture=fixture.Key,input=fixture.Value,items=parsed.Cast<object>().Select(Describe).ToArray(),screenshot,error,authority="original-managed-parser-and-control/synthetic-model",nativeModuleCreated=false,platformDesktopOperation=false});
   }
  } catch(Exception e){File.WriteAllText(Path.Combine(output,"fatal.log"),e.ToString());return 1;}
  File.WriteAllText(Path.Combine(output,"probe.json"),new JavaScriptSerializer{MaxJsonLength=int.MaxValue}.Serialize(rows));return 0;
 }
}
