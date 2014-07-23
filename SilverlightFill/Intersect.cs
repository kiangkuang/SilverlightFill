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

			changeStrokeToThinnerWidth(inkCanvas);

			WriteableBitmap compressBitmap = Common.convertToBitmap(inkCanvas);
			WriteableBitmap tempBitmap = new WriteableBitmap(compressBitmap.PixelWidth, compressBitmap.PixelHeight);
			WriteableBitmap outputBitmap = new WriteableBitmap(compressBitmap.PixelWidth, compressBitmap.PixelHeight);
			Color targetColor = Common.getTargetColor(e, inkCanvas, compressBitmap);

			int tempR = targetColor.R;
			Color tempColor = setDifferentColor(ref targetColor, ref tempR);

			keepSelectedArea(e, inkCanvas, clickedLayer, compressBitmap, tempBitmap, outputBitmap, ref targetColor, ref tempColor);

			changeStrokeToOriginalWidth(inkCanvas);
		}

		private static void changeStrokeToThinnerWidth(InkPresenter inkCanvas)
		{
			for (int i = 0; i < inkCanvas.Strokes.Count; i++)
			{
				inkCanvas.Strokes[i].DrawingAttributes.Height = inkCanvas.Strokes[i].DrawingAttributes.Width = 1;
			}
		}

		private static void changeStrokeToOriginalWidth(InkPresenter inkCanvas)
		{
			for (int i = 0; i < inkCanvas.Strokes.Count; i++)
			{
				inkCanvas.Strokes[i].DrawingAttributes.Height = inkCanvas.Strokes[i].DrawingAttributes.Width = 5;
			}
		}

		private static void keepSelectedArea(MouseButtonEventArgs e, InkPresenter inkCanvas, int clickedLayer, WriteableBitmap compressBitmap, WriteableBitmap tempBitmap, WriteableBitmap outputBitmap, ref Color targetColor, ref Color tempColor)
		{
			Fill.floodFill(new Point((int)e.GetPosition(inkCanvas).X, (int)e.GetPosition(inkCanvas).Y), targetColor, tempColor, compressBitmap, tempBitmap);
			Fill.floodFill(new Point((int)e.GetPosition(inkCanvas).X, (int)e.GetPosition(inkCanvas).Y), tempColor, targetColor, tempBitmap, outputBitmap);

			MainPage.layerList[clickedLayer].wb = outputBitmap;
			MainPage.layerList[clickedLayer].img.Source = outputBitmap;
		}

		private static Color setDifferentColor(ref Color targetColor, ref int tempR)
		{
			if (targetColor.R + 50 >= 240)
			{
				tempR += 50;
			}
			else
			{
				tempR -= 50;
			}

			Color tempColor = Color.FromArgb(255, (byte)tempR, 255, 255);
			return tempColor;
		}
	}
}
