#include <windows.h>
#include <cstdio>
#include <vector>
#include <algorithm>
#include "MVDCore.h"
#include "MVDShapeCpp.h"
#include "CalibBoardCalibCpp.h"
using namespace VisionDesigner;
using namespace VisionDesigner::CalibBoardCalib;
int main(int argc,char**argv){
 if(argc!=2)return 2;SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);SetDllDirectoryA(argv[1]);setvbuf(stdout,nullptr,_IOFBF,1024*1024);
 auto shapes=LoadLibraryA("MVDShapeCpp.dll");if(!shapes)return 3;
 auto rectangle=(int(__stdcall*)(IMvdRectangleF**,float,float,float,float))GetProcAddress(shapes,"CreateRectangleInstance");auto rectangleDestroy=(int(__stdcall*)(IMvdRectangleF*))GetProcAddress(shapes,"DestroyRectangleInstance");
 for(float center:{0.f,.25f,.49f,.5f,.51f,40.f,50.5f,100.125f,1000000.5f})for(float width:{1.f,2.5f,3.25f,64.1f,127.3f}){
  IMvdRectangleF*r=nullptr;float cy=center+.13f,height=width*.7f;if(rectangle(&r,center,cy,width,height))return 4;
  printf("{\"phase\":\"shape\",\"cx\":%.9g,\"cy\":%.9g,\"width\":%.9g,\"height\":%.9g,\"left\":%.9g,\"top\":%.9g}\n",r->GetCenterX(),r->GetCenterY(),r->GetWidth(),r->GetHeight(),r->GetLeftTopX(),r->GetLeftTopY());rectangleDestroy(r);
 }
 auto sdk=LoadLibraryA("MVDCalibBoardCalibCpp.dll"),images=LoadLibraryA("MVDImageCpp.dll");if(!sdk||!images)return 5;
 auto create=(int(__stdcall*)(ICalibBoardCalibTool**))GetProcAddress(sdk,"CreateCalibBoardCalibToolInstance");auto destroy=(int(__stdcall*)(ICalibBoardCalibTool*))GetProcAddress(sdk,"DestroyCalibBoardCalibToolInstance");
 auto imageCreate=(int(__stdcall*)(IMvdImage**))GetProcAddress(images,"CreateImageInstance");auto imageDestroy=(int(__stdcall*)(IMvdImage*))GetProcAddress(images,"DestroyImageInstance");
 ICalibBoardCalibTool*tool=nullptr;IMvdImage*image=nullptr;if(create(&tool)||imageCreate(&image))return 6;tool->SetRunParam("HomoFreedom","1");tool->SetRunParam("CalibBoardWeightFun","2");tool->GetBasicParam()->SetCalibBoardType((MVD_CALIBBOARD_TYPE)2);
 image->InitImage(480,400,MVD_PIXEL_MONO_08);auto data=image->GetImageData(0);for(int y=0;y<400;y++)std::fill(data->pData+y*data->nRowStep,data->pData+y*data->nRowStep+480,255);
 for(int row=0;row<8;row++)for(int col=0;col<10;col++)for(int dy=-9;dy<=9;dy++)for(int dx=-9;dx<=9;dx++)if(dx*dx+dy*dy<81)data->pData[(80+row*30+dy)*data->nRowStep+90+col*30+dx]=0;tool->SetInputImage(image);
 float cases[][4]={{0,0,480,400},{50.4f,40.4f,370.2f,300.2f},{53.5f,43.5f,360.2f,300.2f},{80.49f,70.49f,300.02f,240.02f},{480,0,1,100},{-10,0,5,100},{-10,-10,400,330},{479.75f,0,.1f,100},{-.49f,0,.98f,100},{0,400,100,1},{0,-20,400,10},{0,0,64,64},{0,0,480,319},{0,0,480,320},{0,0,480,321},{0,0,32,32},{0,0,63,63}};
 int index=0;for(auto&values:cases){IMvdRectangleF*r=nullptr;rectangle(&r,values[0]+values[2]*.5f,values[1]+values[3]*.5f,values[2],values[3]);tool->SetROI(r);
  float left=r->GetLeftTopX(),top=r->GetLeftTopY();printf("{\"phase\":\"roi\",\"index\":%d,\"left\":%.9g,\"top\":%.9g,\"width\":%.9g,\"height\":%.9g,",index++,left,top,r->GetWidth(),r->GetHeight());
  try{tool->Run();auto locate=tool->GetResult()->GetCalibBoardLocateInfo();printf("\"code\":0,\"count\":%d,\"points\":[",locate->GetCalibPointNum());for(int i=0;i<locate->GetCalibPointNum();i++){auto p=locate->GetCalibPointImageCoordinate(i);printf("%s[%.9g,%.9g]",i?",":"",p.fX,p.fY);}printf("]}\n");}
  catch(IMVDException&e){printf("\"code\":%u}\n",e.GetErrorCode());}tool->SetROI(nullptr);rectangleDestroy(r);
 }
 destroy(tool);imageDestroy(image);return 0;
}
