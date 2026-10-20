using VisionPlugins.Calibration;
namespace VisionPlugins.CalibrationBoard;

public sealed record CalibrationBoardCircleGridPoint(int CandidateId,int IndexX,int IndexY,float X,float Y,float Size);
public sealed record CalibrationBoardCircleGridComponent(uint Error,IReadOnlyList<CalibrationBoardCircleGridPoint> Points);
public sealed record CalibrationBoardCircleGridWindow(IReadOnlyList<CalibrationBoardCircleGridPoint> Points,int Width,int Height);

/// <summary>Source 8d07c0 initial four-sector neighbor stage. Complete topology/grid assembly remains separate.</summary>
public static class CalibrationBoardCircleGrid
{
 public static CalibrationBoardCircleGridComponent Match(IReadOnlyList<CalibrationBoardCircleCandidate> points)
 {
  var best=new CalibrationBoardCircleGridComponent(0,[]);
  foreach(float orientation in new[]{0f,.785398185f})
  {
   var component=CanonicalComponent(points,orientation);
   if(component.Points.Count<=best.Points.Count)continue;
   var source=component.Points.ToArray();var order=Enumerable.Range(0,source.Length).ToArray();SortIndices(source.Select(p=>(float)p.IndexY).ToArray(),order);var sorted=order.Select(i=>source[i]).ToArray();
   int first=0;while(first<sorted.Length){int last=first+1;while(last<sorted.Length&&sorted[last].IndexY==sorted[first].IndexY)last++;if(last-first>1){var row=sorted[first..last];var rowOrder=Enumerable.Range(0,row.Length).ToArray();SortIndices(row.Select(p=>(float)p.IndexX).ToArray(),rowOrder);for(int i=0;i<row.Length;i++)sorted[first+i]=row[rowOrder[i]];}first=last;}
   best=component with{Points=sorted};
  }
  return best;
 }
 public static int[][] NeighborWords(IReadOnlyList<CalibrationBoardCircleCandidate> points,float orientation)
 {
  var result=points.Select(_=>new int[21]).ToArray();float cos=MathF.Cos(orientation),sin=MathF.Sin(orientation);
  static float F(int value)=>BitConverter.Int32BitsToSingle(value);static int I(float value)=>BitConverter.SingleToInt32Bits(value);
  foreach(var node in result){foreach(int offset in new[]{0,5,10,15}){node[offset]=-1;node[offset+4]=I(2147483648f);}node[20]=I(2147483648f);}
  for(int i=0;i<points.Count;i++)
  {
   var a=points[i];var distances=points.Select(p=>{float x=a.X-p.X,y=a.Y-p.Y;return MathF.Sqrt(x*x+y*y);}).ToArray();var order=Enumerable.Range(0,points.Count).ToArray();SortIndices(distances,order);
   var node=result[i];foreach(int j in order)
   {
    if(i==j)continue;var b=points[j];float ratio=Math.Min(a.Size,b.Size)/Math.Max(a.Size,b.Size);if(ratio<.8f||F(node[20])<distances[j])continue;
    float dx=a.X-b.X,dy=a.Y-b.Y,inverse=1f/MathF.Sqrt(dx*dx+dy*dy);dx*=inverse;dy*=inverse;
    float angle=MathF.Atan2(dy*cos-dx*sin,cos*dx+sin*dy);int slot=-1;
    const float quarter=.785398185f,threeQuarter=2.3561945f;
    if(-threeQuarter<angle&&angle<-quarter)slot=1;else if(-quarter<angle&&angle<quarter)slot=3;else if(threeQuarter<angle||angle<-threeQuarter)slot=2;else if(quarter<angle&&angle<threeQuarter)slot=0;
    if(slot>=0){int offset=slot*5;if(distances[j]<=F(node[offset+4])){node[offset]=j;node[offset+1]=I(ratio);node[offset+2]=I(dx);node[offset+3]=I(dy);node[offset+4]=I(distances[j]);float maximum=F(node[4]);foreach(int at in new[]{9,14,19})maximum=Math.Max(maximum,F(node[at]));node[20]=I(maximum);}}
    if(node[0]>=0&&node[5]>=0&&node[10]>=0&&node[15]>=0)break;
   }
  }
  return result;
 }
 public static int[][] PrunedNeighborWords(IReadOnlyList<CalibrationBoardCircleCandidate> points,float orientation)
 {
  var nodes=NeighborWords(points,orientation);Reciprocal(nodes);var snapshot=Clone(nodes);
  float F(int value)=>BitConverter.Int32BitsToSingle(value);
  bool Pair(int[] node,int first,int second){float a=F(node[first*5+4]),b=F(node[second*5+4]);return Math.Max(a,b)<=Math.Min(a,b)*1.3f&&-(F(node[first*5+2])*F(node[second*5+2])+F(node[first*5+3])*F(node[second*5+3]))>=-.7596879f;}
  void Drop(int row,int first,int second,float expected){var node=snapshot[row];float Score(int slot){float distance=F(node[slot*5+4]);return MathF.Sqrt(Math.Min(distance,expected)/Math.Max(distance,expected))*F(node[slot*5+1]);}nodes[row][(Score(second)<=Score(first)?second:first)*5]=-1;}
  for(int i=0;i<nodes.Length;i++)
  {
   var node=snapshot[i];bool vertical=node[0]>=0&&node[5]>=0,horizontal=node[10]>=0&&node[15]>=0;
   if(vertical&&!Pair(node,0,1)&&horizontal&&Pair(node,2,3))Drop(i,0,1,(F(node[14])+F(node[19]))*.5f);
   if(horizontal&&!Pair(node,2,3)&&vertical&&Pair(node,0,1))Drop(i,2,3,(F(node[4])+F(node[9]))*.5f);
  }
  Reciprocal(nodes);snapshot=Clone(nodes);
  for(int i=0;i<nodes.Length;i++)for(int direction=0;direction<4;direction++)ValidateLink(i,direction,snapshot,nodes);
  Reciprocal(nodes);return nodes;
 }
 static int Opposite(int slot)=>slot^1;
 public static CalibrationBoardCircleGridComponent CanonicalComponent(IReadOnlyList<CalibrationBoardCircleCandidate> points,float orientation)
 {
  var component=LargestComponent(points,orientation);if(component.Points.Count==0)return component;
  var window=SelectWindow(component.Points);component=component with{Points=window.Points};if(window.Points.Count==0)return component;
  int columns=window.Width,rows=window.Height;
  // 8cf2d0 calls 7ff3c0 directly: grid-to-image, WeightFun1, 50/1e-4, factor2.
  var fit=NPointSolver.FitNativePoints(component.Points.Select(p=>((float)p.IndexX,(float)p.IndexY,p.X,p.Y)).ToArray(),1,1,2f,50,1e-4f);
  if(fit.ErrorCode!=0||fit.Matrix is null)return component;var m=fit.Matrix;
  (float X,float Y) Project(int x,int y){float z=m[6]*x+m[7]*y+m[8];return ((m[0]*x+m[1]*y+m[2])/z,(m[3]*x+m[4]*y+m[5])/z);}
  var projected=new (float X,float Y)[columns*rows];for(int y=0;y<rows;y++)for(int x=0;x<columns;x++)projected[y*columns+x]=Project(x,y);
  float top=0,bottom=0,left=0,right=0;for(int x=0;x<columns;x++){top+=projected[x].Y;bottom+=projected[(rows-1)*columns+x].Y;}top/=columns;bottom/=columns;
  for(int y=0;y<rows;y++){left+=projected[y*columns].Y;right+=projected[y*columns+columns-1].Y;}left/=rows;right/=rows;
  float minimum=columns<rows?Math.Min(left,right):rows<columns?Math.Min(top,bottom):Math.Min(Math.Min(top,left),Math.Min(bottom,right));
  var center=projected[(rows/2)*columns+columns/2];var first=projected[0];var second=projected[columns-1];int newColumns=columns;
  Func<CalibrationBoardCircleGridPoint,CalibrationBoardCircleGridPoint> transform=p=>p;
  const float epsilon=1.1920929e-7f;
  if(Math.Abs(left-minimum)<epsilon){newColumns=rows;transform=p=>p with{IndexX=p.IndexY,IndexY=p.IndexX};second=projected[(rows-1)*columns];}
  else if(Math.Abs(bottom-minimum)<epsilon){transform=p=>p with{IndexY=rows-1-p.IndexY};first=projected[(rows-1)*columns];second=projected[rows*columns-1];}
  else if(Math.Abs(right-minimum)<epsilon){newColumns=rows;transform=p=>p with{IndexX=p.IndexY,IndexY=columns-1-p.IndexX};first=projected[columns-1];second=projected[rows*columns-1];}
  bool flip=(first.X-center.X)*(second.Y-center.Y)-(first.Y-center.Y)*(second.X-center.X)<0;
  return component with{Points=component.Points.Select(p=>{var transformed=transform(p);return flip?transformed with{IndexX=newColumns-1-transformed.IndexX}:transformed;}).ToArray()};
 }
 /// <summary>8cfb40/89cfa0/89ead0 maximum occupancy window; row-major first maximum wins.</summary>
 public static CalibrationBoardCircleGridWindow SelectWindow(IReadOnlyList<CalibrationBoardCircleGridPoint> points,int maximumWidth=300,int maximumHeight=300)
 {
  if(points.Count==0)return new([],0,0);
  if(maximumWidth<1||maximumHeight<1)throw new ArgumentOutOfRangeException(nameof(maximumWidth));
  int left=points.Min(p=>p.IndexX),top=points.Min(p=>p.IndexY);
  var normalized=points.Select(p=>p with{IndexX=p.IndexX-left,IndexY=p.IndexY-top}).ToArray();
  int width=normalized.Max(p=>p.IndexX)+1,height=normalized.Max(p=>p.IndexY)+1;
  if(width>600||height>600)return new([],0,0);
  if(Math.Min(width,height)<Math.Min(maximumWidth,maximumHeight)&&Math.Max(width,height)<Math.Max(maximumWidth,maximumHeight))return new(normalized,width,height);
  int stride=width+1;var occupied=new int[width*height];foreach(var p in normalized)occupied[p.IndexY*width+p.IndexX]=1;
  var integral=new int[(width+1)*(height+1)];for(int y=0;y<height;y++)for(int x=0;x<width;x++)integral[(y+1)*stride+x+1]=occupied[y*width+x]+integral[y*stride+x+1]+integral[(y+1)*stride+x]-integral[y*stride+x];
  (int Count,int X,int Y,int Width,int Height) Find(int w,int h)
  {
   w=Math.Min(w,width);h=Math.Min(h,height);int best=int.MinValue,bx=0,by=0;
   for(int y=0;y<=height-h;y++)for(int x=0;x<=width-w;x++){int count=integral[(y+h)*stride+x+w]-integral[y*stride+x+w]-integral[(y+h)*stride+x]+integral[y*stride+x];if(count>best){best=count;bx=x;by=y;}}
   return(best,bx,by,w,h);
  }
  var selected=Find(maximumWidth,maximumHeight);if(maximumWidth!=maximumHeight){var reversed=Find(maximumHeight,maximumWidth);if(reversed.Count>selected.Count)selected=reversed;}
  return new(normalized.Where(p=>p.IndexX>=selected.X&&p.IndexX<selected.X+selected.Width&&p.IndexY>=selected.Y&&p.IndexY<selected.Y+selected.Height).ToArray(),selected.Width,selected.Height);
 }
 public static CalibrationBoardCircleGridComponent LargestComponent(IReadOnlyList<CalibrationBoardCircleCandidate> points,float orientation)
 {
  var nodes=PrunedNeighborWords(points,orientation);var stack=new Stack<int>(Enumerable.Range(0,points.Count));var seen=new bool[points.Count];var groups=new int[points.Count];var x=new int[points.Count];var y=new int[points.Count];
  int group=0;uint error=0x10000106;var current=new List<int>();var best=new List<int>();
  void KeepBest(){if(error==0&&current.Count>best.Count)best=[..current];}
  while(stack.Count>0)
  {
   int i=stack.Pop();if(seen[i])continue;seen[i]=true;
   if(groups[i]==0){KeepBest();error=0;group++;current=[i];groups[i]=group;x[i]=y[i]=0;}
   for(int slot=0;slot<4;slot++)
   {
    int other=nodes[i][slot*5];if(other<0)continue;int nextX=x[i]+(slot==2?-1:slot==3?1:0),nextY=y[i]+(slot==0?-1:slot==1?1:0);
    if(groups[other]==0){groups[other]=group;x[other]=nextX;y[other]=nextY;stack.Push(other);current.Add(other);Align(nodes,i,other);}
    else if(x[other]!=nextX||y[other]!=nextY)error=0x10000106;
   }
  }
  KeepBest();if(best.Count==0)return new(0x10000106,[]);int minX=best.Min(i=>x[i]),minY=best.Min(i=>y[i]);
  return new(0,best.Select(i=>new CalibrationBoardCircleGridPoint(i+1,x[i]-minX,y[i]-minY,points[i].X,points[i].Y,points[i].Size)).ToArray());
 }
 static int[][] Clone(int[][] nodes)=>nodes.Select(n=>(int[])n.Clone()).ToArray();
 static void Reciprocal(int[][] nodes)
 {
  for(int i=0;i<nodes.Length;i++)for(int slot=0;slot<4;slot++){int other=nodes[i][slot*5];if(other>=0&&!Enumerable.Range(0,4).Any(s=>nodes[other][s*5]==i))nodes[i][slot*5]=-1;}
 }
 // 8ce8e0 aligns the second node's four stored neighbor blocks by quarter turns.
 static void Align(int[][] nodes,int a,int b)
 {
  int[] cycle=[0,2,1,3];
  for(int rotation=0;rotation<4;rotation++)
  {
   bool matches=false;for(int slot=0;slot<4;slot++)if(nodes[a][slot*5]==b)for(int other=0;other<4;other++)if(nodes[b][other*5]==a&&cycle[(Array.IndexOf(cycle,other)+rotation)%4]==Opposite(slot))matches=true;
   if(!matches)continue;if(rotation==0)return;var copy=(int[])nodes[b].Clone();for(int old=0;old<4;old++){int target=cycle[(Array.IndexOf(cycle,old)+rotation)%4];Array.Copy(copy,old*5,nodes[b],target*5,5);}return;
  }
 }
 // 8cd840 slot0, followed by 8ced00/8ceea0/8cf040, checks each local square.
 static void ValidateLink(int i,int direction,int[][] reference,int[][] output)
 {
  int j=reference[i][direction*5];if(j<0)return;Align(reference,i,j);Align(output,i,j);int reverse=Opposite(direction);
  if(reference[j][reverse*5]!=i){output[i][direction*5]=-1;return;}
  bool valid=true;foreach(int side in direction<2?new[]{2,3}:new[]{0,1})
  {
   int a=reference[i][side*5],b=reference[j][side*5];if(a<0||b<0){if(a!=-1||b!=-1){valid=false;break;}continue;}
   Align(reference,i,a);Align(reference,j,b);a=reference[i][side*5];b=reference[j][side*5];
   if(reference[a][direction*5]!=b||reference[b][reverse*5]!=a){valid=false;break;}
  }
  if(!valid){output[i][direction*5]=-1;output[j][reverse*5]=-1;}
 }
 // 8287b0 calls original MSVCR90 qsort, whose equal-key order is observable in
 // missing-dot ties. Independent index adaptation of recovered 7867df20.
 static void SortIndices(float[] keys,int[] order)
 {
  // 828590 deliberately returns -1 for equal float keys (never 0).
  int Compare(int a,int b)=>keys[order[a]]>keys[order[b]]?1:-1;
  void Swap(int a,int b){if(a!=b)(order[a],order[b])=(order[b],order[a]);}
  void Sort(int lo,int hi)
  {
   if(hi-lo+1<=8){for(int end=hi;end>lo;end--){int maximum=lo;for(int p=lo+1;p<=end;p++)if(Compare(p,maximum)>0)maximum=p;Swap(maximum,end);}return;}
   int mid=lo+(hi-lo+1)/2;if(Compare(lo,mid)>0)Swap(lo,mid);if(Compare(lo,hi)>0)Swap(lo,hi);if(Compare(mid,hi)>0)Swap(mid,hi);
   int left=lo,right=hi;
   while(true)
   {
    do{left++;}while(left<=hi&&(left==mid||Compare(left,mid)<=0));
    do{right--;}while(right>mid&&Compare(right,mid)>0);
    if(left>right)break;Swap(left,right);if(mid==right)mid=left;
   }
   right++;do{right--;}while(right>lo&&(right==mid||Compare(right,mid)==0));
   if(lo<right)Sort(lo,right);if(left<hi)Sort(left,hi);
  }
  if(order.Length>1)Sort(0,order.Length-1);
 }
}
