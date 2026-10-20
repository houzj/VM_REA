// Adds 813020 observation to the existing owned-process 813ca0 trace.
// No foreign process or on-disk SDK binary is modified.
#include "trace-tag-owned.h"
#include <climits>
static BYTE *g_mapHook=nullptr,*g_mapReturn=nullptr;static BYTE g_mapOriginal,g_mapReturnOriginal;static bool g_mapStep=false;
static unsigned *g_mapDimensions=nullptr;static int *g_mapIndices=nullptr;static float *g_mapPixels=nullptr;static int g_mapCount=0;
static unsigned*g_searchRadius=nullptr;
static LONG CALLBACK TraceAnchorCaller(EXCEPTION_POINTERS*e){auto c=e->ContextRecord;auto where=e->ExceptionRecord->ExceptionAddress;
 if(e->ExceptionRecord->ExceptionCode==EXCEPTION_BREAKPOINT&&where==g_hook){
  auto dimensions=*(unsigned long long*)(c->Rsp+0x38);int width=(int)dimensions,height=(int)(dimensions>>32);auto grid=(int**)c->R9;g_searchRadius=*(unsigned**)(c->Rsp+0x58);
  // 813ca0 param_5 contributes to Y, param_6 to X (four 800670 call sites).
  fprintf(g_trace,"{\"phase\":\"search-entry\",\"offsetAxisLabels\":\"source-confirmed\",\"sequence\":%d,\"board\":%d,\"width\":%d,\"height\":%d,\"offsetRow\":%d,\"offsetColumn\":%d,\"grid\":[",g_sequence+1,g_board,width,height,*(int*)(c->Rsp+0x28),*(int*)(c->Rsp+0x30));
  for(int y=0;y<height;y++)for(int x=0;x<width;x++)fprintf(g_trace,"%s[%d,%d,%d,%d]",x||y?",":"",x,y,grid[y][x*2],grid[y][x*2+1]);fprintf(g_trace,"]}\n");fflush(g_trace);
  return EXCEPTION_CONTINUE_SEARCH; // Existing tag handler owns this breakpoint.
 }
 if(e->ExceptionRecord->ExceptionCode==EXCEPTION_BREAKPOINT&&where==g_mapHook){
  int width=(int)c->R8,height=(int)(c->R8>>32);auto pixels=(float**)c->Rcx;auto indices=(int**)c->Rdx;
  g_mapDimensions=*(unsigned**)(c->Rsp+0x48);g_mapIndices=*(int**)(c->Rsp+0x50);g_mapPixels=*(float**)(c->Rsp+0x58);g_mapCount=0;
  auto core=(BYTE*)GetModuleHandleA("MVD_Algorithm.dll");
  fprintf(g_trace,"{\"phase\":\"map-entry\",\"sequence\":%d,\"board\":%d,\"callerReturnRva\":%llu,\"width\":%d,\"height\":%d,\"anchorRow\":%d,\"anchorColumn\":%d,\"originX\":%d,\"originY\":%d,\"orientation\":%d,\"decodeRotation\":%d,\"grid\":[",g_sequence,g_board,(unsigned long long)(*(BYTE**)c->Rsp-core),width,height,(int)c->R9,*(int*)(c->Rsp+0x28),*(int*)(c->Rsp+0x30),*(int*)(c->Rsp+0x38),*(int*)(c->Rsp+0x40),g_rotation?*g_rotation:-999);
  for(int y=0;y<height;y++)for(int x=0;x<width;x++){int ix=indices[y][x*2],iy=indices[y][x*2+1];if(ix!=INT_MIN)g_mapCount++;fprintf(g_trace,"%s[%d,%d,%d,%d,%.9g,%.9g]",x||y?",":"",x,y,ix,iy,pixels[y][x*2],pixels[y][x*2+1]);}fprintf(g_trace,"],\"searchRadius\":%u}\n",g_searchRadius?*g_searchRadius:0);fflush(g_trace);
  g_mapReturn=*(BYTE**)c->Rsp;g_mapReturnOriginal=*g_mapReturn;Patch(g_mapReturn,0xcc);Patch(g_mapHook,g_mapOriginal);c->Rip=(DWORD64)g_mapHook;c->EFlags|=0x100;g_mapStep=true;return EXCEPTION_CONTINUE_EXECUTION;
 }
 if(e->ExceptionRecord->ExceptionCode==EXCEPTION_BREAKPOINT&&g_mapReturn&&where==g_mapReturn){
  fprintf(g_trace,"{\"phase\":\"map-return\",\"sequence\":%d,\"board\":%d,\"status\":%u,\"dimensions\":[%u,%u],\"countFromInputSentinels\":%d,\"points\":[",g_sequence,g_board,(unsigned)c->Rax,g_mapDimensions[0],g_mapDimensions[1],g_mapCount);
  for(int i=0;i<g_mapCount;i++)fprintf(g_trace,"%s[%d,%d,%.9g,%.9g]",i?",":"",g_mapIndices[i*2],g_mapIndices[i*2+1],g_mapPixels[i*2],g_mapPixels[i*2+1]);fprintf(g_trace,"]}\n");fflush(g_trace);
  Patch(g_mapReturn,g_mapReturnOriginal);c->Rip=(DWORD64)g_mapReturn;g_mapReturn=nullptr;c->EFlags|=0x100;g_mapStep=true;return EXCEPTION_CONTINUE_EXECUTION;
 }
 if(e->ExceptionRecord->ExceptionCode==EXCEPTION_SINGLE_STEP&&g_mapStep){Patch(g_mapHook,0xcc);c->EFlags&=~0x100;g_mapStep=false;return EXCEPTION_CONTINUE_EXECUTION;}
 return EXCEPTION_CONTINUE_SEARCH;
}
static void InstallAnchorCallerTrace(){InstallFitTrace();if(g_mapHook)return;auto core=(BYTE*)GetModuleHandleA("MVD_Algorithm.dll");if(!core||!g_trace)return;g_mapHook=core+0x813020;g_mapOriginal=*g_mapHook;AddVectoredExceptionHandler(1,TraceAnchorCaller);Patch(g_mapHook,0xcc);}
