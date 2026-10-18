// Owned original camera-map module with normally-created SDK tools attached to source-confirmed +0xc0/+0xc8. No platform Init.
// Only ModuleFrame input/output/runtime imports are redirected to an explicit synthetic host.
// No algorithm/license/code patch, platform identity, disk DLL change, or physical device.
#include <windows.h>
#include <vector>
#include <string>
#include <map>
#include <cstdio>
#include <cstring>
#include <cmath>
#include <filesystem>
#include "CameraMapCpp.h"
#include "MatrixToParamCpp.h"


static std::map<std::string,std::vector<float>> inputs;
static std::map<std::string,std::string> strings;
static int instanceSerial=0;
static unsigned absent=0xe000031c;
static std::string failOutput;
static std::vector<std::string> calls,writes;
static std::string num(float x){char s[64];if(!std::isfinite(x))return "null";sprintf_s(s,"%.9g",x);return s;}
static std::string escape(const char* p){std::string s;for(;*p;p++){if(*p=='\\'||*p=='"')s+='\\';s+=*p;}return s;}
static std::string array(const float* p,int n){std::string s="[";for(int i=0;i<n;i++){if(i)s+=",";s+=num(p[i]);}return s+"]";}
static void getter(const char* op,const char* name,unsigned code,int count){calls.push_back("{\"op\":\""+std::string(op)+"\",\"name\":\""+name+"\",\"code\":"+std::to_string(code)+",\"count\":"+std::to_string(count)+"}");}
static int getFloat(void*,const char* name,int index,float* value,int* count){auto it=inputs.find(name);if(it==inputs.end()){*count=0;getter("GetFloat",name,absent,0);return absent;}*count=(int)it->second.size();if(index>=0&&index<*count)*value=it->second[index];getter("GetFloat",name,0,*count);return 0;}
static int getInt(void*,const char* name,int index,int* value,int* count){auto it=inputs.find(name);if(it==inputs.end()){*count=0;getter("GetInt",name,absent,0);return absent;}*count=(int)it->second.size();if(index>=0&&index<*count)*value=(int)it->second[index];getter("GetInt",name,0,*count);return 0;}
static int batchGetFloat(void*,const char* name,int capacity,float* value,int* count){auto it=inputs.find(name);if(it==inputs.end()){*count=0;getter("BatchGetFloat",name,absent,0);return absent;}*count=(int)it->second.size();memcpy(value,it->second.data(),min(capacity,*count)*sizeof(float));getter("BatchGetFloat",name,0,*count);return 0;}
static int getString(void*,const char* name,int index,char* value,int capacity,int* bytes,int* count){auto it=strings.find(name);if(it==strings.end()){*bytes=*count=0;getter("GetString",name,absent,0);return absent;}*bytes=(int)it->second.size();*count=1;if(capacity<=*bytes)return 0xe0000012;memcpy(value,it->second.c_str(),*bytes+1);getter("GetString",name,0,1);return 0;}
static int write(const char* name,const char* kind,const float* values,int count){int code=failOutput==name?(int)0xe0000001:0;writes.push_back("{\"name\":\""+std::string(name)+"\",\"type\":\""+kind+"\",\"values\":"+array(values,count)+",\"returned\":"+std::to_string((unsigned)code)+"}");return code;}
static int setFloat(void*,const char* name,int index,float value,int){return write(name,"float",&value,1);}
static int setInt(void*,const char* name,int index,int value){float f=(float)value;return write(name,"int",&f,1);}
static int batchSetFloat(void*,const char* name,float* values,int count,int){return write(name,"float",values,count);}
static int batchSetInt(void*,const char* name,int* values,int count,int){std::vector<float> v(count);for(int i=0;i<count;i++)v[i]=(float)values[i];return write(name,"int",v.data(),count);}
static int getRuntime(void*,void* data){memset(data,0,0x2b0);return 0;}
static int setRuntime(void*,void*){return 0;}
struct Import {void** slot;void* original;std::string name;};
static std::vector<Import> patched;
static void put(void** slot,void* value){DWORD old; if(!VirtualProtect(slot,sizeof(void*),PAGE_READWRITE,&old))throw 11;*slot=value;DWORD ignored;VirtualProtect(slot,sizeof(void*),old,&ignored);}
static void install(HMODULE module){auto base=(unsigned char*)module;auto dos=(IMAGE_DOS_HEADER*)base;auto nt=(IMAGE_NT_HEADERS64*)(base+dos->e_lfanew);auto imports=(IMAGE_IMPORT_DESCRIPTOR*)(base+nt->OptionalHeader.DataDirectory[IMAGE_DIRECTORY_ENTRY_IMPORT].VirtualAddress);
 std::map<std::string,void*> replacements={{"VM_M_GetFloat",(void*)getFloat},{"VM_M_GetInt",(void*)getInt},{"VM_M_SetFloat",(void*)setFloat},{"VM_M_SetInt",(void*)setInt},{"VM_M_BatchGetFloat",(void*)batchGetFloat},{"VM_M_GetString",(void*)getString},{"VM_M_SetModuleRuntimeInfo",(void*)setRuntime}};
 for(;imports->Name;imports++){const char* library=(const char*)(base+imports->Name);if(_stricmp(library,"ModuleFrame.dll"))continue;auto names=(IMAGE_THUNK_DATA64*)(base+imports->OriginalFirstThunk);auto slots=(IMAGE_THUNK_DATA64*)(base+imports->FirstThunk);for(;names->u1.AddressOfData;names++,slots++){if(IMAGE_SNAP_BY_ORDINAL64(names->u1.Ordinal))continue;auto named=(IMAGE_IMPORT_BY_NAME*)(base+names->u1.AddressOfData);std::string name((char*)named->Name);auto found=replacements.find(name);if(found==replacements.end())continue;void** slot=(void**)&slots->u1.Function;void* expected=(void*)GetProcAddress(GetModuleHandleA("ModuleFrame.dll"),name.c_str());if(*slot!=expected||!expected)throw 12;patched.push_back({slot,*slot,name});put(slot,found->second);}}
 if(patched.empty())throw 13;
 printf("{\"phase\":\"synthetic-host-installed\",\"importCount\":%zu,\"moduleCodeUnchanged\":true,\"platformRuntime\":false}\n",patched.size());fflush(stdout);
}
static void restore(){for(auto& p:patched)put(p.slot,p.original);patched.clear();}


struct Instance {
 std::vector<unsigned char> bytes=std::vector<unsigned char>(0x100000);
 using Set=int(*)(void*,const char*,const char*,int);using Process=int(*)(void*,void*,void*);
 Set set;Process process;int(*get)(void*,const char*,char*,int,int*);void(*destroy)(void*);
 Instance(HMODULE module){auto ctor=(void*(*)(void*))GetProcAddress(module,"??0CIMVSCameraMapModu@@QEAA@XZ");destroy=(void(*)(void*))GetProcAddress(module,"??1CIMVSCameraMapModu@@UEAA@XZ");set=(Set)GetProcAddress(module,"?SetParam@CIMVSCameraMapModu@@UEAAHPEBD0H@Z");get=(int(*)(void*,const char*,char*,int,int*))GetProcAddress(module,"?GetParam@CIMVSCameraMapModu@@UEAAHPEBDPEADHPEAH@Z");process=(Process)GetProcAddress(module,"?Process@CIMVSCameraMapModu@@UEAAHPEAX0@Z");if(!ctor||!destroy||!set||!process||!get)throw 14;ctor(bytes.data());
 auto sdk=GetModuleHandleA("MVDCameraMapCpp.dll");auto create=(int(__stdcall*)(VisionDesigner::CameraMap::ICameraMapTool**))GetProcAddress(sdk,"CreateCameraMapToolInstance");
 VisionDesigner::CameraMap::ICameraMapTool* tool=nullptr;if(!create||create(&tool))throw 17;*(void**)(bytes.data()+0xc0)=tool;
 auto matrix=GetModuleHandleA("MVDMatrixToParamCpp.dll");auto createMatrix=(int(__stdcall*)(VisionDesigner::MatrixToParam::IMatrixToParamTool**))GetProcAddress(matrix,"CreateMatrixToParamToolInstance");
 VisionDesigner::MatrixToParam::IMatrixToParamTool* mt=nullptr;if(!createMatrix||createMatrix(&mt))throw 18;*(void**)(bytes.data()+0xc8)=mt;
 }
 ~Instance(){destroy(bytes.data());}
 void param(const char*key,const char*value){int code=set(bytes.data(),key,value,(int)strlen(value));printf("{\"phase\":\"set\",\"key\":\"%s\",\"code\":%u}\n",key,(unsigned)code);}
 void read(const char*key){char value[4096]={};int size=0;int code=get(bytes.data(),key,value,4096,&size);printf("{\"phase\":\"get\",\"key\":\"%s\",\"code\":%u,\"value\":\"%s\"}\n",key,(unsigned)code,escape(value).c_str());}
 void run(const char* phase){calls.clear();writes.clear();int in=1,out=2;int code=process(bytes.data(),&in,&out);printf("{\"phase\":\"%s\",\"code\":%u,\"writes\":[",phase,(unsigned)code);bool first=true;for(auto&i:writes){if(!first)printf(",");first=false;printf("%s",i.c_str());}printf("],\"getters\":[");first=true;for(auto&i:calls){if(!first)printf(",");first=false;printf("%s",i.c_str());}printf("]}\n");fflush(stdout);}
};
int main(int argc,char**argv){SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX|SEM_NOOPENFILEERRORBOX);setvbuf(stdout,nullptr,_IONBF,0);if(argc!=3)return 2;auto apps=std::filesystem::path(argv[1]).parent_path().parent_path().parent_path().parent_path().parent_path();for(auto name:{"HSlog.dll","ModuleFrame.dll"}){auto p=apps/"ModuleProxy"/"x64"/name;LoadLibraryExA(p.string().c_str(),nullptr,LOAD_WITH_ALTERED_SEARCH_PATH);}auto module=LoadLibraryExA(argv[1],nullptr,LOAD_WITH_ALTERED_SEARCH_PATH);if(!module)return 3;try{install(module);{
 Instance m(module);for(auto key:{"HomoFreedom","WeightFun","WeightFactor","Data1","Data2","Data3","RefreshFileEnable","CalibPathName"})m.read(key);
 m.param("HomoFreedom","1");m.param("WeightFun","3");m.param("WeightFactor","20");m.param("Data1","Data1#1");m.param("Data2","Data2#1");m.run("absent");inputs={{"ObjectPointX1",{0}},{"ObjectPointY1",{0}},{"TargetPointX1",{10}},{"TargetPointY1",{-5}}};m.run("one-pair-other-group-absent");inputs.clear();m.param("Data1","Data1#0");m.param("Data2","Data2#0");m.run("no-active-groups");m.param("Data1","Data1#1");m.param("Data2","Data2#1");
 for(int n=2;n<=4;n++){
  inputs.clear();for(int i=1;i<=n;i++){float x=(i-1)%2*10.f,y=(i-1)/2*10.f;inputs["ObjectPointX"+std::to_string(i)]={x};inputs["ObjectPointY"+std::to_string(i)]={y};inputs["TargetPointX"+std::to_string(i)]={2*x+10};inputs["TargetPointY"+std::to_string(i)]={2*y-5};if(i>2){auto key="Data"+std::to_string(i);auto value=key+"#1";m.param(key.c_str(),value.c_str());}}
  m.run(("points-"+std::to_string(n)).c_str());
 }
 m.param("SaveCalibPath",argv[2]);
 for(int i=1;i<=4;i++){float x=(i-1)%2*10.f,y=(i-1)/2*10.f,z=.002f*x-.001f*y+1;inputs["ObjectPointX"+std::to_string(i)]={x};inputs["ObjectPointY"+std::to_string(i)]={y};inputs["TargetPointX"+std::to_string(i)]={(1.5f*x+.1f*y+10)/z};inputs["TargetPointY"+std::to_string(i)]={(-.2f*x+2*y-5)/z};}m.run("perspective-four-points");auto perspectivePath=std::string(argv[2])+".perspective.iwcal";m.param("SaveCalibPath",perspectivePath.c_str());m.param("HomoFreedom","2");auto recalculatedPath=std::string(argv[2])+".recalculated-affine.iwcal";m.param("SaveCalibPath",recalculatedPath.c_str());m.read("CalibPathName");m.param("HomoFreedom","1");
 m.param("Data3","Data3#0");m.param("Data4","Data4#0");inputs={{"ObjectPointX1",{0,10}},{"ObjectPointY1",{0,0}},{"TargetPointX1",{10,30}},{"TargetPointY1",{-5,-5}},{"ObjectPointX2",{0,10}},{"ObjectPointY2",{10,10}},{"TargetPointX2",{10,30}},{"TargetPointY2",{15,15}}};m.run("two-vector-groups-four-points");
 inputs["ObjectPointX1"]={0,10};inputs["ObjectPointY1"]={0};m.run("mismatched-coordinate-count");
 inputs.erase("TargetPointY1");m.run("missing-required-coordinate");m.param("SaveCalibPath",argv[2]);
m.param("Data3","Data3#0");m.param("Data4","Data4#0");
 inputs={{"ObjectPointX1",{1}},{"ObjectPointY1",{2}},{"TargetPointX1",{4}},{"TargetPointY1",{5}},{"ObjectPointX2",{1}},{"ObjectPointY2",{2}},{"TargetPointX2",{6}},{"TargetPointY2",{9}}};m.run("singular-object-two-points");printf("{\"phase\":\"explicit-after-singular-matrix\"}\n");auto singularPath=std::string(argv[2])+".singular.iwcal";m.param("SaveCalibPath",singularPath.c_str());
 inputs["ObjectPointX2"]={10};inputs["ObjectPointY2"]={0};inputs["TargetPointX2"]={4};inputs["TargetPointY2"]={5};m.run("coincident-target-two-points");
inputs={{"ObjectPointX1",{0}},{"ObjectPointY1",{0}},{"TargetPointX1",{10}},{"TargetPointY1",{-5}},{"ObjectPointX2",{10}},{"ObjectPointY2",{0}},{"TargetPointX2",{30}},{"TargetPointY2",{-5}}};auto refreshPath=std::string(argv[2])+".refresh.iwcal";m.param("CalibPathName",refreshPath.c_str());m.param("RefreshFileEnable","True");m.run("refresh-file-valid-input");
 auto invalidPath=std::string(argv[2])+".missing-parent/calibration.iwcal";m.param("CalibPathName",invalidPath.c_str());m.run("refresh-file-invalid-parent");printf("{\"phase\":\"explicit-invalid-parent\"}\n");m.param("SaveCalibPath",invalidPath.c_str());printf("{\"phase\":\"explicit-after-io-failure\"}\n");auto afterIoPath=std::string(argv[2])+".after-io-failure.iwcal";m.param("SaveCalibPath",afterIoPath.c_str());m.read("CalibPathName");m.param("CalibPathName",argv[2]);inputs.clear();m.run("refresh-file-no-input");
 }restore();fflush(stdout);TerminateProcess(GetCurrentProcess(),0);return 0;}catch(...){restore();return 5;}}
