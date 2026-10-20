// Owned SDK process, fixed four-point N-point fixture; records the original
// MVD_Algorithm 0x7ff3c0 entry buffers and returned candidate matrix.
#include <windows.h>
#include <cstdio>
#include "NPointCalibCpp.h"
#include "MVDCore.h"
using namespace VisionDesigner;
using namespace VisionDesigner::NPointCalib;

static unsigned char* hookAddress=nullptr,*normalizationAddress=nullptr,*returnAddress=nullptr,*steppingAddress=nullptr;
static unsigned char hookOriginal=0,normalizationOriginal=0,returnOriginal=0;
static unsigned char* solverAddress=nullptr,*solverReturnAddress=nullptr,*solverSteppingAddress=nullptr;
static unsigned char solverOriginal=0,solverReturnOriginal=0;static float* solverOutput=nullptr;static int solverDimension=0,solverSequence=0;
static unsigned char* refineAddress=nullptr,*refineReturnAddress=nullptr;static unsigned char refineOriginal=0,refineReturnOriginal=0;static float* refineOutput=nullptr;static int refineSequence=0;
static bool stepping=false;
static float* outputMatrix=nullptr;
static float* meanError=nullptr;
static int callSequence=0,pendingSequence=0;
static FILE* trace=nullptr;

static LONG CALLBACK Capture(EXCEPTION_POINTERS* e){
 auto c=e->ContextRecord;
 if(e->ExceptionRecord->ExceptionCode==EXCEPTION_BREAKPOINT&&e->ExceptionRecord->ExceptionAddress==refineAddress){
  refineSequence++;refineOutput=(float*)c->R9;refineReturnAddress=*(unsigned char**)c->Rsp;refineReturnOriginal=*refineReturnAddress;DWORD old;VirtualProtect(refineReturnAddress,1,PAGE_EXECUTE_READWRITE,&old);*refineReturnAddress=0xcc;FlushInstructionCache(GetCurrentProcess(),refineReturnAddress,1);fprintf(trace,"{\"phase\":\"irls-entry\",\"sequence\":%d,\"count\":%u,\"weight\":%u,\"matrix\":[",refineSequence,(unsigned)c->Rdx,*(unsigned*)((char*)c->R8+4));for(int i=0;i<9;i++)fprintf(trace,"%s%.9g",i?",":"",refineOutput[i]);fprintf(trace,"]}\n");fflush(trace);*refineAddress=refineOriginal;FlushInstructionCache(GetCurrentProcess(),refineAddress,1);c->Rip=(DWORD64)refineAddress;c->EFlags|=0x100;stepping=true;steppingAddress=refineAddress;return EXCEPTION_CONTINUE_EXECUTION;
 }
 if(e->ExceptionRecord->ExceptionCode==EXCEPTION_BREAKPOINT&&refineReturnAddress&&e->ExceptionRecord->ExceptionAddress==refineReturnAddress){fprintf(trace,"{\"phase\":\"irls-return\",\"sequence\":%d,\"returnRax\":%llu,\"matrix\":[",refineSequence,c->Rax);for(int i=0;i<9;i++)fprintf(trace,"%s%.9g",i?",":"",refineOutput[i]);fprintf(trace,"]}\n");fflush(trace);*refineReturnAddress=refineReturnOriginal;FlushInstructionCache(GetCurrentProcess(),refineReturnAddress,1);c->Rip=(DWORD64)refineReturnAddress;refineReturnAddress=nullptr;c->EFlags|=0x100;stepping=true;steppingAddress=refineAddress;return EXCEPTION_CONTINUE_EXECUTION;}
 if(e->ExceptionRecord->ExceptionCode==EXCEPTION_BREAKPOINT&&e->ExceptionRecord->ExceptionAddress==solverAddress){
  int rows=(int)c->R8;int dimension=*(int*)(c->Rsp+0x28);if(rows>0&&rows<=64&&dimension>0&&dimension<=64){solverDimension=dimension;solverSequence++;solverOutput=(float*)c->R9;solverReturnAddress=*(unsigned char**)c->Rsp;solverReturnOriginal=*solverReturnAddress;DWORD old;VirtualProtect(solverReturnAddress,1,PAGE_EXECUTE_READWRITE,&old);*solverReturnAddress=0xcc;FlushInstructionCache(GetCurrentProcess(),solverReturnAddress,1);auto a=(float*)c->Rcx;auto b=(float*)c->Rdx;fprintf(trace,"{\"phase\":\"normal-equation-entry\",\"sequence\":%d,\"rows\":%d,\"dimension\":%d,\"a\":[",solverSequence,rows,dimension);for(int i=0;i<rows*dimension;i++)fprintf(trace,"%s%.9g",i?",":"",a[i]);fprintf(trace,"],\"b\":[");for(int i=0;i<rows;i++)fprintf(trace,"%s%.9g",i?",":"",b[i]);fprintf(trace,"]}\n");fflush(trace);}
  *solverAddress=solverOriginal;FlushInstructionCache(GetCurrentProcess(),solverAddress,1);c->Rip=(DWORD64)solverAddress;c->EFlags|=0x100;stepping=true;steppingAddress=solverAddress;return EXCEPTION_CONTINUE_EXECUTION;
 }
 if(e->ExceptionRecord->ExceptionCode==EXCEPTION_BREAKPOINT&&solverReturnAddress&&e->ExceptionRecord->ExceptionAddress==solverReturnAddress){fprintf(trace,"{\"phase\":\"normal-equation-return\",\"sequence\":%d,\"returnRax\":%llu,\"solution\":[",solverSequence,c->Rax);for(int i=0;i<solverDimension;i++)fprintf(trace,"%s%.9g",i?",":"",solverOutput[i]);fprintf(trace,"]}\n");fflush(trace);*solverReturnAddress=solverReturnOriginal;FlushInstructionCache(GetCurrentProcess(),solverReturnAddress,1);c->Rip=(DWORD64)solverReturnAddress;solverReturnAddress=nullptr;c->EFlags|=0x100;stepping=true;steppingAddress=solverAddress;return EXCEPTION_CONTINUE_EXECUTION;}
 if(e->ExceptionRecord->ExceptionCode==EXCEPTION_BREAKPOINT&&e->ExceptionRecord->ExceptionAddress==normalizationAddress){
  int count=(int)c->R8;if(count>0&&count<=64){auto image=(float*)c->Rcx;auto world=(float*)c->Rdx;fprintf(trace,"{\"phase\":\"normalized\",\"count\":%d,\"image\":[",count);for(int i=0;i<count;i++)fprintf(trace,"%s[%.9g,%.9g]",i?",":"",image[2*i],image[2*i+1]);fprintf(trace,"],\"world\":[");for(int i=0;i<count;i++)fprintf(trace,"%s[%.9g,%.9g]",i?",":"",world[2*i],world[2*i+1]);fprintf(trace,"]}\n");fflush(trace);}
  *normalizationAddress=normalizationOriginal;FlushInstructionCache(GetCurrentProcess(),normalizationAddress,1);c->Rip=(DWORD64)normalizationAddress;c->EFlags|=0x100;stepping=true;steppingAddress=normalizationAddress;return EXCEPTION_CONTINUE_EXECUTION;
 }
 if(e->ExceptionRecord->ExceptionCode==EXCEPTION_BREAKPOINT&&e->ExceptionRecord->ExceptionAddress==hookAddress){
  int count=(int)c->R8;
  if(count>0&&count<=64){
   pendingSequence=callSequence++; outputMatrix=(float*)c->R9;
   meanError=*(float**)(c->Rsp+0x28);
   returnAddress=*(unsigned char**)c->Rsp; returnOriginal=*returnAddress;
   DWORD old;VirtualProtect(returnAddress,1,PAGE_EXECUTE_READWRITE,&old);*returnAddress=0xcc;FlushInstructionCache(GetCurrentProcess(),returnAddress,1);
   auto options=*(int**)(c->Rsp+0x38);
   fprintf(trace,"{\"phase\":\"entry\",\"sequence\":%d,\"count\":%d,\"options\":[%d,%d,%d,%d],\"image\":[",pendingSequence,count,options[0],options[1],options[2],options[3]);
   auto image=(float*)c->Rcx;auto world=(float*)c->Rdx;
   for(int i=0;i<count;i++)fprintf(trace,"%s[%.9g,%.9g]",i?",":"",image[2*i],image[2*i+1]);
   fprintf(trace,"],\"world\":[");for(int i=0;i<count;i++)fprintf(trace,"%s[%.9g,%.9g]",i?",":"",world[2*i],world[2*i+1]);fprintf(trace,"]}\n");fflush(trace);
  }
  *hookAddress=hookOriginal;FlushInstructionCache(GetCurrentProcess(),hookAddress,1);c->Rip=(DWORD64)hookAddress;c->EFlags|=0x100;stepping=true;steppingAddress=hookAddress;return EXCEPTION_CONTINUE_EXECUTION;
 }
 if(e->ExceptionRecord->ExceptionCode==EXCEPTION_BREAKPOINT&&returnAddress&&e->ExceptionRecord->ExceptionAddress==returnAddress){
  fprintf(trace,"{\"phase\":\"return\",\"sequence\":%d,\"mean\":%.9g,\"matrix\":[",pendingSequence,*meanError);
  for(int i=0;i<9;i++)fprintf(trace,"%s%.9g",i?",":"",outputMatrix[i]);fprintf(trace,"]}\n");fflush(trace);
  *returnAddress=returnOriginal;FlushInstructionCache(GetCurrentProcess(),returnAddress,1);c->Rip=(DWORD64)returnAddress;returnAddress=nullptr;c->EFlags|=0x100;stepping=true;return EXCEPTION_CONTINUE_EXECUTION;
 }
 if(e->ExceptionRecord->ExceptionCode==EXCEPTION_SINGLE_STEP&&stepping){*steppingAddress=0xcc;FlushInstructionCache(GetCurrentProcess(),steppingAddress,1);c->EFlags&=~0x100;stepping=false;steppingAddress=nullptr;return EXCEPTION_CONTINUE_EXECUTION;}
 return EXCEPTION_CONTINUE_SEARCH;
}

int main(int argc,char**argv){
 if(argc!=2)return 2;SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);SetDllDirectoryA(argv[1]);
 auto sdk=LoadLibraryA("MVDNPointCalibCpp.dll");if(!sdk)return 3;
 auto create=(int(__stdcall*)(INPointCalibTool**))GetProcAddress(sdk,"CreateNPointCalibToolInstance");if(!create)return 4;
 INPointCalibTool* tool=nullptr;if(create(&tool))return 5;
 auto core=(unsigned char*)GetModuleHandleA("MVD_Algorithm.dll");if(!core)return 6;
 trace=fopen("fit-trace.jsonl","wb");if(!trace)return 7;
 hookAddress=core+0x7ff3c0;hookOriginal=*hookAddress;normalizationAddress=core+0x800170;normalizationOriginal=*normalizationAddress;solverAddress=core+0x7075e0;solverOriginal=*solverAddress;refineAddress=core+0x7fd4e0;refineOriginal=*refineAddress;DWORD old;VirtualProtect(hookAddress,1,PAGE_EXECUTE_READWRITE,&old);VirtualProtect(normalizationAddress,1,PAGE_EXECUTE_READWRITE,&old);VirtualProtect(solverAddress,1,PAGE_EXECUTE_READWRITE,&old);VirtualProtect(refineAddress,1,PAGE_EXECUTE_READWRITE,&old);AddVectoredExceptionHandler(1,Capture);*hookAddress=0xcc;*normalizationAddress=0xcc;*solverAddress=0xcc;*refineAddress=0xcc;FlushInstructionCache(GetCurrentProcess(),nullptr,0);
 auto basic=tool->GetBasicParam();basic->SetCameraMode((MVD_CAMERA_MODE)1);basic->SetOriginPoint({0,0});basic->AddOffsetPoint({0,0},{10,-5});basic->AddOffsetPoint({10,0},{30,-5});basic->AddOffsetPoint({20,0},{50,-5});basic->AddOffsetPoint({0,10},{10,15});
 tool->SetRunParam("HomoFreedom","1");tool->SetRunParam("WeightFun","1");tool->SetRunParam("WeightFactor","20");tool->SetRunParam("DistThreshold","30");tool->SetRunParam("SampleRatio","60");tool->Run();
 auto p=tool->GetResult()->GetPlatformCalibInfo();float matrix[9];p->GetHomoMatrix(matrix);printf("{\"status\":%d,\"matrix\":[",p->GetStatus());for(int i=0;i<9;i++)printf("%s%.9g",i?",":"",matrix[i]);printf("]}\n");fflush(stdout);
 TerminateProcess(GetCurrentProcess(),0);return 0;
}
