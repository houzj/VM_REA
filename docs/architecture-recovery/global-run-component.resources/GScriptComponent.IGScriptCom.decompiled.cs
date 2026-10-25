using System;
using System.Runtime.InteropServices;

namespace GScriptComponent;

[Guid("3C4FCB0E-374E-4179-A0D3-C50CE29DBCF4")]
[InterfaceType(ComInterfaceType.InterfaceIsDual)]
public interface IGScriptCom
{
	[DispId(1)]
	int Init(long sdkHandle);

	[DispId(2)]
	int DealMsg(int cmd, IntPtr pInMsg, int nMsgLen, IntPtr pOutMsg, ref int nOutMsgLen);

	[DispId(3)]
	int Dispose();
}
