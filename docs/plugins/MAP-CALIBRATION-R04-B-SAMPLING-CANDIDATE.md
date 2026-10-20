# R04-B 固定cell源采样候选

2026-10-19。候选目录 `analysis/map-calibration/proposals/r04-source-recognition`。新 `NImageSourceTagSampler.cs` + `NImageSourceQuadFit.cs`，新增类完整补丁 `r04-b-integration.patch` 已git apply --check通过，未应用共享；未编译C#。根线R03生产类由测试source-link复用，无第二套word实现。

## 实施链

- 86ff00单item四点，float powf/sqrtf距离与min，最后边长double /12.65再float，实际cell2..8。只处理一个传入排列；不内置8向循环。
- 89faa0 count4/model1归一化 sqrt2/平均距离（区别于7ff3c0常量1），8a0650构建8×8设计矩阵；8580e0→7075e0形成A'A/A'b，730e90全主元float逆及乘法。复用NPoint现有float normal/fullpivot操作顺序，未调用其IRLS/默认求解。新取得8a0650/8580e0/7075e0原C/asm在r04-warp-fit与r04-warp-normal，正常返回链具体可追。
- 89faa0通过(Tb'Tb)^−1 Tb'形成inverseTb，随后inverseTb*Hnormalized*Ta，每个乘法float逐k累加、一次reciprocal乘法归一化；保留h33小于floatEPS时清零矩阵的成功路径。
- 8b1710逐像素800670 float投影，floor/ceil四角判断，超边界写0。保留逐项除法，不替换成floor+1/常见双线性；ceil==floor保留0/0，x64 CVTTSS2SI indefinite转低byte0显式实现。
- 85c8a0当前window=4cell，偶数窗edge-including反射；先ushort竖和，再uint横和。当前最大kernel32不会溢出这些域。普通8a7b90左边界double+.5与中/右float+.5区别保留；CPU11/7/10 large-kernel x0标量舍入，后续向量nearest-even。managed实现不改变线程MXCSR，默认舍入环境为独立实现合同；不声称复制原环境位生命周期。
- 8a8040 mode0/offset0，S<M为0，等号255。86ff00固定mask、block→row→column格位，cell/3整数inset、均值严格>153、棋盘xor、MSB192bit打包24符号，调用已上架R03，反序导出12byte。没有独立adaptive、后验syndrome或方向再试。

## 同图定点原厂oracle

复用旧手工quad [246,253,317,253,294,324,246,324] 和同一1000×1000 help-derived图。新自有进程调用630120/89faa0/86ff00；额外85c8a0只对该次warped输出确认均值图。没有平台Init、设备、授权或新样本。

原件core SHA256 `85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549`。`r04-tag-sampling-oracle/probe.jsonl`：CPU=11，fitStatus1，cell5，flag1/count1，payload [3,3,0,10,0,0,3,120,30,63,136,66]。同目录H/warped/mean/binary/192intbits为原厂原始文件，provenance/process含身份/退出0，原生进程已结束。

`replay_sampling.py` 是**独立Python算术复演**，不是C#运行。其九个拟合float逐bit、6400 warped、6400 mean、6400 binary、192bit与本次原厂全相等；`independent-sampling-replay.json`记录零差异。它只支持本固定图/quad/CPU11与正常环境，不证明任意图像/CPUID等价。

## 最小C#验证入口（待根线槽）

从worktree根build本目录R04SamplingChecks.csproj，用独立artifacts；没有共享项目修改。Program读同目录原厂oracle raw与dump。对三分派输出分解差异报告，然后按**原厂记录CPU编号**选择当前分派检查九个float bits、192bit与12byte（预期PASS7，具体以运行输出为准）。对照report保留原H隔离warp差异与自拟合链差异，防止误把fit误差归均值。

测试默认引用生产NImageSourceCodeword24.cs源码，根线已接R03。这两个新增类暂不改变现TryDecode行为，A与SourceGrid桥接完成后由根线统一接入。当前B未经过C#编译/运行，不计整工具完成。

## 余边界

CPU7/10/普通路径已有源公式与候选分派，但此原厂运行仅CPU11。相异入口MXCSR的reciprocal影响不由managed API模拟。外部异常尺寸/非有限quad安全拒绝，与原未检查域分开。A的200×200固定window与源feature/fit尚未接入，不能把手工quad成功称自动识别完成。

## A继续实施中的文件（不在B补丁）

已写NImageSourceLocalPrelocator.cs（当前CPU7/10/11、[10,50,8,1,10,1]），NImageSourceCircleFit.cs（model1/strategy2/maxiter20/trim0/cutoff5，失败回M0、保留最后mask、最终RMS重算）。这两体是源逐操作候选草稿，尚未构建/运行或原厂微核验证，**未包含在r04-b-integration.patch**，不能据B单quad的0差证明它们。A仍需200×200 wrapper的7ff3c0固定fit/逆采样、四个100×100 crop、70f4e0 30窗实际旋转采样/峰选，与这两体连成完整opening/refinement；绝不以prelocator成功直接作为ring成功或把未支持精化假装返回0走fallback。后续只沿当前固定配置连接，不扩其他模型。

根线后续实际验证：R04SamplingChecks C# **PASS7**，日志 `analysis/r04-sampling-validation/85e70189a2de44c3b1792a144eb9a562/checks.log`，逐分派比较 `analysis/map-calibration/r04-sampling-managed/comparison.json`。这是B候选C#本体的有限验证，补丁尚待A/SourceGrid统一接入；不覆盖A四体。前文“未编译”描述保留为提案交付时状态，由本段更新。
