'use strict';
// 主界面共享控件落地清单派生层：把台账控件逐项挂到既有 WBS 工作包与 E0–E7 门。
// 纯数据与只读校验：不改 WBS 口径、不预填完成状态、不写 wall-clock。
const fs = require('fs');
const path = require('path');

const SEGMENTS = ['CL1', 'CL2', 'CL3', 'CL4', 'CL5', 'CL6'];
const ACCEPTANCE_KEYS = ['sourceClosure', 'originalDifferential', 'localReRun'];

function must(condition, message) {
  if (!condition) throw new Error('[plan-ui-landing] ' + message);
}

function buildUiLanding(model, map, root) {
  must(model && model.wbs && Array.isArray(model.wbs.workPackages), '缺少 WBS 模型');
  must(model && Array.isArray(model.entryGates) && model.entryGates.length === 8, '缺少 E0–E7 门禁');
  must(map && map.schemaVersion === 1, '落地台账 schemaVersion 不符');
  must(map.controls.length === 17 && map.groups.length === 4 && map.acceptanceLevels.length === 3,
    '落地台账应为 17 控件 / 4 类 / 3 级验收');
  must(map.p0Prerequisites.length === 1 && map.p0Prerequisites[0].status === 'open',
    'S0/C03 前置阻塞应为 1 项且仍开放');

  const packageIds = new Set(model.wbs.workPackages.map(item => item.id));
  const gateIds = new Set(model.entryGates.map(item => item.id));
  const groupIds = new Set(map.groups.map(item => item.id));
  const blockerId = map.p0Prerequisites[0].id;
  const segmentNames = new Map(
    (model.closureChainStages || []).map(item => [item.id, item.name])
  );
  const acceptanceById = new Map(map.acceptanceLevels.map(item => [item.id, item]));

  const controls = map.controls.map(item => {
    must(groupIds.has(item.group), '控件分组不存在: ' + item.id + ' → ' + item.group);
    must(Array.isArray(item.relatedWorkPackageIds) && item.relatedWorkPackageIds.length > 0,
      '控件缺少工作包关联: ' + item.id);
    must(Array.isArray(item.relatedGateIds) && item.relatedGateIds.length > 0,
      '控件缺少门禁关联: ' + item.id);
    for (const id of item.relatedWorkPackageIds) {
      must(packageIds.has(id), '控件引用了不存在的工作包: ' + item.id + ' → ' + id);
    }
    for (const id of item.relatedGateIds) {
      must(gateIds.has(id), '控件引用了不存在的门禁: ' + item.id + ' → ' + id);
    }
    for (const relative of item.originalEvidencePaths) {
      must(fs.existsSync(path.join(root, relative)), '原厂证据路径不存在: ' + relative);
    }
    for (const prerequisite of map.p0Prerequisites) {
      for (const relative of prerequisite.evidencePaths) {
        must(fs.existsSync(path.join(root, relative)), '前置证据路径不存在: ' + relative);
      }
    }
    return {
      id: item.id,
      name: item.name,
      group: item.group,
      evidencePaths: item.originalEvidencePaths.slice(),
      sourceMembers: (item.sourceMembers || []).slice(),
      workPackageIds: item.relatedWorkPackageIds.slice(),
      gateIds: item.relatedGateIds.slice(),
      firstAction: item.firstAction,
      unknowns: (item.unknowns || []).slice(),
      evidenceHint: item.evidenceHint,
      sixSegmentStates: SEGMENTS.reduce((acc, id) => {
        acc[id] = { id, name: segmentNames.get(id) || id, state: 'unknown' };
        return acc;
      }, {}),
      acceptance: ACCEPTANCE_KEYS.map(id => {
        const level = acceptanceById.get(id);
        must(level, '验收级别缺失: ' + id);
        return { id, label: level.label, rule: level.rule, state: 'not_started' };
      }),
      planState: 'blocked',
      blockers: [blockerId],
      exitCriteria: ACCEPTANCE_KEYS.map(id => {
        const level = acceptanceById.get(id);
        return level.label + '：' + level.rule;
      })
    };
  });

  const groups = map.groups.map(group => ({
    id: group.id,
    name: group.name,
    description: group.description,
    workPackageIds: group.workPackageIds.slice(),
    gateIds: group.gateIds.slice(),
    controls: controls.filter(control => control.group === group.id)
  }));
  for (const group of groups) {
    must(group.controls.length > 0, '控件分组为空: ' + group.id);
  }

  const allGateIds = new Set(controls.flatMap(control => control.gateIds));
  must(allGateIds.size === 8, '控件应覆盖 E0–E7 共 8 门，实际 ' + allGateIds.size);
  const counts = {
    controls: controls.length,
    groups: groups.length,
    gates: allGateIds.size,
    acceptanceLevels: ACCEPTANCE_KEYS.length,
    p0Prerequisites: map.p0Prerequisites.length,
    blocked: controls.filter(control => control.planState === 'blocked').length,
    unknownSegments: controls.length * SEGMENTS.length,
    acceptanceDone: controls.reduce((n, control) =>
      n + control.acceptance.filter(item => item.state === 'done').length, 0),
    evidencePaths: controls.reduce((n, control) => n + control.evidencePaths.length, 0)
  };
  must(counts.acceptanceDone === 0, '不得预填任何验收完成状态');
  must(counts.blocked === counts.controls, 'S0 前置未闭时全部控件应为 blocked');

  return {
    controls,
    groups,
    p0Prerequisites: map.p0Prerequisites,
    acceptanceLevels: map.acceptanceLevels,
    evidenceBoundary: map.evidenceBoundary,
    counts
  };
}

module.exports = { buildUiLanding, SEGMENTS, ACCEPTANCE_KEYS };
