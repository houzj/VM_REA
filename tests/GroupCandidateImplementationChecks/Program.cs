using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VisionEditor;
using VisionFramework;

internal static class Program
{
 [STAThread] static void Main()
 {
  int checks=0;void Check(bool condition,string label){if(!condition)throw new Exception(label);Console.WriteLine("PASS "+label);checks++;}
  var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};
  var registry=new ModuleRegistry();
  var descriptor=new ModuleDescriptor("test.group.search","1","ModuleNeedle",[],[],[new("Gain",ParameterKind.Number,true,JsonSerializer.SerializeToElement(1d)),new("gain",ParameterKind.Integer,true,JsonSerializer.SerializeToElement(2)),new("enabled",ParameterKind.Boolean,true,JsonSerializer.SerializeToElement(true))]);
  registry.Register(descriptor,()=>new UnusedModule(descriptor));
  var body=new ProcedureDocument("body","Group",[new("node",descriptor.Id,"1","ModuleNeedle",new())],[],[]){Configuration=new(){RunCategories=[new("category","分组0")]}};
  bool fail=false;
  void Save(WorkflowConfiguration value){if(fail){fail=false;throw new InvalidOperationException("controlled save failure");}body=body with{Configuration=value};}
  void Categories(StackPanel panel,ProcedureDocument current)
  {
   panel.Children.Add(new Button{Tag="GroupCategoryAdd"});
   foreach(var category in current.Configuration.RunCategories??[]){var row=new DockPanel();row.Children.Add(new TextBox{Text=category.Name});foreach(var tag in new[]{"GroupCategoryUp:","GroupCategoryDown:","GroupCategoryDelete:"})row.Children.Add(new Button{Tag=tag+category.Id});panel.Children.Add(row);}
  }
  void Commands(StackPanel panel,WorkflowRunParameter proxy,ProcedureDocument current){var row=new DockPanel();row.Children.Add(new Button());row.Children.Add(new Button());panel.Children.Add(row);}
  var window=new GroupRunConfigurationWindow(()=>body,registry,Save,Commands,Categories,new()){ShowInTaskbar=false,WindowStartupLocation=WindowStartupLocation.Manual,Left=-20000,Top=-20000};window.Show();window.UpdateLayout();
  TextBox Search()=>Children(window).OfType<TextBox>().Single(c=>Equals(c.Tag,"GroupRunSearch"));
  CheckBox Candidate(string name){window.UpdateLayout();return Children(window).OfType<CheckBox>().Single(c=>Equals(c.Tag,"GroupRunCandidate:node:"+name+":False"));}
  int Count(){window.UpdateLayout();return Children(window).OfType<CheckBox>().Count(c=>c.Tag?.ToString()?.StartsWith("GroupRunCandidate:")==true);}
  void Click(CheckBox check,bool value){check.IsChecked=value;check.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));}
  Check(Count()==3,"Empty search retains all compatibility descriptor candidates");
  Search().Text="ModuleNeedle";Check(Count()==0,"Module display name is outside parameter search scope");
  Search().Text="Gain";Check(Count()==1&&Candidate("Gain") is not null,"Search preserves parameter case");
  Search().Text="GAIN";Check(Count()==0,"Uppercase mismatch does not match parameter name");
  Search().Text="gain";Check(Count()==1&&Candidate("gain") is not null,"Lowercase distinct parameter remains searchable");
  Search().Text="";var old=Candidate("Gain");Click(old,true);
  Check(body.Configuration.RunParameters.Count==1,"Leaf check creates one Group proxy");
  var id=body.Configuration.RunParameters.Single().Id;Click(old,true);
  Check(body.Configuration.RunParameters.Count==1&&body.Configuration.RunParameters.Single().Id==id,"Repeated event from detached checkbox does not duplicate proxy identity");
  Search().Text="gain";Click(Candidate("gain"),true);Check(body.Configuration.RunParameters.Count==2,"Case distinct parameter can create its own proxy");
  Click(old,false);Check(body.Configuration.RunParameters.Count==1&&body.Configuration.RunParameters.Single().Parameter=="gain","Stale uncheck removes only exact parameter identity");
  Search().Text="enabled";fail=true;Click(Candidate("enabled"),true);Check(body.Configuration.RunParameters.Count==1,"Rejected save leaves proxy list unchanged");
  Click(Candidate("enabled"),true);Check(body.Configuration.RunParameters.Count==2,"Retry after rejected save creates one proxy");
  Search().Text="";window.Rebuild();Check(Candidate("enabled").IsChecked==true&&Candidate("Gain").IsChecked==false,"Rebuild restores checks from current persisted identities");
  window.Close();app.Shutdown();Console.WriteLine($"PASS {checks} Group candidate implementation checks; no native or desktop acceptance claimed");
 }
 static IEnumerable<FrameworkElement> Children(DependencyObject root){if(root is FrameworkElement element)yield return element;for(int i=0;i<VisualTreeHelper.GetChildrenCount(root);i++)foreach(var child in Children(VisualTreeHelper.GetChild(root,i)))yield return child;}
}
sealed class UnusedModule(ModuleDescriptor descriptor):IModule
{
 public ModuleDescriptor Descriptor=>descriptor;
 public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken cancellationToken)=>throw new NotSupportedException("This fixture never executes tools.");
}
