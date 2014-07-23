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

			changeStrokeToThinnerWidth(inkCanvas);

			WriteableBitmap compressedBitmap = Common.convertToBitmap(inkCanvas);
			WriteableBitmap clickedBitmap = MainPage.wbList[clickedLayer];

			substractSelectedArea(e, inkCanvas, clickedLayer, compressedBitmap, clickedBitmap);

			changeStrokeToOriginalWidth(inkCanvas);
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

		private static void substractSelectedArea(MouseButtonEventArgs e, InkPresenter inkCanvas, int clickedLayer, WriteableBitmap compressedBitmap, WriteableBitmap clickedBitmap)
		{
			Color targetColor = Common.getTargetColor(e, inkCanvas, compressedBitmap);

			Fill.floodFill(new Point((int)e.GetPosition(inkCanvas).X, (int)e.GetPosition(inkCanvas).Y), targetColor, Color.FromArgb(0, 0, 0, 0), compressedBitmap, clickedBitmap);
			MainPage.wbList[clickedLayer] = clickedBitmap;
			MainPage.imageList[clickedLayer].Source = clickedBitmap;
		}


	}
}
