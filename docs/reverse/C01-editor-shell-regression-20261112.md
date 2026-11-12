# C01 编辑器壳与存储回归

日期：2026-11-12

通过：StartupShellChecks（启动身份、命令行方案实体、关闭链）、VisionEditor.Core 122 项、StorageBoundary 22 项、ShellSolutionPathChecks，以及 Persistence 30 项。

`VisionEditor.WpfChecks` 在 104 项通过后，测试夹具当前选中的流程没有 `vision.image-source` 节点而在 `First(...)` 停止；该失败无法仅凭此夹具归因于产品实现，保留为运行未确认。未启动用户界面。
