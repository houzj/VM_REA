using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace VM.PlatformSDKCS;

public class StringUtil
{
	public static byte[] UTF8GetBytes(string str)
	{
		return Encoding.UTF8.GetBytes(str);
	}

	public static byte[] UTF8GetBytesPadZero(string str)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(str);
		byte[] array = new byte[bytes.Length + 1];
		Buffer.BlockCopy(bytes, 0, array, 0, bytes.Length);
		array[array.Length - 1] = 0;
		return array;
	}

	public static byte[] UTF8GetBytesPadZero(byte[] tmpbytes)
	{
		if (tmpbytes.Length != 0)
		{
			byte[] array = new byte[tmpbytes.Length + 1];
			Buffer.BlockCopy(tmpbytes, 0, array, 0, tmpbytes.Length);
			array[array.Length - 1] = 0;
			return array;
		}
		return tmpbytes;
	}

	/// <summary>
	/// convert string to fixed length utf8 byte[].
	/// </summary>
	/// <param name="str"></param>
	/// <param name="len"></param>
	/// <returns></returns>
	public static byte[] UTF8GetFixLenBytes(string str, int len)
	{
		if (str == null)
		{
			return null;
		}
		byte[] array = new byte[len];
		byte[] bytes = Encoding.UTF8.GetBytes(str);
		Buffer.BlockCopy(bytes, 0, array, 0, Math.Min(bytes.Length, len));
		return array;
	}

	/// <summary>
	/// convert fixed length utf8 byte[] to string.
	/// </summary>
	/// <param name="str"></param>
	/// <param name="len"></param>
	/// <returns></returns>
	public static string UTF8GetString(byte[] bt)
	{
		return Encoding.UTF8.GetString(bt).TrimEnd(default(char));
	}

	public static int GetByteCountWithZero(string str)
	{
		return (!string.IsNullOrEmpty(str)) ? (Encoding.UTF8.GetByteCount(str) + 1) : 0;
	}

	public static string ByteGetHexString(byte[] bt)
	{
		return BitConverter.ToString(bt).Replace("-", "");
	}

	public static bool CheckByteString(string strByte, ref byte[] bytes)
	{
		if (string.IsNullOrEmpty(strByte))
		{
			return false;
		}
		if (strByte.Length % 2 != 0)
		{
			return false;
		}
		if (!new Regex("[A-Fa-f0-9]+$").IsMatch(strByte))
		{
			return false;
		}
		byte[] array = new byte[strByte.Length / 2];
		bytes = (from x in Enumerable.Range(0, strByte.Length)
			where x % 2 == 0
			select Convert.ToByte(strByte.Substring(x, 2), 16)).ToArray();
		return true;
	}

	public static string RepairIoName(string varName)
	{
		if (varName.First() != '%' || varName.Last() != '%')
		{
			varName = "%" + varName + "%";
		}
		return varName;
	}
}
