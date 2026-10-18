using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;
using VisionUi;
static class Program {
 static int checks;static void Check(bool valid,string message){if(!valid)throw new Exception(message);checks++;}
 [STAThread]static int Main(){try{Run();Console.WriteLine($"PASS {checks} source display-object/graphics checks");return 0;}catch(Exception error){Console.Error.WriteLine(error);return 1;}}
 static void Run(){
  var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};var definitions=NPointPlugin.DisplayObjectDefinitions;
  Check(definitions.Select(d=>d.Name).SequenceEqual(new[]{"Show Text","Image Point","RotAxisPoint","Data Record","Result List"}),"all source display objects retain ordering");
  Check(!definitions.Single(d=>d.Type=="path").StrokeThicknessConfigEnabled&&definitions.Single(d=>d.Type=="point").StrokeThicknessConfigEnabled,"native object type controls optional thickness field");
  var duplicate=ToolDisplayObjectDefinition.Parse("<ParamRoot><Object Name='Data Record' Type='datarecord'/><Object Name='Data Record' Type='datarecord'/></ParamRoot>");
  Check(duplicate.Count==2,"duplicate non-editable source records remain ordered");
  bool rejected=false;var values=new Dictionary<string,JsonElement>();var store=new ToolDisplayObjectStore(definitions,()=>values,next=>{if(rejected)throw new InvalidOperationException("Host rejected");values=next;});
  Check(new ToolDisplayObjectStore(duplicate,()=>values,_=>{}).Read("Data Record") is null,"ambiguous named bridge fails explicitly without deleting source definitions");
  Check(store.Read("Image Point")!.Style.OkColor=="#66ff00"&&store.Read("RotAxisPoint")!.Style.OkColor=="#0000ff","display defaults come from source object rather than shared global colors");
  var model=ToolConfigurationDefinition.Parse("<AlgorithmTabRoot><Tabs><Tab Name='Result'><Items><ImageViewConfig Name='Path'><BrigdgeSetter><OperationName>GetDisplayObjectOperation</OperationName><OperationParams>Image Point</OperationParams></BrigdgeSetter></ImageViewConfig><ImageViewConfig Name='Point'><BrigdgeSetter><OperationName>GetDisplayObjectOperation</OperationName><OperationParams>RotAxisPoint</OperationParams></BrigdgeSetter></ImageViewConfig></Items></Tab></Tabs></AlgorithmTabRoot>");
  var state=new ToolConfigurationState(model);var controls=new ToolConfigurationControlRegistry();ToolConfigurationImageObjectControls.Register(controls,store);
  using(var view=new ToolConfigurationView(state,controls)){
   var window=new Window{Content=view,Width=420,Height=500,ShowActivated=false,ShowInTaskbar=false,Left=-30000,Top=-30000};window.Show();window.UpdateLayout();
   var editors=Walk(view).OfType<RoiDisplayStyleEditor>().ToArray();Check(editors.Length==2&&view.Diagnostics.Count==0,"source bridge renders both real reusable editors");
   var path=editors.Single(e=>e.Value.OkColor=="#66ff00");var point=editors.Single(e=>e.Value.OkColor=="#0000ff");
   Check(((Border)path.Popup.Child).Height==120&&((Border)point.Popup.Child).Height==150,"source popup heights follow stroke support");Check(path.Popup.HorizontalOffset==-90&&((Border)path.Popup.Child).Width==260,"source popup width and placement");
   path.Eye.IsChecked=false;path.Eye.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));Check(!store.Read("Image Point")!.Style.Visible&&store.Read("RotAxisPoint")!.Style.Visible,"eye writes only matching source object");
   path.Eye.IsChecked=true;path.Eye.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));path.OpacityField.Step(-1);Check(Math.Abs(store.Read("Image Point")!.Style.Opacity-.99)<1e-10,"source opacity step writes presentation store");
   rejected=true;point.WidthField.Step(1);Check(point.WidthField.Input.Text=="1"&&store.Read("RotAxisPoint")!.Style.StrokeThickness==1,"host rejection restores visible numeric and accepted style");rejected=false;
   Check(state.Diagnostics.Any(d=>d.Code=="DisplayObjectWrite"),"rejection is diagnosed");
   point.Settings.IsChecked=true;var popupIdentity=point.Popup.Child;store.Write("RotAxisPoint",store.Read("RotAxisPoint")!.Style with{OkColor="#112233"});view.Refresh();Check(ReferenceEquals(popupIdentity,point.Popup.Child)&&point.Popup.IsOpen&&point.Value.OkColor=="#112233","external readback preserves open popup instance");point.Settings.IsChecked=false;
   window.Close();Check(!path.Popup.IsOpen&&!point.Popup.IsOpen,"unload closes source popups");
  }
  var outputs=new Dictionary<string,PortValue>{["OutImagePointX"]=PortValue.Numbers([10,110,110,10]),["OutImagePointY"]=PortValue.Numbers([20,20,120,120]),["WorldPointX"]=PortValue.Numbers([3,203,203,3]),["WorldPointY"]=PortValue.Numbers([4,4,204,204]),["CalibCurIndex"]=PortValue.Integer(4),["CalibStatus"]=PortValue.Integer(1),["RotCenterImageX"]=PortValue.Number(60),["RotCenterImageY"]=PortValue.Number(70)};
  var graphics=ToolDisplayGraphics.Build(definitions,values,new Dictionary<string,PortValue>(),outputs);Check(graphics.Count==2,"real source path and rotation axis project separately");var trajectory=graphics.Single(g=>g.Type=="path");var rotation=graphics.Single(g=>g.Type=="point");
  Check(trajectory.Points.Count==4&&trajectory.Points[1].X==110&&trajectory.Points[1].Tips=="ImagePos:(110,20) PhyPos:(203,4) ","source per-point tips match ordered native mappings");Check(rotation.Points.Single().X==60&&rotation.Color=="#112233","source rotation axis reads independent source style and scalar output");
  var shape=(GeometryGroup)ToolDisplayGraphicGeometry.Create(trajectory,640);Check(shape.Children.Count==17,"three arrowed segments plus four cross markers");var first=(LineGeometry)shape.Children[0];Check(Math.Abs(first.StartPoint.X-30)<1e-4&&Math.Abs(first.EndPoint.X-90)<1e-4,"source path uses float 0.2 through 0.8 segment");
  var marker=(GeometryGroup)ToolDisplayGraphicGeometry.Create(rotation,640,2);Check(Math.Abs(marker.Bounds.Width-3)<1e-9,"point marker uses source 3-times-thickness screen scaling");
  outputs["OutImagePointX"]=PortValue.Numbers([10]);outputs["OutImagePointY"]=PortValue.Numbers([20]);Check(ToolDisplayGraphics.Build(definitions,values,new Dictionary<string,PortValue>(),outputs).All(g=>g.Type!="path"),"source path suppresses fewer than two points");outputs["OutImagePointX"]=PortValue.Numbers([10,110,110,10]);outputs["OutImagePointY"]=PortValue.Numbers([20,20,120,120]);
  var frame=new ToolDisplayFrame(null,"","#66ff00",10,1,0,0,"",[]){Graphics=graphics};var presenter=new ToolDisplayImagePresenter();presenter.Show(frame);presenter.Measure(new Size(640,480));presenter.Arrange(new Rect(0,0,640,480));presenter.UpdateLayout();Check(Walk(presenter).OfType<System.Windows.Shapes.Path>().Count()==6,"shared image presenter consumes real graphic frames");
  var output=Path.GetFullPath("analysis/full-audit/display-object-graphics");Directory.CreateDirectory(output);var bitmap=new RenderTargetBitmap(640,480,96,96,PixelFormats.Pbgra32);bitmap.Render(presenter);var png=new PngBitmapEncoder();png.Frames.Add(BitmapFrame.Create(bitmap));using(var stream=File.Create(Path.Combine(output,"source-npoint-graphics.png")))png.Save(stream);
  presenter.Show(null);Check(!Walk(presenter).OfType<System.Windows.Shapes.Path>().Any(),"clear removes stale geometry");app.Shutdown();
 }
 static IEnumerable<DependencyObject> Walk(DependencyObject root){yield return root;for(int i=0;i<VisualTreeHelper.GetChildrenCount(root);i++)foreach(var child in Walk(VisualTreeHelper.GetChild(root,i)))yield return child;}
}
