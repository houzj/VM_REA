using Binding=System.Windows.Data.Binding;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using VisionFramework;
using Button=System.Windows.Controls.Button;
using TextBox=System.Windows.Controls.TextBox;
using ComboBox=System.Windows.Controls.ComboBox;
using TabControl=System.Windows.Controls.TabControl;
using DataGrid=System.Windows.Controls.DataGrid;
using MenuItem=System.Windows.Controls.MenuItem;
using ContextMenu=System.Windows.Controls.ContextMenu;
using MessageBox=System.Windows.MessageBox;
namespace VisionEditor;
public partial class WorkflowLocalVariablesWindow:Window
{
 readonly ModuleRegistry? registry;readonly Func<ProcedureDocument> read;readonly Action<WorkflowConfiguration> save;readonly Func<DataKind?,IEnumerable<WorkflowSourceOption>> sources;readonly Func<string,string,string?> liveValue;
 bool refreshing;int page=1,pageSize=12;string? group;
 sealed class Row
 {
  public required string Id{get;init;}public int Index{get;init;}public string Name{get;set;}="";public string Type{get;set;}="int";public string Value{get;set;}="0";public string Remark{get;set;}="";public string Binding{get;set;}="------";public string BindingValue{get;set;}="";public bool IsBound{get;set;}public string References{get;set;}="------";
 }
 public WorkflowLocalVariablesWindow(Func<ProcedureDocument> read,Action<WorkflowConfiguration> save,Func<DataKind?,IEnumerable<WorkflowSourceOption>> sources,Func<string,string,string?> liveValue,ModuleRegistry? registry=null)
 {
  this.registry=registry;this.read=read;this.save=save;this.sources=sources;this.liveValue=liveValue;InitializeComponent();BuildColumns();
  if(read().Configuration.Groups.Count==0)save(read().Configuration with{Groups=[new(Guid.NewGuid().ToString("N"),"分组0")]});
  Refresh();
 }
 void BuildColumns()
 {
  Variables.Columns.Add(new DataGridTextColumn {Header="序号",Binding=new Binding("Index"),Width=60,IsReadOnly=true});
  TextColumn("名称","Name",135);TextColumn("备注","Remark",130);
  var type=new FrameworkElementFactory(typeof(ComboBox));type.SetValue(ComboBox.ItemsSourceProperty,new[]{"int","float","string","byte"});type.SetBinding(ComboBox.SelectedItemProperty,new Binding("Type"){Mode=BindingMode.TwoWay,UpdateSourceTrigger=UpdateSourceTrigger.PropertyChanged});type.SetValue(FrameworkElement.MarginProperty,new Thickness(5,0,5,0));type.AddHandler(ComboBox.SelectionChangedEvent,new SelectionChangedEventHandler(TypeChanged));Variables.Columns.Add(new DataGridTemplateColumn {Header="类型",Width=95,CellTemplate=new DataTemplate {VisualTree=type}});
  TextColumn("初始值","Value",158);
  var references=new FrameworkElementFactory(typeof(Button));references.SetBinding(Button.ContentProperty,new Binding("References"));references.SetBinding(Button.ToolTipProperty,new Binding("References"));references.SetValue(System.Windows.Controls.Control.ForegroundProperty,System.Windows.Media.Brushes.DodgerBlue);references.SetValue(System.Windows.Controls.Control.BackgroundProperty,System.Windows.Media.Brushes.Transparent);references.SetValue(System.Windows.Controls.Control.BorderThicknessProperty,new Thickness(0));references.SetValue(System.Windows.Controls.Control.HorizontalContentAlignmentProperty,System.Windows.HorizontalAlignment.Left);references.AddHandler(Button.ClickEvent,new RoutedEventHandler(BindTargets));Variables.Columns.Add(new DataGridTemplateColumn {Header="引用位置",Width=150,CellTemplate=new DataTemplate {VisualTree=references}});
  var binding=new FrameworkElementFactory(typeof(Button));binding.SetBinding(Button.ContentProperty,new Binding("Binding"));binding.SetBinding(Button.ToolTipProperty,new Binding("BindingValue"));binding.AddHandler(Button.ClickEvent,new RoutedEventHandler(BindSource));Variables.Columns.Add(new DataGridTemplateColumn {Header="绑定模块结果",Width=160,CellTemplate=new DataTemplate {VisualTree=binding}});
  var delete=new FrameworkElementFactory(typeof(Button));delete.SetValue(Button.ContentProperty,"×");delete.AddHandler(Button.ClickEvent,new RoutedEventHandler(DeleteVariable));Variables.Columns.Add(new DataGridTemplateColumn {Header="操作",Width=75,CellTemplate=new DataTemplate {VisualTree=delete}});
 }
 void TextColumn(string header,string property,int width)
 {
  var text=new FrameworkElementFactory(typeof(TextBox));text.SetValue(FrameworkElement.MarginProperty,new Thickness(5,0,5,0));if(property=="Value")text.SetBinding(TextBox.IsReadOnlyProperty,new Binding("IsBound"));text.SetBinding(TextBox.TextProperty,new Binding(property){Mode=BindingMode.TwoWay,UpdateSourceTrigger=UpdateSourceTrigger.LostFocus});text.AddHandler(UIElement.LostFocusEvent,new RoutedEventHandler(CommitCell));Variables.Columns.Add(new DataGridTemplateColumn {Header=header,Width=width,CellTemplate=new DataTemplate {VisualTree=text}});
 }
 void Safe(Action action){try{action();Error.Text="";}catch(Exception error){Error.Text=error.Message;RefreshRows();}}
 void CommitCell(object sender,RoutedEventArgs e){if(refreshing||sender is not TextBox {DataContext:Row row})return;Dispatcher.BeginInvoke(new Action(()=>Safe(()=>Commit(row))));}
 void Commit(Row row)
 {
  var c=read().Configuration;var original=c.Variables.FirstOrDefault(v=>v.Id==row.Id);if(original is null)return;string type=row.Type;string value=original.SourceNode is not null&&type==original.Type?original.Value:row.Value;
  if(type!=original.Type){if(type=="string")value="";else if(type=="byte")value="00";else{try{value=WorkflowConfiguration.NormalizeValue(type,value);}catch{value=type=="float"?"0.0":type=="byte"?"00":"0";}}}
  var updated=original with{Name="%"+row.Name.Trim('%')+"%",Type=type,Value=WorkflowConfiguration.NormalizeValue(type,value),Remark=row.Remark,SourceNode=type==original.Type?original.SourceNode:null,SourcePort=type==original.Type?original.SourcePort:null};
  save(c with{Variables=c.Variables.Select(v=>v.Id==row.Id?updated:v).ToArray(),Subscriptions=type==original.Type?c.Subscriptions:c.Subscriptions.Where(b=>b.Source.Kind!=WorkflowSourceKind.LocalVariable||b.Source.Id!=row.Id).ToArray()});
 }
 void TypeChanged(object sender,SelectionChangedEventArgs e){if(refreshing||sender is not ComboBox {DataContext:Row row}||read().Configuration.Variables.FirstOrDefault(v=>v.Id==row.Id)?.Type==row.Type)return;Safe(()=>{Commit(row);RefreshRows();});}
 void Refresh()
 {
  refreshing=true;var c=read().Configuration;group=c.Groups.Any(g=>g.Id==group)?group:c.Groups.FirstOrDefault()?.Id;GroupTabs.Items.Clear();
  foreach(var g in c.Groups){var tab=new TabItem {Header=g.Name,Tag=g.Id,IsSelected=g.Id==group};var menu=new ContextMenu();var rename=new MenuItem {Header="重命名"};rename.Click+=(_,_)=>RenameGroup(tab,g);var delete=new MenuItem {Header="删除"};delete.Click+=(_,_)=>{if(MessageBox.Show(this,"确认删除分组及其变量？","",MessageBoxButton.YesNo)==MessageBoxResult.Yes)Safe(()=>{var c=read().Configuration;save(c with{Groups=c.Groups.Where(x=>x.Id!=g.Id).ToArray(),Variables=c.Variables.Where(v=>v.GroupId!=g.Id).ToArray(),Subscriptions=c.Subscriptions.Where(b=>b.Source.Kind!=WorkflowSourceKind.LocalVariable||!c.Variables.Any(v=>v.GroupId==g.Id&&v.Id==b.Source.Id)).ToArray()});Refresh();});};menu.Items.Add(rename);menu.Items.Add(delete);tab.ContextMenu=menu;GroupTabs.Items.Add(tab);}
  refreshing=false;RefreshRows();
 }
 void RefreshRows()
 {
  refreshing=true;var c=read().Configuration;var matches=c.Variables.Where(v=>v.GroupId==group&&v.Name.Contains(Search.Text,StringComparison.OrdinalIgnoreCase)).ToArray();int total=Math.Max(1,(matches.Length+pageSize-1)/pageSize);page=Math.Clamp(page,1,total);
  Variables.ItemsSource=matches.Skip((page-1)*pageSize).Take(pageSize).Select(v=>new Row {Id=v.Id,References=string.Join("；",c.Subscriptions.Where(b=>b.Source.Kind==WorkflowSourceKind.LocalVariable&&b.Source.Id==v.Id).Select(b=>{var node=read().Nodes.First(n=>n.Id==b.NodeId);return node.Name+"."+(b.IsParameter?WorkflowPresentation.Parameter(node.ModuleId,b.Target):b.Target);}).DefaultIfEmpty("------")),Index=c.Variables.ToList().FindIndex(x=>x.Id==v.Id)+1,Name=v.Name.Trim('%'),Type=v.Type,IsBound=v.SourceNode is not null,Value=v.SourceNode is null?v.Value:liveValue(v.SourceNode,v.SourcePort!)??v.Value,Remark=v.Remark,BindingValue=v.SourceNode is null?"":liveValue(v.SourceNode,v.SourcePort!)??"暂无运行结果",Binding=v.SourceNode is null?"------":sources(null).FirstOrDefault(s=>s.NodeId==v.SourceNode&&s.Port==v.SourcePort)?.Label??"------"}).ToArray();PageLabel.Text=$"{page}/{total}  共{matches.Length}项";refreshing=false;UpdateButtons();
 }
 void AddVariable(object sender,RoutedEventArgs e)=>Safe(()=>{var c=read().Configuration;if(group is null)return;if(c.Variables.Count>=2048)throw new InvalidOperationException("变量数量不能超过2048。");int n=0;while(c.Variables.Any(v=>v.Name==$"%var{n}%"))n++;save(c with{Variables=c.Variables.Append(new(Guid.NewGuid().ToString("N"),group,$"%var{n}%","int","0")).ToArray()});Search.Text="";page=int.MaxValue;RefreshRows();Variables.SelectedIndex=Variables.Items.Count-1;});
 void DeleteVariable(object sender,RoutedEventArgs e){if(sender is Button {DataContext:Row row})Safe(()=>{var c=read().Configuration;save(c with{Variables=c.Variables.Where(v=>v.Id!=row.Id).ToArray(),Subscriptions=c.Subscriptions.Where(b=>b.Source.Kind!=WorkflowSourceKind.LocalVariable||b.Source.Id!=row.Id).ToArray()});RefreshRows();});}
 void AddGroup(object sender,RoutedEventArgs e)=>Safe(()=>{var c=read().Configuration;if(c.Groups.Count>=10)throw new InvalidOperationException("分组数量不能超过10。");int n=0;while(c.Groups.Any(g=>g.Name=="分组"+n))n++;var item=new WorkflowVariableGroup(Guid.NewGuid().ToString("N"),"分组"+n);save(c with{Groups=c.Groups.Append(item).ToArray()});group=item.Id;page=1;Refresh();});
 void RenameGroup(TabItem tab,WorkflowVariableGroup g){var editor=new TextBox {Text=g.Name,MinWidth=80};tab.Header=editor;bool done=false;void Apply(){if(done)return;done=true;Safe(()=>{var c=read().Configuration;save(c with{Groups=c.Groups.Select(item=>item.Id==g.Id?item with{Name=editor.Text}:item).ToArray()});});Refresh();}editor.LostFocus+=(_,_)=>Apply();editor.KeyDown+=(_,e)=>{if(e.Key==Key.Return)Apply();};editor.Focus();editor.SelectAll();}
 IEnumerable<WorkflowSourceOption> TargetOptions(WorkflowLocalVariable variable)
 {
  if(registry is null)yield break;var p=read();var kind=WorkflowSubscriptions.VariableKind(variable.Type);var claimed=p.Configuration.Subscriptions.Where(b=>b.Source.Kind!=WorkflowSourceKind.LocalVariable||b.Source.Id!=variable.Id).ToArray();
  foreach(var node in p.Nodes){var descriptor=registry.GetDescriptor(node);foreach(var parameter in descriptor.Parameters.Where(v=>WorkflowSubscriptions.ParameterType(v.Kind)==kind))if(!claimed.Any(b=>b.NodeId==node.Id&&b.Target==parameter.Name&&b.IsParameter))yield return new(node.Id,parameter.Name,kind,node.Name+"."+WorkflowPresentation.Parameter(node.ModuleId,parameter.Name));foreach(var input in descriptor.Inputs.Where(v=>v.Kind==kind))if(!claimed.Any(b=>b.NodeId==node.Id&&b.Target==input.Name&&!b.IsParameter)&&!p.Connections.Any(c=>c.ToNode==node.Id&&c.ToPort==input.Name))yield return new(node.Id,input.Name,kind,node.Name+"."+input.Name){IsInput=true};}
 }
 void BindTargets(object sender,RoutedEventArgs e)
 {
  if(sender is not Button {DataContext:Row row}||registry is null)return;var c=read().Configuration;var variable=c.Variables.Single(v=>v.Id==row.Id);var options=TargetOptions(variable).ToArray();var old=c.Subscriptions.Where(b=>b.Source.Kind==WorkflowSourceKind.LocalVariable&&b.Source.Id==row.Id).ToArray();var picker=new WorkflowVariableTargetsWindow(options,options.Where(o=>old.Any(b=>b.NodeId==o.NodeId&&b.Target==o.Port&&b.IsParameter!=o.IsInput))){Owner=this};if(picker.ShowDialog()==true)Safe(()=>{var current=read().Configuration;var proposed=current with{Subscriptions=current.Subscriptions.Where(b=>b.Source.Kind!=WorkflowSourceKind.LocalVariable||b.Source.Id!=row.Id).Concat(picker.Selections.Select(o=>new WorkflowSubscription(o.NodeId,o.Port,!o.IsInput,new(WorkflowSourceKind.LocalVariable,row.Id)))).ToArray()};var error=GraphValidator.Validate(read() with{Configuration=proposed},registry).Issues.FirstOrDefault(i=>i.Code is "WorkflowConfiguration" or "DataExecutionOrder" or "Cycle");if(error is not null)throw new InvalidOperationException(error.Message);save(proposed);RefreshRows();});
 }
 void BindSource(object sender,RoutedEventArgs e)
 {
  if(sender is not Button {DataContext:Row row})return;var kind=row.Type=="int"?DataKind.Integer:row.Type=="float"?DataKind.Number:DataKind.Text;var options=sources(kind).Where(o=>o.Cardinality==ValueCardinality.Scalar&&CanBindSource(row.Id,o)).ToArray();var picker=new WorkflowSourcePicker(new[]{new WorkflowSourceOption("","",DataKind.Text,"解除绑定")}.Concat(options)){Owner=this};if(picker.ShowDialog()==true&&picker.Selection is {} selected)Safe(()=>{var c=SourceConfiguration(row.Id,selected);if(registry is not null){var error=GraphValidator.Validate(read() with{Configuration=c},registry).Issues.FirstOrDefault(i=>i.Code is "WorkflowConfiguration" or "DataExecutionOrder" or "Cycle");if(error is not null)throw new InvalidOperationException(error.Message);}save(c);RefreshRows();});
 }
 WorkflowConfiguration SourceConfiguration(string id,WorkflowSourceOption source){var c=read().Configuration;return c with{Variables=c.Variables.Select(v=>v.Id==id?v with{SourceNode=source.NodeId.Length==0?null:source.NodeId,SourcePort=source.Port.Length==0?null:source.Port}:v).ToArray()};}
 bool CanBindSource(string id,WorkflowSourceOption source){if(registry is null)return true;var error=GraphValidator.Validate(read() with{Configuration=SourceConfiguration(id,source)},registry).Issues.FirstOrDefault(i=>i.Code is "WorkflowConfiguration" or "DataExecutionOrder" or "Cycle");return error is null;}
 void Move(int delta,bool top=false)=>Safe(()=>{if(Variables.SelectedItem is not Row row)return;var c=read().Configuration;var list=c.Variables.ToList();var filtered=list.Where(v=>v.GroupId==group).ToList();int at=filtered.FindIndex(v=>v.Id==row.Id),next=top?0:at+delta;if(next<0||next>=filtered.Count)return;int from=list.FindIndex(v=>v.Id==row.Id),to=list.FindIndex(v=>v.Id==filtered[next].Id);if(top){var item=list[from];list.RemoveAt(from);list.Insert(to,item);}else(list[from],list[to])=(list[to],list[from]);save(c with{Variables=list});RefreshRows();Variables.SelectedItem=Variables.Items.OfType<Row>().FirstOrDefault(v=>v.Id==row.Id);});
 void UpdateButtons(){var list=read().Configuration.Variables.Where(v=>v.GroupId==group).ToList();int index=Variables.SelectedItem is Row row?list.FindIndex(v=>v.Id==row.Id):-1;TopButton.IsEnabled=UpButton.IsEnabled=index>0;DownButton.IsEnabled=index>=0&&index<list.Count-1;}
 void MoveTop(object s,RoutedEventArgs e)=>Move(0,true);void MoveUp(object s,RoutedEventArgs e)=>Move(-1);void MoveDown(object s,RoutedEventArgs e)=>Move(1);
 void SelectionChanged(object s,SelectionChangedEventArgs e){if(!refreshing)UpdateButtons();}
 void GroupChanged(object s,SelectionChangedEventArgs e){if(!refreshing&&GroupTabs.SelectedItem is TabItem {Tag:string id}){group=id;page=1;RefreshRows();}}
 void SearchChanged(object s,TextChangedEventArgs e){if(IsInitialized&&!refreshing){page=1;RefreshRows();}}
 void PageSizeChanged(object s,SelectionChangedEventArgs e){if(!IsInitialized)return;if(PageSize.SelectedItem is ComboBoxItem item)pageSize=int.Parse(item.Content.ToString()!,CultureInfo.InvariantCulture);page=1;RefreshRows();}
 void Previous(object s,RoutedEventArgs e){page--;RefreshRows();}void Next(object s,RoutedEventArgs e){page++;RefreshRows();}
 void CloseClick(object s,RoutedEventArgs e)=>Close();void DragTitle(object s,MouseButtonEventArgs e){if(e.ChangedButton==MouseButton.Left)DragMove();}
 protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
 {
  if(Keyboard.FocusedElement is TextBox {DataContext:Row row} text&&Window.GetWindow(text)==this){text.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();Safe(()=>Commit(row));}
  base.OnClosing(e);
 }
}
