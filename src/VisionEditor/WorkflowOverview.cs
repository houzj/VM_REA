using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VisionFramework;
using Point=System.Windows.Point;
using Brushes=System.Windows.Media.Brushes;
using Orientation=System.Windows.Controls.Orientation;
using HorizontalAlignment=System.Windows.HorizontalAlignment;
using Image=System.Windows.Controls.Image;
using Color=System.Windows.Media.Color;
namespace VisionEditor;
public partial class MainWindow
{
 readonly Dictionary<string,(long Count,double Milliseconds)> workflowRunStats=new();
 WorkflowImageButton WorkflowVector(string key,string tip,Action? action,bool enabled=true)
 {
  var button=new WorkflowImageButton {Style=(Style)FindResource("WorkflowImageButton"),ToolTip=tip,IsEnabled=enabled,NormalSource=(ImageSource)FindResource(key+"normalDrawingImage"),HoverSource=(ImageSource)FindResource(key+"hoverDrawingImage"),PressedSource=(ImageSource)FindResource(key+"clickDrawingImage"),DisabledSource=(ImageSource)FindResource(key+"disableDrawingImage")};
  if(action is not null)button.Click+=(_,e)=>{e.Handled=true;Safe(action);};return button;
 }
 void RecordWorkflowRun(string id,double elapsed){var old=workflowRunStats.GetValueOrDefault(id);workflowRunStats[id]=(old.Count+1,elapsed);}
 void WorkflowViewClick(object sender,RoutedEventArgs e)
 {
  if(workflowListPopup is not null)workflowListPopup.IsOpen=false;
  if(toolPopup is not null)toolPopup.IsOpen=false;
  ResetEditingGesture();
  canvasViews[workflowOverview?"__solution":doc.CurrentProcedureId]=(scale,panX,panY);
  workflowOverview=!workflowOverview;
  var view=canvasViews.GetValueOrDefault(workflowOverview?"__solution":doc.CurrentProcedureId,(1d,0d,0d));
  scale=view.Item1;panX=view.Item2;panY=view.Item3;canvasProcedure=doc.CurrentProcedureId;
  selected=null;selectedNodes.Clear();RefreshAll();
 }
 void OpenWorkflowFromOverview(string id){workflowOverview=false;SelectWorkflow(id);canvasProcedure=null;RefreshAll();}
 void RenderWorkflowOverview()
 {
  GraphLayer.Children.Clear();nodeCards.Clear();GraphLayer.Width=GraphLayer.Height=5000;Breadcrumb.Text="";
  int index=0;
  foreach(var procedure in doc.Document.Procedures.Where(p=>p.GroupOwner is null))
  {
   // PROCESS template: original 140x120 surface, 30px title and 46px control area.
   var position=new Point(procedure.OverviewX??(index%3)*150,procedure.OverviewY??(index/3)*150);index++;
   var card=CreateWorkflowOverviewCard(procedure);
   Canvas.SetLeft(card,position.X);Canvas.SetTop(card,position.Y);GraphLayer.Children.Add(card);
  }
  ApplyCanvasTransform();
 }
 Grid CreateWorkflowOverviewCard(ProcedureDocument procedure,bool preview=false)
 {
   var card=new Grid {Width=140,Height=120,Tag=procedure.Id,ContextMenu=!preview&&!isLocked&&activeRun is null?ProcedureMenu(procedure.Id):null};
   card.RowDefinitions.Add(new RowDefinition {Height=new GridLength(30)});card.RowDefinitions.Add(new RowDefinition());
   var body=new Border {Background=new SolidColorBrush(Color.FromRgb(242,243,245)),CornerRadius=new CornerRadius(0,0,4,4)};Grid.SetRow(body,1);card.Children.Add(body);
   var inner=new Grid();inner.RowDefinitions.Add(new RowDefinition {Height=new GridLength(46)});inner.RowDefinitions.Add(new RowDefinition());body.Child=inner;
   var controls=new StackPanel {Orientation=Orientation.Horizontal,HorizontalAlignment=HorizontalAlignment.Center,VerticalAlignment=VerticalAlignment.Center};
   controls.Children.Add(WorkflowVector("ic_play_____","单次运行",()=>{_=RunProcedure(procedure.Id);},procedure.Nodes.Count>0&&activeRun is null));
   controls.Children.Add(WorkflowVector(continuousRun&&procedure.Id==runningProcedureId?"ic_stop_":"ic______","连续运行",()=>StartProcedureLoop(procedure.Id),procedure.Nodes.Count>0&&(activeRun is null||continuousRun&&procedure.Id==runningProcedureId)));
   controls.Children.Add(WorkflowIcon("ic_tab_group_configuration","流程配置",()=>OpenWorkflowConfiguration(procedure.Id),!preview&&!isLocked&&activeRun is null));inner.Children.Add(controls);
   var stats=workflowRunStats.GetValueOrDefault(procedure.Id);
   var details=new TextBlock {Text=$"{stats.Count} 次\n{stats.Milliseconds:N2}ms",FontSize=11,Foreground=new SolidColorBrush(Color.FromRgb(102,102,102)),Margin=new Thickness(12,6,0,0)};Grid.SetRow(details,1);inner.Children.Add(details);
   var title=new Border {Background=new SolidColorBrush(Color.FromRgb(40,42,48)),CornerRadius=new CornerRadius(4,4,0,0),Cursor=System.Windows.Input.Cursors.SizeAll};
   var titleRow=new StackPanel {Orientation=Orientation.Horizontal};
   titleRow.Children.Add(new System.Windows.Shapes.Ellipse {Width=8,Height=8,Fill=new SolidColorBrush(Color.FromRgb(236,0,0)),Margin=new Thickness(10,0,0,0),Visibility=GraphValidator.Validate(procedure,modules).IsValid?Visibility.Collapsed:Visibility.Visible});
   titleRow.Children.Add(new TextBlock {Text=procedure.Name,Width=90,FontSize=13,FontWeight=FontWeights.Bold,Foreground=new SolidColorBrush(Color.FromRgb(228,228,228)),VerticalAlignment=VerticalAlignment.Center,Margin=new Thickness(5,0,0,0),TextTrimming=TextTrimming.CharacterEllipsis});
   var open=WorkflowIcon("ic_展开","展开流程",()=>OpenWorkflowFromOverview(procedure.Id));titleRow.Children.Add(open);
   title.Child=titleRow;card.Children.Add(title);
   if(!preview)AttachOverviewDrag(title,card,procedure.Id);
   return card;
 }
}
