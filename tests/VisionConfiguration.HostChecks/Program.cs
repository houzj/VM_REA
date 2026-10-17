using System.Windows;
using VisionPluginContracts;
static class Program {
 [STAThread] static int Main(){try{Run();return 0;}catch(Exception error){Console.Error.WriteLine(error);return 1;}}
 static void Run(){
  int checks=0;void Check(bool ok,string message){checks++;if(!ok)throw new Exception(message);}
  var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};
  Window Hidden()=>new(){Width=100,Height=100,Left=-20000,Top=-20000,WindowStartupLocation=WindowStartupLocation.Manual,ShowActivated=false,ShowInTaskbar=false};
  var owner=Hidden();owner.Show();
  try{
   using var host=new ToolConfigurationWindowHost(owner);
   var first=new ToolConfigurationIdentity("solution","procedure","one");
   int created=0,changes=0;host.Changed+=()=>changes++;
   var a=host.Open(first,()=>{created++;return Hidden();});
   Check(a.IsVisible&&owner.IsEnabled,"Ordinary configuration opens nonmodally and leaves shell enabled");
   Check(ReferenceEquals(a.Owner,owner)&&!a.ShowInTaskbar,"Configuration belongs to shell without a second taskbar entry");
   Check(ReferenceEquals(host.Open(first,()=>{created++;return Hidden();}),a)&&created==1,"Same node reuses open configuration without another factory call");
   bool closed=false;a.Closed+=(_,_)=>closed=true;
   var second=first with{NodeId="two"};var b=host.Open(second,Hidden);
   Check(closed&&!a.IsVisible&&b.IsVisible,"Switching modules closes previous ordinary configuration");
   Check(host.Identity==second&&ReferenceEquals(host.Current,b),"Host tracks the complete open node identity");
   b.Close();Check(host.Current is null&&host.Identity is null,"User closure releases configuration identity");
   Check(changes==4,"Host publishes open and close lifecycle transitions");
   var c=host.Open(first,Hidden);host.Close();Check(!c.IsVisible&&host.Current is null,"Explicit solution shutdown closes owned configuration");
   bool failure=false;try{host.Open(first,()=>throw new InvalidOperationException("factory"));}catch(InvalidOperationException){failure=true;}
   Check(failure&&host.Current is null,"Factory failure leaves host reusable");
   var sameNodeOtherSolution=first with{SolutionId="different"};host.Open(first,Hidden);var d=host.Open(sameNodeOtherSolution,Hidden);
   Check(ReferenceEquals(host.Current,d)&&host.Identity==sameNodeOtherSolution,"Identical node ids in different solutions do not share configuration");
   host.Dispose();Check(!d.IsVisible&&host.Current is null,"Disposal closes owned configuration");
   var definition=ToolConfigurationDefinition.Parse("<AlgorithmTabRoot><Tabs><Tab Name='Tab_Basic Params'><Categorys><Category Name='group'><Items><Boolean Name='enabled'/></Items></Category></Categorys></Tab></Tabs></AlgorithmTabRoot>");
   var state=new ToolConfigurationState(definition);var registry=new ToolConfigurationControlRegistry();
   registry.Register("Boolean",context=>new System.Windows.Controls.Primitives.ToggleButton{Content="Explicit test factory"});
   bool released=false;var session=new ToolConfigurationSession(state,registry,release:()=>released=true);
   var node=new VisionFramework.NodeDocument("fixture","test","1","框架模型宿主",new());
   var context=new ToolEditorContext(node,Environment.CurrentDirectory,_=>{},_=>throw new NotSupportedException());
   var modelWindow=new ToolConfigurationModelWindow(context,session){Left=-20000,Top=-20000,WindowStartupLocation=WindowStartupLocation.Manual,ShowActivated=false};
   host.Open(first,()=>modelWindow);
   Check(modelWindow.Width==420&&modelWindow.Height==500,"Common window geometry comes from original configuration model");
   Check(modelWindow.ConfigurationView.Tabs.Items.Count==1&&!modelWindow.ConfigurationView.TabHeadersVisible,"Single-page definition does not acquire invented tabs");
   Check(modelWindow.ConfigurationView.Diagnostics.Count==0,"Explicit registered leaf factory is consumed by common shell");
   session.RequestClose();Check(released&&host.Current is null,"Model close request releases shared view and configuration adapter");
   Console.WriteLine($"{checks} configuration host lifecycle checks passed; original source comparison, not full template parity.");
  }finally{owner.Close();app.Shutdown();}
 }
}
