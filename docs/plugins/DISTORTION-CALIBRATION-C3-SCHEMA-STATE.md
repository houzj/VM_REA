# 畸变标定 C3-2：schema、字符串转换与延迟参数应用

2026-10-19。本切片复用C1/C2/C3-1，只新增静态源码/原指令、DLL内嵌资源及既有托管反编译快照。没有原厂DLL运行、产品/公共核修改或新泛化probe。第一切片对7020立即写算法context的概述已修正：它先校验/缓存，下一Process才实际应用。

## 具体schema及证据来源

C1 CreateHandle(69)→1bff0→Init1c210→1cb20→10ae0(this,ce)。10ae0保持RDX，12ee0调用GetModuleHandleA("MVD_Algorithm.dll")、FindResourceA(module,ce,"XML")、Load/Lock/SizeofResource，将资源复制进schema缓冲；00e8a0随后解析XML。这不是磁盘AlgorithmTab，也不是测试人员编造的参数表。

REA静态PE parser证据 `ev_c8783f8868ea1d4bb3f158433106ca12d728147dacf523e95d318cdc39166adf`，原core SHA256 `85D17ECC87D52E9A62D8BB86C8D5208AC999BCD31EC6D0806453F8165733F549`。XML/206/lang2052资源file offset51670720，8810bytes，SHA256 `A2FDD6190C031E54B195652E87DA8D0B9DC37949D7B29F94FB5AE06E3AF1EADE`。保留原字节为analysis/distortion-calibration/c3-schema-source/resource-206.xml；资源元数据和REA提供器在resources-rea.json。XML205同时作为原资源索引邻项留存，不用它证明206行为。

| schema名称 | ID / 类型 | 裸core默认 | 资源范围/枚举 | 7c7c20实际数值验证 |
|---|---|---|---|---|
| HomoFreedom | 40001 / Enumeration | 2 | 1 Perspective、2 Affine、3 Similarity | unsigned(value-1)≤2；否则10103026 |
| CalibBoardWeightFun | 40002 / Enumeration | 3 | 1 LeastSqure、2 Huber、3 Tukey、4 Ransac | unsigned(value-1)≤3；否则10103027 |
| WeightFactor | 40003 / Integer | 20 (0x14) | 1..200 (0xc8), inc1 | unsigned(value-1)≤199；否则10103028 |
| CorrectModel | 40401 / Enumeration | 1 | 1 Linear、2 Radial、3 PersRadial | unsigned(value-1)≤2；否则1010303d |

校验前另有算法能力bit掩码判断；不能只凭范围宣称任意core类型支持这些字段。畸变69对应的真实路径已由C1确定。模块默认仍由外层Algorithm.xml覆盖为Freedom1/Weight2/Factor20/Model1；原内嵌schema默认不能覆盖实际模块初始化顺序。UI仅声明Weight1/2/3，内嵌schema声明4，两者确有层级差异。

## 托管入口到精确名称/值

既有原厂反编译ImageCalibModuParams.cs：CorrectModel.SetEnumValue((int)value)，WeightFun同样SetEnumValue，WeightFactor.SetIntValue；构造绑定分别是"CorrectModel"、"WeightFun"、"WeightFactor"。ParamItem的两个setter均直接value.ToString()→Value→IMVS_PF_SetParamValue_CS(handle,moduleId,Name,value)，没有Enum.IsDefined或范围检查；读取int.Parse返回字符串。此处描述取得的旧公开PlatformSDK包装，不冒称所有托管SDK版本一致。

模块21560对WeightFun重命名为CalibBoardWeightFun；原名或别名且value首字符4时直接返回0，不调用SDK。模块1fb30读取同样做别名转换。故托管WeightFun=(enum)4会形成"4"并在模块忽略；不能声称.NET枚举本身禁止强制转换值。直接SDK旧名WeightFun并不因此获得同一别名合同：别名处理证据属于模块。

SDK28e0只改RCX到core handle，RDX/R8仍name/value；core22ab0→1d6c0在this+570的0x98字节记录表中逐字节区分大小写比较name，返回record+84的数字ID。该表来自Init1c210→67790→11250解析同一schema的AlgorithmIndex；未找到返回8010001e。读路径22890→6d30从缓存取ID/value，2c60按schema类型格式化，整数/数值枚举用%d，符号枚举标记非0另走21a0。

2af0先11250取得类型(+18c)：2 bool→1340，3 integer→检查字符串仅含"0123456789.-"后14d0，4 float→1650，6 enum→1830。WeightFactor走integer，三个选择项走enum。14d0用stringstream提取int，不是strtol(base0)；不能把资源中的十六进制文字直接当作SetRunParam字符串的数字合同。1830从13320建schema枚举映射，按名称查符号值；未匹配的token仅在上述字符集检查通过时交14d0，最终OR组合值并输出数值/符号标记。分隔与枚举映射容器helper内部异常/空token行为不在本有限切片闭合，不以猜测补全。资源十六进制解析与运行字符串转换是两条路径。

## 缓存、schema DOM、算法context的时点

1. 22ab0取得ID和解析值后调用7020；6ec0回调7c6830的operation6，8字节payload为(ID,value)。该case只执行7c84f0能力判断和7c7c20范围判断，没有写a0。
2. 校验成功，7020→6c70→7790比较缓存现有ID/value；相同不再dirty，否则存8字节原值，参数dirty表置1、缓存管理器+110置1。校验失败不进入该缓存更新。
3. 22ab0随后2e70以原始解析value更新schema对象400字段；enum另存符号标记到198。12590把schema DOM的CurValue改写；2e70/12590失败不等于前面的缓存更新已回滚，异常与持久化磁盘事务仍未完整证明。
4. core Process1c4e0调用7120：若dirty，逐项先清dirty再以operation1调用7c6830，用的仍是缓存的原始ID/value；操作失败保存错误，最后全局dirty清0。成功应用不把缓存value改为算法写后值。
5. operation1与批量operation2都在ID40002/value4时令algorithm context+a0=3，其余值直接写；这是运行字段兼容转换。参数缓存、schema DOM和algorithm context是三份状态，不得统称“core改存3”。

因此，在直接SDK SetRunParam("CalibBoardWeightFun","4")成功、没有其他setter介入的源码路径中，缓存/DOM为4，后续Process把算法a0写3，GetRunParam的缓存读/数值格式化仍会给4。这是源码推导的正常路径结论；本轮没有实际调用验证，未扩展到任意失败/并发情形。模块输入"4"会提前忽略，不能用这条SDK路径替换模块行为；直接低层options+14=4则仍是832830另一路。

## 依赖式模型点/中心补证及边界

本轮不继续无边界展开矩阵数学。既有7ed0c0的模型1/3尺度可明确到相同形状：选定两点a/b，scale在理想点距离大于阈值时取图像点距离/理想点距离，否则取默认1；按(ideal[i]-ideal[a])*scale+image[a]平移缩放后交选定拟合入口。a/b的全部选择/并列规则、旋转网格推断和中心初值分支仍待专门切片。模型2/3的853820及857a60与模型1分派按C3-1保留。probe中的warp只是生成输入像素的测试标签，未变成schema字段。

## 验证及下一有限缺口

ExportC3SchemaEvidence.ps1验证原binary、REA资源原字节哈希、四字段ID/default/range和源/汇编锚点，并收录取得文件哈希。源码量及锚点数不是运行覆盖率。未修改产品、未构建托管项目、未执行原厂运行。

保留未知：enum splitter/符号映射全部异常、溢出/非标准数字字符串的运行观测；schema DOM失败后缓存回滚；保存/载入到磁盘的全事务；模型网格选点并列/中心与初值完整表达式；真实宿主并发。C1容量/异常与C2非有限及越界安全策略仍开放。不据源码推导硬编码历史prefix。
