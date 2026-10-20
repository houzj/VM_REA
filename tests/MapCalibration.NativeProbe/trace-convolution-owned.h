// Isolated float-convolution evidence in this probe's process only.
static int g_board=0,g_model=0,g_sequence=0,g_width=0,g_height=0,g_outputStride=0;
static FILE* g_trace=nullptr;
static BYTE *g_hook=nullptr,*g_return=nullptr,g_original=0,g_returnOriginal=0;
static float* g_output=nullptr;
static bool g_step=false;
static void Patch(BYTE* address,BYTE value){DWORD old;VirtualProtect(address,1,PAGE_EXECUTE_READWRITE,&old);*address=value;FlushInstructionCache(GetCurrentProcess(),address,1);}
static void Dump(const char* kind,float* pixels,int width,int height,int stride){char path[100];sprintf_s(path,"convolution-%03d-%s.float",g_sequence,kind);FILE* file=fopen(path,"wb");if(file){for(int y=0;y<height;y++)fwrite(pixels+(long long)y*stride,4,width,file);fclose(file);}}
static LONG CALLBACK TraceConvolution(EXCEPTION_POINTERS* error){auto c=error->ContextRecord;auto where=error->ExceptionRecord->ExceptionAddress;
 if(error->ExceptionRecord->ExceptionCode==EXCEPTION_BREAKPOINT){
  if(where==g_hook){g_sequence++;uint64_t dimensions=*(uint64_t*)(c->Rsp+0x28),kernel=*(uint64_t*)(c->Rsp+0x38),anchor=*(uint64_t*)(c->Rsp+0x40);g_width=(int)dimensions;g_height=(int)(dimensions>>32);g_output=(float*)c->R8;g_outputStride=(int)c->R9;
   if(g_width<1||g_width>64||g_height<1||g_height>64)TerminateProcess(GetCurrentProcess(),20);
   Dump("input",(float*)c->Rcx,g_width,g_height,(int)c->Rdx);Dump("kernel",*(float**)(c->Rsp+0x30),(int)kernel,(int)(kernel>>32),(int)kernel);
   fprintf(g_trace,"{\"sequence\":%d,\"board\":%d,\"width\":%d,\"height\":%d,\"kernelWidth\":%d,\"kernelHeight\":%d,\"rawStack40Low\":%d,\"rawStack40High\":%d}\n",g_sequence,g_board,g_width,g_height,(int)kernel,(int)(kernel>>32),(int)anchor,(int)(anchor>>32));fflush(g_trace);
   g_return=*(BYTE**)c->Rsp;g_returnOriginal=*g_return;Patch(g_return,0xcc);Patch(g_hook,g_original);c->Rip=(DWORD64)g_hook;c->EFlags|=0x100;g_step=true;return EXCEPTION_CONTINUE_EXECUTION;
  }
  if(g_return&&where==g_return){Dump("output",g_output,g_width,g_height,g_outputStride);Patch(g_return,g_returnOriginal);c->Rip=(DWORD64)g_return;g_return=nullptr;c->EFlags|=0x100;g_step=true;return EXCEPTION_CONTINUE_EXECUTION;}
 }
 if(error->ExceptionRecord->ExceptionCode==EXCEPTION_SINGLE_STEP&&g_step){if(g_sequence<48)Patch(g_hook,0xcc);c->EFlags&=~0x100;g_step=false;return EXCEPTION_CONTINUE_EXECUTION;}
 return EXCEPTION_CONTINUE_SEARCH;
}
static void InstallFitTrace(){if(g_hook)return;g_trace=fopen("convolution-trace.jsonl","wb");auto core=GetModuleHandleA("MVD_Algorithm.dll");if(!g_trace||!core)return;g_hook=(BYTE*)core+0x756f10;g_original=*g_hook;AddVectoredExceptionHandler(1,TraceConvolution);Patch(g_hook,0xcc);}

