$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$tabPath=Join-Path $root 'analysis/camera-mapping/parser/model/IMVSCameraMapModuAlgorithmTab.xml'
$modulePath='D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSCameraMapModu/IMVSCameraMapModu.xml'
[xml]$tab=Get-Content -LiteralPath $tabPath -Raw
[xml]$module=Get-Content -LiteralPath $modulePath -Raw
$groups=@(foreach($pair in $tab.SelectNodes('//PointPair')){
 $children=@(foreach($name in @('ObjectPoint','TargetPoint','ObjectPointX','TargetPointX','ObjectPointY','TargetPointY')){
  $node=$pair.SelectSingleNode($name)
  [ordered]@{member=$name;sourceOperations=@($node.SelectNodes('Triggers/Trigger/Setters/Setter')|ForEach-Object{[ordered]@{operation=[string]$_.OperationName;parameter=[string]$_.OperationParams;target=[string]$_.TargetName}});initOperations=@($node.SelectNodes('Initers/Setter')|ForEach-Object{[ordered]@{operation=[string]$_.OperationName;parameter=[string]$_.OperationParams;target=[string]$_.TargetName}})}
 })
 [ordered]@{name=$pair.GetAttribute('Name');fixed=[string]$pair.Fixed;canDelete=[string]$pair.CanDelete;customVisible=[string]$pair.CustomVisible;displayIndex=[string]$pair.DisplayIndex;children=$children}
})
$mode=@(foreach($trigger in $tab.SelectNodes('//RadioSelecter[@Name="InputWay"]/Triggers/Trigger')){[ordered]@{property=[string]$trigger.Property;value=[string]$trigger.Value;setters=@($trigger.Setters.Setter|ForEach-Object{[ordered]@{target=[string]$_.TargetName;property=[string]$_.PropertyName;value=[string]$_.Value}})}})
$inputs=@($module.SelectNodes('//Category[@Name="Input"]/Items/Combination')|ForEach-Object{[ordered]@{name=$_.GetAttribute('Name');style=$_.GetAttribute('Style');fields=@($_.Filters.Filter|ForEach-Object{[ordered]@{name=$_.GetAttribute('Name');type=$_.GetAttribute('ValueType');force=$_.GetAttribute('IsForce')}})}})
$result=[ordered]@{provider='System.Xml passive resource read';sources=@($tabPath,$modulePath)|ForEach-Object{[ordered]@{path=$_;sha256=(Get-FileHash -LiteralPath $_).Hash}};groups=$groups;maxGroups=[string]$tab.SelectSingleNode('//AddButton_PointPair/MaxCount').InnerText;mode=$mode;inputs=$inputs}
$result|ConvertTo-Json -Depth 15|Set-Content "$root/analysis/camera-mapping/b3-point-index-source-20261019/resource-map.json" -Encoding utf8
