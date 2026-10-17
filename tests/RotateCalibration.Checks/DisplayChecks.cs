using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VisionCore;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;
using W=System.Windows.Controls;
public static class DisplayChecks
{
 public static int Run(string root){int checks=0;void Check(bool ok,string text){checks++;if(!ok)throw new Exception(text);}
  var plugin=new RotateCalibrationPlugin();var definition=plugin.DisplayDefinition;var settings=ToolDisplayService.Defaults(definition);
  var image=new GrayImage(640,240,new byte[640*240]);var inputs=new Dictionary<string,PortValue>{{"InputImage",PortValue.Image(image)},{ToolDisplayService.PositionXInput,PortValue.Integer(5)},{ToolDisplayService.PositionYInput,PortValue.Number(6)}};
  var outputs=new Dictionary<string,PortValue>{{"ModuStatus",PortValue.Integer(1)},{"RotCenterImageX",PortValue.Number(30.0000019f)},{"RotCenterImageY",PortValue.Number(40)},{"RotCenterWorldX",PortValue.Number(60.0000038f)},{"RotCenterWorldY",PortValue.Number(80)},{"RotError",PortValue.Number(.698591053f)},{"RotWorldError",PortValue.Number(1.39718211f)},{"remainCount",PortValue.Integer(0)}};
  var frame=ToolDisplayService.Build(definition,settings,inputs,outputs);
  using var native=JsonDocument.Parse(File.ReadAllText(Path.Combine(root,"docs/plugins/rotate-calibration-evidence.json")));
  var expected=native.RootElement.GetProperty("uiRun").GetProperty("events").EnumerateArray().Single(p=>p.GetProperty("phase").GetString()=="original-feature-format").GetProperty("formatted")[0].GetString();
  Check(frame.Text==expected,"Original FeatureBase.GetValue localized float formatting");Check(frame.RecordText==expected,"Data Record uses native default content mapping");Check(ReferenceEquals(frame.Image,image),"InputImage object uses owned subscribed source");Check(frame.ResultList.Count==7&&frame.ResultList.Select(c=>c.Column).SequenceEqual(definition.ResultColumns),"Result List exact seven declared columns");Check(frame.ResultList[0].Value=="1"&&frame.ResultList[5].Value=="0.6985911","Result List native float precision");Check(frame.Color=="#66ff00"&&frame.FontSize==10&&frame.Opacity==1,"Original success shape defaults");Check(frame.PositionX==5&&frame.PositionY==6,"Integer/float subscribed display position");
  var errorRecords=native.RootElement.GetProperty("uiRun").GetProperty("events").EnumerateArray().Single(p=>p.GetProperty("phase").GetString()=="original-error-records").GetProperty("records");
  foreach(var error in errorRecords.EnumerateArray()){var text=error.GetProperty("content").GetString()!;var record=ToolDisplayService.Build(definition,settings,inputs,outputs,diagnostic:new(error.GetProperty("error").GetUInt32(),text.Split("错误信息:")[1]));Check(record.RecordText==text,"Original GetRecordContent native error DTO");}
  var missing=native.RootElement.GetProperty("uiRun").GetProperty("events").EnumerateArray().Single(p=>p.GetProperty("phase").GetString()=="original-feature-missing");Check(ToolDisplayService.Build(definition,settings,new Dictionary<string,PortValue>(),new Dictionary<string,PortValue>()).Text==missing.GetProperty("absentFormatted")[0].GetString(),"Original all mappings absent produces empty text");Check(ToolDisplayService.Build(definition,settings,new Dictionary<string,PortValue>(),new Dictionary<string,PortValue>{{"RotError",PortValue.Number(.25)}}).Text==missing.GetProperty("partialFormatted")[0].GetString(),"Original partial mappings preserve labels and blank missing fields");
  outputs["ModuStatus"]=PortValue.Integer(0);var ng=ToolDisplayService.Build(definition,settings,inputs,outputs);Check(ng.Color=="#ff0000","Original status zero NG color");outputs["ModuStatus"]=PortValue.Integer(-1);Check(ToolDisplayService.Build(definition,settings,inputs,outputs).Color=="#66ff00","Original nonzero status OK color");
  var changed=settings with{Content="自定义 {RotCenterImageX}/{remote}",OkColor="#112233",NgColor="#445566",FontSize=18,Opacity=.4,PositionX=7,PositionY=8};
  var parameters=new Dictionary<string,JsonElement>{{ToolDisplayService.SettingsParameter,ToolDisplayService.Serialize(changed)}};
  Check(ToolDisplayService.Read(parameters,definition)==changed,"Display settings persist separately from algorithm parameters");
  var custom=ToolDisplayService.Build(definition,changed,new Dictionary<string,PortValue>(),outputs,k=>k=="remote"?PortValue.Text("变量"):null);
  Check(custom.Text=="自定义 30/变量"&&custom.PositionX==7&&custom.PositionY==8,"Named mapping and literal positions");Check(custom.RecordText!=custom.Text,"Editing Show Text does not overwrite Data Record content");
  Check(!ToolDisplayService.Valid(changed with{Opacity=2})&&!ToolDisplayService.Valid(changed with{FontSize=5})&&!ToolDisplayService.Valid(changed with{OkColor="invalid"}),"Original bounds and valid colors");
  foreach(string name in new[]{ToolDisplayService.PositionXInput,ToolDisplayService.PositionYInput}){var port=plugin.Descriptor.Inputs.Single(p=>p.Name==name);Check(DataKinds.CanFlow(DataKind.Integer,port)&&DataKinds.Coerce(PortValue.Integer(6),port).Kind==DataKind.Number,"Integer display subscription promotion");}
  Exception? failure=null;var thread=new Thread(()=>{Window? window=null;try{
   var picture=new ToolDisplayImagePresenter();var record=new ToolDisplayRecordPresenter();var list=new ToolDisplayResultListPresenter();picture.Show(frame);record.Show(frame);list.Show(frame);
   var dock=new W.DockPanel();var bottom=new W.StackPanel{Height=140};W.DockPanel.SetDock(bottom,W.Dock.Bottom);bottom.Children.Add(record);bottom.Children.Add(list);dock.Children.Add(bottom);dock.Children.Add(picture);
   window=new Window{Content=dock,Width=720,Height=480,Left=-30000,Top=-30000,ShowInTaskbar=false,ShowActivated=false};window.Show();window.UpdateLayout();Check(record.Text==frame.RecordText,"Record presenter consumes content");Check(list.Columns.Count==7&&list.Items.Count==1,"Result list presenter consumes one row");Check(picture.Frame==frame,"Image presenter consumes exact frame");
   var bitmap=new RenderTargetBitmap(720,480,96,96,PixelFormats.Pbgra32);bitmap.Render(window);var encoder=new PngBitmapEncoder();encoder.Frames.Add(BitmapFrame.Create(bitmap));using(var file=File.Create(Path.Combine(root,"analysis/full-audit/rotate-calibration/checks/display-four-objects.png")))encoder.Save(file);var pixels=new byte[720*480*4];bitmap.CopyPixels(pixels,720*4,0);Check(Enumerable.Range(0,pixels.Length/4).Any(i=>pixels[i*4+1]>100&&pixels[i*4+1]>pixels[i*4+2]*1.5),"Green native status overlay renders pixels");
   picture.Show(ng);window.UpdateLayout();bitmap=new RenderTargetBitmap(720,480,96,96,PixelFormats.Pbgra32);bitmap.Render(window);bitmap.CopyPixels(pixels,720*4,0);Check(Enumerable.Range(0,pixels.Length/4).Any(i=>pixels[i*4+2]>100&&pixels[i*4+2]>pixels[i*4+1]*1.5),"NG red overlay renders pixels");
  }catch(Exception e){failure=e;}finally{window?.Close();}});thread.SetApartmentState(ApartmentState.STA);thread.Start();thread.Join();if(failure is not null)throw failure;
  return checks;
 }
}
