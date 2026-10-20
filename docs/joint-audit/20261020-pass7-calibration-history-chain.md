# 第七轮：标定矩阵跨 Group 连接的撤销/重做

范围为本轮已建立的 N 点标定→标定加载→owned Group→坐标系链，仅核主框架的连接历史、恢复后的实际执行及保存重开。图像源只提供坐标系必需的受控输入。未修改 `tool-plugins` 在制工作树。

## 原版代码定位和不可混同的边界

- `analysis/decompiled/VMControls.WPF/VMControls.WPF.DataForUI/VmConnectionDecorator.cs` SHA-256 `4784D7FFBE95B87D9BDB7D1DBC70E6F9307200F37B640474A425951021890BEE`：`DeleteConnectionCommandExecute` 对非全局模块先向 `IUnRedoService` 注册 `RootOperation`，再由 `DeleteConnection` 删除 `source.NextModules` 的目标；`VM.Core.VmModule.NextModules_CollectionChanged` 对删除边调用 `DisConnectOneModule`、调整 `PreModules`/订阅并触发 `ModuleOperate(null)`。后者所在 `analysis/decompiled/VM.Core/VM.Core/VmModule.cs` SHA-256 `53E2F0F9032C4631B59CF0EB2A00E06D50469D94706E7E370E9CC7E1059C72CE`。
- `VM.Core.VmIO.RelateIO` setter 另向撤销服务注册 `SubscribeUnRedoAction`，随后执行取消旧订阅/建立新订阅/刷新 ready 状态。原版 **模块间执行线** 和 **参数叶订阅** 并非同一种历史对象。本探针只核当前 Group 输出至坐标系矩阵输入的 `ConnectionDocument`，不能宣称原版该具体连接的撤销根或 UI 订阅重建已同态证实。
- 本地 `GraphEditorDocument.Disconnect` 经 `ChangeProcedure/Edit/RecordEditHistory` 注册文档快照；`Undo/Redo` 经 `DocumentHistoryEngine.Replay` 恢复。它是当前独立适配，不是原版 `RootOperation`/`SubscribeUnRedoAction` 的复制。

## 受控实际执行

新增 `tests/CalibrationHistoryChain.Checks/Program.cs` SHA-256 `1094356E9E1A5B4D56FDF1283B38653E1BC0C4082C78604487B9E913705385C3`，工程 SHA-256 `34BB43761C26C5791F413357F88B89A8C654EF850043CE3A18CD1D488AF552A4`。基于已审的正常九项链，选定唯一 `Group.Matrix→Coordinate.CalibMatrix` 连接：先执行正常图，断开并验证边消失/历史可撤销；撤销后验证边恢复、`CanRedo`、重新实际执行四节点成功且九项矩阵逐项传输；重做验证边再次消失；第二次撤销验证边与执行恢复；最后保存方案、重新载入并再次实际执行。测试并未把单纯 JSON 反序列化算作链路通过。

命令 `dotnet run --project tests/CalibrationHistoryChain.Checks/CalibrationHistoryChain.Checks.csproj -c Release` 退出码 0。保留原始日志 `analysis/joint-audit/pass7-cal-history/checks.log` SHA-256 `C04E3D7D766570C0B60806A5FC604A1DB0ADD6DA845DB1C7D2512E3869D5116B`；四次实际运行分别为 initial、after-undo、after-second-undo、reopened。生成的 XML/方案只位于本线被忽略的 `analysis/joint-audit/pass7-cal-history/`。

## 判定

当前实现的该条主框架连接历史与恢复执行、保存重开切片自审通过，无证据支持在此处改产品代码。原版相同整图、动态标定加载端口的撤销、配置参数撤销、多层 Group、失败文件后恢复与桌面事件尚未覆盖；这些不能由本探针代替。此项只关闭 J09 历史链中的一个具名边，不把十个标定工具或 96 个框架实施单元的总数增加。
