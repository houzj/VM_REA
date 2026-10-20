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
 tool->GetBasicParam()->SetCalibBoardType((MVD_CALIBBOARD_TYPE)2);
 IMvdImage*image=nullptr;imageCreate(&image);image->InitImage(480,400,MVD_PIXEL_MONO_08);tool->SetInputImage(image);auto data=image->GetImageData(0);
 for(int shape:{0})for(int contrast:{2,5}){
  for(int y=0;y<400;y++)for(int x=0;x<480;x++){int p=255;for(int row=0;row<8;row++)for(int col=0;col<10;col++){int dx=x-(90+col*30),dy=y-(80+row*30);bool inside=shape==0?dx*dx+dy*dy<81:shape==1?dx*dx/81.f+dy*dy/25.f<1:abs(dx)<9&&abs(dy)<9;if(inside)p=255-contrast;}data->pData[y*data->nRowStep+x]=(unsigned char)p;}
  for(int circularity:{50})for(int threshold:{1,40}){destroy(tool);tool=nullptr;if(create(&tool))return 6;tool->SetRunParam("HomoFreedom","1");tool->SetRunParam("CalibBoardWeightFun","2");tool->GetBasicParam()->SetCalibBoardType((MVD_CALIBBOARD_TYPE)2);tool->SetInputImage(image);
   try{char value[32];sprintf_s(value,"%d",circularity);tool->SetRunParam("Circularity",value);sprintf_s(value,"%d",threshold/2);tool->SetRunParam("EdgeThreshLow",value);sprintf_s(value,"%d",threshold);tool->SetRunParam("EdgeThreshHigh",value);char actual[256];tool->GetRunParam("Circularity",actual);printf("{\"phase\":\"parameter-readback\",\"shape\":%d,\"contrast\":%d,\"requestedCircularity\":%d,\"actualCircularity\":%d}\n",shape,contrast,circularity,atoi(actual));tool->Run();auto result=tool->GetResult();auto point=result->GetCalibBoardLocateInfo()->GetCalibPointImageCoordinate(0);printf("{\"shape\":%d,\"contrast\":%d,\"circularity\":%d,\"low\":%d,\"high\":%d,\"code\":0,\"count\":%d,\"x\":%.9g,\"y\":%.9g}\n",shape,contrast,circularity,threshold/2,threshold,result->GetCalibBoardLocateInfo()->GetCalibPointNum(),point.fX,point.fY);}
   catch(IMVDException&e){printf("{\"shape\":%d,\"contrast\":%d,\"circularity\":%d,\"low\":%d,\"high\":%d,\"code\":%u}\n",shape,contrast,circularity,threshold/2,threshold,e.GetErrorCode());}
  }
 }
 destroy(tool);imageDestroy(image);return 0;
}


