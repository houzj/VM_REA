using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VisionPluginContracts;
using W=System.Windows.Controls;
using Primitives=System.Windows.Controls.Primitives;
namespace VisionPlugins.ImageSource;
public partial class ImageSourceWindow:System.Windows.Window {
 readonly ToolEditorContext context;readonly Dictionary<string,JsonElement> values;readonly Dictionary<string,System.Windows.FrameworkElement> controls=new();readonly Dictionary<string,W.Grid> rows=new();bool loading=true;
 public ImageSourceWindow(ToolEditorContext context) {
  this.context=context;
  values=new ImageSourcePlugin().Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());
  foreach(var parameter in context.Node.Parameters)values[parameter.Key]=parameter.Value.Clone();
  InitializeComponent();
  Caption.Text=(context.DisplayIndex is int index?index+" ":"")+context.Node.Name;
  Group("基本参数",false);
  Enum("ImageSourceType","图像源",["本地图像","相机","SDK"]);
  Enum("PixelFormat","像素格式",["MONO8","RGB24"]);
  Toggle("SolSaveImageData","方案存图",false);
  var saveHint=new W.TextBlock{Text="最大保存120张图片",Foreground=System.Windows.Media.Brushes.DarkOrange,FontSize=12,Margin=new Thickness(53,0,0,0),VerticalAlignment=VerticalAlignment.Center,IsHitTestVisible=false};W.Grid.SetColumn(saveHint,1);rows["SolSaveImageData"].Children.Add(saveHint);
  Toggle("ShowImageName","显示图像名称");
  Subscription("CameraID","关联相机");
  Subscription("ExposureTimeInput","控制曝光");
  Subscription("GainInput","控制增益");
  Number("InitialSN","SN初始值");
  Number("PathCache","图片缓存");
  Toggle("OutMono8","输出Mono8");
  Toggle("StitchEnable","拼接使能");
  Number("StitchStartHeight","起始高度");
  Number("StitchHeight","拼接高度");
  Toggle("ClearTrigger","触发清空",false);
  Subscription("ClearTriggerCondition","触发变量");
  Group("触发设置",true);
  Toggle("AutoPlay","自动切换");
  Toggle("AutoStop","最后一张停止",false);
  Toggle("TriggerStringEnable","字符触发过滤",false);
  Subscription("TriggerString","输入字符");
  Subscription("TriggerFilter","触发字符");
  Group("订阅图片文件夹",true);
  Toggle("IsSubscribeFolderMode","订阅文件夹",false);
  Text("SubscribeFolderPath","图像路径");
  Toggle("RefreshImage","更新缩略图",false);
  Toggle("TriggerProcessRun","触发流程执行",false);
  loading=false;
  UpdateVisibility();
 }
 void Subscription(string key,string caption) {
  if(key=="CameraID"){Row(key,caption,new W.ComboBox{IsEnabled=false,Text="请选择",ToolTip="需要连接相机服务"});Decorate(key,"ic_modal_camera_normal.png",true);}
  else {var box=new W.TextBox{IsReadOnly=true,IsEnabled=false,Height=29,ToolTip="需要连接变量订阅服务"};Row(key,caption,box);Decorate(key,"ic_btn_link_disable.png",false);}
 }
 W.Image ResourceIcon(string file)=>new(){Source=new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/VisionPlugins.ImageSource;component/Resources/"+file)),Stretch=System.Windows.Media.Stretch.None};
 void Decorate(string key,string file,bool outside) {var row=rows[key];var editor=controls[key];row.Children.Remove(editor);var grid=new W.Grid();grid.ColumnDefinitions.Add(new W.ColumnDefinition());grid.ColumnDefinitions.Add(new W.ColumnDefinition{Width=new GridLength(28)});W.Grid.SetColumn(editor,0);grid.Children.Add(editor);var icon=ResourceIcon(file);icon.IsHitTestVisible=false;icon.ToolTip="需要连接变量订阅服务";W.Grid.SetColumn(icon,1);grid.Children.Add(icon);if(!outside)W.Grid.SetColumnSpan(editor,2);W.Grid.SetColumn(grid,1);row.Children.Add(grid);}
 void Group(string caption,bool separator){if(separator)Rows.Children.Add(new W.Border{Height=1,Background=new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(218,218,218)),Margin=new Thickness(0,3,0,17)});Rows.Children.Add(new W.TextBlock{Text=caption,FontWeight=FontWeights.Bold,FontSize=14,Foreground=new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(50,54,57)),Margin=new Thickness(0,0,0,9)});}
 void Row(string key,string caption,System.Windows.FrameworkElement editor){var row=new W.Grid{Height=36};row.ColumnDefinitions.Add(new W.ColumnDefinition{Width=new GridLength(91)});row.ColumnDefinitions.Add(new W.ColumnDefinition{Width=new GridLength(262)});row.Children.Add(new W.TextBlock{Text=caption,Foreground=new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(90,96,101)),VerticalAlignment=VerticalAlignment.Center});editor.Name=key;row.Tag=key;editor.VerticalAlignment=VerticalAlignment.Center;W.Grid.SetColumn(editor,1);row.Children.Add(editor);Rows.Children.Add(row);controls[key]=editor;rows[key]=row;}
 void Enum(string key,string caption,string[] options){var c=new W.ComboBox{ItemsSource=options,SelectedIndex=values[key].GetInt32()-1};c.SelectionChanged+=(_,_)=>{if(c.SelectedIndex>=0){values[key]=JsonSerializer.SerializeToElement(c.SelectedIndex+1);Apply();UpdateVisibility();}};Row(key,caption,c);}
 void Toggle(string key,string caption,bool enabled=true){var c=new Primitives.ToggleButton{Style=(Style)Resources["Switch"],IsChecked=values[key].GetBoolean(),IsEnabled=enabled,ToolTip=enabled?null:"此功能需要运行服务接入"};c.Checked+=Changed;c.Unchecked+=Changed;void Changed(object? sender,RoutedEventArgs e){values[key]=JsonSerializer.SerializeToElement(c.IsChecked==true);Apply();UpdateVisibility();}Row(key,caption,c);}
 void Number(string key,string caption){var box=new WpfNumericControl{Maximum=key=="InitialSN"?100000:key=="PathCache"?5:key is "StitchHeight" or "StitchStartHeight"?20000:int.MaxValue,Text=values[key].ToString()};box.ValueChanged+=text=>{if(int.TryParse(text,out int n)&&n>=0){values[key]=JsonSerializer.SerializeToElement(n);Apply();}};Row(key,caption,box);if(key!="PathCache")Decorate(key,"ic_btn_link切换_disable.png",true);}
 void Text(string key,string caption){var c=new W.TextBox{Text=values[key].GetString(),MaxLength=200,Height=27,Padding=new Thickness(8,2,0,0)};c.LostKeyboardFocus+=(_,_)=>{values[key]=JsonSerializer.SerializeToElement(c.Text);Apply();};Row(key,caption,c);}
 void Apply(){if(loading)return;try{context.Apply(new(values));}catch(Exception ex){System.Windows.MessageBox.Show(this,ex.Message,"参数未应用");}}
 void UpdateVisibility() {
  if(loading)return;
  bool local=values["ImageSourceType"].GetInt32()==1;
  bool camera=values["ImageSourceType"].GetInt32()==2;
  void Show(string key,bool visible)=>rows[key].Visibility=visible?Visibility.Visible:Visibility.Collapsed;
  foreach(string key in new[]{"PixelFormat","SolSaveImageData","ShowImageName","AutoPlay","PathCache","IsSubscribeFolderMode"})Show(key,local);
  foreach(string key in new[]{"CameraID","ExposureTimeInput","GainInput"})Show(key,camera);
  Show("OutMono8",!local||values["PixelFormat"].GetInt32()==2);
  Show("AutoStop",local&&values["AutoPlay"].GetBoolean());
  bool stitch=values["StitchEnable"].GetBoolean();
  foreach(string key in new[]{"StitchStartHeight","StitchHeight","ClearTrigger"})Show(key,stitch);
  Show("ClearTriggerCondition",stitch&&values["ClearTrigger"].GetBoolean());
  foreach(string key in new[]{"TriggerString","TriggerFilter"})Show(key,values["TriggerStringEnable"].GetBoolean());
  bool folder=local&&values["IsSubscribeFolderMode"].GetBoolean();
  foreach(string key in new[]{"SubscribeFolderPath","RefreshImage","TriggerProcessRun"})Show(key,folder);
 }
 void CloseWindow(object sender,RoutedEventArgs e)=>Close();void DragTitle(object sender,MouseButtonEventArgs e){if(e.ChangedButton==MouseButton.Left)DragMove();}
}
