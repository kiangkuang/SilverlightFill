﻿using System.Collections.Generic;
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
		public static void down(MouseButtonEventArgs e)
		{

		}

		public static void move(MouseEventArgs e)
		{

		}

		public static void up(MouseButtonEventArgs e, InkPresenter inkCanvas, Grid LayoutRoot, Color selectedColor)
		{
			Common.convertToBitmap(inkCanvas);
			Color targetColor = Common.getTargetColor(e, inkCanvas);

			InkPresenter newPresenter = new InkPresenter();
			LayoutRoot.Children.Add(newPresenter);
			MainPage.presenterList.Add(newPresenter);
			floodFill(new Point((int)e.GetPosition(inkCanvas).X, (int)e.GetPosition(inkCanvas).Y), targetColor, selectedColor, newPresenter);
		}

		public static void floodFill(Point pt, Color targetColor, Color replacementColor, InkPresenter presenter)
		{
			Queue<Point> q = new Queue<Point>();

			if (!Common.ColorMatch(Common.wb.GetPixel((int)pt.X, (int)pt.Y), targetColor) || Common.ColorMatch(Common.wb.GetPixel((int)pt.X, (int)pt.Y), replacementColor))
			{
				return;
			}
			q.Enqueue(pt);

			while (q.Count > 0)
			{
				Point n = q.Dequeue();

				if (Common.ColorMatch(Common.wb.GetPixel((int)n.X, (int)n.Y), targetColor))
				{
					Point w = new Point(n.X, n.Y);
					Point e = new Point(n.X, n.Y);

					while ((w.X >= 0) && (w.X < Common.wb.PixelWidth) && Common.ColorMatch(Common.wb.GetPixel((int)w.X, (int)w.Y), targetColor))
					{
						w.X--;
					}
					while ((e.X >= 0) && (e.X < Common.wb.PixelWidth) && Common.ColorMatch(Common.wb.GetPixel((int)e.X, (int)e.Y), targetColor))
					{
						e.X++;
					}

					StylusPointCollection stylusPoints = new StylusPointCollection();
					stylusPoints.Add(new StylusPoint(++w.X, w.Y));
					stylusPoints.Add(new StylusPoint(--e.X, w.Y));
					Stroke stroke = new Stroke(stylusPoints);
					stroke.DrawingAttributes.Height = 4;
					stroke.DrawingAttributes.Width = 4;
					stroke.DrawingAttributes.Color = replacementColor;
					presenter.Strokes.Add(stroke);

					for (int i = (int)w.X; i <= e.X; i++)
					{
						Common.wb.SetPixel(i, (int)w.Y, replacementColor);
						if ((i >= 0) && (i < Common.wb.PixelWidth) && (w.Y + 1 >= 0 && w.Y + 1 < Common.wb.PixelHeight) && Common.ColorMatch(Common.wb.GetPixel(i, (int)w.Y + 1), targetColor))
						{
							q.Enqueue(new Point(i, w.Y + 1));
						}
						if ((i >= 0) && (i < Common.wb.PixelWidth) && (w.Y - 1 >= 0 && w.Y - 1 < Common.wb.PixelHeight) && Common.ColorMatch(Common.wb.GetPixel(i, (int)w.Y - 1), targetColor))
						{
							q.Enqueue(new Point(i, w.Y - 1));
						}
					}
				}
			}
		}

	}
}
