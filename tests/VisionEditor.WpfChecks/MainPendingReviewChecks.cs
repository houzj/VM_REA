using System.Collections;
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

static class MainPendingReviewChecks
{
 public static void Run()
 {
  var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};
  var main=new MainWindow{ShowInTaskbar=false,ShowActivated=false,Left=-20000,Top=-20000};
  SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(main.Dispatcher));
  int checks=0;
  void Check(bool condition,string message){if(!condition)throw new Exception(message);Console.WriteLine($"PASS {++checks}: {message}");}
  object? Invoke(string method,params object[] args)=>typeof(MainWindow).GetMethod(method,BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(main,args);
  var doc=(GraphEditorDocument)typeof(MainWindow).GetField("doc",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(main)!;
  try
  {
   main.Show();Pump();doc.NewSolution();var root=doc.CurrentProcedureId;
   var group=doc.AddNode(WorkflowCalls.ModuleId,WorkflowCalls.Version,200,200);
   doc.EnterOwnedGroup(group);var body=doc.CurrentProcedureId;
   var source=doc.AddNode("vision.image-source","1.0.0",120,100);
   var parameters=doc.CurrentProcedure.Nodes.Single(n=>n.Id==source).Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());
   parameters["ImagePaths"]=JsonSerializer.SerializeToElement(JsonSerializer.Serialize(new[]{Path.GetFullPath("analysis/images/circle-find-sample.bmp")}));
   doc.SetParameters(source,parameters);doc.SelectProcedure(root);Pump();
   ProcedureDocument Body()=>doc.Document.Procedures.Single(p=>p.Id==body);
   var window=(SubprogramCallWindow)Invoke("CreateSubprogramCallWindow",group)!;
   window.ShowInTaskbar=false;window.Left=window.Top=-20000;window.WindowStartupLocation=WindowStartupLocation.Manual;window.Show();Pump();
   var tabs=(TabControl)window.FindName("ConfigurationTabs");
   T Control<T>(string tag) where T:FrameworkElement=>Children(window).OfType<T>().Single(c=>Equals(c.Tag,tag));
   void Click(string tag){Control<Button>(tag).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));Pump();}
   Check(((Button)window.FindName("RunOnce")).IsEnabled&&((Button)window.FindName("Repeat")).IsEnabled,"Group run controls remain enabled before first execution");
   foreach(int tab in new[]{0,1})
   {
    tabs.SelectedIndex=tab;Pump();var type=Control<ComboBox>(tab==0?"GroupInputAddType":"GroupOutputAddType");
    foreach(var label in new[]{"pointset","POLYGON","byte","ANNULUS"})
    {
     var choice=type.Items.Cast<object>().Single(v=>v.ToString()==label);
     Check(!(bool)choice.GetType().GetProperty("IsEnabled")!.GetValue(choice)!,label+" remains unavailable without a supported native contract");
    }
    type.SelectedItem=type.Items.Cast<object>().Single(v=>v.ToString()=="pointset");
    Click(tab==0?"GroupInputAdd":"GroupOutputAdd");
    Check(Body().Configuration.Inputs.Count==0&&Body().Configuration.Outputs.Count==0,"Programmatic unsupported type selection creates no mislabeled point-array port");
   }
   tabs.SelectedIndex=1;Pump();Control<ComboBox>("GroupOutputAddType").SelectedIndex=0;
   Exception? pickerError=null;
   main.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle,new Action(()=>
   {
    var picker=app.Windows.OfType<WorkflowSourcePicker>().Single();
    try
    {
     var boxes=Children(picker).OfType<CheckBox>().ToArray();
     boxes.Single(b=>Equals(b.Content,"Width")).IsChecked=true;
     boxes.Single(b=>Equals(b.Content,"Height")).IsChecked=true;
     boxes.Single(b=>Equals(b.Content,"包含模块名")).IsChecked=false;
     Children(picker).OfType<Button>().Single(b=>Equals(b.Content,"确定")).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
    }
    catch(Exception e){pickerError=e;picker.Close();}
   }));
   Click("GroupOutputAdd");if(pickerError is not null)throw pickerError;
   Check(Body().Configuration.Outputs.Count==2&&Body().Configuration.Outputs.All(o=>o.NodeId==source)&&Body().Configuration.Outputs.Select(o=>o.Name).Order().SequenceEqual(new[]{"Height","Width"}),"Multi-select exports actual Group body ports even when choices carry no input Reference");
   tabs.SelectedIndex=3;Pump();Click("GroupDisplayAdd");var display=Body().Configuration.Displays.Single();
   Click("GroupDisplayDelete:"+display.Id);
   Check(Body().Configuration.Displays.Count==0,"Actual display delete button removes its persisted row");
   doc.Undo();Check(Body().Configuration.Displays.Single().Id==display.Id,"Undo restores the deleted display with its identity");doc.Redo();
   Capture(window,"analysis/full-audit/main-pending-review/group-display-delete.png");window.Close();Pump();
   Wait((Task)Invoke("RunProcedure",root)!);
   var sessions=(Dictionary<string,ExecutionSessionResult>)typeof(MainWindow).GetField("workflowSessions",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(main)!;
   Check(sessions[root].Status==SessionStatus.Success&&sessions[body].WorkflowOutputs["Width"].As<long>()==1296,"New multi-selected exports carry actual BMP execution data");
   var path=Path.GetFullPath("analysis/full-audit/main-pending-review/group-exports.vmx.json");doc.Save(path);doc.Load(path);Wait((Task)Invoke("RunProcedure",root)!);
   Check(sessions[body].WorkflowOutputs["Width"].As<long>()==1296&&Body().Configuration.Outputs.Count==2,"Export identities, source bindings and real data survive save/reopen");

   var groupId="review-vars";var variables=Enumerable.Range(0,25).Select(i=>new WorkflowLocalVariable("v"+i,groupId,"%var"+i+"%","int",i.ToString())).ToArray();
   variables[24]=variables[24] with{Name="%请输入变量名称%"};
   var flow=new ProcedureDocument("review","review",[],[],[]){Configuration=new(){Groups=[new(groupId,"分组0")],Variables=variables}};
   var variableWindow=new WorkflowLocalVariablesWindow(()=>flow,c=>flow=flow with{Configuration=c},_=>[],(_,_)=>null){ShowInTaskbar=false,Left=-20000,Top=-20000,WindowStartupLocation=WindowStartupLocation.Manual};
   variableWindow.Show();Pump();var grid=(DataGrid)variableWindow.FindName("Variables");var search=(TextBox)variableWindow.FindName("Search");
   Check(search.Text==""&&grid.Items.Count==12,"Search hint does not become filter data and first page has twelve variables");
   var page=(TextBox)variableWindow.FindName("PageNumber");page.Text="3";
   Children(variableWindow).OfType<Button>().Single(b=>Equals(b.Content,"跳转")).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));Pump();
   Check(grid.Items.Count==1&&page.Text=="3","Page jump reaches the last variable without altering configuration");
   search.Text="请输入变量名称";Pump();
   Check(grid.Items.Count==1&&page.Text=="1","Literal text equal to the visual hint still searches as a real variable name");
   Check(grid.Columns.Select(c=>c.Header?.ToString()).SequenceEqual(new[]{"序号","名称","注释","类型","当前值","输入来源","目标输出","操作"}),"Variable column labels and order follow recovered resource headers");
   Capture(variableWindow,"analysis/full-audit/main-pending-review/variables-search.png");variableWindow.Close();Pump();
   Console.WriteLine($"PASS {checks} main pending review checks");
  }
  finally{main.Close();app.Shutdown();}
 }
 static void Pump(){Dispatcher.CurrentDispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);}
 static void Wait(Task task){var deadline=DateTime.UtcNow.AddSeconds(30);while(!task.IsCompleted&&DateTime.UtcNow<deadline){Pump();Thread.Sleep(1);}if(!task.IsCompleted)throw new TimeoutException("Review operation did not finish");task.GetAwaiter().GetResult();Pump();}
 static IEnumerable<FrameworkElement> Children(DependencyObject root){if(root is FrameworkElement element)yield return element;for(int i=0;i<VisualTreeHelper.GetChildrenCount(root);i++)foreach(var child in Children(VisualTreeHelper.GetChild(root,i)))yield return child;}
 static void Capture(Window window,string path){window.UpdateLayout();var bitmap=new RenderTargetBitmap((int)Math.Ceiling(window.ActualWidth),(int)Math.Ceiling(window.ActualHeight),96,96,PixelFormats.Pbgra32);bitmap.Render(window);var png=new PngBitmapEncoder();png.Frames.Add(BitmapFrame.Create(bitmap));Directory.CreateDirectory(Path.GetDirectoryName(path)!);using var file=File.Create(path);png.Save(file);}
}
