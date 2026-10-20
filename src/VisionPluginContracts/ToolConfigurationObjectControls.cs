using System.Globalization;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using W=System.Windows.Controls;
using Brushes=System.Windows.Media.Brushes;
using Brush=System.Windows.Media.Brush;
using HorizontalAlignment=System.Windows.HorizontalAlignment;

namespace VisionPluginContracts;

/// <summary>BridgeItemBase templates bind a display object, never CurValue. Color palette is supplied by the shared UI layer.</summary>
public static class ToolConfigurationObjectControls
{
    /// <summary>Compose the source default/more tabs without a contracts-to-VisionUi assembly dependency.
    /// Host passes SourceColorPalette and SourceAdvancedColorPicker factories and their source styles URI.</summary>
    public static FrameworkElement CreateColorTabs(string initialColor,Action<string> selected,
        Func<Action<string>,FrameworkElement> defaultPalette,
        Func<string,Action<string>,FrameworkElement> advancedPicker,Uri? styles=null,
        Func<string,string>? localize=null)
    {
        var tabs=new W.TabControl();if(styles!=null)tabs.Resources.MergedDictionaries.Add(new ResourceDictionary{Source=styles});
        tabs.Items.Add(new W.TabItem{Header=(localize??(s=>s))("默认"),Content=defaultPalette(selected)});
        tabs.Items.Add(new W.TabItem{Header=(localize??(s=>s))("更多"),Content=advancedPicker(initialColor,selected)});
        return new W.Border{Background=Brushes.White,Child=tabs};
    }
    public static void Register(ToolConfigurationControlRegistry registry,IToolConfigurationObjectService objects,
        Func<string,string>? localize=null,Func<Action<string>,FrameworkElement>? colorPalette=null,
        IToolConfigurationTextPlaceholderService? placeholders=null,
        Func<string,Action<string>,FrameworkElement>? colorPicker=null)
    {
        foreach(var key in new[]{"ColorConfig","FloatConfig","StringConfig"})registry.Register(key,c=>Create(c,objects,localize??(s=>s),colorPalette,placeholders,colorPicker));
    }
    static FrameworkElement Create(ToolConfigurationControlContext c,IToolConfigurationObjectService objects,Func<string,string> text,
        Func<Action<string>,FrameworkElement>? palette,IToolConfigurationTextPlaceholderService? placeholders,
        Func<string,Action<string>,FrameworkElement>? colorPicker)
    {
        var route=ConfigurationObjectRoute.Parse(c.Node);
        // BridgeObjA is lazy and cached, including retry when original resolution returns null.
        IToolConfigurationObjectBinding? binding=null;
        IToolConfigurationObjectBinding? Resolve()=>binding??=route==null?null:objects.Resolve(route);
        var row=new W.Grid{Height=32};row.ColumnDefinitions.Add(new(){Width=new GridLength(89)});row.ColumnDefinitions.Add(new());
        var label=new W.TextBlock{FontSize=14,Foreground=Brushes.Gray,VerticalAlignment=VerticalAlignment.Center,TextTrimming=TextTrimming.CharacterEllipsis};row.Children.Add(label);
        FrameworkElement editor;
        bool Writable()=>c.Node.Enabled&&((c.Node.Get("AccessMode")??"").Contains('W')||(c.Node.Get("AccessMode")??"").Contains('O'));
        string Read()=>Resolve()?.Read()??"";
        Action? RefreshValue=null;
        void Commit(string value){var result=Resolve()?.Write(value);row.ToolTip=result?.Error;RefreshValue?.Invoke();}
        if(c.Element.Tag=="FloatConfig"){
            var spinner=new ConfigurationNumericSpinner(true){DecimalDigits=3};editor=spinner;
            spinner.ValueCommitted+=Commit;
            RefreshValue=()=>{spinner.Minimum=Number(c.Node.Get("MinValue"),0);spinner.Maximum=Number(c.Node.Get("MaxValue"),0);spinner.Increment=Number(c.Node.Get("IncValue"),1);spinner.DecimalDigits=(int)Number(c.Node.Get("DecimalDigits"),3);spinner.SetText(Read());spinner.IsReadOnly=!Writable()||Resolve()==null;};
        }else if(c.Element.Tag=="ColorConfig"){
            var button=new ToggleButton{Margin=new Thickness(0,2,0,2),HorizontalContentAlignment=HorizontalAlignment.Stretch};editor=button;
            var swatch=new W.Border{Width=22,Height=18,Margin=new Thickness(4),BorderThickness=new Thickness(1),BorderBrush=Brushes.Gray};var display=new W.TextBlock{VerticalAlignment=VerticalAlignment.Center};var panel=new W.DockPanel();W.DockPanel.SetDock(swatch,W.Dock.Left);panel.Children.Add(swatch);panel.Children.Add(display);button.Content=panel;
            var popup=new Popup{PlacementTarget=button,Placement=PlacementMode.Bottom,StaysOpen=false,AllowsTransparency=true};
            void Selected(string value){Commit(NormalizeColor(value));popup.IsOpen=false;}
            if(palette!=null&&colorPicker==null)popup.Child=palette(Selected);
            button.Checked+=(_,_)=>{if(colorPicker!=null)popup.Child=colorPicker(Read(),Selected);popup.IsOpen=Writable()&&Resolve()!=null&&(palette!=null||colorPicker!=null);};
            button.Unchecked+=(_,_)=>popup.IsOpen=false;popup.Closed+=(_,_)=>button.IsChecked=false;
            button.Unloaded+=(_,_)=>popup.IsOpen=false;
            RefreshValue=()=>{var value=Read();try{var brush=(SolidColorBrush)new BrushConverter().ConvertFromInvariantString(value)!;swatch.Background=brush;display.Text=$"{brush.Color.R,3},{brush.Color.G,3},{brush.Color.B,3}";}catch{swatch.Background=Brushes.Transparent;display.Text="";}button.IsEnabled=Writable()&&Resolve()!=null&&(palette!=null||colorPicker!=null);if(palette==null&&colorPicker==null)button.ToolTip="未支持: 公共颜色选择器未提供";};
        }else{
            var edit=new ConfigurationObjectTextBox{FontSize=14,Foreground=Brushes.Gray,Background=Brushes.Transparent};
            editor=new W.Border{BorderThickness=new Thickness(1),CornerRadius=new CornerRadius(4),BorderBrush=new SolidColorBrush(System.Windows.Media.Color.FromRgb(204,204,205)),Background=Brushes.White,Child=edit};
            var tree=new W.TreeView{Width=180,Height=180};
            var gradient=new LinearGradientBrush();gradient.GradientStops.Add(new(System.Windows.Media.Color.FromRgb(252,252,252),0));gradient.GradientStops.Add(new(System.Windows.Media.Color.FromRgb(252,252,252),.15));gradient.GradientStops.Add(new(System.Windows.Media.Color.FromRgb(247,247,247),1));
            var popup=new Popup{PlacementTarget=edit,Placement=PlacementMode.Bottom,StaysOpen=false,AllowsTransparency=true,Child=new W.Border{Margin=new Thickness(0,4,0,0),BorderBrush=new SolidColorBrush(System.Windows.Media.Color.FromRgb(204,204,205)),BorderThickness=new Thickness(1),CornerRadius=new CornerRadius(4),Padding=new Thickness(0,6,0,6),Background=gradient,Child=tree}};int caret=0;
            void Focus(){caret=edit.CaretIndex;popup.IsOpen=Writable()&&placeholders!=null&&ConfigurationTextPlaceholderEditing.TryRange(edit.Text,caret,out _,out _);if(popup.IsOpen){tree.Items.Clear();foreach(var choice in placeholders!.Enumerate())tree.Items.Add(new W.TreeViewItem{Header=choice.Caption,Tag=choice});}}
            edit.FocusStateChanged+=()=>{if(edit.IsKeyboardFocused)Focus();else popup.IsOpen=false;};
            tree.MouseDoubleClick+=(_,_)=>{if(tree.SelectedItem is W.TreeViewItem{Tag:ConfigurationTextPlaceholder choice}){var result=ConfigurationTextPlaceholderEditing.Insert(edit.Text,caret,choice);if(result!=null)Commit(result);popup.IsOpen=false;}};
            edit.LostKeyboardFocus+=(_,_)=>{if(!popup.IsKeyboardFocusWithin){popup.IsOpen=false;Commit(edit.Text);}};edit.Unloaded+=(_,_)=>popup.IsOpen=false;
            RefreshValue=()=>{var value=Read();if(edit.Text!=value)edit.Text=value;edit.IsReadOnly=!Writable()||Resolve()==null;edit.ToolTip=value;};
        }
        W.Grid.SetColumn(editor,1);row.Children.Add(editor);
        c.OnRefresh(()=>{label.Text=text(c.Node.CaptionKey);RefreshValue();if(Resolve()==null){row.ToolTip="未支持: 对象桥接不存在或类型不匹配";row.Tag=new ConfigurationControlLimitation("UnsupportedObjectBinding",c.Element.Tag,"Original display-object/property route is unavailable.");}});
        return row;
    }
    static double Number(string? value,double fallback)=>double.TryParse(value,NumberStyles.Float,CultureInfo.InvariantCulture,out var number)&&double.IsFinite(number)?number:fallback;
    static string NormalizeColor(string value)=>value.Length==9&&value.StartsWith("#FF",StringComparison.OrdinalIgnoreCase)?"#"+value[3..]:value;
}

/// <summary>DeleteTextBox.xaml/.cs semantics: zero border, 10px inset, Delete clears all;
/// focus notifications only for an empty or full selection. No invented clear button.</summary>
public sealed class ConfigurationObjectTextBox : W.TextBox
{
    public event Action? FocusStateChanged;
    public ConfigurationObjectTextBox()
    {
        Margin=new Thickness(10,0,0,0);VerticalContentAlignment=VerticalAlignment.Center;BorderThickness=new Thickness(0);
        SelectionChanged+=(_,_)=>{if(SelectionLength==0||SelectionLength==Text.Length)FocusStateChanged?.Invoke();};
        KeyUp+=(_,e)=>{if(e.Key==System.Windows.Input.Key.Delete)Text="";};
        MouseDown+=(_,e)=>e.Handled=true;
    }
}
