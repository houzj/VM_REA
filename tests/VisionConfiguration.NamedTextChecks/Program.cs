using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.DistortionCalibration;
class Program {
 static int count;static void Check(bool value,string message){count++;if(!value)throw new Exception(message);}
 [STAThread]static int Main(){try{Run();Console.WriteLine($"PASS {count} named text feature checks; native zero-font rendering unverified.");return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
 static void Run(){
 var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};const string name="Show Evaluation Error Text";
 var values=DistortionCalibrationMetadata.Defaults();values["DisplaySettings"]=JsonSerializer.SerializeToElement("legacy-snapshot");values["DisplayObjectSettings"]=JsonSerializer.SerializeToElement("{}");values["ConfigurationUserData"]=JsonSerializer.SerializeToElement("{\"other\":17}");
 var definitions=DistortionCalibrationMetadata.DisplayObjects;bool reject=false;var store=new ToolDisplayFeatureStore(definitions,()=>values,next=>{if(reject)throw new Exception("rejected");values=next;});
 var old=new ToolTextDisplaySettings("old","#00ff00","#ff0000",10,1,3,4);var legacy=new ToolDisplayConfigurationObjectService(()=>old,next=>{old=next;return true;});var service=new ToolDisplayNamedTextConfigurationObjectService(store,legacy);
 var route=new ConfigurationObjectRoute(name,"FontSize",ConfigurationObjectProperty.FontSize);var binding=service.Resolve(route)!;
 Check(binding.Read()=="10","source font default");Check(ReferenceEquals(binding,service.Resolve(route)),"stable bridge identity");Check(binding.Write("18").Accepted,"named feature write");
 Check(values["DisplaySettings"].GetString()=="legacy-snapshot"&&values["DisplayObjectSettings"].GetString()=="{}","legacy snapshots unchanged");
 using(var data=JsonDocument.Parse(values["ConfigurationUserData"].GetString()!)){Check(data.RootElement.GetProperty("other").GetInt32()==17,"unrelated user data preserved");Check(data.RootElement.GetProperty(ToolDisplayFeatureStore.Key(name,"FontSize")).GetString()=="18","source feature key");}
 Check(service.Resolve(new("Other","FontSize",ConfigurationObjectProperty.FontSize)) is null,"unknown object rejected");
 foreach(var bad in new[]{"NaN","Infinity","0","-1","bad"})Check(!binding.Write(bad).Accepted&&binding.Read()=="18","invalid font rejected");
 reject=true;Check(!binding.Write("20").Accepted&&binding.Read()=="18","host rejection readback");reject=false;
 var noop=new ToolDisplayFeatureStore(definitions,()=>values,_=>{});Check(!noop.Write(name,"FontSize","21").Accepted,"no-op apply rejected");Check(!store.Write(name,"FontSize",new string('中',172)).Accepted,"source UTF8 limit");
 var legacyFont=service.Resolve(new("Show Text","FontSize",ConfigurationObjectProperty.FontSize))!;Check(legacyFont.Write("22").Accepted&&old.FontSize==22&&binding.Read()=="18","legacy service remains authoritative");
 var model=ToolConfigurationDefinition.Parse("<AlgorithmTabRoot><Tabs><Tab Name='T'><Items><FloatConfig Name='Size'><AccessMode>O</AccessMode><CurValue>DO_NOT_WRITE</CurValue><PropertyName>Value</PropertyName><MinValue>6</MinValue><MaxValue>72</MaxValue><IncValue>1</IncValue><BrigdgeSetter><OperationName>GetDisplayObjectFeatureOperation</OperationName><OperationParams>Show Evaluation Error Text;FontSize</OperationParams></BrigdgeSetter></FloatConfig></Items></Tab></Tabs></AlgorithmTabRoot>");
 var state=new ToolConfigurationState(model);var registry=new ToolConfigurationControlRegistry();ToolConfigurationObjectControls.Register(registry,service);using(var view=new ToolConfigurationView(state,registry)){view.Measure(new Size(420,500));view.Arrange(new Rect(0,0,420,500));view.UpdateLayout();var spinner=Walk(view).OfType<ConfigurationNumericSpinner>().Single();spinner.Editor.Text="24";Check(spinner.CommitText()&&binding.Read()=="24","real WPF spinner writes named feature");spinner.Step(1);Check(binding.Read()=="25","source increment");Check(state.Find("Size")!.Value=="DO_NOT_WRITE","algorithm CurValue unchanged");}
 var node=new NodeDocument("n",DistortionCalibrationMetadata.ModuleId,"1.0.0","畸变",values,0,0,100);var procedure=new ProcedureDocument("p","流程",[node],[]);var solution=new SolutionDocument(1,"s","方案",[procedure],[]);var path=Path.GetFullPath("analysis/named-text-checks/saved.json");Directory.CreateDirectory(Path.GetDirectoryName(path)!);SolutionStore.Save(path,solution);values=SolutionStore.OpenWorkspace(path).Document.Procedures[0].Nodes[0].Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());Check(binding.Read()=="25","save reopen feature");Check(values["DisplaySettings"].GetString()=="legacy-snapshot","save reopen legacy snapshot");
 var outputs=new Dictionary<string,PortValue>{["CalibPointX"]=PortValue.Numbers([12,32]),["CalibPointY"]=PortValue.Numbers([16,46]),["CalibPointStatus"]=PortValue.Numbers([0,1]),["CalibPointError"]=PortValue.Numbers([.25,.5])};var inputs=new Dictionary<string,PortValue>();
 Check(ToolDisplayGraphics.Build(definitions,values,inputs,outputs).All(g=>g.Type!="text"),"old Build remains opt-out");var graphics=ToolDisplayGraphics.BuildNamedTexts(definitions,values,inputs,outputs);Check(graphics.Count==2&&graphics.All(g=>g.FontSize==25),"per-point stored font projection");Check(graphics[0].Points[0].Text=="0.25"&&graphics[1].Points[0].Text=="0.5","source content mapping");Check(graphics[0].Points[0].X==12&&graphics[1].Points[0].Y==46,"source position projection");Check(graphics[0].Color!=graphics[1].Color,"per-point status colors");
 var presenter=new ToolDisplayImagePresenter();var frame=new ToolDisplayFrame(null,"global","#00ff00",10,1,0,0,"",[]){Graphics=graphics,TextVisible=true};presenter.Show(frame);presenter.Measure(new Size(640,480));presenter.Arrange(new Rect(0,0,640,480));presenter.UpdateLayout();var texts=Walk(presenter).OfType<TextBlock>().ToArray();Check(texts.Length==3&&texts.Count(t=>t.FontSize==25)==2,"actual presenter named and global text");Check(texts.Where(t=>t.Tag is string).Select(Canvas.GetLeft).SequenceEqual(new double[]{12,32}),"actual canvas positions");presenter.Show(frame);Check(Walk(presenter).OfType<TextBlock>().Count()==3,"refresh no duplicate labels");
 var pointDefinition=definitions.Single(d=>d.Name=="Display Calibration Point");
 ToolDisplayGraphic PointGraphic()=>ToolDisplayGraphics.Build([pointDefinition],values,inputs,outputs).Single();
 var pointGraphic=PointGraphic();Check(pointGraphic.Points.Select(p=>p.Index).SequenceEqual(new[]{0,1}),"point source indexes preserved");
 Check(pointGraphic.Points.Select(p=>p.Color).SequenceEqual(new[]{pointDefinition.DefaultStyle.NgColor,pointDefinition.DefaultStyle.OkColor}),"mixed point status uses per-index color");
 presenter.Show(frame with{Graphics=[pointGraphic],TextVisible=false});
 var markers=Walk(presenter).OfType<System.Windows.Shapes.Path>().Where(p=>Equals(p.Tag,pointDefinition.Name)).ToArray();
 Check(markers.Length==2&&markers.Select(p=>p.Stroke.ToString()).SequenceEqual(pointGraphic.Points.Select(p=>((Brush)new BrushConverter().ConvertFromInvariantString(p.Color!)!).ToString())),"presenter consumes each point color");
 outputs["CalibPointStatus"]=PortValue.Numbers([1]);Check(PointGraphic().Points.All(p=>p.Color==pointDefinition.DefaultStyle.OkColor),"single status broadcasts");
 outputs["CalibPointStatus"]=PortValue.Numbers([]);Check(PointGraphic().Points.All(p=>p.Color==pointDefinition.DefaultStyle.NgColor),"empty status defaults NG");
 outputs["CalibPointStatus"]=PortValue.Numbers([0,1]);
 Check(binding.Write("14.5").Accepted,"source fractional string persisted");var fractional=ToolDisplayGraphics.BuildNamedTexts(definitions,values,inputs,outputs);Check(fractional.All(g=>g.FontSize==0),"source int string conversion fallback");presenter.Show(frame with{Graphics=fractional});Check(Walk(presenter).OfType<TextBlock>().Count()==1,"invalid WPF font safely skipped");
 values["ConfigurationUserData"]=JsonSerializer.SerializeToElement("bad");Check(!store.Write(name,"FontSize","30").Accepted&&values["ConfigurationUserData"].GetString()=="bad","corrupt user data preserved on rejection");
 }
 static IEnumerable<DependencyObject> Walk(DependencyObject item){yield return item;for(int i=0;i<VisualTreeHelper.GetChildrenCount(item);i++)foreach(var child in Walk(VisualTreeHelper.GetChild(item,i)))yield return child;}
}
