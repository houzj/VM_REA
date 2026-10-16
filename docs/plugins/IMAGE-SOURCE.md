# 图像源：首项逆向记录

状态：P0 进行中，尚未完成完整交付。先完成全插件清单，再按本页检查项继续。

## 证据入口

原始证据根目录：`D:/MyWork/REA逆向测试/analysis`。不复制原始反编译项目或完整审计输出进 Git。

| 证据 | 精确入口 | 已知结论 |
|---|---|---|
| full-audit/image-source-audit/ImageSourceModuleAlgorithm.xml | AlgorithmParamList | 算法默认值；与 AlgorithmTab 的部分 UI 默认值不一致，不能直接混为运行默认值 |
| full-audit/image-source-audit/ImageSourceModuleAlgorithmTab.xml | InitialSN, PathCache, AutoPlay, ImageSourceType | SN 范围 0–100000；缓存范围 0–5；关闭自动切换隐藏 AutoStop；本地/相机/SDK=1/2/3 |
| decompiled/ImageSourceModuleControl/ImageSourceModuleControl/LocalImageListView.xaml.cs | CustomTabWidth, cbxMode_SelectionChanged | 中文逻辑宽 420；ExecuteType=1 全部、2 选中 |
| decompiled/ImageSourceModuleControl/Apps.XmlParser.ParamTab/LocalImageList.cs | Init | SDK 模式才强制显示 LocalImageList，不应凭猜测在桌面参数窗口添加 SDK 列表 |
| full-audit/image-source-native-all-methods.json | CMyAlgorithmManager::Process 0x18000d820 | 已有原生入口索引；Process 的状态偏移和分支仍需逐项映射 |
| full-audit/image-source-native-extra-26.c | GetImageFilesPathNameFromDir 0x180022750 | 原生使用 FindFirstFileA 枚举并过滤文件；当前仅 BMP 的托管枚举不是完整等价实现 |
| full-audit/image-source-media-decode-implementation.c | FUN_180383ef0 | 文件打开、格式分派和错误路径；不能以系统解码器替代后声称逐像素相同 |
| full-audit/image-source-audit/audit.json | artifacts, runtimeEvidence, remaining | 二进制身份、UI资源闭包；旧运行图没有已验证操作序列 |

原生目标 ImageSourceModule.dll SHA-256：`91ea3659be97e51edeab0c913779d4555d51dcd0d6288086fbf9916d35b942db`。
保留的 REA 方法索引 Evidence ID：`ev_0130129c0e1136246d8822092b50e26bdca5b56ea744dc930cbe35646c448ca7`。

## 当前独立实现及验证边界

已有本地 BI_RGB BMP → GrayImage 真实数据执行，包含列表、SN、循环、末张停止、可选灰度输出、持久化后宿主执行和 WPF 参数窗口。BmpReader 是独立实现；颜色转灰公式和原生解码是否一致尚未证实。

本轮修正 XML 已确认的数值范围和显隐；选中模式空列表不再退回当前图像，文件夹空列表也不再读取无关当前路径；列表空路径返回明确错误。这些运行防误读改进属于独立实现，尚无原始运行对照。

专用检查：`dotnet run --project tests/ImageSource.PluginChecks`，现有 18 项通过。截图和检查 JSON 输出位于当前 worktree 的 analysis/full-audit；截图代码仅在 tests。18 项检查未覆盖本轮新增全部边界，也不证明原生等价。

## 下一步检查项

- [x] 插件身份、参数/IO/UI 声明与原生方法索引已找到。
- [x] 记录默认值冲突、桌面/SDK 界面差别和公共契约需求。
- [ ] 完整映射 Process 状态：列表、当前项、ExecuteType、SN、AutoStop、错误后状态及 Reset。
- [ ] 跟踪格式识别、转换系数、像素排布、错误码并建立同输入原始输出对照。
- [ ] 目录订阅的顺序、增删、RefreshImage、线程和关闭释放。
- [ ] 缓存、间隔、拼接、字符过滤及触发流程。
- [ ] 相机和 SDK 帧输入、RGB 主输出及 Mono8 附加输出（IS-001/002）。
- [ ] 方案内嵌图像、预览文字和流程停止（IS-003）。
- [ ] 本地/相机/SDK 全模式 UI 与真实操作序列对照；资源存在不等于 UI 对齐。
- [ ] 插件全路径专用验证和框架线集成验收，完成后才标记首项完整交付。
