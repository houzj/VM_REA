using TabControl=System.Windows.Controls.TabControl;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using VisionFramework;
using VisionPluginContracts;
using VisionUi;
using Button=System.Windows.Controls.Button;
using ComboBox=System.Windows.Controls.ComboBox;
using Orientation=System.Windows.Controls.Orientation;
namespace VisionPlugins.ImageBinary;
public sealed class ImageBinaryParameterWindow:SourceConfirmWindow {
 public GroupRoiEditor Roi{get;}
 public ImageBinaryParameterWindow(ToolParameterEditorContext c){if(c.Parameter.Editor?.Kind!=ParameterEditorKind.Roi)throw new NotSupportedException("此专用参数窗口仅负责ROI。");Title="配置";Roi=new(c.Node.Rois??[],ImageBinaryPlugin.RoiCapabilities);Roi.SetImage(c.Image);string? lastSaved=null;void Save(){var regions=Roi.Regions;string serialized=JsonSerializer.Serialize(regions);if(serialized==lastSaved)return;c.Apply(new Dictionary<string,JsonElement>(),regions);lastSaved=serialized;}Roi.GeometryCommitted+=(_,_)=>{try{Save();Error.Text="";}catch(Exception e){Error.Text=e.Message;}};SetMainContent(Roi,()=>{Roi.CommitPending();Save();});}
}
