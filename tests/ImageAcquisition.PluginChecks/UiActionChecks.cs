using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Button=System.Windows.Controls.Button;
using ComboBox=System.Windows.Controls.ComboBox;
using ButtonBase=System.Windows.Controls.Primitives.ButtonBase;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.ImageSource;
static partial class Program {
 static void UiActions(){
  var plugin=new ImageAcquisitionPlugin();var p=plugin.Descriptor.Parameters.ToDictionary(d=>d.Name,d=>d.DefaultValue!.Value.Clone());Dictionary<string,JsonElement>? applied=null;string? subscription=null;
  var node=new NodeDocument("actions",plugin.Manifest.Id,"1.0.0","多图采集",p);using var scope=new ModuleExecutionScope();
  var context=new ToolEditorContext(node,Environment.CurrentDirectory,values=>applied=values,_=>throw new NotSupportedException()){Scope=scope,PickSubscription=(_,key,parameter,kind)=>{Check(parameter&&kind==DataKind.Integer,"native integer parameter subscription");subscription=key;return "变量.数量";}};
  var window=plugin.CreateWpfEditor(context);window.Left=-30000;window.Top=-30000;window.ShowActivated=false;window.ShowInTaskbar=false;window.Show();window.UpdateLayout();
  Button Button(string name)=>Descendants(window).OfType<Button>().Single(b=>b.Name==name);
  try{
   Check(window.Width==420&&window.Height==500,"desktop logical dimensions");
   Check(Descendants(window).OfType<ComboBox>().Single(c=>c.Name=="AcquisitionCamera").SelectedItem is CameraInfo{Id:-1,Name:"请选择"},"desktop camera placeholder");
   for(int i=0;i<8;i++)Button("AddAcquisitionConfig").RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));window.UpdateLayout();
   Check(JsonSerializer.Deserialize<AcquisitionImageConfig[]>(applied!["ImageConfigList"].GetString()!)!.Length==8&&!Button("AddAcquisitionConfig").IsEnabled,"UI maximum eight configuration items");
   Button("DeleteConfig3").RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));window.UpdateLayout();
   Check(JsonSerializer.Deserialize<AcquisitionImageConfig[]>(applied!["ImageConfigList"].GetString()!)!.Length==7&&Descendants(window).OfType<TextBlock>().Any(t=>t.Text=="图像配置 7")&&!Descendants(window).OfType<TextBlock>().Any(t=>t.Text=="图像配置 8"),"UI deletion renumbers items");
   var mode=Descendants(window).OfType<ComboBox>().Single(c=>c.Name=="AcquisitionMode");mode.SelectedIndex=0;window.UpdateLayout();
   var count=Descendants(window).OfType<AcquisitionNumericControl>().First();count.Text="2";Check(applied!["ImageCount"].GetInt32()==4,"UI rejects count below native limit");count.Text="3";Check(applied!["ImageCount"].GetInt32()==3,"UI accepts count minimum");Button("SubscribeImageCount").RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));Check(subscription=="ImageCount","UI invokes typed count subscription");
   string root=Path.GetFullPath("analysis/full-audit/image-acquisition/ui-path-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(root);Fixture(Path.Combine(root,"one.bmp"),10);File.WriteAllText(Path.Combine(root,"test.txt"),"preserve");string sub=Path.Combine(root,"sub"),deep=Path.Combine(sub,"deep");Directory.CreateDirectory(deep);Fixture(Path.Combine(deep,"deep.bmp"),20);
   var setDirectory=window.GetType().GetMethod("SetDirectory",BindingFlags.NonPublic|BindingFlags.Instance)!;setDirectory.Invoke(window,[root,false]);Check(JsonSerializer.Deserialize<string[]>(applied!["ImagePaths"].GetString()!)!.Length==0,"typed directory clears without browse import");setDirectory.Invoke(window,[root,true]);Check(JsonSerializer.Deserialize<string[]>(applied!["ImagePaths"].GetString()!)!.Length==1,"browse imports UI depth only");Check(File.ReadAllText(Path.Combine(root,"test.txt"))=="preserve"&&!Directory.GetFiles(root,".vision-access-*").Any(),"access probe does not overwrite existing files");
   var sdk=plugin.AppendSdkImageDirectory(node,root);Check(JsonSerializer.Deserialize<string[]>(sdk.Parameters["ImagePaths"].GetString()!)!.Length==2,"SDK append is recursive independently of UI browse");
  }finally{window.Close();}
 }
}
