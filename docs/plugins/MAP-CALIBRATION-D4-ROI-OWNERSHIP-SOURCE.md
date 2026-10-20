# D4 双 ROI setter 所有权有限源码切片（2026-10-19）

本切片从原 SDK 虚表恢复两个 ROI setter、克隆校验和析构释放，未改产品、未执行原厂、未新增实图。D4 仍只计已批准 S0，D 总 5/32，整卡0/4。

## 身份与静态入口

原件 `D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVDMapCalibCpp.dll`，SHA256 `fb373e8ef51572f1ea377a1218bc3fcc230e61f8fa72264d6715445467b4b75a`。复用既有 `analysis/map-calibration/ghidra/wrapper` C；原目录无 asm，故用身份匹配私有 Ghidra 工程、`-noanalysis -readOnly` 定向补汇编。REA provider 不可用沿用先前状态，无 REA Evidence ID，不改全局配置。新原始产物 `analysis/map-calibration/d4-roi-ownership/source`；`source-index.json` 含原 PE 字节、导入名、头文件与 C/asm hash。

历史 wrapper 工程的函数 body 有仅首指令的情况；初次导出空/单指令 asm 不作为完整证据。导出助手改为明确起止的有限字节区间逐条解码（脚本中列出），末尾可能含对齐 INT3；32a0区间还包含3330处两条相邻setter指令，32a0自身RET在3327，本报告不将3330算入析构。不据区间归属推断每条指令都可达。私有数据库变更不保存。

`3340` 构造基本参数对象，分配0x40字节双图参数对象，写 vptr=`180010f88`；board=7，各图/ROI/region指针为0，字节+8/+9为0。同一原 PE 直接读取 vtable：

| 槽 | 地址 | 原头文件含义/实际字段 |
|---|---|---|
| 0 | 1800019c0 | deleting destructor →19f0 |
| +0x28 | 180001aa0 | GetSourceROI，直接返回 this+0x18 |
| +0x30 | 180001ab0 | SetSourceROI |
| +0x58 | 180001b60 | GetTargetROI，直接返回 this+0x30 |
| +0x60 | 180001b70 | SetTargetROI |

槽名称由原 `Includes/Algorithms/MapCalibCpp.h` 的 `ITwoImageMapBasicParam` 声明次序确定，入口由 PE 指针字节确认，非按函数邻接或托管 clone 推测。原模块 `ProcessAlg` 两侧非空ROI向量取首shape传+0x30/+0x60的入口证据见 `MAP-CALIBRATION-D4-DUAL-ROI-ENTRY.md`。

## setter 与失败/替换顺序

两 setter 对称，source字段=+0x18、标志=+8，target字段=+0x30、标志=+9。

| 实参条件 | 源码/汇编行为 |
|---|---|
| incoming=null、旧值=null | 直接返回；无 clone、无 destroy、标志不写。 |
| incoming=null、旧值非null | `DestroyShapeInstance(old)`，对应字段置0，再返回；标志不写。不能描述为 null 自动设 dirty。 |
| incoming==stored、非null | 指针相等直接返回；不重新校验type、不clone、不destroy、不写标志。 |
| incoming非null且不同 | 临时 new=0 → `1c00(this,incoming,&new)` → 若旧值非0调用 DestroyShapeInstance(old) → 标志写1 → stored=new。先 clone 后 destroy，不先清旧值；source与target各自处理。 |

调用 `DestroyShapeInstance` 的原 PE thunk 为 `18000e1f4`，字节 `ff2546210000` → IAT `180010340` → `MVDShapeCpp.dll!DestroyShapeInstance`。原头文件声明接收一个 `IMvdShape*` 并返回 int；反编译显示的 `func_e1f4()` 无参是漏参，实际 asm 中 RCX 是所销毁的旧字段值。返回 int 未用于判定 setter 成败；不能假设 destroy 报错会阻止覆盖旧指针。

`1c00` 检查 incoming非null且输出临时指针当前为0，否则抛错误 `0x80100006`。调用 incoming虚表+0x18取type，仅接受5/6/10；其他类型抛 `0x80100001`。原 `MVD_ShapeParamDefine.h` 将它们映射为 Rectangle/Circle/AnnularSector。随后调用 incoming虚表+0x88，将 RAX 写给输出临时指针。

原 `MVDShapeCpp.h` 基类槽顺序为 destructor、operator=、operator==、GetShapeType…IsValid；三个支持的派生接口新增类型专属 operator=/operator==，其后 Clone 位于+0x88。原头文件将 Clone 定义为深拷贝接口，失败抛异常。这里源码确认 **调用 Clone 并保存其返回值、由 SDK 对其执行 destroy**；未取三个具体 Shape 类的 Clone 函数体，不能额外声称已验证全部字段深复制/分配布局。

clone/类型检查抛异常发生在 destroy old 之前，所以从可见的 setter 顺序看旧字段与标志尚未被替换。若一个实现违背接口返回null而不抛异常，此 setter 无额外非null断言：仍可能销毁旧值、写标志1并存null；此为静态分支边界，未构造或运行非标准 shape。

getter返回内部存储指针，不再clone。因此“相同指针无操作”是与 stored 比较；连续传同一外部原始shape通常仍会重新调用Clone，不能把它等同于输入内容相同缓存命中。Clone是否返回同地址这一非标准情形未运行，不将接口约定替代具体对象证据。

## 析构链与释放对象

`19c0` 调用 `19f0(this)`；若 deleting标志bit0为1，再经 `18000e250` 调用 `MSVCR90.dll!operator delete(void*)` 释放参数对象本身（导入修饰名 `??3@YAXPEAX@Z`）。

`19f0` 将vptr回写双图类型，按字段顺序：source image +0x10只清0；source ROI +0x18非0则DestroyShapeInstance后清0；source region image +0x20只清0；target image +0x28只清0；target ROI +0x30非0则DestroyShapeInstance后清0；回写基类vptr；target region image +0x38只清0。本析构没有对四个image字段调用destroy，不能把ROI持有策略扩展到image字段。

`32a0` 基本参数析构对 this+0x10（双图参数）及+0x18（双文件参数）各自调用虚表槽0，EDX=1，然后清成员。由双图槽0接19c0→19f0，ROI释放可连到基本参数对象析构；模块何时触发SDK整个工具销毁/重建另属后续生命周期。

## 对模块与产品的有限含义

模块 `ProcessAlg` 发现某侧ROI向量为空时跳过setter；SDK setter本身支持传null清理，这两条路径不同。只有确认同一SDK参数对象持续使用、且中间无其他clear/reset时，“跳过setter”才会保留此前ROI；本切片不据单一函数断言整个模块必然保留缓存。

产品位置仍为 `MapCalibrationPlugin` 的两组ROI、`MapCalibrationImageSolver` 的值式ROI输入；现有托管clone/裁剪不是上述所有权链的原证据。无需把SDK内部裸指针生命周期复制到托管实现，也未提出共享补丁。

## 明确后续

- 文件 op0x6f：本次定向检索现有Map报告及索引，未发现同hash核心注册/转发体证据。保持 `MVD_Algorithm_Process` 到7fbec0的注册桥缺体单列，不因wrapper操作号或文件方向体补齐而视为闭合。
- 三个实际形状Clone内部字段复制、DestroyShapeInstance具体实现尚未取体；本切片闭合的是调用接口与SDK参数对象的持有/释放顺序。
- 模块空ROI向量跨轮行为、形状创建/角度桥、GetProcessInput、工具销毁时机、显示与结果释放仍需各自有限切片，未扩大所有SDK/CPU路径。

验证仅包含原件hash、PE虚表/IAT解析、定向C/汇编核对及索引复现；无产品测试或真实宿主验收新增，不能据此宣称完整运行一致。
