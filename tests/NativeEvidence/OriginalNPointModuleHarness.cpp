// Controlled replay of original exported methods in an owned native instance.
// The host context is synthetic: bind a normally-created official algorithm object
// to the source-confirmed member at +0x15d98. Do not call or claim platform Init.
// No original code, license checks, DLL files or other processes are modified.
#include <windows.h>
#include <vector>
#include <cstdio>
#include <string>
#include "NPointCalibCpp.h"
using namespace VisionDesigner::NPointCalib;
int main(int argc,char** argv){
 if(argc!=4)return 2;
 HMODULE a=LoadLibraryA(argv[1]),m=LoadLibraryA(argv[2]);if(!a||!m){printf("{\"phase\":\"load\",\"code\":%lu}\n",GetLastError());return 3;}
 auto create=(int(__stdcall*)(INPointCalibTool**))GetProcAddress(a,"CreateNPointCalibToolInstance");
 auto ctor=(void*(*)(void*))GetProcAddress(m,"??0CIMVSNPointCalibModu@@QEAA@XZ");
 auto dtor=(void(*)(void*))GetProcAddress(m,"??1CIMVSNPointCalibModu@@UEAA@XZ");
 auto set=(int(*)(void*,const char*,const char*,int))GetProcAddress(m,"?SetParam@CIMVSNPointCalibModu@@UEAAHPEBD0H@Z");
 auto get=(int(*)(void*,const char*,char*,int,int*))GetProcAddress(m,"?GetParam@CIMVSNPointCalibModu@@UEAAHPEBDPEADHPEAH@Z");
 auto readInput=(int(*)(void*,void*,void*))GetProcAddress(m,"?GetProcessInput@CIMVSNPointCalibModu@@AEAAHQEBXPEAU_ALIGN_CALIB_MODU_INPUT@1@@Z");
 auto run=(int(*)(void*,void*,void*))GetProcAddress(m,"?ProcessAlg@CIMVSNPointCalibModu@@AEAAHPEAU_ALIGN_CALIB_MODU_INPUT@1@PEAUBASIC_MODU_OUTPUT@1@@Z");
 std::vector<unsigned char> instance(0x16608);void* self=instance.data();ctor(self);
 INPointCalibTool* tool=nullptr;int code=create(&tool);printf("{\"phase\":\"create-algorithm\",\"code\":%u}\n",(unsigned)code);if(code){dtor(self);return 4;}
 *(INPointCalibTool**)(instance.data()+0x15d98)=tool;
 try{
 auto read=[&](const char* key){char value[4096]={};int size=0;int c=get(self,key,value,4096,&size);printf("{\"phase\":\"get\",\"key\":\"%s\",\"code\":%u,\"value\":\"%s\"}\n",key,(unsigned)c,value);};
 for(auto key:{"CalibPointGet","CalibPointTotalNum","RotPointTotalNum","CalibOrigin","TeachEnable","BasePointX","BasePointY","MoveAlignX","MoveAlignY","MoveFirstType","ChangeDirectionMoveTime","BaseAngle","MoveAngle","UseRelativeCoordinates","CameraMode","HomoFreedom","WeightFun","WeightFactor","DistThreshold","SampleRatio"})read(key);
 for(auto key:{"CalibPointTotalNum","RotPointTotalNum","CalibOrigin","ChangeDirectionMoveTime"})for(int n:{-1,0,1,3,4,9,15,16,17,32}){std::string text=std::to_string(n);int c=set(self,key,text.c_str(),(int)text.size()+1);printf("{\"phase\":\"set\",\"key\":\"%s\",\"requested\":%d,\"code\":%u}\n",key,n,(unsigned)c);read(key);}
 set(self,"CalibPointGet","1",2);set(self,"CalibPointTotalNum","9",2);set(self,"RotPointTotalNum","0",2);set(self,"HomoFreedom","2",2);set(self,"WeightFun","1",2);
 for(int i=0;i<9;i++){float x=(i%3)*10.f,y=(i/3)*10.f;for(int j=0;j<4;j++){std::string key=std::string(j==0?"ImagePointX":j==1?"ImagePointY":j==2?"WorldPointX":"WorldPointY")+std::to_string(i);std::string value=std::to_string(j==0?x:j==1?y:j==2?2*x+10:2*y-5);set(self,key.c_str(),value.c_str(),(int)value.size()+1);}}
 int input[3]={1,9,0};int result[8]={};readInput(self,(void*)1,input);code=run(self,input,result);printf("{\"phase\":\"run-original-process-alg\",\"code\":%u,\"status\":%d,\"nativeError\":%u}\n",(unsigned)code,result[0],(unsigned)result[1]);
 code=set(self,"SaveCalibPath",argv[3],(int)strlen(argv[3])+1);printf("{\"phase\":\"save\",\"code\":%u}\n",(unsigned)code);
 }catch(int e){printf("{\"phase\":\"exception\",\"code\":%u}\n",(unsigned)e);}catch(...){printf("{\"phase\":\"unknown-exception\"}\n");}
 dtor(self);return 0;
}
