'use strict';
const fs=require('fs'),path=require('path');const {root}=require('./evidence-view.cjs');
module.exports=function(){const s=JSON.parse(fs.readFileSync(path.join(root,'docs/framework-progress-audit.json'))).summary;return s.overallDone+'/'+s.units+' = '+s.overallCompletionPercent+'%; documentation credit=0';};
