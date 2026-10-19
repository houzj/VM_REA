# D02 batch3：真实插件准备保存与历史的独立复核

2026-10-19；审查基线 `6a2cf54e708044022e8bd916b460412e09e255f0`。只读核对 main 已有源码与根任务运行的 `VisionEditor.AssetHistoryChecks`，未重新构建、未运行SDK、未访问tool-plugins。本切片是独立 `.vmx.json` 框架集成诊断，不提供原厂VM行为合同或 `.sol` 等价性证据。普通完整源代码审查不需要重复反编译原程序。

## 结论与真实调用链

诊断结果 `passed=false` 成立。相对图像路径经成功另存后，Undo恢复准备保存前的历史，资源身份与实际读取均失败。这不是检查只看路径文本导致的误报：探针实际调用生产 `ImageSourcePlugin.OpenEncodedImage`，捕获到新目录下不存在的 `saved/assets/rows.bmp`；原目录资产当时仍存在，字节匹配失败不是探针提前删除文件。

- `src/VisionEditor/MainWindow.xaml.cs:49` 将 `GraphEditorDocument.PrepareSave` 接到真实 `ToolPluginRegistry.PrepareSave`，并传执行scope；探针使用同一注册器与ImageSourcePlugin，但没有执行scope，所以不宣称完整WPF或运行态覆盖。
- `src/VisionEditor.Core/GraphEditorDocument.cs:37` 的Save把当前文档副本交给准备器，重定位并写盘成功后提交新的文档、文件名及BaseDirectory。历史栈没有同时转换。
- `src/VisionPluginContracts/Contracts.cs:20` 的注册器为支持资产持久化的节点执行准备方法，并替换插件资源表；它不是纯路径重定位器。
- `src/VisionPlugins.ImageSource/ImageSourceAssets.cs:16` 的准备器把ImagePaths等转换成绝对路径；SolSaveImageData为true时保存字节、摘要和路径到资源ID的映射。
- `src/VisionEditor.Core/GraphEditorDocument.cs:249` 的历史快照保存旧文档与旧基目录。Restore先Rebase到当前基目录再转移栈；Rebase只识别资源表和内置 `vision.image-file.Path`，不转换插件私有ImagePaths。
- `src/VisionPlugins.ImageSource/ImageSourceAssets.cs:11` 的读取器优先查嵌入映射；准备前快照的映射为空、资源表为空，因此回退到 `Path.GetFullPath(path, baseDirectory)` 并读错新目录。

## 操作结果与覆盖边界

根任务日志 `analysis/full-audit/batch3-20261019/D02/asset-history.log` 及其独占 `asset-history-*/observations.json` 记录：加载准备前文档正常；改名后另存正常且有1个资源；Undo时identity/encodedBytes两项均false并有DirectoryNotFoundException；Redo恢复准备后快照正常；最后删除外部图片并重开，仍可从嵌入资源读取相同编码字节。最后一步的绝对路径身份成立只代表字符串指向原资产位置，不代表该位置仍存在；日志单列sourceExists=false，语义没有混淆。

只有一次RenameProcedure产生的历史项、一个相对图片、开启嵌入保存、跨目录另存被验证。它不足以说明所有插件、相对文件夹、未开启嵌入、scope状态、深层历史或失败事务的行为。保留原始失败JSON和退出1，不能用后续成功重开覆盖Undo失败。

## 所有权与可执行后续

框架负责历史和方案基目录，插件负责私有参数/资源语义；现有合同没有可供Core调用的纯路径重定位入口。安全的main本地动作是保留诊断、标记已知缺口，并登记跨线FCR，明确producer/consumer/兼容性/生命周期和验收证据。此复核不要求修改插件、共享Contracts或另建插件配置框架。

不建议Core硬编码 `vision.image-source.ImagePaths`：这把私有参数责任搬到主框架，也遗漏ImageAcquisition、校准文件等其他工具。不建议在Restore或成功Save时把PrepareSave批量应用到历史快照：注册器可接收当前执行scope，Translation/NPoint等准备器会按当前运行状态写参数，并进行资源I/O；这样可能把当前状态写进历史，新增读盘失败或改变旧编辑语义。不能把它当无副作用的路径规范化函数。

候选合同应仅提供无文件/设备/执行副作用的节点路径或资产身份重定位，输入旧/新基目录及节点/资源快照，返回独立快照；必须明确嵌入映射键、无此能力插件的降级或拒绝策略。具体接口由唯一所有者确定并记录FCR后交付，不能在本切片推断通用实现。原厂历史及资源保留规则继续独立取证。

诊断项目当前名以Checks结尾且预期退出1，容易被当默认绿色测试。建议明确命名 `AssetHistoryProbe`，或至少提供README标注“已知失败诊断、预期退出1、原始passed=false保留”，不要加入默认通过套件；若未来修复，应新增修复后运行记录，保留本轮失败证据。

## 审计

根任务拥有测试源码和执行，本子任务仅独立阅读与记录。审查输入的SHA256清单保存到 `analysis/full-audit/batch3-20261019/D02/review/review-input-sha256.csv`，含真实Core、插件准备器、注册器、探针、日志与原始JSON。未写产品、台账、合同或父任务探针。
