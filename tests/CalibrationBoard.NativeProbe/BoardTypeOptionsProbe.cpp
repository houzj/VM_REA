// Same original module/SDK instance across all six XML-exposed board types.
// Reuses the proven owned-thread hardware breakpoint setup; no algorithm code patch.
#define main ExistingBoardModuleMain
#include "ModuleProbe.cpp"
#undef main
#include <cstdint>
static uintptr_t edgeAddresses[3]{},edgeCore=0;
static int edgeStage=-1;
static bool edgeStop=false;
static void* edgeHandler=nullptr;
static HANDLE edgeTarget=nullptr;
static DWORD WINAPI ConfigureBoardBreaks(void*){
 if(SuspendThread(edgeTarget)==(DWORD)-1)return 1;CONTEXT c{};c.ContextFlags=CONTEXT_DEBUG_REGISTERS;bool ok=GetThreadContext(edgeTarget,&c)!=0;
 if(ok){c.Dr0=edgeStop?0:edgeAddresses[0];c.Dr1=edgeStop?0:edgeAddresses[1];c.Dr2=edgeStop?0:edgeAddresses[2];c.Dr3=0;c.Dr6=0;c.Dr7=edgeStop?0:0x15;ok=SetThreadContext(edgeTarget,&c)!=0;}ResumeThread(edgeTarget);return ok?0:2;
}
static bool configureEdge(){auto thread=CreateThread(nullptr,0,ConfigureBoardBreaks,nullptr,0,nullptr);if(!thread)return false;WaitForSingleObject(thread,INFINITE);DWORD code=3;GetExitCodeThread(thread,&code);CloseHandle(thread);return code==0;}
static bool readable(const void* address,size_t bytes){MEMORY_BASIC_INFORMATION p{};return address&&VirtualQuery(address,&p,sizeof(p))&&p.State==MEM_COMMIT&&!(p.Protect&(PAGE_NOACCESS|PAGE_GUARD))&&(uintptr_t)address+bytes<=(uintptr_t)p.BaseAddress+p.RegionSize;}
static std::string observedString(uintptr_t address){std::string value;for(size_t i=0;i<96&&readable((void*)(address+i),1);i++){unsigned char ch=*(unsigned char*)(address+i);if(!ch)break;if(ch<' '||ch>'~'){value+="<non-ascii>";break;}if(ch=='"'||ch=='\\')value+='\\';value+=(char)ch;}return value;}
static int optionBoard=0;
static LONG CALLBACK ObserveBoardOptions(EXCEPTION_POINTERS* e){
 auto c=e->ContextRecord;if(e->ExceptionRecord->ExceptionCode!=EXCEPTION_SINGLE_STEP)return EXCEPTION_CONTINUE_SEARCH;
 int which=-1;for(int i=0;i<3;i++)if(c->Rip==edgeAddresses[i])which=i;if(which<0)return EXCEPTION_CONTINUE_SEARCH;
 if(which==0){auto name=observedString(c->Rdx),value=observedString(c->R8);printf("{\"phase\":\"setter\",\"stage\":%d,\"board\":%d,\"name\":\"%s\",\"value\":\"%s\"}\n",edgeStage,optionBoard,name.c_str(),value.c_str());}
 else{auto options=*(unsigned char**)(c->Rsp+0x50);if(!readable(options,0x50))return EXCEPTION_CONTINUE_SEARCH;printf("{\"phase\":\"%s\",\"stage\":%d,\"board\":%d,\"callerRva\":%llu,\"options\":\"%llx\",\"coreKind\":%u,\"slot24Bits\":%u,\"low\":%d,\"high\":%d,\"words\":[",which==1?"pipeline-entry":"dispatch-entry",edgeStage,optionBoard,*(uintptr_t*)c->Rsp-edgeCore,(unsigned long long)options,*(unsigned*)(options+8),*(unsigned*)(options+0x24),*(int*)(options+0x28),*(int*)(options+0x2c));for(int j=0;j<20;j++)printf("%s%u",j?",":"",((unsigned*)options)[j]);printf("]}\n");}
 c->Dr6=0;c->EFlags|=0x10000;return EXCEPTION_CONTINUE_EXECUTION;
}
int main(int argc,char** argv){
 if(argc!=9)return 2;SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);setvbuf(stdout,nullptr,_IONBF,0);SetDllDirectoryA(argv[1]);
 for(auto name:{"HSlog.dll","ModuleFrame.dll"})LoadLibraryExA((std::filesystem::path(argv[2])/"ModuleProxy"/"x64"/name).string().c_str(),nullptr,LOAD_WITH_ALTERED_SEARCH_PATH);
 auto sdk=LoadLibraryA("MVDCalibBoardCalibCpp.dll"),images=LoadLibraryA("MVDImageCpp.dll"),module=LoadLibraryExA(argv[3],nullptr,LOAD_WITH_ALTERED_SEARCH_PATH);if(!sdk||!images||!module)return 3;
 auto create=(int(__stdcall*)(ICalibBoardCalibTool**))GetProcAddress(sdk,"CreateCalibBoardCalibToolInstance");auto imageCreate=(int(__stdcall*)(IMvdImage**))GetProcAddress(images,"CreateImageInstance");
 auto ctor=(void*(*)(void*))GetProcAddress(module,"??0CIMVSCalibBoardCalibModu@@QEAA@XZ");auto dtor=(void(*)(void*))GetProcAddress(module,"??1CIMVSCalibBoardCalibModu@@UEAA@XZ");
 auto set=(int(*)(void*,const char*,const char*,int))GetProcAddress(module,"?SetParam@CIMVSCalibBoardCalibModu@@UEAAHPEBD0H@Z");auto run=(int(*)(void*,void*,void*))GetProcAddress(module,"?ProcessAlg@CIMVSCalibBoardCalibModu@@AEAAHPEAU_MVDSDK_BASE_MODU_INPUT@CVmAlgModuleBase@@PEAUBASIC_MODU_OUTPUT@1@@Z");if(!create||!imageCreate||!ctor||!dtor||!set||!run)return 4;
 install(module);std::vector<unsigned char> object(0x20000),input(0x200);ctor(object.data());ICalibBoardCalibTool* tool=nullptr;IMvdImage* image=nullptr;if(create(&tool)||imageCreate(&image))return 5;*(void**)(object.data()+0x15d98)=tool;
 edgeCore=(uintptr_t)GetModuleHandleA("MVD_Algorithm.dll");edgeAddresses[0]=(uintptr_t)sdk+0x2980;edgeAddresses[1]=edgeCore+0x7e4ff0;edgeAddresses[2]=edgeCore+0x815560;
 edgeHandler=AddVectoredExceptionHandler(1,ObserveBoardOptions);edgeTarget=OpenThread(THREAD_SUSPEND_RESUME|THREAD_GET_CONTEXT|THREAD_SET_CONTEXT,FALSE,GetCurrentThreadId());if(!edgeTarget||!configureEdge())return 6;
 int result=0;
 try{
  auto param=[&](const char* name,const char* value){int code=set(object.data(),name,value,(int)strlen(value));if(code)throw code;};
  param("HomoFreedom","1");param("CalibBoardWeightFun","2");param("Circularity","20");
  const int boards[]={1,2,6,7,8,9};const char* lows[]={"20","0","20"};const char* highs[]={"40","40","80"};
  for(int board:boards){optionBoard=board;char value[16];sprintf_s(value,"%d",board);param("CalibBoardType",value);
   if(board<=2){image->InitImage(480,400,MVD_PIXEL_MONO_08);auto pixels=image->GetImageData(0);for(int y=0;y<400;y++)for(int x=0;x<480;x++){unsigned char p=255;if(board==1&&x>=60&&x<420&&y>=50&&y<350)p=(((x-60)/30+(y-50)/30)&1)?255:0;pixels->pData[y*pixels->nRowStep+x]=p;}if(board==2)for(int r=0;r<8;r++)for(int col=0;col<10;col++)for(int y=-9;y<=9;y++)for(int x=-9;x<=9;x++)if(x*x+y*y<81)pixels->pData[(80+r*30+y)*pixels->nRowStep+90+col*30+x]=0;}
   else image->InitImage(argv[board-1],MVD_PIXEL_MONO_08);
   *(void**)(input.data()+0x10)=image;*(int*)(input.data()+4)=1;float roi[]={image->GetWidth()*.5f,image->GetHeight()*.5f,(float)image->GetWidth(),(float)image->GetHeight(),0};memcpy(input.data()+0xa4,roi,sizeof(roi));
   for(int variant=0;variant<3;variant++){edgeStage++;param("EdgeThreshLow",lows[variant]);param("EdgeThreshHigh",highs[variant]);printf("{\"phase\":\"stage\",\"stage\":%d,\"board\":%d,\"variant\":%d,\"width\":%d,\"height\":%d,\"low\":%s,\"high\":%s}\n",edgeStage,board,variant,image->GetWidth(),image->GetHeight(),lows[variant],highs[variant]);int output[8]{};int code=run(object.data(),input.data(),output);printf("{\"phase\":\"run\",\"stage\":%d,\"board\":%d,\"code\":%u,\"status\":%d}\n",edgeStage,board,(unsigned)code,output[0]);}
  }
 }catch(IMVDException& e){printf("{\"phase\":\"exception\",\"code\":%u}\n",e.GetErrorCode());result=7;}catch(...){printf("{\"phase\":\"exception\"}\n");result=8;}
 edgeStop=true;configureEdge();CloseHandle(edgeTarget);RemoveVectoredExceptionHandler(edgeHandler);dtor(object.data());restore();return result;
}
