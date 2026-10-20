// Original documented SDK, owned synthetic images only; no platform module/device.
#include <windows.h>
#include <cstdio>
#include <vector>
#include <cmath>
#include "MVDCore.h"
#include "CalibBoardCalibCpp.h"
using namespace VisionDesigner;
using namespace VisionDesigner::CalibBoardCalib;
static unsigned char*hook=nullptr,*ret=nullptr;static unsigned char original,oldRet;static bool stepping=false;static FILE*trace=nullptr;static unsigned*records=nullptr;static int*count=nullptr;static int sequence=0;
static void PrintRecords(const char*phase){fprintf(trace,"{\"phase\":\"%s\",\"sequence\":%d,\"count\":%d,\"words\":[",phase,sequence,*count);if(*count>=0&&*count<30000)for(int i=0;i<*count*7;i++)fprintf(trace,"%s%u",i?",":"",records[i]);fprintf(trace,"]}\n");fflush(trace);}
static LONG CALLBACK Capture(EXCEPTION_POINTERS*e){auto c=e->ContextRecord;if(e->ExceptionRecord->ExceptionCode==EXCEPTION_BREAKPOINT&&e->ExceptionRecord->ExceptionAddress==hook){records=*(unsigned**)(c->Rsp+0x30);count=*(int**)(c->Rsp+0x38);auto cfg=(unsigned*)c->R9;unsigned w=(unsigned)c->R8,h=(unsigned)(c->R8>>32),stride=(unsigned)c->Rdx;fprintf(trace,"{\"phase\":\"context\",\"sequence\":%d,\"width\":%u,\"height\":%u,\"stride\":%u,\"capacity\":%u,\"options\":[%u,%u,%u,%u]}\n",sequence,w,h,stride,*(unsigned*)(c->Rsp+0x28),cfg[0],cfg[1],cfg[2],cfg[3]);fflush(trace);if(w<=480&&h<=400&&stride<=480){char name[64];sprintf_s(name,"image-%03d.bin",sequence);auto f=fopen(name,"wb");if(f){fwrite((void*)c->Rcx,1,stride*h,f);fclose(f);}}PrintRecords("before");ret=*(unsigned char**)c->Rsp;oldRet=*ret;DWORD p;VirtualProtect(ret,1,PAGE_EXECUTE_READWRITE,&p);*ret=0xcc;*hook=original;FlushInstructionCache(GetCurrentProcess(),nullptr,0);c->Rip=(DWORD64)hook;c->EFlags|=0x100;stepping=true;return EXCEPTION_CONTINUE_EXECUTION;}
if(e->ExceptionRecord->ExceptionCode==EXCEPTION_BREAKPOINT&&ret&&e->ExceptionRecord->ExceptionAddress==ret){PrintRecords("after");sequence++;*ret=oldRet;FlushInstructionCache(GetCurrentProcess(),ret,1);c->Rip=(DWORD64)ret;ret=nullptr;c->EFlags|=0x100;stepping=true;return EXCEPTION_CONTINUE_EXECUTION;}if(e->ExceptionRecord->ExceptionCode==EXCEPTION_SINGLE_STEP&&stepping){*hook=0xcc;FlushInstructionCache(GetCurrentProcess(),hook,1);c->EFlags&=~0x100;stepping=false;return EXCEPTION_CONTINUE_EXECUTION;}return EXCEPTION_CONTINUE_SEARCH;}
int main(int argc,char**argv){
 if(argc!=2)return 2;SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);setvbuf(stdout,nullptr,_IONBF,0);SetDllDirectoryA(argv[1]);
 auto lib=LoadLibraryA("MVDCalibBoardCalibCpp.dll"),images=LoadLibraryA("MVDImageCpp.dll");if(!lib||!images){printf("{\"phase\":\"load\",\"error\":%lu}\n",GetLastError());return 3;}
 auto create=(int(__stdcall*)(ICalibBoardCalibTool**))GetProcAddress(lib,"CreateCalibBoardCalibToolInstance");
 auto destroy=(int(__stdcall*)(ICalibBoardCalibTool*))GetProcAddress(lib,"DestroyCalibBoardCalibToolInstance");
 auto imageCreate=(int(__stdcall*)(IMvdImage**))GetProcAddress(images,"CreateImageInstance");auto imageDestroy=(int(__stdcall*)(IMvdImage*))GetProcAddress(images,"DestroyImageInstance");
 ICalibBoardCalibTool*tool=nullptr;int code=create(&tool);printf("{\"phase\":\"create\",\"code\":%u}\n",(unsigned)code);if(code)return 4;
 trace=fopen("refinement.jsonl","wb");hook=(unsigned char*)GetModuleHandleA("MVD_Algorithm.dll")+0x8bb3a0;original=*hook;DWORD protect;VirtualProtect(hook,1,PAGE_EXECUTE_READWRITE,&protect);AddVectoredExceptionHandler(1,Capture);*hook=0xcc;try{unsigned n=0;try{tool->SaveConfiguration(nullptr,0,&n);}catch(...){}std::vector<unsigned char>xml(n+1);tool->SaveConfiguration(xml.data(),n,&n);FILE*f=fopen("original-config.xml","wb");if(f){fwrite(xml.data(),1,n,f);fclose(f);}}catch(...){}
 tool->SetRunParam("HomoFreedom","1");tool->SetRunParam("CalibBoardWeightFun","2");
 for(int board:{1})for(int mode:{3,4}){
tool->SetRunParam("CalibBoardWeightFun",mode==3?"3":"4");
 IMvdImage*image=nullptr;imageCreate(&image);image->InitImage(480,400,MVD_PIXEL_MONO_08);auto data=image->GetImageData(0);
 for(int y=0;y<400;y++)for(int x=0;x<480;x++){unsigned char p=255;if(mode){if(board==1&&x>=60&&x<420&&y>=50&&y<350)p=(((x-60)/30+(y-50)/30)&1)?255:0;if(board==2)for(int row=0;row<8;row++)for(int col=0;col<10;col++)if((x-(90+col*30))*(x-(90+col*30))+(y-(80+row*30))*(y-(80+row*30))<81)p=0;}data->pData[y*480+x]=p;}
 tool->SetInputImage(image);tool->GetBasicParam()->SetCalibBoardType((MVD_CALIBBOARD_TYPE)board);tool->GetBasicParam()->SetCoordinateMode((MVD_CALIBBOARD_COORDINATE_MODE)(mode==2||mode==5?2:1));
 tool->GetBasicParam()->SetOriginPoint({2.f,-3.f});tool->GetBasicParam()->SetAngle(0.f);tool->GetBasicParam()->SetCalibBoardGridSize(1.f);
 try{tool->Run();char output[64];sprintf_s(output,"board-%d-%d.iwcal",board,mode);tool->ExportCalibFile(output);auto r=tool->GetResult();auto locate=r->GetCalibBoardLocateInfo();auto h=r->GetCalibBoardHomoEstInfo();float matrix[9];h->GetHomoMatrix(matrix);printf("{\"board\":%d,\"mode\":%d,\"code\":0,\"count\":%d,\"matrix\":[",board,mode,locate->GetCalibPointNum());for(int j=0;j<9;j++)printf("%s%.9g",j?",":"",matrix[j]);printf("],\"points\":[");for(int j=0;j<locate->GetCalibPointNum();j++){auto p=locate->GetCalibPointImageCoordinate(j);auto q=locate->GetCalibPointObjectCoordinate(j);printf("%s[%.9g,%.9g,%d,%d]",j?",":"",p.fX,p.fY,q.nX,q.nY);}auto params=r->GetCalibMatrixToParamInfo();auto origin=h->GetOriginPoint(),vx=h->GetXVector(),vy=h->GetYVector();printf("],\"origin\":[%.9g,%.9g],\"vx\":[%.9g,%.9g],\"vy\":[%.9g,%.9g]}\n",origin.fX,origin.fY,vx.fX,vx.fY,vy.fX,vy.fY);printf("{\"phase\":\"metrics\",\"board\":%d,\"mode\":%d,\"error\":%.9g,\"homoScale\":%.9g,\"scale\":%.9g,\"x\":%.9g,\"y\":%.9g,\"rotate\":%.9g,\"aspect\":%.9g,\"skew\":%.9g,\"precision\":%.9g}\n",board,mode,h->GetEstimationError(),h->GetScale(),params->GetScale(),params->GetOffsetX(),params->GetOffsetY(),params->GetRotate(),params->GetAspect(),params->GetSkew(),params->GetPixelPrecision());}
 catch(IMVDException&e){printf("{\"board\":%d,\"mode\":%d,\"code\":%u}\n",board,mode,e.GetErrorCode());}catch(...){printf("{\"board\":%d,\"mode\":%d,\"unknown\":true}\n",board,mode);}
 imageDestroy(image);
 }destroy(tool);return 0;
}







