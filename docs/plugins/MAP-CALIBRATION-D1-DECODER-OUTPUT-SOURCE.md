# D1 18/24 decoder、payload与产品差异有限切片（2026-10-19）

本切片已完成当前18/24入口的纠错状态、输出次序和payload→orientation/origin连接，不再继续扩base/shape或所有CPU。只读共享产品，未改算法、构建或执行原厂。已有原厂40个字节纠错案例重新核身份与数据一致性，不能冒充本次原厂运行或图像识别验证。总体仍按完整工具数统计0/10；逆向、实施、自动验证和自审全部完成才计工具，人工验收单列且不阻挡计分。

## 证据身份与有限取体

原件SDK core SHA256 `85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549`。复用8b0260、8b11a0、8b0670、86ff00及当前两个caller的C/asm/cookie尾；定向补8个被这些体实际调用的叶体：931cd0、931dd0、932120、932570、932440、932080、931c60、85aee0。身份匹配私有Ghidra12.1.4只读工程，无REA Evidence ID，REA provider不可用沿用已有记录。新C/asm在 `analysis/map-calibration/d1-decoder-output/source`；20体hash及5个共享产品文件读取快照、PE常量和既有40案例一致性见 `source-index.json`。

## 当前入口与状态（已纠正旧码分类）

86ff00在workspace+ce00生成192个整数bit，在870843调用 `8b0260(bits,18,24,workspace+d100,scratch)`。8b0260：

1. 输入/输出/workspace任一null跳8b0638，cookie后返回 **0x10000014**。`checkCount<2 || symbolCount<=checkCount`跳8b02bd，返回 **0x10000019**。旧SOURCE-CHAIN将两类对调，本次依实际asm跳转纠正。当前18/24正常指针不触发两门。
2. 每8个bit依128、64、32、16、8、4、2、1打包一个int符号，24个符号保持采样顺序。初始化GF表932480，移位乘法按0x11d约简；符号数组按升幂系数供9322a0求值。
3. 对i=0..17，932370生成2的i次幂，9322a0求24符号多项式值，得到18个syndrome。全部为0直接复制24个int到out，返回0。
4. 首个非零syndrome进入8b11a0；若非0透传状态。它成功才调8b0670；成功返回0，失败透传。最终输出写入与外层payload成功标志之间没有额外“纠错次数”字段。

### 8b11a0：定位系数与零点门

该体将18个syndrome复制到前缀0之后的工作数组，初始两个系数向量首项1，按18轮discrepancy做更新。discrepancy为当前syndrome异或先前syndrome与当前系数的GF乘积；非0时调用逐系数缩放931cd0、逆查表85aee0、端对齐xor931dd0，并按实际向量长度门替换/更新系数。931dd0的具体地址规则为 `out[maxLen-lenA+i]=A[i]`，再异或 `B` 到 `out[maxLen-lenB+i]`；不能无视这个端对齐把它替换成任意系数约定的多项式加法。

末尾8b15b7..8b15ea只扫描i=0..23，求值 `locator(2^i)`，值0才按递增i写位置数组。**找到的位置数必须等于当前记录的degree**；不等返回 `0xfffffc18`（signed -1000），相等写出count并返回0。体中未见单独的 `degree<=9` gate，也未见从24以外继续搜索。不能用熟悉的纠错能力名称代替这个实际接受条件。

### 8b0670：纠正量和一次性输出

输入为syndrome、前一步位置数组、位置count、原24符号及最终out。它构造每个位置的 `[2^position,1]` 系数并逐次932120卷积，再翻转有限syndrome系数、卷积并截取工作系数；最终对各位置计算逆值、其他位置差积、求值和除法，纠正量写入96字节全0的errorMask对应位置。

补体已确认932570是 **系数数组反序复制**，不是除法；要求两descriptor degree相等、两指针非0，失败0x10000019。第一处返回失败只绕过其后的工作系数填充继续；第二处失败从8b10a8透传。当前18/24 caller内部两处均由非空栈缓冲与相同degree构造，结构条件本身不失败；不把人为坏descriptor当实际图像分支。

931c60才是查表除法：分母0直接返回0x10000019，8b0670从8b116a透传；否则把纠正量写errorMask。**分子0时先写0、但没有return，仍继续Log/Exp查询并覆盖输出**，asm931c70→931c77确认。932480不写Log[0]；该槽值与该分支在真实纠错输入中的可达性仍是独立精确边界，不能按通常GF规则静默补成分子0直接返回0。

成功最后按24个int计算 `out[i]=original[i] XOR errorMask[i]`（SIMD/标量复制路径均有），返回0；纠正量失败发生在最终xor写回前。体中未见输出后重新计算18个syndrome的拒绝门，也没有独立的 `count>9` 拒绝。此为源码可见分支，尚未据此构造>9损坏输入或宣称其原厂返回必然成功。

## 输出字段链（原体，不从几何猜标识）

令Z[0..23]为8b0260成功后的24个int；86ff00的out位于workspace+d100。定义P[j]=lowByte(Z[23-j])，j=0..11。

| 字段/动作 | 原体赋值与消费 |
|---|---|
| payload12 | 86ff00只在8b0260返回0后，从d15c、d158…d130依次取低byte，写record+11c…127，即P[0..11]；不是原符号0..11。 |
| 成功 | 同一分支写record+f0+item*4=1，并使record+118计数+1；失败置对应status0，不执行上述payload写入。当前813ca0先清整个输出，且首成功即停，不以旧payload残留当成功。 |
| 方向 | 813ca0四个正序循环、四个逆序循环，状态1时将试次写param10，范围1..8。方向来自成功quad尝试序号，不是payload中的位；86ff00投影方向向量也不是该整数枚举。 |
| radius/anchor | 同一首成功分支写radius与候选格网位置；两个上层caller仅在status==1时按方向组1/5、2/6、3/7、4/8对相应anchor分量作0或−2radius校正，再传813020。这个anchor来自稠密格网搜索，不是最近点拟合。 |
| 80e690 origin/pitch | 813020的originX=P0、originY=P1；pitch=`float(P2)*100f+float(P3)+float(P4)*.01f+float(P5)*.0001f`。 |
| 80bcd0 origin/pitch | originX=`(P0&15)*64+(P1>>2)`；originY=`(P1&3)*256+P2`；pitch=`float(P3)*100f+float(P4)+float(P5)*.01f`。 |
| 最终ID/顺序 | 原始整数grid、校正anchor、两origin、orientation一起进813020；其变换/输出顺序与尺寸复用已审计直接320例，但这不证明真实图像方向2..8已覆盖。 |

100/.01/.0001三常量来自同hash PE字节，见索引。两caller只解释P0..5，复制12 byte不等于12 byte均为当前工具的物理字段。

## 共享产品只读对应与最小差异请求

本段读取时点的hash已记录；根线可能继续修改共享文件，审查以实际差异为准。R01/R02由根线拥有，本报告不另写共享实现。

| 源码条件 | 当前文件/方法 | 具体差异与有限实施/验证 |
|---|---|---|
| bit→24符号、12 byte反序、六字段解释 | NImageCodeReedSolomon.TryDecodeBits；NImageBoardIdentity.FromDecodedBytes | 顺序和两个pitch/origin公式已有对应；复用既有40原厂纠错fixture及Identity/Anchor检查，不因补源码重写正常字段。 |
| 8b11a0根数等degree、8b0670查表纠正后直接成功 | NImageCodeReedSolomon.TryCorrect | 产品采用独立BM+线性方程幅值求解，显式degree1..9、纠正后重验18syndrome；Div分子0直接0。对已证0..9正例结果相同，不能据此称失败/超纠错能力接受域相同。**R03**：若要覆盖原厂完整拒绝/误接受域，应在现有24符号层提供source-compatible内部入口/状态与原体门槛，专用0/1/9错误正例、根数不匹配与必要的失败词差分；不要仅删保护门或按标准RS库猜。现无失败词原厂trace，此项尚未实施/验证；保留“仅已证正例兼容”的当前边界不能冒充全域。 |
| 当前native首成功试次1..8、原始格网anchor | NImageCodeDecoder.TryDecode；NImageBoardRecognition.Recognize | decoder的0..7循环索引可对应native试次−1；Recognize仍拒绝Orientation!=0，WorldIndex固定1，并从Opening[0]拟合Nearest anchor。**R04**：接收R02真实candidate上下文中的native方向/anchor，按813020转换；只去掉方向拒绝不够。先用真实成功caller证明方向，再开放相应产品路径。 |
| warp/位采样的单精度与预处理 | NImageCodeDecoder.TryOrientation | 当前double Homography/双线性、独立adaptive过滤仍明确独立。用已恢复86ff00/85c8a0/8b1710当前caller体逐阶段替换或确认，不再重复取与这条路径无关的函数。验证应比较warp/processed192 bits→24符号→payload，不能只比最终点。 |
| first success / grid筛选 | NImageDetectedBoard.Recognize；NImageSourceLatticeAdapter | R01/R02根线处理；本次快照有首identity注释，但仍扫描全部opening作mask。用根线审查后的最终实现与既有反序fixture验差异，本线不再建第二套状态。 |

## 本次验证与生产接入最小剩余

`docs/plugins/n-image-code-evidence.json`是同core SHA的既有原厂字节层回放，4 payload×0..9损坏=40例。索引助手重新检查40返回0、24输入/输出长度、实际差异数与errors字段一致；独立按原GF约简/升幂求值核40个输出均满足18syndrome全0，共960符号、480个反序导出byte。没有运行DLL或当前C#；`tests/NImageCode.Checks`已有逐24输出和12byte断言，应由根线串行验证，不能算本次已通过的新产品测试。其独立旋转性质测试仍不是813ca0真实八向识别。

整工具生产接入剩余按以下有限序列，而非逐函数无限扩：

1. 根线合并R01/R02的grid/首成功上下文；对R03决定并实施明确的source-compatible24符号路径，保持独立安全边界可审；R04消费真实方向/anchor，修复Map当前生产识别入口。
2. D1只剩当前warp/192格位次序、整卡字段→产品映射的收口与其精确分叉验证；已恢复8个纠错叶体不再列缺体。D1 S1/S2尚未整卡退出，不能凭这条decoder链自动加分。
3. D3用源码实际支持分支补真实caller方向、失败/稀疏验证；D2复用原卡已闭合规则与现有fixture验共享实施；D4按整工具退出清单补文件注册桥/绑定/显示释放等实际工具必需链，不扩全部SDK。
4. 原资源/配置/两ROI/运行结果/持久化/生命周期的产品对应、有限自动验证及自审都完成后，交根线整工具审计计数；人工验收独立记录，不作为完成率阻挡项。

未提交推送、未操作其他worktree/进程/设备。当前切片没有足够依据自行宣称映射标定整工具完成。
