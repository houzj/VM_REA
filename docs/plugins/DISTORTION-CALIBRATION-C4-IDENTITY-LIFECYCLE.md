# 畸变 C4：S0 身份、复用及多图恢复首切片

2026-10-19。此文交根线审计 C4 整卡 S0；不把首切片视为整卡 S1/S2 完成，不计宿主 S6 或人工 S7。未改产品、未运行原厂程序。

## 身份与证据范围

机器可复核索引：`analysis/distortion-calibration/c4-source/identity-and-lifecycle.json`。包含原模块目录全部 11 文件、6 个实际框架程序集、10 个复用反编译源文件的绝对路径/大小/SHA256，以及实际调用链 11 个当前程序集方法的 token、RVA、原始 IL SHA256、异常区间和成员/字符串锚点。完整选型元数据在 `managed/selected-metadata.json`（VMControls 560、Apps.XmlParser 330 方法）。生成器为 `tests/DistortionCalibration.NativeProbe/InspectC4ManagedMetadata.ps1`，被动 PEReader，不加载目标程序集。

原模块 `IMVSImageCalibModu.dll` SHA256 为 `0A6BAC886F1AA1CCC41433EF9D35FC1091B3D43354DE5794563454D8BBA03BCB`；托管模块 `IMVSImageCalibModuCs.dll` 为 `62E82AFD9B3727F4BA3EEEB6DEE2D12F01D1EF154106E852B94E81C7D14611EA`。VMControls.WPF 为 `99FFA401C8784472913AD43813D68A8A5577890C00E149506E3E5664FA61C97D`，MVID `d144317f-3992-4275-9b0f-13fb6163e07d`；Apps.XmlParser 为 `78EC7AE1E5ADEFFF29197099C7109B153A3F75D092B7C3E9DC253D8C1B25A8D1`，MVID `d0604401-ab61-48a4-938b-faad16d74e7c`。原目录没有专用 Control.dll；不推导出不存在其他扩展控件。

REA 当前程序集身份已取得；VMControls 全成员请求超过 64 MiB 表示预算，无方法体返回。该失败及 evidence ID 已存索引；随后仅对选中类型做当前 CIL 校核。已有反编译源属于复用证据，本轮没有声称 REA 重新反编译全部方法或逐 IL 重建所有分支。

## 整卡目标、实际入口与复用/缺体表

| 目标 | 原厂入口/复用项 | 当前产品位置（只读） | 尚需取体/闭合 |
|---|---|---|---|
| 配置入口与多图恢复 | ProcedureConfigControl；ModuleResultView.DoSourceChanged；ModuleDisplayDecorator.Init/RefreshImageList/RecoveryCheckedImages/RecordCheckedImages/DeInit；ImageViewItem | DistortionCalibrationConfiguration.cs；公共 ToolDisplayContracts/Presenter/ImageLayers；VisionEditor/ToolDisplays.cs | UserData 解决方案序列化、复制、重置消费者；双图完整命令与绑定模板 |
| ROI 配置、编辑与生命周期 | 原 AlgorithmTab 的 ROISelecter、SingleBox/FullScreen；Apps.XmlParser.ParamTab.ROISelecter；ROISelecterHolder；原配置通用 ParamTab 路径 | DistortionCalibrationMetadata.cs / Configuration.cs / Plugin.cs；公共 ToolConfigurationRoiControls/Service；VisionUi ROI 编辑器 | ROIDataStore、操作分派、模板/事件/坐标和订阅解绑链；不能以独立矩形实现替代原厂证明 |
| 畸变显示对象与属性 | 原 Display.xml 的 InImage、Post-Correction Image、Calibration Point、Evaluation Error Text、Text、ROI、Data Record、Result List；ObjectBase/FeatureBase；RenderControlBase | DistortionCalibrationPlugin.LastDisplay；资源 DistortionDisplay.xml；公共 ToolDisplayObjectDefinitions/FeatureStore/Presenter | IsDisplay、字体/坐标/特征值持久化到真实渲染控件的完整模板与消费者 |
| 兼容策略及会话边界 | C1 池所有权、C2 排序边界、C3 分派/文件/catch 已有证据；本轮 ModuleResultView 源切换销毁链 | Configuration 会话 Dispose、执行 Changed 订阅；公共宿主会话由根线所有 | 历史 prefix 非确定性策略由根线汇总供用户选；不清零/硬编码宣称等价；C3 外层状态/错误消费者独立待查 |

索引保留原 XML 与跟踪资源各自哈希，资源同名/存在不等于 UI 绑定已闭合。复用既有 C1/C2/C3 报告，不重复原生运行。当前共享产品中 SelectedImageName 已有契约和 Presenter 消费，不能据此断言 CheckedImages 格式、迁移或原厂多图生命周期均已实现。

## 首个有限源码切片：实际装饰器与恢复迁移

关键纠偏：ModuleDisplayDecorator 与 DisplayImageListDecorator 各自继承 DecoratorBase，并非父子类。ModuleResultView.DoSourceChanged（反编译源 439 起；当前方法 token `0x060001af`）先 Dispose 旧 m_CurrentInfo、清数据，再 new **ModuleDisplayDecorator**，设置 DecorateSource/ParentUI，Init 并绑定 resultGrid/localImageGrid.DataContext。不能将另一类同名恢复方法当此路径源码。

实际链为 DoSourceChanged → ModuleDisplayDecorator.Init (`0x06000951`) → Dispatcher 内 RefreshImageList (`0x06000957`) → RecoveryCheckedImages (`0x0600095f`)。Refresh 禁止记录标志包围恢复，catch 吞异常，finally 恢复标志；随后单独 RefreshInputImageList。Dispose (`0x0600094f`) → DeInit (`0x06000950`) → DeInitImageList (`0x06000959`) → DeInitInputImageList (`0x0600095b`)。后两者对启用 callback 的图像 RemoveOutputFilter、移除弱 ValueChanged 订阅、清 callback 标志和集合；DeInit 还处理模块/前驱/参数订阅与计时器、渲染数据中心。此为托管订阅清理，不推导 native buffer free。

恢复源为 VMModule.UserData["CheckedImages"]，读取同时取 IsSolModifyInfo；解码空值以空串处理。首 token 按显示对象 FullName 查主图；后续 token 以 Contains BackgroundImage/CheckedImage 分派，再 Split(':')[1] 查完整对象名。背景图置 IsBackgroundImage/IsCheck；附加图 SetOutputFilter、移除再添加弱 ValueChanged、置 callback，并以 **ParentUI.ExecuteCount** 取图传 AddCheckImage。循环计数在每个后续 token 后加一，`num > 4` 才退出，即至多处理五个后续 token；该上限不能直接当渲染层最大图数。格式异常由外围恢复 catch 吞掉，前面已执行的订阅/状态不因此回滚。

当 FullName 主图未找到且 IsSolModifyInfo 为真，才尝试 DisplayName；成功后以当前 FullName 替换首 token，保留其余原串并回写 CheckedImages。仍无主图时依次选择本模块非 OutputMask、首个 Inputs IMAGE/POINTCLOUD 的 RelateIO 对应项、ImageViewList[0]。Refresh 仅在列表非空时进入恢复。

RecordCheckedImages (`0x06000960`) 在记录开关允许时组装主图 FullName、首背景标记、其余勾选标记；比较旧串后决定写回。特别地，token 数相同、余串相同、旧首项等于 SelectedImage.DisplayName 且 IsSolModifyInfo 为假时抑制该名称转换写入。异常被吞。该分支与上述恢复迁移必须一起保留，不能概括成“每次选择均无条件保存”。

## 验证与边界

被动索引成功解析两个程序集 890 个选中方法；11 个链方法有当前 IL 哈希和调用锚点。没有托管产品构建、没有新增运行样本、没有原厂执行、没有宿主验收。完整 C4 S0 是否计分由根线审计；S1-S7 整卡仍未满足。C3 外层框架消费者、ROI 全链、显示模板和原生非确定性策略仍显式开放。
