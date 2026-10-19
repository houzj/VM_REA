# 工具目录元数据发现：原生证据与独立实现

2026-10-19补充：策略改为最后匹配项覆盖，专项11项通过；UserTools/GroupTools原生首次仅首个有效条目，本地全量是独立差异。第二轮闭合悬停/展开/右键共享缓存、扫描前IsInit置位与失败残留、Group完成回调早于托管模型落地，见 [加载第二轮](LOADER-ROUND2-20261019.md)。本地扫描器仍未接入主宿主，不复制原生失败残留或宣称插件生命周期完成。

更新：2026-10-17。范围仅为工具目录元数据，不包含程序集自动加载或主窗口工具箱接入；不能据此声明 NFI-07 完成。

## 原生证据

| 行为 | 原生入口 | 确认程度 |
| --- | --- | --- |
| `AppData.ModulesDir` 一级目录中有 `ToolGroup.xml` 或目录名为 `Global` 才建分类 | `analysis/decompiled/VMControls.Data/VMControls.Data/ToolBoxData.cs` `LoadData` | 托管源码确认 |
| 普通分类先插入代理条目，展开时 `UpdateLoad` 扫描子目录 `ToolItemInfo.xml` 并按 Priority 排序；`UserTools`、`GroupTools` 初次直接读取 | 同上 `LoadData`、`UpdateLoad` | 托管源码确认 |
| 全局工具单独筛选 `ProcedureModule`、`GlobalCameraModule`、`DataQueueModule`；`Reload` 清缓存并通知视图 | 同上 `GetItems`、`Reload` | 托管源码确认 |
| 拖放节点携带 `ModuleDir`、`ModuleTypeName`、`DisplayType`；鼠标按下后首次移动开始拖放 | `analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/ToolItemGrid.cs` | 托管源码确认 |
| Group 的程序集搜索/装载属于专门路径，不能外推成所有工具的加载器 | `analysis/decompiled/VM.Core/VM.Core/IMVSGroup.cs` `LoadAssemblies` | 托管源码确认 |

真实安装样本 `D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Collection/ToolGroup.xml`、`Collection/ImageSourceModule/ToolItemInfo.xml` 已只读核对，包含 `name`、`priority`、`displayType`、`toolTip` 字段。

## 当前独立实现

`src/VisionEditor/ToolDirectoryCatalog.cs` 只解析 XML、分类及优先级策略；普通分类延迟展开，全局工具按原生白名单过滤，损坏 XML 产生诊断且不阻断其他工具。为避免读取外部实体，独立读取器拒绝 DTD。它没有加载 DLL、建立 `ModuleRegistry` 工厂或激活工具。与原生代码的具体差异：当前对同 Priority 使用目录名作稳定次排序；原生比较器相等时返回 0，最终顺序依赖枚举和列表排序。独立扫描器在缺失目录时返回诊断，原生相应异常路径未做桌面对照。

`tests/ToolDirectoryCatalog.Checks` 验证分类延迟加载、策略优先级、全局白名单、无效 XML/DTD 诊断、缺目录、重复展开以及展示类型。受控目录专项结果：9 项检查通过。以原厂已安装目录只读运行：16 分类、`Collection` 展开后 5 工具、初始扫描 0 诊断。此为**元数据读取对照**，不是原厂 UI/拖放/插件生命周期验收。

## 后续合流

插件线提供经过审计的工具身份、版本、资源及工厂清单后，框架线需确定“目录条目 → 已注册工具”的显式映射，并处理缺失、版本冲突、禁用及 Reload。待插件线交付正在修改的共享入口后，再把扫描结果接入 WPF `ToolNavigation`；不可扫描未知 DLL 后直接执行。完成主窗口展开、拖放、配置、运行、保存重开和关闭释放的实际操作验证后，才可更新 NFI-07 的完成状态。
