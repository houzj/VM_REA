'use strict';
// 原厂 25 个唯一程序集的源码级成员清单：从 analysis/decompiled 的 canonical 树批量提取。
// 只做静态文本取证（source-confirmed-decompiled），不推断运行时行为、不补缺失函数体。
// 纯函数：目录顺序固定，不写 wall-clock、不用随机数。
const fs = require('fs');
const path = require('path');
const lib = require('./plan-model.cjs');

const root = lib.root;
const OUT_JSON = path.join(root, 'tests/ArchitectureRecovery/original-members.map.json');
const OUT_MD = path.join(root, 'docs/ORIGINAL-MEMBERS-INDEX.md');
const EXCLUDED = new Set(['bin', 'obj', '.git', '.a01group', '.pending', 'TestResults']);
const MAX_METHODS_PER_TYPE = 24;

function must(condition, message) {
  if (!condition) throw new Error('[build-original-members] ' + message);
}
function rel(p) { return path.relative(root, p).replace(/\\/g, '/'); }

function collectCs(dirAbs) {
  const files = [];
  (function walk(dir) {
    const entries = fs.readdirSync(dir, { withFileTypes: true })
      .filter(entry => !EXCLUDED.has(entry.name))
      .sort((a, b) => a.name < b.name ? -1 : a.name > b.name ? 1 : 0);
    for (const entry of entries) {
      const p = path.join(dir, entry.name);
      if (entry.isDirectory()) walk(p);
      else if (entry.name.endsWith('.cs')) files.push(p);
    }
  })(dirAbs);
  return files;
}

const TYPE_RE = /^\s*(?:\[[^\]]*\]\s*)*(?:(?:public|internal|protected|private|static|sealed|abstract|partial|unsafe|readonly|ref|new)\s+)*(class|struct|interface|enum|record|delegate)\s+([A-Za-z_][A-Za-z0-9_<>,\.]*)/gm;
const NS_RE = /^\s*namespace\s+([A-Za-z_][A-Za-z0-9_\.]*)/gm;
const METHOD_RE = /^\s*(?:\[[^\]]*\]\s*)*(?:(?:public|internal|protected|private|static|sealed|abstract|partial|virtual|override|extern|unsafe|async|new|readonly)\s+)+(?:[A-Za-z_][A-Za-z0-9_<>,\.\?\[\]]*\s+)+([A-Za-z_][A-Za-z0-9_]*)\s*\(([^\);]{0,200})\)/gm;

function extractTypeMembers(text, fileRel) {
  const namespaces = [...new Set([...text.matchAll(NS_RE)].map(m => m[1]))];
  const types = [];
  for (const m of text.matchAll(TYPE_RE)) {
    types.push({ kind: m[1], name: m[2], file: fileRel });
  }
  const methods = [];
  for (const m of text.matchAll(METHOD_RE)) {
    const name = m[1];
    if (['if', 'for', 'while', 'switch', 'catch', 'using', 'lock', 'return', 'new', 'get', 'set'].includes(name)) continue;
    methods.push({
      name,
      params: m[2].replace(/\s+/g, ' ').trim().slice(0, 120),
      file: fileRel
    });
  }
  return { namespaces, types, methods, lines: text.split('\n').length };
}

function build() {
  const model = lib.buildModel();
  lib.verifyInputs(model);
  must(Array.isArray(model.assemblies) && model.assemblies.length === 25, '原厂唯一程序集应为 25 个');
  const decompiled = path.join(root, 'analysis', 'decompiled');
  must(fs.existsSync(decompiled), '缺少 analysis/decompiled');

  const treeCount = model.assemblies.reduce((n, a) => n + (a.trees ? a.trees.length : 0), 0);
  must(treeCount === 28, '反编译树应为 28 棵，实际 ' + treeCount);
  const duplicatePasses = model.assemblies.filter(a => a.duplicatePasses).length;
  must(duplicatePasses === 3, '重复 pass 程序集应为 3 个，实际 ' + duplicatePasses);

  const assemblies = [];
  for (const asm of model.assemblies) {
    const dir = path.join(decompiled, asm.canonicalTree);
    must(fs.existsSync(dir), '缺少 canonical 树目录: ' + asm.canonicalTree);
    const files = collectCs(dir);
    const namespaces = new Set();
    const types = [];
    const methods = [];
    let lines = 0;
    for (const file of files) {
      const text = fs.readFileSync(file, 'utf8');
      const found = extractTypeMembers(text, rel(file));
      for (const ns of found.namespaces) namespaces.add(ns);
      for (const type of found.types) types.push(type);
      for (const method of found.methods) methods.push(method);
      lines += found.lines;
    }
    const byType = new Map();
    for (const method of methods) {
      const key = method.file;
      if (!byType.has(key)) byType.set(key, []);
      byType.get(key).push(method);
    }
    const typeList = types.map(type => ({
      kind: type.kind,
      name: type.name,
      file: type.file,
      methods: (byType.get(type.file) || []).slice(0, MAX_METHODS_PER_TYPE).map(m => m.name + '(' + m.params + ')'),
      methodsTruncated: Math.max(0, (byType.get(type.file) || []).length - MAX_METHODS_PER_TYPE)
    }));
    assemblies.push({
      id: asm.id,
      canonicalTree: asm.canonicalTree,
      trees: asm.trees,
      duplicatePasses: !!asm.duplicatePasses,
      layerIds: (model.layers || []).filter(layer => (layer.assemblies || []).includes(asm.id)).map(layer => layer.id),
      baseline: {
        cs: asm.cs, xaml: asm.xaml, lines: asm.lines,
        types: asm.types, methods: asm.methods, fanIn: asm.fanIn
      },
      scan: {
        csFiles: files.length,
        lines,
        namespaces: [...namespaces].sort(),
        declaredTypes: types.length,
        methodSignatures: methods.length
      },
      types: typeList
    });
  }

  const totals = {
    assemblies: assemblies.length,
    treePasses: treeCount,
    duplicatePasses,
    canonicalCsFiles: assemblies.reduce((n, a) => n + a.scan.csFiles, 0),
    scannedLines: assemblies.reduce((n, a) => n + a.scan.lines, 0),
    declaredTypes: assemblies.reduce((n, a) => n + a.scan.declaredTypes, 0),
    methodSignatures: assemblies.reduce((n, a) => n + a.scan.methodSignatures, 0),
    baselineUniqueCs: model.metrics.original.cs,
    baselineUniqueLines: model.metrics.original.lines,
    baselineTypes: model.metrics.original.types,
    baselineMethods: model.metrics.original.methods
  };

  const ledger = {
    schemaVersion: 1,
    note: '原厂 25 个唯一程序集的源码级成员清单；按 canonical 归并 28 棵反编译树、3 个重复 pass 不累加。全部来自静态反编译文本（source-confirmed-decompiled），不是运行时证据，不含函数体实现。',
    evidenceBoundary: '类型/方法签名由正则从反编译 .cs 文本提取，可能遗漏自动生成代码、显式接口实现、属性访问器与局部函数；计数与 architecture-baseline.json 的去重口径允许不一致，差异原样登记，不强行对齐。',
    builtFrom: model.builtFrom.head.slice(0, 8) + '/' + model.builtFrom.inputsDigest.slice(0, 12),
    totals,
    assemblies
  };

  const lines = [
    '# 原厂 25 个唯一程序集源码级成员清单（索引）',
    '',
    '机器台账：`tests/ArchitectureRecovery/original-members.map.json` · builtFrom `' + ledger.builtFrom + '`',
    '',
    '按 `canonicalTree` 归并 28 棵反编译树，3 个重复 pass 不累加；只扫描排除 `bin/obj/.git/.a01group/.pending/TestResults` 后的 `.cs`。',
    '',
    '**证据边界**：类型与方法签名来自静态反编译文本的正则提取，属 `source-confirmed-decompiled`，不是运行时证据；不含函数体，不用推断补齐。扫描计数与 `docs/architecture-baseline.json` 去重口径（' +
      totals.baselineUniqueCs + ' `.cs` / ' + totals.baselineUniqueLines + ' 行 / ' + totals.baselineTypes + ' 类型 / ' + totals.baselineMethods + ' 方法）不同，差异原样保留。',
    '',
    '## 汇总',
    '',
    '| 项 | 值 |',
    '|---|---|',
    '| 唯一程序集 | ' + totals.assemblies + ' |',
    '| 反编译树（含重复 pass） | ' + totals.treePasses + ' |',
    '| 重复 pass 程序集 | ' + totals.duplicatePasses + ' |',
    '| canonical 树 `.cs` 文件 | ' + totals.canonicalCsFiles + ' |',
    '| 扫描行数 | ' + totals.scannedLines + ' |',
    '| 提取类型声明 | ' + totals.declaredTypes + ' |',
    '| 提取方法签名 | ' + totals.methodSignatures + ' |',
    '',
    '## 25 个程序集',
    '',
    '| 程序集 | canonical 树 | 层 | `.cs` | 类型 | 方法签名 | 命名空间数 | 重复 pass |',
    '|---|---|---|---|---|---|---|---|',
    ...assemblies.map(a => '| ' + a.id + ' | ' + a.canonicalTree + ' | ' + (a.layerIds.join('、') || '—') +
      ' | ' + a.scan.csFiles + ' | ' + a.scan.declaredTypes + ' | ' + a.scan.methodSignatures +
      ' | ' + a.scan.namespaces.length + ' | ' + (a.duplicatePasses ? '是' : '否') + ' |'),
    '',
    '每类型方法名最多列 ' + MAX_METHODS_PER_TYPE + ' 个，超出在台账中记 `methodsTruncated`，完整清单见机器台账 `assemblies[].types[]`。',
    '',
    '## 口径冲突与未完成项',
    '',
    '- `42/96=43.75%` 与 `41/96=42.71%` 并列保留；`PositionCorrection` 批次身份差异不擅自统一。',
    '- C03 差分 `45/971486` 对 `47/973202` 仍开放；本清单不解释根因，不声称 parity。',
    '- 缺失函数体、资源与 ABI 仍是 REA 补证任务，本清单不生成实现代码。'
  ];

  return { ledger, markdown: lines.join('\n') };
}

function main() {
  const result = build();
  fs.writeFileSync(OUT_JSON, JSON.stringify(result.ledger, null, 2) + '\n', 'utf8');
  fs.writeFileSync(OUT_MD, result.markdown, 'utf8');
  console.log(JSON.stringify({
    assemblies: result.ledger.totals.assemblies,
    treePasses: result.ledger.totals.treePasses,
    canonicalCsFiles: result.ledger.totals.canonicalCsFiles,
    declaredTypes: result.ledger.totals.declaredTypes,
    methodSignatures: result.ledger.totals.methodSignatures,
    jsonBytes: Buffer.byteLength(JSON.stringify(result.ledger, null, 2) + '\n', 'utf8'),
    mdBytes: Buffer.byteLength(result.markdown, 'utf8')
  }, null, 2));
}

if (require.main === module) main();
module.exports = { build };
