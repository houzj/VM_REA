using System.Collections.ObjectModel;

namespace VisionEditor.Core;

public enum ShellLogKind { Info, Warning, Error, Award }

/// <summary>日志条目。字段对应原厂 LogView 的三列：类型图标、时间、内容。</summary>
public sealed record ShellLogEntry(long Seq, string Time, ShellLogKind Kind, string Event)
{
    public string KindText => Kind switch
    {
        ShellLogKind.Warning => "警告",
        ShellLogKind.Error => "错误",
        ShellLogKind.Award => "授权",
        _ => "信息"
    };
}

/// <summary>
/// 宿主日志。契约依据 W01 第 4 节：
/// 底部 180 高日志面板 + 28 高筛选栏，筛选码 1=全部 2=警告 3=错误 4=信息，默认折叠。
/// 只承载宿主侧消息，不复制 SDK 日志缓冲区；筛选只影响可见集合，不丢弃记录。
/// </summary>
public sealed class ShellLogStore
{
    public const int MaximumRecords = 500;

    readonly List<ShellLogEntry> store = new();
    readonly ObservableCollection<ShellLogEntry> visible = new();
    long sequence;

    public ShellLogStore()
    {
        Visible = new ReadOnlyObservableCollection<ShellLogEntry>(visible);
    }

    public ReadOnlyObservableCollection<ShellLogEntry> Visible { get; }

    /// <summary>全部记录（最新在前），供筛选与统计使用。</summary>
    public IReadOnlyList<ShellLogEntry> All => store;

    /// <summary>当前筛选：null 表示全部。</summary>
    public ShellLogKind? Filter { get; private set; }

    public bool IsFolded { get; private set; } = true;

    public ShellLogEntry? Latest { get; private set; }

    public void Append(ShellLogKind kind, string message, DateTime? at = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        var entry = new ShellLogEntry(++sequence,
            (at ?? DateTime.Now).ToString("HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture), kind, message);
        store.Insert(0, entry);
        Latest = entry;
        if (Filter is null || Filter == kind) visible.Insert(0, entry);
        Trim();
    }

    /// <summary>设置筛选。参数取自原厂 LogTypeCommand：1=全部 2=警告 3=错误 4=信息。</summary>
    public ShellLogKind? SetFilter(int code)
    {
        Filter = code switch
        {
            2 => ShellLogKind.Warning,
            3 => ShellLogKind.Error,
            4 => ShellLogKind.Info,
            _ => null
        };
        RebuildVisible();
        return Filter;
    }

    /// <summary>折叠开关：原厂 <c>IsLogFold = !toggleChecked</c>。</summary>
    public bool SetFolded(bool folded)
    {
        IsFolded = folded;
        return IsFolded;
    }

    public void Clear()
    {
        store.Clear();
        visible.Clear();
        sequence = 0;
        Latest = null;
    }

    void RebuildVisible()
    {
        visible.Clear();
        foreach (var entry in store)
            if (Filter is null || entry.Kind == Filter) visible.Add(entry);
    }

    void Trim()
    {
        if (store.Count > MaximumRecords) store.RemoveRange(MaximumRecords, store.Count - MaximumRecords);
        while (visible.Count > MaximumRecords) visible.RemoveAt(visible.Count - 1);
    }
}
