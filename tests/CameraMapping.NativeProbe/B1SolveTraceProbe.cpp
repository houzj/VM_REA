// B1 observes owned original CameraMap SDK execution using thread-local hardware breakpoints.
// No original DLL bytes or algorithm state are patched.
#define main ReachableMainUnused
#include "ReachableMatrixModuleProbe.cpp"
#undef main
#include <cstdint>
static uintptr_t b1core=0,b1addresses[4]{};static HANDLE b1thread=nullptr;static bool b1stop=false;
static bool b1residualMode=false,b1residualReturn=false;static int b1residualCount=0,b1residualSeq=0;static float* b1errors=nullptr;static float* b1sigma=nullptr;static const char* b1case="";static int b1sequence=0,b1refine=0;static float* b1solution=nullptr;static int b1dimension=0;static bool b1return2=false,b1return3=false;static float* b1refineOutput=nullptr;
static DWORD WINAPI SetB1(void*){if(SuspendThread(b1thread)==(DWORD)-1)return 1;CONTEXT c{};c.ContextFlags=CONTEXT_DEBUG_REGISTERS;bool ok=GetThreadContext(b1thread,&c)!=0;if(ok){c.Dr0=b1stop?0:b1addresses[0];c.Dr1=b1stop?0:b1addresses[1];c.Dr2=b1stop?0:b1addresses[2];c.Dr3=b1stop?0:b1addresses[3];c.Dr6=0;c.Dr7=b1stop?0:0x55;ok=SetThreadContext(b1thread,&c)!=0;}ResumeThread(b1thread);return ok?0:2;}
static bool configureB1(){auto t=CreateThread(nullptr,0,SetB1,nullptr,0,nullptr);if(!t)return false;WaitForSingleObject(t,INFINITE);DWORD code=3;GetExitCodeThread(t,&code);CloseHandle(t);return code==0;}
static bool safeB1(const void* p,size_t n){MEMORY_BASIC_INFORMATION info{};return p&&VirtualQuery(p,&info,sizeof(info))&&info.State==MEM_COMMIT&&!(info.Protect&(PAGE_NOACCESS|PAGE_GUARD))&&(uintptr_t)p+n<=(uintptr_t)info.BaseAddress+info.RegionSize;}
static void dumpB1(const char* key,const float* p,int count){printf(",\"%s\":",key);if(count<0||count>8192||!safeB1(p,count*4)){printf("null");return;}printf("%s",array(p,count).c_str());}
static LONG CALLBACK ObserveB1(EXCEPTION_POINTERS* e){
 auto c=e->ContextRecord;if(e->ExceptionRecord->ExceptionCode!=EXCEPTION_SINGLE_STEP)return EXCEPTION_CONTINUE_SEARCH;int slot=-1;uintptr_t current[]={c->Dr0,c->Dr1,c->Dr2,c->Dr3};for(int i=0;i<4;i++)if(c->Rip==current[i])slot=i;if(slot<0)return EXCEPTION_CONTINUE_SEARCH;
 if(slot==0){if(!b1residualMode){int n=(int)c->R8;auto options=*(int**)(c->Rsp+0x38);printf("{\"phase\":\"b1-solve-entry\",\"case\":\"%s\",\"count\":%d,\"callerRva\":%llu,\"options\":[%d,%d,%d,%d,%d]",b1case,n,*(uintptr_t*)c->Rsp-b1core,options[0],options[1],options[2],options[3],options[4]);dumpB1("object",(float*)c->Rcx,n*2);dumpB1("target",(float*)c->Rdx,n*2);printf("}\n");c->Dr0=b1core+0x7fec40;b1residualMode=true;}
 else if(!b1residualReturn){b1residualSeq++;b1residualCount=(int)c->R8;b1errors=*(float**)(c->Rsp+0x38);b1sigma=*(float**)(c->Rsp+0x40);printf("{\"phase\":\"b1-residual-entry\",\"case\":\"%s\",\"sequence\":%d,\"count\":%d",b1case,b1residualSeq,b1residualCount);dumpB1("coefficients",(float*)c->R9,8);printf("}\n");c->Dr0=*(uintptr_t*)c->Rsp;b1residualReturn=true;}
 else{printf("{\"phase\":\"b1-residual-return\",\"case\":\"%s\",\"sequence\":%d",b1case,b1residualSeq);dumpB1("errors",b1errors,b1residualCount);dumpB1("sigma",b1sigma,1);printf("}\n");c->Dr0=b1core+0x7fec40;b1residualReturn=false;}}
 if(slot==1){int n=(int)c->R8;printf("{\"phase\":\"b1-normalized\",\"case\":\"%s\",\"count\":%d,\"model\":%d",b1case,n,(int)c->R9);dumpB1("object",(float*)c->Rcx,n*2);dumpB1("target",(float*)c->Rdx,n*2);printf("}\n");}
 if(slot==2){if(!b1return2){int rows=(int)c->R8;int dim=*(int*)(c->Rsp+0x28);b1sequence++;b1solution=(float*)c->R9;b1dimension=dim;printf("{\"phase\":\"b1-linear-entry\",\"case\":\"%s\",\"sequence\":%d,\"rows\":%d,\"dimension\":%d,\"callerRva\":%llu",b1case,b1sequence,rows,dim,*(uintptr_t*)c->Rsp-b1core);dumpB1("a",(float*)c->Rcx,rows*dim);dumpB1("b",(float*)c->Rdx,rows);printf("}\n");c->Dr2=*(uintptr_t*)c->Rsp;b1return2=true;}
 else{printf("{\"phase\":\"b1-linear-return\",\"case\":\"%s\",\"sequence\":%d,\"rax\":%llu",b1case,b1sequence,c->Rax);dumpB1("solution",b1solution,b1dimension);printf("}\n");c->Dr2=b1addresses[2];b1return2=false;}}
 if(slot==3){if(!b1return3){b1refine++;b1refineOutput=(float*)c->R9;auto state=(uintptr_t*)c->Rcx;int n=(int)c->Rdx;printf("{\"phase\":\"b1-refine-entry\",\"case\":\"%s\",\"sequence\":%d,\"count\":%d",b1case,b1refine,n);dumpB1("options",(float*)c->R8,6);dumpB1("matrix",b1refineOutput,9);dumpB1("sourceNormalize",(float*)state[6],9);dumpB1("targetNormalize",(float*)state[7],9);printf("}\n");c->Dr3=*(uintptr_t*)c->Rsp;b1return3=true;}
 else{printf("{\"phase\":\"b1-refine-return\",\"case\":\"%s\",\"sequence\":%d,\"rax\":%llu",b1case,b1refine,c->Rax);dumpB1("matrix",b1refineOutput,9);printf("}\n");c->Dr3=b1addresses[3];b1return3=false;}}
 c->Dr6=0;c->EFlags|=0x10000;return EXCEPTION_CONTINUE_EXECUTION;
}
int main(int argc,char**argv){
 SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX|SEM_NOOPENFILEERRORBOX);setvbuf(stdout,nullptr,_IONBF,0);if(argc!=3)return 2;
 auto apps=std::filesystem::path(argv[1]).parent_path().parent_path().parent_path().parent_path().parent_path();for(auto name:{"HSlog.dll","ModuleFrame.dll"})LoadLibraryExA((apps/"ModuleProxy"/"x64"/name).string().c_str(),nullptr,LOAD_WITH_ALTERED_SEARCH_PATH);
 auto module=LoadLibraryExA(argv[1],nullptr,LOAD_WITH_ALTERED_SEARCH_PATH);if(!module)return 3;FILE*f=nullptr;fopen_s(&f,argv[2],"r");if(!f)return 4;
 try{install(module);char name[128];int count;while(fscanf_s(f,"%127s%d",name,(unsigned)sizeof(name),&count)==2){
  Instance m(module);m.param("HomoFreedom","1");m.param("WeightFun","3");m.param("WeightFactor","20");for(int i=1;i<=8;i++){std::string key="Data"+std::to_string(i);m.param(key.c_str(),(key+(i<=2?"#1":"#0")).c_str());}
  inputs.clear();for(int i=0;i<count;i++){float x,y,u,v;if(fscanf_s(f,"%f%f%f%f",&x,&y,&u,&v)!=4)throw 20;std::string g=i<count/2?"1":"2";inputs["ObjectPointX"+g].push_back(x);inputs["ObjectPointY"+g].push_back(y);inputs["TargetPointX"+g].push_back(u);inputs["TargetPointY"+g].push_back(v);}
  b1case=name;b1residualMode=false;b1residualReturn=false;b1residualSeq=0;b1sequence=0;b1refine=0;b1stop=false;b1core=(uintptr_t)GetModuleHandleA("MVD_Algorithm.dll");uintptr_t rvas[]={0x7ff3c0,0x800170,0x7075e0,0x7fd4e0};for(int i=0;i<4;i++)b1addresses[i]=b1core+rvas[i];
  auto handler=AddVectoredExceptionHandler(1,ObserveB1);b1thread=OpenThread(THREAD_SUSPEND_RESUME|THREAD_GET_CONTEXT|THREAD_SET_CONTEXT,FALSE,GetCurrentThreadId());if(!b1thread||!configureB1())return 6;m.run(name);b1stop=true;configureB1();CloseHandle(b1thread);RemoveVectoredExceptionHandler(handler);
 }
 fclose(f);restore();TerminateProcess(GetCurrentProcess(),0);return 0;
 }catch(...){restore();return 5;}
}
