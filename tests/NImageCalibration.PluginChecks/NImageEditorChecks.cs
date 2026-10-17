using System.Text.Json;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;
public static class NImageEditorChecks
{
 public static int Run(){int count=0;Exception? failure=null;var thread=new Thread(()=>{try{
  var values=NImageParameters.Defaults();var bindings=new Dictionary<string,double>();int clear=0;var node=new NodeDocument("nimage","vision.n-image-calibration","1.0.0","N图像标定",values);
  var context=new ToolEditorContext(node,Directory.GetCurrentDirectory(),p=>values=p,p=>Task.FromResult(new NodeRunRecord("nimage",NodeRunStatus.Success,new Dictionary<string,PortValue>{{"CurNum",PortValue.Integer(1)}},0))){InputNumber=k=>bindings.TryGetValue(k,out double v)?v:null,SetInputNumber=(k,v)=>{if(v.HasValue)bindings[k]=v.Value;else bindings.Remove(k);},SubscriptionLabel=(k,p)=>bindings.ContainsKey(k)?"绑定 "+k:null};
  void Check(bool c,string text){count++;if(!c)throw new Exception(text);}
  var window=new NImageWindow(context,()=>clear++);window.Left=-10000;window.Top=-10000;window.Show();window.UpdateLayout();
  T? Find<T>(string name) where T:FrameworkElement{T? Walk(DependencyObject d){if(d is T e&&e.Name==name)return e;for(int i=0;i<VisualTreeHelper.GetChildrenCount(d);i++){var r=Walk(VisualTreeHelper.GetChild(d,i));if(r!=null)return r;}return null;}return Walk(window);}
  void Capture(string name,bool bottom=false){var page=(Find<TabControl>("")!.SelectedItem as TabItem)?.Content as ScrollViewer;if(bottom)page?.ScrollToBottom();window.UpdateLayout();var bitmap=new RenderTargetBitmap(420,650,96,96,PixelFormats.Pbgra32);bitmap.Render(window);var encoder=new PngBitmapEncoder();encoder.Frames.Add(BitmapFrame.Create(bitmap));string dir=Path.GetFullPath("analysis/full-audit/n-image-calibration/reconstructed-ui");Directory.CreateDirectory(dir);using var file=File.Create(Path.Combine(dir,name+".png"));encoder.Save(file);page?.ScrollToTop();}
  Check(Find<TabControl>("")==null||window.IsLoaded,"editor loaded");Check(Find<FrameworkElement>("InputPairMovePhysicalPointX")!=null,"default point selector");Check(Find<FrameworkElement>("BasePointX")!=null,"automatic coordinates visible");Check(Find<FrameworkElement>("BaseAngle")!=null,"rotation coordinates visible");Check(Find<FrameworkElement>("InputPairPicPointX")==null,"matrix fixture selectors hidden by default");Capture("basic-default");
  var fix=Find<ToggleButton>("HomoFixEnable")!;fix.IsChecked=true;fix.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));window.UpdateLayout();Check(values["HomoFixEnable"].GetBoolean(),"fixture setting applied");Check(Find<FrameworkElement>("InputPairPicPointX")!=null,"fixture point selectors");Capture("fixture-point",true);
  var radio=Find<RadioButton>("InputMode1")!;radio.IsChecked=true;window.UpdateLayout();Check(Find<FrameworkElement>("InputPicPointX")!=null,"fixture coordinate selectors");Capture("fixture-coordinates",true);
  Find<RadioButton>("CalibPointInput1")!.IsChecked=true;window.UpdateLayout();Check(Find<FrameworkElement>("InputMovePhysicalPointX")!=null,"motion coordinate mode");Capture("motion-coordinates");
  var tabs=Find<TabControl>("")!;tabs.SelectedIndex=1;window.UpdateLayout();Check(Find<ComboBox>("CalibBoardType")!.Items.Count==4,"four original coded board types");Check((int)Find<ComboBox>("CalibBoardType")!.SelectedValue==7,"Algorithm XML board default overrides bare SDK default");Check(Find<FrameworkElement>("GrayContrast")==null,"abandoned gray contrast omitted");Check(Find<FrameworkElement>("SubPixelWindowSize")==null,"abandoned subpixel setting omitted");Capture("run-parameters");Find<Button>("ClearNImageFrames")!.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));Check(clear==1&&values["ClearRevision"].GetInt32()==1,"clear command callback");
  window.Close();
 }catch(Exception ex){failure=ex;}});thread.SetApartmentState(ApartmentState.STA);thread.Start();thread.Join();if(failure!=null)throw new Exception("NImage editor checks failed",failure);return count;}
}
