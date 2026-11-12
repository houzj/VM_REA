namespace VisionEditor;

/// <summary>
/// 主窗口宿主侧接线，对应原厂 <c>VisionMasterMain.MainWindow</c> 的
/// <c>IsShowProgressMask</c> / <c>MaskMessage</c> 依赖属性与 <c>ProgressMask</c> 遮罩层
/// （契约 W02 / W04 / W05）。
/// </summary>
public partial class MainWindow
{
    public static readonly System.Windows.DependencyProperty IsShowProgressMaskProperty = System.Windows.DependencyProperty.Register(
        nameof(IsShowProgressMask), typeof(bool), typeof(MainWindow), new System.Windows.PropertyMetadata(false, OnIsShowProgressMaskChanged));

    public static readonly System.Windows.DependencyProperty MaskMessageProperty = System.Windows.DependencyProperty.Register(
        nameof(MaskMessage), typeof(string), typeof(MainWindow), new System.Windows.PropertyMetadata(null));

    System.Windows.Controls.Border? progressMask;

    public bool IsShowProgressMask
    {
        get => (bool)GetValue(IsShowProgressMaskProperty);
        set => SetValue(IsShowProgressMaskProperty, value);
    }

    public string? MaskMessage
    {
        get => (string?)GetValue(MaskMessageProperty);
        set => SetValue(MaskMessageProperty, value);
    }

    static void OnIsShowProgressMaskChanged(System.Windows.DependencyObject target, System.Windows.DependencyPropertyChangedEventArgs args)
    {
        var window = (MainWindow)target;
        if (args.NewValue is true) window.AddProgressMask();
        else window.RemoveProgressMask();
    }

    void AddProgressMask()
    {
        RemoveProgressMask();
        var message = new System.Windows.Controls.TextBlock
        {
            Foreground = System.Windows.Media.Brushes.White,
            FontSize = 16,
            HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
            VerticalAlignment = System.Windows.VerticalAlignment.Center
        };
        message.SetBinding(System.Windows.Controls.TextBlock.TextProperty,
            new System.Windows.Data.Binding(nameof(MaskMessage)) { Source = this });
        progressMask = new System.Windows.Controls.Border
        {
            Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(170, 20, 20, 20)),
            Child = message
        };
        System.Windows.Controls.Panel.SetZIndex(progressMask, 1000);
        ShellRoot.Children.Add(progressMask);
    }

    void RemoveProgressMask()
    {
        if (progressMask is null) return;
        ShellRoot.Children.Remove(progressMask);
        progressMask = null;
    }

    /// <summary>
    /// 授权提醒骨架，对应原厂 <c>Program.IsNeedRemind</c> 弹窗与
    /// <c>Program.StartAuthActivation</c> 调用点（契约 S09）。本轮不读取加密狗。
    /// </summary>
    internal void ShowAuthorizationReminder() => StatusText.Text = "授权提醒：当前为开发模式，未启用授权校验";
}
