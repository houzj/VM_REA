#include <windows.h>
#include <cstdio>
#include <vector>
#include <cstdint>
#include <cstring>
// Independent owned-process stage capture, not compiled into any product assembly.
static unsigned char* hook=nullptr,*ret=nullptr;static unsigned char original,retOriginal;static bool stepping=false;
static float* output=nullptr;static int sequence,stage,columns;static FILE* logFile=nullptr;
static LONG CALLBACK Capture(EXCEPTION_POINTERS*e){auto c=e->ContextRecord;
 if(e->ExceptionRecord->ExceptionCode==EXCEPTION_BREAKPOINT&&e->ExceptionRecord->ExceptionAddress==hook){
  columns=*(int*)(c->Rsp+0x28);int rows=(int)c->R8;output=(float*)c->R9;
  if(rows>0&&rows<10000&&columns>0&&columns<=8){char name[128];sprintf_s(name,"seq-%03d-stage-%02d-input.bin",sequence,stage);FILE*f=fopen(name,"wb");if(f){fwrite((void*)c->Rcx,4,rows*columns,f);fwrite((void*)c->Rdx,4,rows,f);fclose(f);}fprintf(logFile,"{\"phase\":\"linear-input\",\"sequence\":%d,\"stage\":%d,\"rows\":%d,\"columns\":%d}\n",sequence,stage,rows,columns);fflush(logFile);}
  ret=*(unsigned char**)c->Rsp;retOriginal=*ret;DWORD protection;VirtualProtect(ret,1,PAGE_EXECUTE_READWRITE,&protection);*ret=0xcc;FlushInstructionCache(GetCurrentProcess(),ret,1);
  *hook=original;FlushInstructionCache(GetCurrentProcess(),hook,1);c->Rip=(DWORD64)hook;c->EFlags|=0x100;stepping=true;return EXCEPTION_CONTINUE_EXECUTION;
 }
 if(e->ExceptionRecord->ExceptionCode==EXCEPTION_BREAKPOINT&&ret&&e->ExceptionRecord->ExceptionAddress==ret){
  fprintf(logFile,"{\"phase\":\"linear-output\",\"sequence\":%d,\"stage\":%d,\"code\":%llu,\"coefficients\":[",sequence,stage,c->Rax);for(int i=0;i<columns;i++)fprintf(logFile,i?",%.9g":"%.9g",output[i]);fprintf(logFile,"]}\n");fflush(logFile);stage++;
  *ret=retOriginal;FlushInstructionCache(GetCurrentProcess(),ret,1);c->Rip=(DWORD64)ret;ret=nullptr;c->EFlags|=0x100;stepping=true;return EXCEPTION_CONTINUE_EXECUTION;
 }
 if(e->ExceptionRecord->ExceptionCode==EXCEPTION_SINGLE_STEP&&stepping){*hook=0xcc;FlushInstructionCache(GetCurrentProcess(),hook,1);c->EFlags&=~0x100;stepping=false;return EXCEPTION_CONTINUE_EXECUTION;}
 return EXCEPTION_CONTINUE_SEARCH;
}
static int CurrentFitSequence(){return sequence;}
#include "trace-irls-entry-owned.h"
int main(int argc,char**argv){if(argc!=3)return 2;SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);SetDllDirectoryA(argv[1]);auto core=LoadLibraryA("MVD_Algorithm.dll");if(!core)return 3;
 logFile=fopen("stages.jsonl","wb");if(!logFile)return 4;hook=(unsigned char*)core+0x8580e0;original=*hook;DWORD protection;VirtualProtect(hook,1,PAGE_EXECUTE_READWRITE,&protection);AddVectoredExceptionHandler(1,Capture);*hook=0xcc;InstallIrlsEntryTrace();
 using Fixed=int(*)(void*,void*,int,float*,float*,unsigned*,void*,void*);auto fit=(Fixed)((char*)core+0x7ff3c0);std::vector<unsigned char> scratch(16*1024*1024+128);void* aligned=(void*)(((uintptr_t)scratch.data()+127)&~(uintptr_t)127);
 for(int seq:{4,56,63,67,76,83}){sequence=seq;stage=0;char name[4096];sprintf_s(name,"%s/capture-%03d.bin",argv[2],seq);FILE*f=fopen(name,"rb");if(!f)return 5;fseek(f,0,SEEK_END);int length=ftell(f);rewind(f);int count=length/16;std::vector<float> pairs(length/4);fread(pairs.data(),1,length,f);fclose(f);
  int options[6]={0,1,2,20};((float*)options)[4]=.0001f;((float*)options)[5]=2;float h[9]={},error=0;unsigned flag=0;memset(aligned,0,16*1024*1024);int code=fit(pairs.data(),pairs.data()+count*2,count,h,&error,&flag,options,aligned);
  fprintf(logFile,"{\"phase\":\"fixed-output\",\"sequence\":%d,\"code\":%d,\"error\":%.9g,\"stages\":%d,\"matrix\":[",seq,code,error,stage);for(int i=0;i<9;i++)fprintf(logFile,i?",%.9g":"%.9g",h[i]);fprintf(logFile,"]}\n");fflush(logFile);
 }
 return 0;
}
