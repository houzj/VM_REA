#include <windows.h>
#include <intrin.h>
#include <cstdio>
#include <vector>
#include <cstdint>
using Kernel=void(__cdecl*)(unsigned char*,int,uint64_t,unsigned char*,int*);
static DWORD WINAPI deadline(void* ready){SetEvent((HANDLE)ready);Sleep(20000);std::fputs("timeout20s\n",stderr);std::fflush(stderr);TerminateProcess(GetCurrentProcess(),124);return 124;}
int main(){
 SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX|SEM_NOOPENFILEERRORBOX);
 HANDLE ready=CreateEventW(nullptr,TRUE,FALSE,nullptr);HANDLE t=CreateThread(nullptr,0,deadline,ready,0,nullptr);if(!t||WaitForSingleObject(ready,1000)!=WAIT_OBJECT_0)return 10;
 int cpu[4];__cpuid(cpu,1);if(!(cpu[2]&(1<<19)))return 12;
 SetDllDirectoryW(L"D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64");
 HMODULE h=LoadLibraryW(L"D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll");if(!h)return 2;
 auto base=(unsigned char*)h;Kernel ks[]={(Kernel)(base+0xd46a10),(Kernel)(base+0xd472b0),(Kernel)(base+0xc96f20)};
 std::printf("{\"scope\":\"one65x65tightchain; pure leaves; no initialization\",\"chains\":[");
 for(int k=0;k<3;k++){
  int w=65,hh=65;std::vector<unsigned char> input(w*hh+128,237);
  for(int y=0;y<hh;y++)for(int x=0;x<w;x++)input[y*w+x]=(unsigned char)((13*x+19*y+7)%181);
  std::printf("%s{\"kernel\":%d,\"levels\":[",k?",":"",k);int level=0;
  for(;;){
   std::printf("%s{\"w\":%d,\"h\":%d,\"bytes\":[",level++?",":"",w,hh);for(int i=0;i<w*hh;i++)std::printf("%s%u",i?",":"",input[i]);std::printf("]}");std::fflush(stdout);
   int nw=(w+1)/2,nh=(hh+1)/2;if(nw*nh<32||nw<=1||nh<=1||level==8)break;
   std::vector<unsigned char> out(nw*nh+128,0xcc);int dims[2]={-1,-1};ks[k](input.data(),w,(uint64_t(hh)<<32)|uint32_t(w),out.data(),dims);
   if(dims[0]!=nw||dims[1]!=nh)TerminateProcess(GetCurrentProcess(),13);input.swap(out);w=nw;hh=nh;
  }std::printf("]}");
 }
 std::puts("]}");std::fflush(stdout);std::fputs("complete3chains\n",stderr);std::fflush(stderr);TerminateProcess(GetCurrentProcess(),0);return 11;
}
