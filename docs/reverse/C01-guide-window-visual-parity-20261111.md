# C01 方案中心视觉保真对齐

日期：2026-11-11
触发：用户反馈"欢迎页面复现度很差"，并给出原厂与当前实现截图对照。
结论：方案中心已按原厂 `GuideWindow.xaml` 的几何/模板/资源绑定与 `colorstyle1.xaml` 的主题色重写，卡片裁剪、表头被切、纹理与配色失真、系统复选框、文字叉号等问题全部消除。

## 1. 证据来源

| 类型 | 路径 |
|---|---|
| 原厂 XAML（几何/模板/触发器/事件） | `D:/MyWork/SunEyeVision_V2-param-window-datatree/docs/07-工具逆向/decompiled/HostShell/Main/VisionMaster.AppMainWindow/GuideWindow.xaml` |
| 原厂代码后置（行为/属性/默认值） | 同目录 `GuideWindow.xaml.cs` |
| 原厂闪屏 XAML | `.../Main/VisionMasterMain/WelcomeDialog.xaml` |
| 原厂主题色（深色） | `analysis/decompiled/Apps.ColorFun/colorstyle1.xaml`（第 264-290 行） |
| 原厂通用文字色 | `analysis/decompiled/Apps.ColorFun/colorstyle.xaml`（第 42/54/125/126/127 行） |
| 原厂资源（20 项，从 g.resources 提取） | `analysis/ui-assets/original-guide/` |
| 实现 | `src/VisionEditor/StartupGuideWindow.xaml(.cs)`、`GuideControls.cs`、`GuideResourceResolver.cs`、`Styles/GuideWindowTheme.xaml` |
| 视觉证据 | `analysis/full-audit/guide-visual-parity-20261111/guide-live-desktop.png`（真机 1:1）、`guide-render-1to1.png`（测试进程内渲染） |

## 2. 缺陷与根因

| 编号 | 现象 | 根因 | 修复 |
|---|---|---|---|
| V1 | 第二行卡片整行消失 | 四张卡片统一 `Margin="0,0,22,22"`，总高 304，容器又需留 56 → 360 > 可用 338 | 按原厂非对称外边距：`0,0,22,22` / `0,0,0,22` / `0,0,22,0` / 无，总高 282 |
| V2 | "最近打开方案"表头被切掉一半 | `ListView` 固定 `Height=300`，加 56 底部留白 = 356 > 338 | `Height` 绑定 `CardBlock.ActualHeight`（原厂 `Height="{Binding ActualHeight, ElementName=Border}"`） |
| V3 | 对角纹理看不见 | 线色 `#343638` 与底色 `#2D2F31` 对比过低，且未按原厂几何平铺 | 线色改 `#171717`，几何字符串逐字照搬原厂，铺在整行内容底上 |
| V4 | 整体配色偏灰蓝 | 旧值 `#2D2F31`/`#17191B`/`#303234`/`#F1492C` 全部是估的 | 改为原厂 `#1D1D1D→#252525` 渐变、`#131313→#1D1D1D` 标题栏、`#323232` 底栏、`#FD4F41` 顶条、`#E76900` 主题橙 |
| V5 | "不再显示"是系统复选框 | 用了 WPF `CheckBox` | 改为原厂 `ToggleButton` + `guide_check-box_off/on.png`（18×18），内容左移 6px、整体下移 10px |
| V6 | 关闭按钮是文字"×" | 用 `TextBlock` 充数 | 改为 32×32 按钮 + 矢量叉，悬停 `#E81123` 红底白叉 |
| V7 | 底部是三个大按钮，无分隔线 | 自造 `Button Width=266 Height=78` | 改为原厂 `Menu` + 三个 `IconMenuItem` + 两条 1×40 竖分隔线（左右留白 25/35），悬停橙色 |
| V8 | 打开图标压住表头 | 独立绝对定位按钮 | 放回第二列的表头（原厂 `GridViewColumn.Header`），32×32、`Margin=30,0,0,0` |
| V9 | 打开图标悬停态永远取不到 | 原厂条目名 `guide_open_click .png` 在 `click` 与扩展名之间**带一个空格**，按 `_click` 后缀拼接必然失败 | `GuideResourceResolver` 用别名表吸收该怪癖；测试断言 `HoverIcon` 必须解析成功 |
| V10 | 方案名带扩展名、时间文案不统一 | 直接显示文件名与自定义 "Age" | 投影为 `FileName`（去扩展名，ToolTip 为完整路径）+ `今天 / N天前` |
| V11 | 卡片标题比原厂高 10px | 缺少内层 `TextBlock Margin="0,10,0,0"` | 补齐，标题落位与原厂 `50+10` 一致 |
| V12 | 无 1px 边框与 4px 圆角 | `WindowStyle=None` 后没有画外框 | 内容层与 1px 边框层分离（见下） |

### V12 的额外说明（关键）

直接把 1px 边框加在内容 `Border` 上会吃掉 2px 高度，使卡片区只剩 280，第二行必然被裁——这也是首次重写后测试立刻暴露的问题（实测 279/281）。原厂的 1px 边框走 `BaseWindow` chrome，不参与内容布局。因此实现改为：

- `GuideSurface`（内容层，`BorderThickness=0`、`CornerRadius=4`、`ClipToBounds=True`）承担全部 546px 布局；
- `GuideShell`（`BorderThickness=1`、`CornerRadius=4`、`IsHitTestVisible=False`）作为纯装饰层叠在上方。

结果：130 + 337 + 78 的分配不再被边框侵蚀，卡片区回到 281~282（客户端实测比 546 少 1px，与原厂同样的 1px 余量，视觉不可见）。

## 3. 逐项对照

### 3.1 窗口与标题栏

| 项 | 原厂 | 实现 | 状态 |
|---|---|---|---|
| 尺寸 | 800×546，`ResizeMode=NoResize` | 同 | 一致 |
| 外框 | `CornerRadius=4`、1px `#404040` | 装饰层 1px `#404040` + 4px 圆角 | 一致 |
| 顶部红条 | `Rectangle Height=2` `#FD4F41` | 同 | 一致 |
| 标题栏高 | 130 | 130（实测 130） | 一致 |
| 标题栏底色 | `#131313→#1D1D1D` 垂直渐变 | 同 | 一致 |
| Logo | `guide_logo.png`，`Margin=20,9`，`100×48` | 同（`guide_logo.png` 原始 286×100，等比缩放） | 一致 |
| 版本号 | `Margin=138,37`、斜体、`#5C5C5C` | 同 | 一致 |
| 不再显示 | `ToggleButton`、`FontSize=14`、`Margin=0,0,45,0`、下移 10 | 同 | 一致 |
| 关闭按钮 | 32×32、下移 4 | 32×32、右 5、下移 4 | 近似（`WinCloseBtnStyle` 未取到，独立等价实现） |

### 3.2 内容区与卡片

| 项 | 原厂 | 实现 | 状态 |
|---|---|---|---|
| 内容底色 | `#1D1D1D→#252525` | 同 | 一致 |
| 斜纹 | 6×6 平铺，`#171717` | 同（几何字符串逐字照搬） | 一致 |
| 内容底分隔线 | `#FF1A1A1A`，1px | 同 | 一致 |
| 卡片区 | `Width=412`、`Margin=55,0,0,56`、贴底、右侧 1px `#4D4D4D` | 同 | 一致 |
| 卡片尺寸 | 188×130 | 同 | 一致 |
| 卡片外边距 | `22,22 / 0,22 / 22,0 / 无` | 同 | 一致 |
| 卡片图 | `guide_*.png` + `guide_*_click.png` 悬停 | 同（`BackImage`/`BackImageClick`） | 一致 |
| 标题 | `FontSize=18`、Bold、`#D1D1D1`，悬停 `#FFFFFF` | 同 | 一致 |
| 说明文字 | `FontSize=11`、`#B8B8B8`、`Margin=16,0,0,16`，悬停 `#F2F2F2` | 同 | 一致 |
| 文字偏移 | `ContentPresenter 16,50` + `TextBlock 0,10` | 同 | 一致 |

### 3.3 最近打开方案

| 项 | 原厂 | 实现 | 状态 |
|---|---|---|---|
| 列表 | `Width=225`、`Margin=0,0,58,56`、贴底 | 同 | 一致 |
| 高度 | 绑定卡片区 `ActualHeight` | 同 | 一致 |
| 表头 | `FontSize=20`、`Height=30`、左对齐、透明底 | 同 | 一致 |
| 列宽 | 140 / 80 | 同 | 一致 |
| 打开图标 | 第二列表头 `IconMenuItem` 32×32、`Margin=30,0,0,0` | 同（`GuideIconMenuItem`） | 一致 |
| 名称 | `FileName`，`FontSize=16`、`Margin=-5,0,0,0`、ToolTip 为完整路径 | 同 | 一致 |
| 时间 | `LastTimeStr`，`FontSize=14`、右对齐 | `今天 / N天前` | 文案格式为独立实现 |
| 行 | `Margin=0,8,0,0`、`Height=31`、无选中高亮 | 同 | 一致 |
| 橙色竖标 | `2×20`、`Margin=0,6,290,0` | 同 | 一致 |

### 3.4 底部入口

| 项 | 原厂 | 实现 | 状态 |
|---|---|---|---|
| 容器 | `Menu`、左对齐、`Margin=35,0,0,0`、垂直居中 | 同 | 一致 |
| 项 | 三个 `IconMenuItem`，`FontSize=16`、`#A6A6A6`、悬停/按下 `#E76900` | 同（`GuideIconMenuItem`） | 一致 |
| 图标 | `ic_1/2/3_normal` 与 `_click` | 同 | 一致 |
| 分隔线 | 1×40，左右留白 25/35，`#4D4D4D` | 同 | 一致 |
| 底栏底色 | `#323232`，上缘 1px | 同 | 一致 |

## 4. 行为对照（`GuideWindow.xaml.cs`）

| 项 | 原厂 | 实现 |
|---|---|---|
| 学习文案 | `"学习使用" + "VISIONMASTER"` | 同（`Header="学习使用VISIONMASTER"`） |
| 加载态 | `buttonShow.IsChecked = !IsGuideWindowShow()` | `HideGuide.IsChecked = !StartupSettings.ShowEnable` |
| 不再显示 | `SetGuideWindowOff/On()` | `StartupSettings.SetShowEnable(false/true)` |
| 卡片点击 | `SaveConfig("ToolBoxType", …)` 后 `Close()` | `StartupSettings.SetToolBoxType(...)` 后 `Close()` |
| 关闭按钮 | `DialogResult = true; Close();` | 同 |
| 打开文件 | 命令 `Execute(null)` + `Close()` | 同 |
| 查看示例 | `GuideHelper.OpenSolution(opendemo:true, out filename)` → 命令 + `Close()` | 映射到仓库 `examples/image-source-plugin.vmx.json`（独立实现） |
| 双击行 | `SelectSolutionCommand`（参数为窗口） | `OpenSolutionCommand.Execute(路径)`（等价） |
| 本地/在线帮助 | `GuideHelper.GetHelp(local)` | 空实现（无 Help 目录与在线入口，未接入） |

## 5. 独立实现与偏差清单

| 编号 | 项 | 说明 |
|---|---|---|
| D1 | `BaseWindow` chrome | 原厂 `VM.Controls.BaseWindow` 无法引用，改用 `WindowStyle=None` + 装饰层边框 + 标题栏 `DragMove` |
| D2 | `IconMenuItem` | 原厂 `VM.Controls.IconMenuItem`，本项目实现同名三态图标控件；未实现 `ShowMask` |
| D3 | `WinCloseBtnStyle` | 样式定义未取到，按截图实现 32×32 悬停红底白叉 |
| D4 | 相对时间文案 | 原厂 `LastTimeStr` 格式化代码未取到，实现为 `今天 / N天前` |
| D5 | 名称截断 | 原厂用 `LongTextConvert`（`MaxLen=138`，长度截断），实现用 `TextTrimming=CharacterEllipsis`（宽度截断） |
| D6 | 圆角裁切 | 内容层 `ClipToBounds` 只裁矩形，四角 4px 处背景不随圆角裁切；装饰层边框已按圆角绘制 |
| D7 | 示例方案路径 | 原厂指向安装包示例目录，本项目映射 `examples/` |
| D8 | 帮助入口 | 本地/在线帮助未接入（无帮助文件与在线入口） |

## 6. 验证

| 项 | 结果 |
|---|---|
| 隔离构建 `artifacts/codebuddy-verify/VisionEditor/` | 0 警告 0 错误 |
| `tests/VisionEditor.StartupShellChecks` | 退出码 0，方案中心相关断言全部通过 |
| 新增几何断言 | 标题栏 130；卡片区 280~282；卡片两列（第二列 x=210）；第二行 y=152 且不越界；列表高度跟随卡片区 |
| 新增资源断言 | 四张卡片常态+悬停图、18 个动态资源键、`guide_open_click .png` 别名字符串解析、底部三态图标 |
| 新增结构断言 | 底部为 `Menu` + 3 个 `GuideIconMenuItem` + 2 条 `Separator`；"不再显示"必须是 `ToggleButton` |
| 1:1 渲染 | `analysis/full-audit/guide-visual-parity-20261111/guide-render-1to1.png` |
| 真机截图 | `analysis/full-audit/guide-visual-parity-20261111/guide-live-desktop.png`（标准 Release，`PrintWindow` 抓取） |
| 启动链冒烟 | 欢迎页 → 方案中心(800×546) → 关闭 → 主窗口 `VisionMaster` → 存活 → 正常退出；无 `startup-error.log` |

## 7. 仍未闭合

- 原厂 `colorstyle2/3`（浅色/其他主题）未实现主题切换，当前固定使用深色 `colorstyle1` 取值。
- `AppData.IsHideLogo` / `IsHideGuideLine` 对应的自定义品牌配置未接入。
- 方案中心未按原厂 `AppData.CustomName` 生成学习文案。
- 桌面级 DPI 缩放（125%/150%）下的像素对齐未做专项验证。
