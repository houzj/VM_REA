#include <windows.h>
#include <cstdio>
#include <vector>
#include <cstdint>
#include <climits>
struct Pair { int x,y; };
struct Pixel { float x,y; };
int main(int argc,char**argv){
 if(argc!=2)return 2;SetDllDirectoryA(argv[1]);
 auto wrapper=LoadLibraryA("MVDMapCalibCpp.dll"),core=GetModuleHandleA("MVD_Algorithm.dll");if(!wrapper||!core)return 3;
 using Map=uint64_t(__fastcall*)(Pixel**,Pair**,uint64_t,int,int,int,int,int,unsigned*,Pair*,Pixel*);
 auto map=(Map)((BYTE*)core+0x813020);
 for(int width:{1,2,3,4})for(int height:{1,2,3,4})for(int missing:{0,1})for(int orientation=0;orientation<=9;orientation++){
  std::vector<std::vector<Pixel>> pixels(height,std::vector<Pixel>(width));std::vector<std::vector<Pair>> indices(height,std::vector<Pair>(width));
  std::vector<Pixel*> p(height);std::vector<Pair*> q(height);
  for(int y=0;y<height;y++){p[y]=pixels[y].data();q[y]=indices[y].data();for(int x=0;x<width;x++){pixels[y][x]={(float)(x+.125+y*10),(float)(y+.375+x*10)};indices[y][x]={x+11,y+23};if(missing&&(x+y)%3==0)indices[y][x].x=INT_MIN;}}
  Pair output[32];Pixel xy[32];for(int i=0;i<32;i++){output[i]={123456,654321};xy[i]={-123,-456};}unsigned dimensions[2]={111,222};
  auto status=map(p.data(),q.data(),(uint64_t(height)<<32)|width,24,12,101,203,orientation,dimensions,output,xy);
  int count=0;while(count<32&&output[count].x!=123456)count++;
  printf("{\"width\":%d,\"height\":%d,\"missing\":%d,\"orientation\":%d,\"status\":%llu,\"dimensions\":[%u,%u],\"points\":[",width,height,missing,orientation,(unsigned long long)status,dimensions[0],dimensions[1]);
  for(int i=0;i<count;i++)printf("%s[%d,%d,%.9g,%.9g]",i?",":"",output[i].x,output[i].y,xy[i].x,xy[i].y);puts("]}");
 }
 return 0;
}
