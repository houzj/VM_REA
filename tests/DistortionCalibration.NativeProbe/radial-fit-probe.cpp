#include <windows.h>
#include <cstdio>
#include <cstring>
#include <vector>
#include <cstdint>
int main(int argc,char**argv){if(argc!=2)return 2;SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);SetDllDirectoryA(argv[1]);auto lib=LoadLibraryA("MVD_Algorithm.dll");if(!lib)return 3;using Fit=void(*)(void*,void*,int,uint64_t,float*,float*,void*,void*);auto fit=(Fit)((char*)lib+0x853820);std::vector<unsigned char>scratch(4*1024*1024+128);void*aligned=(void*)(((uintptr_t)scratch.data()+127)&~(uintptr_t)127);
for(int index=0;index<8;index++){char path[80];sprintf_s(path,"case-%d.bin",index);FILE*f=fopen(path,"rb");if(!f)return 4;int n;fread(&n,4,1,f);std::vector<int>world(n*2);std::vector<float>observed(n*2);fread(world.data(),4,n*2,f);fread(observed.data(),4,n*2,f);fclose(f);alignas(16) unsigned char options[128]={};*(int*)(options+0x14)=2;*(int*)(options+0x1c)=1;*(float*)(options+0x20)=2;float radial[4]={-999,-999,-999,-999},center[2]={-999,-999};memset(aligned,0,4*1024*1024);fit(world.data(),observed.data(),n,((uint64_t)400<<32)|480,radial,center,options,aligned);printf("{\"case\":%d,\"count\":%d,\"radial\":[%.9g,%.9g,%.9g,%.9g],\"center\":[%.9g,%.9g]}\n",index,n,radial[0],radial[1],radial[2],radial[3],center[0],center[1]);fflush(stdout);}}

