// Original documented SDK, owned synthetic images only; no platform module/device.
#include <windows.h>
#include <cstdio>
#include <vector>
#include <cmath>
#include "MVDCore.h"
#include "CalibBoardCalibCpp.h"
using namespace VisionDesigner;
using namespace VisionDesigner::CalibBoardCalib;
struct Hook{unsigned char*address;unsigned char original;unsigned count;};static Hook hooks[3];static int active=-1;
static LONG CALLBACK Capture(EXCEPTION_POINTERS*e){auto c=e->ContextRecord;if(e->ExceptionRecord->ExceptionCode==EXCEPTION_BREAKPOINT){for(int i=0;i<3;i++)if(e->ExceptionRecord->ExceptionAddress==hooks[i].address){auto&h=hooks[i];h.count++;*h.address=h.original;FlushInstructionCache(GetCurrentProcess(),h.address,1);c->Rip=(DWORD64)h.address;c->EFlags|=0x100;active=i;return EXCEPTION_CONTINUE_EXECUTION;}}if(e->ExceptionRecord->ExceptionCode==EXCEPTION_SINGLE_STEP&&active>=0){auto&h=hooks[active];*h.address=0xcc;FlushInstructionCache(GetCurrentProcess(),h.address,1);c->EFlags&=~0x100;active=-1;return EXCEPTION_CONTINUE_EXECUTION;}return EXCEPTION_CONTINUE_SEARCH;}
int main(int argc,char**argv){
 if(argc!=2)return 2;SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);setvbuf(stdout,nullptr,_IONBF,0);SetDllDirectoryA(argv[1]);
 auto lib=LoadLibraryA("MVDCalibBoardCalibCpp.dll"),images=LoadLibraryA("MVDImageCpp.dll");if(!lib||!images){printf("{\"phase\":\"load\",\"error\":%lu}\n",GetLastError());return 3;}
 auto create=(int(__stdcall*)(ICalibBoardCalibTool**))GetProcAddress(lib,"CreateCalibBoardCalibToolInstance");
 auto destroy=(int(__stdcall*)(ICalibBoardCalibTool*))GetProcAddress(lib,"DestroyCalibBoardCalibToolInstance");
 auto imageCreate=(int(__stdcall*)(IMvdImage**))GetProcAddress(images,"CreateImageInstance");auto imageDestroy=(int(__stdcall*)(IMvdImage*))GetProcAddress(images,"DestroyImageInstance");
 ICalibBoardCalibTool*tool=nullptr;int code=create(&tool);printf("{\"phase\":\"create\",\"code\":%u}\n",(unsigned)code);if(code)return 4;
 auto core=(unsigned char*)GetModuleHandleA("MVD_Algorithm.dll");unsigned offsets[3]={0x8b47c0,0x8bb3a0,0x8bcff0};for(int i=0;i<3;i++){hooks[i].address=core+offsets[i];hooks[i].original=*hooks[i].address;DWORD p;VirtualProtect(hooks[i].address,1,PAGE_EXECUTE_READWRITE,&p);*hooks[i].address=0xcc;}AddVectoredExceptionHandler(1,Capture);try{unsigned n=0;try{tool->SaveConfiguration(nullptr,0,&n);}catch(...){}std::vector<unsigned char>xml(n+1);tool->SaveConfiguration(xml.data(),n,&n);FILE*f=fopen("original-config.xml","wb");if(f){fwrite(xml.data(),1,n,f);fclose(f);}}catch(...){}
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
 }printf("{\"phase\":\"branch-counts\",\"8b47c0\":%u,\"8bb3a0\":%u,\"8bcff0\":%u}\n",hooks[0].count,hooks[1].count,hooks[2].count);destroy(tool);return 0;
}







