using System.Buffers.Binary;
using System.IO;
namespace VisionPlugins.CalibrationBoard;

public sealed class CalibrationBoardFileFailure(uint nativeErrorCode,string message):IOException(message){
 public uint NativeErrorCode{get;}=nativeErrorCode;
}

/// <summary>36-byte little-endian float32 matrix verified against original SDK ExportCalibFile fixtures.</summary>
public static class CalibrationBoardFile {
 public static byte[] Encode(IReadOnlyList<double> matrix){if(matrix.Count!=9||matrix.Any(v=>!double.IsFinite(v)||!float.IsFinite((float)v)))throw new InvalidDataException("Finite 3x3 float matrix required.");var bytes=new byte[36];for(int i=0;i<9;i++)BinaryPrimitives.WriteSingleLittleEndian(bytes.AsSpan(i*4,4),(float)matrix[i]);return bytes;}
 public static double[] Decode(ReadOnlySpan<byte> bytes){if(bytes.Length!=36)throw new InvalidDataException("Invalid .iwcal length.");var matrix=new double[9];for(int i=0;i<9;i++){matrix[i]=BinaryPrimitives.ReadSingleLittleEndian(bytes.Slice(i*4,4));if(!double.IsFinite(matrix[i]))throw new InvalidDataException("Invalid .iwcal coefficient.");}return matrix;}
 public static void Write(string path,IReadOnlyList<double> matrix){if(!Path.GetExtension(path).Equals(".iwcal",StringComparison.OrdinalIgnoreCase))throw new InvalidDataException("Calibration file must use .iwcal.");var directory=Path.GetDirectoryName(Path.GetFullPath(path));if(directory is not null)Directory.CreateDirectory(directory);File.WriteAllBytes(path,Encode(matrix));}
 // Explicit SaveCalibPath calls SDK Export directly; it does not execute the
 // CalibPathName setter's CreateMultipleDirectory preparation.
 public static void WriteExplicit(string path,IReadOnlyList<double> matrix){
  if(!Path.GetExtension(path).Equals(".iwcal",StringComparison.Ordinal))throw new CalibrationBoardFileFailure(0xe0000204,"标定文件必须使用 .iwcal（0xe0000204）。");
  try{File.WriteAllBytes(path,Encode(matrix));}catch(Exception error)when(error is IOException or UnauthorizedAccessException){throw new CalibrationBoardFileFailure(0x80100010,"无法打开标定文件（0x80100010）："+error.Message);}
 }
}

