# A03 第三批：缩放字节、历史恢复和流程集合生命周期

2026-10-19；接续 main `6a2cf54` 和 A03-tabs-batch2。仅新增静态调用链证据；不关闭 A03 工作包，不把导出方法数算作运行检查数。未访问或修改 tool-plugins，未运行共享构建、SDK、设备、原厂窗口或原类构造器。

## 取证身份与复验

使用 reverse-engineer-anything 技能的“复用同版本证据、按具体问题补方法”流程。REA 对 WPF 元数据的已知 64MiB 失败见第二批 `ev_dd0a0670e77771079905d007438370d5ca2fd94b3ea25e9acef8513d1a0cea0f`；本批不重复请求。使用补充 ReflectionOnlyLoadFrom 导出器读取原件 CIL、元数据、异常区；不是新 REA Evidence，不执行目标代码。

命令：`& tests/NativeEvidence/A03Zoom/Run.ps1`。最终 exit0：WPF16、Core15、SDK6，共37个定向方法体（包括7个闭包/谓词辅助方法）；未重复导出第二批缩放 getter/setter 或静态构造器。没有原机运行断言。

| 原件 | SHA256 | MVID |
|---|---|---|
| Applications/myLibs/VMControls.WPF.dll | 99ffa401c8784472913ad43813d68a8a5577890c00e149506e3e5664fa61c97d | d144317f-3992-4275-9b0f-13fb6163e07d |
| Applications/myLibs/VM.Core.dll | 936763f6c8a1bf9fcf59f22acebca8feb09b5f3d5701ae77c82bdc81e626190d | 290025bc-e040-4cd5-94dd-cd49b9af9c99 |
| Applications/PublicFile/x64/VM.PlatformSDKCS.dll | 34c0c90b00cb75a701685b96670b6490f61376f62eacc0ab29d240157db401c1 | e7b3a3b0-7083-486a-8ca2-97cecec02ea2 |

安装根 `D:/Program Files/VisionMaster4.4.0/`。原件取证前后哈希相同。结果在 `analysis/full-audit/batch3-20261019/A03/`：三个 `*.cil-methods.json` 含原始 IL SHA/token/异常区，三个 `*.cil-disassembly.txt` 含操作数，`source-sha256.csv` 锚定15份源码和main对照，`output-sha256.csv` 锚定审计输出。`attempts.txt`保留Python启动器不可用、SDK初始路径错误、一次脚本参数错误及修正，未抹去失败。

## A03.03 静态子项：ProceZoomIndex 如何登记并恢复

source-confirmed，非 runtime-observed。第二批原 getter/setter 已确认 key=`ProceZoomIndex`、默认索引9，以及字段先更新、SDK写可能失败。本批补齐下游：

1. `analysis/decompiled/VM.Core/VM.Core/UserData.cs:28–105`。getter 在 Parent 非活动或正在Dispose时返回null；setter只检查Parent不存在/正在Dispose，没有同样的IsActive门槛。普通模块先Get旧字节→Set新字节→RecordSingleData；比较是byte[]引用不同，不是序列内容相等。ProceZoomIndex不享受CheckItemParamReady的UTF8内容比较特殊分支。
2. 同文件动态模块分支：ModifyConfig根先RecordAllUserData再写；DeleteModule/CopyModule根下，只有包含SourceIndex/TargetIndex/Position的key单键登记，其余整包登记，因此ProceZoomIndex走整包。非上述根则普通单键路径；DynamicIO例外与此key无关。不能把所有模型的缩放统一说成单键历史。
3. `RecordSingleData:123–134` 创建UserDataUnRedoOperation，保存ModuleFullName、key、old/new；无predicate。`UserDataUnRedoOperation.cs:9,33–45` IsPart=true；单键IsDataComplete=true；ProceZoomIndex使用默认执行优先级。它没有登记RootOperation；所以单独缩放是否实际进入栈取决于当时root、ForbidBuild及服务Register条件，尚无操作证明，不能直接给main增加“每次缩放一个撤销步”。
4. Undo/Redo在`UserData.cs:178–203`同步Dispatcher.Invoke；闭包按ModuleFullName在当前VmSolution.Instance查找，存在才调用VmModule.SetUserData。缺失模块是静默跳过，不是保存旧对象引用并强行复活。
5. `VmModule.cs:3138–3250`的单键分支先`UserData[property]=(byte[])value`；ProceZoomIndex不匹配TargetIndex/SourceIndex/[ParamRoot]/Position/IsForbidden，最终`OnPropertyChanged(property)`。因此历史恢复还发模型ProceZoomIndex通知，不能简化成SDK写字节。装饰器/绑定最终重读和真实ZoomArea更新仍待受控WPF观测，不能用该通知代替已验证屏幕变化。
6. `RecordAllUserData/CanRegisterUserData`仅在ModifyConfig/DeleteModule/CopyModule根允许整包登记，同模块已有整包时拒绝，OldValue表达式在准入时求值。恢复ALL_USERDATA会删除旧UI参数并重放整包，VmProcedure还跳过IsForbidden。这里与C线共享原服务语义，不设计第二个历史机制。

精确CIL：UserData.set_Item token060005AF SHA `6e97df7c27af6b20d1ae693d96e6a27f4483d5f8061152b871f2ceb51738eb22`；RecordSingleData060005B1 `edc0f3696fa60728dc3c59ec613d700f495e39460ac535ae8a074982034bc11d`；VmModule.SetUserData060002F1 `f1c69afe41be5da3900d1eebd33d4bd916d739f250957bce3990c5b8a5ea3730`。两恢复闭包06000845/06000847调用SetUserData，原IL与反编译一致；完整摘要在JSON。

## A03.02 静态子项：SDK如何消费UI字节

`analysis/decompiled/VM.PlatformSDKCS.Audit/VM.PlatformSDKCS/StringUtil.cs`中UTF8GetBytesPadZero(string)追加一个NUL；UTF8GetString按UTF8解码并TrimEnd NUL。两个原方法和byte[]重载已导CIL。

`ModuleManager.cs:765–849`：SetUIParamValue对非空字节AllocHGlobal/Marshal.Copy→ServerSDKAPI.IMVS_SetUIParamValue(handle,objectID,key,pointer,length,wait)→FreeHGlobal，返回非0抛VmException；null或空字节转DeleteUIParamValue。IsForbidden专用EnableModule分支与ProceZoomIndex无关。原方法无finally覆盖P/Invoke抛异常时的释放，不能声称原实现所有错误均释放。

GetUIParamValueByKey使用wait10000，-536870911返回null；成功后分配托管byte[]并复制，再IMVS_ReleaseDynamicMemory，释放错误仍抛异常。Marshal.Copy异常被吞，可能返回零/部分内容，不能补“损坏值默认9”的原厂规则。

`ServerSDKAPI.cs:983–985,1009–1011,1439–1441`直转NativeMethods；`NativeMethods.cs:317,320,389`声明key为UTF8Marshaler、data为pointer+length。此处闭合托管→原生API参数合同，**未取得native UI容器实现函数体或证明保存重开**，仍交D线。

## A03.01 静态子项：非活动流程与集合移除

原路径 `analysis/decompiled/VMControls.WPF/VMControls.WPF/ProcedureConfigControl.xaml.cs:974–1267`，相关方法与Dispatcher闭包已核对CIL：

- Procedures属性整体替换：解除旧集合handler→ClearProcedures→绑定新handler→AddProcedures。copy状态true时非init添加并清copy状态；否则isInit=true。新装饰器由new VmProcedureDecorator创建，DisplayModuleSet自然初始this；不是按旧流程ID恢复Group显示。
- AddProcedures中首次isInit选择First，后续新增选择Last。特例为单元素且不是IVmProcedure：BeginInvoke添加VmModuleDecorator后返回。普通路径Dispatcher.Invoke；AddModulesAsync使用BeginInvoke。它们不按已有流程对象找回旧装饰器。
- RemoveProcedures按DecorateSource==data引用找到装饰器，分别从DecoratedProcedures/DecoratedModules移除，没有显式Dispose，也不把其他存活流程的DisplayModuleSet重置为根。因此存活非活动Tab保留字段，与删除后重新添加新装饰器是不同生命周期。
- CollectionChanged.Remove仅在原DecoratorSelectedItem非null的大条件中清理选中项与选中Tab；Tab清理不是无条件执行。旧项Contains比较的是模型对象。不要据此设计“删除任意流程必选前一个Tab”的规则。
- Move直接throw NotImplementedException；Replace调用的ReplaceProcedures只有nop/ret（2字节IL）；Reset调用ClearProcedures。不能把源码空分支填成常规排序/替换行为并声称原厂如此。
- ClearProcedures先ResetDisposeStatus把IsDisposeNotify置true再false，随后清两集合。ZoomArea.xaml:103仅在本次审查范围将该DP绑定到SearchDryPopup；其OnDisposeStatusChanged(true)调用DeleteSelf，解除输入/清除按钮和Window事件并清window引用。这不是对每个装饰器或模型Dispose的证明。
- `VmModuleBaseDecorator.cs:537–574`子Group移除路径只从decorateChildren按引用移除；Reset清子集合。此方法不写VmProcedureDecorator.DisplayModuleSet。当前显示Group被删除之后由谁使显示回根，跨事件/模型Dispose处理仍未闭合；不能以这里没写就声称整个产品必悬挂。

精确CIL：Procedures_CollectionChanged060002CC SHA `b9b60eef5679ccc8313fba02ac1164981c8bb9c2160c434e0e29d26f3b551b77`；RemoveProcedures060002C3 `e47abffda5585c1285e1fc78054a9a6b6b11ac6721c6eda7a9d2dab605a74678`；ReplaceProcedures060002CB `587bae728805519c3542d21766295396bd01087b6c47765ae3cadbf679813bbe`。

## main缺口与下一步

main仍以会话canvasViews保存scale/pan，未证明存在模型级ProceZoomIndex独立存储合同；非活动Tab根渲染与原DisplayModuleSet字段保留的差异延续第二批。建议root只在独立格式中显式加入“每显示容器缩放索引”后再实现持久化，并明确独立格式；不把pan平移字段称为原厂ProceZoomIndex，不为缩放擅自创建根历史或每Tab栈。本批不改产品。

已关闭：UI key字节到SDK参数的托管链、单键历史恢复的模型通知链、流程集合Add/Remove/Reset/Replace/Move静态分支。保持开放：真实缩放根归属、非活动Tab原机Undo焦点、显示Group删除后的跨事件恢复、native容器编码与保存重开、实际WPF绑定反馈。

原类初始化隔离继续未满足：VmModuleBaseDecorator.cctor的日志与CurrentPathHelper/ModulePathGenerate读取已由第二批原IL确认。本批未获得CurrentPathHelper与日志完整可控路径证据，不能证明只改appbase就隔离所有读写，因此没有运行原类，也没有用未初始化对象绕过安全门槛。下一受控探针必须先补该两依赖源码，再显式记录自有路径和原件哈希，不能复用他任务配置或窗口。
