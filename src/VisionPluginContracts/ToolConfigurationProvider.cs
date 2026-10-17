using System.Windows;

namespace VisionPluginContracts;

/// <summary>Parallel configuration contract. The plugin supplies a model and leaf
/// extensions; the ordinary configuration window belongs to the shared host.</summary>
public interface IToolConfigurationProvider {
 ToolConfigurationSession CreateConfiguration(ToolEditorContext context);
}

public sealed class ToolConfigurationSession : IDisposable {
 readonly Action? release;
 bool disposed;
 public ToolConfigurationState State {get;}
 public ToolConfigurationControlRegistry Controls {get;}
 public string Language {get;}
 public Func<string,string> Localize {get;}
 public Action? OpenHelp {get;init;}
 public event Action? CloseRequested;
 public ToolConfigurationSession(ToolConfigurationState state,ToolConfigurationControlRegistry controls,string language="Chinese",Func<string,string>? localize=null,Action? release=null){
  State=state??throw new ArgumentNullException(nameof(state));Controls=controls??throw new ArgumentNullException(nameof(controls));Language=language;Localize=localize??(key=>key);this.release=release;
 }
 public void RequestClose(){ObjectDisposedException.ThrowIf(disposed,this);CloseRequested?.Invoke();}
 public void Dispose(){if(disposed)return;disposed=true;CloseRequested=null;release?.Invoke();}
}
