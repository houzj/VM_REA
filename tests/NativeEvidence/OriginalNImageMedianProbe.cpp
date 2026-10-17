// Controlled original internal median function; owned inputs/process only, no equipment.
#include <windows.h>
#include <cstdio>
#include <vector>
#include <cstdint>
int main(int argc,char**argv){if(argc<2)return 2;setvbuf(stdout,nullptr,_IONBF,0);SetDllDirectoryA(argv[1]);HMODULE core=LoadLibraryA("MVD_Algorithm.dll");if(!core){printf("{\"loadError\":%lu}\n",GetLastError());return 3;}
 using Filter=void(__fastcall*)(const unsigned char*,int,unsigned char*,int,uint64_t);auto filter=(Filter)((unsigned char*)core+0x8afe10);
 for(int width:{8,17,32}){int height=9,stride=width+32;std::vector<unsigned char> input(stride*height+128,255),output(stride*height+128,170);uint32_t seed=0x1937+width;for(int y=0;y<height;y++)for(int x=0;x<width;x++){seed=1664525*seed+1013904223;input[y*stride+x]=(unsigned char)(seed>>24);}filter(input.data(),stride,output.data(),stride,(uint64_t(height)<<32)|width);printf("{\"width\":%d,\"height\":%d,\"input\":[",width,height);for(int y=0;y<height;y++)for(int x=0;x<width;x++)printf("%s%d",y||x?",":"",input[y*stride+x]);printf("],\"output\":[");for(int y=0;y<height;y++)for(int x=0;x<width;x++)printf("%s%d",y||x?",":"",output[y*stride+x]);printf("]}\n");}
 // Core unload is unsupported in this synthetic host. Flush and end this owned probe only.
 fflush(stdout);TerminateProcess(GetCurrentProcess(),0);return 0;}
