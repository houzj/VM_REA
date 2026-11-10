'use strict';
// 原厂目标源码树：按 architecture-baseline.json 的 25 个唯一程序集，
// 归并 analysis/decompiled 的 28 棵反编译树（重复 pass 只取 canonical，不累加）。
// 纯函数：目录顺序固定，不写 wall-clock、不用随机数；为保证站点体积，每目录截断源码文件并显式标注。
const fs = require('fs');
const path = require('path');

const EXCLUDED = new Set(['bin', 'obj', '.git', '.a01group', '.pending', 'TestResults']);
const MAX_FILES_PER_DIR = 8;
const MAX_DEPTH = 4;
const ESC = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' };

function esc(value) {
  return String(value === null || value === undefined ? '' : value).replace(/[&<>"']/g, c => ESC[c]);
}
function must(condition, message) {
  if (!condition) throw new Error('[plan-original-tree] ' + message);
}
function sortEntries(a, b) {
  if (a.isDirectory() !== b.isDirectory()) return a.isDirectory() ? -1 : 1;
  return a.name < b.name ? -1 : a.name > b.name ? 1 : 0;
}
function isSource(name) { return /\.(cs|xaml)$/i.test(name); }

/** 构建单个程序集 canonical 树的节点；每目录最多列 MAX_FILES_PER_DIR 个源码文件。 */
function buildNodes(dirAbs, depth, stats) {
  const entries = fs.readdirSync(dirAbs, { withFileTypes: true })
    .filter(entry => !EXCLUDED.has(entry.name))
    .sort(sortEntries);
  const dirs = entries.filter(entry => entry.isDirectory());
  const files = entries.filter(entry => !entry.isDirectory() && isSource(entry.name));
  stats.dirs += dirs.length;
  stats.files += files.length;
  const nodes = dirs.map(entry => ({
    name: entry.name + '/', kind: 'dir',
    children: depth < MAX_DEPTH
      ? buildNodes(path.join(dirAbs, entry.name), depth + 1, stats)
      : [{ name: '…（层级截断，完整清单见 01/02 页台账）', kind: 'note', children: [] }]
  }));
  files.slice(0, MAX_FILES_PER_DIR).forEach(entry => {
    nodes.push({ name: entry.name, kind: 'file', children: [] });
    stats.rendered++;
  });
  if (files.length > MAX_FILES_PER_DIR) {
    nodes.push({
      name: '…其余 ' + (files.length - MAX_FILES_PER_DIR) + ' 个源码文件（见 01/02 页台账）',
      kind: 'note', children: []
    });
  }
  return nodes;
}
function renderTree(nodes, prefix) {
  return nodes.map((node, i) => {
    const last = i === nodes.length - 1;
    const line = prefix + (last ? '└── ' : '├── ') + node.name;
    if (node.children.length) {
      return line + '\n' + renderTree(node.children, prefix + (last ? '    ' : '│   '));
    }
    return line;
  }).join('\n');
}

function buildOriginalTree(model, root) {
  must(model && Array.isArray(model.assemblies) && model.assemblies.length === 25,
    '原厂唯一程序集应为 25 个');
  must(model.metrics && model.metrics.original, '缺少原厂源码量台账');
  const originalRoot = path.join(root, 'analysis', 'decompiled');
  must(fs.existsSync(originalRoot), '缺少反编译源码目录: analysis/decompiled');
  const canonicalSet = new Set(model.assemblies.map(item => item.canonicalTree));
  must(canonicalSet.size === 25, 'canonical 树应唯一，实际 ' + canonicalSet.size);
  const treeCount = model.assemblies.reduce((n, item) => n + (item.trees ? item.trees.length : 0), 0);
  must(treeCount === 28, '反编译树应为 28 棵，实际 ' + treeCount);
  const duplicates = model.assemblies.filter(item => item.duplicatePasses).length;
  must(duplicates === 3, '重复反编译 pass 的程序集应为 3 个，实际 ' + duplicates);

  const groups = model.assemblies.map((assembly, i) => {
    const canonical = assembly.canonicalTree;
    const dirAbs = path.join(originalRoot, canonical);
    must(fs.existsSync(dirAbs), 'canonical 反编译树不存在: ' + canonical);
    const stats = { dirs: 0, files: 0, rendered: 0 };
    const tree = renderTree(buildNodes(dirAbs, 1, stats), '');
    const layer = model.layers.find(item => (item.assemblies || []).includes(assembly.id));
    return {
      id: assembly.id,
      canonical,
      layerId: layer ? layer.id : 'L0-unassigned',
      cs: assembly.cs, xaml: assembly.xaml, lines: assembly.lines,
      types: assembly.types, methods: assembly.methods, fanIn: assembly.fanIn,
      duplicatePasses: !!assembly.duplicatePasses,
      treePasses: (assembly.trees || []).map(item => item.tree),
      stats,
      tree
    };
  });

  const counts = {
    assemblies: groups.length,
    treePasses: treeCount,
    canonicalTrees: canonicalSet.size,
    duplicatePasses: duplicates,
    uniqueCs: model.metrics.original.cs,
    uniqueLines: model.metrics.original.lines,
    uniqueTypes: model.metrics.original.types,
    uniqueMethods: model.metrics.original.methods,
    sourceFilesSeen: groups.reduce((n, group) => n + group.stats.files, 0),
    sourceFilesRendered: groups.reduce((n, group) => n + group.stats.rendered, 0)
  };
  must(counts.uniqueCs === 2614 && counts.uniqueLines === 435666,
    '原厂去重源码量与台账不符');

  const html = groups.map((group, i) =>
    '<details class="original-assembly" id="original-' + esc(i + 1) + '"' + (i === 0 ? ' open' : '') +
    ' data-original-assembly="' + esc(group.id) + '" data-original-tree="' + esc(group.canonical) +
    '" data-original-cs="' + group.cs + '" data-original-lines="' + group.lines + '">' +
    '<summary><code>' + esc(group.id) + '</code>' +
    '<span class="chip" data-state="idle">' + esc(group.canonical) + '</span>' +
    '<span class="chip" data-state="idle">' + group.cs + ' .cs / ' + group.xaml + ' .xaml</span>' +
    '<span class="chip" data-state="idle">' + group.lines.toLocaleString('en-US') + ' 行 / ' + group.types + ' 类型</span>' +
    (group.duplicatePasses ? '<span class="chip" data-state="unknown">重复 pass 已归并</span>' : '') +
    '</summary>' +
    '<p class="path">层 ' + esc(group.layerId) + ' · fanIn ' + group.fanIn +
    ' · 反编译树：' + esc(group.treePasses.join('、')) +
    '（只取 canonical，不累加重复 pass）</p>' +
    '<pre class="original-tree">' + esc(group.tree) + '</pre></details>').join('');

  return { html, groups, counts };
}

function renderOriginalTreeControls(tree) {
  return '<div class="filters">' +
    '<label>过滤程序集<input id="original-tree-query" type="search" placeholder="程序集 / 反编译树名" autocomplete="off"></label>' +
    '<button type="button" class="filter-button" id="original-tree-expand-all">展开全部</button>' +
    '<button type="button" class="filter-button" id="original-tree-collapse-all">收起全部</button>' +
    '<span class="filter-count" id="original-tree-count">' + tree.counts.assemblies + ' / ' + tree.counts.assemblies + '</span></div>';
}

module.exports = { buildOriginalTree, renderOriginalTreeControls };
