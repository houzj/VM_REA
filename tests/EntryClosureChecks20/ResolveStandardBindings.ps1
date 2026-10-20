param([string]$Records='analysis/full-audit/batch20-20261020/A/diagramitem-records.json',[string]$OutputDirectory='analysis/full-audit/batch20-20261020/A/standard-wpf')
$ErrorActionPreference='Stop'
[AppContext]::SetSwitch('VisionEditor.DisableAutomaticCameraDiscovery',$true)
Add-Type -AssemblyName PresentationCore
Add-Type -AssemblyName PresentationFramework
$root=[IO.Path]::GetFullPath($OutputDirectory);[IO.Directory]::CreateDirectory($root)|Out-Null
$data=Get-Content -LiteralPath $Records -Raw|ConvertFrom-Json
$gestureConverter=New-Object System.Windows.Input.KeyGestureConverter
$modifierConverter=New-Object System.Windows.Input.ModifierKeysConverter
$rows=New-Object System.Collections.Generic.List[object]
foreach($entry in $data.bindings){
 $binding=New-Object System.Windows.Input.KeyBinding
 $assignments=New-Object System.Collections.Generic.List[object]
 foreach($property in $entry.properties){
  switch($property.name){
   'Gesture' {$binding.Gesture=$gestureConverter.ConvertFromInvariantString($property.value)}
   'Key' {$binding.Key=[Enum]::Parse([System.Windows.Input.Key],$property.value)}
   'Modifiers' {$binding.Modifiers=$modifierConverter.ConvertFromInvariantString($property.value)}
   'CommandParameter' {$binding.CommandParameter=$property.value}
   default {throw "Unexpected property in selected KeyBinding: $($property.name)"}
  }
  $assignments.Add([pscustomobject]@{Offset=$property.offset;Property=$property.name;Value=$property.value;AfterKey=$binding.Key.ToString();AfterModifiers=$binding.Modifiers.ToString()})
 }
 if($binding.Modifiers-ne[System.Windows.Input.ModifierKeys]::Control){throw 'Unexpected final modifier'}
 $rows.Add([pscustomobject]@{ElementOffset=$entry.offset;TypeId=$entry.typeId;Parameter=$binding.CommandParameter;Key=$binding.Key.ToString();Modifiers=$binding.Modifiers.ToString();RawNestedText=$entry.nestedText;Assignments=$assignments})
}
if($rows.Count-ne11){throw 'Expected exactly eleven original DiagramItem bindings'}
$known=[System.Windows.Window].Assembly.GetType('System.Windows.Markup.KnownElements')
$knownId=if($known){[int][Enum]::Parse($known,'KeyBinding')}else{$null}
if($knownId-ne337){throw 'KnownElements KeyBinding id is not the selected raw BAML type id'}
$framework=[System.Windows.Input.KeyBinding].Assembly.Location
[pscustomobject]@{Passed=$rows.Count;SourceRecordsSHA256=(Get-FileHash -LiteralPath $Records -Algorithm SHA256).Hash;FrameworkPath=$framework;FrameworkSHA256=(Get-FileHash -LiteralPath $framework -Algorithm SHA256).Hash;FrameworkAssembly=[System.Windows.Input.KeyBinding].Assembly.FullName;CLR=[Runtime.InteropServices.RuntimeEnvironment]::GetSystemVersion();KnownKeyBindingTypeId=$knownId;Authority='Original BAML record order replayed only on standard Microsoft WPF KeyBinding; no original control, schema resolution or SDK';Bindings=$rows}|ConvertTo-Json -Depth 12|Set-Content -Encoding UTF8 (Join-Path $root 'resolved-bindings.json')
Write-Output "PASS $($rows.Count) standard WPF key assignments; no vendor objects instantiated"
