using Size = System.Windows.Size;
using Panel = System.Windows.Controls.Panel;
using Brushes = System.Windows.Media.Brushes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UserControl = System.Windows.Controls.UserControl;
using Control = System.Windows.Controls.Control;
using TabControl = System.Windows.Controls.TabControl;
using Orientation = System.Windows.Controls.Orientation;
using HorizontalAlignment = System.Windows.HorizontalAlignment;

namespace VisionPluginContracts;

public sealed class ToolConfigurationControlContext
{
    readonly List<Action> refresh = [];
    internal ToolConfigurationControlContext(ToolConfigurationState state, ConfigurationNodeState node, Func<Window?> owner)
    { State=state; Node=node; Owner=owner; }
    public ToolConfigurationState State { get; }
    public ConfigurationNodeState Node { get; }
    public ConfigurationElement Element => Node.Element;
    public Func<Window?> Owner { get; }
    public Window? OwnerWindow => Owner();
    public void OnRefresh(Action action) => refresh.Add(action);
    internal void Refresh() { foreach(var action in refresh)action(); }
}

/// <summary>Leaf factories own FrameworkElements, never configuration windows or algorithm execution.</summary>
public sealed class ToolConfigurationControlRegistry
{
    readonly Dictionary<string,Func<ToolConfigurationControlContext,FrameworkElement>> factories = new(StringComparer.Ordinal);
    public void Register(string templateKey,Func<ToolConfigurationControlContext,FrameworkElement> factory)
    { ArgumentException.ThrowIfNullOrWhiteSpace(templateKey); ArgumentNullException.ThrowIfNull(factory); factories[templateKey]=factory; }
    public bool TryCreate(string key,ToolConfigurationControlContext context,out FrameworkElement? control)
    { control=factories.TryGetValue(key,out var factory)?factory(context):null; return control!=null; }
}

public sealed record ConfigurationViewDiagnostic(string Code,string TemplateKey,string? Node,string Message);
/// <summary>Compact, common shell. Template factories and evaluated state remain separate from the shell.</summary>
public sealed class ToolConfigurationView : UserControl, IDisposable
{
    public static readonly DependencyProperty UseItemMarginProperty=DependencyProperty.RegisterAttached("UseItemMargin",typeof(bool),typeof(ToolConfigurationView),new PropertyMetadata(true));
    public static void SetUseItemMargin(DependencyObject target,bool value)=>target.SetValue(UseItemMarginProperty,value);
    public static bool GetUseItemMargin(DependencyObject target)=>(bool)target.GetValue(UseItemMarginProperty);
    readonly ToolConfigurationState state;
    readonly ToolConfigurationControlRegistry registry;
    readonly Func<string,string> localize;
    readonly List<(ConfigurationNodeState Node,FrameworkElement Control,ToolConfigurationControlContext? Context)> bindings=[];
    readonly List<ConfigurationViewDiagnostic> diagnostics=[];
    readonly StackPanel left = new(){Orientation=Orientation.Horizontal,HorizontalAlignment=HorizontalAlignment.Left,Height=32,Margin=new Thickness(4,0,0,0)};
    readonly StackPanel right = new(){Orientation=Orientation.Horizontal,HorizontalAlignment=HorizontalAlignment.Right,Height=32,Margin=new Thickness(0,0,14,0)};
    readonly StackPanel externs = new();
    readonly List<Action> shellRefresh=[];
    readonly HashSet<FrameworkElement> panelContents=[];
    bool disposed;
    public Thickness ChildMargin {get;}
    public Thickness ChildHeaderMargin {get;}
    public TabControl Tabs { get; } = new();
    public FrameworkElement LeftCommands => left;
    public FrameworkElement RightCommands => right;
    public FrameworkElement ExternParameters => externs;
    public Size DesiredConfigurationSize { get; }
    public IReadOnlyList<ConfigurationViewDiagnostic> Diagnostics => diagnostics.AsReadOnly();
    public bool TabHeadersVisible => Tabs.Items.OfType<TabItem>().Count(t=>t.Visibility==Visibility.Visible)>1;
    public ToolConfigurationView(ToolConfigurationState state,ToolConfigurationControlRegistry registry,string language="Chinese",Func<string,string>? localize=null)
    {
        ChildMargin=language=="English"?new Thickness(24,1,24,0):new Thickness(32,1,32,0);
        ChildHeaderMargin=language=="English"?new Thickness(24,0,0,8):new Thickness(32,0,0,8);
        this.state=state;this.registry=registry;this.localize=localize??(key=>key);
        Resources.MergedDictionaries.Add(new ResourceDictionary{Source=new Uri("/VisionPluginContracts;component/Resources/ConfigurationShell.xaml",UriKind.Relative)});
        var geometry=state.Definition.ResolveGeometry(language);DesiredConfigurationSize=new(geometry.Width,geometry.Height);
        Tabs.Style=(Style)FindResource("ConfigurationTabs");Tabs.SelectionChanged+=(_,_)=>RefreshLeft();
        var shell=new Grid{Background=(System.Windows.Media.Brush)FindResource("Col_SubForm_Background")};shell.RowDefinitions.Add(new(){Height=new GridLength(1,GridUnitType.Star)});shell.RowDefinitions.Add(new(){Height=GridLength.Auto});shell.RowDefinitions.Add(new(){Height=GridLength.Auto});
        shell.Children.Add(Tabs);Grid.SetRow(externs,1);shell.Children.Add(externs);
        var footer=new Grid{Background=(System.Windows.Media.Brush)FindResource("GlobalBorderBg")};footer.Children.Add(left);footer.Children.Add(right);Grid.SetRow(footer,2);shell.Children.Add(footer);
        foreach(var element in state.Definition.Tabs)
        {
            var node=state.GetNode(element);if(node==null){Missing(element);continue;}
            var tab=new TabItem{Header=this.localize(node.CaptionKey),Tag=element.Name,Width=language=="English"?110:76,Style=(Style)FindResource("ConfigurationTab")};
            if(node.IsPanel) {
                // Original Tab.InitCollection flattens Category.Items; gridPanal binds Collection[0], not a category.
                var first=element.Children.FirstOrDefault(x=>x.Tag=="Categorys")?.Children
                    .Where(x=>state.GetNode(x)?.IsCategory==true)
                    .SelectMany(x=>x.Children.FirstOrDefault(c=>c.Tag=="Items")?.Children??Array.Empty<ConfigurationElement>()).FirstOrDefault();
                var panel=new Grid();
                if(first!=null){var control=Create(first);control.Margin=new Thickness(0);panelContents.Add(control);panel.Children.Add(control);}
                tab.Content=panel;
            } else {
                var contents=new StackPanel();BuildChildren(element,contents);
                tab.Content=new ScrollViewer{VerticalScrollBarVisibility=ScrollBarVisibility.Auto,HorizontalScrollBarVisibility=ScrollBarVisibility.Disabled,Content=contents};
            }
            Tabs.Items.Add(tab);bindings.Add((node,tab,null));
        }
        foreach(var e in state.Definition.ExternParams)externs.Children.Add(Create(e));
        foreach(var e in state.Definition.LeftBottomCommands){var c=Create(e);c.Margin=new Thickness(0,4,0,4);left.Children.Add(c);}
        foreach(var e in state.Definition.BottomCommands){var c=Create(e);c.Margin=new Thickness(0,4,0,4);right.Children.Add(c);}
        Content=shell;state.Changed+=StateChanged;Refresh();
    }
    void BuildChildren(ConfigurationElement element,Panel panel)
    {
        foreach(var child in element.Children)
        {
            if(child.Tag is "Categorys" or "Items") {BuildChildren(child,panel);continue;}
            if(state.GetNode(child)?.IsCategory==true)
            {
                var group=new StackPanel{Margin=new Thickness(0,0,0,5)};var node=state.GetNode(child);
                var header=new StackPanel{Margin=ChildHeaderMargin};
                if(panel.Children.Count>0)header.Children.Add(new Border{Style=(Style)FindResource("ConfigurationSeparator")});
                var title=new TextBlock{Text=localize(node?.CaptionKey??child.Name??""),Style=(Style)FindResource("ConfigurationCategoryHeader")};header.Children.Add(title);group.Children.Add(header);
                if(node!=null)shellRefresh.Add(()=>title.Visibility=(node.Get("DisplayType")??node.DisplayType)=="CategoryNoHead"?Visibility.Collapsed:Visibility.Visible);
                BuildChildren(child,group);panel.Children.Add(group);if(node!=null)bindings.Add((node,group,null));else Missing(child);
            }
            else if(state.GetNode(child)!=null){var control=Create(child);if(GetUseItemMargin(control))control.Margin=ChildMargin;panel.Children.Add(control);}
        }
    }
    FrameworkElement Create(ConfigurationElement element)
    {
        var node=state.GetNode(element);
        if(node==null){Missing(element);return Unsupported(element.Tag,element.Name);}
        var context=new ToolConfigurationControlContext(state,node,()=>Window.GetWindow(this));
        var key=node.Template.TemplateKey??node.Template.EffectiveDisplayType??"";
        FrameworkElement? control=null;
        if(string.IsNullOrEmpty(key)||!registry.TryCreate(key,context,out control))
        {
            var diagnosticKey=string.IsNullOrEmpty(key)?element.Tag:key;
            diagnostics.Add(new("UnsupportedTemplate",diagnosticKey,node.Name,"No registered shared configuration template."));control=Unsupported(diagnosticKey,node.Name);
        }
        if(control is Window)throw new InvalidOperationException("Configuration factories must return leaf content, not Window.");
        bindings.Add((node,control!,context));
        shellRefresh.Add(()=>{var current=node.Template.TemplateKey??node.Template.EffectiveDisplayType??"";if(current!=key&&!diagnostics.Any(d=>d.Code=="TemplateChanged"&&d.Node==node.Name&&d.TemplateKey==current))diagnostics.Add(new("TemplateChanged",current,node.Name,"Template selection changed; explicit structural rebuild required."));});
        return control!;
    }
    static FrameworkElement Unsupported(string key,string? name)=>new TextBlock{Text=$"未支持控件: {key} ({name})",Foreground=Brushes.DarkRed,TextWrapping=TextWrapping.Wrap,Margin=new Thickness(0,4,0,4)};
    void Missing(ConfigurationElement element)=>diagnostics.Add(new("MissingState",element.Tag,element.Name,"Configuration element has no evaluated node state."));
    void StateChanged(){if(Dispatcher.CheckAccess())Refresh();else Dispatcher.BeginInvoke((Action)Refresh);}
    public void Refresh()
    {
        foreach(var b in bindings){
            // gridPanal's direct ParamUserControl has no ParamItemTemplate enable/visibility wrapper.
            if(!panelContents.Contains(b.Control)){b.Control.Visibility=b.Node.Visible?Visibility.Visible:Visibility.Collapsed;b.Control.IsEnabled=b.Node.Enabled;}
            b.Context?.Refresh();
        }
        foreach(var refresh in shellRefresh)refresh();
        Tabs.Tag=TabHeadersVisible?"True":"False";
        if(Tabs.SelectedItem is not TabItem selected || selected.Visibility!=Visibility.Visible)Tabs.SelectedItem=Tabs.Items.OfType<TabItem>().FirstOrDefault(t=>t.Visibility==Visibility.Visible);
        externs.Visibility=externs.Children.Count==0?Visibility.Collapsed:Visibility.Visible;right.Visibility=right.Children.Count==0?Visibility.Collapsed:Visibility.Visible;
        RefreshLeft();
    }
    void RefreshLeft()=>left.Visibility=left.Children.Count>0 && (Tabs.SelectedItem as TabItem)?.Tag as string=="Tab_Run Params"?Visibility.Visible:Visibility.Collapsed;
    public void Dispose(){if(disposed)return;disposed=true;state.Changed-=StateChanged;}
}
