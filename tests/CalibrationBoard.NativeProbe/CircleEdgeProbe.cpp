#include <windows.h>
#include <algorithm>
#include <cstdio>
#include <cstring>
#include <vector>
#include <cmath>
#include "MVDCore.h"
#include "CalibBoardCalibCpp.h"
using namespace VisionDesigner::CalibBoardCalib;
static unsigned long long dims(unsigned w,unsigned h){return w|((unsigned long long)h<<32);}
static void save(const char*path,const void*data,size_t length){FILE*f=nullptr;fopen_s(&f,path,"wb");if(f){fwrite(data,1,length,f);fclose(f);}}
int main(int argc,char**argv){
 if(argc<2||argc>3)return 2;SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);setvbuf(stdout,nullptr,_IOFBF,1024*1024);SetDllDirectoryA(argv[1]);
 auto sdk=LoadLibraryA("MVDCalibBoardCalibCpp.dll");if(!sdk)return 3;
 auto create=(int(__stdcall*)(ICalibBoardCalibTool**))GetProcAddress(sdk,"CreateCalibBoardCalibToolInstance");auto destroy=(int(__stdcall*)(ICalibBoardCalibTool*))GetProcAddress(sdk,"DestroyCalibBoardCalibToolInstance");
 ICalibBoardCalibTool*tool=nullptr;if(create(&tool))return 4;auto core=(unsigned char*)GetModuleHandleA("MVD_Algorithm.dll");if(!core)return 5;
 using Sobel=void(*)(const unsigned char*,int,short*,int,unsigned long long);
 using Circle=int(*)(const unsigned char*,int,unsigned long long,float*,int,int,void*);
 using Edges=int(*)(const unsigned char*,int,unsigned long long,unsigned long long*,int*,float*,long long*,void*);
 using Nms=void(*)(const short*,const short*,const float*,int,unsigned long long,float,float,void*);
 using Links=void(*)(const short*,const short*,int,void*,int*,int*);
 auto gx=(Sobel)(core+0x6a7570),gy=(Sobel)(core+0x6a6c50);auto circle=(Circle)(core+0x877b20);auto edges=(Edges)(core+0x8b3d90);auto refinedEdges=(Edges)(core+0x8609a0);
 std::vector<unsigned char>input(32*32),scratch(4*1024*1024);std::vector<long long>storage(256*1024);std::vector<short>x(32*32),y(32*32);
 FILE*nmsFile=nullptr;fopen_s(&nmsFile,"nms.jsonl","wb");if(!nmsFile)return 8;
 auto nms=(Nms)(core+0x8b30c0);auto links=(Links)(core+0x8b1b10);std::vector<float>magnitude(1024),nmsPoints(2048);std::vector<int>flags(1024),previous(1024),next(1024);
 int thresholds[6][2]={{0,1},{20,40},{100,200},{127,255},{0,0},{20,20}};
 for(int shape:{0,1,2})for(int contrast:{2,5,10,20,50,255}){
  std::fill(input.begin(),input.end(),255);for(int dy=-9;dy<=9;dy++)for(int dx=-9;dx<=9;dx++){
   bool inside=shape==0?dx*dx+dy*dy<81:shape==1?dx*dx/81.f+dy*dy/25.f<1:abs(dx)<9&&abs(dy)<9;if(inside)input[(16+dy)*32+16+dx]=(unsigned char)(255-contrast);
  }
  gx(input.data(),32,x.data(),32,dims(32,32));gy(input.data(),32,y.data(),32,dims(32,32));char path[128];
  sprintf_s(path,"shape-%d-contrast-%d-gx.bin",shape,contrast);save(path,x.data(),x.size()*2);sprintf_s(path,"shape-%d-contrast-%d-gy.bin",shape,contrast);save(path,y.data(),y.size()*2);
  for(int i=0;i<1024;i++)magnitude[i]=sqrtf((float)x[i]*x[i]+(float)y[i]*y[i]);
  for(int t=0;t<4;t++){
   unsigned long long header[3]={},layout[10]={};layout[4]=(unsigned long long)flags.data();layout[7]=(unsigned long long)header;layout[8]=(unsigned long long)nmsPoints.data();
   nms(x.data(),y.data(),magnitude.data(),32,dims(32,32),thresholds[t][1]*4.4721360206604f,thresholds[t][0]*4.4721360206604f,layout);
   unsigned count=(unsigned)header[2];if(count>1024)return 9;
   fprintf(nmsFile,"{\"shape\":%d,\"contrast\":%d,\"low\":%d,\"high\":%d,\"points\":[",shape,contrast,thresholds[t][0],thresholds[t][1]);
   for(unsigned i=0;i<count;i++)fprintf(nmsFile,"%s[%.9g,%.9g]",i?",":"",nmsPoints[i*2],nmsPoints[i*2+1]);
   links(x.data(),y.data(),32,header,previous.data(),next.data());fprintf(nmsFile,"],\"previousPixels\":[");for(unsigned i=0;i<count;i++)fprintf(nmsFile,"%s%d",i?",":"",previous[i]);
   fprintf(nmsFile,"],\"nextPixels\":[");for(unsigned i=0;i<count;i++)fprintf(nmsFile,"%s%d",i?",":"",next[i]);fprintf(nmsFile,"]}\n");
  }
  if(argc==3)continue;
  for(int stage=0;stage<2;stage++)for(auto&threshold:thresholds){
   int low=threshold[0],high=threshold[1],automatic=low==high;float setting[3]={automatic?0.f:(float)high,automatic?0.f:(float)low,0};memcpy(setting+2,&automatic,4);
   unsigned long long handle=0;int found=0;int code=(stage?refinedEdges:edges)(input.data(),32,dims(32,32),&handle,&found,setting,storage.data(),scratch.data());
   printf("{\"stage\":%d,\"shape\":%d,\"contrast\":%d,\"low\":%d,\"high\":%d,\"edgeCode\":%d,\"found\":%d,\"contours\":[",stage,shape,contrast,low,high,code,found);
   if(found&&handle){auto header=(unsigned long long*)handle;auto records=(unsigned char*)header[0];unsigned count=(unsigned)header[1];if(count>1024)return 6;
    for(unsigned i=0;i<count;i++){auto r=records+i*24;int n=*(int*)(r+12);auto points=*(float**)(r+16);if(n<0||n>4096)return 7;printf("%s{\"features\":[%.9g,%.9g,%.9g],\"points\":[",i?",":"",*(float*)r,*(float*)(r+4),*(float*)(r+8));for(int j=0;j<n;j++)printf("%s[%.9g,%.9g]",j?",":"",points[j*2],points[j*2+1]);printf("]}");}
   }
   float center[2]={-999,-999};int fit=stage?-1:circle(input.data(),32,dims(32,32),center,low,high,scratch.data());printf("],\"circleCode\":%d,\"center\":[%.9g,%.9g]}\n",fit,center[0],center[1]);
  }
 }
 fclose(nmsFile);
 using Fallback=void(*)(const unsigned char*,int,unsigned long long,float*,float*,void*);auto fallback=(Fallback)(core+0x8620c0);
 FILE*fallbackFile=nullptr;fopen_s(&fallbackFile,"fallback.jsonl","wb");if(!fallbackFile)return 10;
 for(int width:{28,32,37})for(int pattern:{0,1,2}){
  int height=width-3;std::vector<unsigned char>mask(width*height,255);
  for(int y=1;y<height-1;y++)for(int x=1;x<width-1;x++)if(pattern==1||(pattern==2&&((x*7+y*13)%5<3)))mask[y*width+x]=0;
  float center[2]={-999,-999},radius=-999;fallback(mask.data(),width,dims(width,height),center,&radius,scratch.data());
  fprintf(fallbackFile,"{\"width\":%d,\"height\":%d,\"pattern\":%d,\"center\":[%.9g,%.9g],\"radius\":%.9g}\n",width,height,pattern,center[0],center[1],radius);
 }
 fclose(fallbackFile);destroy(tool);return 0;
}
