using System.Buffers.Binary;
using VisionIO;
int passed = 0;
void Check(string name, Action body) { body(); Console.WriteLine("PASS " + name); passed++; }
void Equal(byte[] bmp, params byte[] expected) { using var s = new MemoryStream(bmp); var image = BmpReader.Read(s); if (!image.Pixels.SequenceEqual(expected)) throw new Exception("Wrong pixels"); if (!s.CanRead) throw new Exception("Stream closed"); }
void Reject(byte[] bmp) { try { BmpReader.Read(new MemoryStream(bmp)); } catch (InvalidDataException) { return; } throw new Exception("Invalid BMP accepted"); }
byte[] Make(int w, int h, ushort bits, byte[] rows, byte[]? palette = null)
{
    int offset = 54 + (palette?.Length ?? 0);
    var b = new byte[offset + rows.Length]; b[0] = (byte)'B'; b[1] = (byte)'M';
    void U(int p, uint v) => BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(p), v);
    U(2, (uint)b.Length); U(10, (uint)offset); U(14, 40); U(18, (uint)w); U(22, unchecked((uint)h));
    BinaryPrimitives.WriteUInt16LittleEndian(b.AsSpan(26), 1); BinaryPrimitives.WriteUInt16LittleEndian(b.AsSpan(28), bits);
    U(34, (uint)rows.Length); U(46, (uint)((palette?.Length ?? 0)/4));
    palette?.CopyTo(b, 54); rows.CopyTo(b, offset); return b;
}
Check("24bit padding and bottom-up", () => Equal(Make(1, 2, 24, [255,0,0,99, 0,0,255,88]), 76,29));
Check("24bit top-down", () => Equal(Make(1, -2, 24, [255,0,0,99, 0,255,0,88]), 29,150));
Check("8bit color palette", () => Equal(Make(2, -1, 8, [0,1,99,99], [0,0,255,0, 0,255,0,0]), 76,150));
Check("32bit ignores alpha", () => Equal(Make(2,1,32,[255,255,255,0, 0,0,0,255]),255,0));
Check("truncated pixels", () => Reject(Make(1,1,24,[1,2,3,0])[..^1]));
Check("missing palette", () => Reject(Make(1,1,8,[0,0,0,0])));
Check("missing palette entry", () => Reject(Make(1,1,8,[1,0,0,0],[0,0,0,0])));
Check("compression rejected", () => { var b=Make(1,1,24,[0,0,0,0]); b[30]=1; Reject(b); });
Check("bitfields rejected", () => { var b=Make(1,1,32,[0,0,0,0]); b[30]=3; Reject(b); });
Check("zero dimensions", () => Reject(Make(0,1,24,[0,0,0,0])));
Check("resource limit", () => Reject(Make(100000,100000,24,[0,0,0,0])));
Check("bad signature", () => { var b=Make(1,1,24,[0,0,0,0]); b[0]=0; Reject(b); });
Check("header overlap", () => { var b=Make(1,1,24,[0,0,0,0]); b[10]=40; Reject(b); });
Check("negative extreme height", () => Reject(Make(1,int.MinValue,24,[0,0,0,0])));
Console.WriteLine($"{passed} semantic checks passed.");
