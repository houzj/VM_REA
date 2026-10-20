using System.IO;
namespace VisionPlugins.CameraMapping;
public static class CameraMappingFile {
 public static double[] Read(string path){using var stream=File.OpenRead(path);if(stream.Length!=36)throw new InvalidDataException("标定文件必须包含9个小端单精度系数。");using var reader=new BinaryReader(stream);var h=Enumerable.Range(0,9).Select(_=>(double)reader.ReadSingle()).ToArray();if(h.Any(x=>!double.IsFinite(x)))throw new InvalidDataException("标定文件包含非有限数值。");return h;}
 public static void Write(string path,IReadOnlyList<double> h){if(h.Count!=9||h.Any(x=>!double.IsFinite(x)||!float.IsFinite((float)x)))throw new ArgumentException("标定矩阵无效。",nameof(h));using var stream=File.Create(path);using var writer=new BinaryWriter(stream);foreach(var value in h)writer.Write((float)value);}
}

public sealed class CameraMappingOperationException(uint nativeErrorCode,string message):InvalidOperationException(message+" (0x"+nativeErrorCode.ToString("x8")+")"){public uint NativeErrorCode{get;}=nativeErrorCode;}
