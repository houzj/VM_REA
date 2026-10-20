using System.Globalization;
using System.Text;
namespace VisionPlugins.BranchString;
// Source UI model only. Native comparison/encoding are not inferred here.
public sealed class BranchStringListModel {
 public sealed class Row:System.ComponentModel.INotifyPropertyChanged {
  public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
  string text="";bool debugMode;public string NodeId{get;}public string Text{get=>text;set{if(Allowed(value)){text=value;PropertyChanged?.Invoke(this,new(nameof(Text)));}}}public bool DebugMode{get=>debugMode;set{debugMode=value;PropertyChanged?.Invoke(this,new(nameof(DebugMode)));}}
  internal Row(string id,string text,bool debug){NodeId=id;TrySetText(text);DebugMode=debug;}
  static bool Allowed(string value){ArgumentNullException.ThrowIfNull(value);return value.Length<=30&&!value.Contains('#')&&!value.Contains('$');}
  public bool TrySetText(string value){if(!Allowed(value))return false;Text=value;return true;}
 }
 readonly List<Row> rows=[];public IReadOnlyList<Row> Rows=>rows.AsReadOnly();
 public void Read(string? value){rows.Clear();if(string.IsNullOrWhiteSpace(value))return;var parts=value.Split('$');for(int i=0;i<parts.Length-1;i++){var fields=parts[i].Split('#');if(fields.Length<3)throw new FormatException("BranchString UI row requires three fields.");rows.Add(new(fields[0],fields[1],fields[2]=="1"));}}
 public string Serialize()=>string.Concat(rows.Select(r=>r.NodeId+"#"+r.Text+"#"+(r.DebugMode?"1":"0")+"$"));
 public bool Move(int index,int direction){if(direction is not(-1 or 1))throw new ArgumentOutOfRangeException(nameof(direction));int target=index+direction;if(index<0||index>=rows.Count||target<0||target>=rows.Count)return false;var row=rows[index];rows.RemoveAt(index);rows.Insert(target,row);return true;}
}

