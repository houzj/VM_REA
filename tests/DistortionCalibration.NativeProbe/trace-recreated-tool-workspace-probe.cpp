// Destroy/recreate original SDK tools with one identical image kept alive.
// No devices and no VisionMaster platform initialization.
#include <windows.h>
#include <cstdio>
#include <vector>
#include <cmath>
#include "MVDCore.h"
#include "ImageCalibCpp.h"
using namespace VisionDesigner;
using namespace VisionDesigner::ImageCalib;
#include "trace-recreated-workspace-writer-owned.h"

int main(int argc, char** argv) {
    if (argc < 2 || argc > 3) return 2;
    SetErrorMode(SEM_FAILCRITICALERRORS | SEM_NOGPFAULTERRORBOX);
    setvbuf(stdout, nullptr, _IONBF, 0);
    SetDllDirectoryA(argv[1]);
    auto lib = LoadLibraryA("MVDImageCalibCpp.dll");
    auto img = LoadLibraryA("MVDImageCpp.dll");
    if (!lib || !img) return 3;
    auto create = (int(__stdcall*)(IImageCalibTool**))GetProcAddress(lib, "CreateImageCalibToolInstance");
    auto destroy = (int(__stdcall*)(IImageCalibTool*))GetProcAddress(lib, "DestroyImageCalibToolInstance");
    auto imageCreate = (int(__stdcall*)(IMvdImage**))GetProcAddress(img, "CreateImageInstance");
    auto imageDestroy = (int(__stdcall*)(IMvdImage*))GetProcAddress(img, "DestroyImageInstance");
    if (!create || !destroy || !imageCreate || !imageDestroy) return 4;
    IImageCalibTool* tool = nullptr;
    IMvdImage* image = nullptr;
    if (imageCreate(&image)) return 5;
    FILE* runs = fopen("runs.jsonl", "wb");
    if (!runs) return 6;
    int exitCode = 0;
    try {
        image->InitImage(480, 400, MVD_PIXEL_MONO_08);
        auto pixels = image->GetImageData(0);
        for (int instance = 0; instance < 24; instance++) {
        if (create(&tool)) return 7;
        const int board = argc == 3 && instance == 0 ? 1 : 2;
        for (int y = 0; y < 400; y++) for (int x = 0; x < 480; x++) {
            unsigned char value = 255;
            if (board == 1) {
                if(x>=60 && x<420 && y>=50 && y<350)
                    value = (((x-60)/30 + (y-50)/30) & 1) ? 255 : 0;
            } else for(int row=0;row<8;row++) for(int col=0;col<10;col++) {
                const double dx=x-(90+col*30),dy=y-(80+row*30);
                if(dx*dx+dy*dy<81)value=0;
            }
            pixels->pData[y*pixels->nRowStep+x]=value;
        }
        fprintf(runs, "{\"phase\":\"created\",\"instance\":%d,\"tool\":%llu}\n", instance, (unsigned long long)tool);
        tool->SetRunParam("CorrectModel", "1");
        tool->SetRunParam("HomoFreedom", "1");
        tool->SetRunParam("CalibBoardWeightFun", "2");
        tool->SetRunParam("Circularity", "50");
        tool->SetInputImage(image);
        tool->GetBasicParam()->SetCalibBoardType((MVD_CALIBBOARD_TYPE)board);
        tool->GetBasicParam()->SetPerspectiveCenterPoint({240, 200});
        tool->GetBasicParam()->SetErrorEvaluationStatus(true);
        InstallFitTrace();
        g_board = board; g_model = 1; g_warp = instance;
        for (int repeat = 0; repeat < 2; repeat++) {
            tool->Run();
            auto result = tool->GetResult();
            auto locate = result->GetLocateInfo();
            auto estimate = result->GetHomoEstInfo();
            float matrix[19];
            estimate->GetWarpMatrix(matrix);
            fprintf(runs, "{\"instance\":%d,\"repeat\":%d,\"count\":%d,\"error\":%.9g,\"matrix\":[", instance, repeat,
                locate->GetCalibPointNum(), estimate->GetEstimationError());
            for (int i = 0; i < 19; i++) fprintf(runs, "%s%.9g", i ? "," : "", matrix[i]);
            fprintf(runs, "]}\n");
            fflush(runs);
        }
        fprintf(runs, "{\"phase\":\"destroy-begin\",\"instance\":%d}\n", instance); fflush(runs);
        destroy(tool); tool = nullptr;
        fprintf(runs, "{\"phase\":\"destroy-end\",\"instance\":%d}\n", instance); fflush(runs);
        }
    } catch (IMVDException& e) {
        exitCode = 8;
        fprintf(runs, "{\"code\":%u}\n", e.GetErrorCode());
    } catch (...) {
        exitCode = 9;
        fprintf(runs, "{\"unknown\":true}\n");
    }
    fclose(runs);
    if(tool) destroy(tool);
    imageDestroy(image);
    return exitCode;
}





