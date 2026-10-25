using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Apps.Log;
using Apps.UIHelper.Extern;
using FrontendUI.Design.Controls;
using FrontendUI.Design.Utility;
using VM.Core;
using VM.PlatformSDKCS;
using VM.Utility;
using VMControls.Interface;

namespace FrontendUI.Design.Helper;

public class SolutionFileHelper
{
	public readonly object _Lock = new object();

	private const string _DESIGNERBUFFER_KEY = "DESIGNERBUFFER";

	private const string _Const_MultiImageData = "MultiImageData";

	private const string _Const_ColumnsData = "ColumnsData";

	private const string _Const_GridData = "GridData";

	private const string _Const_Document = "Document";

	private const string _Const_BindingPath = "BindingPath";

	private bool _isLoad = false;

	private string _tempStrDesingerBuffer;

	public bool IsModifyFromVM = false;

	public bool IsConvertFromVMDoing = false;

	public bool IsSolutionDesignerModified = false;

	private static object syncObj = new object();

	public static SolutionFileHelper instance = null;

	public MemoryStream SolutionDesignerBufferFromVM
	{
		get
		{
			if (string.IsNullOrWhiteSpace(_tempStrDesingerBuffer))
			{
				return null;
			}
			MemoryStream memoryStream = new MemoryStream();
			byte[] bytes = Encoding.UTF8.GetBytes(_tempStrDesingerBuffer);
			return new MemoryStream(bytes, 0, bytes.Length, writable: true, publiclyVisible: true);
		}
		set
		{
			if (value == null)
			{
				_tempStrDesingerBuffer = string.Empty;
			}
		}
	}

	public static SolutionFileHelper Instance => GetInstance();

	private SolutionFileHelper()
	{
		((IVmSolution)VmSolution.Instance).ModuleActionEvnetChangedCallBack += SolutionFileHelper_ModuleActionEvnetChanged;
		VmSolution.OnSolutionLoadBeginCallBack += VmSolution_OnSolutionLoadBeginCallBack;
		VmSolution.OnSolutionLoadEndCallBack += VmSolution_OnSolutionLoadEndCallBack;
		VmSolution.OnSolutionSaveBeginCallBack += VmSolution_OnSolutionSaveBeginCallBack;
		VmSolution.OnSolutionSaveEndCallBack += VmSolution_OnSolutionSaveEndCallBack;
		VmSolution.InstanceChanged += VmSolution_InstanceChanged;
	}

	private void VmSolution_InstanceChanged(object sender, EventArgs e)
	{
		GetInstance().ReSet();
	}

	private void VmSolution_OnSolutionSaveEndCallBack(object sender, EndEventArgs args)
	{
	}

	private void VmSolution_OnSolutionSaveBeginCallBack(object sender, ObjectEventArgs args)
	{
		DocumentManager.Instance().isSolutionDesignerBuffer_Loaded = false;
		GetInstance().SaveDesingerBuffer();
	}

	private void VmSolution_OnSolutionLoadEndCallBack(object sender, EndEventArgs args)
	{
		GetInstance().ReSet();
	}

	private void VmSolution_OnSolutionLoadBeginCallBack(object sender, ObjectEventArgs args)
	{
	}

	~SolutionFileHelper()
	{
		VmSolution.OnSolutionLoadBeginCallBack -= VmSolution_OnSolutionLoadBeginCallBack;
		VmSolution.OnSolutionLoadEndCallBack -= VmSolution_OnSolutionLoadEndCallBack;
		VmSolution.OnSolutionSaveBeginCallBack -= VmSolution_OnSolutionSaveBeginCallBack;
		VmSolution.OnSolutionSaveEndCallBack -= VmSolution_OnSolutionSaveEndCallBack;
		VmSolution.InstanceChanged -= VmSolution_InstanceChanged;
	}

	public static SolutionFileHelper GetInstance()
	{
		lock (syncObj)
		{
			if (instance == null)
			{
				instance = new SolutionFileHelper();
			}
			return instance;
		}
	}

	private void SolutionFileHelper_ModuleActionEvnetChanged(object sender, EventArgs e)
	{
		if (e is ModuleActionEventArgs e2)
		{
			ModuleNameValueChanged(e2.ActionName, e2.ModuleName, e2.NewModuleName);
		}
	}

	private void ModuleNameValueChanged(string actionName, string inModuleName, string inNewModuleName)
	{
		if (string.IsNullOrWhiteSpace(actionName) || string.IsNullOrWhiteSpace(inModuleName) || (actionName == "ReName" && string.IsNullOrWhiteSpace(inNewModuleName)))
		{
			return;
		}
		EditFrontendWindow frontendWindow = FrontendWindowInstanceHelper.GetFrontendWindow();
		Task.Run(() =>
		{
			try
			{
				lock (_Lock)
				{
					IsConvertFromVMDoing = true;
					LoadDesingerBuffer();
					if (!string.IsNullOrWhiteSpace(_tempStrDesingerBuffer))
					{
						if (actionName == "ReName")
						{
							string text = inModuleName;
							string text2 = inNewModuleName;
							if (!string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(text2))
							{
								ConvertReName(actionName, text, text2, ref _tempStrDesingerBuffer);
							}
						}
						else if ((actionName == "DeleteModule" || actionName == "DeleteProcedure" || actionName == "DeleteVMIO" || actionName == "DeleteObject") && !string.IsNullOrWhiteSpace(inModuleName))
						{
							ConvertDeleteModule(actionName, inModuleName, inNewModuleName, ref _tempStrDesingerBuffer);
						}
					}
				}
			}
			catch (Exception message)
			{
				LogHelper.frontedLog.Info(message);
			}
			finally
			{
				IsConvertFromVMDoing = false;
			}
		});
	}

	private void ConvertReName(string actionName, string inObjectName, string inNewObjectName, ref string orgTempStrDesingerBuffer)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(orgTempStrDesingerBuffer);
		ForeachControlCode(xmlDocument, actionName, inObjectName, inNewObjectName);
		string outerXml = xmlDocument.OuterXml;
		if (outerXml != orgTempStrDesingerBuffer)
		{
			orgTempStrDesingerBuffer = outerXml;
			if (!IsModifyFromVM)
			{
				IsModifyFromVM = true;
			}
		}
	}

	private void ConvertDeleteModule(string actionName, string inObjectName, string inNewObjectName, ref string orgTempStrDesingerBuffer)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(orgTempStrDesingerBuffer);
		ForeachControlCode(xmlDocument, actionName, inObjectName, inNewObjectName);
		string outerXml = xmlDocument.OuterXml;
		if (outerXml != orgTempStrDesingerBuffer)
		{
			orgTempStrDesingerBuffer = outerXml;
			if (!IsModifyFromVM)
			{
				IsModifyFromVM = true;
			}
		}
	}

	private static string GetValueStringFromKey(string sourceStr, string findKey)
	{
		string result = string.Empty;
		int length = findKey.Length;
		int num = sourceStr.IndexOf(findKey);
		if (num > -1)
		{
			int num2 = sourceStr.IndexOf("\"", num + length + 1);
			result = sourceStr.Substring(num, num2 - num + 1);
		}
		return result;
	}

	private void LoadDesingerBuffer()
	{
		if (_isLoad && !IsSolutionDesignerModified)
		{
			return;
		}
		MemoryStream memoryStream = null;
		byte[] array = VmSolution.Instance.UserData["DESIGNERBUFFER"];
		if (array != null)
		{
			memoryStream = new MemoryStream(array, 0, array.Length - 1, writable: true, publiclyVisible: true);
		}
		if (memoryStream != null)
		{
			XmlDocument xmlDocument = new XmlDocument();
			using (XmlReader reader = XmlReader.Create(memoryStream))
			{
				memoryStream = null;
				xmlDocument.Load(reader);
				_tempStrDesingerBuffer = xmlDocument.OuterXml.ToString();
			}
			_isLoad = true;
			IsSolutionDesignerModified = false;
		}
	}

	public void SaveDesingerBuffer()
	{
		try
		{
			if (IsModifyFromVM && !string.IsNullOrWhiteSpace(_tempStrDesingerBuffer))
			{
				MemoryStream memoryStream = new MemoryStream();
				byte[] bytes = Encoding.UTF8.GetBytes(_tempStrDesingerBuffer);
				memoryStream = new MemoryStream(bytes, 0, bytes.Length, writable: true, publiclyVisible: true);
				VmSolution.Instance.UserData["DESIGNERBUFFER"] = StringUtil.UTF8GetBytesPadZero(memoryStream.ToArray());
				ReSet();
			}
		}
		catch (Exception message)
		{
			LogHelper.frontedLog.Info(message);
		}
	}

	private void ReSet()
	{
		_isLoad = false;
		IsModifyFromVM = false;
		_tempStrDesingerBuffer = string.Empty;
		IsSolutionDesignerModified = false;
	}

	private void ForeachControlCode(XmlDocument doc, string actionName, string inObjectName, string inNewObjectName)
	{
		List<XmlNode> list = doc.FirstChild?.ChildNodes.OfType<XmlNode>().ToList();
		if (list == null)
		{
			return;
		}
		foreach (XmlNode item in list)
		{
			if (item.LocalName == "FrontendWindow.TitleBar")
			{
				XmlNode firstChild = item.FirstChild;
				if (firstChild == null || firstChild.LocalName != "CustomTitleBar")
				{
					continue;
				}
				XmlNode firstChild2 = firstChild.FirstChild;
				if (firstChild2 == null)
				{
					continue;
				}
				List<XmlElement> list2 = firstChild2.ChildNodes.OfType<XmlElement>().ToList();
				foreach (XmlElement item2 in list2)
				{
					if (!item2.LocalName.Contains("ColumnDefinitions") && !item2.LocalName.Contains("RowDefinitions") && !item2.LocalName.Contains("SubBorder") && !item2.LocalName.Contains("TitleBarHandleForRoot"))
					{
						CleanOneXmlElementInfo(item2, actionName, inObjectName, inNewObjectName);
					}
				}
			}
			else
			{
				if (!(item.LocalName == "DockControl"))
				{
					continue;
				}
				foreach (XmlNode childNode in item.ChildNodes)
				{
					switch (childNode.LocalName)
					{
					case "DockControl.BottomBar":
						foreach (XmlElement childNode2 in childNode.ChildNodes)
						{
							if (childNode2 != null && !(childNode2.LocalName != "StatusBar"))
							{
							}
						}
						break;
					case "DockControl.RightBar":
					case "DockControl.LeftBar":
					{
						if (!(childNode.FirstChild.LocalName == "SideBar"))
						{
							break;
						}
						List<XmlNode> list4 = childNode.FirstChild.ChildNodes.OfType<XmlNode>().ToList();
						if (list4 == null)
						{
							break;
						}
						foreach (XmlElement item3 in list4)
						{
							if (!item3.LocalName.Contains("ColumnDefinitions") && !item3.LocalName.Contains("RowDefinitions"))
							{
								CleanOneXmlElementInfo(item3, actionName, inObjectName, inNewObjectName);
							}
						}
						break;
					}
					case "FrontendTabControl":
						foreach (XmlElement childNode3 in childNode.ChildNodes)
						{
							if (childNode3 == null || childNode3.LocalName != "FrontendTabItem" || !(childNode3.FirstChild.LocalName == "SubRoot"))
							{
								continue;
							}
							List<XmlNode> list3 = childNode3.FirstChild.ChildNodes.OfType<XmlNode>().ToList();
							if (list3 == null)
							{
								continue;
							}
							foreach (XmlElement item4 in list3)
							{
								if (!item4.LocalName.Contains("ColumnDefinitions") && !item4.LocalName.Contains("RowDefinitions") && !item4.LocalName.Contains("SubBorder"))
								{
									CleanOneXmlElementInfo(item4, actionName, inObjectName, inNewObjectName);
								}
							}
						}
						break;
					}
				}
			}
		}
	}

	private void CleanOneXmlElementInfo(XmlElement xmlElement, string actionName, string inObjectName, string inNewObjectName)
	{
		if (xmlElement == null)
		{
			return;
		}
		switch (xmlElement.LocalName)
		{
		case "SubWindow":
		{
			XmlNode namedItem = xmlElement.Attributes.GetNamedItem("Document");
			if (namedItem != null && !string.IsNullOrWhiteSpace(namedItem.Value))
			{
				string xml = namedItem.Value.Base64StringToString();
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(xml);
				namedItem.Value = CleanSubRootXmlElement(xmlDocument, actionName, inObjectName, inNewObjectName);
			}
			return;
		}
		case "Controller":
		{
			if (!(xmlElement.Attributes.GetNamedItem("DataSourceInfo") is XmlAttribute xmlAttribute11) || string.IsNullOrWhiteSpace(xmlAttribute11.Value))
			{
				return;
			}
			string value6 = xmlAttribute11.Value;
			if (value6 == null || !value6.Contains(inObjectName))
			{
				return;
			}
			switch (actionName)
			{
			case "ReName":
				xmlAttribute11.Value = value6.Replace(inObjectName, inNewObjectName);
				break;
			default:
				if (!(actionName == "DeleteObject"))
				{
					break;
				}
				goto case "DeleteModule";
			case "DeleteModule":
			case "DeleteProcedure":
			case "DeleteVMIO":
				xmlElement.Attributes.Remove(xmlAttribute11);
				break;
			}
			return;
		}
		case "TableControl":
		{
			if (!(xmlElement.Attributes.GetNamedItem("ColumnsData") is XmlAttribute xmlAttribute7) || string.IsNullOrWhiteSpace(xmlAttribute7.Value))
			{
				return;
			}
			ExpandableCollection<DataColumnItem> expandableCollection2 = XElement.Parse(xmlAttribute7.Value.Base64StringToString().Substring(1)).DeSerialization<ExpandableCollection<DataColumnItem>>();
			if (expandableCollection2 == null)
			{
				return;
			}
			foreach (DataColumnItem item in expandableCollection2)
			{
				if (string.IsNullOrWhiteSpace(item.BindingPath) || !item.BindingPath.Contains(inObjectName))
				{
					continue;
				}
				switch (actionName)
				{
				case "ReName":
					item.BindingPath = item.BindingPath.Replace(inObjectName, inNewObjectName);
					continue;
				default:
					if (!(actionName == "DeleteObject"))
					{
						continue;
					}
					break;
				case "DeleteModule":
				case "DeleteProcedure":
				case "DeleteVMIO":
					break;
				}
				item.BindingPath = null;
			}
			xmlAttribute7.Value = expandableCollection2.ToXml(omitXmlDeclaration: true).String2Base64();
			expandableCollection2 = null;
			return;
		}
		case "MultiImageControl":
		{
			if (!(xmlElement.Attributes.GetNamedItem("MultiImageData") is XmlAttribute xmlAttribute) || string.IsNullOrWhiteSpace(xmlAttribute.Value))
			{
				return;
			}
			ExpandableCollection<DataMultiImageItem> expandableCollection = XElement.Parse(xmlAttribute.Value.Base64StringToString().Substring(1)).DeSerialization<ExpandableCollection<DataMultiImageItem>>();
			if (expandableCollection == null)
			{
				return;
			}
			foreach (DataMultiImageItem item2 in expandableCollection)
			{
				if (string.IsNullOrWhiteSpace(item2.ImageSourceInfo))
				{
					continue;
				}
				if (item2.ImageSourceInfo.Contains(inObjectName))
				{
					switch (actionName)
					{
					case "ReName":
						item2.ImageSourceInfo = item2.ImageSourceInfo.Replace(inObjectName, inNewObjectName);
						break;
					default:
						if (!(actionName == "DeleteObject"))
						{
							break;
						}
						goto case "DeleteModule";
					case "DeleteModule":
					case "DeleteProcedure":
					case "DeleteVMIO":
						item2.ImageSourceInfo = null;
						item2.BindingPath = null;
						item2.LayerOverlayInfo1 = null;
						item2.LayerOverlayInfo2 = null;
						item2.LayerOverlayInfo3 = null;
						item2.ShapeObjectPath = null;
						break;
					}
				}
				if (item2.ShapeObjectPath != null && item2.ShapeObjectPath.Contains(inObjectName))
				{
					switch (actionName)
					{
					case "ReName":
						item2.ShapeObjectPath = item2.ShapeObjectPath.Replace(inObjectName, inNewObjectName);
						break;
					default:
						if (!(actionName == "DeleteObject"))
						{
							break;
						}
						goto case "DeleteModule";
					case "DeleteModule":
					case "DeleteProcedure":
					case "DeleteVMIO":
						item2.ShapeObjectPath = null;
						break;
					}
				}
				if (item2.LayerOverlayInfo1 != null && item2.LayerOverlayInfo1.Contains(inObjectName))
				{
					switch (actionName)
					{
					case "ReName":
						item2.LayerOverlayInfo1 = item2.LayerOverlayInfo1.Replace(inObjectName, inNewObjectName);
						break;
					default:
						if (!(actionName == "DeleteObject"))
						{
							break;
						}
						goto case "DeleteModule";
					case "DeleteModule":
					case "DeleteProcedure":
					case "DeleteVMIO":
						item2.LayerOverlayInfo1 = null;
						break;
					}
				}
				if (item2.LayerOverlayInfo2 != null && item2.LayerOverlayInfo2.Contains(inObjectName))
				{
					switch (actionName)
					{
					case "ReName":
						item2.LayerOverlayInfo2 = item2.LayerOverlayInfo2.Replace(inObjectName, inNewObjectName);
						break;
					default:
						if (!(actionName == "DeleteObject"))
						{
							break;
						}
						goto case "DeleteModule";
					case "DeleteModule":
					case "DeleteProcedure":
					case "DeleteVMIO":
						item2.LayerOverlayInfo2 = null;
						break;
					}
				}
				if (item2.LayerOverlayInfo3 == null || !item2.LayerOverlayInfo3.Contains(inObjectName))
				{
					continue;
				}
				switch (actionName)
				{
				case "ReName":
					item2.LayerOverlayInfo3 = item2.LayerOverlayInfo3.Replace(inObjectName, inNewObjectName);
					continue;
				default:
					if (!(actionName == "DeleteObject"))
					{
						continue;
					}
					break;
				case "DeleteModule":
				case "DeleteProcedure":
				case "DeleteVMIO":
					break;
				}
				item2.LayerOverlayInfo3 = null;
			}
			xmlAttribute.Value = expandableCollection.ToXml(omitXmlDeclaration: true).String2Base64();
			expandableCollection = null;
			return;
		}
		case "WorkStatusControl":
		{
			if (!(xmlElement.Attributes.GetNamedItem("DataSourceInfo") is XmlAttribute xmlAttribute18) || string.IsNullOrWhiteSpace(xmlAttribute18.Value))
			{
				return;
			}
			string value11 = xmlAttribute18.Value;
			if (value11 == null || !value11.Contains(inObjectName))
			{
				return;
			}
			switch (actionName)
			{
			case "ReName":
				xmlAttribute18.Value = value11.Replace(inObjectName, inNewObjectName);
				break;
			default:
				if (!(actionName == "DeleteObject"))
				{
					break;
				}
				goto case "DeleteModule";
			case "DeleteModule":
			case "DeleteProcedure":
			case "DeleteVMIO":
				xmlElement.Attributes.Remove(xmlAttribute18);
				break;
			}
			return;
		}
		case "MethodButton":
		{
			if (xmlElement.Attributes.GetNamedItem("DataPathA") is XmlAttribute xmlAttribute14 && !string.IsNullOrWhiteSpace(xmlAttribute14.Value))
			{
				string value7 = xmlAttribute14.Value;
				if (value7 != null && value7.Contains(inObjectName))
				{
					switch (actionName)
					{
					case "ReName":
						xmlAttribute14.Value = value7.Replace(inObjectName, inNewObjectName);
						break;
					default:
						if (!(actionName == "DeleteObject"))
						{
							break;
						}
						goto case "DeleteModule";
					case "DeleteModule":
					case "DeleteProcedure":
					case "DeleteVMIO":
						xmlElement.Attributes.Remove(xmlAttribute14);
						break;
					}
				}
			}
			if (xmlElement.Attributes.GetNamedItem("FuncParameterA") is XmlAttribute xmlAttribute15 && !string.IsNullOrWhiteSpace(xmlAttribute15.Value))
			{
				string value8 = xmlAttribute15.Value;
				if (value8 != null && value8.Contains(inObjectName))
				{
					switch (actionName)
					{
					case "ReName":
						xmlAttribute15.Value = value8.Replace(inObjectName, inNewObjectName);
						break;
					default:
						if (!(actionName == "DeleteObject"))
						{
							break;
						}
						goto case "DeleteModule";
					case "DeleteModule":
					case "DeleteProcedure":
					case "DeleteVMIO":
						xmlElement.Attributes.Remove(xmlAttribute15);
						break;
					}
				}
			}
			if (xmlElement.Attributes.GetNamedItem("MethodTypeA") is XmlAttribute xmlAttribute16 && !string.IsNullOrWhiteSpace(xmlAttribute16.Value))
			{
				string value9 = xmlAttribute16.Value;
				if (value9 != null && value9.Contains(inObjectName))
				{
					switch (actionName)
					{
					case "ReName":
						xmlAttribute16.Value = value9.Replace(inObjectName, inNewObjectName);
						break;
					default:
						if (!(actionName == "DeleteObject"))
						{
							break;
						}
						goto case "DeleteModule";
					case "DeleteModule":
					case "DeleteProcedure":
					case "DeleteVMIO":
						xmlElement.Attributes.Remove(xmlAttribute16);
						break;
					}
				}
			}
			if (!(xmlElement.Attributes.GetNamedItem("ModuleFullNameA") is XmlAttribute xmlAttribute17) || string.IsNullOrWhiteSpace(xmlAttribute17.Value))
			{
				return;
			}
			string value10 = xmlAttribute17.Value;
			if (value10 == null || !value10.Contains(inObjectName))
			{
				return;
			}
			switch (actionName)
			{
			case "ReName":
				xmlAttribute17.Value = value10.Replace(inObjectName, inNewObjectName);
				break;
			default:
				if (!(actionName == "DeleteObject"))
				{
					break;
				}
				goto case "DeleteModule";
			case "DeleteModule":
			case "DeleteProcedure":
			case "DeleteVMIO":
				xmlElement.Attributes.Remove(xmlAttribute17);
				break;
			}
			return;
		}
		case "ImageControl":
		{
			if (xmlElement.Attributes.GetNamedItem("ImageSourceInfo") is XmlAttribute xmlAttribute2 && !string.IsNullOrWhiteSpace(xmlAttribute2.Value))
			{
				string value = xmlAttribute2.Value;
				if (value != null && value.Contains(inObjectName))
				{
					switch (actionName)
					{
					case "ReName":
						xmlAttribute2.Value = value.Replace(inObjectName, inNewObjectName);
						break;
					default:
						if (!(actionName == "DeleteObject"))
						{
							break;
						}
						goto case "DeleteModule";
					case "DeleteModule":
					case "DeleteProcedure":
					case "DeleteVMIO":
						xmlElement.Attributes.Remove(xmlAttribute2);
						break;
					}
				}
			}
			if (xmlElement.Attributes.GetNamedItem("ShapeObjectPath") is XmlAttribute xmlAttribute3 && !string.IsNullOrWhiteSpace(xmlAttribute3.Value))
			{
				string value2 = xmlAttribute3.Value;
				if (value2 != null && value2.Contains(inObjectName))
				{
					switch (actionName)
					{
					case "ReName":
						xmlAttribute3.Value = value2.Replace(inObjectName, inNewObjectName);
						break;
					default:
						if (!(actionName == "DeleteObject"))
						{
							break;
						}
						goto case "DeleteModule";
					case "DeleteModule":
					case "DeleteProcedure":
					case "DeleteVMIO":
						xmlElement.Attributes.Remove(xmlAttribute3);
						break;
					}
				}
			}
			if (xmlElement.Attributes.GetNamedItem("LayerOverlayInfo1") is XmlAttribute xmlAttribute4 && !string.IsNullOrWhiteSpace(xmlAttribute4.Value))
			{
				string value3 = xmlAttribute4.Value;
				if (value3 != null && value3.Contains(inObjectName))
				{
					switch (actionName)
					{
					case "ReName":
						xmlAttribute4.Value = value3.Replace(inObjectName, inNewObjectName);
						break;
					default:
						if (!(actionName == "DeleteObject"))
						{
							break;
						}
						goto case "DeleteModule";
					case "DeleteModule":
					case "DeleteProcedure":
					case "DeleteVMIO":
						xmlElement.Attributes.Remove(xmlAttribute4);
						break;
					}
				}
			}
			if (xmlElement.Attributes.GetNamedItem("LayerOverlayInfo2") is XmlAttribute xmlAttribute5 && !string.IsNullOrWhiteSpace(xmlAttribute5.Value))
			{
				string value4 = xmlAttribute5.Value;
				if (value4 != null && value4.Contains(inObjectName))
				{
					switch (actionName)
					{
					case "ReName":
						xmlAttribute5.Value = value4.Replace(inObjectName, inNewObjectName);
						break;
					default:
						if (!(actionName == "DeleteObject"))
						{
							break;
						}
						goto case "DeleteModule";
					case "DeleteModule":
					case "DeleteProcedure":
					case "DeleteVMIO":
						xmlElement.Attributes.Remove(xmlAttribute5);
						break;
					}
				}
			}
			if (!(xmlElement.Attributes.GetNamedItem("LayerOverlayInfo3") is XmlAttribute xmlAttribute6) || string.IsNullOrWhiteSpace(xmlAttribute6.Value))
			{
				return;
			}
			string value5 = xmlAttribute6.Value;
			if (value5 == null || !value5.Contains(inObjectName))
			{
				return;
			}
			switch (actionName)
			{
			case "ReName":
				xmlAttribute6.Value = value5.Replace(inObjectName, inNewObjectName);
				break;
			default:
				if (!(actionName == "DeleteObject"))
				{
					break;
				}
				goto case "DeleteModule";
			case "DeleteModule":
			case "DeleteProcedure":
			case "DeleteVMIO":
				xmlElement.Attributes.Remove(xmlAttribute6);
				break;
			}
			return;
		}
		case "LabelArray":
		{
			if (!(xmlElement.Attributes.GetNamedItem("BindingPath") is XmlAttribute xmlAttribute8) || string.IsNullOrWhiteSpace(xmlAttribute8.Value))
			{
				return;
			}
			ExpandableCollection<LabelItem> expandableCollection3 = XElement.Parse(xmlAttribute8.Value.Base64StringToString().Substring(1)).DeSerialization<ExpandableCollection<LabelItem>>();
			if (expandableCollection3 == null)
			{
				return;
			}
			foreach (LabelItem item3 in expandableCollection3)
			{
				if (string.IsNullOrWhiteSpace(item3.BindPath) || !item3.BindPath.Contains(inObjectName))
				{
					continue;
				}
				switch (actionName)
				{
				case "ReName":
					item3.BindPath = item3.BindPath.Replace(inObjectName, inNewObjectName);
					continue;
				default:
					if (!(actionName == "DeleteObject"))
					{
						continue;
					}
					break;
				case "DeleteModule":
				case "DeleteProcedure":
				case "DeleteVMIO":
					break;
				}
				item3.BindPath = null;
			}
			xmlAttribute8.Value = expandableCollection3.ToXml(omitXmlDeclaration: true).String2Base64();
			expandableCollection3 = null;
			return;
		}
		case "LineDiagramArray":
		{
			if (!(xmlElement.Attributes.GetNamedItem("BindingPath") is XmlAttribute xmlAttribute12) || string.IsNullOrWhiteSpace(xmlAttribute12.Value))
			{
				return;
			}
			ExpandableCollection<LineGraphItem> expandableCollection6 = XElement.Parse(xmlAttribute12.Value.Base64StringToString().Substring(1)).DeSerialization<ExpandableCollection<LineGraphItem>>();
			if (expandableCollection6 == null)
			{
				return;
			}
			foreach (LineGraphItem item4 in expandableCollection6)
			{
				if (string.IsNullOrWhiteSpace(item4.BindPath) || !item4.BindPath.Contains(inObjectName))
				{
					continue;
				}
				switch (actionName)
				{
				case "ReName":
					item4.BindPath = item4.BindPath.Replace(inObjectName, inNewObjectName);
					continue;
				default:
					if (!(actionName == "DeleteObject"))
					{
						continue;
					}
					break;
				case "DeleteModule":
				case "DeleteProcedure":
				case "DeleteVMIO":
					break;
				}
				item4.BindPath = null;
			}
			xmlAttribute12.Value = expandableCollection6.ToXml(omitXmlDeclaration: true).String2Base64();
			expandableCollection6 = null;
			if (xmlElement.Attributes.GetNamedItem("BindingPathUndo") is XmlAttribute xmlAttribute13 && !string.IsNullOrWhiteSpace(xmlAttribute13.Value))
			{
				xmlAttribute13.Value = xmlAttribute12.Value;
			}
			return;
		}
		case "MultilineDiagram":
		{
			if (!(xmlElement.Attributes.GetNamedItem("BindingPath") is XmlAttribute xmlAttribute21) || string.IsNullOrWhiteSpace(xmlAttribute21.Value))
			{
				return;
			}
			ExpandableCollection<LineGraphItem> expandableCollection8 = XElement.Parse(xmlAttribute21.Value.Base64StringToString().Substring(1)).DeSerialization<ExpandableCollection<LineGraphItem>>();
			if (expandableCollection8 == null)
			{
				return;
			}
			foreach (LineGraphItem item5 in expandableCollection8)
			{
				if (string.IsNullOrWhiteSpace(item5.BindPath) || !item5.BindPath.Contains(inObjectName))
				{
					continue;
				}
				switch (actionName)
				{
				case "ReName":
					item5.BindPath = item5.BindPath.Replace(inObjectName, inNewObjectName);
					continue;
				default:
					if (!(actionName == "DeleteObject"))
					{
						continue;
					}
					break;
				case "DeleteModule":
				case "DeleteProcedure":
				case "DeleteVMIO":
					break;
				}
				item5.BindPath = null;
			}
			xmlAttribute21.Value = expandableCollection8.ToXml(omitXmlDeclaration: true).String2Base64();
			expandableCollection8 = null;
			if (xmlElement.Attributes.GetNamedItem("BindingPathUndo") is XmlAttribute xmlAttribute22 && !string.IsNullOrWhiteSpace(xmlAttribute22.Value))
			{
				xmlAttribute22.Value = xmlAttribute21.Value;
			}
			return;
		}
		case "ParamSettingsArray":
		{
			if (!(xmlElement.Attributes.GetNamedItem("BindingPath") is XmlAttribute xmlAttribute9) || string.IsNullOrWhiteSpace(xmlAttribute9.Value))
			{
				return;
			}
			ExpandableCollection<FrontendUI.Design.Controls.ParamItem> expandableCollection4 = XElement.Parse(xmlAttribute9.Value.Base64StringToString().Substring(1)).DeSerialization<ExpandableCollection<FrontendUI.Design.Controls.ParamItem>>();
			if (expandableCollection4 == null)
			{
				return;
			}
			foreach (FrontendUI.Design.Controls.ParamItem item6 in expandableCollection4)
			{
				if (string.IsNullOrWhiteSpace(item6.BindPath) || !item6.BindPath.Contains(inObjectName))
				{
					continue;
				}
				switch (actionName)
				{
				case "ReName":
					item6.BindPath = item6.BindPath.Replace(inObjectName, inNewObjectName);
					continue;
				default:
					if (!(actionName == "DeleteObject"))
					{
						continue;
					}
					break;
				case "DeleteModule":
				case "DeleteProcedure":
				case "DeleteVMIO":
					break;
				}
				item6.BindPath = null;
			}
			xmlAttribute9.Value = expandableCollection4.ToXml(omitXmlDeclaration: true).String2Base64();
			expandableCollection4 = null;
			return;
		}
		case "PieControl":
		{
			if (!(xmlElement.Attributes.GetNamedItem("BindingPath") is XmlAttribute xmlAttribute19) || string.IsNullOrWhiteSpace(xmlAttribute19.Value))
			{
				return;
			}
			ExpandableCollection<PieControl.LegendItem> expandableCollection7 = XElement.Parse(xmlAttribute19.Value.Base64StringToString().Substring(1)).DeSerialization<ExpandableCollection<PieControl.LegendItem>>();
			if (expandableCollection7 == null)
			{
				return;
			}
			foreach (PieControl.LegendItem item7 in expandableCollection7)
			{
				if (string.IsNullOrWhiteSpace(item7.BindPath) || !item7.BindPath.Contains(inObjectName))
				{
					continue;
				}
				switch (actionName)
				{
				case "ReName":
					item7.BindPath = item7.BindPath.Replace(inObjectName, inNewObjectName);
					continue;
				default:
					if (!(actionName == "DeleteObject"))
					{
						continue;
					}
					break;
				case "DeleteModule":
				case "DeleteProcedure":
				case "DeleteVMIO":
					break;
				}
				item7.BindPath = null;
			}
			xmlAttribute19.Value = expandableCollection7.ToXml(omitXmlDeclaration: true).String2Base64();
			expandableCollection7 = null;
			if (xmlElement.Attributes.GetNamedItem("BindingPathUndo") is XmlAttribute xmlAttribute20 && !string.IsNullOrWhiteSpace(xmlAttribute20.Value))
			{
				xmlAttribute20.Value = xmlAttribute19.Value;
			}
			return;
		}
		case "ProductionStatistics":
		{
			if (!(xmlElement.Attributes.GetNamedItem("BindingPath") is XmlAttribute xmlAttribute10) || string.IsNullOrWhiteSpace(xmlAttribute10.Value))
			{
				return;
			}
			ExpandableCollection<StatisticalItem> expandableCollection5 = XElement.Parse(xmlAttribute10.Value.Base64StringToString().Substring(1)).DeSerialization<ExpandableCollection<StatisticalItem>>();
			if (expandableCollection5 == null)
			{
				return;
			}
			foreach (StatisticalItem item8 in expandableCollection5)
			{
				if (string.IsNullOrWhiteSpace(item8.BindPath) || !item8.BindPath.Contains(inObjectName))
				{
					continue;
				}
				switch (actionName)
				{
				case "ReName":
					item8.BindPath = item8.BindPath.Replace(inObjectName, inNewObjectName);
					continue;
				default:
					if (!(actionName == "DeleteObject"))
					{
						continue;
					}
					break;
				case "DeleteModule":
				case "DeleteProcedure":
				case "DeleteVMIO":
					break;
				}
				item8.BindPath = null;
			}
			xmlAttribute10.Value = expandableCollection5.ToXml(omitXmlDeclaration: true).String2Base64();
			expandableCollection5 = null;
			return;
		}
		case "GroupBox":
			if (!(xmlElement.FirstChild is XmlElement { ChildNodes: not null } xmlElement4))
			{
				return;
			}
			{
				foreach (object childNode in xmlElement4.ChildNodes)
				{
					if (childNode is XmlElement xmlElement5)
					{
						CleanOneXmlElementInfo(xmlElement5, actionName, inObjectName, inNewObjectName);
					}
				}
				return;
			}
		case "Tab":
		{
			foreach (XmlElement childNode2 in xmlElement.ChildNodes)
			{
				if (childNode2 == null || childNode2.LocalName != "TabColumnItem" || childNode2.FirstChild == null || !(childNode2.FirstChild.LocalName == "Grid"))
				{
					continue;
				}
				XmlNodeList childNodes = childNode2.FirstChild.ChildNodes;
				if (childNodes == null)
				{
					continue;
				}
				foreach (XmlElement item9 in childNodes)
				{
					if (item9 != null)
					{
						CleanOneXmlElementInfo(item9, actionName, inObjectName, inNewObjectName);
					}
				}
			}
			return;
		}
		}
		if (!(xmlElement.Attributes.GetNamedItem("BindingPath") is XmlAttribute xmlAttribute23) || string.IsNullOrWhiteSpace(xmlAttribute23.Value))
		{
			return;
		}
		string value12 = xmlAttribute23.Value;
		if (value12 == null || !value12.Contains(inObjectName))
		{
			return;
		}
		switch (actionName)
		{
		case "ReName":
			xmlAttribute23.Value = value12.Replace(inObjectName, inNewObjectName);
			break;
		default:
			if (!(actionName == "DeleteObject"))
			{
				break;
			}
			goto case "DeleteModule";
		case "DeleteModule":
		case "DeleteProcedure":
		case "DeleteVMIO":
			xmlElement.Attributes.Remove(xmlAttribute23);
			break;
		}
	}

	private string CleanSubRootXmlElement(XmlDocument doc, string actionName, string inObjectName, string inNewObjectName)
	{
		string empty = string.Empty;
		if (doc == null || doc.FirstChild == null)
		{
			return empty;
		}
		foreach (XmlElement childNode in doc.FirstChild.ChildNodes)
		{
			CleanOneXmlElementInfo(childNode, actionName, inObjectName, inNewObjectName);
		}
		return doc.OuterXml.String2Base64();
	}
}
