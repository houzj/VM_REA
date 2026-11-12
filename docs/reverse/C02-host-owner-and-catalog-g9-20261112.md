# C02 宿主执行归属与动态目录门禁

日期：2026-11-12

## 验证结果

- `WorkflowOwnerHostChecks17`：owner isolation 通过，覆盖运行中替换方案、执行 epoch、会话/历史清理和释放归属。
- `ManagedToolCatalogHostChecks18`：15/15 通过，覆盖动态目录首次展开、错误哈希拒绝、显式刷新、插件注册去重、真实节点创建、执行、保存重开和失败缓存。

## 保真边界

这些检查证明当前宿主公共合同和自有插件目录闭环成立。原厂动态目录协议、设备探测、原厂 native 回调和 `.sol` 字段仍为“运行未确认”；测试通过不宣称原厂等价。

## 复跑

```powershell
dotnet run --project tests/WorkflowOwnerHostChecks17/WorkflowOwnerHostChecks17.csproj -c Release -- artifacts/workflow-owner-g9
dotnet run --project tests/ManagedToolCatalogHostChecks18/ManagedToolCatalogHostChecks18.csproj -c Release -- artifacts/managed-catalog-g9
```
