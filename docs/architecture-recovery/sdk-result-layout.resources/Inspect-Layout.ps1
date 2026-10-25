# Windows PowerShell .NET Framework reflection-only metadata; no target method invocation.
$ErrorActionPreference='Stop'
$layoutInput='D:/Program Files/VisionMaster4.4.0/Applications/PublicFile/x64/VM.PlatformSDKCS.dll'
$layoutAssembly=[Reflection.Assembly]::ReflectionOnlyLoadFrom($layoutInput)
$layoutTypes=@('IMVS_OUTPUT_PLATFORM_INFO','IMVS_MODULE_CHUNK_VALUE_EX','IMVS_MODULE_BYTEDATA_VALUE_EX','IMVS_MODULE_RESULT_INFO_EX','IMVS_MODULE_RESULT_RESERVED_INFO','IMVS_MODULE_RESULT_INFO_LIST_EX')
$layoutRows=@()
foreach($layoutName in $layoutTypes){
 $layoutType=$layoutAssembly.GetType('VM.PlatformSDKCS.ImvsSdkDefine+'+$layoutName,$true)
 $layoutAttr=$layoutType.StructLayoutAttribute
 $layoutSize=$null;$layoutError=$null
 try{$layoutSize=[Runtime.InteropServices.Marshal].GetMethod('SizeOf',[type[]]@([Type])).Invoke($null,@($layoutType))}catch{$layoutError=$_.Exception.Message}
 $layoutFields=@()
 foreach($layoutField in $layoutType.GetFields([Reflection.BindingFlags]'Public,NonPublic,Instance')){
  $layoutOffset=$null;$layoutOffsetError=$null
  try{$layoutOffset=[Runtime.InteropServices.Marshal]::OffsetOf($layoutType,$layoutField.Name).ToInt64()}catch{$layoutOffsetError=$_.Exception.Message}
  $layoutFields+=@{name=$layoutField.Name;type=$layoutField.FieldType.FullName;offset=$layoutOffset;offsetError=$layoutOffsetError;metadataToken=('0x{0:x8}' -f $layoutField.MetadataToken)}
 }
 $layoutRows+=@{type=$layoutType.FullName;metadataToken=('0x{0:x8}' -f $layoutType.MetadataToken);layoutKind=[string]$layoutAttr.Value;pack=$layoutAttr.Pack;charSet=[string]$layoutAttr.CharSet;declaredSize=$layoutAttr.Size;marshalSize=$layoutSize;marshalError=$layoutError;fields=$layoutFields}
}
@{artifact=$layoutInput;sha256=(Get-FileHash -LiteralPath $layoutInput -Algorithm SHA256).Hash.ToLowerInvariant();mvid=[string]$layoutAssembly.ManifestModule.ModuleVersionId;processPointerSize=[IntPtr]::Size;method='ReflectionOnlyLoadFrom; no target code execution; Marshal layout queries attempted on reflection-only types';types=$layoutRows}|ConvertTo-Json -Depth 8
