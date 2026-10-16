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
 readonly Func<Task> run;readonly Action stop;readonly Func<ExecutionSessionResult?> result;bool refreshing,running;
 public string? EnterProcedureId{get;private set;}
 public SubprogramCallWindow(Func<ProcedureDocument> owner,Func<NodeDocument> node,Func<IReadOnlyList<ProcedureDocument>> procedures,Action<string> configure,Action<string,WorkflowValueReference?> bind,Func<DataKind,IEnumerable<WorkflowSourceOption>> sources,Func<Task> run,Action stop,Func<ExecutionSessionResult?> result)
 {
  this.owner=owner;this.node=node;this.procedures=procedures;this.configure=configure;this.bind=bind;this.sources=sources;this.run=run;this.stop=stop;this.result=result;InitializeComponent();Heading.Text=node().Name;Loaded+=(_,_)=>Refresh();
 }
 void Safe(Action action){try{action();Error.Text="";}catch(Exception e){Error.Text=e.Message;}Refresh();}
 void Refresh()
 {
  if(!IsInitialized)return;refreshing=true;var ownedBody=procedures().FirstOrDefault(p=>p.Id==node().Call?.ProcedureId&&p.GroupOwner is not null);TargetSelectionPanel.Visibility=ownedBody is null?Visibility.Visible:Visibility.Collapsed;Targets.ItemsSource=procedures().Where(p=>p.Id!=owner().Id&&(p.GroupOwner is null||p.Id==ownedBody?.Id)).ToArray();Targets.SelectedValue=node().Call?.ProcedureId;refreshing=false;
  foreach(var tab in ConfigurationTabs.Items.OfType<TabItem>())tab.Content=null;
  if(ConfigurationTabs.SelectedItem is not TabItem selected)return;var call=node().Call;var panel=new StackPanel {Margin=new Thickness(18,0,2,0)};
  if(call is null)panel.Children.Add(new TextBlock {Text="请选择调用流程。",Foreground=System.Windows.Media.Brushes.DimGray});
  else if(ConfigurationTabs.SelectedIndex==0)
  {
   var header=new Grid();foreach(var width in new[]{70d,68d,double.NaN})header.ColumnDefinitions.Add(new(){Width=double.IsNaN(width)?new(1,GridUnitType.Star):new(width)});foreach(var h in new[]{("参数名称",0),("类型",1),("订阅关系",2)}){var text=new TextBlock {Text=h.Item1,FontWeight=FontWeights.Bold,Margin=new Thickness(0,5,0,8)};Grid.SetColumn(text,h.Item2);header.Children.Add(text);}panel.Children.Add(header);
   foreach(var input in call.Inputs){var row=new Grid {Height=32,Margin=new Thickness(0,0,0,6)};row.ColumnDefinitions.Add(new(){Width=new(70)});row.ColumnDefinitions.Add(new(){Width=new(68)});row.ColumnDefinitions.Add(new(){Width=new(1,GridUnitType.Star)});row.Children.Add(new TextBox {Text=input.Name,IsReadOnly=true,ToolTip=input.Name});var kind=new TextBox {Text=TypeName(input.Kind),IsReadOnly=true,Margin=new Thickness(8,0,0,0)};Grid.SetColumn(kind,1);row.Children.Add(kind);var source=new Button {Content=InputLabel(input.Name),ToolTip=InputLabel(input.Name),Tag=input.Name,Margin=new Thickness(8,0,0,0)};Grid.SetColumn(source,2);source.Click+=(_,_)=>BindInput(input);row.Children.Add(source);panel.Children.Add(row);}
  }
  else if(ConfigurationTabs.SelectedIndex==1){foreach(var output in call.Outputs){var row=new DockPanel {Height=32,Margin=new Thickness(0,0,0,6)};row.Children.Add(new TextBox {Text=output.Name,Width=120,IsReadOnly=true});row.Children.Add(new TextBox {Text=TypeName(output.Kind),Width=80,IsReadOnly=true});row.Children.Add(new TextBlock {Text=result()?.WorkflowOutputs.GetValueOrDefault(output.Name)?.Value is VisionCore.GrayImage gray?$"{gray.Width}×{gray.Height}":result()?.WorkflowOutputs.GetValueOrDefault(output.Name)?.Value.ToString()??"------",VerticalAlignment=VerticalAlignment.Center,Margin=new Thickness(8,0,0,0)});panel.Children.Add(row);}}
  else if(ConfigurationTabs.SelectedIndex==2){var target=procedures().FirstOrDefault(p=>p.Id==call.ProcedureId);if(target is not null)foreach(var proxy in target.Configuration.RunParameters)panel.Children.Add(new TextBlock {Text=proxy.Label,Margin=new Thickness(0,6,0,6)});panel.Children.Add(new TextBlock {Text=target is null?"目标流程已不存在，请重新选择。":"进入Group后编辑其运行参数。",TextWrapping=TextWrapping.Wrap,Foreground=System.Windows.Media.Brushes.DimGray});}
  else if(ConfigurationTabs.SelectedIndex==3)panel.Children.Add(new TextBlock {Text="右侧显示当前调用返回的图像和具名结果。",TextWrapping=TextWrapping.Wrap});
  selected.Content=new ScrollViewer {Content=panel,VerticalScrollBarVisibility=ScrollBarVisibility.Auto};RefreshResults();
 }
 static string TypeName(DataKind kind)=>kind switch{DataKind.GrayImage=>"图像",DataKind.Integer=>"int",DataKind.Number=>"float",DataKind.Text=>"string",_=>kind.ToString()};
 string InputLabel(string port){var p=owner();var subscription=p.Configuration.Subscriptions.FirstOrDefault(b=>b.NodeId==node().Id&&b.Target==port&&!b.IsParameter);if(subscription is not null){var s=subscription.Source;return s.Kind switch{WorkflowSourceKind.LocalVariable=>"局部变量."+p.Configuration.Variables.Single(v=>v.Id==s.Id).Name,WorkflowSourceKind.WorkflowInput=>"流程输入."+p.Configuration.Inputs.Single(v=>v.Id==s.Id).Name,_=>p.Nodes.Single(n=>n.Id==s.Id).Name+"."+s.Port};}var c=p.Connections.FirstOrDefault(c=>c.ToNode==node().Id&&c.ToPort==port);return c is null?"------":p.Nodes.Single(n=>n.Id==c.FromNode).Name+"."+c.FromPort;}
 void BindInput(WorkflowCallPort port){var picker=new WorkflowSourcePicker(new[]{new WorkflowSourceOption("","",port.Kind,"解除订阅")}.Concat(sources(port.Kind))){Owner=this};if(picker.ShowDialog()==true&&picker.Selection is {} selected)Safe(()=>bind(port.Name,selected.Reference));}
 void RefreshResults(){var session=result();Results.ItemsSource=session?.WorkflowOutputs.Select(v=>new{名称=v.Key,值=v.Value.Value is VisionCore.GrayImage image?$"{image.Width}×{image.Height}":v.Value.Value.ToString()}).ToArray();var gray=session?.WorkflowOutputs.Values.Select(v=>v.Value).OfType<VisionCore.GrayImage>().FirstOrDefault();Preview.Source=gray is null?null:System.Windows.Media.Imaging.BitmapSource.Create(gray.Width,gray.Height,96,96,System.Windows.Media.PixelFormats.Gray8,null,gray.Pixels.ToArray(),gray.Width);if(session?.Status is SessionStatus.Failed or SessionStatus.Cancelled or SessionStatus.TimedOut)Error.Text=session.Status+" "+string.Join("；",session.OutputErrors);}
 void TargetChanged(object sender,SelectionChangedEventArgs e){if(!refreshing&&IsLoaded&&Targets.SelectedValue is string id)Safe(()=>configure(id));}
 void UpdateClick(object sender,RoutedEventArgs e){if(Targets.SelectedValue is string id)Safe(()=>configure(id));}
 void TabChanged(object sender,SelectionChangedEventArgs e){if(IsLoaded&&ReferenceEquals(e.Source,ConfigurationTabs))Refresh();}
 async void RunClick(object sender,RoutedEventArgs e){if(running)return;running=true;Targets.IsEnabled=ConfigurationTabs.IsEnabled=RunOnce.IsEnabled=false;Stop.IsEnabled=true;try{Error.Text="";await run();Refresh();}catch(Exception error){RefreshResults();Error.Text=error.Message;}finally{running=false;Targets.IsEnabled=ConfigurationTabs.IsEnabled=RunOnce.IsEnabled=true;Stop.IsEnabled=false;}}
 void StopClick(object sender,RoutedEventArgs e)=>stop();
 void EnterClick(object sender,RoutedEventArgs e){if(node().Call is {} call&&procedures().Any(p=>p.Id==call.ProcedureId)){EnterProcedureId=call.ProcedureId;Close();}}
 void CloseClick(object sender,RoutedEventArgs e)=>Close();
 void DragTitle(object sender,MouseButtonEventArgs e){if(e.ChangedButton==MouseButton.Left)DragMove();}
 protected override void OnClosed(EventArgs e){if(running)stop();base.OnClosed(e);}
}
