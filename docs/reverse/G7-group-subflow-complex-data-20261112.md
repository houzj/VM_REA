# G7 Group、子流程和复杂数据门禁

日期：2026-11-12

## 已闭合链路

```text
Group/子流程实体
  → 多层 owned procedure
  → 循环索引/执行连接/输入输出代理
  → Point/Circle/Fixture/数组复杂数据
  → 每轮实例与持久状态分离
  → 失败/取消/停止传播
  → 保存重开、Undo/Redo、删除恢复
  → 作用域和资源释放
```

## 门禁结果

| 测试 | 结果 |
|---|---|
| `NestedCompositionChecks19` | 46 项通过 |
| `GroupRoundLifecycleChecks20` | 58 项通过 |
| `VisionWorkflow.CallChecks` | 93 项通过 |

覆盖三层嵌套、正负步进、空范围、每轮异步释放、循环中取消、作用域关闭、失败恢复、状态隔离、复杂 Point/Circle/Fixture 输出、子流程调用、保存重开和剪贴板/接口传播。

## 原厂边界

上述结果证明当前托管 Group/子流程合同和复杂数据传输。原厂 Group 装饰器 `DisplayModuleSet` 的跨 Tab 生命周期、原生 Group 参数资格、`.sol` 容器字段以及原厂运行回调仍标记“运行未确认”。

## 复跑

```powershell
dotnet D:\MyWork\Rea逆向\artifacts\nested-g7\NestedCompositionChecks19.dll D:\MyWork\Rea逆向\artifacts\nested-g7
dotnet D:\MyWork\Rea逆向\artifacts\group-round-g7\GroupRoundLifecycleChecks20.dll D:\MyWork\Rea逆向\artifacts\group-round-g7
dotnet D:\MyWork\Rea逆向\artifacts\workflow-call-g7\VisionWorkflow.CallChecks.dll
```
