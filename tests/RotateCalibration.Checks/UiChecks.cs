using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;
using W = System.Windows.Controls;

public static class UiChecks
{
    public static int Run(string root)
    {
        var checks = 0;
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                if (Application.Current is null) _ = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
                var plugin = new RotateCalibrationPlugin();
                var values = plugin.Descriptor.Parameters.ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone());
                var node = new NodeDocument("rotate", RotateCalibrationPlugin.Id, "1.0.0", "旋转标定", values);
                var imageBinding = new ConfigurationSubscriptionBinding(new ConfigurationOutputReference("flow", "image-node", "image-port", "IMAGE"), ConfigurationSubscriptionIndex.All);
                var subscriptions = new ImageSubscriptionService(new ConfigurationSubscriptionTreeNode("image-source", "上一图像源", binding: imageBinding));
                var notifications = new NotificationService();
                var files = new FileService();
                var context = new ToolEditorContext(node, root, p => values = p,
                    _ => Task.FromResult(new NodeRunRecord("rotate", NodeRunStatus.Success, new Dictionary<string, PortValue>(), 0)))
                { ConfigurationSubscriptions = subscriptions, ConfigurationNotifications = notifications, ConfigurationFiles = files };
                var session = plugin.CreateConfiguration(context);
                var window = new ToolConfigurationModelWindow(context, session)
                { Left = -30000, Top = -30000, ShowInTaskbar = false, ShowActivated = false };
                window.Show(); window.UpdateLayout();

                IEnumerable<FrameworkElement> Elements(DependencyObject item)
                {
                    if (item is FrameworkElement element) yield return element;
                    for (var i = 0; i < VisualTreeHelper.GetChildrenCount(item); i++)
                        foreach (var child in Elements(VisualTreeHelper.GetChild(item, i))) yield return child;
                }
                void Check(bool ok, string message) { checks++; if (!ok) throw new Exception(message); }

                var view = window.ConfigurationView;
                Check(view.DesiredConfigurationSize.Width == 420 && view.DesiredConfigurationSize.Height == 500, "Uses the original AlgorithmTabRoot 420×500 Chinese-window defaults");
                Check(view.Tabs.Items.Count == 2 && ((W.TabItem)view.Tabs.Items[1]).Header?.ToString() == "结果显示", "Uses the original Tab_Basic Params and ResultShow pages");
                Check(view.Diagnostics.Count == 0, "Every source-declared rotate control resolves through the shared template registry: " + string.Join("; ", view.Diagnostics.Select(d => d.Code + "/" + d.TemplateKey + "/" + d.Node)));
                Check(session.State.Find("InputWay")?.Value == "0" && session.State.Find("ImagePoints")?.Visible == true, "Default point mode is evaluated by the source model");
                Check(ConfigurationSubscriptionValidation.FromElement(session.State.Find("ImagePoints")!.Element)?.Destination == new ConfigurationSubscriptionDestination(ConfigurationSubscriptionDomain.Combination, "Image Point", "POINT"), "POINT selector identity comes from original control declaration");

                var combos = Elements(window).OfType<W.ComboBox>().ToArray();
                Check(combos.Length == 1, "Original ImageSourceGroup uses the generic image enumeration template");
                combos[0].IsDropDownOpen = true; window.UpdateLayout();
                Check(subscriptions.LastQuery?.Destination == new ConfigurationSubscriptionDestination(ConfigurationSubscriptionDomain.Combination, "InputImage", "IMAGE") && subscriptions.LastQuery.NativeTypes.SequenceEqual(new[] { "IMAGE" }), "Original ImageSourceGroup enumerates IMAGE combinations");
                combos[0].SelectedIndex = 0;
                Check(subscriptions.Last == imageBinding, "Image source selection stores typed combination identity");
                Check(session.State.Find("ImagePointXs")?.Visible == false && session.State.Find("ImagePointYs")?.Visible == false, "Coordinate controls follow the original InputWay trigger");
                session.State.CommitUser("InputWay", "1"); window.UpdateLayout();
                Check(session.State.Find("ImagePoints")?.Visible == false && session.State.Find("ImagePointXs")?.Visible == true && session.State.Find("ImagePointYs")?.Visible == true, $"Coordinate mode follows source visibility setters; InputWay={session.State.Find("InputWay")?.Value}, ImagePoints={session.State.Find("ImagePoints")?.Visible}/{session.State.Find("ImagePoints")?.Get("CustomVisible")}, X={session.State.Find("ImagePointXs")?.Visible}/{session.State.Find("ImagePointXs")?.Get("CustomVisible")}, Y={session.State.Find("ImagePointYs")?.Visible}/{session.State.Find("ImagePointYs")?.Get("CustomVisible")}, diagnostics={string.Join(";",session.State.Diagnostics.Select(d=>d.Code+":"+d.Message))}");
                var userData = JsonSerializer.Deserialize<Dictionary<string, string>>(values["ConfigurationUserData"].GetString() ?? "{}") ?? [];
                Check(userData.GetValueOrDefault("Param.InputWay") == "1", "Source RadioSelecter persists InputWay under IUserStringData key Param.InputWay");
                Check(session.State.Find("RevOutputFlag") is not null && session.State.Find("FilePath") is not null && session.State.Find("RefreshSignal") is not null, "Original file and signal fields remain in the shared model");
                var views = Elements(window).ToArray();
                var pathInput = views.OfType<W.TextBox>().Single(e => e.Name == "RotateCalibrationPathInput");
                string? pathAtNotification = null; notifications.OnNotify = _ => pathAtNotification = pathInput.Text;
                var invalidInput = Path.Combine(root, "uppercase.XML"); pathInput.Text = invalidInput; pathInput.RaiseEvent(new RoutedEventArgs(UIElement.LostFocusEvent));
                Check(session.State.Find("FilePath")?.Value == "" && pathInput.Text == invalidInput && notifications.Last?.Kind == ConfigurationNotificationKind.Error, "Invalid typed .XML remains visible in the original control while SetParam is rejected with an error");
                var previousNotification = notifications.Count; pathInput.RaiseEvent(new RoutedEventArgs(UIElement.LostFocusEvent));
                Check(notifications.Count == previousNotification, "An unchanged rejected CurValue is not re-submitted on every focus loss");
                var choosePath = views.OfType<W.Button>().Single(e => e.Name == "ChooseCalibrationPath");
                pathAtNotification = null; files.NextPath = Path.Combine(root, "uppercase.IWCAL"); choosePath.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                Check(session.State.Find("FilePath")?.Value == "" && pathInput.Text == files.LastSelected && pathAtNotification == files.LastSelected, "Picker candidate is displayed before synchronous rejection notification, remains visible, and is not written");
                Check(files.LastRequest is { Filter: ".xml|*.xml;*.iwcal", Multiselect: false, CheckFileExists: false, CheckPathExists: true }, "Win32 file picker request preserves original filter and existence flags");
                var lowerPath = Path.Combine(root, "lowercase.xml"); files.NextPath = lowerPath; choosePath.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                Check(session.State.Find("FilePath")?.Value == lowerPath && pathInput.Text == lowerPath, "Original lowercase .xml path passes file-picker validation");
                var lowerBinaryPath = Path.Combine(root, "lowercase.iwcal"); files.NextPath = lowerBinaryPath; choosePath.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                Check(session.State.Find("FilePath")?.Value == lowerBinaryPath && pathInput.Text == lowerBinaryPath, "Original lowercase .iwcal path passes file-picker validation");

                var errorDialog = RotateCalibrationFileErrorDialog.CreateWindow("文件打开失败", "确定");
                errorDialog.Left = -30000; errorDialog.Top = -30000; errorDialog.ShowActivated = false;
                Check(errorDialog.Title == "" && errorDialog.Width == 290 && errorDialog.Height == 156 && errorDialog.Topmost && !errorDialog.ShowInTaskbar && errorDialog.WindowStyle == WindowStyle.None && errorDialog.ResizeMode == ResizeMode.NoResize, "Rotate-only error window carries the source-confirmed empty-title, size, topmost, taskbar and fixed-window settings");
                bool modalVisible = false; bool sourceTextAndIcon = false; bool oneActionButton = false;
                var closeAfterInspect = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
                closeAfterInspect.Tick += (_, _) =>
                {
                    closeAfterInspect.Stop();
                    var dialogElements = Elements(errorDialog).ToArray();
                    modalVisible = errorDialog.IsVisible;
                    var message = dialogElements.OfType<W.TextBlock>().SingleOrDefault(e => e.Name == "MessageText");
                    var errorIcon = dialogElements.OfType<W.Image>().SingleOrDefault(e => e.Name == "ErrorIcon");
                    sourceTextAndIcon = message?.Text == "文件打开失败" && errorIcon?.Source?.ToString().EndsWith("/RotateCalibrationError.png", StringComparison.Ordinal) == true;
                    oneActionButton = dialogElements.OfType<W.Button>().Count(e => e.Name == "ConfirmButton") == 1 && dialogElements.OfType<W.Button>().SingleOrDefault(e => e.Name == "ConfirmButton")?.Content?.ToString() == "确定";
                    dialogElements.OfType<W.Button>().Single(e => e.Name == "ConfirmButton").RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                };
                closeAfterInspect.Start(); errorDialog.ShowDialog();
                Check(modalVisible && errorDialog.DialogResult == true && errorDialog.Owner is null, "STA rejection uses an ownerless blocking ShowDialog path and closes through the OK command");
                Check(sourceTextAndIcon && oneActionButton, "Rotate error dialog presents the localized source message, original error image asset and single OK action");
                var bitmap = new System.Windows.Media.Imaging.RenderTargetBitmap((int)window.ActualWidth, (int)window.ActualHeight, 96, 96, System.Windows.Media.PixelFormats.Pbgra32);
                bitmap.Render(window);
                var encoder = new System.Windows.Media.Imaging.PngBitmapEncoder(); encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bitmap));
                var output = Path.Combine(root, "analysis/full-audit/rotate-calibration/checks"); Directory.CreateDirectory(output);
                using (var file = File.Create(Path.Combine(output, "template-model-window.png"))) encoder.Save(file);
                Check(views.OfType<W.TabControl>().Single() == view.Tabs, "Displayed tabs are the common ToolConfigurationView instance");
                window.Close();
            }
            catch (Exception error) { failure = error; }
        });
        thread.SetApartmentState(ApartmentState.STA); thread.Start(); thread.Join();
        if (failure is not null) throw failure;
        return checks;
    }

    sealed class ImageSubscriptionService(ConfigurationSubscriptionTreeNode leaf) : IToolConfigurationSubscriptionService
    {
        public ConfigurationSubscriptionQuery? LastQuery;
        public ConfigurationSubscriptionBinding? Last;
        public ConfigurationSubscriptionTreeResult Enumerate(ConfigurationSubscriptionQuery query) { LastQuery = query; return new(ConfigurationSubscriptionStatus.Available, [leaf]); }
        public ConfigurationSubscriptionBindingResult Read(ConfigurationSubscriptionDestination destination) => new(ConfigurationSubscriptionStatus.Available, Last, Last is null ? null : "source image");
        public ConfigurationSubscriptionBindingResult Bind(ConfigurationSubscriptionDestination destination, ConfigurationSubscriptionBinding? binding) { Last = binding; return new(ConfigurationSubscriptionStatus.Available, binding, binding is null ? null : "source image"); }
    }

    sealed class NotificationService : IToolConfigurationNotificationService
    {
        public ConfigurationNotification? Last;
        public int Count;
        public Action<ConfigurationNotification>? OnNotify;
        public ConfigurationNotificationResult Notify(Window? owner, ConfigurationNotification notification)
        { Last = notification; Count++; OnNotify?.Invoke(notification); return new(ConfigurationNotificationStatus.Displayed); }
    }

    sealed class FileService : IToolConfigurationFileService
    {
        public string? NextPath;
        public string? LastSelected;
        public ConfigurationFileRequest? LastRequest;
        public ConfigurationFileSelection Select(Window? owner, ConfigurationFileRequest request)
        { LastRequest = request; var selected = NextPath; NextPath = null; LastSelected = selected; return selected is null ? new(ConfigurationFileStatus.Cancelled, []) : new(ConfigurationFileStatus.Selected, [selected]); }
        public ConfigurationFileHistoryResult Remember(string moduleKey, string selectedFile) => new(true, Path.GetDirectoryName(selectedFile));
    }
}
