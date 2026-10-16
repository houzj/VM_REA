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
  foreach(var item in body.Configuration.Displays){var row=new DockPanel{Margin=new Thickness(0,0,0,8)};var remove=new Button{Content="×",Padding=new Thickness(0),Width=26,Tag="GroupDisplayDelete:"+item.Id};DockPanel.SetDock(remove,Dock.Right);remove.Click+=(_,_)=>Safe(()=>SaveDisplays(GroupBody().Configuration.Displays.Where(p=>p.Id!=item.Id)));row.Children.Add(remove);if(item.Kind!=DataKind.GrayImage){var accumulate=new CheckBox{Content="累积显示",IsChecked=item.Accumulate,Tag="GroupDisplayAccumulate:"+item.Id,VerticalAlignment=VerticalAlignment.Center};DockPanel.SetDock(accumulate,Dock.Right);accumulate.Click+=(_,_)=>Safe(()=>SaveDisplays(GroupBody().Configuration.Displays.Select(p=>p.Id==item.Id?p with{Accumulate=accumulate.IsChecked==true}:p)));row.Children.Add(accumulate);}
   var name=new TextBox{Text=item.Name,Width=76,Tag="GroupDisplayName:"+item.Id};name.LostKeyboardFocus+=(_,_)=>{if(name.Text==item.Name)return;Safe(()=>SaveDisplays(GroupBody().Configuration.Displays.Select(p=>p.Id==item.Id?p with{Name=name.Text}:p)));};row.Children.Add(name);var options=groupSources?.Invoke(item.Kind)??[];var selected=options.FirstOrDefault(s=>s.NodeId==item.NodeId&&s.Port==item.Port);var source=new Button{Content=selected?.Label??"------",ToolTip=selected?.Label,Tag="GroupDisplaySource:"+item.Id};source.IsEnabled=item.LinkedOutputId is null;source.Click+=(_,_)=>{var picker=new WorkflowSourcePicker(new[]{new WorkflowSourceOption("","",item.Kind,"解除订阅")}.Concat(groupSources?.Invoke(item.Kind)??[])){Owner=this};if(picker.ShowDialog()==true&&picker.Selection is {} s)Safe(()=>SaveDisplays(GroupBody().Configuration.Displays.Select(p=>p.Id==item.Id?p with{NodeId=s.NodeId.Length==0?null:s.NodeId,Port=s.Port.Length==0?null:s.Port,Cardinality=s.Cardinality}:p)));};row.Children.Add(source);panel.Children.Add(row);
  }
  var kinds=new[]{DataKind.GrayImage,DataKind.Circle,DataKind.Components};var addRow=new StackPanel{Orientation=System.Windows.Controls.Orientation.Horizontal};var type=new ComboBox{ItemsSource=new[]{"图像","圆","连通域"},SelectedIndex=0,Width=90,Tag="GroupDisplayAddType"};var add=new Button{Content="＋",Tag="GroupDisplayAdd",Width=32};add.Click+=(_,_)=>Safe(()=>{var values=GroupBody().Configuration.Displays;int n=0;while(values.Any(p=>p.Name=="display"+n))n++;SaveDisplays(values.Append(new(Guid.NewGuid().ToString("N"),"display"+n,kinds[type.SelectedIndex])));});addRow.Children.Add(type);addRow.Children.Add(add);panel.Children.Add(addRow);
 }
 VisionCore.GrayImage? currentGroupImage;
 void RenderGroupDisplays(ExecutionSessionResult? session,VisionCore.GrayImage? image){
  currentGroupImage=image;DisplayOverlay.Children.Clear();if(image is null||session?.Status!=SessionStatus.Success)return;double width=DisplayOverlay.ActualWidth,height=DisplayOverlay.ActualHeight;if(width<=0||height<=0)return;double scale=Math.Min(width/image.Width,height/image.Height),x=(width-image.Width*scale)/2,y=(height-image.Height*scale)/2;
  foreach(var value in session.WorkflowDisplays.Values.SelectMany(v=>v)){
   if(value.Value is VisionCore.CircleFit circle){var shape=new Ellipse{Width=circle.Radius*2*scale,Height=circle.Radius*2*scale,Stroke=Brushes.Lime,StrokeThickness=1.5};Canvas.SetLeft(shape,x+(circle.CenterX-circle.Radius)*scale);Canvas.SetTop(shape,y+(circle.CenterY-circle.Radius)*scale);DisplayOverlay.Children.Add(shape);}
   else if(value.Value is IReadOnlyList<VisionCore.Component> components)foreach(var component in components){var shape=new Rectangle{Width=(component.MaxX-component.MinX+1)*scale,Height=(component.MaxY-component.MinY+1)*scale,Stroke=Brushes.Lime,StrokeThickness=1.5};Canvas.SetLeft(shape,x+component.MinX*scale);Canvas.SetTop(shape,y+component.MinY*scale);DisplayOverlay.Children.Add(shape);}
  }
 }
}
