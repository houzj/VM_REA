# 标定加载完整实施退出矩阵（本轮在审）

2026-10-19。继相机映射完整实施后接续；整体沿用根线已审1/10=10%，本工具未计完成。人工验收单列。只审标定加载工具消费者及现插件框架接入，不扩远程FTP/设备/任意native heap。

## 身份与可复用原证

原ReadCalibFileModu.dll SHA`86B8B112ADED58DE27A41A5EDE6EC6B41117D7DB8D0C5934647FA9514E6FA403`；Control.dll SHA`9C5C2C529B5242F9735A51DEF23C30FE90B2E3CBFE16C1FF31F95D0F1C129300`。资源与源码身份已收在`read-calib-file-definition.json`、`read-calib-file-verification.json`；受控原厂模块事件在`read-calib-file-evidence.json`（32 Process、17 LoadFile，ModuleFrame合成输入宿主，非原桌面/设备）。现原生体在本树`analysis/full-audit/read-calib-file/native`，托管体在`analysis/full-audit/calibration-dependencies/decompiled/ReadCalibFileModuControl`，本轮先复用，不重启原平台。

Process c520伪码SHA`E3C6315832CE99019996B39A5BDC7B4CB10867E8C35C7C1F132092353750DC2A`；原指令SHA`AD95A4DE838D033D5F583B449B4D6AE2965596E59BD6F4412B6A87CA80A59EC6`。GetFloat指令cdac→cdcf：RSP+20=count指针、R9=value指针、R8D=0；ret0且count>=1才覆盖本次值，没有count==1门。Refresh GetInt c5a3→c5d5同第五参数count/index0，ret非0清本地signal为0，成功仅判断value非0。

## 整工具退出矩阵

| ID / 活动链 | 原入口与现实现 | 本轮判断/最短实施与验证 |
|---|---|---|
| RL01 资源/配置/运行命令 | 原AlgorithmTab→OpenFileDialog4ReadFile、10个ButtonSelecter、ReadFileConfigData、BottomCommandGroup | 已有公共配置实现和历史48项；复核全部setter/模板及当前真实MainWindow，不能仅旧专属Window通过 |
| RL02 文件选择/立即写参/输入面板 | OpenFileDialog4ReadFile.ValueChanged先base再ReadData；ab50/d120文件路径立即加载 | 旧846含路径换成功/失败保留；新审UI返回路径后Configure是否无意重新读旧文件，需失败→再运行具体回归。拒绝宿主Apply回滚是已明示独立安全事务，不冒充nativeSetParam返回值 |
| RL03 XML内容/清旧顺序 | ab50解析成功后清集合、缺OutputParam E17；FilePath/FileData只在后段提交；Data.Parse/Apply | 已有154事件差分和各坏XML/FloatList/姿态数组；本轮复用全部32执行场景，不凭测试数当覆盖率；异常结构防内存故障/非有限数值拒绝仍独立边界 |
| RL04 九路float覆盖 | c520构造九名→GetFloat(index0,count)→ret0/count>0覆盖本次对应分量 | **确定性缺口**：Runtime已接受NumberArray首项，九口Descriptor却Number scalar，真实Graph/桥拒绝数组。拟专属NumberArray+deferred读，保文件值/错误忽略/本轮不污染缓存；需真实Graph+host数组/空/失败/恢复 |
| RL05 RefreshSignal | c520 GetInt(index0)，ret!=0 signal0，success value!=0再SetParam(FilePath) | **共享运输缺口**：Integer scalar合同挡IntegerArray；不能把int改float或猜空数组为绑定成功。向根线提交最小Integer首项读取合同/候选，原调度不改 |
| RL06 动态额外IO选择/逐点覆盖 | 9410只Name/ValueType/IsResultShow精确子串筛选；c100按每次getter实际count循环，float每index失败才尝试int，最后覆盖 | 旧extra-array fixture通过，不代表所有IsArray=false源可达。当前Extra.Copy材料化支持数组，但Descriptor按IsArray固定scalar/array、UpdatedDescriptor在extra写前构造；需核真实数组来源接入及actual输出schema，不能只直调Copy蒙混。typed per-read error/fallback也需核合同，优先原c100全体 |
| RL07 动态端口/下游 | Runtime实际Data构建UpdatedDescriptor→Graph事件/结果→MainWindow卡片 | 旧846+共享68/WPF通用动态检查可复用；本工具真实MainWindow中刷新、删名/改型、旧线阻断需完整自动自审；额外IO新增实际输出kind要同步descriptor |
| RL08 文件缓存/保存恢复 | Save cf10开关FileData，DynamicIn/Out独立；Load bef0无路径也可FileData恢复 | 已有原受控save/load；产品CachedFileData JSON为独立运输。补实际文档PrepareSave→保存重开→源文件消失仍可run、关闭save不保存缓存、空path+Refresh清缓存 |
| RL09 运行显示/失败恢复 | c520输出顺序/ModuStatus按集合非空，非重新验矩阵 | 现LastDisplay/Runtime fields，已用资源隐藏图像/文本、记录显示；真实host本轮结果/动态schema/失败无旧值需核。独立NaN安全失败不声称native错误等价 |
| RL10 Close/Remove/Dispose | 析构9150、模块原集合/字符串释放；配置session关execution/event | 当前纯托管集合/短期文件流；需自有scope配置重开保缓存/新scope隔离/Remove重建验证，不能靠GC泛称native句柄逐地址等价 |
| RL11 Reset/ROI | 原资源只有Continuous/Stop/Execute/Confirm，没有Reset和ROI组 | 原Algorithm默认XML仅FilePath空与SaveCalibDataEnable true，不从同名API虚构正常UI Reset；工具无ROI活动入口，不补ROI编辑器 |
| RL12 提供者边界 | XML int/float订阅树、Remote文件请求、DynamicIO元数据 | 当前typed root-flow服务消费要实接；Global/Local/父Group/Special/真实FTP由接口提供者定义，明确Unsupported/mock，不自动当算法缺失；人工验收不作门 |

## 具体源确认与实现边界

ReadFileConfigData.ReadData在GetParam失败或描述为空白时直接return -1，不清CalibParam。现ReadCalibFileConfigurationPanel在空描述保留旧Children是源一致分支，不是凭常见刷新语义清旧UI。BottomCommandGroup原资源2938起只有四命令，未出现Reset/ROI。

当前已确认RL04不能仅Runtime直接字典输入通过就关闭；该输入必须通过本产品真实图/配置订阅/host自运行路径可达。九pose专属descriptor/Runtime读取已落盘，测试准备在`tests/ReadCalibFile.InputChecks`，尚未编译/运行；根线同程序集Board构建期间产品源已冻结。RL05/RL06完整10共享文件候选在`analysis/read-calib-file/proposals/typed-input`，原默认严格规则保留、只有显式opt-in才保raw scalar/array及独立int读取相位；未应用生产。

补取完整Frame链已纠正旧索引失败Count结论，见`READ-CALIB-FILE-FRAME-COUNT-CORRECTION.md`，根线待审最小一行共享修复；这是先前已完成工具的确证缺陷修复，不能重新包装成额外provider未知。32个旧成功Process/846断言仍仅历史，本轮没有预报通过。

额外IO逐写叶已闭合：Frame SetFloat130f50→122800→c98b0→f4880→f3fe0，int对应f3ee0。fb90仅首次写初始化并清标志；同一轮已有count时，index<count覆盖，否则只append一项，不填中间洞，也不截旧尾。故本轮先发布文件9项、extra再成功写两项时应保后7项；现Extra.Copy整数组replace是确定性差异。数值合并候选已准备，仍待完整专属落盘/descriptor及自动验证，不能用旧846无对应fixture声称已闭合。

RL02失败后隐式重读也已定位：Load设置Requested为失败新path，文档参数拒绝后仍旧path，下轮Configure误读旧文件。拟仅在配置失败分支将Requested标回已接受参数，保Load既有部分Data/Error及旧Path/Cache；测试覆盖旧文件外改、malformed保旧内容、wellformed缺Output清旧/保新输入、显式Refresh才再读旧path。UI错误弹窗保独立诊断边界，d120原指令d2b5调用ab50、d2ba MOV EDI,EAX、d62d MOV EAX,EDI，证明加载状态实际返回UI；旧反编译unaff_R12/探针void声明不作为返回语义证据。


2026-10-19 typed套件实际应用并验证：Input/typed/Extra/真实宿主63，Plugin846，Configuration48；CameraIndex259、WorkflowSubscription58、ConfigurationExecution14、FrameworkExecution68、Debug30全部退出0。日志及SHA见analysis/b-next/verification-manifest.json。48配置断言仍是RL02修改前基线，不代表失败保留已修。RL02完整两文件候选file-failure.patch SHA D7F0FC22C3756E249B7706B86ADF8DE478F45E79D49323C39F008B513C6E6034，尚待根线审落及新针对回归，故标定加载尚未交完整计数。上文候选未应用/未运行描述保留为历史。
