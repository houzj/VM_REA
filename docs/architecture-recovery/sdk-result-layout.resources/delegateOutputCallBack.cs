using System;
using System.Runtime.InteropServices;

namespace VM.PlatformSDKCS;

/// <summary>
/// 回调函数
/// </summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
public delegate void delegateOutputCallBack(IntPtr pstOutputPlatformInfo, IntPtr pUser);
