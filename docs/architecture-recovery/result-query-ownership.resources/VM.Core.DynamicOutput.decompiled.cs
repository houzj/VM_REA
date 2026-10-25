using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Apps.Log;
using VM.PlatformSDKCS;

namespace VM.Core;

/// <summary>
/// DynamicOutput3D
/// </summary>
public class DynamicOutput
{
	private VmModule moduTool;

	private uint m_nModuleId;

	public DynamicOutput(VmModule ModuTool)
	{
		if (ModuTool == null)
		{
			throw new VmException(-536870911);
		}
		m_nModuleId = ModuTool.ID;
		moduTool = ModuTool;
	}

	/// <summary>
	/// 获取整型输出(结果为空时nValueNum为0且pIntVal和nReserved为null)
	/// </summary>
	/// <param name="strParam"></param>
	/// <returns></returns>
	public IntDataArray GetOutputInt(string strParam)
	{
		if (string.IsNullOrEmpty(strParam))
		{
			VmException ex = new VmException(-536870911);
			throw ex;
		}
		if (!moduTool.GetOutputIOByName("%" + strParam + "%", out var _))
		{
			LogHelper.Error("Cannot Find IO Name " + strParam.ToString());
			throw new VmException(-536870881, "Cannot Find IO " + strParam.ToString() + " From Name");
		}
		IntDataArray result = default;
		ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX? iMVS_MODULE_RESULT_INFO_LIST_EX = ServerSDKManager.serverSDKManager.mModuleManager.QueryModuleOneResult(m_nModuleId);
		if (!iMVS_MODULE_RESULT_INFO_LIST_EX.HasValue)
		{
			return result;
		}
		ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX_Data iMVS_MODULE_RESULT_INFO_LIST_EX_Data = ServerSDKManager.serverSDKManager.mModuleManager.PreProData(iMVS_MODULE_RESULT_INFO_LIST_EX.Value);
		for (int i = 0; i < iMVS_MODULE_RESULT_INFO_LIST_EX_Data.nResultNum; i++)
		{
			if (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[i].nParamType != 1)
			{
				continue;
			}
			string text = DeleteSpecifyChar(iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[i].strParamName, "%");
			if (strParam == text)
			{
				result.nValueNum = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[i].nValueNum;
				result.pIntVal = new int[result.nValueNum];
				for (int j = 0; j < result.nValueNum; j++)
				{
					result.pIntVal[j] = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[i].pIntValue[j];
				}
				break;
			}
		}
		return result;
	}

	/// <summary>
	/// 获取浮点型输出(结果为空时nValueNum为0且pFloatVal和nReserved为null)
	/// </summary>
	/// <param name="strParam"></param>
	/// <returns></returns>
	public FloatDataArray GetOutputFloat(string strParam)
	{
		if (string.IsNullOrEmpty(strParam))
		{
			VmException ex = new VmException(-536870911);
			throw ex;
		}
		if (!moduTool.GetOutputIOByName("%" + strParam + "%", out var _))
		{
			LogHelper.Error("Cannot Find IO Name " + strParam.ToString());
			throw new VmException(-536870881, "Cannot Find IO " + strParam.ToString() + " From Name");
		}
		FloatDataArray result = default;
		ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX? iMVS_MODULE_RESULT_INFO_LIST_EX = ServerSDKManager.serverSDKManager.mModuleManager.QueryModuleOneResult(m_nModuleId);
		if (!iMVS_MODULE_RESULT_INFO_LIST_EX.HasValue)
		{
			return result;
		}
		ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX_Data iMVS_MODULE_RESULT_INFO_LIST_EX_Data = ServerSDKManager.serverSDKManager.mModuleManager.PreProData(iMVS_MODULE_RESULT_INFO_LIST_EX.Value);
		for (int i = 0; i < iMVS_MODULE_RESULT_INFO_LIST_EX_Data.nResultNum; i++)
		{
			if (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[i].nParamType != 2)
			{
				continue;
			}
			string text = DeleteSpecifyChar(iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[i].strParamName, "%");
			if (strParam == text)
			{
				result.nValueNum = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[i].nValueNum;
				result.pFloatVal = new float[result.nValueNum];
				for (int j = 0; j < result.nValueNum; j++)
				{
					result.pFloatVal[j] = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[i].pFloatValue[j];
				}
				break;
			}
		}
		return result;
	}

	/// <summary>
	/// 获取字符串型输出(结果为空时nValueNum为0且astStringVal和nReserved为null)
	/// </summary>
	/// <param name="strParam">参数名称</param>
	/// <returns></returns>
	public StringDataArray GetOutputString(string strParam)
	{
		if (string.IsNullOrEmpty(strParam))
		{
			VmException ex = new VmException(-536870911);
			throw ex;
		}
		if (!moduTool.GetOutputIOByName("%" + strParam + "%", out var _))
		{
			LogHelper.Error("Cannot Find IO Name " + strParam.ToString());
			throw new VmException(-536870881, "Cannot Find IO " + strParam.ToString() + " From Name");
		}
		StringDataArray result = default;
		ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX? iMVS_MODULE_RESULT_INFO_LIST_EX = ServerSDKManager.serverSDKManager.mModuleManager.QueryModuleOneResult(m_nModuleId);
		if (!iMVS_MODULE_RESULT_INFO_LIST_EX.HasValue)
		{
			return result;
		}
		ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX_Data iMVS_MODULE_RESULT_INFO_LIST_EX_Data = ServerSDKManager.serverSDKManager.mModuleManager.PreProData(iMVS_MODULE_RESULT_INFO_LIST_EX.Value);
		for (int i = 0; i < iMVS_MODULE_RESULT_INFO_LIST_EX_Data.nResultNum; i++)
		{
			if (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[i].nParamType != 3)
			{
				continue;
			}
			string text = DeleteSpecifyChar(iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[i].strParamName, "%");
			if (strParam == text)
			{
				result.nValueNum = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[i].nValueNum;
				result.astStringVal = new StringData[result.nValueNum];
				for (int j = 0; j < result.nValueNum; j++)
				{
					result.astStringVal[j].strValue = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[i].pStringValue[j].strValue;
				}
				break;
			}
		}
		return result;
	}

	/// <summary>
	/// 获取二进制数据型输出(结果为空时strValue为null)
	/// </summary>
	/// <param name="strParam"></param>
	/// <returns></returns>
	public StringData GetOutputByteArray(string strParam)
	{
		if (string.IsNullOrEmpty(strParam))
		{
			VmException ex = new VmException(-536870911);
			throw ex;
		}
		if (!moduTool.GetOutputIOByName("%" + strParam + "%", out var _))
		{
			LogHelper.Error("Cannot Find IO Name " + strParam.ToString());
			throw new VmException(-536870881, "Cannot Find IO " + strParam.ToString() + " From Name");
		}
		StringData result = default;
		ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX? iMVS_MODULE_RESULT_INFO_LIST_EX = ServerSDKManager.serverSDKManager.mModuleManager.QueryModuleOneResult(m_nModuleId);
		if (iMVS_MODULE_RESULT_INFO_LIST_EX.HasValue)
		{
			ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX_Data iMVS_MODULE_RESULT_INFO_LIST_EX_Data = ServerSDKManager.serverSDKManager.mModuleManager.PreProData(iMVS_MODULE_RESULT_INFO_LIST_EX.Value);
			for (int i = 0; i < iMVS_MODULE_RESULT_INFO_LIST_EX_Data.nResultNum; i++)
			{
				if (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[i].nParamType != 6)
				{
					continue;
				}
				string text = DeleteSpecifyChar(iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[i].strParamName, "%");
				if (strParam == text)
				{
					if (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[i].nValueNum > 0)
					{
						result.strValue = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[i].pByteValue[0].strValue;
					}
					break;
				}
			}
		}
		return result;
	}

	/// <summary>
	/// 图像型结果(结果为空时nDataLen为0且pData和nReserved为null)
	/// </summary>
	/// <param name="strParam"></param>
	/// <returns></returns>
	public ByteArrayData GetOutputImage(string strParam)
	{
		if (string.IsNullOrEmpty(strParam))
		{
			VmException ex = new VmException(-536870911);
			throw ex;
		}
		if (!moduTool.GetOutputIOByName("%" + strParam + "%", out var _))
		{
			LogHelper.Error("Cannot Find IO Name " + strParam.ToString());
			throw new VmException(-536870881, "Cannot Find IO " + strParam.ToString() + " From Name");
		}
		ByteArrayData result = default;
		ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX? iMVS_MODULE_RESULT_INFO_LIST_EX = ServerSDKManager.serverSDKManager.mModuleManager.QueryModuleOneResult(m_nModuleId);
		if (!iMVS_MODULE_RESULT_INFO_LIST_EX.HasValue)
		{
			return result;
		}
		ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX_Data iMVS_MODULE_RESULT_INFO_LIST_EX_Data = ServerSDKManager.serverSDKManager.mModuleManager.PreProData(iMVS_MODULE_RESULT_INFO_LIST_EX.Value);
		for (int i = 0; i < iMVS_MODULE_RESULT_INFO_LIST_EX_Data.nResultNum; i++)
		{
			if (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[i].nParamType != 4)
			{
				continue;
			}
			string text = DeleteSpecifyChar(iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[i].strParamName, "%");
			if (strParam == text)
			{
				if (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[i].nValueNum > 0)
				{
					byte[] data = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[i].pImageValue[0].GetData(bUseMemPool: false);
					result.nDataLen = (uint)(data?.Length ?? 0);
					result.pData = data;
				}
				break;
			}
		}
		return result;
	}

	/// <summary>
	/// 点集型结果
	/// </summary>
	/// <param name="strParam"></param>
	/// <returns></returns>
	public MatchOutline GetOutputPointset(string strParam)
	{
		if (string.IsNullOrEmpty(strParam))
		{
			VmException ex = new VmException(-536870911);
			throw ex;
		}
		if (!moduTool.GetOutputIOByName("%" + strParam + "%", out var _))
		{
			LogHelper.Error("Cannot Find IO Name " + strParam.ToString());
			throw new VmException(-536870881, "Cannot Find IO " + strParam.ToString() + " From Name");
		}
		MatchOutline matchOutline = new MatchOutline();
		ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX? iMVS_MODULE_RESULT_INFO_LIST_EX = ServerSDKManager.serverSDKManager.mModuleManager.QueryModuleOneResult(m_nModuleId);
		if (!iMVS_MODULE_RESULT_INFO_LIST_EX.HasValue)
		{
			return matchOutline;
		}
		ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX_Data iMVS_MODULE_RESULT_INFO_LIST_EX_Data = ServerSDKManager.serverSDKManager.mModuleManager.PreProData(iMVS_MODULE_RESULT_INFO_LIST_EX.Value);
		for (int i = 0; i < iMVS_MODULE_RESULT_INFO_LIST_EX_Data.nResultNum; i++)
		{
			if (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[i].nParamType != 5)
			{
				continue;
			}
			string text = DeleteSpecifyChar(iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[i].strParamName, "%");
			if (!(strParam == text))
			{
				continue;
			}
			if (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[i].nValueNum <= 0)
			{
				break;
			}
			byte[] data = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[i].pChunkValue[0].GetData();
			if (data != null)
			{
				int num = data.Length / 16;
				IntPtr intPtr = Marshal.AllocHGlobal(16);
				int num2 = 0;
				for (int j = 0; j < data.Length; j += 16)
				{
					Marshal.Copy(data, j, intPtr, 16);
					matchOutline.MatchOutlinePoints.Add((MatchPoint)Marshal.PtrToStructure(intPtr, typeof(MatchPoint)));
					num2++;
				}
				Marshal.FreeHGlobal(intPtr);
			}
			break;
		}
		return matchOutline;
	}

	public ImageBaseData GetOutputImageV2(string strParam)
	{
		if (string.IsNullOrEmpty(strParam))
		{
			VmException ex = new VmException(-536870911);
			throw ex;
		}
		byte[] imageByteData = null;
		string sharedMemAddress = null;
		IVmImageData imageByteData2 = moduTool.GetImageByteData("%" + strParam + "%", ref imageByteData, ref sharedMemAddress);
		if (imageByteData2 == null)
		{
			return null;
		}
		ImageBaseData imageBaseData = null;
		if (ModuResultMemoryTypeEnum.Memory_Shallow_Copy == moduTool.ModuResultMemType)
		{
			imageBaseData = new ImageBaseData();
			imageBaseData.Width = imageByteData2.Width;
			imageBaseData.Height = imageByteData2.Height;
			imageBaseData.Pixelformat = imageByteData2.ImageFormat;
			if (sharedMemAddress != null)
			{
				ServerSDKManager.serverSDKManager.mModuleResultManager.InitImageObjBySharedMem(ref imageBaseData, sharedMemAddress, moduTool.ExecuteCount);
			}
		}
		else
		{
			uint dataLen = (uint)(imageByteData?.Length ?? 0);
			imageBaseData = new ImageBaseData(imageByteData, dataLen, imageByteData2.Width, imageByteData2.Height, imageByteData2.ImageFormat);
		}
		return imageBaseData;
	}

	public List<PointF> GetOutputPointArray(string strParam)
	{
		if (string.IsNullOrEmpty(strParam))
		{
			VmException ex = new VmException(-536870911);
			throw ex;
		}
		IVmPoint[] vmIOPointValue = moduTool.GetVmIOPointValue("%" + strParam + "%");
		if (vmIOPointValue == null)
		{
			return null;
		}
		PointF pointF = new PointF();
		List<PointF> list = new List<PointF>(vmIOPointValue.Length);
		IVmPoint[] array = vmIOPointValue;
		foreach (IVmPoint vmPoint in array)
		{
			if (vmPoint != null)
			{
				pointF = new PointF(vmPoint.X, vmPoint.Y);
				list.Add(pointF);
			}
		}
		return list;
	}

	/// <summary>
	/// 获取多边形结果
	/// </summary>
	/// <param name="strParam"></param>
	/// <returns></returns>
	public List<Polygon> GetOutputPolygonArray(string strParam)
	{
		if (string.IsNullOrEmpty(strParam))
		{
			VmException ex = new VmException(-536870911);
			throw ex;
		}
		IVmROIPolygon[] vmIOROIPolygonValue = moduTool.GetVmIOROIPolygonValue("%" + strParam + "%");
		if (vmIOROIPolygonValue == null)
		{
			return null;
		}
		Polygon polygon = new Polygon();
		PointF pointF = new PointF();
		List<Polygon> list = new List<Polygon>(vmIOROIPolygonValue.Length);
		IVmROIPolygon[] array = vmIOROIPolygonValue;
		foreach (IVmROIPolygon vmROIPolygon in array)
		{
			if (vmROIPolygon != null)
			{
				PointF[] array2 = new PointF[vmROIPolygon.Num];
				int num = 0;
				IVmPoint[] points = vmROIPolygon.Points;
				foreach (IVmPoint vmPoint in points)
				{
					pointF = new PointF(vmPoint.X, vmPoint.Y);
					array2[num] = pointF;
					num++;
				}
				polygon = new Polygon(array2);
				list.Add(polygon);
			}
		}
		return list;
	}

	/// <summary>
	/// 获取直线输出集合
	/// </summary>
	/// <param name="strParam"></param>
	/// <returns></returns>
	public List<Line> GetOutputLineArray(string strParam)
	{
		if (string.IsNullOrEmpty(strParam))
		{
			VmException ex = new VmException(-536870911);
			throw ex;
		}
		IVmLine[] vmIOLineValue = moduTool.GetVmIOLineValue("%" + strParam + "%");
		if (vmIOLineValue == null)
		{
			return null;
		}
		PointF pointF = new PointF();
		PointF pointF2 = new PointF();
		Line line = new Line();
		List<Line> list = new List<Line>(vmIOLineValue.Length);
		IVmLine[] array = vmIOLineValue;
		foreach (IVmLine vmLine in array)
		{
			if (vmLine != null)
			{
				pointF = new PointF(vmLine.StartPoint.X, vmLine.StartPoint.Y);
				pointF2 = new PointF(vmLine.EndPoint.X, vmLine.EndPoint.Y);
				double num = Math.Atan2(vmLine.EndPoint.Y - vmLine.StartPoint.Y, vmLine.EndPoint.X - vmLine.StartPoint.X) * 180.0 / Math.PI;
				line = new Line(pointF, pointF2, (float)num);
				list.Add(line);
			}
		}
		return list;
	}

	public List<Circle> GetOutputCircleArray(string strParam)
	{
		if (string.IsNullOrEmpty(strParam))
		{
			VmException ex = new VmException(-536870911);
			throw ex;
		}
		IVmCircle[] vmIOCircleValue = moduTool.GetVmIOCircleValue("%" + strParam + "%");
		if (vmIOCircleValue == null)
		{
			return null;
		}
		Circle circle = new Circle();
		PointF pointF = new PointF();
		List<Circle> list = new List<Circle>(vmIOCircleValue.Length);
		IVmCircle[] array = vmIOCircleValue;
		foreach (IVmCircle vmCircle in array)
		{
			if (vmCircle != null)
			{
				pointF = new PointF(vmCircle.CenterPoint.X, vmCircle.CenterPoint.Y);
				circle = new Circle(pointF, vmCircle.Radius);
				list.Add(circle);
			}
		}
		return list;
	}

	public List<Ellipse> GetOutputEllipseArray(string strParam)
	{
		if (string.IsNullOrEmpty(strParam))
		{
			VmException ex = new VmException(-536870911);
			throw ex;
		}
		IVmEllipse[] vmIOEllipseValue = moduTool.GetVmIOEllipseValue("%" + strParam + "%");
		if (vmIOEllipseValue == null)
		{
			return null;
		}
		Ellipse ellipse = new Ellipse();
		PointF pointF = new PointF();
		List<Ellipse> list = new List<Ellipse>(vmIOEllipseValue.Length);
		IVmEllipse[] array = vmIOEllipseValue;
		foreach (IVmEllipse vmEllipse in array)
		{
			if (vmEllipse != null)
			{
				pointF = new PointF(vmEllipse.CenterPoint.X, vmEllipse.CenterPoint.Y);
				ellipse = new Ellipse(pointF, vmEllipse.MajorRadius, vmEllipse.MinorRadius, vmEllipse.Angle);
				list.Add(ellipse);
			}
		}
		return list;
	}

	public List<RectBox> GetOutputRectBoxArray(string strParam)
	{
		if (string.IsNullOrEmpty(strParam))
		{
			VmException ex = new VmException(-536870911);
			throw ex;
		}
		IVmROIBox[] vmIORoiBoxValue = moduTool.GetVmIORoiBoxValue("%" + strParam + "%");
		if (vmIORoiBoxValue == null)
		{
			return null;
		}
		RectBox rectBox = new RectBox();
		PointF pointF = new PointF();
		List<RectBox> list = new List<RectBox>(vmIORoiBoxValue.Length);
		IVmROIBox[] array = vmIORoiBoxValue;
		foreach (IVmROIBox vmROIBox in array)
		{
			if (vmROIBox != null)
			{
				pointF = new PointF(vmROIBox.CenterPoint.X, vmROIBox.CenterPoint.Y);
				rectBox = new RectBox(pointF, vmROIBox.Width, vmROIBox.Height, vmROIBox.Angle);
				list.Add(rectBox);
			}
		}
		return list;
	}

	public List<RectF> GetOutputRectFArray(string strParam)
	{
		if (string.IsNullOrEmpty(strParam))
		{
			VmException ex = new VmException(-536870911);
			throw ex;
		}
		IVmRect[] vmIORectValue = moduTool.GetVmIORectValue("%" + strParam + "%");
		if (vmIORectValue == null)
		{
			return null;
		}
		RectF rectF = new RectF();
		PointF pointF = new PointF();
		List<RectF> list = new List<RectF>(vmIORectValue.Length);
		IVmRect[] array = vmIORectValue;
		foreach (IVmRect vmRect in array)
		{
			if (vmRect != null)
			{
				pointF = new PointF(vmRect.CenterPoint.X, vmRect.CenterPoint.Y);
				rectF = new RectF(pointF, vmRect.Width, vmRect.Height);
				list.Add(rectF);
			}
		}
		return list;
	}

	public List<Fixture> GetOutputFixtureArray(string strParam)
	{
		if (string.IsNullOrEmpty(strParam))
		{
			VmException ex = new VmException(-536870911);
			throw ex;
		}
		IVmFixture[] vmIOFixtureValue = moduTool.GetVmIOFixtureValue("%" + strParam + "%");
		if (vmIOFixtureValue == null)
		{
			return null;
		}
		PointF pointF = new PointF();
		PointF pointF2 = new PointF();
		Fixture fixture = new Fixture();
		List<Fixture> list = new List<Fixture>(vmIOFixtureValue.Length);
		IVmFixture[] array = vmIOFixtureValue;
		foreach (IVmFixture vmFixture in array)
		{
			if (vmFixture != null)
			{
				pointF = new PointF(vmFixture.FixtureInfo.Point.X, vmFixture.FixtureInfo.Point.Y);
				pointF2 = new PointF(vmFixture.UnFixtureInfo.Point.X, vmFixture.UnFixtureInfo.Point.Y);
				fixture = new Fixture(pointF, vmFixture.FixtureInfo.Angle, vmFixture.FixtureInfo.ScaleX, vmFixture.FixtureInfo.ScaleY, pointF2, vmFixture.UnFixtureInfo.Angle, vmFixture.UnFixtureInfo.ScaleX, vmFixture.UnFixtureInfo.ScaleY);
				list.Add(fixture);
			}
		}
		return list;
	}

	public List<Annulus> GetOutputAnnulusArray(string strParam)
	{
		if (string.IsNullOrEmpty(strParam))
		{
			VmException ex = new VmException(-536870911);
			throw ex;
		}
		IVmROIAnnulus[] vmIOROIAnnulusValue = moduTool.GetVmIOROIAnnulusValue("%" + strParam + "%");
		if (vmIOROIAnnulusValue == null)
		{
			return null;
		}
		Annulus annulus = new Annulus();
		PointF pointF = new PointF();
		List<Annulus> list = new List<Annulus>(vmIOROIAnnulusValue.Length);
		IVmROIAnnulus[] array = vmIOROIAnnulusValue;
		foreach (IVmROIAnnulus vmROIAnnulus in array)
		{
			if (vmROIAnnulus != null)
			{
				pointF = new PointF(vmROIAnnulus.CenterPoint.X, vmROIAnnulus.CenterPoint.Y);
				annulus = new Annulus(pointF, vmROIAnnulus.InnerRadius, vmROIAnnulus.OuterRadius, vmROIAnnulus.StartAngle, vmROIAnnulus.AngleExtend);
				list.Add(annulus);
			}
		}
		return list;
	}

	public List<ClassInfo> GetOutputClassInfoArray(string strParam)
	{
		if (string.IsNullOrEmpty(strParam))
		{
			VmException ex = new VmException(-536870911);
			throw ex;
		}
		IVmClassInfo[] vmIOClassInfoValue = moduTool.GetVmIOClassInfoValue("%" + strParam + "%");
		if (vmIOClassInfoValue == null)
		{
			return null;
		}
		ClassInfo classInfo = new ClassInfo();
		List<ClassInfo> list = new List<ClassInfo>(vmIOClassInfoValue.Length);
		IVmClassInfo[] array = vmIOClassInfoValue;
		foreach (IVmClassInfo vmClassInfo in array)
		{
			if (vmClassInfo != null)
			{
				classInfo = new ClassInfo
				{
					Name = vmClassInfo.ClassName,
					GrayValue = vmClassInfo.GrayValue
				};
				list.Add(classInfo);
			}
		}
		return list;
	}

	public PixelImage GetOutputPixelImage(string strParam)
	{
		if (string.IsNullOrEmpty(strParam))
		{
			VmException ex = new VmException(-536870911);
			throw ex;
		}
		byte[] imageByteData = null;
		string sharedMemAddress = null;
		IVmPixelImageData pixelImageByteData = moduTool.GetPixelImageByteData("%" + strParam + "%", ref imageByteData, ref sharedMemAddress);
		if (pixelImageByteData == null)
		{
			return null;
		}
		ImageBaseData imageBaseData = null;
		if (ModuResultMemoryTypeEnum.Memory_Shallow_Copy == moduTool.ModuResultMemType)
		{
			imageBaseData = new ImageBaseData();
			imageBaseData.Width = pixelImageByteData.ImageData.Width;
			imageBaseData.Height = pixelImageByteData.ImageData.Height;
			imageBaseData.Pixelformat = pixelImageByteData.ImageData.ImageFormat;
			if (sharedMemAddress != null)
			{
				ServerSDKManager.serverSDKManager.mModuleResultManager.InitImageObjBySharedMem(ref imageBaseData, sharedMemAddress, moduTool.ExecuteCount);
			}
		}
		else
		{
			uint dataLen = (uint)(imageByteData?.Length ?? 0);
			imageBaseData = new ImageBaseData(imageByteData, dataLen, pixelImageByteData.ImageData.Width, pixelImageByteData.ImageData.Height, pixelImageByteData.ImageData.ImageFormat);
		}
		List<ClassInfo> list = new List<ClassInfo>(pixelImageByteData.ClassInfo.Length);
		IVmClassInfo[] classInfo = pixelImageByteData.ClassInfo;
		foreach (IVmClassInfo vmClassInfo in classInfo)
		{
			if (vmClassInfo != null)
			{
				ClassInfo item = new ClassInfo
				{
					Name = vmClassInfo.ClassName,
					GrayValue = vmClassInfo.GrayValue
				};
				list.Add(item);
			}
		}
		return new PixelImage
		{
			Image = imageBaseData,
			Info = list
		};
	}

	public List<Posture> GetOutputIoPostureArray(string strParam)
	{
		if (string.IsNullOrEmpty(strParam))
		{
			VmException ex = new VmException(-536870911);
			throw ex;
		}
		IVmPosture[] vmIOPostureValue = moduTool.GetVmIOPostureValue("%" + strParam + "%");
		if (vmIOPostureValue == null)
		{
			return null;
		}
		Posture posture = new Posture();
		PointF pointF = new PointF();
		List<Posture> list = new List<Posture>(vmIOPostureValue.Length);
		IVmPosture[] array = vmIOPostureValue;
		foreach (IVmPosture vmPosture in array)
		{
			if (vmPosture != null)
			{
				pointF = new PointF(vmPosture.Point.X, vmPosture.Point.Y);
				posture = new Posture(pointF, vmPosture.R);
				list.Add(posture);
			}
		}
		return list;
	}

	/// <summary>
	/// 获取所有输出名称信息（CONTOURPOINTS、POINTSET、小写的image,这三种类型不支持）
	/// </summary>
	public List<VmDynamicIODefine.IoNameInfo> GetAllOutputNameInfo()
	{
		List<VmDynamicIODefine.IoNameInfo> list = new List<VmDynamicIODefine.IoNameInfo>();
		foreach (DynamicVmIO item in moduTool.Outputs.OfType<DynamicVmIO>())
		{
			if (item.Visible)
			{
				IMVS_MODULE_BASE_DATA_TYPE iMVS_MODULE_BASE_DATA_TYPE = ConvertTypeName(item.TypeName);
				if (iMVS_MODULE_BASE_DATA_TYPE != IMVS_MODULE_BASE_DATA_TYPE.IMVS_BASE_TYPE_UNDEFINED)
				{
					list.Add(new VmDynamicIODefine.IoNameInfo
					{
						Name = Regex.Replace(item.Name, "%(.+)%", "$1"),
						TypeName = iMVS_MODULE_BASE_DATA_TYPE
					});
				}
			}
		}
		return list;
	}

	private IMVS_MODULE_BASE_DATA_TYPE ConvertTypeName(string typeName)
	{
		switch (typeName)
		{
		case "int":
			return IMVS_MODULE_BASE_DATA_TYPE.IMVS_GRAP_TYPE_INT;
		case "float":
			return IMVS_MODULE_BASE_DATA_TYPE.IMVS_GRAP_TYPE_FLOAT;
		case "string":
			return IMVS_MODULE_BASE_DATA_TYPE.IMVS_GRAP_TYPE_STRING;
		case "byte":
			return IMVS_MODULE_BASE_DATA_TYPE.IMVS_GRAP_TYPE_BYTE;
		case "IMAGE":
			return IMVS_MODULE_BASE_DATA_TYPE.IMVS_BASE_TYPE_IMAGE_DATA;
		case "PIXELIMAGE":
			return IMVS_MODULE_BASE_DATA_TYPE.IMVS_GRAP_TYPE_PIXELIMAGE;
		case "BOX":
		case "ROIBOX":
			return IMVS_MODULE_BASE_DATA_TYPE.IMVS_GRAP_TYPE_RECT_BOX;
		case "POINT":
			return IMVS_MODULE_BASE_DATA_TYPE.IMVS_GRAP_TYPE_POINT_F;
		case "LINE":
			return IMVS_MODULE_BASE_DATA_TYPE.IMVS_GRAP_TYPE_LINE;
		case "CIRCLE":
			return IMVS_MODULE_BASE_DATA_TYPE.IMVS_GRAP_TYPE_CIRCLE;
		case "FIXTURE":
			return IMVS_MODULE_BASE_DATA_TYPE.IMVS_GRAP_TYPE_FIXTURE;
		case "ANNULUS":
		case "ROIANNULUS":
			return IMVS_MODULE_BASE_DATA_TYPE.IMVS_GRAP_TYPE_ANNULUS;
		case "RECT":
		case "Rect":
			return IMVS_MODULE_BASE_DATA_TYPE.IMVS_GRAP_TYPE_RECT_F;
		case "CLASSINFO":
			return IMVS_MODULE_BASE_DATA_TYPE.IMVS_GRAP_TYPE_CLASSINFO;
		case "POLYGON":
		case "ROIPOLYGON":
			return IMVS_MODULE_BASE_DATA_TYPE.IMVS_GRAP_TYPE_POLYGON;
		case "POSTURE":
			return IMVS_MODULE_BASE_DATA_TYPE.IMVS_GRAP_TYPE_POSTURE;
		case "ELLIPSE":
			return IMVS_MODULE_BASE_DATA_TYPE.IMVS_GRAP_TYPE_ELLIPSE;
		case "double":
			return IMVS_MODULE_BASE_DATA_TYPE.IMVS_GRAP_TYPE_DOUBLE;
		default:
			return IMVS_MODULE_BASE_DATA_TYPE.IMVS_BASE_TYPE_UNDEFINED;
		}
	}

	/// <summary>
	/// 剔除指定字符
	/// </summary>
	private string DeleteSpecifyChar(string strInput, string strFlag)
	{
		string text = strInput;
		if (strInput.Length > 0)
		{
			string text2 = strInput.Substring(0, 1);
			if (strFlag == text2)
			{
				text = strInput.Substring(1, strInput.Length - 1);
			}
			if (text.Length > 0)
			{
				string text3 = text.Substring(text.Length - 1, 1);
				if (strFlag == text3)
				{
					text = text.Remove(text.Length - 1, 1);
				}
			}
		}
		return text;
	}

	public RangeImageBaseData_V2 GetOutputRangeImage(string strParam)
	{
		IVmRangeImageData vmIORangeImageValue = moduTool.GetVmIORangeImageValue("%" + strParam + "%");
		if (vmIORangeImageValue == null)
		{
			return null;
		}
		VMPixelFormat value = (VMPixelFormat)(vmIORangeImageValue.IntensityImageData?.ImageFormat).Value;
		ImageBaseData_V2 intensityImageData = new ImageBaseData_V2(vmIORangeImageValue.IntensityImageData.Data, (uint)vmIORangeImageValue.IntensityImageData.DataLenth, vmIORangeImageValue.IntensityImageData.Width, vmIORangeImageValue.IntensityImageData.Height, value);
		VMPixelFormat rangeImageFormat = (VMPixelFormat)vmIORangeImageValue.RangeImageFormat;
		return new RangeImageBaseData_V2(vmIORangeImageValue.RangeData, (uint)vmIORangeImageValue.RangeDataLenth, intensityImageData, vmIORangeImageValue.RangeWidth, vmIORangeImageValue.RangeHeight, rangeImageFormat, vmIORangeImageValue.Xoffset, vmIORangeImageValue.Yoffset, vmIORangeImageValue.Zoffset, vmIORangeImageValue.Xscale, vmIORangeImageValue.Yscale, vmIORangeImageValue.Zscale);
	}

	public ImageBaseData3D_V2 GetOutputImageBaseData3D(string strParam)
	{
		IVm3DImageData vmIO3DImageValue = moduTool.GetVmIO3DImageValue("%" + strParam + "%");
		if (vmIO3DImageValue == null)
		{
			return null;
		}
		VMPixelFormat value = (VMPixelFormat)(vmIO3DImageValue.RangeImageData.IntensityImageData?.ImageFormat).Value;
		ImageBaseData_V2 intensityImageData = new ImageBaseData_V2(vmIO3DImageValue.RangeImageData.IntensityImageData.Data, (uint)vmIO3DImageValue.RangeImageData.IntensityImageData.DataLenth, vmIO3DImageValue.RangeImageData.IntensityImageData.Width, vmIO3DImageValue.RangeImageData.IntensityImageData.Height, value);
		VMPixelFormat rangeImageFormat = (VMPixelFormat)vmIO3DImageValue.RangeImageData.RangeImageFormat;
		RangeImageBaseData_V2 rangeImageData = new RangeImageBaseData_V2(vmIO3DImageValue.RangeImageData.RangeData, (uint)vmIO3DImageValue.RangeImageData.RangeDataLenth, intensityImageData, vmIO3DImageValue.RangeImageData.RangeWidth, vmIO3DImageValue.RangeImageData.RangeHeight, rangeImageFormat, vmIO3DImageValue.RangeImageData.Xoffset, vmIO3DImageValue.RangeImageData.Yoffset, vmIO3DImageValue.RangeImageData.Zoffset, vmIO3DImageValue.RangeImageData.Xscale, vmIO3DImageValue.RangeImageData.Yscale, vmIO3DImageValue.RangeImageData.Zscale);
		return new ImageBaseData3D_V2(vmIO3DImageValue.ImageMode, rangeImageData);
	}
}
