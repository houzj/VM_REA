using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VisionFramework;
using VisionPluginContracts;
static class Program {
 [DllImport("kernel32.dll")]static extern uint SetErrorMode(uint value);
 static readonly string output=Path.Combine(Environment.CurrentDirectory,"analysis/full-audit/configuration-acceptance-demo");
 static int checks;
 static void Check(bool ok,string message){checks++;if(!ok)throw new InvalidOperationException(message);}
 [STAThread]static int Main(string[] args){Console.OutputEncoding=System.Text.Encoding.UTF8;SetErrorMode(0x8003);try{return Run(args);}catch(Exception e){Directory.CreateDirectory(output);File.WriteAllText(Path.Combine(output,"failure.txt"),e.ToString());Console.Error.WriteLine(e);return 1;}}
 static int Run(string[] args){
  bool visible=args.Contains("--demo");bool manual=args.Contains("--manual");var variant=manual?"-manual":"";var fixture=Path.Combine(AppContext.BaseDirectory,"Fixtures");
  var xml=File.ReadAllText(Path.Combine(fixture,"IMVSNPointCalibModuAlgorithmTab.xml"));var groups=File.ReadAllText(Path.Combine(fixture,"Group.xml"));
  var originalSources=JsonDocument.Parse(File.ReadAllText(Path.Combine(fixture,"sources.json")));
  foreach(var file in new[]{"IMVSNPointCalibModuAlgorithmTab.xml","Group.xml"}){
   var expected=originalSources.RootElement.EnumerateArray().Single(e=>e.GetProperty("file").GetString()==file).GetProperty("sha256").GetString();
   Check(Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(Path.Combine(fixture,file))))==expected,"Original fixture hash: "+file);
  }
  Directory.CreateDirectory(output);var backend=new PreviewBackend();var definition=ToolConfigurationDefinition.Parse(xml,groups);var state=new ToolConfigurationState(definition,backend){ExecutionEnabled=false};state.Initialize();if(manual)state.CommitUser("CalibPointGet","1");
  var language=args.Contains("--english")?"English":"Chinese";var localize=ConfigurationLocalization.ForLanguage(language).Get;
  var registry=new ToolConfigurationControlRegistry();ToolConfigurationStandardControls.Register(registry,localize,language:language);ToolConfigurationSubscriptionControls.Register(registry,null,localize,language);
  var session=new ToolConfigurationSession(state,registry,language,localize);
  var node=new NodeDocument("npoint-framework-acceptance","IMVSNPointCalibModu","4.4.0","N点标定 · 框架验收样例（无算法执行）",new());
  var context=new ToolEditorContext(node,Environment.CurrentDirectory,_=>throw new InvalidOperationException("Preview does not apply production node parameters"),_=>throw new NotSupportedException("No algorithm run in template acceptance demo"),1);
  var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};
  app.DispatcherUnhandledException+=(_,e)=>{File.WriteAllText(Path.Combine(output,"dispatcher-failure.txt"),e.Exception.ToString());Console.Error.WriteLine(e.Exception);e.Handled=true;app.Shutdown(1);};
  var window=new ToolConfigurationModelWindow(context,session){ShowInTaskbar=visible};
  if(!visible){window.Left=-20000;window.Top=-20000;window.WindowStartupLocation=WindowStartupLocation.Manual;window.ShowActivated=false;}
  window.Closed+=(_,_)=>app.Shutdown();window.Show();var view=window.ConfigurationView;
  Check(view.Tabs.Items.Count==definition.Tabs.Count,"Actual original top tabs retained");Check(view.DesiredConfigurationSize.Width==(language=="English"?680:420)&&view.DesiredConfigurationSize.Height==500,"Original language geometry");
  var pages=new List<object>();
  for(int i=0;i<view.Tabs.Items.Count;i++){
   var tab=(TabItem)view.Tabs.Items[i];view.Tabs.SelectedIndex=i;view.Refresh();window.UpdateLayout();
   var w=Math.Max(1,(int)Math.Ceiling(window.ActualWidth));var h=Math.Max(1,(int)Math.Ceiling(window.ActualHeight));var bitmap=new RenderTargetBitmap(w,h,96,96,PixelFormats.Pbgra32);bitmap.Render(window);
   var path=$"{language.ToLowerInvariant()}{variant}-tab-{i+1}.png";var encoder=new PngBitmapEncoder();encoder.Frames.Add(BitmapFrame.Create(bitmap));using(var file=File.Create(Path.Combine(output,path)))encoder.Save(file);
   var elements=Walk(view).OfType<FrameworkElement>().ToArray();
   Check(elements.Where(e=>e is ConfigurationNumericSpinner).All(e=>e.GetType().Assembly==typeof(ToolConfigurationStandardControls).Assembly),"Production numeric factory only");
   foreach(var combo in elements.OfType<ComboBox>().Where(c=>c.Items.Count>0))Check(combo.SelectedValue!=null,"Original integer EnumEntry values select actual current value");
   pages.Add(new{index=i,sourceTabName=tab.Tag,caption=tab.Header,visible=tab.Visibility.ToString(),image=path,renderedTypes=elements.GroupBy(e=>e.GetType().FullName).ToDictionary(g=>g.Key!,g=>g.Count())});
  }
  var unsupported=view.Diagnostics.Where(x=>x.Code=="UnsupportedTemplate").ToArray();
  Check(unsupported.Length>0,"Incomplete original complex templates explicitly diagnosed");
  var rows=state.Nodes.Select(n=>new{tag=n.Element.Tag,name=n.Name,captionKey=n.CaptionKey,template=n.Template.TemplateKey,displayType=n.DisplayType,source=n.Element.Source,visible=n.Visible,enabled=n.Enabled,fields=n.Element.Children.Where(c=>c.Children.Count==0).Select(c=>new{property=c.Tag,value=c.Text}),choices=n.Choices,renderer=unsupported.Any(d=>d.Node==n.Name)?"Unsupported":n.Element.Tag is "Tab" or "Category" or "CategoryNoHead"?"SharedStructural":"ProductionLeaf",n.Template.Support}).ToArray();
  var manifest=new{label="TEST HARNESS: original N-point XML/Group with production shared configuration controls",language,manual,checks,sourceXmlSha256=definition.SourceSha256,groupSha256=definition.GroupsSourceSha256,pages,nodes=rows,unsupportedTemplates=unsupported,unsupportedState=state.Diagnostics,previewBackendOperations=backend.Events,boundaries=new[]{"XML and Group are byte-identical original fixtures; no test leaf factories are registered.","Backend reads/writes XML preview values only. It does not run algorithms, open/save calibration, subscribe real nodes or implement tool-specific custom rules.","Subscription controls show explicit missing-service diagnostics. Complex missing templates stay visible as Unsupported diagnostics.","ShowInTaskbar=true is a demo-only accessibility override; product common host remains unchanged.","Render PNGs are independent implementation evidence, not original desktop captures or pixel-equivalence proof."}};
  File.WriteAllText(Path.Combine(output,$"{language.ToLowerInvariant()}{variant}-coverage.json"),JsonSerializer.Serialize(manifest,new JsonSerializerOptions{WriteIndented=true}));
  Console.WriteLine($"{checks} original N-point shared-host checks passed; {unsupported.Length} unsupported leaves. Report: {output}");
  view.Tabs.SelectedIndex=0;view.Refresh();
  if(visible)return app.Run();window.Close();app.Shutdown();return 0;
 }
 static IEnumerable<DependencyObject> Walk(DependencyObject root){yield return root;for(int i=0;i<VisualTreeHelper.GetChildrenCount(root);i++)foreach(var x in Walk(VisualTreeHelper.GetChild(root,i)))yield return x;}
 sealed class PreviewBackend:IToolConfigurationDispatcher {
  readonly Dictionary<string,string> parameters=new(StringComparer.Ordinal);readonly Dictionary<string,string> user=new(StringComparer.Ordinal);
  public List<object> Events{get;}=[];
  public ConfigurationOperationResult Dispatch(ConfigurationOperationRequest r){
   var key=(r.Source.Name??"")+(r.Source.Get("ComTailName")??"");Events.Add(new{kind=r.Kind.ToString(),r.OriginalName,key,r.Value});
   return r.Kind switch{
    ConfigurationOperationKind.ReadParameter=>new(true,parameters.GetValueOrDefault(key,r.Source.Value)),
    ConfigurationOperationKind.WriteParameter=>Write(parameters,key,r.Value),
    ConfigurationOperationKind.ReadUserValue=>new(true,user.GetValueOrDefault(r.Parameters.FirstOrDefault()??key)),
    ConfigurationOperationKind.PersistUserValue=>Write(user,r.Parameters.FirstOrDefault()??key,r.Value),
    _=>new(false,Error:"Preview backend does not implement: "+r.OriginalName)
   };
  }
  static ConfigurationOperationResult Write(Dictionary<string,string> store,string key,string? value){store[key]=value??"";return new(true,value);}
 }
}

