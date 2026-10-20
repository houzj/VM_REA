using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VisionFramework;
using VisionCore;
using W = System.Windows.Controls;
using Brush=System.Windows.Media.Brush;
namespace VisionPluginContracts;

/// <summary>Display.xml InputImage and Show Text consumers; all coordinates remain image pixels.</summary>
public sealed class ToolDisplayImagePresenter : W.UserControl
{
    readonly W.Grid scene = new(); readonly W.Image image = new() { Stretch = Stretch.Fill };
    readonly W.Canvas labels = new();
    public ToolDisplayFrame? Frame { get; private set; }
    public ToolDisplayImagePresenter()
    {
        scene.Children.Add(image); scene.Children.Add(labels); Content = new W.Viewbox { Stretch = Stretch.Uniform, Child = scene };
        SizeChanged+=(_,_)=>DrawGraphics();
    }
    public void Show(ToolDisplayFrame? frame)
    {
        Frame = frame; labels.Children.Clear(); image.Source = null;
        foreach(var old in scene.Children.OfType<W.Image>().Where(item=>!ReferenceEquals(item,image)).ToArray())scene.Children.Remove(old);
        if (frame is null) { image.Visibility=Visibility.Visible;scene.Width = scene.Height = 1; return; }
        static BitmapSource? Bitmap(IImageData? source)
        {
            if(source is null||ImagePreview.Packed(source) is not{} packed)return null;
            var bitmap=BitmapSource.Create(packed.Width,packed.Height,96,96,
                packed.Format==ImagePixelFormat.Rgb24?PixelFormats.Rgb24:PixelFormats.Gray8,null,packed.Pixels.ToArray(),packed.Stride);
            bitmap.Freeze();return bitmap;
        }
        var selectedLayer=frame.SelectedImageName is null?null:frame.ImageLayers.FirstOrDefault(layer=>layer.Name==frame.SelectedImageName);
        selectedLayer??=frame.ImageLayers.FirstOrDefault();
        var sceneImage=frame.ImageLayers.Count>0?selectedLayer?.Image:frame.Image;
        scene.Width = sceneImage?.Width ?? 640; scene.Height = sceneImage?.Height ?? 480;
        if(frame.ImageLayers.Count>0)
        {
            image.Visibility=Visibility.Collapsed;
            // Source result view selects one base image, then may add up to three
            // separately checked overlays. The overlay list is explicit session state.
            if(selectedLayer is not null)
            {
                var rendered=new W.Image{Tag="DisplayImage:"+selectedLayer.Name,Stretch=Stretch.Fill,Opacity=selectedLayer.Opacity,
                    Visibility=Visibility.Visible,Source=Bitmap(selectedLayer.Image)};
                scene.Children.Add(rendered);
            }
            bool overlaysVisible=selectedLayer is not null&&frame.BackgroundImageName==selectedLayer.Name;
            byte[] nativeAlpha=[191,127,63];int[] nativeZ=[100,101,102];int slot=0;
            foreach(var name in frame.CheckedImageNames.Distinct(StringComparer.Ordinal).Take(3))
            {
                var layer=frame.ImageLayers.FirstOrDefault(item=>item.Name==name&&item.Image is not null);
                if(layer is null)continue;
                int z=layer.OverlayZIndex==0?nativeZ[slot]:layer.OverlayZIndex;
                double alpha=(layer.OverlayAlpha??nativeAlpha[slot])/255d;
                var rendered=new W.Image{Tag="DisplayOverlay:"+layer.Name,Stretch=Stretch.Fill,Opacity=alpha,
                    Visibility=overlaysVisible?Visibility.Visible:Visibility.Collapsed,Source=Bitmap(layer.Image)};
                W.Panel.SetZIndex(rendered,z);scene.Children.Add(rendered);slot++;
            }
        }
        else
        {
            image.Visibility=Visibility.Visible;
            if (frame.ImageVisible && frame.Image is {} source) image.Source=Bitmap(source);
        }
        if (frame.TextVisible)
        {
            var text = new W.TextBlock { Name = "DisplayOverlayText", Text = frame.Text, FontSize = frame.FontSize, Opacity = frame.Opacity,
                Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFromInvariantString(frame.Color)! };
            W.Canvas.SetLeft(text, frame.PositionX); W.Canvas.SetTop(text, frame.PositionY); labels.Children.Add(text);
        }
        DrawGraphics();
    }
    void DrawGraphics(){
        foreach(var shape in labels.Children.OfType<System.Windows.Shapes.Path>().ToArray())labels.Children.Remove(shape);
        foreach(var text in labels.Children.OfType<W.TextBlock>().Where(t=>t.Tag is string).ToArray())labels.Children.Remove(text);
        if(Frame is null)return;
        double scale=Math.Min(ActualWidth/scene.Width,ActualHeight/scene.Height);if(!double.IsFinite(scale)||scale<=0)scale=1;
        foreach(var graphic in Frame.Graphics){
            if(graphic.Type=="text"){
                // FontSize zero is the source conversion fallback. Avoid an invalid
                // WPF FontSize assignment; original native zero-font rendering remains unverified.
                if(graphic.FontSize<=0)continue;
                foreach(var point in graphic.Points){var text=new W.TextBlock{Tag=graphic.Name,Text=point.Text??"",FontSize=graphic.FontSize,Opacity=graphic.Opacity,Foreground=(Brush)new BrushConverter().ConvertFromInvariantString(graphic.Color)!};W.Canvas.SetLeft(text,point.X);W.Canvas.SetTop(text,point.Y);labels.Children.Add(text);}
                continue;
            }
            void Add(ToolDisplayGraphic item,string? tip,bool markers=true){
                var path=new System.Windows.Shapes.Path{Tag=item.Name,Data=ToolDisplayGraphicGeometry.Create(item,scene.Width,scale,markers),Stroke=(Brush)new BrushConverter().ConvertFromInvariantString(item.Color)!,StrokeThickness=item.StrokeThickness/scale,Opacity=item.Opacity,IsHitTestVisible=!string.IsNullOrEmpty(tip),ToolTip=tip};
                if(item.Type=="coordinate")path.Fill=path.Stroke;
                labels.Children.Add(path);
            }
            if(graphic.Type is "point" or "path"){
                if(graphic.Type=="path")Add(graphic,null,false);
                foreach(var point in graphic.Points)Add(graphic with{Type="point",Color=point.Color??graphic.Color,Points=new[]{point}},point.Tips);
            }else Add(graphic,string.Join(Environment.NewLine,graphic.Points.Select(p=>p.Tips)));
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
