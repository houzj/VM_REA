# R03固定24符号源码翻译接入

2026-10-19，根线审查后接入共享产品。工具总完成度仍0/10；此能力不单独计为完整工具。

`src/VisionPlugins.Calibration/NImageCodeReedSolomon.cs`现在委托给`NImageSourceCodeword24`，删除原独立BM/Gaussian求幅值路径及其未使用GF辅助。192 bit的MSB打包和12 byte反序输出保持原调用合同。

原厂core SHA256：85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549。具体体、指令窗和索引位于`analysis/map-calibration/d1-decoder-output/source`和`d1-source-chain/source`。接入保留8b11a0六区域初始化及跨轮系数尾部、根数等于degree谓词、8b0670卷积/截取/反序求幅值次序、931c60零分子落入查表覆盖，以及原厂没有纠正后再次syndrome拒绝的行为。不是按标准算法意图重写。

Log[0]=0依据PE .data虚拟尾部loader零填充，当前恢复链932480只写非零索引。它不是“没在初始化函数赋值所以随机”，也不是全模块任意写者不存在的证明。只读共享表、托管独立缓冲以及非24长度拒绝属于明确安全适配；不宣称原厂全局表并发或任意指针重叠语义等价。

验证：

- C#候选122项通过，40个既有原厂词共960符号逐项比较：`analysis/r03-candidate-validation/c4b5b9e103d94ff8bdf05c734b48fe06/checks.log`。前一次构建把`-m:1`误传为程序参数，失败日志保留；修正调用后通过。
- 产品共享入口163项、实图手工quad字节及独立方向属性18项通过：`analysis/r03-product-validation/d4fdef1e42524e1eb00550ad798a229c/checks.log`。
- 八组source/target、HIK7/9、normal/reverse识别边界均168点：`analysis/r03-boundary-validation/27b218b5a661475ea2d7ca1d61285e49/result.json`。这是产品回归，不是新原厂八向定位证据。
- byte0..23失败词原厂返回-1000且不写输出，记录在`analysis/map-calibration/r03-failure-oracle/probe.jsonl`；degree9/root1是独立复演内部值，不能称运行观察。

R04自动定位、warp和采样仍在D线实施。本接入不把手工quad或方向属性冒充完整映射工具闭合。人工验收独立记录，不参与实施进度。
