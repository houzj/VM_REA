param([string]$SdkRoot='D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK')
$ErrorActionPreference='Stop'
$repoRoot=Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$run=Join-Path $repoRoot 'analysis/source-audit-regression/20261018/native-binary-modes'
$build=Join-Path $run 'runner'
$output=Join-Path $run 'outputs'
New-Item -ItemType Directory -Force -Path $build,$output | Out-Null
$names=@('MVDImage.Net.dll','MVDShape.Net.dll','MVDCore.Net.dll','MVDRegion.Net.dll')
$refs=@()
foreach($name in $names){$source=Join-Path $SdkRoot ('ReferencedAssemblies/Common/'+$name);Copy-Item -LiteralPath $source -Destination $build -Force;$refs+=('/r:'+ (Join-Path $build $name))}
$algorithm=Join-Path $SdkRoot 'ReferencedAssemblies/Algorithms/MVDImageBinary.Net.dll'
Copy-Item -LiteralPath $algorithm -Destination $build -Force
$refs+=('/r:'+ (Join-Path $build 'MVDImageBinary.Net.dll'))
$compiler=Join-Path $env:WINDIR 'Microsoft.NET/Framework64/v4.0.30319/csc.exe'
& $compiler /nologo /r:System.Drawing.dll ('/out:'+ (Join-Path $build 'ModesProbe.exe')) @refs (Join-Path $PSScriptRoot 'ModesProbe.cs')
if($LASTEXITCODE -ne 0){throw '原厂图像二值化模式探针编译失败。'}
$stdout=Join-Path $run 'stdout.log';$stderr=Join-Path $run 'stderr.log'
Push-Location -LiteralPath $build
try { & (Join-Path $build 'ModesProbe.exe') $SdkRoot $output *> $stdout;if($LASTEXITCODE -ne 0){throw "原厂图像二值化模式探针失败，查看 $output/failure.txt"} } finally {Pop-Location}
$inputs=@(Get-ChildItem -LiteralPath $build -Filter '*.dll')+@(Get-Item $algorithm)
$record=@{authority='installed MVD SDK controlled replay';nativeModuleDesktop=$false;noDeviceTest=$true;sdkRoot=$SdkRoot;runnerSha256=(Get-FileHash (Join-Path $build 'ModesProbe.exe') -Algorithm SHA256).Hash;inputSha256=(Get-FileHash (Join-Path $output 'input.bmp') -Algorithm SHA256).Hash;outputs=@(Get-ChildItem -LiteralPath $output -Filter '*.bmp' | ForEach-Object {@{name=$_.Name;sha256=(Get-FileHash $_.FullName -Algorithm SHA256).Hash}});sdkAssemblies=@($inputs | ForEach-Object {@{path=$_.FullName;sha256=(Get-FileHash $_.FullName -Algorithm SHA256).Hash}})}
$record|ConvertTo-Json -Depth 8|Set-Content -LiteralPath (Join-Path $run 'evidence.json')
Get-Content -LiteralPath (Join-Path $output 'modes.txt')
Write-Output "原厂SDK模式证据：$run"
