using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Apps.Json;
using VMGlobalScript;

namespace VM.GlobalScript.Support;

public class GlobalManager : IDisposable
{
	private static GlobalManager _instance = null;

	private BaseZmqCommunicate objZmqCom = null;

	private int _iReceiveTime = 50;

	private int _iWriteTime = 100;

	private UserGlobalScriptManger objScriptManager = null;

	private string _strSaveFilePath;

	private Timer objListenServerTime = null;

	private int iListenServerTime = 15000;

	private Timer objLoadDefaultScriptTime = null;

	private int iLoadDefaultScriptTimer = 5000;

	private bool bNeedLoadDefaultScript = true;

	private bool bZmqType = false;

	private bool IsSingleProcess = false;

	public ServerInfo objServerInfo { get; private set; }

	public static GlobalManager GetInstance()
	{
		if (_instance == null)
		{
			_instance = new GlobalManager();
		}
		return _instance;
	}

	public GlobalManager()
	{
		objServerInfo = new ServerInfo();
	}

	public int StartByExe(string msg)
	{
		IsSingleProcess = false;
		objScriptManager = new UserGlobalScriptManger();
		return (!StartDealCommandLines(msg)) ? (-536870911) : 0;
	}

	public int StartByDll(IntPtr sdkHandle)
	{
		IsSingleProcess = true;
		objScriptManager = new UserGlobalScriptManger(isSingleMode: true);
		objScriptManager.SetSdkHandel(sdkHandle);
		initLoadDefaultScriptTimer();
		return 0;
	}

	public int HandleMsg(int cmd, IntPtr InMsg, int nMsgLen, ref string reMsg)
	{
		if (IsSingleProcess)
		{
			return HandleCmd(cmd, InMsg, nMsgLen, ref reMsg);
		}
		return 0;
	}

	public void RegsiterRepotCallBack(GlobalScriptReportMsg reportMsg)
	{
		if (objScriptManager != null)
		{
			objScriptManager.ReportMsgAction = reportMsg;
		}
	}

	public bool StartDealCommandLines(string paramers)
	{
		try
		{
			LogHelper.Info(paramers);
			string[] array = paramers.Split(';');
			if (array.Length < 4)
			{
				return false;
			}
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(':');
				if (array2.Length < 2)
				{
					break;
				}
				if (array2[0] == "ServerPairAddr")
				{
					objServerInfo.ServerPairAddr = array[i].Substring(array2[0].Length + 1);
				}
				else if (array2[0] == "ReportPairAddr")
				{
					objServerInfo.ReportPairAddr = array[i].Substring(array2[0].Length + 1);
				}
				else if (array2[0] == "ServerRepAddr")
				{
					objServerInfo.ServerRepAddr = array[i].Substring(array2[0].Length + 1);
					if (objServerInfo.ServerRepAddr.Contains("tcp"))
					{
						objServerInfo.ServerRepAddr = objServerInfo.ServerRepAddr.Substring(objServerInfo.ServerRepAddr.LastIndexOf('/') + 1);
					}
					if (objScriptManager != null)
					{
						objScriptManager.ServerRepAddr = objServerInfo.ServerRepAddr;
					}
				}
				else if (array2[0] == "ClientCommAddr")
				{
					objServerInfo.ClientCommAddr = array[i].Substring(array2[0].Length + 1);
					if (objServerInfo.ClientCommAddr.Contains("tcp"))
					{
						objServerInfo.ClientCommAddr = objServerInfo.ClientCommAddr.Substring(objServerInfo.ClientCommAddr.LastIndexOf('/') + 1);
					}
					if (objScriptManager != null)
					{
						objScriptManager.ClientCommAddr = objServerInfo.ClientCommAddr;
					}
				}
				else if (array2[0] == "ServerName")
				{
					objServerInfo.ServerName = array2[1];
				}
				else if (array2[0] == "ServerPid")
				{
					if (int.TryParse(array2[1], out var result))
					{
						objServerInfo.ServerPID = result;
						if (objScriptManager != null)
						{
							objScriptManager.ServerPid = objServerInfo.ServerPID;
						}
					}
				}
				else if (array2[0] == "IsCrash")
				{
					objServerInfo.IsCrash = array2[1] == "1";
				}
				else if (array2[0] == "SmGlobalProfix")
				{
					objServerInfo.IsSmGlobalProfix = array2[1] == "1";
					if (objScriptManager != null)
					{
						objScriptManager.bSmGlobalProfix = objServerInfo.IsSmGlobalProfix;
					}
				}
				else if (array2[0] == "MQType")
				{
					bZmqType = array2[1] == "1";
				}
			}
			if (string.IsNullOrEmpty(objServerInfo.ServerPairAddr) || string.IsNullOrEmpty(objServerInfo.ServerName) || objServerInfo.ServerPID <= 0 || string.IsNullOrEmpty(objServerInfo.ClientCommAddr))
			{
				LogHelper.Error("Get the command from server is fault,paramers is " + paramers);
				return false;
			}
		}
		catch (Exception ex)
		{
			LogHelper.Error("Deal CommandLine Error:" + ex.Message);
			return false;
		}
		initZmq();
		initLoadDefaultScriptTimer();
		initListenTimer();
		if (objScriptManager != null)
		{
			objScriptManager.InitZmqToServer(objServerInfo.ReportPairAddr, _iReceiveTime, _iWriteTime, bZmqType);
		}
		if (objServerInfo.IsCrash)
		{
			bNeedLoadDefaultScript = false;
			LogHelper.Info("GlobalScript last is crash,recover globalScript");
			if (objScriptManager != null)
			{
				objScriptManager.bCrashFlag = true;
				objScriptManager.LoadRecoverSolution();
			}
			else
			{
				LogHelper.Error("objScriptManager is null");
			}
		}
		return true;
	}

	private void initLoadDefaultScriptTimer()
	{
		objLoadDefaultScriptTime = new Timer(iLoadDefaultScriptTimer);
		objLoadDefaultScriptTime.Elapsed += objLoadDefaultScriptTime_Elapsed;
		objLoadDefaultScriptTime.AutoReset = false;
		objLoadDefaultScriptTime.Enabled = true;
	}

	private void objLoadDefaultScriptTime_Elapsed(object sender, ElapsedEventArgs e)
	{
		if (bNeedLoadDefaultScript)
		{
			LogHelper.Info("Load default globalScript begin");
			objScriptManager.LoadDefaultSolution();
			LogHelper.Info("Load default globalScript end");
		}
		bNeedLoadDefaultScript = false;
		objLoadDefaultScriptTime.Enabled = false;
	}

	private void initZmq()
	{
		bool flag = false;
		ZmqDataContext zmqDataContext = new ZmqDataContext
		{
			ConnectionString = objServerInfo.ServerPairAddr,
			RcvTimout = _iReceiveTime,
			Encod = Encoding.UTF8,
			ServerOrClient = true,
			WriteTimeOut = _iWriteTime,
			StartReceiveTask = true
		};
		if (bZmqType)
		{
			zmqDataContext.ZmqType = 1;
			objZmqCom = new HkrMqCommunicate(zmqDataContext);
		}
		else
		{
			zmqDataContext.ZmqType = 0;
			objZmqCom = new ZmqCommunicate(zmqDataContext);
		}
		BaseZmqCommunicate baseZmqCommunicate = objZmqCom;
		baseZmqCommunicate.GetReceiveData = (Func<string, string>)Delegate.Combine(baseZmqCommunicate.GetReceiveData, new Func<string, string>(ZmqGetReceiveData));
		if (objZmqCom.InitCommuncate())
		{
			LogHelper.Info(string.Format("Create {0} Zmqserver {1} Succeed:", bZmqType ? "hkrmq" : "zmq", objServerInfo.ServerPairAddr));
		}
		else
		{
			LogHelper.Info(string.Format("Create {0} Zmqserver {1} Faild:", bZmqType ? "hkrmq" : "zmq", objServerInfo.ServerPairAddr));
		}
	}

	private void initListenTimer()
	{
		objListenServerTime = new Timer(iListenServerTime);
		objListenServerTime.Elapsed += objListenServerTime_Elapsed;
		objListenServerTime.AutoReset = true;
		objListenServerTime.Enabled = true;
	}

	private void objListenServerTime_Elapsed(object sender, ElapsedEventArgs e)
	{
		if (!isCheckServer(objServerInfo.ServerName, objServerInfo.ServerPID))
		{
			LogHelper.Error("Check Server.exe is not exit,kill globalscript,serverPid:" + objServerInfo.ServerPID);
			Dispose();
			LogHelper.Info("Close Global Script over");
			try
			{
				Environment.Exit(0);
			}
			catch (Exception)
			{
				LogHelper.Error("Close Global Script error");
			}
		}
	}

	private bool isCheckServer(string processName, int processID)
	{
		try
		{
			Process processById = Process.GetProcessById(processID);
			if (processName.Contains("."))
			{
				processName = processName.Substring(0, processName.IndexOf('.'));
			}
			if (processById.ProcessName == processName)
			{
				processById.Close();
				return true;
			}
			processById.Close();
			LogHelper.Info("Server exe pid is not equal name");
			return false;
		}
		catch
		{
			return false;
		}
	}

	private void initJson()
	{
		Task.Run(() =>
		{
			try
			{
				string value = "{\"head\":{\"command\":4005,\"description\":\"reserved\",\"type\":\"request\",\"seqId\":2}}";
				dynamic val = JsonConvert.DeserializeObject(value);
				string text = val.head.command;
				string text2 = JsonConvert.SerializeObject(val);
			}
			catch
			{
			}
		});
	}

	public int HandleCmd(int cmd, IntPtr pInMsg, int nMsgLen, ref string reMsg)
	{
		uint num = 0u;
		reMsg = "";
		string text = "";
		CMDStatusWithServer cMDStatusWithServer = (CMDStatusWithServer)Convert.ToInt32(cmd);
		if (cMDStatusWithServer == CMDStatusWithServer.SetCommunicateData)
		{
			if (pInMsg == IntPtr.Zero || nMsgLen <= 0)
			{
				return 0;
			}
			objScriptManager.SetCommunicateData(pInMsg);
		}
		else if (pInMsg != IntPtr.Zero && nMsgLen >= 0)
		{
			byte[] array = new byte[nMsgLen];
			Marshal.Copy(pInMsg, array, 0, nMsgLen);
			text = Encoding.UTF8.GetString(array);
		}
		LogHelper.Info("Receive:" + cMDStatusWithServer);
		switch (cMDStatusWithServer)
		{
		case CMDStatusWithServer.LoadSolution:
			bNeedLoadDefaultScript = false;
			num = objScriptManager.LoadSolution(text, objServerInfo.IsCrash);
			break;
		case CMDStatusWithServer.SaveSolution:
			num = objScriptManager.SaveSolutionByMap(out reMsg);
			break;
		case CMDStatusWithServer.ExcuteOnce:
			num = objScriptManager.StartOnce();
			break;
		case CMDStatusWithServer.ExcuteContinue:
			num = objScriptManager.StartContinueRun();
			break;
		case CMDStatusWithServer.StopExcute:
			num = objScriptManager.StopExcute();
			break;
		case CMDStatusWithServer.ShowWindow:
			num = 0u;
			break;
		case CMDStatusWithServer.CloseScript:
			objServerInfo.IsCrash = false;
			num = objScriptManager.CloseScript();
			break;
		case CMDStatusWithServer.GetMsgToUI:
			num = objScriptManager.GetMsgToUI(text, ref reMsg);
			break;
		case CMDStatusWithServer.SetMsgFromUI:
			num = objScriptManager.SetMsgFromUI(text);
			break;
		case CMDStatusWithServer.SetVMZmqPair:
			num = objScriptManager.SetVMZmqPair(text);
			break;
		case CMDStatusWithServer.ReleaseSharedMemory:
			num = objScriptManager.ReleaseShaleMap(text);
			break;
		case CMDStatusWithServer.SilentlyExecuteOnce:
			num = objScriptManager.SilentlyExecuteOnce();
			break;
		case CMDStatusWithServer.LoadSolutionEnd:
			num = objScriptManager.ExecuteLoadInit();
			break;
		}
		LogHelper.Info("Receive end:" + cMDStatusWithServer);
		return 0;
	}

	public string ZmqGetReceiveData(string obj)
	{
		string empty = string.Empty;
		if (string.IsNullOrEmpty(obj))
		{
			return empty;
		}
		uint num = 0u;
		CMDStatusWithServer cMDStatusWithServer = CMDStatusWithServer.UnKnow;
		dynamic val = null;
		dynamic val2 = null;
		try
		{
			val = JsonConvert.DeserializeObject(obj);
			if (val == null)
			{
				LogHelper.Error("Json Deserial Error,Info:" + obj);
				return empty;
			}
			string value = val.head.command;
			cMDStatusWithServer = (CMDStatusWithServer)Convert.ToInt32(value);
			LogHelper.Info("Receive:" + cMDStatusWithServer);
			switch (cMDStatusWithServer)
			{
			case CMDStatusWithServer.LoadSolution:
			{
				bNeedLoadDefaultScript = false;
				string filePath = val.body.filePath;
				num = objScriptManager.LoadSolution(filePath, objServerInfo.IsCrash);
				break;
			}
			case CMDStatusWithServer.SaveSolution:
				num = objScriptManager.SaveSolutionByMap(out _strSaveFilePath);
				val2 = new
				{
					filePath = _strSaveFilePath
				};
				break;
			case CMDStatusWithServer.ExcuteOnce:
				num = objScriptManager.StartOnce();
				break;
			case CMDStatusWithServer.ExcuteContinue:
				num = objScriptManager.StartContinueRun();
				break;
			case CMDStatusWithServer.StopExcute:
				num = objScriptManager.StopExcute();
				break;
			case CMDStatusWithServer.ShowWindow:
				num = 0u;
				break;
			case CMDStatusWithServer.CloseScript:
				objServerInfo.IsCrash = false;
				num = objScriptManager.CloseScript();
				break;
			case CMDStatusWithServer.GetMsgToUI:
			{
				string transparentData = null;
				num = objScriptManager.GetMsgToUI(val.body.transparentData.ToString(), ref transparentData);
				val2 = new { transparentData };
				break;
			}
			case CMDStatusWithServer.SetMsgFromUI:
				num = objScriptManager.SetMsgFromUI(val.body.transparentData.ToString());
				break;
			case CMDStatusWithServer.SetVMZmqPair:
				num = objScriptManager.SetVMZmqPair(val.body.clientCommAddr.ToString());
				break;
			case CMDStatusWithServer.ReleaseSharedMemory:
				num = objScriptManager.ReleaseShaleMap(val.body.sharedMemoryName.ToString());
				break;
			case CMDStatusWithServer.SilentlyExecuteOnce:
				num = objScriptManager.SilentlyExecuteOnce();
				break;
			case CMDStatusWithServer.LoadSolutionEnd:
				num = objScriptManager.ExecuteLoadInit();
				break;
			case (CMDStatusWithServer)4010:
				break;
			}
		}
		catch (Exception ex)
		{
			LogHelper.Error("Deal Recive Json Command Error," + ex.Message + obj);
		}
		if (val == null)
		{
			LogHelper.Info("send:434{ 434:r34}");
			return "434{434:r34}";
		}
		dynamic val3 = null;
		object head = new
		{
			command = (object)val.head.command,
			type = "response",
			seqId = (object)val.head.seqId,
			errorCode = num,
			errorDesc = ErrorCode.GetErrorInfo(num)
		};
		val3 = ((!((val2 != null) ? true : false)) ? ((object)new { head }) : ((object)new
		{
			head = head,
			body = (object)val2
		}));
		empty = JsonConvert.SerializeObject(val3);
		objZmqCom.SendData(empty);
		LogHelper.Info("Send:" + cMDStatusWithServer);
		if (num != 0)
		{
			LogHelper.Info(empty);
		}
		return empty;
	}

	public void Dispose()
	{
		unEnableTimer();
		if (objZmqCom != null)
		{
			BaseZmqCommunicate baseZmqCommunicate = objZmqCom;
			baseZmqCommunicate.GetReceiveData = (Func<string, string>)Delegate.Remove(baseZmqCommunicate.GetReceiveData, new Func<string, string>(ZmqGetReceiveData));
			objZmqCom.Dispose();
		}
		if (objScriptManager != null)
		{
			objScriptManager.Dispose();
		}
	}

	private void unEnableTimer()
	{
		if (objListenServerTime != null)
		{
			objListenServerTime.Enabled = false;
		}
		if (objLoadDefaultScriptTime != null)
		{
			objLoadDefaultScriptTime.Enabled = false;
		}
	}
}
