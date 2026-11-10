'use strict';
// 启动链路 + 整屏空载初始态：独立复算校验器。
// 逐字比对三份交付、复算 SHA-256 引用、零预填、引用闭合、builtFrom 与重跑确定性。
const fs = require('fs');
const path = require('path');
const crypto = require('crypto');
const lib = require('./plan-model.cjs');
const startup = require('./plan-startup-initial-ui.cjs');
const gen = require('./build-startup-initial-ui.cjs');

const root = lib.root;
const LEDGER = 'tests/ArchitectureRecovery/startup-initial-ui.map.json';
function must(condition, message) {
  if (!condition) throw new Error('[verify-startup-initial-ui] ' + message);
}
function sha(relative) {
  return crypto.createHash('sha256').update(fs.readFileSync(path.join(root, relative))).digest('hex');
}

function verify() {
  const model = lib.buildModel();
  lib.verifyInputs(model);
  const map = JSON.parse(fs.readFileSync(path.join(root, LEDGER), 'utf8'));
  const derived = startup.buildStartupUi(model, map, root);
  const expected = gen.buildStartupInitialUi(model, derived);

  const markdown = fs.readFileSync(gen.OUT_MD, 'utf8');
  const review = fs.readFileSync(gen.OUT_REVIEW, 'utf8');
  const cardText = fs.readFileSync(gen.OUT_CARD, 'utf8');
  const card = JSON.parse(cardText);

  must(markdown === expected.markdown, '可派工清单不是确定性产物');
  must(review === expected.review, '人读证据结论不是确定性产物');
  must(cardText === expected.card, '机器证据卡不是确定性产物');
  must(!markdown.includes('- [x]'), '清单不得预填任何勾选框');
  must((markdown.match(/^- \[ \] /gm) || []).length === derived.steps.length + derived.controls.length,
    '可勾选任务数应为 ' + (derived.steps.length + derived.controls.length));
  must(markdown.includes('§0 状态面与边界') && markdown.includes('§8 与既有产物的关系'),
    '清单章节不完整');

  // builtFrom 与站点同源；产物基线 HEAD 允许早于当前 HEAD。
  const builtFrom = model.builtFrom.head.slice(0, 8) + '/' + model.builtFrom.inputsDigest.slice(0, 12);
  must(card.builtFrom === builtFrom && markdown.includes(builtFrom),
    'builtFrom 与当前模型输入摘要不一致');

  // 全部证据引用真实存在且哈希/字节数匹配。
  let badEvidence = 0;
  for (const item of card.evidence) {
    if (!fs.existsSync(path.join(root, item.path))) { badEvidence++; continue; }
    if (sha(item.path) !== item.sha256) badEvidence++;
    if (fs.statSync(path.join(root, item.path)).size !== item.bytes) badEvidence++;
  }
  must(badEvidence === 0, '证据卡存在 ' + badEvidence + ' 条哈希/字节/路径不符');
  must(card.evidence.length >= derived.counts.evidencePaths,
    '证据卡引用数少于台账去重证据路径数');

  // 结构与诚实性。
  must(card.commands.length === 0 && card.productChanges.length === 0,
    '本轮不得登记命令或产品改动');
  must(card.counts.acceptanceDone === 0 && card.counts.done === 0 &&
    card.counts.sixSegmentUnknown === (derived.steps.length + derived.controls.length) * 6,
    '零预填或六段链 unknown 计数不符');
  must(card.acceptanceLevels.every(level => level.state === 'not_started'),
    '三级验收必须全部未开始');
  must(card.stateScope === map.stateScope, '状态面标注不一致');
  must(card.unlinkedLayoutEdges.length === 8 && card.counts.layoutEdgesTotal === 30 &&
    card.linkedLayoutEdges.length + card.unlinkedLayoutEdges.length === 30,
    '布局边关联/未关联计数不自洽');
  must(card.gateCoverage.length === 8 && card.packageCoverage.length === 8,
    'E0–E7 或 A 线工作包覆盖不符');
  must(card.unknowns.length > 0 && card.questionChecklist.some(q => q.status === 'unresolved'),
    '必须保留 unknown 与未解决问题');
  must(review.includes('source-confirmed') && review.includes('runtime-confirmed') &&
    review.includes('independent-implementation') && review.includes('unknown') &&
    review.includes('不把静态分析写成运行观察'),
    '证据结论缺少证据分级或边界声明');
  must(review.includes('未改 `src/` 产品代码；未跑方案测试') &&
    review.includes('未做原厂同状态差分与自研操作级复跑'),
    '证据结论缺少本轮边界');

  // 引用闭合：由派生层复核（台账路径/门禁/工作包/布局边）。
  const rebuilt = startup.buildStartupUi(model, map, root);
  must(JSON.stringify(rebuilt.counts) === JSON.stringify(derived.counts),
    '派生层计数不可复现');

  return {
    startupSteps: derived.counts.startupSteps,
    controls: derived.counts.controls,
    tasks: derived.steps.length + derived.controls.length,
    sixSegmentUnknown: derived.counts.sixSegmentUnknown,
    acceptanceDone: derived.counts.acceptanceDone,
    evidenceReferences: card.evidence.length,
    badEvidence,
    layoutEdgesLinked: derived.counts.layoutEdges,
    layoutEdgesUnlinked: derived.counts.unlinkedLayoutEdges,
    prefilledCheckboxes: 0,
    deterministic: true,
    builtFrom
  };
}

if (require.main === module) console.log(JSON.stringify(verify(), null, 2));
module.exports = { verify };
