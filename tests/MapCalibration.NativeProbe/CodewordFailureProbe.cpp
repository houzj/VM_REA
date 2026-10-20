#include <windows.h>
#include <cstdio>
#include <cstdint>
#include <vector>
int main(int argc,char**argv){
 if(argc!=2)return 2;
 auto module=LoadLibraryA(argv[1]);if(!module)return 3;
 auto decode=reinterpret_cast<int(*)(int*,int,int,int*,void*)>(reinterpret_cast<unsigned char*>(module)+0x8b0260);
 constexpr int guard=0x51472a39, sentinel=-999;
 std::vector<int> input(192+32,guard), output(24+32,guard);
 for(int i=0;i<24;i++)for(int b=0;b<8;b++)input[16+i*8+b]=(i>>(7-b))&1;
 for(int i=0;i<24;i++)output[16+i]=sentinel;
 auto before=input;
 std::vector<unsigned char> allocation(65536+512,0xa7);
 auto raw=reinterpret_cast<uintptr_t>(allocation.data());
 auto work=reinterpret_cast<unsigned char*>((raw+255)&~uintptr_t(127));
 // 85aff0(18,24) requires 1280 bytes. Supply 65536 aligned bytes.
 printf("{\"event\":\"before\",\"rva\":\"0x8b0260\",\"checks\":18,\"symbols\":24,\"workspaceCapacity\":65536,\"sourceRequiredBytes\":1280}\n");fflush(stdout);
 int code=decode(input.data()+16,18,24,output.data()+16,work);
 bool outputUnchanged=true,outputGuards=true,workspaceGuards=true;
 for(int i=0;i<24;i++)outputUnchanged&=output[16+i]==sentinel;
 for(int i=0;i<16;i++)outputGuards&=output[i]==guard&&output[40+i]==guard;
 for(auto p=allocation.data();p<work;p++)workspaceGuards&=*p==0xa7;
 for(auto p=work+65536;p<allocation.data()+allocation.size();p++)workspaceGuards&=*p==0xa7;
 bool inputUnchanged=input==before;
 printf("{\"event\":\"result\",\"signedReturn\":%d,\"unsignedReturn\":%u,\"outputUnchanged\":%s,\"outputGuardsUnchanged\":%s,\"inputUnchanged\":%s,\"workspaceOuterGuardsUnchanged\":%s,\"output\":[",code,(unsigned)code,outputUnchanged?"true":"false",outputGuards?"true":"false",inputUnchanged?"true":"false",workspaceGuards?"true":"false");
 for(int i=0;i<24;i++)printf("%s%d",i?",":"",output[16+i]);puts("]}");fflush(stdout);
 bool ok=code==-1000&&outputUnchanged&&outputGuards&&inputUnchanged&&workspaceGuards;
 // Same owned-process exit as the existing oracle; avoid core DLL unload lifecycle.
 TerminateProcess(GetCurrentProcess(),ok?0:4);return 5;
}
