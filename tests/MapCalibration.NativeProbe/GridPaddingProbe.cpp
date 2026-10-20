#include <windows.h>
#include <cstdio>
#include <vector>
#include <cstdint>
static std::vector<unsigned char> Read(const char* path){FILE* file=fopen(path,"rb");if(!file)return {};fseek(file,0,SEEK_END);long size=ftell(file);fseek(file,0,SEEK_SET);std::vector<unsigned char> bytes(size);fread(bytes.data(),1,size,file);fclose(file);return bytes;}
struct GridState{int count,padding;int* indices;float* scores;};
int main(int argc,char** argv){
 if(argc!=4)return 2;setvbuf(stdout,nullptr,_IONBF,0);SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);SetDllDirectoryA(argv[1]);
 auto wrapper=LoadLibraryA("MVDMapCalibCpp.dll"),core=GetModuleHandleA("MVD_Algorithm.dll");if(!wrapper||!core)return 3;
 auto input=Read(argv[3]);if(input.empty()||input.size()%28)return 4;int count=(int)input.size()/28;
 using Seed=unsigned(__fastcall*)(void*,int,GridState*,int*,int,void*,void*);
 int sequence=0;for(int kind=0;kind<2;kind++)for(int reference:{0,1,count/2,count-1})for(int direction=0;direction<4;direction++){int mode=0;
  auto seed=(Seed)((unsigned char*)core+(kind==0?0x8c1340:0x8c1780));std::vector<int> indices(90000,0x55555555),used(90000,mode==0?0:1);std::vector<float> scores(90000*3);std::vector<unsigned char> costs(16*1024*1024);
  GridState state{mode==2?17:0,0,indices.data(),scores.data()};int dimensions[2]={mode==2?19:0,mode==2?23:0};unsigned status=seed(input.data(),count,&state,dimensions,reference,used.data(),costs.data());
  using Pad=unsigned(__fastcall*)(GridState*,int*,int);auto pad=(Pad)((unsigned char*)core+0x8c00f0);unsigned padStatus=pad(&state,dimensions,direction),repeatStatus=pad(&state,dimensions,direction);char path[100];sprintf_s(path,"seed-%03d-indices.bin",++sequence);FILE* file=fopen(path,"wb");if(file){fwrite(indices.data(),4,indices.size(),file);fclose(file);}sprintf_s(path,"seed-%03d-used.bin",sequence);file=fopen(path,"wb");if(file){fwrite(used.data(),4,used.size(),file);fclose(file);}sprintf_s(path,"seed-%03d-scores.float",sequence);file=fopen(path,"wb");if(file){for(int y=0;y<3;y++)fwrite(scores.data()+y*900,4,9,file);fclose(file);}
  sprintf_s(path,"seed-%03d-padded.float",sequence);file=fopen(path,"wb");if(file){fwrite(scores.data(),4,scores.size(),file);fclose(file);}printf("{\"sequence\":%d,\"kind\":%d,\"reference\":%d,\"mode\":%d,\"status\":%u,\"count\":%d,\"dimensions\":[%d,%d],\"direction\":%d,\"padStatus\":%u,\"repeatStatus\":%u}\n",sequence,kind,reference,mode,status,state.count,dimensions[0],dimensions[1],direction,padStatus,repeatStatus);
 }
 fflush(stdout);TerminateProcess(GetCurrentProcess(),0);
}


