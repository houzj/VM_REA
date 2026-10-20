namespace VisionPlugins.Calibration;

public sealed record NImageBatchSolveResult(uint ErrorCode,bool Complete,NImageTranslationSolveResult? Translation,NImageRotationSolveResult? Rotation,IReadOnlyList<double> Matrix,string? Diagnostic=null);

/// <summary>Connects owned recognition frames to the independently reconstructed numerical stages.
/// Error metrics, source file camera movement and MatrixFix are projected by the production module.</summary>
public static class NImageBatchSolver
{
 public static NImageBatchSolveResult Solve(NImageRecognitionBatch batch,CancellationToken cancellationToken=default){
  ArgumentNullException.ThrowIfNull(batch);cancellationToken.ThrowIfCancellationRequested();
  if(batch.ErrorCode!=0)return new(batch.ErrorCode,false,null,null,[],"Recognition failed; no previous matrix is reused.");
  if(!batch.Sequence.Ready)return new(0,false,null,null,[]);
  if(batch.Boards.Count!=batch.Sequence.Frames.Count)throw new ArgumentException("Recognized board count must match the owned frame sequence.",nameof(batch));
  var frames=batch.Sequence.Frames.Select((f,i)=>new NImageSolveFrame(batch.Boards[i],new(f.PhysicalX,f.PhysicalY,f.Angle))).ToArray();
  var translation=frames.Where((_,i)=>batch.Sequence.Frames[i].Kind==1).ToArray();var rotation=frames.Where((_,i)=>batch.Sequence.Frames[i].Kind==2).ToArray();
  // Source 7cf3f0 initializes the matrix to zero and only writes the translation
  // fit when >=4 translation frames exist. Controlled SDK 2/3/5 rotation-only
  // runs succeed with this zero matrix; an identity matrix would change behavior.
  if(translation.Length==0){var only=NImageSolveRotation.Solve(rotation);return only.ErrorCode==0?new(0,true,null,only,Array.AsReadOnly(new double[9])):new(only.ErrorCode,false,null,only,[]);}
  cancellationToken.ThrowIfCancellationRequested();var t=NImageSolveTranslation.Solve(translation);if(t.ErrorCode!=0)return new(t.ErrorCode,false,t,null,[]);
  if(rotation.Length==0)return new(0,true,t,null,Array.AsReadOnly(t.Matrix.ToArray()));
  cancellationToken.ThrowIfCancellationRequested();var r=NImageSolveRotation.Solve(rotation);if(r.ErrorCode!=0)return new(r.ErrorCode,false,t,r,[]);
  return new(0,true,t,r,NImageSolveRotation.CenterMatrix(t.Matrix,r));
 }
}
