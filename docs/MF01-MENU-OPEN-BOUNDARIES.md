# MF-01 主菜单剩余边界审计

本表依据 `analysis/decompiled/VisionMaster.Exe.Audit/menuitems.xml`、`VisionMaster.AppMainWindow/TitleMenueItemData.cs` 和 `VisionMasterMain.AppMainWindow/TitleBar.xaml`；当前 WPF 入口见 `src/VisionEditor/MainWindow.xaml.cs` 的 `BuildChrome`。源码菜单项的存在不等于本项目已经具备相应后端。

|原始入口|原始命令链|本项目状态与归属|
|---|---|---|
|设置：权限、软件、方案、运行、采集策略|`AuthSet/VMSet/Solset/ProcRunSet/CollectSet` → 软件设置窗和 SDK/方案状态|当前“所选模块参数”并非上述入口。需要对应配置数据与持久化契约；不从 Shell 造空窗。|
|工具：标定向导与外部工具|`CreateCalibGuide/OpenExe/OpenExeByLang/OpenModuleParamTool`|当前撤消/重做/校验/适应画布是独立 WPF 操作，非原始工具菜单目录。外部工具产物及配置缺失。|
|系统：日志|`LogShow` → `UILogManager(true).ShowDialog()`|本项目无原生日志模型和窗口；执行历史表不能替代日志管理器。|
|系统：通信、相机|`CommSet/CameraSet` → 动态通信 DLL、`VmSolution.Modules`、`GlobalCameraManager`、登录权限|属于插件与 SDK 契约；相机工具栏当前仅刷新独立相机提供器，不能声称等同原机相机设置。|
|帮助：本地手册|`HelpDoc` → `GuideHelper.GetHelp(true)` → 本地帮助文件或“帮助文档缺失”告警|当前工程无手册文件；静态操作提示不是原生帮助文档。|
|运行模式|`TitleBar.xaml` 的 `FrontedButton` → `OpenRunSetCommand`|当前仅静态文字。需全局运行参数配置服务和窗口，不能用虚设切换代替。|

另：`TitleBar.IsButtonClickEnabled` 绑定 `MainView.IsShowProgressMask` 的取反，同时约束菜单和工具栏；当前框架缺该操作掩罩状态，不能简单用 `activeRun` 等同。已闭合的文件菜单、最近方案、锁定、标题栏和 NOItem 状态不在本表重复。以上为源码审计，未宣称运行验收或原生像素一致。
