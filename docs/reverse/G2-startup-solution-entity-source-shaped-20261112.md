# G2 启动链与解决方案实体门禁

日期：2026-11-12

## 目标

启动、方案中心、命令行独立方案、创建、保存、重开和关闭必须使用同一个 `GraphEditorDocument` 实体生命周期。该门禁只确认当前项目的独立格式合同；原厂 `.sol` 容器、场景字段和自动加载载荷保持“运行未确认”。

## 原厂证据边界

- `Program.Main → App.OnStartup → BeforeStartPart → AppAfterStart → ProjectManager.LoadSolution` 是源码/反编译确认的启动入口链。
- `WelcomeDialog` 的 640×360 与 `WelcomePage.GIF` 来自原厂 XAML/C# 和资源。
- `ProjectManager.LoadSolution` 的完整方案容器、场景恢复和失败回滚尚未取得足够原厂成员体，因此不能把 `.sol` 映射成 JSON。
- 授权/加密狗调用保留为可插拔门，不阻塞当前启动实现；这属于独立实现策略。

证据原件：

- `docs/reverse/C01-startup-chain-design-review-20261111.md`
- `docs/reverse/C01-startup-solution-chain-20261111.md`
- `D:/MyWork/SunEyeVision_V2-param-window-datatree/docs/07-工具逆向/decompiled/HostShell/Main/VisionMaster.AppMainWindow/GuideWindow.xaml`

## 当前实现合同

```text
Program.Main
  → StartupLaunch.Prepare
  → WelcomeDialog ∥ CompleteInitialization
  → AppAfterStart / Guide
  → new MainWindow
  → one GraphEditorDocument
  → command-line vmx.json / recent / default / new
  → Load or NewSolution
  → Save / Load keep SolutionId and CurrentProcedureId
  → MainWindow.OnClosed disposes the same document
```

恢复优先级由 `MainWindow.StartupSolution.cs` 固定：命令行独立格式、最近方案、用户默认方案、内存新方案。原厂 `.sol` 仅进入显式 unsupported 槽位，不会被静默当成 JSON。

## G2 门禁

| 检查 | 证据/实现 | 状态 |
|---|---|---|
| 启动时存在非空方案身份和主流程 | `GraphEditorDocument.SolutionId` / `CurrentProcedureId` | source-shaped |
| 保存绑定规范化路径 | `GraphEditorDocument.Save` | source-shaped |
| 保存不替换方案身份 | G2 测试 | source-shaped |
| 重开恢复同一身份、流程和 clean 状态 | G2 测试 | source-shaped |
| 关闭释放同一文档 | `WorkflowExecution.OnClosed` + 关闭链测试 | source-shaped |
| 原厂 `.sol` 真实兼容 | 缺少原件和运行证据 | 运行未确认 |
| 原厂自动加载/场景直启 | 缺少完整 `ProjectManager.LoadSolution` 载荷证据 | 运行未确认 |

## 复跑

```powershell
dotnet build tests/VisionEditor.StartupShellChecks/VisionEditor.StartupShellChecks.csproj -c Release --no-restore -p:UseSharedCompilation=false -p:BuildInParallel=false -p:OutDir=D:\MyWork\Rea逆向\artifacts\startup-checks-g2\ -p:UseAppHost=false
dotnet D:\MyWork\Rea逆向\artifacts\startup-checks-g2\VisionEditor.StartupShellChecks.dll
```

测试无界面运行；它不能替代原厂人工验收或 `.sol` 同输入差分。

## 解除条件

取得原厂 `.sol` 原件、版本/哈希、`ProjectManager.LoadSolution` 的完整字段读写和至少一条创建/保存/重开/场景直启运行轨迹后，才能把对应条目标为 `source-shaped` 或 `parity-closed`。
