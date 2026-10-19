using System.Collections;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisionEditor;

static class Program
{
 const BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
 [STAThread]static int Main(string[] args)
 {
  try
  {
   string root=Path.Combine(Path.GetFullPath(args.Single()),"tool-lifecycle-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(root);string path=Path.Combine(root,"own-catalog.json");
   var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};var window=new MainWindow();int checks=0;
   object? Call(string name,params object[] values)=>typeof(MainWindow).GetMethod(name,Private)!.Invoke(window,values);
   object? Field(string name)=>typeof(MainWindow).GetField(name,Private)!.GetValue(window);
   void Set(string name,object value)=>typeof(MainWindow).GetField(name,Private)!.SetValue(window,value);
   void Check(bool ok,string label){if(!ok)throw new Exception(label);checks++;Console.WriteLine("PASS "+label);}
   object Item(string id,object priority)=>new{Id=id,Name=id,Priority=priority,DisplayType="IMAGE",Icon=(string?)null,ModuleId=(string?)null};
   void Write(bool broken)=>File.WriteAllText(path,JsonSerializer.Serialize(new{Schema=1,Groups=new[]{new{Id="A",Name="A",Priority=1d,Icon=(string?)null,HoverIcon=(string?)null,Items=new[]{Item("a1",5d),Item("a2",1d),Item("a3",broken?(object)"invalid-number":3d)}},new{Id="B",Name="B",Priority=2d,Icon=(string?)null,HoverIcon=(string?)null,Items=new[]{Item("b1",2d),Item("b2",1d)}}}}));
   IDictionary States()=>(IDictionary)Field("toolCategoryLoads")!;
   object State(string id)=>States()[id]!;
   int Count(string id)=>((ICollection)State(id).GetType().GetProperty("Items")!.GetValue(State(id))!).Count;
   int Attempts(string id)=>(int)State(id).GetType().GetProperty("Attempts")!.GetValue(State(id))!;
   StackPanel Rail()=>(StackPanel)window.FindName("Rail");
   void Hover(Button button)=>button.RaiseEvent(new MouseEventArgs(Mouse.PrimaryDevice,0){RoutedEvent=Mouse.MouseEnterEvent});
   Write(true);Set("toolCatalogPath",path);Set("toolboxExpanded",false);Call("RefreshToolCatalog");
   Check(States().Count==0&&Rail().Children.Count==2,"First rail build reads headers but initializes no category or tile grid");
   var a=(Button)Rail().Children[0];Hover(a);var oldA=State("A");
   Check(Attempts("A")==1&&Count("A")==2,"First hover preserves successful prefix when third tool metadata fails");
   Check((bool)oldA.GetType().GetProperty("IsInitialized")!.GetValue(oldA)!&&oldA.GetType().GetProperty("Failure")!.GetValue(oldA) is not null,"Initialization and failure are retained before metadata enumeration completes");
   Write(false);Hover(a);Check(Attempts("A")==1&&Count("A")==2,"Repairing source does not silently retry a sticky failed category");
   Hover((Button)Rail().Children[1]);Check(Attempts("B")==1&&Count("B")==2,"Another category loads independently after first category failure");
   Call("ToggleToolbox");Check(Rail().Children.OfType<Expander>().All(e=>e.Content is null),"Expanding rail itself does not eagerly build category tiles");
   var expander=Rail().Children.OfType<Expander>().First();expander.IsExpanded=true;
   Check(expander.Content is System.Windows.Controls.Primitives.UniformGrid grid&&grid.Children.Count==2&&Attempts("A")==1,"Expander shares hover state and successful prefix without retry");
   var menu=(ContextMenu)Call("CanvasSelectionMenu",new Point(100,100))!;var category=menu.Items.OfType<MenuItem>().Single(i=>i.Tag as string=="A");
   category.RaiseEvent(new MouseEventArgs(Mouse.PrimaryDevice,0){RoutedEvent=Mouse.MouseEnterEvent});
   Check(category.Items.Count==2&&Attempts("A")==1,"Canvas category menu uses the same already loaded state");
   Check(category.Items.OfType<MenuItem>().Select(i=>i.Tag as string).SequenceEqual(new[]{"a1","a2"}),"Failed unsorted prefix is retained without completing source sort");
   Check(category.Items.OfType<MenuItem>().All(i=>!i.IsEnabled),"Unregistered palette metadata remains disabled instead of executing unknown DLLs");
   Call("RefreshToolCatalog");Check(States().Count==0,"Explicit refresh creates fresh category loading identities");
   var fresh=Rail().Children.OfType<Expander>().First();fresh.IsExpanded=true;
   Check(!ReferenceEquals(State("A"),oldA)&&Count("A")==3&&Attempts("A")==1,"Explicit refresh permits one new successful attempt after repair");
   Check(((System.Windows.Controls.Primitives.UniformGrid)fresh.Content).Children.OfType<Button>().Select(b=>b.Tag as string).SequenceEqual(new[]{"a2","a3","a1"}),"Fully successful category sorts its tiles by priority after enumeration");
   fresh.IsExpanded=false;fresh.IsExpanded=true;Check(Attempts("A")==1,"Repeated Expanded does not rescan metadata");
   var now=State("A");File.Delete(path);fresh.IsExpanded=false;fresh.IsExpanded=true;Check(ReferenceEquals(State("A"),now)&&Count("A")==3,"Cached loaded category remains usable when backing metadata disappears");
   Write(false);Call("RefreshToolCatalog");File.Delete(path);fresh=Rail().Children.OfType<Expander>().First();fresh.IsExpanded=true;
   Check(Attempts("A")==1&&Count("A")==0,"First-attempt file read failure is retained without inventing loaded tools");
   Write(false);fresh.IsExpanded=false;fresh.IsExpanded=true;Check(Attempts("A")==1&&Count("A")==0,"Restoring missing metadata still requires explicit refresh");
   if(Field("toolPopup") is System.Windows.Controls.Primitives.Popup popup)popup.IsOpen=false;
   window.Close();app.Shutdown();File.WriteAllText(Path.Combine(root,"summary.json"),JsonSerializer.Serialize(new{checks,root,scope="actual MainWindow routed control events and synthetic metadata; no original DLL runtime or desktop parity"}));Console.WriteLine($"PASS {checks}/{checks}; {root}");return 0;
  }
  catch(Exception error){Console.Error.WriteLine(error);return 1;}
 }
}
