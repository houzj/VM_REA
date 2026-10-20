using VisionCore;
using VisionPlugins.CalibrationBoard;

namespace VisionPlugins.DistortionCalibration;

public sealed record DistortionCalibrationRecognitionResult(uint ErrorCode,
 IReadOnlyList<DistortionCalibrationPoint> Points,string? Diagnostic=null){
 // 7ed0c0 receives unit grid indices for this tool, including encoded boards.
 // The encoded board's physical pitch belongs to other calibration callers.
 public float GridPitch=>1f;
}

/// <summary>Recognition adapter only. Does not run another tool's world fit or
/// replace missing calibration/evaluation outputs with default values.</summary>
public static class DistortionCalibrationRecognition {
 public static DistortionCalibrationRecognitionResult Recognize(GrayImage image,
  int boardType=1,int filter=1,int circularity=50,int dotMode=1,int edgeLow=20,int edgeHigh=40,
  CancellationToken token=default){
  var found=CalibrationBoardSolver.Recognize(image,new(BoardType:boardType,Filter:filter,
   DotMode:dotMode,Circularity:circularity,EdgeLow:edgeLow,EdgeHigh:edgeHigh),token);
  return Convert(found);
 }
 public static DistortionCalibrationRecognitionResult RecognizeCropped(GrayImage crop,
  int originX,int originY,int boardType=1,int filter=1,int circularity=50,int dotMode=1,
  int edgeLow=20,int edgeHigh=40,CancellationToken token=default){
  var found=CalibrationBoardSolver.RecognizeCropped(crop,new(BoardType:boardType,Filter:filter,
   DotMode:dotMode,Circularity:circularity,EdgeLow:edgeLow,EdgeHigh:edgeHigh),originX,originY,token);
  return Convert(found);
 }
 // Encoded crops retain the original image for source-coordinate tag sampling.
 public static DistortionCalibrationRecognitionResult RecognizeCropped(GrayImage source,GrayImage crop,
  int originX,int originY,int boardType=1,int filter=1,int circularity=50,int dotMode=1,
  int edgeLow=20,int edgeHigh=40,CancellationToken token=default){
  var found=CalibrationBoardSolver.RecognizeCropped(source,crop,new(BoardType:boardType,Filter:filter,
   DotMode:dotMode,Circularity:circularity,EdgeLow:edgeLow,EdgeHigh:edgeHigh),originX,originY,token);
  return Convert(found);
 }
 public static DistortionCalibrationRecognitionResult RecognizeMasked(GrayImage source,RectangleRoiMask mask,
  int boardType=1,int filter=1,int circularity=50,int dotMode=1,
  int edgeLow=20,int edgeHigh=40,CancellationToken token=default){
  var found=CalibrationBoardSolver.RecognizeMasked(source,new(BoardType:boardType,Filter:filter,
   DotMode:dotMode,Circularity:circularity,EdgeLow:edgeLow,EdgeHigh:edgeHigh),
   new(mask.ImageWidth,mask.ImageHeight,mask.Pixels,mask.Left,mask.Top,mask.Width,mask.Height,mask.Vertices),token);
  return Convert(found);
 }
 static DistortionCalibrationRecognitionResult Convert(BoardRecognitionResult found)=>new(found.Error,
  found.Points.Select(p=>new DistortionCalibrationPoint((float)p.X,(float)p.Y,p.IndexX,p.IndexY)).ToArray(),found.Diagnostic);
}
