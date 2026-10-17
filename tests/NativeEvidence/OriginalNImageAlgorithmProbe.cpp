// Owned process, documented original algorithm SDK only. No devices.
#include <windows.h>
#include <cstdio>
#include <vector>
#include "MVDCore.h"
#include "NImageCalibV2Cpp.h"
using namespace VisionDesigner;
using namespace VisionDesigner::NImageCalibV2;
int main(int argc,char**argv){
 if(argc<2)return 2;setvbuf(stdout,nullptr,_IONBF,0);SetDllDirectoryA(argv[1]);
 HMODULE lib=LoadLibraryA("MVDNImageCalibV2Cpp.dll"),images=LoadLibraryA("MVDImageCpp.dll");
 if(!lib||!images){printf("{\"phase\":\"load\",\"code\":%lu}\n",GetLastError());return 3;}
 auto create=(int(__stdcall*)(INImageCalibV2Tool**))GetProcAddress(lib,"CreateNImageCalibV2ToolInstance");
 auto destroy=(int(__stdcall*)(INImageCalibV2Tool*))GetProcAddress(lib,"DestroyNImageCalibV2ToolInstance");
 auto imageCreate=(int(__stdcall*)(IMvdImage**))GetProcAddress(images,"CreateImageInstance");
 auto imageDestroy=(int(__stdcall*)(IMvdImage*))GetProcAddress(images,"DestroyImageInstance");
 INImageCalibV2Tool*tool=nullptr;int code=create(&tool);printf("{\"phase\":\"create\",\"code\":%u}\n",(unsigned)code);if(code)return 4;
 try{unsigned n=0;try{tool->SaveConfiguration(nullptr,0,&n);}catch(...){}std::vector<unsigned char>xml(n+1);tool->SaveConfiguration(xml.data(),n,&n);printf("{\"phase\":\"configuration\",\"length\":%u}\n",n);FILE*f=fopen("original-config.xml","wb");if(f){fwrite(xml.data(),1,n,f);fclose(f);}}catch(IMVDException &ex){unsigned int e=ex.GetErrorCode();printf("{\"phase\":\"config-error\",\"code\":%u}\n",(unsigned)e);}
 for(int board: {7,6,8,9,1,2})for(int count:{0,1,2,3,9,12}){
  if(argc>3&&(board!=7||count!=9))continue;
  auto basic=tool->GetBasicParam();basic->ClearCalibImageData();basic->SetCalibBoardType((MVD_CALIBBOARD_TYPE)board);std::vector<IMvdImage*> owned;
  try{for(int i=0;i<count;i++){IMvdImage*im=nullptr;imageCreate(&im);owned.push_back(im);if(argc>2){char path[1024];sprintf_s(path,"%s-%d.bmp",argv[2],i%9);im->InitImage(path,MVD_PIXEL_MONO_08);}else im->InitImage(256,256,MVD_PIXEL_MONO_08);basic->AddCalibImageData(im,nullptr,nullptr,i<9?MVD_CALIBBOARD_OFFSET_IMAGE:MVD_CALIBBOARD_ROTATE_IMAGE,{(float)(i%3),(float)(i/3)},(float)(i-9));}
   tool->Run();auto result=tool->GetResult();float h[9];result->GetHomoMatrix(h);printf("{\"phase\":\"run\",\"board\":%d,\"count\":%d,\"code\":0,\"status\":%u,\"pairs\":%d,\"matrix\":[",board,count,(unsigned)result->GetStatus(),result->GetCornerPairsNum());for(int j=0;j<9;j++)printf("%s%.9g",j?",":"",h[j]);printf("],\"transError\":%.9g,\"corners\":[",result->GetCalibErr()->GetTransEstErr()->GetErrAvg());for(int j=0;j<result->GetCornerPairsNum();j++){auto pair=result->GetCornerPairs(j);auto world=pair->GetCornersWorld();printf("%s{\"world\":[%d,%d],\"pixels\":[",j?",":"",world.nX,world.nY);for(int k=0;k<pair->GetCornersNum();k++){auto pt=pair->GetCorners(k);printf("%s[%.9g,%.9g]",k?",":"",pt.fX,pt.fY);}printf("]}");}printf("]}\n");
  }catch(IMVDException &ex){unsigned int e=ex.GetErrorCode();printf("{\"phase\":\"run\",\"board\":%d,\"count\":%d,\"code\":%u}\n",board,count,(unsigned)e);}catch(...){printf("{\"phase\":\"unknown\",\"board\":%d,\"count\":%d}\n",board,count);}
  basic->ClearCalibImageData();for(auto im:owned)imageDestroy(im);
 }
 destroy(tool);return 0;
}
