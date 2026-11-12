# G5 ImageSource → 真实工具垂直切片

日期：2026-11-12

## 切片边界

本切片以当前工作区已有源码和资源证据为依据，闭合以下独立执行链：

```text
图像源参数/列表/目录订阅
  → ImageSource runtime
  → Image / PixelFormat / CurrentImagePath
  → ImageBinary 或 Blob 输入绑定
  → 执行连接与数据连接
  → 真实工具结果/状态/显示载体
  → 保存、重开、嵌入资源和错误恢复
```

工具注册和数据形状来自 `src/VisionPlugins.ImageSource`、`src/VisionPlugins.ImageBinary`、`src/VisionPlugins.Blob` 及 `VisionFramework` 合同。原厂 SDK/宿主 Blob 的数值差分仍由 C03 单独负责。

## 已验证行为

- 本地图像、RGB/Mono8、`OutMono8`、缓存和列表选择。
- 目录订阅创建/删除/修改/重命名、索引保持和关闭释放。
- 方案资源嵌入、删除外部源后的重开执行、SaveAs 和资源裁剪。
- ImageSource → ImageBinary 的图像类型边界和 Mask 输入。
- ImageSource → Blob 的当前 managed 组件结果、Mask、4/8 连通性和阈值模式边界。
- 新建方案、执行载体、保存重开和真实执行结果均通过同一图模型。

## 门禁结果

| 测试 | 结果 |
|---|---|
| `ImageSource.PluginChecks` | 773 项通过 |
| `FrameworkPluginChain.Checks` | 11 个图用例通过 |
| G4 生产配置/ROI 宿主 | 49 项通过 |

## 原厂边界

这些结果证明当前源码实现的垂直合同，不证明 VisionMaster 原厂等价。以下仍是“运行未确认”：

- 原厂 `.sol` 容器与方案直启载荷。
- 原厂 Blob 宿主的活动 ROI/Mask、Run 前像素和结果缓冲。
- 原厂 SDK 与宿主同输入的首处分歧归属。
- 原厂显示容器跨 Group/Tab 生命周期。

## 复跑

```powershell
dotnet D:\MyWork\Rea逆向\artifacts\image-source-g5\ImageSource.PluginChecks.dll
dotnet D:\MyWork\Rea逆向\artifacts\source-blob-g5\FrameworkPluginChain.Checks.dll
```
