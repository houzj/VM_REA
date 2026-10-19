# D02 第四批存储边界独立复核

2026-10-19，审查基线 `8b7b2de915155c031d06b633c38f0b778a59f8a4`。本复核只读当前 main 完整源码、探针和已经完成的运行记录，没有再次运行 dotnet，没有调用原厂程序集或访问 tool-plugins。按 REA 技能的完整源码分析边界，本切片无需原生分析工具。

结论：22 个具名检查和保留的 `passed=true` 记录一致，可作为独立 JSON 格式的此次操作证据；未发现需要阻断整合的断言问题。它们不是 22 个独立场景，也不证明原厂 sol 或完整 D02 等价。无产品修改。

## 源链和断言审查

- `src/VisionFramework/SolutionStore.cs::Load` 先检查输入流长度，再解析 JSON、递归拒绝重复属性、以 Disallow 策略反序列化、检查结构。未来版本、未知流程字段、显式 null 模块版本、插件不透明载荷中的重复属性和过深嵌套分别触发对应分支。日志保留了异常类型和消息；深度异常为 JsonReaderException，是探针预期 JsonException 的派生类型，匹配有效。
- 精确输入边界使用有效 JSON 加尾部空白，实际留存文件为 33,554,432 和 33,554,433 字节；前者接受、后者拒绝。此证据针对文件字节上限，不能解释为同等大小的有效 JSON 结构复杂度接受。
- `SolutionStore.Save` 在路径目录/临时文件创建前完成结构、序列化与大小检查。探针用 ASCII Name 推算固定开销，再以真实文件长度断言精确边界；超一字节失败后检查原目标 SHA256 和特定临时文件模式。该操作支持此次超限拒绝未改变目标，不支持断电、文件锁、跨卷或实际写盘中断的保证。
- `src/VisionEditor.Core/GraphEditorDocument.cs::Load` 在 Store 成功后才写编辑器状态。`Save` 在 PrepareSave、Rebase 和 Store 成功后才提交状态。探针事先构造两次改名及一步 Undo，逐拒绝核对序列化文档、基目录、文件路径、Revision、IsDirty、CanUndo/CanRedo、流程选择，以及两类事件计数；最后实际 Redo 及两步 Undo。比只验证异常更强，确实覆盖这组历史操作的可用性。
- PrepareSave 分支使用只返回超大副本的受控委托，确认准备器调用一次、原始目标不变以及公共状态不变。不能外推任意准备器产生的文件、设备或其他外部副作用可被回滚。

## 表述边界与仍缺范围

`missing-module-version` 的实际输入是属性显式 null，不是删除属性；结论应写“模块版本为空被拒绝”。未知插件载荷只核对当前 `Opaque.FutureVendorField=12` 的 Save→Load 值，不保证任意厂商载荷的字节保真。深度测试覆盖一个超过默认 64 层的输入，没有定位每一种文档形状的最大有效嵌套边界。

“历史不变”在此次证据中指公共状态逐次相等及指定历史操作成功；未直接枚举内部栈对象或证明任意历史长度/内容。检查的两个事件是 Core 的 Changed/SolutionReplaced，没有桌面窗口、插件监听器或原厂运行回调验收。

FCR-024 的跨目录另存后插件资产 Undo 失败仍为既有开放缺口；本探针没有真实插件资产、插件 PrepareSave 或跨目录历史场景，不能覆盖、更不能关闭该失败。本次也未覆盖文件锁、进程终止、并发文件变化、迁移或原生容器。

## 审计一致性

输入清单 `analysis/full-audit/batch4-20261019/D02/source-operation-sha256.csv` 的 15 项当前文件哈希与字节长度全部匹配，包括两份产品源码、项目/探针源码、有效/非法文件、日志及 result.json；产品两文件没有未提交 diff。复核明细保留在 `analysis/full-audit/batch4-20261019/D02/review/hash-verification.csv`。运行记录为 `storage-boundary-750ad645a43b41c7ba77ded445341925/result.json`，未重新生成或覆盖。
