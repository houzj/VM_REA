# 标定加载 ReadCalibFileModu

本工具从原厂 DLL、托管控件、XML 资源和自有进程受控运行取证，完成独立文件读取、缓存、动态结果端口与配置窗口。没有设备测试。受控 ModuleFrame 输入输出宿主不代表原厂平台或桌面运行，原厂结果控件截图也分别标记其合成模型来源。

## 原厂来源

安装目录 `D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/ReadCalibFileModu` 共 13 个文件，身份、长度、SHA-256、10 个输入、输出声明与 13 个配置控件见 [声明清单](read-calib-file-definition.json)。主 DLL 为 110592 字节，SHA-256 `86b8b112aded58de27a41a5ede6ec6b41117d7db8d0c5934647fa9514e6fa403`。

托管源码在本工作树 `analysis/full-audit/calibration-dependencies/decompiled/ReadCalibFileModuControl` 与 `ReadCalibFileModuCs`。REA 静态托管检查 `ev_ca66bc7e6c4cc7a9571f53344daf7263f2ee1dc9fa37fd17dc18320bb536c94d` 证明 Control.dll 身份；原生 DLL 用 REA 打开确认 PE/x64/SHA-256，原生反编译由独立 Ghidra 数据库完成。REA 原生 provider 未配置，使用现有本地 Ghidra，没有修改其共享连接设置。数据库与导出位于本工作树 `analysis/full-audit/read-calib-file/ghidra-project`、`native`，未打开其他任务数据库。

| 方法/对象 | RVA | 依据 |
|---|---:|---|
| CreateModule / DestroyModule | `0x10e80` / `0x10f20` | 仅有两个导出，正常平台创建会再执行 Init |
| 构造 / 析构 | `0x8e30` / `0x9150` | 实例大小 `0x180`、初始空文件、SaveCalibDataEnable=True |
| 虚表 | `0x14a48` | Process/GetParam/SetParam/保存/加载的真实入口 |
| GetParam | `0xa2d0` | FilePath、GetCalibInputParam 描述协议、DynamicInData/OutData |
| Init | `0xaa90` | 真实平台上下文与模块 ID，本受控宿主不执行 |
| LoadFile | `0xab50` | 磁盘 XML 与方案 FileData 的两种读取分支 |
| Process | `0xc520` | 刷新、输出顺序、九路姿态覆盖、模块状态 |
| SetParam | `0xd120` | 文件清空、参数、动态 IO 数据；探针不把未定义返回寄存器当加载结果 |
| AnalyzeExtraIO / ProcessExtraIO | `0x9410` / `0xc100` | IsResultShow 筛选、同名读写、float→int getter 回退 |
| SaveModuleData / LoadModuleData | `0xcf10` / `0xbef0` | FileData 开关与动态 IO 字节记录持久化 |

## 真实运行与文件语义

[运行证据](read-calib-file-evidence.json) 保存完整事件顺序、输入、getter 返回、逐次输出及 9 个 XML fixture 原文/哈希。当前包含 25 次原厂 Process、13 次 LoadFile，以及实际 LoadModuleData 的 FileData 缓存加载。入口为 `tests/NativeEvidence/Invoke-OriginalReadCalibFileProbe.ps1`；构造和方法 RVA 均源确认，仅在自有实例的 ModuleFrame IAT 输入/输出/运行信息位置装入合成服务，校验原始目标并恢复，不修改算法、授权、安装文件或其他进程。

- 默认无文件、文件不存在、XML 格式错误之后 Process 仍返回 0；没有结果时输出 ModuStatus=0。文件读取本身返回 `0xe0000201`，不能把它改成每次 Process 失败。
- XML 外层根名不限 CalibInfo。已解析文档再清除旧参数集合，缺少 CalibOutputParam 返回 `0xe0000017`；文件打不开或 XML 语法错误在清除前返回，保留旧结果。
- 输入参数识别 int、float、string、CalibPointList；输出识别 int、float、string、FloatList、CalibPointF。标量数值按原厂前缀扫描，未知类型忽略。
- FloatList 的坏数值会保留前一个成功解析的 float。CalibPointF 按 XML 子节点顺序发布其实际节点名；X/Y 的负值照常发布，第三项 R 只有 `>=-990` 才发布。第四项使 LoadFile 返回 `0xe0000207`，已解析的前三项仍可输出；两项姿态也接受。
- ModuStatus 表示已加载的输出集合是否非空，数值不是标定矩阵有效性的重新判断。CalibMatrix 作为任意长度 float 数组发布；恰好 9 个有限值时额外提供独立框架的类型化 CalibData。
- 文件只在配置变化或 RefreshSignal 非零时重读。磁盘变化本身不刷新；刷新失败保留上次结果。九个 `*1` 姿态输入仅覆盖本次对应分量，不改变缓存文件，不保留到下一次运行。
- SaveCalibDataEnable=True 时方案包含加载的 FileData，False 时不保存该块。加载缓存可以没有源文件或 FilePath。DynamicInData/OutData 原始 XML 始终独立保存。独立方案采用 JSON 参数 CachedFileData 表达同样的生命周期，没有冒用原生二进制方案格式。

原厂输入参数展示协议是 `类型#名称,值$;`，点用 `X~Y~R`。该协议用于 GetCalibInputParam 与原厂 ReadFileConfigData，并非磁盘 XML 格式。GetCalibOutputParam 在该 getter 内没有生成内容的分支。

## 额外结果与共享接口

[额外 IO 规格](read-calib-file-extra-io.json) 附带最小原厂 XML、真实 int/float 数组、600 字符字符串、未绑定与 float getter 回退事件。只选择 Output/Items 中 Others 包含精确子串 `IsResultShow=t` 或 `IsResultShow=T` 的项，ValueType 对 int/float/string 不区分大小写。数值按 getter 实际数量发布，string 只读首项；float getter 返回非零才尝试 int getter。

额外展示最后覆盖输出；未绑定不写，已有文件结果保留，不继承上次运行的额外值。独立隐藏参数 DynamicOutData 保留原始 XML，DescribeNode 从该参数生成同名输入/输出。int/float 数组以 NumberArray 承载，int 数组逐项保持 32 位整数语义。

主线负责 CAL-005/006：节点描述读取持久化 OutputSchema 与 DynamicOutData，不在 GetDescriptor 中读磁盘；共享注册冻结动态端口、验证连接并执行显式整数到浮点转换。刷新导致文件输出类型或名称超出已确认定义时，独立框架返回 CALIBRATION_SCHEMA_CHANGED，需要在配置中重新读取确认。这是明确的宿主类型边界，原厂受控宿主没有该限制。

本工具 Display.xml 的活动对象仅为 Data Record/datarecord，将 `Module Status:{0}` 映射 ModuStatus；Show Text 被注释，AlgorithmTab 没有 TextDisplayGroup。通用文字叠加服务使用主线与旋转工具的 [显示合同证据](rotate-calibration-display-contract.json)，本窗口不添加原厂未声明的文字叠加选项。

## 界面与验证边界

原厂 ReadFileConfigData.AnalyzeString、ReadFilePanel、原始 DataTemplate 实际执行并渲染，保留全类型、无效条目、空值和原厂模块实际 GetCalibInputParam 文本四个场景。原厂模板需要正常注册 UserTempConverter 的扩展字典；未注册时的空布局截图不作为有效 UI 证据。最终截图与加载依赖哈希进入运行清单，明确不是平台桌面操作。

独立配置窗口覆盖原厂文件、刷新订阅、方案存矩阵、三组姿态订阅、只读三列点表与参数文本，使用同线公共主题。中文字段名来自原厂实际 GetLang 结果。截图辅助均位于 tests，产品程序集没有截图代码。

`tests/ReadCalibFile.PluginChecks` 按原始事件逐条比较错误码、输出存在性、类型、数组和值，覆盖缓存保存重载、磁盘变化、刷新失败、方案作用域、坏 OutputSchema/DynamicOutData 的图验证，以及专属 WPF 渲染。最终数量、构建结果及源码哈希见 [验证索引](read-calib-file-verification.json)。主线负责宿主注册、真实图执行与可操作界面验收；用户确认通过后才进行中文提交和推送。

原厂异常 XML 可能导致空指针/不定行为；独立读取器返回可诊断错误，不复刻内存故障。框架拒绝非有限数值发布，报告兼容性错误，不把 NaN 改为 0。有限场景通过不构成任意文件、全部平台状态或逐像素界面等价的证明。

## 公共配置模型迁移（待手动验收）

标定加载现提供 `IToolConfigurationProvider`，原厂 AlgorithmTab/Group 以内嵌资源解析，统一宿主创建窗口；专属代码仅适配文件选择、只读数据面板、参数和typed操作。旧专属Window仅兼容保留，不作为新架构验收证据。独立配置检查41项、原受控执行回归810项通过，详见 `tests/ReadCalibFile.ConfigurationChecks/README.md`；公共壳源值回归另计。默认打开目录历史与通知已接公共服务（29项合同检查，独立JSON/WPF外观映射）；远程文件分支仍明确Unsupported，当前不能称完整配置闭合或原厂桌面像素等价。用户手动验收前不提交推送。
