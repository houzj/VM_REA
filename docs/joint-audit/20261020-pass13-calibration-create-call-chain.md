# 第十三轮：标定工具箱拖放到 SDK 创建的原始共同调用链

本报告只关闭十项标定工具共用的**原始代码路径**，不把静态路径当成十次原厂实际拖放或动态实例 ID。与[第六轮当前宿主实测](20261020-pass6-calibration-catalog-host.md)合用；无产品改动、无 `tool-plugins` 改动。

| 顺序 | 原始成员与源证 | 已确认的数据/条件 |
| --- | --- | --- |
| 目录装入 | `VMControls.Data.ToolBoxData.UpdateLoad/LoadData` | 子目录名写 `ToolBoxItemData.Info`，目录全路径写 `DirPath`，随后读取 `ToolItemInfo.xml`。`Info` 不是 XML `<name>` 的别名。 |
| 手势 | `VMControls.WPF.Base.ToolItemGrid.ToolItemGrid_PreviewMouseMove` | 用当前 `DataContext` 作 `VmModuleDecorator.DecorateSource`，封入 `DiagramItem` 列表并发起 `DragDrop.DoDragDrop`。 |
| 放置 | `VMControls.WPF.Base.ItemsPage.OnDrop → GetDropItems → DoDropItems`、`DiagramPage.xaml` 的 `OnDropItems` 绑定 | 携带 decorator 与位置；文件拖放、无 Point、非可见热区等分支先返回。 |
| 实例化 | `VMControls.WPF.DataForUI.DecoratorBase.DropItemsCommandExecute → ToolBoxItemData.AddNewItem` | 先检查运行态；非自定义 Group 的普通工具按 `Info` 取 `{Info}Cs.dll`，可加载 `Librarys.Config` 和 `{Info}Control.dll`；选第一个实现 `IVmModule` 的类型，用父模块集参数构造。SDK/构造异常另有捕获。 |
| SDK 请求 | `VM.Core.VmModule..ctor(object,string) → CreateToModuleSet` | 父为 `VmProcedure`、`IMVSGroup`、`VmSolution` 时分别设置节点类型/父 ID；将 `typeName ?? GetType().Name` 写入 `IMVS_SYNC_ADD_ONE_MODULE_INFO.strModuleDllName`，等待值 5000，调用 `SyncAddOneModule`，返回 `IMVS_MODULE_INFO` 再 `Init`/加到父集合。随后 `AllModules.AddOrUpdate(ID, this)` 和 `InvokeModuleAddedEvent`。原实例数值 `ID` 来自 SDK 返回结构，不可由目录推断。 |

**标定板三种名字已用 CIL 消歧。** 安装目录和 DLL 为 `IMVSCalibBoardCalibModu`，`ToolItemInfo.xml` 的 `<name>` 却是 `CalibBoardCalib`。REA 对安装版 `IMVSCalibBoardCalibModuCs.dll`（SHA-256 `FF1408B85625819490EFEB51E9E6D2714C7DB003E5329AA65CC6B55A8CE8BA58`，MVID `41886863-165b-46de-83f0-ab60c218d812`，evidence `ev_7eccd132b38875c6a84e8c7578581fa97cdf759ee36a41206cb5f045cc250166`）确认类型 `IMVSCalibBoardCalibModuCs.IMVSCalibBoardCalibModuTool` 的 `object` 构造函数 token `0x06000002`。原 CIL `IL_0002 ldstr "IMVSCalibBoardCalibModu" → IL_0007 call VmModule..ctor(object,string)`，而不是将 XML `<name>` 送入基类；原 CIL SHA-256 `96F740EFEE5C7AA9F24BF7A3BE8FEBC573C156C862CFEC61DFD9CFB43F0A4D84`，完整可复读 IL 导出 `analysis/joint-audit/pass13-cal-create/board-cs-ctor.il`，SHA-256 `284757E6D679097FD369AD0B1998AB13B909CD000BB9D309CAF01F688C2E45F4`。原 `ToolItemInfo.xml` SHA-256 `F8ECFC757CA9AF9EAC94474BE24B406D34D427FAD42D5D50F7CB273214CF49E3`。因此该异名**不是**“拖放时 SDK 请求 CalibBoardCalib”的证据；按已证普通构造链，标定板请求的是 `IMVSCalibBoardCalibModu`。若存在 `customInstanceFullName` 或代理路径，须另核该实际分支，不能外推。

原反编译文本 SHA-256：`ToolBoxData.cs` `C1B11D2C3BEBCE66A11769EB0E209455CB1E5D0C4EFA7C4A6DAD5992EB9CF45B`；`ToolBoxItemData.cs` `378A1319599C725EAFB5EEAE0D105AEDB58C8CE0B77F131BC9B480D15ABBC409`；`ToolItemGrid.cs` `304BE361EFFDD7C42D3FB34BF4EFE65967D042F73DC44924054ECE8712D940D7`；`ItemsPage.cs` `BC3A7A487A75BE202162BBE4BA6D6202B9B0E6EE9B618A9DB9373A299091491B`；`DecoratorBase.cs` `25C71C878E2EA21678DB2FBF1E69FEEF0D2892F9356CAE443F747F79D0A83299`；`VmModule.cs` `53E2F0F9032C4631B59CF0EB2A00E06D50469D94706E7E370E9CC7E1059C72CE`。这些是离线源码/CIL，可复核调用边；尚未观察原安装版十项逐一拖入后的 SDK 请求结构、动态 ID、失败/撤销事件顺序。

**与当前实现的关系：** 本地当前用 `tool-catalog.json` 的 `Id/ModuleId`、registry factory 和 `GraphEditorDocument.AddNode` 建节点；第六轮真实 WPF 宿主十项均创建成功。但目前 N图像与相机映射的 `ModuleId` 仍仅在已有未提交工作区映射，不能称 `HEAD` 的十项入口已交付。原 `SyncAddOneModule` 与本地 AddNode 是两套机制；本报告补的是原创建链的代码级依据，不强求相同数值 ID，也不将原厂动态实例缺失错误算成工具算法缺失。
