# 图像源：证据与交付状态

P0 进行中。非桌面部分已完成本轮实现、真实相机对照和回归；公共服务已由插件线接入。原厂桌面三模式、订阅/相机/目录弹层及远程目录分支仍待现场证据，不标记完整等价。

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

REA MCP 原生 Ghidra 配置曾不可用，使用已有 CLI 取得聚焦源码，并保留工具来源区别。Computer Use 曾定位运行中的 VisionMaster，但截图报 FrameArrived timeout，随后用户按 Esc 停止操作；未将 SDK 宿主截图冒充实际桌面进程证据。未修改原 checkout 或其工具安装文件。

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
| 相机模式，CameraID=-1 | ErrorCode 0xe0000309；实机绑定后的新对照见下文 |
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

已补三种模式的相机订阅行、RGB 的 OutMono8、拼接和过滤依赖行。依据原厂截图校正文案、方案存图 120 张提示、曝光/增益等订阅文本框、数值和文本订阅图标、相机图标、文件夹文本长度 200；本轮另补本地目录选择按钮与校验，详见下节。图标来自原厂提取资源，来源哈希已记录。缺少服务的控件保持禁用；实际订阅弹层、远程目录选择和相机枚举尚未接入，不能宣称外观和操作完整等价。

末张停止、目录订阅、RGB 输出、相机/SDK、内嵌图像、拼接触发清空、字符过滤仍明确报告缺口；本地 MONO8 基础拼接已实现。本轮插件独立加入 LocalImageDecoder，支持 BMP/PNG/TIFF 到 MONO8；未修改共享 VisionIO/BmpReader。JPEG 已接入独立构建的同版本后端；小尺寸彩色转灰度仍有已知差异。已移除永久 NoMatch 和逐次扫描 BMP 的错误替代；有效语料对照不代表通用解码等价。

构建：`dotnet build tests/ImageSource.PluginChecks --artifacts-path analysis/build/image-source --disable-build-servers`，运行产物 ImageSource.PluginChecks.exe。362 项通过，0 警告/0 错误。新增 13 个解码语料的逐文件身份/像素或明确缺口验证，以及原厂目录校验助手和本地目录行为对照；保留此前的本地状态、并发/取消、持久化宿主运行、三模式显隐、样本像素和 11 步拼接检查。目录展开 QA 截图已检查。截图/原厂探针均在 tests，框架源文件未修改。

## 图像解码：彩色、调色板、多格式与异常语料

源码关联：原始 CMyAlgorithmManager::Process、ConvertFileImage（0x18001c8d0，native-4.c），以及 MVDMediaProcess.dll 的文件解码入口 FUN_180383ef0。入口读文件并分派解码，再由图像层提供像素；本轮未反编译出最终灰度内核系数，不能把观测公式称作源码确认。

原厂记录为本工作树 `analysis/full-audit/image-source-runtime/run-4f8687bcc001476bbd34f5409be3345a/probe.json`；13 个输入语料的 SHA256、输出字节/哈希、原生错误码，以及额外 JPEG RGB24 参考见 [image-source-codec-evidence.json](image-source-codec-evidence.json)。用 `Invoke-OriginalImageSourceProbe.ps1 -ProbeCodecs` 和 `Export-ImageSourceCodecEvidence.py --runtime PATH` 重放。输入生成代码共享给原厂探针与重建测试，仅生成语料，不含解码或预期结果算法。

| 语料 | 原厂观测与本轮处理 |
|---|---|
| 17×19 彩色 24 位 BMP、顶向下 BMP、含变化 alpha 的 32 位 BMP | 行方向、非 4 字节对齐宽度、alpha 忽略；重建逐像素哈希一致 |
| 灰度、反向灰度、彩色 8 位调色板 BMP | 实际应用调色板；重建逐像素哈希一致 |
| 彩色 PNG、TIFF | 解码后 MONO8 哈希与原厂一致 |
| BMP 改名 .png、PNG 改名 .bmp | 原厂按内容识别；重建同样按内容解码并对真实 BMP 做完整性检查 |
| JPEG | 原厂成功；WIC 解码后有 24/323 像素与原厂不同，最大差 1。原厂 JPEG RGB24 再按观测灰度规则转换与原厂 MONO8 全字节相同，差异已定位在 RGB 解码阶段。本轮已改用独立编译 libjpeg-turbo 2.1.3，首个样本 RGB/MONO8 全字节匹配；扩展语料与已知边界见下节 |
| 截短 BMP | 原厂返回成功，但三个运行输出的缺失像素不稳定；本轮独立实施严格拒绝，返回 ImageReadFailed，不声明该错误行为等价 |
| 非图像 .bmp | 原厂错误 0x80100001；插件返回 ImageReadFailed；数值错误输出统一政策仍待 IS-003 |

彩色语料中原厂纯 R/G/B 输出为 75/149/29，现有共享 BmpReader 的四舍五入公式为 76/150/29。插件使用独立整数规则 `(76R+150G+30B)>>8`；它与本批有效无损语料及原厂 JPEG RGB→MONO8 对照吻合，不代表已证明所有色彩空间、配置文件和位深。WIC 负责容器解码，64M 像素和严格 BMP 完整性检查是独立资源/有效性政策。JPEG 的小尺寸原厂转换错误边界、CMYK、16 位、特殊压缩、ICC、多帧 TIFF 等仍待扩展验证；不得据此标记全解码器等价。

## 本地目录选择与校验

原厂源码：Apps.XmlParser.ParamTab.OpenFolderDialog 的 CurValueA、DoOpenFile、IsCorrectDir、IsHaveRWAccess；路径图标来自原厂 Apps.Style BtnFolderStyle。当前 worktree 的 `run-8fdfdc1bf96c4561b6d877015860d013/probe.json` 直接调用原厂托管助手，确认 6 种路径语法结果和自建目录的写访问探测，记录与源码/资源/原始程序集哈希见 [image-source-folder-evidence.json](image-source-folder-evidence.json)。重放开关为 `-ProbeFolder`。

插件已补本地 FolderBrowserDialog 按钮，选中后去首尾空白；手工输入在失焦时校验。接受本地盘符加反斜杠路径，限制 200 字符，检查磁盘存在、按需创建目录并探测可写性；失败保持旧值。原厂写探测会创建并删除固定 test.txt；插件使用独立随机名和 DeleteOnClose，验证保留已有 test.txt，这是明确的独立差异。相对路径、UNC、正斜杠和中间控制字符的语法行为已有原厂助手对照。

范围限制：尚未操作原厂目录弹窗，不宣称弹窗外观/键盘行为等价。目录监视开关仍禁用，持久化开启的配置可显示选择器。原厂远程分支使用 DirectoryBrowser 与不同校验策略，缺少编辑器宿主上下文；SetParamUnit 报告、PreExecuteRun 自动重跑也需公共服务接入，不能用本地选目录冒充已完成目录订阅。

## 本地 MONO8 拼接：原厂逐帧对照

源码入口仍为原始 `image-source-native-7.c` 的 CMyAlgorithmManager::StitchImageOutput（0x180020980）；SetParam 的 StitchHeight/StitchStartHeight 赋值不清理缓冲区。原厂新探针：`Invoke-OriginalImageSourceProbe.ps1 -ProbeStitch`；有效记录为本工作树 `analysis/full-audit/image-source-runtime/run-70d41c7b16174235ab6bd43c8993aa6f/probe.json`。11 步的原生错误码、SN、尺寸、逐行值、像素哈希及源码/探针哈希见 [image-source-stitch-evidence.json](image-source-stitch-evidence.json)。

语料 A/B 为 4×4、8 位灰度调色板 BMP，每行分别全为 A=[10,20,30,40]、B=[50,60,70,80]，从上到下排列。以下为每行首像素，且探针已确认每行均匀：

| 操作 | 原厂输出行 | 结果 |
|---|---|---|
| 高度 2、起点 1，首帧 A | 0,0,10,20,30,40 | 首帧前缀补零 |
| 下一帧 B | 20,30,50,60,70,80 | 取前帧从起点开始的 2 行 |
| 起点改 0，再读 A | 60,70,10,20,30,40 | 已缓存前缀不受新起点追溯改变 |
| 拼接高度改 1，读 B | 0,50,60,70,80 | 完整输出字节数变化时重新分配并清零 |
| 起点改 4 | 空帧，0xe0000502 | 消耗 A 的列表位置，SN 不增长 |
| 起点恢复 0，读 B | 50,50,60,70,80 | 错误保留前缀 |
| 关闭拼接读 A，再开启读 B | A 原图；50,50,60,70,80 | 关闭不释放历史 |
| 高度改 0，读 A | 10,20,30,40 | 旁路，不更新拼接历史 |
| A 宽度改 5，再恢复 4（手动选 A） | 先 0xe0000502，后 50,10,20,30,40 | 宽度变化失败，恢复后继续 |

插件现以每节点保留字节缓冲区实现这些行为，先保存输出副本，再更新下帧所需的前缀，避免后续运行改写已发布帧；只缓存解码原图。拼接开关已启用。错误映射为插件 `StitchGeometryInvalid`，其原厂数值对应关系保留在证据中；当前共享执行器的失败输出政策仍待 IS-003。

范围限制：仅本地图像 MONO8、ClearTrigger=False；RGB 和相机/SDK 拼接未接入。ClearTrigger=True 且高度非零仍返回 StitchTriggerUnavailable，不能用界面开关代替整数订阅。原生 ClearTriggerValue 非零时会在拼接前清零缓存并清除已记录宽度；实际订阅取得、清除时机和错误需公共服务接入后继续对照。插件 Reset 释放状态是独立生命周期处理，未据此声称原厂 Reset 等价；完整 session 身份仍待 IS-002。

## JPEG 后端与扩展像素证据（2026-10-16）

- 原厂媒体 DLL SHA256 `eea3f2310ead31fbfebe0dc187f270b11cf686344275d3ddcaeb95e2051868a7` 在文件偏移 `0x3bf208` 包含 libjpeg-turbo 2.1.3/build 20220324 标识。独立 Ghidra CLI 分析 `0x18038c8a0`、`0x1802b5590`、`0x1802b5980`，结合上游源码确认 RGB 格式 0、pitch 0、flags 0；warning 0 可接受、fatal 1 报错。源码、原始运行、编译产物及输入/输出哈希见 [image-source-jpeg-evidence.json](image-source-jpeg-evidence.json)，不将 CLI 结果冒称 REA MCP 证据。
- 实现使用官方 2.1.3 未修改源码的独立 Windows x64 标量构建，未复制原厂媒体 DLL。本地构建脚本、许可证、来源与部署说明随插件提交；无需新增 NuGet 依赖。SIMD 性能、其他平台及架构未对齐。
- 27 个自有 JPEG 语料经过原厂 SDK 实际运行：26 个有效 RGB 输出逐字节匹配；原厂成功的 13 个 MONO8 输出逐字节匹配。涵盖 4:4:4/4:2:2/4:2:0、质量变化、渐进式、重启标记、灰度、EXIF Orientation=6 不旋转、缺 EOI 警告接受、非法头拒绝。旧样本 WIC 24/323 个 MONO8 像素差异现为 0。
- 已知差异：13 个小尺寸彩色语料原厂 RGB 成功、MONO8 返回 `0x801000ff`，重建仍能输出灰度。1x1 灰度原厂成功；3x3 彩色失败而 4x4 成功，但未把这些观测硬编码为通用最小尺寸规则。原厂转换调用已追到 `0x18038b190 -> 0x180389440 -> 0x1800031b0`，底层内核/失败条件尚未恢复。测试将差异单独写入 `analysis/full-audit/image-source-checks/jpeg-known-boundaries.json`，不计作匹配。
- 原厂重放：`tests/NativeEvidence/Invoke-OriginalImageSourceProbe.ps1 -ProbeJpeg`；紧凑证据导出：`tests/Export-ImageSourceJpegEvidence.py --runtime PATH`。普通重建检查使用已跟踪输入与原厂预期值，无需原厂安装。12 路并发解码检查句柄隔离。CMYK/YCCK、ICC、更多损坏数据及性能仍未验证，不能据此宣称完整等价。

## 当前验收清单

- [x] 本地关键状态源码映射、4 个辅助函数反编译和原厂 SDK 操作对照。
- [x] 三种模式声明/托管规则；未绑定相机、SDK 未提交和已提交的实际运行结果。
- [x] 本地/SDK/相机执行、像素和失败边界的 767 项插件检查。
- [x] 原厂 SDK 宿主三模式与展开状态截图、25 项实时参数模型及显隐对照。
- [ ] 本地/相机/SDK 完整桌面截图及操作序列，含下部区域、OutMono8 联动和订阅弹层。
- [x] 彩色/调色板/BMP 行布局、PNG/TIFF、改名容器、JPEG 和损坏文件语料；已识别转换差异并实现无损语料对齐。
- [x] 本地目录选择、路径校验和原厂助手运行对照。
- [x] 原生现代 SIMD 转换源码、35 个 JPEG 边界、26 个扩展位深/压缩/首帧语料。ICC 等未穷尽输入与其他 CPU 分支列为覆盖限制。
- [x] 目录自然排序、已测创建/删除/修改/改名事件、RefreshImage/TriggerProcessRun、释放和重载；未宣称事件风暴时序等价。
- [x] 本地 MONO8 基础拼接及 11 步原厂像素/错误/SN 对照。
- [x] RGB/SDK 提交、拼接与触发清空、字符过滤、预览文字、内嵌图像、顶层连续流程末帧停止。
- [x] IS-001/002/003/004 必要接口由插件线在本 worktree 落地并回归。
- [x] MV-CS050-10GC 真实取帧、曝光/增益写入与读回、负参数拒绝后继续、超时/恢复、失败计数与相机拼接部分输出。
- [x] 目录模式内嵌保存、首次运行前保存和空目录重载行为；不回退到内嵌图像。
- [ ] 原厂桌面相机选择/订阅/目录弹层及完整操作序列；SDK 宿主窗口不能代替此项。
- [ ] 远程目录分支需要原厂远程环境和协议证据；当前产品仅接入本地选择。

## 2026-10-16 续批：小尺寸条件、格式边界、目录与触发清空
此前 JPEG 小尺寸与触发清空缺口由本节替代；P0 尚在继续，不标记完整等价。
- 源码 0x18000c4d0 的命令 4 要求宽高>3，否则返回 0x10000008，SDK 最终为 0x801000ff；灰度 JPEG 旁路。0x180142980 确认现代 SIMD 系数 76/150/30 和右移8；标量浮点分支的跨CPU行为尚未对齐。35 个原厂 JPEG 语料：34 个有效 RGB、17 个成功 MONO8 全字节匹配，17 个小尺寸转换失败与非法头拒绝均对齐。源码与运行见更新后的 image-source-jpeg-evidence.json。
- 26 个扩展格式语料见 image-source-format-evidence.json：Gray16 PNG/TIFF 取高字节，彩色小尺寸限制扩展到 BMP/PNG/TIFF，补齐低位深拒绝、alpha、TIFF 首帧/LZW、8位调色板对照。packed palette PNG 明确返回 ImageCodecUnavailable：原厂 0x18038cc20 按逐字节读取未解包数据，输出受行外字节影响；同一文件原厂 MONO 与 RGB 再转 MONO 也不一致。未模拟越过行边界读取。
- 目录比较器源码确认 StrCmpLogicalW<0；121 对原厂调用与目录快照确认自然排序、小写扩展名 jpg/png/bmp/tif、非递归。见 image-source-catalog-evidence.json。LocalFolderCatalog 是会话监视的快照组件；事件与释放仍待接入，等值项顺序与原厂 ANSI 路径限制不宣称一致。
- 触发清空通过现有 ClearTriggerValue 可选整数端口实现：缺失不清空，任意非零每帧清空，先清宽度再检查几何。8 步原厂像素/错误对照见 image-source-clear-evidence.json；UI开关已启用，订阅选择器继续接入。
- 本批 611 项检查通过，构建零警告、零错误。用户已将必要框架调整交由插件线，接下来在当前 worktree 完成公共接口与插件接入，不改另一工作目录。

## 2026-10-16 公共接口与运行集成续批

- 已实现 RGB 图像契约、真实格式预览、RGB 拼接/额外 MONO8、SDK 提交一次消费、字符串过滤及控制后继抑制、连续运行末帧停止、会话状态和目录监听释放。新增原厂运行与像素证据见 rgb/control/watch 四个独立 ledger；与原厂对照和独立宿主测试分开记录。
- 已实现类型订阅按钮、相机目录服务和可选已安装 MVS SDK 适配。真实 SDK 枚举成功，记录为 0 台设备；用户正在连接相机，准备好后通知。真实取帧、曝光/增益、计数与硬件像素验证待完成，模拟提供器通过不代表相机等价。
- 已实现 VMX 编码图像资源保存/重载、SHA256 校验、删除源文件后执行和预览、120 图上限与关闭后清理。原厂 SaveAs/Load 删除源图像后逐像素对照见 embed ledger。VMX 不复刻原厂 SOL 格式和恢复文件名前缀；目录模式内嵌、远程目录与原厂桌面完整弹层仍待验收。
- 图像源检查 750 项通过，产品编辑器构建零警告/错误；共享回归 209 项通过。全 solution 构建的旧 VmRouting.Checks 依赖当前 worktree 缺失的反编译文件。WPF 实际窗口 242 项检查通过，已按原厂 SN/AutoStop 语义修正旧断言；不代表原厂桌面完整等价。
- 运行库生命周期：MVS 原生库跨方案保留加载，设备句柄按会话释放；之前会话替换后原生访问冲突在该调整后的回归中未复现。释放取消回调期间完成的执行租约会延迟到 Cancel 返回后再释放服务和 token。

## 2026-10-16 实机、非桌面剩余项与最终回归

本节替代上一批“0 台相机、目录内嵌待接入”的状态。设备为 MV-CS050-10GC，序列号 DA5108130，软件触发模式为 TriggerMode=1 / TriggerSource=7。

- 原厂与重建均取得 2448×2048 RGB24。8 个原厂有效 RGB 帧和各自额外 MONO8 共 40,108,032 个像素全部满足 `(76R+150G+30B)>>8`。不同实时曝光的传感器帧不相同，未拿跨进程照片哈希相等作为结论。
- 曝光请求 1000.1234 / 5000.5678，设备读回 1000 / 5001；增益原厂读回 1.221 / 3.446，与重建原始读回 1.2206 / 3.4464 在原厂三位小数精度一致。负参数被拒绝时保留原值并继续出图。参数输入先按 float32、再三位小数写入；曝光与增益的等待槽均执行。
- 无外部触发时，原厂结果 ErrorCode 仍为 0，但无图、ModuStatus=0、失败计数增至 1；恢复软件触发后出图并保留失败计数。相机 SN 在拼接前递增；几何错误 0xe0000502 保留先前产生的 SN/帧号/失败计数。重建分别保留执行失败状态和原厂数值，避免把 ErrorCode=0 当作成功。
- 已修正软件触发缺失、相机 SN 缺失、失败时部分输出丢失、额外 MONO 文件解码边界；已确认的小尺寸/位深/缺失文件错误有数值诊断。CameraIdentity 是独立宿主的持久序列号绑定，防止重新枚举误绑；不冒充原厂全局相机模块序列化。
- 目录保存从当前会话快照取图；首次运行前从配置目录取图。空配置不扫描工作目录。原厂订阅目录重载后重新扫描；即使存过图，目录为空仍为 0xe0000203。VMX 实现保持同一优先级，资源编码和原厂 SOL 文件名前缀的差异继续明示。
- 原厂窗口类和曝光订阅窗的离屏探针另存于 `image-source-window-host-evidence.json`。窗口仍使用 SDK 后端，尺寸和图片列表有上下文差异，订阅树为空；它不是实际桌面验收证据。
- 最终构建：`dotnet build VisionReplica.slnx --artifacts-path analysis/build/image-source --disable-build-servers`，零警告/错误。已把插件检查及流程数据/调用检查纳入 solution。767 插件检查、242 实际 WPF 检查、460 项其他回归通过；详细命令、二进制与源码哈希见 `image-source-verification.json`。
- 原厂探针仅向新建输出目录 SaveAs，未保存安装样例。实机验证结束恢复曝光 20、增益 0、TriggerMode=1、TriggerSource=7。原厂/独立探针和载入二进制哈希随 camera ledger 保存。

### 覆盖限制和需要协助的桌面证据

- 当前硬件证据限这一型号、软件触发及无触发超时。非零物理丢帧/丢包的适配计数规则、其他型号和备用参数名没有正向丢失运行对照；不以零计数或模拟提供器宣称全部相机等价。
- packed-palette PNG 的原厂行外读取和截断 BMP 的不稳定像素不模拟；跨 CPU 标量路径、ICC 等其他语料不作全量等价保证。
- 需要用户协助采集实际 VisionMaster 桌面：三种模式的完整窗口与滚动到底状态；相机列表/绑定；曝光、增益、清空、字符过滤和整数参数订阅面板；本地目录选择弹窗。保留完整窗口、缩放比例和打开顺序，无需保存或覆盖原方案。截图或短录屏可用于补齐 UI 差异，不能替代源码规格。
- 若要验收远程目录，另需可用原厂远程环境及连接方式；未凭空实现协议。收到这些证据前保持 P0 进行中，不推进到下一工具。
