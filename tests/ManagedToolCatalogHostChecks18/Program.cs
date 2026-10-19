using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using Button=System.Windows.Controls.Button;
using MenuItem=System.Windows.Controls.MenuItem;
using ContextMenu=System.Windows.Controls.ContextMenu;
using Point=System.Windows.Point;
using Application=System.Windows.Application;

static class Program
{
 const BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
 [STAThread]static int Main(string[] args)
 {
  // Must precede MainWindow/ToolExecutionScope construction. No camera/device probing.
  AppContext.SetSwitch("VisionEditor.DisableAutomaticCameraDiscovery",true);
  string root=Path.Combine(Path.GetFullPath(args.Single()),"host-catalog-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(root);int checks=0;
  try
  {
   var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};var window=new MainWindow();
   object? Field(string name)=>typeof(MainWindow).GetField(name,Private)!.GetValue(window);
   void Set(string name,object value)=>typeof(MainWindow).GetField(name,Private)!.SetValue(window,value);
   object? Call(string name,params object[] values)=>typeof(MainWindow).GetMethod(name,Private)!.Invoke(window,values);
   void Check(bool ok,string label){if(!ok)throw new Exception(label);checks++;Console.WriteLine("PASS "+label);}
   var modules=(ModuleRegistry)Field("modules")!;var plugins=(ToolPluginRegistry)Field("plugins")!;var doc=(GraphEditorDocument)Field("doc")!;
   string assembly=Path.Combine(AppContext.BaseDirectory,"fixture","ManagedToolFixture16.dll");string hash=Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(assembly)));
   var reference=new ManagedToolPluginReference(assembly,"ManagedToolFixture16.FixturePlugin",hash,"fixture.dynamic","1.0.0");string catalog=Path.Combine(root,"owned-catalog.json");
   object Tool(string id,ManagedToolPluginReference implementation)=>new{Id=id,Name=id,Priority=1d,DisplayType="IMAGE",Icon=(string?)null,ModuleId="fixture.dynamic",Implementation=implementation};
   void Write(bool repair)=>File.WriteAllText(catalog,JsonSerializer.Serialize(new{Schema=1,Groups=new[]{new{Id="Broken",Name="Broken",Priority=1d,Icon=(string?)null,HoverIcon=(string?)null,Items=new[]{Tool("broken",reference with{Sha256=repair?hash:new string('0',64)})}},new{Id="Dynamic",Name="Dynamic",Priority=2d,Icon=(string?)null,HoverIcon=(string?)null,Items=new[]{Tool("dynamic",reference)}}}}));
   StackPanel Rail()=>(StackPanel)window.FindName("Rail");
   Expander Category(string id)=>Rail().Children.OfType<Expander>().Single(e=>Equals(e.Header,id));
   doc.NewSolution();Set("toolboxExpanded",true);Set("toolCatalogPath",catalog);Write(false);Call("RefreshToolCatalog");
   Check(plugins.Find(reference.ModuleId) is null&&!modules.Descriptors.Any(d=>d.Id==reference.ModuleId),"Initial category header discovery does not register or instantiate a plugin");
   var broken=Category("Broken");broken.IsExpanded=true;
   Check(plugins.Find(reference.ModuleId) is null&&broken.Content is null,"Wrong-hash category failure does not create an enabled tool or partial host registration");
   var dynamic=Category("Dynamic");dynamic.IsExpanded=true;
   var plugin=plugins.Find(reference.ModuleId);Check(plugin is not null&&modules.Descriptors.Any(d=>d.Id==reference.ModuleId&&d.Version==reference.Version),"First actual category expansion dynamically registers the selected current-contract plugin");
   var grid=(System.Windows.Controls.Primitives.UniformGrid)dynamic.Content;
   Check(grid.Children.OfType<Button>().Single().IsEnabled,"Real palette tile becomes enabled from its dynamic ModuleRegistry mapping");
   var menu=(ContextMenu)Call("CanvasSelectionMenu",new Point(120,140))!;var item=menu.Items.OfType<MenuItem>().Single(i=>i.Tag as string=="Dynamic");
   item.RaiseEvent(new System.Windows.Input.MouseEventArgs(Mouse.PrimaryDevice,0){RoutedEvent=Mouse.MouseEnterEvent});var add=item.Items.OfType<MenuItem>().Single();Check(add.IsEnabled,"Actual canvas category consumes the same dynamically registered plugin");
   add.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
   Check(doc.CurrentProcedure.Nodes.Count==1&&doc.CurrentProcedure.Nodes.Single().ModuleId==reference.ModuleId,"Actual canvas menu click creates a real dynamic node through the existing host path");
   var run=new GraphExecutor(modules).RunAsync(doc.CurrentProcedure,root).GetAwaiter().GetResult();Check(run.Status==SessionStatus.Success&&run.Nodes.Single().Outputs["Value"].As<long>()==42,"Real host document executes the dynamic plugin node successfully");
   string file=Path.Combine(root,"dynamic.vmx.json");doc.Save(file);doc.Load(file);Check(doc.CurrentProcedure.Nodes.Single().ModuleId==reference.ModuleId,"Ordinary host save/load retains dynamically discovered node identity");
   Write(true);broken.IsExpanded=false;broken.IsExpanded=true;Check(broken.Content is null||broken.Content is System.Windows.Controls.Primitives.UniformGrid {Children.Count:0},"Repairing catalog metadata does not retry failed category without explicit refresh");
   Call("RefreshToolCatalog");Category("Dynamic").IsExpanded=true;Category("Broken").IsExpanded=true;
   Check(ReferenceEquals(plugins.Find(reference.ModuleId),plugin)&&modules.Descriptors.Count(d=>d.Id==reference.ModuleId)==1,"Explicit refresh preserves successful registration without duplicate plugin instances");
   Check(((System.Windows.Controls.Primitives.UniformGrid)Category("Broken").Content).Children.OfType<Button>().Single().IsEnabled,"Explicit refresh permits repaired category to consume the already registered identical reference");
   // Direct session lifecycle validates registration failure retention independently of UI state.
   var session=new ManagedToolCatalogLoadSession();int registrations=0;
   bool failed=false;try{session.EnsureLoaded(reference,_=>{registrations++;throw new InvalidOperationException("owned registration failure");});}catch(ManagedToolCatalogLoadException error){failed=error.Stage=="register"&&error.InnerException?.Message=="owned registration failure";}
   Check(failed&&registrations==1&&session.Observations.Single().Stage=="failed-register","Session records exact registration phase and original failure");
   try{session.EnsureLoaded(reference,_=>registrations++);}catch(ManagedToolCatalogLoadException){}
   Check(registrations==1,"Failed session reference is sticky and does not silently retry registration");
   session.ClearFailures();var successful=session.EnsureLoaded(reference,_=>registrations++);session.ClearFailures();var cached=session.EnsureLoaded(reference,_=>registrations++);
   Check(registrations==2&&ReferenceEquals(successful,cached)&&session.Observations.Single().Stage=="loaded","Explicit failure refresh permits one new registration and preserves later success cache");
   Check(Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(assembly)))==hash,"Only self-owned fixture was loaded and its assembly bytes remain unchanged");
   window.Close();app.Shutdown();File.WriteAllText(Path.Combine(root,"summary.json"),JsonSerializer.Serialize(new{checks,reference,scope="Actual MainWindow category/registry/node consumption; self-owned synthetic plugin only; camera discovery disabled"},new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine($"PASS {checks}/{checks}; {root}");return 0;
  }
  catch(Exception error){File.WriteAllText(Path.Combine(root,"failed.json"),JsonSerializer.Serialize(new{checks,error=error.ToString()}));Console.Error.WriteLine(error);return 1;}
 }
}
