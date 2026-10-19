# D01 第七批原生保存指令独立复核

本复核只支持 D01.3「native writer/reader与单操作字段归因」源码层由未开始推进为 **partial**。未取得 reader、文件格式字段归因、原厂保存操作或往返证据，不能计为 done；实现层、验证层不因本次静态复核增加完成数。

## 原件与边界

复核输入为 `analysis/full-audit/batch7-20261019/D01/instructions-b6c3c571be4e4bd6be05c21800e0d063/` 中 functions.json、各 region.bin 和汇编。使用 reverse-engineer-anything 技能的证据分层要求；本次是原 PE 字节与 MSVC dumpbin 指令输出，**不是 REA 反编译**。已有 rea-session.json 的 provider 缺失状态仍开放。本复核未重新调用 dumpbin、未加载或运行目标、未安装 provider、未操作 SDK。

| 原件 | SHA256 | 确认的导出范围 |
|---|---|---|
| PublicFile/x64/iMVS-6000PlatformSDK.dll | b62d7e15588ccda6671228aebc11fec3e27aca878f0e03067e44b17d5a54a9a1 | IMVS_SaveSolution RVA 646470–646584（右端不含），78 行指令 |
| ModuleProxy/x64/ModuleFrame.dll | 45c57bc46f72fd1f9e0b3a6f9183ff974d14590cfd05ecf4e9fa97a4ff95ae9b | VM_M_SaveSolutionToFile RVA 13cc60–13cf31，4 段合计 158 行指令 |

独立从原件解析 PE exception directory、RUNTIME_FUNCTION 与 UNWIND_INFO：SDK 只有一个所属区段；ModuleFrame 主 tuple 为 `(13cc60,13cc97,2846f8)`，后三段 `(13cc97,13cf00,284710)`、`(13cf00,13cf14,284728)`、`(13cf14,13cf31,28473c)` 均以完整三元组链回主记录。不是根据相同 unwind RVA 合并。五个导出片段的原件字节、region.bin、摘要均一致，236 行指令地址及每行已打印字节前缀与原件一致。该核对不声称独立重解码全部 opcode 或异常 funclet 完整性。

首轮 `instructions-689e17839b7c441a9c560e43ff53a81f` 只取到首区段，不能据其认定 ModuleFrame 函数只有 13 行；失败范围与 first-fragment 脚本继续保留。

## 可确认的数据流

SDK VA 180646470 入口保存 RCX→RDI、RDX→R14、R8→RBP。180646493 调用 180645170 后，返回对象为空、链表找不到键匹配、关联对象为空或二次核查不一致均可走 EAX=`e0000006`。这里只称返回码，不凭数值猜完整错误枚举或并发语义。

匹配后 180646568 调用 180665ba0：RCX 是全局地址 180ca02e0，RDX、R8 分别来自入口第二、第三寄存器参数；入口第五栈参数在当前栈 +70 读取并放入调用第五参数槽 +20。R9 未见本段显式保存/恢复，不能从这一段补出完整签名或认定入口第四参数如何传递。callee EAX 经 EBX 保存，在调用 1800010b0 后返回；0 与非0分支都能保留该结果。180001070/10b0 的具体锁/生命周期含义未分析，不据调用形状命名。

ModuleFrame VA 18013cc83 检查首参 RCX，空指针于 18013cc88 返回 `e0000001`；非空参数按字节扫描到 NUL 后，通过 180006300 构造局部对象，再在 18013cd64 将三个局部对象地址传给 18013c9a0。不能把这三个对象的具体目录/文件名含义当作已证。

18013ce31 调用 180006470 更新全局对象；18013ce65 的调用目标 180046f10 接受全局地址 1802b12d0 与局部地址 `[rbp+7]`。返回 EAX 于 18013ce6f 保存至 EDI，日志及局部释放后 18013cf0a 恢复 EAX 返回。该段没有以该返回码区分成功后额外提交/失败回滚的显式分支。callee 内部是否写临时文件、刷新、替换、回滚完全未知；不能据 wrapper 推断事务性质。

SDK 的 180665ba0 与 ModuleFrame 导出之间 **尚未连通**，不能拼接成已确认的直接调用链。

## 剩余退出条件

取得两侧上述 callee 的原指令/可信反编译及跨模块实际绑定后，再追写入 API、缓冲区、文件/资源序列化布局、错误传播及 reader 对应字段；使用自有实例与受控目录进行单操作保存差分和往返。此次只解除「完全没有保存入口原指令」这一局部缺证据，不解除原生 provider、日志/运行隔离或原生格式验证门槛。

复核结果与哈希保存在 `analysis/full-audit/batch7-20261019/D01/review/`。tool-plugins 未读取或修改；未改产品、共享计划或探针，未构建或提交。
