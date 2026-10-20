# 第二十六轮 A05.1 壳入口收口审查

## 身份与范围

- 子任务：A05.1（完整菜单、工具栏、快捷键入口清点）；本轮基线 `ea64cb0e573a9e658e656c3eebc951e9daf8a514`。
- 原始证据沿用第十七轮 `A05-full-menu-map-batch17.md`、`tests/MenuEntryInventory17/Reconcile.py` 及 `analysis/full-audit/batch17-20261019/A`。本轮没有启动原厂程序、没有触碰安装原件，也没有访问 `tool-plugins`。
- 本轮只尝试关闭仍有源码依据的入口清点/壳实现，不把静态清单或人工桌面验收计为实施单元。

## 入口到执行链核对

当前 main 的已接入入口仍可从以下成员追到实际动作：

| 原入口 | main 入口 | 当前结论 |
|---|---|---|
| `CreateNewSolution` / `OpenSolution` | `MainWindow.xaml.cs`、`MainWindow.ShellOpenSolution.cs` | 已有独立本地动作；打开前清连续运行、文件选择后记忆目录、待保存确认、加载并刷新最近项。 |
| `SaveSolution` / `SaveAsSolution` | `MainWindow.ShellSaveSolution.cs` | 已有保存/另存文件链。 |
| `Undo` / `Redo` | `MainWindow.ShellHistory.cs` | 已进入统一历史守卫；编辑器打开、锁定、运行态按现有源码分支拦截。原 `UndoList` 明细下拉仍无源码合同。 |
| `SigRun` / `MutiRun` / `StopRun` | `MainWindow.xaml.cs` | 已连接真实运行、连续运行和取消动作；不是本轮新增。 |
| `OnlineHelpDoc` / `VerInfo` / `Exit` | `MainWindow.ShellHelp.cs`、`MainWindow.ShellAbout.cs`、`MainWindow.ShellFileMenu.cs` | 已有本地壳动作。 |

原资源中剩余 resolver 仍要求外部服务或未取得的原生数据合同：算法导出、流程导入、权限/软件/方案/采集设置、日志、通信、光源、全局变量/脚本/触发器、语言刷新、本地帮助、历史明细、综合配置、前端设计器及外部工具检查/启动。空菜单项、禁用按钮或假窗口不能闭合这些入口；这些缺口与 A05.3 服务门槛报告一致。

窗口 `VisionMasterMain.MainWindow.BaseWindow_PreviewKeyDown` 的原始 CIL 仍为 `nop; ret`（RVA `0x818C`，原库 SHA 在第十七轮审计中记录），因此没有证据为主窗口补 F5/Ctrl+S 等快捷键。画布局部快捷键继续由 `WorkflowCanvas` 的焦点/锁定分支独立负责，不能上提为全局壳入口。

## 可复验操作

`Reconcile.py` 是既有静态对账探针，要求原 `menuitems.xml` 的 53 个具名节点、43 个 resolver、工具栏/隐藏节点和每个当前 main 锚点均存在，并拒绝未分类 resolver。当前环境的 `python.exe` 是 WindowsApps 占位路径，`py -3` 也报告未安装 Python，因此本轮未能重跑探针；该失败已写入本轮审计 JSON，不把历史退出 0 冒充本轮验证。原探针代码和第十七轮输出未修改。

## 结论

本轮没有发现可以在不引入未证服务合同的情况下独立闭合的 A05.1 实施单元。现有本地入口属于历史已实施内容；剩余入口的源码链在外部服务、原生模块/配置或 A06 设计器交付处断开。**实施增量 +0，不计入 96 项总进度。**

解除条件：取得对应原始服务的完整入口→状态/权限→会话或 native 调用→错误/关闭链，或用户明确选择独立兼容合同后再实施；不能用静态清点、截图或禁用菜单替代。

