'use strict';
// 逆向计划站点的唯一数据装配层。页面只消费本模块产出的 planModel，不自行读取台账、不写死统计数字。
const fs = require('fs');
const path = require('path');
const crypto = require('crypto');
const cp = require('child_process');
const { buildWbs } = require('./plan-wbs.cjs');
const { buildTargetTree } = require('./plan-target-tree.cjs');
const { buildUiLanding } = require('./plan-ui-landing.cjs');
const { buildIncremental } = require('./plan-incremental.cjs');
const { buildStartupUi } = require('./plan-startup-initial-ui.cjs');

const root = path.resolve(__dirname, '../..');
const out = path.join(root, 'docs/reverse-plan');

// 9 个机器台账：页面中的所有统计数字、状态与既有结论均来自这里。
const INPUTS = [
  'analysis/full-audit/plugin-coverage.json',
  'docs/architecture-baseline.json',
  'docs/framework-reverse-plan.json',
  'docs/framework-progress-audit.json',
  'docs/NEXT-BATCH-PLUGIN-PROGRESS.json',
  'docs/SOURCE-LEVEL-REVERSE-PROGRESS.json',
  'docs/VM-LOCAL-RESTORATION-STATUS.json',
  'docs/plugins/inventory.json',
  'docs/architecture-recovery/entry-inventory.json'
];

// 补充源：切片运行证据、基线人读表、计划编辑性文本、目标树映射、工程用途、控件落地台账。均参与哈希锁定。
const SUPPORTING = [
  'analysis/full-audit/batch1-20261107/C01/runtime-report.json',
  'docs/ARCHITECTURE-BASELINE.md',
  'tests/ArchitectureRecovery/reverse-plan-editorial.json',
  'tests/ArchitectureRecovery/target-source-tree.map.json',
  'tests/ArchitectureRecovery/project-purpose.map.json',
  'tests/ArchitectureRecovery/ui-control-landing.map.json',
  'tests/ArchitectureRecovery/incremental-landing.map.json',
  'tests/ArchitectureRecovery/startup-initial-ui.map.json'
];

function must(condition, message) {
  if (!condition) throw new Error('[plan-model] ' + message);
}
function absolute(relative) {
  return path.join(root, relative);
}
function readBytes(relative) {
  const file = absolute(relative);
  must(fs.existsSync(file), '缺少输入文件: ' + relative);
  return fs.readFileSync(file);
}
function readText(relative) {
  return readBytes(relative).toString('utf8').replace(/^\uFEFF/, '');
}
function readJson(relative) {
  try {
    return JSON.parse(readText(relative));
  } catch (error) {
    throw new Error('[plan-model] JSON 解析失败: ' + relative + ' :: ' + error.message);
  }
}
function sha256Of(relative) {
  return crypto.createHash('sha256').update(readBytes(relative)).digest('hex');
}
function describe(relative) {
  const bytes = readBytes(relative);
  return {
    path: relative,
    sha256: crypto.createHash('sha256').update(bytes).digest('hex'),
    bytes: bytes.length
  };
}
function sum(items, key) {
  return items.reduce((total, item) => total + (Number(item[key]) || 0), 0);
}
function round1(value) {
  return Math.round(value * 10) / 10;
}
function unique(items) {
  return [...new Set(items)];
}

// 8 项遗漏面直接取自已冻结的人读基线第 7 节表格；表格变化时构建必须失败，不允许页面自行改写。
function parseGapTable(markdown) {
  const section = markdown.split(/^## 7\./m)[1];
  must(section, 'ARCHITECTURE-BASELINE.md 缺少第 7 节遗漏风险登记');
  const body = section.split(/^## 8\./m)[0];
  const rows = body.split(/\r?\n/).filter(line => /^\|/.test(line) && !/^\|\s*[-:|]+\s*\|/.test(line));
  must(rows.length >= 2, '第 7 节表格无法解析');
  const cells = row => row.split('|').slice(1, -1).map(cell => cell.trim().replace(/`/g, ''));
  const header = cells(rows[0]);
  must(header.length === 4, '第 7 节应为四列表格，实际 ' + header.length + ' 列');
  const gaps = rows.slice(1).map(row => {
    const values = cells(row);
    must(values.length === 4, '遗漏面表格行不完整: ' + row);
    return { item: values[0], size: values[1], status: values[2], risk: values[3] };
  });
  must(gaps.length === 8, '遗漏面应为 8 项，实际 ' + gaps.length + ' 项');
  return gaps;
}

function buildModel() {
  must(INPUTS.length === 9, '机器台账输入应为 9 个，实际 ' + INPUTS.length);
  must(SUPPORTING.length === 8, '补充源应为 8 个，实际 ' + SUPPORTING.length);

  const inputs = INPUTS.map(describe);
  const supporting = SUPPORTING.map(describe);
  const head = cp.execFileSync('git', ['rev-parse', 'HEAD'], { cwd: root, encoding: 'utf8' }).trim();
  const inputsDigest = crypto.createHash('sha256')
    .update(inputs.concat(supporting).map(item => item.path + ':' + item.sha256).join('\n'))
    .digest('hex');

  const coverage = readJson(INPUTS[0]);
  const baseline = readJson(INPUTS[1]);
  const reversePlan = readJson(INPUTS[2]);
  const audit = readJson(INPUTS[3]);
  const nextBatch = readJson(INPUTS[4]);
  const sourceLevel = readJson(INPUTS[5]);
  const vmStatus = readJson(INPUTS[6]);
  const inventory = readJson(INPUTS[7]);
  const entryInventory = readJson(INPUTS[8]);
  const sliceRuntime = readJson(SUPPORTING[0]);
  const baselineDoc = readText(SUPPORTING[1]);
  const editorial = readJson(SUPPORTING[2]);
  const targetTreeMap = readJson(SUPPORTING[3]);
  const purposeMap = readJson(SUPPORTING[4]);
  const uiLandingMap = readJson(SUPPORTING[5]);
  const incrementalMap = readJson(SUPPORTING[6]);
  const startupUiMap = readJson(SUPPORTING[7]);

  // ---- 分母与内部计数一致性（计数不符一律中止，不产出降级页面） ----
  must(baseline.totals.original.assemblies === 25, '原厂唯一程序集应为 25');
  must(baseline.assemblies.length === 25, '程序集清单应为 25 项');
  must(baseline.layers.length === 8, '架构层应为 8 层');
  must(baseline.currentProjects.length === 29, '当前工程应为 29 个');
  must(baseline.priorities.length === 9, '优先级应为 9 项');
  must(baseline.layers.reduce((n, layer) => n + layer.assemblies.length, 0) === 25, '8 层程序集归并未覆盖 25 个唯一程序集');
  must(sum(baseline.assemblies, 'lines') === baseline.totals.original.uniqueLines, '程序集行数之和与总量不符');
  must(sum(baseline.assemblies, 'types') === baseline.totals.original.uniqueTypes, '程序集类型数之和与总量不符');
  must(sum(baseline.assemblies, 'methods') === baseline.totals.original.uniqueMethods, '程序集方法数之和与总量不符');
  must(sum(baseline.layers.map(layer => layer.original), 'lines') === baseline.totals.original.uniqueLines, '8 层行数与总量不符');
  must(sum(baseline.currentProjects, 'Lines') === baseline.totals.current.lines, '当前工程行数与总量不符');
  must(sum(baseline.currentProjects, 'Types') === baseline.totals.current.types, '当前工程类型数与总量不符');
  must(sum(baseline.currentProjects, 'CsFiles') === baseline.totals.current.cs, '当前工程 .cs 数与总量不符');

  must(audit.packages.length === 32, '框架包应为 32 个');
  must(audit.summary.units === 96, '框架单元分母应为 96');
  const frameworkUnits = audit.packages.flatMap(pkg => pkg.units.map(unit => ({
    id: unit.id,
    criterion: unit.criterion,
    source: unit.source,
    implementation: unit.implementation,
    validation: unit.validation,
    limitation: unit.limitation,
    blockerIds: unit.blockerIds || [],
    package: pkg.taskId,
    packageTitle: pkg.title,
    domains: pkg.domains || [],
    implementationArtifacts: pkg.implementationArtifacts || [],
    sourceAndOperationReports: pkg.sourceAndOperationReports || []
  })));
  must(frameworkUnits.length === 96, '展开后的具名单元应为 96 个，实际 ' + frameworkUnits.length);
  must(unique(frameworkUnits.map(unit => unit.id)).length === 96, '具名单元 ID 存在重复');
  const implementationTally = frameworkUnits.reduce((tally, unit) => {
    tally[unit.implementation] = (tally[unit.implementation] || 0) + 1;
    return tally;
  }, {});
  must(implementationTally.done === audit.summary.layers.implementation.done, '逐单元实施 done 计数与汇总字段不符');

  must(coverage.summary.plugin_count === 162, '原厂插件目录应为 162 个');
  must(coverage.plugins.length === 162, '覆盖审计插件清单应为 162 项');
  must(inventory.plugins.length === 162, '插件身份清单应为 162 项');
  const categoryCounts = coverage.summary.plugin_categories || {};
  const categories = Object.keys(categoryCounts).sort();
  must(categories.length === 14, '原厂分类应为 14 类，实际 ' + categories.length);
  must(sum(Object.values(categoryCounts).map(count => ({ count })), 'count') === 162, '分类计数之和应为 162');
  must(inventory.summary.plugin_count === 162, '两份 162 台账的汇总数不一致');
  const coverageKeys = new Set(coverage.plugins.map(plugin => plugin.category + '/' + plugin.plugin));
  must(coverageKeys.size === 162, '覆盖审计插件身份存在重复');
  for (const plugin of inventory.plugins) {
    must(coverageKeys.has(plugin.category + '/' + plugin.plugin),
      '插件身份未在覆盖审计中出现: ' + plugin.category + '/' + plugin.plugin);
  }

  must(nextBatch.totalCount === 11 && nextBatch.completedTools.length === nextBatch.completedCount, '11 工具批次计数不一致');
  must(sourceLevel.totalTools === 10 && sourceLevel.toolCompletion.length === sourceLevel.completedTools, '标定 10 工具计数不一致');
  // 页数由编辑源驱动；只锁「既有页面 URL 不得消失」，新增页无需改动生成器断言。
  const pageSlugs = editorial.pages.map(page => page.slug);
  const stableSlugs = [
    'index', '01-scope', '02-architecture', '03-roadmap', '04-vertical-slice',
    '05-p0-mechanism', '06-p1-coverage', '07-p2-p3', '08-tools-162',
    '09-gates-evidence', '10-governance'
  ];
  must(pageSlugs.length >= stableSlugs.length && new Set(pageSlugs).size === pageSlugs.length,
    '页面 slug 应唯一且不少于既有页面数');
  must(stableSlugs.every(slug => pageSlugs.includes(slug)), '既有页面 URL 稳定性被破坏');
  must(editorial.pages.every(page => page.slug && page.title && page.kicker && page.tier && page.description),
    '页面元数据字段不完整');
  must(editorial.masterPlan && editorial.taskStateMachine && editorial.legacyPlans.length === 4,
    '编辑源缺少总纲、状态机或 4 份旧计划处置');
  must(editorial.closureChainStages.length === 6, '闭合链六段应为 6 项');
  must(editorial.cardTemplates.unit.requiredFields.length === 6 &&
    editorial.cardTemplates.tool.requiredFields.length === 6, '任务卡必填模板字段应为 6 项');
  must(targetTreeMap.schemaVersion === 1 && targetTreeMap.layers.length === 8 &&
    targetTreeMap.policy && targetTreeMap.policy.noDiskCreation === true,
    '目标源码树映射台账结构不符');
  must(purposeMap.schemaVersion === 1 && purposeMap.projects.length === 29 &&
    purposeMap.projects.every(item => item.name && item.purpose && item.purpose.trim().length > 0 &&
      (item.kind === 'framework' || item.kind === 'tool')),
    '工程用途映射应为 29 条且字段完整');
  must(purposeMap.projects.filter(item => item.kind === 'framework').length === 15 &&
    purposeMap.projects.filter(item => item.kind === 'tool').length === 14,
    '工程用途映射的框架层/工具层分组应为 15/14');
  must(purposeMap.tools && purposeMap.tools.total === 162 &&
    purposeMap.tools.notEstablished === 161 && purposeMap.tools.partialIndependent === 1,
    '162 工具建立情况统计不符');
  must(uiLandingMap.schemaVersion === 1 && uiLandingMap.controls.length === 17 &&
    uiLandingMap.groups.length === 4 && uiLandingMap.acceptanceLevels.length === 3 &&
    uiLandingMap.p0Prerequisites.length === 1 &&
    uiLandingMap.p0Prerequisites[0].status === 'open' &&
    uiLandingMap.p0Prerequisites[0].productChanges === 0,
    '主界面控件落地台账结构或 S0 前置状态不符');
  must(incrementalMap.schemaVersion === 1 && incrementalMap.batches.length === 4 &&
    incrementalMap.batches.reduce((n, batch) => n + batch.steps.length, 0) === 13 &&
    incrementalMap.batches[0].id === 'B0' && incrementalMap.batches[0].status === 'open',
    '渐进落地台账应为 4 批次 13 步且 B0 开放');
  must(startupUiMap.schemaVersion === 1 && startupUiMap.startupSteps.length === 11 &&
    startupUiMap.controls.length === 10 && startupUiMap.groups.length === 4 &&
    startupUiMap.acceptanceLevels.every(item => item.state === 'not_started') &&
    startupUiMap.stateScope.indexOf('空载初始态') === 0,
    '启动态台账结构或状态面不符');
  must(editorial.stages.length === 10, 'S0–S9 阶段应为 10 项');
  must(editorial.architectureVerdict.differences.length === 3, '架构机制差异应为 3 项');
  must(editorial.pageNotes && ['p0Intro', 'p1Intro', 'p23Intro', 'threeProgress', 'sliceFirst'].every(k => editorial.pageNotes[k]),
    '编辑源缺少页面说明');
  const layerIds = new Set(baseline.layers.map(layer => layer.id));
  for (const diff of editorial.architectureVerdict.differences) {
    must(layerIds.has(diff.layerId), '机制差异引用的层不存在: ' + diff.layerId);
  }
  must(editorial.stages.map(stage => stage.id).join(',') === 'S0,S1,S2,S3,S4,S5,S6,S7,S8,S9', '阶段 ID 顺序不符');
  must(entryInventory.gates.length === 8, '入口完整性门禁应为 8 项');
  must(entryInventory.counts.sourceFiles > 0 && entryInventory.candidates.length === entryInventory.counts.candidates,
    '入口清单候选计数不一致');
  const phase1 = vmStatus.phase1Landing;
  must(phase1 && phase1.evidenceCard && phase1.evidenceReview && phase1.gateCheck &&
    Array.isArray(phase1.productChanges) && phase1.productChanges.length === 0 &&
    phase1.s0ExitGates && Object.keys(phase1.s0ExitGates).length === 3,
    '第一阶段落地台账字段缺失或产品改动不为零');
  must(phase1.diffPreserved &&
    phase1.diffPreserved.delta.blobCount === Math.abs(
      phase1.diffPreserved.directSdkAndLocal.blobCount - phase1.diffPreserved.originalVm.blobCount),
    '第一阶段登记的差分计数与两侧结果不符');
  const incrementalLanding = vmStatus.incrementalLanding;
  must(incrementalLanding && incrementalLanding.cardId === 'C03-INCREMENTAL-B0B1-001' &&
    incrementalLanding.evidenceCard && incrementalLanding.humanReview &&
    incrementalLanding.commandsExecuted === 1 &&
    Array.isArray(incrementalLanding.productChanges) && incrementalLanding.productChanges.length === 0 &&
    incrementalLanding.batchStatus && incrementalLanding.batchStatus.B0 === 'open',
    '渐进落地第一批回写字段缺失或状态不符');
  const b1Static = incrementalLanding.b1Static;
  must(b1Static && b1Static.cardId === 'C03-INCREMENTAL-B1-STATIC-001' &&
    b1Static.evidenceCard && b1Static.humanReview &&
    b1Static.sourceFiles === 8 && b1Static.solutionFiles === 3 &&
    b1Static.commandsExecuted === 0 && b1Static.productChanges === 0 &&
    b1Static.batchStatusUnchanged === 'B0 open / B1-B3 blocked',
    '第二批静态取证回写字段缺失或状态不符');
  const b1Contracts = b1Static.b1Contracts;
  must(b1Contracts && b1Contracts.cardId === 'C03-INCREMENTAL-B1-CONTRACTS-001' &&
    b1Contracts.evidenceCard && b1Contracts.humanReview &&
    b1Contracts.gapsClosed.length === 2 &&
    b1Static.outputNameComparison.closedBy === b1Contracts.cardId &&
    b1Static.outputNameComparison.boundary.includes('not original native ABI'),
    '第三批输出名契约回写字段缺失或边界不符');

  // ---- 三套进度口径（并列展示，冲突不擅自统一） ----
  const legacyProgress = reversePlan.activeBatch.progress;
  must(legacyProgress.unitsTotal === 96, '历史口径分母不是 96');
  const progress = {
    framework: {
      current: {
        done: audit.summary.layers.implementation.done,
        total: audit.summary.units,
        percent: audit.summary.layers.implementation.percent,
        statuses: audit.summary.layers.implementation.statuses,
        source: INPUTS[3],
        reconciliation: audit.summary.summaryReconciliation
      },
      legacy: {
        done: legacyProgress.implementationDone,
        total: legacyProgress.unitsTotal,
        percent: legacyProgress.overallCompletionPercent,
        source: INPUTS[2] + ' (activeBatch.progress) 与 AGENTS.md / 历史交接'
      },
      layers: audit.summary.layers,
      allThreeDone: audit.summary.allThreeDone,
      allThreePercent: audit.summary.allThreePercent,
      policy: audit.overallProgressPolicy,
      packages: audit.packages.length,
      blockers: audit.blockers.length
    },
    plugins: {
      calibration: {
        done: sourceLevel.completedTools,
        total: sourceLevel.totalTools,
        percent: Number(sourceLevel.completionPercent),
        source: INPUTS[5],
        manualAcceptance: sourceLevel.toolCompletion[0].manualAcceptance
      },
      nextBatch: {
        done: nextBatch.completedCount,
        total: nextBatch.totalCount,
        percent: nextBatch.completionPercent,
        source: INPUTS[4],
        completedTools: nextBatch.completedTools,
        activeTools: nextBatch.activeTools,
        nextTools: nextBatch.nextTools
      },
      catalog: { total: coverage.summary.plugin_count, categories: categories.length }
    },
    closure: {
      objective: vmStatus.currentObjective,
      status: vmStatus.objectiveStatus,
      closedStages: vmStatus.closedStages,
      totalStages: vmStatus.totalStages,
      completionPercent: Number(vmStatus.completionPercent),
      scope: vmStatus.completionScope,
      source: INPUTS[6],
      nextAction: vmStatus.nextAction
    }
  };
  must(progress.framework.current.done === 42 && progress.framework.legacy.done === 41,
    '41/96 与 42/96 的汇总字段冲突不再成立，需重新登记口径');

  // ---- 纵向切片：数值全部来自运行报告与 VM 状态台账 ----
  const blob = sliceRuntime.blob;
  must(blob && Number.isFinite(blob.count) && Number.isFinite(blob.totalArea), 'C01 运行报告缺少 Blob 数值');
  const originalText = vmStatus.currentSlice.c03RuntimeEvidence || '';
  const blobCountMatch = /BlobCount\s+(\d+)/.exec(originalText);
  const totalAreaMatch = /TotalArea\s+(\d+)/.exec(originalText);
  must(blobCountMatch && totalAreaMatch, 'VM 状态台账中缺少原厂 BlobCount / TotalArea 运行确认值');
  const slice = Object.assign({}, editorial.slice, {
    directSdk: { count: blob.count, totalArea: blob.totalArea, outputs: (blob.outputs || []).length, nodeStatus: blob.nodeStatus },
    originalVm: { count: Number(blobCountMatch[1]), totalArea: Number(totalAreaMatch[1]) },
    c01: {
      passedChecks: vmStatus.c01Evidence.passedChecks,
      testProject: vmStatus.c01Evidence.testProject,
      report: vmStatus.c01Evidence.report
    },
    c02Differential: vmStatus.c02Differential,
    currentSliceEvidence: vmStatus.currentSlice.c03UiEvidence,
    knownLimit: vmStatus.currentSlice.knownLimit
  });
  must(slice.directSdk.count !== slice.originalVm.count, '原厂/直接 SDK 数值差分已消失，页面结论需重新取证');

  // ---- 162 工具计划视图：身份来自两份台账，批次来自台账 priority，进度来自批次台账 ----
  const batchByPriority = new Map(editorial.batchRules.map(rule => [rule.priority, rule]));
  const completedTools = new Set(nextBatch.completedTools);
  const activeTools = new Set(nextBatch.activeTools);
  const coverageByKey = new Map(coverage.plugins.map(plugin => [plugin.category + '/' + plugin.plugin, plugin]));
  const tools = inventory.plugins.map(plugin => {
    const coverageRow = coverageByKey.get(plugin.category + '/' + plugin.plugin);
    must(coverageRow, '缺少覆盖审计行: ' + plugin.plugin);
    const batchRule = batchByPriority.get(plugin.priority);
    must(batchRule, '未知 priority，未登记分批规则: ' + plugin.priority + ' (' + plugin.plugin + ')');
    const progressState = completedTools.has(plugin.plugin) ? 'completed'
      : activeTools.has(plugin.plugin) ? 'active' : 'not-started';
    let evidenceState = 'unknown';
    if (/partial-independent|implemented|done/i.test(plugin.implementationStatus || '')) evidenceState = 'independent-implementation';
    else if (/matched|confirmed/i.test(plugin.runtimeStatus || '')) evidenceState = 'runtime-confirmed';
    else if ((plugin.xmlEvidence || []).length) evidenceState = 'source-confirmed';
    return {
      key: plugin.plugin,
      category: plugin.category,
      displayName: plugin.displayName,
      moduleId: plugin.moduleId,
      priority: plugin.priority,
      batch: batchRule.batch,
      batchLabel: batchRule.label,
      batchRule: batchRule.rule,
      owner: plugin.owner,
      sourcePath: plugin.sourcePath,
      progress: progressState,
      evidenceState,
      staticStatus: plugin.staticStatus,
      implementationStatus: plugin.implementationStatus,
      runtimeStatus: plugin.runtimeStatus,
      dependencyStatus: plugin.dependencyStatus,
      nextAction: plugin.nextAction,
      details: {
        uiTabs: plugin.uiTabs || [],
        uiControls: plugin.uiControls || [],
        algorithmParameters: plugin.algorithmParameters || [],
        inputs: plugin.inputs || [],
        outputs: plugin.outputs || [],
        xmlEvidence: plugin.xmlEvidence || [],
        binaryEvidence: plugin.binaryEvidence || [],
        declaredLibraries: plugin.declaredLibraries || [],
        sharedUiGroups: plugin.sharedUiGroups || [],
        resourceEvidence: plugin.resourceEvidence || [],
        catalogIcon: plugin.catalogIcon || null
      },
      counts: {
        xmls: (coverageRow.xmls || []).length,
        binaries: (coverageRow.binaries || []).length,
        uiTabs: (plugin.uiTabs || []).length,
        uiControls: (plugin.uiControls || []).length,
        algorithmParameters: (plugin.algorithmParameters || []).length,
        inputs: (plugin.inputs || []).length,
        outputs: (plugin.outputs || []).length
      },
      nativeAnalysis: coverageRow.native_analysis,
      runtimeComparison: coverageRow.runtime_comparison,
      semanticCompleteness: coverageRow.semantic_completeness,
      analysisScope: coverageRow.analysis_scope
    };
  });
  must(tools.length === 162 && unique(tools.map(tool => tool.key)).length === 162, '162 工具身份存在重复');
  // 批次台账里的名字未必都能在 162 目录身份中一一对应；差异必须原样登记，不允许猜测映射。
  const matchedCompleted = new Set(tools.filter(tool => tool.progress === 'completed').map(tool => tool.key));
  const matchedActive = new Set(tools.filter(tool => tool.progress === 'active').map(tool => tool.key));
  const batchIdentityGaps = {
    completed: nextBatch.completedTools.filter(name => !matchedCompleted.has(name)),
    active: nextBatch.activeTools.filter(name => !matchedActive.has(name)),
    policy: '未匹配项保留批次台账原名，禁止在 162 目录台账中猜测或补造身份'
  };
  must(matchedCompleted.size + batchIdentityGaps.completed.length === nextBatch.completedCount,
    '已完成工具与身份差异之和不等于批次台账总数');
  must(matchedActive.size + batchIdentityGaps.active.length === nextBatch.activeTools.length,
    '在制工具与身份差异之和不等于批次台账总数');
  progress.plugins.nextBatch.identityGaps = batchIdentityGaps;
  const toolsByCategory = categories.map(category => ({
    name: category,
    declared: categoryCounts[category],
    tools: tools.filter(tool => tool.category === category)
  }));
  must(toolsByCategory.every(group => group.declared === group.tools.length), '分类计数与实际工具数不一致');

  const stages = editorial.stages.map((stage, index) => ({
    ...stage,
    priority: index === 0 ? null : baseline.priorities[index - 1] || null
  }));

  const layerTotal = sum(baseline.layers.map(layer => layer.original), 'lines');
  const layers = baseline.layers.map(layer => ({
    id: layer.id,
    name: layer.name,
    status: layer.status,
    gap: layer.gap,
    units: layer.units,
    assemblies: layer.assemblies,
    original: layer.original,
    currentProjects: (layer.current || []).map(project => project.project),
    currentLines: layer.currentLines,
    sharePercent: round1(layer.original.lines / layerTotal * 100),
    lineCoveragePercent: round1(layer.currentLines / layer.original.lines * 100)
  }));

  const model = {
    schemaVersion: 1,
    builtFrom: { head, inputsDigest },
    inputs,
    supporting,
    sourcePaths: {
      coverage: INPUTS[0],
      baseline: INPUTS[1],
      reversePlan: INPUTS[2],
      progressAudit: INPUTS[3],
      nextBatch: INPUTS[4],
      sourceLevel: INPUTS[5],
      vmStatus: INPUTS[6],
      inventory: INPUTS[7],
      entryInventory: INPUTS[8],
      sliceRuntime: SUPPORTING[0],
      baselineDoc: SUPPORTING[1],
      editorial: SUPPORTING[2]
    },
    metrics: {
      original: {
        assemblies: baseline.totals.original.assemblies,
        trees: baseline.totals.original.trees,
        rawCs: baseline.totals.original.rawCs,
        rawLines: baseline.totals.original.rawLines,
        cs: baseline.totals.original.uniqueCs,
        lines: baseline.totals.original.uniqueLines,
        types: baseline.totals.original.uniqueTypes,
        methods: baseline.totals.original.uniqueMethods,
        xaml: sum(baseline.assemblies, 'xaml'),
        dependencies: baseline.originalEdges.length
      },
      current: {
        projects: baseline.totals.current.projects,
        cs: baseline.totals.current.cs,
        lines: baseline.totals.current.lines,
        types: baseline.totals.current.types,
        methods: sum(baseline.currentProjects, 'Methods'),
        xaml: sum(baseline.currentProjects, 'XamlFiles'),
        dependencies: baseline.currentEdges.length
      },
      lineRatio: round1(baseline.totals.original.uniqueLines / baseline.totals.current.lines)
    },
    scope: {
      method: baseline.method,
      regenerate: baseline.regenerate,
      sourceInventory: baseline.sourceInventory,
      mappingTable: baseline.mappingTable,
      auditDate: baseline.auditDate
    },
    layers,
    assemblies: baseline.assemblies,
    currentProjects: baseline.currentProjects,
    currentEdges: baseline.currentEdges,
    originalEdges: baseline.originalEdges,
    priorities: baseline.priorities,
    stages,
    architectureVerdict: editorial.architectureVerdict,
    pageNotes: editorial.pageNotes,
    masterPlan: editorial.masterPlan,
    taskStateMachine: editorial.taskStateMachine,
    legacyPlans: editorial.legacyPlans,
    closureChainStages: editorial.closureChainStages,
    cardTemplates: editorial.cardTemplates,
    purposeMap,
    frameworkTasks: reversePlan.tasks.map(task => ({
      id: task.id, group: task.group, title: task.title, status: task.status,
      wave: task.wave, dependencies: task.dependencies || [], entry: task.entry || '',
      question: task.question || '', actions: task.actions || '', done: task.done || '',
      evidenceStatus: task.evidenceStatus || {}, reconciledProgress: task.reconciledProgress || {},
      domains: task.domains || [],
      evidenceDoc: task.evidenceDoc, report: task.report, nextAction: task.nextAction,
      batchEvidenceDirectory: task.batchEvidenceDirectory
    })),
    frameworkUnits,
    tools,
    categories: toolsByCategory,
    categoryCounts,
    batchIdentityGaps,
    gaps: parseGapTable(baselineDoc),
    slice,
    phase1Landing: vmStatus.phase1Landing,
    incrementalLanding: vmStatus.incrementalLanding,
    progress,
    conflict: editorial.governance.conflictNote,
    evidenceStates: editorial.evidenceStates,
    acceptanceLevels: editorial.acceptanceLevels,
    stageGates: editorial.stageGates,
    entryGates: entryInventory.gates,
    entryCounts: entryInventory.counts,
    governance: editorial.governance,
    batchRules: editorial.batchRules,
    pages: editorial.pages,
    frameworks: {
      stageCriteria: audit.stageCriteria,
      statusPolicy: audit.statusPolicy,
      planProgressPolicy: reversePlan.progressPolicy,
      validationFreshness: audit.validationFreshness
    }
  };
  // WBS 与目标源码树都是纯派生结果；页面、plan-data.json 摘要与搜索索引共用同一份，避免两套口径。
  model.wbs = buildWbs(model);
  model.targetTree = buildTargetTree(model, targetTreeMap, root);
  model.uiLanding = buildUiLanding(model, uiLandingMap, root);
  model.incremental = buildIncremental(model, incrementalMap, root);
  model.startupInitialUi = buildStartupUi(model, startupUiMap, root);
  return model;
}

// 构建前后各调用一次；任何输入在生成过程中变化即中止。
function verifyInputs(model) {
  for (const item of model.inputs.concat(model.supporting)) {
    if (sha256Of(item.path) !== item.sha256) {
      throw new Error('[plan-model] 输入台账在构建期间发生变化: ' + item.path);
    }
  }
  return true;
}

module.exports = { root, out, INPUTS, SUPPORTING, buildModel, verifyInputs, readText, readJson, sha256Of, must, sum };
