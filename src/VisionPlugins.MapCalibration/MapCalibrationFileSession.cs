using System.IO;
namespace VisionPlugins.MapCalibration;

/// <summary>Per-module file-mode cache reconstructed from SetParam/ReadFileMatrix/Process. No UI or global state.</summary>
public sealed class MapCalibrationFileSession(string baseDirectory)
{
 readonly string[] paths=["",""];readonly float[]?[] matrices=new float[]?[2];
 public string InputPath(int side)=>paths[side];
 public IReadOnlyList<float>? CachedMatrix(int side)=>matrices[side]?.ToArray();
 public string OutputPath{get;private set;}="";
 public bool RefreshOutput{get;set;}
 public bool CalibrationValid{get;private set;}
 public MapCalibrationSolveResult? LastResult{get;private set;}
 public uint SetInputPath(int side,string path){
  if(side is not (0 or 1))throw new ArgumentOutOfRangeException(nameof(side));
  MapCalibrationFileRead read;try{read=MapCalibrationFiles.Read(Path.GetFullPath(path,baseDirectory));}catch(Exception error)when(error is ArgumentException or NotSupportedException){return 0xe0000201;}
  if(read.Replacement is {} replacement)matrices[side]=replacement;
  if(read.ErrorCode==0)paths[side]=path;
  return read.ErrorCode;
 }
 public uint SetOutputPath(string path){if(string.IsNullOrEmpty(path))return 0xe0000204;OutputPath=path;return 0;}
 public MapCalibrationSolveResult Run(bool refreshInputs=false){
  CalibrationValid=false;
  // Original refresh returns on the first setter failure before SDK Run; its old result remains available.
  if(refreshInputs)for(int side=0;side<2;side++){uint code=SetInputPath(side,paths[side]);if(code!=0)return new(code,[],0,0);}
  LastResult=null;
  if(matrices[0] is null||matrices[1] is null)return new(0x80100006,[],0,0);
  var result=MapCalibrationSolver.SolveFiles(matrices[0]!,matrices[1]!);
  if(result.ErrorCode!=0)return result;
  LastResult=result;CalibrationValid=true;
  if(RefreshOutput&&OutputPath.Length>0)MapCalibrationFiles.SaveFileMode(Path.GetFullPath(OutputPath,baseDirectory),result.Matrix);
  return result;
 }
 public MapCalibrationSave Save(string path){
  if(!path.EndsWith(".xml",StringComparison.Ordinal))return new(0xe0000204,false);
  if(LastResult is null)return new(0x80100203,false);
  var saved=MapCalibrationFiles.SaveFileModeOperation(Path.GetFullPath(path,baseDirectory),LastResult.Matrix);
  if(saved.ErrorCode==0)OutputPath=path;
  return saved;
 }
}

