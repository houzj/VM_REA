using Binding=System.Windows.Data.Binding;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VisionPluginContracts;
using Button=System.Windows.Controls.Button;
using TextBox=System.Windows.Controls.TextBox;
using Orientation=System.Windows.Controls.Orientation;
using HorizontalAlignment=System.Windows.HorizontalAlignment;
namespace VisionPlugins.BranchString;
internal static class BranchStringControls {
 static BitmapImage Image(string asset)=>new(new Uri("pack://application:,,,/VisionPlugins.BranchString;component/Resources/"+asset+".png"));
 public static void Register(ToolConfigurationControlRegistry controls,Func<string,string> text){controls.Register("ListParamStr",c=>{
  var panel=new StackPanel();var model=new BranchStringListModel();string? lastRead=null;bool refreshing=false;
  void Commit(){string next=model.Serialize();if(c.Node.Value!=next)c.State.CommitUser(c.Node,next);lastRead=c.Node.Value;}
  void Render(){panel.Children.Clear();for(int index=0;index<model.Rows.Count;index++){int position=index;var row=model.Rows[index];var line=new Grid{Height=32,Margin=new Thickness(0,6,0,0)};line.ColumnDefinitions.Add(new(){Width=new GridLength(89)});line.ColumnDefinitions.Add(new());line.Children.Add(new TextBlock{Text=text("模块ID：")+row.NodeId,VerticalAlignment=VerticalAlignment.Center});var right=new StackPanel{Orientation=Orientation.Horizontal,Margin=new Thickness(-10,0,0,0)};Grid.SetColumn(right,1);line.Children.Add(right);
   var input=new TextBox{Width=115,VerticalContentAlignment=VerticalAlignment.Center,HorizontalContentAlignment=HorizontalAlignment.Left,TextWrapping=TextWrapping.Wrap};input.SetBinding(TextBox.TextProperty,new Binding(nameof(row.Text)){Source=row,Mode=BindingMode.TwoWay,UpdateSourceTrigger=UpdateSourceTrigger.PropertyChanged});input.TextChanged+=(_,_)=>{if(!refreshing)Commit();};right.Children.Add(input);
   var toggle=new ToggleButton{Margin=new Thickness(15,0,0,0),IsChecked=row.DebugMode,VerticalAlignment=VerticalAlignment.Center};void ToggleImage()=>toggle.Content=new System.Windows.Controls.Image{Source=Image(toggle.IsChecked==true?"swith_on":"swith_off"),Stretch=Stretch.None};ToggleImage();void Changed(){ToggleImage();if(!refreshing){row.DebugMode=toggle.IsChecked==true;Commit();}}toggle.Checked+=(_,_)=>Changed();toggle.Unchecked+=(_,_)=>Changed();right.Children.Add(toggle);
   void Move(int direction,string asset){var image=new System.Windows.Controls.Image{Width=24,Height=24,Stretch=Stretch.None,UseLayoutRounding=true};var button=new Button{Width=20,Height=20,Content=image,IsEnabled=direction<0?position>0:position<model.Rows.Count-1,VerticalAlignment=VerticalAlignment.Center};void State(string state)=>image.Source=Image("branch-"+asset+"-"+state);State(button.IsEnabled?"normal":"disable");button.IsEnabledChanged+=(_,_)=>State(button.IsEnabled?"normal":"disable");button.PreviewMouseLeftButtonDown+=(_,_)=>State("click");button.PreviewMouseLeftButtonUp+=(_,_)=>State(button.IsEnabled?"normal":"disable");button.MouseLeave+=(_,_)=>State(button.IsEnabled?"normal":"disable");button.Click+=(_,_)=>{if(model.Move(position,direction)){Commit();refreshing=true;try{Render();}finally{refreshing=false;}}};right.Children.Add(button);}Move(-1,"up");Move(1,"down");panel.Children.Add(line);
  }}
  c.OnRefresh(()=>{if(lastRead==c.Node.Value)return;refreshing=true;try{if(c.Node.Value!=model.Serialize()){model.Read(c.Node.Value);Render();}lastRead=c.Node.Value;}finally{refreshing=false;}});return panel;
 });}
}

