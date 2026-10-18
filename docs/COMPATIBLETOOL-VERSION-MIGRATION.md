# CompatibleTool 版本迁移逆向记录

更新：2026-10-18。范围是 VM 4.x 安装版 `CompatibleTool` 的托管迁移调度和 WPF 主程序调用链。证据为反编译重建源码的静态分析；未运行原版迁移器，也没有旧版 `.sol/.prc/.gro` 控制样本。因此本文确认的是托管调用和转换步骤，不确认原生容器格式、迁移成功率或输出可重开。

## 证据与定位

| 证据 | 位置 | SHA-256 | 可确认范围 |
|---|---|---|---|
| 主程序版本检查及隐藏子进程启动 | `analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster.Project/ProjectManager.cs:604-700` | `1ae41a0841dd65f5dfa7461078c370822e61826877c42e299939af1fd47f9370` | 查询原生方案版本、低版本提示、输出路径冲突处理、启动参数、等待进程和输出文件存在性检查 |
| 迁移工具入口 | `analysis/decompiled/CompatibleTool.Audit/CompatibleTool/Program.cs:10-46` | `796184f7cbe03c2c9c267873b0992e1cc4b1574ff6f46814bf6285288aca2bb7` | 参数组合、四类迁移入口、异常捕获、返回码与 SDK 释放 |
| 方案/流程/Group/设计转换 | `analysis/decompiled/CompatibleTool.Audit/CompatibleTool/ConvertHelper.cs:131-585` | `e402795ab8954269b1886274786f7734afc0970abfbeb53c098701831c53ab35` | 版本分段、迁移步骤、保存入口和原生 SDK 边界 |
| 动态 IO 迁移规则 | `analysis/decompiled/CompatibleTool.Audit/CompatibleTool/DynamicIOUtils.cs:33-520` | `7e823cdab9ecfd0b5665e6ba2b2ecbd2b7851bdd5026430e7882cef343309515` | V4.2、V4.3、V4.4 的部分工具级动态输入输出改写 |
| 序列化调用边界 | `analysis/full-audit/solution-serialization-contract.json` 中 `save`、`load`、`version-migration` 链 | 报告记录了托管文件与 SDK 头文件哈希 | 方案读写由 SDK/native 承担；迁移器转换规则和原生容器仍有未知项 |

## 迁移调用链

1. `ProjectManager` 调用 `IMVS_GetSolutionVersion`，对返回的 `strTargetSolutionInfo` 作字符串版本比较。低于本机 `Lang_version` 时，显示升级遮罩，并在原方案同目录选择 `原名_目标版本.sol`；文件已存在时依次尝试带序号的新名字，不覆盖旧迁移输出。
2. 主程序以隐藏、无 Shell、重定向标准输出/错误的子进程启动同目录 `CompatibleTool.exe`，传入 `VMSolution;源路径;密码;目标路径;目标版本`，等待退出，再检查目标文件是否存在。主程序不以子进程退出码作为唯一成功条件。
3. 工具先调用 `IMVS_GetSolutionVersion`、`IMVS_GetPrcVersion` 或 `IMVS_GetGroVersion` 取得文件版本，再由 `VmSolution.Load`、流程导入 API 或 `IMVSGroupTool.LoadIndependentGroup` 加载原对象。它设置 SDK 的版本升级标志，对对象作原生托管对象操作，最后调用 `VmSolution.SaveAs`、`SyncExportProcess` 或 `ExportGroup` 写出。
4. 方案从 V4.0/V4.1 到 V4.4 的托管步骤包括：IsReady 数据、动态 IO、模板匹配模型、前端设计、定制模块；V4.2→V4.3 处理动态 IO；V4.3→V4.4 更新前端设计、动态 IO、全局变量和定制模块。V4.4 的动态 IO步骤还会更新特定模块，并对原生模块类型 `0` 清空 `PathPoints` 用户数据。
5. V4.3→V4.4 的设计文档转换有 XML 层实现；V4.0→V4.4 的设计 XML 路径经过 VM4.2、VM4.3/4.4 两个转换工具。以上只覆盖显式托管步骤，插件模型、附件、ROI 持久化和二进制容器仍由 SDK/插件层负责。

## 已发现的源码风险点

- `ConvertSolution`、`ConvertProcedure`、`ConvertGroup` 在版本分支之后都会无条件执行 `Convert*4_3To4_4` 并重新赋值给 `flag`。对 V4.2，前一阶段的动态 IO 返回值可能被覆盖；对 V4.0/V4.1，前一阶段聚合迁移结果也可能被覆盖。流程和 Group 的 V4.3→V4.4 帮助方法在反编译代码中只返回 `true`（流程还重复记录 GlobalVar 日志），因而静态控制流显示先前失败可能不再阻止保存。需要原版受控样本才能确认这是实际迁移器缺陷、反编译重建异常，还是受其他 SDK 状态门控。
- `Program.Main` 将收到的命令参数拼接后按分号切分，但检查 `Count() >= 4` 后读取 `array[4]`。按静态代码，恰好四段参数会进入异常路径；主程序当前调用通常传五段。密码和包含分号的字段边界也未见可靠转义协议。尚无实际进程参数观测验证。
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
