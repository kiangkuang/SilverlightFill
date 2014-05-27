using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
			Common.hitTestLayer(e, inkCanvas);
			Common.convertToBitmap(inkCanvas);
			Color targetColor = Common.getTargetColor(e, inkCanvas);

			Common.floodfill(new Point((int)e.GetPosition(MainPage.presenterList[Common.clickedLayer]).X, (int)e.GetPosition(MainPage.presenterList[Common.clickedLayer]).Y), targetColor, selectedColor, MainPage.presenterList[Common.clickedLayer]);

		}
	}
}
