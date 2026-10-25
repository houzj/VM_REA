using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows.Threading;
using Apps.Localization;
using Apps.Log;
using VM.PlatformSDKCS;
using VMControls.Interface;

namespace VM.Core;

public class VmIO : VmFrameTreeBase, IDisposable, IVmIO, IVmFrameBase, IParent, ITypeName, IVmOwner
{
	public const int MaxNameLenth = 31;

	public VmModule _root;

	private VmIO _relateIO;

	private VmIO _relateIndex;

	internal uint _ExecuteCount;

	internal bool isNull = true;

	internal IEnumerable _value;

	internal List<object> _valueList = new List<object>();

	private bool needMergeIOLoopValue = false;

	internal bool RaiseValueChanged = true;

	private VmIO _CombinRoot;

	internal string typeName;

	public VmIOCollection Childs = new VmIOCollection();

	private bool disposedValue = false;

	public string customName = "";

	private bool customVisible = true;

	/// <summary>
	/// 是否在界面可见
	/// </summary>
	internal bool visible = true;

	/// <summary>
	/// 是否在模块结果树中显示
	/// </summary>
	internal bool visibleInResultTree = true;

	private bool isReturnRelateValue = true;

	private bool? isForce;

	private bool? isPrefer;

	internal string autoSubscribeName;

	private bool? autoSubscribe;

	private bool? autoSubscribeByOther;

	private bool canSubscribe = true;

	internal bool IsImageIOParent;

	public object _Parent;

	private Action<ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_EX_Data>[] setValueDic = new Action<ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_EX_Data>[7];

	internal bool isResultShow = false;

	public VmModule Root
	{
		get
		{
			return _root;
		}
		set
		{
			if (_root != value)
			{
				_root = value;
				customName = Name + _root.ID;
			}
		}
	}

	public override string Name
	{
		get
		{
			return name;
		}
		set
		{
			string oldName = name;
			this.SetProperty(ref name, value, "Name");
			Root?.InvokeOnModuleIONameChanged(oldName, name);
		}
	}

	/// <summary>
	/// 是否准备好
	/// </summary>
	public bool IsReady => !IsForce || GetReadyState();

	/// <summary>
	/// 列表容器
	/// </summary>
	internal VmIOCollection CollectionParent { get; set; }

	public VmIO RelateIO
	{
		get
		{
			LazyLoadRelateIO();
			if (!IsLeafNode)
			{
				List<VmIO> list = new List<VmIO>();
				List<VmIO> list2 = new List<VmIO>();
				foreach (VmIO child in Childs)
				{
					VmIO relateIO = child.RelateIO;
					if (relateIO == null)
					{
						return null;
					}
					if (relateIO.Parent == null)
					{
						return null;
					}
					if (!list.Contains(relateIO.Parent))
					{
						list.Add(relateIO.Parent as VmIO);
					}
					if (!list2.Contains(relateIO))
					{
						list2.Add(relateIO);
					}
				}
				if (list.Count == 1 && Childs.Count == list2.Count)
				{
					return IsSuitable(TypeName, list[0].TypeName) ? list[0] : null;
				}
				return null;
			}
			return _relateIO;
		}
		set
		{
			try
			{
				LazyLoadRelateIO();
				if (value == _relateIO && IsLeafNode)
				{
					return;
				}
				Expression<Func<object>> oldSubList = () => Root.GetModuleAllSubscribe();
				string displayName = string.Format("{0}{1}{2}", AppLocalizationService.GetLang("Lang_Edit"), Root.FullName, AppLocalizationService.GetLang("RunParam_Subscribe"));
				Ioc.Get<IUnRedoService>()?.Register(new SubscribeUnRedoAction(Subscribe_Redo, Subscribe_Undo, Subscribe_Update, new UnRedo_SubscribeData
				{
					ModuleFullName = Root.FullName,
					OldSubList = oldSubList,
					NewSubList = null
				}), displayName, (IBuilder builder, IUnRedoOperation operation) => CanRegisterSubscribe(builder, operation));
				VmIO relateIO = RelateIO;
				if (relateIO != null)
				{
					Root.UnSubscribe(this, relateIO);
				}
				else
				{
					foreach (VmIO child in Childs)
					{
						child.RelateIO = null;
					}
				}
				_relateIO = value;
				if (_relateIO != null)
				{
					try
					{
						Root.Subscribe(this, _relateIO);
					}
					catch (Exception ex)
					{
						_relateIO = null;
						throw ex;
					}
				}
				RelatedIOChanged?.Invoke(this, new ValueEventArgs
				{
					Value = _relateIO
				});
				if (!Root.IsOnDispose)
				{
					Root.ReFlashReadyState();
				}
			}
			catch (Exception ex2)
			{
				throw ex2;
			}
		}
	}

	public int ValueTypeIndex
	{
		get
		{
			switch (TypeName)
			{
			case "int":
			case "pixelformat":
				return 1;
			case "float":
				return 2;
			case "string":
				return 3;
			case "image":
				return 4;
			case "pointset":
			case "chunk":
				return 5;
			case "byte":
				return 6;
			case "double":
				return 7;
			default:
				return 0;
			}
		}
	}

	/// <summary>
	/// 订阅信息
	/// </summary>
	/// <returns></returns>
	public string SubscriptionInfo
	{
		get
		{
			if (RelateIO != null)
			{
				if (RelateIndex != null)
				{
					return $"{RelateIO.ModulePathName}[{RelateIndex.ModulePathName}]";
				}
				if (RelateIO is VmConstantIO)
				{
					return RelateIO.Name;
				}
				return $"{RelateIO.ModulePathName}[]";
			}
			return "";
		}
	}

	public VmIO RelateIndex
	{
		get
		{
			LazyLoadRelateIO();
			if (!IsLeafNode)
			{
				foreach (VmIO child in Children)
				{
					if (child.RelateIndex != null)
					{
						return child.RelateIndex;
					}
				}
			}
			return _relateIndex;
		}
		set
		{
			LazyLoadRelateIO();
			try
			{
				VmIO relateIO = RelateIO;
				if (relateIO != null)
				{
					Root.UnSubscribe(this, relateIO);
				}
				SetRelateIndex(value);
				if (relateIO != null)
				{
					try
					{
						Root.Subscribe(this, relateIO);
						return;
					}
					catch (Exception ex)
					{
						_relateIO = null;
						_relateIndex = null;
						throw ex;
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
		}
	}

	public uint ExecuteCount
	{
		get
		{
			return _ExecuteCount;
		}
		set
		{
			_ExecuteCount = value;
		}
	}

	public virtual IEnumerable Value
	{
		get
		{
			if (RelateIO != null && IsReturnRelateValue)
			{
				VmModule root = Root;
				if (root != null && root.Inputs?.leafIOs?.ContainsKey(Name.GetHashCode()) == true)
				{
					return RelateIO.Value;
				}
			}
			VmModule vmModule = ((Root is VmProcedure) ? Root : Root?.Root);
			if (Root != null && Root.isSetCallBack != 1 && vmModule?.ExecuteCount != ExecuteCount)
			{
				Root.RefreshResult();
			}
			MergeIOLoopValue();
			return _value;
		}
		set
		{
			if (value != _value)
			{
				_value = value;
				_valueList.Clear();
				if (_value != null)
				{
					_valueList.Add(_value);
				}
			}
		}
	}

	string IVmIO.DisplayValue
	{
		get
		{
			MergeIOLoopValue();
			if (_value != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (TypeName == "int" || TypeName == "float" || TypeName == "double")
				{
					foreach (object item in _value)
					{
						if (item != null)
						{
							stringBuilder.Append(item.ToString()).Append(";");
						}
					}
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Remove(stringBuilder.Length - 1, 1);
					}
					return stringBuilder.ToString();
				}
				if (TypeName == "string")
				{
					foreach (object item2 in _value)
					{
						if (item2 is ImvsSdkDefine.IMVS_MODULE_STRING_VALUE_EX iMVS_MODULE_STRING_VALUE_EX)
						{
							stringBuilder.Append(iMVS_MODULE_STRING_VALUE_EX.strValue).Append(";");
						}
					}
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Remove(stringBuilder.Length - 1, 1);
					}
					return stringBuilder.ToString();
				}
				if (TypeName == "byte")
				{
					foreach (object item3 in _value)
					{
						if (item3 is ImvsSdkDefine.IMVS_MODULE_BYTEDATA_VALUE_EX_Data iMVS_MODULE_BYTEDATA_VALUE_EX_Data)
						{
							stringBuilder.Append(iMVS_MODULE_BYTEDATA_VALUE_EX_Data.strValue).Append(";");
						}
					}
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Remove(stringBuilder.Length - 1, 1);
					}
					return stringBuilder.ToString();
				}
			}
			return null;
		}
	}

	public VmIO CombinRoot
	{
		get
		{
			if (_CombinRoot == null)
			{
				_CombinRoot = (Parent as IVmFrameBase).GetParent<VmIO>();
			}
			return _CombinRoot;
		}
	}

	IVmModule IVmOwner.Owner => Root;

	public virtual string TypeName
	{
		get
		{
			return typeName;
		}
		set
		{
			typeName = value;
		}
	}

	public VmIOCollection Children => Childs;

	public bool IsLeafNode => Childs.Count == 0;

	public override string UniqueName => Name;

	/// <summary>
	/// 输出配置自定义名称
	/// </summary>
	public string CustomName
	{
		get
		{
			return customName;
		}
		set
		{
			if (value != CustomName)
			{
				customName = value;
				OnPropertyChanged("CustomName");
			}
		}
	}

	public virtual bool CustomVisibleA
	{
		get
		{
			return customVisible;
		}
		set
		{
			if (customVisible != value)
			{
				customVisible = value;
				OnPropertyChanged("CustomVisibleA");
			}
		}
	}

	public virtual bool Visible
	{
		get
		{
			return visible;
		}
		set
		{
			if (visible != value)
			{
				visible = value;
				OnPropertyChanged("Visible");
			}
		}
	}

	public virtual bool VisibleInResultTree
	{
		get
		{
			return visibleInResultTree;
		}
		set
		{
			if (visibleInResultTree != value)
			{
				visibleInResultTree = value;
				OnPropertyChanged("VisibleInResultTree");
			}
		}
	}

	/// <summary>
	/// 是否为私有IO，比如group的LoopIndex为私有IO，只能内部模块订阅
	/// </summary>
	public bool IsPrivate { get; set; }

	/// <summary>
	/// 是否优先返回RelateIO的值
	/// </summary>
	public virtual bool IsReturnRelateValue
	{
		get
		{
			if (Root is VmModuleSet)
			{
				return false;
			}
			return isReturnRelateValue;
		}
		set
		{
			if (isReturnRelateValue != value)
			{
				isReturnRelateValue = value;
			}
		}
	}

	public int Index { get; set; }

	/// <summary>
	/// 是否需要强制订阅
	/// </summary>
	public bool IsForce
	{
		get
		{
			if (!isForce.HasValue && !IsLeafNode)
			{
				isForce = Childs.Any((VmIO p) => p.IsForce);
			}
			return isForce ?? false;
		}
		internal set
		{
			isForce = value;
		}
	}

	/// <summary>
	/// 是否建议订阅
	/// </summary>
	public bool IsPrefer
	{
		get
		{
			if (!isPrefer.HasValue && !IsLeafNode)
			{
				isPrefer = Childs.Any((VmIO p) => p.IsPrefer);
			}
			return isPrefer ?? false;
		}
		internal set
		{
			isPrefer = value;
		}
	}

	public string AutoSubscribeName
	{
		get
		{
			return autoSubscribeName;
		}
		set
		{
			if (autoSubscribeName != value)
			{
				autoSubscribeName = value;
			}
		}
	}

	/// <summary>
	/// 是否允许自动订阅
	/// </summary>
	public bool AutoSubscribe
	{
		get
		{
			if (!autoSubscribe.HasValue && !IsLeafNode)
			{
				autoSubscribe = Childs.Any((VmIO p) => p.AutoSubscribe);
			}
			return autoSubscribe ?? true;
		}
		set
		{
			autoSubscribe = value;
		}
	}

	/// <summary>
	/// 是否允许被自动订阅
	/// </summary>
	public bool AutoSubscribeByOther
	{
		get
		{
			if (!autoSubscribeByOther.HasValue && !IsLeafNode)
			{
				autoSubscribeByOther = Childs.Any((VmIO p) => p.AutoSubscribeByOther);
			}
			return autoSubscribeByOther ?? true;
		}
		set
		{
			autoSubscribeByOther = value;
		}
	}

	/// <summary>
	/// 是否允许订阅
	/// </summary>
	public bool CanSubscribeA
	{
		get
		{
			return canSubscribe;
		}
		set
		{
			canSubscribe = value;
		}
	}

	public object Tag { get; set; }

	/// <summary>
	///             id+ 模块名+参数名
	/// </summary>
	public string ModulePathName
	{
		get
		{
			string text = "";
			if (this is VmConstantIO)
			{
				return UniqueName;
			}
			if (Root == null)
			{
				return AppLocalizationService.GetLang(UniqueName);
			}
			return Root.ID + " " + Root.UniqueName + "." + AppLocalizationService.GetLang(UniqueName);
		}
	}

	/// <summary>
	/// 是否为输出IO
	/// </summary>
	public bool IsOutput => Root.Outputs.GetAllIO().Contains(this);

	public override object Parent
	{
		get
		{
			return _Parent;
		}
		set
		{
			_Parent = value;
			if (_Parent is VmIO { TypeName: "IMAGE" })
			{
				IsImageIOParent = true;
			}
		}
	}

	/// <summary>
	/// 是否结果显示IO,如果是，模块底层需要在执行时输出
	/// </summary>
	public bool IsResultShow
	{
		get
		{
			return isResultShow;
		}
		set
		{
			if (IsResultShow != value)
			{
				isResultShow = value;
				OnPropertyChanged("IsResultShow");
			}
		}
	}

	public event VmIOEventHandler ValueChanged;

	public event EventHandler RelatedIOChanged;

	private bool GetReadyState()
	{
		return RelateIO != null || (!IsLeafNode && !Childs.Any((VmIO p) => p != null && !p.IsReady));
	}

	/// <summary>
	/// 删除自身
	/// </summary>
	public void RemoveSelf()
	{
		ClearRelation();
		CollectionParent.Remove(this);
	}

	public bool CanRegisterSubscribe(IBuilder builder, IUnRedoOperation unRedoOperation)
	{
		IUnRedoDataDone<UnRedo_SubscribeData> unRedoDataDone = unRedoOperation as IUnRedoDataDone<UnRedo_SubscribeData>;
		IEnumerable<IUnRedoActionOperation> enumerable = builder.Actions.Where((IUnRedoActionOperation p) => p is IUnRedoDataDone<UnRedo_SubscribeData>);
		foreach (IUnRedoActionOperation item in enumerable)
		{
			if ((item as IUnRedoDataDone<UnRedo_SubscribeData>).Data.ModuleFullName == unRedoDataDone.Data.ModuleFullName)
			{
				return false;
			}
		}
		if (unRedoDataDone.Data.OldSubList is Expression<Func<object>> expression)
		{
			unRedoDataDone.Data.OldSubList = expression.Compile()();
		}
		return true;
	}

	protected void Subscribe_Undo(UnRedo_SubscribeData data, IUnRedoParams unRedoParams)
	{
		(unRedoParams.Dispatcher as Dispatcher).Invoke(() =>
		{
			if (VmSolution.Instance[data.ModuleFullName] is VmModule vmModule)
			{
				vmModule.SetAllSubscribe(data.OldSubList);
			}
		});
	}

	protected void Subscribe_Redo(UnRedo_SubscribeData data, IUnRedoParams unRedoParams)
	{
		(unRedoParams.Dispatcher as Dispatcher).Invoke(() =>
		{
			if (VmSolution.Instance[data.ModuleFullName] is VmModule vmModule)
			{
				vmModule.SetAllSubscribe(data.NewSubList);
			}
		});
	}

	protected void Subscribe_Update(UnRedo_SubscribeData data)
	{
		object newSubList = ((VmModule)VmSolution.Instance[data.ModuleFullName])?.GetModuleAllSubscribe();
		data.NewSubList = newSubList;
	}

	internal virtual bool IsSuitable(string targetType, string sourceType)
	{
		if (sourceType == "string" || targetType == sourceType)
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// 获取订阅信息
	/// </summary>
	/// <returns></returns>
	public string GetSubscriptionInfo()
	{
		StringBuilder stringBuilder = new StringBuilder("(");
		bool flag = true;
		List<VmIO> list = new List<VmIO>();
		GetAllIo(list);
		foreach (VmIO item in list)
		{
			if (item.IsLeafNode)
			{
				if (item.RelateIO != null)
				{
					stringBuilder.Append(item.RelateIO.ModulePathName);
					flag = false;
				}
				else
				{
					stringBuilder.Append(" ");
				}
				if (Children.IndexOf(item) != Children.Count - 1)
				{
					stringBuilder.Append(",");
				}
			}
		}
		stringBuilder.Append(")");
		return flag ? "" : stringBuilder.ToString();
	}

	/// <summary>
	/// 迭代设置_relateIndex
	/// </summary>
	/// <param name="value"></param>
	private void SetRelateIndex(VmIO value)
	{
		_relateIndex = value;
		foreach (VmIO child in Children)
		{
			child.SetRelateIndex(value);
		}
	}

	private void LazyLoadRelateIO()
	{
		if (!Root.relateIO_IsLoaded)
		{
			Root.relateIO_IsLoaded = true;
			Root.LoadModuleRelateIO();
		}
	}

	/// <summary>
	/// 初始化RelateIndexIO，懒加载用
	/// </summary>
	/// <param name="vmIO"></param>
	public void InitRelateIndexIO(VmIO vmIO)
	{
		_relateIndex = vmIO;
	}

	/// <summary>
	/// 初始化IO，懒加载用
	/// </summary>
	/// <param name="vmIO"></param>
	public void InitRelateIO(VmIO vmIO)
	{
		_relateIO = vmIO;
	}

	internal void InvokeValueChangedEvent()
	{
		if (RaiseValueChanged)
		{
			VmSolution instance = VmSolution.Instance;
			if (instance != null && ((IVmSolution)instance).IsOpenSetValueToDisplay)
			{
				MergeIOLoopValue();
				ValueEventArgs e = new ValueEventArgs();
				e.Value = _value;
				e.ExecuteCount = ExecuteCount;
				e.ValueType = TypeName;
				Tuple<VmIO, ValueEventArgs> data = new Tuple<VmIO, ValueEventArgs>(this, e);
				VmSolution.Instance.EnqueueForCallBack(data);
			}
		}
	}

	private void MergeIOLoopValue()
	{
		if (!needMergeIOLoopValue)
		{
			return;
		}
		needMergeIOLoopValue = false;
		int num = 0;
		foreach (object value in _valueList)
		{
			if (value is Array array)
			{
				num += array.Length;
			}
		}
		object[] array2 = null;
		if (num <= 0)
		{
			return;
		}
		array2 = new object[num];
		int num2 = 0;
		foreach (object value2 in _valueList)
		{
			if (!(value2 is Array array3) || num2 >= num)
			{
				continue;
			}
			for (int i = 0; i < array3.Length; i++)
			{
				array2[num2] = array3.GetValue(i);
				num2++;
				if (num2 == num)
				{
					break;
				}
			}
		}
		_valueList.Clear();
		_value = array2;
	}

	internal void OnValueChanged(ValueEventArgs args)
	{
		ValueChanged?.Invoke(this, args);
	}

	void IVmIO.OnVarIOChanged(ValueEventArgs args)
	{
		ValueChanged?.Invoke(this, args);
	}

	internal void SetNoneValueWithoutRaiseEvent()
	{
		if (_value != null)
		{
			_value = null;
		}
	}

	public override object GetNode(string key)
	{
		if (key == "Value")
		{
			return Value;
		}
		return Value;
	}

	public void ClearSubscribe()
	{
		RelateIO = null;
		foreach (VmIO child in Childs)
		{
			child.ClearSubscribe();
		}
	}

	public virtual void Dispose()
	{
		if (disposedValue)
		{
			return;
		}
		foreach (VmIO item in Childs.ToList())
		{
			item.Dispose();
		}
		CollectionParent.Remove(this);
		RelateIO = null;
		disposedValue = true;
	}

	public object GetValue(uint excuteCount, int index, Dictionary<string, string> customConfig = null)
	{
		if (excuteCount != uint.MaxValue && excuteCount != ExecuteCount && excuteCount != RelateIO?.ExecuteCount && !(RelateIO is VmConstantIO))
		{
			Value = null;
			return null;
		}
		MergeIOLoopValue();
		if (Value is Array array && index >= 0)
		{
			if (array == null || array.Length == 0)
			{
				return null;
			}
			index = ((index < GetValueCount()) ? index : 0);
			object obj = (Value as Array)?.GetValue(index);
			if (obj is ImvsSdkDefine.IMVS_MODULE_STRING_VALUE_EX iMVS_MODULE_STRING_VALUE_EX)
			{
				return iMVS_MODULE_STRING_VALUE_EX.strValue;
			}
			if (obj is ImvsSdkDefine.IMVS_MODULE_BYTEDATA_VALUE_EX_Data iMVS_MODULE_BYTEDATA_VALUE_EX_Data)
			{
				return iMVS_MODULE_BYTEDATA_VALUE_EX_Data.hexValue;
			}
			return obj;
		}
		return Value;
	}

	public int GetValueCount()
	{
		if (Value is Array)
		{
			if (isNull)
			{
				return 0;
			}
			return (Value as Array)?.Length ?? 0;
		}
		return 0;
	}

	public int GetCurValueCount()
	{
		MergeIOLoopValue();
		if (_value is Array)
		{
			if (isNull)
			{
				return 0;
			}
			return (_value as Array)?.Length ?? 0;
		}
		return 0;
	}

	public override string ToString()
	{
		return Root?.ToString() + "." + Name;
	}

	/// <summary>
	/// 获取叶子IO
	/// </summary>
	/// <param name="ls"></param>
	/// <returns></returns>
	public List<VmIO> GetAllLeafIO(List<VmIO> ls = null)
	{
		if (ls == null)
		{
			ls = new List<VmIO>();
		}
		if (IsLeafNode)
		{
			ls.Add(this);
		}
		else
		{
			Childs.ToList().ForEach((VmIO p) =>
			{
				p.GetAllLeafIO(ls);
			});
		}
		return ls;
	}

	public List<VmIO> GetAllIO(List<VmIO> ls = null)
	{
		if (ls == null)
		{
			ls = new List<VmIO>();
		}
		ls.Add(this);
		if (!IsLeafNode)
		{
			Childs.ToList().ForEach((VmIO p) =>
			{
				p.GetAllIO(ls);
			});
		}
		return ls;
	}

	public void GetAllIo(IList<VmIO> ios)
	{
		if (IsLeafNode)
		{
			ios.Add(this);
			return;
		}
		ios.Add(this);
		foreach (VmIO child in Childs)
		{
			if (child.IsLeafNode)
			{
				ios.Add(child);
			}
			else
			{
				child.GetAllIo(ios);
			}
		}
	}

	public void RemovePubIO()
	{
	}

	internal virtual void OnAddCollection(VmIOCollection vmIOCollection)
	{
		CollectionParent = vmIOCollection;
	}

	internal virtual void OnRemoveCollection()
	{
		Dispose();
	}

	/// <summary>
	/// 清除订阅
	/// </summary>
	protected List<Tuple<int, VmIO, VmIO>> ClearRelation(List<Tuple<int, VmIO, VmIO>> nextSubIO = null)
	{
		if (nextSubIO == null)
		{
			nextSubIO = new List<Tuple<int, VmIO, VmIO>>();
		}
		foreach (VmIO child in Children)
		{
			nextSubIO.AddRange(child.ClearRelation());
		}
		RelateIndex = null;
		RelateIO = null;
		if (IsLeafNode)
		{
			ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST iMVS_MODULE_SUBSCRIBE_INFO_LIST = ServerSDKManager.serverSDKManager.mModuleManager.QueryModuleIOBeSubscribed(Root.ID, Name);
			ImvsSdkDefine.IMVS_SUBSCRIBE_INFO[] array = iMVS_MODULE_SUBSCRIBE_INFO_LIST.pstSubscribeInfo();
			for (int i = 0; i < iMVS_MODULE_SUBSCRIBE_INFO_LIST.nNum; i++)
			{
				ImvsSdkDefine.IMVS_SUBSCRIBE_INFO subScribeInfo = array[i];
				List<VmModule> subScribeVisible = Root.GetSubScribeVisible(Root.ID == 13000);
				VmModule vmModule = subScribeVisible.Single((VmModule p) => p.ID == subScribeInfo.nSubModuleID);
				string text = StringUtil.UTF8GetString(subScribeInfo.strSubInput);
				string text2 = StringUtil.UTF8GetString(subScribeInfo.strPubOutput);
				string text3 = StringUtil.UTF8GetString(subScribeInfo.strIndex);
				uint nIndexModuleID = subScribeInfo.nIndexModuleID;
				uint nIsIndexConstant = subScribeInfo.nIsIndexConstant;
				int item = 0;
				if (Name == text3 && nIndexModuleID == Root.ID)
				{
					item = 1;
					if (Name == text2)
					{
						item = 2;
					}
				}
				VmIO leafIO = vmModule.GetLeafIO(text);
				nextSubIO.Add(new Tuple<int, VmIO, VmIO>(item, leafIO, leafIO.RelateIndex));
			}
			foreach (Tuple<int, VmIO, VmIO> item4 in nextSubIO)
			{
				int item2 = item4.Item1;
				VmIO item3 = item4.Item2;
				switch (item2)
				{
				case 0:
					item3.RelateIndex = null;
					item3.RelateIO = null;
					break;
				case 1:
					item3.RelateIndex = null;
					break;
				case 2:
					item3.RelateIndex = null;
					item3.RelateIO = null;
					break;
				}
			}
		}
		return nextSubIO;
	}

	private void SetValue<T>(IEnumerable<T> newValue, bool needRaiseEvent = true)
	{
		if (newValue is IEnumerable<ImvsSdkDefine.IMVS_MODULE_CHUNK_VALUE_EX_Data> enumerable && TypeName != "image")
		{
			foreach (ImvsSdkDefine.IMVS_MODULE_CHUNK_VALUE_EX_Data item in enumerable)
			{
				item.FillData();
			}
		}
		_value = newValue;
		needMergeIOLoopValue = false;
		if (_valueList.Count == 0)
		{
			_valueList.Add(newValue);
		}
		else if (IsImageIOParent)
		{
			_valueList[0] = newValue;
		}
		else
		{
			_valueList.Add(newValue);
			needMergeIOLoopValue = true;
		}
		if (needRaiseEvent)
		{
			InvokeValueChangedEvent();
		}
	}

	internal void SetValueFromServer(ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_EX_Data struResultInfo)
	{
		switch (struResultInfo.nParamType)
		{
		case 1:
			SetValue(Array.ConvertAll(struResultInfo.pIntValue, (Converter<int, int?>)((int s) => s)));
			break;
		case 2:
			SetValue(Array.ConvertAll(struResultInfo.pFloatValue, (Converter<float, float?>)((float s) => s)));
			break;
		case 3:
			SetValue(struResultInfo.pStringValue);
			break;
		case 4:
			SetValue(struResultInfo.pImageValue);
			break;
		case 5:
			SetValue(struResultInfo.pChunkValue);
			break;
		case 6:
			SetValue(struResultInfo.pByteValue);
			break;
		case 7:
			SetValue(struResultInfo.pDoubleValue);
			break;
		}
	}

	/// <summary>
	/// 添加默认值null
	/// </summary>
	internal void SetValueFromDefault()
	{
		switch (TypeName.ToLower())
		{
		case "int":
			SetValue(new int?[1], needRaiseEvent: false);
			break;
		case "float":
			SetValue(new float?[1], needRaiseEvent: false);
			break;
		case "string":
			SetValue(new ImvsSdkDefine.IMVS_MODULE_STRING_VALUE_EX[1]
			{
				new ImvsSdkDefine.IMVS_MODULE_STRING_VALUE_EX
				{
					strValue = null
				}
			}, needRaiseEvent: false);
			break;
		case "double":
			SetValue(new double?[1], needRaiseEvent: false);
			break;
		}
	}

	internal void SetIsReturnRelateValueProperty(bool value)
	{
		IsReturnRelateValue = value;
		if (Children.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < Children.Count; i++)
		{
			if (Children[i].isReturnRelateValue != value)
			{
				Children[i].SetIsReturnRelateValueProperty(value);
			}
		}
	}

	public VmIO()
	{
		Childs.Parent = this;
		ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_EX_Data struResultInfo = new ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_EX_Data
		{
			nParamType = 1
		};
		SetValueFromServer(this, ref struResultInfo);
		struResultInfo.nParamType = 2;
		SetValueFromServer(this, ref struResultInfo);
		struResultInfo.nParamType = 3;
		SetValueFromServer(this, ref struResultInfo);
		struResultInfo.nParamType = 4;
		SetValueFromServer(this, ref struResultInfo);
		struResultInfo.nParamType = 5;
		SetValueFromServer(this, ref struResultInfo);
		struResultInfo.nParamType = 6;
		SetValueFromServer(this, ref struResultInfo);
		struResultInfo.nParamType = 7;
		SetValueFromServer(this, ref struResultInfo);
		_value = null;
	}

	internal static void SetValueFromServer(VmIO io, ref ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_EX_Data struResultInfo, bool needRaiseEvent = false)
	{
		if (io.TypeName == null)
		{
			return;
		}
		switch ((ModuleManager.ParamTypeEnum)struResultInfo.nParamType)
		{
		case ModuleManager.ParamTypeEnum.IntType:
			io.SetValue(Array.ConvertAll(struResultInfo.pIntValue, (Converter<int, int?>)((int s) => s)), needRaiseEvent);
			break;
		case ModuleManager.ParamTypeEnum.FloatType:
			io.SetValue(Array.ConvertAll(struResultInfo.pFloatValue, (Converter<float, float?>)((float s) => s)), needRaiseEvent);
			break;
		case ModuleManager.ParamTypeEnum.StringType:
			io.SetValue(struResultInfo.pStringValue, needRaiseEvent);
			break;
		case ModuleManager.ParamTypeEnum.ImageType:
			io.SetValue(struResultInfo.pImageValue, needRaiseEvent);
			break;
		case ModuleManager.ParamTypeEnum.ChunkType:
			io.SetValue(struResultInfo.pChunkValue, needRaiseEvent);
			break;
		case ModuleManager.ParamTypeEnum.ByteType:
			io.SetValue(struResultInfo.pByteValue, needRaiseEvent);
			break;
		case ModuleManager.ParamTypeEnum.DoubleType:
			io.SetValue(struResultInfo.pDoubleValue, needRaiseEvent);
			break;
		}
	}

	private void GetUnBindInfo(VmIO pubIO, VmIO subIO, ref List<ImvsSdkDefine.IMVS_UNBIND_GLOBALVAR_PARAM_INFO> UnbindList)
	{
		if (pubIO.IsLeafNode && subIO.IsLeafNode)
		{
			ImvsSdkDefine.IMVS_UNBIND_GLOBALVAR_PARAM_INFO item = new ImvsSdkDefine.IMVS_UNBIND_GLOBALVAR_PARAM_INFO
			{
				nModuleId = ((IVmOwner)pubIO).Owner.ID,
				strParamName = StringUtil.UTF8GetFixLenBytes(pubIO.Name, 128),
				stBindValue = new ImvsSdkDefine.IMVS_SUBSCRIBE_MODU_PARAM_INFO
				{
					eVarType = ImvsSdkDefine.IMVS_ENUM_SUBSCRIBE_VAR_TYPE.SUBSCRIBE_VAR_VARIABLE,
					stVariableValue = new ImvsSdkDefine.IMVS_VARIABLE_VALUE_INFO
					{
						eIndexType = ImvsSdkDefine.IMVS_ENUM_SUBSCRIBE_INDEX_TYPE.SUBSCRIBE_INDEX_ALL,
						strIndexConstant = StringUtil.UTF8GetFixLenBytes("All", 256),
						stModuleParam = new ImvsSdkDefine.IMVS_SUBSCRIBE_MODULE_PARAM_INFO
						{
							nModuleId = (int)((IVmOwner)subIO).Owner.ID,
							byParamName = StringUtil.UTF8GetFixLenBytes(subIO.Name, 128)
						}
					}
				}
			};
			switch (pubIO.TypeName)
			{
			case "int":
				item.eGlobalVarType = ImvsSdkDefine.IMVS_ENUM_MODULE_RESULT_TYPE.IMVS_MODURES_TYPE_INT;
				break;
			case "float":
				item.eGlobalVarType = ImvsSdkDefine.IMVS_ENUM_MODULE_RESULT_TYPE.IMVS_MODURES_TYPE_FLOAT;
				break;
			case "string":
				item.eGlobalVarType = ImvsSdkDefine.IMVS_ENUM_MODULE_RESULT_TYPE.IMVS_MODURES_TYPE_STRING;
				break;
			case "pointset":
				item.eGlobalVarType = ImvsSdkDefine.IMVS_ENUM_MODULE_RESULT_TYPE.IMVS_MODURES_TYPE_CHUNK;
				break;
			case "image":
				item.eGlobalVarType = ImvsSdkDefine.IMVS_ENUM_MODULE_RESULT_TYPE.IMVS_MODURES_TYPE_IMAGE;
				break;
			case "byte":
				item.eGlobalVarType = ImvsSdkDefine.IMVS_ENUM_MODULE_RESULT_TYPE.IMVS_MODURES_TYPE_BYTEARRAY;
				break;
			case "double":
				item.eGlobalVarType = ImvsSdkDefine.IMVS_ENUM_MODULE_RESULT_TYPE.IMVS_MODURES_TYPE_DOUBLE;
				break;
			}
			UnbindList.Add(item);
		}
		else
		{
			for (int i = 0; i < pubIO.Childs.Count; i++)
			{
				GetUnBindInfo(pubIO.Childs[i], subIO.Children[i], ref UnbindList);
			}
		}
	}

	/// <summary>
	/// 解绑与变量的绑定关系，不考虑IO的情况，适用于名称修改
	/// </summary>
	/// <returns></returns>
	public string UnBindingVariableWithNoIo(string varName, string varTypeName, VmModule varModule)
	{
		LogHelper.Info("输出参数" + ((IVmFrameBase)this).FullName + " try UnBinding Variable " + varName);
		if (varModule != null && ((IVmOwner)this).Owner != null)
		{
			ImvsSdkDefine.IMVS_UNBIND_GLOBALVAR_PARAM_INFO_LIST stGlobalVarInfo = new ImvsSdkDefine.IMVS_UNBIND_GLOBALVAR_PARAM_INFO_LIST
			{
				astBindGlobalVarList = new ImvsSdkDefine.IMVS_UNBIND_GLOBALVAR_PARAM_INFO[128]
			};
			ImvsSdkDefine.IMVS_UNBIND_GLOBALVAR_PARAM_INFO iMVS_UNBIND_GLOBALVAR_PARAM_INFO = new ImvsSdkDefine.IMVS_UNBIND_GLOBALVAR_PARAM_INFO
			{
				nModuleId = varModule.ID,
				strParamName = StringUtil.UTF8GetFixLenBytes(varName, 128),
				stBindValue = new ImvsSdkDefine.IMVS_SUBSCRIBE_MODU_PARAM_INFO
				{
					eVarType = ImvsSdkDefine.IMVS_ENUM_SUBSCRIBE_VAR_TYPE.SUBSCRIBE_VAR_VARIABLE,
					stVariableValue = new ImvsSdkDefine.IMVS_VARIABLE_VALUE_INFO
					{
						eIndexType = ImvsSdkDefine.IMVS_ENUM_SUBSCRIBE_INDEX_TYPE.SUBSCRIBE_INDEX_ALL,
						strIndexConstant = StringUtil.UTF8GetFixLenBytes("All", 256),
						stModuleParam = new ImvsSdkDefine.IMVS_SUBSCRIBE_MODULE_PARAM_INFO
						{
							nModuleId = (int)((IVmOwner)this).Owner.ID,
							byParamName = StringUtil.UTF8GetFixLenBytes(((IVmFrameBase)this).Name, 128)
						}
					}
				}
			};
			switch (varTypeName)
			{
			case "int":
				iMVS_UNBIND_GLOBALVAR_PARAM_INFO.eGlobalVarType = ImvsSdkDefine.IMVS_ENUM_MODULE_RESULT_TYPE.IMVS_MODURES_TYPE_INT;
				break;
			case "float":
				iMVS_UNBIND_GLOBALVAR_PARAM_INFO.eGlobalVarType = ImvsSdkDefine.IMVS_ENUM_MODULE_RESULT_TYPE.IMVS_MODURES_TYPE_FLOAT;
				break;
			case "string":
				iMVS_UNBIND_GLOBALVAR_PARAM_INFO.eGlobalVarType = ImvsSdkDefine.IMVS_ENUM_MODULE_RESULT_TYPE.IMVS_MODURES_TYPE_STRING;
				break;
			case "pointset":
				iMVS_UNBIND_GLOBALVAR_PARAM_INFO.eGlobalVarType = ImvsSdkDefine.IMVS_ENUM_MODULE_RESULT_TYPE.IMVS_MODURES_TYPE_CHUNK;
				break;
			case "image":
				iMVS_UNBIND_GLOBALVAR_PARAM_INFO.eGlobalVarType = ImvsSdkDefine.IMVS_ENUM_MODULE_RESULT_TYPE.IMVS_MODURES_TYPE_IMAGE;
				break;
			case "byte":
				iMVS_UNBIND_GLOBALVAR_PARAM_INFO.eGlobalVarType = ImvsSdkDefine.IMVS_ENUM_MODULE_RESULT_TYPE.IMVS_MODURES_TYPE_BYTEARRAY;
				break;
			case "double":
				iMVS_UNBIND_GLOBALVAR_PARAM_INFO.eGlobalVarType = ImvsSdkDefine.IMVS_ENUM_MODULE_RESULT_TYPE.IMVS_MODURES_TYPE_DOUBLE;
				break;
			}
			stGlobalVarInfo.astBindGlobalVarList[0] = iMVS_UNBIND_GLOBALVAR_PARAM_INFO;
			stGlobalVarInfo.nNum = 1u;
			try
			{
				ServerSDKManager.serverSDKManager.mGlobalModuleManager.UnBindGlobalVarParam(stGlobalVarInfo);
				((IVmIO)this).Tag = null;
				(this as ModelBase).OnPropertyChanged("Tag");
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return varName;
		}
		throw new Exception(AppLocalizationService.GetLang("Lang_GlobalVariableGetModuleListERR"));
	}

	public void UnBindingGlobalVariable(string varName, VmModule varModule)
	{
		LogHelper.Info("输出参数" + ((IVmFrameBase)this).FullName + " try UnBinding GlobalVariable " + varName);
		if (varModule != null && ((IVmOwner)this).Owner != null)
		{
			int iD = (int)varModule.ID;
			VmIO vmIO = varModule.Outputs.FirstOrDefault((VmIO x) => x.Name == varName);
			if (vmIO == null)
			{
				LogHelper.Error($"UnBindingVariableWithNoIo is error:io name is not exit:{varName}");
				UnBindingVariableWithNoIo(varName, ((ITypeName)this).TypeName, varModule);
			}
			ImvsSdkDefine.IMVS_UNBIND_GLOBALVAR_PARAM_INFO_LIST stGlobalVarInfo = new ImvsSdkDefine.IMVS_UNBIND_GLOBALVAR_PARAM_INFO_LIST
			{
				astBindGlobalVarList = new ImvsSdkDefine.IMVS_UNBIND_GLOBALVAR_PARAM_INFO[128]
			};
			List<ImvsSdkDefine.IMVS_UNBIND_GLOBALVAR_PARAM_INFO> UnbindList = new List<ImvsSdkDefine.IMVS_UNBIND_GLOBALVAR_PARAM_INFO>();
			GetUnBindInfo(vmIO, this, ref UnbindList);
			for (int num = 0; num < UnbindList.Count; num++)
			{
				stGlobalVarInfo.astBindGlobalVarList[num] = UnbindList[num];
			}
			stGlobalVarInfo.nNum = (uint)UnbindList.Count;
			try
			{
				ServerSDKManager.serverSDKManager.mGlobalModuleManager.UnBindGlobalVarParam(stGlobalVarInfo);
				((IVmIO)this).Tag = null;
				(this as ModelBase).OnPropertyChanged("Tag");
				return;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		throw new Exception(AppLocalizationService.GetLang("Lang_GlobalVariableGetModuleListERR"));
	}

	/// <summary>
	/// 解除与全局变量的绑定关系
	/// </summary>
	public string UnBindingGlobalVariable()
	{
		string globalVarName = null;
		if (((IVmIO)this).Tag != null)
		{
			IVariable variable = ((IVmIO)this).Tag as IVariable;
			globalVarName = variable?.Name;
			LogHelper.Info("输出参数" + ((IVmFrameBase)this).FullName + " try UnBinding GlobalVariable " + globalVarName);
			if (!(variable.Owner is VmModule vmModule) || ((IVmOwner)this).Owner == null)
			{
				throw new Exception(AppLocalizationService.GetLang("Lang_GlobalVariableGetModuleListERR"));
			}
			int iD = (int)vmModule.ID;
			VmIO vmIO = vmModule.Outputs.FirstOrDefault((VmIO x) => x.Name == globalVarName);
			if (vmIO == null)
			{
				LogHelper.Error($"UnBindingVariableWithNoIo is error:io name is not exit:{globalVarName}");
				UnBindingVariableWithNoIo(globalVarName, ((ITypeName)this).TypeName, vmModule);
				return globalVarName;
			}
			ImvsSdkDefine.IMVS_UNBIND_GLOBALVAR_PARAM_INFO_LIST stGlobalVarInfo = new ImvsSdkDefine.IMVS_UNBIND_GLOBALVAR_PARAM_INFO_LIST
			{
				astBindGlobalVarList = new ImvsSdkDefine.IMVS_UNBIND_GLOBALVAR_PARAM_INFO[128]
			};
			List<ImvsSdkDefine.IMVS_UNBIND_GLOBALVAR_PARAM_INFO> UnbindList = new List<ImvsSdkDefine.IMVS_UNBIND_GLOBALVAR_PARAM_INFO>();
			GetUnBindInfo(vmIO, this, ref UnbindList);
			for (int num = 0; num < UnbindList.Count; num++)
			{
				stGlobalVarInfo.astBindGlobalVarList[num] = UnbindList[num];
			}
			stGlobalVarInfo.nNum = (uint)UnbindList.Count;
			try
			{
				ServerSDKManager.serverSDKManager.mGlobalModuleManager.UnBindGlobalVarParam(stGlobalVarInfo);
				((IVmIO)this).Tag = null;
				(this as ModelBase).OnPropertyChanged("Tag");
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		return globalVarName;
	}
}
