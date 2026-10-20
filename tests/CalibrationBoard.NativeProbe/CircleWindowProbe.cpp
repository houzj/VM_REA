#include <windows.h>
#include <cstdio>
#include <vector>
#include "MVDCore.h"
#include "CalibBoardCalibCpp.h"
using namespace VisionDesigner::CalibBoardCalib;
struct Candidate{float x,y;int fit,row,col,id;float size;int tail1,tail2;};static_assert(sizeof(Candidate)==36);
int main(int argc,char**argv){if(argc!=2)return 2;SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);SetDllDirectoryA(argv[1]);auto sdk=LoadLibraryA("MVDCalibBoardCalibCpp.dll");auto create=(int(*)(ICalibBoardCalibTool**))GetProcAddress(sdk,"CreateCalibBoardCalibToolInstance");auto destroy=(int(*)(ICalibBoardCalibTool*))GetProcAddress(sdk,"DestroyCalibBoardCalibToolInstance");ICalibBoardCalibTool*tool=nullptr;if(create(&tool))return 3;auto core=(unsigned char*)GetModuleHandleA("MVD_Algorithm.dll");using Window=void(*)(void*,unsigned,unsigned long long,void*,unsigned*,int*,void*);auto window=(Window)(core+0x8cfb40);std::vector<unsigned char>scratch(16*1024*1024);int index=0;
 for(int width:{299,300,301,599,600,601,20})for(int missing:{0,1,2}){int height=width==20?10:3,lw=width==20?8:300,lh=width==20?6:300;std::vector<Candidate>input,output(2500);for(int y=0;y<height;y++)for(int x=0;x<width;x++){if(missing==1&&x<width/2&&(x+y)%3==0||missing==2&&(x+y)%7==0)continue;input.push_back({x*30.f,y*30.f,1,y-7,x-11,(int)input.size()+1,8.5f,0,0});}printf("{\"index\":%d,\"limit\":[%d,%d],\"inputs\":[",index++,lw,lh);for(size_t i=0;i<input.size();i++)printf("%s[%d,%d,%d]",i?",":"",input[i].id,input[i].col,input[i].row);unsigned count=0;int dimensions[2]={};window(input.data(),(unsigned)input.size(),(unsigned long long)lw|((unsigned long long)lh<<32),output.data(),&count,dimensions,scratch.data());if(count>2500)return 4;printf("],\"dimensions\":[%d,%d],\"outputs\":[",dimensions[0],dimensions[1]);for(unsigned i=0;i<count;i++)printf("%s[%d,%d,%d]",i?",":"",output[i].id,output[i].col,output[i].row);printf("]}\n");}
 destroy(tool);return 0;}
