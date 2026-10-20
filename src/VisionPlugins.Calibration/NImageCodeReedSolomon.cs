namespace VisionPlugins.Calibration;

/// <summary>Fixed 24-symbol source translation of original 8b0260 and its GF/polynomial callees.
/// This is the tag codeword layer, not an image detector.</summary>
public static class NImageCodeReedSolomon {
 public static bool TryCorrect(ReadOnlySpan<byte> symbols,out byte[] corrected){
  var result=NImageSourceCodeword24.Correct(symbols);
  corrected=result.Symbols;
  return result.Success;
 }
 public static bool TryDecodeBits(ReadOnlySpan<byte> bits,out byte[] exportedBytes){exportedBytes=[];if(bits.Length!=192)return false;byte[] symbols=new byte[24];for(int i=0;i<192;i++){if(bits[i]>1)return false;symbols[i/8]|=(byte)(bits[i]<<(7-i%8));}if(!TryCorrect(symbols,out var corrected))return false;exportedBytes=Enumerable.Range(0,12).Select(i=>corrected[23-i]).ToArray();return true;}
}
