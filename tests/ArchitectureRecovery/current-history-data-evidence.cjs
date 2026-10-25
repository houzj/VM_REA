'use strict';
const fs=require('fs'),path=require('path'),crypto=require('crypto'),{root,out,source}=require('./evidence-view.cjs');
module.exports=function(){
 const doc='src/VisionEditor.Core/GraphEditorDocument.cs',store='src/VisionEditor.Core/HistoryPayloadStore.cs',engine='src/VisionEditor.Core/DocumentHistoryEngine.cs';
 const R=(p,m,a,n)=>source(p,m,a,n),facts=[];
 const add=(id,from,to,behavior,representation,cache,release,refs,unknowns)=>facts.push({id,classification:'independent-implementation',stages:['history','retirement'],modes:['all'],from,to,behavior,representation,cache,release,refs,unknowns});
 add('HP01','GraphEditorDocument.Capture → CaptureSnapshot','配置文档副本 → 内存快照或独占JSON文件载荷',
 'Capture先Copy(document)，再SerializeToUtf8Bytes。仅当没有显式historyPayloadParentDirectory且字节数小于64KiB时，Snapshot直接保存传入的文档副本；其余情况创建HistoryPayloadStore并写独占.payload。Snapshot还保存ProcedureId和BaseDirectory。Restore通过Snapshot.Document读取/反序列化，再从旧BaseDirectory重基到当前BaseDirectory，替换document并Dirty/Revision/Changed；没有把workflowSessions、scope或工具像素FIFO装进这个Snapshot。',
 'MemoryDocument是SolutionDocument结构副本；Parameters逐JsonElement.Clone，Resources复制数组，ResourceDocument承载路径/哈希/可选EmbeddedBase64字符串；文件型Snapshot持HistoryPayloadLease，不是原生sol。',
 '快照捕获配置、文档资源表示与当时的基目录，不捕获所有运行对象。64KiB边界以序列化字节计，不是图像像素缓存容量。',
 'Snapshot.Dispose只Dispose文件载荷租约；内存快照没有逐字段资源Dispose协议。结果会话、provider和scope各自退役。',
 [R(doc,'Snapshot representation','    sealed record Snapshot(',7),R(doc,'Capture and CaptureSnapshot','    Snapshot Capture()=>',9),R(doc,'RestoreSnapshot','    void RestoreSnapshot(',4),R(doc,'Copy document fields','    static SolutionDocument Copy(',1),R('src/VisionFramework/Contracts.cs','ResourceDocument','public sealed record ResourceDocument(',1)],
 ['仅源码级配置快照合同；不证明原版undo载荷或当前完整GUI操作等价，也未据此测量内存峰值。']);
 add('HP02','HistoryPayloadStore.CreateMemory / AcquireFile / Retain','字节复制、共享Entry引用数与借用文件权限分开',
 'CreateMemory调用bytes.ToArray，Read内存条目再次ToArray；文件Read调用File.ReadAllBytes。AcquireFile把路径归一并按OrdinalIgnoreCase查既有Entry，命中只增加References并返回新租约；未命中且文件存在时登记BorrowedFile，不取得删除权。Retain同样增加Entry引用数；它不会产生新文件或复制原文件内容。',
 'HistoryPayloadLease是单个持有者，Entry是同路径/内存载荷的共享计数项；Memory、OwnedFile、BorrowedFile三个Kind决定载荷与权限。',
 '同路径命中复用原Entry的Kind，包括已有OwnedFile的删除权；不能把每次AcquireFile都说成借用或深复制。',
 '只有最后引用退出才进入该Entry的实际清理；BorrowedFile不删外部文件，Memory只清自身数组持有。',
 [R(store,'Payload kinds','public enum HistoryPayloadKind',1),R(store,'CreateMemory copies bytes','    public HistoryPayloadLease CreateMemory(',6),R(store,'AcquireFile identity and ownership','    public HistoryPayloadLease AcquireFile(',14),R(store,'Retain and Read','    internal HistoryPayloadLease Retain(',9)],
 ['路径大小写策略是当前实现的比较器选择，不是原版路径或跨平台文件身份等价证明；外部修改/删除借用文件的运行竞争未验证。']);
 add('HP03','HistoryPayloadLease.Dispose → Store.Release','先删除owned文件再撤Entry；失败可保留可重试租约',
 'Entry.References>1时只减计数；最后引用若OwnedFile先File.Delete，成功后才entries.Remove、Bytes=null、References=0。删除抛错时Entry仍live，Lease.Dispose保留owner并重抛；如果Entry已移除而随后目录清理抛错，catch依据IsLive=false把owner清空，不能把两类异常都当文件仍有可重试租约。Store.Dispose只closing，尚有Entry时等待最后租约，不强制遍历删除。',
 '租约owner、Entry成员资格、文件是否已删及目录是否已释放是四种状态；异常不是统一回滚。',
 '最后租约退出可能才触发ReleaseDirectory；目录还有未知文件时只返回，不递归删除。目录清理还验证.owner内容等于当前identity。',
 '本Store仅有独占目录/owned文件的权限；失败保留与重复Dispose是否重试取决于异常发生在Entry移除之前还是之后。',
 [R(store,'Release entry ordering','    internal void Release(',12),R(store,'Store closing and directory ownership','        lock (gate) { closing = true; if (entries.Count == 0) ReleaseDirectory(); }',16),R(store,'Lease.Dispose checks live state','public sealed class HistoryPayloadLease : IDisposable',25)],
 ['源码中两个Dispose方法处于不同类型，引用会保留各自完整区段；未用真实文件占用、权限故障或GUI并发作本轮运行验证。']);
 add('HP04','DocumentHistoryPart.AdaptOldValue / AdaptNewValue','先换当前payload，旧payload释放失败则单独保留',
 'AdaptOldValue先调用adapt产生next，非同引用时先将OldValue=next，再ReleaseReplaced(previous)；AdaptNewValue同理且要求IsDataComplete。ReleaseReplaced失败把旧值加入retainedValues并重抛，因此适配失败不表示原payload仍是当前值。Release分别尝试OldValue、已捕获newValue和retainedValues，成功项设标志或移出；Root逐part聚合错误，Engine把失败Root加入retainedReleaseFailures。显式ReleaseRetainedPayloads才重试这些Root。',
 '当前旧/新值、待释放旧值列表、根级失败保留列表分层持有；DocumentHistoryRoot/Part不是框架执行session。',
 'View-only快照适配可保留缩放而替换快照载荷；失败Root仍保留未成功释放的payload，不把已切栈当全部文件释放完成。',
 '此处有显式重试接口，区别于ModuleExecutionScope重复Dispose不自动重试的合同；不能跨模块套用同一种失败策略。',
 [R(engine,'Part adaptation and retained payloads','    public void AdaptOldValue(',37),R(engine,'ReleaseReplaced and Part.Release','    void ReleaseReplaced(',16),R(engine,'Root.Release aggregates part errors','public sealed class DocumentHistoryRoot<T>',21),R(engine,'Root failure retention and explicit retry','    void ReleaseRoot(',13)],
 ['未穷举adapt/capture/restore的全部异常组合；原动作历史与当前JSON载荷的全量对应继续未知。']);
 const files=[...new Set(facts.flatMap(f=>f.refs.map(r=>r.path)))].map(p=>({path:p,sha256:crypto.createHash('sha256').update(fs.readFileSync(path.resolve(root,p))).digest('hex')}));
 const model={schemaVersion:1,complete:false,facts,provenance:{sourceFiles:files,currentRuntimeExecuted:false,originalRuntimeExecuted:false,guiStarted:false,productModified:false,implementationCredit:0}};
 fs.writeFileSync(path.join(out,'current-history-data-evidence.json'),JSON.stringify(model,null,2)+'\n');return model;
};
