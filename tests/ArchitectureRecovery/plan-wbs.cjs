'use strict';
// 完整逆向总计划的 WBS 派生层。只读入 plan-model 已装配的模型，不读文件、不写盘、不产生新数字。
// 产出：masterPlan / legacyPlans / workPackages(32) / unitCards(96) / toolCards(162) / entryChains(8×6)。
function must(condition, message) {
  if (!condition) throw new Error('[plan-wbs] ' + message);
}
function uniq(items) { return [...new Set(items.filter(Boolean))]; }
function text(value) { return value === null || value === undefined ? '' : String(value); }
function namesOf(items, limit) {
  if (!Array.isArray(items)) return [];
  const names = items.map(item => {
    if (item === null || item === undefined) return '';
    if (typeof item === 'string') return item;
    if (typeof item === 'object') {
      return text(item.name || item.key || item.id || item.path || item.displayName ||
        item.file || item.label || item.type || item.caption || item.value).slice(0, 160);
    }
    return text(item);
  }).filter(Boolean);
  return limit ? names.slice(0, limit) : names;
}
// 台账中确有空字段（例如部分工作包未写 nextAction）。计划不得臆造动作，只登记 unknown 与补证要求。
const UNKNOWN_FIELD = '台账未登记（unknown）：需按源码级取证或台账补证后填入，不以合理设计填空';
function filled(card, fields, label) {
  for (const field of fields) {
    const value = card[field];
    must(Array.isArray(value) ? value.length > 0 : text(value).trim().length > 0,
      label + ' 缺少必填字段: ' + field);
  }
}

function packageState(task, units) {
  const implDone = units.filter(unit => unit.implementation === 'done').length;
  const valDone = units.filter(unit => unit.validation === 'done').length;
  const srcDone = units.filter(unit => unit.source === 'done').length;
  if (task.status === 'blocked') return 'blocked';
  if (implDone === units.length && valDone === units.length && srcDone === units.length) return 'done';
  if (implDone === units.length) return 'self_review';
  if (implDone > 0 || task.status === 'running') return 'implementation';
  if (task.status === 'evidence-ready' || srcDone > 0) return 'evidence';
  if (units.some(unit => unit.implementation === 'blocked' || unit.validation === 'blocked')) return 'blocked';
  return 'not_started';
}
function unitState(unit) {
  if (unit.implementation === 'done' && unit.validation === 'done' && unit.source === 'done') return 'done';
  if (unit.implementation === 'done') return 'self_review';
  if (unit.implementation === 'partial' || unit.implementation === 'done') return 'implementation';
  if (unit.implementation === 'blocked' || unit.validation === 'blocked') return 'blocked';
  if (unit.source === 'partial' || unit.source === 'done') return 'evidence';
  return 'not_started';
}
function toolState(tool) {
  if (tool.progress === 'completed') return 'done';
  if (tool.progress === 'active') return 'implementation';
  if (tool.evidenceState === 'source-confirmed' || tool.evidenceState === 'runtime-confirmed') return 'evidence';
  return 'not_started';
}

function dependencyOrder(workPackages) {
  const ids = new Set(workPackages.map(pkg => pkg.id));
  const indegree = new Map(workPackages.map(pkg => [pkg.id, 0]));
  for (const pkg of workPackages) {
    for (const dep of pkg.dependencies) {
      must(ids.has(dep), '工作包依赖 ID 无法解析: ' + pkg.id + ' → ' + dep);
      indegree.set(pkg.id, indegree.get(pkg.id) + 1);
    }
  }
  const byWave = (a, b) => (a.wave - b.wave) || a.id.localeCompare(b.id);
  const ready = workPackages.filter(pkg => indegree.get(pkg.id) === 0).sort(byWave);
  const order = [];
  while (ready.length) {
    const current = ready.shift();
    order.push(current.id);
    for (const pkg of workPackages) {
      if (!pkg.dependencies.includes(current.id)) continue;
      indegree.set(pkg.id, indegree.get(pkg.id) - 1);
      if (indegree.get(pkg.id) === 0) ready.push(pkg);
    }
    ready.sort(byWave);
  }
  must(order.length === workPackages.length, '工作包依赖图存在环，接力顺序无法生成');
  return order;
}

function buildWbs(model) {
  must(model.frameworkTasks.length === 32, '工作包应为 32 个，实际 ' + model.frameworkTasks.length);
  must(model.frameworkUnits.length === 96, '具名单元应为 96 个');
  must(model.tools.length === 162, '目录工具应为 162 个');
  must(model.entryGates.length === 8, '入口门禁应为 8 个');
  must(model.closureChainStages.length === 6, '闭合链应为 6 段');

  const stateIds = new Set(model.taskStateMachine.states.map(state => state.id));
  const unitTemplate = model.cardTemplates.unit;
  const toolTemplate = model.cardTemplates.tool;
  const unitFields = unitTemplate.requiredFields;
  const toolFields = toolTemplate.requiredFields;
  must(unitFields.length === 6 && toolFields.length === 6, '任务卡必填字段应为 6 项');
  const g6 = model.stageGates.find(gate => gate.id === 'G6');
  const s9 = model.stages.find(stage => stage.id === 'S9');
  must(g6 && s9, '缺少 G6 门禁或 S9 阶段，工具卡退出判据无法生成');

  // ---- 32 个工作包：全部字段来自 framework-reverse-plan.json，计划状态由三层单元状态派生 ----
  const layerByPackage = new Map();
  for (const layer of model.layers) {
    for (const task of model.frameworkTasks) {
      if (!layer.units.includes(task.id)) continue;
      if (!layerByPackage.has(task.id)) layerByPackage.set(task.id, []);
      layerByPackage.get(task.id).push(layer.id);
    }
  }
  const workPackages = model.frameworkTasks.map(task => {
    const units = model.frameworkUnits.filter(unit => unit.package === task.id);
    must(units.length === 3, '工作包 ' + task.id + ' 应含 3 个具名单元，实际 ' + units.length);
    const layerIds = layerByPackage.get(task.id) || [];
    const stageIds = model.stages
      .filter(stage => stage.priority && stage.priority.layers.some(layer => layerIds.includes(layer)))
      .map(stage => stage.id);
    const rawFields = {
      entry: task.entry, question: task.question, actions: task.actions,
      done: task.done, nextAction: task.nextAction
    };
    const fieldGaps = Object.entries(rawFields)
      .filter(([, value]) => !text(value).trim())
      .map(([key]) => key);
    return {
      id: task.id,
      title: task.title,
      group: task.group,
      wave: task.wave,
      status: task.status,
      dependencies: task.dependencies,
      entry: text(task.entry).trim() || UNKNOWN_FIELD,
      question: text(task.question).trim() || UNKNOWN_FIELD,
      actions: text(task.actions).trim() || UNKNOWN_FIELD,
      done: text(task.done).trim() || UNKNOWN_FIELD,
      nextAction: text(task.nextAction).trim() || UNKNOWN_FIELD,
      fieldGaps,
      evidenceStatus: task.evidenceStatus,
      reconciledProgress: task.reconciledProgress,
      domains: task.domains,
      unitIds: units.map(unit => unit.id),
      layerIds,
      stageIds,
      evidencePaths: uniq([
        task.evidenceDoc, task.report, task.batchEvidenceDirectory,
        ...units.flatMap(unit => unit.sourceAndOperationReports)
      ]),
      implementationPaths: uniq(units.flatMap(unit => unit.implementationArtifacts)),
      planState: packageState(task, units),
      planAction: true
    };
  });
  must(new Set(workPackages.map(pkg => pkg.id)).size === 32, '工作包 ID 存在重复');
  const order = dependencyOrder(workPackages);
  const orderIndex = new Map(order.map((id, index) => [id, index + 1]));
  for (const pkg of workPackages) {
    pkg.orderIndex = orderIndex.get(pkg.id);
    must(stateIds.has(pkg.planState), '工作包计划状态未登记: ' + pkg.id + '/' + pkg.planState);
    filled(pkg, ['entry', 'question', 'actions', 'done', 'nextAction'], '工作包 ' + pkg.id);
  }

  // ---- 96 张框架单元任务卡 ----
  const packageById = new Map(workPackages.map(pkg => [pkg.id, pkg]));
  const criteria = model.frameworks.stageCriteria;
  const unitCards = model.frameworkUnits.map(unit => {
    const pkg = packageById.get(unit.package);
    must(pkg, '单元所属工作包不存在: ' + unit.id);
    const fallback = model.sourcePaths.reversePlan;
    const sourcePaths = uniq([...unit.sourceAndOperationReports, ...pkg.evidencePaths, fallback]);
    const unknowns = uniq([
      unit.limitation,
      unit.blockerIds.length ? '阻塞 ID：' + unit.blockerIds.join('、') : '',
      unit.source !== 'done' ? '源码证据未闭合：需 REA 补证具体成员与调用链' : '',
      unit.validation !== 'done' ? '操作级验证未闭合：需可复跑记录' : ''
    ]);
    const card = {
      id: unit.id,
      package: unit.package,
      packageTitle: pkg.title,
      orderIndex: pkg.orderIndex,
      criterion: unit.criterion,
      states: { source: unit.source, implementation: unit.implementation, validation: unit.validation },
      blockers: unit.blockerIds,
      limitation: unit.limitation,
      objective: unit.criterion + '：按源码级调用链与数据流补齐该具名单元',
      sourcePaths,
      implementationPaths: uniq([...unit.implementationArtifacts, ...pkg.implementationPaths]),
      actions: unitTemplate.checklist.concat([
        '工作包动作：' + pkg.actions,
        '待答问题：' + pkg.question
      ]),
      deliverable: [
        criteria.implementation,
        '证据卡：源路径、准确成员、证据类型、已确认行为、重建行为、未知项、操作级验证'
      ],
      operationCheck: [
        criteria.validation,
        '历史记录未在本次复跑的须注明未重跑；失败分支与反例不得被覆盖'
      ],
      unknowns,
      checklist: unitTemplate.checklist,
      unknownRule: unitTemplate.unknownRule,
      exitCriteria: [criteria.source, criteria.implementation, criteria.validation, unitTemplate.exitRule],
      planState: unitState(unit),
      planAction: true
    };
    filled(card, unitFields, '单元卡 ' + unit.id);
    must(stateIds.has(card.planState), '单元卡计划状态未登记: ' + unit.id);
    return card;
  });
  must(new Set(unitCards.map(card => card.id)).size === 96, '单元卡 ID 存在重复');

  // ---- 162 张目录工具任务卡 ----
  const detailHint = (label, items, limit) => label + '：' +
    (namesOf(items, limit).join('、') || '台账未登记');
  const toolCards = model.tools.map(tool => {
    const details = tool.details || {};
    const unknowns = uniq([
      tool.nativeAnalysis, tool.runtimeComparison, tool.semanticCompleteness,
      tool.analysisScope, tool.dependencyStatus, tool.implementationStatus, tool.runtimeStatus
    ]).map(value => text(value)).filter(Boolean);
    const card = {
      key: tool.key,
      displayName: tool.displayName,
      category: tool.category,
      moduleId: tool.moduleId,
      owner: tool.owner,
      sourcePath: tool.sourcePath,
      batch: tool.batch,
      batchLabel: tool.batchLabel,
      batchRule: tool.batchRule,
      progress: tool.progress,
      evidenceState: tool.evidenceState,
      counts: tool.counts,
      identity: [
        '显示名：' + tool.displayName,
        '工具键：' + tool.key,
        '原厂分类：' + tool.category,
        'ModuleId：' + (tool.moduleId === null ? '台账未登记' : tool.moduleId),
        '优先级批次：' + tool.batch + ' · ' + tool.batchLabel,
        '当前接收路径：' + tool.sourcePath
      ],
      checklist: toolTemplate.checklist.concat([
        detailHint('已登记页签', details.uiTabs, 4),
        detailHint('已登记输入', details.inputs, 4),
        detailHint('已登记输出', details.outputs, 4),
        detailHint('XML 证据', details.xmlEvidence, 3),
        detailHint('二进制证据', details.binaryEvidence, 3)
      ]),
      firstAction: tool.nextAction + '（' + tool.batchRule + '）',
      exitCriteria: [
        toolTemplate.exitRule,
        g6.rule,
        'S9 退出：' + s9.exitCriteria.join('；')
      ],
      ownerLine: [
        '责任线：' + tool.owner,
        model.governance.ownership[1],
        '人工主机验收独立，既不阻塞也不计入完成度'
      ],
      evidenceFields: [
        '源路径与准确成员', '证据类型（四态之一）', '确认的行为', '重建的行为',
        '未知项与 REA 补证目标', '操作级验证',
        '静态状态：' + tool.staticStatus,
        '实施状态：' + tool.implementationStatus,
        '运行对比：' + tool.runtimeStatus
      ],
      unknowns,
      unknownRule: toolTemplate.unknownRule,
      evidenceSources: namesOf([...(details.xmlEvidence || []), ...(details.binaryEvidence || [])], 5),
      planState: toolState(tool),
      planAction: true
    };
    filled(card, toolFields, '工具卡 ' + tool.key);
    must(stateIds.has(card.planState), '工具卡计划状态未登记: ' + tool.key);
    return card;
  });
  must(new Set(toolCards.map(card => card.key)).size === 162, '工具卡 key 存在重复');

  // ---- E0–E7 入口闭合链：8 门禁 × 6 段 ----
  const entryChains = model.entryGates.map(gate => ({
    gateId: gate.id,
    title: gate.title,
    gateState: gate.state,
    gap: gate.gap,
    nodes: model.closureChainStages.map(stage => ({
      segment: stage.name,
      stageId: stage.id,
      task: stage.objective + ' · 门禁 ' + gate.id + ' ' + gate.title,
      evidence: stage.evidence,
      exitCriteria: stage.exitCriteria,
      unknowns: gate.gap,
      gateState: gate.state,
      planState: 'unknown',
      planAction: true
    })),
    unknownCount: model.closureChainStages.length
  }));
  must(new Set(entryChains.map(chain => chain.gateId)).size === 8, '入口门禁 ID 存在重复');
  const chainNodes = entryChains.reduce((count, chain) => count + chain.nodes.length, 0);
  must(chainNodes === 48, '链节点应为 48 个，实际 ' + chainNodes);

  // ---- 总纲与旧计划处置（原样透传，只做字段完整性断言） ----
  must(text(model.masterPlan.authority).length > 0 && model.masterPlan.executionRules.length > 0,
    '权威总纲声明或执行规则为空');
  must(model.legacyPlans.length === 4, '旧计划处置登记应为 4 份');
  for (const legacy of model.legacyPlans) {
    filled(legacy, ['path', 'title', 'disposition', 'effectiveParts', 'status', 'note'], '旧计划 ' + legacy.path);
  }

  return {
    masterPlan: model.masterPlan,
    taskStateMachine: model.taskStateMachine,
    legacyPlans: model.legacyPlans,
    closureChainStages: model.closureChainStages,
    cardTemplates: model.cardTemplates,
    workPackages,
    unitCards,
    toolCards,
    entryChains,
    dependencyOrder: order,
    entryCounts: model.entryCounts,
    batchIdentityGaps: model.batchIdentityGaps,
    counts: {
      workPackages: workPackages.length,
      unitCards: unitCards.length,
      toolCards: toolCards.length,
      entryChains: entryChains.length,
      chainNodes,
      legacyPlans: model.legacyPlans.length,
      planStates: model.taskStateMachine.states.length,
      workPackageFieldGaps: workPackages.reduce((n, pkg) => n + pkg.fieldGaps.length, 0)
    }
  };
}

module.exports = { buildWbs };
