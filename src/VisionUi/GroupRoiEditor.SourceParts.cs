using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VisionCore;
using VisionFramework;
namespace VisionUi;

public sealed record RoiEditorSourceParts(FrameworkElement Selector,FrameworkElement Fields,FrameworkElement Viewport);
public sealed partial class GroupRoiEditor {
 public Action<DrawingContext,Matrix,double>? ImageOverlayRenderer{get;set;}
 public bool SourceImageVisible{get;set;}=true;
 public void InvalidateViewport()=>surface.InvalidateVisual();
 /// <summary>Reuses one geometry editor in an ordinary configuration window and its external image view.</summary>
 public RoiEditorSourceParts DetachSourceParts(string caption,bool single){
  var left=(StackPanel)((ScrollViewer)Children.OfType<FrameworkElement>().Single(e=>GetColumn(e)==0)).Content;
  var selector=left.Children.OfType<DockPanel>().First();left.Children.Remove(selector);
  var label=selector.Children.OfType<TextBlock>().First();label.Text=caption;label.Width=89;
  parameters.Content=null;
  ((Panel)surface.Parent).Children.Remove(surface);
  if(single&&surface.ContextMenu is {} menu)foreach(var item in menu.Items.OfType<MenuItem>().Where(i=>Equals(i.Header,"复制")).ToArray())menu.Items.Remove(item);
  return new(selector,values,surface);
 }
 public void RefreshSourceRegions(IReadOnlyList<RoiDocument> source){
  RoiGeometry.ValidateForEditor(source,capability);surface.CancelAll();regions.Clear();normalizedIds.Clear();
  foreach(var roi in source.Where(r=>r.Kind!=RoiKind.FullImage)){var next=roi with{Vertices=roi.Vertices?.ToArray()};if(next.CoordinateSpace==RoiCoordinateSpace.Normalized){normalizedIds.Add(next.Id);if(coordinateWidth>0)next=RoiGeometry.ToPixels(next,coordinateWidth,coordinateHeight);}regions.Add(next);}
  RefreshList(regions.Count>0?0:-1);surface.InvalidateVisual();
 }
 public void SetSourceImage(IImageData? image){
  SetImage(image is null?null:ImageColorConversion.ToMono8(image));
  if(image?.Format==ImagePixelFormat.Rgb24){surface.Image=BitmapSource.Create(image.Width,image.Height,96,96,PixelFormats.Rgb24,null,image.Pixels.ToArray(),image.Stride);surface.InvalidateVisual();}
 }
 public void ZoomSource(double factor)=>surface.SetScale(surface.Scale*factor);
 public void FitSource()=>surface.Fit();
}
