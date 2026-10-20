// Four-byte hardware write observation in this owned SDK process only.
// For checker prelude watches the address that the subsequent n=80 circle call will use.
static int g_board=0,g_model=0,g_warp=0;
static unsigned char *g_mainHook=nullptr,*g_fixedHook=nullptr,*g_rearm=nullptr,*g_core=nullptr;
static unsigned char g_mainOriginal=0,g_fixedOriginal=0;
static FILE*g_writerLog=nullptr;static float*g_watched=nullptr;static unsigned g_previousBits=0,g_changes=0;
static size_t Align128(size_t n){return (n+127)&~(size_t)127;}
static size_t PrefixOffset(int n){return 5*Align128(n*8)+Align128(n*64)+256+Align128(n*4)+Align128(n*16)-4;}
static LONG CALLBACK WorkspaceWriter(EXCEPTION_POINTERS*e){auto c=e->ContextRecord;
 if(e->ExceptionRecord->ExceptionCode==EXCEPTION_SINGLE_STEP){
  bool ours=(c->Dr6&1)!=0&&g_watched;
  if(ours){unsigned bits=*(unsigned*)g_watched;if(bits!=g_previousBits&&g_changes++<10000){
   fprintf(g_writerLog,"{\"phase\":\"write\",\"board\":%d,\"model\":%d,\"warp\":%d,\"address\":%llu,\"afterInstruction\":%llu,\"coreRva\":%llu,\"previousBits\":%u,\"bits\":%u}\n",g_board,g_model,g_warp,(unsigned long long)g_watched,c->Rip,c->Rip>=(DWORD64)g_core&&c->Rip<(DWORD64)g_core+0x3000000?c->Rip-(DWORD64)g_core:0,g_previousBits,bits);fflush(g_writerLog);
   if(g_board==1 && c->Rip==(DWORD64)g_core+0x8003e0) {
    fprintf(g_writerLog,"{\"phase\":\"writer-registers\",\"address\":%llu,\"designBase\":%llu,\"rowStrideIndex\":%llu,\"pointIndex\":%llu,\"xmm4Bits\":%u,\"bits\":%u}\n",(unsigned long long)g_watched,c->Rax,c->Rbx,c->R10,(unsigned)c->Xmm4.Low,bits);fflush(g_writerLog);
   }
  }g_previousBits=bits;c->Dr6=0;if(g_changes>=10000)c->Dr7=0;}
  if(g_rearm){*g_rearm=0xcc;FlushInstructionCache(GetCurrentProcess(),g_rearm,1);g_rearm=nullptr;c->EFlags&=~0x100;return EXCEPTION_CONTINUE_EXECUTION;}
  if(ours)return EXCEPTION_CONTINUE_EXECUTION;
 }
 if(e->ExceptionRecord->ExceptionCode==EXCEPTION_BREAKPOINT&&(e->ExceptionRecord->ExceptionAddress==g_mainHook||e->ExceptionRecord->ExceptionAddress==g_fixedHook)){
  auto hook=(unsigned char*)e->ExceptionRecord->ExceptionAddress;
  if(hook==g_mainHook){
   unsigned long long capacity=*(unsigned long long*)(c->Rsp+0x28);unsigned x=(unsigned)capacity,y=(unsigned)(capacity>>32);int count=*(int*)(c->Rsp+0x30);
   auto workspace=*(unsigned char**)(c->Rsp+0x68);
   if(x>0&&x<=300&&y>0&&y<=300&&workspace){
    auto inner=workspace+5*Align128((size_t)x*y*8);auto target=(float*)(inner+PrefixOffset(80)-(g_board==1?5*(Align128((size_t)x*y*8)-Align128(80*8)):0));MEMORY_BASIC_INFORMATION info{};
    if(VirtualQuery(target,&info,sizeof(info))&&info.State==MEM_COMMIT&&!(info.Protect&(PAGE_NOACCESS|PAGE_GUARD))){
     g_watched=target;g_previousBits=*(unsigned*)target;c->Dr0=(DWORD64)target;c->Dr6=0;c->Dr7=0xd0001;
     fprintf(g_writerLog,"{\"phase\":\"main-entry\",\"board\":%d,\"model\":%d,\"warp\":%d,\"count\":%d,\"capacityX\":%u,\"capacityY\":%u,\"workspace\":%llu,\"expectedInner\":%llu,\"watchAddress\":%llu,\"initialBits\":%u}\n",g_board,g_model,g_warp,count,x,y,(unsigned long long)workspace,(unsigned long long)inner,(unsigned long long)target,g_previousBits);fflush(g_writerLog);
    }
   }
   *hook=g_mainOriginal;
  }else{
   int count=(int)c->R8;auto inner=*(unsigned char**)(c->Rsp+0x40);
   fprintf(g_writerLog,"{\"phase\":\"fixed-entry\",\"board\":%d,\"model\":%d,\"warp\":%d,\"count\":%d,\"workspace\":%llu,\"prefix80Address\":%llu,\"prefix80Bits\":%u}\n",g_board,g_model,g_warp,count,(unsigned long long)inner,(unsigned long long)(inner+PrefixOffset(80)),*(unsigned*)(inner+PrefixOffset(80)));fflush(g_writerLog);*hook=g_fixedOriginal;
  }
  FlushInstructionCache(GetCurrentProcess(),hook,1);c->Rip=(DWORD64)hook;c->EFlags|=0x100;g_rearm=hook;return EXCEPTION_CONTINUE_EXECUTION;
 }
 return EXCEPTION_CONTINUE_SEARCH;
}
static void InstallFitTrace(){if(g_mainHook)return;g_writerLog=fopen("writer-trace.jsonl","wb");g_core=(unsigned char*)GetModuleHandleA("MVD_Algorithm.dll");if(!g_core||!g_writerLog)return;g_mainHook=g_core+0x7ed0c0;g_fixedHook=g_core+0x7ff3c0;g_mainOriginal=*g_mainHook;g_fixedOriginal=*g_fixedHook;DWORD protect;VirtualProtect(g_mainHook,1,PAGE_EXECUTE_READWRITE,&protect);VirtualProtect(g_fixedHook,1,PAGE_EXECUTE_READWRITE,&protect);AddVectoredExceptionHandler(1,WorkspaceWriter);*g_mainHook=0xcc;*g_fixedHook=0xcc;FlushInstructionCache(GetCurrentProcess(),nullptr,0);}



