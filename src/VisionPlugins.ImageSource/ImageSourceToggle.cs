using System.ComponentModel;
using System.Drawing.Drawing2D;
namespace VisionPlugins.ImageSource;
sealed class ImageSourceToggle:Control {
 bool isChecked;
 [DefaultValue(false)]public bool Checked{get=>isChecked;set{if(isChecked==value)return;isChecked=value;Invalidate();CheckedChanged?.Invoke(this,EventArgs.Empty);}}
 public event EventHandler? CheckedChanged;
 public ImageSourceToggle(){SetStyle(ControlStyles.UserPaint|ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer|ControlStyles.ResizeRedraw|ControlStyles.Selectable,true);Cursor=Cursors.Hand;TabStop=true;AccessibleRole=AccessibleRole.CheckButton;}
 protected override void OnClick(EventArgs e){if(Enabled){Focus();Checked=!Checked;}base.OnClick(e);}
 protected override void OnKeyDown(KeyEventArgs e){if(Enabled&&e.KeyCode==Keys.Space){Checked=!Checked;e.Handled=true;e.SuppressKeyPress=true;}base.OnKeyDown(e);}
 protected override void OnPaintBackground(PaintEventArgs e)=>e.Graphics.Clear(Parent?.BackColor??SystemColors.Control);
 protected override void OnPaint(PaintEventArgs e){e.Graphics.Clear(Parent?.BackColor??SystemColors.Control);float s=DeviceDpi/96f;var r=new RectangleF(3*s,(Height-18*s)/2,40*s,18*s);using var fill=new SolidBrush(Checked?Color.FromArgb(242,110,0):Color.FromArgb(176,179,181));e.Graphics.FillRectangle(fill,r);using var border=new Pen(Color.FromArgb(152,155,155),s);e.Graphics.DrawRectangle(border,r.X,r.Y,r.Width,r.Height);var thumb=new RectangleF(Checked?r.Right-18*s:r.Left,r.Top,18*s,r.Height);using var brush=new LinearGradientBrush(thumb,Color.White,Color.FromArgb(222,223,223),90);e.Graphics.FillRectangle(brush,thumb);e.Graphics.DrawRectangle(border,thumb.X,thumb.Y,thumb.Width,thumb.Height);if(Focused&&ShowFocusCues)ControlPaint.DrawFocusRectangle(e.Graphics,Rectangle.Inflate(Rectangle.Round(r),2,2));}
}
