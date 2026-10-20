// Original documented SDK, owned synthetic images only; no platform module/device.
#include <windows.h>
#include <cstdio>
#include <vector>
#include <cmath>
#include "MVDCore.h"
#include "CalibBoardCalibCpp.h"
#include "NPointCalibCpp.h"
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
 for(int board:{1,2})for(int mode:{0,1,2,3,4,5}){
 IMvdImage*image=nullptr;imageCreate(&image);image->InitImage(480,400,MVD_PIXEL_MONO_08);auto data=image->GetImageData(0);
 for(int y=0;y<400;y++)for(int x=0;x<480;x++){unsigned char p=255;if(mode){if(board==1&&x>=60&&x<420&&y>=50&&y<350)p=(((x-60)/30+(y-50)/30)&1)?255:0;if(board==2)for(int row=0;row<8;row++)for(int col=0;col<10;col++)if((x-(90+col*30))*(x-(90+col*30))+(y-(80+row*30))*(y-(80+row*30))<81)p=0;}data->pData[y*480+x]=p;}
 tool->SetInputImage(image);tool->GetBasicParam()->SetCalibBoardType((MVD_CALIBBOARD_TYPE)board);tool->GetBasicParam()->SetCoordinateMode((MVD_CALIBBOARD_COORDINATE_MODE)(mode==2||mode==5?2:1));
 tool->GetBasicParam()->SetOriginPoint({mode>=3?2.f:0.f,mode>=3?-3.f:0.f});tool->GetBasicParam()->SetAngle(mode==3||mode==5?90.f:0.f);tool->GetBasicParam()->SetCalibBoardGridSize(mode>=3?2.f:1.f);
 try{tool->Run();char output[64];sprintf_s(output,"board-%d-%d.iwcal",board,mode);tool->ExportCalibFile(output);auto r=tool->GetResult();auto locate=r->GetCalibBoardLocateInfo();auto h=r->GetCalibBoardHomoEstInfo();auto nlib=LoadLibraryA("MVDNPointCalibCpp.dll");auto createN=(int(__stdcall*)(VisionDesigner::NPointCalib::INPointCalibTool**))GetProcAddress(nlib,"CreateNPointCalibToolInstance");auto destroyN=(int(__stdcall*)(VisionDesigner::NPointCalib::INPointCalibTool*))GetProcAddress(nlib,"DestroyNPointCalibToolInstance");VisionDesigner::NPointCalib::INPointCalibTool*ntool=nullptr;createN(&ntool);auto nb=ntool->GetBasicParam();for(int j=0;j<locate->GetCalibPointNum();j++){auto p=locate->GetCalibPointImageCoordinate(j);auto q=locate->GetCalibPointObjectCoordinate(j);nb->AddOffsetPoint(p,{(float)q.nX,(float)q.nY});}ntool->SetRunParam("HomoFreedom","1");ntool->SetRunParam("WeightFun","2");try{ntool->Run();printf("{\"phase\":\"npoint-native-points\",\"board\":%d,\"mode\":%d,\"code\":0,\"error\":%.9g}\n",board,mode,ntool->GetResult()->GetPlatformCalibInfo()->GetOffsetError());}catch(IMVDException&e){printf("{\"phase\":\"npoint-native-points\",\"board\":%d,\"mode\":%d,\"code\":%u}\n",board,mode,e.GetErrorCode());}destroyN(ntool);
}
 catch(IMVDException&e){printf("{\"board\":%d,\"mode\":%d,\"code\":%u}\n",board,mode,e.GetErrorCode());}catch(...){printf("{\"board\":%d,\"mode\":%d,\"unknown\":true}\n",board,mode);}
 imageDestroy(image);
 }destroy(tool);return 0;
}










