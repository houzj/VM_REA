using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VisionFramework;
using VisionPluginContracts;
using Button = System.Windows.Controls.Button;
using HorizontalAlignment = System.Windows.HorizontalAlignment;

namespace VisionPlugins.Calibration;

/// <summary>Source-XML configuration model used by calibration tools that share the ordinary template host.</summary>
internal sealed class CalibrationToolConfigurationProvider(
    string moduleId,
    string displayName,
    ModuleDescriptor descriptor,
    ToolDisplayDefinition displayDefinition,
    string tabResource,
    Action<ToolEditorContext, IReadOnlyDictionary<string, JsonElement>> update,
    Func<ToolEditorContext, float[,]>? pointSnapshot = null,
    Action<ToolEditorContext>? clearPoints = null,
    Action<ToolEditorContext, string>? exportCalibration = null,
    string? algorithmResetResource = null,
    Func<ConfigurationOperationRequest,ConfigurationOperationResult?>? resetParameterWriter = null,
    Func<ToolEditorContext,IToolConfigurationDispatcher,ConfigurationOperationRequest,ConfigurationOperationResult?>? parameterWriter = null,
    Action<string>? calibrationPathError = null) : IToolConfigurationProvider
{
    const string Language = "Chinese";
    static readonly string[] PointKeys = ["ImagePointX", "ImagePointY", "WorldPointX", "WorldPointY", "DeltaAngle"];

    public ToolConfigurationSession CreateConfiguration(ToolEditorContext context)
    {
        string Resource(string name)
        {
            using var stream = typeof(CalibrationToolConfigurationProvider).Assembly.GetManifestResourceStream("VisionPlugins.Calibration.Resources." + name)
                ?? throw new InvalidDataException("Missing calibration configuration resource: " + name);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        var definition = ToolConfigurationDefinition.Parse(Resource(tabResource), Resource("ConfigurationGroups.xml"));
        var values = descriptor.Parameters.ToDictionary(parameter => parameter.Name, parameter => parameter.DefaultValue!.Value.Clone(), StringComparer.Ordinal);
        foreach (var pair in context.Node.Parameters) values[pair.Key] = pair.Value.Clone();
        ToolEditorContext Current() => context with { Node = context.Node with { Parameters = values.ToDictionary(p => p.Key, p => p.Value.Clone(), StringComparer.Ordinal) } };
        update(Current(), values);
        if (pointSnapshot is not null)
        {
            var snapshot = pointSnapshot(Current());
            for (var index = 0; index < 32; index++)
                for (var axis = 0; axis < PointKeys.Length; axis++)
                    if (values.ContainsKey(PointKeys[axis] + index))
                        values[PointKeys[axis] + index] = JsonSerializer.SerializeToElement((double)snapshot[index, axis]);
        }

        ToolConfigurationState? state = null;
        ToolConfigurationSession? session = null;
        Func<Window?> owner = () => null;
        var text = ConfigurationLocalization.ForLanguage(Language).Get;
        var files = context.ConfigurationFiles ?? new WpfConfigurationFileService();
        var saveFiles = context.ConfigurationSaveFiles ?? new WpfConfigurationSaveFileService();
        var notifications = context.ConfigurationNotifications ?? new WpfConfigurationNotificationService();
        ConfigurationExecutionHostStatus Host() => context.ConfigurationExecutionStatus?.Invoke() ?? new(false, false, false, false, false);
        void Notify(string message, bool error = false)
        {
            var result = notifications.Notify(owner(), new(error ? ConfigurationNotificationKind.Error : ConfigurationNotificationKind.Success, message, displayName, context.Mode));
            if (result.Status == ConfigurationNotificationStatus.Failed)
                state?.ReportDiagnostic(new("NotificationService", "Notify", null, result.Diagnostic ?? "Notification failed."));
        }
        void Apply(Dictionary<string, JsonElement> next)
        {
            var prepared = next.ToDictionary(p => p.Key, p => p.Value.Clone(), StringComparer.Ordinal);
            // Independent host transaction: rejected writes must not publish local parameter state.
            context.Apply(prepared.ToDictionary(p => p.Key, p => p.Value.Clone(), StringComparer.Ordinal));
            values = prepared;
            update(Current(), values);
        }
        Dictionary<string, string> UserData() => values.TryGetValue("ConfigurationUserData", out var raw)
            ? JsonSerializer.Deserialize<Dictionary<string, string>>(raw.GetString() ?? "{}") ?? []
            : [];
        var adapter = new ToolConfigurationParameterAdapter(descriptor, () => values, Apply,
            key => UserData().GetValueOrDefault(key), (key, value) =>
            {
                var user = UserData(); user[key] = value;
                var next = values.ToDictionary(p => p.Key, p => p.Value.Clone(), StringComparer.Ordinal);
                next["ConfigurationUserData"] = JsonSerializer.SerializeToElement(JsonSerializer.Serialize(user));
                Apply(next);
            });
        var execution = new ToolConfigurationExecution(
            () => context.Run(values.ToDictionary(p => p.Key, p => p.Value.Clone(), StringComparer.Ordinal)),
            () => Host().SolutionBusy, () => Host().Ready && Host().RunEnabled, () => Host().Loading, () => Host().ProcedureBusy);

        void Clear()
        {
            var next = values.ToDictionary(p => p.Key, p => p.Value.Clone(), StringComparer.Ordinal);
            foreach (var key in PointKeys)
                for (var index = 0; index < 32; index++)
                    if (next.ContainsKey(key + index)) next[key + index] = JsonSerializer.SerializeToElement(0d);
            if (next.TryGetValue("ClearRevision", out var revision)) next["ClearRevision"] = JsonSerializer.SerializeToElement(checked(revision.GetInt32() + 1));
            Apply(next);
            clearPoints?.Invoke(Current());
        }

        ConfigurationOperationResult Save()
        {
            if (exportCalibration is null) return new(true);
            var selected = saveFiles.SelectSave(owner(), new(moduleId,
                values.TryGetValue("CalibPathName", out var path) ? path.GetString() ?? "" : "",
                ".xml|*.xml", text("Create Calibration File"), ".xml", context.IsRemoteMode));
            if (selected.Status == ConfigurationFileStatus.Cancelled) return new(true);
            if (selected.Status != ConfigurationFileStatus.Selected || selected.Paths.Count != 1)
            {
                var message = selected.Diagnostic ?? selected.Status.ToString(); Notify(message, true); return new(true, Error: message);
            }
            try
            {
                var file = selected.Paths[0];
                if (ToolConfigurationCalibrationPathPolicy.ValidateSelectedSave(state!,file,text) is {} saveError) throw new InvalidDataException(saveError);
                exportCalibration(Current(), file);
                var next = values.ToDictionary(p => p.Key, p => p.Value.Clone(), StringComparer.Ordinal);
                if (next.ContainsKey("CalibPathName")) next["CalibPathName"] = JsonSerializer.SerializeToElement(file);
                var user = UserData(); user["SaveCalibPath"] = file;
                if (next.ContainsKey("ConfigurationUserData")) next["ConfigurationUserData"] = JsonSerializer.SerializeToElement(JsonSerializer.Serialize(user));
                Apply(next);
                var remembered = saveFiles.Remember(moduleId, file);
                if (!remembered.Success) state?.ReportDiagnostic(new("FileHistory", "Remember", null, remembered.Diagnostic ?? "History failed."));
                Notify(text("Lang_SaveSuccess")); return new(true);
            }
            catch (Exception error) when (error is InvalidDataException or IOException or InvalidOperationException or ArgumentException or UnauthorizedAccessException)
            {
                Notify(error.Message, true); return new(true, Error: error.Message);
            }
        }

        ConfigurationOperationResult Subscription(ConfigurationOperationRequest request)
        {
            var parameter = request.Kind is ConfigurationOperationKind.ReadParameterBinding or ConfigurationOperationKind.WriteParameterBinding or ConfigurationOperationKind.ClearParameterBinding;
            var query = ConfigurationSubscriptionValidation.FromElement(request.Source.Element, parameter ? ConfigurationSubscriptionDomain.Parameter : null);
            if (query is null || context.ConfigurationSubscriptions is null) return new(true, Error: "Source-declared subscription service is unavailable.");
            var write = request.Kind is ConfigurationOperationKind.WriteInputBinding or ConfigurationOperationKind.WriteParameterBinding or ConfigurationOperationKind.ClearParameterBinding;
            var result = write
                ? context.ConfigurationSubscriptions.Bind(query.Destination, request.Kind == ConfigurationOperationKind.ClearParameterBinding ? null : request.SubscriptionBinding)
                : context.ConfigurationSubscriptions.Read(query.Destination);
            if (result.Status != ConfigurationSubscriptionStatus.Available) return new(true, Error: result.Diagnostic ?? result.Status.ToString());
            if (state is not null && !state.ApplySubscription(request.Source, result, parameter)) return new(true, Error: "Subscription readback was not authoritative.");
            return new(true, result.DisplayText ?? "");
        }

        var dispatcher = new CalibrationDispatcher(request =>
        {
            if (request.Kind is ConfigurationOperationKind.ReadInputBinding or ConfigurationOperationKind.WriteInputBinding or ConfigurationOperationKind.ReadParameterBinding or ConfigurationOperationKind.WriteParameterBinding or ConfigurationOperationKind.ClearParameterBinding)
                return Subscription(request);
            if (request.Kind is ConfigurationOperationKind.SaveCalibration or ConfigurationOperationKind.SaveFile) return Save();
            if (request.Kind is ConfigurationOperationKind.Clear or ConfigurationOperationKind.ClearCalibrationPoints) { Clear(); return new(true); }
            if (request.Kind == ConfigurationOperationKind.Close) { session?.RequestClose(); return new(true); }
            if (request.Kind == ConfigurationOperationKind.Run)
            {
                if (Host().Loading || Host().SolutionBusy || !Host().Ready || !Host().RunEnabled) return new(true, Error: "Configuration execution is not ready.");
                _ = execution.RunOnceAsync(); return new(true);
            }
            if (request.Kind == ConfigurationOperationKind.RunContinuous) return new(true, Error: execution.StartContinuous() == ConfigurationRunRejection.None ? null : "Continuous execution is not available.");
            if (request.Kind == ConfigurationOperationKind.StopContinuous) { _ = execution.StopAsync(); return new(true); }
            if (request.Kind == ConfigurationOperationKind.ResetParameters)
            {
                var confirmation = context.ConfigurationConfirmations ?? new WpfConfigurationConfirmationService();
                if (!confirmation.Confirm(owner(), new(text("Lang_Sure"), text("Lang_ResetAlgoParamTips") + "?"))) return new(true);
                if (algorithmResetResource is not null)
                {
                    void SynchronizeValues(){if(context.ReadEffectiveParameters?.Invoke() is {} current)foreach(var pair in current)values[pair.Key]=pair.Value.Clone();}
                    SynchronizeValues();
                    var resetParameters=new CalibrationDispatcher(operation => resetParameterWriter?.Invoke(operation) ?? adapter.Dispatch(operation));
                    var result=ToolConfigurationAlgorithmReset.Execute(descriptor,state!,resetParameters,context.ConfigurationSubscriptions,Resource(algorithmResetResource),SynchronizeValues,adapter.WriteParameter);
                    Notify(result.Error ?? text("Lang_ResetAlgoParamSuccess"),result.Error is not null);return result;
                }
                if (context.ResetConfigurationParameters is null) return new(true, Error: "Atomic parameter reset service is unavailable.");
                var next = descriptor.Parameters.Where(p => p.Name != "ConfigurationUserData" && p.Name != ToolDisplayService.SettingsParameter)
                    .ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone(), StringComparer.Ordinal);
                foreach (var item in values.Where(p => !next.ContainsKey(p.Key))) next[item.Key] = item.Value.Clone();
                context.ResetConfigurationParameters(next.ToDictionary(p => p.Key, p => p.Value.Clone(), StringComparer.Ordinal));
                Apply(next); foreach (var node in state!.Nodes) if (node.Name is { } name && next.ContainsKey(name)) state.RefreshParameter(node);
                Notify(text("Lang_ResetAlgoParamSuccess")); return new(true);
            }
            if (request.Kind == ConfigurationOperationKind.ReadParameter && !descriptor.Parameters.Any(p => p.Name == request.Source.Name)) return new(true, request.Source.Value);
            return parameterWriter?.Invoke(Current(),adapter,request) ?? adapter.Dispatch(request);
        });

        state = new(definition, dispatcher) { ExecutionEnabled = Host().RunEnabled };
        ToolConfigurationCalibrationPathPolicy.Attach(state,context,owner,text,displayName,calibrationPathError??(message=>Notify(message,true)));
        var controls = new ToolConfigurationControlRegistry();
        ToolConfigurationStandardControls.Register(controls, text,
            () => new(execution.IsContinuous, Host().ProcedureBusy, Host().RunEnabled), Language, context.ConfigurationSubscriptions);
        ToolConfigurationSubscriptionControls.Register(controls, context.ConfigurationSubscriptions, text, Language);
        if (moduleId == RotateCalibrationPlugin.Id)
            ToolConfigurationCalibrationPathControls.RegisterPreservingRejectedText(controls, control =>
            {
                owner = control.Owner;
                var selected = files.Select(owner(), new(moduleId, control.Node.Value, ".xml|*.xml;*.iwcal", text("Open"), false, context.IsRemoteMode, false));
                if (selected.Status == ConfigurationFileStatus.Selected && selected.Paths.Count == 1) return selected.Paths[0];
                if (selected.Status is ConfigurationFileStatus.Failed or ConfigurationFileStatus.Unsupported) Notify(selected.Diagnostic ?? selected.Status.ToString(), true);
                return null;
            }, null, text, context.ConfigurationSubscriptions, selectionCommitted:(control,path)=>files.Remember(moduleId,path));
        else
            ToolConfigurationCalibrationPathControls.Register(controls, control =>
            {
                owner = control.Owner;
                var selected = files.Select(owner(), new(moduleId, control.Node.Value, ".xml|*.xml", text("Open"), false, context.IsRemoteMode, false));
                if (selected.Status == ConfigurationFileStatus.Selected && selected.Paths.Count == 1)
                {
                    state.CommitUser(control.Node, selected.Paths[0], true);
                    files.Remember(moduleId, selected.Paths[0]);
                }
                else if (selected.Status is ConfigurationFileStatus.Failed or ConfigurationFileStatus.Unsupported) Notify(selected.Diagnostic ?? selected.Status.ToString(), true);
            }, text, context.ConfigurationSubscriptions);
        controls.Register("SaveFileDialog", control =>
        {
            var button = new Button { Content = text(control.Node.CaptionKey), HorizontalAlignment = HorizontalAlignment.Right, MinWidth = 112, Height = 30 };
            button.Click += (_, _) => state.Invoke(control.Node);
            control.OnRefresh(() => { button.Content = text(control.Node.CaptionKey); button.IsEnabled = control.Node.Enabled; });
            return button;
        });
        if (pointSnapshot is not null)
        {
            controls.Register("EditButton_Calib", control =>
            {
                var row = new Grid { Height = 32 };
                row.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/VisionPluginContracts;component/Resources/ConfigurationControls.xaml", UriKind.Relative) });
                row.ColumnDefinitions.Add(new() { Width = new GridLength(89) }); row.ColumnDefinitions.Add(new());
                var label = new TextBlock { FontSize = 14, Foreground = (System.Windows.Media.Brush)row.FindResource("ConfigurationControlForeground"), VerticalAlignment = VerticalAlignment.Center, TextTrimming = TextTrimming.CharacterEllipsis };
                row.Children.Add(label);
                var editor = new Grid(); editor.ColumnDefinitions.Add(new()); editor.ColumnDefinitions.Add(new() { Width = GridLength.Auto }); Grid.SetColumn(editor, 1); row.Children.Add(editor);
                var spinner = new ConfigurationNumericSpinner(false) { Tag = control.Node };
                spinner.ValueCommitted += value => { control.State.CommitUser(control.Node, value); spinner.SetText(control.Node.Value); };
                editor.Children.Add(spinner);
                var edit = new Button { Width = 32, Height = 32, Margin = new Thickness(4, 0, 0, 0), ToolTip = "编辑标定点", Name = "CalibrationEditPoints", Template = NPointConfigurationControls.IconTemplate("edit") };
                edit.Click += (_, _) =>
                {
                    owner = control.Owner;
                    var data = values.ToDictionary(p => p.Key, p => p.Value.Clone(), StringComparer.Ordinal);
                    var points = pointSnapshot(Current());
                    for (var i = 0; i < 32; i++) for (var axis = 0; axis < PointKeys.Length; axis++) data[PointKeys[axis] + i] = JsonSerializer.SerializeToElement((double)points[i, axis]);
                    new TranslationTableWindow(owner() ?? new Window(), data, () => Apply(data), Clear).ShowDialog();
                };
                Grid.SetColumn(edit, 1); editor.Children.Add(edit);
                control.OnRefresh(() => { label.Text = text(control.Node.CaptionKey); label.ToolTip = text(control.Node.Get("Description") ?? control.Node.CaptionKey); spinner.Minimum = Number(control.Node.Get("MinValue"), 0); spinner.Maximum = Number(control.Node.Get("MaxValue"), 0); spinner.Increment = Number(control.Node.Get("IncValue"), 1); spinner.DecimalDigits = (int)Number(control.Node.Get("DecimalDigits"), 0); spinner.SetText(control.Node.Value); spinner.IsReadOnly = !control.Node.Enabled || !((control.Node.Get("AccessMode") ?? "").Contains('W', StringComparison.OrdinalIgnoreCase) || (control.Node.Get("AccessMode") ?? "").Contains('O', StringComparison.OrdinalIgnoreCase)); edit.IsEnabled = control.Node.Enabled; });
                return row;
            });
        }
        ToolConfigurationObjectControls.Register(controls, new ToolDisplayConfigurationObjectService(
            () => ToolDisplayService.Read(values, displayDefinition), settings =>
            {
                var next = values.ToDictionary(p => p.Key, p => p.Value.Clone(), StringComparer.Ordinal);
                next[ToolDisplayService.SettingsParameter] = ToolDisplayService.Serialize(settings); Apply(next); return true;
            }), text, colorPicker: (initial, selected) => ToolConfigurationObjectControls.CreateColorTabs(initial,
                value => selected(value.Length == 9 ? "#" + value[3..] : value),
                callback => { var palette = new VisionUi.SourceColorPalette(); palette.Selected += callback; return palette; },
                (color, callback) => { var picker = new VisionUi.SourceAdvancedColorPicker { SelectedCurrentColor = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(color)) }; picker.ColorChanged += brush => callback(brush.ToString()); return picker; },
                new Uri("/VisionUi;component/SourceColorPickerStyles.xaml", UriKind.Relative), text));
        void Changed() { state.IsBusy = execution.IsRunning; }
        execution.Changed += Changed;
        session = new(state, controls, Language, text, () => { execution.Changed -= Changed; _ = execution.DisposeAsync(); });
        state.Initialize();
        return session;
    }

    sealed class CalibrationDispatcher(Func<ConfigurationOperationRequest, ConfigurationOperationResult> dispatch) : IToolConfigurationDispatcher
    {
        public ConfigurationOperationResult Dispatch(ConfigurationOperationRequest request) => dispatch(request);
    }

    static double Number(string? value, double fallback) => double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var number) && double.IsFinite(number) ? number : fallback;
}
