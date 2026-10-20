# 映射标定 File / Image / Teach 状态生命周期审计

2026-10-19 源码修正：新取DeInit及有限asm确认18001fbc9尾JMP ResetMember（22d50），因此下文历史“ResetMember仅Init调用”的xref判断已被新原证据覆盖。正常ROI类型1消息生成全图矩形；真实空工作向量跳SDK setter会留下旧clone，DeInit销毁SDK工具才释放。详见 [MAP-CALIBRATION-D4-MODULE-ROI-STATE-SOURCE.md](MAP-CALIBRATION-D4-MODULE-ROI-STATE-SOURCE.md)。不以正常全屏等同空vector。

本审计检查原厂 Map 模块 `SetParam → Process → ProcessAlg → ProcessOutput/GetResult → ResetMember`，并对照已有原厂模块 DLL 回放。目标仅为确认模式切换、输入缓存、文件刷新和失败后的状态；不做设备测试，不修改实现或共享框架。

## 原厂源证据

| 路径 | 直接证据 | SHA-256 |
|---|---|---|
| `analysis/map-calibration/ghidra/module-analyzed/180023870.c` | `CIMVSMapCalibModu::SetParam`：CalibPath0/1 在 `ReadFileMatrix` 成功后才将矩阵传给 `GetTwoFileMapBasicParam`、清空并保存路径字符串；read 返回非零时直接返回。InputType 分支是反编译质量不够的 `sscanf_s` 片段，单靠该伪代码不能确定内部赋值细节。 | `AEAE7D07725EF741D10D72BAC2443F0A88420F89EFA7C081B6E858CCC2495811` |
| `analysis/map-calibration/ghidra/module/1800227d0.c` | `ReadFileMatrix`：成功打开/定位输入容器后会释放并重建对应 side 的缓存，再逐项填矩阵。短 XML 仍可能产生部分替换矩阵后报告 `0xE0000018`；错误路径不是统一的“保留旧矩阵”。 | `4ABF886B99912352F541CC5A2DF537CBA0C2164B2447178630864F388AC6CD94` |
| `analysis/map-calibration/ghidra/module-analyzed/180021690.c` | `Process` 入口清零本轮模块状态标记；InputType=1 走图像缓存/Teach，InputType=2 读取 `RefreshSignal`，刷新时依次用保存路径再次调用 `SetParam(CalibPath0/1)`；任一失败即提前返回，未到 `ProcessAlg`/`ProcessOutput`。无新图像则返回 `0xE0000314`；半帧时写 `ModuStatus=1, CalibStatus=0`；匹配示教触发时读物理点与旋转角。 | `112ABD26934DD4288BD4A99F0675196A895B671964255AE80C9290EE1005C028` |
| `analysis/map-calibration/ghidra/module-analyzed/180021c50.c` | `ProcessAlg` 按 mode 1/2 分别装配双图或双文件参数，调用算法运行，再查询 `GetResult`；结果指针是否非空写入基本输出状态。成功时按 Teach/RefreshFileEnable/输出路径条件调用 `WriteXmlData`。 | `643F1892E704C1039C234485163CB653E61D01CC7F67F8B88C9C3A7DEFAE3972` |
| `analysis/map-calibration/ghidra/module-analyzed/1800220b0.c` | `ProcessOutput` 每次常规输出先写 `ModuStatus`、`CalibStatus`；Image 模式分支再输出检测框、比率、误差、角点、状态和尺度。File 刷新在 `SetParam` 出错时不会进入此函数。 | `12F085ABAC1773CD4D3D93A7B158FDFE527317B3A0E0FFCDF9C177CE344DC9F4` |
| `analysis/map-calibration/ghidra/module-analyzed/180022d50.c` 与 `analysis/map-calibration/ghidra/module/180021340.c` | `ResetMember` 清路径、文件矩阵缓存和 Teach 标记，默认 InputType=1；最新有限汇编确认 `Init` CALL 与 `DeInit` 尾JMP均进入 `ResetMember`；未见 InputType 切换路径调用。图像待处理标志是否由该函数完全重置未确认。 | `EDBCF36400FEFEBEBAB78BA4CE8DF40943A92E49706C9FC802E58E352A2F2676`; `05ED00B36E029DA5D0941EC1E88A807C851F521EBC5D65DFCDDF4F51B05DD1EF` |

## 已有原厂模块回放

回放来自 `tests/MapCalibration.NativeProbe/ModuleRuntimeProbe.cpp` 与 `ModuleImageRuntimeProbe.cpp`，加载安装目录原厂 Map 模块及算法 SDK，模块 DLL 代码保持不变；`ModuleHost.hpp` 仅替换模块 IO/runtime imports。记录首行明确标注 `platformRuntime:false`。因此这些 trace 是原厂模块在受控 IO host 下的回放，不等同完整 VisionMaster 平台运行。

- File trace：`analysis/map-calibration/native/module-runtime-results.jsonl`，SHA-256 `F3B19E9265C4D79AC6936C267E6D2129468A44E6BAA2BDCB18EC01B65CC51A23`。初次有效文件运行输出状态 1/1、SDK `GetResult` 有值。将 A 文件缩成两值后 Refresh 返回 `0xE0000018`，本轮 `writes=[]`，SDK `GetResult` 仍有先前结果；下一次不刷新仍运行当前缓存并写 1/1，SDK 读回矩阵变成 `[8388608,0,0,0,0,0,0,0,0]`，不再是先前单位矩阵。该数值很可能受原生部分填充/未初始化缓存影响，不将其作为稳定输出规范；可以确认的是失败读取改变了后续算法所见状态。恢复有效文件刷新又得到有效结果。之后将两侧置零，算法运行返回 `0x10103002`，模块写状态 0/0 且 SDK `GetResult` 为空；后续恢复有效文件可再次得到结果。该序列直接证明“刷新失败”和“算法运行失败”对旧结果的影响不同。
- Image/Teach/切换 trace：`analysis/map-calibration/image-runtime-source-grid/image-results.jsonl`，SHA-256 `F56DFB34B752192B51B1612C281D5E4505E6FAA9BD63CCE2B60B6A611C0D5913`。11步序列中：无图像返回 `0xE0000314` 且无写出；单帧写状态 1/0；第二帧组成图像对后输出完整结果；匹配 Teach 触发写出完整结果并生成 `teach-process.xml`；触发不匹配且无帧返回 `0xE0000314`；切至 File 且无两侧矩阵返回 `0x80100006` 并写 0/0；切回 Image 后一帧仍为 1/0，第二帧才运行完整求解。该 harness 没有在图像单帧待处理期间切换模式，也没有逐步读取算法 SDK `GetResult`，所以不能验证这两种情况下 SDK 内部旧结果的可见性。

## 与当前 Map 实现的逐项对照

- `MapCalibrationFileSession.SetInputPath` 将 parser 返回的 `Replacement` 即使在失败时也写入矩阵缓存；仅成功时更新路径。这对应原厂 `ReadFileMatrix` 可以先替换缓冲区、而 `SetParam` 只有成功后才提交 SDK 文件参数与路径的顺序。现有 `MapCalibration.FileChecks` 已逐项比较 reader 错误码及替换矩阵，未发现新差异。
- `MapCalibrationFileSession.Run` 在进入算法运行前清理 `LastResult`；输入 Refresh 的 setter 失败则提前返回、不调用 `Run`，因此保留先前结果。这与 file trace 中失败刷新不写出、SDK `GetResult` 仍有旧结果，以及后续算法失败输出 0/0 且 `GetResult` 为空的路径一致。
- `MapCalibrationPlugin` 分别用节点级 file session 和 image session 保存状态。InputType 分派不删除另一个模式的 session，模式切换本身不重置状态；原厂 `ResetMember` 由 `Init` 及 `DeInit`（尾跳）进入；当前静态调用链未发现单纯模式切换时进入该清理链。已有 image trace 覆盖了 File→Image 切换的无矩阵失败和新帧等待，但没有覆盖待处理半帧跨模式留存；静态源码两边都未见切换时重置，行为方向一致。
- Image 单帧/Teach 触发的状态输出与既有 harness 对照一致。Image 完整算法失败后算法 `GetResult` 是否仍保留更早的成功结果没有在现存 Image trace 中逐步读取，当前证据不足以把壳层显示的旧结果与模块本轮 output writes 等同起来。

本轮没有证据支持修改 Map 专属实现。现有 file 缓存和错误阶段代码按原厂已证实顺序保留。不要把 `writes=[]` 等同于 VisionMaster 全平台 UI 必然清空/保留旧动态端口：probe 的 IO host 不模拟平台输出存储策略。

## 尚需原厂平台证据

新定向回放已覆盖同一原厂模块实例的半帧跨模式状态和 Image 算法失败后的 SDK `GetResult`。剩余未知仅在真实 VM 宿主：当 `Process` 因空帧早返且没有 `VM_M_Set*` 写出时，宿主动态输出端口/订阅历史值是保留、清除还是标记无效；以及算法失败时宿主是否按当轮 0/0 覆盖之前 UI 显示。最小证据是原厂平台中逐轮记录 `ProcessOutput` IO 字典提交前后及订阅/显示值，不可从当前 synthetic host 推断。

## 定向生命周期回放（2026-10-19）

新增 `tests/MapCalibration.NativeProbe/ModuleStateLifecycleProbe.cpp` 与 `RunStateLifecycleProbe.ps1`。新探针加载安装目录原厂 `IMVSMapCalibModu.dll`（SHA-256 `658D63E2C4E2378E7EB1776D27B966990C0D0E01E29CA57ED82FE9ACC740FDE8`）和原厂算法 SDK；仍只把 `ModuleFrame.dll` 导入重定向到 `ModuleHost.hpp` 的受控 IO host，记录 `platformRuntime:false`。运行输出 `analysis/map-calibration/state-lifecycle-asserted-20261019/results.jsonl` SHA-256 `3D7C3960123DCB7D79E1404DE72295CD2DF5740A195220EAD6AA3F81824B5B1F`。输入 `frame-0.bmp` SHA-256 `86E175CD993F9737380690A60F7BADA49935196691CA4509C93FF0066FADCC40`，`frame-4.bmp` SHA-256 `D1ED36D2CF24A57AB541191E1FDE147C826AB182F0D4E6560B5FC69A304E73AF`；失败输入由脚本生成全黑 640×480 24-bit BMP，SHA-256 `5093C8551B9EBA0DF64BB5972ADF150FFDAE270C5745C993B1C83798F5CD8D6D`。探针源码 SHA-256 `E5DCFFBB7A6A9EB63C0F78E79EAF9160AA381AEA25501491DA34AF4EA80767B5`；运行脚本 SHA-256 `52DA0E4E5E8C6C82417E97CDC7DA4B4D2F41C456B33C889E3282EBB8D4761020`。

回放使用单一模块实例，步骤与观测如下：

| 步骤 | 操作 | 原厂 `Process` 返回 | 模块 pending 标志 | 当轮写出的状态 | SDK `GetResult` |
|---|---|---:|---|---|---|
| 0 | Image：送入 source `frame-0` + target `frame-4` | `0x00000000` | `[0,0]` | `ModuStatus=1, CalibStatus=1` | 非空，单应矩阵平移约 `(10,10)` |
| 1 | Image：只送 source | `0x00000000` | `[1,0]` | `ModuStatus=1, CalibStatus=0` | 仍非空，仍是步骤0的结果 |
| 2 | 切到 File，不配置有效矩阵 | `0x80100006` | `[1,0]` | `ModuStatus=0, CalibStatus=0` | 仍非空，仍是步骤0结果 |
| 3 | 切回 Image，不送图 | `0xE0000314` | `[1,0]` | 无写出 | 仍非空，仍是步骤0结果 |
| 4 | 只送 target `frame-4` | `0x00000000` | `[0,0]` | `ModuStatus=1, CalibStatus=1` | 非空；重算后矩阵与同一图像对一致 |
| 5 | 送 source `frame-0` + 全黑 target | `0x10103002` | `[0,0]` | `ModuStatus=0, CalibStatus=0` | 空 |

这组受控回放直接确认：一侧图像待配对时切换到 File 并经历 File 算法错误，再切回 Image，不会清除 pending 的 source 标志；Image 空帧调用本身返回 `0xE0000314` 且没有输出写入，但既有 SDK 结果仍可读。随后到来的 target 使缓存 source 与新 target 组成算法输入并清除 pending 标志。另一条支路确认真实 Image 求解失败（全黑输入导致 `0x10103002`）时，本轮模块输出写 0/0，算法 SDK 的 `GetResult()` 从先前非空变为空。

这些观测分别属于模块的 `VM_M_SetInt/VM_M_SetFloat` 写调用与 SDK `IMapCalibTool::GetResult()`，不是 VisionMaster VM 壳层的输出字典提交/清理行为。`Process` 反编译源 `analysis/map-calibration/ghidra/module-analyzed/180021690.c`（SHA-256 `112ABD26934DD4288BD4A99F0675196A895B671964255AE80C9290EE1005C028`）显示 Image 分支在缺帧时先回填实例 pending 标志；仅当两边标志都置位时才重置并运行。`GetInputImage` 源 `analysis/map-calibration/ghidra/module/1800203d0.c`（SHA-256 `5E9F0933E3B806D32E7EBE6F067F05013573F965F4563D9FAC40176581697402`）将收到的帧创建/写入各自实例 image，并设置该侧标志。`ProcessAlg` 源 `analysis/map-calibration/ghidra/module-analyzed/180021c50.c`（SHA-256 `643F1892E704C1039C234485163CB653E61D01CC7F67F8B88C9C3A7DEFAE3972`）运行 SDK 后取 `GetResult`，按结果非空与否形成状态。源与新回放在这些条件上相符。

探针已编译并运行，脚本内六步状态断言全部通过；仅有 SDK 头文件的既有 C4091 警告。未执行设备测试。未修改生产实现。VM 壳层输出保留、动态端口缓存以及用户界面历史结果如何显示仍未证实；要闭合这些部分，需要对真实平台每轮提交的输出对象/订阅值做原厂运行 trace，不能由本 probe 推断。


