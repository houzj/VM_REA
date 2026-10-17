# 双线逆向调度记录（2026-10-17）

本记录由 `main` 维护。当前只有框架/集成线和 `tool-plugins` 插件线两个产品工作目录；`framework` 旧分支不承接新任务。交付与文件锁以 [节点插件与框架边界](PLUGIN-FRAMEWORK-BOUNDARY.md) 和 [集成清单](PLUGIN-FRAMEWORK-INTEGRATION-CHECKLIST.md) 为准。

## 起点与交付门槛

| 线 | 本地/远端已提交基线 | 当前状态 | 本轮主责 |
| --- | --- | --- | --- |
| 框架与集成 `main` | `325b48c` = `origin/main` | 工作目录干净；相对插件线独有 18 个提交 | 审计原生工具发现/工具箱映射，确定安全的目录描述与注册边界；待插件交付提交后做逐文件集成 |
| 插件 `tool-plugins` | `1d66877` = `origin/tool-plugins` | 21 个已跟踪文件修改、246 个未跟踪文件条目；都是在制，尚未交付 | 先闭合公共配置模型、模板、宿主/订阅链路，并以图像源和另一真实工具验证运行与窗口 |

共同祖先为 `bf52c13`；插件线相对主线独有 14 个已提交变更。远端基线相同仅指各线已提交的 HEAD；不能把插件工作树的在制内容当作已推送功能，也不能直接快进合流。

## 已分派的插件线工作

管理线已向现有插件对话“复刻图像源工具插件”发出恢复指令，要求保护未提交改动；按 PF-001～006 和相关清单核对原厂反编译源码/XAML/资源，闭合公共配置模型、模板、订阅及动态端口、宿主生命周期；实际操作 WPF 窗口和运行两个真实工具。交付时提供原生证据、界面与运行结果、剩余差异、契约请求、中文提交号与远端分支；在必要闭合节点构建并启动版本，留人工验收。插件线只改自己的工作目录，不合并 `main`。

## 主线当前工作：工具发现与工具箱边界（NFI-07）

先独立核对原生证据与当前实现，**不碰插件线正在修改的** `MainWindow.xaml.cs`、`tool-catalog.json`、`VisionFramework` 契约及 `VisionPluginContracts`。完成契约设计后，待插件线提交再确定由谁修改注册入口。

原生源码已确认：

1. `analysis/decompiled/VMControls.Data/VMControls.Data/ToolBoxData.cs` 的 `LoadData` 从 `AppData.ModulesDir` 枚举一级目录；`ToolGroup.xml` 或 `Global` 建立分类。普通分类先插入 `defaultProxyModule`，展开时 `UpdateLoad` 再枚举分类下的 `ToolItemInfo.xml`，按 `Priority` 排序；`UserTools` 和 `GroupTools` 初次直接读子目录。
2. 同文件的 `GetItems` 将 `Lang_Tools` 与普通工具分开；全局工具限 `ProcedureModule`、`GlobalCameraModule`、`DataQueueModule`。`Reload` 清空源缓存并触发通知。
3. `analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/ToolItemGrid.cs` 从条目的 `ModuleDir`、`ModuleTypeName`、`DisplayType` 构造拖放装饰节点；拖动从鼠标按下后首次移动触发，释放会移除装饰层与预览处理器。
4. `analysis/decompiled/VM.Core/VM.Core/IMVSGroup.cs` 的 `LoadAssemblies` 针对 Group 在 `Module(sp)\\x64` 搜索 `IMVSGroup`，再加载 `IMVSGroupCs.dll`、`IMVSGroupControl.dll` 和可选 `DynamicObjectConfigControl.dll`。这是一条 Group 专有路径，不能据此声称所有插件的加载器已完整确认。

现状：`src/VisionEditor/ToolNavigation.cs` 读取打包的 `tool-catalog.json`，以 `ModuleId` 是否已注册决定条目能否拖入；`src/VisionEditor/MainWindow.xaml.cs` 显式注册图像源、二值化。工具目录发现、版本/缺失诊断及真实元数据到宿主注册仍未实现。静态目录与原生惰性展开的行为存在差异；后续必须用实际主窗口操作和缺失工具场景验证，不能只核对目录文件。

## 合流顺序

1. 插件线先给可审查提交及 `CONTRACT-REQUESTS.md` 差异；主线核对提交相对于 `bf52c13` 和当前 `main` 的受影响文件，保留双方已有 Group、复合叶和 ROI 语义。
2. 依次整合配置模型/模板、宿主、订阅与端口、一个真实工具；共享文件逐方法合并，不整文件覆盖。
3. 在实际 WPF 主窗口验证工具箱拖放、配置、输入订阅、运行/停止、Group 内调用、结果显示、保存重开及窗口释放；原厂同状态差分单独记录。
4. 功能闭合后更新 `docs/framework-acceptance.html`，提供人工验收入口，再由各线中文提交并推送。未验证的原生 ABI、设备及完整工具库保持开放。
