namespace VisionEditor.Core;

/// <summary>Source-guided managed root/part history. Payload T belongs to its host, not the VM SDK.</summary>
public sealed class DocumentHistoryEngine<T> : IDisposable
{
    readonly List<DocumentHistoryRoot<T>> undo = new(), redo = new();
    readonly Action<T>? releasePayload;
    readonly List<DocumentHistoryRoot<T>> retainedReleaseFailures = new();
    readonly List<Exception> releaseErrors = new();
    public DocumentHistoryEngine(Action<T>? releasePayload = null) { this.releasePayload = releasePayload; }
    public IReadOnlyList<Exception> ReleaseErrors => releaseErrors.AsReadOnly();
    public int RetainedReleaseFailureCount => retainedReleaseFailures.Count;
    int replayDepth;
    int lazyLoadingDepth;
    uint capacity = 15;
    public int UndoCount => undo.Count;
    public int RedoCount => redo.Count;
    public bool IsReplaying => replayDepth != 0;
    public int LazyLoadingDepth => lazyLoadingDepth;
    public DocumentHistoryRoot<T>? CurrentRoot => undo.Count == 0 ? null : undo[^1];
    public DocumentHistoryRoot<T>? NextRedoRoot => redo.Count == 0 ? null : redo[^1];
    public Exception? LastError { get; private set; }
    public uint Capacity
    {
        get => capacity;
        set
        {
            capacity = value;
            if ((ulong)undo.Count + (ulong)redo.Count <= capacity) return;
            ReleaseStack(redo);
            Trim(undo);
        }
    }

    public bool BeginRoot(string operation = "Edit", Func<DocumentHistoryRoot<T>, bool>? canRegister = null)
    {
        ArgumentNullException.ThrowIfNull(operation);
        if (IsReplaying || lazyLoadingDepth > 0 || (canRegister is not null && (CurrentRoot is null || !canRegister(CurrentRoot)))) return false;
        undo.Add(new(operation));
        Trim(undo);
        return CurrentRoot is not null;
    }

    public void SetForbidRecord(bool forbidden)
    {
        if (CurrentRoot is { } root) root.ForbidBuild = forbidden;
    }

    /// <summary>Original lazy-loading admission is a signed counter, unlike the root boolean flag.</summary>
    public void SetIsLazyLoading(bool loading)
    {
        if (loading) Interlocked.Increment(ref lazyLoadingDepth);
        else Interlocked.Decrement(ref lazyLoadingDepth);
    }

    /// <summary>Checks admission before evaluating capture, preserving the first admitted old value.</summary>
    public bool RegisterPart(string key, Func<T> capture, Action<T> restore, int priority = 0,
        Func<DocumentHistoryRoot<T>, string, bool>? canRegister = null)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(capture);
        ArgumentNullException.ThrowIfNull(restore);
        var root = CurrentRoot;
        if (IsReplaying || lazyLoadingDepth > 0 || root is null || root.ForbidBuild || (canRegister is not null && !canRegister(root, key))) return false;
        T oldValue = capture();
        // BuildPart invalidates redo only after a new part passed registration.
        ReleaseStack(redo);
        root.Add(new(key, priority, oldValue, capture, restore, releasePayload));
        return true;
    }

    public bool Undo() => Replay(isUndo: true);
    public bool Redo() => Replay(isUndo: false);

    public void Clear()
    {
        ReleaseStack(undo);
        ReleaseStack(redo);
        LastError = null;
    }

    bool Replay(bool isUndo)
    {
        LastError = null;
        replayDepth++;
        try
        {
            var source = isUndo ? undo : redo;
            var destination = isUndo ? redo : undo;
            if (source.Count == 0) return false;
            var root = source[^1];
            if (isUndo)
                foreach (var part in root.Parts.Where(part => !part.IsDataComplete).ToArray()) part.CompleteNewValue();
            source.RemoveAt(source.Count - 1);
            destination.Add(root);
            Trim(destination);
            foreach (var part in Ordered(root, isUndo))
                if (isUndo) part.Undo(); else part.Redo();
            return true;
        }
        catch (Exception error)
        {
            // Original service catches failures: no implicit rollback or safe retry is invented.
            LastError = error;
            return false;
        }
        finally { replayDepth--; }
    }

    static IEnumerable<DocumentHistoryPart<T>> Ordered(DocumentHistoryRoot<T> root, bool undo)
    {
        if (root.Operation == "CopyModule")
            return undo ? root.Parts.Reverse().ToArray()
                : root.Parts.Where(part => part.Priority != 2).Concat(root.Parts.Where(part => part.Priority == 2)).ToArray();
        return undo
            ? root.Parts.Where(part => part.Priority == 1).Reverse().Concat(root.Parts.Where(part => part.Priority != 1).Reverse()).ToArray()
            : root.Parts.Where(part => part.Priority == 1).Concat(root.Parts.Where(part => part.Priority != 1)).ToArray();
    }

    void Trim(List<DocumentHistoryRoot<T>> stack)
    {
        while ((ulong)stack.Count > capacity)
        {
            var root = stack[0]; stack.RemoveAt(0); ReleaseRoot(root);
        }
    }
    void ReleaseStack(List<DocumentHistoryRoot<T>> stack)
    {
        var roots = stack.ToArray(); stack.Clear();
        foreach (var root in roots) ReleaseRoot(root);
    }
    void ReleaseRoot(DocumentHistoryRoot<T> root)
    {
        try { root.Release(); }
        catch (Exception error) { releaseErrors.Add(error); retainedReleaseFailures.Add(root); }
    }
    // Explicit cleanup retry retains failed payload ownership; no silent filesystem retry loop.
    public void ReleaseRetainedPayloads()
    {
        var roots = retainedReleaseFailures.ToArray(); retainedReleaseFailures.Clear();
        foreach (var root in roots) ReleaseRoot(root);
    }
    public void Dispose() => Clear();
}

public sealed class DocumentHistoryRoot<T>
{
    readonly List<DocumentHistoryPart<T>> parts = new();
    internal DocumentHistoryRoot(string operation) { Operation = operation; Parts = parts.AsReadOnly(); }
    public string Operation { get; }
    public bool ForbidBuild { get; internal set; }
    public IReadOnlyList<DocumentHistoryPart<T>> Parts { get; }
    internal void Add(DocumentHistoryPart<T> part) => parts.Add(part);
    internal void Release()
    {
        List<Exception>? errors = null;
        foreach (var part in parts)
            try { part.Release(); } catch (Exception error) { (errors ??= new()).Add(error); }
        if (errors is not null) throw new AggregateException("History root payload release failed; ownership retained.",errors);
    }
}

public sealed class DocumentHistoryPart<T>
{
    readonly Func<T> capture;
    readonly Action<T> restore;
    readonly Action<T>? release;
    readonly List<T> retainedValues = new();
    bool oldReleased, newReleased, hasNewValue;
    T newValue = default!;
    internal DocumentHistoryPart(string key, int priority, T oldValue, Func<T> capture, Action<T> restore, Action<T>? release)
    { Key = key; Priority = priority; OldValue = oldValue; this.capture = capture; this.restore = restore; this.release = release; }
    public string Key { get; }
    public int Priority { get; }
    public T OldValue { get; private set; }
    public T NewValue => newValue;
    public bool IsDataComplete { get; private set; }
    /// <summary>Explicit host adaptation seam; not part of the original service's payload format.</summary>
    public void AdaptOldValue(Func<T, T> adapt)
    {
        ArgumentNullException.ThrowIfNull(adapt);
        var previous = OldValue; var next = adapt(previous);
        if (ReferenceEquals(previous,next)) return;
        OldValue = next;
        ReleaseReplaced(previous);
    }
    /// <summary>Explicit host view-only adaptation; it never recaptures an action's new structure.</summary>
    public void AdaptNewValue(Func<T, T> adapt)
    {
        ArgumentNullException.ThrowIfNull(adapt);
        if (!IsDataComplete) throw new InvalidOperationException("The new payload has not been completed.");
        var previous = newValue; var next = adapt(previous);
        if (ReferenceEquals(previous,next)) return;
        newValue = next;
        ReleaseReplaced(previous);
    }
    internal void CompleteNewValue()
    {
        IsDataComplete = true; // Source sets this before Update, including when capture throws.
        newValue = capture();
        hasNewValue = true;
    }
    void ReleaseReplaced(T value)
    {
        if (release is null) return;
        try { release(value); } catch { retainedValues.Add(value); throw; }
    }
    internal void Release()
    {
        if (release is null) return;
        List<Exception>? errors = null;
        if (!oldReleased) try { release(OldValue); oldReleased = true; } catch (Exception e) { (errors ??= new()).Add(e); }
        if (hasNewValue && !newReleased) try { release(newValue); newReleased = true; } catch (Exception e) { (errors ??= new()).Add(e); }
        foreach (var value in retainedValues.ToArray())
            try { release(value); retainedValues.Remove(value); } catch (Exception e) { (errors ??= new()).Add(e); }
        if (errors is not null) throw new AggregateException(errors);
    }
    internal void Undo() => restore(OldValue);
    internal void Redo() => restore(newValue);
}
