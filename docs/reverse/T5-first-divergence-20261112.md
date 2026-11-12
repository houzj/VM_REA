# T5 / G8 原厂宿主首处分歧（2026-11-12）

执行：CodeBuddy ｜ 任务：T5.2 ｜ 状态：**`blocked` / `unknown`，未定位首处分歧**。

## 1. 门禁与前置

`docs/VM-EXECUTION-GOALS-20261112.md` G8 要求：宿主与直接 SDK 各独立运行至少两次，比较输入像素、活动 ROI/Mask、Run 前缓冲、完整结果向量、BinaryImage、BlobImage、错误码和释放顺序，找到第一处分歧并归属公共框架或工具专属实现。

`docs/VM-REVERSE-GATE-MATRIX-20261112.json` 的 `noEvidenceAction`：证据不足时不改行为，记录 `unknown` 与解除条件。

## 2. 已具备的证据

| 项 | 状态 | 来源 |
|---|---|---|
| 原厂 EXE / SDK 身份 | **已固定 E0**：`VisionMaster.exe` `CF5730DF…D291`、`iMVS-6000PlatformSDK.dll` `F5129789…2A4E`，与 C03 归档一致 | `docs/reverse/T5-original-asset-gap-20261112.md` §1 |
| 直接 SDK 重复基线 | **已归档（历史）**：同一 `1.jpg` 两次独立进程均得 `47 / 973202`，47 行结果向量与两个图像缓冲哈希一致 | `docs/reverse/C03-same-input-differential-20261110.md` §43-49 |
| 原厂宿主历史观察 | **仅历史记录**：`45 / 971486`，首个已记录面积 `716928`（对应直接 SDK `718420`），5 行界面值可查 | C03 §33-41 |
| 静态调用边 | `CIMVSBlobFindModu::ProcessAlg` VA `0x18001FEC0`、Mask 生成 vtable `+0x48` / RVA `0x9C80`、`GenerateMaskImage` 区域构建与回写 | C03 §51-65、`docs/reverse/T3-execution-config-dataflow-20261112.md` §2.2 |
| 原厂 `.sol` 样本 | `CircleFind.sol`（3 份同哈希）已取得身份，但**不是**历史宿主所用的 `test1108.sol` | T5 资产清单 §2.2 |

## 3. 仍缺的前置（决定阻塞）

1. **`test1108.sol` 缺失**：安装根全树、`d:/MyWork/Rea逆向`、`d:/MyWork/测试图片` 递归均无命中。历史差分绑定该方案的「流程1、图像源1→Blob分析1」结构，无法用 `CircleFind.sol` 替代。
2. **匹配宿主环境未建立**：设备/认证运行状态、原厂启动与注册、实际加载的 DLL 集合未取证；C03 记录 `iMVS-6000PlatformSDK.dll`、`PlatformManager.dll`、`ModuleFrame.dll`、`ModuleProxy.dll` 与归档存在哈希差异。
3. **Run 前状态未采集**：活动 ROI/Mask 集合、像素缓冲、参数集在宿主侧的 Run 前快照从未取得（`ROIAreaType=0` 仅说明模式字段）。
4. **宿主侧重复运行未执行**：本次未启动 VisionMaster 窗口，未做宿主侧两次独立运行。

## 4. 判定

- 历史 `45 / 971486` 与 `47 / 973202` 是**输出层可见差异**，不是「首处分歧点」；两者之间的第一处运行时差异（输入像素 → ROI/Mask → Run 前缓冲 → 结果向量 → 图像缓冲 → 错误码 → 释放顺序）**仍未定位**。
- 按 `noEvidenceAction`：**不修改** Blob 私有算法、不按差异值反推修正、不把直接 SDK 结果当作宿主预期、不生成替代方案文件。
- 状态保持 `unknown`；G8 与 `T5-first-divergence` 维持 `blocked`，不计入任何完成度，也不阻塞不依赖原厂宿主的静态取证（T5.1、T5.4 已完成）。

## 5. 解除条件与复跑路径

| 前置 | 解除条件 | 复跑 |
|---|---|---|
| 原厂方案 | 用户提供 `test1108.sol` 或明确可替代的同结构方案及身份哈希 | `tests/NativeEvidence/C03/Get-HostIdentityPreflight.ps1 -Solution <path> -Image <path>` |
| 宿主身份 | 固定宿主、SDK、Blob 模块、样图与依赖的 SHA-256，记录实际装载模块 | 预检脚本输出 `summary.json` |
| 宿主侧运行 | 匹配环境下两次独立运行，采集 Run 前缓冲、活动 ROI/Mask、完整结果向量与两个图像缓冲 | 需先建立受控探针，本阶段未创建 |
| 首处差异归属 | 逐观察点比较两侧，定位首个可重复差异并归属公共框架 / 工具专属 | 差异报告 + 正负例复跑 |

## 6. 与 T5.3 的关系

CHECK-FR 判定未通过（`docs/reverse/T4-first-fidelity-slice-20261112.md` §5），按 `fidelityCorrectionBeforeExpansion` 继续冻结工具批次扩展。T5.2 的 `unknown` 与 T5.3 的 `blocked` 是两条独立门禁，不相互替代，也不因本阶段新增的 E0 身份证据而解除。
