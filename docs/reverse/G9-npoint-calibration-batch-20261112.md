# G9 NPoint 标定工具批次

日期：2026-11-12

`NPointCalibration.PluginChecks` 通过 5817 项，`NPointCalibration.HostChecks` 通过 32 项；NumericInput 12、TextInput 43、PointAlias 35、Reset 36 项通过。宿主检查覆盖真实 MainWindow 注册、输入样本求解、路径/旋转轴、Presenter 几何、结果列表、点表立即写入、显示数组、保存重开、失败清理、恢复求解和配置会话释放；补充检查覆盖输入边界、教学文本、生产别名服务和源图重置。

这批结果确认当前源码证据范围内的插件与生产宿主合同可复跑。原厂 native 求解实现、原厂 `.sol` 字段和桌面像素级一致性仍为“运行未确认”；测试通过不提升为 parity-closed。
