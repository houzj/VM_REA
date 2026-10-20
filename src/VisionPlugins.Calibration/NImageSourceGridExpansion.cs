namespace VisionPlugins.Calibration;

/// <summary>8bdef0 directional untried-hole filling, then optional one-sided border extension.</summary>
public static class NImageSourceGridExpansion
{
 public static int Expand(IReadOnlyList<NImageSourceCandidate> candidates,int[] used,NImageSourceGridState state,int direction,out int[] targetCoordinates)
 {
  ArgumentNullException.ThrowIfNull(candidates);ArgumentNullException.ThrowIfNull(used);ArgumentNullException.ThrowIfNull(state);
  if(direction is <0 or >3)throw new ArgumentOutOfRangeException(nameof(direction));
  if(used.Length<90000)throw new ArgumentException("Native grid flags require 90000 entries",nameof(used));
  targetCoordinates=[];if(state.Width==0&&state.Height==0)return 0;
  var targets=new List<(int X,int Y)>();var triples=new List<NImageSourcePredictionTriple>();
  int Read(int position)
  {
   if(position>=0)return state.Indices[position];
   // Native matrix begins after Align128(90000*4), not directly after the flags.
   int flagIndex=90016+position;
   if((uint)flagIndex<90000)return used[flagIndex];
   if(flagIndex>=90000&&flagIndex<90016)return state.FlagAlignmentPadding[flagIndex-90000];
   throw new InvalidOperationException("Grid lookup precedes its source workspace");
  }
  void Add(int x,int y,int first,int second,int third)
  {
   if(second<0||third<0)return;
   triples.Add(new(first<0?-2:first,second,third));targets.Add((x,y));
  }
  void Gather(int depth)
  {
   void Cell(int x,int y)
   {
    int position=y*300+x;if(Read(position)!=-1)return;
    int step=direction switch{0=>300,1=>1,2=>-300,_=>-1};
    int first=depth==3?Read(position+step*3):-2,second=Read(position+step*2),third=Read(position+step);
    if(depth==3&&first<0)return;Add(x,y,first,second,third);
   }
   if(direction==0){for(int y=state.Height-1-depth;y>=0;y--)for(int x=0;x<state.Width;x++)Cell(x,y);}
   else if(direction==1){for(int y=0;y<state.Height;y++)for(int x=state.Width-1-depth;x>=0;x--)Cell(x,y);}
   else if(direction==2){for(int y=depth;y<state.Height;y++)for(int x=0;x<state.Width;x++)Cell(x,y);}
   else{for(int y=0;y<state.Height;y++)for(int x=depth;x<state.Width;x++)Cell(x,y);}
  }
  void Assign()
  {
   var assigned=new int[triples.Count];Array.Fill(assigned,-2);var predicted=new float[triples.Count*2];
   NImageSourceGridAssignment.Assign(candidates,triples.ToArray(),used,assigned,predicted);
   state.Count+=triples.Count;
   for(int i=0;i<assigned.Length;i++){if(assigned[i]==-2)state.Count--;state.Indices[targets[i].Y*300+targets[i].X]=assigned[i];}
  }
  if((state.Width-2)*(state.Height-2)!=state.Count)
  {
   Gather(3);if(targets.Count==0)Gather(2);
   if(targets.Count>0){Assign();targetCoordinates=targets.SelectMany(p=>new[]{p.X,p.Y}).ToArray();return 1;}
  }
  if(!NImageSourceGridPadding.AddBorder(state,direction))return 0;
  if(direction is 0 or 2)
  {
   int y=direction==0?0:state.Height-1,step=direction==0?300:-300;
   for(int x=0;x<state.Width;x++){int position=y*300+x;Add(x,y,Read(position+step*3),Read(position+step*2),Read(position+step));}
  }
  else
  {
   int x=direction==1?0:state.Width-1,step=direction==1?1:-1;
   for(int y=0;y<state.Height;y++){int position=y*300+x;Add(x,y,Read(position+step*3),Read(position+step*2),Read(position+step));}
  }
  Assign();targetCoordinates=targets.SelectMany(p=>new[]{p.X,p.Y}).ToArray();return 2;
 }
}
