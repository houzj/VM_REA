namespace VisionEditor.Core;

/// <summary>Independent host ownership guard for completions crossing solution/scope replacement.</summary>
public sealed class ExecutionOwnerEpoch
{
    long generation;
    int closed;

    public ExecutionOwnerStamp Capture(string solutionId, object scopeOwner)
    {
        ArgumentNullException.ThrowIfNull(solutionId);
        ArgumentNullException.ThrowIfNull(scopeOwner);
        return new(this, Interlocked.Read(ref generation), solutionId, scopeOwner);
    }

    public bool IsCurrent(ExecutionOwnerStamp stamp, string solutionId, object scopeOwner) =>
        Volatile.Read(ref closed) == 0 && ReferenceEquals(stamp.Epoch, this)
        && stamp.Generation == Interlocked.Read(ref generation)
        && string.Equals(stamp.SolutionId, solutionId, StringComparison.Ordinal)
        && ReferenceEquals(stamp.ScopeOwner, scopeOwner);

    /// <summary>Call before replacing or disposing the old execution scope, even for identical document IDs.</summary>
    public void Invalidate() => Interlocked.Increment(ref generation);

    public void Close()
    {
        Interlocked.Exchange(ref closed, 1);
        Invalidate();
    }
}

public readonly struct ExecutionOwnerStamp
{
    internal ExecutionOwnerStamp(ExecutionOwnerEpoch epoch, long generation, string solutionId, object scopeOwner)
    { Epoch = epoch; Generation = generation; SolutionId = solutionId; ScopeOwner = scopeOwner; }
    internal ExecutionOwnerEpoch? Epoch { get; }
    internal long Generation { get; }
    internal string? SolutionId { get; }
    internal object? ScopeOwner { get; }
}
