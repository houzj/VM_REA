using System;
using System.Diagnostics;
using VM.GlobalScript;
using VM.GlobalScript.Support;

namespace VMGlobalScript;

public class GlobalScript
{
	private const string version = "V4.3.0";

	private static GlobalScript _instance = null;

	private bool bInit = false;

	public event GlobalScriptReportMsg GlobalReportCallBackEvent = null;

	public static GlobalScript GetInstance()
	{
		if (_instance == null)
		{
			_instance = new GlobalScript();
		}
		return _instance;
	}

	public int Init(GloablScriptMode mode, IntPtr sdkHandle, string msg = null)
	{
		try
		{
			if (bInit)
			{
				return 0;
			}
			bInit = true;
			LogHelper.Info(" ====================================================V4.3.0 GlobalScript Start Print Log====================================================");
			if (string.IsNullOrEmpty(msg) && mode == GloablScriptMode.EXE)
			{
				return -536870911;
			}
			if (mode == GloablScriptMode.EXE)
			{
				return GlobalManager.GetInstance().StartByExe(msg);
			}
			return GlobalManager.GetInstance().StartByDll(sdkHandle);
		}
		catch (Exception ex)
		{
			Debugger.Log(0, null, "Globals:Init is error:" + ex.ToString());
			return -536870910;
		}
	}

	public int DeInit()
	{
		GlobalManager.GetInstance().Dispose();
		bInit = false;
		return 0;
	}

	public int HandleMsg(int cmd, IntPtr InMsg, int nMsgLen, ref string reMsg)
	{
		return GlobalManager.GetInstance().HandleMsg(cmd, InMsg, nMsgLen, ref reMsg);
	}

	public int RegsiterRepotCallBack(GlobalScriptReportMsg reportMsg)
	{
		GlobalManager.GetInstance().RegsiterRepotCallBack(reportMsg);
		return 0;
	}
}
