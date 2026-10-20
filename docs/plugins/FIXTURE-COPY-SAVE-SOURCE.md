# 位置修正：Save/Copy具体原指令与实施

2026-10-20。仅静态原模块/平台链；唯一运行探针是原MSVCR120格式叶，无原Fixture/SDK/Frame加载或设备调用。

## GetParam→保存

原Fixture GetParam RVAa430，asm a507/a524/a53e/a558/a572/a58c分别从this+10/+14/+18/+3c/+40/+44加载float，a591 CVTPS2PD提升double，a594装入%f，a59b缓冲len参数，a5a1 MOVQ R9,XMM3，a5a6经IAT调用sprintf_s。flag从+6c/+68读后%d。原GetAll ae80调用VM_M_GetCurrentConfigByFile，Frame1257a0按原Algorithm八成员逐项调用virtual+10 GetParam(cap0x400)，Saveab60调用CAbstractUserModule.SaveModuleData再追加DynamicIn/Out blob。正文与Frame具体体继承前一已核相同ModuleFrame身份，非以另一个工具数学作证。

原PublicFile/x64/msvcr120.dll SHAAD32240BB1DE55C3F5FCAC8789F583A17057F9D14914C538C2A7A5AD346B341C；FormatProbe只绝对路径Load此CRT→sprintf_s(%f,double(float))，默认locale=C。12例原叶run SHA15350C40BD5C554DCAFE0B16614B149522F26D5A5EB41D22925E022C999A3AB6，含.0078125精确中点→.007813、负中点→-.007813、负零、相邻float、最大finite、小数/下溢量化。不是原GetParam运行，也不证明VM运行进程更改过的locale。生产只恢复解析后float，使用Round(double(float),6,AwayFromZero)再float，显式保零符号；最大值原字符串有效数字差异不冒完整文本等价，最终parsed float bits12例逐项一致。

源Save不写回模块Core；产品GraphEditorDocument.Save会提交prepared document。专属SaveProjection逐字段识别此次保存投影：值等投影只确认Seed，别的真正用户编辑才SetCore。不能整组匹配失败后将其余量化字段回灌。真实Host覆盖Save→仅改FixInitAngle→首次Run，RefX仍源精确tie且Angle确实应用；Load新scope才恢复量化。PrepareSave失败未提交时首次Sync丢未匹配投影，不改原Core。

## Copy完整具体调用链

1. 原VmModuleSet.cs1398..1403：创建新tool后ModuleManager.CopyModuleParam(old.ID,new.ID)。本Fixture Cs无Paste/PasteFinish override；VmModule基类两虚方法为空，不凭继承假造额外工具命令。
2. VM.PlatformSDKCS.ModuleManager1420..1428→ServerSDKAPI→NativeMethods IMVS_CopyModuleParam；原iMVS-6000PlatformSDK.dll SHA B62D7E15588CCDA6671228AEBC11FEC3E27ACA878F0E03067E44B17D5A54A9A1；export RVA648bf0→6785f0，构造命令0x410再6c00d0请求。
3. 原PlatformManager SHA06D86F51BEBFEBA357D1B79297E20A3B0A70580A40AF7B77C8ABDD6C38CA0624；602df0 switch410→6280b0解析源/目标ID→5e7be0→67a530构造JSON command0xBE9，description明确Copy module parameters from srcModuleId to dstModuleId，keys srcModuleId/dstModuleId；5de710派发。C包装若显示void/丢参，以线性asm寄存器ABI为准。
4. 原ModuleProxy SHA76F09A40184B2B5F99B7173EDDE23A716D339E3C64955696D6927495EFDF68A9；04a610 caseBE9解析两ID→024310。实际指令：MOV EAX,EDX; MOV R9,RCX; MOV EDX,R8D; MOV ECX,EAX; JMP [R9+a8]，最终callee只有源/目标两个uint32，RCX不是this。
5. loader021c25装入字符串VA180146928=VM_M_CopyModuleNodeParam，021c32 CALL IAT18012d138（原PE import确认为KERNEL32.GetProcAddress），021c38将RAX写table+a8；并非凭函数名关联。string-xrefs工具在旧DB无ref，继而原PE RIP相对LEA扫描定位021c25再显式线性反汇编；初UNDECODED窗口在指令中部不作代码依据。
6. 原ModuleFrame SHA45C57BC46F72FD1F9E0B3A6F9183FF974D14590CFD05ECF4E9FA97A4FF95AE9B；export13a0f0→040d50。先查源/目标各module+128对象，分配SAVE0x611a8/LOAD0x61198；source vtable+28 SaveModuleData，结果0或E0000019继续复制buffer，随后dest vtable+30 LoadModuleData。不是只拷固定doc参数，也不是拷原对象heap。
7. Fixture自身Save/GetAll输出当前八参数（六float经%f）；自身Load aa70→SetAll ad10依序SetParam，额外只DynamicIn/Out blob。尺度不在八项，目标构造仍0。原八参复制时必须取live源scope；新宿主窄IToolScopedNodeCopyProvider只此工具启用，旧接口保原行为。

## 产品对应与验证

FixturePlugin.PrepareCopiedNode接root提供的同源过程活节点+sourceScope，仅源确认八字段经Save格式量化写目标参数，source pose/SaveProjection不动，目标不复制缓存尺度。无同过程活源时null scope使用immutable fragment的旧安全边界，非原已销毁VmModule对象仍可复制的行为保证。

Plugin最终43通过，其中12原CRT parsed-float bits；真实MainWindow Host最终30通过：POINT数组实际选择→leaf输入首项、模式持久化、教学/显示/Debug、Paste当前Run15而doc仍旧值、复制Ref中点量化/目标Scale0/source原pose不变、Save/单项编辑/Load、删除。copied destination的typed Runtime核对由真实Paste结果调用当前生产Runtime，未谎称该一步另执行完整MainWindow工作流。源主节点的运行/自运行/保存/加载/删除均实际MainWindow入口。

原native未运行，输出setter写失败/heap损坏/外部远程provider、locale改变/任意非法SDK字符串不在此已验证正常域；不因为43/30通过称全平台或字节ABI等价。
