# B05 第九批：二进制参数跨模块绑定

基线 `f11bc13d413ea82fb074c0c00b3b2f2dffad01ad`。提前只读前置调查，B01/B04未关闭，不集成。复用原REA native provider缺配置记录，使用原PE表/字节和已有MSVC dumpbin，不运行LoadLibrary/SDK/原目标。

## 原件与范围

| 原件（VisionMaster4.4.0根） | SHA256 |
|---|---|
| Applications/PublicFile/x64/iMVS-6000PlatformSDK.dll | b62d7e15588ccda6671228aebc11fec3e27aca878f0e03067e44b17d5a54a9a1 |
| Applications/Server/PlatformManager.dll | 06d86f51bebfeba357d1b79297e20a3b0a70580a40af7b77c8abdd6c38ca0624 |

所有取证探针在 `tests/NativeEvidence/B05BinaryBinding/`。FindCandidates.py仅扫描SDK可执行原始节中LE32位移23a8h，得到11个字节候选，明确不是11个交叉引用；其中有分支相对位移/无pdata候选，未按字节匹配认定对象访问。由候选选择完整6615b0、661d00函数，再用原寄存器流和导入表确认赋值。

最终SDK指令 `analysis/full-audit/batch9-20261019/B05/instructions-15b9673441ce468283b92be9ef55fe5d/`：6615b0–661cfd（408行）、661d00–661e4c（71行）。Platform最终 `instructions-9618e174e5bc44ecaeeedd0639971047/`：PF_M_SetBinaryData 6a0320–6a051f（132行）、下一callee5f62e0（270行）。原件前后hash相同，原段bytes和打印前缀复核479/402行通过；不是独立解码器交叉验证。首次平台仅导出132行的产物保留，不重复累计。

## SDK：精确装载来源和字段赋值

initializer日志身份为 `CServerManager::SM_SP_LoadLib`。同一LoadLibraryW返回句柄存对象+2368h，之后GetProcAddress以此句柄取多个函数。VA180661b48引用ASCII `PF_M_SetBinaryData`（RVAac4198），180661b59调用IAT180a391c0，导入表确认为KERNEL32!GetProcAddress；180661b5f将返回RAX写入对象+23a8h。第八批66f090正从该字段调用0bd2h二进制消息，静态跨模块绑定由此连通。

相邻+2398h取PF_M_MsgDeal，单独由D线追踪，不能与+23a8h混为同一协议。原clear661d00于180661d64清ESI=0，180661d97写+23a8h=0；同区清多个函数槽，后续句柄释放见原指令，不把字段归零当完整并发安全证明。

DLL构造引用UTF16 `\\..\\..\\Server\\`、`PlatformManager.dll`；与SDK目录结合对应安装Applications/Server原件。路径helper的GetModuleHandleW/GetModuleFileNameW/PathRemoveFileSpecW由D线独立取证，见其[报告](D01-save-binding-batch9.md)。本批未观察实际Windows装载成功或运行时搜索覆盖。首次字符串读取把wide当ASCII只得前缀，已保留first-ascii-prefix-only输出，修为明确编码后重取；不以首字母推DLL名。

## 平台：0bd2h消费者

原PE导出表确认PlatformManager!PF_M_SetBinaryData RVA6a0320、ordinal5，非forwarder。入口按减法分派；0bd2h路径依次减423h、0ah、45h、2后等75eh，跳1806a03b5，最终1806a03e9调用1805f62e0；未知消息返回e0000007。成功分支还写全局字节180976710=1，其含义不以“dirty”猜名。

5f62e0日志字符串为 `FilterNodeManagerForSP::SPSetBinaryData`。入口保存原数据指针R8与R9长度，拒绝两者为0；从输入命令以09字节分隔，要求结果向量存储跨度掩码后40h（两个20h记录），原split helper未完整还原。之后调用66cd10构造新的命令，输出字符串非空才继续。结构中存原数据指针、长度；对象+7d518h缓存的间接目标为空时，利用对象+7d510h句柄GetProcAddress查询 **VM_PROXY_RecvServerCommand**（RVA7e3ba8），查不到返回e000000a；1805f65b9调用该指针。该句柄的实际模块赋值及66cd10命令编码尚缺，不能把代理名当已确认具体ModuleProxy DLL。

因此本批从托管路径载荷连到了SDK→PlatformManager→代理接口名；仍未到GlobalValueImport/Export真正文件消费者。gvar头/版本/GUID重建/覆盖/密码/事务与实际往返仍未知。下一问题仅需定位+7d510h句柄来源与VM_PROXY_RecvServerCommand实际实现，不能再重复这批函数导出。

## 退出与进度

有限绑定问题已完成，B05.3源码partial保持；实现未交付、验证blocked。96项完成分子增加0。无产品修改、目标执行、原厂差分、tool-plugins访问。完整字节、导入、常量、候选及SHA保存在本批B05审计目录；失败与修正不删除。
