using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using HikExternalCall.Common;

namespace VM.PlatformSDKCS;

/// <summary>
/// ModuleResultManager3D
/// </summary>
public class ModuleResultManager
{
	public void SetModuleResult(ref ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX stModuleResult, ref CModuleResultBase stModuBase, ModuResultMemoryTypeEnum nMemType = ModuResultMemoryTypeEnum.Memory_Deep_Copy)
	{
		stModuBase.ErrorCode = stModuleResult.nErrorCode;
		stModuBase.ExecuteCount = stModuleResult.nExecuteCount;
		stModuBase.ModuRunTime = stModuleResult.fModuleTime;
		int nIndexOffset = stModuleResult.nResultNum - 1;
		uint[] anResIndex = new uint[stModuleResult.nResultNum];
		string[] astrResName = new string[stModuleResult.nResultNum];
		ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_EX[] astModuRes = ExternalCallHelper.PtrToStructurs<ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_EX>(stModuleResult.pInfo, stModuleResult.nResultNum);
		for (uint num = 0u; num < stModuleResult.nResultNum; num++)
		{
			anResIndex[num] = num;
			astrResName[num] = StringUtil.UTF8GetString(astModuRes[num].strParamName);
		}
		bool bIsShallowCopy = ModuResultMemoryTypeEnum.Memory_Shallow_Copy == nMemType;
		PropertyInfo[] properties = stModuBase.GetType().GetProperties();
		PropertyInfo[] array = properties;
		foreach (PropertyInfo propertyInfo in array)
		{
			object value = propertyInfo.GetValue(stModuBase, null);
			if (value is IBaseData)
			{
				switch (((IBaseData)value).BaseDataType)
				{
				case IMVS_MODULE_BASE_DATA_TYPE.IMVS_BASE_TYPE_IMAGE_DATA:
					SetImageResultData(propertyInfo, ref nIndexOffset, ref anResIndex, ref astrResName, ref astModuRes, ref stModuBase, bIsShallowCopy, stModuleResult.nExecuteCount);
					break;
				case IMVS_MODULE_BASE_DATA_TYPE.IMVS_GRAP_TYPE_POINT_F:
					SetPointFResultData(propertyInfo, ref nIndexOffset, ref anResIndex, ref astrResName, ref astModuRes, ref stModuBase);
					break;
				case IMVS_MODULE_BASE_DATA_TYPE.IMVS_GRAP_TYPE_RECT_I:
					SetRectIResultData(propertyInfo, ref nIndexOffset, ref anResIndex, ref astrResName, ref astModuRes, ref stModuBase);
					break;
				case IMVS_MODULE_BASE_DATA_TYPE.IMVS_GRAP_TYPE_RECT_BOX:
					SetRectBoxResultData(propertyInfo, ref nIndexOffset, ref anResIndex, ref astrResName, ref astModuRes, ref stModuBase);
					break;
				case IMVS_MODULE_BASE_DATA_TYPE.IMVS_GRAP_TYPE_LINE:
					SetLineResultData(propertyInfo, ref nIndexOffset, ref anResIndex, ref astrResName, ref astModuRes, ref stModuBase);
					break;
				case IMVS_MODULE_BASE_DATA_TYPE.IMVS_GRAP_TYPE_CIRCLE:
					SetCircleResultData(propertyInfo, ref nIndexOffset, ref anResIndex, ref astrResName, ref astModuRes, ref stModuBase);
					break;
				case IMVS_MODULE_BASE_DATA_TYPE.IMVS_GRAP_TYPE_ANNULUS:
					SetAnnulusResultData(propertyInfo, ref nIndexOffset, ref anResIndex, ref astrResName, ref astModuRes, ref stModuBase);
					break;
				case IMVS_MODULE_BASE_DATA_TYPE.IMVS_GRAP_TYPE_DEFECT_FIXTURE:
					SetDefectFixtureResultData(propertyInfo, ref nIndexOffset, ref anResIndex, ref astrResName, ref astModuRes, ref stModuBase);
					break;
				case IMVS_MODULE_BASE_DATA_TYPE.IMVS_GRAP_TYPE_MATCHOUTLINE:
					SetOutLineResultData(propertyInfo, ref nIndexOffset, ref anResIndex, ref astrResName, ref astModuRes, ref stModuBase, bIsShallowCopy, stModuleResult.nExecuteCount);
					break;
				case IMVS_MODULE_BASE_DATA_TYPE.IMVS_GRAP_TYPE_ELLIPSE:
					SetEllipseResultData(propertyInfo, ref nIndexOffset, ref anResIndex, ref astrResName, ref astModuRes, ref stModuBase);
					break;
				case IMVS_MODULE_BASE_DATA_TYPE.IMVS_BASE_TYPE_IMAGE3D_DATA:
					SetImageResultData3D(propertyInfo, ref stModuleResult, ref stModuBase);
					break;
				}
			}
			else if (value is List<Annulus>)
			{
				SetAnnulusResultData(propertyInfo, ref nIndexOffset, ref anResIndex, ref astrResName, ref astModuRes, ref stModuBase, bMultiRes: true);
			}
			else if (value is List<Circle>)
			{
				SetCircleResultData(propertyInfo, ref nIndexOffset, ref anResIndex, ref astrResName, ref astModuRes, ref stModuBase, bMultiRes: true);
			}
			else if (value is List<DefectFixture>)
			{
				SetDefectFixtureResultData(propertyInfo, ref nIndexOffset, ref anResIndex, ref astrResName, ref astModuRes, ref stModuBase, bMultiRes: true);
			}
			else if (value is List<Line>)
			{
				SetLineResultData(propertyInfo, ref nIndexOffset, ref anResIndex, ref astrResName, ref astModuRes, ref stModuBase, bMultiRes: true);
			}
			else if (value is List<PointF>)
			{
				SetPointFResultData(propertyInfo, ref nIndexOffset, ref anResIndex, ref astrResName, ref astModuRes, ref stModuBase, bMultiRes: true);
			}
			else if (value is List<RectBox>)
			{
				SetRectBoxResultData(propertyInfo, ref nIndexOffset, ref anResIndex, ref astrResName, ref astModuRes, ref stModuBase, bMultiRes: true);
			}
			else if (value is List<RectI>)
			{
				SetRectIResultData(propertyInfo, ref nIndexOffset, ref anResIndex, ref astrResName, ref astModuRes, ref stModuBase, bMultiRes: true);
			}
			else if (value is List<Ellipse>)
			{
				SetEllipseResultData(propertyInfo, ref nIndexOffset, ref anResIndex, ref astrResName, ref astModuRes, ref stModuBase, bMultiRes: true);
			}
			else if (value is int)
			{
				SetIntResultData(propertyInfo, ref nIndexOffset, ref anResIndex, ref astrResName, ref astModuRes, ref stModuBase);
			}
			else if (value is float)
			{
				SetFloatResultData(propertyInfo, ref nIndexOffset, ref anResIndex, ref astrResName, ref astModuRes, ref stModuBase);
			}
			else if (value is string)
			{
				SetStringResultData(propertyInfo, ref nIndexOffset, ref anResIndex, ref astrResName, ref astModuRes, ref stModuBase);
			}
			else if (value is List<int>)
			{
				SetIntResultData(propertyInfo, ref nIndexOffset, ref anResIndex, ref astrResName, ref astModuRes, ref stModuBase, bMultiRes: true);
			}
			else if (value is List<float>)
			{
				SetFloatResultData(propertyInfo, ref nIndexOffset, ref anResIndex, ref astrResName, ref astModuRes, ref stModuBase, bMultiRes: true);
			}
			else if (value is List<string>)
			{
				SetStringResultData(propertyInfo, ref nIndexOffset, ref anResIndex, ref astrResName, ref astModuRes, ref stModuBase, bMultiRes: true);
			}
		}
	}

	private int GetIndexByName(int nResType, string strResName, ref int nIndexOffset, ref uint[] anResIndex, ref string[] astrResName, ref ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_EX[] astModuRes)
	{
		uint num = 0u;
		for (int i = 0; i < anResIndex.Length; i++)
		{
			num = anResIndex[i];
			if (nResType == astModuRes[num].nParamType && astModuRes[num].nValueNum > 0 && strResName == astrResName[num])
			{
				if (i <= nIndexOffset)
				{
					anResIndex[i] = anResIndex[nIndexOffset];
					anResIndex[nIndexOffset] = num;
					nIndexOffset--;
				}
				return (int)num;
			}
		}
		return -1;
	}

	private unsafe void SetIntResultData(PropertyInfo propInfo, ref int nIndexOffset, ref uint[] anResIndex, ref string[] astrResName, ref ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_EX[] astModuRes, ref CModuleResultBase stModuBase, bool bMultiRes = false)
	{
		uint num = 0u;
		for (int i = 0; i < anResIndex.Length; i++)
		{
			num = anResIndex[i];
			if (1 == astModuRes[num].nParamType && astModuRes[num].nValueNum > 0 && astrResName[num] == propInfo.Name)
			{
				if (bMultiRes)
				{
					int[] array = new int[astModuRes[num].nValueNum];
					Marshal.Copy(astModuRes[num].pIntValue, array, 0, astModuRes[num].nValueNum);
					List<int> value = new List<int>(array);
					propInfo.SetValue(stModuBase, value, null);
				}
				else
				{
					int* ptr = (int*)astModuRes[num].pIntValue.ToPointer();
					propInfo.SetValue(stModuBase, *ptr, null);
				}
				if (i <= nIndexOffset)
				{
					anResIndex[i] = anResIndex[nIndexOffset];
					anResIndex[nIndexOffset] = num;
					nIndexOffset--;
				}
				break;
			}
		}
	}

	private unsafe void SetFloatResultData(PropertyInfo propInfo, ref int nIndexOffset, ref uint[] anResIndex, ref string[] astrResName, ref ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_EX[] astModuRes, ref CModuleResultBase stModuBase, bool bMultiRes = false)
	{
		uint num = 0u;
		for (int i = 0; i < anResIndex.Length; i++)
		{
			num = anResIndex[i];
			if (2 == astModuRes[num].nParamType && astModuRes[num].nValueNum > 0 && astrResName[num] == propInfo.Name)
			{
				if (bMultiRes)
				{
					float[] array = new float[astModuRes[num].nValueNum];
					Marshal.Copy(astModuRes[num].pFloatValue, array, 0, astModuRes[num].nValueNum);
					List<float> value = new List<float>(array);
					propInfo.SetValue(stModuBase, value, null);
				}
				else
				{
					float* ptr = (float*)astModuRes[num].pFloatValue.ToPointer();
					propInfo.SetValue(stModuBase, *ptr, null);
				}
				if (i <= nIndexOffset)
				{
					anResIndex[i] = anResIndex[nIndexOffset];
					anResIndex[nIndexOffset] = num;
					nIndexOffset--;
				}
				break;
			}
		}
	}

	private void SetStringResultData(PropertyInfo propInfo, ref int nIndexOffset, ref uint[] anResIndex, ref string[] astrResName, ref ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_EX[] astModuRes, ref CModuleResultBase stModuBase, bool bMultiRes = false)
	{
		uint num = 0u;
		for (int i = 0; i < anResIndex.Length; i++)
		{
			num = anResIndex[i];
			if (3 == astModuRes[num].nParamType && astModuRes[num].nValueNum > 0 && astrResName[num] == propInfo.Name)
			{
				ImvsSdkDefine.IMVS_MODULE_STRING_VALUE[] array = ExternalCallHelper.PtrToStructurs<ImvsSdkDefine.IMVS_MODULE_STRING_VALUE>(astModuRes[num].pStringValue, astModuRes[num].nValueNum);
				List<string> list = new List<string>(array.Length);
				for (int j = 0; j < array.Length; j++)
				{
					list.Add(StringUtil.UTF8GetString(array[j].strValue));
				}
				if (bMultiRes)
				{
					propInfo.SetValue(stModuBase, list, null);
				}
				else
				{
					propInfo.SetValue(stModuBase, list[0], null);
				}
				if (i <= nIndexOffset)
				{
					anResIndex[i] = anResIndex[nIndexOffset];
					anResIndex[nIndexOffset] = num;
					nIndexOffset--;
				}
				break;
			}
		}
	}

	public void InitImageObjBySharedMem(ref ImageBaseData imgObj, string strMemAddress, uint nExeCount)
	{
		imgObj.SharedMemObj = new SharedMemoryItemVM(strMemAddress, nExeCount);
	}

	private unsafe void SetImageResultData(PropertyInfo propInfo, ref int nIndexOffset, ref uint[] anResIndex, ref string[] astrResName, ref ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_EX[] astModuRes, ref CModuleResultBase stModuBase, bool bIsShallowCopy, uint nExecuteCount)
	{
		string text = "";
		string text2 = "";
		string text3 = "";
		string text4 = "";
		switch (propInfo.Name)
		{
		case "ImageData":
			text = "Image";
			text2 = "Width";
			text3 = "Height";
			text4 = "PixelFormat";
			break;
		case "ImageDataMono8":
			text = "ImageMono8";
			text2 = "WidthMono8";
			text3 = "HeightMono8";
			text4 = "PixelFormatMono8";
			break;
		case "OutputImage":
		case "OutImageComb":
			text = "OutImage";
			text2 = "OutImageWidth";
			text3 = "OutImageHeight";
			text4 = "OutImagePixelFormat";
			break;
		case "OutputMask":
			text = "OutMask";
			text2 = "OutMaskWidth";
			text3 = "OutMaskHeight";
			text4 = "OutMaskPixelFormat";
			break;
		case "BinaryImageComb":
		case "BlobBinImage":
			text = "BinaryImage";
			text2 = "BinaryImageWidth";
			text3 = "BinaryImageHeight";
			text4 = "BinaryImagePixelFormat";
			break;
		case "BlobImageComb":
		case "BlobBlockImage":
			text = "BlobImage";
			text2 = "BlobImageWidth";
			text3 = "BlobImageHeight";
			text4 = "BlobImagePixelFormat";
			break;
		case "PImage":
			text = "ProcImage";
			text2 = "ProcImageWidth";
			text3 = "ProcImageHeight";
			text4 = "ProcImagePixelFormat";
			break;
		case "StitchImageComb":
			text = "StitchImage";
			text2 = "StitchImageWidth";
			text3 = "StitchImageHeight";
			text4 = "StitchImagePixelFormat";
			break;
		case "AlbedoImageCom":
			text = "AlbedoImage";
			text2 = "AlbedoImageWidth";
			text3 = "AlbedoImageHeight";
			text4 = "AlbedoImagePixelFormat";
			break;
		case "OutResidualImage":
			text = "ResidualImage";
			text2 = "ResidualImageWidth";
			text3 = "ResidualImageHeight";
			text4 = "ResidualImagePixelFormat";
			break;
		case "MarkFlawImage":
			text = "FlawImage";
			text2 = "FlawImageWidth";
			text3 = "FlawImageHeight";
			text4 = "FlawImagePixelFormat";
			break;
		case "ScoreImageData":
			text = "ScoreImage";
			text2 = "ScoreImageWidth";
			text3 = "ScoreImageHeight";
			text4 = "ScoreImagePixelFormat";
			break;
		case "ClassImageData":
			text = "ClassImage";
			text2 = "ClassImageWidth";
			text3 = "ClassImageHeight";
			text4 = "ClassImagePixelFormat";
			break;
		case "OutMaskImage":
			text = "MaskImage";
			text2 = "MaskImageWidth";
			text3 = "MaskImageHeight";
			text4 = "MaskImagePixelFormat";
			break;
		case "ProbabilityImageData":
			text = "ProbabilityImage";
			text2 = "ProbabilityImageWidth";
			text3 = "ProbabilityImageHeight";
			text4 = "ProbabilityImagePixelFormat";
			break;
		case "OutRenderImage":
			text = "RenderImage";
			text2 = "RenderImageWidth";
			text3 = "RenderImageHeight";
			text4 = "RenderImagePixelFormat";
			break;
		default:
		{
			string text5 = "";
			if (propInfo.Name.Length > 3)
			{
				text5 = propInfo.Name.Substring(propInfo.Name.Length - 3, 3);
			}
			if (text5 == "Com")
			{
				text5 = propInfo.Name.Substring(0, propInfo.Name.Length - 3);
				text = text5;
				text2 = text5 + "Width";
				text3 = text5 + "Height";
				text4 = text5 + "PixelFormat";
			}
			else if (HasNumber(text5) && propInfo.Name.Contains("OutImage"))
			{
				text = propInfo.Name + "Image";
				text2 = propInfo.Name + "Width";
				text3 = propInfo.Name + "Height";
				text4 = propInfo.Name + "PixelFormat";
			}
			else if (HasNumber(propInfo.Name) && propInfo.Name.Contains("ScoreImage"))
			{
				if (propInfo.Name.Length > 4)
				{
					text5 = propInfo.Name.Substring(0, propInfo.Name.Length - 4);
				}
				text = text5;
				text2 = text5 + "Width";
				text3 = text5 + "Height";
				text4 = text5 + "PixelFormat";
			}
			else
			{
				text = propInfo.Name + "Data";
				text2 = propInfo.Name + "Width";
				text3 = propInfo.Name + "Height";
				text4 = propInfo.Name + "PixelFormat";
			}
			break;
		}
		}
		ImageBaseData_Base imageBaseData_Base = new ImageBaseData();
		bool flag = true;
		bool flag2 = true;
		bool flag3 = true;
		bool flag4 = true;
		bool flag5 = false;
		uint num = 0u;
		uint num2 = 0u;
		int num3 = 0;
		while (num3 < anResIndex.Length)
		{
			flag5 = false;
			num2 = anResIndex[num3];
			if (1 == astModuRes[num2].nParamType && astModuRes[num2].nValueNum > 0)
			{
				if (flag && astrResName[num2] == text2)
				{
					int* ptr = (int*)astModuRes[num2].pIntValue.ToPointer();
					imageBaseData_Base.Width = *ptr;
					flag = false;
					flag5 = true;
					num++;
				}
				else if (flag2 && astrResName[num2] == text3)
				{
					int* ptr2 = (int*)astModuRes[num2].pIntValue.ToPointer();
					imageBaseData_Base.Height = *ptr2;
					flag2 = false;
					flag5 = true;
					num++;
				}
				else if (flag3 && astrResName[num2] == text4)
				{
					int* ptr3 = (int*)astModuRes[num2].pIntValue.ToPointer();
					(imageBaseData_Base as ImageBaseData).Pixelformat = *ptr3;
					flag3 = false;
					flag5 = true;
					num++;
				}
			}
			else if (4 == astModuRes[num2].nParamType && astModuRes[num2].nValueNum > 0 && flag4 && astrResName[num2] == text)
			{
				ImvsSdkDefine.IMVS_MODULE_CHUNK_VALUE_EX[] array = ExternalCallHelper.PtrToStructurs<ImvsSdkDefine.IMVS_MODULE_CHUNK_VALUE_EX>(astModuRes[num2].pImageValue, astModuRes[num2].nValueNum);
				if (bIsShallowCopy)
				{
					if (array[0].nUseRealData == 0 || 2 == array[0].nUseRealData)
					{
						(imageBaseData_Base as ImageBaseData).SharedMemObj = new SharedMemoryItemVM(array[0].strValue, nExecuteCount);
					}
				}
				else
				{
					bool isExcOccur = false;
					ImvsSdkDefine.IMVS_MODULE_CHUNK_VALUE_EX_Data iMVS_MODULE_CHUNK_VALUE_EX_Data = new ImvsSdkDefine.IMVS_MODULE_CHUNK_VALUE_EX_Data();
					ServerSDKManager.serverSDKManager.mModuleManager.LargeObject(ref array[0], iMVS_MODULE_CHUNK_VALUE_EX_Data, ref isExcOccur);
					imageBaseData_Base.DataLen = (uint)(((imageBaseData_Base as ImageBaseData).ImageData = iMVS_MODULE_CHUNK_VALUE_EX_Data.GetData(bUseMemPool: false))?.Length ?? 0);
				}
				flag4 = false;
				flag5 = true;
				num++;
			}
			if (flag5)
			{
				if (num3 < nIndexOffset)
				{
					anResIndex[num3] = anResIndex[nIndexOffset];
					anResIndex[nIndexOffset] = num2;
					nIndexOffset--;
				}
				else if (num3 == nIndexOffset)
				{
					nIndexOffset--;
					num3++;
				}
				if (num >= 4)
				{
					break;
				}
			}
			else
			{
				num3++;
			}
		}
		propInfo.SetValue(stModuBase, imageBaseData_Base, null);
	}

	private unsafe void SetAnnulusResultData(PropertyInfo propInfo, ref int nIndexOffset, ref uint[] anResIndex, ref string[] astrResName, ref ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_EX[] astModuRes, ref CModuleResultBase stModuBase, bool bMultiRes = false)
	{
		string text = "DetectAnnulusCenterX";
		string text2 = "DetectAnnulusCenterY";
		string text3 = "DetectAnnulusInnerRadius";
		string text4 = "DetectAnnulusOuterRadius";
		string text5 = "DetectAnnulusStartAngle";
		string text6 = "DetectAnnulusAngleExtend";
		switch (propInfo.Name)
		{
		case "OutputAnnulus":
			text = "OutputAnnulusCenterX";
			text2 = "OutputAnnulusCenterY";
			text3 = "OutputAnnulusInnerRadius";
			text4 = "OutputAnnulusOuterRadius";
			text5 = "OutputAnnulusStartAngle";
			text6 = "OutputAnnulusAngleExtend";
			break;
		case "ListOutputResultCircle":
			text = "ListCircleCenterX";
			text2 = "ListCircleCenterY";
			text3 = "ListCircleInnerRadius";
			text4 = "ListRadius";
			text5 = "ListCircleStartAngle";
			text6 = "ListCircleAngleExtend";
			break;
		case "OutputCircleAnnulus":
			text = "CircleCenterX";
			text2 = "CircleCenterY";
			text3 = "CircleInnerRadius";
			text4 = "CircleRadius";
			text5 = "CircleStartAngle";
			text6 = "CircleAngleExtend";
			break;
		case "ROIAnnuluss":
			text = "MultiDetectAnnulusCenterX";
			text2 = "MultiDetectAnnulusCenterY";
			text3 = "MultiDetectAnnulusInnerRadius";
			text4 = "MultiDetectAnnulusOuterRadius";
			text5 = "MultiDetectAnnulusStartAngle";
			text6 = "MultiDetectAnnulusAngleExtend";
			break;
		case "MeasureCircleAnnulus1":
			text = "CircleAnnulus1CenterX";
			text2 = "CircleAnnulus1CenterY";
			text3 = "CircleAnnulus1InnerRadius";
			text4 = "CircleAnnulus1Radius";
			text5 = "CircleAnnulus1StartAngle";
			text6 = "CircleAnnulus1AngleExtend";
			break;
		case "MeasureCircleAnnulus2":
			text = "CircleAnnulus2CenterX";
			text2 = "CircleAnnulus2CenterY";
			text3 = "CircleAnnulus2InnerRadius";
			text4 = "CircleAnnulus2Radius";
			text5 = "CircleAnnulus2StartAngle";
			text6 = "CircleAnnulus2AngleExtend";
			break;
		case "MeasureCircleAnnulus":
			text = "InputCircleAnnulusCenterPointX";
			text2 = "InputCircleAnnulusCenterPointY";
			text3 = "InputCircleAnnulusInnerRadius";
			text4 = "InputCircleAnnulusRadius";
			text5 = "InputCircleAnnulusStartAngle";
			text6 = "InputCircleAnnulusAngleExtend";
			break;
		case "StandardAnnulus":
			text = "StandardAnnulusCenterX";
			text2 = "StandardAnnulusCenterY";
			text3 = "StandardAnnulusInnerRadius";
			text4 = "StandardAnnulusOuterRadius";
			text5 = "StandardAnnulusStartAngle";
			text6 = "StandardAnnulusAngleExtend";
			break;
		}
		IntPtr intPtr = IntPtr.Zero;
		IntPtr intPtr2 = IntPtr.Zero;
		IntPtr intPtr3 = IntPtr.Zero;
		IntPtr intPtr4 = IntPtr.Zero;
		IntPtr intPtr5 = IntPtr.Zero;
		IntPtr intPtr6 = IntPtr.Zero;
		int num = int.MaxValue;
		bool flag = false;
		uint num2 = 0u;
		uint num3 = 0u;
		int num4 = 0;
		while (num4 < anResIndex.Length)
		{
			flag = false;
			num3 = anResIndex[num4];
			if (2 == astModuRes[num3].nParamType && astModuRes[num3].nValueNum > 0)
			{
				if (IntPtr.Zero == intPtr && astrResName[num3] == text)
				{
					intPtr = astModuRes[num3].pFloatValue;
					num = Math.Min(num, astModuRes[num3].nValueNum);
					flag = true;
					num2++;
				}
				else if (IntPtr.Zero == intPtr2 && astrResName[num3] == text2)
				{
					intPtr2 = astModuRes[num3].pFloatValue;
					num = Math.Min(num, astModuRes[num3].nValueNum);
					flag = true;
					num2++;
				}
				else if (IntPtr.Zero == intPtr3 && astrResName[num3] == text3)
				{
					intPtr3 = astModuRes[num3].pFloatValue;
					num = Math.Min(num, astModuRes[num3].nValueNum);
					flag = true;
					num2++;
				}
				else if (IntPtr.Zero == intPtr4 && astrResName[num3] == text4)
				{
					intPtr4 = astModuRes[num3].pFloatValue;
					num = Math.Min(num, astModuRes[num3].nValueNum);
					flag = true;
					num2++;
				}
				else if (IntPtr.Zero == intPtr5 && astrResName[num3] == text5)
				{
					intPtr5 = astModuRes[num3].pFloatValue;
					num = Math.Min(num, astModuRes[num3].nValueNum);
					flag = true;
					num2++;
				}
				else if (IntPtr.Zero == intPtr6 && astrResName[num3] == text6)
				{
					intPtr6 = astModuRes[num3].pFloatValue;
					num = Math.Min(num, astModuRes[num3].nValueNum);
					flag = true;
					num2++;
				}
			}
			if (flag)
			{
				if (num4 < nIndexOffset)
				{
					anResIndex[num4] = anResIndex[nIndexOffset];
					anResIndex[nIndexOffset] = num3;
					nIndexOffset--;
				}
				else if (num4 == nIndexOffset)
				{
					nIndexOffset--;
					num4++;
				}
				if (num2 >= 6)
				{
					break;
				}
			}
			else
			{
				num4++;
			}
		}
		if (IntPtr.Zero == intPtr || IntPtr.Zero == intPtr2 || IntPtr.Zero == intPtr3 || IntPtr.Zero == intPtr4 || IntPtr.Zero == intPtr5 || IntPtr.Zero == intPtr6 || int.MaxValue == num)
		{
			return;
		}
		float* ptr = (float*)intPtr.ToPointer();
		float* ptr2 = (float*)intPtr2.ToPointer();
		float* ptr3 = (float*)intPtr3.ToPointer();
		float* ptr4 = (float*)intPtr4.ToPointer();
		float* ptr5 = (float*)intPtr5.ToPointer();
		float* ptr6 = (float*)intPtr6.ToPointer();
		Annulus annulus = new Annulus();
		if (!bMultiRes)
		{
			annulus.CenterPoint.X = *ptr;
			annulus.CenterPoint.Y = *ptr2;
			annulus.InnerRadius = *ptr3;
			annulus.OuterRadius = *ptr4;
			annulus.StartAngle = *ptr5;
			annulus.AngleExtend = *ptr6;
			propInfo.SetValue(stModuBase, annulus, null);
			return;
		}
		List<Annulus> list = new List<Annulus>(num);
		for (int i = 0; i < num; i++)
		{
			annulus = new Annulus();
			annulus.CenterPoint.X = ptr[i];
			annulus.CenterPoint.Y = ptr2[i];
			annulus.InnerRadius = ptr3[i];
			annulus.OuterRadius = ptr4[i];
			annulus.StartAngle = ptr5[i];
			annulus.AngleExtend = ptr6[i];
			list.Add(annulus);
		}
		propInfo.SetValue(stModuBase, list, null);
	}

	private unsafe void SetCircleResultData(PropertyInfo propInfo, ref int nIndexOffset, ref uint[] anResIndex, ref string[] astrResName, ref ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_EX[] astModuRes, ref CModuleResultBase stModuBase, bool bMultiRes = false)
	{
		string text = "";
		string text2 = "";
		string text3 = "";
		switch (propInfo.Name)
		{
		case "StandardCircle":
			text = "StandardCircleRadius";
			text2 = "StandardCircleCenterX";
			text3 = "StandardCircleCenterY";
			break;
		case "MeasureCircle":
			text = "InputCircleRadius";
			text2 = "InputCircleCenterPointX";
			text3 = "InputCircleCenterPointY";
			break;
		case "MeasureCircle1":
			text = "Circle1Radius";
			text2 = "Circle1CenterX";
			text3 = "Circle1CenterY";
			break;
		case "MeasureCircle2":
			text = "Circle2Radius";
			text2 = "Circle2CenterX";
			text3 = "Circle2CenterY";
			break;
		case "OutputResultCircle":
			text = "Radius";
			text2 = "CircleCenterX";
			text3 = "CircleCenterY";
			break;
		default:
			text = "Radius";
			text2 = "CenterX";
			text3 = "CenterY";
			break;
		}
		IntPtr intPtr = IntPtr.Zero;
		IntPtr intPtr2 = IntPtr.Zero;
		IntPtr intPtr3 = IntPtr.Zero;
		int num = int.MaxValue;
		bool flag = false;
		uint num2 = 0u;
		uint num3 = 0u;
		int num4 = 0;
		while (num4 < anResIndex.Length)
		{
			flag = false;
			num3 = anResIndex[num4];
			if (2 == astModuRes[num3].nParamType && astModuRes[num3].nValueNum > 0)
			{
				if (IntPtr.Zero == intPtr && astrResName[num3] == text)
				{
					intPtr = astModuRes[num3].pFloatValue;
					num = Math.Min(num, astModuRes[num3].nValueNum);
					flag = true;
					num2++;
				}
				else if (IntPtr.Zero == intPtr2 && astrResName[num3] == text2)
				{
					intPtr2 = astModuRes[num3].pFloatValue;
					num = Math.Min(num, astModuRes[num3].nValueNum);
					flag = true;
					num2++;
				}
				else if (IntPtr.Zero == intPtr3 && astrResName[num3] == text3)
				{
					intPtr3 = astModuRes[num3].pFloatValue;
					num = Math.Min(num, astModuRes[num3].nValueNum);
					flag = true;
					num2++;
				}
			}
			if (flag)
			{
				if (num4 < nIndexOffset)
				{
					anResIndex[num4] = anResIndex[nIndexOffset];
					anResIndex[nIndexOffset] = num3;
					nIndexOffset--;
				}
				else if (num4 == nIndexOffset)
				{
					nIndexOffset--;
					num4++;
				}
				if (num2 >= 3)
				{
					break;
				}
			}
			else
			{
				num4++;
			}
		}
		if (IntPtr.Zero == intPtr || IntPtr.Zero == intPtr2 || IntPtr.Zero == intPtr3 || int.MaxValue == num)
		{
			return;
		}
		float* ptr = (float*)intPtr.ToPointer();
		float* ptr2 = (float*)intPtr2.ToPointer();
		float* ptr3 = (float*)intPtr3.ToPointer();
		Circle circle = new Circle();
		if (!bMultiRes)
		{
			circle.Radius = *ptr;
			circle.CenterPoint.X = *ptr2;
			circle.CenterPoint.Y = *ptr3;
			propInfo.SetValue(stModuBase, circle, null);
			return;
		}
		List<Circle> list = new List<Circle>(num);
		for (int i = 0; i < num; i++)
		{
			circle = new Circle();
			circle.Radius = ptr[i];
			circle.CenterPoint.X = ptr2[i];
			circle.CenterPoint.Y = ptr3[i];
			list.Add(circle);
		}
		propInfo.SetValue(stModuBase, list, null);
	}

	private unsafe void SetDefectFixtureResultData(PropertyInfo propInfo, ref int nIndexOffset, ref uint[] anResIndex, ref string[] astrResName, ref ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_EX[] astModuRes, ref CModuleResultBase stModuBase, bool bMultiRes = false)
	{
		string text = "MinWidth";
		string text2 = "MaxWidth";
		string text3 = "AverageWidth";
		string text4 = "ApproximateArea";
		IntPtr intPtr = IntPtr.Zero;
		IntPtr intPtr2 = IntPtr.Zero;
		IntPtr intPtr3 = IntPtr.Zero;
		IntPtr intPtr4 = IntPtr.Zero;
		int num = int.MaxValue;
		bool flag = false;
		uint num2 = 0u;
		uint num3 = 0u;
		int num4 = 0;
		while (num4 < anResIndex.Length)
		{
			flag = false;
			num3 = anResIndex[num4];
			if (2 == astModuRes[num3].nParamType && astModuRes[num3].nValueNum > 0)
			{
				if (IntPtr.Zero == intPtr && astrResName[num3] == text)
				{
					intPtr = astModuRes[num3].pFloatValue;
					num = Math.Min(num, astModuRes[num3].nValueNum);
					flag = true;
					num2++;
				}
				else if (IntPtr.Zero == intPtr2 && astrResName[num3] == text2)
				{
					intPtr2 = astModuRes[num3].pFloatValue;
					num = Math.Min(num, astModuRes[num3].nValueNum);
					flag = true;
					num2++;
				}
				else if (IntPtr.Zero == intPtr3 && astrResName[num3] == text3)
				{
					intPtr3 = astModuRes[num3].pFloatValue;
					num = Math.Min(num, astModuRes[num3].nValueNum);
					flag = true;
					num2++;
				}
				else if (IntPtr.Zero == intPtr4 && astrResName[num3] == text4)
				{
					intPtr4 = astModuRes[num3].pFloatValue;
					num = Math.Min(num, astModuRes[num3].nValueNum);
					flag = true;
					num2++;
				}
			}
			if (flag)
			{
				if (num4 < nIndexOffset)
				{
					anResIndex[num4] = anResIndex[nIndexOffset];
					anResIndex[nIndexOffset] = num3;
					nIndexOffset--;
				}
				else if (num4 == nIndexOffset)
				{
					nIndexOffset--;
					num4++;
				}
				if (num2 >= 4)
				{
					break;
				}
			}
			else
			{
				num4++;
			}
		}
		if (IntPtr.Zero == intPtr || IntPtr.Zero == intPtr2 || IntPtr.Zero == intPtr3 || IntPtr.Zero == intPtr4 || int.MaxValue == num)
		{
			return;
		}
		float* ptr = (float*)intPtr.ToPointer();
		float* ptr2 = (float*)intPtr2.ToPointer();
		float* ptr3 = (float*)intPtr3.ToPointer();
		float* ptr4 = (float*)intPtr4.ToPointer();
		DefectFixture defectFixture = new DefectFixture();
		if (!bMultiRes)
		{
			defectFixture.MinWidth = *ptr;
			defectFixture.MaxWidth = *ptr2;
			defectFixture.AverageWidth = *ptr3;
			defectFixture.ApproximateArea = *ptr4;
			propInfo.SetValue(stModuBase, defectFixture, null);
			return;
		}
		List<DefectFixture> list = new List<DefectFixture>(num);
		for (int i = 0; i < num; i++)
		{
			defectFixture = new DefectFixture();
			defectFixture.MinWidth = ptr[i];
			defectFixture.MaxWidth = ptr2[i];
			defectFixture.AverageWidth = ptr3[i];
			defectFixture.ApproximateArea = ptr4[i];
			list.Add(defectFixture);
		}
		propInfo.SetValue(stModuBase, list, null);
	}

	private unsafe void SetLineResultData(PropertyInfo propInfo, ref int nIndexOffset, ref uint[] anResIndex, ref string[] astrResName, ref ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_EX[] astModuRes, ref CModuleResultBase stModuBase, bool bMultiRes = false)
	{
		string text = propInfo.Name + "StartPointX";
		string text2 = propInfo.Name + "StartPointY";
		string text3 = propInfo.Name + "EndPointX";
		string text4 = propInfo.Name + "EndPointY";
		string text5 = propInfo.Name + "Angle";
		string text6 = "";
		switch (propInfo.Name)
		{
		case "OutputLine":
			text = "LineStartPointX";
			text2 = "LineStartPointY";
			text3 = "LineEndPointX";
			text4 = "LineEndPointY";
			text5 = "LineAngle";
			break;
		case "MeasureLine":
			text6 = "InputLine";
			text = text6 + "StartPointX";
			text2 = text6 + "StartPointY";
			text3 = text6 + "EndPointX";
			text4 = text6 + "EndPointY";
			text5 = text6 + "Angle";
			break;
		case "MeasureLine1":
			text6 = "InputLine1";
			text = text6 + "StartPointX";
			text2 = text6 + "StartPointY";
			text3 = text6 + "EndPointX";
			text4 = text6 + "EndPointY";
			text5 = text6 + "Angle";
			break;
		case "MeasureLine2":
			text6 = "InputLine2";
			text = text6 + "StartPointX";
			text2 = text6 + "StartPointY";
			text3 = text6 + "EndPointX";
			text4 = text6 + "EndPointY";
			text5 = text6 + "Angle";
			break;
		case "OutputResultLine":
			text = "StartX";
			text2 = "StartY";
			text3 = "EndX";
			text4 = "EndY";
			text5 = "LineAngle";
			break;
		case "EdgeMidLine":
			text = "LineMidStartPointX";
			text2 = "LineMidStartPointY";
			text3 = "LineMidEndPointX";
			text4 = "LineMidEndPointY";
			text5 = "LineMidAngle";
			break;
		case "LinesInfo":
			text = "StartPointX";
			text2 = "StartPointY";
			text3 = "EndPointX";
			text4 = "EndPointY";
			text5 = "LineAngle";
			break;
		case "StandardLine":
			text = "StandardLinePointStartX";
			text2 = "StandardLinePointStartY";
			text3 = "StandardLinePointEndX";
			text4 = "StandardLinePointEndY";
			text5 = "StandardLineAngle";
			break;
		case "InputOutLine":
			text = "InOutLinePointStartX";
			text2 = "InOutLinePointStartY";
			text3 = "InOutLinePointEndX";
			text4 = "InOutLinePointEndY";
			break;
		case "MedianLine":
			text = "LineMidStartPointX";
			text2 = "LineMidStartPointY";
			text3 = "LineMidEndPointX";
			text4 = "LineMidEndPointY";
			text5 = "LineAngle";
			break;
		case "AngleBisector":
			text5 = "L2LAngle";
			break;
		case "ParallelLine":
		case "ParallelLine1":
		case "ParallelLine2":
			text5 = "InputLineAngle";
			break;
		case "VerticalLine":
			text5 = "OutputLineAngle";
			break;
		case "MinDistMatchedLine":
			text = "MinDistPoint0X";
			text2 = "MinDistPoint0Y";
			text3 = "MinDistPoint1X";
			text4 = "MinDistPoint1Y";
			text5 = "Angle";
			break;
		case "MaxDistMatchedLine":
			text = "MaxDistPoint0X";
			text2 = "MaxDistPoint0Y";
			text3 = "MaxDistPoint1X";
			text4 = "MaxDistPoint1Y";
			text5 = "Angle";
			break;
		case "ListOutputResultLine":
			text = "ListStartX";
			text2 = "ListStartY";
			text3 = "ListEndX";
			text4 = "ListEndY";
			text5 = "ListLineAngle";
			break;
		default:
			if (propInfo.Name.Contains("EdgeLine"))
			{
				string text7 = propInfo.Name.Substring(propInfo.Name.Length - 1);
				text = "Line" + text7 + "StartPointX";
				text2 = "Line" + text7 + "StartPointY";
				text3 = "Line" + text7 + "EndPointX";
				text4 = "Line" + text7 + "EndPointY";
				text5 = "Line" + text7 + "Angle";
			}
			break;
		}
		IntPtr intPtr = IntPtr.Zero;
		IntPtr intPtr2 = IntPtr.Zero;
		IntPtr intPtr3 = IntPtr.Zero;
		IntPtr intPtr4 = IntPtr.Zero;
		IntPtr intPtr5 = IntPtr.Zero;
		int num = int.MaxValue;
		bool flag = false;
		uint num2 = 0u;
		uint num3 = 0u;
		int num4 = 0;
		while (num4 < anResIndex.Length)
		{
			flag = false;
			num3 = anResIndex[num4];
			if (2 == astModuRes[num3].nParamType && astModuRes[num3].nValueNum > 0)
			{
				if (IntPtr.Zero == intPtr && astrResName[num3] == text)
				{
					intPtr = astModuRes[num3].pFloatValue;
					num = Math.Min(num, astModuRes[num3].nValueNum);
					flag = true;
					num2++;
				}
				else if (IntPtr.Zero == intPtr2 && astrResName[num3] == text2)
				{
					intPtr2 = astModuRes[num3].pFloatValue;
					num = Math.Min(num, astModuRes[num3].nValueNum);
					flag = true;
					num2++;
				}
				else if (IntPtr.Zero == intPtr3 && astrResName[num3] == text3)
				{
					intPtr3 = astModuRes[num3].pFloatValue;
					num = Math.Min(num, astModuRes[num3].nValueNum);
					flag = true;
					num2++;
				}
				else if (IntPtr.Zero == intPtr4 && astrResName[num3] == text4)
				{
					intPtr4 = astModuRes[num3].pFloatValue;
					num = Math.Min(num, astModuRes[num3].nValueNum);
					flag = true;
					num2++;
				}
				else if (IntPtr.Zero == intPtr5 && astrResName[num3] == text5)
				{
					intPtr5 = astModuRes[num3].pFloatValue;
					num = Math.Min(num, astModuRes[num3].nValueNum);
					flag = true;
					num2++;
				}
			}
			if (flag)
			{
				if (num4 < nIndexOffset)
				{
					anResIndex[num4] = anResIndex[nIndexOffset];
					anResIndex[nIndexOffset] = num3;
					nIndexOffset--;
				}
				else if (num4 == nIndexOffset)
				{
					nIndexOffset--;
					num4++;
				}
				if (num2 >= 5)
				{
					break;
				}
			}
			else
			{
				num4++;
			}
		}
		if (IntPtr.Zero == intPtr || IntPtr.Zero == intPtr2 || IntPtr.Zero == intPtr3 || IntPtr.Zero == intPtr4 || int.MaxValue == num)
		{
			return;
		}
		float* ptr = (float*)intPtr.ToPointer();
		float* ptr2 = (float*)intPtr2.ToPointer();
		float* ptr3 = (float*)intPtr3.ToPointer();
		float* ptr4 = (float*)intPtr4.ToPointer();
		float* ptr5 = (float*)intPtr5.ToPointer();
		Line line = new Line();
		if (!bMultiRes)
		{
			line.StartPoint.X = *ptr;
			line.StartPoint.Y = *ptr2;
			line.EndPoint.X = *ptr3;
			line.EndPoint.Y = *ptr4;
			if (null != ptr5)
			{
				line.Angle = *ptr5;
			}
			propInfo.SetValue(stModuBase, line, null);
			return;
		}
		List<Line> list = new List<Line>(num);
		if (null != ptr5)
		{
			for (int i = 0; i < num; i++)
			{
				line = new Line();
				line.StartPoint.X = ptr[i];
				line.StartPoint.Y = ptr2[i];
				line.EndPoint.X = ptr3[i];
				line.EndPoint.Y = ptr4[i];
				line.Angle = ptr5[i];
				list.Add(line);
			}
		}
		else
		{
			for (int j = 0; j < num; j++)
			{
				line = new Line();
				line.StartPoint.X = ptr[j];
				line.StartPoint.Y = ptr2[j];
				line.EndPoint.X = ptr3[j];
				line.EndPoint.Y = ptr4[j];
				list.Add(line);
			}
		}
		propInfo.SetValue(stModuBase, list, null);
	}

	private unsafe static void SetOutLineResultData(PropertyInfo propInfo, ref int nIndexOffset, ref uint[] anResIndex, ref string[] astrResName, ref ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_EX[] astModuRes, ref CModuleResultBase stModuBase, bool bIsShallowCopy, uint nExecuteCount)
	{
		uint num = 0u;
		for (int i = 0; i < anResIndex.Length; i++)
		{
			num = anResIndex[i];
			if (5 != astModuRes[num].nParamType || astModuRes[num].nValueNum <= 0)
			{
				continue;
			}
			MatchOutline matchOutline = new MatchOutline();
			ImvsSdkDefine.IMVS_MODULE_CHUNK_VALUE_EX[] array = ExternalCallHelper.PtrToStructurs<ImvsSdkDefine.IMVS_MODULE_CHUNK_VALUE_EX>(astModuRes[num].pChunkValue, astModuRes[num].nValueNum);
			if (array[0].nUseRealData == 0 || 2 == array[0].nUseRealData)
			{
				SharedMemoryItemVM sharedMemoryItemVM = new SharedMemoryItemVM(array[0].strValue, nExecuteCount);
				if (bIsShallowCopy)
				{
					matchOutline.sharedMemObj = sharedMemoryItemVM;
				}
				else
				{
					IntPtr dataPtr = sharedMemoryItemVM.GetDataPtr();
					if (IntPtr.Zero != dataPtr)
					{
						int num2 = Marshal.SizeOf(typeof(MatchPoint));
						int num3 = (int)sharedMemoryItemVM.DataLen / num2;
						MatchPoint* ptr = (MatchPoint*)dataPtr.ToPointer();
						matchOutline.MatchOutlinePoints = new List<MatchPoint>(num3);
						for (int j = 0; j < num3; j++)
						{
							matchOutline.MatchOutlinePoints.Add(ptr[j]);
						}
					}
					GC.KeepAlive(sharedMemoryItemVM);
				}
			}
			if (1 == array[0].nUseRealData)
			{
				if (bIsShallowCopy)
				{
					matchOutline.dataPtr = array[0].pData;
					matchOutline.dataLen = array[0].nLen;
				}
				else if (IntPtr.Zero != array[0].pData)
				{
					int num4 = Marshal.SizeOf(typeof(MatchPoint));
					int num5 = (int)array[0].nLen / num4;
					MatchPoint* ptr2 = (MatchPoint*)array[0].pData.ToPointer();
					matchOutline.MatchOutlinePoints = new List<MatchPoint>(num5);
					for (int k = 0; k < num5; k++)
					{
						matchOutline.MatchOutlinePoints.Add(ptr2[k]);
					}
				}
			}
			propInfo.SetValue(stModuBase, matchOutline, null);
			if (i <= nIndexOffset)
			{
				anResIndex[i] = anResIndex[nIndexOffset];
				anResIndex[nIndexOffset] = num;
				nIndexOffset--;
			}
			break;
		}
	}

	private bool HasNumber(string strInput)
	{
		foreach (char c in strInput)
		{
			if (char.IsNumber(c))
			{
				return true;
			}
		}
		return false;
	}

	private unsafe void SetPointFResultData(PropertyInfo propInfo, ref int nIndexOffset, ref uint[] anResIndex, ref string[] astrResName, ref ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_EX[] astModuRes, ref CModuleResultBase stModuBase, bool bMultiRes = false)
	{
		string text = propInfo.Name + "X";
		string text2 = propInfo.Name + "Y";
		switch (propInfo.Name)
		{
		case "PointsAssemble":
		case "OutputPoint":
			text = "PointX";
			text2 = "PointY";
			break;
		case "Midpoint":
			text = "PointMidX";
			text2 = "PointMidY";
			break;
		case "MeasurePoint":
			text = "InputPointX";
			text2 = "InputPointY";
			break;
		case "MeasurePoint1":
			text = "InputPoint1X";
			text2 = "InputPoint1Y";
			break;
		case "MeasurePoint2":
			text = "InputPoint2X";
			text2 = "InputPoint2Y";
			break;
		case "FootPoint":
			text = "PointProjx";
			text2 = "PointProjy";
			break;
		case "OutputFittingPoint":
			text = "FitPointX";
			text2 = "FitPointY";
			break;
		case "CaliperPoint":
			text = "CalipIdeaPointX";
			text2 = "CalipIdeaPointY";
			break;
		case "ContourPoint":
			text = "EdgePointX";
			text2 = "EdgePointY";
			break;
		case "Line0ContourPoint":
			text = "Edge0PointX";
			text2 = "Edge0PointY";
			break;
		case "Line1ContourPoint":
			text = "Edge1PointX";
			text2 = "Edge1PointY";
			break;
		case "Line2ContourPoint":
			text = "Edge2PointX";
			text2 = "Edge2PointY";
			break;
		case "Line3ContourPoint":
			text = "Edge3PointX";
			text2 = "Edge3PointY";
			break;
		case "Line4ContourPoint":
			text = "Edge4PointX";
			text2 = "Edge4PointY";
			break;
		case "EdgePoints":
			text = "EdgePointX";
			text2 = "EdgePointY";
			break;
		case "EdgePoint":
			text = "CaliperEdgePointX";
			text2 = "CaliperEdgePointY";
			break;
		case "TopPoint":
			text = "PeakPointX";
			text2 = "PeakPointY";
			break;
		case "EdgesInfo":
			text = "EdgePointX";
			text2 = "EdgePointY";
			break;
		case "EdgePoint0":
			text = "CaliperEdge0PointX";
			text2 = "CaliperEdge0PointY";
			break;
		case "EdgePoint1":
			text = "CaliperEdge1PointX";
			text2 = "CaliperEdge1PointY";
			break;
		case "CaliperCorner":
			text = "CornerPointX";
			text2 = "CornerPointY";
			break;
		case "RotAxisPoint":
			text = "RotCenterImageX";
			text2 = "RotCenterImageY";
			break;
		case "RotPhysicalPoint":
			text = "RotCenterWorldX";
			text2 = "RotCenterWorldY";
			break;
		case "CalibImagePoint":
			text = "OutImagePointX";
			text2 = "OutImagePointY";
			break;
		case "CalibPhysicalPoint":
			text = "WorldPointX";
			text2 = "WorldPointY";
			break;
		case "CalibrationPoint":
			text = "CalibPointX";
			text2 = "CalibPointY";
			break;
		case "Intersection":
			text = "L2LPointInterX";
			text2 = "L2LPointInterY";
			break;
		case "Intersection1":
			text = "PointInter1x";
			text2 = "PointInter1y";
			break;
		case "Intersection2":
			text = "PointInter2x";
			text2 = "PointInter2y";
			break;
		case "CornerPoint":
			text = "CodePosX";
			text2 = "CodePosY";
			break;
		case "CaliperIdeaPoint":
			text = "CalipIdeaPointX";
			text2 = "CalipIdeaPointY";
			break;
		case "CurrentCornersPoint":
			text = "CornersCurPointX";
			text2 = "CornersCurPointY";
			break;
		case "ObjectCornersPoint":
			text = "CornersObjPointX";
			text2 = "CornersObjPointY";
			break;
		case "ProjectionCornersPoint":
			text = "CornersProPointX";
			text2 = "CornersProPointY";
			break;
		case "CircleCoutourPoint":
			text = "EdgePointX";
			text2 = "EdgePointY";
			break;
		case "CentroidPoint":
			text = "CentroidX";
			text2 = "CentroidY";
			break;
		case "MidLineCoutourPoint":
			text = "EdgeMidPointX";
			text2 = "EdgeMidPointY";
			break;
		case "LinearContourPoint":
			text = "EdgePointX";
			text2 = "EdgePointY";
			break;
		case "TransPoint":
			text = "WorldPointX";
			text2 = "WorldPointY";
			break;
		case "PosXVector":
			text = "CalibOriPointXVecX";
			text2 = "CalibOriPointXVecY";
			break;
		case "PosYVector":
			text = "CalibOriPointYVecX";
			text2 = "CalibOriPointYVecY";
			break;
		case "InOutRotateCenter":
			text = "InOutRotatePointX";
			text2 = "InOutRotatePointY";
			break;
		case "InputOutPoint":
			text = "InOutPointX";
			text2 = "InOutPointY";
			break;
		case "OutputFittingPoint1":
			text = "FitPoint1X";
			text2 = "FitPoint1Y";
			break;
		case "OutputFittingPoint2":
			text = "FitPoint2X";
			text2 = "FitPoint2Y";
			break;
		case "PolyonContourPoint":
			text = "EdgePointX";
			text2 = "EdgePointY";
			break;
		case "RotCenterImagePoint":
			text = "RotCenterImagePointX";
			text2 = "RotCenterImagePointY";
			break;
		case "RotCenterWorldPoint":
			text = "RotCenterWorldPointX";
			text2 = "RotCenterWorldPointY";
			break;
		case "SnapImagePoint":
			text = "SnapImagePointX";
			text2 = "SnapImagePointY";
			break;
		case "SnapWorldPoint":
			text = "SnapWorldPointX";
			text2 = "SnapWorldPointY";
			break;
		case "TeachWorldPoint":
			text = "TeachWorldPointX";
			text2 = "TeachWorldPointY";
			break;
		case "SnapWorldCurPoint":
			text = "SnapWorldCurPointX";
			text2 = "SnapWorldCurPointY";
			break;
		case "SnapWorldObjPoint":
			text = "SnapWorldObjPointX";
			text2 = "SnapWorldObjPointY";
			break;
		case "GluePathAll":
			text = "GluePointX";
			text2 = "GluePointY";
			break;
		case "DstOutputPoint":
			text = "DstPointX";
			text2 = "DstPointY";
			break;
		case "RunParam_EdgePoint":
			text = "CaliperEdgePointX";
			text2 = "CaliperEdgePointY";
			break;
		case "RunParam_EdgePoint0":
			text = "CaliperEdge0PointX";
			text2 = "CaliperEdge0PointY";
			break;
		case "RunParam_EdgePoint1":
			text = "CaliperEdge1PointX";
			text2 = "CaliperEdge1PointY";
			break;
		case "EdgePosMinDistIndexPoint":
			text = "MinDistPointX";
			text2 = "MinDistPointY";
			break;
		case "EdgePosMaxDistIndexPoint":
			text = "MaxDistPointX";
			text2 = "MaxDistPointY";
			break;
		case "ListOutputPoint":
			text = "ListPointX";
			text2 = "ListPointY";
			break;
		case "EllipseCoutourPoint":
			text = "EllipseCoutourPointX";
			text2 = "EllipseCoutourPointY";
			break;
		case "ObjIdPosition":
			text = "ObjIdPointX";
			text2 = "ObjIdPointY";
			break;
		default:
			if (propInfo.Name.Contains("Vertex"))
			{
				text = propInfo.Name + "PointX";
				text2 = propInfo.Name + "PointY";
			}
			else if (HasNumber(propInfo.Name) && propInfo.Name.Contains("GluePath"))
			{
				string text3 = "GluePath";
				int num = propInfo.Name.IndexOf(text3);
				string text4 = propInfo.Name.Substring(num + text3.Length);
				text = "GluePointX" + text4;
				text2 = "GluePointY" + text4;
			}
			break;
		}
		IntPtr intPtr = IntPtr.Zero;
		IntPtr intPtr2 = IntPtr.Zero;
		IntPtr intPtr3 = IntPtr.Zero;
		IntPtr intPtr4 = IntPtr.Zero;
		int num2 = int.MaxValue;
		bool flag = false;
		uint num3 = 0u;
		uint num4 = 0u;
		uint num5 = 0u;
		int num6 = 0;
		while (num6 < anResIndex.Length)
		{
			flag = false;
			num5 = anResIndex[num6];
			if (astModuRes[num5].nValueNum > 0)
			{
				if (2 == astModuRes[num5].nParamType)
				{
					if (IntPtr.Zero == intPtr && astrResName[num5] == text)
					{
						intPtr = astModuRes[num5].pFloatValue;
						num2 = Math.Min(num2, astModuRes[num5].nValueNum);
						flag = true;
						num3++;
					}
					else if (IntPtr.Zero == intPtr2 && astrResName[num5] == text2)
					{
						intPtr2 = astModuRes[num5].pFloatValue;
						num2 = Math.Min(num2, astModuRes[num5].nValueNum);
						flag = true;
						num3++;
					}
				}
				else if (1 == astModuRes[num5].nParamType)
				{
					if (IntPtr.Zero == intPtr3 && astrResName[num5] == text)
					{
						intPtr3 = astModuRes[num5].pIntValue;
						num2 = Math.Min(num2, astModuRes[num5].nValueNum);
						flag = true;
						num4++;
					}
					else if (IntPtr.Zero == intPtr4 && astrResName[num5] == text2)
					{
						intPtr4 = astModuRes[num5].pIntValue;
						num2 = Math.Min(num2, astModuRes[num5].nValueNum);
						flag = true;
						num4++;
					}
				}
			}
			if (flag)
			{
				if (num6 < nIndexOffset)
				{
					anResIndex[num6] = anResIndex[nIndexOffset];
					anResIndex[nIndexOffset] = num5;
					nIndexOffset--;
				}
				else if (num6 == nIndexOffset)
				{
					nIndexOffset--;
					num6++;
				}
				if (num3 >= 2 || num4 >= 2)
				{
					break;
				}
			}
			else
			{
				num6++;
			}
		}
		if (int.MaxValue == num2)
		{
			return;
		}
		PointF pointF = new PointF();
		if (!bMultiRes)
		{
			if (IntPtr.Zero != intPtr && IntPtr.Zero != intPtr2)
			{
				float* ptr = (float*)intPtr.ToPointer();
				float* ptr2 = (float*)intPtr2.ToPointer();
				pointF.X = *ptr;
				pointF.Y = *ptr2;
				propInfo.SetValue(stModuBase, pointF, null);
			}
			if (IntPtr.Zero != intPtr3 && IntPtr.Zero != intPtr4)
			{
				int* ptr3 = (int*)intPtr3.ToPointer();
				int* ptr4 = (int*)intPtr4.ToPointer();
				pointF.X = *ptr3;
				pointF.Y = *ptr4;
				propInfo.SetValue(stModuBase, pointF, null);
			}
			return;
		}
		List<PointF> list = new List<PointF>(num2);
		if (IntPtr.Zero != intPtr && IntPtr.Zero != intPtr2)
		{
			float* ptr5 = (float*)intPtr.ToPointer();
			float* ptr6 = (float*)intPtr2.ToPointer();
			for (int i = 0; i < num2; i++)
			{
				pointF = new PointF();
				pointF.X = ptr5[i];
				pointF.Y = ptr6[i];
				list.Add(pointF);
			}
			propInfo.SetValue(stModuBase, list, null);
		}
		if (IntPtr.Zero != intPtr3 && IntPtr.Zero != intPtr4)
		{
			int* ptr7 = (int*)intPtr3.ToPointer();
			int* ptr8 = (int*)intPtr4.ToPointer();
			for (int j = 0; j < num2; j++)
			{
				pointF = new PointF();
				pointF.X = ptr7[j];
				pointF.Y = ptr8[j];
				list.Add(pointF);
			}
			propInfo.SetValue(stModuBase, list, null);
		}
	}

	private unsafe void SetRectBoxResultData(PropertyInfo propInfo, ref int nIndexOffset, ref uint[] anResIndex, ref string[] astrResName, ref ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_EX[] astModuRes, ref CModuleResultBase stModuBase, bool bMultiRes = false)
	{
		string text = "CenterX";
		string text2 = "CenterY";
		string text3 = "Width";
		string text4 = "Height";
		string text5 = "Angle";
		switch (propInfo.Name)
		{
		case "ROI":
			text = "DetectCenterX";
			text2 = "DetectCenterY";
			text3 = "DetectWidth";
			text4 = "DetectHeight";
			text5 = "DetectAngle";
			break;
		case "MatchRect":
			text = "MatchBoxCenterX";
			text2 = "MatchBoxCenterY";
			text3 = "MatchBoxWidth";
			text4 = "MatchBoxHeight";
			text5 = "MatchBoxAngle";
			break;
		case "CaliperBox":
			text = "CaliperCenterX";
			text2 = "CaliperCenterY";
			text3 = "CaliperWidth";
			text4 = "CaliperHeight";
			text5 = "CaliperAngle";
			break;
		case "CharRect":
			text = "CharCenterX";
			text2 = "CharCenterY";
			text3 = "CharBoxWidth";
			text4 = "CharBoxHeight";
			text5 = "CharBoxAngle";
			break;
		case "Rect":
			text3 = "BoxWidth";
			text4 = "BoxHeight";
			text5 = "BoxAngle";
			break;
		case "MinBoudingRect":
			text3 = "BoxWidth";
			text4 = "BoxHeight";
			text5 = "Angle";
			break;
		case "TextBox":
		case "IdeaTrajCaliper":
			text = propInfo.Name + text;
			text2 = propInfo.Name + text2;
			text3 = propInfo.Name + text3;
			text4 = propInfo.Name + text4;
			text5 = propInfo.Name + text5;
			break;
		case "CharInfoRect":
			text = "StringCenterX";
			text2 = "StringCenterY";
			text3 = "StringBoxWidth";
			text4 = "StringBoxHeight";
			text5 = "StringBoxAngle";
			break;
		case "_RectBox":
			text = "RectCenterX";
			text2 = "RectCenterY";
			text3 = "RectWidth";
			text4 = "RectHeight";
			text5 = "RectAngle";
			break;
		case "TargetInfoRect":
			text = "ObjCenterX";
			text2 = "ObjCenterY";
			text3 = "ObjBoxWidth";
			text4 = "ObjBoxHeight";
			text5 = "ObjBoxAngle";
			break;
		case "FlawRect":
			text = "FlawCenterX";
			text2 = "FlawCenterY";
			text3 = "FlawWidth";
			text4 = "FlawHeight";
			text5 = "FlawAngle";
			break;
		case "FlawBox":
			text = "FlawBoxCenterX";
			text2 = "FlawBoxCenterY";
			text3 = "FlawBoxWidth";
			text4 = "FlawBoxHeight";
			text5 = "FlawBoxAngle";
			break;
		case "MarkBox":
			text = "MarkBoxCenterX";
			text2 = "MarkBoxCenterY";
			text3 = "MarkBoxWidth";
			text4 = "MarkBoxHeight";
			text5 = "MarkBoxAngle";
			break;
		case "InspROI":
			text = "InspDetectCenterX";
			text2 = "InspDetectCenterY";
			text3 = "InspDetectWidth";
			text4 = "InspDetectHeight";
			text5 = "InspDetectAngle";
			break;
		case "OutputResultBox":
			text = "BoxCenterX";
			text2 = "BoxCenterY";
			text3 = "BoxWidth";
			text4 = "BoxHeight";
			text5 = "BoxAngle";
			break;
		case "MergeBox":
			text = "MergeBoxCenterX";
			text2 = "MergeBoxCenterY";
			text3 = "MergeBoxWidth";
			text4 = "MergeBoxHeight";
			text5 = "MergeBoxAngle";
			break;
		case "DivideRect":
			text = "DivideRectCenterX";
			text2 = "DivideRectCenterY";
			text3 = "DivideRectWidth";
			text4 = "DivideRectHeight";
			text5 = "DivideRectAngle";
			break;
		case "TrackRect":
			text = "TrackRectCenterX";
			text2 = "TrackRectCenterY";
			text3 = "TrackRectWidth";
			text4 = "TrackRectHeight";
			text5 = "TrackRectAngle";
			break;
		case "MinRowRect":
			text = "MinRowRectCenterX";
			text2 = "MinRowRectCenterY";
			text3 = "MinRowRectBoxWidth";
			text4 = "MinRowRectBoxHeight";
			text5 = "MinRowRectAngle";
			break;
		case "FixRect":
			text = "FixRectCenterX";
			text2 = "FixRectCenterY";
			text3 = "FixRectBoxWidth";
			text4 = "FixRectBoxHeight";
			text5 = "FixRectAngle";
			break;
		case "ListOutputResultBox":
			text = "ListBoxCenterX";
			text2 = "ListBoxCenterY";
			text3 = "ListBoxWidth";
			text4 = "ListBoxHeight";
			text5 = "ListBoxAngle";
			break;
		case "DetectROIs":
			text = "DetectROIsCenterX";
			text2 = "DetectROIsCenterY";
			text3 = "DetectROIsWidth";
			text4 = "DetectROIsHeight";
			text5 = "DetectROIsAngle";
			break;
		case "MaxAreaInnerRect":
			text = "MaxAreaInnerRectCenterX";
			text2 = "MaxAreaInnerRectCenterY";
			text3 = "MaxAreaInnerRectWidth";
			text4 = "MaxAreaInnerRectHeight";
			text5 = "MaxAreaInnerRectAngle";
			break;
		default:
			if (HasNumber(propInfo.Name))
			{
				if (propInfo.Name.Contains("ROI"))
				{
					string text6 = propInfo.Name.Substring(propInfo.Name.Length - 1, 1);
					text = "Detect" + text6 + "CenterX";
					text2 = "Detect" + text6 + "CenterY";
					text3 = "Detect" + text6 + "Width";
					text4 = "Detect" + text6 + "Height";
					text5 = "Detect" + text6 + "Angle";
					break;
				}
				string text7 = propInfo.Name.Substring(propInfo.Name.Length - 1, 1);
				int num = propInfo.Name.IndexOf("Box");
				string text8 = "";
				if (num >= 0)
				{
					text8 = propInfo.Name.Remove(num);
				}
				text = text8 + text + text7;
				text2 = text8 + text2 + text7;
				text3 = text8 + text3 + text7;
				text4 = text8 + text4 + text7;
				text5 = text8 + text5 + text7;
			}
			else
			{
				int num2 = propInfo.Name.IndexOf("Box");
				string text9 = "";
				if (num2 >= 0)
				{
					text9 = propInfo.Name.Remove(num2);
				}
				text = text9 + text;
				text2 = text9 + text2;
				text3 = text9 + text3;
				text4 = text9 + text4;
				text5 = text9 + text5;
			}
			break;
		}
		IntPtr intPtr = IntPtr.Zero;
		IntPtr intPtr2 = IntPtr.Zero;
		IntPtr intPtr3 = IntPtr.Zero;
		IntPtr intPtr4 = IntPtr.Zero;
		IntPtr intPtr5 = IntPtr.Zero;
		int num3 = int.MaxValue;
		bool flag = false;
		uint num4 = 0u;
		uint num5 = 0u;
		int num6 = 0;
		while (num6 < anResIndex.Length)
		{
			flag = false;
			num5 = anResIndex[num6];
			if (2 == astModuRes[num5].nParamType && astModuRes[num5].nValueNum > 0)
			{
				if (IntPtr.Zero == intPtr && astrResName[num5] == text)
				{
					intPtr = astModuRes[num5].pFloatValue;
					num3 = Math.Min(num3, astModuRes[num5].nValueNum);
					flag = true;
					num4++;
				}
				else if (IntPtr.Zero == intPtr2 && astrResName[num5] == text2)
				{
					intPtr2 = astModuRes[num5].pFloatValue;
					num3 = Math.Min(num3, astModuRes[num5].nValueNum);
					flag = true;
					num4++;
				}
				else if (IntPtr.Zero == intPtr3 && astrResName[num5] == text3)
				{
					intPtr3 = astModuRes[num5].pFloatValue;
					num3 = Math.Min(num3, astModuRes[num5].nValueNum);
					flag = true;
					num4++;
				}
				else if (IntPtr.Zero == intPtr4 && astrResName[num5] == text4)
				{
					intPtr4 = astModuRes[num5].pFloatValue;
					num3 = Math.Min(num3, astModuRes[num5].nValueNum);
					flag = true;
					num4++;
				}
				else if (IntPtr.Zero == intPtr5 && astrResName[num5] == text5)
				{
					intPtr5 = astModuRes[num5].pFloatValue;
					num3 = Math.Min(num3, astModuRes[num5].nValueNum);
					flag = true;
					num4++;
				}
			}
			if (flag)
			{
				if (num6 < nIndexOffset)
				{
					anResIndex[num6] = anResIndex[nIndexOffset];
					anResIndex[nIndexOffset] = num5;
					nIndexOffset--;
				}
				else if (num6 == nIndexOffset)
				{
					nIndexOffset--;
					num6++;
				}
				if (num4 >= 5)
				{
					break;
				}
			}
			else
			{
				num6++;
			}
		}
		if (IntPtr.Zero == intPtr || IntPtr.Zero == intPtr2 || IntPtr.Zero == intPtr3 || IntPtr.Zero == intPtr4 || IntPtr.Zero == intPtr5 || int.MaxValue == num3)
		{
			return;
		}
		float* ptr = (float*)intPtr.ToPointer();
		float* ptr2 = (float*)intPtr2.ToPointer();
		float* ptr3 = (float*)intPtr3.ToPointer();
		float* ptr4 = (float*)intPtr4.ToPointer();
		float* ptr5 = (float*)intPtr5.ToPointer();
		RectBox rectBox = new RectBox();
		if (!bMultiRes)
		{
			rectBox.CenterPoint.X = *ptr;
			rectBox.CenterPoint.Y = *ptr2;
			rectBox.BoxWidth = *ptr3;
			rectBox.BoxHeight = *ptr4;
			rectBox.Angle = *ptr5;
			propInfo.SetValue(stModuBase, rectBox, null);
			return;
		}
		List<RectBox> list = new List<RectBox>(num3);
		for (int i = 0; i < num3; i++)
		{
			rectBox = new RectBox();
			rectBox.CenterPoint.X = ptr[i];
			rectBox.CenterPoint.Y = ptr2[i];
			rectBox.BoxWidth = ptr3[i];
			rectBox.BoxHeight = ptr4[i];
			rectBox.Angle = ptr5[i];
			list.Add(rectBox);
		}
		propInfo.SetValue(stModuBase, list, null);
	}

	private unsafe void SetRectIResultData(PropertyInfo propInfo, ref int nIndexOffset, ref uint[] anResIndex, ref string[] astrResName, ref ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_EX[] astModuRes, ref CModuleResultBase stModuBase, bool bMultiRes = false)
	{
		string text = "X";
		string text2 = "Y";
		string text3 = "Width";
		string text4 = "Height";
		if (propInfo.Name == "BlobRect")
		{
			text = propInfo.Name + text;
			text2 = propInfo.Name + text2;
			text3 = propInfo.Name + text3;
			text4 = propInfo.Name + text4;
		}
		IntPtr intPtr = IntPtr.Zero;
		IntPtr intPtr2 = IntPtr.Zero;
		IntPtr intPtr3 = IntPtr.Zero;
		IntPtr intPtr4 = IntPtr.Zero;
		IntPtr intPtr5 = IntPtr.Zero;
		IntPtr intPtr6 = IntPtr.Zero;
		IntPtr intPtr7 = IntPtr.Zero;
		IntPtr intPtr8 = IntPtr.Zero;
		int num = int.MaxValue;
		bool flag = false;
		uint num2 = 0u;
		uint num3 = 0u;
		uint num4 = 0u;
		int num5 = 0;
		while (num5 < anResIndex.Length)
		{
			flag = false;
			num4 = anResIndex[num5];
			if (astModuRes[num4].nValueNum > 0)
			{
				if (1 == astModuRes[num4].nParamType)
				{
					if (IntPtr.Zero == intPtr && astrResName[num4] == text)
					{
						intPtr = astModuRes[num4].pIntValue;
						num = Math.Min(num, astModuRes[num4].nValueNum);
						flag = true;
						num3++;
					}
					else if (IntPtr.Zero == intPtr2 && astrResName[num4] == text2)
					{
						intPtr2 = astModuRes[num4].pIntValue;
						num = Math.Min(num, astModuRes[num4].nValueNum);
						flag = true;
						num3++;
					}
					else if (IntPtr.Zero == intPtr3 && astrResName[num4] == text3)
					{
						intPtr3 = astModuRes[num4].pIntValue;
						num = Math.Min(num, astModuRes[num4].nValueNum);
						flag = true;
						num3++;
					}
					else if (IntPtr.Zero == intPtr4 && astrResName[num4] == text4)
					{
						intPtr4 = astModuRes[num4].pIntValue;
						num = Math.Min(num, astModuRes[num4].nValueNum);
						flag = true;
						num3++;
					}
				}
				else if (2 == astModuRes[num4].nParamType)
				{
					if (IntPtr.Zero == intPtr5 && astrResName[num4] == text)
					{
						intPtr5 = astModuRes[num4].pFloatValue;
						num = Math.Min(num, astModuRes[num4].nValueNum);
						flag = true;
						num2++;
					}
					else if (IntPtr.Zero == intPtr6 && astrResName[num4] == text2)
					{
						intPtr6 = astModuRes[num4].pFloatValue;
						num = Math.Min(num, astModuRes[num4].nValueNum);
						flag = true;
						num2++;
					}
					else if (IntPtr.Zero == intPtr7 && astrResName[num4] == text3)
					{
						intPtr7 = astModuRes[num4].pFloatValue;
						num = Math.Min(num, astModuRes[num4].nValueNum);
						flag = true;
						num2++;
					}
					else if (IntPtr.Zero == intPtr8 && astrResName[num4] == text4)
					{
						intPtr8 = astModuRes[num4].pFloatValue;
						num = Math.Min(num, astModuRes[num4].nValueNum);
						flag = true;
						num2++;
					}
				}
			}
			if (flag)
			{
				if (num5 < nIndexOffset)
				{
					anResIndex[num5] = anResIndex[nIndexOffset];
					anResIndex[nIndexOffset] = num4;
					nIndexOffset--;
				}
				else if (num5 == nIndexOffset)
				{
					nIndexOffset--;
					num5++;
				}
				if (num3 >= 4 || num2 >= 4)
				{
					break;
				}
			}
			else
			{
				num5++;
			}
		}
		if (int.MaxValue == num)
		{
			return;
		}
		RectI rectI = new RectI();
		if (!bMultiRes)
		{
			if (IntPtr.Zero != intPtr && IntPtr.Zero != intPtr2 && IntPtr.Zero != intPtr3 && IntPtr.Zero != intPtr4)
			{
				int* ptr = (int*)intPtr.ToPointer();
				int* ptr2 = (int*)intPtr2.ToPointer();
				int* ptr3 = (int*)intPtr3.ToPointer();
				int* ptr4 = (int*)intPtr4.ToPointer();
				rectI.RectPoint.X = *ptr;
				rectI.RectPoint.Y = *ptr2;
				rectI.RectWidth = *ptr3;
				rectI.RectHeight = *ptr4;
				propInfo.SetValue(stModuBase, rectI, null);
			}
			if (IntPtr.Zero != intPtr5 && IntPtr.Zero != intPtr6 && IntPtr.Zero != intPtr7 && IntPtr.Zero != intPtr8)
			{
				float* ptr5 = (float*)intPtr5.ToPointer();
				float* ptr6 = (float*)intPtr6.ToPointer();
				float* ptr7 = (float*)intPtr7.ToPointer();
				float* ptr8 = (float*)intPtr8.ToPointer();
				rectI.RectPoint.X = (int)(*ptr5);
				rectI.RectPoint.Y = (int)(*ptr6);
				rectI.RectWidth = (int)(*ptr7);
				rectI.RectHeight = (int)(*ptr8);
				propInfo.SetValue(stModuBase, rectI, null);
			}
			return;
		}
		List<RectI> list = new List<RectI>(num);
		if (IntPtr.Zero != intPtr && IntPtr.Zero != intPtr2 && IntPtr.Zero != intPtr3 && IntPtr.Zero != intPtr4)
		{
			int* ptr9 = (int*)intPtr.ToPointer();
			int* ptr10 = (int*)intPtr2.ToPointer();
			int* ptr11 = (int*)intPtr3.ToPointer();
			int* ptr12 = (int*)intPtr4.ToPointer();
			for (int i = 0; i < num; i++)
			{
				rectI = new RectI();
				rectI.RectPoint.X = ptr9[i];
				rectI.RectPoint.Y = ptr10[i];
				rectI.RectWidth = ptr11[i];
				rectI.RectHeight = ptr12[i];
				list.Add(rectI);
			}
			propInfo.SetValue(stModuBase, list, null);
		}
		if (IntPtr.Zero != intPtr5 && IntPtr.Zero != intPtr6 && IntPtr.Zero != intPtr7 && IntPtr.Zero != intPtr8)
		{
			float* ptr13 = (float*)intPtr5.ToPointer();
			float* ptr14 = (float*)intPtr6.ToPointer();
			float* ptr15 = (float*)intPtr7.ToPointer();
			float* ptr16 = (float*)intPtr8.ToPointer();
			for (int j = 0; j < num; j++)
			{
				rectI = new RectI();
				rectI.RectPoint.X = (int)ptr13[j];
				rectI.RectPoint.Y = (int)ptr14[j];
				rectI.RectWidth = (int)ptr15[j];
				rectI.RectHeight = (int)ptr16[j];
				list.Add(rectI);
			}
			propInfo.SetValue(stModuBase, list, null);
		}
	}

	private unsafe void SetEllipseResultData(PropertyInfo propInfo, ref int nIndexOffset, ref uint[] anResIndex, ref string[] astrResName, ref ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_EX[] astModuRes, ref CModuleResultBase stModuBase, bool bMultiRes = false)
	{
		string text = "";
		string text2 = "";
		string text3 = "";
		string text4 = "";
		string text5 = "";
		if (propInfo.Name == "OutputEllipse")
		{
			text = "OutputEllipseCenterX";
			text2 = "OutputEllipseCenterY";
			text3 = "OutputEllipseMajorRadius";
			text4 = "OutputEllipseMinorRadius";
			text5 = "OutputEllipseAngle";
		}
		else if (propInfo.Name == "OutputEllipseRing")
		{
			text = "OutputEllipseRingCenterX";
			text2 = "OutputEllipseRingCenterY";
			text3 = "OutputEllipseRingMajorRadius";
			text4 = "OutputEllipseRingMinorRadius";
			text5 = "OutputEllipseRingAngle";
		}
		IntPtr intPtr = IntPtr.Zero;
		IntPtr intPtr2 = IntPtr.Zero;
		IntPtr intPtr3 = IntPtr.Zero;
		IntPtr intPtr4 = IntPtr.Zero;
		IntPtr intPtr5 = IntPtr.Zero;
		int num = int.MaxValue;
		bool flag = false;
		uint num2 = 0u;
		uint num3 = 0u;
		int num4 = 0;
		while (num4 < anResIndex.Length)
		{
			flag = false;
			num3 = anResIndex[num4];
			if (2 == astModuRes[num3].nParamType && astModuRes[num3].nValueNum > 0)
			{
				if (IntPtr.Zero == intPtr && astrResName[num3] == text)
				{
					intPtr = astModuRes[num3].pFloatValue;
					num = Math.Min(num, astModuRes[num3].nValueNum);
					flag = true;
					num2++;
				}
				else if (IntPtr.Zero == intPtr2 && astrResName[num3] == text2)
				{
					intPtr2 = astModuRes[num3].pFloatValue;
					num = Math.Min(num, astModuRes[num3].nValueNum);
					flag = true;
					num2++;
				}
				else if (IntPtr.Zero == intPtr3 && astrResName[num3] == text3)
				{
					intPtr3 = astModuRes[num3].pFloatValue;
					num = Math.Min(num, astModuRes[num3].nValueNum);
					flag = true;
					num2++;
				}
				else if (IntPtr.Zero == intPtr4 && astrResName[num3] == text4)
				{
					intPtr4 = astModuRes[num3].pFloatValue;
					num = Math.Min(num, astModuRes[num3].nValueNum);
					flag = true;
					num2++;
				}
				else if (IntPtr.Zero == intPtr5 && astrResName[num3] == text5)
				{
					intPtr5 = astModuRes[num3].pFloatValue;
					num = Math.Min(num, astModuRes[num3].nValueNum);
					flag = true;
					num2++;
				}
			}
			if (flag)
			{
				if (num4 < nIndexOffset)
				{
					anResIndex[num4] = anResIndex[nIndexOffset];
					anResIndex[nIndexOffset] = num3;
					nIndexOffset--;
				}
				else if (num4 == nIndexOffset)
				{
					nIndexOffset--;
					num4++;
				}
				if (num2 >= 5)
				{
					break;
				}
			}
			else
			{
				num4++;
			}
		}
		if (IntPtr.Zero == intPtr || IntPtr.Zero == intPtr2 || IntPtr.Zero == intPtr3 || IntPtr.Zero == intPtr4 || IntPtr.Zero == intPtr5 || int.MaxValue == num)
		{
			return;
		}
		float* ptr = (float*)intPtr.ToPointer();
		float* ptr2 = (float*)intPtr2.ToPointer();
		float* ptr3 = (float*)intPtr3.ToPointer();
		float* ptr4 = (float*)intPtr4.ToPointer();
		float* ptr5 = (float*)intPtr5.ToPointer();
		Ellipse ellipse = new Ellipse();
		if (!bMultiRes)
		{
			ellipse.CenterPoint.X = *ptr;
			ellipse.CenterPoint.Y = *ptr2;
			ellipse.MajorRadius = *ptr3;
			ellipse.MinorRadius = *ptr4;
			ellipse.Angle = *ptr5;
			propInfo.SetValue(stModuBase, ellipse, null);
			return;
		}
		List<Ellipse> list = new List<Ellipse>(num);
		for (int i = 0; i < num; i++)
		{
			ellipse = new Ellipse();
			ellipse.CenterPoint.X = ptr[i];
			ellipse.CenterPoint.Y = ptr2[i];
			ellipse.MajorRadius = ptr3[i];
			ellipse.MinorRadius = ptr4[i];
			ellipse.Angle = ptr5[i];
			list.Add(ellipse);
		}
		propInfo.SetValue(stModuBase, list, null);
	}

	private void SetImageResultData3D(PropertyInfo propInfo, ref ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX stModuleResultex, ref CModuleResultBase stModuBase, bool isOptimizeImage = false)
	{
		ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX_Data iMVS_MODULE_RESULT_INFO_LIST_EX_Data = ServerSDKManager.serverSDKManager.mModuleManager.PreProData(stModuleResultex);
		string text = "";
		string strB = "";
		string text2 = "";
		string text3 = "";
		string text4 = "";
		string text5 = "";
		string text6 = "";
		string text7 = "";
		string text8 = "";
		string text9 = "";
		string text10 = "";
		string strB2 = "";
		string text11 = "";
		string text12 = "";
		string text13 = "";
		ImageBaseData3D_Base imageBaseData3D_Base = null;
		imageBaseData3D_Base = ((!isOptimizeImage) ? ((ImageBaseData3D_Base)new ImageBaseData3D()) : ((ImageBaseData3D_Base)new ImageBaseData3D_V2()));
		if (propInfo.Name == "ImageData3D")
		{
			text = "ImageMode";
			text5 = "Xoffset";
			text6 = "Yoffset";
			text7 = "Zoffset";
			text8 = "Xscale";
			text9 = "Yscale";
			text10 = "Zscale";
			strB2 = "IntensityImage";
			text11 = "IntensityWidth";
			text12 = "IntensityHeight";
			text13 = "IntensityPixelFormat";
			for (int i = 0; i < iMVS_MODULE_RESULT_INFO_LIST_EX_Data.nResultNum; i++)
			{
				int nParamType = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[i].nParamType;
				if (nParamType == 1 && iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[i].strParamName == text && iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[i].nValueNum > 0)
				{
					imageBaseData3D_Base.ImageMode = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[i].pIntValue[0];
					if (1 == imageBaseData3D_Base.ImageMode)
					{
						strB = "OriginImage";
						text2 = "OriginWidth";
						text3 = "OriginHeight";
						text4 = "OriginPixelFormat";
					}
					if (4 == imageBaseData3D_Base.ImageMode)
					{
						strB = "PointCloudImage";
						text2 = "PointCloudWidth";
						text3 = "PointCloudHeight";
						text4 = "PointCloudPixelFormat";
						strB2 = "PCIntensityImage";
						text11 = "PCIntensityWidth";
						text12 = "PCIntensityHeight";
						text13 = "PCIntensityPixelFormat";
					}
					if (7 == imageBaseData3D_Base.ImageMode)
					{
						strB = "RangeImage";
						text2 = "RangeWidth";
						text3 = "RangeHeight";
						text4 = "RangePixelFormat";
					}
				}
			}
		}
		if (propInfo.Name == "RangeImageDataOut")
		{
			imageBaseData3D_Base.ImageMode = 7;
			strB = "RangeImageOut";
			text2 = "RangeWidthOut";
			text3 = "RangeHeightOut";
			text4 = "RangePixelFormatOut";
			text5 = "XoffsetOut";
			text6 = "YoffsetOut";
			text7 = "ZoffsetOut";
			text8 = "XscaleOut";
			text9 = "YscaleOut";
			text10 = "ZscaleOut";
			strB2 = "IntensityImageOut";
			text11 = "IntensityWidthOut";
			text12 = "IntensityHeightOut";
			text13 = "IntensityPixelFormatOut";
		}
		if (7 == imageBaseData3D_Base.ImageMode)
		{
			for (int j = 0; j < iMVS_MODULE_RESULT_INFO_LIST_EX_Data.nResultNum; j++)
			{
				switch (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].nParamType)
				{
				case 1:
					if (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].strParamName == text2)
					{
						if (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].nValueNum > 0)
						{
							if (isOptimizeImage)
							{
								(imageBaseData3D_Base as ImageBaseData3D_V2).RangeImageData.Width = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].pIntValue[0];
							}
							else
							{
								(imageBaseData3D_Base as ImageBaseData3D).RangeImageData.Width = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].pIntValue[0];
							}
						}
					}
					else if (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].strParamName == text3)
					{
						if (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].nValueNum > 0)
						{
							if (isOptimizeImage)
							{
								(imageBaseData3D_Base as ImageBaseData3D_V2).RangeImageData.Height = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].pIntValue[0];
							}
							else
							{
								(imageBaseData3D_Base as ImageBaseData3D).RangeImageData.Height = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].pIntValue[0];
							}
						}
					}
					else if (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].strParamName == text4)
					{
						if (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].nValueNum > 0)
						{
							if (isOptimizeImage)
							{
								(imageBaseData3D_Base as ImageBaseData3D_V2).RangeImageData.Pixelformat = (VMPixelFormat)iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].pIntValue[0];
							}
							else
							{
								(imageBaseData3D_Base as ImageBaseData3D).RangeImageData.Pixelformat = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].pIntValue[0];
							}
						}
					}
					else if (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].strParamName == text5)
					{
						if (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].nValueNum > 0)
						{
							if (isOptimizeImage)
							{
								(imageBaseData3D_Base as ImageBaseData3D_V2).RangeImageData.Xoffset = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].pIntValue[0];
							}
							else
							{
								(imageBaseData3D_Base as ImageBaseData3D).RangeImageData.Xoffset = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].pIntValue[0];
							}
						}
					}
					else if (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].strParamName == text6)
					{
						if (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].nValueNum > 0)
						{
							if (isOptimizeImage)
							{
								(imageBaseData3D_Base as ImageBaseData3D_V2).RangeImageData.Yoffset = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].pIntValue[0];
							}
							else
							{
								(imageBaseData3D_Base as ImageBaseData3D).RangeImageData.Yoffset = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].pIntValue[0];
							}
						}
					}
					else if (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].strParamName == text7)
					{
						if (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].nValueNum > 0)
						{
							if (isOptimizeImage)
							{
								(imageBaseData3D_Base as ImageBaseData3D_V2).RangeImageData.Zoffset = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].pIntValue[0];
							}
							else
							{
								(imageBaseData3D_Base as ImageBaseData3D).RangeImageData.Zoffset = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].pIntValue[0];
							}
						}
					}
					else if (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].strParamName == text11)
					{
						if (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].nValueNum > 0)
						{
							if (isOptimizeImage)
							{
								(imageBaseData3D_Base as ImageBaseData3D_V2).RangeImageData.IntensityImageData.Width = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].pIntValue[0];
							}
							else
							{
								(imageBaseData3D_Base as ImageBaseData3D).RangeImageData.IntensityImageData.Width = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].pIntValue[0];
							}
						}
					}
					else if (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].strParamName == text12)
					{
						if (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].nValueNum > 0)
						{
							if (isOptimizeImage)
							{
								(imageBaseData3D_Base as ImageBaseData3D_V2).RangeImageData.IntensityImageData.Height = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].pIntValue[0];
							}
							else
							{
								(imageBaseData3D_Base as ImageBaseData3D).RangeImageData.IntensityImageData.Height = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].pIntValue[0];
							}
						}
					}
					else if (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].strParamName == text13 && iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].nValueNum > 0)
					{
						if (isOptimizeImage)
						{
							(imageBaseData3D_Base as ImageBaseData3D_V2).RangeImageData.IntensityImageData.Pixelformat = (VMPixelFormat)iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].pIntValue[0];
						}
						else
						{
							(imageBaseData3D_Base as ImageBaseData3D).RangeImageData.IntensityImageData.Pixelformat = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].pIntValue[0];
						}
					}
					break;
				case 2:
					if (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].strParamName == text8)
					{
						if (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].nValueNum > 0)
						{
							if (isOptimizeImage)
							{
								(imageBaseData3D_Base as ImageBaseData3D_V2).RangeImageData.Xscale = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].pFloatValue[0];
							}
							else
							{
								(imageBaseData3D_Base as ImageBaseData3D).RangeImageData.Xscale = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].pFloatValue[0];
							}
						}
					}
					else if (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].strParamName == text9)
					{
						if (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].nValueNum > 0)
						{
							if (isOptimizeImage)
							{
								(imageBaseData3D_Base as ImageBaseData3D_V2).RangeImageData.Yscale = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].pFloatValue[0];
							}
							else
							{
								(imageBaseData3D_Base as ImageBaseData3D).RangeImageData.Yscale = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].pFloatValue[0];
							}
						}
					}
					else if (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].strParamName == text10 && iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].nValueNum > 0)
					{
						if (isOptimizeImage)
						{
							(imageBaseData3D_Base as ImageBaseData3D_V2).RangeImageData.Zscale = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].pFloatValue[0];
						}
						else
						{
							(imageBaseData3D_Base as ImageBaseData3D).RangeImageData.Zscale = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].pFloatValue[0];
						}
					}
					break;
				case 4:
					if (string.Compare(iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].strParamName, strB) == 0)
					{
						if (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].nValueNum > 0)
						{
							if (isOptimizeImage)
							{
								(imageBaseData3D_Base as ImageBaseData3D_V2).RangeImageData.ImageData = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].pImageValue[0].GetData(out var lenth);
								(imageBaseData3D_Base as ImageBaseData3D_V2).RangeImageData.DataLen = (uint)lenth;
							}
							else
							{
								(imageBaseData3D_Base as ImageBaseData3D).RangeImageData.ImageData = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].pImageValue[0].GetData();
								(imageBaseData3D_Base as ImageBaseData3D).RangeImageData.DataLen = (uint)iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].pImageValue[0].GetData().Length;
							}
						}
					}
					else if (string.Compare(iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].strParamName, strB2) == 0 && iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].nValueNum > 0)
					{
						if (isOptimizeImage)
						{
							(imageBaseData3D_Base as ImageBaseData3D_V2).RangeImageData.IntensityImageData.ImageData = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].pImageValue[0].GetData(out var lenth2);
							(imageBaseData3D_Base as ImageBaseData3D_V2).RangeImageData.IntensityImageData.DataLen = (uint)lenth2;
						}
						else
						{
							(imageBaseData3D_Base as ImageBaseData3D).RangeImageData.IntensityImageData.ImageData = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].pImageValue[0].GetData();
							(imageBaseData3D_Base as ImageBaseData3D).RangeImageData.IntensityImageData.DataLen = (uint)iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[j].pImageValue[0].GetData().Length;
						}
					}
					break;
				}
			}
		}
		propInfo.SetValue(stModuBase, imageBaseData3D_Base, null);
	}
}
