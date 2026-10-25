# 独立文档生成与增量维护

写入边界只有本目录及 `docs/架构逆向与实现对照文档/`。不运行原生成链，不替换根目录的 `framework-architecture.html`。

当前捕获基线存于站点的 `清单/拆分前基线.json`。全量 `node tests/ArchitectureRecovery/DocumentSplit/build.cjs` 会重新读取活动输入；主对话仍在写源文档时不要用它更新已有交付。它也不会自动重放后续的场景拆分。

仅更新公共资源：

```powershell
node tests/ArchitectureRecovery/DocumentSplit/build.cjs --assets-only
node tests/ArchitectureRecovery/DocumentSplit/verify-navigation.cjs
```

仅重建一个已捕获专题的场景分页（不会扫描其它专题或反编译证据）：

```powershell
node tests/ArchitectureRecovery/DocumentSplit/split-scenes.cjs --topic=image
node tests/ArchitectureRecovery/DocumentSplit/split-scenes.cjs --topic=group
node tests/ArchitectureRecovery/DocumentSplit/split-scenes.cjs --topic=retirement
node tests/ArchitectureRecovery/DocumentSplit/split-scenes.cjs --topic=global
node tests/ArchitectureRecovery/DocumentSplit/verify-scenes.cjs
node tests/ArchitectureRecovery/DocumentSplit/build-stop-state.cjs
node tests/ArchitectureRecovery/DocumentSplit/verify-stop-state.cjs
```

也可使用有界更新入口 `node tests/ArchitectureRecovery/DocumentSplit/update.cjs --topic=owner`。可选 image、group、retirement、global、assets、state、owner；只执行所选专题、相关图及首页维护，并记录实际命令/退出码。它不会运行全站原生成链；最终交付仍须单独执行全站静态验证。

每条命令只选择一个专题。第一次运行固定该专题分页前 HTML 到 `历史/分页输入-*.source`，后续使用该固定输入，完整复制匹配的平衡 details 块。证据正文不改写；更新引用位置与清单。来源语义发生变化时，需显式评审新输入，不能删除快照后静默把新内容当旧基线。

最后更新首页交付信息并串行检查；检查运行期间不要修改站点：

```powershell
node tests/ArchitectureRecovery/DocumentSplit/refine.cjs
node tests/ArchitectureRecovery/DocumentSplit/verify.cjs
```

`verify.cjs` 覆盖文件 hash、证据块、路由回放与文件链接；`verify-scenes.cjs` 覆盖22个场景完整块保真和旧锚点占位；`verify-navigation.cjs` 是 VM DOM 替代环境操作检查，包含迁移锚点的查询和 hash 保留。三者均不证明浏览器呈现、打印、键盘或历史验收通过。

`build-stop-state.cjs` 只读取已捕获的停止执行条目，生成原版全局脚本 setter 的专用分支状态图。`verify-stop-state.cjs` 校验来源记录 SHA、事实全文、图与模型节点/边一致及开始/停止分支隔离，不证明原生运行。

`verify-anchors.cjs` 逐条检查7056条路由目标片段。`repair-route-targets.cjs` 使用已捕获的 `.edges.json` 为动态调用边生成独立全文证据页，并明确保留不能匹配的登记字符串；没有待修项时保留已有修复记录。`verify-edge-targets.cjs` 逐页核对919个调用边正文的完整 JSON、来源 SHA 与锚点。41条未解析登记不计语义兼容通过，锚点存在不等于浏览器运行通过。

剩余明确工作：全局变量与执行专题的场景粒度；针对数据流、Owner 等关系的图形表达及来源校验；实际浏览器验收和主入口集成方案。现有通用关系投影不作为这些项目完成的证明。遵守先前浏览器 file:// 拒绝，不使用 localhost 或替代浏览器绕过。
