using System.IO;
using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using VisionFramework;
using VisionPluginContracts;
internal static class Program {
 static int checks;static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
 [STAThread] static int Main(){try{Run();Console.WriteLine($"VisionConfiguration.IntegerMetadataChecks: {checks} checks passed");return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
 static void Run(){
  _=new Application();var fixtures=Path.Combine(AppContext.BaseDirectory,"Fixtures");var originalXml=File.ReadAllText(Path.Combine(fixtures,"IMVSCaliperModuAlgorithmTab.xml"));var groups=File.ReadAllText(Path.Combine(fixtures,"Group.xml"));
  var definition=ToolConfigurationDefinition.Parse(originalXml,groups);var original=new ToolConfigurationState(definition);var originalNode=original.Find("PositionNegX0")!;
  Check(originalNode.Value=="-100","real Caliper signed CurValue -100");Check(originalNode.Get("DefaultValue")=="-100","real Caliper signed default -100");Check(originalNode.Get("MinValue")=="-100000"&&originalNode.Get("MaxValue")=="100000"&&originalNode.Get("IncValue")=="1","real Caliper signed range and step");
  Check(originalNode.Element.OriginalXml.Contains("0xffffff9c")&&originalNode.Element.OriginalXml.Contains("0xfffe7960"),"immutable definition keeps original hex evidence");
  using var native=JsonDocument.Parse(File.ReadAllText(Path.Combine(fixtures,"original-integer-metadata.json")));
  foreach(var field in native.RootElement.GetProperty("fields").EnumerateArray())Check(field.GetProperty("type").GetString()=="System.Int32","original reflection confirms Int32 field");
  foreach(var row in native.RootElement.GetProperty("rows").EnumerateArray()) {
   string input=row.GetProperty("input").GetString()!,expected=row.GetProperty("value").GetInt32().ToString(CultureInfo.InvariantCulture);
   foreach(var field in new[]{"CurValue","DefaultValue","MinValue","MaxValue","IncValue"}) {
    var xml="<AlgorithmTabRoot><Tabs><Tab Name='A'><Categorys><Category Name='C'><Items><Integer Name='X'><"+field+">"+input+"</"+field+"></Integer></Items></Category></Categorys></Tab></Tabs></AlgorithmTabRoot>";
    var state=new ToolConfigurationState(ToolConfigurationDefinition.Parse(xml));Check(state.Find("X")!.Get(field)==expected,"original ParseInt boundary "+field+"="+input);
   }
  }
  foreach(var type in new[]{"Integer","IntegerBx","IntegerConfirm","Integer_CircleFind","Integer_CirclePairInsp","Integer_CaliperHeight","Integer_CaliperHeight2","Integer_IdeaWidth"}) {
   var xml="<AlgorithmTabRoot><Tabs><Tab Name='A'><Categorys><Category Name='C'><Items><"+type+" Name='X'><CurValue>0xffffffff</CurValue><DefaultValue>0x80000000</DefaultValue><MinValue>0x80000000</MinValue><MaxValue>0x7fffffff</MaxValue><IncValue>0x1</IncValue></"+type+"></Items></Category></Categorys></Tab></Tabs></AlgorithmTabRoot>";
   var state=new ToolConfigurationState(ToolConfigurationDefinition.Parse(xml));var node=state.Find("X")!;Check(node.IsInteger32&&node.Value=="-1"&&node.Get("DefaultValue")=="-2147483648","source-confirmed derived type "+type);Check(node.Get("MinValue")=="-2147483648"&&node.Get("MaxValue")=="2147483647"&&node.Get("IncValue")=="1","source-confirmed derived range "+type);
  }
  var exclusion=ToolConfigurationDefinition.Parse("<AlgorithmTabRoot><Tabs><Tab Name='A'><Categorys><Category Name='C'><Items><Float Name='F'><CurValue>0xffffffff</CurValue><MinValue>0xffffffff</MinValue></Float><String Name='S'><CurValue>0xffffffff</CurValue></String><Integer64 Name='L'><CurValue>0xffffffff</CurValue></Integer64><EnumerationDataCenter Name='E'><CurValue>0xffffffff</CurValue></EnumerationDataCenter></Items></Category></Categorys></Tab></Tabs></AlgorithmTabRoot>");var excluded=new ToolConfigurationState(exclusion);
  Check(excluded.Find("F")!.Value=="0xffffffff"&&excluded.Find("F")!.Get("MinValue")=="0xffffffff","never apply Int32 conversion to Float");Check(excluded.Find("S")!.Value=="0xffffffff"&&excluded.Find("L")!.Value=="0xffffffff","string and Int64 not reinterpreted as Int32");Check(excluded.Find("E")!.Value=="-1","confirmed EnumerationDataCenter inherits signed Int32 sentinel");
  var focusXml=new XElement("AlgorithmTabRoot",new XElement("Tabs",new XElement("Tab",new XAttribute("Name","Focused Caliper field"),new XElement("Categorys",new XElement("Category",new XAttribute("Name","Caliper / PositionNegX0"),new XElement("Items",XElement.Parse(originalNode.Element.OriginalXml))))))).ToString();
  var values=new Dictionary<string,JsonElement>{{"PositionNegX0",JsonSerializer.SerializeToElement(-100)}};
  var descriptor=new ModuleDescriptor("audit","1","Audit",[],[],new[]{new ParameterDescriptor("PositionNegX0",ParameterKind.Integer,true,Minimum:-100000,Maximum:100000)});
  var adapter=new ToolConfigurationParameterAdapter(descriptor,()=>values,v=>values=v);var focused=new ToolConfigurationState(ToolConfigurationDefinition.Parse(focusXml),adapter);focused.Initialize();focused.SetProgrammatic("PositionNegX0","CustomVisible","True");
  var registry=new ToolConfigurationControlRegistry();ToolConfigurationStandardControls.Register(registry);using var view=new ToolConfigurationView(focused,registry);
  view.Measure(new Size(420,500));view.Arrange(new Rect(0,0,420,500));view.UpdateLayout();
  var group=(StackPanel)((StackPanel)((ScrollViewer)((TabItem)view.Tabs.Items[0]).Content).Content).Children[0];var rowControl=group.Children.OfType<Grid>().Single();var editor=rowControl.Children.OfType<Grid>().Single();var spinner=editor.Children.OfType<ConfigurationNumericSpinner>().Single();
  Check(spinner.Minimum==-100000&&spinner.Maximum==100000&&spinner.Increment==1,"production spinner reads normalized source metadata");Check(spinner.Editor.Text=="-100","production spinner preserves signed source value");  var output=Path.GetFullPath("analysis/full-audit/configuration-review");Directory.CreateDirectory(output);var bitmap=new RenderTargetBitmap(420,500,96,96,System.Windows.Media.PixelFormats.Pbgra32);bitmap.Render(view);var encoder=new PngBitmapEncoder();encoder.Frames.Add(BitmapFrame.Create(bitmap));using var stream=File.Create(Path.Combine(output,"caliper-integer-source-derived.png"));encoder.Save(stream);
  spinner.Step(1);Check(focused.Find("PositionNegX0")!.Value=="-99"&&values["PositionNegX0"].GetInt32()==-99,"production spinner step writes -99 rather than clamping huge unsigned value to zero");
  focused.SetProgrammatic("PositionNegX0","MinValue","0xffffff9c");Check(focused.Find("PositionNegX0")!.Get("MinValue")=="-100"&&spinner.Minimum==-100,"typed programmatic metadata uses same signed conversion");

 }
}
