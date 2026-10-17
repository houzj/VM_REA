"""Controlled official gen_calibration calls; ABI recovered from shipped Python 3.6 wrapper."""
import ctypes,pathlib,os,json,shutil,struct,zlib,hashlib
root=pathlib.Path('analysis/full-audit/n-image-calibration/board-generator').resolve();os.chdir(root)
# Extract only bundled dependent DLLs. No installed files are modified.
source=pathlib.Path('D:/Program Files/VisionMaster4.4.0/Applications/Tools/CalibrationBoardGenerationTool/DemoGenCalBoard_en.exe')
b=source.read_bytes();pos=b.rfind(b'MEI\014\013\012\013\016');magic,size,tocoff,toclen,py,lib=struct.unpack('!8sIIII64s',b[pos:pos+88]);base=pos+88-size;toc=b[base+tocoff:base+tocoff+toclen];out=root/'extracted';out.mkdir(exist_ok=True);i=0
while i<len(toc):
 length,offset,compressed,uncompressed,flag,kind=struct.unpack('!IIIIbc',toc[i:i+18]);name=toc[i+18:i+length].rstrip(b'\0').decode();i+=length
 if name.endswith('.dll') or name=='DemoGenCalBoard_en':
  data=b[base+offset:base+offset+compressed]
  if flag:data=zlib.decompress(data)
  (out/(name+('.marshal' if kind==b's' else ''))).write_bytes(data)
class Point(ctypes.Structure):_fields_=[('x',ctypes.c_int),('y',ctypes.c_int)]
class Input(ctypes.Structure):_fields_=[('row_num',ctypes.c_int),('col_num',ctypes.c_int),('board_type',ctypes.c_int),('mark_dist',ctypes.c_double),('hik_1_code_scale',ctypes.c_int)]
depdir=os.add_dll_directory(str(root/'extracted'));sdkdir=os.add_dll_directory('D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64');dll=ctypes.CDLL('D:/Program Files/VisionMaster4.4.0/Applications/Tools/CalibrationBoardGenerationTool/DemoMvbCalib.dll')
for board in [1,2,3,4]:
 p=Input(16,16,board,.01,1);line=(Point*(16*16*4))();code=(Point*(16*16*256))();locate=(Point*(16*16*4))();mark=(Point*(16*16*50))();counts=[(ctypes.c_int*1)() for _ in range(4)];cr=(ctypes.c_float*1)();lr=(ctypes.c_float*6)()
 result=dll.gen_calibration(p,line,counts[0],code,counts[1],cr,locate,counts[2],lr,mark,counts[3],0)
 shutil.copyfile(root/'calibboard.jpg',root/f'board-{board}.jpg')
 data={'board':board,'return':result,'counts':[c[0] for c in counts],'codeRadius':list(cr),'locateRadii':list(lr),'lines':[[p.x,p.y] for p in line[:counts[0][0]]],'code':[[p.x,p.y] for p in code[:counts[1][0]]],'locate':[[p.x,p.y] for p in locate[:counts[2][0]]]}
 data['producerSha256']=hashlib.sha256(pathlib.Path(__file__).read_bytes()).hexdigest();data['generatorSha256']=hashlib.sha256(source.read_bytes()).hexdigest();data['nativeSha256']=hashlib.sha256(pathlib.Path('D:/Program Files/VisionMaster4.4.0/Applications/Tools/CalibrationBoardGenerationTool/DemoMvbCalib.dll').read_bytes()).hexdigest();data['imageSha256']=hashlib.sha256((root/f'board-{board}.jpg').read_bytes()).hexdigest();(root/f'board-{board}.json').write_text(json.dumps(data));print(board,result,data['counts'],flush=True)



