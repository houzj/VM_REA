using System.Globalization;
namespace VisionPlugins.Calibration;

/// <summary>Source WriteXmlData fields, distinct from ProcessOutput and SDK export XML.</summary>
public static class NImageCalibrationExport{
 public static NImageCalibrationFileData Build(NImageRecognitionBatch batch,NImageBatchSolveResult solved,NImageOutputMetricsResult metrics,IReadOnlyList<double>? fixedMatrix=null,bool cameraMoves=false,string? created=null,NImageSequenceSnapshot? currentSequence=null,int? translationCount=null,int? rotationCount=null){
  if(!solved.Complete||solved.ErrorCode!=0)throw new InvalidOperationException("尚无完整标定结果。");var tracks=NImageTrackedCorners.Build(batch.Boards);if(tracks.Count<2)throw new InvalidOperationException("原厂模块文件需要至少两条完整公共角点轨迹。");
  var frames=(currentSequence??batch.Sequence).Frames;int trans=translationCount??frames.Count(f=>f.Kind==1),rot=rotationCount??frames.Count(f=>f.Kind==2);var image=Enumerable.Range(0,tracks[0].Pixels.Count).Select(i=>{var a=tracks[0].Pixels[i];var b=tracks[1].Pixels[i];float angle=(float)((double)MathF.Atan2((float)b.Y-(float)a.Y,(float)b.X-(float)a.X)/Math.PI*180);return new NImagePose(a.X,a.Y,angle);}).ToArray();var world=frames.Select(f=>new NImagePose(f.PhysicalX,f.PhysicalY,f.Angle)).ToArray();int relation=-999;
  if(rot==3&&(image.Length<trans+3||world.Length<trans+3))throw new NotSupportedException("原厂旋转关系访问超出保留结果，未模拟越界内存。");
  if(rot==3){static float[] Unwrap(IEnumerable<double> values){var a=values.Select(v=>(float)v).ToArray();if(a[0]< -90&&a[2]>90||a[0]>90&&a[2]< -90)for(int i=0;i<3;i++)if(a[i]>=-180&&a[i]<0)a[i]+=360;return a;}var a=Unwrap(image.Skip(trans).Select(p=>p.R));var b=Unwrap(world.Skip(trans).Select(p=>p.R));bool Ascending(float[] v){if(v[1]-v[0]>0&&v[2]-v[1]>0)return true;if(v[1]-v[0]<0&&v[2]-v[1]<0)return false;throw new InvalidOperationException("三张旋转图的图像角度与物理角度须分别严格单调。");}relation=Ascending(a)==Ascending(b)?1:-1;}
  var h=(fixedMatrix??solved.Matrix).ToArray();double px=(float)Math.Sqrt(h[0]*h[0]+h[3]*h[3]),py=(float)Math.Sqrt(h[1]*h[1]+h[4]*h[4]);int handedness=-999;
  // Helper 1a950 works in the inverse matrix coordinate system. The source
  // negates its orientation when the world/camera rotation relation is false.
  double determinant=h[0]*h[4]-h[1]*h[3];if(h[0]!=0&&Math.Abs(determinant)>1e-15){double xAngle=Math.Atan2(-h[3]/determinant,h[4]/determinant)*180/Math.PI,yAngle=Math.Atan2(h[0]/determinant,-h[1]/determinant)*180/Math.PI;if(xAngle-yAngle< -180||xAngle-yAngle>180){if(xAngle<0)xAngle+=360;if(yAngle<0)yAngle+=360;}handedness=(yAngle<xAngle?-1:1)*(relation==1?1:-1);}
  double Value(string name)=>metrics.Numbers[name];var data=new NImageCalibrationFileData(created??DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss",CultureInfo.InvariantCulture),trans,rot,unchecked((int)metrics.Status),Value("TransError"),rot>0?Value("RotError"):-999,Value("TransWorldError"),rot>0?Value("RotWorldError"):-999,px,py,h[0]==0?0:(float)((Math.Sqrt(h[0]*h[0]+h[3]*h[3])+Math.Sqrt(h[1]*h[1]+h[4]*h[4]))*.5),image,world,relation,handedness,new(Value("RotateCenterPointX"),Value("RotateCenterPointY"),-999),Array.AsReadOnly(h));return NImageFile.ForCameraMovement(data,cameraMoves);
 }
}
