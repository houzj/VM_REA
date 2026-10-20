#include <windows.h>
#include <cstdio>
#include <cstdlib>
#include <cstring>
#include "CameraMapCpp.h"
#include "MVDCore.h"

using namespace VisionDesigner;
using namespace VisionDesigner::CameraMap;

static void printPoint(const char* op, int index, int repeat, MVD_POINT_F point) {
    printf("{\"event\":\"%s\",\"index\":%d,\"repeat\":%d,\"x\":%.9g,\"y\":%.9g}\n",
           op, index, repeat, point.fX, point.fY);
}

static void printError(const char* op, int index, int repeat, unsigned code) {
    printf("{\"event\":\"%s-exception\",\"index\":%d,\"repeat\":%d,\"code\":%u}\n",
           op, index, repeat, code);
}

static void snapshotCount(ICameraMapBasicParam* basic, int repeat) {
    const unsigned count = basic->GetAlignPairPointNum();
    printf("{\"event\":\"count\",\"repeat\":%d,\"value\":%u}\n", repeat, count);
}

static void snapshot(ICameraMapBasicParam* basic, int repeat, int base) {
    snapshotCount(basic, repeat);
    if (base != 0 && base != 1) {
        printf("{\"event\":\"snapshot-skipped\",\"reason\":\"getter index base unresolved\"}\n");
        return;
    }
    const unsigned count = basic->GetAlignPairPointNum();
    const int first = base;
    const int last = base == 0 ? static_cast<int>(count) - 1 : static_cast<int>(count);
    for (int index = first; index <= last; ++index) {
        try { printPoint("snapshot-object", index, repeat, basic->GetObjectPoint(index)); }
        catch (const IMVDException& e) { printError("snapshot-object", index, repeat, e.GetErrorCode()); }
        catch (...) { printError("snapshot-object-unknown", index, repeat, 0); }
        try { printPoint("snapshot-target", index, repeat, basic->GetTargetPoint(index)); }
        catch (const IMVDException& e) { printError("snapshot-target", index, repeat, e.GetErrorCode()); }
        catch (...) { printError("snapshot-target-unknown", index, repeat, 0); }
    }
}

int main(int argc, char** argv) {
    SetErrorMode(SEM_FAILCRITICALERRORS | SEM_NOGPFAULTERRORBOX | SEM_NOOPENFILEERRORBOX);
    setvbuf(stdout, nullptr, _IONBF, 0);
    if (argc != 5) return 2;
    const char* dllPath = argv[1];
    const char* operation = argv[2];
    const int index = std::atoi(argv[3]);
    const int getterBase = std::atoi(argv[4]);

    HMODULE dll = LoadLibraryA(dllPath);
    if (!dll) { printf("{\"event\":\"load-failed\",\"win32\":%lu}\n", GetLastError()); return 3; }
    auto create = reinterpret_cast<int(__stdcall*)(ICameraMapTool**)>(GetProcAddress(dll, "CreateCameraMapToolInstance"));
    if (!create) { printf("{\"event\":\"export-missing\"}\n"); return 4; }
    auto destroy = reinterpret_cast<int(__stdcall*)(ICameraMapTool*)>(GetProcAddress(dll, "DestroyCameraMapToolInstance"));
    ICameraMapTool* tool = nullptr;
    const int createCode = create(&tool);
    printf("{\"event\":\"create\",\"code\":%d}\n", createCode);
    if (createCode || !tool) return 5;

    ICameraMapBasicParam* basic = tool->GetBasicParam();
    if (!basic) { printf("{\"event\":\"basic-null\"}\n"); return 6; }
    const MVD_POINT_F objects[] = {{10.f, 1.f}, {20.f, 2.f}, {30.f, 3.f}};
    const MVD_POINT_F targets[] = {{101.f, 201.f}, {102.f, 202.f}, {103.f, 203.f}};
    for (int i = 0; i < 3; ++i) basic->AddAlignPairPoint(objects[i], targets[i]);
    snapshotCount(basic, -1);

    if (std::strcmp(operation, "get-object") == 0 || std::strcmp(operation, "get-target") == 0) {
        for (int repeat = 0; repeat < 2; ++repeat) {
            try {
                const auto point = std::strcmp(operation, "get-object") == 0
                    ? basic->GetObjectPoint(index) : basic->GetTargetPoint(index);
                printPoint(operation, index, repeat, point);
            } catch (const IMVDException& e) { printError(operation, index, repeat, e.GetErrorCode()); }
              catch (...) { printError(operation, index, repeat, 0); }
        }
        snapshotCount(basic, 2);
    } else if (std::strcmp(operation, "remove") == 0) {
        for (int repeat = 0; repeat < 2; ++repeat) {
            const unsigned before = basic->GetAlignPairPointNum();
            printf("{\"event\":\"remove-before\",\"index\":%d,\"repeat\":%d,\"count\":%u}\n", index, repeat, before);
            try {
                basic->RemoveAlignPairAt(index);
                printf("{\"event\":\"remove-return\",\"index\":%d,\"repeat\":%d}\n", index, repeat);
            } catch (const IMVDException& e) { printError("remove", index, repeat, e.GetErrorCode()); }
              catch (...) { printError("remove-unknown", index, repeat, 0); }
            snapshot(basic, repeat, getterBase);
        }
    } else {
        printf("{\"event\":\"unknown-operation\"}\n");
    }

    // The official SDK can hang during standalone teardown; the existing
    // controlled probe uses the same process-local termination boundary.
    fflush(stdout);
    TerminateProcess(GetCurrentProcess(), 0);
    return 0;
}
