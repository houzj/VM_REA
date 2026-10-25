using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using VMGlobalScript;

namespace GScriptComponent;

[Guid("3FF87FA1-4D8A-4A2B-ACCE-7D49422EFEA5")]
[ComDefaultInterface(typeof(IGScriptCom))]
[ComSourceInterfaces(typeof(IGScriptEvent))]
[ComVisible(true)]
public class GScriptCom : IGScriptCom
{
	public delegate void ReportDelegate(ref string msg);

	public GlobalScriptReportMsg globalScriptReportMsg = null;

	public event ReportDelegate ReportMsg;

	public int Init(long sdkHandle)
	{
		IntPtr sdkHandle2 = (IntPtr)sdkHandle;
		return GlobalScript.GetInstance().Init(GloablScriptMode.DLL, sdkHandle2);
	}

	public int DealMsg(int cmd, IntPtr pInMsg, int nMsgLen, IntPtr pOutMsg, ref int nOutMsgLen)
	{
		try
		{
			string reMsg = "";
			int num = GlobalScript.GetInstance().HandleMsg(cmd, pInMsg, nMsgLen, ref reMsg);
			if (num == 0 && pOutMsg != IntPtr.Zero)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(reMsg);
				if (nOutMsgLen <= bytes.Length)
				{
					nOutMsgLen = bytes.Length + 1;
					return -536870894;
				}
				Marshal.Copy(bytes, 0, pOutMsg, bytes.Length);
			}
			return num;
		}
		catch (Exception ex)
		{
			Debugger.Log(0, null, "GlobalScript:DealMsg is error:" + ex.ToString());
			return -536870657;
		}
	}

	public int Dispose()
	{
		globalScriptReportMsg = null;
		ReportMsg = null;
		return GlobalScript.GetInstance().DeInit();
	}

	public void ReportScriptMsg(string msg)
	{
		if (ReportMsg != null)
		{
			ReportMsg(ref msg);
		}
	}
}
