# C02 / C03 整块交付退出审计

后续执行规则已按用户要求纠正：本报告的原厂验证缺口只限制对应差分，不能据此停止其他框架功能的源码逆向和实施。目标已恢复运行；不依赖原方案的 [公共成对数值配置](B06-numeric-range-configuration-20261111.md) 已继续落地。

本次按目标的完整范围核对交付条件。C02 已有源码链记录和自研生命周期修复通过整批回归；C03 已有固定输入的直接 SDK 差分及身份报告。**原厂同轮生命周期与宿主首处分歧尚未证明，整块原厂等价目标保持开放。** 测试数不计作完成单元数，人工验收不参与实施进度。

| 条件 | 当前权威证据 | 判定与剩余工作 |
|---|---|---|
| C02 单次提交、同步等待与失败返回 | `VmProcedure.Run → ProcessManager.SyncExecuteOnce → IMVS_ExecuteOnce_V30`；原反编译成员、CIL 及 SDK 原件见 [生命周期报告](C02-shared-execution-lifecycle-20261110.md) | 已有静态源码链；不据此认定实际回调一定到达。 |
| C02 回调、结果缓存、清结果和通知顺序 | `GetWorkStatusCallBackInfo → VmSolution.MProcessManager_OnWorkStatusCallBack`，独立的 `SetValue` 路径，以及 33 份原始方法体见 [回调报告](C02-callbacks-batch4.md) | 已有静态分支、计数及消费者记录；原实例的同轮投递、异常订阅者及队列关闭仍待运行证据。 |
| C02 连续停止、停止失败与资源完成边界 | SDK `0x408`、代理 `0xBC9`、ModuleFrame `DD030/DD330`；停止事件、返回码与通知分开记录 | 已有托管及原生静态链；停止返回不等于回调和资源收尾均完成。 |
| C02 配置等待阈值的真实执行效果 | `0x41f → 0xBDD → record+0x170 → ScheduleHelper → E0CF0`；任务直接及间接派发、`StopWhenNG` 的 `ModuStatus` 读取见 [原生报告](C02-native-wait-transport-20261022.md) | 已证模块入口前的超时跳过及错误写入；在途算法、后继模块及工作状态正常标志的同轮实际组合未证。自研仍为独立协作取消。 |
| C02 已确认的自研生命周期缺陷修复 | `GraphExecutor`、`SolutionExecutor`、`ToolDebugSession`、`ModuleExecutionScope` 的取消、清理、参数缓存、资源快照、状态创建及服务替换修复 | 已实施并自审；仅声明自研合同，没有将其替代为原厂调度合同。 |
| C02 无界面整批回归 | [11 套、462 项汇总](../../analysis/full-audit/batch1-20261107/C02/lifecycle-regression-495f7b05aab7469e9bd08ee3633d1496/summary.json)，含每套构建/运行退出码、日志、程序集哈希；同批身份审计确认 13 对程序集/PDB 与 87 份源码文件一致 | 已通过。覆盖执行、调试、持久化、真实模块图、Group、回调队列、执行所有权、子流程及配置。原厂、设备、桌面呈现不在此范围内。 |
| 测试及桌面产物对应当前源码 | [构建身份审计](../../analysis/full-audit/batch1-20261107/C02/lifecycle-regression-495f7b05aab7469e9bd08ee3633d1496/source-identity-audit.json) | 13 对程序集/PDB 的 CodeView GUID、stamp 匹配；87 份仓库源码的 PDB SHA-256 与当前文件一致。生成代码单列排除，不增加运行测试数。 |
| C03 原件、样图和历史方案身份 | [身份预检](../../analysis/full-audit/batch1-20261107/C03/host-identity-a28da3dbf1394c1d92d8145253651002/summary.json)；用户确认原方案已不在当前机器 | 样图和直接 SDK 身份可核。历史 `test1108.sol` 缺失，历史宿主主程序身份未证，四个安装宿主依赖不同于归档；历史同输入门未通过。 |
| C03 直接 SDK 的固定输入差分 | [26 次 SDK 矩阵](../../analysis/full-audit/batch1-20261107/C03/sdk-region-matrix-4dd306f7d33a447dba8e0793ad073807/summary.json)；探针源码 SHA-256 与汇总一致 | 已复现默认区域 `47 / 973202`、全图矩形 `45 / 971486`；借用像素初始化和图像输出开关已覆盖。不能推断历史宿主实际区域或像素。 |
| C03 入口、区域与输出的静态线索 | `ProcessAlg → 区域选择/SetROI → SDK Run → GetResult → ProcessOutput`；可选掩膜输出的调用者及第五参数区别见 [差分报告](C03-same-input-differential-20261110.md) | 已有精确调用点与缓冲比较。历史宿主 Run 前像素、活动区域、完整结果和输出缓冲未取得，因此首处分歧与根因保持开放。 |
| C03 按差异归属修改实现 | 需要先取得上述宿主首处分歧；当前 C01 真实注册表仍缺 `vision.blob@1.0.0` | 未满足实施依据，不用归档中的其他工具框架或猜测替代当前 Blob 注册。工具专属修改与公共合同依职责分开，跨项目同步由用户决定。 |
| 人工启动与桌面验收 | [当前 Release](../../analysis/manual-acceptance/flow-c02-d9f5f3d2bda44e13b16028f3abd5cb85/build/bin/VisionEditor/release/VisionEditor.exe)；0 错误、同一处 CS8603 两次 | 已构建，未启动；用户自行验收。手验状态不阻塞或增加实施计数。 |

## 复核入口与下一步

2026-11-11 复核：本轮再次搜索当前工作区的原厂方案，未发现 `test1108.sol`；工具清单没有可调用的 REA，进程清单没有 `VisionMaster` 或 `vServerApp`。重新执行只读身份预检，样图及直接 SDK 仍通过，四个宿主依赖仍不匹配归档，历史方案缺失。[最新身份预检](../../analysis/full-audit/batch1-20261107/C03/host-identity-a28da3dbf1394c1d92d8145253651002/summary.json)。历史方案和匹配宿主仍未具备，因此历史原厂差分保持开放。当前实现只具备独立 JSON 方案的创建、保存、重开和执行，原厂启动屏障、引导页、SDK 自动加载、`.sol` 容器及方案直启场景链仍未实现，运行未确认行为保持开放。新基线不能替代历史复现，但可以在独立方案合同闭合后继续当前宿主与直接 SDK 的差分。该条件只限制历史方案复现，自研修复、框架实施和证据审计继续推进。

产品和测试源码未变时，可核对已完成测试与当前工作树的对应关系，无需重跑整批测试：

```powershell
pwsh -File tests/NativeEvidence/C02/Audit-RegressionIdentity.ps1 `
  -SummaryPath analysis/full-audit/batch1-20261107/C02/lifecycle-regression-495f7b05aab7469e9bd08ee3633d1496/summary.json `
  -DesktopDirectory analysis/manual-acceptance/flow-c02-d9f5f3d2bda44e13b16028f3abd5cb85/build/bin/VisionEditor/release
```

该入口核对整批套件集合、计数、运行器哈希、各套程序集哈希、程序集/PDB 身份及仓库源码校验和。它只证明产物身份，不执行原厂程序，不证明 VM 等价，也不构成新增测试通过。运行器、计数、程序集不符的拒绝观察另存 [负例记录](../../analysis/full-audit/batch1-20261107/C02/lifecycle-regression-495f7b05aab7469e9bd08ee3633d1496/source-identity-negative-cases.json)。

后续原厂取证需要匹配运行环境和可复核方案，并记录实际加载模块身份。C02 采集同轮的模块进入/返回、阈值跳过、错误、正常标志、停止动作、回调及资源完成。C03 采集 Run 前像素和区域，再比较完整结果向量与输出图像。新建方案须登记为新基线；缺少历史方案时不能称历史复现已通过。未获得这些证据前，不把自研停止语义替换为推测的原厂调度行为，不修改 Blob 私有算法。

实施与自审进度沿用逐单元复核账本：框架 **41/96（42.71%）**，分母为 96 个框架单元；本轮没有整单元新增完成。已选标定工具 **10/10（100%）**，分母仅为已授权的 10 个工具。两者均不表示 162 个目录工具或原厂全域行为完成。






