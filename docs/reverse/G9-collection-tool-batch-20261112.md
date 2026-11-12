# G9 Collection 工具批次门禁

日期：2026-11-12

## 批次范围

本批选择 Collection 类中已有源码候选、独立注册和完整插件测试的三个工具：

- ImageAcquisition
- ImageBuffer
- Light

批次依赖 G2–G7 已闭合的启动、方案、执行、配置、ROI、保存和 Group 公共合同，不修改 tool-plugins 工作树。

## 验证结果

| 工具 | 测试结果 | 产物 |
|---|---:|---|
| ImageAcquisition | 327 项通过 | `artifacts/batch-collection-g9/ImageAcquisition.PluginChecks.dll` |
| ImageBuffer | 1623 项通过 | `artifacts/batch-collection-g9/ImageBuffer.PluginChecks.dll` |
| Light | 11836 项通过 | `artifacts/batch-collection-g9/Light.PluginChecks.dll` |

三项均 Release 构建，0 警告、0 错误。覆盖参数边界、输入输出、失败传播、资源/会话合同和已有工具专属回归。

## 保真边界

本批只证明当前源码实现、插件合同和测试范围。原厂每个 Collection 工具的完整宿主 UI、设备服务、动态 IO、`.sol` 载荷和真实设备行为仍保持“运行未确认”。测试通过不提升原厂 parity，也不改变固定总体计数。

## 复跑

```powershell
dotnet D:\MyWork\Rea逆向\artifacts\batch-collection-g9\ImageAcquisition.PluginChecks.dll
dotnet D:\MyWork\Rea逆向\artifacts\batch-collection-g9\ImageBuffer.PluginChecks.dll
dotnet D:\MyWork\Rea逆向\artifacts\batch-collection-g9\Light.PluginChecks.dll
```

## 下一批

继续选择已有完整源码证据和真实测试的简单工具族；涉及设备、通信或原厂动态 IO 的条目先做身份和合同审计，不以空实现补齐。
