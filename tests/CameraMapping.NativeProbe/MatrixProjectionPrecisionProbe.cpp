#include <windows.h>
#include <cstdio>
#include <cstdlib>
#include "MatrixToParamCpp.h"
#include "MVDCore.h"

using namespace VisionDesigner;
using namespace VisionDesigner::MatrixToParam;

int main(int argc, char** argv) {
    SetErrorMode(SEM_FAILCRITICALERRORS | SEM_NOGPFAULTERRORBOX | SEM_NOOPENFILEERRORBOX);
    setvbuf(stdout, nullptr, _IONBF, 0);
    if (argc != 11) return 2;
    HMODULE dll = LoadLibraryA(argv[1]);
    if (!dll) { printf("{\"event\":\"load-failed\",\"win32\":%lu}\n", GetLastError()); return 3; }
    auto create = reinterpret_cast<int(__stdcall*)(IMatrixToParamTool**)>(GetProcAddress(dll, "CreateMatrixToParamToolInstance"));
    if (!create) { printf("{\"event\":\"export-missing\"}\n"); return 4; }
    IMatrixToParamTool* tool = nullptr;
    const int createCode = create(&tool);
    if (createCode || !tool) { printf("{\"event\":\"create-failed\",\"code\":%d}\n", createCode); return 5; }
    float matrix[9];
    for (int i = 0; i < 9; ++i) matrix[i] = static_cast<float>(std::strtod(argv[i + 2], nullptr));
    try {
        tool->ImportHomoMatrix(matrix);
        tool->Run();
        auto* result = tool->GetResult();
        if (!result) { printf("{\"event\":\"result-null\"}\n"); return 6; }
        printf("{\"event\":\"result\",\"scale\":%.17g,\"offsetX\":%.17g,\"offsetY\":%.17g,\"rotate\":%.17g,\"aspect\":%.17g,\"skew\":%.17g}\n",
               result->GetScale(), result->GetOffsetX(), result->GetOffsetY(), result->GetRotate(), result->GetAspect(), result->GetSkew());
    } catch (const IMVDException& e) {
        printf("{\"event\":\"exception\",\"code\":%u,\"module\":%d,\"description\":\"%s\"}\n",
               static_cast<unsigned>(e.GetErrorCode()), static_cast<int>(e.GetModuleType()), e.GetDescription());
    } catch (...) { printf("{\"event\":\"unknown-exception\"}\n"); }
    fflush(stdout);
    TerminateProcess(GetCurrentProcess(), 0);
    return 0;
}

