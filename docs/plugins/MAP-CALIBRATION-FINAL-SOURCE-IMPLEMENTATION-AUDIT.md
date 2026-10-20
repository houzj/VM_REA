> 2026-10-20 最终自审提交：根线已接原typed tree初始化结果运输，真实MainWindow Host26与Path30实际通过；450个src快照0漂移。全工具源码→实施→验证矩阵已复核，交根线最终审计，未自行提高工具计数。原Save80100203与忙门失败日志保留，缓存所有权产品修复及真实宿主等待测试修正分列；最新manifest SHA cc86e1612d925bac0e8b1d9f169d2885714fd5aae74e51e96bf2af23dd8425ac，完整结果见MAP-CALIBRATION-PATH-HOST-FINAL-VALIDATION。下文未应用/未运行/仍缺枚举的段落均是历史阶段，不能覆盖本段。人工验收单列，不阻挡源码+实施自审完成标准。
> 2026-10-20 当前：base路径候选已接，生产Path30和真实Host21已运行通过；Host首80100203揭示伪.images owner键被Retain清理，已根线批准ScopedState(File+Images)修复并复跑成功，旧失败保留。最终审计又发现实际ImageSource_0/1的GetFrontParamItems初始化未在Map dispatcher处理；真实控件直接service可工作，但此缺口仍必须补。枚举专属候选与真实WPF IMAGE A/B下拉选择新版Host已备、尚待统一编辑窗和运行。因此Map仍不完整，不以旧Host21替代新版断言。最新完整退出矩阵见CURRENT-WHOLE-TOOL-EXIT-MATRIX。
> 当前修订（2026-10-19，路径/真实主壳批待验证）：Typed生产121、InputState48、Region25已通过，原10例typed oracle已核。原Map完整计数因typed接缝曾撤回；现在另发现真实MainWindow注册/工程引用/catalog入口与derived文件路径策略接缝，根线已补产品。待执行tests/MapCalibration.PathChecks和MapCalibration.HostChecks，后者真实主壳两图168、显示、保存重开与释放。旧算法/ROI/显示证据保留，不以独立Graph或旧DLL通过替代本次主壳完整验证；整工具仍开放等待根线审计。
> 2026-10-19 最新重开：根线发现六个Primitive输入仍为scalar As读取，已撤回Map完整计分。旧通过与manifest保留为有限历史证据；typed六口候选已按原caller落Map专属，等待实际TypedInput/Graph/InputState/Region验证及根审。见MAP-CALIBRATION-TYPED-INPUT-SOURCE.md。

# Map 整工具源码→实施退出自审（2026-10-19）

状态：根线已审最终退出矩阵并独立核五项源码及12项artifact哈希，0不一致，批准Map整工具完整。总进度由根线统一维护；人工验收独立记录。

| 活动路径 | 原始证据 | 当前实现及自审结论 | 剩余退出项 |
|---|---|---|---|
| Reset普通按钮 | 原Button_ResetAlgoParams.Click→AlgorithmTabRoot.ResetRunParamBind→VmModule.ResetParam→SetDefaultParamList；安装MapAlgorithm.xml | 已移除整字典Atomic，生产MapConfiguration接公共ToolConfigurationAlgorithmReset，保留确认/顺序写/解绑/结果通知，不自建循环 | ResetChecks PASS27，含取消、五支持ISubInfo、非名单保留、中途拒写后续继续 |
| 默认有效名单 | 原Algorithm.xml真实Elements(ParamItem)，注释不计 | 只有CalibBoardType7、FilterStatus1、FilterStatus2=1、HomoFreedom1、WeightFun3、WeightFactor20六项，按此顺序；InputType/Teach/路径/Refresh/display不在名单 | 原资源已内嵌生产，六项有效默认经Reset27验证，hash见manifest |
| 实际ISubInfo | ValueBase实现ISubInfo→Integer/Boolean；String直接实现；OpenFileForCalibDialog直接实现；AlgorithmTabRoot按顶层或ISubInfoCollection子项IsSubA清除 | Map声明九个该类型节点：TeachEnable、TeachFlag、CalibPathName、RefreshFileEnable、WeightFactor及四IsAbandon整数GrayContrast/SubPixelWindowSize/GrayContrast2/SubPixelWindowSize2。产品支持执行参数为前五个；四废弃节点不被混入有效默认名单，无原厂活跃绑定证据。公共helper枚举对应类型并仅清开启模式；不按SubscribeVisible猜IsSubA | 五支持节点解绑经Reset27验证；废弃节点保持声明，不编造执行能力/默认 |
| Image双输入及等待 | 原Process21690/GetInputImage203d0/ProcessOutput220b0；state-lifecycle六步原回放 | Runtime独立两侧pending/pose，每新图Angle=-999f，半帧只1/0，无新图E0000314，配对后运行，失败0/0并清结果；模式切换不清pending | InputState48既有；Region25已验证配对轮独立合同，未更改原缓存门 |
| 图像识别/ROI | 80e690与6/8的80f9a0/80cfc0、7/9的813ca0及813020，真实CPU630120 | FindSource绕legacy gate；crop→mask→原图XY→SourceGrid→原proposal→codeword→ID，6/8 cell与7/9 missing窗分别实现；prefix不确定区间仅在straddle拒绝 | 生产2046、Board axis12/mask4/full4既有原序/bit对照；Region落后RoiRuntime7361再次通过，不靠8向微核证明真实识别 |
| 文件模式 | SetParam23870/ReadFileMatrix227d0/Process21690/SDK7d4f20原体及运行trace | MapCalibrationFileSession按读取Replacement决定替换，成功才提交路径；刷新首失败提前返回；inverse(target)*source具体方向已原体证明；文件常规仅输出状态 | 既有File/生命周期证据复用；未穷尽外部op6f注册桥不扩大产品支持，不能当所有文件格式声明 |
| Teach/Save | 原Process及WriteXmlData，保存XML字段源、原模块回放 | MapImageFiles以当前pose/TeachPose/pending/trigger判保存；缺省哨兵保留，配置Save与运行Refresh独立，文件.xml严格大小写门 | 核现有ImageFiles/FileChecks；不把Reset当清教导/算法缓存 |
| Detect字段 | image-member ctor1f190→6f80清+a4..b4，ProcessOutput真实地址；新增三轮oracle全图168点/ROI154点+标量订阅仍零 | 生产Solve成功Boxes两项0，已移除由RoiDocument造框，检测ROI保持原样；不是全外部接口总零结论 | Detect补丁已应用；Region25核五数组两项0及空输出ROI通过 |
| Region输入 | 原XML五float ROIBOX和独立rawROI配置无优先级证明；产品RoiRegionValue为独立运输 | 已批准窄合同：原图ImagePixels/单包含矩形或全图；全图配置或相同几何可消费，冲突明确；只配对轮读，不锁存第一帧Region | Runtime已落；RegionChecks生产引用PASS25，含真实图像配对/双侧独立/冲突 |
| 显示 | MapDisplay.xml Image InImage2、三个角点XY/count/status、Detect ROI矩形、CalibOriPoint坐标对象 | LastDisplay已接实际Metadata Graphics/ImageLayers+alias；保留Result/Target/mode缓存门；CalibOriPoint无实测producer不造轴。Detect改零后不会冒充配置ROI框 | DisplayChecks PASS15；这是资源行为核对，不能代替原VM桌面视觉验收 |
| cache释放 | 原SDK SetROI Clone/Destroy、替换先clear、DeInit→ResetMember尾跳；原文件矩阵替换析构；产品scope/Reset | 原原生指针由SDK拥有；产品GrayImage/ROI为托管对象，无原生指针复用。Map.Reset移除自身两缓存字典的node项；scope.State独立节点缓存随scope/node移除。配置Dispose只释放ROI交互/execution，不销毁运行结果 | Reset27已验证配置关闭保留cache/显式Reset移除；真实Platform无回调历史显示规则仍未证，不擅自逐轮clear |

## 证据类别与边界

原Reset源码目录D:/MyWork/REA逆向测试/analysis/decompiled/Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab，VmModule在analysis/decompiled/VM.Core/VM.Core。原MapAlgorithm.xml SHA f4dfc100a8bd320576bf55c631202704d3b72d0dfd4d2adb57ae98e0681d53cc，候选仅增加末尾换行以生成有效patch，不更改XML意义。候选analysis/map-calibration/proposals/map-reset/map-reset-source-list.patch SHA ce338f5053a6952722f4ee664561e327bc8f6a23adefc0ceb617320a904b68e3，git apply --check通过。原件以安装资源为依据，不由Descriptor全字典推导名单。

测试通过只能证明所列域；共享其他CPU后端、原VM无回调历史显示、外部未取API、废弃配置参数不是已经证明全域等价。它们与已识别的正常域必修差异分开；不继续无限扩依赖以推迟整工具退出。

## 已批准落盘并完成统一构建

根线授权后Region+Detect+Reset三个专属patch及原Algorithm六项资源已一次应用，未改共享，立即冻结。身份在analysis/map-calibration/map-final-validation/applied-manifest.json。实际实现/资源已落，自动验证已完成且源码身份未变，见最终manifest。

## 本轮退出审查结果

Region/Detect/Reset已应用并完成Display15、Region25、Reset27、RoiRuntime7361，四build/run均0/0warning/error，源码身份未变，managed已释放。详见MAP-CALIBRATION-FINAL-VALIDATION.md及最终manifest。此表“剩余退出项”历史格已由该实际结果覆盖；根线已审计通过并计完整工具，人工验收独立待办。

## typed实际验证与最新剩余项

TypedInput121/InputState48/Region25已在生产DLL通过，见TYPED-FINAL-VALIDATION；六口接缝已闭合。但CalibPathName明确继承原OpenFileForCalibDialog，当前配置缺Directory gate/模型attempt值保留，整工具保持开放，复用公共policy候选继续修。
