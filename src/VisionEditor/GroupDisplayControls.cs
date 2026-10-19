using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using VisionFramework;
using Brushes=System.Windows.Media.Brushes;
using Rectangle=System.Windows.Shapes.Rectangle;
using Button=System.Windows.Controls.Button;
using ComboBox=System.Windows.Controls.ComboBox;
using TextBox=System.Windows.Controls.TextBox;
using CheckBox=System.Windows.Controls.CheckBox;
namespace VisionEditor;
public partial class SubprogramCallWindow {
 void SaveDisplays(IEnumerable<WorkflowInterfaceItem> items)=>saveGroupConfiguration!(GroupBody().Configuration with{Displays=items.ToArray()});
 void BuildGroupDisplays(StackPanel panel,ProcedureDocument body){
  var title=new TextBlock{Text="图像显示",FontWeight=FontWeights.Bold,Margin=new Thickness(0,2,0,4)};panel.Children.Add(title);var header=NativeDisplayRow();foreach(var column in new[]{("参数名称",0),("类型",1),("关联关系",2),("累积显示",3)}){var text=new TextBlock{Text=column.Item1,FontWeight=FontWeights.Bold,VerticalAlignment=VerticalAlignment.Center,Margin=new Thickness(column.Item2==0?0:8,0,0,0)};Grid.SetColumn(text,column.Item2);if(column.Item2==3)Grid.SetColumnSpan(text,2);header.Children.Add(text);}panel.Children.Add(header);
  foreach(var item in body.Configuration.Displays){var row=NativeDisplayRow();var name=new TextBox{Text=item.Name,MaxLength=31,Tag="GroupDisplayName:"+item.Id};name.LostKeyboardFocus+=(_,_)=>{if(name.Text==item.Name)return;Safe(()=>SaveDisplays(GroupBody().Configuration.Displays.Select(p=>p.Id==item.Id?p with{Name=name.Text}:p)));};row.Children.Add(name);var typeText=new TextBox{Text=DisplayKindName(item.Kind),IsReadOnly=true,Margin=new Thickness(8,0,0,0),VerticalContentAlignment=VerticalAlignment.Center};Grid.SetColumn(typeText,1);row.Children.Add(typeText);var options=()=>new[]{new WorkflowSourceOption("","",item.Kind,"解除订阅")}.Concat(groupSources?.Invoke(item.Kind)??[]);var selected=groupSources?.Invoke(item.Kind).FirstOrDefault(s=>s.NodeId==item.NodeId&&s.Port==item.Port);var source=new NativeCompoundSourceSelector(selected?.Label??"",options,s=>Safe(()=>SaveDisplays(GroupBody().Configuration.Displays.Select(p=>p.Id==item.Id?p with{NodeId=s.NodeId.Length==0?null:s.NodeId,Port=s.Port.Length==0?null:s.Port,Cardinality=s.Cardinality}:p)))){Tag="GroupDisplaySource:"+item.Id,IsEnabled=item.LinkedOutputId is null,Margin=new Thickness(8,0,0,0)};Grid.SetColumn(source,2);row.Children.Add(source);var accumulate=new CheckBox{IsChecked=item.Accumulate,Tag="GroupDisplayAccumulate:"+item.Id,VerticalAlignment=VerticalAlignment.Center,HorizontalAlignment=System.Windows.HorizontalAlignment.Center,IsEnabled=item.Kind!=DataKind.GrayImage};NativeGroupSwitch(accumulate);Grid.SetColumn(accumulate,3);accumulate.Click+=(_,_)=>Safe(()=>SaveDisplays(GroupBody().Configuration.Displays.Select(p=>p.Id==item.Id?p with{Accumulate=accumulate.IsChecked==true}:p)));row.Children.Add(accumulate);var remove=new Button{Content="×",Padding=new Thickness(0),Width=24,Tag="GroupDisplayDelete:"+item.Id,ToolTip="删除显示项"};NativeGroupIcon(remove,"delete");remove.Click+=(_,_)=>Safe(()=>SaveDisplays(GroupBody().Configuration.Displays.Where(p=>p.Id!=item.Id)));Grid.SetColumn(remove,4);row.Children.Add(remove);panel.Children.Add(row);}
  var kinds=new[]{DataKind.GrayImage,DataKind.Circle,DataKind.Components};var addRow=new StackPanel{Orientation=System.Windows.Controls.Orientation.Horizontal,Margin=new Thickness(0,0,0,6)};var type=new ComboBox{ItemsSource=new[]{"输出图","Circle","Components"},SelectedIndex=0,Width=80,Height=32,Tag="GroupDisplayAddType"};var add=new Button{Tag="GroupDisplayAdd",Width=32,Height=32,Margin=new Thickness(7,0,0,0)};NativeGroupIcon(add,"add");add.Click+=(_,_)=>Safe(()=>{var values=GroupBody().Configuration.Displays;int n=0;while(values.Any(p=>p.Name=="输出图像"+n))n++;SaveDisplays(values.Append(new(Guid.NewGuid().ToString("N"),"输出图像"+n,kinds[type.SelectedIndex])));});addRow.Children.Add(type);addRow.Children.Add(add);panel.Children.Add(addRow);
 }
 static string DisplayKindName(DataKind kind)=>kind==DataKind.GrayImage?"image":kind==DataKind.Circle?"Circle":kind==DataKind.Components?"Components":kind.ToString();
 static Grid NativeDisplayRow(){var row=new Grid{Margin=new Thickness(0,0,5,6),Height=32};foreach(var width in new[]{new GridLength(70),new GridLength(60),new GridLength(1,GridUnitType.Star),new GridLength(48),new GridLength(26)})row.ColumnDefinitions.Add(new(){Width=width});return row;}
 VisionCore.GrayImage? currentGroupImage;
 void RenderGroupDisplays(ExecutionSessionResult? session,VisionCore.GrayImage? image){
  currentGroupImage=image;DisplayOverlay.Children.Clear();if(image is null||session?.Status!=SessionStatus.Success)return;double width=DisplayOverlay.ActualWidth,height=DisplayOverlay.ActualHeight;if(width<=0||height<=0)return;double scale=Math.Min(width/image.Width,height/image.Height),x=(width-image.Width*scale)/2,y=(height-image.Height*scale)/2;
  foreach(var value in session.WorkflowDisplays.Values.SelectMany(v=>v).SelectMany(v=>v.Elements())){
   if(value.Value is VisionCore.CircleFit circle){var shape=new Ellipse{Width=circle.Radius*2*scale,Height=circle.Radius*2*scale,Stroke=Brushes.Lime,StrokeThickness=1.5};Canvas.SetLeft(shape,x+(circle.CenterX-circle.Radius)*scale);Canvas.SetTop(shape,y+(circle.CenterY-circle.Radius)*scale);DisplayOverlay.Children.Add(shape);}
   else if(value.Value is IReadOnlyList<VisionCore.Component> components)foreach(var component in components){var shape=new Rectangle{Width=(component.MaxX-component.MinX+1)*scale,Height=(component.MaxY-component.MinY+1)*scale,Stroke=Brushes.Lime,StrokeThickness=1.5};Canvas.SetLeft(shape,x+component.MinX*scale);Canvas.SetTop(shape,y+component.MinY*scale);DisplayOverlay.Children.Add(shape);}
  }
 }
}
