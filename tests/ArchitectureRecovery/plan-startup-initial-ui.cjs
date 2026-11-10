'use strict';
// 启动链路 + 整屏空载初始态：纯派生层。
// 只做引用闭合与状态注入，不写结论数字、不预填验收、不生成产品代码改动清单。
const fs = require('fs');
const path = require('path');

function must(condition, message) {
  if (!condition) throw new Error('[plan-startup-initial-ui] ' + message);
}

function rel(root, absolute) {
  return path.relative(root, absolute).replace(/\\/g, '/');
}

function buildStartupUi(model, map, root) {
  must(model && model.wbs && model.entryGates, '缺少模型或 WBS/门禁台账');
  must(map && map.schemaVersion === 1, '启动态台账结构不符');
  must(map.stateScope === '空载初始态（程序刚启动、未加载任何方案、流程画布空白、图像显示区棋盘格空、历史结果表为空）',
    '启动态台账必须显式标注空载初始态状态面');
  must(Array.isArray(map.startupSteps) && map.startupSteps.length > 0, '缺少启动步骤');
  must(Array.isArray(map.controls) && map.controls.length > 0, '缺少初始态控件');
  must(map.sixSegmentStages.length === 6, '六段链应为 6 段');
  must(map.acceptanceLevels.length === 3 &&
    map.acceptanceLevels.every(item => item.state === 'not_started'),
    '三级验收必须存在且零预填');

  const layout = JSON.parse(fs.readFileSync(path.join(root,
    'docs/architecture-recovery/window-layout.edges.json'), 'utf8'));
  const edgeIds = new Set();
  (function collect(node) {
    if (node.edgeId) edgeIds.add(node.edgeId);
    for (const child of node.children || []) collect(child);
  })(layout.hierarchy);
  must(edgeIds.size === 30, '窗口布局边应为 30 条，实际 ' + edgeIds.size);

  const entry = JSON.parse(fs.readFileSync(path.join(root,
    'docs/architecture-recovery/entry-inventory.json'), 'utf8'));
  const gateIds = new Set(entry.gates.map(gate => gate.id));
  const packageIds = new Set(model.wbs.workPackages.map(item => item.id));
  const groupIds = new Set(map.groups.map(group => group.id));

  const seen = new Set();
  const evidencePaths = new Set();
  const sixSegmentUnknown = { steps: 0, controls: 0 };

  function derive(raw, kind) {
    must(raw.id && raw.name && raw.group, kind + ' 缺少 id/name/group: ' + JSON.stringify(raw).slice(0, 80));
    must(!seen.has(raw.id), kind + ' ID 重复: ' + raw.id);
    seen.add(raw.id);
    must(groupIds.has(raw.group), kind + ' 分组不可解析: ' + raw.id + ' → ' + raw.group);
    must(Array.isArray(raw.originalEvidencePaths) && raw.originalEvidencePaths.length > 0,
      kind + ' 缺少原厂证据路径: ' + raw.id);
    for (const relative of raw.originalEvidencePaths) {
      must(fs.existsSync(path.join(root, relative)),
        kind + ' 证据路径不存在: ' + raw.id + ' → ' + relative);
      evidencePaths.add(relative);
    }
    must(Array.isArray(raw.sourceMembers) && raw.sourceMembers.length > 0,
      kind + ' 缺少准确成员: ' + raw.id);
    for (const gate of raw.relatedGateIds || []) {
      must(gateIds.has(gate), kind + ' 门禁不可解析: ' + raw.id + ' → ' + gate);
    }
    for (const workPackage of raw.relatedWorkPackageIds || []) {
      must(packageIds.has(workPackage), kind + ' 工作包不可解析: ' + raw.id + ' → ' + workPackage);
    }
    for (const edge of raw.layoutEdgeIds || []) {
      must(edgeIds.has(edge), kind + ' 布局边不可解析: ' + raw.id + ' → ' + edge);
    }
    must(raw.firstAction && Array.isArray(raw.exitCriteria || []), kind + ' 缺少第一动作: ' + raw.id);
    must(!Array.isArray(raw.productChanges) && !raw.commands,
      kind + ' 不得预写产品改动清单或命令: ' + raw.id);

    // 六段链一律 unknown：本轮是源码级证据台账，不声称任何段已闭合。
    const sixSegmentStates = {};
    for (const stage of map.sixSegmentStages) {
      sixSegmentStates[stage.id] = {
        id: stage.id, name: stage.name, state: 'unknown',
        evidence: [], exitCriteria: '需源码级取证后填入'
      };
      sixSegmentUnknown[kind === 'step' ? 'steps' : 'controls']++;
    }
    const acceptance = map.acceptanceLevels.map(level => ({
      id: level.id, label: level.label, rule: level.rule, state: 'not_started'
    }));

    return Object.assign({}, raw, {
      kind,
      sixSegmentStates,
      acceptance,
      planState: 'not_started',
      blockers: ['S0/C03 根因仍开放；本轮未做原厂同状态差分与自研操作级复跑'],
      exitCriteria: (raw.exitCriteria || []).concat([
        '逐项登记证据类型，未取证处保持 unknown',
        '不以截图作实现规格、不臆造 native 函数体或资源'
      ])
    });
  }

  const steps = map.startupSteps.map(item => derive(item, 'step'));
  const controls = map.controls.map(item => derive(item, 'control'));

  const gateCoverage = new Set();
  const packageCoverage = new Set();
  const edgeCoverage = new Set();
  for (const item of steps.concat(controls)) {
    for (const gate of item.relatedGateIds) gateCoverage.add(gate);
    for (const workPackage of item.relatedWorkPackageIds) packageCoverage.add(workPackage);
    for (const edge of item.layoutEdgeIds || []) edgeCoverage.add(edge);
  }
  must(gateCoverage.size === 8, '应覆盖 E0–E7 共 8 门，实际 ' + gateCoverage.size);
  // 30 条窗口布局边并非全部与「空载初始态首屏」相关；未关联边如实登记，不强行凑数。
  const unlinkedLayoutEdges = [...edgeIds].filter(id => !edgeCoverage.has(id)).sort();

  const counts = {
    startupSteps: steps.length,
    controls: controls.length,
    groups: map.groups.length,
    acceptanceLevels: map.acceptanceLevels.length,
    sixSegmentStages: map.sixSegmentStages.length,
    layoutEdges: edgeCoverage.size,
    layoutEdgesTotal: edgeIds.size,
    unlinkedLayoutEdges: unlinkedLayoutEdges.length,
    gateCoverage: gateCoverage.size,
    packageCoverage: packageCoverage.size,
    sixSegmentUnknown: sixSegmentUnknown.steps + sixSegmentUnknown.controls,
    acceptanceDone: steps.concat(controls).reduce((n, item) =>
      n + item.acceptance.filter(level => level.state === 'done').length, 0),
    blocked: 0,
    done: 0,
    evidencePaths: evidencePaths.size
  };
  must(counts.acceptanceDone === 0 && counts.done === 0, '不得预填任何完成状态');
  must(counts.sixSegmentUnknown === (steps.length + controls.length) * 6,
    '六段链 unknown 计数不符');

  return {
    note: map.note,
    evidenceBoundary: map.evidenceBoundary,
    stateScope: map.stateScope,
    screenshotScope: map.screenshotScope,
    groups: map.groups,
    acceptanceLevels: map.acceptanceLevels,
    sixSegmentStages: map.sixSegmentStages,
    steps,
    controls,
    counts,
    edgeIds: [...edgeIds].sort(),
    linkedEdgeIds: [...edgeCoverage].sort(),
    unlinkedLayoutEdges,
    gateIds: [...gateCoverage].sort(),
    packageIds: [...packageCoverage].sort(),
    evidencePaths: [...evidencePaths].sort(),
    layoutSource: 'docs/architecture-recovery/window-layout.edges.json'
  };
}

module.exports = { buildStartupUi, must, rel };
