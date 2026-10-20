// Controlled original-module probe for retired subpixel parameters.
// Reuses one original module instance, one SDK tool instance and identical board frames.
#include <windows.h>
#include <vector>
#include <string>
#include <cstdio>
#include <cstring>
#include "MVDCore.h"
#include "NImageCalibV2Cpp.h"
using namespace VisionDesigner;
using namespace VisionDesigner::NImageCalibV2;
struct Frame { IMvdImage* image; void* roi; void* mask; int kind; float x,y,angle; };
using SetParam = int(*)(void*,const char*,const char*,int);
using GetParam = int(*)(void*,const char*,char*,int,int*);
using ProcessAlg = int(*)(void*,void*,void*);
static void ReadParam(GetParam get, void* self, const char* key) {
 char value[4096]{}; int size=0; int code=get(self,key,value,sizeof(value),&size);
 printf("{\"phase\":\"parameter\",\"key\":\"%s\",\"code\":%u,\"size\":%d,\"value\":\"%s\"}\n",key,(unsigned)code,size,value);
}
static void SetAndRead(SetParam set, GetParam get, void* self, const char* key, const char* value) {
 int code=set(self,key,value,(int)std::strlen(value)+1);
 printf("{\"phase\":\"set-parameter\",\"key\":\"%s\",\"value\":\"%s\",\"code\":%u}\n",key,value,(unsigned)code);
 ReadParam(get,self,key);
}
static void DumpResult(INImageCalibV2Tool* tool, const char* label, int moduleCode, int status, unsigned error) {
 auto result=tool->GetResult();
 if(!result){printf("{\"phase\":\"result\",\"label\":\"%s\",\"moduleCode\":%u,\"status\":%d,\"nativeError\":%u,\"nullResult\":true}\n",label,(unsigned)moduleCode,status,error);return;}
 float matrix[9]{};result->GetHomoMatrix(matrix);
 printf("{\"phase\":\"result\",\"label\":\"%s\",\"moduleCode\":%u,\"status\":%d,\"nativeError\":%u,\"sdkStatus\":%u,\"pairs\":%d,\"matrix\":[",label,(unsigned)moduleCode,status,error,(unsigned)result->GetStatus(),result->GetCornerPairsNum());
 for(int i=0;i<9;i++)printf("%s%.9g",i?",":"",matrix[i]);
 printf("],\"corners\":[");
 for(int i=0;i<result->GetCornerPairsNum();i++){
  auto pair=result->GetCornerPairs(i); auto world=pair->GetCornersWorld();
  printf("%s{\"world\":[%d,%d],\"pixels\":[",i?",":"",world.nX,world.nY);
  for(int j=0;j<pair->GetCornersNum();j++){auto point=pair->GetCorners(j);printf("%s[%.9g,%.9g]",j?",":"",point.fX,point.fY);}
  printf("]}");
 }
 printf("]}\n");
}
int main(int argc,char**argv){
 if(argc!=4)return 2;setvbuf(stdout,nullptr,_IONBF,0);SetDllDirectoryA(argv[1]);
 HMODULE alg=LoadLibraryA("MVDNImageCalibV2Cpp.dll"),img=LoadLibraryA("MVDImageCpp.dll"),module=LoadLibraryA(argv[2]);
 if(!alg||!img||!module){printf("{\"phase\":\"load\",\"code\":%lu}\n",GetLastError());return 3;}
 auto create=(int(__stdcall*)(INImageCalibV2Tool**))GetProcAddress(alg,"CreateNImageCalibV2ToolInstance");
 auto imageCreate=(int(__stdcall*)(IMvdImage**))GetProcAddress(img,"CreateImageInstance");
 auto ctor=(void*(*)(void*))GetProcAddress(module,"??0CIMVSNImageCalibModu@@QEAA@XZ");
 auto dtor=(void(*)(void*))GetProcAddress(module,"??1CIMVSNImageCalibModu@@UEAA@XZ");
 auto set=(SetParam)GetProcAddress(module,"?SetParam@CIMVSNImageCalibModu@@UEAAHPEBD0H@Z");
 auto get=(GetParam)GetProcAddress(module,"?GetParam@CIMVSNImageCalibModu@@UEAAHPEBDPEADHPEAH@Z");
 auto run=(ProcessAlg)GetProcAddress(module,"?ProcessAlg@CIMVSNImageCalibModu@@AEAAHPEAU_MVDSDK_BASE_MODU_INPUT@CVmAlgModuleBase@@PEAUBASIC_MODU_OUTPUT@1@@Z");
 // The original export is resolved first; the decompiled RVA is a fallback for this hash-pinned binary only.
 if(!run)run=(ProcessAlg)((char*)module+0x21450);
 auto append=(void*(*)(void*,void*,void*,Frame*))((char*)module+0x1f5a0);
 if(!create||!imageCreate||!ctor||!dtor||!set||!get||!append||!run){printf("{\"phase\":\"resolve\",\"code\":1}\n");return 4;}
 std::vector<unsigned char>instance(0x16180);void*self=instance.data();ctor(self);INImageCalibV2Tool*tool=nullptr;
 int code=create(&tool);printf("{\"phase\":\"create\",\"code\":%u}\n",(unsigned)code);if(code){dtor(self);return 5;}
 *(INImageCalibV2Tool**)(instance.data()+0x15d98)=tool;
 std::vector<IMvdImage*> images;
 try{
  ReadParam(get,self,"SubpixelWinAuto");ReadParam(get,self,"SubPixelWindowSize");
  ReadParam(get,self,"SubpixelWin");ReadParam(get,self,"SubpixelWinSizeUserSetValue");
  SetAndRead(set,get,self,"CalibPointTotalNum","9");SetAndRead(set,get,self,"RotPointTotalNum","0");SetAndRead(set,get,self,"CalibBoardType","7");
  for(int i=0;i<9;i++){
   IMvdImage*image=nullptr;imageCreate(&image);images.push_back(image);char path[1024];sprintf_s(path,"%s-%d.bmp",argv[3],i);image->InitImage(path,MVD_PIXEL_MONO_08);
   Frame frame{image,nullptr,nullptr,1,(float)(i%3),(float)(i/3),0};auto head=*(void***)(instance.data()+0x160b0);void*node=append(instance.data()+0x160b0,head,head[1],&frame);head[1]=node;**(void***)((char*)node+8)=node;(*(size_t*)(instance.data()+0x160b8))++;
  }
  auto execute=[&](const char*label){int output[8]{};int c=run(self,(void*)1,output);DumpResult(tool,label,c,output[0],(unsigned)output[1]);};
  execute("module-default");
  SetAndRead(set,get,self,"SubpixelWinAuto","1");SetAndRead(set,get,self,"SubPixelWindowSize","7");execute("module-resource-default-attempt");
  SetAndRead(set,get,self,"SubpixelWin","1");SetAndRead(set,get,self,"SubpixelWinSizeUserSetValue","7");execute("sdk-alias-resource-default");
  SetAndRead(set,get,self,"SubpixelWin","2");SetAndRead(set,get,self,"SubpixelWinSizeUserSetValue","3");execute("sdk-alias-lower-window");
  SetAndRead(set,get,self,"SubpixelWin","1");SetAndRead(set,get,self,"SubpixelWinSizeUserSetValue","150");execute("sdk-alias-upper-window");
  SetAndRead(set,get,self,"SubpixelWin","2");SetAndRead(set,get,self,"SubpixelWinSizeUserSetValue","150");execute("sdk-alias-both-alternate");
  SetAndRead(set,get,self,"SubpixelWinAuto","2");execute("module-auto-boundary-user");
  SetAndRead(set,get,self,"SubpixelWinAuto","1");SetAndRead(set,get,self,"SubPixelWindowSize","3");execute("module-size-lower-bound");
  SetAndRead(set,get,self,"SubPixelWindowSize","150");execute("module-size-upper-bound");
  SetAndRead(set,get,self,"SubpixelWinAuto","2");SetAndRead(set,get,self,"SubPixelWindowSize","150");execute("module-both-alternate");
 }catch(IMVDException&ex){printf("{\"phase\":\"sdk-exception\",\"code\":%u}\n",(unsigned)ex.GetErrorCode());}catch(int e){printf("{\"phase\":\"module-exception\",\"code\":%u}\n",(unsigned)e);}catch(...){printf("{\"phase\":\"unknown-exception\"}\n");}
 dtor(self);return 0;
}
