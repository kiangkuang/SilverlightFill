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
			WriteableBitmap wb = new WriteableBitmap((int)inkCanvas.ActualWidth, (int)inkCanvas.ActualHeight);
			for (int i = 0; i < MainPage.layerList.Count; i++)
			{
				wb.Render(MainPage.layerList[i].img, new TranslateTransform());
			}

			wb.Render(inkCanvas, new TranslateTransform());
			wb.Invalidate();
			return wb;
		}

		public static Color getTargetColor(MouseButtonEventArgs e, InkPresenter inkCanvas, WriteableBitmap wb)
		{
			return wb.GetPixel((int)e.GetPosition(inkCanvas).X, (int)e.GetPosition(inkCanvas).Y);
		}

		public static int hitTestLayer(MouseButtonEventArgs e, InkPresenter inkCanvas)
		{
			for (int i = MainPage.layerList.Count - 1; i >= 0; i--)
			{
				if (getTargetColor(e, inkCanvas, MainPage.layerList[i].wb) != Color.FromArgb(0, 0, 0, 0))
				{
					return i;
				}
			}
			return -1;
		}


		public static void checkIfOutOfBound(MouseButtonEventArgs e, InkPresenter inkCanvas, Image img, int clickedLayer, WriteableBitmap imageBackup, double offSetLeft, double offSetRight, double offSetTop, double  offSetBottom, double maxLeft, double maxRight, double maxTop, double maxBottom)
		{
			double test = e.GetPosition(inkCanvas).X;
			if ( test - offSetLeft <= 0 || maxLeft == 0)
			{
				System.Diagnostics.Debug.WriteLine("left out");
				MainPage.layerList[clickedLayer].imgBackup = imageBackup;
				Point newPoint = new Point(img.Margin.Left, img.Margin.Top);
				MainPage.layerList[clickedLayer].imageBackupOffset = newPoint;

			}
			else if (e.GetPosition(inkCanvas).X + offSetRight >= MainPage.layerList[clickedLayer].wb.PixelWidth || maxRight == 0)
			{  
				System.Diagnostics.Debug.WriteLine("right out");
				MainPage.layerList[clickedLayer].imgBackup = imageBackup;
				Point newPoint = new Point(img.Margin.Left, img.Margin.Top);
				MainPage.layerList[clickedLayer].imageBackupOffset = newPoint;
			}
			else if (e.GetPosition(inkCanvas).Y - offSetTop <= 0 || maxTop == 0)
			{    
				System.Diagnostics.Debug.WriteLine("top out");
				MainPage.layerList[clickedLayer].imgBackup = imageBackup;
				Point newPoint = new Point(img.Margin.Left, img.Margin.Top);
				MainPage.layerList[clickedLayer].imageBackupOffset = newPoint;
			}
			else if (e.GetPosition(inkCanvas).Y + offSetBottom >= MainPage.layerList[clickedLayer].wb.PixelHeight || maxBottom == 0)
			{
				System.Diagnostics.Debug.WriteLine("bottom out");
				MainPage.layerList[clickedLayer].imgBackup = imageBackup;
				Point newPoint = new Point(img.Margin.Left, img.Margin.Top);
				MainPage.layerList[clickedLayer].imageBackupOffset = newPoint;
			}
		}

		private static bool flag1 = false;
		private static bool flag2 = false;

		private const int LEFT = 0;
		private const int RIGHT = 1;
		private const int TOP = 2;
		private const int BOTTOM = 3;
		public static void calculateMax(Image image, MouseButtonEventArgs e, InkPresenter inkCanvas, int clickedLayer)
		{
			clickedLayer = Common.hitTestLayer(e, inkCanvas);

			if (clickedLayer == -1)
			{
				return;
			}

			WriteableBitmap wb = MainPage.layerList[clickedLayer].wb;

			//max left and right
			for (int w = 0; w < MainPage.layerList[clickedLayer].wb.PixelWidth; w++)
			{
				for (int h = 0; h < MainPage.layerList[clickedLayer].wb.PixelHeight; h++)
				{
					if (wb.GetPixel(w, h) != Color.FromArgb(0, 0, 0, 0) && !flag1)
					{
						flag1 = true;
						MainPage.layerList[clickedLayer].imageToBorderDist[LEFT] = w;
					}

					if (wb.GetPixel(w, h) != Color.FromArgb(0, 0, 0, 0) && flag1)
					{
						flag2 = true;
					}
				}

				if (flag1 && !flag2)
				{
					MainPage.layerList[clickedLayer].imageToBorderDist[RIGHT] = w;
					break;
				}


				flag2 = false;
			}


			flag1 = false;
			flag2 = false;

			//max top and bottom
			for (int h = 0; h < MainPage.layerList[clickedLayer].wb.PixelHeight; h++)
			{
				for (int w = 0; w < MainPage.layerList[clickedLayer].wb.PixelWidth; w++)
				{
					if (wb.GetPixel(w, h) != Color.FromArgb(0, 0, 0, 0) && !flag1)
					{
						flag1 = true;
						MainPage.layerList[clickedLayer].imageToBorderDist[TOP] = h;
					}

					if (wb.GetPixel(w, h) != Color.FromArgb(0, 0, 0, 0) && flag1)
					{
						flag2 = true;
					}
				}

				if (flag1 && !flag2)
				{
					MainPage.layerList[clickedLayer].imageToBorderDist[BOTTOM] = h;
					break;
				}


				flag2 = false;
			}

			flag1 = false;
			flag2 = false;

		}

		public static void findNewImageToBorderDist(Image image, MouseButtonEventArgs e, InkPresenter inkCanvas, int clickedLayer, double offSetLeft, double offSetRight, double offSetTop, double offSetBottom, double maxLeft, double maxRight, double maxTop, double maxBottom)
		{
			if (maxLeft == 0 || maxRight == 0 || maxTop == 0 || maxBottom == 0)
			{
				return;
			}

			MainPage.layerList[clickedLayer].imageToBorderDist[LEFT] = e.GetPosition(inkCanvas).X - offSetLeft;
			MainPage.layerList[clickedLayer].imageToBorderDist[RIGHT] = e.GetPosition(inkCanvas).X + offSetRight;
			MainPage.layerList[clickedLayer].imageToBorderDist[TOP] = e.GetPosition(inkCanvas).Y - offSetTop;
			MainPage.layerList[clickedLayer].imageToBorderDist[BOTTOM] = e.GetPosition(inkCanvas).Y + offSetBottom;
		}
	}
}
