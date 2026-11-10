'use strict';
// 162 个目录工具的实现规格台账：从 docs/plugins/inventory.json 的证据字段批量派生六项规格。
// 只做证据整理与分级，不生成实现代码、不推断 native 函数体；缺失项一律标 unknown。
// 纯函数：输入顺序固定，不写 wall-clock、不用随机数。
const fs = require('fs');
const path = require('path');
const lib = require('./plan-model.cjs');

const root = lib.root;
const OUT_JSON = path.join(root, 'tests/ArchitectureRecovery/tool-implementation-specs.map.json');
const OUT_MD = path.join(root, 'docs/TOOL-IMPLEMENTATION-SPECS.md');

function must(condition, message) {
  if (!condition) throw new Error('[build-tool-specs] ' + message);
}
function arr(value) { return Array.isArray(value) ? value : []; }
function sha8(value) { return String(value || '').slice(0, 8); }
function basename(value) { return String(value || '').split(/[\\/]/).pop(); }

function evidenceLevel(tool) {
  const xml = arr(tool.xmlEvidence).length;
  const bin = arr(tool.binaryEvidence).length;
  const res = arr(tool.resourceEvidence).length;
  const params = arr(tool.algorithmParameters).length;
  const inputs = arr(tool.inputs).length;
  const outputs = arr(tool.outputs).length;
  const score = xml * 2 + bin * 2 + res + params + inputs + outputs;
  return {
    score,
    band: score >= 12 ? 'high' : score >= 7 ? 'mid' : 'low',
    counts: { xml, binary: bin, resource: res, parameters: params, inputs, outputs }
  };
}

function build() {
  const model = lib.buildModel();
  lib.verifyInputs(model);
  must(Array.isArray(model.tools) && model.tools.length === 162, '目录工具应为 162 个');
  must(Array.isArray(model.categories) && model.categories.length === 14, '分类应为 14 个');

  const grouped = new Map(model.categories.map(c => [c.name, c]));
  const tools = model.tools.map((tool, i) => {
    const details = tool.details || {};
    const xml = arr(details.xmlEvidence ?? tool.xmlEvidence);
    const binary = arr(details.binaryEvidence ?? tool.binaryEvidence);
    const resource = arr(details.resourceEvidence ?? tool.resourceEvidence);
    const params = arr(details.algorithmParameters ?? tool.algorithmParameters);
    const inputs = arr(details.inputs ?? tool.inputs);
    const outputs = arr(details.outputs ?? tool.outputs);
    const level = evidenceLevel({
      xmlEvidence: xml, binaryEvidence: binary, resourceEvidence: resource,
      algorithmParameters: params, inputs, outputs
    });

    const nativeKinds = [...new Set(binary.map(item => item.kind).filter(Boolean))];
    const identity = {
      plugin: tool.key,
      displayName: tool.displayName,
      category: tool.category,
      moduleId: tool.moduleId || null,
      sourcePath: tool.sourcePath,
      implementationStatus: tool.implementationStatus || 'unknown',
      owner: tool.owner || null,
      priority: tool.priority || null,
      batch: tool.batch || null
    };
    const resources = {
      xml: xml.map(item => ({ file: basename(item.path), sha256: item.sha256 ? sha8(item.sha256) : null, root: item.root || null })),
      binary: binary.map(item => ({ file: basename(item.path), sha256: item.sha256 ? sha8(item.sha256) : null, kind: item.kind || null, size: item.size || null })),
      resource: resource.map(item => ({ file: basename(item.path), sha256: item.sha256 ? sha8(item.sha256) : null }))
    };
    const io = {
      parameters: params.map(item => typeof item === 'string' ? item : (item.name || item.key || JSON.stringify(item).slice(0, 80))),
      inputs: inputs.map(item => typeof item === 'string' ? item : (item.name || item.key || JSON.stringify(item).slice(0, 80))),
      outputs: outputs.map(item => typeof item === 'string' ? item : (item.name || item.key || JSON.stringify(item).slice(0, 80))),
      uiTabs: arr(details.uiTabs ?? tool.uiTabs).length,
      uiControls: arr(details.uiControls ?? tool.uiControls).length
    };
    const unknowns = [];
    if (!binary.length) unknowns.push('无 DLL 证据');
    if (!params.length) unknowns.push('无算法参数声明');
    if (!inputs.length) unknowns.push('无输入端口声明');
    if (!outputs.length) unknowns.push('无输出名声明');
    if (!nativeKinds.length) unknowns.push('native/managed 分类未登记');
    unknowns.push('Process native 函数体、ABI 与首调用目标未取证（REA provider 不可用）');
    unknowns.push('原厂运行对比未做，不声称 parity');

    return {
      index: i + 1,
      taskCardAnchor: '13-tool-cards.html#toolcard-' + (i + 1),
      identity,
      evidenceLevel: level,
      resources,
      io,
      nativeBoundary: {
        kinds: nativeKinds,
        declaredLibraries: details.declaredLibraries || tool.declaredLibraries || null,
        state: 'not-recovered'
      },
      implementationState: tool.implementationStatus === 'partial-independent-local-BMP-MONO8'
        ? 'partial' : 'not-established',
      acceptance: [
        '身份与资源哈希可复核',
        '参数 IO 与输出名契约对齐',
        'Process native 路径取得原厂证据',
        '当前构建产物操作级复跑',
        'unknown 逐项登记解除条件',
        '不以测试通过冒充原厂 parity'
      ],
      unknowns
    };
  });

  const bands = tools.reduce((acc, t) => { acc[t.evidenceLevel.band] = (acc[t.evidenceLevel.band] || 0) + 1; return acc; }, {});
  const statuses = tools.reduce((acc, t) => { acc[t.implementationState] = (acc[t.implementationState] || 0) + 1; return acc; }, {});
  const categories = model.categories.map(category => ({
    name: category.name,
    declared: category.declared,
    tools: category.tools.length,
    high: tools.filter(t => t.identity.category === category.name && t.evidenceLevel.band === 'high').length
  }));

  const ledger = {
    schemaVersion: 1,
    note: '162 个目录工具的实现规格台账；证据只来自 docs/plugins/inventory.json 与 model.tools 的带哈希字段。全部为待逆向实现目标，不含实现代码。',
    evidenceBoundary: 'XML/DLL/资源声明是静态存在性证据，不是原厂行为或 ABI 结论；native 函数体、首调用目标与运行时输出仍需 REA 补证，一律标 unknown。输出名声明不等于已实现。',
    builtFrom: model.builtFrom.head.slice(0, 8) + '/' + model.builtFrom.inputsDigest.slice(0, 12),
    counts: {
      tools: tools.length,
      categories: categories.length,
      evidenceBands: bands,
      implementationStates: statuses,
      withXml: tools.filter(t => t.resources.xml.length > 0).length,
      withBinary: tools.filter(t => t.resources.binary.length > 0).length,
      withResource: tools.filter(t => t.resources.resource.length > 0).length,
      withParameters: tools.filter(t => t.io.parameters.length > 0).length,
      withInputs: tools.filter(t => t.io.inputs.length > 0).length,
      withOutputs: tools.filter(t => t.io.outputs.length > 0).length,
      totalEvidenceFiles: tools.reduce((n, t) => n + t.evidenceLevel.counts.xml + t.evidenceLevel.counts.binary + t.evidenceLevel.counts.resource, 0)
    },
    categories,
    tools
  };

  const lines = [
    '# 162 个目录工具实现规格（索引）',
    '',
    '机器台账：`tests/ArchitectureRecovery/tool-implementation-specs.map.json` · builtFrom `' + ledger.builtFrom + '`',
    '',
    '**全部为待逆向实现目标**：' + statuses['not-established'] + ' 个未建立、' + statuses['partial'] + ' 个 partial；不按自研工程映射、不声称等价。',
    '',
    '## 证据覆盖',
    '',
    '| 项 | 数量 |',
    '|---|---|',
    '| 工具 | ' + ledger.counts.tools + ' |',
    '| 分类 | ' + ledger.counts.categories + ' |',
    '| 有 XML 证据 | ' + ledger.counts.withXml + ' |',
    '| 有 DLL 证据 | ' + ledger.counts.withBinary + ' |',
    '| 有资源证据 | ' + ledger.counts.withResource + ' |',
    '| 有算法参数 | ' + ledger.counts.withParameters + ' |',
    '| 有输入端口 | ' + ledger.counts.withInputs + ' |',
    '| 有输出名 | ' + ledger.counts.withOutputs + ' |',
    '| 证据文件总数 | ' + ledger.counts.totalEvidenceFiles + ' |',
    '| 证据分级 high / mid / low | ' + (bands.high || 0) + ' / ' + (bands.mid || 0) + ' / ' + (bands.low || 0) + ' |',
    '',
    '## 14 分类',
    '',
    '| 分类 | 工具数 | 声明数 | high 证据 |',
    '|---|---|---|---|',
    ...categories.map(c => '| ' + c.name + ' | ' + c.tools + ' | ' + c.declared + ' | ' + c.high + ' |'),
    '',
    '## 每工具六项规格',
    '',
    '台账 `tools[]` 每项含：身份与状态、资源哈希（XML/DLL/资源，SHA-256 前 8 位）、参数 IO 与输出名、native 边界（kind / 声明库 / `not-recovered`）、实现状态与验收项、unknown 列表，并挂 `13-tool-cards.html#toolcard-N` 任务卡锚点。',
    '',
    '## 边界',
    '',
    '- 静态证据充分度 `high` 只表示声明性材料齐备，**不等于**函数体已取得或可实现。',
    '- `Process native 函数体、ABI 与首调用目标` 全部标 `not-recovered`；REA native provider 当前不可用。',
    '- 不以合理设计填补缺失函数体/资源/ABI，不把 XML 声明写成已实现，不把测试通过写成 parity。',
    '- C03 差分 `45/971486` 对 `47/973202` 仍开放；`IMVSBlobFindModu` 相关算法不在本批实现范围。'
  ];

  return { ledger, markdown: lines.join('\n') };
}

function main() {
  const result = build();
  fs.writeFileSync(OUT_JSON, JSON.stringify(result.ledger, null, 2) + '\n', 'utf8');
  fs.writeFileSync(OUT_MD, result.markdown, 'utf8');
  console.log(JSON.stringify({
    tools: result.ledger.counts.tools,
    categories: result.ledger.counts.categories,
    bands: result.ledger.counts.evidenceBands,
    states: result.ledger.counts.implementationStates,
    evidenceFiles: result.ledger.counts.totalEvidenceFiles,
    jsonBytes: Buffer.byteLength(JSON.stringify(result.ledger, null, 2) + '\n', 'utf8'),
    mdBytes: Buffer.byteLength(result.markdown, 'utf8')
  }, null, 2));
}

if (require.main === module) main();
module.exports = { build };
