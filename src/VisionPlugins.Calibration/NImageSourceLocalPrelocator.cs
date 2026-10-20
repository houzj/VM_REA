using VisionCore;
namespace VisionPlugins.Calibration;
// Fixed 80ac20 quadrant prelocator: [10,50,8,1,10,1], CPU7/10/11 ->75fbc0.
// Ordinary zero-amplitude semantics are intentionally not substituted for this branch.
internal static class NImageSourceLocalPrelocator {
 internal sealed record Result(bool StatusOne,int X,int Y,int Maximum,int CandidateCount,int[] Scores);
 internal static Result Locate(GrayImage image){
  int width=image.Width,height=image.Height;var gx=new short[width*height];var gy=new short[width*height];var magnitude=new short[width*height];
  if(width<3||height<3)return new(false,0,0,0,0,new int[width*height]);
  int vectorLast=1+((width-2)/8)*8;int maximumVector=0,maximumTail=0;
  // 75f300 writes a zero outer ring, vector eight-lane nearest conversion and scalar remainder +.5.
  for(int y=1;y<height-1;y++)for(int x=1;x<width-1;x++){
   int dx=(image[x+1,y-1]-image[x-1,y-1])+2*(image[x+1,y]-image[x-1,y])+(image[x+1,y+1]-image[x-1,y+1]);
   int dy=(image[x-1,y+1]-image[x-1,y-1])+2*(image[x,y+1]-image[x,y-1])+(image[x+1,y+1]-image[x+1,y-1]);
   int i=y*width+x;gx[i]=(short)dx;gy[i]=(short)dy;float length=MathF.Sqrt(dx*dx+dy*dy);
   if(x<vectorLast){int converted=(int)MathF.Round(length,MidpointRounding.ToEven);magnitude[i]=(short)converted;maximumVector=Math.Max(maximumVector,converted);}
   else{magnitude[i]=(short)(int)(length+.5f);maximumTail=Math.Max(maximumTail,(int)length);}
  }
  int maximum=Math.Max(maximumVector,maximumTail);float threshold=maximum*(8f/255f);int cutoff=Math.Max(1,(int)MathF.Ceiling(threshold));
  var candidates=new List<(int X,int Y)>();for(int y=0;y<height;y++)for(int x=0;x<width;x++)if(magnitude[y*width+x]>=cutoff)candidates.Add((x,y)); //75f8c0
  int minimumRadius=Math.Min(10,Math.Max(width,height)),maximumRadius=Math.Min(50,Math.Max(width,height));
  int count=2*(maximumRadius-minimumRadius)+1;var weights=new int[count];float radius=minimumRadius;
  for(int i=0;i<count;i++){float value=(maximumRadius*.15915493667125702f)/radius;weights[i]=(int)(value>=0?value+.5f:value-.5f);radius+=.5f;} //728060
  var scores=new int[width*height];
  //7299e0 candidate batches do not change serial integer updates; preserve candidate and radius order.
  foreach(var c in candidates){int i=c.Y*width+c.X;float inverse=1f/magnitude[i];float ux=-gx[i]*inverse,uy=-gy[i]*inverse;
   float px=c.X+minimumRadius*ux,py=c.Y+minimumRadius*uy;
   for(int k=0;k<count;k++){
    int x=(int)(px>=0?px+.5f:px-.5f),y=(int)(py>=0?py+.5f:py-.5f);
    if(x>=0&&x<width&&y>=0&&y<height)scores[y*width+x]=unchecked(scores[y*width+x]+weights[k]);
    py+=uy*.5f;px+=ux*.5f;
   }
  }
  int best=0,bestX=0,bestY=0;for(int y=0;y<height;y++)for(int x=0;x<width;x++)if(scores[y*width+x]>best){best=scores[y*width+x];bestX=x;bestY=y;}
  float accept=(10f*.01f)*50f;bool success=accept<=best;return new(success,success?bestX:0,success?bestY:0,best,candidates.Count,scores);
 }
}
