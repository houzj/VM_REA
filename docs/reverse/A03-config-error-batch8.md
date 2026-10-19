# A03 第八批：配置错误产生点与异常返回链

2026-10-19；基线 `5e76aee`。本切片仅静态读取原程序集及既有运行记录，不运行 logger、SDK，不访问或写注册表、不改产品或 tool-plugins。有限目标已收口：错误串来源、异常吞没/返回路径和解除阻塞所需证据；原 logger 配置初始化仍 **blocked**。

## 身份与可复验产物

- 原件 `D:/Program Files/VisionMaster4.4.0/Applications/myLibs/Apps.BaseLog.dll`，SHA256 `AC114176CDFDB5A7478D1AFF4294EE006C842ACDC31FC104DCC92604FEF61F93`，MVID `41738887-4e20-451a-b9a9-ec8c9ef218a2`。
- REA `inspect_managed_artifact` Evidence `ev_9f63391503b604c1833699d81c3fa64fce08f5c253882a757dea6be8f6f272c6`，完整结果保存在本批 `A03/rea-base-identity.json`。确认 .NET Framework 4.5、CIL、System.Configuration 引用；此操作不执行原件。
- `tests/NativeEvidence/A03ConfigError/Run.ps1` 只编译/运行本批元数据导出器。导出器使用 `ReflectionOnlyLoadFrom`、MethodBody/ExceptionHandlingClauses，不调用原类型方法/构造器；请求件与实际元数据装载件 SHA 不等即退出。原件与两个本机 CLR 辅助程序集前后哈希见 `artifact-integrity.json`。精确 CIL、token、IL SHA、异常表见 `*.cil-disassembly.txt`/`*.cil-methods.json`。

## 原错误的两个准确产生点

1. `Apps.BaseLog.Util.SystemInfo.GetAppSetting`，token `060001E2`，IL SHA256 `b734425e1b7ad415fc79e4ddca7adebcee31dd0a89b0fc153d9977b13dbf3443`。IL_0000 调用 `System.Configuration.ConfigurationManager.get_AppSettings`，IL_0006 调用 `NameValueCollection.get_Item(key)`。try `[0,14)`、catch `System.Exception` `[14,33)`；catch 在 IL_0014 装入固定消息 **Exception while reading ConfigurationSettings. Check your .config file is well formed XML.**，IL_001A 调 `LogLog.Error(Type,String,Exception)`，随后 IL_0021～22 返回 null。消息中的 XML 提示是通用固定文本，不能由这句话证明 XML 损坏。
2. `Apps.BaseLog.Util.LogLog..cctor`，token `06000117`，IL SHA256 `c0438fb2790b3c36fe91065db2d8420298aa40797ff235dc5211b224e511298c`。try `[18,83)` 对 `Internal.Debug/Quiet/Emit` 三项调用 GetAppSetting；catch `System.Exception` `[83,107)` 用同一固定消息调用 Error，然后 ret。故单独的消息第一行不能区分这两个捕获点。

`SystemInfo..cctor` 还对 `Apps.BaseLog.NullText/NotAvailableText` 调用 GetAppSetting；null 路径保留默认 `(null)`/`NOT AVAILABLE`。这条静态容错分支不等于第七批实际运行已经继续到初始化完成。

## 原异常输出、吞没与外层中断

`LogLog.Error(Type,String,Exception)`，`06000127`，IL_0000 先检查 `!s_quietMode`；内部消息开启时，IL_0019 **先** `EmitErrorLine(prefix+message)`，再在 IL_0021～27 对非 null 异常执行 `ToString()` 并输出第二行/多行。最后 IL_0034 调 `OnLogReceived`；存在订阅者才调用事件，事件异常在该方法没有 catch。

`EmitErrorLine`，`06000129`，先 `Console.Error.WriteLine`，再 `Trace.WriteLine`；try `[0,19)` catch `System.Object` `[19,22)` 吞没自身输出错误。`GetAppSetting` 的 catch 不是对其内部 Error 调用再包一层 catch，因此不能声称任何输出/事件异常都必然吞没。

第七批 `run-9195d39c667c47958e7fb302d9323aa4` 的 `logger-observation.log` 只到实际程序集身份，`registry-proof.log` 只到 mapped-value，`stdout-stderr.log` **0 字节**。根线既有 tool 输出记录了 PowerShell NativeCommandError 的固定消息；原 runner `$ErrorActionPreference='Stop'` 且 `& Probe.exe *> stdout-stderr.log`，其后的 `$LASTEXITCODE`、integrity/hash不在 finally。现有证据不足以恢复真实 InnerException、Probe exit、异常输出的后续行，不能从外层 exit=1 推导原库退出码。此处不重新运行或改写第七批 runner。

## HKLM 映射与系统配置：已证实的静态依赖和未知

本次本机 CLR `System.Configuration.dll` 静态链为 `ConfigurationManager.AppSettings 06000235 → GetSection 06000237 → PrepareConfigSystem 06000233 → EnsureConfigurationSystem 06000230 / ClientConfigurationSystem`。客户端初始化读取 machine 与 complete config records 并 `ThrowIfInitErrors`；捕获后用 `ConfigurationErrorsException(message,inner)` 包装并缓存 `_initError/s_initError`，后续读取可再次抛出缓存错误。它读取的是应用/机器配置系统，不能把它等同于独立 `myLibs/Apps.BaseLog.config` 的 XmlConfigurator 解析。

本机配置路径还有条件链 `ClientConfigPaths..ctor → GetTypeAndHashSuffix 060000C5 → GetHash 060000C7 → new SHA1CryptoServiceProvider`；本机 `mscorlib` 中该构造器 `06002359` 调 `Utils.StaticProvHandle 060023C1`，首次 provider 获取用 `CspParameters(24)` → `AcquireProvHandle 060023C3 → _AcquireCSP 06002406`（InternalCall，无可导出托管方法体）。这说明只预载两个日志 DLL **没有静态证明**系统配置/密码提供者依赖已预初始化。它是全 HKLM 映射风险的具体候选依赖链，不是第七批错误原因的运行证明。

上述系统程序集是本机只读辅助证据，哈希/MVID/实际 ReflectionOnly 位置单列在本批 JSON；旧失败子进程未完成系统程序集清单，因此不宣称它当时逐个绑定了本批检查的相同系统件。未恢复 `_AcquireCSP` native 实现、未证明原错误走到 SHA1 分支，也未检查或复制任何宿主 CSP 注册表。第七批此前 `0x80090017` 是另一个已知 LoadFrom 失败，不得移植为最终配置错误的 InnerException。

## 退出与下一解除条件

源码错误边界已静态确定；真实最内层异常、文件名/行号、HRESULT、完整堆栈仍缺。**当前整体状态 blocked，不再扩框架反编译或重跑。** 下一次如根线单独选择有界取证，必须先有不会因 stderr 首行中断的独立 stdout/stderr 全量捕获、实际子进程退出码、父进程 finally 审计与精确自有键清理证据，并证明相应系统配置/CSP依赖在隔离内可用。优先独立受控 Windows 环境；不以复制大段 HKLM、关闭强名、替换配置或猜测 XML 修复来解除阻塞。本报告不是新运行授权或可直接执行的旁路方案。

A03.3 维持源码 done、实现 partial、验证 blocked；新增有限静态链不增加 96 单元分子。主框架总完成率不因本切片变化。

## 失败与范围保留

- REA inspect_managed_members 两次传 `type_name`/`type_filter` 均参数校验拒绝；没有取得方法证据，不冒称成功；改为原 CIL 元数据导出。
- 首次直接 csc 相对路径因编译器参数解释失败，未运行导出器/原件；修为绝对路径后成功。完整尝试说明在本批 `attempts.json`。
- 复用了第五批已知 cctor 仅作调用上下文，本批新焦点是 GetAppSetting/Error/异常表与系统配置边界，不把同一 cctor 重复计数。没有新增原厂运行、产品改动或手验结论。
