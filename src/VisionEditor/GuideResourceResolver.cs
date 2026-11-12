using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VisionEditor;

/// <summary>
/// 原厂方案中心 UI 资源的统一查找与缓存。
/// 资源来自已安装 VisionMasterMain.dll 的 g.resources 提取结果
/// （analysis/ui-assets/original-guide/），运行时按文件名加载。
/// 原厂条目 <c>guide_open_click .png</c> 在 click 与扩展名之间带一个空格，
/// 这里用别名表吸收该怪癖，不外泄到调用方。
/// </summary>
internal static class GuideResourceResolver
{
    static readonly Dictionary<string, ImageSource?> Cache = new(StringComparer.OrdinalIgnoreCase);
    static readonly object Gate = new();

    static readonly Dictionary<string, string> RawNames = new(StringComparer.OrdinalIgnoreCase)
    {
        ["guide_open_click"] = "guide_open_click "
    };

    internal static string Root { get; set; } = Path.Combine(AppContext.BaseDirectory, "ui-assets", "original-guide");

    /// <summary>按不含扩展名的逻辑名加载资源，缺失或损坏返回 null。</summary>
    internal static ImageSource? Image(string name)
    {
        lock (Gate)
        {
            if (Cache.TryGetValue(name, out var cached)) return cached;
        }
        var loaded = Load(name);
        lock (Gate)
        {
            Cache[name] = loaded;
        }
        return loaded;
    }

    /// <summary>悬停态：先找 <c>{name}_click</c>，缺失时回退常态图。</summary>
    internal static ImageSource? Hover(string name) => Image(name + "_click") ?? Image(name);

    /// <summary>清空缓存，供测试切换资源根目录后重新加载。</summary>
    internal static void ResetCache()
    {
        lock (Gate)
        {
            Cache.Clear();
        }
    }

    internal static IEnumerable<string> CandidateNames(string name)
    {
        if (RawNames.TryGetValue(name, out var raw)) yield return raw;
        yield return name;
    }

    static ImageSource? Load(string name)
    {
        foreach (var candidate in CandidateNames(name))
        {
            var path = Path.Combine(Root, candidate + ".png");
            if (!File.Exists(path)) continue;
            try
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(path, UriKind.Absolute);
                image.EndInit();
                image.Freeze();
                return image;
            }
            catch (Exception error) when (error is IOException or NotSupportedException or UriFormatException)
            {
                // 单张图片损坏只降级，不阻断窗口打开。
            }
        }
        return null;
    }
}
