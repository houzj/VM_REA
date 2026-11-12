const fs = require('fs');
const path = require('path');

const root = path.resolve(__dirname, '..', '..');
const gatePath = path.join(root, 'docs', 'reverse', 'C01-startup-implementation-gate.json');
const gate = JSON.parse(fs.readFileSync(gatePath, 'utf8'));
const failures = [];

if (!['deferred-until-design-gate', 'source-confirmed-shell-batch'].includes(gate.implementation.mode))
  failures.push('startup implementation mode is not recognized');
if (gate.implementation.batchPolicy !== 'all-source-confirmed-shell-stages-in-one-batch')
  failures.push('startup implementation must use the all-source-confirmed-shell-stages-in-one-batch policy');
if (!['deferred', 'partial-source-confirmed-shell'].includes(gate.implementation.status))
  failures.push('implementation status is not recognized');

const evidenceIds = new Set(gate.evidence.map(item => item.id));
for (const item of gate.evidence) {
  if (!item.source || !item.members?.length || !item.classification)
    failures.push(`evidence entry ${item.id} is incomplete`);
}
for (const change of gate.plannedChanges) {
  if (!change.evidence?.length || change.evidence.some(id => !evidenceIds.has(id)))
    failures.push(`planned change ${change.id} has no valid evidence mapping`);
  if (change.classification === 'runtime-unconfirmed' && ['implement', 'implement-partial', 'implement-with-independent-mapping'].includes(change.action))
    failures.push(`runtime-unconfirmed behavior ${change.id} cannot be implemented`);
  if (change.classification === 'runtime-observation-only' && change.action === 'implement')
    failures.push(`observation-only behavior ${change.id} cannot be implemented`);
  if (change.action === 'defer-nonblocking' && change.classification !== 'runtime-unconfirmed')
    failures.push(`nonblocking deferral ${change.id} must remain explicitly runtime-unconfirmed`);
}
const deferredUnknowns = gate.plannedChanges.filter(item => item.classification.includes('unconfirmed') && item.action === 'defer');
if (deferredUnknowns.length === 0) failures.push('at least one unresolved startup behavior must remain explicitly deferred');
if (!gate.plannedChanges.some(item => item.id === 'license-and-dongle-provider' && item.action === 'defer-nonblocking'))
  failures.push('license-and-dongle provider must be deferred without blocking startup');
if (!gate.requiredVerification.includes('manual host comparison by user')) failures.push('manual host comparison must remain a final verification step');

if (failures.length) {
  console.error('STARTUP DESIGN GATE: FAIL');
  for (const failure of failures) console.error(`- ${failure}`);
  process.exitCode = 1;
} else {
  console.log(`STARTUP DESIGN GATE: PASS (${gate.evidence.length} evidence entries, ${gate.plannedChanges.length} planned changes, ${deferredUnknowns.length} deferred unknowns)`);
}
