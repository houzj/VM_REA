using System.IO;
namespace VisionPlugins.DistortionCalibration;

public sealed class DistortionCalibrationFileFailure(uint nativeErrorCode,string message):IOException(message){
 public uint NativeErrorCode{get;}=nativeErrorCode;
}

/// <summary>Observed module export policy, separate from the nineteen-float codec.
/// SetParam's directory preparation remains DistortionCalibrationPath.Prepare.</summary>
public static class DistortionCalibrationFileLifecycle{
 public static bool WriteAutomatic(DistortionCalibrationFile result,string path,bool refresh){
  // Module helper 1800020c0 uses _waccess(path,0), which also accepts directories.
  if(path.Length==0||(File.Exists(path)||Directory.Exists(path))&&!refresh)return false;
  // SDK export rejects uppercase and absent extensions; ProcessAlg maps its exception
  // to 8010000e. This differs from the explicit SaveCalibPath setter's e0000204.
  if(!ValidExtension(path))throw new DistortionCalibrationFileFailure(0x8010000e,"自动标定文件路径的扩展名必须为 .iccal（0x8010000e）。");
  WritePayload(result,path);return true;
 }
 public static void WriteExplicit(DistortionCalibrationFile result,string path){
  ValidateExplicitPath(path);
  WritePayload(result,path);
 }
 static void WritePayload(DistortionCalibrationFile result,string path){
  // SDK 180003b20 reports 80100010 when fopen/_wfopen cannot open the target.
  // Managed write/close failures are also reported safely; the SDK does not
  // inspect fwrite/fflush/fclose results, so that broader check is independent.
  try{result.Write(path);}catch(Exception error)when(error is IOException or UnauthorizedAccessException){throw new DistortionCalibrationFileFailure(0x80100010,"无法写入标定文件（0x80100010）："+error.Message);}
 }
 public static void ValidateExplicitPath(string path){if(!ValidExtension(path))throw new DistortionCalibrationFileFailure(0xe0000204,"标定文件路径的扩展名必须为 .iccal（0xe0000204）。");}
 static bool ValidExtension(string path)=>Path.GetExtension(path).Equals(".iccal",StringComparison.Ordinal);
}
