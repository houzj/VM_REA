#include <windows.h>
#include <cstdio>
#include <vector>
#include <fstream>
#include <string>
int main(int argc,char**argv){if(argc!=4)return 2;auto module=LoadLibraryA(argv[1]);if(!module)return 3;
 std::ifstream f(argv[2],std::ios::binary);std::vector<unsigned char> pixels((std::istreambuf_iterator<char>(f)),{});if(pixels.size()!=1000000)return 4;
 float q[]={222.496078f,227.96312f,339.595917f,227.95932f,339.596008f,345.15213f,222.494217f,345.155426f};
 std::vector<unsigned char> output(4096),warp(8*1024*1024),local(8*1024*1024),scratch(8*1024*1024);
 auto opening=(void(*)(void*,int,unsigned long long,float*,void*,void*,void*,void*))((unsigned char*)module+0x80a7f0);
 int cpu=((int(*)())((unsigned char*)module+0x630120))();opening(pixels.data(),1000,1000ull|(1000ull<<32),q,output.data(),warp.data(),local.data(),scratch.data());
 std::string directory=argv[3];auto dump=[&](const char*name,const void*data,size_t size){std::ofstream out(directory+"/"+name,std::ios::binary);out.write((const char*)data,size);};dump("native-opening-warp.gray",warp.data(),40000);
 printf("{\"cpu\":%d,\"flag\":%d,\"count\":%d,\"bytes\":[",cpu,*(int*)(output.data()+0xf0),*(int*)(output.data()+0x118));for(int i=0;i<12;i++)printf("%s%u",i?",":"",output[0x11c+i]);puts("]}");
 float canonical[]={0,0,200,0,200,200,0,200},inner[8]={};
 auto refinement=(int(*)(void*,int,unsigned long long,unsigned long long,float*,float*,float*,void*,void*,void*))((unsigned char*)module+0x80ac20);
 int refined=refinement(pixels.data(),1000,1000ull|(1000ull<<32),200ull|(200ull<<32),canonical,q,inner,warp.data(),local.data(),scratch.data());
 printf("{\"refinementStatus\":%d,\"inner\":[",refined);for(int i=0;i<8;i++)printf("%s%.9g",i?",":"",inner[i]);puts("]}");
 auto preloc=(void(*)(void*,unsigned long long,unsigned long long,int*,unsigned*,int*,void*))((unsigned char*)module+0x6eea70);
 int origins[4][2]={{0,0},{100,0},{100,100},{0,100}};int config[]={10,50,8,1,10,1};
 for(int k=0;k<4;k++){std::vector<unsigned char> crop(10000);for(int y=0;y<100;y++)memcpy(crop.data()+100*y,warp.data()+(origins[k][1]+y)*200+origins[k][0],100);int xy[2]={-999,-999};unsigned status=0;
  preloc(crop.data(),100,100ull|(100ull<<32),xy,&status,config,scratch.data());printf("{\"quadrant\":%d,\"preStatus\":%u,\"preX\":%d,\"preY\":%d}\n",k,status,xy[0],xy[1]);
  std::string scoreName="native-score-"+std::to_string(k)+".i32";dump(scoreName.c_str(),scratch.data(),10000*4);
  std::string name="native-crop-"+std::to_string(k)+".gray";dump(name.c_str(),crop.data(),crop.size());
 }
 fflush(stdout);TerminateProcess(GetCurrentProcess(),0);return 6;
}


