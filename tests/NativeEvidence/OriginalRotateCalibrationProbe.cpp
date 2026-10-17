// Original exports invoked on our own normally-constructed object. No platform Init,
// license change, devices, original binary patching or foreign process interaction.
#include <windows.h>
#include <vector>
#include <cstdio>
#include <cmath>
#include <filesystem>
int main(int argc,char**argv){
 SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX|SEM_NOOPENFILEERRORBOX);
 if(argc!=2)return 2;
 LoadLibraryA("MSVCP120.dll");LoadLibraryA("MSVCR120.dll");
 auto apps=std::filesystem::path(argv[1]).parent_path().parent_path().parent_path().parent_path().parent_path();
 for(auto name:{"HSlog.dll","ModuleFrame.dll"}){auto path=apps/"ModuleProxy"/"x64"/name;auto h=LoadLibraryA(path.string().c_str());if(!h)fprintf(stderr,"preload %s %lu\n",path.string().c_str(),GetLastError());}
 auto dll=LoadLibraryA(argv[1]);if(!dll){fprintf(stderr,"load-error %lu %s\n",GetLastError(),argv[1]);return 3;}
 auto ctor=(void*(*)(void*))GetProcAddress(dll,"??0CAlgorithmModule@@QEAA@XZ");
 auto dtor=(void(*)(void*))GetProcAddress(dll,"??1CAlgorithmModule@@UEAA@XZ");
 using Solve=int(*)(void*,std::vector<float>&,std::vector<float>&,std::vector<float>&,int,float*,bool,double*,double*,double*,double*,double*,double*);
 auto solve=(Solve)GetProcAddress(dll,"?CalibRot@CAlgorithmModule@@AEAAHAEAV?$vector@MV?$allocator@M@std@@@std@@00HQEAM_NPEAN33333@Z");
 if(!ctor||!dtor||!solve)return 4;std::vector<unsigned char> memory(0x16000);void*self=memory.data();ctor(self);
 float matrices[][9]={{1,0,0,0,1,0,0,0,1},{2,.3f,10,.1f,3,-5,0,0,1},{-2,0,0,0,2,0,0,0,1},{0,0,0,0,0,0,0,0,1}};
 for(int m=0;m<4;m++)for(int reverse=0;reverse<2;reverse++)for(int count:{2,3,4,9,16})for(int noise=0;noise<3;noise++){
  std::vector<float>x,y,a;for(int i=0;i<count;i++){double angle=i*17.0*3.141592653589793/180.;x.push_back((float)(30+20*cos(angle)));y.push_back((float)(40+20*sin(angle)));a.push_back((float)(i*17));}
  if(noise==1&&count>3){x.back()+=5;y.back()-=3;}if(noise==2)for(int i=0;i<count;i++)a[i]=0;
  double values[6]={};int code=solve(self,x,y,a,count,matrices[m],reverse!=0,&values[0],&values[1],&values[2],&values[3],&values[4],&values[5]);
  printf("{\"matrix\":%d,\"reverse\":%s,\"count\":%d,\"noise\":%d,\"code\":%u,\"values\":[",m,reverse?"true":"false",count,noise,(unsigned)code);
  for(int i=0;i<6;i++){if(i)printf(",");if(std::isfinite(values[i]))printf("%.17g",values[i]);else printf("\"NaN\"");}
  printf("],\"points\":[");for(int i=0;i<count;i++)printf("%s[%.9g,%.9g,%.9g]",i?",":"",x[i],y[i],a[i]);printf("]}\n");
 }
 dtor(self);return 0;
}
