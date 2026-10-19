# 第十六批 A07.1：公开插件合同的显式托管装载工具

本批新增非空可执行装载与消费链，A07.1仍为partial：共享启动注册入口和tool-catalog按当前文件锁未接入；不会把本工具冒充已完成宿主动态发现。没有改变插件公共合同、配置框架或tool-plugins，没有执行任何安装厂商DLL/SDK。

## 原源码边界

`analysis/decompiled/VMControls.Data/VMControls.Data/ToolBoxData.cs` 的 AddToolItem只读取ToolItemInfo.xml并刷新分组；实际程序集/type装载在 `ToolBoxItemData.AddNewItem`：customInstanceFullName优先，否则按Info查找/加载 `${Info}Cs.dll`，可选Librarys.Config/Control.dll，查首个IVmModule类型并以param构造。`VM.Core/IMVSGroup.LoadAssemblies` 和 `VmModuleSet` 装载链也按模块名、模块目录及已加载程序集身份寻找Cs/Control。

该证据确认“目录元数据→选定程序集→合同类型→实例”层次，**不提供当前独立IVisionToolPlugin的原厂manifest**。因此本工具不发明新的磁盘manifest，也不把IVmModule构造参数翻译成插件参数页。原厂SDK、Librarys.Config、Control.dll、Group初始化仍未执行。

## 新交付

`ManagedToolPluginReference` 是调用参数，包含绝对程序集路径、精确类型全名、SHA256和期望模块ID/版本；不是新人工审批流程或新配置文件格式。`ManagedToolPluginLoader.Load`：

1. 读选定文件字节并认证SHA；只读PE metadata核验精确public/concrete/non-generic类型直接实现当前 `VisionPluginContracts.IVisionToolPlugin`，接口引用名称、版本、文化和token相符。
2. Default ALC已有同名程序集时要求相同文件哈希；否则仅显式LoadFromAssemblyPath。无自动目录扫描、无自设DLL解析hook、无邻接Librarys.Config执行。加载后核对MVID/哈希及真实接口assignability，要求public无参构造。
3. 实例化后核对Manifest/Descriptor身份；使用私有ModuleRegistry复用完整descriptor校验，返回实例与证据身份。真实registry由唯一宿主调用者注册，不在失败中改变它。

这是当前公开独立合同的实现；父类间接继承接口、泛型/嵌套非公开类、原IVmModule和动态依赖宿主不在支持域。代码以宿主权限执行，metadata/hash/type检查不是沙箱；实际运行仅允许本项目自有fixture。原厂装载及算法等价没有因此获得证明。

## 复验交付

`tests/ManagedToolFixture16` 提供独立自有程序集：一个可执行整数输出工具、非插件类型及故意抛错构造器，没有厂商代码或设备访问。测试项目不静态引用该fixture，构建后只复制到独占fixture子目录。

`tests/ManagedToolPluginChecks16` 含17项自审：错误hash、错误type/合同和缺失type在代码加载前被拒绝；正确模块通过既有ToolPluginRegistry/ModuleRegistry注册，GraphEditorDocument创建节点后由真实GraphExecutor运行输出42；方案保存重开保留模块身份；公开editor工厂可消费；身份/构造失败不修改实registry；重复显式装载复用相同程序集；原fixture字节不变。所有样本与异常为自有synthetic，构建运行由根线串行完成，实际结果写本批独占审计目录。

## 尚未完成

A07.1需要后续共享入口的提交级合流：真实目录元数据如何提供本地明确类型/模块身份、宿主启动/刷新唯一注册入口、工具箱映射与已有方案缺依赖行为。根线按边界记录FCR；本批不私自改MainWindow或tool-catalog，也不以测试中真实registry消费冒充主窗口已经自动接入。重复/版本冲突属于A07.2，与现公开Register可能在模块注册异常后残留plugin项的风险另列，不能偷偷复制另一套注册框架。

根线实际结果：17/17自有fixture消费检查exit0，真实runtime节点与存读/editor factory通过。WPF依赖构建既有CS8603保留。未接startup/目录注册锁文件，A07.1仍partial，不以此helper计完整宿主发现。
