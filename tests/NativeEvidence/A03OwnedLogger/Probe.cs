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
  string sentinel=args[0];
  IntPtr handle=IntPtr.Zero; bool mapped=false,created=false; int disposition; int exit=1;
  Action<string> log=s=>File.AppendAllText(Path.Combine(args[0],"registry-proof.log"),DateTime.UtcNow.ToString("o")+" "+s+Environment.NewLine);
  try {
   string[] names={"Apps.BaseLog.dll","Apps.Log.dll"};
   string[] expected={"AC114176CDFDB5A7478D1AFF4294EE006C842ACDC31FC104DCC92604FEF61F93","FB184C0E339C29107C977A6F84E2FF930B73A3CAB87B650F7F23DD2795D4CFB3"};
   for(int i=0;i<names.Length;i++) {
    string p=Path.Combine(args[0],names[i]); string hash;
    using(var stream=File.OpenRead(p)) using(var sha=new System.Security.Cryptography.SHA256Managed()) hash=BitConverter.ToString(sha.ComputeHash(stream)).Replace("-","");
    if(hash!=expected[i]) throw new Exception("Preload hash not covered by raw module metadata audit");
    var loaded=System.Reflection.Assembly.LoadFrom(p);
    string actual; using(var stream=File.OpenRead(loaded.Location)) using(var sha=new System.Security.Cryptography.SHA256Managed()) actual=BitConverter.ToString(sha.ComputeHash(stream)).Replace("-","");
    if(actual!=expected[i]) throw new Exception("Actual preload hash mismatch");
    log("preload-no-static-member="+loaded.FullName+" path="+loaded.Location+" actualSha="+actual);
   }
   log("process="+System.Diagnostics.Process.GetCurrentProcess().Id+" bitness="+(IntPtr.Size*8)+" own="+own);
   log("HKEY_CURRENT_USER="+HKCU.ToInt64()+" managed="+Registry.CurrentUser.Handle.DangerousGetHandle().ToInt64()+" HKEY_LOCAL_MACHINE="+HKLM.ToInt64()+" managed="+Registry.LocalMachine.Handle.DangerousGetHandle().ToInt64());
   int rc=RegCreateKeyEx(HKCU,own,0,null,1,0x2001F,IntPtr.Zero,out handle,out disposition);
   log("RegCreateKeyEx volatile rc="+rc); if(rc!=0) return 10; created=true;
   using(var root=Registry.CurrentUser.OpenSubKey(own,true)) using(var k=root.CreateSubKey(target,RegistryKeyPermissionCheck.ReadWriteSubTree,RegistryOptions.Volatile)) k.SetValue("",sentinel);
   rc=RegOverridePredefKey(HKLM,handle); log("override rc="+rc); if(rc!=0) return 11; mapped=true;
   using(var k=Registry.LocalMachine.OpenSubKey(target)) { string actual=k==null?null:(string)k.GetValue(""); log("mapped-value="+actual); if(actual!=sentinel) return 12; }
   var a=System.Reflection.Assembly.LoadFrom(Path.Combine(args[0],"LoggerRun.exe")); try { a.EntryPoint.Invoke(null,new object[]{new string[0]}); } catch(System.Reflection.TargetInvocationException ex) { throw ex.InnerException; } log("owned-logger-proof=PASS"); exit=0;
  } catch(Exception ex) { log(ex.ToString()); exit=13; }
  finally {
   if(mapped) log("restore rc="+RegOverridePredefKey(HKLM,IntPtr.Zero));
   if(handle!=IntPtr.Zero) log("close rc="+RegCloseKey(handle));
   if(created) { try { Registry.CurrentUser.DeleteSubKeyTree(own,false); log("own-key-cleanup=PASS"); } catch(Exception ex) { log("own-key-cleanup="+ex); exit=14; } }
  }
  return exit;
 }
}



