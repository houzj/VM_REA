static unsigned char* medianHook=nullptr,*medianReturn=nullptr;static unsigned char medianOriginal,medianReturnOriginal;static bool medianStep=false;
static int medianSequence=-1,medianStage=0,medianCount=0;static float* medianSigma=nullptr,*medianErrors=nullptr,*medianSorted=nullptr;static FILE* medianLog=nullptr;
static LONG CALLBACK CaptureMedian(EXCEPTION_POINTERS*e){auto c=e->ContextRecord;
 if(e->ExceptionRecord->ExceptionCode==EXCEPTION_BREAKPOINT&&e->ExceptionRecord->ExceptionAddress==medianHook){
  if(medianSequence!=CurrentFitSequence()){medianSequence=CurrentFitSequence();medianStage=0;}medianCount=(int)c->R8;
  medianSorted=*(float**)(c->Rsp+0x30);medianErrors=*(float**)(c->Rsp+0x38);medianSigma=*(float**)(c->Rsp+0x40);
  char snapshotName[128];sprintf_s(snapshotName,"median-entry-%03d-%02d.bin",medianSequence,medianStage);FILE*snapshot=fopen(snapshotName,"wb");if(snapshot){int freedom=*(int*)(c->Rsp+0x28),prefixCount=medianCount*2+32;fwrite(&medianCount,4,1,snapshot);fwrite(&freedom,4,1,snapshot);fwrite(&prefixCount,4,1,snapshot);fwrite((void*)c->R9,4,8,snapshot);fwrite((void*)c->Rcx,8,medianCount,snapshot);fwrite((void*)c->Rdx,8,medianCount,snapshot);fwrite(medianSorted-prefixCount,4,prefixCount,snapshot);fwrite(medianSorted,4,medianCount,snapshot);fclose(snapshot);}  fprintf(medianLog,"{\"phase\":\"median-input\",\"sequence\":%d,\"stage\":%d,\"sentinelBits\":%u,\"count\":%d}\n",medianSequence,medianStage,((unsigned*)medianSorted)[-1],medianCount);fflush(medianLog);
  medianReturn=*(unsigned char**)c->Rsp;medianReturnOriginal=*medianReturn;DWORD protection;VirtualProtect(medianReturn,1,PAGE_EXECUTE_READWRITE,&protection);*medianReturn=0xcc;FlushInstructionCache(GetCurrentProcess(),medianReturn,1);
  *medianHook=medianOriginal;FlushInstructionCache(GetCurrentProcess(),medianHook,1);c->Rip=(DWORD64)medianHook;c->EFlags|=0x100;medianStep=true;return EXCEPTION_CONTINUE_EXECUTION;
 }
 if(e->ExceptionRecord->ExceptionCode==EXCEPTION_BREAKPOINT&&medianReturn&&e->ExceptionRecord->ExceptionAddress==medianReturn){
  char name[128];sprintf_s(name,"median-%03d-%02d.bin",medianSequence,medianStage);FILE*f=fopen(name,"wb");if(f){fwrite(medianErrors,4,medianCount,f);fwrite(medianSorted,4,medianCount,f);fclose(f);}
  fprintf(medianLog,"{\"phase\":\"median-output\",\"sequence\":%d,\"stage\":%d,\"sigma\":%.9g}\n",medianSequence,medianStage++,*medianSigma);fflush(medianLog);
  *medianReturn=medianReturnOriginal;FlushInstructionCache(GetCurrentProcess(),medianReturn,1);c->Rip=(DWORD64)medianReturn;medianReturn=nullptr;c->EFlags|=0x100;medianStep=true;return EXCEPTION_CONTINUE_EXECUTION;
 }
 if(e->ExceptionRecord->ExceptionCode==EXCEPTION_SINGLE_STEP&&medianStep){*medianHook=0xcc;FlushInstructionCache(GetCurrentProcess(),medianHook,1);c->EFlags&=~0x100;medianStep=false;return EXCEPTION_CONTINUE_EXECUTION;}return EXCEPTION_CONTINUE_SEARCH;
}
static void InstallMedianTrace(){if(medianHook)return;auto core=GetModuleHandleA("MVD_Algorithm.dll");if(!core)return;medianLog=fopen("median.jsonl","wb");if(!medianLog)return;medianHook=(unsigned char*)core+0x7fec40;medianOriginal=*medianHook;DWORD protection;VirtualProtect(medianHook,1,PAGE_EXECUTE_READWRITE,&protection);AddVectoredExceptionHandler(1,CaptureMedian);*medianHook=0xcc;FlushInstructionCache(GetCurrentProcess(),medianHook,1);}
