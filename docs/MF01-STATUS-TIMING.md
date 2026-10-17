# MF-01 状态栏流程耗时明细

## 源码确认

- `analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/StatusBar.xaml` 的 `ProcessTemplate` 把流程耗时放在 `ShowTimeClick` 按钮中；`TimeTemplate` 展示所选项的模块耗时和算法耗时。
- 对应 `StatusBar.xaml.cs` 的 `ShowTimeClick` 先关闭旧窗口，有所选对象且当前画布源为 `IVmModuleSet` 时显示 `ShowTimeDialog`；画布源改变会调用 `CloseTimeDialog`。
- `analysis/decompiled/VMControls.WPF/VMControls.WPF.TimeDialog/ShowTimeDialog.xaml` 包含流程耗时总计，以及按模块名称、工具耗时、算法耗时排列的列表。

## 独立实现与运行数据

- 将 WPF 主窗口状态栏的“流程耗时”改为可点击命令，弹出 `ShellTimeDialog`。窗口从当前流程的 `workflowSessions` 读取真正执行的 `NodeRunRecord.ElapsedMilliseconds`；未执行模块显示 `0.00`，重复点击关闭旧窗口，流程 Tab 切换后关闭陈旧窗口。
- 现有独立执行器只提供节点总耗时，没有原生 SDK 的独立算法耗时。因此算法列显示“—”，不把总耗时复制成算法耗时或伪称 `0.00`。流程总耗时按当前记录求和，保持本项目既有状态栏计量方式；原生调度开销另算的精确语义待取证。
- UI 由专属 WPF XAML 窗口显示，不引入 WinForms，也不把截图代码放进产品程序集。

## 操作验收及边界

`dotnet run --project tests/VisionEditor.ShellChecks/VisionEditor.ShellChecks.csproj -v:q -m:1` 创建真实 WPF 窗口，运行一条实际流程，点击状态栏并核对行数、节点耗时和算法未知标识，重复点击和切换流程验证生命周期；专项累计 14 项通过。测试渲染位于本工作树 `analysis/full-audit/shell-time-dialog.png`，已人工查看排版。此截图是独立程序，不是原生 VM 截图。

真实 VM 的颜色资源、精确像素、流程总耗时与工具/算法拆分、Group 内部选中态及原生调度路径仍未完成差分；本项仅闭合已列交互和独立运行数据，不代表状态栏整体或 VM 时间统计完整复刻。
