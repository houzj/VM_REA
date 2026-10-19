# 第十八批 A：未重复设计器阻塞，落地托管目录引用的真实宿主消费

A06.1仍依赖实际设计面、XAML快照、三缓冲MD5及事件清理。当前没有新增设计面交付，因此不重复第十三批原厂探针、报告或用空窗口骗完成；根线明确将本线切换到A07.1的真实发现/注册消费者。

## 原源码和独立适配合同

`ToolBoxItemData.AddNewItem` 的原链为目录项Info/DirPath→`${Info}Cs.dll`或customInstanceFullName→已装载程序集/目标类型→构造；Control.dll/Librarys.Config及IVmModule的SDK参数不是当前公开插件合同。本批复用第十六批已认证原源码与静态方法依据，不执行任何原厂DLL/SDK。

现main继续使用既有tool-catalog元数据和 `IVisionToolPlugin`、`ToolPluginRegistry`、`ModuleRegistry`。根线在 `ToolNavigation` 单一写入可选 `Implementation` 引用，负载为现有 `ManagedToolPluginReference`：显式绝对程序集路径、精确类型名、SHA256、模块ID、版本。没有自动扫描或探测旁边DLL，不猜原厂目录、不新建参数配置框架或另一种manifest。旧条目缺Implementation时保持现有静态注册行为。

启动文件MainWindow和公开插件/注册工厂合同本身不改。主框架目录发现/加载的唯一写入者由本批根线负责；原协调锁的适用范围和新接缝须由根线记录FCR。本线仅新增session与自有检查，未触碰tool-plugins。

## 新实施

`ManagedToolCatalogLoadSession.EnsureLoaded(reference,register)` 为真实分类首次装载提供完整生命周期：

- 缺记录时先发布loading状态，再调用既有ManagedToolPluginLoader；loader保持metadata/hash/精确类型/宿主接口/Manifest/Descriptor核验。
- 成功后进入register阶段，调用根线唯一真实注册回调；只有回调成功才发布loaded。相同引用后续分类/刷新消费同一实例，不重复注册。
- 原异常以 `ManagedToolCatalogLoadException.InnerException` 保留，阶段明确为load/register；同引用失败保持sticky，无自动retry。
- 显式分类刷新只 `ClearFailures`，不清成功插件身份。加载或注册正在进行时拒绝重入，不产生第二份插件配置系统。

本session是当前主机UI线程同步消费者；未声明跨线程并发保证或通用恶意程序集隔离。注册回调仍需根线在两个现有registry预查ID/版本冲突；不改变旧public Register非事务边界。程序以宿主权限执行，此次所有运行对象仅为项目自有fixture。

## 实际消费自审核

`tests/ManagedToolCatalogHostChecks18` 在创建MainWindow前设置 `VisionEditor.DisableAutomaticCameraDiscovery=true`；使用自有tool-catalog JSON与复用的 `ManagedToolFixture16`（非静态程序集引用）。15项检查覆盖：

1. 首次header discovery不装载/注册；错误hash分类失败不污染host。
2. 真实分类Expanded触发选择程序集→ToolPluginRegistry/ModuleRegistry注册，实际palette tile变enabled。
3. 实际CanvasSelectionMenu类别复用映射，点击原host菜单路径创建真实节点；GraphExecutor执行输出42。
4. 正常host保存/重开保留节点身份；修复失败元数据不自动retry，显式刷新允许消费成功身份且不重复注册。
5. session单独验证register阶段原异常保全、sticky、不retry、ClearFailures后一次成功、成功缓存不重复callback。

本线未并行构建或运行WPF；根线串行运行最终 **exit0，15/15**。结果目录：`analysis/full-audit/batch18-20261019/A/host-catalog-e6b9d5c328cc4b4f87fa9f556ccde991`。自有fixture运行SHA256为 `2f0b4545fb95106f284f4159e23d39e1906bc20759833cfd69c1fae047e997cd`。首轮测试的MouseEventArgs命名歧义由根线显式全名修复，初始编译失败保留；不把该轮称为通过。

实际运行之后只读复审 `ToolNavigation` 与session：模块ID与Implementation.ModuleId先对齐；首次分类通过唯一回调写现有双registry；回调对任何已注册同ID拒绝替换（包括builtin或不同版本），成功的相同引用靠session缓存复用，不重复写registry；Refresh只移失败。该明确支持域内没有阻断动态发现→注册→真实创建执行的剩余缺口，可将A07.1的**当前main公开合同实施**判为done。这里的发现载体为main现有tool-catalog显式引用，非原安装Cs自动扫描；不能据此宣称原厂ABI完成。

## 仍独立开放

原Cs/Control程序集ABI、Librarys.Config、原IVmModule构造/native初始化和全部插件覆盖继续未知，不能以当前公开合同fixture外推原厂兼容。A06.1依旧未实施，A07.2全部版本冲突/缺插件恢复矩阵单列。A07.1源码和原厂验证层仍按实际证据边界保留，不因本地实施done自动改成原厂一致；总进度更新由根线统一完成。
