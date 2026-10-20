#define NOMINMAX
#include <windows.h>
#include <cstdio>
#include <cstdlib>
#include <vector>
#include <algorithm>
#include "MVDCore.h"
#include "MVDShapeCpp.h"
#include "CalibBoardCalibCpp.h"
#include "PreproMaskCpp.h"
using namespace VisionDesigner;using namespace VisionDesigner::CalibBoardCalib;using namespace VisionDesigner::PreproMask;
int main(int argc,char**argv){if(argc!=4)return 2;SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);SetDllDirectoryA(argv[1]);setvbuf(stdout,nullptr,_IOFBF,1024*1024);
 auto sdk=LoadLibraryA("MVDCalibBoardCalibCpp.dll"),images=LoadLibraryA("MVDImageCpp.dll"),shapes=LoadLibraryA("MVDShapeCpp.dll"),masks=LoadLibraryA("MVDPreproMaskCpp.dll");if(!sdk||!images||!shapes||!masks)return 3;
 auto create=(int(__stdcall*)(ICalibBoardCalibTool**))GetProcAddress(sdk,"CreateCalibBoardCalibToolInstance");auto destroy=(int(__stdcall*)(ICalibBoardCalibTool*))GetProcAddress(sdk,"DestroyCalibBoardCalibToolInstance");
 auto imageCreate=(int(__stdcall*)(IMvdImage**))GetProcAddress(images,"CreateImageInstance");auto imageDestroy=(int(__stdcall*)(IMvdImage*))GetProcAddress(images,"DestroyImageInstance");
 auto rectangle=(int(__stdcall*)(IMvdRectangleF**,float,float,float,float))GetProcAddress(shapes,"CreateRectangleInstance");auto freeRectangle=(int(__stdcall*)(IMvdRectangleF*))GetProcAddress(shapes,"DestroyRectangleInstance");auto maskCreate=(int(__stdcall*)(IPreproMaskTool**))GetProcAddress(masks,"CreatePreproMaskToolInstance");auto maskDestroy=(int(__stdcall*)(IPreproMaskTool*))GetProcAddress(masks,"DestroyPreproMaskToolInstance");
 IMvdImage*image=nullptr;if(imageCreate(&image))return 4;image->InitImage(argv[2],MVD_PIXEL_MONO_08);const int W=image->GetWidth(),H=image->GetHeight();int board=atoi(argv[3]);
 int index=0;for(float ratio:{.7f,.85f,.98f})for(float angle:{.0002f,5.f,15.f,30.f,45.f,90.f,-30.f}){float width=W*ratio,cx=W*.5f,cy=H*.5f,height=H*ratio;ICalibBoardCalibTool*tool=nullptr;IMvdRectangleF*r=nullptr;IPreproMaskTool*mask=nullptr;if(create(&tool)||rectangle(&r,cx,cy,width,height)||maskCreate(&mask))return 5;r->SetAngle(angle);mask->SetInputImageSize(W,H);mask->AddRegion(r,true);fprintf(stderr,"case=%d phase=maskRun\n",index);mask->Run();fprintf(stderr,"case=%d phase=maskReady\n",index);auto md=mask->GetOutputImage()->GetImageData(0);int minx=W,miny=H,maxx=-1,maxy=-1;for(int y=0;y<H;y++)for(int x=0;x<W;x++)if(md->pData[y*md->nRowStep+x]){minx=std::min(minx,x);maxx=std::max(maxx,x);miny=std::min(miny,y);maxy=std::max(maxy,y);}
 tool->SetRunParam("HomoFreedom","1");tool->SetRunParam("CalibBoardWeightFun","2");tool->GetBasicParam()->SetCalibBoardType((MVD_CALIBBOARD_TYPE)board);tool->SetInputImage(image);fprintf(stderr,"case=%d phase=setROI\n",index);tool->SetROI(r);fprintf(stderr,"case=%d phase=roiReady\n",index);
 printf("{\"index\":%d,\"cx\":%.9g,\"cy\":%.9g,\"width\":%.9g,\"height\":%.9g,\"angle\":%.9g,\"bounds\":[%d,%d,%d,%d],",index++,cx,cy,width,height,angle,minx,miny,maxx<0?0:maxx+1-minx,maxy<0?0:maxy+1-miny);
 fflush(stdout);fprintf(stderr,"case=%d phase=toolRun\n",index-1);try{tool->Run();fprintf(stderr,"case=%d phase=toolReady\n",index-1);auto result=tool->GetResult()->GetCalibBoardLocateInfo();fprintf(stderr,"case=%d phase=count value=%d\n",index-1,result->GetCalibPointNum());if(result->GetCalibPointNum()<0||result->GetCalibPointNum()>2500)return 12;printf("\"code\":0,\"count\":%d,\"points\":[",result->GetCalibPointNum());for(int i=0;i<result->GetCalibPointNum();i++){auto p=result->GetCalibPointImageCoordinate(i);auto o=result->GetCalibPointObjectCoordinate(i);printf("%s[%d,%d,%.9g,%.9g]",i?",":"",o.nX,o.nY,p.fX,p.fY);}printf("]}\n");}catch(IMVDException&e){printf("\"code\":%u}\n",e.GetErrorCode());}fflush(stdout);fprintf(stderr,"case=%d phase=destroy\n",index-1);tool->SetROI(nullptr);destroy(tool);mask->ClearRegions();maskDestroy(mask);freeRectangle(r);
 }imageDestroy(image);return 0;}
