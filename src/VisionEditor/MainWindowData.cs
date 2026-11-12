using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;

namespace VisionEditor;

/// <summary>
/// 宿主数据容器，对应原厂 <c>VisionMaster.MainWindowData</c> 中被启动链使用的部分
/// （IsShowProgressMask / MaskMessage / ConfigZoomIndex / IsNormalMode /
/// PreLoad / RecoverMainWindowState，见契约 S04 / S08 / W04）。
/// 菜单、方案树等原厂字段不在此列（契约 U08 未闭合）。
/// </summary>
internal sealed class MainWindowData : INotifyPropertyChanged
{
    bool isShowProgressMask;
    string? maskMessage;
    double configZoomIndex = 1.0;
    bool isNormalMode = true;
    WindowState mainWindowState = WindowState.Normal;

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool IsShowProgressMask
    {
        get => isShowProgressMask;
        set
        {
            if (isShowProgressMask == value) return;
            isShowProgressMask = value;
            OnPropertyChanged();
        }
    }

    public string? MaskMessage
    {
        get => maskMessage;
        set
        {
            if (maskMessage == value) return;
            maskMessage = value;
            OnPropertyChanged();
        }
    }

    public double ConfigZoomIndex
    {
        get => configZoomIndex;
        set
        {
            if (Math.Abs(configZoomIndex - value) < double.Epsilon) return;
            configZoomIndex = value;
            OnPropertyChanged();
        }
    }

    public bool IsNormalMode
    {
        get => isNormalMode;
        set
        {
            if (isNormalMode == value) return;
            isNormalMode = value;
            OnPropertyChanged();
        }
    }

    internal WindowState MainWindowState => mainWindowState;

    internal static string StatePath { get; set; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VisionEditor", "window-state.txt");

    /// <summary>
    /// 对应原厂 <c>MainWindowData.PreLoad</c> 中与启动壳相关的部分：
    /// 运行模式、缩放档位与窗口状态。原厂的菜单 XML 与工具箱数据不在本实现范围。
    /// </summary>
    internal void PreLoad()
    {
        ConfigZoomIndex = StartupSettings.ZoomIndex;
        IsNormalMode = StartupSettings.RunningMode != 1;
        mainWindowState = ReadWindowState();
    }

    internal void ShowMask(string? message)
    {
        MaskMessage = message;
        IsShowProgressMask = true;
    }

    internal void HideMask() => IsShowProgressMask = false;

    /// <summary>
    /// 对应原厂 <c>RecoverMainWindowState</c>：只恢复 <see cref="WindowState"/>，
    /// 不恢复位置与尺寸（原始成员体已确认，见契约 S08）。
    /// </summary>
    internal void RecoverMainWindowState(Window window)
    {
        if (window.WindowState != mainWindowState) window.WindowState = mainWindowState;
    }

    internal void SaveWindowState(Window window)
    {
        mainWindowState = window.WindowState;
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(StatePath)!);
            File.WriteAllText(StatePath, mainWindowState.ToString());
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException)
        {
            // 状态保存失败不影响关闭流程。
        }
    }

    static WindowState ReadWindowState()
    {
        try
        {
            if (!File.Exists(StatePath)) return WindowState.Normal;
            var text = File.ReadAllText(StatePath).Trim();
            return Enum.TryParse<WindowState>(text, ignoreCase: true, out var state) ? state : WindowState.Normal;
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException)
        {
            return WindowState.Normal;
        }
    }

    void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
