using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using VisionPluginContracts;
using Application = System.Windows.Application;
using Brushes = System.Windows.Media.Brushes;
using Button = System.Windows.Controls.Button;
using Color = System.Windows.Media.Color;
using FlowDirection = System.Windows.FlowDirection;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Image = System.Windows.Controls.Image;
using MessageBox = System.Windows.MessageBox;
using Orientation = System.Windows.Controls.Orientation;
using Panel = System.Windows.Controls.Panel;

[assembly: InternalsVisibleTo("RotateCalibration.Checks")]

namespace VisionPlugins.Calibration;

/// <summary>RotateCalibration-only subset of the original HikMessageBox error path.</summary>
internal static class RotateCalibrationFileErrorDialog
{
    internal static Window CreateWindow(string message, string confirmText)
    {
        var window = new Window
        {
            Title = string.Empty,
            Width = 290,
            Height = 156,
            Topmost = true,
            ShowInTaskbar = false,
            WindowStyle = WindowStyle.None,
            ResizeMode = ResizeMode.NoResize,
            Background = new SolidColorBrush(Color.FromRgb(0xf2, 0xf3, 0xf5)),
            Name = "RotateCalibrationFileErrorDialog"
        };

        var outer = new Border
        {
            BorderBrush = new SolidColorBrush(Color.FromRgb(0x8f, 0x8b, 0x88)),
            BorderThickness = new Thickness(1),
            Background = window.Background
        };
        var root = new Grid();
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(24) });
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        outer.Child = root;

        var titleBar = new Grid { Background = window.Background, Name = "TitleBar" };
        var close = new Button
        {
            Name = "CloseButton",
            Width = 24,
            Height = 24,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(0, 0, 5, 0),
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Padding = new Thickness(0),
            ToolTip = "关闭"
        };
        var closeImage = new Image
        {
            Name = "CloseIcon",
            Stretch = Stretch.None,
            Source = ResourceImage("RotateCalibrationCloseNormal.png")
        };
        var closeTemplate = new ControlTemplate(typeof(Button));
        var closeHost = new FrameworkElementFactory(typeof(Grid), "closeHost");
        closeHost.SetValue(Panel.BackgroundProperty, Brushes.Transparent);
        var closeVisual = new FrameworkElementFactory(typeof(Image), "closeIcon");
        closeVisual.SetValue(Image.StretchProperty, Stretch.None);
        closeVisual.SetValue(Image.SourceProperty, ResourceImage("RotateCalibrationCloseNormal.png"));
        closeHost.AppendChild(closeVisual);
        closeTemplate.VisualTree = closeHost;
        var hover = new Trigger { Property = UIElement.IsMouseOverProperty, Value = true };
        hover.Setters.Add(new Setter(Image.SourceProperty, ResourceImage("RotateCalibrationCloseClick.png"), "closeIcon"));
        closeTemplate.Triggers.Add(hover);
        var pressed = new Trigger { Property = System.Windows.Controls.Primitives.ButtonBase.IsPressedProperty, Value = true };
        pressed.Setters.Add(new Setter(Image.SourceProperty, ResourceImage("RotateCalibrationCloseClick.png"), "closeIcon"));
        closeTemplate.Triggers.Add(pressed);
        close.Template = closeTemplate;
        close.Click += (_, _) => window.Close();
        titleBar.Children.Add(close);
        Grid.SetRow(titleBar, 0);
        root.Children.Add(titleBar);

        var content = new Grid();
        content.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        content.RowDefinitions.Add(new RowDefinition { Height = new GridLength(55) });
        content.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        var heading = new StackPanel
        {
            Name = "TitleContent",
            Orientation = Orientation.Horizontal,
            Height = 42,
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        heading.Children.Add(new Image
        {
            Name = "ErrorIcon",
            Stretch = Stretch.None,
            Source = ResourceImage("RotateCalibrationError.png"),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center
        });
        content.Children.Add(heading);

        var messageViewer = new ScrollViewer
        {
            Name = "MessageViewer",
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            MinHeight = 30,
            MaxHeight = 35,
            Margin = new Thickness(0, 11, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Top,
            Content = new TextBlock
            {
                Name = "MessageText",
                Text = message,
                FontSize = 12,
                Foreground = new SolidColorBrush(Color.FromRgb(0x66, 0x66, 0x66)),
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Center,
                MaxWidth = 254
            }
        };
        Grid.SetRow(messageViewer, 1);
        content.Children.Add(messageViewer);

        var buttons = new StackPanel
        {
            Name = "Buttons",
            Orientation = Orientation.Horizontal,
            FlowDirection = FlowDirection.RightToLeft,
            VerticalAlignment = VerticalAlignment.Bottom,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 0, 0, 12)
        };
        var confirm = new Button
        {
            Name = "ConfirmButton",
            Content = confirmText,
            Width = 86,
            Height = 24,
            Margin = new Thickness(9, 0, 9, 0),
            IsDefault = true
        };
        confirm.Click += (_, _) => window.DialogResult = true;
        buttons.Children.Add(confirm);
        Grid.SetRow(buttons, 2);
        content.Children.Add(buttons);

        Grid.SetRow(content, 1);
        root.Children.Add(content);
        window.Content = outer;
        return window;
    }

    internal static void Show(string message, string confirmText, ToolEditorMode mode)
    {
        if (mode == ToolEditorMode.SdkEmbedded) return;
        var application = Application.Current;
        if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
        {
            if (application is null)
            {
                MessageBox.Show(message);
                return;
            }
            application.Dispatcher.Invoke(() =>
            {
                var window = CreateWindow(message, confirmText);
                if (application.MainWindow?.IsActive == true) window.Owner = application.MainWindow;
                window.ShowDialog();
            });
            return;
        }
        CreateWindow(message, confirmText).ShowDialog();
    }

    static BitmapImage ResourceImage(string name) => new(new Uri($"pack://application:,,,/VisionPlugins.Calibration;component/Resources/{name}"));
}
