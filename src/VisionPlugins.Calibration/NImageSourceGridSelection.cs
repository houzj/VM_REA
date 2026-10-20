namespace VisionPlugins.Calibration;

public readonly record struct NImageSourceGridPoint(int Column,int Row,float X,float Y);
public sealed record NImageSourceGridResult(IReadOnlyList<NImageSourceGridPoint> Points,int Width,int Height,float Score);

/// <summary>8bcff0 seed traversal, quality gates, expansion/pruning and later-equal-score replacement.</summary>
public static class NImageSourceGridSelection
{
 public static NImageSourceGridResult Find(IReadOnlyList<NImageSourceCandidate> candidates,int maximumWidth=300,int maximumHeight=300,
  NImageSourceGridState? workspace=null,int[]? usedFlags=null,CancellationToken cancellationToken=default)
 {
  ArgumentNullException.ThrowIfNull(candidates);if(candidates.Count>90000)throw new ArgumentException("Source candidate capacity is 90000",nameof(candidates));
  if(maximumWidth is <1 or >300||maximumHeight is <1 or >300)throw new ArgumentOutOfRangeException(nameof(maximumWidth));
  var state=workspace??new();var used=usedFlags??new int[90000];if(used.Length<90000)throw new ArgumentException("Grid flags require 90000 entries",nameof(usedFlags));
  state.Count=state.Width=state.Height=0;Array.Clear(used,0,candidates.Count);
  float bestScore=float.MaxValue;NImageSourceGridResult result=new(Array.Empty<NImageSourceGridPoint>(),0,0,0);
  void ClearSelected(){for(int y=0;y<state.Height;y++)for(int x=0;x<state.Width;x++){int index=state.Indices[y*300+x];if(index>=0)used[index]=0;}}
  for(int seed=0;seed<candidates.Count;)
  {
   cancellationToken.ThrowIfCancellationRequested();int next=seed+1;if(used[seed]==1){seed=next;continue;}
   bool accepted=NImageSourceGridSeeds.Create3x3(candidates,seed,used,state),fallback=false;float score=0;
   if(accepted){score=NImageSourceGridScoring.FindWorst(candidates,state).Value;if(score>-6f)accepted=false;}
   if(!accepted)
   {
    foreach(int position in new[]{0,1,2,300,301,302,600,601,602}){int index=state.Indices[position];if(index>=0)used[index]=0;}
    if(NImageSourceGridSeeds.Create2x2(candidates,seed,used,state))
    {
     score=FallbackScore(candidates,state);accepted=score<=-3f;fallback=accepted;
    }
    if(!accepted){ClearSelected();seed=next;continue;}
   }
   if(state.Count!=0)
   {
    int previous;
    do
    {
     cancellationToken.ThrowIfCancellationRequested();previous=state.Count;
     for(int direction=0;direction<4;direction++)
     {
      int status=NImageSourceGridExpansion.Expand(candidates,used,state,direction,out var targets);
      if(status!=0){if(status==1)direction--;int targetCount=targets.Length/2;NImageSourceGridPruning.Prune(candidates,used,state,targets,ref targetCount,ref score);}
     }
    }while(state.Height<=maximumHeight&&state.Width<=maximumWidth&&state.Count!=previous);
   }
   if(score<=bestScore)
   {
    var points=new List<NImageSourceGridPoint>();int minX=int.MaxValue,minY=int.MaxValue,maxX=0,maxY=0;
    for(int y=0;y<state.Height;y++)for(int x=0;x<state.Width;x++)
    {
     int index=state.Indices[y*300+x];if(index<0)continue;var point=candidates[index];
     minX=Math.Min(minX,x);minY=Math.Min(minY,y);maxX=Math.Max(maxX,x);maxY=Math.Max(maxY,y);points.Add(new(x,y,point.X,point.Y));
    }
    result=new(points.AsReadOnly(),unchecked(maxX-minX+1),unchecked(maxY-minY+1),score);bestScore=score;
    if(fallback){if(candidates.Count*.75d<state.Count)break;}else if(candidates.Count>1000)break;
   }
   Array.Clear(used,0,candidates.Count);
   if(state.Count>9){next+=(int)(candidates.Count*.01f)+5;if(!fallback)next=seed+6;}
   seed=next;
  }
  return result;
 }
 static float FallbackScore(IReadOnlyList<NImageSourceCandidate> candidates,NImageSourceGridState state)
 {
  var a=candidates[state.Indices[0]];var b=candidates[state.Indices[1]];var c=candidates[state.Indices[300]];var d=candidates[state.Indices[301]];
  float dx=b.X-a.X,dy=b.Y-a.Y,ex=c.X-a.X,ey=c.Y-a.Y,angle=MathF.Atan2(dy,dx);
  if(angle<-.7853981852531433f||angle>.7853981852531433f||dx*ey-dy*ex<0)return float.MaxValue;
  float bx=b.X-c.X,by=b.Y-c.Y,right=MathF.Sqrt(dx*dx+dy*dy),adx=a.X-d.X,ady=a.Y-d.Y;
  float deviationX=d.X-(dx+c.X),deviationY=d.Y-(dy+c.Y);
  float otherDiagonal=MathF.Sqrt(bx*bx+by*by),diagonal=MathF.Sqrt(adx*adx+ady*ady),deviation=MathF.Sqrt(deviationX*deviationX+deviationY*deviationY),vertical=MathF.Sqrt(ex*ex+ey*ey);
  if(diagonal==0||otherDiagonal==0||vertical==0)return float.MaxValue;
  float larger=right,smaller=vertical;if(right<=vertical){larger=vertical;smaller=right;}
  float penalty=1f-smaller/larger,denominator=otherDiagonal<diagonal?diagonal:otherDiagonal;
  return -(float)state.Count*(((1f-deviation/diagonal)-MathF.Abs(diagonal-otherDiagonal)/denominator)-penalty);
 }
}
