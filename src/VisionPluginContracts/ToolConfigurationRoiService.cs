using System.Text.Json;
using System.Windows;
using System.Windows.Media;
using VisionCore;
using VisionFramework;
using VisionUi;
using Brush=System.Windows.Media.Brush;
using Pen=System.Windows.Media.Pen;
using FlowDirection=System.Windows.FlowDirection;
namespace VisionPluginContracts;

public sealed record ConfigurationRoiRequest(string NodeId,string SourceName,string InputPort,RoiEditorCapabilities Capabilities,string Caption);
public sealed record ConfigurationRoiSnapshot(NodeDocument Node,IImageData? Image,ToolDisplayFrame? Display,bool Editable);
public interface IToolConfigurationRoiSession:IDisposable {
 GroupRoiEditor Editor{get;}
 RoiEditorSourceParts Parts{get;}
 ConfigurationRoiSnapshot Snapshot{get;}
 string? Diagnostic{get;}
 event Action? Changed;
 void Refresh();
}
public interface IToolConfigurationRoiService {IToolConfigurationRoiSession Open(ConfigurationRoiRequest request);}

/// <summary>Source ordinary ROISelecter + external image view. Managed geometry is not native IUserBytesData ABI.</summary>
public sealed class ToolConfigurationRoiService(
 string nodeId,Func<bool> active,Func<ConfigurationRoiRequest,ConfigurationRoiSnapshot> read,
 Action<string,Dictionary<string,JsonElement>,IReadOnlyList<RoiDocument>> apply,
 Func<Dictionary<string,JsonElement>,Task<NodeRunRecord>> run,
 Action<ConfigurationRoiRequest,FrameworkElement?> mount):IToolConfigurationRoiService {
 readonly Dictionary<string,Session> current=new(StringComparer.Ordinal);
 public IToolConfigurationRoiSession Open(ConfigurationRoiRequest request){
  if(request.NodeId!=nodeId||!active())throw new InvalidOperationException("ROI configuration owner is no longer active.");
  if(current.TryGetValue(request.SourceName,out var old))old.Dispose();var session=new Session(request,active,read,apply,run,mount,()=>current.Remove(request.SourceName));current[request.SourceName]=session;return session;
 }
 sealed class Session:IToolConfigurationRoiSession {
  readonly ConfigurationRoiRequest request;readonly Func<bool> active;readonly Func<ConfigurationRoiRequest,ConfigurationRoiSnapshot> read;
  readonly Action<string,Dictionary<string,JsonElement>,IReadOnlyList<RoiDocument>> apply;readonly Func<Dictionary<string,JsonElement>,Task<NodeRunRecord>> run;readonly Action<ConfigurationRoiRequest,FrameworkElement?> mount;readonly Action closed;
  bool disposed,refreshing,writing,running;string geometry="";IImageData? previousImage;
  public GroupRoiEditor Editor{get;}
  public RoiEditorSourceParts Parts{get;}
  public ConfigurationRoiSnapshot Snapshot{get;private set;}
  public string? Diagnostic{get;private set;}
  public event Action? Changed;
  public Session(ConfigurationRoiRequest request,Func<bool> active,Func<ConfigurationRoiRequest,ConfigurationRoiSnapshot> read,Action<string,Dictionary<string,JsonElement>,IReadOnlyList<RoiDocument>> apply,Func<Dictionary<string,JsonElement>,Task<NodeRunRecord>> run,Action<ConfigurationRoiRequest,FrameworkElement?> mount,Action closed){
   this.request=request;this.active=active;this.read=read;this.apply=apply;this.run=run;this.mount=mount;this.closed=closed;
   Snapshot=read(request);Editor=new(InitialRois(Snapshot.Node,request.SourceName),request.Capabilities);Parts=Editor.DetachSourceParts(request.Caption,request.Capabilities.Maximum==1);
   Editor.ImageOverlayRenderer=DrawOverlay;Editor.GeometryCommitted+=Commit;mount(request,Parts.Viewport);Refresh();
  }
  static IReadOnlyList<RoiDocument> InitialRois(NodeDocument node,string source)=>node.RoiGroups?.GetValueOrDefault(source)??(source is "RoiType" or "RoiType_0"?node.Rois:Array.Empty<RoiDocument>())??Array.Empty<RoiDocument>();
  public void Refresh(){
   if(disposed||refreshing||writing)return;refreshing=true;
   try{
    if(!active()){Dispose();return;}Snapshot=read(request);
    string next=JsonSerializer.Serialize(InitialRois(Snapshot.Node,request.SourceName));
    if(next!=geometry){Editor.RefreshSourceRegions(InitialRois(Snapshot.Node,request.SourceName));geometry=next;}
    if(!ReferenceEquals(previousImage,Snapshot.Image)){Editor.SetSourceImage(Snapshot.Image);previousImage=Snapshot.Image;}
    Editor.SourceImageVisible=Snapshot.Display?.ImageVisible??true;
    bool enabled=Snapshot.Editable&&!running&&Snapshot.Image is not null;Parts.Selector.IsEnabled=Parts.Fields.IsEnabled=Parts.Viewport.IsEnabled=enabled;
    Editor.InvalidateViewport();Changed?.Invoke();
   }catch(Exception error){Diagnostic=error.Message;Parts.Selector.IsEnabled=Parts.Fields.IsEnabled=Parts.Viewport.IsEnabled=false;Changed?.Invoke();}
   finally{refreshing=false;}
  }
  async void Commit(object? sender,EventArgs args){
   if(disposed||refreshing||writing)return;
   try{
    if(!active())throw new InvalidOperationException("ROI configuration session has ended.");
    var current=read(request);if(!current.Editable||running)throw new InvalidOperationException("ROI is not editable while the procedure is running or locked.");
    var regions=Editor.Regions;RoiGeometry.ValidateForEditor(regions,request.Capabilities);
    writing=true;try{apply(request.SourceName,current.Node.Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone()),regions);}finally{writing=false;}
    Diagnostic=null;running=true;Refresh();
    // ROISelecter.SetValue: write ROI + sequence before PreExecuteRun.
    try{await run(read(request).Node.Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone()));}finally{running=false;}
   }catch(Exception error){Diagnostic=error.Message;}
   finally{if(!disposed){geometry="";Refresh();}}
  }
  void DrawOverlay(DrawingContext dc,Matrix transform,double scale){
   if(disposed||Snapshot.Display is not {} frame)return;
   dc.PushTransform(new MatrixTransform(transform));
   foreach(var graphic in frame.Graphics.Where(g=>g.Name!="ROI")){
    dc.PushOpacity(graphic.Opacity);
    void Draw(ToolDisplayGraphic item,bool markers=true){
     var brush=(Brush)new BrushConverter().ConvertFromInvariantString(item.Color)!;
     dc.DrawGeometry(item.Type=="coordinate"?brush:null,new Pen(brush,item.StrokeThickness/scale),ToolDisplayGraphicGeometry.Create(item,Snapshot.Image?.Width??640,scale,markers));
    }
    if(graphic.Type=="text"){
     if(graphic.FontSize>0){var brush=(Brush)new BrushConverter().ConvertFromInvariantString(graphic.Color)!;foreach(var point in graphic.Points)dc.DrawText(new FormattedText(point.Text??"",System.Globalization.CultureInfo.CurrentCulture,FlowDirection.LeftToRight,new Typeface("Microsoft YaHei"),graphic.FontSize,brush,1),new System.Windows.Point(point.X,point.Y));}
    }else if(graphic.Type is "point" or "path"){
     if(graphic.Type=="path")Draw(graphic,false);
     foreach(var point in graphic.Points)Draw(graphic with{Type="point",Color=point.Color??graphic.Color,Points=new[]{point}});
    }else Draw(graphic);
    dc.Pop();
   }
   if(frame.TextVisible){var brush=(Brush)new BrushConverter().ConvertFromInvariantString(frame.Color)!;dc.PushOpacity(frame.Opacity);dc.DrawText(new FormattedText(frame.Text,System.Globalization.CultureInfo.CurrentCulture,FlowDirection.LeftToRight,new Typeface("Microsoft YaHei"),frame.FontSize,brush,1),new System.Windows.Point(frame.PositionX,frame.PositionY));dc.Pop();}
   dc.Pop();
  }
  public void Dispose(){if(disposed)return;disposed=true;Editor.GeometryCommitted-=Commit;Editor.ImageOverlayRenderer=null;Editor.CancelGesture();mount(request,null);Changed=null;closed();}
 }
}
