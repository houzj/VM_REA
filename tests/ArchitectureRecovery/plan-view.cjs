'use strict';
// 逆向计划站点统一页面骨架。所有页面共用 head / 导航 / 页内目录 / 页脚，正文由 build-reverse-plan.cjs 拼装。
const ESC = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' };

function esc(value) {
  return String(value === null || value === undefined ? '' : value).replace(/[&<>"']/g, c => ESC[c]);
}
function fmt(value) {
  if (value === null || value === undefined || value === '') return '—';
  const text = String(value);
  const parts = text.split('.');
  parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ',');
  return parts.join('.');
}
function pct(value) { return value === null || value === undefined ? '—' : esc(String(value)) + '%'; }
function code(value) { return '<code>' + esc(value) + '</code>'; }
function mono(value) { return '<span class="mono">' + esc(value) + '</span>'; }
function pathHtml(value) { return '<span class="path">' + esc(value) + '</span>'; }
function hrefFor(slug, hash) {
  const file = slug === 'index' ? 'index.html' : slug + '.html';
  return file + (hash ? '#' + encodeURIComponent(hash).replace(/%2D/g, '-').replace(/%5F/g, '_') : '');
}
function slugId(prefix, value) {
  return prefix + '-' + String(value).replace(/[^A-Za-z0-9_-]+/g, '-').replace(/^-+|-+$/g, '');
}

const STATUS_STATE = {
  done: 'done', closed: 'done', completed: 'done', passed: 'done',
  'source-confirmed': 'done', 'runtime-confirmed': 'done',
  partial: 'partial', running: 'partial', active: 'partial', 'in-progress': 'partial',
  'independent-implementation': 'partial', unverified: 'partial', open: 'partial',
  blocked: 'blocked', failed: 'blocked', absent: 'blocked', unknown: 'blocked',
  not_started: 'idle', idle: 'idle', 'not-started': 'idle'
};
function statusState(value) { return STATUS_STATE[String(value || '').toLowerCase()] || 'idle'; }
function chip(state, label) {
  return '<span class="chip" data-state="' + esc(state) + '">' + esc(label) + '</span>';
}
function statusChip(value, label) { return chip(statusState(value), label === undefined ? value : label); }
function chips(items) { return '<div class="chips">' + items.map(c => c.html ? c.html : chip(c.state, c.label)).join('') + '</div>'; }

function cards(items) {
  return '<div class="cards">' + items.map(item =>
    '<div class="card"><span class="num ' + esc(item.tone || '') + '">' + esc(item.num) + '</span>' +
    '<span class="label">' + esc(item.label) + '</span>' +
    '<span class="hint">' + esc(item.hint || '') + '</span></div>').join('') + '</div>';
}
function notice(text, tone) {
  return '<div class="notice ' + esc(tone || '') + '">' + esc(text) + '</div>';
}
function noticeHtml(html, tone) {
  return '<div class="notice ' + esc(tone || '') + '">' + html + '</div>';
}
function ul(items, cls) {
  return '<ul' + (cls ? ' class="' + esc(cls) + '"' : '') + '>' +
    items.map(item => '<li>' + (typeof item === 'string' ? esc(item) : item) + '</li>').join('') + '</ul>';
}
function ulRaw(items, cls) {
  return '<ul' + (cls ? ' class="' + esc(cls) + '"' : '') + '>' +
    items.map(item => '<li>' + item + '</li>').join('') + '</ul>';
}
function ol(items) {
  return '<ol>' + items.map(item => '<li>' + (typeof item === 'string' ? esc(item) : item) + '</li>').join('') + '</ol>';
}
function timeline(items) {
  return '<ol class="timeline">' + items.map(item =>
    '<li><b>' + esc(item.title) + '</b>' + (item.text ? '<br><small>' + esc(item.text) + '</small>' : '') + '</li>').join('') + '</ol>';
}
function details(options) {
  return '<details' + (options.open ? ' open' : '') + (options.id ? ' id="' + esc(options.id) + '"' : '') +
    (options.cls ? ' class="' + esc(options.cls) + '"' : '') + '>' +
    '<summary>' + esc(options.title) + '</summary>' + options.body + '</details>';
}
function evidence(tag, body) {
  return '<div class="evidence"><span class="tag">' + esc(tag) + '</span>' + body + '</div>';
}
function listPairs(pairs) {
  return '<ul>' + pairs.map(pair => '<li><b>' + esc(pair[0]) + '</b>：' + pair[1] + '</li>').join('') + '</ul>';
}

/**
 * columns: [{ label, cell(row, i) -> html, sort: 'num'|'text'|null, cls }]
 * rows: 任意数组；rowId(row,i) -> 行 id；rowAttrs(row,i) -> 已转义的原始属性串。
 */
function table(options) {
  const columns = options.columns;
  const rows = options.rows || [];
  const head = columns.map(col => {
    const attrs = [];
    if (col.sort) attrs.push('data-sort="' + esc(col.sort) + '"');
    if (col.cls) attrs.push('class="' + esc(col.cls) + '"');
    return '<th scope="col"' + (attrs.length ? ' ' + attrs.join(' ') : '') + '>' + esc(col.label) + '</th>';
  }).join('');
  const body = rows.map((row, i) => {
    const attrs = [];
    if (options.rowId) {
      const id = options.rowId(row, i);
      if (id) attrs.push('id="' + esc(id) + '"');
    }
    if (options.rowAttrs) {
      const extra = options.rowAttrs(row, i);
      if (extra) attrs.push(extra);
    }
    return '<tr' + (attrs.length ? ' ' + attrs.join(' ') : '') + '>' +
      columns.map(col => '<td class="' + esc(col.cls || '') + '">' + col.cell(row, i) + '</td>').join('') + '</tr>';
  }).join('');
  return '<div class="table-wrap"><table' + (options.id ? ' id="' + esc(options.id) + '"' : '') + '>' +
    '<caption>' + esc(options.caption || '') + '</caption>' +
    '<thead><tr>' + head + '</tr></thead><tbody>' + body + '</tbody></table></div>';
}

/** 任务卡取证清单：纯文本列表，脚本失效时仍完整可读。 */
function checklist(items, cls) {
  return '<ul class="checklist' + (cls ? ' ' + esc(cls) : '') + '">' +
    (items || []).map(item => '<li>' + esc(item) + '</li>').join('') + '</ul>';
}

/**
 * 任务卡组件。sections: [{label, items, open}]；rowAttrs 由页面写入已转义的筛选 data-* 属性。
 * 卡片 ID 同时作为跨页搜索锚点，需保证全页唯一。
 */
function taskCard(options) {
  const attrs = ['data-task-card', 'class="' + (options.cls ? 'task-card ' + esc(options.cls) : 'task-card') + '"'];
  if (options.id) attrs.push('id="' + esc(options.id) + '"');
  if (options.state) attrs.push('data-task-state="' + esc(options.state) + '"');
  if (options.rowAttrs) attrs.push(options.rowAttrs);
  const meta = (options.meta || []).map(item =>
    '<span class="task-meta"><span>' + esc(item.label) + '</span><b>' + esc(item.value) + '</b></span>').join('');
  const sections = (options.sections || []).map(section => details({
    cls: 'task-section', open: !!section.open, title: section.label,
    body: checklist(section.items)
  })).join('');
  return '<article ' + attrs.join(' ') + '>' +
    '<header class="task-head"><h3>' + esc(options.title) + '</h3>' + (options.chipsHtml || '') + '</header>' +
    (meta ? '<div class="task-meta-row">' + meta + '</div>' : '') +
    (options.bodyHtml || '') + sections + (options.footerHtml || '') + '</article>';
}

/** 筛选条：数量与选项均由调用方从台账传入，无写死统计。 */
function filtersBar(options) {
  const selects = (options.selects || []).map(select =>
    '<label>' + esc(select.label) + '<select id="' + esc(select.id) + '">' +
    '<option value="">' + esc(select.allLabel || '全部') + '</option>' +
    select.options.map(item => '<option value="' + esc(item.value) + '">' + esc(item.label) + '</option>').join('') +
    '</select></label>').join('');
  const total = esc(options.total || 0);
  return '<div class="filters">' +
    '<label>' + esc(options.queryLabel || '名称 / 标识') + '<input id="' + esc(options.queryId || 'task-query') +
    '" type="search" placeholder="' + esc(options.placeholder || '输入关键词') + '" autocomplete="off"></label>' +
    selects +
    '<button type="button" class="filter-button" id="task-expand-all">展开全部</button>' +
    '<button type="button" class="filter-button" id="task-collapse-all">收起全部</button>' +
    '<span class="filter-count" id="' + esc(options.countId || 'task-count') + '">' + total + ' / ' + total + '</span></div>';
}

/** 六段闭合链：每个节点带 data-chain-segment，供脚本按段高亮。 */
function chainGrid(nodes) {
  return '<ol class="chain-grid">' + (nodes || []).map((node, i) =>
    '<li class="chain-node" tabindex="0" data-chain-node' +
    (node.id ? ' id="' + esc(node.id) + '"' : '') +
    ' data-chain-segment="' + esc(node.stageId) +
    '" data-chain-state="' + esc(node.planState) + '">' +
    '<span class="chain-step">' + esc(String(i + 1).padStart(2, '0')) + '</span>' +
    '<b>' + esc(node.segment) + '</b>' +
    '<p>' + esc(node.task) + '</p>' +
    '<small>证据：' + esc(node.evidence) + '</small>' +
    '<small>退出：' + esc(node.exitCriteria) + '</small>' +
    statusChip(node.planState, node.planState) + '</li>').join('') + '</ol>';
}
function dependencyList(ids, anchorPrefix) {
  if (!ids || !ids.length) return '<span class="path">无前置依赖</span>';
  return '<ol class="dep-chain">' + ids.map(id =>
    '<li><a href="#' + esc(anchorPrefix + id) + '">' + esc(id) + '</a></li>').join('') + '</ol>';
}

/** 目标源码树节点：状态色由 data-task-state 驱动，多值关联用逗号分隔供筛选器匹配。 */
function treeNode(options) {
  const node = options.node;
  const labels = options.labels || {};
  const stateLabel = labels[node.status] || node.status;
  const chipState = node.status === 'exists' ? 'done' : node.status === 'planned' ? 'partial' : 'unknown';
  const layers = node.layerIds || [];
  const packages = node.workPackageIds || [];
  const stages = node.stageIds || [];
  const rowAttrs = [
    'data-task-name="' + esc(node.path + ' ' + node.rationale + ' ' + (node.project || '')) + '"',
    'data-task-state="' + esc(node.status) + '"',
    'data-task-category="' + esc(layers.join(',')) + '"',
    'data-task-batch="' + esc(stages.join(',')) + '"',
    'data-task-package="' + esc(packages.join(',')) + '"',
    'data-task-owner="' + esc(node.project || '') + '"',
    'data-tree-node="' + esc(node.id) + '"'
  ].join(' ');
  const meta = [
    { label: '类型', value: node.kind === 'project' ? '工程' : '目录' },
    { label: '主层', value: layers[0] || '—' },
    { label: '工作包', value: packages.join('、') || '—' },
    { label: '阶段', value: stages.join('、') || '—' }
  ];
  if (node.metrics) {
    meta.push({ label: '规模', value: node.metrics.lines + ' 行 / ' + node.metrics.types + ' 类型' });
  }
  return taskCard({
    id: node.id,
    cls: 'tree-node',
    title: node.path,
    state: node.status,
    chipsHtml: chips([
      { state: chipState, label: stateLabel },
      { state: 'idle', label: node.evidenceType }
    ]),
    meta,
    bodyHtml: '<p class="tree-path">' + code(node.path) + '</p><p>' + esc(node.rationale) + '</p>',
    rowAttrs,
    sections: [
      {
        label: '证据状态与关联',
        items: [
          '状态：' + stateLabel + '（' + node.evidenceType + '）',
          '关联层：' + (layers.join('、') || '—'),
          '原厂程序集：' + ((node.originalAssemblies || []).join('、') || '—'),
          '工作包：' + (packages.join('、') || '—'),
          '阶段：' + (stages.join('、') || '—')
        ]
      },
      { label: '未知项 / 边界', items: node.unknowns.length ? node.unknowns : ['无'] }
    ]
  });
}

/** 目标源码树层分组：一个节点只渲染一次（主层），其余层通过关联字段表达。 */
function treeGroup(options) {
  const layer = options.layer;
  const nodes = options.nodes || [];
  const counts = { exists: 0, planned: 0, unknown: 0 };
  nodes.forEach(node => { counts[node.status] = (counts[node.status] || 0) + 1; });
  return '<details class="tree-layer" id="tree-' + esc(layer.id) + '"' + (options.open ? ' open' : '') + '>' +
    '<summary>' + esc(layer.id + ' · ' + layer.name) +
    chips([
      { state: 'done', label: '已存在 ' + (counts.exists || 0) },
      { state: 'partial', label: '待建 ' + (counts.planned || 0) },
      { state: 'unknown', label: '未知 ' + (counts.unknown || 0) }
    ]) + '</summary>' +
    '<p class="tree-target">目标：' + esc(layer.target) + '</p>' +
    '<div class="tree-children">' +
    nodes.map(node => treeNode({ node, labels: options.labels })).join('') +
    '</div></details>';
}

function navigation(model, slug) {
  return '<ul class="nav-list">' + model.pages.map((item, i) => {
    const current = item.slug === slug;
    return '<li><a href="' + esc(hrefFor(item.slug)) + '"' + (current ? ' aria-current="page"' : '') + '>' +
      '<span class="idx">' + String(i).padStart(2, '0') + '</span><span>' + esc(item.title) + '</span></a></li>';
  }).join('') + '</ul>';
}
function sideToc(anchors) {
  if (!anchors.length) return '';
  return '<details class="side-toc" open><summary>本页目录</summary><ol>' + anchors.map(a =>
    '<li><a href="#' + esc(a.id) + '">' + esc(a.title) + '</a></li>').join('') + '</ol></details>';
}
function rightToc(anchors, model) {
  return '<aside class="toc" aria-label="页内目录"><div class="title">页内目录</div><ol>' + anchors.map(a =>
    '<li><a href="#' + esc(a.id) + '"' + (a.level === 3 ? ' style="padding-left:18px"' : '') + '>' + esc(a.title) + '</a></li>').join('') +
    '</ol><div class="built">builtFrom<br>' + esc(model.builtFrom.head.slice(0, 8)) + '<br>' +
    esc(model.builtFrom.inputsDigest.slice(0, 12)) + '</div></aside>';
}
function footer(model) {
  const sources = model.inputs.concat(model.supporting);
  return '<footer class="foot"><p>本页所有统计数字、状态与既有结论均由机器台账装配生成；判断性内容来自 ' +
    code(model.sourcePaths.editorial) + '。执行口径见 ' +
    '<a href="' + esc(hrefFor('00-master-plan')) + '">权威总纲</a>，证据四态与门禁规则见 ' +
    '<a href="' + esc(hrefFor('09-gates-evidence')) + '">门禁与证据规则</a>。' +
    '站点不写 wall-clock，同一工作树重跑字节一致。</p>' +
    '<details><summary>数据来源与 SHA-256 前缀（' + sources.length + ' 项）</summary><ul class="hash-list">' +
    sources.map(item => '<li><span class="p">' + esc(item.path) + '</span><span class="h">' +
      esc(item.sha256.slice(0, 12)) + '</span></li>').join('') + '</ul></details>' +
    '<p>' + code('plan-data.json') + ' · builtFrom ' + code(model.builtFrom.head.slice(0, 8) + '/' + model.builtFrom.inputsDigest.slice(0, 12)) +
    ' · <a href="' + esc(hrefFor('index')) + '">返回索引</a></p></footer>';
}

function page(options) {
  const { model, slug, title, kicker, tier, description, headChips, intro, body, anchors } = options;
  const all = anchors || [];
  return '<!doctype html>\n<html lang="zh-CN">\n<head>\n' +
    '<meta charset="utf-8">\n' +
    '<meta name="viewport" content="width=device-width, initial-scale=1">\n' +
    '<title>' + esc(title) + ' · 逆向计划</title>\n' +
    '<meta name="description" content="' + esc(description || kicker) + '">\n' +
    '<link rel="stylesheet" href="assets/plan.css">\n' +
    '</head>\n<body>\n' +
    '<a class="skip" href="#main">跳到正文</a>\n' +
    '<div class="shell">\n' +
    '<nav class="side" aria-label="站点导航"><div class="brand"><span class="mark">逆向计划 · 证据台</span>' +
    '<small>REVERSE PLAN / OFFLINE</small></div>' +
    '<div class="searchbar" id="plan-search-bar" hidden><input id="plan-search-input" type="search" ' +
    'placeholder="搜索工具名 / 单元 ID / 阶段" aria-label="跨页搜索" autocomplete="off"></div>' +
    '<div class="nav-title">' + esc(model.pages.length) + ' 页导航</div>' + navigation(model, slug) +
    '<div class="nav-title">阶段顺序</div>' +
    '<p style="font-size:12px;color:var(--fg-3);margin:0">S0 纵向切片 → S1–S3 机制 → S4–S5 覆盖 → S6–S7 补齐 → S8–S9 铺开</p>' +
    sideToc(all) + '</nav>\n' +
    '<main class="content" id="main">\n' +
    '<div class="crumbs">docs/reverse-plan/' + esc(slug === 'index' ? 'index.html' : slug + '.html') + '</div>\n' +
    '<header class="page-head"><h1>' + esc(title) + '</h1><p class="kicker">' + esc(kicker) + '</p>' +
    '<div class="head-meta">' + chips(headChips || [{ state: 'idle', label: tier }]) + '</div></header>\n' +
    (intro || '') + '\n' + body + '\n' + footer(model) + '\n' +
    '</main>\n' + rightToc(all, model) + '\n</div>\n' +
    '<script src="assets/plan.js" defer></script>\n</body>\n</html>\n';
}

/** 页构造器：sec/sub 自动登记锚点，finish 返回 {slug, html, anchors}。 */
function createPage(model, meta) {
  const anchors = [];
  const parts = [];
  let introHtml = '';
  return {
    anchors,
    intro(html) { introHtml += html; },
    raw(html) { parts.push(html); },
    sec(id, no, title, html) {
      anchors.push({ id, title, level: 2 });
      parts.push('<section><h2 id="' + esc(id) + '"><span class="no">' + esc(no) + '</span>' + esc(title) + '</h2>' + html + '</section>');
    },
    sub(id, title, html) {
      anchors.push({ id, title, level: 3 });
      parts.push('<section><h3 id="' + esc(id) + '">' + esc(title) + '</h3>' + html + '</section>');
    },
    finish() {
      return {
        slug: meta.slug,
        title: meta.title,
        kicker: meta.kicker,
        tier: meta.tier,
        description: meta.description,
        anchors,
        html: page({
          model, slug: meta.slug, title: meta.title, kicker: meta.kicker, tier: meta.tier,
          description: meta.description, headChips: meta.headChips || [{ state: 'idle', label: meta.tier }],
          intro: introHtml, body: parts.join('\n'), anchors
        })
      };
    }
  };
}

module.exports = {
  esc, fmt, pct, code, mono, pathHtml, hrefFor, slugId, statusState, chip, statusChip, chips,
  cards, notice, noticeHtml, ul, ulRaw, ol, timeline, details, evidence, listPairs, table, page, createPage,
  checklist, taskCard, filtersBar, chainGrid, dependencyList, treeNode, treeGroup
};
