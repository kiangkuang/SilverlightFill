using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SilverlightFill
{
	public class Common
	{
		public static Point clickedPos;

		public static bool ColorMatch(Color a, Color b)
		{
			int tolerance = 32;
			return Math.Abs(a.A - b.A) < tolerance &&
				   Math.Abs(a.R - b.R) < tolerance &&
				   Math.Abs(a.G - b.G) < tolerance &&
				   Math.Abs(a.B - b.B) < tolerance;
		}

		public static bool ColorMatch(Color a, Color b, int tolerance)
		{
			return Math.Abs(a.A - b.A) < tolerance &&
				   Math.Abs(a.R - b.R) < tolerance &&
				   Math.Abs(a.G - b.G) < tolerance &&
				   Math.Abs(a.B - b.B) < tolerance;
		}

		public static Color blendPixel(Color front, Color back)
		{
			double fA = front.A / 255.0;
			double fR = front.R / 255.0;
			double fG = front.G / 255.0;
			double fB = front.B / 255.0;

			double bA = back.A / 255.0;
			double bR = back.R / 255.0;
			double bG = back.G / 255.0;
			double bB = back.B / 255.0;

			double rA = 1 - (1 - fA) * (1 - bA);
			double rR = (fR * fA + bR * bA * (1 - fA)) / rA;
			double rG = (fG * fA + bG * bA * (1 - fA)) / rA;
			double rB = (fB * fA + bB * bA * (1 - fA)) / rA;

			return Color.FromArgb((byte)(rA * 255), (byte)(rR * 255), (byte)(rG * 255), (byte)(rB * 255));
		}

		public static WriteableBitmap convertToBitmap(InkPresenter inkCanvas)
		{
			InkPresenter temp = new InkPresenter();
			for (int i = 0; i < MainPage.presenterList.Count; i++)
			{
				for (int j = 0; j < MainPage.presenterList[i].Strokes.Count; j++)
				{
					temp.Strokes.Add(MainPage.presenterList[i].Strokes[j]);
				}
			}
			for (int i = 0; i < inkCanvas.Strokes.Count; i++)
			{
				temp.Strokes.Add(inkCanvas.Strokes[i]);
			}

			WriteableBitmap wb = new WriteableBitmap((int)inkCanvas.ActualWidth, (int)inkCanvas.ActualHeight);
			wb.Render(temp, new TranslateTransform());
			wb.Invalidate();
			return wb;
		}

		public static Color getTargetColor(MouseButtonEventArgs e, InkPresenter inkCanvas, WriteableBitmap wb)
		{
			return wb.GetPixel((int)e.GetPosition(inkCanvas).X, (int)e.GetPosition(inkCanvas).Y);
		}

		public static int hitTestLayer(MouseButtonEventArgs e, InkPresenter inkCanvas)
		{
			for (int i = MainPage.wbList.Count - 1; i >= 0; i--)
			{
				if (getTargetColor(e, inkCanvas, MainPage.wbList[i]) != Color.FromArgb(0, 0, 0, 0))
				{
					return i;
				}
			}
			return -1;
		}
	}
}
