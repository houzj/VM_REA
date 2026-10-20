# Rotate typed生产验证（2026-10-19）

本批typed接口已验证，**整工具尚未计完整**：FilePath原OpenFileForCalibDialog的目录和CurValue尝试值策略仍需修。Reset16/Lifecycle6及数值旧证据保持有效，不互相代替。

两Int/三Float显式opt-in，Runtime取消提前物化；typed Session按Refresh→matrix valid→RotNum→X→Y→Angle实际门读，保存原错误/Count0返回/输出presence/清采样。14例原oracle含源非零poison但目的槽未写和磁盘删除后Refresh不误触证明；Count0/error不消费ReadResult的poison Value。见TYPED-INPUT-SOURCE。

| 项目 | 实际结果 |
|---|---|
| RotateCalibration.TypedInputChecks | PASS179：原14返回/状态/输出presence/读序、Value3/1/999 poison、真实Graph Int/Float数组All/index1/正负越界及Scope优先 |
| RotateCalibration.ResetChecks | PASS16：原唯一空默认E1/两订阅解绑/保缓存和Rev、host拒写不污染后续 |
| RotateCalibration.LifecycleChecks | PASS6：采样/配置关闭/scope隔离/移除/显式Reset |
| RotateCalibration.Checks | 720数值、1190含状态文件、1259含WPF，0fail |

四build/run均0，0warning/error，脚本exit0，managed已释放，无产品后改。仅生产ProjectReference，无候选Compile。旧materialized Session.Run仅归档fixture兼容，本次typed读序以生产Runtime/真实Graph及新原oracle独立证明。

manifest analysis/rotate-calibration/typed-final-validation/final-manifest.json SHA4c32c893e64f1e999abc800e08f3e224e7d34963d2d554e995f3091aed74c8b6；生产Calibration DLL7c04f0fe4f2be409129c733e7058a159154b924b44a6f30231c04dd58031db97。RotatePlugin608411.../Session570edf.../Reader308d64...及其它源码/资源/依赖DLL/log身份见manifest。
