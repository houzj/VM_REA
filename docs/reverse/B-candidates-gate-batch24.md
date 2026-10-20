# B线第二十四轮：FCR-018/FCR-021 producer 门槛复核

日期：2026-10-20  
基线：`6fc5e70`（`main`）  
结论：**未发现新的 producer 交付；B06.1/B06.2 不实施、不计完成。**

## 检查范围

本轮只检查 FCR-018（Group 参数选中后的工具装饰器）和 FCR-021（Group 运行参数候选资格元数据）的 producer 是否已经进入当前 main 可审查范围，避免重复 B06 候选扫描和避免在 main 复制插件配置框架。

核对结果：

| 项目 | 当前证据 | 门槛结果 |
|---|---|---|
| producer 提交 | `git log --all` 在 2026-10-19 以后没有出现 FCR-018/FCR-021 producer 提交；`tool-plugins` 工作树不读取、不修改 | 未解除 |
| FCR-021 consumer | `GroupRunConfigurationWindow` 仍遍历 descriptor 的 `Parameters`/`Inputs`；没有 `IGroupRunParam`、`GroupRunParamEnable`、原生类型身份、`CloneSource`、`DisplayForGroupType` 生产快照 | 不能闭合 |
| FCR-018 consumer | main 只有现有 `IWpfToolParameterEditorFactory` 有限 seam；没有插件侧 `GetSupportDisplayForGroupData` 等价 producer，也没有可审查的 Group 装饰器生命周期 | 不能闭合 |
| 合同文档 | `docs/CONTRACT-REQUESTS.md` 中 FCR-018/FCR-021 仍标记“请求，未实现”；未出现 producer SHA、版本化载荷或交接记录 | 未解除 |

## 可复核哈希

```
src/VisionFramework/Contracts.cs                         664E3AAD63ABB2F8F8C6FB18E90E663D4DA76ABBC27C3F64D83A75E8D9E44299
src/VisionEditor/GroupRunConfigurationWindow.cs          C151C447B2819258539C2D47532BAAFF2AAD8D272CD8427730CF5B0F452C6BF2
docs/CONTRACT-REQUESTS.md                                 AADEFCBB5180EFA1E27A8E2177B2D5FD708B97C915DD2F0953A08D5C7CB99E01
docs/reverse/B-runtime-tab-gate-batch23.md                B906AFED83DA8D59C53452FD270EE899B83CF9C60E2F1AFF22E49DBFF96F1D6E
```

## 退出门槛

本轮 B 线实施增量为 **0**。只有收到 producer 的完整 SHA 和源码/合同证据后，main 才能实现消费端：先物化稳定身份与资格快照，再按原始谓词过滤，保留不合格存量代理的诊断；在此之前新增字段或筛选器会是未经授权的第二套插件配置模型。

`tool-plugins` 保持不动；本报告不代表跨项目采用或合流。
