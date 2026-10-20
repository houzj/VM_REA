using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using VisionCore;

static class RectangleRoiMaskChecks
{
 public static void Run(Action<bool,string> check)
 {
  using var fixture=JsonDocument.Parse(File.ReadAllText("docs/plugins/calibration-board-roi-mask-fixtures.json"));var root=fixture.RootElement;int w=root.GetProperty("imageWidth").GetInt32(),h=root.GetProperty("imageHeight").GetInt32();
  foreach(var row in root.GetProperty("rows").EnumerateArray())
  {
   int index=row.GetProperty("index").GetInt32();var mask=RectangleRoiMaskRasterizer.Create(w,h,row.GetProperty("cx").GetSingle(),row.GetProperty("cy").GetSingle(),row.GetProperty("width").GetSingle(),row.GetProperty("height").GetSingle(),row.GetProperty("angle").GetSingle());
   check(new[]{mask.Left,mask.Top,mask.Width,mask.Height}.SequenceEqual(row.GetProperty("bounds").EnumerateArray().Select(e=>e.GetInt32())),"native PreproMask ROI bounds "+index);
   using var compressed=new MemoryStream(Convert.FromBase64String(row.GetProperty("zlibBase64").GetString()!));using var zlib=new ZLibStream(compressed,CompressionMode.Decompress);using var pixels=new MemoryStream();zlib.CopyTo(pixels);byte[] expected=pixels.ToArray();
   check(mask.Pixels.SequenceEqual(expected),"native PreproMask complete pixel mask "+index);check(Convert.ToHexString(SHA256.HashData(mask.Pixels)).Equals(row.GetProperty("pixelSha256").GetString(),StringComparison.OrdinalIgnoreCase),"native PreproMask pixel digest "+index);
  }
 }
}
