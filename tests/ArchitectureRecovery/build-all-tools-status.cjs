// Reconcile the expanded user scope without turning declarations into implementation credit.
const fs = require('fs');
const path = require('path');
const crypto = require('crypto');
const root = path.resolve(__dirname, '../..');
const read = p => JSON.parse(fs.readFileSync(path.join(root, p), 'utf8').replace(/^\uFEFF/, ''));
const sha = p => crypto.createHash('sha256').update(fs.readFileSync(path.join(root, p))).digest('hex');
const inventory = read('docs/plugins/inventory.json');
const catalog = read('src/VisionEditor/tool-catalog.json');
const calibration = read('docs/SOURCE-LEVEL-REVERSE-PROGRESS.json');
const framework = read('docs/framework-progress-audit.json');
const mapped = new Map(catalog.Groups.flatMap(g => g.Items.map(t => [t.Id, { category: g.Id, moduleId: t.ModuleId ?? null }])));
const additionalSelfReviewed = new Map([
 ['BranchModule_STD', 'docs/plugins/BRANCH-MODULE-IMPLEMENTATION-EXIT.md'],
 ['BranchStringCpmL', 'docs/plugins/BRANCH-STRING-IMPLEMENTATION-EXIT.md'],
 ['IMVSFixtureModu', 'docs/FIXTURE-NATIVE-FIELD-AUDIT.md'],
 ['ImageAcquisitionModule', 'docs/plugins/IMAGE-ACQUISITION.md'],
 ['ImageBufferModule', 'docs/plugins/IMAGE-BUFFER.md'],
 ['LightModule', 'docs/plugins/LIGHT.md']
]);
const decompiledRoot = path.join(root, 'analysis/decompiled');
const folders = fs.existsSync(decompiledRoot) ? fs.readdirSync(decompiledRoot, { withFileTypes: true }).filter(e => e.isDirectory()).map(e => e.name) : [];
const rows = inventory.plugins.map(tool => {
 const existing = calibration.toolCompletion.find(t => tool.category === 'Calibration' && t.name === tool.displayName && t.complete);
 const additional = additionalSelfReviewed.get(tool.plugin);
 const report = existing?.evidence ? 'docs/' + existing.evidence : additional ?? null;
 const candidates = [...new Set(tool.binaryEvidence.filter(b => b.kind === 'managed-CLI').flatMap(binary => {
  const stem = path.win32.basename(binary.path, '.dll');
  return folders.filter(folder => folder.toLowerCase() === stem.toLowerCase() || folder.toLowerCase().startsWith(stem.toLowerCase() + '.'));
 }))];
 return {
  identity: tool.category + '/' + tool.plugin, name: tool.displayName,
  catalog: mapped.get(tool.plugin) ?? { category: null, moduleId: null },
  originalEvidence: { xmlFiles: tool.xmlEvidence.length, binaries: tool.binaryEvidence.length, parameters: tool.algorithmParameters.length, inputs: tool.inputs.length, outputs: tool.outputs.length },
  managedSourceCandidates: candidates,
  sourceCandidateBoundary: 'Folder-name matches are retrieval leads; not original artifact identity or recovered-behavior proof.',
  implementation: existing || additional ? 'prior-scoped-self-review-complete' : 'not-counted-as-complete',
  priorSelfReview: existing || additional ? { scope: existing ? calibration.scope : 'Branch tool-specific scope', report, reportExists: report ? fs.existsSync(path.join(root, report)) : false, manualAcceptance: 'separate' } : null,
  completionRule: 'Directory, source presence, catalog mapping and an openable editor do not count as a finished tool.'
 };
});
if (rows.length !== 162 || new Set(rows.map(r => r.identity)).size !== 162) throw Error('The authorized catalog must contain 162 unique identities.');
const categories = [...new Set(inventory.plugins.map(p => p.category))].map(category => ({
 category, tools: rows.filter(r => r.identity.startsWith(category + '/')).length,
 priorScopedComplete: rows.filter(r => r.identity.startsWith(category + '/') && r.priorSelfReview).length
}));
const completed = rows.filter(r => r.priorSelfReview).length;
const report = {
 date: '2026-11-11', authorization: 'User expanded work to the main interface and every installed directory tool; C03 historical replay does not block independent source-backed work.',
 scope: { frameworkUnits: 96, toolDirectories: 162, categories: categories.length, ownership: 'Current workspace only; no edits to the other worktree or silent integration.' },
 baseline: { frameworkImplementedAndReviewed: framework.summary.overallDone, frameworkPercent: framework.summary.overallCompletionPercent, priorScopedToolReviews: completed, expandedToolPercent: Number((100 * completed / rows.length).toFixed(2)), note: 'The ten inherited calibration reviews preserve their finite scope and parity limits; all remaining tools require actual implementation and self-review.' },
 inputHashes: Object.fromEntries(['docs/plugins/inventory.json', 'src/VisionEditor/tool-catalog.json', 'docs/SOURCE-LEVEL-REVERSE-PROGRESS.json', 'docs/framework-progress-audit.json'].map(p => [p, sha(p)])),
 executionOrder: ['Main interface and ordinary image-source operation chain', 'Shared configuration/execution/display contracts', 'Existing simple tool families with complete source bodies', 'Remaining algorithms and dedicated editors in evidence-backed batches', 'Platform/device services with their own environments'],
 acceptance: 'Headless tests in the agent; the user launches desktop acceptance from src/VisionEditor/bin/Release/net10.0-windows/VisionEditor.exe.',
 categories, tools: rows
};
fs.writeFileSync(path.join(root, 'docs/ALL-TOOLS-IMPLEMENTATION-STATUS.json'), JSON.stringify(report, null, 2) + '\n');
console.log(JSON.stringify({ tools: rows.length, categories: categories.length, catalogMapped: rows.filter(r => r.catalog.moduleId).length, sourceCandidateTools: rows.filter(r => r.managedSourceCandidates.length).length, framework: report.baseline.frameworkImplementedAndReviewed + '/96', priorScopedToolReviews: completed, expandedToolPercent: report.baseline.expandedToolPercent }, null, 2));
