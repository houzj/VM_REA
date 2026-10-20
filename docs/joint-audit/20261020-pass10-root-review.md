# 第十轮根线自审：标定双图、板矩阵文件与点编辑成本

当前目标只含“标定”十工具的原源码到框架实际交接。根线串行复跑本轮三个独立探针，并按载荷类型审查原证据：映射标定的双图输入、标定板到旋转标定的 9-float 文件、N 点编辑到方案/文件消费者。没有产品代码修复，`tool-plugins` 在制文件未动。

| 专属链 | 根线实测 | 原件边与保留界限 |
|---|---|---|
| 映射标定双 IMAGE→XML→标定加载/Group/坐标系 | 首跑及保存重开后删除旧 XML 再跑，各 4 项成功，源/目标各 168 角点、九项矩阵被真实消费者读取；日志 `analysis/joint-audit/pass10-cal-map-image/run.log` SHA-256 `AC2578A7849BA0BDA0C9BFAB65966C64CF8187774C30107C3B71A44E5E1FB522`。 | [双图报告](20261020-pass10-map-image-chain.md) 的原 `Process/GetInputImage/ProcessAlg` 双图与导出分支；双 ROI、教学、原厂同构执行和数值逐位差分未证。 |
| 标定板→旋转标定 `.iwcal` | 20/20，经当前 Board 图产 99 点/36B，文件九 float 与 Board 结果及 Rotate loader 逐 float32 位相同；3 帧累积 2→1→0，空白/缺文件失败后同 scope 恢复。根线独立复跑日志 `analysis/joint-audit/pass10-cal-board-rotate/root-run.log` SHA-256 `B2A984ADA01B8944E46EE94F1452DDED9FCA61366ACD35C5CCA92061145032C7`。 | [板/旋转报告](20261020-pass10-board-rotate-chain.md) 的原 SDK Export `fwrite 0x24`、Rotate `LoadCalibFile` 精确 0x24 `fread` 与 `CalibRot` 索引 `[0,1,3,4]`；旋转样本由测试专用源提供，原厂桌面同构图和设备/数值全域未证。 |
| N 点点表单格→历史→XML/标定加载 | 真实点编辑器配同构回调，在真实 `GraphEditorDocument` 上 4 行初始化 20 次 Apply/Changed/revision，单格再 +1；Undo/Redo、保存重开、两节点求解/加载均 Success。根线独立复跑日志 `analysis/joint-audit/pass10-cal-npoint-edit/root-run.log` SHA-256 `6342C9071DCD54BECE0E0F1DBA63B2FC2F6B4572610088F237AB608893FCCC18`。 | [点编辑报告](20261020-pass10-npoint-edit-chain.md) 的原 `NPointList.Init→PointList` 单叶 setter 与 `SetParam`；当前额外整文档历史/刷新放大有源码与受控 API 计数，但非实际 MainWindow 点击/原厂时延差分，也不能据此单独解释长期无响应。既有跨项目候选合同 FCR-033 追加证据，尚未实施。 |

探针首试缺引用、夹具路径错误及其他构建差异保留在各自审计目录/报告，最终通过不抹去失败。N 点 XML 含实时 `CreateCalibTime`，因此两次 SHA 不相同；矩阵和编辑值的断言仍通过，不把动态文件哈希作为数值回归 oracle。第六至十轮证明了多种生产者/消费者的实际图链，尚未完成十工具逐一 J01–J12 源码→UI→算法→错误/释放的全部边，自审范围以索引逐行未覆盖项为准。

具名主框架实施进度仍为 **41/96 = 42.71%**：本轮只补原源码与当前已实现代码的交接证据，没有闭合新的框架实施单元。人工桌面验收另列，不纳入分母。
