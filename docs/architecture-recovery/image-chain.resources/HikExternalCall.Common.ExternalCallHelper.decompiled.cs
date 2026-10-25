using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace HikExternalCall.Common;

public static class ExternalCallHelper
{
	public static IntPtr UnsafeBytes2IntPtr(byte[] bytes)
	{
		return Marshal.UnsafeAddrOfPinnedArrayElement((Array)bytes, 0);
	}

	public static IntPtr Bytes2IntPtr(byte[] bytes, out GCHandle hObject)
	{
		hObject = GCHandle.Alloc(bytes, GCHandleType.Pinned);
		return hObject.AddrOfPinnedObject();
	}

	public static void FreeGCHandle(GCHandle hObject)
	{
		if (hObject.IsAllocated)
		{
			hObject.Free();
		}
	}

	public static IntPtr ArrayToIntptr(byte[] source)
	{
		if (source == null)
		{
			return IntPtr.Zero;
		}
		IntPtr intPtr = Marshal.AllocHGlobal(source.Length);
		Marshal.Copy(source, 0, intPtr, source.Length);
		return intPtr;
	}

	public static IntPtr Object2IntPtr(object obj)
	{
		GCHandle gCHandle = GCHandle.Alloc(obj, GCHandleType.Pinned);
		IntPtr result = gCHandle.AddrOfPinnedObject();
		if (gCHandle.IsAllocated)
		{
			gCHandle.Free();
		}
		return result;
	}

	public static byte[] IntPtr2Bytes(IntPtr ptr, int size)
	{
		if (IntPtr.Zero == ptr || size == 0)
		{
			return null;
		}
		byte[] array = new byte[size];
		Marshal.Copy(ptr, array, 0, size);
		return array;
	}

	public static IntPtr Bytes2IntPtr(byte[] bytes)
	{
		IntPtr intPtr = Marshal.AllocHGlobal(bytes.Length);
		Marshal.Copy(bytes, 0, intPtr, bytes.Length);
		return intPtr;
	}

	public static void FreeBytesIntPtr(IntPtr ptr)
	{
		Marshal.FreeHGlobal(ptr);
	}

	public static T[] IntPtr2Ts<T>(IntPtr ptr, int size)
	{
		if (size == 0 || ptr == IntPtr.Zero)
		{
			return null;
		}
		T[] array = new T[size];
		try
		{
			if (array is int[])
			{
				int[] destination = array as int[];
				Marshal.Copy(ptr, destination, 0, size);
			}
			else if (array is byte[])
			{
				byte[] destination2 = array as byte[];
				Marshal.Copy(ptr, destination2, 0, size);
			}
			else if (array is float[])
			{
				float[] destination3 = array as float[];
				Marshal.Copy(ptr, destination3, 0, size);
			}
			else
			{
				if (!(array is double[]))
				{
					return null;
				}
				double[] destination4 = array as double[];
				Marshal.Copy(ptr, destination4, 0, size);
			}
			return array;
		}
		catch
		{
			return array;
		}
	}

	public static void PtrToStructure<T>(IntPtr ptr, ref T structure)
	{
		try
		{
			structure = (T)Marshal.PtrToStructure(ptr, typeof(T));
		}
		catch (Exception ex)
		{
			throw ex;
		}
		finally
		{
			Marshal.FreeHGlobal(ptr);
		}
	}

	public static void PtrToStructureWithoutFree<T>(IntPtr ptr, ref T structure)
	{
		structure = (T)Marshal.PtrToStructure(ptr, typeof(T));
	}

	public static void StructureToPtr(object structure, ref IntPtr ptr)
	{
		ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(structure));
		Marshal.StructureToPtr(structure, ptr, fDeleteOld: true);
	}

	public static void FreeCoTaskMemIntPtr(IntPtr ptr)
	{
		if (ptr != IntPtr.Zero)
		{
			Marshal.FreeCoTaskMem(ptr);
		}
	}

	public static IntPtr StructureToPtr(object structure)
	{
		IntPtr intPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(structure));
		Marshal.StructureToPtr(structure, intPtr, fDeleteOld: true);
		return intPtr;
	}

	public static byte[] StructToBytes(object structObj, int size)
	{
		IntPtr intPtr = Marshal.AllocHGlobal(size);
		try
		{
			Marshal.StructureToPtr(structObj, intPtr, fDeleteOld: false);
			byte[] array = new byte[size];
			Marshal.Copy(intPtr, array, 0, size);
			return array;
		}
		catch (Exception ex)
		{
			throw new Exception("Error in StructToBytes ! " + ex.Message);
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	public static byte[] StructToBytes(object structObj)
	{
		int num = Marshal.SizeOf(structObj);
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		try
		{
			Marshal.StructureToPtr(structObj, intPtr, fDeleteOld: false);
			byte[] array = new byte[num];
			Marshal.Copy(intPtr, array, 0, num);
			return array;
		}
		catch (Exception ex)
		{
			throw new Exception("Error in StructToBytes ! " + ex.Message);
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	public static byte[] StructsToBytes<T>(IList<T> structObjs)
	{
		if (structObjs.Count == 0)
		{
			return new byte[0];
		}
		int count = structObjs.Count;
		int num = Marshal.SizeOf((object)structObjs[0]);
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		byte[] array = new byte[num * count];
		try
		{
			for (int i = 0; i < count; i++)
			{
				Marshal.StructureToPtr((object)structObjs[i], intPtr, false);
				Marshal.Copy(intPtr, array, i * num, num);
			}
			return array;
		}
		catch (Exception ex)
		{
			throw new Exception("Error in StructToBytes ! " + ex.Message);
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	public static T BytesToStruct<T>(byte[] bytes, int startIndex, int length)
	{
		if (bytes == null)
		{
			return default;
		}
		if (bytes.Length == 0)
		{
			return default;
		}
		IntPtr intPtr = Marshal.AllocHGlobal(length);
		try
		{
			Marshal.Copy(bytes, startIndex, intPtr, length);
			return (T)Marshal.PtrToStructure(intPtr, typeof(T));
		}
		catch (Exception ex)
		{
			throw new Exception("Error in BytesToStruct ! " + ex.Message);
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	public static List<T> BytesToStructs<T>(byte[] bytes) where T : struct
	{
		if (bytes == null)
		{
			return null;
		}
		if (bytes.Length == 0)
		{
			return null;
		}
		IntPtr intPtr = Marshal.AllocHGlobal(bytes.Length);
		List<T> list = new List<T>();
		int num = Marshal.SizeOf((object)new T());
		int num2 = bytes.Length / num;
		try
		{
			for (int i = 0; i < num2; i++)
			{
				Marshal.Copy(bytes, i * num, intPtr, num);
				list.Add((T)Marshal.PtrToStructure(intPtr, typeof(T)));
			}
			return list;
		}
		catch (Exception ex)
		{
			throw new Exception("Error in BytesToStruct ! " + ex.Message);
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	public static T[] PtrToStructurs<T>(IntPtr pt, int lenth)
	{
		T[] array = new T[lenth];
		int num = Marshal.SizeOf(typeof(T));
		for (int i = 0; i < lenth; i++)
		{
			IntPtr ptr = pt + i * num;
			array[i] = (T)Marshal.PtrToStructure(ptr, typeof(T));
		}
		return array;
	}

	public static IntPtr StringToPtr(string str)
	{
		return Marshal.StringToHGlobalAnsi(str);
	}

	public static T[] IntPtr2Structures<T>(IntPtr pt, int lenth)
	{
		try
		{
			T[] array = new T[lenth];
			int num = Marshal.SizeOf(typeof(T));
			for (int i = 0; i < lenth; i++)
			{
				IntPtr zero = IntPtr.Zero;
				zero = ((8 != IntPtr.Size) ? ((IntPtr)((uint)(int)pt + i * num)) : ((IntPtr)((long)pt + i * num)));
				array[i] = (T)Marshal.PtrToStructure(zero, typeof(T));
			}
			return array;
		}
		catch (Exception ex)
		{
			throw new Exception("Error in IntPtr2Structures: " + ex.Message);
		}
	}

	public static T[] IntPtrs2Structures<T>(List<IntPtr> pt, List<int> lenth)
	{
		try
		{
			T[] array = new T[lenth.Sum()];
			int num = Marshal.SizeOf(typeof(T));
			int num2 = 0;
			for (int i = 0; i < lenth.Count; i++)
			{
				for (int j = 0; j < lenth[i]; j++)
				{
					IntPtr zero = IntPtr.Zero;
					zero = ((8 != IntPtr.Size) ? ((IntPtr)((uint)(int)pt[i] + j * num)) : ((IntPtr)((long)pt[i] + j * num)));
					array[num2] = (T)Marshal.PtrToStructure(zero, typeof(T));
					num2++;
				}
			}
			return array;
		}
		catch (Exception ex)
		{
			throw new Exception("Error in IntPtr2Structures: " + ex.Message);
		}
	}

	public static T DeepCopyByReflection<T>(T obj)
	{
		if (obj is string || obj.GetType().IsValueType)
		{
			return obj;
		}
		object obj2 = Activator.CreateInstance(obj.GetType());
		FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (FieldInfo fieldInfo in fields)
		{
			try
			{
				fieldInfo.SetValue(obj2, DeepCopyByReflection(fieldInfo.GetValue(obj)));
			}
			catch (Exception ex)
			{
				throw new Exception("Error in DeepCopyByReflection: " + ex.Message);
			}
		}
		return (T)obj2;
	}

	public static T DeepCopy<T>(T obj)
	{
		object obj2 = null;
		try
		{
			using MemoryStream memoryStream = new MemoryStream();
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, obj);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			obj2 = binaryFormatter.Deserialize(memoryStream);
			memoryStream.Close();
		}
		catch (Exception ex)
		{
			throw new Exception("Error in DeepCopy: " + ex.Message);
		}
		return (T)obj2;
	}
}
