# Proposed single1390 trainer oracle: source precheck, not executed

Target core SHA85D17ECC87D52E9A62D8BB86C8D5208AC999BCD31EC6D0806453F8165733F549, direct `befbb0→befdc0`. Root requires this ABI/address/ownership review before execution. Proposed input is one16x16 tight gray image with byte `(13*x+19*y+7)%181`, mask256 bytes all1, requested level1, angle start0/end0/step2, version1390, fixpoint(7.5f,7.5f). It is an isolated generator fixture, not a final matching score sample.

## Exact ABI

Windows x64 first4 arguments: RCX=image pointer, EDX=image stride16, R8=mask pointer, R9D=mask stride16. `befbb0` prolog delta0xA8; following caller-entry stack argument offsets0x28..0x60 correspond to its current offsets0xD0..0x108. `befdc0` prolog delta0x4B8; corresponding offsets0x4E0..0x518.

|Arg|Type/payload|befbb0 current stack|befdc0 current stack|
|---|---|---|---|
|5|uint64 packed width16,height16|D0|4E0|
|6|uint64 packed float fixpoint bits|D8|4E8|
|7|int32 version1390|E0|4F0|
|8|pointer to16-byte `{int level; float start,end,step;}`|E8|4F8|
|9|caller-owned writable model buffer|F0|500|
|10|uint64* used length output|F8|508|
|11|void** model pointer output|100|510|
|12|caller-owned writable workspace|108|518|

Original `befbb0` calls trainer with its own zero-initialized temporary used length and writes that value to argument10 after return, preserving EAX. It does not clear argument11 on every failure. Success return0 and failure10100000 are confirmed by explicit decoded epilogs. Buffer/no-pointer errors10000014, dimensions1000001A, stride1000001B, level10000019, version10100001 and contrast10100000 remain earlier guards. Fixed proposed input passes these guards by construction; do not simulate other malformed calls.

## Fixed-case ranges and capacities

All pointer owners are the probe. Allocate separate aligned blocks with prefix/suffix guards, at least1MiB each for model and workspace, and256-byte logical image/mask plus128 owned slack. No pointers refer to module globals or another process. Record untouched guards and exact returned relative pointers. Initial sentinel bytes are probe input, not a claim about original SDK allocator initialization.

Image/mask source reads are within256 bytes for the tight16x16 fixture. `c97550` sees selected image contrast>1. `c96ce0(level1,16x16,1390)` returns384 bytes: header128+aligned256; image pyramid model-relative `[0x27700,0x27880)`, mask pyramid `[0x27880,0x27A00)`. Base metadata lies below0x27700; version writes at0x276B8, level quality arrays start0x276C0. One angle's reserved selected-byte payload is384 bytes atmodel+0x27A00; subsequent angle header starts0x27B80.

At angle0 the destination is16x16; mask lookup rounds source coordinates within0..15. Full mask yields one contiguous span per row,16 spans total,256 selected pixels. Expected record end/used size is0x27B80+0x40+16*0x18=0x27D40. This is a **source-derived expected fixed-case bound**, not an observed returned value. Exact model bytes/unknown header holes cannot be asserted before original execution. Even a conservative256 one-pixel-span upper bound ends below0x29400, well inside1MiB. No unseen angle table beyond one entry is created; no quarter-turn group variant is generated because c978c0 single-angle branch returnscount1/index0 directly.

`c97840(16x16)` gives align128(17²+17²)=640 bytes=A. Workspace reserves four such plane regions `[0,4*A)`. `c978a0(720,&size)` returnsalign128(720*20)=14464; angle group buffer starts4*A=2560. `c97890(720,&size)` returnsalign128(720)=768; visited starts4*A+14464=17024. Reserved workspace end is17792. **Visited does not start5*A**; pointer-order asm and output-pointer size helpers establish this. Fixed one-angle group writes20 bytes and visited branch does not touch its byte array. Remaining scratch accesses are bounded by their640-byte planes; full-case output image/mask are256 bytes each.

## Ownership and dependencies

No allocation/free operation occurs in these direct trainer bodies. They consume caller storage; result model pointer should alias argument9, and used length is an integer. Probe must not call a vendor destructor on returned pointer. Probe owns freeing its blocks, or exits its own process after flushing; it never releases another process or shared object. Its SetDllDirectory/LoadLibrary is the established isolated same-hash SDK resolution, without PlatformInit, licensing, camera or device APIs.

Concrete dependencies: c97550 contrast; c96ce0 capacity; c96ef0 dimensions; c97840/90/A0 workspace sizes; c97090 builds image/mask pyramids;6e7ca0 copy;62f260 selects the image pyramid CPU backend; d46a10/d472b0 or c96f20 samplers (actual original oracles already recorded); c96e60 extracts layer0; c9ace0/c9ad00 compute training statistics (original bit oracles recorded); c978c0 single-angle grouping; bf2610 small-template pixel/span record generator;6ebb90 scratch fill; imported cosf/sinf/ceilf and security cookie. Large>=64 inline path and old versions are not reached. bf2610 exact20-argument stack surface and original bounds are in the dedicated small-template source report.

Remaining review gates before execution: root verifies this fixed-case pointer ledger and source chain; probe then logs load/error/return/guards and complete bounded bytes in isolated output. Any unexpected return, size, pointer or guard stops without trying more inputs. An original allocation-policy guarantee outside this explicitly owned fixture is not inferred.
