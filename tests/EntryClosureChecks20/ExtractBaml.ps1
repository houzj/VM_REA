param([string]$OutputDirectory='analysis/full-audit/batch20-20261020/A')
$ErrorActionPreference='Stop'
$root=[IO.Path]::GetFullPath($OutputDirectory)
[IO.Directory]::CreateDirectory($root)|Out-Null
$path='D:/Program Files/VisionMaster4.4.0/Applications/myLibs/VMControls.WPF.dll'
$before=(Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash
$assembly=[Reflection.Assembly]::ReflectionOnlyLoadFrom($path)
$stream=$assembly.GetManifestResourceStream('VMControls.WPF.g.resources')
$reader=New-Object System.Resources.ResourceReader($stream)
try {
 $keys=New-Object System.Collections.Generic.List[string]
 $iterator=$reader.GetEnumerator()
 while($iterator.MoveNext()){$keys.Add([string]$iterator.Key)}
 $keys|Sort-Object|Set-Content -Encoding UTF8 (Join-Path $root 'resource-keys.txt')
 foreach($key in @('base/diagramitem.baml','base/connection/connection.baml','base/diagrampage.baml','base/rectdragpanal.baml')){
  if(-not$keys.Contains($key)){[pscustomobject]@{Key=$key;Status='missing-in-current-resource-table'}|ConvertTo-Json -Compress|Add-Content -Encoding UTF8 (Join-Path $root 'missing-resources.jsonl');continue}
  [string]$type=$null;[byte[]]$data=$null
  $reader.GetResourceData($key,[ref]$type,[ref]$data)
  $file=Join-Path $root (($key -replace '/','_')+'.resource-data.bin')
  [IO.File]::WriteAllBytes($file,$data)
  [pscustomobject]@{Key=$key;ResourceType=$type;Path=$file;Length=$data.Length;SHA256=(Get-FileHash -LiteralPath $file -Algorithm SHA256).Hash}|ConvertTo-Json -Compress|Add-Content -Encoding UTF8 (Join-Path $root 'baml-resources.jsonl')
 }
} finally {$reader.Dispose();$stream.Dispose()}
$after=(Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash
if($before-ne$after){throw 'Original assembly changed.'}
[pscustomobject]@{Path=$path;BeforeSHA256=$before;AfterSHA256=$after;MVID=$assembly.ManifestModule.ModuleVersionId;Authority='ReflectionOnly resource bytes; no original code or XAML/schema execution'}|ConvertTo-Json|Set-Content -Encoding UTF8 (Join-Path $root 'carrier-identity.json')
