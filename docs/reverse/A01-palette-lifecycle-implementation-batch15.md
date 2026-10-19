# 第十五批 A01.2：main工具箱首次装载、共用入口与刷新失败状态

本批实现具名A01.2在 main 已有元数据palette中的全部首次/悬停/展开/画布菜单/显式刷新及失败状态链。没有加载、执行任何厂商DLL/SDK，也没有另造工具参数配置框架或修改tool-plugins。原厂DLL发现到真实registry的映射仍属于A07；原Group导入回调到模型属于A01.3，不以本批完成它们。

## 原始依据

按 REA 技能复用已认证原程序集及源码，不重跑原方法。`VMControls.Data.dll` SHA256 `A0321EB4669C5114C791B32AF8D0CE5AC17A690210BC87319908658F7457A9F8`，MVID `8e3bf4de-1ba9-4d75-adc1-10ec9b2523f8`；身份 Evidence `ev_bf0242bcea12b015c7793a0fae62b7d2ef8114378a17786e8fb5ac39435231e7`，成员/CIL Evidence `ev_0dd9b2693de8c6d62eba0086d85c7cacb827c47fdf8e5f9d58ca3e99db6c2377`，原方法受控证据见 `A01-loader-batch1.md`。

`analysis/decompiled/VMControls.Data/VMControls.Data/ToolBoxData.cs`：UpdateLoad先置IsInit再枚举/解析；完整成功后才Sort；异常前缀保留，后续同身份入口不自动重试。Reload/Refresh清cache，新身份才重新装载。`ToolBox.xaml.cs`悬停、Expander.Expanded和`DiagramPage.xaml`菜单MouseEnter都走同一UpdateLoad，不能每入口建立另一缓存或提前全量构建。

## 实施

- `ToolNavigation.ReadToolGroupHeaders` 首次只读取当前本地JSON类别头；不反序列化条目或构建tile。
- `ToolCategoryLoadState<T>` 被真实主窗口消费；首次尝试前标记initialized，逐条保留成功项，记录完整Failure。成功后按Priority排序；失败保留原输入顺序。后续入口不重读、不自动重试。
- 折叠悬停、Expanded和画布右键类别共用 `LoadToolCategory`。Expanded前不创建grid；失败首触发向状态栏报错并终止本次打开，后续同身份可呈现原成功前缀。
- 分类右键和画布菜单提供明确“刷新工具目录”：关闭旧popup，清头与加载状态，重建类别。失败修复后只有此操作才产生新尝试。
- 工具身份仍由既有curated JSON的ModuleId消费当前 `modules.Descriptors`，未注册条目禁用；不将元数据装载冒充DLL/算法实例化。

`ToolDirectoryCatalog` 的既有宽容inventory合同保持不变（最终diff为空），它不被冒充原严格UpdateLoad。原UserTools/GroupTools在LoadData先读取首项、首次UpdateLoad再读全量的native/目录适配，与当前本地JSON空自定义类别不是相同载体；不把源安装目录、Group导入或动态DLL宿主接入混入本单元。全局三项palette已有独立allowlist路径，保持该入口不做普通惰性加载。

## 自审核范围

`tests/ToolCatalogLifecycleChecks15` 在实际MainWindow上使用自有synthetic JSON和路由控件事件：首开无状态/无tile、多类别隔离、首次第三条失败前缀、sticky及修复不自动重试、展开和真实CanvasSelectionMenu共用状态、失败不排序而成功按Priority排序、未注册禁用、显式刷新新身份、重复展开不读文件、loaded缓存可承受源消失、首次文件不存在后sticky。测试不是原厂UI/用户桌面差分；根线统一构建运行后保留实际断言与失败日志。本线未执行构建，不提前声明通过。

原始错误保留而不加retry；不以本次专项替代已有长稳失败或原机差分，也不重计A01.1已完成单元。

根线实际结果：ToolCatalogLifecycleChecks15 exit0、17/17通过（实际数量以日志为准）；成功排序与失败不重排已覆盖。WorkflowGestures真实CanvasSelectionMenu已接共用入口。该通过只关闭main实施，不声称原DLL加载/原机UI差分通过。
