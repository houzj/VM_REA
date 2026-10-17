using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using VisionEditor;
using VisionFramework;

static class NativeCompoundSelectorChecks
{
 public static void Run(MainWindow host,Action<bool,string> check)
 {
  int refreshes=0;var chosen=new List<WorkflowSourceOption>();
  var all=new[]{new WorkflowSourceOption("","",DataKind.Number,"解除订阅"),new WorkflowSourceOption("blob","Components.Area",DataKind.Number,"连通域.Components.Area"),new WorkflowSourceOption("blob","Components.CentroidPoint.CentroidX",DataKind.Number,"连通域.Components.CentroidPoint.CentroidX")};
  var selector=new NativeCompoundSourceSelector("------",()=>{refreshes++;return all;},chosen.Add);
  var window=new Window{Owner=host,Title="原生复合订阅控件操作复验",Width=480,Height=210,Content=new Border{Padding=new Thickness(24),Child=selector},ShowInTaskbar=false,WindowStartupLocation=WindowStartupLocation.CenterOwner};
  void Pump(){window.UpdateLayout();host.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);}
  void Open(){selector.LinkButton.IsChecked=true;Pump();}
  IEnumerable<TreeViewItem> Items(ItemsControl parent){foreach(var item in parent.Items.OfType<TreeViewItem>()){yield return item;foreach(var child in Items(item))yield return child;}}
  try{
   window.Show();Pump();check(refreshes==0&&selector.RelationText.IsReadOnly,"Native subscription selector does not fabricate index entry or refresh before link opens");
   Open();check(refreshes==1&&selector.SubscriptionPopup.IsOpen&&selector.LinkButton.ActualWidth==32&&selector.SubscriptionTree.ActualWidth==180&&selector.SubscriptionTree.ActualHeight==180,"Actual source-guided link opens refreshed 180x180 subscription popup with 32px icon");
   check(Items(selector.SubscriptionTree).Any(i=>Equals(i.Header,"CentroidPoint")&&i.Items.Count==1)&&!Items(selector.SubscriptionTree).Any(i=>i.Header is CheckBox),"Actual source subscription tree preserves compound branches without single-select checkboxes");
   var branch=Items(selector.SubscriptionTree).Single(i=>Equals(i.Header,"CentroidPoint"));branch.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice,0,MouseButton.Left){RoutedEvent=Control.MouseDoubleClickEvent});Pump();check(chosen.Count==0&&selector.SubscriptionPopup.IsOpen,"Compound branch double-click does not submit a subscription");
   var area=Items(selector.SubscriptionTree).Single(i=>i.Tag is WorkflowSourceOption o&&o.Port=="Components.Area");area.IsSelected=true;Pump();check(chosen.Count==0,"Selecting a leaf once does not prematurely commit native subscription");
   Capture((FrameworkElement)selector.SubscriptionPopup.Child,"wpf-native-compound-subscription-popup.png");
   area.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice,0,MouseButton.Left){RoutedEvent=Control.MouseDoubleClickEvent});Pump();check(chosen.Count==1&&chosen[0].Port=="Components.Area"&&!selector.SubscriptionPopup.IsOpen&&selector.LinkButton.IsChecked==false&&selector.RelationText.Text==all[1].Label,"Actual leaf double-click submits precise typed path once and closes popup");
   Open();selector.SubscriptionPopup.IsOpen=false;Pump();check(refreshes==2&&chosen.Count==1&&selector.LinkButton.IsChecked==false,"Reopening refreshes sources and dismissing popup does not change selected data");
   Open();var clear=Items(selector.SubscriptionTree).Single(i=>i.Tag is WorkflowSourceOption o&&o.NodeId.Length==0);clear.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice,0,MouseButton.Left){RoutedEvent=Control.MouseDoubleClickEvent});Pump();check(chosen.Count==2&&chosen[1].NodeId.Length==0&&!selector.SubscriptionPopup.IsOpen,"Actual null-source leaf invokes explicit unsubscribe callback");
   Open();window.Close();Pump();check(!selector.SubscriptionPopup.IsOpen,"Closing actual owner unloads source selector and closes its popup");
  }finally{selector.SubscriptionPopup.IsOpen=false;if(window.IsVisible)window.Close();}
 }
 static void Capture(FrameworkElement view,string name){view.UpdateLayout();var bitmap=new RenderTargetBitmap((int)Math.Ceiling(view.ActualWidth),(int)Math.Ceiling(view.ActualHeight),96,96,PixelFormats.Pbgra32);bitmap.Render(view);var png=new PngBitmapEncoder();png.Frames.Add(BitmapFrame.Create(bitmap));Directory.CreateDirectory("analysis/full-audit");using var file=File.Create(Path.Combine("analysis/full-audit",name));png.Save(file);}
}
