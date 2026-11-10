# 启动链路与空载初始态：源码级证据结论

日期：2026-11-10 · 证据卡 `analysis/full-audit/batch1-20261107/C03/c03-startup-initial-ui-20261110.json` · builtFrom `7d65a1e1/dfd5fd922c28`

## 结论口径

本轮按 `reverse-engineer-anything` 流程复用本机既有反编译产物、窗口布局边、XAML/CIL 台账与自研源码做**静态源码级取证**。REA MCP 与 native provider 在当前主机不可用，因此**未新取 native 函数体、资源或 ABI**；不声称 parity，不把静态分析写成运行观察。

| 证据类型 | 含义 | 本轮来源 |
|---|---|---|
| `source-confirmed` | 原厂反编译源码 / XAML / CIL / 布局边可复核 | `analysis/decompiled/VisionMaster.Exe.Audit`、`VisionMasterMain`、`vServerApp.Audit`、`docs/architecture-recovery/*.edges.json` |
| `runtime-confirmed` | 既有运行报告已记录 | 仅引用既有 `C01` / `C03` 报告，本轮**未新增**运行证据 |
| `independent-implementation` | 自研 managed 壳，不是原厂等价 | `src/VisionEditor/*`、`src/VisionEditor.Core/*` |
| `unknown` | 未取证，保持开放 | 见下节 |

## 主要观察（observation）

1. 启动链路跨 `Program.cs` 与 `App.cs` 两个文件：`Program.Main` @61 → `SingleInstanceManager.Initialize` @70 / `StartApp` @76 → `App.OnStartup` @92 → 子线程 `BeforeStartPart` @111 → `BeforeStartMian` @136 → `InitMainWindow` @159 → `AppAfterStart` @179 → `App.ShowMainWindow` @354 → `Show()` @363 → `ConfigZoomIndex` @364。
2. `MainWindowData.cs` 是启动期数据容器（`PreLoad` @984、`RecoverMainWindowState` @958、`ConfigZoomIndex` @340），`VisionMasterMain.MainWindow` 继承 `BaseWindow` 并带进度遮罩依赖属性。
3. 存在**双入口**：`Program.Main`@61 与 `App.Main`@560-564 各含 `[STAThread]` 与 `app.Run()`；`vServerApp.Audit` 另有伴随进程入口。
4. 窗口布局边 30 条分五个 subsystem（W01–W16 / K01–K07 / C01–C05 / U01–U02），分类含 `source-confirmed`、`independent-implementation`、`unknown`。
5. 右下角导航缩略图在自研 `MainWindow.xaml` 有 `MiniMapPanel/MiniMapViewport/MiniMapToggle` 等命名控件，`WorkflowCanvas.cs` 提供视口同步与拖动方法。

## 关键陷阱（避免把推断写成事实）

- `WorkflowOverview.cs`、`WorkflowOverviewDrag.cs`、`WorkflowCanvas.cs`、`MainWindow.ShellStatus.cs` 等文件内**没有同名类型**，实际都是 `VisionEditor.MainWindow` 的 partial 分部；文件名不能当类型名。
- `AppBeforStart` @142、`InitLang` @272、`AwakeServerAsProcess` @528、`InitMq` @624、`StartServerApp` @600 等**有定义但在可见 Main→首屏链路中无直接调用点**，单列「定义存在/调用未知」。
- 原厂 `BeforeStartMian`、`AppBeforStart` 拼写按原文保留，不纠正。
- `window-layout.edges.json` 没有画布 / 显示区 / 结果区 / 状态栏 / 导航缩略图子系统；这些控件必须引用其它 `.edges.json` 与 `MF02-MINIMAP-*.html`，不能把 W/K/C/U 边硬套上去。
- A 线证据中「单实例 / 历史结果」0 命中，改用 `docs/architecture-recovery/` 台账与 C 线文档；`closedEntryChains=0` 仍需保留。

## 未做的事（本轮用户选定深度）

- 未改 `src/` 产品代码；未跑方案测试；未做原厂同状态差分与自研操作级复跑。
- 截图只用于识别控件范围与空载状态，不作实现规格、不请求新截图。
- 未把六段链任何一段标为已闭合、未预填三级验收；126 个六段链节点全部 `unknown`。
- 未解除 `B0 open / B1–B3 blocked`；C03 差分 `45/971486` 对 `47/973202` 仍开放。

## 证据引用

共 52 条，全部带 SHA-256 与字节数，见证据卡 `evidence[]`。