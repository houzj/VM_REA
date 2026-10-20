// Owned IMVSMapCalibModu host helper; ModuleRuntimeProbe attaches the normally-created SDK at source-confirmed +0x16598. No platform Init.
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
#ifdef MAP_IMAGE_HOST
static std::map<std::string,VisionDesigner::IMvdImage*> imageInputs;
#endif
static int instanceSerial=0;
static unsigned absent=0xe000031c;
static std::string failOutput;
static std::vector<std::string> calls,writes;
static std::string num(float x){char s[64];if(!std::isfinite(x))return "null";sprintf_s(s,"%.9g",x);return s;}
static std::string escape(const char* p){std::string s;for(;*p;p++){if(*p=='\\'||*p=='"')s+='\\';s+=*p;}return s;}
static std::string array(const float* p,int n){std::string s="[";for(int i=0;i<n;i++){if(i)s+=",";s+=num(p[i]);}return s+"]";}
static void getter(const char* op,const char* name,unsigned code,int count){calls.push_back("{\"op\":\""+std::string(op)+"\",\"name\":\""+name+"\",\"code\":"+std::to_string(code)+",\"count\":"+std::to_string(count)+"}");}
#ifdef MAP_IMAGE_HOST
static int getImage(void*,const char* name,int,void* data,int* count){auto found=imageInputs.find(name);memset(data,0,0x90);if(found==imageInputs.end()){*count=0;getter("GetImage",name,absent,0);return absent;}auto channel=found->second->GetImageData(0);*(void**)data=channel->pData;*(unsigned*)((char*)data+8)=channel->nLen;*((char*)data+12)=1;*count=1;return 0;}
#endif
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
 std::map<std::string,void*> replacements={{"VM_M_BatchSetFloat",(void*)batchSetFloat},{"VM_M_BatchSetInt",(void*)batchSetInt},{"VM_M_GetFloat",(void*)getFloat},{"VM_M_GetInt",(void*)getInt},{"VM_M_SetFloat",(void*)setFloat},{"VM_M_SetInt",(void*)setInt},{"VM_M_BatchGetFloat",(void*)batchGetFloat},{"VM_M_GetString",(void*)getString},{"VM_M_SetModuleRuntimeInfo",(void*)setRuntime}};
#ifdef MAP_IMAGE_HOST
 replacements["VM_M_GetImage"]=(void*)getImage;
#endif
 for(;imports->Name;imports++){const char* library=(const char*)(base+imports->Name);if(_stricmp(library,"ModuleFrame.dll"))continue;auto names=(IMAGE_THUNK_DATA64*)(base+imports->OriginalFirstThunk);auto slots=(IMAGE_THUNK_DATA64*)(base+imports->FirstThunk);for(;names->u1.AddressOfData;names++,slots++){if(IMAGE_SNAP_BY_ORDINAL64(names->u1.Ordinal))continue;auto named=(IMAGE_IMPORT_BY_NAME*)(base+names->u1.AddressOfData);std::string name((char*)named->Name);auto found=replacements.find(name);if(found==replacements.end())continue;void** slot=(void**)&slots->u1.Function;void* expected=(void*)GetProcAddress(GetModuleHandleA("ModuleFrame.dll"),name.c_str());if(*slot!=expected||!expected)throw 12;patched.push_back({slot,*slot,name});put(slot,found->second);}}
 if(patched.empty())throw 13;
 printf("{\"phase\":\"synthetic-host-installed\",\"importCount\":%zu,\"moduleCodeUnchanged\":true,\"platformRuntime\":false}\n",patched.size());fflush(stdout);
}
static void restore(){for(auto& p:patched)put(p.slot,p.original);patched.clear();}





