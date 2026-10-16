using System.Windows.Input;
namespace VisionEditor;
public partial class MainWindow
{
 string GroupBreadcrumb(){var names=new List<string>();var current=doc.CurrentProcedure;names.Add(current.Name);while(current.GroupOwner is {} owner){current=doc.Document.Procedures.Single(p=>p.Id==owner.ProcedureId);names.Insert(0,current.Name);}return string.Join(" > ",names)+" > "+(doc.CurrentProcedure.Nodes.FirstOrDefault(n=>n.Id==selected)?.Name??"");}
 void ReturnFromGroup(){if(doc.CurrentProcedure.GroupOwner is null||parameterEditorOpen||activeRun is not null)return;ResetEditingGesture();ClearToolGhost();if(toolPopup is not null)toolPopup.IsOpen=false;selected=null;selectedNodes.Clear();selectedWire=null;doc.LeaveGroup();}
 void GroupBreadcrumbBack(object sender,MouseButtonEventArgs e){if(doc.CurrentProcedure.GroupOwner is null||parameterEditorOpen||activeRun is not null)return;ReturnFromGroup();e.Handled=true;}
}
