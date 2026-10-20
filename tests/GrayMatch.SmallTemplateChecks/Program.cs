using System.Text.Json;
using VisionPlugins.GrayMatch;
if(args.Length!=1)throw new ArgumentException("Exact trainer-native oracle.json required.");
using var doc=JsonDocument.Parse(File.ReadAllText(args[0]));var root=doc.RootElement;
if(root.GetProperty("return").GetString()!="00000000"||!root.GetProperty("modelGuard").GetBoolean()||!root.GetProperty("workGuard").GetBoolean())throw new Exception("Original generator fixture invalid.");
byte[] model=Convert.FromHexString(root.GetProperty("modelBytesHex").GetString()!);
int n=0;void Check(bool ok,string name){if(!ok)throw new Exception(name);Console.WriteLine($"PASS {++n}: {name}");}
int I(int p)=>BitConverter.ToInt32(model,p);long Q(int p)=>BitConverter.ToInt64(model,p);
byte[] input=new byte[256],mask=Enumerable.Repeat((byte)1,256).ToArray();for(int y=0;y<16;y++)for(int x=0;x<16;x++)input[y*16+x]=(byte)((13*x+19*y+7)%181);
var result=GrayMatchSmallTemplate.Sample(input,16,mask,16,16,16,16,16,1,0);
int header=checked((int)Q(0x16b8)),selected=checked((int)Q(0x38)),count=I(header+0x28),records=checked((int)Q(header+8));
Check(result.Selected.Length==count,"original selected pixel count");
for(int i=0;i<count;i++)if(result.Selected[i]!=model[selected+i])throw new Exception($"First selected byte divergence index{i}: {result.Selected[i]} != {model[selected+i]}");
Check(true,"all256 original cubic selected bytes strict");
Check(BitConverter.DoubleToInt64Bits(result.Sum)==Q(header+0x18),"original signed-span to double total sum bits");
Check(BitConverter.DoubleToInt64Bits(result.Squares)==Q(header+0x20),"original signed-span to double square total bits");
Check(BitConverter.SingleToInt32Bits(result.ShiftX)==I(header+0x10)&&BitConverter.SingleToInt32Bits(result.ShiftY)==I(header+0x14),"original crop shift float bits");
Check(result.Spans.Count==I(header),"original primary span count");
for(int i=0;i<result.Spans.Count;i++)
{
 var s=result.Spans[i];int p=records+i*24;
 Check(new[]{s.Row,s.Start,s.End,s.Offset,s.Sum,s.Squares}.SequenceEqual(Enumerable.Range(0,6).Select(j=>I(p+j*4))),$"original primary six-int span{i}");
}
int auxiliary=checked((int)Q(header+0x30)),ar=checked((int)Q(auxiliary+8));
Check(result.AuxiliaryMask.Count==I(auxiliary),"original groupCount1 auxiliary mask span count");
for(int i=0;i<result.AuxiliaryMask.Count;i++)
{
 var s=result.AuxiliaryMask[i];Check(new[]{s.Row,s.Start,s.End}.SequenceEqual(Enumerable.Range(0,3).Select(j=>I(ar+i*24+j*4))),$"original auxiliary three defined int fields{i}");
}
Check(!result.Selected.SequenceEqual(input),"angle0 original cubic path differs from copying source pixels");
Console.WriteLine($"PASS {n}; one original small-template fixed caller, not full generator/search parity");
