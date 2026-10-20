#include <windows.h>
#include <intrin.h>
#include <vector>
#include <cstdio>
#include <cstdint>
#include <cstring>

// Exact x64 ABI from ca0310/c9fa70/c9ff70 C + asm stack arguments.
// Pure arithmetic leaves, no algorithm handle/platform/device initialization.
using Dot = void(__cdecl*)(const unsigned char*, const unsigned char*, const int*, const int*, const int*, int, double*);
static unsigned long long bits(double v) { unsigned long long b; std::memcpy(&b, &v, 8); return b; }
static DWORD WINAPI deadline(void* ready) {
    SetEvent(static_cast<HANDLE>(ready)); Sleep(20000);
    std::fprintf(stderr, "phase:timeout20s\n"); std::fflush(stderr);
    TerminateProcess(GetCurrentProcess(), 124); return 124;
}
int main() {
    SetErrorMode(SEM_FAILCRITICALERRORS | SEM_NOGPFAULTERRORBOX | SEM_NOOPENFILEERRORBOX);
    HANDLE ready = CreateEventW(nullptr, TRUE, FALSE, nullptr);
    HANDLE timer = CreateThread(nullptr, 0, deadline, ready, 0, nullptr);
    if (!timer || WaitForSingleObject(ready, 1000) != WAIT_OBJECT_0) return 10;
    int cpu[4]{}; __cpuidex(cpu, 1, 0);
    bool avx = (cpu[2] & (1 << 27)) && (cpu[2] & (1 << 28));
    if (avx) avx = (_xgetbv(0) & 6) == 6;
    __cpuidex(cpu, 7, 0); avx = avx && (cpu[1] & (1 << 5));
    const wchar_t* dll = L"D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll";
    // Private probe process only; also covers SDK DLL internal LoadLibrary calls.
    SetDllDirectoryW(L"D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64");
    std::fprintf(stderr, "phase:before-load\n"); std::fflush(stderr);
    HMODULE h = LoadLibraryW(dll);
    if (!h) { std::fprintf(stderr, "LoadLibrary error %lu\n", GetLastError()); return 2; }
    std::fprintf(stderr, "phase:loaded\n"); std::fflush(stderr);
    auto scalar = reinterpret_cast<Dot>(reinterpret_cast<unsigned char*>(h) + 0xca0310);
    auto sse = reinterpret_cast<Dot>(reinterpret_cast<unsigned char*>(h) + 0xc9fa70);
    auto vector = reinterpret_cast<Dot>(reinterpret_cast<unsigned char*>(h) + 0xc9ff70);
    std::printf("{\"targetExecuted\":\"pure arithmetic leaves only\",\"avx2OsAvailable\":%s,\"cases\":[", avx ? "true" : "false");
    bool first = true;
    for (int width : {0,1,7,8,31,32,33,33025,33026,33027}) {
        for (int rows : {1,3}) {
            std::vector<unsigned char> a(static_cast<size_t>(width) + 128, 255), b(a.size(), 255);
            std::vector<int> ao(rows, 0), bo(rows, 0), lengths(rows, width);
            double x = -1234.5, y = -1234.5, z = -1234.5;
            std::fprintf(stderr, "phase:scalar-%d-%d\n", width, rows); std::fflush(stderr);
            scalar(a.data(), b.data(), ao.data(), bo.data(), lengths.data(), rows, &x);
            std::fprintf(stderr, "phase:sse-%d-%d\n", width, rows); std::fflush(stderr);
            sse(a.data(), b.data(), ao.data(), bo.data(), lengths.data(), rows, &y);
            std::fprintf(stderr, "phase:avx-%d-%d\n", width, rows); std::fflush(stderr);
            if (avx) vector(a.data(), b.data(), ao.data(), bo.data(), lengths.data(), rows, &z);
            std::printf("%s{\"width\":%d,\"rows\":%d,\"fill\":255,\"scalarBits\":\"%016llx\",\"sseBits\":\"%016llx\",\"avxBits\":\"%016llx\"}",first?"":",",width,rows,bits(x),bits(y),bits(z)); std::fflush(stdout); first=false;
        }
    }
    std::printf("]}\n"); std::fflush(stdout);
    std::fprintf(stderr, "phase:all20-arithmetic-cases-complete\n"); std::fflush(stderr);
    // Same owned-process boundary as existing native CodewordFailureProbe.
    // No algorithm handles were created; do not exercise unrelated DLL-unload lifecycle.
    TerminateProcess(GetCurrentProcess(), 0); return 11;
}
