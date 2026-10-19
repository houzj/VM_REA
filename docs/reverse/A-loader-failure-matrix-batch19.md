# 第十九批 A07.2：当前托管发现支持域的失败、冲突与刷新矩阵

本批不重计已完成A07.1成功装载路径。范围为main现有tool-catalog显式引用、公开IVisionToolPlugin合同及UI线程注册/刷新，不宣称原Cs/Control/native ABI兼容。仅自有fixture程序集，所有WPF宿主创建前禁用自动相机发现；不访问tool-plugins、不执行原厂DLL或设备。

## 原证据与本地策略

原ToolBoxItemData.AddNewItem按Info/DirPath查找Cs/Control、加载依赖、选择IVmModule类型并构造，异常不在该方法本地重试；ToolBoxData.UpdateLoad先IsInit再枚举，失败保留前缀且后续同身份不重试，刷新替换身份。本地显式引用的SHA/type/version/当前合同校验与拒绝热替换是独立适配合同；原ABI、DLL搜索副作用、fallback及完整SDK生命周期保持未知。

完整闭合门槛：实际MainWindow分类对缺文件、类型、hash、版本、构造器、托管依赖、重复/版本/builtin冲突逐项留下阶段诊断，不能污染真实双registry；失败前缀、sticky、修复后显式刷新以及成功身份保留全部可复验。只测试一次每个独立负例，不自动重试隐藏失败。

## 源码发现的真实事务风险与窄修

旧ToolPluginRegistry.Register先plugins.TryAdd再modules.Register。如果最后模块descriptor校验/冲突失败，会留下plugin-only项；Manifest/Descriptor被多次外部getter读取，还存在身份key漂移。此结论来自旧源码顺序，**没有声称本批运行过旧产品before版本复现**。

根线作为当前main唯一写者窄修既有Register：保留二参入口、增加expected ID/version重载，在任何外部metadata getter之前置UI线程注册准入bool；然后一次capture Manifest/Descriptor，做身份/expected/重复检查，再modules.Register的内部Freeze校验/提交，最后plugins.Add，提交后不再读外部getter。finally清准入，metadata失败或expected拒绝不会永久卡住后续合法注册。ToolNavigation直接把选定引用ID/version传入同一次capture守卫。公开插件配置框架未重写，原合同和二参入口保留；本线未修改该产品文件。

## 交付夹具与真实宿主矩阵

`tests/ManagedToolFailureFixture19` 提供全部自有无设备类型；`ManagedToolDependencyFixture19`为纯托管空基类，故意不部署到测试运行目录，用于真实缺依赖异常。主fixture中的其他类型不引用此基类，不能因缺依赖而凭空把所有选定类型都报失败。

`tests/ManagedToolFailureMatrixChecks19` 首先检查依赖确未部署，然后在真实MainWindow展开各自有分类：

| 独立输入 | 核验 |
|---|---|
| 缺文件、缺精确type、错误hash、错误期望version | load失败阶段保留，真实双registry精确前后不变 |
| 构造抛错、缺托管依赖、private ctor、非合同type | 原异常链保留，不回退到别的类型或导入旁边DLL |
| 同ID不同type、同ID不同version、builtin同ID | register阶段明确拒绝，不替换既有成功插件，不留单边注册 |
| 同类别成功前缀后冲突 | 首次中止展示，再进入只保留原成功前缀，不自动重新注册 |
| 修复先前缺文件引用 | source变好仍sticky，显式Refresh后才消费成功身份；原成功引用不重复注册 |

额外在隔离的真实registry上测试根线窄修：loader验证后descriptor变非法、模块已占位时plugin registry仍为空；volatile Descriptor只读一次；Manifest getter后续漂移也不改变一次capture的key；加载后合法身份改变，被同次Register expected守卫拒绝且两registry均无变更。这些是修后自审核，不伪称旧版本动态差分。

为覆盖已有方案打开的缺插件消费者，固定增加两份自有方案：未知ModuleId，以及已注册ID但不存在的99.0.0版本。实际MainWindow.doc.Load→选中缺失节点→RefreshRight，核验不崩溃、不删除节点或不透明嵌套参数；doc.Validate返回Module错误；真正StartRun在创建activeRun、workflow session或自有runtime工厂之前拒绝。然后SaveAs/Reload再核对身份及payload，源文件哈希保持不变。每份5项、共10项，不增加插件占位后端，也不把缺失模块默认为当前已注册版本。

## 运行与关闭状态

本线未构建。根线首次固定矩阵运行 **exit0、37/37**，summary保存在 `analysis/full-audit/batch19-20261019/A/failure-matrix-e925df5ad2c94ef8921329bf89e99ed3`；追加上述10项已有方案消费者后的固定终版实际 **exit0、47/47**，目录为 `analysis/full-audit/batch19-20261019/A/failure-matrix-7085eedc6d714282a55a7ee294c9a90f`。自有fixture最终SHA256 `a1d460f749797a2c8cc625555620303f0ba1f259533df04135acc530a68a3f5c`。缺依赖实际保留FileNotFoundException链，构造实际保留TargetInvocationException/原InvalidOperationException，不是仅按任意异常冒充具体分支。

根线追加独立metadata重入检查实际 **exit0、5/5**，目录 `analysis/full-audit/batch19-20261019/D/registry-reentry-de27681625c948c083c243606546df46`：getter重入在读取嵌套metadata前拒绝；外层唯一提交；getter异常不留下ghost；失败后准入恢复；expected拒绝后可正常注册。47与5是两份不同运行，不合成一个虚构单次52项运行。

最终只读复审源码、47实际摘要（含unknown/wrong-version方案10项）及重入5项，**A07.2当前main公开合同的实施可判done**。main明确支持域的缺文件/type/hash/version/构造/托管依赖/冲突/类别前缀与刷新、旧方案缺模块/缺版本信息保留及运行拒绝均已具真实消费者。原厂验证层保持partial；不能由实施done推成原Cs或native已兼容。源码、运行DLL、两份摘要、未知模块原/重存方案的哈希另存 `final-source-runtime-sha256.json`。

进程终止、OOM和跨线程并发事务不在该UI线程内存注册合同；没有宣称回滚DLL执行自身任意副作用。原厂插件/native兼容和设备生命周期另列，不因本地失败矩阵通过改成原厂已验。
