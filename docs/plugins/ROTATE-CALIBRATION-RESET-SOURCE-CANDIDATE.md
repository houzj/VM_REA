# Rotate Reset：空默认项的完整源码链与实施候选

2026-10-19。静态取证，无Platform Init/设备/原厂UI reset执行；复用B身份匹配原件，不修改其项目。

## 原默认项实际落到哪里

安装RotateCalibModuAlgorithm.xml有效唯一ParamItem是FilePath、DefaultValue空。ModuleFrame SHA45c57bc46f72fd1f9e0b3a6f9183ff974d14590cfd05ecf4e9fa97a4ff95ae9b。原导出复用analysis/camera-mapping/b3-reset-source-20261019/frame：141ee0→581e0→125100→e5cb0→e8220。空DefaultValue留初始化空串，但仍插pair-vector，无跳过；默认正常写经42760。其原指令429a8..429c6给SetParam实际this/name/value/strlen(value)，不是len+1；失败最后非零保EBP，继续后42a45返回。

Rotate自身ctor372c0的原指令372e2/372e9把vtable18004ec78写this；该PE+18槽地址18004ec90原8bytes指向180038980 SetParam。新增纯被动PE数据在analysis/rotate-calibration/final-audit/reset-vtable.json，原module-disasm.txt保全部dumpbin原指令；模块hash b08486b8481ed6d7774634b30b5cae167f85b911ebf394122663bab400a9bb41。

SetParam38980入口明确param3!=0才处理；空默认长度0直接返回0xe0000001，不触及FilePath/矩阵有效/矩阵字节/采样/RevOutputFlag。复用B RETURN-SOURCE闭合Frame→Proxy响应errorCode→Manager Err*code→SDK恢复错误→托管VmException→普通Reset按钮catch；正常SP响应路径错误不被序列化成功吞掉。因唯一默认失败，不执行成功刷新/成功通知；之前IsSubA=false解绑不回滚。不是实际桌面操作记录。

## 候选与验证入口

analysis/rotate-calibration/proposals/reset/rotate-reset-source-list.patch SHA8cec01e798e404b464b511623ada4302ac781df163da55cff0e004a57af71bc2，普通git apply --check通过。未应用/未构建，src冻结。

共享CalibrationToolConfigurationProvider只新增可选algorithmResetResource；有值时调用既有公共ToolConfigurationAlgorithmReset，不另写解绑循环。Rotate传原resource，增加专属CheckResetParameter空FilePath门返回E0000001；该门在默认写时才用，不改常规文件编辑。未传可选参数的Translation调用/旧行为不变。原资源显式EmbeddedResource（项目非XML通配）。同步宿主有效值供解绑后保留，不清缓存。

tests/RotateCalibration.ResetChecks仅ProjectReference生产，检查两实际ISubInfo按序解绑、取消、不调用Atomic/Run、非名单InputWay/RevOutput保留、默认空路径拒绝/错误无成功通知、矩阵/采样/path/关闭配置保留与显式Reset移除。tests/RotateCalibration.LifecycleChecks检查正常采样→打开关闭配置→Reset及scope独立/RemoveNode。二者均未构建。随后重放旧RotateCalibration.Checks原数值/状态/文件/显示/WPF必要集；旧1259只是历史数量，以本次实际日志为准。
