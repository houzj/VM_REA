using System.Diagnostics;

namespace VisionEditor;

public partial class MainWindow
{
    // VisionMaster.Exe.Audit/app.config: OnlineHelpDoc. Keep this shell fallback
    // aligned with the inspected native configuration until an app-config seam exists.
    const string OnlineHelpDocumentUrl = "http://pinfodata.hikvision.com/analysisQR/showQR/4e994cf1";

    // An operation seam lets WPF route tests observe the real menu action without
    // launching an external browser. Production uses the Windows shell association.
    Action<string> onlineHelpLauncher = LaunchOnlineHelp;

    void OpenOnlineHelpDocument() => onlineHelpLauncher(OnlineHelpDocumentUrl);

    static void LaunchOnlineHelp(string url) => Process.Start(new ProcessStartInfo(url)
    {
        UseShellExecute = true
    });
}
