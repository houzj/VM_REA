namespace VisionEditor.Core;

public enum HistoryPayloadKind { Memory, OwnedFile, BorrowedFile }

/// <summary>Owns only a fresh directory and opaque payloads. No SDK serialization or shared TempFiles cleanup.</summary>
public sealed class HistoryPayloadStore : IDisposable
{
    readonly object gate = new();
    readonly HashSet<Entry> entries = new();
    readonly List<HistoryPayloadWriteFailure> writeFailures = new();
    readonly string marker, identity = Guid.NewGuid().ToString("N");
    bool closing, directoryReleased;
    public HistoryPayloadStore(string parentDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(parentDirectory);
        DirectoryPath = Path.Combine(Path.GetFullPath(parentDirectory), "history-" + identity);
        if (Directory.Exists(DirectoryPath)) throw new IOException("Owned history directory already exists.");
        Directory.CreateDirectory(DirectoryPath);
        marker = Path.Combine(DirectoryPath, ".owner");
        using var stream = new FileStream(marker, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        using var writer = new StreamWriter(stream); writer.Write(identity);
    }
    public string DirectoryPath { get; }
    public int LivePayloadCount { get { lock (gate) return entries.Count; } }
    public IReadOnlyList<HistoryPayloadWriteFailure> WriteFailures { get { lock (gate) return writeFailures.ToArray(); } }
    public HistoryPayloadLease CreateMemory(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        lock (gate) { EnsureOpen(); return Add(new(HistoryPayloadKind.Memory, bytes.ToArray(), null)); }
    }
    public HistoryPayloadLease CreateFile(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        return CreateFile(output => output.Write(bytes));
    }
    /// <summary>Writes a new opaque payload; failures retain diagnostics and clean only that newly created file.</summary>
    public HistoryPayloadLease CreateFile(Action<Stream> writePayload)
    {
        ArgumentNullException.ThrowIfNull(writePayload);
        lock (gate)
        {
            EnsureOpen();
            string path = Path.Combine(DirectoryPath, Guid.NewGuid().ToString("N") + ".payload");
            bool created = false;
            try
            {
                using var output = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                created = true; writePayload(output); output.Flush(true);
            }
            catch (Exception failure)
            {
                Exception? cleanupFailure = null;
                if (created) try { File.Delete(path); } catch (Exception error) { cleanupFailure = error; }
                writeFailures.Add(new(path, failure.ToString(), cleanupFailure?.ToString(), File.Exists(path)));
                if (cleanupFailure is not null) throw new AggregateException("Payload write and owned partial-file cleanup both failed.", failure, cleanupFailure);
                throw;
            }
            return Add(new(HistoryPayloadKind.OwnedFile, null, path));
        }
    }
    /// <summary>Reuses tracked ownership for the same path, otherwise borrows without deletion rights.</summary>
    public HistoryPayloadLease AcquireFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        string full = Path.GetFullPath(path);
        lock (gate)
        {
            EnsureOpen();
            var existing = entries.FirstOrDefault(entry => entry.Path is not null && StringComparer.OrdinalIgnoreCase.Equals(entry.Path, full));
            if (existing is not null) { existing.References++; return new(this, existing); }
            if (!File.Exists(full)) throw new FileNotFoundException("History payload does not exist.", full);
            return Add(new(HistoryPayloadKind.BorrowedFile, null, full));
        }
    }
    HistoryPayloadLease Add(Entry entry) { entries.Add(entry); return new(this, entry); }
    void EnsureOpen() => ObjectDisposedException.ThrowIf(closing, this);
    internal HistoryPayloadLease Retain(Entry entry)
    {
        lock (gate) { EnsureOpen(); Verify(entry); entry.References++; return new(this, entry); }
    }
    internal byte[] Read(Entry entry)
    {
        lock (gate) { Verify(entry); return entry.Kind == HistoryPayloadKind.Memory ? entry.Bytes!.ToArray() : File.ReadAllBytes(entry.Path!); }
    }
    void Verify(Entry entry) { if (!entries.Contains(entry)) throw new ObjectDisposedException(nameof(HistoryPayloadLease)); }
    internal bool IsLive(Entry entry) { lock (gate) return entries.Contains(entry); }
    internal void Release(Entry entry)
    {
        lock (gate)
        {
            Verify(entry);
            if (entry.References > 1) { entry.References--; return; }
            // Failure keeps ownership/reference alive so the caller can retain the exact failed payload.
            if (entry.Kind == HistoryPayloadKind.OwnedFile) File.Delete(entry.Path!);
            entries.Remove(entry); entry.Bytes = null; entry.References = 0;
            if (closing && entries.Count == 0) ReleaseDirectory();
        }
    }
    public void Dispose()
    {
        lock (gate) { closing = true; if (entries.Count == 0) ReleaseDirectory(); }
    }
    void ReleaseDirectory()
    {
        if (directoryReleased || !Directory.Exists(DirectoryPath)) { directoryReleased = true; return; }
        // Never recursively delete or remove unknown files. Even this directory may contain new audit material.
        if (Directory.EnumerateFileSystemEntries(DirectoryPath).Any(path => !StringComparer.OrdinalIgnoreCase.Equals(path, marker))) return;
        if (!File.Exists(marker) || File.ReadAllText(marker) != identity) throw new IOException("Owned history marker changed; cleanup refused.");
        File.Delete(marker); Directory.Delete(DirectoryPath, recursive: false); directoryReleased = true;
    }
    internal sealed class Entry(HistoryPayloadKind kind, byte[]? bytes, string? path)
    {
        internal HistoryPayloadKind Kind { get; } = kind;
        internal byte[]? Bytes = bytes;
        internal string? Path { get; } = path;
        internal int References = 1;
    }
}

public sealed record HistoryPayloadWriteFailure(string FilePath, string WriteError, string? CleanupError, bool FileRetained);

public sealed class HistoryPayloadLease : IDisposable
{
    readonly object gate = new();
    HistoryPayloadStore? owner;
    readonly HistoryPayloadStore.Entry entry;
    internal HistoryPayloadLease(HistoryPayloadStore owner, HistoryPayloadStore.Entry entry) { this.owner = owner; this.entry = entry; }
    public HistoryPayloadKind Kind => entry.Kind;
    public string? FilePath => entry.Path;
    public byte[] ReadBytes() { lock (gate) return Active.Read(entry); }
    public HistoryPayloadLease Retain() { lock (gate) return Active.Retain(entry); }
    HistoryPayloadStore Active => owner ?? throw new ObjectDisposedException(nameof(HistoryPayloadLease));
    public void Dispose()
    {
        lock (gate)
        {
            if (owner is null) return;
            var current = owner;
            try { current.Release(entry); owner = null; }
            catch { if (!current.IsLive(entry)) owner = null; throw; }
        }
    }
}
