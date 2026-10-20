using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VisionPluginContracts;
using VisionPlugins.Calibration;
static class Program {
 static int checks;static void Check(bool value,string message){if(!value)throw new Exception(message);Console.WriteLine($"PASS {++checks}: {message}");}
 static IEnumerable<DependencyObject> Walk(DependencyObject item){yield return item;for(int i=0;i<VisualTreeHelper.GetChildrenCount(item);i++)foreach(var child in Walk(VisualTreeHelper.GetChild(item,i)))yield return child;}
 [STAThread]static int Main(){try{
 _=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};var values=NImageParameters.Defaults();var service=new Service();int applies=0;
 var context=new ToolEditorContext(new("dist",NImageConfigurationProvider.ModuleId,"1","dist",values),Environment.CurrentDirectory,next=>{values=next;applies++;},_=>throw new Exception("No algorithm execution in image enumeration test")){ConfigurationSubscriptions=service};
 using var session=new NImageConfigurationProvider(_=>throw new Exception("No clear"),(_,_)=>throw new Exception("No export")).CreateConfiguration(context);var state=session.State;var image=state.Find("ImageSource")!;
 Check(service.Queries.Count==1,"provider Initialize dispatches original image enumeration exactly once before any control dropdown");
 var query=service.Queries.Single();Check(query.Destination==new ConfigurationSubscriptionDestination(ConfigurationSubscriptionDomain.Combination,"InputImage","IMAGE")&&query.NativeTypes.SequenceEqual(new[]{"IMAGE"}),"source Initers IMAGE query uses InputImage combination destination");
 Check(!state.Diagnostics.Any(d=>d.Operation=="GetFrontParamItemsOperation"),"image initializer has no Unsupported operation diagnostic");
 var cc=(ToolConfigurationControlContext)Activator.CreateInstance(typeof(ToolConfigurationControlContext),BindingFlags.Instance|BindingFlags.NonPublic,null,new object[]{state,image,(Func<Window?>)(()=>null)},null)!;
 Check(session.Controls.TryCreate("EnumerationG",cc,out var element),"real provider owns original EnumerationG control");element!.Measure(new Size(400,100));element.Arrange(new Rect(0,0,400,100));element.UpdateLayout();var combo=Walk(element).OfType<ComboBox>().Single();
 Check(combo.Items.OfType<ComboBoxItem>().Any(i=>i.Tag is ConfigurationSubscriptionTreeNode{StableId:"visible-image"}),"source initialized typed IMAGE entries exist before dropdown opens");var window=new Window{Content=element,Width=400,Height=100,Left=-20000,Top=-20000,ShowActivated=false,ShowInTaskbar=false};window.Show();window.UpdateLayout();combo.IsDropDownOpen=true;Check(service.Queries.Count==2,"actual dropdown refresh re-enumerates rather than replacing Initialize contract");
 var choice=combo.Items.OfType<ComboBoxItem>().Single(i=>i.Tag is ConfigurationSubscriptionTreeNode{StableId:"visible-image"});Check((string)choice.Content=="Visible image","actual IMAGE dropdown presents service-provided visible source caption");
 combo.SelectedItem=choice;Check(service.LastDestination==query.Destination&&service.Selected==service.Image,"actual selection writes InputImage combination identity");Check(image.Value=="Visible image"&&image.SubscriptionBinding==service.Image,"selection reads authoritative presentation and typed binding into state");Check(applies==0,"image subscription selection does not write algorithm parameters or execute");combo.IsDropDownOpen=false;window.Close();
 Console.WriteLine($"PASS {checks}: NImage actual provider image source Initialize/control/selection");return 0;
 }catch(Exception error){Console.Error.WriteLine(error);return 1;}}
 sealed class Service:IToolConfigurationSubscriptionService {
  public readonly List<ConfigurationSubscriptionQuery> Queries=[];public ConfigurationSubscriptionBinding Image=new(new ConfigurationOutputReference("procedure","image-source","OutputImage","IMAGE"),ConfigurationSubscriptionIndex.All);public ConfigurationSubscriptionBinding? Selected;public ConfigurationSubscriptionDestination? LastDestination;
  public ConfigurationSubscriptionTreeResult Enumerate(ConfigurationSubscriptionQuery query){Queries.Add(query);return new(ConfigurationSubscriptionStatus.Available,[new("visible-image","Visible image",binding:Image)]);}
  public ConfigurationSubscriptionBindingResult Read(ConfigurationSubscriptionDestination destination)=>new(ConfigurationSubscriptionStatus.Available,destination.Name=="InputImage"?Selected:null,destination.Name=="InputImage"&&Selected is not null?"Visible image":"");
  public ConfigurationSubscriptionBindingResult Bind(ConfigurationSubscriptionDestination destination,ConfigurationSubscriptionBinding? binding){LastDestination=destination;Selected=binding;return new(ConfigurationSubscriptionStatus.Available,binding,binding is null?"":"Visible image");}
 }
}


