// One-hop literal references only. Edges between two neighbours are deliberately excluded.
module.exports=function(projects,selected){
 if(!projects.projects.some(p=>p.path===selected))return null;
 const indices=projects.edges.flatMap((e,i)=>e.from===selected||e.to===selected?[i]:[]),members=new Set([selected]);
 for(const i of indices){members.add(projects.edges[i].from);members.add(projects.edges[i].to);}
 const subset={...projects,projects:projects.projects.filter(p=>members.has(p.path)),edges:indices.map(i=>projects.edges[i]),projectCount:members.size};
 const graph=require('./execution-dependency-layout.cjs')(subset,true);graph.mode='neighbors';graph.selectedProject=selected;
 for(const e of graph.edges)e.referenceIndices=e.referenceIndices.map(i=>indices[i]);for(const e of graph.internalReferences)e.referenceIndex=indices[e.referenceIndex];
 graph.meaning='One-hop incoming and outgoing literal ProjectReference subset for the selected project; not transitive dependencies or runtime calls.';
 return graph;
};
