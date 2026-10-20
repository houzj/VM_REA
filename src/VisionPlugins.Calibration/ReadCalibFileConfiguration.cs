using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VisionPluginContracts;
using VisionFramework;
using Button=System.Windows.Controls.Button;
using TextBox=System.Windows.Controls.TextBox;
using Image=System.Windows.Controls.Image;
using Brushes=System.Windows.Media.Brushes;
using HorizontalAlignment=System.Windows.HorizontalAlignment;

namespace VisionPlugins.Calibration;

public sealed partial class ReadCalibFilePlugin : IToolConfigurationProvider
{
 public ToolConfigurationSession CreateConfiguration(ToolEditorContext context)
 {
  var language="Chinese";var text=ConfigurationLocalization.ForLanguage(language).Get;
  string Resource(string name){using var stream=typeof(ReadCalibFilePlugin).Assembly.GetManifestResourceStream("VisionPlugins.Calibration.Resources."+name)??throw new InvalidDataException("Missing Read configuration resource: "+name);using var reader=new StreamReader(stream);return reader.ReadToEnd();}
  var definition=ToolConfigurationDefinition.Parse(Resource("ReadCalibFileAlgorithmTab.xml"),Resource("ConfigurationGroups.xml"));
  var values=context.Node.Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());
  foreach(var parameter in Descriptor.Parameters)if(!values.ContainsKey(parameter.Name)&&parameter.DefaultValue is {} value)values[parameter.Name]=value.Clone();
  ToolEditorContext Current()=>context with{Node=context.Node with{Parameters=values}};
  void ApplyValues(Dictionary<string,JsonElement> next){context.Apply(next.ToDictionary(p=>p.Key,p=>p.Value.Clone()));values=next;}
  ToolConfigurationState? state=null;ToolConfigurationSession? session=null;
  var files=context.ConfigurationFiles??new WpfConfigurationFileService();
  var notifications=context.ConfigurationNotifications??new WpfConfigurationNotificationService();
  Func<Window?> owner=()=>null;
  void Notify(ConfigurationNotificationKind kind,string message,string caption=""){var result=notifications.Notify(owner(),new(kind,message,caption,context.Mode));if(result.Status==ConfigurationNotificationStatus.Failed&&state?.Find("FilePath") is {} file)state.ReportDiagnostic(new("NotificationService","Notify",file.Name,result.Diagnostic??"Notification service failed."));}
  ConfigurationExecutionHostStatus Host()=>context.ConfigurationExecutionStatus?.Invoke()??new(false,false,false,false,false);
  var execution=new ToolConfigurationExecution(()=>context.Run(values.ToDictionary(p=>p.Key,p=>p.Value.Clone())),()=>Host().SolutionBusy,()=>Host().Ready&&Host().RunEnabled,()=>Host().Loading,()=>Host().ProcedureBusy);
  Dictionary<string,string> ReadUser(){try{return JsonSerializer.Deserialize<Dictionary<string,string>>(values["ConfigurationUserData"].GetString()??"{}")??[];}catch(JsonException error){throw new InvalidDataException("Invalid configuration user-data store.",error);}}
  var parameters=new ToolConfigurationParameterAdapter(Descriptor,()=>values,ApplyValues,key=>ReadUser().GetValueOrDefault(key),(key,value)=>{var user=ReadUser();user[key]=value;var next=values.ToDictionary(p=>p.Key,p=>p.Value.Clone());next["ConfigurationUserData"]=JsonSerializer.SerializeToElement(JsonSerializer.Serialize(user));ApplyValues(next);});
  string InputDescription()=>Snapshot(Current()).Data.InputDescription();
  void RefreshData(){if(state?.Find("CalibInputParam") is {} panel){var raw=GetState(context.Node.Id,context.BaseDirectory,context.Scope);lock(raw.Gate)state.SetProgrammatic(panel,"CurValue",raw.Data.InputDescription());}}
  ConfigurationOperationResult ReadBinding(ConfigurationOperationRequest request,bool write)
  {
   var query=ConfigurationSubscriptionValidation.FromElement(request.Source.Element);
   if(query is null)return new(true,Error:"Unsupported source-declared subscription query.");
   if(context.ConfigurationSubscriptions is null)return new(true,Error:"Subscription service is unavailable.");
   if(write&&request.SubscriptionBinding is null&&!string.IsNullOrEmpty(request.Value))return new(true,Error:"A typed subscription reference is required.");
   var result=write?context.ConfigurationSubscriptions.Bind(query.Destination,request.SubscriptionBinding):context.ConfigurationSubscriptions.Read(query.Destination);
   if(result.Status!=ConfigurationSubscriptionStatus.Available)return new(true,Error:result.Diagnostic??result.Status.ToString());
   if(state is not null&&!state.ApplySubscription(request.Source,result))return new(true,Error:"Subscription service did not provide authoritative presentation.");
   return new(true,result.DisplayText??"");
  }
  var dispatch=new ReadConfigurationDispatcher(request=>
  {
   if(request.Kind==ConfigurationOperationKind.ReadInputBinding)return ReadBinding(request,false);
   if(request.Kind==ConfigurationOperationKind.WriteInputBinding)return ReadBinding(request,true);
   if(request.Kind==ConfigurationOperationKind.ReadParameter&&request.Source.Name=="CalibInputParam")return new(true,InputDescription());
   if(request.Kind==ConfigurationOperationKind.WriteParameter&&request.Source.Name=="FilePath")
   {
    var raw=GetState(context.Node.Id,context.BaseDirectory,context.Scope);
    (bool Initialized,string Requested,string Path,string Cache,ReadCalibFileData Data,uint Error) previous;
    lock(raw.Gate)previous=(raw.Initialized,raw.Requested,raw.Path,raw.Cache,raw.Data,raw.LoadError);
    var path=request.Value??"";var loaded=Load(Current(),path);
    if(loaded.ErrorCode!=0){
     // Keep native partial data/path commits; the failed picker value is presentation only.
     var partial=values.ToDictionary(p=>p.Key,p=>p.Value.Clone());
     if(loaded.CommitPath){partial["FilePath"]=JsonSerializer.SerializeToElement(path);partial["CachedFileData"]=JsonSerializer.SerializeToElement("");}
     if(loaded.ReplaceData&&loaded.Data is {} data)partial["OutputSchema"]=JsonSerializer.SerializeToElement(Schema(data));
     try{if(partial.Any(p=>!values.TryGetValue(p.Key,out var oldValue)||oldValue.GetRawText()!=p.Value.GetRawText()))ApplyValues(partial);}
     catch(Exception error)when(error is InvalidOperationException or IOException or UnauthorizedAccessException or ArgumentException){
      // Independent host transaction refusal: restore the prior runtime snapshot.
      lock(raw.Gate)(raw.Initialized,raw.Requested,raw.Path,raw.Cache,raw.Data,raw.LoadError)=previous;
      return new(true,Error:error.Message);
     }
     lock(raw.Gate)raw.Requested=values["FilePath"].GetString()??"";
     RefreshData();var message=$"LoadFile returned 0x{loaded.ErrorCode:x8}";Notify(ConfigurationNotificationKind.Error,text("Lang_SysErr")+":"+message,text("Lang_FileOpenFail"));return new(true,Error:message){KeepAttemptedValueOnError=true};
    }
    var next=values.ToDictionary(p=>p.Key,p=>p.Value.Clone());next["FilePath"]=JsonSerializer.SerializeToElement(path);next["CachedFileData"]=JsonSerializer.SerializeToElement("");next["OutputSchema"]=JsonSerializer.SerializeToElement(Schema(loaded.Data!));
    try{ApplyValues(next);}catch(Exception error)when(error is InvalidOperationException or IOException or UnauthorizedAccessException or ArgumentException){
     // Independent document-transaction boundary: a refused host write must
     // not leave the runtime using an uncommitted file/schema.
     lock(raw.Gate)(raw.Initialized,raw.Requested,raw.Path,raw.Cache,raw.Data,raw.LoadError)=previous;
     return new(true,Error:error.Message);
    }
    RefreshData();Notify(ConfigurationNotificationKind.Success,text("Lang_SetSuc"));return new(true,path);
   }
   if(request.Kind==ConfigurationOperationKind.Run){if(!Host().Ready||!Host().RunEnabled||Host().Loading||Host().SolutionBusy)return new(true,Error:"Configuration execution host is not ready or is busy.");_ = execution.RunOnceAsync();return new(true);}
   if(request.Kind==ConfigurationOperationKind.RunContinuous){var rejected=execution.StartContinuous();return rejected==ConfigurationRunRejection.None?new(true):new(true,Error:rejected.ToString());}
   if(request.Kind==ConfigurationOperationKind.StopContinuous){_ = execution.StopAsync();return new(true);}
   if(request.Kind==ConfigurationOperationKind.Close){session?.RequestClose();return new(true);}
   return parameters.Dispatch(request);
  });
  state=new(definition,dispatch){ExecutionEnabled=Host().RunEnabled};
  var controls=new ToolConfigurationControlRegistry();
  ToolConfigurationStandardControls.Register(controls,text,()=>new(execution.IsContinuous,Host().ProcedureBusy,Host().RunEnabled),language);
  ToolConfigurationSubscriptionControls.Register(controls,context.ConfigurationSubscriptions,text,language);
  controls.Register("OpenFileDialog4ReadFile",c=>{owner=c.Owner;return ReadFilePicker(c,text,files,notifications,context.Mode,context.IsRemoteMode);});
  controls.Register("ReadFileConfigData",c=>new ReadCalibFileConfigurationPanel(c,text));
  long revision=0;
  void ExecutionChanged(){state.IsBusy=execution.IsRunning;RefreshData();if(state.Find("FilePath") is {} file)state.SetProgrammatic(file,"ExecutionRevision",(++revision).ToString(CultureInfo.InvariantCulture));}
  execution.Changed+=ExecutionChanged;
  session=new(state,controls,language,text,()=>{execution.Changed-=ExecutionChanged;_ = execution.DisposeAsync();});
  state.Initialize();return session;
 }
 sealed class ReadConfigurationDispatcher(Func<ConfigurationOperationRequest,ConfigurationOperationResult> run):IToolConfigurationDispatcher
 {public ConfigurationOperationResult Dispatch(ConfigurationOperationRequest request)=>run(request);}
 static FrameworkElement ReadFilePicker(ToolConfigurationControlContext c,Func<string,string> text,IToolConfigurationFileService files,IToolConfigurationNotificationService notifications,ToolEditorMode mode,bool isRemoteMode)
 {
  var row=new Grid{Height=34};System.Windows.Documents.TextElement.SetFontWeight(row,FontWeights.Normal);row.ColumnDefinitions.Add(new(){Width=new GridLength(89)});row.ColumnDefinitions.Add(new(){Width=new GridLength(1,GridUnitType.Star)});
  var label=new TextBlock{FontSize=14,VerticalAlignment=VerticalAlignment.Center,TextTrimming=TextTrimming.CharacterEllipsis,Foreground=new SolidColorBrush(System.Windows.Media.Color.FromRgb(102,102,102))};row.Children.Add(label);
  var editor=new Grid();editor.ColumnDefinitions.Add(new(){Width=new GridLength(1,GridUnitType.Star)});editor.ColumnDefinitions.Add(new(){Width=GridLength.Auto});Grid.SetColumn(editor,1);row.Children.Add(editor);
  var path=new TextBox{Name="ReadCalibFilePath",Background=Brushes.Transparent,BorderThickness=new Thickness(0),VerticalAlignment=VerticalAlignment.Center,FontSize=14,FontWeight=FontWeights.Normal,Foreground=new SolidColorBrush(System.Windows.Media.Color.FromRgb(128,128,128))};editor.Children.Add(path);
  var icon=new Image{Stretch=Stretch.None};
  BitmapImage Icon(string state)=>new(new Uri("pack://application:,,,/VisionPlugins.Calibration;component/Resources/ReadCalibFilePath_"+state+".png"));
  icon.Source=Icon("normal");var choose=new Button{Name="ChooseReadCalibFile",Content=icon,Background=Brushes.Transparent,BorderThickness=new Thickness(0),Margin=new Thickness(0,-1,0,1)};Grid.SetColumn(choose,1);editor.Children.Add(choose);
  choose.MouseEnter+=(_,_)=>icon.Source=Icon("hover");choose.MouseLeave+=(_,_)=>icon.Source=Icon("normal");choose.PreviewMouseLeftButtonDown+=(_,_)=>icon.Source=Icon("click");
  var border=new Border{BorderThickness=new Thickness(1),CornerRadius=new CornerRadius(4),BorderBrush=new SolidColorBrush(System.Windows.Media.Color.FromRgb(204,204,205)),IsHitTestVisible=false};Grid.SetColumnSpan(border,2);editor.Children.Add(border);
  void Commit(){if(path.Text!=c.Node.Value)c.State.CommitUser(c.Node,path.Text);path.Text=c.Node.Value;}
  path.LostKeyboardFocus+=(_,_)=>Commit();
  choose.Click+=(_,_)=>{
   var result=files.Select(c.OwnerWindow,new(Id,c.Node.Value,text("%.xml|*.xml%"),text("Open"),false,isRemoteMode));
   void Error(string diagnostic){c.State.ReportDiagnostic(new("FileService","Select/Remember",c.Node.Name,diagnostic));choose.ToolTip=diagnostic;notifications.Notify(c.OwnerWindow,new(ConfigurationNotificationKind.Error,text("Lang_SysErr")+":"+diagnostic,text("Lang_FileOpenFail"),mode));}
   if(result.Status==ConfigurationFileStatus.Selected){if(result.Paths.Count==0){Error("File service returned an empty selected-file list.");return;}var selected=result.Paths[0];path.Text=selected;c.State.CommitUser(c.Node,selected,forceWrite:true);path.Text=c.Node.Value;var saved=files.Remember(Id,selected);if(!saved.Success)Error(saved.Diagnostic??"Open-directory history could not be stored.");}
   else if(result.Status is ConfigurationFileStatus.Failed or ConfigurationFileStatus.Unsupported)Error(result.Diagnostic??result.Status.ToString());
  };
  c.OnRefresh(()=>{label.Text=text(c.Node.CaptionKey);label.ToolTip=text(c.Node.Get("Description")??c.Node.CaptionKey);if(!path.IsKeyboardFocusWithin)path.Text=c.Node.Value;path.ToolTip=c.State.Diagnostics.LastOrDefault(d=>d.Node==c.Node.Name&&d.Code is "NotificationService" or "FileService")?.Message??c.Node.Value;});return row;
 }
}

/// <summary>ReadFilePanel/ReadFileConfigData source-derived read-only leaf; it never owns a Window.</summary>
public sealed class ReadCalibFileConfigurationPanel:StackPanel
{
 readonly ToolConfigurationControlContext context;readonly Func<string,string> text;string? rendered;
 public ReadCalibFileConfigurationPanel(ToolConfigurationControlContext context,Func<string,string> text)
 {this.context=context;this.text=text;System.Windows.Documents.TextElement.SetFontWeight(this,FontWeights.Normal);ToolConfigurationView.SetUseItemMargin(this,false);context.OnRefresh(RefreshData);}
 void RefreshData()
 {
  var description=context.Node.Value;if(rendered==description||string.IsNullOrWhiteSpace(description))return;rendered=description;Children.Clear();
  foreach(var section in description.Split(';'))
  {
   var typed=section.Split('#');if(typed.Length!=2)continue;
   foreach(var record in typed[1].Split('$'))
   {
    var fields=record.Split(',');if(fields.Length<2)continue;
    if(typed[0] is "int" or "float" or "string")
    {
     if(fields.Length!=2)continue;
     if(typed[0]=="int"&&!int.TryParse(fields[1],NumberStyles.Integer,CultureInfo.InvariantCulture,out _))continue;
     if(typed[0]=="float"&&!float.TryParse(fields[1],NumberStyles.Float,CultureInfo.InvariantCulture,out _))continue;
     var grid=new Grid{Margin=new Thickness(30,5,30,5),HorizontalAlignment=HorizontalAlignment.Left};grid.ColumnDefinitions.Add(new());grid.ColumnDefinitions.Add(new(){Width=new GridLength(5)});grid.ColumnDefinitions.Add(new());
     AddText(grid,text(fields[0]),0);AddText(grid,":",1);AddText(grid,fields[1],2);Children.Add(grid);
    }
    else if(typed[0]=="CalibPointList")
    {
     Children.Add(new TextBlock{Text=text(fields[0]),FontSize=14,FontWeight=FontWeights.Normal,FontFamily=new("Microsoft YaHei"),Foreground=new SolidColorBrush(System.Windows.Media.Color.FromRgb(128,128,128)),Margin=new Thickness(30,5,0,0)});
     var table=new StackPanel{Margin=new Thickness(34,10,0,0)};
     foreach(var entry in fields.Skip(1)){var xyz=entry.Split('~');if(xyz.Length!=3||xyz.Any(s=>!float.TryParse(s,NumberStyles.Float,CultureInfo.InvariantCulture,out _)))continue;var row=new UniformGrid{Columns=3};foreach(var axis in xyz)row.Children.Add(new TextBox{Text=float.Parse(axis,CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture),ToolTip=axis,Height=30,IsReadOnly=true,FontWeight=FontWeights.Normal,VerticalContentAlignment=VerticalAlignment.Center,HorizontalContentAlignment=HorizontalAlignment.Center});table.Children.Add(row);}Children.Add(table);
    }
   }
  }
 }
 static void AddText(Grid row,string value,int column){var block=new TextBlock{Text=value,FontFamily=new("Microsoft YaHei"),FontSize=14,FontWeight=FontWeights.Normal,Foreground=new SolidColorBrush(System.Windows.Media.Color.FromRgb(128,128,128))};Grid.SetColumn(block,column);row.Children.Add(block);}
}
