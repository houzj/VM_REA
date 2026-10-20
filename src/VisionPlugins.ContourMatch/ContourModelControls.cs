using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using VisionPluginContracts;
using Button = System.Windows.Controls.Button;
using CheckBox = System.Windows.Controls.CheckBox;
using TextBox = System.Windows.Controls.TextBox;
namespace VisionPlugins.ContourMatch;

// Normal model list, not the full training dialog. No invented training result.
public static class ContourModelControls
{
    public static void Register(ToolConfigurationControlRegistry registry,ContourModelCollection models,
        Func<bool> isRunning,Action persist,Func<string,bool> confirm,Action<string> notify,
        Action build,Action load,Action<int> edit)
    {
        var storage=models.Storage;
        registry.Register("FeaturepatMatchConfigPanalData",context=>{
            var panel=new StackPanel();var toolbar=new WrapPanel();var list=new StackPanel();
            void Run(Action action){if(isRunning()){notify("Process is running; stop before editing models.");return;}try{action();Refresh();}catch(Exception e)when(e is InvalidOperationException or NotSupportedException or ArgumentException){notify(e.Message);}}
            Button Button(string name,string caption,Action action){var button=new Button{Name=name,Content=caption,Margin=new Thickness(2)};button.Click+=(_,_)=>Run(action);return button;}
            void Ensure(uint error){if(error!=0)throw new InvalidOperationException($"Contour model operation failed: {error:X8}");}
            void Refresh(){
                list.Children.Clear();
                foreach(var item in storage.Snapshot()){
                    int index=item.Slot;var row=new WrapPanel();var checkbox=new CheckBox{Name="select",IsChecked=item.Value.Selected,VerticalAlignment=VerticalAlignment.Center};
                    // Managed text presentation is explicit UTF8; the storage
                    // itself preserves the original 260 raw name bytes.
                    var bytes=item.Value.Name;int end=Array.IndexOf(bytes,(byte)0);if(end<0)end=bytes.Length;
                    var name=new TextBox{Name="ModelName",Text=Encoding.UTF8.GetString(bytes,0,end),Width=140,MaxLength=63};
                    checkbox.Click+=(_,_)=>Run(()=>{Ensure(storage.Select(index,checkbox.IsChecked==true?1:0));persist();});
                    name.LostFocus+=(_,_)=>{
                        if(name.Text.Length>32){notify("Model name length exceeds 32.");return;}
                        if(string.IsNullOrWhiteSpace(name.Text)){notify("Model name must not be blank.");return;}
                        try{Ensure(storage.Rename(index,Encoding.UTF8.GetBytes(name.Text)));persist();}
                        catch(Exception e)when(e is InvalidOperationException or NotSupportedException or ArgumentException){notify(e.Message);}
                    };
                    row.Children.Add(new TextBlock{Text=(index+1).ToString(),Width=24});row.Children.Add(name);
                    row.Children.Add(Button("up","↑",()=>Ensure(models.Move(index,true,persist))));
                    row.Children.Add(Button("down","↓",()=>Ensure(models.Move(index,false,persist))));
                    row.Children.Add(Button("delete","删除",()=>Ensure(models.Delete(index,persist))));row.Children.Add(checkbox);
                    row.Children.Add(Button("btnEditTemplate","编辑模板",()=>edit(index)));list.Children.Add(row);
                }
            }
            toolbar.Children.Add(Button("Build","创建模板",build));toolbar.Children.Add(Button("LoadModule","加载模板",load));
            toolbar.Children.Add(Button("btnDeleteAllModels","删除所有模板",()=>{if(confirm("Delete all templates?"))models.Clear(persist);}));
            panel.Children.Add(toolbar);panel.Children.Add(list);context.OnRefresh(Refresh);Refresh();return panel;
        });
    }
}
