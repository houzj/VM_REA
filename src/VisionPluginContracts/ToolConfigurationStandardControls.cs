using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using SystemColors = System.Windows.SystemColors;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Button = System.Windows.Controls.Button;
using ComboBox = System.Windows.Controls.ComboBox;
using RadioButton = System.Windows.Controls.RadioButton;
using TextBox = System.Windows.Controls.TextBox;
using Orientation = System.Windows.Controls.Orientation;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Brushes = System.Windows.Media.Brushes;

namespace VisionPluginContracts;

public sealed record ConfigurationExecutionVisualState(bool Continuous, bool ProcedureRunning, bool RunEnabled);
public sealed record ConfigurationControlLimitation(string Code, string TemplateKey, string Message);

/// <summary>Shared leaf factories. Selection is by original template key, never execution DataKind.</summary>
public static class ToolConfigurationStandardControls
{
    public static void Register(ToolConfigurationControlRegistry registry, Func<string,string>? localize = null,
        Func<ConfigurationExecutionVisualState>? executionState = null, string language = "Chinese", IToolConfigurationSubscriptionService? subscriptions = null)
    {
        var text=localize??(s=>s);
        registry.Register("Boolean",c=>Boolean(c,text));
        registry.Register("Integer",c=>Numeric(c,text,false,subscriptions));
        registry.Register("Float",c=>Numeric(c,text,true,subscriptions));
        registry.Register("Enumeration",c=>Enumeration(c,text));
        registry.Register("RadioSelecter",c=>Radios(c,text,language));
        registry.Register("String",c=>String(c,text,subscriptions));
        registry.Register("Command",c=>InlineCommand(c,text));
        registry.Register("Button_ResetAlgoParams",c=>Reset(c,text));
        registry.Register("Expand",c=>Expand(c,text));
        foreach(var key in new[]{"BottomCommand","ExecuteOnceBottomCommand","ContinuousExecuteBottomCommand","StopExecuteBottomCommand"})
        { var captured=key; registry.Register(key,c=>Command(c,text,captured,executionState)); }
    }
    static ResourceDictionary Resources()=>new(){Source=new Uri("/VisionPluginContracts;component/Resources/ConfigurationControls.xaml",UriKind.Relative)};
    static Grid Row(ToolConfigurationControlContext c, FrameworkElement editor, Func<string,string> text, double left=89)
    {
        var row=new Grid{Height=32};row.Resources.MergedDictionaries.Add(Resources());
        row.ColumnDefinitions.Add(new(){Width=new GridLength(left)});row.ColumnDefinitions.Add(new(){Width=new GridLength(1,GridUnitType.Star)});
        var label=new TextBlock{FontSize=14,FontWeight=FontWeights.Normal,Foreground=(Brush)row.FindResource("ConfigurationControlForeground"),VerticalAlignment=VerticalAlignment.Center,TextTrimming=TextTrimming.CharacterEllipsis};
        row.Children.Add(label);Grid.SetColumn(editor,1);row.Children.Add(editor);
        c.OnRefresh(()=>{label.Text=text(c.Node.CaptionKey);label.ToolTip=text(c.Node.Get("Description")??c.Node.CaptionKey);});return row;
    }
    static FrameworkElement Boolean(ToolConfigurationControlContext c,Func<string,string> text)
    {
        bool updating=false;
        var toggle=new ToggleButton{HorizontalAlignment=HorizontalAlignment.Left};
        var row=Row(c,toggle,text);toggle.Style=(Style)row.FindResource("ConfigurationSwitch");
        toggle.Click+=(_,_)=>{if(!updating)c.State.CommitUser(c.Node,(toggle.IsChecked==true).ToString());};
        c.OnRefresh(()=>{updating=true;toggle.IsChecked=bool.TryParse(c.Node.Value,out var b)&&b;toggle.IsEnabled=Writable(c);updating=false;});return row;
    }
    static bool Writable(ToolConfigurationControlContext c)=>c.Node.Enabled&&((c.Node.Get("AccessMode")??"").Contains('W',StringComparison.OrdinalIgnoreCase)||(c.Node.Get("AccessMode")??"").Contains('O',StringComparison.OrdinalIgnoreCase));
    static FrameworkElement Numeric(ToolConfigurationControlContext c,Func<string,string> text,bool floating,IToolConfigurationSubscriptionService? subscriptions)
    {
        var spinner=new ConfigurationNumericSpinner(floating);
        spinner.ValueCommitted+=value=>{c.State.CommitUser(c.Node,value);spinner.SetText(c.Node.Value);};
        var editor=new Grid();editor.ColumnDefinitions.Add(new(){Width=new GridLength(1,GridUnitType.Star)});editor.ColumnDefinitions.Add(new(){Width=GridLength.Auto});editor.Children.Add(spinner);
        var row=Row(c,editor,text);
        var mode=new ToggleButton{Style=(Style)row.FindResource("ConfigurationEditLinkButton")};Grid.SetColumn(mode,1);editor.Children.Add(mode);
        ToolConfigurationSubscriptionSelector? selector=subscriptions==null?null:ToolConfigurationSubscriptionControls.CreateInline(c,subscriptions,true,text);
        if(selector!=null)editor.Children.Add(selector);
        var warning=new TextBlock{Text="未支持控件: ButtonSelecter_sub",Foreground=Brushes.DarkRed,VerticalAlignment=VerticalAlignment.Center,
            Tag=new ConfigurationControlLimitation("UnsupportedSubscription","ButtonSelecter_sub","Subscribed numeric leaf requires the shared subscription selector."),Visibility=Visibility.Collapsed};
        c.OnRefresh(()=>{
            spinner.Minimum=Number(c.Node.Get("MinValue"),0);spinner.Maximum=Number(c.Node.Get("MaxValue"),0);
            spinner.Increment=Number(c.Node.Get("IncValue"),1);spinner.DecimalDigits=(int)Number(c.Node.Get("DecimalDigits"),2);
            spinner.NeedSlider=!floating||!bool.TryParse(c.Node.Get("NeedSlider"),out var need)||need;
            spinner.SetText(c.Node.Value);spinner.IsReadOnly=!Writable(c);
            bool subscribed=bool.TryParse(c.Node.Get("IsSub"),out var sub)&&sub;
            if(subscribed)spinner.HideSlider();
            spinner.Visibility=subscribed?Visibility.Collapsed:Visibility.Visible;
            if(selector!=null)selector.Visibility=subscribed?Visibility.Visible:Visibility.Collapsed;
            warning.Visibility=subscribed&&selector==null?Visibility.Visible:Visibility.Collapsed;
            mode.IsChecked=subscribed;mode.Visibility=c.Node.SubscribeVisible?Visibility.Visible:Visibility.Collapsed;
            mode.IsEnabled=Writable(c)&&subscriptions!=null;mode.ToolTip=subscriptions==null?"未支持: 订阅服务未提供":subscribed?"切换编辑":"切换订阅";
        });
        mode.Click+=(_,_)=>{if(selector!=null)selector.SetMode(mode.IsChecked==true);mode.IsChecked=bool.TryParse(c.Node.Get("IsSub"),out var current)&&current;};
        editor.Children.Add(warning);return row;
    }
    static double Number(string? value,double fallback)=>double.TryParse(value,NumberStyles.Float,CultureInfo.InvariantCulture,out var n)&&double.IsFinite(n)?n:fallback;
    sealed record DisplayChoice(string Caption,string Value);
    static FrameworkElement Enumeration(ToolConfigurationControlContext c,Func<string,string> text)
    {
        var combo=new ComboBox{DisplayMemberPath="Caption",SelectedValuePath="Value",FontWeight=FontWeights.Normal};bool updating=false;
        var row=Row(c,combo,text);combo.Style=(Style)row.FindResource("ConfigurationEnumeration");
        combo.SelectionChanged+=(_,_)=>{if(!updating&&combo.SelectedValue is string v)c.State.CommitUser(c.Node,v);};
        combo.DropDownOpened+=(_,_)=>c.State.OpenChoices(c.Node);
        c.OnRefresh(()=>{updating=true;combo.ItemsSource=c.Node.Choices.Select(x=>new DisplayChoice(text(x.CaptionKey),x.Value)).ToArray();combo.SelectedValue=c.Node.Value;combo.IsEnabled=Writable(c);combo.ToolTip=combo.Text;updating=false;});return row;
    }
    static FrameworkElement Radios(ToolConfigurationControlContext c,Func<string,string> text,string language)
    {
        var panel=new WrapPanel{VerticalAlignment=VerticalAlignment.Center};var row=Row(c,panel,text,language=="English"?165:89);bool updating=false;
        var group=Guid.NewGuid().ToString("N");
        c.OnRefresh(()=>{
            updating=true;panel.Children.Clear();
            // Original RadioSelecter explicitly renders EnumEntrys[0] and [1], not an arbitrary enum grid.
            foreach(var choice in c.Node.Choices.Take(2)){
                var radio=new RadioButton{Content=text(choice.CaptionKey),FontWeight=FontWeights.Normal,GroupName=group,Margin=new Thickness(2,0,0,0),Focusable=true,Style=(Style)row.FindResource("ConfigurationRadio"),IsChecked=c.Node.Value==choice.Value,IsEnabled=Writable(c)};
                radio.Checked+=(_,_)=>{if(!updating)c.State.CommitUser(c.Node,choice.Value);};panel.Children.Add(radio);
            }updating=false;
        });return row;
    }
    static FrameworkElement InlineCommand(ToolConfigurationControlContext c,Func<string,string> text)
    {
        var button=new Button{Width=double.NaN,Padding=new Thickness(4,0,4,0),HorizontalAlignment=HorizontalAlignment.Left};
        var row=Row(c,button,text);button.Style=(Style)row.FindResource("ConfigurationBottomButton");
        button.Click+=(_,_)=>c.State.Invoke(c.Node);c.OnRefresh(()=>button.Content=text(c.Node.CaptionKey));return row;
    }
    static FrameworkElement Reset(ToolConfigurationControlContext c,Func<string,string> text)
    {
        var panel=new StackPanel{Orientation=Orientation.Horizontal,HorizontalAlignment=HorizontalAlignment.Left};panel.Resources.MergedDictionaries.Add(Resources());
        var button=new Button{Margin=new Thickness(0,0,8,0),ToolTip=text("Lang_Reset parameters"),Style=(Style)panel.FindResource("ConfigurationResetButton")};
        button.Click+=(_,_)=>c.State.Invoke(c.Node);panel.Children.Add(button);return panel;
    }
    static FrameworkElement Expand(ToolConfigurationControlContext c,Func<string,string> text)
    {
        var toggle=new ToggleButton{Height=30,Margin=new Thickness(0,0,40,0),Padding=new Thickness(6,0,0,0),HorizontalAlignment=HorizontalAlignment.Right,VerticalContentAlignment=VerticalAlignment.Center,Content=text("高级参数")};
        toggle.Resources.MergedDictionaries.Add(Resources());toggle.Resources["收起"]=text("收起");toggle.Style=(Style)toggle.FindResource("ConfigurationExpandButton");
        toggle.Click+=(_,_)=>c.State.CommitUser(c.Node,(toggle.IsChecked==true).ToString());
        c.OnRefresh(()=>toggle.IsChecked=bool.TryParse(c.Node.Value,out var b)&&b);return toggle;
    }
    static FrameworkElement String(ToolConfigurationControlContext c,Func<string,string> text,IToolConfigurationSubscriptionService? subscriptions)
    {
        var editor=new Grid();editor.ColumnDefinitions.Add(new(){Width=new GridLength(1,GridUnitType.Star)});editor.ColumnDefinitions.Add(new(){Width=GridLength.Auto});
        var input=new TextBox{FontSize=14,FontWeight=FontWeights.Normal,Foreground=new SolidColorBrush(Color.FromRgb(128,128,128)),Background=Brushes.Transparent,BorderThickness=new Thickness(0),VerticalAlignment=VerticalAlignment.Center};
        var border=new Border{CornerRadius=new CornerRadius(4),BorderThickness=new Thickness(1),BorderBrush=new SolidColorBrush(Color.FromRgb(204,204,205)),Child=input};editor.Children.Add(border);
        var row=Row(c,editor,text);var mode=new ToggleButton{Style=(Style)row.FindResource("ConfigurationEditLinkButton")};Grid.SetColumn(mode,1);editor.Children.Add(mode);
        var selector=subscriptions==null?null:ToolConfigurationSubscriptionControls.CreateInline(c,subscriptions,true,text);if(selector!=null)editor.Children.Add(selector);
        var warning=new TextBlock{Text="未支持控件: ButtonSelecter_sub（订阅服务未提供）",Foreground=Brushes.DarkRed,TextWrapping=TextWrapping.Wrap,Tag=new ConfigurationControlLimitation("UnsupportedSubscription","ButtonSelecter_sub","String parameter requires the shared typed subscription service.")};editor.Children.Add(warning);
        input.LostFocus+=(_,_)=>{if(!input.IsReadOnly)c.State.CommitUser(c.Node,input.Text);input.Text=c.Node.Value;};
        mode.Click+=(_,_)=>{if(selector!=null)selector.SetMode(mode.IsChecked==true);mode.IsChecked=bool.TryParse(c.Node.Get("IsSub"),out var current)&&current;};
        c.OnRefresh(()=>{
            input.Text=c.Node.Value;input.ToolTip=c.Node.Value;input.IsReadOnly=!Writable(c);input.MaxLength=Math.Max(0,(int)Number(c.Node.Get("Length"),0));
            bool subscribed=bool.TryParse(c.Node.Get("IsSub"),out var sub)&&sub;border.Visibility=subscribed?Visibility.Collapsed:Visibility.Visible;
            if(selector!=null)selector.Visibility=subscribed?Visibility.Visible:Visibility.Collapsed;
            warning.Visibility=subscribed&&selector==null?Visibility.Visible:Visibility.Collapsed;mode.IsChecked=subscribed;mode.Visibility=c.Node.SubscribeVisible?Visibility.Visible:Visibility.Collapsed;mode.IsEnabled=Writable(c)&&subscriptions!=null;
        });return row;
    }
    static FrameworkElement Command(ToolConfigurationControlContext c,Func<string,string> text,string key,Func<ConfigurationExecutionVisualState>? execution)
    {
        var button=new Button{MinWidth=66,Width=double.NaN,Margin=new Thickness(2.5,0,2.5,0),Padding=new Thickness(10,0,10,0)};
        button.Resources.MergedDictionaries.Add(Resources());button.Style=(Style)button.FindResource("ConfigurationBottomButton");
        button.Click+=(_,_)=>c.State.Invoke(c.Node);
        c.OnRefresh(()=>{
            var e=execution?.Invoke()??new(false,c.State.IsBusy,true);button.Content=text(c.Node.CaptionKey);
            bool stop=key=="StopExecuteBottomCommand",run=key is "ExecuteOnceBottomCommand" or "ContinuousExecuteBottomCommand";
            button.Visibility=c.Node.Visible&&(!stop||e.Continuous)&&(!run||!e.Continuous&&e.RunEnabled)?Visibility.Visible:Visibility.Collapsed;
            button.IsEnabled=c.Node.Enabled&&(!run||!e.ProcedureRunning);
        });return button;
    }
}

/// <summary>NumericControl reconstruction: 32px row, 20px up/down RepeatButtons, focused wheel, finite bounded edits.</summary>
public sealed class ConfigurationNumericSpinner : System.Windows.Controls.UserControl
{
    readonly bool floating;
    Border? controlBorder;
    public string? ValidationError {get;private set;}
    public TextBox Editor {get;}=new(){FontWeight=FontWeights.Normal,BorderThickness=new Thickness(0),Background=Brushes.Transparent,Margin=new Thickness(7,4,2,3),VerticalContentAlignment=VerticalAlignment.Center};
    public RepeatButton UpButton {get;}=new(){Width=20,Height=15,Focusable=false,VerticalAlignment=VerticalAlignment.Bottom};
    public RepeatButton DownButton {get;}=new(){Width=20,Height=15,Focusable=false,VerticalAlignment=VerticalAlignment.Top,RenderTransformOrigin=new(.5,.5),RenderTransform=new ScaleTransform(1,-1)};
    public double Minimum {get;set;}=0;
    public double Maximum {get;set;}=100;
    public double Increment {get;set;}=1;
    public int DecimalDigits {get;set;}=2;
    int FormatDigits=>DecimalDigits>=0&&DecimalDigits<10?DecimalDigits:3;
    bool needSlider=true;
    public bool NeedSlider {get=>needSlider;set{needSlider=value;if(!value)HideSlider();}}
    public Slider ValueSlider {get;}=new(){IsSnapToTickEnabled=true,TickPlacement=TickPlacement.BottomRight,IsSelectionRangeEnabled=true};
    public Popup SliderPopup {get;}=new(){Placement=PlacementMode.Bottom,AllowsTransparency=true,StaysOpen=false};
    bool updatingSlider, hidingSlider;
    public bool IsReadOnly {get=>Editor.IsReadOnly;set{Editor.IsReadOnly=value;UpButton.Visibility=DownButton.Visibility=value?Visibility.Collapsed:Visibility.Visible;if(value)HideSlider();}}
    public event Action<string>? ValueCommitted;
    public ConfigurationNumericSpinner(bool floating)
    {
        this.floating=floating;Height=32;Resources.MergedDictionaries.Add(new ResourceDictionary{Source=new Uri("/VisionPluginContracts;component/Resources/ConfigurationControls.xaml",UriKind.Relative)});
        Editor.Foreground=(Brush)FindResource("ConfigurationControlForeground");Editor.FontSize=14;InputMethod.SetIsInputMethodEnabled(Editor,false);
        UpButton.Style=DownButton.Style=(Style)FindResource("ConfigurationNumericArrow");
        var grid=new Grid();grid.ColumnDefinitions.Add(new(){Width=new GridLength(1,GridUnitType.Star)});grid.ColumnDefinitions.Add(new(){Width=new GridLength(20)});grid.Children.Add(Editor);
        var arrows=new Grid();arrows.RowDefinitions.Add(new());arrows.RowDefinitions.Add(new());arrows.Children.Add(UpButton);Grid.SetRow(DownButton,1);arrows.Children.Add(DownButton);Grid.SetColumn(arrows,1);grid.Children.Add(arrows);
        Content=controlBorder=new Border{BorderThickness=new Thickness(1),Margin=new Thickness(0,2,0,2),CornerRadius=new CornerRadius(2),BorderBrush=new SolidColorBrush(Color.FromRgb(204,204,204)),Background=new SolidColorBrush(Color.FromRgb(252,252,252)),Child=grid};
        ValueSlider.Style=(Style)FindResource("SliderStyle1");
        SliderPopup.PlacementTarget=this;
        SliderPopup.Child=new Border{Margin=new Thickness(3),BorderThickness=new Thickness(1),BorderBrush=SystemColors.WindowFrameBrush,Background=SystemColors.WindowBrush,Child=ValueSlider,
            Effect=new System.Windows.Media.Effects.DropShadowEffect{Color=Colors.Gray,BlurRadius=6,Direction=321,ShadowDepth=2,Opacity=.5}};
        Editor.GotKeyboardFocus+=(_,_)=>OpenSlider();
        Editor.PreviewMouseDown+=(_,_)=>OpenSlider();
        SliderPopup.Closed+=(_,_)=>{if(!hidingSlider)CommitText();};
        ValueSlider.AddHandler(Thumb.DragCompletedEvent,new DragCompletedEventHandler((_,_)=>Commit(ValueSlider.Value)));
        ValueSlider.PreviewMouseLeftButtonUp+=(_,_)=>Commit(ValueSlider.Value);
        ValueSlider.ValueChanged+=(_,_)=>{if(!updatingSlider)Editor.Text=ValueSlider.Value.ToString(floating?"F"+FormatDigits:"0",CultureInfo.InvariantCulture);};
        Unloaded+=(_,_)=>SliderPopup.IsOpen=false;
        UpButton.Click+=(_,_)=>Step(1);DownButton.Click+=(_,_)=>Step(-1);
        Editor.LostKeyboardFocus+=(_,_)=>CommitText();
        Editor.PreviewKeyDown+=(_,e)=>{if(e.Key==Key.Space)e.Handled=true;else if(e.Key==Key.Enter){CommitText();e.Handled=true;}};
        Editor.PreviewMouseWheel+=(_,e)=>{if(Editor.IsKeyboardFocused&&!IsReadOnly){Step(e.Delta>0?1:-1);e.Handled=true;}};
    }
    public void SetText(string value)
    {
        if(floating&&double.TryParse(value,NumberStyles.Float,CultureInfo.InvariantCulture,out var number)&&double.IsFinite(number))value=number.ToString("F"+FormatDigits,CultureInfo.InvariantCulture);
        if(Editor.Text!=value)Editor.Text=value;
        ValidateMessage(null);
    }
    public void OpenSlider()
    {
        if(IsReadOnly||!NeedSlider||!IsLoaded)return;
        ConfigureSlider();SliderPopup.IsOpen=true;
    }
    public void HideSlider(){hidingSlider=true;try{SliderPopup.IsOpen=false;}finally{hidingSlider=false;}}
    public void ConfigureSlider()
    {
        updatingSlider=true;
        ValueSlider.Minimum=Minimum;ValueSlider.Maximum=Math.Max(Minimum,Maximum);
        ValueSlider.TickFrequency=ValueSlider.SmallChange=ValueSlider.LargeChange=Increment>0?Increment:1;
        ValueSlider.Width=Math.Max(1,ActualWidth);
        if(double.TryParse(Editor.Text,NumberStyles.Float,CultureInfo.InvariantCulture,out var n)&&double.IsFinite(n))ValueSlider.Value=Math.Clamp(n,Minimum,Math.Max(Minimum,Maximum));
        updatingSlider=false;
    }
    public bool CommitText()
    {
        if(IsReadOnly)return false;
        var literal=Editor.Text.Length==0?"0":Editor.Text;
        if(!double.TryParse(literal,NumberStyles.Float,CultureInfo.InvariantCulture,out var value)||!double.IsFinite(value)||!floating&&value!=Math.Truncate(value)){ValidateMessage("Illegal characters or not a finite number");return false;}
        if(value<Minimum||value>Maximum){ValidateMessage($"Please enter an age in the range: {Minimum} - {Maximum}.");return false;}
        Commit(value);return true;
    }
    public void Step(int direction)
    {
        if(IsReadOnly)return;
        if(!double.TryParse(Editor.Text,NumberStyles.Float,CultureInfo.InvariantCulture,out var value)||!double.IsFinite(value))return;
        Commit(value+(direction>0?Increment:-Increment));
    }
    void Commit(double value)
    {
        ValidateMessage(null);
        value=Math.Clamp(value,Minimum,Math.Max(Minimum,Maximum));
        value=floating?Math.Round(value,FormatDigits):Math.Truncate(value);
        Editor.Text=value.ToString(floating?"F"+FormatDigits:"0",CultureInfo.InvariantCulture);ValueCommitted?.Invoke(value.ToString(CultureInfo.InvariantCulture));
    }
    void ValidateMessage(string? error)
    {
        ValidationError=error;Editor.ToolTip=error;
        if(controlBorder!=null)controlBorder.BorderBrush=error==null?new SolidColorBrush(Color.FromRgb(204,204,204)):Brushes.Red;
    }
}
