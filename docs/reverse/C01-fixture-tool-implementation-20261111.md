# 位置修正工具交付核对

位置修正工具已经接入主窗口和通用执行器。本轮复核了其源码路径、参数读写、位置修正结果、范围错误、零值传输、数组首项、保存投影、配置拒绝回退和节点删除释放。

`tests/Fixture.PluginChecks` 无界面运行通过 **43 项**。检查使用生产 `FixturePlugin`、生产 `ModuleExecutionScope` 和真实配置资源，原厂 native DLL 没有执行。结果目录为 `analysis/full-audit/fixture-host/run-8b4931c176e648788972ff4c0814d87f`。

该工具的 native 字段、float round-trip 和资源边界沿用 [FIXTURE-NATIVE-FIELD-AUDIT](../FIXTURE-NATIVE-FIELD-AUDIT.md)。当前全量工具台账把位置修正、分支模块和分支字符串列为已有专项自审工具。
