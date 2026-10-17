"""Generate owned JPEG fixtures using the independently built official cjpeg tool."""
import argparse
import os
from pathlib import Path
import subprocess

parser=argparse.ArgumentParser()
parser.add_argument('--encoder',type=Path,default=Path('analysis/build/jpeg-backend/cjpeg.exe'))
parser.add_argument('--output',type=Path,default=Path('tests/ImageSource.PluginChecks/Fixtures/Jpeg'))
args=parser.parse_args()
args.output.mkdir(parents=True,exist_ok=True)
scratch=Path('analysis/full-audit/image-source-jpeg/encoder-inputs');scratch.mkdir(parents=True,exist_ok=True)
env=dict(os.environ);env['PATH']=str(args.encoder.resolve().parent/'sharedlib')+os.pathsep+env.get('PATH','')
cases=[('444-q90',17,19,90,'1x1',False,False),('422-q75',31,17,75,'2x1',False,False),('420-q35',65,49,35,'2x2',False,False),('420-q100',33,9,100,'2x2',False,False),('progressive-420',17,19,90,'2x2',True,False),('gray-q80',19,17,80,'1x1',False,True),('one-pixel',1,1,100,'1x1',False,False),('restart-420',65,49,85,'2x2',False,False)]
cases += [('tiny-2x1',2,1,90,'1x1',False,False),('tiny-1x2',1,2,90,'1x1',False,False),('tiny-2x2',2,2,90,'1x1',False,False),('column-1x17',1,17,90,'1x1',False,False),('row-17x1',17,1,90,'1x1',False,False),('gray-one-pixel',1,1,90,'1x1',False,True)]
cases += [(f'boundary-{w}x{h}',w,h,90,'1x1',False,False) for w,h in [(3,3),(3,2),(2,3),(4,4),(8,2),(2,8),(8,8),(16,2),(2,16),(16,16)]]
cases += [(f'boundary-{w}x{h}',w,h,90,'1x1',False,False) for w,h in [(3,4),(4,3),(4,5),(5,4),(3,17),(17,3),(4,17),(17,4)]]
for name,w,h,quality,sampling,progressive,gray in cases:
    raw=bytes(((i*73+((i//w)*47))%256) for i in range(w*h)) if gray else bytes(v for i in range(w*h) for v in ((i*73)%256,(i*151+31)%256,(i*199+17)%256))
    source=scratch/(name+'.pnm');source.write_bytes(f'{"P5" if gray else "P6"}\n{w} {h}\n255\n'.encode()+raw)
    command=[str(args.encoder.resolve()),'-quality',str(quality),'-dct','int','-sample',sampling]
    if progressive:command+=['-progressive']
    if name.startswith('restart'):command+=['-restart','1']
    command+=['-outfile',str((args.output/(name+'.jpg')).resolve()),str(source.resolve())]
    subprocess.run(command,check=True,env=env,capture_output=True)
base=(args.output/'444-q90.jpg').read_bytes()
# EXIF Orientation=6. Like the original raw JPEG API, decode without display rotation.
exif=b'Exif\0\0II*\0\x08\0\0\0\x01\0\x12\x01\x03\0\x01\0\0\0\x06\0\0\0\0\0\0\0'
(args.output/'orientation-6.jpg').write_bytes(base[:2]+b'\xff\xe1'+(len(exif)+2).to_bytes(2,'big')+exif+base[2:])
(args.output/'missing-eoi.jpg').write_bytes(base[:-2])
(args.output/'invalid-header.jpg').write_bytes(b'\xff\xd8\xff\xd9')
print('Created 35 JPEG fixtures')
