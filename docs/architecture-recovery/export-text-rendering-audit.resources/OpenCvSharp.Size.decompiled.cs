using System;

namespace OpenCvSharp;

[Serializable]
public struct Size : IEquatable<Size>
{
	public int Width;

	public int Height;

	public static Size Zero => default;

	public Size(int width, int height)
	{
		Width = width;
		Height = height;
	}

	public Size(double width, double height)
	{
		Width = (int)width;
		Height = (int)height;
	}

	public bool Equals(Size obj)
	{
		if (Width == obj.Width)
		{
			return Height == obj.Height;
		}
		return false;
	}

	public static bool operator ==(Size lhs, Size rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(Size lhs, Size rhs)
	{
		return !lhs.Equals(rhs);
	}

	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		return Width.GetHashCode() ^ Height.GetHashCode();
	}

	public override string ToString()
	{
		return $"(width:{Width} height:{Height})";
	}
}
