'use strict';
const fs=require('fs'),path=require('path'),crypto=require('crypto'),assert=require('assert');
const {root,source}=require('./evidence-view.cjs');
const doc='src/VisionEditor.Core/GraphEditorDocument.cs',contracts='src/VisionPluginContracts/Contracts.cs',image='src/VisionPlugins.ImageSource/ImageSourceAssets.cs',read='src/VisionPlugins.Calibration/ReadCalibFilePlugin.cs',rotate='src/VisionPlugins.Calibration/RotateCalibrationPlugin.cs',store='src/VisionFramework/SolutionStore.cs',catalog='src/VisionFramework/GeometryResources.cs';
const ref=(p,member,anchor,count=1)=>source(p,member,anchor,count);
module.exports=function(){
 const facts=[],add=(id,stages,from,to,behavior,representation,cache,release,refs,unknowns)=>facts.push({id,classification:'independent-implementation',stages,modes:['all'],from,to,behavior,representation,cache,release,refs,unknowns});
 add('PST01',['prepare','persistence'],'ToolPluginRegistry.PrepareSave旧资源目录','逐节点资产投影与新Resources容器',
 '先以输入document.Resources构造ResourceCatalog，之后另建resources字典：过滤所有已注册IToolAssetPersistence插件的plugin:<Manifest.Id>:前缀资源，不要求这些插件当前一定还有节点。逐procedure/node调用participant.PrepareSave，并传scope.ForProcedure；每个snapshot.Resources按Id覆盖新字典，snapshot.Node成为新节点。旧catalog仍允许后面的参与者读取输入资源。任何参与者异常会中断ToArray与return，但此前参与者对scope或自身state的副作用没有跨参与者回滚代码。',
 '旧ResourceCatalog字典、新resources字典、节点投影是不同容器；catalog保存ResourceDocument引用，不复制编码文件字节。新snapshot.Node没有PrepareLoad式Id/ModuleId/版本一致性检查，最终结构校验是另一步。',
 '旧输入资源不因过滤即被删除；新字典的Id覆盖只改变拟保存文档。尚未返回的新投影不能算编辑器已提交。',
 '此入口没有外部文件删除、旧document/history清理或参与者state恢复事务。局部容器失去引用不能表述为已观察到GC释放。',
 [ref(contracts,'ToolPluginRegistry.PrepareSave scoped projection',' public SolutionDocument PrepareSave(SolutionDocument document,string baseDirectory,ModuleExecutionScope? scope){',7),ref(catalog,'ResourceCatalog retains resource records','    public ResourceCatalog(string baseDirectory,IEnumerable<ResourceDocument> resources)')],
 ['未枚举所有已注册参与者的副作用；PST02/03只给两个实际插件族的具体分支。','当前独立实现的静态合同，未以真实保存失败运行注入验证，不证明原版等价。']);
 add('PST02',['prepare','persistence'],'ImageSourcePlugin.PrepareSave路径选择','编码资源副本、Base64文档字段及scope状态',
 '先Clone默认值及节点参数。订阅文件夹模式会通过scope.State取得或创建State；路径来自匹配目录的state.FolderPaths，否则读取存在的目录，否则空表。SolSaveImageData为true时仅paths.Take(120)嵌入：优先现存磁盘文件，文件不存在才经旧节点EmbeddedImageResources/旧catalog回读；单个编码流超过256MiB即抛错。CopyTo内存流后ToArray产生byte[]，SHA256及Base64得到ResourceDocument与路径→Id映射。ImagePaths仍写入全部paths的绝对路径，故120条嵌入上限不等于路径表截断。',
 '新嵌入的是编码文件字节及Base64字符串，不是GrayImage像素缓存；资源Id包含插件Id、nodeId和编码SHA。SelectedPaths、CurrentImagePath、SubscribeFolderPath也转绝对路径；EmbeddedImageResources每次重建。关闭SolSaveImageData时新assets/map为空，不自行删除磁盘原图。',
 '旧catalog作为缺失磁盘文件的读取后备；save可能新建的scope.State在后续保存失败时仍由scope持有。此方法没有清理既有解码Cache、PendingFrame或显示结果的调用。',
 '输入流和复制MemoryStream均using；成功时返回的资源持有Base64字符串。临时bytes/流缓冲的GC时点未指定。后续方案32MiB限制可能在插件已投影之后拒绝保存，不逆向撤销此前scope.State创建。',
 [ref(image,'ImageSource scoped PrepareSave full branch',' public ToolAssetSnapshot PrepareSave(NodeDocument node,string baseDirectory,ResourceCatalog resources,ModuleExecutionScope? scope){',18),ref(image,'OpenEncodedImage old embedded resource fallback',' public Stream OpenEncodedImage(',4),ref(catalog,'OpenVerified ownership and hash verification','    public Stream OpenVerified(string id)'),ref(store,'Post-plugin document size limit','    public static void Save(string path,SolutionDocument solution) {',8)],
 ['文件夹监控并发更新与120条外图的可移植性未运行验证；不能声称所有路径均已嵌入。','不会从本分支推断GrayImage最终释放；编码临时分配与像素/结果持有分属不同链。']);
 add('PST03',['prepare','persistence'],'标定插件PrepareSave','持久scope/fallback状态先同步，随后输出参数副本',
 'ReadCalibFile.Save先GetState，再在state.Gate内Configure；Configure会先置Initialized/Requested，再按首次缓存、空路径或磁盘读取分支Apply，更新LoadError及按ReplaceData/CommitPath条件更新Data/Path/Cache。随后才克隆节点参数并按SaveCalibDataEnable输出CachedFileData。Rotate.PrepareSave同样Get并Synchronize：首次可从36字节CachedMatrix恢复9个float，否则按AppliedPath差异Session.Load并写AppliedPath，随后把当前Matrix序列化为新Base64 CachedMatrix及Loaded标志。故参数字典的隔离不等于插件state也隔离。',
 'scope存在时state放入按插件/节点/流程定位的ModuleExecutionScope；无scope时各插件fallback字典以绝对directory|node为key。Read缓存XML字符串/typed Data；Rotate缓存会话矩阵，再复制成byte[]及Base64参数。',
 '保存触发的Initialized/Requested/AppliedPath等同步可留在原state；SolutionStore尚未开始时就可能发生。随后序列化、写文件或Move失败，没有由GraphEditorDocument.Save恢复这些state的代码。',
 '此处锁保护插件state同步，不是跨文档/文件事务。新参数仅随最终文档提交；state的生命期仍由scope或fallback持有者控制，不因投影失败自动Dispose。',
 [ref(read,'ReadCalibFile GetState/Apply/Configure',' State GetState(',3),ref(read,'ReadCalibFile save synchronizes before parameter clone',' ToolAssetSnapshot Save('),ref(rotate,'Rotate scoped and fallback state','    State Get(string node, string directory, ModuleExecutionScope? scope)',5),ref(rotate,'Rotate Synchronize mutations','    static void Synchronize(State state, IReadOnlyDictionary<string, JsonElement> p, string directory)',17),ref(rotate,'Rotate PrepareSave caches matrix','    public ToolAssetSnapshot PrepareSave(NodeDocument node, string directory, ResourceCatalog resources, ModuleExecutionScope? scope)',11),ref(doc,'Save has no participant rollback','    public void Save(string path) {',7)],
 ['本条只确认当前两个插件族；Session内部Load/Restore数值算法和全部异常不在本切片展开。','未执行保存失败探针；静态无回滚不等于所有失败都会改变state，取决于具体分支和旧状态。']);
 add('PST04',['prepare','persistence'],'SaveAs的Copy/Rebase与SolutionStore.Save','路径字段变换、文件提交、编辑器提交分离',
 'Copy给Resources建立新数组但仍使用原资源record引用；参数JsonElement.Clone、ROI顶点数组等另行快照。Rebase再次Copy，只改每个ResourceDocument.Path及ModuleId恰为vision.image-file的字符串Path参数：绝对路径原样保留，相对路径先按旧base解析，再相对新base表示。任意插件私有路径不被通用Rebase扫描；ImageSource自己的绝对化来自PST02。Rebase不复制外部图像或标定文件。SolutionStore.Save完成结构/JSON检查、临时文件写入Flush(true)、Move覆盖且finally正常结束后，doc才写document/BaseDirectory/FilePath/IsDirty/Revision并发Changed。',
 '旧document、prepared、rebased、UTF8 json及磁盘临时文件各是不同层；当前实际API名是SolutionStore.Save，没有独立SaveAtomic方法。',
 'PrepareSave/Rebase/store在返回前抛错，doc这行提交未执行，旧编辑器字段仍保留；这不撤销PST02/03状态副作用。若Changed订阅者抛错，则编辑器字段与文件已提交，调用仍可异常结束；外层目录偏好/Recent更新只在doc.Save正常返回后发生。',
 '临时文件由finally尝试Delete，异常仍可传播；不把该单方案文件替换协议扩展为插件state、外部资产、历史payload或偏好文件的全域原子事务。',
 [ref(doc,'Copy container and JsonElement identities','    static SolutionDocument Copy(SolutionDocument d)=>'),ref(doc,'Selective Rebase path transformation','    static SolutionDocument Rebase(SolutionDocument d,string oldBase,string newBase) {',4),ref(doc,'File then editor fields then Changed','    public void Save(string path) {',7),ref(store,'Exact persisted file write boundary','    public static void Save(string path,SolutionDocument solution) {',8),ref('src/VisionEditor/MainWindow.ShellSaveSolution.cs','Outer Save preferences after doc.Save','    bool Save(bool asNew)',20)],
 ['原子覆盖的文件系统/崩溃持久性未在本切片实测；源码Flush/Move不能证明多文件事务。','通用路径规则不保证每个插件资源的跨机器可移植性；只能按其具体参与者分支判断。']);
 add('PST05',['history','persistence'],'GraphEditorDocument.Save保留既有history','旧BaseDirectory快照与SaveAs之后的RestoreSnapshot',
 'Save没有history.Clear、solutionGeneration递增或SolutionReplaced事件，而Load在提交时明确执行这些操作。Snapshot保留ProcedureId与创建时BaseDirectory，并持有MemoryDocument或payload；Capture把Copy(document)作为快照输入。因此已有历史快照可继续保留旧Resources/嵌入Base64，即使保存投影的新Resources已过滤或替换；并非每次Save必有这样的快照。RestoreSnapshot用snapshot.Document及snapshot.BaseDirectory按当前BaseDirectory重新Rebase，然后替换document/选择流程/标dirty/Revision/Changed，不修改当前FilePath/BaseDirectory，也不写方案文件。',
 '编辑器当前document与历史快照是不同持有层。快照保存的是文档数据，不是ModuleExecutionScope/state、GrayImage或运行结果的自动副本。',
 'SaveAs改变当前base后，旧快照仍携自己的旧base，恢复时转换到当前base；不能说保存成功就清空旧资源或刷新全部运行期缓存。历史payload的租约与删除失败由专门HP事实另述。',
 'Save本身不Dispose历史；快照退役由历史引擎/编辑器生命周期处理。本事实不声称字节、文件或资源在Save成功时已经释放。',
 [ref(doc,'Snapshot document and base-directory holder','    sealed record Snapshot(',6),ref(doc,'Save keeps history while Load clears it','    public void Load(string path) {',16),ref(doc,'Capture copied document','    Snapshot Capture()=>'),ref(doc,'CaptureSnapshot memory or payload holder','    Snapshot CaptureSnapshot(SolutionDocument value,string procedure,string directory)',8),ref(doc,'Restore uses snapshot old base and current base','    void RestoreSnapshot(Snapshot snapshot) {',4)],
 ['历史中是否实际存在含旧嵌入资源的快照取决于此前操作，未把条件持有推成所有Save的必然内存驻留。','这里只确认文档历史与路径边界；payload文件租约/删除失败、GUI回放与原版行为分别保留其独立证据。']);
 const writeBoundaryTable=[
  {phase:'准备资源集合',holder:'registry局部catalog/resources',fields:'旧ResourceDocument引用；新Resources按插件前缀过滤/按Id覆盖',writeTime:'参与者调用之前与逐节点返回时',success:'新资源表随prepared返回',failure:'投影未返回；早先参与者state可能已经变更；旧文档资源仍在',factIds:['PST01']},
  {phase:'图像资产投影',holder:'ImageSource局部values/assets/embedded；scope.State',fields:'全路径表；最多120个编码资源Base64；可能新建State',writeTime:'SolutionStore序列化前',success:'新参数和资源进入prepared',failure:'using流退役；scope状态无跨层回滚；像素缓存未由此清理',factIds:['PST02']},
  {phase:'标定缓存同步',holder:'scope或fallback State',fields:'Initialized/Requested/Data/Cache/LoadError；Session/AppliedPath',writeTime:'参数Clone及文件写入前',success:'参数缓存取同步后的state',failure:'state可能已变，文档尚未提交',factIds:['PST03']},
  {phase:'路径重基',holder:'rebased文档副本',fields:'Resources.Path；vision.image-file.Path',writeTime:'PrepareSave之后、磁盘写入之前',success:'文件与后续编辑器使用rebased',failure:'旧编辑器base/file/document未因本步骤改变；不搬移资产',factIds:['PST04']},
  {phase:'方案文件替换',holder:'SolutionStore局部json/temp；文件系统',fields:'UTF8字节；临时文件；目标方案',writeTime:'验证/32MiB检查→Write/Flush(true)→Move→finally',success:'正常返回后允许编辑器提交',failure:'异常向外传播，临时文件尝试删除；不恢复插件状态',factIds:['PST04']},
  {phase:'编辑器与外层记录',holder:'GraphEditorDocument；MainWindow偏好/recent',fields:'document/BaseDirectory/FilePath/IsDirty/Revision；Changed；偏好文件',writeTime:'store返回后提交字段；Changed后doc.Save返回；然后外层更新',success:'文档路径及最近方案更新',failure:'Changed或外层更新失败时前面文件/字段已可能提交，无全链回滚',factIds:['PST04']},
  {phase:'历史保留与回放',holder:'Snapshot MemoryDocument或payload',fields:'旧document/ProcedureId/BaseDirectory',writeTime:'保存前既有快照；回放才Rebase到当前base',success:'Save不主动清history；回放只改当前文档为dirty',failure:'不声称Save释放旧嵌入数据；payload失败另看HP事实',factIds:['PST05']}
 ];
 const sourceFiles=[...new Set(facts.flatMap(f=>f.refs.map(r=>r.path)))].map(p=>{const b=fs.readFileSync(path.resolve(root,p));return{path:p,sha256:crypto.createHash('sha256').update(b).digest('hex'),byteLength:b.length};});
 const existing=JSON.parse(fs.readFileSync(path.resolve(root,'docs/architecture-recovery/execution-storage.edges.json'),'utf8')).edges;
 const existingContext=existing.filter(e=>['J02','J03','J04','J05'].includes(e.id)).map(e=>({sourceDocument:'docs/architecture-recovery/execution-storage.edges.json',sourceKey:e.id,record:e}));
 for(const f of facts){assert(f.refs.length&&f.unknowns.length);assert(f.classification==='independent-implementation');}
 return{schemaVersion:1,complete:false,facts,writeBoundaryTable,existingContext,unknowns:[{id:'PSTU01',target:'whole-save transaction',question:'单方案文件写入、插件state同步、历史payload、外部资产与外层偏好属于不同提交域；没有跨全部域回滚合同。'},{id:'PSTU02',target:'runtime/original corroboration',question:'本批仅核当前实际源码分支；未运行故障注入/GUI/设备，不代表原版或所有插件资产保存行为。'}],provenance:{sourceFiles,runtimeExecuted:false,originalRuntimeExecuted:false,targetCodeExecuted:false,productModified:false,otherWorktreeRead:false,implementationCredit:0,coverageDelta:'Specific resource/state/path/history holders and failure boundaries beyond CP06 and J02-J05 overview; HP lease/delete details excluded.'}};
};
if(require.main===module){const model=module.exports(),output='docs/architecture-recovery/current-persistence-data-evidence.json';fs.writeFileSync(path.resolve(root,output),JSON.stringify(model,null,2)+'\n');console.log(JSON.stringify({output,facts:model.facts.length,refs:model.facts.reduce((n,f)=>n+f.refs.length,0),sources:model.provenance.sourceFiles.length,tableRows:model.writeBoundaryTable.length}));}
