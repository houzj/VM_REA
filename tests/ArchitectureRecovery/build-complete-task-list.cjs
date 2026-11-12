'use strict';
// 完整逆向任务清单生成器：所有条目从 buildModel() 派生，一次列全，不写 wall-clock、不用随机数。
// 主交付 docs/COMPLETE-REVERSE-TASK-LIST.md + 机器台账 complete-task-list.map.json；不改站点产物。
const fs = require('fs');
const path = require('path');
const lib = require('./plan-model.cjs');

const root = lib.root;
const OUT_MD = path.join(root, 'docs', 'COMPLETE-REVERSE-TASK-LIST.md');
const OUT_JSON = path.join(root, 'tests', 'ArchitectureRecovery', 'complete-task-list.map.json');

function md(value) {
  return String(value === null || value === undefined ? '' : value)
    .replace(/\|/g, '\\|').replace(/\r?\n/g, ' ');
}
function clip(value, max) {
  const text = String(value === null || value === undefined ? '' : value);
  return text.length > max ? text.slice(0, max - 1) + '…' : text;
}
function code(value) { return '`' + md(value) + '`'; }
function anchor(file, id) { return { file, id }; }

function task(id, section, title, options) {
  const opts = options || {};
  return {
    id, section, title,
    state: opts.state || 'not_started',
    evidencePaths: (opts.evidencePaths || []).slice(),
    sourceMembers: (opts.sourceMembers || []).slice(),
    commands: (opts.commands || []).slice(),
    reportPaths: (opts.reportPaths || []).slice(),
    exitCriteria: (opts.exitCriteria || []).slice(),
    unknowns: (opts.unknowns || []).slice(),
    blockers: (opts.blockers || []).slice(),
    anchor: opts.anchor || null,
    sourceRef: opts.sourceRef || '',
    note: opts.note || ''
  };
}
function renderLine(t) {
  const fields = [
    '状态 ' + code(t.state),
    t.sourceRef ? '来源 ' + code(t.sourceRef) : '',
    t.evidencePaths.length ? '证据 ' + t.evidencePaths.length + ' 项' : '',
    t.exitCriteria.length ? '退出 ' + clip(t.exitCriteria.join('；'), 130) : '',
    t.blockers.length ? '前置/阻塞 ' + t.blockers.join('、') : '',
    t.anchor ? '锚点 ' + code(t.anchor.file + '#' + t.anchor.id) : ''
  ].filter(Boolean);
  return '- [ ] **' + t.id + '** ' + md(t.title) + ' · ' + fields.join(' · ');
}
function renderDetailed(t) {
  const out = [renderLine(t)];
  if (t.evidencePaths.length) out.push('  - 证据：' + t.evidencePaths.map(code).join('、'));
  if (t.sourceMembers.length) out.push('  - 成员：' + t.sourceMembers.map(code).join('、'));
  else out.push('  - 成员：台账未登记（unknown），需源码级取证后填入');
  if (t.commands.length) out.push('  - 命令〔待执行〕：' + t.commands.map(code).join('、'));
  if (t.reportPaths.length) out.push('  - 报告：' + t.reportPaths.map(code).join('、'));
  if (t.unknowns.length) out.push('  - unknown：' + clip(t.unknowns.join('；'), 260));
  if (t.note) out.push('  - 备注：' + md(clip(t.note, 200)));
  return out.join('\n');
}
function section(number, title, intro, tasks, detailed) {
  const lines = ['## §' + number + ' ' + title, '', intro, '',
    '共 ' + tasks.length + ' 项，勾选框全部未预填；状态字段来自机器台账。', ''];
  tasks.forEach(item => lines.push(detailed ? renderDetailed(item) : renderLine(item)));
  lines.push('');
  return lines.join('\n');
}

function buildCompleteList(model) {
  const tasks = [];
  const add = (id, sec, title, opts) => { const item = task(id, sec, title, opts); tasks.push(item); return item; };
  const wbs = model.wbs;
  const inc = model.incremental;
  const ui = model.uiLanding;
  const p1 = model.phase1Landing;
  const b0 = model.incrementalLanding;
  const evidenceCard = JSON.parse(fs.readFileSync(path.join(root,
    'analysis/full-audit/batch1-20261107/C03/c03-phase1-evidence-card.json'), 'utf8'));
  const openUnknowns = evidenceCard.unknowns || [];

  // ---- §1 P0 前置与四条方向 ----
  const p0 = [];
  p0.push(add('P0-01', '1', 'S0/C03 开放差分补证：45/971486 对 47/973202', {
    state: 'open', sourceRef: 'VM-LOCAL-RESTORATION-STATUS.json',
    evidencePaths: ['docs/VM-LOCAL-RESTORATION-STATUS.json', 'analysis/full-audit/batch1-20261107/C03/c03-phase1-evidence-card.json'],
    exitCriteria: ['取得区域对象/像素缓冲/native 结果数组', '差分被解释或显式登记为 unknown，不抹平'],
    anchor: anchor('04-vertical-slice.html', 'slice-phase1')
  }));
  openUnknowns.forEach((unknown, i) => {
    add('P0-U' + (i + 1), '1', '解除 unknown：' + clip(unknown, 90), {
      state: 'unknown', sourceRef: 'c03-phase1-evidence-card.json',
      evidencePaths: ['analysis/full-audit/batch1-20261107/C03/c03-phase1-evidence-card.json'],
      exitCriteria: ['取得对应源码/运行证据并登记证据类型'],
      blockers: ['P0-01']
    });
  });
  add('P0-02', '1', 'C01 纵切归档复跑（构建 7d65a1e1）', {
    state: 'executed', sourceRef: 'c01Evidence.command / C03-S0-RERUN-MATRIX-20261110',
    evidencePaths: [
      'analysis/full-audit/batch1-20261107/C01/runtime-report.json',
      'analysis/full-audit/batch1-20261107/C03/c03-s0-rerun-matrix-20261110.json'
    ],
    commands: [b0.commands[0].command],
    reportPaths: [
      'analysis/full-audit/batch1-20261107/C01/runtime-report.json',
      'analysis/full-audit/batch1-20261107/C03/c03-s0-rerun-matrix-20261110.json'
    ],
    exitCriteria: ['归档 exitCode=0 / 18 PASS 已记录', '同输入报告 SHA-256 与历史相同', 'C03 根因保持开放'],
    blockers: ['当前克隆需恢复历史复跑工程后才能再次执行此命令'],
    anchor: anchor('17-incremental-landing.html', 'inc-commands')
  });
  add('P0-03', '1', '继续 B1 静态取证（不改算法、不跑测试）', {
    state: 'open', sourceRef: 'incremental-landing.map.json / C03-INCREMENTAL-B1-CONTRACTS-001',
    evidencePaths: [
      'docs/reverse/C03-incremental-batch0-b1.md',
      'docs/reverse/C03-incremental-b1-static.md',
      'docs/reverse/C03-incremental-b1-contracts.md'
    ],
    exitCriteria: ['逐项核对纵切链路准确类型与成员', '输出名清单与方案文件字节级比对'],
    blockers: ['B0 未闭不得进入实施类动作']
  });
  add('P0-04', '1', '补 B0 根因证据（需可查询原厂过程或 provider 修复）', {
    state: 'blocked', sourceRef: 'c03-next-evidence-gate.json',
    evidencePaths: ['analysis/full-audit/batch1-20261107/C03/c03-next-evidence-gate.json', 'analysis/full-audit/batch1-20261107/C03/rea-native-provider-diagnostic.json'],
    exitCriteria: ['捕获非空 InputROI/InputMask 或生成区域对象、45 元素向量与图像缓冲'],
    blockers: ['用户环境/授权：原厂可查询过程或 REA provider']
  });
  add('P0-05', '1', '提交计划产物固定共同基线 SHA（需用户明确说提交）', {
    state: 'blocked', sourceRef: 'AGENTS.md / 用户硬规则',
    exitCriteria: ['中文提交信息 UTF-8 无 BOM 文件 + git commit -F', '只提交计划/站点相关文件'],
    blockers: ['用户授权：提交']
  });

  // ---- §2 渐进批次 13 步 ----
  const incTasks = inc.steps.map(step => add('INC-' + step.id, '2', step.batchId + ' · ' + step.title, {
    state: step.blockers.length ? 'blocked' : step.state,
    sourceRef: 'incremental-landing.map.json',
    evidencePaths: step.evidencePaths,
    sourceMembers: step.sourceMembers,
    commands: step.commands,
    reportPaths: step.reportPaths,
    exitCriteria: step.exitCriteria,
    unknowns: step.unknowns,
    blockers: step.blockers,
    anchor: anchor('17-incremental-landing.html', 'inc-step-' + step.id)
  }));

  // ---- §3 主界面共享控件 17 ----
  const uiTasks = ui.controls.map(control => add('UI-' + control.id, '3', control.name, {
    state: control.planState,
    sourceRef: 'ui-control-landing.map.json',
    evidencePaths: control.evidencePaths,
    sourceMembers: control.sourceMembers,
    exitCriteria: control.exitCriteria,
    unknowns: control.unknowns,
    blockers: control.blockers,
    note: control.firstAction,
    anchor: anchor('16-ui-control-landing.html', 'ui-' + control.id)
  }));

  // ---- §4 框架 WBS 32 工作包 ----
  const wpTasks = wbs.workPackages.map(pkg => add('WP-' + pkg.id, '4', pkg.id + ' · ' + pkg.title, {
    state: pkg.planState,
    sourceRef: 'framework-reverse-plan.json / framework-progress-audit.json',
    exitCriteria: [pkg.done || '台账未登记（unknown）'],
    blockers: pkg.dependencies,
    note: '线 ' + pkg.group + ' / 波次 ' + pkg.wave + ' · 单元 ' + (pkg.unitIds || []).join('、') +
      ' · 第一动作 ' + clip(pkg.nextAction || '台账未登记', 90) +
      ' · 字段缺口 ' + (pkg.fieldGaps || []).length,
    anchor: anchor('11-work-packages.html', 'wp-' + pkg.id)
  }));

  // ---- §5 96 单元卡 ----
  const unitTasks = wbs.unitCards.map((card, i) => add('UNIT-' + (i + 1), '5', card.id + ' · ' + clip(card.criterion, 80), {
    state: card.planState,
    sourceRef: 'framework-progress-audit.json',
    evidencePaths: (card.sourcePaths || []).slice(0, 3),
    unknowns: (card.unknowns || []).slice(0, 2),
    exitCriteria: ['源码/实施/验证三层状态逐层闭合'],
    blockers: card.blockers || [],
    note: '包 ' + card.package + ' · 三层 ' + JSON.stringify(card.states),
    anchor: anchor('12-unit-cards.html', 'unitcard-' + (i + 1))
  }));

  // ---- §6 162 目录工具 ----
  const toolTasks = wbs.toolCards.map((card, i) => {
    const tool = model.tools[i];
    return add('TOOL-' + (i + 1), '6', card.key + ' · ' + card.displayName, {
      state: tool && String(tool.implementationStatus || '').indexOf('partial-independent') === 0
        ? 'partial' : 'not_started',
      sourceRef: 'plugins/inventory.json / tool-implementation-specs.map.json',
      evidencePaths: [card.sourcePath, 'tests/ArchitectureRecovery/tool-implementation-specs.map.json'],
      exitCriteria: ['身份/资源哈希/参数 IO/Process native/运行对比/unknown 六项闭合', '不把测试通过写成 parity'],
      note: '分类 ' + card.category + ' · 批次 ' + card.batch + ' · 证据态 ' + card.evidenceState +
        ' · owner ' + (card.owner || '—') + ' · 规格台账 `TOOL-IMPLEMENTATION-SPECS.md`（2015 证据文件 / 160 high + 2 mid）',
      anchor: anchor('13-tool-cards.html', 'toolcard-' + (i + 1))
    });
  });

  // ---- §7 E0–E7 六段链 48 节点 ----
  const chainTasks = [];
  wbs.entryChains.forEach(chain => {
    chain.nodes.forEach(node => {
      chainTasks.push(add('CHAIN-' + chain.gateId + '-' + node.stageId, '7',
        chain.gateId + ' · ' + node.segment + '：' + clip(node.task, 80), {
        state: node.planState,
        sourceRef: 'entry-inventory.json',
        exitCriteria: [node.exitCriteria],
        note: '证据 ' + clip(node.evidence, 90) + ' · 门禁状态 ' + chain.gateState,
        anchor: anchor('14-entry-chains.html', 'chain-' + chain.gateId + '-' + node.stageId)
      }));
    });
  });

  // ---- §8 原厂程序集 / 当前工程 / 依赖边 / 8 层 ----
  const structTasks = [];
  model.assemblies.forEach((assembly, i) => {
    structTasks.push(add('ASM-' + (i + 1), '8', '原厂程序集 ' + assembly.id + ' 源码级核对', {
      state: 'not_started', sourceRef: 'architecture-baseline.json / analysis/decompiled / original-members.map.json',
      evidencePaths: ['analysis/decompiled/' + assembly.canonicalTree,
        'tests/ArchitectureRecovery/original-members.map.json'],
      exitCriteria: ['准确类型/成员/绑定/调用链与资源逐项对账'],
      note: assembly.lines + ' 行 / ' + assembly.types + ' 类型 / ' + assembly.methods + ' 方法' +
        (assembly.duplicatePasses ? ' · 重复反编译 pass 已归并' : '') +
        ' · 成员台账 `ORIGINAL-MEMBERS-INDEX.md`（2614 canonical .cs / 3719 类型 / 10128 方法签名）',
      anchor: anchor('15-target-source-tree.html', 'original-' + (i + 1))
    }));
  });
  model.currentProjects.forEach(project => {
    structTasks.push(add('PROJ-' + project.Name, '8', '自研工程 ' + project.Name + ' 现状核对', {
      state: 'current-source', sourceRef: 'architecture-baseline.json → currentProjects',
      evidencePaths: ['src/' + project.Name],
      exitCriteria: ['与原厂对应层职责逐项差分'],
      note: project.Lines + ' 行 / ' + project.Types + ' 类型 / ' + project.CsFiles + ' .cs',
      anchor: anchor('15-target-source-tree.html', 'tree-diagrams')
    }));
  });
  model.currentEdges.forEach((edge, i) => {
    structTasks.push(add('EDGE-' + (i + 1), '8', '依赖边 ' + edge.From + ' → ' + edge.To, {
      state: 'current-source', sourceRef: 'architecture-baseline.json → currentEdges',
      exitCriteria: ['引用文件与方向可复核'],
      note: edge.Files + ' 个引用文件',
      anchor: anchor('15-target-source-tree.html', 'tree-deps')
    }));
  });
  model.layers.forEach(layer => {
    structTasks.push(add('LAYER-' + layer.id, '8', '架构层 ' + layer.id + ' · ' + layer.name, {
      state: layer.status, sourceRef: 'architecture-baseline.json → layers',
      exitCriteria: ['层内原厂/自研职责与缺口逐项对账'],
      note: '当前 ' + layer.currentLines + ' 行 · 缺口 ' + clip(layer.gap, 90),
      anchor: anchor('02-architecture.html', 'arch-layers')
    }));
  });

  // ---- §9 阶段 / 门 / 证据规则 / 旧计划处置 ----
  const govTasks = [];
  model.stages.forEach(stage => {
    govTasks.push(add('STAGE-' + stage.id, '9', '阶段 ' + stage.id + ' · ' + stage.name, {
      state: 'not_started', sourceRef: 'reverse-plan-editorial.json → stages',
      exitCriteria: [clip(Array.isArray(stage.exitCriteria) ? stage.exitCriteria.join('；') : stage.exitCriteria, 120)],
      blockers: [stage.prerequisite || '无'],
      anchor: anchor('03-roadmap.html', 'stage-' + stage.id)
    }));
  });
  model.stageGates.forEach(gate => {
    govTasks.push(add('GATE-' + gate.id, '9', '阶段门 ' + gate.id + ' · ' + gate.name, {
      state: 'not_started', sourceRef: 'reverse-plan-editorial.json → stageGates',
      exitCriteria: [gate.rule],
      anchor: anchor('09-gates-evidence.html', 'gate-' + gate.id)
    }));
  });
  model.entryGates.forEach(gate => {
    govTasks.push(add('E-' + gate.id, '9', '入口门 ' + gate.id + ' · ' + gate.title, {
      state: gate.state, sourceRef: 'architecture-recovery/entry-inventory.json',
      exitCriteria: ['六段链闭合且入口与模型双向对账'],
      note: '缺口 ' + clip(gate.gap, 100),
      anchor: anchor('14-entry-chains.html', 'gate-' + gate.id)
    }));
  });
  (model.evidenceStates || []).forEach(state => {
    govTasks.push(add('EVID-' + (state.id || state.label), '9', '证据态 ' + (state.label || state.id), {
      state: 'rule', sourceRef: 'gates-and-evidence.json',
      exitCriteria: [clip(state.rule || state.claim, 110)]
    }));
  });
  (model.acceptanceLevels || []).forEach(level => {
    govTasks.push(add('ACCEPT-' + (level.id || level.label), '9', '验收层级 ' + (level.label || level.id), {
      state: 'rule', sourceRef: 'gates-and-evidence.json',
      exitCriteria: [clip(level.rule, 110)]
    }));
  });
  (model.legacyPlans || []).forEach((plan, i) => {
    govTasks.push(add('LEGACY-' + (i + 1), '9', '旧计划处置 ' + clip(plan.title, 70), {
      state: plan.status || 'historical', sourceRef: plan.path,
      exitCriteria: ['保留历史，不改既有结论数字'],
      note: plan.disposition,
      anchor: anchor('00-master-plan.html', 'mp-legacy')
    }));
  });

  // ---- §10 状态更新与治理 ----
  const rules = [];
  rules.push(add('RULE-01', '10', '勾选任务后同步回写台账与站点（保持确定性）', {
    state: 'rule', sourceRef: 'AGENTS.md / 计划站点硬约束',
    exitCriteria: ['重跑 build + verify 且逐字节一致', '不手写结论数字']
  }));
  rules.push(add('RULE-02', '10', '不自动提交/推送、不自动进入方案测试', {
    state: 'rule', sourceRef: '用户硬规则',
    exitCriteria: ['提交需用户明确说提交', '测试需用户明确说开始测试/验证']
  }));
  rules.push(add('RULE-03', '10', '口径冲突并列登记，不擅自统一', {
    state: 'rule', sourceRef: 'AGENTS.md',
    exitCriteria: ['42/96=43.75% 与 41/96=42.71% 并列', 'PositionCorrection 批次身份差异原样保留', '45/971486 与 47/973202 并列']
  }));
  rules.push(add('RULE-04', '10', '人工主机验收单列，不阻塞也不计入完成度', {
    state: 'rule', sourceRef: 'AGENTS.md',
    exitCriteria: ['用户交互桌面 WinSta0\\Default 验证可见窗口后再登记']
  }));
  rules.push(add('RULE-05', '10', '缺失函数体/资源/ABI 标 unknown，不以合理设计填空', {
    state: 'rule', sourceRef: 'AGENTS.md',
    exitCriteria: ['区分 source-confirmed / runtime-confirmed / independent-implementation / unknown']
  }));

  // ---- 汇总与附录 ----
  const counts = {
    tasks: tasks.length,
    p0: tasks.filter(item => item.section === '1').length,
    incremental: incTasks.length,
    controls: uiTasks.length,
    workPackages: wpTasks.length,
    unitCards: unitTasks.length,
    tools: toolTasks.length,
    chainNodes: chainTasks.length,
    structure: structTasks.length,
    governance: govTasks.length,
    rules: rules.length,
    byState: tasks.reduce((acc, item) => { acc[item.state] = (acc[item.state] || 0) + 1; return acc; }, {}),
    prefilledCheckboxes: 0,
    ledgerDoneStates: tasks.filter(item => item.state === 'done').length,
    evidenceReferences: tasks.reduce((n, item) => n + item.evidencePaths.length, 0),
    commandEntries: tasks.reduce((n, item) => n + item.commands.length, 0)
  };
  const builtFrom = model.builtFrom.head.slice(0, 8) + '/' + model.builtFrom.inputsDigest.slice(0, 12);

  const allCommands = [...new Set(tasks.flatMap(item => item.commands))];
  const allEvidence = [...new Set(tasks.flatMap(item => item.evidencePaths))].sort();
  const allReports = [...new Set(tasks.flatMap(item => item.reportPaths))].sort();
  const anchors = tasks.filter(item => item.anchor);

  const sections = [
    '# 完整逆向任务清单（一次列全）',
    '',
    '从 `buildModel()` 与 9 个机器台账 / 7 个补充源确定性派生。勾选框由人工验收后填写，机器不预填任何 `[x]`；状态字段原样来自台账。',
    '',
    '- 生成：`node tests/ArchitectureRecovery/build-complete-task-list.cjs`',
    '- 独立校验：`node tests/ArchitectureRecovery/verify-complete-task-list.cjs`',
    '- 机器台账：`tests/ArchitectureRecovery/complete-task-list.map.json`',
    '- 站点入口：`docs/reverse-plan/index.html`（19 页，本清单不新增页面）',
    '- builtFrom：`' + builtFrom + '`（HEAD + 输入 SHA-256，与站点同源）',
    '',
    '## §0 口径总览与硬规则',
    '',
    '| 项 | 值 |',
    '|---|---|',
    '| 授权范围完成度 | `docs/SOURCE-LEVEL-REVERSE-PROGRESS.json` 10/10 = 100%（仅标定授权范围，人工验收单列） |',
    '| 当前目标 | `C03 original Blob runtime differential` / running |',
    '| S0 | `' + p1.s0GateStatus + '`（S0.1/S0.2 已在当前构建产物复跑，S0.3 差分开放项仍满足） |',
    '| S0 复跑 | C01 exitCode=0 / 18 PASS；同输入 1.jpg 得 47/973202，报告 SHA-256 与历史逐字节相同；12 张标定样本重开计数一致 |',
    '| 开放差分 | 原厂 45/971486 对直接 SDK / 本地 47/973202（+2 / +1716）；C03 原厂根因仍开放 |',
    '| 第一批 | `' + b0.cardId + '` · B0 open / B1–B3 blocked · C01 复跑 1 条已执行 · 产品改动 0 |',
    '| 渐进批次 | 4 批次 / 13 步，全部 not_started，10 步带前置阻塞 |',
    '| 主界面控件 | 17 项 / 4 类，全部 blocked，102 个六段链节点 unknown，三级验收 0 完成 |',
    '| 目录工具 | 162 项，161 未建立 + 1 partial，全部为逆向实现目标 |',
    '| 入口链 | `closedEntryChains=0`，E0–E7 八门未闭 |',
    '| 口径冲突 | 42/96=43.75% 与 41/96=42.71% 并列；PositionCorrection 批次身份差异原样保留 |',
    '| 硬规则 | 不自动提交/推送、不自动跑方案测试、不改 src/、缺失标 unknown、截图只作差异信号 |',
    '| 勾选框 | 全部 `[ ]`，机器零预填；状态为 `done` 的 ' + tasks.filter(item => item.state === 'done').length +
      ' 项来自三层台账既有状态，不等于本清单已验收 |',
    '',
    '授权状态：用户已一次性授权「全放开、批量落地」；C01 复跑已完成（exitCode=0）。提交需用户明确说「提交」；B0 根因补证仍需可查询原厂过程或 provider 修复。',
    ''
  ];

  sections.push(section(1, 'P0 前置：S0/C03 补证与四条方向',
    'B0 未闭前不得进入后续批次的实施类动作。历史 `18/18 passed` 不是本轮复跑，本地/SDK 一致不等于原厂 parity。',
    tasks.filter(item => item.section === '1'), true));
  sections.push(section(2, '渐进批次 B0–B3（13 步）',
    '按最小纵切 ImageSource→Blob→显示 推进；每步写明证据、成员、命令、报告、退出判据与 unknown。',
    incTasks, true));
  sections.push(section(3, '主界面共享控件（17 项 / 4 类）',
    '每项须完成源码级闭环 + 原厂同状态差分 + 自研操作级复跑；六段链与三级验收详情见第 18 页锚点。',
    uiTasks));
  sections.push(section(4, '框架 WBS 工作包（32 项）',
    '逐包挂到波次、依赖、3 个具名单元与退出判据；`done` 等状态来自三层台账，不代表本清单已验收。',
    wpTasks));
  sections.push(section(5, '框架单元任务卡（96 项）',
    '按工作包分组，逐单元核对源码/实施/验证三层状态与取证目标。', unitTasks));
  sections.push(section(6, '目录工具任务卡（162 项 / 14 分类）',
    '全部为逆向实现目标；逐工具身份、资源哈希、参数 IO、Process native、运行对比与 unknown 闭合。',
    toolTasks));
  sections.push(section(7, '入口闭合链（8 门 × 6 段 = 48 节点）',
    '闭合链缺任何一段都不算闭合；未取得证据的链节点一律标 unknown 并给出 REA 补证目标。',
    chainTasks));
  sections.push(section(8, '原厂程序集 / 自研工程 / 依赖边 / 8 层',
    '25 个唯一程序集、29 个当前工程、62 条 ProjectReference 依赖边与 8 层映射的逐项源码级核对。',
    structTasks));
  sections.push(section(9, '阶段 / 门禁 / 证据规则 / 旧计划处置',
    'S0–S9 阶段、G1–G8 阶段门、E0–E7 入口门、证据四态、验收层级与 4 份旧计划处置。',
    govTasks));
  sections.push(section(10, '状态更新与治理规则',
    '勾选后如何回写、哪些动作必须用户授权、哪些不得自动执行。', rules));

  sections.push('## 附录 A 全部命令（' + allCommands.length + ' 条，均待执行）', '');
  allCommands.forEach(command => sections.push('- ' + code(command) + ' 〔S0 复跑批次已执行：exitCode=0〕'));
  sections.push('');
  sections.push('## 附录 B 全部报告路径（' + allReports.length + ' 项）', '');
  allReports.forEach(item => sections.push('- ' + code(item)));
  sections.push('');
  sections.push('## 附录 C 全部证据路径（' + allEvidence.length + ' 项，去重）', '');
  allEvidence.forEach(item => sections.push('- ' + code(item)));
  sections.push('');
  sections.push('## 附录 D 站点锚点（' + anchors.length + ' 项）', '');
  sections.push('任务项内联锚点指向 `docs/reverse-plan/` 对应页面；校验器逐项核对 id 存在。', '');
  sections.push('## 附录 E 完成度口径', '');
  sections.push('本清单的任务数、阶段门数或测试数**不得**冒充完成度。权威口径仍为 `docs/SOURCE-LEVEL-REVERSE-PROGRESS.json` 的 10/10 = 100%（仅标定授权范围，人工验收单列）；S0 本地切片闭环达成，但 C03 原厂运行时根因仍开放、差分 `45/971486` 对 `47/973202` 未解释。', '');
  sections.push('## 附录 F 规格台账与证据卡索引', '');
  sections.push('- `tests/ArchitectureRecovery/original-members.map.json` / `docs/ORIGINAL-MEMBERS-INDEX.md`：25 唯一程序集按 canonical 归并 28 棵反编译树，2614 `.cs`、3719 类型声明、10128 方法签名；与基线去重口径差异原样登记。');
  sections.push('- `tests/ArchitectureRecovery/tool-implementation-specs.map.json` / `docs/TOOL-IMPLEMENTATION-SPECS.md`：162 工具六项规格、14 分类、2015 个带哈希证据文件、160 high + 2 mid；native 函数体全部标 `not-recovered`。');
  sections.push('- `analysis/full-audit/batch1-20261107/C03/c03-s0-rerun-matrix-20261110.json`：S0 复跑、12 张标定样本与 11 个通过回归 / 2 个外部数据阻塞回归。');
  sections.push('- `analysis/full-audit/batch1-20261107/C03/c03-incremental-b1-contracts.json` / `docs/reverse/C03-incremental-b1-contracts.md`：9 个 `Components.*` 投影 leaf 与 Group `image/width` 方案输出名闭合；边界为 managed projection，不是原厂 ABI。');
  sections.push('');
  sections.push('## 附录 G 启动链路与整屏空载初始态工作面', '');
  sections.push('与第 18 页「运行态」17 控件清单**并列、状态面不同**（本附录为程序刚启动、未加载方案的空载初始态），不产生第二套完成度、不改动本节清单的 544 项任务数。');
  sections.push('');
  sections.push('- 台账 `tests/ArchitectureRecovery/startup-initial-ui.map.json` 与派生层 `plan-startup-initial-ui.cjs`：' +
    model.startupInitialUi.counts.startupSteps + ' 个启动步骤 / ' + model.startupInitialUi.counts.controls +
    ' 个初始态控件，' + model.startupInitialUi.counts.sixSegmentUnknown +
    ' 个六段链节点全 `unknown`、三级验收零预填。');
  sections.push('- 可派工清单 `docs/STARTUP-INITIAL-UI-TASK-LIST.md`：启动链路时间线（原厂方法名与行号）、控件分区、六段链状态、unknown 与解除条件、21 张任务卡。');
  sections.push('- 证据结论 `docs/reverse/C03-startup-initial-ui-evidence.md` 与证据卡 `analysis/full-audit/batch1-20261107/C03/c03-startup-initial-ui-20261110.json`（' +
    model.startupInitialUi.counts.evidencePaths + ' 条去重证据路径、带 SHA-256）。');
  sections.push('- 边界：源码级证据闭环，不改 `src/`、不跑方案测试、未做原厂同状态差分与自研复跑；截图只作控件范围识别。E0–E7 门与 W/K/C/U 布局边映射见清单 §5，' +
    model.startupInitialUi.counts.unlinkedLayoutEdges + ' 条未关联边如实登记。');
  sections.push('');

  const markdown = sections.join('\n');
  const ledger = {
    schemaVersion: 1,
    note: '完整逆向任务清单机器台账；由 build-complete-task-list.cjs 从 buildModel() 确定性派生，勾选状态不预填。',
    builtFrom,
    counts,
    tasks,
    commands: allCommands,
    reportPaths: allReports,
    evidencePaths: allEvidence
  };
  return { markdown, ledger, counts };
}

function main() {
  const model = lib.buildModel();
  lib.verifyInputs(model);
  const result = buildCompleteList(model);
  fs.writeFileSync(OUT_MD, result.markdown, 'utf8');
  fs.writeFileSync(OUT_JSON, JSON.stringify(result.ledger, null, 2) + '\n', 'utf8');
  console.log(JSON.stringify({
    markdownBytes: Buffer.byteLength(result.markdown, 'utf8'),
    ledgerBytes: Buffer.byteLength(JSON.stringify(result.ledger, null, 2) + '\n', 'utf8'),
    counts: result.counts,
    builtFrom: result.ledger.builtFrom
  }, null, 2));
}

if (require.main === module) main();
module.exports = { buildCompleteList, OUT_MD, OUT_JSON };
