namespace VisionPlugins.CalibrationBoard;

public readonly record struct BoardCalibrationAxes(float OriginX,float OriginY,float XVectorX,float XVectorY,float YVectorX,float YVectorY);

public static class CalibrationBoardAxes
{
 /// <summary>7ed0c0 final 730e90 float inverse, projection at 0/0,1/0,0/1 and float unit vectors.</summary>
 public static BoardCalibrationAxes FromMatrix(IReadOnlyList<float> matrix)
 {
  if(matrix.Count!=9)throw new ArgumentException("3x3 matrix required.",nameof(matrix));
  var inverse=new float[3,3];for(int i=0;i<9;i++)inverse[i/3,i%3]=matrix[i];CalibrationBoardCoordinates.Invert(inverse);
  (float X,float Y) Project(float x,float y)
  {
   float z=inverse[2,0]*x+inverse[2,1]*y+inverse[2,2];float reciprocal=z>=1.1920928955078125e-7f?1f/z:0;
   return((inverse[0,0]*x+inverse[0,1]*y+inverse[0,2])*reciprocal,(inverse[1,0]*x+inverse[1,1]*y+inverse[1,2])*reciprocal);
  }
  var origin=Project(0,0);var x=Project(1,0);var y=Project(0,1);
  float xx=x.X-origin.X,xy=x.Y-origin.Y,yx=y.X-origin.X,yy=y.Y-origin.Y;
  float xi=1f/MathF.Sqrt(xy*xy+xx*xx),yi=1f/MathF.Sqrt(yy*yy+yx*yx);
  return new(origin.X,origin.Y,xx*xi,xy*xi,yx*yi,yy*yi);
 }
}
