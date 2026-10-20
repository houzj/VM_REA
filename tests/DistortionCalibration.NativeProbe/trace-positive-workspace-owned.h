// Controlled observation of the two addresses from retained seq56/67 fixtures.
// Future offsets are probe hypotheses, checked against actual median addresses.
static unsigned char *pwCore=nullptr,*pwEntry=nullptr,*pwRearm=nullptr;
static unsigned char pwOriginal=0;static FILE*pwLog=nullptr;
static unsigned char*pwTargets[2]={};static unsigned pwBits[2]={};static unsigned pwEvents=0;
static LONG CALLBACK PositiveWriter(EXCEPTION_POINTERS*e){auto c=e->ContextRecord;
 if(e->ExceptionRecord->ExceptionCode==EXCEPTION_SINGLE_STEP){
  bool ours=false;for(int i=0;i<2;i++)if((c->Dr6&(1<<i))&&pwTargets[i]){ours=true;unsigned bits=*(unsigned*)pwTargets[i];
   if(bits!=pwBits[i]&&pwEvents++<12000){fprintf(pwLog,"{\"phase\":\"write\",\"slot\":%d,\"sequence\":%d,\"board\":%d,\"model\":%d,\"warp\":%d,\"address\":%llu,\"nextRva\":%llu,\"previousBits\":%u,\"bits\":%u,\"rax\":%llu,\"rbx\":%llu,\"rcx\":%llu,\"rdx\":%llu,\"rsi\":%llu,\"rdi\":%llu,\"r8\":%llu,\"r9\":%llu,\"r10\":%llu,\"xmm0\":%u,\"xmm1\":%u,\"xmm4\":%u}\n",i,g_pendingSequence,g_board,g_model,g_warp,(unsigned long long)pwTargets[i],c->Rip>=(DWORD64)pwCore&&c->Rip<(DWORD64)pwCore+0x3000000?c->Rip-(DWORD64)pwCore:0,pwBits[i],bits,c->Rax,c->Rbx,c->Rcx,c->Rdx,c->Rsi,c->Rdi,c->R8,c->R9,c->R10,(unsigned)c->Xmm0.Low,(unsigned)c->Xmm1.Low,(unsigned)c->Xmm4.Low);fflush(pwLog);}pwBits[i]=bits;}
  if(ours)c->Dr6=0;
  if(pwRearm){*pwRearm=0xcc;FlushInstructionCache(GetCurrentProcess(),pwRearm,1);pwRearm=nullptr;c->EFlags&=~0x100;return EXCEPTION_CONTINUE_EXECUTION;}
  if(ours)return EXCEPTION_CONTINUE_EXECUTION;
 }
 if(e->ExceptionRecord->ExceptionCode==EXCEPTION_BREAKPOINT&&e->ExceptionRecord->ExceptionAddress==pwEntry){
  auto outer=*(unsigned char**)(c->Rsp+0x68);auto cap=*(unsigned long long*)(c->Rsp+0x28);
  size_t offsets[2]={13436,34428};
  for(int i=0;i<2;i++){pwTargets[i]=outer+offsets[i];pwBits[i]=*(unsigned*)pwTargets[i];}
  c->Dr0=(DWORD64)pwTargets[0];c->Dr1=(DWORD64)pwTargets[1];c->Dr6=0;c->Dr7=0xdd0005;
  fprintf(pwLog,"{\"phase\":\"main-entry\",\"board\":%d,\"model\":%d,\"warp\":%d,\"outer\":%llu,\"capacityX\":%u,\"capacityY\":%u,\"slot0Address\":%llu,\"slot0Bits\":%u,\"slot1Address\":%llu,\"slot1Bits\":%u}\n",g_board,g_model,g_warp,(unsigned long long)outer,(unsigned)cap,(unsigned)(cap>>32),(unsigned long long)pwTargets[0],pwBits[0],(unsigned long long)pwTargets[1],pwBits[1]);fflush(pwLog);
  *pwEntry=pwOriginal;FlushInstructionCache(GetCurrentProcess(),pwEntry,1);c->Rip=(DWORD64)pwEntry;c->EFlags|=0x100;pwRearm=pwEntry;return EXCEPTION_CONTINUE_EXECUTION;
 }
 return EXCEPTION_CONTINUE_SEARCH;
}
static void InstallPositiveTrace(){if(pwEntry)return;pwCore=(unsigned char*)GetModuleHandleA("MVD_Algorithm.dll");if(!pwCore)return;pwLog=fopen("positive-writer.jsonl","wb");if(!pwLog)return;pwEntry=pwCore+0x7ed0c0;pwOriginal=*pwEntry;DWORD old;VirtualProtect(pwEntry,1,PAGE_EXECUTE_READWRITE,&old);AddVectoredExceptionHandler(1,PositiveWriter);*pwEntry=0xcc;FlushInstructionCache(GetCurrentProcess(),pwEntry,1);}
static void PositiveLifecycle(const char* phase,int board,int model,int warp,IImageCalibTool*tool){
 if(!pwLog)return;
 unsigned actual[2]={};int readable[2]={};
 for(int i=0;i<2;i++){MEMORY_BASIC_INFORMATION m{};if(pwTargets[i]&&VirtualQuery(pwTargets[i],&m,sizeof(m))&&m.State==MEM_COMMIT&&!(m.Protect&(PAGE_NOACCESS|PAGE_GUARD))){readable[i]=1;actual[i]=*(unsigned*)pwTargets[i];}}
 fprintf(pwLog,"{\"phase\":\"%s\",\"board\":%d,\"model\":%d,\"warp\":%d,\"tool\":%llu,\"thread\":%u,\"slot0Readable\":%d,\"slot1Readable\":%d,\"slot0Bits\":%u,\"slot1Bits\":%u}\n",phase,board,model,warp,(unsigned long long)tool,GetCurrentThreadId(),readable[0],readable[1],actual[0],actual[1]);fflush(pwLog);
}
