# Controlled sample derived from an installed documentation image, never a device capture.
param([string]$InstallRoot='D:/Program Files/VisionMaster4.4.0')
$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Drawing
$root=Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$audit=Join-Path $root 'analysis/full-audit/n-image-calibration'
$source="$InstallRoot/Applications/Help/CH/GUID-3D2B2BA5-C470-43B0-8EFD-D2D7D34DB3F0-high.png"
$img=[Drawing.Bitmap]::new($source)
$records=@()
try{for($i=0;$i -lt 9;$i++){
 $b=[Drawing.Bitmap]::new(1000,1000);$g=[Drawing.Graphics]::FromImage($b)
 try{$g.Clear([Drawing.Color]::White);$r=[Drawing.Rectangle]::new(50+($i%3)*10,50+[math]::Floor($i/3)*10,882,885);$g.InterpolationMode=[Drawing.Drawing2D.InterpolationMode]::NearestNeighbor;$g.DrawImage($img,$r,740,0,294,295,[Drawing.GraphicsUnit]::Pixel);$path=Join-Path $audit "board-$i.bmp";$b.Save($path,[Drawing.Imaging.ImageFormat]::Bmp);$records+=@{path=$path;sha256=(Get-FileHash $path -Algorithm SHA256).Hash.ToLower();translation=@((($i%3)*10),([math]::Floor($i/3)*10))}}finally{$g.Dispose();$b.Dispose()}
}}finally{$img.Dispose()}
@{authority='controlled documentation-derived synthetic fixture';source=$source;sourceSha256=(Get-FileHash $source -Algorithm SHA256).Hash.ToLower();crop=@(740,0,294,295);scale=3;canvas=@(1000,1000);fixtures=$records}|ConvertTo-Json -Depth 8|Set-Content (Join-Path $audit 'fixture.json')
