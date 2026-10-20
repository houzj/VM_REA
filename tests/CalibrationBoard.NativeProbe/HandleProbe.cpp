// Original documented SDK, owned synthetic images only; no platform module/device.
#include <windows.h>
#include <cstdio>
#include <vector>
#include <cmath>
#include "MVDCore.h"
#include "CalibBoardCalibCpp.h"
using namespace VisionDesigner;
using namespace VisionDesigner::CalibBoardCalib;
int main(int argc,char**argv){
 if(argc!=2)return 2;SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);setvbuf(stdout,nullptr,_IONBF,0);SetDllDirectoryA(argv[1]);
 auto lib=LoadLibraryA("MVDCalibBoardCalibCpp.dll"),images=LoadLibraryA("MVDImageCpp.dll");if(!lib||!images){printf("{\"phase\":\"load\",\"error\":%lu}\n",GetLastError());return 3;}
 auto create=(int(__stdcall*)(ICalibBoardCalibTool**))GetProcAddress(lib,"CreateCalibBoardCalibToolInstance");
 auto destroy=(int(__stdcall*)(ICalibBoardCalibTool*))GetProcAddress(lib,"DestroyCalibBoardCalibToolInstance");
 auto imageCreate=(int(__stdcall*)(IMvdImage**))GetProcAddress(images,"CreateImageInstance");auto imageDestroy=(int(__stdcall*)(IMvdImage*))GetProcAddress(images,"DestroyImageInstance");
 ICalibBoardCalibTool*tool=nullptr;int code=create(&tool);printf("{\"phase\":\"create\",\"code\":%u}\n",(unsigned)code);if(code)return 4;
 try{unsigned n=0;try{tool->SaveConfiguration(nullptr,0,&n);}catch(...){}std::vector<unsigned char>xml(n+1);tool->SaveConfiguration(xml.data(),n,&n);FILE*f=fopen("original-config.xml","wb");if(f){fwrite(xml.data(),1,n,f);fclose(f);}}catch(...){}
 tool->SetRunParam("HomoFreedom","1");tool->SetRunParam("CalibBoardWeightFun","2");
 void*handle=*(void**)((char*)tool+0x28);void**table=*(void***)handle;auto core=(char*)GetModuleHandleA("MVD_Algorithm.dll");printf("{\"phase\":\"source-confirmed-handle-vtable\",\"slots\":[");for(int i=0;i<10;i++)printf("%s\"%llx\"",i?",":"",0x180000000ull+(char*)table[i]-core);printf("]}\n");void**methods=*(void***)tool;printf("{\"phase\":\"sdk-run-address\",\"address\":\"%llx\"}\n",0x180000000ull+(char*)methods[15]-(char*)lib);destroy(tool);return 0;
}


