using System.Buffers.Binary;
using System.IO;
namespace VisionPlugins.DistortionCalibration;

/// <summary>Observed SDK .iccal payload: exactly nineteen little-endian IEEE float values.
/// This is independent validation/serialization, not a claim about every historic file version.</summary>
public sealed class DistortionCalibrationFile {
 readonly float[] values;
 public IReadOnlyList<float> Values=>Array.AsReadOnly(values);
 public int Model=>(int)values[18];
 public int Width=>(int)values[16];
 public int Height=>(int)values[17];
 public DistortionCalibrationFile(IEnumerable<float> matrix){
  values=matrix.ToArray();
  if(values.Length!=19||values.Any(v=>!float.IsFinite(v)))throw new InvalidDataException("畸变标定数据必须为19个有限float。");
  if(values[0]!=2)throw new InvalidDataException("未支持的畸变标定数据头。");
  if(values[16]<1||values[17]<1||values[16]!=MathF.Truncate(values[16])||values[17]!=MathF.Truncate(values[17])||values[16]>=2147483648f||values[17]>=2147483648f)throw new InvalidDataException("畸变标定图像尺寸无效。");
  if(values[18] is not (1 or 2 or 3))throw new InvalidDataException("未支持的畸变模型。");
 }
 public static DistortionCalibrationFile Read(ReadOnlySpan<byte> bytes){if(bytes.Length!=76)throw new InvalidDataException("当前.iccal格式必须为76字节。");var matrix=new float[19];for(int i=0;i<19;i++)matrix[i]=BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32LittleEndian(bytes.Slice(i*4,4)));return new(matrix);}
 public byte[] Bytes(){var bytes=new byte[76];for(int i=0;i<19;i++)BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(i*4,4),BitConverter.SingleToInt32Bits(values[i]));return bytes;}
 public void Write(string path)=>File.WriteAllBytes(path,Bytes());
}
