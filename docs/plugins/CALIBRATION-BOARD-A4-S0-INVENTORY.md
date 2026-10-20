# A4 配置、ROI与显示：整卡S0身份及缺体清单

2026-10-19。此卡覆盖原配置入口/模板/binding/trigger/ROI事件/显示对象生命周期及现有宿主对照。S0仅建立当前身份、可复用项、缺体和产品位置；未将旧宿主通过或A3原生切片计为A4整卡完成。

`ExportA4Inventory.ps1`记录模块目录11原件、9个实际共享程序集，共20份SHA/大小；7个产品文件和6个跟踪资源分别记录。机器清单为`analysis/calibration-board/a4-source/identity-index.json`。复用共享C4已列的反编译文件，但本工具AlgorithmTab/Display按其自己的原件处理，不能直接套畸变布局或控件节点。

REA当前安装模块身份已获取：`IMVSCalibBoardCalibModuCs.dll` SHA256 `ff1408b85625819490efeb51e9e6d2714c7db003e5329aa65cc6b55a8ce8ba58`，MVID `41886863-165b-46de-83f0-ab60c218d812`。身份证据`ev_80ccadddacca09c728b4f900dafb275b2664d00c7002ca2a8be96dd1d417dd1e`；成员证据`ev_7eccd132b38875c6a84e8c7578581fa97cdf759ee36a41206cb5f045cc250166`记录11类型/83方法的token/RVA/IL哈希及锚点，索引另存current-managed-index.json。成员报告coverage为partial，CIL锚点不是完整C#源码恢复，未宣称全部分支已反编译。

| A4子项 | 实际原件/复用入口 | 当前产品位置 | 必需后续 |
|---|---|---|---|
| 配置入口、原始参数 | 当前模块Cs的Tool/CalibBoardCalibParam/CalibBoardCalibResult；原AlgorithmTab/Algorithm.xml；ProcedureConfigControl/ModuleParam共享源 | CalibrationBoardConfiguration/Metadata及ToolConfigurationProvider/WindowHost | 当前模块具体方法体；原资源逐节点初始化、读写、序列化和异常消费者 |
| 模板、绑定、触发 | Apps.XmlParser/XmlUI/Style及VMControls.WPF；共享C4同版本关闭/ROI链可复用 | ToolConfiguration model/adapter/operations及公共标准控件 | 当前板资源的触发操作与类型链，不以同名操作接线 |
| ROI事件与原生边界 | 原RoiType=Single/Box/FullScreenEnable；A3普通空ROI门、规范化坐标、clone及缓存shape销毁 | ToolConfigurationRoiService/Controls、Board Plugin ROI执行 | 订阅字段/绘制协议、非0模式、扩展提供者与实际关闭释放；A3不替代全托管事件链 |
| 显示与生命周期 | 原Display.xml、ModuleDisplayDecorator/ModuleResultView共享源码及当前IL；原模块ProcessOutput | CalibrationBoardDisplay、ToolDisplayPresenter及资源对象 | 属性/样式/特征持久化到实际渲染、更新/关闭消费者和源码对照 |
| 宿主对应与验收 | 已有49生产宿主检查及保持的人工验收记录 | 注册/公共配置窗口/ROI/Group执行链 | 先完成源→产品矩阵；旧检查不证明全部原厂桌面等价，人工验收仍未确认 |

旧`iMVS-6000PlatformSDKCS.PublicAudit`的CalibBoard公开包装仅为历史参考，不能代替当前安装模块的CalibBoardCalibParam/Result。缺体按具体成员检索或经REA恢复；不以产品实现填源码空白。源身份与共享资源已明确，A4 S1–S7仍开放，无产品修改或新增原厂执行。
