using System.IO;
using System.IO.Compression;
using System.Text.Json;
using VisionCore;
using VisionPlugins.CalibrationBoard;

static class PreprocessChecks
{
    public static void Run(Action<bool,string> check)
    {
        using var document=JsonDocument.Parse(File.ReadAllText("docs/plugins/calibration-board-preprocess-fixtures.json"));
        var differences=new List<object>();var assertions=new List<(bool Passed,string Message)>();
        byte[] Decode(JsonElement blob){using var input=new MemoryStream(Convert.FromBase64String(blob.GetProperty("gzipBase64").GetString()!));using var compressed=new GZipStream(input,CompressionMode.Decompress);using var output=new MemoryStream();compressed.CopyTo(output);return output.ToArray();}
        foreach(var fixture in document.RootElement.GetProperty("fixtures").EnumerateArray())
        {
            bool shape=fixture.TryGetProperty("shape",out var shapeValue);int width=shape?480:fixture.GetProperty("width").GetInt32(),height=shape?400:fixture.GetProperty("height").GetInt32();
            byte[] input;
            string name;
            if(shape)
            {
                int kind=shapeValue.GetInt32(),contrast=fixture.GetProperty("contrast").GetInt32();name=$"shape{kind}/contrast{contrast}";input=Enumerable.Repeat((byte)255,width*height).ToArray();
                for(int row=0;row<8;row++)for(int col=0;col<10;col++)for(int dy=-9;dy<=9;dy++)for(int dx=-9;dx<=9;dx++)
                {bool inside=kind==0?dx*dx+dy*dy<81:kind==1?dx*dx/81f+dy*dy/25f<1:Math.Abs(dx)<9&&Math.Abs(dy)<9;if(inside)input[(80+row*30+dy)*width+90+col*30+dx]=(byte)(255-contrast);}
            }
            else{name=$"pattern{fixture.GetProperty("pattern").GetInt32()}/{width}x{height}";input=Decode(fixture.GetProperty("input"));}
            int factor=fixture.TryGetProperty("factor",out var factorValue)?factorValue.GetInt32():2;
            name+=$"/factor{factor}";
            var expectedHalf=Decode(fixture.GetProperty("half"));var actualHalf=CalibrationBoardPreprocess.Resize(new(width,height,input),width/factor,height/factor).Pixels.ToArray();
            int halfDifferences=actualHalf.Zip(expectedHalf).Count(pair=>pair.First!=pair.Second);
            assertions.Add((halfDifferences==0,$"native resize every pixel {name}: {halfDifferences} differences"));
            int maskDifferences=0,threshold=-1;
            if(shape)
            {
                var nativeHalf=new GrayImage(width/factor,height/factor,expectedHalf);threshold=CalibrationBoardPreprocess.Otsu(nativeHalf);var expectedMask=Decode(fixture.GetProperty("mask"));
                var actualMask=expectedHalf.Select(value=>value<threshold?(byte)0:(byte)255).ToArray();maskDifferences=actualMask.Zip(expectedMask).Count(pair=>pair.First!=pair.Second);
                assertions.Add((maskDifferences==0,$"native Otsu every pixel {name}: {maskDifferences} differences at threshold {threshold}"));
                assertions.Add((actualMask.SequenceEqual(Decode(fixture.GetProperty("alternateMask"))),$"native feature-6 Otsu every half-size pixel {name}"));
                int rawThreshold=CalibrationBoardPreprocess.Otsu(new(width,height,input));var nativeRaw=Decode(fixture.GetProperty("rawMask"));
                assertions.Add((input.Select(value=>value<rawThreshold?(byte)0:(byte)255).SequenceEqual(nativeRaw),$"native two-level raw Otsu every pixel {name}"));
                assertions.Add((nativeRaw.SequenceEqual(Decode(fixture.GetProperty("alternateRawMask"))),$"native feature-6 Otsu every raw pixel {name}"));
            }
            differences.Add(new{name,halfDifferences,maskDifferences,threshold});
        }
        File.WriteAllText("analysis/calibration-board/preprocess-differences.json",JsonSerializer.Serialize(differences,new JsonSerializerOptions{WriteIndented=true}));
        foreach(var assertion in assertions)check(assertion.Passed,assertion.Message);
    }
}
