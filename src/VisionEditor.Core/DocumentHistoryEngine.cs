namespace VisionEditor.Core;

/// <summary>Source-guided managed root/part history. Payload T belongs to its host, not the VM SDK.</summary>
public sealed class DocumentHistoryEngine<T>
{
    readonly List<DocumentHistoryRoot<T>> undo = new(), redo = new();
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
            redo.Clear();
            while ((ulong)undo.Count > capacity) undo.RemoveAt(0);
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
        redo.Clear();
        root.Add(new(key, priority, oldValue, capture, restore));
        return true;
    }

    public bool Undo() => Replay(isUndo: true);
    public bool Redo() => Replay(isUndo: false);

    public void Clear()
    {
        undo.Clear();
        redo.Clear();
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
        while ((ulong)stack.Count > capacity) stack.RemoveAt(0);
    }
}

public sealed class DocumentHistoryRoot<T>
{
    readonly List<DocumentHistoryPart<T>> parts = new();
    internal DocumentHistoryRoot(string operation) { Operation = operation; Parts = parts.AsReadOnly(); }
    public string Operation { get; }
    public bool ForbidBuild { get; internal set; }
    public IReadOnlyList<DocumentHistoryPart<T>> Parts { get; }
    internal void Add(DocumentHistoryPart<T> part) => parts.Add(part);
}

public sealed class DocumentHistoryPart<T>
{
    readonly Func<T> capture;
    readonly Action<T> restore;
    T newValue = default!;
    internal DocumentHistoryPart(string key, int priority, T oldValue, Func<T> capture, Action<T> restore)
    { Key = key; Priority = priority; OldValue = oldValue; this.capture = capture; this.restore = restore; }
    public string Key { get; }
    public int Priority { get; }
    public T OldValue { get; private set; }
    public T NewValue => newValue;
    public bool IsDataComplete { get; private set; }
    /// <summary>Explicit host adaptation seam; not part of the original service's payload format.</summary>
    public void AdaptOldValue(Func<T, T> adapt)
    {
        ArgumentNullException.ThrowIfNull(adapt);
        OldValue = adapt(OldValue);
    }
    /// <summary>Explicit host view-only adaptation; it never recaptures an action's new structure.</summary>
    public void AdaptNewValue(Func<T, T> adapt)
    {
        ArgumentNullException.ThrowIfNull(adapt);
        if (!IsDataComplete) throw new InvalidOperationException("The new payload has not been completed.");
        newValue = adapt(newValue);
    }
    internal void CompleteNewValue()
    {
        IsDataComplete = true; // Source sets this before Update, including when capture throws.
        newValue = capture();
    }
    internal void Undo() => restore(OldValue);
    internal void Redo() => restore(newValue);
}
