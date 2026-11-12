using System.Windows;
using VisionEditor.Core;
using RadioButton = System.Windows.Controls.RadioButton;

namespace VisionEditor;

/// <summary>
/// 底部日志区。契约：W01 第 4 节——LogView 180 高 + 28 高筛选栏，
/// 筛选码 1=全部 2=警告 3=错误 4=信息，默认折叠，折叠时只在栏上显示最近一条。
/// </summary>
public partial class MainWindow
{
    readonly ShellLogStore shellLog = new();

    /// <summary>测试用：日志存储。</summary>
    internal ShellLogStore LogStore => shellLog;

    void InitializeShellLog()
    {
        LogGrid.ItemsSource = shellLog.Visible;
        LogFoldButton.IsChecked = false;
        ApplyLogFold();
        AppendShellLog(ShellLogKind.Info, "工作区已就绪");
    }

    internal void AppendShellLog(ShellLogKind kind, string message)
    {
        shellLog.Append(kind, message);
        var latest = shellLog.Latest;
        if (latest is not null) LogLatestText.Text = latest.Time + "  " + latest.KindText + "  " + latest.Event;
    }

    void LogFilterClick(object sender, RoutedEventArgs e)
    {
        if (sender is not RadioButton { Tag: string code } || !int.TryParse(code, out var value)) return;
        shellLog.SetFilter(value);
    }

    void LogFoldClick(object sender, RoutedEventArgs e)
    {
        // 原厂 IsLogFold = !toggleChecked：勾选表示展开。
        shellLog.SetFolded(LogFoldButton.IsChecked != true);
        ApplyLogFold();
    }

    void ApplyLogFold()
    {
        LogPanel.Visibility = shellLog.IsFolded ? Visibility.Collapsed : Visibility.Visible;
    }
}
