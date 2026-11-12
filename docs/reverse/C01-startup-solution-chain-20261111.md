# C01 启动与方案实体链

## 已定位的问题

启动入口原来直接加载安装目录下的 `demo.vmx.json`。这份文件只有一个图像源节点，没有持久化用户方案目录，也没有工具之间的数据连接和执行连接。方案对象虽然存在，用户启动后看到的是示例载荷，不能体现方案管理、工具载体和执行流的闭环。

## 当前实现

`MainWindow.LoadStartupSolution` 按以下顺序恢复方案。

0. 如果命令行提供了 `.vmx.json` 路径，先加载该方案，支持 `VisionEditor.exe solution.vmx.json` 和文件关联启动。
1. 优先读取最近打开且仍存在的方案文件。
   安装目录的 `demo.vmx.json` 不作为用户最近方案恢复，避免旧版本记录绕过持久化方案初始化。
2. 没有可恢复方案时，在 `%LocalAppData%/VisionEditor/Solutions/default.vmx.json` 创建用户方案实体。
3. 首次创建同时复制 `assets` 样例资源，保证相对路径在用户方案目录下可用。
4. 默认方案只有图像源时，追加图像二值化节点，并建立 `Image.Image` 到 `ImageBinary.Image` 的数据连接和同一条执行连接。
5. 保存完成后把用户方案写入最近打开记录，后续启动直接恢复该实体。

已有旧版 `default.vmx.json` 也会在最近方案恢复路径中执行同一迁移检查，补齐缺失的二值化节点和执行连接。

方案仍由 `GraphEditorDocument` 持有，节点、数据连接、执行连接、资源、流程和脏状态继续使用统一的保存、加载、撤销和校验路径。

## 复核

- `dotnet build src/VisionEditor/VisionEditor.csproj -c Release --no-restore -p:UseSharedCompilation=false -p:BuildInParallel=false`
- Release 构建 0 警告、0 错误。
- `FrameworkPluginChain.Checks` 当前 6 个图执行/保存重开用例通过，其中新增新建方案用例确认主执行流程存在，启动方案实体用例确认数据连接、执行连接保存后仍存在，并由 `GraphExecutor` 和 `SolutionExecutor` 实际把图像源结果传递到二值化节点。
- 启动方案生成不启动界面。用户可手动启动 `src/VisionEditor/bin/Release/net10.0-windows/VisionEditor.exe` 查看默认流程实体和执行连接。

## 边界

用户方案目录首次创建需要当前用户配置目录可写。不可写时保留内存中的有效方案对象，并继续使用新建、保存和校验入口。旧默认方案迁移保存失败时保留已加载方案并记录诊断，不中断启动。原厂 `.sol` 二进制格式尚未被声明为 VMX JSON 兼容格式。
