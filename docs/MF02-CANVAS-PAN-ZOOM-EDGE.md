# MF-02 画布拖拽与滚轮边界闭合

## 原始依据

- `analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/ZoomArea.xaml.cs`：`_ZoomTicks` 为 0.1 至 2.0 的二十档；`OnPreviewMouseWheel` 先执行 Shift 横滚，再执行 Ctrl 缩放；`OnChangeZoom` 只在滚轮增量大于或小于零时修改档位。
- 同文件 `OnMouseLeftButtonDown`、`ScrollViewer_MouseMove`、`OnMouseLeftButtonUp`：Ctrl 按下记录鼠标坐标，每次移动先计算与上一次坐标之差，然后更新记录；抬起释放捕获。原生 `ViewArea` 还有边界与边距处理，不能仅凭本次测试宣称全部视口公式等价。

## 本次独立实现与验收

- `WorkflowCanvas.cs` 的画布平移改用逐次鼠标位移。到达左边界后反向拖动 20 像素立即让水平视口偏移 20 像素；旧的按下点累计公式会一直停在边界。
- Ctrl 滚轮零增量不再误退一档；正负增量各移动一个 0.1 档。Shift 横滚、锚点缩放和普通滚轮仍由原有路径处理。
- 专项 WPF 验证在真实主窗口/ScrollViewer 中调用画布平移及缩放操作：检查边界反向、档位、模型修订未变及方案保存重开。这一项是实际 WPF 控件状态检查，但没有用系统鼠标注入 Ctrl 拖动。`analysis/full-audit/canvas-pan-edge-reversal.png` 是专项测试的 WPF 渲染截图，截图代码仅在测试项目。

## 尚未验证的原生差异

- 原生 `ViewArea` 在画布边缘调整 `GridContent.Margin` 的整套公式、实际系统鼠标连贯拖动、不同 DPI 和多个窗口尺寸仍未做 VM 同状态差分。本次只关闭边缘反向拖拽时序和零增量档位差异。
- 视口为当前会话的每流程状态，保存的方案数据不会包含鼠标位置；保存重开验收只验证画布操作没有污染工作流模型。
