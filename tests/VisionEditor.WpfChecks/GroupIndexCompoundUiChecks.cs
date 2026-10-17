using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;
using Button=System.Windows.Controls.Button;
using CheckBox=System.Windows.Controls.CheckBox;
using ComboBox=System.Windows.Controls.ComboBox;
using TextBox=System.Windows.Controls.TextBox;

static class GroupIndexCompoundUiChecks
{
 static IEnumerable<DependencyObject> Children(DependencyObject root){for(int i=0;i<VisualTreeHelper.GetChildrenCount(root);i++){var child=VisualTreeHelper.GetChild(root,i);yield return child;foreach(var item in Children(child))yield return item;}}
 public static void Run(MainWindow host,Action<bool,string> check)
 {
  T Field<T>(string name)=>(T)typeof(MainWindow).GetField(name,BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(host)!;
  void Pump(){host.UpdateLayout();host.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);}
  var doc=Field<GraphEditorDocument>("doc");doc.NewSolution();var root=doc.CurrentProcedureId;
  doc.SetWorkflowConfiguration(root,new(){Groups=[new("g","分组0")],Variables=[new("index","g","数组索引","int","1")]});var group=doc.AddOwnedGroup(100,100);doc.SetParameter(group,"EnableLoop",J(true));doc.SetParameter(group,"EndIndex",J(5));var body=doc.EnterOwnedGroup(group);
  var source=doc.AddNode("vision.image-source","1.0.0",100,100,new(){["InitialSN"]=J(0),["ImagePaths"]=J(JsonSerializer.Serialize(new[]{Path.GetFullPath("analysis/images/circle-find-sample.bmp")}))});
  var circle=doc.AddNode("vision.circle","1.0.0",100,200,new(){["CenterX"]=J(717d),["CenterY"]=J(507d),["InitialRadius"]=J(375d),["SearchHalfWidth"]=J(12d),["SampleCount"]=J(64),["GradientThreshold"]=J(15d),["Polarity"]=J("Both"),["RadialStep"]=J(.5d),["GradientHalfSpan"]=J(1d),["MinimumEdges"]=J(20)});doc.Connect(source,"Image",circle,"Image");
  doc.SetWorkflowConfiguration(body,new(){Outputs=[new("sn","sn",DataKind.Integer,source,"SNCode",Accumulate:true),new("circle","circle",DataKind.Circle,circle,"Circle"),new("image","image",DataKind.GrayImage,source,"Image",Display:true)],Displays=[new("circles","circles",DataKind.Circle,circle,"Circle",Accumulate:true)]});Pump();
  typeof(MainWindow).GetMethod("OpenDisplayedConfiguration",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(host,null);Pump();var config=Application.Current.Windows.OfType<SubprogramCallWindow>().Single();var tabs=(TabControl)config.FindName("ConfigurationTabs");
  T Control<T>(string tag) where T:FrameworkElement=>Children(config).OfType<T>().Single(c=>Equals(c.Tag,tag));
  void Click(ButtonBase button){button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));Pump();}
  void Pick(Button button,Func<WorkflowSourceOption,bool> predicate){Exception? error=null;host.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle,new Action(()=>{WorkflowSourcePicker? picker=null;try{picker=Application.Current.Windows.OfType<WorkflowSourcePicker>().Single();picker.SetInitialSelection(predicate);Children(picker).OfType<Button>().Single(b=>Equals(b.Content,"确定")).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));}catch(Exception e){error=e;picker?.Close();}}));Click(button);if(error is not null)throw error;}
  tabs.SelectedIndex=1;Pump();var accumulate=Control<CheckBox>("GroupOutputAccumulate:circle");check(accumulate.IsEnabled,"Actual Circle output exposes source-guided compound accumulation switch");accumulate.IsChecked=true;Click(accumulate);
  check(doc.Document.Procedures.Single(p=>p.Id==root).Nodes.Single(n=>n.Id==group).Call!.Outputs.Single(o=>o.Name=="circle").Cardinality==ValueCardinality.Array,"Actual compound accumulation switch updates parent call signature");
  tabs.SelectedIndex=4;Pump();var enable=Control<CheckBox>("GroupBreakEnabled");enable.IsChecked=true;Click(enable);Pick(Control<Button>("GroupBreakLeft"),o=>o.Label=="内部.Group.LoopIndex");Pick(Control<Button>("GroupBreakRight"),o=>o.Label=="Group.sn");Control<ComboBox>("GroupBreakOperator").SelectedIndex=4;Pump();
  check(NativeGroupIndexEditor.IsInsideIndex("Group.sn[数组索引]",9)&&!NativeGroupIndexEditor.IsInsideIndex("Group.sn[数组索引]",7)&&!NativeGroupIndexEditor.IsInsideIndex("Group.sn[数组索引]",16),"Native bracket-index editor activates only when caret is inside one matching bracket pair");
  check(Control<TextBox>("GroupBreakRightIndex").Text.StartsWith('[')&&Control<TextBox>("GroupBreakRightIndex").Text.EndsWith(']'),"Group index displays a bracketed source relation in the actual configuration window");
  var inline=Children(config).OfType<NativeGroupIndexEditor>().Single();inline.Text.Focus();inline.Text.CaretIndex=2;Pump();
  check(inline.IndexPopup.IsOpen&&inline.IndexTree.Width==180&&inline.IndexTree.Height==180&&inline.IndexTree.Items.Count>0,"Actual Group index caret opens a refreshed 180x180 integer source tree");
  Capture((FrameworkElement)inline.IndexPopup.Child,"wpf-group-index-popup.png");
  var indexLeaf=inline.IndexTree.Items.OfType<TreeViewItem>().Single(i=>i.Tag is WorkflowSourceOption option&&option.Reference?.Kind==WorkflowSourceKind.LocalVariable&&option.Reference.Id=="index");
  indexLeaf.RaiseEvent(new System.Windows.Input.MouseButtonEventArgs(System.Windows.Input.Mouse.PrimaryDevice,0,System.Windows.Input.MouseButton.Left){RoutedEvent=System.Windows.Controls.Control.MouseDoubleClickEvent});Pump();
  check(doc.Document.Procedures.Single(p=>p.Id==root).Nodes.Single(n=>n.Id==group).GroupBreak!.Right!.IndexSource!.Id=="index"&&!inline.IndexPopup.IsOpen,"Inline integer leaf double-click commits the actual Group runtime index source");
  check(Control<TextBox>("GroupBreakRightIndex").Text.Contains("数组索引")&&doc.Document.Procedures.Single(p=>p.Id==root).Nodes.Single(n=>n.Id==group).GroupBreak!.Right!.IndexSource!.Id=="index","Actual integer-index picker stores a typed index subscription and shows selected relation");
  var run=(Button)config.FindName("RunOnce");void Run(){Click(run);var watch=System.Diagnostics.Stopwatch.StartNew();while(!run.IsEnabled&&watch.ElapsedMilliseconds<15000){Pump();Thread.Sleep(1);}Pump();check(run.IsEnabled,"Actual dynamic-index Group run completes with responsive window");}
  ExecutionSessionResult Result()=>Field<Dictionary<string,ExecutionSessionResult>>("workflowSessions")[root].Calls[group];
  Run();var result=Result();check(result.Status==SessionStatus.Success&&result.GroupBroke&&result.GroupBreakIndex==2&&result.CompletedGroupIterations==2&&result.WorkflowOutputs["sn"].As<IReadOnlyList<long>>().SequenceEqual(new long[]{0,1}),"Actual image-source SN array indexed by parent variable1 waits for available element then breaks after two rounds");
  check(result.WorkflowOutputs["circle"].ElementCount==2&&result.WorkflowOutputs["circle"].As<IReadOnlyList<VisionCore.CircleFit>>().All(c=>c.Radius>370&&c.Radius<385),"Actual radial-circle tool returns two compound results through window-run Group accumulation");
  check(!Children(config).OfType<TextBox>().Any(c=>Equals(c.Tag,"GroupBreakConstant")),"Bound array comparison does not leave a blank disabled constant editor");
  var valueText=Children((DataGrid)config.FindName("Results")).OfType<TextBlock>().Single(t=>t.Text.StartsWith("[圆("));check(valueText.TextWrapping==TextWrapping.Wrap&&valueText.ActualHeight>24&&valueText.ToolTip?.ToString()==valueText.Text,"Actual compound result cell wraps both elements and retains complete tooltip instead of clipping second record");
  check(((Canvas)config.FindName("DisplayOverlay")).Children.OfType<System.Windows.Shapes.Ellipse>().Count()==2,"Actual accumulated circle display draws two scalar geometry records over real image");
  Capture(config,"wpf-group-dynamic-index-compound.png");
  // Numeric edit explicitly leaves subscription mode and is one undoable configuration change.
  var index=Control<TextBox>("GroupBreakRightIndex");index.Text="0";index.RaiseEvent(new System.Windows.Input.KeyboardFocusChangedEventArgs(System.Windows.Input.Keyboard.PrimaryDevice,0,index,null){RoutedEvent=System.Windows.Input.Keyboard.LostKeyboardFocusEvent});Pump();
  check(doc.Document.Procedures.Single(p=>p.Id==root).Nodes.Single(n=>n.Id==group).GroupBreak!.Right!.IndexSource is null,"Actual numeric index edit clears dynamic subscription instead of retaining hidden competing binding");doc.Undo();Pump();tabs.SelectedIndex=0;tabs.SelectedIndex=4;Pump();check(Control<TextBox>("GroupBreakRightIndex").Text.Contains("数组索引"),"Undo restores dynamic index relation and displayed label");
  var path=Path.GetFullPath("analysis/full-audit/group-index-window.vmx.json");doc.Save(path);var loaded=new GraphEditorDocument(Field<ModuleRegistry>("modules"));loaded.Load(path);check(loaded.Document.Procedures.Single(p=>p.Id==root).Nodes.Single(n=>n.Id==group).GroupBreak!.Right!.IndexSource!.Id=="index"&&loaded.Document.Procedures.Single(p=>p.Id==root).Nodes.Single(n=>n.Id==group).Call!.Outputs.Single(o=>o.Name=="circle").Cardinality==ValueCardinality.Array,"Actual window save reopen preserves both dynamic index and compound output signature");
  tabs.SelectedIndex=1;Pump();accumulate=Control<CheckBox>("GroupOutputAccumulate:circle");accumulate.IsChecked=false;Click(accumulate);Run();check(Result().WorkflowOutputs["circle"].Cardinality==ValueCardinality.Scalar,"Actual compound accumulation off returns final scalar geometry after another real run");Capture(config,"wpf-group-compound-final-scalar.png");config.Close();Pump();doc.Load(Path.Combine(AppContext.BaseDirectory,"demo.vmx.json"));Pump();
  void Capture(FrameworkElement control,string name){control.UpdateLayout();var image=new RenderTargetBitmap((int)Math.Ceiling(control.ActualWidth),(int)Math.Ceiling(control.ActualHeight),96,96,PixelFormats.Pbgra32);image.Render(control);var png=new PngBitmapEncoder();png.Frames.Add(BitmapFrame.Create(image));using var file=File.Create("analysis/full-audit/"+name);png.Save(file);}
  static JsonElement J<T>(T value)=>JsonSerializer.SerializeToElement(value);
 }
}
