#include <windows.h>
#include <cstdio>
#include <vector>
#include <cstdint>
#include <algorithm>
static std::vector<unsigned char> Read(const char* path){FILE* file=fopen(path,"rb");if(!file)return {};fseek(file,0,SEEK_END);long size=ftell(file);fseek(file,0,SEEK_SET);std::vector<unsigned char> bytes(size);fread(bytes.data(),1,size,file);fclose(file);return bytes;}
int main(int argc,char** argv){
 if(argc!=4)return 2;setvbuf(stdout,nullptr,_IONBF,0);SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);SetDllDirectoryA(argv[1]);
 auto wrapper=LoadLibraryA("MVDMapCalibCpp.dll"),core=GetModuleHandleA("MVD_Algorithm.dll");if(!wrapper||!core)return 3;
 auto input=Read(argv[3]);if(input.empty()||input.size()%28)return 4;
 using Grid=void(__fastcall*)(void*,int,uint64_t,void*,void*,int*,int*,float*,void*);
 auto grid=(Grid)((unsigned char*)core+0x8bcff0);int count=(int)input.size()/28;
 // Controlled input-order diagnostic; workspace reserves the source -2/-1 clear prefix.
 for(int variant=0;variant<4;variant++){
  std::vector<unsigned char> candidates(90000*28);for(int i=0;i<count;i++){int source=variant==1?count-1-i:variant==2?(i+1)%count:variant==3?(i+count/2)%count:i;std::copy_n(input.data()+source*28,28,candidates.data()+i*28);}
  std::vector<unsigned char> scratch(16*1024*1024+128),pixels(90000*8),indices(90000*8);int found=0,dimensions[2]={0,0};float score=0;
  std::fill_n(scratch.data(),128,0xa5);grid(candidates.data(),count,((uint64_t)300<<32)|300,pixels.data(),indices.data(),&found,dimensions,&score,scratch.data()+128);
  if(found<0||found>90000)return 5;char path[100];sprintf_s(path,"grid-%d-prefix.bin",variant);FILE* prefix=fopen(path,"wb");if(prefix){fwrite(scratch.data(),1,128,prefix);fclose(prefix);}sprintf_s(path,"grid-%d-pixels.bin",variant);FILE* file=fopen(path,"wb");if(file){fwrite(pixels.data(),8,found,file);fclose(file);}sprintf_s(path,"grid-%d-indices.bin",variant);file=fopen(path,"wb");if(file){fwrite(indices.data(),8,found,file);fclose(file);}
  printf("{\"variant\":%d,\"inputCount\":%d,\"count\":%d,\"width\":%d,\"height\":%d,\"score\":%.9g}\n",variant,count,found,dimensions[0],dimensions[1],score);
 }
 fflush(stdout);TerminateProcess(GetCurrentProcess(),0);
}




