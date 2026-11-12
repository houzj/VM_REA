# C03 当前版本 Blob 基线

当前主程序已注册 `vision.blob@1.0.0`，可在当前方案实体中作为图像源下游节点执行。实现使用已确认的 Blob 参数合同，接受 Mono8 图像，执行托管阈值和八连通组件提取，发布 `Components` 及其托管投影字段。

当前基线覆盖 `ThresholdType`、`LowThreshold`、`HightThreshold`、`Polarity`、`Connectivity`、`MinArea`、`MaxArea`、`FindNum` 和 `OutputBinaryImage`。`CompoundOutputFields` 将组件集合投影为 `BlobNum`、`TotalArea`、`Area`、质心和外接框数组。

## 验证

- Release 构建 0 警告、0 错误。
- `FrameworkPluginChain.Checks` 11 个图执行/保存重开用例通过，包含 Blob 直接运行、输入掩膜、4/8 连通性、不支持阈值模式的明确失败、图像源 → Blob 方案保存重开和 `SolutionExecutor` 端到端当前基线检查。
- 当前测试只证明主程序注册、参数读取、托管执行和结果投影，不证明原厂 Blob native 算法等价。

## 边界

当前实现是新方案基线的托管运行时。原厂 SDK 的 ROI/Mask、完整结果向量、图像缓冲、LimitJudge、原生错误码和历史 `45 / 971486` 结果仍需单独差分。历史 `test1108.sol` 不作为当前方案基线前置条件。
