# 旋转标定源码级逆向审计

日期：2026-10-19。本文是旋转标定专属的增量审计，结合原厂反编译 C++、随程序发布的 XML、既有受控原厂导出证据和当前插件实现。源码结论、独立实现、受控原厂运行、原厂桌面运行分别标注；离线探针不等于完整产品运行。

## 目标与一手证据

- 原厂模块：`D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Calibration\RotateCalibModu\RotateCalibModu.dll`，SHA-256 `b08486b8481ed6d7774634b30b5cae167f85b911ebf394122663bab400a9bb41`。
- 原厂 UI 定义：`RotateCalibModuAlgorithmTab.xml`，资源 SHA-256 `a6d2e3c283597ac044ec49d572a50c743f5221e0b858488f2a14d559e432e73e`（以 `analysis/full-audit/plugin-coverage.json` 中该资源记录为准）；原厂显示对象定义另见 `docs/plugins/rotate-calibration-display-contract.json`。
- 当前 worktree 中 Ghidra 输出位于 `analysis/full-audit/rotate-calibration/native/`。主链函数：构造 `1800372c0.c`、`Init` `1800374e0.c`、`Process` `1800376b0.c`、`GetParam` `180038680.c`、`SetParam` `180038980.c`、`ResetMember` `180038bb0.c`、`DeInitVecData` `180038cc0.c`、`CalibRot` `1800392e0.c`、`LeastSquareAXB` `1800390e0.c`、`MatrixInv2x2` `180038f60.c`、`LoadFile` `18003a670.c`、`LoadCalibFile` `18003a340.c`、`SaveModuleData` `18003b590.c`、`LoadModuleData` `18003b3f0.c`。各导出反编译文件 SHA-256 与字节数记录在 `docs/plugins/rotate-calibration-evidence.json` 的 `sourceFunctions`。
- 原厂算法导出探针及受控宿主状态探针分别为 `tests/NativeEvidence/OriginalRotateCalibrationProbe.cpp` 和 `OriginalRotateCalibrationStateProbe.cpp`。它们在自有进程创建模块实例；状态探针仅将自身进程内的 `ModuleFrame.dll` I/O 导入替换为合成回调，不执行 Platform Init、不触碰设备或改变原厂模块代码。结果保存在 `docs/plugins/rotate-calibration-evidence.json`。

## 源码确认的行为

### 配置模型及连接点

原厂 AlgorithmTab 有 `Tab_Basic Params` 和 `ResultShow` 两页。基本页依次链接 `ImageSourceGroup`，再提供“Fit Point”和“Tab_Align File”类别；结果页链接 `TextDisplayGroup`；底部左侧是 `Button_ResetAlgoParams`，底部命令链接 `BottomCommandGroup`。

`InputWay` 是无 `AccessMode` 的 `RadioSelecter`，默认 `0`：模式 0 显示并初始化 `ImagePoints`，模式 1 显示并初始化 `ImagePointXs`、`ImagePointYs`。`ImagePoints` 从前序组合树筛选 `POINT`，写入组合源名 `Image Point`；X/Y 是前序 `float` 过滤器，分别写入 `ImagePointX`、`ImagePointY`。`WorldRotateAngles` 选前序 `float`，`RotNums` 选前序 `int`；`RefreshSignal` 选前序 `int`。`InputWay` 本身不在原生算法 I/O 中读取；它改变可见控件和连接方式。

文件控件声明单选，过滤器 `.xml|*.xml;*.iwcal`。原生 `SetParam("FilePath",...)` 并不按 UI 扩展名枚举：仅检查传入长度末两字节；末尾为小写 `ml` 时走 XML 路径，为小写 `al` 时走 36 字节二进制路径，其余报 `0xe0000204` 且不更新存储路径。`RevOutputFlag` 仅当字符串前四字节精确为 `True` 时置真，否则置假。

### Process 状态机和数据形状

原生 `Process` 先检查输入/输出上下文非空，读取 `RefreshSignal`；该读取失败按 0 处理，非零则使用模块当前保存的 `FilePath` 再次调用 `SetParam`。矩阵有效标志未置位时，清空采样向量/当前样本序号并返回 `0xe0000008`，此路径不写状态结果。

每次运行读取 `RotNum` 的第 0 项并要求 `3..16`；有效值更新总数。之后当前序号加 1，`remainCount=总数-当前序号`；负数时清采样状态并报 `0xe0000001`。X、Y、角度各仅读取第 0 个 `float`。X/Y/角度任一读取失败会写 `ModuStatus=0` 并调用 `DeInitVecData`；该函数清三个采样向量和当前序号，但不清总数及上次剩余计数。成功样本保存图像 X/Y 和 `(序号-1) * 本次 WorldRotateAngle`，角度乘法结果先转 `float`。因此本节点需要逐轮变化的角度输入，不是把数组里的后续元素批量作为一次采样读取。

达到总采样数时，先把当前序号归零，再各运行一次正、反方向 `CalibRot`。`RevOutputFlag=false` 选世界误差较小者；为真选较大者；相等时都选反方向调用的结果。无论是否完成采样，成功路径写 `ModuStatus=1` 和 `remainCount`；采样未完成时，六个标定结果字段写零，完成时写已选解。完成后采样向量已清空，下一次运行从第一点重新累计。

`CalibRot` 使用矩阵左上 2×2 系数，忽略平移和投影项；用前 3 个采样构造固定 4×2 最小二乘系统求中心，全部样本用于累计误差并除以总样本数。角度差先在 float 域乘方向符号，再除以 `180.0`、乘原厂常量 `3.1415926`。2×2 求逆在行列式绝对值不超过 `float(1e-7)` 时保持零矩阵并返回 0。源代码还用该 2×2 逆把世界中心换算回图像中心。此逻辑不等价于通用圆拟合或 N 点求解。

### 文件与方案生命周期

XML loader 读取根下的 `CalibOutputParam`。缺少该节点报 `0xe0000017`；遍历其子节点时，`DataType="FloatList"` 的子文本按原厂 float 查询方式依序写入 9-float 矩阵，忽略 `ParamName`。少于 9 个值会保留此前矩阵尾项；空列表也会把矩阵有效标志置真；多于 9 项会越过原生矩阵存储，属于不安全原生行为，不在独立实现中复现内存破坏。`float`/`CalibPointF` 子项另用于组装 XML 运行参数关联表。文件打开、结构缺失、后缀错误各有不同返回码。

二进制 loader 只接受恰好 36 字节，直接读为 9 个 float；异常长度返回 `0xe0000204`。每次加载先把矩阵有效标志清零，但失败不主动清零矩阵字节。`SaveModuleData` 保存 `Matrix` 的 36 字节和 `ReadMatrix` 的 1 字节有效标志；`LoadModuleData` 仅恢复这两项并设置一次性的“已恢复”守卫，不保存样本、序号或累计进度。恢复后第一次 `SetParam(FilePath)` 只保存路径并清除守卫，不重新加载文件。

`Init` 先创建并销毁 N 点标定 SDK tool instance，再读取 module id，随后走基类/虚方法初始化链。当前受控原厂探针没有调用 `Init`，无设备任务也未尝试实际 SDK 初始化；该生命周期成功/失败行为仍属未验证。

## 与当前插件实现的核对

源码对应的独立实现位于 `RotateCalibrationSolver.cs`、`RotateCalibrationSession.cs`、`RotateCalibrationFile.cs`、`RotateCalibrationPlugin.cs`。当前实现与受控原厂样例覆盖并对齐：基本矩阵运算、正反向选择、数组 POINT 投影为 X/Y、取每个输入第一个值、样本累计及失败清零、刷新/文件错误、XML 与 36 字节二进制装载、有限数值接口无法表示原厂 NaN 时显式返回 `NativeNonFiniteResult` 的边界。实现将 XML 越界写限制在 9 个矩阵元素内，这是内存安全适配，不是原厂行为。

当前保存将矩阵及有效位编码到节点参数 `CachedMatrix` / `CachedMatrixLoaded`，并没有实现原生 `SAVE_MODULE_DATA_PARAM` ABI；它保留矩阵可用性和“采样重启”，但存储承载形式不同。该差异是插件侧独立持久化设计，不能表述为原生 ABI 复刻。

配置 UI 已接公共 `ToolConfigurationModelWindow` 和源 XML 的配置解析。源 XML/触发器、模板与当前产品窗口的 WPF 回归可自动核查；原厂 `ParamsConfigControlBase` 的 BAML schema probe 仍抛 `NotImplementedException`，因此无法据此取得原厂完整面板像素。已有原厂 probe 只验证原厂解析模型状态/特征，不是人工桌面窗口采集。旋转标定的原厂完整控件间距、文件弹层、页签点击、宿主关闭等仍未同状态人工对照。

## 验证状态与边界

2026-10-19 在独立 artifacts 目录执行：

```powershell
dotnet run --project tests/RotateCalibration.Checks/RotateCalibration.Checks.csproj -c Release --artifacts-path analysis/build/rotate-calibration-independent-audit-20261019
```

结果：720 项数值字段检查、总计 1,190 项旋转状态/文件检查、加入 WPF 行为后合计 1,250 项，失败 0。原厂数值导出覆盖 120 组，原厂合成宿主探针覆盖 34 个 Process 状态案例及 8 个文件案例（以 evidence JSON 本身的 `moduleRun` 为准）。这是合成输入下的原厂运行对照和独立实现测试，不是物理设备、真实主流程、真实 SDK Init、像素级原厂桌面对照或人工验收。

仍未知/未验收：

1. `Init` 与真实 MVD SDK 生命周期在当前环境的实际行为；本轮遵照“不做设备测试”。
2. 原厂完整 WPF 配置窗实际视觉和弹层交互；原厂 BAML probe 失败，暂无原厂桌面截图。
3. 真实 VisionMaster 主流程/Group 中的拖放、运行、停止、保存重开及结果呈现；本轮 1,250 项专属检查不替代宿主人工验收。
4. 原生方案存储 ABI 与插件节点 JSON `CachedMatrix` 的格式并不相同，后续跨项目集成需检查持久化兼容，不宣称字节级兼容。

## 增量差异：文件路径扩展名校验大小写

原厂配置路径使用 `Apps.XmlParser.ParamTab.OpenFileForCalibDialog`，反编译来源为 `analysis/full-audit/n-image-calibration/configuration-model/parser/Apps.XmlParser.ParamTab/OpenFileForCalibDialog.cs`。`DoOpenFile` 配置 Win32 `OpenFileDialog` 的 `Filter = FilterName`、`CheckFileExists = false`、`CheckPathExists = true`；选中后依次清空并设置 `CurValueA`。基类 `OpenFileDialog.CurValueA` setter 同步调用 `ValueChangedCmdExecute`。标定路径派生类覆盖该方法：先将 `FilterName` 按 `|`、`;` 拆出扩展名，再使用 `text.EndsWith(pattern.Substring(1))`（无 `StringComparison` 参数，默认区分大小写）；只有通过后才检查目录并调用 `SetParamUnit`、`PreExecuteRun`。AlgorithmTab 资源为 `FilterName=.xml|*.xml;*.iwcal`。因此原厂同一路径验证拒绝大写 `.XML` / `.IWCAL` 后缀，低写 `.xml` / `.iwcal` 才进入参数写入路径。原生 `CAlgorithmModule::SetParam`（`analysis/full-audit/rotate-calibration/native/180038980.c`）也只识别路径末尾精确小写 `ml` 或 `al`，从算法层再次确认大写后缀不会加载。

此前 `CalibrationToolConfigurationProvider` 对所有校准文件路径用 `Path.GetExtension(...).Equals(..., OrdinalIgnoreCase)`。这使旋转标定配置模型接受大写后缀，虽然后续原生匹配算法的 `RotateCalibrationSession.Load` 按 `Ordinal` 拒绝并返回 `0xe0000204`，而 UI 写入静默继续。现改为只对 `vision.rotate-calibration` 使用原厂大小写敏感的 `.xml` / `.iwcal` 尾缀校验；拒绝时发错误通知并阻止参数写入，其他校准工具仍走原路径规则。Win32 选择器的请求仍为单选、`CheckFileExists=false`、`CheckPathExists=true`，与该原厂控件相符。

旋转专属 WPF 检查以同一路径验证：手输 `uppercase.XML`、文件选择 `uppercase.IWCAL` 均保留在输入框中，而配置模型 `FilePath` 不变、错误通知产生；重复失焦不会再次提交同一拒绝值。`lowercase.xml`、`lowercase.iwcal` 可从选择器进入模型。选择器请求为单选、`CheckFileExists=false`、`CheckPathExists=true`，过滤器 `.xml|*.xml;*.iwcal`。最新运行 `dotnet run --project tests/RotateCalibration.Checks/RotateCalibration.Checks.csproj -c Release --artifacts-path analysis/build/rotate-calibration-case-audit-20261019`：数值 `720`、状态/文件 `1190`、WPF 行为后总计 `1256` 项检查通过，失败 `0`。这是插件 UI/状态检查；原厂拒绝结论来自反编译 `ValueChangedCmdExecute` 的同一路径调用链，不是实际打开原厂 Win32 弹窗逐文件操作的录屏。

### 增量差异：拒绝路径的编辑框值与参数值分离

同一原厂配置控件链确认拒绝时的顺序：`Apps.XmlParser.ParamTab.OpenFileForCalibDialog.DoOpenFile` 对话框选择后先执行 `CurValueA=null`、再执行 `CurValueA=选中路径`；基类 `OpenFileDialog.CurValueA` setter 同步调用派生 `ValueChangedCmdExecute`。派生方法在扩展名校验失败分支调用 `HikMessageBox.Show(...)` 并立即 `return`，在该分支之前没有 `SetParamUnit`/`PreExecuteRun`，也没有把 `CurValueA` 设回旧值。因此“编辑框当前文本”保持新路径，“模块参数”保持原值，且失败输入不会写入模块。此结论是反编译调用顺序，不代表已做原厂桌面逐路径录像。

插件侧现在只为旋转标定注册 `RegisterPreservingRejectedText`。它允许选择路径作为候选值送入现有配置状态写入路径；当 `CommitUser` 因 `.EndsWith` 大小写校验失败、模型仍保留旧值时，独立保留候选文本到 `RotateCalibrationPathInput`。错误通知由本插件的通知服务展示；不会触发参数适配器的 `Dispatch`，所以不写 `FilePath`。有效候选清除保留文本并调用原有文件历史记录回调。此实现只在旋转标定路径控件启用，未改变其他插件的路径规则。

原厂错误窗调用补充核查：`HikMessageBox.Show(string, MessageIcons, bool isTranslate=false)` 收到派生控件已经本地化的 `Lang_FileOpenFail`，因此默认不再翻译；STA 调用同步创建窗口并走 `ShowDialog()`，不设置 Owner。非 STA 且 `Application.Current` 存在时，调用经应用 Dispatcher 同步切回 UI，Owner 仅在 `Application.MainWindow.IsActive` 时设置为 MainWindow；非 STA 且没有 Application 时回退到 `MessageBox.Show(message)`。因此正常 WPF 属性失焦/选择回调（STA）为 ownerless 的模态窗口，调用返回前保持阻塞；原厂源码没有在该路径中异步延时或恢复焦点的逻辑。

`HikMessageBox.xaml` 将标题设为空、窗口外尺寸 `290×156`、`Topmost=true`、`ShowInTaskbar=false`；`BaseWindow` 默认样式将 `WindowStyle=None`、`ResizeMode=NoResize`，并定义非客户区模板。内容使用 `#f2f3f5` 背景和 `#8f8b88` 边框，按钮栏仅显示一个 OK（宽 86、高 24、默认按钮）；消息为 12px、灰色、最大宽 254。`Initialize` 对 Error 图标设置 error 资源并显示 GetInfo 按钮；OK 点击设置结果并关闭，标题栏关闭按钮直接 Close。专属实现复刻本任务确认的空标题、尺寸、置顶、非任务栏、不可调整大小、Error 图标、源文本、OK 按钮和 STA 模态/Ownerless 路径。未移植 GetInfo 点击后生成外部日志并打开 Explorer 的辅助动作，也未移植 BaseWindow 主题按钮模板；此边界单独保留，不作完整皮肤一致声明。

用于像素样式的原厂依赖可在反编译材料中定位，但未打包到插件程序集：`VM.Controls.dll` 的 `BaseWindow`、`FouerColorBorder` 与 `SelectAbleTextBlock`，`Styles/ButtonsStyle.xaml` 及其 `ColorThemesResourceDictionary.Instance` / LocalizationResourceDictionary，主题动态资源，以及 error/close 图片。error 和 close PNG 已从提取资源按原字节复制到旋转标定独立资源目录；其哈希与原厂提取物一致。其他主题/基础控件依赖没有复制，因此自定义按钮及标题栏主题的逐像素视觉仍未闭合。之前通过的 1256 项回归发生在该专属模态窗实现之前；包含新弹窗离屏 WPF 断言的回归正在等待 N 图像线完成共享构建后再执行，现阶段不声明本次改动已验证。

| 本次证据/实现 | SHA-256 |
|---|---|
| 原厂 AlgorithmTab 资源 | `a6d2e3c283597ac044ec49d572a50c743f5221e0b858488f2a14d559e432e73e` |
| 反编译 `OpenFileForCalibDialog.cs` | `16b255c28c1cc21e57ffc162e349c5cc77a4b3f0cbf3f206bdedc97e796d8ee4` |
| 反编译基类 `OpenFileDialog.cs` | `129d9185e0fbdccd5146ea57c1ca9033ab1d4d0aaf8eefe29fca2b1921c40bc4` |
| 原生 `180038980.c` (`SetParam`) | `487625befe572f7cbbccd887cae0fc677f0cc4f2a57419958f3c95a06828892c` |
| `CalibrationToolConfigurationProvider.cs` | `a3b028ac7a6d1332cecb6f38c92b8550420466ed72fec47325ebb5497d1ecdde` |
| `ToolConfigurationCalibrationPathControls.cs` | `7a4fcbe556902b0bdefbec01f6d77ddcc24b29233f986079e1dac8493100ccbd` |
| `RotateCalibrationFileErrorDialog.cs` | `77d303f9e1f807c01c2b3e6467c8b46059ee2d8ac740cd5e9087031e5406e668` |
| `RotateCalibration.Checks/UiChecks.cs` | `ceae8e2996a5778e4b8580ffd611dbab63100a37e401125e39da96e521249871` |
| `VisionPlugins.Calibration.csproj` | `fac106f37691f791c025357279d74b8d1be1d7e8710c56f5ae8daa093b7f258a` |
| 原厂 `HikMessageBox.xaml.cs` | `d9551bdef67bf1324ef7d2a726f2afc191ddc2bebfdfb86f8021c42e594319f1` |
| 原厂 `HikMessageBox.xaml` | `85b1d9d829d9c76a3ee06db41eb468f40c4c60c552fb4ff094397ce9ca0a4df3` |
| 原厂 `BaseWindowStyle.xaml` | `46989e854d2dc8f66c9e05b06aad8f48995a0d8f963566371efaab0dd71a1e27` |
| 原厂 `ButtonsStyle.xaml` | `238781dffa6780f1c2209d19bcf283f35882aab95f1b0e98767dd61362ac5b0e` |
| 原厂 / 插件 Error PNG | `b358c47d10d75b7ef58a9e08854c48f404afc869af46048384861851f42d4d3a` |
| 原厂 / 插件 Close normal PNG | `0f4714eab21fed1c6a1c39b8e9842668bab55572101ced871e5c199dc04c739e` |
| 原厂 / 插件 Close click PNG | `24b406738080b8d424fddcb529c140d0dbe32184d762d0aa83b631804d65e037` |

2026-10-19 最终专属回归（包含专属弹窗实现、资源加载与离屏 WPF 模态生命周期断言）执行：

```powershell
dotnet run --project tests/RotateCalibration.Checks/RotateCalibration.Checks.csproj -c Release --artifacts-path analysis/build/rotate-calibration-hik-dialog-20261019
```

退出码 `0`；720 numerical + 1190 state/file + WPF 总计 1259 项，失败 0。离屏窗口通过 DispatcherTimer 自动在模态显示后点击 OK 关闭，未截取弹窗截图。此检查覆盖确认的配置/窗口属性和回调顺序，不验证完整原厂皮肤像素、不触发 GetInfo 外部日志动作、不涉及设备或人工桌面验收。
