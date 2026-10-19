# 第十七批 B：C03.1/C05.2 换源与旧事件独立复审

只读原源码与当前main消费者，未构建、运行原SDK、修改共享产品或访问tool-plugins。原证据复用VMControls.WPF/VM.Core同版本反编译，不以普通会话模型替代原native生命周期。

## 原结果视图换源与释放链

`analysis/decompiled/VMControls.WPF/VMControls.WPF/ModuleResultView.xaml.cs::ModuleSourceChanged`：首次DP改变用Dispatcher.BeginInvoke，执行前比较捕获newValue与当前ModuleSource，过时请求不应用；后续直接DoSourceChanged。Addition.ModuleSourceChangedOut另行通知，不证明同样异常安全。

`DoSourceChanged`：old非null时先置rebuilding→旧CurrentInfo.Dispose→清flag/CurrentInfo；随后ClearDataStore；new非null时刷新执行计数、新建ModuleDisplayDecorator、Init、设置result/localImage DataContext及ImageAlgorithmTabRoot，再UpdateModuleView。没有try/finally保证Dispose/Init异常仍完成flag复位，不能发明原异常安全。

`VMControls.WPF.DataForUI/ModuleDisplayDecorator.cs::DeInit` 显式解除：模块AsyncModuleResultCallBackArrived；ModuleParams.ParamRootReloadEnd；祖先流程AsyncWorkStatusCallBack；PreModules集合变化；Shape刷新；DynamicObject.MappingChanged；非image/pixelinfoimage显示对象OutputFilter和ValueChanged；displayList集合；IMAGE输入RelatedIOChanged；容器动态Inputs集合。随后Dispatcher清模块视图、清数据与shape、扩展DeInit、清结果/图像列表、错误与选择，Tick解绑/Stop/清timer、renderDataCenter.DeInit。Dispose再清ModuleDisplayInfoList/AlgorithmItems/ModuleParams/DecorateSource。

`VM.Core/VmModule.cs::InitParasticValue` 将代理IsActive=false，借HostModule.Parent/Params/UserData；普通Init最后置IsActive=true。IsActive不是“主界面当前选中节点”开关；不能用它解释所有非活动Tab生命周期。UserData getter检查IsActive/IsOnDispose。InnerDispose先IsOnDispose=true，再解绑变量、内部模块/前后关系、参数绑定、CommonDispose/集合移除/CloseModuleInterface；异常重置IsOnDispose并转VmException。这不是简单dispose显示装饰器。

## main已有真正消费者

main展示是拉取式 `RefreshRight/RefreshToolDisplay`，没有复制原装饰器事件网：刷新先清ModuleResultsGrid、Preview.Source及图像名；ToolDisplay三Presenter.Show(frame/null)清旧image/text/columns/items。只有当前results含节点时向IToolDisplayProvider.LastDisplay取帧。未运行节点/切无provider时清空。此为独立main展示适配，可按其现有边界检验缓存归属，不宣称原事件实现完全等价。

`AttachExecutionScope` 的排队refresh/run闭包捕获owner，并校验ReferenceEquals当前scope/ScopeId/当前procedure，能拒绝旧scope请求。`ReplaceExecutionScope`关闭配置、Cancel旧activeRun、清configurationResults/workflowSessions/results、Dispose旧scope再建新scope；ModuleExecutionScope.Dispose先cancel、lease归零才释放state/service。此避免运行租约尚在时释放同一方案资源，但无法自动阻止await后的UI回写。

配置Owner有(solution,procedure,node)身份及base目录校验，Closed把ConfigurationActive置false；Apply检查活跃/方案/目录/节点。ToolConfigurationModelWindow.Closed负责解除session.CloseRequested并Dispose view/session，未建立第二插件会话框架。

## 本批发现的真实缺口

1. 原阅读版本WorkflowExecution.RunCore在await返回后没有核对发起solution/scope/activeRun owner，先StoreWorkflowCallResults及计数，可能把旧方案会话写入新方案同ID或抛索引错误。只在稍后windowClosed判断不足。已交C线/root串行修复。
2. RunConfiguredTool虽然校验返回时solution/scope/node类型版本，却未校验发起编辑会话Alive。Connect Run在调用前检查Alive，不等于await后仍有效；同方案关闭/重开配置期间旧run可能发布configurationResults及RefreshRight。已即时交根线，不能按scope未变忽略。
3. ToolConfigurationWindowHost.Close仅发Window.Close；Closing可取消且Current仍在。ReplaceExecutionScope随后无条件dispose/newscope。必须核实主换方案busy限制或明确主动失效关闭中的旧会话；不能把Close调用等于已Closed。

以上为代码边界，不是已经重现的原厂缺陷或本次运行结论。本线未自行复跑/修改共享文件。

## 完整关闭建议

C03.1固定“原结果视图切源、缓存与旧事件解绑”：main的pull式替代必须真实测试选择A→B→无source、方案更换/同ID重用、排队旧refresh与旧await返回、显示/结果/错误/计数清理，后续旧事件不能重建旧内容。原弱事件清单与每个本地替代点应保留，不能只测Show(null)就关闭；native过滤器/SDK回调解注册另列未知。

C05.2固定“换方案/配置并发关闭与回调解除”：还需配置Run进行时关闭/重开/方案替换；旧session拒Apply与回调、租约资源实际等待释放；关闭取消分支；配置状态与计数不能污染新owner。当前上述缺口未被实际修复及验证前不可关闭。用户人工验收后置不豁免这些尚缺实施；完成后main支持域可自审计完成，native会话回调/设备线程生命周期仍另列。

本线审计索引 `analysis/full-audit/batch17-20261019/B/review-hashes.json` 绑定本次阅读版本；C/root随后产品变化须更新最终审查，不能把本报告的缺口当作已修复证据。
