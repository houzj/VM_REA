# G6 保存、重开、错误恢复和重复执行门禁

日期：2026-11-12

## 已闭合链路

```text
GraphEditorDocument / SolutionStore
  → 原子保存与临时文件清理
  → 资源/ROI/执行连接校验
  → 重开恢复方案、流程、资源和身份
  → 运行失败/超时/取消
  → 同一方案重新执行
  → 连续执行停止或恢复
  → 窗口关闭排空活动运行并释放资源
```

## 门禁结果

| 测试 | 结果 |
|---|---|
| `VisionFramework.PersistenceChecks` | 30 项通过 |
| `VisionFramework.SaveRoundTripChecks` | 9 项通过 |
| `HostRecoveryChecks20` | 89 项通过 |

覆盖范围包括重复 JSON、非法结构、资源哈希、嵌入资源、ROI、执行关系、失败保存不替换旧文件、异常/失败/无效输出/释放异常/取消/超时、连续运行策略、失败后重启、关闭时异步清理和服务单次释放。

## 原厂边界

这些结果证明当前独立方案和托管宿主合同。原厂 `.sol` 容器、`ProjectManager.LoadSolution` 的场景回滚字段、原厂同输入运行时顺序仍标记“运行未确认”，不由 JSON 测试替代。

## 复跑

```powershell
dotnet D:\MyWork\Rea逆向\artifacts\persistence-g6\VisionFramework.PersistenceChecks.dll
dotnet D:\MyWork\Rea逆向\artifacts\save-roundtrip-g6\VisionFramework.SaveRoundTripChecks.dll D:\MyWork\Rea逆向\artifacts\save-roundtrip-g6
dotnet D:\MyWork\Rea逆向\artifacts\host-recovery-g6\HostRecoveryChecks20.dll D:\MyWork\Rea逆向\artifacts\host-recovery-g6
```
