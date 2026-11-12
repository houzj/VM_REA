# C01 ImageSource 与流程载体回归

日期：2026-11-12

本轮验证 ImageSource 进入真实流程前的公共载体：

- `VisionWorkflow.Checks`：13 项通过。
- `VisionWorkflow.DataChecks`：31 项通过，覆盖实际数据计数、变化、新执行范围和绑定变量。
- `VisionWorkflow.CallChecks`：93 项通过，覆盖画布 Group 调用、嵌套调用、失败、取消和超时。
- `ImageSource.ConfigurationChecks`：57 项通过。
- `FrameworkImageTypeGroup.Checks`：7 个真实图流程通过，覆盖 Mono/RGB、ImageMono8、可选 Mask 和保存重开。
- `FrameworkImageBufferChain.Checks`：8 个真实图像缓冲链案例通过。

这些回归确认 ImageSource→Buffer/Binary→Group→下游消费者的当前公共数据流没有回归。原厂 native 算法、设备输入和原厂 `.sol` 字段仍保持“运行未确认”。
