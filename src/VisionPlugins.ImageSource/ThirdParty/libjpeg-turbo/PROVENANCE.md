# Independent JPEG backend

This plugin uses libjpeg-turbo 2.1.3, independently built from the official
[2.1.3 source](https://github.com/libjpeg-turbo/libjpeg-turbo/tree/2.1.3).
It does not redistribute the reverse-engineering target's media DLL.
This software is based in part on the work of the Independent JPEG Group.
Copyright and license terms are in LICENSE.md and README.ijg in this directory.

Archive: https://github.com/libjpeg-turbo/libjpeg-turbo/archive/refs/tags/2.1.3.zip

Archive SHA256: cc868c30e5ed5ae08a103ed726e7cd235d8d7208124cea1a4740bd8b94482eb9

Rebuild from the repository root with PowerShell 7, Visual Studio C++ and its
bundled CMake/Ninja installed:

```powershell
# Save the official archive at analysis/downloads/jpeg/libjpeg-turbo-2.1.3.zip.
./tests/NativeEvidence/Build-JpegBackend.ps1
```

The script validates the archive, extracts the unmodified source, then builds
Windows x64 Release TurboJPEG with WITH_SIMD=OFF and WITH_CRT_DLL=OFF. The DLL
is copied to runtimes/win-x64/native/turbojpeg.dll and included in plugin build
and publish outputs, together with this directory. Scalar performance differs
from the target's SIMD implementation. Other architectures are unsupported.
Binary identity is recorded in docs/plugins/image-source-jpeg-evidence.json;
compiler changes, paths and PE timestamps can change that hash on rebuilding.

For the owned fixture generator, build the cjpeg target in the generated build
directory under the same vcvars64 environment, then run
tests/NativeEvidence/Create-JpegCorpus.py --encoder PATH/TO/cjpeg.exe.
The checked-in fixtures and native expected pixels let ordinary checks run
without Visual Studio C++, source downloads, or the original application.

Scope: default RGB decoding, an observed integer MONO8 conversion and the
documented corpus. Small-color native conversion failures, CMYK/YCCK, ICC,
additional malformed streams and general native equivalence remain unverified.
