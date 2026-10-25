using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Apps.Localization;
using HikExternalCall.Common;
using OpenCvSharp;
using VM.Utility;
using VMControls.RenderInterface;

namespace Apps.Data.Common;

public static class ImageSaveOpenCvHelper
{
	public struct IMVS_PATMATCH_POINT_INFO
	{
		public float MatchOutlineX;

		public float MatchOutlineY;

		public float MatchOutlineScore;

		public int MatchOutlineIndex;
	}

	public static int SaveImageCompressionRatio;

	private static readonly TaskFactory factory;

	public static event EventHandler<ErrorEventArgs> ErrorEvent;

	static ImageSaveOpenCvHelper()
	{
		SaveImageCompressionRatio = 70;
		factory = null;
		factory = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(1));
	}

	public static void SaveRawImage(string saveName, IImageData imageData)
	{
		factory.StartNew(SaveRawImage, new { saveName, imageData }, TaskCreationOptions.PreferFairness);
	}

	public static void SaveRawImageSyn(string saveName, IImageData imageData, int jpgQuality = 0)
	{
		SaveRawImage(new { saveName, imageData, jpgQuality });
	}

	private static void SaveRawImage(dynamic o)
	{
		string text = o.saveName;
		IImageData imageData = o.imageData;
		int num = o.jpgQuality;
		try
		{
			CheckData(imageData, text);
			Mat val = bytesToMat(imageData, isRender: false);
			byte[] array = null;
			string text2 = Path.GetExtension(text).ToLower();
			if ((text2.Contains("jpg") || text2.Contains("jpeg")) && num > 0 && num <= 100)
			{
				int[] array2 = new int[3] { 1, num, 0 };
				array = val.ImEncode(text2, array2);
			}
			else
			{
				array = val.ImEncode(text2, (int[])null);
			}
			WriteByteToFile(array, text);
			((DisposableObject)val).Dispose();
		}
		catch (Exception ex)
		{
			ErrorEvent?.Invoke(null, new ErrorEventArgs(ex));
			throw ex;
		}
	}

	public static void SaveRoiImage(string saveName, IImageData imageData, List<object> shapesSource, Image controlImage)
	{
		factory.StartNew(SaveRoiImage, new { saveName, imageData, shapesSource, controlImage }, TaskCreationOptions.PreferFairness);
	}

	public static void SaveRoiImageSyn(string saveName, IImageData imageData, List<object> shapesSource, byte[] controlImage, int jpgQuality = 0)
	{
		SaveRoiImage(new { saveName, imageData, shapesSource, controlImage, jpgQuality });
	}

	private static void SaveRoiImage(dynamic o)
	{
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Expected Obj, but got Unknown
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Expected Obj, but got Unknown
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		string text = o.saveName;
		IImageData imageData = o.imageData;
		byte[] array = o.controlImage;
		List<object> list = o.shapesSource;
		int num = o.jpgQuality;
		Mat val = null;
		Mat val2 = null;
		Mat val3 = new Mat();
		Mat val4 = new Mat();
		try
		{
			CheckData(imageData, text);
			val = bytesToMat(imageData);
			if (list != null)
			{
				foreach (object item2 in list)
				{
					if (item2 is IShape shape)
					{
						DrawShape(shape, val);
					}
					else
					{
						if (!(item2 is Tuple<string, object> { Item2: List<object> item }))
						{
							continue;
						}
						foreach (IShape item3 in item)
						{
							DrawShape(item3, val);
						}
					}
				}
			}
			if (array != null)
			{
				val2 = Mat.FromImageData(array, (ImreadModes)1);
				float num2 = (float)imageData.Width / (float)val2.Width;
				int width = imageData.Width;
				int num3 = (int)((float)val2.Height * num2);
				Size val5 = new Size(width, num3);
				Cv2.Resize(InputArray.op_Implicit(val2), OutputArray.op_Implicit(val3), val5, 0.0, 0.0, (InterpolationFlags)1);
				Cv2.VConcat(InputArray.op_Implicit(val), InputArray.op_Implicit(val3), OutputArray.op_Implicit(val4));
				byte[] pReadByte = val4.ImEncode(Path.GetExtension(text), (int[])null);
				WriteByteToFile(pReadByte, text);
				return;
			}
			byte[] array2 = null;
			string text2 = Path.GetExtension(text).ToLower();
			if ((text2.Contains("jpg") || text2.Contains("jpeg")) && num > 0 && num <= 100)
			{
				int[] array3 = new int[3] { 1, num, 0 };
				array2 = val.ImEncode(text2, array3);
			}
			else
			{
				array2 = val.ImEncode(text2, (int[])null);
			}
			WriteByteToFile(array2, text);
		}
		catch (Exception ex)
		{
			ErrorEvent?.Invoke(null, new ErrorEventArgs(ex));
			throw ex;
		}
		finally
		{
			if (val != null)
			{
				((DisposableObject)val).Dispose();
			}
			if (val2 != null)
			{
				((DisposableObject)val2).Dispose();
			}
			if (val3 != null)
			{
				((DisposableObject)val3).Dispose();
			}
			if (val4 != null)
			{
				((DisposableObject)val4).Dispose();
			}
		}
	}

	private static void DrawShape(IShape shape, Mat source)
	{
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		if (shape.Opacity == 0.0 && !(shape is IMultiPoint))
		{
			return;
		}
		if (shape is IMultiText)
		{
			IMultiText multiText = shape as IMultiText;
			List<IGrouping<System.Windows.Point, IText>> list = (from p in multiText.Texts
				group p by p.Position).ToList();
			{
				foreach (IGrouping<System.Windows.Point, IText> item2 in list)
				{
					int num = 0;
					foreach (IText item3 in item2)
					{
						System.Windows.Point org = new System.Windows.Point(item3.Position.X, item3.Position.Y + (double)num);
						num += source.DrawText(item3, org);
					}
				}
				return;
			}
		}
		if (shape is ICircle)
		{
			ICircle circle = shape as ICircle;
			source.DrawCircle(circle);
		}
		else if (shape is IEllipse)
		{
			IEllipse ellipse = shape as IEllipse;
			source.DrawEllipse(ellipse);
		}
		else if (shape is IAnnularSector)
		{
			IAnnularSector arc = shape as IAnnularSector;
			source.DrawArc(arc);
		}
		else if (shape is ILine)
		{
			ILine line = shape as ILine;
			if (line.IsThroughLine)
			{
				source.DrawThroughLine(line);
			}
			else
			{
				source.DrawLine(line);
			}
		}
		else if (shape is IRectangle)
		{
			IRectangle oriRect = shape as IRectangle;
			source.DrawRect(oriRect);
		}
		else if (shape is IPoint)
		{
			IPoint point = shape as IPoint;
			int color = (XmlColorToInt(point.Stroke) & 0xFFFFFF) | ((int)(point.Opacity * 255.0) << 24);
			source.DrawPoint((float)point.Point.X, (float)point.Point.Y, color, (int)point.StrokeThickness);
		}
		else if (shape is ICoordinateShape)
		{
			ICoordinateShape coordinateShape = shape as ICoordinateShape;
			DrawAxis("X", source, (float)coordinateShape.CenterPoint.X, (float)coordinateShape.CenterPoint.Y, (float)coordinateShape.VectorX.X, (float)coordinateShape.VectorX.Y, coordinateShape.Stroke);
			DrawAxis("Y", source, (float)coordinateShape.CenterPoint.X, (float)coordinateShape.CenterPoint.Y, (float)coordinateShape.VectorY.X, (float)coordinateShape.VectorY.Y, coordinateShape.Stroke);
		}
		else if (shape is IArrowPath)
		{
			IArrowPath path = shape as IArrowPath;
			DrawArrowPath(source, path);
		}
		else if (shape is IAngleShape)
		{
			IAngleShape angle = shape as IAngleShape;
			DrawLineAngle(source, angle);
		}
		else if (shape is IPolygon)
		{
			IPolygon polygon = shape as IPolygon;
			DrawPolygon(source, polygon);
		}
		else if (shape is IPolyline)
		{
			IPolyline polygon2 = shape as IPolyline;
			DrawPolyline(source, polygon2);
		}
		else if (shape is ICross)
		{
			ICross cross = shape as ICross;
			source.DrawLine(new Point(0, (int)cross.CenterPoint.Y), new Point(source.Width, (int)cross.CenterPoint.Y), cross.Stroke, cross.StrokeThickness);
			source.DrawLine(new Point((int)cross.CenterPoint.X, 0), new Point((int)cross.CenterPoint.X, source.Height), cross.Stroke, cross.StrokeThickness);
		}
		else
		{
			if (!(shape is IMultiPoint))
			{
				return;
			}
			IMultiPoint multiPoint = shape as IMultiPoint;
			if (multiPoint.MicroPoints != null)
			{
				foreach (IMicroPoint microPoint in multiPoint.MicroPoints)
				{
					if (microPoint.Opacity != 0.0)
					{
						int nColor = (XmlColorToInt(microPoint.Stroke) & 0xFFFFFF) | ((int)(microPoint.Opacity * 255.0) << 24);
						System.Drawing.Color color2 = CvGetColorFromInt(nColor);
						source.Circle((int)microPoint.Point.X, (int)microPoint.Point.Y, 1, new Scalar((double)(int)color2.B, (double)(int)color2.G, (double)(int)color2.R), 1, (LineTypes)8, 0);
					}
				}
			}
			if (multiPoint.OutlinePoints != null)
			{
				List<byte[]> list2 = new List<byte[]>();
				List<int> list3 = new List<int>();
				List<int> externPoint_Buffer_NgColor = new List<int>();
				List<double> list4 = new List<double>();
				foreach (IOutlinePoint outlinePoint in multiPoint.OutlinePoints)
				{
					if (outlinePoint.Opacity != 0.0)
					{
						int item = XmlColorToInt(outlinePoint.Stroke);
						double opacity = outlinePoint.Opacity;
						if (outlinePoint.OutlineData != null)
						{
							list2.Add(outlinePoint.OutlineData);
							list3.Add(item);
							list4.Add(opacity);
						}
					}
				}
				DrawOutLinePoint(source, list2, list3, externPoint_Buffer_NgColor, list4);
			}
			if (multiPoint.Rectangles == null)
			{
				return;
			}
			foreach (IRectangle rectangle in multiPoint.Rectangles)
			{
				source.DrawRect(rectangle);
			}
		}
	}

	public static int XmlColorToInt(string xmlString)
	{
		if (xmlString != "")
		{
			return ColorTranslator.FromHtml(xmlString).ToArgb();
		}
		return 0;
	}

	private static void DrawPolygon(Mat source, IPolygon polygon)
	{
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		if (source == null || polygon == null)
		{
			return;
		}
		int num = (int)polygon.StrokeThickness;
		if (num < 2)
		{
			num = 2;
		}
		System.Drawing.Color color = ColorTranslator.FromHtml(polygon.Stroke);
		if (polygon.Points.Count <= 2)
		{
			return;
		}
		List<Point> item = ((IEnumerable<System.Windows.Point>)polygon.Points).Select((Func<System.Windows.Point, Point>)((System.Windows.Point p) =>
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			return new Point(p.X, p.Y);
		})).ToList();
		List<List<Point>> list = new List<List<Point>>();
		list.Add(item);
		Scalar val = new Scalar((double)(int)color.B, (double)(int)color.G, (double)(int)color.R);
		Cv2.Polylines(source, (IEnumerable<IEnumerable<Point>>)list, true, val, num, (LineTypes)8, 0);
		if (!string.IsNullOrEmpty(polygon.Fill))
		{
			color = ColorTranslator.FromHtml(polygon.Fill);
			if (color.A > 0)
			{
				val = new Scalar((double)(int)color.B, (double)(int)color.G, (double)(int)color.R);
				Cv2.FillPoly(source, (IEnumerable<IEnumerable<Point>>)list, val, (LineTypes)8, 0, (Point?)null);
			}
		}
	}

	private static void DrawPolyline(Mat source, IPolyline polygon)
	{
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		if (source == null || polygon == null)
		{
			return;
		}
		int num = (int)polygon.StrokeThickness;
		if (num < 2)
		{
			num = 2;
		}
		if (polygon.PolylinePointList.Count > 1)
		{
			List<Point> item = ((IEnumerable<System.Windows.Point>)polygon.PolylinePointList).Select((Func<System.Windows.Point, Point>)((System.Windows.Point p) =>
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				return new Point(p.X, p.Y);
			})).ToList();
			List<List<Point>> list = new List<List<Point>>();
			list.Add(item);
			System.Drawing.Color color = ColorTranslator.FromHtml(polygon.Stroke);
			Cv2.Polylines(source, (IEnumerable<IEnumerable<Point>>)list, false, new Scalar((double)(int)color.B, (double)(int)color.G, (double)(int)color.R), num, (LineTypes)8, 0);
		}
	}

	public static bool WriteByteToFile(byte[] pReadByte, string fileName)
	{
		FileStream fileStream = null;
		try
		{
			fileStream = new FileStream(fileName, FileMode.Create);
			fileStream.Write(pReadByte, 0, pReadByte.Length);
		}
		catch
		{
			return false;
		}
		finally
		{
			fileStream?.Close();
		}
		return true;
	}

	public static void DrawLine(this Mat img, ILine line)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		if (img != null)
		{
			int num = (int)line.StrokeThickness;
			if (num < 2)
			{
				num = 2;
			}
			Point start = new Point(line.StartPoint.X, line.StartPoint.Y);
			Point end = new Point(line.EndPoint.X, line.EndPoint.Y);
			img.DrawLine(start, end, line.Stroke, num);
		}
	}

	public static void DrawLine(this Mat img, Point start, Point end, string stroke, double strokeThickness)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (img != null)
		{
			System.Drawing.Color color = ColorTranslator.FromHtml(stroke);
			Cv2.Line(InputOutputArray.op_Implicit(img), start, end, new Scalar((double)(int)color.B, (double)(int)color.G, (double)(int)color.R), ((int)strokeThickness <= 2) ? 2 : ((int)strokeThickness), (LineTypes)8, 0);
		}
	}

	public static void DrawThroughLine(this Mat img, ILine line)
	{
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		if (img != null)
		{
			int num = (int)line.StrokeThickness;
			if (num < 2)
			{
				num = 2;
			}
			int width = img.Width;
			int height = img.Height;
			if (CalBoderPoint(new System.Windows.Point(line.StartPoint.X, line.StartPoint.Y), new System.Windows.Point(line.EndPoint.X, line.EndPoint.Y), width, height, out var borderPoint, out var borderPoint2))
			{
				Point val = new Point(borderPoint.X, borderPoint.Y);
				Point val2 = new Point(borderPoint2.X, borderPoint2.Y);
				System.Drawing.Color color = ColorTranslator.FromHtml(line.Stroke);
				Cv2.Line(InputOutputArray.op_Implicit(img), val, val2, new Scalar((double)(int)color.B, (double)(int)color.G, (double)(int)color.R), num, (LineTypes)8, 0);
			}
		}
	}

	public static void DrawPoint(this Mat img, float CenterX, float CenterY, int color, int thickness)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		if (img != null)
		{
			double num = 3.0;
			if (thickness < 2)
			{
				thickness = 2;
			}
			double num2 = num * (double)thickness;
			System.Drawing.Color color2 = CvGetColorFromInt(color);
			Cv2.Line(InputOutputArray.op_Implicit(img), new Point((double)CenterX, (double)CenterY - num2), new Point((double)CenterX, (double)CenterY + num2), new Scalar((double)(int)color2.B, (double)(int)color2.G, (double)(int)color2.R), thickness, (LineTypes)16, 0);
			Cv2.Line(InputOutputArray.op_Implicit(img), new Point((double)CenterX - num2, (double)CenterY), new Point((double)CenterX + num2, (double)CenterY), new Scalar((double)(int)color2.B, (double)(int)color2.G, (double)(int)color2.R), thickness, (LineTypes)16, 0);
		}
	}

	public static void DrawRect(this Mat img, IRectangle oriRect)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		if (img != null)
		{
			int num = (int)oriRect.StrokeThickness;
			if (num < 2)
			{
				num = 2;
			}
			RotatedRect val = new RotatedRect(new Point2f((float)oriRect.CenterPoint.X, (float)oriRect.CenterPoint.Y), new Size2f(oriRect.Width, oriRect.Height), (float)oriRect.Angle);
			Point2f[] array = val.Points();
			System.Drawing.Color color = ColorTranslator.FromHtml(oriRect.Stroke);
			for (int i = 0; i < 4; i++)
			{
				Cv2.Line(InputOutputArray.op_Implicit(img), new Point((double)array[i].X, (double)array[i].Y), new Point((double)array[(i + 1) % 4].X, (double)array[(i + 1) % 4].Y), new Scalar((double)(int)color.B, (double)(int)color.G, (double)(int)color.R), num, (LineTypes)8, 0);
			}
		}
	}

	public static int DrawText(this Mat img, IText text, System.Windows.Point org)
	{
		int result = 0;
		if (img == null)
		{
			return result;
		}
		if (text.Position.X > (double)img.Width || text.Position.Y > (double)img.Height)
		{
			return result;
		}
		return img.DrawText(org, text.Content, text.FontSize, text.Stroke);
	}

	public static int DrawText(this Mat img, System.Windows.Point org, string content, int font, string stroke)
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		int result = 0;
		if (img == null)
		{
			return result;
		}
		if (content == (AppLocalizationService.GetLang("Lang_ModuleErr") ?? "模块执行异常"))
		{
			return result;
		}
		int num = (int)((double)img.Width * 0.01);
		if (font > num)
		{
			num = font;
		}
		if (num < 10 || num > 1000)
		{
			num = 10;
		}
		Size val = MeasureText(content, num, "Microsoft YaHei");
		result = val.Height;
		System.Drawing.Color color = ColorTranslator.FromHtml(stroke);
		double num2 = (double)img.Width - org.X;
		if (num2 < (double)val.Width)
		{
			Size val2 = MeasureText("中", num, "Microsoft YaHei");
			Size val3 = MeasureText("A", num, "Microsoft YaHei");
			Size val4 = MeasureText("3", num, "Microsoft YaHei");
			StringBuilder stringBuilder = new StringBuilder();
			int num3 = 0;
			string text = content;
			for (int i = 0; i < text.Length; i++)
			{
				char value = text[i];
				int num4 = 0;
				if (IsWord(value.ToString()))
				{
					num4 = val3.Width;
				}
				else if (IsNumber(value.ToString()))
				{
					num4 = val4.Width;
				}
				else
				{
					num4 = ((!IsChinese(value.ToString())) ? val3.Width : val2.Width);
				}
				num3 += num4;
				if ((double)num3 > num2)
				{
					stringBuilder.AppendLine();
					num3 = num4;
				}
				stringBuilder.Append(value);
			}
			content = stringBuilder.ToString();
		}
		DrawMultiLineText(img, new Point((int)org.X, (int)org.Y), new Scalar((double)(int)color.B, (double)(int)color.G, (double)(int)color.R), num, content);
		return result;
	}

	public static void DrawCircle(this Mat img, ICircle circle)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		if (img != null)
		{
			int num = (int)circle.StrokeThickness;
			if (num < 2)
			{
				num = 2;
			}
			System.Drawing.Color color = ColorTranslator.FromHtml(circle.Stroke);
			Cv2.Circle(InputOutputArray.op_Implicit(img), new Point((int)circle.CenterPoint.X, (int)circle.CenterPoint.Y), (int)circle.Radius, new Scalar((double)(int)color.B, (double)(int)color.G, (double)(int)color.R), num, (LineTypes)8, 0);
		}
	}

	public static void DrawEllipse(this Mat img, IEllipse ellipse)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		if (img != null)
		{
			int num = (int)ellipse.StrokeThickness;
			if (num < 2)
			{
				num = 2;
			}
			System.Drawing.Color color = ColorTranslator.FromHtml(ellipse.Stroke);
			Scalar val = new Scalar((double)(int)color.B, (double)(int)color.G, (double)(int)color.R);
			Cv2.Ellipse(InputOutputArray.op_Implicit(img), new Point(ellipse.CenterPoint.X, ellipse.CenterPoint.Y), new Size(ellipse.MajorRadius, ellipse.MinorRadius), ellipse.Angle, 0.0, 360.0, val, num, (LineTypes)8, 0);
		}
	}

	public static void DrawArc(this Mat img, IAnnularSector arc)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		if (img != null)
		{
			int num = (int)arc.StrokeThickness;
			if (num < 2)
			{
				num = 2;
			}
			System.Drawing.Color color = ColorTranslator.FromHtml(arc.Stroke);
			Scalar val = new Scalar((double)(int)color.B, (double)(int)color.G, (double)(int)color.R);
			if (arc.AngleRange == 360.0)
			{
				Cv2.Circle(InputOutputArray.op_Implicit(img), new Point((int)arc.CenterPoint.X, (int)arc.CenterPoint.Y), (int)arc.InnerRadius, val, num, (LineTypes)8, 0);
				Cv2.Circle(InputOutputArray.op_Implicit(img), new Point((int)arc.CenterPoint.X, (int)arc.CenterPoint.Y), (int)arc.OuterRadius, val, num, (LineTypes)8, 0);
				return;
			}
			System.Windows.Point point = new System.Windows.Point(arc.CenterPoint.X + arc.InnerRadius, arc.CenterPoint.Y);
			System.Windows.Point point2 = new System.Windows.Point(arc.CenterPoint.X + arc.OuterRadius, arc.CenterPoint.Y);
			RotateTransform rotateTransform = new RotateTransform();
			rotateTransform.CenterX = arc.CenterPoint.X;
			rotateTransform.CenterY = arc.CenterPoint.Y;
			rotateTransform.Angle = arc.StartAngle;
			System.Windows.Point point3 = rotateTransform.Transform(point);
			System.Windows.Point point4 = rotateTransform.Transform(point2);
			rotateTransform.Angle = arc.StartAngle + arc.AngleRange;
			System.Windows.Point point5 = rotateTransform.Transform(point);
			System.Windows.Point point6 = rotateTransform.Transform(point2);
			img.DrawLine(new Point(point3.X, point3.Y), new Point(point4.X, point4.Y), arc.Stroke, num);
			img.DrawLine(new Point(point5.X, point5.Y), new Point(point6.X, point6.Y), arc.Stroke, num);
			bool flag = arc.AngleRange > 180.0;
			Cv2.Ellipse(InputOutputArray.op_Implicit(img), new Point(arc.CenterPoint.X, arc.CenterPoint.Y), new Size(arc.InnerRadius, arc.InnerRadius), 0.0, arc.StartAngle, arc.StartAngle + arc.AngleRange, val, num, (LineTypes)8, 0);
			Cv2.Ellipse(InputOutputArray.op_Implicit(img), new Point(arc.CenterPoint.X, arc.CenterPoint.Y), new Size(arc.OuterRadius, arc.OuterRadius), 0.0, arc.StartAngle, arc.StartAngle + arc.AngleRange, val, num, (LineTypes)8, 0);
		}
	}

	public static void DrawArrowLine(this Mat img, System.Windows.Point start, System.Windows.Point end, string color, double thickness)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		if (img != null)
		{
			if (thickness < 2.0)
			{
				thickness = 2.0;
			}
			System.Drawing.Color color2 = ColorTranslator.FromHtml(color);
			Cv2.ArrowedLine(InputOutputArray.op_Implicit(img), new Point((int)start.X, (int)start.Y), new Point((int)end.X, (int)end.Y), new Scalar((double)(int)color2.B, (double)(int)color2.G, (double)(int)color2.R), (int)thickness, (LineTypes)8, 0, 0.1);
		}
	}

	public static Mat bytesToMat(IImageData imageData, bool isRender = true)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected Obj, but got Unknown
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Expected Obj, but got Unknown
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Expected Obj, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected Obj, but got Unknown
		Mat val = null;
		Mat val2 = null;
		try
		{
			if (imageData.PixelFormat == "Rgb24")
			{
				val = new Mat(imageData.Height, imageData.Width, MatType.CV_8UC3, (Array)imageData.Buffer, 0L);
				return val.CvtColor((ColorConversionCodes)4, 0);
			}
			if (imageData.PixelFormat == "Rgb24_P3")
			{
				byte[] array = RGB_C3ToRGB_P3(imageData.Buffer, imageData.Width, imageData.Height);
				val = new Mat(imageData.Height, imageData.Width, MatType.CV_8UC3, (Array)array, 0L);
				return val.CvtColor((ColorConversionCodes)4, 0);
			}
			if (isRender)
			{
				val = new Mat(imageData.Height, imageData.Width, MatType.CV_8UC1, (Array)imageData.Buffer, 0L);
				return val.CvtColor((ColorConversionCodes)8, 0);
			}
			return new Mat(imageData.Height, imageData.Width, MatType.CV_8UC1, (Array)imageData.Buffer, 0L);
		}
		catch (Exception ex)
		{
			throw ex;
		}
		finally
		{
			if (val != null)
			{
				((DisposableObject)val).Dispose();
			}
		}
	}

	public static Mat bytesToMat(byte[] bytes, int width, int height, string PixelFormat)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected Obj, but got Unknown
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected Obj, but got Unknown
		Mat val = null;
		if (PixelFormat == PixelFormats.Gray8.ToString())
		{
			Mat val2 = new Mat(height, width, MatType.CV_8UC1, (Array)bytes, 0L);
			try
			{
				return val2.CvtColor((ColorConversionCodes)8, 0);
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		if (PixelFormat == PixelFormats.Rgb24.ToString())
		{
			Mat val3 = new Mat(height, width, MatType.CV_8UC3, (Array)bytes, 0L);
			try
			{
				return val3.CvtColor((ColorConversionCodes)4, 0);
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
		}
		return null;
	}

	public static byte[] MatToBytes(Mat AllImg)
	{
		int num = (int)(AllImg.Total() * AllImg.ElemSize());
		byte[] array = new byte[num];
		Buffer.BlockCopy(ExternalCallHelper.IntPtr2Bytes(AllImg.Data, num), 0, array, 0, num);
		return array;
	}

	public static byte[] ImageDataToBytes(ImageData imageData, string type = ".png")
	{
		try
		{
			Mat val = bytesToMat(imageData);
			byte[] result = val.ImEncode(type, (int[])null);
			((DisposableObject)val).Dispose();
			return result;
		}
		catch (Exception ex)
		{
			ErrorEvent?.Invoke(null, new ErrorEventArgs(ex));
			throw ex;
		}
	}

	public static byte[] BytesToImageData(byte[] bytes)
	{
		try
		{
			Mat val = Mat.ImDecode(bytes, (ImreadModes)0);
			byte[] result = MatToBytes(val);
			((DisposableObject)val).Dispose();
			return result;
		}
		catch (Exception ex)
		{
			ErrorEvent?.Invoke(null, new ErrorEventArgs(ex));
			throw ex;
		}
	}

	public static bool ImagePathCheck(string filePath)
	{
		try
		{
			string directoryName = Path.GetDirectoryName(filePath);
			string extension = Path.GetExtension(filePath);
			if (!Directory.Exists(directoryName) || (extension != ".jpg" && extension != ".bmp" && extension != ".png" && extension != ".jpeg"))
			{
				return false;
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	private static void CheckData(IImageData imageData, string saveName)
	{
		if (!ImagePathCheck(saveName))
		{
			throw new Exception(AppLocalizationService.GetLang("PathError"));
		}
		if (imageData.Buffer == null && !string.IsNullOrEmpty(imageData.MemoryAddress))
		{
			imageData.Buffer = GetImageBufferByFileName(imageData.MemoryAddress, imageData.Width, imageData.Height, imageData.PixelFormat);
		}
		if (imageData.Buffer == null || imageData.Buffer.Length == 0)
		{
			throw new Exception(AppLocalizationService.GetLang("NoImageData"));
		}
		if (imageData.Width * imageData.Height != imageData.Buffer.Length && imageData.Width * imageData.Height * 3 != imageData.Buffer.Length)
		{
			throw new Exception(AppLocalizationService.GetLang("ImageDataError"));
		}
		long hardDiskFreeSpace = GetHardDiskFreeSpace(Directory.GetDirectoryRoot(saveName));
		if (hardDiskFreeSpace < imageData.Buffer.LongLength / 8)
		{
			throw new Exception(AppLocalizationService.GetLang("InsufficientDiskSpace"));
		}
	}

	private unsafe static byte[] RGB_C3ToRGB_P3(byte[] srcBuffer, int width, int height)
	{
		byte[] array = null;
		if (srcBuffer != null && srcBuffer.Length == width * height * 3)
		{
			array = new byte[srcBuffer.Length];
			fixed (byte* ptr = srcBuffer)
			{
				fixed (byte* ptr2 = array)
				{
					int num = width * 3;
					int num2 = width * height;
					byte* ptr3 = ptr;
					byte* ptr4 = ptr + num2;
					byte* ptr5 = ptr + num2 * 2;
					for (int i = 0; i < num2; i++)
					{
						ptr2[i * 3] = ptr3[i];
						(ptr2 + i * 3)[1] = ptr4[i];
						(ptr2 + i * 3)[2] = ptr5[i];
					}
				}
			}
		}
		return array;
	}

	private static Size MeasureText(string text, double fontSize, string fontFamily)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		FormattedText formattedText = new FormattedText(text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface(fontFamily), fontSize, System.Windows.Media.Brushes.Black);
		return new Size(formattedText.WidthIncludingTrailingWhitespace, formattedText.Height);
	}

	private static int[] GetImageEncoding(string path)
	{
		string text = path.Substring(path.LastIndexOf("."));
		int[] array = new int[4];
		if (text.ToLower() == ".bmp")
		{
			return null;
		}
		if (text.ToLower() == ".jpg" || text.ToLower() == ".jpeg")
		{
			array[0] = 1;
			array[1] = SaveImageCompressionRatio;
			return array;
		}
		if (text.ToLower() == ".png")
		{
			array[0] = 16;
			array[1] = SaveImageCompressionRatio;
			return array;
		}
		return null;
	}

	private static System.Drawing.Color CvGetColorFromInt(int nColor)
	{
		if (Convert.ToByte((nColor >> 24) & 0xFF) == 0)
		{
			SolidColorBrush solidColorBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(Convert.ToByte(255), Convert.ToByte((nColor >> 16) & 0xFF), Convert.ToByte((nColor >> 8) & 0xFF), Convert.ToByte(nColor & 0xFF)));
			return System.Drawing.Color.FromArgb(solidColorBrush.Color.A, solidColorBrush.Color.R, solidColorBrush.Color.G, solidColorBrush.Color.B);
		}
		SolidColorBrush solidColorBrush2 = new SolidColorBrush(System.Windows.Media.Color.FromArgb(Convert.ToByte((nColor >> 24) & 0xFF), Convert.ToByte((nColor >> 16) & 0xFF), Convert.ToByte((nColor >> 8) & 0xFF), Convert.ToByte(nColor & 0xFF)));
		return System.Drawing.Color.FromArgb(solidColorBrush2.Color.A, solidColorBrush2.Color.R, solidColorBrush2.Color.G, solidColorBrush2.Color.B);
	}

	private static void DrawMultiLineText(Mat img, Point pt, Scalar scalar, int size, string str)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (!(str == "") && str != null)
		{
			Cv2.PutTextZh(InputOutputArray.op_Implicit(img), str, pt, size, scalar, "Microsoft YaHei", false, false);
		}
	}

	private static void DrawMultiLineText(Mat img, Point pt, Scalar scalar, int size, string str, float width, float height)
	{
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		if (str == "" || str == null)
		{
			return;
		}
		if (size < 1)
		{
			size = 1;
		}
		if (str.Length > 4000)
		{
			str = str.Substring(0, 4000);
		}
		if (width == 0f && height == 0f)
		{
			Cv2.PutTextZh(InputOutputArray.op_Implicit(img), str, pt, size, scalar, "微软雅黑", false, false);
			return;
		}
		Size textSize = GetTextSize(str, size);
		float num = (float)textSize.Width / width;
		int num2 = (int)Math.Ceiling(num);
		int num3 = (int)(width / (float)textSize.Width * (float)str.Length);
		int num4 = 0;
		for (int i = 0; i < num2; i++)
		{
			if (i == num2 - 1)
			{
				Cv2.PutTextZh(InputOutputArray.op_Implicit(img), str.Substring(num4), pt, size, scalar, "微软雅黑", false, false);
				num4 += num3;
			}
			else
			{
				Cv2.PutTextZh(InputOutputArray.op_Implicit(img), str.Substring(num4, num3), pt, size, scalar, "微软雅黑", false, false);
				num4 += num3;
			}
			pt.Y += textSize.Height;
		}
	}

	private static Size GetTextSize(string Text, double Size)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		Size /= 1.3333333;
		FormattedText formattedText = new FormattedText(Text, Thread.CurrentThread.CurrentCulture, FlowDirection.LeftToRight, new Typeface("微软雅黑"), Size, System.Windows.Media.Brushes.White);
		return new Size(formattedText.Width, formattedText.Height);
	}

	private static bool CalBoderPoint(System.Windows.Point pt1, System.Windows.Point pt2, int Width, int Height, out System.Windows.Point borderPoint1, out System.Windows.Point borderPoint2)
	{
		borderPoint1 = default;
		borderPoint2 = default;
		if (pt1.X == pt2.X && pt1.Y == pt2.Y)
		{
			return false;
		}
		if (pt1.X == pt2.X)
		{
			borderPoint1.X = pt1.X;
			borderPoint1.Y = 0.0;
			borderPoint2.X = pt1.X;
			borderPoint2.Y = Height;
		}
		else if (pt1.Y == pt2.Y)
		{
			borderPoint1.X = 0.0;
			borderPoint1.Y = pt1.Y;
			borderPoint2.X = Width;
			borderPoint2.Y = pt1.Y;
		}
		else
		{
			System.Windows.Point[] array = new System.Windows.Point[4];
			int[] array2 = new int[4] { 1, 1, 1, 1 };
			double num = 0.0;
			double num2 = 0.0;
			num = (pt1.Y - pt2.Y) / (pt1.X - pt2.X);
			num2 = pt2.Y - num * pt2.X;
			array[0].X = 0.0;
			array[0].Y = (int)num2;
			if (array[0].Y < 0.0 || array[0].Y > (double)Height)
			{
				array2[0] = 0;
			}
			array[1].X = Width;
			array[1].Y = (int)(num * array[1].X + num2);
			if (array[1].Y < 0.0 || array[1].Y > (double)Height)
			{
				array2[1] = 0;
			}
			array[2].Y = 0.0;
			array[2].X = (int)((array[2].Y - num2) / num);
			if (array[2].X < 0.0 || array[2].X > (double)Width)
			{
				array2[2] = 0;
			}
			array[3].Y = Height;
			array[3].X = (int)((array[3].Y - num2) / num);
			if (array[3].X < 0.0 || array[3].X > (double)Width)
			{
				array2[3] = 0;
			}
			int num3 = 0;
			for (int i = 0; i < 4; i++)
			{
				if (array2[i] == 1)
				{
					switch (num3)
					{
					case 0:
						borderPoint1.X = array[i].X;
						borderPoint1.Y = array[i].Y;
						num3++;
						break;
					case 1:
						borderPoint2.X = array[i].X;
						borderPoint2.Y = array[i].Y;
						break;
					}
				}
			}
		}
		return true;
	}

	public static byte[] GetImageBufferByFileName(string fileName, int nWidth, int nHeight, string npixelFormats)
	{
		int num = 16;
		IntPtr intPtr = IntPtr.Zero;
		IntPtr intPtr2 = IntPtr.Zero;
		byte[] result = null;
		try
		{
			byte[] array = new byte[16];
			uint num2 = (uint)array.Length;
			intPtr2 = MemoryHelper.OpenFileMapping(4, bInheritHandle: false, fileName);
			if (intPtr2 == IntPtr.Zero)
			{
				throw new Exception();
			}
			intPtr = MemoryHelper.MapViewOfFile(intPtr2, 4, 0, 0, new IntPtr(num2));
			if (intPtr == IntPtr.Zero)
			{
				throw new Exception();
			}
			Marshal.Copy(intPtr, array, 0, array.Length);
			if (intPtr != IntPtr.Zero)
			{
				MemoryHelper.UnmapViewOfFile(intPtr);
			}
			int num3 = BitConverter.ToInt32(array, 0);
			if (num3 < 0)
			{
				throw new Exception();
			}
			if (npixelFormats == PixelFormats.Gray8.ToString())
			{
				if (num3 != nWidth * nHeight)
				{
					throw new Exception();
				}
			}
			else if (num3 != nWidth * nHeight * 3)
			{
				throw new Exception();
			}
			num = BitConverter.ToInt16(array, 14);
			num2 = (uint)(num3 + num);
			intPtr = MemoryHelper.MapViewOfFile(intPtr2, 4, 0, 0, new IntPtr(num2));
			if (intPtr == IntPtr.Zero)
			{
				throw new Exception();
			}
			result = ExternalCallHelper.IntPtr2Bytes(intPtr + num, num3);
		}
		catch (Exception)
		{
			result = null;
		}
		finally
		{
			if (intPtr != IntPtr.Zero)
			{
				MemoryHelper.UnmapViewOfFile(intPtr);
			}
			if (intPtr2 != IntPtr.Zero)
			{
				MemoryHelper.CloseHandle(intPtr2);
			}
		}
		return result;
	}

	public static void SetEventEmpty()
	{
		if (ErrorEvent != null)
		{
			ErrorEvent = null;
		}
	}

	private static void DrawAxis(string content, Mat source, float centerX, float centerY, float coorDXVecX, float coorDXVecY, string color)
	{
		Vector vector = new Vector(coorDXVecX, coorDXVecY);
		vector.Normalize();
		if (!double.IsNaN(vector.X) && !double.IsNaN(vector.Y))
		{
			float num = (float)((double)source.Width * 0.05);
			System.Windows.Point point = new System.Windows.Point(centerX, centerY) + vector * num;
			source.DrawArrowLine(new System.Windows.Point(centerX, centerY), point, color, 2.0);
			source.DrawText(point, content, 7, color);
		}
	}

	private static void DrawArrowPath(Mat source, IArrowPath path)
	{
		int count = path.Points.Count;
		int num = (int)path.StrokeThickness;
		if (num < 2)
		{
			num = 2;
		}
		for (int i = 0; i < count; i++)
		{
			System.Windows.Point start = path.Points[i];
			if (i + 1 < count)
			{
				System.Windows.Point end = path.Points[i + 1];
				source.DrawArrowLine(start, end, path.Stroke, num);
			}
		}
	}

	private unsafe static void DrawOutLinePoint(Mat image, List<byte[]> ExternPoint_Buffer, List<int> ExternPoint_Buffer_OkColor, List<int> ExternPoint_Buffer_NgColor, List<double> ExternPoint_Buffer_Opacity)
	{
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		int num = 65280;
		int num2 = 16776960;
		float num3 = 1f;
		if (ExternPoint_Buffer == null)
		{
			return;
		}
		int num4 = Marshal.SizeOf<IMVS_PATMATCH_POINT_INFO>();
		int num14 = default;
		for (int i = 0; i < ExternPoint_Buffer.Count; i++)
		{
			int num5 = ((ExternPoint_Buffer_OkColor.Count > i) ? (ExternPoint_Buffer_OkColor[i] & 0xFFFFFF) : num);
			int num6 = ((ExternPoint_Buffer_NgColor.Count > i) ? (ExternPoint_Buffer_NgColor[i] & 0xFFFFFF) : num2);
			double num7 = ((ExternPoint_Buffer_Opacity.Count > i) ? ExternPoint_Buffer_Opacity[i] : ((double)num3));
			int num8 = (int)(num7 * 255.0) << 24;
			int num9 = num8 | num5;
			int num10 = num8 | num6;
			int num11 = num8 | 0xFF0000;
			byte[] array = ExternPoint_Buffer[i];
			if (array == null)
			{
				continue;
			}
			int num12 = array.Length;
			fixed (byte* ptr = array)
			{
				IMVS_PATMATCH_POINT_INFO* ptr2 = (IMVS_PATMATCH_POINT_INFO*)ptr;
				int num13 = num12 / num4;
				int* ptr3 = &num14;
				for (int j = 0; j < num13; j++)
				{
					int num15 = (int)ptr2->MatchOutlineX;
					if (num15 < 0 || num15 > image.Width)
					{
						ptr2++;
						continue;
					}
					int num16 = (int)ptr2->MatchOutlineY;
					if (num16 < 0 || num16 > image.Height)
					{
						ptr2++;
						continue;
					}
					switch ((int)ptr2->MatchOutlineScore)
					{
					case 0:
						*ptr3 = num9;
						break;
					case 1:
						*ptr3 = num10;
						break;
					default:
						*ptr3 = num11;
						break;
					}
					ptr2++;
					System.Drawing.Color color = CvGetColorFromInt(*ptr3);
					image.Circle(num15, num16, 1, new Scalar((double)(int)color.B, (double)(int)color.G, (double)(int)color.R), 1, (LineTypes)8, 0);
				}
			}
		}
	}

	private static void DrawLineAngle(Mat source, IAngleShape angle)
	{
		Vector vector = new Vector(0.0, 0.0);
		Vector vector2 = new Vector(0.0, 0.0);
		Vector vector3 = new Vector(0.0, 0.0);
		DrawDashLine(source, angle.InitialLineStartPoint.X, angle.InitialLineStartPoint.Y, angle.InitialLineEndPoint.X, angle.InitialLineEndPoint.Y, (float)angle.StrokeThickness, angle.Stroke, 3.0);
		DrawDashLine(source, angle.TerminalLineStartPoint.X, angle.TerminalLineStartPoint.Y, angle.TerminalLineEndPoint.X, angle.TerminalLineEndPoint.Y, (float)angle.StrokeThickness, angle.Stroke, 3.0);
		vector = new Vector(angle.InitialLineEndPoint.X - angle.InitialLineStartPoint.X, angle.InitialLineEndPoint.Y - angle.InitialLineStartPoint.Y);
		vector.Normalize();
		vector *= 25.0;
		vector2 = new Vector(angle.TerminalLineEndPoint.X - angle.TerminalLineStartPoint.X, angle.TerminalLineEndPoint.Y - angle.TerminalLineStartPoint.Y);
		vector2.Normalize();
		vector2 *= 25.0;
		System.Windows.Point point = new System.Windows.Point(angle.Intersection.X, angle.Intersection.Y);
		System.Windows.Point point2 = point + vector;
		System.Windows.Point point3 = point + vector2;
		DrawArc(source, angle.Intersection.X, angle.Intersection.Y, point2.X, point2.Y, point3.X, point3.Y, 25.0, (int)angle.StrokeThickness, angle.Stroke);
		string content = angle.Angle.ToString("N1") + "°";
		source.DrawText(angle.Intersection, content, 14, angle.Stroke);
	}

	private static void DrawArc(Mat img, double centerX, double centerY, double startX, double startY, double endX, double endY, double radius, int thickness, string color)
	{
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		double num = Math.Atan2(startY - centerY, startX - centerX) * 180.0 / Math.PI;
		double num2 = Math.Atan2(endY - centerY, endX - centerX) * 180.0 / Math.PI;
		double num3 = Math.Abs(num2 - num);
		double num4 = Math.Max(num, num2);
		double num5 = Math.Min(num, num2);
		System.Drawing.Color color2 = ColorTranslator.FromHtml(color);
		if (num3 > 180.0)
		{
			num5 += 360.0;
		}
		if (thickness < 2)
		{
			thickness = 2;
		}
		Cv2.Ellipse(InputOutputArray.op_Implicit(img), new Point(centerX, centerY), new Size(radius, radius), 0.0, num5, num4, new Scalar((double)(int)color2.B, (double)(int)color2.G, (double)(int)color2.R), thickness, (LineTypes)8, 0);
	}

	private static void DrawDashLine(Mat source, double startX, double startY, double endX, double endY, float strokeThickness, string color, double interval)
	{
		double num = Math.Abs(startX - endX);
		double num2 = Math.Abs(startY - endY);
		int num3 = (int)(Math.Sqrt(num * num + num2 * num2) / interval);
		double num4 = num / (double)num3;
		double num5 = num2 / (double)num3;
		int num6 = ((startX < endX) ? 1 : (-1));
		int num7 = ((startY < endY) ? 1 : (-1));
		int num8;
		for (num8 = 0; num8 < num3; num8++)
		{
			System.Windows.Point start = new System.Windows.Point
			{
				X = (float)(startX + (double)(num6 * num8) * num4),
				Y = (float)(startY + (double)(num7 * num8) * num5)
			};
			num8++;
			DrawArrowLine(end: new System.Windows.Point
			{
				X = (float)(startX + (double)(num6 * num8) * num4),
				Y = (float)(startY + (double)(num7 * num8) * num5)
			}, img: source, start: start, color: color, thickness: strokeThickness);
		}
	}

	public static long GetHardDiskFreeSpace(string str_HardDiskName)
	{
		long result = 0L;
		DriveInfo[] drives = DriveInfo.GetDrives();
		DriveInfo[] array = drives;
		foreach (DriveInfo driveInfo in array)
		{
			if (driveInfo.Name == str_HardDiskName.ToUpper())
			{
				result = driveInfo.TotalFreeSpace;
			}
		}
		return result;
	}

	private static bool IsChinese(string value)
	{
		Regex regex = new Regex("^[一-龥]$");
		return regex.IsMatch(value);
	}

	private static bool IsNumber(string value)
	{
		Regex regex = new Regex("^[0-9]$");
		return regex.IsMatch(value);
	}

	private static bool IsWord(string value)
	{
		Regex regex = new Regex("^[a-zA-Z]$");
		return regex.IsMatch(value);
	}

	private static bool IsChineseOrNumberOrWord(string value)
	{
		Regex regex = new Regex("^[一-龥a-zA-Z0-9]$");
		return regex.IsMatch(value);
	}
}
