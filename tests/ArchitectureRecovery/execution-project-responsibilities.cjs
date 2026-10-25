'use strict';
// Source-grounded current-project responsibilities; presentation labels are not runtime layers.
const fs=require('fs'),path=require('path'),{root,source}=require('./evidence-view.cjs');
module.exports=function(){
 const span=(p,member,anchor,count=32)=>{const lines=fs.readFileSync(path.join(root,p),'utf8').split(/\r?\n/),hits=lines.map((l,i)=>l.includes(anchor)?i:-1).filter(i=>i>=0);if(hits.length!==1)throw Error('Responsibility anchor not unique '+p+' '+anchor);return {...source(p,member,anchor,count),evidenceType:'current-main-source-responsibility',meaning:'Selected declarations and implementation body, not a complete runtime or ownership contract.'};};
 const rows=[
 ['VisionCore','阈值与连通域算法',[span('src/VisionCore/Algorithms.cs','Algorithms.Threshold / ConnectedComponents8','public static GrayImage Threshold',55)]],
 ['VisionData','图像像素与几何类型',[span('src/VisionData/ImageGeometry.cs','GrayImage storage / constructor','public sealed class GrayImage',38)]],
 ['VisionIO','BMP文件读取',[span('src/VisionIO/BmpReader.cs','BmpReader.Read','public static GrayImage Read',65)]],
 ['VisionUi','ROI几何与WPF呈现',[span('src/VisionUi/RoiWpfGeometry.cs','RoiWpfGeometry.Create','public static Geometry Create',16)]],
 ['VisionModules','内置模块注册',[span('src/VisionModules/BuiltinModules.cs','BuiltinModules.RegisterAll','public static void RegisterAll',32)]],
 ['VisionPluginContracts','插件合同与编辑上下文',[span('src/VisionPluginContracts/Contracts.cs','ToolEditorContext / IVisionToolPlugin','public sealed record ToolEditorContext',15)]],
 ['VisionEditor.Core','文档编辑、历史与保存',[span('src/VisionEditor.Core/GraphEditorDocument.cs','GraphEditorDocument state/history','public sealed partial class GraphEditorDocument',35),span('src/VisionEditor.Core/GraphEditorDocument.cs','GraphEditorDocument.Save','public void Save(string path)',8)]],
 ['VisionFramework','图验证与模块调度',[span('src/VisionFramework/GraphExecutor.cs','GraphExecutor.RunAsync','public async Task<ExecutionSessionResult> RunAsync',42)]],
 ];
 const execution=require('./execution-architecture-evidence.cjs')(),window=execution.chains.flatMap(c=>c.current).find(e=>e.id==='EST02');rows.push(['VisionEditor','主窗与用户入口',window.refs]);
 return rows.map(([id,role,refs])=>({path:'src/'+id+'/'+id+'.csproj',role,classification:'independent-implementation',refs,limits:['这是已读取当前成员的职责摘要，不宣称该工程的全部职责已恢复。','源码职责分组与布局不代表原版架构或实际运行调用顺序。']}));
};
