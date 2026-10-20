using VisionCore;
namespace VisionPlugins.Calibration;
// Complete source-context composition candidate; no source failure -> legacy decoder fallback.
internal static class NImageSourceBoardRecognition {
 internal sealed record Result(NImageDetectedBoard? Board,string? UnsupportedReason,bool? SelectedRefined=null);
 // dispatch must come from the host's explicitly validated source CPU mapping.
 // No product-wide CPUID policy is guessed from Avx2.IsSupported here.
 internal static Result Recognize(GrayImage source,int boardType,NImageLattice lattice,
  NImageSourceTagSampler.MeanDispatch dispatch,CancellationToken token=default){
  var grid=lattice.SourceGrid;if(grid is null)return new(null,"Source grid context required");
  float[]? gridMatrix=null;
  if(boardType is not (6 or 8)){
   var pairs=grid.OrderedPoints.Select(p=>((float)p.Column,(float)p.Row,p.X,p.Y)).ToArray();
   var fit=NPointSolver.FitNativePoints(pairs,1,3,2f,20,1e-4f);if(fit.ErrorCode!=0||fit.Matrix is null)return new(null,null);gridMatrix=fit.Matrix;
  }
  NImageSourceOpening.Result? selectedOpening=null;
  NImageSourceRecognitionBridge.DecodeResult Decode(NImageSourceRecognitionBridge.Proposal p){
   token.ThrowIfCancellationRequested();var q=p.Opening.Select(v=>new NImageSourceTagSampler.F(v.X,v.Y)).ToArray();
   var opening=NImageSourceOpening.Decode(source,q,dispatch);
   if(opening.UnsupportedReason is not null)return new(false,[],opening.UnsupportedReason);
   var sample=opening.Sample;if(sample is null||sample.Status!=0||sample.Payload.Length!=12)return new(false,[]);
   selectedOpening=opening;return new(true,sample.Payload);
  }
  NImageSourceRecognitionBridge.FloatPoint Project(float x,float y){var p=NImageSourceTagSampler.Project(gridMatrix!,x,y);return new(p.X,p.Y);}
  var result=boardType is 6 or 8?NImageSourceRecognitionBridge.RecognizeCells(grid,boardType,Decode,token):NImageSourceRecognitionBridge.Recognize(grid,boardType,Project,Decode,cancel:token);
  if(result.UnsupportedReason is not null)return new(null,result.UnsupportedReason);
  if(result.Selected is null||selectedOpening?.ActualInner is null)return new(null,null);
  var openingPoints=result.Selected.Proposal.Opening.Select(p=>new Point2(p.X,p.Y)).ToArray();
  var inner=selectedOpening.ActualInner.Select(p=>new Point2(p.X,p.Y)).ToArray();
  var tag=new NImageDetectedTag(result.Selected.Identity,Array.AsReadOnly(openingPoints),Array.AsReadOnly(inner));
  return new(new(result.Selected.Identity.GridPitch,result.Points,Array.AsReadOnly(new[]{tag})),null,selectedOpening.Refined);
 }
}


