# C4 显示字号桥接、参数保存恢复与实际呈现切片

2026-10-19。复用C4原件哈希，新增当前IL与定向复用源码。无产品修改、原厂运行、构建或宿主验收。C仍6/32；根线当前全局31/128由根线维护，本文不更新全局分数。

## 本卡剩余闭合条件

1. 所有畸变Display对象的具体生成/映射/成功失败清理与呈现条件：输入/矫正图、标定点/状态/误差文本、普通文本、ROI、DataRecord、ResultList。单一字号链不能代替全部对象。
2. IsDisplay/颜色/透明度/线宽/文本特征与图像选择的设置、重置、重命名、复制、恢复及保存失败分支；用户数据键与方案文件真正落盘链分开。
3. 多图完整配置命令/恢复及源切换、ROI普通确定性链已有有限切片；未闭合事件/模式保留，WPF内部绑定释放列依赖边界，不无限展开供应商外实现。
4. 原生历史prefix兼容策略由根线汇总用户决定；独立实现、差分、真实宿主和手验按整卡S4–S7另行满足。本片只选ErrorTextFontSize一条直接链。

## 原资源到真实Feature对象

原AlgorithmTab的FloatConfig ErrorTextFontSize：AccessMode=O，MinValue=6、MaxValue=72、IncValue=1，PropertyName=Value；BrigdgeSetter TargetName=BridgeObj，GetDisplayObjectFeatureOperation参数为`Show Evaluation Error Text;FontSize`。原Display.xml该对象Type=text、IsDisplay=true；FontSize Feature Mapping=NULL、Value=10，位置映射CalibPointX/Y，内容映射CalibPointError，状态映射CalibPointStatus。参数名是字号桥接，不是算法的一个RunParam。

FloatConfig继承BridgeItemBase，BindingPathA拼`BridgeObjA.`+PropertyName+`A`，在此为BridgeObjA.ValueA。BridgeObjA首次为空时创建并执行BrigdgeSetter.Operation。GetDisplayObjectFeatureOperation依据AlgorithmTabRoot.Owner模块FullPath+`.[Display]`取ParamRoot，在显示列表Single查对象NameA，再GetFeatureByName("FontSize")；通过SetPropertyValueByNameNotrigger("BridgeObjA",feature)写回桥接引用。对象缺失/重复时Single会抛异常，不是随意选首个；Feature找不到返回null。BridgeItemBase.GetParamUnit/SetParamUnit均返回0，不走算法参数setter。

Apps.XmlUI FloatConfig_sub模板用SliderArrangeFloat，Minimum/Maximum/SliderTickFrequency分别绑定上述范围和步进；AttachBinding.BindingString为`StartValue:BridgeObjA.ValueA`。命名空间明确HikPcUI.Commom/VM.PcUI；AttachBinding.DoChangeColorBinding按冒号分隔，为StartValueProperty创建BindingMode.TwoWay到第二项。因此编辑器与Feature.ValueA双向连接。这里6..72是实际UI控件范围；未把它描述成Feature.ValueA或native SDK通用校验，Slider内部精度行为也不据名称推断。

## 设置→刷新→呈现

Feature.ValueA setter仅Value改变时继续，ByteCount()>512会提示后返回。对FontSize不进入Content/Tips/PositionX/Y的重建映射特殊支路，赋Value，解析数值缓存，curReturnValue=value，父Object.InitVmIOs；随后ClearTempValue、OnValueChanged(PropertyEventArgs{PropertyName=FontSize})、通知ValueA，最后SaveValue。ObjectBase.OnValueChanged捕获订阅方异常并记录；保存发生在事件之后，不是事务回滚。

ModuleDisplayDecorator.Init对非image/pixelinfoimage/PointCloud3DImage对象SetOutputFilter并弱订阅ValueChanged→ParentUI.P_ValueChanged。对本属性事件，ModuleResultView.P_ValueChanged在窗口未close且ownerProcedure存在且不运行时调用UpdateModuleViewResultShow；运行中不保证立即主动刷新，后续结果回调属另一条更新入口。

UpdateModuleViewResultShow在Dispatcher、图像/模块/ModuleParams可用时清旧ShapeList，ActiveUpdateShapes跳过IsDisplay=false及非shape类别，清对象缓存后Object.GetValue→GetValueList/FillValueList→GetShapeValue→SetShapeProperty。text分支以GetValueByType("FontSize",...,typeof(int))写Apps.Data.ShapeDefine.Text.FontSize，位置/内容/状态分别按该Feature映射消费。固定NULL映射的Feature.GetValue返回ValueA，转换走ObjectBase.ConvertValue，int用Convert.ToInt32(obj.ToString())，失败返回默认值；不能说float编辑值总是四舍五入到字号整数。

DrawCurModuleUIResult的普通text落default→DrawShapeHelper.DrawShapes，识别IText→PretreatTexts→IMultiText加入m_CurrentInfo.ShapeList；ModuleResultView/ImageViewBox模板把ShapeList交ShapeItemsPage.ItemsSource。ShapeItemsPage.AddShape识别IMultiText调用TextShape.AddMultiText，最终DrawingContext.DrawText。该消费者使用text.FontSize；<=0或>1000时回退10，按图像显示宽高比可调整字号；若文字带非零Width/Height则计算适配框的字号。字体来自SystemFonts.MessageFontFamily/Style/Weight；坐标乘ControlToImageRadio。故存储的整数特征与最终屏幕字号并非无条件1:1。渲染数量上限、图像尺寸为0、越界位置等门控仍适用，不冒充每次改值必绘制。

## 保存与恢复：实际持久化层级

FeatureBase.SaveValue写键`[ParamRoot]_Show Evaluation Error Text.FontSize.Value`，数据StringUtil.UTF8GetBytesPadZero(Value)，经所属VmModule.UserData。SaveValue捕获全部异常；因此字号可能已改变并刷新但保存失败静默，不能以UI改值当保存成功证据。

普通UserData setter在Parent空或Dispose时直接返回；本键不属于CheckedImages等直接特殊键，普通非dynamic模块先GetUIParamValue、SetUIParamValue，再按差异登记撤销。UserData.SetUIParamValue普通分支调用ModuleManager.SetUIParamValue(ID,key,data)，后者复制字节调用IMVS_SetUIParamValue；空数据走DeleteUIParamValue。这里是原厂UI参数层，不是原生算法二进制参数层，也未证明已经写入.sol磁盘文件。

恢复入口ParamRoot.InitNode加载ModuleDir/ModuleTypeName+Display.xml，设owner，初始化动态对象，ReloadProperty，再InitVmIOs。ReloadProperty从ModuleManager.GetUIParamValueByObject(ID)获取列表，每个Object.UpdateResultShowParam按精确键匹配Feature.Value/Mapping，直接回填字段后Initializer；随后InitVmIOs→Feature.InitVmIOs→Initializer/RefreshParseValue，重建ValueA返回缓存与映射。无匹配用户值时普通对象保留资源默认10，DynamicObject另有模板feature回退，不能混入本普通对象。恢复写字段而非ValueA setter，不重复SaveValue。

## 证据、验证与未知

`analysis/distortion-calibration/c4-source/display-font-evidence.json`记录资源/复用源码哈希、当前程序集身份和关键IL。当前被动工具InspectC4DisplayFont.ps1覆盖原实际类型，原结果managed/display-font-metadata.json；没有Assembly.Load目标。参数桥/Feature/ParamRoot/Object/UserData/ModuleManager/AttachBinding/结果视图/绘制消费者各有当前IL校核，不以旧反编译同名类型替代。源为既有反编译，未声称本轮REA重新反编译所有体。

这条链到原厂UI参数保存/恢复及文字呈现已取得；方案文件磁盘提交、复制/重命名/重置的所有键迁移、非法保存值的更广运行差分及其余Display对象整卡链仍开放。没有完整C4 S1/S2、真实宿主S6或手验S7声明。

