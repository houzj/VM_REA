# 图像源公共配置模型迁移

产品已冻结，等待主线综合回归和用户手动验收，尚未提交。

实现 `IToolConfigurationProvider`，嵌入原厂 AlgorithmTab/Group 定义，使用公共模型、状态、宿主和生产模板；插件仅扩展相机、目录与 SDK 图像列表叶内容，不创建顶层窗口。保留旧窗口工厂仅作兼容入口。参数提交先由宿主接受再更新私有状态，拒绝写入不会污染后续运行。

原厂模板/资源确认包括相机按钮、目录按钮、列表滚动条、缩略图项、复选框、删除、展开折叠和首图按钮。运行模式使用原厂两项 ExecuteType=1/2；SDK 列表选择按源码请求所属流程运行，首图按钮不触发运行。缩略图和保存后重载使用方案 ResourceCatalog。

验证：`dotnet run --project tests/ImageSource.ConfigurationChecks`，57 项通过。实际公共 WPF 控件操作覆盖三模式、相机选择、目录校验、文件添加/清空/子集/首图、真实 GraphExecutor 执行、拒绝提交回滚、删除本地文件后的纯嵌入资源缩略图及运行。没有设备测试。

源码、资源 SHA256 和截图路径见 `image-source-configuration-evidence.json`。未闭合项逐项见 `image-source-configuration-contract-gaps.json`；通过独立实现检查不表示原厂完整等价。截图位于当前 worktree 的 `analysis/full-audit/image-source-configuration`，是源码驱动 WPF 运行证据，不是原厂桌面差分。
