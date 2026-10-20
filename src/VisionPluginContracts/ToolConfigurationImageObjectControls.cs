using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Brushes=System.Windows.Media.Brushes;
namespace VisionPluginContracts;

public static class ToolConfigurationImageObjectControls {
 public static void Register(ToolConfigurationControlRegistry registry,ToolDisplayObjectStore objects,Func<string,string>? localize=null){
  var text=localize??(key=>key);
  registry.Register("ImageViewConfig",context=>{
   var bridge=context.Element.Children.FirstOrDefault(e=>e.Tag=="BrigdgeSetter");
   string? Field(string key)=>bridge?.Children.FirstOrDefault(e=>e.Tag==key)?.Text;
   string? name=Field("OperationName")=="GetDisplayObjectOperation"?Field("OperationParams"):null;
   var state=name is null?null:objects.Read(name);
   if(state is null)return new TextBlock{Text="未支持显示对象："+name,Foreground=Brushes.DarkRed};
   var editor=new VisionUi.RoiDisplayStyleEditor(text(context.Node.CaptionKey),state.Style,state.Definition.StrokeThicknessConfigEnabled);
   editor.Changed+=style=>{var result=objects.Write(name!,style);if(result.State is {} accepted)editor.RefreshValue(accepted.Style);if(!result.Accepted)context.State.ReportDiagnostic(new("DisplayObjectWrite","ImageViewConfig",context.Node.Name,result.Error??"Display object write failed."));};
   context.OnRefresh(()=>{var current=objects.Read(name!);editor.IsEnabled=context.Node.Enabled&&current is not null;if(current is not null)editor.RefreshValue(current.Style);editor.Settings.Visibility=bool.TryParse(context.Node.Get("PanVisible"),out var visible)&&!visible?Visibility.Collapsed:Visibility.Visible;});
   return editor;
  });
 }
}
