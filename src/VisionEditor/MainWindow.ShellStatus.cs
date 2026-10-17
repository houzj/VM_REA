using System.Windows;
using System.Windows.Controls;
using VisionFramework;

namespace VisionEditor;

public partial class MainWindow
{
 ShellTimeDialog? shellTimeDialog;

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
