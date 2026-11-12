# G9 Fixture 工具批次门禁

日期：2026-11-12

## 批次范围

本批覆盖 Fixture 工具的独立插件逻辑和生产宿主链路。测试使用当前仓库已恢复的源码、资源和公共宿主合同，不宣称原厂 DLL 等价。

## 验证结果

| 层 | 结果 |
|---|---:|
| Fixture 插件 | 43 项通过 |
| Fixture 生产宿主 | 30 项通过 |

覆盖输出发布、锁定/未锁定引用、类型化 FIXTURE 数据、范围与错误码、原始 getter 顺序、教学/重教事务、保存加载、资源化本地化、显示状态释放和宿主注册。

## 保真边界

本批关闭的是当前实现的源码级合同和宿主检查。原厂 native 算法执行、设备输入、原厂 `.sol` 字段和人工桌面验收仍为“运行未确认”。CameraMapping 的独立检查因缺少原厂资源路径而未纳入本批；该资源缺口保持为证据阻塞，不用独立实现替代。

## 复跑

```powershell
dotnet artifacts/batch-fixture-cameramapping-g9/Fixture.PluginChecks.dll
dotnet artifacts/batch-fixture-cameramapping-g9/Fixture.HostChecks.dll
```
