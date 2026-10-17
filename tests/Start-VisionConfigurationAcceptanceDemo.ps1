param([switch]$Show,[switch]$English,[switch]$Manual)
$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
Push-Location $root
try {
 dotnet build tests/VisionConfiguration.AcceptanceDemo --no-restore -v:q
 if($LASTEXITCODE -ne 0){throw 'Framework acceptance demo build failed'}
 $out=Join-Path $root ('analysis/full-audit/configuration-acceptance-demo/run-'+[Guid]::NewGuid().ToString('N'))
 New-Item -ItemType Directory -Force $out|Out-Null
 $arguments=@($(if($Show){'--demo'}else{'--check'}))
 if($English){$arguments+='--english'}
 if($Manual){$arguments+='--manual'}
 $p=Start-Process -FilePath (Join-Path $root 'tests/VisionConfiguration.AcceptanceDemo/bin/Debug/net10.0-windows/VisionConfiguration.AcceptanceDemo.exe') -ArgumentList $arguments -WindowStyle Hidden -WorkingDirectory $root -RedirectStandardOutput (Join-Path $out 'stdout.txt') -RedirectStandardError (Join-Path $out 'stderr.txt') -PassThru
 if($Show){Write-Output ('Started identified framework acceptance demo. Own PID='+$p.Id+'; log='+$out);return}
 if(!$p.WaitForExit(30000)){$p.Kill();throw 'Own framework acceptance demo check timed out'}
 Get-Content (Join-Path $out 'stdout.txt') -Encoding utf8
 if($p.ExitCode -ne 0){throw (Get-Content (Join-Path $out 'stderr.txt') -Raw -Encoding utf8)}
 Write-Output ('ExitCode='+$p.ExitCode+'; log='+$out)
}finally{Pop-Location}
