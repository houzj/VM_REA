# 联合审核第四轮根线自审：输入来源、二值化 SDK 与 UI 响应

基线为 main `31f0dee5f345e6c73cf2e2d3e14d91987b049547` 加本轮明确的在制文件；其他未提交工作区内容不算本轮交付。三条证据线分别见 [输入源显示](20261020-pass4-source-label.md)、[原生 SDK](20261020-pass4-binary-sdk.md)、[空闲 UI](20261020-pass4-idle-ui.md)。本轮未修改 `worktrees/tool-plugins`，未接触用户桌面窗口或其他任务进程。

## 实施与自审

main 宿主 `ToolExecutionScope.ConnectToolEditorServices` 的 `SubscriptionLabel` 此前仅读 `Configuration.Subscriptions`。实际画布接线由 `ProcedureDocument.Connections` 持有，故当前 ImageSource→ImageBuffer 有真实执行边但配置窗来源空白。原版 `Apps.XmlParser` 的 `GetSelectedCombinationOperation.Operate` 读目标 IO 当前订阅关系，`SetCombinationSourceOperation.Operate` 设置或清除 `RelateIO/RelateIndex`。按此证据，本轮在没有类型化订阅时从当前连接取来源节点与端口；有订阅时仍优先用订阅记录。字符串排版为当前宿主实现，尚未确认为原厂路径文本。

新增 `tests/WorkflowInputSourcePresentation.Checks`，使用真实 `MainWindow`、ImageSource/ImageBuffer 和插件配置窗核未连接、连接、断开、重连、保存重开、新配置上下文、订阅替换、清订阅与撤销共七项检查，7/7 通过。最终日志 `analysis/joint-audit/input-source-presentation/checks.log` SHA-256 `35B255988E524AD52A028E6AE37D7A6CB2B38B2C584952746612A6A8D89D5A37`。初版测试缺 `System.IO` 的构建失败日志仍保留。根线审阅产品 diff 限于显示桥；没有改变端口 schema、算法、持久化格式或插件代码。

## 新取证和仍开放的问题

原版 `MVDImageBinaryCpp.dll` 的真实虚表和汇编现定位 `SetInputImage=0x180001ac0`、`SetRunParam=0x180002000`、`Run=0x1800021b0`，以及算法 ID `0x25a` 的 `CreateHandle`/`Process` 入参；88 个包装层导出文件与逐文件哈希已保全。53 MB `MVD_Algorithm.dll` 全量 Ghidra 分析因 Java heap OOM，失败日志保留。五模式算法、实际格式门、错误释放顺序仍为 `blocked-exact-evidence`，不得把包装层地址当核心算法恢复。

自有离屏真实 ImageSource→ImageBinary 流程成功后，空闲心跳、刷新、选择、切流程、保存未出现长段；打开二值化配置窗在无 dump 对照中同步占 UI 线程 3.94–5.95 秒。根线重跑 64×64 探针又观测 4867.94 ms 心跳中断，预期诊断退出码 1；初步拆段显示长段在 `Window.Show()`，尚未解出具体 WPF 调用栈。用户报告的长期/空闲卡死仍未复现；不以临时长停顿推定其根因，不把离屏探针当人工桌面验收。

下一轮须从自有 minidump 或分段 WPF 取证确定 `Show()` 内热点；原生库改用新私有工程 `-noanalysis` 定点跟踪 `0x25a` 分派，或做受控 SDK 格式/五模式差分。J04 仍缺原厂跨 Group/变量、排序、通知与 `.sol` 重开差分；J07 和 J12 未关闭。联合审核 19 个工作区映射对象（含 Group）的 J01–J12 全矩阵未关闭，本轮修的是已有框架合同切片，具名框架实施进度仍为 **41/96=42.71%**；人工验收另列，不计分。
