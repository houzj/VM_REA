'use strict';
// Additive, independently recovered original native bodies. Earlier snapshots stay verbatim.
module.exports=function(){
 const sources=[['composite','original-variable-composite-evidence.cjs','original-variable-composite.json'],['payload','original-variable-payload-evidence.cjs','original-variable-payload.json'],['dispatch','original-variable-dispatch-evidence.cjs','original-variable-dispatch.json']].map(([id,provider,report])=>({id,provider,report,...require('./'+provider)()}));
 const facts=sources.flatMap(s=>s.facts);if(new Set(facts.map(f=>f.id)).size!==facts.length)throw Error('Duplicate extended variable fact');return{facts,sources};
};
