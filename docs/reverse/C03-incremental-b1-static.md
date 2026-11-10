# 渐进落地第二批：B1 纵切静态取证、输出名对照与方案文件字节级比对

日期：2026-11-10 · 机器证据卡：`analysis/full-audit/batch1-20261107/C03/c03-incremental-b1-static.json`（`C03-INCREMENTAL-B1-STATIC-001`）

## 边界

本批只做本地静态取证：读取 8 个纵切源码文件、C01 运行报告与 3 份 vmx 方案文件，用 SHA-256 锁定 16 个引用。**未执行任何测试、未改 `src/` 产品代码、未提交、未配置 REA provider**。B0 仍 `open`，B1/B2/B3 仍 `blocked`。

## 准确类型与成员（source-confirmed）

| 文件 | 类型 | 主要方法 |
|---|---|---|
| `ImageSourcePlugin.cs` | `ImageSourcePlugin` / `State` / `Runtime` | `CreateRuntime`、`GetImages(NodeDocument, ModuleExecutionScope)`、`SubmitImage(...)`、`Reset(...)`、`CreateEditor/CreateWpfEditor`、`ExecuteAsync(ModuleExecutionContext, CancellationToken)` |
| `ImageAcquisitionPlugin.cs` | `ImageAcquisitionPlugin` / `AcquisitionImageConfig` / `State` / `Runtime` / `AcquisitionFailure` | `LightService`、`Paths`、`Configs`、`ReadUiDirectory`、`AppendSdkImageDirectory`、`PrepareSave`、`ExecuteAsync` |
| `Algorithms.cs` | `Algorithms` | `Threshold`、`ThresholdBelow`、`ConnectedComponents8`、`FitCircle` |
| `BuiltinModules.cs` | `BuiltinModules` / `Adapter` | `RegisterAll(ModuleRegistry)`、`ExecuteAsync` |
| `OfflineCircle.cs` | `OfflineCirclePlan` / `OfflineCircleRunResult` / `OfflinePlanStore` / `OfflineCircleExecutor` | `Load`、`Save`、`Validate`、`Require`、`Run` |
| `ImageCanvas.cs` | `ImageCanvas` | `SetImage`、`SetOverlay`、`ZoomBy`、`ResetView`、`ShowActualSize`、`ClearImage` |
| `OverlayPng.cs` | `OverlayPng` | `Write`、`Chunk` |
| `GraphExecutor.cs` | `GraphExecutor` | `RunAsync(ProcedureDocument, string baseDirectory, IProgress<ExecutionEvent>?, …)` |

输入契约字符串可见 `ImagePaths`、`ImageConfigList`、`EmbeddedImageResources`、`ImageSourceType`、`ModuStatus`、`CurImageName/CurImagePath`；结果侧可见 `BlobNum`、`TotalArea`、`Count`、`BlobModeUnavailable`。

## 输出名清单对照（430 个本地 `.cs` 全量检索）

| 组 | 总数 | 整串字面量命中 | 未命中 / 待取证 |
|---|---|---|---|
| `first` / `second`（ImageSource） | 9 | **9/9** | 无 |
| `blob` | 13 | **4/13** | 9 个 `Components.*` 复合名（`Area`、`BlobNum`、`BlobRect.*`、`CentroidPoint.*`、`TotalArea`）未作为整串出现 |
| `group` | 2 | **1/2** | `width` 未命中；源码中可见首字母大写的 `Width`，大小写契约待核对 |

- `group.failureOutputs` 为空，`group.reloadedOutputs` 与 `group.outputs` 一致（`image`、`width`）。
- `Components.*` 是否由 `Components` 输出投影动态拼接，**未取证，不推断**；需追输出投影调用链后再登记。
- 输出名一致**不等于**原厂行为等价。

## 三份 vmx 方案文件（字节级身份）

| 文件 | 字节 | SHA-256 | Procedure 数 | Resources |
|---|---|---|---|---|
| `image-source-vertical-slice.vmx.json` | 2196 | `bde9847f…` | 1 | 0 |
| `image-source-blob-vertical-slice.vmx.json` | 3233 | `aff5ade1…` | 1 | 0 |
| `image-source-group-vertical-slice.vmx.json` | 4640 | `e534b43a…` | 2 | 0 |

三者顶层键一致（`FormatVersion/Id/Name/Procedures/Resources`）；Group 方案多一个 Procedure，字节数随链路增长。只登记可观察的静态差异，未修改方案文件，也未把结构差异写成行为结论。

## B0 剩余边界（本批未关闭）

差分仍为原厂 `45/971486` 对直接 SDK / 本地 `47/973202`（+2 / +1716）。仍需原厂运行时证据：非空 `InputROI/InputMask` 或生成区域对象、45 元素结果向量与 `BinaryImage/BlobImage` 缓冲、Run 前确切像素/灰度/边缘字节、`FullScreen [1,0]` 到 `param_1+0x20/+0x38` 的转换结果、生成区域坐标与输出标志的规范化对照。证据门 `c03-next-evidence-gate.json` 明确：取得这些之前不改产品算法。

## 命令与决策

`dotnet run --project tests/VMVerticalSlice.C01/VMVerticalSlice.C01.csproj --no-restore` 仍为 `registered-not-executed`；本批 `commandsExecuted=0`、`productChanges=[]`。历史 `18/18 passed` 不是本批复跑。

**决策**：B0 仍 `open`、B1 仍 `blocked`。静态成员与方案文件身份已补齐，但批次阻塞不解除；不把输出名一致或方案可比对写成 parity，不把 unknown 写成已确认。若要继续推进实施类动作，需先按门槛取得原厂运行时证据，或由用户明确说「开始测试/验证」执行 C01 复跑。
