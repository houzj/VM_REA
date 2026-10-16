using System.IO;
using System.Text.RegularExpressions;

namespace VisionPlugins.ImageSource;

internal static class LocalFolderSelection
{
 public static bool HasLocalSyntax(string value)=>Regex.IsMatch(value,@"^[A-Za-z]:\\[^\f\n\r\t\v]*$");
 public static string Prepare(string value)
 {
  if(string.IsNullOrWhiteSpace(value))throw new ArgumentException("图像路径不能为空。");
  // Original OpenFolderDialog.IsCorrectDir accepts local drive-rooted paths only.
  if(!HasLocalSyntax(value))throw new ArgumentException("请输入本地磁盘的绝对目录路径。");
  if(value.Length>200)throw new ArgumentException("图像路径长度不能超过200。");
  if(!Directory.Exists(value[..3]))throw new DirectoryNotFoundException("图像路径所在磁盘不存在。");
  Directory.CreateDirectory(value);
  // The vendor probes a fixed test.txt name. Use CreateNew on a unique name to
  // avoid overwriting existing directory contents while preserving the write check.
  string probe=Path.Combine(value,".vision-write-"+Guid.NewGuid().ToString("N")+".tmp");
  using(var stream=new FileStream(probe,FileMode.CreateNew,FileAccess.Write,FileShare.None,1,FileOptions.DeleteOnClose)){}
  return value;
 }
}
