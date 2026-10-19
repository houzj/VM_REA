# Group 双栏配置器、图形 ROI 与真实工具接入

本次工作属于 main 框架线；没有修改插件实现或另一工作树。原始证据文件和哈希见 group-native-editors-source-manifest.json。不能将本轮结果称为原生 Group 全部完成。

## 原始源码确认

- DynamicRunParamConfigWin.xaml：1100×680，取消按钮隐藏。DynamicRunParamConfigPanal.xaml：左栏370，标题/筛选各37；右侧数据源150、自定义名称150、类型90、关联150、同步70、操作90。
- ParamTypeList.cs：全部、数值、枚举、开关、单选、ROI、文件、训练模型八类。没有对应编辑元数据的类型不能伪装成已经提供该类工具。
- ParamConfigDataBase.AddGroupCommandExecute / GetNewGroupName：分组0起，新增后选中。AddParam：无类别时创建，新增到选中类别；模块勾选逐项加入，达到32项停止。UpDownParam只在同类别排序。DeleteAllParams与DelGroupCommandExecute要求是/否确认；单参数删除无确认。
- GroupRunParam.cs：初始化选中首个类别。原生没有每个参数下方的类别下拉行；类别重归属需要取消选中后在目标类别重新添加。
- ROIConfigControl.xaml：内容1000×600，左380，右图像区上方40工具栏；RoiSelectControl.xaml和roiselectdatatemplate.xaml：ROI参数默认折叠，选图形后编辑。
- CircleROI.Parent_MouseMove：按下点为圆心，鼠标距离为半径。RectROI：矩形拖绘、移动、角点/边/旋转控制。PolygonROI：单击顶点、右键撤点、双击结束。
- ROIControlBaseStyle：蓝色#0F6AF3轮廓、橙色#F76900锚点；原生删除/复制命令。ImageViewBox：棋盘格、最近邻图像、左键空白平移、放大1.5/缩小0.5、光标锚定滚轮缩放、Init重置视图。
- 原始按钮PNG已复制为构建资源（逐个源/目标路径及哈希在清单中）：矩形/圆/环形/多边形、关联、同步、上下排序及状态图。资源来源在上述XAML中；资源相同不代表整体视觉已等同。

## 已接通的产品行为

- Group运行参数“编辑”打开独立双栏配置器，含类型/文本筛选、模块批量勾选、类别选中/新增/重命名/排序/删除、参数勾选/名称/排序/删除、多关联与同步。运行页显示分组参数，管理控件移入配置器。
- 去除ROI表格适配器，改为图像坐标中的矩形、圆、圆环、多边形图形编辑，支持移动、角点尺寸、矩形旋转、圆/环半径、多边形顶点、撤点、复制/删除、缩放/平移/全屏、数值校验、保存及取消回滚。参数区按原生默认折叠。图形绘制和事件桥接是独立WPF实现，并非原厂控件直接运行。
- ROI预览读取该工具唯一的图像输入绑定或其唯一图像输出；不会从其他节点任取图像。NodeDebugExecutor.ResolveInput只读连接/订阅/流程输入，不执行生产者；缺失、失败、重复或类型不符均拒绝。
- Group内部真实图像源工具通过既有IWpfToolEditorFactory创建专用窗口，宿主提供内部流程的参数应用、独立执行、结果缓存与关闭清理；流程保持在父级。没有以通用参数窗替代图像源专用窗口。
- 实际验证路径：Group内图像源→专用窗口修改InitialSN=17→关闭→Group独立运行→真实BMP宽度1296、节点SNCode=17。此为已集成插件现有能力验证，未合并插件工作线后续提交，也未验证相机硬件。

## 明确未完成的部分

1. 当前main只有图像源专用插件，且它没有ROI参数。ROI图形到文档/多目标/框架执行的验证使用测试模块；**尚未接通真实ROI算法工具**。图像源完整工具窗口入口不等于原生Group的单参数专属窗口。
2. 当前ROI契约只有矩形、圆、圆环、多边形。原生扇形、卡尺、线、点、矩阵圆、3D及屏蔽模式未提供；图形控件的全部边/旋转手柄、归一化坐标、夹取和原生事件状态机尚未完全等价。
3. Fixture缺少变换时明确拒绝，不把其数值当像素编辑。原生多边形捕获恢复、全部焦点/剪贴板行为与独立取消策略仍需差分验证。
4. 窗口外框、主题与阴影没有完成逐像素对照；截图是测试渲染的WPF客户区，不能验证系统/原生非客户区。原生SliderArrangeFloat的弹出滑轨/数值模板尚未完整迁移；当前ROI数值框、普通数值/范围/文件编辑器不是原生全模板恢复。模型训练与真实工具专属复杂编辑器缺少集成。
5. 关联选择仍使用现有来源树，未完成原生DynamicRunParamRelationModulesConfigWin全部布局；输入代理/历史未分组数据为已有独立兼容行为。
6. 没有本轮真实VM对照截图或差分运行记录。生成截图全部来自本项目测试渲染；图像中的BMP是固定样图。WPF检查通过不能证明原生UI/算法完全一致。

## 验证与查阅

实际WPF操作结果、界面截图、生命周期结果位于 analysis/full-audit/group-native-editors.html 及相邻文件。源码范围：GroupRunConfigurationWindow、GroupRoiEditor、GroupInputRunControls、GroupRunCategories、ToolEditorHost、NodeDebugExecutor。截图代码仅在tests。

下一步的实质依赖是插件线提供一个经过原始源码审计的ROI工具、明确ROI类型/坐标/图像来源及专属编辑协议；请求见CONTRACT-REQUESTS.md FCR-007。不能通过新增测试模块宣称已完成这一步。

## 本轮最终验证

- 330项实际WPF检查：双栏操作、类型/文本筛选、选类别后新增、两个真实图像源的批量勾选/32项截止/整模块取消、类别与参数管理、清空是/否及文档结果、图形绘制/移动/半径/顶点/复制/校验与保存、真实图像源InitialSN→SNCode/Width。
- 28项输入解析/独立调试检查；115项核心语义、18项现有图像源插件检查通过。
- 发布：dist/VisionEditor-WpfGroupNativeEditorsFinal，Start-Flow-Editor.cmd已指向该版本。
- group-native-editors-release-lifecycle/exit-results.json：Core/Plugin/WPF/EditorClose均退出码0，0.586/2.044/39.002/5.186秒。仅管理本次测试创建的进程；没有终止其他任务进程。此结果不能推广为所有外部驱动无泄漏保证。
- 实际结果：BMP宽1296，SNCode17；结果JSON与可打开的本机示例位于analysis/full-audit/group-native-editors-real-tool-results.json和group-native-editors-example.vmx.json。
- 已查看生成的双栏与ROI客户区图；未做真实VM画面/运行差分。24份原始源码/资源定义文件哈希、21份复制PNG的源/目标/哈希已保存。截图辅助仍全部在tests。
- NU1900：缓存的NuGet漏洞源不可达警告；没有代码编译错误。

## 2026-10-19 F-15 运行配置候选树/撤销作用域源码核对

本批只有源级差异登记，没有改运行代码。反编译调用链为：`DynamicRunParamConfigControl.xaml.cs::EditRunParam` 对 `GroupRunParamConfigData` 调用 `AsyncLoadParams()` 后打开对话框；`GroupRunParamConfigData::AsyncLoadParams` 异步创建 `RunParamPickData`，并用 `ExpendHolder`/`EnumEntrysA` 生成候选；`RunParamPickDataExtern.cs::GetInsideRunParamTree` 逐项应用资格谓词；原生参数类型类别来自 `DynamicRunParamConfigControl.Common/ParamTypeList.cs`。具体谓词及逐字段合同记在 [FCR-021](CONTRACT-REQUESTS.md#fcr-021-group运行参数候选资格元数据插件框架请求未实现)。

本地 `src/VisionEditor/GroupRunConfigurationWindow.cs::BuildTree` 目前枚举节点描述符中的全部参数和输入，没有原生可见性、Group 可运行标志、原生类型名、访问模式或 `DisplayForGroupType`。因此界面虽然有类型/文本筛选和候选勾选，候选集合仍不能声称等价 VM；把缺失参数能力在 Group 主线推导出来会违反插件框架所有权。插件资格元数据尚未接入，也没有用真实插件逐项做 VM 同状态差分。此处没有安全 Group-only 代码修复。

`GroupRunParam.cs::ConfigCommandExecute` 在模态编辑委托调用前/后设置 `IUnRedoService.SetForbidRecord(true/false)`；调用后重新读取两个集合、执行 `GetParamUnit` 并复原各类别 `IsExpanded`。当前独立 WPF 每次配置变更均经 `GroupRunConfigurationWindow.Change` 的保存委托写回文档，撤销颗粒度/窗口关闭行为没有与 VM 同状态验证。反编译代码本身也没有在该方法中显示 `try/finally`，所以不能擅自说原生在异常后必然释放；重建若提供异常安全作用域应标注为独立安全处理。通用 undo 服务不是本 Group 专属文件，本线不修改它，见 [FCR-022](CONTRACT-REQUESTS.md#fcr-022-group-配置对话框期间撤销记录抑制框架-undo-seam-请求未实现)。

验证层级：使用一次性临时 `Directory.Build.props` 将每个项目的 obj/bin 隔离到本 worktree 的 `analysis/full-audit/build-mf04/{obj,bin}/<ProjectName>`，先通过 `dotnet msbuild ... -getProperty` 核实路径，再运行 `dotnet run --project tests/VisionEditor.GroupCategoryExpansionChecks/VisionEditor.GroupCategoryExpansionChecks.csproj -p:BuildInParallel=false -p:UseSharedCompilation=false`。构建完成并输出 nullable warning `MainWindow.xaml.cs(83,224)` 后，离屏 WPF 宿主实际执行20项断言：Group配置页开关、类别展开/折叠和改名写回、类别新增/数量限制、失败保存后重试、代理选择与数量限制、保存重开及真实Group输出均通过；末行 `PASS 20 Group category expansion checks`。这确认当前WPF交互可运行，但该专项没有点选原生候选资格过滤样例、没有测Undo历史，也没有输出截图，且最终工具轮询未保留数值退出码，因此不把它记成FCR-021/022验收通过。窗口被测试固定在 `Left=-20000` 的离屏位置，属于真实WPF控件操作而非用户桌面手工验收。已清除仅本轮创建的临时 props 与隔离build目录；构建产物没有保留。

## 2026-10-19 F-15 模态确认/关闭语义复核

原生调用链现在可确认没有配置对话框取消/回滚：`DynamicRunParamConfigWin.xaml` 把 Cancel 隐藏；其 `OnClosing` 在允许关闭时直接设 `DialogResult=true`，加载进度窗口尚未关闭时通过 `_isCloseDialog=false` 暂时拒绝关闭；公共 `ConfirmBaseWindow.CancelButton_Click` 虽返回 false，但该按钮对此窗口不可见。`GroupRunParam.ConfigCommandExecute` 调用 `method.Invoke` 后不读取返回值，无条件重新加载 `Collection` 和 `GroupCollection`、刷新关联参数单位并恢复展开状态。Group 配置命令的类别/参数操作直接更新 owner SDK/user data/集合，因而“确定”和普通窗口关闭都保留即时变更；`SetForbidRecord` 周围没有源码级 finally，异常后是否恢复未知。方法和字段完整列表见 [FCR-022](CONTRACT-REQUESTS.md#fcr-022-group-配置对话框期间撤销记录抑制框架-undo-seam-请求未实现)。

本地 WPF 也没有 Cancel 按钮，`GroupRunConfigurationWindow.Change` 通过保存委托逐项写回。扩展 `VisionEditor.GroupCategoryExpansionChecks` 增加实际控件操作：检查无 Cancel；修改配置后触发真实“确定”按钮，断言窗口关闭、重新打开仍保留参数；再触发 `Window.Close()` 关闭路径，断言改名配置仍保留，保存重开后实际 Group 输出7。离屏 WPF runner 共24项通过，命令 stdout 明确 `EXIT_CODE=0`。首次新增 Close 断言误期待已删除的代理仍存在而失败；已纠正测试顺序/预期后完整重跑通过，该失败不代表产品行为。

截图 `analysis/full-audit/group-run-config-f15-confirm-close.png` 已目检：显示的是实际 Group 双栏 WPF 客户区，测试窗口位于离屏坐标并通过 `RenderTargetBitmap` 渲染；它不是交互桌面抓屏，不能证明系统非客户区，也没有与真实 VM 同状态图像对比。该轮构建仅 nullable warning `MainWindow.xaml.cs(83,224)`；已删除临时根级 props 和 `analysis/full-audit/build-mf04` 隔离输出。该验证关闭 F-15 中“WPF 点击确定/普通窗口关闭后即时配置数据保留”的本地操作点，但不关闭候选资格 FCR-021、通用 undo 历史抑制 FCR-022、真实桌面验收或原生像素差分。

## 2026-10-19 FCR-022 undo 服务加载边界与本地历史粒度

**VM 服务证据边界。** Group 运行参数入口 `analysis/decompiled/DynamicObjectConfigControl/Apps.XmlParser.ParamTab/GroupRunParam.cs::ConfigCommandExecute` 在调用模态编辑器前后显式执行 `Ioc.Get<IUnRedoService>()?.SetForbidRecord(true/false)`。另一个独立调用点是 `analysis/decompiled/VM.Core/VM.Core/VmModuleSet.cs::CreateLinkModule`：在 `CreatModuleByInfo`、`PreAddModuleTo` 和记录到 `_preGenerateModules` 期间设置 true，之后设置 false。两个可见调用点都没有 `try/finally`，且 false 是固定值而非读回先前状态；这是调用方源码形态，不能据此判断栈实现或异常后的状态。

实现通过应用配置动态装载：`analysis/decompiled/VisionMaster.Exe.Audit/app.config` 的 `UnRedoServiceImplement` 值为 `myLibs\\VM.UnRedo.dll`；`VisionMaster/App.cs` 从 `AppDomain.CurrentDomain.BaseDirectory` 拼接该值，`Assembly.LoadFrom`，再按 `IUnRedoService` 接口筛选类型并 `Ioc.Set`。提供的反编译树没有该 DLL、接口声明或实现类型源文件（路径检查未找到 DLL，树内 `rg --files` 也无 `VM.UnRedo` 文件）。因此 nested forbid 是否引用计数、是否全局/线程/调用作用域、服务是否维护 undo 栈、是否有异常恢复均为**未知**，不得称 VM 已验证抑制生效。

**当前本地历史边界（静态源确认）。** `SubprogramCallHost.cs::CreateBoundGroupWindow` 为 Group 配置器提供的保存委托逐次调用 `doc.SetWorkflowConfiguration(Body().Id, configuration)`；`GroupRunConfigurationWindow.cs::Change` 在每个配置事件后立即调用此委托。`GraphEditorDocument.cs::SetWorkflowConfiguration` 相等检查后通过 `Edit` 写回；`Edit` 每次压入 `Capture()` 的完整方案快照、清空 redo、提交新文档并增加 `Revision`，`Undo/Restore` 按完整方案快照恢复。由此每次不同的即时配置写入都形成单独历史项；确认或 X 关闭不会合并或回滚。此前 F-15 测试只验确认/X 后配置保留和实际运行结果，没有连续 Undo/Redo 历史边界断言；该粒度虽由代码确定，仍标记为**未完成操作级验收**。

**本地操作验证。** 扩展 `tests/VisionEditor.GroupCategoryExpansionChecks/Program.cs`，在实际 `GroupRunConfigurationWindow` 上将类别名称改为“历史边界”，再勾选 `offset` 创建代理，然后对绑定的 `GraphEditorDocument` 连续执行两次 Undo 和两次 Redo。六项断言实际通过：第一次 Undo 只删除最后添加的代理并保留改名；第二次再恢复改名前状态；Redo 按相同顺序逐项重放。构建输出隔离于本 worktree `analysis/full-audit/build-mf04/{obj,bin}/<ProjectName>`，经 `dotnet msbuild -getProperty` 核验属性指向正确 worktree。首次运行退出码1，仅因新增测试缺少 `using VisionEditor.Core`（CS0246）而未执行断言；补引用后第二次完整执行，明确退出码0，末行 `PASS 30 Group category expansion checks`，只有既有 `MainWindow.xaml.cs(83,224)` nullable warning。测试窗在 `Left=-20000` 离屏位置；新操作是实际 WPF 控件事件与实际 undo/redo 文档操作，不是用户桌面手工操作，也不证明 VM 的 undo 服务语义。`analysis/full-audit/group-run-config-f15-confirm-close.png` 已再次目检为真实 WPF Group 配置器客户区；没有真实 VM 同状态截图。

**归属与结论。** 本地撤销栈位于通用编辑器文档，原生服务位于缺失的通用 `VM.UnRedo.dll`；这不是可在 Group 线复制实现的专属功能，也没有从调用方证据导出可安全等价的局部修复。本轮仅补操作级证据并扩充 [FCR-022](CONTRACT-REQUESTS.md#fcr-022-group-配置对话框期间撤销记录抑制框架-undo-seam-请求未实现)，未改运行代码。下一步原生服务仍需取得 DLL/接口实现或 VM 实际撤销栈证据，随后才能判断它是否合并/抑制窗口期间变更。异常恢复不作猜测。
