# G9 Logic 工具批次门禁

日期：2026-11-12

## 批次范围

本批覆盖 Logic 线中已有源码、生产注册和宿主门禁的两个工具：

- BranchModule
- BranchString

## 验证结果

| 工具/层 | 结果 |
|---|---:|
| BranchModule 插件 | 44 项通过 |
| BranchString 插件 | 56 项通过 |
| BranchModule 生产宿主 | 15 项通过 |
| BranchString 生产宿主 | 25 项通过 |

所有测试均为 Release 构建并通过，0 错误。宿主构建为 0 警告；BranchString 插件构建保留一个已知的 `CS0169`（`ModuleRunLedger.sequence` 未使用）警告。覆盖分支表、字符串匹配、空值/调试标志、拓扑连接、配置窗口、执行选择、显示数据、剪贴板映射、保存重开和 Undo/Redo。

## 保真边界

本批验证当前源码和公共宿主合同。原厂 Logic 工具的完整动态 IO、`.sol` 字段、原生回调和人工桌面视觉仍标记“运行未确认”。批次通过不自动提升原厂 parity 或固定总体计数。

## 复跑

```powershell
dotnet D:\MyWork\Rea逆向\artifacts\batch-logic-g9\BranchModule.PluginChecks.dll
dotnet D:\MyWork\Rea逆向\artifacts\batch-logic-g9\BranchString.PluginChecks.dll
dotnet D:\MyWork\Rea逆向\artifacts\batch-logic-g9\BranchModule.HostChecks.dll
dotnet D:\MyWork\Rea逆向\artifacts\batch-logic-g9\BranchString.HostChecks.dll
```
