# 图像源公共配置模型检查

运行：`dotnet run --project tests/ImageSource.ConfigurationChecks`。Windows STA/WPF；使用隐藏公共 ModelWindow，截图导出仅在 tests 中。57 项检查，不连接设备。

覆盖参数与模式真实控件、官方资源模板、目录创建/失败、SDK 列表选择和整流程运行、提交拒绝后的事务回滚、嵌入资源保存与删除源文件后重载。完整证据和边界见 docs/plugins/image-source-configuration-evidence.json 与 image-source-configuration-contract-gaps.json。
