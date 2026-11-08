# C03 ROIAreaType 与 native 分支

证据写入 `analysis/full-audit/batch1-20261107/C03/source-runtime-roiarea-branch.json`。

运行时默认导出中的 `ROIAreaType=0` 与 Blob native `ProcessAlg` 分支对齐。native 在 `this+0x28 == 1` 时直接把 `param+0x118` 交给 SDK ROI 设置，否则调用 `GenerateMaskImage` 候选并走 PreproMask/RegionImageAssistant 路径。因此当前 VM 默认状态实际选择的是 Mask/PreproMask 分支，不是直接 ROI 分支。

`OutputMaskImage=False` 和 `FixtureInfo` 属于同一运行时 ModuleFrame 状态，但其具体 native 输出和坐标转换仍分别保持未决。
