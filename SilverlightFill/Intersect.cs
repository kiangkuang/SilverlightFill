using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SilverlightFill
{
	class Intersect
	{
		public static void down(MouseButtonEventArgs e)
		{

		}

		public static void move(MouseEventArgs e)
		{

		}

		public static void up(MouseButtonEventArgs e, InkPresenter inkCanvas, Color selectedColor, Grid LayoutRoot)
		{
			int clickedLayer = Common.hitTestLayer(e, inkCanvas);
			if (clickedLayer == -1)
			{
				return;
			}

			for (int i = 0; i < inkCanvas.Strokes.Count; i++)
			{
				inkCanvas.Strokes[i].DrawingAttributes.Height = inkCanvas.Strokes[i].DrawingAttributes.Width = 1;
			}

			WriteableBitmap wb1 = Common.convertToBitmap(inkCanvas);
			WriteableBitmap wb2 = new WriteableBitmap(wb1.PixelWidth, wb1.PixelHeight);
			WriteableBitmap wb3 = new WriteableBitmap(wb1.PixelWidth, wb1.PixelHeight);
			Color targetColor = Common.getTargetColor(e, inkCanvas, wb1);

			int tempR = targetColor.R;
			if (targetColor.R + 50 >= 240)
			{
				tempR += 50;
			}
			else
			{
				tempR -= 50;
			}

			Color tempColor = Color.FromArgb(255, (byte)tempR, 255, 255);

			Fill.floodFill(new Point((int)e.GetPosition(inkCanvas).X, (int)e.GetPosition(inkCanvas).Y), targetColor, tempColor, wb1, wb2);
			Fill.floodFill(new Point((int)e.GetPosition(inkCanvas).X, (int)e.GetPosition(inkCanvas).Y), tempColor, targetColor, wb2, wb3);

			MainPage.wbList[clickedLayer] = wb3;
			MainPage.imageList[clickedLayer].Source = wb3;

			for (int i = 0; i < inkCanvas.Strokes.Count; i++)
			{
				inkCanvas.Strokes[i].DrawingAttributes.Height = inkCanvas.Strokes[i].DrawingAttributes.Width = 5;
			}
		}
	}
}
