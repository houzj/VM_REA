#include <windows.h>
#include <cstdio>
#include <vector>
#include <cmath>
#include <cstring>
#include "MVDCore.h"
#include "CalibBoardCalibCpp.h"
using namespace VisionDesigner::CalibBoardCalib;
struct Point{int x,y;};
int main(int argc,char**argv){if(argc!=2)return 2;SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);SetDllDirectoryA(argv[1]);auto sdk=LoadLibraryA("MVDCalibBoardCalibCpp.dll");auto create=(int(*)(ICalibBoardCalibTool**))GetProcAddress(sdk,"CreateCalibBoardCalibToolInstance");auto destroy=(int(*)(ICalibBoardCalibTool*))GetProcAddress(sdk,"DestroyCalibBoardCalibToolInstance");ICalibBoardCalibTool*tool=nullptr;if(!sdk||!create||!destroy||create(&tool))return 3;using Circle=void(*)(Point*,int,float*,float*);auto circle=(Circle)((unsigned char*)GetModuleHandleA("MVD_Algorithm.dll")+0x8a9f50);unsigned state=0x183e7a;
auto core=(unsigned char*)GetModuleHandleA("MVD_Algorithm.dll");for(unsigned address:{0x1f43640u,0x16476a0u,0x16476f0u}){int count=address==0x16476f0?16:16;printf("{\"rva\":%u,\"words\":[",address);for(int i=0;i<count;i++){unsigned value;memcpy(&value,core+address+i*4,4);printf("%s%u",i?",":"",value);}printf("]}\n");}
 destroy(tool);return 0;}
