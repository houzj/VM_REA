// Controlled original SDK process. No camera, platform injection or disk DLL patching.
#include <windows.h>
#include <cstdio>
#include "MVDCore.h"
#include "MatrixFixCpp.h"
using namespace VisionDesigner;
using namespace VisionDesigner::MatrixFix;
int main(int argc,char** argv){
 if(argc!=2)return 2;SetDllDirectoryA(argv[1]);auto lib=LoadLibraryA("MVDMatrixFixCpp.dll");if(!lib)return 3;
 auto create=(int(__stdcall*)(IMatrixFixTool**))GetProcAddress(lib,"CreateMatrixFixToolInstance");auto destroy=(int(__stdcall*)(IMatrixFixTool*))GetProcAddress(lib,"DestroyMatrixFixToolInstance");IMatrixFixTool* tool=nullptr;int code=create(&tool);if(code||!tool)return 4;
 float cases[5][9]={{1,0,0,0,1,0,0,0,1},{2,3,4,5,6,7,0,0,1},{2,3,4,5,6,7,.01f,.02f,1},{2,3,4,5,6,7,0,0,2},{0,0,0,0,0,0,0,0,0}};
 for(int i=0;i<5;i++){auto basic=tool->GetBasicParam();basic->SetHomoMatrix(cases[i]);basic->SetImagePoint({10,20});basic->SetWorldPoint({100,200});try{tool->Run();float h[9];tool->GetResult()->GetHomoFixMatrix(h);printf("{\"case\":%d,\"code\":0,\"input\":[",i);for(int j=0;j<9;j++)printf("%s%.9g",j?",":"",cases[i][j]);printf("],\"image\":[10,20],\"world\":[100,200],\"matrix\":[");for(int j=0;j<9;j++)printf("%s%.9g",j?",":"",h[j]);printf("]}\n");}catch(IMVDException& error){printf("{\"case\":%d,\"code\":%u}\n",i,error.GetErrorCode());}}
 destroy(tool);return 0;
}
