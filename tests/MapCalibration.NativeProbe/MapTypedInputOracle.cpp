#include "MVDCore.h"
#include "MVDImageCpp.h"
#define MAP_IMAGE_HOST
#include "ModuleHost.hpp"
#include "MapCalibCpp.h"
using namespace VisionDesigner::MapCalib;
static std::string failFloat,failText;
static int floatCode=0,textCode=0,textCount=1;
static int typedFloat(void* ctx,const char* name,int index,float* value,int* count){
 if(failFloat==name){*count=0;getter("GetFloat",name,(unsigned)floatCode,0);return floatCode;}return getFloat(ctx,name,index,value,count);
}
static int typedText(void* ctx,const char* name,int index,char* value,int cap,int* bytes,int* count){
 int code;
 if(failText==name){*count=textCount;*bytes=0;code=textCode;getter("GetString",name,(unsigned)code,*count);}
 else code=getString(ctx,name,index,value,cap,bytes,count);
 calls.push_back("{\"op\":\"TextABI\",\"name\":\""+std::string(name)+"\",\"index\":"+std::to_string(index)+",\"capacity\":"+std::to_string(cap)+",\"bytes\":"+std::to_string(*bytes)+",\"count\":"+std::to_string(*count)+",\"code\":"+std::to_string((unsigned)code)+"}");return code;
}
int main(int argc,char**argv){if(argc!=4)return 2;SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);setvbuf(stdout,nullptr,_IONBF,0);SetDllDirectoryA(argv[1]);
 for(auto name:{"HSlog.dll","ModuleFrame.dll"}){auto p=std::string("D:/Program Files/VisionMaster4.4.0/Applications/ModuleProxy/x64/")+name;LoadLibraryExA(p.c_str(),nullptr,LOAD_WITH_ALTERED_SEARCH_PATH);}auto module=LoadLibraryExA(argv[2],nullptr,LOAD_WITH_ALTERED_SEARCH_PATH);if(!module)return 3;
 try{install(module);for(auto& p:patched){if(p.name=="VM_M_GetFloat")put(p.slot,(void*)typedFloat);if(p.name=="VM_M_GetString")put(p.slot,(void*)typedText);}
 auto ctor=(void*(*)(void*))GetProcAddress(module,"??0CIMVSMapCalibModu@@QEAA@XZ");auto set=(int(*)(void*,const char*,const char*,int))GetProcAddress(module,"?SetParam@CIMVSMapCalibModu@@UEAAHPEBD0H@Z");auto process=(int(*)(void*,void*,void*))GetProcAddress(module,"?Process@CIMVSMapCalibModu@@UEAAHPEAX0@Z");if(!ctor||!set||!process)return 4;
 for(int test=0;test<10;test++){
  std::vector<unsigned char> mem(0x100000);ctor(mem.data());IMapCalibTool* tool=nullptr;auto create=(int(__stdcall*)(IMapCalibTool**))GetProcAddress(GetModuleHandleA("MVDMapCalibCpp.dll"),"CreateMapCalibToolInstance");if(!create||create(&tool))return 6;*(void**)(mem.data()+0x16598)=tool;inputs={{"PhysicalPointX",{12,999}},{"PhysicalPointY",{13,999}},{"WorldRotateAngle",{14,999}}};strings={{"Trigger","A"},{"TeachFlagInput","A"}};calls.clear();writes.clear();failFloat=failText="";floatCode=textCode=0;textCount=1;
  set(mem.data(),"TeachEnable",test==0?"False":"True",test==0?5:4);mem[0x1749a]=1;
  if(test==1)strings["TeachFlagInput"]="B";
  if(test>=2&&test<=4){failFloat=test==2?"PhysicalPointX":test==3?"PhysicalPointY":"WorldRotateAngle";floatCode=0xe0000123;}
  if(test==5)inputs["PhysicalPointX"]={};
  if(test==6){failText="Trigger";textCode=0xe0000123;textCount=1;strings["TeachFlagInput"]="";}
  if(test==7){failText="TeachFlagInput";textCode=0xe0000123;textCount=1;}
  if(test==8){strings["Trigger"]=strings["TeachFlagInput"]=std::string(511,'Q');}
  if(test==9){strings["Trigger"]=strings["TeachFlagInput"]=std::string(512,'Q');}
  unsigned code=(unsigned)process(mem.data(),(void*)1,(void*)2);float pose[3];memcpy(pose,mem.data()+0x17370,12);
  printf("{\"case\":%d,\"code\":%u,\"teach\":%s,\"pose\":%s,\"writes\":[",test,code,mem[0x17395]?"true":"false",array(pose,3).c_str());for(size_t i=0;i<writes.size();i++)printf("%s%s",i?",":"",writes[i].c_str());printf("],\"calls\":[");for(size_t i=0;i<calls.size();i++)printf("%s%s",i?",":"",calls[i].c_str());printf("]}\n");
 }
 restore();fflush(stdout);TerminateProcess(GetCurrentProcess(),0);
 }catch(...){restore();return 5;}}
