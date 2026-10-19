using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;
using System.Text.Json;

int checks=0;
void Check(bool value,string label){if(!value)throw new Exception(label);checks++;Console.WriteLine("PASS "+label);}
JsonElement Inspect(byte[] value,string key="ProceZoomIndex")
{
    using var bytes=new MemoryStream();
    using(var zip=new ZipArchive(bytes,ZipArchiveMode.Create,true))
    using(var writer=new BinaryWriter(zip.CreateEntry("SolutionFile/UiParamData/test").Open()))
    {
        void Int(int value){Span<byte> b=stackalloc byte[4];BinaryPrimitives.WriteInt32BigEndian(b,value);writer.Write(b);}
        Int(0x66553322);Int(1);Int(1);byte[] name=Encoding.UTF8.GetBytes(key);Int(name.Length);writer.Write(name);Int(value.Length);writer.Write(value);
    }
    bytes.Position=0;using var read=new ZipArchive(bytes,ZipArchiveMode.Read);
    return JsonSerializer.SerializeToElement(UiParameters.Inspect(read.Entries[0])).GetProperty("records")[0];
}
foreach(var text in new[]{"0","9","19","-1","20","2147483647"," 12 "})
{
    byte[] raw=Encoding.UTF8.GetBytes(text+"\0\0");var row=Inspect(raw);var decoded=row.GetProperty("decoded");
    Check(decoded.GetProperty("status").GetString()=="decoded-integer"&&decoded.GetProperty("index").GetInt32()==int.Parse(text),"Source integer including non-UI range: "+text);
    Check(Convert.FromBase64String(row.GetProperty("rawBase64").GetString()!).SequenceEqual(raw),"Raw payload retained: "+text);
}
foreach(var text in new[]{"","bad","2147483648","9\0x"})
    Check(Inspect(Encoding.UTF8.GetBytes(text)).GetProperty("decoded").GetProperty("status").GetString()=="invalid-integer","Malformed value not replaced by default9: "+text);
Check(Inspect([0xff]).GetProperty("decoded").GetProperty("status").GetString()=="invalid-utf8","Inspector strict UTF8 policy reported");
Check(Inspect([1,2,3],"UnknownKey").GetProperty("decoded").ValueKind==JsonValueKind.Null,"Unknown key stays opaque");
Console.WriteLine($"PASS {checks}/{checks}; observed envelope and source-guided zoom key, no SDK or native format parity");
