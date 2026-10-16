# VisionIO

Independent .NET 10 image input; no vendor DLL or third-party dependency.

```csharp
using var input = File.OpenRead(path);
VisionCore.GrayImage image = VisionIO.BmpReader.Read(input);
```

Supported input: BMP with Windows DIB header (at least 40 bytes), BI_RGB, 8-bit indexed palette or 24/32-bit BGR. Rows are converted to top-left-origin gray pixels, handling bottom-up/top-down and 4-byte padding. Gray is rounded `(299*R + 587*G + 114*B)/1000`; 32-bit alpha is ignored.

The caller retains ownership of the readable stream; seeking is unnecessary. Malformed/truncated/unsupported input throws `InvalidDataException`. Limit: 64 million pixels and 1 MiB metadata/header. No RLE, bitfields, OS/2 DIB, JPEG, PNG or vendor-equivalence claim. No color-profile correction.

Verify with `dotnet run --project tests/VisionIO.Checks/VisionIO.Checks.csproj` (14 semantic checks).
