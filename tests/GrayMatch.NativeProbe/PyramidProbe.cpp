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
 auto base=(unsigned char*)h; Kernel ks[]={(Kernel)(base+0xd46a10),(Kernel)(base+0xd472b0),(Kernel)(base+0xc96f20)};
 std::printf("{\"scope\":\"pure pyramid leaves; allocated padding; no initialization\",\"cases\":[");int count=0;
 for(int w:{3,32,33})for(int hh:{3,4})for(int pad:{0,7}){
  int stride=w+pad;std::vector<unsigned char> input(stride*(hh+2)+128,237);
  for(int y=0;y<hh;y++)for(int x=0;x<stride;x++)input[y*stride+x]=(unsigned char)(x<w?(13*x+19*y+7)%181:201+(x%31));
  std::printf("%s{\"w\":%d,\"h\":%d,\"stride\":%d,\"input\":[",count++?",":"",w,hh,stride);
  for(int i=0;i<stride*hh;i++)std::printf("%s%u",i?",":"",input[i]);std::printf("],\"outputs\":[");
  for(int k=0;k<3;k++){std::vector<unsigned char> out(4096,0xcc);int dims[2]={-1,-1};ks[k](input.data(),stride,(uint64_t(hh)<<32)|uint32_t(w),out.data(),dims);
   if(dims[0]<0||dims[1]<0||dims[0]*dims[1]>4096)TerminateProcess(GetCurrentProcess(),13);
   std::printf("%s{\"kernel\":%d,\"w\":%d,\"h\":%d,\"bytes\":[",k?",":"",k,dims[0],dims[1]);for(int i=0;i<dims[0]*dims[1];i++)std::printf("%s%u",i?",":"",out[i]);std::printf("]}");std::fflush(stdout);
  }std::printf("]}");std::fflush(stdout);
 }
 std::puts("]}");std::fflush(stdout);std::fputs("complete12layouts36outputs\n",stderr);std::fflush(stderr);TerminateProcess(GetCurrentProcess(),0);return 11;
}
