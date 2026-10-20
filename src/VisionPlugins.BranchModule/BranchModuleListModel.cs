#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
namespace VisionPlugins.BranchModule;
// ListParam/ListParamItem behavior recovered from the installed Control CIL.
// This model has no renderer, host scheduling, or automatic execution side effect.
public sealed class BranchModuleListModel {
 public sealed class Row {
  readonly BranchModuleListModel owner;
  public string Id {get;}
  public string? ValueValue {get;private set;}
  public string? InputValue {get;private set;}
  public bool Enabled {get;internal set;}=true;
  internal Row(BranchModuleListModel owner,string id,string value){this.owner=owner;Id=id;ValueValue=value;}
  public bool TrySetInput(string? text){
   if(!string.IsNullOrWhiteSpace(text)&&text!="Defaults"&&!int.TryParse(text,NumberStyles.Integer,CultureInfo.CurrentCulture,out _))return false;
   if(!string.IsNullOrWhiteSpace(text)&&text.Length>1&&text.Substring(0,1)=="0")text=text.Substring(1);
   InputValue=text;if(owner.Mode==0)ValueValue=text;return true;
  }
 }
 readonly List<Row> rows=new();public IReadOnlyList<Row> Rows=>rows.AsReadOnly();
 public int Mode {get;private set;}
 public void SetMode(int mode,Action? inputValueChanged=null){
  if(mode is 0 or 1)foreach(var row in rows)row.Enabled=mode==0;
  if(Mode==mode)return;Mode=mode;
  foreach(var row in rows){string? before=row.InputValue;if(mode==1)row.TrySetInput("Defaults");else if(mode==0)row.TrySetInput(row.ValueValue);if(mode==0&&before!=row.InputValue)inputValueChanged?.Invoke();}
 }
 public void Read(string? value){
  rows.Clear();if(string.IsNullOrWhiteSpace(value))return;
  var parts=value.Split('$');for(int i=0;i<parts.Length-1;i++){
   var fields=parts[i].Split('#');var row=new Row(this,fields[0],fields[1]);
   if(Mode==1){row.TrySetInput("Defaults");row.Enabled=false;}else if(Mode==0){row.TrySetInput(fields[1]);row.Enabled=true;}rows.Add(row);
  }
 }
 public string Serialize(){var value=new System.Text.StringBuilder();foreach(var row in rows){string input=string.IsNullOrWhiteSpace(row.InputValue)?"0":row.InputValue!;value.Append(row.Id).Append('#').Append(input=="Defaults"?"N":input).Append('$');}return value.ToString();}
 public bool Move(int index,int direction){if(direction is not(-1 or 1))throw new ArgumentOutOfRangeException(nameof(direction));int target=index+direction;if(index<0||index>=rows.Count||target<0||target>=rows.Count)return false;var row=rows[index];rows.RemoveAt(index);rows.Insert(target,row);return true;}
}

