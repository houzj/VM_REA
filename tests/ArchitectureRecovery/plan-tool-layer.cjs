'use strict';
// 工具层 = 162 个原厂 Module(sp) 工具目录/资源树，全部是逆向实现目标。
// 节点只来自 docs/plugins/inventory.json 的 xml/binary/resource 证据路径与 SHA-256；
// 不实时扫描原厂安装目录（保证确定性），不做工具→自研工程映射。
const ESC = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' };
const STATE_NOT_ESTABLISHED = 'not-established';
const STATE_PARTIAL = 'partial';

function esc(value) {
  return String(value === null || value === undefined ? '' : value).replace(/[&<>"']/g, c => ESC[c]);
}
function must(condition, message) {
  if (!condition) throw new Error('[plan-tool-layer] ' + message);
}
function basename(value) {
  const parts = String(value || '').split(/[\\/]/).filter(Boolean);
  return parts.length ? parts[parts.length - 1] : '';
}
function sha8(value) { return String(value || '').slice(0, 8); }
// 页面只保留可定位的相对路径，避免 162 条完整安装盘路径撑大站点；完整路径仍在台账里。
function relativeToolPath(tool) {
  return String(tool.category || '') + '/' + basename(tool.sourcePath);
}
function stateOf(tool) {
  return String(tool.implementationStatus || '').indexOf('partial-independent') === 0
    ? STATE_PARTIAL : STATE_NOT_ESTABLISHED;
}
function evidenceOf(tool) {
  const details = tool.details || {};
  const xml = details.xmlEvidence || [];
  const binary = details.binaryEvidence || [];
  const resource = details.resourceEvidence || [];
  const picked = [];
  xml.slice(0, 1).forEach(item => picked.push({
    name: basename(item.path), tag: 'XML', sha: sha8(item.sha256)
  }));
  binary.slice(0, 2).forEach(item => picked.push({
    name: basename(item.path), tag: item.kind || 'DLL', sha: sha8(item.sha256), size: item.size
  }));
  resource.slice(0, 1).forEach(item => picked.push({
    name: basename(item.path), tag: '资源', sha: sha8(item.sha256)
  }));
  const total = xml.length + binary.length + resource.length;
  return { picked, total, omitted: Math.max(0, total - picked.length) };
}

function buildToolLayer(model) {
  must(model && Array.isArray(model.tools) && model.tools.length === 162,
    '工具层应为 162 个目录工具');
  must(Array.isArray(model.categories) && model.categories.length === 14,
    '工具层应为 14 个原厂分类');
  const cards = (model.wbs && model.wbs.toolCards) || [];
  must(cards.length === 162, '目录工具任务卡应为 162 张');
  for (let i = 0; i < model.tools.length; i++) {
    must(cards[i].key === model.tools[i].key, '工具与任务卡顺序不一致: ' + model.tools[i].key);
  }

  const indexOfKey = new Map(model.tools.map((tool, i) => [tool.key, i]));
  const groups = model.categories.map(category => {
    const tools = (category.tools && category.tools.length)
      ? category.tools
      : model.tools.filter(tool => tool.category === category.name);
    must(tools.length > 0, '分类为空: ' + category.name);
    const rows = tools.map(tool => {
      const index = indexOfKey.get(tool.key);
      must(typeof index === 'number', '分类引用了不存在的工具: ' + tool.key);
      const evidence = evidenceOf(tool);
      must(evidence.picked.length > 0, '工具缺少原厂目录证据: ' + tool.key);
      return {
        tool, evidence,
        state: stateOf(tool),
        anchor: 'toolcard-' + (index + 1),
        dirName: basename(tool.sourcePath) || tool.key
      };
    });
    return { name: category.name, count: rows.length, rows };
  });

  const rows = groups.flatMap(group => group.rows);
  must(rows.length === 162, '分类行合计应为 162，实际 ' + rows.length);
  must(new Set(rows.map(row => row.tool.key)).size === 162, '工具层存在重复工具');
  const counts = {
    tools: 162,
    categories: 14,
    notEstablished: rows.filter(row => row.state === STATE_NOT_ESTABLISHED).length,
    partial: rows.filter(row => row.state === STATE_PARTIAL).length,
    evidenceFiles: rows.reduce((n, row) => n + row.evidence.total, 0),
    renderedEvidenceFiles: rows.reduce((n, row) => n + row.evidence.picked.length, 0)
  };
  must(counts.notEstablished === 161 && counts.partial === 1,
    '工具状态应为 161 未建立 + 1 partial，实际 ' + counts.notEstablished + '+' + counts.partial);

  const html = groups.map((group, i) => renderGroup(group, i === 0)).join('');
  return { groups, counts, html };
}

function renderToolDir(row, category) {
  const stateChip = row.state === STATE_PARTIAL
    ? '<span class="chip" data-state="unknown">partial 独立实现</span>'
    : '<span class="chip" data-state="partial">待逆向实现</span>';
  const lines = [row.dirName + '/'];
  row.evidence.picked.forEach((item, i) => {
    const last = i === row.evidence.picked.length - 1 && row.evidence.omitted === 0;
    lines.push((last ? '└── ' : '├── ') + item.name + '  [' + item.tag +
      (item.size ? ' ' + item.size + 'B' : '') + ' sha:' + item.sha + ']');
  });
  if (row.evidence.omitted > 0) {
    lines.push('└── …其余 ' + row.evidence.omitted + ' 个证据文件（见 inventory 台账）');
  }
  return '<div class="tool-dir" data-tool-layer-key="' + esc(row.tool.key) +
    '" data-tool-layer-category="' + esc(category) +
    '" data-tool-layer-state="' + row.state +
    '" data-tool-layer-path="' + esc(relativeToolPath(row.tool)) +
    '" data-tool-layer-sha="' + esc(row.evidence.picked[0].sha) + '">' +
    '<div class="tool-dir-head"><code>' + esc(row.tool.key) + '</code> · ' +
    esc(row.tool.displayName) + ' ' + stateChip +
    ' <a href="13-tool-cards.html#' + esc(row.anchor) + '">任务卡 →</a></div>' +
    '<pre class="tool-dir-tree">' + esc(lines.join('\n')) + '</pre></div>';
}

function renderGroup(group, open) {
  return '<details class="tool-layer-group"' + (open ? ' open' : '') + '>' +
    '<summary>' + esc(group.name) +
    '<span class="chip" data-state="idle">' + group.count + ' 个工具目录</span></summary>' +
    group.rows.map(row => renderToolDir(row, group.name)).join('') + '</details>';
}

function renderToolLayerControls(layer) {
  return '<div class="filters">' +
    '<label>过滤工具<input id="tool-layer-query" type="search" placeholder="工具名 / 中文名 / 分类 / 目录路径" autocomplete="off"></label>' +
    '<button type="button" class="filter-button" id="tool-layer-expand-all">展开全部</button>' +
    '<button type="button" class="filter-button" id="tool-layer-collapse-all">收起全部</button>' +
    '<span class="filter-count" id="tool-layer-count">' + layer.counts.tools + ' / ' + layer.counts.tools + '</span></div>';
}

module.exports = { buildToolLayer, renderToolLayerControls, STATE_NOT_ESTABLISHED, STATE_PARTIAL };
