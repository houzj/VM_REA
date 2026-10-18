using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using VisionEditor;
using VisionEditor.Core;

internal static class Program
{
[STAThread]
private static void Main()
{
var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
var window = new MainWindow { ShowInTaskbar = false, ShowActivated = false, Left = -20000, Top = -20000 };
try
{
    window.Show();
    window.Dispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);
    var document = (GraphEditorDocument)typeof(MainWindow).GetField("doc", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(window)!;
    var editorFlag = typeof(MainWindow).GetField("parameterEditorOpen", BindingFlags.NonPublic | BindingFlags.Instance)!;
    var status = (TextBlock)window.FindName("StatusText")!;
    var toolbar = (StackPanel)window.FindName("ToolBar")!;
    var menu = (Menu)window.FindName("MenuHost")!;
    var undo = toolbar.Children.OfType<Button>().Single(b => Equals(b.ToolTip, "撤销"));
    var redo = toolbar.Children.OfType<Button>().Single(b => Equals(b.ToolTip, "重做"));
    var tools = menu.Items.OfType<MenuItem>().Single(b => Equals(b.Header, "工具"));
    var undoMenu = tools.Items.OfType<MenuItem>().Single(i => Equals(i.Header, "撤销"));
    var redoMenu = tools.Items.OfType<MenuItem>().Single(i => Equals(i.Header, "重做"));
    var node = document.CurrentProcedure.Nodes.First();
    var originalName = node.Name;
    document.SetNodeName(node.Id, originalName + "_MF01");
    var revision = document.Revision;
    if (!document.CanUndo) throw new Exception("预置编辑没有产生可撤销历史");
    editorFlag.SetValue(window, true);
    foreach (var control in new Control[] { undo, redo, undoMenu, redoMenu })
    {
        control.RaiseEvent(new RoutedEventArgs(control is MenuItem ? MenuItem.ClickEvent : ButtonBase.ClickEvent, control));
        if (document.Revision != revision || status.Text != "打开模块配置时，不能执行撤消重做")
            throw new Exception($"配置窗口打开时 {control.GetType().Name} 仍然更改历史或没有提示");
    }
    Console.WriteLine("PASS: 菜单与工具栏撤销/重做在配置窗口打开时均拒绝执行并提示");
    editorFlag.SetValue(window, false);
    undo.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, undo));
    if (document.CurrentProcedure.Nodes.First(n => n.Id == node.Id).Name != originalName)
        throw new Exception("关闭配置窗口后工具栏撤销没有恢复实际节点名称");
    redoMenu.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent, redoMenu));
    if (document.CurrentProcedure.Nodes.First(n => n.Id == node.Id).Name != originalName + "_MF01")
        throw new Exception("关闭配置窗口后菜单重做没有重新应用节点名称");
    Console.WriteLine("PASS: 关闭配置窗口后工具栏撤销及菜单重做均作用于实际节点名称");
}
finally { window.Close(); app.Shutdown(); }
}
}
