# 灰度匹配VA：首个真实module caller切片

2026-10-20。已取26个同hash本模块C体和对应asm，只读静态恢复，未实现产品算法、未执行DLL/Platform Init/设备。

## 证据身份

主DLL `IMVSGrayMatchModuVA.dll` SHA256 `b7327ffa6e71345f950e78b6ad221357cdf64b8fc930a90933cd12498c3b6d4d`。同字节复制到专属input，经private Ghidra12.1.4；最终采用ASCII临时工程+application.settingsdir/cachedir/tempdir三项隔离，project只读增量取体，不切共享REA session。完整26体/C/asm/直接调用/警告/常量身份在 `analysis/gray-match/native-first/source-index.json`，SHA256 `225e521b4b045e084e3f8aa37ac8264135acd0aea76d554c90115eafb68a5cb5`。

前两bat启动遇括号路径/Unicode失败；首次直接Java有URI/默认cache访问错误，但后留函数TSV表明也曾产C。此前即时目录检查说“无C”的判断已纠正。原日志保留；不能把exit0或warning日志本身代替原体。最终隔离批及后续-readOnly批均退出0，以下引用当前工作树C/asm哈希。

## 已恢复的真实运行顺序

1. `Process180023f60`：检查param1/param2及input+10 image指针，清this+15d90；image虚表+20返回1才入Alg（对照IMvdImage头，+20为GetPixelFormat）。非1返回E0000503，不能擅自RGB转灰。param3自身未见先行空检查，正常caller必须提供有效input对象，不执行未知null输入。
2. `ProcessAlg180024320`：CreateToolInstance→SetMultiThread→SetInputImage→ROI vector非空才取**第一项**SetROI→GetResult；每步非0直接返回。ROI vector空是跳setter，不代表SDK清为全屏；正常全图是否base给full shape需后续原输入准备链。开头1ddf0是QPC计时而非清缓存。
3. `GetResult180023180`：显式ClearMatchResult；原模型记录100槽，每槽步长228hex，仅data+210非零且flag+218非零才调用SDK SetPatternBuffer(+30，len+220)→Run(+98)→GetResult(+a0)。SDK result null抛E0000315。GetMatchNum非零才UpdateMatchResult；UseMatchAllMode（this+15e31）false时在第一个有结果模型处停止，true继续聚合。不能实现成“始终从所有模型挑最高分”。没有可运行模型时初始E0000513不被改0；已运行但零结果则return0，ModuStatus受OKWhenNumIsZero控制。
4. `UpdateMatchResult180025a40`：逐SDK结果索引GetMatchInfo(i)，GetMatchBox、点、分数取字段，模型槽索引写记录+14c，记录步长150hex，缺MatchInfo/Box只日志并跳该项；result本身null抛E315。decompiler遗漏GetMatchInfo索引，asm25b33确有EDX=i，不能照伪C无参调用翻译。
5. `LimitJudge1800238b0`：按聚合全数量out+24分配statusArray；原数量限制与Angle/Score/boxX/boxY/matchX/matchY六float谓词AND到每项和ModuStatus。此体没有Scale谓词，不能因ResetMember有Scale存储就添加判断。float范围边界是MOVSS lower/upper±精确FLT_EPSILON(2^-23)，不是int转换；1dd90伪C将指针推为int*属类型误恢复，asm已核。整数数量为有符号闭区间。NaN/浮点异常域只记录COMISS/JA/JBE原指令，不臆造安全过滤。
6. `ProcessOutput1800245d0`：先VM_M_SetModuleRuntimeInfo，BatchSetInt ModuStatus与MatchNum；MatchNum=min(聚合数,MaxMatchNum成员+15e5c)。UseMatchAllMode true才排序，vector不足Count抛E00000FF，模型表异常E0000315。随后短路发布ModelIndex/ModelName、Box五量、PointXY、Score、Status；任一helper非0停止，不能当原子提交。LimitJudge在排序之前且statusArray另存，排序与status关联须后续原sort/comparator核对，尚不宣称已修复或错误。
7. `Process`只在Alg返回0、LimitJudge返回0后调用Output；失败提前返不进入后续发布。因此失败/等待不允许统一伪造当前输出；原共享输出容器清理语义需框架桥复用确认。

## SDK与发布ABI

CreateToolInstance224c0持有两个实例：GrayPatMatchV2Tool(+15d98)与GrayPatternV2(+15e38)，已有非零就复用；Create失败或空句柄按原返回码抛异常。SetInputImage24cb0转SDK+10，SetROI25990转+40。SetMask24ce0先SDK+50清mask再逐项+48，SetMaskImage24d50按bool传输入或null到+70；这些override的**上层调用时机**尚未从Frame取全，不把imports当顺序。

SetMultiThread24d90读取VM_M_GetOwnerProcedureRunningPolicy；失败抛返回码。policy!=2时threads=1/cpuMask0；policy2按原1024个byte标志倒序折叠uint64 mask，再SDK+d0/e0。接口支持较大policy结构，不据CPU数量猜规则。

Output四薄桥22070/22090/220b0/220d0的asm分别尾jmp VM_M_SetFloat/Int/String/BatchSetInt；传入的第二参status在薄桥被name重排覆盖。伪C将桥误识别void，而caller测试EAX真实返回状态，实施必须按asm/目标导出签名恢复，不能照伪void丢错误门。

## 清理

ClearMatchResult22430逐150hex记录delete[] +140指针，置+140/+148，memset前13c，vector.end=begin，保留capacity。DeleteModuBuf226f0分别Destroy SDK matcher/pattern，再析构并delete模型对象；DeInit22620先free statusArray，再DeleteModuBuf、清状态/范围字段。ResetMember24a90只初始化状态/范围，不等同DeInit。析构22310和Save/Load/模型命令内部仍列后续，不能声称方案与UI模型生命周期全闭合。

QPC helper1ddf0按counter*1000/frequency返回double，GetResult末转float runtime；常量31260原double1000，310a4原float bits34000000(2^-23)已在source-index落原bytes。

## 明确剩余必需链

- SDK GrayPatMatchV2 wrapper自身vtable/Run→MVD_Algorithm实际opID/handle、预处理mask、模型训练/匹配参数与输出布局，当前模块体仅证明具体调用，匹配核未恢复。
- 模型CFeaturePattern构造/SetModelData与Import/Save/Load/清候选具体体及GUI继承ModelCommonHelper链；当前token/Cs资源库存不能代替具体序列/失败/释放。
- SetParam各sscanf_s目的地址、UseMatchAllMode/OKWhenNumIsZero/MaxMatchNum/SortType与资源可达性需用asm逐项连通；当前伪C有遗漏参数，禁止据其造默认。
- sort82a0已取分派与6/7按roundf分组，但比较器75d0..7640、std排序tie语义与statusArray关联尚缺；空结果分组边界需先静态核ctor，禁止运行未知危险输入。
- 原输入准备（typed输入/ROI/mask/fixture）、Reset有效名单、实际结果marshaller及失败输出容器/显示缓存生命周期。只列当前工具必需链，不扩所有图像库或全部CPU。

Blob本轮仍保持独立首轮身份库存，没有把灰度体外推给Blob。两工具都未计完整，本轮无产品实现、提交或人工验收启动。
