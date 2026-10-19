namespace VisionEditor;

public partial class MainWindow
{
    private void OpenRunModeSettings()
    {
        var dialog = new RunModeSettingsWindow(runningModeIndex) { Owner = this };
        if (dialog.ShowDialog() != true) return;

        // Source evidence: ProcedureRunData.SetRunPolicyConfig persists index
        // 1 for diagnostics, 0 otherwise, then sets IsNormalMode accordingly.
        runningModeIndex = dialog.SelectedMode == 1 ? 1 : 0;
        ShellRunningModePreference.Write(runningModeIndex);
        shellIsNormalMode = runningModeIndex != 1;
    }
}
