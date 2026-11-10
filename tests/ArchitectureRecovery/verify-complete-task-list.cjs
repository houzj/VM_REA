'use strict';
// 完整任务清单独立校验：复算全部条目，逐字比对 Markdown/台账，核对覆盖、路径、锚点、零预填与 builtFrom。
const fs = require('fs');
const path = require('path');
const cp = require('child_process');
const lib = require('./plan-model.cjs');
const gen = require('./build-complete-task-list.cjs');

const root = lib.root;
function must(condition, message) {
  if (!condition) throw new Error('[verify-complete-task-list] ' + message);
}
function isLocal(relative) {
  return /^(src|docs|analysis|tests)\//.test(relative);
}
const EXPECTED_PREFIX_COUNTS = {
  'P0-': 10, 'INC-': 13, 'UI-': 17, 'WP-': 32, 'UNIT-': 96, 'TOOL-': 162,
  'CHAIN-': 48, 'ASM-': 25, 'PROJ-': 29, 'EDGE-': 62, 'LAYER-': 8,
  'STAGE-': 10, 'GATE-': 8, 'E-': 8, 'EVID-': 4, 'ACCEPT-': 3,
  'LEGACY-': 4, 'RULE-': 5
};

function verify() {
  const model = lib.buildModel();
  lib.verifyInputs(model);
  const expected = gen.buildCompleteList(model);
  const markdown = fs.readFileSync(gen.OUT_MD, 'utf8');
  const raw = fs.readFileSync(gen.OUT_JSON, 'utf8');
  const ledger = JSON.parse(raw);

  // 产物记录「生成时基线 HEAD」；提交后当前 HEAD 前进，逐字比对时把新基线归一化到产物基线，
  // 并单独验证产物基线确为当前 HEAD 的祖先（或相等），避免自指循环。
  const diskBuiltFrom = ledger.builtFrom;
  const currentBuiltFrom = expected.ledger.builtFrom;
  const diskHead = diskBuiltFrom.split('/')[0];
  const currentHead = model.builtFrom.head;
  let headAncestor = currentHead.startsWith(diskHead) || diskHead === currentHead.slice(0, 8);
  if (!headAncestor) {
    const probe = cp.spawnSync('git', ['merge-base', '--is-ancestor', diskHead, 'HEAD'],
      { cwd: root, encoding: 'utf8' });
    headAncestor = probe.status === 0;
  }
  must(headAncestor, '清单 builtFrom.head 应为当前 HEAD 或其祖先（产物生成基线）');
  must(diskBuiltFrom.split('/')[1] === currentBuiltFrom.split('/')[1],
    '清单 inputsDigest 与当前输入不符');
  const normalize = text => text.split(currentBuiltFrom).join(diskBuiltFrom);
  must(markdown === normalize(expected.markdown), 'Markdown 不是确定性产物');
  must(raw === normalize(JSON.stringify(expected.ledger, null, 2) + '\n'), '机器台账不是确定性产物');
  must(!markdown.includes('- [x]'), '清单不得预填任何勾选框');
  must(ledger.counts.prefilledCheckboxes === 0, '台账预填勾选框计数不为 0');
  must(fs.readFileSync(gen.OUT_MD).equals(Buffer.from(markdown, 'utf8')), 'Markdown 字节与文本不一致');

  must(ledger.tasks.length === 544 && expected.counts.tasks === 544,
    '任务总数应为 544，实际 ' + ledger.tasks.length);
  must(new Set(ledger.tasks.map(item => item.id)).size === ledger.tasks.length, '任务 ID 重复');
  for (const [prefix, count] of Object.entries(EXPECTED_PREFIX_COUNTS)) {
    const actual = ledger.tasks.filter(item => item.id.startsWith(prefix)).length;
    must(actual === count, prefix + ' 覆盖数应为 ' + count + '，实际 ' + actual);
  }
  must(ledger.counts.workPackages === model.wbs.workPackages.length &&
    ledger.counts.unitCards === model.wbs.unitCards.length &&
    ledger.counts.tools === model.wbs.toolCards.length &&
    ledger.counts.chainNodes === model.wbs.entryChains.reduce((n, chain) => n + chain.nodes.length, 0),
    '清单计数与 WBS 台账不一致');

  const missingPaths = [];
  for (const item of ledger.tasks) {
    for (const relative of item.evidencePaths) {
      if (isLocal(relative) && !fs.existsSync(path.join(root, relative))) {
        missingPaths.push(item.id + ':' + relative);
      }
    }
  }
  must(missingPaths.length === 0,
    '本地证据路径不存在: ' + missingPaths.slice(0, 6).join(' | '));

  for (const command of ledger.commands) {
    must(model.incremental.allowedCommands.includes(command),
      '清单登记了未授权命令: ' + command);
  }
  must(ledger.counts.commandEntries === 3, '命令登记条目应为 3');

  const htmlCache = new Map();
  const missingAnchors = [];
  for (const item of ledger.tasks) {
    if (!item.anchor) continue;
    const file = path.join(root, 'docs', 'reverse-plan', item.anchor.file);
    if (!fs.existsSync(file)) { missingAnchors.push(item.id + ':文件 ' + item.anchor.file); continue; }
    if (!htmlCache.has(file)) htmlCache.set(file, fs.readFileSync(file, 'utf8'));
    if (!htmlCache.get(file).includes('id="' + item.anchor.id + '"')) {
      missingAnchors.push(item.id + ':锚点 ' + item.anchor.file + '#' + item.anchor.id);
    }
  }
  must(missingAnchors.length === 0,
    '站点锚点缺失: ' + missingAnchors.slice(0, 8).join(' | '));

  // 产物基线 HEAD 可早于当前 HEAD；仅要求 inputsDigest 一致且清单写明产物基线。
  const builtFrom = model.builtFrom.head.slice(0, 8) + '/' + model.builtFrom.inputsDigest.slice(0, 12);
  must(ledger.builtFrom.split('/')[1] === builtFrom.split('/')[1] &&
    markdown.includes(ledger.builtFrom), 'builtFrom 与输入哈希不一致');
  must(model.pages.length === 19, '本清单不得改变站点页数');
  must(markdown.includes('不自动提交/推送') && markdown.includes('开始测试/验证') &&
    markdown.includes('不以合理设计填空'), '硬规则或授权门槛未写入清单');
  must(markdown.includes('45/971486') && markdown.includes('47/973202'), '开放差分未并列');
  must(markdown.includes('42/96=43.75%') && markdown.includes('41/96=42.71%'), '口径冲突未并列');

  const b1Static = model.incrementalLanding.b1Static;
  must(b1Static && b1Static.cardId === 'C03-INCREMENTAL-B1-STATIC-001' &&
    fs.existsSync(path.join(root, b1Static.evidenceCard)) &&
    b1Static.batchStatusUnchanged === 'B0 open / B1-B3 blocked' &&
    b1Static.commandsExecuted === 0 && b1Static.productChanges === 0,
    '第二批静态取证未回写或批次状态被误改');
  const b1Tasks = ledger.tasks.filter(item => item.id.startsWith('INC-B1-'));
  must(b1Tasks.length === 4 && b1Tasks.slice(0, 3).every(item => item.sourceMembers.length > 0),
    'B1-1/B1-2/B1-3 的准确成员未补齐到完整清单');

  console.log(JSON.stringify({
    markdownBytes: Buffer.byteLength(markdown, 'utf8'),
    ledgerBytes: Buffer.byteLength(raw, 'utf8'),
    tasks: ledger.tasks.length,
    sections: 10,
    evidenceReferences: ledger.evidencePaths.length,
    commands: ledger.commands.length,
    anchorsChecked: ledger.tasks.filter(item => item.anchor).length,
    prefilledCheckboxes: ledger.counts.prefilledCheckboxes,
    ledgerDoneStates: ledger.counts.ledgerDoneStates,
    builtFrom,
    deterministic: true
  }, null, 2));
}

verify();
