'use strict';
// 三张确定性内联 SVG 图：8 层架构分层图 / 68 节点源码树图 / 29 工程 + 62 边依赖图。
// 纯函数：所有坐标由台账数据顺序与固定算法计算，不用 Date / Math.random / 外部资源。
// 不写 xmlns（内联 HTML SVG 无需，且避免 http(s):// 外链断言）。
const ESC = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' };
function esc(value) {
  return String(value === null || value === undefined ? '' : value).replace(/[&<>"']/g, c => ESC[c]);
}
function fmt(value) {
  return String(value).replace(/\B(?=(\d{3})+(?!\d))/g, ',');
}
function must(condition, message) {
  if (!condition) throw new Error('[plan-diagrams] ' + message);
}
function clip(value, max) {
  const text = String(value);
  return text.length > max ? text.slice(0, max - 1) + '…' : text;
}

function projectLayerMap(model) {
  const map = new Map();
  for (const layer of model.layers) {
    for (const project of layer.currentProjects) {
      if (!map.has(project)) map.set(project, layer.id);
    }
  }
  for (const project of model.currentProjects) {
    if (!map.has(project.Name)) map.set(project.Name, 'L0-unassigned');
  }
  return map;
}
function scaleBands(values) {
  const sorted = values.slice().sort((a, b) => a - b);
  const at = ratio => sorted.length ? sorted[Math.min(sorted.length - 1, Math.floor(sorted.length * ratio))] : 0;
  return [at(0.25), at(0.5), at(0.75)];
}
function scaleIndex(value, bands) {
  if (value <= bands[0]) return 0;
  if (value <= bands[1]) return 1;
  if (value <= bands[2]) return 2;
  return 3;
}
function svgWrap(options) {
  return '<div class="diagram" data-diagram="' + esc(options.diagram) + '">' +
    '<svg viewBox="0 0 ' + options.width + ' ' + options.height + '" role="img" ' +
    'aria-labelledby="' + esc(options.id) + '-t ' + esc(options.id) + '-d" ' +
    'data-diagram="' + esc(options.diagram) + '">' +
    '<title id="' + esc(options.id) + '-t">' + esc(options.title) + '</title>' +
    '<desc id="' + esc(options.id) + '-d">' + esc(options.desc) + '</desc>' +
    '<defs><marker id="' + esc(options.id) + '-arrow" viewBox="0 0 10 10" refX="9" refY="5" ' +
    'markerWidth="6" markerHeight="6" orient="auto-start-reverse"><path d="M 0 0 L 10 5 L 0 10 z" class="dg-arrow" /></marker></defs>' +
    options.body + '</svg></div>';
}
function text(x, y, value, cls, anchor) {
  return '<text x="' + x + '" y="' + y + '"' + (cls ? ' class="' + esc(cls) + '"' : '') +
    (anchor ? ' text-anchor="' + esc(anchor) + '"' : '') + '>' + esc(value) + '</text>';
}

/** 图 1：8 层架构分层图 —— 原厂侧 / 自研侧并列，下方泳道画层间依赖箭头。 */
function renderArchitectureDiagram(model) {
  const layers = model.layers;
  must(layers.length === 8, '架构分层图应为 8 层');
  const projectLayers = projectLayerMap(model);
  const layerIndex = new Map(layers.map((layer, i) => [layer.id, i]));
  const pairs = new Map();
  let layerEdges = 0;
  let unresolved = 0;
  for (const edge of model.currentEdges) {
    const from = projectLayers.get(edge.From);
    const to = projectLayers.get(edge.To);
    if (from === 'L0-unassigned' || to === 'L0-unassigned' || !layerIndex.has(from) || !layerIndex.has(to)) {
      unresolved++;
      continue;
    }
    layerEdges++;
    const key = from + '>' + to;
    pairs.set(key, (pairs.get(key) || 0) + 1);
  }
  const rowH = 104;
  const top = 54;
  const laneTop = top + layers.length * rowH + 46;
  const height = laneTop + 178;
  const width = 1160;
  const body = [];

  body.push('<g class="dg-head">' +
    text(24, 26, '原厂（25 个唯一程序集）', 'dg-col-title') +
    text(548, 26, '自研当前（29 个工程）', 'dg-col-title') +
    text(24, 44, '分层职责同向，运行时机制 / 方案容器 / 控件树不一致；本图不表示等价', 'dg-note') +
    '</g>');

  layers.forEach((layer, i) => {
    const y = top + i * rowH;
    const original = layer.original || {};
    const lines = layer.currentLines || 0;
    const band = scaleIndex(lines, scaleBands(layers.map(item => item.currentLines || 0)));
    body.push('<g class="dg-row" data-diagram-layer="' + esc(layer.id) + '">');
    body.push('<rect class="dg-box dg-original" x="20" y="' + y + '" width="440" height="84" rx="8" />');
    body.push(text(34, y + 22, layer.id + ' · ' + clip(layer.name, 30), 'dg-layer-name'));
    body.push(text(34, y + 44, '原厂程序集 ' + (layer.assemblies || []).length + ' 个 · ' +
      fmt(original.lines || 0) + ' 行 / ' + fmt(original.types || 0) + ' 类型', 'dg-metric'));
    body.push(text(34, y + 66, 'fanIn ' + fmt(original.fanIn || 0) + ' · 缺口：' + clip(layer.gap || layer.status || '', 42), 'dg-sub'));

    body.push('<path class="dg-correspond" d="M 462 ' + (y + 42) + ' H 536" />');
    body.push(text(499, y + 34, '对应', 'dg-note', 'middle'));

    body.push('<rect class="dg-box dg-current dg-scale-' + band + '" x="540" y="' + y + '" width="596" height="84" rx="8" />');
    body.push(text(554, y + 22, '自研工程 ' + layer.currentProjects.length + ' 个 · 当前 ' + fmt(lines) + ' 行', 'dg-layer-name'));
    body.push(text(554, y + 44, clip(layer.currentProjects.join('、') || '无', 74), 'dg-metric'));
    body.push(text(554, y + 66, '状态 ' + esc(layer.status) + ' · 行覆盖 ' + esc(layer.lineCoveragePercent) + '%', 'dg-sub'));
    body.push('</g>');
  });

  // 层间依赖泳道：只画两端均可归层的边聚合，未归层边不推断归属。
  body.push('<g class="dg-lane">');
  body.push(text(24, laneTop - 14, '层间依赖箭头（仅统计两端均可归层的 ' + layerEdges + '/' + model.currentEdges.length +
    ' 条 ProjectReference；未归层 ' + unresolved + ' 条不画、不推断）', 'dg-note'));
  const dotX = i => 84 + i * 140;
  const dotY = laneTop + 76;
  layers.forEach((layer, i) => {
    body.push('<circle class="dg-lane-dot" cx="' + dotX(i) + '" cy="' + dotY + '" r="13" />');
    body.push(text(dotX(i), dotY + 4, layer.id.split('-')[0], 'dg-lane-label', 'middle'));
    body.push(text(dotX(i), dotY + 32, clip(layer.name, 12), 'dg-lane-sub', 'middle'));
  });
  let arc = 0;
  for (const [key, count] of pairs) {
    const [from, to] = key.split('>');
    const i = layerIndex.get(from);
    const j = layerIndex.get(to);
    if (i === undefined || j === undefined) continue;
    const x1 = dotX(i);
    const x2 = dotX(j);
    if (i === j) {
      const cy = dotY - 34 - (arc % 3) * 9;
      body.push('<path class="dg-arc" d="M ' + (x1 - 10) + ' ' + (dotY - 12) +
        ' C ' + (x1 - 42) + ' ' + cy + ' ' + (x1 + 42) + ' ' + cy + ' ' + (x1 + 10) + ' ' + (dotY - 12) +
        '" marker-end="url(#arch-diagram-arrow)"><title>' + esc(from + ' → ' + to + '：' + count + ' 条') + '</title></path>');
    } else {
      const lift = 26 + (arc % 5) * 13;
      const cy = dotY - lift;
      body.push('<path class="dg-arc" d="M ' + x1 + ' ' + (dotY - 14) +
        ' Q ' + ((x1 + x2) / 2) + ' ' + cy + ' ' + x2 + ' ' + (dotY - 14) +
        '" marker-end="url(#arch-diagram-arrow)"><title>' + esc(from + ' → ' + to + '：' + count + ' 条') + '</title></path>');
      body.push(text((x1 + x2) / 2, cy + 4, String(count), 'dg-arc-count', 'middle'));
    }
    arc++;
  }
  body.push('</g>');

  return {
    html: svgWrap({
      id: 'arch-diagram', diagram: 'architecture', width, height,
      title: '原厂与自研 8 层架构分层图',
      desc: '每层左侧为原厂程序集与行数、右侧为自研当前工程与行数；下方泳道按 ProjectReference 聚合画层间依赖箭头。数字来自架构基线台账。',
      body: body.join('')
    }),
    pairs: [...pairs.entries()].map(([key, count]) => ({ key, count })),
    layerEdges,
    unresolved
  };
}

/** 图 2：源码树图 —— 按主层分组的连线式树，节点按状态着色。 */
function renderSourceTreeDiagram(model) {
  const nodes = model.targetTree.nodes;
  must(nodes.length === 68, '源码树图节点应为 68 个');
  const nameOf = new Map(model.layers.map(layer => [layer.id, layer.name]));
  nameOf.set('L0-unassigned', '未归层');
  const groups = new Map();
  for (const node of nodes) {
    const layer = node.layerIds[0] || 'L0-unassigned';
    if (!groups.has(layer)) groups.set(layer, []);
    groups.get(layer).push(node);
  }
  const ordered = [...groups.entries()].sort((a, b) => a[0].localeCompare(b[0]));
  // 3 列贪心分配，按节点数降序放入当前最轻的列，保证布局确定且高度接近。
  const columns = [[], [], []];
  const weights = [0, 0, 0];
  for (const group of ordered.slice().sort((a, b) => b[1].length - a[1].length || a[0].localeCompare(b[0]))) {
    let target = 0;
    for (let i = 1; i < 3; i++) if (weights[i] < weights[target]) target = i;
    columns[target].push(group);
    weights[target] += group[1].length + 1;
  }
  const colW = 390;
  const rowH = 24;
  const groupH = 28;
  const width = colW * 3 + 20;
  const maxRows = Math.max(...weights);
  const height = 40 + maxRows * rowH + 20;
  const body = [];
  body.push(text(14, 24, '目标源码树：工程 → 关键子目录；实线=已存在，琥珀=待建，粉红虚线=未知', 'dg-note'));

  columns.forEach((column, colIndex) => {
    const colX = 10 + colIndex * colW;
    let row = 0;
    column.forEach(([layerId, list]) => {
      const groupY = 40 + row * rowH;
      body.push('<g class="dg-group" data-diagram-group="' + esc(layerId) + '">');
      body.push('<rect class="dg-group-bar" x="' + colX + '" y="' + groupY + '" width="' + (colW - 20) + '" height="22" rx="4" />');
      body.push(text(colX + 8, groupY + 15, layerId + ' · ' + clip(nameOf.get(layerId) || layerId, 22) + '（' + list.length + '）', 'dg-group-title'));
      row++;
      const sorted = list.slice().sort((a, b) => a.path.localeCompare(b.path));
      const projectNodes = sorted.filter(node => node.kind === 'project');
      const placed = new Map();
      sorted.forEach(node => {
        const y = 40 + row * rowH;
        const x = colX + (node.kind === 'directory' ? 22 : 6);
        const w = colW - 20 - (node.kind === 'directory' ? 22 : 6);
        placed.set(node.id, { x, y, w });
        const number = node.metrics
          ? fmt(node.metrics.lines) + ' 行 / ' + fmt(node.metrics.types) + ' 类型'
          : (node.status === 'planned' ? '待建（无源码数）' : node.status === 'unknown' ? '未知' : '目录');
        const shortPath = clip(node.path.replace(/^src\//, ''), 34);
        body.push('<a href="#' + esc(node.id) + '" class="dg-link">');
        body.push('<rect class="dg-node dg-' + esc(node.status) + (node.metrics ? ' dg-scale-' +
          scaleIndex(node.metrics.lines, scaleBands(model.currentProjects.map(p => p.Lines))) : '') +
          '" x="' + x + '" y="' + y + '" width="' + w + '" height="20" rx="4" ' +
          'data-diagram-node="' + esc(node.id) + '" />');
        body.push(text(x + 7, y + 14, shortPath, 'dg-node-label'));
        body.push(text(x + w - 7, y + 14, number, 'dg-node-number', 'end'));
        body.push('<title>' + esc(node.path + ' · ' + node.status + ' · ' + number) + '</title>');
        body.push('</a>');
        row++;
      });
      // 目录节点连到同组内路径前缀匹配的工程节点；无父则挂到组干线。
      sorted.filter(node => node.kind === 'directory').forEach(node => {
        const parent = projectNodes.find(project => node.path.startsWith(project.path + '/'));
        const from = parent ? placed.get(parent.id) : null;
        const to = placed.get(node.id);
        if (!to) return;
        if (from) {
          const x1 = from.x + 12;
          const x2 = to.x - 8;
          const midY = from.y + 20;
          body.push('<path class="dg-tree-link" d="M ' + x1 + ' ' + midY + ' V ' + (midY + 2) +
            ' H ' + x2 + ' V ' + (to.y + 10) + '" />');
        } else {
          body.push('<path class="dg-tree-link dg-tree-orphan" d="M ' + (colX + 8) + ' ' + (to.y + 10) +
            ' H ' + (to.x - 2) + '" />');
        }
      });
      body.push('</g>');
    });
  });

  return {
    html: svgWrap({
      id: 'tree-diagram', diagram: 'source-tree', width, height,
      title: '自研目标架构源码树图',
      desc: '按主层分组的连线式目录树，共 68 个节点；工程节点内嵌行数与类型数，目录节点无源码数时标注待建或未知。',
      body: body.join('')
    }),
    groups: ordered.length,
    nodes: nodes.length
  };
}

/** 图 3：工程依赖图 —— 29 节点 + 62 边，按主层着色、按行数分档。 */
function renderDependencyDiagram(model) {
  const projects = model.currentProjects.slice().sort((a, b) => a.Name.localeCompare(b.Name));
  must(projects.length === 29, '依赖图工程节点应为 29 个');
  const edges = model.currentEdges;
  must(edges.length === 62, '依赖图边应为 62 条');
  const projectLayers = projectLayerMap(model);
  const bands = scaleBands(projects.map(project => project.Lines));
  const cols = 5;
  const cellW = 232;
  const cellH = 78;
  const width = cols * cellW + 20;
  const rows = Math.ceil(projects.length / cols);
  const height = 40 + rows * cellH + 20;
  const pos = new Map();
  projects.forEach((project, i) => {
    pos.set(project.Name, {
      x: 10 + (i % cols) * cellW,
      y: 40 + Math.floor(i / cols) * cellH
    });
  });
  const body = [];
  body.push(text(14, 24, '29 个当前工程 + 62 条 ProjectReference；节点数字为「行 / 类型」，颜色按规模分档', 'dg-note'));
  for (const edge of edges) {
    const a = pos.get(edge.From);
    const b = pos.get(edge.To);
    must(a && b, '依赖边端点不在工程清单: ' + edge.From + ' → ' + edge.To);
    const x1 = a.x + 108;
    const y1 = a.y + 60;
    const x2 = b.x + 108;
    const y2 = b.y + 60;
    body.push('<path class="dg-edge" d="M ' + x1 + ' ' + y1 + ' L ' + x2 + ' ' + y2 +
      '" data-diagram-edge data-edge-from="' + esc(edge.From) + '" data-edge-to="' + esc(edge.To) + '">' +
      '<title>' + esc(edge.From + ' → ' + edge.To + '：' + edge.Files + ' 个引用文件') + '</title></path>');
  }
  projects.forEach(project => {
    const p = pos.get(project.Name);
    const layerId = projectLayers.get(project.Name);
    const band = scaleIndex(project.Lines, bands);
    body.push('<g class="dg-project" data-diagram-key="' + esc(project.Name) + '">');
    body.push('<rect class="dg-node dg-exists dg-scale-' + band + '" x="' + p.x + '" y="' + p.y +
      '" width="216" height="62" rx="7" data-diagram-node="' + esc(project.Name) + '" />');
    body.push(text(p.x + 9, p.y + 19, clip(project.Name, 28), 'dg-node-label'));
    body.push(text(p.x + 9, p.y + 38, fmt(project.Lines) + ' 行 / ' + fmt(project.Types) + ' 类型 / ' +
      fmt(project.CsFiles) + ' .cs', 'dg-node-number'));
    body.push(text(p.x + 9, p.y + 54, esc(layerId), 'dg-sub'));
    body.push('<title>' + esc(project.Name + ' · ' + project.Lines + ' 行 / ' + project.Types +
      ' 类型 / ' + project.Methods + ' 方法 / ' + project.CsFiles + ' .cs · ' + layerId) + '</title>');
    body.push('</g>');
  });
  return {
    html: svgWrap({
      id: 'deps-diagram', diagram: 'dependency', width, height,
      title: '当前 29 个工程依赖图',
      desc: '每个节点内嵌行数、类型数与 .cs 文件数，按行规模分档着色；62 条 ProjectReference 以淡色直线表示，悬停可高亮关联边。',
      body: body.join('')
    }),
    nodes: projects.length,
    edges: edges.length,
    bands
  };
}

function buildDiagrams(model) {
  const architecture = renderArchitectureDiagram(model);
  const dependency = renderDependencyDiagram(model);
  return {
    architecture,
    dependency,
    counts: {
      diagrams: 2,
      architectureLayers: model.layers.length,
      layerPairs: architecture.pairs.length,
      layerEdges: architecture.layerEdges,
      unresolvedLayerEdges: architecture.unresolved,
      dependencyNodes: dependency.nodes,
      dependencyEdges: dependency.edges,
      scaleBands: dependency.bands
    }
  };
}

function diagramLegend(model, diagrams) {
  const bands = diagrams.dependency.bands;
  const items = [
    '<span class="dg-key dg-exists">已存在（current-source）</span>',
    '<span class="dg-key dg-planned">待建（计划编排，无源码数）</span>',
    '<span class="dg-key dg-unknown">未知（待补证）</span>',
    '<span class="dg-key dg-scale-0">≤ ' + fmt(bands[0]) + ' 行</span>',
    '<span class="dg-key dg-scale-1">≤ ' + fmt(bands[1]) + ' 行</span>',
    '<span class="dg-key dg-scale-2">≤ ' + fmt(bands[2]) + ' 行</span>',
    '<span class="dg-key dg-scale-3">&gt; ' + fmt(bands[2]) + ' 行</span>'
  ];
  return '<div class="diagram-legend" data-diagram-legend><b>图例</b>' + items.join('') +
    '<p>数字只来自 <code>docs/architecture-baseline.json → currentProjects / currentEdges / layers</code>；' +
    '目录节点没有源码数，不编造。层间箭头只统计两端均可归层的 ' + diagrams.architecture.layerEdges + '/' +
    model.currentEdges.length + ' 条依赖边，未归层 ' + diagrams.architecture.unresolved +
    ' 条不画、不推断归属。颜色之外用实线/虚线边框区分状态。</p></div>';
}

module.exports = {
  buildDiagrams, diagramLegend,
  renderArchitectureDiagram, renderSourceTreeDiagram, renderDependencyDiagram
};
