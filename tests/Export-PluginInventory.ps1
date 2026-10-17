param(
    [string]$EvidenceRoot = 'D:/MyWork/REA逆向测试/analysis/full-audit',
    [string]$OutputDirectory = (Join-Path $PSScriptRoot '../docs/plugins'),
    [switch]$ResetChecklist
)
$ErrorActionPreference = 'Stop'
$source = Join-Path $EvidenceRoot 'plugin-coverage.json'
$audit = Get-Content -LiteralPath $source -Raw -Encoding utf8 | ConvertFrom-Json
$catalog = Get-Content -LiteralPath (Join-Path $PSScriptRoot '../src/VisionEditor/tool-catalog.json') -Raw -Encoding utf8 | ConvertFrom-Json
$items = @($catalog.Groups | ForEach-Object { $_.Items })
New-Item -ItemType Directory -Force -Path $OutputDirectory | Out-Null
$records = @(foreach ($plugin in $audit.plugins) {
    $item = @($items | Where-Object Id -eq $plugin.plugin) | Select-Object -First 1
    $imageSource = $plugin.plugin -eq 'ImageSourceModule'
    $dependencyFile = Join-Path $plugin.path 'Librarys.Config'
    $dependencies = if (Test-Path -LiteralPath $dependencyFile) { Get-Content -LiteralPath $dependencyFile -Raw -Encoding utf8 } else { $null }
    [pscustomobject][ordered]@{
        category = $plugin.category
        plugin = $plugin.plugin
        displayName = if ($item) { $item.Name } else { $plugin.plugin }
        moduleId = if ($item) { $item.ModuleId } else { $null }
        priority = if ($imageSource) { 'P0-first-delivery' } elseif ($plugin.category -eq 'Collection') { 'P1-after-image-source' } else { 'P2-not-started' }
        owner = if ($plugin.category -in @('Logic','Global','SplitCombination')) { 'coordinate-framework-ownership-before-implementation' } else { 'plugins' }
        sourcePath = $plugin.path
        algorithmParameters = @($plugin.algorithm_parameters)
        inputs = @($plugin.input_declarations)
        outputs = @($plugin.output_declarations)
        uiTabs = @($plugin.ui_tabs)
        uiControls = @($plugin.ui_controls | Select-Object type,name)
        sharedUiGroups = @($plugin.group_link_names)
        xmlEvidence = @($plugin.xmls | Select-Object path,sha256,root,error)
        binaryEvidence = @($plugin.binaries | Select-Object path,sha256,kind,size)
        catalogIcon = if ($item) { $item.Icon } else { $null }
        resourceEvidence = @(if (Test-Path -LiteralPath $plugin.path) {
            Get-ChildItem -LiteralPath $plugin.path -File | Where-Object Extension -in @('.png','.ico','.xaml') | ForEach-Object {
                [ordered]@{ path = $_.FullName; sha256 = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant() }
            }
        })
        declaredLibraries = $dependencies
        dependencyStatus = 'declarations-and-binary-identity-only; transitive imports not verified'
        staticStatus = if ($imageSource) { 'managed mode rules, native Process mapping and four list helpers analyzed; see IMAGE-SOURCE.md' } else { 'XML-and-PE-inventory; semantic audit pending' }
        implementationStatus = if ($imageSource) { 'partial-independent-local-BMP-MONO8' } else { 'native-equivalent-plugin-not-established' }
        runtimeStatus = if ($imageSource) { 'original SDK state scenarios and sample pixel hash matched; complete mode UI/provider comparison pending' } else { 'original-versus-reconstruction-comparison-pending' }
        nextAction = if ($imageSource) { 'map Process state, decode, list selection and mode UI; resolve shared contracts' } else { 'trace registration, managed UI, native execution, resources and dependencies' }
    }
})
if ($records.Count -ne $audit.summary.plugin_count) { throw 'Plugin count mismatch' }
if (@($records | ForEach-Object { $_.category + '/' + $_.plugin } | Sort-Object -Unique).Count -ne $records.Count) { throw 'Duplicate plugin identity' }
$native = @($records | ForEach-Object { $_.binaryEvidence } | Where-Object kind -eq 'native-PE').Count
$managed = @($records | ForEach-Object { $_.binaryEvidence } | Where-Object kind -eq 'managed-CLI').Count
if ($native -ne $audit.summary.plugin_dll_kinds.'native-PE' -or $managed -ne $audit.summary.plugin_dll_kinds.'managed-CLI') { throw 'Binary totals mismatch' }
[ordered]@{
    schema = 1
    source = $source
    sourceSha256 = (Get-FileHash -LiteralPath $source -Algorithm SHA256).Hash.ToLowerInvariant()
    scope = '162 installed plugin folders including Global; declarations do not prove behavior or loadability'
    summary = $audit.summary
    plugins = $records
    sharedControls = $audit.shared_controls
    sharedLibraries = $audit.myLibs
} | ConvertTo-Json -Depth 60 -Compress | Set-Content -LiteralPath (Join-Path $OutputDirectory 'inventory.json') -Encoding utf8
$lines = @('# 逐插件逆向任务表', '', '机器可读完整声明、二进制身份、依赖声明和证据索引见 [inventory.json](inventory.json)。P0 图像源先完成；P1 其余采集插件；P2 暂未开始，不表示已确定最终实施顺序。涉及流程和全局资源的插件先与框架线明确归属。', '', '| 优先级 | 类别 | 插件 | 名称 | 参数/输入/输出 | UI类型 | 逆向状态 |', '|---|---|---|---|---|---|---|')
foreach ($record in ($records | Sort-Object priority,category,plugin)) {
    $types = @($record.uiControls.type | Sort-Object -Unique) -join ', '
    $counts = "$($record.algorithmParameters.Count)/$($record.inputs.Count)/$($record.outputs.Count)"
    $state = if ($record.plugin -eq 'ImageSourceModule') { '本地状态和样本像素已有原厂对照；全模式UI及服务未完整验收' } else { '声明已清点；语义与运行待逆向' }
    $lines += "|$($record.priority.Split('-')[0])|$($record.category)|$($record.plugin)|$($record.displayName)|$counts|$types|$state|"
}
$lines += @('', '## 每项验收顺序', '', '1. 确认二进制身份、注册信息、版本、资源和依赖，追踪托管到原生的调用入口。', '2. 记录参数默认值、范围、显隐、输入输出类型、数组、订阅和持久化语义。', '3. 跟踪真实执行路径、状态机、错误、取消及资源释放；未知项保持未知。', '4. 实现插件算法和专用配置界面；公共服务需求写入 ../CONTRACT-REQUESTS.md。', '5. 用原始程序与重建程序的同输入结果、模式界面和生命周期验证，记录差异。', '6. 专用测试通过后更新 HANDOFF.md；测试通过不能单独证明原生等价。')
$checklistPath=Join-Path $OutputDirectory 'CHECKLIST.md'
# CHECKLIST also carries reviewed priorities, framework work and acceptance state.
# Re-exporting source declarations must not erase that human-maintained progress.
if($ResetChecklist -or -not (Test-Path -LiteralPath $checklistPath)) {
    $lines | Set-Content -LiteralPath $checklistPath -Encoding utf8
} else {
    Write-Output 'Preserved reviewed CHECKLIST.md; only inventory.json was regenerated. Use -ResetChecklist for an explicit declaration-only reset.'
}
Write-Output "Validated $($records.Count) plugins; $native native DLLs; $managed managed DLLs."
