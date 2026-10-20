# B3：复位返回错误链与 Proxy 加载绑定

2026-10-19。接续 [B3 reset/cache 切片](CAMERA-MAPPING-B3-RESET-SOURCE.md)，复用其31个原生体。本切片只读取原厂二进制、资源、托管反编译和 PE 元数据；未运行原厂复位、未触碰验收进程、未构建或修改产品。

**静态结论：在正常 SP 路径、使用原厂七项默认 XML 且响应正常生成/解析的条件下，空 CalibPathName 写入的 0xe0000204 会作为最后非零错误一路到 UI catch。它不会因为 Proxy 传输/JSON 序列化成功而变为成功；之前清的绑定和已写参数不回滚，成功刷新不执行。** 这不是一次实际 UI 观察记录。

## 从最后非零到 UI catch

| 层/实际位置 | 错误码如何传递 |
|---|---|
| CameraMap.SetParam RVA0xce30 | CalibPathName 空字符串或长度0，在改路径前返回0xe0000204；默认后续项仍可写入 |
| ModuleFrame RVA0x42760 | 每项经算法 vtable+0x18 调用；0x429c9保存返回，非零才在0x42a0a更新EBP，继续循环，0x42a45返回EBP，即最后非零 |
| ModuleFrame 0x581e0 / export0x141ee0 | 分别在0x58523保存EAX、0x5856c恢复EAX；export也保存调用结果并在0x141faa恢复EAX |
| ModuleProxy helper0x24a30 | JMP [this+0x398]，目标为 VM_M_ResetModuleNodeAlgParam；直接保留返回约定 |
| ModuleProxy JSON case0xc0b | 0x556bd调用helper，0x556c2保存EAX；非零在0x556fd写响应结构的errorCode（结构+0x48），0x55749交0x61a60构造响应 |
| ModuleProxy response0x61a60 | 将结构[0x11]写head.responseId，[0x12]写head.errorCode；这是模块状态。其自身返回值表示响应构造是否成功，与嵌入的模块错误码不同 |
| Proxy0x49cb0 / VM_PROXY_RecvServerCommand | 正常响应按容量复制；容量不足0xe0000012供外层扩容重试。0x49daf返回构造/传输状态；模块错误仍在JSON中 |
| PlatformManager SPSendCmdPairMessage0x5de710 | 校验head.command、responseId、errorCode类型；responseId匹配请求序号后，0x5deeba读errorCode，0x5defad将它送ESI；最终0x5df14c返回ESI。false参数只控制成功时是否复制完整JSON，不吞非零errorCode |
| FilterNodeManagerForSP0x5ee170 | 保存上一步EAX，0x5ee30f恢复；0x62b600据此生成OK或Err*%d，并写文本响应长度 |
| PlatformManager ClientMsgDeal0x602df0 / PF_M_MsgDeal | case0x419把0x62b600返回保存并走公共清理返回；导出继续转交该结果 |
| SDK SendCommand0x6c00d0 | 首先保留PlatformManager调用的非零返回；若调用成功但文本非OK，则按`%[^*]*%d`解析Err*code。0x6c0412取解析整数、0x6c0424返回；解析失败为0xe0000108 |
| SDK SM_SetDefaultParamList0x6b7970 | 0x6b7a3f先把SendCommand结果存栈+0x30；非零走C++抛出入口。PE unwind/EH3证明本函数有catch(...)，不是任错误越过P/Invoke |
| SDK catch funclet0x9f85a0 | 返回续执行地址0x6b7ad1；该地址`MOV EAX,[RSP+0x30]`恢复原错误码。0x64a2e0 export保存该EAX并返回；export自身没有EH handler |
| ModuleManager.SetDefaultParamList | 非零构造并抛VmException(num)，errorCode保留int原位值 |
| VmModule.ResetParam | catch VmException，转抛Exception(ErrorCodeInfo.GetErrorInfo(ex.errorCode)) |
| Button_ResetAlgoParams.ClickCommandCmdExecute | catch Exception，GetLang(ex.Message)、显示错误、记录日志、return；不进入后面的_IsResetAlgoParamsing/GetItemsValueUnit/成功提示 |

Apps.ErrorCode.ErrorCodeInfo.GetErrorInfo(int) 原方法 token **0x06000009**、RVA **0x2204**，从语言ErrorCode资源字典按uint错误码取文本。安装 `ErrorCode/zh-cn.xaml:2908` 将0xe0000204映射为“文件格式错误”，en-us同码为“File Format Error”；当前UI语言/实际对话框没有运行确认。找不到错误码或资源加载失败另有源确认回退，不能硬编码显示文本。

### 容易被反编译隐藏的 SDK catch

PE `.pdata` 将0x6b7970归属到[0x6b7970,0x6b7af6)，UnwindRVA0xb957f0、flags3、FuncInfoRVA0xb11ad0；EH3 magic0x19930522，tryLow/high=0，catchHigh=1，adjectives64/typeRVA0，catch(...) funcletRVA0x9f85a0。funclet不是普通显式call边，因此只看C伪码会漏掉。funclet返回0x6b7ad1，从原保存槽恢复错误码；普通成功分支在0x6b7acd清EAX并跳过该恢复点。`platform-eh.json`、funclet原体及两段原指令共同支持这个结论。

## manager+0x7d510 实际绑定

0x5d3570 初始化 FilterNodeManagerForSP 时先调用0x5d3f00准备配置。0x5d3f00读取 `ProxyPath`；缺省为 `../ModuleProxy/x64/ModuleProxy`，并加`.exe`存manager+0x7d438。原安装 `Applications/Server/ServerConfiguration.ini:44` 配置同值。不能把它误写成算法XML路径。

随后0x5d3570调用0x63bc40(manager+0x7d510,manager+0x7d438)。0x63bc40原日志名CLoadProxyDll::LoadDll：截取`.exe`之前部分、转换/解析路径、追加宽字符`.dll`，LoadLibraryW的返回句柄写对象首字段，即manager+0x7d510。0x5de710从相同字段取handle，GetProcAddress `VM_PROXY_RecvServerCommand`并缓存manager+0x7d518，再直接调用。由此默认配置绑定到已审计ModuleProxy.dll的匹配导出，而不是仅凭同名字符串猜目标。

这条SP路径的SDK→PlatformManager→ModuleProxy→ModuleFrame边界为DLL加载和函数调用，JSON本身不证明跨进程。自定义ProxyPath可以改变目标；本切片没有执行实际加载来读取最终绝对路径，也不将SP链结论扩到其他运行模式。

## 验证、交接与剩余范围

新报告索引：[CAMERA-MAPPING-B3-RETURN-SOURCE.json](CAMERA-MAPPING-B3-RETURN-SOURCE.json)。新产物目录 `analysis/camera-mapping/b3-return-source-20261019`；旧31体归档及其索引保持原内容。新工具位于 tests/CameraMapping.NativeProbe，使用私有Ghidra设置/缓存、被动PEReader和ILSpy；没有加载托管目标或执行native目标。指令窗中的正常返回和异常续执行分别核对，保留首次跨尾部数据解码失败日志，修正边界后再导出，不把工具退出0当完整窗口成功。

本切片静态闭合了指定错误链与默认Proxy绑定。实际成功Process→用户复位→后续保存/运行/显示/最后帧尚未执行；数组索引、模式和文件其他生命周期仍属B3后续范围。本轮不先改atomic复位适配。B1/B2各S0–S5/S7、B3/B4各仅S0，仍为 **16/32阶段（相机线50%）**，向总128阶段贡献16；局部源码增量不充作整卡新阶段。
