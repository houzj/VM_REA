param([string]$OutputDirectory='analysis/full-audit/batch7-20261019/A03',[switch]$Execute)
$ErrorActionPreference='Stop'
$root=[IO.Path]::GetFullPath($OutputDirectory)
$run=Join-Path $root ('run-'+[Guid]::NewGuid().ToString('N'))
[IO.Directory]::CreateDirectory((Join-Path $run 'myLibs')) | Out-Null
$lib='D:\Program Files\VisionMaster4.4.0\Applications\myLibs'
$cfg='D:\Program Files\VisionMaster4.4.0\Development\V4.x\ComControls\Assembly\myLibs\Apps.BaseLog.config'
$inputFiles=@((Join-Path $lib 'Apps.Log.dll'),(Join-Path $lib 'Apps.BaseLog.dll'),$cfg)
$before=@($inputFiles | ForEach-Object { Get-FileHash -LiteralPath $_ -Algorithm SHA256 } | Select-Object Path,Hash)
Copy-Item -LiteralPath $inputFiles[0],$inputFiles[1] -Destination $run
Copy-Item -LiteralPath $cfg -Destination (Join-Path $run 'myLibs/Apps.BaseLog.config')
[xml]$xml=Get-Content -LiteralPath $cfg -Raw
$paths=@()
foreach($a in $xml.configuration.'Apps.BaseLog'.appender) {
 if($a.type -cne 'Apps.BaseLog.Appender.RollingFileAppender'){throw 'Unsupported appender'}
 foreach($node in $a.SelectNodes('.//*[@type]')) { if($node.type -notin @('Apps.BaseLog.Layout.PatternLayout','Apps.BaseLog.Appender.FileAppender+MinimalLock')){throw 'Unsupported configuration type'} }
 $v=[string]($a.param | Where-Object name -eq File).value
 if([IO.Path]::IsPathRooted($v) -or $v.Contains('%')){throw 'Absolute or expandable output rejected'}
 $full=[IO.Path]::GetFullPath((Join-Path $run $v))
 if(!$full.StartsWith($run+[IO.Path]::DirectorySeparatorChar,[StringComparison]::OrdinalIgnoreCase)){throw 'Output escapes run'}
 $paths += @{appender=$a.name;original=$v;resolved=$full}
}
if($paths.Count -ne 6){throw 'Unexpected appender count'}
$paths | ConvertTo-Json | Set-Content (Join-Path $run 'output-path-preflight.json') -Encoding utf8
$csc=Join-Path $env:WINDIR 'Microsoft.NET/Framework64/v4.0.30319/csc.exe'
foreach($name in @('Probe','LoggerRun')) {
 & $csc /nologo /target:exe ("/out:"+(Join-Path $run ($name+'.exe'))) (Join-Path $PSScriptRoot ($name+'.cs')) *> (Join-Path $run ($name+'-compile.log'))
 if($LASTEXITCODE -ne 0){throw 'Compile failed'}
}
$key='SOFTWARE\WOW6432Node\Microsoft\.NETFramework\v4.0.30319\AssemblyFoldersEx\VisionMaster'
function ReadHostValue { $k=[Microsoft.Win32.Registry]::LocalMachine.OpenSubKey($key); try {if($k){[string]$k.GetValue('')}} finally {if($k){$k.Dispose()}} }
$hostBefore=ReadHostValue
$code=$null
if($Execute) { & (Join-Path $run 'Probe.exe') $run *> (Join-Path $run 'stdout-stderr.log'); $code=$LASTEXITCODE }
$after=@($inputFiles | ForEach-Object { Get-FileHash -LiteralPath $_ -Algorithm SHA256 } | Select-Object Path,Hash)
@{before=$before;after=$after;unchanged=(($before|ConvertTo-Json -Compress) -ceq ($after|ConvertTo-Json -Compress));hostBefore=$hostBefore;hostAfter=(ReadHostValue);executed=[bool]$Execute;exit=$code} | ConvertTo-Json -Depth 6 | Set-Content (Join-Path $run 'integrity.json') -Encoding utf8
Get-ChildItem $run -File -Recurse | Get-FileHash -Algorithm SHA256 | Select-Object Path,Hash | ConvertTo-Json | Set-Content (Join-Path $run 'hashes.json') -Encoding utf8
$run
if($Execute){exit $code}

