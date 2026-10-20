# 标定板最终手验启动只读审查

2026-10-20。本轮未build、未launch、未终止/干预旧验收PID。根线等待其它工具冻结后统一执行。

最短命令（PowerShell7，在本工作树）：

```powershell
pwsh -NoProfile -File .\tests\Launch-CalibrationAcceptance.ps1 -Tool '标定板标定'
```

不要传SampleCalibFile（仅Loader接受）；无需CalibrationPair（Board分支先return）；不要WaitForExit以便启动后继续核就绪。脚本本身每次新GUID独立build目录，构建失败先throw、不启动；固定WorkingDirectory为本工作树。窗口可见是用户已明确要求的交互验收。脚本不会查杀旧PID、清旧产物或写别的worktree。

真实执行链：Calibration.ManualAcceptance→真实MainWindow→NewSolution→生成480×400 Gray8棋盘BMP（30像素格）→真实image-source与calibration-board节点、execution边及Image→InputImage绑定→源路径参数→RunProcedure→检查board NodeRunStatus.Success→选中board、RefreshAll、OpenToolConfiguration。ready标记只在运行成功且配置打开后输出；生成样本为独立合成fixture，不冒原厂图片或设备验收。MainWindow当前仍有doc/results/selected字段、RunProcedure/RefreshAll/OpenToolConfiguration私有成员和Board/ImageSource注册，静态反射入口匹配。项目引用真实VisionEditor，非第二宿主壳。

日志：脚本输出新analysis/manual-acceptance/<GUID>目录，保存build.log/stdout.log/stderr.log/launch.json。首先核build成功、launch.json的新PID和tool/module，然后等待stdout的ASCII `MANUAL_ACCEPTANCE_READY vision.calibration-board source -> board`，同时stderr无新异常；仅launch.json存在不能证明就绪。人工验收userAccepted仍false，不自动改。窗口橙色“插件线验收”标记可区分旧窗口。旧PID保持不动。

旧实证：analysis/manual-acceptance/72145c93fbba47d5af6e46c34449c780记录Board启动且stdout已READY，旧build0error/2历史CS8603warning。它不是当前最新产品的复验。当前源码静态可用；最后统一构建/启动后须检查本次新日志，不把旧READY复用。stdout中文路径可能受终端编码显示影响，ASCII就绪标记可准确查找，实际生成fixture路径在独立board-fixtures/GUID下。

本次只读身份：Launch脚本SHA C2F9709690AE81B963840AA057DAFD56222DE4770E2A0828A7A4AAFB6BA838E4；Program.cs SHA2221C79EDF62B15F4A577DADB8156BB7E6DAAD86AA0785269B69CEA9EBA48B84；csproj SHA71D589A6CACD216AB258C955C6B7DA3BDCC6A67387B2EC234184063930FDA57D。
