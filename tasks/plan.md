# Implementation Plan: VisionMaster 高保真本地逆向

总方案：`docs/VM-COMPLETE-REVERSE-IMPLEMENTATION-PLAN-20261112.md`

## Architecture decisions

- 采用证据等级 E0-E5 和每单元 S0-S7 门禁。
- 按 VS0-VS8 垂直切片推进，先闭合启动、方案、执行、结果和释放，再扩展工具批次。
- 主框架和工具插件使用唯一提交所有者和合同请求，不复制另一工作树未提交代码。
- 原厂 `.sol`、native ABI 和设备行为在证据缺失时保持不透明，不用独立 JSON 或托管替身冒充兼容。
- 执行、配置、方案和原生适配均通过深模块 seam 对外提供小接口，内部隐藏状态机和释放语义。

## Task list

### Phase -1: Fidelity correction before expansion

- [ ] FR-01 建立现有实现逐功能保真度账本，区分 parity-closed、source-shaped、independent-contract、visual-only、divergent 和 unknown。
- [ ] FR-02 采集当前构建与原厂启动/方案/主窗口的对照矩阵，固定窗口、资源、状态转移和交互差异。
- [ ] FR-03 重审启动链、方案实体、工具载体和主窗口导航的原厂调用链。
- [ ] FR-04 重审执行、停止、结果、错误和释放的原厂状态机。
- [ ] FR-05 重审配置、ROI、Mask、显示和参数提交的数据流。
- [ ] FR-06 以 ImageSource → Blob 或一个证据最完整的真实工具闭合第一条高保真垂直切片。

### Fidelity checkpoint

- [ ] 当前实现与原厂差异有证据路径，不再只依赖截图和测试数量。
- [ ] FR0-FR4 的每个差异都有处理决定：保留、重写、隔离或运行未确认。
- [ ] 未达到 source-shaped 的区域冻结继续扩展。

### Phase 0: Evidence foundation

- [ ] P0-01 冻结两个工作树 HEAD、工作树差异、安装根和工具版本。
- [ ] P0-02 建立 DLL、SDK、PDB、资源、方案、样图和依赖哈希清单。
- [ ] P0-03 固定输入、参数、ROI/Mask、像素格式、输出字段和失败码基线。
- [ ] P0-04 建立原厂机器、桌面、权限、设备和可调用 REA 能力清单。

### Phase 1: Source and architecture recovery

- [ ] P1-01 恢复 Program/App/启动服务/主窗口入口链。
- [ ] P1-02 恢复方案、Procedure、Node、资源和最近列表调用链。
- [ ] P1-03 恢复 Run/Stop/Callback/Result/Error/Dispose 生命周期。
- [ ] P1-04 恢复配置、ROI、Mask、显示、订阅和二进制载荷链。
- [ ] P1-05 恢复 native 虚表、寄存器、栈、导入和装载路径。

### Checkpoint 1

- [ ] 关键路径入口到结果和释放没有未标记断边。
- [ ] 缺失成员已经进入取证清单，没有推测实现。

### Phase 2: Shared contracts and execution

- [ ] P2-01 固定数据类型、端口、动态 IO、Region、Mask、Fixture、数组和复合叶合同。
- [ ] P2-02 固定方案身份、节点版本、位置、连线和资源引用。
- [ ] P2-03 固定 Run/Stop/Cancel/Callback/Result/Error/Dispose 顺序。
- [ ] P2-04 完成 ImageSource → Binary/Blob 垂直切片 VS1。
- [ ] P2-05 完成保存、退出、重开和资源恢复 VS2。

### Phase 3: Startup and shell

- [ ] P3-01 完成空载启动、单实例、工作目录、日志和关闭释放 VS0。
- [ ] P3-02 完成 Guide、最近方案、创建方案和文件打开。
- [ ] P3-03 完成主窗口工具箱、画布、流程 Tab、结果和错误面板。
- [ ] P3-04 建立 `.sol` 只读探针和原厂格式证据卡。

### Phase 4: Configuration and interaction

- [ ] P4-01 完成参数模型、普通配置和保存恢复。
- [ ] P4-02 完成数值、枚举、布尔、文本、数组和二进制参数。
- [ ] P4-03 完成 ROI/Mask/全屏/图像尺寸/显示开关和失败恢复。
- [ ] P4-04 完成订阅、结果显示和调试会话。
- [ ] P4-05 以一个真实工具闭合 VS3。

### Phase 5: Group and complex data

- [ ] P5-01 完成 Group、子流程、输入输出、引用和持久化。
- [ ] P5-02 完成 Loop、Branch、Break、累加、数组和复合叶。
- [ ] P5-03 完成嵌套、重入、取消、异常和资源释放。
- [ ] P5-04 闭合 VS4 和 VS5。

### Phase 6: Tool batches

- [ ] P6-01 ImageSource、ImageBinary、Blob、Fixture、Branch 批次。
- [ ] P6-02 标定、坐标、定位和测量批次。
- [ ] P6-03 识别、缺陷和其余工具批次。
- [ ] P6-04 每批至少一个主窗口真实消费和保存重开。

### Phase 7: Original differential

- [ ] P7-01 恢复匹配原厂宿主、SDK、方案和输入身份。
- [ ] P7-02 宿主与 SDK 各独立重复运行。
- [ ] P7-03 比较像素、ROI/Mask、结果向量、图像缓冲、错误码和释放。
- [ ] P7-04 定位第一处分歧并归属公共或工具专属实现。
- [ ] P7-05 对已证实差异实施最小修改并复跑正负例。

### Phase 8: Release and handoff

- [ ] P8-01 全量无界面回归和身份审计。
- [ ] P8-02 关键切片主窗口人工验收。
- [ ] P8-03 生成每工具退出矩阵、构建产物、哈希和复跑命令。
- [ ] P8-04 用户决定提交、推送和跨项目同步。

## Checkpoint 2

- [ ] C2-01 所有关键路径通过 S0-S6。
- [ ] C2-02 至少一个真实工具通过主窗口 VS3。
- [ ] C2-03 Group 和失败/停止/释放通过 VS4/VS5。

## Checkpoint 3

- [ ] C3-01 原厂差分报告能指向第一处可重复分歧。
- [ ] C3-02 未确认行为和解除条件已登记。
- [ ] C3-03 交接报告包含身份、命令、产物、哈希和回滚点。

## Risks

| 风险 | 处理 |
|---|---|
| 原厂方案缺失 | 继续静态和独立实现，原厂门单独打开 |
| 宿主版本不匹配 | 先恢复身份，不比较数值 |
| ABI 不清 | 采集真实寄存器/栈/虚表，禁止猜测 |
| 测试通过但非 VM 语义 | 保持 independent，等待原厂证据 |
| 两工作树冲突 | 单一所有者和 CONTRACT-REQUESTS |
| 构建输出占用 | 独立 OutDir 和串行构建 |

