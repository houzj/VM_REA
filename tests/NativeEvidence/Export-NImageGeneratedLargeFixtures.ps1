# Same official II/expanded-II JPEG inputs, at a resolution that the native detector accepts.
$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Drawing
$root=Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$audit=Join-Path $root 'analysis/full-audit/n-image-calibration/board-generator'
foreach($n in 2,4){
 $im=[Drawing.Image]::FromFile((Join-Path $audit "board-$n.jpg"))
 try{foreach($i in 0..8){
  $bmp=[Drawing.Bitmap]::new(2000,2000);$g=[Drawing.Graphics]::FromImage($bmp)
  try{$g.Clear([Drawing.Color]::White);$g.InterpolationMode=[Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic;$g.DrawImage($im,100+($i%3)*10,100+[math]::Floor($i/3)*10,1800,1800)
   $bmp.Save((Join-Path $audit "large-$n-$i.bmp"),[Drawing.Imaging.ImageFormat]::Bmp)
   if($i -eq 0){$bmp.Save((Join-Path $root "tests/NImageCalibration.PluginChecks/Fixtures/hik-generated-$n-large.png"),[Drawing.Imaging.ImageFormat]::Png)}
  }finally{$g.Dispose();$bmp.Dispose()}
 }}finally{$im.Dispose()}
}
