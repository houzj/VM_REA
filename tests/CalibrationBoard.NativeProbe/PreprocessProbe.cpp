// Source-addressed pure image helpers; normal public SDK creation, owned images, no platform/device.
#include <windows.h>
#include <algorithm>
#include <cstdio>
#include <vector>
#include <cmath>
#include "MVDCore.h"
#include "CalibBoardCalibCpp.h"
using namespace VisionDesigner::CalibBoardCalib;
static unsigned long long dimensions(unsigned w,unsigned h){return w|((unsigned long long)h<<32);}
static void save(const char*name,const std::vector<unsigned char>&bytes){FILE*f=nullptr;fopen_s(&f,name,"wb");if(f){fwrite(bytes.data(),1,bytes.size(),f);fclose(f);}}
int main(int argc,char**argv){
 if(argc!=2)return 2;SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);setvbuf(stdout,nullptr,_IONBF,0);SetDllDirectoryA(argv[1]);
 HMODULE sdk=LoadLibraryA("MVDCalibBoardCalibCpp.dll");if(!sdk)return 3;
 auto create=(int(__stdcall*)(ICalibBoardCalibTool**))GetProcAddress(sdk,"CreateCalibBoardCalibToolInstance");
 auto destroy=(int(__stdcall*)(ICalibBoardCalibTool*))GetProcAddress(sdk,"DestroyCalibBoardCalibToolInstance");
 ICalibBoardCalibTool*tool=nullptr;if(create(&tool))return 4;
 auto core=(unsigned char*)GetModuleHandleA("MVD_Algorithm.dll");if(!core)return 5;
 using Resize=void(*)(const unsigned char*,int,unsigned long long,unsigned char*,int,unsigned long long,int,void*);
 using Otsu=void(*)(const unsigned char*,int,unsigned char*,int,unsigned long long);
 auto resize=(Resize)(core+0x727a50);auto otsu=(Otsu)(core+0x724460);
 auto alternateOtsu=(Otsu)(core+0x87db00);int featureCpu=-1;((void(*)(int*))(core+0x62f260))(&featureCpu);printf("{\"phase\":\"feature-dispatch\",\"value\":%d}\n",featureCpu);
 auto cpu=(int(*)())(core+0x630120);printf("{\"phase\":\"cpu-dispatch\",\"value\":%d}\n",cpu());
 std::vector<unsigned char>scratch(480*4096),input(480*400),half(240*200),mask(240*200),rawMask(480*400);
 for(int factor:{2,5})for(int shape:{0,1,2})for(int contrast:{2,5,10,20,50,255}){
  int outWidth=480/factor,outHeight=400/factor;half.resize(outWidth*outHeight);mask.resize(outWidth*outHeight);
  std::fill(input.begin(),input.end(),255);
  for(int row=0;row<8;row++)for(int col=0;col<10;col++)for(int dy=-9;dy<=9;dy++)for(int dx=-9;dx<=9;dx++){
   bool inside=shape==0?dx*dx+dy*dy<81:shape==1?dx*dx/81.f+dy*dy/25.f<1:abs(dx)<9&&abs(dy)<9;
   if(inside)input[(80+row*30+dy)*480+90+col*30+dx]=(unsigned char)(255-contrast);
  }
  resize(input.data(),480,dimensions(480,400),half.data(),outWidth,dimensions(outWidth,outHeight),2,scratch.data());
  otsu(half.data(),outWidth,mask.data(),outWidth,dimensions(outWidth,outHeight));
  char prefix[100],name[128];if(factor==2)sprintf_s(prefix,"shape-%d-contrast-%d",shape,contrast);else sprintf_s(prefix,"shape-%d-contrast-%d-factor-%d",shape,contrast,factor);sprintf_s(name,"%s-half.bin",prefix);save(name,half);
  sprintf_s(name,"%s-mask.bin",prefix);save(name,mask);
  otsu(input.data(),480,rawMask.data(),480,dimensions(480,400));sprintf_s(name,"%s-raw-mask.bin",prefix);save(name,rawMask);
  alternateOtsu(input.data(),480,rawMask.data(),480,dimensions(480,400));sprintf_s(name,"%s-alternate-raw-mask.bin",prefix);save(name,rawMask);
  alternateOtsu(half.data(),outWidth,mask.data(),outWidth,dimensions(outWidth,outHeight));sprintf_s(name,"%s-alternate-mask.bin",prefix);save(name,mask);
  otsu(half.data(),outWidth,mask.data(),outWidth,dimensions(outWidth,outHeight));
  unsigned histogram[256]={},black=0;for(unsigned char pixel:half)histogram[pixel]++;for(unsigned char pixel:mask)if(!pixel)black++;
  printf("{\"factor\":%d,\"shape\":%d,\"contrast\":%d,\"blackPixels\":%u,\"histogram\":[",factor,shape,contrast,black);
  bool first=true;for(int i=0;i<256;i++)if(histogram[i]){printf("%s[%d,%u]",first?"":",",i,histogram[i]);first=false;}printf("]}\n");
 }
 for(int width:{64,65,66,80,96,126,128,130,160,192})for(int pattern:{0,1,2}){
  int height=width+2,outWidth=width/2,outHeight=height/2;
  std::vector<unsigned char>source(width*height),target(outWidth*outHeight);unsigned seed=1;
  for(int y=0;y<height;y++)for(int x=0;x<width;x++){
   seed=seed*1664525u+1013904223u;
   source[y*width+x]=pattern==0?(unsigned char)((x*3+y*5)%256):pattern==1?(x==0&&y==0?0:255):(unsigned char)(seed>>24);
  }
  resize(source.data(),width,dimensions(width,height),target.data(),outWidth,dimensions(outWidth,outHeight),2,scratch.data());
  char name[128];sprintf_s(name,"pattern-%d-width-%d-input.bin",pattern,width);save(name,source);
  sprintf_s(name,"pattern-%d-width-%d-half.bin",pattern,width);save(name,target);
  printf("{\"phase\":\"boundary\",\"pattern\":%d,\"width\":%d,\"height\":%d,\"outWidth\":%d,\"outHeight\":%d}\n",pattern,width,height,outWidth,outHeight);
 }
 destroy(tool);return 0;
}

