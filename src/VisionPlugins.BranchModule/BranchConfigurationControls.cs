using Color=System.Windows.Media.Color;
using Button=System.Windows.Controls.Button;
using RadioButton=System.Windows.Controls.RadioButton;
using TextBox=System.Windows.Controls.TextBox;
using Label=System.Windows.Controls.Label;
using HorizontalAlignment=System.Windows.HorizontalAlignment;
using Orientation=System.Windows.Controls.Orientation;
#nullable enable
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VisionPluginContracts;
namespace VisionPlugins.BranchModule;
// Dedicated leaf controls for the existing common configuration host; no window
// or graph dispatcher is owned here. Geometry comes from the original templates.
public static class BranchConfigurationControls {
 public static void Register(ToolConfigurationControlRegistry registry,Func<string,string> text){
  Action<int>? beforeModeChange=null;
  registry.Register("RadioSelecter_Branch",c=>{
   var panel=new Grid{Height=32,Margin=new Thickness(34,1,70,0)};panel.ColumnDefinitions.Add(new());panel.ColumnDefinitions.Add(new());bool updating=false;
   var buttons=c.Node.Choices.Take(2).Select((choice,i)=>{var radio=new RadioButton{Content=text(choice.CaptionKey),HorizontalAlignment=HorizontalAlignment.Left};Grid.SetColumn(radio,i);panel.Children.Add(radio);radio.Checked+=(_,_)=>{if(!updating){beforeModeChange?.Invoke(int.Parse(choice.Value,CultureInfo.InvariantCulture));c.State.CommitUser(c.Node,choice.Value);}};return radio;}).ToArray();
   c.OnRefresh(()=>{updating=true;try{for(int i=0;i<buttons.Length;i++)buttons[i].IsChecked=c.Node.Value==c.Node.Choices[i].Value;}finally{updating=false;}});return panel;
  });
  registry.Register("ListParam",c=>{
   var panel=new StackPanel();var model=new BranchModuleListModel();string? lastRead=null;bool updating=false,transitioningMode=false;
   int Mode()=>int.TryParse(c.State.Find("IndexUserSet")?.Value,out var mode)?mode:0;
   void Commit(){string value=model.Serialize();if(c.Node.Value!=value)c.State.CommitUser(c.Node,value);lastRead=c.Node.Value;}
   void Render(){
    panel.Children.Clear();for(int index=0;index<model.Rows.Count;index++){
     int position=index;var row=model.Rows[index];var line=new Grid{Height=32,Margin=new Thickness(0,6,0,0)};line.ColumnDefinitions.Add(new(){Width=new GridLength(89)});line.ColumnDefinitions.Add(new());
     line.Children.Add(new TextBlock{Text=text("RunParam_Module ID:")+row.Id,Foreground=new SolidColorBrush(Color.FromRgb(102,102,102)),VerticalAlignment=VerticalAlignment.Center});var right=new Grid();right.ColumnDefinitions.Add(new());right.ColumnDefinitions.Add(new(){Width=GridLength.Auto});right.ColumnDefinitions.Add(new(){Width=GridLength.Auto});Grid.SetColumn(right,1);line.Children.Add(right);
     var input=new TextBox{Text=row.InputValue??"",Height=30,VerticalContentAlignment=VerticalAlignment.Center,HorizontalContentAlignment=HorizontalAlignment.Left,TextWrapping=TextWrapping.Wrap,IsEnabled=row.Enabled};right.Children.Add(input);
     input.TextChanged+=(_,_)=>{if(updating)return;if(!row.TrySetInput(input.Text)){updating=true;input.Text=row.InputValue??"";updating=false;return;}if(model.Mode==0)Commit();};
     void AddMove(int direction,string asset,int column){var image=new System.Windows.Controls.Image{Width=24,Height=24,Stretch=Stretch.None,UseLayoutRounding=true};void SetImage(string state)=>image.Source=new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/VisionPlugins.BranchModule;component/Resources/branch-"+asset+"-"+state+".png"));var button=new Button{Content=image,Height=20,Width=20,Margin=new Thickness(column==1?5:0,0,0,0),IsEnabled=direction<0?position>0:position<model.Rows.Count-1,VerticalAlignment=VerticalAlignment.Center};SetImage(button.IsEnabled?"normal":"disable");button.IsEnabledChanged+=(_,_)=>SetImage(button.IsEnabled?"normal":"disable");button.PreviewMouseLeftButtonDown+=(_,_)=>SetImage("click");button.PreviewMouseLeftButtonUp+=(_,_)=>SetImage(button.IsEnabled?"normal":"disable");button.MouseLeave+=(_,_)=>SetImage(button.IsEnabled?"normal":"disable");Grid.SetColumn(button,column);right.Children.Add(button);button.Click+=(_,_)=>{if(model.Move(position,direction)){Commit();updating=true;try{Render();}finally{updating=false;}}};}
     AddMove(-1,"up",1);AddMove(1,"down",2);panel.Children.Add(line);
    }
   }
   beforeModeChange=mode=>{transitioningMode=true;updating=true;try{model.SetMode(mode,Commit);Render();}finally{updating=false;transitioningMode=false;}};
   c.OnRefresh(()=>{if(transitioningMode)return;updating=true;try{int mode=Mode();bool changed=model.Mode!=mode;model.SetMode(mode);if(lastRead!=c.Node.Value){if(c.Node.Value!=model.Serialize()){model.Read(c.Node.Value);changed=true;}lastRead=c.Node.Value;}if(changed||panel.Children.Count!=model.Rows.Count)Render();}finally{updating=false;}});return panel;
  });
 }
}



