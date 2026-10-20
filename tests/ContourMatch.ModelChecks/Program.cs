using System.Text;
using VisionPlugins.ContourMatch;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using VisionFramework;
using VisionPluginContracts;

// Prepared against production members at next authorized source integration.
// No candidate source links/shadow assembly and no vendor target execution.
static class Program
{
    static int count;
    static void Check(bool value,string message){if(!value)throw new Exception(message);count++;}
    static byte[] B(string value)=>Encoding.ASCII.GetBytes(value);
    [STAThread] static int Main()
    {
        var store=new ContourModelStorage();store.SetPublicName(B("one"));var input=new byte[]{1,2};
        Check(store.Add(input)==0&&store.Count==1,"add count");input[0]=99;
        Check(store.Snapshot()[0].Value.Data[0]==1&&store.Snapshot()[0].Value.Selected,"owned selected buffer");
        store.SetPublicName(B("two"));Check(store.Add([3])==0,"second add");
        Check(store.Select(1,0)==0&&!store.Snapshot()[1].Value.Selected,"deselect");
        Check(store.Select(2,1)==0xe0000001&&store.Select(0,2)==0xe0000001,"selection bounds");
        Check(store.Move(1,true)==0&&store.Snapshot()[0].Value.Name[0]=='t'&&!store.Snapshot()[0].Value.Selected,"swap entire record");
        Check(store.Delete(0)==0&&store.Count==1&&store.Snapshot()[0].Value.Name[0]=='o',"delete shifts");
        Check(store.Rename(0,B("renamed"))==0&&store.Snapshot()[0].Value.Name[0]=='r',"rename");
        var copy=store.Snapshot();copy[0].Value.Data[0]=88;Check(store.Snapshot()[0].Value.Data[0]==1,"snapshot cannot mutate owner");
        store.Clear();Check(store.Count==0&&store.Snapshot().Count==0,"clear");
        store.Add([7]);Check(store.Snapshot()[0].Value.Name[0]=='t',"clear retains pending name");
        bool rejected=false;try{store.Delete(99);}catch(NotSupportedException){rejected=true;}Check(rejected,"empty deletion explicit safety boundary");
        store.Clear();for(int i=0;i<100;i++)Check(store.Add([1])==0,"capacity add");
        Check(store.Add([1])==0xe0000326&&store.Count==100,"full capacity");
        Check(ContourModelDecoder.Decode(B("TQ==")).SequenceEqual(new byte[]{77}),"one-byte padding");
        Check(ContourModelDecoder.Decode(B("TWE=")).SequenceEqual(new byte[]{77,97}),"two-byte padding");
        Check(ContourModelDecoder.Decode(B("TWFu")).SequenceEqual(B("Man")),"three bytes");
        Check(ContourModelDecoder.Decode(B("TQ")).Length==0,"incomplete group ignored");
        Check(ContourModelDecoder.Decode(B("!!!!")).SequenceEqual(new byte[]{255,255,255}),"invalid chars preserve source ff mapping");
        Check(ContourModelDecoder.Decode(B("\nTQ==")).SequenceEqual(new byte[]{253,52}),"decoder does not trim newline");
        var models=new ContourModelCollection();int persists=0;
        var first=new Dictionary<string,byte[]?>{["_BasicData"]=[11],["_PointsNums"]=[91]};
        models.Add(B("a"),[1],first,()=>persists++);
        models.Add(B("b"),[2],new Dictionary<string,byte[]?>{["_BasicData"]=[22],["_PointsNums"]=[92]},()=>persists++);
        models.Move(1,true,()=>persists++);var user=models.UserDataSnapshot();
        Check(user["SolModelFile_0_BasicData"]![0]==22&&user["SolModelFile_0_PointsNums"]![0]==91,"source swaps five keys; points count remains");
        models.Delete(0,()=>persists++);user=models.UserDataSnapshot();
        Check(user["SolModelFile_0_BasicData"]![0]==11&&user["SolModelFile_1_BasicData"] is null,"delete sidecars shift");
        models.Clear(()=>persists++);user=models.UserDataSnapshot();
        Check(user["SolModelFile_0_BasicData"] is null&&user["SolModelFile_0_PointsNums"]![0]==91,"clear source sidecar set");
        try{models.Add(B("c"),[3],first,()=>throw new InvalidOperationException("persist failure"));}catch(InvalidOperationException){}
        Check(models.Storage.Count==0&&models.UserDataSnapshot()["SolModelFile_0_BasicData"]![0]==11,"add failure removes candidate but preserves partial userdata");
        var restored=ContourModelCollection.Load(models.Save());Check(restored.Storage.Count==0&&restored.UserDataSnapshot()["SolModelFile_0_BasicData"]![0]==11,"independent solution transport retains partial userdata");
        models.Add(B("UI model"),[5],first,()=>{});models.Storage.Select(0,0);
        restored=ContourModelCollection.Load(models.Save());Check(!restored.Storage.Snapshot()[0].Value.Selected&&restored.Storage.Snapshot()[0].Value.Data[0]==5,"owned model JSON roundtrip");
        ConfigurationUi(models);
        Console.WriteLine($"PASS {count} contour source model-storage/decoder/configuration checks");return 0;
    }
    static IEnumerable<T> Visuals<T>(DependencyObject parent)where T:DependencyObject{
        for(int i=0;i<VisualTreeHelper.GetChildrenCount(parent);i++){var child=VisualTreeHelper.GetChild(parent,i);if(child is T value)yield return value;foreach(var nested in Visuals<T>(child))yield return nested;}
    }
    sealed class Notices:IToolConfigurationNotificationService{
        public readonly List<ConfigurationNotification> Items=[];
        public ConfigurationNotificationResult Notify(Window? owner,ConfigurationNotification value){Items.Add(value);return new(ConfigurationNotificationStatus.Displayed);}
    }
    sealed class Confirmations:IToolConfigurationConfirmationService{public bool Confirm(Window? owner,ConfigurationConfirmation request)=>true;}
    static void ConfigurationUi(ContourModelCollection models){
        _=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};
        var plugin=new ContourMatchPlugin();var values=plugin.Descriptor.Parameters.Where(p=>p.DefaultValue.HasValue).ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());
        values[ContourModelCollection.ParameterName]=JsonSerializer.SerializeToElement(models.Save());var notices=new Notices();
        var context=new ToolEditorContext(new("contour",ContourMatchPlugin.Id,"1","Contour",values),Environment.CurrentDirectory,next=>values=next,_=>throw new Exception("No matcher run expected")){
            ReadEffectiveParameters=()=>values,ConfigurationNotifications=notices,ConfigurationConfirmations=new Confirmations()
        };
        using var session=plugin.CreateConfiguration(context);using var view=new ToolConfigurationView(session.State,session.Controls);
        var window=new Window{Content=view,Width=700,Height=700,ShowInTaskbar=false};window.Show();
        try{
            // Original second tab owns HPPyramidMatchPat.
            view.Tabs.SelectedIndex=1;window.UpdateLayout();Application.Current.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);
            var pane=session.State.Find("HPPyramidMatchPat")!;Check(pane.Template.TemplateKey=="FeaturepatMatchConfigPanalData","original runtime template key resolved");
            Check(!view.Diagnostics.Any(d=>d.Node=="HPPyramidMatchPat"),"actual model control registered");
            var checkbox=Visuals<CheckBox>(view).Single(c=>c.Name=="select");Check(checkbox.IsChecked==false,"persisted model selected flag shown");
            checkbox.IsChecked=true;checkbox.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
            Check(ContourModelCollection.Load(values[ContourModelCollection.ParameterName].GetString()!).Storage.Snapshot()[0].Value.Selected,"real selection click persists");
            var build=Visuals<Button>(view).Single(b=>b.Name=="Build");build.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
            Check(notices.Items.Last().Message.Contains("not implemented"),"training button explicit unsupported, no fake model");
            var clear=Visuals<Button>(view).Single(b=>b.Name=="btnDeleteAllModels");clear.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
            Check(ContourModelCollection.Load(values[ContourModelCollection.ParameterName].GetString()!).Storage.Count==0,"confirmed actual clear click persists");
        }finally{window.Close();}
    }
}
