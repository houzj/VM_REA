# D02 第三批：实际 PrepareSave 与跨目录历史

基线 main `6a2cf54`。本批使用 main 已交付的 ImageSourcePlugin、ToolPluginRegistry、GraphEditorDocument 和 SolutionStore，不读取 tool-plugins。属于独立实现集成诊断，不代表原厂 VM 行为。

## 已核对调用链

`src/VisionEditor.Core/GraphEditorDocument.cs` 的 Save 对当前文档调用 PrepareSave，再 Rebase、存储和提交；Snapshot 保存 Doc/ProcedureId/BaseDir，Restore 将旧快照 Rebase 到当前基目录。Rebase 处理通用资源路径与内置 vision.image-file 的 Path，不处理插件私有 ImagePaths。

`src/VisionPluginContracts/Contracts.cs::ToolPluginRegistry.PrepareSave` 遍历节点并调用资产参与者。`src/VisionPlugins.ImageSource/ImageSourceAssets.cs::PrepareSave` 将 ImagePaths 等转换为绝对路径，并生成嵌入映射与资源。`OpenEncodedImage` 优先使用启用的嵌入映射，否则按当前基目录打开路径。保存前历史仍有相对路径、空映射和零资源。

## 实际操作与失败

诊断 `tests/VisionEditor.AssetHistoryProbe` 用自有有效 BMP：载入未经准备的相对路径方案 → 改流程名 → 跨目录另存 → Undo → Redo → 删除自有外部图像 → 重开嵌入方案。通过实际插件 OpenEncodedImage 比较编码字节及路径身份，未运行模块 ExecuteAsync 或原厂 SDK。

| 阶段 | 路径身份/字节 | 资源数 |
|---|---|---|
| 载入未准备方案 | true/true | 0 |
| 准备并另存 | true/true | 1 |
| Undo 到准备前快照 | false/false，DirectoryNotFoundException | 0 |
| Redo | true/true | 1 |
| 删除外部文件后重开 | true/true | 1 |

原文件在 Undo 时仍存在；失败来自新 saved 基目录解释旧 assets/rows.bmp。原结果为 **exit1、passed=false**，保存在 `analysis/full-audit/batch3-20261019/D02/asset-history-d657fad77f514ab28af698c9ebb08645/observations.json`，日志 asset-history.log。首次运行项目名为 AssetHistoryChecks，之后仅改名 Probe 并补 README，未更改诊断逻辑或抹去失败。

独立审查见 [复核](D02-asset-history-review-batch3.md)。不能对旧历史批量执行带 scope、运行状态和 I/O 的 PrepareSave；不能在 Core 硬编码插件私有字段。登记 FCR-024 的纯资产重定位合同，待用户选择跨项目交接；本批无产品补丁。多层历史、实际执行、原生方案及桌面差分仍开放。
