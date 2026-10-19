# D02 batch2：独立方案多次另存、历史与资源身份

2026-10-19；main基线 `6e5b05c`。这是独立 `.vmx.json` 的源代码/操作验证，不是原厂 `.sol`、SDK保存事务或VM撤销模型取证。原始格式仍由D01等任务追踪。本批未修改产品或tool-plugins。

## 本轮问题与源码

历史检查覆盖单次另存、准备失败、锁定写入失败及损坏加载，但没有组合验证“另存改变资源基目录后，多次Undo/Redo仍保持同一资源”。先读当前完整调用链，避免凭常规意图认定缺陷：

- `src/VisionEditor.Core/GraphEditorDocument.cs::Save`：准备独立文档副本，Rebase到新目录，SolutionStore.Save成功后才更新document/BaseDirectory/FilePath/脏状态和Revision。
- `Capture/Restore`：Snapshot同时保存Document、ProcedureId和BaseDirectory；Restore先将历史文档从其快照基目录重定位到当前基目录，才转移历史栈。未采用“恢复旧路径但保留新目录”的不完整模型，本轮没有发现该疑似缺口。
- `Rebase`：重定位ResourceDocument.Path，以及明确支持的内置 `vision.image-file` 字符串Path；不推断其他插件私有参数路径。
- `src/VisionFramework/SolutionStore.cs::Save`：结构/编码/大小校验、同目录独占临时文件、Flush、替换、finally清理；`Load`校验版本、重复及未知属性等。
- `src/VisionFramework/GeometryResources.cs::ResourceCatalog.OpenVerified`：磁盘与嵌入字节分别打开，有摘要时完整哈希验证并复位流位置，调用方持有流所有权。

以上为本地重构源码事实，不是REA原程序证据。按REA技能的范围规则，普通完整源代码审查不重复调用原生引擎。仅在将来对照原厂实现时使用其原始成员/指令证据。

## 操作级检查

新增 `tests/VisionEditor.SaveHistoryChecks`，引用真实main Core/Framework/Modules；不用镜像实现或mock存储器。为每次运行建立独占GUID目录并保留实际文件和result.json，无递归清理。资源使用固定二进制字节及SHA256，不运行图像算法。

| 操作序列 | 验证内容 |
|---|---|
| 原目录加载→改名→另存到first | 磁盘/嵌入资源完整字节，内置图像Path仍解析到原资产 |
| Undo跨原目录历史快照 | 恢复改名前名称但保持当前first文件/目录，脏状态及redo保留，资源身份一致 |
| 再另存到second→Redo跨first历史快照 | 文件/目录仍为second，名称恢复编辑值，资源和内置路径均正确 |
| Undo→锁住first目标→尝试另存first | 实际写入失败，当前second文档/基目录/修订/脏状态/redo及目标字节不变，临时文件清除 |
| 解除锁→Redo→保存second→重开 | redo仍有效、最终存储名称正确、资源可校验读取；原方案和资产字节不变 |

命令：`dotnet run --project tests/VisionEditor.SaveHistoryChecks/VisionEditor.SaveHistoryChecks.csproj -p:BuildInParallel=false -p:UseSharedCompilation=false -- analysis/full-audit/batch2-20261019/D02`。

退出0，23项具名检查通过。日志 `analysis/full-audit/batch2-20261019/D02/save-history.log`；本次产物 `save-history-f1af2a7dae404d29b57c5c035af90187/result.json` 及所有原始/first/second文件保留。根任务串行运行此dotnet构建，其他子任务仅做各自源码或独占Framework探针。

## 结论和未关闭范围

本切片验证了独立格式的跨基目录历史及失败另存事务，没有产品补丁。仅一个改名历史项跨两次另存，不证明任意深度、多种编辑历史；未设置PrepareSave，也未验证实际插件准备器资源重写。独立复核见 [D02-review-batch2.md](D02-review-batch2.md)。D02整包不关闭：32MiB/深度边界、跨版本/未知字段策略、全部插件私有资源、复杂准备器写外部文件的副作用、跨卷/断电/真实进程中断仍未由本探针证明。当前Load显式拒绝未知属性，不能宣称保留未来字段。Save后的Changed事件异常属于提交后通知边界，未当作写盘失败原子性保证。

下一步按实际使用需求拆分资源种类/准备器副作用/版本矩阵，源确认后补必要操作；不为增加测试数重复已有30项基础持久化或28项流程检查。原生格式与其未知字段保存需原始writer/reader证据，不能从本地JSON规则推导。
