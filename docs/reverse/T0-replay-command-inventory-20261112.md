# T0 复跑命令清单（2026-11-12）

规则：只登记**已在 CodeBuddy 侧实际执行**的命令及其真实退出码；未冻结 `csproj` 路径、参数与预期产物的套件不得臆造命令。日志统一落到 `artifacts/codebuddy-verify/`（已被 `.gitignore` 忽略）。

## 1. 已冻结并实跑

| # | 命令 | 参数 | 退出码 | 日志 | 结果 |
|---|---|---|---|---|---|
| 1 | `dotnet build src/VisionEditor/VisionEditor.csproj -c Release --no-restore -p:UseSharedCompilation=false -p:BuildInParallel=false` | 无位置参数 | `0` | `t0-build.log` | 0 警告 / 0 错误，用时 3.00s |
| 2 | `dotnet run --project tests/ToolCatalogLifecycleChecks15/ToolCatalogLifecycleChecks15.csproj -c Release -- src/VisionEditor` | `src/VisionEditor` | `0` | `t0-toolcatalog-tests.log` | 19/19 通过 |
| 3 | `dotnet run --project tests/NodeProjectionChecks/NodeProjectionChecks.csproj -c Release` | 无 | `0` | `t0-NodeProjectionChecks.log` | 54 项投影断言通过 |
| 4 | `dotnet run --project tests/VisionEditor.WorkspaceProjectionChecks/VisionEditor.WorkspaceProjectionChecks.csproj -c Release` | 无 | `0` | `t0-VisionEditor.WorkspaceProjectionChecks.log` | 79 项投影接线断言通过 |
| 5 | `dotnet run --project tests/VisionEditor.ShellMenuChecks/VisionEditor.ShellMenuChecks.csproj -c Release` | 无 | `0` | `t0-VisionEditor.ShellMenuChecks.log` | 65 项入口清点断言通过 |
| 6 | `dotnet run --project tests/VisionEditor.ShellChecks/VisionEditor.ShellChecks.csproj -c Release` | 无 | `-532462766` | `t0-VisionEditor.ShellChecks.log` | **失败**：`Save menu persists and orders recent solution paths`（`Program.cs:275`） |
| 7 | `dotnet run --project tests/VisionEditor.StartupShellChecks/VisionEditor.StartupShellChecks.csproj -c Release` | 无 | `0` | `t0-VisionEditor.StartupShellChecks.log` | WelcomeDialog/GuideWindow/宿主数据/关闭链通过 |
| 8 | `dotnet run --project tests/VisionEditor.ShellHistoryChecks/VisionEditor.ShellHistoryChecks.csproj -c Release` | 无 | `0` | `t0-VisionEditor.ShellHistoryChecks.log` | 工具栏撤消/重做作用于实际节点名称通过 |

命令 2 的参数来自 `docs/CODEBUDDY-HANDOFF-20261112.md` §62-67，已复跑验证；命令 3–8 的 `csproj` 路径经 `Test-Path` 逐个确认存在，参数经 `Program.cs` 检索确认无必需位置参数（唯一命中的 `args` 是 `VisionEditor.WorkspaceProjectionChecks/Program.cs:463` 的反射辅助方法形参，不是命令行参数）。

## 2. 首个失败记录

```
Unhandled exception. System.Exception: Save menu persists and orders recent solution paths
   at Program.<Main>g__Check|0_1(Boolean value, String description) in tests/VisionEditor.ShellChecks/Program.cs:line 48
   at Program.Main(String[] args) in tests/VisionEditor.ShellChecks/Program.cs:line 275
```

- 期望：最近方案菜单 2 项，顺序 `recentSecond, recentFirst`。
- 现场：前序 PASS 行显示 `标头:最近打开方案 Items.Count:3`。
- 处置：T0 不改代码、不改夹具、不跳过断言。差异归 T2（方案/最近列表）与 T5.4（合流验收）；解除条件为原厂最近列表条目数/排序/持久化的 E1 证据。
- 影响：该套件在第 45 行中断，其后的断言**未执行**，不得据此宣称套件其余部分通过。

## 3. 未冻结（禁止臆造）

- `tests/` 下其余 300 余个 `*Checks*` 套件的 `csproj` 路径、位置参数、预期产物与退出码，尚未逐一冻结；T1.3/T3/T5 按任务卡需要时再补。
- 44 份 `analysis/full-audit/fidelity/20261112/*.json` 对应的原生探针与差分流程命令，尚未冻结；`docs/reverse/T0-gate-artifact-hashes-20261112.json` 的 `replayCommand` 保持为空，`codebuddyVerification` 保持「未复跑」。
- `tests/VisionEditor.WpfChecks/` 路径已确认存在，但交接文档记录其在 104 项后因测试流程缺 ImageSource 节点停止，本阶段未复跑，不登记为可用命令。

## 4. 构建纪律

- 使用 `-p:UseSharedCompilation=false -p:BuildInParallel=false`，串行执行，独立日志目录。
- 未终止任何进程，未改动其他任务拥有的窗口或构建产物。
- 截图/导出辅助逻辑仅存在于 `tests`，产品程序集未引入取证代码。
