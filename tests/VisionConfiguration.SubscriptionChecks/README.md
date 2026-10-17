# 公共订阅树及选择器检查

运行：`dotnet run --project tests/VisionConfiguration.SubscriptionChecks`。

源依据见 `docs/plugins/configuration-subscription-evidence.json`：19 条原厂源码片段，涵盖连接图前驱、父容器输入、私有/可订阅/类型过滤、层级构建、两类绑定、索引、用户态持久化及原厂树控件；3 张链按钮图与原厂资源逐字节相同。测试直接消费相邻 ModelChecks 跟踪的原厂 NImage XML，服务为明确的受控独立实现，不是原厂 VM runtime。

检查层级树、叶对象的稳定身份、同名 caption 的不同来源、输入关联和参数订阅分离、typed 常量和索引、显示文字不可解析为引用、失败/缺失服务不伪造绑定、参数订阅不覆盖数字值、空绑定可保留订阅编辑模式、`Param.` 与 `@` 用户态键分离，以及 ClearCalibPoint/ContinuousSelfExcuteModule/StopSelfExcuteModule 的显式 typed 方法分派。

选择器使用文本框、链式弹层及 180×180 TreeView。新来源只从服务给出的 typed reference 选择；直接文本输入只允许 typed 常量，或在保留既有 typed reference 时编辑数字索引。原厂直接输入模块路径、独立变量管理器及高级订阅窗口不在此控件已支持范围，绝不通过解析翻译后的显示文字补身份。完整原厂 UI 像素与原厂完整 VM 运行未验证。

宿主对未有元数据的 Global/Index/父容器/Private/CanSubscribe 必须返回 Unsupported 或列出 UnsupportedCapabilities；合同支持不代表宿主已提供这些能力。控件在服务缺失时保留明确诊断，不能把空树当原厂无可订阅项。
