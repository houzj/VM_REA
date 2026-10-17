// Original 86ff00 diagnostic on a real help-derived pixel image and explicitly manual quad.
// No physical devices, product dependency, code patch, or desktop operation.
#include <windows.h>
#include <cstdio>
#include <vector>
#include <fstream>
static unsigned invoke(void* fn,void* quads,void* pixels,int stride,unsigned long long size,void* result,void* scratch){__try{((void(*)(void*,void*,int,unsigned long long,void*,void*))fn)(quads,pixels,stride,size,result,scratch);return 0;}__except(EXCEPTION_EXECUTE_HANDLER){return GetExceptionCode();}}
int main(int argc,char**argv){SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);if(argc!=3)return 2;auto module=LoadLibraryA(argv[1]);if(!module)return 3;std::ifstream f(argv[2],std::ios::binary);std::vector<unsigned char> pixels((std::istreambuf_iterator<char>(f)),{});if(pixels.size()!=1000000)return 4;float quad[]={246,253,317,253,294,324,246,324};struct Quads{int count=1;int pad=0;float* points;};Quads quads;quads.points=quad;std::vector<unsigned char> output(4096),scratch(8*1024*1024);printf("{\"phase\":\"before\",\"manualQuad\":[246,253,317,253,294,324,246,324]}\n");fflush(stdout);unsigned exception=invoke((unsigned char*)module+0x86ff00,&quads,pixels.data(),1000,1000ull|(1000ull<<32),output.data(),scratch.data());printf("{\"phase\":\"result\",\"exception\":%u,\"count\":%d,\"flag\":%d,\"bytes\":[",exception,*(int*)(output.data()+0x118),*(int*)(output.data()+0xf0));for(int i=0;i<12;i++)printf("%s%u",i?",":"",output[0x11c+i]);printf("]}\n");fflush(stdout);TerminateProcess(GetCurrentProcess(),exception?5:0);return 0;}
