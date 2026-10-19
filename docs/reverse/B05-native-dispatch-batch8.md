# B05 第八批：原生二进制参数分派

基线 `5e76aee5546a29243c9d95c17d29c2e73080bceb`；仅提前只读源码调查，B01/B04依赖保持开放，不集成、不关闭B05。上一批停在P/Invoke；本批取得真实导出及下一层，未执行任何目标。

## 原件和范围

`D:/Program Files/VisionMaster4.4.0/Applications/PublicFile/x64/iMVS-6000PlatformSDK.dll` SHA256 `b62d7e15588ccda6671228aebc11fec3e27aca878f0e03067e44b17d5a54a9a1`，前后相同。已有REA native provider缺配置证据复用，不重复setup；以下是原PE字节与已有MSVC dumpbin指令，不冒充REA反编译。

探针 `tests/NativeEvidence/B05NativeDispatch/InspectBinaryDispatch.py` 按原PE导出名解析入口，按完整pdata/CHAININFO tuple取范围；内部callee由入口的直接call选定。最终输出 `analysis/full-audit/batch8-20261019/B05/instructions-abc22ca0df7e4a1782356f1e05017828/`。前两次输出保留，分别增加内部callee及直接常量读取，不把重复导出计多次完成。

| 成员 | RVA范围，右端不含 | 指令 |
|---|---|---:|
| IMVS_SetBinaryData | 6471f0–6472ff | 80 |
| 内部callee，未命名原符号 | 66f090–66f2fe | 142 |

## 确认的数据流

原托管声明在 `analysis/decompiled/VM.PlatformSDKCS.Audit/VM.PlatformSDKCS/NativeMethods.cs`：handle、uint模块ID、ref IMVS_SET_BINARY_DATA_INFO、uint等待时间。`ImvsSdkDefine.cs:764` 字段依次为256字节Name、IntPtr数据、uint长度及4个reserved。对应原生读偏移100h数据指针、108h长度；不是根据截图或格式猜测。

导出入口保存EDX模块ID、R8结构体指针、R9D等待值；查handle匹配节点，缺失/失效返回e0000006。VA1806472e3向18066f090传RCX全局对象180ca02e0、EDX模块ID、R8原结构、R9D原等待值。返回EAX经EBX保存后返回；helper180001070/10b0含义未确认，不擅自命名锁。

内部66f090先检查结构指针非空；模块ID以**有符号jg**相对-1判断，不能说所有uint都合法；失败返回e0000001。随后用模块ID和结构首地址构造命令缓冲区，格式常量RVA ac7c14为 `%u\t%s`（中间实际09字节）；日志常量RVA ac7cb0为 `CServerManager::SM_SetBinaryData`。原生未在该函数内根据GlobalValueImport/Export文本分支。

VA18066f1fd读取对象+23a8h函数指针。存在时，18066f22d间接call参数为ECX=0bd2h、RDX栈命令字符串、R8=结构+100h数据指针、R9D=结构+108h长度，栈第5/6参均0。原入口等待时间在该可见间接调用未转发（在日志参数使用），不外推整个API超时语义。函数指针空则e0000010；间接调用非零返回保留原错误，成功返回0。

这把gvar的托管UTF8+NUL路径载荷推进到native二进制消息回调边界；并未取得回调实际实现，不能称gvar文件writer/reader已找到。

## 搜索边界与未知

对既有同哈希SDK和ModuleFrame两原件的ASCII/UTF16LE精确GlobalValueImport/GlobalValueExport搜索均无匹配，保存在key-literals.json。该有限阴性不代表全部模块没有实现，不以字符串缺失猜测加密或不存在。

直接引用常量原字节保存在direct-constants.json；间接格式化IAT函数名称尚未解析。下一退出条件是对象+23a8h函数指针的注册写入点、实际DLL/导出和0bd2h消费者；不能再次导出本批两函数当进展。当前gvar格式、覆盖、GUID重建、密码、事务、实际导出重导入仍未知。B05.3源码partial、实现未交付、验证blocked不变，96项完成分子增加0。无产品改动、SDK运行或tool-plugins访问。
