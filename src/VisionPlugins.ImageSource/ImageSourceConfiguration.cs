using Brushes=System.Windows.Media.Brushes;
using Color=System.Windows.Media.Color;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VisionFramework;
using VisionPluginContracts;
using W=System.Windows.Controls;
namespace VisionPlugins.ImageSource;

public sealed partial class ImageSourcePlugin:IToolConfigurationProvider {
 public ToolConfigurationSession CreateConfiguration(ToolEditorContext context){
  const string language="Chinese";var text=ConfigurationLocalization.ForLanguage(language).Get;
  string Resource(string file){using var stream=typeof(ImageSourcePlugin).Assembly.GetManifestResourceStream("VisionPlugins.ImageSource.Resources."+file)??throw new InvalidDataException("Missing image source model resource.");using var reader=new StreamReader(stream);return reader.ReadToEnd();}
  var definition=ToolConfigurationDefinition.Parse(Resource("ImageSourceAlgorithmTab.xml"),Resource("ImageSourceGroups.xml"));
  var values=Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());foreach(var p in context.Node.Parameters)values[p.Key]=p.Value.Clone();
  void Apply(Dictionary<string,JsonElement> next){
   if(next["CameraID"].GetInt32()!=values["CameraID"].GetInt32()){int id=next["CameraID"].GetInt32();next["CameraIdentity"]=J(context.Cameras?.Invoke().FirstOrDefault(c=>c.Id==id)?.PersistentId??"");next["ImagePaths"]=J("[]");next["SelectedPaths"]=J("[]");next["CurrentImagePath"]=J("");}
   context.Apply(next.ToDictionary(p=>p.Key,p=>p.Value.Clone()));values=next;
  }
  Dictionary<string,string> User(){try{return JsonSerializer.Deserialize<Dictionary<string,string>>(values["ConfigurationUserData"].GetString()??"{}")??[];}catch(JsonException error){throw new InvalidDataException("Invalid image-source configuration user data.",error);}}
  var parameters=new ToolConfigurationParameterAdapter(Descriptor,()=>values,Apply,key=>User().GetValueOrDefault(key),(key,value)=>{var user=User();user[key]=value;var next=new Dictionary<string,JsonElement>(values);next["ConfigurationUserData"]=J(JsonSerializer.Serialize(user));Apply(next);});
  ToolConfigurationState? state=null;ToolConfigurationSession? session=null;Func<Window?> owner=()=>null;
  void NotifyFolder(string key,bool error){var result=(context.ConfigurationNotifications??new WpfConfigurationNotificationService()).Notify(owner(),new(error?ConfigurationNotificationKind.Error:ConfigurationNotificationKind.Success,text(key),Mode:context.Mode));if(result.Status==ConfigurationNotificationStatus.Failed)state?.ReportDiagnostic(new("NotificationService","OpenFolderDialog","SubscribeFolderPath",result.Diagnostic??"Notification service failed."));}
  ConfigurationExecutionHostStatus Host()=>context.ConfigurationExecutionStatus?.Invoke()??new(false,false,false,false,false);
  var execution=new ToolConfigurationExecution(()=>context.Run(new(values)),()=>Host().SolutionBusy,()=>Host().Ready&&Host().RunEnabled,()=>Host().Loading,()=>Host().ProcedureBusy);
  var dispatcher=new ImageSourceDispatcher(request=>{
   if(request.Kind is ConfigurationOperationKind.ReadInputBinding or ConfigurationOperationKind.WriteInputBinding or ConfigurationOperationKind.ReadParameterBinding or ConfigurationOperationKind.WriteParameterBinding or ConfigurationOperationKind.ClearParameterBinding){
    var query=ConfigurationSubscriptionValidation.FromElement(request.Source.Element,request.Kind is ConfigurationOperationKind.ReadParameterBinding or ConfigurationOperationKind.WriteParameterBinding or ConfigurationOperationKind.ClearParameterBinding?ConfigurationSubscriptionDomain.Parameter:null);
    if(query is null)return new(true,Error:"Unsupported source-declared input query.");
    if(context.ConfigurationSubscriptions is null)return new(true,Error:"Typed subscription service is unavailable.");
    bool write=request.Kind is ConfigurationOperationKind.WriteInputBinding or ConfigurationOperationKind.WriteParameterBinding or ConfigurationOperationKind.ClearParameterBinding;
    if(write&&request.SubscriptionBinding is null&&!string.IsNullOrEmpty(request.Value)&&request.Kind!=ConfigurationOperationKind.ClearParameterBinding)return new(true,Error:"A typed subscription identity is required.");
    var result=write?context.ConfigurationSubscriptions.Bind(query.Destination,request.Kind==ConfigurationOperationKind.ClearParameterBinding?null:request.SubscriptionBinding):context.ConfigurationSubscriptions.Read(query.Destination);
    if(result.Status!=ConfigurationSubscriptionStatus.Available)return new(true,Error:result.Diagnostic??result.Status.ToString());
    state!.ApplySubscription(request.Source,result,query.Destination.Domain==ConfigurationSubscriptionDomain.Parameter);return new(true,result.DisplayText??"");
   }
   if(request.Kind==ConfigurationOperationKind.WriteParameter&&request.Source.Name=="SubscribeFolderPath"){
    var value=request.Value??"";ConfigurationOperationResult Fail(string key){NotifyFolder(key,true);return new(true,Error:text(key));}
    if(string.IsNullOrWhiteSpace(value))return Fail("Lang_FilePathEptErr");if(!LocalFolderSelection.HasLocalSyntax(value))return Fail("Lang_FilePathFmtErr");if(value.Length>200)return Fail("Lang_FilePathLenErr");if(!Directory.Exists(value[..3]))return Fail("Lang_FilePathErr");
    try{Directory.CreateDirectory(value);}catch(Exception error)when(error is IOException or UnauthorizedAccessException or ArgumentException){return Fail("Lang_FilePathCharErr");}
    try{var path=LocalFolderSelection.Prepare(value);var result=parameters.Dispatch(request with{Value=path});NotifyFolder(result.Error is null?"Lang_DrOpenSuc":"Lang_DrOpenFail",result.Error!=null);return result;}catch(Exception error)when(error is IOException or UnauthorizedAccessException or ArgumentException){return Fail("Lang_FileRdOnly");}
   }
   if(request.Kind==ConfigurationOperationKind.Run){_ = execution.RunOnceAsync();return new(true);}
   if(request.Kind==ConfigurationOperationKind.RunContinuous){var result=execution.StartContinuous();return result==ConfigurationRunRejection.None?new(true):new(true,Error:result.ToString());}
   if(request.Kind==ConfigurationOperationKind.StopContinuous){_ = execution.StopAsync();return new(true);}
   if(request.Kind==ConfigurationOperationKind.Close){session?.RequestClose();return new(true);}
   return parameters.Dispatch(request);
  });
  state=new(definition,dispatcher){ExecutionEnabled=Host().RunEnabled};
  state.RegisterDisplayTypeResolver("EnumerationCameraForImage",_=>"EnumerationCamera");
  foreach(var name in new[]{"GeneralParamListVisibleOperation","GeneralParamListHiddenOperation"})state.RegisterRule(name,new ImageSourceRule((s,r)=>{
   var mode=int.TryParse(r.Source.Value,out var id)?id:1;
   var request=new ConfigurationPresentationRequest(context.Node.Id,ConfigurationPresentationKind.ImageSourceMode,(ImageSourcePresentationMode)mode);
   var result=context.ConfigurationPresentation?.Apply(request)??new(ConfigurationPresentationStatus.Unsupported,"Host image-list presentation service is unavailable.");
   if(result.Status!=ConfigurationPresentationStatus.Supported)s.ReportDiagnostic(new("PresentationService",r.OriginalName,r.Source.Name,result.Diagnostic??result.Status.ToString()));
  }));
  state.RegisterRule("GetGlobalModuleListOperation",new ImageSourceRule((s,r)=>{if(context.Cameras is null)s.ReportDiagnostic(new("CameraService",r.OriginalName,r.Source.Name,"Global camera module list service is unavailable."));}));
  state.RegisterRule("UpdateModuleStateOperation",new ImageSourceRule((s,r)=>{if(r.Source.Visible&&r.Source.Value=="-1")s.ReportDiagnostic(new("ModuleSelectionReadiness",r.OriginalName,r.Source.Name,"Original SDK dynamic-I/O/CheckItemParamReady bridge is not implemented; graph readiness is independent."));}));
  // In inspected managed source frontFilter is never assigned; no forced input mutation is invented.
  state.RegisterRule("SetFilterForcePropertyOperation",new ImageSourceRule((_,_)=>{}));
  var controls=new ToolConfigurationControlRegistry();ToolConfigurationStandardControls.Register(controls,text,()=>new(execution.IsContinuous,Host().ProcedureBusy,Host().RunEnabled),language,context.ConfigurationSubscriptions);
  ToolConfigurationSubscriptionControls.Register(controls,context.ConfigurationSubscriptions,text,language);
  controls.Register("ButtonSelecter_ImageList",c=>{var empty=new W.Border{Height=0};ToolConfigurationView.SetUseItemMargin(empty,false);return empty;});
  controls.Register("BooleanTips",c=>{controls.TryCreate("Boolean",c,out var row);var grid=(W.Grid)row!;var tip=new W.TextBlock{Text=text(c.Element.Children.FirstOrDefault(e=>e.Tag=="SelecetedTips")?.Text??""),FontSize=12,Margin=new Thickness(50,0,0,0),Foreground=Brushes.DarkOrange,VerticalAlignment=System.Windows.VerticalAlignment.Center,IsHitTestVisible=false};W.Grid.SetColumn(tip,1);grid.Children.Add(tip);c.OnRefresh(()=>tip.Visibility=c.Node.Value=="True"?Visibility.Visible:Visibility.Collapsed);return grid;});
  controls.Register("EnumerationCamera",c=>CameraSelector(c,context,text));
  controls.Register("OpenFileDialog",c=>{owner=c.Owner;return FolderSelector(c,context,text);});
  var list=new ImageSourceConfigurationList(this,context,state,()=>values,Apply,text,execution);
  controls.Register("LocalImageList",c=>{c.OnRefresh(list.Refresh);ToolConfigurationView.SetUseItemMargin(list,false);return list;});
  if(context.Mode==ToolEditorMode.SdkEmbedded&&state.Find("Lang_LocalImgList") is {} externList)state.SetProgrammatic(externList,"CustomVisible","True");
  long revision=0;void Changed(){state.IsBusy=execution.IsRunning;if(state.Find("ImageSourceType") is {} node)state.SetProgrammatic(node,"ExecutionRevision",(++revision).ToString(CultureInfo.InvariantCulture));}
  execution.Changed+=Changed;session=new(state,controls,language,text,()=>{execution.Changed-=Changed;list.Dispose();_ = execution.DisposeAsync();});state.Initialize();return session;
 }
 sealed class ImageSourceDispatcher(Func<ConfigurationOperationRequest,ConfigurationOperationResult> invoke):IToolConfigurationDispatcher{public ConfigurationOperationResult Dispatch(ConfigurationOperationRequest request)=>invoke(request);}
 sealed class ImageSourceRule(Action<ToolConfigurationState,ConfigurationOperationRequest> apply):IToolConfigurationRule{public void Apply(ToolConfigurationState state,ConfigurationOperationRequest request)=>apply(state,request);public void Revert(ToolConfigurationState state,ConfigurationOperationRequest request){}}
 static void ConfigurationResources(FrameworkElement target){target.Resources.MergedDictionaries.Add(new ResourceDictionary{Source=new Uri("/VisionPluginContracts;component/Resources/ConfigurationControls.xaml",UriKind.Relative)});target.Resources.MergedDictionaries.Add(new ResourceDictionary{Source=new Uri("/VisionPlugins.ImageSource;component/Resources/ImageSourceConfigurationResources.xaml",UriKind.Relative)});}
 static FrameworkElement CameraSelector(ToolConfigurationControlContext c,ToolEditorContext context,Func<string,string> text){
  var row=new W.Grid{Height=32};ConfigurationResources(row);row.ColumnDefinitions.Add(new(){Width=new GridLength(89)});row.ColumnDefinitions.Add(new());row.ColumnDefinitions.Add(new(){Width=GridLength.Auto});
  var label=new W.TextBlock{Text=text(c.Node.CaptionKey),FontSize=14,FontWeight=FontWeights.Normal,Foreground=new SolidColorBrush(Color.FromRgb(102,102,102)),VerticalAlignment=System.Windows.VerticalAlignment.Center};row.Children.Add(label);
  var combo=new W.ComboBox{Style=(Style)row.FindResource("ConfigurationEnumeration"),Name="ImageSourceCamera",DisplayMemberPath="Name",SelectedValuePath="Id",FontWeight=FontWeights.Normal};W.Grid.SetColumn(combo,1);row.Children.Add(combo);bool updating=false;
  void Refresh(){updating=true;var cameras=new[]{new CameraInfo(-1,text("Lang_PlzChoose"))}.Concat((context.Cameras?.Invoke()??[]).Select(camera=>camera with{Name=camera.Id+" "+camera.Name})).ToArray();combo.ItemsSource=cameras;combo.SelectedValue=int.TryParse(c.Node.Value,out var id)?id:-1;updating=false;}
  combo.SelectionChanged+=(_,_)=>{if(!updating&&combo.SelectedValue is int id)c.State.CommitUser(c.Node,id.ToString(CultureInfo.InvariantCulture));};
  var button=new W.Button{Style=(Style)row.FindResource("ImageSourceCameraButton"),Tag=new W.Image{Source=new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/VisionPlugins.ImageSource;component/Resources/ic_modal_camera_click.png")),Stretch=Stretch.None},Name="ConfigureImageSourceCamera",Content=new W.Image{Source=new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/VisionPlugins.ImageSource;component/Resources/ic_modal_camera_normal.png")),Stretch=Stretch.None},Background=Brushes.Transparent,BorderThickness=new Thickness(0),IsEnabled=context.ConfigureCameras!=null};W.Grid.SetColumn(button,2);row.Children.Add(button);button.Click+=(_,_)=>{context.ConfigureCameras?.Invoke();Refresh();};c.OnRefresh(()=>{if(!combo.IsDropDownOpen)Refresh();});return row;
 }
 static FrameworkElement FolderSelector(ToolConfigurationControlContext c,ToolEditorContext context,Func<string,string> text){
  var row=new W.Grid{Height=34};ConfigurationResources(row);row.ColumnDefinitions.Add(new(){Width=new GridLength(89)});row.ColumnDefinitions.Add(new());
  row.Children.Add(new W.TextBlock{Text=text(c.Node.CaptionKey),FontSize=14,FontWeight=FontWeights.Normal,Foreground=new SolidColorBrush(Color.FromRgb(102,102,102)),VerticalAlignment=System.Windows.VerticalAlignment.Center});
  var right=new W.Grid();right.ColumnDefinitions.Add(new());right.ColumnDefinitions.Add(new(){Width=GridLength.Auto});W.Grid.SetColumn(right,1);row.Children.Add(right);
  var editor=new W.TextBox{Style=(Style)row.FindResource("ImageSourcePathText"),Name="ImageSourceFolder",FontWeight=FontWeights.Normal};right.Children.Add(editor);
  var outline=new W.Border{BorderThickness=new Thickness(1),CornerRadius=new CornerRadius(4),BorderBrush=new SolidColorBrush(Color.FromRgb(204,204,205)),IsHitTestVisible=false};W.Grid.SetColumnSpan(outline,2);right.Children.Add(outline);
  void Commit(string path){c.State.CommitUser(c.Node,path);editor.Text=c.Node.Value;}
  editor.LostKeyboardFocus+=(_,_)=>{if(editor.Text!=c.Node.Value)Commit(editor.Text);};
  var choose=new W.Button{Style=(Style)row.FindResource("ImageSourcePathButton"),Name="ChooseImageSourceFolder"};W.Grid.SetColumn(choose,1);right.Children.Add(choose);
  choose.Click+=(_,_)=>{var result=(context.ConfigurationFolders??new WpfConfigurationFolderService()).Select(c.OwnerWindow,new(""));if(result.Status==ConfigurationFileStatus.Selected&&result.Path is {} path)Commit(path.Trim());else if(result.Status is ConfigurationFileStatus.Failed or ConfigurationFileStatus.Unsupported)c.State.ReportDiagnostic(new("FolderService","Select",c.Node.Name,result.Diagnostic??result.Status.ToString()));};c.OnRefresh(()=>{if(!editor.IsKeyboardFocusWithin)editor.Text=c.Node.Value;editor.ToolTip=c.State.Diagnostics.LastOrDefault(d=>d.Node==c.Node.Name)?.Message??c.Node.Value;});return row;
 }
}
