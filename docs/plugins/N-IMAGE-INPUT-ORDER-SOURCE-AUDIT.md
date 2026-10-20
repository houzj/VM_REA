# N图像输入读取顺序与末帧修正门

2026-10-19，根线接续整工具自审。当前安装模块SHA256 `68C04FC9782E873A80776D6EA4BC1E0515B87D54E9D13B41DBD73799747525EC`。复用已有Process@20b90 C体后，为避免按伪代码ABI猜测，使用自有现成Ghidra工程readOnly/noanalysis定点重新导出一函数，输出`analysis/n-image-calibration/input-read-source/output/180020b90.{c,asm.txt}`，自有作业exit0；未执行原厂、未保存项目，不冒充REA EvidenceID。REA provider限制沿用既有已确认记录。

20d76/7a/7e将三个Count槽清0；20d82..20dfa分别把Count槽地址置[RSP+20]（Win64第5参），R9置value槽，R8D=0，调用MovePhysicalPointX、MovePhysicalPointY、RotateAngle三个GetFloat。三次均执行，未据EAX状态短路。随后检查三个Count，任一0就把三个motion值一起改为原sequence自动生成值。不能把GetFloat显示签名的四参当没有Count，也不能把类型为int的源静默当成功float读。正常树源可为数组，实际读取index0。

21011..2103b在实际链表插入图像并增加frameCount。21097..210bc先要求count非0且count>=translationCount+rotationCount；不满足即跳过修正输入和ProcessAlg。210be再检查this+160d0(HomoFixEnable)。只有满批且启用才按PicPointX→PicPointY→PhysicalPointX→PhysicalPointY读取，均index0，各返回EAX成功才继续下一项；失败仍在append之后，随后ProcessOutput输出当前计数。四修正读复用一个Count槽，但该caller只判断返回码，不把Count作为第二成功条件。源成员写入位置为this+160d4/d8/dc/e0。

当前产品确认两处实施缺口：NImagePlugin.Number/Point只读物化scalar，不消费scope/read callback/数组首项；修正输入在每帧开始就读取并在未满批时判缺源失败。已有NImageModuleChecks“第1/9帧缺修正就失败”断言来自当前实现，不能覆盖原条件；必须改为普通未满批成功、末帧才失败且Count已增加，并保留原日志。旧数值求解、原37输出和完整float逆矩阵不因此失效。

此处产品冻结中（B typed运输批），只取证不改src。下一实施切片：七个数值输入显式deferred numeric opt-in；运动严格三读/Count门；修正满批+启用才顺序读/返回码门；必要scope优先、物化数组首项与兼容Point桥分别列为接口适配，不给旧直接Point多通道冲突猜原生优先级。专项固定读序、未满批不读修正、末帧短路/失败不丢已append图像，随后重跑现实际像素批次和配置/显示宿主。不得把本报告或候选当已修复。
