$ErrorActionPreference='Stop'
$vfPath='D:/Program Files/VisionMaster4.4.0/Applications/myLibs/VMControls.WPF.dll'
$vfStream=[IO.File]::OpenRead($vfPath)
$vfPe=[System.Reflection.PortableExecutable.PEReader]::new($vfStream)
try {
 $vfMd=[System.Reflection.Metadata.PEReaderExtensions]::GetMetadataReader($vfPe)
 $vfRows=@();$vfIndex=0
 foreach($vfHandle in $vfMd.MethodDefinitions){
  $vfIndex++;$vfMethod=$vfMd.GetMethodDefinition($vfHandle);$vfName=$vfMd.GetString($vfMethod.Name)
  $vfType=$vfMd.GetTypeDefinition($vfMethod.GetDeclaringType());$vfTypeName=$vfMd.GetString($vfType.Name)
  if($vfName -like '*ImportGlobalVar*' -or $vfName -like '*ExportGlobalVar*' -or $vfTypeName -like '<ImportGlobalVar>*' -or $vfTypeName -like '<ExportGlobalVar>*'){
   $vfRows+=@{name=$vfName;type=($vfMd.GetString($vfType.Namespace)+'.'+$vfTypeName);token=('0x{0:x8}' -f (0x06000000+$vfIndex));rva=$vfMethod.RelativeVirtualAddress}
  }
 }
 # MethodDefinitions is metadata row enumeration; focused exporter revalidates the exact tokens/type.
 $vfRows | ConvertTo-Json -Depth 5 | Set-Content -Encoding utf8 docs/architecture-recovery/variable-file-entry-audit.generated-methods.json
} finally {$vfPe.Dispose();$vfStream.Dispose()}
