# MF-01：运行界面（EditFrontendUI）源码与程序集审计

日期：2026-10-19
状态：入口、窗口托管代码及部分数据/事件调用链已确认；XAML视觉树和完整操作契约未确认，未仿造替代实现。

## 已确认的原始证据

| 证据 | 类型/成员/资源 | 确认内容 |
|---|---|---|
| `analysis/decompiled/VisionMaster.Exe.Audit/menuitems.xml:405-415` | `TitleMenueItemData`，`Lang_FrontEdit` → `EditFrontendUI`；资源 `Resources\ToolBar\ic_topbar_fronted_normal.png` / `_disable.png` | 运行界面工具栏入口通过本地化键 `Lang_FrontEdit` 绑定到 `EditFrontendUI`，并声明正常/禁用图标资源。 |
| `analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster.AppMainWindow/TitleMenueItemData.cs:1545-1558` | `TitleMenueItemData.EditFrontendUI()` | handler `new EditFrontendWindow()`；若 `Application.Current.MainWindow` 非空则设为 `Owner`；随后调用 `BaseWindow.ShowDialog().Value`。反编译片段中的条件体为空；没有看到对话框返回值之后的持久化、状态写回或其他副作用。 |
| `analysis/decompiled/VisionMasterMain/VisionMasterMain.csproj:171-173` | `FrontendUI.Design` 引用 | HintPath 指向 `..\..\..\..\..\Program Files\VisionMaster4.4.0\Applications\myLibs\FrontendUI.Design.dll`。 |
| `D:\Program Files\VisionMaster4.4.0\Applications\myLibs\FrontendUI.Design.dll` | 实际安装托管 PE | REA `inspect_managed_artifact`：managed CIL、.NET Framework 4.6.1、x86 PE/AnyCPU CLI，文件大小 2,980,864 字节，SHA-256 `60c0c7b1f404ad8bab4504b9b928148f84c7694a335331c4e4340b7c8017cdf5`，MVID `0de25f5a-f9cd-4617-9954-2ea69f747be1`。REA evidence ID：`ev_9ad26df612da33ccfe28a3a7cacc5aae75c7951d2d27b97c890ba2e4436261fa`。 |
| 同安装目录 `Development\V4.x\ComControls\Assembly\FrontendUI.Design.dll` | 对照副本 | 与 Applications 副本长度相同，SHA-256 相同；确认两份文件字节一致，不代表源码/符号存在。 |
| 程序集 manifest resource | `FrontendUI.Design.g.resources`（2,000,001 bytes，SHA-256 `ee835f8c94dc9a32a31dcf42c29baec7113b47ae5a34792ddfb939327d807197`）及 `FrontendUI.Design.Properties.Resources.resources`（180 bytes，SHA-256 `e13ed2c59366d0eea74863fd71a81f0cb977cce1edfde304fc538690a4f6ac89`） | 通过 `Assembly.ReflectionOnlyLoadFrom` 读取资源流；`ResourceReader.GetEnumerator().Key` 枚举键，`GetResourceData` 取序列化字节，不访问 `.Value`。已导出 110 个 `.baml` payload；名称、长度、哈希见 `analysis/full-audit/frontend-ui-inspect/resources/baml-payloads.json`。 |
| REA `inspect_managed_members` | 同一应用 DLL，evidence `ev_6192b568ec156c61dd401f8cbf23c3a50baa006193f222f262ff6b28443cb80a` | 该操作没有返回类型/方法/调用边：CLI metadata 被报告为 unavailable，具体 issue 是 `resource-limit`，`metadata.representation` 超过 67,108,864 字节预算（文件偏移 219,932）。因此本次无法确认 `EditFrontendWindow`、`FrontendWindowData.EditFrontendUI` 或任何目标方法是否在该模块中，不能把空结果解读为类型不存在。 |
| REA `inspect_pe_resources` | 同一应用 DLL，evidence `ev_c4a964a4386204a0ed9b3713e23ff6d603e3ad6a4edffc57a35f073fe9589e3a` | Win32 PE resource tree 检查完成：3 个目录项、1 个资源，type ID 16/name ID 1/language ID 0，payload 长度 896、SHA-256 `d0218468635a21989733d049585e0b7924cde43aee37d3981d1a74e8849a945b`。该工具声明除图标组外 payload opaque；此资源不能据此识别为 WPF BAML/XAML。 |

## 定向静态 IL 核查

本机现有 `dotnet-ildasm` 0.12.2 可运行，但要求 .NET Core 3.0；本机只有 8/9/10，因此仅对反编译器进程使用 `DOTNET_ROLL_FORWARD=Major` 后成功解析 DLL。没有加载或执行 VM 程序集。按类名定向导出：

- `analysis/full-audit/frontend-ui-inspect/EditFrontendWindow-simple.il`：`dotnet-ildasm <DLL> --item EditFrontendWindow --output <file>`；73,243 字节。
- `analysis/full-audit/frontend-ui-inspect/FrontendWindowData-simple.il`：同一命令 `--item FrontendWindowData`；134,769 字节。
- `analysis/full-audit/frontend-ui-inspect/FrontendEdit-simple.il`：同一命令 `--item FrontendEdit`；344,557 字节。

由这些目标类 IL 可确认的调用链/状态变化：

1. 主界面路径 `TitleMenueItemData.EditFrontendUI()` 直接 `new EditFrontendWindow()`、设 Owner、模态 `ShowDialog()`；该窗口构造函数调用 `InitializeComponent()`，把单例 `FrontendWindowData.DisplayMode` 设为枚举值 `1`，用 `DesignerContext.DisplayStringA` 调用 `FrontendEdit.InitFrontendWindow(string)`，然后先 `RemoveHandlerVM()` 再 `AddHandlerVM()`。随后把 `VmSolution.Instance.IsRunning` 与 `ContinuousRunEnable` 以 `MultiBinding`、`MulitBoolOrConvert` 绑定到窗口 `IsBusy`。
2. `EditFrontendWindow.OnClosing`：若 `_isCodeCreating` 为 true 则取消关闭；否则调用 `mainEditView.Close(true)`，仅当返回 true 才继续 `BaseWindow.OnClosing` 并解绑本窗口所订阅的 VM 事件；返回 false 时设置 `Cancel=true`。
3. `AddHandlerVM`/`RemoveHandlerVM` 成对订阅/解除 `FrontendWindowData.CreateNewDesignEvent`，以及 `VmSolution` 的 solution load/save begin、progress、end 回调。窗口回调经 WPF `Dispatcher.Invoke` 更新遮罩/进度；Load end 还将 `DocumentManager.Instance.isSolutionDesignerBuffer_Loaded` 置 false，再触发 `FrontendEdit.InitDesignerSurface(...)`。创建设计事件也经 Dispatcher 路由到窗口处理器。
4. `InitializeComponent()` 通过 WPF `Application.LoadComponent` 加载 `/FrontendUI.Design;component/editfrontendwindow.xaml`。生成的 `IComponentConnector.Connect` 映射到字段 `editFrontendWindow`、`img`、`PART_View`（Grid）和 `mainEditView`（`FrontendEdit`）。因此可以确认 XAML 资源路径和几个命名控件，但看不到资源里的实际布局/样式/其余绑定。
5. 插件宿主存在另一条同名但不同入口：反编译 `VMControls.WPF/VMControls.WPF/ToolControl.xaml.cs:1197-1210` 的 `ToolControl.EditFrontendUI()` 加载程序集后反射取得 `FrontendUI.Design.FrontendWindowData`，调用其 public static `EditFrontendUI(parent)`。该静态方法同样新建窗口并设 `DisplayMode=1`，另将 `IsSDKMode=true`；从传入参数取 `Window` owner 或 `IntPtr` owner，设置 `ShowInTaskbar=true`，模态显示后把 `IsSDKMode=false`。不要将此 SDK 工具宿主入口和主窗口菜单入口合并描述。此 IL 未显示 try/finally 包围 `ShowDialog` 与复位。
6. `FrontendEdit` 自身是 WPF `UserControl`，`InitializeComponent()` 装入 `/FrontendUI.Design;component/frontendedit.xaml`。`IComponentConnector.Connect` 可确认 `Grid`、`Border TitleBar`、`ScrollViewer m_HorScroll`、`Menu`、`Slider`、Preview/CreateCode 按钮、`ToolBox`、`ICSharpCode.WpfDesign.Designer.DesignSurface`、`TabControl settingTab` 及两个 PropertyGrid 等控件字段；Connect 还把若干菜单项/按钮 Click 接到明确命名的处理器。其余控件位置、样式、具体菜单文本仍在 XAML/BAML 资源内部。
7. `FrontendEdit.Close(bool)` 先调用 `DoSave(bool)`；原生 DialogResult 数值 3 路径返回 false（上层窗口取消关闭），其它结果再进入清理。已初始化时 Dispose ToolBox、清空选中对象、解除 DesignSurface 选择/网格移动/删除/DesignerLoaded 等事件、解绑并 Dispose PropertyGrid、销毁 UI 设计面、清理两类设计期间字典，最后返回 true。`DoSave` 进一步根据 `IsSubDesign` 和 `DocumentManager.NeedSave()` 分支；需要保存时显示 `HikMessageBox`，确认分支调用 `Save(true,null,false,false)` 并置 `SolutionFileHelper.IsSolutionDesignerModified=true`，取消保存分支调用 `ResetBuffer()`。保存序列化细节另见 `FrontendEdit-simple.il` 中 `Save` 方法，本报告不把该 IL 之外行为扩展成 VM 实机结论。

上述结论由目标 SHA 对应的定向 CIL 导出确认，不等于 C# 原始变量/源码行，也不覆盖动态依赖实现。

## 边界：不可混同 RunningMode

`Lang_FrontEdit → EditFrontendUI → EditFrontendWindow.ShowDialog()` 是单独的运行界面编辑器入口。它不是右上角运行模式文字，也不是设置页的 `RunningMode`/ProcedureRunData 状态链。本次没有将运行策略 0/1 的设置页面当作 `EditFrontendWindow` 来实现。


## `.g.resources` 与 BAML 资源的静态取证（2026-10-19）

使用 Windows PowerShell 5.1 的 `Assembly.ReflectionOnlyLoadFrom` 只读读取已安装程序集的 manifest resource stream；没有普通 `Assembly.Load`、没有访问资源对象 `.Value`、没有加载或实例化目标窗口，也没有运行目标程序集代码。通过 `ResourceReader.GetResourceData` 获取 `ResourceTypeCode.Stream` 序列化数据，校验前 4 字节 little-endian payload 长度与后续字节数相等后，去除该 4 字节 framing，原样保存 BAML payload 并计算 SHA-256。

| `.g.resources` key | payload 长度 | payload SHA-256 | resource data 序列化字节 SHA-256 |
|---|---:|---|---|
| `editfrontendwindow.baml` | 3,466 | `317dcf7ef21751e00ae431952f4faad8dd1054e286bea450ce5f40b1ec4aa028` | `fa787f2cfcf158f493cbc2a106f3e1c76f4a1c9c3d1cf91aeae206ac5df935d7` |
| `frontendedit.baml` | 25,751 | `2afa47cf732663dd44493aae129c210db6d3c8cb0ff5db866958eb691490ab0c` | `1e9c1f7775cf05ba7635a45ec5f9be1b712caae659d97f4fb8923546e7f7350c` |

原始 `.g.resources`、resource data、110 份 BAML payload、逐项清单分别保存在 `analysis/full-audit/frontend-ui-inspect/resources/`；`manifest-resources.json` 记录 manifest resource 及每个资源键的序列化类型/长度/哈希，`baml-payloads.json` 记录每个 BAML payload 的 key、payload 长度和 SHA-256。Windows SDK `ildasm` 定向静态导出本机 .NET Framework 4.8 `PresentationFramework.dll` 的 `System.Windows.Baml2006.Baml2006Reader` 到 `analysis/full-audit/frontend-ui-inspect/Baml2006Reader-net48.il`，仅用于评估 reader，不是目标程序集代码。

已评估通用 WPF BAML reader 的安全边界：其 `Read()` 会逐条处理 BAML 并转发 XAML node reader；但反汇编的 `Process_OptimizedStaticResource` 包含 `StaticExtension` 构造及 `MarkupExtension.ProvideValue(null)` 调用（同文件约 IL_0078-IL_007B）；reader 还有其它静态扩展提供值路径。因此无法保证把未知目标 BAML 输入该 reader 不会执行扩展逻辑。本轮未实例化该 reader、未调用 `XamlReader.Load`，故没有恢复或宣称确认 XAML 控件树。安全下一步是经验证只解码 BAML records、不做 schema/type resolution 或 `ProvideValue` 的自定义静态 parser，或可信的离线解析结果；在此之前不猜布局、模板、绑定和属性。
## 已检查范围与缺口

- 实际安装路径：`D:\Program Files\VisionMaster4.4.0\Applications\myLibs`；目录中存在 DLL，但没有同名 `.pdb` 或其它同名伴随文件。
- 开发副本路径：`D:\Program Files\VisionMaster4.4.0\Development\V4.x\ComControls\Assembly`；同一 DLL 的字节副本，无同名 `.pdb` 或其它同名伴随文件。
- 反编译证据树 `D:\MyWork\REA逆向测试\analysis\decompiled` 和全文审计树 `analysis\full-audit` 中检索 `EditFrontendWindow` / `EditFrontendUI` / `FrontendUI.Design`：只找到主程序入口与 `ToolControl`/`GlobalToolDataItem` 的同名动态编辑路由，未找到 `FrontendUI.Design` 的反编译源文件或 `EditFrontendWindow` 实现。
- `VisionMasterMain.csproj` 的依赖 HintPath 可解析到上述安装 DLL；但源码树中的该默认相对位置没有副本，真实安装 DLL 已定位。
- REA `inspect_managed_artifact` 返回完整托管元数据和 manifest resource 名称，但不反编译 CIL；REA `inspect_managed_members` 因 metadata representation budget 超限不可用；因此使用现存 `dotnet-ildasm` 完成目标类的静态 IL 导出。没有安装工具，没有加载/执行 VM 程序集。
- `ilspycmd` 未在 PATH 检出；Windows SDK `ResGen.exe` 可用，本轮使用自写只读提取脚本直接枚举并保存资源流，没有改写安装 DLL。REA PE resource 检查只覆盖 Win32 资源目录，不解析 .NET `.g.resources` 内部；本轮已只读枚举并抽取 110 个 BAML payload，但未安全解码为节点树。由 Connector 和 CIL 可确认部分字段/事件处理器；窗口/编辑器 XAML 布局、样式、模板、尺寸、完整命令绑定及全部按钮文本仍未确认。

## 结论与验收状态

**源码确认：** 菜单键/主窗口 handler、目标窗口构造/Close 确认逻辑、DisplayMode 状态设置、绑定对象、VM 事件订阅/清理、进度遮罩回调、XAML URI/命名连接点、`FrontendEdit` 的控件字段、部分按钮事件及关闭保存/取消分支，以及独立的插件 `FrontendWindowData.EditFrontendUI` SDK 调用链。
**未确认：** BAML 解码后的视觉树及样式、控件具体布局尺寸和文案、完整绑定/命令/事件路由、配置持久化与运行时消费位置、VM 实机视觉差异。
**实现：** 本次仅新增原始资源清单、BAML payload 静态导出和报告，没有新增 `EditFrontendWindow` 替代 UI。
**操作验证：** 未点击 VM 入口、未打开 VM 对话框、未改变/检查保存数据；静态 IL 分析不代表 UI 操作通过。
**下一步：** 需要经验证只解码 BAML records 的离线 parser（不会触发类型构造、转换器或 MarkupExtension）以及运行界面配置保存/消费链的反编译依赖；取得后继续核查控件、按钮事件和实际数据闭环，再实现和做 VM 实机差分。
