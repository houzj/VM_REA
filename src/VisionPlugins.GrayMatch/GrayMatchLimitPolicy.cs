namespace VisionPlugins.GrayMatch;

// Source: module b7327ffa..., 1800238b0/18001dd90/18001ddd0 and matching asm.
// Source-confirmed limit leaf. Template scoring/location remain separate obligations.
public readonly record struct GrayMatchIntegerLimit(int Enable, int Low, int High)
{
    public bool Accept(int value) => Enable != 1 || (value >= Low && value <= High);
}

public readonly record struct GrayMatchFloatLimit(int Enable, float Low, float High)
{
    public bool Accept(float value)
    {
        // Original PE 1800310a4 raw 00000034, not System.Single.Epsilon.
        const float epsilon = 1.1920928955078125e-7f;
        if (Enable != 1) return true;
        float lower = Low - epsilon;
        float upper = High + epsilon;
        // COMISS/JA then COMISS/JBE: unordered comparisons accept.
        return !(value < lower || value > upper);
    }
}

public sealed record GrayMatchLimits(
    GrayMatchIntegerLimit Count,
    GrayMatchFloatLimit Angle,
    GrayMatchFloatLimit Score,
    GrayMatchFloatLimit MatchPointX,
    GrayMatchFloatLimit MatchPointY,
    GrayMatchFloatLimit BoxPointX,
    GrayMatchFloatLimit BoxPointY)
{
    // The original reads these raw record slots even where UI names are misleading.
    // Source statuses are computed before SortType and output MaxMatchNum truncation.
    public (uint ModuleStatus, uint[] MatchStatus) Apply(uint initialStatus,
        int totalCount, IReadOnlyList<float[]> sourceRecords)
    {
        if (totalCount <= 0) return (initialStatus, []);
        // Independent managed safety boundary for malformed caller records.
        // The supported module path derives totalCount from this same vector.
        if (sourceRecords.Count != totalCount)
            throw new ArgumentException("Source count and source vector must agree.", nameof(totalCount));
        uint status = initialStatus & (Count.Accept(totalCount) ? 1u : 0u);
        var individual = new uint[sourceRecords.Count];
        for (int i = 0; i < sourceRecords.Count; i++)
        {
            var p = sourceRecords[i];
            if (p.Length < 11)
                throw new ArgumentException("A source match record requires at least 11 float slots.", nameof(sourceRecords));
            uint accepted = Angle.Accept(p[4]) && Score.Accept(p[10]) &&
                MatchPointX.Accept(p[0]) && MatchPointY.Accept(p[1]) &&
                BoxPointX.Accept(p[5]) && BoxPointY.Accept(p[6]) ? 1u : 0u;
            individual[i] = accepted;
            status &= accepted;
        }
        return (status, individual);
    }
}
