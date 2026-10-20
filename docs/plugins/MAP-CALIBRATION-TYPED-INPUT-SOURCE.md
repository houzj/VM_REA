# Map六口 typed输入真实caller恢复（2026-10-19）

根线撤回Map完整计分，因旧生产六个标量As读取未闭合原可达数组/原协议。旧源码/数值/Reset/ROI证据保留，本切片不以它们替代此接缝。

| 原口 | 原caller及谓词 | 显式产品合同 |
|---|---|---|
| PhysicalPointX/Y/WorldRotateAngle | GetInputImage203d0在每侧新图实际成功后按X/Y/A各index0调用GetFloat；槽初始化X=-999999/Y,A=-999；caller丢返回，不独立Count门 | NumberArray + SupportsDeferredNumericRead，保留成功的标量兼容；仅该侧新图时读，transport未产生有效值则保留初始化槽，不提前物化其它分支 |
| 三float的Teach分支 | Process21690先把Teach三槽全部-999；X→Y→A，首次非零return原码早退；不检查Count（成功Count0也继续）；partial字段已写不可全回滚 | scope/execution typed读原码、Count，更新逐字段槽并按原返回门停止；Count0不伪造0 |
| Trigger/TeachFlagInput | 原XML GetFrontFilterTree(string)没有数组Count过滤。只Image+TeachEnable读，两次index0/cap512；只第二return、两Count>=1及strcmp相等门。首return忽略 | Text + SupportsDeferredTextRead，scope→deferred→materialized；UTF8容量/NUL兼容，首错误不自行新增拒绝门 |
| RefreshSignal | File模式GetInt index0；返回0且输出值非0刷新，Count未独立判断，Image分支不读 | Integer + SupportsDeferredIntegerRead；不以As<long>提前拒绝数组/索引错误 |

原Process ASM 217f2/21828传0x200；21848..2185e第二返回/两Count；218cc/218f8/21924的三float及各TEST/JNZ。原资源float selectors 298/335/372、string 481/519、int572，类型可达并无只标量过滤。正常完整VM_M_GetFloat/GetInt的失败Count0及无订阅不写Count差异复用READ-CALIB-FILE-FRAME-COUNT-CORRECTION；不能把底层叶Count当完整读结果。

## 原模块定点oracle

MapTypedInputOracle.cpp在自有进程加载原hash658d63...；constructor、正常SDK tool attach与源确认字段、IO import替换沿既有许可，无Platform Init/设备。10例exit0，真实capacity512/slot初值/返回/读序在typed-input-oracle/module-file-results.jsonl，identity/probe/原体hash在provenance。首版缺SDK/image getter前置失败保留first-failed-results，修宿主后才得本结果，不冒充产品失败。

2/3/4例分别X/Y/A错误E0000123，原partial[-999,-999,-999]/[12,-999,-999]/[12,13,-999]、后续读取停止；5例X成功Count0原继续且X=-999；6例首text失败Count1、其空buffer与第二空串相等仍触发；7例第二text失败挡；8/9例511bytes成功/512bytes容量不足不触发。scope协议未知“错误时仍修改出参”的任意实现不可由只含Value的接口推断为普遍支持；产品适配正常Frame读的成功写槽/失败保持，不伪称caller有Count/return gate。

## 候选和真实Graph测试

proposals/map-typed-input/map-typed-input.patch SHAdf6cd4807410e7113367dbb169b6c05e9f766caa2abbcadee5d20e07c5749001，仅MapPlugin+新专属reader，apply-check通过，未落/未构建。六口scope服务优先、execution deferred次之、materialized兼容最后，禁止共享改动。

MapCalibration.TypedInputChecks已准备：原10oracle逐项重放门/partial/flag/调用顺序，真实Graph数组All/index1/正负越界及模式门，普通新frame失败保留初始化且继续三读；仅ProjectReference生产Map，无候选Compile。结果以实际构建日志为准，不因候选存在恢复完整计分。
