using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;
public static class NImageConfigurationChecks{
 public static int Run(){int checks=0;Exception? failure=null;var thread=new Thread(()=>{try{
  void Check(bool pass,string message){checks++;if(!pass)throw new Exception(message);}
  var values=NImageParameters.Defaults();int cleared=0;bool reject=false;var node=new NodeDocument("nimage",NImageConfigurationProvider.ModuleId,"1.0.0","N图像标定",values);
  var context=new ToolEditorContext(node,Directory.GetCurrentDirectory(),next=>{if(reject)throw new InvalidOperationException("owned rejection");values=next;},_=>throw new Exception("Original XML has no execution footer"));
  using var session=new NImageConfigurationProvider(_=>cleared++,(_,_)=>throw new InvalidOperationException("No complete solve")).CreateConfiguration(context);var state=session.State;
  Check(state.Definition.Tabs.Count==3,"Original XML includes two algorithm tabs plus linked display tab");
  Check(state.Find("CalibPointTotalNum")!.Value=="9"&&state.Find("RotPointTotalNum")!.Value=="3","Module XML count defaults");
  Check(state.Definition.BottomCommands.Count==3,"Source footer has only save, clear, confirm");
  Check(!state.Definition.BottomCommands.Any(e=>e.Children.Any(c=>c.Tag=="CommandSetter"&&c.Children.Any(x=>x.Text.Contains("SelfExcute")))),"No invented execute/continuous footer");
  Check(state.Find("Tab_Phys Coord Params")!.Visible,"Automatic physical category visible without subscriptions");
  state.CommitUser("CalibPointInput","1");Check(JsonSerializer.Deserialize<Dictionary<string,string>>(values["ConfigurationUserData"].GetString()!)!["Param.CalibPointInput"]=="1","UI-only input selector persisted in original user-data domain");Check(state.Find("MovePhysicalPointX")!.Visible&&!state.Find("PhyPoint")!.Visible,"Source point/scalar branch");
  state.CommitUser("HomoFixEnable","True");Check(state.Find("InputMode")!.Visible&&state.Find("Image Point")!.Visible,"Matrix-fix point mode");state.CommitUser("InputMode","1");Check(state.Find("PicPointX")!.Visible&&!state.Find("Image Point")!.Visible,"Matrix-fix coordinate mode");
  state.CommitUser("RotPointTotalNum","0");Check(!state.Find("BaseAngle")!.Visible&&state.Find("CalibPointTotalNum")!.Get("MinValue")=="1","Rotation fields hidden and translation nonzero guard");
  state.CommitUser("RotPointTotalNum","3");state.CommitUser("CalibPointTotalNum","0");Check(state.Find("RotPointTotalNum")!.Get("MinValue")=="1","Reciprocal nonzero minimum");
  state.CommitUser("CalibPointTotalNum","9");reject=true;state.CommitUser("BasePointX","5");Check(values["BasePointX"].GetDouble()==0&&state.Find("BasePointX")!.Value=="0","Host rejected write rolls back both stores");reject=false;
  state.Invoke(state.Find("ClearImage")!);Check(cleared==1&&values["ClearRevision"].GetInt32()==1,"Clear footer reaches owner and persists revision");
  Check(!state.Find("GrayContrast")!.Visible&&!state.Find("SubpixelWinAuto")!.Visible&&!state.Find("SubPixelWindowSize")!.Visible,"Abandoned fields retain source hidden visibility");
  var window=new ToolConfigurationModelWindow(context,session){Left=-10000,Top=-10000,WindowStartupLocation=WindowStartupLocation.Manual};window.Show();window.UpdateLayout();
  string directory="analysis/full-audit/n-image-calibration/common-model";Directory.CreateDirectory(directory);
  void Capture(string name){window.UpdateLayout();var bitmap=new RenderTargetBitmap((int)window.ActualWidth,(int)window.ActualHeight,96,96,PixelFormats.Pbgra32);bitmap.Render(window);var encoder=new PngBitmapEncoder();encoder.Frames.Add(BitmapFrame.Create(bitmap));using var file=File.Create(Path.Combine(directory,name+".png"));encoder.Save(file);}
  Capture("source-model");state.CommitUser("CalibPointInput","0");state.CommitUser("HomoFixEnable","False");Capture("basic-default");
  T? Find<T>(DependencyObject element)where T:DependencyObject{if(element is T match)return match;for(int i=0;i<VisualTreeHelper.GetChildrenCount(element);i++)if(Find<T>(VisualTreeHelper.GetChild(element,i)) is {} child)return child;return null;}
  var tabs=Find<System.Windows.Controls.TabControl>(window)!;Check(tabs.Items.Count==3,"All three original declared tabs render in the shared host");tabs.SelectedIndex=1;Capture("run-parameters");tabs.SelectedIndex=2;Capture("result-display");
  var diagnostics=window.ConfigurationView.Diagnostics.ToArray();window.Close();Check(diagnostics.Length==0,"All original model leaf factories render through production shared host: "+JsonSerializer.Serialize(diagnostics));
  File.WriteAllText(Path.Combine(directory,"verification.json"),JsonSerializer.Serialize(new{checks,sourceSha256=state.Definition.SourceSha256,manualAccepted=false,originalDesktopPixelsCompared=false,scope="Production XML model/control/host; no complete algorithm registration"},new JsonSerializerOptions{WriteIndented=true}));
 }catch(Exception error){failure=error;}});thread.SetApartmentState(ApartmentState.STA);thread.Start();thread.Join();if(failure!=null)throw new Exception("NImage source model checks failed",failure);return checks;}
}
