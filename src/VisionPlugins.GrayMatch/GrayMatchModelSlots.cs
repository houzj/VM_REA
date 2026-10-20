namespace VisionPlugins.GrayMatch;

// Source b7327ffa... 8780/89a0/8ed0/8f70/8da0/9b70/98d0.
// Typed normal-domain store; does not claim original string-atoi or unsafe ABI parity.
public sealed class GrayMatchModelSlots
{
    public const uint InvalidArgument = 0xe0000001;
    public const uint Full = 0xe0000326;
    private sealed record Slot(byte[] Name, byte[] Data, int Selected);
    private readonly Slot?[] slots = new Slot?[100];
    private readonly byte[] pendingName = new byte[0x104];
    // Exact ctor73d0: +d9f0 = -1; Clear does not touch this field.
    private int chosen = -1;
    private readonly object gate = new();
    private int count;
    public int Count { get { lock (gate) return count; } }

    public uint SetPendingName(ReadOnlySpan<byte> bytes)
    {
        lock (gate)
        {
        // Native memcpy_s oversize has CRT effects; explicit independent rejection.
        if (bytes.Length > pendingName.Length) return InvalidArgument;
        pendingName.AsSpan().Clear(); bytes.CopyTo(pendingName); return 0;
        }
    }

    public uint Add(ReadOnlySpan<byte> bytes)
    {
        lock (gate)
        {
        if (bytes.IsEmpty) return InvalidArgument;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] is not null) continue;
            slots[i] = new(pendingName.ToArray(), bytes.ToArray(), 1);
            count++; return 0;
        }
        return Full;
        }
    }

    public uint Choose(int index)
    {
        lock (gate)
        {
        // Original Choose permits 100, but later Change indexes a 100-slot array.
        // This typed interface only supports safe source-UI indices 0..99.
        if ((uint)index >= 100) return InvalidArgument;
        chosen = index; return 0;
        }
    }

    public uint Change(ReadOnlySpan<byte> bytes)
    {
        lock (gate)
        {
        if (bytes.IsEmpty || chosen < 0) return InvalidArgument;
        var old = slots[chosen];
        // Native does not increment Count or update Name/Selected here.
        slots[chosen] = new(old?.Name ?? new byte[0x104], bytes.ToArray(), old?.Selected ?? 0);
        return 0;
        }
    }

    public uint Delete(int index)
    {
        lock (gate)
        {
        if ((uint)index >= 100) return InvalidArgument;
        // Source compacts all trailing slots and decrements even if slot was empty.
        for (int i = index; i < 99; i++) slots[i] = slots[i + 1];
        slots[99] = null; count--; return 0;
        }
    }

    public uint SetSelected(int index, int value)
    {
        lock (gate)
        {
        if ((uint)value >= 2 || index < 0 || index >= count || index >= 100)
            return InvalidArgument;
        var old = slots[index];
        // Buffer absent is outside source UI domain; refuse to invent an active model.
        if (old is null) return InvalidArgument;
        slots[index] = old with { Selected = value }; return 0;
        }
    }

    public uint Rename(ReadOnlySpan<byte> bytes)
    {
        lock (gate)
        {
            if (chosen < 0 || bytes.Length > 0x104) return InvalidArgument;
            var old = slots[chosen];
            // Native empty-slot name state is outside this typed populated-slot surface.
            if (old is null) return InvalidArgument;
            byte[] name = new byte[0x104]; bytes.CopyTo(name);
            slots[chosen] = old with { Name = name }; return 0;
        }
    }

    public uint MoveUp(int index)
    {
        lock (gate)
        {
            if (index < 1 || index >= 100) return InvalidArgument;
            for (int i = index - 1; i >= 0; i--)
                if (slots[i] is not null) { (slots[i], slots[index]) = (slots[index], slots[i]); break; }
            return 0;
        }
    }

    public uint MoveDown(int index)
    {
        lock (gate)
        {
            if ((uint)index >= 99) return InvalidArgument;
            for (int i = index + 1; i < 100; i++)
                if (slots[i] is not null) { (slots[i], slots[index]) = (slots[index], slots[i]); break; }
            return 0;
        }
    }

    public void Clear() { lock (gate) { Array.Clear(slots); count = 0; } }

    public int? NextFreeIndex()
    {
        lock (gate)
        {
            for (int i = 0; i < 100; i++) if (slots[i] is null) return i;
            // Source7f40 full path returns0 but does not write output/length.
            // This typed representation reports no available value explicitly.
            return null;
        }
    }

    public (uint Code, byte[] Name, int NativeOutLength) ReadName(int index)
    {
        lock (gate)
        {
            if ((uint)index >= 100) return (InvalidArgument, [], 0);
            var slot = slots[index];
            if (slot is null) return (0, [], 0);
            int end = Array.IndexOf(slot.Name, (byte)0);
            if (end < 0) throw new NotSupportedException("Unterminated native name is outside the typed safe UI domain.");
            return (0, slot.Name[..end], end + 1);
        }
    }

    public (uint Code, byte[] Data, int RequiredLength) ReadData(int index, int capacity)
    {
        lock (gate)
        {
            // Source invalid>=100 path returns0 without writes; do not invent a value.
            if ((uint)index >= 100 || capacity < 0) return (InvalidArgument, [], 0);
            var slot = slots[index];
            if (slot is null) return (InvalidArgument, [], 0);
            if (capacity < slot.Data.Length) return (0xe0000012, [], slot.Data.Length);
            return (0, slot.Data.ToArray(), slot.Data.Length);
        }
    }

    public (uint Code, string Value, int NativeOutLength) ReadSelected(int index)
    {
        lock (gate)
        {
            if (index < 0 || index >= count || index >= 100) return (InvalidArgument, "", 0);
            var slot = slots[index];
            return slot is null ? (0u, "", 0) : (0u, slot.Selected == 0 ? "0" : "1", 1);
        }
    }

    public IReadOnlyList<(int Index, byte[] Name, byte[] Data)> ActiveSnapshot()
    {
        lock (gate)
        {
        var result = new List<(int, byte[], byte[])>();
        for (int i = 0; i < 100; i++)
            if (slots[i] is { Selected: not 0 } s)
                result.Add((i, s.Name.ToArray(), s.Data.ToArray()));
        return result;
        }
    }
}
