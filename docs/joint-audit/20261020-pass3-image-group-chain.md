# 第三轮：owned Group 中真实图像链的当前实现证据

范围：只验证 **当前 main 独立实现** 的 J05/J09/J11 有界切片，不将本地 VMX、Group 调度或像素结果称作原厂 `.sol`/SDK 行为。没有修改产品源码、共享合同或 `worktrees/tool-plugins`，没有操控用户已有窗口/进程。

## 图与调用路径

可重跑测试在 `tests/FrameworkImageGroupChain.Checks/`。它从实际 `GraphEditorDocument` 创建主流程的 owned Group，在 body 中建立 `ImageSourcePlugin.Image → ImageBufferPlugin.InputImage → ImageBinaryPlugin.Image → 第二个 ImageBinaryPlugin.Image`，把第二个 Binary 的 Gray 输出导出为 `Group.Result`；主流程再把 `Group.Result` 连至第三个、真实执行的 `ImageBinaryPlugin.Image` 消费者。源图是自有 2×2 Gray 内存帧，经 `ImageSourcePlugin.SubmitImage(scope.ForProcedure(body), sourceNode, sample)` 提交，而非伪造 `GraphExecutor` 的输出记录。

`GraphEditorDocument.EnterOwnedGroup/Connect/SetWorkflowConfiguration/LeaveGroup` 固定了 owner、端口和三条 body 连接；`SolutionExecutor.RunAsync` 按 `WorkflowCalls` 调用 body，`GraphExecutor` 用同一 solution `ModuleExecutionScope` 的 procedure/module/node 状态执行实际插件并向 parent 返回输出。测试断言 `result.Calls[group]` 中四个 body 节点实际运行或跳过，还断言 parent Group 和第三个 Binary 消费者状态、最终 `GrayImage` Kind/尺寸。它没有使用原厂执行器或原生二值化内核。

## 五次运行与文档恢复

| 状态 | 实际结果 |
|---|---|
| 初始 | body 四节点成功，Group `Result` 输出 Gray，parent Binary 成功消费 |
| body `BufferCount=0` 整数常量订阅 | Buffer 成功但空槽无 `OutImage0Image`，两级 body Binary 跳过，Group 失败，parent Binary `ExecutionPredecessorUnavailable` 且没有旧输出 |
| Undo | 订阅移除，body 和 parent 实际重新执行成功，Group 身份/连接未变 |
| Redo | 同一失败路径重现，无旧输出泄漏 |
| 再 Undo，`GraphEditorDocument.Save`，新 `GraphEditorDocument.Load`，新 `ModuleExecutionScope` | Group owner、Call 目标、三条 body 连接、parent `Result` 连线保持；重新提交源帧后四个 body 节点及 parent Binary 成功 |

可重跑命令：`dotnet run --project tests/FrameworkImageGroupChain.Checks/FrameworkImageGroupChain.Checks.csproj -c Release`。5 次实际执行和身份/保存检查全部通过。日志 `analysis/joint-audit/image-group-chain/group-chain-checks.log` SHA256 `B6A279C962140A959B2977689BF93BF9318C3E975E1AD711230D4437869DEFAB`；本次保存文档 `analysis/joint-audit/image-group-chain/owned-image-chain.vmx.json` SHA256 `5AC571820E4ABC7924214AAD0E536F36BDEA94E40009AC294ACF1398FCA60772`。文档包含每次新建时生成的 solution/node ID，重跑会生成新哈希，须按那次产物重新核。原始失败分支保留在测试断言和日志，不以最后成功覆盖。

## 已证与仍缺的边

- **confirmed-current**：上述 Gray 路径中真实 producer、Buffer、两个 body Binary、Group 返回和 parent Binary 消费者均经 `SolutionExecutor` 运行；失败后旧结果不被 parent 消费；Undo/Redo 和本地 VMX 保存重开保留 Group/端口身份。
- **independent-compatibility**：使用自有内存 Gray 帧、当前 `workflow.subprogram` 调度、当前 `ImageBinaryPlugin` 硬阈值及本地 VMX 文档。新 scope 重新提交帧才运行，不能据此说图像样本或原厂缓存队列被保存。
- **blocked-exact-evidence**：原版 Group 的 IMAGE 叶/全局变量候选、`PreModules` 通知、原厂缓存队列保存/失败释放、二值化原生五模式及 `.sol` 重开同图执行仍需对应成员体/指令或自有原厂 SDK 探针。RGB 跨 Group、Mask/条件输出、取消/超时、设备分支、UI Dispatcher 无响应也未覆盖。J05/J09/J11 只是当前实现的指定切片，不能整体关闭；J12 需无响应时 UI 线程堆栈。

本轮只增加可复验证据，没有新增 96 项框架实施完成单元。
