namespace VisionPlugins.Calibration;

/// <summary>8c0880 source-ordered removal of newly proposed cells until the prior score is restored.</summary>
public static class NImageSourceGridPruning
{
 public static void Prune(IReadOnlyList<NImageSourceCandidate> candidates,int[] used,NImageSourceGridState state,int[] targets,ref int targetCount,ref float threshold)
 {
  ArgumentNullException.ThrowIfNull(candidates);ArgumentNullException.ThrowIfNull(used);ArgumentNullException.ThrowIfNull(state);ArgumentNullException.ThrowIfNull(targets);
  if(targetCount<0||targetCount*2>targets.Length)throw new ArgumentOutOfRangeException(nameof(targetCount));
  float Minimum(int x,int y)
  {
   int position=(y*300+x)*3;float first=state.Scores[position],second=state.Scores[position+1],value=state.Scores[position+2];
   if(first<=second)second=first;if(second<=value)value=second;
   void Include(int offset){float other=state.Scores[position+offset];if(value<=other)other=value;value=other;}
   if(x>0)Include(-3);
   if(x>0&&y>0)Include(-902);
   if(y>0)Include(-898);
   if(x>1)Include(-6);
   if(x>1&&y>1)Include(-1805);
   if(y>1)Include(-1798);
   return value;
  }
  while(targetCount>0)
  {
   var worst=NImageSourceGridScoring.FindWorst(candidates,state);
   if(worst.Value<=threshold){threshold=worst.Value;return;}
   int[] xs=[worst.Column,worst.Column+(worst.Axis==0?1:0),worst.Column+(worst.Axis==0?2:0)];
   int[] ys=[worst.Row,worst.Row+(worst.Axis==2?1:0),worst.Row+(worst.Axis==2?2:0)];
   float[] minima=[Minimum(xs[0],ys[0]),Minimum(xs[1],ys[1]),Minimum(xs[2],ys[2])];
   float best=-float.MaxValue;int selected=0,column=worst.Column,row=worst.Row;
   for(int i=0;i<targetCount;i++)for(int point=0;point<3;point++)
    if(targets[i*2]==xs[point]&&targets[i*2+1]==ys[point]&&best<minima[point]){selected=i;best=minima[point];column=targets[i*2];row=targets[i*2+1];}
   for(int i=selected;i<targetCount-1;i++){targets[i*2]=targets[i*2+2];targets[i*2+1]=targets[i*2+3];}
   targets[(targetCount-1)*2]=targets[(targetCount-1)*2+1]=0;
   int cell=row*300+column,index=state.Indices[cell];if(index>=0){state.Count--;used[index]=0;}
   // Native also clears used[-1/-2] here. Keep that external workspace write outside managed arrays.
   state.Indices[cell]=-2;state.Scores[cell*3+worst.Axis]=float.MaxValue;targetCount--;
  }
 }
}
