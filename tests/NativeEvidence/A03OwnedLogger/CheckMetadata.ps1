param([string]$LibraryRoot='D:/Program Files/VisionMaster4.4.0/Applications/myLibs',[string]$Output='analysis/full-audit/batch7-20261019/A03/module-initializer-check.json')
$ErrorActionPreference='Stop'
$rows=@()
foreach($n in 'Apps.Log.dll','Apps.BaseLog.dll') {
 $p=Join-Path $LibraryRoot $n
 $s=[IO.File]::OpenRead($p); $pe=[System.Reflection.PortableExecutable.PEReader]::new($s)
 try {
  $md=[System.Reflection.Metadata.PEReaderExtensions]::GetMetadataReader($pe)
  $found=$false
  foreach($h in $md.TypeDefinitions) {
   $t=$md.GetTypeDefinition($h)
   if($md.GetString($t.Name) -ceq '<Module>') {
    $found=$true; $names=@($t.GetMethods() | ForEach-Object {$md.GetString($md.GetMethodDefinition($_).Name)})
    if($names.Count -ne 0){throw 'Global method table non-empty; preload not approved'}
    $rows+=@{path=$p;sha256=(Get-FileHash -LiteralPath $p).Hash;moduleType='<Module>';methodNames=$names;methodCount=0;moduleInitializerPresent=$false;authority='raw PE metadata via System.Reflection.Metadata; no assembly execution'}
   }
  }
  if(!$found){throw 'Module TypeDef missing'}
 } finally {$pe.Dispose();$s.Dispose()}
}
$rows | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $Output -Encoding utf8
