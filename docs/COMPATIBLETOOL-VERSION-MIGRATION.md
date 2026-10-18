# CompatibleTool 版本迁移逆向记录

更新：2026-10-18。范围是 VM 4.x 安装版 `CompatibleTool` 的托管迁移调度和 WPF 主程序调用链。证据为反编译重建源码的静态分析；未运行原版迁移器，也没有旧版 `.sol/.prc/.gro` 控制样本。因此本文确认的是托管调用和转换步骤，不确认原生容器格式、迁移成功率或输出可重开。

## 证据与定位

| 证据 | 位置 | SHA-256 | 可确认范围 |
|---|---|---|---|
| 主程序版本检查及隐藏子进程启动 | `analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster.Project/ProjectManager.cs:604-700` | `1ae41a0841dd65f5dfa7461078c370822e61826877c42e299939af1fd47f9370` | 查询原生方案版本、低版本提示、输出路径冲突处理、启动参数、等待进程和输出文件存在性检查 |
| 迁移工具入口 | `analysis/decompiled/CompatibleTool.Audit/CompatibleTool/Program.cs:10-46` | `796184f7cbe03c2c9c267873b0992e1cc4b1574ff6f46814bf6285288aca2bb7` | 参数组合、四类迁移入口、异常捕获、返回码与 SDK 释放 |
| 方案/流程/Group/设计转换 | `analysis/decompiled/CompatibleTool.Audit/CompatibleTool/ConvertHelper.cs:131-585` | `e402795ab8954269b1886274786f7734afc0970abfbeb53c098701831c53ab35` | 版本分段、迁移步骤、保存入口和原生 SDK 边界 |
| 动态 IO 迁移规则 | `analysis/decompiled/CompatibleTool.Audit/CompatibleTool/DynamicIOUtils.cs:33-520` | `7e823cdab9ecfd0b5665e6ba2b2ecbd2b7851bdd5026430e7882cef343309515` | V4.2、V4.3、V4.4 的部分工具级动态输入输出改写 |
| IsReady 重建 | `analysis/decompiled/CompatibleTool.Audit/CompatibleTool/CompatIsReadyPropertyUtils.cs:15-247` | `41bf41e9727bf41405cb12ac5b1d9dccac1142b4efa6f583d2569aa48a948894` | 递归检查流程/Group模块的动态输入是否就绪，并写 `IsReady`、`UnReadyModuleNames` UI 数据 |
| 模板匹配模型迁移 | `analysis/decompiled/CompatibleTool.Audit/CompatibleTool/TempMatchUtils.cs:21-620` | `701d2b8f1c49357ce621ae19a9ae60e1e642fea68d3e8afb94c372b7cd0b85ff` | 对明确列出的匹配/检测模块调用其专属模型转换控件和模型数据构建逻辑 |
| 全局变量/绑定迁移 | `analysis/decompiled/CompatibleTool.Audit/CompatibleTool/GlobalVarUtils.cs:53-453` | `5b8a918bbfd387ddba0ceb47050c9efb885161d0038c5bef7ded6ffd0bbb826f` | 读取全局变量与模块绑定，更新参数名/type/index并分批重建绑定 |
| 定制模块迁移 | `analysis/decompiled/CompatibleTool.Audit/CompatibleTool/CustomizedModuleUtils.cs:13-154` | `0745a14106c9ed8a0a869a6dcaf4fbd2993a7bcaa6b9bae18889528a8fca056a` | 扫描对象模块并分发专属 `CustomizedCompatible`；托管枚举中仅见 `TestModu` |
| 前端设计迁移 | `analysis/decompiled/CompatibleTool.Audit/CompatibleTool/FrontendUIUtils.cs:44-402` | `dde697e974174d6bbe35535d90d6f17e63c44dfcb71b909959fc7c4e9d54c362` | 设计器缓冲区读取、XAML/XML 改写、版本属性及布局转换 |
| 序列化调用边界 | `analysis/full-audit/solution-serialization-contract.json` 中 `save`、`load`、`version-migration` 链 | 报告记录了托管文件与 SDK 头文件哈希 | 方案读写由 SDK/native 承担；迁移器转换规则和原生容器仍有未知项 |

### SDK 托管边界的进一步确认（静态）

本轮对反编译的 `VM.PlatformSDKCS` 与 `VM.Core` 再核对，确认托管层没有实现 `.sol` 字节格式的读写器：`VmSolution` 最终把读写委托给 `SolutionManager.SyncSaveSolution/SyncLoadSolution`，后者再调用 `ServerSDKAPI.IMVS_SaveSolution/IMVS_LoadSolution`。常规 `SaveAs` 路径传给保存 API 的附加缓冲区是 `IntPtr.Zero`、长度 `0`；保存/加载调用返回非零即转成 `VmException`。同步 Save/Load 还会等待 SDK 回调；Load 在 SDK 模式下另检查异步加载错误码。方案版本查询同样通过 `IMVS_GetSolutionVersion` 返回原生结构中的 `strTargetSolutionInfo`。

| 反编译证据 | SHA-256 | 本轮静态确认 | 证据限制 |
|---|---|---|---|
| `analysis/decompiled/VM.Core/VM.Core/VmSolution.cs`（保存与加载委托） | `C141E2C53913B233B768782D6757FAE5997ECAF638B35DB2DDF98F5EF951A13E` | SaveAs 委托 base 的保存路径；底层同步保存传零指针/零长度；Load 委托 SDK 并经过回调/异常处理 | 反编译重建源码，不是原始 C#；不能推出容器布局和原生事务语义 |
| `analysis/decompiled/VM.PlatformSDKCS.Audit/VM.PlatformSDKCS/SolutionManager.cs`（SDK 包装） | `46B1D8249E3A30FE3BF7A0ED8E92207835F1A8E272833D5462041BC1756F220A` | Save/Load/version 三个调用的错误码、等待回调和异步错误检查 | 仅描述托管包装器行为 |
| `analysis/decompiled/VM.PlatformSDKCS.Audit/VM.PlatformSDKCS/NativeMethods.cs`（P/Invoke 声明） | `6A60F612FB385987FF96D6973844A256402F097D1EB371E7AD462C44E57127D0` | 声明了 `IMVS_SaveSolution(handle,path,password,pData,nDataLen)`、`IMVS_LoadSolution(handle,path,password,checkFlag,silentFlag)` 及版本查询入口；DLL 名为 `iMVS-6000PlatformSDK.dll` | P/Invoke 声明是托管接口证据，不等于 native ABI/实现反汇编证据；调用约定、内部容器和字段未知 |
| 安装版 SDK C 头 `Development/V4.x/Includes/iMVS-6000PlatformSDKC.h` 与 `iMVS-6000PlatformSDKDefine.h` | 分别为 `2839B317C7F40E0791ADE924B2DBF1B6FB49E8CFCCD0BBE88B648C8C95331B2B`、`4F1D84AE3EB0C150AADF2682CFFE05B6CECF113B99329C64789CEFF3A57EE769` | 公开的 V4.x C 接口声明 `__stdcall IMVS_PF_SaveSolution`、`IMVS_PF_SyncSaveSolution`、`IMVS_PF_LoadSolution` 和版本查询；保存输入结构明示路径、密码、`pAdditionData` 与 `nDataLen`，结构另有保留字段；头文件也声明 Save/Load 进度查询 | SDK 头是接口/字段定义证据，不包含其函数体；`IMVS_PF_*` 公共接口与托管 `IMVS_*` 入口的内部映射关系未由此证明；容器编码/压缩/加密/资源布局仍未知 |

安装目录还带有 SDK 样例源码，但它没有暴露 `.sol` 容器实现。本轮尝试使用 REA 打开已定位的 x64 SDK 二进制时，深度分析不可用：Ghidra 未配置，IDA 未注册连接，Hopper 不支持当前 Windows 主机。故目前比“只知托管包装”多确认了 V4.x 公开 C 接口的调用约定与保存数据结构，但仍没有获得 native 函数体；不能把接口声明写成原生序列化算法结论。

### 安装版 V4.4 `.sol` 样本容器观察（静态）

对安装包附带的 SDK 样本做只读头部与目录解析，没有调用 VM/SDK。三份方案样本均以 ZIP local-file signature `50 4B 03 04` 开头：

| 样本 | SHA-256 | 观察 |
|---|---|---|
| `Development/V4.x/Samples/VC/PlatformSDKSampleVC/CircleFind/CircleFind.sol` | `B91A7736F467757E15A55AF7836E22364AE153478E17C69AAEB69E0C30674D0B` | .NET ZIP 目录读取成功；含 `SolutionFile/VmServer.xml`、`MoudleFrame`、`UiParamData/*` 和 `ModuleAttachments/*.bmp` |
| `Development/V4.x/Samples/VC/PlatformSDKSampleVC/GetResultControl/GetResultSample.sol` | `8B30CA25747F0F7254E44D0B209A1EE5C9F0348628BA1579FC8D3647815767D2` | 文件头同为 `50 4B 03 04` |
| `Development/V4.x/Samples/C#/ApplicationDemo/OCRDemoCs/OCRDemoEnglish.sol` | `E4B4BD7576EF2DA6D55558ACD1DB0D557FFB86C9F590D8830A94375D47DAD462` | 文件头同为 `50 4B 03 04` |

`CircleFind.sol` 的 `SolutionFile/VmServer.xml` 可读 XML，包含版本 `V4.4.0`、方案名、模块及流程列表、连线/订阅关系；`UiParamData` 条目解压后是 VM 二进制记录（以 `fU3"` 等字节起始），不应作为普通 XML 解析。目录内单独存放 `MoudleFrame` 与图像附件，说明方案包不只是单一 XML 文件。该样本证明的是“这些未加密 SDK 样本能按 ZIP 目录读取”，不是所有 `.sol` 版本/密码模式都用相同封装，更没有证明 SDK 保存时的压缩参数、加密、校验、写入顺序、未知条目保留策略或旧版本迁移往返。

对本机 `D:\Program Files\VisionMaster4.4.0` 安装目录递归列出的 11 个 `.sol` 样本逐个读取 `SolutionFile/VmServer.xml` 版本节点，观察结果全为 `V4.4.0`。因此当前机器提供了 V4.4 同版本样本证据，但没有安装版旧版本 `.sol` 样本可做迁移前后差分；数量和版本只代表本次安装快照。

## 迁移调用链

1. `ProjectManager` 调用 `IMVS_GetSolutionVersion`，对返回的 `strTargetSolutionInfo` 作字符串版本比较。低于本机 `Lang_version` 时，显示升级遮罩，并在原方案同目录选择 `原名_目标版本.sol`；文件已存在时依次尝试带序号的新名字，不覆盖旧迁移输出。
2. 主程序以隐藏、无 Shell、重定向标准输出/错误的子进程启动同目录 `CompatibleTool.exe`，传入 `VMSolution;源路径;密码;目标路径;目标版本`，等待退出，再检查目标文件是否存在。主程序不以子进程退出码作为唯一成功条件。
3. 工具先调用 `IMVS_GetSolutionVersion`、`IMVS_GetPrcVersion` 或 `IMVS_GetGroVersion` 取得文件版本，再由 `VmSolution.Load`、流程导入 API 或 `IMVSGroupTool.LoadIndependentGroup` 加载原对象。它设置 SDK 的版本升级标志，对对象作原生托管对象操作，最后调用 `VmSolution.SaveAs`、`SyncExportProcess` 或 `ExportGroup` 写出。
4. 方案从 V4.0/V4.1 到 V4.4 的托管步骤包括：IsReady 数据、动态 IO、模板匹配模型、前端设计、定制模块；V4.2→V4.3 处理动态 IO；V4.3→V4.4 更新前端设计、动态 IO、全局变量和定制模块。V4.4 的动态 IO步骤还会更新特定模块，并对原生模块类型 `0` 清空 `PathPoints` 用户数据。
5. V4.3→V4.4 的设计文档转换有 XML 层实现；V4.0→V4.4 的设计 XML 路径经过 VM4.2、VM4.3/4.4 两个转换工具。以上只覆盖显式托管步骤，插件模型、附件、ROI 持久化和二进制容器仍由 SDK/插件层负责。

## 转换器职责拆分

| 子系统 | 反编译可见的具体行为 | 不能由此推出的结论 |
|---|---|---|
| `CompatIsReadyPropertyUtils` | 读取流程/Group 的首层模块树；递归处理嵌套 `IMVSGroup`；按模块 XML 中的动态 IO 树解析 readiness；向对象 UI 数据写入未就绪模块名和 `IsReady` 字节 | 这不是算法校验；未证明缺 XML、缺插件或 SDK 报错时状态完整 |
| `DynamicIOUtils` | V4.2 路径扫描动态输入/输出用户数据并更新 IO；也处理 GlobalVariableModule。V4.3 路径针对 `IMVSTargetTrackModu` 更新动态 IO，并把订阅名 `roicenterx/roicentery/roiwidth/roiheight/roiangle` 改为 `BoxCenterx/BoxCentery/BoxWidth/BoxHeight/BoxAngle`。V4.4 路径应用特殊模块表，并删除 type 0 节点的 `PathPoints` UI blob | 未涵盖全部插件所有版本；迁移后的每个新参数/模型含义需工具线逐插件差分 |
| `TempMatchUtils` | 按模块名处理 HP/快速特征匹配、灰度匹配、MarkFind、Inspect、MarkInsp 等模型；部分通过加载插件 DLL、反射专属迁移控件并重建模型 | 不能把模型迁移泛化到全工具集；真实模型字节与原厂可运行结果仍待对照 |
| `FrontendUIUtils` | 对 DESIGNERBUFFER 做 XML/XAML 转换；删/改特定 localization、标题和中文按钮文案；将 4.3 设计节点转成 4.4 节点并补充属性/尺寸 | 不能视为任意第三方 XAML 的无损往返 |
| `GlobalVarUtils` | 用 `CompatibleTool.xml` 的 IO 改名映射，重建模块参数绑定；将索引订阅改为 `All`，绑定按 128 项批次写回，并更新全局变量结果参数名/type | 转换只在 solution 的 V4.4 路径显式调用；无全部变量种类和失败回滚差分 |
| `CustomizedModuleUtils` | V4.2 扫描对象内流程/Group/模块，根据定制模块枚举分发 `CustomizedCompatible`；当前托管枚举只列 `TestModu`；V4.4 方法直接返回 `true` | 这不是所有自定义插件的迁移支持证明 |

## 已发现的源码风险点

- `ConvertSolution`、`ConvertProcedure`、`ConvertGroup` 在版本分支之后都会无条件执行 `Convert*4_3To4_4` 并重新赋值给 `flag`。对 V4.2，前一阶段的动态 IO 返回值可能被覆盖；对 V4.0/V4.1，前一阶段聚合迁移结果也可能被覆盖。流程和 Group 的 V4.3→V4.4 帮助方法在反编译代码中只返回 `true`（流程还重复记录 GlobalVar 日志），因而静态控制流显示先前失败可能不再阻止保存。需要原版受控样本才能确认这是实际迁移器缺陷、反编译重建异常，还是受其他 SDK 状态门控。
- `Program.Main` 先检查原始 `args.Count() >= 4`，然后才把参数拼接并按分号切分，随后读取 `array[4]`。因此此检查并不保证切分后有五段；主程序传入的命令行实际如何落入 `args`，需结合 Windows 参数解析和原版进程观测复核。密码和包含分号的字段边界也未见可靠转义协议，静态风险不等于已复现崩溃。
- 主程序启动迁移器后等待退出并检查输出文件。反编译代码没有显示读取迁移进程的 ExitCode，也没有显示 `Process` 的 `using`/`Dispose`；这是静态资源管理风险记录，不代表已观测到 VM 进程泄漏。
- `ConvertSolution4_3To4_4` 将多个子步骤用按位 `&` 汇总，能保证各步骤均被调用；早期分支则通过顺序赋值覆盖结果，不能把它们简化成相同的失败传播模型。

## 逆向边界与下一步

| 项目 | 状态 |
|---|---|
| 版本查询、输出命名、子进程启动及主程序门控 | 反编译托管代码已确认；未作 VM 运行差分 |
| CompatibleTool 的转换分段和可见子步骤 | 反编译托管代码已确认；具体插件/native写入结果未验证 |
| `.sol/.prc/.gro` 容器解析与写入者 | 未确认；托管层把实际读写委派到 SDK/native |
| 迁移的原子性、部分失败回滚、密码格式与版本支持矩阵 | 未确认 |
| 主框架 `.vmx.json` 与原生 `.sol` 互操作 | 未实现，不能互称兼容 |

继续完成原生格式逆向需要旧版原厂文件与目标版迁移输出的成对样本、VM 原版迁移器受控运行、同一文件迁移前后版本查询，以及 native `IMVS_SaveSolution` / `IMVS_LoadSolution` 与插件模型/附件持久化写入者的交叉定位。在取得这些证据前，main 继续把 `.vmx.json` 明确标为独立格式；不编造 `.sol` 导出器，也不把静态反编译冒充运行验证。
