// Owned original module instance + normally created official algorithm.
// Only ModuleFrame input/output/runtime imports are redirected to an explicit synthetic host.
// No algorithm/license/code patch, platform identity, disk DLL change, or physical device.
#include <windows.h>
#include <vector>
#include <string>
#include <map>
#include <cstdio>
#include <cstring>
#include <cmath>
#include "NPointCalibCpp.h"
using namespace VisionDesigner::NPointCalib;
static std::map<std::string,std::vector<float>> inputs;
static std::map<std::string,std::string> strings;
static unsigned absent=0xe000031c;
static std::string failOutput;
static std::vector<std::string> calls,writes;
static std::string num(float x){char s[64];if(!std::isfinite(x))return "null";sprintf_s(s,"%.9g",x);return s;}
static std::string array(const float* p,int n){std::string s="[";for(int i=0;i<n;i++){if(i)s+=",";s+=num(p[i]);}return s+"]";}
static void getter(const char* op,const char* name,unsigned code,int count){calls.push_back("{\"op\":\""+std::string(op)+"\",\"name\":\""+name+"\",\"code\":"+std::to_string(code)+",\"count\":"+std::to_string(count)+"}");}
static int getFloat(void*,const char* name,int index,float* value,int* count){auto it=inputs.find(name);if(it==inputs.end()){*count=0;getter("GetFloat",name,absent,0);return absent;}*count=(int)it->second.size();if(index>=0&&index<*count)*value=it->second[index];getter("GetFloat",name,0,*count);return 0;}
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
 std::map<std::string,void*> replacements={{"VM_M_GetFloat",(void*)getFloat},{"VM_M_BatchGetFloat",(void*)batchGetFloat},{"VM_M_GetString",(void*)getString},{"VM_M_SetFloat",(void*)setFloat},{"VM_M_SetInt",(void*)setInt},{"VM_M_BatchSetFloat",(void*)batchSetFloat},{"VM_M_BatchSetInt",(void*)batchSetInt},{"VM_M_GetModuleRuntimeInfo",(void*)getRuntime},{"VM_M_SetModuleRuntimeInfo",(void*)setRuntime}};
 for(;imports->Name;imports++){const char* library=(const char*)(base+imports->Name);if(_stricmp(library,"ModuleFrame.dll"))continue;auto names=(IMAGE_THUNK_DATA64*)(base+imports->OriginalFirstThunk);auto slots=(IMAGE_THUNK_DATA64*)(base+imports->FirstThunk);for(;names->u1.AddressOfData;names++,slots++){if(IMAGE_SNAP_BY_ORDINAL64(names->u1.Ordinal))continue;auto named=(IMAGE_IMPORT_BY_NAME*)(base+names->u1.AddressOfData);std::string name((char*)named->Name);auto found=replacements.find(name);if(found==replacements.end())continue;void** slot=(void**)&slots->u1.Function;void* expected=(void*)GetProcAddress(GetModuleHandleA("ModuleFrame.dll"),name.c_str());if(*slot!=expected||!expected)throw 12;patched.push_back({slot,*slot,name});put(slot,found->second);}}
 if(patched.size()!=replacements.size())throw 13;
 printf("{\"phase\":\"synthetic-host-installed\",\"importCount\":%zu,\"moduleCodeUnchanged\":true,\"platformRuntime\":false}\n",patched.size());fflush(stdout);
}
static void restore(){for(auto& p:patched)put(p.slot,p.original);patched.clear();}
struct Instance {
 std::vector<unsigned char> bytes=std::vector<unsigned char>(0x16608);
 using Set=int(*)(void*,const char*,const char*,int);using Process=int(*)(void*,void*,void*);
 Set set;Process process;void(*destroy)(void*);
 Instance(HMODULE module,HMODULE algorithm){auto ctor=(void*(*)(void*))GetProcAddress(module,"??0CIMVSNPointCalibModu@@QEAA@XZ");destroy=(void(*)(void*))GetProcAddress(module,"??1CIMVSNPointCalibModu@@UEAA@XZ");set=(Set)GetProcAddress(module,"?SetParam@CIMVSNPointCalibModu@@UEAAHPEBD0H@Z");process=(Process)GetProcAddress(module,"?Process@CIMVSNPointCalibModu@@UEAAHPEAX0@Z");ctor(bytes.data());INPointCalibTool* tool=nullptr;auto create=(int(__stdcall*)(INPointCalibTool**))GetProcAddress(algorithm,"CreateNPointCalibToolInstance");int code=create(&tool);if(code)throw code;*(INPointCalibTool**)(bytes.data()+0x15d98)=tool;}
 ~Instance(){destroy(bytes.data());}
 void param(const char* key,const char* value){int code=set(bytes.data(),key,value,(int)strlen(value)+1);printf("{\"phase\":\"set\",\"key\":\"%s\",\"value\":\"%s\",\"code\":%u}\n",key,value,(unsigned)code);fflush(stdout);}
 void snapshot(const char* phase,int code){printf("{\"phase\":\"%s\",\"code\":%u,\"rawMembers\":{\"current\":%d,\"translation\":%d,\"rotation\":%d,\"angles163b0\":%s,\"angles16430\":%s},\"inputs\":{",phase,(unsigned)code,*(int*)(bytes.data()+0x164c0),*(int*)(bytes.data()+0x164b8),*(int*)(bytes.data()+0x164bc),array((float*)(bytes.data()+0x163b0),32).c_str(),array((float*)(bytes.data()+0x16430),32).c_str());bool first=true;for(auto& input:inputs){if(!first)printf(",");first=false;printf("\"%s\":%s",input.first.c_str(),array(input.second.data(),(int)input.second.size()).c_str());}printf("},\"strings\":{");first=true;for(auto& input:strings){if(!first)printf(",");first=false;printf("\"%s\":\"%s\"",input.first.c_str(),input.second.c_str());}printf("},\"getters\":[");first=true;for(auto& call:calls){if(!first)printf(",");first=false;printf("%s",call.c_str());}printf("],\"writes\":[");first=true;for(auto& out:writes){if(!first)printf(",");first=false;printf("%s",out.c_str());}printf("]}\n");fflush(stdout);}
 void run(const char* phase){calls.clear();writes.clear();int in=1,out=2;int code=process(bytes.data(),&in,&out);snapshot(phase,code);}
};
static void reset(){inputs.clear();strings.clear();calls.clear();writes.clear();absent=0xe000031c;failOutput.clear();}
static void points(int start,int count,bool physical=true,bool angles=true){inputs.clear();for(int i=start;i<start+count;i++){float x=(i%3)*10.f,y=(i/3)*10.f;inputs["ImagePointX"].push_back(x);inputs["ImagePointY"].push_back(y);if(physical){inputs["PhysicalPointX"].push_back(2*x+10);inputs["PhysicalPointY"].push_back(2*y-5);}if(angles){inputs["ImageRotateAngle"].push_back((float)(i+1));inputs["WorldRotateAngle"].push_back((float)(100+i));}}}
int main(int argc,char** argv){if(argc!=3)return 2;HMODULE algorithm=LoadLibraryA(argv[1]),module=LoadLibraryA(argv[2]);if(!algorithm||!module){printf("{\"phase\":\"load-failed\",\"win32\":%lu}\n",GetLastError());return 3;}try{install(module);
 {Instance m(module,algorithm);reset();m.run("default-no-input-31c");absent=0xe000031b;m.run("default-no-input-31b");absent=0xe000031c;inputs={{"ImagePointX",{}},{"ImagePointY",{}}};m.run("bound-empty-images");reset();inputs={{"ImagePointX",{0}},{"ImagePointY",{0}},{"PhysicalPointX",{10}}};m.run("mismatch-physical-counts");reset();points(0,2);m.run("trigger-two");inputs["ImagePointY"]={0};m.run("mismatch-image-counts");points(2,6);m.run("trigger-six-reaching-eight");points(8,4);m.run("batch-cross-translation-boundary");points(9,3);m.run("rotation-three");points(12,2);m.run("full-keeps-values");reset();m.run("full-no-input");m.param("Clear","1");calls.clear();writes.clear();m.snapshot("after-clear-raw",0);m.run("after-clear-no-input");points(0,1,false,false);m.run("default-generated-physical");points(1,3);inputs["PhysicalPointX"]={30};inputs["PhysicalPointY"]={-5};m.run("short-physical-prefix");failOutput="OutImagePointY";points(1,1);m.run("synthetic-output-failure");}
 {Instance m(module,algorithm);reset();m.param("RotPointTotalNum","0");points(0,9);m.run("translation-only-complete");reset();m.run("translation-only-full-no-input");m.param("Clear","1");m.param("TeachEnable","True");strings={{"Trigger","teach"},{"TeachFlagInput","different"}};points(0,1);m.run("teach-mismatch-samples");strings["TeachFlagInput"]="teach";m.run("teach-match-incomplete");inputs.erase("PhysicalPointX");m.run("teach-match-missing-physical-x");strings.clear();inputs.clear();m.run("teach-next-no-input");}
 {Instance m(module,algorithm);reset();m.param("CalibPointGet","1");m.param("RotPointTotalNum","0");for(int i=0;i<9;i++){float x=(i%3)*10.f,y=(i/3)*10.f;for(int j=0;j<4;j++){std::string key=std::string(j==0?"ImagePointX":j==1?"ImagePointY":j==2?"WorldPointX":"WorldPointY")+std::to_string(i);std::string value=std::to_string(j==0?x:j==1?y:j==2?2*x+10:2*y-5);m.param(key.c_str(),value.c_str());}}m.run("manual-table");}
 restore();printf("{\"phase\":\"finished\",\"physicalDevicesTested\":false,\"nativeParityClaimed\":false}\n");return 0;
 }catch(int code){printf("{\"phase\":\"exception\",\"code\":%u}\n",(unsigned)code);restore();return 4;}catch(...){printf("{\"phase\":\"unknown-exception\"}\n");restore();return 5;}}
