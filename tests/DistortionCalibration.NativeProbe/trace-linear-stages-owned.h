static unsigned char* hook=nullptr,*ret=nullptr;static unsigned char original,retOriginal;static bool stepping=false;
static float* output=nullptr;static int sequence=-1,stage,columns;static FILE* logFile=nullptr;
static LONG CALLBACK Capture(EXCEPTION_POINTERS*e){auto c=e->ContextRecord;
 if(e->ExceptionRecord->ExceptionCode==EXCEPTION_BREAKPOINT&&e->ExceptionRecord->ExceptionAddress==hook){
  if(sequence!=g_pendingSequence){sequence=g_pendingSequence;stage=0;} columns=*(int*)(c->Rsp+0x28);int rows=(int)c->R8;output=(float*)c->R9;
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
static void InstallStageTrace(){if(hook)return;auto core=GetModuleHandleA("MVD_Algorithm.dll");if(!core)return;logFile=fopen("stages.jsonl","wb");if(!logFile)return;hook=(unsigned char*)core+0x8580e0;original=*hook;DWORD protection;VirtualProtect(hook,1,PAGE_EXECUTE_READWRITE,&protection);AddVectoredExceptionHandler(1,Capture);*hook=0xcc;FlushInstructionCache(GetCurrentProcess(),hook,1);}