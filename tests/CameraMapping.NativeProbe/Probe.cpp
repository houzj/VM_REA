#include <windows.h>
#include <cstdio>
#include "CameraMapCpp.h"
#include "MVDCore.h"
using namespace VisionDesigner;
using namespace VisionDesigner::CameraMap;
int main(int argc,char**argv){
 SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);setvbuf(stdout,nullptr,_IONBF,0);if(argc!=3)return 2;
 auto dll=LoadLibraryA(argv[1]);if(!dll){fprintf(stderr,"load %lu",GetLastError());return 3;}
 auto create=(int(__stdcall*)(ICameraMapTool**))GetProcAddress(dll,"CreateCameraMapToolInstance");auto destroy=(int(__stdcall*)(ICameraMapTool*))GetProcAddress(dll,"DestroyCameraMapToolInstance");
 FILE*f=nullptr;fopen_s(&f,argv[2],"r");if(!f)return 4;char name[128];int freedom,weight,count,factor;
 while(fscanf_s(f,"%127s%d%d%d%d",name,(unsigned)sizeof(name),&freedom,&weight,&factor,&count)==5){
 ICameraMapTool*t=nullptr;int code=create(&t);printf("{\"name\":\"%s\",\"create\":%u",name,(unsigned)code);
 if(code){printf("}\n");return 5;}
 static bool printed=false;if(!printed){auto vtable=*(void***)t;fprintf(stderr,"wrapper vtable RVAs:");for(int i=0;i<9;i++)fprintf(stderr," %llx",(unsigned long long)((char*)vtable[i]-(char*)dll));fprintf(stderr,"\n");auto core=GetModuleHandleA("MVD_Algorithm.dll");auto handle=*(void**)((char*)t+0x18);auto table=*(void***)handle;fprintf(stderr,"core handle vtable RVAs:");for(int i=0;i<10;i++)fprintf(stderr," %llx",(unsigned long long)((char*)table[i]-(char*)core));fprintf(stderr,"\n");printed=true;}
 try{auto p=t->GetBasicParam();for(int i=0;i<count;i++){MVD_POINT_F o{},target{};fscanf_s(f,"%f%f%f%f",&o.fX,&o.fY,&target.fX,&target.fY);p->AddAlignPairPoint(o,target);}
 char v[32];sprintf_s(v,"%d",freedom);t->SetRunParam("HomoFreedom",v);sprintf_s(v,"%d",weight);t->SetRunParam("WeightFun",v);sprintf_s(v,"%d",factor);t->SetRunParam("WeightFactor",v);t->Run();
 auto result=t->GetResult();float h[9];result->GetTargetMatrix(h);printf(",\"ratio\":[%.9g,%.9g],\"matrix\":[",result->GetRatioX(),result->GetRatioY());for(int i=0;i<9;i++)printf("%s%.9g",i?",":"",h[i]);printf("]");
 }catch(const IMVDException&e){printf(",\"exception\":%u",(unsigned)e.GetErrorCode());}catch(...){printf(",\"unknownException\":true");}
 printf("}\n");destroy(t);
 }fclose(f);fflush(stdout);TerminateProcess(GetCurrentProcess(),0);return 0;
}
