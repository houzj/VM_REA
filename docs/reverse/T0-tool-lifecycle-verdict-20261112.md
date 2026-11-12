# T0 `tool-lifecycle-*` 目录判定（2026-11-12）

## 1. 判定对象

`src/VisionEditor/` 下三个哈希目录：

| 目录 | 创建时间 | 文件 |
|---|---|---|
| `tool-lifecycle-e93f30fcec9e4806ab947e288eea4f59` | 2026-11-12 14:17:17 | `own-catalog.json`（268 B） |
| `tool-lifecycle-d633aab970fa4d8eb1fb3fd22cca8984` | 2026-11-12 14:17:48 | `own-catalog.json`（268 B）、`summary.json`（234 B） |
| `tool-lifecycle-b50ec11880b7448391ae42baaa890bea` | 2026-11-12 15:17:45 | `own-catalog.json`（268 B）、`summary.json`（234 B） |

## 2. 判定性证据

1. 前两个目录出现在 Codex 交接批次（14:17），第三个目录**在 CodeBuddy 于 15:17 复跑 `ToolCatalogLifecycleChecks15` 之后立即出现**：测试 stdout 末行直接输出 `19/19; D:\MyWork\Rea逆向\src\VisionEditor\tool-lifecycle-b50ec11880b7448391ae42baaa890bea`。
2. 新目录 `summary.json` 内容：
   ```json
   {"checks":19,"root":"D:\\MyWork\\Rea逆向\\src\\VisionEditor\\tool-lifecycle-b50ec11880b7448391ae42baaa890bea","scope":"actual MainWindow routed control events and synthetic metadata; no original DLL runtime or desktop parity"}
   ```
   明确写有 `synthetic metadata`（合成元数据），并声明不含原厂 DLL 运行或桌面等价证据。
3. 每次运行生成不同哈希目录，内容字节数一致，属于按运行实例隔离的测试夹具输出。
4. `git status --short` 将三者列为未跟踪目录；无人将其加入跟踪，也无产品代码引用这些路径。

## 3. 判定

- **类别**：`ToolCatalogLifecycleChecks15` 的运行时临时产物/测试夹具输出，**不是交付物、不是原厂证据、不是产品资源**。
- **对交付的影响**：无。19/19 结果由测试断言与 stdout 证明，不依赖这些残留目录。
- **证据等级**：E4（独立实现证据，当前测试夹具），不构成 E0/E1 原厂证据。
- **是否可删除**：按接手计划「不删除历史证据、临时目录是否清理由用户决定」，CodeBuddy **不自行删除**这三个目录。
- **建议（待用户决定）**：把 `src/VisionEditor/tool-lifecycle-*/` 加入 `.gitignore`，或让 `ToolCatalogLifecycleChecks15` 在测试结束后清理自身夹具目录。二者都属于行为/配置变更，需用户确认且不属于 T0 范围。

## 4. 剩余未知

- `tool-lifecycle-e93f...` 缺 `summary.json`，可能是 14:17 那次测试被中断或提前退出；无日志佐证，保持 `unknown`，不推断原因。
- 14:17 两次运行的具体调用者与退出码未记录；只有目录时间戳与文件集合可证。

## 5. 被跟踪 UI 资产清单（对照）

与临时目录不同，以下资产在 `31acdb4c` 已跟踪，属于构建必需资源，不得与临时产物混为一谈：

- `analysis/ui-assets/original-guide/*`（原厂 Guide 卡片、图标、GIF）
- `analysis/images/circle-find-sample.bmp`（样图）

`.gitignore` 中 `analysis/*` 被排除，但含 `!analysis/ui-assets/`、`!analysis/ui-assets/**`、`!analysis/images/circle-find-sample.bmp` 例外。
