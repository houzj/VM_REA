using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using VisionFramework;
using HorizontalAlignment=System.Windows.HorizontalAlignment;
namespace VisionPluginContracts;

public interface IToolConfigurationRoiRegistration:IDisposable {void Refresh();}

public static class ToolConfigurationRoiControls {
 public static IDisposable Register(ToolConfigurationControlRegistry registry,ToolEditorContext context,RoiEditorCapabilities capabilities,string inputPort,Func<string,string>? localize=null)=>Register(registry,context,capabilities,_=>inputPort,localize);
 public static IDisposable Register(ToolConfigurationControlRegistry registry,ToolEditorContext context,RoiEditorCapabilities capabilities,Func<ConfigurationNodeState,string> inputPort,Func<string,string>? localize=null)=>RegisterRefreshable(registry,context,capabilities,inputPort,localize);
 public static IToolConfigurationRoiRegistration RegisterRefreshable(ToolConfigurationControlRegistry registry,ToolEditorContext context,RoiEditorCapabilities capabilities,Func<ConfigurationNodeState,string> inputPort,Func<string,string>? localize=null){
  var leases=new List<IToolConfigurationRoiSession>();var text=localize??(key=>key);
  registry.Register("ROISelecter",c=>{
   if(context.ConfigurationRois is null)return new TextBlock{Text="未支持ROI：公共外部图像会话不可用"};
   bool box=c.Node.Get("ROISelection")=="Box"&&capabilities.Kinds.SequenceEqual(new[]{RoiKind.Rectangle});
   bool coordinate=c.Node.Get("ROISelection")=="Coordinate"&&capabilities.Kinds.SequenceEqual(new[]{RoiKind.Coordinate})&&!capabilities.FullImage&&!capabilities.Exclusions;
   if(c.Node.Name is null||c.Node.Get("SelectType")!="Single"||capabilities.Maximum!=1||!(box||coordinate))return new TextBlock{Text="未支持ROI：该源类型尚未接线"};
   var lease=context.ConfigurationRois.Open(new(context.Node.Id,c.Node.Name,inputPort(c.Node),capabilities,text(c.Node.CaptionKey)));leases.Add(lease);
   var panel=new StackPanel();panel.Children.Add(lease.Parts.Selector);
   var expand=new ToggleButton{Content=text("ROI参数"),Height=30,HorizontalAlignment=HorizontalAlignment.Right,Margin=new Thickness(0,0,40,0),Padding=new Thickness(6,0,0,0)};
   expand.Resources.MergedDictionaries.Add(new ResourceDictionary{Source=new Uri("/VisionPluginContracts;component/Resources/ConfigurationControls.xaml",UriKind.Relative)});expand.Style=(Style)expand.FindResource("ConfigurationExpandButton");panel.Children.Add(expand);
   lease.Parts.Fields.Visibility=Visibility.Collapsed;panel.Children.Add(lease.Parts.Fields);
   expand.Checked+=(_,_)=>lease.Parts.Fields.Visibility=Visibility.Visible;expand.Unchecked+=(_,_)=>lease.Parts.Fields.Visibility=Visibility.Collapsed;
   void Update(){expand.Visibility=lease.Snapshot.Image is not null&&lease.Editor.Regions.Count>0?Visibility.Visible:Visibility.Collapsed;if(lease.Diagnostic is {} error)c.State.ReportDiagnostic(new("RoiSession","ROISelecter",c.Node.Name,error));}
   lease.Changed+=Update;c.OnRefresh(lease.Refresh);panel.IsVisibleChanged+=(_,_)=>{if(!panel.IsVisible)lease.Editor.CancelGesture();};Update();return panel;
  });
  return new Release(()=>{foreach(var lease in leases)lease.Dispose();leases.Clear();},()=>{foreach(var lease in leases.ToArray())lease.Refresh();});
 }
 sealed class Release(Action release,Action refresh):IToolConfigurationRoiRegistration {bool disposed;public void Refresh(){if(!disposed)refresh();}public void Dispose(){if(disposed)return;disposed=true;release();}}
}

