// Original documented SDK, owned synthetic images only; no platform module/device.
#include <windows.h>
#include <cstdio>
#include <vector>
#include <cmath>
#include "MVDCore.h"
#include "CalibBoardCalibCpp.h"
using namespace VisionDesigner;
using namespace VisionDesigner::CalibBoardCalib;
int main(int argc,char**argv){
 if(argc!=2)return 2;SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);setvbuf(stdout,nullptr,_IONBF,0);SetDllDirectoryA(argv[1]);
 auto lib=LoadLibraryA("MVDCalibBoardCalibCpp.dll"),images=LoadLibraryA("MVDImageCpp.dll");if(!lib||!images){printf("{\"phase\":\"load\",\"error\":%lu}\n",GetLastError());return 3;}
 auto create=(int(__stdcall*)(ICalibBoardCalibTool**))GetProcAddress(lib,"CreateCalibBoardCalibToolInstance");
 auto destroy=(int(__stdcall*)(ICalibBoardCalibTool*))GetProcAddress(lib,"DestroyCalibBoardCalibToolInstance");
 auto imageCreate=(int(__stdcall*)(IMvdImage**))GetProcAddress(images,"CreateImageInstance");auto imageDestroy=(int(__stdcall*)(IMvdImage*))GetProcAddress(images,"DestroyImageInstance");
 ICalibBoardCalibTool*tool=nullptr;int code=create(&tool);printf("{\"phase\":\"create\",\"code\":%u}\n",(unsigned)code);if(code)return 4;
 try{unsigned n=0;try{tool->SaveConfiguration(nullptr,0,&n);}catch(...){}std::vector<unsigned char>xml(n+1);tool->SaveConfiguration(xml.data(),n,&n);FILE*f=fopen("original-config.xml","wb");if(f){fwrite(xml.data(),1,n,f);fclose(f);}}catch(...){}
 tool->SetRunParam("HomoFreedom","1");tool->SetRunParam("CalibBoardWeightFun","2");
 IMvdImage*image=nullptr;imageCreate(&image);image->InitImage(480,400,MVD_PIXEL_MONO_08);auto data=image->GetImageData(0);for(int y=0;y<400;y++)for(int x=0;x<480;x++){unsigned char p=255;if(x>=60&&x<420&&y>=50&&y<350)p=(((x-60)/30+(y-50)/30)&1)?255:0;data->pData[y*data->nRowStep+x]=p;}tool->SetInputImage(image);
 for(int filter:{1,2})for(int automatic:{1,2})for(int size:{3,5,7,9,11,15,21,30,31,60,100,150}){
  try{char value[64];sprintf_s(value,"%d",filter);tool->SetRunParam("FilterStatus",value);sprintf_s(value,"%d",automatic);tool->SetRunParam("SubpixelWinAuto",value);sprintf_s(value,"%d",size);tool->SetRunParam("SubPixelWindowSize",value);tool->Run();auto result=tool->GetResult();auto point=result->GetCalibBoardLocateInfo()->GetCalibPointImageCoordinate(0);printf("{\"filter\":%d,\"automatic\":%d,\"size\":%d,\"x\":%.9g,\"y\":%.9g,\"error\":%.9g}\n",filter,automatic,size,point.fX,point.fY,result->GetCalibBoardHomoEstInfo()->GetEstimationError());}
  catch(IMVDException&e){printf("{\"filter\":%d,\"automatic\":%d,\"size\":%d,\"code\":%u}\n",filter,automatic,size,e.GetErrorCode());}
 }
 destroy(tool);imageDestroy(image);return 0;
}
