# N 点标定配置架构证据

本轮从共同基线 `0f3a1d4` 审计原厂源码、安装 XML 和资源，未运行设备，也未新增原厂桌面像素对照。可重建索引见 [n-point-ui-architecture-evidence.json](n-point-ui-architecture-evidence.json)，导出器为 `tests/Export-NPointUiArchitectureEvidence.py`。JSON 保存每份证据的完整本地路径、SHA256、字节数及匹配行号；不能把文本搜索结果直接当有效 XML 模型。

## 原厂链与当前差异

原厂 `VM.Core/VmModule.cs:1809` 设置模块专用 AlgorithmTab.xml 路径；`VmModuleExtern.GetParamRoot` 缓存 `ModuleParam.Data` 中的 AlgorithmTabRoot。`ParamsConfigControlBase.xaml.cs:217–249` 加载模块控件程序集、获取同一模型并按语言选择尺寸。`AlgorithmTabRoot` 默认中文宽 420、英文宽 680、高 500；本模块 XML 未覆写这些值。这是源模型逻辑尺寸，不能据此声称操作系统 DPI 或外框像素已一致。

共同基线中的 NPointPlugin `CreateWpfEditor` 直接创建 NPointWindow（本轮迁移前状态）。该窗口手工创建并反复清空重建页签、数值框、订阅按钮和表格，绕开 XML 模型、源模板以及通用操作路由。换字体或边距无法恢复被绕开的模型语义。迁移应使用公共配置 Session，并为原厂自定义叶提供显式工厂，保留同一模型的触发、初始化、子模型和生命周期。

原厂实际有效页签为基本参数、运行参数、结果显示。结果显示不是算法输出字符串：XML 1057 链接 `TextDisplayGroup`。安装 `Module(sp)/Group.xml:2767` 起将文本内容、OK/NG 颜色、字号、透明度桥接到 Show Text 显示对象及其 Feature。`BridgeItemBase.BindingPathA` 为 `BridgeObjA.<PropertyName>A`，GetParamUnit/SetParamUnit 都是空操作；不得把这些值当算法参数保存。StringConfig 还在光标位置插入源树引用，不能以普通 String 替代完整行为。

## 自定义编辑和文件操作

`RotPointTotalNum` 是 `EditButton_Calib : Integer`，模板将 `Integer_Sub` 与 32×32 原厂编辑图标并排；按钮执行 `EditDialogA.ClickCommand`。XML 的嵌套 EditDialog 创建 `IMVSNPointCalibModuControl.NPointList`。OpenDialog 缓存内容实例、设置父模型和 IOwner，再 ShowDialog；并非独立的“标定点”新行。XML 对话框初值为 493×700，但原厂 NPointList Loaded 主动将 Owner.Height 改为 399，因此 493×399 是有源码依据的运行覆盖，不能仅据 XML 700 判定错误。

OpenFileForCalibDialog 允许选择尚不存在的标定文件：CheckFileExists=false、CheckPathExists=true。选择后先 CurValue=null，再写路径，以便同路径也触发；随后验证扩展名及父目录，调用 SetParamUnit，成功通知并 PreExecuteRun。它还延迟创建 string ButtonSelecter 子模型，登记初始化、绑定触发及 `@ComName` 用户模式，隐藏时按删除订阅规则清绑定。普通存在文件浏览器不等价。

SaveFileDialog 的 DisplayType=Command 只是外观复用；点击实际保存路径到 `IUserStringData[Name]`，然后查找 CalibPathName 并 GetParamUnit 刷新。它使用模块级历史目录、本地 SaveFileDialog 和原厂字面后缀校验。不能把已有 OpenFile 服务接上就声明完成保存行为。远程 FTP/目录行为仍需单独标为未验证。

## 页脚与物理坐标条件

原厂 XML 左侧只有 Reset；右侧是 ClearPoint、SaveFile、ExecuteOnce。不存在手写窗口中的连续执行、执行、确定组合。**NeedPopTip=true 不是操作前 Yes/No 确认**：BottomCommand.ClearCalibPoint 直接写 `owner.Params["Clear"]="0"`，成功后显示 Lang_ClearCalibPointSuc，异常显示 Lang_ClearCalibPointErr，均为普通通知。SaveFile 路由查找 SaveCalibPath.DoSaveFile。

HidePhysicInfoOperation 要求 CalibPointGet=0、WorldRotateAngle 无绑定；按点输入时 PhyPoint 无绑定，按坐标输入时 PhyPointX、PhyPointY 都无绑定，才显示物理坐标参数。原厂 GetOriginalValue 固定 true，ReOperate 恢复 true；每次匹配触发先全部回滚再执行，初始化 Setter 也会执行。公共规则实现位于 `src/VisionPlugins.Calibration/NPointPhysicalVisibilityRule.cs`，注册由提供器完成。规则不凭 Caption 解析绑定，缺节点给出诊断。

Category.CustomVisibleA 自身只赋值并通知；不能简单认定隐藏 Category 自动删除所有绑定。数值或文件等支持订阅的具体叶子，在自身 CustomVisibleA setter 内结合父 Category.IsDeleteSubscribeA 与本身 IsDeleteSubscribeA 决定清订阅。该分层必须保留。

## 注释陷阱与验收边界

文本检索能找到 Ransac、DistThreshold、SampleRatio，但这些声明处于 XML 注释内；ElementTree 解析后 WeightFun 只有 Least Squares、Huber、Tukey 三项。**不要据 rg 结果把描述符上限从 3 扩到 4，也不要增添原厂未启用控件。** JSON 中 effectiveWeightFunctions 明确记录有效条目。

已有 `n-point-ui-evidence.json` 是原厂托管控件及 XML 解析器在合成模型下的受控运行，不是完整原生模块，也不是实际 VM 桌面。此轮架构结论为原厂静态证据与独立实现审计；后续必须通过真实产品提供器打开节点、运行实际输入、验证文件和显示桥、再由用户手工验收。自动测试通过不代表完整 VM 等价。

## 显示坐标订阅的托管投影

后续原厂源码追踪确认 GetSelectedDisplayFilterOperation 会先找模块输入/输出（含 `%name%` 结果别名），再回退 Show Text 对应 Feature：Mapping=NULL 时读 Feature.Value，否则读映射 IO 的 ModulePathName。SetResultIOToModuleOperation 则创建/复用隐藏的 IsResultShow 动态输出，设置 RelateIO、RelateIndex 和 Value，最后设置 Show Text Feature.Mapping 并 InitVmIOs。常量和索引也由该路径解释。这与算法输入订阅是不同通道。

新增公共 `ToolConfigurationDisplaySubscriptions` 是上述语义的**独立托管投影**：用 Display domain、DisplayoutputPositionX/Y 目的身份，保存显式 output/constant DTO 到 DisplayInputBindings 参数；不向工作流输入添加订阅，因此订阅本节点输出不会制造算法自环。枚举仅根流程显式执行祖先及 self 的 scalar int/float 输出；不猜测 Caption，不接受数组、索引、全局、跨流程或远程输入。Bind 后必须读回一致才报告成功。实际缓存无匹配值时将已绑定坐标回落 0 并给诊断，避免旧成功坐标伪装为当前值；未绑定坐标保留显示设置默认值。

`tests/VisionConfiguration.DisplaySubscriptionChecks` 的 27 项检查已通过，覆盖持久化往返、自输出无算法自环、只读应用失败、清除、缺缓存、流程切换、节点删除与不支持边界。窗口关闭后同流程同 ID 的拒写仍由宿主已有 ConfigurationActive 保护；该服务不能凭回调参数独立推断窗口生命周期。原厂 native 动态 IO ABI、索引/全局/远程映射和实际 VM 桌面行为未由这些测试建立。

## 本轮实现与验证状态

生产入口现已迁移到 `IToolConfigurationProvider` 和公共 `ToolConfigurationModelWindow`，工厂兼容入口也返回同一宿主。使用原厂 AlgorithmTab/Group，而非手工重建三页。原厂点编辑、独立物理坐标显隐、保存文件和复位确认均通过服务接入。路径叶已提升为 `VisionPluginContracts.ToolConfigurationCalibrationPathControls`，N 点包装保留兼容。

本轮已完成的独立验证：NPoint.ConfigurationChecks 53项，NPoint.ConfigurationControlChecks 25项，VisionEditor.Core.Checks 122项，实际主窗口 WpfChecks 458项（含拖入 N 点节点并打开公共420×500三页窗口）。这些结果确认独立实现和真实宿主接线，不构成原厂桌面像素验收或完整原生一致结论。后续公共框架变更仍须最终回归。

未闭合：原厂桌面同状态像素对照、原生动态显示对象ABI及 Image Point 路径/RotAxisPoint 的完整交互绘制、所有原生数值极端边界。不得把本次公共配置架构修复登记为整个 N 点工具完全复现。
2026-10-17：用户明确回复‘通过本轮配置架构验收’。验收范围为本轮配置模型/模板/宿主修复；上述整个工具及原生显示/数值边界不因此自动关闭。
