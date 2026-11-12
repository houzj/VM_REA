using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VisionEditor;

/// <summary>
/// 原厂 <c>VisionMaster.AppMainWindow.GuideButton</c> 的独立等价实现
/// （见 docs/reverse/C01-guide-window-visual-parity-20261111.md）。
/// 只声明依赖属性，外观模板放在窗口资源中，便于审阅。
/// </summary>
public sealed class GuideCard : System.Windows.Controls.Button
{
    public static readonly DependencyProperty BackImageProperty = DependencyProperty.Register(
        nameof(BackImage), typeof(ImageSource), typeof(GuideCard), new PropertyMetadata(null));

    public static readonly DependencyProperty BackImageClickProperty = DependencyProperty.Register(
        nameof(BackImageClick), typeof(ImageSource), typeof(GuideCard), new PropertyMetadata(null));

    public static readonly DependencyProperty HelpContentProperty = DependencyProperty.Register(
        nameof(HelpContent), typeof(string), typeof(GuideCard), new PropertyMetadata(null));

    public static readonly DependencyProperty ToolBoxTypeProperty = DependencyProperty.Register(
        nameof(ToolBoxType), typeof(string), typeof(GuideCard), new PropertyMetadata(null));

    public ImageSource? BackImage
    {
        get => (ImageSource?)GetValue(BackImageProperty);
        set => SetValue(BackImageProperty, value);
    }

    public ImageSource? BackImageClick
    {
        get => (ImageSource?)GetValue(BackImageClickProperty);
        set => SetValue(BackImageClickProperty, value);
    }

    public string? HelpContent
    {
        get => (string?)GetValue(HelpContentProperty);
        set => SetValue(HelpContentProperty, value);
    }

    /// <summary>原厂取值 General / Position / Defect / Recognition。</summary>
    public string? ToolBoxType
    {
        get => (string?)GetValue(ToolBoxTypeProperty);
        set => SetValue(ToolBoxTypeProperty, value);
    }
}

/// <summary>
/// 原厂 <c>VM.Controls.IconMenuItem</c> 的独立等价实现：常态 / 悬停 / 点击三态图标。
/// 原厂另有 <c>ShowMask</c>，本项目不使用，故不实现。
/// </summary>
public sealed class GuideIconMenuItem : System.Windows.Controls.MenuItem
{
    public static readonly DependencyProperty NormalIconProperty = DependencyProperty.Register(
        nameof(NormalIcon), typeof(ImageSource), typeof(GuideIconMenuItem), new PropertyMetadata(null));

    public static readonly DependencyProperty HoverIconProperty = DependencyProperty.Register(
        nameof(HoverIcon), typeof(ImageSource), typeof(GuideIconMenuItem), new PropertyMetadata(null));

    public static readonly DependencyProperty ClickIconProperty = DependencyProperty.Register(
        nameof(ClickIcon), typeof(ImageSource), typeof(GuideIconMenuItem), new PropertyMetadata(null));

    public ImageSource? NormalIcon
    {
        get => (ImageSource?)GetValue(NormalIconProperty);
        set => SetValue(NormalIconProperty, value);
    }

    public ImageSource? HoverIcon
    {
        get => (ImageSource?)GetValue(HoverIconProperty);
        set => SetValue(HoverIconProperty, value);
    }

    public ImageSource? ClickIcon
    {
        get => (ImageSource?)GetValue(ClickIconProperty);
        set => SetValue(ClickIconProperty, value);
    }
}
