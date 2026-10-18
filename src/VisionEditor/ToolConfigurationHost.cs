using System.Text.Json;
using System.Windows;
using VisionFramework;
using VisionPluginContracts;

namespace VisionEditor;
public partial class MainWindow {
 ToolConfigurationWindowHost? toolConfigurationWindows;
 string? configurationBaseDirectory;
 ToolConfigurationWindowHost ConfigurationWindows(){
  if(toolConfigurationWindows is not null)return toolConfigurationWindows;
  var host=new ToolConfigurationWindowHost(this);
  host.Changed+=()=>{parameterEditorOpen=host.Current is not null;if(host.Current is null)configurationBaseDirectory=null;if(!windowClosed)RefreshAll();};
  return toolConfigurationWindows=host;
 }
 void OpenToolConfiguration(string id){
  var procedure=doc.CurrentProcedure;
  var node=procedure.Nodes.Single(n=>n.Id==id);
  if(node.ModuleId==WorkflowCalls.ModuleId){OpenSubprogramCall(id);return;}
  if(windowClosed||isLocked||activeRun is not null)return;
  if(parameterEditorOpen&&toolConfigurationWindows?.Current is null)return;
  var plugin=plugins.Find(node.ModuleId);
  if(plugin is not (IToolConfigurationProvider or IWpfToolEditorFactory)){System.Windows.MessageBox.Show(this,"该节点的配置模型尚未接入。","模块参数");return;}
  string solution=doc.SolutionId;
  var identity=new ToolConfigurationIdentity(solution,procedure.Id,id);
  ConfigurationWindows().Open(identity,()=>{
   var ownerDirectory=doc.BaseDirectory;configurationBaseDirectory=ownerDirectory;bool active=true;
   void Apply(Dictionary<string,JsonElement> values){
    if(!active||windowClosed||isLocked||activeRun is not null||doc.SolutionId!=solution||!StringComparer.OrdinalIgnoreCase.Equals(ownerDirectory,doc.BaseDirectory))throw new InvalidOperationException("当前模块不可编辑。");
    if(!doc.Document.Procedures.Any(p=>p.Id==procedure.Id&&p.Nodes.Any(n=>n.Id==id)))throw new InvalidOperationException("模块已移除。");
    var before=doc.Document.Procedures.Single(p=>p.Id==procedure.Id).Nodes.Single(n=>n.Id==id).Parameters;
    doc.SetProcedureNodeParameters(procedure.Id,id,values);
    executionScope.ForProcedure(procedure.Id).RememberConfigurationParameters(node.ModuleId,id,before,values);
   }
   var context=new ToolEditorContext(node,ownerDirectory,Apply,_=>throw new NotSupportedException("配置执行服务尚未连接。"),procedure.Nodes.ToList().FindIndex(n=>n.Id==id)){ConfigurationActive=()=>active};
   context=ConnectToolEditorServices(context,id);
   ResetEditingGesture();
   // Transitional adapter: window factories will be replaced by the source-derived
   // configuration-model provider. Central lifecycle already matches ordinary modules.
   Window window=plugin is IToolConfigurationProvider provider?new ToolConfigurationModelWindow(context,provider.CreateConfiguration(context)):((IWpfToolEditorFactory)plugin).CreateWpfEditor(context);
   window.Closed+=(_,_)=>active=false;return window;
  });
 }
 void ValidateToolConfigurationOwner(){
  if(toolConfigurationWindows?.Identity is not {} identity)return;
  bool exists=doc.Document.Procedures.Any(p=>p.Id==identity.ProcedureId&&p.Nodes.Any(n=>n.Id==identity.NodeId));
  if(doc.SolutionId!=identity.SolutionId||!exists||!StringComparer.OrdinalIgnoreCase.Equals(configurationBaseDirectory,doc.BaseDirectory))CloseToolConfiguration();
 }
 void CloseToolConfiguration(){toolConfigurationWindows?.Close();if(toolConfigurationWindows?.Current is null)configurationBaseDirectory=null;}
}
