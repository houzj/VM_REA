#include <windows.h>
#include <cstdio>
#include <vector>
#include <cstring>
#include "MVDCore.h"
#include "CalibBoardCalibCpp.h"
using namespace VisionDesigner::CalibBoardCalib;
int main(int argc,char**argv){if(argc!=2)return 2;SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);SetDllDirectoryA(argv[1]);auto sdk=LoadLibraryA("MVDCalibBoardCalibCpp.dll");auto create=(int(*)(ICalibBoardCalibTool**))GetProcAddress(sdk,"CreateCalibBoardCalibToolInstance");auto destroy=(int(*)(ICalibBoardCalibTool*))GetProcAddress(sdk,"DestroyCalibBoardCalibToolInstance");ICalibBoardCalibTool*tool=nullptr;if(!create||create(&tool))return 3;using Circle=int(*)(unsigned char*,int,unsigned long long,float*,int,int,void*);auto circle=(Circle)((unsigned char*)GetModuleHandleA("MVD_Algorithm.dll")+0x877b20);std::vector<unsigned char>scratch(8*1024*1024),image(1024);
 for(int shape=0;shape<4;shape++){std::fill(image.begin(),image.end(),255);for(int y=0;y<32;y++)for(int x=0;x<32;x++)if(shape==0&&x>=1&&x<=17&&y>=8&&y<=24||shape==1&&x>=1&&x<=17&&y>=8&&y<=24||shape==3&&x<9)image[y*32+x]=shape==1?250:0;float center[2]={123.25f,-77.5f};int code=circle(image.data(),32,32ull|(32ull<<32),center,127,255,scratch.data());unsigned bits[2];memcpy(bits,center,8);printf("{\"shape\":%d,\"code\":%d,\"center\":[%.9g,%.9g],\"bits\":[%u,%u]}\n",shape,code,center[0],center[1],bits[0],bits[1]);fflush(stdout);}
 destroy(tool);return 0;}
