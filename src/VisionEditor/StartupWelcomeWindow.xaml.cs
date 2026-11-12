using System.Drawing;
using System.IO;
using System.Windows;
using FormsPictureBox = System.Windows.Forms.PictureBox;
using PictureBoxSizeMode = System.Windows.Forms.PictureBoxSizeMode;

namespace VisionEditor;

public partial class StartupWelcomeWindow : Window
{
    public StartupWelcomeWindow()
    {
        InitializeComponent();
        LoadingPicture.SizeMode = PictureBoxSizeMode.StretchImage;
        var path = Path.Combine(AppContext.BaseDirectory, "ui-assets", "original-guide", "WelcomePage.GIF");
        if (File.Exists(path)) LoadingPicture.Image = Image.FromFile(path);
    }
}
