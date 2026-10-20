#define main ArchivedStateProbeMain
#include "../NativeEvidence/OriginalRotateCalibrationStateProbe.cpp"
#undef main
static std::string controlled;
static int controlledCode=0;
static int controlledInt(void* ctx,const char* name,int index,int* value,int* count){if(controlled==name){int before=*value;*count=0;printf("{\"phase\":\"int-slot\",\"name\":\"%s\",\"sourcePoison\":%.9g,\"before\":%d,\"after\":%d,\"code\":%u}\n",name,inputs[name][0],before,*value,(unsigned)controlledCode);getter("GetInt",name,(unsigned)controlledCode,0);return controlledCode;}return getInt(ctx,name,index,value,count);}
static int controlledFloat(void* ctx,const char* name,int index,float* value,int* count){if(controlled==name){*count=0;getter("GetFloat",name,(unsigned)controlledCode,0);return controlledCode;}return getFloat(ctx,name,index,value,count);}
int main(int argc,char**argv){if(argc!=3)return 2;SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);setvbuf(stdout,nullptr,_IONBF,0);for(auto name:{"HSlog.dll","ModuleFrame.dll"}){auto p=std::string("D:/Program Files/VisionMaster4.4.0/Applications/ModuleProxy/x64/")+name;LoadLibraryExA(p.c_str(),nullptr,LOAD_WITH_ALTERED_SEARCH_PATH);}auto module=LoadLibraryExA(argv[1],nullptr,LOAD_WITH_ALTERED_SEARCH_PATH);if(!module)return 3;
 try{install(module);for(auto& p:patched){if(p.name=="VM_M_GetInt")put(p.slot,(void*)controlledInt);if(p.name=="VM_M_GetFloat")put(p.slot,(void*)controlledFloat);}
 std::string path=std::string(argv[2])+"/matrix.iwcal";FILE*f=nullptr;fopen_s(&f,path.c_str(),"wb");float matrix[9]={2,0,10,0,2,-5,0,0,1};fwrite(matrix,4,9,f);fclose(f);
 for(int test=0;test<14;test++){fopen_s(&f,path.c_str(),"wb");fwrite(matrix,4,9,f);fclose(f);Instance m(module);controlled="";controlledCode=0;inputs={{"RefreshSignal",{0,1}},{"RotNum",{3,16}},{"ImagePointX",{50,99}},{"ImagePointY",{40,99}},{"WorldRotateAngle",{17,99}}};if(test!=11)m.param("FilePath",path.c_str());
  if(test>=1&&test<=3){controlled=test==1?"ImagePointX":test==2?"ImagePointY":"WorldRotateAngle";controlledCode=0xe0000123;}
  if(test>=4&&test<=6){controlled=test==4?"ImagePointX":test==5?"ImagePointY":"WorldRotateAngle";controlledCode=0;}
  if(test==7){controlled="RotNum";controlledCode=0xe0000123;}
  if(test==8){controlled="RotNum";controlledCode=0;}
  if(test==9){controlled="RefreshSignal";controlledCode=0xe0000123;}
  if(test==10){inputs["RotNum"]={2,3};}
  if(test==12||test==13){inputs["RefreshSignal"]={1,0};controlled="RefreshSignal";controlledCode=test==12?0:0xe0000123;DeleteFileA(path.c_str());}
  m.run(("typed-"+std::to_string(test)).c_str());
 }
 restore();return 0;}catch(...){restore();return 5;}}
