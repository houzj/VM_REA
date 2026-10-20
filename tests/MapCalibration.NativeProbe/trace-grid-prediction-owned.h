static int g_board=0,g_model=0,g_sequence=0,g_count=0,g_lengths[3]={};
static FILE* g_trace=nullptr;
static BYTE *g_hook=nullptr,*g_return=nullptr,g_original=0,g_returnOriginal=0;
static int *g_used=nullptr,*g_assigned=nullptr,*g_assignedCount=nullptr;
static float* g_predictions=nullptr;
static bool g_step=false;
static void Patch(BYTE* address,BYTE value){DWORD old;VirtualProtect(address,1,PAGE_EXECUTE_READWRITE,&old);*address=value;FlushInstructionCache(GetCurrentProcess(),address,1);}
static void Dump(const char* kind,void* values,int bytes){char path[100];sprintf_s(path,"prediction-%03d-%s.bin",g_sequence,kind);FILE* file=fopen(path,"wb");if(file){fwrite(values,1,bytes,file);fclose(file);}}
static LONG CALLBACK TracePrediction(EXCEPTION_POINTERS* error){auto c=error->ContextRecord;auto where=error->ExceptionRecord->ExceptionAddress;
 if(error->ExceptionRecord->ExceptionCode==EXCEPTION_BREAKPOINT){
  if(where==g_hook){g_sequence++;g_count=(int)c->Rdx;auto lengths=*(int**)(c->Rsp+0x38);for(int i=0;i<3;i++)g_lengths[i]=lengths[i];if(g_count<1||g_count>100000||g_lengths[2]<0||g_lengths[2]>300)TerminateProcess(GetCurrentProcess(),20);
   g_used=(int*)c->R8;g_assigned=*(int**)(c->Rsp+0x40);g_assignedCount=*(int**)(c->Rsp+0x48);g_predictions=*(float**)(c->Rsp+0x50);
   Dump("candidates",(void*)c->Rcx,g_count*28);Dump("used-entry",g_used,g_count*4);Dump("first",(void*)c->R9,g_lengths[2]*4);Dump("second",*(void**)(c->Rsp+0x28),g_lengths[2]*4);Dump("third",*(void**)(c->Rsp+0x30),g_lengths[2]*4);Dump("assigned-entry",g_assigned,g_lengths[2]*4);
   g_return=*(BYTE**)c->Rsp;g_returnOriginal=*g_return;Patch(g_return,0xcc);Patch(g_hook,g_original);c->Rip=(DWORD64)g_hook;c->EFlags|=0x100;g_step=true;return EXCEPTION_CONTINUE_EXECUTION;}
  if(g_return&&where==g_return){Dump("used-return",g_used,g_count*4);Dump("assigned-return",g_assigned,g_lengths[2]*4);Dump("points-return",g_predictions,g_lengths[2]*8);fprintf(g_trace,"{\"sequence\":%d,\"board\":%d,\"candidateCount\":%d,\"lengths\":[%d,%d,%d],\"assignedCount\":%d}\n",g_sequence,g_board,g_count,g_lengths[0],g_lengths[1],g_lengths[2],*g_assignedCount);fflush(g_trace);Patch(g_return,g_returnOriginal);c->Rip=(DWORD64)g_return;g_return=nullptr;c->EFlags|=0x100;g_step=true;return EXCEPTION_CONTINUE_EXECUTION;}
 }
 if(error->ExceptionRecord->ExceptionCode==EXCEPTION_SINGLE_STEP&&g_step){if(g_sequence<64)Patch(g_hook,0xcc);c->EFlags&=~0x100;g_step=false;return EXCEPTION_CONTINUE_EXECUTION;}
 return EXCEPTION_CONTINUE_SEARCH;
}
static void InstallFitTrace(){if(g_hook)return;g_trace=fopen("prediction-trace.jsonl","wb");auto core=GetModuleHandleA("MVD_Algorithm.dll");if(!g_trace||!core)return;g_hook=(BYTE*)core+0x8bf960;g_original=*g_hook;AddVectoredExceptionHandler(1,TracePrediction);Patch(g_hook,0xcc);}
