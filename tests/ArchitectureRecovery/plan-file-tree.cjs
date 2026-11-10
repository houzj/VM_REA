'use strict';
// 真实源码文件结构树：扫描 src/ 磁盘实际目录与代码文件，输出 tree 命令风格的等宽树线。
// 只保留 .cs/.xaml/.csproj/.props/.config；排除构建产物；不写盘、不创建目录、不写 wall-clock。
const fs = require('fs');
const path = require('path');

const CODE_EXT = new Set(['.cs', '.xaml', '.csproj', '.props', '.config']);
const EXCLUDED = ['bin', 'obj', 'artifacts', 'tmp', '.a01group', '.pending', 'TestResults'];
const EXCLUDED_SET = new Set(EXCLUDED);
const ESC = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' };

function esc(value) {
  return String(value === null || value === undefined ? '' : value).replace(/[&<>"']/g, c => ESC[c]);
}
function must(condition, message) {
  if (!condition) throw new Error('[plan-file-tree] ' + message);
}
function slug(value) {
  return String(value).replace(/[^A-Za-z0-9_-]+/g, '-').replace(/^-+|-+$/g, '') || 'item';
}
function sortEntries(a, b) {
  if (a.isDirectory() !== b.isDirectory()) return a.isDirectory() ? -1 : 1;
  return a.name < b.name ? -1 : a.name > b.name ? 1 : 0;
}
function walk(dirAbs) {
  const entries = fs.readdirSync(dirAbs, { withFileTypes: true })
    .filter(entry => !EXCLUDED_SET.has(entry.name))
    .sort(sortEntries);
  const nodes = [];
  for (const entry of entries) {
    if (entry.isDirectory()) {
      nodes.push({ name: entry.name, kind: 'dir', children: walk(path.join(dirAbs, entry.name)) });
    } else if (CODE_EXT.has(path.extname(entry.name).toLowerCase())) {
      nodes.push({ name: entry.name, kind: 'file', children: [] });
    }
  }
  return nodes;
}
function renderTree(nodes, prefix) {
  return nodes.map((node, i) => {
    const last = i === nodes.length - 1;
    const line = prefix + (last ? '└── ' : '├── ') + node.name + (node.kind === 'dir' ? '/' : '');
    if (node.kind === 'dir' && node.children.length) {
      return line + '\n' + renderTree(node.children, prefix + (last ? '    ' : '│   '));
    }
    return line;
  }).join('\n');
}
function countNodes(nodes, depth) {
  let files = 0, dirs = 0, maxDepth = depth;
  for (const node of nodes) {
    if (node.kind === 'file') { files++; continue; }
    dirs++;
    const sub = countNodes(node.children, depth + 1);
    files += sub.files;
    dirs += sub.dirs;
    maxDepth = Math.max(maxDepth, sub.maxDepth);
  }
  return { files, dirs, maxDepth };
}
function collectFiles(nodes, into) {
  for (const node of nodes) {
    if (node.kind === 'file') into.push(node.name);
    else collectFiles(node.children, into);
  }
  return into;
}

function buildFileTree(model, srcAbs, purposeMap, options) {
  must(model && model.metrics && model.metrics.current, '缺少模型或源码量台账');
  const includeToolProjects = !!(options && options.includeToolProjects);
  // 工具层由 162 个待逆向实现工具清单表示；本树默认只保留框架层 15 工程 + src 根文件。
  const top = walk(srcAbs).filter(entry => includeToolProjects || !entry.name.startsWith('VisionPlugins.'));
  // 目录数 = 框架工程目录 + 工程内子目录；src 根下的文件不计目录。
  let files = 0, dirs = top.filter(entry => entry.kind === 'dir').length, maxDepth = 1;
  const byExt = {};
  const frameworkPurpose = (purposeMap && purposeMap.projects ? purposeMap.projects : [])
    .filter(item => item.kind === 'framework');
  const purposeByName = new Map(frameworkPurpose.map(item => [item.name, item]));
  must(purposeByName.size === 15, '框架层工程用途映射应为 15 条，实际 ' + purposeByName.size);
  const projects = top.map(entry => {
    const stats = entry.kind === 'dir' ? countNodes(entry.children, 2) : { files: 1, dirs: 0, maxDepth: 1 };
    const names = entry.kind === 'dir' ? collectFiles(entry.children, []) : [entry.name];
    for (const name of names) {
      const ext = path.extname(name).toLowerCase();
      byExt[ext] = (byExt[ext] || 0) + 1;
    }
    files += stats.files;
    dirs += stats.dirs;
    maxDepth = Math.max(maxDepth, stats.maxDepth);
    const isDir = entry.kind === 'dir';
    const meta = isDir ? purposeByName.get(entry.name)
      : { kind: 'framework', purpose: 'src 根构建属性（Directory.Build.props）' };
    must(meta && meta.purpose && meta.purpose.trim().length > 0, '工程缺少设计用途说明: ' + entry.name);
    must(meta.kind === 'framework' || meta.kind === 'tool', '工程分组只能是 framework/tool: ' + entry.name);
    return {
      id: isDir ? 'ft-' + slug(entry.name) : 'ft-root',
      name: isDir ? entry.name : 'src 根文件',
      kind: meta.kind,
      purpose: meta.purpose,
      tree: renderTree(isDir ? entry.children : [entry], ''),
      fileCount: stats.files,
      // 页面按「工程自身 + 其子目录」展示，求和与 counts.dirs 对齐。
      dirCount: isDir ? stats.dirs + 1 : 0,
      maxDepth: stats.maxDepth
    };
  });

  // 框架层子集与机器台账交叉校验；文件/目录数由扫描派生，不写死旧全量。
  must(files > 0 && dirs > 0, '框架层扫描结果为空');
  must(projects.length === 16, '框架层应为 15 工程 + 1 根文件，实际 ' + projects.length);
  must(byExt['.csproj'] === 15, '.csproj 应为 15 个框架工程，实际 ' + byExt['.csproj']);
  must(byExt['.cs'] <= model.metrics.current.cs, '.cs 数量超过全量台账: ' + byExt['.cs']);
  must(byExt['.xaml'] <= model.metrics.current.xaml, '.xaml 数量超过全量台账: ' + byExt['.xaml']);

  const frameworkProjects = projects.filter(project => project.kind === 'framework');
  const frameworkEngineering = frameworkProjects.filter(project => project.id !== 'ft-root');
  const toolProjects = projects.filter(project => project.kind === 'tool');
  must(frameworkEngineering.length === 15, '框架层工程应为 15 个，实际 ' + frameworkEngineering.length);
  if (includeToolProjects) {
    must(toolProjects.length === 14, '工具层工程应为 14 个，实际 ' + toolProjects.length);
  }

  const renderGroup = list => {
    const openName = list.slice().sort((a, b) => b.fileCount - a.fileCount || a.name.localeCompare(b.name))[0].name;
    return list.map(project => '<details class="file-project" id="' + esc(project.id) +
      '" data-file-project="' + esc(project.name) + '" data-file-kind="' + esc(project.kind) +
      '" data-file-purpose="' + esc(project.purpose) + '"' +
      (project.name === openName ? ' open' : '') + '>' +
      '<summary><code>' + esc(project.name) + (project.name === 'src 根文件' ? '' : '/') + '</code>' +
      '<span class="file-purpose">' + esc(project.purpose) + '</span>' +
      '<span class="chip" data-state="idle">' + project.fileCount + ' 文件</span>' +
      (project.dirCount ? '<span class="chip" data-state="idle">' + project.dirCount + ' 目录</span>' : '') +
      '</summary>' +
      '<pre class="file-tree" data-file-count="' + project.fileCount + '" data-file-dir-count="' + project.dirCount + '">' +
      esc(project.tree) + '</pre></details>').join('');
  };
  const groups = [{
    kind: 'framework', label: '框架层（15 工程 + src 根文件）',
    count: frameworkEngineering.length, html: renderGroup(frameworkProjects)
  }];
  if (toolProjects.length) {
    groups.push({
      kind: 'tool', label: '工具层（' + toolProjects.length + ' 工程）',
      count: toolProjects.length, html: renderGroup(toolProjects)
    });
  }

  return {
    html: groups.map(group => group.html).join(''),
    groups,
    projects,
    tools: purposeMap.tools,
    counts: {
      projects: projects.length,
      frameworkProjects: frameworkEngineering.length,
      toolProjects: toolProjects.length,
      files, dirs, maxDepth, byExt,
      excluded: EXCLUDED.slice()
    }
  };
}

function renderFileTreeControls(tree) {
  return '<div class="filters">' +
    '<label>过滤工程<input id="file-query" type="search" placeholder="输入工程名或用途关键词" autocomplete="off"></label>' +
    '<label>分组<select id="file-kind"><option value="">全部（' + tree.counts.projects + '）</option>' +
    '<option value="framework">框架层（' + tree.counts.frameworkProjects + ' 工程）</option>' +
    (tree.counts.toolProjects > 0
      ? '<option value="tool">工具层（' + tree.counts.toolProjects + ' 工程）</option>'
      : '') + '</select></label>' +
    '<button type="button" class="filter-button" id="file-expand-all">展开全部</button>' +
    '<button type="button" class="filter-button" id="file-collapse-all">收起全部</button>' +
    '<span class="filter-count" id="file-count">' + tree.counts.projects + ' / ' + tree.counts.projects + '</span></div>';
}

module.exports = { buildFileTree, renderFileTreeControls, EXCLUDED };
