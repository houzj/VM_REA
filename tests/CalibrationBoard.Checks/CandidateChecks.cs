using System.IO;
using System.Text.Json;
using VisionPlugins.CalibrationBoard;

static class CandidateChecks
{
    public static void Run(Action<bool,string> check,bool all=false)
    {
        using var document=JsonDocument.Parse(File.ReadAllText("docs/plugins/calibration-board-candidate-fixtures.json"));
        using var sdk=JsonDocument.Parse(File.ReadAllText("docs/plugins/calibration-board-refinements.json"));
        int Value(JsonElement row,string key)=>row.GetProperty(key).GetInt32();
        var sdkRows=sdk.RootElement.GetProperty("circle").GetProperty("rows").EnumerateArray().ToDictionary(r=>(Value(r,"shape"),Value(r,"circularity"),Value(r,"contrast"),Value(r,"low"),Value(r,"high")));
        var assertions=new List<(bool Passed,string Message)>();var differences=new List<object>();
        using var fallback=JsonDocument.Parse(File.ReadAllText("docs/plugins/calibration-board-fallback-fixtures.json"));
        foreach(var fixture in fallback.RootElement.GetProperty("rows").EnumerateArray())
        {
            int width=fixture.GetProperty("width").GetInt32(),height=fixture.GetProperty("height").GetInt32(),pattern=fixture.GetProperty("pattern").GetInt32();var pixels=Enumerable.Repeat((byte)255,width*height).ToArray();
            for(int y=1;y<height-1;y++)for(int x=1;x<width-1;x++)if(pattern==1||(pattern==2&&(x*7+y*13)%5<3))pixels[y*width+x]=0;
            bool found=CalibrationBoardCircleCandidates.TryFrameFallback(new(width,height,pixels),out var center);var expected=fixture.GetProperty("center");
            check(found&&center.X==expected[0].GetSingle()&&center.Y==expected[1].GetSingle(),$"native frame fallback float center {width}x{height}, pattern {pattern}");
        }
        foreach(var fixture in document.RootElement.GetProperty("rows").EnumerateArray())
        {
            int shape=fixture.TryGetProperty("shape",out var shapeValue)?shapeValue.GetInt32():0,circularity=fixture.TryGetProperty("circularity",out var circularityValue)?circularityValue.GetInt32():50;
            int contrast=fixture.GetProperty("contrast").GetInt32(),low=fixture.GetProperty("low").GetInt32(),high=fixture.GetProperty("high").GetInt32();var pixels=Enumerable.Repeat((byte)255,480*400).ToArray();
            if(!all&&(shape!=0||circularity!=50||(contrast!=2&&contrast!=5&&contrast!=255)||(high!=1&&high!=40)))continue;
            for(int row=0;row<8;row++)for(int col=0;col<10;col++)for(int dy=-9;dy<=9;dy++)for(int dx=-9;dx<=9;dx++){bool inside=shape==0?dx*dx+dy*dy<81:shape==1?dx*dx/81f+dy*dy/25f<1:Math.Abs(dx)<9&&Math.Abs(dy)<9;if(inside)pixels[(80+row*30+dy)*480+90+col*30+dx]=(byte)(255-contrast);}
            var actual=CalibrationBoardCircleCandidates.Find(new(480,400,pixels),circularity,low,high);var expected=fixture.GetProperty("records").EnumerateArray().ToArray();
            bool same=actual.Count==expected.Length;
            if(same)for(int i=0;i<actual.Count;i++)same&=actual[i].X==expected[i][0].GetSingle()&&actual[i].Y==expected[i][1].GetSingle()&&actual[i].EdgeFit==(expected[i][2].GetSingle()!=0)&&actual[i].Size==expected[i][6].GetSingle();
            float maximum=0;if(actual.Count==expected.Length)for(int i=0;i<actual.Count;i++)maximum=Math.Max(maximum,Math.Max(Math.Abs(actual[i].X-expected[i][0].GetSingle()),Math.Abs(actual[i].Y-expected[i][1].GetSingle())));
            differences.Add(new{shape,circularity,contrast,low,high,passed=same,actualCount=actual.Count,expectedCount=expected.Length,maximum});
            assertions.Add((same,$"native factor5 initial candidates every coordinate/flag/size shape {shape}, circularity {circularity}, contrast {contrast}, thresholds {low}/{high}: {actual.Count}/{expected.Length}"));
            var nativeSdk=sdkRows[(shape,circularity,contrast,low,high)];var matched=CalibrationBoardCircleGrid.Match(actual).Points;
            bool nativeSuccess=nativeSdk.GetProperty("code").GetUInt32()==0;
            assertions.Add((nativeSuccess?matched.Count==Value(nativeSdk,"count"):matched.Count==0,"original SDK recognition success/failure and point count"));
            if(nativeSuccess)assertions.Add((matched.Count>0&&matched[0].X==nativeSdk.GetProperty("x").GetSingle()&&matched[0].Y==nativeSdk.GetProperty("y").GetSingle(),"original SDK first sorted point exact float"));
        }
        File.WriteAllText(all?"analysis/calibration-board/candidate-differences-full.json":"analysis/calibration-board/candidate-differences.json",JsonSerializer.Serialize(differences,new JsonSerializerOptions{WriteIndented=true}));
        foreach(var assertion in assertions)check(assertion.Passed,assertion.Message);
    }
}
