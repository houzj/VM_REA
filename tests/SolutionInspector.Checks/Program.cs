using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
int passed = 0;
void Check(string name, Action run) { run(); Console.WriteLine("PASS " + name); passed++; }
void Require(bool value) { if (!value) throw new Exception("Assertion failed"); }
byte[] Ints(params int[] values) { byte[] b = new byte[values.Length*4]; for(int i=0;i<values.Length;i++) BinaryPrimitives.WriteInt32BigEndian(b.AsSpan(i*4),values[i]); return b; }
byte[] Join(params byte[][] pieces) => pieces.SelectMany(x=>x).ToArray();
JsonElement Inspect(byte[] bytes, bool ui = false)
{
    using var ms = new MemoryStream();
    using (var zip = new ZipArchive(ms,ZipArchiveMode.Create,true)) { using var s=zip.CreateEntry("test").Open(); s.Write(bytes); }
    ms.Position=0; using var read = new ZipArchive(ms,ZipArchiveMode.Read);
    return JsonSerializer.SerializeToElement(ui ? UiParameters.Inspect(read.Entries[0]) : ModuleFrame.Inspect(read.Entries[0]));
}
void Status(byte[] b, string status, bool ui=false) => Require(Inspect(b,ui).GetProperty("status").GetString()==status);
byte[] Name(string name) { var b=new byte[256]; Encoding.Unicode.GetBytes(name).CopyTo(b,0); return b; }
Check("frame unknown version",()=>Status(Ints(6,0),"unsupported"));
Check("frame truncated header",()=>Status(Ints(5),"unsupported"));
Check("frame huge count",()=>Status(Ints(5,10001),"unsupported"));
Check("frame negative count",()=>Status(Ints(5,-1),"unsupported"));
Check("frame truncated length",()=>Status(Join(Ints(5,1),Name("test"),new byte[]{0,0}),"unsupported"));
Check("frame oversized length",()=>Status(Join(Ints(5,1),Name("test"),Ints(100)),"unsupported"));
Check("frame negative length",()=>Status(Join(Ints(5,1),Name("test"),Ints(-1)),"unsupported"));
Check("frame exact consumption",()=>{ var j=Inspect(Join(Ints(5,1),Name("test"),Ints(3),new byte[]{1,2,3})); Require(j.GetProperty("status").GetString()=="parsed-container"); Require(j.GetProperty("trailingBytes").GetInt32()==0); Require(j.GetProperty("records")[0].GetProperty("bytes").GetInt32()==3); });
Check("frame detects tail",()=>Status(Join(Ints(5,0),new byte[]{0}),"partial-container"));
Check("frame preserves full UTF16 name slot",()=>{
    var slot=Name("a\0b");var j=Inspect(Join(Ints(5,1),slot,Ints(0)));
    Require(Convert.FromBase64String(j.GetProperty("records")[0].GetProperty("nameSlotBase64").GetString()!).SequenceEqual(slot));
});
Check("frame preserves full parameter key slot",()=>{
    byte[] key=new byte[256];Encoding.UTF8.GetBytes("Gain").CopyTo(key,0);
    byte[] payload=Join(Ints(1),key,Ints(0),new byte[1024]);
    var j=Inspect(Join(Ints(5,1),Name("x.mdata"),Ints(payload.Length),payload));
    var item=j.GetProperty("records")[0].GetProperty("parameters").GetProperty("items")[0];
    Require(item.GetProperty("key").GetString()=="Gain");
    Require(Convert.FromBase64String(item.GetProperty("keySlotBase64").GetString()!).SequenceEqual(key));
});
Check("ui unknown version",()=>Status(Ints(0x66553322,2,0),"unsupported",true));
Check("ui truncated header",()=>Status(Ints(0x66553322,1),"unsupported",true));
Check("ui huge count",()=>Status(Ints(0x66553322,1,10001),"unsupported",true));
Check("ui negative length",()=>Status(Ints(0x66553322,1,1,-1),"unsupported",true));
Check("ui missing value",()=>Status(Join(Ints(0x66553322,1,1,1),new byte[]{65}),"unsupported",true));
Check("ui truncated value",()=>Status(Join(Ints(0x66553322,1,1,1),new byte[]{65},Ints(8),new byte[]{0}),"unsupported",true));
Check("ui observed position",()=>{
    byte[] value=new byte[16]; BinaryPrimitives.WriteInt64LittleEndian(value,BitConverter.DoubleToInt64Bits(145)); BinaryPrimitives.WriteInt64LittleEndian(value.AsSpan(8),BitConverter.DoubleToInt64Bits(85));
    var j=Inspect(Join(Ints(0x66553322,1,1,8),Encoding.UTF8.GetBytes("Position"),Ints(16),value),true);
    Require(j.GetProperty("status").GetString()=="parsed"); Require(j.GetProperty("trailingBytes").GetInt32()==0);
    var p=j.GetProperty("records")[0].GetProperty("decoded"); Require(p.GetProperty("x").GetDouble()==145 && p.GetProperty("y").GetDouble()==85);
});
string sample=args.Length>0 ? args[0] : @"D:\Program Files\VisionMaster4.4.0\Development\V4.x\Samples\C#\PlatformSDKSampleCS\CircleFind\CircleFind.sol";
Check("real CircleFind frame and parameters",()=>{
    using var zip=ZipFile.OpenRead(sample); var entry=zip.Entries.Single(e=>e.FullName.EndsWith("MoudleFrame",StringComparison.Ordinal));
    Require(entry.Length==90696); var j=JsonSerializer.SerializeToElement(ModuleFrame.Inspect(entry));
    Require(j.GetProperty("status").GetString()=="parsed-container" && j.GetProperty("trailingBytes").GetInt32()==0 && j.GetProperty("count").GetInt32()==7);
    var record=j.GetProperty("records").EnumerateArray().Single(r=>r.GetProperty("name").GetString()!.Contains("CircleFind") && r.GetProperty("name").GetString()!.EndsWith(".mdata"));
    var table=record.GetProperty("parameters"); Require(table.GetProperty("status").GetString()=="parsed-fixed-table");
    var fields=table.GetProperty("items").EnumerateArray().ToDictionary(x=>x.GetProperty("key").GetString()!,x=>x.GetProperty("text").GetString());
    Require(fields["EdgeWidth"]=="1" && fields["RadNum"]=="30");
});
Console.WriteLine($"{passed} parser semantic checks passed. These verify observed structure, not vendor algorithm equivalence.");
