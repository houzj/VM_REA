# 分支字符串：源码恢复索引

2026-10-20最终实现/验证见BRANCH-STRING-IMPLEMENTATION-EXIT.md。旧“尚未比较/待native”断点已由具体体补齐，不覆盖当前状态。

原安装Cs/Control的ILSpy及BAML-XAML、原XML在analysis/branch-string/source-20261020/{cs-decompiled,control-decompiled,resources}。校正IAT签名后的native-signatures为权威静态C/asm：Create196d0→init17840→vtable23a18；Process17890；Get17ff0→18610；Set181e0→18970；拓扑18e30→183c0/18ec0；Save196c0→1df90；Load193b0→1e0a0→193d0；Reset1bfd0；destructor177c0。native初次GetNext一参伪码曾删比较块，已以二参原ABI恢复，初稿留作失败历史。

Process输入StringA,index0,capacity0x100；error/Count<1清下一表不写输出；outLen<1为独立门。Frame11d440证明outLen=strlen+1，空输入outLen1仍可匹配空默认。flag恰1先追加(D)，否则精确全串相等；真实后继匹配才追加ID，最终文本非空即status1。GetNext0结果是空串；有后继而无匹配是No Module。静态恢复后未执行原native。

Frame Save实际12fca0→virtual+20→GetAll19390→GetCurrentConfig132fe0→1257a0，按原Algorithm唯一ModuleInfoList GetParam(cap0x400)；Load通过SetParam重入strtok，因此空字段位移是源行为，不能Save前归一化。具体Frame C和显式线性asm在frame/；最初未materialize listing的空asm导出不是无指令证据。真实Load专用共享hook已根线落，Plugin56/Host25验证Save与Load分离，JSON/NodeId的容量适配边界单列。
