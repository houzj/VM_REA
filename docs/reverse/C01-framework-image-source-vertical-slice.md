# C01 本地纵向切片运行报告索引

普通本地图像源的主界面操作链已另行交付： [C01 图像源主界面操作链](C01-image-source-host-operations-20261111.md)。

以下为历史报告中的本地实现观察。[既有运行报告](../../analysis/full-audit/batch1-20261107/C01/runtime-report.json)

- ImageSource 两次执行均记录九个输出名，包括 Image、Width、Height 和 PixelFormat。
- Blob 路径记录 47 个结果、TotalArea 973202，重载后计数仍为 47。
- Group 路径在重载前后均记录 image、width 两个输出名，失败用例没有输出。

报告中的样图和方案使用原机器绝对路径。原复跑工程 `tests/VMVerticalSlice.C01/VMVerticalSlice.C01.csproj` 在当前工作树、已检索的 Git 历史和 `analysis.zip` 中均未找到。归档构建 `7d65a1e1` 的 18 项结果独立保留。

## 当前工作树复跑

使用存档三份 VMX，将图像源参数中的旧绝对路径替换为同 SHA-256 的 `1.jpg`。生成的方案写入独立目录。ImageSource 两次执行、Group 图像和宽度、保存重开及空输入失败共 **10 项通过**。1280×960 Mono8 像素 SHA-256 为 `0DB38D6CEF0FDDC6CEBECD13BA49A3AFBBA76E0C58DA4B63322BA73ED5FDED47`，与 C03 直接 SDK 的解码缓冲一致。

Blob 方案在当前注册表校验时返回 `Unknown module: vision.blob@1.0.0`。当前 C01 纵切的 Blob 环节处于阻塞。需要恢复或接入历史方案引用的工具后再复跑该环节。

```powershell
dotnet run -c Release --project tests/C01.ReplayChecks/C01.ReplayChecks.csproj -- analysis/full-audit/batch1-20261107/C01 'D:/MyWork/测试图片/标定/1.jpg' analysis/full-audit/batch1-20261107/C01/current-replay-next
```

最后一个参数应为新目录。报告记录样图与存档方案的身份、通过项和 Blob 校验结果。此次汇总 SHA-256 为 `12D3CB7B03B65D54806EC812FA24AD0F89FF6FABBF3F08DECFD77FE0BB2BD583`。[当前复跑工程](../../tests/C01.ReplayChecks/C01.ReplayChecks.csproj) · [当前结果](../../analysis/full-audit/batch1-20261107/C01/current-replay-3f9071c00aae43ab9e48c74c16b760a0/replay-final/summary.json)

## Blob 工具交接

存档方案的 Blob 节点要求 `vision.blob@1.0.0`，其 `Image` 输入连接图像源的 `Image` 输出。已检索本地与现有远端跟踪引用中的 `src` 和 `tests`，没有找到提交过该模块 ID 的源码记录。工作树和已构建 Release 的工具目录都将 `IMVSBlobFindModu` 显示为目录项，`ModuleId=null`，且无 `Implementation`。当前 Release 的插件程序集清单与无界面注册结果分别保存在审计和复跑汇总。

本机原厂 `IMVSBlobFindModu.dll` 没有 CLR 元数据。`IMVSBlobFindModuCs.dll` 的程序集引用仅有 `mscorlib`、`VM.Core` 和 `VM.PlatformSDKCS`。`ManagedToolPluginLoader.Load` 要求选择的公开类型直接声明当前 `IVisionToolPlugin`，并核对 SHA-256、模块 ID 和版本。上述两件原厂 DLL 均无法满足这个入口的静态校验。[注册与原件审计](../../analysis/full-audit/batch1-20261107/C01/blob-registration-audit-20261111.json)

工具线交接需提供符合现有接口的 Blob 源码或固定产物、依赖、类型和版本，以及该工具的源码与运行证据。框架侧使用现有注册接口验证精确身份、Image 输入、运行结果与保存重开。由用户决定同步和接入时点。当前缺口定位到具体工具产物，公共加载接口是否存在工具专属消费障碍，待拿到产物后实测。
