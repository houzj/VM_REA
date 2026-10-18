using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisionFramework;
using TextBox=System.Windows.Controls.TextBox;
using Button=System.Windows.Controls.Button;
using Orientation=System.Windows.Controls.Orientation;
namespace VisionEditor;

public partial class SubprogramCallWindow:Window
{
 readonly Func<ProcedureDocument> owner;readonly Func<NodeDocument> node;readonly Func<IReadOnlyList<ProcedureDocument>> procedures;
 readonly Action<string> configure;readonly Action<string,WorkflowValueReference?> bind;readonly Func<DataKind,IEnumerable<WorkflowSourceOption>> sources;
 readonly Action<string,System.Text.Json.JsonElement>? setLoopParameter; readonly Action<string,WorkflowValueReference?>? bindLoopParameter;
 readonly Func<Task> run;readonly Action stop;readonly Func<ExecutionSessionResult?> result;bool refreshing,running,closeReady,closePending;Task? runningTask;
 public Func<string,VisionCore.GrayImage?>? GroupToolImage{get;set;}
 public Action<string>? OpenGroupToolEditor{get;set;}
 public Func<string,bool>? HasGroupToolEditor{get;set;}
 public bool IsExecuting=>running;
 public string? EnterProcedureId{get;private set;}
 public SubprogramCallWindow(Func<ProcedureDocument> owner,Func<NodeDocument> node,Func<IReadOnlyList<ProcedureDocument>> procedures,Action<string> configure,Action<string,WorkflowValueReference?> bind,Func<DataKind,IEnumerable<WorkflowSourceOption>> sources,Func<Task> run,Action stop,Func<ExecutionSessionResult?> result,Action<string,System.Text.Json.JsonElement>? setLoopParameter=null,Action<string,WorkflowValueReference?>? bindLoopParameter=null,Action<bool,int,int,GroupBreakBindings?>? setBreak=null,Func<DataKind,IEnumerable<GroupComparisonOption>>? breakSources=null,Action<WorkflowConfiguration>? saveGroupConfiguration=null,Func<DataKind?,IEnumerable<WorkflowSourceOption>>? groupSources=null,ModuleRegistry? groupRegistry=null,Action<string,string,System.Text.Json.JsonElement>? setGroupParameter=null,Action<string,string,bool,WorkflowValueReference?>? bindGroupTarget=null,Action<IReadOnlyList<WorkflowInterfaceItem>,IReadOnlyDictionary<string,WorkflowValueReference?>>? saveGroupInputs=null,Action<string,System.Text.Json.JsonElement>? setGroupProxyValue=null,Action<string,WorkflowValueReference?>? setGroupProxySource=null,Action<string,IReadOnlyDictionary<string,System.Text.Json.JsonElement>,IReadOnlyList<RoiDocument>?>? setGroupComplexValue=null)
 {
  this.setGroupComplexValue=setGroupComplexValue;this.setGroupProxyValue=setGroupProxyValue;this.setGroupProxySource=setGroupProxySource;this.saveGroupInputs=saveGroupInputs;this.groupRegistry=groupRegistry;this.setGroupParameter=setGroupParameter;this.bindGroupTarget=bindGroupTarget;this.setBreak=setBreak;this.breakSources=breakSources;this.saveGroupConfiguration=saveGroupConfiguration;this.groupSources=groupSources;this.setLoopParameter=setLoopParameter;this.bindLoopParameter=bindLoopParameter;this.owner=owner;this.node=node;this.procedures=procedures;this.configure=configure;this.bind=bind;this.sources=sources;this.run=run;this.stop=stop;this.result=result;InitializeComponent();DisplayOverlay.SizeChanged+=(_,_)=>RenderGroupDisplays(result(),currentGroupImage);Preview.SizeChanged+=(_,_)=>RenderGroupDisplays(result(),currentGroupImage);Heading.Text=node().Name;Loaded+=(_,_)=>Refresh();
 }
 void Safe(Action action){string? error=null;try{action();Error.Text="";}catch(Exception e){error=e.Message;}Refresh();groupRunConfiguration?.Rebuild();groupRunConfiguration?.ShowError(error??"");if(error is not null)Error.Text=error;}
 void Refresh()
 {
  if(!IsInitialized)return;refreshing=true;var ownedBody=procedures().FirstOrDefault(p=>p.Id==node().Call?.ProcedureId&&p.GroupOwner is not null);TargetSelectionPanel.Visibility=ownedBody is null?Visibility.Visible:Visibility.Collapsed;Targets.ItemsSource=procedures().Where(p=>p.Id!=owner().Id&&(p.GroupOwner is null||p.Id==ownedBody?.Id)).ToArray();Targets.SelectedValue=node().Call?.ProcedureId;refreshing=false;
  ((TabItem)ConfigurationTabs.Items[4]).IsEnabled=ownedBody is not null&&setLoopParameter is not null;
  foreach(var tab in ConfigurationTabs.Items.OfType<TabItem>())tab.Content=null;
  if(ConfigurationTabs.SelectedItem is not TabItem selected)return;var call=node().Call;var panel=new StackPanel {Margin=new Thickness(18,0,2,0)};
  if(call is null)panel.Children.Add(new TextBlock {Text="请选择调用流程。",Foreground=System.Windows.Media.Brushes.DimGray});
  else if(ConfigurationTabs.SelectedIndex==0&&ownedBody is not null)BuildGroupInputs(panel,ownedBody);
  else if(ConfigurationTabs.SelectedIndex==0)
  {
   var header=new Grid();foreach(var width in new[]{70d,68d,double.NaN})header.ColumnDefinitions.Add(new(){Width=double.IsNaN(width)?new(1,GridUnitType.Star):new(width)});foreach(var h in new[]{("参数名称",0),("类型",1),("订阅关系",2)}){var text=new TextBlock {Text=h.Item1,FontWeight=FontWeights.Bold,Margin=new Thickness(0,5,0,8)};Grid.SetColumn(text,h.Item2);header.Children.Add(text);}panel.Children.Add(header);
   foreach(var input in call.Inputs){var row=new Grid {Height=32,Margin=new Thickness(0,0,0,6)};row.ColumnDefinitions.Add(new(){Width=new(70)});row.ColumnDefinitions.Add(new(){Width=new(68)});row.ColumnDefinitions.Add(new(){Width=new(1,GridUnitType.Star)});row.Children.Add(new TextBox {Text=input.Name,IsReadOnly=true,ToolTip=input.Name});var kind=new TextBox {Text=TypeName(input.Kind),IsReadOnly=true,Margin=new Thickness(8,0,0,0)};Grid.SetColumn(kind,1);row.Children.Add(kind);var source=new Button {Content=InputLabel(input.Name),ToolTip=InputLabel(input.Name),Tag=input.Name,Margin=new Thickness(8,0,0,0)};Grid.SetColumn(source,2);source.Click+=(_,_)=>BindInput(input);row.Children.Add(source);panel.Children.Add(row);}
  }
  else if(ConfigurationTabs.SelectedIndex==1&&ownedBody is not null)BuildGroupOutputs(panel,ownedBody);
  else if(ConfigurationTabs.SelectedIndex==1){foreach(var output in call.Outputs){var row=new DockPanel {Height=32,Margin=new Thickness(0,0,0,6)};row.Children.Add(new TextBox {Text=output.Name,Width=120,IsReadOnly=true});row.Children.Add(new TextBox {Text=TypeName(output.Kind),Width=80,IsReadOnly=true});row.Children.Add(new TextBlock {Text=result()?.WorkflowOutputs.GetValueOrDefault(output.Name)?.Value is VisionCore.GrayImage gray?$"{gray.Width}×{gray.Height}":result()?.WorkflowOutputs.GetValueOrDefault(output.Name) is {} outValue?FormatGroupValue(outValue):null??"------",VerticalAlignment=VerticalAlignment.Center,Margin=new Thickness(8,0,0,0)});panel.Children.Add(row);}}
  else if(ConfigurationTabs.SelectedIndex==2&&ownedBody is not null)BuildGroupRunParameters(panel,ownedBody);
  else if(ConfigurationTabs.SelectedIndex==2){var target=procedures().FirstOrDefault(p=>p.Id==call.ProcedureId);if(target is not null)foreach(var proxy in target.Configuration.RunParameters)panel.Children.Add(new TextBlock {Text=proxy.Label,Margin=new Thickness(0,6,0,6)});panel.Children.Add(new TextBlock {Text=target is null?"目标流程已不存在，请重新选择。":"进入Group后编辑其运行参数。",TextWrapping=TextWrapping.Wrap,Foreground=System.Windows.Media.Brushes.DimGray});}
  else if(ConfigurationTabs.SelectedIndex==4)BuildLoopPanel(panel,ownedBody is not null);
  else if(ConfigurationTabs.SelectedIndex==3&&ownedBody is not null)BuildGroupDisplays(panel,ownedBody);
  else if(ConfigurationTabs.SelectedIndex==3)panel.Children.Add(new TextBlock {Text="右侧显示当前调用返回的图像和具名结果。",TextWrapping=TextWrapping.Wrap});
  selected.Content=new ScrollViewer {Content=panel,VerticalScrollBarVisibility=ScrollBarVisibility.Auto};RefreshResults();
 }
 void BuildLoopPanel(StackPanel panel,bool owned)
 {
  panel.IsEnabled=owned&&setLoopParameter is not null;
  var settings=GroupLoop.Read(node());var enabled=new System.Windows.Controls.CheckBox{Content="循环使能",IsChecked=settings.Enabled,Margin=new Thickness(0,12,0,12)};
  enabled.Click+=(_,_)=>Safe(()=>setLoopParameter!("EnableLoop",System.Text.Json.JsonSerializer.SerializeToElement(enabled.IsChecked==true)));NativeGroupSwitch(enabled);panel.Children.Add(Labeled("循环使能",enabled));
  if(!settings.Enabled)return;
  foreach(var item in new[]{("StartIndex","循环起始值",settings.Start),("EndIndex","循环结束值",settings.End)}){
   var row=new DockPanel{Margin=new Thickness(0,6,0,6)};row.Children.Add(new TextBlock{Text=item.Item2,Width=89,VerticalAlignment=VerticalAlignment.Center});
   var subscription=owner().Configuration.Subscriptions.FirstOrDefault(b=>b.NodeId==node().Id&&b.IsParameter&&b.Target==item.Item1);
   var button=new Button{Content=subscription is null?item.Item3.ToString():sources(DataKind.Integer).FirstOrDefault(s=>s.Reference==subscription.Source)?.Label??"------"};
   button.Click+=(_,_)=>{var picker=new WorkflowSourcePicker(new[]{new WorkflowSourceOption("","",DataKind.Integer,"解除订阅")}.Concat(sources(DataKind.Integer))){Owner=this};if(picker.ShowDialog()==true&&picker.Selection is {} chosen)Safe(()=>bindLoopParameter!(item.Item1,chosen.Reference));};row.Children.Add(button);panel.Children.Add(row);
  }
  var gap=new DockPanel{Margin=new Thickness(0,6,0,6)};gap.Children.Add(new TextBlock{Text="循环间隔(ms)",Width=89,VerticalAlignment=VerticalAlignment.Center});
  var input=new TextBox{Text=settings.IntervalMs.ToString(),Width=70,Tag="GroupLoopTimeGapValue"};DockPanel.SetDock(input,Dock.Right);gap.Children.Add(input);
  var slider=new Slider{Minimum=0,Maximum=10000,TickFrequency=1,IsSnapToTickEnabled=true,Value=settings.IntervalMs,Tag="GroupLoopTimeGapSlider",VerticalAlignment=VerticalAlignment.Center,Margin=new Thickness(0,0,8,0)};
  gap.Children.Add(slider);bool updating=false;int committedGap=settings.IntervalMs;
  void CommitGap(int value){if(value==committedGap)return;Safe(()=>{setLoopParameter!("LoopTimeGap",System.Text.Json.JsonSerializer.SerializeToElement(value));committedGap=value;});}
  slider.ValueChanged+=(_,_)=>{if(!updating)input.Text=((int)slider.Value).ToString();};
  slider.AddHandler(System.Windows.Controls.Primitives.Thumb.DragCompletedEvent,new System.Windows.Controls.Primitives.DragCompletedEventHandler((_,_)=>CommitGap((int)slider.Value)));
  slider.PreviewMouseLeftButtonUp+=(_,_)=>CommitGap((int)slider.Value);
  slider.LostKeyboardFocus+=(_,_)=>CommitGap((int)slider.Value);
  input.LostKeyboardFocus+=(_,_)=>{if(!int.TryParse(input.Text,out var value)||value is <0 or >10000){Safe(()=>throw new ArgumentException("循环间隔范围为0～10000ms。"));return;}if(value==settings.IntervalMs)return;updating=true;slider.Value=value;updating=false;CommitGap(value);};
  panel.Children.Add(gap);BuildBreakPanel(panel);
 }
 static string TypeName(DataKind kind)=>kind switch{DataKind.GrayImage=>"图像",DataKind.Integer=>"int",DataKind.Number=>"float",DataKind.Text=>"string",DataKind.Circle=>"圆",DataKind.Point=>"点",DataKind.Fixture=>"位置修正",DataKind.Components=>"连通域",DataKind.Region=>"区域",_=>kind.ToString()};
 string InputLabel(string port){var p=owner();var subscription=p.Configuration.Subscriptions.FirstOrDefault(b=>b.NodeId==node().Id&&b.Target==port&&!b.IsParameter);if(subscription is not null){var s=subscription.Source;return s.Kind switch{WorkflowSourceKind.GroupLoopIndex=>"Group.LoopIndex",WorkflowSourceKind.LocalVariable=>"局部变量."+p.Configuration.Variables.Single(v=>v.Id==s.Id).Name,WorkflowSourceKind.WorkflowInput=>"流程输入."+p.Configuration.Inputs.Single(v=>v.Id==s.Id).Name,_=>p.Nodes.Single(n=>n.Id==s.Id).Name+"."+s.Port};}var c=p.Connections.FirstOrDefault(c=>c.ToNode==node().Id&&c.ToPort==port);return c is null?"------":p.Nodes.Single(n=>n.Id==c.FromNode).Name+"."+c.FromPort;}
 void BindInput(WorkflowCallPort port){var picker=new WorkflowSourcePicker(new[]{new WorkflowSourceOption("","",port.Kind,"解除订阅")}.Concat(sources(port.Kind))){Owner=this};if(picker.ShowDialog()==true&&picker.Selection is {} selected)Safe(()=>bind(port.Name,selected.Reference));}
 void RefreshResults(){var session=result();Results.ItemsSource=session?.WorkflowOutputs.Select(v=>new{名称=v.Key,值=v.Value.Value is VisionCore.GrayImage image?$"{image.Width}×{image.Height}":FormatGroupValue(v.Value)}).ToArray();var gray=session?.Status==SessionStatus.Success?session.WorkflowDisplays.Values.SelectMany(v=>v).Select(v=>v.Value).OfType<VisionCore.GrayImage>().LastOrDefault()??session.WorkflowOutputs.Values.Select(v=>v.Value).OfType<VisionCore.GrayImage>().FirstOrDefault():null;Preview.Source=gray is null?null:System.Windows.Media.Imaging.BitmapSource.Create(gray.Width,gray.Height,96,96,System.Windows.Media.PixelFormats.Gray8,null,gray.Pixels.ToArray(),gray.Width);RenderGroupDisplays(session,gray);if(session?.Status is SessionStatus.Failed or SessionStatus.Cancelled or SessionStatus.TimedOut)Error.Text=session.Status+" "+string.Join("；",session.OutputErrors.Concat(session.Nodes.Where(n=>n.ErrorCode is not null).Select(n=>n.ErrorCode+": "+n.Message)));else if(session?.GroupBroke==true)Error.Text="循环已中断 · 索引 "+session.GroupBreakIndex+" · 已执行 "+session.CompletedGroupIterations+" 次";else if(session?.GroupDiagnostics.Count>0)Error.Text=string.Join("；",session.GroupDiagnostics);else Error.Text="";}
 void TargetChanged(object sender,SelectionChangedEventArgs e){if(!refreshing&&IsLoaded&&Targets.SelectedValue is string id)Safe(()=>configure(id));}
 void UpdateClick(object sender,RoutedEventArgs e){if(Targets.SelectedValue is string id)Safe(()=>configure(id));}
 void TabChanged(object sender,SelectionChangedEventArgs e){if(IsLoaded&&ReferenceEquals(e.Source,ConfigurationTabs))Refresh();}
 bool repeatRequested;
 async Task ExecuteGroupRuns(){do{await run();Refresh();if(result()?.Status!=SessionStatus.Success)break;if(repeatRequested)await System.Windows.Threading.Dispatcher.Yield(System.Windows.Threading.DispatcherPriority.Background);}while(repeatRequested);}
 async void StartGroupRun(bool repeat){if(running)return;running=true;repeatRequested=repeat;Targets.IsEnabled=ConfigurationTabs.IsEnabled=RunOnce.IsEnabled=Repeat.IsEnabled=false;Stop.IsEnabled=true;try{Error.Text="";runningTask=ExecuteGroupRuns();await runningTask;}catch(Exception error){RefreshResults();Error.Text=error is OperationCanceledException?"运行已停止":error.Message;}finally{repeatRequested=false;running=false;Targets.IsEnabled=ConfigurationTabs.IsEnabled=RunOnce.IsEnabled=Repeat.IsEnabled=true;Stop.IsEnabled=false;}}
 void RunClick(object sender,RoutedEventArgs e)=>StartGroupRun(false);
 void RepeatClick(object sender,RoutedEventArgs e)=>StartGroupRun(true);
 void StopClick(object sender,RoutedEventArgs e){repeatRequested=false;} void EnterClick(object sender,RoutedEventArgs e){if(node().Call is {} call&&procedures().Any(p=>p.Id==call.ProcedureId)){EnterProcedureId=call.ProcedureId;Close();}}
 void CloseClick(object sender,RoutedEventArgs e)=>Close();
 void DragTitle(object sender,MouseButtonEventArgs e){if(e.ChangedButton==MouseButton.Left)DragMove();}
  public async Task StopAndCloseAsync(){repeatRequested=false;stop();try{if(runningTask is {} active)await active;}catch(Exception error){Error.Text=error.Message;}closeReady=true;Close();}
 protected override async void OnClosing(System.ComponentModel.CancelEventArgs e){base.OnClosing(e);if(e.Cancel||closeReady||!running)return;e.Cancel=true;if(closePending)return;closePending=true;repeatRequested=false;stop();try{if(runningTask is {} active)await active;}catch(Exception error){Error.Text=error.Message;}finally{closeReady=true;_ = Dispatcher.BeginInvoke(new Action(Close));}}
 protected override void OnClosed(EventArgs e){if(running)stop();base.OnClosed(e);}
}
