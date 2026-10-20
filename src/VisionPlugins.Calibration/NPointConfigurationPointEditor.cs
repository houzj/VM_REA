using Binding=System.Windows.Data.Binding;
using Color=System.Windows.Media.Color;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using W=System.Windows.Controls;
namespace VisionPlugins.Calibration;

/// <summary>Source-guided NPointList leaf. SDK writes are supplied by the configuration session.</summary>
public sealed class NPointConfigurationPointEditor:W.UserControl {
 public sealed class PointRow:INotifyPropertyChanged {
  internal readonly string[] Cells=new string[5]; public int ID{get;internal set;}
  public System.Windows.Input.ICommand? DeleteCommand{get;internal set;}
  internal Func<int,string,bool>? Write;
  public string ImageX{get=>Cells[0];set=>Set(0,value);} public string ImageY{get=>Cells[1];set=>Set(1,value);}
  public string WorldX{get=>Cells[2];set=>Set(2,value);} public string WorldY{get=>Cells[3];set=>Set(3,value);} public string Angle{get=>Cells[4];set=>Set(4,value);}
  void Set(int a,string value){if(Cells[a]==value)return;if(Write?.Invoke(a,value)!=true)return;Cells[a]=value;PropertyChanged?.Invoke(this,new(new[]{nameof(ImageX),nameof(ImageY),nameof(WorldX),nameof(WorldY),nameof(Angle)}[a]));}
  public event PropertyChangedEventHandler? PropertyChanged;
 }
 readonly Func<int,int,string,bool> commit;readonly Func<int,bool> delete;readonly Func<bool> clear;readonly Action<string,bool> notify;
 public ObservableCollection<PointRow> Rows{get;}=[];
 public W.DataGrid Table{get;}=new(){AutoGenerateColumns=false,CanUserAddRows=false,CanUserDeleteRows=false,CanUserSortColumns=false,CanUserReorderColumns=false,CanUserResizeRows=false,ColumnHeaderHeight=30,RowHeight=30,HeadersVisibility=W.DataGridHeadersVisibility.Column,GridLinesVisibility=W.DataGridGridLinesVisibility.Vertical,Margin=new(14,20,4,10),FontSize=14,BorderThickness=new(0),RowBackground=new SolidColorBrush(Color.FromRgb(235,235,235)),AlternatingRowBackground=new SolidColorBrush(Color.FromRgb(224,224,224)),Foreground=new SolidColorBrush(Color.FromRgb(132,132,132))};
 public NPointConfigurationPointEditor(float[,] snapshot,int count,Func<int,int,string,bool> commitCell,Func<int,bool> deletePoint,Func<bool> clear,Func<string?> chooseImport,Func<string?> chooseExport,Action<string,bool>? notify=null){
  FontFamily=new System.Windows.Media.FontFamily("Microsoft YaHei UI");FontSize=14;foreach(string dictionary in new[]{"Theme.xaml","RecScrollBarStyle.xaml"})Resources.MergedDictionaries.Add(new ResourceDictionary{Source=new Uri("/VisionPlugins.ImageSource;component/Resources/"+dictionary,UriKind.Relative)});Resources.MergedDictionaries.Add(new ResourceDictionary{Source=new Uri("/VisionPluginContracts;component/Resources/ConfigurationControls.xaml",UriKind.Relative)});
  if(count<0||count>32||snapshot.GetLength(0)<count||snapshot.GetLength(1)<5)throw new ArgumentOutOfRangeException(nameof(count));
  Table.Resources.MergedDictionaries.Add(new ResourceDictionary{Source=new Uri("/VisionPlugins.Calibration;component/Resources/NPointPointTable.xaml",UriKind.Relative)});Table.Style=(Style)Table.Resources[typeof(W.DataGrid)];commit=commitCell;delete=deletePoint;this.clear=clear;this.notify=notify??((_,_)=>{});for(int i=0;i<count;i++){var row=new PointRow{ID=i};Attach(row);for(int a=0;a<5;a++){row.Cells[a]="0";var formatted=snapshot[i,a].ToString("f3",CultureInfo.CurrentCulture);if(commit(i,a,formatted))row.Cells[a]=formatted;}Rows.Add(row);}
  Table.ItemsSource=Rows;
  foreach(var item in new[]{("ID","ID"),("图像坐标X","ImageX"),("图像坐标Y","ImageY"),("物理坐标X","WorldX"),("物理坐标Y","WorldY"),("角度","Angle")}){var edit=new Style(typeof(W.TextBox));edit.Setters.Add(new Setter(W.TextBox.MaxLengthProperty,item.Item2.StartsWith("World",StringComparison.Ordinal)?15:10));edit.Setters.Add(new Setter(W.Control.BorderThicknessProperty,new Thickness(0)));var display=new Style(typeof(W.TextBlock));display.Setters.Add(new Setter(W.TextBlock.TextAlignmentProperty,TextAlignment.Center));Table.Columns.Add(new W.DataGridTextColumn{Header=item.Item1,Binding=new Binding(item.Item2){UpdateSourceTrigger=UpdateSourceTrigger.LostFocus},IsReadOnly=item.Item2=="ID",EditingElementStyle=edit,ElementStyle=display,Width=W.DataGridLength.Auto});}
  var layout=new W.Grid();layout.RowDefinitions.Add(new());layout.RowDefinitions.Add(new(){Height=new GridLength(32)});var scroll=new W.ScrollViewer{Style=(Style)FindResource("RecScrollViewer"),Tag="1",VerticalScrollBarVisibility=W.ScrollBarVisibility.Auto,Content=Table};scroll.PreviewMouseWheel+=(_,e)=>{double amount=Math.Abs(e.Delta/2);double next=e.Delta<=0?Math.Min(scroll.ScrollableHeight,scroll.VerticalOffset+amount):Math.Max(0,scroll.VerticalOffset-amount);if(next!=scroll.VerticalOffset){scroll.ScrollToVerticalOffset(next);scroll.Focus();}e.Handled=true;};layout.Children.Add(scroll);var footer=new W.StackPanel{Orientation=W.Orientation.Horizontal,HorizontalAlignment=System.Windows.HorizontalAlignment.Right,Background=new SolidColorBrush(Color.FromRgb(228,228,231))};
  void Button(string caption,int width,Action action){var b=new W.Button{Content=caption,Width=width,Height=24,Style=(Style)FindResource("ConfigurationBottomButton"),Margin=new(0,0,10,0),VerticalAlignment=VerticalAlignment.Center};b.Click+=(_,_)=>{try{Table.CommitEdit();Table.CommitEdit();action();}catch(Exception ex){this.notify(ex.Message,true);}};footer.Children.Add(b);}
  Button("导出",66,()=>{if(chooseExport() is {} path){File.WriteAllText(path,ExportText());this.notify("保存成功",false);}});Button("导入",66,()=>{if(chooseImport() is {} path){ImportText(File.ReadAllText(path));this.notify("加载成功",false);}});Button("清空标定点",78,()=>ClearPoints());Button("确定",66,()=>Window.GetWindow(this)?.Close());var footerBackground=new W.Border{Background=new SolidColorBrush(Color.FromRgb(228,228,231)),Child=footer};W.Grid.SetRow(footerBackground,1);layout.Children.Add(footerBackground);Content=layout;
 }
 void Attach(PointRow row){row.Write=(axis,value)=>commit(row.ID,axis,value);row.DeleteCommand=new RowCommand(()=>DeletePoint(row.ID));}
 sealed class RowCommand(Action run):System.Windows.Input.ICommand{public bool CanExecute(object? p)=>true;public void Execute(object? p)=>run();public event EventHandler? CanExecuteChanged{add{}remove{}}}
 void RefreshDelete(){}
 public bool TrySetCell(int row,int axis,string value){if(row<0||row>=Rows.Count||axis<0||axis>=5)return false;var item=Rows[row];string before=item.Cells[axis];switch(axis){case 0:item.ImageX=value;break;case 1:item.ImageY=value;break;case 2:item.WorldX=value;break;case 3:item.WorldY=value;break;case 4:item.Angle=value;break;}return item.Cells[axis]==value;}
 public bool DeletePoint(int index){if(Rows.Count==4||index<0||index>=Rows.Count)return false;var row=Rows[index];Rows.RemoveAt(index);if(!delete(index))return false;for(int i=0;i<Rows.Count;i++)Rows[i].ID=i;Table.Items.Refresh();RefreshDelete();return true;}
 public bool ClearPoints(){if(!clear())return false;notify("清空标定点成功",false);foreach(var row in Rows)for(int a=0;a<5;a++)TrySetCell(row.ID,a,"0.000");return true;}
 public string ExportText()=>NPointFile.ExportPointRows(Rows.Select(r=>(IReadOnlyList<string>)r.Cells));
 public void ImportText(string text){var accepted=new ObservableCollection<PointRow>();var lines=text.Split(["\r\n"],StringSplitOptions.RemoveEmptyEntries);NPointFile.ImportPointText(text,Rows.Count,(i,values)=>{var fields=lines[i].Split(' ',StringSplitOptions.RemoveEmptyEntries);var row=new PointRow{ID=i};for(int a=0;a<5;a++){row.Cells[a]="0";if(commit(i,a,fields[a]))row.Cells[a]=fields[a];}Attach(row);accepted.Add(row);});Rows.Clear();foreach(var row in accepted)Rows.Add(row);RefreshDelete();}
 public Window CreateWindow(Window? owner=null)=>new(){Owner=owner,Title="编辑标定点",Width=493,Height=399,Content=this,WindowStartupLocation=WindowStartupLocation.CenterOwner,ResizeMode=ResizeMode.NoResize};
}
