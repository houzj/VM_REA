# C02.2：流程等待时间的原生导出与命令传输

2026-10-22。取证对象是安装原件 `D:/Program Files/VisionMaster4.4.0/Applications/PublicFile/x64/iMVS-6000PlatformSDK.dll`，SHA-256 `b62d7e15588ccda6671228aebc11fec3e27aca878f0e03067e44b17d5a54a9a1`。本次只静态读取 PE 并反编译指定地址；没有加载 DLL、建立 SDK 实例、连接设备或修改安装目录。REA `open_binary` 确认该 PE 身份；其深度 provider 在本机报告 `provider_unavailable`（Ghidra 未配置到 REA、Hopper 不支持 Windows、IDA 未配置），故使用仓库既有受控 Ghidra 12.1.4 脚本 `tests/NativeImageBinaryProbe/Run-GhidraFunctions.ps1`，输出留在 `analysis/full-audit/reverse-c02-20261022/`。伪代码是反编译结果，不是原始 C++ 源码。

## 已确认的入口至传输链

1. 原托管 `analysis/decompiled/VM.PlatformSDKCS.Audit/VM.PlatformSDKCS/ProcessManager.cs::SetProcedureWaitTime(uint,uint)` 将 `CVmObjectBase.mhandle`、流程 ID、等待值传入 `ServerSDKAPI.IMVS_SetProcedureWaitTime`；非零返回转成 `VmException`。`NativeMethods.cs` 的 P/Invoke 目标是 `.\\iMVS-6000PlatformSDK.dll`。读取方法同样转发，并以 `ref uint` 接收结果。
2. x64 PE 导出表给出 `IMVS_SetProcedureWaitTime` VA `0x18064ed60`、`IMVS_GetProcedureWaitTime` VA `0x18064ee70`。两者先从列表按传入 handle 寻找对象、取得次级句柄，并再次核对；失败路径返回 `0xE0000006`。这是本导出体的判断，不能据此概括服务端错误全集。
3. 设置导出的汇编 `0x18064ed7a–0x18064ed80` 保存 `R8D`（等待值）、`EDX`（流程 ID）、`RCX`（handle）；`0x18064ee43–0x18064ee50` 显式把 `R8D/EDX` 送至 `0x1806abda0`。所以第三参数没有因伪代码签名而消失。读取导出对应把 `R8` 输出指针传给 `0x1806abf40`。
4. `0x1806abda0` 拒绝 `processID < 1`，正常路径以 `sprintf_s` 格式串 `%d-%u-%u-%u`、常数 `0x41f`、流程 ID、字段号 `8`、传入等待值组装命令，再调用 `0x1806c00d0` 发送；返回该传输调用的错误码或 0。字符串来自该 PE 的 RVA `0xac9560`，不是根据产品 UI 猜测。
5. 读取路径 `0x1806abf40` 拒绝空输出指针或 `processID < 1`，以 `%d-%u-%u`、常数 `0x420`、流程 ID、字段号 `8` 组装查询，经 `0x1806c1f30` 获取返回；响应满足 `OK` 前缀时把第二字段的 `atoi` 结果写到输出指针。字符串分别位于 RVA `0xac5830`、`0xac5258`，扫描格式 `%[^*]*%d` 位于 `0xac5208`。Ghidra 对复杂响应分支的局部变量恢复不够稳定，具体错误码/所有畸形响应路径尚不作结论。
6. 后续定向反编译 `0x1806c00d0` 表明设置命令使用 SDK 对象偏移 `+0x2398` 的函数指针发送。汇编 `0x1806c01d9–0x1806c0204` 显示先检查该指针，再将命令字符串、长度、响应缓冲与输出指针交给 `CALL RAX`，并将其返回码存入 `EDI`；`0x1806c0424` 将 `EDI` 送回 `EAX`。Ghidra 伪代码把函数恢复为 `void` 是错误的，不能据此说下层错误被丢弃。特定返回码 `0xE0000012` 进入 `0x180662a70` 重发路径；读取命令经 `0x1806c1f30` 转入 `0x1806c20b0` 并持有返回缓冲。当前证据只到**客户端传输回调边界**，没有定位真正处理命令的进程或计时器。

7. 定向反编译和原汇编进一步消除了“备用传输”的歧义：`0x180662a70` 在 `0x180662a7a` 读取同一个 `+0x2398` 回调；存在时于 `0x180662ab1` 以尾跳转传递重排后的原参数，不存在时返回 `0xE0000010`。它不是已证实的另一接收通道。查询下层 `0x1806c20b0` 首先分配并清零 `0x2000` 字节、用容量 `0x1fff` 调用同一回调；只有回调给出 `0xE0000012` 时才检查所需长度、释放初始缓冲、分配 `required+1` 并第二次调用同一回调；成功把缓冲和响应长度交给上层，失败释放缓冲并传回错误码。这里可确认的是客户端的变长响应重试与所有权转移，仍不能推断服务端计时策略。`0x180662a70` 的伪代码把尾跳转显示为一次普通调用，其实汇编是 `JMP R10`；按汇编解释。

8. 对客户端 `.text` 中位移 `0x2398` 的有界字节扫描共得 8 个候选，并以 x64 `.pdata` 函数范围校验：`0x180661ab3` 是唯一取得非零回调的直接写入点，函数 `0x1806615b0` 先构造 `..\\..\\Server\\PlatformManager.dll` 路径、`LoadLibraryW`，再以 `GetProcAddress` 查导出 `PF_M_MsgDeal`，写入 SDK 对象 `+0x2398`；失败时 `0x180661d00` 将 `+0x2370` 至 `+0x23a8` 的函数指针清零，并 `FreeLibrary` 句柄 `+0x2368`。扫描的另外一处写入正是这个清理分支；其余候选为读取或非该字段指令。字面量在客户端 PE 的 RVA `0xac0aa8`、`0xac40d0`、`0xac41c8`，`tests/NativeEvidence/C02/FindTransportSlotRefs.cjs` 可只读复验。此结论只涵盖该 DLL 的直接位移写入；间接内存复制仍未穷尽。

9. 原安装件 `D:/Program Files/VisionMaster4.4.0/Applications/Server/PlatformManager.dll` SHA-256 `06D86F51BEBFEBA357D1B79297E20A3B0A70580A40AF7B77C8ABDD6C38CA0624`。REA `open_binary` 确认 x64 PE 身份；本机深反编译继续用独立受控 Ghidra 输出。PE 导出表确认 `PF_M_MsgDeal` RVA `0x6a0250`（VA `0x1806a0250`），并非转发导出。该导出拒绝空响应缓冲或容量 `<0x200`，返回 `0xE0000012`；正常路径将五参数传给 `0x180602df0`。该分派函数的 `case 0x41f` 进入 `0x18061a320`，`case 0x420` 进入 `0x18061a8b0`。原分派伪代码约 1.6 万行，记录具体 case 和下层函数，而不把整个分派树宣称已审核。

10. `0x18061a320` 要求命令拆出四字段（流程 ID、字段号、值），经 `0x1805e5c10` 操作流程配置容器并 `0x1805dd580` 找流程对象；字段号 `8` 时 `atoi` 值最终以 `MOV [RSI+0x2a8],EBX` 写入该对象。`0x18061a8b0` 要求三字段，调用 `0x1805e5de0` 取值，同样找流程对象；字段号 `8` 时把转换值写入对象 `+0x2a8`，再格式化响应。两处理函数还处理字段号 `4`，但这不是本次等待时间结论。这里定位了**服务端存储字段**；后续读取位置见第 11 条，计时、取消和回调关系仍未知。Ghidra 对处理函数返回型恢复为 `void`，本报告不据此推断成功码完整集合。`node tests/NativeEvidence/C02/PlatformManagerDispatchProbe.cjs` 复验服务端 PE 身份、导出、分派 case 与原汇编写入点。

11. 继续静态扫描服务端 `.text` 中 `0x2a8` 位移，得到 393 个**原始字节候选**；大量位于栈访问或其他结构，绝不能把 393 当成实际字段引用。`tests/NativeEvidence/C02/FindWaitFieldRefs.cjs` 默认只显示已定向检查的函数，`--all` 给出全部字节候选和 `.pdata` 函数起点。汇编证实 `0x1805dda30` 在 `0x1805ddc00` 从流程对象读 `+0x2a8`，置入临时记录并交 `0x1805d2890` 建立列表元素；`0x1805eec10` 在 `0x1805eee8c` 从流程对象读 `+0x2a8`，写到其第三参数所指记录的 `+0x10`，随后才继续调用对象虚方法。`0x1806a32e0` 还在初始化对象时写 `+0x2a8`。另一个 `0x180651750` 虽有 `LEA [RDI+0x2a8]`，同时成组处理 `+0x298/+0x2b8/+0x2c8` 的容器元素，尚不能把这个偏移认作同一流程对象字段。以上确定了**保存后的值至少进入两条下游记录构造链**，但记录如何供执行器计时、超时是否停止/仅报告、是否影响回调仍未证实；`FUN_1805eec10` 的反编译数组别名把 `+0x2a8` 表示为 `plVar13[0x55]`，以原汇编为准。

12. 对两条读函数做 PE `.text` 的 `E8 rel32` 反向候选搜索，并以 Ghidra 函数汇编复核，得到 `0x1805aa6f0` 在 `0x1805ab371` 调用 `0x1805eec10`；`0x1805b7770` 在 `0x1805b86ca`、`0x180622e70` 在 `0x180622f41` 调用 `0x1805dda30`。`0x1805aa6f0` 在调用时传入流程配置容器和一个本地输出记录，成功分支准备一个 `0x7fe` 格式的后续消息；仅凭此调用上下文，尚不能给该函数命名为执行超时回调。另两位调用者在多个配置容器操作间生成流程列表，不能直接视为计时器。`tests/NativeEvidence/C02/FindServerDirectCallers.cjs` 复验同 SHA PE 的相对调用位移；它不能找到间接调用或证明仅有这些调用者。

13. `0x1805aa6f0` 的成功路径在 `0x1805ab4d1–0x1805ab54b` 先格式化 `0x7fe` 并调用 `0x1806203a0`，随后有多轮记录与对象处理，至 `0x1805ab6e1` 才调用 `0x1805cd170`，传入同一个本地流程记录地址；不能把这两次调用压缩成单一步骤。`0x1805cd170` 原汇编保留记录指针于 `R14`，转入 `0x1805cce90` 的 `R8`，之后创建含 `0x70` 字节的对象并调用虚方法 `+0x80`；反编译仅显示它最终从该对象取字符串，未解释 `0x1805cce90` 如何使用记录中的等待字段。当前可证是字段由服务端对象进入运行前记录，再穿过 `0x1805cce90` 的参数边界；`0x7fe` 的外部语义和计时/取消效果未证。原汇编/反编译与 `tests/NativeEvidence/C02/PlatformManagerDispatchProbe.cjs` 共同复验这个有序链。

14. 继续解开 `0x1805cce90`：入口 `R8` 被保存为 `RSI`，在 `0x1805cd0a9–0x1805cd0af` 以第三参数传给 `0x1805cc780`。该函数的原汇编 `0x1805cc9ab–0x1805cc9b9` 从记录 `+0x10` 取 32 位值，连同 `.rdata` 的 `RunTimeout` 键交 `0x1806aca10`；`tests/NativeEvidence/C02/ReadRunRecordKeys.cjs` 对同 SHA PE 读取并校验该键，另可见 `Index/DisplayName/ShieldGlobalCtrl/ContinueExecuteTimGap/Guid/StopWhenNG`。因此原 `SetProcedureWaitTime` 的字段 8 已追至服务端流程对象 `+0x2a8`、运行前记录 `+0x10`、序列化对象的 `RunTimeout` 字段。这里证明**运行配置构造/序列化**，不是执行器已按时计时或中断；`0x1805cce90` 后续多个序列化步骤遇到非零错误会提前返回，不能把该记录当成总能完整生成。反编译对 `0x1805cce90` 省略了寄存器实参，结论以原汇编的 `R8→RSI→R8` 和 `0x1805cc780` 中的 `R14+0x10` 为准。

15. 对同一 `.rdata` `RunTimeout` 地址 `0x1807d2150` 做原 PE `.text` 的 RIP-relative `LEA` 候选扫描，只有四处：`0x1805c4878`（`0x1805c43d0`）、`0x1805c81f0`（`0x1805c7ea0`）、已知写入点 `0x1805cc9af`（`0x1805cc780`）、`0x1805ce254`（`0x1805ce0e8` 代码范围）。逐处原汇编复核后，`0x1805c43d0` 从配置键找值、转换为整数，缺键时将相应本地值置零，并交 `0x1805d2890` 建列表元素；`0x1805ce0e8` 代码范围也查键，存在时经 `atoi` 写记录 `+0x10`，缺键或空值时写零。`0x1805c7ea0` 则从其输入记录 `+0x20` 以 `RunTimeout` 键再次构造配置对象。这里形成一组**配置读/写/再构造**边，但 `0x1805ce0e8` 的反编译入口显示未追明的上游寄存器，不能把它单独当完整函数；四处候选也只覆盖该 DLL 的此种 RIP-relative `LEA` 形式，不能证明其他模块或间接引用不存在。`tests/NativeEvidence/C02/FindRunTimeoutRefs.cjs` 只读复验候选，`PlatformManagerDispatchProbe.cjs` 核对实际反汇编及哈希。暂未发现执行器启动计时或取消的具体指令。

16. `E8 rel32` 候选及原汇编共同确认 `0x1805c5750` 在 `0x1805c58d7` 调 `0x1805c43d0`，`0x1805c5f40` 在 `0x1805c617f` 调 `0x1805c7ea0`。两位调用者都把该处理置于多段配置构造/转换序列内，每次非零返回提前退出，未见直接的计时器或取消调用；这进一步限定了当前所追对象仍是配置/列表链。`0x1805ce0e8` 没有在本次直接 `E8` 扫描中出现，可能为更大函数的代码范围或由其他控制流进入，不能以零命中断言它不可达。`FindServerDirectCallers.cjs` 的自定义目标模式可只读复扫，原汇编产物另以哈希固定。

17. `0x1805c43d0` 的列表构造调用 `0x1805d2890`，后者将输入记录传给 `0x1805c2570` 复制到元素内部偏移 `+0x10`；原汇编逐个复制记录前五个 32 位字段，因而输入 `+0x10` 的等待值成为列表元素内部 `+0x20`。另一条已确认的 `0x1805c7ea0` 序列化路径恰从输入元素 `+0x20` 取值写 `RunTimeout`。这是同一字段在配置元素中的布局证据，不代表这些具体元素之间已经证明是同一运行实例；还需识别列表发布/选择与实际执行消费者。`0x1805c2570` 的后续两段字符串复制保留原样，不能把 0x68/0x90 等分配尺寸误认作等待时间。

18. `0x1805aa6f0` 使用的 `0x1806203a0` 不是计时器：原汇编先检查消息和长度指针，持锁后读取服务端对象 `+0x7fc48`，以消息指针、长度和 `+0x7fc50` 上下文调用该函数指针。返回码保存到 `EDI` 仅供可选日志，正常消息路径在解锁后以 `XOR EAX,EAX` 返回 0；空指针则返回 `0xE0000001`。所以调用者收到 0 **不能证明回调实际处理成功**。同一字段位移在原 DLL `.text` 仅有五个原始字节候选，汇编确认 `0x180631510` 于 `0x180631582` 将其第二参数写到 `+0x7fc48`，第三参数写到 `+0x7fc50`；另 `0x1806315f0` 也读取并调用该指针。`0x180631510` 在本次直接 `E8` 扫描和导出表中均无命中，故绑定方与实际回调模块仍未知，不把 `0x7fe` 外部语义猜成超时停止。`tests/NativeEvidence/C02/FindServerRunCallbackRefs.cjs` 只读查原 PE 位移与导出，已确认地址以原汇编和哈希核对；该扫描不覆盖间接写入/复制。

## 对先前缺口记录的更正

`docs/reverse/C02-scheduling-batch2.md` 早已从原 `VM.Util.dll` CIL 和受控探针取得 `Hik.Util.SyncWaitor` 的实际实现：普通 `Wait/PrecisionWait(ref bool)` 无托管超时；P2P 数组重载有独立 nTimeout，超时返回时 flag 可保持 false。`docs/reverse/C-timeout-boundary-batch25.md` 及 `docs/MF05-PROCEDURE-RUN-CALLCHAIN.md` 中“SyncWaitor 实现未在反编译树/未取得”的描述已过期；准确缺口是 **native 服务端如何消费 0x41f/0x420 命令、何时开始计时、超时如何改变执行/回调/错误状态**。此更正不覆盖第二批历史运行结果，也不证明整个原生调度器行为。

当前 main 的 `ProcedureDocument.WaitTimeMs`、`SolutionExecutor.RunAsync` 仍为独立的 `CancellationTokenSource.CancelAfter` 协作式流程预算；已确认的原导出只是把字段 8 传给下层，未见在导出体内取消执行。不能把两者宣称等价，也不能仅因传输格式已知就改写产品超时逻辑。C02.2 保持 `implementation=partial`，总实施进度不增加。

## 复验与下一取证点

`node tests/NativeEvidence/C02/ProcedureWaitTransportProbe.cjs` 对同 SHA 原 PE 静态核对导出地址、命令字符串以及保存的伪代码/汇编关键数据流，实测退出 0。取证产物：

| 文件 | SHA-256 |
|---|---|
| `analysis/full-audit/reverse-c02-20261022/native-wait/output/18064ed60.c` | `D5D37876DD9EA6EE7C0B7BD3DF23DC972C874C57F2C68FDEAB01F4FA925E1C64` |
| `analysis/full-audit/reverse-c02-20261022/native-wait/output/18064ed60.asm.txt` | `B820F497D8687B84FB039E110DB4CED0A98EEBDA6B39DE21C241E4DD73CF3FD0` |
| `analysis/full-audit/reverse-c02-20261022/native-wait-callees/output/1806abda0.c` | `CC2C1CBF8435626C48D782261CB196487A15210869001607B2F5D334A6071EFA` |
| `analysis/full-audit/reverse-c02-20261022/native-wait-callees/output/1806abf40.c` | `FBF82DC683DEC0ABC0A63517363403704D25BC19963CF62266F99A614A2D7EAD` |
| `analysis/full-audit/reverse-c02-20261022/native-transport/output/1806c00d0.c` | `00DA7DEB7E8B35F2C98B7C83323E6FA30C02FCF87B82E915F5C4D08484763053` |
| `analysis/full-audit/reverse-c02-20261022/native-transport/output/1806c00d0.asm.txt` | `D2EF29B0AE20ED85E78BD692E110CF28AD4EE7E197AACFCF3DC3F121B35DBC14` |
| `analysis/full-audit/reverse-c02-20261022/native-transport/output/1806c1f30.c` | `2FA0821E3B4D908A5852944145AAE41AFF20DDAD38754EA21ECF0E6E793AFEAC` |
| `analysis/full-audit/reverse-c02-20261022/native-transport-fallback/output/180662a70.c` | `A63422BEEB789175A4B840B3C9A0A1D00CA606D4C3CE298BC86489BE74242B4B` |
| `analysis/full-audit/reverse-c02-20261022/native-transport-fallback/output/180662a70.asm.txt` | `E08BB31038A926A5B83460E19D056912B8A82B930A546CE5F6DB7EC9D5C12D71` |
| `analysis/full-audit/reverse-c02-20261022/native-transport-fallback/output/1806c20b0.c` | `CDEF9D4FD64C594B4831ACE760C151C73A3B4DAEBD51993FF5BEFC3ADEFA5930` |
| `analysis/full-audit/reverse-c02-20261022/native-transport-fallback/output/1806c20b0.asm.txt` | `C0E8FE6D5A48D7B76F52B2F2E035FF3772E6C0CD41E0C1D2C22179C5A65AE875` |
| `analysis/full-audit/reverse-c02-20261022/native-transport-writers/output/1806615b0.c` | `7A0BF7CAFEA848CF456C3F4EE84FCA67E9AF996593778F83DCBB5D9B6694BFF6` |
| `analysis/full-audit/reverse-c02-20261022/native-transport-writers/output/1806615b0.asm.txt` | `E01B03F86BB972705EAE2D98AC80FD5BA6604748C68E89F4FFC2916EE34CB5A2` |
| `analysis/full-audit/reverse-c02-20261022/native-transport-writers/output/180661d00.c` | `6290DA1844F198E910D40D0F99E0F818F7B793D0A416D4C96E05D75F7461CE4A` |
| `analysis/full-audit/reverse-c02-20261022/native-transport-writers/output/180661d00.asm.txt` | `AF520D8D8ADBBDBB9AEE42526430AC410835724A098AE6A3EC7AC77DA99D4B8F` |
| `analysis/full-audit/reverse-c02-20261022/server-message-export/output/1806a0250.c` | `0BE6BCBC9C25545ABB38AD1C4818E52184789F4796EF18D5E2F324F3AF4A982B` |
| `analysis/full-audit/reverse-c02-20261022/server-message-export/output/1806a0250.asm.txt` | `4394E7DE22E263778D1A9DC779A258153A6C4C3E37A7EB02A4DD1FBB13B05629` |
| `analysis/full-audit/reverse-c02-20261022/server-message-dispatch/output/180602df0.c` | `AF99B707CB47309609C63D83992826728CD1190F27A381A6AD25A811B0517C46` |
| `analysis/full-audit/reverse-c02-20261022/server-wait-handlers/output/18061a320.c` | `0F37DAFAB4C96D7C529FBB6A347D52B6DEA1A3B1F5F6AC2331AB669747B1D642` |
| `analysis/full-audit/reverse-c02-20261022/server-wait-handlers/output/18061a320.asm.txt` | `657E5C24C5142FA923517539C7B02B406070FDE10A67924B85E7D46C3AEFB10C` |
| `analysis/full-audit/reverse-c02-20261022/server-wait-handlers/output/18061a8b0.c` | `DCC18D71986441CE0FCD5D94172E324724315A46B262F31AAF80A8D686E078A1` |
| `analysis/full-audit/reverse-c02-20261022/server-wait-handlers/output/18061a8b0.asm.txt` | `8B6D9FB79152E40F0E88BC80D9E2465D0F3C2FDA1CD07CF88C815FCCF10CD774` |
| `analysis/full-audit/reverse-c02-20261022/server-wait-field-consumers/output/1805dda30.c` | `E510FA79451361C8EFDD9E63F9FA0A0D71FA3F1A2E29E22A4AC1A2B76166771C` |
| `analysis/full-audit/reverse-c02-20261022/server-wait-field-consumers/output/1805dda30.asm.txt` | `6B525056585DEC891011B4F4937C799137E4F86EDFB02C2FA754CAF303ECCCA0` |
| `analysis/full-audit/reverse-c02-20261022/server-wait-field-consumers/output/1805eec10.c` | `CAFB54C49085221CC6F4518C0AC426B43CB56F3CEF4C285CE317CD1D7F6532B9` |
| `analysis/full-audit/reverse-c02-20261022/server-wait-field-consumers/output/1805eec10.asm.txt` | `9DEE5580FE02BE7AE65921BC6B491F3777AC361088BBBEF953D3ECD57E8BA33B` |
| `analysis/full-audit/reverse-c02-20261022/server-wait-field-consumers/output/1806a32e0.c` | `3C7BEC711BB2160F62E6D0D92050952BB7112B5CA448297DA576EA180EAF8A65` |
| `analysis/full-audit/reverse-c02-20261022/server-wait-field-consumers/output/180651750.c` | `E15F7FE1FF5D9025100E439088CFD34DEA48C8F90793FBCB1E3CAF34C24B4C18` |
| `analysis/full-audit/reverse-c02-20261022/server-wait-snapshot/output/1805d2890.c` | `F1EDBDA8DC367B235C126325DAB1315F2531E179FCCC868B2371460B5E0912A7` |
| `analysis/full-audit/reverse-c02-20261022/server-wait-read-callers/output/1805aa6f0.asm.txt` | `CAD64373914E1621CB69DEF8116ACEC508B5D7F3919A4F101BF53C56B9D47F51` |
| `analysis/full-audit/reverse-c02-20261022/server-wait-read-callers/output/1805b7770.asm.txt` | `9D05863DF06D363B69AC90C33FD4D59CC77DEB40BD9DD7BA844EF67B419D746C` |
| `analysis/full-audit/reverse-c02-20261022/server-wait-read-callers/output/180622e70.asm.txt` | `B2412AB0A69EF436EAFFC978D7570A5490995E961D08CE57CC6CCD063BB132B1` |
| `analysis/full-audit/reverse-c02-20261022/server-run-record-consumer/output/1805cd170.c` | `FE9BC59C2AE0FD984E6561992F30E2F0D5EC8EE3F02C33A66AC4CAD3D59B92F2` |
| `analysis/full-audit/reverse-c02-20261022/server-run-record-consumer/output/1805cd170.asm.txt` | `77B92B5BDFF0F14D7DA4DDE4D7EA3C1743C61E79841B259678554464C5B590B5` |
| `analysis/full-audit/reverse-c02-20261022/server-run-record-next/output/1805cce90.c` | `5F81FEA09303ECC31D7C63C7A15E385C80BB217DF01AE595A407DDAD2517AC4B` |
| `analysis/full-audit/reverse-c02-20261022/server-run-record-next/output/1805cce90.asm.txt` | `E5461549E4511BF48465C3D9B0DAEAB5F6A9E2CF1306EE5F8DF7FD9354185908` |
| `analysis/full-audit/reverse-c02-20261022/server-run-record-field/output/1805cc780.c` | `B8A282EB6504A0EECEF5FED7E981D8073ADED36E3D3B4069E78ECB32FA4FB813` |
| `analysis/full-audit/reverse-c02-20261022/server-run-record-field/output/1805cc780.asm.txt` | `090A000973E168DDCC0F9298106FCDA7BA79C994FE8E2390ED2402FC19402121` |
| `analysis/full-audit/reverse-c02-20261022/server-run-timeout-refs/output/1805c43d0.c` | `8FCCBCC2174FA0B4401372AFE494D529AB845DAE6196075AC84392C077FBDFA2` |
| `analysis/full-audit/reverse-c02-20261022/server-run-timeout-refs/output/1805c43d0.asm.txt` | `061C4765B1A84BDC57FF4FFC12369AB2D851929273BB52266AFFCFF5AA55C123` |
| `analysis/full-audit/reverse-c02-20261022/server-run-timeout-refs/output/1805c7ea0.c` | `CE357E4FFDC2E3C227E862B797F60E61EC450250D2F434125CAC3E232E23510E` |
| `analysis/full-audit/reverse-c02-20261022/server-run-timeout-refs/output/1805c7ea0.asm.txt` | `581907C1C867EDD389147BF1A3027C493C50CFE0C7061EF84F1712959BB7AF12` |
| `analysis/full-audit/reverse-c02-20261022/server-run-timeout-refs/output/1805ce0e8.c` | `AFF1C8A6B627674089C48286BDA2F998731AA87E322EAB2F1011A2FEFAE0146C` |
| `analysis/full-audit/reverse-c02-20261022/server-run-timeout-refs/output/1805ce0e8.asm.txt` | `24B12C0943799DC57204D2C70E932C9CF59A93B818E0B3A194A131959AD03723` |
| `analysis/full-audit/reverse-c02-20261022/server-run-timeout-callers/output/1805c5750.asm.txt` | `58ADA515BCF67487E3B0B1997678674E6B783F35A73F24F876C3FB648938946F` |
| `analysis/full-audit/reverse-c02-20261022/server-run-timeout-callers/output/1805c5f40.asm.txt` | `60745EBD998C4F34D4AEF07B6C186D3CBBEDD1093BEB7EF7E98196DFEA65EF9D` |
| `analysis/full-audit/reverse-c02-20261022/server-run-timeout-record-copy/output/1805c2570.c` | `5AD3DE41FE112A376DB8845D0B4DB43B33EA4FBEBF3C0CF345D975AE87C3298F` |
| `analysis/full-audit/reverse-c02-20261022/server-run-timeout-record-copy/output/1805c2570.asm.txt` | `426F3AC967C842C69A12ADEC86CEE6AD6AEEDD70374D7B6B710EBDA8ADE30559` |
| `analysis/full-audit/reverse-c02-20261022/server-run-message-bridge/output/1806203a0.c` | `8840B33D4CD30575DB9B9E89431DCFFED3F53FA62A63F2D83AFA17A5FDCC0175` |
| `analysis/full-audit/reverse-c02-20261022/server-run-message-bridge/output/1806203a0.asm.txt` | `A56BDD8E638E12879E5DE03FE14A2C299227D487271CF509E8EFA0D9CAEBDD6A` |
| `analysis/full-audit/reverse-c02-20261022/server-run-callback-binding/output/180631510.c` | `BE5BEBE75AEE675E00E9CD45EE1B52170F456525AF8455A38FBBAB72A72BC28E` |
| `analysis/full-audit/reverse-c02-20261022/server-run-callback-binding/output/180631510.asm.txt` | `8A9C2BC60ADA4879BC5B6352374A30F4F0FBC85D8A8C598A47826B0EF98FF643` |
| `analysis/full-audit/reverse-c02-20261022/server-run-callback-binding/output/1806315f0.asm.txt` | `1BE4F4D9AAD15260D05A56790FFD5436869973EF9408700B0BEABD31EAC4E168` |

下一步沿 `0x1805c43d0` 产生的列表元素与 `0x1805ce0e8` 所在上游完整函数追执行调度入口及异常返回；另须定位 `0x180631510` 的间接调用者/服务端 `+0x7fc48` 回调绑定方，再到 `0x7fe` 接收方。还需校验 `0x1805e5c10/0x1805e5de0` 的配置容器变更及失败时状态。不能从字段已进入 `RunTimeout`、发送侧 `OK` 或 `0x1806203a0` 的零返回推断执行超时语义。之后才可决定 main 有无证据充分的最小实施修复。该取证需要独立命令/状态探针，不使用另一任务的 SDK/桌面实例。
