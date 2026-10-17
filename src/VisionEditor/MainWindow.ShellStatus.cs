using System.Windows;
using System.Windows.Controls;
using VisionFramework;

namespace VisionEditor;

public partial class MainWindow
{
 ShellTimeDialog? shellTimeDialog;

 // VMControls.WPF.Base.StatusBar.ZoomCallBackClick selects the 1.0 zoom tick.
 // The existing WPF slider owns the viewport transform, so reuse that path.
 void ResetCanvasZoomClick(object sender,RoutedEventArgs e)=>ZoomSlider.Value=1;

 void ShowTimeClick(object sender,RoutedEventArgs e)
 {
  CloseTimeDialog();
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
  var dialog=new ShellTimeDialog(records.Values.Sum(r=>r.ElapsedMilliseconds),rows){Owner=this};
  shellTimeDialog=dialog;
  ProcedureTabs.SelectionChanged+=CloseTimeOnProcedureChange;
  dialog.Closed+=(_,_)=>{if(ReferenceEquals(shellTimeDialog,dialog)){shellTimeDialog=null;ProcedureTabs.SelectionChanged-=CloseTimeOnProcedureChange;}};
  dialog.Show();
 }
 void CloseTimeDialog()
 {
  var dialog=shellTimeDialog;
  if(dialog is null)return;
  shellTimeDialog=null;
  ProcedureTabs.SelectionChanged-=CloseTimeOnProcedureChange;
  dialog.Close();
 }
 void CloseTimeOnProcedureChange(object sender,SelectionChangedEventArgs e)
 {
  if(ReferenceEquals(e.OriginalSource,ProcedureTabs))CloseTimeDialog();
 }
}
