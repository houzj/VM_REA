// Original documented SDK, owned synthetic images only; no platform module/device.
#include <windows.h>
#include <cstdio>
#include <vector>
#include <cmath>
#include <algorithm>
#include "MVDCore.h"
#include "CalibBoardCalibCpp.h"
using namespace VisionDesigner;
using namespace VisionDesigner::CalibBoardCalib;
static unsigned char*hook=nullptr,*ret=nullptr;static unsigned char original,oldRet;static bool stepping=false;static FILE*trace=nullptr;static float*outH=nullptr;static int activeCase=-1,sequence=0;
static LONG CALLBACK Capture(EXCEPTION_POINTERS*e){auto c=e->ContextRecord;if(e->ExceptionRecord->ExceptionCode==EXCEPTION_BREAKPOINT&&e->ExceptionRecord->ExceptionAddress==hook){auto a=(float*)c->Rcx,b=(float*)c->Rdx;int n=(int)c->R8;outH=(float*)c->R9;fprintf(trace,"{\"phase\":\"before\",\"index\":%d,\"sequence\":%d,\"caller\":%llu,\"points\":[",activeCase,sequence,*(unsigned long long*)c->Rsp-(unsigned long long)GetModuleHandleA("MVD_Algorithm.dll"));if(n>0&&n<2500)for(int i=0;i<n;i++)fprintf(trace,"%s[%.9g,%.9g,%.9g,%.9g]",i?",":"",a[i*2],a[i*2+1],b[i*2],b[i*2+1]);fprintf(trace,"]}\n");fflush(trace);ret=*(unsigned char**)c->Rsp;oldRet=*ret;DWORD protect;VirtualProtect(ret,1,PAGE_EXECUTE_READWRITE,&protect);*ret=0xcc;*hook=original;FlushInstructionCache(GetCurrentProcess(),nullptr,0);c->Rip=(DWORD64)hook;c->EFlags|=0x100;stepping=true;return EXCEPTION_CONTINUE_EXECUTION;}
if(e->ExceptionRecord->ExceptionCode==EXCEPTION_BREAKPOINT&&ret&&e->ExceptionRecord->ExceptionAddress==ret){fprintf(trace,"{\"phase\":\"after\",\"index\":%d,\"sequence\":%d,\"code\":%u,\"matrix\":[",activeCase,sequence++,(unsigned)c->Rax);for(int i=0;i<9;i++)fprintf(trace,"%s%.9g",i?",":"",outH[i]);fprintf(trace,"]}\n");fflush(trace);*ret=oldRet;FlushInstructionCache(GetCurrentProcess(),ret,1);c->Rip=(DWORD64)ret;ret=nullptr;c->EFlags|=0x100;stepping=true;return EXCEPTION_CONTINUE_EXECUTION;}
if(e->ExceptionRecord->ExceptionCode==EXCEPTION_SINGLE_STEP&&stepping){*hook=0xcc;FlushInstructionCache(GetCurrentProcess(),hook,1);c->EFlags&=~0x100;stepping=false;return EXCEPTION_CONTINUE_EXECUTION;}return EXCEPTION_CONTINUE_SEARCH;}
int main(int argc,char**argv){
 if(argc!=2)return 2;SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);setvbuf(stdout,nullptr,_IONBF,0);SetDllDirectoryA(argv[1]);
 auto lib=LoadLibraryA("MVDCalibBoardCalibCpp.dll"),images=LoadLibraryA("MVDImageCpp.dll");if(!lib||!images){printf("{\"phase\":\"load\",\"error\":%lu}\n",GetLastError());return 3;}
 auto create=(int(__stdcall*)(ICalibBoardCalibTool**))GetProcAddress(lib,"CreateCalibBoardCalibToolInstance");
 auto destroy=(int(__stdcall*)(ICalibBoardCalibTool*))GetProcAddress(lib,"DestroyCalibBoardCalibToolInstance");
 auto imageCreate=(int(__stdcall*)(IMvdImage**))GetProcAddress(images,"CreateImageInstance");auto imageDestroy=(int(__stdcall*)(IMvdImage*))GetProcAddress(images,"DestroyImageInstance");
 ICalibBoardCalibTool*tool=nullptr;int code=create(&tool);printf("{\"phase\":\"create\",\"code\":%u}\n",(unsigned)code);if(code)return 4;
 try{unsigned n=0;try{tool->SaveConfiguration(nullptr,0,&n);}catch(...){}std::vector<unsigned char>xml(n+1);tool->SaveConfiguration(xml.data(),n,&n);FILE*f=fopen("original-config.xml","wb");if(f){fwrite(xml.data(),1,n,f);fclose(f);}}catch(...){}
 tool->SetRunParam("HomoFreedom","1");tool->SetRunParam("CalibBoardWeightFun","2");
 tool->GetBasicParam()->SetCalibBoardType((MVD_CALIBBOARD_TYPE)2);
 IMvdImage*image=nullptr;imageCreate(&image);image->InitImage(480,400,MVD_PIXEL_MONO_08);tool->SetInputImage(image);auto data=image->GetImageData(0);
trace=fopen("stages.jsonl","wb");hook=(unsigned char*)GetModuleHandleA("MVD_Algorithm.dll")+0x7ff3c0;original=*hook;DWORD protect;VirtualProtect(hook,1,PAGE_EXECUTE_READWRITE,&protect);AddVectoredExceptionHandler(1,Capture);*hook=0xcc;FlushInstructionCache(GetCurrentProcess(),hook,1);std::vector<int> selected={83,120,121,124,125,126,127,130,131,132,133,134,135,136,137,138,139,140,141,142,143,144,145,148,149,150,151,154,155,160,161,166,167};int next=0;
 for(int gradient:{0,1})for(int start:{3,5,8,9,12})for(int top:{0,1})for(int shape:{0,1,2})for(int contrast:{5,255}){if(gradient&&contrast==255)continue;for(int threshold:{40,255}){int index=next++;if(std::find(selected.begin(),selected.end(),index)==selected.end())continue;
  for(int y=0;y<400;y++)std::fill(data->pData+y*data->nRowStep,data->pData+y*data->nRowStep+480,255);
  for(int row=0;row<8;row++)for(int col=0;col<10;col++)for(int dy=-9;dy<=9;dy++)for(int dx=-9;dx<=9;dx++){bool inside=shape==0?dx*dx+dy*dy<81:shape==1?dx*dx/81.f+dy*dy/25.f<1:abs(dx)<9&&abs(dy)<9;int px=start+col*30+dx,py=(top?start:80)+row*30+dy;if(inside&&px>=0&&px<480&&py>=0&&py<400)data->pData[py*data->nRowStep+px]=(unsigned char)(255-contrast+(gradient&&dx*dx+dy*dy<16?2:0));}
  printf("{\"index\":%d,\"gradient\":%s,\"start\":%d,\"top\":%d,\"shape\":%d,\"contrast\":%d,\"low\":%d,\"high\":%d,",index,gradient?"true":"false",start,top,shape,contrast,threshold/2,threshold);fflush(stdout);
  try{char value[32];tool->SetRunParam("Circularity","20");sprintf_s(value,"%d",threshold/2);tool->SetRunParam("EdgeThreshLow",value);sprintf_s(value,"%d",threshold);tool->SetRunParam("EdgeThreshHigh",value);activeCase=index;tool->Run();auto result=tool->GetResult();auto locate=result->GetCalibBoardLocateInfo();auto h=result->GetCalibBoardHomoEstInfo();auto params=result->GetCalibMatrixToParamInfo();float matrix[9];h->GetHomoMatrix(matrix);auto origin=h->GetOriginPoint(),vx=h->GetXVector(),vy=h->GetYVector();printf("\"code\":0,\"points\":[");for(int i=0;i<locate->GetCalibPointNum();i++){auto p=locate->GetCalibPointImageCoordinate(i);auto q=locate->GetCalibPointObjectCoordinate(i);printf("%s[%d,%d,%.9g,%.9g]",i?",":"",q.nX,q.nY,p.fX,p.fY);}printf("],\"matrix\":[");for(int i=0;i<9;i++)printf("%s%.9g",i?",":"",matrix[i]);printf("],\"origin\":[%.9g,%.9g],\"vx\":[%.9g,%.9g],\"vy\":[%.9g,%.9g],\"error\":%.9g,\"scale\":%.9g,\"x\":%.9g,\"y\":%.9g,\"rotate\":%.9g,\"aspect\":%.9g,\"skew\":%.9g,\"precision\":%.9g}\n",origin.fX,origin.fY,vx.fX,vx.fY,vy.fX,vy.fY,h->GetEstimationError(),params->GetScale(),params->GetOffsetX(),params->GetOffsetY(),params->GetRotate(),params->GetAspect(),params->GetSkew(),params->GetPixelPrecision());}
  catch(IMVDException&e){printf("\"code\":%u}\n",e.GetErrorCode());}fflush(stdout);
 }}
 *hook=original;FlushInstructionCache(GetCurrentProcess(),hook,1);fclose(trace);destroy(tool);imageDestroy(image);return 0;
}

