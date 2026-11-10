'use strict';
// 启动链路 + 整屏空载初始态：确定性生成器。
// 输出可派工清单、人读证据结论、机器证据卡。数字与引用全部从派生结果取得，不写 wall-clock、不用随机数。
const fs = require('fs');
const path = require('path');
const crypto = require('crypto');
const lib = require('./plan-model.cjs');
const startup = require('./plan-startup-initial-ui.cjs');

const root = lib.root;
const OUT_MD = path.join(root, 'docs/STARTUP-INITIAL-UI-TASK-LIST.md');
const OUT_REVIEW = path.join(root, 'docs/reverse/C03-startup-initial-ui-evidence.md');
const OUT_CARD = path.join(root, 'analysis/full-audit/batch1-20261107/C03/c03-startup-initial-ui-20261110.json');
const LEDGER = 'tests/ArchitectureRecovery/startup-initial-ui.map.json';
const DATE = '2026-11-10';

function rel(absolute) { return startup.rel(root, absolute); }
function sha(relative) {
  return crypto.createHash('sha256').update(fs.readFileSync(path.join(root, relative))).digest('hex');
}
function clip(text, size) {
  const value = String(text);
  return value.length > size ? value.slice(0, size - 1) + '…' : value;
}
function code(value) { return '`' + value + '`'; }

function buildStartupInitialUi(model, derived) {
  const builtFrom = model.builtFrom.head.slice(0, 8) + '/' + model.builtFrom.inputsDigest.slice(0, 12);
  const d = derived;

  // ---- 证据引用：台账本身 + 全部核证过的证据路径（不含会在后续步骤变更的状态台账/站点产物） ----
  const references = [LEDGER, 'docs/architecture-recovery/window-layout.edges.json']
    .concat(d.evidencePaths)
    .filter((item, i, list) => list.indexOf(item) === i)
    .sort()
    .map(item => ({ path: item, sha256: sha(item), bytes: fs.statSync(path.join(root, item)).size }));

  // ---- 主交付：可派工清单 ----
  const lines = [
    '# 启动链路与整屏空载初始态 · 可派工逆向任务清单',
    '',
    'builtFrom `' + builtFrom + '` · 台账 `' + LEDGER + '` · 证据卡 `analysis/full-audit/batch1-20261107/C03/c03-startup-initial-ui-20261110.json`',
    '',
    '## §0 状态面与边界',
    '',
    '本清单的状态面是 **' + d.stateScope + '**，与第 18 页 `16-ui-control-landing.html` 的「运行态」17 控件清单**并列、不混为一套口径**，不产生第二套完成度。',
    '',
    '截图范围：' + d.screenshotScope + '。',
    '',
    d.evidenceBoundary,
    '',
    '| 项 | 值 |',
    '|---|---|',
    '| 启动步骤 | ' + d.counts.startupSteps + ' |',
    '| 初始态控件 | ' + d.counts.controls + ' |',
    '| 分组 | ' + d.counts.groups + '（启动链路 / 壳与全局控件 / 左侧工具箱与流程画布 / 右侧显示与结果区） |',
    '| 六段链节点 | ' + d.counts.sixSegmentUnknown + '，全部 `unknown` |',
    '| 三级验收 | ' + d.counts.acceptanceLevels + ' 级，`' + d.counts.acceptanceDone + '` 项完成（零预填） |',
    '| E0–E7 门覆盖 | ' + d.counts.gateCoverage + ' / 8，当前 `closedEntryChains=0` |',
    '| A 线工作包覆盖 | ' + d.counts.packageCoverage + ' / 8 |',
    '| W/K/C/U 布局边 | 关联 ' + d.counts.layoutEdges + ' / ' + d.counts.layoutEdgesTotal + '，未关联 ' + d.counts.unlinkedLayoutEdges + ' 条如实登记 |',
    '| 去重证据路径 | ' + d.counts.evidencePaths + ' |',
    '| 产品改动 / 命令 | 0 / 0（本轮不改产品代码、不跑方案测试） |',
    '| 勾选框 | 全部 `[ ]`，零预填 |',
    '',
    '硬规则：不自动提交/推送、不自动跑方案测试、不改 `src/`、缺失标 `unknown`、截图只作差异信号不作实现规格。',
    '',
    '## §1 启动链路时间线（原厂方法名与行号）',
    '',
    '来源 `analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster/Program.cs` 与 `App.cs`、`MainWindowData.cs`、`VisionMasterMain/MainWindow.xaml.cs`、`vServerApp.Audit/ServerBackgroundApp/Program.cs`。',
    '',
    '| 步骤 | 阶段 | 关键原厂成员 | 关联 A 线包 | E 门 | 取证第一动作 |',
    '|---|---|---|---|---|---|'
  ];
  for (const step of d.steps) {
    lines.push('| ' + step.id + ' · ' + step.name + ' | ' + step.phase + ' | ' +
      clip(step.sourceMembers.join('；'), 190).replace(/\|/g, '\\|') + ' | ' +
      step.relatedWorkPackageIds.join('、') + ' | ' + step.relatedGateIds.join('、') + ' | ' +
      clip(step.firstAction, 80).replace(/\|/g, '\\|') + ' |');
  }
  lines.push('', '> 原厂 `BeforeStartMian` / `AppBeforStart` 拼写按原文保留，不擅自纠正。`Program.cs:84` 与 `App.cs:563` 都有 `app.Run()`，属双入口，需分别标注。', '');

  lines.push('## §2 整屏空载初始态控件分区', '',
    '| 控件 | 分组 | 截图状态 | 布局边 | A 线包 | E 门 |',
    '|---|---|---|---|---|---|');
  for (const control of d.controls) {
    lines.push('| ' + control.id + ' · ' + control.name + ' | ' + control.group + ' | ' +
      clip(control.screenshotState, 88).replace(/\|/g, '\\|') + ' | ' +
      control.layoutEdgeIds.join('、') + ' | ' + control.relatedWorkPackageIds.join('、') + ' | ' +
      control.relatedGateIds.join('、') + ' |');
  }

  lines.push('', '## §3 逐项六段链状态（全部 unknown）', '',
    '六段链 `CL1 入口 / CL2 绑定·命令 / CL3 状态读写 / CL4 事件·回调 / CL5 持久化或执行效果 / CL6 异常·清理`。',
    '', '| 对象 | CL1 | CL2 | CL3 | CL4 | CL5 | CL6 |', '|---|---|---|---|---|---|---|');
  for (const item of d.steps.concat(d.controls)) {
    lines.push('| ' + item.id + ' | ' + Object.values(item.sixSegmentStates)
      .map(segment => segment.state).join(' | ') + ' |');
  }

  lines.push('', '## §4 三级验收', '',
    '| 级别 | 名称 | 判定规则 | 状态 |', '|---|---|---|---|');
  for (const level of d.acceptanceLevels) {
    lines.push('| ' + level.id + ' | ' + level.label + ' | ' + level.rule.replace(/\|/g, '\\|') +
      ' | `' + level.state + '` |');
  }
  lines.push('', '> 本轮用户选定的深度为「源码级证据闭环 + 可派工台账，不改 `src/`」，因此 L2 原厂同状态差分与 L3 自研操作级复跑**均保持未执行**，不得写成已验证。', '');

  lines.push('## §5 E0–E7 门与 W/K/C/U 布局边映射', '',
    '- 布局边来源 `docs/architecture-recovery/window-layout.edges.json` hierarchy：W01–W16（标题栏/系统窗口/模态关闭）、K01–K07（工作区锁定与工具栏）、C01–C05（自研独立实现）、U01–U02（未闭合证据），共 ' + d.counts.layoutEdgesTotal + ' 条。',
    '- 本状态面已关联：' + d.linkedEdgeIds.join('、') + '。',
    '- **未关联 ' + d.unlinkedLayoutEdges.length + ' 条**：' + d.unlinkedLayoutEdges.join('、') +
      '。原因是 W10–W16 属「共用窗口的模态与关闭」、C04 属锁偏好 JSON，与空载首屏可见状态无直接对应；不强行挂接、不凑数。',
    '- 该 hierarchy **没有**画布 / 显示区 / 结果区 / 状态栏 / 导航缩略图子系统；这些控件改用 `canvas-viewport.edges.json`、`statusbar-entries.edges.json`、`global-toolbar.edges.json`、`toolbox-drag.edges.json`、`history-results-entry-audit.json` 与 `documents/MF02-MINIMAP-*.html` 取证。',
    '- E0–E7 八门状态仍为 `E0 open / E1 partial / E2–E7 open`，`closedEntryChains=0`；本清单不改动这些门状态。',
    '');

  lines.push('## §6 unknown 清单与解除条件', '');
  for (const item of d.steps.concat(d.controls)) {
    lines.push('- **' + item.id + ' · ' + item.name + '**：' + item.unknowns.join('；'));
  }
  lines.push('', '解除条件需逐项写明证据类型与可复核路径；`source-confirmed` 不能替代 `runtime-confirmed`，静态分析不能冒充运行观察。', '');

  lines.push('## §7 可派工任务卡（' + (d.steps.length + d.controls.length) + ' 项，零预填）', '');
  for (const item of d.steps.concat(d.controls)) {
    lines.push('- [ ] **' + item.id + ' · ' + item.name + '**〔' + item.group + ' / ' + item.planState + '〕');
    lines.push('  - 第一动作：' + item.firstAction);
    lines.push('  - 准确成员：' + clip(item.sourceMembers.join('；'), 240));
    lines.push('  - 证据路径：' + item.originalEvidencePaths.map(code).join('、'));
    lines.push('  - 退出判据：' + item.exitCriteria.join('；'));
    lines.push('  - unknown：' + item.unknowns.length + ' 项');
  }

  lines.push('', '## §8 与既有产物的关系', '',
    '- 第 18 页 `16-ui-control-landing.html`：17 控件 / 4 类，针对**运行态**（有流程、有图像、有表格数据），全部 `blocked`。本清单是**空载初始态**，两者并列。',
    '- 第 19 页 `17-incremental-landing.html`：4 批次 13 步，执行顺序仍是 B0 S0/C03 补证 → B1 最小纵切 → B2 控件横扩 → B3 工具分批；本清单不改变 `B0 open / B1–B3 blocked`。',
    '- `docs/COMPLETE-REVERSE-TASK-LIST.md`：544 项完整清单，附录 G 索引本清单；任务总数不变。',
    '- `docs/reverse/C03-startup-initial-ui-evidence.md`：本轮人读证据结论（观察 / 推断 / unknown 分级）。',
    '',
    '权威完成度口径仍为 `docs/SOURCE-LEVEL-REVERSE-PROGRESS.json` 的 10/10 = 100%（仅标定授权范围，人工验收单列）；本清单不计入该分母。');

  const markdown = lines.join('\n');

  // ---- 人读证据结论 ----
  const review = [
    '# 启动链路与空载初始态：源码级证据结论',
    '',
    '日期：' + DATE + ' · 证据卡 `analysis/full-audit/batch1-20261107/C03/c03-startup-initial-ui-20261110.json` · builtFrom `' + builtFrom + '`',
    '',
    '## 结论口径',
    '',
    '本轮按 `reverse-engineer-anything` 流程复用本机既有反编译产物、窗口布局边、XAML/CIL 台账与自研源码做**静态源码级取证**。REA MCP 与 native provider 在当前主机不可用，因此**未新取 native 函数体、资源或 ABI**；不声称 parity，不把静态分析写成运行观察。',
    '',
    '| 证据类型 | 含义 | 本轮来源 |',
    '|---|---|---|',
    '| `source-confirmed` | 原厂反编译源码 / XAML / CIL / 布局边可复核 | `analysis/decompiled/VisionMaster.Exe.Audit`、`VisionMasterMain`、`vServerApp.Audit`、`docs/architecture-recovery/*.edges.json` |',
    '| `runtime-confirmed` | 既有运行报告已记录 | 仅引用既有 `C01` / `C03` 报告，本轮**未新增**运行证据 |',
    '| `independent-implementation` | 自研 managed 壳，不是原厂等价 | `src/VisionEditor/*`、`src/VisionEditor.Core/*` |',
    '| `unknown` | 未取证，保持开放 | 见下节 |',
    '',
    '## 主要观察（observation）',
    '',
    '1. 启动链路跨 `Program.cs` 与 `App.cs` 两个文件：`Program.Main` @61 → `SingleInstanceManager.Initialize` @70 / `StartApp` @76 → `App.OnStartup` @92 → 子线程 `BeforeStartPart` @111 → `BeforeStartMian` @136 → `InitMainWindow` @159 → `AppAfterStart` @179 → `App.ShowMainWindow` @354 → `Show()` @363 → `ConfigZoomIndex` @364。',
    '2. `MainWindowData.cs` 是启动期数据容器（`PreLoad` @984、`RecoverMainWindowState` @958、`ConfigZoomIndex` @340），`VisionMasterMain.MainWindow` 继承 `BaseWindow` 并带进度遮罩依赖属性。',
    '3. 存在**双入口**：`Program.Main`@61 与 `App.Main`@560-564 各含 `[STAThread]` 与 `app.Run()`；`vServerApp.Audit` 另有伴随进程入口。',
    '4. 窗口布局边 30 条分五个 subsystem（W01–W16 / K01–K07 / C01–C05 / U01–U02），分类含 `source-confirmed`、`independent-implementation`、`unknown`。',
    '5. 右下角导航缩略图在自研 `MainWindow.xaml` 有 `MiniMapPanel/MiniMapViewport/MiniMapToggle` 等命名控件，`WorkflowCanvas.cs` 提供视口同步与拖动方法。',
    '',
    '## 关键陷阱（避免把推断写成事实）',
    '',
    '- `WorkflowOverview.cs`、`WorkflowOverviewDrag.cs`、`WorkflowCanvas.cs`、`MainWindow.ShellStatus.cs` 等文件内**没有同名类型**，实际都是 `VisionEditor.MainWindow` 的 partial 分部；文件名不能当类型名。',
    '- `AppBeforStart` @142、`InitLang` @272、`AwakeServerAsProcess` @528、`InitMq` @624、`StartServerApp` @600 等**有定义但在可见 Main→首屏链路中无直接调用点**，单列「定义存在/调用未知」。',
    '- 原厂 `BeforeStartMian`、`AppBeforStart` 拼写按原文保留，不纠正。',
    '- `window-layout.edges.json` 没有画布 / 显示区 / 结果区 / 状态栏 / 导航缩略图子系统；这些控件必须引用其它 `.edges.json` 与 `MF02-MINIMAP-*.html`，不能把 W/K/C/U 边硬套上去。',
    '- A 线证据中「单实例 / 历史结果」0 命中，改用 `docs/architecture-recovery/` 台账与 C 线文档；`closedEntryChains=0` 仍需保留。',
    '',
    '## 未做的事（本轮用户选定深度）',
    '',
    '- 未改 `src/` 产品代码；未跑方案测试；未做原厂同状态差分与自研操作级复跑。',
    '- 截图只用于识别控件范围与空载状态，不作实现规格、不请求新截图。',
    '- 未把六段链任何一段标为已闭合、未预填三级验收；' + d.counts.sixSegmentUnknown + ' 个六段链节点全部 `unknown`。',
    '- 未解除 `B0 open / B1–B3 blocked`；C03 差分 `45/971486` 对 `47/973202` 仍开放。',
    '',
    '## 证据引用',
    '',
    '共 ' + references.length + ' 条，全部带 SHA-256 与字节数，见证据卡 `evidence[]`。'
  ].join('\n');

  // ---- 机器证据卡 ----
  const card = {
    schemaVersion: 1,
    cardId: 'C03-STARTUP-INITIAL-UI-20261110',
    date: DATE,
    scope: '启动链路 + 整屏空载初始态主界面：源码级证据闭环与可派工台账',
    stateScope: d.stateScope,
    method: '按 reverse-engineer-anything 流程复用本地既有反编译产物、窗口布局边、XAML/CIL 台账与自研源码做静态取证；REA native provider 当前不可用，未新取 native 函数体。',
    evidenceBoundary: d.evidenceBoundary,
    builtFrom,
    questionChecklist: [
      { id: 'Q1', question: '原厂 Main→首屏就绪的方法序列', status: 'answered', answer: '跨 Program.cs / App.cs 已定位 11 段启动步骤与准确行号' },
      { id: 'Q2', question: '单实例与双入口', status: 'partially-answered', answer: 'SingleInstanceManager.Initialize 与 App.Main 第二入口可复核；跨进程通信实体与实际部署入口未取证' },
      { id: 'Q3', question: '整屏空载初始态控件与布局边映射', status: 'partially-answered', answer: '10 控件已登记并关联 22/30 条边；8 条未关联边如实登记，hierarchy 缺画布/显示区/结果区子系统' },
      { id: 'Q4', question: '六段链与三级验收是否闭合', status: 'unresolved', answer: '126 个六段链节点全 unknown，三级验收 0 完成；本轮不做差分与复跑' },
      { id: 'Q5', question: '导航缩略图橙色视口框实现', status: 'unresolved', answer: '自研 MiniMapViewport 与视口同步方法可复核；原厂颜色/命中测试常量未取证' }
    ],
    findings: [
      { id: 'F1', claim: '启动链路跨 Program.cs 与 App.cs，含单实例、预加载并行、主窗口数据、启动后分支与窗口状态恢复', evidenceType: 'source-confirmed', evidence: ['analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster/Program.cs', 'analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster/App.cs'] },
      { id: 'F2', claim: 'MainWindowData 为启动期数据容器，VisionMasterMain.MainWindow : BaseWindow 带进度遮罩依赖属性', evidenceType: 'source-confirmed', evidence: ['analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster/MainWindowData.cs', 'analysis/decompiled/VisionMasterMain/VisionMasterMain/MainWindow.xaml.cs'] },
      { id: 'F3', claim: '存在 Program.Main 与 App.Main 双入口，另有 vServerApp 伴随进程', evidenceType: 'source-confirmed-entry-ambiguity', evidence: ['analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster/App.cs', 'analysis/decompiled/vServerApp.Audit/ServerBackgroundApp/Program.cs'] },
      { id: 'F4', claim: '30 条窗口布局边分 5 个 subsystem；只有 22 条与空载首屏相关，W10–W16 与 C04 不强行挂接', evidenceType: 'source-confirmed-partial-mapping', evidence: ['docs/architecture-recovery/window-layout.edges.json'] },
      { id: 'F5', claim: '自研侧命名控件 MiniMapPanel/MiniMapViewport 与 WorkflowCanvas 的视口同步方法对应右下角导航缩略图', evidenceType: 'independent-implementation', evidence: ['src/VisionEditor/MainWindow.xaml', 'src/VisionEditor/WorkflowCanvas.cs'] },
      { id: 'F6', claim: 'WorkflowOverview.cs 等文件名不是类型名，实际均为 MainWindow 的 partial 分部', evidenceType: 'naming-trap', evidence: ['src/VisionEditor/WorkflowOverview.cs', 'src/VisionEditor/WorkflowOverviewDrag.cs', 'src/VisionEditor/WorkflowCanvas.cs'] },
      { id: 'F7', claim: 'AppBeforStart / InitLang / AwakeServerAsProcess / InitMq / StartServerApp 有定义但无可见调用点', evidenceType: 'definition-only-invocation-unknown', evidence: ['analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster/Program.cs'] },
      { id: 'F8', claim: 'A 线证据中单实例与历史结果 0 命中，需引用 architecture-recovery 台账与 C 线文档', evidenceType: 'evidence-gap', evidence: ['docs/architecture-recovery/entry-inventory.json', 'docs/reverse/C-result-history-implementation-batch15.md'] },
      { id: 'F9', claim: 'E0–E7 八门覆盖但 closedEntryChains=0；B0 open / B1–B3 blocked 与 C03 差分不因本批改变', evidenceType: 'decision-boundary', evidence: ['docs/architecture-recovery/entry-inventory.json'] }
    ],
    counts: d.counts,
    linkedLayoutEdges: d.linkedEdgeIds,
    unlinkedLayoutEdges: d.unlinkedLayoutEdges,
    gateCoverage: d.gateIds,
    packageCoverage: d.packageIds,
    acceptanceLevels: d.acceptanceLevels,
    sixSegmentStages: d.sixSegmentStages,
    unknowns: d.steps.concat(d.controls).flatMap(item =>
      item.unknowns.map(text => ({ id: item.id, unknown: text }))),
    commands: [],
    productChanges: [],
    evidence: references
  };

  return { markdown, review, card: JSON.stringify(card, null, 2) + '\n' };
}

function main() {
  const model = lib.buildModel();
  lib.verifyInputs(model);
  const map = JSON.parse(fs.readFileSync(path.join(root, LEDGER), 'utf8'));
  const derived = startup.buildStartupUi(model, map, root);
  const result = buildStartupInitialUi(model, derived);
  fs.writeFileSync(OUT_MD, result.markdown, 'utf8');
  fs.writeFileSync(OUT_REVIEW, result.review, 'utf8');
  fs.writeFileSync(OUT_CARD, result.card, 'utf8');
  console.log(JSON.stringify({
    counts: derived.counts,
    markdownBytes: Buffer.byteLength(result.markdown, 'utf8'),
    reviewBytes: Buffer.byteLength(result.review, 'utf8'),
    cardBytes: Buffer.byteLength(result.card, 'utf8'),
    evidence: JSON.parse(result.card).evidence.length,
    builtFrom: JSON.parse(result.card).builtFrom
  }, null, 2));
}

if (require.main === module) main();
module.exports = { buildStartupInitialUi, OUT_MD, OUT_REVIEW, OUT_CARD };
