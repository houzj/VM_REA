# 第十轮：映射标定双图模式进入标定结果消费者

本切片补第八轮仅审的相机映射**文件模式**之外，映射标定自己的 `InputType=1` 双 IMAGE 输入链。仍只统计“标定”分类及其必要的主框架 ImageSource、Group、标定加载和坐标系；不修改 `tool-plugins`。共享目录、配置窗和文件模式不重复计数。

## 原件源码边

原 `CIMVSMapCalibModu::Process` RVA `0x21690`：图像模式在非教学重用分支调用 `GetInputImage`，两侧均未更新时返回 `E0000314`；若一侧先到则缓存 pending，双侧齐备后执行两侧预处理及 `RoiFix`。反编译体 `worktrees/tool-plugins/analysis/map-calibration/ghidra/module-analyzed/180021690.c` SHA-256 `112ABD26934DD4288BD4A99F0675196A895B671964255AE80C9290EE1005C028`。`ProcessAlg` RVA `0x21c50` 为 `InputType=1` 取得双图算法参数，分别通过虚表 +0x20/+0x50 传 source/target 图；两组非空 ROI 分别用 +0x30/+0x60 传入，调用 Run、GetResult，再按成功/路径/刷新状态调用 `WriteXmlData`。该体 `.../180021c50.c` SHA-256 `643F1892E704C1039C234485163CB653E61D01CC7F67F8B88C9C3A7DEFAE3972`。`ProcessOutput` `.../1800220b0.c` SHA-256 `12F085ABAC1773CD4D3D93A7B158FDFE527317B3A0E0FFCDF9C177CE344DC9F4` 发布 `CalibStatus` 等结果。上述是原件模块调用边；其原版 VisionEditor 同构多节点图未运行。

## 当前实际图

`tests/CalibrationMapImageChain.Checks/Program.cs` SHA-256 `C91B36853A7AEBE441183B8EFB7651D88B5D2F2A2DE40B82090F15B44F00262E`，工程 SHA-256 `F15D1ABC764A3C98DC10D93B812F9B01295FDD0FBF8B1B4B89F06BFE266C7ECA`。在 owned Group 里放两个真实 ImageSource，分别提交已有原 SDK 图像夹具 `frame-0.bmp`/`frame-4.bmp`，把 `Image` 连到映射标定 `InputImage1/2`；映射标定 `InputType=1, WeightFun=1, RefreshFileEnable=true` 写 `.xml`，`CalibStatus` 触发标定加载，Group 将九项矩阵交给外层坐标系。另一只 ImageSource 给坐标系提供必需的图像前置。

`dotnet run --project tests/CalibrationMapImageChain.Checks/CalibrationMapImageChain.Checks.csproj -c Release` 退出0。首跑、保存/重开后删除旧 XML 再跑各四项通过：两图源、映射标定、加载、Group、坐标系均 Success；映射标定识别源/目标各 168 个角点；每轮新 XML 均由加载器发布九项矩阵，Group 值与加载器一致。日志 `analysis/joint-audit/pass10-cal-map-image/run.log` SHA-256 `AC2578A7849BA0BDA0C9BFAB65966C64CF8187774C30107C3B71A44E5E1FB522`，最后 XML SHA-256 `364DB7F15508615E45F0BD029047F8EAC8A9D40B8EAE5A15697A1A4B18384B21`。首次新工程缺 `System.IO` 编译失败、其次夹具路径误指 main 而非只读取证工作树，失败日志分别留在同目录 `first-compile-failure.log`、`fixture-path-failure.log`，修复探针后才计最终成功。

本切片证明当前双图数据运输及文件结果消费，不证明原件/当前 ROI 两侧选择、教学路径、数值逐位相同或原版整图同构运行。原插件已存在多组 SDK 图像差分和专属 Host 测试；这里只增一条**跨主框架至消费者**的操作证据，不能据此宣称映射标定所有 J01–J12 边完成。
