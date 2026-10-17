using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VisionFramework;
using VisionPluginContracts;

namespace VisionEditor;
public partial class MainWindow {
 readonly ToolDisplayImagePresenter toolDisplayImage=new(){Visibility=Visibility.Collapsed};
 readonly ToolDisplayRecordPresenter toolDisplayRecord=new(){Visibility=Visibility.Collapsed};
 readonly ToolDisplayResultListPresenter toolDisplayList=new(){Visibility=Visibility.Collapsed};
 readonly TabItem toolRecordTab=new(){Header="记录",Visibility=Visibility.Collapsed};
 readonly TabItem toolListTab=new(){Header="结果列表",Visibility=Visibility.Collapsed};
 void InitializeToolDisplays(){
  ((Grid)Preview.Parent).Children.Add(toolDisplayImage);
  var pane=(Grid)ImagePreviewPane.Parent;
  Grid.SetRow(toolDisplayRecord,2);Grid.SetRow(toolDisplayList,2);
  pane.Children.Add(toolDisplayRecord);pane.Children.Add(toolDisplayList);
  DisplayTabs.Items.Add(toolRecordTab);DisplayTabs.Items.Add(toolListTab);
 }
 void RefreshToolDisplay(NodeDocument? node){
  var provider=node is null?null:plugins.Find(node.ModuleId) as IToolDisplayProvider;
  ToolDisplayFrame? frame=provider is not null&&node is not null&&results.ContainsKey(node.Id)?provider.LastDisplay(node,doc.BaseDirectory,executionScope.ForProcedure(doc.CurrentProcedureId)):null;
  if(frame is not null&&node is not null)frame=ToolConfigurationDisplaySubscriptions.ApplyPositions(frame,node,doc.CurrentProcedureId,(source,port)=>results.GetValueOrDefault(source)?.Outputs.GetValueOrDefault(port));
  toolRecordTab.Visibility=provider is null?Visibility.Collapsed:Visibility.Visible;
  toolListTab.Visibility=provider is null?Visibility.Collapsed:Visibility.Visible;
  if(provider is null&&DisplayTabs.SelectedIndex>1)DisplayTabs.SelectedIndex=0;
  toolDisplayImage.Show(frame);toolDisplayRecord.Show(frame);toolDisplayList.Show(frame);
  bool overlay=frame is not null&&DisplayTabs.SelectedIndex==0;
  toolDisplayImage.Visibility=overlay?Visibility.Visible:Visibility.Collapsed;
  Preview.Visibility=overlay?Visibility.Collapsed:Visibility.Visible;
  PreviewImageName.Visibility=overlay?Visibility.Collapsed:Visibility.Visible;
  if(overlay)EmptyImage.Visibility=Visibility.Collapsed;
  ImagePreviewPane.Visibility=DisplayTabs.SelectedIndex==0?Visibility.Visible:Visibility.Collapsed;
  ModuleResultsGrid.Visibility=DisplayTabs.SelectedIndex==1?Visibility.Visible:Visibility.Collapsed;
  toolDisplayRecord.Visibility=provider is not null&&DisplayTabs.SelectedIndex==2?Visibility.Visible:Visibility.Collapsed;
  toolDisplayList.Visibility=provider is not null&&DisplayTabs.SelectedIndex==3?Visibility.Visible:Visibility.Collapsed;
  toolDisplayImage.LayoutTransform=new ScaleTransform(previewScale,previewScale);
 }
}
