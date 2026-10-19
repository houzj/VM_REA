# 第十五批 B 独立复审：A01.2 加载状态实施关闭

只读复审原 UpdateLoad、main ToolNavigation/ToolCatalogLoadSession 及 ToolCatalogLifecycleChecks15；未修改A文件、构建、执行测试或访问tool-plugins。测试实际通过与否由主智能串行运行记录，本片不预报。

## 固定单元和真实职责

A01.2为“原加载器首次/悬停/刷新及失败分支”。A01.1是目录XML/分类优先级/惰性元数据；A01.3另列原Group导入回调到模型；A07是发现条目到真实registry/缺插件映射。故不能拿本片状态交付声称原Group导入或动态DLL创建已实现，也不应将A01.2本身应完成的首次/失败残留简单转给A07。

原 `analysis/decompiled/VMControls.Data/VMControls.Data/ToolBoxData.cs:77`：排除null、Lang_Tools、IsInit；在枚举前先置IsInit；无占位首项返回；清原Items，逐ToolItemInfo.xml加载，成功后才按Priority排序、替换扁平占位项。目录/XML中途失败会留initialized/成功前缀；没有本地自动重试。Reload/Refresh重建状态身份，且前者清global缓存、后者只ordinary/root。原 ToolBox.xaml.cs.OnMouseEnter、ToolBox Expander.Expanded、DiagramPage.xaml右键MouseEnter共用此状态。

## 本批真实实施

`ToolCategoryLoadState<T>.EnsureLoaded` 在枚举前置初始化/次数；逐项发布前缀；捕获并保留Failure后重新抛。再次调用不重试。ToolNavigation.LoadToolCategory捕获已列文件/JSON/metadata异常，首次失败不打开新内容；下一入口复用前缀；Refresh清分类身份，允许新尝试。

实际主UI BuildRail仅读headers，首次折叠hover、Expanded、画布右键菜单共用同一字典与LoadToolCategory；另一分类独立，显式刷新关闭popup并重建。未注册条目禁用，没有执行未知DLL。UI错误显示及使用JSON导出目录是main独立适配，不是原XML扫描器本身。

新 `tests/ToolCatalogLifecycleChecks15/Program.cs` 实际调用MainWindow并RaiseEvent，覆盖首hover第二项失败保留首项、repair不重试、另一类加载、Expanded/右键共享状态、unknown registry禁用、refresh新身份、重复Expanded不扫描、缓存源丢失仍可用、初次文件读取失败sticky。测试是实际WPF控件事件+合成catalog，非用户桌面或原WPF差分。

## 关闭前必须明确的差异

main ReadCategoryTools逐JSON数组枚举，当前并不按Priority重排。现有 `tests/VisionUi.Checks/Export-VmToolCatalog.ps1:26` 在生成JSON时对Items按Priority排序，组也排序；当前随包导出物满足该输入合同。但任意被修改catalog的数组顺序与Priority矛盾时，不应宣称刷新会按字段重排。若需支持任意catalog，需在全量成功后才排序，不能提前OrderBy破坏失败前缀；本片不代改。

main没有原CollectionViewGroup的初始placeholder/Items为空时不扫描分支，普通组初次只读header、UserTools/GroupTools首锚点由独立metadata scanner表达，不是运行UI隐藏创建原占位项。Global概览是独立可用项固定视图，不能把它说成真实原global缓存Reload差分。上述必须作为独立导出catalog适配说明，不能隐去后声称所有原分支完全等价。

## 自审核意见

若计分含义为在main可用catalog支持域完整交付已证首次/三入口共享/Refresh/失败sticky的状态行为，根线实际测试通过并记录上述输入与适配边界后，A01.2 implementation可关闭；DLLregistry和Group导入继续留在其原有独立单元，不冒充已做。源码已有依据，人工验收后置不阻实施。

若根线仍将该固定criterion解释为完整原目录扫描对象/placeholder/global缓存逐分支复刻，则当前差异未实施，不可凭新UI测试关闭，必须保留partial；不得为了增加分子默默缩小解释。主智能最终判定须在台账明确采用的main实施域和未等价边界。当前复审本身不修改分子，绝不表示A01全包或原厂完全等价。

根线最终复核：已补成功时按Priority排序、失败保持前缀原序，实际MainWindow17项全部通过；ToolDirectoryCatalog最终未更改。固定A01.2按main状态实施关闭，原native/目录载体与A01.3/A07未知边界全部保留。
