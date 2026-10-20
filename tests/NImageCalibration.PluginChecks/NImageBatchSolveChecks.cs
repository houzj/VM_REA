using System.IO;
using System.Text.Json;
using VisionCore;
using VisionPlugins.Calibration;
public static class NImageBatchSolveChecks{
 public static int Run(){int checks=0;void Check(bool pass,string message){checks++;if(!pass)throw new Exception(message);}
  using var evidence=JsonDocument.Parse(File.ReadAllText("docs/plugins/n-image-solver-evidence.json"));var row=evidence.RootElement.GetProperty("cases").EnumerateArray().Single(r=>r.GetProperty("count").GetInt32()==11);var image=new GrayImage(1,1,new byte[1]);
  var frames=Enumerable.Range(0,11).Select(i=>new NImageFrame(image,i<9?1:2,i<9?i%3:0,i<9?i/3:0,i<9?0:(i-9)*10)).ToArray();
  var boards=Enumerable.Range(0,11).Select(i=>new NImageDetectedBoard(10,row.GetProperty("corners").EnumerateArray().Select(c=>new NImageBoardPoint(c.GetProperty("world")[0].GetInt32(),c.GetProperty("world")[1].GetInt32(),c.GetProperty("pixels")[i][0].GetDouble(),c.GetProperty("pixels")[i][1].GetDouble())).ToArray(),[])).ToArray();
  var batch=new NImageRecognitionBatch(new(frames,11,11),0,boards);var result=NImageBatchSolver.Solve(batch);Check(result.Complete&&result.ErrorCode==0,"Complete recognized batch reaches numerical stages");Check(Math.Abs(result.Rotation!.CenterX-row.GetProperty("center")[0].GetDouble())<1e-4,"Native two-rotation center");Check(result.Matrix.Count==9,"Matrix returned only on completed numerical solve");
  for(int i=0;i<9;i++)Check(Math.Abs(result.Matrix[i]-row.GetProperty("matrix")[i].GetDouble())<1e-3,"Native composed matrix component "+i);
  Check(!NImageBatchSolver.Solve(batch with{Sequence=new(frames[..3],3,11),Boards=[]}).Complete,"Partial batch cannot fabricate ready result");
  var failed=NImageBatchSolver.Solve(batch with{ErrorCode=0x10103000});Check(!failed.Complete&&failed.Matrix.Count==0&&failed.ErrorCode==0x10103000,"Failed recognition cannot reuse stale matrix");
  var rotationOnly=NImageBatchSolver.Solve(new(new(frames[9..],2,2),0,boards[9..]));Check(rotationOnly.Complete&&rotationOnly.Matrix.Count==9&&rotationOnly.Matrix.All(v=>v==0),"Original rotation-only branch succeeds with zero matrix");Check(Math.Abs(rotationOnly.Rotation!.CenterX-row.GetProperty("center")[0].GetDouble())<1e-4,"Rotation-only keeps original center solve");
  using var boundaries=JsonDocument.Parse(File.ReadAllText("docs/plugins/n-image-count-boundary-evidence.json"));foreach(var boundary in boundaries.RootElement.GetProperty("cases").EnumerateArray()){int t=boundary.GetProperty("trans").GetInt32();var indices=Enumerable.Range(0,t).Concat(new[]{9}).ToArray();var selectedFrames=indices.Select(i=>frames[i]).ToArray();var selectedBoards=indices.Select(i=>boards[i]).ToArray();var invalid=NImageBatchSolver.Solve(new(new(selectedFrames,selectedFrames.Length,selectedFrames.Length),0,selectedBoards));Check(!invalid.Complete&&invalid.ErrorCode==boundary.GetProperty("code").GetUInt32(),"Official SDK singleton rotation count rejection "+t);}
  using var cancellation=new CancellationTokenSource();cancellation.Cancel();try{NImageBatchSolver.Solve(batch,cancellation.Token);throw new Exception("Cancellation ignored");}catch(OperationCanceledException){checks++;}
  return checks;
 }
}
