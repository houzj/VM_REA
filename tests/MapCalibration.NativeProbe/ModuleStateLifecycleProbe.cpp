#include "MVDCore.h"
#include "MVDImageCpp.h"
#define MAP_IMAGE_HOST
#include "ModuleHost.hpp"
#include "MapCalibCpp.h"
using namespace VisionDesigner; using namespace VisionDesigner::MapCalib;

static void emitResult(IMapCalibTool* tool) {
    IMapCalibResult* result=tool->GetResult();
    printf(",\"sdkGetResult\":%s",result?"true":"false");
    if(result){float h[9]{};result->GetHomoMatrix(h);printf(",\"sdkHomo\":[");for(int i=0;i<9;i++)printf("%s%.9g",i?",":"",h[i]);printf("]");}
}

int main(int argc,char**argv){
    if(argc!=7)return 2;setvbuf(stdout,nullptr,_IONBF,0);SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);SetDllDirectoryA(argv[1]);
    for(auto name:{"HSlog.dll","ModuleFrame.dll"}){auto path=std::string("D:/Program Files/VisionMaster4.4.0/Applications/ModuleProxy/x64/")+name;LoadLibraryExA(path.c_str(),nullptr,LOAD_WITH_ALTERED_SEARCH_PATH);}
    auto module=LoadLibraryExA(argv[2],nullptr,LOAD_WITH_ALTERED_SEARCH_PATH);if(!module)return 3;auto image=LoadLibraryA("MVDImageCpp.dll");
    auto create=(int(__stdcall*)(IMapCalibTool**))GetProcAddress(GetModuleHandleA("MVDMapCalibCpp.dll"),"CreateMapCalibToolInstance");auto ic=(int(__stdcall*)(IMvdImage**))GetProcAddress(image,"CreateImageInstance");auto ctor=(void*(*)(void*))GetProcAddress(module,"??0CIMVSMapCalibModu@@QEAA@XZ");auto set=(int(*)(void*,const char*,const char*,int))GetProcAddress(module,"?SetParam@CIMVSMapCalibModu@@UEAAHPEBD0H@Z");auto process=(int(*)(void*,void*,void*))GetProcAddress(module,"?Process@CIMVSMapCalibModu@@UEAAHPEAX0@Z");if(!create||!ic||!ctor||!set||!process)return 4;
    try{
        IMapCalibTool*tool=nullptr;create(&tool);IMvdImage*a=nullptr,*b=nullptr,*bad=nullptr;ic(&a);ic(&b);ic(&bad);a->InitImage(argv[3],MVD_PIXEL_MONO_08);b->InitImage(argv[4],MVD_PIXEL_MONO_08);bad->InitImage(argv[5],MVD_PIXEL_MONO_08);
        tool->GetBasicParam()->SetMapCalibMode((MVD_MAP_CALIB_MODE)1);tool->GetBasicParam()->GetTwoImageMapBasicParam()->SetCalibBoardType((MVD_CALIBBOARD_TYPE)7);tool->SetRunParam("HomoFreedom","1");tool->SetRunParam("WeightFun","1");
        std::vector<unsigned char>memory(0x100000);ctor(memory.data());*(void**)(memory.data()+0x16598)=tool;set(memory.data(),"InputType","1",1);install(module);
        struct Scenario {const char* name;int source;int target;int fileMode;};
        const Scenario steps[]={{"image-success",1,1,0},{"source-half-pending",1,0,0},{"switch-file",0,0,1},{"switch-image-empty",0,0,0},{"complete-old-half",0,1,0},{"failed-image-candidate",1,2,0}};
        for(int step=0;step<6;step++){
            imageInputs.clear();inputs.clear();strings.clear();calls.clear();writes.clear();
            if(steps[step].fileMode)set(memory.data(),"InputType","2",1);else set(memory.data(),"InputType","1",1);
            if(steps[step].source)imageInputs["InImage1"]=a;
            if(steps[step].target)imageInputs["InImage2"]=(step==5?bad:b);
            auto addImageInputs=[&](const char* side,IMvdImage* im){std::string s=side;inputs[s+"Width"]={(float)im->GetWidth()};inputs[s+"Height"]={(float)im->GetHeight()};inputs[s+"PixelFormat"]={(float)0x1000000};};
            if(steps[step].source)addImageInputs("InImage1",a);if(steps[step].target)addImageInputs("InImage2",step==5?bad:b);
            unsigned code=0;try{code=(unsigned)process(memory.data(),(void*)1,(void*)2);}catch(const IMVDException&e){code=e.GetErrorCode();}catch(...){code=0xffffffff;}
            printf("{\"step\":%d,\"name\":\"%s\",\"inputType\":%d,\"code\":%u,\"pending\":[%d,%d],\"writes\":[",step,steps[step].name,steps[step].fileMode?2:1,code,memory[0x1749a],memory[0x1749b]);
            for(size_t i=0;i<writes.size();i++)printf("%s%s",i?",":"",writes[i].c_str());printf("]");emitResult(tool);printf("}\n");
        }
        restore();
    }catch(const IMVDException&e){restore();printf("{\"exception\":%u}\n",(unsigned)e.GetErrorCode());return 5;}
    fflush(stdout);TerminateProcess(GetCurrentProcess(),0);
}
