# MF-01 打开示例的加载边界

原始 `analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster.AppMainWindow/TitleMenueItemData.cs` 的 `OpenSamples` 先经 `GuideHelper.OpenSolution(true, ref path)` 选择示例，只在 `File.Exists(path)` 时调用 `LoadSolution(path)`；后者在加载前调用 `StopRun`，其含义是关闭连续运行开关而不取消当前单轮。`GuideHelper.cs` 保留示例目录偏好；选择器取消时的原版 INI 写入行为没有独立实机复核。

当前 WPF 示例菜单保留已验的取消不改变方案/目录结果。选中不存在的文件只给出状态提示，不停连续运行、不加载、不进最近方案。选中存在的文件时才停连续；独立执行器若有活动单轮，则保持其 token 和现有文档，提示等待，完成后再次选择才能加载。成功加载刷新最近方案。

ShellChecks 以真实 WPF 菜单事件验证先选择后停连续、活动单轮保护、成功加载与最近记录、取消和缺失文件边界；测试程序的文件菜单图 `analysis/full-audit/shell-file-menu.png` 已目检示例入口可见。原版 `.sol` 选择器像素、全局脚本窗口关闭与 SDK 方案加载仍未验证。
