# 第二十七轮 D04.1/D04.2 版本迁移与兼容门槛

## 结论

本轮审查 `D04.1`（迁移）与 `D04.2`（版本/密码兼容入口）是否存在可以在 main 独立落地、并由原始源码证据闭合的实施单元。结论为 **+0**，没有修改产品源码。

`src/FrontendDesignMigration` 是已落地的纯托管、源码确认的 `Root/V4.0 -> V4.4.0` 设计 XML 转换分支，并有 18 项自审；它不等价于 CompatibleTool 的完整迁移器，也不构成 `.sol/.solx/.gro/.prc` 容器读写实现。现有证据明确显示方案保存、加载、版本查询及密码处理最终委托 SDK/native，native 函数体和容器事务语义尚未取得。

## D04.1 迁移

- 已确认：`DesignConverter` 读取 XML，禁止 DTD，使用显式编码，按原 `FirstChild` 分支处理 Root，执行模板、尺寸、透明画刷、嵌入 SubWindow 的源码确认转换。
- 已验证：`tests/FrontendDesignMigrationChecks13` 的 18 项 synthetic 模板断言及 CLI 的拒覆盖行为；这些是托管独立实现的自审，不是原厂迁移器运行差分。
- 未确认：原厂设计缓冲区如何从 `.sol` 读取和回写；迁移失败的原子性、旧新配对样本、插件模型/附件/ROI 写回、版本分段实际效果。
- 因此不新增实现，避免把 XML 分支冒充 `.sol` 迁移或增加未经证据支持的容器写入器。

## D04.2 版本/密码兼容

静态托管证据确认 `ProjectManager` 查询 `IMVS_GetSolutionVersion`，CompatibleTool 通过 SDK 的 `Load/SaveAs`、流程导入和 Group 导出入口工作；`NativeMethods`/SDK 头仅给出 P/Invoke、`__stdcall` 声明和参数结构。没有原生函数体、加密/密码校验算法、版本矩阵或“错误密码/取消/升级失败恢复”的可复验样本。因此不能在 main 添加密码校验、版本路由或 fallback 逻辑；任何实现都会是独立兼容猜测。

## 审计证据

- 托管迁移与 SDK 边界：`docs/COMPATIBLETOOL-VERSION-MIGRATION.md`。
- 已有托管分支实现：`docs/reverse/D-frontend-migration-implementation-batch13.md`。
- 既有门槛结论：`docs/reverse/D04-version-gate-batch24.md`。
- 源码：`src/FrontendDesignMigration/DesignConverter.cs`、`src/FrontendDesignMigration/Program.cs`。
- 原生 `.sol` 样本仅观察到 ZIP 头与目录，不能推出全格式/密码协议。

## 解除条件

需要至少一组同一版本的旧/新 `.sol`（含正确密码、错误密码、取消和失败输出）、受控原厂 CompatibleTool 运行日志，以及 `IMVS_SaveSolution`/`IMVS_LoadSolution` 函数体或可复验 SDK 读写探针。取得前 D04.1/D04.2 保持 partial/blocked，下一轮不应重复静态 XML 门槛。

本轮未访问或修改 `tool-plugins`。
