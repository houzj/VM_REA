# Map双输入路径base控件失败状态审查

2026-10-19，源码静态核对；候选未落产品，整工具继续开放。

## 原链

安装导出 `analysis/decompiled/Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/OpenFileDialog.cs`：55-59先写CurValue再ValueChanged；168-197仅SetParamUnit==0时SetSuc→PreExecuteRun。控件没有derived suffix/目录预校验。NameValueItem.cs:904-924中Check异常才Value=OldValue；但Check默认空（1129-1135）。927-973原native桥异常路径只标SetParamErro、OnSetValueCompleted(false)、return-1，不恢复Value；864-870事件只通知，没有本类回滚。该能力审计全项目检索SetValueCompleted订阅，仅CNN衍生控件注册自己的处理，未见OpenFileDialog处理。

picker：OpenFileDialog.cs:127-128 SetCurValue后无条件INISetDefaultOpenPath；163-165先null再path，故同坏值可重试。前端picker本身CheckFileExists/CheckPathExists均true；手输没有这两个预门。远程使用其真实远程分支，本地实现不能宣称FTP等价。

Map原资源CalibPath0/1确是OpenFileDialog，不是OpenFileForCalibDialog。原模块 `analysis/map-calibration/ghidra/module-analyzed/180023870.c` SetParam分别ReadFileMatrix(side0/1)，read非零直接返，不保存新路径；既有ReadFileMatrix证据允许失败时先替换矩阵缓存，不能把旧路径保留理解成缓存完整回滚。源码/哈希复用MAP-CALIBRATION-STATE-LIFECYCLE-AUDIT。

## 产品差异与有界补丁

生产FileSession.SetInputPath已有正确Replacement/成功才path提交规则。配置native error普通Error导致State默认rollback却与上述原UI尝试值不符；picker的if(node.Value==path)也错误跳过history。成功还缺base控件SetSuc通知。

候选 `analysis/map-calibration/proposals/base-input-path/base-input-path.patch` SHA256 E2E5220E6A2280E3DD0C77A04FBB737BC945805CA5EAA6847511F378ABA8A753：仅CalibPath0/1失败KeepAttemptedValueOnError，保留真实hex错误并通知；成功adapter提交后SetSuc（State再决定PreRun）；picker无条件Remember。未对base端口添加derived校验。已有通用NotifySave改名NotifyConfiguration以准确表达共享用途。此为专属候选，产品冻结未apply、未构建。

待专项：双侧原reader失败样本→native路径不变、矩阵按Replacement事实变化、UI仍尝试值、document只保存成功path、同坏值picker再次触native且history；成功通知且下一运行使用实际两文件矩阵。全原厂WPF运行仍是独立人工记录，不以静态与托管测试替代。

## 后端非XML分派补核（区别UI与native）

1800227d0.c:60-85按'.xml'四bytes子串查找，未找到goto 22b7c；该标签后直接_wfopen/ftell/fseek，无扩展名检查。side0分配fileLength+1，side1分配0x25，fread长度为ftell，读全才成功出口；SetParam23870 read返回0继续提交SDK两文件参数与path。因此36bytes非XML子串的.payload是原binary实际支持路径，测试不是由base UI不验证推后端允许。本次测试失败样本使用存在XML但缺CalibOutputParam/CalibFloatListParam（原E0000018、尚未替换matrix）；不使用3byte短binary推原错误，因为托管只允许36byte是独立安全选择，原source短binary并无同一长度门。原binary不安全大小域不执行、不扩证据。
