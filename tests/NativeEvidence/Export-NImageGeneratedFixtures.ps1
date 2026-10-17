param([string]$InstallRoot='D:/Program Files/VisionMaster4.4.0')
$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Drawing
$root=Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$audit=Join-Path $root 'analysis/full-audit/n-image-calibration/board-generator'
$records=@()
foreach($n in 1..4){
 $source=Join-Path $audit "board-$n.jpg"
 $im=[Drawing.Image]::FromFile($source)
 try{foreach($i in 0..8){
  $bmp=[Drawing.Bitmap]::new(1000,1000);$g=[Drawing.Graphics]::FromImage($bmp)
  try{$g.Clear([Drawing.Color]::White);$g.InterpolationMode=[Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic;$g.DrawImage($im,50+($i%3)*10,50+[math]::Floor($i/3)*10,900,900)
   $path=Join-Path $audit "generated-$n-$i.bmp";$bmp.Save($path,[Drawing.Imaging.ImageFormat]::Bmp)
   if($i -eq 0){$png=Join-Path $root "tests/NImageCalibration.PluginChecks/Fixtures/hik-generated-$n.png";$bmp.Save($png,[Drawing.Imaging.ImageFormat]::Png)}
   $records+=@{board=$n;frame=$i;path=$path;sha256=(Get-FileHash $path -Algorithm SHA256).Hash.ToLower();sourceSha256=(Get-FileHash $source -Algorithm SHA256).Hash.ToLower();sourceSize=@($im.Width,$im.Height)}
  }finally{$g.Dispose();$bmp.Dispose()}
 }}finally{$im.Dispose()}
}
@{authority='controlled official-generator images resampled for algorithm input; no device';canvas=@(1000,1000);destination=@(50,50,900,900);interpolation='HighQualityBicubic';fixtures=$records}|ConvertTo-Json -Depth 8|Set-Content (Join-Path $audit 'fixtures.json')
