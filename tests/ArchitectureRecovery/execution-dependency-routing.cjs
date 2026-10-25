// Orthogonal presentation routing. Rectangles are obstacles, never runtime semantics.
module.exports=function(graph){
 const margin=12,unique=a=>[...new Set(a)].sort((a,b)=>a-b),nodes=graph.nodes;
 const xs=unique([8,graph.width-8,...nodes.flatMap(n=>[n.x-margin,n.x+n.width/2,n.x+n.width+margin])]),ys=unique([8,graph.height-8,...nodes.flatMap(n=>[n.y-margin,n.y+n.height+margin])]);
 const obstacles=nodes.map(n=>({x:n.x-6,y:n.y-6,r:n.x+n.width+6,b:n.y+n.height+6}));
 const inside=(x,y)=>obstacles.some(r=>x>r.x&&x<r.r&&y>r.y&&y<r.b);
 const blocked=(a,b)=>obstacles.some(r=>a.x===b.x?a.x>r.x&&a.x<r.r&&Math.max(a.y,b.y)>r.y&&Math.min(a.y,b.y)<r.b:a.y>r.y&&a.y<r.b&&Math.max(a.x,b.x)>r.x&&Math.min(a.x,b.x)<r.r);
 const grid=new Map();for(let xi=0;xi<xs.length;xi++)for(let yi=0;yi<ys.length;yi++)if(!inside(xs[xi],ys[yi]))grid.set(xi+','+yi,{x:xs[xi],y:ys[yi],xi,yi,near:[]});
 for(const p of grid.values())for(const[dx,dy]of [[1,0],[-1,0],[0,1],[0,-1]]){let xi=p.xi+dx,yi=p.yi+dy;while(xi>=0&&xi<xs.length&&yi>=0&&yi<ys.length){const q=grid.get(xi+','+yi);if(q){if(!blocked(p,q))p.near.push(q);break;}xi+=dx;yi+=dy;}}
 for(const e of graph.edges){const a=nodes.find(n=>n.id===e.from),b=nodes.find(n=>n.id===e.to),start=grid.get(xs.indexOf(a.x+a.width/2)+','+ys.indexOf(a.y+a.height+margin)),end=grid.get(xs.indexOf(b.x+b.width/2)+','+ys.indexOf(b.y-margin));if(!start||!end)throw Error('No routing endpoint '+e.id);
 const key=(p,d)=>p.xi+','+p.yi+':'+d,dist=new Map(),prev=new Map(),states=new Map(),open=[],heur=p=>Math.abs(p.x-end.x)+Math.abs(p.y-end.y);const sk=key(start,'v');dist.set(sk,0);states.set(sk,{p:start,d:'v'});open.push({k:sk,f:heur(start)});let finish;
 while(open.length){open.sort((a,b)=>b.f-a.f);const top=open.pop(),state=states.get(top.k),g=dist.get(top.k);if(top.f!==g+heur(state.p))continue;if(state.p===end){finish=top.k;break;}for(const q of state.p.near){const d=q.x===state.p.x?'v':'h',k=key(q,d),ng=g+Math.abs(q.x-state.p.x)+Math.abs(q.y-state.p.y)+(d===state.d?0:24);if(ng<(dist.get(k)??Infinity)){dist.set(k,ng);prev.set(k,top.k);states.set(k,{p:q,d});open.push({k,f:ng+heur(q)});}}}
 if(!finish)throw Error('No obstacle-free dependency route '+e.id);let pts=[];for(let k=finish;k;k=prev.get(k)){const p=states.get(k).p;pts.push({x:p.x,y:p.y});}pts.reverse();pts.unshift({x:start.x,y:a.y+a.height});pts.push({x:end.x,y:b.y});const compact=[];for(const p of pts){while(compact.length>=2){const u=compact.at(-2),v=compact.at(-1);if((u.x===v.x&&v.x===p.x)||(u.y===v.y&&v.y===p.y))compact.pop();else break;}compact.push(p);}e.points=compact;e.svgPath=compact.map((p,i)=>(i?'L ':'M ')+p.x+' '+p.y).join(' ');
 }
 return graph;
};
