// Original exported methods in an owned synthetic instance. Not platform Init.
// Normal official algorithm object; no code patches, devices or authorization workarounds.
#include <windows.h>
#include <vector>
#include <string>
#include <cstdio>
#include "MVDCore.h"
#include "NImageCalibV2Cpp.h"
using namespace VisionDesigner;
using namespace VisionDesigner::NImageCalibV2;
struct Frame{IMvdImage*image;void*roi;void*mask;int kind;float x,y,angle;};
int main(int argc,char**argv){
 if(argc!=5)return 2;setvbuf(stdout,nullptr,_IONBF,0);SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);SetDllDirectoryA(argv[1]);
 HMODULE alg=LoadLibraryA("MVDNImageCalibV2Cpp.dll"),img=LoadLibraryA("MVDImageCpp.dll"),module=LoadLibraryA(argv[2]);if(!alg||!img||!module){printf("{\"phase\":\"load\",\"code\":%lu}\n",GetLastError());return 3;}
 auto create=(int(__stdcall*)(INImageCalibV2Tool**))GetProcAddress(alg,"CreateNImageCalibV2ToolInstance");
 auto imageCreate=(int(__stdcall*)(IMvdImage**))GetProcAddress(img,"CreateImageInstance");
 auto ctor=(void*(*)(void*))GetProcAddress(module,"??0CIMVSNImageCalibModu@@QEAA@XZ");
 auto dtor=(void(*)(void*))GetProcAddress(module,"??1CIMVSNImageCalibModu@@UEAA@XZ");
 auto set=(int(*)(void*,const char*,const char*,int))GetProcAddress(module,"?SetParam@CIMVSNImageCalibModu@@UEAAHPEBD0H@Z");
 auto get=(int(*)(void*,const char*,char*,int,int*))GetProcAddress(module,"?GetParam@CIMVSNImageCalibModu@@UEAAHPEBDPEADHPEAH@Z");
 auto run=(int(*)(void*,void*,void*))GetProcAddress(module,"?ProcessAlg@CIMVSNImageCalibModu@@AEAAHPEAU_MVDSDK_BASE_MODU_INPUT@CVmAlgModuleBase@@PEAUBASIC_MODU_OUTPUT@1@@Z");
 auto append=(void*(*)(void*,void*,void*,Frame*))((char*)module+0x1f5a0);
 std::vector<unsigned char>instance(0x16180);void*self=instance.data();ctor(self);INImageCalibV2Tool*tool=nullptr;int code=create(&tool);printf("{\"phase\":\"create-algorithm\",\"code\":%u}\n",(unsigned)code);if(code){dtor(self);return 4;}*(INImageCalibV2Tool**)(instance.data()+0x15d98)=tool;
 try{
  for(auto key:{"CalibPointTotalNum","RotPointTotalNum","BasePointX","BasePointY","MoveAlignX","MoveAlignY","MoveFirstType","ChangeDirectionMoveTime","BaseAngle","MoveAngle","CareraMove","HomoFixEnable","CalibBoardType","FilterStatus"}){char value[4096]={};int size=0;int c=get(self,key,value,sizeof(value),&size);printf("{\"phase\":\"get-default\",\"key\":\"%s\",\"code\":%u,\"value\":\"%s\"}\n",key,(unsigned)c,value);}
  set(self,"CalibPointTotalNum","9",2);set(self,"RotPointTotalNum","3",2);set(self,"CalibBoardType","7",2);
  for(int i=0;i<12;i++){IMvdImage*image=nullptr;imageCreate(&image);char path[1024];if(i<9)sprintf_s(path,"%s-%d.bmp",argv[3],i);else sprintf_s(path,"%s-r%d.bmp",argv[3],i-9);image->InitImage(path,MVD_PIXEL_MONO_08);Frame f{image,nullptr,nullptr,i<9?1:2,i<9?(float)(i%3):0,i<9?(float)(i/3):0,i<9?0:(float)((i-9)*10)};auto head=*(void***)(instance.data()+0x160b0);void*node=append(instance.data()+0x160b0,head,head[1],&f);head[1]=node;**(void***)((char*)node+8)=node;(*(size_t*)(instance.data()+0x160b8))++;}
  int result[8]={};code=run(self,(void*)1,result);printf("{\"phase\":\"original-process-alg\",\"code\":%u,\"status\":%d,\"nativeError\":%u}\n",(unsigned)code,result[0],(unsigned)result[1]);
  int save=set(self,"SaveCalibPath",argv[4],(int)strlen(argv[4])+1);printf("{\"phase\":\"original-save\",\"code\":%u}\n",(unsigned)save);
 set(self,"CareraMove","True",5);std::string moving=std::string(argv[4])+".moving.xml";int moved=set(self,"SaveCalibPath",moving.c_str(),(int)moving.size()+1);printf("{\"phase\":\"moving-save\",\"code\":%u}\n",(unsigned)moved);
 }catch(IMVDException&ex){printf("{\"phase\":\"sdk-exception\",\"code\":%u}\n",(unsigned)ex.GetErrorCode());}catch(int e){printf("{\"phase\":\"module-exception\",\"code\":%u}\n",(unsigned)e);}catch(...){printf("{\"phase\":\"unknown-exception\"}\n");}
 dtor(self);return 0;
}
