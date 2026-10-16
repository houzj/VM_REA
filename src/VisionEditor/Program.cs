using System.Windows;
namespace VisionEditor;
static class Program {
 [STAThread] static void Main(){var app=new System.Windows.Application{ShutdownMode=ShutdownMode.OnMainWindowClose};app.Run(new MainWindow());}
}

