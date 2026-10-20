using System.IO;
using System.Text.Json;
using System.Windows;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.MapCalibration;
static class Program {
 [STAThread] static int Main(){try{Run();return 0;}catch(Exception error){Console.Error.WriteLine(error);return 1;}}
 sealed class Files:IToolConfigurationFileService {public ConfigurationFileRequest? Request;public string? Selected,Remembered;public ConfigurationFileSelection Select(Window? owner,ConfigurationFileRequest request){Request=request;return Selected is null?new(ConfigurationFileStatus.Cancelled,[]):new(ConfigurationFileStatus.Selected,[Selected]);}public ConfigurationFileHistoryResult Remember(string moduleKey,string selectedFile){Remembered=selectedFile;return new(true);}}
 static void Run(){int checks=0;void Check(bool ok,string text){if(!ok)throw new Exception(text);checks++;}
  var plugin=new MapCalibrationPlugin();var values=plugin.Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());var node=new NodeDocument("map",MapCalibrationPlugin.Id,"1.0.0","映射标定",values);
  var requests=new List<ConfigurationRoiRequest>();var roiService=new ToolConfigurationRoiService(node.Id,()=>true,request=>{requests.Add(request);return new(node,null,null,true);},(_,_,_)=>{},_=>throw new InvalidOperationException("No self-run in model check"),(_,_)=>{});
  var files=new Files();var context=new ToolEditorContext(node,Directory.GetCurrentDirectory(),next=>values=next,_=>throw new InvalidOperationException("No self-run in model check")){ConfigurationRois=roiService,ConfigurationFiles=files};
  using var session=plugin.CreateConfiguration(context);var state=session.State;
  using var original=JsonDocument.Parse(File.ReadAllText("analysis/map-calibration/parser/model/probe.json"));
  void Compare(string phase){var expected=original.RootElement.EnumerateArray().Single(p=>p.GetProperty("phase").GetString()==phase);foreach(var item in expected.GetProperty("items").EnumerateArray()){var properties=item.GetProperty("values");if(!properties.TryGetProperty("NameA",out var name)||string.IsNullOrEmpty(name.GetString()))continue;var actual=state.Find(name.GetString()!);if(actual is null)continue;if(properties.TryGetProperty("CustomVisibleA",out var visible))Check(actual.CustomVisible==bool.Parse(visible.GetString()!),phase+" visibility "+name.GetString());}}
  Check(state.Definition.Tabs.Count==3,"Original three tabs");Compare("default");state.CommitUser("InputType","2");Compare("file-mode");state.CommitUser("InputType","1");Compare("image-mode");state.CommitUser("TeachEnable","True");Compare("teach-mode");
  foreach(var weight in new[]{"1","2","3"}){state.CommitUser("WeightFun",weight);Check(state.Find("WeightFactor")?.CustomVisible==true,"WeightFactor remains visible under original one-way visibility trigger "+weight);}
  var window=new ToolConfigurationModelWindow(context,session){Left=-10000,Top=-10000,WindowStartupLocation=WindowStartupLocation.Manual};window.Show();window.UpdateLayout();
  Check(requests.Any(r=>r.SourceName=="RoiType_0"&&r.InputPort=="InputImage1"),"First ROI tied to first image");Check(requests.Any(r=>r.SourceName=="RoiType_1"&&r.InputPort=="InputImage2"),"Second ROI tied to second image");
  IEnumerable<DependencyObject> Descendants(DependencyObject parent){for(int i=0;i<System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent);i++){var child=System.Windows.Media.VisualTreeHelper.GetChild(parent,i);yield return child;foreach(var nested in Descendants(child))yield return nested;}}
  var save=Descendants(window).OfType<System.Windows.Controls.Button>().First(b=>b.Content?.ToString()=="生成标定文件");Check(save.HorizontalAlignment==HorizontalAlignment.Left&&double.IsNaN(save.Width),"Original save Command auto-width left alignment");Check(save.Padding==new Thickness(4,0,4,0),"Original Command padding");Check(save.Parent is System.Windows.Controls.Grid row&&row.Height==32,"Original Command 32px row");
  state.CommitUser("InputType","2");window.UpdateLayout();System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(()=>{},System.Windows.Threading.DispatcherPriority.Render);
  var open=Descendants(window).OfType<System.Windows.Controls.Button>().First(b=>Descendants(b).OfType<System.Windows.Controls.Image>().Any(i=>i.Name=="pathIcon"));var pathGrid=(System.Windows.Controls.Grid)open.Parent;Check(pathGrid.ColumnDefinitions.Count==2&&pathGrid.ColumnDefinitions[1].Width.IsAuto,"Original path star/auto columns");var input=pathGrid.Children.OfType<System.Windows.Controls.TextBox>().Single();Check(input.FontSize==14&&input.BorderThickness==new Thickness(0),"Original path transparent borderless text input");Check(pathGrid.Parent is System.Windows.Controls.Border border&&border.CornerRadius==new CornerRadius(4),"Original path four pixel corner");open.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));Check(files.Request is {Multiselect:false,Filter:".xml;.iwcal|*.xml;*.iwcal"},"Original local file request is single file with calibration filter");Check(files.Remembered is null,"Cancelled picker does not update history");files.Selected=Path.GetFullPath("analysis/map-calibration/native/module-export.xml");open.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));Check(files.Remembered==files.Selected,"Successful path commits and remembers calibration file");
  var diagnostics=window.ConfigurationView.Diagnostics.ToArray();Check(diagnostics.Length==0,"No unknown configuration templates: "+JsonSerializer.Serialize(diagnostics));window.Close();
  Console.WriteLine($"PASS {checks}: original parser visibility and dual ROI ordinary window");Directory.CreateDirectory("analysis/map-calibration/configuration");File.WriteAllText("analysis/map-calibration/configuration/checks.json",JsonSerializer.Serialize(new{checks,diagnostics,originalParser=true,originalDesktopCompared=false}));
 }
}




