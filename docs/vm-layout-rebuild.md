# 主界面布局重建：第一版

2026-10-14。依据 docs/vm-ui-decompilation.md 中真实主视图链路调整独立编辑器。

已实现：左侧窄工具栏、可展开模块工具箱、流程选择栏及画布；右侧图像/模块结果选项卡；底部历史结果/帮助/运行记录；双击节点参数设置；紧凑浅色节点及橙色上下折线连接；暗色主题。

验证：编译零警告零错误。编辑器事件级 smoke 检查模块拖入、端口连接、节点拖动、参数应用及非法参数拒绝、撤销重做、保存重载、三个模块独立执行。结果和实际生成的窗口截图见 analysis/vm-layout-preview.png.checks.json 与 analysis/vm-layout-preview.png。这是程序内事件验证，不是对原 VM 的真人操作回放或逐像素对比。

当前限制：五个内置模块；只支持独立 vmx.json 编辑；类型化数据连接保留原型语义，尚未实现原 VM 独立执行序与数据订阅的全部规则。参数窗口的完整分区、图像缩放工具、原始模块树、ROI 编辑、运行模式和全部工具栏功能仍待重建。历史目前按流程运行追加概览，不是原 VM 的完整模块结果记录结构。未加载原 VM 程序集作为运行依赖。窗口系统边框仍与原 VM 的自定义窗口不同。

入口：`Start-Flow-Editor.cmd`。该入口先把当前工作树的 `src/VisionEditor/VisionEditor.csproj` Release 构建到唯一的 `analysis/manual-acceptance/flow-<GUID>/build` 目录，再启动其中的 `bin/VisionEditor/release/VisionEditor.exe`；不再引用已移除的 `dist/VisionEditor-WpfCompoundLeaf` 产物。
