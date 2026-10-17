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

static class GroupLoopUiChecks
{
 static IEnumerable<DependencyObject> Children(DependencyObject root){for(int i=0;i<VisualTreeHelper.GetChildrenCount(root);i++){var child=VisualTreeHelper.GetChild(root,i);yield return child;foreach(var item in Children(child))yield return item;}}
 public static void Run(MainWindow host,Action<bool,string> check)
 {
  T Field<T>(string name)=>(T)typeof(MainWindow).GetField(name,BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(host)!;
  void Pump(){host.UpdateLayout();host.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);}
  void Capture(FrameworkElement control,string name){control.UpdateLayout();var image=new RenderTargetBitmap((int)Math.Ceiling(control.ActualWidth),(int)Math.Ceiling(control.ActualHeight),96,96,PixelFormats.Pbgra32);image.Render(control);var png=new PngBitmapEncoder();png.Frames.Add(BitmapFrame.Create(image));using var file=File.Create("analysis/full-audit/"+name);png.Save(file);}
  var doc=Field<GraphEditorDocument>("doc");doc.NewSolution();var root=doc.CurrentProcedureId;
  doc.SetWorkflowConfiguration(root,new(){Groups=[new("g","分组0")],Variables=[new("limit","g","终值","int","5")]});
  var group=doc.AddOwnedGroup(200,200);doc.SetParameter(group,"LoopStep",JsonSerializer.SerializeToElement(2));var body=doc.EnterOwnedGroup(group);
  var source=doc.AddNode("vision.image-source","1.0.0",150,150,new(){["InitialSN"]=JsonSerializer.SerializeToElement(17),["ImagePaths"]=JsonSerializer.SerializeToElement(JsonSerializer.Serialize(new[]{Path.GetFullPath("analysis/images/circle-find-sample.bmp")}))});
  doc.SetWorkflowConfiguration(body,new(){Outputs=[new("width","width",DataKind.Integer,source,"Width",Accumulate:true),new("sn","sn",DataKind.Integer,source,"SNCode",Accumulate:true)]});Pump();
  typeof(MainWindow).GetMethod("OpenDisplayedConfiguration",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(host,null);Pump();
  var config=Application.Current.Windows.OfType<SubprogramCallWindow>().Single();var tabs=(TabControl)config.FindName("ConfigurationTabs");tabs.SelectedIndex=4;Pump();
  T Control<T>(string tag) where T:FrameworkElement=>Children(config).OfType<T>().Single(c=>Equals(c.Tag,tag));
  void Click(ButtonBase button){button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));Pump();}
  var enable=Children(config).OfType<CheckBox>().Single(c=>Equals(c.Content,"循环使能"));enable.IsChecked=true;Click(enable);
  void Pick(Button button,Func<WorkflowSourceOption,bool> predicate){Exception? error=null;host.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle,new Action(()=>{WorkflowSourcePicker? picker=null;try{picker=Application.Current.Windows.OfType<WorkflowSourcePicker>().Single();picker.SetInitialSelection(predicate);Children(picker).OfType<Button>().Single(b=>Equals(b.Content,"确定")).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));}catch(Exception e){error=e;picker?.Close();}}));Click(button);if(error is not null)throw error;}
  var end=Children(config).OfType<DockPanel>().Single(p=>p.Children.OfType<TextBlock>().Any(t=>t.Text=="循环结束值"));Pick(end.Children.OfType<Button>().Single(),o=>o.Reference?.Kind==WorkflowSourceKind.LocalVariable&&o.Reference.Id=="limit");
  var run=(Button)config.FindName("RunOnce");
  void Wait(){var watch=System.Diagnostics.Stopwatch.StartNew();while(!run.IsEnabled&&watch.ElapsedMilliseconds<12000){Pump();Thread.Sleep(1);}Pump();check(run.IsEnabled,"Group loop UI operation completes without blocked dispatcher");}
  ExecutionSessionResult Result()=>Field<Dictionary<string,ExecutionSessionResult>>("workflowSessions")[root].Calls[group];
  Click(run);Wait();var result=Result();
  check(result.Status==SessionStatus.Success&&result.CompletedGroupIterations==3&&result.GroupLoopIndex==4&&result.WorkflowOutputs["width"].As<IReadOnlyList<long>>().SequenceEqual(new long[]{1296,1296,1296}),"Actual Group window runs saved step2 with subscribed end5 at indexes0/2/4 and real image accumulation");
  check(!Children(config).OfType<TextBlock>().Any(t=>t.Text.Contains("步长")),"Shipped loop XML has no LoopStep widget; framework does not invent one");
  var breakEnable=Control<CheckBox>("GroupBreakEnabled");breakEnable.IsChecked=true;Click(breakEnable);
  Pick(Control<Button>("GroupBreakLeft"),o=>o.Label=="内部.Group.LoopIndex");
  Control<ComboBox>("GroupBreakOperator").SelectedIndex=4;Pump();
  var constant=Control<TextBox>("GroupBreakConstant");constant.Text="4";constant.RaiseEvent(new System.Windows.Input.KeyboardFocusChangedEventArgs(System.Windows.Input.Keyboard.PrimaryDevice,0,constant,null){RoutedEvent=System.Windows.Input.Keyboard.LostKeyboardFocusEvent});Pump();
  Click(run);Wait();result=Result();
  check(result.Status==SessionStatus.Success&&result.GroupBroke&&result.GroupBreakIndex==4&&result.CompletedGroupIterations==2&&result.WorkflowOutputs["width"].As<IReadOnlyList<long>>().Count==2,"Actual break source picker, operator and constant stop step2 before index4 and retain two real image outputs");
  check(((TextBlock)config.FindName("Error")).Text.Contains("索引 4")&&((TextBlock)config.FindName("Error")).Text.Contains("2 次"),"Actual Group result shows stepped break index and completed count");
  Capture(config,"wpf-group-stepped-break.png");
  // Error text must not leave the previous successful break visible after cancellation.
  doc.EditProcedure(root,d=>{d.SetGroupBreak(group,false,1,5,null);d.SetParameter(group,"LoopTimeGap",JsonSerializer.SerializeToElement(10000));});
  tabs.SelectedIndex=0;tabs.SelectedIndex=4;Pump();Click(run);var stop=(Button)config.FindName("Stop");check(stop.IsEnabled&&!tabs.IsEnabled,"Actual Group long interval disables editing while running");Click(stop);Wait();
  var parentResult=Field<Dictionary<string,ExecutionSessionResult>>("workflowSessions")[root];
  check(parentResult.Status==SessionStatus.Cancelled&&!((TextBlock)config.FindName("Error")).Text.Contains("循环已中断"),"Stop cancels actual Group interval and removes stale successful break message");
  doc.EditProcedure(root,d=>d.SetParameter(group,"LoopTimeGap",JsonSerializer.SerializeToElement(0)));tabs.SelectedIndex=0;tabs.SelectedIndex=4;Pump();
  enable=Children(config).OfType<CheckBox>().Single(c=>Equals(c.Content,"循环使能"));enable.IsChecked=false;Click(enable);
  check(!Children(config).OfType<CheckBox>().Any(c=>Equals(c.Tag,"GroupBreakEnabled")),"Disabling loop hides dependent break controls as shipped XML triggers specify");
  Click(run);Wait();check(Result().Status==SessionStatus.Success&&Result().CompletedGroupIterations==1&&Result().WorkflowOutputs["width"].As<IReadOnlyList<long>>().Count==1,"Actual window restarts after stop; disabled loop returns one fresh image output");
  check(((TextBlock)config.FindName("Error")).Text.Length==0,"Successful restart clears earlier Group break or cancelled status rather than leaving stale red message");
  var path=Path.GetFullPath("analysis/full-audit/group-stepped-window.vmx.json");doc.Save(path);var reopen=new GraphEditorDocument(Field<ModuleRegistry>("modules"));reopen.Load(path);
  check(reopen.Document.Procedures.Single(p=>p.Id==root).Nodes.Single(n=>n.Id==group).Parameters["LoopStep"].GetInt32()==2,"Actual window save reopen retains hidden engine step without adding a native-looking control");
  Capture(config,"wpf-group-loop-disabled.png");config.Close();Pump();doc.Load(Path.Combine(AppContext.BaseDirectory,"demo.vmx.json"));Pump();
 }
}
