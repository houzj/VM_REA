param([string]$Installation='D:\Program Files\VisionMaster4.4.0\Applications')
$ErrorActionPreference='Stop'
$projectRoot=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$moduleRoot=Join-Path $Installation 'Module(sp)/x64'
$assetRoot=Join-Path $projectRoot 'analysis/ui-assets/catalog'
[void][IO.Directory]::CreateDirectory($assetRoot)
$manifest=[Collections.Generic.List[object]]::new()
function RecordSource([string]$path) { $manifest.Add([ordered]@{path=$path;sha256=(Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash}) }
$languagePath=Join-Path $Installation 'Lang/zh-cn.xaml'
$language=[xml][IO.File]::ReadAllText($languagePath)
RecordSource $languagePath
$names=@{}
foreach($entry in $language.DocumentElement.ChildNodes){if($entry.NodeType -eq 'Element'){$key=$entry.GetAttribute('Key','http://schemas.microsoft.com/winfx/2006/xaml');if($key){$names[$key]=$entry.InnerText}}}
function LocalName([string]$key){if($names.ContainsKey($key)){return $names[$key]};return $key}
function ExportIcon([string]$directory,[string]$stem,[string]$state){$path=Join-Path $directory ($stem+'Image'+$state+'.png');if(-not(Test-Path -LiteralPath $path)){return $null};$file=$stem+'Image'+$state+'.png';Copy-Item -LiteralPath $path -Destination (Join-Path $assetRoot $file);RecordSource $path;return 'catalog/'+$file}
$groups=@(foreach($directory in Get-ChildItem -LiteralPath $moduleRoot -Directory){
    $groupPath=Join-Path $directory.FullName 'ToolGroup.xml'
    if(-not(Test-Path -LiteralPath $groupPath)){continue}
    RecordSource $groupPath
    $group=[xml][IO.File]::ReadAllText($groupPath)
    $items=@(foreach($toolDir in Get-ChildItem -LiteralPath $directory.FullName -Directory){
        $itemPath=Join-Path $toolDir.FullName 'ToolItemInfo.xml';if(-not(Test-Path -LiteralPath $itemPath)){continue}
        RecordSource $itemPath;$item=[xml][IO.File]::ReadAllText($itemPath)
        [ordered]@{Id=$toolDir.Name;Name=(LocalName ([string]$item.ToolBoxItemData.name));Priority=[double]$item.ToolBoxItemData.priority;DisplayType=[string]$item.ToolBoxItemData.displayType;Icon=(ExportIcon $toolDir.FullName $toolDir.Name 'Enable');ModuleId=$(if($toolDir.Name -eq 'ImageSourceModule'){'vision.image-source'}else{$null})}
    })
    [ordered]@{Id=$directory.Name;Name=(LocalName ([string]$group.ToolBoxItemData.name));Priority=[double]$group.ToolBoxItemData.priority;Icon=(ExportIcon $directory.FullName $directory.Name 'Enable');HoverIcon=(ExportIcon $directory.FullName $directory.Name 'Hover');Items=@($items|Sort-Object { $_.Priority })}
})
$catalog=[ordered]@{Schema=1;Groups=@($groups|Sort-Object {$_.Priority})}
[IO.File]::WriteAllText((Join-Path $projectRoot 'src/VisionEditor/tool-catalog.json'),($catalog|ConvertTo-Json -Depth 12),[Text.UTF8Encoding]::new($false))
$report=[ordered]@{evidenceId='ev_2505534319eaae13cf1f3b937f4d6eba568940fae9b0ba5799aa36bce4ff070e';assemblySha256='a0321eb4669c5114c791b32af8d0ce5ac17a690210bc87319908658f7457a9f8';implementation='ToolBoxData.LoadData/UpdateLoad/GetItems/SortToolBoxItemCompare';configuration='ToolBoxType=General';runtimeObserved=$false;groups=$groups.Count;items=($groups|ForEach-Object {$_.Items.Count}|Measure-Object -Sum).Sum;files=$manifest}
[IO.File]::WriteAllText((Join-Path $projectRoot 'analysis/full-audit/toolbox-source-manifest.json'),($report|ConvertTo-Json -Depth 12),[Text.UTF8Encoding]::new($false))
Write-Output "Exported $($report.groups) groups / $($report.items) tools; independent runtime enabled for ImageSourceModule only."
