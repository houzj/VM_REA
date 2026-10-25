'use strict';
// Explicit, bounded incremental pipeline; never invokes the original generator.
const fs=require('fs'),path=require('path'),{spawnSync}=require('child_process'),assert=require('assert/strict');
const choice=process.argv.find(a=>a.startsWith('--topic='))?.slice(8);assert(['image','group','retirement','global','domain','nonimage','assets','state','owner'].includes(choice),'Use --topic=image|group|retirement|global|domain|nonimage|assets|state|owner');
const jobs=[];
if(['image','group','retirement','global','domain','nonimage'].includes(choice))jobs.push(['split-scenes.cjs','--topic='+choice],['verify-scenes.cjs']);
if(choice==='assets')jobs.push(['build.cjs','--assets-only'],['verify-navigation.cjs']);
if(choice==='state')jobs.push(['build-stop-state.cjs'],['verify-stop-state.cjs']);
if(choice==='owner'||choice==='image')jobs.push(['build-owner-data.cjs'],['verify-owner-data.cjs']);
jobs.push(['chunk-relations.cjs'],['verify-chunks.cjs'],['refine.cjs']);const results=[];
for(const [script,...args]of jobs){const r=spawnSync(process.execPath,[path.join(__dirname,script),...args],{encoding:'utf8',windowsHide:true});results.push({script,args,exitCode:r.status,stdout:r.stdout.trim(),stderr:r.stderr.trim()});if(r.status!==0){console.error(JSON.stringify({passed:false,topic:choice,results}));process.exit(1);}}
const report={passed:true,topic:choice,sourceEvidenceRescanned:false,originalGeneratorInvoked:false,productModified:false,browserVerified:false,results};
fs.writeFileSync(path.resolve(__dirname,'../../../docs/架构逆向与实现对照文档/清单/增量更新-'+choice+'.json'),JSON.stringify(report,null,2)+'\n');console.log(JSON.stringify(report));
