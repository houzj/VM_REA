using System.Globalization;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using VisionFramework;
using Button=System.Windows.Controls.Button;
using TextBox=System.Windows.Controls.TextBox;
using CheckBox=System.Windows.Controls.CheckBox;
using ComboBox=System.Windows.Controls.ComboBox;
namespace VisionEditor;
public sealed record GroupComparisonOption(string Label,GroupOperand Operand,bool Array=false);
public partial class SubprogramCallWindow {
 readonly Action<bool,int,int,GroupBreakBindings?>? setBreak;readonly Func<DataKind,IEnumerable<GroupComparisonOption>>? breakSources;readonly Action<WorkflowConfiguration>? saveGroupConfiguration;readonly Func<DataKind?,IEnumerable<WorkflowSourceOption>>? groupSources;
 static string FormatGroupValue(PortValue value)=>value.Cardinality==ValueCardinality.Array?"["+string.Join(", ",value.Elements().Select(FormatGroupScalar))+"]":FormatGroupScalar(value);
 static string FormatGroupScalar(PortValue value)=>value.Value switch{
  VisionCore.GrayImage image=>$"{image.Width}×{image.Height}",
  VisionCore.CircleFit c=>string.Create(CultureInfo.InvariantCulture,$"圆({c.CenterX:F2},{c.CenterY:F2};r={c.Radius:F2})"),
  VisionCore.Point2 point=>string.Create(CultureInfo.InvariantCulture,$"点({point.X:F2},{point.Y:F2})"),
  RoiRegionValue region=>$"区域({region.Regions.Count})",
  IReadOnlyList<VisionCore.Component> components=>$"连通域({components.Count})",
  FixtureValue f=>string.Create(CultureInfo.InvariantCulture,$"位置({f.Run.X:F2},{f.Run.Y:F2};{f.Run.Angle:F2}°)"),
  _=>Convert.ToString(value.Value,CultureInfo.InvariantCulture)??""};
 ProcedureDocument GroupBody()=>procedures().Single(p=>p.Id==node().Call!.ProcedureId);
 void WriteBreak(bool? enabled=null,int? type=null,int? comparison=null,GroupBreakBindings? bindings=null,bool replace=false){setBreak!(enabled??GroupBreak.Enabled(node()),type??GroupBreak.Integer(node(),"DataType",1),comparison??GroupBreak.Integer(node(),"CmpType",1),replace?bindings:node().GroupBreak);}
 void BuildBreakPanel(StackPanel panel){
  var enabled=new CheckBox{Content="中断循环",Tag="GroupBreakEnabled",IsChecked=GroupBreak.Enabled(node()),Margin=new Thickness(0,12,0,10),IsEnabled=setBreak is not null};enabled.Click+=(_,_)=>Safe(()=>WriteBreak(enabled:enabled.IsChecked==true));NativeGroupSwitch(enabled);panel.Children.Add(Labeled("中断循环",enabled));if(!GroupBreak.Enabled(node()))return;
  var type=new ComboBox{Tag="GroupBreakType",ItemsSource=new[]{"int","float","string"},SelectedIndex=GroupBreak.Integer(node(),"DataType",1)-1};type.SelectionChanged+=(_,_)=>{if(type.SelectedIndex>=0)Safe(()=>WriteBreak(type:type.SelectedIndex+1,bindings:null,replace:true));};panel.Children.Add(Labeled("数据类型",type));
  panel.Children.Add(BuildOperand(true));var comparison=new ComboBox{Tag="GroupBreakOperator",ItemsSource=new[]{">","<","=","!=",">=","<="},SelectedIndex=GroupBreak.Integer(node(),"CmpType",1)-1};comparison.SelectionChanged+=(_,_)=>{if(comparison.SelectedIndex>=0)Safe(()=>WriteBreak(comparison:comparison.SelectedIndex+1));};panel.Children.Add(Labeled("比较运算",comparison));panel.Children.Add(BuildOperand(false));
 }
 static DockPanel Labeled(string label,UIElement content){var row=new DockPanel{Margin=new Thickness(0,6,0,6)};row.Children.Add(new TextBlock{Text=label,Width=89,VerticalAlignment=VerticalAlignment.Center});row.Children.Add(content);return row;}
 FrameworkElement BuildOperand(bool left){
  var operand=left?node().GroupBreak?.Left:node().GroupBreak?.Right;var panel=new StackPanel();var key=left?"GroupBreakLeft":"GroupBreakRight";var options=breakSources?.Invoke(GroupBreak.Kind(node())).ToArray()??[];var selected=options.FirstOrDefault(o=>o.Operand.Inside==operand?.Inside&&o.Operand.Source==operand?.Source&&operand?.Source is not null);
  var link=new Button{Content=selected?.Label??"------",Tag=key};link.Click+=(_,_)=>ChooseOperand(left);panel.Children.Add(link);
  if(!left&&operand?.Source is null){var value=new TextBox{Tag="GroupBreakConstant",Text=operand?.Constant?.ToString()??"",IsEnabled=operand?.Source is null};value.LostKeyboardFocus+=(_,_)=>{if(value.Text==(operand?.Constant?.ToString()??""))return;Safe(()=>{var kind=GroupBreak.Kind(node());JsonElement literal=kind switch{DataKind.Integer=>JsonSerializer.SerializeToElement(int.Parse(value.Text,CultureInfo.InvariantCulture)),DataKind.Number=>JsonSerializer.SerializeToElement(float.Parse(value.Text,CultureInfo.InvariantCulture)),_=>JsonSerializer.SerializeToElement(value.Text)};SetOperand(false,new(Constant:literal));});};panel.Children.Add(value);}
  if(selected?.Array==true){
   var row=new DockPanel();var choose=new Button{Tag=key+"IndexSource",Content="…",Width=26,ToolTip="订阅整数索引"};DockPanel.SetDock(choose,Dock.Right);row.Children.Add(choose);
   var indexOptions=sources(DataKind.Integer).Where(o=>o.Cardinality==ValueCardinality.Scalar).ToArray();
   var initial=operand?.IndexSource is {} reference?indexOptions.FirstOrDefault(o=>o.Reference==reference)?.Label??"------":(operand?.Index??0).ToString(CultureInfo.InvariantCulture);
   var editor=new NativeGroupIndexEditor("["+initial+"]",()=>new[]{new WorkflowSourceOption("","",DataKind.Integer,"清空索引订阅")}.Concat(indexOptions),chosen=>Safe(()=>SetOperand(left,operand! with{IndexSource=chosen.Reference,Index=0})));var index=editor.Text;
   index.Tag=key+"Index";index.ToolTip="数组索引：光标进入方括号可选前置整数来源";
   index.LostKeyboardFocus+=(_,_)=>{var edited=index.Text.Trim();if(edited=="["+initial+"]")return;
    if(edited.StartsWith('[')&&edited.EndsWith(']'))edited=edited[1..^1];
    Safe(()=>{if(!int.TryParse(edited,out int v)||v<0)throw new ArgumentException("索引必须为非负整数。");SetOperand(left,operand! with{Index=v,IndexSource=null});});};
   choose.Click+=(_,_)=>{var picker=new WorkflowSourcePicker(new[]{new WorkflowSourceOption("","",DataKind.Integer,"解除索引订阅")}.Concat(indexOptions)){Owner=this};picker.SetInitialSelection(o=>o.Reference==operand?.IndexSource);if(picker.ShowDialog()==true&&picker.Selection is {} chosen)Safe(()=>SetOperand(left,operand! with{IndexSource=chosen.Reference,Index=0}));};row.Children.Add(editor);panel.Children.Add(row);
  }
  return Labeled(left?"基准比较值":"目标比较值",panel);
 }
 void SetOperand(bool left,GroupOperand? operand){var b=node().GroupBreak??new();WriteBreak(bindings:left?b with{Left=operand}:b with{Right=operand},replace:true);}
 void ChooseOperand(bool left){var kind=GroupBreak.Kind(node());var options=breakSources?.Invoke(kind).ToArray()??[];var mapped=options.Select((o,i)=>new WorkflowSourceOption("break:"+i,o.Operand.Source?.Port??o.Operand.Source?.Id??"",kind,o.Label)).ToArray();var picker=new WorkflowSourcePicker(new[]{new WorkflowSourceOption("","",kind,"解除订阅")}.Concat(mapped)){Owner=this};if(picker.ShowDialog()==true&&picker.Selection is {} selected)Safe(()=>SetOperand(left,selected.NodeId.Length==0?null:options[int.Parse(selected.NodeId[6..],CultureInfo.InvariantCulture)].Operand));}
 void SaveOutputs(IEnumerable<WorkflowInterfaceItem> outputs){var body=GroupBody();saveGroupConfiguration!(body.Configuration with{Outputs=outputs.ToArray()});}
 void BuildGroupOutputs(StackPanel panel,ProcedureDocument body){
  var header=NativeOutputRow();foreach(var column in new[]{("参数名称",0),("类型",1),("订阅关系",2),("累积输出",3)}){var label=new TextBlock{Text=column.Item1,FontWeight=FontWeights.Bold,VerticalAlignment=VerticalAlignment.Center,Margin=new Thickness(column.Item2==0?0:8,0,0,0)};Grid.SetColumn(label,column.Item2);if(column.Item2==3)Grid.SetColumnSpan(label,2);header.Children.Add(label);}panel.Children.Add(header);
  foreach(var item in body.Configuration.Outputs){var row=NativeOutputRow();
   var name=new TextBox{Text=item.Name,MaxLength=31,Tag="GroupOutputName:"+item.Id};name.LostKeyboardFocus+=(_,_)=>{if(name.Text==item.Name)return;Safe(()=>SaveOutputs(GroupBody().Configuration.Outputs.Select(o=>o.Id==item.Id?o with{Name=name.Text}:o)));};row.Children.Add(name);
   var type=new TextBox{Text=TypeName(item.Kind),IsReadOnly=true,VerticalAlignment=VerticalAlignment.Center,Margin=new Thickness(8,0,0,0)};Grid.SetColumn(type,1);row.Children.Add(type);
   var relation=groupSources?.Invoke(item.Kind).FirstOrDefault(s=>s.NodeId==item.NodeId&&s.Port==item.Port);var source=new Button{Content=new TextBlock{Text=relation?.Label??"------",TextTrimming=TextTrimming.CharacterEllipsis},Tag="GroupOutputSource:"+item.Id,ToolTip=relation?.Label};source.Margin=new Thickness(8,0,0,0);Grid.SetColumn(source,2);source.Click+=(_,_)=>{var options=groupSources?.Invoke(item.Kind)??[];var picker=new WorkflowSourcePicker(new[]{new WorkflowSourceOption("","",item.Kind,"解除订阅")}.Concat(options)){Owner=this};if(picker.ShowDialog()==true&&picker.Selection is {} selected)Safe(()=>SaveOutputs(GroupBody().Configuration.Outputs.Select(o=>o.Id==item.Id?o with{NodeId=selected.NodeId.Length==0?null:selected.NodeId,Port=selected.Port.Length==0?null:selected.Port,Cardinality=selected.Cardinality}:o)));};if(item.Kind==DataKind.GrayImage){var sourcePanel=new DockPanel();var display=new CheckBox{IsChecked=item.Display,Tag="GroupOutputDisplay:"+item.Id,ToolTip="显示图像",VerticalAlignment=VerticalAlignment.Center,Margin=new Thickness(4,0,0,0)};DockPanel.SetDock(display,Dock.Right);display.Click+=(_,_)=>Safe(()=>SaveOutputs(GroupBody().Configuration.Outputs.Select(o=>o.Id==item.Id?o with{Display=display.IsChecked==true}:o)));sourcePanel.Children.Add(display);sourcePanel.Children.Add(source);Grid.SetColumn(sourcePanel,2);row.Children.Add(sourcePanel);}else if(item.Kind is DataKind.Components or DataKind.Region){var nativeSource=new NativeCompoundSourceSelector(relation?.Label??"------",()=>new[]{new WorkflowSourceOption("","",item.Kind,"解除订阅")}.Concat(groupSources?.Invoke(item.Kind)??[]),selected=>Safe(()=>SaveOutputs(GroupBody().Configuration.Outputs.Select(o=>o.Id==item.Id?o with{NodeId=selected.NodeId.Length==0?null:selected.NodeId,Port=selected.Port.Length==0?null:selected.Port,Cardinality=selected.Cardinality}:o)))){Tag="GroupOutputSource:"+item.Id,Margin=new Thickness(8,0,0,0)};Grid.SetColumn(nativeSource,2);row.Children.Add(nativeSource);}else row.Children.Add(source);
   var accumulated=new CheckBox{IsChecked=item.Accumulate,Tag="GroupOutputAccumulate:"+item.Id,HorizontalAlignment=System.Windows.HorizontalAlignment.Center,VerticalAlignment=VerticalAlignment.Center,IsEnabled=GroupBreak.Accumulatable(item.Kind)||item.Kind==DataKind.GrayImage};NativeGroupSwitch(accumulated);Grid.SetColumn(accumulated,3);accumulated.Click+=(_,_)=>Safe(()=>SaveOutputs(GroupBody().Configuration.Outputs.Select(o=>o.Id==item.Id?o with{Accumulate=accumulated.IsChecked==true}:o)));row.Children.Add(accumulated);
   var delete=new Button{Content="×",Tag="GroupOutputDelete:"+item.Id,Padding=new Thickness(0),Margin=new Thickness(1),MinWidth=20};NativeGroupIcon(delete,"delete");Grid.SetColumn(delete,4);delete.Click+=(_,_)=>Safe(()=>SaveOutputs(GroupBody().Configuration.Outputs.Where(o=>o.Id!=item.Id)));row.Children.Add(delete);panel.Children.Add(row);
  }
  void AddOutput(DataKind kind)=>Safe(()=>{var config=GroupBody().Configuration;var items=config.Outputs;if(items.Count>=1024)throw new InvalidOperationException("最多添加1024个输出。");var name="out";int index=0;while(items.Any(o=>o.Name==name)||config.Inputs.Any(i=>i.Name==name))name="out"+index++;SaveOutputs(items.Append(new(Guid.NewGuid().ToString("N"),name,kind)));});
  var addRow=new StackPanel{Orientation=System.Windows.Controls.Orientation.Horizontal,Margin=new Thickness(0,8,0,0)};var kinds=new[]{DataKind.Integer,DataKind.Number,DataKind.Text,DataKind.GrayImage,DataKind.Circle,DataKind.Point,DataKind.Fixture,DataKind.Components,DataKind.Region};var types=new ComboBox{ItemsSource=kinds.Select(TypeName),SelectedIndex=0,Width=68,Tag="GroupOutputAddType"};var add=new Button{Content="增加输出",Tag="GroupOutputAdd"};add.Click+=(_,_)=>AddOutput(kinds[types.SelectedIndex]);NativeGroupIcon(add,"add");var addPoint=new Button{Content="增加点输出",Tag="GroupOutputAddPoint",Margin=new Thickness(10,0,0,0)};addPoint.Click+=(_,_)=>AddOutput(DataKind.Point);NativeGroupIcon(addPoint,"add");addRow.Children.Add(types);addRow.Children.Add(add);addRow.Children.Add(addPoint);panel.Children.Add(addRow);
 }
}
