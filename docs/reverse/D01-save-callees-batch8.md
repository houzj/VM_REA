# D01 第八批：保存 callee、命令与异步边界

基线 `5e76aee`。按 reverse-engineer-anything 证据分层，只读原始 PE 与已有 MSVC dumpbin；沿用第七批已记录的 REA native provider 未配置阻塞，不重复安装或 setup。不执行目标、SDK，不触碰 tool-plugins，无产品改动。两指定 callee 与各一层消费者已取得，格式/reader/跨模块实际绑定仍未闭合。

## 身份与可复验范围

原件根 `D:/Program Files/VisionMaster4.4.0/Applications/`：

| 原件 | SHA256 |
|---|---|
| PublicFile/x64/iMVS-6000PlatformSDK.dll | b62d7e15588ccda6671228aebc11fec3e27aca878f0e03067e44b17d5a54a9a1 |
| ModuleProxy/x64/ModuleFrame.dll | 45c57bc46f72fd1f9e0b3a6f9183ff974d14590cfd05ecf4e9fa97a4ff95ae9b |

探针 `tests/NativeEvidence/D01SaveCallees/InspectCallees.py` 从第七批探针独立派生，不修改旧探针。最终目录 `analysis/full-audit/batch8-20261019/D01/instructions-7cdc69808fdc483187f0b442d7b86ab4/`；functions.json 记录原件前后全哈希、dumpbin 身份与命令、逐段字节 SHA。normal import directory 按 OriginalFirstThunk/FirstThunk 静态解析，未模拟延迟导入或运行时 GetProcAddress。

| 函数 VA | .pdata 范围 RVA，右端不含 | 指令行 |
|---|---|---:|
| SDK 180665ba0 | 665ba0–6664ce | 459 |
| ModuleFrame 180046f10 | 46f10–47151 | 144 |
| ModuleFrame 参数函数地址 180049410 | 49410–4afcd | 1430 |
| SDK 1806c00d0 | 6c00d0–6c0178、6c0178–6c0446、6c0446–6c0461 | 198 |

前3项各1个 primary 区段。最后3段以完整 RUNTIME_FUNCTION tuple 链回 `(6c00d0,6c0178,b96778)`；不以共享 unwind RVA 归并。共2231行，6段逐字节及每条已打印 opcode 前缀核对通过；这不是另一解码器复核，也不包含语言特定 EH funclet。

复验：运行 InspectCallees.py 生成新 GUID 目录，再运行 `Verify.py <目录>`。Verify.py 会重新核对原件全哈希、region.bin、摘要和打印前缀，输出 resolved-calls.json/verification.json。原件前后哈希均一致。

## ModuleFrame：事件和函数参数，不能把返回 0 当落盘完成

180046f10 的 RCX 保存到 RBX，RDX 保存到 RDI。对象 +10h/+18h 为零时，分别通过 IAT VA1802040b0 调用 **KERNEL32!CreateEventW**，参数 `(NULL,1,0,NULL)`；两个结果写回对应字段。任一仍为零走返回 e0000002。两句 ResetEvent 分别作用于这两个字段；失败仅调用 GetLastError 后继续，不能推断异常事务。

第二参数通过 180006470 复制到局部；18000da760 接受该局部与 `global[1802a56c4]==1` 的布尔值，类型/编码转换的精确语义未还原。随后 **18004700e 将地址180049410置RDX**，R8 指向包含原对象指针的局部，R9 为前一返回对象，调用 180070bf0，再经180070c40、1800f3be0、1800fb9d0（全局1802b2998）处理封装。正常路径1800470e7清EAX返回0。

因此已证这是把函数地址与对象/参数交给封装的路径；异步执行是高度相符的解释，但**没有取得封装实现，不把具体线程/队列契约当已证源码**。返回0只证明此正常路径的返回值，不能证明文件已写完。

向下一层取得180049410：入口 RCX/RDX 分别保存到R13/R12，并写对象+21h=1；包含 IAT `_wstat64i32`、`_waccess_s`、`_wmkdir` 路径检查，以及 CreateEventW、WaitForSingleObject 调用。1430行包含多组容器遍历、函数封装和后续处理，本批未逐个还原其字段语义或全部callee。没有从这些导入直接猜出完整 .sol 格式、临时文件策略或回滚。

## SDK：实际先形成文本命令，再调用对象函数指针

180665ba0 将入口RCX/RDX/R8保存到R13/R14/R15。R14/R15为空、R15的NUL终止字符串连终止字节长度大于20h等分支走e0000001。它不是在本段直接调用文件API。

VA180666247 的静态 IAT 已解析为 **MSVCR120!_snprintf_s**。格式字符串 RVAac5ed0 原字节为 `%d-%d\t%s\t%s\t%s\t%d`（此处 `\t` 表示实际09字节；static-literals.json保留hex）。RCX为栈上800h容量缓冲区，RDX=800h、R8=7ffh；可确认格式实参依次为：43dh、对象+1ach的DWORD、对象+1c0h字符串存储（容量+18h决定内联/指针）、入口第二参数R14、入口第三参数R15、局部EBX布尔值。EBX与对象+c978h及前一字符串查找18005b2040的结果有关，具体标志意义未知。日志使用的原文字串 `CServerManager::SM_SaveSolution` 也被按原RVA提取；名称不是文件格式证据。

VA18066626c调用1806c00d0，RCX=原对象、RDX=格式化缓冲区、R8=扫描后的长度。后者内部再次计算NUL长度，并在1806c01d9从 **[对象+2398h]** 取函数指针：不是虚表+48h。1806c0200调用该指针，RCX=命令地址、RDX=字节长度、R8=栈上响应缓冲区、R9=3ffh，第五栈参数=局部输出长度地址。响应/返回的后续规则包括：返回e0000012时要求输出长度非零，另分配缓冲并调用180662a70；否则沿返回码路径。正常零码分支比较响应前三字节（静态源 `OK\0`），不相符时使用 **sscanf_s** 和原格式 `%[^*]*%d` 取错误数，匹配数不为2保留e0000108。这里只报告比较与解析指令，不定义完整协议。

**跨模块边界仍未连通**：没有查明对象+2398h的赋值点/实际目标，不能声称SDK命令已绑定到ModuleFrame保存导出；没有证明180662a70的传输角色。最终writer API、加密/压缩、字段布局、reader、落盘事务与原厂往返依旧未知。

## 尝试、局限及退出

首轮 instructions-092c367112a84585aa189665d68c92d1 取得指定两callee；第二轮 instructions-622c4c063b7e4f18b004c1e30a0fe5e3 增加一层消费者和normal imports；最终轮仅增补4个精确RVA字串，3轮成功且全部保留，不叠加为覆盖数。当前探针无目标执行失败；终端长预览截断不影响磁盘原始完整文本，结论以磁盘证据为准。协调中曾误写“虚表+48”，即时更正为字段+2398h，原沟通不作为源码结论。

本批有限退出条件达到：两callee范围、调用/返回、参数消费者及下一缺口已固定；不代表D01.3整项完成。D01.3保持源码partial，实现/验证不加分；96项总完成分子不因2231条指令增加。

下一定向问题：SDK对象+2398h赋值与180662a70实际绑定；ModuleFrame180070bf0/1800fb9d0如何执行180049410，以及该callee实际文件序列化者。原生反编译provider与受控原厂运行条件未解除；本批不假造C++源码或原厂差分。

