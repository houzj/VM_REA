# 标定板配置宿主与公共图像/ROI契约取证

## 2026-10-19 真实主窗口接入候选

`VisionEditor` 生产项目引用并注册 `CalibrationBoardPlugin`；源工具箱 `IMVSCalibBoardCalibModu` 项映射 `vision.calibration-board`。继续复用普通 `ToolConfigurationModelWindow`、外置 `ToolConfigurationRoiService` 和流程作用域，不建立专属图像窗。注册是独立框架适配；原厂普通窗/ROI链依据下文列出的 `ProcedureConfigControl.OpenParamsTabControl`、`ROISelecter` 和真实 XML。

重新读取原厂 `ProcedureConfigControl.xaml.cs` 的具体入口，确认先连接 `ROIPartUI` 再设 `ParamsConfig`，指定Owner并modeless Show；本工具无Control.dll，走普通参数窗。原厂 AlgorithmTab/Display 嵌入副本SHA256分别为 `76c9cd661a359216041b902cce8fae115566726707e749cc42c0bb9722bcbf91` / `01b6aa734e09bb8284f09cd13cdcabd02115e26b01c0ba218a01dbc960f041d6`，与已登记安装资源一致。

真实产品检查取消了测试临时注册，直接消费生产工厂、工具箱映射、实际图像源和执行作用域。`VisionConfiguration.RoiHostChecks` 最终49项通过：三页签和操作诊断、480×400输入/99点结果、单矩形绘制与原子Undo/Redo、删除回全图、锁定拒写、关窗残留回调失效、两个ImageViewConfig独立眼睛与透明度、旋转ROI保存重开、NoMatch的NG ROI与成功恢复，以及owned Group执行和配置图像来源。日志为 `analysis/board-host-acceptance/host-localized.{stdout,stderr}.log`，stderr为空。测试截图仅在该检查项目生成，`analysis/ordinary-roi/board-common-window.png`、`board-result-page.png` 与 `board-production-shell.png` 已目检；它们是独立产品渲染，非原厂桌面图。

静态语言提取入口 `tests/CalibrationBoard.NativeProbe/ExportLocalization.py` 从原厂AlgorithmTab、递归引用Group及`Applications/Lang/{zh-cn,en-us}.xaml`提取各79项，工具专属`Localization.json`叠加公共回退；身份及四项缺失键见 `calibration-board-localization-evidence.json`。缺失键不自造翻译。原厂Display.xml声明两条同名Data Record；本工具显示适配分别消费其Content/Mapping并保留顺序，托管记录视图用换行拼接，两条记录间的原厂精确排版仍未对照。

手验入口为 `Start-CalibrationBoard-Acceptance.cmd`。测试助手创建合成棋盘图，连接真实图像源到生产标定板并运行，再选中节点并打开配置窗；合成样本、反射及渲染辅助均限于tests，不进产品程序集。完整套件、主窗口回归和实际启动状态由本轮HANDOFF记录，不据本节49项推导全部图像域、原厂ROI字节ABI、阈值内部槽位或桌面像素等价。

原厂证据根为 `D:/MyWork/REA逆向测试/analysis/decompiled`；以下不是桌面观测。

## 宿主类型

- `VMControls.WPF/VMControls.WPF/ProcedureConfigControl.xaml.cs:708–788`：GetExternTabWin(vmModule)为空则 `new ParamsTabControlBase()`；先ROIPartUI=partUI，再ParamsConfig；指定Owner、modeless Show；ModuleResultView.Tag关联该配置窗并SetRoiVisible。
- :794–826 GetExternTabWin仅探测对应Control.dll中的TabWinBase派生类。标定板安装目录没有Control.dll，故主程序路径走普通ParamsTabControlBase。不能从输入图像推导出WithRender独立窗口。
- `VMControls.WPF/VMControls.WPF.DataForUI/VmModuleDecorator.cs:242–267`同样普通ParamsTabControlBase +外部HikImageView。
- `VMControls.WPF/VMControls.WPF/ParamsTabControlBase.xaml.cs:34–54`从paramstab数据上下文绑定窗口宽高/最大化可见性。
- SDK还存在 `VMControls.WPF.Release/VmParamsConfigWithRenderControl.xaml.cs` 和通用 `ParamsConfigWithRenderControlBase`，这只证明另有承载路径，不能替代上面的主程序工具入口证据。

## 本工具XML节点

`Resources/AlgorithmTab.xml:36–46` ROISelecter Name=RoiType，RW，CustomVisible=True，FullScreenEnable=True，SelectType=Single，ROISelection=Box。

:686–705 两个ImageViewConfig：Name分别CalibPoint/CalibOrignPoint，AccessMode=O；BrigdgeSetter TargetName=BridgeObj / OperationName=GetDisplayObjectOperation，OperationParams分别 `Display Calibration Point` / `Display Original Calibration Point`。

## 公共模板和对象绑定

`Apps.XmlUI/styles/paramusercontroldatatemplate.xaml:853–855` ROISelecter→RoiSelectControl，ImageWidth/Height绑定CurImageWidth/Height，CanUpDateROI绑定IROISelecter.CanUpdateROI（OneWayToSource）。

:2503–2556 ImageViewConfig→32高LeftRightControl，右100宽2列UniformGrid。眼睛TwoWay绑定BridgeObjA.IsDisplayA；PopupButton HorizontalOffsetVal=-90，显示由PanVisibleA控制。弹层宽260，高120；StrokeThicknessConfigEnable时150。OK/NG ColorPicker直接双向绑定okcolorA/ngcolorA；透明度0..1步0.01；可选线宽1..10步1。不得把这些写入算法标量CurValue。

`Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/ImageViewConfig.cs`仅新增PanVisibleA；`BridgeItemBase.cs` BridgeObjA惰性执行BrigdgeSetter，GetParamUnit/SetParamUnit均return0。

## ROI读写与生命周期

同目录 `ROISelecter.cs`：

- :1315 SetParamUnit仅Access包含w时写；:1325 InnerSetValue写IUserBytesData[ComName]。
- :1353 SetValue按NormalizeReversParse与PreValue比较，变化时SetImageSize→SetParamUnit→SetRoiSeq→PreExecuteRun。不是只在文本框中保留几何值。
- :1462 IGetROI.SetRoiList(imageWidth,imageHeight,roiInputs)：NormalizeReversParse、SetImageSize、字节写入、SetRoiSeq；:1478 GetRoiList读取字节ParseRoiProtocol。
- :2105 全图选择ClearRoi/StopDrawROI/SetValue。
- :2665 DoRoiSelect停止绘制、清其它组/选中；:2693 DoRoiDeleted清当前、SetValue；:2703 UpdateROI仅CustomVisible&&CanUpdateROI时回读并更新已有ROI。
- :2766 Single+FullScreenEnable允许删除；Single禁止复制，不能由通用多ROI编辑器默认决定。
- `ProcedureConfigControl.xaml.cs:829–857`关闭清当前窗/ModuleResultView.Tag/IsOpenParams、关闭ROI可见性，再RemoveEmptyOperation。

## 本项目公共能力请求

Producer：公共工具配置宿主/图像结果视图；consumer：CalibrationBoardPlugin.CreateConfiguration，消费RoiType与两个ImageViewConfig源节点。

载荷：当前订阅InputImage的所有者身份/像素尺寸；单个Box或全图；绘制/选择/删除状态；按源对象名取显示对象与IsDisplay/颜色/透明度/线宽；源几何+参数原子Apply及Undo；提交后的自运行完成结果/图像刷新。保留实际图像坐标，不用裁剪后局部坐标替换。

生命周期：方案/流程/节点身份和配置会话绑定；切节点/删节点/关窗/方案切换后回调失效；关窗停止绘制及取消选中，不停止其他任务或共享设备。当前GraphExecutor已提供procedure-bound ModuleExecutionScope，工具只消费，不另建第二个图像宿主或调试缓存。

兼容：普通紧凑配置窗继续使用外部预览服务。若未来提供SDK WithRender容器，应由公共宿主明确选择；工具不根据有无输入图像自行决定窗口类型。现有纯参数Apply不能替代原子参数+ROI写回。

验收：真实产品中连图像源→标定板，普通配置窗开启/关闭；单Box绘制、拖动、删除回全图；坐标不平移；提交触发本节点运行；两个ImageViewConfig眼睛/样式只改变对应结果对象；跨流程同ID不串ROI/显示；关窗残留回调不可修改Undo恢复的同ID节点。当前尚未完成这些产品操作，独立Checks通过不代表本项闭合。

## 本轮外置 ROI 与模板接线进展（2026-10-18，在制）

生产已接入 ModuleDescriptor.RoiEditor、ToolEditorContext.ConfigurationRois 与普通配置宿主外置图像面；复用现有 GroupRoiEditor，不新建标定板专属窗口。实际图像源→板流程的 23 项检查已通过，随后补强了实际 State 操作诊断和订阅显示断言。检查揭示并修复了仅 Initers 声明的 EnumerationG 路由，以及 DisplayInputBindings 未声明问题；此前只看 View.Diagnostics 会漏过这些操作错误。截图 analysis/ordinary-roi/board-common-window.png 已人工查看，输入源名称显示实际订阅。

源控件 IntegerBettween 两个成员已纳入公共状态及同行编辑，交叉限制真实端点，保留独立操作身份；原厂统一双滑轨弹层尚未复刻，当前两数值复用已有浮动滑条。高亮/全部 DPI、归一化 ROI 字节 ABI、全部形状仍开放；板圆点/亚像素算法清单由专线继续，不以该宿主接线节点宣称整个标定板完成。
