using HorizontalAlignment=System.Windows.HorizontalAlignment;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisionFramework;
using Button=System.Windows.Controls.Button;
using TextBox=System.Windows.Controls.TextBox;
using ComboBox=System.Windows.Controls.ComboBox;
using Orientation=System.Windows.Controls.Orientation;
using ImageSource=System.Windows.Media.ImageSource;
namespace VisionEditor;
public partial class WorkflowConfigurationWindow:Window
{
 public event Action? RepeatRequested;
 public event Action? StopRequested;
 public void SetExecutionState(bool running,bool ownsRun,bool locked){ConfigurationTabs.IsEnabled=!running&&!locked;RunOnce.IsEnabled=Repeat.IsEnabled=!running;Stop.IsEnabled=running&&ownsRun;}
 readonly Action<string,string,WorkflowValueReference?>? bindInput;readonly Func<ProcedureDocument> read;readonly Action<WorkflowConfiguration> save;readonly Func<DataKind?,IEnumerable<WorkflowSourceOption>> sources;readonly ModuleRegistry registry;readonly Action<string,string,JsonElement> setParameter;readonly Func<Task> execute;readonly Func<string,string,PortValue?> result;
 public WorkflowConfigurationWindow(Func<ProcedureDocument> read,Action<WorkflowConfiguration> save,Func<DataKind?,IEnumerable<WorkflowSourceOption>> sources,ModuleRegistry registry,Action<string,string,JsonElement> setParameter,Func<Task> execute,Func<string,string,PortValue?> result,Action<string,string,WorkflowValueReference?>? bindInput=null)
 {
  this.bindInput=bindInput;this.read=read;this.save=save;this.sources=sources;this.registry=registry;this.setParameter=setParameter;this.execute=execute;this.result=result;InitializeComponent();Heading.Text=read().Name;Loaded+=(_,_)=>Refresh();
 }
 static string TypeName(DataKind kind)=>kind switch{DataKind.GrayImage=>"图像",DataKind.Integer=>"int",DataKind.Number=>"float",DataKind.Text=>"string",DataKind.Boolean=>"bool",DataKind.Circle=>"圆",_=>"连通域"};
 public void Refresh()
 {
  if(!IsInitialized)return;var c=read().Configuration;
  foreach(var tab in ConfigurationTabs.Items.OfType<TabItem>().Take(4))tab.Content=null;
  if(ConfigurationTabs.SelectedItem is not TabItem selected)return;
  int index=ConfigurationTabs.SelectedIndex;selected.Content=index==2?BuildRunParameters(c):BuildInterfaces(index==0?"input":index==1?"output":"display");RefreshResults();
 }
 void Safe(Action action){try{action();Error.Text="";}catch(Exception error){Error.Text=error.Message;}}
 IReadOnlyList<WorkflowInterfaceItem> Items(string section)=>section switch{"input"=>read().Configuration.Inputs,"output"=>read().Configuration.Outputs,_=>read().Configuration.Displays};
 void SaveItems(string section,IEnumerable<WorkflowInterfaceItem> items){var c=read().Configuration;save(section switch{"input"=>c with{Inputs=items.ToArray(),Subscriptions=c.Subscriptions.Where(b=>b.Source.Kind!=WorkflowSourceKind.WorkflowInput||items.Any(i=>i.Id==b.Source.Id)).ToArray()},"output"=>c with{Outputs=items.ToArray()},_=>c with{Displays=items.ToArray()}});}
 FrameworkElement BuildInterfaces(string section)
 {
  var panel=new StackPanel {Margin=new Thickness(18,0,2,0)};
  var header=RowGrid();header.Height=32;foreach(var entry in new[]{("参数名称",0),("类型",1),(section=="display"?"关联":"订阅关系",2)}){var label=new TextBlock {Text=entry.Item1,FontWeight=FontWeights.Bold,VerticalAlignment=VerticalAlignment.Center,Foreground=System.Windows.Media.Brushes.DimGray};Grid.SetColumn(label,entry.Item2);header.Children.Add(label);}panel.Children.Add(header);
  foreach(var item in Items(section))
  {
   var row=RowGrid();row.Margin=new Thickness(0,0,0,6);row.Height=32;
   var name=new TextBox {Text=item.Name,MaxLength=32};name.LostFocus+=(_,_)=>Safe(()=>{SaveItems(section,Items(section).Select(p=>p.Id==item.Id?p with{Name=name.Text}:p));});row.Children.Add(name);
   var type=new TextBox {Text=TypeName(item.Kind),IsReadOnly=true,Margin=new Thickness(8,0,0,0)};Grid.SetColumn(type,1);row.Children.Add(type);
   var relation=sources(item.Kind).FirstOrDefault(s=>s.NodeId==item.NodeId&&s.Port==item.Port);
   var source=new Button {Content=relation?.Label??"------",Margin=new Thickness(8,0,0,0),ToolTip=relation?.Label};Grid.SetColumn(source,2);source.Click+=(_,_)=>BindInterface(section,item);row.Children.Add(source);
   var delete=new Button {Content="×",Padding=new Thickness(0),Margin=new Thickness(3,0,0,0)};Grid.SetColumn(delete,3);delete.Click+=(_,_)=>Safe(()=>{SaveItems(section,Items(section).Where(p=>p.Id!=item.Id));Refresh();});row.Children.Add(delete);panel.Children.Add(row);
  }
  var addRow=new StackPanel {Orientation=Orientation.Horizontal,Margin=new Thickness(0,6,0,0)};var types=new ComboBox {Width=68};types.Items.Add(new ComboBoxItem {Content="多选",Tag="multi"});foreach(var kind in section=="display"?new[]{DataKind.GrayImage}:new[]{DataKind.Integer,DataKind.Number,DataKind.Text,DataKind.GrayImage})types.Items.Add(new ComboBoxItem {Content=TypeName(kind),Tag=kind});foreach(var label in section=="display"?new[]{"点","线","圆","矩形","椭圆"}:new[]{"byte","pointset","像素图像","矩形ROI","点","线","坐标系","圆环ROI","轮廓点","矩形","分类信息","多边形ROI","姿态","椭圆"})types.Items.Add(new ComboBoxItem {Content=label,IsEnabled=false,ToolTip="独立复合数据类型尚未接入"});types.SelectedIndex=0;
  var add=new Button {Content="＋",Width=32,Height=32,Margin=new Thickness(8,0,0,0)};add.Click+=(_,_)=>Safe(()=>{if(types.SelectedItem is not ComboBoxItem option)return;if(option.Tag is DataKind kind)AddInterface(section,kind,null);else{var picker=new WorkflowSourcePicker(sources(null),true){Owner=this};if(picker.ShowDialog()==true){var before=Items(section).ToList();foreach(var selection in picker.Selections){string stem=picker.IncludeModuleName?selection.Label.Replace(".","_"):selection.Port;stem=stem.Length>28?stem[..28]:stem;string name=stem;int n=0;while(before.Any(p=>p.Name==name))name=stem+n++;before.Add(new(Guid.NewGuid().ToString("N"),name,selection.Kind,selection.NodeId,selection.Port));}SaveItems(section,before);}}Refresh();});addRow.Children.Add(types);addRow.Children.Add(add);panel.Children.Add(addRow);return new ScrollViewer {Content=panel,VerticalScrollBarVisibility=ScrollBarVisibility.Auto,HorizontalScrollBarVisibility=ScrollBarVisibility.Disabled};
 }
 static Grid RowGrid(){var row=new Grid();foreach(double width in new[]{70d,68d,double.NaN,42d})row.ColumnDefinitions.Add(new ColumnDefinition {Width=double.IsNaN(width)?new GridLength(1,GridUnitType.Star):new GridLength(width)});return row;}
 void AddInterface(string section,DataKind kind,WorkflowSourceOption? source)
 {
  var list=Items(section);string prefix=section=="input"?"in":section=="output"?"out":"display";int n=0;while(list.Any(p=>p.Name==prefix+n))n++;SaveItems(section,list.Append(new(Guid.NewGuid().ToString("N"),prefix+n,kind,source?.NodeId,source?.Port)));
 }
 void BindInterface(string section,WorkflowInterfaceItem item)
 {
  var none=new WorkflowSourceOption("","",item.Kind,"解除订阅");var picker=new WorkflowSourcePicker(new[]{none}.Concat(sources(item.Kind))){Owner=this};if(picker.ShowDialog()==true&&picker.Selection is {} selected)Safe(()=>{SaveItems(section,Items(section).Select(p=>p.Id==item.Id?p with{NodeId=selected.NodeId==""?null:selected.NodeId,Port=selected.Port==""?null:selected.Port}:p));Refresh();});
 }
 FrameworkElement BuildRunParameters(WorkflowConfiguration c)
 {
  var panel=new StackPanel {Margin=new Thickness(10,0,10,0)};var toolbar=new DockPanel {Height=38};var edit=new Button {Content="编辑",Width=80,HorizontalAlignment=HorizontalAlignment.Right};DockPanel.SetDock(edit,Dock.Right);edit.Click+=(_,_)=>AddRunParameter();toolbar.Children.Add(edit);toolbar.Children.Add(new TextBlock {Text="运行参数",FontWeight=FontWeights.Bold,VerticalAlignment=VerticalAlignment.Center,Foreground=System.Windows.Media.Brushes.DimGray});panel.Children.Add(toolbar);
  foreach(var proxy in c.RunParameters)
  {
   var node=read().Nodes.First(n=>n.Id==proxy.NodeId);var descriptor=registry.GetDescriptor(node);var subscription=c.Subscriptions.FirstOrDefault(b=>b.NodeId==node.Id&&b.Target==proxy.Parameter&&b.IsParameter!=proxy.IsInput);
   var row=new DockPanel {Margin=new Thickness(0,3,0,3)};var delete=new Button {Content="×",Width=28};DockPanel.SetDock(delete,Dock.Right);delete.Click+=(_,_)=>Safe(()=>{save(read().Configuration with{RunParameters=read().Configuration.RunParameters.Where(p=>p.Id!=proxy.Id).ToArray()});Refresh();});row.Children.Add(delete);
   var subscribe=new Button {Content="订阅",Width=48,Tag=proxy.Id};DockPanel.SetDock(subscribe,Dock.Right);subscribe.Click+=(_,_)=>BindRunParameter(proxy);row.Children.Add(subscribe);
   row.Children.Add(new TextBlock {Text=node.Name+"."+(proxy.IsInput?proxy.Parameter:WorkflowPresentation.Parameter(node.ModuleId,proxy.Parameter)),Width=155,TextTrimming=TextTrimming.CharacterEllipsis,VerticalAlignment=VerticalAlignment.Center,Foreground=System.Windows.Media.Brushes.DimGray});
   if(proxy.IsInput){row.Children.Add(new TextBlock {Text=subscription is null?"------":SourceLabel(subscription.Source),VerticalAlignment=VerticalAlignment.Center,TextTrimming=TextTrimming.CharacterEllipsis});}
   else
   {
    var spec=descriptor.Parameters.First(p=>p.Name==proxy.Parameter);var value=new TextBox {Text=subscription is not null?SourceLabel(subscription.Source):node.Parameters.TryGetValue(proxy.Parameter,out var v)?v.ToString():spec.DefaultValue?.ToString()??"",IsReadOnly=subscription is not null,ToolTip=subscription is null?null:SourceLabel(subscription.Source)+" · 运行时读取订阅值"};
    value.LostFocus+=(_,_)=>Safe(()=>{if(read().Configuration.Subscriptions.Any(b=>b.NodeId==node.Id&&b.Target==proxy.Parameter&&b.IsParameter))return;object parsed=spec.Kind switch{ParameterKind.Text=>value.Text,ParameterKind.Boolean=>bool.Parse(value.Text),ParameterKind.Integer=>int.Parse(value.Text,System.Globalization.CultureInfo.InvariantCulture),_=>double.Parse(value.Text,System.Globalization.CultureInfo.InvariantCulture)};setParameter(node.Id,proxy.Parameter,JsonSerializer.SerializeToElement(parsed));});row.Children.Add(value);
   }
   panel.Children.Add(row);
  }
  return new ScrollViewer {Content=panel,VerticalScrollBarVisibility=ScrollBarVisibility.Auto};
 }
 string SourceLabel(WorkflowValueReference reference)=>reference.Kind switch{WorkflowSourceKind.GroupLoopIndex=>"Group.LoopIndex",WorkflowSourceKind.LocalVariable=>"局部变量."+read().Configuration.Variables.First(v=>v.Id==reference.Id).Name,WorkflowSourceKind.WorkflowInput=>"流程输入."+read().Configuration.Inputs.First(v=>v.Id==reference.Id).Name,_=>sources(null).FirstOrDefault(s=>s.NodeId==reference.Id&&s.Port==reference.Port)?.Label??"来源已失效"};
 IEnumerable<WorkflowSourceOption> SubscriptionSources(DataKind kind)
 {
  var p=read();
  if(kind==DataKind.Integer&&p.GroupOwner is not null)yield return new("group:loop","LoopIndex",kind,"Group.LoopIndex"){Reference=new(WorkflowSourceKind.GroupLoopIndex,"LoopIndex")};
  foreach(var v in p.Configuration.Variables.Where(v=>WorkflowSubscriptions.VariableKind(v.Type)==kind))yield return new("variable:"+v.Id,v.Name,kind,"局部变量."+v.Name){Reference=new(WorkflowSourceKind.LocalVariable,v.Id)};
  foreach(var v in p.Configuration.Inputs.Where(v=>v.Kind==kind))yield return new("input:"+v.Id,v.Name,kind,"流程输入."+v.Name){Reference=new(WorkflowSourceKind.WorkflowInput,v.Id)};
  foreach(var s in sources(kind))yield return s with{Reference=new(WorkflowSourceKind.NodeOutput,s.NodeId,s.Port)};
 }
 void SaveSubscription(string node,string target,bool parameter,WorkflowValueReference? source)
 {
  if(!parameter&&bindInput is not null){bindInput(node,target,source);Refresh();return;}var c=read().Configuration;var remaining=c.Subscriptions.Where(b=>b.NodeId!=node||b.Target!=target||b.IsParameter!=parameter);var proposed=c with{Subscriptions=(source is null?remaining:remaining.Append(new(node,target,parameter,source))).ToArray()};var invalid=GraphValidator.Validate(read() with{Configuration=proposed},registry).Issues.FirstOrDefault(i=>i.Code is "WorkflowConfiguration" or "DataExecutionOrder" or "Cycle");if(invalid is not null)throw new InvalidOperationException(invalid.Message);save(proposed);Refresh();
 }
 void BindRunParameter(WorkflowRunParameter proxy)
 {
  var node=read().Nodes.Single(n=>n.Id==proxy.NodeId);var descriptor=registry.GetDescriptor(node);var kind=proxy.IsInput?descriptor.Inputs.Single(p=>p.Name==proxy.Parameter).Kind:WorkflowSubscriptions.ParameterType(descriptor.Parameters.Single(p=>p.Name==proxy.Parameter).Kind);
  var picker=new WorkflowSourcePicker(new[]{new WorkflowSourceOption("","",kind,"解除订阅")}.Concat(SubscriptionSources(kind))){Owner=this};if(picker.ShowDialog()==true&&picker.Selection is {} selected)Safe(()=>SaveSubscription(node.Id,proxy.Parameter,!proxy.IsInput,selected.Reference));
 }
 void AddRunParameter()
 {
  var options=read().Nodes.SelectMany(n=>registry.GetDescriptor(n).Parameters.Select(p=>new WorkflowSourceOption(n.Id,p.Name,DataKind.Text,n.Name+"."+WorkflowPresentation.Parameter(n.ModuleId,p.Name))).Concat(registry.GetDescriptor(n).Inputs.Select(p=>new WorkflowSourceOption(n.Id,p.Name,p.Kind,n.Name+"."+p.Name){IsInput=true}))).ToArray();var picker=new WorkflowSourcePicker(options){Owner=this};if(picker.ShowDialog()==true&&picker.Selection is {} selected)Safe(()=>{var c=read().Configuration;if(!c.RunParameters.Any(p=>p.NodeId==selected.NodeId&&p.Parameter==selected.Port&&p.IsInput==selected.IsInput))save(c with{RunParameters=c.RunParameters.Append(new(Guid.NewGuid().ToString("N"),selected.NodeId,selected.Port,selected.Label,selected.IsInput)).ToArray()});Refresh();});
 }
 public void RefreshResults()
 {
  var c=read().Configuration;var entries=c.Outputs.Concat(c.Displays).ToArray();Results.ItemsSource=entries.Select(p=>new {名称=p.Name,类型=TypeName(p.Kind),值=p.NodeId is null?"":result(p.NodeId,p.Port!)?.Value is VisionCore.GrayImage image?$"{image.Width}×{image.Height}":result(p.NodeId,p.Port!)?.Value.ToString()??""}).ToArray();var gray=entries.Select(p=>p.NodeId is null?null:result(p.NodeId,p.Port!)?.Value).OfType<VisionCore.GrayImage>().FirstOrDefault();Preview.Source=gray is null?null:System.Windows.Media.Imaging.BitmapSource.Create(gray.Width,gray.Height,96,96,System.Windows.Media.PixelFormats.Gray8,null,gray.Pixels.ToArray(),gray.Width);
 }
 async void RunClick(object s,RoutedEventArgs e){RunOnce.IsEnabled=false;try{await execute();RefreshResults();}catch(Exception error){Error.Text=error.Message;}finally{RunOnce.IsEnabled=true;}}
 void RepeatClick(object s,RoutedEventArgs e)=>RepeatRequested?.Invoke();
 void StopClick(object s,RoutedEventArgs e)=>StopRequested?.Invoke();
 void TabChanged(object s,SelectionChangedEventArgs e){if(IsLoaded&&ReferenceEquals(e.Source,ConfigurationTabs))Refresh();}
 void CloseClick(object s,RoutedEventArgs e)=>Close();void DragTitle(object s,MouseButtonEventArgs e){if(e.ChangedButton==MouseButton.Left)DragMove();}
 protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
 {
  if(Keyboard.FocusedElement is TextBox text&&Window.GetWindow(text)==this){text.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();text.RaiseEvent(new RoutedEventArgs(UIElement.LostFocusEvent));}
  base.OnClosing(e);
 }
}
