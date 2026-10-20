param([string]$StartAt='SolverChecks')
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$output=Join-Path $root 'analysis/map-calibration/regression'
New-Item -ItemType Directory -Force $output | Out-Null
$names=@('SolverChecks','FileChecks','SessionChecks','ImageFitChecks','ImageFileChecks','PluginChecks','ConfigurationChecks','RoiRuntimeChecks')
$start=[Array]::IndexOf($names,$StartAt);if($start -lt 0){throw 'Unknown starting check'}
$results=@();if($start -gt 0){$results=@(Get-Content "$output/results.json" -Raw | ConvertFrom-Json | Where-Object {$names[0..($start-1)] -contains ($_.name -replace '^MapCalibration\.','')})}
foreach($name in $names[$start..($names.Count-1)]){
 $project=Join-Path $root "tests/MapCalibration.$name/MapCalibration.$name.csproj"
 & dotnet build $project -v:q *> "$output/$name-build.log"
 if($LASTEXITCODE -ne 0){Get-Content "$output/$name-build.log";throw "Map build failed: $name"}
 $assembly=Join-Path $root "tests/MapCalibration.$name/bin/Debug/net10.0-windows/MapCalibration.$name.dll"
 & dotnet $assembly *> "$output/$name-run.log"
 $code=$LASTEXITCODE
 $results += [pscustomobject]@{name="MapCalibration.$name";exitCode=$code;log="analysis/map-calibration/regression/$name-run.log"}
 $results | ConvertTo-Json | Set-Content "$output/results.json" -Encoding utf8
 Get-Content "$output/$name-run.log"
 if($code -ne 0){throw "Map check failed: $name"}
}
$python=Join-Path $env:USERPROFILE '.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe'
foreach($script in @('CompareCapturedInput.py','CheckImagePostprocess.py','CompareRoiMask.py','CompareProcessImageXml.py','ExportEvidence.py')){
 & $python (Join-Path $PSScriptRoot $script) *> "$output/$script.log"
 if($LASTEXITCODE -ne 0){Get-Content "$output/$script.log";throw "Map evidence check failed: $script"}
}
Write-Output 'Map managed regression finished; shared detector boundaries remain recorded separately; fixed fit matrix evidence is strict.'

