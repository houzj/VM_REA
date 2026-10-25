'use strict';
// Presentation repair for this topic only; evidence records remain byte-for-byte.
function compactOverview(html){
 const start=html.indexOf('<h2>先从主界面进入</h2>'),end=html.indexOf('<h2>再向下看五层主链</h2>');
 if(start>=0&&end>start)html=html.slice(0,start)+'<p><a href="../index.html#main">按界面位置查找入口 →</a></p>'+html.slice(end);
 html=html.replace('再向下看五层主链','五层架构主链');
 const groups={originalChain:['oShell','oTool','oModel','oParam','oSdk','oResult'],currentChain:['mShell','mDoc','mPorts','mExec','pTool','mOut'],crossChain:['xUndo','xLife','xGlobal','xNative']};
 for(const [host,keys]of Object.entries(groups)){
  const cards=keys.map(key=>{
   const record=new RegExp('<details class="record" id="'+key+'"[\\s\\S]*?</details>').exec(html)?.[0];
   if(!record)throw Error('Missing architecture record '+key);
   const title=/<summary>([\s\S]*?)<\/summary>/.exec(record)[1];
   const sub=/<dt>sub<\/dt><dd><span>([\s\S]*?)<\/span><\/dd>/.exec(record)?.[1]||'';
   return '<a class="node" href="#'+key+'"><b>'+title+'</b><small>'+sub+'</small></a>';
  }).join('');
  html=html.replace(new RegExp('(<div class="(?:chain|cross)" id="'+host+'">)[\\s\\S]*?</div>'),'$1'+cards+'</div>');
 }
 html=html.replace(/<nav aria-label="主导航">[\s\S]*?<\/nav>/,'<nav aria-label="返回首页"><a href="../index.html#main">← 返回首页</a></nav>').replace(/(<footer class="site-footer">)<nav>[\s\S]*?<\/nav>/,'$1').replace('<div class="layout">','<div class="layout" style="grid-template-columns:minmax(0,1fr)">').replace('<aside class="inspector" id="inspector"></aside>','');
 return html;
}
module.exports=compactOverview;
