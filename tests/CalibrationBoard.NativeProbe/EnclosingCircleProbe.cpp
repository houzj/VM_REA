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
 int index=0;for(int shape=0;shape<8;shape++)for(int count:{1,2,3,4,5,8,16,32,64}){std::vector<Point>points;for(int i=0;i<count;i++){state=1664525*state+1013904223;int x=(int)((state>>16)%101)-50;state=1664525*state+1013904223;int y=(int)((state>>16)%101)-50;if(shape==1){x=i*3-30;y=7;}if(shape==2){x=i%2?40:-20;y=(i/2)%2?30:-10;}if(shape==3){x=(int)(30*cos(i*6.283185307179586/count));y=(int)(20*sin(i*6.283185307179586/count));}if(shape==4){x=i<2?0:i*2;y=i<2?0:i*3;}if(shape==5){x=i%2?0:i*2;y=i%2?i*3:0;}if(shape==6){x=i%2?100000+i*7:100001-i*4;y=200000+i%3;}if(shape==7){x=i%2?6:7;y=i%2?9:8;}points.push_back({x,y});}float center[2]={},radius=0;circle(points.data(),count,center,&radius);unsigned bits[3];memcpy(bits,center,8);memcpy(bits+2,&radius,4);printf("{\"index\":%d,\"shape\":%d,\"points\":[",index++,shape);for(int i=0;i<count;i++)printf("%s[%d,%d]",i?",":"",points[i].x,points[i].y);printf("],\"resultBits\":[%u,%u,%u]}\n",bits[0],bits[1],bits[2]);fflush(stdout);}
 destroy(tool);return 0;}
