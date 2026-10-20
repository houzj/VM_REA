#include <windows.h>
#include <cstdio>
#include <vector>
#include "MVDCore.h"
#include "CalibBoardCalibCpp.h"
using namespace VisionDesigner::CalibBoardCalib;
static unsigned long long dims(unsigned w,unsigned h){return w|((unsigned long long)h<<32);}
int main(int argc,char**argv){if(argc!=2)return 2;SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);SetDllDirectoryA(argv[1]);setvbuf(stdout,nullptr,_IOFBF,1024*1024);auto sdk=LoadLibraryA("MVDCalibBoardCalibCpp.dll");auto create=(int(__stdcall*)(ICalibBoardCalibTool**))GetProcAddress(sdk,"CreateCalibBoardCalibToolInstance");auto destroy=(int(__stdcall*)(ICalibBoardCalibTool*))GetProcAddress(sdk,"DestroyCalibBoardCalibToolInstance");ICalibBoardCalibTool*tool=nullptr;if(create(&tool))return 3;auto core=(unsigned char*)GetModuleHandleA("MVD_Algorithm.dll");
 using Wrapper=unsigned long long(*)(const unsigned char*,int,unsigned long long,int,const unsigned char*,int,unsigned long long,void*,int*,int*,float*,unsigned long long,void*);
 auto wrapper=(Wrapper)(core+0x877f90);std::vector<unsigned char>scratch(64*1024*1024),output(90000*36);float parameters[3]={.5f,0,0};
 for(int factor:{2,3,4,5,6})for(int delta:{-1,0,1}){int w=80*factor,h=64*factor+delta,sw=w/factor,sh=h/factor;std::vector<unsigned char>original(w*h,255),reduced(sw*sh,255);int count=0,grid[2]={0,0};auto code=wrapper(reduced.data(),sw,dims(sw,sh),factor,original.data(),w,dims(w,h),output.data(),&count,grid,parameters,dims(10,8),scratch.data());printf("{\"factor\":%d,\"width\":%d,\"height\":%d,\"reducedWidth\":%d,\"reducedHeight\":%d,\"code\":%u,\"count\":%d}\n",factor,w,h,sw,sh,(unsigned)code,count);}
 destroy(tool);return 0;}
