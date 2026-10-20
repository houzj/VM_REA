static int g_board=0,g_model=0,g_sequence=0;
static FILE* g_trace=nullptr;
static BYTE *g_hook=nullptr,*g_return=nullptr,g_original=0,g_returnOriginal=0;
static double *g_values=nullptr,*g_vectors=nullptr;
static bool g_step=false;
static void Patch(BYTE* address,BYTE value){DWORD old;VirtualProtect(address,1,PAGE_EXECUTE_READWRITE,&old);*address=value;FlushInstructionCache(GetCurrentProcess(),address,1);}
static void Dump(const char* kind,double* values,int count){char path[100];sprintf_s(path,"eigen-%03d-%s.double",g_sequence,kind);FILE* file=fopen(path,"wb");if(file){fwrite(values,8,count,file);fclose(file);}}
static LONG CALLBACK TraceEigen(EXCEPTION_POINTERS* error){auto c=error->ContextRecord;auto where=error->ExceptionRecord->ExceptionAddress;
 if(error->ExceptionRecord->ExceptionCode==EXCEPTION_BREAKPOINT){
  if(where==g_hook){g_sequence++;g_values=(double*)c->Rdx;g_vectors=(double*)c->R8;Dump("input",(double*)c->Rcx,4);g_return=*(BYTE**)c->Rsp;g_returnOriginal=*g_return;Patch(g_return,0xcc);Patch(g_hook,g_original);c->Rip=(DWORD64)g_hook;c->EFlags|=0x100;g_step=true;return EXCEPTION_CONTINUE_EXECUTION;}
  if(g_return&&where==g_return){Dump("values",g_values,2);Dump("vectors",g_vectors,4);fprintf(g_trace,"{\"sequence\":%d,\"board\":%d,\"status\":%u}\n",g_sequence,g_board,(unsigned)c->Rax);fflush(g_trace);Patch(g_return,g_returnOriginal);c->Rip=(DWORD64)g_return;g_return=nullptr;c->EFlags|=0x100;g_step=true;return EXCEPTION_CONTINUE_EXECUTION;}
 }
 if(error->ExceptionRecord->ExceptionCode==EXCEPTION_SINGLE_STEP&&g_step){if(g_sequence<96)Patch(g_hook,0xcc);c->EFlags&=~0x100;g_step=false;return EXCEPTION_CONTINUE_EXECUTION;}
 return EXCEPTION_CONTINUE_SEARCH;
}
static void InstallFitTrace(){if(g_hook)return;g_trace=fopen("eigen-trace.jsonl","wb");auto core=GetModuleHandleA("MVD_Algorithm.dll");if(!g_trace||!core)return;g_hook=(BYTE*)core+0x8bcba0;g_original=*g_hook;AddVectoredExceptionHandler(1,TraceEigen);Patch(g_hook,0xcc);}
