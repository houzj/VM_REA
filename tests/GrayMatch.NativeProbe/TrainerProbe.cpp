#include <windows.h>
#include <cstdio>
#include <cstring>
#include <cstdint>
struct Config { int levels;float start,end,step; };
using Trainer=uint32_t(__cdecl*)(unsigned char*,int,unsigned char*,int,uint64_t,uint64_t,int,Config*,unsigned char*,uint64_t*,void**,unsigned char*);
static DWORD WINAPI deadline(void* ready){SetEvent((HANDLE)ready);Sleep(20000);std::fputs("timeout20s\n",stderr);std::fflush(stderr);TerminateProcess(GetCurrentProcess(),124);return 124;}
static uint32_t invoke(Trainer f,unsigned char* image,unsigned char* mask,Config* cfg,unsigned char* model,uint64_t* used,void** result,unsigned char* work,DWORD* exception){
 __try {return f(image,16,mask,16,(uint64_t(16)<<32)|16,0x40f0000040f00000ULL,0x1390,cfg,model,used,result,work);}
 __except(EXCEPTION_EXECUTE_HANDLER){*exception=GetExceptionCode();return 0xffffffff;}
}
static bool guard(unsigned char* base,size_t n){for(size_t i=0;i<128;i++)if(base[i]!=0x19||base[128+n+i]!=0x19)return false;return true;}
static unsigned u32(unsigned char* p){unsigned v;std::memcpy(&v,p,4);return v;}
int main(){
 SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX|SEM_NOOPENFILEERRORBOX);
 HANDLE ready=CreateEventW(nullptr,TRUE,FALSE,nullptr);HANDLE t=CreateThread(nullptr,0,deadline,ready,0,nullptr);if(!t||WaitForSingleObject(ready,1000)!=WAIT_OBJECT_0)return 10;
 constexpr size_t cap=1024*1024;
 auto mb=(unsigned char*)VirtualAlloc(nullptr,cap+256,MEM_COMMIT|MEM_RESERVE,PAGE_READWRITE);
 auto wb=(unsigned char*)VirtualAlloc(nullptr,cap+256,MEM_COMMIT|MEM_RESERVE,PAGE_READWRITE);if(!mb||!wb)return 11;
 std::memset(mb,0x19,cap+256);std::memset(wb,0x19,cap+256);auto model=mb+128;auto work=wb+128;std::memset(model,0xa5,cap);std::memset(work,0xcc,cap);
 unsigned char image[384],mask[384];std::memset(image,237,sizeof image);std::memset(mask,237,sizeof mask);
 for(int y=0;y<16;y++)for(int x=0;x<16;x++){image[y*16+x]=(unsigned char)((13*x+19*y+7)%181);mask[y*16+x]=1;}
 SetDllDirectoryW(L"D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64");
 HMODULE h=LoadLibraryW(L"D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll");if(!h)return 2;
 auto f=(Trainer)((unsigned char*)h+0xbefbb0);Config cfg{1,0,0,2};uint64_t used=0xfeedbeef;void* result=(void*)0xfeed;DWORD exception=0;
 std::fputs("before-fixed-trainer\n",stderr);std::fflush(stderr);uint32_t code=invoke(f,image,mask,&cfg,model,&used,&result,work,&exception);
 bool mg=guard(mb,cap),wg=guard(wb,cap);bool alias=result==model;
 std::printf("{\"scope\":\"single16x16manual1-angle0trainer; no initialization\",\"return\":\"%08x\",\"exception\":\"%08lx\",\"used\":%llu,\"modelGuard\":%s,\"workGuard\":%s,\"resultAliasesModel\":%s,\"modelAddress\":\"%p\",\"workAddress\":\"%p\",\"capacity\":%zu",code,exception,used,mg?"true":"false",wg?"true":"false",alias?"true":"false",model,work,cap);
 if(code==0&&exception==0&&used<=cap&&alias&&mg&&wg){
  std::printf(",\"actualLevels\":%u,\"version\":\"%08x\",\"firstSpanCount\":%u,\"firstPixelCount\":%u",u32(model+0x24),u32(model+0x276b8),u32(model+0x27b80),u32(model+0x27b80+0x30));
  std::printf(",\"modelBytesHex\":\"");for(uint64_t i=0;i<used;i++)std::printf("%02x",model[i]);std::printf("\"");
 }
 std::puts("}");std::fflush(stdout);std::fprintf(stderr,"fixed-trainer-return=%08x exception=%08lx\n",code,exception);std::fflush(stderr);
 TerminateProcess(GetCurrentProcess(),code==0&&exception==0&&used<=cap&&alias&&mg&&wg?0:20);return 21;
}
