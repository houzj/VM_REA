function fullProjectLayout(nodes, mode) {
    const pos = new Map(), bands = [];
    if (mode === 'overview') {
        const fixed = {
            'SunEyeVision.UI': [530, 65],
            'SunEyeVision.Workflow': [310, 225],
            'SunEyeVision.Plugin.Infrastructure': [850, 225],
            '__tools__': [530, 385],
            'SunEyeVision.Plugin.SDK': [130, 545],
            'CommonControl': [650, 545],
            'SunEyeVision.Core': [1150, 545],
            'SunEyeVision.DeviceDriver': [110, 705],
            'SunEyeVision.HalAdapter': [465, 705],
            'SunEyeVision.CvAdapter': [820, 705],
            'SunEyeVision.CvEngine': [1175, 705]
        };
        nodes.forEach(n => pos.set(n.id, fixed[n.id] || [1290, 65]));
        bands.push(
            { x: 20, y: 15, w: 1200, h: 130, label: '应用宿主 · 1工程' },
            { x: 20, y: 175, w: 1570, h: 130, label: '流程执行与扩展装配 · 2工程' },
            { x: 20, y: 335, w: 1570, h: 130, label: '工具插件层 · 23工程 / 24项能力，默认合并；具体工具不是独立架构层' },
            { x: 20, y: 495, w: 1570, h: 130, label: '公共合同、控件与基础 · 3工程；SDK运行合同与UI扩展尚需分界' },
            { x: 20, y: 655, w: 1570, h: 130, label: '设备与算法后端 · 4工程' },
            { x: 1260, y: 15, w: 330, h: 130, label: '辅助工程 · 不计产品架构层' }
        );
        return { pos, bands, height: 825 };
    }
    const tools = nodes.filter(n => n.kind === 'tool');
    const core = nodes.filter(n => n.kind === 'core');
    const aux = nodes.filter(n => n.kind === 'auxiliary');
    tools.forEach((n, i) => pos.set(n.id, [30 + (i % 6) * 260, 70 + Math.floor(i / 6) * 100]));
    const coreY = tools.length ? 100 + Math.ceil(tools.length / 6) * 100 : 65;
    core.forEach((n, i) => pos.set(n.id, [30 + (i % 5) * 310, coreY + 40 + Math.floor(i / 5) * 180]));
    const auxY = coreY + Math.ceil(core.length / 5) * 180 + 65;
    aux.forEach((n, i) => pos.set(n.id, [30 + i * 310, auxY + 40]));
    if (tools.length) bands.push({ x: 15, y: 15, w: 1580, h: coreY - 30, label: `工具插件层 · ${tools.length}个同层工程展开，不是${tools.length}个架构层` });
    if (core.length) bands.push({ x: 15, y: coreY, w: 1580, h: Math.ceil(core.length / 5) * 180 + 25, label: '核心工程明细 · 这里按工程展开，不把工程行数当职责层数' });
    if (aux.length) bands.push({ x: 15, y: auxY, w: 1580, h: 125, label: '辅助工程 · 不参与产品业务层次比较' });
    return { pos, bands, height: aux.length ? auxY + 150 : coreY + Math.ceil(core.length / 5) * 180 + 70 };
}

(()=>{const d=JSON.parse(document.getElementById('full-project-graph-data').textContent),$=id=>document.getElementById(id),E=s=>String(s||'').replaceAll('&','&amp;').replaceAll('<','&lt;').replaceAll('>','&gt;').replaceAll('"','&quot;');let mode='overview',selected='',zoom=1;const groupId='__tools__',kindLabel=k=>({core:'核心工程',tool:'工具插件',auxiliary:'辅助工程','tool-group':'工具分组'}[k]||k);$('graph-project').innerHTML+=[...d.nodes].sort((a,b)=>a.id.localeCompare(b.id)).map(n=>`<option value="${E(n.id)}">${E(n.label)} · ${n.kind}</option>`).join('');
function detail(id){const n=d.nodes.find(n=>n.id===id);if(!n){$('full-project-detail').innerHTML='<h3>工具插件分组</h3><p>23个工程、24项工具能力；展开全部或从下拉框选择具体工程。此处合并显示引用边，不代表工具之间相互调用。</p>';return}const incoming=d.edges.filter(e=>e.to===id).map(e=>e.from),outgoing=d.edges.filter(e=>e.from===id).map(e=>e.to);$('full-project-detail').innerHTML=`<h3>${E(n.label)} · ${E(kindLabel(n.kind))}</h3><p>${E(n.role)}</p><p>引用它的工程：${incoming.map(E).join('、')||'没有仓库内直接ProjectReference；可能由运行时加载，不能判未使用。'}</p><p>它直接引用：${outgoing.map(E).join('、')||'无ProjectReference声明'}</p><p>目标：${E(n.target)}；${n.useWpf?'声明UseWPF=true':'未在本工程声明UseWPF=true，不等传递依赖无WPF'}</p><p>${n.tools.map(t=>`<a href="${E(t.file)}">${E(t.name)} · 实际工具链 ↗</a>`).join('；')}</p><details><summary>NuGet / 程序集声明与来源</summary><p>NuGet：${n.packages.map(E).join('、')||'本文件无声明'}</p><p>程序集：${n.assemblyReferences.map(E).join('、')||'本文件无声明'}</p><p><a href="file:///D:/MyWork/SunEyeVision_V2/${encodeURI(n.file)}">${E(n.file)}</a><small> SHA256 ${n.sha256}</small></p><p>只核声明，未执行条件求值、构建或部署验收。</p></details>`}
function render(){let nodes,edges;if(mode==='overview'){nodes=[...d.nodes.filter(n=>n.kind!=='tool'),{id:groupId,label:'工具插件（23工程/24能力）',kind:'tool-group'}];const seen=new Set;edges=d.edges.map(e=>({from:d.nodes.find(n=>n.id===e.from).kind==='tool'?groupId:e.from,to:d.nodes.find(n=>n.id===e.to).kind==='tool'?groupId:e.to})).filter(e=>{const k=e.from+'>'+e.to;if(seen.has(k))return false;seen.add(k);return true})}else{const ids=mode==='neighbors'&&selected?new Set([selected,...d.edges.filter(e=>e.from===selected||e.to===selected).flatMap(e=>[e.from,e.to])]):null;nodes=d.nodes.filter(n=>!ids||ids.has(n.id));edges=d.edges.filter(e=>nodes.some(n=>n.id===e.from)&&nodes.some(n=>n.id===e.to))}const{pos,bands,height}=fullProjectLayout(nodes,mode);const bandsHtml=bands.map(b=>`<g class="graph-layer-band"><rect x="${b.x}" y="${b.y}" width="${b.w}" height="${b.h}" rx="12" fill="#112736" stroke="#36596f" stroke-dasharray="6 4"/><text x="${b.x+14}" y="${b.y+25}" fill="#a9cee2" font-size="14">${E(b.label)}</text></g>`).join('');const activeSelection=mode==='overview'&&d.nodes.find(n=>n.id===selected)?.kind==='tool'?groupId:selected;const paths=edges.map(e=>{const a=pos.get(e.from),b=pos.get(e.to),active=!activeSelection||e.from===activeSelection||e.to===activeSelection;return`<path data-from="${E(e.from)}" data-to="${E(e.to)}" d="M${a[0]+110},${a[1]+60} C${a[0]+110},${a[1]+100} ${b[0]+110},${b[1]-40} ${b[0]+110},${b[1]}" fill="none" stroke="${active?'#86c8e8':'#35556c'}" opacity="${active?.8:.2}" marker-end="url(#full-arrow)"/>`}).join('');$('full-project-canvas').innerHTML=`<svg viewBox="0 0 1620 ${height}" role="img" aria-label="${mode==='overview'?'分组':'完整或邻居'}静态工程依赖图"><defs><marker id="full-arrow" markerWidth="8" markerHeight="8" refX="7" refY="3" orient="auto"><path d="M0,0 L0,6 L8,3z" fill="#86c8e8"/></marker></defs>${bandsHtml}${paths}${nodes.map(n=>{const [x,y]=pos.get(n.id);return`<g data-project="${E(n.id)}" role="button" tabindex="0" aria-label="${E(n.label)}"><rect x="${x}" y="${y}" width="240" height="65" rx="10" fill="${selected===n.id?'#254b61':'#173044'}" stroke="${selected===n.id?'#ffcc77':'#86c8e8'}"/><text x="${x+10}" y="${y+25}" fill="#eef6ff" font-size="13">${E(n.label)}</text><text x="${x+10}" y="${y+49}" fill="#a9cee2" font-size="11">${E(kindLabel(n.kind))}${n.useWpf?' · WPF声明':''}</text></g>`}).join('')}</svg>`;$('full-project-canvas').querySelector('svg').style.width=zoom*100+'%';$('graph-mode').textContent=`${mode==='overview'?'职责分层展示；23工具工程合为一层，边合并':mode==='neighbors'?'只显示一跳直接邻居，不是全部传递依赖':'全部正式工程'}：${nodes.length}节点 / ${edges.length}显示边；完整清单34工程 / 61条声明。`}
function select(id,write=true){selected=id;if(id===groupId){detail(id);return}if(mode==='overview'&&d.nodes.find(n=>n.id===id)?.kind==='tool')mode='all';$('graph-project').value=id;render();detail(id);if(write){const u=new URL(location.href);u.searchParams.set('project',id);history.pushState({},'',u)}}$('graph-project').onchange=()=>{if($('graph-project').value)select($('graph-project').value)};$('full-project-canvas').onclick=e=>{const g=e.target.closest('[data-project]');if(g)select(g.dataset.project)};$('full-project-canvas').onkeydown=e=>{if(e.key==='Enter'||e.key===' '){const g=e.target.closest('[data-project]');if(g){e.preventDefault();select(g.dataset.project)}}};$('graph-overview').onclick=()=>{mode='overview';render()};$('graph-all').onclick=()=>{mode='all';render()};$('graph-neighbors').onclick=()=>{mode=selected?'neighbors':'all';render()};$('graph-plus').onclick=()=>{zoom=Math.min(3,zoom+.25);render()};$('graph-minus').onclick=()=>{zoom=Math.max(.5,zoom-.25);render()};$('graph-fit').onclick=()=>{zoom=1;render()};function route(){const id=new URL(location.href).searchParams.get('project');if(d.nodes.some(n=>n.id===id))select(id,false);else{selected='';$('graph-project').value='';render();$('full-project-detail').innerHTML='<p>点击节点查看引用与职责；箭头由引用者指向被引用者。</p>'}}addEventListener('popstate',route);let beforePrint;addEventListener('beforeprint',()=>{beforePrint={mode,zoom};mode='all';zoom=1;render()});addEventListener('afterprint',()=>{if(beforePrint){({mode,zoom}=beforePrint);render()}});route()})();
