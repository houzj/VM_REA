#include <windows.h>
#include <cstdio>
#include <vector>
#include <algorithm>
#include <cstring>
#include "MVDCore.h"
#include "CalibBoardCalibCpp.h"
using namespace VisionDesigner::CalibBoardCalib;
static unsigned long long dims(unsigned w,unsigned h){return w|((unsigned long long)h<<32);}
int main(int argc,char**argv){
 if(argc<2||argc>3)return 2;int group=argc==3?atoi(argv[2]):-1;SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);setvbuf(stdout,nullptr,_IOFBF,1024*1024);SetDllDirectoryA(argv[1]);
 auto sdk=LoadLibraryA("MVDCalibBoardCalibCpp.dll");if(!sdk)return 3;
 auto create=(int(__stdcall*)(ICalibBoardCalibTool**))GetProcAddress(sdk,"CreateCalibBoardCalibToolInstance");auto destroy=(int(__stdcall*)(ICalibBoardCalibTool*))GetProcAddress(sdk,"DestroyCalibBoardCalibToolInstance");
 ICalibBoardCalibTool*tool=nullptr;if(create(&tool))return 4;auto core=(unsigned char*)GetModuleHandleA("MVD_Algorithm.dll");if(!core)return 5;
 tool->SetRunParam("HomoFreedom","1");
 using Resize=void(*)(const unsigned char*,int,unsigned long long,unsigned char*,int,unsigned long long,int,void*);
 using Candidates=int(*)(const unsigned char*,int,unsigned long long,int,const unsigned char*,int,unsigned long long,void*,int*,float*,unsigned long long,void*);
 auto resize=(Resize)(core+0x727a50);auto candidates=(Candidates)(core+0x877230);
 std::vector<unsigned char>image(480*400),half(240*200),scratch(64*1024*1024),records(90000*36);
 int caseIndex=0;for(int shape:{0,1,2})for(int contrast:{2,5,10,20,50,255})for(int circularity:{20,50,70,90,100})for(int high:{1,40,200,255}){
  int index=caseIndex++,factor=5,sampling=0,low=high/2;if(group<0){if(shape||circularity!=50||(contrast!=2&&contrast!=5&&contrast!=255)||(high!=1&&high!=40))continue;}else if(index/10!=group)continue;
  std::fill(image.begin(),image.end(),255);std::fill(records.begin(),records.end(),0);
  for(int row=0;row<8;row++)for(int col=0;col<10;col++)for(int dy=-9;dy<=9;dy++)for(int dx=-9;dx<=9;dx++){bool inside=shape==0?dx*dx+dy*dy<81:shape==1?dx*dx/81.f+dy*dy/25.f<1:abs(dx)<9&&abs(dy)<9;if(inside)image[(80+row*30+dy)*480+90+col*30+dx]=(unsigned char)(255-contrast);}
  int halfWidth=480/factor,halfHeight=400/factor;
  if(sampling)for(int y=0;y<halfHeight;y++)for(int x=0;x<halfWidth;x++)half[y*halfWidth+x]=image[y*factor*480+x*factor];
  else resize(image.data(),480,dims(480,400),half.data(),halfWidth,dims(halfWidth,halfHeight),2,scratch.data());
  int count=0;float parameters[3]={circularity*.01f,0,0};memcpy(parameters+1,&low,4);memcpy(parameters+2,&high,4);
  int code=candidates(half.data(),halfWidth,dims(halfWidth,halfHeight),factor,image.data(),480,dims(480,400),records.data(),&count,parameters,dims(10,8),scratch.data());
  printf("{\"index\":%d,\"shape\":%d,\"circularity\":%d,\"factor\":%d,\"sampling\":%d,\"contrast\":%d,\"low\":%d,\"high\":%d,\"code\":%d,\"count\":%d,\"records\":[",index,shape,circularity,factor,sampling,contrast,low,high,code,count);
  if(count<0||count>90000)return 6;for(int i=0;i<count;i++){auto r=(float*)(records.data()+i*36);printf("%s[",i?",":"");for(int j=0;j<9;j++)printf("%s%.9g",j?",":"",r[j]);printf("]");}printf("]}\n");
 }
 destroy(tool);return 0;
}
