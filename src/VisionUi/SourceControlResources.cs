using System.Runtime.CompilerServices;using System.Windows;using System.Windows.Threading;
namespace VisionUi;
public static class SourceControlResources{static readonly ConditionalWeakTable<Dispatcher,ResourceDictionary> dictionaries=new();public static ResourceDictionary Current=>dictionaries.GetValue(Dispatcher.CurrentDispatcher,_=>new(){Source=new Uri("/VisionUi;component/SourceControlStyles.xaml",UriKind.Relative)});}
