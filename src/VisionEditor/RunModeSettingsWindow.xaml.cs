using System.Windows;
using System.Windows.Controls;

namespace VisionEditor;

public partial class RunModeSettingsWindow : Window
{
    public int SelectedMode => ModeSelector.SelectedIndex;

    public RunModeSettingsWindow(int mode)
    {
        InitializeComponent();
        ModeSelector.SelectedIndex = mode is 0 or 1 ? mode : 0;
        UpdateDescription();
    }

    private void ModeSelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateDescription();

    private void UpdateDescription()
    {
        if (ModeDescription is null || ModeSelector is null) return;
        // The native page binds localized tips, but those resource values are absent
        // from the decompiled resource set; leave that source-owned text unresolved.
        ModeDescription.Text = string.Empty;
    }

    private void ApplyClick(object sender, RoutedEventArgs e) => DialogResult = true;
    private void CancelClick(object sender, RoutedEventArgs e) => DialogResult = false;
}
