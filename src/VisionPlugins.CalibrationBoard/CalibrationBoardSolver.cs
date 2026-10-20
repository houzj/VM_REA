using VisionCore;
using VisionPlugins.Calibration;
namespace VisionPlugins.CalibrationBoard;

public sealed record BoardCalibrationOptions(int BoardType=1,int CoordinateMode=1,double PhysicalSize=1,double OriginX=0,double OriginY=0,double RotationAngle=0,int Freedom=1,int Weight=2,int Filter=1,int DotMode=1,int Circularity=50,double WeightFactor=20,double DistThreshold=30,double SampleRatio=60,int EdgeLow=20,int EdgeHigh=40,NImageSourceExecution? SourceExecution=null);
public sealed record BoardCalibrationResult(uint Error,IReadOnlyList<NImageBoardPoint> Points,NPointSolveResult? Fit,string? Diagnostic=null);
public sealed record BoardRecognitionResult(uint Error,IReadOnlyList<NImageBoardPoint> Points,double? EncodedGridPitch=null,string? Diagnostic=null);

/// <summary>Recognition is independent of board world calibration. Original source grid orientation fitting is still an intrinsic detection step. Evidence is bounded by recorded fixtures.</summary>
public static class CalibrationBoardSolver {
 static bool InvalidWorld(BoardCalibrationOptions options)=>options.CoordinateMode is not (1 or 2)||options.PhysicalSize is <.01 or >1000||!double.IsFinite(options.PhysicalSize+options.OriginX+options.OriginY+options.RotationAngle);
 static BoardCalibrationResult InvalidOptions()=>new(0xe000030e,[],null,"标定参数无效。");
 public static BoardCalibrationResult Solve(GrayImage image,BoardCalibrationOptions options,CancellationToken token=default){
  if(image.Width<64||image.Height<64)return new(0x10103054u,[],null,"标定板输入图像的宽和高均须至少64像素。");
  return SolveCropped(image,options,0,0,token);
 }
 public static BoardCalibrationResult SolveCropped(GrayImage image,BoardCalibrationOptions options,int roiOriginX,int roiOriginY,CancellationToken token=default){
  token.ThrowIfCancellationRequested();if(InvalidWorld(options))return InvalidOptions();
  return Finish(RecognizeCropped(image,options,roiOriginX,roiOriginY,token),options);
 }
 /// <summary>Preserves original-image decoding while detecting in the exact local crop.</summary>
 public static BoardCalibrationResult SolveCropped(GrayImage source,GrayImage crop,BoardCalibrationOptions options,int roiOriginX,int roiOriginY,CancellationToken token=default){
  token.ThrowIfCancellationRequested();if(InvalidWorld(options))return InvalidOptions();
  return Finish(RecognizeCropped(source,crop,options,roiOriginX,roiOriginY,token),options);
 }
 public static BoardCalibrationResult SolveMasked(GrayImage source,BoardCalibrationOptions options,CalibrationBoardMask mask,CancellationToken token=default){
  if(source.Width<64||source.Height<64)return new(0x10103054u,[],null,"标定板输入图像的宽和高均须至少64像素。");
  token.ThrowIfCancellationRequested();if(InvalidWorld(options))return InvalidOptions();
  return Finish(RecognizeMasked(source,options,mask,token),options);
 }
 static BoardCalibrationResult Finish(BoardRecognitionResult detected,BoardCalibrationOptions options){
  if(detected.Error!=0)return new(detected.Error,detected.Points,null,detected.Diagnostic);
  var stages=CalibrationBoardFitting.Fit(detected.Points,options);
  var fit=CalibrationBoardFitting.Analyze(detected.Points,options,stages);return new(fit.ErrorCode,detected.Points,fit);
 }
 /// <summary>Detect source indices and global float pixels only; ignores world-coordinate and calibration weight options. EncodedGridPitch is board metadata, not an applied world transform.</summary>
 public static BoardRecognitionResult Recognize(GrayImage image,BoardCalibrationOptions options,CancellationToken token=default){
  if(image.Width<64||image.Height<64)return new(0x10103054u,[],null,"标定板输入图像的宽和高均须至少64像素。");
  return RecognizeCropped(image,options,0,0,token);
 }
 /// <summary>Caller validates original image dimensions and passes real local crop pixels. Local-wrapper guards remain distinct from original-image guards.</summary>
 public static BoardRecognitionResult RecognizeCropped(GrayImage image,BoardCalibrationOptions options,int roiOriginX,int roiOriginY,CancellationToken token=default)=>RecognizeLocal(image,options,roiOriginX,roiOriginY,null,token);
 /// <summary>Encoded ROI kernels detect locally but decode from the original image.</summary>
 public static BoardRecognitionResult RecognizeCropped(GrayImage source,GrayImage crop,BoardCalibrationOptions options,int roiOriginX,int roiOriginY,CancellationToken token=default){
  if(options.BoardType is not (6 or 7 or 8 or 9))return RecognizeCropped(crop,options,roiOriginX,roiOriginY,token);
  var lattice=NImageSourceLatticeAdapter.FindSource(crop,30f,140f,token,options.BoardType is 6 or 8?1:2,options.Filter);
  if(lattice is null)return new(0x10000107,[],null,"未识别到足够的标定板点。");
  var points=lattice.Points.Select(p=>p with{X=(float)p.X+(float)roiOriginX,Y=(float)p.Y+(float)roiOriginY}).ToArray();
  var quads=lattice.CodeQuadrilaterals.Select(q=>(IReadOnlyList<Point2>)q.Select(p=>new Point2((float)p.X+(float)roiOriginX,(float)p.Y+(float)roiOriginY)).ToArray()).ToArray();
  var decoded=DecodeSource(source,options,new(lattice.PitchX,lattice.PitchY,points,quads){SourceGrid=lattice.SourceGrid?.RebuildForProjectedPoints(points.Select(p=>new NImageSourceGridPoint(p.Column,p.Row,(float)p.X,(float)p.Y)).ToArray())},token);
  return decoded is null||decoded.Points.Count<4?new(0x10000107,[],null,"未识别到足够的标定板点。"):new(0,decoded.Points,decoded.GridPitch);
 }
 /// <summary>Source mask: unmodified bbox pixels, local center filtering, original indices, float-add source origin. Encoded mask projection precedes identity decoding.</summary>
 public static BoardRecognitionResult RecognizeMasked(GrayImage source,BoardCalibrationOptions options,CalibrationBoardMask mask,CancellationToken token=default){
  if(source.Width<64||source.Height<64)return new(0x10103054u,[],null,"标定板输入图像的宽和高均须至少64像素。");
  if(mask.ImageWidth!=source.Width||mask.ImageHeight!=source.Height||mask.Pixels.Length!=checked(source.Width*source.Height))throw new ArgumentException("Mask and source dimensions differ.",nameof(mask));
  if(options.BoardType is not (1 or 2 or 6 or 7 or 8 or 9))return new(0xe000030e,[],null,"标定板类型无效。");
  if(mask.Width<=0||mask.Height<=0)throw new NotSupportedException("空ROI掩膜的原厂异常分支尚未确认。");
  byte[] original=source.Pixels.ToArray(),pixels=new byte[checked(mask.Width*mask.Height)];
  for(int row=0;row<mask.Height;row++)Buffer.BlockCopy(original,(mask.Top+row)*source.Width+mask.Left,pixels,row*mask.Width,mask.Width);
  if(options.BoardType is 6 or 7 or 8 or 9){
   var local=NImageSourceLatticeAdapter.FindSource(new(mask.Width,mask.Height,pixels),30f,140f,token,options.BoardType is 6 or 8?1:2,options.Filter);
   if(local is null)return new(0x10000107,[],null,"未识别到足够的标定板点。");
   var points=CalibrationBoardMaskedGrid.Project(local.Points,mask);
   var quads=local.CodeQuadrilaterals.Select(q=>(IReadOnlyList<Point2>)q.Select(p=>new Point2((float)p.X+mask.Left,(float)p.Y+mask.Top)).ToArray()).ToArray();
   var decoded=DecodeSource(source,options,new(local.PitchX,local.PitchY,points,quads){SourceGrid=local.SourceGrid?.RebuildForProjectedPoints(points.Select(p=>new NImageSourceGridPoint(p.Column,p.Row,(float)p.X,(float)p.Y)).ToArray())},token);
   return decoded is null||decoded.Points.Count<4?new(0x10000107,[],null,"未识别到足够的标定板点。"):new(0,decoded.Points,decoded.GridPitch);
  }
  bool Keep(NImageBoardPoint point){int x=(int)(.5f+(float)point.X),y=(int)(.5f+(float)point.Y);return x>=0&&y>=0&&x<mask.Width&&y<mask.Height&&mask.Pixels[(mask.Top+y)*source.Width+mask.Left+x]!=0;}
  return RecognizeLocal(new(mask.Width,mask.Height,pixels),options,mask.Left,mask.Top,Keep,token);
 }
 static BoardRecognitionResult RecognizeLocal(GrayImage image,BoardCalibrationOptions options,int roiOriginX,int roiOriginY,Func<NImageBoardPoint,bool>? keep,CancellationToken token){
  token.ThrowIfCancellationRequested();if(options.BoardType is not (1 or 2 or 6 or 7 or 8 or 9))return new(0xe000030e,[],null,"标定板类型无效。");
  IReadOnlyList<NImageBoardPoint> points;double? encodedGridPitch=null;
  if(options.BoardType==1){var lattice=NImageSourceGridPyramid.Find(image,30f,140f,options.Filter,cancellationToken:token);points=lattice.Points.Select(p=>new NImageBoardPoint(p.Column,p.Row,p.X,p.Y)).ToArray();}
  else if(options.BoardType==2)points=Circles(image,options,token);
  else {var lattice=NImageSourceLatticeAdapter.FindSource(image,30f,140f,token,options.BoardType is 6 or 8?1:2,options.Filter);var detected=lattice is null?null:DecodeSource(image,options,lattice,token);points=detected?.Points??[];encodedGridPitch=detected?.GridPitch;}
  if(keep is not null)points=points.Where(keep).ToArray();
  if(roiOriginX!=0||roiOriginY!=0)points=points.Select(point=>new NImageBoardPoint(point.IndexX,point.IndexY,(float)point.X+(float)roiOriginX,(float)point.Y+(float)roiOriginY)).ToArray();
  if(points.Count<4)return new(options.BoardType==1?0x10103000u:0x10000107u,[],null,"未识别到足够的标定板点。");
  return new(0,points,encodedGridPitch);
 }
 static NImageDetectedBoard? DecodeSource(GrayImage image,BoardCalibrationOptions options,NImageLattice lattice,CancellationToken token){
  var result=NImageSourceRecognition.RecognizeLattice(image,options.BoardType,lattice,options.SourceExecution??NImageSourceExecution.Current(),token);
  if(result.UnsupportedReason is not null)throw new NotSupportedException(result.UnsupportedReason);
  return result.Board;
 }
 static IReadOnlyList<NImageBoardPoint> Circles(GrayImage image,BoardCalibrationOptions options,CancellationToken token){
  // Factor5 preprocessing, relative area/circularity, contours and circle fitting
  // and 8cd840 topology/consistent subgrid/canonical ordering follow recovered source.
  // 877f90 validates both ROI and default-factor5 reduced image axes >63 before candidate extraction.
  if(image.Width<64||image.Height<64||image.Width/5<64||image.Height/5<64)return [];
  var components=CalibrationBoardCircleCandidates.Find(image,options.Circularity,options.EdgeLow,options.EdgeHigh,options.DotMode,token:token);
  if(components.Count<4)return [];token.ThrowIfCancellationRequested();
  var matched=CalibrationBoardCircleGrid.Match(components);
  return matched.Points.Select(p=>new NImageBoardPoint(p.IndexX,p.IndexY,p.X,p.Y)).ToArray();
 }
}



