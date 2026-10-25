using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace VM.PlatformSDKCS;

public class UTF8Marshaler : ICustomMarshaler
{
	private static UTF8Marshaler instance = new UTF8Marshaler();

	public void CleanUpManagedData(object managedObj)
	{
	}

	public void CleanUpNativeData(IntPtr pNativeData)
	{
		Marshal.FreeHGlobal(pNativeData);
	}

	public int GetNativeDataSize()
	{
		return -1;
	}

	public IntPtr MarshalManagedToNative(object managedObj)
	{
		if (managedObj == null)
		{
			return IntPtr.Zero;
		}
		if (!(managedObj is string))
		{
			throw new InvalidOperationException();
		}
		byte[] bytes = Encoding.UTF8.GetBytes(managedObj as string);
		IntPtr intPtr = Marshal.AllocHGlobal(bytes.Length + 1);
		Marshal.Copy(bytes, 0, intPtr, bytes.Length);
		Marshal.WriteByte(intPtr, bytes.Length, 0);
		return intPtr;
	}

	public object MarshalNativeToManaged(IntPtr pNativeData)
	{
		if (pNativeData == IntPtr.Zero)
		{
			return null;
		}
		List<byte> list = new List<byte>();
		int num = 0;
		while (true)
		{
			byte b = Marshal.ReadByte(pNativeData, num);
			if (b == 0)
			{
				break;
			}
			list.Add(b);
			num++;
		}
		return Encoding.UTF8.GetString(list.ToArray(), 0, list.Count);
	}

	public static ICustomMarshaler GetInstance(string cookie)
	{
		return instance;
	}
}
