namespace VisionPlugins.Calibration;

/// <summary>Original explicit Save return transported across the managed editor boundary.</summary>
public sealed class NImageExportException(uint nativeErrorCode,string message):InvalidOperationException(message){
 public uint NativeErrorCode{get;}=nativeErrorCode;
}
