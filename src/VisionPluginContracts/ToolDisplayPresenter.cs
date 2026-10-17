using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VisionFramework;
using VisionCore;
using W = System.Windows.Controls;
namespace VisionPluginContracts;

/// <summary>Display.xml InputImage and Show Text consumers; all coordinates remain image pixels.</summary>
public sealed class ToolDisplayImagePresenter : W.UserControl
{
    readonly W.Grid scene = new(); readonly W.Image image = new() { Stretch = Stretch.Fill };
    readonly W.Canvas labels = new() { IsHitTestVisible = false };
    public ToolDisplayFrame? Frame { get; private set; }
    public ToolDisplayImagePresenter()
    {
        scene.Children.Add(image); scene.Children.Add(labels); Content = new W.Viewbox { Stretch = Stretch.Uniform, Child = scene };
    }
    public void Show(ToolDisplayFrame? frame)
    {
        Frame = frame; labels.Children.Clear(); image.Source = null; if (frame is null) { scene.Width = scene.Height = 1; return; }
        scene.Width = frame.Image?.Width ?? 640; scene.Height = frame.Image?.Height ?? 480;
        if (frame.ImageVisible && frame.Image is {} source && ImagePreview.Packed(source) is {} packed)
        {
            var bitmap = BitmapSource.Create(packed.Width, packed.Height, 96, 96,
                packed.Format == VisionCore.ImagePixelFormat.Rgb24 ? PixelFormats.Rgb24 : PixelFormats.Gray8, null, packed.Pixels.ToArray(), packed.Stride);
            bitmap.Freeze(); image.Source = bitmap;
        }
        if (frame.TextVisible)
        {
            var text = new W.TextBlock { Name = "DisplayOverlayText", Text = frame.Text, FontSize = frame.FontSize, Opacity = frame.Opacity,
                Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFromInvariantString(frame.Color)! };
            W.Canvas.SetLeft(text, frame.PositionX); W.Canvas.SetTop(text, frame.PositionY); labels.Children.Add(text);
        }
    }
}
/// <summary>The native Data Record object supplies one formatted record per execution.</summary>
public sealed class ToolDisplayRecordPresenter : W.TextBox
{
    public ToolDisplayRecordPresenter() { IsReadOnly = true; AcceptsReturn = true; TextWrapping = TextWrapping.Wrap; }
    public void Show(ToolDisplayFrame? frame) { Text = frame?.RecordVisible == true ? frame.RecordText : ""; }
}
/// <summary>Native Result List is a single row with declared output columns.</summary>
public sealed class ToolDisplayResultListPresenter : W.DataGrid
{
    public ToolDisplayResultListPresenter() { AutoGenerateColumns = false; IsReadOnly = true; CanUserAddRows = false; CanUserDeleteRows = false; }
    public void Show(ToolDisplayFrame? frame)
    {
        Columns.Clear(); ItemsSource = null; if (frame?.ResultListVisible != true) return;
        foreach (var cell in frame.ResultList) Columns.Add(new W.DataGridTextColumn { Header = cell.Column, Binding = new System.Windows.Data.Binding("[" + cell.Column + "]") });
        ItemsSource = new[] { frame.ResultList.ToDictionary(c => c.Column, c => c.Value) };
    }
}
