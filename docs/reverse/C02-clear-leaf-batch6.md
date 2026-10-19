# C02 第六批：原清值叶方法受控运行

2026-10-19，基线 `48a0760`。预定有界子项 `C02.clear-leaf-operation` 已完成：**4 个固定案例、32/32 项断言通过，原进程 exit 0**。这是原方法在合成 uninitialized 对象上的运行；不是实际 VM 流程、SDK 回调或原机差分，完整 C02 仍开放。无产品修改。

## 原证据与安全门

使用 reverse-engineer-anything 工作流，复用第四批原 CIL，不重复导出33方法或重试既有REA成员预算失败。原来源：`analysis/decompiled/VM.Core/VM.Core/VmIO.cs:934`，`VmFrameTreeBase.cs`，`ModelBase.cs`。调用者链和队列边界见 `docs/reverse/C02-callbacks-batch4.md`。

原 `SetNoneValueWithoutRaiseEvent` token `06000524`，IL SHA256 `361e8712c1b44dc1d1c1e18d659f5e09e6ef8ea60eae4ba954a9fb8bf75062cc`。探针在对象创建前硬核对该哈希，并逐层检查 VmIO → VmFrameTreeBase → ModelBase → Object 的 TypeInitializer 全为空，输出构造 IL。普通 VmIO 构造在 `VmIO.cs:1279` 调用七次 SetValueFromServer；本轮不执行任何普通原构造，采用 FormatterServices.GetUninitializedObject，仅通过原实例字段注入状态。

加载的 VM.Core 实际由强名解析到 GAC，SHA256 `936763f6c8a1bf9fcf59f22acebca8feb09b5f3d5701ae77c82bdc81e626190d`，与安装原件一致；实际程序集路径和SHA逐个记录在 probe.json。VM.PlatformSDKCS 被类型依赖加载不等于SDK执行，本探针不调用其API、不构造 VmSolution、不触达设备/窗口，不调用 Value、RelateIO 或 Dispose。

## 固定案例及结果

4案例为 `_value` 非空/空 × `isNull=false,RaiseValueChanged=true` 与 `isNull=true,RaiseValueChanged=false` 两组状态。每例预置 `_ExecuteCount=37`、`_valueList=["retained",42]`、`needMergeIOLoopValue=true`，对继承链全部委托字段注入计数器。

每例固定8断言：_value为空、列表身份不变、列表内容不变、ExecuteCount不变、isNull不变、RaiseValueChanged不变、事件不触发、其余39实例字段不变，共32项。原叶函数只清当前 `_value`；40实例字段中其他字段均不变，包括事件委托和merge标志。空值分支同样无额外变化。

这将第四批静态结论增加为可复验的**局部原方法执行证据**，没有把“清结果”扩大为清列表/计数/事件/资源。没有计算属性消费，RelateIO、MergeIOLoopValue、消费者显示、真正SDKBegin回调及图像所有权依然未知；此子项不再需要重复静态派工。

## 复验与失败保全

入口 `tests/NativeEvidence/C02ClearLeaf/Run.ps1`，Framework csc 编译，无dotnet产品构建。输出：`analysis/full-audit/batch6-20261019/C02/run-b90af130c8e84568ae2f255d4b3b2a23/`。

原方法第一次运行32/32通过、exit0，原件前后SHA一致。但包装脚本生成哈希CSV时流枚举包含正在写的CSV导致文件占用错误，包装命令exit1。`runner-failure.txt` 保留该失败；Run.ps1已改为先快照文件列表再写manifest，现场manifest用同一快照方式补齐，没有重复运行原方法，也不把包装失败隐去。probe.json、process.json、stdout/stderr、probe源码哈希和output-sha256.csv均保留。

本轮不实现替代清值逻辑、不声称实际VM正常初始化或原生资源释放等价。下一可关闭门槛是有受控真实实例条件后记录Begin回调到消费者值链；在此之前不得拿本32项替代该门槛。
