static unsigned char* irlsHook=nullptr;static unsigned char irlsOriginal;static bool irlsStep=false;
static LONG CALLBACK CaptureIrlsEntry(EXCEPTION_POINTERS*e){auto c=e->ContextRecord;
 if(e->ExceptionRecord->ExceptionCode==EXCEPTION_BREAKPOINT&&e->ExceptionRecord->ExceptionAddress==irlsHook){
  int count=(int)c->Rdx;auto structure=(unsigned char*)c->Rcx;auto options=(int*)c->R8;auto h=(float*)c->R9;
  if(count>0&&count<4096){char name[128];sprintf_s(name,"irls-entry-%03d.bin",CurrentFitSequence());FILE*f=fopen(name,"wb");if(f){fwrite(&count,4,1,f);fwrite(options,4,6,f);fwrite(h,4,9,f);fwrite(*(void**)(structure+0x10),8,count,f);fwrite(*(void**)(structure+0x18),8,count,f);fclose(f);}}
  *irlsHook=irlsOriginal;FlushInstructionCache(GetCurrentProcess(),irlsHook,1);c->Rip=(DWORD64)irlsHook;c->EFlags|=0x100;irlsStep=true;return EXCEPTION_CONTINUE_EXECUTION;
 }
 if(e->ExceptionRecord->ExceptionCode==EXCEPTION_SINGLE_STEP&&irlsStep){*irlsHook=0xcc;FlushInstructionCache(GetCurrentProcess(),irlsHook,1);c->EFlags&=~0x100;irlsStep=false;return EXCEPTION_CONTINUE_EXECUTION;}
 return EXCEPTION_CONTINUE_SEARCH;
}
static void InstallIrlsEntryTrace(){if(irlsHook)return;auto core=GetModuleHandleA("MVD_Algorithm.dll");if(!core)return;irlsHook=(unsigned char*)core+0x7fd4e0;irlsOriginal=*irlsHook;DWORD protection;VirtualProtect(irlsHook,1,PAGE_EXECUTE_READWRITE,&protection);AddVectoredExceptionHandler(1,CaptureIrlsEntry);*irlsHook=0xcc;FlushInstructionCache(GetCurrentProcess(),irlsHook,1);}
