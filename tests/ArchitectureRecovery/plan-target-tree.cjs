'use strict';
// 自研目标架构源码树派生层。只读入 plan-model 已装配的模型与判断性映射台账；不写盘、不创建目录。
// 关键约束：exists 节点必须在磁盘真实存在；planned / unknown 节点必须不存在（待建不落盘）。
const fs = require('fs');
const path = require('path');

function must(condition, message) {
  if (!condition) throw new Error('[plan-target-tree] ' + message);
}
function uniq(items) { return [...new Set(items.filter(Boolean))]; }
function nodeId(relativePath) {
  return 'node-' + relativePath.replace(/[^A-Za-z0-9_-]+/g, '-').replace(/^-+|-+$/g, '');
}
function hasExcludedSegment(relativePath, excluded) {
  return relativePath.split('/').some(segment => excluded.includes(segment));
}

function buildTargetTree(model, map, root) {
  must(model && map && root, '缺少 model / map / root');
  must(map.schemaVersion === 1, '目标树映射台账 schemaVersion 不符');
  must(map.policy && map.policy.noDiskCreation === true, '必须声明待建项不落盘');
  must(Array.isArray(map.policy.excludedDirectories) && map.policy.excludedDirectories.length > 0,
    '缺少排除目录规则');
  must(map.layers.length === 8 && model.layers.length === 8, '目标树应为 8 层');
  must(map.layers.map(layer => layer.id).join(',') === model.layers.map(layer => layer.id).join(','),
    '映射台账层 ID 与架构基线不一致');

  const excluded = map.policy.excludedDirectories;
  const assemblyIds = new Set(model.assemblies.map(item => item.id));
  const packageIds = new Set(model.wbs.workPackages.map(item => item.id));
  const stageIds = new Set(model.stages.map(item => item.id));
  const projectNames = new Set(model.currentProjects.map(item => item.Name));
  const layerById = new Map(model.layers.map(layer => [layer.id, layer]));

  // 层 → 工作包（由 layers[].units 字符串解析）/ 层 → 阶段（由 priority.layers 反查）
  const layerPackageIds = new Map();
  const layerStageIds = new Map();
  for (const layer of model.layers) {
    const ids = uniq([...String(layer.units).matchAll(/[A-D]\d{2}/g)]
      .map(match => match[0]).filter(id => packageIds.has(id)));
    layerPackageIds.set(layer.id, ids);
    layerStageIds.set(layer.id, model.stages
      .filter(stage => stage.priority && stage.priority.layers.includes(layer.id))
      .map(stage => stage.id));
  }

  // L8 的层 units 是「插件线」，解析不出包 ID；用该层目标目录的编排关联兜底。
  const layerFallbackPackages = new Map();
  const layerFallbackStages = new Map();
  for (const mapped of map.layers) {
    const entries = (mapped.planned || []).concat(mapped.unknown || []);
    layerFallbackPackages.set(mapped.id, uniq(entries.flatMap(item => item.workPackageIds || [])));
    layerFallbackStages.set(mapped.id, uniq(entries.flatMap(item => item.stageIds || [])));
  }
  const packagesFor = layerId => {
    const parsed = layerPackageIds.get(layerId) || [];
    return parsed.length ? parsed : (layerFallbackPackages.get(layerId) || []);
  };
  const stagesFor = layerId => {
    const parsed = layerStageIds.get(layerId) || [];
    return parsed.length ? parsed : (layerFallbackStages.get(layerId) || []);
  };

  function validateRefs(item, label) {
    must(Array.isArray(item.workPackageIds) && item.workPackageIds.length > 0,
      label + ' 缺少工作包关联');
    must(Array.isArray(item.stageIds) && item.stageIds.length > 0, label + ' 缺少阶段关联');
    for (const id of item.workPackageIds) must(packageIds.has(id), label + ' 引用了不存在的工作包: ' + id);
    for (const id of item.stageIds) must(stageIds.has(id), label + ' 引用了不存在的阶段: ' + id);
  }
  function validatePath(relativePath, shouldExist, label) {
    must(!hasExcludedSegment(relativePath, excluded),
      label + ' 路径包含排除目录: ' + relativePath);
    const absolute = path.join(root, relativePath);
    const exists = fs.existsSync(absolute);
    must(exists === shouldExist,
      label + ' 磁盘状态不符（期望 ' + (shouldExist ? '存在' : '不存在') + '）: ' + relativePath);
  }

  const nodes = [];
  const pushNode = (node) => {
    must(!nodes.some(item => item.id === node.id), '目标树节点 ID 重复: ' + node.id);
    nodes.push(node);
    return node;
  };

  // 工程节点：29 个，全部来自 currentProjects；层归属来自 layers[].currentProjects。
  const projectLayerMap = new Map();
  for (const layer of model.layers) {
    for (const project of layer.currentProjects) {
      must(projectNames.has(project), '层 ' + layer.id + ' 引用了不存在的工程: ' + project);
      if (!projectLayerMap.has(project)) projectLayerMap.set(project, []);
      projectLayerMap.get(project).push(layer.id);
    }
  }
  let unassignedProjects = 0;
  for (const project of model.currentProjects) {
    const relativePath = 'src/' + project.Name;
    validatePath(relativePath, true, '工程节点');
    const mappedLayers = projectLayerMap.get(project.Name) || [];
    // 未纳入 8 层映射的工程仍要出现在树里，但归层状态显式登记为待补证，不臆造归属。
    const layerIds = mappedLayers.length ? mappedLayers : ['L0-unassigned'];
    if (!mappedLayers.length) unassignedProjects++;
    pushNode({
      id: nodeId(relativePath),
      path: relativePath,
      kind: 'project',
      project: project.Name,
      layerIds,
      status: 'exists',
      evidenceType: 'current-source',
      originalAssemblies: uniq(layerIds.flatMap(id => (layerById.get(id) || {}).assemblies || [])),
      workPackageIds: uniq(layerIds.flatMap(packagesFor)),
      stageIds: uniq(layerIds.flatMap(stagesFor)),
      rationale: mappedLayers.length
        ? '当前自研工程，来自 docs/architecture-baseline.json → currentProjects'
        : '当前自研工程，但未被 8 层映射表收录；归层待补证，不臆造归属',
      unknowns: mappedLayers.length ? [] : ['未纳入 8 层映射，需按原厂程序集与职责重新归层'],
      metrics: {
        csFiles: project.CsFiles, xamlFiles: project.XamlFiles,
        lines: project.Lines, types: project.Types, methods: project.Methods
      }
    });
  }
  must(nodes.filter(node => node.kind === 'project').length === 29, '工程节点应为 29 个');

  // 目录节点：已存在 / 待建 / 未知，来自判断性映射台账。
  const layers = map.layers.map(mapped => {
    const baselineLayer = layerById.get(mapped.id);
    must(baselineLayer, '映射层不存在: ' + mapped.id);
    const originalAssemblies = baselineLayer.assemblies || [];
    for (const assembly of originalAssemblies) {
      must(assemblyIds.has(assembly), '层 ' + mapped.id + ' 引用了不存在的原厂程序集: ' + assembly);
    }
    const layerNodes = [];
    for (const item of mapped.existingDirectories || []) {
      validatePath(item.path, true, '已存在目录');
      layerNodes.push(pushNode({
        id: nodeId(item.path),
        path: item.path,
        kind: 'directory',
        project: item.path.split('/')[1],
        layerIds: [mapped.id],
        status: 'exists',
        evidenceType: 'current-source',
        originalAssemblies,
        workPackageIds: packagesFor(mapped.id),
        stageIds: stagesFor(mapped.id),
        rationale: item.note,
        unknowns: [],
        metrics: null
      }));
    }
    for (const item of mapped.planned || []) {
      validateRefs(item, '待建目录 ' + item.path);
      validatePath(item.path, false, '待建目录');
      must(item.rationale && item.rationale.trim().length > 0, '待建目录缺少依据: ' + item.path);
      layerNodes.push(pushNode({
        id: nodeId(item.path),
        path: item.path,
        kind: 'directory',
        project: item.path.split('/')[1],
        layerIds: [mapped.id],
        status: 'planned',
        evidenceType: 'independent-implementation-planned',
        originalAssemblies,
        workPackageIds: item.workPackageIds,
        stageIds: item.stageIds,
        rationale: item.rationale,
        unknowns: map.policy.plannedUnknowns.slice(),
        metrics: null
      }));
    }
    for (const item of mapped.unknown || []) {
      validateRefs(item, '未知目录 ' + item.path);
      validatePath(item.path, false, '未知目录');
      must(item.rationale && item.rationale.trim().length > 0, '未知目录缺少登记理由: ' + item.path);
      layerNodes.push(pushNode({
        id: nodeId(item.path),
        path: item.path,
        kind: 'directory',
        project: item.path.split('/')[1],
        layerIds: [mapped.id],
        status: 'unknown',
        evidenceType: 'unknown',
        originalAssemblies,
        workPackageIds: item.workPackageIds,
        stageIds: item.stageIds,
        rationale: item.rationale,
        unknowns: [item.rationale, '需 REA / 台账补证后才能判定是否应建'],
        metrics: null
      }));
    }
    return {
      id: mapped.id,
      name: baselineLayer.name,
      target: mapped.target,
      status: baselineLayer.status,
      gap: baselineLayer.gap,
      originalAssemblies,
      currentProjects: baselineLayer.currentProjects,
      unitIds: packagesFor(mapped.id),
      stageIds: stagesFor(mapped.id),
      nodeIds: layerNodes.map(node => node.id),
      counts: {
        exists: layerNodes.filter(node => node.status === 'exists').length,
        planned: layerNodes.filter(node => node.status === 'planned').length,
        unknown: layerNodes.filter(node => node.status === 'unknown').length
      }
    };
  });

  // 依赖边：62 条 ProjectReference，端点必须全部落在工程节点内。
  const edges = model.currentEdges.map(edge => {
    must(projectNames.has(edge.From) && projectNames.has(edge.To),
      '依赖边端点不是当前工程: ' + edge.From + ' → ' + edge.To);
    return { from: edge.From, to: edge.To, files: edge.Files };
  });
  must(edges.length === 62, '当前依赖边应为 62 条，实际 ' + edges.length);

  // 层统计按「主层」计入工程节点与目录节点，避免页面只显示目录数。
  for (const layer of layers) {
    const owned = nodes.filter(node => node.layerIds[0] === layer.id);
    layer.nodeIds = owned.map(node => node.id);
    layer.counts = {
      exists: owned.filter(node => node.status === 'exists').length,
      planned: owned.filter(node => node.status === 'planned').length,
      unknown: owned.filter(node => node.status === 'unknown').length
    };
  }
  const unassignedNodes = nodes.filter(node => node.layerIds[0] === 'L0-unassigned');
  must(unassignedNodes.length === unassignedProjects, '未归层工程计数不一致');

  const existsCount = nodes.filter(node => node.status === 'exists').length;
  const plannedCount = nodes.filter(node => node.status === 'planned').length;
  const unknownCount = nodes.filter(node => node.status === 'unknown').length;
  must(existsCount + plannedCount + unknownCount === nodes.length, '节点状态分布不完整');
  must(plannedCount > 0 && unknownCount > 0, '待建与未知节点不应为空');

  return {
    schemaVersion: 1,
    policy: map.policy,
    layers,
    nodes,
    edges,
    counts: {
      layers: layers.length,
      nodes: nodes.length,
      projectNodes: nodes.filter(node => node.kind === 'project').length,
      directoryNodes: nodes.filter(node => node.kind === 'directory').length,
      exists: existsCount,
      planned: plannedCount,
      unknown: unknownCount,
      edges: edges.length,
      excludedDirectories: excluded.length,
      unassignedProjects
    }
  };
}

module.exports = { buildTargetTree, nodeId };
