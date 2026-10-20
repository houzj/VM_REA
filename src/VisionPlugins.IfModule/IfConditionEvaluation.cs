using System.Globalization;

namespace VisionPlugins.IfModule;

/// <summary>Normal int/float condition path recovered from IfModule.dll Process 0x18000b0c0.
/// Parsing malformed native char buffers and SDK handle reads remain separate boundaries.</summary>
public static class IfConditionEvaluation {
    public const uint EmptyConditions = 0xe0000309;
    public const uint InvalidRange = 0xe000030e;
    public const uint AllInputsUnreadable = 0xe0000314;

    public static IfConditionOutcome Evaluate(IReadOnlyList<IfConditionSample> samples, int logicalMode) {
        ArgumentNullException.ThrowIfNull(samples);
        if (samples.Count == 0) return new(EmptyConditions, false, false, false, [], []);
        var passed = new List<bool>(samples.Count);
        var read = new List<bool>(samples.Count);
        bool anyRead = false, aggregate = false;
        foreach (var sample in samples) {
            bool item;
            if (sample.Row.Kind == "int") {
                int minimum = ParseIntegerBound(sample.Row.Minimum, true), maximum = ParseIntegerBound(sample.Row.Maximum, false);
                if (maximum < minimum) return new(InvalidRange, false, anyRead, aggregate, passed.AsReadOnly(), read.AsReadOnly());
                item = sample.IntegerValue is int value && value <= maximum && value >= minimum;
                anyRead |= sample.IntegerValue.HasValue;
                read.Add(sample.IntegerValue.HasValue);
            } else if (sample.Row.Kind == "float") {
                float minimum = ParseFloat(sample.Row.Minimum), maximum = ParseFloat(sample.Row.Maximum);
                if (minimum > maximum) return new(InvalidRange, false, anyRead, aggregate, passed.AsReadOnly(), read.AsReadOnly());
                // COMISS/J(C) in the original rejects unordered sample comparisons.
                item = sample.FloatValue is float value && !float.IsNaN(value) && value <= maximum && value >= minimum;
                anyRead |= sample.FloatValue.HasValue;
                read.Add(sample.FloatValue.HasValue);
            } else throw new ArgumentException("Only source-declared int/float condition rows are supported.", nameof(samples));
            passed.Add(item);
            if (passed.Count == 1) aggregate = item;
            else if (logicalMode is 1 or 3) aggregate &= item;
            else aggregate |= item;
        }
        if (logicalMode is 3 or 4) aggregate = !aggregate;
        return new(anyRead ? 0u : AllInputsUnreadable, true, anyRead, aggregate,
            passed.AsReadOnly(), read.AsReadOnly());
    }

    // Native IFText setter calls atoi on each integer bound, then splits the
    // original spelling with strtok("."). A positive second nonempty token
    // increments Min or decrements Max. Malformed atoi prefixes/overflow stay
    // outside this independently safe managed parser boundary.
    static int ParseIntegerBound(string value, bool minimum) {
        int dot = value.IndexOf('.');
        var integral = dot < 0 ? value : value[..dot];
        int number = integral.Length == 0 ? 0 : int.Parse(integral, NumberStyles.Integer, CultureInfo.InvariantCulture);
        if (dot < 0) return number;
        var tokens = value.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length > 1 && int.Parse(tokens[1], NumberStyles.Integer, CultureInfo.InvariantCulture) > 0)
            return checked(number + (minimum ? 1 : -1));
        return number;
    }
    static float ParseFloat(string value) {
        var parsed = float.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);
        if (!float.IsFinite(parsed)) throw new ArgumentOutOfRangeException(nameof(value));
        return parsed;
    }
}

public sealed record IfConditionSample(IfConditionRow Row, int? IntegerValue = null, float? FloatValue = null);
public sealed record IfConditionOutcome(uint NativeErrorCode, bool AggregatePublished, bool ModuStatus,
    bool Result, IReadOnlyList<bool> ItemResults, IReadOnlyList<bool> ItemReads) {
    // The matching native PE .rdata at RVA 0x12498 stores "OK\0\0NG\0\0".
    public string? StrResult => AggregatePublished ? (Result ? "OK" : "NG") : null;
    public IReadOnlyList<string> IfItemResult => ItemResults.Select(value => value ? "OK" : "NG").ToArray();
    // Native Process writes bVar14, the condition verdict, not the input-read status.
    public IReadOnlyList<int> IfItemStatus => ItemResults.Select(value => value ? 1 : 0).ToArray();
}
