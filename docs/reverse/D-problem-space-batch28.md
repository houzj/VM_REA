# 第二十八轮 D 问题空间：持久化/兼容可独立实施性

## 问题
哪些 D02/D04/D05/D08 行为能够以已确认源码独立落地，且不会冒充原生 `.sol/.solx/.gro/.prc` 等价？

## 结论
本轮 **+0**。没有可安全关闭的新增实施单元，也未修改产品源码。D02 的独立 `.vmx.json` 事务/结构校验已有自审实现，但其剩余资产历史重定位合同仍未证实；D04 的托管迁移 XML 分支不能扩展为原生方案迁移；D05 依赖 D03/D04/B07 及原生读写链，当前缺证据；D08 只能做最终对账，不能产生实施计分。

## 证据与边界
- `docs/SOLUTION-PERSISTENCE-BOUNDARY.md:11-17,29`：`VmSolution`/`SolutionManager` 将原生保存、加载委托给 SDK/native；托管层没有容器、校验块、压缩或密码算法实现。现有 `SolutionStore` 的 Base64/SHA 校验属于独立 `.vmx.json` 格式。
- `docs/COMPATIBLETOOL-VERSION-MIGRATION.md:22,28-29,43,50-51,79-84`：`CompatibleTool` 的托管迁移步骤和 P/Invoke/头文件可确认入口，但 native writer/reader、密码、版本矩阵、原子失败和往返未知。
- `docs/reverse/D04-migration-gate-batch27.md`：D04.1/D04.2 已复核，禁止把 `FrontendDesignMigration.DesignConverter` XML 转换分支标作 `.sol` 迁移器。
- 原生样本仅能证明部分 `.sol` 外层 ZIP/目录可只读解析；不能推出所有版本、密码、压缩参数、未知条目保留或写回顺序。

## 逐项判定
- **D02**：可继续维护独立 JSON 存储合同，但现有闭合切片已计入；剩余纯历史/资产重定位缺少合同，不能再补实现。
- **D04**：迁移/版本/密码实现 blocked；解除需同版旧新样本、正确/错误密码/取消/失败日志、原生 Save/Load 函数体或可复验 SDK 探针。
- **D05**：依赖 D03、D04、B07，且要求原机打开/执行/再保存；当前不能实施兼容写回。
- **D08**：仅全域对账，不产生 done reverse unit；依赖 D07。

## 不确定项与解除条件
需要取得同一版本成对 `.sol/.solx/.gro/.prc` 样本及迁移前后版本查询，受控原厂 CompatibleTool 运行记录（正确密码、错误密码、取消、升级失败），并定位 `IMVS_SaveSolution`/`IMVS_LoadSolution` native writer/reader 与未知字段/附件所有权。取得前保留 partial/blocked，不重派静态门槛，不更改 `tool-plugins`。

本轮无 main 源码改动；`tool-plugins` 未访问、未修改。
