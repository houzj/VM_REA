# MF-01：示例方案文件选择与目录记忆

## 原始证据

- `analysis/decompiled/VisionMasterMain/VisionMaster.AppMainWindow/GuideHelper.cs:65-112`：`OpenSolution(true)` 以 `OpenSample` 记录的目录为初始目录；无记录时采用程序示例目录；显示系统文件选择框，筛选 `.sol`，选择后记录所在目录，取消时不加载。
- `analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster.AppMainWindow/TitleMenueItemData.cs:507-530`：`OpenSamples` 仅在所选文件存在时调用方案加载，并走原有最近方案刷新路径。

## 独立 WPF 实现和验收

文件菜单“打开示例”现在先显示 Windows 文件选择框，默认从随包示例目录或上次选择目录开始；取消不更改文档，选择有效文件后记住目录、执行未保存方案确认、加载方案并刷新最近方案。启动时演示方案的自动加载继续保持，不触发选择框且不污染最近列表。目录偏好独立保存在 `%LOCALAPPDATA%/VisionEditor/sample-directory.txt`。

专项 `VisionEditor.ShellChecks` 显示实际 WPF 主窗口并点击菜单，通过可控的选择器返回随包真实 `demo.vmx.json`，验证初始目录、加载后的实际 `doc.FilePath`、最近菜单首项和目录文件；再通过同一菜单模拟取消，验证当前方案不变。目录文件在测试中重定向到本工作树。完整 `VisionEditor.WpfChecks` 456 项通过。

## 仍待原生差分

原版 WinForms 通用对话框筛选 `.sol`；独立项目使用 Windows WPF 通用对话框与 `.vmx.json`。专项自动验收覆盖实际 WPF 菜单和加载结果，但系统文件选择框的像素外观尚未单独截图对照真实 VM，不能宣称该子窗口完全一致。原版通过配置 INI 记住目录，本项目使用独立文件。
