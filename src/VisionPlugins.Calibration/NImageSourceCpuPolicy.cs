using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
namespace VisionPlugins.Calibration;

// 630120 return-value slice. Do not rename leaf7 bits as conventional ISA levels.
public static class NImageSourceCpuPolicy {
 public static int Detect(){
  if(!OperatingSystem.IsWindows()||!X86Base.IsSupported)throw new PlatformNotSupportedException("Original source CPU policy requires Windows x86 CPUID");
  var one=X86Base.CpuId(1,0);var seven=X86Base.CpuId(7,0);
  var version=new VersionInfo{Size=284,Major=6,Minor=1,ServicePackMajor=1};ulong condition=0;
  condition=VerSetConditionMask(condition,2,3);condition=VerSetConditionMask(condition,1,3);condition=VerSetConditionMask(condition,0x20,3);
  bool versionAccepted=VerifyVersionInfoW(ref version,0x23,condition);
  return Classify(unchecked((uint)one.Eax),unchecked((uint)one.Ecx),unchecked((uint)seven.Ebx),unchecked((uint)seven.Ecx),unchecked((uint)seven.Edx),versionAccepted);
 }
 public static int Classify(uint leaf1Eax,uint leaf1Ecx,uint leaf7Ebx,uint leaf7Ecx,uint leaf7Edx,bool versionAccepted){
  // 630235 family gate leaves every classification flag zero when false.
  if((leaf1Eax&0xf00)<0x600)return 0;
  int lower=(leaf7Ecx&(1u<<20))!=0?7:(leaf7Ecx&(1u<<19))!=0?6:(leaf7Ecx&(1u<<9))!=0?5:(leaf7Ecx&1)!=0?4:(leaf7Edx&(1u<<26))!=0?3:(leaf7Edx&(1u<<25))!=0?2:(leaf7Edx&(1u<<23))!=0?1:0;
  // 63060a..663: failed Windows6.1/SP1 check drops high classes only.
  if(!versionAccepted)return lower;
  if((leaf7Ebx&0x20)!=0)return 11;
  return (leaf1Ecx&0x18000000)==0x18000000?10:lower;
 }
 [StructLayout(LayoutKind.Sequential,CharSet=CharSet.Unicode)]
 struct VersionInfo {
  internal uint Size,Major,Minor,Build,Platform;
  [MarshalAs(UnmanagedType.ByValTStr,SizeConst=128)]internal string? ServicePack;
  internal ushort ServicePackMajor,ServicePackMinor,Suite;internal byte Product,Reserved;
 }
 [DllImport("kernel32.dll")]static extern ulong VerSetConditionMask(ulong mask,uint type,byte condition);
 [DllImport("kernel32.dll",CharSet=CharSet.Unicode,ExactSpelling=true)]
 [return:MarshalAs(UnmanagedType.Bool)]static extern bool VerifyVersionInfoW(ref VersionInfo version,uint type,ulong condition);
}
