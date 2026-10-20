# D2第二切片：radius2负列与原厂工作区地址

2026-10-19。只追813ca0 radius2列起1的读取与80e690/80bcd0行指针、812090分区地址的关系，不追seed flags负索引。没有执行负索引probe、被动附加其他进程或新增图像；全部为原始C/汇编与整数地址推导。

## 身份与取体

原MVD_Algorithm.dll SHA256 `85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549`。复用D1/D2已有导出，缺失812090与相关ABI汇编定向通过私有Ghidra12.1.4 readOnly/noanalysis补取，REA不可用来源与前文相同，无REA Evidence ID。

本轮独占目录`analysis/map-calibration/d2-padding-address/source`取得812090、816650、7e3f50、813ca0、80e690、80bcd0的C/asm；加既有7d3e90 C，共7入口索引。`ExportD2PaddingIndex.py`保存源码/可用汇编SHA及符号地址推导，不执行/解引用任何目标地址。旧7d3e90同目录asm缺失如实null。

## 负列的确切读取

813ca0汇编180813e83为每一行候选初始化ESI=1（column），radius独立从1增2；180813fa2将实际列EDX符号扩展到R15，fa5取row pointer，fa9/fb3比较`[rowPtr+column*8]`及`+4`两个int与INT_MIN。没有在实际读取前夹取负列。

因此radius2、候选column1的缺槽窗口包含column−1。首次该候选centerRow2时，窗口读row0..4、column−1..3（排除中心cell）；row pointer索引仍非负。以下令整数格网基址B、实际格宽w：

```
rowPtr[j]=B+j*w*8
address(row=j,column=−1)=B+(j*w−1)*8
j=0: B−8
j>0: rowPtr[j−1]+(w−1)*8
```

即只有row0访问B之前8字节；其他负列项读到**上一行最后cell**，不是每行独立prefix。r1首成功会提前返回，已采direction1图像并未证明执行过这些r2读取；本轮不主动触发它们。

## 两板型row pointer没有每行padding

HIK7的80e690从context[9]取整数row table、context[11]取整数payload base；HIK9的80bcd0使用同样context字段。两者都用`rowPtr[next]=rowPtr[current]+actualWidth*8`构表，首指针直接为context[11]，没有额外+8或+一行偏移。HIK7 asm18080f00f/f014加载context+50/+58，随后对应两套payload；int grid的初始化只覆盖实际w*h个cell。

整数grid双int哨兵首次写入仍在两wrapper的填槽循环；并没有另一个“column−1列全INT_MIN”的初始化循环。不能从row table的128对齐推导每个row有哨兵边框。

## 812090分区：B−8究竟属于哪里

816650 asm180816731把incoming workspace地址送RDX，18081675f调用812090；R8是图尺寸，R9是识别配置前两个int构成的packed最大格宽/高。812090只切分caller提供的连续workspace并计算所需总长度，不在自身malloc/free。

定义S=传给812090的分区基址，capW/capH=原始配置最大格宽/高，Ncap=capW*capH：

```
R=align128(capH*8)
Q=align128(Ncap*8)
T=S+0x80+2R
```

| context字段 | 语义 | 起点/保留长度 |
|---|---|---|
| [8] / +40 | float XY row-pointer table | S+0x80，R |
| [9] / +48 | int ID row-pointer table | S+0x80+R，R |
| [12] / +60 | 有序有效点XY输出列表 | T，Q |
| [10] / +50 | float XY grid payload | T+Q，Q |
| [13] / +68 | 有序有效点整数ID输出列表 | T+2Q，Q |
| [11] / +58 | int ID grid payload B | T+3Q，Q |
| [14] / +70 | 后续拟合点scratch | T+4Q，后续段 |

关键汇编812090的180812251/265写两row table；278→27f→286→28d依次按Q加地址，写context[12]/[10]/[13]/[11]。故：

`B−8 = context[13]+Q−8`

它属于**前一个整数ID输出列表的保留段尾8字节**，不是专门给grid左边界分配的哨兵。若Ncap*8已128对齐，正好是该列表声明容量最后一对int；否则它可能是该列表之后的对齐padding。两者均没有在812090中写INT_MIN的保证。

原始容量Ncap与当前有效点数count、实际格宽高w/h必须分开：有效列表只写/复制count对；当前grid只初始化w*h槽。未写到的保留段尾可含先前workspace内容，不能假设0、INT_MIN或其他值。

## 能核与仍未知的分配边界

812090可核分区相对地址、原始配置容量、对齐保留长度及所需总长度输出指针；816650/7e3f50汇编确认workspace由caller传入并继续转发，无该层新分配。7d3e90 C显示上层总工作区及多个输出/可选缓存指针参与scratch选择；该处复杂指针表达尚未全部用汇编核对，当前不把它简化为某个固定malloc偏移。

因而本轮**不知道实际SDK heap allocation起址/总分配字节、S到heap起址偏移、B−8当次具体8字节内容或是否被更上层清过**。分区布局表足以定位相邻保留段，不足以宣称原厂有安全语义padding、任意allocation上下文都相同，或该逻辑负列等同heap外读。没有用托管新数组清0填这些未知。

地址推导JSON含两个明确标为算术示例的布局：cap300×300/w14时R2432、Q720000，B−8落列表最后声明pair；cap13×13/w7时Q1408、原始1352字节，B−8落对齐尾padding。它们不是被动采到的SDK实参或内存值。

## 对后续工作的限制

当前可以确认radius2负列会跨到前行末槽或前一ID保留段尾，不能正规化成每行哨兵边框。若必须取得当次值，只在已授权自有SDK调用中被动记S/capW/capH/context[9]/[11]/[13]及已分配范围，且不能新增会执行该未知读取的输入；本轮未做该操作。

此切片闭合了相对地址和容量关系，allocator实际值与上层来源保留精确缺口。seed flags负索引释放仍单列后续，未混入此次结论。完整阶段统计仍D1/D2各S0，D3/D4未完整，2/32阶段、整卡0/4；局部取体不加整卡分数。无产品/宿主/提交推送变化。
