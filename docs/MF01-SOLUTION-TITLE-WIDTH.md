# MF-01：长方案名的标题栏显示

## 源码证据

- `analysis/decompiled/VisionMasterMain/VisionMasterMain.AppMainWindow/TitleBar.xaml:26,35,317`：方案名经 `LongTextConvert`，字体为微软雅黑 13、最大文本宽度 128；标题按钮的提示保留方案路径，点击仍走 `OpenSolPathCommand`。
- `analysis/decompiled/VM.Controls/VM.Utility.Convert/LongTextConvert.cs` 和 `VM.Utility/MeasureText.cs`：使用 `FormattedText.WidthIncludingTrailingWhitespace` 逐字减短，尾部添加 `...`。

## 独立实现与实际界面

WPF 标题栏采用相同字号、字体和 128 DIP 文本测量截短方案名；文档路径与点击定位仍取未截短的 `doc.FilePath`。测试截图发现原右侧固定 130 宽列恰被窗口按钮占满、方案名完全不可见，已改为按内容测量的右列。

`VisionEditor.ShellSolutionPathChecks` 实际显示窗口，保存长中文方案名，检查文字宽度不超过 128、后缀为 `...`、控件可见且在主窗口范围内，并点击按钮确认仍提交完整实际路径；还复验了短方案名和缺失文件。截图由测试生成于本工作树 `analysis/full-audit/shell-long-title.png`，目检可见截短名称和右侧窗口按钮。`VisionEditor.ShellChecks` 复验标题栏命令及窄窗口工具栏滚动通过。

## 原生未验

原版方案名来自 `.sol`、本项目来自 `.vmx.json`；真实 VM 的字形测量、DPI 缩放和运行时列宽仍需并排差分。未据测试宣称全窗口像素一致。
