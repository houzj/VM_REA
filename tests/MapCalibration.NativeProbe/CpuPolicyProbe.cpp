#include <windows.h>
#include <intrin.h>
#include <cstdio>
int main(int argc,char**argv){if(argc!=4)return 2;auto module=LoadLibraryA(argv[1]);if(!module)return 3;
 int one[4],seven[4];__cpuidex(one,1,0);__cpuidex(seven,7,0);
 OSVERSIONINFOEXW v={};v.dwOSVersionInfoSize=284;v.dwMajorVersion=6;v.dwMinorVersion=1;v.wServicePackMajor=1;
 DWORDLONG c=0;c=VerSetConditionMask(c,2,3);c=VerSetConditionMask(c,1,3);c=VerSetConditionMask(c,0x20,3);BOOL ok=VerifyVersionInfoW(&v,0x23,c);
 unsigned ecx=(unsigned)seven[2],edx=(unsigned)seven[3];int low=(ecx&(1u<<20))?7:(ecx&(1u<<19))?6:(ecx&(1u<<9))?5:(ecx&1)?4:(edx&(1u<<26))?3:(edx&(1u<<25))?2:(edx&(1u<<23))?1:0;
 int predicted=((one[0]&0xf00)<0x600)?0:!ok?low:(seven[1]&0x20)?11:((one[2]&0x18000000)==0x18000000)?10:low;
 int actual=((int(*)())((unsigned char*)module+0x630120))();
 printf("{\"leaf1Eax\":%u,\"leaf1Ecx\":%u,\"leaf7Ebx\":%u,\"leaf7Ecx\":%u,\"leaf7Edx\":%u,\"versionAccepted\":%s,\"predicted\":%d,\"original\":%d}\n",(unsigned)one[0],(unsigned)one[2],(unsigned)seven[1],ecx,edx,ok?"true":"false",predicted,actual);
 fflush(stdout);TerminateProcess(GetCurrentProcess(),predicted==actual?0:5);return 6;
}
