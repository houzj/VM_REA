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
 for(int board:{1,2})for(int size:{32,63,64,65,100}){int mode=size;
 IMvdImage*image=nullptr;imageCreate(&image);image->InitImage(size,size,MVD_PIXEL_MONO_08);auto data=image->GetImageData(0);
 for(int y=0;y<size;y++)for(int x=0;x<size;x++)data->pData[y*data->nRowStep+x]=255;
 tool->SetInputImage(image);tool->GetBasicParam()->SetCalibBoardType((MVD_CALIBBOARD_TYPE)board);tool->GetBasicParam()->SetCoordinateMode((MVD_CALIBBOARD_COORDINATE_MODE)(mode==2||mode==5?2:1));
 tool->GetBasicParam()->SetOriginPoint({0.f,0.f});tool->GetBasicParam()->SetAngle(0.f);tool->GetBasicParam()->SetCalibBoardGridSize(1.f);
 try{tool->Run();printf("{\"board\":%d,\"mode\":%d,\"code\":0}\n",board,mode); }
 catch(IMVDException&e){printf("{\"board\":%d,\"mode\":%d,\"code\":%u}\n",board,mode,e.GetErrorCode());}catch(...){printf("{\"board\":%d,\"mode\":%d,\"unknown\":true}\n",board,mode);}
 imageDestroy(image);
 }destroy(tool);return 0;
}









