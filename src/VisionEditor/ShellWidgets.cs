using System.Drawing.Drawing2D;
namespace VisionEditor;
static class VmUiAssets {
 static readonly Dictionary<string,Image> cache=new(StringComparer.Ordinal);
 public static Image? Get(string name){if(cache.TryGetValue(name,out var old))return old;string path=Path.Combine(AppContext.BaseDirectory,"ui-assets",name);if(!File.Exists(path))return null;using var input=new MemoryStream(File.ReadAllBytes(path));using var source=Image.FromStream(input);var bitmap=new Bitmap(source);cache[name]=bitmap;return bitmap;}
}
sealed class VmIconButton:Button {
 [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)] public Image? Glyph{get;set;}
 public VmIconButton(){FlatStyle=FlatStyle.Flat;FlatAppearance.BorderSize=0;Margin=Padding.Empty;Padding=Padding.Empty;TabStop=false;SetStyle(ControlStyles.OptimizedDoubleBuffer|ControlStyles.UserPaint,true);}
 protected override void OnPaint(PaintEventArgs e){e.Graphics.Clear(BackColor);if(ClientRectangle.Contains(PointToClient(Cursor.Position))&&Enabled){using var hover=new SolidBrush(Color.FromArgb(75,78,81));e.Graphics.FillRectangle(hover,ClientRectangle);}if(Glyph is not null){float size=26*DeviceDpi/96f;var rect=new RectangleF((Width-size)/2,(Height-size)/2,size,size);e.Graphics.InterpolationMode=InterpolationMode.HighQualityBicubic;if(Enabled)e.Graphics.DrawImage(Glyph,rect);else{using var a=new System.Drawing.Imaging.ImageAttributes();a.SetColorMatrix(new System.Drawing.Imaging.ColorMatrix{Matrix33=.45f});e.Graphics.DrawImage(Glyph,Rectangle.Round(rect),0,0,Glyph.Width,Glyph.Height,GraphicsUnit.Pixel,a);}}else TextRenderer.DrawText(e.Graphics,Text,Font,ClientRectangle,Enabled?ForeColor:Color.FromArgb(98,100,103),TextFormatFlags.HorizontalCenter|TextFormatFlags.VerticalCenter);}
}
sealed class VmToggle:Control {
 bool on;[System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)] public bool IsOn{get=>on;set{on=value;Invalidate();}}public event Action<bool>? Toggled;
 public VmToggle(){SetStyle(ControlStyles.UserPaint|ControlStyles.OptimizedDoubleBuffer|ControlStyles.AllPaintingInWmPaint,true);Cursor=Cursors.Hand;}
 protected override void OnMouseClick(MouseEventArgs e){base.OnMouseClick(e);IsOn=!IsOn;Toggled?.Invoke(IsOn);}
 protected override void OnPaint(PaintEventArgs e){float s=DeviceDpi/96f;int h=(int)(18*s),w=(int)(36*s),x=(Width-w)/2,y=(Height-h)/2;using var fill=new SolidBrush(on?Color.FromArgb(188,82,0):Color.FromArgb(90,91,93));e.Graphics.FillRectangle(fill,x,y,w,h);using var thumb=new SolidBrush(Color.FromArgb(183,184,185));e.Graphics.FillRectangle(thumb,on?x+w/2:x,y,w/2,h);using var border=new Pen(Color.FromArgb(81,82,84));e.Graphics.DrawRectangle(border,x,y,w,h);}
}
