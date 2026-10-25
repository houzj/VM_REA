$ErrorActionPreference='Stop'
# PEReader reads file bytes; no vendor assembly is loaded or invoked.
$gvRoot=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$gvOut=Join-Path $gvRoot 'docs/architecture-recovery/global-variable-execution.resources'
New-Item -ItemType Directory -Force $gvOut | Out-Null
$gvOps=@{}
foreach($field in [Reflection.Emit.OpCodes].GetFields([Reflection.BindingFlags]'Public,Static')) {
 if($field.FieldType -eq [Reflection.Emit.OpCode]) { $op=$field.GetValue($null);$gvOps[([int]$op.Value -band 65535)]=$op }
}
function Get-GvType($md,$handle) {
 $handle=[Reflection.Metadata.EntityHandle]$handle
 switch($handle.Kind.ToString()) {
  'TypeReference' { $t=$md.GetTypeReference([Reflection.Metadata.Ecma335.MetadataTokens]::TypeReferenceHandle(([Reflection.Metadata.Ecma335.MetadataTokens]::GetToken($handle)) -band 16777215));return ($md.GetString($t.Namespace)+'.'+$md.GetString($t.Name)) }
  'TypeDefinition' { $t=$md.GetTypeDefinition([Reflection.Metadata.Ecma335.MetadataTokens]::TypeDefinitionHandle(([Reflection.Metadata.Ecma335.MetadataTokens]::GetToken($handle)) -band 16777215));return ($md.GetString($t.Namespace)+'.'+$md.GetString($t.Name)) }
  default {return $handle.Kind.ToString()}
 }
}
function Resolve-GvToken($md,[int]$token) {
 $rid=$token -band 16777215
 switch(($token -shr 24) -band 255) {
  6 {$m=$md.GetMethodDefinition([Reflection.Metadata.Ecma335.MetadataTokens]::MethodDefinitionHandle($rid));return @{type=(Get-GvType $md ($m.GetDeclaringType()));member=$md.GetString($m.Name)}}
  10 {$m=$md.GetMemberReference([Reflection.Metadata.Ecma335.MetadataTokens]::MemberReferenceHandle($rid));return @{type=(Get-GvType $md $m.Parent);member=$md.GetString($m.Name)}}
  4 {$m=$md.GetFieldDefinition([Reflection.Metadata.Ecma335.MetadataTokens]::FieldDefinitionHandle($rid));return @{type=(Get-GvType $md ($m.GetDeclaringType()));member=$md.GetString($m.Name)}}
  default {return @{kind=('table-'+(($token -shr 24) -band 255))}}
 }
}
$gvArtifacts=@(
 @{path='D:/Program Files/VisionMaster4.4.0/Applications/myLibs/VM.Core.dll';types=@{
 'VM.Core.VmSolution'=@('MModuleManager_OnModuleResultCallBack','SetValue','MProcessManager_OnWorkStatusCallBack');
 'VM.Core.VmIO'=@('GetValue','get_Value');
 'VM.Core.VmProcedure'=@('set_ContinuousRunEnable')}},
 @{path='D:/Program Files/VisionMaster4.4.0/Applications/PublicFile/x64/VM.PlatformSDKCS.dll';types=@{
 'VM.PlatformSDKCS.ProcessManager'=@('RefreshUIStopExecute','StopExecute','ContinousRunStart');
 'VM.PlatformSDKCS.GlobalModuleManager'=@('BindGlobalVarParam')}}
)
$gvResults=@()
foreach($artifact in $gvArtifacts) {
 $stream=[IO.File]::OpenRead($artifact.path);$pe=[Reflection.PortableExecutable.PEReader]::new($stream)
 try {
  $md=[Reflection.Metadata.PEReaderExtensions]::GetMetadataReader($pe);$methods=@()
  foreach($handle in $md.MethodDefinitions) {
   $m=$md.GetMethodDefinition($handle);$type=Get-GvType $md ($m.GetDeclaringType());$name=$md.GetString($m.Name)
   if(!$artifact.types.ContainsKey($type) -or $name -notin $artifact.types[$type]) {continue}
   if(!$m.RelativeVirtualAddress) {continue}
   $body=[Reflection.Metadata.PEReaderExtensions]::GetMethodBody($pe,$m.RelativeVirtualAddress);$il=$body.GetILBytes();$instructions=@();$i=0
   while($i -lt $il.Length) {
    $offset=$i;$code=[int]$il[$i++];if($code -eq 254){$code=65024+[int]$il[$i++]};$op=$gvOps[$code];if(!$op){throw "Unknown IL opcode $code"}
    $size=switch($op.OperandType.ToString()) {'InlineNone'{0};'ShortInlineBrTarget'{1};'ShortInlineI'{1};'ShortInlineVar'{1};'InlineVar'{2};'InlineI8'{8};'InlineR'{8};'InlineSwitch'{4+4*[BitConverter]::ToInt32($il,$i)};default{4}}
    $row=@{offset=$offset;opcode=$op.Name;operandBytes=[Convert]::ToHexString($il[$i..($i+$size-1)]).ToLowerInvariant()}
    if($size -eq 0){$row.operandBytes=''}
    if($op.OperandType.ToString() -in @('InlineMethod','InlineField','InlineType','InlineTok')) {
     $token=[BitConverter]::ToInt32($il,$i);$row.token=('0x{0:x8}' -f $token);$row.resolved=Resolve-GvToken $md $token
    }
    $instructions+=@($row);$i+=$size
   }
   $methods+=@(@{type=$type;member=$name;token=('0x{0:x8}' -f [Reflection.Metadata.Ecma335.MetadataTokens]::GetToken([Reflection.Metadata.EntityHandle]$handle));rva=$m.RelativeVirtualAddress;ilBytes=[Convert]::ToHexString($il).ToLowerInvariant();ilSha256=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($il)).ToLowerInvariant();instructions=$instructions})
  }
  $gvResults+=@(@{path=$artifact.path;sha256=(Get-FileHash -LiteralPath $artifact.path -Algorithm SHA256).Hash.ToLowerInvariant();mvid=$md.GetGuid($md.GetModuleDefinition().Mvid).ToString();methods=$methods})
 } finally {$pe.Dispose();$stream.Dispose()}
}
@{method='System.Reflection.Metadata.PEReader; file bytes only';vendorAssemblyLoaded=$false;runtimeExecuted=$false;artifacts=$gvResults} | ConvertTo-Json -Depth 15 | Set-Content -LiteralPath (Join-Path $gvOut 'managed-cil.json') -Encoding utf8
Write-Output ('Selected CIL bodies: '+(($gvResults | ForEach-Object {$_.methods.Count} | Measure-Object -Sum).Sum))
