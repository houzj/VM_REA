# 宿主中立订阅桥接检查

运行：`dotnet run --project tests/VisionConfiguration.WorkflowSubscriptionChecks`。

`WorkflowConfigurationSubscriptionService` 仅依赖 VisionFramework 的 ProcedureDocument、ModuleRegistry 与写入委托，不依赖 VisionEditor，不运行算法。构造形式：

```csharp
new WorkflowConfigurationSubscriptionService(
    () => currentProcedure,
    registry,
    targetNodeId,
    (destination, reference) => ApplyTypedBinding(destination, reference));
```

写入委托负责替换对应目标的配置订阅/数据连接；`reference == null` 表示解除。桥接调用后重新读取文档，检查对应目标确实只剩下请求的引用；无操作/失败回调不会返回伪成功。服务绑定首次构造时的流程身份，防止切换流程后重用节点 ID 造成误写。

已支持范围：当前根流程内、显式 ExecutionLinks 的递归祖先、注册描述符实际声明且类型兼容的输出、整数/有限数值/文本标量常量，以及上述配置订阅和已有数据连接的读取。数据连接不是执行关系的替代物：ExecutionLinks 为 null 时，输出来源枚举/绑定明确 Unsupported；常量不需要来源执行图。数据连接和配置订阅并存或重复时明确失败。目标参数/输入以 Domain 区分，常量参数还验证真实声明范围。

`UnsupportedCapabilities` 列出 Global/LocalVariable/WorkflowInput、父容器输入、数组索引、原厂 private/Visible/CanSubscribe/timestamp/层级/浮点宽度元数据。当前公开框架端口作为明确的可用子集，不能据此宣称覆盖原厂隐藏或私有端口行为。`Number` / `NumberArray` 映射为此桥接约定的 native float；没有假造 Point/Line/Components 等缺失精确类型。常量 float 输入按 Single 转换，框架不接受 native Infinity。输出/常量都保留类型化身份，caption 只用于呈现。

58 项检查涵盖反向节点顺序、递归祖先、分支/后继/自身排除、同名来源、读两种储存关系、持久化核对、类型/范围检查、显式 capability 边界、环/悬挂节点、禁用来源及流程身份变化。相关原厂连接图和过滤源码依据见 `docs/plugins/configuration-subscription-evidence.json`。这些为独立框架检查，不是原厂 VM 完整运行或 UI 等价证明。
