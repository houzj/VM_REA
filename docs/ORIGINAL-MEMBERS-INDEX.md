# 原厂 25 个唯一程序集源码级成员清单（索引）

机器台账：`tests/ArchitectureRecovery/original-members.map.json` · builtFrom `7d65a1e1/dfd5fd922c28`

按 `canonicalTree` 归并 28 棵反编译树，3 个重复 pass 不累加；只扫描排除 `bin/obj/.git/.a01group/.pending/TestResults` 后的 `.cs`。

**证据边界**：类型与方法签名来自静态反编译文本的正则提取，属 `source-confirmed-decompiled`，不是运行时证据；不含函数体，不用推断补齐。扫描计数与 `docs/architecture-baseline.json` 去重口径（2614 `.cs` / 435666 行 / 3503 类型 / 10195 方法）不同，差异原样保留。

## 汇总

| 项 | 值 |
|---|---|
| 唯一程序集 | 25 |
| 反编译树（含重复 pass） | 28 |
| 重复 pass 程序集 | 3 |
| canonical 树 `.cs` 文件 | 2614 |
| 扫描行数 | 381424 |
| 提取类型声明 | 3719 |
| 提取方法签名 | 10128 |

## 25 个程序集

| 程序集 | canonical 树 | 层 | `.cs` | 类型 | 方法签名 | 命名空间数 | 重复 pass |
|---|---|---|---|---|---|---|---|
| VisionMaster.exe | VisionMasterMain | L1-shell | 43 | 45 | 218 | 8 | 是 |
| Apps.XmlParser | Apps.XmlParser.CapabilityAudit | L5-config | 365 | 398 | 1488 | 7 | 是 |
| VMControls.WPF | VMControls.WPF | L4-canvas-controls | 215 | 238 | 1327 | 12 | 是 |
| vServerApp | vServerApp.Audit | L1-shell | 12 | 18 | 46 | 4 | 否 |
| VM.Core | VM.Core | L2-model | 114 | 143 | 688 | 1 | 否 |
| VMControls.Data | VMControls.Data | L2-model | 8 | 7 | 39 | 1 | 否 |
| VM.PlatformSDKCS | VM.PlatformSDKCS.Audit | L3-runtime-sdk | 158 | 409 | 1020 | 1 | 否 |
| iMVS-6000PlatformSDKCS | iMVS-6000PlatformSDKCS.PublicAudit | L3-runtime-sdk | 751 | 1391 | 838 | 3 | 否 |
| VM.Controls | VM.Controls | L4-canvas-controls | 289 | 322 | 1378 | 23 | 否 |
| VMBaseControls | VMBaseControls | L4-canvas-controls | 133 | 140 | 646 | 5 | 否 |
| DynamicObjectConfigControl | DynamicObjectConfigControl | L5-config | 89 | 88 | 323 | 8 | 否 |
| Apps.XmlUI | Apps.XmlUI | L5-config | 130 | 144 | 416 | 6 | 否 |
| ExtensionParamUserControl | ExtensionParamUserControl.CapabilityAudit | L5-config | 4 | 3 | 2 | 1 | 否 |
| VM.PcUI | VM.PcUI | L6-display | 162 | 219 | 1324 | 11 | 否 |
| Apps.Style | Apps.Style | L6-display | 1 | 0 | 0 | 0 | 否 |
| Apps.ColorFun | Apps.ColorFun | L6-display | 3 | 3 | 3 | 1 | 否 |
| Apps.PcUI.ImageSource | Apps.PcUI.ImageSourceAudit | L6-display | 21 | 22 | 65 | 5 | 否 |
| CompatibleTool | CompatibleTool.Audit | L7-migration-hosts | 35 | 38 | 111 | 2 | 否 |
| VM.ExternalCall | VM.ExternalCall.Audit | L7-migration-hosts | 52 | 61 | 86 | 4 | 否 |
| ImageSourceModuleControl | ImageSourceModuleControl | L7-migration-hosts | 7 | 8 | 20 | 2 | 否 |
| IMVSGroup | IMVSGroup.WorkflowAudit | L8-tool-plugins | 4 | 5 | 54 | 1 | 否 |
| IfModule | IfModule.WorkflowAudit | L8-tool-plugins | 5 | 5 | 5 | 1 | 否 |
| IfBranchModule | IfBranchModule.WorkflowAudit | L8-tool-plugins | 4 | 4 | 17 | 1 | 否 |
| BranchModule_STD | BranchModule_STD.WorkflowAudit | L8-tool-plugins | 4 | 4 | 5 | 1 | 否 |
| BranchStringCpmL | BranchStringCpmL.WorkflowAudit | L8-tool-plugins | 5 | 4 | 9 | 1 | 否 |

每类型方法名最多列 24 个，超出在台账中记 `methodsTruncated`，完整清单见机器台账 `assemblies[].types[]`。

## 口径冲突与未完成项

- `42/96=43.75%` 与 `41/96=42.71%` 并列保留；`PositionCorrection` 批次身份差异不擅自统一。
- C03 差分 `45/971486` 对 `47/973202` 仍开放；本清单不解释根因，不声称 parity。
- 缺失函数体、资源与 ABI 仍是 REA 补证任务，本清单不生成实现代码。