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
	public class Fill
	{
		private static double maxLeft;
		private static double maxRight;
		private static double maxTop;
		private static double maxBottom;
		public static void down(MouseButtonEventArgs e)
		{

		}

		public static void move(MouseEventArgs e)
		{

		}

		public static void up(MouseButtonEventArgs e, InkPresenter inkCanvas, Grid LayoutRoot, Color selectedColor)
		{
			long before = DateTime.Now.Ticks;

			//make the line thinner
			changeStrokeToThinnerWidth(inkCanvas);

			WriteableBitmap compressedBitmap = Common.convertToBitmap(inkCanvas);
			WriteableBitmap outputBitmap = new WriteableBitmap(compressedBitmap.PixelWidth, compressedBitmap.PixelHeight);
			Color targetColor = Common.getTargetColor(e, inkCanvas, compressedBitmap);

			floodFill(new Point((int)e.GetPosition(inkCanvas).X, (int)e.GetPosition(inkCanvas).Y), targetColor, selectedColor, compressedBitmap, outputBitmap);

			MainPage.layerList.Add(new Layer(LayoutRoot, outputBitmap));
			//change the line back to original width
			changeStrokeToOriginalWidth(inkCanvas);

			long after = DateTime.Now.Ticks;
			TimeSpan elapsedTime = new TimeSpan(after - before);
			System.Diagnostics.Debug.WriteLine("Fill:		" + elapsedTime.TotalMilliseconds + " milliseconds");
		}

		private static void changeStrokeToOriginalWidth(InkPresenter inkCanvas)
		{
			for (int i = 0; i < inkCanvas.Strokes.Count; i++)
			{
				inkCanvas.Strokes[i].DrawingAttributes.Height = inkCanvas.Strokes[i].DrawingAttributes.Width = 5;
			}
		}

		private static void changeStrokeToThinnerWidth(InkPresenter inkCanvas)
		{
			for (int i = 0; i < inkCanvas.Strokes.Count; i++)
			{
				inkCanvas.Strokes[i].DrawingAttributes.Height = inkCanvas.Strokes[i].DrawingAttributes.Width = 1;
			}
		}

		public static void floodFill(Point pt, Color targetColor, Color replacementColor, WriteableBitmap compressedBitmap, WriteableBitmap outputBitmap)
		{
			Queue<Point> q = new Queue<Point>();

			maxLeft = compressedBitmap.PixelWidth;
			//System.Diagnostics.Debug.WriteLine(maxLeft);
			maxRight = 0;
			maxTop = compressedBitmap.PixelHeight;
			//System.Diagnostics.Debug.WriteLine("while filling" + maxTop);
			maxBottom = 0;

			if (!Common.ColorMatch(compressedBitmap.GetPixel((int)pt.X, (int)pt.Y), targetColor) || Common.ColorMatch(compressedBitmap.GetPixel((int)pt.X, (int)pt.Y), replacementColor))
			{
				return;
			}

			q.Enqueue(pt);

			while (q.Count > 0)
			{
				Point n = q.Dequeue();

				if (Common.ColorMatch(compressedBitmap.GetPixel((int)n.X, (int)n.Y), targetColor))
				{
					Point west = n;
					Point east = n;

					while ((west.X >= 0) && (west.X < compressedBitmap.PixelWidth) && Common.ColorMatch(compressedBitmap.GetPixel((int)west.X, (int)west.Y), targetColor))
					{
						west.X--;
					}
					while ((east.X >= 0) && (east.X < compressedBitmap.PixelWidth) && Common.ColorMatch(compressedBitmap.GetPixel((int)east.X, (int)east.Y), targetColor))
					{
						east.X++;
					}
					west.X++;
					east.X--;

					if (west.X < maxLeft)
					{
						maxLeft = west.X;
					}

					if (east.X > maxRight)
					{
						maxRight = east.X;
					}


					for (int i = (int)west.X; i <= east.X; i++)
					{
						compressedBitmap.SetPixel(i, (int)west.Y, replacementColor);
						outputBitmap.SetPixel(i, (int)west.Y, replacementColor);
						if ((i >= 0) && (i < compressedBitmap.PixelWidth) && (west.Y + 1 >= 0 && west.Y + 1 < compressedBitmap.PixelHeight) && Common.ColorMatch(compressedBitmap.GetPixel(i, (int)west.Y + 1), targetColor))
						{
							if (west.Y + 1 > maxBottom)
							{
								maxBottom = west.Y + 1;
							}

							q.Enqueue(new Point(i, west.Y + 1));
						}
						if ((i >= 0) && (i < compressedBitmap.PixelWidth) && (west.Y - 1 >= 0 && west.Y - 1 < compressedBitmap.PixelHeight) && Common.ColorMatch(compressedBitmap.GetPixel(i, (int)west.Y - 1), targetColor))
						{
							if (west.Y - 1 < maxTop)
							{
								maxTop = west.Y - 1;
							}

							q.Enqueue(new Point(i, west.Y - 1));
						}
					}
				}
			}
		}

	}
}
