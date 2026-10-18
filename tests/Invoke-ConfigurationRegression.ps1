#Requires -Version 7.0
param([string]$RunDirectory='',[int]$WpfTimeoutSeconds=600)
$ErrorActionPreference='Stop'
$workspace=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
if(-not $RunDirectory){$RunDirectory=Join-Path $workspace ('analysis/configuration-regression/'+[guid]::NewGuid().ToString('N'))}
$RunDirectory=[IO.Path]::GetFullPath($RunDirectory)
$artifacts=Join-Path $RunDirectory 'build'
New-Item -ItemType Directory -Force -Path $RunDirectory | Out-Null
Push-Location $workspace
try {
 & dotnet build VisionReplica.slnx "-p:ArtifactsPath=$artifacts" --nologo -v:q *> (Join-Path $RunDirectory 'build.log')
 if($LASTEXITCODE -ne 0){throw "Configuration regression build failed: $RunDirectory/build.log"}
 $names=@('VisionConfiguration.ModelChecks','VisionConfiguration.StateChecks','VisionConfiguration.TemplateChecks','VisionConfiguration.ViewChecks','VisionConfiguration.ControlChecks','VisionConfiguration.HostChecks','VisionConfiguration.ExecutionChecks','VisionConfiguration.OperationChecks','VisionConfiguration.SubscriptionChecks','VisionConfiguration.WorkflowSubscriptionChecks','VisionConfiguration.InteractionChecks','VisionConfiguration.IntegerMetadataChecks','ImageSource.ConfigurationChecks','ReadCalibFile.ConfigurationChecks','ImageSource.PluginChecks','ReadCalibFile.PluginChecks','VisionFramework.ExecutionChecks','VisionFramework.PersistenceChecks','VisionFramework.IntegrationChecks','VisionEditor.Core.Checks','VisionPluginDefinitions.Checks','VisionEditor.WpfChecks','VisionConfiguration.ObjectChecks','VisionConfiguration.DisplaySubscriptionChecks','NPoint.ConfigurationControlChecks','NPoint.ConfigurationChecks','NPointSolver.Checks','NPointCalibration.PluginChecks','VisionConfiguration.GraphicsChecks','VisionConfiguration.PointAliasChecks','Calibration.HostChecks')
 $records=@()
 foreach($name in $names){
  $exe=Join-Path $artifacts ('bin/'+$name+'/debug/'+$name+'.exe')
  if(-not (Test-Path -LiteralPath $exe)){throw "Missing regression executable: $exe"}
  $stdout=Join-Path $RunDirectory ($name+'.stdout.log');$stderr=Join-Path $RunDirectory ($name+'.stderr.log')
  $watch=[Diagnostics.Stopwatch]::StartNew()
  $process=Start-Process -FilePath $exe -WorkingDirectory $workspace -WindowStyle Hidden -PassThru -RedirectStandardOutput $stdout -RedirectStandardError $stderr
  $timeout=if($name -eq 'VisionEditor.WpfChecks'){$WpfTimeoutSeconds}else{180};while(-not $process.WaitForExit(1000)){if($watch.Elapsed.TotalSeconds -gt $timeout){$process.Kill();throw "Own regression process timed out: $name"}}
  $files=@(Get-ChildItem -LiteralPath ([IO.Path]::GetDirectoryName($exe)) -Recurse -File | Where-Object { $_.Extension -in @('.exe','.dll','.json') } | Sort-Object FullName | ForEach-Object { @{path=[IO.Path]::GetRelativePath($artifacts,$_.FullName);sha256=(Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash} })
  $records+=@{name=$name;exitCode=$process.ExitCode;binarySha256=(Get-FileHash -LiteralPath $exe -Algorithm SHA256).Hash;executedAssembliesAndDependencies=$files;stdout=$stdout;stderr=$stderr;elapsedSeconds=$watch.Elapsed.TotalSeconds}
  @{scope='independent implementation and captured original-fixture replay; no device tests or original desktop parity claim';records=$records;nativeDesktopCompared=$false;manualAccepted=$false} | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath (Join-Path $RunDirectory 'summary.json')
  Write-Output ($name+': '+$process.ExitCode)
  if($process.ExitCode -ne 0){Get-Content -LiteralPath $stderr;throw "Regression failed: $name"}
 }
 Write-Output ('REGRESSION_LOG='+$RunDirectory)
} finally {Pop-Location}
