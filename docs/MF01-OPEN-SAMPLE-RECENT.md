# MF-01：打开示例后的最近方案同步

## 源码依据

`analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster.AppMainWindow/TitleMenueItemData.cs:507-530` 中 `OpenSamples` 选择真实示例文件，存在时经 `LoadSolution(path)` 加载；`ProjectManager.cs:296,339,744` 的加载成功路径更新最近方案。此项与前一轮最近菜单是同一数据链的另一入口。

## 实现与操作复验

独立壳的“打开示例”继续加载随程序部署的 `demo.vmx.json`，仅在用户从文件菜单触发且加载成功后更新最近方案并刷新子菜单。主窗口构造阶段加载示例不写用户最近列表。

`VisionEditor.ShellChecks` 显示实际 WPF 主窗口，先保存两个方案并通过最近菜单重新打开，再点击文件菜单“打开示例”：文档路径切到实际示例文件，最近菜单首项也切到该路径。专项全过。随后完整运行 `VisionEditor.WpfChecks`，456 项全过，并目检其主窗口截图 `analysis/full-audit/wpf-workflow-parity.png`，未见壳层布局回退。

## 原生差异

原版通过 `GuideHelper` 弹出 `.sol` 示例目录选择器；本项提交时独立项目只有随包的 `.vmx.json` 示例，故这里只闭合示例加载后的最近列表同步。后续选择器实现与限制见 [MF01-SAMPLE-PICKER.md](MF01-SAMPLE-PICKER.md)。
