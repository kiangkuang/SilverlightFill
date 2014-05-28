using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SilverlightFill
{
	public class Merge
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
			WriteableBitmap wb = Common.convertToBitmap(inkCanvas);
			Color targetColor = Common.getTargetColor(e, inkCanvas, wb);
			if (clickedLayer != -1)
			{
				Fill.floodFill(new Point((int)e.GetPosition(MainPage.presenterList[clickedLayer]).X, (int)e.GetPosition(MainPage.presenterList[clickedLayer]).Y), targetColor, selectedColor, MainPage.presenterList[clickedLayer], wb);
			}
		}
	}
}
