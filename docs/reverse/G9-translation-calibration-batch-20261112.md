# G9 TranslationCalibration 工具批次门禁

日期：2026-11-12

## 验证结果

| 层 | 结果 |
|---|---:|
| TranslationCalibration 插件 | 7469 项通过 |
| TranslationCalibration 生产宿主 | 19 项通过 |

检查覆盖源码配置模板、手动表格求解、结果前缀/轴映射、显示对象、配置自运行与缓存、文档重载、节点删除释放，以及 POINT 数组输入和调试路径。

## 保真边界

本批是当前实现的源码与生产宿主自审。原厂 native 求解器、动态 IO、原厂 `.sol` 字段和人工桌面验收继续标记“运行未确认”；测试通过不提升 VM 等价计数。

## 复跑

```powershell
dotnet run --project tests/TranslationCalibration.PluginChecks/TranslationCalibration.PluginChecks.csproj -c Release
dotnet run --project tests/TranslationCalibration.HostChecks/TranslationCalibration.HostChecks.csproj -c Release
```
