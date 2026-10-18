using System.Windows;
using System.Windows.Controls;
using VisionFramework;

namespace VisionEditor;

public partial class MainWindow
{
 ShellTimeDialog? shellTimeDialog;

 // StatusBar.xaml maps the source control's NOItem state to the config icon
 // and "等待添加组件". The native IsReady check for populated graphs remains
 // SDK-owned, so the independent validator only provides a visible fallback.
 void RefreshShellConfigurationStatus()
 {
  if(doc is null)return;
  if(!doc.HasCurrentProcedure || doc.CurrentProcedure.Nodes.Count==0)
  {
   StatusIcon.Source=Asset("bottombar_config.png");
   StatusText.Text="等待添加组件";
   return;
  }
  bool ready=GraphValidator.Validate(doc.CurrentProcedure,modules).IsValid;
  StatusIcon.Source=Asset(ready?"bottombar_run.png":"bottombar_abnormal.png");
  if(StatusText.Text=="等待添加组件")StatusText.Text=ready?"组件配置完成":"模块配置待完成";
 }

 // VMControls.WPF.Base.StatusBar.ZoomCallBackClick selects the 1.0 zoom tick.
 // The existing WPF slider owns the viewport transform, so reuse that path.
 void ResetCanvasZoomClick(object sender,RoutedEventArgs e)=>ZoomSlider.Value=1;

 void ShowTimeClick(object sender,RoutedEventArgs e)
 {
  shellTimeDialog?.Close();
  if(!doc.HasCurrentProcedure||doc.CurrentProcedure.Nodes.Count==0)return;
  var procedure=doc.CurrentProcedure;
  workflowSessions.TryGetValue(procedure.Id,out var session);
  var records=session?.Nodes.ToDictionary(n=>n.NodeId)??new Dictionary<string,NodeRunRecord>();
  var rows=procedure.Nodes.Select(node=>
  {
   records.TryGetValue(node.Id,out var run);
   // The independent executor exposes one elapsed value per node, not the
   // native SDK's separate tool and algorithm timings.
   return new ShellTimeDialog.TimeRow(node.Name,(run?.ElapsedMilliseconds??0).ToString("N2"),"—");
  }).ToArray();
  shellTimeDialog=new ShellTimeDialog(records.Values.Sum(r=>r.ElapsedMilliseconds),rows){Owner=this};
  ProcedureTabs.SelectionChanged+=CloseTimeOnProcedureChange;
  shellTimeDialog.Closed+=(_,_)=>{ProcedureTabs.SelectionChanged-=CloseTimeOnProcedureChange;shellTimeDialog=null;};
  shellTimeDialog.Show();
 }
 void CloseTimeOnProcedureChange(object sender,SelectionChangedEventArgs e)
 {
  if(ReferenceEquals(e.OriginalSource,ProcedureTabs))shellTimeDialog?.Close();
 }
}
