// Original SDK in an owned process; synthetic image fixtures; no devices.
#include <windows.h>
#include <cstdio>
#include <vector>
#include <cmath>
#include "MVDCore.h"
#include "NImageCalibV2Cpp.h"
using namespace VisionDesigner;using namespace VisionDesigner::NImageCalibV2;
void F(float x){if(std::isfinite(x))printf("%.9g",x);else printf("\"%s\"",std::isnan(x)?"NaN":x>0?"Infinity":"-Infinity");}
void B(INImageCalibV2BaseErrInfo*p){printf("[");F(p->GetErrAvg());printf(",");F(p->GetErrMax());printf(",%d,",p->GetErrMaxPtsNum());F(p->GetErrWorldAvg());printf(",");F(p->GetErrWorldMax());printf("]");}
void T(INImageCalibV2MechTransErrInfo*p){printf("[");F(p->GetTrans());printf(",");F(p->GetScale());printf(",");F(p->GetRotate());printf("]");}
int main(int argc,char**argv){
 SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);if(argc<3||argc>4)return 2;setvbuf(stdout,nullptr,_IONBF,0);SetDllDirectoryA(argv[1]);auto lib=LoadLibraryA("MVDNImageCalibV2Cpp.dll"),img=LoadLibraryA("MVDImageCpp.dll");if(!lib||!img)return 3;
 auto create=(int(__stdcall*)(INImageCalibV2Tool**))GetProcAddress(lib,"CreateNImageCalibV2ToolInstance");auto destroy=(int(__stdcall*)(INImageCalibV2Tool*))GetProcAddress(lib,"DestroyNImageCalibV2ToolInstance");auto imageCreate=(int(__stdcall*)(IMvdImage**))GetProcAddress(img,"CreateImageInstance");auto imageDestroy=(int(__stdcall*)(IMvdImage*))GetProcAddress(img,"DestroyImageInstance");
 struct Case{int trans,rot,start;};Case cases[]={{0,2,0},{0,3,0},{0,5,0},{0,2,1},{0,2,2},{0,2,3},{9,0,0},{9,2,0},{9,3,0},{9,5,0},{9,1,0},{0,1,0}};
 int index=0;for(auto c:cases){if(argc==4&&index++!=atoi(argv[3]))continue;INImageCalibV2Tool*tool=nullptr;int code=create(&tool);if(code)return 4;auto basic=tool->GetBasicParam();basic->SetCalibBoardType((MVD_CALIBBOARD_TYPE)7);std::vector<IMvdImage*> owned;
 try{for(int i=0;i<c.trans+c.rot;i++){IMvdImage*im=nullptr;imageCreate(&im);owned.push_back(im);char path[1024];if(i<c.trans)sprintf_s(path,"%s-%d.bmp",argv[2],i);else sprintf_s(path,"%s-r%d.bmp",argv[2],i-c.trans+c.start);im->InitImage(path,MVD_PIXEL_MONO_08);basic->AddCalibImageData(im,nullptr,nullptr,i<c.trans?MVD_CALIBBOARD_OFFSET_IMAGE:MVD_CALIBBOARD_ROTATE_IMAGE,{i<c.trans?(float)(i%3):0.f,i<c.trans?(float)(i/3):0.f},i<c.trans?0.f:(float)(i-c.trans+c.start)*10);}
 tool->Run();auto r=tool->GetResult();float h[9];r->GetHomoMatrix(h);auto center=r->GetRotImageCenter();printf("{\"trans\":%d,\"rot\":%d,\"start\":%d,\"code\":0,\"status\":%u,\"direction\":%d,\"matrix\":[",c.trans,c.rot,c.start,(unsigned)r->GetStatus(),r->GetRotDirectState());for(int j=0;j<9;j++){if(j)printf(",");F(h[j]);}printf("],\"center\":[");F(center.fX);printf(",");F(center.fY);auto e=r->GetCalibErr();printf("],\"transError\":");B(e->GetTransEstErr());printf(",\"rotError\":");B(e->GetRotEstErr());printf(",\"transMotionAverage\":");T(e->GetTransMotionErr()->GetTransMotionErrAvg());printf(",\"transMotionMaximum\":");T(e->GetTransMotionErr()->GetTransMotionErrMax());printf(",\"transMotionIndex\":%d,\"rotMotion\":[",e->GetTransMotionErr()->GetPicIndex());F(e->GetRotMotionErr()->GetRotMotionErrAvg());printf(",");F(e->GetRotMotionErr()->GetRotMotionErrMax());printf(",%d],\"vectors\":[",e->GetRotMotionErr()->GetPicIndex());auto x=r->GetDirection()->GetXVector(),y=r->GetDirection()->GetYVector();F(x.fX);printf(",");F(x.fY);printf(",");F(y.fX);printf(",");F(y.fY);printf(",");F(r->GetDirection()->GetMoveDirec());printf("],\"corners\":[");for(int j=0;j<r->GetCornerPairsNum();j++){auto pair=r->GetCornerPairs(j);auto world=pair->GetCornersWorld();printf("%s{\"world\":[%d,%d],\"pixels\":[",j?",":"",world.nX,world.nY);for(int k=0;k<pair->GetCornersNum();k++){auto p=pair->GetCorners(k);printf("%s[%.9g,%.9g]",k?",":"",p.fX,p.fY);}printf("]}");}printf("]}\n");
 }catch(IMVDException&ex){printf("{\"trans\":%d,\"rot\":%d,\"start\":%d,\"code\":%u}\n",c.trans,c.rot,c.start,ex.GetErrorCode());}
 basic->ClearCalibImageData();for(auto p:owned)imageDestroy(p);destroy(tool);
 }return 0;
}
