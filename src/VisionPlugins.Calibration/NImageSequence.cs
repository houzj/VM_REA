using VisionCore;
namespace VisionPlugins.Calibration;

/// <summary>Source-confirmed acquisition state of IMVSNImageCalibModu, separate from coded-board recognition.</summary>
public sealed record NImageSequenceParameters(int TranslationCount=9,int RotationCount=3,double BaseX=0,double BaseY=0,double StepX=1,double StepY=1,int FirstAxis=1,int DirectionChangeCount=3,double BaseAngle=0,double AngleStep=1)
{
 public int Total=>TranslationCount+RotationCount;
 public void Validate(){if(TranslationCount is <0 or >15||RotationCount is <0 or >9||Total==0||DirectionChangeCount is <1 or >16||FirstAxis is <1 or >2)throw new ArgumentOutOfRangeException(nameof(TranslationCount));if(new[]{BaseX,BaseY,StepX,StepY,BaseAngle,AngleStep}.Any(v=>!double.IsFinite(v)))throw new ArgumentException("Non-finite calibration coordinate.");}
}
public sealed record NImageFrame(GrayImage Image,int Kind,double PhysicalX,double PhysicalY,double Angle);
public sealed record NImageSequenceSnapshot(IReadOnlyList<NImageFrame> Frames,int Current,int Total){public bool Ready=>Current>=Total;}
public sealed class NImageSequence
{
 readonly List<NImageFrame> frames=[];
 readonly object gate=new();
 /// <summary>All three optional subscribed motion values must be present; otherwise the native module replaces all three.</summary>
 public static (int Kind,double X,double Y,double Angle) Motion(int index,NImageSequenceParameters p,double? physicalX=null,double? physicalY=null,double? angle=null){
  p.Validate();if(index<0)throw new ArgumentOutOfRangeException(nameof(index));int kind=index<p.TranslationCount?1:2;
  if(physicalX.HasValue&&physicalY.HasValue&&angle.HasValue){if(new[]{physicalX.Value,physicalY.Value,angle.Value}.Any(v=>!double.IsFinite(v)))throw new ArgumentException("Non-finite subscribed motion.");return(kind,physicalX.Value,physicalY.Value,angle.Value);}
  if(kind==2)return(kind,0,0,(index-p.TranslationCount)*p.AngleStep+p.BaseAngle);
  int row=index/p.DirectionChangeCount,col=index%p.DirectionChangeCount;
  int across=(row%2==0?col:p.DirectionChangeCount-col-1)-1,along=row-1;
  return(kind,(p.FirstAxis==1?across:along)*p.StepX+p.BaseX,(p.FirstAxis==1?along:across)*p.StepY+p.BaseY,0);
 }
 public NImageSequenceSnapshot Add(GrayImage image,NImageSequenceParameters p,double? physicalX=null,double? physicalY=null,double? angle=null){ArgumentNullException.ThrowIfNull(image);p.Validate();lock(gate){if(frames.Count>=p.Total)frames.Clear();var m=Motion(frames.Count,p,physicalX,physicalY,angle);frames.Add(new(new GrayImage(image.Width,image.Height,image.Pixels),m.Kind,m.X,m.Y,m.Angle));return Snapshot(p.Total);}}
 public NImageSequenceSnapshot Inspect(int total){lock(gate)return Snapshot(total);}
 NImageSequenceSnapshot Snapshot(int total)=>new(Array.AsReadOnly(frames.Select(f=>f with{Image=new GrayImage(f.Image.Width,f.Image.Height,f.Image.Pixels)}).ToArray()),frames.Count,total);
 public void Clear(){lock(gate)frames.Clear();}
}
