$ErrorActionPreference='Stop'
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$out=Join-Path $root 'analysis/calibration-board/a4-source'
New-Item -ItemType Directory -Force $out | Out-Null
$module='D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSCalibBoardCalibModu'
$common=Get-Content (Join-Path $root 'analysis/distortion-calibration/c4-source/identity-and-lifecycle.json') -Raw | ConvertFrom-Json
$originals=@(Get-ChildItem -LiteralPath $module -File -Recurse | ForEach-Object {$_.FullName})+@($common.originalFiles | Select-Object -Last 6 -ExpandProperty path)+@('D:/Program Files/VisionMaster4.4.0/Applications/myLibs/Apps.XmlUI.dll','D:/Program Files/VisionMaster4.4.0/Applications/myLibs/VMBaseControls.dll','D:/Program Files/VisionMaster4.4.0/Applications/myLibs/Apps.Style.dll')
function Identity([string]$path){
 if(!(Test-Path -LiteralPath $path)){throw "Missing inventory input $path"}
 @{path=$path;bytes=(Get-Item -LiteralPath $path).Length;sha256=(Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash}
}
$products=@('src/VisionPlugins.CalibrationBoard/CalibrationBoardConfiguration.cs','src/VisionPlugins.CalibrationBoard/CalibrationBoardDisplay.cs','src/VisionPlugins.CalibrationBoard/CalibrationBoardPlugin.cs','src/VisionPluginContracts/ToolConfigurationProvider.cs','src/VisionPluginContracts/ToolConfigurationRoiService.cs','src/VisionPluginContracts/ToolConfigurationWindowHost.cs','src/VisionPluginContracts/ToolDisplayPresenter.cs')
$index=@{
 scope='A4 S0 identity/reuse/product-location inventory; no S1-S7 completion claim'
 originalFiles=@($originals | ForEach-Object {Identity $_})
 reusedSharedDecompilation=$common.reusedDecompilation
 managedEvidence=@('ev_80ccadddacca09c728b4f900dafb275b2664d00c7002ca2a8be96dd1d417dd1e','ev_7eccd132b38875c6a84e8c7578581fa97cdf759ee36a41206cb5f045cc250166')
 currentManagedIndex=Identity (Join-Path $out 'current-managed-index.json')
 productFiles=@($products | ForEach-Object {Identity (Join-Path $root $_)})
 trackedResources=@(Get-ChildItem (Join-Path $root 'src/VisionPlugins.CalibrationBoard/Resources') -File | ForEach-Object {Identity $_.FullName})
 missing=@('Current installed CalibBoardCalibParam/Result decompiled member bodies: REA identity/CIL anchors are available, not full recovered source','Complete configuration init/trigger/binding and serialization consumers for this board resource','ROI subscription field ABI, nonzero mask mode and extension provider lifetime','Display object feature/style persistence to renderer; resource presence is not runtime parity','Source-to-product matrix and actual host/manual acceptance remain separate')
 runtimeExecuted=$false
}
$index | ConvertTo-Json -Depth 12 | Set-Content (Join-Path $out 'identity-index.json')
Write-Output "Originals=$($index.originalFiles.Count); products=$($index.productFiles.Count); resources=$($index.trackedResources.Count)"
