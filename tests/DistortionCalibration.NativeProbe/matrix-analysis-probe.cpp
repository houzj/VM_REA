// Owned, non-device original core/SDK controlled matrix comparison.
#include <windows.h>
#include <cstdio>
#include <vector>
#include <cstring>
#include <cmath>
#include "MVDCore.h"
#include "MatrixToParamCpp.h"
using namespace VisionDesigner;using namespace VisionDesigner::MatrixToParam;
int main(int argc,char**argv){
 if(argc!=3)return 2;SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);SetDllDirectoryA(argv[1]);
 auto core=LoadLibraryA("MVD_Algorithm.dll"),sdk=LoadLibraryA("MVDMatrixToParamCpp.dll");if(!core||!sdk)return 3;
 auto create=(int(*)(IMatrixToParamTool**))GetProcAddress(sdk,"CreateMatrixToParamToolInstance");
 auto destroy=(int(*)(IMatrixToParamTool*))GetProcAddress(sdk,"DestroyMatrixToParamToolInstance");
 auto decompose=(int(*)(float*,float*))((char*)core+0x800af0);
 FILE*input=fopen(argv[2],"rb");if(!input)return 4;float h[9];int sequence=0;
 while(fread(h,4,9,input)==9){
  float raw[9],values[6]={};memcpy(raw,h,sizeof(h));int code=decompose(raw,values);
  printf("{\"sequence\":%d,\"matrix\":[",sequence++);for(int i=0;i<9;i++)printf(i?",%.9g":"%.9g",h[i]);
  printf("],\"coreCode\":%d,\"coreValues\":[",code);for(int i=0;i<6;i++)printf(i?",%.9g":"%.9g",values[i]);
  printf("],\"coreInputPreserved\":%s",memcmp(raw,h,sizeof(h))==0?"true":"false");
  IMatrixToParamTool*tool=nullptr;unsigned error=0;float sv[6]={};
  if(create(&tool))return 5;
  try{tool->ImportHomoMatrix(h);tool->Run();auto r=tool->GetResult();if(!r)error=0xffffffff;else{sv[0]=r->GetScale();sv[1]=r->GetOffsetX();sv[2]=r->GetOffsetY();sv[3]=r->GetRotate();sv[4]=r->GetAspect();sv[5]=r->GetSkew();}}
  catch(IMVDException&e){error=e.GetErrorCode();}
  printf(",\"sdkError\":%u,\"sdkValues\":[",error);for(int i=0;i<6;i++)printf(i?",%.9g":"%.9g",sv[i]);printf("]}\n");
  destroy(tool);
 }
 auto atan2native=(float(*)(float,float))((char*)core+0x157243c);unsigned negativeZeroBits=0x80000000;float negativeZero;memcpy(&negativeZero,&negativeZeroBits,4);for(float y:{0.f,negativeZero})for(float x:{1.f,-1.f,0.f,negativeZero}){float value=atan2native(y,x);unsigned bits;memcpy(&bits,&value,4);printf("{\"phase\":\"atan2-zero\",\"negativeY\":%s,\"negativeX\":%s,\"x\":%.9g,\"bits\":%u}\n",std::signbit(y)?"true":"false",std::signbit(x)?"true":"false",x,bits);}fclose(input);return 0;
}
