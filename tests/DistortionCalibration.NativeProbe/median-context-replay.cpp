#include <windows.h>
#include <cstdio>
#include <vector>
#include <cstring>
// Minimal 7fec40 replay with captured surrounding workspace. No image/device needed.
int main(int argc,char**argv){if(argc!=3)return 2;SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);SetDllDirectoryA(argv[1]);auto core=LoadLibraryA("MVD_Algorithm.dll");if(!core)return 3;
 using Median=int(*)(void*,void*,int,float*,int,void*,float*,float*);auto median=(Median)((char*)core+0x7fec40);
 for(int seq:{4,56,63,67,76,83}){char path[4096];sprintf_s(path,"%s/median-entry-%03d-00.bin",argv[2],seq);FILE*f=fopen(path,"rb");if(!f)return 4;
  int n,freedom,prefix;float coefficients[8];fread(&n,4,1,f);fread(&freedom,4,1,f);fread(&prefix,4,1,f);fread(coefficients,4,8,f);
  std::vector<float> source(n*2),target(n*2),surrounding(prefix+n);fread(source.data(),4,n*2,f);fread(target.data(),4,n*2,f);fread(surrounding.data(),4,prefix+n,f);fclose(f);
  for(int mode:{0,1}){std::vector<float> allocation(prefix+n+256,0),errors(n);if(mode)memcpy(allocation.data()+128,surrounding.data(),(prefix+n)*4);float sigma=-999;auto sorted=allocation.data()+128+prefix;int code=median(source.data(),target.data(),n,coefficients,freedom,sorted,errors.data(),&sigma);
   printf("{\"sequence\":%d,\"capturedContext\":%s,\"code\":%d,\"sigma\":%.9g,\"errors\":[",seq,mode?"true":"false",code,sigma);for(int i=0;i<n;i++)printf(i?",%.9g":"%.9g",errors[i]);printf("]}\n");fflush(stdout);
  }
 }
 return 0;
}
