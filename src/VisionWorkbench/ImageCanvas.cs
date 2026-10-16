using VisionCore;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
namespace VisionWorkbench;
sealed class ImageCanvas : Control {
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)] public bool CheckerboardWhenEmpty{get;set;}
    float viewZoom=1;bool actualSize;
    public void ZoomBy(float factor){viewZoom=Math.Clamp(viewZoom*factor,.1f,12f);Invalidate();}
    public void ResetView(){viewZoom=1;actualSize=false;Invalidate();}
    public void ShowActualSize(){viewZoom=1;actualSize=true;Invalidate();}
    Bitmap? bitmap; CircleDetectionResult? result; RadialCircleOptions? options;
    public Action<double,double>? CenterPicked=null;
    public ImageCanvas(){DoubleBuffered=true; BackColor=Color.FromArgb(24,28,34); Dock=DockStyle.Fill;}
    public void ClearImage(){bitmap?.Dispose();bitmap=null;result=null;options=null;Invalidate();}
    public void SetImage(GrayImage image){var next=new Bitmap(image.Width,image.Height,PixelFormat.Format24bppRgb);var data=next.LockBits(new Rectangle(0,0,image.Width,image.Height),ImageLockMode.WriteOnly,PixelFormat.Format24bppRgb);try{var row=new byte[data.Stride];for(int y=0;y<image.Height;y++){for(int x=0;x<image.Width;x++){byte v=image[x,y];row[x*3]=row[x*3+1]=row[x*3+2]=v;}Marshal.Copy(row,0,data.Scan0+y*data.Stride,row.Length);}}finally{next.UnlockBits(data);}bitmap?.Dispose();bitmap=next;result=null;Invalidate();}
    public void SetOverlay(RadialCircleOptions? seed,CircleDetectionResult? detected){options=seed;result=detected;Invalidate();}
    (float Scale,float X,float Y) Transform(){if(bitmap is null)return(1,0,0);float scale=Math.Min((float)Math.Max(1,Width-24)/bitmap.Width,(float)Math.Max(1,Height-24)/bitmap.Height);scale=(actualSize?1:scale)*viewZoom;return(scale,(Width-bitmap.Width*scale)/2,(Height-bitmap.Height*scale)/2);}
    protected override void OnPaint(PaintEventArgs e){base.OnPaint(e);if(bitmap is null&&CheckerboardWhenEmpty){int size=Math.Max(1,(int)(10*DeviceDpi/96f));using var a=new SolidBrush(Color.FromArgb(58,58,58));using var b=new SolidBrush(Color.FromArgb(44,44,44));for(int y=0;y<Height;y+=size)for(int x=0;x<Width;x+=size)e.Graphics.FillRectangle(((x/size+y/size)&1)==0?a:b,x,y,size,size);return;}if(bitmap is null){TextRenderer.DrawText(e.Graphics,"打开 BMP 图像，或点击「载入示例」",Font,ClientRectangle,Color.LightGray,TextFormatFlags.HorizontalCenter|TextFormatFlags.VerticalCenter);return;}var(s,ox,oy)=Transform();e.Graphics.DrawImage(bitmap,new RectangleF(ox,oy,bitmap.Width*s,bitmap.Height*s));e.Graphics.SmoothingMode=System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        if(options is not null){using var seed=new Pen(Color.FromArgb(230,184,78),1){DashStyle=System.Drawing.Drawing2D.DashStyle.Dash};foreach(double r in new[]{options.InitialRadius-options.SearchHalfWidth,options.InitialRadius+options.SearchHalfWidth})e.Graphics.DrawEllipse(seed,(float)(ox+(options.CenterX-r)*s),(float)(oy+(options.CenterY-r)*s),(float)(2*r*s),(float)(2*r*s));}
        if(result?.Circle is CircleFit c){using var pen=new Pen(Color.DeepPink,2);e.Graphics.DrawEllipse(pen,(float)(ox+(c.CenterX-c.Radius)*s),(float)(oy+(c.CenterY-c.Radius)*s),(float)(2*c.Radius*s),(float)(2*c.Radius*s));float x=(float)(ox+c.CenterX*s),y=(float)(oy+c.CenterY*s);e.Graphics.DrawLine(pen,x-6,y,x+6,y);e.Graphics.DrawLine(pen,x,y-6,x,y+6);}
        if(result is not null)foreach(var p in result.EdgePoints)e.Graphics.FillEllipse(Brushes.LimeGreen,(float)(ox+p.X*s)-2,(float)(oy+p.Y*s)-2,4,4);
    }
    protected override void OnMouseClick(MouseEventArgs e){base.OnMouseClick(e);if(bitmap is null)return;var(s,ox,oy)=Transform();double x=(e.X-ox)/s,y=(e.Y-oy)/s;if(x>=0&&y>=0&&x<bitmap.Width&&y<bitmap.Height)CenterPicked?.Invoke(x,y);}
    protected override void OnResize(EventArgs e){base.OnResize(e);Invalidate();}
    protected override void Dispose(bool disposing){if(disposing)bitmap?.Dispose();base.Dispose(disposing);}
}


