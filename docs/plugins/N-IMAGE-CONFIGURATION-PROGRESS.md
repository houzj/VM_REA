# N图像标定：独立工具线配置与数值接线

## 当前状态

本轮按工具清单独占 `IMVSNImageCalibModu`，没有把 N 点标定拆成多个并行分支。产品已新增公共配置模型、真实像素识别→批次求解→37个原厂输出、矩阵修正、模块文件导出及公共显示接线；工厂为 `new NImagePlugin()`，宿主注册由根线统一完成。没有设备测试，没有修改其他工作树，没有提交。

## 原厂证据

- 直接复制原厂 `IMVSNImageCalibModuAlgorithmTab.xml` 为 `Resources/NImageAlgorithmTab.xml`，原始 SHA256 `a83474fec309e81eaed70366ad813a2d5a06a02a8dcc9f12d398079e16e5c3c3`。原文件直接声明基本参数、运行参数、结果显示三个 Tab；结果显示内链接公共 `TextDisplayGroup`。不是另造结果文本页。
- 原厂 footer 只有生成标定文件、清除图像、确定。XML 没有执行、连续执行按钮；原有 `NImageWindow` 的这两个按钮不作为生产接入依据。
- `CanTriggerRun` 仍由公共模型和 `ToolConfigurationExecution` 处理，不能因为没有执行 footer 就漏掉修改参数后的原厂触发行为。
- `HidePhysicInfoOperation` 来自原厂控件源码；REA 静态证据仍为 `ev_47d8fe724a3d57ef879aa778e0fb1e24e74df258e6f6f9266459af03c8e39ca6`。原始 parser 动态记录继续保留于已有 UI evidence，静态分析不冒充桌面运行。
- 默认标定板采用原厂AlgorithmTab的 `0x7`（生产配置初始化值）；受控模块裸构造的GetParam曾返回6，两者不是同一初始化阶段，不能把裸构造值当作打开配置后的默认值。
- `SetParam` RVA `0x229d0`：`CalibPathName` 的空路径被拒绝；它不会加载文件，也没有 `SaveCalibPath` 的 `.xml` 后缀处理。废弃的 GrayContrast、SubpixelWinAuto、SubPixelWindowSize 写入直接返回成功、不会影响算法；保留源隐藏字段。
- `ProcessAlg` RVA `0x21450`：可选调用原厂 MatrixFix，把算法矩阵、图像点、物理点依次交给修正工具。新受控 SDK 运行见 [MatrixFix 原厂五案例](n-image-matrix-fix-evidence.json)，包含单位、仿射、透视、非单位 h8 和全零矩阵；全零矩阵也返回成功。
- `CareraMove` 的模块字段 `0x16104` 仅出现在构造、Set/GetParam 和 WriteXmlData。保存函数 RVA `0x230e0` 仅在恰好三张旋转图且方向关系有效时翻转文件 `RotDirectionState`，并通过该关系翻转 `IsRightCoorA`；其余旋转数保留 `-999` 关系，不改变矩阵。原厂12帧模块运行、两次保存均成功，确认方向和handedness分别由1变为-1，见 [三旋转文件证据](n-image-three-rotation-file-evidence.json)。这是受控实例的原厂方法调用，不是完整平台 Init。
- 原厂 `7cb4c0` 的相邻旋转图匹配保留前图全部点：后图缺失网格 ID 使用最近格点、平均相邻格长和累计归一化轴外推。完整相邻点修正后，2/3/5旋转图中心和径向误差的15项官方SDK差分通过。
- 原厂 `7cf3f0` 在仅旋转分支保留初始化的全零9元素矩阵，同时成功返回真实中心和误差。新鲜进程官方SDK的2/3/5张旋转图均确认该行为，不用单位矩阵伪造。


## 实现与公共框架依赖

| 清单项 | 原厂证据 | 消费的公共插件框架 | 主框架接口/生命周期 | 产品验证与状态 |
|---|---|---|---|---|
| 配置模型与模式 | 原厂 AlgorithmTab、HidePhysicInfoOperation、已有 parser 记录 | ToolConfigurationDefinition/State、标准与订阅控件、EnumerationG、对象模板、统一标定路径控件 | ToolEditorContext 的订阅、Apply、执行状态、Reset 回调 | 新 `NImageConfigurationProvider`，不返回独立旧 Window；实际 WPF 模型检查 |
| 文件/清图 | SetParam、WriteXmlData、原厂模块导出 XML | ConfigurationFiles/SaveFiles/Confirmations/Notifications、独立 UIUserData 域 | owner 传入真实 clear/export，拒绝写入必须保留先前状态 | 共享文件选择与原厂 footer 接线；NImagePlugin owner实际清图/导出和持久会话 |
| 图像批次与求解 | Process/ProcessAlg、既有原厂角点与矩阵记录 | NImageRuntime、NImageBatchSolver、已有平移/旋转求解器 | 生产模块使用 procedure-bound ModuleExecutionScope，不全局按 NodeId 缓存 | 官方角点隔离数值与独立像素批次分别验证；实际像素9图得到168公共角点及真实矩阵 |
| 矩阵修正 | MatrixFixCpp.h、五次真实原厂 SDK Run | 新 NImageMatrixFix | 产品不依赖原厂 DLL；完整模块在 HomoFixEnable 时调用 | 50 项逐分量/调用方矩阵不变检查 |
| 相机移动文件方向 | WriteXmlData 对 0x16104 的唯一消费 | NImageFile.ForCameraMovement | 保存时应用，不应改变执行结果矩阵 | 源码规则及12帧原厂双文件保存差分 |
| 完整注册/显示 | 原厂 37 输出、Display.xml、原生误差阶段 | 已实现 IVisionToolPlugin、IToolDisplayProvider、会话、动态输入/输出 | main 消费显式插件注册，不另造发现系统 | NImagePlugin生产实现；37原厂字段及显式扩展CalibData，显示使用公共Graphics投影 |

配置中 UI-only RadioSelecter 使用 `ConfigurationUserData` 的 `Param.*`，与真正算法参数分开；测试曾错误期待它写入普通参数，已按原厂域修正。有限数/浮点可表示守卫是独立实现，不声称原厂非法数路径相同。

## 验证边界与剩余验收

生产入口为 `NImagePlugin`/`NImageConfigurationProvider`，不使用旧 `NImageWindow`。实际像素批次、跨流程同NodeId隔离、清图、次批清旧矩阵、真实XML、显示轨迹、取消和缺失修正点均有执行验证。数值阶段覆盖2/3/5旋转与9平移+0/2/3/5旋转的误差、机构误差、方向向量、离散最大索引及告警状态。相邻旋转缺点外推与公共归一化的源码修正解决了此前差异，没有按案例硬编码。`7d12e0`四图子集筛选的逆投影、sqrt和评分累加也改为原厂float顺序，218项仍通过；该评分修改单独不消除文件矩阵差异；真正消除项为后续完整float逆矩阵，两个实验没有混为一谈。

这些验证不代表所有原生条件完全一致：标定板识别是独立实现，任意畸变/裁切/噪声仍需扩大样本。此前168点板矩阵仅37/81分量位一致，三旋转文件h2差+0.0001025864超出原1e-4门槛；已追到共享7fcf60/7fd4e0世界归一化矩阵必须使用完整float伴随求逆，而不是数学等价的1/sw、wx/wy。四变体隔离验证后由根线接入公共核，当前板矩阵81/81及文件矩阵9/9 float位一致，原1e-4断言恢复，并新增位断言，未扩大门槛。参见[完整float逆矩阵证据](n-image-float-inverse-evidence.json)。RGB已补真实彩色原厂SDK差分：9图每通道不同，RGB24与76/150/30>>8生成的Mono8输出JSON完全一致，见 [彩色运行证据](n-image-rgb-evidence.json)。生产输入声明DataKind.Image并消费公共VisionCore.ImageColorConversion.ToMono8，不引用图像源工具私有实现。该证据仅覆盖已测彩色场景，原厂内部转换静态调用链仍未知。原厂桌面UI差分与用户手动验收仍需宿主启动。无设备测试。

同进程反复创建/销毁原厂旋转SDK实例观察到角点元数据累积；数值测试改用新鲜进程完整相邻角点，保留原始异常记录，不将其作为产品期望。

验证入口：`tests/NImageCalibration.PluginChecks` 及 `--configuration-only / --batch-solve-only / --matrix-fix-only / --complete-rotation-only / --output-metrics-only / --board-fit-only / --module-only / --file-only`。原厂探针为 `tests/NImageNumericalProbe` 与 `tests/NImageMatrixFixProbe`。产物在当前工作树 `analysis`，最终计数记录于交付回报；测试通过不等于完整原厂一致。


## 本轮交付检查

最终独立产物目录 `analysis/build/nimage-translation-origin-production-20261017` 构建0警告0错误。配置16、文件257、批次19、完整相邻旋转15、原厂标量/文件行为229（含原1e-4、9+0及9+3的SDK/静止/移动文件矩阵9/9float位正式断言）、168点板拟合9（每张9分量float位断言）、MatrixFix50项通过。旧完整识别/模板/序列/文件回归8751项通过。生产实际像素63项回归包含RGB与显式Mono双批次、真实修正、自动写文件、清图和生命周期，精确最终计数见运行报告 `analysis/full-audit/n-image-calibration/module-runtime/verification.json`。

共享Point组合以IToolPointInputAliasProvider声明三组原厂组合与轴：PhyPoint→MovePhysicalPointX/Y、Image Point→PicPointX/Y、Physical Point→PhysicalPointX/Y；主宿主使用公共原子绑定服务，插件不再独立保存另一份组合身份。根线已完成NImagePlugin显式注册及实际MainWindow三页签打开检查；后续启动用户手动验收和提交由根线协调。没有提交/推送，没有改其他工作树。

最后计数边界审查：SDK新鲜进程9+1和0+1均抛0x10000019，见[n-image-count-boundary-evidence.json](n-image-count-boundary-evidence.json)。虽7cf3f0内部对旋转数<2跳过旋转段，外层SDK验证使单旋转分支不可达；产品保留同错误码，不能仅凭内层反编译改成成功。初始float正规方程单独不改变旧微差；后续完整float逆矩阵的隔离结果及生产接入另见上述证据，不将阶段回归混写。

完整float逆接入生产后的独立重建确认：220原厂输出/文件、9板矩阵、19批次、63实际像素生产、18旧solver均通过。浮点位一致结论限定于上述已测168点板拟合、9+0和9+3矩阵文件。后续9+0原点隔离确认，必须恢复7d12e0的invTranslation/(inv8+floatEpsilon)及float有符号乘减顺序；经根线授权接入后，原SDK矩阵和静止/移动XML均由7/9提升至9/9位一致，见[n-image-translation-origin-evidence.json](n-image-translation-origin-evidence.json)。没有把该有限案例结论扩展到任意图像。

9+0生产接入后，229项原厂输出/文件、9板矩阵、19批次检查通过；生产17张实际像素63项回归及18项旧solver也通过。没有修改公共Solver，本轮只改NImage专属原点投影顺序。

2026-10-18：实际宿主已注册 NImagePlugin，用户通过 CMD 启动的双标定节点验收版检查后明确回复“通过本轮验收”。本轮公共显示、点绑定与 N 图像生产接线据此进入中文提交和同步通知；标定板、相机映射仍为独立在制任务，不随本次验收算作完成。
