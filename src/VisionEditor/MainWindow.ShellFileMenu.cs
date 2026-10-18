using MenuItem = System.Windows.Controls.MenuItem;
using Separator = System.Windows.Controls.Separator;

namespace VisionEditor;

public partial class MainWindow
{
    void BuildFileMenu()
    {
        var fileMenu = Menu("文件", ("新建方案", New), ("打开方案", Open));
        fileMenu.Style = (System.Windows.Style)FindResource("ShellFileMenuRoot");
        fileMenu.Resources[typeof(MenuItem)] = FindResource("ShellFileMenuItem");
        fileMenu.Resources[typeof(Separator)] = FindResource("ShellFileMenuSeparator");
        InitializeRecentSolutionsMenu(fileMenu);
        foreach (var item in fileMenu.Items.OfType<MenuItem>())
            item.Style = (System.Windows.Style)FindResource("ShellFileMenuItem");
        fileMenu.Items.Add(new Separator());
        AddFileCommand(fileMenu, "打开示例", Demo);
        fileMenu.Items.Add(new Separator());
        AddFileCommand(fileMenu, "保存", () => Save(false));
        AddFileCommand(fileMenu, "另存为", () => Save(true));
        // Import and algorithm export have no corresponding framework runtime yet.
        fileMenu.Items.Add(new Separator());
        AddFileCommand(fileMenu, "退出", Close);
    }

    void AddFileCommand(MenuItem parent, string title, Action action)
    {
        var item = new MenuItem { Header = title, Style = (System.Windows.Style)FindResource("ShellFileMenuItem") };
        item.Click += (_, _) => Safe(action);
        parent.Items.Add(item);
    }
}
