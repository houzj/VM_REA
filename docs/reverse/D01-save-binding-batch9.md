# D01 第九批：保存消息函数指针的实际静态绑定

基线 f11bc13；按 reverse-engineer-anything 技能复用既有 REA provider 不可用证据，未 setup、未执行原件/SDK，未改产品或 tool-plugins。有限退出条件是查明对象 +2398h 写入与其 DLL/导出，而不是闭合文件格式。

## 原件、工具和复验

SDK 原件 `D:/Program Files/VisionMaster4.4.0/Applications/PublicFile/x64/iMVS-6000PlatformSDK.dll` SHA256 `b62d7e15588ccda6671228aebc11fec3e27aca878f0e03067e44b17d5a54a9a1`。新原件 `D:/Program Files/VisionMaster4.4.0/Applications/Server/PlatformManager.dll` SHA256 `06d86f51bebfeba357d1b79297e20a3b0a70580a40af7b77c8abdd6c38ca0624`。

`tests/NativeEvidence/D01SaveBinding/InspectBinding.py` 扫 SDK 所有可执行节的 `98 23 00 00`，8 个原始候选；先用 .pdata 确定完整 primary/chained 区间，沿用完整 parent RUNTIME_FUNCTION tuple，不以共享 unwind RVA 合并。`InspectMsgDeal.py` 独立导出平台入口。均只读 PE 加 MSVC dumpbin，保存工具哈希、命令、原件前后哈希、逐段 bytes 和哈希。

最终证据：`analysis/full-audit/batch9-20261019/D01/instructions-5fb54595a19848ea94331c925ee59dae/`（SDK）与 `instructions-9f0e4b6064d34738958b39d3344685f8/`（平台）。对各目录运行同目录探针 Verify.py：逐段 bytes/打印 opcode 前缀一致，原件未变；69918、48 条打印前缀核对。大函数导出中多数指令未作语义分析，不把数量当覆盖。不是独立解码器，也未覆盖语言特定 EH funclet。

## 确认的绑定与生命周期

SDK 1806615b0 入口 RCX 保存至 RSI。661AAD 调用 IAT VA180A391C0，normal import 解析为 KERNEL32!GetProcAddress；RCX=[RSI+2368h]、RDX=VA180AC41C8 原始 ASCII `PF_M_MsgDeal`。661AB3 把 RAX 写入 [RSI+2398h]，随即检查零。这是实际写入证据，不是从裸字节猜 xref。

+2368h 在66185A接收 LoadLibraryW（IAT180A391B8）的返回值；此前 RCX 从局部宽字符串取得。路径链：6616C7 调6d8750，RCX 的原始宽字符串为 `iMVS-6000PlatformSDK`，RDX 是输出字符串地址。6d8750 内 GetModuleHandleW→GetModuleFileNameW（104h宽字符缓冲）→SHLWAPI!PathRemoveFileSpecW→5b2a40写回目录。initializer 继而把该目录、原始宽字符串 `\..\..\Server\`、`PlatformManager.dll` 交给5b3350/5b3210，再将结果供 LoadLibraryW。这两字符串组合 helper 未在本批独立逐指令还原，因此路径拼接的最终值是由参数和加载位置支持的静态解释，未运行证明搜索/规范化规则。

按当前 SDK 所在目录解析，候选对应实际存在的 Applications/Server/PlatformManager.dll。该原件 export table 有 PF_M_MsgDeal，RVA6a0250；不声称当前用户进程已经加载此哈希，也不排除其它部署路径/加载搜索行为。

SDK 180661d00 中 ESI 清零，661D89 将 +2398h 清零，同段清 +2370h 至 +23a8h；随后对 +2368h 非零句柄调用 KERNEL32!FreeLibrary。是否所有异常路径必到此处、对象完整析构契约未闭合。

SDK 180662a70 是另一处实际消费者：从 RCX+2398h 取 R10，非零将后续参数前移并尾跳 R10；零返回 e0000010。与第八批6c00d0消费者共用同一字段，不能误称另一个响应传输协议。

## 平台入口参数与下一边界

PF_M_MsgDeal 完整 .pdata 主段 [6a0250,6a0318)，48 条。保存 RCX/RDX/R8/R9 到 RBP/RSI/RDI/RBX。R8为空或R9小于200h，返回 e0000012；否则经全局初始化守卫（global1808F6AE8 bit0），以 RCX=global1808F6AF0，RDX=原命令指针，R8=原第二参数，R9=原响应缓冲，栈第五参数=原R9，第六参数=入口第五参数，调用180602df0。正常返回直接保留该 callee 的 EAX。

因此SDK保存命令生产者→+2398h消费者→PlatformManager!PF_M_MsgDeal 的静态绑定已经连通；尚未追602df0如何解析43dh命令、分发到ModuleFrame保存，不能声称实际writer、文件格式、压缩/加密、原厂往返已证。

## 扫描排除、失败和复用

8 raw候选中3a7ca5是 add rdx,2398h，5c85d4在巨大5b7ac0函数内实际是栈偏移写浮点，均不是对象函数指针写入。661ab6是真赋值、661d8c是清零、662a7d/6c21e8/6c2328为读取。6c01dc是第八批既有6c00d0链段读取，最终探针直接排除该已知范围。

首次探针单段长度保护小于10000h，巨大候选函数导致 AssertionError；将只读上限调至100000h后完整导出。此轮按片段begin排除旧callee的条件不足，误重复导出6c00d0的198行，保留该轮不计新增覆盖；随后改为按已知RVA范围排除。一次补字串使用外层offset闭包时其size变量被后续解析覆盖，ValueError；改用当前PE的正确offset函数后成功。失败输出目录全部保留，不把失败轮或重复轮累加。终端一次rg通配符路径错误亦未影响证据。

B线独立取得同initializer/clear与原始字串；这是复核，不能相加两次。本线仅归因+2398h，B线+23a8h独立归因，未修改共享文件。

本有限子项完成，D01.3仍源码 partial、实现与验证不加分。下一精确缺口：PlatformManager 180602df0的43dh消息分派与ModuleFrame导出绑定，之后才是序列化实际消费者。没有产品修改、原厂运行、设备操作或人工验收。
