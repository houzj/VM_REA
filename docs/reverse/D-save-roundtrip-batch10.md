# 第十批D：独立存储保存/重开一致性实施

基线 `877671f08eb2e6bf4eefff0bea461473624895cf`。本项针对main自有JSON格式的真实缺陷，依据完整本地源码核对，不调用REA或把规则冒充原厂.sol格式。

## 问题与补丁

`SolutionStore.Load` 对完整JSON递归拒绝重复对象成员；`Save`原来只做类型化结构校验与序列化大小校验，不透明`NodeDocument.Parameters`中的JsonElement可携带重复成员。结果是Save可成功替换目标，Load随后拒绝同一文件。

`src/VisionFramework/SolutionStore.cs::Save`现在在创建目录/临时文件前解析已序列化的字节，并复用Load的RejectDuplicateProperties。拒绝不改目标、不制造目录或临时文件；不同对象中的相同属性名仍合法。复用现有加载规范，没有猜插件私有字段或改公共合同。

`tests/VisionFramework.SaveRoundTripChecks`实际调用当前Store，对平级/数组内重复属性、合法不同对象同名、旧目标字节不变和无目录/临时文件副作用共9项检查，根线运行exit0，结果`analysis/full-audit/batch10-20261019/D/save-roundtrip-f21e979a779c4506b420f62d169c1fa4/result.json`。该文件记录真实运行结果，原始首次命令输出保留在会话工具记录。

## 自审核与计分

新增校验及历史seam合并后，根线另跑既有`VisionEditor.SaveHistoryChecks`，23/23通过、exit0；完整日志`analysis/full-audit/batch10-20261019/D/save-history-regression.log`，结果在其save-history-e69fbc5ad0d34153a50f427159b0c209子目录。覆盖跨目录多次另存、Undo/Redo、目标锁定失败和资源字节身份，未把此回归扩成FCR024插件私有资产已修复。

补丁仅影响独立格式写盘前的校验，不改变原SDK或插件准备器生命周期。D02.1原已实施done，本修复不重复增加分子。FCR-024插件私有资产历史重定位仍未交付，D02.2保持partial；不因这9项通过关闭资产Undo或原生格式问题。

无原目标/SDK运行、设备操作、人工窗口验收、tool-plugins访问或改动。人工验收后置不阻挡本补丁自审核，但不把产品修复数量作为总实施单元数量。
