// Owned original module constructor + ProcessAlg, source-confirmed SDK pointer at +0x15d98.
// Normal SDK object; no platform Init, device, code patch or authorization workaround.
#include <windows.h>
#include <cstdio>
#include <vector>
#include <string>
#include <filesystem>
#include <map>
#include <cmath>
#include <cstring>
#include "MVDCore.h"
#include "CalibBoardCalibCpp.h"
using namespace VisionDesigner;using namespace VisionDesigner::CalibBoardCalib;
static std::map<std::string,std::vector<float>> inputs;
static std::map<std::string,std::string> strings;
static int instanceSerial=0;
static unsigned absent=0xe000031c;
static std::string failOutput;
static std::vector<std::string> calls,writes;
static std::string num(float x){char s[64];if(!std::isfinite(x))return "null";sprintf_s(s,"%.9g",x);return s;}
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
 std::map<std::string,void*> replacements={{"VM_M_GetFloat",(void*)getFloat},{"VM_M_GetInt",(void*)getInt},{"VM_M_SetFloat",(void*)setFloat},{"VM_M_SetInt",(void*)setInt},{"VM_M_BatchGetFloat",(void*)batchGetFloat},{"VM_M_GetString",(void*)getString},{"VM_M_SetModuleRuntimeInfo",(void*)setRuntime},{"VM_M_BatchSetFloat",(void*)batchSetFloat},{"VM_M_BatchSetInt",(void*)batchSetInt}};
 for(;imports->Name;imports++){const char* library=(const char*)(base+imports->Name);if(_stricmp(library,"ModuleFrame.dll"))continue;auto names=(IMAGE_THUNK_DATA64*)(base+imports->OriginalFirstThunk);auto slots=(IMAGE_THUNK_DATA64*)(base+imports->FirstThunk);for(;names->u1.AddressOfData;names++,slots++){if(IMAGE_SNAP_BY_ORDINAL64(names->u1.Ordinal))continue;auto named=(IMAGE_IMPORT_BY_NAME*)(base+names->u1.AddressOfData);std::string name((char*)named->Name);auto found=replacements.find(name);if(found==replacements.end())continue;void** slot=(void**)&slots->u1.Function;void* expected=(void*)GetProcAddress(GetModuleHandleA("ModuleFrame.dll"),name.c_str());if(*slot!=expected||!expected)throw 12;patched.push_back({slot,*slot,name});put(slot,found->second);}}
 if(patched.empty())throw 13;
 printf("{\"phase\":\"synthetic-host-installed\",\"importCount\":%zu,\"moduleCodeUnchanged\":true,\"platformRuntime\":false}\n",patched.size());fflush(stdout);
}
static void restore(){for(auto& p:patched)put(p.slot,p.original);patched.clear();}



int main(int argc,char**argv){
 SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);setvbuf(stdout,nullptr,_IONBF,0);if(argc!=5)return 2;SetDllDirectoryA(argv[1]);
 for(auto n:{"HSlog.dll","ModuleFrame.dll"}){auto p=std::filesystem::path(argv[2])/"ModuleProxy"/"x64"/n;LoadLibraryExA(p.string().c_str(),nullptr,LOAD_WITH_ALTERED_SEARCH_PATH);}
 auto alg=LoadLibraryA("MVDCalibBoardCalibCpp.dll"),images=LoadLibraryA("MVDImageCpp.dll"),module=LoadLibraryExA(argv[3],nullptr,LOAD_WITH_ALTERED_SEARCH_PATH);if(!alg||!images||!module){printf("{\"phase\":\"load\",\"error\":%lu}\n",GetLastError());return 3;}
 auto create=(int(__stdcall*)(ICalibBoardCalibTool**))GetProcAddress(alg,"CreateCalibBoardCalibToolInstance");auto imageCreate=(int(__stdcall*)(IMvdImage**))GetProcAddress(images,"CreateImageInstance");
 auto ctor=(void*(*)(void*))GetProcAddress(module,"??0CIMVSCalibBoardCalibModu@@QEAA@XZ");auto dtor=(void(*)(void*))GetProcAddress(module,"??1CIMVSCalibBoardCalibModu@@UEAA@XZ");
 auto set=(int(*)(void*,const char*,const char*,int))GetProcAddress(module,"?SetParam@CIMVSCalibBoardCalibModu@@UEAAHPEBD0H@Z");auto get=(int(*)(void*,const char*,char*,int,int*))GetProcAddress(module,"?GetParam@CIMVSCalibBoardCalibModu@@UEAAHPEBDPEADHPEAH@Z");auto run=(int(*)(void*,void*,void*))GetProcAddress(module,"?ProcessAlg@CIMVSCalibBoardCalibModu@@AEAAHPEAU_MVDSDK_BASE_MODU_INPUT@CVmAlgModuleBase@@PEAUBASIC_MODU_OUTPUT@1@@Z");if(!ctor||!dtor||!set||!get||!run)return 4;
 install(module);auto outputProcess=(int(*)(void*,void*,void*,const void*))GetProcAddress(module,"?ProcessOutput@CIMVSCalibBoardCalibModu@@AEAAHPEAU_MVDSDK_BASE_MODU_INPUT@CVmAlgModuleBase@@PEAUBASIC_MODU_OUTPUT@1@QEBX@Z");
 std::vector<unsigned char> bytes(0x20000),input(0x200);ctor(bytes.data());ICalibBoardCalibTool*tool=nullptr;if(create(&tool))return 5;*(void**)(bytes.data()+0x15d98)=tool;
 IMvdImage*image=nullptr;imageCreate(&image);image->InitImage(480,400,MVD_PIXEL_MONO_08);auto data=image->GetImageData(0);for(int y=0;y<400;y++)for(int x=0;x<480;x++){unsigned char p=255;if(x>=60&&x<420&&y>=50&&y<350)p=(((x-60)/30+(y-50)/30)&1)?255:0;data->pData[y*data->nRowStep+x]=p;}*(void**)(input.data()+0x10)=image;
 try{
  auto param=[&](const char*key,const char*value){int c=set(bytes.data(),key,value,(int)strlen(value));printf("{\"phase\":\"set\",\"key\":\"%s\",\"code\":%u}\n",key,(unsigned)c);};
  std::string file=(std::filesystem::path(argv[4])/"generated.iwcal").string();std::filesystem::remove(file);
  param("HomoFreedom","1");param("CalibBoardWeightFun","2");param("CalibPathName",file.c_str());param("RefreshFileEnable","False");
  int outputData[8]={};auto execute=[&](const char*phase){int*output=outputData;memset(outputData,0,sizeof(outputData));int code=run(bytes.data(),input.data(),output);bool exists=std::filesystem::exists(file);printf("{\"phase\":\"%s\",\"code\":%u,\"status\":%d,\"exists\":%s,\"length\":%llu}\n",phase,(unsigned)code,output[0],exists?"true":"false",exists?(unsigned long long)std::filesystem::file_size(file):0);};
  auto readParam=[&](const char*key){char value[256]{};int length=-1;int code=get(bytes.data(),key,value,sizeof(value),&length);printf("{\"phase\":\"get\",\"key\":\"%s\",\"code\":%u,\"length\":%d,\"value\":\"%s\"}\n",key,(unsigned)code,length,value);};
  execute("missing-path-refresh-false");FILE*f=fopen(file.c_str(),"ab");if(f){fputc(77,f);fclose(f);}execute("existing-path-refresh-false");param("RefreshFileEnable","True");execute("existing-path-refresh-true");
  *(int*)(input.data()+4)=1;float roi[]={240,200,480,400,0};memcpy(input.data()+0xa4,roi,sizeof(roi));
  // Controlled EdgeThreshLow/High replay: same original module + SDK object and identical image.
  // The XML SetParam route is exercised; GetParam uses the exact decorated ABI exported by this DLL.
  auto edgeExecute=[&](const char*phase){execute(phase);writes.clear();int c=outputProcess(bytes.data(),input.data(),outputData,(void*)1);std::string joined;for(auto&w:writes)joined+=w;unsigned long long hash=1469598103934665603ull;for(unsigned char ch:joined){hash^=ch;hash*=1099511628211ull;}printf("{\"phase\":\"%s-output\",\"code\":%u,\"writeCount\":%zu,\"fnv1a64\":\"%016llx\",\"writes\":[",phase,(unsigned)c,writes.size(),hash);for(size_t i=0;i<writes.size();i++)printf("%s%s",i?",":"",writes[i].c_str());printf("]}\n");};
  param("EdgeThreshLow","20");param("EdgeThreshHigh","40");readParam("EdgeThreshLow");readParam("EdgeThreshHigh");edgeExecute("edge-default-low20-high40");
  param("EdgeThreshLow","0");readParam("EdgeThreshLow");readParam("EdgeThreshHigh");edgeExecute("edge-low0-high40");
  param("EdgeThreshLow","20");param("EdgeThreshHigh","80");readParam("EdgeThreshLow");readParam("EdgeThreshHigh");edgeExecute("edge-low20-high80");
  writes.clear();int outputCode=outputProcess(bytes.data(),input.data(),outputData,(void*)1);printf("{\"phase\":\"outputs\",\"code\":%u,\"writes\":[",(unsigned)outputCode);for(size_t i=0;i<writes.size();i++)printf("%s%s",i?",":"",writes[i].c_str());printf("]}\n");
 }catch(IMVDException&e){printf("{\"phase\":\"sdk-exception\",\"code\":%u}\n",e.GetErrorCode());return 6;}catch(...){printf("{\"phase\":\"exception\"}\n");return 7;}
 dtor(bytes.data());restore();return 0;
}

