using VisionCore;
namespace VisionPlugins.Calibration;
// Fixed single-item 86ff00 path, source core SHA85d17ecc...549.
// No internal orientation loop: 813ca0 owns proposal order and the direction.
internal static class NImageSourceTagSampler {
 internal readonly record struct F(float X,float Y);
 internal enum MeanDispatch { Ordinary, Cpu7Or10, Cpu11 }
 internal sealed record Sample(int Cell,float[] Matrix,byte[] Warped,byte[] Mean,byte[] Binary,byte[] Bits,byte[] Symbols,byte[] Payload,uint? Status,string? Unsupported=null);
 static readonly int[] Mask=[0,1,1,0,0,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,0,1,0,1,0,1];
 internal static Sample? Decode(GrayImage image,IReadOnlyList<F> q,MeanDispatch dispatch){
  if(q.Count!=4||q.Any(p=>!float.IsFinite(p.X)||!float.IsFinite(p.Y)))return null; // managed safety boundary
  // 86ff00 powf(...,2), sqrtf, float min/divides; the final side uses double /12.65.
  float Sq(float v)=>MathF.Pow(v,2f);
  float D(int a,int b)=>MathF.Sqrt(Sq(q[a].X-q[b].X)+Sq(q[a].Y-q[b].Y));
  float v=D(0,1)/12f;v=MathF.Min(v,float.MaxValue);v=MathF.Min(D(0,3)/12f,v);
  v=MathF.Min(D(2,3)*.125f,v);v=MathF.Min((float)((double)D(2,1)/12.65),v);
  int cell=v>=8f?8:(int)v;if(v<2f)return null;
  F[] reference=[new(2*cell,2*cell),new(14*cell,2*cell),new(10*cell,14*cell),new(2*cell,14*cell)];
  var h=NImageSourceQuadFit.Fit(reference,q);if(h is null)return null;
  return DecodeWithMatrix(image,cell,h,dispatch);
 }
 internal static Sample DecodeWithMatrix(GrayImage image,int cell,float[] h,MeanDispatch dispatch){
  if(cell is <2 or >8||h.Length!=9)throw new ArgumentException("Fixed 86ff00 cell/matrix domain");
  int size=16*cell;var warped=Warp(image,size,h);var mean=LocalMean(warped,size,4*cell,dispatch);
  var binary=new byte[warped.Length];for(int i=0;i<binary.Length;i++)binary[i]=warped[i]<mean[i]?(byte)0:(byte)255; // 8a8040 mode0,offset0,equality255
  var bits=Extract(binary,size,cell);var symbols=new byte[24];for(int i=0;i<192;i++)symbols[i/8]|=(byte)(bits[i]<<(7-i%8));
  var decoded=NImageSourceCodeword24.Correct(symbols);
  var payload=decoded.Success?decoded.Symbols.Reverse().Take(12).ToArray():[];
  return new(cell,h,warped,mean,binary,bits,symbols,payload,decoded.NativeStatus,decoded.UnsupportedReason);
 }
 // 800670 float operations, no epsilon in denominator.
 internal static F Project(float[] h,float x,float y){float den=h[6]*x+h[7]*y+h[8];return new((h[0]*x+h[1]*y+h[2])/den,(h[3]*x+h[4]*y+h[5])/den);}
 static int Cvtt(float x)=>!float.IsFinite(x)||x>=2147483648f||x< -2147483648f?int.MinValue:(int)x;
 internal static byte[] Warp(GrayImage source,int size,float[] h){
  var result=new byte[size*size];for(int y=0;y<size;y++)for(int x=0;x<size;x++){
   var p=Project(h,x,y);int x0=Cvtt((float)Math.Floor((double)p.X)),x1=Cvtt(MathF.Ceiling(p.X));
   int y0=Cvtt((float)Math.Floor((double)p.Y)),y1=Cvtt(MathF.Ceiling(p.Y));
   // 8b1710 checks all floor/ceil sample corners, writes zero if any is out.
   if(x0<0||x1<0||y0<0||y1<0||x0>=source.Width||x1>=source.Width||y0>=source.Height||y1>=source.Height)continue;
   // Preserve two individually divided X terms, followed by individually divided Y terms.
   // ceil==floor deliberately reaches 0/0. x64 CVTTSS2SI indefinite low byte is zero.
   float top=(source[x0,y0]*((float)x1-p.X))/(x1-x0)+(source[x1,y0]*(p.X-x0))/(x1-x0);
   float bottom=(source[x0,y1]*((float)x1-p.X))/(x1-x0)+(source[x1,y1]*(p.X-x0))/(x1-x0);
   float value=(top*((float)y1-p.Y))/(y1-y0)+(bottom*(p.Y-y0))/(y1-y0);
   result[y*size+x]=unchecked((byte)Cvtt(value));
  }return result;
 }
 // 8a4ae0 / 8e47b0 / 8e6720 use edge-including reflection. Fixed even kernel=4cell.
 // Vertical ushort sums and horizontal uint sums cannot overflow for this size/kernel domain.
 internal static byte[] LocalMean(byte[] input,int size,int kernel,MeanDispatch dispatch){
  int half=kernel/2;int Reflect(int i)=>i<0?-i-1:i>=size?2*size-i-1:i;
  var vertical=new ushort[input.Length];for(int y=0;y<size;y++)for(int x=0;x<size;x++){
   int sum=0;for(int k=-half;k<half;k++)sum+=input[Reflect(y+k)*size+x];vertical[y*size+x]=(ushort)sum;
  }
  var output=new byte[input.Length];float reciprocal=1f/(kernel*kernel);
  for(int y=0;y<size;y++)for(int x=0;x<size;x++){
   int sum=0;for(int k=-half;k<half;k++)sum+=vertical[y*size+Reflect(x+k)];float value=sum*reciprocal;int converted;
   if(dispatch==MeanDispatch.Ordinary){
    // 8a7b90 left boundary converts product to double before adding .5; interior/right use float.
    converted=x<=half?(int)((double)value+.5):(int)(value+.5f);
   }else if(x==0)converted=dispatch==MeanDispatch.Cpu11?(int)(value+.5f):(int)((double)value+.5);
   else converted=(int)MathF.Round(value,MidpointRounding.ToEven); // 8e4010/8e62d0 force nearest for vector lanes
   output[y*size+x]=unchecked((byte)converted);
  }return output;
 }
 internal static byte[] Extract(byte[] binary,int size,int cell){
  var bits=new byte[192];int index=0,inset=cell/3,width=cell-2*inset;
  // 86ff00 c800 table: block outer, first row x0..3 then second row x0..3.
  for(int block=0;block<32;block++)if(Mask[block]!=0)for(int dy=0;dy<2;dy++)for(int dx=0;dx<4;dx++){
   int cx=(block&3)*4+dx,cy=(block>>2)*2+dy;float sum=0;
   for(int y=0;y<width;y++)for(int x=0;x<width;x++)sum+=binary[(cy*cell+inset+y)*size+cx*cell+inset+x];
   int white=153f<sum/(width*width)?1:0;bits[index++]=(byte)(white^((cx+cy)&1)^1);
  }return bits;
 }
}
