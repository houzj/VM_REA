using System.ComponentModel;
using Hik.Xml;
using VMControls.RenderInterface;

namespace Apps.Data.ShapeDefine;

public class Shape : BaseNode, INotifyPropertyChanged, IShape, IShapeBase, IMatrixEnable
{
	public float CenterX;

	public float CenterY;

	public double Opacity;

	public string Color;

	public string FillColor;

	public double StrokeThickness = 1.0;

	public bool MatrixConvertEnable;

	public string Tips;

	public string HighLightIndex = "";

	public float CenterXA
	{
		get
		{
			return CenterX;
		}
		set
		{
			if (CenterX != value)
			{
				CenterX = value;
				OnPropertyChanged("CenterXA");
			}
		}
	}

	public float CenterYA
	{
		get
		{
			return CenterY;
		}
		set
		{
			if (CenterY != value)
			{
				CenterY = value;
				OnPropertyChanged("CenterYA");
			}
		}
	}

	public double OpacityA
	{
		get
		{
			return Opacity;
		}
		set
		{
			if (Opacity != value)
			{
				Opacity = value;
				OnPropertyChanged("OpacityA");
			}
		}
	}

	public string ColorA
	{
		get
		{
			return Color;
		}
		set
		{
			if (Color != value)
			{
				Color = value;
				OnPropertyChanged("ColorA");
			}
		}
	}

	public string FillColorA
	{
		get
		{
			return FillColor;
		}
		set
		{
			if (FillColor != value)
			{
				FillColor = value;
				OnPropertyChanged("FillColorA");
			}
		}
	}

	public double StrokeThicknessA
	{
		get
		{
			return StrokeThickness;
		}
		set
		{
			if (StrokeThickness != value)
			{
				StrokeThickness = value;
				OnPropertyChanged("StrokeThicknessA");
			}
		}
	}

	string IShapeBase.Stroke => Color;

	double IShapeBase.Opacity => Opacity;

	string IShape.Fill => FillColor;

	double IShape.StrokeThickness => StrokeThickness;

	string IShape.ToolTip => Tips;

	bool IMatrixEnable.MatrixConvertEnable => MatrixConvertEnable;

	public event PropertyChangedEventHandler PropertyChanged;

	public void OnPropertyChanged(string propertyName)
	{
		if (PropertyChanged != null)
		{
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
