using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;
using W=System.Windows.Controls;
public static class UiChecks
{
 public static int Run(string root){int checks=0;Exception? failure=null;var thread=new Thread(()=>{try{
  var plugin=new RotateCalibrationPlugin();var values=plugin.Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());
  var node=new NodeDocument("rotate",RotateCalibrationPlugin.Id,"1.0.0","旋转标定",values);
  var context=new ToolEditorContext(node,root,p=>values=p,_=>Task.FromResult(new NodeRunRecord("rotate",NodeRunStatus.Success,new Dictionary<string,PortValue>(),0)));
  var window=plugin.CreateWpfEditor(context);window.Left=-30000;window.Top=-30000;window.ShowInTaskbar=false;window.ShowActivated=false;window.Show();window.UpdateLayout();
  IEnumerable<FrameworkElement> Elements(DependencyObject item){if(item is FrameworkElement fe)yield return fe;for(int i=0;i<VisualTreeHelper.GetChildrenCount(item);i++)foreach(var child in Elements(VisualTreeHelper.GetChild(item,i)))yield return child;}
  void Check(bool ok,string text){checks++;if(!ok)throw new Exception(text);}
  var controls=Elements(window).ToArray();Check(controls.Any(e=>e.Name=="InputPairImagePointX"),"Point mode picker");Check(!controls.Any(e=>e.Name=="InputImagePointX"),"Point mode hides individual coordinates");
  var coordinate=controls.OfType<W.RadioButton>().Single(e=>e.Name=="InputWay1");coordinate.IsChecked=true;window.UpdateLayout();controls=Elements(window).ToArray();Check(values["InputWay"].GetInt32()==1,"Coordinate mode applies");Check(controls.Any(e=>e.Name=="InputImagePointX")&&controls.Any(e=>e.Name=="InputImagePointY"),"Coordinate subscriptions visible");
  var reverse=controls.OfType<W.CheckBox>().Single(e=>e.Name=="RevOutputFlag");reverse.IsChecked=true;Check(values["RevOutputFlag"].GetBoolean(),"Reverse applies");
  foreach(string key in new[]{"WorldRotateAngle","RotNum","RefreshSignal"})Check(controls.Any(e=>e.Name=="Input"+key),"Native input "+key);
  var bmp=new RenderTargetBitmap((int)window.ActualWidth,(int)window.ActualHeight,96,96,PixelFormats.Pbgra32);bmp.Render(window);var encoder=new PngBitmapEncoder();encoder.Frames.Add(BitmapFrame.Create(bmp));Directory.CreateDirectory(Path.Combine(root,"analysis/full-audit/rotate-calibration/checks"));using(var file=File.Create(Path.Combine(root,"analysis/full-audit/rotate-calibration/checks/independent-coordinate.png")))encoder.Save(file);
  controls.OfType<W.TabControl>().Single().SelectedIndex=1;window.UpdateLayout();controls=Elements(window).ToArray();
  foreach(string name in new[]{"DisplayContent","DisplayOKColor","DisplayNGColor","DisplayCharacterSize","DisplayTransparency",ToolDisplayService.PositionXInput,ToolDisplayService.PositionYInput})Check(controls.Any(e=>e.Name==name),"Shared TextDisplayGroup field "+name);
  var content=controls.OfType<W.TextBox>().Single(e=>e.Name=="DisplayContent");content.Text="中心 {RotCenterImageX}";content.RaiseEvent(new System.Windows.Input.KeyboardFocusChangedEventArgs(System.Windows.Input.Keyboard.PrimaryDevice,0,content,window){RoutedEvent=System.Windows.Input.Keyboard.LostKeyboardFocusEvent});Check(ToolDisplayService.Read(values,plugin.DisplayDefinition).Content=="中心 {RotCenterImageX}","Shared display content applies independently");
  var font=controls.OfType<W.TextBox>().Single(e=>e.Name=="DisplayCharacterSize");font.Text="18";font.RaiseEvent(new System.Windows.Input.KeyboardFocusChangedEventArgs(System.Windows.Input.Keyboard.PrimaryDevice,0,font,window){RoutedEvent=System.Windows.Input.Keyboard.LostKeyboardFocusEvent});Check(ToolDisplayService.Read(values,plugin.DisplayDefinition).FontSize==18,"Shared display font applies");
  font.Text="5";font.RaiseEvent(new System.Windows.Input.KeyboardFocusChangedEventArgs(System.Windows.Input.Keyboard.PrimaryDevice,0,font,window){RoutedEvent=System.Windows.Input.Keyboard.LostKeyboardFocusEvent});Check(ToolDisplayService.Read(values,plugin.DisplayDefinition).FontSize==18,"Original minimum font6 enforced");
  window.Close();
 }catch(Exception ex){failure=ex;}});thread.SetApartmentState(ApartmentState.STA);thread.Start();thread.Join();if(failure is not null)throw failure;return checks;}
}
