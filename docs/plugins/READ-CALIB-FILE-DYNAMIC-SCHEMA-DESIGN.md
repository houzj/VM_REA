# 标定加载动态输出契约设计

## 原厂证据

原厂 `ReadCalibFileModu` 的 `Process` 在非零 `RefreshSignal` 时重读当前文件，随后按新文件里的输出项、名称和类型调用输出 setter。原厂受控运行已实测：同 schema 的 `Dynamic=42` 更新为 `Dynamic=43`；文件改成九项 `CalibMatrix` 后，下一次 Process 实际发布九个 float；`GetCalibInputParam` 也同步变为新输入描述。原厂没有独立图编辑器的静态端口注册契约。

原先独立宿主把 `OutputSchema` 冻结到节点参数，导致刷新后类型/名称变化时只能报 `CALIBRATION_SCHEMA_CHANGED`。仅放宽静态 GraphValidator 不安全：执行时必须以刷新后的输出描述检查本轮输出，并在消费者读取前重新检查旧连接。

## 方案与兼容约束

1. `ModuleDescriptor.SupportsDynamicOutputs` 是显式能力标记，缺省 false。只有标记该能力的模块，GraphValidator 才延迟其“源输出端口名/类型”检查；目标输入名、重复绑定、执行顺序等静态检查仍照常执行。所有既有固定端口模块继续走原验证分支。
2. 动态模块用 `ModuleResult.UpdatedDescriptor` 返回本轮实际输出 schema。GraphExecutor 要求模块 ID/版本、输入、参数与注册描述一致，仅允许输出列表变化，并通过 registry 的端口校验/冻结路径检查新描述。未标记模块试图更新、身份变化、输入/参数变化或无效端口都作为模块失败处理。
3. GraphExecutor 先用注册描述解析模块输入，再执行模块。成功结果先按 `UpdatedDescriptor` 校验输出并记下运行时描述；下游连接取值时以该描述重验端口存在、基数和类型。刷新删除旧端口或改成不兼容类型时，消费者明确失败为 `DynamicConnectionInvalid`，不读取残留旧值。匹配新 schema 的连线可继续运行。
4. `ExecutionEvent.UpdatedDescriptor` 和 `ExecutionSessionResult.NodeDescriptors` 把本轮实际描述传给宿主。WPF 主窗口在事件到达时更新运行态端口卡片，完成后以 session descriptor 刷新；模块结果页并列展示“运行端口”schema（类型、标量/数组、必需/可选）和本轮实际输出值。未订阅该可选信息的既有进度接收器仍只看到原节点状态事件。当前不自动改写用户流程文件或重连旧线；失效连线保留在画布模型中，运行时明确报告 `DynamicConnectionInvalid`，用户自行决定如何修复。
5. 现阶段动态节点输出只支持显式数据连线。工作流变量、接口、显示和参数订阅仍要求静态持久化 schema；在工作流订阅契约加入动态运行值前，GraphValidator 对这种路径保持拒绝，不能把不确定类型当作兼容。

## 小型验证计划

通过真实 `GraphExecutor` 和一个仅测试用的输入源/下游 consumer 验证：同 schema Refresh 更新值并发出 descriptor 事件；刷新后原线已删除或类型不兼容时消费者不执行有效读取、返回 `DynamicConnectionInvalid`；连接到新类型端口则成功。另验证静态模块原有错误码、校验和事件序列不变。远程选择器通过注入 `IToolConfigurationFileService` mock 核对 `Remote=true`、XML filter、当前路径及选择首项提交；默认服务无 remote backend 时必须返回 `Unsupported`。没有真实 FTP backend/宿主句柄，不报告远程 native 运行通过。

## 文件边界

预期共享实现范围：`VisionFramework/Contracts.cs`、`GraphExecutor.cs`、`GraphValidator.cs`、`VisionPluginContracts/Contracts.cs`、`ToolConfigurationInteractionServices.cs` 与专属执行/配置检查。工作流订阅与接口 schema 暂不扩展。ReadCalibFile 插件设置动态能力并返回刷新后的 descriptor。文件所有权已由主控确认；Map/Board 并行线未修改以上共享接口文件。

## WPF 宿主验证

`tests/VisionEditor.WpfChecks` 通过实际 MainWindow Run 按钮执行测试动态插件，覆盖 progress event 即时卡片更新、最终 session descriptor、结果 schema 与输出值显示、普通静态节点高度保持不变、工作流序列化前后不变，以及保留旧连线并在下游执行前失败的端到端路径。报告为本轮本机生成的 `analysis/full-audit/wpf-workflow-parity.json`；它验证当前独立 WPF 宿主，不代表原厂平台或远程 FTP native 验证。
