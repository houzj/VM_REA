# 第十一轮：N 点实际 MainWindow 点表写入成本

本轮把第六、十轮的受控回调计数推进到**真实 `MainWindow` 配置窗口按钮路径**。样本只覆盖标定分类的 N 点标定；离屏 WPF 是当前代码探针，不等于用户交互桌面性能验收，更不能解释用户所见的长期“未响应”。

## 原版与当前路径

- 原版 `IMVSNPointCalibModuControl.NPointList.UserControl_Loaded → Init` 按 `CalibPointTotalNum + RotPointTotalNum` 建行，每行五次读值、格式化，再调用 `PointList` 五个 setter；setter 向 `IUserStringData[字段+ID]` 逐叶写。原 `PointList.cs`、`NPointList.xaml.cs` 和 XAML 的完整 SHA 分别为 `A396060C73DBF56B1A7FB7849A4BCD17FE5CBC3D04AD8356F68D4B3B1C9B3813`、`B972D12AF77B27DC3A19E00629C77F3BED58D3FD7A266DD0AA28050A281B5A46`、`92945CE0D5ED92F98F12187A0EB92F1673516760B894F0CD52F89FC2C7341D79`，原体路径见[第六轮源码审计](20261020-pass6-npoint-ui.md)。逐叶写是原源码事实；原版的图文档历史开销和实际耗时不由该成员证明。
- 当前 `NPointConfigurationControls.Register` 的 `NPointEditPoints` 点击调用 `NPointConfigurationPointEditor`，构造器为每行五列调用 `commit`；`NPointConfiguration.CreateConfiguration` 的回调经 `PointValues → Apply → ToolConfigurationHost.Apply → GraphEditorDocument.SetProcedureNodeParameters → Edit`，`Edit` 每次 `RecordEditHistory`、`Revision++`、`Changed`。实际配置窗口的 `Changed` 进一步触发 `RefreshAll`。源码见 `src/VisionPlugins.Calibration/NPointConfigurationControls.cs`、`NPointConfigurationPointEditor.cs`、`NPointConfiguration.cs`、`src/VisionEditor/ToolConfigurationHost.cs`、`src/VisionEditor.Core/GraphEditorDocument.cs`。这整个文档编辑链是当前适配，不能移称原版必然逐次重画。

## 实际窗口探针

`tests/CalibrationNPointMainWindowCost.Checks/Program.cs` 创建当前 `MainWindow` 和真实图节点，实际打开工具配置窗并点击 `NPointEditPoints`，在模态点编辑窗口出现时读 `GraphEditorDocument.Revision`/`Changed`，再编辑一个单元格。执行：

```powershell
dotnet run --project tests/CalibrationNPointMainWindowCost.Checks/CalibrationNPointMainWindowCost.Checks.csproj -c Release
```

最终 exit 0，三组共 12 个断言通过；日志 `analysis/joint-audit/pass11-cal-npoint-mainwindow/final-run.log` SHA-256 `85D8940022B6495D9A5899C067F272A6B1291F471B2DA51D40E8D60FA9D3AAF6`，结构化数据为同目录 `summary.json`。探针源码 SHA-256 `EB08EE289ACA482DBC89C74028B61A6DBD0970A5B238657435838E25181B3FD9`。原先仅 4/12 行的首跑日志 `first-run.log` 保留，没有抹去。

| 行数 | 点表打开期间 Revision / Changed 增量 | 单格编辑增量 | 此次离屏模态调用耗时 |
|---:|---:|---:|---:|
| 4 | 20 / 20 | 1 / 1 | 851.7 ms |
| 12 | 60 / 60 | 1 / 1 | 1948.3 ms |
| 32 | 160 / 160 | 1 / 1 | 3463.2 ms |

**结论：** 当前真实 MainWindow 路径证实打开点表有 `5×行数` 次完整文档编辑/变更事件，单格一次。此处未测一次编辑在不同图规模或历史负载下的分布，不作“编辑单格必然慢”的性能定论；离屏三次样本仅说明同步工作有秒级成本，不能与原版速度直接比较。界面长期卡死需当次 UI 线程栈/等待链才能归因。

## 后续合同与界限

[FCR-033](../CONTRACT-REQUESTS.md) 已记录插件专属点表与共享图文档写入边界。本轮实际 MainWindow 的 20/60/160 次 `Revision` 和 `Changed` 可作为修复前基线；优化应在插件所有权范围内审原逐叶成功/失败与打开时副作用，并保证编辑历史、导入/删除/清空、方案持久化不退化。此轮没有直接修改插件代码，也没有改动 `tool-plugins` 在制文件。
