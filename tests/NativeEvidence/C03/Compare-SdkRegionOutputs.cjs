const assert = require('node:assert/strict');
const { createHash } = require('node:crypto');
const { readFileSync } = require('node:fs');
const { join } = require('node:path');

const [matrixDir] = process.argv.slice(2);
assert(matrixDir, 'Usage: node Compare-SdkRegionOutputs.cjs <SDK matrix directory>');
const width = 1280;
const height = 960;
const pixelCount = width * height;
const sha256 = bytes => createHash('sha256').update(bytes).digest('hex');

function readRun(mode, attempt) {
  const directory = join(matrixDir, `${mode}-${attempt}`);
  const input = readFileSync(join(directory, 'input.bin'));
  const binary = readFileSync(join(directory, 'binary.bin'));
  const blob = readFileSync(join(directory, 'blob.bin'));
  const log = readFileSync(join(directory, 'probe.log'));
  for (const buffer of [input, binary, blob]) assert.equal(buffer.length, pixelCount);
  for (const mask of [binary, blob]) {
    assert(mask.every(pixel => pixel === 0 || pixel === 255), 'Expected a binary mask');
  }
  const rows = [...log.toString().matchAll(/^blob\[(\d+)\] area=(\d+)/gm)]
    .map(([, index, area]) => ({ index: Number(index), area: Number(area) }));
  const [, count, total] = log.toString().match(/^result count=(\d+) total=(\d+)/m);
  assert.equal(rows.length, Number(count));
  assert.equal(rows.reduce((sum, row) => sum + row.area, 0), Number(total));
  return { input, binary, blob, rows, logSha256: sha256(log) };
}

function compareMask(before, after) {
  const removedByBorderDistance = {};
  let added = 0;
  for (let i = 0; i < before.length; i++) {
    if (before[i] === after[i]) continue;
    if (after[i]) {
      added++;
      continue;
    }
    const x = i % width;
    const y = Math.floor(i / width);
    const distance = Math.min(x, y, width - 1 - x, height - 1 - y);
    removedByBorderDistance[distance] = (removedByBorderDistance[distance] ?? 0) + 1;
  }
  return { added, removedByBorderDistance };
}

function labelComponents(mask, defaultLabels) {
  const labels = new Int32Array(pixelCount);
  const queue = new Int32Array(pixelCount);
  const components = [];
  for (let i = 0; i < mask.length; i++) {
    if (!mask[i] || labels[i]) continue;
    const id = components.length + 1;
    const overlappingDefaultIds = new Set();
    labels[i] = id;
    queue[0] = i;
    let area = 1;
    for (let j = 0; j < area; j++) {
      const pixel = queue[j];
      if (defaultLabels) overlappingDefaultIds.add(defaultLabels[pixel]);
      const x = pixel % width;
      const y = Math.floor(pixel / width);
      for (let row = Math.max(0, y - 1); row <= Math.min(height - 1, y + 1); row++) {
        for (let column = Math.max(0, x - 1); column <= Math.min(width - 1, x + 1); column++) {
          const next = row * width + column;
          if (!mask[next] || labels[next]) continue;
          labels[next] = id;
          queue[area++] = next;
        }
      }
    }
    components.push({ id, area, firstPixel: [i % width, Math.floor(i / width)], overlappingDefaultIds });
  }
  return { labels, components };
}

function compareAttempt(attempt) {
  const before = readRun('default', attempt);
  const after = readRun('full-roi', attempt);
  assert(before.input.equals(after.input), 'ROI comparison requires identical input pixels');
  const binaryImage = compareMask(before.binary, after.binary);
  const blobImage = compareMask(before.blob, after.blob);
  const defaults = labelComponents(before.blob);
  const cropped = labelComponents(after.blob, defaults.labels);
  for (const [run, set] of [[before, defaults], [after, cropped]]) {
    assert.deepEqual(set.components.map(item => item.area).sort((a, b) => b - a),
      run.rows.map(row => row.area), 'BlobImage 8-connected areas must match the SDK vector');
  }
  const retained = new Map();
  for (const component of cropped.components) {
    const [id] = component.overlappingDefaultIds;
    assert(id && component.overlappingDefaultIds.size === 1, 'ROI component must have one default parent');
    const areas = retained.get(id) ?? [];
    areas.push(component.area);
    retained.set(id, areas);
  }
  const roiBinaryAreas = new Uint32Array(defaults.components.length + 1);
  for (let i = 0; i < pixelCount; i++) {
    if (after.binary[i]) roiBinaryAreas[defaults.labels[i]]++;
  }
  const changedComponents = defaults.components.flatMap(({ id, area, firstPixel }) => {
    const roiAreas = retained.get(id) ?? [];
    if (roiAreas.length === 1 && roiAreas[0] === area) return [];
    return [{ firstPixel, defaultArea: area, roiBinaryArea: roiBinaryAreas[id], roiAreas }];
  }).sort((a, b) => b.defaultArea - a.defaultArea);
  return {
    evidence: {
      inputSha256: sha256(before.input),
      default: { log: before.logSha256, binary: sha256(before.binary), blob: sha256(before.blob) },
      fullRoi: { log: after.logSha256, binary: sha256(after.binary), blob: sha256(after.blob) },
    },
    binaryImage,
    blobImage,
    defaultCount: defaults.components.length,
    fullRoiCount: cropped.components.length,
    changedComponents,
  };
}

const attempts = [compareAttempt(1), compareAttempt(2)];
const [{ evidence: first, ...diff }, { evidence: second, ...repeat }] = attempts;
assert.deepEqual(diff, repeat, 'SDK attempts must produce the same spatial difference');
console.log(JSON.stringify({ scope: 'Direct SDK output buffers', width, height, evidence: [first, second], ...diff }, null, 2));
