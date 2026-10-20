# 完整VM_M读取链的Count失败写回更正

2026-10-19。原ModuleFrame.dll SHA256 `45c57bc46f72fd1f9e0b3a6f9183ff974d14590cfd05ecf4e9fa97a4ff95ae9b`。本轮独立Ghidra项目、33体初次导出及定点补叶在`analysis/read-calib-file/frame-source-20261019`；不依赖原桌面/设备。源码来源为安装原DLL，未修改DLL或共享REA配置。

旧相机索引文档把float叶fd90“先写数组Count，再返回越界错误”直接当完整VM_M_GetFloat返回Count；这遗漏两层门，结论撤回。完整普通NodeOutput订阅链：1301f0→11f5f0→ca720→f61e0→fd90。GetInt为1300f0→11e580→ca500→f5f40。

* float叶fd90正越界可写底层Count，但ca720在ca7b0先清自己的临时Count；ca7d6调用叶；ca7dd TEST返回；ca7df JNZ跳过写回；仅成功在ca7e1/ca7e8把临时Count写到上层。int ca590/ca5b6/ca5bd/ca5bf/ca5c1/ca5c8同样。
* 上层11f5f0在11f857写本地Count=R13D（前文XOR R13D,R13D）；读取失败不改此0；11f9d6…11f9e3仅成功且indexKind!=All时设1；11fd62/66/6a最终写到调用者。
* 因此普通NodeOutput订阅的成功All保底层Count、成功constant/dynamic Count1；正越界/负越界/空数据读取失败Count0。没有订阅记录的E031b走11f852→11fd6c，绕过Count写回；这和“已有绑定但读失败Count0”必须区分。scope原协议提供者可保留其真实Count；本修复只纠正产品普通NodeOutput适配。

最小共享候选：`analysis/read-calib-file/proposals/full-count-correction/full-count.patch`，仅将WorkflowNumericInputRead正越界返回Count从底层长度改0。不改算法、容差、scope reader原协议。

专属Index测试改原Count2断言为0，并新增真实Graph：仅第一组活动，TX正越界、TY/OX/OY All两项应在TX/TY计数比较返回E0000001；旧实现会错误地把四count视作2、排除组并返回E0000309。四叶均越界则四Count0/读取失败，组被排除，返回E0000309。现测试已准备，尚未取得托管构建槽、未宣称通过。

原指令文件SHA256：ca720 `C7965C38301A5570088EB5FCB189B3BE530F773510B6E72AD2FAC4A2C5C79456`，11f5f0 `4E399575C54A8F5A5815E5B3D2E6DCE3582CBB4D9C3DE21E056F267F0F55A6A7`。
