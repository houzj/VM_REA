# MF-01：最近打开的方案

## 原始源码确认

- `analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster.AppMainWindow/TitleMenueItemData.cs:476-505`：菜单项按最近方案路径打开，菜单显示文件名、悬停提示完整路径，并从 `ProjectManager.RecentlySolutions` 动态取项。
- 同文件 `OpenRecentSolution` 在创建、保存、加载期间拒绝打开，先停止运行再加载指定方案。
- `analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster.Project/ProjectManager.cs:864-926`：最近方案按 `LastTime` 倒序，超过 99 天移除；成功打开或保存后刷新路径及时间，最多保留六项并通知菜单刷新。原版持久化于应用 `.cfg`。

## 独立实现与实际操作验收

WPF 文件菜单增加“最近打开”；保存、另存为、打开成功后立即刷新。最近项以文件名展示、完整路径为提示，点击先停当前运行、处理未保存修改，再加载该方案；缺失文件可确认移除。独立项目采用 `%LOCALAPPDATA%/VisionEditor/recent-solutions.json`，不改原 VM 的 `.cfg`。

`VisionEditor.ShellChecks` 实际显示 WPF 主窗口：通过保存菜单记录两个不同路径，展开“最近打开”验证倒序，再点击较早方案，检查当前文档实际加载路径及菜单排序，并从偏好文件解析持久化记录。全部检查通过。测试将偏好路径重定向到本工作树的 `analysis/full-audit`，不写用户常用列表。

## 未验证原生差异

原生格式是 `.sol` 且依赖 `ProjectManager` 与 SDK；本线用项目现有 `.vmx.json` 文件模型独立实现。原版 `.cfg` 的精确 XML 序列化、启动欢迎页最近方案及远程方案行为没有复刻或宣称一致。此功能仅闭合文件菜单的本地保存—最近列表—重新打开路径。
