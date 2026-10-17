using System.Windows;

namespace VisionPluginContracts;

public sealed record ToolConfigurationIdentity(string SolutionId,string ProcedureId,string NodeId);

/// <summary>
/// Ordinary module configuration lifecycle reconstructed from
/// VMControls.WPF.DataForUI.VmModuleDecorator.ConfigParamsExecute/OpenParamsTabControl.
/// This owns the window, not the configuration model or template selection.
/// </summary>
public sealed class ToolConfigurationWindowHost : IDisposable {
 readonly Window owner;
 public Window? Current {get;private set;}
 public ToolConfigurationIdentity? Identity {get;private set;}
 public event Action? Changed;
 public ToolConfigurationWindowHost(Window owner)=>this.owner=owner??throw new ArgumentNullException(nameof(owner));
 public Window Open(ToolConfigurationIdentity identity,Func<Window> create){
  owner.Dispatcher.VerifyAccess();ArgumentNullException.ThrowIfNull(identity);ArgumentNullException.ThrowIfNull(create);
  if(Current is {} existing&&Identity==identity)return existing;
  Close();
  if(Current is not null)throw new InvalidOperationException("当前配置窗口取消了关闭。");
  var window=create()??throw new InvalidOperationException("配置宿主未创建窗口。");
  if(ReferenceEquals(window,owner))throw new InvalidOperationException("配置窗口不能复用主窗口。");
  window.Owner=owner;window.ShowInTaskbar=false;Current=window;Identity=identity;
  window.Closed+=OnClosed;
  try{window.Show();Changed?.Invoke();return window;}
  catch{window.Closed-=OnClosed;Current=null;Identity=null;throw;}
 }
 void OnClosed(object? sender,EventArgs args){
  if(sender is Window window)window.Closed-=OnClosed;
  if(!ReferenceEquals(Current,sender))return;
  Current=null;Identity=null;Changed?.Invoke();
 }
 public void Close(){owner.Dispatcher.VerifyAccess();Current?.Close();}
 public void Dispose()=>Close();
}
