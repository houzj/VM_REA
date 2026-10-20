#include <windows.h>
#include <cstdio>
#include "MVDImageCpp.h"
#include "MVDCore.h"
using namespace VisionDesigner;
int main(int argc,char**argv){if(argc!=4)return 2;SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);SetDllDirectoryA(argv[1]);auto dll=LoadLibraryA("MVDImageCpp.dll");auto create=(int(__stdcall*)(IMvdImage**))GetProcAddress(dll,"CreateImageInstance");if(!create)return 3;for(int side=0;side<2;side++){IMvdImage*image=nullptr;create(&image);image->InitImage(argv[side+2],MVD_PIXEL_MONO_08);auto channel=image->GetImageData(0);const char*name=side==0?"native-source.gray":"native-target.gray";FILE*f=fopen(name,"wb");for(unsigned row=0;row<image->GetHeight();row++)fwrite(channel->pData+row*channel->nRowStep,1,image->GetWidth(),f);fclose(f);printf("{\"side\":%d,\"width\":%u,\"height\":%u,\"step\":%u,\"bytes\":%u}\n",side,image->GetWidth(),image->GetHeight(),channel->nRowStep,channel->nLen);}fflush(stdout);TerminateProcess(GetCurrentProcess(),0);}
