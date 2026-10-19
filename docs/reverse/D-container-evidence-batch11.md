# 第十一批D：原容器检查器的原始槽保全

基线 `b894e8c9dffed558c5eae8a2bacd68d70085782b`。对已交付的只读`SolutionInspector`作实际代码补齐，不把原生格式未知部分猜成写入器。工具自身源码完整，依REA技能边界不为普通源码修改重复启动native引擎。

原观察范围仍是MoudleFrame v5：每记录256字节UTF16名称槽、长度、payload；已识别mdata固定表每项256字节UTF8键槽、tag、1024字节值。过去只输出TrimEnd后的名称/键，不能从输出复原原始固定槽字节。

`src/SolutionInspector/ModuleFrame.cs`现在对成功解析的记录输出`nameSlotBase64`，固定参数项输出`keySlotBase64`，保留完整256字节及NUL位置；原文本、tag、payload hash和不透明值仍保留。字段为v1检查结果的增量输出，不更改原件，不写容器，不把内嵌NUL或padding猜成新语义。无效编码仍按既有分支拒绝，未声称对不支持记录也完整解码。

`tests/SolutionInspector.Checks`新增两个实际槽字节一致性检查（名称含内嵌NUL、参数键完整padding），并继续运行既有坏长度/计数/版本、尾部及安装CircleFind静态样本检查。实际结果由本批D/check.log记录；样本只读前后哈希另存input-hashes.json。未执行VM/SDK/模块，不作原厂运行或往返声明。

D03的XML/原对象映射与内部格式仍不完整，本片只推进其只读检查工具子集，不关闭整个单元，不因增加两个字段增加完成分子。原始字节保全可以用于后续字段归因，不能据此声称mdata类型标签、GUID重建、资源所有权或文件事务已恢复。
