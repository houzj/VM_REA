# 第七轮：标定十工具的配置入口和参数文档链

范围只包括当前“标定”分类十工具及共用配置宿主。原始反编译代码、安装版 XML 均只读；探针输出位于忽略目录 `analysis/joint-audit/pass7-cal-config/`。本报告不以开窗成功推断算法、图像订阅、工具专用保存按钮或原厂桌面完全相符。

## 原版可确认调用边

- `VMControls.WPF.DataForUI.VmModuleDecorator.ConfigParamsExecute` 先由 `CanOpenConfigurationInterface` 检查禁用、加载失败及低权限用户的 `ConfigAvailable`；同一 `VMModule.Params` 已打开时返回，其他参数窗先关闭。`OpenParamsTabControl` 注册 `ModifyConfig` 撤销根操作，创建 `ParamsTabControlBase`、赋 `VMModule.Params` 和 `ROIPartUI`、指定 Owner、`Show`；关闭回调清静态窗口并删除空操作。源文件 `analysis/decompiled/VMControls.WPF/VMControls.WPF.DataForUI/VmModuleDecorator.cs` SHA-256 `FCF5ADFAF9CC6422E9F12B3FF19A3D880A5EF6EF4174FE8DFA5355FC31C7C9E4`，成员 `CanOpenConfigurationInterface/ConfigParamsExecute/OpenParamsTabControl/CurTabControlBase_Closed`。
- `ParamsTabControlBase.ParamsConfig` 转发给内部 `paramstab.ParamsConfig`，宽高与最大化按钮绑定该 DataContext，见 `analysis/decompiled/VMControls.WPF/VMControls.WPF/ParamsTabControlBase.xaml.cs` SHA-256 `D1F0AD7C6677124C713F324DD6960ED19DE8410C1598FD6EF55623B98C242593`。XML 控件层并非十份独立配置窗。
- 安装版 `IMVSNImageCalibModuAlgorithmTab.xml` SHA-256 `A83474FEC309E81EAED70366AD813A2D5A06A02A8DCC9F12D398079E16E5C3C3`：`ImageSource` 的 `GetFrontParamItemsOperation(IMAGE)` 初始化候选、`GetSelectedCombinationOperation(InputImage)` 读订阅、`SetCombinationSourceOperation(InputImage)` 写订阅；`CalibPointTotalNum` 为 `Integer`/`RW`/默认 9/范围 0..15，改值对 `RotPointTotalNum` 有 `SetMutualExclusion` trigger。三 operation 原体分别为 `GetFrontParamItemsOperation.cs` SHA `7DE4AF80A1228CD36FEEA628EE24868A656809CAD84E60D1CD8D629325F07132`、`GetSelectedCombinationOperation.cs` SHA `2E284885B38F2FAB0D93FD74C64DB482A25255DE4DEC14136CFBAD9BD888D86C`、`SetCombinationSourceOperation.cs` SHA `7CE50140DF9BC50B1E954A3C604B5FA294F91DB1318A22253FEEBB9811BE9444`。前者调用 `VmModule.GetFrontIOWithLeafItemsByName`，后者通过 `VmIO.RelateIO/RelateIndex` 改订阅，不能把界面下拉文本本身当连接。
- 安装版 `IMVSCameraMapModuAlgorithmTab.xml` SHA-256 `9832E201BC27E31413BD54BD92D55464B35C0657F681F7CA0059A695E0A77DFB`：`WeightFactor` 为 `Integer`/`Rw`/默认 20，`WeightFun` trigger 隐藏/显示它；`Integer.CurValueA` 有范围与步进处理，见 `analysis/decompiled/Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/Integer.cs` SHA-256 `530ADE2D749EB089211786A20909FA24771EE25E38C6B66B1CA4275412CEC278`。上述 XML 的本地派生资源 SHA 与安装原件一致。这里确认的是托管 UI/参数入口，不是原生 SetParam 成功或执行结果。

## 当前框架的去重分流与真实探针

`MainWindow.OpenToolConfiguration` (`src/VisionEditor/ToolConfigurationHost.cs`) 在节点拥有 `IToolConfigurationProvider` 时优先构造 `ToolConfigurationModelWindow(provider.CreateConfiguration(context))`，否则才用 `IWpfToolEditorFactory.CreateWpfEditor`。`ToolConfigurationWindowHost.Open` (`src/VisionPluginContracts/ToolConfigurationWindowHost.cs`) 根据方案/流程/节点身份复用或关闭窗口；`Apply` 回调调用 `GraphEditorDocument.SetProcedureNodeParameters` 并将前后参数记入 execution scope。窗关闭后 `ToolConfigurationModelWindow` 销毁 view/session。单独调试入口 `CreateToolEditorWindow` (`src/VisionEditor/ToolEditorHost.cs`) 则要求 `IWpfToolEditorFactory`，并绑定 `ToolDebugSession` 释放。这两个入口不能混称。

真实离屏 `MainWindow` 探针 `tests/CalibrationConfigBridge.Checks/` 逐个新建并打开十项：

| 工具 | 普通配置窗实际分流 | 专用调试工厂 |
|---|---|---|
| N点、N图像、标定板、畸变、平移旋转、旋转、坐标系、标定加载 | 均由 provider 进入共享配置窗 | 均有 factory；标定加载的 factory 是独立 `ReadCalibFileWindow` |
| 相机映射、映射标定 | 均由 provider 进入共享配置窗 | 无 factory；不能由此声称它们已有专用调试窗 |

探针进一步对 N图像 `CalibPointTotalNum: 9→8`、相机映射 `WeightFactor: 20→30` 实际调用配置 session 的 `State.CommitUser`；`GraphEditorDocument` 值和 Revision 更新、Undo/Redo 恢复、方案 `Save/Load` 后值保留，全部通过。此处 `Save/Load` 指 **主框架方案持久化**，并未调用两工具 XML/iwcal 专用导出。最终 stdout `analysis/joint-audit/pass7-cal-config/checks.log` SHA-256 `7AFC01687C62D3851E8C08B0E9B263A7C12B4AC88B8A12A148AEF0C53386A79C`；测试源 `tests/CalibrationConfigBridge.Checks/Program.cs` SHA-256 `CBE123ED0170313D8C188E2483419A651841EB85F47CF064C10CE980AEE7D925`，保存的方案亦在该忽略目录。运行命令：`dotnet run --project tests/CalibrationConfigBridge.Checks/CalibrationConfigBridge.Checks.csproj -c Release`。构建现有 `MainWindow.xaml.cs:122` 两处 CS8603 警告，本探针通过，未据此改产品。

## 已证与未证边界

本轮证实十项普通配置入口实际可达，以及两项代表性参数写入→文档撤销→方案持久化链。不能用这两项参数推定所有控件和业务保存一致；未跑十工具的每个订阅选择、异常写回、原生 SetParam、独立调试运行、工具导出/加载及最终用户桌面。原版 `CanOpenConfigurationInterface` 权限/加载失败门槛与当前 `OpenToolConfiguration` 的 `windowClosed/isLocked/activeRun` 门槛不同，尚缺同条件原机差分，不把差异自动判定为缺陷。`NImage` 初始化 IMAGE 候选与实际连接验收另由共享候选链审核，不在本报告重复计数。当前没有充分源码证据支持 main 的产品修复；工具专用配置仍归插件项目，`tool-plugins` 未修改。
