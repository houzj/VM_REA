using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfOpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace VisionEditor;

internal sealed record StartupGuideRecentItem(string Name, string Path, string Age);

internal sealed class GuideOpenCommand : ICommand
{
    readonly Action<object?> execute;
    internal GuideOpenCommand(Action<object?> execute) => this.execute = execute;
    public event EventHandler? CanExecuteChanged { add { } remove { } }
    public bool CanExecute(object? parameter) => true;
    public void Execute(object? parameter) => execute(parameter);
}

/// <summary>
/// 方案中心，对应原厂 <c>VisionMaster.AppMainWindow.GuideWindow</c>。
/// 外观契约见 docs/reverse/C01-guide-window-visual-parity-20261111.md；
/// 交互语义对应原厂 GuideWindow.xaml.cs 的事件处理器。
/// </summary>
public partial class StartupGuideWindow : Window
{
    public static readonly DependencyProperty RecentlySolutionsProperty = DependencyProperty.Register(
        nameof(RecentlySolutions), typeof(IEnumerable), typeof(StartupGuideWindow), new PropertyMetadata(null));

    public static readonly DependencyProperty OpenSolutionCommandProperty = DependencyProperty.Register(
        nameof(OpenSolutionCommand), typeof(ICommand), typeof(StartupGuideWindow), new PropertyMetadata(null));

    public IEnumerable? RecentlySolutions
    {
        get => (IEnumerable?)GetValue(RecentlySolutionsProperty);
        set => SetValue(RecentlySolutionsProperty, value);
    }

    public ICommand? OpenSolutionCommand
    {
        get => (ICommand?)GetValue(OpenSolutionCommandProperty);
        set => SetValue(OpenSolutionCommandProperty, value);
    }

    public string? SelectedPath { get; private set; }

    public bool StartNewSolution { get; private set; }

    internal string? ChosenToolBoxType { get; private set; }

    public StartupGuideWindow()
    {
        InitializeComponent();
        InjectOriginalImages();
        GuideLogo.Source = GuideResourceResolver.Image("guide_logo");
        // 原厂 GuideWindow_Loaded：buttonShow.IsChecked = !GuideHelper.IsGuideWindowShow()
        HideGuide.IsChecked = !StartupSettings.ShowEnable;
        RecentlySolutions = Project(RecentSolutionStore.Read());
        OpenSolutionCommand = new GuideOpenCommand(ExecuteOpen);
        DataContext = this;
    }

    /// <summary>把原厂 g.resources 条目注入动态资源键，模板经 DynamicResource 解析。</summary>
    void InjectOriginalImages()
    {
        void Put(string key, string name)
        {
            var image = GuideResourceResolver.Image(name);
            if (image is not null) Resources[key] = image;
        }

        Put("Img_CardGeneral", "guide_general");
        Put("Img_CardGeneralClick", "guide_general_click");
        Put("Img_CardPosition", "guide_position");
        Put("Img_CardPositionClick", "guide_position_click");
        Put("Img_CardDefect", "guide_defect");
        Put("Img_CardDefectClick", "guide_defect_click");
        Put("Img_CardRecognition", "guide_recognition");
        Put("Img_CardRecognitionClick", "guide_recognition_click");
        Put("Img_CheckOff", "guide_check-box_off");
        Put("Img_CheckOn", "guide_check-box_on");
        Put("Img_OpenNormal", "guide_open_normal");
        Put("Img_OpenClick", "guide_open_click");
        Put("Img_Ic1Normal", "ic_1_normal");
        Put("Img_Ic1Click", "ic_1_click");
        Put("Img_Ic2Normal", "ic_2_normal");
        Put("Img_Ic2Click", "ic_2_click");
        Put("Img_Ic3Normal", "ic_3_normal");
        Put("Img_Ic3Click", "ic_3_click");
    }

    /// <summary>
    /// 最近方案投影：方案名去扩展名 + 完整路径 + 相对时间。
    /// 原厂绑定 <c>FileName</c> / <c>SolutionPath</c> / <c>LastTimeStr</c>；
    /// 相对时间文案属于独立实现（原厂格式化代码未取到）。
    /// </summary>
    static IList Project(IReadOnlyList<RecentSolutionStore.Entry> entries)
    {
        var now = DateTime.Now;
        return entries
            .Where(entry => File.Exists(entry.Path))
            .Select(entry => new StartupGuideRecentItem(
                Path.GetFileNameWithoutExtension(entry.Path),
                entry.Path,
                Age(entry.LastTime, now)))
            .ToArray();
    }

    static string Age(DateTime time, DateTime now)
    {
        var days = Math.Max(0, (now.Date - time.Date).Days);
        return days == 0 ? "今天" : days + "天前";
    }

    void ExecuteOpen(object? parameter)
    {
        if (parameter is string path && File.Exists(path))
        {
            Choose(path);
            return;
        }
        var dialog = new WpfOpenFileDialog
        {
            Title = "打开方案",
            Filter = "流程方案|*.vmx.json;*.json;*.sol",
            CheckFileExists = true
        };
        if (dialog.ShowDialog(this) == true) Choose(dialog.FileName);
        else Close();
    }

    void Choose(string? path)
    {
        SelectedPath = path;
        StartNewSolution = path is null;
        Close();
    }

    void TitleBarMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState != MouseButtonState.Pressed) return;
        try
        {
            DragMove();
        }
        catch (InvalidOperationException)
        {
            // 未按下主键时 DragMove 会抛出，忽略即可。
        }
    }

    /// <summary>原厂 GuideButton_OnClick：写 ToolBoxType 配置后关闭窗口。</summary>
    void GuideCardClick(object sender, RoutedEventArgs e)
    {
        if (sender is not GuideCard card) return;
        ChosenToolBoxType = card.ToolBoxType;
        StartupSettings.SetToolBoxType(card.ToolBoxType ?? "General");
        StartNewSolution = false;
        Close();
    }

    void RecentDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (RecentList.SelectedItem is StartupGuideRecentItem item) OpenSolutionCommand?.Execute(item.Path);
    }

    void OpenFileClick(object sender, RoutedEventArgs e)
    {
        OpenSolutionCommand?.Execute(null);
        Close();
    }

    void OpenDemoClick(object sender, RoutedEventArgs e)
    {
        // 原厂 GuideHelper.OpenSolution(opendemo: true, out filename) 指向安装包示例目录；
        // 本项目无该目录，映射到仓库 examples（独立实现）。
        const string name = "image-source-plugin.vmx.json";
        var path = Path.Combine(AppContext.BaseDirectory, name);
        if (!File.Exists(path))
            path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "examples", name));
        if (File.Exists(path)) OpenSolutionCommand?.Execute(path);
        Close();
    }

    void LocalHelpClick(object sender, RoutedEventArgs e)
    {
        // 原厂 GuideHelper.GetHelp(local: true) 需要安装包 Help 目录，本项目未随附。
    }

    void OnlineHelpClick(object sender, RoutedEventArgs e)
    {
        // 原厂 GuideHelper.GetHelp(local: false) 打开在线帮助页，本项目未接入。
    }

    /// <summary>原厂 DoNotShow_OnChecked / OnUnchecked：写 ShowEnable 单开关。</summary>
    void HideGuideChecked(object sender, RoutedEventArgs e) => StartupSettings.SetShowEnable(false);

    void HideGuideUnchecked(object sender, RoutedEventArgs e) => StartupSettings.SetShowEnable(true);

    void CloseClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }
}
