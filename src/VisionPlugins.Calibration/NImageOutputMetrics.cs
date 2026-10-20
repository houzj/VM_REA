namespace VisionPlugins.Calibration;

public sealed record NImageOutputMetricsResult(IReadOnlyDictionary<string,double> Numbers,IReadOnlyDictionary<string,int> Integers,uint Status);

/// <summary>Independent scalar output projection from reconstructed numerical results.
/// Source: 7d12e0 mechanism errors and 7cf3f0 world errors/directions/status thresholds.</summary>
public static class NImageOutputMetrics{
 const float Epsilon=1.1920928955078125e-7f,Degrees=57.295780181884766f;
 public static NImageOutputMetricsResult Measure(NImageBatchSolveResult solved,IReadOnlyList<NImagePose> translationPoses,IReadOnlyList<NImagePose> rotationPoses){
  if(!solved.Complete||solved.ErrorCode!=0||solved.Matrix.Count!=9)throw new ArgumentException("Completed numerical stages required.",nameof(solved));
  var h=solved.Matrix.Select(v=>(float)v).ToArray();var t=solved.Translation;var r=solved.Rotation;
  var numbers=new Dictionary<string,double>();var integers=new Dictionary<string,int>();
  float scale=(MathF.Sqrt(h[0]*h[0]+h[1]*h[1])+MathF.Sqrt(h[3]*h[3]+h[4]*h[4]))*(1/(Epsilon+h[8]))*.5f;
  void Error(string average,string maximum,string worldAverage,string worldMaximum,string index,double mean,double max,int frame){numbers[average]=(float)mean;numbers[maximum]=(float)max;numbers[worldAverage]=(float)mean*scale;numbers[worldMaximum]=(float)max*scale;integers[index]=frame;}
  Error("TransError","TransEstMax","TransWorldError","TransErrWorldMax","TransErrMaxPtsNum",t?.AverageRegistrationError??0,t?.MaximumRegistrationError??0,t?.MaximumRegistrationFrame??0);
  Error("RotError","RotErrMax","RotWorldError","RotErrWorldMax","RotErrMaxPtsNum",r?.AverageRadialError??0,r?.MaximumRadialError??0,r?.MaximumErrorFrame??0);
  float distanceTotal=0,scaleTotal=0,rotateTotal=0,distanceMax=0,scaleAtMax=0,rotateAtMax=0,relativeTransMax=0;int transIndex=0;
  if(t is not null){if(translationPoses.Count!=t.ImageMotion.Count)throw new ArgumentException("Translation pose count must match solved frames.",nameof(translationPoses));for(int i=0;i<t.RelativeMotion.Count;i++){var motion=t.RelativeMotion[i];float dx=(float)translationPoses[i+1].X-(float)translationPoses[i].X,dy=(float)translationPoses[i+1].Y-(float)translationPoses[i].Y;float expected=MathF.Sqrt(dx*dx+dy*dy),actual=MathF.Sqrt(motion.X*motion.X+motion.Y*motion.Y)*(float)t.BoardScale,error=Math.Abs(actual-expected);distanceTotal+=error;scaleTotal+=motion.Scale;rotateTotal+=motion.Rotation;if(distanceMax<error){distanceMax=error;scaleAtMax=motion.Scale;rotateAtMax=motion.Rotation;transIndex=i;}relativeTransMax=Math.Max(relativeTransMax,error/(expected+Epsilon));}}
  int transPairs=t?.RelativeMotion.Count??0;numbers["TransMotionErrAvgTrans"]=transPairs>0?distanceTotal/transPairs:0;numbers["TransMotionErrAvgScale"]=transPairs>0?scaleTotal/transPairs:0;numbers["TransMotionErrAvgRotate"]=transPairs>0?rotateTotal/transPairs:0;numbers["TransMotionErrMaxTrans"]=distanceMax;numbers["TransMotionErrMaxScale"]=scaleAtMax;numbers["TransMotionErrMaxRotate"]=rotateAtMax;integers["TransMotionErrPicIndex"]=transIndex;
  float angleTotal=0,angleMaximum=0,relativeRotMax=0;int angleIndex=0,votes=0;
  if(r is not null){if(rotationPoses.Count!=r.AnglesRadians.Count+1)throw new ArgumentException("Rotation pose count must match solved adjacent angles.",nameof(rotationPoses));for(int i=0;i<r.AnglesRadians.Count;i++){
   float expected=(float)rotationPoses[i+1].R-(float)rotationPoses[i].R;while(expected>180)expected-=360;while(expected< -180)expected+=360;float angle=(float)r.AnglesRadians[i];
   if(t is not null){(float X,float Y) Map(float x,float y){float d=h[6]*x+h[7]*y+h[8]+Epsilon;return((h[0]*x+h[1]*y+h[2])/d,(h[3]*x+h[4]*y+h[5])/d);}var origin=Map(0,0);var start=Map(1,0);var end=Map(MathF.Cos(angle),MathF.Sin(angle));angle=MathF.Atan2(end.Y-origin.Y,end.X-origin.X)-MathF.Atan2(start.Y-origin.Y,start.X-origin.X);while(angle< -MathF.PI)angle+=2*MathF.PI;while(angle>MathF.PI)angle-=2*MathF.PI;}
   float measured=angle*Degrees,error=Math.Abs(measured-expected);angleTotal+=error;if(error>angleMaximum){angleMaximum=error;angleIndex=i;}relativeRotMax=Math.Max(relativeRotMax,Math.Abs(error/(expected+Epsilon)));votes+=expected*measured>0?1:-1;
  }}
  int rotPairs=r?.AnglesRadians.Count??0;numbers["RotMotionErrAvg"]=rotPairs>0?angleTotal/(rotPairs+Epsilon):0;numbers["RotMotionErrMax"]=angleMaximum;integers["RotMotionErrPicIndex"]=angleIndex;integers["RotDirectState"]=r is null?0:t is null?1:votes<0?-1:1;
  numbers["RotateCenterPointX"]=r?.CenterX??0;numbers["RotateCenterPointY"]=r?.CenterY??0;
  float determinant=h[0]*(h[4]*h[8]-h[5]*h[7])-h[1]*(h[3]*h[8]-h[5]*h[6])+h[2]*(h[3]*h[7]-h[4]*h[6]);
  if(Math.Abs(determinant)>=2.220446049250313e-16){float ox=h[2]/h[8],oy=h[5]/h[8];numbers["DirectionXVectorX"]=(h[0]+h[2])/(h[6]+h[8])-ox;numbers["DirectionXVectorY"]=(h[3]+h[5])/(h[6]+h[8])-oy;numbers["DirectionYVectorX"]=(h[1]+h[2])/(h[7]+h[8])-ox;numbers["DirectionYVectorY"]=(h[4]+h[5])/(h[7]+h[8])-oy;}else foreach(var name in new[]{"DirectionXVectorX","DirectionXVectorY","DirectionYVectorX","DirectionYVectorY"})numbers[name]=0;
  numbers["MoveDirec"]=MathF.Sqrt(h[1]*h[1]+h[4]*h[4])/(Epsilon+MathF.Sqrt(h[0]*h[0]+h[3]*h[3]));
  uint status=numbers["TransError"]>1.5?0x1010304bu:numbers["TransEstMax"]>1.5?0x1010304au:numbers["RotError"]>1.5?0x1010304du:numbers["RotErrMax"]>1.5?0x1010304cu:relativeTransMax>.07999999821186066f?0x1010304eu:relativeRotMax>.07999999821186066f?0x1010304fu:0;
  return new(new System.Collections.ObjectModel.ReadOnlyDictionary<string,double>(numbers),new System.Collections.ObjectModel.ReadOnlyDictionary<string,int>(integers),status);
 }
}
