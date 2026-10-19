using System.Collections.ObjectModel;
using System.Globalization;
using VisionFramework;

namespace VisionEditor.Core;

/// <summary>A bounded independent host summary. It does not own SDK/module result buffers.</summary>
public sealed class FrameworkRunHistory
{
    public const int MaximumRecords = 500;
    readonly ObservableCollection<FrameworkRunHistoryEntry> entries = new();
    long sequence;

    public FrameworkRunHistory() => Entries = new(entries);
    public ReadOnlyObservableCollection<FrameworkRunHistoryEntry> Entries { get; }

    public void Append(string procedureId, string procedureName, SessionStatus status, double elapsedMilliseconds, DateTime completedAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(procedureId);
        ArgumentNullException.ThrowIfNull(procedureName);
        if (!double.IsFinite(elapsedMilliseconds) || elapsedMilliseconds < 0)
            throw new ArgumentOutOfRangeException(nameof(elapsedMilliseconds));
        long next = checked(sequence + 1);
        var entry = new FrameworkRunHistoryEntry(next, completedAt.ToString("yyyy-MM-dd HH:mm:ss:fff", CultureInfo.InvariantCulture),
            procedureName + " · " + status, procedureId, elapsedMilliseconds);
        entries.Insert(0, entry);
        sequence = next;
        if (entries.Count > MaximumRecords) entries.RemoveAt(MaximumRecords);
    }

    /// <summary>Only clears this host's summary and sequence, not native result caches.</summary>
    public void Clear()
    {
        entries.Clear();
        sequence = 0;
    }
}

public sealed record FrameworkRunHistoryEntry(long Seq, string Time, string Data, string ProcedureId, double ElapsedMilliseconds);

/// <summary>Times available from current managed results; algorithm-only time is unavailable.</summary>
public sealed record FrameworkResultTiming(double? ProcessMilliseconds, double? ModuleMilliseconds)
{
    public const string Unavailable = "—";
    public string ProcessText => Format(ProcessMilliseconds);
    public string ModuleText => Format(ModuleMilliseconds);
    public string AlgorithmText => Unavailable;

    public static FrameworkResultTiming From(ExecutionSessionResult? currentSession, IEnumerable<NodeRunRecord> visibleResults, string? selectedNodeId)
    {
        ArgumentNullException.ThrowIfNull(visibleResults);
        var selected = selectedNodeId is null ? null : visibleResults.FirstOrDefault(record => record.NodeId == selectedNodeId);
        return new(currentSession is null ? null : currentSession.Nodes.Sum(record => record.ElapsedMilliseconds), selected?.ElapsedMilliseconds);
    }

    static string Format(double? milliseconds) => milliseconds is double value && double.IsFinite(value) && value >= 0
        ? value.ToString("F2", CultureInfo.InvariantCulture) : Unavailable;
}
