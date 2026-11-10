'use strict';
// 独立复算：不复用 build-reverse-plan.cjs 的断言，只从磁盘产物与机器台账重新核对一遍。
const fs = require('fs');
const path = require('path');
const crypto = require('crypto');
const vm = require('vm');
const cp = require('child_process');
const lib = require('./plan-model.cjs');
const build = require('./build-reverse-plan.cjs');
const diagramsLib = require('./plan-diagrams.cjs');

const { root, out, buildModel, verifyInputs } = lib;
const NEW_EXECUTION_PAGES = [
  '00-master-plan.html', '11-work-packages.html', '12-unit-cards.html',
  '13-tool-cards.html', '14-entry-chains.html', '15-target-source-tree.html',
  '16-ui-control-landing.html', '17-incremental-landing.html'
];
function expectedPages(model) {
  return model.pages.map(item => item.slug === 'index' ? 'index.html' : item.slug + '.html');
}

function read(relative) { return fs.readFileSync(path.join(out, relative), 'utf8'); }
function shaOf(relative) {
  return crypto.createHash('sha256').update(fs.readFileSync(path.join(root, relative))).digest('hex');
}
function ids(html) {
  const found = [];
  const re = /\sid="([^"]+)"/g;
  let match;
  while ((match = re.exec(html))) found.push(match[1]);
  return found;
}
function check(condition, message) {
  if (!condition) throw new Error('[verify-reverse-plan] ' + message);
}

function verify() {
  const model = buildModel();
  verifyInputs(model);
  const EXPECTED_PAGES = expectedPages(model);
  const EXPECTED_FILES = EXPECTED_PAGES.concat(['plan-data.json', 'assets/plan.css', 'assets/plan.js']);
  check(EXPECTED_PAGES.length === model.pages.length, '页面清单与文件名映射不一致');
  for (const file of NEW_EXECUTION_PAGES) {
    check(EXPECTED_PAGES.includes(file), '完整计划新增页面未登记: ' + file);
  }

  // 1. 产物齐全，且没有多余文件。
  for (const file of EXPECTED_FILES) {
    check(fs.existsSync(path.join(out, file)), '缺少产物: ' + file);
  }
  const actual = fs.readdirSync(out).filter(f => f.endsWith('.html')).sort()
    .concat(fs.readdirSync(path.join(out, 'assets')).map(f => 'assets/' + f).sort())
    .concat(fs.existsSync(path.join(out, 'plan-data.json')) ? ['plan-data.json'] : []).sort();
  check(actual.length === EXPECTED_FILES.length,
    '产物文件数量应为 ' + EXPECTED_FILES.length + '，实际 ' + actual.length);
  for (const file of EXPECTED_FILES) check(actual.includes(file), '产物清单缺少: ' + file);

  const pages = new Map(EXPECTED_PAGES.map(file => [file, read(file)]));
  const idsByPage = {};
  for (const [file, html] of pages) {
    const list = ids(html);
    check(new Set(list).size === list.length, '页面存在重复锚点: ' + file);
    idsByPage[file] = new Set(list);
  }

  // 2. 页面结构、导航互链、锚点与链接闭环、无外链。
  let anchors = 0;
  let externalRefs = 0;
  let totalBytes = 0;
  for (const file of EXPECTED_FILES) {
    const content = read(file);
    totalBytes += Buffer.byteLength(content, 'utf8');
    if (/https?:\/\//i.test(content)) externalRefs++;
    if (!file.endsWith('.html')) continue;
    const html = pages.get(file);
    check(Buffer.byteLength(html, 'utf8') < build.PAGE_LIMIT, '单页超过 400 KB: ' + file);
    check(html.startsWith('<!doctype html>'), '缺少 doctype: ' + file);
    check(html.includes('<html lang="zh-CN">'), '缺少中文语言标注: ' + file);
    check(/<nav\b/.test(html) && /<main\b/.test(html) && /<aside\b/.test(html), '缺少语义化结构: ' + file);
    check((html.split('aria-current="page"').length - 1) === 1, '当前页标记不唯一: ' + file);
    check(/<caption>/.test(html), '表格缺少 caption: ' + file);
    anchors += (html.match(/<h2 id="|<h3 id="/g) || []).length;

    const nav = /<ul class="nav-list">([\s\S]*?)<\/ul>/.exec(html);
    check(nav, '缺少 ' + model.pages.length + ' 项导航: ' + file);
    const navLinks = [...nav[1].matchAll(/href="([^"]+\.html)"/g)].map(match => match[1]);
    check(navLinks.length === model.pages.length,
      '导航项不是 ' + model.pages.length + ' 个: ' + file + ' (' + navLinks.length + ')');
    for (const target of EXPECTED_PAGES) check(navLinks.includes(target), '导航未链接 ' + target + ': ' + file);

    const linkRe = /(?:href|src)="([^"]*)"/g;
    let link;
    while ((link = linkRe.exec(html))) {
      const target = link[1];
      check(target.length > 0, '空链接: ' + file);
      const at = target.indexOf('#');
      const filePart = at < 0 ? target : target.slice(0, at);
      const hash = at < 0 ? '' : target.slice(at + 1);
      if (filePart) {
        const abs = path.resolve(out, filePart);
        check(fs.existsSync(abs), '链接目标不存在: ' + file + ' → ' + target);
        if (hash && filePart.endsWith('.html')) {
          const targetPage = path.basename(filePart);
          check(idsByPage[targetPage], '跨页锚点目标页面未解析: ' + targetPage);
          check(idsByPage[targetPage].has(hash), '跨页锚点不存在: ' + file + ' → ' + target);
        }
      } else if (hash) {
        check(idsByPage[file].has(hash), '页内锚点不存在: ' + file + ' → #' + hash);
      }
    }

    const scriptRe = /<script\b[^>]*>([\s\S]*?)<\/script>/g;
    let script;
    while ((script = scriptRe.exec(html))) {
      const body = script[1].trim();
      if (!body) continue;
      try { new vm.Script(body); }
      catch (error) { throw new Error('[verify-reverse-plan] 内联脚本语法错误: ' + file + ' :: ' + error.message); }
    }
  }
  check(totalBytes < build.SITE_LIMIT, '站点总量超过 2 MB: ' + totalBytes);
  check(externalRefs === 0, '发现 ' + externalRefs + ' 个含 http(s):// 的文件');

  const planJs = read('assets/plan.js');
  try { new vm.Script(planJs); }
  catch (error) { throw new Error('[verify-reverse-plan] assets/plan.js 语法错误: ' + error.message); }
  check(read('assets/plan.css').includes('--bg: #0B1423'), '样式资源不是计划站点样式');

  // 3. 搜索索引：内联、可解析、每条指向真实页面与锚点。
  const marker = 'window.PLAN_SEARCH_INDEX = ';
  const startAt = planJs.indexOf(marker);
  check(startAt >= 0, '缺少内联搜索索引');
  const endAt = planJs.indexOf(';', startAt + marker.length);
  check(endAt > startAt, '搜索索引语句不完整');
  let searchIndex;
  try { searchIndex = JSON.parse(planJs.slice(startAt + marker.length, endAt)); }
  catch (error) { throw new Error('[verify-reverse-plan] 搜索索引不是合法 JSON: ' + error.message); }
  check(Array.isArray(searchIndex) && searchIndex.length > 300, '搜索索引条目不足: ' + (searchIndex || []).length);
  for (const item of searchIndex) {
    check(pages.has(item.p), '搜索索引指向不存在的页面: ' + item.p);
    if (item.h) check(idsByPage[item.p].has(item.h.slice(1)), '搜索索引锚点不存在: ' + item.p + item.h);
    check(typeof item.t === 'string' && item.t.length > 0, '搜索索引缺少标题');
  }

  // 4. 96 单元与 162 工具各出现一次，且与台账身份完全一致。
  const toolsHtml = pages.get('08-tools-162.html');
  const toolKeys = [...toolsHtml.matchAll(/data-tool-key="([^"]+)"/g)].map(match => match[1]);
  check(toolKeys.length === 162, '工具行数应为 162，实际 ' + toolKeys.length);
  check(new Set(toolKeys).size === 162, '工具行存在重复 key');
  check(model.tools.every(tool => toolKeys.includes(tool.key)), '台账工具未全部出现在页面');
  check((toolsHtml.match(/data-tool-row/g) || []).length === 162, '可筛选工具行数不是 162');

  const gatesHtml = pages.get('09-gates-evidence.html');
  const unitIds = [...gatesHtml.matchAll(/data-unit-id="([^"]+)"/g)].map(match => match[1]);
  check(unitIds.length === 96, '单元行数应为 96，实际 ' + unitIds.length);
  check(new Set(unitIds).size === 96, '单元行存在重复 ID');
  check(model.frameworkUnits.every(unit => unitIds.includes(unit.id)), '96 具名单元未全部出现在页面');

  // 4b. 完整计划：WBS 工作包 / 单元卡 / 工具卡 / 闭合链 / 总纲 / 旧计划处置。
  const wbs = model.wbs;
  const wpHtml = pages.get('11-work-packages.html');
  const wpHits = [...wpHtml.matchAll(/data-work-package="([^"]+)"/g)].map(match => match[1]);
  check(wpHits.length === 32 && new Set(wpHits).size === 32, 'WBS 工作包卡不是 32 张或存在重复');
  check(wbs.workPackages.every(pkg => wpHits.includes(pkg.id)), '工作包未全部出现在 WBS 页');
  for (const pkg of wbs.workPackages) {
    for (const dep of pkg.dependencies) {
      check(idsByPage['11-work-packages.html'].has('wp-' + dep), '依赖锚点不存在: ' + pkg.id + ' → ' + dep);
    }
  }
  const ucHtml = pages.get('12-unit-cards.html');
  const ucHits = [...ucHtml.matchAll(/data-unit-card-id="([^"]+)"/g)].map(match => match[1]);
  check(ucHits.length === 96 && new Set(ucHits).size === 96, '框架单元任务卡不是 96 张或存在重复');
  check(wbs.unitCards.every(card => ucHits.includes(card.id)), '单元任务卡身份与台账不符');
  for (const field of wbs.cardTemplates.unit.requiredFields) {
    check(ucHtml.includes('>' + field + '<'), '单元卡模板缺少必填字段: ' + field);
  }
  const tcHtml = pages.get('13-tool-cards.html');
  const tcHits = [...tcHtml.matchAll(/data-tool-card-key="([^"]+)"/g)].map(match => match[1]);
  check(tcHits.length === 162 && new Set(tcHits).size === 162, '目录工具任务卡不是 162 张或存在重复');
  check(wbs.toolCards.every(card => tcHits.includes(card.key)), '工具任务卡身份与台账不符');
  for (const field of wbs.cardTemplates.tool.requiredFields) {
    check(tcHtml.includes('>' + field + '<'), '工具卡模板缺少必填字段: ' + field);
  }
  const ecHtml = pages.get('14-entry-chains.html');
  check((ecHtml.match(/data-chain-node/g) || []).length === 48, '闭合链节点不是 48 个');
  for (const chain of wbs.entryChains) {
    for (const node of chain.nodes) {
      check(idsByPage['14-entry-chains.html'].has('chain-' + chain.gateId + '-' + node.stageId),
        '链节点锚点缺失: ' + chain.gateId + '/' + node.stageId);
    }
  }
  const masterHtml = pages.get('00-master-plan.html');
  const govHtml = pages.get('10-governance.html');
  check(masterHtml.includes(model.masterPlan.authority), '权威总纲缺少唯一执行口径声明');
  for (const legacy of wbs.legacyPlans) {
    check(masterHtml.includes(legacy.path) && govHtml.includes(legacy.path),
      '旧计划处置登记缺失: ' + legacy.path);
  }
  check(wbs.dependencyOrder.length === 32 && new Set(wbs.dependencyOrder).size === 32,
    '依赖接力顺序不完整');

  // 4c. 目标源码树：节点覆盖、状态标注与「待建不落盘」独立复算。
  const targetTree = model.targetTree;
  const treePage = pages.get('15-target-source-tree.html');
  const pendingHits = [...treePage.matchAll(/id="pending-[^"]+"/g)].length;
  check(pendingHits === targetTree.nodes.filter(node => node.status !== 'exists').length,
    '待建/未知目录表格行数不符: ' + pendingHits);
  for (const node of targetTree.nodes) {
    const onDisk = fs.existsSync(path.join(root, node.path));
    check(onDisk === (node.status === 'exists'), '树节点磁盘状态与登记不符: ' + node.path);
    if (node.status === 'planned') {
      check(node.evidenceType === 'independent-implementation-planned' && node.unknowns.length > 0,
        '待建节点缺少计划编排标注或边界说明: ' + node.path);
    }
    if (node.status === 'unknown') {
      check(node.evidenceType === 'unknown', '未知节点证据类型不符: ' + node.path);
    }
  }
  check(targetTree.policy.noDiskCreation === true, '目标树必须声明待建不落盘');
  check(treePage.includes('independent-implementation-planned') && treePage.includes('待建不落盘'),
    '树页未显示计划编排与不落盘口径');
  const wpPageHtml = pages.get('11-work-packages.html');
  check(wpPageHtml.includes('id="wp-tree"') && wpPageHtml.includes('15-target-source-tree.html'),
    '工作包页缺少目标源码树入口章节或互链');

  // 4d. 两张内联 SVG 图：独立复算布局，逐字比对页面产物。
  const rebuiltDiagrams = diagramsLib.buildDiagrams(model);
  const archHtml = pages.get('02-architecture.html');
  const treeHtml = pages.get('15-target-source-tree.html');
  check(archHtml.includes(rebuiltDiagrams.architecture.html), '架构分层图不是确定性产物');
  check(treeHtml.includes(rebuiltDiagrams.dependency.html), '工程依赖图不是确定性产物');
  check(!treeHtml.includes('data-diagram="source-tree"'), '源码树已改为真实文件树，不应再有 SVG 源码树');
  check((archHtml.match(/<svg/g) || []).length === 1 && (treeHtml.match(/<svg/g) || []).length === 1,
    '两页 SVG 数量应为 1 + 1');
  check((archHtml.match(/data-diagram-layer=/g) || []).length === 8, '架构图未覆盖 8 层');
  const diagramNodes = (treeHtml.match(/data-diagram-node=/g) || []).length;
  check(diagramNodes === rebuiltDiagrams.counts.dependencyNodes, '依赖图节点数应为 29，实际 ' + diagramNodes);
  const diagramEdges = (treeHtml.match(/data-diagram-edge/g) || []).length;
  check(diagramEdges === rebuiltDiagrams.counts.dependencyEdges, '依赖图边数不是 62');
  check(archHtml.includes('data-diagram-legend') && treeHtml.includes('data-diagram-legend'),
    '两页缺少图例');
  check(archHtml.includes('未归层'), '架构图图例未登记未归层边口径');
  check(!/foreignObject|xmlns=/i.test(archHtml + treeHtml), 'SVG 含外部命名空间或 foreignObject');

  // 4e. 框架层源码树：15 工程 + 根文件 / 派生文件与目录 / tree 树线。
  const rebuiltFileTree = require('./plan-file-tree.cjs').buildFileTree(model, path.join(root, 'src'), model.purposeMap);
  check(rebuiltFileTree.counts.projects === 16 && rebuiltFileTree.counts.toolProjects === 0,
    '框架层树应为 16 个条目（15 工程 + 根文件）且不含工具工程');
  check((treeHtml.match(/data-file-project=/g) || []).length === 16, '页面框架层条目不是 16');
  const fileSum = [...treeHtml.matchAll(/data-file-count="(\d+)"/g)]
    .reduce((n, match) => n + Number(match[1]), 0);
  check(fileSum === rebuiltFileTree.counts.files, '框架层文件数不符，实际 ' + fileSum);
  const dirSum = [...treeHtml.matchAll(/data-file-dir-count="(\d+)"/g)]
    .reduce((n, match) => n + Number(match[1]), 0);
  check(dirSum === rebuiltFileTree.counts.dirs, '框架层目录数不符，实际 ' + dirSum);
  check(treeHtml.includes('├── ') && treeHtml.includes('└── '), '框架层树缺少 tree 树线字符');
  check(treeHtml.includes(rebuiltFileTree.groups[0].html), '框架层树不是确定性产物');
  check((treeHtml.split('data-file-purpose="').length - 1) === 16 &&
    !treeHtml.includes('data-file-purpose=""'), '框架层用途说明应为 16 条且非空');
  check((treeHtml.split('class="file-purpose"').length - 1) === 16, '用途说明未出现在 16 个 summary 行');
  check((treeHtml.split('data-file-kind="framework"').length - 1) === 16 &&
    (treeHtml.split('data-file-kind="tool"').length - 1) === 0,
    '框架层应为 16（含根文件）、工具层不应再有自研工程树');

  // 4f. 工具层：162 个待逆向实现工具 / 14 分类 / 161 未建立 + 1 partial。
  const rebuiltToolLayer = require('./plan-tool-layer.cjs').buildToolLayer(model);
  const toolLayerKeys = [...treeHtml.matchAll(/data-tool-layer-key="([^"]+)"/g)].map(match => match[1]);
  check(toolLayerKeys.length === 162 && new Set(toolLayerKeys).size === 162,
    '工具层清单应为 162 个唯一工具，实际 ' + toolLayerKeys.length);
  check((treeHtml.match(/data-tool-layer-category=/g) || []).length === 162, '工具行必须带分类属性');
  check((treeHtml.split('data-tool-layer-state="not-established"').length - 1) === 161,
    '未建立工具应为 161');
  check((treeHtml.split('data-tool-layer-state="partial"').length - 1) === 1, 'partial 工具应为 1');
  check(treeHtml.includes('13-tool-cards.html#toolcard-'), '工具行缺少任务卡链接');
  check(treeHtml.includes(rebuiltToolLayer.html), '工具层清单不是确定性产物');

  // 4g. 原厂目标源码树：25 程序集 / 28 反编译树归并 / 162 工具目录证据。
  const rebuiltOriginalTree = require('./plan-original-tree.cjs').buildOriginalTree(model, root);
  const originalIds = [...treeHtml.matchAll(/data-original-assembly="([^"]+)"/g)].map(match => match[1]);
  check(originalIds.length === 25 && new Set(originalIds).size === 25,
    '原厂程序集树应为 25 个唯一节点，实际 ' + originalIds.length);
  check((treeHtml.match(/data-original-tree=/g) || []).length === 25, '原厂 canonical 树属性应为 25 个');
  check((treeHtml.match(/class="original-tree"/g) || []).length === 25, '原厂树线块应为 25 个');
  check((treeHtml.match(/data-tool-layer-sha=/g) || []).length === 162,
    '162 个工具目录节点必须带 SHA-256 标识');
  check(treeHtml.includes('原厂目标') && treeHtml.includes('自研现状'), '页面缺少双口径标题');
  check(treeHtml.includes(rebuiltOriginalTree.html), '原厂源码树不是确定性产物');
  check(rebuiltOriginalTree.counts.treePasses === 28 &&
    rebuiltOriginalTree.counts.duplicatePasses === 3 &&
    rebuiltOriginalTree.counts.uniqueCs === model.metrics.original.cs &&
    rebuiltOriginalTree.counts.uniqueLines === model.metrics.original.lines,
    '原厂树归并与去重源码量与台账不符');

  // 4h. 主界面控件落地清单：17 控件 / 4 类 / 8 门 / S0 前置 / 三级验收。
  const ui = model.uiLanding;
  const landingHtml = pages.get('16-ui-control-landing.html');
  const uiIds = [...landingHtml.matchAll(/data-ui-control="([^"]+)"/g)].map(match => match[1]);
  check(uiIds.length === 17 && new Set(uiIds).size === 17, '控件任务卡应为 17 个且唯一');
  check(uiIds.join(',') === ui.controls.map(control => control.id).join(','),
    '控件 ID 与台账顺序不符');
  check((landingHtml.match(/id="uigroup-/g) || []).length === 4, '控件分组应为 4 个');
  for (const id of ['ui-p0', 'ui-acceptance', 'ui-gates', 'ui-cards', 'ui-links']) {
    check(landingHtml.includes('id="' + id + '"'), '落地清单缺少章节: ' + id);
  }
  check(landingHtml.includes('971486') && landingHtml.includes('973202'),
    '落地清单未并列开放差分两侧数值');
  check(landingHtml.includes('unavailable-on-current-host'), '落地清单未登记 REA provider 不可用');
  check((landingHtml.match(/id="acceptance-/g) || []).length === 3, '三级验收应为 3 行');
  check((landingHtml.split('data-task-state="blocked"').length - 1) === 17,
    'S0 前置未闭时 17 个控件应全部 blocked');
  check(ui.counts.acceptanceDone === 0 && ui.counts.unknownSegments === 102,
    '验收不得预填、六段链应全为 unknown');
  check(new Set(ui.controls.flatMap(control => control.gateIds)).size === 8,
    '控件应覆盖 E0–E7 共 8 门');

  // 4i. 渐进落地路线：批次/步骤/命令与页面一致，且零预填。
  const inc = model.incremental;
  const incHtml = pages.get('17-incremental-landing.html');
  const incIds = [...incHtml.matchAll(/data-ui-incremental="([^"]+)"/g)].map(match => match[1]);
  check(incIds.length === 13 && new Set(incIds).size === 13, '渐进步骤卡应为 13 个且唯一');
  check(incIds.join(',') === inc.steps.map(step => step.id).join(','), '步骤顺序与台账不符');
  check((incHtml.match(/id="inc-batch-/g) || []).length === 4, '批次锚点应为 4 个');
  check((incHtml.match(/id="inc-step-/g) || []).length === 13, '步骤锚点应为 13 个');
  for (const id of ['inc-timeline', 'inc-b0', 'inc-cards', 'inc-commands', 'inc-links']) {
    check(incHtml.includes('id="' + id + '"'), '渐进路线页缺少章节: ' + id);
  }
  check(incHtml.includes('971486') && incHtml.includes('973202'),
    '渐进路线页未并列开放差分两侧数值');
  check(incHtml.includes(inc.allowedCommands[0]) && incHtml.includes('待执行'),
    '渐进路线页未登记真实复跑命令或缺少待执行标注');
  check((incHtml.split('data-task-state="not_started"').length - 1) === 13,
    '13 个步骤应全部为 not_started');
  check(inc.counts.done === 0 && inc.counts.stepsWithBlockers === 10 &&
    inc.batches[0].status === 'open' && inc.batches.slice(1).every(batch => batch.status === 'blocked'),
    '批次阻塞关系或零预填状态不符');
  for (const step of inc.steps) {
    for (const command of step.commands) {
      check(inc.allowedCommands.includes(command), '步骤登记了未授权命令: ' + step.id);
    }
  }
  const batch0 = model.incrementalLanding;
  check(batch0 && batch0.cardId === 'C03-INCREMENTAL-B0B1-001' &&
    batch0.commandsExecuted === 1 && batch0.productChanges.length === 0 &&
    fs.existsSync(path.join(root, batch0.evidenceCard)) &&
    fs.existsSync(path.join(root, batch0.humanReview)),
    '第一批证据卡或零执行/零改动状态不符');
  check(incHtml.includes(batch0.cardId) && incHtml.includes('第一批已执行'),
    '渐进路线页未回写第一批静态取证结果');

  // 4j. 第二批 B1 静态取证：证据卡、成员补齐与批次仍阻塞。
  const b1 = model.incrementalLanding.b1Static;
  check(b1 && b1.cardId === 'C03-INCREMENTAL-B1-STATIC-001' &&
    fs.existsSync(path.join(root, b1.evidenceCard)) &&
    fs.existsSync(path.join(root, b1.humanReview)) &&
    b1.sourceFiles === 8 && b1.solutionFiles === 3 &&
    b1.commandsExecuted === 0 && b1.productChanges === 0 &&
    b1.batchStatusUnchanged === 'B0 open / B1-B3 blocked',
    '第二批静态证据卡或批次状态不符');
  check(incHtml.includes(b1.cardId) && incHtml.includes('B1 仍 blocked'),
    '第 19 页未回写第二批结果或 B1 阻塞说明');
  check(inc.batches[1].status === 'blocked' && inc.batches[1].steps[0].sourceMembers.length > 0,
    'B1 应仍 blocked 且准确成员已补齐');

  // 4k. 第三批 B1 输出名契约：两个缺口闭合、边界明确、批次仍阻塞。
  const b3 = model.incrementalLanding.b1Static.b1Contracts;
  check(b3 && b3.cardId === 'C03-INCREMENTAL-B1-CONTRACTS-001' &&
    fs.existsSync(path.join(root, b3.evidenceCard)) &&
    fs.existsSync(path.join(root, b3.humanReview)) &&
    b3.gapsClosed.length === 2 &&
    b1.outputNameComparison.closedBy === b3.cardId,
    '第三批契约证据卡或闭合状态不符');
  const contractsCard = JSON.parse(fs.readFileSync(path.join(root, b3.evidenceCard), 'utf8'));
  check(contractsCard.compositeOutputContract.exactMatch === true &&
    contractsCard.compositeOutputContract.leafPaths.length === 9 &&
    contractsCard.compositeOutputContract.reportedCompositeNames.length === 9 &&
    contractsCard.compositeOutputContract.missingFromBuilt.length === 0 &&
    contractsCard.compositeOutputContract.extraBuilt.length === 0 &&
    contractsCard.groupWidthContract.port === 'Width',
    '9 个复合输出名或 Group width 契约核对不符');
  check(b1.outputNameComparison.boundary.includes('not original native ABI'),
    '契约边界必须明确排除原厂 ABI 与 parity');
  check(incHtml.includes(b3.cardId) && incHtml.includes('managed projection，非原厂 ABI'),
    '第 19 页未回写第三批契约结果');
  check(inc.batches[1].status === 'blocked',
    '输出名契约闭合不得解除 B1 阻塞');

  // 5. plan-data.json：计数、输入哈希、builtFrom 与当前工作树一致。
  const data = JSON.parse(read('plan-data.json'));
  check(data.schemaVersion === 1, 'plan-data.json schemaVersion 不符');
  // 产物记录的是生成时的基线 HEAD；提交后当前 HEAD 会前进，因此要求「产物基线是 HEAD 的祖先或相等」。
  const recordedHead = data.builtFrom.head;
  const currentHead = model.builtFrom.head;
  let headAncestor = recordedHead === currentHead;
  if (!headAncestor) {
    const probe = cp.spawnSync('git', ['merge-base', '--is-ancestor', recordedHead, 'HEAD'],
      { cwd: root, encoding: 'utf8' });
    headAncestor = probe.status === 0;
  }
  check(headAncestor, 'builtFrom.head 应为当前 HEAD 或其祖先（产物生成基线）');
  check(data.builtFrom.inputsDigest === model.builtFrom.inputsDigest, 'inputsDigest 与当前输入不符');
  for (const item of data.inputs.concat(data.supporting)) {
    check(shaOf(item.path) === item.sha256, '输入台账哈希已变化: ' + item.path);
  }
  check(data.counts.pages === model.pages.length && data.counts.units === 96 && data.counts.tools === 162,
    'plan-data.json 分母不符');
  check(data.counts.categories === 14 && data.counts.assemblies === 25 && data.counts.currentProjects === 29,
    'plan-data.json 基线分母不符');
  check(data.counts.workPackages === 32 && data.counts.unitCards === 96 &&
    data.counts.toolCards === 162 && data.counts.chainNodes === 48 &&
    data.counts.legacyPlans === 4, 'plan-data.json WBS 分母不符');
  check(data.wbs && data.wbs.authority === model.masterPlan.authority &&
    data.wbs.dependencyOrder.length === 32 &&
    data.wbs.unitCardIds.length === 96 && data.wbs.toolCardKeys.length === 162 &&
    data.wbs.entryChainGates.length === 8, 'plan-data.json WBS 摘要不完整');
  check(data.wbs.dependencyOrder.join(',') === wbs.dependencyOrder.join(','), 'WBS 接力顺序与派生结果不一致');
  check(data.targetTree && data.targetTree.counts.nodes === targetTree.counts.nodes &&
    data.targetTree.policy.noDiskCreation === true &&
    data.targetTree.nodeIds.length === targetTree.counts.nodes &&
    data.targetTree.edgeCount === targetTree.counts.edges,
    'plan-data.json 目标树摘要与派生结果不一致');
  check(data.counts.targetTreeNodes === targetTree.counts.nodes &&
    data.counts.targetTreePlanned === targetTree.counts.planned &&
    data.counts.targetTreeUnknown === targetTree.counts.unknown,
    'plan-data.json 目标树计数不符');
  check(data.diagrams && data.diagrams.counts.diagrams === 2 &&
    data.diagrams.counts.layerPairs === rebuiltDiagrams.counts.layerPairs &&
    data.diagrams.counts.layerEdges === rebuiltDiagrams.counts.layerEdges &&
    data.diagrams.counts.unresolvedLayerEdges === rebuiltDiagrams.counts.unresolvedLayerEdges &&
    data.diagrams.layerPairs.length === rebuiltDiagrams.counts.layerPairs,
    'plan-data.json 图摘要与独立复算不一致');
  check(data.fileTree && data.fileTree.counts.projects === 16 &&
    data.fileTree.counts.files === rebuiltFileTree.counts.files &&
    data.fileTree.counts.dirs === rebuiltFileTree.counts.dirs &&
    data.fileTree.counts.frameworkProjects === 15 &&
    data.fileTree.counts.toolProjects === 0,
    'plan-data.json 文件树摘要与独立复算不一致');
  check(data.toolLayer && data.toolLayer.counts.tools === 162 &&
    data.toolLayer.counts.categories === 14 &&
    data.toolLayer.counts.notEstablished === 161 &&
    data.toolLayer.counts.partial === 1,
    'plan-data.json 工具层摘要不符');
  check(data.originalTree && data.originalTree.counts.assemblies === 25 &&
    data.originalTree.counts.treePasses === 28 &&
    data.originalTree.counts.duplicatePasses === 3 &&
    data.originalTree.counts.uniqueCs === model.metrics.original.cs,
    'plan-data.json 原厂树摘要不符');
  check(data.uiLanding && data.uiLanding.counts.controls === 17 &&
    data.uiLanding.counts.groups === 4 && data.uiLanding.counts.gates === 8 &&
    data.uiLanding.counts.blocked === 17 && data.uiLanding.counts.acceptanceDone === 0 &&
    data.uiLanding.controlIds.length === 17 &&
    data.counts.uiLandingControls === 17 && data.counts.pages === model.pages.length,
    'plan-data.json 控件落地摘要不符');
  check(data.incremental && data.incremental.counts.batches === 4 &&
    data.incremental.counts.steps === 13 && data.incremental.counts.done === 0 &&
    data.incremental.stepIds.length === 13 &&
    data.incremental.allowedCommands[0] === inc.allowedCommands[0] &&
    data.counts.incrementalBatches === 4 && data.counts.incrementalSteps === 13 &&
    data.counts.pages === 19 &&
    data.incremental.batch0CardId === batch0.cardId &&
    data.incremental.batch2CardId === b1.cardId && data.incremental.batch2SourceFiles === 8 &&
    data.incremental.batch3CardId === b3.cardId &&
    data.incremental.commandsExecuted === 1 && data.incremental.productChanges === 0,
    'plan-data.json 渐进路线摘要不符');
  check(data.memberSpecs && data.memberSpecs.assemblies === 25 &&
    data.memberSpecs.canonicalCsFiles === 2614 &&
    data.toolSpecs && data.toolSpecs.tools === 162 &&
    data.toolSpecs.evidenceFiles === 2015 &&
    fs.existsSync(path.join(root, data.memberSpecs.ledger)) &&
    fs.existsSync(path.join(root, data.toolSpecs.ledger)),
    'plan-data.json 规格台账摘要不符');

  // 5d. 启动链路 + 整屏空载初始态工作面。
  const su = model.startupInitialUi;
  check(su && su.counts.startupSteps === 11 && su.counts.controls === 10 &&
    su.counts.sixSegmentUnknown === 126 && su.counts.acceptanceDone === 0 &&
    su.counts.layoutEdgesTotal === 30 && su.counts.unlinkedLayoutEdges === 8,
    '启动态派生计数不符');
  for (const relative of [
    'tests/ArchitectureRecovery/startup-initial-ui.map.json',
    'docs/STARTUP-INITIAL-UI-TASK-LIST.md',
    'docs/reverse/C03-startup-initial-ui-evidence.md',
    'analysis/full-audit/batch1-20261107/C03/c03-startup-initial-ui-20261110.json'
  ]) {
    check(fs.existsSync(path.join(root, relative)), '缺少启动态交付: ' + relative);
  }
  const suCard = JSON.parse(fs.readFileSync(path.join(root,
    'analysis/full-audit/batch1-20261107/C03/c03-startup-initial-ui-20261110.json'), 'utf8'));
  check(suCard.cardId === 'C03-STARTUP-INITIAL-UI-20261110' &&
    suCard.commands.length === 0 && suCard.productChanges.length === 0 &&
    suCard.counts.acceptanceDone === 0 && suCard.evidence.length >= 50,
    '启动态证据卡或零执行/零改动边界不符');
  const suList = fs.readFileSync(path.join(root, 'docs/STARTUP-INITIAL-UI-TASK-LIST.md'), 'utf8');
  check(!suList.includes('- [x]') && (suList.match(/^- \[ \] /gm) || []).length === 21,
    '启动态清单应含 21 个空勾选框');
  check(incHtml.includes('C03-STARTUP-INITIAL-UI-20261110') &&
    incHtml.includes('STARTUP-INITIAL-UI-TASK-LIST.md'),
    '第 19 页缺少启动态 notice');
  check(data.startupInitialUi && data.startupInitialUi.steps === 11 &&
    data.startupInitialUi.controls === 10 &&
    data.startupInitialUi.acceptanceDone === 0 &&
    data.startupInitialUi.stateScope === '空载初始态' &&
    data.supporting.length === 8 && data.counts.pages === 19,
    'plan-data.json 启动态摘要或补充源/页数不符');
  const statusLedger = JSON.parse(fs.readFileSync(path.join(root,
    'docs/VM-LOCAL-RESTORATION-STATUS.json'), 'utf8'));
  const statusStartup = statusLedger.incrementalLanding &&
    statusLedger.incrementalLanding.startupInitialUi;
  check(statusStartup && statusStartup.cardId === suCard.cardId &&
    statusStartup.startupSteps === 11 && statusStartup.controls === 10 &&
    statusStartup.commandsExecuted === 0 && statusStartup.productChanges === 0 &&
    statusStartup.batchStatusUnchanged === 'B0 open / B1-B3 blocked',
    '状态台账未回写启动态摘要或批次状态被误改');

  // 5c. 完整任务清单：与同一工作树同源，且不改变站点页数。
  const listMd = path.join(root, 'docs', 'COMPLETE-REVERSE-TASK-LIST.md');
  const listJson = path.join(root, 'tests', 'ArchitectureRecovery', 'complete-task-list.map.json');
  check(fs.existsSync(listMd) && fs.existsSync(listJson), '完整任务清单或其机器台账不存在');
  const listLedger = JSON.parse(fs.readFileSync(listJson, 'utf8'));
  const listText = fs.readFileSync(listMd, 'utf8');
  const listBuiltFrom = model.builtFrom.head.slice(0, 8) + '/' + model.builtFrom.inputsDigest.slice(0, 12);
  // 产物基线可在提交后早于当前 HEAD；只要求 inputsDigest 一致且 head 为 HEAD 祖先。
  check(listLedger.builtFrom.split('/')[1] === listBuiltFrom.split('/')[1] &&
    listText.includes(listLedger.builtFrom),
    '完整任务清单 inputsDigest 与站点输入哈希不一致');
  check(listLedger.counts.tasks === 544 && listLedger.counts.prefilledCheckboxes === 0 &&
    listLedger.counts.tools === 162 && listLedger.counts.unitCards === 96 &&
    listLedger.counts.chainNodes === 48,
    '完整任务清单覆盖计数或零预填不符');
  check(!listText.includes('- [x]'), '完整任务清单不得预填勾选框');
  check(model.pages.length === 19, '完整任务清单不得改变站点页数');
  for (const field of wbs.cardTemplates.unit.requiredFields) {
    check(data.wbs.cardRequiredFields.unit.includes(field), '机器摘要缺少单元卡必填字段: ' + field);
  }
  check(data.stages.length === 10 && data.priorities.length === 9 && data.gaps.length === 8, '阶段/优先级/遗漏面数量不符');
  check(Array.isArray(data.exceptions) && data.exceptions.length === 0, '并行例外登记异常');
  check(data.pages.length === model.pages.length && data.pages.every(p => pages.has(p.file)),
    'plan-data.json 页面清单与磁盘不符');
  check(data.pages.map(p => p.file).join(',') === EXPECTED_PAGES.join(','), '页面清单顺序与编辑源不一致');

  // 5b. 第一阶段落地状态：台账引用、机器摘要与切片页三者一致。
  const phase1 = model.phase1Landing;
  for (const relative of [phase1.evidenceCard, phase1.evidenceReview, phase1.gateCheck, phase1.humanReview]) {
    check(fs.existsSync(path.join(root, relative)), '第一阶段台账引用不存在: ' + relative);
  }
  check(data.phase1Landing && data.phase1Landing.status === phase1.status &&
    data.phase1Landing.s0GateStatus === phase1.s0GateStatus &&
    data.phase1Landing.productChanges === 0 &&
    data.phase1Landing.diffPreserved.delta.blobCount === phase1.diffPreserved.delta.blobCount,
    'plan-data.json 第一阶段摘要与台账不一致');
  const sliceHtml = pages.get('04-vertical-slice.html');
  check(sliceHtml.includes('slice-phase1') && sliceHtml.includes(phase1.status) &&
    sliceHtml.includes(phase1.s0GateStatus) && sliceHtml.includes(phase1.evidenceCard),
    '切片页未同步第一阶段落地状态');
  check(sliceHtml.includes(String(phase1.diffPreserved.originalVm.blobCount)) &&
    sliceHtml.includes(String(phase1.diffPreserved.directSdkAndLocal.blobCount)),
    '切片页未保留两侧差分数值');
  for (const [file, anchorsInData] of Object.entries(data.pageAnchors)) {
    check(pages.has(file), 'pageAnchors 指向不存在的页面: ' + file);
    for (const anchor of anchorsInData) check(idsByPage[file].has(anchor), 'pageAnchors 锚点缺失: ' + file + '#' + anchor);
  }

  // 6. 口径冲突与身份差异在页面上显式可见。
  check(pages.get('index.html').includes('42/96 = 43.75%') && pages.get('index.html').includes('41/96 = 42.71%'),
    '索引页未并列展示 41/96 与 42/96');
  check(pages.get('10-governance.html').includes(model.conflict), '治理页未登记口径冲突');
  check(pages.get('08-tools-162.html').includes('PositionCorrection'), '162 工具页未登记批次身份差异');
  check(pages.get('04-vertical-slice.html').includes(String(model.slice.originalVm.count)) &&
    pages.get('04-vertical-slice.html').includes(String(model.slice.directSdk.count)), '切片页缺少两侧数值差分');

  // 7. 确定性：同一模型重跑一次，逐字节比对磁盘产物。
  // 产物记录生成时基线 HEAD；提交后当前 HEAD 前进，比对时把当前 HEAD 归一化到产物基线，
  // 其余内容仍逐字节严格比对（祖先关系已在第 5 节校验）。
  const artifactHead = data.builtFrom.head;
  const liveHead = model.builtFrom.head;
  const normalizeHead = text => text
    .split(liveHead).join(artifactHead)
    .split(liveHead.slice(0, 8)).join(artifactHead.slice(0, 8));
  const rebuilt = build.renderAll(model);
  check(rebuilt.files.length === EXPECTED_FILES.length, '重跑产物数量不一致');
  for (const file of rebuilt.files) {
    check(fs.existsSync(path.join(out, file.relative)), '重跑产物在磁盘缺失: ' + file.relative);
    check(normalizeHead(read(file.relative)) === normalizeHead(file.content),
      '重跑产物字节不一致: ' + file.relative);
  }
  verifyInputs(model);

  return {
    pages: EXPECTED_PAGES.length,
    files: EXPECTED_FILES.length,
    anchors,
    unitsCovered: unitIds.length,
    toolsCovered: toolKeys.length,
    workPackages: wpHits.length,
    unitCards: ucHits.length,
    toolCards: tcHits.length,
    chainNodes: (ecHtml.match(/data-chain-node/g) || []).length,
    targetTreeNodes: targetTree.counts.nodes,
    diagrams: rebuiltDiagrams.counts.diagrams,
    diagramNodes,
    diagramEdges,
    fileTreeProjects: rebuiltFileTree.counts.projects,
    fileTreeFiles: rebuiltFileTree.counts.files,
    fileTreeDirs: rebuiltFileTree.counts.dirs,
    toolLayerTools: rebuiltToolLayer.counts.tools,
    toolLayerCategories: rebuiltToolLayer.counts.categories,
    toolLayerNotEstablished: rebuiltToolLayer.counts.notEstablished,
    toolLayerPartial: rebuiltToolLayer.counts.partial,
    originalAssemblies: rebuiltOriginalTree.counts.assemblies,
    originalTreePasses: rebuiltOriginalTree.counts.treePasses,
    toolDirShas: rebuiltToolLayer.counts.tools,
    uiLandingControls: ui.counts.controls,
    uiLandingGroups: ui.counts.groups,
    uiLandingBlocked: ui.counts.blocked,
    incrementalBatches: inc.counts.batches,
    incrementalSteps: inc.counts.steps,
    incrementalDone: inc.counts.done,
    batch0Card: batch0.cardId,
    legacyPlans: wbs.legacyPlans.length,
    searchIndexEntries: searchIndex.length,
    externalRefs,
    inputsStable: true,
    deterministic: true,
    bytes: totalBytes,
    builtFrom: model.builtFrom.head.slice(0, 8) + '/' + model.builtFrom.inputsDigest.slice(0, 12)
  };
}

if (require.main === module) {
  console.log(JSON.stringify(verify(), null, 2));
}
module.exports = { verify };
