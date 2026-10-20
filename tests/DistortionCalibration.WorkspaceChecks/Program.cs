using System.Text.Json;
// Source 7fec40 inserts without an index>=0 guard. Replay inside a bounded array;
// crossing the captured prefix is an explicit failure, never an unsafe memory read.
static float Replay(float[] errors,float[] prefix){
 var buffer=prefix.Concat(errors.Select(MathF.Abs)).ToArray();int offset=prefix.Length;
 for(int i=0;i<errors.Length-1;i++){
  float value=buffer[offset+i+1];if(!(value<buffer[offset+i]))continue;int position=offset+i;
  do{buffer[position+1]=buffer[position];position--;if(position<0)throw new InvalidDataException("Native sort crossed the captured workspace boundary.");}while(value<buffer[position]);
  buffer[position+1]=value;
 }
 return buffer[offset+errors.Length/2]*1.4825797080993652f;
}
try{
using var evidence=JsonDocument.Parse(File.ReadAllText("docs/plugins/distortion-calibration-workspace-evidence.json"));int checks=0;
foreach(var row in evidence.RootElement.GetProperty("cases").EnumerateArray()){
 byte[] entry=Convert.FromBase64String(row.GetProperty("entryBase64").GetString()!);int count=BitConverter.ToInt32(entry,0),prefixCount=BitConverter.ToInt32(entry,8),prefixOffset=12+32+count*16;
 byte[] rawErrors=Convert.FromBase64String(row.GetProperty("errorsBase64").GetString()!);if(entry.Length!=prefixOffset+(prefixCount+count)*4||rawErrors.Length!=count*4)throw new Exception("Fixture layout differs.");
 var errors=Enumerable.Range(0,count).Select(i=>BitConverter.ToSingle(rawErrors,i*4)).ToArray();var prefix=Enumerable.Range(0,prefixCount).Select(i=>BitConverter.ToSingle(entry,prefixOffset+i*4)).ToArray();
 foreach(bool captured in new[]{false,true}){float expected=row.GetProperty(captured?"capturedContextSigma":"zeroContextSigma").GetSingle(),actual=Replay(errors,captured?prefix:new float[prefixCount]);if(BitConverter.SingleToInt32Bits(expected)!=BitConverter.SingleToInt32Bits(actual))throw new Exception($"Workspace sigma differs at {row.GetProperty("sequence")} captured={captured}: {actual:R}/{expected:R}.");checks++;}
}
Console.WriteLine($"Workspace source replay: {checks} original median results bit-exact; bounded prefix access only. This is not a production scratch-lifetime implementation.");

return 0;
}catch(Exception error){Console.Error.WriteLine(error);return 1;}
