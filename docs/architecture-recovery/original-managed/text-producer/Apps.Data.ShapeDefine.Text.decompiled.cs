using System.Windows;
using VMControls.RenderInterface;

namespace Apps.Data.ShapeDefine;

public class Text : Shape, IText, IShapeBase, ISize
{
	public string Content;

	public float PositionX;

	public float PositionY;

	public int FontSize;

	public float Width;

	public float Height;

	public string ContentA
	{
		get
		{
			return Content;
		}
		set
		{
			if (Content != value)
			{
				Content = value;
				OnPropertyChanged("ContentA");
			}
		}
	}

	public float PositionXA
	{
		get
		{
			return PositionX;
		}
		set
		{
			if (PositionX != value)
			{
				PositionX = value;
				OnPropertyChanged("PositionXA");
			}
		}
	}

	public float PositionYA
	{
		get
		{
			return PositionY;
		}
		set
		{
			if (PositionY != value)
			{
				PositionY = value;
				OnPropertyChanged("PositionYA");
			}
		}
	}

	public int FontSizeA
	{
		get
		{
			return FontSize;
		}
		set
		{
			if (FontSize != value)
			{
				FontSize = value;
				OnPropertyChanged("FontSizeA");
			}
		}
	}

	public float WidthA
	{
		get
		{
			return Width;
		}
		set
		{
			if (Width != value)
			{
				Width = value;
				OnPropertyChanged("WidthA");
			}
		}
	}

	public float HeightA
	{
		get
		{
			return Height;
		}
		set
		{
			if (Height != value)
			{
				Height = value;
				OnPropertyChanged("HeightA");
			}
		}
	}

	string IText.Content => Content;

	System.Windows.Point IText.Position => new System.Windows.Point(PositionX, PositionY);

	int IText.FontSize => FontSize;

	double ISize.Width => WidthA;

	double ISize.Height => HeightA;
}
