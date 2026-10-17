// Owned original module instance. Rotate uses its own exported CalibRot, with no platform Init.
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
 std::map<std::string,void*> replacements={{"VM_M_GetFloat",(void*)getFloat},{"VM_M_GetInt",(void*)getInt},{"VM_M_SetFloat",(void*)setFloat},{"VM_M_SetInt",(void*)setInt},{"VM_M_SetModuleRuntimeInfo",(void*)setRuntime}};
 for(;imports->Name;imports++){const char* library=(const char*)(base+imports->Name);if(_stricmp(library,"ModuleFrame.dll"))continue;auto names=(IMAGE_THUNK_DATA64*)(base+imports->OriginalFirstThunk);auto slots=(IMAGE_THUNK_DATA64*)(base+imports->FirstThunk);for(;names->u1.AddressOfData;names++,slots++){if(IMAGE_SNAP_BY_ORDINAL64(names->u1.Ordinal))continue;auto named=(IMAGE_IMPORT_BY_NAME*)(base+names->u1.AddressOfData);std::string name((char*)named->Name);auto found=replacements.find(name);if(found==replacements.end())continue;void** slot=(void**)&slots->u1.Function;void* expected=(void*)GetProcAddress(GetModuleHandleA("ModuleFrame.dll"),name.c_str());if(*slot!=expected||!expected)throw 12;patched.push_back({slot,*slot,name});put(slot,found->second);}}
 if(patched.size()!=replacements.size())throw 13;
 printf("{\"phase\":\"synthetic-host-installed\",\"importCount\":%zu,\"moduleCodeUnchanged\":true,\"platformRuntime\":false}\n",patched.size());fflush(stdout);
}
static void restore(){for(auto& p:patched)put(p.slot,p.original);patched.clear();}

struct Instance {
 std::vector<unsigned char> bytes=std::vector<unsigned char>(0x16000);
 using Set=int(*)(void*,const char*,const char*,int);using Process=int(*)(void*,void*,void*,void*);
 Set set;Process process;void(*destroy)(void*);
 Instance(HMODULE module){auto ctor=(void*(*)(void*))GetProcAddress(module,"??0CAlgorithmModule@@QEAA@XZ");destroy=(void(*)(void*))GetProcAddress(module,"??1CAlgorithmModule@@UEAA@XZ");set=(Set)GetProcAddress(module,"?SetParam@CAlgorithmModule@@UEAAHPEBD0H@Z");process=(Process)GetProcAddress(module,"?Process@CAlgorithmModule@@UEAAHPEAX0PEAU_MVDSDK_BASE_MODU_INPUT@CVmAlgModuleBase@@@Z");if(!ctor||!destroy||!set||!process)throw 14;ctor(bytes.data());printf("{\"phase\":\"instance-start\",\"serial\":%d}\n",++instanceSerial);}
 ~Instance(){destroy(bytes.data());printf("{\"phase\":\"instance-end\"}\n");fflush(stdout);}
 void param(const char* key,const char* value,int delta=0){int code=set(bytes.data(),key,value,(int)strlen(value)+delta);printf("{\"phase\":\"set\",\"key\":\"%s\",\"lengthDelta\":%d,\"code\":%u}\n",key,delta,(unsigned)code);}
 void run(const char* phase){calls.clear();writes.clear();int in=1,out=2;int code=process(bytes.data(),&in,&out,nullptr);printf("{\"phase\":\"%s\",\"code\":%u,\"current\":%d,\"total\":%d,\"remain\":%d,\"loaded\":%s,\"inputs\":{",phase,(unsigned)code,*(int*)(bytes.data()+0x15db0),*(int*)(bytes.data()+0x15db8),*(int*)(bytes.data()+0x15db4),bytes[0x15e44]?"true":"false");bool first=true;for(auto& item:inputs){if(!first)printf(",");first=false;printf("\"%s\":%s",item.first.c_str(),array(item.second.data(),(int)item.second.size()).c_str());}printf("},\"writes\":[");first=true;for(auto& item:writes){if(!first)printf(",");first=false;printf("%s",item.c_str());}printf("],\"getters\":[");first=true;for(auto& item:calls){if(!first)printf(",");first=false;printf("%s",item.c_str());}printf("]}\n");fflush(stdout);}
};
int main(int argc,char**argv){SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX|SEM_NOOPENFILEERRORBOX);if(argc!=3)return 2;auto apps=std::filesystem::path(argv[1]).parent_path().parent_path().parent_path().parent_path().parent_path();for(auto name:{"HSlog.dll","ModuleFrame.dll"}){auto p=apps/"ModuleProxy"/"x64"/name;LoadLibraryExA(p.string().c_str(),nullptr,LOAD_WITH_ALTERED_SEARCH_PATH);}auto module=LoadLibraryExA(argv[1],nullptr,LOAD_WITH_ALTERED_SEARCH_PATH);if(!module)return 3;try{install(module);
 std::string binary=std::string(argv[2])+"/matrix.iwcal";float h[9]={2,0,10,0,2,-5,0,0,1};FILE*f=nullptr;fopen_s(&f,binary.c_str(),"wb");fwrite(h,4,9,f);fclose(f);
 for(int mode=0;mode<2;mode++){Instance m(module);inputs.clear();m.run("no-file");m.param("FilePath",binary.c_str(),1);m.run("nul-path-rejected");m.param("FilePath",binary.c_str());m.run("no-count");m.param("RevOutputFlag",mode?"True":"False");
 for(int i=0;i<8;i++){double a=i%3*17*3.141592653589793/180;inputs={{"ImagePointX",{(float)(30+20*cos(a)),999}},{"ImagePointY",{(float)(40+20*sin(a)),999}},{"WorldRotateAngle",{17}},{"RotNum",{3}}};m.run(("sample-"+std::to_string(i)).c_str());}
 inputs.erase("ImagePointY");m.run("missing-y");inputs["ImagePointY"]={40};m.run("restart-after-missing");inputs["RotNum"]={2};m.run("invalid-two");inputs["RotNum"]={16};m.run("change-count");inputs["RefreshSignal"]={1};m.run("refresh-binary");m.param("FilePath","not-found.iwcal");inputs.erase("RefreshSignal");m.run("after-load-fail");}

 {Instance m(module);inputs.clear();int serial=0;auto fileCase=[&](const char* name,const char* text){std::string path=std::string(argv[2])+"/"+name;FILE*f=nullptr;fopen_s(&f,path.c_str(),"wb");fwrite(text,1,strlen(text),f);fclose(f);int code=m.set(m.bytes.data(),"FilePath",path.c_str(),(int)path.size());printf("{\"phase\":\"file-case\",\"name\":\"%s\",\"code\":%u,\"loaded\":%s,\"matrix\":%s}\n",name,(unsigned)code,m.bytes[0x15e44]?"true":"false",array((float*)(m.bytes.data()+0x15e20),9).c_str());fflush(stdout);};
 fileCase("valid.xml","<CalibInfo><CalibOutputParam><Anything DataType=\"FloatList\" ParamName=\"not-a-matrix\"><V>1</V><V>0</V><V>10</V><V>0</V><V>1</V><V>-5</V><V>0</V><V>0</V><V>1</V></Anything></CalibOutputParam></CalibInfo>");
 fileCase("short.xml","<Root><CalibOutputParam><X DataType=\"FloatList\"><A>3.5 trailing</A><A>bad</A></X></CalibOutputParam></Root>");
 fileCase("empty-list.xml","<Root><CalibOutputParam><X DataType=\"FloatList\" /></CalibOutputParam></Root>");
 fileCase("no-list.xml","<Root><CalibOutputParam><X DataType=\"float\" ParamName=\"example\"><ParamValue>2</ParamValue></X></CalibOutputParam></Root>");
 fileCase("no-output.xml","<Root />");fileCase("malformed.xml","<Root>");fileCase("upper.XML","<Root />");fileCase("invalid.iwcal","1234");
 }
 restore();return 0;}catch(int x){fprintf(stderr,"exception %d",x);restore();return 4;}catch(...){restore();return 5;}}



