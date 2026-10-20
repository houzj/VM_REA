using VisionPluginContracts;
namespace VisionPlugins.Calibration;

public sealed record RotateCalibrationStep(uint ErrorCode, IReadOnlyDictionary<string, double> Outputs);
/// <summary>Source-confirmed sequential sampler; raw result output presence is significant on failures.</summary>
public sealed class RotateCalibrationSession
{
    readonly List<RotateCalibrationSample> samples = [];
    public float[] Matrix { get; } = new float[9];
    public bool Loaded { get; private set; }
    public string FilePath { get; private set; } = "";
    public int Current => samples.Count;
    public int Total { get; private set; }
    public int Remaining { get; private set; }
    public uint Load(string path)
    {
        // An invalid suffix returns before changing both matrix validity and path.
        if (!path.EndsWith("ml", StringComparison.Ordinal) && !path.EndsWith("al", StringComparison.Ordinal)) return 0xe0000204;
        uint code = RotateCalibrationFile.Read(path, Matrix, out var loaded);
        Loaded = loaded; FilePath = path;
        return code;
    }
    public void Restore(IReadOnlyList<float> matrix, bool loaded, string path)
    { for (int i = 0; i < Math.Min(9, matrix.Count); i++) Matrix[i] = matrix[i]; Loaded = loaded; FilePath = path; }
    // Compatibility adapter for the archived synthetic-host fixtures. Production calls
    // the typed overload below; absent protocol here retains the fixture host's E031c.
    public RotateCalibrationStep Run(IReadOnlyDictionary<string,IReadOnlyList<double>> inputs,bool reverseOutput) =>
        Run(name=>inputs.TryGetValue(name,out var values)?new(0,values.Count,values.Count>0?values[0]:0):new(0xe000031c,0,0),
            name=>inputs.TryGetValue(name,out var values)?new(0,values.Count,values.Count>0?unchecked((int)values[0]):0):new(0xe000031c,0,0),reverseOutput);
    public RotateCalibrationStep Run(Func<string,ToolNumericInputReadResult> floating,Func<string,ToolIntegerInputReadResult> integer,bool reverseOutput)
    {
        var output = new Dictionary<string,double>();
        RotateCalibrationStep Failure(uint code,bool status=false){samples.Clear();if(status)output["ModuStatus"]=0;return new(code,output);}
        // 376b0: Refresh is always first. Failure explicitly resets its local value to 0.
        var refresh=integer("RefreshSignal");if(refresh.ErrorCode==0&&refresh.Count>0&&refresh.Value!=0)Load(FilePath);
        if(!Loaded)return Failure(0xe0000008);
        var countRead=integer("RotNum");int count=countRead.ErrorCode==0&&countRead.Count>0?countRead.Value:0;
        if(countRead.ErrorCode==0&&countRead.Count>0&&count is >=3 and <=16)Total=count;
        if(count is <3 or >16)return Failure(0xe0000001);
        int next=samples.Count+1;Remaining=Total-next;if(Remaining<0)return Failure(0xe0000001);
        var x=floating("ImagePointX");if(x.ErrorCode!=0||x.Count<=0)return Failure(x.ErrorCode,true);
        var y=floating("ImagePointY");if(y.ErrorCode!=0||y.Count<=0)return Failure(y.ErrorCode,true);
        var angle=floating("WorldRotateAngle");if(angle.ErrorCode!=0||angle.Count<=0)return Failure(angle.ErrorCode,true);
        samples.Add(new((float)x.Value,(float)y.Value,(next-1)*(float)angle.Value));
        RotateCalibrationResult? result = null;
        if (samples.Count == Total)
        {
            var positive = RotateCalibrationSolver.Solve(samples, Matrix, true);
            var negative = RotateCalibrationSolver.Solve(samples, Matrix, false);
            result = reverseOutput ? (positive.WorldError <= negative.WorldError ? negative : positive)
                : (negative.WorldError <= positive.WorldError ? negative : positive);
            samples.Clear();
        }
        output["ModuStatus"] = 1; output["remainCount"] = Remaining;
        output["RotCenterImageX"] = (float)(result?.ImageX ?? 0); output["RotCenterImageY"] = (float)(result?.ImageY ?? 0);
        output["RotCenterWorldX"] = (float)(result?.WorldX ?? 0); output["RotCenterWorldY"] = (float)(result?.WorldY ?? 0);
        output["RotError"] = (float)(result?.PixelError ?? 0); output["RotWorldError"] = (float)(result?.WorldError ?? 0);
        return new(0, output);
    }
}
