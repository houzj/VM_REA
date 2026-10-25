using System;
using System.Collections;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using Apps.Log;

namespace VM.PlatformSDKCS;

public static class MemoryHelper
{
	/// <summary>
	///
	/// </summary>
	public enum MapAccess
	{
		FileMapCopy = 1,
		FileMapWrite = 2,
		FileMapRead = 4,
		FileMapAllAccess = 31
	}

	public static long imageBufferOffset;

	private static IntPtr largeMemoryIntPtr;

	private static int MaxMemoryLenth;

	private static int offset;

	/// <summary>
	/// 创建文件
	/// </summary>
	/// <param name="lpFileName"></param>
	/// <param name="dwDesiredAccess"></param>
	/// <param name="dwShareMode"></param>
	/// <param name="lpSecurityAttributes"></param>
	/// <param name="dwCreationDisposition"></param>
	/// <param name="dwFlagsAndAttributes"></param>
	/// <param name="hTemplateFile"></param>
	/// <returns></returns>
	public static IntPtr CreateFile(string lpFileName, int dwDesiredAccess, int dwShareMode, IntPtr lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile)
	{
		return MemoryNativeMethods.CreateFile(lpFileName, dwDesiredAccess, dwShareMode, lpSecurityAttributes, dwCreationDisposition, dwFlagsAndAttributes, hTemplateFile);
	}

	/// <summary>
	/// 打开文件映射
	/// </summary>
	/// <param name="hFile"></param>
	/// <param name="lpAttributes"></param>
	/// <param name="flProtect"></param>
	/// <param name="dwMaximumSizeLow"></param>
	/// <param name="dwMaximumSizeHigh"></param>
	/// <param name="lpName"></param>
	/// <returns></returns>
	public static IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpAttributes, long flProtect, long dwMaximumSizeLow, long dwMaximumSizeHigh, string lpName)
	{
		return MemoryNativeMethods.CreateFileMapping(hFile, lpAttributes, flProtect, dwMaximumSizeLow, dwMaximumSizeHigh, lpName);
	}

	/// <summary>
	/// Flush文件的View
	/// </summary>
	/// <param name="lpBaseAddress"></param>
	/// <param name="dwNumBytesToFlush"></param>
	/// <returns></returns>
	public static bool FlushViewOfFile(IntPtr lpBaseAddress, IntPtr dwNumBytesToFlush)
	{
		return MemoryNativeMethods.FlushViewOfFile(lpBaseAddress, dwNumBytesToFlush);
	}

	/// <summary>
	/// 获取文件的MapView
	/// </summary>
	/// <param name="hFileMappingObject"></param>
	/// <param name="dwDesiredAccess"></param>
	/// <param name="dwFileOffsetHigh"></param>
	/// <param name="dwFileOffsetLow"></param>
	/// <param name="dwNumBytesToMap"></param>
	/// <returns></returns>
	public static IntPtr MapViewOfFile(IntPtr hFileMappingObject, long dwDesiredAccess, long dwFileOffsetHigh, long dwFileOffsetLow, IntPtr dwNumBytesToMap)
	{
		return MemoryNativeMethods.MapViewOfFile(hFileMappingObject, dwDesiredAccess, dwFileOffsetHigh, dwFileOffsetLow, dwNumBytesToMap);
	}

	/// <summary>
	/// 打开文件映射
	/// </summary>
	/// <param name="dwDesiredAccess"></param>
	/// <param name="bInheritHandle"></param>
	/// <param name="lpName"></param>
	/// <returns></returns>
	public static IntPtr OpenFileMapping(long dwDesiredAccess, bool bInheritHandle, string lpName)
	{
		return MemoryNativeMethods.OpenFileMapping(dwDesiredAccess, bInheritHandle, lpName);
	}

	/// <summary>
	/// 解映射
	/// </summary>
	/// <param name="lpBaseAddress"></param>
	/// <returns></returns>
	public static bool UnmapViewOfFile(IntPtr lpBaseAddress)
	{
		return MemoryNativeMethods.UnmapViewOfFile(lpBaseAddress);
	}

	/// <summary>
	/// 关闭句柄
	/// </summary>
	/// <param name="handle"></param>
	/// <returns></returns>
	public static bool CloseHandle(IntPtr handle)
	{
		return MemoryNativeMethods.CloseHandle(handle);
	}

	/// <summary>  
	/// 读共享内存  
	/// </summary>
	/// <param name="imageDataSize"></param>
	/// <param name="fileName">文件映射对象的名称</param>
	/// <param name="pBuffer"></param>
	/// <param name="startOffset"></param>
	/// <returns>返回读到的映射对象</returns>  
	public static object ReadFromMemory(long imageDataSize, string fileName, byte[] pBuffer, int startOffset)
	{
		long value = imageDataSize + imageBufferOffset - startOffset;
		IntPtr intPtr = OpenFileMapping(4L, bInheritHandle: false, fileName);
		if (intPtr == IntPtr.Zero)
		{
			return null;
		}
		IntPtr intPtr2 = MapViewOfFile(intPtr, 4L, 0L, 0L, new IntPtr(value));
		if (intPtr2 == IntPtr.Zero)
		{
			CloseHandle(intPtr);
			return null;
		}
		Marshal.Copy(intPtr2 + startOffset, pBuffer, 0, (int)imageDataSize);
		if (intPtr2 != IntPtr.Zero)
		{
			UnmapViewOfFile(intPtr2);
		}
		if (intPtr != IntPtr.Zero)
		{
			CloseHandle(intPtr);
		}
		return pBuffer;
	}

	/// <summary>  
	/// 读共享内存  
	/// </summary>
	/// <param name="imageDataSize"></param>
	/// <param name="fileName">文件映射对象的名称</param>
	/// <param name="pBuffer"></param>
	/// <returns>返回读到的映射对象</returns>  
	public static object ReadFromMemory(long imageDataSize, string fileName, byte[] pBuffer)
	{
		return ReadFromMemory(imageDataSize, fileName, pBuffer, 0);
	}

	static MemoryHelper()
	{
		imageBufferOffset = 152L;
		MaxMemoryLenth = 268435455;
		largeMemoryIntPtr = Marshal.AllocHGlobal(MaxMemoryLenth);
	}

	private static IntPtr GetBytes(int lenth, out int oriOffset)
	{
		if (offset + lenth > MaxMemoryLenth)
		{
			offset = 0;
		}
		if (lenth > MaxMemoryLenth)
		{
			Marshal.FreeHGlobal(largeMemoryIntPtr);
			int num = Math.Min(lenth + 268435455, int.MaxValue);
			largeMemoryIntPtr = Marshal.AllocHGlobal(num);
			MaxMemoryLenth = num;
		}
		oriOffset = offset;
		IntPtr result = largeMemoryIntPtr + offset;
		offset += lenth;
		return result;
	}

	[DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "memcpy")]
	public static extern void MemCopy(IntPtr dest, IntPtr src, int count);

	[HandleProcessCorruptedStateExceptions]
	internal static IntPtr VMReadFromMemoryPtr(IntPtr memoryPtr, int dataLenth, ref bool isExcOccur, bool bUseMemPool = true)
	{
		IntPtr zero = IntPtr.Zero;
		isExcOccur = false;
		try
		{
			if (bUseMemPool)
			{
				zero = GetBytes(dataLenth, out var _);
				MemCopy(zero, memoryPtr, dataLenth);
			}
			else
			{
				zero = memoryPtr;
			}
		}
		catch (AccessViolationException)
		{
			LogHelper.Error("AccessViolationException Error");
			zero = IntPtr.Zero;
			isExcOccur = true;
		}
		return zero;
	}

	/// <summary>
	/// 自定义读取共享内存,前八位是共享内存长度
	/// </summary>
	/// <param name="fileName"></param>
	/// <param name="startOffset"></param>
	/// <returns></returns>
	public static IntPtr VMReadFromMemory(string fileName, int startOffset, out int lenth, ref byte[] byteData, bool bUseMemPool = true)
	{
		byte[] array = new byte[16];
		long value = array.Length;
		IntPtr intPtr = OpenFileMapping(4L, bInheritHandle: false, fileName);
		lenth = 0;
		if (intPtr == IntPtr.Zero)
		{
			return IntPtr.Zero;
		}
		IntPtr intPtr2 = MapViewOfFile(intPtr, 4L, 0L, 0L, new IntPtr(value));
		if (intPtr2 == IntPtr.Zero)
		{
			CloseHandle(intPtr);
			return IntPtr.Zero;
		}
		Marshal.Copy(intPtr2, array, 0, array.Length);
		UnmapViewOfFile(intPtr2);
		if (array[8] != 0)
		{
			CloseHandle(intPtr);
			return IntPtr.Zero;
		}
		long num = BitConverter.ToInt64(array, 0);
		startOffset = BitConverter.ToInt16(array, 14);
		if (num < 0)
		{
			CloseHandle(intPtr);
			return IntPtr.Zero;
		}
		lenth = (int)num;
		value = num + startOffset;
		intPtr2 = MapViewOfFile(intPtr, 4L, 0L, 0L, new IntPtr(value));
		if (intPtr2 == IntPtr.Zero)
		{
			CloseHandle(intPtr);
			return IntPtr.Zero;
		}
		IntPtr zero = IntPtr.Zero;
		if (bUseMemPool)
		{
			zero = GetBytes((int)num, out var _);
			MemCopy(zero, intPtr2 + startOffset, (int)num);
			byteData = null;
		}
		else
		{
			byteData = new byte[num];
			Marshal.Copy(intPtr2 + startOffset, byteData, 0, (int)num);
			zero = IntPtr.Zero;
		}
		if (intPtr2 != IntPtr.Zero)
		{
			UnmapViewOfFile(intPtr2);
		}
		if (intPtr != IntPtr.Zero)
		{
			CloseHandle(intPtr);
		}
		return zero;
	}

	/// <summary>
	/// 按照VM格式解析获取共享内存内数据指针
	/// </summary>
	/// <param name="memAddress">共享内存地址</param>
	/// <param name="openHandle">资源句柄 保证指针生命周期 外部释放</param>
	/// <param name="mapHandle">资源句柄 保证指针生命周期 外部释放</param>
	/// <returns>数据指针</returns>
	public static IntPtr VMGetMemDataPtr(string memAddress, ref IntPtr openHandle, ref IntPtr mapHandle, ref long memDataLen, ref uint executeCount)
	{
		IntPtr intPtr = OpenFileMapping(4L, bInheritHandle: false, memAddress);
		if (IntPtr.Zero == intPtr)
		{
			return IntPtr.Zero;
		}
		long num = 128L;
		byte[] array = new byte[num];
		IntPtr intPtr2 = MapViewOfFile(intPtr, 4L, 0L, 0L, new IntPtr(num));
		if (IntPtr.Zero == intPtr2)
		{
			CloseHandle(intPtr);
			return IntPtr.Zero;
		}
		Marshal.Copy(intPtr2, array, 0, (int)num);
		UnmapViewOfFile(intPtr2);
		if (array[8] != 0)
		{
			CloseHandle(intPtr);
			return IntPtr.Zero;
		}
		long num2 = BitConverter.ToInt64(array, 0);
		short num3 = BitConverter.ToInt16(array, 14);
		uint num4 = BitConverter.ToUInt32(array, 80);
		if (num2 < 0)
		{
			CloseHandle(intPtr);
			return IntPtr.Zero;
		}
		intPtr2 = MapViewOfFile(intPtr, 4L, 0L, 0L, new IntPtr(num2 + num3));
		if (IntPtr.Zero == intPtr2)
		{
			CloseHandle(intPtr);
			return IntPtr.Zero;
		}
		executeCount = num4;
		memDataLen = num2;
		mapHandle = intPtr2;
		openHandle = intPtr;
		return intPtr2 + num3;
	}

	/// <summary>
	/// 深拷贝方法
	/// </summary>
	/// <param name="obj">拷贝对象</param>
	/// <returns></returns>
	public static object DeepCopyObject(object obj)
	{
		if (obj != null)
		{
			object obj2 = Activator.CreateInstance(obj.GetType());
			FieldInfo[] fields = obj.GetType().GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				if (fieldInfo.FieldType.GetInterface("IList", ignoreCase: false) == null)
				{
					fieldInfo.SetValue(obj2, fieldInfo.GetValue(obj));
					continue;
				}
				IList list = (IList)fieldInfo.GetValue(obj2);
				if (list == null)
				{
					continue;
				}
				foreach (object item in (IList)fieldInfo.GetValue(obj))
				{
					list.Add(DeepCopyObject(item));
				}
			}
			return obj2;
		}
		return null;
	}
}
