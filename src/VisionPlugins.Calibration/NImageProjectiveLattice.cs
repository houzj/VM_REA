using VisionCore;
namespace VisionPlugins.Calibration;

/// <summary>Independent local-neighbor grid growth. A decoded code is still required to identify the board.</summary>
public static class NImageProjectiveLattice
{
 public static NImageLattice? Find(GrayImage image,int codeCellSize,CancellationToken cancellationToken=default,int filterStatus=1){
  ArgumentNullException.ThrowIfNull(image);if(codeCellSize is not (1 or 2))throw new ArgumentOutOfRangeException(nameof(codeCellSize));if(image.Width<64||image.Height<64)return null;
  double pitchX=NImageBoardDetector.EstimatePitch(image,true),pitchY=NImageBoardDetector.EstimatePitch(image,false);if(pitchX<8||pitchY<8)return null;
  var raw=NImageBoardDetector.DetectCandidates(image,pitchX,pitchY,cancellationToken,filterStatus==1);if(raw.Count<4)return null;double RefinedPitch(bool horizontal){var distances=new List<double>();for(int i=0;i<raw.Count;i++){double best=double.PositiveInfinity;for(int j=0;j<raw.Count;j++){double dx=Math.Abs(raw[j].X-raw[i].X),dy=Math.Abs(raw[j].Y-raw[i].Y),along=horizontal?dx:dy,cross=horizontal?dy:dx;if(along>8&&cross<along*.35)best=Math.Min(best,along);}if(double.IsFinite(best))distances.Add(best);}return distances.Count>0?distances.Order().ElementAt(distances.Count/2):0;}pitchX=RefinedPitch(true);pitchY=RefinedPitch(false);if(pitchX<8||pitchY<8)return null;raw=NImageBoardDetector.DetectCandidates(image,pitchX,pitchY,cancellationToken,filterStatus==1);var edges=new int[raw.Count,4];
  for(int i=0;i<raw.Count;i++){cancellationToken.ThrowIfCancellationRequested();for(int direction=0;direction<4;direction++){double best=double.PositiveInfinity;int selected=-1;for(int j=0;j<raw.Count;j++){if(i==j)continue;double dx=raw[j].X-raw[i].X,dy=raw[j].Y-raw[i].Y;double along=direction switch{0=>dx,1=>dy,2=>-dx,_=>-dy},cross=direction%2==0?dy:dx,pitch=direction%2==0?pitchX:pitchY;if(along<pitch*.55||along>pitch*1.55||Math.Abs(cross)>along*.45)continue;double score=Math.Abs(along-pitch)+Math.Abs(cross)*.5;if(score<best){best=score;selected=j;}}edges[i,direction]=selected;}}
  var visited=new bool[raw.Count];var bestComponent=new Dictionary<(int,int),int>();
  for(int seed=0;seed<raw.Count;seed++){if(visited[seed])continue;var assigned=new Dictionary<int,(int X,int Y)>{{seed,(0,0)}};var grid=new Dictionary<(int,int),int>{{(0,0),seed}};var queue=new Queue<int>();queue.Enqueue(seed);visited[seed]=true;
   while(queue.TryDequeue(out int i)){var current=assigned[i];for(int direction=0;direction<4;direction++){int j=edges[i,direction];if(j<0||edges[j,(direction+2)%4]!=i||assigned.ContainsKey(j))continue;var next=(current.X+(direction==0?1:direction==2?-1:0),current.Y+(direction==1?1:direction==3?-1:0));if(grid.ContainsKey(next))continue;assigned[j]=next;grid[next]=j;visited[j]=true;queue.Enqueue(j);}}
   if(grid.Count>bestComponent.Count)bestComponent=grid;
  }
  if(bestComponent.Count<16)return null;int minX=bestComponent.Keys.Min(p=>p.Item1),minY=bestComponent.Keys.Min(p=>p.Item2);var points=bestComponent.Select(p=>new NImageLatticePoint(p.Key.Item1-minX,p.Key.Item2-minY,raw[p.Value].X,raw[p.Value].Y,raw[p.Value].Response)).ToArray();
  var fit=NPointSolver.Solve(points.Select(p=>new NPointPair(p.Column,p.Row,p.X,p.Y)).ToArray(),[],new(1,1));if(fit.ErrorCode!=0)return null;
  var occupied=points.ToDictionary(p=>(p.Column,p.Row));int columns=points.Max(p=>p.Column)+1,rows=points.Max(p=>p.Row)+1;var quads=new List<IReadOnlyList<Point2>>();
  Point2 Position(int x,int y){if(occupied.TryGetValue((x,y),out var point))return new(point.X,point.Y);var predicted=NPointSolver.Transform(fit.Matrix,x,y);return new(predicted.X,predicted.Y);}
  for(int y=0;y+codeCellSize<rows;y++)for(int x=0;x+codeCellSize<columns;x++){if(codeCellSize==2&&occupied.TryGetValue((x+1,y+1),out var center)&&center.Contrast>180)continue;var corners=new[]{(x,y),(x+codeCellSize,y),(x+codeCellSize,y+codeCellSize),(x,y+codeCellSize)};if(corners.Count(occupied.ContainsKey)<2)continue;quads.Add(Array.AsReadOnly(corners.Select(p=>Position(p.Item1,p.Item2)).ToArray()));}
  return new(pitchX,pitchY,Array.AsReadOnly(points),quads.AsReadOnly());
 }
}

