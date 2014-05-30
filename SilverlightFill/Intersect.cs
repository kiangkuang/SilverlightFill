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
			if (clickedLayer != -1)
			{
				WriteableBitmap wb = Common.convertToBitmap(inkCanvas);
				Color replacementColor = Common.getTargetColor(e, inkCanvas, wb);

				MainPage.presenterList[clickedLayer].Strokes.Clear();
				wb = Common.convertToBitmap(inkCanvas);
				Color targetColor = Common.getTargetColor(e, inkCanvas, wb);
				Fill.floodFill(new Point((int)e.GetPosition(MainPage.presenterList[clickedLayer]).X, (int)e.GetPosition(MainPage.presenterList[clickedLayer]).Y), targetColor, replacementColor, MainPage.presenterList[clickedLayer], wb);
			}
		}
	}
}
