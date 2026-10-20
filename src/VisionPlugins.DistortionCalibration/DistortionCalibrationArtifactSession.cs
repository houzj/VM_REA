namespace VisionPlugins.DistortionCalibration;

/// <summary>Owned result lifetime observed through original module ProcessAlg/SaveCalibPath.
/// Failure changes run status but retains the last successfully calibrated file payload.
/// This session does not invent calibration results or execute a substitute algorithm.</summary>
public sealed class DistortionCalibrationArtifactSession:IDisposable{
 readonly object gate=new();DistortionCalibrationFile? artifact;bool disposed;uint lastError;
 public DistortionCalibrationFile? LastSuccessfulArtifact{get{lock(gate)return artifact;}}
 public uint LastRunErrorCode{get{lock(gate)return lastError;}}
 public void AcceptFit(DistortionCalibrationFile result){ArgumentNullException.ThrowIfNull(result);lock(gate){ObjectDisposedException.ThrowIf(disposed,this);artifact=result;lastError=0;}}
 public void Fail(uint errorCode){if(errorCode==0)throw new ArgumentOutOfRangeException(nameof(errorCode));lock(gate){ObjectDisposedException.ThrowIf(disposed,this);lastError=errorCode;}}
 public void Export(string path){
  DistortionCalibrationFileLifecycle.ValidateExplicitPath(path);
  lock(gate){ObjectDisposedException.ThrowIf(disposed,this);if(artifact is null)throw new DistortionCalibrationFileFailure(0x80100003,"尚无可导出的标定结果（0x80100003）。");DistortionCalibrationFileLifecycle.WriteExplicit(artifact,path);}
 }
 public void Dispose(){lock(gate){artifact=null;lastError=0;disposed=true;}}
}
