#include <windows.h>
#include <cstdio>
#include <cstring>
#include <cmath>
#include <limits>
using Variance = double(__cdecl*)(double,double,int);
using Score = double(__cdecl*)(double,double,double,double,double);
static unsigned long long bits(double v) { unsigned long long b; std::memcpy(&b,&v,8); return b; }
static DWORD WINAPI deadline(void* ready) { SetEvent(static_cast<HANDLE>(ready)); Sleep(20000); std::fputs("timeout20s\n",stderr);std::fflush(stderr);TerminateProcess(GetCurrentProcess(),124);return 124; }
int main() {
 SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX|SEM_NOOPENFILEERRORBOX);
 HANDLE ready=CreateEventW(nullptr,TRUE,FALSE,nullptr);HANDLE timer=CreateThread(nullptr,0,deadline,ready,0,nullptr);
 if(!timer||WaitForSingleObject(ready,1000)!=WAIT_OBJECT_0)return 10;
 SetDllDirectoryW(L"D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64");
 std::fputs("before-load\n",stderr);std::fflush(stderr);
 HMODULE h=LoadLibraryW(L"D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll");
 if(!h){std::fprintf(stderr,"LoadLibrary %lu\n",GetLastError());return 2;}
 std::fputs("loaded\n",stderr);std::fflush(stderr);
 auto base=reinterpret_cast<unsigned char*>(h);
 auto variance=reinterpret_cast<Variance>(base+0xc9ace0);
 auto score=reinterpret_cast<Score>(base+0xc9ad00);
 double epsilon;std::memcpy(&epsilon,base+0x16d0200,8);
 double nan=std::numeric_limits<double>::quiet_NaN();
 struct V { double sum,squares;int count; };
 V vs[]={{10,30,4},{5,0,0},{0,0,0},{10,30,-1},{nan,30,4}};
 std::printf("{\"epsilonBits\":\"%016llx\",\"variance\":[",bits(epsilon));
 int index=0;
 for(auto c:vs){double value=variance(c.sum,c.squares,c.count);std::printf("%s{\"sumBits\":\"%016llx\",\"squaresBits\":\"%016llx\",\"count\":%d,\"resultBits\":\"%016llx\"}",index++?",":"",bits(c.sum),bits(c.squares),c.count,bits(value));std::fflush(stdout);}
 struct S {double dot,mean,templateVariance,sum,imageVariance;};
 S ss[]={{10,2,5,3,4},{1,2,5,3,4},{6,2,5,3,4},{10,2,0,3,0},{nan,2,5,3,4},{1+std::ldexp(1.,-52),1+std::ldexp(1.,-27),1,1-std::ldexp(1.,-27),1}};
 std::printf("],\"score\":[");index=0;
 for(auto c:ss){double value=score(c.dot,c.mean,c.templateVariance,c.sum,c.imageVariance);std::printf("%s{\"dotBits\":\"%016llx\",\"meanBits\":\"%016llx\",\"templateVarianceBits\":\"%016llx\",\"sumBits\":\"%016llx\",\"imageVarianceBits\":\"%016llx\",\"resultBits\":\"%016llx\"}",index++?",":"",bits(c.dot),bits(c.mean),bits(c.templateVariance),bits(c.sum),bits(c.imageVariance),bits(value));std::fflush(stdout);}
 std::puts("]}");std::fflush(stdout);std::fputs("all11-cases-complete\n",stderr);std::fflush(stderr);
 TerminateProcess(GetCurrentProcess(),0);return 11;
}
