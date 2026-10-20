#include "MVDCore.h"
#include "MVDImageCpp.h"
#define MAP_IMAGE_HOST
#include "ModuleHost.hpp"
#include "MapCalibCpp.h"
using namespace VisionDesigner;using namespace VisionDesigner::MapCalib;
int main(int argc,char**argv){if(argc!=6)return 2;setvbuf(stdout,nullptr,_IONBF,0);SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);SetDllDirectoryA(argv[1]);
 for(auto name:{"HSlog.dll","ModuleFrame.dll"}){auto path=std::string("D:/Program Files/VisionMaster4.4.0/Applications/ModuleProxy/x64/")+name;LoadLibraryExA(path.c_str(),nullptr,LOAD_WITH_ALTERED_SEARCH_PATH);}
 auto module=LoadLibraryExA(argv[2],nullptr,LOAD_WITH_ALTERED_SEARCH_PATH);if(!module)return 3;auto image=LoadLibraryA("MVDImageCpp.dll");
 auto create=(int(__stdcall*)(IMapCalibTool**))GetProcAddress(GetModuleHandleA("MVDMapCalibCpp.dll"),"CreateMapCalibToolInstance");auto ic=(int(__stdcall*)(IMvdImage**))GetProcAddress(image,"CreateImageInstance");
 auto ctor=(void*(*)(void*))GetProcAddress(module,"??0CIMVSMapCalibModu@@QEAA@XZ");auto set=(int(*)(void*,const char*,const char*,int))GetProcAddress(module,"?SetParam@CIMVSMapCalibModu@@UEAAHPEBD0H@Z");auto process=(int(*)(void*,void*,void*))GetProcAddress(module,"?Process@CIMVSMapCalibModu@@UEAAHPEAX0@Z");if(!create||!ic||!ctor||!set||!process)return 4;
 try{IMapCalibTool*tool=nullptr;create(&tool);IMvdImage*a=nullptr,*b=nullptr;ic(&a);ic(&b);a->InitImage(argv[3],MVD_PIXEL_MONO_08);b->InitImage(argv[4],MVD_PIXEL_MONO_08);tool->GetBasicParam()->SetMapCalibMode((MVD_MAP_CALIB_MODE)1);tool->GetBasicParam()->GetTwoImageMapBasicParam()->SetCalibBoardType((MVD_CALIBBOARD_TYPE)7);tool->SetRunParam("HomoFreedom","1");tool->SetRunParam("WeightFun","1");
 std::vector<unsigned char> memory(0x100000);ctor(memory.data());*(void**)(memory.data()+0x16598)=tool;set(memory.data(),"InputType","1",1);install(module);
 for(int step=0;step<3;step++){
  unsigned char packet[22]={2,0};float normalized[]={.5f,.5f,.8f,.8f,0};memcpy(packet+2,normalized,20);unsigned char full[]={1,0};const char*data=(const char*)(step?packet:full);int length=step?22:2;
  int set0=set(memory.data(),"RoiType_0",data,length),set1=set(memory.data(),"RoiType_1",data,length);
  imageInputs.clear();inputs.clear();strings.clear();calls.clear();writes.clear();
  auto add=[&](int side,IMvdImage*im){std::string n="InImage"+std::to_string(side);imageInputs[n]=im;inputs[n+"Width"]={(float)im->GetWidth()};inputs[n+"Height"]={(float)im->GetHeight()};inputs[n+"PixelFormat"]={(float)0x1000000};};add(1,a);add(2,b);
  if(step==2)for(int side=1;side<=2;side++){std::string prefix="Roi"+std::to_string(side);inputs[prefix+"CenterX"]={400};inputs[prefix+"CenterY"]={410};inputs[prefix+"Width"]={600};inputs[prefix+"Height"]={610};inputs[prefix+"Angle"]={7};}
  float before[10],after[10];for(int side=0;side<2;side++)memcpy(before+side*5,memory.data()+0x1695c+side*0x128,20);
  unsigned code=0;try{code=(unsigned)process(memory.data(),(void*)1,(void*)2);}catch(const IMVDException&e){code=e.GetErrorCode();}catch(...){code=0xffffffff;}
  for(int side=0;side<2;side++)memcpy(after+side*5,memory.data()+0x1695c+side*0x128,20);
  printf("{\"step\":%d,\"set\":[%d,%d],\"code\":%u,\"before\":%s,\"after\":%s,\"writes\":[",step,set0,set1,code,array(before,10).c_str(),array(after,10).c_str());bool first=true;for(auto&w:writes)if(w.find("Detect")!=std::string::npos||w.find("PointNum")!=std::string::npos||w.find("Status")!=std::string::npos){printf("%s%s",first?"":",",w.c_str());first=false;}printf("],\"calls\":[");for(size_t i=0;i<calls.size();i++)printf("%s%s",i?",":"",calls[i].c_str());puts("]}");
 }
 restore();}catch(const IMVDException&e){restore();printf("{\"exception\":%u}\n",(unsigned)e.GetErrorCode());return 5;}
 fflush(stdout);TerminateProcess(GetCurrentProcess(),0);return 6;
}
