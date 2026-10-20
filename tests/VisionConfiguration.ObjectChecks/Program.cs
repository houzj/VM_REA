using System.Windows;
using System.Windows.Media;
using VisionPluginContracts;
class Program {
 static int count;
 static void Check(bool condition,string message){count++;if(!condition)throw new Exception(message);}
 [STAThread]static int Main(){try{Run();Console.WriteLine($"PASS {count} object binding checks; native desktop parity unverified.");return 0;}catch(Exception error){Console.Error.WriteLine(error);return 1;}}
 static void Run(){
 var settings=new ToolTextDisplaySettings("A{old}Z","#66ff00","#ff0000",10,1,0,0);bool accept=true,write=true;int changes=0,applies=0;
 var service=new ToolDisplayConfigurationObjectService(()=>settings,next=>{applies++;if(!accept)return false;if(write)settings=next;return true;},()=>changes++);
 var routes=new[]{new ConfigurationObjectRoute("Show Text","Content",ConfigurationObjectProperty.Content),new("Show Text",null,ConfigurationObjectProperty.OkColor),new("Show Text",null,ConfigurationObjectProperty.NgColor),new("Show Text","FontSize",ConfigurationObjectProperty.FontSize),new("Show Text",null,ConfigurationObjectProperty.Opacity)};
 foreach(var route in routes)Check(ReferenceEquals(service.Resolve(route),service.Resolve(route)),"object binding identity stable");
 Check(service.Resolve(new("Other",null,ConfigurationObjectProperty.OkColor))==null,"unknown object unsupported");Check(service.Resolve(new("Show Text","Content",ConfigurationObjectProperty.Opacity))==null,"feature mismatch unsupported");
 var font=service.Resolve(routes[3])!;Check(font.Read()=="10","font reads object");Check(font.Write("20").Accepted&&settings.FontSize==20&&changes==1,"font writes display object");
 foreach(var invalid in new[]{"NaN","Infinity","73","5","bad"})Check(!font.Write(invalid).Accepted&&settings.FontSize==20,"invalid font rejected");
 accept=false;Check(!font.Write("30").Accepted&&font.Read()=="20"&&changes==1,"host rejection preserves state");accept=true;write=false;Check(!font.Write("30").Accepted&&changes==1,"missing readback not success");write=true;
 var opacity=service.Resolve(routes[4])!;Check(opacity.Write("0.5").Accepted&&settings.Opacity==.5,"opacity writes typed property");Check(!opacity.Write("1.1").Accepted,"opacity bounds");
 var color=service.Resolve(routes[1])!;Check(color.Write("#010203").Accepted&&settings.OkColor=="#010203","color writes typed property");Check(!color.Write("garbage").Accepted,"invalid color rejected");
 var reopened=new ToolDisplayConfigurationObjectService(()=>settings,next=>{settings=next;return true;});Check(reopened.Resolve(routes[1])!.Read()=="#010203","window reopen reads shared persisted display");
 settings=settings with{OkColor="#040506"};Check(color.Read()=="#040506","binding observes external host update");
 var throws=new ToolDisplayConfigurationObjectService(()=>settings,_=>throw new InvalidOperationException("reject"));Check(!throws.Resolve(routes[0])!.Write("new").Accepted&&settings.Content=="A{old}Z","host exception does not change object");
 Check(service.Resolve(new("Show Text",null,(ConfigurationObjectProperty)999))==null,"unknown property unsupported");
 var choice=new ConfigurationTextPlaceholder("stable","caption","Flow.Node.Value");Check(ConfigurationTextPlaceholderEditing.Insert("A{old}Z",3,choice)=="A{Flow.Node.Value}Z","caret placeholder replacement");
 foreach(var pair in new[]{("plain",2),("A{bad",3),("A}bad}",3),("A{{x}",2),("A{x}Z",5)})Check(!ConfigurationTextPlaceholderEditing.TryRange(pair.Item1,pair.Item2,out _,out _),"reject nonbrace context");
 var items=string.Join("",new[]{("StringConfig","Content","Value","GetDisplayObjectFeatureOperation","Show Text;Content"),("ColorConfig","OKColor","okcolor","GetDisplayObjectOperation","Show Text"),("ColorConfig","NGColor","ngcolor","GetDisplayObjectOperation","Show Text"),("FloatConfig","Size","Value","GetDisplayObjectFeatureOperation","Show Text;FontSize"),("FloatConfig","Opacity","opacity","GetDisplayObjectOperation","Show Text")}.Select(x=>$"<{x.Item1} Name='{x.Item2}'><AccessMode>O</AccessMode><CurValue>DO_NOT_WRITE</CurValue><PropertyName>{x.Item3}</PropertyName><MinValue>0</MinValue><MaxValue>72</MaxValue><BrigdgeSetter><OperationName>{x.Item4}</OperationName><OperationParams>{x.Item5}</OperationParams></BrigdgeSetter></{x.Item1}>"));
 var state=new ToolConfigurationState(ToolConfigurationDefinition.Parse($"<AlgorithmTabRoot><Tabs><Tab Name='T'><Categorys><Category Name='C'><Items>{items}</Items></Category></Categorys></Tab></Tabs></AlgorithmTabRoot>"));
 foreach(var node in state.Nodes.Where(n=>n.Get("PropertyName")!=null))Check(ConfigurationObjectRoute.Parse(node)!=null,"source route parse");
 var registry=new ToolConfigurationControlRegistry();ToolConfigurationObjectControls.Register(registry,service,colorPalette:selected=>new System.Windows.Controls.Button{Content="palette"});using var view=new ToolConfigurationView(state,registry);view.Measure(new Size(420,500));view.Arrange(new Rect(0,0,420,500));view.UpdateLayout();
 var spinners=Walk(view).OfType<ConfigurationNumericSpinner>().ToArray();Check(spinners.Length==2,"two production object spinners");spinners[0].Editor.Text="25";Check(spinners[0].CommitText()&&settings.FontSize==25,"production spinner edits object");Check(state.Find("Size")!.Value=="DO_NOT_WRITE","algorithm CurValue untouched");
 var objectText=Walk(view).OfType<ConfigurationObjectTextBox>().Single();Check(objectText.Margin==new Thickness(10,0,0,0)&&objectText.BorderThickness==new Thickness(0),"original DeleteTextBox inset and border");
 int focusEvents=0;objectText.FocusStateChanged+=()=>focusEvents++;objectText.Text="abcdef";objectText.Select(1,2);int partial=focusEvents;objectText.Select(2,2);Check(focusEvents==partial,"partial selection does not publish source focus event");objectText.SelectAll();Check(focusEvents>partial,"full selection publishes source focus event");
 using var inputSource=new System.Windows.Interop.HwndSource(new System.Windows.Interop.HwndSourceParameters("ObjectChecks input"){Width=1,Height=1,WindowStyle=0});objectText.RaiseEvent(new System.Windows.Input.KeyEventArgs(System.Windows.Input.Keyboard.PrimaryDevice,inputSource,0,System.Windows.Input.Key.Delete){RoutedEvent=System.Windows.Input.Keyboard.KeyUpEvent});Check(objectText.Text=="","source Delete key clears complete text");
 Action<string>? first=null,second=null;string? selected=null,initial=null;
 var colorTabs=(System.Windows.Controls.Border)ToolConfigurationObjectControls.CreateColorTabs("#123456",v=>selected=v,callback=>{first=callback;return new System.Windows.Controls.TextBlock();},(value,callback)=>{initial=value;second=callback;return new System.Windows.Controls.TextBlock();});
 var tabs=(System.Windows.Controls.TabControl)colorTabs.Child;Check(tabs.Items.Count==2&&initial=="#123456","default and more color tabs preserve initial color");first!("#010101");Check(selected=="#010101","default palette callback");second!("#020202");Check(selected=="#020202","advanced picker callback");
 var initialColors=new List<string>();var typedRegistry=new ToolConfigurationControlRegistry();ToolConfigurationObjectControls.Register(typedRegistry,service,colorPicker:(value,callback)=>{initialColors.Add(value);return new System.Windows.Controls.TextBlock();});
 using var typedView=new ToolConfigurationView(state,typedRegistry);typedView.Measure(new Size(420,500));typedView.Arrange(new Rect(0,0,420,500));
 var colorButtons=Walk(typedView).OfType<System.Windows.Controls.Primitives.ToggleButton>().Where(button=>button.Content is System.Windows.Controls.DockPanel).ToArray();Check(colorButtons.Length==2,"independent OK and NG color controls");
 colorButtons[0].IsChecked=true;colorButtons[0].IsChecked=false;colorButtons[1].IsChecked=true;colorButtons[1].IsChecked=false;
 Check(initialColors.SequenceEqual(new[]{settings.OkColor,settings.NgColor}),"each color picker opens with its own binding value");
 settings=settings with{OkColor="#112233"};colorButtons[0].IsChecked=true;colorButtons[0].IsChecked=false;Check(initialColors[^1]=="#112233","reopening advanced picker reads latest host color");
 }
 static IEnumerable<DependencyObject> Walk(DependencyObject item){yield return item;for(int i=0;i<VisualTreeHelper.GetChildrenCount(item);i++)foreach(var child in Walk(VisualTreeHelper.GetChild(item,i)))yield return child;}
}
