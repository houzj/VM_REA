static int g_board=0,g_model=0,g_sequence=0,g_count=0,g_direction=0,g_beforeCount=0,g_beforeWidth=0,g_beforeHeight=0,g_beforeTargetCount=0;
static FILE* g_trace=nullptr;static BYTE *g_hook=nullptr,*g_return=nullptr,g_original=0,g_returnOriginal=0;static bool g_step=false;
static int *g_used=nullptr,*g_state=nullptr,*g_dimensions=nullptr,*g_targets=nullptr,*g_targetCount=nullptr,*g_indices=nullptr;static float* g_scores=nullptr;
static void Patch(BYTE* address,BYTE value){DWORD old;VirtualProtect(address,1,PAGE_EXECUTE_READWRITE,&old);*address=value;FlushInstructionCache(GetCurrentProcess(),address,1);}
static void Dump(const char* kind,void* values,int bytes){char path[100];sprintf_s(path,"expansion-%03d-%s.bin",g_sequence,kind);FILE* file=fopen(path,"wb");if(file){fwrite(values,1,bytes,file);fclose(file);}}
static void Snap(const char* phase){char name[100];sprintf_s(name,"used-%s",phase);Dump(name,g_used,90000*4);sprintf_s(name,"indices-%s",phase);Dump(name,g_indices,90000*4);sprintf_s(name,"scores-%s",phase);Dump(name,g_scores,270000*4);}
static LONG CALLBACK TraceExpansion(EXCEPTION_POINTERS* error){auto c=error->ContextRecord;auto where=error->ExceptionRecord->ExceptionAddress;
 if(error->ExceptionRecord->ExceptionCode==EXCEPTION_BREAKPOINT){
  if(where==g_hook){g_sequence++;g_count=(int)c->Rdx;g_used=(int*)c->R8;g_state=(int*)c->R9;g_indices=*(int**)((BYTE*)g_state+8);g_scores=*(float**)((BYTE*)g_state+16);g_dimensions=*(int**)(c->Rsp+0x28);g_targets=*(int**)(c->Rsp+0x30);g_targetCount=*(int**)(c->Rsp+0x38);g_direction=*(int*)(c->Rsp+0x40);g_beforeCount=g_state[0];g_beforeWidth=g_dimensions[0];g_beforeHeight=g_dimensions[1];g_beforeTargetCount=*g_targetCount;
   if(g_count<1||g_count>100000)TerminateProcess(GetCurrentProcess(),20);Dump("candidates",(void*)c->Rcx,g_count*28);Dump("alignment-entry",g_indices-16,16*4);Snap("entry");
   g_return=*(BYTE**)c->Rsp;g_returnOriginal=*g_return;Patch(g_return,0xcc);Patch(g_hook,g_original);c->Rip=(DWORD64)g_hook;c->EFlags|=0x100;g_step=true;return EXCEPTION_CONTINUE_EXECUTION;}
  if(g_return&&where==g_return){Snap("return");if(*g_targetCount<0||*g_targetCount>100000)TerminateProcess(GetCurrentProcess(),21);Dump("targets-return",g_targets,*g_targetCount*8);fprintf(g_trace,"{\"sequence\":%d,\"board\":%d,\"candidateCount\":%d,\"direction\":%d,\"beforeCount\":%d,\"beforeDimensions\":[%d,%d],\"beforeTargetCount\":%d,\"status\":%u,\"count\":%d,\"dimensions\":[%d,%d],\"targetCount\":%d}\n",g_sequence,g_board,g_count,g_direction,g_beforeCount,g_beforeWidth,g_beforeHeight,g_beforeTargetCount,(unsigned)c->Rax,g_state[0],g_dimensions[0],g_dimensions[1],*g_targetCount);fflush(g_trace);Patch(g_return,g_returnOriginal);c->Rip=(DWORD64)g_return;g_return=nullptr;c->EFlags|=0x100;g_step=true;return EXCEPTION_CONTINUE_EXECUTION;}
 }
 if(error->ExceptionRecord->ExceptionCode==EXCEPTION_SINGLE_STEP&&g_step){if(g_sequence<64)Patch(g_hook,0xcc);c->EFlags&=~0x100;g_step=false;return EXCEPTION_CONTINUE_EXECUTION;}
 return EXCEPTION_CONTINUE_SEARCH;
}
static void InstallFitTrace(){if(g_hook)return;g_trace=fopen("expansion-trace.jsonl","wb");auto core=GetModuleHandleA("MVD_Algorithm.dll");if(!g_trace||!core)return;g_hook=(BYTE*)core+0x8bdef0;g_original=*g_hook;AddVectoredExceptionHandler(1,TraceExpansion);Patch(g_hook,0xcc);}
