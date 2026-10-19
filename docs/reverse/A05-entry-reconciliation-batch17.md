# 第十七批 A05.1：完整入口资源对账及真实未实施范围

本批不重复已完成运行/撤销/画布实施，不创建外部服务假窗口，不修改主窗口本体或tool-plugins。交付可执行逐项对账工具而非重新描述几个旧专题。

## 固定有限分母

完整遍历原 `VisionMaster.Exe.Audit/menuitems.xml`，含普通与隐藏工具栏、五个 `ExternTitleMenueItemData`，共53具名节点、43唯一可执行resolver、15工具栏映射、16隐藏具名节点。每项保全所有原字段（执行、动态项、custom data初始化、可见/运行标志、图标、优先级、外部info路径）、完整层级、原方法声明路径/行/哈希及当前实现锚点或精确缺口。

当前对照：18个具名节点已有独立本地入口、1个相机后端部分接入、27个未实施、7个结构容器。节点数不等于不同服务数（锁定子项等重复保留），更不是96项实施进度的新分母。完整逐项表见 `A05-full-menu-map-batch17.md`；JSON和键盘候选索引在本批A/inventory。

未实施集中在：权限/软件/方案/采集策略；算法导出/原流程导入；原日志/全局变量/脚本/通信/触发/光源；标定引导/前端设计；语言/本地帮助/更多版本/V社区/欢迎页；历史下拉明细；五个外部工具及相关可用性初始化。禁用按钮、已有工具插件或普通Undo不替代这些服务。

## 原快捷键的载体认证

旧 `VisionMasterMain.MainWindow` 的PreviewKeyDown看似空stub，本批从实际 `Applications/myLibs/VisionMasterMain.dll` 用现有dotnet-ildasm静态重取：`BaseWindow_PreviewKeyDown` RVA0x818C，代码确为2-byte `nop; ret`；WPF连接器确实订阅该handler。原DLL SHA256 `319618b9f82dfc30546c900c1c19a665e0fce1d87d2a7ffa7bb4345915b180c1`。因此不能为此窗口臆造F5/CtrlS等全局热键。

第一次按 `VisionMaster.exe --item MainWindow` 导出0字节，是错误载体选择，不作为类型不存在的证据。实际exe SHA256 `42e2437780ec21c9d1ade2d07ace0a26635961f5c90559934a7fde7e233e0c23`。两个输出均保留。

原 `VM.Controls.BaseWindow` 构造绑定四个SystemCommands（Close/Maximize/Minimize/Restore）。画布的ItemsPage/RectDragPanal/DiagramItem存在带focus、lock、params和权限门槛的CtrlV/CtrlC/CtrlF/CtrlA/Delete，ZoomArea还有Ctrl/Shift鼠标修饰键；不归并成无门槛主窗口快捷键。可执行工具保全四个源码树中的全部词法键盘候选，供逐消费者核查；候选不是操作通过数。SDK/脚本触发以及控件编辑按各服务范围继续单列。

## 实施决策与可复验

SigRun/MutiRun/StopRun已经由当前toolbar接到真实Run/StartContinuous/Cancel。原XML这些在隐藏工具栏分支；再往本地工具菜单追加一套只会重复旧实施，不能推进完成。其余未实施入口需要上列真实服务或数据合同，本批没有以空窗口骗完整。

`tests/MenuEntryInventory17/Reconcile.py`：全树遍历且要求每个原命令被明确分类、所有原resolver找到方法声明、所有声称本地接入的锚点确实存在；源集变动/漏映射/锚点失效会失败。最终运行退出0。首次Save返回类型锚点误写void（实际bool）及遗漏Extern resolver的检查失败均记录在attempts，不把失败输出算成功。本批未构建产品或执行原厂程序；仅新增静态证据与对账工具。

A05.1若按实际全部入口实施仍为partial，不能把53行审计齐全当成27个未实施节点已交付；source状态也需按快捷键继承/服务的声明边界由根线审定，不虚报全程序完全覆盖。
