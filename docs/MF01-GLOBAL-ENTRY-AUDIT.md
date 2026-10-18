# MF-01 全局配置入口审计（未实现）

本轮核对主窗口仍缺的全局变量、通信管理和全局触发。三个入口均不是纯 Shell 窗口：原始 `VisionMaster.AppMainWindow/TitleMenueItemData.cs` 中 `GlobalVariable`、`CommSet`、`LoadGlobalTrigger` 均依赖单独的原生全局模块 DLL、`VmSolution` 内的模块实例、权限/配置可用检查，再把模块传给专用 WPF 配置窗口。`VMControls.WPF.GlobalUI` 中有对应窗口和配置类，但脱离其真实模块不会产生可运行或可持久化的数据。

当前主框架工具栏将三个入口标为未接入；`VisionPluginContracts` 尚无对应全局模块配置会话。依据上述链路，此轮没有启用按钮或创建占位配置窗，也没有声称功能闭合。跨线所需接口和生命周期登记在 `docs/CONTRACT-REQUESTS.md` 的 FCR-015。插件项目完成相关模块与会话后，Shell 再接可用状态、窗口调度和端到端验收。

证据分类：动态模块路径、模块 ID、专用窗口与权限检查为反编译确认；FCR-015 的抽象负载为待协商建议；原生方案持久化格式、SDK 权限反馈和实机窗口差分尚未验证。
