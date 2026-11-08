# C03 Blob GenerateMaskImage 与区域桥接

静态证据写入 `analysis/full-audit/batch1-20261107/C03/source-blob-generatemask-region-bridge.json`。

`CIMVSBlobFindModu::ProcessAlg` 在非直接 ROI 模式下通过对象 vtable `+0x48` 调用 `GenerateMaskImage` 候选。调用参数显示 `param_1+0x20` 是 PreproMask 对象，`param_1+0x38` 是辅助状态，其他入口寄存器保持活跃。成功后 SDK 区域助手先清空 ROI，再遍历 PreproMask 节点，读取节点值和启用标志，把启用节点传给区域助手的 `+0x08` AddROI 候选。失败返回码会在 SDK Run 前直接返回。

构造函数证据确认 `CreatePreproMaskToolInstance(param+0x18)`、算法句柄 `MVD_Algorithm_CreateHandle(...,0x12d,0)` 和 RegionImageAssistant 对象都在同一模块状态中创建。性能助手、区域助手和预处理对象分配失败会抛出 `IMVDException`，部分路径使用 `0x80100009`。

这些结论是 stored decomp、SDK 头文件和 vtable 对齐得到的静态证据。确切 ROI 原生类型、坐标变换和 live native 执行仍未确认。
