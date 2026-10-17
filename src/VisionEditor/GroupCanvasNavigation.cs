using System.Windows.Input;
namespace VisionEditor;
public partial class MainWindow
{
 SubprogramCallWindow? displayedGroupConfiguration;
 void OpenDisplayedConfiguration()
 {
  if(displayedGroupConfiguration is {IsVisible:true}){displayedGroupConfiguration.Activate();return;}
  if(!CanEditProcedure(doc.CurrentProcedureId))return;
  if(doc.CurrentProcedure.GroupOwner is not {} owner){OpenWorkflowConfiguration(doc.CurrentProcedureId);return;}
  ResetEditingGesture();var window=CreateBoundGroupWindow(owner.ProcedureId,owner.NodeId);
  displayedGroupConfiguration=window;parameterEditorOpen=true;
  window.Closed+=(_,_)=>{if(ReferenceEquals(displayedGroupConfiguration,window)){displayedGroupConfiguration=null;parameterEditorOpen=false;if(!windowClosed)RefreshAll();}};
  try{window.Show();RefreshAll();}catch{displayedGroupConfiguration=null;parameterEditorOpen=false;throw;}
 }
 string GroupBreadcrumb(){var names=new List<string>();var current=doc.CurrentProcedure;names.Add(current.Name);while(current.GroupOwner is {} owner){current=doc.Document.Procedures.Single(p=>p.Id==owner.ProcedureId);names.Insert(0,current.Name);}return string.Join(" > ",names)+" > "+(doc.CurrentProcedure.Nodes.FirstOrDefault(n=>n.Id==selected)?.Name??"");}
 void ReturnFromGroup(){if(doc.CurrentProcedure.GroupOwner is null||parameterEditorOpen||activeRun is not null)return;ResetEditingGesture();ClearToolGhost();if(toolPopup is not null)toolPopup.IsOpen=false;selected=null;selectedNodes.Clear();selectedWire=null;doc.LeaveGroup();}
 void GroupBreadcrumbBack(object sender,MouseButtonEventArgs e){if(doc.CurrentProcedure.GroupOwner is null||parameterEditorOpen||activeRun is not null)return;ReturnFromGroup();e.Handled=true;}
}
