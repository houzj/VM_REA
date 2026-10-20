#include <windows.h>
#include <cstdio>
#include <vector>
#include <fstream>
#include <cstring>
int main(int argc,char**argv){if(argc!=4)return 2;auto module=LoadLibraryA(argv[1]);if(!module)return 3;
 std::ifstream f(argv[2],std::ios::binary);std::vector<unsigned char> pixels((std::istreambuf_iterator<char>(f)),{});if(pixels.size()!=1000000)return 4;
 float q[]={222.496078f,227.96312f,339.595917f,227.95932f,339.596008f,345.15213f,222.494217f,345.155426f};
 float canonical[]={0,0,200,0,200,200,0,200},h[9]={},rms=0;int fitState=0;int config[]={0,1,3,20,0x38d1b717,0x40000000};
 std::vector<unsigned char> scratch(8*1024*1024);
 // 80ac20 assembly 80aeb3..80afca: stack args rms/state/config/workspace.
 auto fit=(int(*)(float*,float*,unsigned,float*,float*,int*,int*,void*))((unsigned char*)module+0x7ff3c0);
 int status=fit(q,canonical,4,h,&rms,&fitState,config,scratch.data());
 printf("{\"fitReturn\":%d,\"fitState\":%d,\"rms\":%.9g,\"matrix\":[",status,fitState,rms);for(int i=0;i<9;i++)printf("%s%.9g",i?",":"",h[i]);puts("]}");
 std::string out=argv[3];auto dump=[&](const char*name,const void*data,size_t size){std::ofstream file(out+"/"+name,std::ios::binary);file.write((const char*)data,size);};dump("native-outer-fit.f32",h,sizeof(h));
 // 80ac20 80b0xx copies H then calls730e90(matrix,3,packed3x3,workspace).
 auto inverse=(int(*)(float*,int,unsigned long long,void*))((unsigned char*)module+0x730e90);
 int invStatus=inverse(h,3,3ull|(3ull<<32),scratch.data());dump("native-inverse-outer-fit.f32",h,sizeof(h));
 printf("{\"inverseReturn\":%d,\"matrix\":[",invStatus);for(int i=0;i<9;i++)printf("%s%.9g",i?",":"",h[i]);puts("]}");
 fflush(stdout);TerminateProcess(GetCurrentProcess(),0);return 6;
}
