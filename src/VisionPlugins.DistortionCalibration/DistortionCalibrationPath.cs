using System.IO;
namespace VisionPlugins.DistortionCalibration;
/// <summary>Module SetParam(CalibPathName), 180021560: last backslash, one-character
/// drive prefix split by ':', then CreateMultipleDirectory before storing the path.</summary>
public static class DistortionCalibrationPath {
 public static void Prepare(string path){
  if(string.IsNullOrEmpty(path))throw new InvalidDataException("SetParam(CalibPathName): 0xe0000204 empty path.");
  int slash=path.LastIndexOf('\\');if(slash<0)throw Invalid();string directory=path[..slash];string[] driveParts=directory.Split(':');if(driveParts.Length!=2||driveParts[0].Length!=1)throw Invalid();
  try{Directory.CreateDirectory(directory);}catch(Exception error)when(error is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException){throw new InvalidDataException("SetParam(CalibPathName): 0xe0000200 directory creation failed.",error);}
 }
 static InvalidDataException Invalid()=>new("SetParam(CalibPathName): 0xe0000200 invalid directory.");
}
