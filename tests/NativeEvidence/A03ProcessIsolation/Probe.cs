using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;
class Probe {
 [DllImport("advapi32.dll", CharSet=CharSet.Unicode)] static extern int RegCreateKeyEx(IntPtr h, string sub, int r, string cls, int options, int access, IntPtr attrs, out IntPtr result, out int disposition);
 [DllImport("advapi32.dll")] static extern int RegOverridePredefKey(IntPtr key, IntPtr replacement);
 [DllImport("advapi32.dll")] static extern int RegCloseKey(IntPtr key);
 static readonly IntPtr HKCU=new IntPtr(unchecked((int)0x80000001u)), HKLM=new IntPtr(unchecked((int)0x80000002u));
 static int Main(string[] args) {
  string own="Software\\REA-A03-"+Guid.NewGuid().ToString("N");
  string target="SOFTWARE\\WOW6432Node\\Microsoft\\.NETFramework\\v4.0.30319\\AssemblyFoldersEx\\VisionMaster";
  string sentinel=Path.Combine(args[0],"owned-appbase");
  IntPtr handle=IntPtr.Zero; bool mapped=false,created=false; int disposition; int exit=1;
  Action<string> log=s=>File.AppendAllText(Path.Combine(args[0],"registry-proof.log"),DateTime.UtcNow.ToString("o")+" "+s+Environment.NewLine);
  try {
   log("process="+System.Diagnostics.Process.GetCurrentProcess().Id+" bitness="+(IntPtr.Size*8)+" own="+own);
   log("HKEY_CURRENT_USER="+HKCU.ToInt64()+" managed="+Registry.CurrentUser.Handle.DangerousGetHandle().ToInt64()+" HKEY_LOCAL_MACHINE="+HKLM.ToInt64()+" managed="+Registry.LocalMachine.Handle.DangerousGetHandle().ToInt64());
   int rc=RegCreateKeyEx(HKCU,own,0,null,1,0x2001F,IntPtr.Zero,out handle,out disposition);
   log("RegCreateKeyEx volatile rc="+rc); if(rc!=0) return 10; created=true;
   using(var root=Registry.CurrentUser.OpenSubKey(own,true)) using(var k=root.CreateSubKey(target,RegistryKeyPermissionCheck.ReadWriteSubTree,RegistryOptions.Volatile)) k.SetValue("",sentinel);
   rc=RegOverridePredefKey(HKLM,handle); log("override rc="+rc); if(rc!=0) return 11; mapped=true;
   using(var k=Registry.LocalMachine.OpenSubKey(target)) { string actual=k==null?null:(string)k.GetValue(""); log("mapped-value="+actual); if(actual!=sentinel) return 12; }
   log("registry-local-proof=PASS; original DLL/logger NOT loaded; next safety gate remains separate"); exit=0;
  } catch(Exception ex) { log(ex.ToString()); exit=13; }
  finally {
   if(mapped) log("restore rc="+RegOverridePredefKey(HKLM,IntPtr.Zero));
   if(handle!=IntPtr.Zero) log("close rc="+RegCloseKey(handle));
   if(created) { try { Registry.CurrentUser.DeleteSubKeyTree(own,false); log("own-key-cleanup=PASS"); } catch(Exception ex) { log("own-key-cleanup="+ex); exit=14; } }
  }
  return exit;
 }
}
