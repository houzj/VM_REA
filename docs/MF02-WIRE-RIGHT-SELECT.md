# MF-02 连线右键选中与删除菜单

原始 `analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/Connection.xaml.cs` 的 `OnPreviewMouseDown` 未按鼠标键过滤：它取得焦点、生成中点柄，并标记事件已处理。`base/connection/connectionstyle.xaml` 为连接控件设置一个“删除”菜单项，绑定 `DeleteConnectionCommand`；`Connection.xaml` 另有焦点范围内的 Delete KeyBinding。此前本地连线仅在左键按下时选中，右键菜单文案也多加了“连线”二字。

本轮把命中层选择事件移到 `PreviewMouseDown`，所以右键按下也会选中连线并显示中点柄；菜单文案改为“删除”。独立 WPF 检查 `tests/VisionEditor.WireFocusChecks` 先验证既有左键选择、转焦、中点拖动、撤销/重做和保存，再经 WPF 预览右键事件确认连线取得焦点与中点柄显示，打开控件已绑定的菜单并点击“删除”，验证连线删除事务、撤销和保存重开。截图 `analysis/full-audit/wire-right-click-menu.png` 已目检菜单控件；截图实现只存在测试工程。

测试环境中合成 `PostMessage` / 路由右键释放均未自动唤起独立 Popup HWND，系统级注入也未稳定送达测试窗口。原始源码证明菜单绑定和右键按下选中，但桌面真实右键释放自动弹出菜单的效果仍需在 VM 和本地程序同屏实测。产品端未添加与原始事件树无依据的强制弹窗逻辑。
