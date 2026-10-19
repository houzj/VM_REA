# B05.3 gvar 源码前置审查（第七批）

基线：`39a54c8`。这是总计划允许的提前源码调查，B01/B04 未关闭，未集成或关闭 B05。固定验收单元仍为 B05.3，不新增分母。**结论：源码 partial 保持，未达到 done；实现未交付、验证阻塞保持。**

## 有限问题与退出结论

本次只回答导入/导出入口、路径载荷、变量重建、SDK 边界与格式权威，不扩展整个变量服务。托管层并不写 gvar 文件；它把路径传给原生参数消费者。原生 `GlobalValueImport` / `GlobalValueExport` 消费函数及文件 reader/writer 未取得，格式/覆盖/密码/磁盘事务不能以托管入口代替。达到该缺体边界即停止，没有启动 SDK、真实实例或文件导入导出。

## 原源码链

原始证据均在 `analysis/decompiled/`：

| 源路径 / 成员 | 确认行为 |
|---|---|
| `VMControls.WPF/VMControls.WPF.GlobalUI/GlobalVariableDialog.xaml.cs:549,1578–1753`：装饰源、BtnImport_Click、ImportGlobalVar、BtnExport_Click、ExportGlobalVar | `VariableManager(DecorateSource)`；本地导入选已有 `.gvar`，Items 非空请求清空确认；FileInfo.Length 被读取但未用作格式校验。异步导入先按旧 Name 收集运行参数绑定；传入逆序 Dispose Outputs 的 UI 回调。成功后清旧 UI 并重读变量/组。导出要求有源且 Items 非空。nRet 初始 0 未被 SDK 返回赋值，不能把 UI 成功消息当实际往返证据。 |
| 同文件 `1990–2071`：远程导入/导出入口 | 本地上传到固定 `/root/vmtempfiles/ftp/ModelFiles/temp.gvar` 时 `bCreatDir=true,bCover=true`；导出下载 `bCover=true` 后检查本地存在，成功移除远端临时文件。该 FTP 覆盖选项不能外推本地 native gvar 覆盖规则。 |
| `Apps.XmlParser.CapabilityAudit/Apps.XmlParser.Variable/VariableManager.cs:1566–1634`：ExportVar、ImportVar | 路径空/null 抛 VmException(-536870911)；路径 `UTF8GetBytesPadZero` 写 DataSource.Params 的 `GlobalValueExport` / `GlobalValueImport`。无格式、密码、扩展名解析；不是 managed XML/JSON 序列化。导入 native 写入先于旧输出解绑和重建。 |
| 同文件 `1024–`：GetAllVar 两重载 | `VarCount` 可解析且 >0 才取 `GlobalValueList`；长度须整除 IMVS_VARIABLE 大小。读取 Name/Type/Index/Guid/GroupInfo 等 SDK 字段，数组值涉及原生指针复制。Guid 是读取值，不证明导入保留或重生成 GUID 的规则。 |
| `VM.Core/VM.Core/ModuleParam.cs:55–111`：IUserBytesData setter | 已 Dispose 则直接返回；包含模型时注册模型历史。普通键走 ModuleManager.SetBinaryData(Parent.ID,key,value,TimeOut)，非零抛异常。此处不序列化文件。 |
| `VM.PlatformSDKCS.Audit/VM.PlatformSDKCS/ModuleManager.cs:1564–1596`、ServerSDKAPI.IMVS_SetBinaryData、NativeMethods.IMVS_SetBinaryData | 名称固定 256 UTF8 字节，byte[] 分配 HGlobal、Copy、原调用、FreeHGlobal；普通返回非零仍释放，但非 finally，调用异常时释放无保证。P/Invoke 声明 `.\\iMVS-6000PlatformSDK.dll`、StdCall；声明不是 native export/函数体运行验证。 |

ImportVar 对 CustomVisibleA 旧输出逆序解绑，解绑异常记录后继续；ChangeToSendReceiveData 后按旧输出 Name 恢复提供的 runParaminfo。其默认 runParaminfo=null，但该分支直接 ContainsKey；UI 路径提供字典，不能据此宣称全部调用安全。之后调用 delIOAct；UI 路径清静态 ModuleVarDict，GetAllVar 并 AddIoByType，以新数据 Name/Type/组合叶名建输出，再设 Index。类型不支持会在 native import 之后抛出，未看到一体回滚。变量附着 DataSource 模块，不应混为已有局部变量实现已支持 gvar。

## 原件与 CIL 审计

`tests/NativeEvidence/B05Gvar/Run.ps1` 使用 .NET Framework csc 构建自有元数据导出器；ReflectionOnlyLoadFrom，不调用目标构造、方法或属性。导出9个原方法体：VariableManager 5（含 GetAllVar 双重载）、ModuleParam 字节 setter 1、SDK SetBinaryData 双重载及转发 3。包括 IL 字节摘要、token、异常表、操作数和原件 MVID/SHA256。

输出：`analysis/full-audit/batch7-20261019/B05/run-77efb7b589ec4b1ca3c6a7e69d27a961` 与 `run-a1eb26d62f3d4dfcafdfa301abf51e50`。首次显式接口方法名匹配为0，原日志保留；第二次按元数据后缀修正为1，不能把首次0当完成。以包含9体的 run 作为最终证据，具体各 JSON 记录方法数。三个安装原件前后 SHA256 相等。

REA 原生边界证据 `ev_3d164ee62a60810882e6c7885ea294d96fd5dc0b2cef93878d1aab1c2a296d5f` 落盘 `rea-native-boundaries.json`，其 native import 是 managed declaration only。没有重复失败的 native provider 初始化或大范围方法库存；未取得 native 参数处理体的事实保留。

## 仍缺、解除条件与进度

1. 取得同版本 native 参数键处理 → gvar 文件读写函数体，证明文件头、版本、字段/变量 GUID、组与作用域、覆盖与密码规则；否则源码仍 partial。
2. 取得受控自有实例隔离条件后，运行导入前后变量及订阅身份、错误码、损坏文件、重复名、导出重导入；检查原 native 文件往返。不得对安装实例直接试验。
3. 产品交付须等待 B01/B04 前置和所属主框架合同，不以 JSON 改名 gvar；本批无产品补丁或运行通过数。

本次新增结果是排除托管格式假设并定位精确 native 缺体，不产生验收项完成数。B05.3 源码/实现/验证三层增量均为0，96项分母不变。tool-plugins 未读取或修改；无构建主产品、提交或推送。
