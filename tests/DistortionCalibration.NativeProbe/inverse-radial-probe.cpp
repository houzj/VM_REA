// Read-only direct original inverse-radial kernel probe on already-exported nineteen-float SDK files.
#include <windows.h>
#include <cstdio>
#include <cstring>
#include <vector>
#include <cstdint>
int main(int argc,char**argv){if(argc!=2)return 2;SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);SetDllDirectoryA(argv[1]);auto lib=LoadLibraryA("MVD_Algorithm.dll");if(!lib)return 3;using Inverse=void(*)(uint64_t,float*,uint64_t,float*,void*);auto inverse=(Inverse)((char*)lib+0x857a60);std::vector<unsigned char>scratch(1024*1024+128);void*aligned=(void*)(((uintptr_t)scratch.data()+127)&~(uintptr_t)127);
 for(int board:{1,2})for(int model:{2,3})for(int warp:{0,1}){char path[256];sprintf_s(path,"../pixels/board-%d-model-%d-warp-%d-eval-1.iccal",board,model,warp);FILE*f=fopen(path,"rb");if(!f)return 4;float h[19];if(fread(h,4,19,f)!=19)return 5;fclose(f);float radial[]={h[12],h[13],h[14],h[15]};uint64_t center;memcpy(&center,h+10,8);
 for(float y:{30.f,100.f,200.f,300.f,370.f})for(float x:{30.f,120.f,240.f,360.f,450.f}){float p[]={x,y},out[]={-999.f,-999.f};uint64_t point;memcpy(&point,p,8);memset(aligned,0,1024*1024);inverse(point,radial,center,out,aligned);printf("{\"board\":%d,\"model\":%d,\"warp\":%d,\"input\":[%.9g,%.9g],\"center\":[%.9g,%.9g],\"radial\":[%.9g,%.9g,%.9g,%.9g],\"ideal\":[%.9g,%.9g]}\n",board,model,warp,x,y,h[10],h[11],radial[0],radial[1],radial[2],radial[3],out[0],out[1]);}}
}
