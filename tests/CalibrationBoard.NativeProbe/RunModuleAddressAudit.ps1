param(
 [Parameter(Mandatory=$true)][string]$EvidenceRun,
 [Parameter(Mandatory=$true)][string[]]$Addresses,
 [string]$NativeTools='D:/MyWork/REA逆向测试/.tools/native-analysis'
)
$ErrorActionPreference='Stop'
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$projectRoot=[IO.Path]::GetFullPath((Join-Path $root $EvidenceRun))
$allowed=[IO.Path]::GetFullPath((Join-Path $root 'analysis/calibration-board'))+[IO.Path]::DirectorySeparatorChar
if(!$projectRoot.StartsWith($allowed,[StringComparison]::OrdinalIgnoreCase)){throw 'Use an owned calibration-board evidence project'}
if(!(Test-Path -LiteralPath "$projectRoot/project/board.gpr")){throw 'Owned project does not exist'}
if(Test-Path -LiteralPath "$projectRoot/project/board.lock"){throw 'Owned project is still in use; wait for its export to finish'}
foreach($address in $Addresses){if($address -notmatch '^180[0-9a-fA-F]{6}$'){throw 'Expected exact module virtual address'}}
$run='analysis/calibration-board/module-reuse-'+[guid]::NewGuid().ToString('N')
New-Item -ItemType Directory -Force "$root/$run/output","$root/$run/settings","$root/$run/cache","$root/$run/temp" | Out-Null
$alias=Join-Path $env:TEMP ('board-reuse-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $alias | Out-Null
New-Item -ItemType Junction -Path "$alias/work" -Target $root | Out-Null
New-Item -ItemType Junction -Path "$alias/tools" -Target $NativeTools | Out-Null
$oldJavaHome=$env:JAVA_HOME; $oldJavaOptions=$env:JAVA_TOOL_OPTIONS
try {
 $env:JAVA_HOME="$alias/tools/jdk-21.0.12.1+1"
 $env:JAVA_TOOL_OPTIONS="-Dapplication.settingsdir=$alias/work/$run/settings -Dapplication.cachedir=$alias/work/$run/cache -Dapplication.tempdir=$alias/work/$run/temp"
 $arguments=@("$alias/work/$EvidenceRun/project",'board','-process','IMVSCalibBoardCalibModu.dll','-readOnly','-noanalysis','-scriptPath',"$alias/work/tests/CalibrationBoard.NativeProbe",'-postScript','BoardModuleAddressAudit.java',"$alias/work/$run/output")+$Addresses
 & "$alias/tools/ghidra_12.1.4_PUBLIC/support/analyzeHeadless.bat" @arguments *> "$root/$run/headless.log"
 if($LASTEXITCODE -ne 0 -or !(Test-Path "$root/$run/output/functions.tsv")){throw 'No complete verified export; inspect owned log'}
 @{project=$EvidenceRun;addresses=$Addresses;output=$run;mode='owned readOnly noanalysis';exitCode=$LASTEXITCODE} | ConvertTo-Json | Set-Content "$root/$run/receipt.json"
 Write-Output $run
} finally {$env:JAVA_HOME=$oldJavaHome;$env:JAVA_TOOL_OPTIONS=$oldJavaOptions}
