# 完整逆向任务清单（一次列全）

从 `buildModel()` 与 9 个机器台账 / 7 个补充源确定性派生。勾选框由人工验收后填写，机器不预填任何 `[x]`；状态字段原样来自台账。

- 生成：`node tests/ArchitectureRecovery/build-complete-task-list.cjs`
- 独立校验：`node tests/ArchitectureRecovery/verify-complete-task-list.cjs`
- 机器台账：`tests/ArchitectureRecovery/complete-task-list.map.json`
- 站点入口：`docs/reverse-plan/index.html`（19 页，本清单不新增页面）
- builtFrom：`7d65a1e1/dfd5fd922c28`（HEAD + 输入 SHA-256，与站点同源）

## §0 口径总览与硬规则

| 项 | 值 |
|---|---|
| 授权范围完成度 | `docs/SOURCE-LEVEL-REVERSE-PROGRESS.json` 10/10 = 100%（仅标定授权范围，人工验收单列） |
| 当前目标 | `C03 original Blob runtime differential` / running |
| S0 | `S0-closed-on-local-slice; C03-root-cause-still-open`（S0.1/S0.2 已在当前构建产物复跑，S0.3 差分开放项仍满足） |
| S0 复跑 | C01 exitCode=0 / 18 PASS；同输入 1.jpg 得 47/973202，报告 SHA-256 与历史逐字节相同；12 张标定样本重开计数一致 |
| 开放差分 | 原厂 45/971486 对直接 SDK / 本地 47/973202（+2 / +1716）；C03 原厂根因仍开放 |
| 第一批 | `C03-INCREMENTAL-B0B1-001` · B0 open / B1–B3 blocked · C01 复跑 1 条已执行 · 产品改动 0 |
| 渐进批次 | 4 批次 / 13 步，全部 not_started，10 步带前置阻塞 |
| 主界面控件 | 17 项 / 4 类，全部 blocked，102 个六段链节点 unknown，三级验收 0 完成 |
| 目录工具 | 162 项，161 未建立 + 1 partial，全部为逆向实现目标 |
| 入口链 | `closedEntryChains=0`，E0–E7 八门未闭 |
| 口径冲突 | 42/96=43.75% 与 41/96=42.71% 并列；PositionCorrection 批次身份差异原样保留 |
| 硬规则 | 不自动提交/推送、不自动跑方案测试、不改 src/、缺失标 unknown、截图只作差异信号 |
| 勾选框 | 全部 `[ ]`，机器零预填；状态为 `done` 的 29 项来自三层台账既有状态，不等于本清单已验收 |

授权状态：用户已一次性授权「全放开、批量落地」；C01 复跑已完成（exitCode=0）。提交需用户明确说「提交」；B0 根因补证仍需可查询原厂过程或 provider 修复。

## §1 P0 前置：S0/C03 补证与四条方向

B0 未闭前不得进入后续批次的实施类动作。历史 `18/18 passed` 不是本轮复跑，本地/SDK 一致不等于原厂 parity。

共 10 项，勾选框全部未预填；状态字段来自机器台账。

- [ ] **P0-01** S0/C03 开放差分补证：45/971486 对 47/973202 · 状态 `open` · 来源 `VM-LOCAL-RESTORATION-STATUS.json` · 证据 2 项 · 退出 取得区域对象/像素缓冲/native 结果数组；差分被解释或显式登记为 unknown，不抹平 · 锚点 `04-vertical-slice.html#slice-phase1`
  - 证据：`docs/VM-LOCAL-RESTORATION-STATUS.json`、`analysis/full-audit/batch1-20261107/C03/c03-phase1-evidence-card.json`
  - 成员：台账未登记（unknown），需源码级取证后填入
- [ ] **P0-U1** 解除 unknown：活动运行 param_1+0x20 / +0x38 集合基数与 FullScreen 转换结果 · 状态 `unknown` · 来源 `c03-phase1-evidence-card.json` · 证据 1 项 · 退出 取得对应源码/运行证据并登记证据类型 · 前置/阻塞 P0-01
  - 证据：`analysis/full-audit/batch1-20261107/C03/c03-phase1-evidence-card.json`
  - 成员：台账未登记（unknown），需源码级取证后填入
- [ ] **P0-U2** 解除 unknown：SDK Run 前确切输入像素缓冲、灰度转换与边缘包含 · 状态 `unknown` · 来源 `c03-phase1-evidence-card.json` · 证据 1 项 · 退出 取得对应源码/运行证据并登记证据类型 · 前置/阻塞 P0-01
  - 证据：`analysis/full-audit/batch1-20261107/C03/c03-phase1-evidence-card.json`
  - 成员：台账未登记（unknown），需源码级取证后填入
- [ ] **P0-U3** 解除 unknown：45 元素结果向量与 BinaryImage/BlobImage 原始缓冲区 · 状态 `unknown` · 来源 `c03-phase1-evidence-card.json` · 证据 1 项 · 退出 取得对应源码/运行证据并登记证据类型 · 前置/阻塞 P0-01
  - 证据：`analysis/full-audit/batch1-20261107/C03/c03-phase1-evidence-card.json`
  - 成员：台账未登记（unknown），需源码级取证后填入
- [ ] **P0-U4** 解除 unknown：R8/R9 指向布局、第五栈参数、首个间接调用目标、生成掩膜栅格化 · 状态 `unknown` · 来源 `c03-phase1-evidence-card.json` · 证据 1 项 · 退出 取得对应源码/运行证据并登记证据类型 · 前置/阻塞 P0-01
  - 证据：`analysis/full-audit/batch1-20261107/C03/c03-phase1-evidence-card.json`
  - 成员：台账未登记（unknown），需源码级取证后填入
- [ ] **P0-U5** 解除 unknown：原厂异常/取消/释放路径的运行时证据 · 状态 `unknown` · 来源 `c03-phase1-evidence-card.json` · 证据 1 项 · 退出 取得对应源码/运行证据并登记证据类型 · 前置/阻塞 P0-01
  - 证据：`analysis/full-audit/batch1-20261107/C03/c03-phase1-evidence-card.json`
  - 成员：台账未登记（unknown），需源码级取证后填入
- [ ] **P0-02** 复跑 C01 纵切（已在 S0 复跑批次执行） · 状态 `executed` · 来源 `c01Evidence.command / C03-S0-RERUN-MATRIX-20261110` · 证据 3 项 · 退出 exitCode=0 / 18 PASS 已记录；同输入报告 SHA-256 与历史相同；S0.1/S0.2 升级为当前构建产物已验证；C03 根因保持开放 · 锚点 `17-incremental-landing.html#inc-commands`
  - 证据：`tests/VMVerticalSlice.C01/VMVerticalSlice.C01.csproj`、`analysis/full-audit/batch1-20261107/C01/runtime-report.json`、`analysis/full-audit/batch1-20261107/C03/c03-s0-rerun-matrix-20261110.json`
  - 成员：台账未登记（unknown），需源码级取证后填入
  - 命令〔待执行〕：`dotnet run --project tests/VMVerticalSlice.C01/VMVerticalSlice.C01.csproj --no-restore`
  - 报告：`analysis/full-audit/batch1-20261107/C01/runtime-report.json`、`analysis/full-audit/batch1-20261107/C03/c03-s0-rerun-matrix-20261110.json`
- [ ] **P0-03** 继续 B1 静态取证（不改算法、不跑测试） · 状态 `open` · 来源 `incremental-landing.map.json / C03-INCREMENTAL-B1-CONTRACTS-001` · 证据 3 项 · 退出 逐项核对纵切链路准确类型与成员；输出名清单与方案文件字节级比对 · 前置/阻塞 B0 未闭不得进入实施类动作
  - 证据：`docs/reverse/C03-incremental-batch0-b1.md`、`docs/reverse/C03-incremental-b1-static.md`、`docs/reverse/C03-incremental-b1-contracts.md`
  - 成员：台账未登记（unknown），需源码级取证后填入
- [ ] **P0-04** 补 B0 根因证据（需可查询原厂过程或 provider 修复） · 状态 `blocked` · 来源 `c03-next-evidence-gate.json` · 证据 2 项 · 退出 捕获非空 InputROI/InputMask 或生成区域对象、45 元素向量与图像缓冲 · 前置/阻塞 用户环境/授权：原厂可查询过程或 REA provider
  - 证据：`analysis/full-audit/batch1-20261107/C03/c03-next-evidence-gate.json`、`analysis/full-audit/batch1-20261107/C03/rea-native-provider-diagnostic.json`
  - 成员：台账未登记（unknown），需源码级取证后填入
- [ ] **P0-05** 提交计划产物固定共同基线 SHA（需用户明确说提交） · 状态 `blocked` · 来源 `AGENTS.md / 用户硬规则` · 退出 中文提交信息 UTF-8 无 BOM 文件 + git commit -F；只提交计划/站点相关文件 · 前置/阻塞 用户授权：提交
  - 成员：台账未登记（unknown），需源码级取证后填入

## §2 渐进批次 B0–B3（13 步）

按最小纵切 ImageSource→Blob→显示 推进；每步写明证据、成员、命令、报告、退出判据与 unknown。

共 13 项，勾选框全部未预填；状态字段来自机器台账。

- [ ] **INC-B0-1** B0 · 复核差分两侧与 5 项 unknown · 状态 `not_started` · 来源 `incremental-landing.map.json` · 证据 4 项 · 退出 45/971486 与 47/973202 原样并列，不抹平；5 项 unknown 逐项登记解除条件；每个结论标注证据类型 · 锚点 `17-incremental-landing.html#inc-step-B0-1`
  - 证据：`docs/VM-LOCAL-RESTORATION-STATUS.json`、`analysis/full-audit/batch1-20261107/C03/c03-phase1-evidence-card.json`、`analysis/full-audit/batch1-20261107/C03/c03-phase1-evidence-review.json`、`analysis/full-audit/batch1-20261107/C01/runtime-report.json`
  - 成员：`mBlobFind.Results`、`IMVS_PF_BLOBFIND_MODU_INFO`、`BlobFindResult()`
  - 报告：`analysis/full-audit/batch1-20261107/C03/c03-phase1-evidence-review.json`、`analysis/full-audit/batch1-20261107/C03/c03-incremental-batch0-b1.json`
  - unknown：wrapper 侧 FullScreen 转换；SDK Run 前确切输入图像/边缘字节；45 元素结果向量与 BinaryImage/BlobImage 原始缓冲区
- [ ] **INC-B0-2** B0 · 登记 REA 补证边界与解除条件 · 状态 `not_started` · 来源 `incremental-landing.map.json` · 证据 3 项 · 退出 provider 不可用原样登记，不在本阶段修复；证据门「未捕获区域对象/像素缓冲/native 结果数组前不改产品算法」原样保留 · 锚点 `17-incremental-landing.html#inc-step-B0-2`
  - 证据：`analysis/full-audit/batch1-20261107/C03/rea-native-provider-diagnostic.json`、`analysis/full-audit/batch1-20261107/C03/c03-next-evidence-gate.json`、`docs/reverse/C03-phase1-evidence-review.md`
  - 成员：台账未登记（unknown），需源码级取证后填入
  - 报告：`analysis/full-audit/batch1-20261107/C03/c03-next-evidence-gate.json`
  - unknown：ghidra / ida 未配置、hopper 不支持当前主机
- [ ] **INC-B0-3** B0 · 登记 S0.1/S0.2 未重跑与复跑命令 · 状态 `not_started` · 来源 `incremental-landing.map.json` · 证据 3 项 · 退出 历史报告不得冒充本轮复跑；命令只登记待执行，需用户明确说开始测试/验证才运行；退出码与新报告哈希在复跑后补登记 · 锚点 `17-incremental-landing.html#inc-step-B0-3`
  - 证据：`analysis/full-audit/batch1-20261107/C03/c03-phase1-s0-gate-check.json`、`analysis/full-audit/batch1-20261107/C01/runtime-report.json`、`tests/VMVerticalSlice.C01/VMVerticalSlice.C01.csproj`
  - 成员：台账未登记（unknown），需源码级取证后填入
  - 命令〔待执行〕：`dotnet run --project tests/VMVerticalSlice.C01/VMVerticalSlice.C01.csproj --no-restore`
  - 报告：`analysis/full-audit/batch1-20261107/C01/runtime-report.json`
  - unknown：本轮是否复跑需用户另行授权
- [ ] **INC-B1-1** B1 · 图像源入口与输入字节 · 状态 `blocked` · 来源 `incremental-landing.map.json` · 证据 5 项 · 退出 登记图像源入口文件、准确类型与输入字节证据路径；输入文件与哈希可复核；与 C01 现有报告字段对齐 · 前置/阻塞 B0 · 锚点 `17-incremental-landing.html#inc-step-B1-1`
  - 证据：`src/VisionPlugins.ImageSource/ImageSourcePlugin.cs`、`src/VisionPlugins.ImageSource/ImageAcquisitionPlugin.cs`、`analysis/full-audit/batch1-20261107/C01/runtime-report.json`、`analysis/full-audit/batch1-20261107/C01/image-source-vertical-slice.vmx.json`、`docs/reverse/C01-framework-image-source-vertical-slice.md`
  - 成员：`ImageSourcePlugin.CreateRuntime`、`ImageSourcePlugin.GetImages`、`ImageSourcePlugin.ExecuteAsync`、`ImageAcquisitionPlugin.PrepareSave`、`ImageAcquisitionPlugin.ExecuteAsync`
  - 报告：`analysis/full-audit/batch1-20261107/C01/runtime-report.json`、`analysis/full-audit/batch1-20261107/C03/c03-incremental-b1-static.json`
  - unknown：原厂 Run 前输入像素字节未捕获
- [ ] **INC-B1-2** B1 · Blob 模块调用链与结果向量 · 状态 `blocked` · 来源 `incremental-landing.map.json` · 证据 5 项 · 退出 六段链中绑定/命令、状态读写、事件/回调逐段标注；ProcessOutput 与 LimitJudge 结果边界原样保留；不把 47/973202 写成 45/971486 的解释结论 · 前置/阻塞 B0 · 锚点 `17-incremental-landing.html#inc-step-B1-2`
  - 证据：`src/VisionCore/Algorithms.cs`、`src/VisionModules/BuiltinModules.cs`、`src/VisionWorkflow/OfflineCircle.cs`、`analysis/full-audit/batch1-20261107/C01/image-source-blob-vertical-slice.vmx.json`、`analysis/full-audit/batch1-20261107/C03/c03-phase1-evidence-card.json`
  - 成员：`ProcessOutput`、`LimitJudge`、`Algorithms.ConnectedComponents8`、`Algorithms.FitCircle`、`BuiltinModules.RegisterAll`、`BuiltinModules.Adapter.ExecuteAsync`、`OfflineCircleExecutor.Run`
  - 报告：`analysis/full-audit/batch1-20261107/C03/c03-phase1-evidence-card.json`、`analysis/full-audit/batch1-20261107/C03/c03-incremental-b1-static.json`、`analysis/full-audit/batch1-20261107/C03/c03-incremental-b1-contracts.json`
  - unknown：本地侧 Blob 结果向量与 SDK result vtable +0x08/+0x28 的字段映射未逐项闭合；45 元素向量未捕获；9 个 Components.* 复合名的 managed 投影规则已定位（C03-INCREMENTAL-B1-CONTRACTS-001）；原厂 native 字段 ABI 仍未取证
- [ ] **INC-B1-3** B1 · 显示与 overlay 输出 · 状态 `blocked` · 来源 `incremental-landing.map.json` · 证据 4 项 · 退出 登记显示画布、overlay 输出文件与执行器入口；输出名清单与保存/重载状态可复核；不以本地显示一致推断原厂 parity · 前置/阻塞 B0 · 锚点 `17-incremental-landing.html#inc-step-B1-3`
  - 证据：`src/VisionWorkbench/ImageCanvas.cs`、`src/VisionRunner/OverlayPng.cs`、`src/VisionFramework/GraphExecutor.cs`、`analysis/full-audit/batch1-20261107/C01/image-source-group-vertical-slice.vmx.json`
  - 成员：`ImageCanvas.SetImage`、`ImageCanvas.SetOverlay`、`OverlayPng.Write`、`GraphExecutor.RunAsync`
  - 报告：`analysis/full-audit/batch1-20261107/C03/c03-incremental-b1-static.json`、`analysis/full-audit/batch1-20261107/C03/c03-incremental-b1-contracts.json`
  - unknown：Group 输出名 image/width 已定位到方案 Configuration.Outputs（C03-INCREMENTAL-B1-CONTRACTS-001）；其与源码端口名的契约边界仍待原厂差分；三份 vmx 的 Procedures/Resources 差异已登记，行为含义未取证
- [ ] **INC-B1-4** B1 · 纵切闭环复跑登记（待执行） · 状态 `blocked` · 来源 `incremental-landing.map.json` · 证据 2 项 · 退出 命令登记但本阶段不执行；复跑后记录退出码、新报告哈希、会话状态与输出名清单；S0.1/S0.2 状态随复跑结果更新，不提前关闭 · 前置/阻塞 B0 · 锚点 `17-incremental-landing.html#inc-step-B1-4`
  - 证据：`tests/VMVerticalSlice.C01/VMVerticalSlice.C01.csproj`、`analysis/full-audit/batch1-20261107/C01/runtime-report.json`
  - 成员：台账未登记（unknown），需源码级取证后填入
  - 命令〔待执行〕：`dotnet run --project tests/VMVerticalSlice.C01/VMVerticalSlice.C01.csproj --no-restore`
  - 报告：`analysis/full-audit/batch1-20261107/C01/runtime-report.json`
  - unknown：本轮是否复跑需用户明确说开始测试/验证
- [ ] **INC-B2-1** B2 · 工具箱 + 工具加载（4 控件） · 状态 `blocked` · 来源 `incremental-landing.map.json` · 证据 2 项 · 退出 4 控件逐项完成三级验收；未闭合段标 unknown 并给出补证目标 · 前置/阻塞 B0、B1 · 锚点 `17-incremental-landing.html#inc-step-B2-1`
  - 证据：`docs/reverse/A01-loader-batch1.md`、`docs/reverse/A07-managed-loader-partial-batch16.md`
  - 成员：`ToolBoxData.UpdateLoad`、`IMVSGroup.LoadAssemblies`
  - unknown：动态发现与失败分支未取证
- [ ] **INC-B2-2** B2 · 图像显示 + 模块结果（5 控件） · 状态 `blocked` · 来源 `incremental-landing.map.json` · 证据 2 项 · 退出 5 控件逐项完成三级验收；显示对象与 ROI 叠层逐项对账 · 前置/阻塞 B0、B1 · 锚点 `17-incremental-landing.html#inc-step-B2-2`
  - 证据：`docs/reverse/A06-frontend-gate-batch24.md`、`docs/reverse/A-container-zoom-implementation-batch12.md`
  - 成员：台账未登记（unknown），需源码级取证后填入
  - unknown：原厂显示对象类型与叠层次序未逐键对账
- [ ] **INC-B2-3** B2 · 流程画布 + 配置面板（4 控件） · 状态 `blocked` · 来源 `incremental-landing.map.json` · 证据 2 项 · 退出 4 控件逐项完成三级验收；节点/连线/视口与原厂同状态差分通过 · 前置/阻塞 B0、B1 · 锚点 `17-incremental-landing.html#inc-step-B2-3`
  - 证据：`docs/reverse/A03-tabs-batch2.md`、`docs/reverse/A03-canvas-replacement-fix.md`
  - 成员：`DiagramItem`、`Connector.HitTesting`
  - unknown：原厂 ItemsPage(ListView)+DiagramItem+Adorner 控件树差异边界未闭合
- [ ] **INC-B2-4** B2 · 结果区 + 壳控件（4 控件） · 状态 `blocked` · 来源 `incremental-landing.map.json` · 证据 2 项 · 退出 4 控件逐项完成三级验收；E0–E7 八门逐门闭合 · 前置/阻塞 B0、B1 · 锚点 `17-incremental-landing.html#inc-step-B2-4`
  - 证据：`docs/reverse/A05-full-menu-map-batch17.md`、`docs/reverse/A08-shell-input-batch30.md`
  - 成员：台账未登记（unknown），需源码级取证后填入
  - unknown：closedEntryChains 当前为 0
- [ ] **INC-B3-1** B3 · 按 14 分类排批次与阻塞项 · 状态 `blocked` · 来源 `incremental-landing.map.json` · 证据 1 项 · 退出 162 工具全部登记批次与任务卡入口；161 未建立 + 1 partial 状态原样保留 · 前置/阻塞 B0、B1、B2 · 锚点 `17-incremental-landing.html#inc-step-B3-1`
  - 证据：`docs/plugins/inventory.json`
  - 成员：台账未登记（unknown），需源码级取证后填入
  - unknown：共享框架未稳定前不启动工具实现
- [ ] **INC-B3-2** B3 · 逐工具源码级取证与实现 · 状态 `blocked` · 来源 `incremental-landing.map.json` · 证据 1 项 · 退出 每工具身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 前置/阻塞 B0、B1、B2 · 锚点 `17-incremental-landing.html#inc-step-B3-2`
  - 证据：`docs/plugins/inventory.json`
  - 成员：台账未登记（unknown），需源码级取证后填入
  - unknown：逐工具 native 函数体受当前 REA provider 限制

## §3 主界面共享控件（17 项 / 4 类）

每项须完成源码级闭环 + 原厂同状态差分 + 自研操作级复跑；六段链与三级验收详情见第 18 页锚点。

共 17 项，勾选框全部未预填；状态字段来自机器台账。

- [ ] **UI-UI-C01** 左侧竖排工具箱分类图标 · 状态 `blocked` · 来源 `ui-control-landing.map.json` · 证据 2 项 · 退出 源码级闭环：六段链（入口/绑定命令/状态读写/事件回调/持久化或执行效果/异常清理）+ 资源/CIL + 准确类型与成员全部有证据；原厂同状态差分：与截图中原厂同状态、同输入的操作级差分通过；截图只作差异信号，不作规格；自研操作级复跑：在当前构建产物复跑并记… · 前置/阻塞 BLK-S0-C03 · 锚点 `16-ui-control-landing.html#ui-UI-C01`
- [ ] **UI-UI-C02** 工具目录与工具项列表 · 状态 `blocked` · 来源 `ui-control-landing.map.json` · 证据 2 项 · 退出 源码级闭环：六段链（入口/绑定命令/状态读写/事件回调/持久化或执行效果/异常清理）+ 资源/CIL + 准确类型与成员全部有证据；原厂同状态差分：与截图中原厂同状态、同输入的操作级差分通过；截图只作差异信号，不作规格；自研操作级复跑：在当前构建产物复跑并记… · 前置/阻塞 BLK-S0-C03 · 锚点 `16-ui-control-landing.html#ui-UI-C02`
- [ ] **UI-UI-C03** 工具发现 / 注册与装载 · 状态 `blocked` · 来源 `ui-control-landing.map.json` · 证据 2 项 · 退出 源码级闭环：六段链（入口/绑定命令/状态读写/事件回调/持久化或执行效果/异常清理）+ 资源/CIL + 准确类型与成员全部有证据；原厂同状态差分：与截图中原厂同状态、同输入的操作级差分通过；截图只作差异信号，不作规格；自研操作级复跑：在当前构建产物复跑并记… · 前置/阻塞 BLK-S0-C03 · 锚点 `16-ui-control-landing.html#ui-UI-C03`
- [ ] **UI-UI-C04** 缺插件 / 加载失败体验 · 状态 `blocked` · 来源 `ui-control-landing.map.json` · 证据 2 项 · 退出 源码级闭环：六段链（入口/绑定命令/状态读写/事件回调/持久化或执行效果/异常清理）+ 资源/CIL + 准确类型与成员全部有证据；原厂同状态差分：与截图中原厂同状态、同输入的操作级差分通过；截图只作差异信号，不作规格；自研操作级复跑：在当前构建产物复跑并记… · 前置/阻塞 BLK-S0-C03 · 锚点 `16-ui-control-landing.html#ui-UI-C04`
- [ ] **UI-UI-C05** 图像 / 模块结果 Tab 切换 · 状态 `blocked` · 来源 `ui-control-landing.map.json` · 证据 2 项 · 退出 源码级闭环：六段链（入口/绑定命令/状态读写/事件回调/持久化或执行效果/异常清理）+ 资源/CIL + 准确类型与成员全部有证据；原厂同状态差分：与截图中原厂同状态、同输入的操作级差分通过；截图只作差异信号，不作规格；自研操作级复跑：在当前构建产物复跑并记… · 前置/阻塞 BLK-S0-C03 · 锚点 `16-ui-control-landing.html#ui-UI-C05`
- [ ] **UI-UI-C06** 图像显示画布（缩放/平移/ROI 与显示叠层） · 状态 `blocked` · 来源 `ui-control-landing.map.json` · 证据 2 项 · 退出 源码级闭环：六段链（入口/绑定命令/状态读写/事件回调/持久化或执行效果/异常清理）+ 资源/CIL + 准确类型与成员全部有证据；原厂同状态差分：与截图中原厂同状态、同输入的操作级差分通过；截图只作差异信号，不作规格；自研操作级复跑：在当前构建产物复跑并记… · 前置/阻塞 BLK-S0-C03 · 锚点 `16-ui-control-landing.html#ui-UI-C06`
- [ ] **UI-UI-C07** 图像源缩略图列表与自动切换 · 状态 `blocked` · 来源 `ui-control-landing.map.json` · 证据 2 项 · 退出 源码级闭环：六段链（入口/绑定命令/状态读写/事件回调/持久化或执行效果/异常清理）+ 资源/CIL + 准确类型与成员全部有证据；原厂同状态差分：与截图中原厂同状态、同输入的操作级差分通过；截图只作差异信号，不作规格；自研操作级复跑：在当前构建产物复跑并记… · 前置/阻塞 BLK-S0-C03 · 锚点 `16-ui-control-landing.html#ui-UI-C07`
- [ ] **UI-UI-C08** 运行全部 / 翻页 / 布局按钮 · 状态 `blocked` · 来源 `ui-control-landing.map.json` · 证据 2 项 · 退出 源码级闭环：六段链（入口/绑定命令/状态读写/事件回调/持久化或执行效果/异常清理）+ 资源/CIL + 准确类型与成员全部有证据；原厂同状态差分：与截图中原厂同状态、同输入的操作级差分通过；截图只作差异信号，不作规格；自研操作级复跑：在当前构建产物复跑并记… · 前置/阻塞 BLK-S0-C03 · 锚点 `16-ui-control-landing.html#ui-UI-C08`
- [ ] **UI-UI-C09** 模块结果数据绑定 · 状态 `blocked` · 来源 `ui-control-landing.map.json` · 证据 2 项 · 退出 源码级闭环：六段链（入口/绑定命令/状态读写/事件回调/持久化或执行效果/异常清理）+ 资源/CIL + 准确类型与成员全部有证据；原厂同状态差分：与截图中原厂同状态、同输入的操作级差分通过；截图只作差异信号，不作规格；自研操作级复跑：在当前构建产物复跑并记… · 前置/阻塞 BLK-S0-C03 · 锚点 `16-ui-control-landing.html#ui-UI-C09`
- [ ] **UI-UI-C10** 流程 Tab 与 Group 导航 · 状态 `blocked` · 来源 `ui-control-landing.map.json` · 证据 2 项 · 退出 源码级闭环：六段链（入口/绑定命令/状态读写/事件回调/持久化或执行效果/异常清理）+ 资源/CIL + 准确类型与成员全部有证据；原厂同状态差分：与截图中原厂同状态、同输入的操作级差分通过；截图只作差异信号，不作规格；自研操作级复跑：在当前构建产物复跑并记… · 前置/阻塞 BLK-S0-C03 · 锚点 `16-ui-control-landing.html#ui-UI-C10`
- [ ] **UI-UI-C11** 节点卡片 / 连线 / 视口 · 状态 `blocked` · 来源 `ui-control-landing.map.json` · 证据 3 项 · 退出 源码级闭环：六段链（入口/绑定命令/状态读写/事件回调/持久化或执行效果/异常清理）+ 资源/CIL + 准确类型与成员全部有证据；原厂同状态差分：与截图中原厂同状态、同输入的操作级差分通过；截图只作差异信号，不作规格；自研操作级复跑：在当前构建产物复跑并记… · 前置/阻塞 BLK-S0-C03 · 锚点 `16-ui-control-landing.html#ui-UI-C11`
- [ ] **UI-UI-C12** 工具参数配置页与公共配置模型 · 状态 `blocked` · 来源 `ui-control-landing.map.json` · 证据 2 项 · 退出 源码级闭环：六段链（入口/绑定命令/状态读写/事件回调/持久化或执行效果/异常清理）+ 资源/CIL + 准确类型与成员全部有证据；原厂同状态差分：与截图中原厂同状态、同输入的操作级差分通过；截图只作差异信号，不作规格；自研操作级复跑：在当前构建产物复跑并记… · 前置/阻塞 BLK-S0-C03 · 锚点 `16-ui-control-landing.html#ui-UI-C12`
- [ ] **UI-UI-C13** 画布手势 / 右键菜单 / 路由 · 状态 `blocked` · 来源 `ui-control-landing.map.json` · 证据 2 项 · 退出 源码级闭环：六段链（入口/绑定命令/状态读写/事件回调/持久化或执行效果/异常清理）+ 资源/CIL + 准确类型与成员全部有证据；原厂同状态差分：与截图中原厂同状态、同输入的操作级差分通过；截图只作差异信号，不作规格；自研操作级复跑：在当前构建产物复跑并记… · 前置/阻塞 BLK-S0-C03 · 锚点 `16-ui-control-landing.html#ui-UI-C13`
- [ ] **UI-UI-C14** 历史结果 / 帮助 Tab · 状态 `blocked` · 来源 `ui-control-landing.map.json` · 证据 2 项 · 退出 源码级闭环：六段链（入口/绑定命令/状态读写/事件回调/持久化或执行效果/异常清理）+ 资源/CIL + 准确类型与成员全部有证据；原厂同状态差分：与截图中原厂同状态、同输入的操作级差分通过；截图只作差异信号，不作规格；自研操作级复跑：在当前构建产物复跑并记… · 前置/阻塞 BLK-S0-C03 · 锚点 `16-ui-control-landing.html#ui-UI-C14`
- [ ] **UI-UI-C15** 运行序号 / 时间 / 模块数据表格 · 状态 `blocked` · 来源 `ui-control-landing.map.json` · 证据 2 项 · 退出 源码级闭环：六段链（入口/绑定命令/状态读写/事件回调/持久化或执行效果/异常清理）+ 资源/CIL + 准确类型与成员全部有证据；原厂同状态差分：与截图中原厂同状态、同输入的操作级差分通过；截图只作差异信号，不作规格；自研操作级复跑：在当前构建产物复跑并记… · 前置/阻塞 BLK-S0-C03 · 锚点 `16-ui-control-landing.html#ui-UI-C15`
- [ ] **UI-UI-C16** 顶部菜单与工具栏图标 · 状态 `blocked` · 来源 `ui-control-landing.map.json` · 证据 2 项 · 退出 源码级闭环：六段链（入口/绑定命令/状态读写/事件回调/持久化或执行效果/异常清理）+ 资源/CIL + 准确类型与成员全部有证据；原厂同状态差分：与截图中原厂同状态、同输入的操作级差分通过；截图只作差异信号，不作规格；自研操作级复跑：在当前构建产物复跑并记… · 前置/阻塞 BLK-S0-C03 · 锚点 `16-ui-control-landing.html#ui-UI-C16`
- [ ] **UI-UI-C17** 底部状态栏（耗时 / 缩放 / 版本） · 状态 `blocked` · 来源 `ui-control-landing.map.json` · 证据 2 项 · 退出 源码级闭环：六段链（入口/绑定命令/状态读写/事件回调/持久化或执行效果/异常清理）+ 资源/CIL + 准确类型与成员全部有证据；原厂同状态差分：与截图中原厂同状态、同输入的操作级差分通过；截图只作差异信号，不作规格；自研操作级复跑：在当前构建产物复跑并记… · 前置/阻塞 BLK-S0-C03 · 锚点 `16-ui-control-landing.html#ui-UI-C17`

## §4 框架 WBS 工作包（32 项）

逐包挂到波次、依赖、3 个具名单元与退出判据；`done` 等状态来自三层台账，不代表本清单已验收。

共 32 项，勾选框全部未预填；状态字段来自机器台账。

- [ ] **WP-A01** A01 · 工具加载与Group导入时序 · 状态 `implementation` · 来源 `framework-reverse-plan.json / framework-progress-audit.json` · 退出 事件顺序逐点匹配源码；缺Cs与构造失败分列；主宿主接入前合同明确 · 锚点 `11-work-packages.html#wp-A01`
- [ ] **WP-A02** A02 · 主窗口状态与DPI · 状态 `self_review` · 来源 `framework-reverse-plan.json / framework-progress-audit.json` · 退出 原资源和几何公式可定位；实际桌面矩阵与独立策略分列 · 前置/阻塞 A01 · 锚点 `11-work-packages.html#wp-A02`
- [ ] **WP-A03** A03 · 流程Tab与Group导航 · 状态 `blocked` · 来源 `framework-reverse-plan.json / framework-progress-audit.json` · 退出 身份/集合/导航链闭合；所有操作保持正确所属流程；导出入口另列 · 锚点 `11-work-packages.html#wp-A03`
- [ ] **WP-A04** A04 · 节点手势、路由和视口 · 状态 `done` · 来源 `framework-reverse-plan.json / framework-progress-audit.json` · 退出 每分支有公式与状态证据；桌面输入和宿主事件分别验收；极端坐标无静默偏差 · 前置/阻塞 A03 · 锚点 `11-work-packages.html#wp-A04`
- [ ] **WP-A05** A05 · 菜单、工具栏及全局入口清点 · 状态 `implementation` · 来源 `framework-reverse-plan.json / framework-progress-audit.json` · 退出 每个入口有任务归属和覆盖状态，不能用仅打开窗口结项；新增服务按父任务拆子项 · 前置/阻塞 A02 · 锚点 `11-work-packages.html#wp-A05`
- [ ] **WP-A06** A06 · 运行界面编辑与显示布局 · 状态 `evidence` · 来源 `framework-reverse-plan.json / framework-progress-audit.json` · 退出 布局与真实结果绑定闭合；未知控件单列；插件显示只消费交付合同 · 前置/阻塞 A05、B06 · 锚点 `11-work-packages.html#wp-A06`
- [ ] **WP-A07** A07 · 通用发现接入与缺插件体验 · 状态 `self_review` · 来源 `framework-reverse-plan.json / framework-progress-audit.json` · 退出 至少一个真实插件消费；失败无悬挂节点；不扫描执行未知DLL；共享入口锁解除后实施 · 前置/阻塞 A01、B07 · 锚点 `11-work-packages.html#wp-A07`
- [ ] **WP-A08** A08 · 壳与交互全状态验收 · 状态 `implementation` · 来源 `framework-reverse-plan.json / framework-progress-audit.json` · 退出 列出的壳范围无遗漏；未支持服务显式开放，不按测试总数关闭领域 · 前置/阻塞 A02、A03、A04、A05、A06、A07 · 锚点 `11-work-packages.html#wp-A08`
- [ ] **WP-B01** B01 · 实际运行Tab与变量目标 · 状态 `evidence` · 来源 `framework-reverse-plan.json / framework-progress-audit.json` · 退出 原始类型及谓词逐项匹配目标树；更新FCR-019字段和生命周期，不凭CustomVisible推断 · 锚点 `11-work-packages.html#wp-B01`
- [ ] **WP-B02** B02 · Group身份、所有权和复制 · 状态 `implementation` · 来源 `framework-reverse-plan.json / framework-progress-audit.json` · 退出 节点/代理/引用/内部图始终指向正确实例；独立跨顶层兼容保留标签 · 前置/阻塞 B01、A01 · 锚点 `11-work-packages.html#wp-B02`
- [ ] **WP-B03** B03 · Group循环、中断与每轮生命周期 · 状态 `self_review` · 来源 `framework-reverse-plan.json / framework-progress-audit.json` · 退出 源码算术/回调/状态变更可定位；本地long和上限策略不冒充native；运行输出与状态同验 · 前置/阻塞 B02、C02 · 锚点 `11-work-packages.html#wp-B03`
- [ ] **WP-B04** B04 · 原生类型、复合叶及累积 · 状态 `implementation` · 来源 `framework-reverse-plan.json / framework-progress-audit.json` · 退出 每种开放类型有原生产者→订阅→Group→存储→结果证据；缺失类型禁用或明确独立合同 · 前置/阻塞 B03 · 锚点 `11-work-packages.html#wp-B04`
- [ ] **WP-B05** B05 · 流程IO、局部/全局变量与gvar · 状态 `implementation` · 来源 `framework-reverse-plan.json / framework-progress-audit.json` · 退出 入口/身份/持久化/执行闭合；全局与局部不混用；未知gvar不改名JSON代替 · 前置/阻塞 B01、B04 · 锚点 `11-work-packages.html#wp-B05`
- [ ] **WP-B06** B06 · Group配置、ROI和结果消费边界 · 状态 `implementation` · 来源 `framework-reverse-plan.json / framework-progress-audit.json` · 退出 原始候选/写入/执行可核对；实际ROI跨Group运行和保存重开；不另造插件配置模板 · 前置/阻塞 B01、B04、C01 · 锚点 `11-work-packages.html#wp-B06`
- [ ] **WP-B07** B07 · 插件交接合同与真实消费 · 状态 `implementation` · 来源 `framework-reverse-plan.json / framework-progress-audit.json` · 退出 明确实际采用SHA与合同；待集成和已实现分开；不搬未提交文件 · 前置/阻塞 B04、B05、B06 · 锚点 `11-work-packages.html#wp-B07`
- [ ] **WP-B08** B08 · 模型与Group组合验收 · 状态 `implementation` · 来源 `framework-reverse-plan.json / framework-progress-audit.json` · 退出 每场景对照原机输出和状态；失败不读陈旧结果；组合未知仍开放 · 前置/阻塞 B02、B03、B04、B05、B06、B07 · 锚点 `11-work-packages.html#wp-B08`
- [ ] **WP-C01** C01 · 撤销root/part实际捕获 · 状态 `implementation` · 来源 `framework-reverse-plan.json / framework-progress-audit.json` · 退出 CIL/反编译与真实命中相互对应；临时文件原件保全；不构造服务清理他任务TempFiles · 锚点 `11-work-packages.html#wp-C01`
- [ ] **WP-C02** C02 · 单次/连续/停止调度与参数准备 · 状态 `implementation` · 来源 `framework-reverse-plan.json / framework-progress-audit.json` · 退出 不是只对按钮状态；确定每轮清理配置有效值及结果；设备路径缺证据单列 · 锚点 `11-work-packages.html#wp-C02`
- [ ] **WP-C03** C03 · 结果历史与方案替换生命周期 · 状态 `implementation` · 来源 `framework-reverse-plan.json / framework-progress-audit.json` · 退出 配置默认及入口有来源；失败加载状态可核对；旧方案回调不污染新方案 · 前置/阻塞 C02、D02 · 锚点 `11-work-packages.html#wp-C03`
- [ ] **WP-C04** C04 · 撤销恢复与临时文件所有权 · 状态 `implementation` · 来源 `framework-reverse-plan.json / framework-progress-audit.json` · 退出 数据恢复、栈显示和文件所有权分别验收；异常假设不当通用回滚；按根类型选择本地适配 · 前置/阻塞 C01、D01 · 锚点 `11-work-packages.html#wp-C04`
- [ ] **WP-C05** C05 · 配置/调试会话与框架释放 · 状态 `implementation` · 来源 `framework-reverse-plan.json / framework-progress-audit.json` · 退出 只改main消费与生命周期；插件会话由插件线交付；无悬挂旧回调或误杀他任务进程 · 前置/阻塞 C02、B07 · 锚点 `11-work-packages.html#wp-C05`
- [ ] **WP-C06** C06 · 外部服务调度与故障隔离 · 状态 `blocked` · 来源 `framework-reverse-plan.json / framework-progress-audit.json` · 退出 每服务有显式证据/合同/错误和资源边界；没有设备证据不得标硬件闭合；不默认操作连接设备 · 前置/阻塞 A05、C02、C03 · 锚点 `11-work-packages.html#wp-C06`
- [ ] **WP-C07** C07 · 长稳、并发与异常恢复 · 状态 `not_started` · 来源 `framework-reverse-plan.json / framework-progress-audit.json` · 退出 测量窗口与阈值有依据；无无界增长/悬挂任务；失败记录不删除；不虚报全平台无泄漏 · 前置/阻塞 C03、C04、C05、C06 · 锚点 `11-work-packages.html#wp-C07`
- [ ] **WP-C08** C08 · 执行与历史组合验收 · 状态 `implementation` · 来源 `framework-reverse-plan.json / framework-progress-audit.json` · 退出 每步结果/历史/文件/资源同时核对；独立重构差异显式保留 · 前置/阻塞 C02、C03、C04、C05、C06、C07、B08 · 锚点 `11-work-packages.html#wp-C08`
- [ ] **WP-D01** D01 · sol单操作保存与native写入点 · 状态 `blocked` · 来源 `framework-reverse-plan.json / framework-progress-audit.json` · 退出 变化记录可归因具体操作和写入点；mdata未知字段不猜；不把外层解析器作为产品导入器 · 锚点 `11-work-packages.html#wp-D01`
- [ ] **WP-D02** D02 · 独立存储与资源事务补齐 · 状态 `implementation` · 来源 `framework-reverse-plan.json / framework-progress-audit.json` · 退出 每类失败文件和编辑器状态有证据；明确独立格式，不追求与sol字节一致 · 锚点 `11-work-packages.html#wp-D02`
- [ ] **WP-D03** D03 · 原生容器与对象数据映射 · 状态 `blocked` · 来源 `framework-reverse-plan.json / framework-progress-audit.json` · 退出 每已识别字段有读写链和差分；原始未知字节保留；不凭相似编码命名格式 · 前置/阻塞 D01、B02、B05 · 锚点 `11-work-packages.html#wp-D03`
- [ ] **WP-D04** D04 · 版本、密码和迁移 · 状态 `evidence` · 来源 `framework-reverse-plan.json / framework-progress-audit.json` · 退出 每格式/版本/密码组合有样本和运行记录；缺样本标等待；不运行会终止他任务的升级路径 · 前置/阻塞 D03 · 锚点 `11-work-packages.html#wp-D04`
- [ ] **WP-D05** D05 · 原生兼容最小实现与往返 · 状态 `blocked` · 来源 `framework-reverse-plan.json / framework-progress-audit.json` · 退出 按格式/版本逐格关闭；数值/引用/资源/执行均匹配；范围外明确拒绝；密码与私有插件不能假完成 · 前置/阻塞 D03、D04、B07 · 锚点 `11-work-packages.html#wp-D05`
- [ ] **WP-D06** D06 · SDK/API/ABI与部署依赖 · 状态 `implementation` · 来源 `framework-reverse-plan.json / framework-progress-audit.json` · 退出 逐导出和结构有证据；不将工具全库算法纳入main；干净机器依赖清单可复现 · 前置/阻塞 A07、B07、D04 · 锚点 `11-work-packages.html#wp-D06`
- [ ] **WP-D07** D07 · 真实工具集成及干净环境验收 · 状态 `implementation` · 来源 `framework-reverse-plan.json / framework-progress-audit.json` · 退出 采用SHA明确；用户桌面可见窗口、实际运行、保存重开、退出释放有证据；未交付工具不计覆盖 · 前置/阻塞 D05、D06、A08、B08、C08 · 锚点 `11-work-packages.html#wp-D07`
- [ ] **WP-D08** D08 · 全域缺口复查与交付 · 状态 `blocked` · 来源 `framework-reverse-plan.json / framework-progress-audit.json` · 退出 全部必需任务及子任务关闭且证据可复验才声明相应范围完成；任何未知仍保留未完成 · 前置/阻塞 D07 · 锚点 `11-work-packages.html#wp-D08`

## §5 框架单元任务卡（96 项）

按工作包分组，逐单元核对源码/实施/验证三层状态与取证目标。

共 96 项，勾选框全部未预填；状态字段来自机器台账。

- [ ] **UNIT-1** A01.1 · 目录XML、分类优先级与惰性元数据读取 · 状态 `done` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-1`
- [ ] **UNIT-2** A01.2 · 原加载器首次/悬停/刷新及失败分支 · 状态 `self_review` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-2`
- [ ] **UNIT-3** A01.3 · 原Group导入回调到模型落地 · 状态 `blocked` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 ORIGINAL-INSTANCE · 锚点 `12-unit-cards.html#unitcard-3`
- [ ] **UNIT-4** A02.1 · 标题拖动、最大化与恢复的命令状态 · 状态 `done` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-4`
- [ ] **UNIT-5** A02.2 · 工作区锁定、确认及解锁 · 状态 `done` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-5`
- [ ] **UNIT-6** A02.3 · 真实桌面100/150/200%DPI与窗口边界矩阵 · 状态 `self_review` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 ACCEPTANCE-PREREQUISITES · 锚点 `12-unit-cards.html#unitcard-6`
- [ ] **UNIT-7** A03.1 · 流程增删复制切换和Tab归属 · 状态 `done` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-7`
- [ ] **UNIT-8** A03.2 · 嵌套Group进入/返回和显示模型归属 · 状态 `done` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-8`
- [ ] **UNIT-9** A03.3 · 每显示容器缩放持久化及非活动Tab历史 · 状态 `self_review` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 ORIGINAL-INSTANCE · 锚点 `12-unit-cards.html#unitcard-9`
- [ ] **UNIT-10** A04.1 · 节点拖放、多选、实时源复制和插入手势 · 状态 `done` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-10`
- [ ] **UNIT-11** A04.2 · 连线中点、插线、固定网格寻路及缓存 · 状态 `done` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-11`
- [ ] **UNIT-12** A04.3 · 视口缩放、小地图和修饰键事件路径 · 状态 `done` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-12`
- [ ] **UNIT-13** A05.1 · 完整菜单/工具栏/快捷键入口清点 · 状态 `implementation` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-13`
- [ ] **UNIT-14** A05.2 · 常规文件、帮助、路径与运行策略命令数据链 · 状态 `done` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-14`
- [ ] **UNIT-15** A05.3 · 全局变量、通信、触发、脚本等专用服务 · 状态 `blocked` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 MISSING-CAPABILITY · 锚点 `12-unit-cards.html#unitcard-15`
- [ ] **UNIT-16** A06.1 · 运行界面编辑入口、窗口状态与关闭流程 · 状态 `evidence` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-16`
- [ ] **UNIT-17** A06.2 · Frontend完整布局、资源、BAML绑定还原 · 状态 `evidence` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-17`
- [ ] **UNIT-18** A06.3 · 运行界面结果绑定、持久化与重开 · 状态 `blocked` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 MISSING-CAPABILITY · 锚点 `12-unit-cards.html#unitcard-18`
- [ ] **UNIT-19** A07.1 · 发现目录到真实ModuleRegistry的动态映射 · 状态 `self_review` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-19`
- [ ] **UNIT-20** A07.2 · 缺插件、版本冲突及刷新失败处理 · 状态 `self_review` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-20`
- [ ] **UNIT-21** A07.3 · 既有真实插件注册到根流程/Group消费 · 状态 `done` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-21`
- [ ] **UNIT-22** A08.1 · 已实现壳/画布/导航的控件事件回归交付 · 状态 `done` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-22`
- [ ] **UNIT-23** A08.2 · 真实键鼠、弹层、DPI及原机同状态矩阵 · 状态 `implementation` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 FAIL-SHELL-INPUT · 锚点 `12-unit-cards.html#unitcard-23`
- [ ] **UNIT-24** A08.3 · 发布版全部壳状态的综合差分 · 状态 `implementation` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-24`
- [ ] **UNIT-25** B01.1 · 真实运行Tab根、XmlLoaded和惰性装配 · 状态 `implementation` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-25`
- [ ] **UNIT-26** B01.2 · 候选类型、占用、首次/缓存及确认取消 · 状态 `implementation` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-26`
- [ ] **UNIT-27** B01.3 · 全局变量目标绑定写入及保存重开 · 状态 `blocked` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 ORIGINAL-INSTANCE · 锚点 `12-unit-cards.html#unitcard-27`
- [ ] **UNIT-28** B02.1 · main Group内部图创建、Parent及所属关系 · 状态 `done` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-28`
- [ ] **UNIT-29** B02.2 · main实时源复制、内部图和代理引用重映射 · 状态 `done` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-29`
- [ ] **UNIT-30** B02.3 · 原gro导入ID/GUID及原生复制持久化 · 状态 `blocked` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 ORIGINAL-INSTANCE · 锚点 `12-unit-cards.html#unitcard-30`
- [ ] **UNIT-31** B03.1 · LoopStep、范围与零值拒绝的当前重建 · 状态 `done` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-31`
- [ ] **UNIT-32** B03.2 · 中断比较、动态索引及执行前准备 · 状态 `done` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-32`
- [ ] **UNIT-33** B03.3 · 每轮真实实例、回调与异常取消生命周期 · 状态 `self_review` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 ORIGINAL-INSTANCE · 锚点 `12-unit-cards.html#unitcard-33`
- [ ] **UNIT-34** B04.1 · 基础标量/数组、基数与类型拒绝 · 状态 `done` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-34`
- [ ] **UNIT-35** B04.2 · 有证复合叶投影与Point/Circle/Fixture传输 · 状态 `done` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-35`
- [ ] **UNIT-36** B04.3 · 全部原生类型、pointset/POLYGON/Region ABI · 状态 `implementation` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 PLUGIN-CONTRACT · 锚点 `12-unit-cards.html#unitcard-36`
- [ ] **UNIT-37** B05.1 · 流程IO和局部变量的作用域、订阅与改型 · 状态 `done` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-37`
- [ ] **UNIT-38** B05.2 · 原全局变量模块的创建、服务与执行消费 · 状态 `blocked` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 MISSING-CAPABILITY · 锚点 `12-unit-cards.html#unitcard-38`
- [ ] **UNIT-39** B05.3 · 原gvar导入导出和往返 · 状态 `blocked` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 MISSING-CAPABILITY · 锚点 `12-unit-cards.html#unitcard-39`
- [ ] **UNIT-40** B06.1 · Group候选资格、类别和代理多目标同步 · 状态 `implementation` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-40`
- [ ] **UNIT-41** B06.2 · 原ROI/专用参数模板及写回消费 · 状态 `implementation` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-41`
- [ ] **UNIT-42** B06.3 · 当前Group显示关联、样式及结果数据消费 · 状态 `done` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-42`
- [ ] **UNIT-43** B07.1 · 既有插件合同、能力及边界登记 · 状态 `implementation` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-43`
- [ ] **UNIT-44** B07.2 · 新插件公共框架的提交级交接与接口整合 · 状态 `blocked` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 PLUGIN-CONTRACT · 锚点 `12-unit-cards.html#unitcard-44`
- [ ] **UNIT-45** B07.3 · main既有真实插件合同的根流程/Group消费 · 状态 `done` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-45`
- [ ] **UNIT-46** B08.1 · 已支持Group嵌套/局部变量/图像组合用例 · 状态 `done` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-46`
- [ ] **UNIT-47** B08.2 · 全类型/完整ROI/代理的组合验收 · 状态 `implementation` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-47`
- [ ] **UNIT-48** B08.3 · 原机混合故障/历史/持久化同状态差分 · 状态 `blocked` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 ACCEPTANCE-PREREQUISITES · 锚点 `12-unit-cards.html#unitcard-48`
- [ ] **UNIT-49** C01.1 · root/part、旧新值与普通动作撤销 · 状态 `self_review` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-49`
- [ ] **UNIT-50** C01.2 · Group配置期禁止记录及异常复位合同 · 状态 `self_review` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 FRAMEWORK-UNDO · 锚点 `12-unit-cards.html#unitcard-50`
- [ ] **UNIT-51** C01.3 · 真实Group参数采集与SDK Undo/Redo · 状态 `blocked` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 ORIGINAL-INSTANCE · 锚点 `12-unit-cards.html#unitcard-51`
- [ ] **UNIT-52** C02.1 · 当前单次/连续/停止及失败状态执行 · 状态 `done` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-52`
- [ ] **UNIT-53** C02.2 · 原参数准备、等待与超时边界消费 · 状态 `implementation` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-53`
- [ ] **UNIT-54** C02.3 · 清结果配置、回调队列及消费者状态 · 状态 `self_review` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-54`
- [ ] **UNIT-55** C03.1 · 原结果视图切源、缓存与旧事件解绑 · 状态 `self_review` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-55`
- [ ] **UNIT-56** C03.2 · 清结果有效配置到实际SDK消费 · 状态 `implementation` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 ORIGINAL-INSTANCE · 锚点 `12-unit-cards.html#unitcard-56`
- [ ] **UNIT-57** C03.3 · 历史、底部耗时与全局结果缓存生命周期 · 状态 `implementation` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-57`
- [ ] **UNIT-58** C04.1 · 各根类型数据回放与栈迁移 · 状态 `implementation` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-58`
- [ ] **UNIT-59** C04.2 · 原TempFiles载荷所有权、淘汰和释放 · 状态 `self_review` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 ORIGINAL-INSTANCE · 锚点 `12-unit-cards.html#unitcard-59`
- [ ] **UNIT-60** C04.3 · 密码取消、恢复栈和异常边界 · 状态 `blocked` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 ORIGINAL-INSTANCE · 锚点 `12-unit-cards.html#unitcard-60`
- [ ] **UNIT-61** C05.1 · 普通节点调试单次/连续/停止/关闭等待 · 状态 `done` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-61`
- [ ] **UNIT-62** C05.2 · 换方案/配置并发关闭与回调解除 · 状态 `self_review` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-62`
- [ ] **UNIT-63** C05.3 · 原SDK/设备取消、回调和资源释放 · 状态 `blocked` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 DEVICE-RELEASE · 锚点 `12-unit-cards.html#unitcard-63`
- [ ] **UNIT-64** C06.1 · 外部触发、脚本、通信服务入口与消费 · 状态 `blocked` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 MISSING-CAPABILITY · 锚点 `12-unit-cards.html#unitcard-64`
- [ ] **UNIT-65** C06.2 · 外部服务调度顺序、故障隔离与恢复 · 状态 `blocked` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 MISSING-CAPABILITY · 锚点 `12-unit-cards.html#unitcard-65`
- [ ] **UNIT-66** C06.3 · 真实设备连接、停止与关闭释放 · 状态 `blocked` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 DEVICE-RELEASE · 锚点 `12-unit-cards.html#unitcard-66`
- [ ] **UNIT-67** C07.1 · 长稳负载/时长及内存线程句柄基线 · 状态 `implementation` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 FAIL-STABILITY-SAVEAS · 锚点 `12-unit-cards.html#unitcard-67`
- [ ] **UNIT-68** C07.2 · 持续并发运行与资源增长验收 · 状态 `evidence` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-68`
- [ ] **UNIT-69** C07.3 · 运行异常、失败后重启和关闭恢复 · 状态 `implementation` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-69`
- [ ] **UNIT-70** C08.1 · 当前运行-编辑-撤销-保存重开组合 · 状态 `done` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-70`
- [ ] **UNIT-71** C08.2 · 嵌套错误、取消和历史释放组合 · 状态 `self_review` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-71`
- [ ] **UNIT-72** C08.3 · 原机执行/历史/资源的综合同状态验收 · 状态 `blocked` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 ORIGINAL-INSTANCE · 锚点 `12-unit-cards.html#unitcard-72`
- [ ] **UNIT-73** D01.1 · 保存加载托管入口、回调及完成标志 · 状态 `evidence` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-73`
- [ ] **UNIT-74** D01.2 · 8个原样本外层记录边界的只读解析 · 状态 `done` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-74`
- [ ] **UNIT-75** D01.3 · native writer/reader与单操作字段归因 · 状态 `blocked` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 NATIVE-FORMAT · 锚点 `12-unit-cards.html#unitcard-75`
- [ ] **UNIT-76** D02.1 · 独立JSON结构/版本/大小/深度与拒绝 · 状态 `done` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-76`
- [ ] **UNIT-77** D02.2 · 资产嵌入、跨目录和多层历史身份 · 状态 `implementation` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 FAIL-ASSET-HISTORY · 锚点 `12-unit-cards.html#unitcard-77`
- [ ] **UNIT-78** D02.3 · 独立写入失败事务与受控进程中断 · 状态 `done` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-78`
- [ ] **UNIT-79** D03.1 · 原XML/UI关系、身份和字段映射 · 状态 `implementation` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-79`
- [ ] **UNIT-80** D03.2 · mdata参数/模型/资源内部语义 · 状态 `blocked` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 NATIVE-FORMAT · 锚点 `12-unit-cards.html#unitcard-80`
- [ ] **UNIT-81** D03.3 · 原未知字段保持和对象图往返 · 状态 `blocked` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 NATIVE-FORMAT · 锚点 `12-unit-cards.html#unitcard-81`
- [ ] **UNIT-82** D04.1 · CompatibleTool托管版本转换调度 · 状态 `implementation` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-82`
- [ ] **UNIT-83** D04.2 · 版本依赖、缺失插件及转换失败恢复 · 状态 `blocked` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 NATIVE-FORMAT · 锚点 `12-unit-cards.html#unitcard-83`
- [ ] **UNIT-84** D04.3 · 旧版原样本迁移、执行与再保存 · 状态 `blocked` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 NATIVE-FORMAT · 锚点 `12-unit-cards.html#unitcard-84`
- [ ] **UNIT-85** D05.1 · 产品原生容器读取导入 · 状态 `blocked` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 NATIVE-FORMAT · 锚点 `12-unit-cards.html#unitcard-85`
- [ ] **UNIT-86** D05.2 · 产品原生容器写入导出 · 状态 `blocked` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 NATIVE-FORMAT · 锚点 `12-unit-cards.html#unitcard-86`
- [ ] **UNIT-87** D05.3 · 原厂打开/执行/再保存最小往返 · 状态 `blocked` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 NATIVE-FORMAT · 锚点 `12-unit-cards.html#unitcard-87`
- [ ] **UNIT-88** D06.1 · 原SDK导出、结构体与ABI消费合同 · 状态 `implementation` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-88`
- [ ] **UNIT-89** D06.2 · 当前win-x64子集打包启动和退出 · 状态 `self_review` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-89`
- [ ] **UNIT-90** D06.3 · 干净机器全部依赖/设备SDK验收 · 状态 `blocked` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 ACCEPTANCE-PREREQUISITES · 锚点 `12-unit-cards.html#unitcard-90`
- [ ] **UNIT-91** D07.1 · 用户选定新插件提交的逐文件交接 · 状态 `blocked` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 PLUGIN-CONTRACT · 锚点 `12-unit-cards.html#unitcard-91`
- [ ] **UNIT-92** D07.2 · main历史真实工具的宿主端到端 · 状态 `done` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-92`
- [ ] **UNIT-93** D07.3 · 最终干净发布、安装与退出完整矩阵 · 状态 `implementation` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-93`
- [ ] **UNIT-94** D08.1 · F-01至F-23原始入口/分支与32包全域对账 · 状态 `implementation` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-94`
- [ ] **UNIT-95** D08.2 · 证据、失败、FCR及验收源的最终一致性 · 状态 `implementation` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 锚点 `12-unit-cards.html#unitcard-95`
- [ ] **UNIT-96** D08.3 · 全部必需项收口及最终兼容声明 · 状态 `blocked` · 来源 `framework-progress-audit.json` · 证据 3 项 · 退出 源码/实施/验证三层状态逐层闭合 · 前置/阻塞 ACCEPTANCE-PREREQUISITES · 锚点 `12-unit-cards.html#unitcard-96`

## §6 目录工具任务卡（162 项 / 14 分类）

全部为逆向实现目标；逐工具身份、资源哈希、参数 IO、Process native、运行对比与 unknown 闭合。

共 162 项，勾选框全部未预填；状态字段来自机器台账。

- [ ] **TOOL-1** CalculatorModule · 变量计算 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-1`
- [ ] **TOOL-2** CoordinateTransform · 坐标转换 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-2`
- [ ] **TOOL-3** IMVSCalibTransformModu · 标定转换 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-3`
- [ ] **TOOL-4** IMVSLineAlignModu · 线对位 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-4`
- [ ] **TOOL-5** IMVSMultiPointAlignModu · 点集对位 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-5`
- [ ] **TOOL-6** IMVSRotateCalculateModu · 旋转计算 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-6`
- [ ] **TOOL-7** IMVSScaleTransformModu · 单位转换 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-7`
- [ ] **TOOL-8** IMVSSinglePointAlignModu · 单点对位 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-8`
- [ ] **TOOL-9** SinglePointGrabModu · 单点抓取 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-9`
- [ ] **TOOL-10** SinglePointMapAlignModu · 单点映射对位 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-10`
- [ ] **TOOL-11** SinglePointRectifyModu · 单点纠偏 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-11`
- [ ] **TOOL-12** CoordinateModule · 坐标系 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-12`
- [ ] **TOOL-13** IMVSCalibBoardCalibModu · 标定板标定 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-13`
- [ ] **TOOL-14** IMVSCameraMapModu · 相机映射 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-14`
- [ ] **TOOL-15** IMVSImageCalibModu · 畸变标定 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-15`
- [ ] **TOOL-16** IMVSMapCalibModu · 映射标定 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-16`
- [ ] **TOOL-17** IMVSNImageCalibModu · N图像标定 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-17`
- [ ] **TOOL-18** IMVSNPointCalibModu · N点标定 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-18`
- [ ] **TOOL-19** ReadCalibFileModu · 标定加载 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-19`
- [ ] **TOOL-20** RotateCalibModu · 旋转标定 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-20`
- [ ] **TOOL-21** TranslationCalibModu · 平移旋转标定 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-21`
- [ ] **TOOL-22** ImageAcquisitionModule · 多图采集 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-22`
- [ ] **TOOL-23** ImageBufferModule · 缓存图像 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-23`
- [ ] **TOOL-24** ImageSourceModule · 图像源 · 状态 `partial` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-24`
- [ ] **TOOL-25** LightModule · 光源 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-25`
- [ ] **TOOL-26** SaveImage · 输出图像 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-26`
- [ ] **TOOL-27** IMVSColorExtractModu · 颜色抽取 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-27`
- [ ] **TOOL-28** IMVSColorImageGenerationModu · 彩图生成 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-28`
- [ ] **TOOL-29** IMVSColorMeasureModu · 颜色测量 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-29`
- [ ] **TOOL-30** IMVSColorRecognitionModu · 颜色识别 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-30`
- [ ] **TOOL-31** IMVSColorSegmentModu · 颜色分割 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-31`
- [ ] **TOOL-32** IMVSColorTransformModu · 颜色转换 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-32`
- [ ] **TOOL-33** CameraIOModule · 相机IO通信 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-33`
- [ ] **TOOL-34** DataAnalysisModule · 协议解析 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-34`
- [ ] **TOOL-35** DataAssembleModule · 协议组装 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-35`
- [ ] **TOOL-36** ReadDatasModule · 接收数据 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-36`
- [ ] **TOOL-37** SendDatasModule · 发送数据 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-37`
- [ ] **TOOL-38** IMVSCircleEdgeInspModu · 圆弧边缘缺陷检测 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-38`
- [ ] **TOOL-39** IMVSCircleEdgePairInspModu · 圆弧对缺陷检测 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-39`
- [ ] **TOOL-40** IMVSEdgeFlawInspModu · 边缘模型缺陷检测 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-40`
- [ ] **TOOL-41** IMVSEdgeInspGroupModu · 边缘组合缺陷检测 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-41`
- [ ] **TOOL-42** IMVSEdgePairFlawInspModu · 边缘对模型缺陷检测 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-42`
- [ ] **TOOL-43** IMVSEdgePairInspGroupModu · 边缘对组合缺陷检测 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-43`
- [ ] **TOOL-44** IMVSEdgePairPosTrendAnalyModu · 边缘对位置趋势分析 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-44`
- [ ] **TOOL-45** IMVSEdgePosTrendAnalyModu · 边缘位置趋势分析 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-45`
- [ ] **TOOL-46** IMVSInspectModu · 异常检测 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-46`
- [ ] **TOOL-47** IMVSLineEdgeInspModu · 直线边缘缺陷检测 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-47`
- [ ] **TOOL-48** IMVSLineEdgePairInspModu · 直线对缺陷检测 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-48`
- [ ] **TOOL-49** IMVSMarkInspModu · IMVSMarkInspModu · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-49`
- [ ] **TOOL-50** IMVSMarkInspModuVA · 字符缺陷检测 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-50`
- [ ] **TOOL-51** IMVSSurfaceDefectFilterModu · 表面缺陷滤波 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-51`
- [ ] **TOOL-52** CommDeviceModule · CommDeviceModule · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-52`
- [ ] **TOOL-53** CommManagerModule · CommManagerModule · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-53`
- [ ] **TOOL-54** DataQueueModule · DataQueueModule · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-54`
- [ ] **TOOL-55** DynamicParamCameraModule · DynamicParamCameraModule · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-55`
- [ ] **TOOL-56** GlobalCameraModule · GlobalCameraModule · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-56`
- [ ] **TOOL-57** GlobalTriggerModule · GlobalTriggerModule · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-57`
- [ ] **TOOL-58** GlobalVariableModule · GlobalVariableModule · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-58`
- [ ] **TOOL-59** ImageCollectDalsaModu · ImageCollectDalsaModu · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-59`
- [ ] **TOOL-60** ImageCollectHikModu · ImageCollectHikModu · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-60`
- [ ] **TOOL-61** ImageCollectIKModu · ImageCollectIKModu · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-61`
- [ ] **TOOL-62** ImageCollectModu · ImageCollectModu · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-62`
- [ ] **TOOL-63** LightControl · LightControl · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-63`
- [ ] **TOOL-64** LocalVariableModule · LocalVariableModule · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-64`
- [ ] **TOOL-65** ProcedureComm · ProcedureComm · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-65`
- [ ] **TOOL-66** ProcedureModule · ProcedureModule · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-66`
- [ ] **TOOL-67** SCGlobalCameraModule · SCGlobalCameraModule · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-67`
- [ ] **TOOL-68** SolutionModule · SolutionModule · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-68`
- [ ] **TOOL-69** GeometryCreate · 几何创建 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-69`
- [ ] **TOOL-70** IMVSCircleFitModu · 圆拟合 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-70`
- [ ] **TOOL-71** IMVSEllipseFitModu · 椭圆拟合 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-71`
- [ ] **TOOL-72** IMVSLineFitModu · 直线拟合 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-72`
- [ ] **TOOL-73** IMVSAffineTransformModu · 仿射变换 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-73`
- [ ] **TOOL-74** IMVSBinaryModu · 图像二值化 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-74`
- [ ] **TOOL-75** IMVSFrameMeanModu · 帧平均 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-75`
- [ ] **TOOL-76** IMVSGeometricTransformModu · 几何变换 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-76`
- [ ] **TOOL-77** IMVSImageCombineProcessModu · 图像组合 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-77`
- [ ] **TOOL-78** IMVSImageCorrectCalibModu · 畸变校正 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-78`
- [ ] **TOOL-79** IMVSImageCorrectManualModu · 图像矫正 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-79`
- [ ] **TOOL-80** IMVSImageEnhanceModu · 图像增强 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-80`
- [ ] **TOOL-81** IMVSImageFilterModu · 图像滤波 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-81`
- [ ] **TOOL-82** IMVSImageFixtureModu · 图像修正 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-82`
- [ ] **TOOL-83** IMVSImageMathModu · 图像运算 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-83`
- [ ] **TOOL-84** IMVSImageMorphModu · 形态学处理 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-84`
- [ ] **TOOL-85** IMVSImageNormlizeModu · 图像归一化 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-85`
- [ ] **TOOL-86** IMVSImageResizeModu · 图像缩放 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-86`
- [ ] **TOOL-87** IMVSImageSharpnessModu · 清晰度评估 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-87`
- [ ] **TOOL-88** IMVSImgStitchCalibModu · 图像拼接 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-88`
- [ ] **TOOL-89** IMVSInverseAffineTransformModu · 逆仿射变换 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-89`
- [ ] **TOOL-90** IMVSMultiImageFusionModu · 多图融合 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-90`
- [ ] **TOOL-91** IMVSPolarUnwarpModu · 圆环展开 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-91`
- [ ] **TOOL-92** IMVSRegionCopyModu · 拷贝填充 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-92`
- [ ] **TOOL-93** IMVSShadeCorrectModu · 阴影校正 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-93`
- [ ] **TOOL-94** IMVSAngleBisectorFindModu · 角平分线查找 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-94`
- [ ] **TOOL-95** IMVSBlobFindLabelsModu · Blob标签分析 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-95`
- [ ] **TOOL-96** IMVSBlobFindModu · Blob分析 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-96`
- [ ] **TOOL-97** IMVSCaliperCornerModu · 边缘交点 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-97`
- [ ] **TOOL-98** IMVSCaliperEdgeModu · 边缘查找 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-98`
- [ ] **TOOL-99** IMVSCaliperModu · 卡尺工具 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-99`
- [ ] **TOOL-100** IMVSCircleFindModu · 圆查找 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-100`
- [ ] **TOOL-101** IMVSContourMatchModu · 轮廓匹配 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-101`
- [ ] **TOOL-102** IMVSEllipseFindModu · 椭圆查找 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-102`
- [ ] **TOOL-103** IMVSFastFeatureMatchModu · 快速匹配 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-103`
- [ ] **TOOL-104** IMVSFixtureModu · 位置修正 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-104`
- [ ] **TOOL-105** IMVSGluePathConductModu · 路径提取 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-105`
- [ ] **TOOL-106** IMVSGrayMatchModu · IMVSGrayMatchModu · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-106`
- [ ] **TOOL-107** IMVSGrayMatchModuVA · 灰度匹配 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-107`
- [ ] **TOOL-108** IMVSHPFeatureMatchModu · 高精度匹配 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-108`
- [ ] **TOOL-109** IMVSLineFindGroupModu · 直线查找组合 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-109`
- [ ] **TOOL-110** IMVSLineFindModu · 直线查找 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-110`
- [ ] **TOOL-111** IMVSMarkFindModu · 图形定位 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-111`
- [ ] **TOOL-112** IMVSMatrixCircleFindModu · 阵列圆查找 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-112`
- [ ] **TOOL-113** IMVSMedianLineFindModu · 中线查找 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-113`
- [ ] **TOOL-114** IMVSMultiLineFindModu · 多直线查找 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-114`
- [ ] **TOOL-115** IMVSPairLineModu · 平行线查找 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-115`
- [ ] **TOOL-116** IMVSParallelCalculateModu · 平行线计算 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-116`
- [ ] **TOOL-117** IMVSPeakFindModu · 顶点检测 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-117`
- [ ] **TOOL-118** IMVSQuadrangleFindModu · 四边形查找 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-118`
- [ ] **TOOL-119** IMVSRectFindModu · 矩形检测 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-119`
- [ ] **TOOL-120** IMVSTargetTrackModu · 目标跟踪 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-120`
- [ ] **TOOL-121** IMVSVerticalLineFindModu · 垂线查找 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-121`
- [ ] **TOOL-122** AndModule · 逻辑 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-122`
- [ ] **TOOL-123** BranchModule_STD · 分支模块 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-123`
- [ ] **TOOL-124** BranchStringCpmL · 分支字符 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-124`
- [ ] **TOOL-125** DataClassificationModule · 数据分类 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-125`
- [ ] **TOOL-126** DataFilterModule · 数据筛选 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-126`
- [ ] **TOOL-127** DataRecordModule · 数据记录 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-127`
- [ ] **TOOL-128** DataSetModule · 数据集合 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-128`
- [ ] **TOOL-129** DataSortModule · 数据排序 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-129`
- [ ] **TOOL-130** FormatModule · 格式化 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-130`
- [ ] **TOOL-131** GraphicsSetModule · 图形收集 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-131`
- [ ] **TOOL-132** IfBranchModule · 条件分支 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-132`
- [ ] **TOOL-133** IfModule · 条件检测 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-133`
- [ ] **TOOL-134** IMVSGroup · Group · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-134`
- [ ] **TOOL-135** PointSetMODU_STD · 点集 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-135`
- [ ] **TOOL-136** PyShellModule · Python脚本 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-136`
- [ ] **TOOL-137** SaveTextModule · 文本保存 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-137`
- [ ] **TOOL-138** ShellModule · 脚本 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-138`
- [ ] **TOOL-139** StringCompareModule · 字符比较 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-139`
- [ ] **TOOL-140** TimeStatisticModule · 耗时统计 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-140`
- [ ] **TOOL-141** TriggerModule · 触发模块 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-141`
- [ ] **TOOL-142** IMVSC2CMeasureModu · 圆圆测量 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-142`
- [ ] **TOOL-143** IMVSEdgeWidthFindModu · 间距检测 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-143`
- [ ] **TOOL-144** IMVSHistToolModu · 直方图工具 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-144`
- [ ] **TOOL-145** IMVSIntensityMeasureModu · 亮度测量 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-145`
- [ ] **TOOL-146** IMVSL2CMeasureModu · 线圆测量 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-146`
- [ ] **TOOL-147** IMVSL2LMeasureModu · 线线测量 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-147`
- [ ] **TOOL-148** IMVSP2CMeasureModu · 点圆测量 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-148`
- [ ] **TOOL-149** IMVSP2LMeasureModu · 点线测量 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-149`
- [ ] **TOOL-150** IMVSP2PMeasureModu · 点点测量 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-150`
- [ ] **TOOL-151** IMVSPixelCountModu · 像素统计 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-151`
- [ ] **TOOL-152** IMVSPixelCountModuVA · 像素统计 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-152`
- [ ] **TOOL-153** IMVS2dBcrModu · 二维码识别 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-153`
- [ ] **TOOL-154** IMVSBcrModu · 条码识别 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-154`
- [ ] **TOOL-155** IMVSMachineLearningClassifierModu · ML分类 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-155`
- [ ] **TOOL-156** IMVSOcrModu · 字符识别 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-156`
- [ ] **TOOL-157** IMVS2dArrayCorrectModu · 二维阵列 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-157`
- [ ] **TOOL-158** IMVSBoxFilterModule · Box过滤 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-158`
- [ ] **TOOL-159** IMVSBoxMergeModu · Box融合 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-159`
- [ ] **TOOL-160** IMVSBoxOverlapCalculationModu · Box重叠率计算 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-160`
- [ ] **TOOL-161** IMVSDivideImageModu · 划片拆分 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-161`
- [ ] **TOOL-162** IMVSMultiLabelFilterModu · 多标签筛选 · 状态 `not_started` · 来源 `plugins/inventory.json / tool-implementation-specs.map.json` · 证据 2 项 · 退出 身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合；不把测试通过写成 parity · 锚点 `13-tool-cards.html#toolcard-162`

## §7 入口闭合链（8 门 × 6 段 = 48 节点）

闭合链缺任何一段都不算闭合；未取得证据的链节点一律标 unknown 并给出 REA 补证目标。

共 48 项，勾选框全部未预填；状态字段来自机器台账。

- [ ] **CHAIN-E0-CL1** E0 · 入口：定位真实入口载体、对象类型与启用条件 · 门禁 E0 原件与程序集范围 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 每条入口有唯一 ID 与载体证据，词法候选不算绑定确认 · 锚点 `14-entry-chains.html#chain-E0-CL1`
- [ ] **CHAIN-E0-CL2** E0 · 绑定 / 命令：追到真实 handler、命令与参数 · 门禁 E0 原件与程序集范围 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 handler 类型与触发条件有源码级证据，不推断常规模式 · 锚点 `14-entry-chains.html#chain-E0-CL2`
- [ ] **CHAIN-E0-CL3** E0 · 状态读写：列出字段读写、分支谓词、默认值与对象所有权 · 门禁 E0 原件与程序集范围 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 读集合与写集合逐项对账，缺失标 unknown · 锚点 `14-entry-chains.html#chain-E0-CL3`
- [ ] **CHAIN-E0-CL4** E0 · 事件 / 回调：追平台事件码、回调注册与分发顺序 · 门禁 E0 原件与程序集范围 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 注册与分发两侧均有证据，顺序不以截图推断 · 锚点 `14-entry-chains.html#chain-E0-CL4`
- [ ] **CHAIN-E0-CL5** E0 · 持久化或执行效果：确认方案文件、缓存或设备侧的可观察效果 · 门禁 E0 原件与程序集范围 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 副作用可复核，且不把格式等价当作行为等价 · 锚点 `14-entry-chains.html#chain-E0-CL5`
- [ ] **CHAIN-E0-CL6** E0 · 异常 / 清理：覆盖错误码、取消、释放与失败保持状态 · 门禁 E0 原件与程序集范围 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 失败分支有证据；未证部分标 unknown 并给出补证动作 · 锚点 `14-entry-chains.html#chain-E0-CL6`
- [ ] **CHAIN-E1-CL1** E1 · 入口：定位真实入口载体、对象类型与启用条件 · 门禁 E1 资源声明全集 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 每条入口有唯一 ID 与载体证据，词法候选不算绑定确认 · 锚点 `14-entry-chains.html#chain-E1-CL1`
- [ ] **CHAIN-E1-CL2** E1 · 绑定 / 命令：追到真实 handler、命令与参数 · 门禁 E1 资源声明全集 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 handler 类型与触发条件有源码级证据，不推断常规模式 · 锚点 `14-entry-chains.html#chain-E1-CL2`
- [ ] **CHAIN-E1-CL3** E1 · 状态读写：列出字段读写、分支谓词、默认值与对象所有权 · 门禁 E1 资源声明全集 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 读集合与写集合逐项对账，缺失标 unknown · 锚点 `14-entry-chains.html#chain-E1-CL3`
- [ ] **CHAIN-E1-CL4** E1 · 事件 / 回调：追平台事件码、回调注册与分发顺序 · 门禁 E1 资源声明全集 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 注册与分发两侧均有证据，顺序不以截图推断 · 锚点 `14-entry-chains.html#chain-E1-CL4`
- [ ] **CHAIN-E1-CL5** E1 · 持久化或执行效果：确认方案文件、缓存或设备侧的可观察效果 · 门禁 E1 资源声明全集 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 副作用可复核，且不把格式等价当作行为等价 · 锚点 `14-entry-chains.html#chain-E1-CL5`
- [ ] **CHAIN-E1-CL6** E1 · 异常 / 清理：覆盖错误码、取消、释放与失败保持状态 · 门禁 E1 资源声明全集 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 失败分支有证据；未证部分标 unknown 并给出补证动作 · 锚点 `14-entry-chains.html#chain-E1-CL6`
- [ ] **CHAIN-E2-CL1** E2 · 入口：定位真实入口载体、对象类型与启用条件 · 门禁 E2 交互与局部输入 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 每条入口有唯一 ID 与载体证据，词法候选不算绑定确认 · 锚点 `14-entry-chains.html#chain-E2-CL1`
- [ ] **CHAIN-E2-CL2** E2 · 绑定 / 命令：追到真实 handler、命令与参数 · 门禁 E2 交互与局部输入 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 handler 类型与触发条件有源码级证据，不推断常规模式 · 锚点 `14-entry-chains.html#chain-E2-CL2`
- [ ] **CHAIN-E2-CL3** E2 · 状态读写：列出字段读写、分支谓词、默认值与对象所有权 · 门禁 E2 交互与局部输入 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 读集合与写集合逐项对账，缺失标 unknown · 锚点 `14-entry-chains.html#chain-E2-CL3`
- [ ] **CHAIN-E2-CL4** E2 · 事件 / 回调：追平台事件码、回调注册与分发顺序 · 门禁 E2 交互与局部输入 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 注册与分发两侧均有证据，顺序不以截图推断 · 锚点 `14-entry-chains.html#chain-E2-CL4`
- [ ] **CHAIN-E2-CL5** E2 · 持久化或执行效果：确认方案文件、缓存或设备侧的可观察效果 · 门禁 E2 交互与局部输入 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 副作用可复核，且不把格式等价当作行为等价 · 锚点 `14-entry-chains.html#chain-E2-CL5`
- [ ] **CHAIN-E2-CL6** E2 · 异常 / 清理：覆盖错误码、取消、释放与失败保持状态 · 门禁 E2 交互与局部输入 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 失败分支有证据；未证部分标 unknown 并给出补证动作 · 锚点 `14-entry-chains.html#chain-E2-CL6`
- [ ] **CHAIN-E3-CL1** E3 · 入口：定位真实入口载体、对象类型与启用条件 · 门禁 E3 后台与反向入口 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 每条入口有唯一 ID 与载体证据，词法候选不算绑定确认 · 锚点 `14-entry-chains.html#chain-E3-CL1`
- [ ] **CHAIN-E3-CL2** E3 · 绑定 / 命令：追到真实 handler、命令与参数 · 门禁 E3 后台与反向入口 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 handler 类型与触发条件有源码级证据，不推断常规模式 · 锚点 `14-entry-chains.html#chain-E3-CL2`
- [ ] **CHAIN-E3-CL3** E3 · 状态读写：列出字段读写、分支谓词、默认值与对象所有权 · 门禁 E3 后台与反向入口 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 读集合与写集合逐项对账，缺失标 unknown · 锚点 `14-entry-chains.html#chain-E3-CL3`
- [ ] **CHAIN-E3-CL4** E3 · 事件 / 回调：追平台事件码、回调注册与分发顺序 · 门禁 E3 后台与反向入口 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 注册与分发两侧均有证据，顺序不以截图推断 · 锚点 `14-entry-chains.html#chain-E3-CL4`
- [ ] **CHAIN-E3-CL5** E3 · 持久化或执行效果：确认方案文件、缓存或设备侧的可观察效果 · 门禁 E3 后台与反向入口 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 副作用可复核，且不把格式等价当作行为等价 · 锚点 `14-entry-chains.html#chain-E3-CL5`
- [ ] **CHAIN-E3-CL6** E3 · 异常 / 清理：覆盖错误码、取消、释放与失败保持状态 · 门禁 E3 后台与反向入口 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 失败分支有证据；未证部分标 unknown 并给出补证动作 · 锚点 `14-entry-chains.html#chain-E3-CL6`
- [ ] **CHAIN-E4-CL1** E4 · 入口：定位真实入口载体、对象类型与启用条件 · 门禁 E4 声明到 handler · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 每条入口有唯一 ID 与载体证据，词法候选不算绑定确认 · 锚点 `14-entry-chains.html#chain-E4-CL1`
- [ ] **CHAIN-E4-CL2** E4 · 绑定 / 命令：追到真实 handler、命令与参数 · 门禁 E4 声明到 handler · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 handler 类型与触发条件有源码级证据，不推断常规模式 · 锚点 `14-entry-chains.html#chain-E4-CL2`
- [ ] **CHAIN-E4-CL3** E4 · 状态读写：列出字段读写、分支谓词、默认值与对象所有权 · 门禁 E4 声明到 handler · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 读集合与写集合逐项对账，缺失标 unknown · 锚点 `14-entry-chains.html#chain-E4-CL3`
- [ ] **CHAIN-E4-CL4** E4 · 事件 / 回调：追平台事件码、回调注册与分发顺序 · 门禁 E4 声明到 handler · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 注册与分发两侧均有证据，顺序不以截图推断 · 锚点 `14-entry-chains.html#chain-E4-CL4`
- [ ] **CHAIN-E4-CL5** E4 · 持久化或执行效果：确认方案文件、缓存或设备侧的可观察效果 · 门禁 E4 声明到 handler · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 副作用可复核，且不把格式等价当作行为等价 · 锚点 `14-entry-chains.html#chain-E4-CL5`
- [ ] **CHAIN-E4-CL6** E4 · 异常 / 清理：覆盖错误码、取消、释放与失败保持状态 · 门禁 E4 声明到 handler · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 失败分支有证据；未证部分标 unknown 并给出补证动作 · 锚点 `14-entry-chains.html#chain-E4-CL6`
- [ ] **CHAIN-E5-CL1** E5 · 入口：定位真实入口载体、对象类型与启用条件 · 门禁 E5 完整行为与回流 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 每条入口有唯一 ID 与载体证据，词法候选不算绑定确认 · 锚点 `14-entry-chains.html#chain-E5-CL1`
- [ ] **CHAIN-E5-CL2** E5 · 绑定 / 命令：追到真实 handler、命令与参数 · 门禁 E5 完整行为与回流 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 handler 类型与触发条件有源码级证据，不推断常规模式 · 锚点 `14-entry-chains.html#chain-E5-CL2`
- [ ] **CHAIN-E5-CL3** E5 · 状态读写：列出字段读写、分支谓词、默认值与对象所有权 · 门禁 E5 完整行为与回流 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 读集合与写集合逐项对账，缺失标 unknown · 锚点 `14-entry-chains.html#chain-E5-CL3`
- [ ] **CHAIN-E5-CL4** E5 · 事件 / 回调：追平台事件码、回调注册与分发顺序 · 门禁 E5 完整行为与回流 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 注册与分发两侧均有证据，顺序不以截图推断 · 锚点 `14-entry-chains.html#chain-E5-CL4`
- [ ] **CHAIN-E5-CL5** E5 · 持久化或执行效果：确认方案文件、缓存或设备侧的可观察效果 · 门禁 E5 完整行为与回流 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 副作用可复核，且不把格式等价当作行为等价 · 锚点 `14-entry-chains.html#chain-E5-CL5`
- [ ] **CHAIN-E5-CL6** E5 · 异常 / 清理：覆盖错误码、取消、释放与失败保持状态 · 门禁 E5 完整行为与回流 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 失败分支有证据；未证部分标 unknown 并给出补证动作 · 锚点 `14-entry-chains.html#chain-E5-CL6`
- [ ] **CHAIN-E6-CL1** E6 · 入口：定位真实入口载体、对象类型与启用条件 · 门禁 E6 入口与模型双向对账 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 每条入口有唯一 ID 与载体证据，词法候选不算绑定确认 · 锚点 `14-entry-chains.html#chain-E6-CL1`
- [ ] **CHAIN-E6-CL2** E6 · 绑定 / 命令：追到真实 handler、命令与参数 · 门禁 E6 入口与模型双向对账 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 handler 类型与触发条件有源码级证据，不推断常规模式 · 锚点 `14-entry-chains.html#chain-E6-CL2`
- [ ] **CHAIN-E6-CL3** E6 · 状态读写：列出字段读写、分支谓词、默认值与对象所有权 · 门禁 E6 入口与模型双向对账 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 读集合与写集合逐项对账，缺失标 unknown · 锚点 `14-entry-chains.html#chain-E6-CL3`
- [ ] **CHAIN-E6-CL4** E6 · 事件 / 回调：追平台事件码、回调注册与分发顺序 · 门禁 E6 入口与模型双向对账 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 注册与分发两侧均有证据，顺序不以截图推断 · 锚点 `14-entry-chains.html#chain-E6-CL4`
- [ ] **CHAIN-E6-CL5** E6 · 持久化或执行效果：确认方案文件、缓存或设备侧的可观察效果 · 门禁 E6 入口与模型双向对账 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 副作用可复核，且不把格式等价当作行为等价 · 锚点 `14-entry-chains.html#chain-E6-CL5`
- [ ] **CHAIN-E6-CL6** E6 · 异常 / 清理：覆盖错误码、取消、释放与失败保持状态 · 门禁 E6 入口与模型双向对账 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 失败分支有证据；未证部分标 unknown 并给出补证动作 · 锚点 `14-entry-chains.html#chain-E6-CL6`
- [ ] **CHAIN-E7-CL1** E7 · 入口：定位真实入口载体、对象类型与启用条件 · 门禁 E7 有限范围自审与运行证据 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 每条入口有唯一 ID 与载体证据，词法候选不算绑定确认 · 锚点 `14-entry-chains.html#chain-E7-CL1`
- [ ] **CHAIN-E7-CL2** E7 · 绑定 / 命令：追到真实 handler、命令与参数 · 门禁 E7 有限范围自审与运行证据 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 handler 类型与触发条件有源码级证据，不推断常规模式 · 锚点 `14-entry-chains.html#chain-E7-CL2`
- [ ] **CHAIN-E7-CL3** E7 · 状态读写：列出字段读写、分支谓词、默认值与对象所有权 · 门禁 E7 有限范围自审与运行证据 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 读集合与写集合逐项对账，缺失标 unknown · 锚点 `14-entry-chains.html#chain-E7-CL3`
- [ ] **CHAIN-E7-CL4** E7 · 事件 / 回调：追平台事件码、回调注册与分发顺序 · 门禁 E7 有限范围自审与运行证据 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 注册与分发两侧均有证据，顺序不以截图推断 · 锚点 `14-entry-chains.html#chain-E7-CL4`
- [ ] **CHAIN-E7-CL5** E7 · 持久化或执行效果：确认方案文件、缓存或设备侧的可观察效果 · 门禁 E7 有限范围自审与运行证据 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 副作用可复核，且不把格式等价当作行为等价 · 锚点 `14-entry-chains.html#chain-E7-CL5`
- [ ] **CHAIN-E7-CL6** E7 · 异常 / 清理：覆盖错误码、取消、释放与失败保持状态 · 门禁 E7 有限范围自审与运行证据 · 状态 `unknown` · 来源 `entry-inventory.json` · 退出 失败分支有证据；未证部分标 unknown 并给出补证动作 · 锚点 `14-entry-chains.html#chain-E7-CL6`

## §8 原厂程序集 / 自研工程 / 依赖边 / 8 层

25 个唯一程序集、29 个当前工程、62 条 ProjectReference 依赖边与 8 层映射的逐项源码级核对。

共 124 项，勾选框全部未预填；状态字段来自机器台账。

- [ ] **ASM-1** 原厂程序集 VisionMaster.exe 源码级核对 · 状态 `not_started` · 来源 `architecture-baseline.json / analysis/decompiled / original-members.map.json` · 证据 2 项 · 退出 准确类型/成员/绑定/调用链与资源逐项对账 · 锚点 `15-target-source-tree.html#original-1`
- [ ] **ASM-2** 原厂程序集 Apps.XmlParser 源码级核对 · 状态 `not_started` · 来源 `architecture-baseline.json / analysis/decompiled / original-members.map.json` · 证据 2 项 · 退出 准确类型/成员/绑定/调用链与资源逐项对账 · 锚点 `15-target-source-tree.html#original-2`
- [ ] **ASM-3** 原厂程序集 VMControls.WPF 源码级核对 · 状态 `not_started` · 来源 `architecture-baseline.json / analysis/decompiled / original-members.map.json` · 证据 2 项 · 退出 准确类型/成员/绑定/调用链与资源逐项对账 · 锚点 `15-target-source-tree.html#original-3`
- [ ] **ASM-4** 原厂程序集 vServerApp 源码级核对 · 状态 `not_started` · 来源 `architecture-baseline.json / analysis/decompiled / original-members.map.json` · 证据 2 项 · 退出 准确类型/成员/绑定/调用链与资源逐项对账 · 锚点 `15-target-source-tree.html#original-4`
- [ ] **ASM-5** 原厂程序集 VM.Core 源码级核对 · 状态 `not_started` · 来源 `architecture-baseline.json / analysis/decompiled / original-members.map.json` · 证据 2 项 · 退出 准确类型/成员/绑定/调用链与资源逐项对账 · 锚点 `15-target-source-tree.html#original-5`
- [ ] **ASM-6** 原厂程序集 VMControls.Data 源码级核对 · 状态 `not_started` · 来源 `architecture-baseline.json / analysis/decompiled / original-members.map.json` · 证据 2 项 · 退出 准确类型/成员/绑定/调用链与资源逐项对账 · 锚点 `15-target-source-tree.html#original-6`
- [ ] **ASM-7** 原厂程序集 VM.PlatformSDKCS 源码级核对 · 状态 `not_started` · 来源 `architecture-baseline.json / analysis/decompiled / original-members.map.json` · 证据 2 项 · 退出 准确类型/成员/绑定/调用链与资源逐项对账 · 锚点 `15-target-source-tree.html#original-7`
- [ ] **ASM-8** 原厂程序集 iMVS-6000PlatformSDKCS 源码级核对 · 状态 `not_started` · 来源 `architecture-baseline.json / analysis/decompiled / original-members.map.json` · 证据 2 项 · 退出 准确类型/成员/绑定/调用链与资源逐项对账 · 锚点 `15-target-source-tree.html#original-8`
- [ ] **ASM-9** 原厂程序集 VM.Controls 源码级核对 · 状态 `not_started` · 来源 `architecture-baseline.json / analysis/decompiled / original-members.map.json` · 证据 2 项 · 退出 准确类型/成员/绑定/调用链与资源逐项对账 · 锚点 `15-target-source-tree.html#original-9`
- [ ] **ASM-10** 原厂程序集 VMBaseControls 源码级核对 · 状态 `not_started` · 来源 `architecture-baseline.json / analysis/decompiled / original-members.map.json` · 证据 2 项 · 退出 准确类型/成员/绑定/调用链与资源逐项对账 · 锚点 `15-target-source-tree.html#original-10`
- [ ] **ASM-11** 原厂程序集 DynamicObjectConfigControl 源码级核对 · 状态 `not_started` · 来源 `architecture-baseline.json / analysis/decompiled / original-members.map.json` · 证据 2 项 · 退出 准确类型/成员/绑定/调用链与资源逐项对账 · 锚点 `15-target-source-tree.html#original-11`
- [ ] **ASM-12** 原厂程序集 Apps.XmlUI 源码级核对 · 状态 `not_started` · 来源 `architecture-baseline.json / analysis/decompiled / original-members.map.json` · 证据 2 项 · 退出 准确类型/成员/绑定/调用链与资源逐项对账 · 锚点 `15-target-source-tree.html#original-12`
- [ ] **ASM-13** 原厂程序集 ExtensionParamUserControl 源码级核对 · 状态 `not_started` · 来源 `architecture-baseline.json / analysis/decompiled / original-members.map.json` · 证据 2 项 · 退出 准确类型/成员/绑定/调用链与资源逐项对账 · 锚点 `15-target-source-tree.html#original-13`
- [ ] **ASM-14** 原厂程序集 VM.PcUI 源码级核对 · 状态 `not_started` · 来源 `architecture-baseline.json / analysis/decompiled / original-members.map.json` · 证据 2 项 · 退出 准确类型/成员/绑定/调用链与资源逐项对账 · 锚点 `15-target-source-tree.html#original-14`
- [ ] **ASM-15** 原厂程序集 Apps.Style 源码级核对 · 状态 `not_started` · 来源 `architecture-baseline.json / analysis/decompiled / original-members.map.json` · 证据 2 项 · 退出 准确类型/成员/绑定/调用链与资源逐项对账 · 锚点 `15-target-source-tree.html#original-15`
- [ ] **ASM-16** 原厂程序集 Apps.ColorFun 源码级核对 · 状态 `not_started` · 来源 `architecture-baseline.json / analysis/decompiled / original-members.map.json` · 证据 2 项 · 退出 准确类型/成员/绑定/调用链与资源逐项对账 · 锚点 `15-target-source-tree.html#original-16`
- [ ] **ASM-17** 原厂程序集 Apps.PcUI.ImageSource 源码级核对 · 状态 `not_started` · 来源 `architecture-baseline.json / analysis/decompiled / original-members.map.json` · 证据 2 项 · 退出 准确类型/成员/绑定/调用链与资源逐项对账 · 锚点 `15-target-source-tree.html#original-17`
- [ ] **ASM-18** 原厂程序集 CompatibleTool 源码级核对 · 状态 `not_started` · 来源 `architecture-baseline.json / analysis/decompiled / original-members.map.json` · 证据 2 项 · 退出 准确类型/成员/绑定/调用链与资源逐项对账 · 锚点 `15-target-source-tree.html#original-18`
- [ ] **ASM-19** 原厂程序集 VM.ExternalCall 源码级核对 · 状态 `not_started` · 来源 `architecture-baseline.json / analysis/decompiled / original-members.map.json` · 证据 2 项 · 退出 准确类型/成员/绑定/调用链与资源逐项对账 · 锚点 `15-target-source-tree.html#original-19`
- [ ] **ASM-20** 原厂程序集 ImageSourceModuleControl 源码级核对 · 状态 `not_started` · 来源 `architecture-baseline.json / analysis/decompiled / original-members.map.json` · 证据 2 项 · 退出 准确类型/成员/绑定/调用链与资源逐项对账 · 锚点 `15-target-source-tree.html#original-20`
- [ ] **ASM-21** 原厂程序集 IMVSGroup 源码级核对 · 状态 `not_started` · 来源 `architecture-baseline.json / analysis/decompiled / original-members.map.json` · 证据 2 项 · 退出 准确类型/成员/绑定/调用链与资源逐项对账 · 锚点 `15-target-source-tree.html#original-21`
- [ ] **ASM-22** 原厂程序集 IfModule 源码级核对 · 状态 `not_started` · 来源 `architecture-baseline.json / analysis/decompiled / original-members.map.json` · 证据 2 项 · 退出 准确类型/成员/绑定/调用链与资源逐项对账 · 锚点 `15-target-source-tree.html#original-22`
- [ ] **ASM-23** 原厂程序集 IfBranchModule 源码级核对 · 状态 `not_started` · 来源 `architecture-baseline.json / analysis/decompiled / original-members.map.json` · 证据 2 项 · 退出 准确类型/成员/绑定/调用链与资源逐项对账 · 锚点 `15-target-source-tree.html#original-23`
- [ ] **ASM-24** 原厂程序集 BranchModule_STD 源码级核对 · 状态 `not_started` · 来源 `architecture-baseline.json / analysis/decompiled / original-members.map.json` · 证据 2 项 · 退出 准确类型/成员/绑定/调用链与资源逐项对账 · 锚点 `15-target-source-tree.html#original-24`
- [ ] **ASM-25** 原厂程序集 BranchStringCpmL 源码级核对 · 状态 `not_started` · 来源 `architecture-baseline.json / analysis/decompiled / original-members.map.json` · 证据 2 项 · 退出 准确类型/成员/绑定/调用链与资源逐项对账 · 锚点 `15-target-source-tree.html#original-25`
- [ ] **PROJ-FrontendDesignMigration** 自研工程 FrontendDesignMigration 现状核对 · 状态 `current-source` · 来源 `architecture-baseline.json → currentProjects` · 证据 1 项 · 退出 与原厂对应层职责逐项差分 · 锚点 `15-target-source-tree.html#tree-diagrams`
- [ ] **PROJ-SolutionInspector** 自研工程 SolutionInspector 现状核对 · 状态 `current-source` · 来源 `architecture-baseline.json → currentProjects` · 证据 1 项 · 退出 与原厂对应层职责逐项差分 · 锚点 `15-target-source-tree.html#tree-diagrams`
- [ ] **PROJ-VisionCore** 自研工程 VisionCore 现状核对 · 状态 `current-source` · 来源 `architecture-baseline.json → currentProjects` · 证据 1 项 · 退出 与原厂对应层职责逐项差分 · 锚点 `15-target-source-tree.html#tree-diagrams`
- [ ] **PROJ-VisionData** 自研工程 VisionData 现状核对 · 状态 `current-source` · 来源 `architecture-baseline.json → currentProjects` · 证据 1 项 · 退出 与原厂对应层职责逐项差分 · 锚点 `15-target-source-tree.html#tree-diagrams`
- [ ] **PROJ-VisionEditor** 自研工程 VisionEditor 现状核对 · 状态 `current-source` · 来源 `architecture-baseline.json → currentProjects` · 证据 1 项 · 退出 与原厂对应层职责逐项差分 · 锚点 `15-target-source-tree.html#tree-diagrams`
- [ ] **PROJ-VisionEditor.Core** 自研工程 VisionEditor.Core 现状核对 · 状态 `current-source` · 来源 `architecture-baseline.json → currentProjects` · 证据 1 项 · 退出 与原厂对应层职责逐项差分 · 锚点 `15-target-source-tree.html#tree-diagrams`
- [ ] **PROJ-VisionFramework** 自研工程 VisionFramework 现状核对 · 状态 `current-source` · 来源 `architecture-baseline.json → currentProjects` · 证据 1 项 · 退出 与原厂对应层职责逐项差分 · 锚点 `15-target-source-tree.html#tree-diagrams`
- [ ] **PROJ-VisionGraphRunner** 自研工程 VisionGraphRunner 现状核对 · 状态 `current-source` · 来源 `architecture-baseline.json → currentProjects` · 证据 1 项 · 退出 与原厂对应层职责逐项差分 · 锚点 `15-target-source-tree.html#tree-diagrams`
- [ ] **PROJ-VisionIO** 自研工程 VisionIO 现状核对 · 状态 `current-source` · 来源 `architecture-baseline.json → currentProjects` · 证据 1 项 · 退出 与原厂对应层职责逐项差分 · 锚点 `15-target-source-tree.html#tree-diagrams`
- [ ] **PROJ-VisionModules** 自研工程 VisionModules 现状核对 · 状态 `current-source` · 来源 `architecture-baseline.json → currentProjects` · 证据 1 项 · 退出 与原厂对应层职责逐项差分 · 锚点 `15-target-source-tree.html#tree-diagrams`
- [ ] **PROJ-VisionPluginContracts** 自研工程 VisionPluginContracts 现状核对 · 状态 `current-source` · 来源 `architecture-baseline.json → currentProjects` · 证据 1 项 · 退出 与原厂对应层职责逐项差分 · 锚点 `15-target-source-tree.html#tree-diagrams`
- [ ] **PROJ-VisionPlugins.BranchModule** 自研工程 VisionPlugins.BranchModule 现状核对 · 状态 `current-source` · 来源 `architecture-baseline.json → currentProjects` · 证据 1 项 · 退出 与原厂对应层职责逐项差分 · 锚点 `15-target-source-tree.html#tree-diagrams`
- [ ] **PROJ-VisionPlugins.BranchString** 自研工程 VisionPlugins.BranchString 现状核对 · 状态 `current-source` · 来源 `architecture-baseline.json → currentProjects` · 证据 1 项 · 退出 与原厂对应层职责逐项差分 · 锚点 `15-target-source-tree.html#tree-diagrams`
- [ ] **PROJ-VisionPlugins.Calibration** 自研工程 VisionPlugins.Calibration 现状核对 · 状态 `current-source` · 来源 `architecture-baseline.json → currentProjects` · 证据 1 项 · 退出 与原厂对应层职责逐项差分 · 锚点 `15-target-source-tree.html#tree-diagrams`
- [ ] **PROJ-VisionPlugins.CalibrationBoard** 自研工程 VisionPlugins.CalibrationBoard 现状核对 · 状态 `current-source` · 来源 `architecture-baseline.json → currentProjects` · 证据 1 项 · 退出 与原厂对应层职责逐项差分 · 锚点 `15-target-source-tree.html#tree-diagrams`
- [ ] **PROJ-VisionPlugins.CameraMapping** 自研工程 VisionPlugins.CameraMapping 现状核对 · 状态 `current-source` · 来源 `architecture-baseline.json → currentProjects` · 证据 1 项 · 退出 与原厂对应层职责逐项差分 · 锚点 `15-target-source-tree.html#tree-diagrams`
- [ ] **PROJ-VisionPlugins.ContourMatch** 自研工程 VisionPlugins.ContourMatch 现状核对 · 状态 `current-source` · 来源 `architecture-baseline.json → currentProjects` · 证据 1 项 · 退出 与原厂对应层职责逐项差分 · 锚点 `15-target-source-tree.html#tree-diagrams`
- [ ] **PROJ-VisionPlugins.CoordinateModule** 自研工程 VisionPlugins.CoordinateModule 现状核对 · 状态 `current-source` · 来源 `architecture-baseline.json → currentProjects` · 证据 1 项 · 退出 与原厂对应层职责逐项差分 · 锚点 `15-target-source-tree.html#tree-diagrams`
- [ ] **PROJ-VisionPlugins.DistortionCalibration** 自研工程 VisionPlugins.DistortionCalibration 现状核对 · 状态 `current-source` · 来源 `architecture-baseline.json → currentProjects` · 证据 1 项 · 退出 与原厂对应层职责逐项差分 · 锚点 `15-target-source-tree.html#tree-diagrams`
- [ ] **PROJ-VisionPlugins.Fixture** 自研工程 VisionPlugins.Fixture 现状核对 · 状态 `current-source` · 来源 `architecture-baseline.json → currentProjects` · 证据 1 项 · 退出 与原厂对应层职责逐项差分 · 锚点 `15-target-source-tree.html#tree-diagrams`
- [ ] **PROJ-VisionPlugins.GrayMatch** 自研工程 VisionPlugins.GrayMatch 现状核对 · 状态 `current-source` · 来源 `architecture-baseline.json → currentProjects` · 证据 1 项 · 退出 与原厂对应层职责逐项差分 · 锚点 `15-target-source-tree.html#tree-diagrams`
- [ ] **PROJ-VisionPlugins.IfModule** 自研工程 VisionPlugins.IfModule 现状核对 · 状态 `current-source` · 来源 `architecture-baseline.json → currentProjects` · 证据 1 项 · 退出 与原厂对应层职责逐项差分 · 锚点 `15-target-source-tree.html#tree-diagrams`
- [ ] **PROJ-VisionPlugins.ImageBinary** 自研工程 VisionPlugins.ImageBinary 现状核对 · 状态 `current-source` · 来源 `architecture-baseline.json → currentProjects` · 证据 1 项 · 退出 与原厂对应层职责逐项差分 · 锚点 `15-target-source-tree.html#tree-diagrams`
- [ ] **PROJ-VisionPlugins.ImageSource** 自研工程 VisionPlugins.ImageSource 现状核对 · 状态 `current-source` · 来源 `architecture-baseline.json → currentProjects` · 证据 1 项 · 退出 与原厂对应层职责逐项差分 · 锚点 `15-target-source-tree.html#tree-diagrams`
- [ ] **PROJ-VisionPlugins.MapCalibration** 自研工程 VisionPlugins.MapCalibration 现状核对 · 状态 `current-source` · 来源 `architecture-baseline.json → currentProjects` · 证据 1 项 · 退出 与原厂对应层职责逐项差分 · 锚点 `15-target-source-tree.html#tree-diagrams`
- [ ] **PROJ-VisionRunner** 自研工程 VisionRunner 现状核对 · 状态 `current-source` · 来源 `architecture-baseline.json → currentProjects` · 证据 1 项 · 退出 与原厂对应层职责逐项差分 · 锚点 `15-target-source-tree.html#tree-diagrams`
- [ ] **PROJ-VisionUi** 自研工程 VisionUi 现状核对 · 状态 `current-source` · 来源 `architecture-baseline.json → currentProjects` · 证据 1 项 · 退出 与原厂对应层职责逐项差分 · 锚点 `15-target-source-tree.html#tree-diagrams`
- [ ] **PROJ-VisionWorkbench** 自研工程 VisionWorkbench 现状核对 · 状态 `current-source` · 来源 `architecture-baseline.json → currentProjects` · 证据 1 项 · 退出 与原厂对应层职责逐项差分 · 锚点 `15-target-source-tree.html#tree-diagrams`
- [ ] **PROJ-VisionWorkflow** 自研工程 VisionWorkflow 现状核对 · 状态 `current-source` · 来源 `architecture-baseline.json → currentProjects` · 证据 1 项 · 退出 与原厂对应层职责逐项差分 · 锚点 `15-target-source-tree.html#tree-diagrams`
- [ ] **EDGE-1** 依赖边 VisionCore → VisionData · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-2** 依赖边 VisionEditor → VisionEditor.Core · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-3** 依赖边 VisionEditor → VisionModules · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-4** 依赖边 VisionEditor → VisionPlugins.BranchModule · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-5** 依赖边 VisionEditor → VisionPlugins.BranchString · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-6** 依赖边 VisionEditor → VisionPlugins.Calibration · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-7** 依赖边 VisionEditor → VisionPlugins.CalibrationBoard · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-8** 依赖边 VisionEditor → VisionPlugins.CameraMapping · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-9** 依赖边 VisionEditor → VisionPlugins.CoordinateModule · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-10** 依赖边 VisionEditor → VisionPlugins.DistortionCalibration · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-11** 依赖边 VisionEditor → VisionPlugins.Fixture · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-12** 依赖边 VisionEditor → VisionPlugins.ImageBinary · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-13** 依赖边 VisionEditor → VisionPlugins.ImageSource · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-14** 依赖边 VisionEditor → VisionPlugins.MapCalibration · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-15** 依赖边 VisionEditor → VisionUi · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-16** 依赖边 VisionEditor.Core → VisionFramework · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-17** 依赖边 VisionFramework → VisionData · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-18** 依赖边 VisionGraphRunner → VisionFramework · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-19** 依赖边 VisionGraphRunner → VisionModules · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-20** 依赖边 VisionIO → VisionData · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-21** 依赖边 VisionModules → VisionCore · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-22** 依赖边 VisionModules → VisionFramework · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-23** 依赖边 VisionModules → VisionIO · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-24** 依赖边 VisionPluginContracts → VisionFramework · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-25** 依赖边 VisionPluginContracts → VisionUi · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-26** 依赖边 VisionPlugins.BranchModule → VisionFramework · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-27** 依赖边 VisionPlugins.BranchModule → VisionPluginContracts · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-28** 依赖边 VisionPlugins.BranchString → VisionFramework · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-29** 依赖边 VisionPlugins.BranchString → VisionPluginContracts · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-30** 依赖边 VisionPlugins.Calibration → VisionFramework · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-31** 依赖边 VisionPlugins.Calibration → VisionPluginContracts · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-32** 依赖边 VisionPlugins.Calibration → VisionPlugins.ImageSource · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-33** 依赖边 VisionPlugins.Calibration → VisionUi · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-34** 依赖边 VisionPlugins.CalibrationBoard → VisionCore · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-35** 依赖边 VisionPlugins.CalibrationBoard → VisionFramework · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-36** 依赖边 VisionPlugins.CalibrationBoard → VisionPluginContracts · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-37** 依赖边 VisionPlugins.CalibrationBoard → VisionPlugins.Calibration · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-38** 依赖边 VisionPlugins.CameraMapping → VisionFramework · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-39** 依赖边 VisionPlugins.CameraMapping → VisionPluginContracts · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-40** 依赖边 VisionPlugins.CameraMapping → VisionUi · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-41** 依赖边 VisionPlugins.ContourMatch → VisionPlugins.Calibration · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-42** 依赖边 VisionPlugins.CoordinateModule → VisionPlugins.Calibration · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-43** 依赖边 VisionPlugins.DistortionCalibration → VisionPluginContracts · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-44** 依赖边 VisionPlugins.DistortionCalibration → VisionPlugins.Calibration · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-45** 依赖边 VisionPlugins.DistortionCalibration → VisionPlugins.CalibrationBoard · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-46** 依赖边 VisionPlugins.Fixture → VisionFramework · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-47** 依赖边 VisionPlugins.Fixture → VisionPluginContracts · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-48** 依赖边 VisionPlugins.IfModule → VisionFramework · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-49** 依赖边 VisionPlugins.IfModule → VisionPluginContracts · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-50** 依赖边 VisionPlugins.ImageBinary → VisionPluginContracts · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-51** 依赖边 VisionPlugins.ImageBinary → VisionUi · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-52** 依赖边 VisionPlugins.ImageSource → VisionIO · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-53** 依赖边 VisionPlugins.ImageSource → VisionPluginContracts · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-54** 依赖边 VisionPlugins.MapCalibration → VisionCore · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-55** 依赖边 VisionPlugins.MapCalibration → VisionPluginContracts · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-56** 依赖边 VisionPlugins.MapCalibration → VisionPlugins.Calibration · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-57** 依赖边 VisionRunner → VisionCore · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-58** 依赖边 VisionRunner → VisionIO · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-59** 依赖边 VisionUi → VisionFramework · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-60** 依赖边 VisionWorkbench → VisionWorkflow · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-61** 依赖边 VisionWorkflow → VisionCore · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **EDGE-62** 依赖边 VisionWorkflow → VisionIO · 状态 `current-source` · 来源 `architecture-baseline.json → currentEdges` · 退出 引用文件与方向可复核 · 锚点 `15-target-source-tree.html#tree-deps`
- [ ] **LAYER-L1-shell** 架构层 L1-shell · 外壳、入口与服务拓扑 · 状态 `partial` · 来源 `architecture-baseline.json → layers` · 退出 层内原厂/自研职责与缺口逐项对账 · 锚点 `02-architecture.html#arch-layers`
- [ ] **LAYER-L2-model** 架构层 L2-model · 方案/流程/模块文档模型与动态 IO · 状态 `partial` · 来源 `architecture-baseline.json → layers` · 退出 层内原厂/自研职责与缺口逐项对账 · 锚点 `02-architecture.html#arch-layers`
- [ ] **LAYER-L3-runtime-sdk** 架构层 L3-runtime-sdk · 执行运行时与平台 SDK · 状态 `absent` · 来源 `architecture-baseline.json → layers` · 退出 层内原厂/自研职责与缺口逐项对账 · 锚点 `02-architecture.html#arch-layers`
- [ ] **LAYER-L4-canvas-controls** 架构层 L4-canvas-controls · 画布、连接与通用控件 · 状态 `partial` · 来源 `architecture-baseline.json → layers` · 退出 层内原厂/自研职责与缺口逐项对账 · 锚点 `02-architecture.html#arch-layers`
- [ ] **LAYER-L5-config** 架构层 L5-config · 配置框架与 XML 参数模型 · 状态 `partial` · 来源 `architecture-baseline.json → layers` · 退出 层内原厂/自研职责与缺口逐项对账 · 锚点 `02-architecture.html#arch-layers`
- [ ] **LAYER-L6-display** 架构层 L6-display · 显示对象、绘制与主题 · 状态 `partial` · 来源 `architecture-baseline.json → layers` · 退出 层内原厂/自研职责与缺口逐项对账 · 锚点 `02-architecture.html#arch-layers`
- [ ] **LAYER-L7-migration-hosts** 架构层 L7-migration-hosts · 版本迁移、外部调用与工具宿主 · 状态 `partial` · 来源 `architecture-baseline.json → layers` · 退出 层内原厂/自研职责与缺口逐项对账 · 锚点 `02-architecture.html#arch-layers`
- [ ] **LAYER-L8-tool-plugins** 架构层 L8-tool-plugins · 工具插件样本 · 状态 `partial` · 来源 `architecture-baseline.json → layers` · 退出 层内原厂/自研职责与缺口逐项对账 · 锚点 `02-architecture.html#arch-layers`

## §9 阶段 / 门禁 / 证据规则 / 旧计划处置

S0–S9 阶段、G1–G8 阶段门、E0–E7 入口门、证据四态、验收层级与 4 份旧计划处置。

共 37 项，勾选框全部未预填；状态字段来自机器台账。

- [ ] **STAGE-S0** 阶段 S0 · 纵向切片闭环 · 状态 `not_started` · 来源 `reverse-plan-editorial.json → stages` · 退出 本地链路在构建产物上可执行，运行报告含会话状态、节点状态与输出名清单；保存后的方案重开能得到同一组结果与输出名；原厂封装与直接 SDK 的数值差分被记录为开放项，不以测试通过冒充等价 · 前置/阻塞 架构基线、源码量分母与 8 层映射冻结；ImageSource / Blob / Group 的最小参数与输出契约已定。 · 锚点 `03-roadmap.html#stage-S0`
- [ ] **STAGE-S1** 阶段 S1 · 执行后端抽象 · 状态 `not_started` · 来源 `reverse-plan-editorial.json → stages` · 退出 原厂命令提交与平台事件回调的调用链有源码级证据路径；托管顺序 await 与 SDK 状态机的差异被逐项列出且不影响既有链路回归；错误码与取消语义的操作级验证记录可复跑 · 前置/阻塞 S0 已给出结果回写与状态可观察点；原厂提交-回调链的原码/CIL 证据可定位。 · 锚点 `03-roadmap.html#stage-S1`
- [ ] **STAGE-S2** 阶段 S2 · 文档身份与动态 IO · 状态 `not_started` · 来源 `reverse-plan-editorial.json → stages` · 退出 身份字段成为连接、订阅、结果缓存与保存的唯一键来源；动态输出改名/改型后下游订阅与保存重开行为有操作级记录；既有方案文件在兼容规则下可打开，破坏性变更单独登记 · 前置/阻塞 S1 后端接口稳定；原厂数值 ModuleId / GUID 与动态端口的生产者、消费者已定位。 · 锚点 `03-roadmap.html#stage-S2`
- [ ] **STAGE-S3** 阶段 S3 · 方案容器与原生帧 · 状态 `not_started` · 来源 `reverse-plan-editorial.json → stages` · 退出 外层标签与偏移有源码/运行时证据，不靠猜测填充；只读导入能还原方案结构，写入能力不提前声明；加密、版本迁移、跨版本方案分别列为独立开放项 · 前置/阻塞 S2 身份字段落地；原厂 .sol 外层与 ModuleFrame 帧边界的静态证据可定位。 · 锚点 `03-roadmap.html#stage-S3`
- [ ] **STAGE-S4** 阶段 S4 · 配置框架覆盖 · 状态 `not_started` · 来源 `reverse-plan-editorial.json → stages` · 退出 覆盖清单按原厂声明逐键对账，缺口有唯一 ID；所有工具配置窗走同一宿主，不再各自使用旧窗口工厂；覆盖比例由台账重算，不在页面里手填 · 前置/阻塞 S1–S3 的共享契约稳定；原厂 NameValueItem / ParamGroup / 模板键的目录证据完整。 · 锚点 `03-roadmap.html#stage-S4`
- [ ] **STAGE-S5** 阶段 S5 · 显示对象与绘制层 · 状态 `not_started` · 来源 `reverse-plan-editorial.json → stages` · 退出 显示类型清单与资产按 key 对账，缺失项单列；工具结果的可观察输出有逐类验收记录；主题样式未按 key 全量对账的部分明确标 unknown · 前置/阻塞 S4 配置模板目录稳定；VM.PcUI 类型族与主题资源键已盘点。 · 锚点 `03-roadmap.html#stage-S5`
- [ ] **STAGE-S6** 阶段 S6 · 画布控件补齐（不重写） · 状态 `not_started` · 来源 `reverse-plan-editorial.json → stages` · 退出 每个补齐项都有原厂成员路径与行为级对照；控件级不等价被明确标注，不以几何相似冒充控件树等价；既有画布回归全绿 · 前置/阻塞 S0 链路与 S5 显示层稳定；原厂 ListView / DiagramItem / Adorner 行为证据可定位。 · 锚点 `03-roadmap.html#stage-S6`
- [ ] **STAGE-S7** 阶段 S7 · 外壳入口逐载体闭合 · 状态 `not_started` · 来源 `reverse-plan-editorial.json → stages` · 退出 每条闭合链有命令、状态写入与持久化/执行副作用证据；BAML 键与动态注册面未穷尽的部分仍标 open；入口完整性门禁 E0–E7 的缺口逐项登记 · 前置/阻塞 entry-inventory 的入口、命令、状态、持久化四段链可逐条定位。 · 锚点 `03-roadmap.html#stage-S7`
- [ ] **STAGE-S8** 阶段 S8 · 平台服务与设备（只读盘点） · 状态 `not_started` · 来源 `reverse-plan-editorial.json → stages` · 退出 服务清单、回调注册点与错误码有源码证据；原生 ABI 歧义处记录实际入口寄存器与活跃性，不采纳伪代码签名推断；真实设备操作不纳入本阶段 · 前置/阻塞 S1 后端抽象可承载服务发现；iMVS-6000 平台服务程序集证据可定位。 · 锚点 `03-roadmap.html#stage-S8`
- [ ] **STAGE-S9** 阶段 S9 · 162 目录工具分批铺开 · 状态 `not_started` · 来源 `reverse-plan-editorial.json → stages` · 退出 162 个工具目录身份逐项可核对，无重复、无遗漏；只有整工具完成源码级重建且通过自审才计入完成，人工验收单列；XML 声明计数不冒充可执行行为，native DLL 未反编译的部分标 unknown · 前置/阻塞 S1–S5 共享机制稳定；11 工具批次与标定 10 工具的既有结论不被回滚。 · 锚点 `03-roadmap.html#stage-S9`
- [ ] **GATE-G1** 阶段门 G1 · 分母冻结 · 状态 `not_started` · 来源 `reverse-plan-editorial.json → stageGates` · 退出 任何页面数字必须来自机器台账；分母或排除规则变化必须提升 schemaVersion 并登记原因。 · 锚点 `09-gates-evidence.html#gate-G1`
- [ ] **GATE-G2** 阶段门 G2 · 证据四态 · 状态 `not_started` · 来源 `reverse-plan-editorial.json → stageGates` · 退出 每个结论标注四态之一；缺失证据标 unknown，不得以用户反馈或截图代替规格。 · 锚点 `09-gates-evidence.html#gate-G2`
- [ ] **GATE-G3** 阶段门 G3 · 调用链闭环 · 状态 `not_started` · 来源 `reverse-plan-editorial.json → stageGates` · 退出 入口→绑定/命令→状态读写→事件/回调→持久化或执行效果→异常/清理，缺一段不算闭合。 · 锚点 `09-gates-evidence.html#gate-G3`
- [ ] **GATE-G4** 阶段门 G4 · 原生 ABI · 状态 `not_started` · 来源 `reverse-plan-editorial.json → stageGates` · 退出 包装器只改写 RCX 时须检查 RDX/R8/R9 活跃性并记录真实入口寄存器，不采纳伪代码签名推断。 · 锚点 `09-gates-evidence.html#gate-G4`
- [ ] **GATE-G5** 阶段门 G5 · 独立实现隔离 · 状态 `not_started` · 来源 `reverse-plan-editorial.json → stageGates` · 退出 独立实现与源码确认的结论分开陈列，不能从测试通过推出行为等价。 · 锚点 `09-gates-evidence.html#gate-G5`
- [ ] **GATE-G6** 阶段门 G6 · 整工具计分 · 状态 `not_started` · 来源 `reverse-plan-editorial.json → stageGates` · 退出 工具完成按整工具源码级重建 + 自审计，人工验收单列，不参与完成度。 · 锚点 `09-gates-evidence.html#gate-G6`
- [ ] **GATE-G7** 阶段门 G7 · 操作级验证 · 状态 `not_started` · 来源 `reverse-plan-editorial.json → stageGates` · 退出 每单元保留操作级验证记录；历史记录未在本次复跑的须注明未重跑。 · 锚点 `09-gates-evidence.html#gate-G7`
- [ ] **GATE-G8** 阶段门 G8 · 确定性重跑 · 状态 `not_started` · 来源 `reverse-plan-editorial.json → stageGates` · 退出 站点不写 wall-clock；输入哈希与 HEAD 构成 builtFrom，重跑字节一致。 · 锚点 `09-gates-evidence.html#gate-G8`
- [ ] **E-E0** 入口门 E0 · 原件与程序集范围 · 状态 `open` · 来源 `architecture-recovery/entry-inventory.json` · 退出 六段链闭合且入口与模型双向对账 · 锚点 `14-entry-chains.html#gate-E0`
- [ ] **E-E1** 入口门 E1 · 资源声明全集 · 状态 `partial` · 来源 `architecture-recovery/entry-inventory.json` · 退出 六段链闭合且入口与模型双向对账 · 锚点 `14-entry-chains.html#gate-E1`
- [ ] **E-E2** 入口门 E2 · 交互与局部输入 · 状态 `open` · 来源 `architecture-recovery/entry-inventory.json` · 退出 六段链闭合且入口与模型双向对账 · 锚点 `14-entry-chains.html#gate-E2`
- [ ] **E-E3** 入口门 E3 · 后台与反向入口 · 状态 `open` · 来源 `architecture-recovery/entry-inventory.json` · 退出 六段链闭合且入口与模型双向对账 · 锚点 `14-entry-chains.html#gate-E3`
- [ ] **E-E4** 入口门 E4 · 声明到 handler · 状态 `open` · 来源 `architecture-recovery/entry-inventory.json` · 退出 六段链闭合且入口与模型双向对账 · 锚点 `14-entry-chains.html#gate-E4`
- [ ] **E-E5** 入口门 E5 · 完整行为与回流 · 状态 `open` · 来源 `architecture-recovery/entry-inventory.json` · 退出 六段链闭合且入口与模型双向对账 · 锚点 `14-entry-chains.html#gate-E5`
- [ ] **E-E6** 入口门 E6 · 入口与模型双向对账 · 状态 `open` · 来源 `architecture-recovery/entry-inventory.json` · 退出 六段链闭合且入口与模型双向对账 · 锚点 `14-entry-chains.html#gate-E6`
- [ ] **E-E7** 入口门 E7 · 有限范围自审与运行证据 · 状态 `open` · 来源 `architecture-recovery/entry-inventory.json` · 退出 六段链闭合且入口与模型双向对账 · 锚点 `14-entry-chains.html#gate-E7`
- [ ] **EVID-source-confirmed** 证据态 源码确认 · 状态 `rule` · 来源 `gates-and-evidence.json` · 退出 反编译源码、随包资源或 CIL 中可定位到具体类型/成员/调用链；记录准确路径与哈希。
- [ ] **EVID-runtime-confirmed** 证据态 运行确认 · 状态 `rule` · 来源 `gates-and-evidence.json` · 退出 在已确认的运行环境或原程序上取得可复核的运行时输出、状态或事件顺序。
- [ ] **EVID-independent-implementation** 证据态 独立实现 · 状态 `rule` · 来源 `gates-and-evidence.json` · 退出 本地按已确认边界自行实现，机制或载体与原厂不同。
- [ ] **EVID-unknown** 证据态 未知 · 状态 `rule` · 来源 `gates-and-evidence.json` · 退出 函数体、资源、ABI、时序或运行证据缺失；REA 补证是检索任务。
- [ ] **ACCEPT-L1** 验收层级 L1 · 状态 `rule` · 来源 `gates-and-evidence.json` · 退出 
- [ ] **ACCEPT-L2** 验收层级 L2 · 状态 `rule` · 来源 `gates-and-evidence.json` · 退出 
- [ ] **ACCEPT-L3** 验收层级 L3 · 状态 `rule` · 来源 `gates-and-evidence.json` · 退出 
- [ ] **LEGACY-1** 旧计划处置 框架层源码级逆向总计划 · 状态 `原则仍有效 / 派工已取代` · 来源 `docs/FRAMEWORK-REVERSE-MASTER-PLAN.md` · 退出 保留历史，不改既有结论数字 · 锚点 `00-master-plan.html#mp-legacy`
- [ ] **LEGACY-2** 旧计划处置 源码级逆向计划（标定工具进度记录） · 状态 `仅历史` · 来源 `docs/SOURCE-LEVEL-REVERSE-PLAN.md` · 退出 保留历史，不改既有结论数字 · 锚点 `00-master-plan.html#mp-legacy`
- [ ] **LEGACY-3** 旧计划处置 VM 本地真实还原计划 · 状态 `原则仍有效 / 单元已并入` · 来源 `docs/VM-LOCAL-RESTORATION-PLAN.md` · 退出 保留历史，不改既有结论数字 · 锚点 `00-master-plan.html#mp-legacy`
- [ ] **LEGACY-4** 旧计划处置 下一批工具插件源码级逆向计划 · 状态 `仅历史` · 来源 `docs/NEXT-BATCH-PLUGIN-REVERSE-PLAN.md` · 退出 保留历史，不改既有结论数字 · 锚点 `00-master-plan.html#mp-legacy`

## §10 状态更新与治理规则

勾选后如何回写、哪些动作必须用户授权、哪些不得自动执行。

共 5 项，勾选框全部未预填；状态字段来自机器台账。

- [ ] **RULE-01** 勾选任务后同步回写台账与站点（保持确定性） · 状态 `rule` · 来源 `AGENTS.md / 计划站点硬约束` · 退出 重跑 build + verify 且逐字节一致；不手写结论数字
- [ ] **RULE-02** 不自动提交/推送、不自动进入方案测试 · 状态 `rule` · 来源 `用户硬规则` · 退出 提交需用户明确说提交；测试需用户明确说开始测试/验证
- [ ] **RULE-03** 口径冲突并列登记，不擅自统一 · 状态 `rule` · 来源 `AGENTS.md` · 退出 42/96=43.75% 与 41/96=42.71% 并列；PositionCorrection 批次身份差异原样保留；45/971486 与 47/973202 并列
- [ ] **RULE-04** 人工主机验收单列，不阻塞也不计入完成度 · 状态 `rule` · 来源 `AGENTS.md` · 退出 用户交互桌面 WinSta0\Default 验证可见窗口后再登记
- [ ] **RULE-05** 缺失函数体/资源/ABI 标 unknown，不以合理设计填空 · 状态 `rule` · 来源 `AGENTS.md` · 退出 区分 source-confirmed / runtime-confirmed / independent-implementation / unknown

## 附录 A 全部命令（1 条，均待执行）

- `dotnet run --project tests/VMVerticalSlice.C01/VMVerticalSlice.C01.csproj --no-restore` 〔S0 复跑批次已执行：exitCode=0〕

## 附录 B 全部报告路径（8 项）

- `analysis/full-audit/batch1-20261107/C01/runtime-report.json`
- `analysis/full-audit/batch1-20261107/C03/c03-incremental-b1-contracts.json`
- `analysis/full-audit/batch1-20261107/C03/c03-incremental-b1-static.json`
- `analysis/full-audit/batch1-20261107/C03/c03-incremental-batch0-b1.json`
- `analysis/full-audit/batch1-20261107/C03/c03-next-evidence-gate.json`
- `analysis/full-audit/batch1-20261107/C03/c03-phase1-evidence-card.json`
- `analysis/full-audit/batch1-20261107/C03/c03-phase1-evidence-review.json`
- `analysis/full-audit/batch1-20261107/C03/c03-s0-rerun-matrix-20261110.json`

## 附录 C 全部证据路径（320 项，去重）

- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Calculation\CalculatorModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Calculation\CoordinateTransform`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Calculation\IMVSCalibTransformModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Calculation\IMVSLineAlignModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Calculation\IMVSMultiPointAlignModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Calculation\IMVSRotateCalculateModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Calculation\IMVSScaleTransformModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Calculation\IMVSSinglePointAlignModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Calculation\SinglePointGrabModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Calculation\SinglePointMapAlignModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Calculation\SinglePointRectifyModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Calibration\CoordinateModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Calibration\IMVSCalibBoardCalibModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Calibration\IMVSCameraMapModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Calibration\IMVSImageCalibModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Calibration\IMVSMapCalibModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Calibration\IMVSNImageCalibModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Calibration\IMVSNPointCalibModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Calibration\ReadCalibFileModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Calibration\RotateCalibModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Calibration\TranslationCalibModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Collection\ImageAcquisitionModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Collection\ImageBufferModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Collection\ImageSourceModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Collection\LightModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Collection\SaveImage`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\ColorProcessing\IMVSColorExtractModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\ColorProcessing\IMVSColorImageGenerationModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\ColorProcessing\IMVSColorMeasureModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\ColorProcessing\IMVSColorRecognitionModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\ColorProcessing\IMVSColorSegmentModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\ColorProcessing\IMVSColorTransformModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Communication\CameraIOModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Communication\DataAnalysisModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Communication\DataAssembleModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Communication\ReadDatasModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Communication\SendDatasModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\DefectDetection\IMVSCircleEdgeInspModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\DefectDetection\IMVSCircleEdgePairInspModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\DefectDetection\IMVSEdgeFlawInspModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\DefectDetection\IMVSEdgeInspGroupModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\DefectDetection\IMVSEdgePairFlawInspModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\DefectDetection\IMVSEdgePairInspGroupModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\DefectDetection\IMVSEdgePairPosTrendAnalyModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\DefectDetection\IMVSEdgePosTrendAnalyModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\DefectDetection\IMVSInspectModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\DefectDetection\IMVSLineEdgeInspModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\DefectDetection\IMVSLineEdgePairInspModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\DefectDetection\IMVSMarkInspModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\DefectDetection\IMVSMarkInspModuVA`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\DefectDetection\IMVSSurfaceDefectFilterModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Global\CommDeviceModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Global\CommManagerModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Global\DataQueueModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Global\DynamicParamCameraModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Global\GlobalCameraModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Global\GlobalTriggerModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Global\GlobalVariableModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Global\ImageCollectDalsaModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Global\ImageCollectHikModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Global\ImageCollectIKModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Global\ImageCollectModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Global\LightControl`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Global\LocalVariableModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Global\ProcedureComm`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Global\ProcedureModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Global\SCGlobalCameraModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Global\SolutionModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\ImageGeneration\GeometryCreate`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\ImageGeneration\IMVSCircleFitModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\ImageGeneration\IMVSEllipseFitModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\ImageGeneration\IMVSLineFitModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\ImageProcessing\IMVSAffineTransformModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\ImageProcessing\IMVSBinaryModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\ImageProcessing\IMVSFrameMeanModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\ImageProcessing\IMVSGeometricTransformModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\ImageProcessing\IMVSImageCombineProcessModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\ImageProcessing\IMVSImageCorrectCalibModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\ImageProcessing\IMVSImageCorrectManualModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\ImageProcessing\IMVSImageEnhanceModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\ImageProcessing\IMVSImageFilterModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\ImageProcessing\IMVSImageFixtureModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\ImageProcessing\IMVSImageMathModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\ImageProcessing\IMVSImageMorphModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\ImageProcessing\IMVSImageNormlizeModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\ImageProcessing\IMVSImageResizeModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\ImageProcessing\IMVSImageSharpnessModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\ImageProcessing\IMVSImgStitchCalibModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\ImageProcessing\IMVSInverseAffineTransformModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\ImageProcessing\IMVSMultiImageFusionModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\ImageProcessing\IMVSPolarUnwarpModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\ImageProcessing\IMVSRegionCopyModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\ImageProcessing\IMVSShadeCorrectModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Location\IMVSAngleBisectorFindModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Location\IMVSBlobFindLabelsModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Location\IMVSBlobFindModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Location\IMVSCaliperCornerModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Location\IMVSCaliperEdgeModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Location\IMVSCaliperModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Location\IMVSCircleFindModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Location\IMVSContourMatchModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Location\IMVSEllipseFindModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Location\IMVSFastFeatureMatchModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Location\IMVSFixtureModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Location\IMVSGluePathConductModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Location\IMVSGrayMatchModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Location\IMVSGrayMatchModuVA`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Location\IMVSHPFeatureMatchModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Location\IMVSLineFindGroupModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Location\IMVSLineFindModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Location\IMVSMarkFindModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Location\IMVSMatrixCircleFindModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Location\IMVSMedianLineFindModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Location\IMVSMultiLineFindModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Location\IMVSPairLineModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Location\IMVSParallelCalculateModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Location\IMVSPeakFindModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Location\IMVSQuadrangleFindModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Location\IMVSRectFindModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Location\IMVSTargetTrackModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Location\IMVSVerticalLineFindModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Logic\AndModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Logic\BranchModule_STD`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Logic\BranchStringCpmL`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Logic\DataClassificationModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Logic\DataFilterModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Logic\DataRecordModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Logic\DataSetModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Logic\DataSortModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Logic\FormatModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Logic\GraphicsSetModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Logic\IMVSGroup`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Logic\IfBranchModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Logic\IfModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Logic\PointSetMODU_STD`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Logic\PyShellModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Logic\SaveTextModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Logic\ShellModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Logic\StringCompareModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Logic\TimeStatisticModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Logic\TriggerModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Measurement\IMVSC2CMeasureModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Measurement\IMVSEdgeWidthFindModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Measurement\IMVSHistToolModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Measurement\IMVSIntensityMeasureModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Measurement\IMVSL2CMeasureModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Measurement\IMVSL2LMeasureModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Measurement\IMVSP2CMeasureModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Measurement\IMVSP2LMeasureModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Measurement\IMVSP2PMeasureModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Measurement\IMVSPixelCountModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Measurement\IMVSPixelCountModuVA`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Recognition\IMVS2dBcrModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Recognition\IMVSBcrModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Recognition\IMVSMachineLearningClassifierModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Recognition\IMVSOcrModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\SplitCombination\IMVS2dArrayCorrectModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\SplitCombination\IMVSBoxFilterModule`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\SplitCombination\IMVSBoxMergeModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\SplitCombination\IMVSBoxOverlapCalculationModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\SplitCombination\IMVSDivideImageModu`
- `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\SplitCombination\IMVSMultiLabelFilterModu`
- `analysis/decompiled/Apps.ColorFun`
- `analysis/decompiled/Apps.PcUI.ImageSourceAudit`
- `analysis/decompiled/Apps.Style`
- `analysis/decompiled/Apps.XmlParser.CapabilityAudit`
- `analysis/decompiled/Apps.XmlUI`
- `analysis/decompiled/BranchModule_STD.WorkflowAudit`
- `analysis/decompiled/BranchStringCpmL.WorkflowAudit`
- `analysis/decompiled/CompatibleTool.Audit`
- `analysis/decompiled/DynamicObjectConfigControl`
- `analysis/decompiled/ExtensionParamUserControl.CapabilityAudit`
- `analysis/decompiled/IMVSGroup.WorkflowAudit`
- `analysis/decompiled/IfBranchModule.WorkflowAudit`
- `analysis/decompiled/IfModule.WorkflowAudit`
- `analysis/decompiled/ImageSourceModuleControl`
- `analysis/decompiled/VM.Controls`
- `analysis/decompiled/VM.Core`
- `analysis/decompiled/VM.ExternalCall.Audit`
- `analysis/decompiled/VM.PcUI`
- `analysis/decompiled/VM.PlatformSDKCS.Audit`
- `analysis/decompiled/VMBaseControls`
- `analysis/decompiled/VMControls.Data`
- `analysis/decompiled/VMControls.WPF`
- `analysis/decompiled/VisionMasterMain`
- `analysis/decompiled/iMVS-6000PlatformSDKCS.PublicAudit`
- `analysis/decompiled/vServerApp.Audit`
- `analysis/full-audit/batch1-20261107/C01/image-source-blob-vertical-slice.vmx.json`
- `analysis/full-audit/batch1-20261107/C01/image-source-group-vertical-slice.vmx.json`
- `analysis/full-audit/batch1-20261107/C01/image-source-vertical-slice.vmx.json`
- `analysis/full-audit/batch1-20261107/C01/runtime-report.json`
- `analysis/full-audit/batch1-20261107/C03/c03-next-evidence-gate.json`
- `analysis/full-audit/batch1-20261107/C03/c03-phase1-evidence-card.json`
- `analysis/full-audit/batch1-20261107/C03/c03-phase1-evidence-review.json`
- `analysis/full-audit/batch1-20261107/C03/c03-phase1-s0-gate-check.json`
- `analysis/full-audit/batch1-20261107/C03/c03-s0-rerun-matrix-20261110.json`
- `analysis/full-audit/batch1-20261107/C03/rea-native-provider-diagnostic.json`
- `docs/COMPATIBLETOOL-VERSION-MIGRATION.md`
- `docs/COMPOUND-LEAF-MAPPING.md`
- `docs/CONTRACT-REQUESTS.md`
- `docs/DEBUG-SESSION-CONTRACT.md`
- `docs/FRAMEWORK-CONTRACTS.md`
- `docs/FRAMEWORK-PROGRESS-RECONCILIATION.md`
- `docs/FRAMEWORK-REVERSE-CHECKLIST.md`
- `docs/FRAMEWORK-WORKTREE-RETIREMENT.md`
- `docs/GROUP-ADVANCED-RUNTIME.md`
- `docs/GROUP-FULL-RECONSTRUCTION-CHECKLIST.md`
- `docs/GROUP-INDEX-COMPOUND.md`
- `docs/GROUP-LOCAL-VARIABLE-BINDING.md`
- `docs/GROUP-LOCAL-VARIABLE-TYPE.md`
- `docs/GROUP-LOOP-BOUNDARIES.md`
- `docs/GROUP-NATIVE-EDITORS.md`
- `docs/GROUP-RELATION-IDENTITY.md`
- `docs/LOADER-ROUND2-20261019.md`
- `docs/MAIN-PENDING-REVIEW.md`
- `docs/MF01-FRONTEND-UI-SOURCE-AUDIT.md`
- `docs/MF01-GLOBAL-ENTRY-AUDIT.md`
- `docs/MF01-SOLUTION-PATH-ACCEPTANCE.md`
- `docs/MF01-TITLE-MENU-ACCEPTANCE.md`
- `docs/MF01-WORKAREA-LOCK-PROMPT.md`
- `docs/MF02-CONNECTION-ROUTE-LIFECYCLE.md`
- `docs/MF05-CONTINUOUS-RUN-CALLCHAIN.md`
- `docs/MF05-PROCEDURE-RUN-CALLCHAIN.md`
- `docs/MF05-PROCESS-LIFETIME-AUDIT.md`
- `docs/MF05-RESULT-LIFECYCLE.md`
- `docs/PLUGIN-FRAMEWORK-BOUNDARY.md`
- `docs/ROI-REAL-TOOL.md`
- `docs/SOL-ROUND2-20261019.md`
- `docs/SOLUTION-PERSISTENCE-BOUNDARY.md`
- `docs/TOOL-DISCOVERY-METADATA.md`
- `docs/UNDO-ROUND2-20261019.md`
- `docs/VARIABLE-TAB-ROUND2-20261019.md`
- `docs/VM-LOCAL-RESTORATION-STATUS.json`
- `docs/WORKFLOW-TAB-TOOLBOX-GROUP-CLOSURE.md`
- `docs/architecture-recovery/window-layout.edges.json`
- `docs/architecture-recovery/xaml-connector-evidence.json`
- `docs/architecture-recovery/xaml-entry-inventory.json`
- `docs/architecture-recovery/xaml-handler-map.json`
- `docs/framework-acceptance.json`
- `docs/framework-progress-audit.json`
- `docs/framework-reverse-plan.json`
- `docs/plugins/inventory.json`
- `docs/reverse/A-canvas-implementation-batch10.md`
- `docs/reverse/A-container-zoom-implementation-batch12.md`
- `docs/reverse/A-loader-failure-matrix-batch19.md`
- `docs/reverse/A-runtime-editor-batch18.md`
- `docs/reverse/A-shell-implementation-batch11.md`
- `docs/reverse/A01-loader-batch1.md`
- `docs/reverse/A01-palette-lifecycle-implementation-batch15.md`
- `docs/reverse/A03-binding-batch4.md`
- `docs/reverse/A03-canvas-replacement-fix.md`
- `docs/reverse/A03-closure-source-review-batch12.md`
- `docs/reverse/A03-tabs-batch2.md`
- `docs/reverse/A03-zoom-history-batch3.md`
- `docs/reverse/A05-entry-reconciliation-batch17.md`
- `docs/reverse/A05-full-menu-map-batch17.md`
- `docs/reverse/A05-input-registration-batch31.md`
- `docs/reverse/A06-close-dependency-batch13.md`
- `docs/reverse/A06-frontend-gate-batch24.md`
- `docs/reverse/A07-managed-loader-partial-batch16.md`
- `docs/reverse/A08-shell-input-batch30.md`
- `docs/reverse/B-identity-semantic-map-batch15.md`
- `docs/reverse/B01-first-fill-batch3.md`
- `docs/reverse/B01-lazy-root-batch4.md`
- `docs/reverse/B02-identity-preflight-batch6.md`
- `docs/reverse/C01-framework-image-source-vertical-slice.md`
- `docs/reverse/C01-registration-batch5.md`
- `docs/reverse/C01-undo-batch1.md`
- `docs/reverse/C02-callbacks-batch4.md`
- `docs/reverse/C02-config-write-batch3.md`
- `docs/reverse/C03-incremental-b1-contracts.md`
- `docs/reverse/C03-incremental-b1-static.md`
- `docs/reverse/C03-incremental-batch0-b1.md`
- `docs/reverse/C03-phase1-evidence-review.md`
- `docs/reverse/D-container-evidence-batch11.md`
- `docs/reverse/D-frontend-migration-implementation-batch13.md`
- `docs/reverse/D01-native-save-batch7.md`
- `docs/reverse/D01-save-callback-batch5.md`
- `docs/reverse/D02-asset-history-batch3.md`
- `docs/reverse/D02-storage-batch2.md`
- `src/FrontendDesignMigration`
- `src/SolutionInspector`
- `src/VisionCore`
- `src/VisionCore/Algorithms.cs`
- `src/VisionData`
- `src/VisionEditor`
- `src/VisionEditor.Core`
- `src/VisionFramework`
- `src/VisionFramework/GraphExecutor.cs`
- `src/VisionGraphRunner`
- `src/VisionIO`
- `src/VisionModules`
- `src/VisionModules/BuiltinModules.cs`
- `src/VisionPluginContracts`
- `src/VisionPlugins.BranchModule`
- `src/VisionPlugins.BranchString`
- `src/VisionPlugins.Calibration`
- `src/VisionPlugins.CalibrationBoard`
- `src/VisionPlugins.CameraMapping`
- `src/VisionPlugins.ContourMatch`
- `src/VisionPlugins.CoordinateModule`
- `src/VisionPlugins.DistortionCalibration`
- `src/VisionPlugins.Fixture`
- `src/VisionPlugins.GrayMatch`
- `src/VisionPlugins.IfModule`
- `src/VisionPlugins.ImageBinary`
- `src/VisionPlugins.ImageSource`
- `src/VisionPlugins.ImageSource/ImageAcquisitionPlugin.cs`
- `src/VisionPlugins.ImageSource/ImageSourcePlugin.cs`
- `src/VisionPlugins.MapCalibration`
- `src/VisionRunner`
- `src/VisionRunner/OverlayPng.cs`
- `src/VisionUi`
- `src/VisionWorkbench`
- `src/VisionWorkbench/ImageCanvas.cs`
- `src/VisionWorkflow`
- `src/VisionWorkflow/OfflineCircle.cs`
- `tests/ArchitectureRecovery/original-members.map.json`
- `tests/ArchitectureRecovery/tool-implementation-specs.map.json`
- `tests/VMVerticalSlice.C01/VMVerticalSlice.C01.csproj`

## 附录 D 站点锚点（524 项）

任务项内联锚点指向 `docs/reverse-plan/` 对应页面；校验器逐项核对 id 存在。

## 附录 E 完成度口径

本清单的任务数、阶段门数或测试数**不得**冒充完成度。权威口径仍为 `docs/SOURCE-LEVEL-REVERSE-PROGRESS.json` 的 10/10 = 100%（仅标定授权范围，人工验收单列）；S0 本地切片闭环达成，但 C03 原厂运行时根因仍开放、差分 `45/971486` 对 `47/973202` 未解释。

## 附录 F 规格台账与证据卡索引

- `tests/ArchitectureRecovery/original-members.map.json` / `docs/ORIGINAL-MEMBERS-INDEX.md`：25 唯一程序集按 canonical 归并 28 棵反编译树，2614 `.cs`、3719 类型声明、10128 方法签名；与基线去重口径差异原样登记。
- `tests/ArchitectureRecovery/tool-implementation-specs.map.json` / `docs/TOOL-IMPLEMENTATION-SPECS.md`：162 工具六项规格、14 分类、2015 个带哈希证据文件、160 high + 2 mid；native 函数体全部标 `not-recovered`。
- `analysis/full-audit/batch1-20261107/C03/c03-s0-rerun-matrix-20261110.json`：S0 复跑、12 张标定样本与 11 个通过回归 / 2 个外部数据阻塞回归。
- `analysis/full-audit/batch1-20261107/C03/c03-incremental-b1-contracts.json` / `docs/reverse/C03-incremental-b1-contracts.md`：9 个 `Components.*` 投影 leaf 与 Group `image/width` 方案输出名闭合；边界为 managed projection，不是原厂 ABI。

## 附录 G 启动链路与整屏空载初始态工作面

与第 18 页「运行态」17 控件清单**并列、状态面不同**（本附录为程序刚启动、未加载方案的空载初始态），不产生第二套完成度、不改动本节清单的 544 项任务数。

- 台账 `tests/ArchitectureRecovery/startup-initial-ui.map.json` 与派生层 `plan-startup-initial-ui.cjs`：11 个启动步骤 / 10 个初始态控件，126 个六段链节点全 `unknown`、三级验收零预填。
- 可派工清单 `docs/STARTUP-INITIAL-UI-TASK-LIST.md`：启动链路时间线（原厂方法名与行号）、控件分区、六段链状态、unknown 与解除条件、21 张任务卡。
- 证据结论 `docs/reverse/C03-startup-initial-ui-evidence.md` 与证据卡 `analysis/full-audit/batch1-20261107/C03/c03-startup-initial-ui-20261110.json`（51 条去重证据路径、带 SHA-256）。
- 边界：源码级证据闭环，不改 `src/`、不跑方案测试、未做原厂同状态差分与自研复跑；截图只作控件范围识别。E0–E7 门与 W/K/C/U 布局边映射见清单 §5，8 条未关联边如实登记。
