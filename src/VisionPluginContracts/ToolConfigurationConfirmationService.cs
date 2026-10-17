using System.Windows;
namespace VisionPluginContracts;
public sealed record ConfigurationConfirmation(string Caption,string Message);
public interface IToolConfigurationConfirmationService { bool Confirm(Window? owner,ConfigurationConfirmation request); }
public sealed class WpfConfigurationConfirmationService:IToolConfigurationConfirmationService {
 readonly Func<Window?,ConfigurationConfirmation,bool> show;
 public WpfConfigurationConfirmationService(Func<Window?,ConfigurationConfirmation,bool>? show=null){this.show=show??Show;}
 public bool Confirm(Window? owner,ConfigurationConfirmation request)=>show(owner,request);
 static bool Show(Window? owner,ConfigurationConfirmation request)=>(owner is null?
  System.Windows.MessageBox.Show(request.Message,request.Caption,MessageBoxButton.YesNo,MessageBoxImage.Question):
  System.Windows.MessageBox.Show(owner,request.Message,request.Caption,MessageBoxButton.YesNo,MessageBoxImage.Question))==MessageBoxResult.Yes;
}
