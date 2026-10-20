using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;
using W = System.Windows.Controls;

public static class UiChecks
{
    public static int Run(TranslationPlugin plugin, string root)
    {
        var checks = 0;
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                if (System.Windows.Application.Current is null) _ = new System.Windows.Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
                var values = plugin.Descriptor.Parameters.ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone());
                var node = new NodeDocument("translation-ui", TranslationPlugin.Id, "1.0.0", "平移旋转标定", values);
                var context = new ToolEditorContext(node, root, p => values = p,
                    _ => Task.FromResult(new NodeRunRecord("translation-ui", NodeRunStatus.Success, new Dictionary<string, PortValue>(), 0)))
                { ConfigurationExecutionStatus = () => new(false, false, true, false, true) };
                var session = plugin.CreateConfiguration(context);
                var window = new ToolConfigurationModelWindow(context, session)
                { Left = -30000, Top = -30000, ShowInTaskbar = false, ShowActivated = false };
                window.Show(); window.UpdateLayout();
                var pointValues = values.ToDictionary(p => p.Key, p => p.Value.Clone());
                var pointEditor = new TranslationTableWindow(window, pointValues, () => { }, () => { });
                Check(pointEditor.Width == 520 && pointEditor.Height == 399 && pointEditor.FontSize == 14, "Point editor uses NPointListWindow/NPointList source dimensions and font");

                IEnumerable<FrameworkElement> Elements(DependencyObject item)
                {
                    if (item is FrameworkElement element) yield return element;
                    for (var i = 0; i < VisualTreeHelper.GetChildrenCount(item); i++)
                        foreach (var child in Elements(VisualTreeHelper.GetChild(item, i))) yield return child;
                }
                void Check(bool ok, string message) { checks++; if (!ok) throw new Exception(message); }

                var clearOuter = pointValues.ToDictionary(p => p.Key, p => p.Value.Clone());
                foreach (var key in new[] { "ImagePointX", "ImagePointY", "WorldPointX", "WorldPointY", "DeltaAngle" }) clearOuter[key + "0"] = JsonSerializer.SerializeToElement(42d);
                var clearView = clearOuter.ToDictionary(p => p.Key, p => p.Value.Clone());
                var clearEditor = new TranslationTableWindow(window, clearView, () => { }, () =>
                {
                    foreach (var key in new[] { "ImagePointX", "ImagePointY", "WorldPointX", "WorldPointY", "DeltaAngle" }) clearOuter[key + "0"] = JsonSerializer.SerializeToElement(0d);
                });
                clearEditor.Show(); clearEditor.UpdateLayout();
                var clearButton = Elements(clearEditor).OfType<W.Button>().Single(button => button.Content?.ToString() == "清空标定点");
                clearButton.RaiseEvent(new RoutedEventArgs(W.Button.ClickEvent));
                var clearedRows = ((W.DataGrid)typeof(TranslationTableWindow).GetField("table", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(clearEditor)!).Items.Cast<TranslationTableWindow.PointRow>().ToArray();
                Check(clearedRows.Length > 0 && clearedRows[0].ImageX == "0.000" && clearedRows[0].WorldY == "0.000"
                    && clearOuter["ImagePointX0"].GetDouble() == 0 && clearView["ImagePointX0"].GetDouble() == 0,
                    "Clear callback updates both the host parameter snapshot and the point editor's captured dictionary before reloading rows");
                clearEditor.Close();

                var pointGrid = (W.DataGrid)typeof(TranslationTableWindow).GetField("table", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(pointEditor)!;
                var import = typeof(TranslationTableWindow).GetMethod("ImportPointText", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
                import.Invoke(pointEditor, ["1.23456 2 3 4 5\r\n6 7 8 9 10\r\n"]);
                Check(pointGrid.Items.Count == 2 && pointValues["CalibPointTotalNum"].GetInt32() == 9 && ((TranslationTableWindow.PointRow)pointGrid.Items[0]).ImageX == "1.23456", "A successful short import replaces the source point-list collection, preserves raw imported text and leaves configured point counts unchanged");
                bool malformedImport = false;
                try { import.Invoke(pointEditor, ["9 8 7 6 5\r\ninvalid 2 3 4 5\r\n"]); }
                catch (System.Reflection.TargetInvocationException error) when (error.InnerException is InvalidDataException) { malformedImport = true; }
                Check(malformedImport && pointValues["ImagePointX0"].GetDouble() == 9 && pointGrid.Items.Count == 2 && ((TranslationTableWindow.PointRow)pointGrid.Items[0]).ImageX == "1.23456", "A later malformed import row preserves original partial parameter writes but leaves the previously assigned table collection visible");
                bool shortRowImport = false;
                try { import.Invoke(pointEditor, ["11 12 13 14\r\n"]); }
                catch (System.Reflection.TargetInvocationException error) when (error.InnerException is IndexOutOfRangeException) { shortRowImport = true; }
                Check(shortRowImport && pointValues["ImagePointX0"].GetDouble() == 11 && pointValues["ImagePointY0"].GetDouble() == 12 && pointValues["WorldPointX0"].GetDouble() == 13 && pointValues["WorldPointY0"].GetDouble() == 14 && pointValues["DeltaAngle0"].GetDouble() == 5, "A numeric short row writes its first four property setters in source order before missing DeltaAngle throws");
                Check(pointGrid.Items.Count == 2 && ((TranslationTableWindow.PointRow)pointGrid.Items[0]).ImageX == "1.23456", "A short-row exception leaves the old point-list collection assigned to the UI");
                import.Invoke(pointEditor, [""]);
                Check(pointGrid.Items.Count == 0, "A successful empty import assigns the source empty point-list collection");

                var commitValues = pointValues.ToDictionary(p => p.Key, p => p.Value.Clone());
                var appliedPointWrites = 0;
                var commitEditor = new TranslationTableWindow(window, commitValues, () => appliedPointWrites++, () => { });
                var commitImport = typeof(TranslationTableWindow).GetMethod("ImportPointText", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
                commitImport.Invoke(commitEditor, ["10 11 12 13 14\r\n61 72 83 94 105\r\n"]);
                appliedPointWrites = 0;
                var rowsForCommit = ((W.DataGrid)typeof(TranslationTableWindow).GetField("table", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(commitEditor)!).Items
                    .Cast<TranslationTableWindow.PointRow>().ToArray();
                rowsForCommit[0].ImageX = "20"; rowsForCommit[0].ImageY = "21"; rowsForCommit[0].WorldX = "22"; rowsForCommit[0].WorldY = "23"; rowsForCommit[0].Angle = "24";
                rowsForCommit[1].ImageX = "62"; rowsForCommit[1].ImageY = "73"; rowsForCommit[1].WorldX = "84";
                rowsForCommit[1].WorldY = "not-a-number";
                bool partialCommit = false;
                try { typeof(TranslationTableWindow).GetMethod("Commit", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.Invoke(commitEditor, null); }
                catch (System.Reflection.TargetInvocationException error) when (error.InnerException is FormatException) { partialCommit = true; }
                Check(partialCommit && appliedPointWrites == 8
                    && commitValues["ImagePointX1"].GetDouble() == 62 && commitValues["ImagePointY1"].GetDouble() == 73
                    && commitValues["WorldPointX1"].GetDouble() == 84 && commitValues["DeltaAngle1"].GetDouble() == 105,
                    "Point property commits publish each successful setter in row/field order; a later invalid field leaves the successful prefix applied and later fields untouched");
                commitEditor.Close();

                var view = window.ConfigurationView;
                Check(view.DesiredConfigurationSize.Width == 420 && view.DesiredConfigurationSize.Height == 500, "Uses the original AlgorithmTabRoot 420×500 Chinese-window defaults");
                Check(view.Tabs.Items.Count == 2 && ((W.TabItem)view.Tabs.Items[1]).Header?.ToString() == "结果显示", "Uses original basic and ResultShow pages");
                Check(view.Diagnostics.Count == 0, "Every source-declared control resolves through shared template registry: " + string.Join("; ", view.Diagnostics.Select(d => d.Code + "/" + d.TemplateKey + "/" + d.Node)));
                Check(session.State.Find("CalibType")?.Value == "0" && session.State.Find("UnionCalibEnable")?.Visible == false, "Default translation mode follows original visibility triggers");
                Check(session.State.Find("CalibPointTotalNum")?.Element.Tag == "EditButton_Calib", "Point count keeps its source-declared EditButton_Calib template");
                Check(Elements(window).Any(e => e.Name == "CalibrationEditPoints"), "Point table editor is hosted by the source template");
                Check(session.State.Find("PicPoint")?.Visible == true && session.State.Find("PicPointX")?.Visible == false, "Default point input follows original POINT selector trigger");
                Check(ConfigurationSubscriptionValidation.FromElement(session.State.Find("PicPoint")!.Element)?.Destination == new ConfigurationSubscriptionDestination(ConfigurationSubscriptionDomain.Combination, "Image Point", "POINT"), "Point selector query comes from source template metadata");

                session.State.CommitUser("CalibType", "1"); window.UpdateLayout();
                Check(session.State.Find("UnionCalibEnable")?.Visible == true && session.State.Find("RotPointTotalNum")?.Visible == true, "Rotation fields follow original CalibType triggers");
                session.State.CommitUser("CalibPointInput", "1"); window.UpdateLayout();
                Check(session.State.Find("PicPoint")?.Visible == false && session.State.Find("PicPointX")?.Visible == true && session.State.Find("PicPointY")?.Visible == true, "Coordinate mode follows original visibility setters");
                session.State.CommitUser("CalibPointGet", "1"); window.UpdateLayout();
                Check(session.State.Find("TeachEnable")?.Visible == false && session.State.Find("PicPointX")?.Visible == false, "Manual entry follows original acquisition trigger");

                view.Tabs.SelectedIndex = 1; window.UpdateLayout();
                var controls = Elements(window).ToArray();
                var displayObjectFields = new[] { "Content", "OKColor", "NGColor", "CharacterSize", "Transparency" };
                var displayPositionFields = new[] { "PositionX", "PositionY" };
                Check(displayObjectFields.All(name => session.State.Find(name) is { } field && ConfigurationObjectRoute.Parse(field) is not null)
                    && displayPositionFields.All(name => session.State.Find(name) is { } field && ConfigurationSubscriptionValidation.FromElement(field.Element) is not null), "ResultShow preserves the source-declared shared TextDisplayGroup object and subscription routes");
                Check(controls.OfType<W.TextBox>().Any(e => !e.IsReadOnly), "ResultShow renders the source object-backed Content editor through the shared control template");
                var bitmap = new RenderTargetBitmap((int)window.ActualWidth, (int)window.ActualHeight, 96, 96, PixelFormats.Pbgra32); bitmap.Render(window);
                var encoder = new PngBitmapEncoder(); encoder.Frames.Add(BitmapFrame.Create(bitmap)); Directory.CreateDirectory(Path.Combine(root, "analysis/full-audit/translation-calibration/checks"));
                using (var file = File.Create(Path.Combine(root, "analysis/full-audit/translation-calibration/checks/template-model-window.png"))) encoder.Save(file);
                window.Close();
            }
            catch (Exception error) { failure = error; }
        });
        thread.SetApartmentState(ApartmentState.STA); thread.Start(); thread.Join();
        if (failure is not null) throw failure;
        return checks;
    }
}
