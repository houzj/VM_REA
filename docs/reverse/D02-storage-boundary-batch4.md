# D02 第四批：独立格式大小、深度与拒绝事务

2026-10-19，基线 `8b7b2de915155c031d06b633c38f0b778a59f8a4`。本批跟进 D02 已登记但未验证的大小/未知字段边界，不重复上批插件资产诊断，不修改 FCR-024 的待交接状态。

## 源码依据和操作范围

完整当前源码可直接审查，因此此独立格式切片不调用 REA 原生工具；原厂其他三线仍按 REA 技能取证。不能从本地 JSON 规则推导原厂 sol 规格。

- `src/VisionFramework/SolutionStore.cs::Load` 在 JSON 解析前检查文件长度，超过 `32*1024*1024` 才拒绝；使用默认 JsonDocument 深度限制，遍历所有对象拒绝重复属性，再以 Disallow 反序列化并检查版本及结构。
- 类型化文档字段拒绝未知成员；`NodeDocument.Parameters` 的 JsonElement 是不透明 JSON 载荷，但依然经过递归重复属性检查与解析深度限制。未知文档字段和未知插件载荷不是相同策略。
- `SolutionStore.Save` 先结构校验、序列化和大小校验，再创建目录及同目录临时文件、Flush(true) 和替换；本轮只验证进程内超限拒绝，不据此声称断电原子性。
- `src/VisionEditor.Core/GraphEditorDocument.cs::Load` 成功 Load 后才替换文档、选择、路径和历史；`Save` 在准备、重定位、写盘成功后才提交编辑器状态。失败准备器的外部副作用不在这一保证之内。

探针 `tests/VisionEditor.StorageBoundaryChecks` 使用真实 main Core/Store，独占 GUID 输出目录，保留所有有效/非法文件及结果，不访问安装原件、设备、tool-plugins。建立两个改名历史并 Undo 一步，使拒绝前同时有 Undo/Redo 和脏状态；逐次核对完整公共状态及 Changed/SolutionReplaced 次数，最后实际 Redo/Undo 验证历史仍可用。

矩阵包括：未来版本、未知流程字段、必填模块版本为空、插件 JSON 深层重复字段、70 层载荷、输入精确 32MiB/多一字节、输出精确 32MiB/多一字节、PrepareSave 产生超大文档、未知插件载荷往返。固定小输入在大文件尾部填空白，区分文件长度限制与实际 JSON 载荷大小；大输出用 ASCII 名称精确控制序列化长度。

## 运行记录

命令：`dotnet run --project tests/VisionEditor.StorageBoundaryChecks/VisionEditor.StorageBoundaryChecks.csproj -p:BuildInParallel=false -p:UseSharedCompilation=false -- analysis/full-audit/batch4-20261019/D02`。

本次 exit0、passed=true，22 项具名检查通过；结果在 `analysis/full-audit/batch4-20261019/D02/storage-boundary-750ad645a43b41c7ba77ded445341925/result.json`，日志为 `storage-boundary.log`。独立复核见 [D02-storage-review-batch4.md](D02-storage-review-batch4.md)，未以运行数关闭整个工作包。逐次断言覆盖公开状态相等及通知次数，最后的 Redo/两步 Undo 验证历史仍可用；没有逐对象比较私有栈内容，版本字段样本是显式 null 而非省略属性。无产品变更。未验证跨卷、断电、进程中断、极限并发修改、任意资源/插件准备器、原生格式迁移或原厂桌面等价；D02 完整包保持开放。
