using System.Windows;
using System.Windows.Controls;
using WpfTabControl=System.Windows.Controls.TabControl;
using WpfPanel=System.Windows.Controls.Panel;
using VisionCore;
using VisionFramework;
using VisionPluginContracts;
using VisionUi;
namespace VisionEditor;

public partial class MainWindow {
 FrameworkElement? configurationRoiViewport;
 GroupRoiEditor? configurationRoiEditor;
 IToolConfigurationRoiSession? configurationRoiLease;
 readonly Dictionary<string,IToolConfigurationRoiSession> configurationRoiLeases=new(StringComparer.Ordinal);
 readonly Dictionary<string,(string Caption,FrameworkElement View)> configurationRoiViews=new(StringComparer.Ordinal);
 ToolEditorContext ConnectConfigurationRoi(ToolEditorContext context,string procedure,string node,string solution){
  bool Active()=>!windowClosed&&doc.SolutionId==solution&&doc.CurrentProcedureId==procedure&&(context.ConfigurationActive?.Invoke()??true)&&doc.Document.Procedures.Any(p=>p.Id==procedure&&p.Nodes.Any(n=>n.Id==node));
  bool Editable()=>Active()&&!isLocked&&activeRun is null&&activeConfigurationRuns==0;
  ProcedureDocument Flow()=>doc.Document.Procedures.Single(p=>p.Id==procedure);
  ConfigurationRoiSnapshot Read(ConfigurationRoiRequest request){
   if(!Active()||request.NodeId!=node)throw new InvalidOperationException("ROI configuration owner has ended.");
   var target=Flow().Nodes.Single(n=>n.Id==node);var descriptor=modules.GetDescriptor(target);var capability=descriptor.RoiEditor??descriptor.Parameters.Select(p=>p.Editor).FirstOrDefault(e=>e?.Kind==ParameterEditorKind.Roi)?.Roi;
   if(capability is null||!capability.Kinds.SequenceEqual(request.Capabilities.Kinds)||capability.Maximum!=request.Capabilities.Maximum||capability.FullImage!=request.Capabilities.FullImage||capability.PolygonPointMaximum!=request.Capabilities.PolygonPointMaximum||capability.Exclusions!=request.Capabilities.Exclusions)throw new InvalidOperationException("ROI source declaration does not match the registered tool capability.");
   IImageData? image=null;
   try{
    var records=new Dictionary<string,NodeRunRecord>(StringComparer.Ordinal);
    if(workflowSessions.TryGetValue(procedure,out var completed))foreach(var record in completed.Nodes)records[record.NodeId]=record;
    foreach(var pair in toolDebugRecords.Where(p=>p.Key.SolutionId==solution&&p.Key.ProcedureId==procedure))records[pair.Key.NodeId]=pair.Value;
    if(configurationResults.TryGetValue(procedure,out var configured))foreach(var pair in configured)records[pair.Key]=pair.Value;
    var previous=workflowSessions.GetValueOrDefault(procedure);
    image=new NodeDebugExecutor(modules).ResolveInput(Flow(),node,request.InputPort,records.Values.ToArray(),previous?.WorkflowInputs,previous?.GroupLoopIndex).Value as IImageData;
   }catch(System.IO.InvalidDataException){}
   var display=(plugins.Find(target.ModuleId) as IToolDisplayProvider)?.LastDisplay(target,doc.BaseDirectory,executionScope.ForProcedure(procedure));
   return new(target,image,display,Editable());
  }
  void Mount(ConfigurationRoiRequest request,FrameworkElement? view){
   if(view is null){configurationRoiViews.Remove(request.SourceName);configurationRoiLeases.Remove(request.SourceName);}
   else configurationRoiViews[request.SourceName]=(request.Caption,view);
   var panel=(Grid)Preview.Parent;
   if(configurationRoiViewport is {} old)panel.Children.Remove(old);
   foreach(var item in configurationRoiViews.Values){if(item.View.Parent is WpfPanel parent)parent.Children.Remove(item.View);else if(item.View.Parent is ContentControl content&&ReferenceEquals(content.Content,item.View))content.Content=null;}
   configurationRoiEditor=null;configurationRoiLease=null;configurationRoiViewport=null;
   if(configurationRoiViews.Count==1){var only=configurationRoiViews.Single();configurationRoiViewport=only.Value.View;panel.Children.Add(only.Value.View);ActivateRoiSource(only.Key);}
   else if(configurationRoiViews.Count>1){var tabs=new WpfTabControl();foreach(var item in configurationRoiViews)tabs.Items.Add(new TabItem{Header=item.Value.Caption,Content=item.Value.View,Tag=item.Key});tabs.SelectionChanged+=(_,_)=>ActivateSelectedRoiSource(tabs);configurationRoiViewport=tabs;panel.Children.Add(tabs);tabs.SelectedIndex=0;DisplayTabs.SelectedIndex=0;}
   else if(!windowClosed)RefreshRight();
  }
  var service=new ToolConfigurationRoiService(node,Active,Read,(sourceName,parameters,rois)=>{
   if(!Editable())throw new InvalidOperationException("当前模块不可编辑。");
   doc.SetProcedureToolConfigurationRoiGroup(procedure,node,sourceName,parameters,rois);
  },values=>context.Run(values),Mount);
  return context with{ConfigurationRois=new RoiSessionBridge(service,(source,session)=>{configurationRoiLeases[source]=session;if(configurationRoiViewport is WpfTabControl tabs)ActivateSelectedRoiSource(tabs);else ActivateRoiSource(source);})};
 }
 void ActivateRoiSource(string source){if(configurationRoiLeases.TryGetValue(source,out var session)){configurationRoiLease=session;configurationRoiEditor=session.Editor;}else{configurationRoiLease=null;configurationRoiEditor=null;}}
 void ActivateSelectedRoiSource(WpfTabControl tabs){if(tabs.SelectedItem is TabItem selected&&selected.Tag is string source)ActivateRoiSource(source);}
 sealed class RoiSessionBridge(IToolConfigurationRoiService inner,Action<string,IToolConfigurationRoiSession> opened):IToolConfigurationRoiService {
  public IToolConfigurationRoiSession Open(ConfigurationRoiRequest request){var session=inner.Open(request);opened(request.SourceName,session);return session;}
 }
 void RefreshConfigurationRoi(){foreach(var lease in configurationRoiLeases.Values.ToArray())lease.Refresh();if(configurationRoiViewport is {} viewport)viewport.Visibility=DisplayTabs.SelectedIndex==0?Visibility.Visible:Visibility.Collapsed;}
}
