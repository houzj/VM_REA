using System.IO;
using System.IO.Compression;
using System.Text.Json;
using VisionPlugins.CalibrationBoard;

static class CircleEdgeChecks
{
    public static void Run(Action<bool,string> check)
    {
        using var document=JsonDocument.Parse(File.ReadAllText("docs/plugins/calibration-board-edge-fixtures.json"));
        byte[] Decode(JsonElement blob){using var input=new MemoryStream(Convert.FromBase64String(blob.GetProperty("gzipBase64").GetString()!));using var gzip=new GZipStream(input,CompressionMode.Decompress);using var output=new MemoryStream();gzip.CopyTo(output);return output.ToArray();}
        foreach(var fixture in document.RootElement.GetProperty("gradients").EnumerateArray())
        {
            int shape=fixture.GetProperty("shape").GetInt32(),contrast=fixture.GetProperty("contrast").GetInt32();
            var pixels=Enumerable.Repeat((byte)255,1024).ToArray();
            for(int dy=-9;dy<=9;dy++)for(int dx=-9;dx<=9;dx++)
            {bool inside=shape==0?dx*dx+dy*dy<81:shape==1?dx*dx/81f+dy*dy/25f<1:Math.Abs(dx)<9&&Math.Abs(dy)<9;if(inside)pixels[(16+dy)*32+16+dx]=(byte)(255-contrast);}
            var actual=CalibrationBoardCircleEdges.Gradients(new(32,32,pixels));
            foreach(var (name,values) in new[]{("gx",actual.X),("gy",actual.Y)})
            {
                var bytes=Decode(fixture.GetProperty(name));var expected=new short[bytes.Length/2];Buffer.BlockCopy(bytes,0,expected,0,bytes.Length);
                check(values.SequenceEqual(expected),$"native signed Sobel {name}, shape {shape}, contrast {contrast}, every pixel");
            }
        }
        foreach(var fixture in document.RootElement.GetProperty("rows").EnumerateArray())
        {
            if(!fixture.TryGetProperty("stage",out var stage)||stage.GetInt32()==0)
            {
                int shape=fixture.GetProperty("shape").GetInt32(),contrast=fixture.GetProperty("contrast").GetInt32();var pixels=Enumerable.Repeat((byte)255,1024).ToArray();
                for(int dy=-9;dy<=9;dy++)for(int dx=-9;dx<=9;dx++){bool inside=shape==0?dx*dx+dy*dy<81:shape==1?dx*dx/81f+dy*dy/25f<1:Math.Abs(dx)<9&&Math.Abs(dy)<9;if(inside)pixels[(16+dy)*32+16+dx]=(byte)(255-contrast);}
                var actual=CalibrationBoardCircleEdges.Contours(new(32,32,pixels),fixture.GetProperty("low").GetInt32(),fixture.GetProperty("high").GetInt32());
                var nativeContours=fixture.GetProperty("contours").EnumerateArray().ToArray();
                bool same=actual.Count==nativeContours.Length;
                if(same)for(int i=0;i<actual.Count;i++){var native=nativeContours[i];same&=actual[i].Points.SequenceEqual(native.GetProperty("points").EnumerateArray().Select(p=>(p[0].GetSingle(),p[1].GetSingle())))&&actual[i].Length==native.GetProperty("features")[1].GetSingle()&&actual[i].Strength==native.GetProperty("features")[2].GetSingle();}
                check(same,$"native full manual contours shape {shape}, contrast {contrast}, thresholds {fixture.GetProperty("low")}/{fixture.GetProperty("high")}");
            }
            if(fixture.GetProperty("circleCode").GetInt32()!=1)continue;
            var contours=fixture.GetProperty("contours").EnumerateArray().ToArray();
            var selected=contours.OrderByDescending(c=>c.GetProperty("features")[1].GetSingle()).First();
            var points=selected.GetProperty("points").EnumerateArray().Select(p=>(p[0].GetSingle(),p[1].GetSingle())).ToArray();
            bool success=CalibrationBoardCircleEdges.FitCircleContour(points,out var center);
            var expected=fixture.GetProperty("center");
            check(success&&center.X==expected[0].GetSingle()&&center.Y==expected[1].GetSingle(),$"native circle float contour fit shape {fixture.GetProperty("shape")}, contrast {fixture.GetProperty("contrast")}, thresholds {fixture.GetProperty("low")}/{fixture.GetProperty("high")}: {center}");
        }
        using var nms=JsonDocument.Parse(File.ReadAllText("docs/plugins/calibration-board-nms-fixtures.json"));
        foreach(var fixture in nms.RootElement.GetProperty("rows").EnumerateArray())
        {
            int shape=fixture.GetProperty("shape").GetInt32(),contrast=fixture.GetProperty("contrast").GetInt32(),low=fixture.GetProperty("low").GetInt32(),high=fixture.GetProperty("high").GetInt32();
            var pixels=Enumerable.Repeat((byte)255,1024).ToArray();
            for(int dy=-9;dy<=9;dy++)for(int dx=-9;dx<=9;dx++)
            {bool inside=shape==0?dx*dx+dy*dy<81:shape==1?dx*dx/81f+dy*dy/25f<1:Math.Abs(dx)<9&&Math.Abs(dy)<9;if(inside)pixels[(16+dy)*32+16+dx]=(byte)(255-contrast);}
            var expected=fixture.GetProperty("points").EnumerateArray().Select(p=>(p[0].GetSingle(),p[1].GetSingle())).ToArray();
            var state=CalibrationBoardCircleEdges.NonMaximumState(new(32,32,pixels),low,high);var actual=state.Points;
            check(actual.SequenceEqual(expected),$"native NMS exact ordered float points shape {shape}, contrast {contrast}, thresholds {low}/{high}: actual {actual.Count}, expected {expected.Length}");
            var links=CalibrationBoardCircleEdges.LinkPixels(new(32,32,pixels),expected,state.PixelIndex);
            check(links.Previous.SequenceEqual(fixture.GetProperty("previousPixels").EnumerateArray().Select(v=>v.GetInt32()))&&links.Next.SequenceEqual(fixture.GetProperty("nextPixels").EnumerateArray().Select(v=>v.GetInt32())),$"native contour link priority shape {shape}, contrast {contrast}, thresholds {low}/{high}");
        }
    }
}
