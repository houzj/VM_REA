# A4 配置可见性触发器有限切片

2026-10-19。被动读取当前原厂 AlgorithmTab XML，复用 Apps.XmlParser.CapabilityAudit 的托管反编译，并补当前程序集IL与八类型重新反编译对照。未执行原厂、未修改产品；本片不关闭 A4 S1/S2，也不计完整工具，总完成度0/10=0%。

原资源：`D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSCalibBoardCalibModu/IMVSCalibBoardCalibModuAlgorithmTab.xml`。资源与当前程序集身份基线见 `analysis/calibration-board/a4-source/identity-index.json`。复用源码根为 `D:/MyWork/REA逆向测试/analysis/decompiled/Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab`；逐文件 SHA 另存 `analysis/calibration-board/a4-source/visibility-reuse-index.json`。这些是既有反编译，不冒充本轮 REA 新导出。

## 资源实际条件

CalibBoardWeightFun 默认值2、CustomVisible=False；其两个 MultiTrigger 分别要求 CurValue=2、CustomVisible=True，或 CurValue=3、CustomVisible=True，操作均为显示 WeightFactor。WeightFactor 自身默认不可见。不能只实现“权函数2/3就显示系数”，父控件可见条件也是谓词。XML注释中的触发器不作为活动配置。

## 复用方法体的具体执行顺序

- Enumeration.CurValueA 仅在数值变化时写字段、标记 IsSetValue、调用 OnPropertyChanged("CurValueA")；不能将 UI SelectedChanged 命令的参数提交步骤混为同一个 setter。
- NameValueItem.OnPropertyChanged 先发布 PropertyChanged，再检查同名 TriggerCondition 抑制；未抑制时进入 TriggerBusiness。CustomVisibleA 变化还可向 Category/Tab 传播可见性变化。
- TriggerBusiness **先遍历所有匹配属性的触发器执行 ReOnTrigger，再第二遍执行 OnTrigger**，不是边撤销边执行。finally 中恢复 FilterPreRun 计数，必要时 PreExecuteRun。
- Trigger.CheckProperty 比较 Property+"A"；MultiTrigger 对任一 Condition.Property+"A" 匹配即参与这两遍。普通 MultiTrigger 的空 LogicCondition 按 and 比较条件值。跨控件 dotted property 另走名称查找，不能把所有条件当本控件属性。
- Setter.Init 解析目标并捕获 Operation 原始值；目标先在最近 Category.Items 按 Name 查找，失败才查 AlgorithmTabRoot。OperationManager 用当前命名空间加 OperationName 反射构造，并写 Executor。
- VisibleOperation.Operate 写目标 CustomVisibleA=true，HiddenOperation 写 false，并置 Working；ReOperate 恢复各自 OriginalValue、清 Working。已读源码中 isFirstFlag 初始 true，true 分支未将其置 false，不能擅自按“只首次捕获”修正它。Visible 的 init 原值=false，Hidden 的 init 原值=true。

因此 WeightFun 从2/3切到1时，原先 Working 的 VisibleOperation 先恢复 false；新谓词不满足，不再显示系数。即使 WeightFun 数值不变，仅 CustomVisible 变化也会撤销/重算相关 MultiTrigger。此为复用源码推导的顺序，未记录真实 UI 运行。

## 下一断点与边界

本轮已补 `InspectA4Visibility.ps1`，使用选定类型PEReader，不加载目标程序集；`visibility-current-il.json`记录当前 Apps.XmlParser.dll SHA/MVID、上述八类型及Tab/AlgorithmTabRoot方法token/RVA/IL哈希/引用。额外使用ILSpy 11.1.0.9782定向导出八类型到current-visibility，全部与既有源码规范化换行后逐字相同，索引为visibility-current-comparison.json。不是把锚点哈希本身当C#语义证明。

关键当前token：Enumeration.set_CurValueA=06000934；NameValueItem.TriggerBusiness=06000beb；HiddenOperation.Operate=06000b17；VisibleOperation.Operate=06000b88；Tab.OnXmlLoaded=06000f7b；AlgorithmTabRoot.OnXmlLoaded=06000370。Tab.OnXmlLoaded先完整遍历XmlLoadedTask（解析setter目标与原值），再第二遍NotifyTrigger；后者只在NeedInit时触发CurValueA和CustomVisibleA。Root按Tabs顺序调用，不能称所有Tab全局原子初始化。

实际模板证据：Apps.XmlUI/styles/paramusercontroldatatemplate.xaml的Enumeration_Sub以TwoWay绑定CurValueA，SelectionChanged另调用SelectedChangedCommand并传SelectedIndex；枚举值与列表索引不可混用。resourcedictionaries/globalresources.xaml的ParamItemStyle在CustomVisibleA=false时将ContentPresenter.Visibility设Collapsed。这里只定位样式定义，未凭定义存在断言所有控件宿主一定应用该样式，样式消费与当前BAML身份为后续补证。

产品对照：ToolConfigurationState.Trigger已先撤销所有匹配setter再执行满足谓词者，Operate对Visible/Hidden记录false/true恢复值；Target先最近Category再全局查找，与本片普通权函数链相符。其Matches支持更多数值/布尔等价及竖线匹配、Initialize在读参/Initers后统一触发，与原厂全局路径不能未经逐类核对宣称完全一致；本工具当前2/3+True谓词未发现需要补丁的偏差。无新增测试或产品修改。

下一断点为实际样式消费者/资源身份、各具体控件初始化门与现有宿主矩阵，随后配置显示/持久化闭合；不将有限触发链称为整卡完成。

追加消费者定位：既有ParamListView.xaml明确指定ItemContainerStyle=ParamItemStyle、ItemTemplate=ParamItemTemplate，并合并GlobalResources.xaml；后者CollectionViewSource绑定Tab.Collection，ParamItemTemplate创建ParamUserControl。因此既有资源中的普通列表消费者已定位。ParamsTabUserControl对Tab头另用IsVisible绑定覆盖继承样式，不能将普通行CustomVisible直接推广到Tab头。上述旧XAML含旧程序集名称引用，当前BAML同一性尚未补齐；索引visibility-template-reuse-index.json明确该版本边界，暂不声明当前安装所有模板已确认。

## 当前安装BAML身份补齐

根线已通过REA静态检查当前Apps.XmlUI.dll，SHA639d0fb9dfca81e38df690e201b73874ce006d1361b689ddc6d5f41f7b0b04fd、MVIDa00f8b40-7a51-4c6d-8aeb-cad134724f1f，证据ev_0b720b58c29ebc598cf14b4033d1cf52c913afcb9c9d066a53d694437e997b36。嵌入Apps.XmlUI.g.resources长779163、SHA93bb8aeb64c20e9eb12e1e8fb33db4a9367c08b69c13ae2dcd2ca42a6b77f3a1。

随后使用已固定ILSpy11.1.0.9782从这同一原件重新导出项目和BAML资源，退出0；输出xmlui-current-d3519bb09be048d28929b8af52f99ea7，选定7文件的完整哈希索引xmlui-current-selected-index.json。没有执行目标程序集。当前ParamListView确实应用ParamItemStyle及ParamItemTemplate、合并GlobalResources；当前GlobalResources第78行以CustomVisibleA=false折叠普通行；当前Enumeration_Sub的CurValueA双向绑定与SelectionChangedCommand传SelectedIndex仍是两条；当前ParamsTabUserControl以IsVisible控制Tab头。由此关闭上述普通消费者链的“仅旧资源版本”缺口；不外推全部模板或像素布局均相同，不单独计完整工具。
