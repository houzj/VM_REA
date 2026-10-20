# C1：畸变工具 workspace 生命周期源码索引

2026-10-19。本轮完成有限S1/S2切片：实际模块→SDK→core对象→每轮临时descriptor→MVDCommon池的所有者、分配、复用、归还与CRT释放路径。没有运行新算法探针、没有产品补丁，也没有据此决定安全托管兼容策略。S3运行、S4实现、S6宿主验收不因此完成。

## 身份、源码与获取边界

SDK为`MVDImageCalibCpp.dll`，core为`MVD_Algorithm.dll`，池为`MVDCommon.dll`，上层模块为`IMVSImageCalibModu.dll`。原文件路径、SHA256、每个方法及汇编导出哈希见`distortion-calibration-c1-workspace-source-index.json`。本轮新源码均位于本worktree `analysis/distortion-calibration/c1-workspace-source/{module,wrapper,core,common}/output/`；既有`7ed0c0`等源码另在索引标复用。

REA本轮`binary_session`观察为open=false、Ghidra provider not_configured（未设GHIDRA_INSTALL_DIR），没有切换共享目标或改全局配置。经根线授权改用现有私有Ghidra项目和隔离进程补缺；来源记为私有Ghidra导出，无REA Evidence ID，不补造。common首次导入的自动分析报两条DemangledVariable建Data错误，文件导入及所选方法/汇编导出成功；不据此声称全部binary已恢复。

## 按实际操作的源码覆盖表

| 操作 | 具体方法/地址 | 字段、分支、生命周期 | 当前状态 |
|---|---|---|---|
| 模块持有SDK工具 | Module `CIMVSImageCalibModu::Init` RVA203f0、`ProcessAlg`20820 | `this+15d98`为空才Create；Init汇编2044e明确`LEA RCX,[RBX+15d98]`，不是无参创建。Run复用同一槽 | 源码及ABI已闭合 |
| 模块Reset/销毁 | Module `ResetMember`21530、`DeInit`1f9f0、析构1f470 | ResetMember只清结果状态；另两者取槽到RCX调用Destroy，再清槽0。析构继续基类清理 | 源码及ABI已闭合 |
| SDK对象创建/初始化 | wrapper Create1000→3fe0 | 分配68 bytes对象；句柄槽+20。4018调用前RCX=&slot、EDX=69、R8D=0 | 源码及ABI已闭合 |
| SDK Run/销毁 | wrapper2a70→core Process；Destroy1070→scalar deleting destructor2590→cleanup41c0 | Process使用operation67；41cd先把句柄槽装RCX，41d8 DestroyHandle后清槽；其它SDK子对象分别销毁，最终delete SDK对象 | 源码及ABI已闭合 |
| core工厂/ctor | core CreateHandle14930→allocator1571c30→ctor1bff0 | algorithm69进入608-byte calibration family；allocator保留传入RCX调用operator new。ctor设虚表181ead878、algorithm字段+130=69、descriptor区+1b0初始清零 | 源码已闭合 |
| core Init | 虚表+8→1c210→1cdc0 | 参数/句柄/descriptor初始化；旧persistent容量足够时保留type1/2，只分配type0；首次或容量增长按mask7分配；成功后替换并释放旧type1/2 | 已取并追主路径；异常回滚细节仍逐分支开放 |
| core Process分派 | 152b0→虚表+18→1c4e0 | null/许可检查；配置准备007120成功后，释放旧+600缓存，operation67→1e330。各switch返回后736c0(mask1)，包括callee错误返回；不把前置准备失败说成执行了该释放 | 源码及ABI已闭合正常/返回错误路径 |
| descriptor请求 | 7c4750，8项、每项78 bytes | +0 size64，+8 alignment，+10 type，+18/+20地址，+28 owned。descriptor0 type1、descriptor1 type0，二者alignment128/owned1 | 源码已闭合 |
| descriptor分配 | 735f0→72ec0→61f0 | 依type0/1/2映射mask1/2/4；非零请求且owned1才`AllocateAlignedPoolMemory(size,align,&pointer)`，写回descriptor地址；失败回滚已分配项 | 源码已闭合；没有清scratch |
| descriptor每轮归还 | 1c4e0末尾1c6ab设置R9D=1→736c0→73030 | 只命中type0；owned1且地址非0才FreePoolMemory，再清descriptor+18。算法context旧地址值不等于仍busy或仍归该工具所有 | 源码及ABI已闭合 |
| 算法context/子块 | 7c5340、7c7360、7c7470 | descriptor0的+18承载context；descriptor1底部先留alignment后的公共区，context+1e0记录该基址，按算法功能推进后context+1e8记录共享scratch尾区 | 源码已闭合布局主链 |
| 畸变调用scratch | op67的1e330→7c6ba0(opf)→7f7050→7ecea0→7ed0c0→7ff3c0 | 7f7050取context+1e8，沿包装栈参数传为7ed0c0 workspace；具体模型/点数改变内部子块，不能用一个捕获偏移外推所有n | 源码已闭合到已复用拟合布局 |
| core析构 | Destroy15270→虚表[0]1c140→1c170→1d890→1d550 | 释放算法context，736c0(mask6)归还persistent type1/2，释放结果malloc数组，再清对象字段；另处理+600。不是仅把指针置0 | 源码及关键ABI已闭合 |
| 独立ReleaseWorkingMemory | 导出156e0→虚表+50→22c90 | 只对对象+600调用FreePoolMemory并清0；不是全部descriptor scratch的统一释放入口 | 源码已闭合 |
| 独立ReleaseResultMemory | 导出156c0→虚表+48→bef0 | 实际空操作返回0；不能从API名称推测释放了结果数组 | 源码已闭合 |

## 尺寸及拟合子块

令`A(x)=Align128(x)`。`7ecd50(n,&size)`要求n≥4，使用`5*A(8n)+max(8n,A(max(A(8n),F_7ff240(n),F_800580(n),F_853360,F_8535a0(n),F_857920)))`，对总大小最高位做溢出失败检查。具体helper含原生参数保留，本文保留函数名形式，不把未展开helper常数猜成固定容量。该项作为畸变需求参与`7c4750`/`7c5340`按功能取最大scratch，descriptor1还含前部公共子区，因此不能将其总size等同7ed0c0可用尾区size。

已恢复7ed0c0把outer中的前5个`A(8*capacityX*capacityY)`子区用于工具阶段，固定fit内层再按实际n布局。7ff3c0内部依次为：normalizedSource=`2*A(8n)`、normalizedTarget=`3*A(8n)`、designA=`4*A(8n)`、rhs追加`A(64n)`、Ti追加`A(8n)`、Tw追加128、pointErrors追加128、weights追加`A(4n)`、medianSort追加`A(16n)`。medianSort[-1]位于weights预留尾部，具体写来源已由前两轮n99→n80以及seq46→56、seq63→67确认。这里只把这些现象接到分配/归还链，不修改数值核。

## 池的真正所有者、复用与CRT释放

MVDCommon导出`AllocateAlignedPoolMemory` RVA8150、`FreePoolMemory`81a0共同使用DLL全局`DAT18001a780`，不是每个IImageCalibTool一份池。全局初始化13020调用15f0并注册atexit13070；池建立退出event及后台线程。

- `18f0`在锁内按alignment和size桶寻找未busy且capacity≥request、差额≤30MiB的块。复用分支只返回原指针、busy=1、更新时间、减少空闲计数，没有payload memset。无可用块才malloc；alignment>0时malloc(request+alignment+8)，向前对齐并在返回指针前8字节保存原malloc地址。首次系统malloc同样没有清payload承诺。
- `1e40`找到对应块后仅busy=0、记录释放时间、增加空闲字节计数；没有CRT free。失败/非法指针返回80100006并日志。这里的“FreePoolMemory”准确含义是归还池。
- `2630(pool,ageSeconds)`遍历未busy且空闲时间达到门槛的块，再对未对齐指针直接free，或对`aligned[-8]`中的原地址free，并从池记录移除。分配malloc失败时`18f0`调用`2630(pool,0)`清空闲块后重试一次。
- 后台清理方法`24f0`每轮WaitForSingleObject15000ms；正常timeout累计4轮后，在空闲总量大于`0x12c00000`（300MiB）时调用`2630(pool,30)`。这是源码策略，未在真实宿主观测其触发；线程入口1570由1710以函数指针交给线程创建helper；1570的15d7汇编明确CALL24f0，不用计时推断替代调用证据。
- atexit13070→1800先2110遍历清理池内块，再SetEvent、等待线程结束、关闭event和互斥资源，随后销毁容器。2110对非零块使用与alignment一致的CRT free；这一层仍不是Windows VirtualFree证据，CRT/系统heap最终何时归还OS在本任务范围外。

因此可从源码解释：每轮type0被归还后，旧payload仍可能留在进程全局池，再被同/另一工具实例重新领取。工具Destroy和pool归还均不构成payload清零；但也不能保证下一次同址或同旧值，池压力清理、容量/对齐/桶选择及其它消费者都会改变它。

## 产品选择、验证与精确剩余项

没有产品修改。当前托管运行会话与artifact缓存不等同上述进程池；不把捕获prefix写进方案，也不将native越界写推广到安全托管核。

可供用户另行选择的独立策略：①保持内存安全且确定的数学核，并公开历史scratch差异；②仅在隔离兼容测试中给定完整显式workspace上下文，越界超过受控buffer立即失败；③原生隔离进程兼容模式依赖同版本原库和真实池生命周期。三者均不是本轮已实施方案，②也不等于重建了真实宿主默认值。

验证为静态获取和一致性审查：核对四个binary身份；导出方法体、完整原始汇编和虚表88 bytes；明确检查Init/Create/Destroy的RCX保留、正常返回后mask1、析构mask6以及池归还路径无CRT free。没有新算法运行或设备测试。源码索引记录hash及所查方法状态。

剩余项：1cdc0容量增长/分配失败的所有回滚及C++异常路径尚未逐分支闭合；core尺寸query中未展开helper与所有功能组合上限；global pool线程创建失败、event/等待失败等完整错误分支尚未逐项闭合；其它插件对同一全局池的所有调用集合未枚举；真实VisionMaster进程实际加载的DLL路径/哈希、并发/配置/长时内存压力尚无运行确认。C2排序起始索引、偶奇点数和越界前提保持独立任务，不因C1解释了来源而关闭。

