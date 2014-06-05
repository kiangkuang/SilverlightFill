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

		public static void up(MouseButtonEventArgs e, InkPresenter inkCanvas, Color selectedColor)
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
			Color replacementColor = Common.getTargetColor(e, inkCanvas, wb1);

			MainPage.wbList[clickedLayer].Clear();
			wb1 = Common.convertToBitmap(inkCanvas);
			Color targetColor = Common.getTargetColor(e, inkCanvas, wb1);

			WriteableBitmap wb2 = new WriteableBitmap(wb1.PixelWidth, wb1.PixelHeight);
			Fill.floodFill(new Point((int)e.GetPosition(inkCanvas).X, (int)e.GetPosition(inkCanvas).Y), targetColor, replacementColor, wb1, wb2);

			MainPage.wbList[clickedLayer] = wb2;
			MainPage.imageList[clickedLayer].Source = wb2;

			for (int i = 0; i < inkCanvas.Strokes.Count; i++)
			{
				inkCanvas.Strokes[i].DrawingAttributes.Height = inkCanvas.Strokes[i].DrawingAttributes.Width = 5;
			}
		}
	}
}
