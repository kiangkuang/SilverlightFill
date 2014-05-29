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
		public static Point clickedPos;

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

		public static WriteableBitmap convertToBitmap(InkPresenter inkCanvas)
		{
			InkPresenter temp = new InkPresenter();
			for (int i = 0; i < MainPage.presenterList.Count; i++)
			{
				for (int j = 0; j < MainPage.presenterList[i].Strokes.Count; j++)
				{
					temp.Strokes.Add(MainPage.presenterList[i].Strokes[j]);
				}
			}
			for (int i = 0; i < inkCanvas.Strokes.Count; i++)
			{
				temp.Strokes.Add(inkCanvas.Strokes[i]);
			}

			WriteableBitmap wb = new WriteableBitmap((int)inkCanvas.ActualWidth, (int)inkCanvas.ActualHeight);
			wb.Render(temp, new TranslateTransform());
			wb.Invalidate();
			return wb;
		}

		public static Color getTargetColor(MouseButtonEventArgs e, InkPresenter inkCanvas, WriteableBitmap wb)
		{
			return wb.GetPixel((int)e.GetPosition(inkCanvas).X, (int)e.GetPosition(inkCanvas).Y);
		}

		public static int hitTestLayer(MouseButtonEventArgs e, InkPresenter inkCanvas)
		{
			clickedPos = new Point(e.GetPosition(inkCanvas).X, e.GetPosition(inkCanvas).Y);

			for (int i = MainPage.presenterList.Count - 1; i >= 0; i--)
			{
				StylusPointCollection targetedStylusPoint = new StylusPointCollection();
				Point hitTestPos = new Point(e.GetPosition(MainPage.presenterList[i]).X, e.GetPosition(MainPage.presenterList[i]).Y);
				targetedStylusPoint.Add(new StylusPoint(hitTestPos.X, hitTestPos.Y));

				if (MainPage.presenterList[i].Strokes.HitTest(targetedStylusPoint).Count > 0)
				{
					return i;
				}
			}
			return -1;
		}
	}
}
