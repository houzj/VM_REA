# 第十三批：纯XML设计迁移独立审查

审查对象为根线新交付 `src/FrontendDesignMigration`，不修改其产品文件，不并行构建。依据 `analysis/decompiled/CompatibleTool.Audit/CompatibleTool/ConvertHelper.cs` 与 `FrontendUIUtils.cs` 的完整选定方法；并非原厂业务运行证据。

## 调用链对账

`ConvertHelper.ConvertDesign` 按 `XmlDocument.FirstChild` 识别Root为V4.0。`ConvertDesignData4_0To_4_4` 先调用 `FrontendUITo_VM4_2Convert` 做四项字符串替换，再调用 `DesignDocFromVM4_3ToVM4_4Convert`，写 `VersionInfo`；外层移除旧controls命名空间。新入口 `ConvertV40ToV44` 保留该有限分支和调用顺序，拒绝其他版本，不宣称完整.sol/gro转换。

选定XML主体已保留以下控制流：

- 默认设计模板的 `FirstChild.LastChild.FirstChild` 下定位首个 `FrontendTabItem`/`SubRoot`，只有源有子节点且目标节点存在才追加转换。
- 根Size设置Width、Height+60；非空Background复制。模板真实结构通过显式参数提供，不猜厂商默认资源。
- GroupBox仅递归首个内容节点的子项；Tab逐项递归各自首个内容节点的子项。未知wrapper不自动深入。
- SubWindow读取Document，按明确编码base64解码→XML→显式子模板，逐子项递归后重新编码。随后仍处理SubWindow自身Size与brush。
- ImageControl/MultiImageControl/TableControl/PicBox设置源码拼写 `AutoStrechA=false`；Size覆盖现有Width/Height并删除Size；Background/BorderBrush的`{x:Null}`转`#00000000`。

未发现选定方法移植遗漏实质分支或字段依赖。源中只读未使用的attribute值仍保留，不据此引入新语义。

## 有意差异及限制

原GetDefaultDesign依赖AppBase并吞部分加载异常；新实现显式模板路径且读取失败抛异常。原Base64依赖 `ByteUtil.DefaultEncode`，目前未认证其实际Encoding实例；新实现强制调用者提供编码。新XML reader禁止DTD/外部解析并限定文档长度；CLI只新建输出以保护输入。这些属于本地安全/配置合同，不能说成原厂兼容完全等价。

`tests/FrontendDesignMigrationChecks13` 包含18个真实转换断言，模板和输入均标注synthetic，涵盖GroupBox/Tab/SubWindow、根与控件Size、nullbrush、编码、未知wrapper不递归、FirstChild声明行为、DTD独立拒绝和版本拒绝。根线串行构建运行并保存实际结果，未以写完测试冒充通过。此交付不关闭完整D04.1或A06.1。
