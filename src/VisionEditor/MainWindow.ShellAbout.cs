namespace VisionEditor;

public partial class MainWindow
{
    void ShowAbout()
    {
        var window = new ShellAboutWindow { Owner = this };
        window.ShowDialog();
    }
}
