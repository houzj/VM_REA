'use strict';
// 生成 docs/reverse-plan 离线站点。所有数字取自 plan-model.cjs 装配的机器台账；本文件只做页面组织与断言。
const fs = require('fs');
const path = require('path');
const crypto = require('crypto');
const vm = require('vm');
const lib = require('./plan-model.cjs');
const V = require('./plan-view.cjs');
const D = require('./plan-diagrams.cjs');
const FT = require('./plan-file-tree.cjs');
const TL = require('./plan-tool-layer.cjs');
const OT = require('./plan-original-tree.cjs');

const { root, out, buildModel, verifyInputs, must } = lib;
const ASSET_SOURCES = [
  { from: 'tests/ArchitectureRecovery/plan-page.css', to: 'assets/plan.css' },
  { from: 'tests/ArchitectureRecovery/plan-page.js', to: 'assets/plan.js' }
];
const PAGE_LIMIT = 400 * 1024;
const SITE_LIMIT = 2 * 1024 * 1024;
const PRIORITY_PAGES = {
  1: '05-p0-mechanism.html', 2: '05-p0-mechanism.html', 3: '05-p0-mechanism.html',
  4: '06-p1-coverage.html', 5: '06-p1-coverage.html',
  6: '07-p2-p3.html', 7: '07-p2-p3.html', 8: '07-p2-p3.html', 9: '07-p2-p3.html'
};

const E = V.esc;
const F = V.fmt;

function readSource(relative) {
  return fs.readFileSync(path.join(root, relative), 'utf8').replace(/^\uFEFF/, '');
}
function sha(text) { return crypto.createHash('sha256').update(text).digest('hex'); }
function uniq(items) { return [...new Set(items)]; }
function idxId(prefix, list, item) {
  const i = list.indexOf(item);
  must(i >= 0, '锚点对象不在台账中: ' + prefix);
  return prefix + '-' + (i + 1);
}
function meta(model, slug) {
  const found = model.pages.find(p => p.slug === slug);
  must(found, '页面清单缺少: ' + slug);
  return found;
}
function joinCodes(items, limit) {
  if (!items.length) return '<span class="path">台账未登记</span>';
  const shown = limit ? items.slice(0, limit) : items;
  const rest = items.length - shown.length;
  return V.ulRaw(shown.map(item => V.pathHtml(item))) + (rest ? '<p class="path">其余 ' + E(rest) + ' 项见 ' + V.code('plan-data.json') + ' 指向的台账。</p>' : '');
}
function definitionTable(rows) {
  return V.table({
    caption: '任务定义（每项均来自机器台账或计划编辑源）',
    columns: [
      { label: '字段', cell: row => '<b>' + E(row.k) + '</b>' },
      { label: '内容', cell: row => row.v }
    ],
    rows
  });
}
function fileOf(slug) { return slug === 'index' ? 'index.html' : slug + '.html'; }
function clip(value, max) {
  const text = String(value === null || value === undefined ? '' : value);
  return text.length > max ? text.slice(0, max) + '…' : text;
}
function planStateLabel(model, state) {
  const found = model.taskStateMachine.states.find(item => item.id === state);
  return found ? found.label : state;
}
function unitCardId(wbs, card) { return idxId('unitcard', wbs.unitCards, card); }
function toolCardId(wbs, card) { return idxId('toolcard', wbs.toolCards, card); }
function chainNodeId(gateId, stageId) { return 'chain-' + gateId + '-' + stageId; }
function cardAttrs(options) {
  return [
    'data-task-name="' + E(options.name) + '"',
    'data-task-state="' + E(options.state) + '"',
    options.category ? 'data-task-category="' + E(options.category) + '"' : '',
    options.batch ? 'data-task-batch="' + E(options.batch) + '"' : '',
    options.package ? 'data-task-package="' + E(options.package) + '"' : '',
    options.owner ? 'data-task-owner="' + E(options.owner) + '"' : ''
  ].filter(Boolean).join(' ');
}

// ---------------------------------------------------------------- 页面 1 · 索引
function renderIndex(m) {
  const p = V.createPage(m, meta(m, 'index'));
  const fw = m.progress.framework, pl = m.progress.plugins, cl = m.progress.closure;
  p.intro(V.cards([
    { num: m.metrics.original.assemblies, label: '原厂唯一程序集', hint: m.metrics.original.trees + ' 个反编译树去重', tone: 'amber' },
    { num: F(m.metrics.original.lines), label: '原厂源码行', hint: F(m.metrics.original.types) + ' 类型 / ' + F(m.metrics.original.methods) + ' 方法', tone: 'amber' },
    { num: F(m.metrics.current.lines), label: '当前源码行', hint: m.metrics.current.projects + ' 工程 / ' + F(m.metrics.current.types) + ' 类型' },
    { num: pl.catalog.total, label: '原厂目录工具', hint: pl.catalog.categories + ' 个分类，全量纳入计划' },
    { num: fw.current.total, label: '框架具名验收单元', hint: fw.packages + ' 包 × 3 单元' },
    { num: m.stages.length, label: '执行阶段', hint: 'S0 切片 → S9 铺开', tone: 'done' }
  ]));

  const progressRows = [
    { name: '框架 · 逐项台账', value: fw.current.done + '/' + fw.current.total + ' = ' + V.pct(fw.current.percent), source: fw.current.source, boundary: 'implementation 层；三层独立等权计数' },
    { name: '框架 · 历史汇总', value: fw.legacy.done + '/' + fw.legacy.total + ' = ' + V.pct(fw.legacy.percent), source: fw.legacy.source, boundary: '与上一行差异只在汇总字段，96 个单元状态未变' },
    { name: '插件 · 标定十工具', value: pl.calibration.done + '/' + pl.calibration.total + ' = ' + V.pct(pl.calibration.percent), source: pl.calibration.source, boundary: '整工具源码级重建 + 自审；人工验收 ' + E(pl.calibration.manualAcceptance), },
    { name: '插件 · 下一批', value: pl.nextBatch.done + '/' + pl.nextBatch.total + ' = ' + V.pct(pl.nextBatch.percent), source: pl.nextBatch.source, boundary: '已完成 ' + E(pl.nextBatch.completedTools.join('、')) },
    { name: '行为闭环 · A01 有界', value: cl.closedStages + '/' + cl.totalStages + ' = ' + V.pct(cl.completionPercent), source: cl.source, boundary: E(cl.scope) + '；当前目标 ' + E(cl.objective) + '（' + E(cl.status) + '）' }
  ];
  const executionSlugs = ['00-master-plan', '11-work-packages', '12-unit-cards', '13-tool-cards', '14-entry-chains', '15-target-source-tree', '16-ui-control-landing', '17-incremental-landing'];
  const executionPages = m.pages.filter(item => executionSlugs.includes(item.slug));
  p.sec('master', '00', '权威总计划与执行视图',
    V.notice(m.pageNotes.masterAuthority, 'cyan') +
    V.table({
      caption: '完整逆向计划新增执行视图；既有页面 URL 与锚点保持不变',
      columns: [
        { label: '页面', cell: r => '<a href="' + V.hrefFor(r.slug) + '">' + E(r.title) + '</a>' },
        { label: '层级', cell: r => V.chip('idle', r.tier) },
        { label: '内容', cell: r => E(r.description) }
      ], rows: executionPages
    }) +
    V.cards([
      { num: m.wbs.counts.workPackages, label: 'WBS 工作包', hint: '可派工一级单元', tone: 'amber' },
      { num: m.wbs.counts.unitCards, label: '框架单元任务卡', hint: '逐单元取证与退出判据' },
      { num: m.wbs.counts.toolCards, label: '目录工具任务卡', hint: '全量目录工具' },
      { num: m.wbs.counts.chainNodes, label: '闭合链节点', hint: 'E0–E7 × 六段' }
    ]));

  p.sec('progress-3', '01', '三套进度口径（并列，不合并）',
    V.notice(m.pageNotes.threeProgress, 'cyan') +
    V.table({
      caption: '框架 / 插件 / 行为闭环三套口径',
      columns: [
        { label: '口径', cell: r => '<b>' + E(r.name) + '</b>' },
        { label: '数值', sort: 'text', cell: r => V.mono(r.value) },
        { label: '来源台账', cell: r => V.pathHtml(r.source) },
        { label: '边界说明', cell: r => E(r.boundary) }
      ],
      rows: progressRows
    }));

  p.sec('conflict', '02', '口径冲突登记：41/96 与 42/96',
    V.notice(m.conflict) +
    V.table({
      caption: '两个汇总字段并列展示，不擅自二选一',
      columns: [
        { label: '来源', cell: r => E(r.name) },
        { label: '分子 / 分母', sort: 'text', cell: r => V.mono(r.value) },
        { label: '口径位置', cell: r => V.pathHtml(r.source) }
      ],
      rows: [
        { name: '逐项重算（台账）', value: fw.current.done + '/' + fw.current.total + ' = ' + V.pct(fw.current.percent), source: INPUT(m, 'progressAudit') + ' → summary.layers.implementation' },
        { name: '历史汇总（交接）', value: fw.legacy.done + '/' + fw.legacy.total + ' = ' + V.pct(fw.legacy.percent), source: INPUT(m, 'reversePlan') + ' → activeBatch.progress / AGENTS.md' }
      ]
    }) +
    V.evidence('summaryReconciliation', '<p>' + E(fw.current.reconciliation) + '</p>') +
    '<p>处置规则与责任边界见 <a href="' + V.hrefFor('10-governance', 'conflict') + '">变更治理与口径冲突</a>。</p>');

  const s0 = m.stages[0];
  p.sec('current', '03', '当前阶段：S0 纵向切片优先',
    V.notice(m.pageNotes.sliceFirst, 'cyan') +
    definitionTable([
      { k: '前置', v: E(s0.prerequisite) },
      { k: '产出', v: E(s0.deliverable) },
      { k: '退出条件', v: V.ol(s0.exitCriteria.map(E)) },
      { k: '禁止并行', v: V.ul(s0.forbiddenParallel.map(E)) },
      { k: '当前目标', v: E(cl.objective) + ' · ' + E(cl.status) + ' · ' + V.pathHtml(cl.source) }
    ]) +
    '<p>链路与差分见 <a href="' + V.hrefFor('04-vertical-slice') + '">纵向切片定义</a>；S0–S9 全表见 <a href="' + V.hrefFor('03-roadmap') + '">阶段路线图</a>。</p>');

  p.sec('nav', '04', m.pages.length + ' 页导航',
    V.table({
      caption: '多页计划站点目录',
      columns: [
        { label: '#', sort: 'num', cls: 'num-cell', cell: (r, i) => E(String(i + 1).padStart(2, '0')) },
        { label: '页面', cell: r => '<a href="' + V.hrefFor(r.slug) + '">' + E(r.title) + '</a>' },
        { label: '层级', cell: r => V.chip('idle', r.tier) },
        { label: '内容', cell: r => E(r.description) }
      ],
      rows: m.pages
    }));

  p.sec('gaps', '05', '8 项遗漏面（先看缺口，再排优先级）',
    V.table({
      caption: '基线第 7 节登记的遗漏面；完整口径见 01 页',
      columns: [
        { label: '#', sort: 'num', cls: 'num-cell', cell: (r, i) => E(String(i + 1).padStart(2, '0')) },
        { label: '遗漏面', cell: r => '<b>' + E(r.item) + '</b>' },
        { label: '规模', cell: r => V.mono(r.size) },
        { label: '状态', cell: r => E(r.status) },
        { label: '风险', cell: r => E(r.risk) }
      ],
      rows: m.gaps,
      rowId: (r, i) => idxId('gap', m.gaps, r)
    }) + '<p><a href="' + V.hrefFor('01-scope', 'scope-gaps') + '">查看完整遗漏面统计口径 →</a></p>');

  const commands = m.scope.regenerate.split(';').map(s => s.trim()).filter(Boolean);
  p.sec('recompute', '06', '复算与产物标识',
    V.listPairs([
      ['基线复算', V.ulRaw(commands.map(c => V.code(c)))],
      ['站点重跑', V.code('node tests/ArchitectureRecovery/build-reverse-plan.cjs')],
      ['独立校验', V.code('node tests/ArchitectureRecovery/verify-reverse-plan.cjs')],
      ['机器数据', '<a href="plan-data.json">plan-data.json</a>'],
      ['输入台账', E(m.inputs.length) + ' 个机器台账 + ' + E(m.supporting.length) + ' 个补充源，逐项 SHA-256 见页脚'],
      ['产物标识', V.code(m.builtFrom.head.slice(0, 8) + ' / ' + m.builtFrom.inputsDigest.slice(0, 12))],
      ['审计日期', V.mono(m.scope.auditDate)]
    ]) +
    V.notice(m.pageNotes.determinism, 'cyan'));
  return p.finish();
}
function INPUT(m, key) { return m.sourcePaths[key]; }

// ---------------------------------------------------------------- 页面 2 · 口径与源码量
function renderScope(m) {
  const p = V.createPage(m, meta(m, '01-scope'));
  p.intro(V.cards([
    { num: m.metrics.original.assemblies, label: '原厂唯一程序集', hint: m.metrics.original.trees + ' 反编译树 / 原始 ' + F(m.metrics.original.rawCs) + ' .cs', tone: 'amber' },
    { num: F(m.metrics.original.lines), label: '去重后原厂行', hint: F(m.metrics.original.types) + ' 类型 / ' + F(m.metrics.original.methods) + ' 方法', tone: 'amber' },
    { num: m.metrics.current.projects, label: '当前工程', hint: F(m.metrics.current.cs) + ' .cs / ' + F(m.metrics.current.xaml) + ' .xaml' },
    { num: F(m.metrics.current.lines), label: '当前源码行', hint: F(m.metrics.current.types) + ' 类型 / ' + F(m.metrics.current.methods) + ' 方法' },
    { num: m.metrics.lineRatio + ' : 1', label: '行数比', hint: '不能按代码量估算完成度' },
    { num: m.gaps.length, label: '遗漏面', hint: '台账未覆盖，逐项登记', tone: 'unknown' }
  ]));

  const methodRows = Object.entries(m.scope.method).map(([key, value]) => ({
    key, value: Array.isArray(value) ? value.join('；') : String(value)
  }));
  p.sec('scope-method', '01', '统计方法与排除规则',
    V.table({
      caption: '基线 method 字段逐项展示；排除规则与归并策略不得在页面里改写',
      columns: [
        { label: '项', cell: r => V.code(r.key) },
        { label: '定义', cell: r => E(r.value) }
      ],
      rows: methodRows
    }) +
    V.notice(m.pageNotes.roslynOnly, 'cyan'));

  const o = m.metrics.original, c = m.metrics.current;
  const totalRows = [
    { name: '唯一单元', a: o.assemblies + ' 程序集（' + o.trees + ' 树）', b: c.projects + ' 工程' },
    { name: '.cs', a: F(o.cs), b: F(c.cs) },
    { name: '.xaml', a: F(o.xaml), b: F(c.xaml) },
    { name: '行', a: F(o.lines), b: F(c.lines) },
    { name: '类型', a: F(o.types), b: F(c.types) },
    { name: '方法', a: F(o.methods), b: F(c.methods) },
    { name: '依赖边', a: F(o.dependencies), b: F(c.dependencies) },
    { name: '原始（未去重）', a: F(o.rawCs) + ' .cs / ' + F(o.rawLines) + ' 行', b: '—' }
  ];
  p.sec('scope-totals', '02', '两栏总量',
    V.table({
      caption: '原厂 / 当前按同一套排除规则计数',
      columns: [
        { label: '指标', cell: r => '<b>' + E(r.name) + '</b>' },
        { label: '原厂', sort: 'text', cell: r => V.mono(r.a) },
        { label: '当前', sort: 'text', cell: r => V.mono(r.b) }
      ],
      rows: totalRows
    }) +
    V.notice('重复反编译归并：' + E(m.scope.method.duplicatePasses) + '。只取较大一次，不累加；因此「原始」行不能与去重后总量相加。'));

  p.sec('scope-layers', '03', '8 层规模占比',
    V.table({
      caption: '8 层职责 / 原厂规模 / 当前覆盖，占比由台账重算',
      columns: [
        { label: '层', cell: r => V.code(r.id) },
        { label: '职责', cell: r => E(r.name) },
        { label: '原厂行', sort: 'num', cls: 'num-cell', cell: r => E(F(r.original.lines)) },
        { label: '占比', sort: 'num', cls: 'num-cell', cell: r => E(V.pct(r.sharePercent)) },
        { label: '当前行', sort: 'num', cls: 'num-cell', cell: r => E(F(r.currentLines)) },
        { label: '行覆盖', sort: 'num', cls: 'num-cell', cell: r => E(V.pct(r.lineCoveragePercent)) },
        { label: '状态', cell: r => V.statusChip(r.status, r.status) },
        { label: '单元', cell: r => V.mono(r.units) },
        { label: '缺口', cell: r => E(r.gap) }
      ],
      rows: m.layers
    }) +
    V.notice(m.pageNotes.layerProxy, 'cyan'));

  p.sec('scope-gaps', '04', '8 项遗漏面',
    V.table({
      caption: '基线第 7 节：台账未覆盖，必须显式登记而不是留白',
      columns: [
        { label: '#', sort: 'num', cls: 'num-cell', cell: (r, i) => E(String(i + 1).padStart(2, '0')) },
        { label: '遗漏面', cell: r => '<b>' + E(r.item) + '</b>' },
        { label: '规模', cell: r => V.mono(r.size) },
        { label: '状态', cell: r => E(r.status) },
        { label: '风险', cell: r => E(r.risk) }
      ],
      rows: m.gaps,
      rowId: r => idxId('gap', m.gaps, r)
    }));

  const commands = m.scope.regenerate.split(';').map(s => s.trim()).filter(Boolean);
  p.sec('scope-recompute', '05', '复算与口径冻结',
    V.listPairs([
      ['原始计数', V.pathHtml(m.scope.sourceInventory)],
      ['映射判断', V.pathHtml(m.scope.mappingTable)],
      ['机器账本', V.pathHtml(INPUT(m, 'baseline'))],
      ['人读基线', V.pathHtml(INPUT(m, 'baselineDoc'))],
      ['审计日期', V.mono(m.scope.auditDate)],
      ['复算命令', V.ulRaw(commands.map(c => V.code(c)))]
    ]) +
    V.notice(m.pageNotes.denominatorChange));
  return p.finish();
}

// ---------------------------------------------------------------- 页面 3 · 架构对照
function renderArchitecture(m) {
  const p = V.createPage(m, meta(m, '02-architecture'));
  p.intro(V.cards([
    { num: m.assemblies.length, label: '原厂唯一程序集', hint: '按 canonical pass 归并', tone: 'amber' },
    { num: m.layers.length, label: '架构层', hint: '程序集归并覆盖 25 个唯一程序集' },
    { num: m.currentProjects.length, label: '当前工程', hint: '含 .csproj 的 src 工程' },
    { num: F(m.metrics.original.dependencies), label: '原厂依赖边', hint: 'using 指令映射（耦合代理）', tone: 'amber' },
    { num: F(m.metrics.current.dependencies), label: '当前依赖边', hint: 'ProjectReference' },
    { num: m.architectureVerdict.differences.length, label: '机制不一致项', hint: '不等价，不声称 parity', tone: 'unknown' }
  ]));

  const diagrams = m.diagrams;
  p.sec('arch-diagram', '00', '架构分层图（一目了然）',
    '<p class="lede">原厂 25 个唯一程序集与自研 29 个工程按 L1–L8 并列；节点内数字与规模色阶来自机器台账，下方泳道按 ProjectReference 聚合画层间依赖箭头。</p>' +
    diagrams.architecture.html + D.diagramLegend(m, diagrams) +
    V.details({
      title: '层间依赖聚合（' + diagrams.architecture.pairs.length + ' 对 / ' +
        diagrams.architecture.layerEdges + ' 条可归层边）',
      body: V.ulRaw(diagrams.architecture.pairs.map(pair =>
        '<b>' + E(pair.key.replace('>', ' → ')) + '</b>：' + E(pair.count) + ' 条'))
    }));

  p.sec('arch-verdict', '01', '一致性判定与边界',
    V.notice(m.architectureVerdict.summary) +
    V.table({
      caption: '三项机制差异；判定依据均指向机器台账',
      columns: [
        { label: 'ID', cell: r => V.code(r.id) },
        { label: '机制', cell: r => '<b>' + E(r.item) + '</b>' },
        { label: '原厂', cell: r => E(r.original) },
        { label: '当前', cell: r => E(r.current) },
        { label: '层', cell: r => V.code(r.layerId) },
        { label: '判定依据', cell: r => V.pathHtml(r.evidence) }
      ],
      rows: m.architectureVerdict.differences,
      rowId: r => idxId('diff', m.architectureVerdict.differences, r)
    }) +
    V.notice(m.architectureVerdict.boundary, 'unknown'));

  p.sec('arch-layers', '02', '8 层职责与当前映射',
    V.table({
      caption: 'L1–L8：原厂规模、被引用广度、当前工程与缺口',
      columns: [
        { label: '层', cell: r => V.code(r.id) + '<br>' + E(r.name) },
        { label: '原厂程序集', cell: r => V.ul(r.assemblies.map(E)) },
        { label: '行 / 类型 / 方法', cell: r => V.mono(F(r.original.lines) + ' / ' + F(r.original.types) + ' / ' + F(r.original.methods)) },
        { label: 'fanIn', sort: 'num', cls: 'num-cell', cell: r => E(F(r.original.fanIn)) },
        { label: '当前工程', cell: r => V.ul(r.currentProjects.map(E)) },
        { label: '当前行', sort: 'num', cls: 'num-cell', cell: r => E(F(r.currentLines)) },
        { label: '状态', cell: r => V.statusChip(r.status, r.status) },
        { label: '缺口', cell: r => E(r.gap) }
      ],
      rows: m.layers,
      rowId: r => idxId('layer', m.layers, r)
    }));

  p.sec('arch-assemblies', '03', '25 个原厂唯一程序集',
    V.table({
      caption: '行数 / 类型 / 方法 / 被引用广度，重复 pass 只取较大一次',
      columns: [
        { label: '#', sort: 'num', cls: 'num-cell', cell: (r, i) => E(String(i + 1).padStart(2, '0')) },
        { label: '程序集', cell: r => '<b>' + E(r.id) + '</b>' },
        { label: 'canonical tree', cell: r => V.pathHtml(r.canonicalTree) },
        { label: '.cs', sort: 'num', cls: 'num-cell', cell: r => E(F(r.cs)) },
        { label: '.xaml', sort: 'num', cls: 'num-cell', cell: r => E(F(r.xaml)) },
        { label: '行', sort: 'num', cls: 'num-cell', cell: r => E(F(r.lines)) },
        { label: '类型', sort: 'num', cls: 'num-cell', cell: r => E(F(r.types)) },
        { label: '方法', sort: 'num', cls: 'num-cell', cell: r => E(F(r.methods)) },
        { label: 'fanIn', sort: 'num', cls: 'num-cell', cell: r => E(F(r.fanIn)) },
        { label: '重复 pass', cell: r => E(r.duplicatePasses ? '是' : '否') },
        { label: '计数依据', cell: r => E(r.countBasis) }
      ],
      rows: m.assemblies,
      rowId: (r, i) => 'asm-' + (i + 1)
    }));

  p.sec('arch-projects', '04', '29 个当前工程',
    V.table({
      caption: 'src/** 中含 .csproj 的工程；Roslyn 仅语法解析',
      columns: [
        { label: '#', sort: 'num', cls: 'num-cell', cell: (r, i) => E(String(i + 1).padStart(2, '0')) },
        { label: '工程', cell: r => '<b>' + E(r.Name) + '</b>' },
        { label: '.cs', sort: 'num', cls: 'num-cell', cell: r => E(F(r.CsFiles)) },
        { label: '.xaml', sort: 'num', cls: 'num-cell', cell: r => E(F(r.XamlFiles)) },
        { label: '行', sort: 'num', cls: 'num-cell', cell: r => E(F(r.Lines)) },
        { label: '类型', sort: 'num', cls: 'num-cell', cell: r => E(F(r.Types)) },
        { label: '方法', sort: 'num', cls: 'num-cell', cell: r => E(F(r.Methods)) },
        { label: '属性', sort: 'num', cls: 'num-cell', cell: r => E(F(r.Properties)) },
        { label: '字段', sort: 'num', cls: 'num-cell', cell: r => E(F(r.Fields)) }
      ],
      rows: m.currentProjects,
      rowId: (r, i) => 'proj-' + (i + 1)
    }));

  p.sec('arch-deps', '05', '依赖边规模与判定依据',
    V.listPairs([
      ['原厂边', E(F(m.metrics.original.dependencies)) + ' 条 · ' + E(m.scope.method.fanIn)],
      ['当前边', E(F(m.metrics.current.dependencies)) + ' 条 · ' + E(m.scope.method.current)],
      ['规模判定', E(m.architectureVerdict.summary)]
    ]) +
    V.noticeHtml('三项机制差异的处置顺序见 <a href="' + V.hrefFor('05-p0-mechanism') + '">P0 机制层</a>；' +
      E(m.architectureVerdict.boundary), 'unknown'));
  return p.finish();
}

// ---------------------------------------------------------------- 页面 4 · 路线图
function renderRoadmap(m) {
  const p = V.createPage(m, meta(m, '03-roadmap'));
  p.intro(V.cards([
    { num: m.stages.length, label: '阶段', hint: 'S0 → S9 顺序固定', tone: 'done' },
    { num: m.stages[0].exitCriteria.length, label: 'S0 退出条件', hint: '切片真跑通才放行' },
    { num: m.priorities.length, label: '优先级项', hint: 'P0–P3 与 S1–S9 对应' },
    { num: m.stages.reduce((n, s) => n + s.forbiddenParallel.length, 0), label: '禁止并行项', hint: '逐阶段登记', tone: 'unknown' },
    { num: m.stages.reduce((n, s) => n + s.exitCriteria.length, 0), label: '退出条件', hint: '全部阶段合计' }
  ]));

  p.sec('road-stages', '01', '阶段总览',
    V.table({
      caption: 'S0–S9：前置、产出、退出条件与禁止并行项数量',
      columns: [
        { label: '阶段', cell: r => '<b>' + E(r.id) + '</b>' },
        { label: '名称', cell: r => '<a href="#' + E('stage-' + r.id) + '">' + E(r.name) + '</a>' },
        { label: '层级', cell: r => V.chip('idle', r.tier) },
        { label: '关联优先级', cell: r => r.priority ? V.code(r.priority.id) : '<span class="path">切片，无 P 编号</span>' },
        { label: '前置', cell: r => E(r.prerequisite) },
        { label: '产出', cell: r => E(r.deliverable) },
        { label: '退出条件', sort: 'num', cls: 'num-cell', cell: r => E(String(r.exitCriteria.length)) },
        { label: '禁止并行', sort: 'num', cls: 'num-cell', cell: r => E(String(r.forbiddenParallel.length)) }
      ],
      rows: m.stages
    }));

  const stageBlocks = m.stages.map((s, i) => {
    const rows = [
      { k: '前置', v: E(s.prerequisite) },
      { k: '产出', v: E(s.deliverable) },
      { k: '退出条件', v: V.ol(s.exitCriteria.map(E)) },
      { k: '禁止并行', v: V.ul(s.forbiddenParallel.map(E)) }
    ];
    if (s.priority) {
      rows.push({ k: '关联优先级', v: V.code(s.priority.id) + ' · ' + E(s.priority.item) });
      rows.push({ k: '依据', v: E(s.priority.why) });
      rows.push({ k: '第一动作', v: E(s.priority.firstAction) });
      rows.push({ k: '阻塞单元', v: V.ul(s.priority.blocks.map(E)) });
    }
    const wbsPkgs = m.wbs.workPackages.filter(pkg => pkg.stageIds.includes(s.id));
    rows.push({
      k: '对应 WBS 工作包',
      v: wbsPkgs.length
        ? V.ulRaw(wbsPkgs.map(pkg => '<a href="' + V.hrefFor('11-work-packages', 'wp-' + pkg.id) + '">' + E(pkg.id + ' · ' + pkg.title) + '</a>'))
        : '<span class="path">无直接阶段映射，按依赖接力顺序推进</span>'
    });
    rows.push({
      k: '任务卡入口',
      v: V.ulRaw([
        '<a href="' + V.hrefFor('12-unit-cards') + '">框架单元任务卡</a>',
        '<a href="' + V.hrefFor('13-tool-cards') + '">目录工具任务卡</a>',
        '<a href="' + V.hrefFor('14-entry-chains') + '">入口闭合链</a>'
      ])
    });
    return V.details({
      id: 'stage-' + s.id, cls: 'stage', open: i === 0,
      title: s.id + ' · ' + s.name + ' · ' + s.tier,
      body: definitionTable(rows)
    });
  }).join('');
  p.sec('road-detail', '02', '阶段明细（手风琴）',
    '<p class="lede">点击展开一个阶段；同一时刻只展开一个。脚本失效时仍可逐个手动展开。</p>' +
    '<div class="accordion">' + stageBlocks + '</div>');

  p.sec('road-order', '03', '顺序与并行规则',
    V.ul(m.governance.changeControl.map(E)) +
    V.notice(m.pageNotes.parallelExceptions + ' 当前 exceptions 为空数组。', 'cyan') +
    '<p>阶段门禁 G1–G8 与证据四态见 <a href="' + V.hrefFor('09-gates-evidence') + '">门禁与证据规则</a>。</p>');
  return p.finish();
}

// ---------------------------------------------------------------- 页面 5 · 纵向切片
function renderSlice(m) {
  const s = m.slice, s0 = m.stages[0];
  const p = V.createPage(m, meta(m, '04-vertical-slice'));
  p.intro(V.cards([
    { num: s.chain.length, label: '链路步骤', hint: 'ImageSource → 保存/重载', tone: 'done' },
    { num: s.c01.passedChecks, label: 'C01 通过检查', hint: s.c01.testProject },
    { num: s.directSdk.count + ' / ' + F(s.directSdk.totalArea), label: '直接 SDK / 本地链路', hint: 'BlobCount / TotalArea' },
    { num: s.originalVm.count + ' / ' + F(s.originalVm.totalArea), label: '原厂 VM', hint: '运行确认，差分开放', tone: 'amber' },
    { num: s.directSdk.outputs, label: '输出名数量', hint: '节点状态 ' + E(s.directSdk.nodeStatus) },
    { num: s.limits.length, label: '已知限制', hint: '不得抹平差分', tone: 'unknown' }
  ]));

  p.sec('slice-chain', '01', '全链路定义',
    V.timeline(s.chain.map(step => ({ title: step.step + ' · ' + step.name, text: step.concern }))) +
    V.notice(m.pageNotes.s0Scope, 'cyan'));

  p.sec('slice-evidence', '02', '当前证据状态与数值差分',
    V.table({
      caption: '同一输入的两侧可观察结果，差分必须保留',
      columns: [
        { label: '侧', cell: r => '<b>' + E(r.side) + '</b>' },
        { label: 'BlobCount', sort: 'num', cls: 'num-cell', cell: r => E(String(r.count)) },
        { label: 'TotalArea', sort: 'num', cls: 'num-cell', cell: r => E(F(r.area)) },
        { label: '证据类型', cell: r => V.chip(r.state, r.label) },
        { label: '来源', cell: r => V.pathHtml(r.source) }
      ],
      rows: [
        { side: '原厂 VM', count: s.originalVm.count, area: s.originalVm.totalArea, state: 'done', label: '运行确认', source: INPUT(m, 'vmStatus') + ' → currentSlice.c03RuntimeEvidence' },
        { side: '直接 SDK / 本地链路', count: s.directSdk.count, area: s.directSdk.totalArea, state: 'partial', label: '独立实现', source: INPUT(m, 'sliceRuntime') + ' → blob' }
      ]
    }) +
    V.noticeHtml('数值差分仍开放：' + E(s.c02Differential.status) + '。差分报告：' + V.pathHtml(s.c02Differential.report) + '。禁止用测试通过冒充等价。', 'unknown') +
    V.listPairs([
      ['C01 检查', E(s.c01.passedChecks) + ' 项 · ' + V.pathHtml(s.c01.testProject)],
      ['C01 报告', V.pathHtml(s.c01.report)],
      ['当前切片证据', V.pathHtml(s.currentSliceEvidence)],
      ['已知限制', V.pathHtml(s.knownLimit)]
    ]));

  p.sec('slice-verify', '03', '逐步验收动作',
    V.ol(s.verification.map(E)) +
    V.evidence('验收口径', '<p>' + E(m.frameworks.stageCriteria.validation) + '</p>'));

  p.sec('slice-limits', '04', '已知差异与不可越过的边界',
    V.ul(s.limits.map(E)) +
    V.notice(m.pageNotes.sliceLimits, 'unknown'));

  p.sec('slice-gate', '05', 'S0 退出条件对照',
    V.ol(s0.exitCriteria.map(E)) +
    '<p>退出后进入 <a href="' + V.hrefFor('05-p0-mechanism') + '">P0 机制层</a>；若要求并行推进，按 <a href="' + V.hrefFor('03-roadmap', 'road-order') + '">并行规则</a> 登记例外。</p>');

  const ph = m.phase1Landing;
  p.sec('slice-phase1', '06', '第一阶段落地状态（证据闭环，根因未闭）',
    V.notice(ph.decision, 'unknown') +
    V.cards([
      { num: ph.diffPreserved.delta.blobCount, label: '计数差分', hint: '原厂 ' + ph.diffPreserved.originalVm.blobCount + ' / 直接 SDK ' + ph.diffPreserved.directSdkAndLocal.blobCount, tone: 'amber' },
      { num: F(ph.diffPreserved.delta.totalArea), label: '面积差分', hint: '原厂 ' + F(ph.diffPreserved.originalVm.totalArea) + ' / 直接 SDK ' + F(ph.diffPreserved.directSdkAndLocal.totalArea), tone: 'amber' },
      { num: ph.unknowns, label: '未决项', hint: '全部登记 unknown', tone: 'unknown' },
      { num: ph.nextEvidenceActions, label: '下一步取证动作', hint: '需可查询原厂过程或 provider 修复' },
      { num: ph.productChanges.length, label: '产品代码改动', hint: '证据门决策：未捕获前不改算法', tone: 'done' }
    ]) +
    V.table({
      caption: 'S0 三退出条件逐条登记（历史报告未在本阶段复跑的显式标注）',
      columns: [
        { label: '条件', cell: r => V.code(r.id) },
        { label: '状态', cell: r => V.chip(r.state === 'satisfied' ? 'done' : 'partial', r.state) },
        { label: '退出条件原文', cell: r => E(s0.exitCriteria[Number(r.id.slice(-1)) - 1] || '') }
      ],
      rows: Object.entries(ph.s0ExitGates).map(([id, state]) => ({ id, state }))
    }) +
    V.listPairs([
      ['闭环状态', V.code(ph.status)],
      ['总判定', V.code(ph.s0GateStatus)],
      ['剩余边界', E(ph.remainingBoundary)],
      ['已确认', V.ul(ph.confirmed.map(E))],
      ['证据卡', V.pathHtml(ph.evidenceCard)],
      ['复核台账', V.pathHtml(ph.evidenceReview)],
      ['门禁对照', V.pathHtml(ph.gateCheck)],
      ['人读结论', V.pathHtml(ph.humanReview)],
      ['未重跑', V.ul(ph.notReRun.map(E))],
      ['人工验收', E(ph.manualAcceptance)],
      ['基线', E(ph.baseline)]
    ]) +
    V.notice('REA native provider：' + E(ph.reaProvider), 'cyan'));
  return p.finish();
}

// ---------------------------------------------------------------- 页面 6/7/8 · 机制与覆盖
function unitsForPriority(priority, m) {
  const tokens = priority.blocks.map(String);
  return m.frameworkUnits.filter(u => tokens.some(t => u.id === t || u.id.indexOf(t + '.') === 0 || u.package === t));
}
function renderPriorityPage(m, slug, ranks, introNote) {
  const p = V.createPage(m, meta(m, slug));
  const selected = ranks.map(rank => m.priorities[rank - 1]);
  const allUnits = uniq(selected.flatMap(pr => unitsForPriority(pr, m).map(u => u.id)));
  p.intro(V.cards([
    { num: selected.length, label: '优先级项', hint: selected.map(pr => pr.id.split('-').slice(0, 2).join('-')).join(' / '), tone: 'amber' },
    { num: uniq(selected.flatMap(pr => pr.layers)).length, label: '涉及架构层', hint: '按 baseline.layers 归层' },
    { num: selected.reduce((n, pr) => n + pr.blocks.length, 0), label: '阻塞项', hint: 'priority.blocks 原样登记', tone: 'unknown' },
    { num: allUnits.length, label: '关联具名单元', hint: '由 blocks 反查 96 单元台账' }
  ]) + V.notice(introNote, 'cyan'));

  selected.forEach((pr, index) => {
    const stage = m.stages.find(s => s.priority && s.priority.id === pr.id);
    must(stage, '优先级缺少对应阶段: ' + pr.id);
    const layers = m.layers.filter(layer => pr.layers.includes(layer.id));
    const units = unitsForPriority(pr, m);
    const artifacts = uniq(units.flatMap(u => u.implementationArtifacts));
    const reports = uniq(units.flatMap(u => u.sourceAndOperationReports));
    const definition = [
      { k: '优先级 / 层', v: V.code(pr.id) + ' · rank ' + E(pr.rank) + ' · ' + V.ul(pr.layers.map(E)) },
      { k: '原厂证据路径', v: V.ul(pr.fanInComponents.map(E)) + joinCodes(reports, 8) },
      { k: '当前实现位置', v: joinCodes(artifacts, 8) + V.ul(layers.flatMap(layer => layer.currentProjects).map(E)) },
      { k: '缺口描述', v: E(pr.why) + V.ul(layers.map(layer => layer.id + '：' + layer.gap)) },
      { k: '阻塞单元', v: V.ul(pr.blocks.map(E)) },
      { k: '第一动作', v: '<b>' + E(pr.firstAction) + '</b>' },
      { k: '完成判据', v: V.ol(stage.exitCriteria.map(E)) }
    ];
    const unitTable = V.table({
      caption: '由 blocks 反查到的具名单元（' + units.length + ' 项）',
      columns: [
        { label: '单元 ID', cell: u => V.code(u.id) },
        { label: '判据', cell: u => E(u.criterion) },
        { label: '源码', cell: u => V.statusChip(u.source, u.source) },
        { label: '实施', cell: u => V.statusChip(u.implementation, u.implementation) },
        { label: '验证', cell: u => V.statusChip(u.validation, u.validation) },
        { label: '限制', cell: u => E(u.limitation) }
      ],
      rows: units
    });
    p.sec('priority-' + pr.rank, String(index + 1).padStart(2, '0'), pr.item,
      definitionTable(definition) +
      V.details({ title: '关联具名单元与三层状态', body: unitTable, open: false }) +
      V.notice('阶段 ' + stage.id + ' · ' + stage.name + ' 的禁止并行项：' + stage.forbiddenParallel.join('；'), 'unknown'));
  });

  p.sec('shared-constraints', String(selected.length + 1).padStart(2, '0'), '共享约束',
    V.listPairs([
      ['三层判据 · 源码', E(m.frameworks.stageCriteria.source)],
      ['三层判据 · 实施', E(m.frameworks.stageCriteria.implementation)],
      ['三层判据 · 验证', E(m.frameworks.stageCriteria.validation)],
      ['状态策略', V.ulRaw(Object.entries(m.frameworks.statusPolicy).map(([k, v]) => V.code(k) + ' ' + E(v)))],
      ['验证新鲜度', E(m.frameworks.validationFreshness)]
    ]) +
    V.notice(m.pageNotes.priorityUnits, 'cyan') +
    '<p><a href="' + V.hrefFor('09-gates-evidence', 'units-96') + '">查看 96 单元逐项台账 →</a></p>');
  return p.finish();
}
function renderP0(m) {
  return renderPriorityPage(m, '05-p0-mechanism', [1, 2, 3], m.pageNotes.p0Intro);
}
function renderP1(m) {
  return renderPriorityPage(m, '06-p1-coverage', [4, 5], m.pageNotes.p1Intro);
}
function renderP2P3(m) {
  return renderPriorityPage(m, '07-p2-p3', [6, 7, 8, 9], m.pageNotes.p23Intro);
}

// ---------------------------------------------------------------- 页面 9 · 162 工具台账
function renderTools(m) {
  const p = V.createPage(m, meta(m, '08-tools-162'));
  const counts = m.tools.reduce((acc, tool) => { acc[tool.batch] = (acc[tool.batch] || 0) + 1; return acc; }, {});
  const progress = m.tools.reduce((acc, tool) => { acc[tool.progress] = (acc[tool.progress] || 0) + 1; return acc; }, {});
  const cardByKey = new Map(m.wbs.toolCards.map(card => [card.key, card]));
  p.intro(V.cards([
    { num: m.tools.length, label: '目录工具', hint: '全量纳入，不抽样', tone: 'amber' },
    { num: m.categories.length, label: '原厂分类', hint: '按 category 分组' },
    { num: m.batchRules.length, label: '优先级批次', hint: 'B0 / B1 / B2' },
    { num: counts.B0 || 0, label: 'B0 首批', hint: '随 S0 切片验收' },
    { num: counts.B1 || 0, label: 'B1 后续', hint: '依赖图像源输出契约' },
    { num: counts.B2 || 0, label: 'B2 全量', hint: 'S9 逐工具铺开', tone: 'unknown' }
  ]));

  const options = (items, value, label) => items.map(item =>
    '<option value="' + E(value(item)) + '">' + E(label(item)) + '</option>').join('');
  const filters = '<div class="filters">' +
    '<label>名称 / 工具键<input id="tool-query" type="search" placeholder="输入工具名" autocomplete="off"></label>' +
    '<label>分类<select id="tool-category"><option value="">全部分类</option>' +
    options(m.categories, g => g.name, g => g.name + ' (' + g.tools.length + ')') + '</select></label>' +
    '<label>批次<select id="tool-batch"><option value="">全部批次</option>' +
    options(m.batchRules, r => r.batch, r => r.batch + ' · ' + r.label) + '</select></label>' +
    '<label>证据状态<select id="tool-status"><option value="">全部状态</option>' +
    options(m.evidenceStates, s => s.id, s => s.label) + '</select></label>' +
    '<span class="filter-count" id="tool-count">' + m.tools.length + ' / ' + m.tools.length + '</span></div>';

  const columns = [
    { label: '工具键', cell: t => '<b>' + V.code(t.key) + '</b>' },
    { label: '显示名', cell: t => E(t.displayName) },
    { label: 'ModuleId', cell: t => V.mono(t.moduleId === null ? '—' : t.moduleId) },
    { label: '分类', cell: t => E(t.category) },
    { label: '批次', cell: t => V.chip(t.batch === 'B0' ? 'done' : t.batch === 'B1' ? 'partial' : 'idle', t.batch + ' · ' + t.batchLabel) },
    { label: '证据四态', cell: t => V.statusChip(t.evidenceState, (m.evidenceStates.find(s => s.id === t.evidenceState) || {}).label || t.evidenceState) },
    { label: '进度', cell: t => V.statusChip(t.progress, t.progress === 'completed' ? '完成' : t.progress === 'active' ? '在制' : '未开始') },
    { label: 'XML / DLL / 页签 / 参数 / 入 / 出', cell: t => V.mono([t.counts.xmls, t.counts.binaries, t.counts.uiTabs, t.counts.algorithmParameters, t.counts.inputs, t.counts.outputs].join(' / ')) },
    { label: '当前接收路径', cell: t => V.pathHtml(t.sourcePath) },
    { label: '第一动作', cell: t => E(t.nextAction) },
    { label: '台账明细', cell: t => V.details({
      title: '展开',
      body: V.listPairs([
        ['静态状态', E(t.staticStatus)],
        ['实施状态', E(t.implementationStatus)],
        ['运行对比', E(t.runtimeStatus)],
        ['依赖状态', E(t.dependencyStatus)],
        ['native 分析', E(t.nativeAnalysis)],
        ['语义完整性', E(t.semanticCompleteness)],
        ['分析范围', E(t.analysisScope)],
        ['分批规则', E(t.batchRule)]
      ])
    }) },
    { label: '任务卡', cell: t => '<a href="' + V.hrefFor('13-tool-cards', toolCardId(m.wbs, cardByKey.get(t.key))) + '">查看 →</a>' }
  ];
  const rowsFor = group => V.table({
    caption: group.name + ' · ' + group.tools.length + ' 个工具（声明 ' + group.declared + '）',
    columns,
    rows: group.tools,
    rowId: t => idxId('tool', m.tools, t),
    rowAttrs: t => 'data-tool-row data-tool-name="' + E(t.displayName) + '" data-tool-key="' + E(t.key) +
      '" data-tool-category="' + E(t.category) + '" data-tool-batch="' + E(t.batch) +
      '" data-tool-status="' + E(t.evidenceState) + '"'
  });
  const groups = m.categories.map((group, i) => V.details({
    id: 'cat-' + (i + 1), open: true,
    title: (i + 1) + ' · ' + group.name + '（' + group.tools.length + '）',
    body: rowsFor(group)
  })).join('');

  p.sec('tool-filters', '01', '筛选与统计',
    filters +
    V.table({
      caption: '分批规则（来自编辑源，计数由 162 台账重算）',
      columns: [
        { label: '批次', cell: r => V.code(r.batch) },
        { label: '标签', cell: r => E(r.label) },
        { label: 'priority', cell: r => V.mono(r.priority) },
        { label: '工具数', sort: 'num', cls: 'num-cell', cell: r => E(String(counts[r.batch] || 0)) },
        { label: '规则', cell: r => E(r.rule) }
      ],
      rows: m.batchRules
    }) +
    V.notice('进度汇总：完成 ' + E(progress.completed || 0) + ' · 在制 ' + E(progress.active || 0) + ' · 未开始 ' + E(progress['not-started'] || 0) +
      '。计数口径来自批次台账，不代表整工具源码级重建完成。', 'cyan'));

  p.sec('tool-groups', '02', '14 个原厂分类 · 162 工具逐项台账',
    '<p class="lede">每个工具一行：身份、类别、当前接收路径、优先级批次、证据状态与第一动作。表格列头可点击排序，筛选只作用于前端呈现，不改台账。</p>' +
    groups);

  const gap = m.batchIdentityGaps;
  p.sec('tool-gaps', '03', '批次身份差异登记',
    V.notice('批次台账中未在 162 目录身份里匹配到的项：completed = ' + E(gap.completed.join('、') || '无') +
      '；active = ' + E(gap.active.join('、') || '无') + '。' + E(gap.policy), 'unknown') +
    V.notice(m.pageNotes.toolsPlanView, 'cyan'));

  p.sec('tool-links', '04', '与既有证据页互链',
    V.ulRaw([
      '<a href="' + V.hrefFor('13-tool-cards') + '">目录工具任务卡（完整执行计划）</a>',
      '<a href="' + V.hrefFor('00-master-plan') + '">权威总纲</a>',
      '<a href="../architecture-recovery/tool-plugin-map.html">插件工具对照证据页（原页）</a>',
      '<a href="../framework-architecture.html">主框架全景（原页）</a>',
      V.pathHtml(INPUT(m, 'inventory')) + ' · 身份清单',
      V.pathHtml(INPUT(m, 'coverage')) + ' · 覆盖审计',
      V.pathHtml(INPUT(m, 'nextBatch')) + ' · 11 工具批次台账',
      V.pathHtml(INPUT(m, 'sourceLevel')) + ' · 标定 10 工具状态'
    ]) +
    '<p>阶段安排：<a href="' + V.hrefFor('03-roadmap', 'stage-S9') + '">S9 逐工具铺开</a>；整工具计分口径见 <a href="' + V.hrefFor('09-gates-evidence', 'gate-G6') + '">G6 门禁</a>。</p>');
  return p.finish();
}

// ---------------------------------------------------------------- 页面 10 · 门禁与证据
function renderGates(m) {
  const p = V.createPage(m, meta(m, '09-gates-evidence'));
  p.intro(V.cards([
    { num: m.evidenceStates.length, label: '证据四态', hint: '每个结论必须标注其一', tone: 'done' },
    { num: m.stageGates.length, label: '阶段门禁', hint: 'G1–G8' },
    { num: m.entryGates.length, label: '入口完整性门禁', hint: 'E0–E7' },
    { num: m.frameworkUnits.length, label: '具名验收单元', hint: '32 包 × 3 单元' },
    { num: m.acceptanceLevels.length, label: '验收等级', hint: 'L1 报告 / L2 自动 / L3 全量' },
    { num: m.entryCounts.closedEntryChains, label: '已闭合入口链', hint: '候选 ' + F(m.entryCounts.candidates) + ' 项', tone: 'unknown' }
  ]));

  p.sec('evidence-4', '01', '证据四态',
    V.table({
      caption: '源码确认 / 运行确认 / 独立实现 / 未知',
      columns: [
        { label: '状态', cell: r => V.statusChip(r.id, r.label) },
        { label: 'ID', cell: r => V.code(r.id) },
        { label: '判定规则', cell: r => E(r.rule) },
        { label: '可陈述范围', cell: r => E(r.claim) }
      ],
      rows: m.evidenceStates,
      rowId: r => 'ev-' + r.id
    }) +
    V.notice(m.pageNotes.evidenceBoundary, 'unknown'));

  p.sec('acceptance', '02', '验收分级',
    V.table({
      caption: 'L1 / L2 / L3：范围逐级扩大，人工主机验收单列',
      columns: [
        { label: '等级', cell: r => V.code(r.id) },
        { label: '名称', cell: r => '<b>' + E(r.name) + '</b>' },
        { label: '范围', cell: r => E(r.scope) },
        { label: '产出', cell: r => E(r.output) }
      ],
      rows: m.acceptanceLevels
    }));

  p.sec('gates-g', '03', '阶段门禁矩阵 G1–G8',
    V.table({
      caption: '任何单元退出前逐条对照',
      columns: [
        { label: '门禁', cell: r => V.code(r.id) },
        { label: '名称', cell: r => '<b>' + E(r.name) + '</b>' },
        { label: '规则', cell: r => E(r.rule) }
      ],
      rows: m.stageGates,
      rowId: r => 'gate-' + r.id
    }));

  const g3 = m.stageGates.find(g => g.id === 'G3');
  p.sec('unit-card', '04', '单元证据卡模板',
    V.listPairs([
      ['调用链闭环（G3）', E(g3.rule)],
      ['源码层判据', E(m.frameworks.stageCriteria.source)],
      ['实施层判据', E(m.frameworks.stageCriteria.implementation)],
      ['验证层判据', E(m.frameworks.stageCriteria.validation)],
      ['状态策略', V.code('done / partial / blocked / not_started / unverified / failed') + '，不给 partial 折半'],
      ['验证新鲜度', E(m.frameworks.validationFreshness)]
    ]) +
    V.notice(m.pageNotes.unitCard, 'cyan'));

  p.sec('gates-e', '05', '入口完整性门禁 E0–E7',
    V.table({
      caption: '入口清单：' + E(m.entryCounts.menuNamedDeclarations) + ' 菜单命名声明 / ' +
        E(m.entryCounts.toolbarDeclarations) + ' 工具栏 / ' + E(m.entryCounts.hiddenNamedDeclarations) +
        ' 隐藏命名 / ' + F(m.entryCounts.sourceFiles) + ' 源文件 / ' + F(m.entryCounts.candidates) + ' 候选',
      columns: [
        { label: '门禁', cell: r => V.code(r.id) },
        { label: '标题', cell: r => '<b>' + E(r.title) + '</b>' },
        { label: '状态', cell: r => V.statusChip(r.state, r.state) },
        { label: '缺口', cell: r => E(r.gap) }
      ],
      rows: m.entryGates,
      rowId: r => 'entry-' + r.id
    }) +
    V.ulRaw([
      '<a href="' + V.hrefFor('14-entry-chains') + '">E0–E7 六段闭合链任务卡</a>',
      '<a href="' + V.hrefFor('12-unit-cards') + '">框架单元任务卡</a>'
    ]));

  const packages = uniq(m.frameworkUnits.map(u => u.package));
  const cardById = new Map(m.wbs.unitCards.map(card => [card.id, card]));
  const unitTable = group => V.table({
    caption: group + ' · ' + m.frameworkUnits.filter(u => u.package === group).length + ' 个具名单元',
    columns: [
      { label: '单元 ID', cell: u => '<b>' + V.code(u.id) + '</b>' },
      { label: '判据', cell: u => E(u.criterion) },
      { label: '源码', cell: u => V.statusChip(u.source, u.source) },
      { label: '实施', cell: u => V.statusChip(u.implementation, u.implementation) },
      { label: '验证', cell: u => V.statusChip(u.validation, u.validation) },
      { label: '阻塞 ID', cell: u => u.blockerIds.length ? V.ul(u.blockerIds.map(E)) : '<span class="path">—</span>' },
      { label: '限制', cell: u => E(u.limitation) },
      { label: '任务卡', cell: u => '<a href="' + V.hrefFor('12-unit-cards', unitCardId(m.wbs, cardById.get(u.id))) + '">查看 →</a>' }
    ],
    rows: m.frameworkUnits.filter(u => u.package === group),
    rowId: u => idxId('unit', m.frameworkUnits, u),
    rowAttrs: u => 'data-unit-id="' + E(u.id) + '"'
  });
  const blocks = packages.map((pkg, i) => V.details({
    id: 'unitpkg-' + (i + 1), open: i === 0,
    title: pkg + ' · ' + (m.frameworkUnits.find(u => u.package === pkg) || {}).packageTitle,
    body: unitTable(pkg)
  })).join('');
  p.sec('units-96', '06', '96 个具名单元逐项台账',
    '<p class="lede">按 32 个包分组；三层状态独立等权计数，逐项状态来自 ' + V.code(INPUT(m, 'progressAudit')) + '。</p>' +
    blocks);

  const layerRows = Object.entries(m.progress.framework.layers).map(([key, value]) => ({
    key, done: value.done, total: value.total, percent: value.percent, remaining: value.remaining
  }));
  p.sec('units-summary', '07', '三层状态汇总',
    V.table({
      caption: '源码 / 实施 / 验证三层；分母固定 96',
      columns: [
        { label: '层', cell: r => V.code(r.key) },
        { label: 'done', sort: 'num', cls: 'num-cell', cell: r => E(String(r.done)) },
        { label: 'total', sort: 'num', cls: 'num-cell', cell: r => E(String(r.total)) },
        { label: 'percent', sort: 'num', cls: 'num-cell', cell: r => E(V.pct(r.percent)) },
        { label: 'remaining', sort: 'num', cls: 'num-cell', cell: r => E(String(r.remaining)) }
      ],
      rows: layerRows
    }) +
    V.noticeHtml('三层全 done = ' + E(m.progress.framework.allThreeDone) + '/96 = ' + V.pct(m.progress.framework.allThreePercent) +
      '；41/96 与 42/96 的汇总字段冲突见 <a href="' + V.hrefFor('10-governance', 'conflict') + '">治理页</a>。', 'cyan'));
  return p.finish();
}

// ---------------------------------------------------------------- 页面 11 · 变更治理
function renderGovernance(m) {
  const p = V.createPage(m, meta(m, '10-governance'));
  const fw = m.progress.framework;
  p.intro(V.cards([
    { num: m.inputs.length, label: '机器台账', hint: '页面数字唯一来源', tone: 'amber' },
    { num: m.supporting.length, label: '补充源', hint: '运行证据 / 人读基线 / 编辑源' },
    { num: m.governance.changeControl.length, label: '变更控制项', hint: '台账为真相源' },
    { num: m.governance.prohibitions.length, label: '禁止项', hint: '逐条不可越过', tone: 'unknown' },
    { num: m.governance.ownership.length, label: '责任边界', hint: '框架线 / 插件线 / 用户' },
    { num: 0, label: '并行例外', hint: 'exceptions 当前为空数组', tone: 'done' }
  ]));

  p.sec('change', '01', '变更控制',
    V.ul(m.governance.changeControl.map(E)) +
    V.notice(m.pageNotes.htmlIsArtifact, 'cyan'));

  p.sec('ownership', '02', '责任边界',
    V.table({
      caption: '两条工作线 + 用户；跨工作树改动必须走合同请求，不静默合入',
      columns: [
        { label: '#', sort: 'num', cls: 'num-cell', cell: (r, i) => E(String(i + 1)) },
        { label: '边界', cell: r => E(r) }
      ],
      rows: m.governance.ownership
    }));

  p.sec('prohibitions', '03', '禁止项',
    V.ul(m.governance.prohibitions.map(E)) +
    V.notice('本站点生成与校验不自动提交、不自动推送、不修改另一工作树。', 'unknown'));

  p.sec('conflict', '04', '口径冲突登记：41/96 与 42/96',
    V.notice(m.conflict) +
    V.table({
      caption: '两个汇总字段并列，等待用户决定统一口径',
      columns: [
        { label: '口径', cell: r => '<b>' + E(r.name) + '</b>' },
        { label: '数值', cell: r => V.mono(r.value) },
        { label: '来源', cell: r => V.pathHtml(r.source) }
      ],
      rows: [
        { name: '逐项重算（42）', value: fw.current.done + '/' + fw.current.total + ' = ' + V.pct(fw.current.percent), source: fw.current.source },
        { name: '历史汇总（41）', value: fw.legacy.done + '/' + fw.legacy.total + ' = ' + V.pct(fw.legacy.percent), source: fw.legacy.source }
      ]
    }) +
    V.evidence('summaryReconciliation', '<p>' + E(fw.current.reconciliation) + '</p>') +
    '<p>索引页同样并列展示：<a href="' + V.hrefFor('index', 'conflict') + '">索引 · 口径冲突</a>。</p>');

  const commands = m.scope.regenerate.split(';').map(s => s.trim()).filter(Boolean);
  p.sec('recompute', '05', '复算流程',
    V.listPairs([
      ['步骤 1 · 原始计数', V.ulRaw(commands.map(c => V.code(c)))],
      ['步骤 2 · 站点重跑', V.code('node tests/ArchitectureRecovery/build-reverse-plan.cjs')],
      ['步骤 3 · 独立校验', V.code('node tests/ArchitectureRecovery/verify-reverse-plan.cjs')],
      ['步骤 4 · 机器数据', '<a href="plan-data.json">plan-data.json</a>（含 inputs 哈希、builtFrom、阶段与批次、exceptions）'],
      ['原始计数', V.pathHtml(m.scope.sourceInventory)],
      ['映射判断', V.pathHtml(m.scope.mappingTable)]
    ]));

  const sources = m.inputs.concat(m.supporting);
  p.sec('inputs', '06', '输入台账与哈希',
    V.table({
      caption: sources.length + ' 个输入文件；构建前后各校验一次 SHA-256，变化即中止',
      columns: [
        { label: '#', sort: 'num', cls: 'num-cell', cell: (r, i) => E(String(i + 1).padStart(2, '0')) },
        { label: '类别', cell: (r, i) => V.chip(i < m.inputs.length ? 'done' : 'partial', i < m.inputs.length ? '机器台账' : '补充源') },
        { label: '路径', cell: r => V.pathHtml(r.path) },
        { label: 'SHA-256', cell: r => V.mono(r.sha256) },
        { label: 'bytes', sort: 'num', cls: 'num-cell', cell: r => E(F(r.bytes)) }
      ],
      rows: sources,
      rowId: (r, i) => 'input-' + (i + 1)
    }));

  p.sec('determinism', '07', '确定性产物与例外登记',
    V.listPairs([
      ['builtFrom', V.code(m.builtFrom.head)],
      ['inputsDigest', V.code(m.builtFrom.inputsDigest)],
      ['wall-clock', E('站点不写入生成时间；若需时间只在 plan-data.json 的 generatedUtc 登记并被校验排除（当前未启用）')],
      ['exceptions', V.code('[]') + ' · S4 之后如需层内并行，必须在此登记并写明原因'],
      ['离线约束', E('无 http(s) 外链、无 CDN、无网络请求；file:// 直接可读')]
    ]) +
    V.notice(m.pageNotes.verifyAll, 'cyan'));

  p.sec('legacy', '08', '旧计划处置登记',
    V.table({
      caption: '既有分散计划逐份登记；文件保留为历史，既有结论数字不改',
      columns: [
        { label: '文件', cell: r => V.pathHtml(r.path) },
        { label: '标题', cell: r => '<b>' + E(r.title) + '</b>' },
        { label: '处置', cell: r => E(r.disposition) },
        { label: '仍有效部分', cell: r => E(r.effectiveParts) },
        { label: '状态', cell: r => V.chip('partial', r.status) },
        { label: '说明', cell: r => E(r.note) }
      ], rows: m.wbs.legacyPlans
    }) +
    V.notice(m.pageNotes.legacyRegister, 'cyan') +
    '<p>完整处置表与唯一执行口径见 <a href="' + V.hrefFor('00-master-plan', 'mp-legacy') + '">权威总纲</a>。</p>');
  return p.finish();
}

// ---------------------------------------------------------------- 页面 12 · 权威总纲
function renderMasterPlan(m) {
  const w = m.wbs, mp = m.masterPlan;
  const p = V.createPage(m, meta(m, '00-master-plan'));
  p.intro(
    '<div class="master-hero"><h2>' + E(mp.authority) + '</h2>' +
    '<p>' + E(mp.authorityBoundary) + '</p>' +
    '<div class="built">builtFrom ' + E(m.builtFrom.head.slice(0, 8)) + ' / ' +
    E(m.builtFrom.inputsDigest.slice(0, 12)) + '</div></div>' +
    V.cards([
      { num: w.counts.workPackages, label: '工作包', hint: '可派工一级单元', tone: 'amber' },
      { num: w.counts.unitCards, label: '框架单元任务卡', hint: '逐单元取证与退出判据' },
      { num: w.counts.toolCards, label: '目录工具任务卡', hint: '全量目录工具', tone: 'cyan' },
      { num: w.counts.chainNodes, label: '闭合链节点', hint: w.counts.entryChains + ' 门禁 × 6 段' },
      { num: w.counts.legacyPlans, label: '旧计划处置', hint: '保留历史，不改结论' },
      { num: w.counts.workPackageFieldGaps, label: '台账字段缺口', hint: '显式登记 unknown', tone: 'unknown' }
    ]));

  p.sec('mp-scope', '01', '范围与边界',
    '<div class="two-col">' +
    '<div class="panel"><h3>做什么</h3>' + V.ul(mp.scope.map(E)) + '</div>' +
    '<div class="panel"><h3>不做什么</h3>' + V.ul(mp.outOfScope.map(E)) + '</div>' +
    '<div class="panel"><h3>完成定义</h3>' + V.ul(mp.completionDefinition.map(E)) + '</div>' +
    '<div class="panel"><h3>责任边界</h3><p>' + E(mp.responsibilityNote) + '</p>' +
    '<p>' + E(mp.stageGateNote) + '</p></div></div>');

  p.sec('mp-rules', '02', '执行规则与计划状态机',
    V.ul(mp.executionRules.map(E)) +
    V.table({
      caption: '计划状态机：独立编排，不等于证据四态，也不改写台账三层状态',
      columns: [
        { label: '状态', cell: r => V.statusChip(r.id, r.label) },
        { label: 'ID', cell: r => V.code(r.id) },
        { label: '判定规则', cell: r => E(r.rule) }
      ],
      rows: m.taskStateMachine.states,
      rowId: r => 'state-' + r.id
    }) +
    V.ul(m.taskStateMachine.transitions.map(E)) +
    V.notice(m.pageNotes.cardsAreActions, 'unknown'));

  p.sec('mp-gates', '03', '阶段门、证据四态与派工入口',
    V.table({
      caption: '阶段门 G1–G8（规则原文见门禁页）',
      columns: [
        { label: '门禁', cell: r => V.code(r.id) },
        { label: '名称', cell: r => '<b>' + E(r.name) + '</b>' },
        { label: '规则', cell: r => E(r.rule) }
      ], rows: m.stageGates
    }) +
    V.table({
      caption: '证据四态（每个结论必须标注其一）',
      columns: [
        { label: '状态', cell: r => V.statusChip(r.id, r.label) },
        { label: '判定规则', cell: r => E(r.rule) },
        { label: '可陈述范围', cell: r => E(r.claim) }
      ], rows: m.evidenceStates
    }) +
    V.ulRaw([
      '<a href="' + V.hrefFor('11-work-packages') + '">WBS 工作包（派工入口）</a>',
      '<a href="' + V.hrefFor('12-unit-cards') + '">框架单元任务卡</a>',
      '<a href="' + V.hrefFor('13-tool-cards') + '">目录工具任务卡</a>',
      '<a href="' + V.hrefFor('14-entry-chains') + '">入口闭合链</a>',
      '<a href="' + V.hrefFor('09-gates-evidence') + '">门禁与证据规则（原文）</a>'
    ]));

  p.sec('mp-legacy', '04', '旧计划处置登记',
    V.table({
      caption: w.counts.legacyPlans + ' 份既有分散计划的处置状态；文件保留，一字不改',
      columns: [
        { label: '文件', cell: r => V.pathHtml(r.path) },
        { label: '标题', cell: r => '<b>' + E(r.title) + '</b>' },
        { label: '处置', cell: r => E(r.disposition) },
        { label: '仍有效部分', cell: r => E(r.effectiveParts) },
        { label: '状态', cell: r => V.chip('partial', r.status) },
        { label: '说明', cell: r => E(r.note) }
      ],
      rows: w.legacyPlans,
      rowId: r => 'legacy-' + V.slugId('id', r.path)
    }) +
    V.notice(mp.legacyNote) +
    V.notice(m.pageNotes.legacyRegister, 'cyan'));

  p.sec('mp-nav', '05', '全站导航（' + m.pages.length + ' 页）',
    V.table({
      caption: '索引 + 总纲 + 基线 + 路线 + 任务 + 执行视图 + 台账 + 门禁 + 治理',
      columns: [
        { label: '#', sort: 'num', cls: 'num-cell', cell: (r, i) => E(String(i + 1).padStart(2, '0')) },
        { label: '页面', cell: r => '<a href="' + V.hrefFor(r.slug) + '">' + E(r.title) + '</a>' },
        { label: '层级', cell: r => V.chip('idle', r.tier) },
        { label: '内容', cell: r => E(r.description) }
      ], rows: m.pages
    }) +
    V.notice(m.pageNotes.masterAuthority, 'cyan'));
  return p.finish();
}

// ---------------------------------------------------------------- 页面 13 · WBS 工作包
function renderWorkPackages(m) {
  const w = m.wbs;
  const p = V.createPage(m, meta(m, '11-work-packages'));
  const unitCardById = new Map(w.unitCards.map(card => [card.id, card]));
  const groups = uniq(w.workPackages.map(pkg => pkg.group)).sort();
  const waves = uniq(w.workPackages.map(pkg => 'wave-' + pkg.wave)).sort();
  const dependencyCount = w.workPackages.reduce((n, pkg) => n + pkg.dependencies.length, 0);
  p.intro(V.cards([
    { num: w.counts.workPackages, label: '一级工作包', hint: '来自框架逆向任务台账', tone: 'amber' },
    { num: w.counts.unitCards, label: '关联具名单元', hint: '每包 3 个，一一对应' },
    { num: dependencyCount, label: '依赖边', hint: '全部可解析，无环' },
    { num: w.counts.workPackageFieldGaps, label: '台账字段缺口', hint: '原样登记 unknown', tone: 'unknown' },
    { num: uniq(w.workPackages.map(pkg => pkg.planState)).length, label: '计划状态种类', hint: '由三层单元状态派生' }
  ]));

  p.sec('wp-order', '01', '依赖接力顺序',
    V.notice(m.pageNotes.wbsDependency, 'cyan') +
    V.timeline(w.dependencyOrder.map(id => {
      const pkg = w.workPackages.find(item => item.id === id);
      return { title: pkg.orderIndex + ' · ' + pkg.id + ' ' + pkg.title, text: '波次 ' + pkg.wave + ' · 前置 ' + (pkg.dependencies.join('、') || '无') + ' · ' + planStateLabel(m, pkg.planState) };
    })));

  const filters = V.filtersBar({
    total: w.counts.workPackages,
    selects: [
      { id: 'task-state', label: '计划状态', options: m.taskStateMachine.states.map(s => ({ value: s.id, label: s.label })) },
      { id: 'task-category', label: '工作线', options: groups.map(g => ({ value: g, label: g + ' 线' })) },
      { id: 'task-batch', label: '波次', options: waves.map(x => ({ value: x, label: x })) }
    ]
  });
  const cards = w.workPackages.map(pkg => {
    const unitLinks = pkg.unitIds.map(id => {
      const card = unitCardById.get(id);
      return '<a href="' + V.hrefFor('12-unit-cards', unitCardId(w, card)) + '">' + E(id) + '</a>';
    }).join(' · ');
    const stageLinks = (pkg.stageIds.length ? pkg.stageIds : []).map(id =>
      '<a href="' + V.hrefFor('03-roadmap', 'stage-' + id) + '">' + E(id) + '</a>').join(' · ') || '<span class="path">无直接阶段映射，按波次接力</span>';
    const body = '<p><b>第一动作</b> ' + E(pkg.nextAction) + '</p>' +
      '<p><b>前置依赖</b> ' + V.dependencyList(pkg.dependencies, 'wp-') + '</p>' +
      '<p><b>关联阶段</b> ' + stageLinks + '</p>' +
      '<p><b>具名单元任务卡</b> ' + unitLinks + '</p>';
    return V.taskCard({
      id: 'wp-' + pkg.id,
      title: pkg.id + ' · ' + pkg.title,
      state: pkg.planState,
      chipsHtml: V.chips([
        { state: V.statusState(pkg.planState), label: planStateLabel(m, pkg.planState) },
        { state: 'idle', label: pkg.group + ' 线 / 波次 ' + pkg.wave }
      ]),
      meta: [
        { label: '接力顺序', value: String(pkg.orderIndex) },
        { label: '台账状态', value: pkg.status },
        { label: '单元', value: pkg.unitIds.length + ' 个' },
        { label: '字段缺口', value: String(pkg.fieldGaps.length) }
      ],
      bodyHtml: body,
      rowAttrs: cardAttrs({ name: pkg.id + ' ' + pkg.title, state: pkg.planState, category: pkg.group, batch: 'wave-' + pkg.wave, package: pkg.id }) +
        ' data-work-package="' + E(pkg.id) + '"',
      sections: [
        { label: '入口链与待答问题', items: [pkg.entry, pkg.question], open: true },
        { label: '取证动作', items: [pkg.actions].concat(m.cardTemplates.unit.checklist) },
        { label: '完成标准与台账状态', items: [pkg.done].concat(Object.entries(pkg.evidenceStatus).map(([k, v]) => k + '：' + clip(v, 220))) },
        { label: '原厂证据路径', items: pkg.evidencePaths.slice(0, 10).map(x => clip(x, 180)) },
        { label: '当前实现位置', items: pkg.implementationPaths.slice(0, 8).map(x => clip(x, 180)) },
        { label: '台账字段缺口（unknown）', items: pkg.fieldGaps.length ? pkg.fieldGaps.map(x => x + '：台账未登记，需补证，不以合理设计填空') : ['无'] }
      ]
    });
  }).join('');
  p.sec('wp-cards', '02', '工作包任务卡',
    filters + '<div class="task-grid">' + cards + '</div>');

  p.sec('wp-map', '03', '包-阶段-优先级映射',
    V.table({
      caption: '阶段与优先级映射由 layers[].units 与 priority.layers 派生，不由页面推断',
      columns: [
        { label: '包', cell: r => V.code(r.id) },
        { label: '标题', cell: r => E(r.title) },
        { label: '线 / 波次', cell: r => V.mono(r.group + ' / ' + r.wave) },
        { label: '架构层', cell: r => r.layerIds.length ? V.ul(r.layerIds.map(E)) : '<span class="path">—</span>' },
        { label: '阶段', cell: r => r.stageIds.length ? V.ul(r.stageIds.map(E)) : '<span class="path">按波次接力</span>' },
        { label: '具名单元', cell: r => V.mono(r.unitIds.join(' / ')) },
        { label: '依赖', cell: r => V.mono(r.dependencies.join('、') || '无') }
      ], rows: w.workPackages,
      rowId: r => 'wpmap-' + r.id
    }));

  p.sec('wp-links', '04', '与其他视图互链',
    V.ulRaw([
      '<a href="' + V.hrefFor('00-master-plan') + '">权威总纲</a>',
      '<a href="' + V.hrefFor('15-target-source-tree') + '">自研目标架构源码树</a>',
      '<a href="' + V.hrefFor('12-unit-cards') + '">框架单元任务卡</a>',
      '<a href="' + V.hrefFor('03-roadmap') + '">阶段路线图</a>',
      '<a href="' + V.hrefFor('09-gates-evidence') + '">门禁与证据规则</a>'
    ]));

  const tree = m.targetTree;
  p.sec('wp-tree', '05', '自研目标架构源码树（入口）',
    V.notice(m.pageNotes.targetTreeNote, 'cyan') +
    V.cards([
      { num: tree.counts.nodes, label: '源码树节点', hint: tree.counts.projectNodes + ' 工程 + ' + tree.counts.directoryNodes + ' 目录' },
      { num: tree.counts.exists, label: '已存在', hint: 'current-source', tone: 'done' },
      { num: tree.counts.planned, label: '待建', hint: '只登记不落盘' },
      { num: tree.counts.unknown, label: '未知 / 未归层', hint: tree.counts.unassignedProjects + ' 个工程未归层', tone: 'unknown' }
    ]) +
    '<p><a href="' + V.hrefFor('15-target-source-tree') + '">打开完整目标源码树 →</a></p>' +
    V.table({
      caption: '8 层目标节点统计（工作包与阶段关联见完整树页）',
      columns: [
        { label: '层', cell: r => V.code(r.id) },
        { label: '目标', cell: r => E(r.target) },
        { label: '当前工程', cell: r => V.mono(r.currentProjects.join('、') || '—') },
        { label: '已存在', sort: 'num', cls: 'num-cell', cell: r => E(String(r.counts.exists)) },
        { label: '待建', sort: 'num', cls: 'num-cell', cell: r => E(String(r.counts.planned)) },
        { label: '未知', sort: 'num', cls: 'num-cell', cell: r => E(String(r.counts.unknown)) }
      ], rows: tree.layers
    }) +
    V.notice(m.pageNotes.plannedBoundary, 'unknown'));
  return p.finish();
}

// ---------------------------------------------------------------- 页面 14 · 96 框架单元任务卡
function renderUnitCards(m) {
  const w = m.wbs;
  const p = V.createPage(m, meta(m, '12-unit-cards'));
  const sample = w.unitCards[0];
  const packages = uniq(w.unitCards.map(card => card.package));
  const stateCount = uniq(w.unitCards.map(card => card.planState)).length;
  p.intro(V.cards([
    { num: w.counts.unitCards, label: '框架单元任务卡', hint: '32 包 × 3 单元', tone: 'amber' },
    { num: packages.length, label: '工作包', hint: '按包折叠分组' },
    { num: stateCount, label: '计划状态种类', hint: '由三层状态派生' },
    { num: m.progress.framework.current.done, label: '实施 done', hint: '来自逐项台账（42/96）' },
    { num: m.progress.framework.legacy.done, label: '历史汇总 done', hint: '41/96，冲突并列登记', tone: 'unknown' }
  ]));

  p.sec('uc-template', '01', '任务卡通用模板（全部单元共用）',
    V.notice(m.pageNotes.cardsAreActions, 'unknown') +
    '<div class="two-col">' +
    '<div class="panel"><h3>必填字段</h3>' + V.ul(w.cardTemplates.unit.requiredFields.map(E)) + '</div>' +
    '<div class="panel"><h3>退出判据</h3>' + V.ul(sample.exitCriteria.map(E)) + '</div>' +
    '<div class="panel"><h3>产出</h3>' + V.ul(sample.deliverable.map(E)) + '</div>' +
    '<div class="panel"><h3>操作级验证</h3>' + V.ul(sample.operationCheck.map(E)) + '</div>' +
    '</div>' +
    V.table({
      caption: '取证清单模板（每张卡按单元专属字段执行）',
      columns: [
        { label: '#', sort: 'num', cls: 'num-cell', cell: (r, i) => E(String(i + 1).padStart(2, '0')) },
        { label: '动作', cell: r => E(r) }
      ], rows: w.cardTemplates.unit.checklist
    }) +
    V.notice(sample.unknownRule, 'unknown'));

  const filters = V.filtersBar({
    total: w.counts.unitCards,
    selects: [
      { id: 'task-state', label: '计划状态', options: m.taskStateMachine.states.map(s => ({ value: s.id, label: s.label })) },
      { id: 'task-package', label: '工作包', options: packages.map(x => ({ value: x, label: x })) },
      { id: 'task-batch', label: '工作线', options: uniq(packages.map(x => x[0])).sort().map(x => ({ value: x, label: x + ' 线' })) }
    ]
  });
  const blocks = packages.map((pkg, i) => {
    const list = w.unitCards.filter(card => card.package === pkg);
    const cards = list.map(card => V.taskCard({
      id: unitCardId(w, card),
      title: card.id + ' · ' + clip(card.criterion, 60),
      state: card.planState,
      chipsHtml: V.chips([
        { state: V.statusState(card.planState), label: planStateLabel(m, card.planState) },
        { state: 'idle', label: '顺序 ' + card.orderIndex }
      ]),
      meta: [
        { label: '源码', value: card.states.source },
        { label: '实施', value: card.states.implementation },
        { label: '验证', value: card.states.validation }
      ],
      bodyHtml: '<p><b>取证目标</b> ' + E(card.objective) + '</p>' +
        '<p><b>适用模板</b> <a href="#uc-template">单元通用模板</a>（取证清单、产出、操作级验证、退出判据全文）</p>' +
        '<p><b>工作包任务卡</b> <a href="' + V.hrefFor('11-work-packages', 'wp-' + card.package) + '">' + E(card.package + ' · ' + card.packageTitle) + '</a></p>',
      rowAttrs: cardAttrs({ name: card.id + ' ' + card.criterion, state: card.planState, package: card.package, batch: card.package[0] }) +
        ' data-unit-card-id="' + E(card.id) + '"',
      sections: [
        { label: '原厂证据路径', items: card.sourcePaths.slice(0, 6).map(x => clip(x, 170)) },
        { label: '当前实现位置', items: card.implementationPaths.slice(0, 5).map(x => clip(x, 170)) },
        { label: '单元专属动作', items: card.actions.slice(-2).map(x => clip(x, 220)) },
        { label: '未知项与阻塞', items: card.unknowns.map(x => clip(x, 220)).concat(card.blockers.length ? ['阻塞 ID：' + card.blockers.join('、')] : []) }
      ]
    })).join('');
    return V.details({ id: 'ucpkg-' + (i + 1), open: true, title: pkg + ' · ' + clip(list[0].packageTitle, 60) + '（' + list.length + ' 张卡）', body: '<div class="task-grid">' + cards + '</div>' });
  }).join('');
  p.sec('uc-cards', '02', '框架单元任务卡', filters + blocks);

  p.sec('uc-links', '03', '证据规则与回链',
    V.ulRaw([
      '<a href="' + V.hrefFor('00-master-plan') + '">权威总纲</a>',
      '<a href="' + V.hrefFor('09-gates-evidence', 'evidence-4') + '">证据四态</a>',
      '<a href="' + V.hrefFor('09-gates-evidence', 'units-96') + '">96 单元三层状态台账</a>',
      '<a href="' + V.hrefFor('11-work-packages') + '">WBS 工作包</a>'
    ]));
  return p.finish();
}

// ---------------------------------------------------------------- 页面 15 · 162 工具任务卡
function renderToolCards(m) {
  const w = m.wbs;
  const p = V.createPage(m, meta(m, '13-tool-cards'));
  const sample = w.toolCards[0];
  const batches = uniq(w.toolCards.map(card => card.batch)).sort();
  p.intro(V.cards([
    { num: w.counts.toolCards, label: '目录工具任务卡', hint: '全量，不抽样', tone: 'amber' },
    { num: m.categories.length, label: '原厂分类', hint: '按 category 筛选' },
    { num: batches.length, label: '优先级批次', hint: batches.join(' / ') },
    { num: w.toolCards.filter(c => c.planState === 'done').length, label: '已闭计划状态', hint: '按批次台账 progress 派生' },
    { num: w.toolCards.filter(c => c.evidenceState === 'independent-implementation').length, label: '独立实现证据态', hint: '不得计入原厂行为' },
    { num: w.batchIdentityGaps.completed.length, label: '批次身份差异', hint: '原样登记，不猜测映射', tone: 'unknown' }
  ]));

  p.sec('tc-template', '01', '工具任务卡通用模板 T1（全部工具共用）',
    V.notice(m.pageNotes.cardsAreActions, 'unknown') +
    '<div class="two-col">' +
    '<div class="panel"><h3>必填字段</h3>' + V.ul(w.cardTemplates.tool.requiredFields.map(E)) + '</div>' +
    '<div class="panel"><h3>退出判据</h3>' + V.ul(sample.exitCriteria.map(x => clip(x, 260))) + '</div>' +
    '<div class="panel"><h3>责任线</h3>' + V.ul(sample.ownerLine.map(E)) + '</div>' +
    '<div class="panel"><h3>证据卡字段</h3>' + V.ul(sample.evidenceFields.map(E)) + '</div>' +
    '</div>' +
    V.table({
      caption: '取证清单模板 T1（每张卡按工具专属字段执行）',
      columns: [
        { label: '#', sort: 'num', cls: 'num-cell', cell: (r, i) => E(String(i + 1).padStart(2, '0')) },
        { label: '动作', cell: r => E(r) }
      ], rows: w.cardTemplates.tool.checklist
    }) +
    V.notice(sample.unknownRule, 'unknown'));

  const filters = V.filtersBar({
    total: w.counts.toolCards,
    selects: [
      { id: 'task-category', label: '原厂分类', options: m.categories.map(g => ({ value: g.name, label: g.name + ' (' + g.tools.length + ')' })) },
      { id: 'task-batch', label: '批次', options: batches.map(b => ({ value: b, label: b + ' · ' + (m.batchRules.find(r => r.batch === b) || {}).label })) },
      { id: 'task-state', label: '计划状态', options: m.taskStateMachine.states.map(s => ({ value: s.id, label: s.label })) }
    ]
  });
  const cards = w.toolCards.map(card => V.taskCard({
    id: toolCardId(w, card),
    title: card.displayName + ' · ' + card.key,
    state: card.planState,
    chipsHtml: V.chips([
      { state: V.statusState(card.planState), label: planStateLabel(m, card.planState) },
      { state: 'idle', label: card.batch + ' · ' + card.batchLabel }
    ]),
    meta: [
      { label: '分类', value: card.category },
      { label: '证据四态', value: card.evidenceState },
      { label: '进度', value: card.progress },
      { label: 'ModuleId', value: card.moduleId === null ? '未登记' : String(card.moduleId) }
    ],
    bodyHtml: '<p class="path">' + E(clip(card.sourcePath, 160)) + '</p>' +
      '<p><b>第一动作</b> ' + E(clip(card.firstAction, 150)) + '</p>' +
      '<p><b>取证清单 / 退出判据 / 责任线 / 证据卡字段</b> 适用 <a href="#tc-template">通用模板 T1</a>，按本工具专属字段执行。</p>',
    rowAttrs: cardAttrs({ name: card.displayName + ' ' + card.key, state: card.planState, category: card.category, batch: card.batch, owner: card.owner }) +
      ' data-tool-card-key="' + E(card.key) + '"',
    sections: [
      { label: '身份与证据线索', items: card.identity.map(x => clip(x, 70)).concat(card.evidenceSources.slice(0, 1).map(x => clip(x, 70))) },
      { label: '工具专属取证线索', items: card.checklist.slice(w.cardTemplates.tool.checklist.length).slice(0, 3).map(x => clip(x, 70)) },
      { label: '未知项（未闭合边界）', items: card.unknowns.slice(0, 2).map(x => clip(x, 70)) }
    ],
    footerHtml: '<p><a href="' + V.hrefFor('08-tools-162', idxId('tool', m.tools, m.tools.find(t => t.key === card.key))) + '">查看目录台账行 →</a></p>'
  })).join('');
  p.sec('tc-cards', '02', '目录工具任务卡', filters + '<div class="task-grid">' + cards + '</div>');

  p.sec('tc-links', '03', '身份差异与互链',
    V.notice('批次台账中未在目录身份里匹配到的项：completed = ' + E(w.batchIdentityGaps.completed.join('、') || '无') +
      '；active = ' + E(w.batchIdentityGaps.active.join('、') || '无') + '。' + E(w.batchIdentityGaps.policy), 'unknown') +
    V.ulRaw([
      '<a href="' + V.hrefFor('08-tools-162') + '">目录工具台账（筛选表）</a>',
      '<a href="' + V.hrefFor('00-master-plan') + '">权威总纲</a>',
      '<a href="' + V.hrefFor('09-gates-evidence', 'gate-G6') + '">G6 整工具计分门禁</a>',
      '<a href="../architecture-recovery/tool-plugin-map.html">插件工具对照证据页（原页）</a>'
    ]));
  return p.finish();
}

// ---------------------------------------------------------------- 页面 16 · E0–E7 入口闭合链
function renderEntryChains(m) {
  const w = m.wbs, counts = m.entryCounts;
  const p = V.createPage(m, meta(m, '14-entry-chains'));
  p.intro(V.cards([
    { num: counts.menuNamedDeclarations, label: '菜单命名声明', hint: '资源声明清点' },
    { num: counts.toolbarDeclarations, label: '工具栏声明', hint: '资源声明清点' },
    { num: counts.hiddenNamedDeclarations, label: '隐藏命名声明', hint: '另列，不并入主菜单' },
    { num: F(counts.sourceFiles), label: '源文件扫描', hint: '候选来源' },
    { num: F(counts.candidates), label: '入口候选', hint: '词法 + 结构化补充' },
    { num: counts.closedEntryChains, label: '已闭合入口链', hint: '当前台账值', tone: 'unknown' }
  ]));

  p.sec('ec-define', '01', '六段闭合链定义',
    V.notice(m.pageNotes.chainUnknown, 'unknown') +
    V.table({
      caption: '入口 → 绑定/命令 → 状态读写 → 事件/回调 → 持久化或执行效果 → 异常/清理',
      columns: [
        { label: '段', cell: r => V.code(r.id) },
        { label: '名称', cell: r => '<b>' + E(r.name) + '</b>' },
        { label: '取证目标', cell: r => E(r.objective) },
        { label: '证据要求', cell: r => E(r.evidence) },
        { label: '退出判据', cell: r => E(r.exitCriteria) }
      ], rows: m.closureChainStages,
      rowId: r => 'segment-' + r.id
    }));

  const gates = w.entryChains.map(chain => {
    const nodes = chain.nodes.map(node => Object.assign({}, node, { id: chainNodeId(chain.gateId, node.stageId) }));
    return '<div class="panel" id="gate-' + E(chain.gateId) + '"><h3>' + E(chain.gateId) + ' · ' + E(chain.title) + '</h3>' +
      '<p>台账状态 ' + V.statusChip(chain.gateState, chain.gateState) + ' · 缺口：' + E(chain.gap) + '</p>' +
      V.chainGrid(nodes) + '</div>';
  }).join('');
  p.sec('ec-gates', '02', 'E0–E7 门禁闭合链（' + w.counts.chainNodes + ' 个链节点）',
    '<p class="lede">点击任一链节点可高亮同一段，便于跨门禁对照；键盘 Enter / 空格同样可用。</p>' + gates);

  p.sec('ec-summary', '03', '未闭合项汇总与回链',
    V.table({
      caption: '入口完整性门禁状态与缺口（来自 entry-inventory.json）',
      columns: [
        { label: '门禁', cell: r => V.code(r.gateId) },
        { label: '标题', cell: r => E(r.title) },
        { label: '状态', cell: r => V.statusChip(r.gateState, r.gateState) },
        { label: '未知节点', sort: 'num', cls: 'num-cell', cell: r => E(String(r.unknownCount)) },
        { label: '缺口', cell: r => E(r.gap) }
      ], rows: w.entryChains
    }) +
    V.ulRaw([
      '<a href="' + V.hrefFor('09-gates-evidence', 'gates-e') + '">E0–E7 门禁原始规则</a>',
      '<a href="' + V.hrefFor('00-master-plan') + '">权威总纲</a>',
      '<a href="' + V.hrefFor('03-roadmap', 'stage-S7') + '">S7 外壳入口逐载体闭合</a>'
    ]));
  return p.finish();
}

// ---------------------------------------------------------------- 页面 17 · 自研目标架构源码树
function targetTreeGroups(model) {
  const tree = model.targetTree;
  const groups = tree.layers.map(layer => ({
    layer,
    nodes: tree.nodes.filter(node => node.layerIds[0] === layer.id)
  }));
  const unassigned = tree.nodes.filter(node => node.layerIds[0] === 'L0-unassigned');
  if (unassigned.length) {
    groups.push({
      layer: {
        id: 'L0-unassigned',
        name: '未归层工程（归层待补证）',
        target: '当前自研工程未被 8 层映射表收录；归层待补证，不臆造归属',
        status: 'unknown',
        gap: '归层证据不足'
      },
      nodes: unassigned
    });
  }
  return groups;
}
function renderTargetTree(m) {
  const tree = m.targetTree;
  const p = V.createPage(m, meta(m, '15-target-source-tree'));
  p.intro(
    V.notice(m.pageNotes.targetTreeNote, 'cyan') +
    V.notice(m.pageNotes.plannedBoundary, 'unknown') +
    V.cards([
      { num: tree.counts.layers, label: '架构层', hint: '来自 architecture-baseline.json', tone: 'amber' },
      { num: tree.counts.nodes, label: '源码树节点', hint: tree.counts.projectNodes + ' 工程 + ' + tree.counts.directoryNodes + ' 目录' },
      { num: tree.counts.exists, label: '已存在', hint: 'current-source，磁盘实证', tone: 'done' },
      { num: tree.counts.planned, label: '待建', hint: 'independent-implementation-planned' },
      { num: tree.counts.unknown, label: '未知 / 未归层', hint: tree.counts.unassignedProjects + ' 个工程未归层', tone: 'unknown' },
      { num: tree.counts.edges, label: '依赖边', hint: 'ProjectReference' }
    ]));

  const legend = [
    { state: 'exists', label: '已存在', rule: tree.policy.statusLegend.exists, evidence: tree.policy.evidenceTypes['current-source'] },
    { state: 'planned', label: '待建（计划编排）', rule: tree.policy.statusLegend.planned, evidence: tree.policy.evidenceTypes['independent-implementation-planned'] },
    { state: 'unknown', label: '未知', rule: tree.policy.statusLegend.unknown, evidence: tree.policy.evidenceTypes.unknown }
  ];
  const diagrams = m.diagrams;
  const fileTree = m.fileTree;
  const toolLayer = m.toolLayer;
  const originalTree = m.originalTree;
  p.sec('tree-diagrams', '00', '原厂目标源码树 / 自研现状源码树',
    '<p class="lede"><b>原厂目标</b>：' + originalTree.counts.assemblies +
    ' 个唯一程序集（' + originalTree.counts.treePasses + ' 棵反编译树按 canonical 归并、' +
    originalTree.counts.duplicatePasses + ' 个重复 pass 不累加）与 ' + toolLayer.counts.tools +
    ' 个 Module(sp) 工具目录树，节点来自 <code>analysis/decompiled</code> 与 inventory 台账证据（含 SHA-256）。' +
    '<b>自研现状</b>：本地 <code>src/</code> 下 ' + fileTree.counts.frameworkProjects +
    ' 个框架工程的真实文件树。两者不等价，数字只来自机器台账；长清单按目录截断并显式标注剩余数量。</p>' +
    V.cards([
      { num: originalTree.counts.assemblies, label: '原厂唯一程序集', hint: originalTree.counts.treePasses + ' 反编译树 / ' + originalTree.counts.duplicatePasses + ' 重复 pass 归并', tone: 'amber' },
      { num: m.metrics.original.cs, label: '原厂去重 .cs', hint: F(m.metrics.original.lines) + ' 行 / ' + F(m.metrics.original.types) + ' 类型', tone: 'cyan' },
      { num: fileTree.counts.frameworkProjects, label: '自研框架层工程', hint: '+ src 根文件 / ' + fileTree.counts.files + ' 代码文件', tone: 'done' },
      { num: toolLayer.counts.tools, label: '原厂工具目录', hint: toolLayer.counts.categories + ' 分类 / 全部待逆向实现', tone: 'amber' },
      { num: toolLayer.counts.evidenceFiles, label: '工具目录证据文件', hint: 'XML / DLL / 资源，树中渲染 ' + toolLayer.counts.renderedEvidenceFiles + ' 个' },
      { num: toolLayer.counts.notEstablished, label: '未建立', hint: 'native-equivalent-plugin-not-established', tone: 'unknown' }
    ]) +
    '<h3>原厂目标：' + originalTree.counts.assemblies + ' 个唯一程序集的反编译源码树</h3>' +
    OT.renderOriginalTreeControls(originalTree) + originalTree.html +
    '<h3>自研现状：' + E(fileTree.groups[0].label) + '</h3>' +
    FT.renderFileTreeControls(fileTree) + fileTree.groups[0].html +
    '<h3>原厂目标：' + toolLayer.counts.tools + ' 个 Module(sp) 工具目录 / 资源树</h3>' +
    TL.renderToolLayerControls(toolLayer) + toolLayer.html +
    V.notice('用途说明口径：' + m.purposeMap.purposeBoundary, 'cyan') +
    V.noticeHtml('口径：原厂树是反编译静态证据（文件存在性），本地树是当前磁盘现状；' +
      '工具目录树节点来自 <code>docs/plugins/inventory.json</code> 的带哈希证据路径，非本机实时扫描。' +
      originalTree.counts.sourceFilesRendered + ' / ' + originalTree.counts.sourceFilesSeen +
      ' 个源码文件在树中展开，其余按目录截断并标注，完整计数见 <a href="' + V.hrefFor('01-scope') + '">口径与源码量</a>。', 'cyan') +
    V.noticeHtml('工具层：162 个目录工具全部为逆向实现目标；其中 ' + toolLayer.counts.notEstablished +
      ' 个未建立、' + toolLayer.counts.partial + ' 个 partial，不按自研工程映射、不声称等价。逐工具取证动作见 ' +
      '<a href="' + V.hrefFor('13-tool-cards') + '">目录工具任务卡</a>。', 'unknown'));

  p.sec('tree-policy', '01', '口径、证据四态与边界',
    V.listPairs([
      ['粒度', V.code(tree.policy.granularity)],
      ['待建不落盘', V.code(String(tree.policy.noDiskCreation))],
      ['排除目录', V.ul(tree.policy.excludedDirectories.map(E))],
      ['边界', E(tree.policy.boundary)]
    ]) +
    V.table({
      caption: '状态与证据类型图例（待建属于独立编排，不是原厂结论）',
      columns: [
        { label: '状态', cell: r => V.chip(r.state === 'exists' ? 'done' : r.state === 'planned' ? 'partial' : 'unknown', r.label) },
        { label: '判定规则', cell: r => E(r.rule) },
        { label: '证据类型', cell: r => V.code(r.evidence) }
      ], rows: legend
    }) +
    V.notice(m.pageNotes.treeEvidence, 'cyan'));

  const pendingNodes = tree.nodes.filter(node => node.status !== 'exists');
  const plannedCount = pendingNodes.filter(node => node.status === 'planned').length;
  const unknownCount = pendingNodes.filter(node => node.status === 'unknown').length;
  p.sec('tree-groups', '02', '待建 / 未知目标目录登记',
    V.notice('以下 ' + pendingNodes.length + ' 个目录是计划编排的目标态（待建 ' + plannedCount +
      ' / 未知 ' + unknownCount + '），磁盘上不存在；不混入真实文件树，只在此登记，也不创建实际目录。', 'unknown') +
    V.table({
      caption: '待建与未知目录（来源于 target-source-tree.map.json，挂到工作包与阶段）',
      columns: [
        { label: '状态', cell: r => V.chip(r.status === 'planned' ? 'partial' : 'unknown', r.status) },
        { label: '路径', cell: r => V.code(r.path) },
        { label: '层', cell: r => V.mono(r.layerIds.join('、')) },
        { label: '依据 / 未知项', cell: r => E(r.rationale) },
        { label: '工作包', cell: r => V.mono(r.workPackageIds.join('、') || '—') },
        { label: '阶段', cell: r => V.mono(r.stageIds.join('、') || '—') }
      ], rows: pendingNodes,
      rowId: r => 'pending-' + V.slugId('id', r.path)
    }));

  p.sec('tree-deps', '03', '工程依赖图与依赖边',
    '<p class="lede">节点内为「行 / 类型」，按规模分档着色；悬停或键盘聚焦节点会高亮其关联边。' +
    tree.edges.length + ' 条边全部来自 architecture-baseline.json → currentEdges。</p>' +
    diagrams.dependency.html + D.diagramLegend(m, diagrams) +
    V.table({
      caption: 'ProjectReference 依赖边（来自 architecture-baseline.json → currentEdges）',
      columns: [
        { label: 'From', cell: r => V.code(r.from) },
        { label: 'To', cell: r => V.code(r.to) },
        { label: '引用文件数', sort: 'num', cls: 'num-cell', cell: r => E(String(r.files)) }
      ], rows: tree.edges
    }));

  p.sec('tree-links', '04', '与其他视图互链',
    V.ulRaw([
      '<a href="' + V.hrefFor('11-work-packages') + '">WBS 工作包（派工入口）</a>',
      '<a href="' + V.hrefFor('02-architecture') + '">原厂 / 当前架构对照</a>',
      '<a href="' + V.hrefFor('00-master-plan') + '">权威总纲</a>',
      '<a href="' + V.hrefFor('09-gates-evidence') + '">门禁与证据规则</a>'
    ]));
  return p.finish();
}

// ---------------------------------------------------------------- 页面 18 · 主界面控件落地清单
function renderUiControlLanding(m) {
  const landing = m.uiLanding;
  const p = V.createPage(m, meta(m, '16-ui-control-landing'));
  const prerequisite = landing.p0Prerequisites[0];
  const gateById = new Map(m.entryGates.map(gate => [gate.id, gate]));
  const packageById = new Map(m.wbs.workPackages.map(pkg => [pkg.id, pkg]));

  p.intro(
    V.notice(landing.evidenceBoundary, 'unknown') +
    V.notice('执行顺序：先闭 S0/C03 补证（P0 前置）→ 四类主界面共享控件完整逆向 → 工具层按 162 目录工具分批落地。未取得区域对象/像素缓冲/native 结果数组前不改产品算法、不批量实施。', 'cyan') +
    V.cards([
      { num: landing.counts.controls, label: '主界面控件', hint: landing.counts.groups + ' 类，逐项可派工', tone: 'amber' },
      { num: landing.counts.gates, label: 'E0–E7 门', hint: '全部关联，当前 closedEntryChains=0', tone: 'unknown' },
      { num: landing.counts.p0Prerequisites, label: 'P0 前置阻塞', hint: prerequisite.id, tone: 'unknown' },
      { num: landing.counts.blocked, label: '阻塞控件', hint: 'S0 前置未闭不批量实施', tone: 'unknown' },
      { num: landing.counts.unknownSegments, label: '六段链 unknown', hint: landing.counts.controls + ' × 6 段，均未取证', tone: 'cyan' },
      { num: landing.counts.acceptanceDone, label: '已闭验收', hint: '3 级验收零预填', tone: 'done' }
    ]));

  p.sec('ui-p0', '01', 'P0 前置阻塞：S0/C03 补证',
    V.notice(prerequisite.releaseCondition, 'unknown') +
    V.table({
      caption: '开放差分与解除条件（不得抹平历史记录）',
      columns: [
        { label: '项', cell: r => '<b>' + E(r.k) + '</b>' },
        { label: '值', cell: r => E(r.v) }
      ],
      rows: [
        { k: '阻塞 ID', v: prerequisite.id },
        { k: '状态', v: prerequisite.status + ' / ' + prerequisite.s0GateStatus },
        { k: '原厂 VM', v: 'BlobCount ' + prerequisite.differential.originalVm.blobCount + ' / TotalArea ' + F(prerequisite.differential.originalVm.totalArea) },
        { k: '直接 SDK / 本地', v: 'BlobCount ' + prerequisite.differential.directSdkAndLocal.blobCount + ' / TotalArea ' + F(prerequisite.differential.directSdkAndLocal.totalArea) },
        { k: '差分', v: '+' + prerequisite.differential.delta.blobCount + ' 计数 / +' + F(prerequisite.differential.delta.totalArea) + ' 面积' },
        { k: 'unknown', v: prerequisite.unknowns + ' 项' },
        { k: '产品改动', v: String(prerequisite.productChanges) },
        { k: 'REA provider', v: prerequisite.reaProvider }
      ]
    }) +
    V.ulRaw(prerequisite.evidencePaths.map(item => V.pathHtml(item))) +
    V.ulRaw([
      '<a href="' + V.hrefFor('04-vertical-slice', 'slice-phase1') + '">第一阶段落地状态</a>',
      '<a href="' + V.hrefFor('09-gates-evidence') + '">门禁与证据规则</a>'
    ]));

  p.sec('ui-acceptance', '02', '三级验收标准（每项控件逐一登记）',
    V.table({
      caption: '源码级闭环 + 原厂同状态差分 + 自研操作级复跑',
      columns: [
        { label: '级别', cell: r => V.code(r.id) },
        { label: '名称', cell: r => '<b>' + E(r.label) + '</b>' },
        { label: '判定规则', cell: r => E(r.rule) }
      ],
      rows: landing.acceptanceLevels,
      rowId: r => 'acceptance-' + r.id
    }) +
    V.notice('截图中的原厂界面只作差异信号与控件范围识别，不作实现规格；测试通过不能推出原厂 parity。人工主机验收单列，不阻塞也不计入完成度。', 'unknown'));

  p.sec('ui-gates', '03', 'E0–E7 门映射',
    V.table({
      caption: '入口完整性门禁与关联控件数（closedEntryChains=0）',
      columns: [
        { label: '门', cell: r => V.code(r.id) },
        { label: '名称', cell: r => E(r.title) },
        { label: '状态', cell: r => V.chip(r.state === 'partial' ? 'partial' : 'unknown', r.state) },
        { label: '关联控件', sort: 'num', cls: 'num-cell', cell: r => E(String(r.controls.length)) },
        { label: '缺口', cell: r => E(r.gap) }
      ],
      rows: m.entryGates.map(gate => Object.assign({}, gate, {
        controls: landing.controls.filter(control => control.gateIds.includes(gate.id))
      }))
    }));

  const filters = V.filtersBar({
    total: landing.counts.controls,
    selects: [
      { id: 'task-state', label: '计划状态', options: m.taskStateMachine.states.map(state => ({ value: state.id, label: state.label })) },
      { id: 'task-category', label: '控件类', options: landing.groups.map(group => ({ value: group.id, label: group.name + ' (' + group.controls.length + ')' })) },
      { id: 'task-batch', label: 'E 门', options: m.entryGates.map(gate => ({ value: gate.id, label: gate.id + ' · ' + gate.title })) }
    ]
  });
  const blocks = landing.groups.map((group, i) => {
    const cards = group.controls.map(control => {
      const segments = Object.values(control.sixSegmentStates)
        .map(segment => segment.id + ' ' + segment.name + '：' + segment.state);
      const acceptance = control.acceptance
        .map(item => item.label + '：' + item.state + ' — ' + item.rule);
      const workPackageLinks = control.workPackageIds.map(id =>
        '<a href="' + V.hrefFor('11-work-packages', 'wp-' + id) + '">' + E(id) + '</a>').join(' · ');
      const gateLinks = control.gateIds.map(id =>
        '<a href="' + V.hrefFor('14-entry-chains', 'gate-' + id) + '">' + E(id) + '</a>').join(' · ');
      return V.taskCard({
        id: 'ui-' + control.id,
        title: control.id + ' · ' + control.name,
        state: control.planState,
        chipsHtml: V.chips([
          { state: V.statusState(control.planState), label: planStateLabel(m, control.planState) },
          { state: 'idle', label: group.name }
        ]),
        meta: [
          { label: '工作包', value: control.workPackageIds.join('、') },
          { label: '门禁', value: control.gateIds.join('、') },
          { label: '验收', value: '0 / 3' },
          { label: '六段链', value: '0 / 6' }
        ],
        bodyHtml: '<p><b>第一动作</b> ' + E(control.firstAction) + '</p>' +
          '<p><b>截图/证据提示</b> ' + E(control.evidenceHint) + '</p>' +
          '<p><b>关联工作包</b> ' + workPackageLinks + ' · <b>E 门</b> ' + gateLinks + '</p>',
        rowAttrs: [
          'data-task-name="' + E(control.id + ' ' + control.name) + '"',
          'data-task-category="' + E(control.group) + '"',
          'data-task-batch="' + E(control.gateIds.join(',')) + '"',
          'data-ui-control="' + E(control.id) + '"'
        ].join(' '),
        sections: [
          { label: '原厂证据路径', items: control.evidencePaths.map(item => clip(item, 150)) },
          { label: '准确类型 / 成员', items: control.sourceMembers.length ? control.sourceMembers : ['台账未登记（unknown）：需源码级取证后填入'] },
          { label: '六段链状态', items: segments },
          { label: '三级验收', items: acceptance },
          { label: '未知项', items: control.unknowns },
          { label: '退出判据', items: control.exitCriteria }
        ]
      });
    }).join('');
    return V.details({
      id: 'uigroup-' + group.id,
      open: i === 0,
      title: group.name + '（' + group.controls.length + ' 项） · ' + group.description,
      body: '<div class="task-grid">' + cards + '</div>'
    });
  }).join('');
  p.sec('ui-cards', '04', '四类控件任务卡（17 项，全部阻塞）',
    filters + blocks);

  p.sec('ui-links', '05', '与既有视图互链',
    V.ulRaw([
      '<a href="' + V.hrefFor('11-work-packages') + '">WBS 工作包（32 包）</a>',
      '<a href="' + V.hrefFor('12-unit-cards') + '">框架单元任务卡（96 张）</a>',
      '<a href="' + V.hrefFor('14-entry-chains') + '">E0–E7 入口闭合链</a>',
      '<a href="' + V.hrefFor('15-target-source-tree') + '">原厂目标 / 自研现状源码树</a>',
      '<a href="' + V.hrefFor('13-tool-cards') + '">162 工具任务卡（后续批次）</a>',
      '<a href="' + V.hrefFor('00-master-plan') + '">权威总纲</a>'
    ]) +
    V.notice('本页不改动既有 WBS 的 32 工作包 / 96 单元卡 / 162 工具卡 / 48 链节点口径；控件状态是「完整逆向」这一新验收标准的状态，S0 前置未闭时一律 blocked。', 'cyan'));
  return p.finish();
}

// ---------------------------------------------------------------- 页面 19 · 渐进落地路线与任务列表
function renderIncrementalLanding(m) {
  const inc = m.incremental;
  const p = V.createPage(m, meta(m, '17-incremental-landing'));
  const b0 = inc.batches[0];
  p.intro(
    V.notice(inc.evidenceBoundary, 'unknown') +
    V.notice('执行顺序：B0 S0/C03 补证 → B1 最小纵切 ImageSource→Blob→显示 → B2 主界面 17 控件横扩 → B3 162 工具分批。前置未闭不得进入后续批次的实施类动作；本页所有命令只登记待执行，不自动跑方案测试、不改产品算法。', 'cyan') +
    V.cards([
      { num: inc.counts.batches, label: '渐进批次', hint: 'B0 → B3', tone: 'amber' },
      { num: inc.counts.steps, label: '可派工步骤', hint: '逐项写明文件/成员/命令/报告', tone: 'cyan' },
      { num: inc.counts.stepsWithBlockers, label: '前置阻塞步骤', hint: 'B0 未闭不批量实施', tone: 'unknown' },
      { num: inc.counts.commandEntries, label: '登记命令', hint: '仅 C01 复跑，待执行' },
      { num: inc.counts.evidencePaths, label: '证据路径', hint: '全部磁盘可复核', tone: 'done' },
      { num: inc.counts.done, label: '已完成步骤', hint: '零预填', tone: 'done' }
    ]));

  p.sec('inc-timeline', '01', '批次时间线与阻塞关系',
    V.timeline(inc.batches.map(batch => ({
      title: batch.id + ' · ' + batch.title,
      text: '前置 ' + (batch.prerequisites.join('、') || '无') + ' · 状态 ' + batch.status + ' · ' + batch.steps.length + ' 步'
    }))) +
    V.table({
      caption: '批次依赖与状态（B0 未闭时后续批次全部 blocked）',
      columns: [
        { label: '批次', cell: r => V.code(r.id) },
        { label: '范围', cell: r => E(r.scope) },
        { label: '前置', cell: r => V.mono(r.prerequisites.join('、') || '无') },
        { label: '状态', cell: r => V.chip(r.status === 'open' ? 'partial' : 'unknown', r.status) },
        { label: '步骤', sort: 'num', cls: 'num-cell', cell: r => E(String(r.steps.length)) }
      ], rows: inc.batches,
      rowId: r => 'inc-batch-' + r.id
    }));

  p.sec('inc-b0', '02', 'B0：S0/C03 前置补证（第一批）',
    V.notice('B0 是本轮唯一无前置的批次。' + b0.steps.length +
      ' 步为静态取证与证据卡：复核差分两侧、5 项 unknown、REA 补证边界；C01 已在当前构建产物复跑（见下方 S0 复算）。原厂运行时根因仍未取得，不改产品算法。', 'unknown') +
    V.noticeHtml('第一批已执行（仅静态取证）：证据卡 <code>' + E(m.incrementalLanding.cardId) +
      '</code> → ' + V.pathHtml(m.incrementalLanding.evidenceCard) + '；人读 ' +
      V.pathHtml(m.incrementalLanding.humanReview) + '。B0 仍 <b>open</b>、B1 仍 <b>blocked</b>；' +
      '产品改动 ' + m.incrementalLanding.productChanges.length + ' 项。', 'cyan') +
    V.noticeHtml('第二批静态取证：证据卡 <code>' + E(m.incrementalLanding.b1Static.cardId) +
      '</code> 已定位 ' + m.incrementalLanding.b1Static.sourceFiles + ' 个源码文件的准确成员，' +
      '输出名对照 9/9（ImageSource）· 4/13（Blob）· 1/2（Group），' +
      m.incrementalLanding.b1Static.solutionFiles + ' 份 vmx 已锁定 SHA-256。<b>B1 仍 blocked</b>。', 'cyan') +
    V.noticeHtml('第三批契约闭合：证据卡 <code>' + E(m.incrementalLanding.b1Static.b1Contracts.cardId) +
      '</code> 定位 9 个 <code>Components.*</code> 由 <code>CompoundOutputFields.cs</code> 动态投影生成，' +
      'Group <code>image/width</code> 来自方案 <code>Configuration.Outputs</code>；输出名 13/13 与 2/2 均可解释。' +
      '<b>managed projection，非原厂 ABI</b>；C03 根因仍开放、B1 仍 blocked。', 'cyan') +
    V.notice('本批补跑：11 个回归 exitCode=0；2 个需原厂外部数据（设备采集 BMP / native scalar oracle）不可执行，不记为通过、不伪造。规格台账：`ORIGINAL-MEMBERS-INDEX.md`（25 程序集 / 2614 .cs）与 `TOOL-IMPLEMENTATION-SPECS.md`（162 工具 / 2015 证据文件），已挂入 544 项清单。', 'done') +
    V.noticeHtml('S0 复跑（用户授权「开始测试/验证」）：<code>dotnet run --project tests/VMVerticalSlice.C01/… --no-restore</code> ' +
      '<b>exitCode=0、18 PASS</b>；用 <code>D:\\MyWork\\图片库\\标定\\1.jpg</code> 同输入复跑得 47/973202，' +
      '报告 SHA-256 与历史逐字节相同；12 张标定样本全部 sessionStatus=0、13 个输出名、重开计数一致。' +
      '证据卡 <code>C03-S0-RERUN-MATRIX-20261110</code>。<b>S0 本地切片闭环达成，C03 原厂根因仍开放</b>；' +
      '样本 3 计数 45 与原厂 45 相同但面积 986444 ≠ 971486，属巧合，不作差分解释。', 'done') +
    V.table({
      caption: '开放差分与解除条件（不得抹平历史记录）',
      columns: [
        { label: '项', cell: r => '<b>' + E(r.k) + '</b>' },
        { label: '值', cell: r => E(r.v) }
      ],
      rows: [
        { k: '原厂 VM', v: 'BlobCount 45 / TotalArea 971486' },
        { k: '直接 SDK / 本地', v: 'BlobCount 47 / TotalArea 973202' },
        { k: '差分', v: '+2 计数 / +1716 面积' },
        { k: 'S0', v: m.phase1Landing.s0GateStatus },
        { k: 'S0.1/S0.2', v: '当前构建产物已复跑（exitCode=0、12 样本重开一致）' },
        { k: 'unknown', v: m.phase1Landing.unknowns + ' 项' },
        { k: '产品改动', v: String(m.phase1Landing.productChanges.length) },
        { k: '解除条件', v: '仍需原厂运行时区域对象、像素缓冲与 native 结果数组以闭 C03 根因' }
      ]
    }) +
    V.ulRaw(b0.steps.map(step =>
      '<a href="#inc-step-' + step.id + '">' + E(step.id + ' · ' + step.title) + '</a>')));

  const filters = V.filtersBar({
    total: inc.counts.steps,
    selects: [
      { id: 'task-state', label: '步骤状态', options: [{ value: 'not_started', label: '未开始' }] },
      { id: 'task-category', label: '批次', options: inc.batches.map(batch => ({ value: batch.id, label: batch.id + ' · ' + batch.title })) }
    ]
  });
  const blocks = inc.batches.map((batch, i) => {
    const cards = batch.steps.map(step => {
      const members = step.sourceMembers.length
        ? step.sourceMembers.map(item => clip(item, 90))
        : ['台账未登记（unknown）：准确类型 / 成员需源码级取证后填入'];
      const commands = step.commands.length
        ? step.commands.map(item => item + ' 〔S0 复跑批次已执行：exitCode=0〕')
        : ['无复跑命令（本步为静态取证 / 计划编排）'];
      const outputs = step.expectedOutputPaths.length
        ? step.expectedOutputPaths.map(item => clip(item, 100) + ' 〔待产出〕')
        : ['无新增报告路径'];
      return V.taskCard({
        id: 'inc-step-' + step.id,
        title: step.id + ' · ' + step.title,
        state: step.state,
        chipsHtml: V.chips([
          { state: 'idle', label: batch.id },
          { state: step.blockers.length ? 'unknown' : 'partial', label: step.blockers.length ? '前置阻塞' : '无前置' }
        ]),
        meta: [
          { label: '状态', value: step.state },
          { label: '证据', value: String(step.evidencePaths.length) + ' 项' },
          { label: '命令', value: String(step.commands.length) + ' 条' },
          { label: '阻塞', value: step.blockers.join('、') || '无' }
        ],
        bodyHtml: '<p><b>批次</b> ' + E(batch.id + ' · ' + batch.title) + '</p>' +
          '<p><b>范围</b> ' + E(clip(batch.scope, 190)) + '</p>',
        rowAttrs: [
          'data-task-name="' + E(step.id + ' ' + step.title) + '"',
          'data-task-category="' + E(batch.id) + '"',
          'data-ui-incremental="' + E(step.id) + '"'
        ].join(' '),
        sections: [
          { label: '证据文件 / 路径', items: step.evidencePaths.slice(0, 5).map(item => clip(item, 95)) },
          { label: '准确类型 / 成员', items: members },
          { label: '复跑命令（待执行）', items: commands },
          { label: '报告路径 / 待产出', items: step.reportPaths.map(item => clip(item, 95)).concat(outputs) },
          { label: '退出判据', items: step.exitCriteria.map(item => clip(item, 150)) },
          { label: '未知项', items: step.unknowns.length ? step.unknowns.map(item => clip(item, 130)) : ['无'] }
        ]
      });
    }).join('');
    return V.details({
      id: 'inc-group-' + batch.id,
      open: i === 0,
      title: batch.id + ' · ' + batch.title + '（' + batch.steps.length + ' 步）',
      body: '<div class="task-grid">' + cards + '</div>'
    });
  }).join('');
  p.sec('inc-cards', '03', '全部任务卡（' + inc.counts.steps + ' 步，全部未开始）', filters + blocks);

  p.sec('inc-commands', '04', '命令与报告路径登记',
    V.table({
      caption: '只允许既有台账登记的真实命令；C01 已在用户授权的 S0 复跑批次执行',
      columns: [
        { label: '步骤', cell: r => V.code(r.id) },
        { label: '命令', cell: r => r.commands.length ? V.mono(r.commands[0]) : '<span class="path">—</span>' },
        { label: '既有报告', cell: r => V.mono(r.reportPaths.join('、') || '—') },
        { label: '待产出', cell: r => V.mono(r.expectedOutputPaths.join('、') || '—') },
        { label: '执行', cell: r => r.commands.length ? V.chip('done', '已执行 exit 0') : V.chip('unknown', '无命令') }
      ], rows: inc.steps
    }) +
    V.notice('C01 复跑由用户明确授权「开始测试/验证」执行：exitCode=0、18 PASS，同输入报告 SHA-256 与历史相同，12 张标定样本矩阵见证据卡 C03-S0-RERUN-MATRIX-20261110。后续补齐 8 个回归后共 11 个 exitCode=0；2 个需原厂外部数据不可执行，不记为通过、不伪造。', 'cyan'));

  p.sec('inc-links', '05', '与既有视图互链',
    V.ulRaw([
      '<a href="' + V.hrefFor('16-ui-control-landing') + '">主界面控件落地清单（B2 范围）</a>',
      '<a href="' + V.hrefFor('04-vertical-slice', 'slice-phase1') + '">第一阶段 S0/C03 落地状态</a>',
      '<a href="' + V.hrefFor('15-target-source-tree') + '">原厂目标 / 自研现状源码树</a>',
      '<a href="' + V.hrefFor('13-tool-cards') + '">162 工具任务卡（B3 范围）</a>',
      '<a href="' + V.hrefFor('11-work-packages') + '">WBS 工作包</a>',
      '<a href="' + V.hrefFor('00-master-plan') + '">权威总纲</a>'
    ]) +
    V.notice('本页不改动既有 WBS 32 工作包 / 96 单元卡 / 162 工具卡 / 48 链节点口径，也不改动 17 控件清单；批次状态是渐进执行顺序，不产生第二套完成度。', 'cyan') +
    V.noticeHtml('启动态工作面：<code>docs/STARTUP-INITIAL-UI-TASK-LIST.md</code> 与 ' +
      '<code>C03-STARTUP-INITIAL-UI-20261110</code>，' +
      m.startupInitialUi.counts.startupSteps + ' 启动步骤 / ' + m.startupInitialUi.counts.controls +
      ' 空载初始态控件、' + m.startupInitialUi.counts.sixSegmentUnknown +
      ' 个六段链节点全 unknown、三级验收零预填。与第 18 页「运行态」清单并列，不改产品代码、不跑差分复跑。', 'done'));
  return p.finish();
}

const BUILDERS = {
  index: renderIndex,
  '16-ui-control-landing': renderUiControlLanding,
  '17-incremental-landing': renderIncrementalLanding,
  '15-target-source-tree': renderTargetTree,
  '00-master-plan': renderMasterPlan,
  '11-work-packages': renderWorkPackages,
  '12-unit-cards': renderUnitCards,
  '13-tool-cards': renderToolCards,
  '14-entry-chains': renderEntryChains,
  '01-scope': renderScope,
  '02-architecture': renderArchitecture,
  '03-roadmap': renderRoadmap,
  '04-vertical-slice': renderSlice,
  '05-p0-mechanism': renderP0,
  '06-p1-coverage': renderP1,
  '07-p2-p3': renderP2P3,
  '08-tools-162': renderTools,
  '09-gates-evidence': renderGates,
  '10-governance': renderGovernance
};

// ---------------------------------------------------------------- 站点装配
function renderAll(model) {
  must(model.pages.length >= 11, '页面清单不应少于既有页面数');
  must(Object.keys(BUILDERS).length === model.pages.length, '生成器页面数量与编辑源页面清单不符');
  must(model.pages.every(item => BUILDERS[item.slug]), '页面清单存在未实现的生成器');
  // 图与真实文件树只装配一次，页面与 plan-data.json 摘要共用，避免两套口径。
  model.diagrams = D.buildDiagrams(model);
  model.fileTree = FT.buildFileTree(model, path.join(root, 'src'), model.purposeMap);
  model.toolLayer = TL.buildToolLayer(model);
  model.originalTree = OT.buildOriginalTree(model, root);
  const memberSpecLedger = JSON.parse(fs.readFileSync(path.join(root,
    'tests/ArchitectureRecovery/original-members.map.json'), 'utf8'));
  const toolSpecLedger = JSON.parse(fs.readFileSync(path.join(root,
    'tests/ArchitectureRecovery/tool-implementation-specs.map.json'), 'utf8'));
  must(memberSpecLedger.totals.assemblies === 25 && toolSpecLedger.counts.tools === 162,
    '规格台账摘要计数不符');
  const files = [];
  const searchIndex = [];
  const pageAnchors = {};

  for (const item of model.pages) {
    must(BUILDERS[item.slug], '缺少页面生成器: ' + item.slug);
    const page = BUILDERS[item.slug](model);
    const file = fileOf(page.slug);
    files.push({ relative: file, content: page.html });
    pageAnchors[file] = page.anchors.map(a => a.id);
    searchIndex.push({ p: file, h: '', t: page.title, k: page.kicker });
    for (const anchor of page.anchors) {
      searchIndex.push({ p: file, h: '#' + anchor.id, t: anchor.title, k: page.title });
    }
  }

  // 可检索实体：层 / 阶段 / 优先级 / 遗漏面 / 分类 / 96 单元 / 162 工具。
  for (const layer of model.layers) {
    searchIndex.push({ p: '02-architecture.html', h: '#' + idxId('layer', model.layers, layer), t: layer.name, k: layer.id + ' ' + layer.gap });
  }
  for (const stage of model.stages) {
    searchIndex.push({ p: '03-roadmap.html', h: '#stage-' + stage.id, t: stage.id + ' · ' + stage.name, k: stage.deliverable });
  }
  for (const priority of model.priorities) {
    searchIndex.push({ p: PRIORITY_PAGES[priority.rank], h: '#priority-' + priority.rank, t: priority.item, k: priority.id + ' ' + priority.firstAction });
  }
  for (const gap of model.gaps) {
    searchIndex.push({ p: '01-scope.html', h: '#' + idxId('gap', model.gaps, gap), t: gap.item, k: gap.size + ' ' + gap.risk });
  }
  model.categories.forEach((group, i) => {
    searchIndex.push({ p: '08-tools-162.html', h: '#cat-' + (i + 1), t: group.name + '（' + group.tools.length + ' 工具）', k: group.name });
  });
  for (const unit of model.frameworkUnits) {
    searchIndex.push({ p: '09-gates-evidence.html', h: '#' + idxId('unit', model.frameworkUnits, unit), t: unit.id + ' · ' + unit.criterion, k: unit.package + ' ' + unit.implementation });
  }
  for (const tool of model.tools) {
    searchIndex.push({ p: '08-tools-162.html', h: '#' + idxId('tool', model.tools, tool), t: tool.displayName + ' · ' + tool.key, k: tool.category + ' ' + tool.batch + ' ' + tool.evidenceState });
  }
  // 完整计划的可派工实体：32 工作包 / 96 单元卡 / 162 工具卡 / 48 链节点。
  const wbs = model.wbs;
  for (const pkg of wbs.workPackages) {
    searchIndex.push({ p: '11-work-packages.html', h: '#wp-' + pkg.id, t: pkg.id + ' · ' + pkg.title, k: pkg.group + ' wave-' + pkg.wave + ' ' + pkg.planState });
  }
  for (const card of wbs.unitCards) {
    searchIndex.push({ p: '12-unit-cards.html', h: '#' + unitCardId(wbs, card), t: card.id + ' · ' + card.criterion, k: card.package + ' ' + card.planState });
  }
  for (const card of wbs.toolCards) {
    searchIndex.push({ p: '13-tool-cards.html', h: '#' + toolCardId(wbs, card), t: card.displayName + ' · ' + card.key + ' 任务卡', k: card.category + ' ' + card.batch + ' ' + card.evidenceState });
  }
  for (const chain of wbs.entryChains) {
    for (const node of chain.nodes) {
      searchIndex.push({
        p: '14-entry-chains.html', h: '#' + chainNodeId(chain.gateId, node.stageId),
        t: chain.gateId + ' · ' + node.segment, k: chain.title + ' ' + node.task
      });
    }
  }
  const targetTree = model.targetTree;
  for (const project of model.fileTree.projects) {
    searchIndex.push({
      p: '15-target-source-tree.html', h: '#' + project.id,
      t: project.name + ' · 真实文件树', k: project.fileCount + ' 文件 ' + project.dirCount + ' 目录'
    });
  }
  for (const node of targetTree.nodes.filter(item => item.status !== 'exists')) {
    searchIndex.push({
      p: '15-target-source-tree.html', h: '#pending-' + V.slugId('id', node.path),
      t: node.path + ' · ' + node.status,
      k: node.rationale + ' ' + node.workPackageIds.join(' ') + ' ' + node.stageIds.join(' ')
    });
  }
  for (const control of model.uiLanding.controls) {
    searchIndex.push({
      p: '16-ui-control-landing.html', h: '#ui-' + control.id,
      t: control.id + ' · ' + control.name,
      k: control.group + ' ' + control.workPackageIds.join(' ') + ' ' + control.gateIds.join(' ') + ' ' + control.firstAction
    });
  }
  for (const group of model.uiLanding.groups) {
    searchIndex.push({
      p: '16-ui-control-landing.html', h: '#uigroup-' + group.id,
      t: group.name + '（' + group.controls.length + ' 项）', k: group.description
    });
  }
  for (const step of model.incremental.steps) {
    searchIndex.push({
      p: '17-incremental-landing.html', h: '#inc-step-' + step.id,
      t: step.id + ' · ' + step.title,
      k: step.batchId + ' ' + step.state + ' ' + step.evidencePaths.join(' ')
    });
  }
  for (const batch of model.incremental.batches) {
    searchIndex.push({
      p: '17-incremental-landing.html', h: '#inc-batch-' + batch.id,
      t: batch.id + ' · ' + batch.title, k: batch.scope + ' ' + batch.prerequisites.join(' ')
    });
  }

  const css = readSource(ASSET_SOURCES[0].from);
  const js = readSource(ASSET_SOURCES[1].from);
  must(js.includes('/*__PLAN_SEARCH_INDEX__*/'), 'plan-page.js 缺少搜索索引占位符');
  const payload = JSON.stringify(searchIndex)
    .replace(/</g, '\\u003c')
    .replace(/\u2028/g, '\\u2028')
    .replace(/\u2029/g, '\\u2029');
  const planJs = js.replace('/*__PLAN_SEARCH_INDEX__*/', 'window.PLAN_SEARCH_INDEX = ' + payload + ';');

  const assetSources = ASSET_SOURCES.map(item => {
    const content = readSource(item.from);
    return { path: item.from, output: item.to, sha256: sha(content) };
  });
  const planData = {
    schemaVersion: 1,
    builtFrom: model.builtFrom,
    inputs: model.inputs,
    supporting: model.supporting,
    assetSources,
    sourcePaths: model.sourcePaths,
    metrics: model.metrics,
    pages: model.pages.map(item => ({ slug: item.slug, file: fileOf(item.slug), tier: item.tier, title: item.title })),
    pageAnchors,
    searchIndexDigest: sha(JSON.stringify(searchIndex)),
    searchIndexEntries: searchIndex.length,
    stages: model.stages.map(s => ({
      id: s.id, name: s.name, tier: s.tier,
      priorityId: s.priority ? s.priority.id : null,
      exitCriteria: s.exitCriteria.length, forbiddenParallel: s.forbiddenParallel.length
    })),
    priorities: model.priorities.map(pr => ({ rank: pr.rank, id: pr.id, item: pr.item, layers: pr.layers, blocks: pr.blocks })),
    progress: model.progress,
    conflict: model.conflict,
    gaps: model.gaps,
    batchRules: model.batchRules,
    batchIdentityGaps: model.batchIdentityGaps,
    wbs: {
      authority: model.masterPlan.authority,
      counts: wbs.counts,
      dependencyOrder: wbs.dependencyOrder,
      legacyPlans: wbs.legacyPlans.map(item => ({ path: item.path, status: item.status, disposition: item.disposition })),
      closureChainStages: model.closureChainStages.map(item => ({ id: item.id, name: item.name })),
      planStates: model.taskStateMachine.states.map(item => item.id),
      cardRequiredFields: {
        unit: model.cardTemplates.unit.requiredFields,
        tool: model.cardTemplates.tool.requiredFields
      },
      workPackages: wbs.workPackages.map(pkg => ({
        id: pkg.id, title: pkg.title, wave: pkg.wave, planState: pkg.planState,
        dependencies: pkg.dependencies, unitIds: pkg.unitIds, stageIds: pkg.stageIds,
        fieldGaps: pkg.fieldGaps
      })),
      unitCardIds: wbs.unitCards.map(card => card.id),
      toolCardKeys: wbs.toolCards.map(card => card.key),
      entryChainGates: wbs.entryChains.map(chain => chain.gateId)
    },
    phase1Landing: {
      status: model.phase1Landing.status,
      s0GateStatus: model.phase1Landing.s0GateStatus,
      productChanges: model.phase1Landing.productChanges.length,
      unknowns: model.phase1Landing.unknowns,
      nextEvidenceActions: model.phase1Landing.nextEvidenceActions,
      diffPreserved: model.phase1Landing.diffPreserved,
      evidenceCard: model.phase1Landing.evidenceCard,
      evidenceReview: model.phase1Landing.evidenceReview,
      gateCheck: model.phase1Landing.gateCheck,
      decision: model.phase1Landing.decision
    },
    targetTree: {
      counts: targetTree.counts,
      policy: {
        granularity: targetTree.policy.granularity,
        noDiskCreation: targetTree.policy.noDiskCreation,
        excludedDirectories: targetTree.policy.excludedDirectories
      },
      layerIds: targetTree.layers.map(layer => layer.id),
      nodeIds: targetTree.nodes.map(node => node.id),
      edgeCount: targetTree.edges.length
    },
    diagrams: {
      counts: model.diagrams.counts,
      layerPairs: model.diagrams.architecture.pairs.map(pair => ({ key: pair.key, count: pair.count }))
    },
    fileTree: {
      counts: model.fileTree.counts,
      tools: model.fileTree.tools
    },
    toolLayer: {
      counts: model.toolLayer.counts
    },
    originalTree: {
      counts: model.originalTree.counts
    },
    uiLanding: {
      counts: model.uiLanding.counts,
      controlIds: model.uiLanding.controls.map(control => control.id),
      groupIds: model.uiLanding.groups.map(group => group.id),
      p0PrerequisiteIds: model.uiLanding.p0Prerequisites.map(item => item.id),
      acceptanceLevelIds: model.uiLanding.acceptanceLevels.map(item => item.id)
    },
    incremental: {
      counts: model.incremental.counts,
      batchIds: model.incremental.batches.map(batch => batch.id),
      stepIds: model.incremental.steps.map(step => step.id),
      allowedCommands: model.incremental.allowedCommands,
      batch0CardId: model.incrementalLanding.cardId,
      batch0EvidenceCard: model.incrementalLanding.evidenceCard,
      batch0HumanReview: model.incrementalLanding.humanReview,
      batch0Status: model.incrementalLanding.status,
      commandsExecuted: model.incrementalLanding.commandsExecuted,
      productChanges: model.incrementalLanding.productChanges.length,
      batch2CardId: model.incrementalLanding.b1Static.cardId,
      batch2EvidenceCard: model.incrementalLanding.b1Static.evidenceCard,
      batch2HumanReview: model.incrementalLanding.b1Static.humanReview,
      batch2SourceFiles: model.incrementalLanding.b1Static.sourceFiles,
      batch2SolutionFiles: model.incrementalLanding.b1Static.solutionFiles,
      batch2OutputComparison: model.incrementalLanding.b1Static.outputNameComparison,
      batch3CardId: model.incrementalLanding.b1Static.b1Contracts.cardId,
      batch3EvidenceCard: model.incrementalLanding.b1Static.b1Contracts.evidenceCard,
      batch3GapsClosed: model.incrementalLanding.b1Static.b1Contracts.gapsClosed
    },
    memberSpecs: {
      assemblies: memberSpecLedger.totals.assemblies,
      canonicalCsFiles: memberSpecLedger.totals.canonicalCsFiles,
      declaredTypes: memberSpecLedger.totals.declaredTypes,
      methodSignatures: memberSpecLedger.totals.methodSignatures,
      ledger: 'tests/ArchitectureRecovery/original-members.map.json',
      index: 'docs/ORIGINAL-MEMBERS-INDEX.md'
    },
    toolSpecs: {
      tools: toolSpecLedger.counts.tools,
      categories: toolSpecLedger.counts.categories,
      evidenceFiles: toolSpecLedger.counts.totalEvidenceFiles,
      ledger: 'tests/ArchitectureRecovery/tool-implementation-specs.map.json',
      index: 'docs/TOOL-IMPLEMENTATION-SPECS.md'
    },
    startupInitialUi: {
      stateScope: '空载初始态',
      steps: model.startupInitialUi.counts.startupSteps,
      controls: model.startupInitialUi.counts.controls,
      sixSegmentUnknown: model.startupInitialUi.counts.sixSegmentUnknown,
      acceptanceDone: model.startupInitialUi.counts.acceptanceDone,
      list: 'docs/STARTUP-INITIAL-UI-TASK-LIST.md',
      evidenceCard: 'analysis/full-audit/batch1-20261107/C03/c03-startup-initial-ui-20261110.json'
    },
    exceptions: [],
    counts: {
      pages: model.pages.length,
      units: model.frameworkUnits.length,
      tools: model.tools.length,
      categories: model.categories.length,
      layers: model.layers.length,
      assemblies: model.assemblies.length,
      currentProjects: model.currentProjects.length,
      stages: model.stages.length,
      evidenceStates: model.evidenceStates.length,
      acceptanceLevels: model.acceptanceLevels.length,
      stageGates: model.stageGates.length,
      entryGates: model.entryGates.length,
      inputs: model.inputs.length,
      supporting: model.supporting.length,
      workPackages: wbs.counts.workPackages,
      unitCards: wbs.counts.unitCards,
      toolCards: wbs.counts.toolCards,
      entryChains: wbs.counts.entryChains,
      chainNodes: wbs.counts.chainNodes,
      legacyPlans: wbs.counts.legacyPlans,
      planStates: wbs.counts.planStates,
      workPackageFieldGaps: wbs.counts.workPackageFieldGaps,
      targetTreeNodes: targetTree.counts.nodes,
      targetTreeExists: targetTree.counts.exists,
      targetTreePlanned: targetTree.counts.planned,
      targetTreeUnknown: targetTree.counts.unknown,
      targetTreeEdges: targetTree.counts.edges,
      targetTreeUnassignedProjects: targetTree.counts.unassignedProjects,
      uiLandingControls: model.uiLanding.counts.controls,
      uiLandingGroups: model.uiLanding.counts.groups,
      uiLandingBlocked: model.uiLanding.counts.blocked,
      uiLandingUnknownSegments: model.uiLanding.counts.unknownSegments,
      incrementalBatches: model.incremental.counts.batches,
      incrementalSteps: model.incremental.counts.steps,
      incrementalDone: model.incremental.counts.done
    }
  };
  files.push({ relative: 'plan-data.json', content: JSON.stringify(planData, null, 2) + '\n' });
  files.push({ relative: ASSET_SOURCES[0].to, content: css });
  files.push({ relative: ASSET_SOURCES[1].to, content: planJs });
  return { files, searchIndex, pageAnchors, planData };
}

function idsOf(html) {
  const ids = [];
  const re = /\sid="([^"]+)"/g;
  let match;
  while ((match = re.exec(html))) ids.push(match[1]);
  return ids;
}
function assertSite(model, site) {
  const { files, searchIndex, planData } = site;
  must(files.length === model.pages.length + 3,
    '产物应为 ' + model.pages.length + ' 页 + plan-data.json + 2 个资源，共 ' + (model.pages.length + 3) + ' 个文件');
  const generated = new Set(files.map(f => f.relative));
  const byFile = new Map(files.map(f => [f.relative, f.content]));
  const htmlFiles = files.filter(f => f.relative.endsWith('.html'));
  must(htmlFiles.length === model.pages.length, 'HTML 页面数应与编辑源页面清单一致');
  const expectedPages = model.pages.map(p => fileOf(p.slug));
  must(expectedPages.every(file => byFile.has(file)), '缺少计划页面');

  const idsByFile = {};
  for (const f of htmlFiles) {
    const ids = idsOf(f.content);
    must(new Set(ids).size === ids.length, '页面存在重复 id: ' + f.relative);
    idsByFile[f.relative] = new Set(ids);
  }

  let externalRefs = 0;
  let totalBytes = 0;
  for (const f of files) {
    const bytes = Buffer.byteLength(f.content, 'utf8');
    totalBytes += bytes;
    if (/https?:\/\//i.test(f.content)) externalRefs++;
    if (f.relative.endsWith('.html')) {
      must(bytes < PAGE_LIMIT, '单页超过 400 KB: ' + f.relative + ' (' + bytes + ')');
      must(f.content.includes('<html lang="zh-CN">'), '缺少语言标注: ' + f.relative);
      must((f.content.split('aria-current="page"').length - 1) === 1, 'aria-current 数量异常: ' + f.relative);
      const nav = /<ul class="nav-list">([\s\S]*?)<\/ul>/.exec(f.content);
      must(nav, '缺少左侧导航: ' + f.relative);
      must(expectedPages.every(file => nav[1].includes('href="' + file + '"')),
        '导航未互链全部 ' + model.pages.length + ' 页: ' + f.relative);
      const linkRe = /(?:href|src)="([^"]*)"/g;
      let link;
      while ((link = linkRe.exec(f.content))) {
        const target = link[1];
        must(target, '空链接: ' + f.relative);
        const hashAt = target.indexOf('#');
        const filePart = hashAt < 0 ? target : target.slice(0, hashAt);
        const hash = hashAt < 0 ? '' : target.slice(hashAt + 1);
        if (filePart) {
          must(generated.has(filePart) || fs.existsSync(path.resolve(out, filePart)),
            '链接目标不存在: ' + f.relative + ' → ' + target);
          if (hash) {
            const targetIds = idsByFile[filePart];
            must(targetIds, '跨页锚点目标不是 HTML 页面: ' + filePart);
            must(targetIds.has(hash), '跨页锚点不存在: ' + f.relative + ' → ' + target);
          }
        } else if (hash) {
          must(idsByFile[f.relative].has(hash), '页内锚点不存在: ' + f.relative + ' → #' + hash);
        }
      }
      const scriptRe = /<script\b[^>]*>([\s\S]*?)<\/script>/g;
      let script;
      while ((script = scriptRe.exec(f.content))) {
        const body = script[1].trim();
        if (body) {
          try { new vm.Script(body); } catch (error) { throw new Error('[plan] 内联脚本语法错误: ' + f.relative + ' :: ' + error.message); }
        }
      }
    }
  }
  must(totalBytes < SITE_LIMIT, '站点总量超过 2 MB: ' + totalBytes);
  must(externalRefs === 0, '出现 ' + externalRefs + ' 个文件含 http(s):// 外链');
  try { new vm.Script(byFile.get('assets/plan.js')); }
  catch (error) { throw new Error('[plan] assets/plan.js 语法错误: ' + error.message); }

  const toolsPage = byFile.get('08-tools-162.html');
  const toolKeys = [...toolsPage.matchAll(/data-tool-key="([^"]+)"/g)].map(x => x[1]);
  must(toolKeys.length === 162 && new Set(toolKeys).size === 162, '162 工具行数量或唯一性不符: ' + toolKeys.length);
  for (const tool of model.tools) must(toolKeys.includes(tool.key), '工具未出现在台账页: ' + tool.key);

  const gatesPage = byFile.get('09-gates-evidence.html');
  const unitIds = [...gatesPage.matchAll(/data-unit-id="([^"]+)"/g)].map(x => x[1]);
  must(unitIds.length === 96 && new Set(unitIds).size === 96, '96 单元行数量或唯一性不符: ' + unitIds.length);
  for (const unit of model.frameworkUnits) must(unitIds.includes(unit.id), '单元未出现在门禁页: ' + unit.id);

  // ---- 完整计划：WBS / 任务卡 / 闭合链覆盖与必填字段 ----
  const wbs = model.wbs;
  const wpPage = byFile.get('11-work-packages.html');
  const wpIds = [...wpPage.matchAll(/data-work-package="([^"]+)"/g)].map(x => x[1]);
  must(wpIds.length === 32 && new Set(wpIds).size === 32, 'WBS 工作包卡数量或唯一性不符: ' + wpIds.length);
  for (const pkg of wbs.workPackages) must(wpIds.includes(pkg.id), '工作包未出现在 WBS 页: ' + pkg.id);
  const unitCardPage = byFile.get('12-unit-cards.html');
  const unitCardIds = [...unitCardPage.matchAll(/data-unit-card-id="([^"]+)"/g)].map(x => x[1]);
  must(unitCardIds.length === 96 && new Set(unitCardIds).size === 96, '框架单元任务卡数量或唯一性不符: ' + unitCardIds.length);
  for (const card of wbs.unitCards) must(unitCardIds.includes(card.id), '单元任务卡缺失: ' + card.id);
  const toolCardPage = byFile.get('13-tool-cards.html');
  const toolCardKeys = [...toolCardPage.matchAll(/data-tool-card-key="([^"]+)"/g)].map(x => x[1]);
  must(toolCardKeys.length === 162 && new Set(toolCardKeys).size === 162, '目录工具任务卡数量或唯一性不符: ' + toolCardKeys.length);
  for (const card of wbs.toolCards) must(toolCardKeys.includes(card.key), '工具任务卡缺失: ' + card.key);
  const chainPage = byFile.get('14-entry-chains.html');
  const chainNodeCount = (chainPage.match(/data-chain-node/g) || []).length;
  must(chainNodeCount === 48, '闭合链节点应为 48 个，实际 ' + chainNodeCount);
  for (const chain of wbs.entryChains) {
    for (const node of chain.nodes) {
      must(idsByFile['14-entry-chains.html'].has(chainNodeId(chain.gateId, node.stageId)),
        '链节点锚点缺失: ' + chain.gateId + '/' + node.stageId);
    }
  }
  const masterPage = byFile.get('00-master-plan.html');
  must(masterPage.includes(V.esc(wbs.masterPlan.authority)), '权威总纲缺少唯一执行口径声明');
  const governancePage = byFile.get('10-governance.html');
  for (const legacy of wbs.legacyPlans) {
    must(masterPage.includes(V.esc(legacy.path)) && governancePage.includes(V.esc(legacy.path)),
      '旧计划处置登记缺失: ' + legacy.path);
  }
  must(wbs.dependencyOrder.length === 32 &&
    planData.wbs.dependencyOrder.join(',') === wbs.dependencyOrder.join(',') &&
    wbs.counts.workPackageFieldGaps === planData.counts.workPackageFieldGaps,
    'WBS 摘要与派生计数不一致');

  // ---- 目标源码树：节点覆盖、待建不落盘、工作包页入口 ----
  const targetTree = model.targetTree;
  const treePage = byFile.get('15-target-source-tree.html');
  const pendingRows = [...treePage.matchAll(/id="pending-[^"]+"/g)].length;
  must(pendingRows === targetTree.nodes.filter(node => node.status !== 'exists').length,
    '待建/未知目录表格行数不符: ' + pendingRows);
  for (const node of targetTree.nodes) {
    const onDisk = fs.existsSync(path.join(root, node.path));
    must(onDisk === (node.status === 'exists'), '树节点磁盘状态与登记不符: ' + node.path);
  }
  must(targetTree.policy.noDiskCreation === true, '目标树必须声明待建不落盘');
  must(planData.targetTree.nodeIds.length === targetTree.counts.nodes &&
    planData.targetTree.policy.noDiskCreation === true,
    'plan-data.json 目标树摘要与派生结果不一致');
  must(wpPage.includes('id="wp-tree"') && wpPage.includes(V.hrefFor('15-target-source-tree')),
    '工作包页缺少目标源码树入口章节或互链');

  // ---- 三张内联 SVG 图：数量、节点/边覆盖、无外部命名空间与图例 ----
  const diagrams = model.diagrams;
  const archPage = byFile.get('02-architecture.html');
  must(archPage.includes('<svg') && archPage.includes('data-diagram="architecture"'),
    '架构页缺少内联架构分层图');
  must(treePage.includes('data-diagram="dependency"') && !treePage.includes('data-diagram="source-tree"'),
    '源码树页应只保留依赖图，源码树已改为真实文件树');
  must((archPage.match(/<svg/g) || []).length === 1 && (treePage.match(/<svg/g) || []).length === 1,
    '两页 SVG 数量应为 1 + 1');
  must((archPage.match(/data-diagram-layer=/g) || []).length === 8, '架构图未覆盖 8 层');
  const diagramNodes = (treePage.match(/data-diagram-node=/g) || []).length;
  must(diagramNodes === diagrams.counts.dependencyNodes, '依赖图节点数应为 29，实际 ' + diagramNodes);
  const diagramEdges = (treePage.match(/data-diagram-edge/g) || []).length;
  must(diagramEdges === diagrams.counts.dependencyEdges, '依赖图边数不是 62，实际 ' + diagramEdges);
  must((archPage.match(/data-diagram-legend/g) || []).length === 1 &&
    (treePage.match(/data-diagram-legend/g) || []).length === 1, '两页图例数量不符');
  must(!/foreignObject|xmlns=/i.test(archPage + treePage), 'SVG 含外部命名空间或 foreignObject');
  must(planData.diagrams && planData.diagrams.counts.diagrams === 2 &&
    planData.diagrams.counts.dependencyEdges === diagrams.counts.dependencyEdges,
    'plan-data.json 图摘要与派生结果不一致');

  // 真实源码文件树：30 工程级条目 / 502 代码文件 / 50 目录，待建目录不混入。
  const fileTree = model.fileTree;
  must(treePage.includes('data-file-project=') && treePage.includes('data-file-count='),
    '源码树页缺少真实文件树');
  const fileProjects = (treePage.match(/data-file-project=/g) || []).length;
  must(fileProjects === fileTree.counts.projects, '文件树工程数应为 30，实际 ' + fileProjects);
  const fileSum = [...treePage.matchAll(/data-file-count="(\d+)"/g)]
    .reduce((n, match) => n + Number(match[1]), 0);
  must(fileSum === fileTree.counts.files, '文件树文件数应为 502，实际 ' + fileSum);
  const dirSum = [...treePage.matchAll(/data-file-dir-count="(\d+)"/g)]
    .reduce((n, match) => n + Number(match[1]), 0);
  must(dirSum === fileTree.counts.dirs, '文件树目录数应为 50，实际 ' + dirSum);
  must(treePage.includes('├── ') && treePage.includes('└── '), '文件树缺少 tree 树线字符');
  must(planData.fileTree && planData.fileTree.counts.files === fileTree.counts.files &&
    planData.fileTree.counts.projects === 16 &&
    planData.fileTree.counts.frameworkProjects === 15 &&
    planData.fileTree.counts.toolProjects === 0,
    'plan-data.json 文件树摘要与派生结果不一致');

  // 框架层 15 工程 + 根文件的 summary 行必须有非空设计用途。
  const purposeCount = treePage.split('data-file-purpose="').length - 1;
  must(purposeCount === 16 && !treePage.includes('data-file-purpose=""'),
    '框架层工程用途说明应为 16 条（15 工程 + 根文件）且非空，实际 ' + purposeCount);
  const purposeSpan = treePage.split('class="file-purpose"').length - 1;
  must(purposeSpan === 16, '用途说明未出现在 16 个 summary 行，实际 ' + purposeSpan);
  const frameworkCount = treePage.split('data-file-kind="framework"').length - 1;
  const toolTreeCount = treePage.split('data-file-kind="tool"').length - 1;
  must(frameworkCount === 16 && toolTreeCount === 0,
    '框架层应为 16（含根文件）、工具层不应再有自研工程树，实际 ' + frameworkCount + '/' + toolTreeCount);

  // 工具层：162 个待逆向实现工具、14 分类、161 未建立 + 1 partial。
  const toolLayerKeys = [...treePage.matchAll(/data-tool-layer-key="([^"]+)"/g)].map(match => match[1]);
  must(toolLayerKeys.length === 162 && new Set(toolLayerKeys).size === 162,
    '工具层清单应为 162 个唯一工具，实际 ' + toolLayerKeys.length);
  must((treePage.match(/data-tool-layer-category=/g) || []).length === 162,
    '每个工具行必须带分类属性');
  const notEstablished = treePage.split('data-tool-layer-state="not-established"').length - 1;
  const partialCount = treePage.split('data-tool-layer-state="partial"').length - 1;
  must(notEstablished === 161 && partialCount === 1,
    '工具状态应为 161 未建立 + 1 partial，实际 ' + notEstablished + '+' + partialCount);
  must(treePage.includes('13-tool-cards.html#toolcard-'), '工具行缺少任务卡链接');
  must(planData.toolLayer && planData.toolLayer.counts.tools === 162 &&
    planData.toolLayer.counts.categories === 14 &&
    planData.toolLayer.counts.notEstablished === 161 &&
    planData.toolLayer.counts.partial === 1,
    'plan-data.json 工具层摘要不符');

  // ---- 原厂目标源码树：25 程序集 / 28 反编译树归并 / 162 工具目录证据 ----
  const originalTree = model.originalTree;
  const originalIds = [...treePage.matchAll(/data-original-assembly="([^"]+)"/g)].map(match => match[1]);
  must(originalIds.length === 25 && new Set(originalIds).size === 25,
    '原厂程序集树应为 25 个唯一节点，实际 ' + originalIds.length);
  must((treePage.match(/data-original-tree=/g) || []).length === 25, '原厂 canonical 树属性应为 25 个');
  must((treePage.match(/class="original-tree"/g) || []).length === 25, '原厂树线块应为 25 个');
  must((treePage.match(/data-tool-layer-sha=/g) || []).length === 162,
    '162 个工具目录节点必须带 SHA-256 标识');
  must(treePage.includes('原厂目标') && treePage.includes('自研现状'), '页面缺少双口径标题');
  must(treePage.includes('├── ') && treePage.includes('└── '), '页面缺少真实树线字符');
  must(planData.originalTree && planData.originalTree.counts.assemblies === 25 &&
    planData.originalTree.counts.treePasses === 28 &&
    planData.originalTree.counts.duplicatePasses === 3 &&
    planData.originalTree.counts.uniqueCs === model.metrics.original.cs &&
    planData.originalTree.counts.uniqueLines === model.metrics.original.lines,
    'plan-data.json 原厂树摘要与台账不符');
  must(originalTree.counts.sourceFilesRendered <= originalTree.counts.sourceFilesSeen &&
    treePage.includes('其余'), '原厂树截断未显式标注或渲染数超过扫描数');

  // ---- 主界面控件落地清单：17 控件 / 4 类 / 8 门 / S0 前置 / 三级验收 ----
  const landing = model.uiLanding;
  const landingPage = byFile.get('16-ui-control-landing.html');
  must(landingPage, '缺少主界面控件落地清单页');
  const controlIds = [...landingPage.matchAll(/data-ui-control="([^"]+)"/g)].map(match => match[1]);
  must(controlIds.length === 17 && new Set(controlIds).size === 17,
    '控件任务卡应为 17 个且唯一，实际 ' + controlIds.length);
  must((landingPage.match(/id="uigroup-/g) || []).length === 4, '控件分组应为 4 个');
  for (const id of ['ui-p0', 'ui-acceptance', 'ui-gates', 'ui-cards', 'ui-links']) {
    must(landingPage.includes('id="' + id + '"'), '落地清单缺少章节: ' + id);
  }
  must(landingPage.includes('45') && landingPage.includes('971486') &&
    landingPage.includes('47') && landingPage.includes('973202'),
    '落地清单未并列开放差分两侧数值');
  must(landingPage.includes('unavailable-on-current-host'), '落地清单未登记 REA provider 不可用');
  must((landingPage.match(/id="acceptance-/g) || []).length === 3, '三级验收应为 3 行');
  must((landingPage.split('data-task-state="blocked"').length - 1) === 17,
    'S0 前置未闭时 17 个控件应全部 blocked');
  must(!landingPage.includes('sourceClosure：done') && !landingPage.includes('originalDifferential：done'),
    '不得预填验收完成状态');
  must(landingPage.includes('E0') && landingPage.includes('E7'), '落地清单应覆盖 E0–E7');
  must(planData.uiLanding && planData.uiLanding.counts.controls === 17 &&
    planData.uiLanding.counts.groups === 4 && planData.uiLanding.counts.gates === 8 &&
    planData.uiLanding.counts.blocked === 17 && planData.uiLanding.counts.acceptanceDone === 0 &&
    planData.uiLanding.controlIds.length === 17,
    'plan-data.json 控件落地摘要不符');
  must(planData.counts.uiLandingControls === 17 && planData.counts.pages === model.pages.length,
    'plan-data.json 页数或控件分母不符');
  must(landing.counts.unknownSegments === 102, '六段链 unknown 段数应为 102');

  // ---- 渐进落地路线：19 页 / 4 批次 / 13 步 / 命令只登记待执行 ----
  const incremental = model.incremental;
  const incPage = byFile.get('17-incremental-landing.html');
  must(incPage, '缺少渐进落地路线页');
  const incIds = [...incPage.matchAll(/data-ui-incremental="([^"]+)"/g)].map(match => match[1]);
  must(incIds.length === 13 && new Set(incIds).size === 13,
    '渐进步骤卡应为 13 个且唯一，实际 ' + incIds.length);
  must((incPage.match(/id="inc-batch-/g) || []).length === 4, '批次锚点应为 4 个');
  must((incPage.match(/id="inc-step-/g) || []).length === 13, '步骤锚点应为 13 个');
  for (const id of ['inc-timeline', 'inc-b0', 'inc-cards', 'inc-commands', 'inc-links']) {
    must(incPage.includes('id="' + id + '"'), '渐进路线页缺少章节: ' + id);
  }
  must(incPage.includes('971486') && incPage.includes('973202'), '渐进路线页未并列开放差分');
  must(incPage.includes(incremental.allowedCommands[0]) && incPage.includes('待执行'),
    '渐进路线页未登记真实复跑命令或缺少待执行标注');
  must((incPage.split('data-task-state="not_started"').length - 1) === 13,
    '13 个步骤应全部为 not_started');
  must(planData.incremental && planData.incremental.counts.batches === 4 &&
    planData.incremental.counts.steps === 13 && planData.incremental.counts.done === 0 &&
    planData.incremental.stepIds.length === 13 &&
    planData.incremental.allowedCommands[0] === incremental.allowedCommands[0],
    'plan-data.json 渐进路线摘要不符');
  must(planData.counts.incrementalBatches === 4 && planData.counts.incrementalSteps === 13 &&
    planData.counts.pages === 19, 'plan-data.json 页数或渐进分母不符');
  must(incPage.includes('C03-INCREMENTAL-B0B1-001') && incPage.includes('第一批已执行'),
    '渐进路线页未回写第一批静态取证结果');
  must(planData.incremental.batch0CardId === 'C03-INCREMENTAL-B0B1-001' &&
    planData.incremental.commandsExecuted === 1 && planData.incremental.productChanges === 0 &&
    planData.incremental.batch0EvidenceCard === model.incrementalLanding.evidenceCard,
    'plan-data.json 第一批摘要不符');

  for (const item of searchIndex) {
    must(byFile.has(item.p), '搜索索引指向不存在的页面: ' + item.p);
    if (item.h) must(idsByFile[item.p].has(item.h.slice(1)), '搜索索引锚点不存在: ' + item.p + item.h);
  }
  must(planData.counts.units === 96 && planData.counts.tools === 162 &&
    planData.counts.pages === model.pages.length && planData.counts.workPackages === 32 &&
    planData.counts.unitCards === 96 && planData.counts.toolCards === 162 &&
    planData.counts.chainNodes === 48, 'plan-data.json 分母不符');
  must(planData.exceptions.length === 0, '本计划尚未登记阶段并行例外');
  must(planData.incremental.batch3CardId === 'C03-INCREMENTAL-B1-CONTRACTS-001' &&
    planData.incremental.batch3GapsClosed.length === 2 &&
    fs.existsSync(path.join(root, planData.incremental.batch3EvidenceCard)),
    'plan-data.json 第三批契约摘要不符');
  must(planData.memberSpecs.assemblies === 25 &&
    planData.memberSpecs.canonicalCsFiles === 2614 &&
    planData.toolSpecs.tools === 162 && planData.toolSpecs.evidenceFiles === 2015 &&
    fs.existsSync(path.join(root, planData.memberSpecs.ledger)) &&
    fs.existsSync(path.join(root, planData.toolSpecs.ledger)),
    'plan-data.json 规格台账摘要或路径不符');
  const inc19 = byFile.get('17-incremental-landing.html');
  must(inc19 && inc19.includes('C03-INCREMENTAL-B1-CONTRACTS-001') &&
    inc19.includes('ORIGINAL-MEMBERS-INDEX.md') && inc19.includes('TOOL-IMPLEMENTATION-SPECS.md'),
    '第 19 页未回写第三批契约与规格索引');

  // ---- 启动态工作面：台账、三份交付、零预填、19 页不变 ----
  const su = model.startupInitialUi;
  must(su && su.counts.startupSteps === 11 && su.counts.controls === 10 &&
    su.counts.sixSegmentUnknown === 126 && su.counts.acceptanceDone === 0 &&
    su.counts.gateCoverage === 8 && su.counts.layoutEdgesTotal === 30,
    '启动态派生计数不符');
  for (const relative of [
    'tests/ArchitectureRecovery/startup-initial-ui.map.json',
    'docs/STARTUP-INITIAL-UI-TASK-LIST.md',
    'docs/reverse/C03-startup-initial-ui-evidence.md',
    'analysis/full-audit/batch1-20261107/C03/c03-startup-initial-ui-20261110.json'
  ]) {
    must(fs.existsSync(path.join(root, relative)), '缺少启动态交付: ' + relative);
  }
  must(inc19.includes('C03-STARTUP-INITIAL-UI-20261110') &&
    inc19.includes('STARTUP-INITIAL-UI-TASK-LIST.md'),
    '第 19 页缺少启动态 notice');
  must(planData.startupInitialUi && planData.startupInitialUi.steps === 11 &&
    planData.startupInitialUi.controls === 10 &&
    planData.startupInitialUi.acceptanceDone === 0 &&
    planData.startupInitialUi.stateScope === '空载初始态',
    'plan-data.json 启动态摘要不符');
  must(planData.counts.pages === 19 && planData.supporting.length === 8,
    '启动态工作面不得改变页数或补充源数量');
  verifyInputs(model);

  // 确定性：同一模型重跑一次，逐字节比对。
  const again = renderAll(model);
  must(again.files.length === files.length, '重跑产物数量不一致');
  for (let i = 0; i < files.length; i++) {
    must(again.files[i].relative === files[i].relative, '重跑产物顺序不一致');
    must(again.files[i].content === files[i].content, '重跑产物字节不一致: ' + files[i].relative);
  }
  return {
    pages: htmlFiles.length,
    files: files.length,
    bytes: totalBytes,
    anchors: Object.values(site.pageAnchors).reduce((n, list) => n + list.length, 0),
    unitsCovered: unitIds.length,
    toolsCovered: toolKeys.length,
    workPackages: wpIds.length,
    unitCards: unitCardIds.length,
    toolCards: toolCardKeys.length,
    chainNodes: chainNodeCount,
    targetTreeNodes: targetTree.counts.nodes,
    diagrams: diagrams.counts.diagrams,
    diagramNodes,
    diagramEdges,
    originalAssemblies: model.originalTree.counts.assemblies,
    originalTreePasses: model.originalTree.counts.treePasses,
    toolDirShas: model.toolLayer.counts.tools,
    uiLandingControls: model.uiLanding.counts.controls,
    uiLandingGroups: model.uiLanding.counts.groups,
    incrementalBatches: model.incremental.counts.batches,
    incrementalSteps: model.incremental.counts.steps,
    fileTreeProjects: fileProjects,
    fileTreeFiles: fileSum,
    fileTreeDirs: dirSum,
    searchIndexEntries: searchIndex.length,
    externalRefs,
    inputsStable: true
  };
}

function main() {
  const model = buildModel();
  verifyInputs(model);
  const site = renderAll(model);
  const result = assertSite(model, site);
  verifyInputs(model);
  fs.mkdirSync(path.join(out, 'assets'), { recursive: true });
  for (const f of site.files) {
    fs.mkdirSync(path.dirname(path.join(out, f.relative)), { recursive: true });
    fs.writeFileSync(path.join(out, f.relative), f.content, 'utf8');
  }
  result.builtFrom = model.builtFrom.head.slice(0, 8) + '/' + model.builtFrom.inputsDigest.slice(0, 12);
  console.log(JSON.stringify(result, null, 2));
  return result;
}

if (require.main === module) main();
module.exports = { renderAll, assertSite, idsOf, fileOf, PAGE_LIMIT, SITE_LIMIT, main };
