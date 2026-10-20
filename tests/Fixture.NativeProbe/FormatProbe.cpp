#include <windows.h>
#include <cstdio>
#include <cstdint>
#include <cstring>
#include <clocale>
using Format=int(__cdecl*)(char*,size_t,const char*,...);
int wmain(){const wchar_t* path=L"D:/Program Files/VisionMaster4.4.0/Applications/PublicFile/x64/msvcr120.dll";auto library=LoadLibraryW(path);if(!library)return 2;auto format=(Format)GetProcAddress(library,"sprintf_s");auto crtLocale=(char*(__cdecl*)(int,const char*))GetProcAddress(library,"setlocale");if(!format||!crtLocale)return 3;char* locale=crtLocale(LC_ALL,nullptr);printf("crt_locale=%s\n",locale?locale:"null");uint32_t values[]={0,0x80000000,0x3bffffff,0x3c000000,0x3c000001,0xbc000000,0x3c800000,0x3f9e0652,0x461c4000,0x3f000000,0x00800000,0x7f7fffff};for(auto bits:values){float value;memcpy(&value,&bits,4);char output[1024]={};int result=format(output,sizeof(output),"%f",(double)value);printf("%08X\t%d\t%s\n",bits,result,output);}FreeLibrary(library);return 0;}
