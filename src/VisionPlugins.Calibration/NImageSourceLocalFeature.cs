using VisionCore;
namespace VisionPlugins.Calibration;
// 70f4e0 current one-round 30-window branch; CPU11 generic and axis samplers.
internal static class NImageSourceLocalFeature {
 internal sealed record Result(NImageSourceCircleFit.Result? Fit,string? UnsupportedReason);
 internal static Result Find(GrayImage image,NImageSourceLocalPrelocator.Result pre){
  if(!pre.StatusOne)return new(null,null);
  const float pi=3.1415927410125732f,twoPi=6.2831854820251465f;
  float delta=twoPi/30f,theta=-(.5f*delta),omega=theta-pi,centerAngle=omega+pi;
  var points=new List<NImageSourceTagSampler.F>();
  for(int k=0;k<30;k++){
   float cx=MathF.Cos(centerAngle)*29.5f+pre.X,cy=MathF.Sin(centerAngle)*29.5f+pre.Y;
   var sample=Sample(image,cx,cy,omega);if(sample is null)return new(null,"Axis sampler requires a nonempty intersecting source rectangle");
   var profile=new float[42];for(int x=0;x<42;x++){int sum=0;for(int y=0;y<6;y++)sum+=sample[y*42+x];profile[x]=sum*(1f/6f);}
   var derivative=new float[42];for(int j=0;j<42;j++)derivative[j]=profile[Math.Max(j-1,0)]-profile[Math.Min(j+1,41)];
   var peaks=new List<(int Index,float Offset,float Score)>();const float eps=1.1920928955078125e-7f;
   for(int j=1;j<41;j++)if(8f-derivative[j]<eps&&eps<derivative[j]-derivative[j-1]&&derivative[j+1]-derivative[j]<eps){
    float den=(2f*derivative[j]-derivative[j+1])-derivative[j-1];
    float sub=MathF.Abs(den)>.001f?(.5f*(derivative[j+1]-derivative[j-1]))/den:0;
    float offset=j-21+sub,strength=derivative[j]+(.25f*(derivative[j+1]-derivative[j-1]))*sub;
    float score=MathF.Abs(strength)/(255f+eps);if(score>1)score=1;peaks.Add((j,offset,score));
   }
   peaks.Sort((a,b)=>MathF.Abs(a.Score-b.Score)>eps?(a.Score>b.Score?-1:1):a.Index.CompareTo(b.Index));
   if(peaks.Count>0&&peaks[0].Score>eps){float offset=peaks[0].Offset;float x=cx+MathF.Cos(omega)*offset,y=cy+MathF.Sin(omega)*offset;
    if(x>=0&&y>=0&&x<=image.Width-1f&&y<=image.Height-1f)points.Add(new(x,y));}
   centerAngle-=delta;omega-=delta;
  }
  return new(NImageSourceCircleFit.Fit(points),null);
 }
 // 6bc000 first corner, 6b6290 row valid segments, 6b5e30 11-bit integer weights.
 static byte[]? Sample(GrayImage image,float cx,float cy,float angle){
  double cos=Math.Cos((double)angle),sin=Math.Sin((double)angle);
  double rowX=cx-21d*cos+3d*sin,rowY=cy-21d*sin-3d*cos;
  bool centerInside=cx>=0&&cy>=0&&cx<image.Width&&cy<image.Height;
  float Residual(double v){float r=(float)(v-(int)v);if(r<-.0010000000474974513f)r+=1f;else if(r>.9990000128746033f)r-=1f;return r;}
  // 6b7ea0: single-precision angle bucket and first-corner phase gate.
  float degrees=57.295780181884766f*angle;degrees-=(int)(degrees/360f)*360f;if(degrees<0)degrees+=360f;int bucket=(int)degrees;float fraction=degrees-bucket;
  if(centerInside&&(bucket is 0 or 90 or 180 or 270)&&fraction> -1.1920928955078125e-7f&&fraction<1.1920928955078125e-7f&&MathF.Abs(Residual(rowX))<=.0010000000474974513f&&MathF.Abs(Residual(rowY))<=.0010000000474974513f){
   // 6ba450 signed half-away conversion of all four corner minima. Axis kernels
   // copy a half-open rectangle; reverse axes use size-1, not the max corner.
   double otherX=rowX+42d*cos,otherY=rowY+42d*sin;
   double minX=Math.Min(Math.Min(rowX,otherX),Math.Min(2d*cx-rowX,2d*cx-otherX));
   double minY=Math.Min(Math.Min(rowY,otherY),Math.Min(2d*cy-rowY,2d*cy-otherY));
   int left=(int)(minX>=0?minX+.5d:minX-.5d),top=(int)(minY>=0?minY+.5d:minY-.5d);
   int width=bucket is 90 or 270?6:42,height=bucket is 90 or 270?42:6;
   if(left>=image.Width||top>=image.Height||left+width<=0||top+height<=0)return null;
   var axis=new byte[42*6];
   // CPU11: 0 inline; 90 7165b0/71b390; 180 6b44b0/inline;
   // 270 718ca0/71d780. Boundary=2 replicates source edge bytes.
   for(int y=0;y<6;y++)for(int x=0;x<42;x++){
    (int dx,int dy)=bucket switch{0=>(x,y),90=>(5-y,x),180=>(41-x,5-y),_=>(y,41-x)};
    axis[y*42+x]=image[Math.Clamp(left+dx,0,image.Width-1),Math.Clamp(top+dy,0,image.Height-1)];
   }return axis;
  }
  var result=new byte[42*6];

  bool Clip(double x,double y,out int ix,out int iy){ix=(int)(x+.5d);iy=(int)(y+.5d);bool valid=true;
   if(x<0){ix=0;valid=false;}else if(x>image.Width-2d){ix=image.Width-1;valid=false;}
   if(y<0){iy=0;valid=false;}else if(y>image.Height-2d){iy=image.Height-1;valid=false;}return valid;
  }
  byte Interpolate(double x,double y){int ix=(int)x,iy=(int)y;
   if(centerInside){int a=(ix*2048+2048)-(int)(2048d*x),b=(int)(2048d*y)-iy*2048;
    int top=image[ix+1,iy]*2048+(image[ix,iy]-image[ix+1,iy])*a;
    int bottom=image[ix+1,iy+1]*2048+(image[ix,iy+1]-image[ix+1,iy+1])*a;
    return unchecked((byte)(unchecked((2048-b)*top+b*bottom+0x200000)>>22));
   }
   float wx=(float)((ix+1d)-x),wy=(float)(y-iy);
   float value=(image[ix,iy]*wx+image[ix+1,iy]*(1f-wx))*(1f-wy)+(image[ix,iy+1]*wx+image[ix+1,iy+1]*(1f-wx))*wy;
   return unchecked((byte)(int)(value+.5f));
  }
  double corner1X=rowX+42d*cos,corner1Y=rowY+42d*sin;
  bool Inside(double x,double y)=>x>0&&y>0&&x<image.Width-1d&&y<image.Height-1d;
  bool allInside=Inside(rowX,rowY)&&Inside(corner1X,corner1Y)&&Inside(2d*cx-rowX,2d*cy-rowY)&&Inside(2d*cx-corner1X,2d*cy-corner1Y);
  double endX=41d*cos+rowX,endY=41d*sin+rowY;
  for(int y=0;y<6;y++){
   int start=0,end=41;double x=rowX,z=rowY;
   if(allInside){for(int i=0;i<42;i++){result[y*42+i]=Interpolate(x,z);x+=cos;z+=sin;}rowX-=sin;rowY+=cos;endX-=sin;endY+=cos;continue;}
   for(;start<42;start++){if(Clip(x,z,out int ix,out int iy))break;result[y*42+start]=image[ix,iy];x+=cos;z+=sin;}
   double rightX=endX,rightY=endY;
   if(!centerInside){
    // 6c2d50 boundary2: left clip, right clip, forward half then reverse half.
    for(;start<end;end--){if(Clip(rightX,rightY,out int ix,out int iy))break;result[y*42+end]=image[ix,iy];rightX-=cos;rightY-=sin;}
    int middle=(start+end)/2;
    for(int i=start;i<=middle&&i<42;i++){result[y*42+i]=Interpolate(x,z);x+=cos;z+=sin;}
    for(int i=end;i>middle;i--){result[y*42+i]=Interpolate(rightX,rightY);rightX-=cos;rightY-=sin;}
    rowX-=sin;rowY+=cos;endX-=sin;endY+=cos;continue;
   }
   for(;end>=start;end--){if(Clip(rightX,rightY,out int ix,out int iy)){result[y*42+end]=Interpolate(rightX,rightY);if(start<end){rightX-=cos;rightY-=sin;result[y*42+end-1]=Interpolate(rightX,rightY);}break;}result[y*42+end]=image[ix,iy];rightX-=cos;rightY-=sin;}
   int exclusive=end==start?end:end-1;
   for(int i=start;i<exclusive;i++){result[y*42+i]=Interpolate(x,z);x+=cos;z+=sin;}
   rowX-=sin;rowY+=cos;endX-=sin;endY+=cos;
  }return result;
 }
}




