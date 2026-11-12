# G4 配置、ROI、Mask、显示和订阅门禁

日期：2026-11-12

## 原厂证据边界

原厂证据确认配置窗口通过工具描述、模板、绑定和显示对象集合驱动；ROI/Mask 进入工具算法前经过区域集合和预处理 Mask 桥。Blob 的 `GenerateMaskImage`、区域助手和 SDK 缓冲差异仍属于 C03 原厂静态/运行取证，不能由当前托管 ROI 栅格直接替代。

主要证据：

- `docs/reverse/A03-binding-batch4.md`
- `docs/reverse/A03-tabs-batch2.md`
- `docs/reverse/C03-source-blob-generatemask-region-bridge.md`
- `docs/reverse/C03-same-input-differential-20261110.md`

## 本轮真实修复

配置宿主运行标定板工具后，结果刷新暴露了两处数据形状错误：

1. `NodeResultProjection` 将 `DataKind.Point` 数组强制读取为标量 `Point2`。
2. `ModuleRunLedger` 对 Point 数组和 Number 数组按标量格式化。

现在两条结果链分别按 `ValueCardinality.Array` 读取并生成行/文本，标量行为保持原路径。

修改文件：

- `src/VisionEditor.Core/NodeResultProjection.cs`
- `src/VisionEditor.Core/ModuleRunLedger.cs`

## 门禁结果

| 范围 | 结果 |
|---|---|
| 生产 MainWindow 配置、参数模板、ROI 窗口、显示对象、撤销重做、保存重开、Group 输入 | 49 项通过 |
| 显示订阅树、常量/自输出/祖先输出、错误保留 | 27 项通过 |
| ROI 栅格、Mask、Fixture/Point 订阅、归一化与缩放 | 16 项通过 |
| 原厂 Blob `GenerateMaskImage` 的真实 ROI 集合和 SDK 缓冲 | 运行未确认 |
| 原厂显示容器 `DisplayModuleSet` 跨 Group/Tab 生命周期 | 运行未确认 |

## 复跑

```powershell
dotnet D:\MyWork\Rea逆向\artifacts\roi-host-g4-fix4\VisionConfiguration.RoiHostChecks.dll
dotnet D:\MyWork\Rea逆向\artifacts\config-display-g4\VisionConfiguration.DisplaySubscriptionChecks.dll
dotnet D:\MyWork\Rea逆向\artifacts\roi-g4\RoiTool.Checks.dll --regions-only
dotnet D:\MyWork\Rea逆向\artifacts\roi-g4\RoiTool.Checks.dll --normalization-only
```

以上测试覆盖当前托管合同和已落地真实工具；不宣称原厂 `.sol` 或原厂 Blob 宿主 parity。
