# D01 第七批：原生保存入口指令

基线 `39a54c810f9b7143cda1f2725a8fcd9e1e7d2a88`。本批取得两个原始 PE 保存导出的指令及异常展开边界；未加载或执行目标，不涉及原厂保存操作、文件格式重建或产品改动。D01.3 源码由未开始推进为部分，不计完成。

## 原件及可复验输出

探针 `tests/NativeEvidence/D01NativeSave/InspectSaveInstructions.py` 使用标准库解析 PE 导出、节和异常目录，调用已有 MSVC dumpbin 导出指定范围。REA 会话检查显示原生 provider 未配置，快照保存在本批 D01/rea-session.json；以下输出是 MSVC 汇编，不冒充 REA 反编译。

| 安装原件相对路径 | SHA256 | 原指令范围，右端不含 |
|---|---|---|
| Applications/PublicFile/x64/iMVS-6000PlatformSDK.dll | b62d7e15588ccda6671228aebc11fec3e27aca878f0e03067e44b17d5a54a9a1 | IMVS_SaveSolution RVA 646470–646584，78行 |
| Applications/ModuleProxy/x64/ModuleFrame.dll | 45c57bc46f72fd1f9e0b3a6f9183ff974d14590cfd05ecf4e9fa97a4ff95ae9b | VM_M_SaveSolutionToFile RVA 13cc60–13cc97、13cc97–13cf00、13cf00–13cf14、13cf14–13cf31，158行 |

原件根目录为 `D:/Program Files/VisionMaster4.4.0/`。最终输出为 `analysis/full-audit/batch7-20261019/D01/instructions-b6c3c571be4e4bd6be05c21800e0d063/`，包括 functions.json、逐段原字节及 dumpbin 文本；原件前后哈希相同。

首轮 instructions-689e17839b7c441a9c560e43ff53a81f 仅导出 ModuleFrame 首段13行，不能称完整函数，已保留。修正按完整 RUNTIME_FUNCTION 三元组解析 CHAININFO，汇集同一 primary 的4段，避免以共享 UnwindInfoRVA 错并下一函数。另保留初始 Python 前置语法错误；未安装 capstone/pefile/iced_x86。

边界依据为 [Microsoft x64 exception handling](https://learn.microsoft.com/en-us/cpp/build/exception-handling-x64?view=msvc-170)：展开码按偶数槽对齐，CHAININFO 后继为先前 RUNTIME_FUNCTION。当前解析不扩展语言特定异常 funclet 或被调函数；4段不代表所有异常路径源码已覆盖。

## 指令确认的数据流

SDK 首参 RCX 用于遍历节点、比较 node+10h；找不到返回 e0000006。第五栈参数转发到后续调用，但第四参数 R9 未被显式保存重置，不能据 wrapper 补完整签名。180665ba0 的返回 EAX 经 EBX 保留后返回。180001070/1800010b0 等 helper 未解，不根据调用形状命名其生命周期或锁语义。

ModuleFrame 于 VA 18013cc83 检查 RCX；空值返回 e0000001。非空参数按字节扫描 NUL，调用 180006300 构造局部对象，18013cd64 调用 18013c9a0 接受三个局部地址；尚未确认具体目录/文件名语义。18013ce65 调用 180046f10，参数为全局地址 1802b12d0 与局部地址 rbp+7；返回 EAX 存 EDI，经过日志及局部清理，于 18013cf0a 恢复返回。18008f750 不是已证 writer。间接 IAT helper 尚未解析名称。

SDK 的 180665ba0 与 ModuleFrame 导出之间未连通；callee 内部的临时文件、写入、替换、回滚和字段布局均未知。wrapper 没有显式成功后提交分支，不代表内部没有事务。

## 复核与下一退出条件

[独立复核](D01-native-save-review-batch7.md) 核对两原件 SHA、完整 parent tuple、5段原字节哈希及236行指令地址/打印字节前缀，一致。复核未运行目标或重跑 dumpbin。

下一有界问题是 180046f10 / 180665ba0 的实际调用与参数消费者，先取得原指令或可信反编译，再归因文件 API 和缓冲区。不重复导出本批入口，不在受控运行条件未成立前启动 SDK。reader、原厂单操作差分和保存重开往返仍缺；没有新增96项完成分子，tool-plugins 未动。
