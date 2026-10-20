#include <windows.h>
#include <cstdio>
#include <vector>
#include <cstdint>
#include <cstring>
using Kernel=void(__cdecl*)(float*,int,int*,int*,unsigned char*);
static DWORD WINAPI deadline(void* ready){SetEvent((HANDLE)ready);Sleep(20000);std::fputs("timeout20s\n",stderr);std::fflush(stderr);TerminateProcess(GetCurrentProcess(),124);return 124;}
static unsigned bits(float f){unsigned b;std::memcpy(&b,&f,4);return b;}
int main(){
 SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX|SEM_NOOPENFILEERRORBOX);
 HANDLE ready=CreateEventW(nullptr,TRUE,FALSE,nullptr);HANDLE t=CreateThread(nullptr,0,deadline,ready,0,nullptr);if(!t||WaitForSingleObject(ready,1000)!=WAIT_OBJECT_0)return 10;
 SetDllDirectoryW(L"D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64");
 HMODULE h=LoadLibraryW(L"D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll");if(!h)return 2;
 auto group=(Kernel)((unsigned char*)h+0xc978c0);
 std::vector<std::vector<float>> cases={{0},{},{},{0,89,90,91,179,180,181,270}};
 for(float a=-45;a<=45;a+=2)cases[1].push_back(a);for(float a=-180;a<=180;a+=2)cases[2].push_back(a);
 std::printf("{\"scope\":\"four fixed source angle tables; pure leaf\",\"cases\":[");int id=0;
 for(auto& a:cases){std::vector<int> output(a.size()*5+32,-9);std::vector<unsigned char> visited(a.size()+64,0xcc);int count=-1;
  group(a.data(),(int)a.size(),output.data(),&count,visited.data());if(count<0||count>(int)a.size())TerminateProcess(GetCurrentProcess(),13);
  std::printf("%s{\"angleBits\":[",id++?",":"");for(int i=0;i<(int)a.size();i++)std::printf("%s\"%08x\"",i?",":"",bits(a[i]));
  std::printf("],\"groups\":[");for(int i=0;i<count;i++){int n=output[i*5];if(n<1||n>4)TerminateProcess(GetCurrentProcess(),14);std::printf("%s[",i?",":"");for(int j=0;j<n;j++)std::printf("%s%d",j?",":"",output[i*5+j+1]);std::printf("]");}std::printf("]}");std::fflush(stdout);
 }
 std::puts("]}");std::fflush(stdout);std::fputs("complete4tables\n",stderr);std::fflush(stderr);TerminateProcess(GetCurrentProcess(),0);return 11;
}
