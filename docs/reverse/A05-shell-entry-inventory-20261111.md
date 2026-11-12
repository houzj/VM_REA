# A05.1 壳层入口清点与数据驱动落地

日期：2026-11-11
单元：`A05.1`「完整菜单/工具栏/快捷键入口清点」
结论：菜单栏与工具栏已改为由原厂 `menuitems.xml` 驱动构建，50 个节点 / 42 个命令 / 15 个工具栏项 / 4 个分组全部覆盖，65 项断言通过；快捷键经全树检索确认原厂未提供定义，记为 not-applicable。

## 1. 证据

| 类型 | 路径 | 说明 |
|---|---|---|
| 菜单/工具栏定义 | `D:/MyWork/SunEyeVision_V2-param-window-datatree/docs/07-工具逆向/decompiled/HostShell/ExeEntry/menuitems.xml` | 15997 字节，SHA-256 `D9E9A1DA6AECA60D97129CB71E07E9F074ED049C09F5A88F168608A01F6E7959` |
| 中文文案 | `.../decompiled/Lang/zh-cn.xaml`（839881 字节） | 逐键抽取，非整体搬运 |
| 入口文案原文校核 | `.../decompiled/HostShell/ExeEntry/resources/zh-cn.xaml` | 早期副本，键较少，仅作交叉核对 |
| 实现 | `src/VisionEditor/Resources/shell-menuitems.xml`（逐字节副本）、`ShellMenuCatalog.cs`、`ShellCommandRouter.cs`、`MainWindow.ShellMenu.cs`、`Styles/ShellMenuStrings.xaml` | — |
| 验证 | `tests/VisionEditor.ShellMenuChecks` | 65 项断言 |
| 视觉证据 | `analysis/full-audit/shell-menu-*.png` | 由检查工程渲染 |

### 1.1 快捷键结论（not-applicable）

对 `decompiled/HostShell` 全树（`*.xaml` + `*.cs`）检索 `InputBindings` / `KeyBinding` / `InputGesture` / `ShortcutKey`：**0 命中**；`menuitems.xml` 及全部 50 个节点也没有快捷键字段。因此本单元快捷键部分判定为「原件未提供」，不自行发明键位。

## 2. 目录规模（由 `ShellMenuCatalog.Verify()` 强制断言）

| 项 | 数量 |
|---|---|
| `TitleMenueItemData` 节点元素 | 50（含根节点） |
| `ExternTitleMenueItemData` 外部工具 | 5 |
| 去重 `methodName` | 42（39 内部 + 3 外部启动） |
| 工具栏挂载项（含 `toolGroupItemInfo`） | 15 |
| 工具栏分组 | 4（group0–group3） |
| 一级菜单 | 5 |

## 3. 菜单栏逐项清单

图例：`I`=已实现，`U`=未接入（禁用并给出原因），`D`=动态子项，`—`=其后有分隔线（`isShowSeparator`）。

### 3.1 文件（Lang_File = 文件）

| 序 | 文案 | 原厂命令 | 状态 |
|---|---|---|---|
| 1 | 新建方案 | CreateNewSolution | I |
| 2 | 打开方案 | OpenSolution | I |
| 3 | 最近打开方案 | dynamicGetItemsMethodName=GetRecentlySolutions | D |
| 4 | 打开示例 — | OpenSamples | I |
| 5 | 保存方案 — | SaveSolution | I |
| 6 | 方案另存为 | SaveAsSolution | I |
| 7 | 导出算子代码 — | ExportAlgorithm | U |
| 8 | 导入流程 — | ImportProcess | U |
| 9 | 退出 — | Exit | I |

### 3.2 设置（Lang_Set = 设置）

| 序 | 文案 | 原厂命令 | 状态 |
|---|---|---|---|
| 1 | 权限设置 | AuthSet | U |
| 2 | 软件设置 | VMSet | U |
| 3 | 方案设置 | Solset | U |
| 4 | 运行策略 | ProcRunSet | I |
| 5 | 采集策略 | CollectSet | U |

### 3.3 工具（Lang_Tools = 工具）——原厂此处是外部工具启动器，不是编辑命令

| 序 | 文案 | 原厂命令 | 外部程序（`info`） | 状态 |
|---|---|---|---|---|
| 1 | 标定配置引导 | CreateCalibGuide | — | U |
| 2 | 深度学习训练工具 | OpenExe | `DeepLearningTool\Hik.VM.DeepLearningTool.exe` | U |
| 3 | 运行环境检测工具 | OpenExe | `Tools\EnvironmentDetectionTool.exe` | U |
| 4 | Calibration Board Generate Tool | OpenExeByLang | `Tools\CalibrationBoardGenerationTool\DemoGenCalBoard_{ch,en}.exe` | U |
| 5 | 运行异常采集机器人工具 | OpenExe | `Tools\VMCollector\VMCollector.exe` | U |
| 6 | 查找表工具 | OpenModuleParamTool | `Tools\VMCollector\VMCollector.exe` | U |

### 3.4 系统（Lang_System = 系统）

| 序 | 文案 | 原厂命令 | 菜单可见 | 状态 |
|---|---|---|---|---|
| 1 | 日志 | LogShow | 是 | U |
| 2 | 通信管理 | CommSet | 是 | U |
| 3 | 控制器管理 | LightSet | **否**（`isDisplayInMenu=false`，仅工具栏） | U |
| 4 | 相机管理 | CameraSet | 是 | U |

### 3.5 帮助（Lang_Help = 帮助）

| 序 | 文案 | 原厂命令 | 状态 |
|---|---|---|---|
| 1 | 语言 → 中文 / English | ChSet / EnSet（节点名为 `%中文%` / `%English%`，百分号为按字面显示标记） | U |
| 2 | 帮助文档 | HelpDoc | U |
| 3 | 帮助文档（在线） | OnlineHelpDoc | I |
| 4 | 版本信息 | VerInfo | I |
| 5 | 更多支持 | MoreInfo | U |
| 6 | V社区 | OpenVClub | U |
| 7 | 打开欢迎页 | WecPage | I（复用方案中心窗口） |

### 3.6 独立入口（本批新增，非原厂）

`isDisplayInMenu=false` 的隐藏组承载 10 个工具栏命令；本项目另有 4 个原厂没有的入口，统一收进显式的「独立入口」菜单，避免污染原厂五菜单：

| 文案 | 说明 |
|---|---|
| 所选模块参数 | 打开当前选中模块的参数窗口 |
| 流程校验 | 调用 `GraphValidator` |
| 适应画布 | 画布 FitToContent |
| 模块目录 | 打开工具箱分类选择 |

## 4. 工具栏逐项清单

分组与组内顺序由 `toolGroupItemInfo(groupName, priority)` 决定，组间插入 1px 竖分隔线（共 3 条）。运行组 `isGroupData=True` 展开为两个按钮，因此 15 个挂载项渲染为 **16 个按钮**。

| 组 | 序 | 文案 | 原厂命令 | 状态 | 图标（本仓库） |
|---|---|---|---|---|---|
| group0 | 0 | 保存方案 | SaveSolution | I | `toolbar_toolbarSave_normal.png` |
| group0 | 1 | 打开方案 | OpenSolution | I | `toolbar_toolbarOpen_normal.png` |
| group0 | 2 | 撤消 | Undo | I | `toolbar_ic_topbar_undo_normal/disable.png` |
| group0 | 3 | 重做 | Redo | I | `toolbar_ic_topbar_redo_normal/disable.png` |
| group0 | 4 | 锁定 | AreaLocking | I | `toolbar_ic_topbar_lock_normal.png`（锁定时换 unlock） |
| group1 | 0 | 相机管理 | CameraSet | U | `toolbar_ic_topbar_camera_normal.png` |
| group1 | 1 | 控制器管理 | LightSet | U | `toolbar_ic_topbar_视觉控制器_normal.png` |
| group2 | 1 | 全局变量 | GlobalVariable | U | `toolbar_ic_全局变量_normal.png` |
| group2 | 2 | 通信管理 | CommSet | U | `toolbar_ic_通信管理_normal.png` |
| group2 | 3 | 全局触发 | GlobalTrigger | U | `toolbar_ic_topbar_全局触发_normal.png` |
| group2 | 4 | 全局脚本 | GlobalScript | U | `toolbar_ic_script_normal.png` |
| group3 | 0 | 单次执行 | SigRun | I | `toolbar_toolbarStart_normal.png` |
| group3 | 1 | 连续执行 | MutiRun（Run 组子项） | I | `toolbar_toolbarRunRepeat_normal.png` |
| group3 | 1 | 停止执行 | StopRun（Run 组子项） | I | `toolbar_toolBarStop_normal.png` |
| group3 | 2 | 综合配置 | ComprehensiveConfig | U | `toolbar_ic_topbar_综合配置_normal.png` |
| group3 | 3 | 编辑运行界面 | EditFrontendUI | U | `toolbar_ic_topbar_fronted_normal.png` |

工具栏 `isEnableByRun` 标记（运行中禁用）保留在目录中，未在本批引入额外运行时状态机；实际启用由既有 `RefreshToolbarState` 与文档历史继续决定。

## 5. 命令接线统计

| 状态 | 数量 | 命令 |
|---|---|---|
| 已实现 | 17 | CreateNewSolution, OpenSolution, OpenSamples, SaveSolution, SaveAsSolution, Exit, Undo, Redo, AreaLocking, AreaUnLocking, SigRun, MutiRun, StopRun, ProcRunSet, VerInfo, OnlineHelpDoc, WecPage |
| 未接入 | 25 | AuthSet, ChSet, EnSet, CollectSet, Solset, VMSet, ComprehensiveConfig, EditFrontendUI, GlobalVariable, GlobalScript, GlobalTrigger, CommSet, CameraSet, LightSet, LogShow, HelpDoc, MoreInfo, OpenVClub, ExportAlgorithm, ImportProcess, UndoList, CreateCalibGuide, OpenExe, OpenExeByLang, OpenModuleParamTool |

未接入项的处置：菜单项/按钮**可见但禁用**，灰色前景 + `ToolTipService.ShowOnDisabled=true`，ToolTip 为「未接入：<依赖能力>」；不创建占位窗口、不静默吞点击。原因表见 `src/VisionEditor/ShellCommandRouter.cs` 的 `ShellCommandReasons`。

## 6. 实现结构

| 文件 | 职责 |
|---|---|
| `Resources/shell-menuitems.xml` | 原厂原件逐字节副本（哈希一致），随构建复制到输出根目录 |
| `ShellMenuCatalog.cs` | XDocument 解析为强类型节点树；`Verify()` 强制 50/42/15/5/5/4 六项数量 |
| `ShellCommandRouter.cs` | 42 命令 → `(状态, 动作, 原因)`；`VerifyCoverage()` 保证目录中每个命令都能落地 |
| `MainWindow.ShellMenu.cs` | 生成菜单栏、四个工具栏分组、运行组展开、最近方案动态子菜单、独立入口菜单 |
| `Styles/ShellMenuStrings.xaml` | 只用到的语言键，逐键取自原厂 `Lang/zh-cn.xaml` |

`BuildChrome()` 已从「手写菜单/工具栏」改为 `BuildShellChrome()` 单调用；原 `BuildFileMenu` / `Menu(...)` / `Tool(...)` 保留供其他 partial 与历史断言使用。

## 7. 与原厂文案的两处差异（已按原厂修正）

| 位置 | 本批之前 | 现在（原厂） | 依据 |
|---|---|---|---|
| 工具栏撤消 | 撤销 | **撤消** | `Lang_Undo` |
| 锁定按钮 | 锁定工作区 / 解锁工作区 | **锁定 / 解除锁定** | `Lang_Locking` / `Lang_Unlock` |

`tests/VisionEditor.ShellChecks` 中对应的 6 处标题断言已同步更新为原厂文案（不是删除断言），并保留同等强度。

## 8. 验证

| 项 | 结果 |
|---|---|
| 隔离构建 | 0 警告 0 错误 |
| `tests/VisionEditor.ShellMenuChecks` | 退出码 0，**65 项断言全过**：目录六项数量、菜单栏六根顺序、文件 9 项与 5 条分隔线、设置 5 项、工具 6 项全部禁用、系统 3 项（控制器管理按原厂隐藏）、帮助 7 项含语言子菜单、独立入口 4 项、工具栏 16 按钮 / 3 分隔线 / 顺序 / 分组启用态、17 已接入 + 25 未接入均带原因 |
| `tests/VisionEditor.ShellChecks` 回归 | 改动前后**同一条既有失败**（第 275 行 `Save menu persists and orders recent solution paths`，F1：构造期启动方案写入最近列表使条目数变 3），前面 44 项 PASS 一致，零新增失败 |
| 视觉 | 菜单栏与工具栏由检查工程渲染截图，位于 `analysis/full-audit/shell-menu-current.png` |

## 9. 未闭合

| 编号 | 内容 | 说明 |
|---|---|---|
| E1 | 全程序集入口穷尽 | 本单元只覆盖 `menuitems.xml` 定义的菜单/工具栏入口；画布右键菜单、节点右键、工具箱悬停等其它入口未纳入穷尽清单 |
| E2 | 真实键鼠与多屏/DPI | 菜单弹出与点击未做真实输入命中矩阵（与 A08.2 同一个失败夹具问题） |
| E3 | `isEnableByRun` 运行时语义 | 字段已解析，未接入「运行中禁用」状态机 |
| E4 | 外部工具启动 | 6 个外部程序未随附，`OpenExe*` 一律禁用 |
| E5 | 语言切换 | `ChSet`/`EnSet` 未实现，仅保留中文字典 |
| E6 | 工具栏 `DownPopTemplate` | 撤消按钮的「历史记录」下拉（UndoList）未实现 |
