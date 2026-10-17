using System.Text.Json;
using VisionPlugins.Calibration;
var root = Directory.GetCurrentDirectory();
var evidence = JsonDocument.Parse(File.ReadAllText(Path.Combine(root,"docs/plugins/n-point-solver-evidence.json")));
int comparisons=0, failures=0, checks=0, sourceCases=0;
var differences=new List<object>();var conditioning=new List<object>();
void Check(string name,string field,double expected,double actual,double tolerance,string? known=null)
{
    checks++;double delta=Math.Abs(expected-actual);
    if(field is "Theta" or "Skew")delta=Math.Min(delta,Math.Abs(delta-Math.PI*2));
    if(double.IsFinite(actual)&&delta<=tolerance)return;
    differences.Add(new{name,field,expected,actual,delta,tolerance,classification=known??"unexpected"});
    if(known is null)failures++;
    // Keep the known convergence discrepancy visible and bounded; it is never counted as parity.
    if(name=="weight-perspective-h3-w2-f1"&&known is not null&&
        ((field is not ("Matrix[2]" or "OffsetWorldMaxError"))||delta>.0015))failures++;
}
var reports=new List<object>();
foreach(var c in evidence.RootElement.GetProperty("cases").EnumerateArray())
{
    sourceCases++;string name=c.GetProperty("name").GetString()!;
    var p=c.GetProperty("points").EnumerateArray().Select(x=>new NPointPair(x[0].GetDouble(),x[1].GetDouble(),x[2].GetDouble(),x[3].GetDouble())).ToArray();
    var r=c.GetProperty("rotation").EnumerateArray().Select(x=>new NPointRotation(x[0].GetDouble(),x[1].GetDouble(),x[2].GetDouble(),x[3].GetDouble(),x[4].GetDouble())).ToArray();
    var o=new NPointSolveOptions(c.GetProperty("freedom").GetInt32(),c.GetProperty("weight").GetInt32(),c.GetProperty("mode").GetInt32(),c.GetProperty("origin")[0].GetDouble(),c.GetProperty("origin")[1].GetDouble(),c.GetProperty("weightFactor").GetDouble(),c.GetProperty("distThreshold").GetDouble(),c.GetProperty("sampleRatio").GetDouble());
    var actual=NPointSolver.Solve(p,r,o); var original=c.GetProperty("result");
    if(!original.TryGetProperty("matrix",out var matrix))
    {Check(name,"ErrorCode",original.GetProperty("exception").GetUInt32(),actual.ErrorCode,0);continue;}
    double max=matrix.EnumerateArray().Select((x,i)=>Math.Abs(x.GetDouble()-actual.Matrix[i])).Max();
    comparisons++;
    string? known=name.StartsWith("mirror-f3-")?"reflection cannot be represented uniquely by an orientation-preserving similarity; native float candidate selection differs":
        name=="count4-f1"?"singular four-point projective set; original emits unstable matrix, independent solver rejects":null;
    string? iteration=name=="weight-perspective-h3-w2-f1"?"minimum Huber factor has a retained normalized-iteration discrepancy; this is not a passing parity comparison":null;
    for(int i=0;i<9;i++)Check(name,$"Matrix[{i}]",matrix[i].GetDouble(),actual.Matrix[i],.001,known??iteration);
    Check(name,"ErrorCode",0,actual.ErrorCode,0,known);
    if(known is null)
    {
        Check(name,"Status",original.GetProperty("status").GetInt32(),actual.Status,0);
        Check(name,"Direction",original.GetProperty("direction").GetInt32(),actual.Direction,0);
        var off=original.GetProperty("offset");var rot=original.GetProperty("rotation");var cen=original.GetProperty("center");var dec=original.GetProperty("decomposition");
        var fields=new[]{("OffsetError",off[0].GetDouble(),actual.OffsetError),("OffsetMaxError",off[1].GetDouble(),actual.OffsetMaxError),
            ("OffsetWorldError",off[3].GetDouble(),actual.OffsetWorldError),("OffsetWorldMaxError",off[4].GetDouble(),actual.OffsetWorldMaxError),
            ("RotateError",rot[0].GetDouble(),actual.RotateError),("RotateMaxError",rot[1].GetDouble(),actual.RotateMaxError),
            ("RotateWorldError",rot[3].GetDouble(),actual.RotateWorldError),("RotateWorldMaxError",rot[4].GetDouble(),actual.RotateWorldMaxError),
            ("CenterImageX",cen[0].GetDouble(),actual.CenterImageX),("CenterImageY",cen[1].GetDouble(),actual.CenterImageY),
            ("CenterWorldX",cen[2].GetDouble(),actual.CenterWorldX),("CenterWorldY",cen[3].GetDouble(),actual.CenterWorldY),
            ("DeltaX",dec[1].GetDouble(),actual.DeltaX),("DeltaY",dec[2].GetDouble(),actual.DeltaY)};
        foreach(var(field,expected,value) in fields)Check(name,field,expected,value,.001,iteration);
        Check(name,"Theta",dec[3].GetDouble(),actual.Theta,.0001,iteration);Check(name,"Skew",dec[5].GetDouble(),actual.Skew,.0001,iteration);
        Check(name,"PixelPrecision",dec[6].GetDouble(),actual.PixelPrecision,.0001,iteration);
        if(Math.Abs(Math.Cos(actual.Theta)+1.1920928955078125e-7)>.0001)Check(name,"Scale",dec[0].GetDouble(),actual.Scale,.0001,iteration);
        else conditioning.Add(new{name,field="Scale",expected=dec[0].GetDouble(),actual=actual.Scale,reason="native cos(theta)+FLT_EPS denominator near pole"});
        if(Math.Abs(Math.Sin(actual.Theta+actual.Skew)*actual.Scale+1.1920928955078125e-7)>.0001)Check(name,"Aspect",dec[4].GetDouble(),actual.Aspect,.001,iteration);
        else conditioning.Add(new{name,field="Aspect",expected=dec[4].GetDouble(),actual=actual.Aspect,reason="native sin(theta+skew)*scale+FLT_EPS denominator near pole"});
        var plain=(double[])actual.Matrix.Clone();if(o.CameraMode==3)for(int i=0;i<6;i++)plain[i]=-plain[i];
        var inv=(double[]?)typeof(NPointSolver).GetMethod("Inverse",System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.NonPublic)!.Invoke(null,new object[]{plain});
        var residuals=inv is null?new[]{0d,0d}:p.Select(point=>{var q=NPointSolver.Transform(inv,point.WorldX,point.WorldY);return Math.Sqrt(Math.Pow(q.X-(point.ImageX-o.OriginX),2)+Math.Pow(q.Y-(point.ImageY-o.OriginY),2));}).OrderDescending().ToArray();
        if(actual.OffsetMaxError>.001&&residuals[0]-residuals[1]>.001)Check(name,"OffsetMaxErrorPointNum",off[2].GetInt32(),actual.OffsetMaxErrorPointNum,0,iteration);
        else conditioning.Add(new{name,field="OffsetMaxErrorPointNum",reason="residual below .001 or top residuals within .001 permit native float roundoff to choose another tied point"});
        if(actual.RotateMaxError>.001)Check(name,"RotateMaxErrorPointNum",rot[2].GetInt32(),actual.RotateMaxErrorPointNum,0,iteration);
        else conditioning.Add(new{name,field="RotateMaxErrorPointNum",reason="residual below .001 permits native float roundoff to choose another tied point"});
    }
    reports.Add(new {name,matrixDelta=max,actual,original=original.Clone()});
}
Directory.CreateDirectory(Path.Combine(root,"analysis/full-audit/n-point-solver"));
File.WriteAllText(Path.Combine(root,"analysis/full-audit/n-point-solver/comparison.json"),JsonSerializer.Serialize(reports,new JsonSerializerOptions{WriteIndented=true}));
File.WriteAllText(Path.Combine(root,"docs/plugins/n-point-solver-verification.json"),JsonSerializer.Serialize(new{scope="Independent C# versus original no-device SDK; passing regression does not establish complete native parity",sourceCases,matrixCases=comparisons,checks,failures,differences,conditioning,cases=reports},new JsonSerializerOptions{WriteIndented=true})+Environment.NewLine);
Console.WriteLine($"NPointSolver: {checks} field checks, {failures} unexpected failures; {differences.Count} documented differing fields; {conditioning.Count} ill-conditioned/tied observations. Original cases {sourceCases}, matrices {comparisons}.");
return failures==0?0:1;
