# VisionMaster 高保真逆向任务清单

- [x] FR-01 现有实现保真度总账 — `analysis/full-audit/fidelity/20261112/g1-completion.json`
- [ ] FR-02 原厂/当前启动与方案对照矩阵
- [ ] FR-03 启动链、方案实体和工具载体重审
- [ ] FR-04 执行/停止/结果/释放状态机重审
- [ ] FR-05 配置/ROI/Mask/显示数据流重审
- [ ] FR-06 第一条高保真真实工具垂直切片
- [ ] CHECK-FR 保真度门禁通过后再扩展工具批次

## Current execution target

`G9/P6`：在 G2–G8 已完成当前源码级范围后，按有证据的工具批次推进真实宿主、配置、保存重开、失败恢复和释放。Group 已完成当前可确认范围，只处理新增证据缺口，不作为工具批次主线阻塞。原厂 native、`.sol` 字段和桌面差分继续独立保持“运行未确认”。

当前 G2 交付：`docs/reverse/G2-startup-solution-entity-source-shaped-20261112.md`；无界面门禁已加入 `tests/VisionEditor.StartupShellChecks/Program.cs`，覆盖启动身份、保存路径、身份保持、重开和 clean 状态。原厂 `.sol`/自动加载/场景直启继续标记“运行未确认”。

G2 结果：`source-shaped`（门禁结果：`analysis/full-audit/fidelity/20261112/g2-completion.json`）。

G3 结果：托管执行/停止/结果/释放达到 `source-shaped-with-runtime-open`（门禁结果：`analysis/full-audit/fidelity/20261112/g3-completion.json`）。下一目标为 `G4`：配置、ROI、Mask、显示和订阅数据流。

G4 结果：配置/ROI/Mask/显示/订阅达到 `source-shaped-with-runtime-open`（门禁结果：`analysis/full-audit/fidelity/20261112/g4-completion.json`）。已修复 Point/Number 数组结果刷新崩溃。下一目标为 `G5`：ImageSource → 首个真实工具的高保真垂直切片。

G5 结果：ImageSource → ImageBinary/Blob 当前真实工具垂直切片达到 `source-shaped-with-native-runtime-open`（门禁结果：`analysis/full-audit/fidelity/20261112/g5-completion.json`）。下一目标为 `G6`：保存、重开、错误恢复和重复执行闭环。

G6 结果：保存/重开/错误恢复/重复执行达到 `source-shaped-with-native-runtime-open`（门禁结果：`analysis/full-audit/fidelity/20261112/g6-completion.json`）。下一目标为 `G7`：Group、子流程和复杂数据。

G7 结果：Group/子流程/复杂数据达到 `source-shaped-with-native-runtime-open`（门禁结果：`analysis/full-audit/fidelity/20261112/g7-completion.json`）。下一目标为 `G8`：原厂宿主与直接 SDK 的同输入首处分歧。

G8 结果：完成直接 SDK 重复基线与 Blob PE/ABI 首处分歧候选审计，状态为 `runtime-open`（证据：`analysis/full-audit/fidelity/20261112/g8-completion.json`）。原厂方案、匹配宿主依赖和 Run 前缓冲仍缺，未修改 Blob 私有算法。下一目标为 `G9`：在公共合同稳定后按工具批次扩展，并持续保留原厂差分门禁。

G9 第一批结果：Collection 的 ImageAcquisition、ImageBuffer、Light 完成批次自审（证据：`analysis/full-audit/fidelity/20261112/g9-completion.json`）。下一批继续选择有源码证据和真实测试的简单工具族；设备/动态 IO 维持“运行未确认”。

- [ ] P0-01 原件、环境和工作树身份冻结
- [ ] P0-02 输入、参数和结果基线冻结
- [ ] P1-01 启动与主窗口源码链
- [ ] P1-02 方案与持久化源码链
- [ ] P1-03 执行、回调、结果和释放源码链
- [ ] P1-04 配置、ROI、显示和订阅源码链
- [ ] P1-05 native ABI 和装载链
- [ ] P2-01 公共数据和执行合同
- [ ] P2-02 ImageSource → Binary/Blob 垂直切片
- [ ] P2-03 保存、退出、重开垂直切片
- [ ] P3-01 启动壳和主窗口闭环
- [ ] P3-02 原厂 `.sol` 只读探针
- [ ] P4-01 公共配置和 ROI 交互
- [ ] P5-01 Group、子流程和复杂数据
- [ ] P6-01 核心图像工具批次
- [ ] P6-02 标定、坐标、定位和测量批次
- [ ] P6-03 识别、缺陷和其余工具批次
- [ ] P7-01 匹配原厂身份恢复
- [ ] P7-02 宿主/SDK 同输入差分
- [ ] P7-03 第一处分歧归属和最小修复
- [ ] P8-01 全量回归、身份审计和主机验收
- [ ] P8-02 用户决定提交、推送和跨项目同步

## 使用规则

- 每个任务必须先有任务卡，再进入实现。
- 每 2 到 3 个任务设置一次 checkpoint。
- 任务完成只在源码证据、实现、自审和规定测试均满足后勾选。
- 原厂差分和人工验收状态独立记录，不能由单元测试数量替代。

## Definition of Done

- [ ] 每个关键路径有 E0 身份、E1 源码链、E3 运行证据、E4 独立回归和 E5 主机验收。
- [ ] 每个功能有唯一保真度状态，不能把 independent-contract 写成 parity-closed。
- [ ] 正例、负例、边界、重复、保存重开、错误、停止和释放均有复跑命令。
- [ ] 未知行为、差异归属、解除条件和回滚点已经记录。
- [ ] 原厂身份不匹配、首处分歧未定位、静默启动失败或资源泄漏时禁止发布。

门禁矩阵：`docs/VM-REVERSE-GATE-MATRIX-20261112.json`

G9 第二批结果：Logic 的 BranchModule、BranchString 完成插件/宿主批次自审（证据：analysis/full-audit/fidelity/20261112/g9-logic-completion.json）。下一批继续选择有源码证据和真实宿主门禁的工具族；设备/动态 IO 维持“运行未确认”。
G9 第三批结果：Fixture 完成插件 43 项与生产宿主 30 项自审（证据：analysis/full-audit/fidelity/20261112/g9-fixture-completion.json）。CameraMapping 因原厂 XML 资源缺失保持证据阻塞，不以独立实现替代。
G9 第四批结果：TranslationCalibration 完成插件 7469 项与生产宿主 19 项自审（证据：analysis/full-audit/fidelity/20261112/g9-translation-calibration-completion.json）。原厂 native 求解器、动态 IO 与 `.sol` 仍为“运行未确认”。
C01 启动链回归：根据本机 startup-error.log 定位旧执行端口 MouseLeave 的 `nodeCards["source"]` 崩溃，已用 TryGetValue 修复并通过 StartupShellChecks 与 VisionEditor Release 构建（证据：analysis/full-audit/fidelity/20261112/c01-startup-crash-completion.json）。
C02 宿主归属与动态目录：WorkflowOwnerHostChecks17 通过；ManagedToolCatalogHostChecks18 15/15 通过，覆盖运行 epoch、释放归属、动态注册、错误哈希、刷新、执行和保存重开（证据：analysis/full-audit/fidelity/20261112/c02-host-owner-catalog-completion.json）。
G7 Group 停止恢复：修复 StopRun 误取消当前 SelfRun 的生命周期偏差；GroupStopRecovery 10 项、GroupRoundLifecycle 58 项通过。关闭窗口仍负责取消和等待释放（证据：analysis/full-audit/fidelity/20261112/g7-stop-recovery-completion.json）。
C02 公共配置回归：WorkflowSubscription 60 项、ParameterHost 23 项通过；SolutionInspector 基础容错通过，原厂 CircleFind.sol 缺失保持运行未确认（证据：analysis/full-audit/fidelity/20261112/c02-configuration-subscription-regression.json）。
C02 结果执行回归：NodeProjection 54 项、ConfigurationExecution 39 项、FrameworkDebug 35 项通过，覆盖结果表、历史日志、停止和关闭清理（证据：analysis/full-audit/fidelity/20261112/c02-result-execution-completion.json）。
C01 ImageSource 流程回归：Workflow 13、WorkflowData 31、WorkflowCalls 93、ImageSource 配置 57、图像类型 Group 7、图像缓冲链 8 项通过（证据：analysis/full-audit/fidelity/20261112/c01-image-source-workflow-completion.json）。
C01 真实图像工具消费：CalibrationImageProducer 通过；CalibrationMapImageChain 与 MapCalibration.ImageExecutionChecks 因工具线 solver-fixture/frame-0.bmp 缺失保持开放，不复制或伪造资产（证据：analysis/full-audit/fidelity/20261112/c01-real-image-tool-consumer-completion.json）。
C01 NImage 真实宿主：ImageSource 10、InputRead 118、NImage Host 49 项通过，覆盖九帧输入、配置、显示、保存重载和释放（证据：analysis/full-audit/fidelity/20261112/c01-nimage-real-host-completion.json）。
C01 标定显示回归：CameraMappingDisplay 7 项、NPointDisplay 6 项通过；MapDisplay 因缺少两帧真实标定图像保持资产开放（证据：analysis/full-audit/fidelity/20261112/c01-calibration-display-completion.json）。
C02 工作区参数回归：WorkspaceProjection 79 项、NodeParameterWindow 11 项通过；ShellHistory 按源 XML 的工具栏-only Undo/Redo 合同修正夹具后通过（证据：analysis/full-audit/fidelity/20261112/c02-workspace-parameter-completion.json）。
P6 标定图像消费回归：NImageSolver 18、NImageCode 163、Distortion IMAGE source 11 项通过；native 求解与图像检测差分仍开放（证据：analysis/full-audit/fidelity/20261112/p6-calibration-image-consumer-completion.json）。
P6 Map/Distortion 资产审计：Map 图像结果 JSONL、original-board fixture 与 distortion fixture.bmp 缺失，保持证据开放，不生成替代资产（证据：analysis/full-audit/fidelity/20261112/p6-map-distortion-fixture-audit.json）。
P6 Coordinate/Translation/NPoint：Translation 宿主 19、NPoint 宿主 32 项通过；Coordinate 因原图 fixture 缺失保持开放（证据：analysis/full-audit/fidelity/20261112/p6-coordinate-translation-npoint-completion.json）。
P6 RotateCalibration 路径回滚修复：非法路径模型回滚、控件保留候选文本；Rotate 数值 720、总检查 1190、WPF 1259、生命周期 6 项通过（证据：analysis/full-audit/fidelity/20261112/p6-rotate-path-rollback-completion.json）。
P6 Map/ReadCalibFile 宿主：ReadCalibFile 输入 84 项、Map 身份 2 项通过；Map 图像执行仍受缺失 solver-fixture/frame-0.bmp 限制（证据：analysis/full-audit/fidelity/20261112/p6-map-readcalib-host-completion.json）。
P6 Coordinate 真实链：ROI、Point 和 Terminal 三组检查通过，覆盖图像尺寸变化、Draw、Group 输出、冷/热 ROI、Undo 和保存重开（证据：analysis/full-audit/fidelity/20261112/p6-coordinate-real-chain-completion.json）。
P6 Rotate/ReadCalib/Board 消费回归：RotateConsumer、RotateDisplay、ReadCalibDisplay 通过，BoardRotate 20 项通过，覆盖矩阵文件、连续样本、显示、保存重开、失败和恢复（证据：analysis/full-audit/fidelity/20261112/p6-rotate-readcalib-board-completion.json）。
P6 NImage 显示状态：NImageDisplay、NImageStateChain 26、NImage Reset 56 项通过，覆盖九帧边界、失败恢复、XML/Group/Coordinate、保存重开和 Reset（证据：analysis/full-audit/fidelity/20261112/p6-nimage-display-state-reset-completion.json）。
P6 NImage 插件自审：NImageCalibration.PluginChecks 8755 项通过，覆盖原始九帧、生成棋盘 6/7/8/9、透视、旋转和官方板解码；native 求解、原厂宿主链与像素级桌面一致性仍为运行未确认（证据：analysis/full-audit/fidelity/20261112/p6-nimage-plugin-completion.json）。
G9 NPoint 标定批次：Plugin 5817、Host 32、NumericInput 12、TextInput 43、PointAlias 35、Reset 36 项通过，覆盖真实宿主求解、旋转轴、Presenter、点表、显示数组、保存重开、失败恢复、输入边界、教学文本、别名和释放；native 求解与原厂 `.sol` 仍为运行未确认（证据：analysis/full-audit/fidelity/20261112/g9-npoint-calibration-completion.json）。
G9 Calibration 公共合同回归：配置桥、生命周期 6、失败恢复 14、历史重开和结果重开检查通过，覆盖八类标定工具的真实配置窗、提交、Undo/Redo、节点替换、单次释放及冷启动失败恢复；native 调用和原厂方案字段仍为运行未确认（证据：analysis/full-audit/fidelity/20261112/g9-calibration-contract-completion.json）。
G9 CalibrationBoard 批次：Enclosing 90、BoundaryFit 退出码 0、Reset 19、RotateChain 20 通过；DistortionDisplay 在板成功/失败显示清理 5 项后因缺失 SDK fixture 停止，主 Board Checks 因缺失 solver-fixture/frame-0.bmp 开放（证据：analysis/full-audit/fidelity/20261112/g9-calibration-board-completion.json）。
G9 ContourMatch 批次：Model 131、Result 24、TrainingPlane 8、TrainingScalar 23，共 186 项通过；native execution、原厂 `.sol`、人工宿主验收和像素差分仍为运行未确认（证据：analysis/full-audit/fidelity/20261112/g9-contour-match-completion.json）。
G9 GrayMatch Oracle 审计：ModelSlots 125、SourcePolicy 13 项通过；Dot/Score/SmallTemplate/TrainingAngle/TrainingPyramid 入口均正确拒绝缺失的 exact native oracle，保持证据开放，不以独立实现替代（证据：analysis/full-audit/fidelity/20261112/g9-gray-match-oracle-audit.json）。
G9 ROI/IO 批次：RoiTool 221、VisionIO 14，共 235 项通过，覆盖真实 ROI 工具、归一化坐标、尺寸变化、栅格化、调试会话释放和 IO 拒绝边界；native、原厂 `.sol` 和桌面差分仍为运行未确认（证据：analysis/full-audit/fidelity/20261112/g9-roi-io-completion.json）。
G9 路由/装载批次：IfModule 32、VisionModules 22、VisionPluginDefinitions 10、VisionRouting 20，共 84 项通过；VmRouting 因指定原件缺失保持开放，不以当前 JSON 替代原始输入（证据：analysis/full-audit/fidelity/20261112/g9-routing-loader-completion.json）。
G9 CameraMapping 审计：Input 36、Reset 47 项通过；Index/Plugin/Reachable/Solver 因缺少原厂 XML、输入或 solver trace 保持证据开放，不伪造原厂资源（证据：analysis/full-audit/fidelity/20261112/g9-camera-mapping-audit.json）。
G9 DistortionCalibration 批次：Configuration 16、Encoded68 24、File 89、PointAlias 14、Radial 48、Recognition 17 等通过，FixedKernel/Mean/Workspace 原始回放通过；CenterInput 与 RotatedRoi 因 fixture 缺失保持开放（证据：analysis/full-audit/fidelity/20261112/g9-distortion-calibration-completion.json）。
P6 标定路径拒绝值保留修复：共享策略增加按提供者选择的候选值保留合同，Map/CalibrationBoard/Distortion/NImage/NPoint/Translation 使用原厂候选显示与后端隔离；Calibration.PathPolicyChecks 85 项通过，Map 路径检查在缺失 reader-8.xml 前通过 16 项（证据：analysis/full-audit/fidelity/20261112/p6-calibration-path-retention-completion.json）。
C02 公共框架合同回归：Persistence 30、Execution 91、Integration 10、Array/Polygon 68、Debug 35，共 234 项通过，覆盖资源/悬空关系、执行状态、真实模块图、数组传输和停止关闭排空（证据：analysis/full-audit/fidelity/20261112/c02-framework-contract-completion.json）。
C01 编辑器壳回归：StartupShell、EditorCore 122、StorageBoundary 22、ShellSolutionPath 和 Persistence 30 通过；WpfChecks 在 104 项后因当前测试流程缺少 ImageSource 节点保持运行未确认（证据：analysis/full-audit/fidelity/20261112/c01-editor-shell-regression-completion.json）。
门禁强化：所有修复/优化必须引用 source-confirmed、native/ABI 或匹配运行证据；截图和用户反馈只登记差异，不直接定义语义。无证据行为不修改，记录为 unknown/unverified 及解除条件（门禁：docs/VM-REVERSE-GATE-MATRIX-20261112.json）。
C01 工具箱类型链路：依据原厂 GuideButton、ToolBoxData.ChangePriorityByInfo 和 PriorityStrategyInfo 源码补齐当前壳的证据化解析/消费；真实 ToolGroup.xml 四类优先级数值缺失，保持运行未确认（证据：docs/reverse/C01-toolbox-type-priority-chain-20261112.md）。
