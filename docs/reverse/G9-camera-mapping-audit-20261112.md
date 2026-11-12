# G9 CameraMapping 工具审计

日期：2026-11-12

`CameraMapping.InputChecks` 通过 36 项，`CameraMapping.ResetChecks` 通过 47 项，覆盖输入生命周期、状态恢复和 Reset。

`IndexChecks`、`PluginChecks`、`ReachableChecks`、`SolverChecks` 均因缺少原厂 XML/输入/solver 证据入口失败；其中 PluginChecks 需要 `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Calibration\IMVSCameraMapModu\IMVSCameraMapModuAlgorithmTab.xml`。这些状态保持证据开放，不用当前实现伪造原厂资源。
