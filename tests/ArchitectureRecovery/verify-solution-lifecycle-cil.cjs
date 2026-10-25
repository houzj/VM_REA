// Validate a finite original-CIL slice and a normal-return control-flow witness.
// Does not load the vendor assembly, interpret arbitrary IL, or assert runtime effects.
const fs=require('fs'),path=require('path'),crypto=require('crypto'),{out}=require('./evidence-view.cjs');
const sha=b=>crypto.createHash('sha256').update(b).digest('hex');
function verify(){
 const read=f=>JSON.parse(fs.readFileSync(path.join(out,f))),d=read('solution-lifecycle-cil-evidence.json'),identity=read('solution-lifecycle-rea-identity.json'),a=d.manifest.artifacts[0];
 if(identity.artifact.sha256!==a.sha256||identity.module.mvid!==a.mvid||sha(fs.readFileSync(a.path))!==a.sha256)throw Error('Solution lifecycle original identity mismatch');
 let count=0;for(const t of d.targets)for(const m of t.methods){const bytes=Buffer.from(m.ilBytes??'','hex'),offsets=new Set(m.instructions.map(i=>i.offset));if(m.status!=='decoded'||bytes.length!==m.ilSize||sha(bytes)!==m.ilSha256)throw Error('Solution lifecycle incomplete CIL');for(const i of m.instructions)if(/BrTarget$/.test(i.operandKind)&&!offsets.has(i.operand)||i.operandKind==='InlineSwitch'&&i.operand.some(o=>!offsets.has(o)))throw Error('Solution lifecycle branch target invalid');count++;}
 if(count!==12)throw Error('Solution lifecycle finite scope changed');
 const m=d.targets.find(t=>t.type==='VisionMaster.Project.ProjectManager').methods.find(m=>m.name==='LoadSolution'),by=new Map(m.instructions.map(i=>[i.offset,i])),index=new Map(m.instructions.map((i,n)=>[i.offset,n]));
 function at(offset,opcode,operand){const i=by.get(offset);if(i?.opcode!==opcode||operand!==undefined&&i.operand!==operand)throw Error('Solution witness instruction changed: '+offset);return i;}
 function call(offset,name,type){const i=by.get(offset);if(!/^(call|callvirt|newobj)$/.test(i?.opcode??'')||i.operand?.name!==name||type&&i.operand?.type!==type)throw Error('Solution witness call changed: '+offset);return i;}
 call(100,'.ctor','Microsoft.Win32.OpenFileDialog');call(265,'.ctor','DirectoryBrowser.OpenFileDialog');call(303,'ShowDialog','VM.Controls.BaseWindow');call(581,'ShowDialog','VM.Controls.BaseWindow');
 // Nullable<bool> != true: value/default comparison AND HasValue, then negate.
 const expected=[[586,'stloc.s',24],[588,'ldloc.s',24],[590,'stloc.s',7],[592,'ldc.i4.1'],[593,'stloc.s',8],[595,'ldloca.s',7],[602,'ldloc.s',8],[604,'ceq'],[606,'ldloca.s',7],[613,'and'],[614,'ldc.i4.0'],[615,'ceq'],[617,'stloc.s',25],[619,'ldloc.s',25],[621,'brfalse.s',628],[624,'ldc.i4.1'],[625,'stloc.s',16],[715,'ldloc.s',16],[717,'ldc.i4.0'],[718,'ceq'],[720,'stloc.s',28],[722,'ldloc.s',28],[724,'brfalse',1001]];
 for(const row of expected)at(...row);call(597,'GetValueOrDefault');call(608,'get_HasValue');
 const forced=new Map([[621,623],[724,1001]]),witness=[];let pc=586;
 while(true){const i=by.get(pc);if(!i||witness.includes(pc)||witness.length>150)throw Error('Invalid solution witness');witness.push(pc);if(i.opcode==='ret')break;const next=m.instructions[index.get(pc)+1]?.offset;
  if(forced.has(pc)){const target=forced.get(pc);if(!/BrTarget$/.test(i.operandKind)||target!==i.operand&&target!==next)throw Error('Invalid selected branch');pc=target;}
  else if(/^br(?:\.s)?$/.test(i.opcode))pc=i.operand;
  else if(/BrTarget$/.test(i.operandKind)||i.operandKind==='InlineSwitch'||/^(throw|rethrow|leave|leave\.s|endfinally)$/.test(i.opcode))throw Error('Unreviewed witness control flow');
  else pc=next;
 }
 const target=call(1081,'UpdateAutoLoadLaseSol','VisionMaster.Project.ProjectManager');
 if(!witness.includes(target.offset)||!witness.includes(625)||!witness.includes(724)||witness.includes(751)||witness.includes(994))throw Error('Witness did not bypass loading Task');
 if(m.instructions.some(i=>witness.includes(i.offset)&&i.opcode==='stloc.s'&&i.operand===16&&i.offset!==625))throw Error('Password flag rewritten on witness');
 call(751,'.ctor','System.Threading.Tasks.Task');call(994,'Start','System.Threading.Tasks.Task');at(1088,'ldc.i4.1');at(1089,'stloc.s',9);at(1093,'ldloc.s',9);at(1095,'ret');
 const report={schemaVersion:1,complete:false,artifact:a,reaEvidence:identity.evidence_id,decodedBodies:count,loadMethod:{token:m.token,ilSha256:m.ilSha256,ilSize:m.ilSize},passwordNonTrueWitness:{condition:'Selected valid-format password-protected path, no explicit password, modal result false or null; each encountered call returns normally.',nullablePredicate:'!((result.GetValueOrDefault() == true) & result.HasValue)',selectedBranches:[{offset:621,decision:'predicate true: brfalse not taken',next:623},{offset:724,decision:'local16=true -> (local16==0) false: brfalse taken',next:1001}],instructions:witness.map(o=>by.get(o)),bypassedTaskConstructor:751,bypassedTaskStart:994,configurationCall:1081,normalReturnValue:true},exceptions:m.exceptions,limitations:['Normal-return CFG witness, not original app execution or arbitrary-IL interpretation.','Calling configuration helper does not prove its condition, successful write, password acceptance, decryption or load completion.','Twelve selected method bodies exclude Task/Dispatcher delegate bodies, SDK/native and complete BAML/runtime identity.']};
 report.dialogConstructors=[by.get(100),by.get(265)];
 fs.writeFileSync(path.join(out,'solution-lifecycle-cil-review.json'),JSON.stringify(report,null,2)+'\n');return report;
}
module.exports={verify};if(require.main===module)console.log(JSON.stringify({decodedBodies:verify().decodedBodies,normalReturnWitness:true,runtimeExecuted:false}));
