# 渐进落地第一批：B0 S0/C03 前置补证 + B1 最小纵切静态取证

日期：2026-11-10 · 机器证据卡：`analysis/full-audit/batch1-20261107/C03/c03-incremental-batch0-b1.json`（`C03-INCREMENTAL-B0B1-001`）

## 方法与边界

按 `reverse-engineer-anything` 的「问题清单 → 复用既有证据 → 区分观察/推断/未知 → 保留未决项」流程执行。当前主机 REA MCP 与 native provider 不可用（ghidra `not_configured`、hopper `unsupported_host`、ida `not_configured`，且目标 PE 无 CLI 目录），本批**不新取函数体、不修复 provider、不执行方案测试、不改产品算法**。所有结论只来自本地既有静态/运行证据，引用文件均带 SHA-256。

## 问题清单

| 问题 | 状态 | 结论 |
|---|---|---|
| 差分两侧是否仍开放 | 已答 | 原厂 `45/971486` 对直接 SDK / 本地 `47/973202`，delta `+2 / +1716`，原样保留 |
| 5 项 unknown 能否本批关闭 | 未决 | 需区域对象、像素缓冲与 native 结果数组 |
| S0 三退出条件状态 | 已答 | `S0.1/S0.2` 历史报告且本批未复跑、`S0.3` 已满足，总判定 `S0-not-fully-closed` |
| REA 补证边界 | 已答 | 证据门要求未捕获区域对象/像素缓冲/native 结果数组前不改产品算法 |
| 纵切链路有哪些既有证据 | 部分 | C01 运行报告、三份方案文件、C03 证据卡与源码文件均在；准确成员与显示叠层待核对 |
| 本批是否复跑 / 改算法 | 已答 | 否；命令只登记待执行，产品改动 0 |

## 已确认（观察）

1. 本地链路与直接原厂 SDK 在同一 `1.jpg` 上一致；差分**不在**本地图/会话/保存重载管道。
2. C01 历史报告记录 `count 47 / blobNum 47 / totalArea 973202`、13 个 Blob 输出名、`reloadedCount 47`、`sessionStatus 0`。这是**历史运行报告**，不是本批复跑。
3. 证据门 `c03-next-evidence-gate.json` 明确「未捕获区域对象、像素缓冲与 native 结果数组前不改产品算法」。
4. 最小纵切三份方案文件与 `tests/VMVerticalSlice.C01/` 工程存在，可作为 B1 后续取证入口。

## 推断与未决（不得写成结论）

- 剩余边界：wrapper 侧 `FullScreen [1,0]` → `param_1+0x20/+0x38` 的转换、SDK Run 前确切输入图像/边缘字节。
- 「单范围直传」与 `GenerateMaskImage` 生成掩膜两条分支**无法由现有证据选定**，登记为 unknown。
- 5 项 unknown 逐项保留：`param_1+0x20/+0x38` 集合基数与转换结果、Run 前像素/灰度/边缘包含、45 元素向量与 `BinaryImage/BlobImage` 缓冲区、R8/R9 布局与首个间接调用目标、原厂异常/取消/释放路径。
- 纵切 B1-3 显示段的准确类型/成员、Group 输出名清单与方案文件字节级比对均未做。

## 纵切四段状态

| 段 | 状态 | 主要未知 |
|---|---|---|
| B1-1 图像源输入 | 历史运行报告 | 准确类型/成员待核对；原厂 Run 前像素字节未捕获 |
| B1-2 Blob 模块与结果向量 | 源码+运行证据，差分开放 | 45 元素向量、图像原始缓冲未捕获 |
| B1-3 显示与 overlay 输出 | 仅文件存在 | `ImageCanvas` / `OverlayPng` 准确成员待核对 |
| B1-4 纵切闭环复跑 | 已登记、未执行 | 需用户明确说「开始测试/验证」 |

## 命令登记（待执行，本批不运行）

```text
dotnet run --project tests/VMVerticalSlice.C01/VMVerticalSlice.C01.csproj --no-restore
```

按项目硬规则，实施完成报告后不自动进入方案测试；复跑后须记录退出码与新报告哈希，再更新 `S0.1/S0.2` 状态。

## 决策

**B0 仍 open、B1 仍 blocked**。本批交付的是可复核的静态证据闭环，而不是算法补丁或 parity 结论：不改产品算法、不把本地/SDK 一致写成原厂等价、不以测试通过冒充 S0 关闭。后续要闭合根因，需要可查询的原厂运行过程捕获非空 ROI/掩膜对象、45 元素向量与图像缓冲，或修复 REA native provider；两者都不在本批授权范围。
