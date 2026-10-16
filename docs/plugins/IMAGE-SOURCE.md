# 图像源：证据与交付状态

P0 进行中。已建立本地执行的源码和原厂 SDK 运行对照；三种模式的完整桌面 UI 证据和公共服务尚未补齐，不标记完整交付。

完整声明、触发条件、源码哈希和运行摘要见 [image-source-evidence.json](image-source-evidence.json)。原始证据根目录为 `D:/MyWork/REA逆向测试/analysis`；本轮产物在当前 worktree 的 `analysis/full-audit/image-source-state`、`image-source-runtime`、`image-source-checks`。

## 原始身份和证据入口

原厂 ImageSourceModule.dll SHA-256：`91ea3659be97e51edeab0c913779d4555d51dcd0d6288086fbf9916d35b942db`。

- 参数/输入输出/UI：full-audit/image-source-audit 的 Algorithm、AlgorithmTab、Module XML。
- Process：full-audit/image-source-native-0.c，入口 0x18000d820；REA Evidence `ev_61cb8f2378ce6e9eea15477a66aa989f8fff80d4f608c2d300ecb6bd658e7bc5`。
- GetParam/SetParam：native-2.c、native-3.c；缓存/拼接/目录监视另见 native-7.c、native-9.c、native-extra-25.c、native-extra-26.c。
- 托管 UI：decompiled/ImageSourceModuleControl；VMControls.WPF/ParamsConfigControlBase；Apps.XmlParser/AlgorithmTabRoot。
- 新补 4 个列表函数：0x18000b430 按索引取项、0x18000b4c0 游标前移、0x18000b550 按路径定位、0x18000b8e0 末项判断。使用已有 Ghidra 12.1.4/JDK 21 CLI 分析校验过哈希的副本；结果在当前 worktree。它们是 CLI 新证据，不冒称 REA MCP Evidence。脚本在 tests/NativeEvidence/VmNativeCapability.java。
- 原厂运行探针：tests/NativeEvidence/OriginalImageSourceProbe.cs。早期状态记录为当前 worktree analysis/full-audit/image-source-runtime/probe11.json；本轮运行与有效 UI 记录为下文 run-85fcec94893c4a63bd0106b9b2889789/probe.json。

可重放入口：`tests/NativeEvidence/Invoke-OriginalImageSourceProbe.ps1`。该脚本已在独立输出目录重新运行成功；可用 `-CaptureSdkUi` 尝试 SDK 控件宿主，空内容仍应拒绝验收。SDK 安装副本的 ImageSourceModule.dll 哈希与分析目标相同；这属于文件身份核对，不是运行调用栈归因。

REA MCP 当前缺少 Ghidra 环境配置，Windows 原生 UI 操作也不支持该主机；使用已有 CLI 和 Windows computer-use 继续采证，未修改原 checkout 或其工具安装文件。

## 原厂 SDK 实际运行结果

探针在独立进程加载安装目录 CircleFind.sol 样例，不保存方案。样例加载报告其他模块失败（IMVS_EC_MODULE_HAS_LOAD_FAIL），随后确认并单独执行存在的 Flow1.Image Source1；不等于整个样例方案验收。

| 场景 | 观察 |
|---|---|
| InitialSN=7，三次自执行 | SN 8、9、10 |
| 改 ShowImageName 后运行 | SN 11，没有重置 |
| a/b/c 列表，当前 a，开启自动切换 | 输出 b |
| 关闭自动切换，当前 a | 输出 a |
| 按 c、a 顺序选择 | 依主列表顺序输出 a、c |
| 清空选中列表 | ErrorCode 0xe0000203，空图像 |
| PathCache=1，a 缓存后移除自建 fixture a | 仍输出 a，SN 16→17；随后恢复 fixture |
| 相机模式，CameraID=-1 | ErrorCode 0xe0000309；未连接真实相机 |
| SDK 模式未提交帧 | ErrorCode 0xe000000f |
| SDK SetImagePath 后执行 | RGB24，1296×1024，3981312 字节；结果 SN=0、名称/路径为空 |

错误帧的 SN=0 是 SDK 结果对象缺少输出后的默认值，不证明内部 SN 被清零。自执行 Run 未抛异常也不证明成功，必须检查 ErrorCode 和图像长度。

随仓库样本输入 SHA-256 为 `690b17333ce8a9f6fe8961c90d7f21fbd6472dbf0f7cc3443fc89da43bb33aba`。原厂本地 MONO8 输出 1296×1024 / 1327104 字节，SHA-256 为 `96187178e18c6e0104e79909116de929b6dceab22a595c6ab8abeefdd57d2098`；重建端该样本哈希一致。不能由此推断任意图像、彩色转换或其他格式等价。

## 源码确认的执行语义

- SetParam InitialSN 写计数器，Process 成功输出前先加一；显示参数变化不会重建计数器。
- Process 在读取前调用游标前移，递增后超过末项回到 0。读取失败已消费位置；SN 在成功输出路径才增加。
- CurrentImagePath 只选择已有列表项，不等价于 useLocalPic 一次性提交。
- SelectExecutePic 按主列表索引插入独立执行列表，空选中列表不回退；AddImageCheck 对路径进行字面去重。
- Sleep(Interval) 发生在选图后、模块耗时起点前；SetParam 只接受 0–1000ms。
- 路径缓存未满时复制当前图像，命中直接复用；没有 LRU 淘汰。现阶段重建只支持 MONO8 缓存。
- AutoPlay+AutoStop 还需要所属流程运行参数满足条件，调用 VM_M_StopOwnerProcedure 后仍输出当前帧；不是插件永久 NoMatch。
- OutMono8 是 RGB 的附加转换，不应把 MONO8 主图重复输出为附加图。
- 原生目录过滤常量由二进制 RVA 字节确认：jpg、png、bmp、tif；最终调用 SortImageNameInFolder / FUN_180023550，比较器尚未完整映射，不能假设简单 Ordinal 排序。
- 目录监视采用完成端口/事件标记，由 Process 消费更新，RefreshImage 还关联 ReportData 通知；逐次扫描 BMP 不是等价实现。

批量 JSON 参数和逐条原厂 SetParam 的适配、跨模式两个游标的完整继承规则、缓存跨格式失效、重载/Reset 的全部原生生命周期仍未完整验证。

## 三种模式 UI

机器可读证据保留了全部 25 个声明及触发 Setter；本轮另取得原厂 SDK 控件的三模式截图、25 项实时参数模型，以及本地展开和 RGB24 展开截图。桌面独立参数窗口仍未完整采集，不能把 SDK 宿主等同于桌面窗口。

| 控件 | 本地 | 相机 | SDK |
|---|---|---|---|
| PixelFormat / SolSaveImageData / ShowImageName / PathCache / AutoPlay | 可见 | 隐藏 | 隐藏 |
| CameraID / ExposureTimeInput / GainInput | 隐藏 | 可见 | 隐藏 |
| InitialSN / StitchEnable | 无模式隐藏 Setter | 同左 | 同左 |
| AutoStop | AutoPlay 关闭时隐藏 | 隐藏 | 隐藏 |
| OutMono8 | MONO8 隐藏、RGB24 显示 | 原厂 SDK 控件实测可见 | 原厂 SDK 控件实测可见 |
| StitchStartHeight / StitchHeight / ClearTrigger | StitchEnable 开启显示 | 同左 | 同左 |
| ClearTriggerCondition | 另依赖 ClearTrigger | 同左 | 同左 |
| TriggerString / TriggerFilter | 字符过滤开启显示 | 同左 | 同左 |
| 目录子项 | 本地+文件夹模式显示 | 隐藏 | 隐藏 |

相机列表来自全局模块类型 6；曝光和增益是 float 订阅，字符过滤是 string 订阅，清除触发是 int 订阅。SubscribeFolderPath 是目录选择器，长度 200。

UI XML 的 StitchEnable、OutMono8、AutoStop 默认值为 True，算法 XML 为 False；已有原厂本地截图显示拼接和末张停止关闭。原生 Init 加载算法默认 XML，因此不以 UI 声明覆盖运行默认值。InitialSN 范围 0–100000、PathCache 0–5、两个拼接高度 0–20000。

桌面窄窗口与 SDK LocalImageList 是不同上下文；LocalImageList.Init 只有 SDK 模式才强制显示列表。当前未把 SDK 列表复制进桌面参数窗口。

此前桌面捕获失败：原始主窗口可访问性可读，但 Windows.Graphics.Capture 超时，点击返回 coordinate input geometry is unavailable。失败日记在 image-source-runtime/desktop-journal.json，问题尚未解决。

SDK 空白问题现已定位并修复测试宿主：Hik.Xml.BaseNode.loadXml 在读取前设置 FileInfo.Attributes；直接加载只读证据路径会抛出访问拒绝，AlgorithmTabRoot 构造函数捕获异常后留下空模型。探针将安装 XML 按原字节复制到本工作树输出，先显式加载原厂 ImageSourceModuleControl.dll，再建立原厂 ParamsConfigControlBase。未修改安装目录的 XML、DLL 或属性。此前空白图继续拒绝，原来“XML 不存在”的推测已撤回：路径存在，问题是写属性权限。

有效原始记录：`analysis/full-audit/image-source-runtime/run-85fcec94893c4a63bd0106b9b2889789/probe.json`。7 张原厂控件图包括 mode-1/2/3（420×700）、local-expanded/local-rgb24 及两张 420×1600 tall 图；每张路径、尺寸、SHA256 已写入 image-source-evidence.json。tall 图仅扩大测试宿主以检查下部内容，不代表原厂窗口的默认高度。原厂模型实测确认相机/SDK OutMono8 可见，以及空“订阅图片文件夹”标题保留。默认三模式 72 项行显隐已逐项纳入重建测试。

限制：原厂宿主仍记录 zh-cn.xaml 回退异常和 LocalImageList BooleanConverter 绑定异常；截图中的中文标签可见，列表缩略图已出现，但本轮不宣称本地化或列表选中状态等价。相机未绑定，订阅浮窗未操作。SDK 列表不应直接放进桌面窗口。重新采集使用 `tests/NativeEvidence/Invoke-OriginalImageSourceProbe.ps1 -CaptureSdkUi`，生成后将新 probe.json 路径传给 Export-ImageSourceEvidence.py 的 --runtime。

## 当前实现和验证

已修正本地 MONO8 列表、SN、读前游标、手动选择、选中执行顺序、去重、失败后游标、首 N 路径缓存及可取消间隔。普通参数变化不再重置全部运行状态；同节点以 SemaphoreSlim 串行，不持有全插件锁睡眠；不同方案目录分离状态。并发/取消是独立托管实现，完整 session 身份仍待框架服务。

已补三种模式的相机订阅行、RGB 的 OutMono8、拼接和过滤依赖行。本轮依据原厂截图校正文案、方案存图 120 张提示、曝光/增益等订阅文本框、数值和文本订阅图标、相机图标、文件夹文本长度 200。图标来自原厂提取资源，来源哈希已记录。缺少服务的控件保持禁用；实际订阅弹层、目录选择器和相机枚举尚未接入，不能宣称外观和操作完整等价。

末张停止、目录订阅、RGB、相机/SDK、内嵌图像、拼接、字符过滤仍明确报告缺口；已移除永久 NoMatch 和逐次扫描 BMP 的错误替代。BmpReader 为独立实现，仅上述样本已做像素哈希对照。

构建：`dotnet build tests/ImageSource.PluginChecks --artifacts-path analysis/build/image-source --disable-build-servers`，运行产物 ImageSource.PluginChecks.exe。136 项通过，0 警告/0 错误。覆盖源码映射的本地状态、缓存、错误、参数边界、同节点并发、间隔取消、宿主持久化后运行、原厂三模式 72 项显隐对照、订阅字段和布局宽度，以及该样本的原厂像素哈希。四张重建 QA 图经检查；截图/原厂探针均在 tests，框架源文件未修改。

## 未完成验收项

- [x] 本地关键状态源码映射、4 个辅助函数反编译和原厂 SDK 操作对照。
- [x] 三种模式声明/托管规则；未绑定相机、SDK 未提交和已提交的实际运行结果。
- [x] MONO8 样本像素对照与 136 项重建检查。
- [x] 原厂 SDK 宿主三模式与展开状态截图、25 项实时参数模型及显隐对照。
- [ ] 本地/相机/SDK 完整桌面截图及操作序列，含下部区域、OutMono8 联动和订阅弹层。
- [ ] 彩色、调色板、损坏图像、多格式语料以及原生转换系数。
- [ ] 目录排序比较器、事件合并、RefreshImage/TriggerProcessRun、释放和重载。
- [ ] 拼接、过滤、预览文字、内嵌图像、真实流程停止。
- [ ] IS-001/002/003/004 由框架线落地后接入公共服务并完成集成验收。
