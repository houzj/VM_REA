# 双项目逆向开发与交接

## 当前职责（2026-10-17 用户最新决定）

用户自行管控两个独立项目。插件项目负责工具及其所需公共节点框架，主项目负责与具体工具无关的主框架。下面历史记录用于追溯，不覆盖本节；旧的“插件只能提框架请求”“管理线自动调度”不再作为当前职责规则。

| 项目 | 独占能力 | 交接边界 |
|---|---|---|
| tool-plugins 当前工作树 | 工具本体、原厂证据/资源、配置模型/状态/模板、节点调试与配置宿主、参数/订阅/ROI/显示适配、插件运行状态/执行适配与工具验证 | 通过公共服务使用主线的流程、Group、方案生命周期、持久化和注册入口；需要能力由本线实现，跨边界接口登记后接入 |
| 主框架项目 | 通用主界面、工作流/画布/Group、方案/流程生命周期、通用调度/持久化、发现装载与最终集成 | 提供稳定主程序接口并接入插件框架；不重复开发插件模型/模板/调试宿主 |

细分所有权与任务结构见 [PLUGIN-DEVELOPMENT-BOUNDARY.md](PLUGIN-DEVELOPMENT-BOUNDARY.md)。不能按程序集目录整体划归一线；同一个交叉文件只指定一位提交所有者。当前已分歧文件先审查存量候选，不得整文件覆盖或在未协调时追加另一套实现。

## 工作树与基线

- 本项目：`worktrees/tool-plugins`，分支 `tool-plugins`；只在本工作树修改。
- 主框架：原始工作目录，当前基线参考 `main 351c778`；由主框架对话自行修改。
- 本线 HEAD、共同祖先、分歧和安全同步条件见 [DEVELOPMENT-BASELINE.md](DEVELOPMENT-BASELINE.md)。当前尚未统一代码基线，不用文档同步冒充代码合流。
- 不强制 reset、clean、rebase、合并或跨目录修改另一线；用户人工验收前不提交当前产品候选。

## 每项插件任务必须一起安排

1. 原厂源码、资源、受控运行证据及未知行为。
2. 所需公共配置/模板/调试/参数/订阅/ROI/显示/执行能力，列对应 PF ID 与本线负责人。
3. 主框架接口与调用方、payload、身份、生命周期和错误语义；交叉文件/提交所有者登记 CONTRACT-REQUESTS。
4. 真实插件装入实际产品后的操作与运行验收；测试通过不等于原厂完全等价。

清单先闭合公共基础链路，再迁移 N 点/N 图像等工具；坐标系最后。不做设备测试。保留算法和历史证据，不以旧专属窗口检查替代新框架验收。

## 集成顺序

登记接口与单一文件提交所有者 → 冻结双方候选 → 用户手动验收 → 中文提交并推送各自分支 → 主线逐文件冲突审查/集成 → 实际 WPF 与旧方案兼容回归 → 记录双方统一提交基线。插件线不强行吸收 main 未提交代码，主线也不直接搬运本线未提交目录。

## Local evidence and reports
Original evidence remains in the original checkout's analysis directory. It is excluded from remote history. Build-required UI assets and sample image are tracked. Store new reports/build outputs inside the active worktree, not the other conversation's directory. Reports that require original binary evidence are not portable solely from Git.

## 2026-10-16 user-assigned integration exception
The user explicitly assigns framework adjustments required by plugin reconstruction to the plugin line. Implement and validate ImageSource-required shared changes in this worktree, record contracts and migrations in CONTRACT-REQUESTS.md, and keep the other worktree untouched. This supersedes the request-only ownership restriction for this work; it does not authorize merging or pushing the other branch.

The user extends that assignment to the next tool, ImageAcquisitionModule. Its necessary shared camera/light contracts, registration and multi-output previews are implemented here. The user subsequently excludes physical-device testing from this line: finish code from original source/resources and controlled regression evidence, without requesting devices or treating equipment availability as a delivery blocker. This does not establish native equivalence for unobserved behavior.

The user extends the same scope to ImageBufferModule and authorizes committing and pushing completed tool milestones from this branch, with Chinese commit descriptions. This does not authorize merging or changing the other worktree.

On 2026-10-17 the user assigns LightModule to the same plugin line, including necessary shared adjustments, excluding device testing, and explicitly requests a Chinese commit and push after this single-tool milestone closes. Global LightControl/CommManager resource services remain separately inventoried tools; LightModule implements its own execution/configuration and their typed calling boundary.

On 2026-10-17 the user prioritizes calibration: N-point first, N-image next, CoordinateModule last. Necessary shared adjustments and Chinese milestone commits/pushes remain assigned to this worktree. The user explicitly authorizes parallel reverse engineering of other calibration plugins. N-point and N-image use separate product/evidence/test files; shared contracts and host registration are coordinated by the primary agent. Other calibration dependencies are audited separately; no physical-device testing or changes to the other worktree.

The user subsequently explicitly requests more calibration plugins in parallel. Use all four available execution slots: primary N-point state/UI/integration, N-point numerical reconstruction, N-image reconstruction, and TranslationCalibModu reconstruction. Shared files stay coordinated by the primary; dedicated plugin/evidence/test files have separate owners. CoordinateModule remains last. Evidence milestones and partial implementations must not be presented as closed tools.


## 2026-10-17 framework-first correction

The user identified structural differences in node configuration architecture, rather than a per-tool visual issue, and explicitly prioritizes reverse engineering and correcting the framework before continuing calibration closure. Four lines now audit configuration schema/model, template dispatch, window lifetime, and command/subscription/ROI execution. Preserve existing numerical work without claiming UI closure. Required shared changes remain in this worktree; do not modify the other checkout. A tool milestone requires launching a clearly identified build for manual acceptance before its Chinese commit and push. Previously passing tests do not waive this acceptance or prove the old template architecture correct.
