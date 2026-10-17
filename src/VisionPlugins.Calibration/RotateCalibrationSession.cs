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
    public RotateCalibrationStep Run(IReadOnlyDictionary<string, IReadOnlyList<double>> inputs, bool reverseOutput)
    {
        var output = new Dictionary<string, double>();
        RotateCalibrationStep Failure(uint code, bool status = false)
        { samples.Clear(); if (status) output["ModuStatus"] = 0; return new(code, output); }
        bool First(string name, out double value)
        { value = 0; if (!inputs.TryGetValue(name, out var array) || array.Count == 0) return false; value = array[0]; return true; }
        if (First("RefreshSignal", out var refresh) && (int)refresh != 0) Load(FilePath);
        if (!Loaded) return Failure(0xe0000008);
        bool hasCount = First("RotNum", out var rawCount); int count = (int)rawCount;
        if (hasCount && count is >= 3 and <= 16) Total = count;
        if (count is < 3 or > 16) return Failure(0xe0000001);
        int next = samples.Count + 1; Remaining = Total - next;
        if (Remaining < 0) return Failure(0xe0000001);
        uint missing = 0xe000031c;
        if (!First("ImagePointX", out var x)) return Failure(inputs.ContainsKey("ImagePointX") ? 0 : missing, true);
        if (!First("ImagePointY", out var y)) return Failure(inputs.ContainsKey("ImagePointY") ? 0 : missing, true);
        if (!First("WorldRotateAngle", out var angle)) return Failure(inputs.ContainsKey("WorldRotateAngle") ? 0 : missing, true);
        samples.Add(new((float)x, (float)y, (next - 1) * (float)angle));
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
