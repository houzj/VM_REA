# 配置交互服务检查

`dotnet run --project tests/VisionConfiguration.InteractionChecks/VisionConfiguration.InteractionChecks.csproj`

29项检查验证共享实际服务：原厂文件选择 flags、当前有效文件目录覆盖每模块history、选择本身不提前写history、显式Remember持久化、不同service实例读取、并发实例不丢模块更新、取消不改history、远程Unsupported不落本地选择器、无效选择/选择器异常/坏history/实际磁盘写失败的明确诊断、通知payload与SDK抑制、源中英文4通知/选择键。磁盘history使用临时目录；picker与消息呈现器注入，不弹出真实对话框。

源锚和哈希见 docs/plugins/configuration-interaction-services-evidence.json。JSON用户路径映射及默认WPF通知外观是独立实现，不能称原厂INI或HikMessageBox像素复现。
