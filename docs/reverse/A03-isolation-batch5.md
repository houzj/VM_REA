# A03 第五批：初始化隔离退出审查

2026-10-19，基线 main `e6bf62a`。有界子项 **A03.init-isolation = blocked**。阻塞审查已完成，原装饰器绑定运行尚未执行，完整 A03 未关闭。本批不改产品、不访问 tool-plugins。

## 固定验收门槛与本批结果

| 门槛 | 结果 |
|---|---|
| 识别路径初始化、ModulePathGenerate、日志初始化的入口与读写依赖 | 已定位并保全本报告列出的 CIL、配置、注册表只读观察 |
| 证明原装饰器初始化仅使用自有受控副本、实例和输出 | 未满足：原日志代码独立选择安装配置并监视其变化；AppBase 重定位不能替换该依赖 |
| 在满足隔离前提后运行原装饰器绑定并记录通知与目标值 | 未执行；运行断言 0 |
| 保全结果、未知与解除条件，不以静态方法数替代验收完成 | 已满足；原文件未修改，证据哈希随目录保留 |

这是一项明确阻塞，不应在后续批次反复追加同一日志静态切片。解除前应调度其他依赖满足的包。不能把本报告作为 Undo 刷新行为已确认的依据。

## 原件证据与复验

本批使用 reverse-engineer-anything；新 REA `inspect_managed_artifact` Evidence `ev_ff90a25ff7174a267deac758048e4cc18f99ed9a1b9aa7d73434aad58f4ed551` 确认 VM.Utility.dll（SHA256 `65ebb5edabf77465fa01f8c6d70efdcbd4b6a87f510911249cd93d73649d1cdf`，MVID `32e8ac89-6335-43c6-aa3e-61d8c0a670e5`）。`rea-utility-identity-summary.json` 明确只是身份摘要，完整结果仍在 REA 本会话账本；不是导出的完整证据包。

`tests/NativeEvidence/A03Isolation/Run.ps1` 使用 .NET Framework 系统 csc 编译元数据读取器，再 ReflectionOnlyLoadFrom 读取原件，不执行目标函数。最终输出 VM.Utility 10、Apps.Log 11、Apps.BaseLog 11 个定向方法体，共 32 个；其中 ConfigurationHelper 的已知 GetConfig/GetConfigValue/cctor 仅是依赖审查复核，不宣称全部为新增覆盖。VM.Util 0 个匹配，亦保留，不计入有效方法数。stdout-stderr、原 CIL/token/IL 字节哈希、异常区域、4 DLL 前后完整性检查、原资源配置副本及哈希位于 `analysis/full-audit/batch5-20261019/A03/`。脚本多次调整定向选择只增加依赖读取，未发生目标运行失败。最初读取不存在报告文件及 PowerShell 通配路径 rg 错误只是工作检索错误，不算原厂行为。

## 已确认初始化数据流

1. 第四批已确认的 `VMControls.WPF.DataForUI.VmModuleBaseDecorator.cctor`（原源码 `analysis/decompiled/VMControls.WPF/VMControls.WPF.DataForUI/VmModuleBaseDecorator.cs:485`）先构造字典、`LogHelper.Info`，然后 `CurrentPathHelper.Instance.GetCurrentPath()`，以 StreamReader 读 `ModulePathGenerate.txt`，按 `" # "` 拆每行并使用数组第 0、2 项；正常完成后再次 Info。该路径没有创建/清理目录动作，但日志在路径文件读取之前已经初始化。格式错误/缺文件不能当成安全旁路，因为前置日志仍可发生。
2. `CurrentPathHelper.cctor` token `060002B8` → ctor `060002B9` → `ConfigurationHelper.GetConfig("StartClientMode")` `0600012C`。空值或 `"1"` 设置 IsSDKMode，`"2"` 设置 IsRpMode，其他值两者保留 false。GetConfig 读取 ConfigurationManager.AppSettings，缺值/异常会调用 LogHelper.Error。构造器本身只写托管字段。
3. `GetCurrentPath` `060002BA` 默认 AppDomain.BaseDirectory；SDK 模式读 HKLM `SOFTWARE\\WOW6432Node\\Microsoft\\.NETFramework\\v4.0.30319\\AssemblyFoldersEx\\VisionMaster` 的默认值并补反斜杠。GetRegisterValueByName `060002BD` 用只读 OpenSubKey/GetValue，并 Close；无注册表写入。空注册表字符串补斜杠的分支也按原 CIL 保留，不假设安装路径总有效。GetVMInstallPath `060002BC` 的 CurrentVersion/Uninstall 查找是另一成员，不是本次装饰器 GetCurrentPath 的直接调用。
4. **日志路径独立于上面的模式**：`Apps.Log.LogHelper.cctor` `06000001` → GetCurrentVersionInstallPath `06000002` 无条件按同一 HKLM 默认路径寻找 `myLibs\\Apps.BaseLog.config`，仅当该文件不存在才退回 AppBase。然后 `Apps.BaseLog.Config.XmlConfigurator.ConfigureAndWatch(FileInfo)`，取得多个 logger，并启动 IsBackground=true 的日志队列线程。两个数组各 10000 个引用；队列闭包 `<.cctor>b__14_0` `0600000B` 定期交换、按 Info/Debug/Error 调 objLog，每轮 Sleep(1000)。不是仅一次同步文本写入。
5. `BaseLog.XmlConfigurator.InternalConfigureAndWatch` `060005BC` 先 InternalConfigure，再按配置完整路径建/替换 ConfigureAndWatchHandler。该链说明运行期继续依赖外部安装配置；本批没有执行 watcher 或声称观察过回调。
6. 日志输出不能被错误描述为“必写安装目录”。`FileAppender.ActivateOptions` `060006AC` 调 ConvertToFullPath `060006BA` → `SystemInfo.ConvertToFullPath` `060001E3` → get_ApplicationBaseDirectory `060001CB`，实际返回 AppDomain.BaseDirectory，再 Path.Combine/GetFullPath。因此受控 AppBase 能重定位普通相对 File 值；OpenFile 后续通过锁模型打开文件。

## 当前机器只读观察与阻塞条件

`registry-observation.json` 为 PowerShell 只读注册表观察，没有运行原厂函数。当前默认值为 `D:/Program Files/VisionMaster4.4.0/Development/V4.x/ComControls/Assembly`；其 `myLibs/Apps.BaseLog.config` **实际存在**。所以单纯把 DLL/应用配置/ModulePathGenerate 复制到审计目录，即使设置 StartClientMode 为非 SDK 模式，原 LogHelper 仍选择这份安装配置。

保全了 Applications/myLibs 与上述 registry-selected 两份原配置，分开命名；分析以实际被选中的版本为准。`registry-selected-appenders.json` 列出各 File 值。目前这些值均为相对路径，因此静态上可结合自有 AppBase 归一化；**这并不能消除安装配置的 watch/reload 和其可变化内容**。本报告未声称已经发生越界写入，也未声称仅因为读安装配置就一定危险。尚未证明自动配置、滚动备份、锁模型、watcher 重载和全部输出端均被强制约束在自有副本/目录；当前四 DLL 定向成员范围不覆盖 BaseLog 全配置器安全证明。

本次退出决策：遵守原机/SDK 运行只用自有受控副本/实例/输出的要求，**不启动原装饰器，不修改 HKLM，不改安装原件、不用反射跳过/篡改原 cctor 后冒称原运行**。

解除条件是提供独立 Windows 受控实例（例如专用虚拟机/快照）使 HKLM 默认路径、原 DLL、原日志配置、AppBase、ModulePathGenerate 和全部文件输出都归属该实例；或先取得等价的进程局部注册表/文件系统隔离及输出约束证明，不改宿主 HKLM。到那时只需复验具体绑定问题：同一源实例的模型 ProceZoomIndex 通知是否更新显式接口 PartentZoomIndex 绑定目标，对照直接 setter 与 Undo，并分开 DisplayModuleSet 换源重求值。桌面验收仍需单独核实用户交互桌面，不能把隔离 WPF 事件计为原机桌面等价。

## 进度指标

本有界子项 4 个门槛：满足 2，未满足 1，因前置未满足而未执行 1；最终状态 blocked。新增原装饰器运行 0、产品修复 0、完整 A03 关闭 0。此次新增的是可核验的外部依赖和退出条件，不是整体完成百分比。
