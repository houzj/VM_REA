// Original algorithm SDK in an owned process; no devices or license workarounds.
#include <windows.h>
#include <cstdio>
#include "NPointCalibCpp.h"
using namespace VisionDesigner::NPointCalib;
int main(int argc,char** argv) {
 if(argc!=3)return 2;
 HMODULE lib=LoadLibraryA(argv[1]);if(!lib){printf("{\"phase\":\"load\",\"error\":%lu}\n",GetLastError());return 3;}
 auto create=(int(__stdcall*)(INPointCalibTool**))GetProcAddress(lib,"CreateNPointCalibToolInstance");
 auto destroy=(int(__stdcall*)(INPointCalibTool*))GetProcAddress(lib,"DestroyNPointCalibToolInstance");
 INPointCalibTool* tool=nullptr;int code=create(&tool);printf("{\"phase\":\"create\",\"code\":%u}\n",(unsigned)code);if(code)return 4;
 try {
 auto basic=tool->GetBasicParam();basic->ClearOffsetPoints();basic->ClearRotatePoints();
 for(int i=0;i<9;i++){float x=(i%3)*10.f,y=(i/3)*10.f;basic->AddOffsetPoint({x,y},{2*x+10,2*y-5});}
 tool->SetRunParam("HomoFreedom","2");tool->SetRunParam("WeightFun","1");tool->Run();
 tool->ExportCalibFile(argv[2]);auto result=tool->GetResult();auto platform=result->GetPlatformCalibInfo();auto param=result->GetCalibMatrixToParamInfo();float h[9];platform->GetHomoMatrix(h);
 printf("{\"phase\":\"run\",\"status\":%d,\"error\":%.9g,\"scale\":%.9g,\"x\":%.9g,\"y\":%.9g,\"matrix\":[",platform->GetStatus(),platform->GetOffsetError(),param->GetScale(),param->GetOffsetX(),param->GetOffsetY());for(int i=0;i<9;i++)printf("%s%.9g",i?",":"",h[i]);printf("]}\n");
 }catch(int e){printf("{\"phase\":\"exception\",\"code\":%u}\n",(unsigned)e);}catch(...){printf("{\"phase\":\"unknown-exception\"}\n");}
 destroy(tool);return 0;
}
