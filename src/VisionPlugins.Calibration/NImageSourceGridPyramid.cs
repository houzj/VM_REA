using VisionCore;
namespace VisionPlugins.Calibration;

/// <summary>Source-order 873de0 multiscale selection with caller-specific descriptor angles.</summary>
public static class NImageSourceGridPyramid
{
 public static NImageSourceGridResult Find(GrayImage source,int filterStatus=1,int maximumScaleExponent=5,int maximumWidth=300,int maximumHeight=300,CancellationToken cancellationToken=default)
  =>Find(source,30f,135f,filterStatus,maximumScaleExponent,maximumWidth,maximumHeight,cancellationToken);
 // 873de0 receives caller-specific float angles; its second hypothesis remains 65/110.
 public static NImageSourceGridResult Find(GrayImage source,float minimumAngle,float maximumAngle,int filterStatus=1,int maximumScaleExponent=5,int maximumWidth=300,int maximumHeight=300,CancellationToken cancellationToken=default)
 {
  ArgumentNullException.ThrowIfNull(source);if(maximumScaleExponent is <1 or >5)throw new ArgumentOutOfRangeException(nameof(maximumScaleExponent));
  if(source.Width<64||source.Height<64)return new(Array.Empty<NImageSourceGridPoint>(),0,0,0);
  var best=new NImageSourceGridResult(Array.Empty<NImageSourceGridPoint>(),0,0,0);float bestScore=float.MaxValue;int previousWidth=0,previousHeight=0;var firstOutputSlots=new float[90000*2];
  for(int scale=1;scale<(1<<maximumScaleExponent);scale*=2)
  {
   cancellationToken.ThrowIfCancellationRequested();int width=source.Width/scale,height=source.Height/scale;byte[] bytes=new byte[width*height];for(int y=0;y<height;y++)for(int x=0;x<width;x++)bytes[y*width+x]=source[x*scale,y*scale];var image=new GrayImage(width,height,bytes);
   var candidates=NImageSourceCandidateDetector.FindCandidates(image,minimumAngle,maximumAngle,filterStatus,cancellationToken);var first=NImageSourceGridSelection.Find(candidates,maximumWidth,maximumHeight,cancellationToken:cancellationToken);
   var secondCandidates=new List<NImageSourceCandidate>();foreach(var candidate in candidates)if(NImageSourceCandidateDescriptor.Describe(image,candidate,candidates.Count,1.75f,65,110) is {} described)secondCandidates.Add(described);
   var second=NImageSourceGridSelection.Find(secondCandidates,maximumWidth,maximumHeight,cancellationToken:cancellationToken);
   for(int i=0;i<first.Points.Count;i++){firstOutputSlots[i*2]=first.Points[i].X;firstOutputSlots[i*2+1]=first.Points[i].Y;}
   Point2? firstTrailing=first.Points.Count<90000?new Point2(firstOutputSlots[first.Points.Count*2],firstOutputSlots[first.Points.Count*2+1]):null;
   float firstScore=AdjustedScore(first,scale,firstTrailing),secondScore=AdjustedScore(second,scale);NImageSourceGridResult? selected=null;float selectedScore=0;
   if(bestScore<=firstScore||secondScore<firstScore){if(!(bestScore<=secondScore||firstScore<=secondScore)){selected=second;selectedScore=secondScore;}}
   else {selected=first;selectedScore=firstScore;}
   if(selected is not null){best=new(selected.Points.Select(p=>new NImageSourceGridPoint(p.Column,p.Row,p.X*scale,p.Y*scale)).ToArray(),selected.Width,selected.Height,selectedScore);bestScore=selectedScore;}
   if(best.Points.Count==best.Width*best.Height&&previousWidth==best.Width&&previousHeight==best.Height&&best.Points.Count>=10)break;
   if(width<=383||height<=383)break;previousWidth=best.Width;previousHeight=best.Height;
  }
  return best;
 }
 public static float AdjustedScore(NImageSourceGridResult result,int scale,Point2? firstHypothesisTrailingSlot=null)
 {
  int count=result.Points.Count;if(count==0)return 0;float minimum=float.MaxValue;
  if(count<10){for(int i=0;i<count;i++)for(int j=0;j<count;j++){if(i==j)continue;float dx=result.Points[i].X-result.Points[j].X,dy=result.Points[i].Y-result.Points[j].Y,value=dx*dx+dy*dy;if(!(minimum<=value))minimum=value;}}
  else {for(int i=0;i<count-1;i++){float dx=result.Points[i].X-result.Points[i+1].X,dy=result.Points[i].Y-result.Points[i+1].Y,value=dx*dx+dy*dy;if(!(minimum<=value))minimum=value;}}
  if(count>=10&&firstHypothesisTrailingSlot is {} tail){float dx=result.Points[^1].X-(float)tail.X,dy=result.Points[^1].Y-(float)tail.Y,value=dx*dx+dy*dy;if(!(minimum<=value))minimum=value;}
  // Buffer initialization is managed zero-fill; captured SDK next slots are zero in the
  // current four-image fixture. No undeclared memory is read by this API.
  float score=result.Score-MathF.Sqrt(minimum)*.08f*scale;if(count==result.Width*result.Height)score-=5f;return score;
 }
}
