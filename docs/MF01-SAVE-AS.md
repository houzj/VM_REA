# MF-01 另存为方案验收

原始 `analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster.AppMainWindow/TitleMenueItemData.cs` 中，`SaveAsSolution` 进入 `ProjectManager.AnotherSaveSolution`；同目录 `VisionMaster.Project/ProjectManager.cs` 的后者从独立 INI 键 `SaveSol` 读取初始目录，只在真正保存完成后记住目录、更新最近方案。取消选择直接返回失败。

独立 WPF 壳将文件菜单“另存为”和未命名方案首次“保存”接到可注入的文件选择器，目录记忆独立于 `OpenSol`；选择取消不改文档、目录和最近记录。保存成功才记目录并刷新最近记录，异常写入不提交新文档路径或历史。已有路径的工具栏“保存”直接写当前方案，不再次弹出另存选择器。

专项 ShellChecks 在真实 WPF 文件菜单和工具栏触发命令，复验取消、写出有效 `.vmx.json`、重开后的初始目录、非法路径保存失败及再次工具栏保存的 JSON 数据；全线 WPF 回归另行记录。原版 FileSaveDialog 的加密按钮、密码方案、`.sol` 格式、进度与远端保存均未实现，本项没有宣称这些行为等价。
