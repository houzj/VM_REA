#define NOMINMAX
#include <windows.h>
#include <cstdio>
#include <algorithm>
#include <cstring>
#include "MVDCore.h"
#include "MVDShapeCpp.h"
#include "PreproMaskCpp.h"
using namespace VisionDesigner;
using namespace VisionDesigner::PreproMask;
int main(int argc,char**argv){
 if(argc!=2)return 2;SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);SetDllDirectoryA(argv[1]);setvbuf(stdout,nullptr,_IOFBF,1024*1024);
 auto dll=LoadLibraryA("MVDPreproMaskCpp.dll"),shapes=LoadLibraryA("MVDShapeCpp.dll");if(!dll||!shapes)return 3;
 auto create=(int(__stdcall*)(IPreproMaskTool**))GetProcAddress(dll,"CreatePreproMaskToolInstance");auto destroy=(int(__stdcall*)(IPreproMaskTool*))GetProcAddress(dll,"DestroyPreproMaskToolInstance");
 auto rectangle=(int(__stdcall*)(IMvdRectangleF**,float,float,float,float))GetProcAddress(shapes,"CreateRectangleInstance");auto freeRectangle=(int(__stdcall*)(IMvdRectangleF*))GetProcAddress(shapes,"DestroyRectangleInstance");
 IPreproMaskTool*metadata=nullptr;if(create(&metadata))return 8;auto table=*(void***)metadata;fprintf(stderr,"runRva=%llx addRegionRva=%llx\n",(unsigned long long)table[4]-(unsigned long long)dll,(unsigned long long)table[5]-(unsigned long long)dll);destroy(metadata);
 using Rotate=void(*)(void*,unsigned long long,unsigned long long,float,float*);auto rotate=(Rotate)((unsigned char*)dll+0x3100);auto pack=[](float x,float y){float p[2]={x,y};unsigned long long bits;memcpy(&bits,p,8);return bits;};
 int index=0;for(float cx:{0.f,16.25f,32.f,63.75f})for(float width:{7.f,22.5f})for(float angle:{0.f,.0002f,15.f,30.f,45.f,90.f,-30.f,135.f}){
  IPreproMaskTool*tool=nullptr;IMvdRectangleF*r=nullptr;if(create(&tool)||rectangle(&r,cx,23.5f,width,13.25f))return 4;r->SetAngle(angle);tool->SetInputImageSize(64,48);tool->AddRegion(r,true);
  try{tool->Run();auto image=tool->GetOutputImage();auto data=image->GetImageData(0);int minx=64,miny=48,maxx=-1,maxy=-1,count=0;
   for(int y=0;y<48;y++)for(int x=0;x<64;x++)if(data->pData[y*data->nRowStep+x]){minx=std::min(minx,x);miny=std::min(miny,y);maxx=std::max(maxx,x);maxy=std::max(maxy,y);count++;}
   printf("{\"index\":%d,\"cx\":%.9g,\"cy\":23.5,\"width\":%.9g,\"height\":13.25,\"angle\":%.9g,\"code\":0,\"count\":%d,\"bounds\":[%d,%d,%d,%d],\"hex\":\"",index,cx,width,angle,count,minx,miny,maxx+1-minx,maxy+1-miny);
   for(int y=0;y<48;y++)for(int x=0;x<64;x++)printf("%02x",data->pData[y*data->nRowStep+x]);printf("\",\"vertices\":[");float corners[8]={cx-width*.5f,23.5f-13.25f*.5f,cx+width*.5f,23.5f-13.25f*.5f,cx+width*.5f,23.5f+13.25f*.5f,cx-width*.5f,23.5f+13.25f*.5f};for(int i=0;i<4;i++){float point[2]={0,0};rotate(tool,pack(corners[2*i],corners[2*i+1]),pack(cx,23.5f),angle,point);printf("%s[%.9g,%.9g]",i?",":"",point[0],point[1]);}printf("]}\n");
  }catch(IMVDException&e){printf("{\"index\":%d,\"code\":%u}\n",index,e.GetErrorCode());}tool->ClearRegions();freeRectangle(r);destroy(tool);index++;
 }
 return 0;
}
