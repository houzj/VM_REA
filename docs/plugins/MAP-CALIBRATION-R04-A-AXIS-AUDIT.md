# R04 A CPU11 轴向候选与运行前缺口

本切片不构建、不运行原厂新样本，不修改公共产品。候选仍未通过 C# 原厂对照，不计工具闭合。

原件 MVD_Algorithm.dll SHA256 `85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549`。复用 `analysis/map-calibration/d1-source-chain/source` 的 6b7ea0、6ba450、7165b0、71b390、718ca0、71d780；唯一缺体 6b44b0 已由私有匹配 Ghidra readOnly/noanalysis 导出至 `analysis/map-calibration/r04-axis-source/source`，进程退出 0。REA provider 不可用沿用既定 fallback，无全局配置更改。

## 源码映射

6b7ea0 先单精度角度归一化、截断整数 bucket，再严格 ±FLT_EPS 相位谓词；第一角点两个坐标各经 ±.001 调整后决定轴向路径，不按整数数学角外推。仅 centerInside 分支进入此核。

6ba450 对四角 min/max 作带符号 ±0.5 截断；90/270 交换有效矩形宽高。0 度 inline，90 度 CPU11 7165b0/71b390，180 度边界2 6b44b0/其余 inline，270 度 CPU11 718ca0/71d780。字节循环与转置尾段给出目标到源半开矩形位置：0=(x,y)，90=(5-y,x)，180=(41-x,5-y)，270=(y,41-x)。反向位置使用长度减一，不能使用 max corner。

边界2在交叠矩形以外复制边缘像素，非填0；7165b0/718ca0 的 corner-byte memset 与 6b44b0 的首末行/首末列复制是直接依据。候选将这些字节写入投影为 clamp 后的整数索引，不移植 SIMD 打包调度。此为源码指导的候选翻译，尚无运行等价结论。对不交叠或空源矩形显式 Unsupported 是独立安全选择，不能声称复现原未检查内存访问。

## 整链入口修正

SourceBoardProgram 直接调用 NImageSourceGridPyramid.Find → NImageSourceGridContext.FromResult，构造仅 Points/SourceGrid 的 lattice。PitchX/Y=NaN，CodeQuadrilaterals 为空；源识别候选不读取这些 legacy 字段。CPU11 仍来自原 oracle record，不猜 HostAvx。运行入口及严格输出门见 MAP-CALIBRATION-R04-A-VALIDATION-ENTRY.md。

## 尚需退出的有限清单

1. 候选 C# 编译后先检查 40000 normalized 字节、四份原 crop 的 40000 分数整数、refinementStatus0、12 payload 字节；任何差异都保留失败并定位首操作，禁止放宽到仅 payload 成功。
2. SourceGrid→A→B→方向/anchor→WorldIndex 的 168 点原序/ID/XY float bit 对照；禁止 legacy geometry 或 decoder fallback。
3. 当前 CPU11 优化通路已补轴向源码候选；普通 CPU 路径仍明确 Unsupported，真实产品 host CPU 分派合同由主线单列。CPU7/10 不能因同属优化而直接宣称与 CPU11 全等。
4. NPointSolver FitNativePoints 候选数值顺序与实际 7ff3c0/80ac20 的首差仍需以上运行暴露；没有首差前不扩函数或原图。
5. 该原 fixture 的 refinementStatus0 不验证 refinement 成功分支；已实现 feature/circle 候选仍需源码自审与必要有限对照，不能把默认 opening 成功当完整精定位闭合。
6. radius2 col1 的原负前缀身份仍显式未知，产品安全选择不能以托管填0替代原值。共享落盘与真实宿主消费由主线统一集成。

候选补丁 r04-a-composition.patch 已重新生成并通过 git apply --check；文件身份见同目录 r04-a-index.json。无 managed 构建占用。
