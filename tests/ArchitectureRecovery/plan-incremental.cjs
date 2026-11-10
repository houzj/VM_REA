'use strict';
// 渐进落地路线派生层：批次 → 步骤 → 可派工任务数据。
// 只读校验引用闭合；状态统一 not_started，不预填 done；命令只允许既有台账登记过的真实命令。
const fs = require('fs');
const path = require('path');

function must(condition, message) {
  if (!condition) throw new Error('[plan-incremental] ' + message);
}

function buildIncremental(model, map, root) {
  must(model && Array.isArray(model.pages), '缺少计划模型');
  must(map && map.schemaVersion === 1 && Array.isArray(map.batches) && map.batches.length === 4,
    '渐进落地台账应为 4 个批次');
  must(map.allowedCommands.length === 1, '允许命令清单应只登记 C01 复跑命令');

  const vmStatus = JSON.parse(fs.readFileSync(
    path.join(root, 'docs', 'VM-LOCAL-RESTORATION-STATUS.json'), 'utf8'
  ));
  must(vmStatus.c01Evidence && vmStatus.c01Evidence.command === map.allowedCommands[0],
    '允许命令与 VM-LOCAL-RESTORATION-STATUS.json → c01Evidence.command 不一致');

  const batchIds = map.batches.map(batch => batch.id);
  must(new Set(batchIds).size === batchIds.length, '批次 ID 重复');
  const order = new Map(batchIds.map((id, i) => [id, i]));

  const batches = map.batches.map(batch => {
    for (const prerequisite of batch.prerequisites) {
      must(order.has(prerequisite), '批次前置不存在: ' + batch.id + ' → ' + prerequisite);
      must(order.get(prerequisite) < order.get(batch.id), '批次依赖顺序不成链: ' + prerequisite + ' → ' + batch.id);
    }
    const steps = batch.steps.map(step => {
      for (const relative of step.evidencePaths) {
        must(fs.existsSync(path.join(root, relative)), '证据路径不存在: ' + step.id + ' → ' + relative);
      }
      for (const relative of step.reportPaths) {
        must(fs.existsSync(path.join(root, relative)), '报告路径不存在: ' + step.id + ' → ' + relative);
      }
      for (const relative of step.expectedOutputPaths) {
        must(!fs.existsSync(path.join(root, relative)),
          '待产出路径已存在，不得冒充计划产出: ' + step.id + ' → ' + relative);
      }
      for (const command of step.commands) {
        must(map.allowedCommands.includes(command),
          '步骤登记了未授权命令: ' + step.id + ' → ' + command);
      }
      return {
        id: step.id,
        batchId: batch.id,
        title: step.title,
        evidencePaths: step.evidencePaths.slice(),
        sourceMembers: (step.sourceMembers || []).slice(),
        commands: step.commands.slice(),
        reportPaths: step.reportPaths.slice(),
        expectedOutputPaths: step.expectedOutputPaths.slice(),
        exitCriteria: step.exitCriteria.slice(),
        unknowns: (step.unknowns || []).slice(),
        state: 'not_started',
        blockers: batch.prerequisites.filter(id => {
          const prerequisite = map.batches.find(item => item.id === id);
          return prerequisite && prerequisite.status !== 'closed';
        })
      };
    });
    return {
      id: batch.id,
      title: batch.title,
      scope: batch.scope,
      prerequisites: batch.prerequisites.slice(),
      status: batch.status,
      steps
    };
  });

  const steps = batches.flatMap(batch => batch.steps);
  must(steps.length === 13, '渐进步骤应为 13 步，实际 ' + steps.length);
  must(new Set(steps.map(step => step.id)).size === steps.length, '步骤 ID 重复');
  const counts = {
    batches: batches.length,
    steps: steps.length,
    notStarted: steps.filter(step => step.state === 'not_started').length,
    done: steps.filter(step => step.state === 'done').length,
    stepsWithBlockers: steps.filter(step => step.blockers.length > 0).length,
    commandEntries: steps.reduce((n, step) => n + step.commands.length, 0),
    evidencePaths: steps.reduce((n, step) => n + step.evidencePaths.length, 0),
    reportPaths: steps.reduce((n, step) => n + step.reportPaths.length, 0),
    expectedOutputPaths: steps.reduce((n, step) => n + step.expectedOutputPaths.length, 0),
    unknownItems: steps.reduce((n, step) => n + step.unknowns.length, 0)
  };
  must(counts.done === 0 && counts.notStarted === counts.steps, '不得预填任何步骤完成状态');
  must(batches[0].status === 'open' && batches.slice(1).every(batch => batch.status === 'blocked'),
    'B0 应开放、后续批次在前置未闭时应为 blocked');

  return {
    batches,
    steps,
    allowedCommands: map.allowedCommands.slice(),
    evidenceBoundary: map.evidenceBoundary,
    counts
  };
}

module.exports = { buildIncremental };
