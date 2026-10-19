# A03 第四批：显示容器到缩放绑定及历史通知的下游

2026-10-19；基线 main `8b7b2de`。本切片仅增加原始方法/资源静态证据，不修改产品，不访问 tool-plugins，不执行原装饰器构造器、SDK或原厂窗口，不关闭 A03 工作包。

## 证据身份与复验

依 reverse-engineer-anything 技能复用同版本反编译及已有 REA WPF 元数据 64MiB 限制记录（第三批报告所引 Evidence），不重复同一失败查询。补充工具 `tests/NativeEvidence/A03Binding/ExportBindingCil.cs` 以 ReflectionOnlyLoadFrom 读取元数据/CIL与原资源 GetResourceData，不是新增 REA Evidence，也不执行目标代码。`Run.ps1` 用系统 .NET Framework csc 编译自己的读取器，输出到本批独占目录。

原件 `D:/Program Files/VisionMaster4.4.0/Applications/myLibs/VMControls.WPF.dll`：SHA256 `99ffa401c8784472913ad43813d68a8a5577890c00e149506e3e5664fa61c97d`，MVID `d144317f-3992-4275-9b0f-13fb6163e07d`。取证前后哈希相同。最终 exit0；20 个新方法体，与第二批22方法及第三批WPF16方法按token比较重复数0（coverage.json）。这是静态方法数，运行断言数0。

`analysis/full-audit/batch4-20261019/A03/` 包含完整CIL/操作数/token/异常区与IL字节哈希、原件完整性JSON、资源索引及原始值、source-sha256.csv与output-sha256.csv。首次25方法选择误包含旧批闭包，保留在 attempt1-with-redundant-helper-selection，修正为定向20新方法，不把初次重复方法计为新增。其后定向选择的调整均为静态读取；无目标方法失败或运行失败结论。

## 已确认的原数据流

所有源码以下均相对 `analysis/decompiled/VMControls.WPF/`；XAML是已有同版本反编译文本，本次同时保全原程序集资源值，未重新执行BAML解码。

1. `VMControls.WPF/ProcedureConfigControl.xaml:526,538`：ContentPresenter.Content=`DisplayModuleSet`；DiagramArea.Procedure同样绑定`DisplayModuleSet`，ItemsSource绑定其模型Children。这不是每次切Tab都把当前显示容器强制改为根。其外层ZoomIndex另由ProcedureConfigControl到DiagramArea的TwoWay double绑定输入。
2. `VMControls.WPF.Base/DiagramArea.xaml:51,56,65`：模板Grid.DataContext显式绑定TemplatedParent.Procedure，因此后续ZoomArea.DiagramZoomIndex与StatusBar.ZoomIndex都绑定当前显示装饰器的显式接口路径`(VMInterface:IZoomIndex.PartentZoomIndex)`，Mode=TwoWay。两控件共享模型索引，不是彼此直接绑定。第三批已确认这个接口getter/setter接ProceZoomIndex，原键存于各显示容器模型UserData。
3. DiagramArea.OnZoomIndexPropertyChanged token06000B2B，外层double缩放值通过StatusBar._ZoomTicks.IndexOf寻找离散索引，仅合法且与当前statusBar.ZoomIndex不同时赋值；源码没有邻近值取整规则。OnApplyTemplate 06000B37获取statusBar模板实例。OnProcedureChanged 06000B24主要处理标定引导控件、模式按钮与Dispose；未发现它为普通Group切换另写ProceZoomIndex或重设所有装饰器。
4. `StatusBar.xaml:163,172`：Slider.Value TwoWay接自身ZoomIndex；百分比TextBlock OneWay接ZoomValueE。ZoomIndexChangeCallback 06000CC7只通知ZoomValueE；get_ZoomValueE 06000CC8将合法索引的tick乘100，非法则1；ZoomCallBackClick 06000CCB设置1.0的索引。它们不是SDK存储代码。
5. `ZoomArea.xaml.cs:388–400,462–483,502–515`：DiagramZoomIndexChanged 06000CF1调用已有第二批OnDiagramZoomIndexChange；非滚轮变化以当前ViewArea中心缩放，并清IsWheelRoll。新增OnPreviewMouseWheel 06000D0A先置IsWheelRoll=true，再水平滚动分支，后OnChangeZoom 06000D0B；仅Control修饰才按Delta方向增减索引且有两端门槛，再以鼠标pos调用DoChangeZoom。Shift水平滚动是OnHoriScroll 06000D0C的另一路；不能说所有滚轮均缩放。
6. DoChangeZoom 06000D0D：新tick=t；新宽高分别为旧宽高×当前Scale/t；X/Y按鼠标在原矩形中的相对比例补偿。写viewAreaCopy后赋ViewArea；并非把平移固定清零。set_ViewArea 06000D04有布局未准备门槛：宽/网格/ScrollViewer宽为0只存viewArea返回；否则由viewportWidth/viewAreaCopy.Width求Scale，写ScaleTransform双轴，再修Margin及滚动偏移，最终回读。OnScrollViewerScrollChanged 06000D0F按实际offset/Scale和viewport/Scale回写viewArea及viewAreaCopy；不能以静态赋索引等同已观察屏幕变化。

## 历史恢复通知存在未闭合的别名环节

第三批已证实UserData_Undo→VmModule.SetUserData→OnPropertyChanged("ProceZoomIndex")。本批新增的原CIL进一步限定下游：

| 原成员 | token | 已确认行为 |
|---|---|---|
| DecoratorBase.set_DecorateSource | 06000906 | 换模型时解除旧模型/订阅新模型，PropertyChangedEventManager属性名为空，接收全部通知；setter本身未通知DecorateSource变更 |
| DecoratorBase.DecorateSourcePropertyChanged | 06000907 | 将e.PropertyName原名传给装饰器OnPropertyChanged |
| VmModuleSetDecorator.DecorateSourcePropertyChanged | 060008E7 | 先base转发，仅Name额外通知ShowName |
| VmModuleSetDecorator.OnPropertyChanged | 060008EF | 先base，只有UnReadyModuleName/IsReady额外通知WorkState/WorkStateInfo；没有ProceZoomIndex→PartentZoomIndex映射 |

与第二批已确认的**直接ProceZoomIndex setter同时通知ProceZoomIndex和PartentZoomIndex**不同，从模型恢复通知经过以上方法只得到原名。资源绑定读的是显式接口PartentZoomIndex。可确认的是两条静态通知链不完全相同；不能在没有原WPF绑定观测时断言Undo一定不刷新、一定刷新、或靠其他事件一定补齐。DisplayModuleSet切换会带来新的绑定源求值，但这不能证明同一源实例的Undo通知能触发同一路径。禁止据此为main增加未经确认的每Tab撤销栈或伪称原厂缺陷。

新增DisplayModuleSetChanged 06000285只通知IsDisplaySingleModulesetSource；HeadText_MouseDown 060002CA读取对应流程的DisplayModuleSet作为选中子Tab，再调用已有SelectedItemToSelectedSubTabItem。与第三批集合生命周期合起来说明当前显示容器、选中对象、缩放绑定是不同字段/链路，不能一律归并为根流程ID。

## 原资源保全范围

原VMControls.WPF.g.resources定向保全5个值：base/zoomarea.baml、base/diagramarea.baml、base/statusbar.baml、procedureconfigcontrol.baml，以及后缀选择带入的oneprocedureconfigcontrol.baml。resource-index.json记录每个key、ResourceTypeCode.Stream、原始值字节数和SHA256。GetResourceData输出包含.NET资源序列化长度帧，文件明确命名resource-data.bin，**不是可直接装载的纯BAML**；第五项仅保全，未作功能结论。

## 安全边界、实现和下一步

第二批已确认VmModuleBaseDecorator.cctor有日志和CurrentPathHelper/ModulePathGenerate读取；本次未补完这些依赖的可控读写路径审计，因此继续不实例化原类，未声称初始化已隔离。读取器只访问程序集元数据、CIL和资源流；没有在安装目录写文件，也没有初始化原SDK。

本批重构行为：无产品变更；当前main会话scale/pan与原每显示容器ProceZoomIndex/TwoWay绑定差异继续开放。运行验证、真实桌面、native UI容器、保存重开、显示Group删除后的跨事件恢复都未闭合。下一步应先完成CurrentPathHelper/日志初始化静态安全审查，再在受控副本WPF宿主记录同源模型ProceZoomIndex通知、接口绑定目标值、直接setter与Undo的差别；同时区分DisplayModuleSet切换导致重求值。以上证据不足前不为该别名环节补规格。