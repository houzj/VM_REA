using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Point=System.Windows.Point;
using DragEventArgs=System.Windows.DragEventArgs;
using DragDropEffects=System.Windows.DragDropEffects;
using DragDropKeyStates=System.Windows.DragDropKeyStates;
using DataObject=System.Windows.DataObject;
using MouseEventArgs=System.Windows.Input.MouseEventArgs;
using QueryContinueDragEventHandler=System.Windows.QueryContinueDragEventHandler;
using DragAction=System.Windows.DragAction;
namespace VisionEditor;
public partial class MainWindow
{
 sealed record NodeDragPayload(MainWindow Owner,string Solution,string Procedure,long Revision);
 sealed record ToolDragPayload(MainWindow Owner,string Solution,string Procedure,long Revision,string Module,bool Overview);
 const string NodeDragFormat="VisionEditor.NodeDrag.v1",ToolDragFormat="VisionEditor.ToolDrag.v1";
 readonly Dictionary<string,object> canvasDragData=new(StringComparer.Ordinal);
 string? toolDragToken;
 bool nodeOleArmed,nodeOleActive;
 bool parameterEditorOpen;
 NodeDragPayload? nodeDragPayload;
 long canvasGestureRevision;
 bool HasEditingGesture=>movingNodes is not null||pendingConnection is not null||wireDrag is not null||boxSelecting;
 bool CurrentDragOrigin(MainWindow owner,string solution,string procedure,long revision)=>ReferenceEquals(owner,this)&&EditableCanvas&&doc.SolutionId==solution&&doc.CurrentProcedureId==procedure&&doc.Revision==revision;
 static DragDropEffects NodeDropEffect(DragDropEffects allowed,DragDropKeyStates keys)
 {
  // DiagramItem/DiagramPage latch AllowedEffects at DoDragDrop entry.
  // ItemsPage.OnDrop accepts Copy only when the released key state is exactly Control.
  var modifiers=keys&(DragDropKeyStates.ControlKey|DragDropKeyStates.ShiftKey|DragDropKeyStates.AltKey);
  if(allowed==DragDropEffects.Copy)return modifiers==DragDropKeyStates.ControlKey?DragDropEffects.Copy:DragDropEffects.None;
  return allowed==DragDropEffects.Move&&(modifiers&DragDropKeyStates.ControlKey)==0?DragDropEffects.Move:DragDropEffects.None;
 }
 bool ValidNodeDrag(NodeDragPayload payload)=>CurrentDragOrigin(payload.Owner,payload.Solution,payload.Procedure,payload.Revision)&&movingNodes is {Count:>0};
 void InitializeInputBoundaries()
 {
  CanvasViewport.AllowDrop=true;
  CanvasViewport.PreviewDragOver+=CanvasDragOver;
  CanvasViewport.PreviewDragEnter+=CanvasDragOver;
  CanvasViewport.PreviewDrop+=CanvasDrop;
  CanvasViewport.DragLeave+=(_,e)=>{var point=e.GetPosition(CanvasViewport);if(IsCanvasDropPoint(point))return;if(nodeOleActive)HideNodePreview();ClearToolGhost();};
  Deactivated+=(_,_)=>{if(!nodeOleActive&&!toolDragStarted){CancelCanvasGesture();ClearCanvasFocusAdorners();}};
  GraphLayer.LostKeyboardFocus+=(_,_)=>Dispatcher.BeginInvoke(new Action(()=>{
   if(GraphLayer.IsKeyboardFocusWithin||HasEditingGesture||nodeOleActive)return;
   // DiagramPage.RectDragPanal_LostFocus and Connection.LostFocus remove adorners.
   ClearCanvasFocusAdorners();
  }));
 }
 void ClearCanvasFocusAdorners(){boxSelectionActive=false;selectedWire=null;DrawSelectionGroup();DrawWireHandles();foreach(var entry in wireViews.Values)entry.View.Stroke=wireNormalBrush;}
 void StartNodeOleDrag(MouseEventArgs e)
 {
  nodeOleArmed=false;
  if(e.LeftButton!=MouseButtonState.Pressed||!EditableCanvas){CancelCanvasGesture();return;}
  nodeCopyDrag=(Keyboard.Modifiers&ModifierKeys.Control)!=0;
  var allowed=nodeCopyDrag?DragDropEffects.Copy:DragDropEffects.Move;
  RunNodeOleDrag(allowed);
 }
 void RunNodeOleDrag(DragDropEffects allowed)
 {
  if(!EditableCanvas||movingNodes is not {Count:>0})return;
  if(allowed is not (DragDropEffects.Move or DragDropEffects.Copy)){CancelCanvasGesture();StatusText.Text="拖动已取消：不支持的拖放模式";return;}
  nodeCopyDrag=allowed==DragDropEffects.Copy;nodeOleArmed=false;
  nodeDragPayload=new(this,doc.SolutionId,doc.CurrentProcedureId,doc.Revision);nodeOleActive=true;
  CanvasViewport.ReleaseMouseCapture();
  QueryContinueDragEventHandler query=(_,args)=>{if(nodeDragPayload is not {} payload||!ValidNodeDrag(payload))args.Action=DragAction.Cancel;};
  GraphLayer.QueryContinueDrag+=query;
  string token=Guid.NewGuid().ToString("N");canvasDragData[token]=nodeDragPayload;
  // Only an opaque string crosses OLE; live Window/model references stay in this window.
  try{DragDrop.DoDragDrop(GraphLayer,new DataObject(NodeDragFormat,token,false),allowed);}
  catch(Exception error){if(!windowClosed)StatusText.Text="拖动已取消："+error.Message;}
  finally{canvasDragData.Remove(token);GraphLayer.QueryContinueDrag-=query;nodeOleActive=false;nodeDragPayload=null;CancelNodeMove();if(!windowClosed)RenderGraph();}
 }
 void CanvasDragOver(object sender,DragEventArgs e)
 {
  if(HandleOverviewDrag(e,false))return;
  if(ReadDragPayload(e.Data,NodeDragFormat) is NodeDragPayload payload){
   e.Effects=ValidNodeDrag(payload)&&IsCanvasDropPoint(e.GetPosition(CanvasViewport))?NodeDropEffect(e.AllowedEffects,e.KeyStates):DragDropEffects.None;
   if(e.Effects!=DragDropEffects.None)PreviewNodeMove(e.GetPosition(GraphLayer));else HideNodePreview();e.Handled=true;return;
  }
  ToolDropOver(sender,e);
 }
 void CanvasDrop(object sender,DragEventArgs e)
 {
  if(HandleOverviewDrag(e,true))return;
  if(ReadDragPayload(e.Data,NodeDragFormat) is NodeDragPayload payload){
   var effect=ValidNodeDrag(payload)&&IsCanvasDropPoint(e.GetPosition(CanvasViewport))?NodeDropEffect(e.AllowedEffects,e.KeyStates):DragDropEffects.None;
   if(effect==DragDropEffects.Copy&&e.KeyStates!=DragDropKeyStates.ControlKey)effect=DragDropEffects.None;
   e.Effects=effect;
   if(effect!=DragDropEffects.None){nodeCopyDrag=effect==DragDropEffects.Copy;PreviewNodeMove(e.GetPosition(GraphLayer));CommitNodeMove();}else CancelNodeMove();e.Handled=true;return;
  }
  ToolDrop(sender,e);
 }
 object? ReadDragPayload(System.Windows.IDataObject data,string format)=>data.GetData(format,false) is string token&&canvasDragData.TryGetValue(token,out var value)?value:null;
 DataObject CreateToolDragData(string id){ReleaseToolDragData();toolDragToken=Guid.NewGuid().ToString("N");canvasDragData[toolDragToken]=new ToolDragPayload(this,doc.SolutionId,doc.CurrentProcedureId,doc.Revision,id,workflowOverview);return new(ToolDragFormat,toolDragToken,false);}
 void ReleaseToolDragData(){if(toolDragToken is not null)canvasDragData.Remove(toolDragToken);toolDragToken=null;}
 void HideNodePreview()
 {
  ClearAlignmentGuides();ClearNodeHotRegion();ClearInsertionWire();foreach(var ghost in movingGhosts.Values)GraphLayer.Children.Remove(ghost);movingGhosts.Clear();
  if(groupMoveFrame is not null)GraphLayer.Children.Remove(groupMoveFrame);groupMoveFrame=null;
 }
 void ResetEditingGesture()
 {
  // Clear state before releasing capture so LostMouseCapture cannot recursively commit/cancel.
  nodeOleArmed=false;nodeDragPayload=null;pendingConnection=null;connectionTarget=null;wireDrag=null;draggedWire=null;wireDragOriginalPoints=null;boxSelecting=false;panning=false;eyeDragging=false;MiniMapCanvas.ReleaseMouseCapture();
  if(selectionBox is not null)GraphLayer.Children.Remove(selectionBox);selectionBox=null;
  if(connectionPreview is not null)GraphLayer.Children.Remove(connectionPreview);connectionPreview=null;
  CancelNodeMove();ClearToolGhost();
 }
 void CheckGestureBoundary()
 {
  if(HasEditingGesture&&(!EditableCanvas||canvasGestureRevision!=doc.Revision))ResetEditingGesture();
 }
}
