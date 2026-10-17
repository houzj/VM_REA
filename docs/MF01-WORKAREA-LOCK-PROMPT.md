# MF-01 工作区锁定确认闭合

## 原始源码确认

- `analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster.AppMainWindow/TitleMenueItemData.cs` 的 `AreaLocking`：未设置 `IsNotifyLock` 时先打开 `LockDlg(false)`，读取“不再显示”复选值；只有“是”才设置 `IsWorkAreaLocking=true`。已设置跳过提示时直接锁定。`AreaUnLocking` 直接解除锁定。
- `analysis/decompiled/VisionMasterMain/VisionMasterMain.AppMainWindow/LockDlg.xaml(.cs)`：350×200 提示窗，正文、底部“不再显示”、是/否按钮，按钮分别设置 `DialogResult`。`MainWindowData.IsNotifyLock` 通过配置项在用户级持久保存。

## 本线 WPF 实现及行为

- 工具栏锁定按钮现在先按原始事件顺序进入专属 WPF 确认窗。选“否”不更改锁状态；选“是”复用既有 `ToggleLock` 使编辑入口及画布手势受 `isLocked` 限制；解锁不弹窗。勾选“不再显示”后，再次锁定直接应用。
- 跳过提示偏好写入独立用户级 `LocalApplicationData/VisionReplica/shell-preferences.json` 的 `IsNotifyLock` 字段，不随方案保存。写入使用临时文件加替换；测试重定向到本工作树的 `analysis/full-audit/shell-lock-preferences.json`，不改真实用户设置。
- 本地提示文字为独立中文适配；原厂 `Lang_LockingTips` 的资源文案和皮肤没有从本次解包证据中确认，不能宣称逐像素一致。

## 实际操作验收及开放边界

`tests/VisionEditor.ShellChecks` 在真实 WPF 窗口中点击工具栏锁按钮，依次验证“否”保留编辑、“是”禁用新增流程、偏好文件实际保存、解锁恢复编辑、再次锁定跳过确认；连同前两项专项共 22 项通过。测试渲染 `analysis/full-audit/shell-lock-confirmation.png` 用于本地视觉复核。现有 `VisionEditor.WpfChecks` 对画布锁定手势另做回归。真实 VM 的提示文字、完整皮肤、快捷键/右键所有入口、配置文件存储位置及跨启动 UI 差分仍未验证。
