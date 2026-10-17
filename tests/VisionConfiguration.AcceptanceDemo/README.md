# 公共配置测试渲染入口

此入口仅供框架内部覆盖检查，不是 N 点标定的完成验收程序。它使用原厂 N 点 XML、Group 原样 fixtures，以及产品中的公共模型、状态、标准控件、订阅控件与公共窗口；没有注册测试控件 factory，也没有使用通用 TextBox 兜底。

从工作树根目录运行：

```powershell
# 隐藏运行、逐页导出独立实现截图和覆盖报告
& tests/Start-VisionConfigurationAcceptanceDemo.ps1

# 主代理需要手工检查时，明确打开带“框架验收样例（无算法执行）”标题的窗口
& tests/Start-VisionConfigurationAcceptanceDemo.ps1 -Show

# 可选英文或手动输入初始模式
& tests/Start-VisionConfigurationAcceptanceDemo.ps1 -English
& tests/Start-VisionConfigurationAcceptanceDemo.ps1 -Manual
```

输出位于 `analysis/full-audit/configuration-acceptance-demo/`，包括逐页 PNG、coverage JSON、启动日志。fixtures 的 SHA-256 在运行前与原厂来源清单核对。`ShowInTaskbar=true` 是演示程序方便找到窗口的测试覆盖，不修改产品宿主策略。

参数后端只读写 XML 初始值的内存副本。它不运行算法、不保存标定文件、不实现插件自定义规则，也不假装连接真实订阅服务。缺少模板显示明确的 Unsupported 文本；订阅服务缺失显示诊断。N 点仍有 7 个未实现的叶，不能据这个样例宣布 N 点界面闭合。

覆盖清单：`docs/plugins/n-point-configuration-template-gaps.json`。公共配置对象桥和保存模型需求分别见 `configuration-object-binding-contract-evidence.json`、`configuration-save-file-dialog-contract-evidence.json`。
