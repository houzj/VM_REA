# MF-01 最近方案的显示与运行边界

原始 `analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster.AppMainWindow/TitleMenueItemData.cs` 的 `GetRecentlySolutions` 使用 `SolutionItemInfo.FileName` 作菜单名、`SolutionPath` 作完整路径；`SolutionItemInfo.cs` 的 `FileName` 明确为 `Path.GetFileNameWithoutExtension`。`OpenRecentSolution` 先执行 `StopRun`，后调用 `ProjectManager.LoadSolution(path)`；`StopRun` 只把连续运行开关置为 `false`，不取消正在执行的单轮。

当前 WPF 最近菜单改为同样的无最后扩展名显示，完整路径保留在提示和菜单数据中。点击条目先停止连续运行；独立执行器当前单轮尚未结束时，保持这一轮和现有文档，提示等待，待结束后可再次点击加载。加载成功仍刷新最近顺序；既有缺失文件移除确认沿用独立实现。

ShellChecks 实际触发 WPF 最近菜单，验证显示名、完整路径、连续状态停止而单轮 token 不取消、运行中不替换、结束后再次点击加载成功，并由测试程序输出 `analysis/full-audit/shell-recent-menu.png` 供目检。原版 `.sol` 单扩展名与当前 `.vmx.json` 双扩展名的展示会自然不同；原版活动单轮加载和缺失文件警告的完整 SDK 行为未动态核验。
