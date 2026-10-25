'use strict';
// Refresh this uniquely owned chapter without rebasing other in-flight evidence panels.
const fs=require('fs'),path=require('path'),assert=require('assert/strict');const {root}=require('./evidence-view.cjs');
const p=path.join(root,'docs/framework-architecture.html'),html=fs.readFileSync(p,'utf8'),marker='<div id="global-variable-execution">',start=html.indexOf(marker);assert(start>=0);assert.equal(html.indexOf(marker,start+1),-1);
const tags=/<\/?div\b[^>]*>/g;tags.lastIndex=start;let depth=0,end=-1,tag;while((tag=tags.exec(html))){depth+=tag[0].startsWith('</')?-1:1;if(depth===0){end=tags.lastIndex;break;}}assert(end>start);
const result=require('./global-variable-execution.cjs')();fs.writeFileSync(p,html.slice(0,start)+result.html+html.slice(end));
console.log(JSON.stringify({chapter:'global-variable-execution',facts:result.model.facts.length,sourceReferences:result.model.facts.reduce((n,f)=>n+f.refs.length,0),otherPageBytesPreserved:true,browserRendered:false}));
