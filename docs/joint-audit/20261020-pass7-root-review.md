# 第七轮根线自审：仅标定十工具的共同宿主、失败与历史边

当前目标仍限定为“标定”十工具和它们必需的主框架边。本轮复用第六轮原件十项身份、目录创建及 N 点→标定加载→owned Group→坐标系正常矩阵链，不重复派发相同入口。此处核的是源码和受控运行，不把离屏窗口当用户桌面验收。

| 实质新增边 | 原始依据 | 当前验证和复核 | 未闭合 |
|---|---|---|---|
| 标定十项普通配置入口及两项参数文档写回 | 原 `VmModuleDecorator.ConfigParamsExecute/OpenParamsTabControl/CurTabControlBase_Closed`、`ParamsTabControlBase.ParamsConfig`、N图像/相机映射安装 XML 与三个 IMAGE parser operation；[分线报告](20261020-pass7-calibration-config-bridge.md) SHA-256 `5F2CFF8E299479E2EAF11C29026263879AC003458CA68FA3EBA52F8704E87EF1` | 实际当前 WPF `MainWindow` 十项都走普通共享配置窗；N图像/相机映射参数编辑、Undo/Redo、方案保存重开通过。分线 stdout SHA-256 `7AFC01687C62D3851E8C08B0E9B263A7C12B4AC88B8A12A148AEF0C53386A79C`；根线独立复跑 exit 0，日志 `analysis/joint-audit/pass7-cal-config/root-rerun.log` SHA-256 `CA2D4509EFD7356E514756D029FDE667FAF8828A771C06E96F7A8CA7E5F5A10F` | 十工具所有控件/订阅、专用导出、原生 SetParam、原版同条件权限/关闭；相机映射/映射标定无专用调试 factory。 |
| 标定加载失败、动态输出缺失和短矩阵 | 原 `ReadCalibFileModu.LoadFile/SetParam/Process` 与 `CoordinateModule.GetProcessCalibMatrixInput` 分支；[分线报告](20261020-pass7-calibration-failure-chain.md) SHA-256 `A10FECA18C4BF39D448945F880F89DDFD137F6E82CDB611DC4B7D2422F2B8B72` | 当前同一真实图 14 态：文件不存在/坏 XML 保旧矩阵，合法但缺输出节导致 Group 失败/坐标系跳过，修复恢复；热八项更新前缀并保旧尾，冷 1..8 项安全拒绝。分线 stdout SHA-256 `5934D860FD609E4EC44C65C31118ACE9F9565014B67125D5EAA0B38397E66222`；根线独立 Release 复跑 exit 0。 | 原版同构 Group 动态输出失败状态及 native 冷短矩阵尾部。当前错误码属独立安全适配，不称原厂等价。 |
| Group 矩阵连接撤销/重做及重开运行 | 原 `VmConnectionDecorator.DeleteConnectionCommandExecute` 注册 `RootOperation`，`VmModule.NextModules_CollectionChanged` 更新原生连接；参数订阅由 `VmIO.RelateIO` 另记历史；[本线报告](20261020-pass7-calibration-history-chain.md) | 当前连接断开→撤销→重做→再撤销，恢复后的 N点→文件→标定加载→Group→坐标系执行及保存重开四次运行通过。stdout SHA-256 `C04E3D7D766570C0B60806A5FC604A1DB0ADD6DA845DB1C7D2512E3869D5116B`。 | 原版同构整图、订阅历史和多层 Group；当前文档快照不是原 RootOperation 的同一实现。 |

配置桥接 `tests/CalibrationConfigBridge.Checks/Program.cs` SHA-256 `CBE123ED0170313D8C188E2483419A651841EB85F47CF064C10CE980AEE7D925`；失败链 `tests/CalibrationFailureChain.Checks/Program.cs` SHA-256 `425BA91ADBD1775720F4D059AEBD9DDF93CCFAB47F5E1C9EDBDAD2F0D6F48B18`；历史链探针见其专报。失败链曾有初版哈希，分线自审增强 warm8 前缀断言后以本报告所列最终 SHA 为准，旧日志仍不作最终证据。

这些是 J03/J09/J11 的具名切片，并没有关闭十工具全部 UI、后台、算法及全链 J01–J12。本轮没有原版证据支持且属于 main 独占的产品修复，故框架**实施总进度仍 41/96 = 42.71%**；十工具既有插件线 10/10 实施计数是另一口径，不能代替本次联合审核。人工验收单列。保留原始失败与边界，未改 `tool-plugins`，未推送。
