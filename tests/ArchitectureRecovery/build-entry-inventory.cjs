// Read-only declaration/candidate inventory. Never runs vendor code.
const fs=require('fs'),path=require('path'),crypto=require('crypto'),vm=require('vm');
const {root,out}=require('./evidence-view.cjs');
const sha=bytes=>crypto.createHash('sha256').update(bytes).digest('hex');
const rel=file=>path.relative(root,file).split(path.sep).join('/');
const menuPath='analysis/decompiled/VisionMaster.Exe.Audit/menuitems.xml';
const menuBytes=fs.readFileSync(path.join(root,menuPath)),menuText=menuBytes.toString('utf8');
// Purpose-limited parser for this text-only menu XML. Reject external entities,
// DTD and CDATA rather than interpreting or fetching them.
if(/<!DOCTYPE|<!ENTITY|<!\[CDATA\[/i.test(menuText))throw Error('Unsupported menu XML declaration');
const document={tag:'#document',children:[],text:''},stack=[document];
for(const match of menuText.matchAll(/<!--[\s\S]*?-->|<\?[\s\S]*?\?>|<[^>]+>|[^<]+/g)){
 const token=match[0];if(token.startsWith('<!--')||token.startsWith('<?'))continue;
 if(token.startsWith('</')){const tag=token.slice(2,-1).trim();if(stack.length<2||stack.pop().tag!==tag)throw Error('Mismatched menu XML');}
 else if(token.startsWith('<')){const parsed=token.match(/^<([\w:.-]+)\s*(\/?)>$/);if(!parsed)throw Error('Unsupported menu XML tag: '+token);const node={tag:parsed[1],children:[],text:'',line:menuText.slice(0,match.index).split('\n').length};stack.at(-1).children.push(node);if(!parsed[2])stack.push(node);}
 else stack.at(-1).text+=token;
}
if(stack.length!==1||document.children.length!==1)throw Error('Incomplete menu XML');
const decode=s=>s.replace(/&lt;/g,'<').replace(/&gt;/g,'>').replace(/&quot;/g,'"').replace(/&apos;/g,"'").replace(/&amp;/g,'&');
const leaf=(node,key)=>decode(node.children.find(n=>n.tag===key)?.text.trim()??'');
const languagePath='analysis/decompiled/VisionMaster.Exe.Audit/resources/zh-cn.xaml';
const langBytes=fs.readFileSync(path.join(root,languagePath)),langText=langBytes.toString('utf8');
const labels=new Map([...langText.matchAll(/<sys:String\s+x:Key="([^"]+)"[^>]*>([^<]*)<\/sys:String>/g)].map(m=>[m[1],decode(m[2])]));
const menu=[];
function menuChildren(node,parents=[],prefix=''){
 const children=node.children.find(n=>n.tag==='items')?.children??[];
 children.forEach((child,i)=>{const id=prefix?prefix+'.'+i:String(i),name=leaf(child,'name'),fields=Object.fromEntries(child.children.filter(n=>!n.children.length).map(n=>[n.tag,decode(n.text.trim())]));const hierarchy=parents.concat(name);const toolbar=child.children.find(n=>n.tag==='toolGroupItemInfo');if(name)menu.push({id:'menu:'+id,name,caption:labels.get(name)||name,hierarchy,source:{path:menuPath,line:child.line,sha256:sha(menuBytes)},fields,toolbar:toolbar?Object.fromEntries(toolbar.children.map(n=>[n.tag,n.text.trim()])):null,hidden:fields.isShow==='false'||fields.isVisible==='false',traceStatus:'open',edgeLinks:[]});menuChildren(child,name?hierarchy:parents,id);});
}
menuChildren(document.children[0]);
const baseline=JSON.parse(fs.readFileSync(path.join(root,'analysis/full-audit/batch31-20261020/A05.1/inventory/menu-entry-map.json'),'utf8'));
if(baseline.source.sha256.toLowerCase()!==sha(menuBytes)||baseline.entries.length!==menu.length)throw Error('Menu resource differs from reviewed baseline: '+baseline.entries.length+' vs '+menu.length+'; hash '+(baseline.source.sha256.toLowerCase()===sha(menuBytes)));
for(const row of menu){const old=baseline.entries.find(e=>'menu:'+e.id===row.id);if(!old||old.name!==row.name)throw Error('Menu hierarchy differs from prior resource parse');row.hierarchy=old.sourceHierarchy;row.hidden=old.hiddenInMenu;row.sourceResolvers=old.sourceResolvers;row.resolverAnchors=old.sourceAnchors;row.resolverEvidence='historical anchor; source file hash verified below';for(const a of Object.values(row.resolverAnchors)){const bytes=fs.readFileSync(a.path);if(sha(bytes)!==a.sha256.toLowerCase())throw Error('Menu handler changed: '+a.path);}}
const views=require('./view-registry.cjs').slugs.map(slug=>({slug,data:JSON.parse(fs.readFileSync(path.join(out,slug+'.edges.json'),'utf8'))}));
for(const row of menu){for(const {slug,data}of views)for(const e of data.edges)for(const anchor of Object.values(row.resolverAnchors)){const target=rel(anchor.path);if(e.refs.some(r=>r.path===target&&new RegExp('\\b'+anchor.member+'\\b').test(r.member))){row.edgeLinks.push({view:slug,id:e.id});row.traceStatus='partial-linked';}}}
const files=[],candidates=[];
function walk(folder){for(const entry of fs.readdirSync(folder,{withFileTypes:true})){if(['obj','bin','.git'].includes(entry.name))continue;const full=path.join(folder,entry.name);if(entry.isDirectory())walk(full);else if(/\.(cs|xaml)$/i.test(entry.name))files.push(full);}}
walk(path.join(root,'analysis/decompiled'));files.sort();
const manifest=[];
const csPatterns=[
 ['event-hook',/\b[\w.]+\s*\+=/g],
 ['event-unhook',/\b[\w.]+\s*-=/g],
 ['dynamic-registration',/\b(?:RegisterClassHandler|RegisterClassInputBinding|RegisterClassCommandBinding|RegisterHotKey|SetWindowsHookEx|AddHook|AddHandler|InputBindings\.Add|CommandBindings\.Add)\s*\(/g],
 ['command-construction',/\bnew\s+(?:[\w.]*DelegateCommand|[\w.]*RoutedCommand|[\w.]*RelayCommand)(?:<[^>]+>)?\s*\(/g],
 ['callback-boundary',/\b(?:public|private|protected|internal)\s+(?:static\s+|override\s+|async\s+)*(?:[\w.<>,?\[\]]+)\s+([\w]*(?:CallBack|Callback|OnStartup|OnExit|OnClosing|OnClosed|Main|Window_Closed|Window_Closing)[\w]*)\s*\(/g]
];
const markupKinds=name=>name==='KeyTime'?'animation-key-time-property':name==='EventName'?'event-trigger':/(?:^|[.:])(?:Command|Executed|CanExecute)$/.test(name)||/Command$/.test(name)?'command-binding':/^(?:Gesture|Modifiers|Key|MouseAction)$/.test(name)?'input-binding':/^(?:Preview)?(?:Mouse\w+|Key\w+|TextInput\w*|Drag\w+|Drop|GiveFeedback|QueryContinueDrag)$/.test(name)?'input-event':/^(?:Click|Checked|Unchecked|Loaded|Unloaded|Closing|Closed|SelectionChanged|ValueChanged|SizeChanged|DataContextChanged|SourceUpdated|TargetUpdated|ScrollChanged|GotFocus|LostFocus|Initialized|Expanded|Collapsed)$/.test(name)?'ui-lifecycle-event':null;
for(const full of files){const bytes=fs.readFileSync(full),text=bytes.toString('utf8'),relative=rel(full),digest=sha(bytes);manifest.push({path:relative,sha256:digest});let ordinal=0;
 for(const [index,line]of text.split(/\r?\n/).entries()){
  const found=[];
  if(full.endsWith('.xaml')){for(const m of line.matchAll(/([\w:.]+)\s*=\s*"([^"]*)"/g)){const kind=markupKinds(m[1]);if(kind)found.push({kind,target:m[1],value:m[2]});}}
  else for(const [kind,pattern]of csPatterns){pattern.lastIndex=0;for(const m of line.matchAll(pattern))found.push({kind,target:m[1]||m[0].trim(),value:line.trim()});}
  for(const f of found)candidates.push({id:'candidate:'+relative+':'+(index+1)+':'+(++ordinal),path:relative,line:index+1,sha256:digest,...f,source:line.trim(),evidence:'lexical-candidate',traceStatus:'unreviewed',closure:false});
 }
}
const gates=[
 {id:'E0',title:'原件与程序集范围',state:'open',gap:'28 个本地反编译树不等于全部随包程序集；各版本 SHA/MVID、依赖和遗漏程序集仍需对账。'},
 {id:'E1',title:'资源声明全集',state:'partial',gap:'四份模式菜单资源已解析并记录身份，默认资源 53 项已独立对照；其它 BAML/XAML 资源及动态生成菜单未穷尽。'},
 {id:'E2',title:'交互与局部输入',state:'open',gap:'全树扫描只提供候选；WPF 焦点/路由、附加行为、系统命令及 Gesture/Key 的 BAML 歧义仍需逐载体核对。'},
 {id:'E3',title:'后台与反向入口',state:'open',gap:'SDK/native/设备/外部触发、定时器和跨线程回调的注册与生产者需独立追链，不保证经过主窗点击。'},
 {id:'E4',title:'声明到 handler',state:'open',gap:'每条资源、反射 resolver、动态注册必须定位真实对象类型、handler 与启用/权限条件；词法匹配不算绑定确认。'},
 {id:'E5',title:'完整行为与回流',state:'open',gap:'每个入口必须追状态/身份、持久化、执行、回调、异常/取消/释放；已有局部边不能替代全链。'},
 {id:'E6',title:'入口与模型双向对账',state:'open',gap:'需保证每个已证实入口都有链，每条链都有入口或回调来源，重复别名共用证据但不丢声明。'},
 {id:'E7',title:'有限范围自审与运行证据',state:'open',gap:'先固定原件/载体范围，再逐入口审查。当前未运行原版；静态源、重建测试、原机证据分别记录。'}
];
const menuHierarchy={title:'主界面菜单与工具栏',key:'root',entryIds:[],children:[]};
for(const entry of menu){let node=menuHierarchy;for(const key of entry.hierarchy){let child=node.children.find(c=>c.key===key);if(!child){child={key,title:key==='[hidden-toolbar]'?'隐藏菜单 / 工具栏声明':labels.get(key)||key,entryIds:[],children:[]};node.children.push(child);}node=child;}node.entryIds.push(entry.id);}
const renderedMenuIds=[];function countMenu(node){renderedMenuIds.push(...node.entryIds);for(const child of node.children)countMenu(child);}countMenu(menuHierarchy);
if(renderedMenuIds.length!==menu.length||new Set(renderedMenuIds).size!==menu.length)throw Error('Menu hierarchy omits or duplicates declarations');
const data={schemaVersion:1,generatedUtc:new Date().toISOString(),complete:false,scope:'All present analysis/decompiled .cs/.xaml files excluding obj/bin; menu declaration inventory plus lexical candidates, not all installed original artifacts.',primarySource:{path:menuPath,sha256:sha(menuBytes)},languageSource:{path:languagePath,sha256:sha(langBytes)},counts:{menuNamedDeclarations:menu.length,toolbarDeclarations:menu.filter(e=>e.toolbar).length,hiddenNamedDeclarations:menu.filter(e=>e.hidden).length,sourceFiles:files.length,candidates:candidates.length,closedEntryChains:0},gates,menu,candidates,files:manifest,limitations:['Line-based XAML matching can miss multiline attributes; this remains a declared open gate.','C# lexical hooks include generated code, standard controls and non-entry statements; each requires semantic review.','analysis/full-audit supplementary sources, unextracted BAML/native and dynamic plugin assemblies are outside this scan.','No source file, plugin worktree, installed original, app process or product source was modified.']};
const structuredXaml=JSON.parse(fs.readFileSync(path.join(out,'xaml-entry-inventory.json'),'utf8'));
const lexicalCandidateCount=candidates.length;
for(const c of structuredXaml.candidates.filter(c=>c.evidence==='xml-element-declaration'))candidates.push({id:'candidate:'+c.id,path:c.path,line:c.line,sha256:c.sha256,kind:c.kind,target:c.target,value:c.value,source:fs.readFileSync(path.join(root,c.path),'utf8').split(/\r?\n/)[c.line-1].trim(),evidence:c.evidence,traceStatus:'unreviewed',closure:false,structuredXamlIds:[c.id]});
data.counts.lexicalCandidates=lexicalCandidateCount;data.counts.structuredSupplementalCandidates=candidates.length-lexicalCandidateCount;data.counts.candidates=candidates.length;
const structuralKey=c=>JSON.stringify([c.path,c.line,c.target.split(':').at(-1),c.value]);
const structuredByKey=new Map();for(const c of structuredXaml.candidates){const key=structuralKey(c);if(!structuredByKey.has(key))structuredByKey.set(key,[]);structuredByKey.get(key).push(c.id);}
for(const f of structuredXaml.files){const current=manifest.find(m=>m.path===f.path);if(!current||current.sha256!==f.sha256)throw Error('Structured XAML snapshot stale: '+f.path);}
for(const c of candidates)if(c.path.endsWith('.xaml'))c.structuredXamlIds=structuredByKey.get(structuralKey(c))??[];
data.structuredXamlInventory={path:'xaml-entry-inventory.json',view:'xaml-entry-inventory.html',counts:structuredXaml.counts,parser:structuredXaml.parser,complete:false,meaning:'Structured XML declaration location only; semantic binding not confirmed.'};
const handlerMap=JSON.parse(fs.readFileSync(path.join(out,'xaml-handler-map.json'),'utf8')),handlerEntries=new Map(handlerMap.entries.map(e=>[e.declarationId,e]));
for(const c of candidates)if(c.structuredXamlIds)c.handlerLocations=c.structuredXamlIds.flatMap(id=>{const e=handlerEntries.get(id);return e?[{declarationId:id,status:e.status,methods:e.methods}]:[];});
data.xamlHandlerLocation={path:'xaml-handler-map.json',view:'xaml-handler-map.html',counts:handlerMap.counts,metadataBaseEvidence:'image-list-base-metadata.json',connectorEvidence:'xaml-connector-review.json',complete:false,meaning:'Same-tree syntax plus one original-metadata-verified ImageListPanel external base; private-base accessibility, delegate and runtime binding remain unproven.'};
const remainder=JSON.parse(fs.readFileSync(path.join(out,'xaml-remainder-map.json'),'utf8'));
for(const e of remainder.entries.filter(e=>!e.coveredElementDeclarations.length)){
 const file=manifest.find(f=>f.path===e.path);if(!file||file.sha256!==e.sha256)throw Error('Stale remainder declaration '+e.id);
 candidates.push({id:'candidate:'+e.id,path:e.path,line:e.line,sha256:e.sha256,kind:e.kind,target:e.target,value:e.value,source:fs.readFileSync(path.join(root,e.path),'utf8').split(/\r?\n/)[e.line-1].trim(),evidence:'xml-remainder-with-root-method-location',traceStatus:'unreviewed',closure:false,remainderAttributeId:e.id,remainderMethodLocations:e.methods.map(m=>m.Id)});
}
data.counts.remainderSupplementalCandidates=candidates.length-lexicalCandidateCount-data.counts.structuredSupplementalCandidates;data.counts.candidates=candidates.length;
data.xamlRemainder={path:'xaml-remainder-map.json',view:'entry-scope-audit.html',counts:remainder.counts,complete:false,meaning:'Attribute value matches a same-tree root-class method; event/delegate property and runtime binding remain unknown.'};
const passwordUi=JSON.parse(fs.readFileSync(path.join(out,'password-ui-review.json'),'utf8'));
for(const c of candidates){const ids=new Set([...(c.structuredXamlIds??[]),c.remainderAttributeId].filter(Boolean));const matches=passwordUi.registrations.filter(r=>r.declarationCandidates.some(d=>ids.has(d.id)));if(matches.length)c.registrationCodeEvidence=matches.map(r=>({view:'password-ui-review.html',type:r.type,connectionId:r.connectionId,event:r.event,handler:r.handler,handlerToken:r.handlerToken,registrationCodeConfirmed:true,originalBamlMappingConfirmed:false,runtimeBindingConfirmed:false}));}
data.passwordUiRegistration={view:'password-ui-review.html',path:'password-ui-review.json',counts:passwordUi.counts,complete:false,meaning:'Original registration code confirmed; declaration association candidates and runtime activation remain separate.'};
const passwordCheck=JSON.parse(fs.readFileSync(path.join(out,'password-check-review.json'),'utf8'));
data.compiledEntryCarriers=[{type:'PasswordWindowUI.PasswordCheckWindow',view:'password-check-review.html',path:'password-check-review.json',artifact:passwordCheck.artifact,counts:passwordCheck.counts,complete:false,meaning:'Original CIL registration code; selected original BAML has now been decoded offline (password-baml-review.html). Static declaration association is separate from raw connection-ID verification and runtime binding; not mixed into local-source candidate count.'}];
const passwordEdit=JSON.parse(fs.readFileSync(path.join(out,'password-edit-review.json'),'utf8'));
data.compiledEntryCarriers.push({types:['PasswordWindowUI.PasswordSetWindow','PasswordWindowUI.PasswordModifyWindow'],view:'password-edit-review.html',path:'password-edit-review.json',counts:passwordEdit.counts,complete:false,meaning:'32 original registration sites plus Group caller state writes; selected original BAML declarations decoded offline (password-baml-review.html). Raw connection-ID correspondence and runtime binding remain unverified.'});
const globalToolbar=JSON.parse(fs.readFileSync(path.join(out,'global-toolbar-entry-inventory.json'),'utf8'));
data.additionalEntryResources=[{view:'global-toolbar.html',path:'global-toolbar-entry-inventory.json',resource:globalToolbar.resource,counts:globalToolbar.counts,complete:false,meaning:'Separate receiver/resource and public wrapper entry family; main-window host and runtime dispatch unverified.'}];
const groupFile=JSON.parse(fs.readFileSync(path.join(out,'group-file-review.json'),'utf8'));
data.groupFileProtection={view:'group-file-storage.html',path:'group-file-review.json',counts:groupFile.counts,passwordChannels:groupFile.passwordChannels,complete:false};
data.gates.find(g=>g.id==='E1').gap='四份模式菜单资源与独立GlobalToolItems.xml已清点；后者11个具名声明另列，不并入214主菜单声明。其它BAML/XAML、动态和外部宿主仍未穷尽。';
data.limitations[0]='Line-based XAML candidates are additionally cross-checked against secure structural XML parsing for the present 471 files; original BAML, markup semantics and future/unrecognized attributes remain open.';
fs.mkdirSync(out,{recursive:true});fs.writeFileSync(path.join(out,'entry-inventory.json'),JSON.stringify(data,null,2)+'\n');
data.menuHierarchy=menuHierarchy;
data.menuResourceVariants=JSON.parse(fs.readFileSync(path.join(out,'menu-variants.json'),'utf8'));
const toolExports=JSON.parse(fs.readFileSync(path.join(out,'tool-function-exports.json'),'utf8'));
data.toolFunctionExports={path:'tool-function-exports.json',view:'external-entries.html',pubAddress:toolExports.pubAddress,count:toolExports.count,complete:false,meaning:toolExports.meaning};
data.resolverMemberLocation={...JSON.parse(fs.readFileSync(path.join(out,'menu-member-map.json'),'utf8')).counts,view:'menu-member-map.html',complete:false,meaning:'Static declaration and public member-body location; no runtime binding or full behavior closure.'};
const selectedPasswordResource=JSON.parse(fs.readFileSync(path.join(out,'password-parameter-resource.json'),'utf8'));
if(sha(fs.readFileSync(selectedPasswordResource.artifact.path))!==selectedPasswordResource.artifact.sha256)throw Error('Selected password XML resource changed');
data.passwordParameterResourceDeclarations={manifest:'password-parameter-resource.json',artifact:selectedPasswordResource.artifact,declarations:selectedPasswordResource.nodes.map(n=>({element:n.element,name:n.name,line:n.line,hierarchy:n.hierarchy,fields:n.fields})),count:selectedPasswordResource.nodes.length,complete:false,meaning:'One explicitly selected shipped XML declaration; separate from main menu resources and .cs/.xaml lexical candidate counts. Runtime visibility and activation unproven.'};
const externalTools=JSON.parse(fs.readFileSync(path.join(out,'external-tool-declarations.json'),'utf8'));
data.externalToolDeclarations={path:'external-tool-declarations.json',view:'external-tools.html',count:externalTools.count,distinctNames:externalTools.distinctNames,complete:false,meaning:externalTools.scope};
const menuObject=JSON.parse(fs.readFileSync(path.join(out,'menu-object-review.json'),'utf8'));
data.functionManagerRegistrations={manifest:'menu-object-review.json',count:menuObject.functionRegistrationSites.length,sites:menuObject.functionRegistrationSites.map(s=>({key:s.key,name:s.name,target:s.target,view:s.view,edge:s.edge})),complete:false,meaning:menuObject.registrationSemantics};
data.securityFamilies=[
 {name:'配置宿主的SDK/用户数值与模块拒绝门',edge:'H03',view:'configuration-hosts'},
 {name:'主菜单另存的临时密码留存与取消/确认分支',edge:'B04',view:'execution-storage'},
 {name:'共享设置确认的返回值丢弃与权限页非短路应用',edge:'S04',view:'settings-lifecycle'},
 {name:'重启拒绝、配置保存及权限应用的分支边界',edge:'S06',view:'settings-lifecycle'},
 {name:'外部模块参数工具的PRO授权与窗口存在门',edge:'D07',view:'external-tools'},
 {name:'工具参数密码控件、失焦提交与日志遮罩',edge:'T01',view:'password-parameters'},
 {name:'流程导入SDK空密码与文件密码缓存分流',edge:'P04',view:'procedure-transfer'},
 {name:'算法代码导出的ProLevel授权门',edge:'X01',view:'procedure-transfer'},
 {name:'方案打开的格式/密码门与true返回边界',edge:'O03',view:'solution-lifecycle'},
 {name:'密码窗未通过后的自动加载配置调用位置',edge:'O05',view:'solution-lifecycle'},
 {name:'SaveImage随包密码参数声明（CustomVisible=False）',edge:'T17',view:'password-parameters'},
 {name:'方案密码、另存加密选项',edge:'E13'},
 {name:'密码确认、错误及取消',edge:'E05'},
 {name:'Group 进入保护',edge:'E06'},
 {name:'全局脚本窗口保护',edge:'E07'},
 {name:'撤销/重做的密码入口',edge:'E14'},
 {name:'加密 XML 与 ZIP 资源加载',edge:'E09'},
 {name:'软件授权/试用与加密狗',edge:'E10'},
 {name:'启动软加密模式与权限加载',edge:'E15'},
 {name:'授权剩余时间缓存与到期提醒',edge:'E17'},
 {name:'启动授权错误及重初始化',edge:'E18'},
 {name:'独立激活工具与语言配置',edge:'E19'},
 {name:'授权设置菜单与软件设置页',edge:'E20'},
 {name:'关闭密码的设置、启停和配置写入',edge:'E21'},
 {name:'自动加载方案的密码载荷',edge:'E23'},
 {name:'自动加载确认事件与保存失败分支',edge:'E25'},
 {name:'自动加载SDK设置、回读与启动优先条件',edge:'E31'},
 {name:'原生密码算法与完整授权链缺口',edge:'E12'}
].map(f=>({...f,view:f.view??'entry-routing-security',complete:false}));
data.securityFamilies.push({name:'角色切换、登录密码与权限回调',edge:'A01',view:'role-access',complete:false},{name:'权限启停、密码/模块设置与配置导入导出',edge:'A17',view:'role-access',complete:false});
data.securityFamilies.push({name:'模块配置权限列表、确认、SDK与逐入口限制',edge:'M01',view:'module-permissions',complete:false});
data.securityFamilies.push({name:'管理员改密、权限重置与结构序列化',edge:'P01',view:'password-lifecycle',complete:false});
data.securityFamilies.push({name:'关闭软件的独立密码验证',edge:'L13',view:'shell-lifecycle',complete:false});
data.securityFamilies.push({name:'资源XML文件加载、标识解密与失败状态',edge:'E36',view:'entry-routing-security',complete:false},{name:'资源字符串/字节输入重载的不同保护路径',edge:'E37',view:'entry-routing-security',complete:false},{name:'资源保存、临时文件与重加密边界',edge:'E39',view:'entry-routing-security',complete:false});
data.securityFamilies.push({name:'ZIP资源的独立密码门、缺密码拒绝与读取校验',edge:'E42',view:'entry-routing-security',complete:false});
data.securityFamilies.push({name:'方案密码窗口的按钮、键盘及粘贴注册',edge:'E46',view:'entry-routing-security',complete:false},{name:'方案密码明文状态、输入同步及回车条件',edge:'E49',view:'entry-routing-security',complete:false});
data.securityFamilies.push({name:'外部工具栏脚本的独立密码门',edge:'G11',view:'global-toolbar',complete:false},{name:'外部工具栏另存的已有密码验证',edge:'G15',view:'global-toolbar',complete:false},{name:'外部工具栏新保存密码与加密按钮',edge:'G17',view:'global-toolbar',complete:false});
data.securityFamilies.push({name:'脚本与Group共用的字符串密码校验窗',edge:'E54',view:'entry-routing-security',complete:false},{name:'密码窗取消挂接与BAML/运行缺口',edge:'E59',view:'entry-routing-security',complete:false},{name:'Group保护启用、修改与取消命令',edge:'E63',view:'entry-routing-security',complete:false},{name:'Group密码字段、启用位与UserData存储边界',edge:'E65',view:'entry-routing-security',complete:false});
data.securityFamilies.push({name:'Group文件导出密码与ModuleFilePassword缓存',edge:'P06',view:'group-file-storage',complete:false},{name:'Group文件导入及升级的密码参数',edge:'P03',view:'group-file-storage',complete:false},{name:'Group SaveAs同步入口与独立密码合同',edge:'P07',view:'group-file-storage',complete:false});
data.securityFamilies.push({name:'资源AES加解密的流、密钥与提供器边界',edge:'E35',view:'entry-routing-security',complete:false},{name:'AES静态初始化、固定IV数据与加解密赋值链',edge:'E67',view:'entry-routing-security',complete:false});
data.securityFamilies.push({name:'Group装饰器独立参数配置命令与PasswordA校验门',edge:'P10',view:'plugin-sessions',complete:false});
data.securityFamilies.push({name:'统一配置权限、ConfigCheck默认继承及代理目标检查',edge:'P12',view:'plugin-sessions',complete:false});
data.securityFamilies.push({name:'三种密码窗原BAML、共享输入约束与取消声明边界',edge:'E68',view:'entry-routing-security',complete:false});
data.securityFamilies.push({name:'通信.gtd文件导入的加密狗解密门',edge:'P03',view:'communication-protection',complete:false},{name:'通信.gtd文件导出的加密狗加密门',edge:'P05',view:'communication-protection',complete:false},{name:'通信文件保护检测、密钥与SDK返回码边界',edge:'P14',view:'communication-protection',complete:false},{name:'切换方案触发器的密码清空与即时修改',edge:'EA09',view:'communication-dispatch',complete:false},{name:'随包远程口令验证（主界面创建者未证）',edge:'R04',view:'remote-server-lifecycle',complete:false},{name:'随包远程配置RSA包装（主界面创建者未证）',edge:'R10',view:'remote-server-lifecycle',complete:false});
data.reverseEngineeringCore={path:'main-ui-map.html',architectureEntry:'main-ui-map.html#main',toolPluginEntry:'main-ui-map.html#plugin-ui',role:'shared-reverse-engineering-core',complete:false};
data.mainUiEntryModel={path:'main-ui-map.html',data:'main-ui-map.json',regionCount:8,completenessProved:false,meaning:'User-defined regions with open operation-family gates, not an entry completeness denominator.'};
data.deferredEntries=[{id:'runtime-mode',title:'运行模式',state:'deferred-by-user',authority:'用户2026-10-21：运行模式本次不深入追，保留待分析入口',internalAnalysisRequiredThisBatch:false,knownDeclarations:JSON.parse(fs.readFileSync(path.join(out,'main-ui-regions-source-map.json'))).deferredEntries,complete:false}];
for(const family of data.securityFamilies){const model=views.find(v=>v.slug===family.view)?.data;if(!model?.edges.some(e=>e.id===family.edge)||family.complete!==false)throw Error('Invalid security family navigation: '+family.name);}
fs.writeFileSync(path.join(out,'entry-inventory.json'),JSON.stringify(data,null,2)+'\n');
const payload=JSON.stringify({...data,files:undefined}).replace(/</g,'\\u003c');
const html=`<!doctype html><html lang="zh-CN"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>入口完整性 · 全量登记与断链</title><style>:root{color-scheme:dark;background:#0a1322;color:#e5eef9;font:15px/1.6 'Microsoft YaHei UI',sans-serif}body{max-width:1500px;margin:auto;padding:28px}a{color:#92cdff}p,small{color:#abc2d7}.notice{border-left:4px solid #ed94b0;background:#322537;padding:15px 20px}.stats,.gates{display:grid;grid-template-columns:repeat(4,1fr);gap:12px;margin:20px 0}.card,.gate{border:1px solid #36516b;border-radius:12px;padding:15px;background:#14263b}.card b{display:block;font-size:26px}.gate h3{margin:0;font-size:15px}.gate p{font-size:12px}.toolbar{display:flex;gap:10px;flex-wrap:wrap;margin:20px 0}button,input,select{font:inherit;background:#192f47;color:inherit;border:1px solid #44637e;padding:9px 12px;border-radius:8px}button{cursor:pointer}input{min-width:300px}details{margin:12px 0;border-left:1px solid #42627c;padding-left:18px}summary{cursor:pointer;font-weight:600}.entry{padding:10px 13px;margin:9px 0;border:1px solid #375572;background:#14263a;border-radius:9px}.entry strong{color:#e5edf7}.entry small{display:block}.entry a{margin-right:12px}pre{white-space:pre-wrap;overflow-wrap:anywhere;font:12px/1.5 Consolas,monospace}.tablewrap{overflow:auto}table{border-collapse:collapse;width:100%;font-size:12px}td,th{padding:10px;text-align:left;border-bottom:1px solid #2f465e;vertical-align:top}td{overflow-wrap:anywhere}td:first-child{width:17%}td:last-child{width:45%}.foot{font-size:12px;margin-top:28px}[hidden]{display:none!important}@media(max-width:900px){.stats,.gates{grid-template-columns:1fr 1fr}}@media(max-width:550px){body{padding:15px}.gates{grid-template-columns:1fr}input{min-width:0;width:100%}}</style><a href="index.html">← 23 领域证据导航</a> · <a href="execution-storage.html">运行与保存层级</a><h1>入口完整性：先保证不漏，再追完整链</h1><section class="notice"><a href="main-ui-map.html">主架构／工具插件逆向核心：主界面向下、工具UI向上 ↗</a><p>18个显式核查维度及逐入口记录另列；已登记区域、局部边与扫描候选均不证明入口完整。<a href="main-ui-map.html#runtime-mode">运行模式本次仅保留待分析入口</a>。</p></section><p>主界面是总索引；局部窗口、后台回调和外部触发也必须登记。隐藏项、未接入项、动态项均保留，不能只统计当前能点击的按钮。</p><div class="notice"><strong>完整性尚未证明</strong>。下方菜单声明已核原资源；扫描命中仅是待审候选。任何候选数、测试数或图中节点数都不代表完整入口覆盖。</div><div class="stats"><div class="card"><b>${menu.length}</b>原菜单具名声明</div><div class="card"><b>${data.counts.toolbarDeclarations}</b>其中工具栏声明</div><div class="card"><b>${files.length}</b>本地扫描源文件</div><div class="card"><b>${candidates.length}</b>待语义核对候选</div></div><h2>入口完整性的八道核查门</h2><div class="gates">${gates.map(g=>`<article class="gate"><h3>${g.id} · ${g.title}</h3><p>${g.gap}</p></article>`).join('')}</div><h2>原菜单与工具栏层级</h2><p>资源声明 53 项完整保留；动态菜单 resolver、隐藏状态和条件字段可展开检查。证据链接只代表已有片段，所有入口全链仍未关闭。</p><section id="menuTree"></section><h2>局部交互、生命周期及后台注册候选</h2><div class="toolbar"><input id="search" placeholder="搜索文件、事件或命令" aria-label="搜索入口候选"><select id="kind" aria-label="候选类别"><option value="">全部类别</option>${[...new Set(candidates.map(c=>c.kind))].sort().map(k=>`<option>${k}</option>`).join('')}</select><button id="more" type="button">继续显示 200 条</button></div><p id="status"></p><div class="tablewrap"><table><thead><tr><th>类型</th><th>源文件 / 行</th><th>声明或注册片段（候选）</th></tr></thead><tbody id="rows"></tbody></table></div><p class="foot">未审候选全部保留在 <a href="entry-inventory.json">机器账本</a>。本页未运行原版、SDK 或设备；未推送。实施总进度 41/96＝42.71%，入口清点与架构建图不加分。浏览器实际渲染尚未验收。</p><script>const inventory=${payload};const el=(tag,text)=>{const n=document.createElement(tag);if(text!==undefined)n.textContent=text;return n};
function entryCard(entry){const card=el('div');card.className='entry';card.dataset.entryId=entry.id;card.append(el('strong',entry.caption+(entry.hidden?' · 隐藏声明':'')+(entry.toolbar?' · 工具栏':'')),el('small',entry.name),el('small','状态：'+(entry.traceStatus==='partial-linked'?'已有片段；完整链未关闭':'待追 handler/完整链')),el('small',entry.source.path+':'+entry.source.line),el('pre',JSON.stringify(entry.fields,null,2)));for(const link of entry.edgeLinks){const a=el('a',link.id+' · 已有调用边');a.href=link.view+'.html#edge-'+link.id;card.append(a)}if(entry.sourceResolvers)card.append(el('pre',JSON.stringify(entry.sourceResolvers,null,2)));return card}function menuNode(node,level=0){const wrapper=el('details');wrapper.open=level<2;wrapper.append(el('summary',node.title));for(const id of node.entryIds)wrapper.append(entryCard(inventory.menu.find(e=>e.id===id)));for(const child of node.children)wrapper.append(menuNode(child,level+1));return wrapper}document.getElementById('menuTree').append(menuNode(inventory.menuHierarchy));let limit=200;
function render(){const q=document.getElementById('search').value.trim().toLowerCase(),kind=document.getElementById('kind').value;const matches=inventory.candidates.filter(c=>(!kind||c.kind===kind)&&(!q||(c.path+' '+c.target+' '+c.value).toLowerCase().includes(q)));const host=document.getElementById('rows');host.replaceChildren();for(const c of matches.slice(0,limit)){const tr=el('tr');const sourceCell=el('td',c.source);if(c.registrationCodeEvidence?.length){const a=el('a','查看原件挂接代码（运行绑定未确认）');a.href='password-ui-review.html';sourceCell.append(el('br'),a)}tr.append(el('td',c.kind+' · 未审'),el('td',c.path+':'+c.line),sourceCell);host.append(tr)}document.getElementById('status').textContent='匹配 '+matches.length+' 条，当前显示 '+Math.min(limit,matches.length)+' 条；完整数据保留在 JSON，未因分页删减。';document.getElementById('more').disabled=limit>=matches.length}for(const id of ['search','kind'])document.getElementById(id).addEventListener('input',()=>{limit=200;render()});document.getElementById('more').onclick=()=>{limit+=200;render()};render();</script></html>`;
const securityPanel='<section class="notice"><h2>加密、密码与授权：单独登记入口</h2><p>以下入口族已有源码切片，完整性仍未证明；不能只登记主菜单按钮。</p>'+data.securityFamilies.map(f=>'<p><a href="'+f.view+'.html#edge-'+f.edge+'">'+f.name+' ↗</a></p>').join('')+'<h2>菜单模式分支：不是只有默认 53 项</h2><p>'+data.menuResourceVariants.variants.map(v=>v.name+'：'+v.namedDeclarations+' 项').join('；')+'</p><a href="entry-routing-security.html">查看四份资源全部声明与选择条件 ↗</a></section>';
const memberPanel='<section class="notice"><h2>逐声明定位原始成员</h2><p>四份资源共 '+data.resolverMemberLocation.declarations+' 个声明、'+data.resolverMemberLocation.resolverReferences+' 个 resolver 引用，定位到 '+data.resolverMemberLocation.referencedBodies+' 个去重成员体；其中 '+data.resolverMemberLocation.bodiesWithDecompilerILWarnings+' 个含 IL 警告。静态定位通过，完整行为链仍未关闭。</p><a href="menu-member-map.html">逐项查看类型、resolver、源码体、条件和未闭边 ↗</a></section>';
const resourcePanel='<section class="notice"><h2>帮助、样例、最近方案与版本迁移</h2><p>本地帮助、在线文档、更多支持与社区分别读取不同资源；欢迎页暂存选择与直接最近菜单分列，旧版本加载另接兼容工具侧链。</p><a href="shell-resources.html">查看文件、窗口、进程、配置和当前独立实现 ↗</a></section>';
const windowPanel='<section class="notice"><h2>标题栏、显示器边界与工作区锁定</h2><p>窗口按钮从样式到系统命令逐边登记；锁定从工具栏子项、提示偏好到历史服务、菜单及视图消费者分别追踪，包含加载后的阈值分支。</p><a href="window-layout.html">查看窗口消息、坐标公式和锁定层级 ↗</a></section>';
const inputPanel='<section class="notice"><h2>输入上下文逐链恢复</h2><p>节点、页面、框选、搜索 popup 与全局通信窗口分别登记；Ctrl+F 搜索与禁用是不同入口。全局通信日志写固定 ID 11000，节点日志写当前模块 ID。完整输入注册集合和事件竞争仍开放。</p><a href="keyboard-context.html">查看快捷键、局部上下文与搜索入口 ↗</a></section>';
const reconciliationPanel='<section class="notice"><h2>入口 ↔ 证据边双向对账</h2><p>四份菜单声明、全部候选与已登记局部图交叉导航；范围重叠只是待审线索，未匹配项保留。</p><a href="entry-reconciliation.html">查看双向线索及无匹配项 ↗</a><p><a href="xaml-entry-inventory.html">XAML结构声明 ↗</a> · <a href="xaml-handler-map.html">事件声明 → 同类型处理器成员 ↗</a> · 文本定位不等于原BAML/运行绑定已确认。</p></section>';
const externalPanel='<section class="notice"><h2>从入口沿分析链阅读</h2><p><a href="menu-materialization-chain.html">主菜单资源选择、加密读取、类型物化与语言刷新总链 ↗</a></p><a href="entry-chain-focus.html">主窗口 → 菜单/工具栏 → 参数页焦点：逐箭头证据图 ↗</a><p><a href="solution-open-chain.html">打开方案分支、取消、任务与配置回流 ↗</a> · <a href="group-protection-chain.html">Group界面保护与文件密码：跨页双路径、条件与断点 ↗</a></p><p>资源包含、事件声明、方法调用和条件分支分别标注；链图不自动关闭入口完整性。</p><h2>非菜单函数源与MQ传输</h2><p>tool函数源有'+toolExports.count+'个函数/对象声明；原件确认注册类型为Frontend.Base.ServerManager，不能以同名类或常量推断网络分派。</p><a href="external-entries.html">查看函数源、线程、传输与未知分派 ↗</a></section>';
const scopePanel='<section class="notice"><h2>四份主菜单之外的独立工具栏入口</h2><p>GlobalToolItems.xml另有11个具名声明及5个公开包装入口；接收者与分派独立，宿主实际可达性未证。</p><a href="global-toolbar.html">查看独立资源、函数字符串、可见性和公开入口 ↗</a><h2>编译窗口入口载体（不混入本地源码候选数）</h2><p>PasswordCheckWindow有7处原件挂接；取消handler未在所选Connect挂接。三密码窗及共享字典原BAML已离线解码，声明关联与运行绑定分开。</p><a href="password-check-review.html">查看15方法、比较逻辑和取消断点 ↗</a><p><a href="password-edit-review.html">另有设置/修改32处挂接与Group状态链 ↗</a></p><p><a href="password-baml-review.html">查看原BAML的39声明、34唯一关联与5重复候选 ↗</a></p><h2>扫描范围与白名单外声明</h2><p>补录 '+data.counts.remainderSupplementalCandidates+' 个属性值匹配本类方法的候选，委托属性与事件不混同；未匹配属性仍保留数量和名称。</p><a href="entry-scope-audit.html">查看文件覆盖、排除边界与全部补录声明 ↗</a></section>';
const auditPanel='<section class="notice"><h2>本轮新增入口遗漏审计</h2><p><a href="resource-scope-reconciliation.html">原资源范围对账：完整载体清单与部分解码分开 ↗</a></p><p><a href="menu-entry-materialization-audit.html">菜单与工具栏：原资源、命令接收者与焦点事件 ↗</a> · <a href="workflow-rendering-entry-audit.html">画布节点／标题／连线 ↗</a> · <a href="image-export-entry-audit.html">原图与叠加图导出 ↗</a></p><p><a href="security-entry-audit.html">通信.gtd加密、触发密码与远程口令候选 ↗</a></p><p><a href="external-entry-audit.html">通信换方案、即时写入与后台接收 ↗</a></p><p><a href="domain-gap-audit.html">23领域／32任务的下一证据队列 ↗</a></p><p>专项调查链单独计数；正式安全族已纳入通信文件保护、触发密码及随包远程口令；仍不能作为加密入口全集。</p></section>';
const finalHtml=html.replace('<h2>入口完整性的八道核查门</h2>',auditPanel+scopePanel+memberPanel+securityPanel+resourcePanel+windowPanel+inputPanel+reconciliationPanel+externalPanel+'<h2>入口完整性的八道核查门</h2>').replace('原菜单具名声明','默认菜单具名声明').replace('原菜单与工具栏层级','默认资源的菜单与工具栏层级');
new vm.Script(finalHtml.match(/<script>([\s\S]*?)<\/script>/)[1]);fs.writeFileSync(path.join(out,'entry-inventory.html'),finalHtml);
console.log(JSON.stringify({...data.counts,complete:data.complete,sourceTrees:new Set(manifest.map(f=>f.path.split('/')[2])).size}));
