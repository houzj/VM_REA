using System.Text.Json;

namespace VisionEditor;

internal static class ShellLockPreference
{
 internal static string FilePath { get; set; }=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"VisionReplica","shell-preferences.json");

 internal static bool Read()
 {
  try
  {
   if(!File.Exists(FilePath))return false;
   using var document=JsonDocument.Parse(File.ReadAllText(FilePath));
   return document.RootElement.TryGetProperty("IsNotifyLock",out var value)&&value.ValueKind==JsonValueKind.True;
  }
  catch(IOException){return false;}
  catch(JsonException){return false;}
  catch(UnauthorizedAccessException){return false;}
 }

 internal static void Write(bool skipPrompt)
 {
  var directory=Path.GetDirectoryName(FilePath)??throw new InvalidOperationException("锁定偏好设置路径无效");
  Directory.CreateDirectory(directory);
  var temporary=Path.Combine(directory,Path.GetRandomFileName());
  try
  {
   File.WriteAllText(temporary,JsonSerializer.Serialize(new {IsNotifyLock=skipPrompt}));
   File.Move(temporary,FilePath,true);
  }
  finally {if(File.Exists(temporary))File.Delete(temporary);}
 }
}
