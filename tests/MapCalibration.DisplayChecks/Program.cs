using System.Reflection;
using System.Text.Json;
using VisionCore;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.MapCalibration;
try{
 int checks=0;void Check(bool value,string message){if(!value)throw new Exception(message);checks++;}
 var plugin=new MapCalibrationPlugin();var values=plugin.Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());string cwd=Directory.GetCurrentDirectory();
 NodeDocument Node()=>new("display",MapCalibrationPlugin.Id,"1.0.0","display",values);
 Check(plugin.LastDisplay(Node(),cwd,null) is null,"No result has no frame");
 var state=typeof(MapCalibrationPlugin).GetMethod("CurrentImages",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(plugin,["display",cwd,null])!;
 void Set(string name,object value)=>state.GetType().GetField(name)!.SetValue(state,value);
 var image=new GrayImage(2,2,new byte[]{1,2,3,4});Set("Target",image);
 Set("Result",new MapCalibrationImageResult(0,[1,0,0,0,1,0,0,0,1],1,1,1,1,0,[new(10,20,1,2)],[new(30,40,1,2)],[new(31,41,1,2)]){Boxes=[default,default]});
 var frame=plugin.LastDisplay(Node(),cwd,null)!;Check(frame.Image==image,"Target input image");Check(frame.ImageLayers.Count==1&&frame.ImageLayers[0].Image==image,"Original InImage2 mapping gets target layer");
 foreach(var item in new[]{("Display Current Corners Point",10d,20d),("Display Object Corners Point",30d,40d),("Display Projection Corners Point",31d,41d)}){var graphic=frame.Graphics.Single(g=>g.Name==item.Item1);Check(graphic.Points.Count==1&&graphic.Points[0].X==item.Item2&&graphic.Points[0].Y==item.Item3,"Original graphic source "+item.Item1);Check(graphic.Points[0].Color=="#66ff00","Original point status color");}
 Check(!frame.Graphics.Any(g=>g.Type=="coordinate"),"No fabricated CalibOriPoint producer");
 var store=new ToolDisplayObjectStore(MapCalibrationMetadata.DisplayObjects,()=>values,next=>values=next);var original=store.Read("Display Current Corners Point")!;Check(store.Write(original.Definition.Name,original.Style with{Visible=false}).Accepted,"Persist graphic visibility");Check(!plugin.LastDisplay(Node(),cwd,null)!.Graphics.Any(g=>g.Name==original.Definition.Name),"LastDisplay consumes configured visibility");
 var input=store.Read("InImage")!;Check(store.Write("InImage",input.Style with{Visible=false}).Accepted,"Persist image visibility");Check(!plugin.LastDisplay(Node(),cwd,null)!.ImageLayers[0].Visible,"Original image object toggle");
 values["InputType"]=JsonSerializer.SerializeToElement(2);Check(plugin.LastDisplay(Node(),cwd,null) is null,"File mode frame gate preserved");
 Console.WriteLine($"PASS {checks}: source resource point/image layers and visibility; no native frame-lifecycle parity claim");return 0;
}catch(Exception error){Console.Error.WriteLine(error);return 1;}
