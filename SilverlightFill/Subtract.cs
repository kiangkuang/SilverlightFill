using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Runtime.InteropServices;

namespace SilverlightFill
{
	public class Subtract
	{
		public static WriteableBitmap wb1;
		public static WriteableBitmap wb2;
		public static WriteableBitmap wb3;
		public static void down(MouseButtonEventArgs e)
		{

		}

		public static void move(MouseEventArgs e)
		{

		}

		public static void up(MouseButtonEventArgs e, InkPresenter inkCanvas, Grid LayoutRoot, Color selectedColor)
		{
			int clickedLayer = Common.hitTestLayer(e, inkCanvas);
			if (clickedLayer == -1)
			{
				return;
			}

			wb1 = Common.convertToBitmap(inkCanvas);
			wb2 = Common.convertToBitmap(inkCanvas);
			Color targetColor = Common.getTargetColor(e, inkCanvas, wb1);

			int[] upDownLeftRight = enclosedArea(new Point((int)e.GetPosition(inkCanvas).X, (int)e.GetPosition(inkCanvas).Y), targetColor);

			StrokeCollection del = new StrokeCollection();
			for (int i = 0; i < MainPage.presenterList[clickedLayer].Strokes.Count; i++)
			{
				if (MainPage.presenterList[clickedLayer].Strokes[i].StylusPoints[0].Y >= upDownLeftRight[0] - 3 &&
					MainPage.presenterList[clickedLayer].Strokes[i].StylusPoints[0].Y <= upDownLeftRight[1] + 3 &&
					MainPage.presenterList[clickedLayer].Strokes[i].StylusPoints[1].X >= upDownLeftRight[2] - 3 &&
					MainPage.presenterList[clickedLayer].Strokes[i].StylusPoints[0].X <= upDownLeftRight[3] + 3)
				{
					del.Add(MainPage.presenterList[clickedLayer].Strokes[i]);
				}
			}
			for (int i = 0; i < del.Count; i++)
			{
				MainPage.presenterList[clickedLayer].Strokes.Remove(del[i]);
			}

			fillLeftRight(inkCanvas, MainPage.presenterList[clickedLayer], targetColor, upDownLeftRight, (int)e.GetPosition(inkCanvas).X, (int)e.GetPosition(inkCanvas).Y);

			if (MainPage.presenterList[clickedLayer].Strokes.Count == 0)
			{
				LayoutRoot.Children.Remove(MainPage.presenterList[clickedLayer]);
				MainPage.presenterList.RemoveAt(clickedLayer);
			}
		}

		private static void fillLeftRight(InkPresenter inkCanvas, InkPresenter inkPresenter, Color targetColor, int[] upDownLeftRight, int x, int y)
		{
			Color replacementColor = Color.FromArgb(255, 200, 200, 200);
			wb3 = Common.convertToBitmap(inkCanvas);
			for (int i = 0; i < wb3.PixelWidth; i++)
			{
				for (int j = 0; j < wb3.PixelHeight; j++)
				{
					if (!Common.ColorMatch(wb3.GetPixel(i, j), wb2.GetPixel(i, j)) && !wb1.GetPixel(i, j).Equals(replacementColor))
					{
						StylusPointCollection targetedStylusPoint = new StylusPointCollection();
						targetedStylusPoint.Add(new StylusPoint(i + 2, j + 2));
						targetedStylusPoint.Add(new StylusPoint(i - 2, j + 2));
						targetedStylusPoint.Add(new StylusPoint(i - 2, j - 2));
						targetedStylusPoint.Add(new StylusPoint(i + 2, j - 2));
						targetedStylusPoint.Add(new StylusPoint(i + 2, j + 2));
						if (inkCanvas.Strokes.HitTest(targetedStylusPoint).Count == 0)
						{
							Fill.floodFill(new Point(i, j), wb3.GetPixel(i, j), targetColor, inkPresenter, wb3);
						}
						
					}
				}
			}

		}

		public static int[] enclosedArea(Point pt, Color targetColor)
		{
			Color replacementColor = Color.FromArgb(255, 200, 200, 200);
			int[] upDownLeftRight = new int[] { (int)pt.Y, (int)pt.Y, (int)pt.X, (int)pt.X };
			Queue<Point> q = new Queue<Point>();

			q.Enqueue(pt);

			while (q.Count > 0)
			{
				Point n = q.Dequeue();

				if (wb1.GetPixel((int)n.X, (int)n.Y).Equals(targetColor))
				{
					upDownLeftRight[0] = (int)Math.Min(upDownLeftRight[0], n.Y);
					upDownLeftRight[1] = (int)Math.Max(upDownLeftRight[1], n.Y);
					upDownLeftRight[2] = (int)Math.Min(upDownLeftRight[2], n.X);
					upDownLeftRight[3] = (int)Math.Max(upDownLeftRight[3], n.X);

					Point w = n;
					Point e = n;

					while ((w.X > 0) && (w.X < wb1.PixelWidth - 1) && wb1.GetPixel((int)w.X, (int)w.Y).Equals(targetColor))
					{
						w.X--;
					}
					while ((e.X > 0) && (e.X < wb1.PixelWidth - 1) && wb1.GetPixel((int)e.X, (int)e.Y).Equals(targetColor))
					{
						e.X++;
					}

					for (int i = (int)w.X; i <= e.X; i++)
					{
						wb1.SetPixel(i, (int)w.Y, replacementColor);
						if ((i >= 0) && (i < wb1.PixelWidth) && (w.Y + 1 >= 0 && w.Y + 1 < wb1.PixelHeight) && wb1.GetPixel(i, (int)w.Y + 1).Equals(targetColor))
						{
							q.Enqueue(new Point(i, w.Y + 1));
						}
						if ((i >= 0) && (i < wb1.PixelWidth) && (w.Y - 1 >= 0 && w.Y - 1 < wb1.PixelHeight) && wb1.GetPixel(i, (int)w.Y - 1).Equals(targetColor))
						{
							q.Enqueue(new Point(i, w.Y - 1));
						}
					}
				}
			}
			return upDownLeftRight;
		}
	}
}
