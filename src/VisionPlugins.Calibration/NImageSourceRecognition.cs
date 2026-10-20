using VisionCore;
namespace VisionPlugins.Calibration;

public sealed record NImageSourceExecution(int NativeCpuClass){
 public static NImageSourceExecution Current()=>new(NImageSourceCpuPolicy.Detect());
}
public sealed record NImageSourceRecognitionResult(NImageDetectedBoard? Board,string? UnsupportedReason,bool? Refined=null);
public static class NImageSourceRecognition {
 public static NImageSourceRecognitionResult Find(GrayImage image,int boardType,NImageSourceExecution execution,float minimumAngle=30f,float maximumAngle=135f,int filterStatus=1,CancellationToken token=default){
  var lattice=NImageSourceLatticeAdapter.FindSource(image,minimumAngle,maximumAngle,token,boardType is 6 or 8?1:2,filterStatus);
  return lattice is null?new(null,null):RecognizeLattice(image,boardType,lattice,execution,token);
 }
 public static NImageSourceRecognitionResult RecognizeLattice(GrayImage image,int boardType,NImageLattice lattice,NImageSourceExecution execution,CancellationToken token=default){
  // CPU11 is the finite source+oracle-supported production candidate. Other host
  // classes require their explicit source backend, never a guessed AVX alias.
  if(execution.NativeCpuClass!=11)return new(null,$"Source recognition backend for original CPU class {execution.NativeCpuClass} is not validated");
  var result=NImageSourceBoardRecognition.Recognize(image,boardType,lattice,NImageSourceTagSampler.MeanDispatch.Cpu11,token);
  return new(result.Board,result.UnsupportedReason,result.SelectedRefined);
 }
}
