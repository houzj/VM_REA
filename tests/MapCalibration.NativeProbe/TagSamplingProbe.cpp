#include <windows.h>
#include <cstdio>
#include <vector>
#include <fstream>
#include <string>
int main(int argc,char**argv){if(argc!=4)return 2;auto module=LoadLibraryA(argv[1]);if(!module)return 3;
 std::ifstream f(argv[2],std::ios::binary);std::vector<unsigned char> pixels((std::istreambuf_iterator<char>(f)),{});if(pixels.size()!=1000000)return 4;
 float q[]={246,253,317,253,294,324,246,324},reference[]={10,10,70,10,50,70,10,70},h[9]={};
 std::vector<unsigned char> scratch(8*1024*1024),output(4096);
 auto fit=(int(*)(float*,float*,int,int,float*,void*))((unsigned char*)module+0x89faa0);
 int cpu=((int(*)())((unsigned char*)module+0x630120))();
 int fitStatus=fit(reference,q,4,1,h,scratch.data());
 struct Quads{int count=1;int pad=0;float*points;};Quads quads;quads.points=q;
 auto tag=(void(*)(void*,void*,int,unsigned long long,void*,void*))((unsigned char*)module+0x86ff00);
 tag(&quads,pixels.data(),1000,1000ull|(1000ull<<32),output.data(),scratch.data());
 std::string directory=argv[3];
 auto dump=[&](const char*name,const void*data,size_t size){std::ofstream out(directory+"/"+name,std::ios::binary);out.write((const char*)data,size);};
 dump("native-h.f32",h,sizeof(h));dump("native-warped.gray",scratch.data(),6400);dump("native-binary.gray",scratch.data()+0x6400,6400);dump("native-bits.i32",scratch.data()+0xce00,192*4);
 std::vector<unsigned char> meanScratch(8*1024*1024),directBinary(6400);
 auto mean=(void(*)(void*,int,void*,int,unsigned long long,unsigned long long,int,int,void*))((unsigned char*)module+0x85c8a0);
 mean(scratch.data(),80,directBinary.data(),80,80ull|(80ull<<32),20ull|(20ull<<32),0,0,meanScratch.data());
 dump("native-mean.gray",meanScratch.data(),6400);dump("native-direct-binary.gray",directBinary.data(),6400);
 printf("{\"cpu\":%d,\"fitStatus\":%d,\"cell\":5,\"flag\":%d,\"count\":%d,\"bytes\":[",cpu,fitStatus,*(int*)(output.data()+0xf0),*(int*)(output.data()+0x118));for(int i=0;i<12;i++)printf("%s%u",i?",":"",output[0x11c+i]);puts("]}");fflush(stdout);
 TerminateProcess(GetCurrentProcess(),fitStatus==1&&*(int*)(output.data()+0xf0)==1?0:5);return 6;
}

