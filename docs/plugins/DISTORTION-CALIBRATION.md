# 畸变标定：独立工具逆向清单

工具为 `IMVSImageCalibModu`，官方C# XML称“畸变标定 / Distortion Calibration Tool”；算法为 `MVDImageCalibCpp`。本线独占 `src/VisionPlugins.DistortionCalibration`、`tests/DistortionCalibration.*` 和本工具文档，公共契约、ROI及最终宿主注册由根线协调。无设备测试，不启动全局原厂平台。

## 宿主及资源

原厂位置 `D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSImageCalibModu`，11个文件，没有独立Control.dll。`ModuleViewMode=1`；原始AlgorithmTab直接声明基本参数、运行参数、结果显示三页签，64个具名节点（含分类和枚举项），不能将节点数当作参数数。

`D:/MyWork/REA逆向测试/analysis/decompiled/VMControls.WPF/VMControls.WPF/ProcedureConfigControl.xaml.cs:708–826` 明确：GetExternTabWin仅查对应Control程序集中的TabWinBase；没有时使用ParamsTabControlBase，先绑定ROIPartUI再ParamsConfig，设置Owner并modeless Show，结果视图Tag关联及SetRoiVisible。故产品复用公共配置模型，不开发另一套独立窗口。该结论来自源码，不冒充原厂桌面截图。

原始AlgorithmTab及Display已逐字复制到专属Resources，清单/哈希/声明树见 [资源及宿主证据](distortion-calibration-evidence.json)。REA静态证据：程序集 `ev_fe7f3bedb9a34638af20fb61040a109e413b43c20a48ab0f8bb142d0e1eb3e9c`，成员 `ev_c12ec875d79056c9a3bf4937742149f068b57efb9bf7280cdad2f73d1b1dd8bd`。

## 2026-10-18：原厂托管组件 IL 复核

本机原厂 `IMVSImageCalibModuCs.dll`（SHA-256 `62E82AFD9B3727F4BA3EEEB6DEE2D12F01D1EF154106E852B94E81C7D14611EA`）已用 `dotnet-ildasm 0.12.2` 导出到当前工作树 `analysis/distortion-calibration/IMVSImageCalibModuCs.il`（SHA-256 `C6FB49B899FC3B4EC8E113844AD898BC39C8B4A597D5CDD7136C231D44664010`）。这是托管插件层 IL，不是 `IMVSImageCalibModu.dll` 原生 ProcessAlg 的反编译，也不替代受控运行。

IL确认托管 `IMVSImageCalibModuTool.Run()` 只调用 `ServerSDKManager.mModuleManager.SyncSelfRun(module.ID, true)`；参数 `InputImage`/`CorrectCenterPoint` 通过 `InputItem.SetInputValue`，普通字段走各自 `ParamItem` typed setter；`DoSaveFile(path)` 将路径写入 `SaveCalibPath`，并捕获 `VmException` 后返回其 errorCode。`ImageCalibResult` 构造器初值为 status=0、estimate=0、point count=0、空校准点/状态列表、evaluationError=0、CalibPointError=0，并预先构造 ROI 与 PostCorrectionImage 容器。以上只确认托管包装/初始结果形状，不代表运行后的原生清理策略或 UI 展示。

因此 `CalibPathName`/`SaveCalibPath` 是标定结果导出路径与显式保存入口；本工具不是“读入现成 .iccal 并据此矫正输入图”的加载器。运行仍使用 `InputImage` 做标定，SDK结果再按自动写出策略生成 `.iccal`。这个结论与原生模块 SetParam/ProcessAlg/SaveCalibPath 受控运行中的首次自动创建、刷新覆盖和显式保存结果相互印证。仍未由托管 IL 确认的是原生 ProcessAlg 内部错误到结果/输出字段的完整映射。

## 强制运行时间输出契约

原厂 `IMVSImageCalibModu.xml` 的 Output 节点将 `ModuRunTime` 声明为 `float / IsForce=true / IsArray=false`（资源 SHA-256 `05DC2A50457CBF63AD387092A0DFC77D52E0B892C192B1C7830C624B6BC98EAA`）。SDK `ModuleResultManager.SetModuleResult` 无条件把 `IMVS_MODULE_RESULT_INFO_LIST_EX.fModuleTime` 复制到 `ModuRunTime`；原始 `ImvsSdkPFDefine.cs` 对该字段的注释说明计时从“输入数据全部到达”开始，到“所有数据发送出去”结束（源码 SHA-256 `079ECE0C0C94F67F59AF42A8DCD28E92D50BF1B576BBB97DE0E5196A8457AFD4`，结果管理器 SHA-256 `FD3960EE308FB739729AF43589035D628544BB5B26941BE5EB6F3250A41739E7`）。插件描述现将此端口标记为必需，所有受控成功及已处理失败结果均携带有限的毫秒数值。当前数值由 `Runtime.ExecuteAsync` 内的 Stopwatch 生成，只覆盖插件调用区间；它不等同于原厂从输入到齐至输出发送完毕的范围，尚无真实宿主 trace 可供精确实现该范围，故明确作为独立实现。

## 独立清单

| 项目 | 当前证据/实现 | 待完成 |
|---|---|---|
| 工具身份、输入输出与宿主 | 原厂XML、官方C#接口、主程序GetExternTabWin源码已确认 | 图及运行端口逐字段接线 |
| 原始配置和显示资源 | 3页签、ROISelecter、中心点/坐标、3种畸变模式、文件、显示桥已清点 | 公共模型生产Provider、真实交互及模式检查 |
| 模块参数语义 | Init/SetParam/GetParam/ProcessAlg/ProcessOutput已CLI反编译 | 初始化有效默认值、中心缺失、ROI/掩膜、文件路径规则的受控调用 |
| 官方算法运行 | 棋盘/圆点×3模式×直板/桶形×评估开关24个实际像素案例全部Run0 | 更广泛无效输入/ROI/输出图/编码板差分 |
| 文件格式 | SDK导出76字节=19个little-endian float，独立读写72项原文件检查通过 | 模块保存回放及加载边界；未将首值2直接称作版本规范 |
| 透视求解 | 已定位0x67→1e330→op0xf→7f7050→7ecea0→7ed0c0 | 原厂19元素构成、点序、尺度/中心与误差，独立实现及差分 |
| 径向及组合求解 | 24案例提供实际非零各向径向系数，组合矩阵不同于单透视 | 原厂拟合模型/迭代/阈值/float顺序完整追证，禁止填零成功 |
| 评估与矫正后图像 | op0x23→7f7450，op0x11→7f8590；SDK评估开关存在数值重算影响 | 原厂评估点与NG状态、重采样/图像及输出使能实现 |
| 生命周期、持久化、显示 | 必须消费公共ModuleExecutionScope、显示/POINT别名、ROI | 生产执行/导出、清旧结果、跨流程隔离、取消和宿主回归 |
| 闭合 | 未注册、不声称完成 | 根线整合、CMD启动人工验收、中文闭合提交推送 |

## 关键边界

SDK裸实例默认HomoFreedom=2、CalibBoardWeightFun=3、Circularity=50；Algorithm.xml为1/2/50，AlgorithmTab为1/2/60。三层值分别保留，需根据模块Init实际装载顺序确定生产初值，不能凭UI声明或裸SDK其中一层直接推断。

Module.SetParam对CalibPathName有Windows目录检查/创建逻辑，与N图像工具不同；SaveCalibPath经.iccal路径规则后调用真实SDK导出。必须独立复现其行为，不能复制另一个工具的文件处理。

公共需求已告知根线：Single Box+FullScreen ROI；CalibPointStatus数组的逐点OK/NG；按CalibPointX/Y定位的CalibPointError逐点文本；FloatConfig通过GetDisplayObjectFeatureOperation操作图形FontSize（6..72，源默认10）；输入与矫正后图像两个image对象的显示切换。

当前已实现真实原厂资源、独立二进制文件读写、公共配置适配、给定矩阵的图像映射和逆径向单点求解；完整标定拟合尚未闭合，不以文件/映射测试或SDK运行代替完整算法完成。原厂UI桌面及设备行为未验证。CLI反编译与REA元数据证据分别注明，未冒充同一提供器。

## 2026-10-18：配置与图像映射进展

生产元数据和公共配置Provider已建立，实际WPF宿主12项模式/回滚检查通过，截图在当前analysis/distortion-calibration/configuration。当前仍有公共ROISelecter、IntegerBettween和显示字体桥需求，Provider通过不代表所有控件可操作。

有效默认值已由模块原生ResetDefaultParam链确定：构造vftable+0x60→180017040→VM_M_GetDefaultConfigByFile逐项SetParam；18001e9d0将模块DLL路径替换为Algorithm.xml，因此新模块采用Algorithm.xml的1/2/50，而不是裸SDK的2/3/50或UI的1/2/60。

图像映射按原厂18083cef0和180858640恢复：以(x+1,y+1)生成源坐标，透视与径向按模型分派，坐标最终转float；分数乘128向零截断，四个整数权重，累加后右移14位。原厂边界按线性基址w*h-w-1检查，不能改成常规四邻域边界。常量1816437b0原始字节确认double epsilon=2^-52，18164383c确认float128。优化路径18085a300→1808a4400采用另一简化径向布局，本轮有效19float案例走通用映射；不外推所有历史载荷。

12组原厂Mono8输出（棋盘/圆点×透视/径向/组合×直板/桶形）共2,304,000像素与独立DistortionCalibrationWarp逐位相同，最大误差0。证据像素压缩存于distortion-calibration-pixel-evidence.json，正式断言在DistortionCalibration.PixelChecks；此仅验证给定原厂矩阵的图像消费，不证明独立标定拟合、RGB或原厂桌面一致。

当前文件读写、配置、映射和径向参数拟合已独立实现；固定点矩阵拟合差分、评估、完整生产插件与生命周期尚未闭合。原生源码已追到14参数LM（180853820→180854e90）和单点逆径向求解（180857a60），不以SDK代跑或零系数替代独立实现。

RGB24非等通道实际探针12/12均被官方SDK以0x10000007拒绝（MVD_ALG_STS_ERR_IMG_FORMAT），见distortion-calibration-rgb-evidence.json。模块180020820直接调用SDK SetInputImage，未见模块内Mono转换；通用宿主输入预处理尚需独立确认。核心warp存在RGB分支不能作为校准Run支持RGB的依据。

## 逆径向单点核验证

原生180857a60以图像点为初值调用180854e90的2变量分支，系数布局为kx/ky/aspect/k2，中心另传。已在独立受控进程调用同一PE的该原核（core SHA和探针存于distortion-calibration-inverse-radial-evidence.json），8组实际导出径向载荷×25点共200点。

DistortionCalibrationInverseRadial按源码解析Jacobian和Nielsen阻尼迭代恢复，200点/400个坐标float字段全部位相同。LM原常量块181643ec0（224字节）的掩码与常量已提取：H8初始化全部active，初阶段只开kx/ky；第二阶段aspect和中心active但k2固定；第三阶段k2也active。各阶段仅在RMS小于前阶段0.8999999761581421时采纳。共同LM相对step阈值1e-10、微小量1e-30、初始阻尼0.001*最大对角、最多200次。14变量拟合的后续实现与独立证据见下节；2变量精确结果不外推其精度。


## 2026-10-18：十四参数拟合与固定核输入证据

DistortionCalibrationRadialFit已独立恢复H8+kx/ky/aspect/k2/centerX/centerY共14变量、解析Jacobian、阻尼迭代、三个active掩码与阶段10%改进采纳规则。8组合成点组由原厂7ff3c0提供初始H、原厂853820提供最终径向输出，覆盖偏心中心、aspect非1和非零k2。正式DistortionCalibration.RadialChecks通过48项：47/48个最终float字段同位；另一近零k2差2.49e-16，整个480×400范围采样映射差最大2.812e-9像素（门槛1e-6）。这里隔离验证径向LM，不包括公共初始拟合和图像检测。

补充校正：旧direct853820诊断曾将nativeFactor填20，而模块UI WeightFactor20经/10后实际传2。正确nativeFactor2回放与SDK包括近零圆点系数均相同；旧结果保留为factor20-diagnostic并明确参数不同，不能据此声称原厂近零输出不稳定。

自有进程单步断点已记录7ff3c0共84次真实输入/返回：82返回1，2返回0且矩阵清零（Weight1边界）。完整float点对与9元素矩阵保存在distortion-calibration-fixed-trace-evidence.json，导出脚本验证唯一返回及字节数。按原厂 `1807fd4e0` 将H33归一化恢复为先求 `1f/H33` 再逐项float乘后，共享固定核直回放82例现为697/738字段同位，最大差0.0071191043；此前为347/738。analysis隔离仅将IRLS上限20及末尾倒数相乘后534/738字段同位，最大0.007117897尚在（在网格→像素矩阵中）。末尾源码为1f/raw33后逐项float乘，不能换成逐项除；剩余5次调用的41字段关联已证实的排序workspace旧值，生命周期仍未确定。

DistortionCalibrationSolver已建立三种模式的文件矩阵组装，给定SDK检测点12例中5例19/19位，另若干仅舍入误差；有两例约1.07e-4，仍列未闭合，不扩大门槛。当前FitChecks是隔离诊断，暂用测试层反射既有private核；生产通过显式delegate等待统一固定核API，不反射、不调用原厂SDK代跑。

原厂额外HIK运行：同168点HIK-I fixture以board7/9×三模式×评估开关12例均Run0；以6/8解释该I板12例拒0x10000107，不能外推其他合法板。7ed0c0入口实际opts确认checker/circle/HIK7/HIK9的GridPitch均为1，和映射工具按编码10/1000的物理单位路径不同。

模块ProcessOutput的额外源码语义已核实：评估开启时始终发布PostCorrectionImage；OutputPostCorrectionEnable仅决定发布原图检测点/误差，还是矫正图重新检测后的点/误差。CalibPointError先roundf(raw*100)/100，然后以rounded > PointErrorThreshold判NG，等于阈值仍OK。PE常量18002ded0原始00-00-C8-42为float100，18002cba8为double1。DistortionCalibrationPointEvaluation已按这一独立模块投影实现，不能直接把SDK原始点误差无舍入输出。

模块Process20550明确先要求输入PixelFormat==1（Mono8），否则返回e0000503，不进入SDK。裸SDK的RGB错误10000007因此不能直接当模块运行错误码。

纠正中心生命周期也来自该Process：裸SDK中心为(0,0)，每次读X再读Y；X缺失允许用0，只有Y成功才SetPerspectiveCenterPoint，Y缺失则保留上次中心（包括已有X），读取非missing错误为e0000314。SDK wrapper2a70使用中心时先转int再转float。DistortionCalibrationCenterState独立保持这一会话状态，不以图像中心填缺失输入；径向拟合的初始中心width/2,height/2属于另一参数，不混淆。

原厂模块已补真实受控运行（不调用平台Init）：新建原模块实例并挂正常官方SDK实例，通过原厂SetParam、ProcessAlg完成默认checker图。空CalibPathName返回e0000204，相对文件/相对目录返回e0000200，绝对目录路径创建成功。未存在文件即使RefreshFileEnable=False也自动导出76字节；已存在4字节标记文件且Refresh=False原样保留；Refresh=True覆盖成76字节；显式SaveCalibPath导出成功，错误扩展名返回e0000204。原始记录和两个实际文件载荷见distortion-calibration-module-evidence.json。此实例显式设H1/Weight2，不冒充模块完整Init默认回放。

## 固定核均值与失败边界补证

84 次真实调用的可携记录现保留 meanError。FitChecks 正式断言原生 returnRax 与独立核 null 的失败边界一致，82 次成功、2 次失败均匹配；归一化恢复后固定核矩阵为697/738字段同位，最大差0.0071191043。剩余差异与未确认的生产宿主排序workspace前值来源相关，未把捕获前缀硬编码到产品。

EstimationError 的模型分派不能直接套用文件矩阵的残差：model1 文件 H 是理想像素网格→观测像素，随后另拟合观测像素→经原点/旋转布局处理的物理网格并覆盖最终均值；model2 使用逆径向后的像素→物理网格；model3 使用理想像素网格→逆径向像素。默认 checker 的 model1 后次均值为 0.00435560197，而文件 H 初次均值 0.00435218168，后者是 model3 的估计误差。当前未把未经实现的评估输出补为零。
## 最小二乘独立分支闭合证据

公共固定点入口追加 Weight1 的独立 8345e0 分派，不借用 IRLS。原厂252次调用与独立实现：248成功、4失败边界相同，成功2232/2232 float字段逐位相同。原始证据见 distortion-calibration-least-squares-evidence.json，与84组已携带float点对按sequence连接。关键源顺序是double累计质心、sqrt2/平均距离和(Tw'Tw)^-1Tw'，直接Tw逆虽数学等价却产生可见差异。Gram行列式先形成float二项乘积再乘第三项，恢复括号后消除最后差异。

DistortionCalibrationSolver.SolveFromPointsNative已接公共入口，UI WeightFactor仅一次float乘0.1，畸变迭代次数20。此接线不代表运行插件、重检测评估或Weight2/3浮点差异已经闭合。

公共入口最终明确分开：FitNativePoints直接对应7ff3c0，Weight1仍是常权IRLS；FitNativeLeastSquaresPoints才对应畸变7ed0c0→8345e0。两工具 WeightFun1 不表示相同上游分派。Map真实12个Weight1主fit走前者108/108 float字段逐位一致；畸变252个独立OLS调用走后者仍2232/2232位。旧NPoint404组/11066字段0意外，24既有差异、788病态/并列保持。固定默认权重差异仍为当前未闭合项。

## 默认权重余差的工作区上下文根因

新增端到端原SDK探针同时捕获7ff3c0点对、options、初始H/raw返回、MXCSR/x87控制字，并在7fd4e0入口捕获归一化点和前置H。seq56新旧完整点对SHA相同；E2E与直接原核的7fd4e0入口1296字节全相同。初始8580e0设计矩阵/RHS1386字段也全同，首次差异在首轮权重后矩阵。

7fec40源码的插入排序没有显式index>=0保护，会读取和改写排序数组前缀。原SDK E2E seq56前1float=1f，实际sigma1.48257971，第一轮Huber权重全1；清零workspace直接原核sigma.00767620793。独立原核只改变捕获前缀即可复现SDK的sigma，证明该差异不是需要扩大矩阵门槛的数值误差。

可携最小证据见distortion-calibration-workspace-evidence.json：6组实际输入/系数和2n+32原始float前缀，各以清零/捕获上下文回放，共12个原厂结果。此边界只覆盖已采集fixture；原厂完整scratch布局、前值来源和跨拟合/重置生命周期仍未恢复，已登记DC-WORKSPACE-001。共享生产核不引入猜测性默认前缀，也不把脏值存成用户参数。

DC-WORKSPACE-001内部布局补证：7ff3c0以A(x)=Align128(x)排列workspace。normalizedSource偏移2*A(8n)、normalizedTarget偏移3*A(8n)、designA偏移4*A(8n)、rhs再加A(64n)、Ti再加A(8n)、Tw再加128、pointErrors再加128、weights再加A(4n)、medianSort再加A(16n)。7fd4e0只写2n个float权重，medianSort[-1]因此位于该区尾部未写padding。n=77时weights=8832，medianSort=10112，实际权重写入止9448，余664字节未写；sentinel偏移10108。内部布局已从原厂源码恢复，仍缺外层SDK分配/清理/复用与前次写入来源，不对未定义旧值规定统一默认。受限托管context replay正式12/12原厂sigma逐float位相同。

本轮补查原厂SDK分配生命周期：`trace-workspace-writer-probe.cpp` 在每个板型/模型/warp案例中分别 Create、Run、Destroy `IImageCalibTool`；钩子记录12次 `MVD_Algorithm+0x7ed0c0` 入口的workspace。12次入口复用同一workspace基址 `0x1D81D63F100`，99点样例的中间缓冲基址为 `0x1D81D640280`，80/77点样例为 `0x1D81D63FD80`；首例排序前缀为0，后续新工具实例的同址前缀含前序调用数据。逐条入口位型和源/trace哈希见 `distortion-calibration-workspace-allocation-evidence.json`。这证明受控原厂SDK进程的分配器可在销毁工具实例后复用这块缓冲，不能用每个工具实例独立清零的状态模型解释；仍未证明 VisionMaster 生产宿主采用同一分配器或是否有中间清理，因此不将这组进程内观察硬编码到产品默认值。

## 2026-10-18：固定点新入口三权重生产回归

`FitNativePoints` 专属路径现按 7fcf60/7fd4e0 恢复迭代前反归一化、重新归一化及 H33=1 系数向量重建，保持原厂 float 行列式分组、逆投影分母乘法顺序及 float delta 累计。旧 `Solve` 的原有路径没有采用此适配。生产源码链接的 `DistortionCalibration.FixedKernelChecks` 对 36 次原厂主调用通过 324/324 float 位断言，覆盖 WeightFun1/2/3、三自由度，默认50次/1e-4；原始点对、参数及矩阵保存于 distortion-calibration-fixed-native-map-evidence.json。Weight4 仍明确拒绝，不能将本结果外推到该分支。

畸变84次固定核回放失败边界仍匹配，82成功矩阵现为697/738 float字段同位；其余5次调用41字段与已证实的排序工作区旧值有关，未给生产核填猜测前缀。独立8345e0入口252次调用仍248成功/4失败、2232/2232字段逐位相同。此项不是畸变完整运行闭合。

## 非设备文件与会话生命周期

原厂模块额外受控运行确认：首次运行前显式保存返回80100003且无输出；显式错误扩展名（含大写.ICCAL及无扩展名）返回e0000204，自动导出错误扩展名返回8010000e。一次成功后，即使后续自动导出失败或白图识别失败，显式保存仍成功，且保持上次成功76字节载荷逐字节不变。证据保存于 distortion-calibration-lifecycle-evidence.json，探针只在测试目录。

`DistortionCalibrationArtifactSession` 保存最后成功产物及本轮错误，失败不覆盖产物；释放后清空并拒绝访问。文件自动策略按原厂_waccess存在性（含目录）和RefreshFileEnable决定是否写入，显式保存独立检查精确扩展名。正式FileChecks现87项通过。本节记录当时尚无生产插件adapter的状态；runtime后续接线及评估差异以本页后续“生产runtime接线与原厂对照”章节为准。

识别接线补记：DistortionCalibrationRecognition消费CalibrationBoardSolver独立Recognize/RecognizeCropped，不调用板工具世界坐标求解。畸变7ed0c0的网格单位保持1，不应用编码板物理pitch。RecognitionChecks17项语义检查通过，12/12原厂输入均识别成功；6张圆点板的77/80点数、全部索引及X/Y float位同值。6张棋盘板点数99和索引相同，默认图坐标最大差0.0234983像素，变换图最大差0.455475像素，明确仍未闭合，未以较宽门槛宣称同值。差分报告在analysis/distortion-calibration/recognition-verification/report.json。

## 公共分析入口与零残差分派闭合（2026-10-18）

生产NPointSolver已追加AnalyzeNativeIrlsMatrix/AnalyzeNativeLeastSquaresMatrix：输入原float点对与raw H9，只分析，不重拟合、不改变或归一化H；输出明确错误、float逆投影均值与逐点float误差。两名字分别对应7ff3c0尾段/8006e0；原始汇编证实两者都先计算全部float逆矩阵，再逐点投影，行列式的三因子先按原乘积分组。最初73/82的差异来自反编译C丢失hoist和括号，按原指令恢复后82/82原始H均值和5926逐点字段同位；正式MeanChecks对两个API累计164/164均值、11852字段同位。失败码10103002是托管接口对原core返回0的映射，不冒充原core返回uint。

MatrixToNativeParameters对应原800af0，内部先float逆，再以1f/invH33逐乘归一化；返回Scale/DeltaX/DeltaY/Theta/Aspect/Skew六float（角度弧度）。109受控原始矩阵同时走原core和官方SDK ImportHomoMatrix/Run：107成功、2失败，1284字段逐位相同，原H始终保留。独立SDK接口没有PixelPrecision getter；板/其它工具的额外精度仍由其调用者另行取证。探针生成器与脚本位于tests/DistortionCalibration.NativeProbe，证据为distortion-calibration-matrix-parameters-evidence.json。JSON导出保留负零；不得用整数解析把'-0'丢为0。

Board7ed0c0的真实零残差入口证明：原厂仍执行首轮IRLS正规方程，可返回singular失败；原生产radii<1e-10提前成功是独立差异。现仅FitNativePoints新原生路径取消该早退，旧Solve保留原路径。FixedKernelChecks正式34/34原厂边界（含两个零残差失败）通过，同时Map36次主调用324/324矩阵字段保持。没有新增用户开关或复制数值核。此修正不解决DC-WORKSPACE-001的未知前值生命周期。

## 共享识别回归待处理（2026-10-18）

Map线按原厂813ca0首成功停止规则调整共享识别后，N图像原厂帮助图9帧的168点ID和原有像素门槛仍通过；专属旧测试的Tags.Count=4已按源修正为选中1个payload。完整生成板回归继续保持原厂计数断言，board7生成图现198而原厂197：现有候选点过滤依赖全部decoded-opening几何，首成功停止后未选中码区不再被排除。已向共享识别owner及根线报告，由其协调原厂格网/INT_MIN排除链；未放宽计数或声称共享变更无回归。NImage测试统一捕获异常返回失败，避免失败时弹桌面对话框。

共享识别回归更新：上节生成board7计数回归已由共享owner修复identity与独立几何mask耦合；完整NImage8751项通过，197/225原厂点数门槛及旋转/透视像素门槛均未改变。该项及RecognitionChecks现已登记全量，NImage实际exe211.761秒、单项预算600秒。独立几何排除距离完整原生873de0恢复仍有明确边界。

## 三模式最终估计误差生产接线

`SolveFromPointsNativeDetailed`现执行三模式真实最终阶段：model1在文件H拟合后追加观测像素→原始物理网格拟合，model2逆径向后拟合物理网格，model3使用理想像素→逆径向像素的文件阶段；分别消费公共分析接口给出实际float均值。该工具世界配置在12次原7ed0c0 entry捕获均为origin0/rotation0/方向1/pitch1（trace-main/main-trace.jsonl），不套映射工具的编码物理pitch；纠正中心仍由调用者显式提供。首阶段索引几何复用板线同源Recode，其27次角0/15/90三自由度/权重原厂中间索引已由该线验证，任意病态索引仍未外推。

正式FitChecks现引用真实生产程序集，12组原始点集全部成功并生成有限19-float文件。10组文件全部字段逐位相同，11组最终EstimationError逐位相同；保持这10/11组严格断言。圆板model2默认图的两个近零径向系数差最大9.68e-26；圆板model3默认图的H平移项最大差1.67846e-4，最终均值4.373962e-5 vs原2.365097e-5，与已证seq67排序前缀上下文关联，继续独立未闭合，不扩大矩阵门槛。返回ErrorCode与null失败，不用零补估计误差。本节描述12组早期solver fixture的差异；后续6组端到端SDK runtime replay见末节。宿主注册仍由根线协调。

## DC-TEXT-001：具名误差文字桥接（2026-10-18）

原厂 GetDisplayObjectFeatureOperation → Feature.ValueA/SaveValue 将具名对象 FontSize 写到 `[ParamRoot]_Show Evaluation Error Text.FontSize.Value`；其持久化不是算法 CurValue。FloatConfig_sub 取源 Min6/Max72/Inc1，Object.ConvertValue 用整数字符串解析；小数字号转换失败回落0。公共 ToolDisplayFeatureStore 保存原键到现有 ConfigurationUserData，保留其它键；组合 service 仅开放具名 text 的 FontSize，旧 Show Text 继续由原 service 处理。DisplaySettings 和 DisplayObjectSettings 均不改格式。

BuildNamedTexts 是显式消费入口，默认 Build 不自动新增文本；畸变专属 display adapter 读取源 Content/PositionX/Y/Status，按点生成文字和NG/OK色。Presenter 在图像坐标绘制并在重绘时清理旧标签；原全局 overlay 不受影响。runtime adapter已接入该display provider，但宿主注册由根线协调。不同长度数组采用安全边界，原生不等长数组和零字号绘制尚未证明；WPF 对0字号明确跳过，不称原厂行为。

VisionConfiguration.NamedTextChecks 33项通过：原键/无关键保留、旧快照不变、拒绝/无写入回滚、UTF8限制、真实WPF Spinner编辑和步进、不写CurValue、SolutionStore保存重开、实际Presenter字号/坐标/逐点内容状态/刷新无重复、小数字符串转换和损坏容器拒写。旧ObjectChecks48项通过。测试已加入solution及统一回归；源码构建0警告0错误。

## 生产runtime接线与原厂对照（2026-10-18）

`DistortionCalibrationPlugin` 现提供本工具的真实 `IModule`、公共配置模型窗口、显示帧/图形适配及按 `ModuleExecutionScope` 隔离的 `DistortionCalibrationArtifactSession` 生命周期。成功运行接收拟合产物；失败保留最后一次成功产物和显示快照；scope释放清理状态。入口独立于最终宿主注册，本线没有修改共享注册入口。Release产品程序集构建0警告0错误。

新增 `DistortionCalibration.PluginChecks` 在本机原厂SDK trace生成的1000×1000 Mono8 Board7图像上执行生产runtime，不使用设备。按原厂 `trace-image-probe.cpp` 输入 `CorrectCenterPoint=(width/2,height/2)` 后，首组Board7/model1输出与原厂逐项精确：原始点、19-float `.iccal`、估计误差、普通评估、校正评估及原生0.01舍入后的误差/状态数组均一致。早先差异是检查上下文漏传正确中心点造成，已修正测试，不保留为产品差异。

该检查在同一原厂输入上对Board7/9 × CorrectModel1/2/3共6组做生产runtime重放。原厂模块 `analysis/distortion-calibration/ghidra/module/180020b70.c` 表明仅在ErrorEvaluationStatus与OutputPostCorrectionEnable都开启时改取 `GetCorrectLocateInfo`/`GetCorrectErrorEvaluationInfo`，否则取普通LocateInfo/ErrorEvaluationInfo；`180021330.c` 将所选定位对象按同序输出点数、X/Y。6/6的19-float文件、168个原始/校正定位点、EstimationError、普通/后校正EvaluationError、两套逐点误差数组（经ProcessOutput原厂0.01舍入后）和按严格大于阈值生成的状态数组全部精确相同。通过把原厂导出的矩阵载入插件并对 `DistortionCalibrationWarp` 的校正图运行 `DistortionCalibrationRecognition`，6组均与SDK `GetCorrectLocateInfo`逐点相同，确认该接口在受控样例中等价于校正图重检测；`CorrectErrorEvaluationInfo`逐点和均值对应定位点的逆投影残差，之后由 `ProcessOutput`执行roundf(x*100)/100及阈值比较。端到端严格回放及上下文设置见 `tests/DistortionCalibration.PluginChecks/Program.cs`，原厂trace样本见 `tests/DistortionCalibration.NativeProbe/trace-image-probe.cpp` 和 `distortion-calibration-post-correction-evidence.json`。此结论有6组SDK运行回放支撑，不将单个样例外推到未测试板型/模型参数。

当前畸变专属程序检查：Configuration12、File87、Fit、FixedKernel34边界+36调用、Mean164均值+11852点字段、Pixel12组×192000像素、Radial48、Recognition17、Workspace12及新PluginChecks六组完整原厂runtime回放均通过。设备测试未做。新检查项目未加入Solution/统一回归，待根线统一集成与编排。

## 2026-10-19：拟合、误差与多图显示复核

### 来源与实际路径

- 原厂模块 `IMVSImageCalibModu.dll` 位于 `D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSImageCalibModu/`，SHA-256 `0A6BAC886F1AA1CCC41433EF9D35FC1091B3D43354DE5794563454D8BBA03BCB`；原厂 `IMVSImageCalibModuDisplay.xml` SHA-256 `B63F5BD17DAEBFE62D004BC963D58FB74CC86CC8CF113644271F86F27CDFF7E7`，资源副本为 `src/VisionPlugins.DistortionCalibration/Resources/DistortionDisplay.xml`。哈希和原始资源目录清单见 `distortion-calibration-evidence.json`。
- 原生 `CIMVSImageCalibModu::ProcessOutput`（模块 RVA `0x20b70`，Ghidra导出 `analysis/distortion-calibration/ghidra/module/180020b70.c`）从运行结果取 `GetLocateInfo`；仅当SDK基本参数中的后校正结果已启用且模块 `OutputPostCorrectionEnable` 打开时，改取 `GetCorrectLocateInfo`。该同一条件下发布 `GetCorrectImage` 为 `PostCorrectionImage*`，并取 `GetCorrectErrorEvaluationInfo`；其他已启用评估的情况取普通 `GetErrorEvaluationInfo`。代码先向模块输出写入校正图像，再写 `EvaluationError`、舍入后的 `CalibPointError` 和逐点 `CalibPointStatus`。点数组顺序/数量由 `CIMVSImageCalibModu::ProcessOutputForCalibPoints`（RVA `0x21330`，`analysis/distortion-calibration/ghidra/module/180021330.c`）通过 `IImageCalibLocateInfo::GetPointNum/GetPoint` 保持。
- 同一 `ProcessOutput` 里，评估错误做 float `roundf(raw*100)/100` 后以严格 `rounded > PointErrorThreshold` 生成NG=0/OK=1；不评估的分支把模块运行状态扩展到每个定位点作为Status。托管层 `IMVSImageCalibModuCs.ImageCalibResult` 只确认结果容器初值，不替代上述原生输出逻辑（托管程序集 SHA 和 IL 哈希见本页“托管组件IL复核”）。
- 原厂 Display XML声明两个独立 image 对象：`InImage` 将 `Image` 映射到 `InImage`、默认 `IsDisplay=true`；`Display Post-Correction Image` 将 `Image` 映射到 `PostCorrectionImageData`、默认 `IsDisplay=false`。这是资源声明确认的对象/映射/初始显隐。当前没有原厂桌面绘制或双图同时显示时层顺序的运行证据，因此“两个独立对象已确认”，但“同时显示是叠加还是互斥切换、谁覆盖谁”仍未知。
- 当前真实插件调用链为 `Runtime.ExecuteAsync` 成功后缓存输入 `InImage` 和输出（评估开启时含 `PostCorrectionImage`）→ `LastDisplay` 调用 `ToolDisplayService.Build`，它按 `ToolDisplayDefinition.ImageInput="InImage"` 只选一个基础 `Frame.Image` → 专属 `DistortionCalibrationDisplay.BuildGraphics` 调用 `ToolDisplayGraphics.Build` 与 `BuildNamedTexts`。公共 `Build` 只生成 point/path/rect/coordinate 图形并跳过image；`ToolDisplayImagePresenter.Show` 也只渲染一个 `Frame.Image`。所以两个原厂图像对象虽在资源及运行输出侧存在，校正图没有进入当前 provider/presenter 图像选取链。畸变工具代码没有自行补一个猜测性显隐/叠加规则；公共图层能力由根线所有者单独处理。

### 验证及剩余拟合边界

本轮命令 `dotnet run --project tests/DistortionCalibration.PluginChecks/DistortionCalibration.PluginChecks.csproj -c Release` 退出码0。测试以同一已捕获原厂SDK Board7 Mono8 图像SHA `193658d409084e28c3c0ff64601dd4b1da509a8f23874a03f6b0315fd98b0b37` 执行生产插件，对Board7/9 × CorrectModel1/2/3共6组比较19-float文件、原始/校正定位点、拟合误差、两种评估均值、逐点误差和状态，6/6逐字段通过；未使用相机/设备。独立受控SDK fixture 的回放不证明所有板型、参数、ROI和宿主状态都一致。

本轮 NPointSolver 的原厂 `1807fd4e0` reciprocal-multiply 修正后，`dotnet run --project tests/DistortionCalibration.FitChecks/DistortionCalibration.FitChecks.csproj -c Release` 退出码0：畸变共享固定点核84组中82成功/2失败的返回边界一致，成功矩阵697/738字段同值，最大差 `0.0071191043`；剩余差异位于已证实的排序workspace旧值生命周期边界，生产宿主的前值来源仍未知，不能将捕获前缀写死“修复”。独立最小二乘252例248成功/4失败、2232/2232字段同位；端到端6组独立SDK runtime回放仍精确，但不覆盖上述IRLS fixtures。

### 单个 SDK 工具实例重复拟合的 workspace 复核（2026-10-19）

新增测试探针 `tests/DistortionCalibration.NativeProbe/trace-live-tool-workspace-probe.cpp` 在同一个原厂 `IImageCalibTool` 实例上，以同一张合成80点圆点板重复执行6次，不重建工具、不启用设备或 VisionMaster 平台；在 `MVD_Algorithm+0x7ed0c0` 入口观察排序workspace前缀，并记录固定核入口。可复跑脚本为 `RunLiveToolWorkspace.ps1`，原始构建、运行及硬件写入轨迹保存在工作树的 `analysis/distortion-calibration/live-tool-workspace/`（该目录不纳入 Git）。六次原厂结果的点数均为80、误差均为 `7.76673478e-05`、19项矩阵逐项一致；被观察的 `n=80` medianSort[-1] 前缀六次初值均为零，硬件写入轨迹没有该地址的写事件。故“单个 IImageCalibTool 实例连续重复该输入会积累并复现旧前缀”这一假设被本探针否定；它不否定此前在不同实例销毁后同址复用时观测到的前缀，也不能证明生产宿主的生命周期。该受控 SDK 结论仅缩小 DC-WORKSPACE-001 的未知范围，不据此清零或硬编码产品 workspace。

插件显示层最小验收用例：在同一成功执行快照下，①默认状态只见输入图；②关闭 `InImage`、开启 `Display Post-Correction Image` 后显示原厂 `PostCorrectionImage` 像素且输入层不再显示；③分别隐藏两层应无图；④不启用评估时校正图对象不可误显示旧帧；⑤重新运行/失败后按ModuleExecutionScope清除或保留快照的既定生命周期，不得串到另一节点。两层同时可见时的叠加/切换规则在取得原厂证据前仍标未知；本轮未对该项声称原厂等价。其余专属拟合和评估差分命令均在本文“固定核”“三模式最终估计误差生产接线”和“生产runtime接线与原厂对照”章节，保持既有逐条边界，不把6组运行覆盖扩大为普遍闭合。

### 多图显示实现状态（2026-10-19）

公共 `ToolDisplayImageLayers.Build` 已按 Display.xml 对象顺序解析image映射。畸变 `LastDisplay` 仅使用当前缓存输入/输出，不触发重跑；显示桥接将资源中的 `PostCorrectionImageData` 映射到真实输出 `PostCorrectionImage`，不改算法端口。原厂结果视图默认选中一张底图，也支持 `DisplayImageListDecorator.SelectImageCommandExecute` 勾选叠加对象，并由 `RenderControlBase.AddCheckImage`/`ModuleResultView.AddCheckImage` 加入检查图层。叠加上限三张；对象设为背景图后检查图层可见，源分配 `ZIndex=100/101/102`、Alpha=`191/127/63`。当前公共Presenter支持显式底图、背景图和最多三张勾选overlay渲染；但原厂DisplayImageList复选交互、UserData `CheckedImages` 的切换/恢复及ImageView对应像素合成尚未接通。`Visible` 仅承载原厂 `IsDisplay` 字段，不能直接当成图像选择资格；真实选择/叠加状态独立传入frame。畸变专属检查覆盖资源映射和校正运行像素，WPF Presenter检查frame驱动底图切换及三张图层alpha/z-index。该结果验证渲染合同，不代表交互全等。

原厂显示消费证据来自反编译 `Apps.XmlParser.CapabilityAudit/Apps.XmlParser/ObjectBase.cs` (`GetValue`/`GetImageValue`, SHA-256 `73168438E06DFD74CAE70F910DB9AECDABF313DB0C376753652180C5BC35D006`) 与 `FeatureBase.cs` (`GetValue`, SHA-256 `672D0B57519FA4E750E38E627910B9F11F1B28DA86E9E1980C607004954E5EAD`)，通过Feature的Image映射读取 `VmIO` 当前执行值；对象 `IsDisplayA` 写入 `[ParamRoot]_<对象名>IsDisplay` 的 `VmModule.UserData`，`UpdateResultShowParam` 可将保存状态覆盖XML默认值。结果视图主图路径见 `ModuleResultView.xaml.cs` (`E165657162322FD6B1CE15758847BED07F8C1E86A637815DF8E87FE19439BB88`)，叠加实现见 `RenderControlBase.xaml.cs` (`E13F97E8E82616056BF1CA02EFB61F15527FA7CE8CA0724EF501C9A8F5EFAC67`) 与 `DisplayImageListDecorator.cs` (`B1A5E6F41B69F8E755A5353FAF79CCCC789236FD1DB7191A0CC22A7C9D6077A9`)。`SelectImageCommandExecute` 的显示选择、checked-image 状态和 `AddCheckImage` 是三个独立动作；`RecordCheckedImages`将主图/背景/勾选项写进 `VmModule.UserData["CheckedImages"]`，`RecoveryCheckedImages`按全名恢复；图层 alpha/z 顺序由结果视图独立的 `TransparencyData` 分配。

### DC-WORKSPACE-001：跨实例同址复用与前值写入来源（2026-10-19）

本次只新增测试探针和证据，不改变产品 workspace 或公共拟合核。`RunRecreatedToolWorkspace.ps1` 在原厂 documented SDK 自有进程中保留一张 `IMvdImage`，创建/运行/销毁24个 `IImageCalibTool`，每实例执行2次；每次重绘相同像素、参数固定 model1/freedom1/WeightFun2、圆点80点、480×400、评估开启。无设备或 VisionMaster 平台初始化。`-CheckerPrelude` 仅让第0实例先执行99点棋盘两次，再销毁后执行23个圆点实例，仍共48次运行；探针不写入算法 workspace，只有已有软件入口断点与线程硬件写断点。

受控原厂运行事实：

- 纯圆点场景24个实例48次 `7ed0c0` 入口均复用同一 workspace；被观察的圆点 `medianSort[-1]` 初值全为 `0x00000000`，没有改值写事件。48个19-float产物、点数和估计误差完全相同。故销毁/重建本身并不足以产生非零前缀。
- 棋盘前置场景48次入口也复用同一 workspace；硬件断点提前放在下一次80点圆点将使用的同一绝对地址，捕获棋盘阶段6次改值写入，最终 `0xbf1dd89e`。之后23次销毁重建、46次圆点入口均保留这一位值，没有后续改值事件。日志 `warp` 字段在此专属探针中保存实例序号，不代表几何warp参数。
- 地址映射可由源码恢复的 `A(x)=Align128(x)` 解释：99点棋盘内部workspace从outer+`5*A(8*99)=4480`开始，designA位于再加`4*A(8*99)=3584`，即outer+8064。后续80点圆点内部workspace为outer+3200，其medianSort[-1]为inner+10236，即outer+13436；同一地址落在棋盘designA的`13436-8064=5372=83*64+15*4`，即第83点的第15个float系数。
- 私有只读Ghidra项目导出与实际写事件对应的原指令：RVA `0x8003da` 是 `MOVSS [RAX+RBX*4+0x3c],XMM4`，硬件写陷阱记录的下一条地址为`0x8003e0`。前序指令读取目标Y、翻符号、乘源Y，写入设计矩阵的`-targetY*sourceY`。6次捕获均断言 `R10=83`、`RBX=83*16`、写地址=`RAX+RBX*4+60`、实际bits=`XMM4`低32位。它确认了这个fixture的旧值来自前次棋盘设计矩阵系数，而非推测的allocator填充。
- 本例负前缀下46次圆点输出仍与纯圆点零前缀baseline完全相同：80点、估计误差`7.76673478e-05`、19-float产物逐项相同。该结论与先前正前缀导致sigma差异的fixture并不冲突；前缀存在并不保证每个输入都触发排序越界或改变最终结果。

可复跑顺序：`RunRecreatedToolWorkspace.ps1`、`RunRecreatedToolWorkspace.ps1 -CheckerPrelude`、`RunRecreatedWorkspaceWriterAudit.ps1`、`ExportRecreatedWorkspaceEvidence.ps1`。最后一项对两个场景运行数、同址、零/非零前值、6次写入寄存器、写地址连续性及94个圆点产物（48+46）执行严格断言，任一失败即退出错误。摘要及代表性原始事件为 `distortion-calibration-recreated-workspace-evidence.json`，含原binary、探针、运行轨迹、产物和Ghidra导出的SHA-256。原始96次运行、硬件轨迹、构建输出在本worktree `analysis/distortion-calibration/recreated-tool-workspace/`，不纳Git。

明确未知：VisionMaster真实宿主的allocator/scratch分配scope、初次分配初值、其它工具或生命周期操作对同块内存的覆盖、此前正前缀差异fixture的精确最近写入来源。Ghidra当前把写指令所在块标为`1808002fe` fragment且存在live-in寄存器，不能将其误称为恢复了完整原函数签名或调用ABI。本次确认的是原指令和捕获寄存器；没有将未知状态持久化到方案、没有统一填零或sentinel、没有宣称完整原厂等价。后续生产接入仍需宿主生命周期证据与公共kernel owner协调。

### DC-WORKSPACE-001：正前缀seq56/67最近写入来源（2026-10-19，第二轮）

本轮复用已留存的 `e2e-fit-median-context` 原厂12-case/84次固定拟合序列，不重复上一轮96次负前缀实验。新专属 `RunPositiveWorkspaceTrace.ps1` 在同一原厂SDK自有进程里，利用先前记录的outer相对地址`+13436`与`+34428`作为**测试观察假设**提前布置两个硬件写监视；随后以真实median入口地址、实际固定核workspace地址和寄存器验证对应关系。这两个偏移没有进入产品或公共kernel。SDK core SHA仍为`85D17ECC87D52E9A62D8BB86C8D5208AC999BCD31EC6D0806453F8165733F549`，脚本先校验原binary再使用固定RVA。

原厂运行及原指令确认：

- **seq56（圆点/model2/warp1，77点）**：median前缀`0x3f800000`，sigma再次精确为`1.48257971`；既有独立清零原核回放为`.00767620793`，本轮没有清零或修改workspace。该绝对地址最近一次改值出现在前一个工具实例的**seq46（圆点/model2/warp0，80点）**。原`7fd4e0`中RVA`0x7fdcfb`执行`MOVSS [R9+RSI*4+4],XMM3`，trap-next=`0x7fdd02`。捕获`RSI=94`，R9=seq46-inner+8960，写地址=R9+380=inner+9340，即该次80点IRLS权重数组第95个float（点47的重复Y权重），实际写入1f。后一次seq56的inner地址前移768bytes，同一地址成为inner+10108，即77点`medianSort[-1]`。这确认了前缀1f来自原算法写过的权重，并非allocator统一填充或工具参数。
- **seq67（圆点/model3/warp0，80点）**：median前缀`0x39c6af7a`，sigma再次精确为`3.61378813`；既有独立清零原核回放为`2.20921592e-7`。最近一次改值来自前一个工具实例的**seq63（圆点/model2/warp1，80点）**。原`7fec40`的RVA`0x7ff183`执行`MOV [R10+RCX*4+4],R9D`，trap-next=`0x7ff188`。捕获RCX为uint64`18446744073709551614`（signed -2），R10=medianSort，故目的地恰为`medianSort[-1]`；R9D=实际写入位值`0x39c6af7a`。这是先前已恢复的缺少下界检查的插入排序实际越界写，不是数学核的无状态输入。
- 12个`7ed0c0`入口均复用同一outer地址；两个受控案例的相关工具`destroy-begin`与`destroy-end`均记录同一线程、该地址仍可读、两处目标位值不变，下一实例main入口也保留。seq46→seq56与seq63→seq67之间没有观察到目标地址改值。该事实确认本进程/当前线程的这些销毁操作未清掉捕获值；**不证明底层allocation没有free或一直由SDK持有**，也不覆盖其它线程/真实VisionMaster宿主的清理策略。

只读私有Ghidra补证得到完整被分析函数标识：写权重为`1807fd4e0`、插入排序为`1807fec40`；原始导出在本worktree `analysis/distortion-calibration/positive-workspace/source/output/`。它们不同于上一轮`1808002fe`设计矩阵fragment，不能混称同一来源。两处assembly、具体写事件、销毁前后事件及全部输入/输出哈希收录在 `distortion-calibration-positive-workspace-evidence.json`；原始12次SDK结果、84次固定核上下文、median入口快照、硬件轨迹和构建输出位于 `analysis/distortion-calibration/positive-workspace/`。

严格导出检查 `ExportPositiveWorkspaceEvidence.ps1` 已通过：12次原厂SDK成功、84次固定调用、12入口同址；seq56/67的归一化点、目标点和8个前置系数与旧fixture逐字节相同，前缀和sigma精确复现；真实median地址与固定核布局一致；最近一次写入地址/位值及相应寄存器恒等式通过；销毁前后可读及位值不变。零前缀sigma引用的是已留存的独立原核回放，本轮只复现捕获上下文。

本轮只增加专属测试和文档，没有产品代码或共享数值核变更。明确剩余未知：真实宿主的allocator/arena/free边界、跨线程写入、任意输入的初始旧值、其它n/模型组合的地址重叠，以及如何在安全托管容器中定义兼容策略。正前缀来源的两个原始case已从未知缩小为明确写入路径；它不授权统一填零、hardcode这些位值、或把native越界写作为全局数值开关。

### C1源码生命周期补证（2026-10-19）

最新有限S1/S2切片见`DISTORTION-CALIBRATION-C1-WORKSPACE-LIFECYCLE.md`及`distortion-calibration-c1-workspace-source-index.json`。它进一步缩小上文“allocator/生命周期未知”：原厂Module工具槽→SDK Create/Run/Destroy→core 0x608对象与虚表→每轮type0 descriptor→MVDCommon进程全局池已取得具体方法体与关键ABI。正常Process操作分支返回后归还type0；归还池只改busy/时间，复用不清payload；空闲池压力/年龄、malloc失败重试及atexit清理才到CRT free。故同址历史数据不需要假定工具一直占用allocation，也不能据此清零。记录190个取得方法，47个必需方法体/汇编检查通过；这是源码获取计数，不是分支覆盖或等价率。无新算法运行、无产品修改。容量增长/失败异常、全部尺寸helper、真实宿主DLL绑定和并发压力继续开放，C2排序边界独立推进。

### C2排序与权重边界源码切片（2026-10-19）

完整分支索引见 `DISTORTION-CALIBRATION-C2-SORT-BOUNDARIES.md`，机器证据见 `distortion-calibration-c2-sort-evidence.json`。按PE pdata边界补齐7fd4e0/7fec40原指令与security-cookie后的epilog：排序输入为n个逐点残差，偶数取上中位；插入排序没有j≥0保护，严格新最小值可读median[-1]，前缀大于key时才继续负索引写入。不能把单个前值或2n+32捕获范围当作通用安全边界。权重为2n个重复X/Y值；WeightFun 3超界分支写EPS（2^-23），并非0，依据7fd944/7fdd85。

本轮仅复用6组既有原厂快照做安全离线回放，12个sigma逐位相同、493个排序float逐位相同，另有4个独立分支见证；这些不是新增原厂执行或全域等价证明。没有产品或公共核改动。非有限/非法n实际合同、全部上游分派/归一化（C3）、矩阵运算退化及C1异常/尺寸helper和真实宿主并发仍开放。

### C3配置到拟合分派第一源码切片（2026-10-19）

见 `DISTORTION-CALIBRATION-C3-CONFIG-DISPATCH.md` 与 `distortion-calibration-c3-dispatch-evidence.json`。模块WeightFun旧名转CalibBoardWeightFun；首字符4被模块忽略，core数字ID40002的4在Process应用时会写为算法context的3（缓存原值另存），低层832830分支存在不证明模块可达。真实SDK vtable/参数寄存器、7f7050字段缩放、7ed0c0三模型和三拟合入口已建立分层索引；WeightFactor20传2f，IRLS上限由本地options另写20。同名Weight1在畸变为独立OLS，在映射工具为常权IRLS。probe中的warp是测试像素生成标记，不是原厂参数。只做专属源码/文档，字符串schema全链和模型点构造精确表达式等剩余项显式保留；未改公共核或产品、未新增原厂执行。


C3-2最新源码：`DISTORTION-CALIBRATION-C3-SCHEMA-STATE.md`及`distortion-calibration-c3-schema-evidence.json`已取得原core XML206（REA PE资源证据）、四字段完整ID/default/range和转换/缓存主链。7020/7c6830 operation6只校验；7790缓存原始ID/value并置dirty；Process前7120以operation1应用，Weight4才在algorithm context转3，schema DOM与读回缓存不能称作3。内嵌schema含Ransac4而模块UI无4；模块字符串首字符4仍提前忽略。本轮静态16锚点/65文件哈希检查通过，无原厂执行或产品变更；数值异常token、失败回滚、完整持久化与模型网格/中心精确选择仍开放。

C3模型选点有限切片见 `DISTORTION-CALIBRATION-C3-MODEL-POINTS.md` 与 `distortion-calibration-c3-model-point-evidence.json`：用户中心经SDK向零截断用于a最近点，等距保留先索引；b按四邻域(-x,+x,-y,+y)每向首匹配、严格更短更新，等长保留先方向。径向LM中心初值独立取图像半宽/半高，初始H内部重新拟合；853820并不接收caller H缓冲，其第5/6参为径向系数和中心输出。12原指令锚点与常量/17源文件哈希检查通过，无新原厂运行或产品修改。旋转网格/原点及gridScale全分支仍开放。

C3 gridScale/原点补证见 `DISTORTION-CALIBRATION-C3-GRID-ORIGIN.md` 与 `distortion-calibration-c3-grid-evidence.json`：SDK常量1f经0x67+0c→op0xf+24→options+10，独立于角度和量化边长。X邻接距离变化率严格>0.1f时保留原网格，否则旋转/量化选原点，再按原标签差幅值及旋转符号重建Q；量化值不是最终Q。MODEL-POINTS已补扫描→处理块→回跳CFG表，四邻域仍为(-x,+x,-y,+y)，不能按离散处理块地址排序。grid检查15锚点/8文件哈希通过，无新原厂运行/产品修改；非有限、溢出、无邻接和零距离实际合同仍开放。

C3文件保存有限切片见 `DISTORTION-CALIBRATION-C3-FILE-SAVE.md` 与 `distortion-calibration-c3-file-evidence.json`：19项字段由核心blob→SDK对象+2c→76byte原样写出，独立于另9项主H；重映射为输出像素到输入采样，组合模型先透视再径向。GetResult受Run有效标志门控，但直接Export只查对象存在；自动保存与显式SaveCalibPath及Refresh门控已定位源码/实际path寄存器。原writer不查fwrite/flush/close返回，产品finite/header/尺寸校验属独立选择。本轮16锚点/32文件哈希检查，无新原厂运行/产品修改；catch映射/全部内层失败/路径异常运行仍开放。整卡阶段审计保守5/32，见DISTORTION-CALIBRATION-STAGE-AUDIT.md，切片通知不计整卡S7。

C3模块异常发布有限切片见 `DISTORTION-CALIBRATION-C3-CATCH-OUTPUT.md` / `distortion-calibration-c3-catch-evidence.json`。被动PE EH严格匹配三主函数，恢复4 try组、16 handler与6实际续点：ProcessAlg第一组含创建/Input/ROI/SDK Run，第二组为GetResult/路径/自动Export；捕获码与BASIC success/错误/时间分开写，非主伪码恒0。ProcessOutput先发ModuStatus/runtime，success0跳过结果字段；其返回码被Process忽略，发布失败不等价于最外层Process错误。原厂无已发布字段回滚证据。12锚点/65源文件检查通过，无新原厂异常运行或产品修改；EH外/handler内异常及框架最终消费仍开放，整卡阶段计数不因切片自动增加。
