#include <windows.h>
#include <cstdio>
#include <vector>
#include <cstdint>
#include <cstring>
static std::vector<unsigned char> Read(const char* path){FILE* file=fopen(path,"rb");if(!file)return {};fseek(file,0,SEEK_END);long size=ftell(file);fseek(file,0,SEEK_SET);std::vector<unsigned char> bytes(size);fread(bytes.data(),1,size,file);fclose(file);return bytes;}
int main(int argc,char** argv){
 if(argc!=4)return 2;setvbuf(stdout,nullptr,_IONBF,0);SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);SetDllDirectoryA(argv[1]);
 auto wrapper=LoadLibraryA("MVDMapCalibCpp.dll"),core=GetModuleHandleA("MVD_Algorithm.dll");if(!wrapper||!core)return 3;
 auto input=Read(argv[3]);if(input.empty()||input.size()%28)return 4;int count=(int)input.size()/28;
 using Neighbor=unsigned(__fastcall*)(void*,int,int,void*,float*,uint64_t,void*);auto neighbor=(Neighbor)((unsigned char*)core+0x8c0fa0);
 int sequence=0;for(int reference:{0,1,count/2,count-1})for(int axis=0;axis<4;axis++)for(int mode=0;mode<3;mode++){
  std::vector<int> used(count);used[reference]=1;if(mode==1){used[(reference+1)%count]=1;used[count-1]=1;}if(mode==2)for(auto& value:used)value=1;
  float direction[2];memcpy(direction,input.data()+reference*28+12+(axis>=2?8:0),8);if(axis%2){direction[0]=-direction[0];direction[1]=-direction[1];}uint64_t packed;memcpy(&packed,direction,8);
  std::vector<float> costs(count);float score=0;unsigned selected=neighbor(input.data(),count,reference,used.data(),&score,packed,costs.data());unsigned scoreBits;memcpy(&scoreBits,&score,4);
  char path[100];sprintf_s(path,"neighbor-%03d-costs.float",++sequence);FILE* file=fopen(path,"wb");if(file){fwrite(costs.data(),4,count,file);fclose(file);}unsigned directionBits[2];memcpy(directionBits,direction,8);
  printf("{\"sequence\":%d,\"reference\":%d,\"axis\":%d,\"mode\":%d,\"directionBits\":[%u,%u],\"selected\":%u,\"scoreBits\":%u}\n",sequence,reference,axis,mode,directionBits[0],directionBits[1],selected,scoreBits);
 }
 fflush(stdout);TerminateProcess(GetCurrentProcess(),0);
}
