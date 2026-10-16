using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Point=System.Windows.Point;
using DragEventArgs=System.Windows.DragEventArgs;
using DragDropEffects=System.Windows.DragDropEffects;
using DataObject=System.Windows.DataObject;
using Border=System.Windows.Controls.Border;
using Brushes=System.Windows.Media.Brushes;
using Panel=System.Windows.Controls.Panel;
using DragAction=System.Windows.DragAction;
namespace VisionEditor;
public partial class MainWindow
{
    sealed record OverviewDragPayload(MainWindow Owner,string Solution,long Revision,string Id,double OffsetX,double OffsetY);
    const string OverviewDragFormat="VisionEditor.OverviewDrag.v1";
    bool ValidOverviewDrag(OverviewDragPayload p)=>EditableOverview&&ReferenceEquals(p.Owner,this)&&p.Solution==doc.SolutionId&&p.Revision==doc.Revision&&doc.Document.Procedures.Any(item=>item.Id==p.Id);
    DataObject CreateOverviewDragData(string id,Point offset)
    {
        string token=Guid.NewGuid().ToString("N");
        canvasDragData[token]=new OverviewDragPayload(this,doc.SolutionId,doc.Revision,id,offset.X,offset.Y);
        return new DataObject(OverviewDragFormat,token,false);
    }
    void AttachOverviewDrag(Border title,Grid card,string id)
    {
        bool armed=false;Point offset=default;
        title.PreviewMouseLeftButtonDown+=(_,e)=>
        {
            for(var current=e.OriginalSource as DependencyObject;current is not null&&current!=title;current=VisualTreeHelper.GetParent(current))if(current is System.Windows.Controls.Primitives.ButtonBase)return;
            if(e.ClickCount==2){armed=false;OpenWorkflowFromOverview(id);e.Handled=true;return;}
            if(!EditableOverview)return;armed=true;offset=e.GetPosition(card);e.Handled=true;
        };
        title.PreviewMouseLeftButtonUp+=(_,_)=>armed=false;
        title.PreviewMouseMove+=(_,e)=>
        {
            if(!armed)return;armed=false;
            if(e.LeftButton!=MouseButtonState.Pressed||!EditableOverview)return;
            var data=CreateOverviewDragData(id,offset);string token=(string)data.GetData(OverviewDragFormat)!;
            var payload=(OverviewDragPayload)canvasDragData[token];
            System.Windows.QueryContinueDragEventHandler query=(_,args)=>{if(!ValidOverviewDrag(payload))args.Action=DragAction.Cancel;};
            title.QueryContinueDrag+=query;
            try{DragDrop.DoDragDrop(title,data,(Keyboard.Modifiers&ModifierKeys.Control)!=0?DragDropEffects.Copy:DragDropEffects.Move);}
            catch(Exception error){if(!windowClosed)StatusText.Text="流程拖动已取消："+error.Message;}
            finally{title.QueryContinueDrag-=query;canvasDragData.Remove(token);ClearToolGhost();if(!windowClosed)RenderGraph();}
            e.Handled=true;
        };
    }
    bool HandleOverviewDrag(DragEventArgs e,bool drop)
    {
        if(ReadDragPayload(e.Data,OverviewDragFormat) is not OverviewDragPayload payload)return false;
        var effect=ValidOverviewDrag(payload)&&IsCanvasDropPoint(e.GetPosition(CanvasViewport))?NodeDropEffect(e.AllowedEffects,e.KeyStates):DragDropEffects.None;
        if(drop&&effect==DragDropEffects.Copy&&e.KeyStates!=DragDropKeyStates.ControlKey)effect=DragDropEffects.None;
        e.Handled=true;e.Effects=effect;
        if(effect==DragDropEffects.None){ClearToolGhost();return true;}
        var point=e.GetPosition(GraphLayer);double x=Math.Clamp(Snap(point.X-payload.OffsetX),0,DiagramPageSize-140),y=Math.Clamp(Snap(point.Y-payload.OffsetY),0,DiagramPageSize-120);
        if(drop)
        {
            ClearToolGhost();
            if(effect==DragDropEffects.Copy)doc.DuplicateProcedure(payload.Id,overviewX:x,overviewY:y);else doc.MoveProcedure(payload.Id,x,y);
            return true;
        }
        if(toolDragGhost is null)
        {
            var original=GraphLayer.Children.OfType<Grid>().FirstOrDefault(item=>(string?)item.Tag==payload.Id);
            toolDragGhost=new Border {Width=140,Height=120,Background=original is null?Brushes.LightGray:new VisualBrush(original){Stretch=Stretch.Fill},Opacity=.7,IsHitTestVisible=false};
            Panel.SetZIndex(toolDragGhost,int.MaxValue);GraphLayer.Children.Add(toolDragGhost);
        }
        Canvas.SetLeft(toolDragGhost,x);Canvas.SetTop(toolDragGhost,y);return true;
    }
}
