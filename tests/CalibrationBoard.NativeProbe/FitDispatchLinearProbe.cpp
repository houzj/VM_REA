#include <windows.h>
#include <cstdio>
#include <vector>
#include <cstdint>
#include <cstring>
#include "MVDCore.h"
#include "CalibBoardCalibCpp.h"
#include <cmath>
using namespace VisionDesigner::CalibBoardCalib;
struct XY{float x,y;};struct IJ{int x,y;};struct Options{float x,y,angle;int coordinate;float pitch;int weight,freedom,model;float factor,threshold,ratio;int count;};
static unsigned long long pack(unsigned a,unsigned b){return (unsigned long long)a|((unsigned long long)b<<32);}
static void print(const char*name,const float*p,int count){printf(",\"%s\":[",name);for(int i=0;i<count;i++)printf("%s%.9g",i?",":"",p[i]);printf("]");}
static unsigned char* hook=nullptr,*ret=nullptr;static unsigned char original,retOriginal;static bool stepping=false;
static float* output=nullptr;static int sequence,stage,columns;static FILE* logFile=nullptr;
static LONG CALLBACK Capture(EXCEPTION_POINTERS*e){auto c=e->ContextRecord;
 if(e->ExceptionRecord->ExceptionCode==EXCEPTION_BREAKPOINT&&e->ExceptionRecord->ExceptionAddress==hook){
  columns=*(int*)(c->Rsp+0x28);int rows=(int)c->R8;output=(float*)c->R9;
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
int main(int argc,char**argv){if(argc!=2)return 2;SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);SetDllDirectoryA(argv[1]);setvbuf(stdout,nullptr,_IOFBF,1024*1024);auto sdk=LoadLibraryA("MVDCalibBoardCalibCpp.dll");auto create=(int(__stdcall*)(ICalibBoardCalibTool**))GetProcAddress(sdk,"CreateCalibBoardCalibToolInstance");auto destroy=(int(__stdcall*)(ICalibBoardCalibTool*))GetProcAddress(sdk,"DestroyCalibBoardCalibToolInstance");ICalibBoardCalibTool*tool=nullptr;if(create(&tool))return 3;auto core=(unsigned char*)GetModuleHandleA("MVD_Algorithm.dll");logFile=fopen("stages.jsonl","wb");hook=core+0x8580e0;original=*hook;DWORD protection;VirtualProtect(hook,1,PAGE_EXECUTE_READWRITE,&protection);AddVectoredExceptionHandler(1,Capture);*hook=0xcc;using Fit=int(*)(void*,void*,unsigned long long,unsigned long long,unsigned long long,int,float*,float*,float*,float*,Options*,float*,void*);auto fit=(Fit)(core+0x7ecea0);std::vector<unsigned char>scratch(64*1024*1024);
 int index=0;for(float angle:{0.f,15.f,90.f})for(int freedom:{1,2,3})for(int weight:{1,2,3}){std::vector<XY>points;std::vector<IJ>grid;float rad=angle*.0174532925f,c=cosf(rad),s=sinf(rad);for(int y=0;y<8;y++)for(int x=0;x<10;x++){float px=(x-4.5f)*30,py=(y-3.5f)*30;points.push_back({px*c-py*s+240,px*s+py*c+200});grid.push_back({x,y});}Options options{2,-3,.3f,1,2,weight,1,freedom,2,30,.6f,50};float matrix[9]={},error=-1,inverse[32]={},scale=-1,axes[6]={};memset(scratch.data(),0,scratch.size());sequence=index;stage=0;int code=fit(points.data(),grid.data(),pack(480,400),pack(240,200),pack(10,8),(int)points.size(),matrix,inverse,&error,&scale,&options,axes,scratch.data());printf("{\"phase\":\"fit\",\"index\":%d,\"angle\":%.9g,\"freedom\":%d,\"weight\":%d,\"code\":%u",index++,angle,freedom,weight,(unsigned)code);print("matrix",matrix,9);print("error",&error,1);print("model",inverse,19);print("scale",&scale,1);print("axes",axes,6);printf(",\"input\":[");for(size_t i=0;i<points.size();i++)printf("%s[%.9g,%.9g,%d,%d]",i?",":"",points[i].x,points[i].y,grid[i].x,grid[i].y);printf("]");printf(",\"recoded\":[");for(size_t i=0;i<grid.size();i++){auto p=((IJ*)scratch.data())[i];printf("%s[%d,%d]",i?",":"",p.x,p.y);}printf("]}\n");fflush(stdout);}
 destroy(tool);return 0;}
