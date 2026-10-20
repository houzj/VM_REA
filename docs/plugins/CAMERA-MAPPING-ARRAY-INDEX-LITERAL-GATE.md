# ArrayIndex 数字判定与正常UI路径门纠正

2026-10-19，源码复核；没有原厂运行、没有产品构建。此前方案把MathHelper.IsNumber接受小数直接外推成普通SetCombination UI小数常量可达，结论撤回。

## 确切源码链

原证根目录 `D:/MyWork/REA逆向测试/analysis/decompiled`：

| 文件 | 具体成员/行 | SHA256 |
|---|---|---|
| Apps.XmlParser.CapabilityAudit/Apps.XmlParser/MathHelper.cs | IsNumber:9–22，四regex联合 | 0C0EC68FD4EFF74D862CAA9EF20594D547B206FBB4999057BDA9A6F9757F1C71 |
| Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/SetFilterSourceOperation.cs | GetBindIndex:183–204 | 197EC3EB19974776DB4DC9AB293CABABED6319EF2A5A1C0DA6A8A9BFD5EB0401 |
| Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/SetCombinationSourceOperation.cs | Operate:60–81，先RelateIO后GetBindIndex/RelateIndex | 7CE50140DF9BC50B1E954A3C604B5FA294F91DB1318A22253FEEBB9811BE9444 |
| VM.Core/VM.Core/VmIO.cs | RelateIO:121起；RelateIndex:262起；SetRelateIndex:828–834 | 15E1832243422B6BFA2714F300B796D54C365950573639EB1039C5DF58E99CB0 |

`IsNumber`先排除非`0-9.-`、多dot、多minus，再要求`(^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$)|(^([-]|[0-9])[0-9]*$)`。后者接受单独`-`；整个联合接受`.5`/`-.5`/`1.5`，拒绝`1.`/`1.2.3`/`--1`/`1-2`/`+1`。候选不再用缩略regex代替。

但`GetBindIndex`的先后门是：非空→先`Contains('.')`查`GetIoByPathFromAround`；**仅else**调用IsNumber并创建Name原串的VmConstantIO。任何查找返回null，显示本地化“订阅失败”，Warning图标，然后仍return null，没有throw或回滚。

`SetCombination.Operate`先切掉`[...]`并查来源。来源不存在提前返回；来源存在且目标IO存在则先`iO.RelateIO=source`，随后`iO.RelateIndex=GetBindIndex(...)`。后一返回null时照常赋值。RelateIndex setter先UnSubscribe旧来源，SetRelateIndex(value)递归给所有Children，再Subscribe原来源。因此**未找到dot路径时来源保持，索引变null/All，两叶都清掉索引**。不得保留旧index、把base绑定全部回滚或报告typed decimal literal成功。

`-`不含dot且IsNumber为true，创建Name=`-`的常量。原native atoi域结果0，原串仍必须保存。DTO/primitive直接带小数原串仍可作为独立底层API适配，但不能冒充上述正常UI的常量入口。超int32拒绝仍是托管边界，未声称原C overflow等价。

## 本轮候选与验证状态

`analysis/camera-mapping/proposals/array-index`现21生产文件候选，23 C#候选/测试文件语法检查零错误、git apply预检通过，**未应用共享、未构建或运行**。

桥Encode遇typed literal的dot数字原串按当前明确未找到IO路径的支持域归All；Bind/BindMany仍应用来源，并携带Available+Diagnostic=“订阅失败”。POINT将两叶结果读回；selector先应用真实结果，再返回false/诊断并通过注入的notification服务显示Warning。新增Warning枚举追加在末尾，保持旧枚举数值。此工作树没有用display caption恢复新IO身份的查找能力；已知同过程int输出仍由typed IndexOnly树选择，真实native任意字符串路径解析不在此扩展。

测试准备：`1.5/.5/-.5`逐个验证失败提示一次、两叶All、来源不回滚及实际Runtime读All；`-`验证UI成功、raw保存和运行取0；独立parser验证上述有限有效/无效集合与overflow；既有动态缺失/恢复、配置Execute按钮与debug链继续保留。未运行前不得计测试通过或整工具完成。人工验收不计完成门，整体数量由根线审计，沿用0/10=0%。
