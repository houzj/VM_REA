// B2 observes the original MatrixToParam core before and after normalization.
// Hardware execute breakpoints affect only this owned thread; original bytes stay unchanged.
#include <windows.h>
#include <cstdio>
#include <cstdlib>
#include <cstdint>
#include <cstring>
#include "MatrixToParamCpp.h"
#include "MVDCore.h"
using namespace VisionDesigner;using namespace VisionDesigner::MatrixToParam;
static bool determinantStage=false;static uintptr_t coreBase=0;static uintptr_t addresses[4]{};static HANDLE threadHandle=nullptr;static bool stop=false;static const char* caseName="";
static DWORD WINAPI Configure(void*){if(SuspendThread(threadHandle)==(DWORD)-1)return 1;CONTEXT c{};c.ContextFlags=CONTEXT_DEBUG_REGISTERS;bool ok=GetThreadContext(threadHandle,&c)!=0;if(ok){c.Dr0=stop?0:addresses[0];c.Dr1=stop?0:addresses[1];c.Dr2=stop?0:addresses[2];c.Dr3=stop?0:addresses[3];c.Dr6=0;c.Dr7=stop?0:0x55;ok=SetThreadContext(threadHandle,&c)!=0;}ResumeThread(threadHandle);return ok?0:2;}
static bool configure(){auto t=CreateThread(nullptr,0,Configure,nullptr,0,nullptr);if(!t)return false;WaitForSingleObject(t,INFINITE);DWORD code=3;GetExitCodeThread(t,&code);CloseHandle(t);return code==0;}
static void array(const float*p,int n){printf("[");for(int i=0;i<n;i++)printf("%s%.17g",i?",":"",p[i]);printf("]");}
static float low(const M128A& value){float f;memcpy(&f,&value,4);return f;}
static LONG CALLBACK Observe(EXCEPTION_POINTERS* e){auto c=e->ContextRecord;if(e->ExceptionRecord->ExceptionCode!=EXCEPTION_SINGLE_STEP)return EXCEPTION_CONTINUE_SEARCH;int slot=-1;for(int i=0;i<4;i++)if(c->Rip==addresses[i])slot=i;if(slot<0)return EXCEPTION_CONTINUE_SEARCH;
 if(slot==0&&!determinantStage){printf("{\"phase\":\"matrix-core-entry\",\"case\":\"%s\",\"matrix\":",caseName);array((float*)c->Rcx,9);printf("}\n");determinantStage=true;addresses[0]=coreBase+0x800c2e;c->Dr0=addresses[0];}else if(slot==0){printf("{\"phase\":\"determinant\",\"case\":\"%s\",\"value\":%.17g}\n",caseName,low(c->Xmm0));}
 if(slot==1){printf("{\"phase\":\"inverse-before-normalize\",\"case\":\"%s\",\"inverse\":",caseName);array((float*)(c->Rsp+0xe0),9);printf(",\"reciprocal\":%.17g}\n",low(c->Xmm6));}
 if(slot==2){printf("{\"phase\":\"inverse-after-normalize\",\"case\":\"%s\",\"firstEight\":",caseName);array((float*)(c->Rsp+0xe0),8);printf("}\n");}
 if(slot==3){printf("{\"phase\":\"first-atan2-call\",\"case\":\"%s\",\"y\":%.17g,\"x\":%.17g}\n",caseName,low(c->Xmm0),low(c->Xmm1));}
 c->Dr6=0;c->EFlags|=0x10000;return EXCEPTION_CONTINUE_EXECUTION;
}
int main(int argc,char**argv){SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX|SEM_NOOPENFILEERRORBOX);setvbuf(stdout,nullptr,_IONBF,0);if(argc!=12)return 2;caseName=argv[2];auto dll=LoadLibraryA(argv[1]);if(!dll)return 3;auto create=(int(__stdcall*)(IMatrixToParamTool**))GetProcAddress(dll,"CreateMatrixToParamToolInstance");IMatrixToParamTool*tool=nullptr;if(!create||create(&tool))return 4;auto core=(uintptr_t)GetModuleHandleA("MVD_Algorithm.dll");coreBase=core;uintptr_t rvas[]={0x800af0,0x800e0b,0x800e6d,0x800e91};for(int i=0;i<4;i++)addresses[i]=core+rvas[i];float matrix[9];for(int i=0;i<9;i++)matrix[i]=(float)std::strtod(argv[i+3],nullptr);
 auto handler=AddVectoredExceptionHandler(1,Observe);threadHandle=OpenThread(THREAD_SUSPEND_RESUME|THREAD_GET_CONTEXT|THREAD_SET_CONTEXT,FALSE,GetCurrentThreadId());if(!threadHandle||!configure())return 5;
 try{tool->ImportHomoMatrix(matrix);tool->Run();auto*r=tool->GetResult();printf("{\"phase\":\"matrix-result\",\"case\":\"%s\",\"scale\":%.17g,\"offsetX\":%.17g,\"offsetY\":%.17g,\"rotate\":%.17g,\"aspect\":%.17g,\"skew\":%.17g}\n",caseName,r->GetScale(),r->GetOffsetX(),r->GetOffsetY(),r->GetRotate(),r->GetAspect(),r->GetSkew());}
 catch(const IMVDException&e){printf("{\"phase\":\"exception\",\"case\":\"%s\",\"code\":%u}\n",caseName,(unsigned)e.GetErrorCode());}
 stop=true;configure();CloseHandle(threadHandle);RemoveVectoredExceptionHandler(handler);TerminateProcess(GetCurrentProcess(),0);return 0;}
