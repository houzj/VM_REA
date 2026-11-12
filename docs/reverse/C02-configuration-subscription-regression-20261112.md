# C02 公共配置与订阅回归

日期：2026-11-12

本轮回归沿用 CodeBuddy 已完成的启动链基线，验证非 Group 的公共配置和方案载体：

- `VisionConfiguration.WorkflowSubscriptionChecks`：60 项通过。
- `VisionConfiguration.ParameterHostChecks`：23 项通过，覆盖 POINT 数组、参数绑定/解绑、有效运行值、Undo/Redo、Reset、跨流程隔离和真实 CameraMapping 配置宿主。
- `SolutionInspector.Checks`：基础 frame/UI 容错检查通过；原厂 `CircleFind.sol` 样本路径缺失，原生方案读取部分保持“运行未确认”，没有用合成文件替代。

这些结果确认当前公共配置和订阅链没有回归；原厂 `.sol` 容器字段及缺失安装样本仍是独立证据边界。
