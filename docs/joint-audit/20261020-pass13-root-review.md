# 第十三轮根线复核：只审标定十工具

本轮范围以[标定十工具清单](20261020-calibration-only-joint-audit.md)为准。ImageSource 与 owned Group 只审作为标定输入/出口的必要交接；二值化等非标定独有代码放到下一目标。工具算法、配置 UI 与后台仍属本目标，不因收窄工具数而排除。`tool-plugins` 在制文件只读。

## 新关闭的具名代码边

1. **共用原创建链**：[拖放→SDK 请求](20261020-pass13-calibration-create-call-chain.md)，SHA-256 `AEC0750D6B4427A9EE63B8ACADF064A3583D710AAE46D07F31B453748B447184`。根线直接核原 `ToolBoxData → ToolItemGrid → ItemsPage → DecoratorBase.DropItemsCommandExecute → ToolBoxItemData.AddNewItem → VmModule..ctor → SyncAddOneModule`，再用 REA 成员元数据与安装版 CIL 确认标定板原 Cs `object` 构造传 `"IMVSCalibBoardCalibModu"`。因此安装 XML `<name>CalibBoardCalib</name>` 不构成 SDK 创建名的依据。未观察十次原安装版动态实例 ID 或工具箱实际拖放；该静态共同边只核一次。
2. **标定加载文件控件**：[原 override→基类写参/预运行→ReadData(false)](20261020-pass13-readcalib-filepicker-dynamic-io.md)，SHA-256 `36AECD04E5961175A149F2D11ECCD2E5B79923A038AF7C7E70FE8F800E5CA2E0`。原安装 Control.dll REA 证 `ValueChangedCmdExecute`/`ReadData` 不直接调用 `InitDynamicIO` 或 `ReadData(true)`；不能从另一个通用 `UpdateModuleStateOperation` 直接推原文件选择自动迁移旧 Group 端口。间接 SDK SetParam/Run→动态 IO 通知仍是精确未知。REA 对大体积 `Apps.XmlParser.dll` 返回 64MiB 表示预算超限，父类只用已有反编译体、未冒称本次 CIL 验证。
3. **坐标系终端输出**：[原声明/ProcessOutput/Display 到当前结果、Group 和同轮显示](20261020-pass13-coordinate-terminal.md)，SHA-256 `F6BC96DBD6B8136C7DE39C422071980247D2E77ED6C8FF3207315808E9F1829D`。真实 owned Group 探针 10/10；根线在独立目录以 `--no-build` 重跑 exit 0，日志 `analysis/joint-audit/pass13-cal-coordinate-terminal/root-rerun/root-run.log` SHA-256 `3CBB70466813941C606689C542DCB3DB94B9748848DF0B04EDA07689DD9441A8`。当前暖缺 ROI 旧矩阵仍可导出，冷缺 ROI 拒绝未知原生初值且 Group 无出口；Undo/保存重开恢复。原厂部分发布失败、真实 MainWindow 旧显示和终端输出的另一个合法工具消费者仍未证。

这三项是**源码/当前运行边闭合**，并非十工具全部 J01–J12 完成。每个工具共享的 J01/J02 只能引用上述一次；不能因报告三篇就把分子加三。原 SDK 动态实例 ID、原文件刷新间接通知、用户长期无响应 UI 栈各保留一条共享或特有缺口。当前没有来源充分且归 main 独占的产品修复；已有 N 点开窗 `5N` 文档写入成本仍由 FCR-033 交插件所有者裁定，标定板/畸变显示差异仍需原版同条件结果，不擅自改。

主框架实施完成数仍 **41/96 = 42.71%**；历史插件实施自审 **10/10** 是另一口径。联合代码审核只报告已证边及精确未证边，不把人工/设备验收算进实施进度。本轮无产品文件改动，未推送；既有用户工作区 `.gitattributes`、`Start-Flow-Editor.cmd`、`tool-catalog.json`、`VisionEditor.WpfChecks/Program.cs` 与 `ISSUE-REVERSE-ANALYSIS-20261020.md` 均未纳入本轮提交。
