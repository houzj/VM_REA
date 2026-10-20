// Owned original module/SDK instance. Hardware execution breakpoints observe the
// setter ABI and candidate options; no code bytes or target buffers are changed.
#define main ExistingBoardModuleProbeMain
#include "ModuleProbe.cpp"
#undef main
#include <cstdint>

static uintptr_t edgeAddresses[3]{};
static uintptr_t edgeCore=0;
static int edgeStage=-1;
static bool edgeStop=false;
static void* edgeHandler=nullptr;
static HANDLE edgeTarget=nullptr;
static DWORD WINAPI ConfigureEdgeBreaks(void*){
 if(SuspendThread(edgeTarget)==(DWORD)-1)return 1;
 CONTEXT c{};c.ContextFlags=CONTEXT_DEBUG_REGISTERS;bool ok=GetThreadContext(edgeTarget,&c)!=0;
 if(ok){c.Dr0=edgeStop?0:edgeAddresses[0];c.Dr1=edgeStop?0:edgeAddresses[1];c.Dr2=edgeStop?0:edgeAddresses[2];c.Dr3=0;c.Dr6=0;c.Dr7=edgeStop?0:0x15;ok=SetThreadContext(edgeTarget,&c)!=0;}
 ResumeThread(edgeTarget);return ok?0:2;
}
static bool configureEdge(){auto worker=CreateThread(nullptr,0,ConfigureEdgeBreaks,nullptr,0,nullptr);if(!worker)return false;WaitForSingleObject(worker,INFINITE);DWORD code=3;GetExitCodeThread(worker,&code);CloseHandle(worker);return code==0;}
static bool readable(const void* address,size_t bytes){MEMORY_BASIC_INFORMATION page{};return address&&VirtualQuery(address,&page,sizeof(page))&&page.State==MEM_COMMIT&&!(page.Protect&(PAGE_NOACCESS|PAGE_GUARD))&&(uintptr_t)address+bytes<=(uintptr_t)page.BaseAddress+page.RegionSize;}
static std::string observedString(uintptr_t address){
 if(!readable((void*)address,1))return "<unreadable>";std::string value;
 for(size_t i=0;i<96&&readable((void*)(address+i),1);i++){unsigned char ch=*(unsigned char*)(address+i);if(!ch)break;if(ch<' '||ch>'~'){value+="<non-ascii>";break;}if(ch=='"'||ch=='\\')value+='\\';value+=(char)ch;}
 return value;
}
static LONG CALLBACK ObserveEdge(EXCEPTION_POINTERS* e){
 auto c=e->ContextRecord;
 if(e->ExceptionRecord->ExceptionCode!=EXCEPTION_SINGLE_STEP)return EXCEPTION_CONTINUE_SEARCH;
 int which=-1;for(int i=0;i<3;i++)if(c->Rip==edgeAddresses[i])which=i;if(which<0)return EXCEPTION_CONTINUE_SEARCH;
 if(which<2){auto name=observedString(c->Rdx),value=observedString(c->R8);printf("{\"phase\":\"%s\",\"stage\":%d,\"rcx\":\"%llx\",\"rdx\":\"%llx\",\"r8\":\"%llx\",\"name\":\"%s\",\"value\":\"%s\"}\n",which==0?"sdk-setter-entry":"algorithm-setter-entry",edgeStage,c->Rcx,c->Rdx,c->R8,name.c_str(),value.c_str());}
 else{auto options=*(unsigned char**)(c->Rsp+0x50);if(!readable(options,12)){printf("{\"phase\":\"candidate-entry-unreadable\",\"stage\":%d}\n",edgeStage);}else{printf("{\"phase\":\"candidate-entry\",\"stage\":%d,\"options\":\"%llx\",\"callerRva\":%llu,\"circularityBits\":%u,\"low\":%d,\"high\":%d}\n",edgeStage,(unsigned long long)options,*(uintptr_t*)c->Rsp-edgeCore,*(unsigned*)options,*(int*)(options+4),*(int*)(options+8));}}
 c->Dr6=0;c->EFlags|=0x10000;return EXCEPTION_CONTINUE_EXECUTION;
}
int main(int argc,char** argv){
 SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);setvbuf(stdout,nullptr,_IONBF,0);if(argc!=5)return 2;SetDllDirectoryA(argv[1]);
 for(auto name:{"HSlog.dll","ModuleFrame.dll"})LoadLibraryExA((std::filesystem::path(argv[2])/"ModuleProxy"/"x64"/name).string().c_str(),nullptr,LOAD_WITH_ALTERED_SEARCH_PATH);
 auto sdk=LoadLibraryA("MVDCalibBoardCalibCpp.dll"),images=LoadLibraryA("MVDImageCpp.dll"),module=LoadLibraryExA(argv[3],nullptr,LOAD_WITH_ALTERED_SEARCH_PATH);if(!sdk||!images||!module)return 3;
 auto create=(int(__stdcall*)(ICalibBoardCalibTool**))GetProcAddress(sdk,"CreateCalibBoardCalibToolInstance");auto imageCreate=(int(__stdcall*)(IMvdImage**))GetProcAddress(images,"CreateImageInstance");
 auto ctor=(void*(*)(void*))GetProcAddress(module,"??0CIMVSCalibBoardCalibModu@@QEAA@XZ");auto dtor=(void(*)(void*))GetProcAddress(module,"??1CIMVSCalibBoardCalibModu@@UEAA@XZ");
 auto set=(int(*)(void*,const char*,const char*,int))GetProcAddress(module,"?SetParam@CIMVSCalibBoardCalibModu@@UEAAHPEBD0H@Z");auto run=(int(*)(void*,void*,void*))GetProcAddress(module,"?ProcessAlg@CIMVSCalibBoardCalibModu@@AEAAHPEAU_MVDSDK_BASE_MODU_INPUT@CVmAlgModuleBase@@PEAUBASIC_MODU_OUTPUT@1@@Z");if(!create||!imageCreate||!ctor||!dtor||!set||!run)return 4;
 install(module);std::vector<unsigned char> object(0x20000),input(0x200);ctor(object.data());ICalibBoardCalibTool* tool=nullptr;IMvdImage* image=nullptr;if(create(&tool)||imageCreate(&image))return 5;*(void**)(object.data()+0x15d98)=tool;
 auto core=GetModuleHandleA("MVD_Algorithm.dll");edgeCore=(uintptr_t)core;edgeAddresses[0]=(uintptr_t)sdk+0x2980;edgeAddresses[1]=(uintptr_t)GetProcAddress(core,"MVD_Algorithm_SetValueByString");edgeAddresses[2]=(uintptr_t)core+0x877230;if(!edgeAddresses[1])return 6;
 printf("{\"phase\":\"hardware-observer\",\"sdkSetterRva\":10624,\"candidateRva\":%u,\"codeBytesChanged\":false,\"platformInit\":false,\"devices\":false}\n",0x877230);edgeHandler=AddVectoredExceptionHandler(1,ObserveEdge);edgeTarget=OpenThread(THREAD_SUSPEND_RESUME|THREAD_GET_CONTEXT|THREAD_SET_CONTEXT,FALSE,GetCurrentThreadId());if(!edgeTarget||!configureEdge())return 9;
 int result=0;
 try{
  auto param=[&](const char* key,const char* value){int code=set(object.data(),key,value,(int)strlen(value));printf("{\"phase\":\"module-set\",\"stage\":%d,\"name\":\"%s\",\"value\":\"%s\",\"code\":%u}\n",edgeStage,key,value,(unsigned)code);};
  param("CalibBoardType","2");param("HomoFreedom","1");param("CalibBoardWeightFun","2");param("Circularity","20");
  image->InitImage(480,400,MVD_PIXEL_MONO_08);auto pixels=image->GetImageData(0);for(int y=0;y<400;y++)memset(pixels->pData+y*pixels->nRowStep,255,480);
  for(int row=0;row<8;row++)for(int col=0;col<10;col++)for(int y=-9;y<=9;y++)for(int x=-9;x<=9;x++)if(x*x+y*y<81)pixels->pData[(80+row*30+y)*pixels->nRowStep+90+col*30+x]=250;
  *(void**)(input.data()+0x10)=image;*(int*)(input.data()+4)=1;float roi[]={240,200,480,400,0};memcpy(input.data()+0xa4,roi,sizeof(roi));
  struct Stage{const char* route;const char* low;const char* high;};Stage stages[]={{"module","20","40"},{"module","0","40"},{"module","20","80"},{"sdk","20","40"},{"sdk","0","40"},{"sdk","20","80"}};
  for(edgeStage=0;edgeStage<6;edgeStage++){auto stage=stages[edgeStage];printf("{\"phase\":\"stage\",\"stage\":%d,\"route\":\"%s\",\"low\":%s,\"high\":%s}\n",edgeStage,stage.route,stage.low,stage.high);if(std::string(stage.route)=="module"){param("EdgeThreshLow",stage.low);param("EdgeThreshHigh",stage.high);}else{tool->SetRunParam("EdgeThreshLow",stage.low);tool->SetRunParam("EdgeThreshHigh",stage.high);}int output[8]{};int code=run(object.data(),input.data(),output);printf("{\"phase\":\"run\",\"stage\":%d,\"code\":%u,\"status\":%d}\n",edgeStage,(unsigned)code,output[0]);}
 }catch(IMVDException& e){printf("{\"phase\":\"exception\",\"code\":%u}\n",e.GetErrorCode());result=7;}catch(...){printf("{\"phase\":\"unknown-exception\"}\n");result=8;}
 edgeStop=true;configureEdge();CloseHandle(edgeTarget);RemoveVectoredExceptionHandler(edgeHandler);dtor(object.data());restore();return result;
}
