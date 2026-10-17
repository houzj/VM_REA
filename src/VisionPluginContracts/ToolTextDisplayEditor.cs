using System.Globalization;
using System.Windows;
using W = System.Windows.Controls;
namespace VisionPluginContracts;

/// <summary>Shared TextDisplayGroup editor reconstructed from shipped Group.xml.</summary>
public sealed class ToolTextDisplayEditor : W.StackPanel
{
    ToolTextDisplaySettings settings;
    public ToolTextDisplaySettings Settings => settings;
    public ToolTextDisplayEditor(ToolEditorContext context, ToolTextDisplaySettings initial, Action<ToolTextDisplaySettings> changed)
    {
        settings = initial;
        void Change(ToolTextDisplaySettings value) { if (!ToolDisplayService.Valid(value)) return; settings = value; changed(value); }
        void Row(string caption, FrameworkElement control)
        {
            var row = new W.Grid { MinHeight = 35 }; row.ColumnDefinitions.Add(new() { Width = new GridLength(128) }); row.ColumnDefinitions.Add(new());
            row.Children.Add(new W.TextBlock { Text = caption, VerticalAlignment = VerticalAlignment.Center, Foreground = System.Windows.Media.Brushes.DimGray });
            W.Grid.SetColumn(control, 1); row.Children.Add(control); Children.Add(row);
        }
        Children.Add(new W.TextBlock { Text = "文本显示", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 12, 0, 12) });
        var content = new W.TextBox { Name = "DisplayContent", Text = settings.Content, AcceptsReturn = true, MinHeight = 60, TextWrapping = TextWrapping.Wrap };
        content.LostKeyboardFocus += (_, _) => Change(settings with { Content = content.Text }); Row("内容", content);
        void Text(string name, string caption, string value, Func<string, ToolTextDisplaySettings> update)
        {
            var edit = new W.TextBox { Name = name, Text = value, Height = 28, VerticalContentAlignment = VerticalAlignment.Center };
            edit.LostKeyboardFocus += (_, _) => { var next = update(edit.Text); if (ToolDisplayService.Valid(next)) Change(next); else edit.Text = value; }; Row(caption, edit);
        }
        Text("DisplayOKColor", "OK颜色", settings.OkColor, value => settings with { OkColor = value });
        Text("DisplayNGColor", "NG颜色", settings.NgColor, value => settings with { NgColor = value });
        Text("DisplayCharacterSize", "字号", settings.FontSize.ToString(CultureInfo.InvariantCulture), value => double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var n) ? settings with { FontSize = n } : settings with { FontSize = double.NaN });
        Text("DisplayTransparency", "透明度", settings.Opacity.ToString(CultureInfo.InvariantCulture), value => double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var n) ? settings with { Opacity = n } : settings with { Opacity = double.NaN });
        void Position(string input, string caption, bool x)
        {
            var panel = new W.DockPanel(); var button = new W.Button { Name = "Subscribe" + input, Content = "↗", Width = 28, Height = 25 };
            button.Click += (_, _) => context.PickSubscription?.Invoke(Window.GetWindow(this), input, false, VisionFramework.DataKind.Number);
            W.DockPanel.SetDock(button, W.Dock.Right); panel.Children.Add(button);
            var edit = new W.TextBox { Name = input, Height = 28, VerticalContentAlignment = VerticalAlignment.Center, Text = context.InputNumber?.Invoke(input)?.ToString(CultureInfo.InvariantCulture) ?? (x ? settings.PositionX : settings.PositionY).ToString(CultureInfo.InvariantCulture) };
            edit.LostKeyboardFocus += (_, _) => { if (!double.TryParse(edit.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) || !double.IsFinite(value)) return; context.SetInputNumber?.Invoke(input, value); Change(x ? settings with { PositionX = value } : settings with { PositionY = value }); };
            panel.Children.Add(edit); Row(caption, panel);
        }
        Position(ToolDisplayService.PositionXInput, "位置X", true); Position(ToolDisplayService.PositionYInput, "位置Y", false);
    }
}
