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
		public static void down(MouseButtonEventArgs e)
		{

		}

		public static void move(MouseEventArgs e)
		{

		}

		public static void up(MouseButtonEventArgs e, InkPresenter inkCanvas, Grid LayoutRoot, Color selectedColor)
		{
			for (int i = 0; i < inkCanvas.Strokes.Count; i++)
			{
				inkCanvas.Strokes[i].DrawingAttributes.Height = inkCanvas.Strokes[i].DrawingAttributes.Width = 1;
			}

			WriteableBitmap wb1 = Common.convertToBitmap(inkCanvas);
			WriteableBitmap wb2 = new WriteableBitmap(wb1.PixelWidth, wb1.PixelHeight);
			Color targetColor = Common.getTargetColor(e, inkCanvas, wb1);

			floodFill(new Point((int)e.GetPosition(inkCanvas).X, (int)e.GetPosition(inkCanvas).Y), targetColor, selectedColor, wb1, wb2);

			Image img = new Image();
			img.Source = wb2;
			img.Stretch = Stretch.None;

			MainPage.imageList.Add(img);
			MainPage.wbList.Add(wb2);
			LayoutRoot.Children.Add(img);

			for (int i = 0; i < inkCanvas.Strokes.Count; i++)
			{
				inkCanvas.Strokes[i].DrawingAttributes.Height = inkCanvas.Strokes[i].DrawingAttributes.Width = 5;
			}
		}

		public static void floodFill(Point pt, Color targetColor, Color replacementColor, WriteableBitmap wb1, WriteableBitmap wb2)
		{
			Queue<Point> q = new Queue<Point>();

			if (!Common.ColorMatch(wb1.GetPixel((int)pt.X, (int)pt.Y), targetColor) || Common.ColorMatch(wb1.GetPixel((int)pt.X, (int)pt.Y), replacementColor))
			{
				return;
			}
			q.Enqueue(pt);

			while (q.Count > 0)
			{
				Point n = q.Dequeue();

				if (Common.ColorMatch(wb1.GetPixel((int)n.X, (int)n.Y), targetColor))
				{
					Point w = n;
					Point e = n;

					while ((w.X >= 0) && (w.X < wb1.PixelWidth) && Common.ColorMatch(wb1.GetPixel((int)w.X, (int)w.Y), targetColor))
					{
						w.X--;
					}
					while ((e.X >= 0) && (e.X < wb1.PixelWidth) && Common.ColorMatch(wb1.GetPixel((int)e.X, (int)e.Y), targetColor))
					{
						e.X++;
					}
					w.X++;
					e.X--;

					for (int i = (int)w.X; i <= e.X; i++)
					{
						wb1.SetPixel(i, (int)w.Y, replacementColor);
						wb2.SetPixel(i, (int)w.Y, replacementColor);
						if ((i >= 0) && (i < wb1.PixelWidth) && (w.Y + 1 >= 0 && w.Y + 1 < wb1.PixelHeight) && Common.ColorMatch(wb1.GetPixel(i, (int)w.Y + 1), targetColor))
						{
							q.Enqueue(new Point(i, w.Y + 1));
						}
						if ((i >= 0) && (i < wb1.PixelWidth) && (w.Y - 1 >= 0 && w.Y - 1 < wb1.PixelHeight) && Common.ColorMatch(wb1.GetPixel(i, (int)w.Y - 1), targetColor))
						{
							q.Enqueue(new Point(i, w.Y - 1));
						}
					}
				}
			}
		}

	}
}
