#include <windows.h>
#include <cstdio>
#include <vector>
#include <cstdint>
#include <cstring>
// Owned process only: original WeightFun1 branch, never product code.
int main(int argc,char**argv){
 if(argc!=3)return 2;SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);SetDllDirectoryA(argv[1]);
 auto lib=LoadLibraryA("MVD_Algorithm.dll");if(!lib)return 3;
 using Fit=int(*)(const float*,const float*,int,int,float*,void*);
 auto fit=(Fit)((char*)lib+0x8345e0);
 std::vector<unsigned char> scratch(16*1024*1024+128);void* aligned=(void*)(((uintptr_t)scratch.data()+127)&~(uintptr_t)127);
 for(int seq=0;seq<84;seq++){
  char path[4096];sprintf_s(path,"%s/capture-%03d.bin",argv[2],seq);FILE*f=fopen(path,"rb");if(!f)return 4;
  fseek(f,0,SEEK_END);long length=ftell(f);rewind(f);if(length<=0||length%16)return 5;int n=length/16;
  std::vector<float> pairs(n*4);fread(pairs.data(),1,length,f);fclose(f);
  for(int freedom=1;freedom<=3;freedom++){
   memset(aligned,0,16*1024*1024);float h[9]={};int code=fit(pairs.data(),pairs.data()+n*2,n,freedom,h,aligned);
   printf("{\"sequence\":%d,\"freedom\":%d,\"count\":%d,\"returnRax\":%d,\"matrix\":[",seq,freedom,n,code);
   for(int i=0;i<9;i++)printf(i?",%.9g":"%.9g",h[i]);printf("]}\n");fflush(stdout);
  }
 }
 return 0;
}
