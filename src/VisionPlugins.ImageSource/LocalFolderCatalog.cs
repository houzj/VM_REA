using System.IO;
using System.Runtime.InteropServices;

namespace VisionPlugins.ImageSource;

/// <summary>Folder snapshot only. Session watching and scheduler integration belong to IS-004.</summary>
internal static class LocalFolderCatalog
{
 [DllImport("shlwapi.dll",CharSet=CharSet.Unicode,ExactSpelling=true)]
 static extern int StrCmpLogicalW(string left,string right);
 internal static int Compare(string left,string right)=>StrCmpLogicalW(left,right);
 internal static string[] Read(string directory)
 {
  // Original GetImageFilesPathNameFromDir is top-level and case-sensitive here,
  // even though the underlying Windows filesystem is normally case-insensitive.
  return Directory.EnumerateFiles(directory,"*",SearchOption.TopDirectoryOnly)
   .Where(path=>Path.GetExtension(path) is ".jpg" or ".png" or ".bmp" or ".tif")
   .OrderBy(path=>path,Comparer<string>.Create(Compare)).ToArray();
 }
}
