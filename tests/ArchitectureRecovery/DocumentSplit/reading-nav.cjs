'use strict';
// Homepage-only reader tasks. Never inject into shared page templates or site.js.
const items=[
 {path:'专题/entry-inventory.html',label:'原版证据',note:'入口与断链',title:'已登记原始声明与成员索引；入口完整性尚未证明',reason:'先查原资源声明、成员定位和断链，避免把扫描候选当已核事实。'},
 {path:'主题/全景架构.html#originalChain',label:'恢复模型',note:'分层对照',title:'捕获版本的原版／当前模型与未闭边；不是全域等价',reason:'直达已有五层主链，并与下方当前重建链对照，避免再次落入页面前部的界面地图。'},
 {path:'主题/范围与证据.html',label:'局部缺口',note:'范围与待闭边',title:'方法、退出门槛与当前最大待闭边；未知不等于不支持',reason:'找到当前最大待闭边和证据退出标准，不引导继续无限扩写。'}
];
const esc=s=>String(s).replaceAll('&','&amp;').replaceAll('<','&lt;').replaceAll('"','&quot;');
function render(prefix=''){return '<nav class="reading-nav" data-reading-nav aria-label="逆向阅读主题"><span>阅读主题</span>'+items.map(i=>'<a href="'+esc(prefix+i.path)+'" title="'+esc(i.title)+'">'+esc(i.label)+'<small>'+esc(i.note)+'</small></a>').join('')+'</nav>';}
module.exports={items,render};
