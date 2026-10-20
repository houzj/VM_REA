namespace VisionPlugins.CalibrationBoard;

public readonly record struct CalibrationBoardCircle(float X,float Y,float Radius);

/// <summary>Source 8a9f50/8aa010/722ac0 integer-contour fallback helper.
/// Keeps four axis supports, float arithmetic, native integer overflow and the
/// 100-iteration fallback. This is not a replacement with a generic minimum circle.</summary>
public static class CalibrationBoardEnclosingCircle
{
 static readonly int[][] Triangles=[[0,1,2,3],[0,1,3,2],[2,3,0,1],[2,3,1,0]];
 static readonly int[][] Replacements=[[0,1,2,3],[0,1,3,2],[0,2,3,1],[1,2,3,0]];
 static readonly float Minimum=BitConverter.Int32BitsToSingle(0x00800000);
 static float Distance((int X,int Y) point,CalibrationBoardCircle circle){float x=point.X-circle.X,y=point.Y-circle.Y;return x*x+y*y;}
 static int Square(int x)=>unchecked(x*x);
 public static CalibrationBoardCircle Find(IReadOnlyList<(int X,int Y)> points)
 {
  ArgumentNullException.ThrowIfNull(points);if(points.Count==0)throw new ArgumentException("轮廓不能为空。",nameof(points));
  if(points.Count==1)return new(points[0].X,points[0].Y,0);
  if(points.Count==2){float x=unchecked(points[0].X+points[1].X)*.5f,y=unchecked(points[0].Y+points[1].Y)*.5f;var result=new CalibrationBoardCircle(x,y,0);return result with{Radius=MathF.Sqrt(Distance(points[0],result))};}
  int ymin=0,xmax=0,ymax=0,xmin=0;
  for(int i=1;i<points.Count;i++){if(points[i].Y<points[ymin].Y)ymin=i;if(points[i].X>points[xmax].X)xmax=i;if(points[i].Y>points[ymax].Y)ymax=i;if(points[i].X<points[xmin].X)xmin=i;}
  var supports=new[]{points[ymin],points[xmax],points[ymax],points[xmin]};
  var current=Four(supports,out supports);var initial=current;
  for(int iteration=0;iteration<100;iteration++)
  {
   float maximum=Minimum;var furthest=(X:0,Y:0);
   foreach(var point in points){float d=Distance(point,current);if(d>maximum){maximum=d;furthest=point;}}
   if((double)current.Radius*current.Radius-maximum>1e-4)return current;
   foreach(var order in Replacements)
   {
    current=Four([supports[order[0]],supports[order[1]],supports[order[2]],furthest],out _);
    if(Distance(supports[order[3]],current)<=current.Radius*current.Radius){supports[order[3]]=furthest;break;}
   }
  }
  float largest=Minimum;foreach(var point in points)largest=Math.Max(largest,Distance(point,initial));
  return initial with{Radius=MathF.Sqrt(largest)};
 }
 static CalibrationBoardCircle Four((int X,int Y)[] original,out (int X,int Y)[] supports)
 {
  supports=original.ToArray();float farthest=Minimum;int first=0,second=0;
  for(int i=0;i<4;i++)for(int j=i+1;j<4;j++){int x=unchecked(supports[i].X-supports[j].X),y=unchecked(supports[i].Y-supports[j].Y);float d=unchecked(Square(x)+Square(y));if(d>farthest){first=i;second=j;farthest=d;}}
  (supports[0],supports[first])=(supports[first],supports[0]);(supports[1],supports[second])=(supports[second],supports[1]);
  var diameter=new CalibrationBoardCircle(unchecked(supports[0].X+supports[1].X)*.5f,unchecked(supports[0].Y+supports[1].Y)*.5f,0);
  diameter=diameter with{Radius=MathF.Sqrt(Distance(supports[0],diameter))};
  if(Distance(supports[2],diameter)<=diameter.Radius*diameter.Radius&&Distance(supports[3],diameter)<=diameter.Radius*diameter.Radius)return diameter;
  float best=float.MaxValue;int selected=-1;var result=diameter;
  for(int i=0;i<Triangles.Length;i++)
  {
   var order=Triangles[i];var a=supports[order[0]];var b=supports[order[1]];var c=supports[order[2]];
   float bx=unchecked(b.X-a.X),by=unchecked(b.Y-a.Y),cx=unchecked(c.X-a.X),cy=unchecked(c.Y-a.Y),determinant=bx*cy-cx*by;
   if(MathF.Abs(determinant)<1e-4f)continue;
   float rb=(((float)Square(b.X)-Square(a.X))+Square(b.Y)-Square(a.Y))*.5f;
   float rc=(((float)Square(c.X)-Square(a.X))+Square(c.Y)-Square(a.Y))*.5f;
   var circle=new CalibrationBoardCircle((cy*rb-by*rc)/determinant,(bx*rc-cx*rb)/determinant,0);
   circle=circle with{Radius=MathF.Sqrt(Distance(a,circle))};
   if((double)Distance(supports[order[3]],circle)<(double)circle.Radius*circle.Radius+1e-4&&circle.Radius<best){best=circle.Radius;selected=i;result=circle;}
  }
  if(selected>=0){var old=supports;supports=Triangles[selected].Select(i=>old[i]).ToArray();}
  return result;
 }
}
